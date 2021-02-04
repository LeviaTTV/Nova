﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Extensions;
using Nova.Common.Noise;
using Nova.Common.Sprite;
using Nova.Environment.Foliage;
using Nova.Objects;

namespace Nova.Environment
{
    public class MapGenerator
    {
        private readonly GraphicsDevice _device;
        private readonly int _seed;
        private readonly ContentManager _contentManager;

        private TileMappings _tileMappings;

        private readonly Dictionary<string, SpriteSheet> _tileMappingSpriteSheets = new();
        private readonly List<TileTransitionMask> _tileTransitionMasks = new();
        private readonly List<SpriteTransition> _spriteTransitions = new();
        private FoliageMappings _foliageMappings;

        private readonly List<GameObject> _gameObjects = new List<GameObject>();

        public MapGenerator(GraphicsDevice device, int seed, ContentManager contentManager)
        {
            _device = device;
            _seed = seed;
            _contentManager = contentManager;
        }

        public Map Generate(int width, int height)
        {
            var map = new Map()
            {
                Width = width,
                Height = height,
                TileWidth = 32,
                TileHeight = 32
            };

            _tileMappings = _contentManager.LoadObject<TileMappings>("TileMappings");
            _foliageMappings = _contentManager.LoadObject<FoliageMappings>("FoliageMappings");

            PreloadSpriteSheets();
            PreloadTransitionMasks();

            foreach (var mapping in _tileMappings.Mappings)
            {
                _spriteTransitions.Add(GenerateTileTransitions(mapping));
            }

            EnvironmentPass(map);
            TransitionDetectionPass(map);
            TransitionSelectionPass(map);

            FoliagePass(map);

            map.GameObjects = _gameObjects;
            return map;
        }

