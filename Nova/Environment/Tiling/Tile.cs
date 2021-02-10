using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nova.Environment
{
    public class Tile
    {
        public TileType TileType { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public TileBlending TileBlending { get; set; }

        public List<Texture2D> BlendTextureList { get; set; } = new List<Texture2D>();

        public  bool Traversable { get; set; }

        public bool InBounds(Map map, Rectangle bounds)
        {
            int x = X * map.TileWidth;
            int y = Y * map.TileHeight;

            return x >= bounds.X && x <= bounds.X + bounds.Width &&
                   y >= bounds.Y && y <= bounds.Y + bounds.Height;
        }
        
        public string FoliageType { get; set; }
    }
}
