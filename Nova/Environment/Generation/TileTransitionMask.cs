using System.Collections.Generic;

namespace Nova.Environment
{
    public class TileTransitionMask
    {
        public List<TileTransitionVariation> TextureVariations { get; set; } = new List<TileTransitionVariation>();
        public TileBlending TileBlending { get; set; }
    }
}