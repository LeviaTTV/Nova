using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Extensions;
using Nova.Common.Primitives;
using Nova.Common.Sprite;
using Nova.Objects;

namespace Nova.Environment
{
    public class MapRenderer
    {
        private readonly GraphicsDevice _device;
        private readonly Map _map;
        private readonly Camera2D _camera2D;
        private PrimitiveRectangle _positionRectangle;
        private PrimitiveLine _line;
        private SpriteFont _font;

        public bool DebugMode { get; set; }
        public bool DoNotRenderTransitions { get; set; }

        private Dictionary<TileType, Sprite> _tileMappings = new();

        public MapRenderer(GraphicsDevice device, Map map,Camera2D camera2D)
        {
            _device = device;
            _map = map;
            _camera2D = camera2D;
        }

        public void LoadContent(ContentManager content)
        {
            _font = content.Load<SpriteFont>("DefaultFont");
            _positionRectangle = new PrimitiveRectangle(_device, Color.Red);

            var dict = new Dictionary<string, SpriteSheet>();


            var tileMappings = content.LoadObject<TileMappings>("TileMappings");

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

            var tilesToRender = _map.Tiles.Where(tile => tile.Value.InBounds(_map, rectBounds)).ToList();

            foreach (var tile in tilesToRender)
                DrawTile(spriteBatch, _camera2D.Position, tile.Value);

            foreach (var gameObject in _map.GameObjects)
                DrawGameObject(spriteBatch, gameObject);

            _positionRectangle.Draw(spriteBatch, _camera2D.Position, 5, 5);
        }
        
        private void DrawTile(SpriteBatch spriteBatch, Vector2 cameraPos, Tile tile)
        {
            var sprite = _tileMappings[tile.TileType];
            var position = new Vector2(tile.X * _map.TileWidth, tile.Y * _map.TileHeight);

            
            sprite.Draw(spriteBatch, position);

            if (tile.TileBlending != 0)
            {
                if (!DoNotRenderTransitions && tile.BlendTextureList.Any())
                {
                    foreach (var blendTexture in tile.BlendTextureList)
                        spriteBatch.Draw(blendTexture, position, Color.White);
                }

                if (DebugMode)
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
        
        private void DrawGameObject(SpriteBatch spriteBatch, GameObject gameObject)
        {
            gameObject.Draw(spriteBatch);
        }
    }
}