        private void FoliagePass(Map map)
        {
            var noiseFunction = new FastNoiseLite(_seed);
            noiseFunction.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            var rand = new Random(Guid.NewGuid().GetHashCode());

            foreach (var foliageMapping in _foliageMappings.Foliage.Where(x => x.NoisePass).OrderByDescending(x => x.NoiseThreshold))
            {
                var eligibleTileTypes = foliageMapping.EligibleTileTypes.Select(x => x.TileType);
                var eligibleTiles = map.Tiles.Where(x => eligibleTileTypes.Contains(x.Value.TileType) && x.Value.Foliage == FoliageType.None);

                foreach (var eligibleTile in eligibleTiles)
                {
                    var noise = noiseFunction.GetNoise(eligibleTile.Key.X, eligibleTile.Key.Y);

                    if (noise > foliageMapping.NoiseThreshold)
                    {
                        int startX = eligibleTile.Key.X - foliageMapping.SameTypeRadius;
                        int endX = eligibleTile.Key.X + foliageMapping.SameTypeRadius;
                        int startY = eligibleTile.Key.Y - foliageMapping.SameTypeRadius;
                        int endY = eligibleTile.Key.Y + foliageMapping.SameTypeRadius;


                        bool allow = true;
                        for (int x = startX; x < endX; x++)
                        {
                            for (int y = startY; y < endY; y++)
                            {
                                if (map.Tiles.TryGetValue(new TileCoordinate(x, y), out var tile))
                                {
                                    if (tile.Foliage == foliageMapping.Type)
                                    {
                                        allow = false;
                                        break;
                                    }
                                }
                            }

                            if (!allow)
                                break;
                        }

                        if (!allow)
                            continue;
                        
                        if (!foliageMapping.AllowOnTransition && eligibleTile.Value.TileBlending != 0)
                            continue;

                        var eligibleTileType = foliageMapping.EligibleTileTypes.FirstOrDefault(x => x.TileType == eligibleTile.Value.TileType);
                        if (eligibleTileType == null)
                            continue;

                        if (eligibleTileType.Reduce > 0)
                        {
                            // Reduce randomly by this percentage.
                            if (rand.NextDouble() < eligibleTileType.Reduce)
                                continue;
                        }

                        eligibleTile.Value.Foliage = foliageMapping.Type;

                        if (foliageMapping.Sprite?.RandomSprite != null)
                            eligibleTile.Value.Variant = rand.Next(0, foliageMapping.Sprite.RandomSprite.Length);

                        if (foliageMapping.GameObject != null)
                        {
                            var gameObject = (FoliageGameObject)Activator.CreateInstance(Type.GetType(foliageMapping.GameObject), new object[]
                            {
                                _device,
                                foliageMapping.Type,
                                eligibleTile.Value
                            });

                            _gameObjects.Add(gameObject);

                            eligibleTile.Value.GameObject = true;
                        }
                    }
                }
            }

            var disc = new PoissonDiscSampler(map.Width, map.Height, 2);

            foreach (var loc in disc.Samples())
            {
                var tile = map.Tiles[new TileCoordinate((int)loc.X, (int)loc.Y)];
                if (tile.Foliage != FoliageType.None)
                    continue;

                foreach (var foliageMapping in _foliageMappings.Foliage.Where(x => x.PoissonPass && x.EligibleTileTypes.Any(z => z.TileType == tile.TileType)).OrderByDescending(x => x.PoissonThreshold))
                {
                    var eligibleTileType = foliageMapping.EligibleTileTypes.FirstOrDefault(x => x.TileType == tile.TileType);

                    if (rand.NextDouble() < foliageMapping.PoissonThreshold || rand.NextDouble() < eligibleTileType.Reduce)
                        continue;

                    int startX = tile.X - foliageMapping.SameTypeRadius;
                    int endX = tile.X + foliageMapping.SameTypeRadius;
                    int startY = tile.Y - foliageMapping.SameTypeRadius;
                    int endY = tile.Y + foliageMapping.SameTypeRadius;


                    bool allow = true;
                    for (int x = startX; x < endX; x++)
                    {
                        for (int y = startY; y < endY; y++)
                        {
                            if (map.Tiles.TryGetValue(new TileCoordinate(x, y), out var testTile))
                            {
                                if (testTile.Foliage == foliageMapping.Type)
                                {
                                    allow = false;
                                    break;
                                }
                            }
                        }

                        if (!allow)
                            break;
                    }

                    if (!allow)
                        continue;

                    if (!foliageMapping.AllowOnTransition && tile.TileBlending != 0)
                        continue;

                    tile.Foliage = foliageMapping.Type;

                    if (foliageMapping.Sprite?.RandomSprite != null)
                        tile.Variant = rand.Next(0, foliageMapping.Sprite.RandomSprite.Length);

                    if (foliageMapping.GameObject != null)
                    {
                        var gameObject = (FoliageGameObject)Activator.CreateInstance(Type.GetType(foliageMapping.GameObject), new object[]
                        {
                            _device,
                            foliageMapping.Type,
                            tile
                        });

                        _gameObjects.Add(gameObject);

                        tile.GameObject = true;
                    }


                    break;
                }
            }
        }

        private void PreloadSpriteSheets()
        {
            var list = _tileMappings.Mappings.Select(x => x.Sheet).ToList();
            list.AddRange(_foliageMappings.Foliage.Where(x => x.Sprite != null).Select(x => x.Sprite.Sheet));

            foreach (var uniqueSheet in list.Distinct())
            {
                var sheet = _contentManager.Load<SpriteSheet>(uniqueSheet);

                _tileMappingSpriteSheets[uniqueSheet] = sheet;
            }
        }

