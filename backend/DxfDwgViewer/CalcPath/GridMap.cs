namespace DxfDwgViewer.CalcPath
{
    public class GridMap
    {
        private readonly Node[,] _grid;
        public int Width { get; }
        public int Height { get; }

        public GridMap(int width, int height)
        {
            Width = width;
            Height = height;
            _grid = new Node[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _grid[y, x] = new Node(x, y);
                }
            }
        }

        public Node GetNode(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException($"Coordinates ({x}, {y}) are out of bounds");
            }
            return _grid[y, x];
        }

        public void SetWalkable(int x, int y, bool walkable)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                _grid[y, x].IsWalkable = walkable;
            }
        }

        public List<Node> GetNeighbors(Node node, bool allowDiagonal = true)
        {
            var neighbors = new List<Node>();
            int x = node.X;
            int y = node.Y;

            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
                {
                    neighbors.Add(_grid[ny, nx]);
                }
            }

            if (allowDiagonal)
            {
                int[] ddx = { -1, -1, 1, 1 };
                int[] ddy = { -1, 1, -1, 1 };

                for (int i = 0; i < 4; i++)
                {
                    int nx = x + ddx[i];
                    int ny = y + ddy[i];

                    if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
                    {
                        neighbors.Add(_grid[ny, nx]);
                    }
                }
            }

            return neighbors;
        }

        public void Reset()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _grid[y, x].G = 0;
                    _grid[y, x].H = 0;
                    _grid[y, x].Parent = null;
                }
            }
        }
    }
}
