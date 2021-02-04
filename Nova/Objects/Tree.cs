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
    public class Tree : FoliageGameObject
    {
        private Sprite _topLeftSprite;
        private Sprite _topRightSprite;
        private Sprite _bottomLeftSprite;
        private Sprite _bottomRightSprite;

        private SpriteObject _obj;

        public Tree(GraphicsDevice graphicsDevice, FoliageType foliageType, Tile tile)
            : base(graphicsDevice, foliageType, tile)
        {
        }

        public override void LoadContent(ContentManager contentManager)
        {
            var environmentSheet = contentManager.Load<SpriteSheet>("environmentSheet");
            var treesSheet = contentManager.Load<SpriteSheet>("treesSheet");


            var rand = new Random(Guid.NewGuid().GetHashCode());


            var rect = new Rectangle[]
            {
                new Rectangle(4 * 32, 3 * 32, 32 * 3, 32 * 4),
                new Rectangle(7 * 32, 3 * 32, 32 * 3, 32 * 4),
                new Rectangle(10 * 32, 3 * 32, 32 * 3, 32 * 4),
            };

            var finalRect = rect[rand.Next(0, rect.Length)];


            if (FoliageType == FoliageType.LightGreenTree)
            {
                /*_topLeftSprite = environmentSheet["9"];
                _topRightSprite = environmentSheet["10"];
                _bottomLeftSprite = environmentSheet["17"];
                _bottomRightSprite = environmentSheet["18"];*/
                _obj = treesSheet.CreateSpriteObject(finalRect);
            }
            else if (FoliageType == FoliageType.GreenTree)
            {
                _topLeftSprite = environmentSheet["11"];
                _topRightSprite = environmentSheet["12"];
                _bottomLeftSprite = environmentSheet["19"];
                _bottomRightSprite = environmentSheet["20"];
            }
            else if (FoliageType == FoliageType.BrownTree)
            {
                _topLeftSprite = environmentSheet["13"];
                _topRightSprite = environmentSheet["14"];
                _bottomLeftSprite = environmentSheet["21"];
                _bottomRightSprite = environmentSheet["22"];
            }
            else if (FoliageType == FoliageType.DeadTree)
            {
                _topLeftSprite = environmentSheet["15"];
                _topRightSprite = environmentSheet["16"];
                _bottomLeftSprite = environmentSheet["23"];
                _bottomRightSprite = environmentSheet["24"];
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation = default(float))
        {
            position = new Vector2(Tile.X * 32, Tile.Y * 32);

            if (_obj != null)
            {
                //spriteBatch.Draw(_obj.Texture, position, Color.White);
                _obj.Draw(spriteBatch, position);
            }
            else
            {
                _bottomLeftSprite.Draw(spriteBatch, position);
                _bottomRightSprite.Draw(spriteBatch, position + new Vector2(32, 0));

                _topLeftSprite.Draw(spriteBatch, position + new Vector2(0, -32));
                _topRightSprite.Draw(spriteBatch, position + new Vector2(32, -32));
            }
        }
    }
}
