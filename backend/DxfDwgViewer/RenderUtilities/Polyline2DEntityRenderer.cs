using System.Linq;
using ACadSharp.Entities;
using CSMath;
using ACadSharp.Extensions;
using System.Collections.Generic;

namespace DxfDwgViewer.RenderUtilities
{
    public class Polyline2DEntityRenderer
    {
        public class Polyline2DData
        {
            public List<VertexData> Vertices { get; set; }
            public bool IsClosed { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            public double LineTypeScale { get; set; }
            public double Elevation { get; set; }
            public double Thickness { get; set; }
            public double StartWidth { get; set; }
            public double EndWidth { get; set; }
            public SmoothSurfaceType SmoothSurface { get; set; }
            
            // 基本几何属性
            public int VertexCount { get; set; }
            public int SegmentCount { get; set; }
            public BoundsData3D? Bounds3D { get; set; }
            public BoundsData? Bounds { get; set; }
            public Point3DData? Centroid { get; set; }
            public double TotalLength { get; set; }
            
            // three.js兼容性属性
            public List<Point3DData> Vertices3D { get; set; }
            public List<int> Indices { get; set; }
            public List<double> NormalsArray { get; set; }
            public List<double> ColorsArray { get; set; }
            public List<double> UVsArray { get; set; }
            public ColorData Color { get; set; }
            public NormalData Normal { get; set; }
            public TransformData Transform { get; set; }
            
            // 实体属性
            public string EntityType { get; set; }
            public bool Visible { get; set; }
            public string LayerName { get; set; }
            public int LayerIndex { get; set; }
            public string Handle { get; set; }
            
            // 材质属性
            public double Transparency { get; set; }
            public string MaterialName { get; set; }
            public MaterialData Material { get; set; }
            
            // 渲染属性
            public bool CastShadows { get; set; }
            public bool ReceiveShadows { get; set; }
            
            // three.js几何体属性
            public string GeometryType { get; set; }
            public bool DoubleSided { get; set; }
            public bool FlatShading { get; set; }
            
            // 圆弧段属性
            public bool HasArcSegments { get; set; }
            public List<ArcSegmentData> ArcSegments { get; set; }
            
            // 线宽属性
            public bool HasVariableWidth { get; set; }
            
            // 几何体验证属性
            public GeometryData Geometry { get; set; }
            
            // three.js顶点数据属性
            public List<double> VertexPositions { get; set; }
            public List<double> VertexNormals { get; set; }
            public List<double> VertexUVs { get; set; }
        }

        public class VertexData
        {
            public PointData Location { get; set; }
            public double Bulge { get; set; }
            public double StartWidth { get; set; }
            public double EndWidth { get; set; }
        }

        public class PointData
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
        }
        
        public class ArcSegmentData
        {
            public PointData StartPoint { get; set; }
            public PointData EndPoint { get; set; }
            public PointData CenterPoint { get; set; }
            public double Radius { get; set; }
            public double StartAngle { get; set; }
            public double EndAngle { get; set; }
            public double SweepAngle { get; set; }
            public bool IsCounterClockwise { get; set; }
            public int SegmentCount { get; set; } // 用于细分圆弧的线段数量
        }

        public class GeometryData
        {
            public string Type { get; set; } = "";
            public int VertexCount { get; set; }
            public int FaceCount { get; set; }
            public bool HasNormals { get; set; }
            public bool HasUVs { get; set; }
            public bool HasIndices { get; set; }
        }

        public class MaterialData
        {
            public string Type { get; set; } = "";
            public int Color { get; set; }
            public double Opacity { get; set; } = 1.0;
            public bool Transparent { get; set; }
            public bool Wireframe { get; set; }
            public double LineWidth { get; set; }
            public bool Side { get; set; }
        }

        public class BoundsData
        {
            public double MinX { get; set; }
            public double MaxX { get; set; }
            public double MinY { get; set; }
            public double MaxY { get; set; }
            public double MinZ { get; set; }
            public double MaxZ { get; set; }
        }

