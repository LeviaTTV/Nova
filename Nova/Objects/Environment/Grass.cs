using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Sprite;
using Nova.Environment;

namespace Nova.Objects.Environment
{
    public class Grass : FoliageGameObject
    {
        private Sprite _sprite;

        public Grass(GameServiceContainer services, Tile tile)
            : base(services, tile)
        {
        }

        public override void LoadContent(ContentManager contentManager)
        {
            var sheet = contentManager.Load<SpriteSheet>("Environment/Terrain/BaseTerrain");

            string[] potentialGrass = Tile.TileType switch
            {
                TileType.LightGrass => new[] { "49", "50", "53", "54", "55", "56" },
                TileType.Grass => new[] { "49", "50", "53", "54", "55", "56" },
                TileType.DeadGrass => new[] { "51", "52" },
                TileType.Sand => new[] { "51", "52" },
                TileType.Gravel => new[] { "51", "52" },
                _ => null
            };

            var rand = new Random(Guid.NewGuid().GetHashCode());

            _sprite = sheet[potentialGrass[rand.Next(0, potentialGrass.Length)]];
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var position = new Vector2(Tile.X * 32, Tile.Y * 32);

            _sprite.Draw(spriteBatch, position, layerDepth: 0.9f);
        }
    }
}
