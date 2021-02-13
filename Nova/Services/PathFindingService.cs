using System.Collections.Generic;
using System.Linq;
using EpPathFinding;
using EpPathFinding.Grid;
using Microsoft.Xna.Framework;
using Nova.Common.Extensions;
using Nova.Environment;

namespace Nova.Services
{
    public class PathFindingService
    {
        private readonly GameObjectManager _gameObjectManager;
        private readonly Map _map;
        private StaticGrid _staticGrid;

        public List<TileCoordinate> NotIncludedTiles = new List<TileCoordinate>();

        public PathFindingService(MapService mapService, GameObjectManager gameObjectManager)
        {
            _gameObjectManager = gameObjectManager;
            _map = mapService.Map;
        }

        public void Initialize()
        {
            var matrix = new bool[_map.Width, _map.Height];
            
            var collisionBounds = _gameObjectManager.GameObjects.Where(x => x.CollisionEnabled).Select(z => z.CollisionBounds).ToList();

            for (int y = 0; y < _map.Height; y++)
            {
                for (int x = 0; x < _map.Width; x++)
                {
                    var tileCoordinate = new TileCoordinate(x, y);

                    if (_map.Tiles.TryGetValue(tileCoordinate, out var tile))
                    {
                        if (!tile.Traversable)
                        {
                            matrix[x, y] = false;
                            NotIncludedTiles.Add(new TileCoordinate(x, y));
                            continue;
                        }
                    }
                    
                    var pos = tileCoordinate.ToActualVector2().ToPoint();
                    var rectangle = new Rectangle(pos.X, pos.Y, 32, 32);
                    if (collisionBounds.Any(rect => rect.Intersects(rectangle)))
                    {
                        matrix[x, y] = false;
                        NotIncludedTiles.Add(new TileCoordinate(x, y));
                        continue;
                    }

                    matrix[x, y] = true;
                }
            }
            
            _staticGrid = new StaticGrid(_map.Width, _map.Height, matrix.ToJaggedArray());
        }

        public IEnumerable<Point> FindPath(Point startPos, Point endPos)
        {
            var start = new GridPos(startPos.X, startPos.Y);
            var end = new GridPos(endPos.X, endPos.Y);

            var param = new JumpPointParam(_staticGrid, start, end, EndNodeUnWalkableTreatment.Allow);

            var listPositions = JumpPointFinder.FindPath(param);
            
            return listPositions.Select(z => new Point()
            {
                X = z.x,
                Y = z.y
            });
        }
    }
}