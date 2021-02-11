using System.Collections.Generic;
using Nova.Common.Sprite;

namespace Nova.Environment.Generation
{
    public class SpriteTransition
    {
        public Sprite Sprite { get; set; }

        public List<SpriteGeneratedTransition> Transitions { get; } = new List<SpriteGeneratedTransition>();
        public TileType TileType { get; set; }
    }
}