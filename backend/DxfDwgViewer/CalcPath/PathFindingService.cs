namespace DxfDwgViewer.CalcPath
{
    public class PathFindingService
    {
        private const int DefaultGridWidth = 100;
        private const int DefaultGridHeight = 100;
        private const bool DefaultAllowDiagonal = true;

        public PathFindingService()
        {
        }

        public PathFindingResult FindPathWithWaypoints(List<(int X, int Y)> waypoints, List<(int X, int Y)> obstacles = null, int gridWidth = 100, int gridHeight = 100, bool allowDiagonal = true)
        {
            if (waypoints == null || waypoints.Count < 2)
            {
                return new PathFindingResult
                {
                    Success = false,
                    Message = "路径点数量必须大于等于2"
                };
            }

            foreach (var waypoint in waypoints)
            {
                if (waypoint.X < 0 || waypoint.X >= gridWidth || waypoint.Y < 0 || waypoint.Y >= gridHeight)
                {
                    return new PathFindingResult
                    {
                        Success = false,
                        Message = $"路径点 ({waypoint.X}, {waypoint.Y}) 超出地图范围，地图大小为 {gridWidth}x{gridHeight}"
                    };
                }
            }

            if (obstacles != null)
            {
                foreach (var obstacle in obstacles)
                {
                    if (obstacle.X < 0 || obstacle.X >= gridWidth || obstacle.Y < 0 || obstacle.Y >= gridHeight)
                    {
                        return new PathFindingResult
                        {
                            Success = false,
                            Message = $"障碍物 ({obstacle.X}, {obstacle.Y}) 超出地图范围，地图大小为 {gridWidth}x{gridHeight}"
                        };
                    }
                }
            }

            var gridMap = new GridMap(gridWidth, gridHeight);
            var pathFinder = new AStarPathFinder(gridMap, allowDiagonal);

            if (obstacles != null && obstacles.Count > 0)
            {
                foreach (var obstacle in obstacles)
                {
                    gridMap.SetWalkable(obstacle.X, obstacle.Y, false);
                }
            }

            var fullPath = new List<(int, int)>();
            double totalCost = 0;
            int totalNodesExplored = 0;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                var start = waypoints[i];
                var end = waypoints[i + 1];

                var segmentResult = pathFinder.FindPath(start.X, start.Y, end.X, end.Y);

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
    }
}