        public static Polyline2DData Render(Polyline2D polyline2D)
        {
            // 计算边界框和质心
            BoundsData3D? bounds3D = null;
            BoundsData? bounds = null;
            Point3DData? centroid = null;
            double totalLength = 0;
            int segmentCount = 0;
            bool hasVariableWidth = false;
            
            if (polyline2D.Vertices != null && polyline2D.Vertices.Any())
            {
                var vertices = polyline2D.Vertices.ToList();
                if (vertices.Count > 0)
                {
                    double minX = vertices[0]!.Location.X, maxX = vertices[0]!.Location.X;
                    double minY = vertices[0]!.Location.Y, maxY = vertices[0]!.Location.Y;
                    double minZ = vertices[0]!.Location.Z, maxZ = vertices[0]!.Location.Z;
                    double sumX = 0, sumY = 0, sumZ = 0;
                    
                    // 检查是否有可变宽度
                    foreach (var v in vertices)
                    {
                        if (v.StartWidth != 0 || v.EndWidth != 0)
                        {
                            hasVariableWidth = true;
                            break;
                        }
                    }
                    
                    // 计算长度
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        var currentVertex = vertices[i];
                        var nextVertex = (i + 1 < vertices.Count) ? vertices[i + 1] : null;
                        
                        // 更新边界框
                        if (currentVertex.Location.X < minX) minX = currentVertex.Location.X;
                        if (currentVertex.Location.X > maxX) maxX = currentVertex.Location.X;
                        if (currentVertex.Location.Y < minY) minY = currentVertex.Location.Y;
                        if (currentVertex.Location.Y > maxY) maxY = currentVertex.Location.Y;
                        if (currentVertex.Location.Z < minZ) minZ = currentVertex.Location.Z;
                        if (currentVertex.Location.Z > maxZ) maxZ = currentVertex.Location.Z;
                        
                        // 累加用于计算质心
                        sumX += currentVertex.Location.X;
                        sumY += currentVertex.Location.Y;
                        sumZ += currentVertex.Location.Z;
                        
                        // 计算线段长度
                        if (nextVertex != null)
                        {
                            segmentCount++;
                            if (currentVertex.Bulge == 0)
                            {
                                // 直线段长度
                                double dx = nextVertex.Location.X - currentVertex.Location.X;
                                double dy = nextVertex.Location.Y - currentVertex.Location.Y;
                                totalLength += Math.Sqrt(dx * dx + dy * dy);
                            }
                            else
                            {
                                // 圆弧段长度
                                XY startPoint = new XY(currentVertex.Location.X, currentVertex.Location.Y);
                                XY endPoint = new XY(nextVertex.Location.X, nextVertex.Location.Y);
                                double bulge = currentVertex.Bulge;
                                
                                // 计算圆心和半径
                                double alpha = Math.Atan(bulge) * 4.0;
                                double tanAlphaHalf = Math.Tan(alpha / 2.0);
                                
                                XY middlePoint = startPoint + (endPoint - startPoint) / 2.0;
                                XY perpendicular = XY.Rotate((endPoint - startPoint), Math.PI / 2.0);
                                XY centerPoint = middlePoint + (-perpendicular * tanAlphaHalf);
                                
                                double dx = startPoint.X - centerPoint.X;
                                double dy = startPoint.Y - centerPoint.Y;
                                double radius = Math.Sqrt(dx * dx + dy * dy);
                                
                                // 计算扫掠角
                                double startAngle = Math.Atan2(startPoint.Y - centerPoint.Y, startPoint.X - centerPoint.X);
                                double endAngle = Math.Atan2(endPoint.Y - centerPoint.Y, endPoint.X - centerPoint.X);
                                
                                double sweepAngle = endAngle - startAngle;
                                
                                // 根据凸度的正负调整扫掠方向
                                if (bulge < 0 && sweepAngle > 0)
                                    sweepAngle -= 2 * Math.PI;
                                else if (bulge > 0 && sweepAngle < 0)
                                    sweepAngle += 2 * Math.PI;
                                    
                                // 弧长 = 半径 * 扫掠角的绝对值
                                totalLength += Math.Abs(radius * sweepAngle);
                            }
                        }
                        else if (polyline2D.IsClosed && vertices.Count > 1)
                        {
                            segmentCount++;
                            // 闭合多段线的最后一段
                            var firstVertex = vertices[0];
                            if (currentVertex.Bulge == 0)
                            {
                                // 直线段长度
                                double dx = firstVertex.Location.X - currentVertex.Location.X;
                                double dy = firstVertex.Location.Y - currentVertex.Location.Y;
                                totalLength += Math.Sqrt(dx * dx + dy * dy);
                            }
                            else
                            {
                                // 圆弧段长度
                                XY startPoint = new XY(currentVertex.Location.X, currentVertex.Location.Y);
                                XY endPoint = new XY(firstVertex.Location.X, firstVertex.Location.Y);
                                double bulge = currentVertex.Bulge;
                                
                                // 计算圆心和半径
                                double alpha = Math.Atan(bulge) * 4.0;
                                double tanAlphaHalf = Math.Tan(alpha / 2.0);
                                
                                XY middlePoint = startPoint + (endPoint - startPoint) / 2.0;
                                XY perpendicular = XY.Rotate((endPoint - startPoint), Math.PI / 2.0);
                                XY centerPoint = middlePoint + (-perpendicular * tanAlphaHalf);
                                
                                double dx = startPoint.X - centerPoint.X;
                                double dy = startPoint.Y - centerPoint.Y;
                                double radius = Math.Sqrt(dx * dx + dy * dy);
                                
                                // 计算扫掠角
                                double startAngle = Math.Atan2(startPoint.Y - centerPoint.Y, startPoint.X - centerPoint.X);
                                double endAngle = Math.Atan2(endPoint.Y - centerPoint.Y, endPoint.X - centerPoint.X);
                                
                                double sweepAngle = endAngle - startAngle;
                                
                                // 根据凸度的正负调整扫掠方向
                                if (bulge < 0 && sweepAngle > 0)
                                    sweepAngle -= 2 * Math.PI;
                                else if (bulge > 0 && sweepAngle < 0)
                                    sweepAngle += 2 * Math.PI;
                                    
                                // 弧长 = 半径 * 扫掠角的绝对值
                                totalLength += Math.Abs(radius * sweepAngle);
                            }
                        }
                    }
                    
                    bounds3D = new BoundsData3D(
                        new Point3DData(minX, minY, minZ),
                        new Point3DData(maxX, maxY, maxZ)
                    );
                    
                    bounds = new BoundsData
                    {
                        MinX = minX,
                        MaxX = maxX,
                        MinY = minY,
                        MaxY = maxY,
                        MinZ = minZ,
                        MaxZ = maxZ
                    };
                    
                    centroid = new Point3DData(
                        sumX / vertices.Count,
                        sumY / vertices.Count,
                        sumZ / vertices.Count
                    );
                }
            }

