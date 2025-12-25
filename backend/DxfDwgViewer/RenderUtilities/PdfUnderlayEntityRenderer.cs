using System;
using System.Linq;
using ACadSharp.Entities;
using System.Collections.Generic;
using ACadSharp.Extensions;
using CSMath;

namespace DxfDwgViewer.RenderUtilities
{
    public class PdfUnderlayEntityRenderer
    {
        public class PdfUnderlayData
    {
        public List<PointData> BoundaryPoints { get; set; }
        public PointData InsertPoint { get; set; }
        public double XScale { get; set; }
        public double YScale { get; set; }
        public double ZScale { get; set; }
        public double Rotation { get; set; }
        public short ColorIndex { get; set; }
        public string LineTypeName { get; set; }
        public double LineWeight { get; set; }
        public byte Contrast { get; set; }
        public byte Fade { get; set; }
        public UnderlayDisplayFlags Flags { get; set; }
        public string DefinitionFileName { get; set; }
        
        // 基本几何属性
        public BoundsData Bounds { get; set; }
        public PointData Centroid { get; set; }
        public int BoundaryPointCount { get; set; }
        public double Area { get; set; }
        
        // three.js兼容性属性
        public List<Point3DData> BoundaryPoints3D { get; set; }
        public Point3DData InsertPoint3D { get; set; }
        public Point3DData Centroid3D { get; set; }
        public ColorData Color { get; set; }
        public BoundsData3D Bounds3D { get; set; }
        public NormalData Normal { get; set; }
        public TransformData Transform { get; set; }
        public string EntityType { get; set; }
        public bool Visible { get; set; }
        public int LayerIndex { get; set; }
        public string LayerName { get; set; }
        
        // PdfUnderlay特定属性
        public string PageNumber { get; set; }
        public string FilePath { get; set; }
        public string ClippingBoundaryType { get; set; }
        
        // three.js增强属性
        public string Type { get; set; }
        public string Handle { get; set; }
        public string CoordinateSystem { get; set; }
        public double Opacity { get; set; }
        public bool Transparent { get; set; }
        public bool DepthTest { get; set; }
        public UnderlayMaterialData Material { get; set; }
        public UnderlayGeometryData Geometry { get; set; }
    }
    
    public class UnderlayMaterialData
    {
        public ColorData Color { get; set; }
        public double Opacity { get; set; }
        public bool Transparent { get; set; }
        public byte Contrast { get; set; }
        public byte Fade { get; set; }
        public string Type { get; set; }
        public bool DepthTest { get; set; }
        public bool Side { get; set; }
    }
    
    public class UnderlayGeometryData
    {
        public List<Point3DData> Vertices { get; set; }
        public List<int> Indices { get; set; }
        public Point3DData Position { get; set; }
        public Point3DData Rotation { get; set; }
        public Point3DData Scale { get; set; }
        public NormalData Normal { get; set; }
        public BoundsData3D BoundingBox { get; set; }
        public string Type { get; set; }
    }

