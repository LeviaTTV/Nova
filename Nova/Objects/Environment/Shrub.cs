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
        private readonly PlayerCharacter _playerCharacter;

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
            Origin = new Vector2(_sprite.Width / 2f - 16, _sprite.Height - 32);
            _rectangle = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.DarkGreen, false);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch, Position);

            if (DebugTools.GenericDebugEnabled)
                _rectangle.Draw(spriteBatch, VisualBounds);
        }
    }
}
