using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Content;
using Nova.Common.Sprite;

namespace Nova.Environment
{
    public class Chunk
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public float[,] ChunkData { get; set; }

        public (int xStart, int yStart, int xEnd, int yEnd) GetChunkBounds() => (X * Width, Y * Height, X * Width + Width, Y * Height + Height);


        public Tile[,] Tiles { get; private set; }

        public void PrepareChunk(SpriteSheet environmentSpriteSheet)
        {
            int tileSize = 32;
            var lightGreenSprite = environmentSpriteSheet["1"];
            var darkerThanLightGreenSprite = environmentSpriteSheet["2"];
            var yellowishSprite = environmentSpriteSheet["3"];
            var deadGrassSprite = environmentSpriteSheet["4"];
            var sandishSprite = environmentSpriteSheet["5"];
            var gravelSprite = environmentSpriteSheet["8"];
            var waterSprite = environmentSpriteSheet["62"];
            var mountainSprite = environmentSpriteSheet["323"];

            Tiles = new Tile[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float mapData = ChunkData[x, y];

                    Sprite spriteToRender = null;
                    if (mapData < 0.25f)
                    {
                        spriteToRender = waterSprite;
                    }
                    else if (mapData >= 0.25f && mapData < 0.4f)
                    {
                        spriteToRender = lightGreenSprite;
                    }
                    else if (mapData >= 0.4f && mapData < 0.6f)
                    {
                        spriteToRender = darkerThanLightGreenSprite;
                    }
                    else if (mapData >= 0.6f && mapData < 0.7f)
                    {
                        spriteToRender = deadGrassSprite;
                    }
                    else if (mapData >= .7f && mapData < .8f)
                    {
                        spriteToRender = sandishSprite;
                    }
                    else if (mapData >= .8f && mapData < .9f)
                    {
                        spriteToRender = gravelSprite;
                    }
                    else
                    {
                        spriteToRender = mountainSprite;
                    }

                    Tiles[x, y] = new Tile()
                    {
                        Sprite = spriteToRender,
                        X = x,
                        Y = y
                    };
                }
            }


            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var tile = Tiles[x, y];

                    if (x - 1 >= 0)
                    {
                        var leftTile = Tiles[x - 1, y];
                        if (leftTile.Sprite != tile.Sprite)
                            tile.TileBlending |= TileBlending.Left;
                    }

                    if (x + 1 <= Width - 1)
                    {
                        var rightTile = Tiles[x + 1, y];
                        if (rightTile.Sprite != tile.Sprite)
                            tile.TileBlending |= TileBlending.Right;
                    }

                    if (y - 1 >= 0)
                    {
                        var upTile = Tiles[x, y - 1];
                        if (upTile.Sprite != tile.Sprite)
                            tile.TileBlending |= TileBlending.Top;
                    }

                    if (y + 1 <= Height - 1)
                    {
                        var belowTile = Tiles[x, y + 1];
                        if (belowTile.Sprite != tile.Sprite)
                            tile.TileBlending |= TileBlending.Bottom;
                    }

                }
            }

        }
    }

    public class Tile
    {
        public Sprite Sprite { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public TileBlending TileBlending { get; set; }


    }

    [Flags]
    public enum TileBlending : byte
    {
        NoBlending = 0,
        Top = 1,
        Left = 2,
        Bottom = 4,
        Right = 8
    }
}