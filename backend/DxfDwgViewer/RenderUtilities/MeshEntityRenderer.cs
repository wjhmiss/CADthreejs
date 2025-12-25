using System;
using ACadSharp.Entities;
using CSMath;
using System.Collections.Generic;
using System.Linq;
using ACadSharp.Extensions;
using DxfDwgViewer.RenderUtilities;

namespace DxfDwgViewer.RenderUtilities
{
    public class MeshEntityRenderer
    {
        public class FaceData
        {
            public List<int> VertexIndices { get; set; }
        }

        public class EdgeData
        {
            public int Start { get; set; }
            public int End { get; set; }
        }

        public class MeshData
        {
            // 基本网格属性
            public List<VertexData> Vertices { get; set; }
            public List<FaceData> Faces { get; set; }
            public List<EdgeData> Edges { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            
            // 基本几何属性
            public int VertexCount { get; set; }
            public int FaceCount { get; set; }
            public int EdgeCount { get; set; }
            public BoundsData Bounds { get; set; }
            public PointData Centroid { get; set; }
            
            // three.js兼容性扩展属性
            public string Type { get; set; } = "Mesh"; // 实体类型
            public string EntityType { get; set; } = "Mesh"; // 实体类型（three.js兼容）
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
            public string MaterialType { get; set; } = "MeshStandardMaterial"; // 材质类型
            public bool DepthTest { get; set; } = true; // 深度测试
            public bool DepthWrite { get; set; } = true; // 深度写入
            public bool Side { get; set; } = true; // 双面渲染（true=双面，false=单面）
            
            // Mesh特有属性
            public int SubdivisionLevel { get; set; } // 细分级别
            public short Version { get; set; } // 版本号
            public bool BlendCrease { get; set; } // 混合折痕标志
            public List<int[]> FaceIndices { get; set; } // 面索引列表（three.js兼容格式）
            public List<int[]> EdgeIndices { get; set; } // 边索引列表（three.js兼容格式）
        }

        public class VertexData
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
        }

        public static MeshData Render(Mesh mesh)
        {
            // 计算边界框和3D顶点数据
            BoundsData? bounds = null;
            BoundsData3D? bounds3D = null;
            PointData? centroid = null;
            Point3DData? centroid3D = null;
            var vertices3D = new List<Point3DData>();
            var normals = new List<Point3DData>();
            var faceIndices = new List<int[]>();
            var edgeIndices = new List<int[]>();
            
            if (mesh.Vertices.Count > 0)
            {
                double minX = mesh.Vertices[0].X, maxX = mesh.Vertices[0].X;
                double minY = mesh.Vertices[0].Y, maxY = mesh.Vertices[0].Y;
                double minZ = mesh.Vertices[0].Z, maxZ = mesh.Vertices[0].Z;
                double sumX = 0, sumY = 0, sumZ = 0;
                
                foreach (var vertex in mesh.Vertices)
                {
                    // 添加到3D顶点列表
                    vertices3D.Add(new Point3DData(vertex.X, vertex.Y, vertex.Z));
                    
                    // 添加默认法向量（指向Z轴正方向）
                    normals.Add(new Point3DData(0, 0, 1));
                    
                    // 更新边界框
                    if (vertex.X < minX) minX = vertex.X;
                    if (vertex.X > maxX) maxX = vertex.X;
                    if (vertex.Y < minY) minY = vertex.Y;
                    if (vertex.Y > maxY) maxY = vertex.Y;
                    if (vertex.Z < minZ) minZ = vertex.Z;
                    if (vertex.Z > maxZ) maxZ = vertex.Z;
                    
                    // 累加用于计算质心
                    sumX += vertex.X;
                    sumY += vertex.Y;
                    sumZ += vertex.Z;
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
                    sumX / mesh.Vertices.Count,
                    sumY / mesh.Vertices.Count
                );
                
                centroid3D = new Point3DData(
                    sumX / mesh.Vertices.Count,
                    sumY / mesh.Vertices.Count,
                    sumZ / mesh.Vertices.Count
                );
            }

            // 收集面索引数据
            foreach (var face in mesh.Faces)
            {
                if (face.Length >= 3)
                {
                    faceIndices.Add(face);
                }
            }

            // 收集边索引数据
            foreach (var edge in mesh.Edges)
            {
                edgeIndices.Add(new int[] { edge.Start, edge.End });
            }

            var meshData = new MeshData
            {
                Vertices = mesh.Vertices.Select(v => new VertexData { X = v.X, Y = v.Y, Z = v.Z }).ToList(),
                Faces = new List<FaceData>(),
                Edges = mesh.Edges.Select(e => new EdgeData { Start = e.Start, End = e.End }).ToList(),
                ColorIndex = mesh.Color.Index,
                LineTypeName = mesh.GetActiveLineType()?.Name ?? "",
                LineWeight = mesh.GetActiveLineWeightType().GetLineWeightValue(),
                VertexCount = mesh.Vertices.Count,
                FaceCount = mesh.Faces.Count(),
                EdgeCount = mesh.Edges.Count(),
                Bounds = bounds!,
                Centroid = centroid!,
                
                // three.js兼容性扩展属性
                Type = "Mesh",
                EntityType = "Mesh",
                Handle = mesh.Handle.ToString(),
                LayerName = mesh.Layer?.Name ?? string.Empty,
                LayerIndex = 0,
                Visible = !mesh.IsInvisible,
                CoordinateSystem = "World",
                
                // 3D坐标和几何信息
                Vertices3D = vertices3D,
                Centroid3D = centroid3D!,
                Normals = normals,
                Bounds3D = bounds3D!,
                Color = new ColorData(mesh.Color.Index),
                Normal = new NormalData(0, 0, 1),
                Transform = new TransformData(),
                
                // 材质属性
                Opacity = 1.0,
                Transparent = false,
                MaterialType = "MeshStandardMaterial",
                DepthTest = true,
                DepthWrite = true,
                Side = true,
                
                // Mesh特有属性
                SubdivisionLevel = mesh.SubdivisionLevel,
                Version = mesh.Version,
                BlendCrease = mesh.BlendCrease,
                FaceIndices = faceIndices,
                EdgeIndices = edgeIndices
            };

            // 收集面数据
            foreach (var face in mesh.Faces)
            {
                if (face.Length >= 3)
                {
                    meshData.Faces.Add(new FaceData { VertexIndices = face.ToList() });
                }
            }

            return meshData;
        }

        private static ColorData GetColorByIndex(short colorIndex)
        {
            // ACI colors (AutoCAD Color Index)
            switch (colorIndex)
            {
                case 1: return new ColorData(255, 0, 0);
                case 2: return new ColorData(255, 255, 0);
                case 3: return new ColorData(0, 255, 0);
                case 4: return new ColorData(0, 255, 255);
                case 5: return new ColorData(0, 0, 255);
                case 6: return new ColorData(255, 0, 255);
                case 7: return new ColorData(255, 255, 255);
                default: return new ColorData(0, 0, 0);
            }
        }
    }
}