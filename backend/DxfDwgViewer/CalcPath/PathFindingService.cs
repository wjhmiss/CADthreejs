namespace DxfDwgViewer.CalcPath
{
    public class PathFindingService
    {
        private readonly GridMap _gridMap;
        private readonly AStarPathFinder _pathFinder;

        public PathFindingService(int width, int height, bool allowDiagonal = true)
        {
            _gridMap = new GridMap(width, height);
            _pathFinder = new AStarPathFinder(_gridMap, allowDiagonal);
        }

        public void SetObstacle(int x, int y, bool isObstacle)
        {
            _gridMap.SetWalkable(x, y, !isObstacle);
        }

        public void SetObstacles(List<(int X, int Y)> obstacles)
        {
            foreach (var obstacle in obstacles)
            {
                SetObstacle(obstacle.X, obstacle.Y, true);
            }
        }

        public void ClearObstacles()
        {
            for (int y = 0; y < _gridMap.Height; y++)
            {
                for (int x = 0; x < _gridMap.Width; x++)
                {
                    _gridMap.SetWalkable(x, y, true);
                }
            }
        }

        public PathFindingResult FindPathWithWaypoints(List<(int X, int Y)> waypoints, List<(int X, int Y)> obstacles = null)
        {
            if (waypoints == null || waypoints.Count < 2)
            {
                return new PathFindingResult
                {
                    Success = false,
                    Message = "路径点数量必须大于等于2"
                };
            }

            if (obstacles != null)
            {
                ClearObstacles();
                SetObstacles(obstacles);
            }

            var fullPath = new List<(int, int)>();
            double totalCost = 0;
            int totalNodesExplored = 0;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                var start = waypoints[i];
                var end = waypoints[i + 1];

                var segmentResult = _pathFinder.FindPath(start.X, start.Y, end.X, end.Y);

                if (!segmentResult.Success)
                {
                    return new PathFindingResult
                    {
                        Success = false,
                        Message = $"无法从点 ({start.X}, {start.Y}) 到点 ({end.X}, {end.Y}) 找到路径: {segmentResult.Message}",
                        Path = fullPath,
                        TotalCost = totalCost,
                        NodesExplored = totalNodesExplored,
                        ExecutionTimeMs = stopwatch.ElapsedMilliseconds
                    };
                }

                if (i > 0)
                {
                    segmentResult.Path.RemoveAt(0);
                }

                fullPath.AddRange(segmentResult.Path);
                totalCost += segmentResult.TotalCost;
                totalNodesExplored += segmentResult.NodesExplored;
            }

            stopwatch.Stop();

            return new PathFindingResult
            {
                Success = true,
                Message = $"成功规划经过 {waypoints.Count} 个点的路径",
                Path = fullPath,
                TotalCost = totalCost,
                NodesExplored = totalNodesExplored,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds
            };
        }

        public int GridWidth => _gridMap.Width;
        public int GridHeight => _gridMap.Height;
    }
}
