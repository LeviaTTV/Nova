using System.Collections.Generic;
using Nova.Common.Sprite;

namespace Nova.Objects.Character
{
    public class DirectionCharacterParts
    {
        public Orientation Orientation { get; set; }
        public List<AnimatedSpriteSheet> SpriteSheets { get; set; } = new List<AnimatedSpriteSheet>();
    }
}