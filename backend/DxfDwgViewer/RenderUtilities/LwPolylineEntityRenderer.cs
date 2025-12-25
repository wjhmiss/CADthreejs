using System;
using System.Collections.Generic;
using System.Linq;
using ACadSharp.Entities;
using ACadSharp.Extensions;

namespace DxfDwgViewer.RenderUtilities
{
    public class LwPolylineEntityRenderer
    {
        public class LwPolylineData
        {
            // 基本多段线属性
            public List<LwPolylineVertexData> Vertices { get; set; }
            public bool IsClosed { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            public double LineTypeScale { get; set; }
            
            // 基本几何属性
            public double TotalLength { get; set; }
            public int VertexCount { get; set; }
            public PointData Centroid { get; set; }
            public List<PointData> Points { get; set; }
            
            // three.js兼容性扩展属性
            public string Type { get; set; } = "LwPolyline"; // 实体类型
            public string EntityType { get; set; } = "LwPolyline"; // 实体类型（three.js兼容）
            public string Handle { get; set; } // 实体句柄（唯一标识符）
            public string LayerName { get; set; } // 图层名称
            public int LayerIndex { get; set; } // 图层索引
            public bool Visible { get; set; } = true; // 可见性
            public string CoordinateSystem { get; set; } = "World"; // 坐标系
            
            // 3D坐标和几何信息
            public List<Point3DData> Vertices3D { get; set; } // 3D顶点列表
            public Point3DData Centroid3D { get; set; } // 3D质心
            public List<Point3DData> Direction { get; set; } // 方向向量列表
            public BoundsData Bounds { get; set; } // 边界框
            public ColorData Color { get; set; } // 颜色信息
            public TransformData Transform { get; set; } // 3D变换信息
            public NormalData Normal { get; set; } // 法向量
            
            // 材质属性
            public double Opacity { get; set; } = 1.0; // 不透明度
            public bool Transparent { get; set; } = false; // 是否透明
            public string MaterialType { get; set; } = "LineBasicMaterial"; // 材质类型
            public bool DepthTest { get; set; } = true; // 深度测试
            public bool DepthWrite { get; set; } = true; // 深度写入
            
            // LwPolyline特有属性
            public double Elevation { get; set; } // 标高
            public double ConstantWidth { get; set; } // 常量宽度
            public double Thickness { get; set; } // 厚度
        }

        public class LwPolylineVertexData
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Bulge { get; set; }
            public double StartWidth { get; set; }
            public double EndWidth { get; set; }
            public Point3DData Location3D { get; set; }
        }
        
