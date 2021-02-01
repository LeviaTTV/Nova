using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace Nova.Common.Sprite
{
    public class SpriteSheet : IDrawable
    {
        public string Name { get; set; }

        public int Height { get; protected set; }
        public int Width { get; protected set; }

        public Texture2D Texture { get; set; }

        public List<Sprite> Sprites { get; protected set; } = new List<Sprite>();

        public string AssetName { get; set; }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(Texture, position, Color.White);
        }

        public Sprite this[string name] => Sprites.FirstOrDefault(x => x.Name.Equals(name));
    }
}
