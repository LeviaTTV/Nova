using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nova.Common
{
    public interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation = default(float), Vector2 origin = default(Vector2), SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0f);
    }
}
