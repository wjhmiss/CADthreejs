using System.Linq;
using ACadSharp.Entities;
using System.Collections.Generic;
using ACadSharp.Extensions;
using CSMath;

namespace DxfDwgViewer.RenderUtilities
{
    public class SplineEntityRenderer
    {
        public class SplineData
        {
            public string Type { get; set; } = "Spline";
            public bool Visible { get; set; } = true;
            public List<Point3DData> ControlPoints { get; set; }
            public List<Point3DData> FitPoints { get; set; }
            public List<double> Knots { get; set; }
            public List<double> Weights { get; set; }
            public int Degree { get; set; }
            public bool IsClosed { get; set; }
            public bool Closed { get; set; }
            public bool IsPeriodic { get; set; }
            public bool Periodic { get; set; }
            public Point3DData StartTangent { get; set; }
            public Point3DData EndTangent { get; set; }
            public Point3DData Normal { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public string LayerName { get; set; }
            public double LineWeight { get; set; }
            public double ControlPointTolerance { get; set; }
            public double FitTolerance { get; set; }
            public double KnotTolerance { get; set; }
            public List<Point3DData> ApproximationPoints { get; set; }
            public int ControlPointCount { get; set; }
            public int FitPointCount { get; set; }
            public int KnotCount { get; set; }
            public BoundsData Bounds { get; set; }
            public Point3DData Centroid { get; set; }
            public double Length { get; set; }
            
            public TransformData Transform { get; set; }
            public GeometryData Geometry { get; set; }
            public MaterialData Material { get; set; }
            public double[] VertexPositions { get; set; }
            public double[] VertexColors { get; set; }
            public int[] Indices { get; set; }
            public int ApproximationPointCount { get; set; }
            public double Tension { get; set; }
            public string SplineType { get; set; }
            public ColorData Color { get; set; }
            public NormalData Tangent { get; set; }
            public double[] Binormal { get; set; }
            public double[] UV { get; set; }
            public double ArcLength { get; set; }
            public double Curvature { get; set; }
            public bool IsRational { get; set; }
            public int SampleCount { get; set; }
            public double[] ControlPointWeights { get; set; }
        }

