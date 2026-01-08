namespace DxfDwgViewer.CalcPath
{
    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double G { get; set; }
        public double H { get; set; }
        public double F => G + H;
        public Node? Parent { get; set; }
        public bool IsWalkable { get; set; } = true;

        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Node other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
