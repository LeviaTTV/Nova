using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Sprite;
using Nova.Environment;
using Nova.Environment.Foliage;

namespace Nova.Objects.Environment
{
    public class Shrub : FoliageGameObject
    {
        private Sprite _sprite;

        public Shrub(GraphicsDevice graphicsDevice, Tile tile)
            : base(graphicsDevice, tile)
        {
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

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var position = new Vector2(Tile.X * 32, Tile.Y * 32);

            _sprite.Draw(spriteBatch, position);
        }
    }
}
