using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nova.Common.Sprite;
using Nova.GUIEngine;
using Nova.GUIEngine.Base;
using Nova.GUIEngine.Triggers;
using Nova.GUIEngine.Units;
using Nova.Services;

namespace Nova.GUI
{
    /// <summary>
    /// Indicator for the ingame time.
    /// </summary>
    public class TimePanel : Panel
    {
        private SpriteFont _font;
        private Sprite _timeSprite;
        private readonly TimeService _timeService;

        public TimePanel(GameServiceContainer services)
        {
            _timeService = services.GetService<TimeService>();
            Triggers.Add(new ToggleVisibilityKeyboardTrigger(this, Keys.B));
        }

        public override void LoadContent(ContentManager content)
        {
            _font = FontCacheService.GetOrLoad(content, "Fonts/TitleFontVariant2");

            var sheet = content.Load<SpriteSheet>("UI/Packed");
            
            _timeSprite = sheet["TimeImage"];
        }

        public override void Draw(DrawingContext ctx)
        {
            ctx.Draw(_timeSprite, ActualPosition);
            ctx.DrawString(_font, _timeService.CurrentTime.ToShortTimeString(), ActualPosition + new UVector2(30f, 8f));
        }
    }
}