            // 获取颜色索引
            short colorIndex = polyline2D.Color.Index;
            
            // 初始化three.js兼容性数据
            var vertices3D = new List<Point3DData>();
            var indices = new List<int>();
            var normalsArray = new List<double>();
            var colorsArray = new List<double>();
            var uvsArray = new List<double>();
            var arcSegments = new List<ArcSegmentData>();
            bool hasArcSegments = false;
            
            // 处理顶点数据，生成three.js兼容的数组
            if (polyline2D.Vertices != null)
            {
                int vertexIndex = 0;
                double accumulatedLength = 0;
                
                foreach (var vertex in polyline2D.Vertices)
                {
                    // 添加3D顶点位置
                    vertices3D.Add(new Point3DData(
                        vertex.Location.X,
                        vertex.Location.Y,
                        vertex.Location.Z
                    ));
                    
                    // 2D多段线的法线通常指向Z轴正方向
                    normalsArray.Add(0);
                    normalsArray.Add(0);
                    normalsArray.Add(1);
                    
                    // 基于长度参数化的UV映射
                    uvsArray.Add(accumulatedLength / (totalLength > 0 ? totalLength : 1));
                    uvsArray.Add(0);
                    
                    // 添加索引（用于绘制线段）
                    if (vertexIndex < polyline2D.Vertices.Count() - 1)
                    {
                        indices.Add(vertexIndex);
                        indices.Add(vertexIndex + 1);
                    }
                    else if (polyline2D.IsClosed && polyline2D.Vertices.Count() > 2)
                    {
                        // 闭合多段线连接最后一个顶点到第一个顶点
                        indices.Add(vertexIndex);
                        indices.Add(0);
                    }
                    
                    // 检查是否有圆弧段
                    if (vertex.Bulge != 0)
                    {
                        hasArcSegments = true;
                        var nextVertex = (vertexIndex < polyline2D.Vertices.Count() - 1) ? 
                                        polyline2D.Vertices.ElementAt(vertexIndex + 1) : 
                                        (polyline2D.IsClosed ? polyline2D.Vertices.First() : null);
                        
                        if (nextVertex != null)
                        {
                            // 计算圆弧参数
                            XY startPoint = new XY(vertex.Location.X, vertex.Location.Y);
                            XY endPoint = new XY(nextVertex.Location.X, nextVertex.Location.Y);
                            double bulge = vertex.Bulge;
                            
                            double alpha = Math.Atan(bulge) * 4.0;
                            double tanAlphaHalf = Math.Tan(alpha / 2.0);
                            
                            XY middlePoint = startPoint + (endPoint - startPoint) / 2.0;
                            XY perpendicular = XY.Rotate((endPoint - startPoint), Math.PI / 2.0);
                            XY centerPoint = middlePoint + (-perpendicular * tanAlphaHalf);
                            
                            double dx = startPoint.X - centerPoint.X;
                            double dy = startPoint.Y - centerPoint.Y;
                            double radius = Math.Sqrt(dx * dx + dy * dy);
                            
                            double startAngle = Math.Atan2(startPoint.Y - centerPoint.Y, startPoint.X - centerPoint.X);
                            double endAngle = Math.Atan2(endPoint.Y - centerPoint.Y, endPoint.X - centerPoint.X);
                            
                            double sweepAngle = endAngle - startAngle;
                            
                            if (bulge < 0 && sweepAngle > 0)
                                sweepAngle -= 2 * Math.PI;
                            else if (bulge > 0 && sweepAngle < 0)
                                sweepAngle += 2 * Math.PI;
                            
                            // 创建圆弧段数据
                            arcSegments.Add(new ArcSegmentData
                            {
                                StartPoint = new PointData { X = startPoint.X, Y = startPoint.Y },
                                EndPoint = new PointData { X = endPoint.X, Y = endPoint.Y },
                                CenterPoint = new PointData { X = centerPoint.X, Y = centerPoint.Y },
                                Radius = radius,
                                StartAngle = startAngle,
                                EndAngle = endAngle,
                                SweepAngle = sweepAngle,
                                IsCounterClockwise = sweepAngle > 0,
                                SegmentCount = Math.Max(8, (int)Math.Abs(sweepAngle * 180 / Math.PI / 10))
                            });
                        }
                    }
                    
                    // 更新累积长度用于UV映射
                    if (vertexIndex > 0)
                    {
                        var prevVertex = polyline2D.Vertices.ElementAt(vertexIndex - 1);
                        double dx = vertex.Location.X - prevVertex.Location.X;
                        double dy = vertex.Location.Y - prevVertex.Location.Y;
                        accumulatedLength += Math.Sqrt(dx * dx + dy * dy);
                    }
                    
                    vertexIndex++;
                }
            }
            
