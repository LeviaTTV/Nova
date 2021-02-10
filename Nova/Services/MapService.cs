using Microsoft.Xna.Framework;
using Nova.Environment;

namespace Nova.Services
{
    public class MapService
    {
        public Map Map { get; set; }

        public MapService()
        {
        }

        public Tile GetTileAt(Vector2 newPosition)
        {
            int tileX = (int)(newPosition.X / 32);
            int tileY = (int)(newPosition.Y / 32);
            var tc = new TileCoordinate(tileX, tileY);

            if (Map.Tiles.ContainsKey(tc))
                return Map.Tiles[new TileCoordinate(tileX, tileY)];

            return null;
        }
    }
}
