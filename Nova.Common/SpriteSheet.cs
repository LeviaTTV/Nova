using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
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
        
        public Dictionary<string, Sprite> Sprites { get; protected set; } = new Dictionary<string, Sprite>();

        public string AssetName { get; set; }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation = default(float), Vector2 origin = default(Vector2), SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0f)
        {
            spriteBatch.Draw(Texture, position, Color.White);
        }

        public Sprite this[string name] => Sprites[name];

        public SpriteObject CreateSpriteObject(Rectangle sourceRectangle)
        {
            var spriteObj = new SpriteObject();
            spriteObj.Texture = Texture;
            spriteObj.SourceRectangle = sourceRectangle;

            return spriteObj;
        }

        public IEnumerable<Sprite> GetSprites(int startInclusive, int count)
        {
            return Sprites.Values.ToList().GetRange(startInclusive, count);
        }
    }
}
