using System;
using System.Collections.Generic;
using ACadSharp.Entities;
using ACadSharp.Extensions;
using CSMath;

namespace DxfDwgViewer.RenderUtilities
{
    public class HatchEntityRenderer
    {
        public class HatchData
        {
            // three.js 对象类型标识
            public string Type { get; set; }
            public string Uuid { get; set; }
            public string EntityType { get; set; }

            // 基本属性
            public bool IsSolid { get; set; }
            public PatternData Pattern { get; set; }
            public List<PathData> Paths { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }

            // 实体属性
            public string Handle { get; set; }
            public string LayerName { get; set; }
            public bool IsInvisible { get; set; }
            public double LineTypeScale { get; set; }
            public double Transparency { get; set; }

            // 样式属性
            public string ColorHex { get; set; }
            public int ColorR { get; set; }
            public int ColorG { get; set; }
            public int ColorB { get; set; }
            public int ColorA { get; set; }

            // 材质属性
            public string MaterialType { get; set; }
            public bool MaterialTransparent { get; set; }
            public double MaterialOpacity { get; set; }
            public bool MaterialDepthTest { get; set; }
            public bool MaterialDepthWrite { get; set; }
            public int MaterialSide { get; set; }

            // 几何属性
            public double Area { get; set; }
            public PointData Centroid { get; set; }
            public BoundsData Bounds { get; set; }
            public Point3DData Centroid3D { get; set; }

            // 法线向量（用于3D渲染）
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }

            // 用于three.js绘制的点列表
            public List<Point3DData> Points { get; set; }
            public List<double> Vertices { get; set; }
            public List<int> Indices { get; set; }
            public List<double> Normals { get; set; }

            // 图案属性
            public string PatternName { get; set; }
            public double PatternAngle { get; set; }
            public double PatternScale { get; set; }
            public string PatternType { get; set; }
            public bool IsDouble { get; set; }

            // 渐变属性
            public bool HasGradient { get; set; }
            public string GradientColorName { get; set; }

            // 边界路径信息
            public int PathCount { get; set; }
            public int TotalEdges { get; set; }
            public List<string> EdgeTypes { get; set; }

            // 坐标系和变换
            public double Elevation { get; set; }
            public string CoordinateSystem { get; set; }
            public bool RequiresYAxisFlip { get; set; }
            public TransformData Transform { get; set; }

            // three.js 场景属性
            public object Parent { get; set; }
            public bool Visible { get; set; }
            public bool CastShadow { get; set; }
            public bool ReceiveShadow { get; set; }
            public int RenderOrder { get; set; }
        }

        public class PatternData
        {
            public string Name { get; set; }
            public List<PatternLine> Lines { get; set; }
        }

        public class PatternLine
        {
            public double Angle { get; set; }
            public PointData BasePoint { get; set; }
            public PointData Offset { get; set; }
            public List<double> DashLengths { get; set; }
        }

        public class PathData
        {
            public List<PointData> Points { get; set; }
            public List<EdgeData> Edges { get; set; }
            public bool IsClosed { get; set; }
            public double PathArea { get; set; }
            public PointData PathCentroid { get; set; }
        }

        public class EdgeData
        {
            public string Type { get; set; }
            public LineEdgeData LineEdge { get; set; }
            public ArcEdgeData ArcEdge { get; set; }
        }

        public class LineEdgeData
        {
            public PointData Start { get; set; }
            public PointData End { get; set; }
            public double Length { get; set; }
        }

        public class ArcEdgeData
        {
            public PointData Center { get; set; }
            public double Radius { get; set; }
            public double StartAngle { get; set; }
            public double EndAngle { get; set; }
            public bool IsCounterClockwise { get; set; }
            public double Length { get; set; }
            public bool IsCCW { get; set; }
        }

