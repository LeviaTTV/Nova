using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Sprite;
using Nova.Environment;

namespace Nova.Objects.Environment
{
    public class Mushroom : FoliageGameObject
    {
        private Sprite _sprite;

        public Mushroom(GameServiceContainer services, Tile tile)
            : base(services, tile)
        {
        }

        public override void LoadContent(ContentManager contentManager)
        {
            var sheet = contentManager.Load<SpriteSheet>("Environment/Plants/FoliagePlants");

            string[] potentialShrooms = new[] { "340", "324", "325", "326", "341", "342", "353", "354", "355", "356", "357", "358", "371", "372", "373", "374" };

            var rand = new Random(Guid.NewGuid().GetHashCode());

            _sprite = sheet[potentialShrooms[rand.Next(0, potentialShrooms.Length)]];

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch, Position);
        }
    }
}