            // 获取颜色数据
            var color = new ColorData(colorIndex);
            
            // 创建颜色数组（每个顶点一个颜色）
            if (polyline2D.Vertices != null)
            {
                foreach (var vertex in polyline2D.Vertices)
                {
                    colorsArray.Add(color.R / 255.0);
                    colorsArray.Add(color.G / 255.0);
                    colorsArray.Add(color.B / 255.0);
                    colorsArray.Add(color.A);
                }
            }
            
            // 创建法向量数据（使用多段线的法向量）
            var normal = new NormalData(
                polyline2D.Normal.X,
                polyline2D.Normal.Y,
                polyline2D.Normal.Z
            );
            
            // 创建变换数据（包含elevation）
            var transform = new TransformData();
            // 设置平移分量（elevation在Z轴）
            transform.Matrix[14] = polyline2D.Elevation;
            
            var polyline2DData = new Polyline2DData
            {
                Vertices = new List<VertexData>(),
                IsClosed = polyline2D.IsClosed,
                ColorIndex = colorIndex,
                LineTypeName = polyline2D.GetActiveLineType()?.Name ?? "",
                LineWeight = polyline2D.GetActiveLineWeightType().GetLineWeightValue(),
                LineTypeScale = polyline2D.LineTypeScale,
                Elevation = polyline2D.Elevation,
                Thickness = polyline2D.Thickness,
                StartWidth = polyline2D.StartWidth,
                EndWidth = polyline2D.EndWidth,
                SmoothSurface = polyline2D.SmoothSurface,
                
                // 基本几何属性
                VertexCount = polyline2D.Vertices?.Count() ?? 0,
                SegmentCount = segmentCount,
                Bounds3D = bounds3D,
                Bounds = bounds,
                Centroid = centroid,
                TotalLength = totalLength,
                
                // three.js兼容性属性
                Vertices3D = vertices3D,
                Indices = indices,
                NormalsArray = normalsArray,
                ColorsArray = colorsArray,
                UVsArray = uvsArray,
                Color = color,
                Normal = normal,
                Transform = transform,
                
                // 实体属性
                EntityType = "Polyline2D",
                Visible = !polyline2D.IsInvisible,
                LayerName = polyline2D.Layer?.Name ?? "",
                LayerIndex = 0,
                Handle = polyline2D.Handle.ToString(),
                
                // 材质属性
                Transparency = 0.0,
                MaterialName = polyline2D.Material?.Name ?? "",
                Material = new MaterialData
                {
                    Type = "LineBasicMaterial",
                    Color = colorIndex,
                    Opacity = 1.0,
                    Transparent = false,
                    Wireframe = false,
                    LineWidth = polyline2D.GetActiveLineWeightType().GetLineWeightValue(),
                    Side = false
                },
                
                // 渲染属性
                CastShadows = false,
                ReceiveShadows = false,
                
                // three.js几何体属性
                GeometryType = "BufferGeometry",
                DoubleSided = false,
                FlatShading = false,
                
                // 圆弧段属性
                HasArcSegments = hasArcSegments,
                ArcSegments = arcSegments,
                
                // 线宽属性
                HasVariableWidth = hasVariableWidth,
                
                // 几何体验证属性
                Geometry = new GeometryData
                {
                    Type = "BufferGeometry",
                    VertexCount = vertices3D?.Count ?? 0,
                    FaceCount = 0,
                    HasNormals = normalsArray != null && normalsArray.Count > 0,
                    HasUVs = uvsArray != null && uvsArray.Count > 0,
                    HasIndices = indices != null && indices.Count > 0
                },
                
                // three.js顶点数据属性
                VertexPositions = vertices3D?.SelectMany(v => new double[] { v.X, v.Y, v.Z }).ToList() ?? new List<double>(),
                VertexNormals = normalsArray ?? new List<double>(),
                VertexUVs = uvsArray ?? new List<double>()
            };

            // 获取活动线型（考虑ByLayer和ByBlock的情况）
            var activeLineType = polyline2D.GetActiveLineType();
            
            // 收集所有顶点数据
            if (polyline2D.Vertices != null)
            {
                foreach (var vertex in polyline2D.Vertices)
            {
                polyline2DData.Vertices.Add(new VertexData
                {
                    Location = new PointData
                    {
                        X = vertex.Location.X,
                        Y = vertex.Location.Y,
                        Z = vertex.Location.Z
                    },
                    Bulge = vertex.Bulge,
                    StartWidth = vertex.StartWidth,
                    EndWidth = vertex.EndWidth
                });
            }
            }

            return polyline2DData;
        }
    }
}