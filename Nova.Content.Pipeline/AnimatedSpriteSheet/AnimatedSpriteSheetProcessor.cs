using Microsoft.Xna.Framework.Content.Pipeline;

namespace Nova.Content.Pipeline.AnimatedSpriteSheet
{
    [ContentProcessor(DisplayName = "Animated Sprite Sheet Processor - Nova")]
    public class AnimatedSpriteSheetProcessor : ContentProcessor<Common.Sprite.AnimatedSpriteSheet, Common.Sprite.AnimatedSpriteSheet>
    {
        public override Common.Sprite.AnimatedSpriteSheet Process(Common.Sprite.AnimatedSpriteSheet input, ContentProcessorContext context)
        {
            return input;
        }
    }
}