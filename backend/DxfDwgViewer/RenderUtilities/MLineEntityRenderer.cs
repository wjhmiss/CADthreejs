using ACadSharp.Entities;
using ACadSharp.Objects;
using ACadSharp.Tables;
using CSMath;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DxfDwgViewer.RenderUtilities
{
    public class MLineEntityRenderer
    {
        public class MLineData
        {
            public List<ElementData> Elements { get; set; }
            public bool IsClosed { get; set; }
            public double ScaleFactor { get; set; }
            public short ColorIndex { get; set; }
            public MLineStyleFlags StyleFlags { get; set; }
            public short FillColorIndex { get; set; }
            public bool FillOn { get; set; }
            public List<VertexData> Vertices { get; set; }
            
            // 基本几何属性
            public int ElementCount { get; set; }
            public int VertexCount { get; set; }
            public BoundsData Bounds { get; set; }
            public PointData Centroid { get; set; }
            
            // three.js兼容性扩展属性
            public string Type { get; set; } = "MLine"; // 实体类型
            public string EntityType { get; set; } = "MLine"; // 实体类型（three.js兼容）
            public string Handle { get; set; } // 实体句柄（唯一标识符）
            public string LayerName { get; set; } // 图层名称
            public int LayerIndex { get; set; } // 图层索引
            public bool Visible { get; set; } = true; // 可见性
            public string CoordinateSystem { get; set; } = "World"; // 坐标系
            
            // 3D坐标和几何信息
            public List<Point3DData> Vertices3D { get; set; } // 3D顶点列表
            public Point3DData Centroid3D { get; set; } // 3D质心
            public List<Point3DData> Normals { get; set; } // 顶点法向量列表
            public BoundsData3D Bounds3D { get; set; } // 3D边界框
            public ColorData Color { get; set; } // 颜色信息
            public TransformData Transform { get; set; } // 3D变换信息
            public NormalData Normal { get; set; } // 默认法向量
            
            // 材质属性
            public double Opacity { get; set; } = 1.0; // 不透明度
            public bool Transparent { get; set; } = false; // 是否透明
            public string MaterialType { get; set; } = "LineBasicMaterial"; // 材质类型
            public bool DepthTest { get; set; } = true; // 深度测试
            public bool DepthWrite { get; set; } = true; // 深度写入
            
            // MLine特有属性
            public MLineFlags Flags { get; set; } // MLine标志位
            public MLineJustification Justification { get; set; } // 对齐方式
            public Point3DData StartPoint { get; set; } // 起始点
            public Point3DData EndPoint { get; set; } // 结束点
            public double TotalLength { get; set; } // 总长度
            public string StyleName { get; set; } // 样式名称
            public double StartAngle { get; set; } // 起始角度
            public double EndAngle { get; set; } // 结束角度
            public bool HasStartCaps { get; set; } = true; // 是否有起始端点
            public bool HasEndCaps { get; set; } = true; // 是否有结束端点
            public bool DisplayJoints { get; set; } = false; // 是否显示关节
            public List<int[]> LineIndices { get; set; } // 线索引列表（three.js兼容格式）
            public List<double> Offsets { get; set; } // 元素偏移量列表
            public List<short> ElementColorIndices { get; set; } // 元素颜色索引列表
        }

        public class ElementData
        {
            public double Offset { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public List<PointData> Points { get; set; }
            // 添加three.js友好的属性
            public int PointCount { get; set; }
            public double TotalLength { get; set; }
        }

        public class VertexData
        {
            public PointData Position { get; set; }
            public PointData Direction { get; set; }
            public PointData Miter { get; set; }
            public List<SegmentData> Segments { get; set; }
        }

        public class SegmentData
        {
            public List<double> Parameters { get; set; }
        }

        public static MLineData Render(MLine mline)
        {
            // 计算边界框和质心
            BoundsData? bounds = null;
            PointData? centroid = null;
            BoundsData3D? bounds3D = null;
            Point3DData? centroid3D = null;
            List<Point3DData> vertices3D = new List<Point3DData>();
            List<Point3DData> normals = new List<Point3DData>();
            List<int[]> lineIndices = new List<int[]>();
            List<double> offsets = new List<double>();
            List<short> elementColorIndices = new List<short>();
            double totalLength = 0;
            Point3DData? startPoint = null;
            Point3DData? endPoint = null;
            
            if (mline.Vertices.Count > 0)
            {
                // 计算所有顶点的边界框
                double minX = double.MaxValue, maxX = double.MinValue;
                double minY = double.MaxValue, maxY = double.MinValue;
                double minZ = double.MaxValue, maxZ = double.MinValue;
                double sumX = 0, sumY = 0, sumZ = 0;
                int pointCount = 0;
                
                startPoint = new Point3DData(mline.StartPoint.X, mline.StartPoint.Y, mline.StartPoint.Z);
                
                foreach (var vertex in mline.Vertices)
                {
                    // 更新边界框
                    if (vertex.Position.X < minX) minX = vertex.Position.X;
                    if (vertex.Position.X > maxX) maxX = vertex.Position.X;
                    if (vertex.Position.Y < minY) minY = vertex.Position.Y;
                    if (vertex.Position.Y > maxY) maxY = vertex.Position.Y;
                    if (vertex.Position.Z < minZ) minZ = vertex.Position.Z;
                    if (vertex.Position.Z > maxZ) maxZ = vertex.Position.Z;
                    
                    // 累加用于计算质心
                    sumX += vertex.Position.X;
                    sumY += vertex.Position.Y;
                    sumZ += vertex.Position.Z;
                    pointCount++;
                    
                    // 添加到3D顶点列表
                    vertices3D.Add(new Point3DData(vertex.Position.X, vertex.Position.Y, vertex.Position.Z));
                    
                    // 添加默认法向量（指向Z轴正方向）
                    normals.Add(new Point3DData(0, 0, 1));
                }
                
                // 设置结束点
                endPoint = new Point3DData(mline.Vertices.Last().Position.X, mline.Vertices.Last().Position.Y, mline.Vertices.Last().Position.Z);
                
                // 计算总长度
                for (int i = 0; i < mline.Vertices.Count - 1; i++)
                {
                    double dx = mline.Vertices[i + 1].Position.X - mline.Vertices[i].Position.X;
                    double dy = mline.Vertices[i + 1].Position.Y - mline.Vertices[i].Position.Y;
                    double dz = mline.Vertices[i + 1].Position.Z - mline.Vertices[i].Position.Z;
                    totalLength += Math.Sqrt(dx * dx + dy * dy + dz * dz);
                }
                
                bounds = new BoundsData(
                    new Point3DData(minX, minY, minZ),
                    new Point3DData(maxX, maxY, maxZ)
                );
                
                bounds3D = new BoundsData3D(
                    new Point3DData(minX, minY, minZ),
                    new Point3DData(maxX, maxY, maxZ)
                );
                
                centroid = new PointData(
                    sumX / pointCount,
                    sumY / pointCount
                );
                
                centroid3D = new Point3DData(
                    sumX / pointCount,
                    sumY / pointCount,
                    sumZ / pointCount
                );
            }

            // 收集元素偏移量和颜色索引
            foreach (var element in mline.Style.Elements)
            {
                offsets.Add(element.Offset);
                elementColorIndices.Add(element.Color.Index);
            }

            // 计算线索引
            int vertexIndex = 0;
            foreach (var vertex in mline.Vertices)
            {
                if (vertexIndex < mline.Vertices.Count - 1)
                {
                    lineIndices.Add(new int[] { vertexIndex, vertexIndex + 1 });
                }
                vertexIndex++;
            }

            var mlineData = new MLineData
            {
                Elements = new List<ElementData>(),
                IsClosed = mline.Flags.HasFlag(MLineFlags.Closed),
                ScaleFactor = mline.ScaleFactor,
                ColorIndex = mline.Color.Index,
                StyleFlags = mline.Style.Flags,
                FillColorIndex = mline.Style.FillColor.Index,
                FillOn = mline.Style.Flags.HasFlag(MLineStyleFlags.FillOn),
                Vertices = new List<VertexData>(),
                ElementCount = mline.Style.Elements.Count(),
                VertexCount = mline.Vertices.Count,
                Bounds = bounds!,
                Centroid = centroid!,
                
                // three.js兼容性扩展属性
                Type = "MLine",
                EntityType = "MLine",
                Handle = mline.Handle.ToString(),
                LayerName = mline.Layer?.Name ?? string.Empty,
                LayerIndex = 0,
                Visible = !mline.IsInvisible,
                CoordinateSystem = "World",
                
                // 3D坐标和几何信息
                Vertices3D = vertices3D,
                Centroid3D = centroid3D!,
                Normals = normals,
                Bounds3D = bounds3D!,
                Color = new ColorData(mline.Color.Index),
                Normal = new NormalData(mline.Normal.X, mline.Normal.Y, mline.Normal.Z),
                Transform = new TransformData(),
                
                // 材质属性
                Opacity = 1.0,
                Transparent = false,
                MaterialType = "LineBasicMaterial",
                DepthTest = true,
                DepthWrite = true,
                
                // MLine特有属性
                Flags = mline.Flags,
                Justification = mline.Justification,
                StartPoint = startPoint!,
                EndPoint = endPoint!,
                TotalLength = totalLength,
                StyleName = mline.Style.Name,
                StartAngle = mline.Style.StartAngle,
                EndAngle = mline.Style.EndAngle,
                HasStartCaps = !mline.Flags.HasFlag(MLineFlags.NoStartCaps),
                HasEndCaps = !mline.Flags.HasFlag(MLineFlags.NoEndCaps),
                DisplayJoints = mline.Style.Flags.HasFlag(MLineStyleFlags.DisplayJoints),
                LineIndices = lineIndices,
                Offsets = offsets,
                ElementColorIndices = elementColorIndices
            };

            if (mline == null || mline.Vertices.Count <= 1)
                return mlineData;

            try
            {
                // 根据MLine的样式处理所有线段
                var elements = mline.Style.Elements.ToList();
            
                // 检查是否为闭合的多线
                bool isClosed = mline.Flags.HasFlag(MLineFlags.Closed);
            
                // 对每个元素（线条）创建一个单独的路径
                for (int i = 0; i < elements.Count; i++)
                {
                    var element = elements[i];
                    var elementData = new ElementData
                    {
                        Offset = element.Offset,
                        ColorIndex = element.Color.Index,
                        LineTypeName = element.LineType?.Name ?? "",
                        Points = new List<PointData>()
                    };
                    
                    // 检查是否有断开的线段
                    bool hasBreaks = CheckForSegmentBreaks(mline, i);
                    
                    List<PointData> renderedPoints;
                    if (hasBreaks)
                    {
                        // 如果有断开的线段，分段处理
                        renderedPoints = RenderBrokenSegments(mline, element, i);
                        elementData.Points.AddRange(renderedPoints);
                    }
                    else
                    {
                        // 如果没有断开的线段，正常处理
                        renderedPoints = RenderContinuousSegment(mline, element, isClosed);
                        elementData.Points.AddRange(renderedPoints);
                    }
                    
                    // 计算元素的总长度
                    double elementTotalLength = 0;
                    for (int j = 0; j < renderedPoints.Count - 1; j++)
                    {
                        double dx = renderedPoints[j + 1].X - renderedPoints[j].X;
                        double dy = renderedPoints[j + 1].Y - renderedPoints[j].Y;
                        elementTotalLength += Math.Sqrt(dx * dx + dy * dy);
                    }
                    
                    elementData.PointCount = renderedPoints.Count;
                    elementData.TotalLength = elementTotalLength;

                    mlineData.Elements.Add(elementData);
                }
                
                // 收集顶点数据
                foreach (var vertex in mline.Vertices)
                {
                    var vertexData = new VertexData
                    {
                        Position = new PointData(vertex.Position.X, vertex.Position.Y),
                        Direction = new PointData(vertex.Direction.X, vertex.Direction.Y),
                        Miter = new PointData(vertex.Miter.X, vertex.Miter.Y),
                        Segments = vertex.Segments.Select(s => new SegmentData 
                        { 
                            Parameters = s.Parameters.ToList() 
                        }).ToList()
                    };
                    mlineData.Vertices.Add(vertexData);
                }
            }
            catch (Exception ex)
            {
                // 输出异常信息但不中断程序
                Console.WriteLine($"Error rendering MLine: {ex.Message}");
            }

            return mlineData;
        }
        
        /// <summary>
        /// 检查指定元素索引的线段是否有断开的部分
        /// </summary>
        private static bool CheckForSegmentBreaks(MLine mline, int elementIndex)
        {
            // 检查每个顶点的Segment参数
            foreach (var vertex in mline.Vertices)
            {
                if (vertex.Segments.Count > elementIndex)
                {
                    var segment = vertex.Segments[elementIndex];
                    // 如果Segment参数不为空，可能表示有断开
                    if (segment.Parameters.Count > 0)
                    {
                        // 检查参数是否表示断开（通常负值或特殊值表示断开）
                        foreach (var param in segment.Parameters)
                        {
                            // 如果参数小于0，表示有断开
                            if (param < 0)
                                return true;
                        }
                    }
                }
            }
            return false;
        }
        
        /// <summary>
        /// 处理连续的线段（包括闭合的线段）
        /// </summary>
        private static List<PointData> RenderContinuousSegment(MLine mline, MLineStyle.Element element, bool isClosed)
        {
            // 创建用于处理此元素的点列表
            List<PointData> points = new List<PointData>();
            
            // 计算每个顶点处该元素的位置
            for (int j = 0; j < mline.Vertices.Count; j++)
            {
                var vertex = mline.Vertices[j];
                
                // 计算垂直于线段方向的偏移
                // 使用Direction向量的垂直向量作为偏移方向
                double offsetX, offsetY;
                
                // 如果Direction为零向量，使用Miter作为替代
                if (vertex.Direction.GetLength() < 0.0001)
                {
                    offsetX = vertex.Miter.X * element.Offset * mline.ScaleFactor;
                    offsetY = vertex.Miter.Y * element.Offset * mline.ScaleFactor;
                }
                else
                {
                    // 计算垂直于Direction的向量（旋转90度）
                    offsetX = -vertex.Direction.Y * element.Offset * mline.ScaleFactor;
                    offsetY = vertex.Direction.X * element.Offset * mline.ScaleFactor;
                }
                
                // 应用坐标变换 - 修正Y坐标处理方式
                points.Add(new PointData(
                    vertex.Position.X + offsetX,
                    vertex.Position.Y - offsetY
                ));
            }

            return points;
        }
        
        /// <summary>
        /// 处理断开的线段
        /// </summary>
        private static List<PointData> RenderBrokenSegments(MLine mline, MLineStyle.Element element, int elementIndex)
        {
            // 创建用于处理此元素的点列表
            List<PointData> allPoints = new List<PointData>();
            
            // 计算每个顶点处该元素的位置
            for (int j = 0; j < mline.Vertices.Count; j++)
            {
                var vertex = mline.Vertices[j];
                
                // 计算垂直于线段方向的偏移
                // 使用Direction向量的垂直向量作为偏移方向
                double offsetX, offsetY;
                
                // 如果Direction为零向量，使用Miter作为替代
                if (vertex.Direction.GetLength() < 0.0001)
                {
                    offsetX = vertex.Miter.X * element.Offset * mline.ScaleFactor;
                    offsetY = vertex.Miter.Y * element.Offset * mline.ScaleFactor;
                }
                else
                {
                    // 计算垂直于Direction的向量（旋转90度）
                    offsetX = -vertex.Direction.Y * element.Offset * mline.ScaleFactor;
                    offsetY = vertex.Direction.X * element.Offset * mline.ScaleFactor;
                }
                
                // 应用坐标变换 - 修正Y坐标处理方式
                allPoints.Add(new PointData(
                    vertex.Position.X + offsetX,
                    vertex.Position.Y - offsetY
                ));
            }
            
            // 根据断开参数分段处理
            List<PointData> segmentPoints = new List<PointData>();
            List<PointData> processedPoints = new List<PointData>(); // 存储所有处理的点
            
            for (int j = 0; j < mline.Vertices.Count; j++)
            {
                var vertex = mline.Vertices[j];
                
                // 检查当前顶点的断开参数
                bool hasBreakStart = false;
                if (vertex.Segments.Count > elementIndex && elementIndex >= 0)
                {
                    var segment = vertex.Segments[elementIndex];
                    // 根据DXF规范，第一个参数为负值时表示断开的起点
                    if (segment.Parameters.Count >= 1 && segment.Parameters[0] < 0)
                    {
                        hasBreakStart = true;
                    }
                }
                
                // 如果是断开起点且当前段有足够点，则处理当前段
                if (hasBreakStart && segmentPoints.Count > 1)
                {
                    processedPoints.AddRange(segmentPoints);
                    // 清空当前段
                    segmentPoints.Clear();
                }
                
                // 添加当前点到段中
                segmentPoints.Add(allPoints[j]);
                
                // 检查是否有断开终点参数
                bool hasBreakEnd = false;
                if (vertex.Segments.Count > elementIndex && elementIndex >= 0)
                {
                    var segment = vertex.Segments[elementIndex];
                    // 根据DXF规范，第二个参数为正值时表示断开的终点
                    if (segment.Parameters.Count >= 2 && segment.Parameters[1] > 0)
                    {
                        hasBreakEnd = true;
                    }
                }
                
                // 如果是断开终点且当前段有足够点，则处理当前段
                if (hasBreakEnd && segmentPoints.Count > 1)
                {
                    processedPoints.AddRange(segmentPoints);
                    // 清空当前段
                    segmentPoints.Clear();
                }
            }
            
            // 处理最后一段（如果有点）
            if (segmentPoints.Count > 1)
            {
                processedPoints.AddRange(segmentPoints);
            }

            return processedPoints;
        }
    }
}