namespace DxfDwgViewer.CalcPath
{
    public class PathFindingResult
    {
        public bool Success { get; set; }
        public List<PointCoordinate> Path { get; set; } = new List<PointCoordinate>();
        public double TotalCost { get; set; }
        public int NodesExplored { get; set; }
        public string Message { get; set; } = string.Empty;
        public long ExecutionTimeMs { get; set; }
    }
}
