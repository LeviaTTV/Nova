using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Sprite;
using Nova.GUIEngine.Elements;
using Nova.GUIEngine.Units;
using Nova.Services;

namespace Nova.GUIEngine.Base
{
    /// <summary>
    /// A window is a frame consisting of a title bar with the title in it and the window itself is also draggable by default.
    /// </summary>
    public class Window : Panel
    {
        public string Title { get; set; }
        public string Icon { get; set; } = null;

        private bool _canClose;
        public bool CanClose
        {
            get => _canClose;
            set
            {
                _canClose = value;

                if (!_canClose)
                {
                    Remove(_closeButtonElement);
                }
                else
                {
                    Add(_closeButtonElement);
                }
            }
        }
        public SpriteFont Font { get; set; }


        private Sprite _titleBarLeft, _titleBarMiddle, _titleBarRight;
        private Sprite _panelTopBackgroundLeft, _panelTopBackgroundMiddle, _panelTopBackgroundRight;
        private Sprite _titleBarDecorationLeft, _titleBarDecorationMiddle, _titleBarDecorationRight;
        private Sprite _iconSprite;

        private readonly ImageButton _closeButtonElement;
        

        public Window()
        {
            _closeButtonElement = new ImageButton("CloseButton", "CloseButtonPressed");
            _closeButtonElement.Position = new UVector2(new UDim(1f, -29f), UDim.Absolute(-11f));
            _closeButtonElement.OnClick += _closeButtonElement_OnClick;

            CanClose = _canClose;
        }

        private void _closeButtonElement_OnClick(Element sender)
        {
            if (CanClose)
            {
                OnClosing();
            }
        }

        protected virtual void OnClosing()
        {
            IsHidden = true;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _titleBarLeft = PackedSheet["TitleBarLeft"];
            _titleBarMiddle = PackedSheet["TitleBarMiddle"];
            _titleBarRight = PackedSheet["TitleBarRight"];
            
            _panelTopBackgroundLeft = PackedSheet["PanelTopBackgroundLeft"];
            _panelTopBackgroundMiddle = PackedSheet["PanelTopBackgroundMiddle"];
            _panelTopBackgroundRight = PackedSheet["PanelTopBackgroundRight"];

            _titleBarDecorationLeft = PackedSheet["TitleBarDecorationLeft"];
            _titleBarDecorationMiddle = PackedSheet["TitleBarDecorationMiddle"];
            _titleBarDecorationRight = PackedSheet["TitleBarDecorationRight"];

            if (Icon != null)
                _iconSprite = PackedSheet["Icon" + Icon];
            
            
            Font = FontCacheService.GetOrLoad(content, "Fonts/TitleFontVariant1");
        }

        public override void Draw(DrawingContext ctx)
        {
            if (IsHidden)
                return;

            base.Draw(ctx);
            
            var spriteCountHorizontal = (int)Math.Ceiling(((double)ActualSize.Width.Absolute() - _titleBarLeft.Width - _titleBarRight.Width) / _titleBarMiddle.Width);


            // Shadow below the bar
            var shadowCountHorizontal = (int)Math.Ceiling(((double)ActualSize.Width.Absolute() - _panelTopBackgroundLeft.Width - _panelTopBackgroundRight.Width - 16f) / _panelTopBackgroundMiddle.Width);
            ctx.Draw(_panelTopBackgroundLeft, ActualPosition + new UVector2(4f, _titleBarMiddle.Height - 16f));
            for (int i = 0; i < shadowCountHorizontal; ++i)
            {
                ctx.Draw(_panelTopBackgroundMiddle, ActualPosition + new UVector2(4f + _panelTopBackgroundLeft.Width + i * _panelTopBackgroundMiddle.Width, _titleBarMiddle.Height - 16f));
            }
            ctx.Draw(_panelTopBackgroundRight, ActualPosition + new UVector2(ActualSize.Width.Absolute() - _panelTopBackgroundLeft.Width - _panelTopBackgroundRight.Width + 4f, _titleBarMiddle.Height - 16f));


            // Title bar
            ctx.Draw(_titleBarLeft, ActualPosition + new UVector2(-1f, -7f));
            for (int i = 0; i < spriteCountHorizontal; ++i)
                ctx.Draw(_titleBarMiddle, ActualPosition + new UVector2(_titleBarMiddle.Width * i + _titleBarLeft.Width - 1f, -7f));
            ctx.Draw(_titleBarRight, ActualPosition + new UVector2(ActualSize.Width.Absolute() - _titleBarRight.Width + 1f, -7f));


            // Decorations
            var titleBarDecorationLeftPosition = ActualPosition + new UVector2(-2f + _titleBarLeft.Width, -7f);
            ctx.Draw(_titleBarDecorationLeft, titleBarDecorationLeftPosition);


            var startPos = titleBarDecorationLeftPosition + new UVector2(_titleBarDecorationLeft.Width, 0);
            var endPos = ActualPosition + new UVector2(ActualSize.Width.Absolute() - _titleBarDecorationRight.Width - _titleBarRight.Width + 3f, -7f);

            var cannotCloseModifier = 21f;
            if (CanClose)
                cannotCloseModifier = 0f;
            
            var width = endPos.X.Absolute() - startPos.X.Absolute() + cannotCloseModifier;
            spriteCountHorizontal = (int)Math.Ceiling(width / _titleBarDecorationMiddle.Width);
            
            for (int i = 0; i < spriteCountHorizontal; ++i)
                ctx.Draw(_titleBarDecorationMiddle, startPos + new UVector2(i * _titleBarDecorationMiddle.Width, 0f));
            ctx.Draw(_titleBarDecorationRight, ActualPosition + new UVector2(ActualSize.Width.Absolute() - _titleBarDecorationRight.Width - _titleBarRight.Width + 3f + cannotCloseModifier, -7f));



            // Icon
            var iconPadding = 0;
            if (_iconSprite != null)
            {
                ctx.Draw(_iconSprite, ActualPosition + new UVector2(_titleBarLeft.Width + 10f, -7f - 6f));
                iconPadding = 34;
            }


            // Title
            ctx.DrawString(Font, Title, ActualPosition + new UVector2(_titleBarLeft.Width + 6f + iconPadding, -4f));
            /*

           


            var iconPadding = 0;
            if (_iconSprite != null)
            {
                drawingContext.Draw(_iconSprite, position + new Vector2(_titleBarLeft.Width + 10f, -7f - 6f));
                iconPadding = 34;
            }

            drawingContext.DrawString(Font, Title, position + new Vector2(_titleBarLeft.Width + 6f + iconPadding, -4f));*/
        }
    }
}