        public static HatchData Render(Hatch hatch)
        {
            var colorInfo = GetColorInfo(hatch.Color.Index);

            double transparency = 1.0;
            if (hatch.Transparency.IsByLayer)
            {
                transparency = 0.0;
            }
            else if (hatch.Transparency.IsByBlock)
            {
                transparency = 0.0;
            }
            else
            {
                transparency = hatch.Transparency.Value / 100.0;
            }
            double opacity = 1.0 - transparency;

            string handle = hatch.Handle.ToString();
            string layerName = hatch.Layer?.Name ?? "0";
            bool isInvisible = hatch.IsInvisible;
            double lineTypeScale = hatch.LineTypeScale;

            XYZ normal = hatch.Normal;

            var hatchData = new HatchData
            {
                Type = "Hatch",
                Uuid = Guid.NewGuid().ToString(),
                EntityType = "HATCH",
                IsSolid = hatch.IsSolid,
                ColorIndex = hatch.Color.Index,
                LineTypeName = hatch.GetActiveLineType().Name,
                LineWeight = hatch.GetActiveLineWeightType().GetLineWeightValue(),
                Paths = new List<PathData>(),
                Handle = handle,
                LayerName = layerName,
                IsInvisible = isInvisible,
                LineTypeScale = lineTypeScale,
                Transparency = transparency,
                ColorHex = colorInfo.Hex,
                ColorR = colorInfo.R,
                ColorG = colorInfo.G,
                ColorB = colorInfo.B,
                ColorA = (int)(opacity * 255),
                MaterialType = hatch.IsSolid ? "MeshBasicMaterial" : "MeshPhongMaterial",
                MaterialTransparent = opacity < 1.0,
                MaterialOpacity = opacity,
                MaterialDepthTest = true,
                MaterialDepthWrite = true,
                MaterialSide = 2,
                Area = 0,
                Centroid = new PointData(0, 0),
                Centroid3D = new Point3DData(0, 0, 0),
                Bounds = new BoundsData(new Point3DData(0, 0, 0), new Point3DData(0, 0, 0)),
                NormalX = normal.X,
                NormalY = normal.Y,
                NormalZ = normal.Z,
                Points = new List<Point3DData>(),
                Vertices = new List<double>(),
                Indices = new List<int>(),
                Normals = new List<double>(),
                PatternName = hatch.Pattern?.Name ?? "SOLID",
                PatternAngle = hatch.PatternAngle,
                PatternScale = hatch.PatternScale,
                PatternType = hatch.PatternType.ToString(),
                IsDouble = hatch.IsDouble,
                HasGradient = hatch.GradientColor != null,
                GradientColorName = hatch.GradientColor?.Name ?? "",
                PathCount = hatch.Paths.Count,
                TotalEdges = 0,
                EdgeTypes = new List<string>(),
                Elevation = hatch.Elevation,
                CoordinateSystem = "AutoCAD",
                RequiresYAxisFlip = true,
                Transform = new TransformData(),
                Parent = null!,
                Visible = !isInvisible,
                CastShadow = false,
                ReceiveShadow = false,
                RenderOrder = 0
            };

            if (hatch.Pattern != null)
            {
                hatchData.Pattern = new PatternData
                {
                    Name = hatch.Pattern.Name,
                    Lines = hatch.Pattern.Lines?.Select(l => new PatternLine
                    {
                        Angle = l.Angle,
                        BasePoint = new PointData(l.BasePoint.X, l.BasePoint.Y),
                        Offset = new PointData(l.Offset.X, l.Offset.Y),
                        DashLengths = l.DashLengths?.ToList() ?? new List<double>()
                    }).ToList() ?? new List<PatternLine>()
                };
            }

            double totalArea = 0;
            double totalCentroidX = 0;
            double totalCentroidY = 0;
            double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;
            int vertexIndex = 0;

            foreach (var path in hatch.Paths)
            {
                var pathData = new PathData
                {
                    Points = new List<PointData>(),
                    Edges = new List<EdgeData>()
                };

                try
                {
                    var pointsSource = path.GetPoints(64);
                    
                    var verticesStorage = new List<XYZ>();
                    var pointsIterator = pointsSource.GetEnumerator();
                    while (pointsIterator.MoveNext())
                    {
                        verticesStorage.Add(pointsIterator.Current);
                    }
                    
                    var dataStorage = new List<PointData>();
                    var storageSize = verticesStorage.Count;
                    for (var loopIndex = 0; loopIndex < storageSize; loopIndex++)
                    {
                        var pointItem = verticesStorage[loopIndex];
                        var dataItem = new PointData(pointItem.X, pointItem.Y);
                        dataStorage.Add(dataItem);
                        
                        var point3D = new Point3DData(pointItem.X, pointItem.Y, hatch.Elevation);
                        hatchData.Points.Add(point3D);
                        hatchData.Vertices.Add(pointItem.X);
                        hatchData.Vertices.Add(pointItem.Y);
                        hatchData.Vertices.Add(hatch.Elevation);
                        
                        hatchData.Normals.Add(normal.X);
                        hatchData.Normals.Add(normal.Y);
                        hatchData.Normals.Add(normal.Z);
                        
                        minX = Math.Min(minX, pointItem.X);
                        minY = Math.Min(minY, pointItem.Y);
                        minZ = Math.Min(minZ, hatch.Elevation);
                        maxX = Math.Max(maxX, pointItem.X);
                        maxY = Math.Max(maxY, pointItem.Y);
                        maxZ = Math.Max(maxZ, hatch.Elevation);
                    }
                    
                    pathData.Points = dataStorage;

                    var vertexCount = verticesStorage.Count;
                    pathData.IsClosed = vertexCount > 2;
                    
                    if (pathData.IsClosed && vertexCount > 0)
                    {
                        var areaValue = 0.0;
                        var centerX = 0.0;
                        var centerY = 0.0;
                        
                        for (var loopIndex = 0; loopIndex < vertexCount; loopIndex++)
                        {
                            var nextIndex = (loopIndex + 1) % vertexCount;
                            
                            var currentVertex = verticesStorage[loopIndex];
                            var nextVertex = verticesStorage[nextIndex];
                            
                            var crossResult = currentVertex.X * nextVertex.Y - nextVertex.X * currentVertex.Y;
                            areaValue += crossResult;
                            centerX += (currentVertex.X + nextVertex.X) * crossResult;
                            centerY += (currentVertex.Y + nextVertex.Y) * crossResult;
                        }
                        
                        pathData.PathArea = Math.Abs(areaValue / 2.0);
                        totalArea += pathData.PathArea;
                        
                        if (areaValue != 0)
                        {
                            centerX /= (6 * areaValue);
                            centerY /= (6 * areaValue);
                            pathData.PathCentroid = new PointData(centerX, centerY);
                            
                            totalCentroidX += centerX * pathData.PathArea;
                            totalCentroidY += centerY * pathData.PathArea;
                        }

                        if (vertexCount >= 3)
                        {
                            for (int i = 1; i < vertexCount - 1; i++)
                            {
                                hatchData.Indices.Add(vertexIndex);
                                hatchData.Indices.Add(vertexIndex + i);
                                hatchData.Indices.Add(vertexIndex + i + 1);
                            }
                        }
                    }

                    if (path.Edges != null)
                    {
                        foreach (var edge in path.Edges)
                        {
                            hatchData.TotalEdges++;
                            hatchData.EdgeTypes.Add(edge.GetType().Name);

                            var edgeData = new EdgeData
                            {
                                Type = edge.GetType().Name
                            };

                            if (edge is Hatch.BoundaryPath.Line lineEdge)
                            {
                                double length = System.Math.Sqrt(
                                    System.Math.Pow(lineEdge.End.X - lineEdge.Start.X, 2) + 
                                    System.Math.Pow(lineEdge.End.Y - lineEdge.Start.Y, 2));
                                
                                edgeData.LineEdge = new LineEdgeData
                                {
                                    Start = new PointData(lineEdge.Start.X, lineEdge.Start.Y),
                                    End = new PointData(lineEdge.End.X, lineEdge.End.Y),
                                    Length = length
                                };
                            }
                            else if (edge is Hatch.BoundaryPath.Arc arcEdge)
                            {
                                double deltaAngle = System.Math.Abs(arcEdge.EndAngle - arcEdge.StartAngle);
                                if (deltaAngle > 2 * System.Math.PI) 
                                    deltaAngle = 2 * System.Math.PI;
                                double length = deltaAngle * arcEdge.Radius;
                                
                                edgeData.ArcEdge = new ArcEdgeData
                                {
                                    Center = new PointData(arcEdge.Center.X, arcEdge.Center.Y),
                                    Radius = arcEdge.Radius,
                                    StartAngle = arcEdge.StartAngle,
                                    EndAngle = arcEdge.EndAngle,
                                    IsCounterClockwise = arcEdge.CounterClockWise,
                                    Length = length,
                                    IsCCW = arcEdge.CounterClockWise
                                };
                            }

                            pathData.Edges.Add(edgeData);
                        }
                    }

                    hatchData.Paths.Add(pathData);
                    vertexIndex += vertexCount;
                }
                catch
                {
                }
            }

            hatchData.Area = totalArea;
            
            if (totalArea > 0)
            {
                hatchData.Centroid = new PointData(totalCentroidX / totalArea, totalCentroidY / totalArea);
                hatchData.Centroid3D = new Point3DData(totalCentroidX / totalArea, totalCentroidY / totalArea, hatch.Elevation);
            }
            else if (hatchData.Points.Count > 0)
            {
                double sumX = 0, sumY = 0;
                foreach (var point in hatchData.Points)
                {
                    sumX += point.X;
                    sumY += point.Y;
                }
                hatchData.Centroid = new PointData(sumX / hatchData.Points.Count, sumY / hatchData.Points.Count);
                hatchData.Centroid3D = new Point3DData(sumX / hatchData.Points.Count, sumY / hatchData.Points.Count, hatch.Elevation);
            }

            if (minX != double.MaxValue)
            {
                hatchData.Bounds = new BoundsData(
                    new Point3DData(minX, minY, minZ),
                    new Point3DData(maxX, maxY, maxZ)
                );
            }

            var transform = new TransformData();
            transform.Matrix = new double[16]
            {
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                hatchData.Centroid3D.X, hatchData.Centroid3D.Y, hatchData.Centroid3D.Z, 1
            };
            hatchData.Transform = transform;

            return hatchData;
        }

        private static (int R, int G, int B, string Hex) GetColorInfo(short colorIndex)
        {
            int r, g, b;
            switch (colorIndex)
            {
                case 1: r = 255; g = 0; b = 0; break;
                case 2: r = 255; g = 255; b = 0; break;
                case 3: r = 0; g = 255; b = 0; break;
                case 4: r = 0; g = 255; b = 255; break;
                case 5: r = 0; g = 0; b = 255; break;
                case 6: r = 255; g = 0; b = 255; break;
                case 7: r = 255; g = 255; b = 255; break;
                default: r = 0; g = 0; b = 0; break;
            }
            return (r, g, b, $"#{r:X2}{g:X2}{b:X2}");
        }
    }
}
