using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Extensions;

namespace Nova.Services
{
    /// <summary>
    /// Lazy class because SpriteFonts for UI are not loaded through the content manager so they won't be cached. This is our own rudimentary cache.
    /// </summary>
    public static class FontCacheService
    {
        public static Dictionary<string, SpriteFont> Fonts = new Dictionary<string, SpriteFont>();

        public static SpriteFont GetOrLoad(ContentManager contentManager, string assetName)
        {
            if (FontCacheService.Fonts.ContainsKey(assetName))
                return FontCacheService.Fonts[assetName];
            else
            {
                var font = contentManager.LoadFont(assetName);

                FontCacheService.Fonts[assetName] = font;
                return font;
            }
        }
    }
}
