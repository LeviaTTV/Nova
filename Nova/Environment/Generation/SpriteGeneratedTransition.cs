using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Nova.Environment.Generation
{
    public class SpriteGeneratedTransition
    {
        public TileBlending TileBlending { get; set; }

        public List<Texture2D> Variations { get; } = new List<Texture2D>();
    }
}