using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Primitives;
using Nova.Common.Sprite;
using Nova.Environment;
using Nova.Environment.Foliage;

namespace Nova.Objects.Environment
{
    public class Tree : FoliageGameObject
    {
        private Sprite _sprite;

        private Rectangle _destinationRectangle;
        private Vector2 _origin;

        public Tree(GraphicsDevice graphicsDevice, Tile tile)
            : base(graphicsDevice, tile)
        {
        }

        public override void LoadContent(ContentManager contentManager)
        {
            string name = Tile.TileType switch
            {
                TileType.Sand => "Dead",
                TileType.LightGrass => "Green",
                TileType.Grass => "Pale",
                TileType.DeadGrass => "Brown",
                TileType.Gravel => "Dead",
                TileType.Mountain => "Dead",
                _ => null
            };

            var treesSheet = contentManager.Load<SpriteSheet>("Environment/Trees/Trees" + name);


            var rand = new Random(Guid.NewGuid().GetHashCode());

            var exclude = new string[]
            {
                "713",
                "722",
                "730",
                "897",
                "730",
                "544",
                "456",
                "117",
                "251",
                "24",
                "68",
                "69",
                "70",
                "71",
                "72",
                "73",
                "74",
                "65",
                "66",
                "67",
                "1",
                "3",
                "5",
                "7",
                "9"
            };
            var potentialTrees = treesSheet.Sprites.Where(x => !exclude.Contains(x.Key)).ToList();
            _sprite = potentialTrees[rand.Next(0, potentialTrees.Count)].Value;


            // Precalculate position and origin
            var position = new Vector2(Tile.X * 32, Tile.Y * 32);
            _destinationRectangle = new Rectangle((int) position.X, (int) position.Y, (int) _sprite.Width, (int) _sprite.Height);
            _origin = new Vector2(_sprite.Width / 2f - 16, _sprite.Height - 32);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite.Texture, _destinationRectangle, _sprite.SourceRectangle, Color.White, 0f, _origin, SpriteEffects.None, 0);
        }
    }
}