        public static SplineData Render(Spline spline)
        {
            var splineData = new SplineData
            {
                ControlPoints = new List<Point3DData>(),
                FitPoints = new List<Point3DData>(),
                Knots = new List<double>(spline.Knots),
                Weights = new List<double>(spline.Weights),
                Degree = spline.Degree,
                IsClosed = spline.IsClosed,
                Closed = spline.IsClosed,
                IsPeriodic = spline.IsPeriodic,
                Periodic = spline.IsPeriodic,
                StartTangent = new Point3DData(spline.StartTangent.X, spline.StartTangent.Y, spline.StartTangent.Z),
                EndTangent = new Point3DData(spline.EndTangent.X, spline.EndTangent.Y, spline.EndTangent.Z),
                Normal = new Point3DData(spline.Normal.X, spline.Normal.Y, spline.Normal.Z),
                ColorIndex = spline.Color.Index,
                LineTypeName = spline.GetActiveLineType()?.Name ?? "",
                LayerName = spline.Layer?.Name ?? "",
                LineWeight = spline.GetActiveLineWeightType().GetLineWeightValue(),
                ControlPointTolerance = spline.ControlPointTolerance,
                FitTolerance = spline.FitTolerance,
                KnotTolerance = spline.KnotTolerance,
                ApproximationPoints = new List<Point3DData>(),
                IsRational = spline.Weights.Count > 0,
                Visible = !spline.IsInvisible
            };

            foreach (var point in spline.ControlPoints)
            {
                splineData.ControlPoints.Add(new Point3DData(point.X, point.Y, point.Z));
            }

            foreach (var point in spline.FitPoints)
            {
                splineData.FitPoints.Add(new Point3DData(point.X, point.Y, point.Z));
            }

            splineData.ControlPointCount = splineData.ControlPoints.Count;
            splineData.FitPointCount = splineData.FitPoints.Count;
            splineData.KnotCount = splineData.Knots.Count;
            splineData.ControlPointWeights = spline.Weights.Count > 0 ? spline.Weights.ToArray() : new double[spline.ControlPoints.Count];

            var vertexPointsList = new List<CSMath.XYZ>();
            if (spline.ControlPoints.Count > 1)
            {
                if (spline.TryPolygonalVertexes(64, out var vertexPoints))
                {
                    vertexPointsList.AddRange(vertexPoints);
                    var polygonPoints = vertexPoints!.Select(p =>
                        new PointData(p.X, p.Y)).ToArray();
                    
                    foreach (var point in vertexPoints)
                    {
                        splineData.ApproximationPoints.Add(new Point3DData(point.X, point.Y, point.Z));
                    }
                    
                    splineData.Length = CalculateArcLength(polygonPoints);
                }
                else if (spline.FitPoints.Count > 1)
                {
                    var fitPoints = spline.FitPoints.Select(p =>
                        new PointData(p.X, p.Y)).ToArray();
                    
                    foreach (var point in spline.FitPoints)
                    {
                        vertexPointsList.Add(point);
                    }
                    
                    foreach (var point in spline.FitPoints)
                    {
                        splineData.ApproximationPoints.Add(new Point3DData(point.X, point.Y, point.Z));
                    }
                    
                    splineData.Length = CalculateArcLength(fitPoints);
                }
            }
            else if (spline.FitPoints.Count > 1)
            {
                var fitPoints = spline.FitPoints.Select(p =>
                    new PointData(p.X, p.Y)).ToArray();
                
                foreach (var point in spline.FitPoints)
                {
                    vertexPointsList.Add(point);
                }
                
                foreach (var point in spline.FitPoints)
                {
                    splineData.ApproximationPoints.Add(new Point3DData(point.X, point.Y, point.Z));
                }
                
                splineData.Length = CalculateArcLength(fitPoints);
            }
            
            splineData.ArcLength = splineData.Length;
            splineData.SampleCount = splineData.ApproximationPoints.Count;
            
            BoundsData? bounds = null;
            Point3DData? centroid = null;
            
            if (vertexPointsList.Count > 0)
            {
                double minX = vertexPointsList[0].X, maxX = vertexPointsList[0].X;
                double minY = vertexPointsList[0].Y, maxY = vertexPointsList[0].Y;
                double minZ = vertexPointsList[0].Z, maxZ = vertexPointsList[0].Z;
                double sumX = 0, sumY = 0, sumZ = 0;
                
                foreach (var point in vertexPointsList)
                {
                    if (point.X < minX) minX = point.X;
                    if (point.X > maxX) maxX = point.X;
                    if (point.Y < minY) minY = point.Y;
                    if (point.Y > maxY) maxY = point.Y;
                    if (point.Z < minZ) minZ = point.Z;
                    if (point.Z > maxZ) maxZ = point.Z;
                    
                    sumX += point.X;
                    sumY += point.Y;
                    sumZ += point.Z;
                }
                
                bounds = new BoundsData(
                    new Point3DData(minX, minY, minZ),
                    new Point3DData(maxX, maxY, maxZ)
                );
                
                centroid = new Point3DData(
                    sumX / vertexPointsList.Count,
                    sumY / vertexPointsList.Count,
                    sumZ / vertexPointsList.Count
                );
            }

            splineData.Bounds = bounds!;
            splineData.Centroid = centroid!;
            
            int colorValue = GetColorRgbByIndex(spline.Color.Index);
            
            var transform = new TransformData();
            if (centroid != null)
            {
                transform.Position = centroid;
            }
            
            var geometry = new GeometryData
            {
                Type = spline.Weights.Count > 0 ? "NURBSCurve" : "CatmullRomCurve3",
                VertexCount = splineData.ApproximationPoints?.Count ?? 0,
                HasColors = true,
                HasIndices = true,
                PrimitiveType = "LineLoop",
                IndexCount = splineData.ApproximationPoints?.Count * 2 ?? 0
            };
            
            var material = new MaterialData
            {
                Type = "LineBasicMaterial",
                Color = colorValue,
                Opacity = 1.0,
                Transparent = false,
                Wireframe = false,
                LineWidth = splineData.LineWeight > 0 ? splineData.LineWeight : 1.0,
                VertexColors = true,
                Side = true
            };
            
            var vertexPositions = new List<double>();
            if (splineData.ApproximationPoints != null)
            {
                foreach (var point in splineData.ApproximationPoints)
                {
                    vertexPositions.Add(point.X);
                    vertexPositions.Add(point.Y);
                    vertexPositions.Add(point.Z);
                }
            }
            
            var vertexColors = new List<double>();
            if (splineData.ApproximationPoints != null)
            {
                for (int i = 0; i < splineData.ApproximationPoints.Count; i++)
                {
                    vertexColors.Add((colorValue >> 16) & 0xFF);
                    vertexColors.Add((colorValue >> 8) & 0xFF);
                    vertexColors.Add(colorValue & 0xFF);
                }
            }
            
            var indices = new List<int>();
            if (splineData.ApproximationPoints != null)
            {
                for (int i = 0; i < splineData.ApproximationPoints.Count - 1; i++)
                {
                    indices.Add(i);
                    indices.Add(i + 1);
                }
                
                if (spline.IsClosed && splineData.ApproximationPoints.Count > 2)
                {
                    indices.Add(splineData.ApproximationPoints.Count - 1);
                    indices.Add(0);
                }
            }
            
            var uv = new List<double>();
            if (splineData.ApproximationPoints != null && splineData.ApproximationPoints.Count > 0)
            {
                for (int i = 0; i < splineData.ApproximationPoints.Count; i++)
                {
                    double t = splineData.Length > 0 ? (double)i / (splineData.ApproximationPoints.Count - 1) : 0;
                    uv.Add(t);
                    uv.Add(0);
                }
            }
            
            var binormal = new List<double>();
            if (splineData.ApproximationPoints != null && splineData.ApproximationPoints.Count >= 2)
            {
                var tangent = CalculateTangent(splineData.ApproximationPoints);
                splineData.Tangent = new NormalData(tangent[0], tangent[1], tangent[2]);
                var normal = new NormalData(splineData.Normal.X, splineData.Normal.Y, splineData.Normal.Z);
                var cross = NormalData.Cross(splineData.Tangent, normal);
                binormal.Add(cross.X);
                binormal.Add(cross.Y);
                binormal.Add(cross.Z);
            }
            
            splineData.Transform = transform;
            splineData.Geometry = geometry;
            splineData.Material = material;
            splineData.VertexPositions = vertexPositions.ToArray();
            splineData.VertexColors = vertexColors.ToArray();
            splineData.Indices = indices.ToArray();
            splineData.ApproximationPointCount = splineData.ApproximationPoints?.Count ?? 0;
            splineData.Tension = 0.5;
            splineData.SplineType = spline.Weights.Count > 0 ? "NURBSCurve" : "CatmullRomCurve3";
            splineData.Color = new ColorData(spline.Color.Index);
            splineData.Binormal = binormal.ToArray();
            splineData.UV = uv.ToArray();
            splineData.Curvature = splineData.ApproximationPoints != null ? CalculateCurvature(splineData.ApproximationPoints) : 0;

            return splineData;
        }

