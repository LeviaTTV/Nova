using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Extensions;
using Nova.Common.Primitives;
using Nova.Common.Sprite;

namespace Nova.Environment
{
    public class MapRenderer
    {
        private readonly GraphicsDevice _device;
        private readonly Map _map;
        private readonly Camera2D _camera2D;
        private PrimitiveRectangle _positionRectangle;
        private PrimitiveLine _line;

        private readonly Dictionary<TileType, Sprite> _tileMappings = new();

        public MapRenderer(GameServiceContainer services, Map map)
        {
            _map = map;
            _device = services.GetService<GraphicsDevice>();
            _camera2D = services.GetService<Camera2D>();
        }

        public void LoadContent(ContentManager content)
        {
            _positionRectangle = new PrimitiveRectangle(_device, Color.Red);

            var dict = new Dictionary<string, SpriteSheet>();


            var tileMappings = content.LoadObject<TileMappings>("Settings/TileMappings");

            var sheetsToLoad = tileMappings.Mappings.Select(x => x.Sheet).ToList();
            foreach (var sheetToLoad in sheetsToLoad.Distinct())
            {
                var sheet = content.Load<SpriteSheet>(sheetToLoad);
                dict[sheetToLoad] = sheet;
            }

            foreach (var spriteMapping in tileMappings.Mappings)
            {
                _tileMappings[spriteMapping.TileType] = dict[spriteMapping.Sheet][spriteMapping.Sprite];
            }

            _line = new PrimitiveLine(_device, Color.Red);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var rectBounds = _camera2D.VisibleArea;
            rectBounds.X -= 32;
            rectBounds.Y -= 32;
            rectBounds.Width += 32;
            rectBounds.Height += 32;
            
            int endY = rectBounds.Y + rectBounds.Height;
            int endX = rectBounds.X + rectBounds.Width;

            
            foreach (var tile in _map.Tiles)
            {
                if (tile.Key.X * 32 >= rectBounds.X && tile.Key.Y * 32 >= rectBounds.Y && tile.Key.X * 32 <= endX && tile.Key.Y * 32 <= endY)
                {
                    DrawTile(spriteBatch, _camera2D.Position, tile.Value);

                    // Bail out because we know the next tiles are definitely going to be out of bounds
                    if (tile.Key.Y * 32 + 32 >= endY && tile.Key.X * 32 + 32 >= endX)
                        break;
                }
            }
        }
        
        private void DrawTile(SpriteBatch spriteBatch, Vector2 cameraPos, Tile tile)
        {
            var sprite = _tileMappings[tile.TileType];
            var position = new Vector2(tile.X * _map.TileWidth, tile.Y * _map.TileHeight);

            
            sprite.Draw(spriteBatch, position, layerDepth: 0.99f);

            if (tile.TileBlending != 0)
            {
                if (!DebugTools.DoNotRenderTileTransitions && tile.BlendTextureList.Any())
                {
                    foreach (var blendTexture in tile.BlendTextureList)
                        spriteBatch.Draw(blendTexture, position, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.95f);
                }

                if (DebugTools.GenericDebugEnabled)
                {
                    if (tile.TileBlending.HasFlag(TileBlending.North))
                    {
                        _positionRectangle.Draw(spriteBatch, position + new Vector2(16, 2), 1, 1);
                    }

                    if (tile.TileBlending.HasFlag(TileBlending.East))
                    {
                        _positionRectangle.Draw(spriteBatch, position + new Vector2(30, 16), 1, 1);
                    }

                    if (tile.TileBlending.HasFlag(TileBlending.South))
                    {
                        _positionRectangle.Draw(spriteBatch, position + new Vector2(16, 30), 1, 1);
                    }

                    if (tile.TileBlending.HasFlag(TileBlending.West))
                    {
                        _positionRectangle.Draw(spriteBatch, position + new Vector2(2, 16), 1, 1);
                    }

                    if (tile.TileBlending.HasFlag(TileBlending.NorthEast))
                    {
                        _positionRectangle.Draw(spriteBatch, position + new Vector2(30, 2), 1, 1);
                    }

                    if (tile.TileBlending.HasFlag(TileBlending.NorthWest))
                    {
                        _positionRectangle.Draw(spriteBatch, position + new Vector2(2, 2), 1, 1);
                    }

                    if (tile.TileBlending.HasFlag(TileBlending.SouthEast))
                    {
                        _positionRectangle.Draw(spriteBatch, position + new Vector2(30, 30), 1, 1);
                    }

                    if (tile.TileBlending.HasFlag(TileBlending.SouthWest))
                    {
                        _positionRectangle.Draw(spriteBatch, position + new Vector2(2, 30), 1, 1);
                    }

                    _line.Draw(spriteBatch, position, position + new Vector2(32, 0), 1f, Color.Red);
                    _line.Draw(spriteBatch, position, position + new Vector2(0, 32), 1f, Color.Red);
                    _line.Draw(spriteBatch, position + new Vector2(0, 32), position + new Vector2(32, 32), 1f, Color.Red);
                    _line.Draw(spriteBatch, position + new Vector2(32, 0), position + new Vector2(32, 32), 1f, Color.Red);
                }
            }
        }
    }
}
