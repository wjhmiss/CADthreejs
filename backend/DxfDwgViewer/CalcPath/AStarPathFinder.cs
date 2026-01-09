using System.Collections.Generic;

namespace DxfDwgViewer.CalcPath
{
    public class AStarPathFinder
    {
        private readonly GridMap _gridMap;
        private readonly bool _allowDiagonal;

        public AStarPathFinder(GridMap gridMap, bool allowDiagonal = true)
        {
            _gridMap = gridMap;
            _allowDiagonal = allowDiagonal;
        }

        public PathFindingResult FindPath(int startX, int startY, int endX, int endY)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new PathFindingResult();

            try
            {
                var startNode = _gridMap.GetNode(startX, startY);
                var endNode = _gridMap.GetNode(endX, endY);

                if (!startNode.IsWalkable)
                {
                    result.Success = false;
                    result.Message = "起始点不可通行";
                    return result;
                }

                if (!endNode.IsWalkable)
                {
                    result.Success = false;
                    result.Message = "终点不可通行";
                    return result;
                }

                _gridMap.Reset();

                var openSet = new PriorityQueue<Node, double>();
                var closedSet = new HashSet<Node>();

                startNode.G = 0;
                startNode.H = CalculateHeuristic(startNode, endNode);
                openSet.Enqueue(startNode, startNode.F);

                int nodesExplored = 0;

                while (openSet.Count > 0)
                {
                    var currentNode = openSet.Dequeue();
                    nodesExplored++;

                    if (currentNode.Equals(endNode))
                    {
                        result.Success = true;
                        result.Path = ReconstructPath(currentNode);
                        result.TotalCost = currentNode.G;
                        result.NodesExplored = nodesExplored;
                        result.Message = "路径规划成功";
                        stopwatch.Stop();
                        result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                        return result;
                    }

                    closedSet.Add(currentNode);

                    var neighbors = _gridMap.GetNeighbors(currentNode, _allowDiagonal);

                    foreach (var neighbor in neighbors)
                    {
                        if (!neighbor.IsWalkable || closedSet.Contains(neighbor))
                        {
                            continue;
                        }

                        double tentativeG = currentNode.G + CalculateDistance(currentNode, neighbor);

                        if (tentativeG < neighbor.G || !IsNodeInOpenSet(openSet, neighbor))
                        {
                            neighbor.Parent = currentNode;
                            neighbor.G = tentativeG;
                            neighbor.H = CalculateHeuristic(neighbor, endNode);
                            openSet.Enqueue(neighbor, neighbor.F);
                        }
                    }
                }

                result.Success = false;
                result.Message = "无法找到有效路径";
                result.NodesExplored = nodesExplored;
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"路径规划失败: {ex.Message}";
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                return result;
            }
        }

        private double CalculateHeuristic(Node a, Node b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);

            if (_allowDiagonal)
            {
                return Math.Sqrt(dx * dx + dy * dy);
            }
            else
            {
                return dx + dy;
            }
        }

        private double CalculateDistance(Node a, Node b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);

            if (dx == 0 || dy == 0)
            {
                return 1.0;
            }
            else
            {
                return Math.Sqrt(2);
            }
        }

        private List<PointCoordinate> ReconstructPath(Node endNode)
        {
            var path = new List<PointCoordinate>();
            var current = endNode;

            while (current != null)
            {
                path.Add(new PointCoordinate { X = current.X, Y = current.Y });
                current = current.Parent;
            }

            path.Reverse();
            return path;
        }

        private bool IsNodeInOpenSet(PriorityQueue<Node, double> openSet, Node node)
        {
            return openSet.UnorderedItems.Any(item => item.Element.Equals(node));
        }
    }
}
