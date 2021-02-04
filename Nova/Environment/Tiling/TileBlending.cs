using System;

namespace Nova.Environment
{
    [Flags]
    public enum TileBlending
    {
        NoBlending = 0,
        NorthWest = 1,
        North = 2,
        NorthEast = 4,
        East = 8,
        SouthEast = 16,
        South = 32,
        SouthWest= 64,
        West = 128
    }
}