using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Services;

namespace Nova.GUIEngine.Elements
{
    public class TextElement : Element
    {
        public string Font { get; set; } = "Fonts/TitleFontVariant1";

        private SpriteFont _spriteFont;
        public Color Color { get; set; } = Color.White;
        public string Text { get; set; }

        public TextElement()
        {
        }

        public override void LoadContent(ContentManager content)
        {
            _spriteFont = FontCacheService.GetOrLoad(content, Font);
        }

        public override void Draw(DrawingContext ctx)
        {
            ctx.DrawString(_spriteFont, Text, ActualPosition, Color);
        }
    }
}