        public static PdfUnderlayData Render(PdfUnderlay pdfUnderlay)
        {
            double opacity = 1.0 - (pdfUnderlay.Fade / 100.0);
            
            var pdfUnderlayData = new PdfUnderlayData
            {
                BoundaryPoints = new List<PointData>(),
                InsertPoint = new PointData(pdfUnderlay.InsertPoint.X, pdfUnderlay.InsertPoint.Y),
                XScale = pdfUnderlay.XScale,
                YScale = pdfUnderlay.YScale,
                ZScale = pdfUnderlay.ZScale,
                Rotation = pdfUnderlay.Rotation,
                ColorIndex = pdfUnderlay.Color.Index,
                LineTypeName = pdfUnderlay.GetActiveLineType()?.Name ?? "",
                LineWeight = pdfUnderlay.GetActiveLineWeightType().GetLineWeightValue(),
                Contrast = pdfUnderlay.Contrast,
                Fade = pdfUnderlay.Fade,
                Flags = pdfUnderlay.Flags,
                DefinitionFileName = pdfUnderlay.Definition?.File ?? "",
                
                // 初始化three.js兼容性属性
                BoundaryPoints3D = new List<Point3DData>(),
                InsertPoint3D = new Point3DData(pdfUnderlay.InsertPoint.X, pdfUnderlay.InsertPoint.Y, pdfUnderlay.InsertPoint.Z),
                Color = new ColorData(pdfUnderlay.Color.Index),
                Normal = new NormalData(pdfUnderlay.Normal.X, pdfUnderlay.Normal.Y, pdfUnderlay.Normal.Z),
                Transform = new TransformData(pdfUnderlay.XScale, pdfUnderlay.YScale, pdfUnderlay.ZScale, 
                                           pdfUnderlay.Rotation, pdfUnderlay.InsertPoint.X, pdfUnderlay.InsertPoint.Y, pdfUnderlay.InsertPoint.Z),
                EntityType = "PdfUnderlay",
                Visible = !pdfUnderlay.IsInvisible,
                LayerIndex = 0,
                LayerName = pdfUnderlay.Layer?.Name ?? "",
                
                // PdfUnderlay特定属性
                PageNumber = pdfUnderlay.Definition?.Page ?? "1",
                FilePath = pdfUnderlay.Definition?.File ?? "",
                ClippingBoundaryType = pdfUnderlay.ClipBoundaryVertices?.Count == 2 ? "Rectangle" : 
                                      pdfUnderlay.ClipBoundaryVertices?.Count > 2 ? "Polygon" : "None",
                
                // three.js增强属性
                Type = "Underlay",
                Handle = pdfUnderlay.Handle.ToString(),
                CoordinateSystem = "WCS",
                Opacity = opacity,
                Transparent = pdfUnderlay.Fade > 0,
                DepthTest = true,
                
                // 初始化材质和几何数据
                Material = new UnderlayMaterialData
                {
                    Color = new ColorData(pdfUnderlay.Color.Index),
                    Opacity = opacity,
                    Transparent = pdfUnderlay.Fade > 0,
                    Contrast = pdfUnderlay.Contrast,
                    Fade = pdfUnderlay.Fade,
                    Type = "UnderlayMaterial",
                    DepthTest = true,
                    Side = true
                },
                Geometry = new UnderlayGeometryData
                {
                    Vertices = new List<Point3DData>(),
                    Indices = new List<int>(),
                    Position = new Point3DData(pdfUnderlay.InsertPoint.X, pdfUnderlay.InsertPoint.Y, pdfUnderlay.InsertPoint.Z),
                    Rotation = new Point3DData(0, 0, pdfUnderlay.Rotation),
                    Scale = new Point3DData(pdfUnderlay.XScale, pdfUnderlay.YScale, pdfUnderlay.ZScale),
                    Normal = new NormalData(pdfUnderlay.Normal.X, pdfUnderlay.Normal.Y, pdfUnderlay.Normal.Z),
                    Type = "UnderlayGeometry"
                }
            };

            try
            {
                // 检查是否有足够的边界顶点
                if (pdfUnderlay.ClipBoundaryVertices == null || pdfUnderlay.ClipBoundaryVertices.Count < 2)
                {
                    return pdfUnderlayData;
                }

                // 将边界顶点转换为相对于插入点的坐标
                List<XYZ> boundaryPoints;

                if (pdfUnderlay.ClipBoundaryVertices.Count == 2)
                {
                    // 矩形边界（两个对角点），转换为4个顶点
                    var p1 = pdfUnderlay.ClipBoundaryVertices[0];
                    var p2 = pdfUnderlay.ClipBoundaryVertices[1];
                    double x1 = p1.X * pdfUnderlay.XScale + pdfUnderlay.InsertPoint.X;
                    double y1 = p1.Y * pdfUnderlay.YScale + pdfUnderlay.InsertPoint.Y;
                    double x2 = p2.X * pdfUnderlay.XScale + pdfUnderlay.InsertPoint.X;
                    double y2 = p2.Y * pdfUnderlay.YScale + pdfUnderlay.InsertPoint.Y;

                    double minX = Math.Min(x1, x2);
                    double maxX = Math.Max(x1, x2);
                    double minY = Math.Min(y1, y2);
                    double maxY = Math.Max(y1, y2);

                    boundaryPoints = new List<XYZ>
                    {
                        new XYZ(minX, minY, pdfUnderlay.InsertPoint.Z),
                        new XYZ(maxX, minY, pdfUnderlay.InsertPoint.Z),
                        new XYZ(maxX, maxY, pdfUnderlay.InsertPoint.Z),
                        new XYZ(minX, maxY, pdfUnderlay.InsertPoint.Z)
                    };
                }
                else
                {
                    // 多边形边界
                    boundaryPoints = pdfUnderlay.ClipBoundaryVertices
                        .Select(v => new XYZ(
                            v.X * pdfUnderlay.XScale + pdfUnderlay.InsertPoint.X,
                            v.Y * pdfUnderlay.YScale + pdfUnderlay.InsertPoint.Y,
                            pdfUnderlay.InsertPoint.Z))
                        .ToList();
                }

                // 将边界点添加到数据对象中
                foreach (var point in boundaryPoints)
                {
                    pdfUnderlayData.BoundaryPoints.Add(new PointData(point.X, point.Y));
                    pdfUnderlayData.BoundaryPoints3D.Add(new Point3DData(point.X, point.Y, point.Z));
                    pdfUnderlayData.Geometry.Vertices.Add(new Point3DData(point.X, point.Y, point.Z));
                }

                // 创建索引数组用于三角形网格
                if (boundaryPoints.Count >= 3)
                {
                    for (int i = 1; i < boundaryPoints.Count - 1; i++)
                    {
                        pdfUnderlayData.Geometry.Indices.Add(0);
                        pdfUnderlayData.Geometry.Indices.Add(i);
                        pdfUnderlayData.Geometry.Indices.Add(i + 1);
                    }
                }

                // 计算新增属性
                pdfUnderlayData.BoundaryPointCount = boundaryPoints.Count;

                // 计算边界框和3D边界框
                if (boundaryPoints.Count > 0)
                {
                    double minX = boundaryPoints.Min(p => p.X);
                    double maxX = boundaryPoints.Max(p => p.X);
                    double minY = boundaryPoints.Min(p => p.Y);
                    double maxY = boundaryPoints.Max(p => p.Y);
                    double z = pdfUnderlay.InsertPoint.Z;

                    // 创建2D边界框（使用旧的格式以保持兼容性）
                    pdfUnderlayData.Bounds = new BoundsData(
                        new Point3DData(minX, minY, z),
                        new Point3DData(maxX, maxY, z)
                    );

                    // 创建3D边界框
                    pdfUnderlayData.Bounds3D = new BoundsData3D(
                        new Point3DData(minX, minY, z),
                        new Point3DData(maxX, maxY, z)
                    );

                    // 设置几何体的边界框
                    pdfUnderlayData.Geometry.BoundingBox = new BoundsData3D(
                        new Point3DData(minX, minY, z),
                        new Point3DData(maxX, maxY, z)
                    );

                    // 计算质心
                    double centerX = boundaryPoints.Average(p => p.X);
                    double centerY = boundaryPoints.Average(p => p.Y);
                    pdfUnderlayData.Centroid = new PointData(centerX, centerY);
                    pdfUnderlayData.Centroid3D = new Point3DData(centerX, centerY, z);

                    // 计算面积（使用鞋带公式）
                    if (boundaryPoints.Count >= 3)
                    {
                        double area = 0;
                        for (int i = 0; i < boundaryPoints.Count; i++)
                        {
                            int j = (i + 1) % boundaryPoints.Count;
                            area += boundaryPoints[i].X * boundaryPoints[j].Y;
                            area -= boundaryPoints[j].X * boundaryPoints[i].Y;
                        }
                        pdfUnderlayData.Area = Math.Abs(area) / 2.0;
                    }
                    else if (boundaryPoints.Count == 2)
                    {
                        // 矩形面积
                        pdfUnderlayData.Area = (maxX - minX) * (maxY - minY);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rendering PDF underlay: {ex.Message}");
            }

            return pdfUnderlayData;
        }
    }
}