        private void PreloadTransitionMasks()
        {
            foreach (var tileTransition in _tileMappings.TransitionMasks)
            {
                var tileTransitionMask = new TileTransitionMask();

                foreach (var variation in tileTransition.Variations)
                {
                    var texture = _contentManager.Load<Texture2D>("TileTransitionMasks/" + variation);

                    var pixelData = new Color[texture.Width * texture.Height];
                    texture.GetData(pixelData);

                    tileTransitionMask.TextureVariations.Add(new TileTransitionVariation()
                    {
                        Texture = texture,
                        PixelData = pixelData
                    });
                }

                tileTransitionMask.TileBlending = 0;
                foreach (var flag in tileTransition.Sides)
                    tileTransitionMask.TileBlending |= flag;


                _tileTransitionMasks.Add(tileTransitionMask);
            }
        }

        private SpriteTransition GenerateTileTransitions(TileMapping mapping)
        {
            var sprite = _tileMappingSpriteSheets[mapping.Sheet][mapping.Sprite];
            var spriteData = new Color[sprite.Width * sprite.Height];
            sprite.Texture.GetData(0, sprite.SourceRectangle, spriteData, 0, spriteData.Length);

            var spriteTransition = new SpriteTransition()
            {
                Sprite = sprite,
                TileType = mapping.TileType
            };

            foreach (var mask in _tileTransitionMasks)
            {
                var transition = new SpriteGeneratedTransition()
                {
                    TileBlending = mask.TileBlending
                };

                foreach (var variation in mask.TextureVariations)
                {
                    var blendedTexture = new Texture2D(_device, sprite.Width, sprite.Height);
                    var blendedData = new Color[blendedTexture.Width * blendedTexture.Height];

                    int count = 0;
                    for (int y = 0; y < blendedTexture.Height; y++)
                    {
                        for (int x = 0; x < blendedTexture.Width; x++)
                        {
                            var spriteColor = spriteData[count];
                            blendedData[count] = Color.FromNonPremultiplied(spriteColor.R, spriteColor.G, spriteColor.B, variation.PixelData[count].A);

                            ++count;
                        }
                    }

                    blendedTexture.SetData(blendedData);
                    transition.Variations.Add(blendedTexture);
                }

                spriteTransition.Transitions.Add(transition);
            }

            return spriteTransition;
        }