        private static int GetColorRgbByIndex(short colorIndex)
        {
            switch (colorIndex)
            {
                case 1: return 0xFF0000;
                case 2: return 0xFFFF00;
                case 3: return 0x00FF00;
                case 4: return 0x00FFFF;
                case 5: return 0x0000FF;
                case 6: return 0xFF00FF;
                case 7: return 0xFFFFFF;
                case 256: return 0xB3B3B3;
                default: return 0x000000;
            }
        }
        
        private static double CalculateArcLength(PointData[] points)
        {
            double length = 0;
            for (int i = 0; i < points.Length - 1; i++)
            {
                double dx = points[i + 1].X - points[i].X;
                double dy = points[i + 1].Y - points[i].Y;
                length += Math.Sqrt(dx * dx + dy * dy);
            }
            return length;
        }
        
        private static double[] CalculateTangent(List<Point3DData> points)
        {
            if (points.Count < 2)
            {
                return new double[] { 1, 0, 0 };
            }
            
            double dx = points[1].X - points[0].X;
            double dy = points[1].Y - points[0].Y;
            double dz = points[1].Z - points[0].Z;
            double length = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            
            if (length > 0)
            {
                return new double[] { dx / length, dy / length, dz / length };
            }
            return new double[] { 1, 0, 0 };
        }
        
        private static double CalculateCurvature(List<Point3DData> points)
        {
            if (points.Count < 3)
            {
                return 0;
            }
            
            double maxCurvature = 0;
            for (int i = 1; i < points.Count - 1; i++)
            {
                var p0 = points[i - 1];
                var p1 = points[i];
                var p2 = points[i + 1];
                
                double v1x = p1.X - p0.X;
                double v1y = p1.Y - p0.Y;
                double v1z = p1.Z - p0.Z;
                double v2x = p2.X - p1.X;
                double v2y = p2.Y - p1.Y;
                double v2z = p2.Z - p1.Z;
                
                double crossX = v1y * v2z - v1z * v2y;
                double crossY = v1z * v2x - v1x * v2z;
                double crossZ = v1x * v2y - v1y * v2x;
                double crossLength = Math.Sqrt(crossX * crossX + crossY * crossY + crossZ * crossZ);
                
                double len1 = Math.Sqrt(v1x * v1x + v1y * v1y + v1z * v1z);
                double len2 = Math.Sqrt(v2x * v2x + v2y * v2y + v2z * v2z);
                
                if (len1 > 0 && len2 > 0)
                {
                    double curvature = 2 * crossLength / (len1 * len2);
                    if (curvature > maxCurvature)
                    {
                        maxCurvature = curvature;
                    }
                }
            }
            return maxCurvature;
        }
    }
}