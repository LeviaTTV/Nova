using Microsoft.Xna.Framework.Content.Pipeline;
using Nova.Common.Sprite;

namespace Nova.Content.Pipeline.SpriteSheet
{
    [ContentProcessor(DisplayName = "Sprite Sheet Processor - Nova")]
    public class SpriteSheetProcessor : ContentProcessor<Common.Sprite.SpriteSheet, Common.Sprite.SpriteSheet>
    {
        public override Common.Sprite.SpriteSheet Process(Common.Sprite.SpriteSheet input, ContentProcessorContext context)
        {
            return input;
        }
    }
}