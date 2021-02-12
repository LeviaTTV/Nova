using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nova.Common.Primitives
{
    public class PrimitiveLine
    {
        private readonly GraphicsDevice _device;
        private readonly Color _color;

        private Texture2D _texture;

        public PrimitiveLine(GraphicsDevice device, Color color)
        {
            _device = device;
            _color = color;

            Initialize();
        }

        private void Initialize()
        {
            _texture = new Texture2D(_device, 1, 1);
            _texture.SetData(new Color[] { Color.White });
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 from, Vector2 to, float scale = 1f, Color color = default(Color))
        {
            if (color == default(Color))
                color = _color;

            spriteBatch.Draw(_texture, from, null, _color,
                (float)Math.Atan2(to.Y - from.Y, to.X - from.X),
                new Vector2(0f, (float)_texture.Height / 2),
                new Vector2(Vector2.Distance(from, to), scale),
                SpriteEffects.None, 0f);
        }
    }
}
