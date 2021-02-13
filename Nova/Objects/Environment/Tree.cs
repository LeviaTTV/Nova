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
        private readonly PlayerCharacter _playerCharacter;
        private PrimitiveRectangle _rectangle;
        private PrimitiveRectangle _rectangle2;
        private PrimitiveRectangle _rectangle3;
        private PrimitiveRectangle _rectangle4;

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
            _destinationRectangle = new Rectangle((int) Position.X, (int)Position.Y, (int) _sprite.Width, (int) _sprite.Height);
            Origin = new Vector2(_sprite.Width / 2f - 16, _sprite.Height - 32);

            _rectangle = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.Red, false);
            _rectangle2 = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.Yellow, false);
            _rectangle3 = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.Green, false);
            _rectangle4 = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.SkyBlue, false);

            Width = _sprite.Width;
            Height = _sprite.Height;

            VisualBounds = new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), Width, Height);


            var width = (int)(_sprite.Width / 2f);
            CollisionBounds = new Rectangle(VisualBounds.X + VisualBounds.Width / 2 - width / 2, (int)Position.Y - 4, width, 36); //32
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite.Texture, _destinationRectangle, _sprite.SourceRectangle, Color.White, 0f, Origin, SpriteEffects.None, 0f);

            if (DebugTools.GenericDebugEnabled)
            {
                _rectangle.Draw(spriteBatch, new Vector2(_destinationRectangle.X, _destinationRectangle.Y), 32, 32);
                _rectangle2.Draw(spriteBatch, VisualBounds);
                _rectangle3.Draw(spriteBatch, CollisionBounds);
                _rectangle4.Draw(spriteBatch, Position, 4, 4);
            }
        }
    }
}
