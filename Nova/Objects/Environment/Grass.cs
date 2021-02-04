using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Sprite;
using Nova.Environment;
using Nova.Environment.Foliage;

namespace Nova.Objects.Environment
{
    public class Grass : FoliageGameObject
    {
        private Sprite _sprite;

        public Grass(GraphicsDevice graphicsDevice, FoliageType foliageType, Tile tile)
            : base(graphicsDevice, foliageType, tile)
        {
        }

        public override void LoadContent(ContentManager contentManager)
        {
            
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var position = new Vector2(Tile.X * 32, Tile.Y * 32);

            _sprite.Draw(spriteBatch, position);
        }
    }
}
