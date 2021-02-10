using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nova.Common.Sprite
{
    public class SpriteObject : IDrawable
    {
        public Texture2D Texture { get; set; }
        public Rectangle SourceRectangle { get; set; }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation = default(float), Vector2 origin = default(Vector2), SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0f)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)position.X, (int)position.Y, SourceRectangle.Width, SourceRectangle.Height), SourceRectangle, Color.White, rotation, origin, spriteEffects, layerDepth);
        }
    }
}