using System.Collections.Generic;
using System.Linq;

namespace Nova.Environment
{
    public class Map
    {
        public int ChunkSize { get; set; }
        public List<Chunk> Chunks { get; } = new List<Chunk>();

        public Chunk this[int x, int y]
        {
            get
            {
                return Chunks.FirstOrDefault(z => z.X == x && z.Y == y);
            }
        }
    }
}