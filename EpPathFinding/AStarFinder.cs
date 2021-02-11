using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using C5;
using EpPathFinding.Grid;

namespace EpPathFinding
{
    public static class AStarFinder
    {
        public static List<GridPos> FindPath(AStarParam parameters)
        {
            object lo = new object();
            var openList = new IntervalHeap<Node>();
            var startNode = parameters.StartNode;
            var endNode = parameters.EndNode;
            var heuristic = parameters.HeuristicFunc;
            var grid = parameters.SearchGrid;
            var diagonalMovement = parameters.DiagonalMovement;
            var weight = parameters.Weight;
            
            startNode.startToCurNodeLen = 0;
            startNode.heuristicStartToEndLen = 0;

            openList.Add(startNode);
            startNode.isOpened = true;

            while (openList.Count != 0)
            {
                var node = openList.DeleteMin();
                node.isClosed = true;

                if (node == endNode)
                {
                    return Node.Backtrace(endNode);
                }

                var neighbors = grid.GetNeighbors(node, diagonalMovement);
                
                Parallel.ForEach(neighbors, neighbor =>
                {
                    if (neighbor.isClosed) return;

                    var x = neighbor.x;
                    var y = neighbor.y;
                    float ng = node.startToCurNodeLen + (float)((x - node.x == 0 || y - node.y == 0) ? 1 : Math.Sqrt(2));

                    if (!neighbor.isOpened || ng < neighbor.startToCurNodeLen)
                    {
                        neighbor.startToCurNodeLen = ng;
                        if (neighbor.heuristicCurNodeToEndLen == null) neighbor.heuristicCurNodeToEndLen = weight * heuristic(Math.Abs(x - endNode.x), Math.Abs(y - endNode.y));
                        neighbor.heuristicStartToEndLen = neighbor.startToCurNodeLen + neighbor.heuristicCurNodeToEndLen.Value;
                        neighbor.parent = node;
                        if (!neighbor.isOpened)
                        {
                            lock (lo)
                            {
                                openList.Add(neighbor);
                            }
                            neighbor.isOpened = true;
                        }
                    }
                });
            }

            return new List<GridPos>();
        }
    }
}
