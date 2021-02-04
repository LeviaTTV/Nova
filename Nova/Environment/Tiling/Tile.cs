using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nova.Environment.Foliage;
using Nova.Objects;

namespace Nova.Environment
{
    public class Tile
    {
        public TileType TileType { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public TileBlending TileBlending { get; set; }

        public List<Texture2D> BlendTextureList { get; set; } = new List<Texture2D>();

        public bool InBounds(Map map, Rectangle bounds)
        {
            int x = X * map.TileWidth;
            int y = Y * map.TileHeight;

            return x >= bounds.X && x <= bounds.X + bounds.Width &&
                   y >= bounds.Y && y <= bounds.Y + bounds.Height;
        }

        public FoliageType Foliage { get; set; }
        public int Variant { get; set; }
        public bool GameObject { get; set; }
    }
}
