using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Primitives;
using Nova.Common.Sprite;
using Nova.Environment;
using Nova.Objects.Character;

namespace Nova.Objects.Environment
{
    public class Tree : FoliageGameObject
    {
        private Sprite _sprite;

        private Rectangle _destinationRectangle;
        private Vector2 _origin;
        private PlayerCharacter _playerCharacter;
        private PrimitiveRectangle _rectangle;
        private PrimitiveRectangle _rectangle2;
        private PrimitiveRectangle _rectangle3;

        public Tree(GameServiceContainer services, Tile tile)
            : base(services, tile)
        {
            _playerCharacter = services.GetService<PlayerCharacter>();
        }

        public override bool CollisionEnabled => true;

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

            _rectangle = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.Red, false);
            _rectangle2 = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.Yellow, false);
            _rectangle3 = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.Green, false);

            Width = _sprite.Width;
            Height = _sprite.Height;

            VisualBounds = new Rectangle((int)(position.X - _origin.X), (int)(position.Y - _origin.Y), Width, Height);


            var width = (int)(_sprite.Width / 2f);
            CollisionBounds = new Rectangle(VisualBounds.X + VisualBounds.Width / 2 - width / 2, (int)position.Y - 4, width, 36); //32
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float z = 0.7f;
            if (Tile.Y * 32 + 32 > _playerCharacter.Position.Y + 32)
            {
                z = 0.4f;
            }

            z -= Tile.Y * 0.00001f + Tile.X * 0.00001f;
            spriteBatch.Draw(_sprite.Texture, _destinationRectangle, _sprite.SourceRectangle, Color.White, 0f, _origin, SpriteEffects.None, z);

            if (DebugTools.GenericDebugEnabled)
            {
                _rectangle.Draw(spriteBatch, new Vector2(_destinationRectangle.X, _destinationRectangle.Y), 32, 32);

                //_rectangle2.Draw(spriteBatch, new Vector2(VisualBounds.X, VisualBounds.Y), VisualBounds.Width, VisualBounds.Height);
                _rectangle2.Draw(spriteBatch, VisualBounds);
                _rectangle3.Draw(spriteBatch, CollisionBounds);

            }
        }
    }
}