        private void EnvironmentPass(Map map)
        {
            var noise = new PerlinNoise();
            map.MapData = noise.Generate(_seed, map.Width, map.Height);

            map.Tiles = new Dictionary<TileCoordinate, Tile>();

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    float mapData = map.MapData[x, y];

                    var type = TileType.None;

                    var mapping = _tileMappings.Mappings.FirstOrDefault(z => mapData >= z.LowerLimit && mapData < z.UpperLimit);
                    if (mapping != null)
                        type = mapping.TileType;

                    map.Tiles[new TileCoordinate(x, y)] = new Tile()
                    {
                        TileType = type,
                        X = x,
                        Y = y
                    };
                }
            }
        }

        private void TransitionDetectionPass(Map map)
        {
            var od = Enum.GetValues(typeof(TileType)).Cast<int>();
            var ordered = od.Where(x => x != (int)TileType.None).OrderByDescending(x => x);

            foreach (var tileTypeToProcess in ordered)
            {
                var tileType = (TileType)tileTypeToProcess;

                var tiles = map.Tiles.Where(x => x.Value.TileType == tileType);

                foreach (var tileEntry in tiles)
                {
                    int x = tileEntry.Key.X;
                    int y = tileEntry.Key.Y;
                    var tile = tileEntry.Value;
                    
                    var topTileKey = new TileCoordinate(x, y - 1);
                    if (map.Tiles.TryGetValue(topTileKey, out var topTile))
                    {
                        if (topTile.TileType != tile.TileType && !topTile.TileBlending.HasFlag(TileBlending.South))
                        {
                            tile.TileBlending |= TileBlending.North;
                        }
                    }

                    var bottomTileKey = new TileCoordinate(x, y + 1);
                    if (map.Tiles.TryGetValue(bottomTileKey, out var bottomTile))
                    {
                        if (bottomTile.TileType != tile.TileType && !bottomTile.TileBlending.HasFlag(TileBlending.North))
                        {
                            tile.TileBlending |= TileBlending.South;
                        }
                    }

                    var eastTileKey = new TileCoordinate(x + 1, y);
                    if (map.Tiles.TryGetValue(eastTileKey, out var eastTile))
                    {
                        if (eastTile.TileType != tile.TileType && !eastTile.TileBlending.HasFlag(TileBlending.West))
                        {
                            tile.TileBlending |= TileBlending.East;
                        }
                    }

                    var westTileKey = new TileCoordinate(x - 1, y);
                    if (map.Tiles.TryGetValue(westTileKey, out var westTile))
                    {
                        if (westTile.TileType != tile.TileType && !westTile.TileBlending.HasFlag(TileBlending.East))
                        {
                            tile.TileBlending |= TileBlending.West;
                        }
                    }
                }

                // Do another pass for others for the corners
                foreach (var tileEntry in tiles)
                {
                    int x = tileEntry.Key.X;
                    int y = tileEntry.Key.Y;
                    var tile = tileEntry.Value;

                    // Needs a direct connection to same tile type.
                    var testArray = new TileCoordinate[4]
                    {
                        new TileCoordinate(x, y - 1),
                        new TileCoordinate(x, y + 1),
                        new TileCoordinate(x - 1, y),
                        new TileCoordinate(x + 1, y)
                    };

                    int count = 0;
                    foreach (var testTileCoordinate in testArray)
                    {
                        if (map.Tiles.TryGetValue(testTileCoordinate, out var testTile))
                        {
                            if (testTile.TileType == tile.TileType && testTile.TileBlending != 0)
                                ++count;
                        }
                    }

                    if (count < 2)
                        continue;

                    var northEastTileKey = new TileCoordinate(x + 1, y - 1);
                    if (map.Tiles.TryGetValue(northEastTileKey, out var northEastTile))
                    {
                        if (northEastTile.TileType != tile.TileType)
                            tile.TileBlending |= TileBlending.NorthEast;
                    }


                    var southEastTileKey = new TileCoordinate(x + 1, y + 1);
                    if (map.Tiles.TryGetValue(southEastTileKey, out var southEastTile))
                    {
                        if (southEastTile.TileType != tile.TileType)
                            tile.TileBlending |= TileBlending.SouthEast;
                    }


                    var southWestTileKey = new TileCoordinate(x - 1, y + 1);
                    if (map.Tiles.TryGetValue(southWestTileKey, out var southWestTile))
                    {
                        if (southWestTile.TileType != tile.TileType)
                            tile.TileBlending |= TileBlending.SouthWest;
                    }


                    var northWestTileKey = new TileCoordinate(x - 1, y - 1);
                    if (map.Tiles.TryGetValue(northWestTileKey, out var northWestTile))
                    {
                        if (northWestTile.TileType != tile.TileType)
                            tile.TileBlending |= TileBlending.NorthWest;
                    }
                }
            }
        }

        private void TransitionSelectionPass(Map map)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    var tile = map.Tiles[new TileCoordinate(x, y)];
                    if (tile.TileBlending == 0)
                        continue;

                    var tileType = TileType.None;

                    if (tile.TileBlending.HasFlag(TileBlending.North))
                    {
                        tileType = map.Tiles[new TileCoordinate(x, y - 1)].TileType;
                    }
                    else if (tile.TileBlending.HasFlag(TileBlending.East))
                    {
                        tileType = map.Tiles[new TileCoordinate(x + 1, y)].TileType;
                    }
                    else if (tile.TileBlending.HasFlag(TileBlending.West))
                    {
                        tileType = map.Tiles[new TileCoordinate(x - 1, y)].TileType;
                    }
                    else if (tile.TileBlending.HasFlag(TileBlending.South))
                    {
                        tileType = map.Tiles[new TileCoordinate(x, y + 1)].TileType;
                    }

                    else if (tile.TileBlending.HasFlag(TileBlending.NorthEast))
                    {
                        tileType = map.Tiles[new TileCoordinate(x + 1, y - 1)].TileType;
                    }
                    else if (tile.TileBlending.HasFlag(TileBlending.NorthWest))
                    {
                        tileType = map.Tiles[new TileCoordinate(x - 1, y - 1)].TileType;
                    }
                    else if (tile.TileBlending.HasFlag(TileBlending.SouthEast))
                    {
                        tileType = map.Tiles[new TileCoordinate(x + 1, y + 1)].TileType;
                    }
                    else if (tile.TileBlending.HasFlag(TileBlending.SouthWest))
                    {
                        tileType = map.Tiles[new TileCoordinate(x - 1, y + 1)].TileType;
                    }

                    var spriteTransition = _spriteTransitions.FirstOrDefault(x => x.TileType == tileType);

                    if (tile.TileBlending.HasFlag(TileBlending.North))
                    {
                        var selectedTransition = spriteTransition.Transitions.FirstOrDefault(x => x.TileBlending == TileBlending.North);

                        if (selectedTransition != null)
                            tile.BlendTextureList.Add(selectedTransition.Variations[random.Next(0, selectedTransition.Variations.Count)]);
                    }
                    if (tile.TileBlending.HasFlag(TileBlending.East))
                    {
                        var selectedTransition = spriteTransition.Transitions.FirstOrDefault(x => x.TileBlending == TileBlending.East);

                        if (selectedTransition != null)
                            tile.BlendTextureList.Add(selectedTransition.Variations[random.Next(0, selectedTransition.Variations.Count)]);
                    }
                    if (tile.TileBlending.HasFlag(TileBlending.West))
                    {
                        var selectedTransition = spriteTransition.Transitions.FirstOrDefault(x => x.TileBlending == TileBlending.West);

                        if (selectedTransition != null)
                            tile.BlendTextureList.Add(selectedTransition.Variations[random.Next(0, selectedTransition.Variations.Count)]);
                    }
                    if (tile.TileBlending.HasFlag(TileBlending.South))
                    {
                        var selectedTransition = spriteTransition.Transitions.FirstOrDefault(x => x.TileBlending == TileBlending.South);

                        if (selectedTransition != null)
                            tile.BlendTextureList.Add(selectedTransition.Variations[random.Next(0, selectedTransition.Variations.Count)]);
                    }




                    if (tile.TileBlending.HasFlag(TileBlending.NorthEast))
                    {
                        var selectedTransition = spriteTransition.Transitions.FirstOrDefault(x => x.TileBlending == TileBlending.NorthEast);

                        if (selectedTransition != null)
                            tile.BlendTextureList.Add(selectedTransition.Variations.FirstOrDefault());
                    }
                    if (tile.TileBlending.HasFlag(TileBlending.NorthWest))
                    {
                        var selectedTransition = spriteTransition.Transitions.FirstOrDefault(x => x.TileBlending == TileBlending.NorthWest);

                        if (selectedTransition != null)
                            tile.BlendTextureList.Add(selectedTransition.Variations.FirstOrDefault());
                    }
                    if (tile.TileBlending.HasFlag(TileBlending.SouthEast))
                    {
                        var selectedTransition = spriteTransition.Transitions.FirstOrDefault(x => x.TileBlending == TileBlending.SouthEast);

                        if (selectedTransition != null)
                            tile.BlendTextureList.Add(selectedTransition.Variations.FirstOrDefault());
                    }
                    if (tile.TileBlending.HasFlag(TileBlending.SouthWest))
                    {
                        var selectedTransition = spriteTransition.Transitions.FirstOrDefault(x => x.TileBlending == TileBlending.SouthWest);

                        if (selectedTransition != null)
                            tile.BlendTextureList.Add(selectedTransition.Variations.FirstOrDefault());
                    }
                }
            }
        }
    }
}