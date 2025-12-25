using System.Collections.Generic;
using System.Linq;
using ACadSharp.Entities;
using System;
using ACadSharp.Extensions;
using DxfDwgViewer.RenderUtilities;
using CSMath;

namespace DxfDwgViewer.RenderUtilities
{
    public class PolyfaceMeshEntityRenderer
    {
        public class PolyfaceMeshData
        {
            public List<VertexData> Vertices { get; set; }
            public List<FaceData> Faces { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            
            // 基本几何属性
            public int VertexCount { get; set; }
            public int FaceCount { get; set; }
            
            // three.js兼容性属性
            public List<Point3DData> Vertices3D { get; set; }
            public List<int> Indices { get; set; }
            public List<double> NormalsArray { get; set; }
            public List<double> ColorsArray { get; set; }
            public List<double> UVsArray { get; set; }
            public ColorData Color { get; set; }
            public BoundsData3D Bounds3D { get; set; }
            public Point3DData Centroid { get; set; }
            public List<NormalData> Normals { get; set; }
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
            
            // 渲染属性
            public bool CastShadows { get; set; }
            public bool ReceiveShadows { get; set; }
            
            // three.js几何体属性
            public string GeometryType { get; set; }
            public bool DoubleSided { get; set; }
            public bool FlatShading { get; set; }
        }

        public class VertexData
        {
            public Point3DData Location { get; set; }
            public NormalData Normal { get; set; }
            public ColorData Color { get; set; }
            
            public VertexData()
            {
                Location = new Point3DData(0, 0, 0);
                Normal = new NormalData(0, 0, 1);
                Color = new ColorData();
            }
        }

        public class FaceData
        {
            public List<int> VertexIndices { get; set; }
            public NormalData Normal { get; set; }
            
            public FaceData()
            {
                VertexIndices = new List<int>();
                Normal = new NormalData(0, 0, 1);
            }
        }

