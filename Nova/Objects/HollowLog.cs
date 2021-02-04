using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common;
using Nova.Common.Sprite;
using Nova.Environment;
using Nova.Environment.Foliage;
using IDrawable = Nova.Common.Sprite.IDrawable;
using IUpdateable = Nova.Common.Sprite.IUpdateable;

namespace Nova.Objects
{
    public class HollowLog : FoliageGameObject
    {
        private Sprite _spriteOne;
        private Sprite _spriteTwo;

        private bool Horizontal = false;

        public HollowLog(GraphicsDevice graphicsDevice, FoliageType foliageType, Tile tile)
            : base(graphicsDevice, foliageType, tile)
        {
        }

        public override void LoadContent(ContentManager contentManager)
        {
            var environmentSheet = contentManager.Load<SpriteSheet>("environmentSheet");

            Horizontal = new Random(Guid.NewGuid().GetHashCode()).NextDouble() >= 0.5f;

            if (Horizontal)
            {
                _spriteOne = environmentSheet["47"];
                _spriteTwo = environmentSheet["48"];
            }
            else
            {
                _spriteOne = environmentSheet["64"];
                _spriteTwo = environmentSheet["72"];
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation = default(float))
        {
            position = new Vector2(Tile.X * 32, Tile.Y * 32);


            if (Horizontal)
            {
                _spriteOne.Draw(spriteBatch, position);
                _spriteTwo.Draw(spriteBatch, position + new Vector2(32, 0));
            }
            else
            {
                _spriteOne.Draw(spriteBatch, position);
                _spriteTwo.Draw(spriteBatch, position + new Vector2(0, 32));
            }
        }
    }
}
