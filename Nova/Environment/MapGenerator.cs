using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Noise;

namespace Nova.Environment
{
    public class MapGenerator
    {
        private readonly GraphicsDevice _device;
        private readonly int _seed;

        public int ChunkSize { get; set; } = 16;

        public MapGenerator(GraphicsDevice device, int seed)
        {
            _device = device;
            _seed = seed;
        }

        public Chunk Generate(Map map, int chunkX, int chunkY)
        {
            var chunk = new Chunk()
            {
                X = chunkX,
                Y = chunkY,
                Width = ChunkSize,
                Height = ChunkSize
            };

            var noiseFunction = new FastNoiseLite(_seed);


            FastNoiseLite noise = new FastNoiseLite();
            noise.SetSeed(_seed);
            noise.SetFractalOctaves(8);
            noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);

            int startY = chunkY * ChunkSize;
            int startX = chunkX * ChunkSize;
            int width = ChunkSize;
            int height = ChunkSize;

            chunk.ChunkData = new float[ChunkSize, ChunkSize];


            float min = -1f;
            float max = 1f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    chunk.ChunkData[x, y] = (noise.GetNoise(startX + x, startY + y) - min) / (max - min);
                }
            }

            map.Chunks.Add(chunk);

            return chunk;
        }
    }
}