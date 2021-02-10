using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Primitives;
using Nova.Common.Sprite;
using Nova.Environment;
using Nova.Objects.Character;

namespace Nova.Objects.Environment
{
    public class Shrub : FoliageGameObject
    {
        private Sprite _sprite;
        private PlayerCharacter _playerCharacter;

        private PrimitiveRectangle _rectangle;

        public Shrub(GameServiceContainer services, Tile tile)
            : base(services, tile)
        {
            _playerCharacter = services.GetService<PlayerCharacter>();
        }

        public override void LoadContent(ContentManager contentManager)
        {
            var sheet = contentManager.Load<SpriteSheet>("Environment/Plants/FoliagePlants");

            string[] potentialShrubs = Tile.TileType switch
            {
                TileType.LightGrass => new []{ "359", "361", "363" },
                TileType.Grass => new[] { "391", "393", "395", "413" },
                TileType.DeadGrass => new [] { "365", "327" },
                TileType.Sand => new [] { "365", "327" },
                TileType.Gravel => new[] { "365", "327" },
                _ => null
            };

            var rand = new Random(Guid.NewGuid().GetHashCode());

            _sprite = sheet[potentialShrubs[rand.Next(0, potentialShrubs.Length)]];


            Width = _sprite.Width;
            Height = _sprite.Height;

            VisualBounds = new Rectangle((int)Tile.X * 32, (int)Tile.Y * 32, Width, Height);

            _rectangle = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.DarkGreen, false);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var position = new Vector2(Tile.X * 32, Tile.Y * 32);
            
            float z = 0.7f;
            if (Tile.Y * 32 + 64 > _playerCharacter.Position.Y + 32 && _playerCharacter.CollisionBounds.Intersects(VisualBounds))
            {
                z = 0.4f;
            }

            z -= Tile.Y * 0.00001f + Tile.X * 0.00001f;
            _sprite.Draw(spriteBatch, position, layerDepth: z);

            if (DebugTools.GenericDebugEnabled)
                _rectangle.Draw(spriteBatch, VisualBounds);
        }
    }
}
