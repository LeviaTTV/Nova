using System.Collections.Generic;
using Nova.Objects;

namespace Nova.Environment
{
    public class Map
    {
        public float[,] MapData { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public Dictionary<TileCoordinate, Tile> Tiles { get; set; }
        public List<GameObject> GameObjects { get; set; }
    }

    public struct TileCoordinate
    {
        public int X { get; set; }
        public int Y { get; set; }

        public TileCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}