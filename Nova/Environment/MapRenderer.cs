using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nova.Common.Sprite;
using Nova.Primitives;

namespace Nova.Environment
{
    public class MapRenderer
    {
        private readonly GraphicsDevice _device;
        private readonly Map _map;
        private readonly MapGenerator _mapGenerator;
        private readonly Camera2D _camera2D;
        private SpriteSheet _environmentSpriteSheet;

        private Line _positionLine;

        public MapRenderer(GraphicsDevice device, Map map, MapGenerator mapGenerator, Camera2D camera2D)
        {
            _device = device;
            _map = map;
            _mapGenerator = mapGenerator;
            _camera2D = camera2D;
        }

        public void LoadContent(ContentManager content)
        {
            _environmentSpriteSheet = content.Load<SpriteSheet>("environmentSheet");

            _positionLine = new Line(_device, Color.Red);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Ignore viewport for now, later we can use it to figure out which chunks to draw where.
            var viewport = _device.Viewport;
            int width = viewport.Width;
            int height = viewport.Height;

            var chunks = CalculateVisibleChunks(_camera2D.Pos, _map.ChunkSize, 32);

            var chunksToRender = new List<Chunk>();

            foreach (var fakeChunk in chunks)
            {
                var chunk = _map[fakeChunk.x, fakeChunk.y];

                if (chunk == null)
                {
                    var chunkToRender = _mapGenerator.Generate(_map, fakeChunk.x, fakeChunk.y);
                    chunkToRender.PrepareChunk(_environmentSpriteSheet);
                    chunksToRender.Add(chunkToRender);
                }
                else
                    chunksToRender.Add(chunk);
            }


            var renderedTilesList = new List<RenderedTile>(chunksToRender.Count * _map.ChunkSize * _map.ChunkSize);
            foreach (var chunk in chunksToRender)
            {
                DrawChunk(renderedTilesList, spriteBatch, chunk, 32);
            }

            foreach (var tile in renderedTilesList.Where(x => x.Tile.TileBlending != TileBlending.NoBlending))
            {
                _positionLine.Draw(spriteBatch, tile.RenderPosition + new Vector2(32, 0), tile.RenderPosition + new Vector2(32, 32), 5f);

                if ((tile.Tile.TileBlending & TileBlending.Bottom) == TileBlending.Bottom)
                {
                    var blendSprite = tile.Tile.Sprite;
                    var pos = tile.RenderPosition + new Vector2(0, 32);

                    // Edge at pos
                }
            }

            /*foreach (var tile in renderedTilesList)
            {
                var different = renderedTilesList.FirstOrDefault(x => x.X == tile.X && x.Y == tile.Y + 1 && tile.Sprite != x.Sprite);

                if (different.X != 0)
                {
                    // Create transition
                    _positionLine.Draw(spriteBatch, different.RenderPosition, different.RenderPosition + new Vector2(1f, 1f), 10f);
                }
            }*/

            _positionLine.Draw(spriteBatch, _camera2D.Pos, _camera2D.Pos + new Vector2(1f, 1f), 10f);
        }

        private IEnumerable<(int x, int y)> CalculateVisibleChunks(Vector2 position, int chunkSize, int tileSize)
        {
            var viewport = _device.Viewport;

            int chunkPixelSize = chunkSize * tileSize;

            int chunkX = (int)(position.X / chunkPixelSize);
            int chunkY = (int)(position.Y / chunkPixelSize);

            float zoom = _camera2D.Zoom;

            if (zoom < 1f)
                zoom += 1.5f;

            var viewportWidth = Math.Ceiling(viewport.Width * zoom);
            var viewportHeight = Math.Ceiling(viewport.Height * zoom);

            // Figure out how many chunks to generate around the players chunk
            int heightAmountOfChunks = (int) Math.Ceiling(viewportHeight / (double)chunkPixelSize);
            int widthAmountOfChunks = (int) Math.Ceiling(viewportWidth / (double)chunkPixelSize);


            int chunkBoundX = chunkX - widthAmountOfChunks;
            int chunkBoundXEnd = chunkX + widthAmountOfChunks;
            int chunkBoundY = chunkY - heightAmountOfChunks;
            int chunkBoundYEnd = chunkY + heightAmountOfChunks;

            for (int x = chunkBoundX; x < chunkBoundXEnd; x++)
            for (int y = chunkBoundY; y < chunkBoundYEnd; y++)
                yield return (x, y);

        }

        private void DrawChunk(List<RenderedTile> list, SpriteBatch spriteBatch, Chunk chunk, int tileSize)
        {
            int pixelX = chunk.X * chunk.Width * tileSize;
            int pixelY = chunk.Y * chunk.Height * tileSize;

            int tilesX = chunk.Width;
            int tilesY = chunk.Height;
            for (int y = 0; y < tilesY; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    var tile = chunk.Tiles[x, y];

                    int spriteX = pixelX + x * tileSize;
                    int spriteY = pixelY + y * tileSize;

                    var pos = new Vector2(spriteX, spriteY);
                    tile.Sprite.Draw(spriteBatch, pos);

                    list.Add(new RenderedTile()
                    {
                        Tile = tile,
                        X = x,
                        Y = y,
                        RenderPosition = pos
                    });
                }
            }
        }
    }

    public struct RenderedTile
    {
        public Tile Tile { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Vector2 RenderPosition { get; set; }
    }
}
