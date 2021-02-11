using System.Collections.Generic;
using Microsoft.Xna.Framework;

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
        public Vector2 StartPosition { get; set; }
        public int Seed { get; set; }
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

        public Vector2 ToActualVector2() => new Vector2(X * 32, Y * 32);
    }
}