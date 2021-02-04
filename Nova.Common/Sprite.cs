using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Nova.Common.Sprite
{
    public class Sprite : IDrawable, ICloneable
    {
        [JsonIgnore]
        public Texture2D Texture { get; set; }

        [JsonIgnore]
        public Rectangle SourceRectangle => new Rectangle(X, Y, Width, Height);

        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation = default(float))
        {
            if (rotation > default(float))
            {
                spriteBatch.Draw(Texture, new Rectangle((int)position.X, (int)position.Y, Width, Height), SourceRectangle, Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(Texture, position, SourceRectangle, Color.White);
            }
        }

        public object Clone()
        {
            return new Sprite()
            {
                Height = Height,
                Name = Name,
                Texture = Texture,
                Width = Width,
                X = X,
                Y = Y
            };
        }

        public override string ToString() => $"{Name} X:{X} Y:{Y} W:{Width} H:{Height}";

    }
}
