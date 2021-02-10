using Microsoft.Xna.Framework.Content;
using Nova.Common.Sprite;

namespace Nova.GUIEngine.Elements
{
    public class ImageElement : Element
    {
        private readonly string _image;
        private readonly string _sheet;

        private Sprite _sprite;

        public ImageElement(string image, string sheet = "UI/Packed")
        {
            _image = image;
            _sheet = sheet;
        }

        public override void LoadContent(ContentManager content)
        {
            var sheet = content.Load<SpriteSheet>(_sheet);
            _sprite = sheet[_image];
        }

        public override void Draw(DrawingContext ctx)
        {
            ctx.Draw(_sprite, ActualPosition);
        }
    }
}
