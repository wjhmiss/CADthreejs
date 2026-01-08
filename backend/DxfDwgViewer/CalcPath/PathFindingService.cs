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

        public PathFindingResult FindPath(int startX, int startY, int endX, int endY)
        {
            return _pathFinder.FindPath(startX, startY, endX, endY);
        }

        public PathFindingResult FindPathWithObstacles(int startX, int startY, int endX, int endY, List<(int X, int Y)> obstacles)
        {
            ClearObstacles();
            SetObstacles(obstacles);
            return FindPath(startX, startY, endX, endY);
        }

        public int GridWidth => _gridMap.Width;
        public int GridHeight => _gridMap.Height;
    }
}
