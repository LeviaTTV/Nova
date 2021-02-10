using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Nova.Common.Sprite;
using Nova.GUIEngine.Units;

namespace Nova.GUIEngine.Elements
{
    public delegate void OnClick(Element element);

    public class ImageButton : Element
    {
        private readonly string _image;
        private readonly string _imagePressed;
        private readonly string _sheet;

        private Sprite _sprite, _spritePressed;
        private bool _isDown;

        public event OnClick OnClick = delegate { };

        public ImageButton(string image, string imagePressed = null, string sheet = "UI/Packed")
        {
            _image = image;
            _imagePressed = imagePressed;
            _sheet = sheet;
        }

        public override void OnLeftMouseDown(Vector2 position)
        {
            _isDown = true;
        }

        public override void OnLeftMouseUp(Vector2 position)
        {
            _isDown = false;
        }

        public override void LoadContent(ContentManager content)
        {
            var sheet = content.Load<SpriteSheet>(_sheet);
            _sprite = sheet[_image];

            if (!string.IsNullOrWhiteSpace(_imagePressed))
            {
                _spritePressed = sheet[_imagePressed];
            }

            Size = new USize(_sprite.Width, _sprite.Height);
        }

        public override void Draw(DrawingContext ctx)
        {
            if (_spritePressed != null && _isDown)
                ctx.Draw(_spritePressed, ActualPosition);
            else
                ctx.Draw(_sprite, ActualPosition);
        }
    }
}