        public static PolyfaceMeshData Render(PolyfaceMesh polyfaceMesh)
        {
            // 初始化three.js兼容性数据
            var polyfaceMeshData = new PolyfaceMeshData
            {
                Vertices = new List<VertexData>(),
                Faces = new List<FaceData>(),
                Vertices3D = new List<Point3DData>(),
                Normals = new List<NormalData>(),
                Indices = new List<int>(),
                NormalsArray = new List<double>(),
                ColorsArray = new List<double>(),
                UVsArray = new List<double>(),
                Transform = new TransformData(),
                
                // 基本属性
                ColorIndex = polyfaceMesh.Color.Index,
                LineTypeName = polyfaceMesh.GetActiveLineType()?.Name ?? "",
                LineWeight = polyfaceMesh.GetActiveLineWeightType().GetLineWeightValue(),
                VertexCount = polyfaceMesh.Vertices?.Count ?? 0,
                FaceCount = polyfaceMesh.Faces?.Count() ?? 0,
                
                // 实体属性
                EntityType = "PolyfaceMesh",
                Visible = !polyfaceMesh.IsInvisible,
                LayerName = polyfaceMesh.Layer?.Name ?? "",
                LayerIndex = 0,
                Handle = polyfaceMesh.Handle.ToString(),
                
                // 材质属性
                Transparency = 0.0,
                MaterialName = polyfaceMesh.Material?.Name ?? "",
                
                // 渲染属性
                CastShadows = true,
                ReceiveShadows = true,
                
                // three.js几何体属性
                GeometryType = "BufferGeometry",
                DoubleSided = false,
                FlatShading = false
            };
            
            // 设置颜色数据
            polyfaceMeshData.Color = new ColorData(polyfaceMesh.Color.Index);
            
            // 计算边界框和质心
            if (polyfaceMesh.Vertices != null && polyfaceMesh.Vertices.Count > 0)
            {
                double minX = polyfaceMesh.Vertices[0].Location.X, maxX = polyfaceMesh.Vertices[0].Location.X;
                double minY = polyfaceMesh.Vertices[0].Location.Y, maxY = polyfaceMesh.Vertices[0].Location.Y;
                double minZ = polyfaceMesh.Vertices[0].Location.Z, maxZ = polyfaceMesh.Vertices[0].Location.Z;
                double sumX = 0, sumY = 0, sumZ = 0;
                
                foreach (var vertex in polyfaceMesh.Vertices)
                {
                    // 更新边界框
                    if (vertex.Location.X < minX) minX = vertex.Location.X;
                    if (vertex.Location.X > maxX) maxX = vertex.Location.X;
                    if (vertex.Location.Y < minY) minY = vertex.Location.Y;
                    if (vertex.Location.Y > maxY) maxY = vertex.Location.Y;
                    if (vertex.Location.Z < minZ) minZ = vertex.Location.Z;
                    if (vertex.Location.Z > maxZ) maxZ = vertex.Location.Z;
                    
                    // 累加用于计算质心
                    sumX += vertex.Location.X;
                    sumY += vertex.Location.Y;
                    sumZ += vertex.Location.Z;
                }
                
                // 创建3D边界框
                var minPoint = new Point3DData(minX, minY, minZ);
                var maxPoint = new Point3DData(maxX, maxY, maxZ);
                polyfaceMeshData.Bounds3D = new BoundsData3D(minPoint, maxPoint);
                
                // 计算质心
                polyfaceMeshData.Centroid = new Point3DData(
                    sumX / polyfaceMesh.Vertices.Count,
                    sumY / polyfaceMesh.Vertices.Count,
                    sumZ / polyfaceMesh.Vertices.Count
                );
            }

            // 检查顶点和面是否存在
            if (polyfaceMesh.Vertices == null || polyfaceMesh.Faces == null)
                return polyfaceMeshData;

            // 收集所有顶点数据
            foreach (var vertex in polyfaceMesh.Vertices)
            {
                var vertexData = new VertexData
                {
                    Location = new Point3DData(vertex.Location.X, vertex.Location.Y, vertex.Location.Z),
                    Normal = new NormalData(0, 0, 1), // 默认法向量
                    Color = polyfaceMeshData.Color
                };
                
                polyfaceMeshData.Vertices.Add(vertexData);
                polyfaceMeshData.Vertices3D.Add(vertexData.Location);
            }

            // 绘制每个面并收集面数据
            foreach (var face in polyfaceMesh.Faces)
            {
                try
                {
                    var indices = GetFaceIndices(polyfaceMesh, face);
                    
                    // 添加面数据
                    if (indices.Count >= 3)
                    {
                        var faceData = new FaceData
                        {
                            VertexIndices = new List<int>(indices)
                        };
                        
                        // 添加索引到Indices列表（用于three.js）
                        if (indices.Count == 3)
                        {
                            polyfaceMeshData.Indices.AddRange(indices);
                        }
                        else if (indices.Count == 4)
                        {
                            // 四边形拆分为两个三角形
                            polyfaceMeshData.Indices.Add(indices[0]);
                            polyfaceMeshData.Indices.Add(indices[1]);
                            polyfaceMeshData.Indices.Add(indices[2]);
                            polyfaceMeshData.Indices.Add(indices[0]);
                            polyfaceMeshData.Indices.Add(indices[2]);
                            polyfaceMeshData.Indices.Add(indices[3]);
                        }
                        
                        // 计算面的法向量
                        if (indices.Count >= 3)
                        {
                            var v1 = polyfaceMesh.Vertices[indices[0]].Location;
                            var v2 = polyfaceMesh.Vertices[indices[1]].Location;
                            var v3 = polyfaceMesh.Vertices[indices[2]].Location;
                            
                            // 计算两个边向量
                            var edge1 = new XYZ(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);
                            var edge2 = new XYZ(v3.X - v1.X, v3.Y - v1.Y, v3.Z - v1.Z);
                            
                            // 计算叉积得到法向量
                            var normal = XYZ.Cross(edge1, edge2);
                            normal = normal.Normalize();
                            
                            faceData.Normal = new NormalData(normal.X, normal.Y, normal.Z);
                        }
                        
                        polyfaceMeshData.Faces.Add(faceData);
                    }
                }
                catch
                {
                    // 忽略单个面的绘制错误
                }
            }
            
            // 计算顶点法向量（平均相邻面的法向量）
            foreach (var vertex in polyfaceMeshData.Vertices)
            {
                var vertexIndex = polyfaceMeshData.Vertices.IndexOf(vertex);
                var adjacentFaces = polyfaceMeshData.Faces.Where(f => f.VertexIndices.Contains(vertexIndex)).ToList();
                
                if (adjacentFaces.Count > 0)
                {
                    double avgX = 0, avgY = 0, avgZ = 0;
                    foreach (var face in adjacentFaces)
                    {
                        avgX += face.Normal.X;
                        avgY += face.Normal.Y;
                        avgZ += face.Normal.Z;
                    }
                    
                    vertex.Normal = new NormalData(
                        avgX / adjacentFaces.Count,
                        avgY / adjacentFaces.Count,
                        avgZ / adjacentFaces.Count
                    );
                }
            }
            
            // 将所有法向量添加到Normals列表中
            polyfaceMeshData.Normals = polyfaceMeshData.Vertices.Select(v => v.Normal).ToList();
            
            // 构建three.js兼容的法向量数组
            foreach (var normal in polyfaceMeshData.Normals)
            {
                polyfaceMeshData.NormalsArray.Add(normal.X);
                polyfaceMeshData.NormalsArray.Add(normal.Y);
                polyfaceMeshData.NormalsArray.Add(normal.Z);
            }
            
            // 构建three.js兼容的颜色数组
            var rgbColor = polyfaceMeshData.Color.ToRGB();
            foreach (var vertex in polyfaceMeshData.Vertices)
            {
                polyfaceMeshData.ColorsArray.Add(rgbColor[0]);
                polyfaceMeshData.ColorsArray.Add(rgbColor[1]);
                polyfaceMeshData.ColorsArray.Add(rgbColor[2]);
            }
            
            // 构建three.js兼容的UV数组（使用简单的平面映射）
            if (polyfaceMeshData.Bounds3D != null)
            {
                double sizeX = polyfaceMeshData.Bounds3D.Size.X;
                double sizeY = polyfaceMeshData.Bounds3D.Size.Y;
                double sizeZ = polyfaceMeshData.Bounds3D.Size.Z;
                double maxDim = Math.Max(Math.Max(sizeX, sizeY), sizeZ);
                
                foreach (var vertex in polyfaceMeshData.Vertices)
                {
                    double u = (vertex.Location.X - polyfaceMeshData.Bounds3D.Min.X) / maxDim;
                    double v = (vertex.Location.Y - polyfaceMeshData.Bounds3D.Min.Y) / maxDim;
                    polyfaceMeshData.UVsArray.Add(u);
                    polyfaceMeshData.UVsArray.Add(v);
                }
            }

            return polyfaceMeshData;
        }

        private static List<int> GetFaceIndices(PolyfaceMesh polyfaceMesh, VertexFaceRecord face)
        {
            var indexList = new List<int>();

            if (face.Index1 != 0 && Math.Abs(face.Index1) <= polyfaceMesh.Vertices.Count)
            {
                indexList.Add(Math.Abs(face.Index1) - 1);
            }

            if (face.Index2 != 0 && Math.Abs(face.Index2) <= polyfaceMesh.Vertices.Count)
            {
                indexList.Add(Math.Abs(face.Index2) - 1);
            }

            if (face.Index3 != 0 && Math.Abs(face.Index3) <= polyfaceMesh.Vertices.Count)
            {
                indexList.Add(Math.Abs(face.Index3) - 1);
            }

            if (face.Index4 != 0 && Math.Abs(face.Index4) <= polyfaceMesh.Vertices.Count)
            {
                indexList.Add(Math.Abs(face.Index4) - 1);
            }

            return indexList;
        }
    }
}