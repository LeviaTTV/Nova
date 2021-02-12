using System;
using System.Collections.Generic;

namespace Nova.Common.Sprite
{
    public enum AssetType : byte
    {
        Unknown,
        SpriteSheet,
        Sprite,
        AnimatedSpriteSheet
    }

    public static class AssetTypeMapping
    {
        public static Dictionary<Type, AssetType> TypeMapping = new Dictionary<Type, AssetType>()
        {
            { typeof(SpriteSheet), AssetType.SpriteSheet },
            { typeof(AnimatedSpriteSheet), AssetType.AnimatedSpriteSheet },
            { typeof(Sprite), AssetType.Sprite }
        };
    }
}
