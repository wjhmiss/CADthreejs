namespace DxfDwgViewer.CalcPath
{
    public class PathFindingResult
    {
        public bool Success { get; set; }
        public List<(int X, int Y)> Path { get; set; } = new List<(int, int)>();
        public double TotalCost { get; set; }
        public int NodesExplored { get; set; }
        public string Message { get; set; } = string.Empty;
        public long ExecutionTimeMs { get; set; }
    }
}
