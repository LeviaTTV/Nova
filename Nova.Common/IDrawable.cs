using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nova.Common.Sprite
{
    public interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation = default(float));
    }
}
