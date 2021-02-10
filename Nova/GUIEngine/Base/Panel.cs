using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Nova.Common.Sprite;
using Nova.GUIEngine.Triggers;
using Nova.GUIEngine.Units;
using IUpdateable = Nova.Common.IUpdateable;

namespace Nova.GUIEngine.Base
{
    /// <summary>
    /// A panel is a visual UI element on screen. It does not necessarily have to contain anything or be draggable.
    /// </summary>
    public class Panel : Visual, IUpdateable
    {
        private readonly string _artPrefix;
        private readonly bool _drawCorners;

        private Sprite _topLeftSprite, _topSprite, _topRightSprite;
        private Sprite _leftSprite, _middleSprite, _rightSprite;
        private Sprite _bottomLeftSprite, _bottomSprite, _bottomRightSprite;

        private Sprite _topLeftCorner, _topRightCorner, _bottomLeftCorner, _bottomRightCorner;

        protected SpriteSheet PackedSheet;

        public List<ITrigger> Triggers { get; } = new List<ITrigger>();

        public Panel(string artPrefix = "Panel", bool drawCorners = false)
        {
            _artPrefix = artPrefix;
            _drawCorners = drawCorners;
        }

        public override void LoadContent(ContentManager content)
        {
            PackedSheet = content.Load<SpriteSheet>("UI/Packed");

            _topLeftSprite = PackedSheet[_artPrefix + "TopLeft"];
            _topSprite = PackedSheet[_artPrefix + "Top"];
            _topRightSprite = PackedSheet[_artPrefix + "TopRight"];
            _leftSprite = PackedSheet[_artPrefix + "Left"];
            _middleSprite = PackedSheet[_artPrefix + "Middle"];
            _rightSprite = PackedSheet[_artPrefix + "Right"];
            _bottomLeftSprite = PackedSheet[_artPrefix + "BottomLeft"];
            _bottomSprite = PackedSheet[_artPrefix + "Bottom"];
            _bottomRightSprite = PackedSheet[_artPrefix + "BottomRight"];

            if (_drawCorners)
            {
                _topLeftCorner = PackedSheet[_artPrefix + "CornerTopLeft"];
                _topRightCorner = PackedSheet[_artPrefix + "CornerTopRight"];
                _bottomLeftCorner = PackedSheet[_artPrefix + "CornerBottomLeft"];
                _bottomRightCorner = PackedSheet[_artPrefix + "CornerBottomRight"];
            }
        }

        public override void Draw(DrawingContext ctx)
        {
            var spriteCountHorizontal = (int) Math.Ceiling((ActualSize.Width.Absolute() - _leftSprite.Width - _rightSprite.Width) / _leftSprite.Width);

            // Top row
            ctx.Draw(_topLeftSprite, ActualPosition);
            for (int i = 0; i < spriteCountHorizontal; ++i)
            {
                ctx.Draw(_topSprite, ActualPosition + new UVector2(_topSprite.Width * i + _topLeftSprite.Width, 0f));

            }

            ctx.Draw(_topRightSprite, ActualPosition + new UVector2(ActualSize.Width - UDim.Absolute(_topRightSprite.Width), UDim.Zero));
            
            // Middle rows and background
            int rowHeight = _leftSprite.Height;
            float height = ActualSize.Height.Absolute() - _topSprite.Height - _bottomSprite.Height;
            int amountOfRows = (int) Math.Ceiling(height / rowHeight);

            for (int row = 0; row < amountOfRows; row++)
            {
                int rowY = row * _leftSprite.Height + _topSprite.Height;
                ctx.Draw(_leftSprite, ActualPosition + new UVector2(UDim.Zero, UDim.Absolute(rowY)));

                for (int i = 0; i < spriteCountHorizontal; i++)
                {
                    int spriteX = _leftSprite.Width + i * _middleSprite.Width;
                    ctx.Draw(_middleSprite, ActualPosition + new UVector2(UDim.Absolute(spriteX), UDim.Absolute(rowY)));
                }

                ctx.Draw(_rightSprite, ActualPosition + new UVector2(ActualSize.Width.Absolute() - _rightSprite.Width, rowY));
            }


            // Bottom row
            ctx.Draw(_bottomLeftSprite, ActualPosition + new UVector2(UDim.Zero, ActualSize.Height - UDim.Absolute(_bottomLeftSprite.Height)));
            for (int i = 0; i < spriteCountHorizontal; ++i)
            {
                ctx.Draw(_bottomSprite, ActualPosition + new UVector2(_bottomSprite.Width * i + _leftSprite.Width, ActualSize.Height.Absolute() - _bottomSprite.Height));
            }

            ctx.Draw(_bottomRightSprite, ActualPosition + new UVector2(ActualSize.Width - UDim.Absolute(_bottomRightSprite.Width), ActualSize.Height - UDim.Absolute(_bottomRightSprite.Height)));


            if (_drawCorners)
            {
                // Corners
                ctx.Draw(_topLeftCorner, ActualPosition + new UVector2(UDim.Absolute(-1f)));
                ctx.Draw(_topRightCorner, ActualPosition + new UVector2(ActualSize.Width.Absolute() - _topRightCorner.Width + 1f, -1f));
                ctx.Draw(_bottomLeftCorner, ActualPosition + new UVector2(-1f, ActualSize.Height.Absolute() - _bottomLeftCorner.Height + 1f));
                ctx.Draw(_bottomRightCorner, ActualPosition + new UVector2(ActualSize.Width.Absolute() - _bottomRightCorner.Width + 1f, ActualSize.Height.Absolute() - _bottomRightCorner.Height + 1f));
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var trigger in Triggers)
            {
                trigger.CheckTrigger();
            }
        }
    }
}