        public class PointData
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        public static LwPolylineData Render(LwPolyline lwPolyline)
        {
            var vertices3D = new List<Point3DData>();
            var direction = new List<Point3DData>();
            
            var lwPolylineData = new LwPolylineData
            {
                Vertices = lwPolyline.Vertices.Select(v => new LwPolylineVertexData
                {
                    X = v.Location.X,
                    Y = v.Location.Y,
                    Bulge = v.Bulge,
                    StartWidth = v.StartWidth,
                    EndWidth = v.EndWidth,
                    Location3D = new Point3DData(v.Location.X, v.Location.Y, lwPolyline.Elevation)
                }).ToList(),
                IsClosed = lwPolyline.IsClosed,
                ColorIndex = lwPolyline.Color.Index,
                LineTypeName = lwPolyline.GetActiveLineType()?.Name ?? string.Empty,
                LineWeight = lwPolyline.GetActiveLineWeightType().GetLineWeightValue(),
                LineTypeScale = lwPolyline.LineTypeScale,
                VertexCount = lwPolyline.Vertices.Count,
                
                // three.js兼容性扩展属性
                Type = "LwPolyline",
                EntityType = "LwPolyline",
                Handle = lwPolyline.Handle.ToString(),
                LayerName = lwPolyline.Layer?.Name ?? string.Empty,
                LayerIndex = 0,
                Visible = !lwPolyline.IsInvisible,
                CoordinateSystem = "World",
                
                // 3D坐标和几何信息
                Vertices3D = vertices3D,
                Direction = direction,
                Color = new ColorData(lwPolyline.Color.Index),
                Normal = new NormalData(lwPolyline.Normal.X, lwPolyline.Normal.Y, lwPolyline.Normal.Z),
                Transform = new TransformData(),
                
                // 材质属性
                Opacity = 1.0,
                Transparent = false,
                MaterialType = "LineBasicMaterial",
                DepthTest = true,
                DepthWrite = true,
                
                // LwPolyline特有属性
                Elevation = lwPolyline.Elevation,
                ConstantWidth = lwPolyline.ConstantWidth,
                Thickness = lwPolyline.Thickness
            };

            // 计算多段线的总长度和点列表
            double totalLength = 0;
            var points = new List<PointData>();
            
            if (lwPolyline.Vertices.Count > 0)
            {
                // 添加所有顶点作为点列表
                foreach (var vertex in lwPolyline.Vertices)
                {
                    points.Add(new PointData { X = vertex.Location.X, Y = vertex.Location.Y });
                    vertices3D.Add(new Point3DData(vertex.Location.X, vertex.Location.Y, lwPolyline.Elevation));
                }
                
                // 计算总长度（仅考虑直线段，忽略Bulge）
                for (int i = 0; i < lwPolyline.Vertices.Count - 1; i++)
                {
                    var current = lwPolyline.Vertices[i].Location;
                    var next = lwPolyline.Vertices[i + 1].Location;
                    double dx = next.X - current.X;
                    double dy = next.Y - current.Y;
                    double length = Math.Sqrt(dx * dx + dy * dy);
                    totalLength += length;
                    
                    // 计算方向向量（归一化）
                    if (length > 0)
                    {
                        direction.Add(new Point3DData(dx / length, dy / length, 0.0));
                    }
                }
                
                // 如果是闭合多段线，加上最后一点到第一点的长度
                if (lwPolyline.IsClosed && lwPolyline.Vertices.Count > 1)
                {
                    var first = lwPolyline.Vertices[0].Location;
                    var last = lwPolyline.Vertices[lwPolyline.Vertices.Count - 1].Location;
                    double dx = first.X - last.X;
                    double dy = first.Y - last.Y;
                    double length = Math.Sqrt(dx * dx + dy * dy);
                    totalLength += length;
                    
                    // 计算方向向量（归一化）
                    if (length > 0)
                    {
                        direction.Add(new Point3DData(dx / length, dy / length, 0.0));
                    }
                }
                
                // 计算质心
                if (lwPolyline.Vertices.Count > 0)
                {
                    double sumX = 0, sumY = 0;
                    foreach (var vertex in lwPolyline.Vertices)
                    {
                        sumX += vertex.Location.X;
                        sumY += vertex.Location.Y;
                    }
                    double centroidX = sumX / lwPolyline.Vertices.Count;
                    double centroidY = sumY / lwPolyline.Vertices.Count;
                    
                    lwPolylineData.Centroid = new PointData 
                    { 
                        X = centroidX, 
                        Y = centroidY 
                    };
                    
                    lwPolylineData.Centroid3D = new Point3DData(centroidX, centroidY, lwPolyline.Elevation);
                }
                
                // 计算边界框
                if (vertices3D.Count > 0)
                {
                    double minX = vertices3D.Min(v => v.X);
                    double minY = vertices3D.Min(v => v.Y);
                    double minZ = vertices3D.Min(v => v.Z);
                    double maxX = vertices3D.Max(v => v.X);
                    double maxY = vertices3D.Max(v => v.Y);
                    double maxZ = vertices3D.Max(v => v.Z);
                    
                    lwPolylineData.Bounds = new BoundsData(
                        new Point3DData(minX, minY, minZ),
                        new Point3DData(maxX, maxY, maxZ)
                    );
                }
            }
            
            lwPolylineData.TotalLength = totalLength;
            lwPolylineData.Points = points;

            return lwPolylineData;
        }
    }
}