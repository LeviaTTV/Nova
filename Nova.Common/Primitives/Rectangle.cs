using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nova.Common.Primitives
{
    public class PrimitiveRectangle
    {
        private readonly GraphicsDevice _device;
        private readonly Color _color;
        
        public bool Fill { get; set; }
        public float BorderWidth { get; set; } = 2f;

        private Texture2D _texture;

        public PrimitiveRectangle(GraphicsDevice device, Color color, bool fill = true)
        {
            _device = device;
            _color = color;
            Fill = fill;

            Initialize();
        }

        private void Initialize()
        {
            _texture = new Texture2D(_device, 1, 1);
            _texture.SetData(new Color[] { Color.White });
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, int width, int height)
        {
            if (Fill)
                spriteBatch.Draw(_texture, new Microsoft.Xna.Framework.Rectangle((int)position.X, (int)position.Y, width, height), _color);
            else
            {
                var from = position;
                var to = position + new Vector2(width, 0);

                spriteBatch.Draw(_texture, from, null, _color,
                    (float)Math.Atan2(to.Y - from.Y, to.X - from.X),
                    new Vector2(0f, (float)_texture.Height / 2),
                    new Vector2(Vector2.Distance(from, to), BorderWidth),
                    SpriteEffects.None, 0f);


                to = position + new Vector2(0, height);

                spriteBatch.Draw(_texture, from, null, _color,
                    (float)Math.Atan2(to.Y - from.Y, to.X - from.X),
                    new Vector2(0f, (float)_texture.Height / 2),
                    new Vector2(Vector2.Distance(from, to), BorderWidth),
                    SpriteEffects.None, 0f);

                from = position + new Vector2(width, 0);
                to = position + new Vector2(width, height);

                spriteBatch.Draw(_texture, from, null, _color,
                    (float)Math.Atan2(to.Y - from.Y, to.X - from.X),
                    new Vector2(0f, (float)_texture.Height / 2),
                    new Vector2(Vector2.Distance(from, to), BorderWidth),
                    SpriteEffects.None, 0f);

                from = position + new Vector2(0, height);
                to = position + new Vector2(width, height);

                spriteBatch.Draw(_texture, from, null, _color,
                    (float)Math.Atan2(to.Y - from.Y, to.X - from.X),
                    new Vector2(0f, (float)_texture.Height / 2),
                    new Vector2(Vector2.Distance(from, to), BorderWidth),
                    SpriteEffects.None, 0f);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rect)
        {
            Draw(spriteBatch, new Vector2(rect.X, rect.Y), rect.Width, rect.Height);
        }
    }
}
