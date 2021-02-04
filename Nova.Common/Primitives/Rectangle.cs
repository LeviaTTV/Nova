using System;
using System.Collections.Generic;
using System.Linq;
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

        private Texture2D _texture;

        public PrimitiveRectangle(GraphicsDevice device, Color color)
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

        public void Draw(SpriteBatch spriteBatch, Vector2 position, int width, int height)
        {
            spriteBatch.Draw(_texture, new Microsoft.Xna.Framework.Rectangle((int)position.X, (int)position.Y, width, height), _color);
        }
    }
}
