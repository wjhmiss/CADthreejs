using System.Collections.Generic;
using System.Linq;
using ACadSharp.Entities;
using System;
using ACadSharp.Extensions;
using ACadSharp.Tables;
using CSMath;
using Color = ACadSharp.Color;

namespace DxfDwgViewer.RenderUtilities
{
    public class PolygonMeshEntityRenderer
    {
        public class PolygonMeshData
        {
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
            public int SubdivisionLevel { get; set; }
            public short Version { get; set; }
            public bool BlendCrease { get; set; }
            
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

        public class EdgeData
        {
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public double? Crease { get; set; }
        }

        public static PolygonMeshData Render(Mesh mesh)
        {
            var polygonMeshData = new PolygonMeshData
            {
                Vertices = new List<VertexData>(),
                Faces = new List<FaceData>(),
                Edges = new List<EdgeData>(),
                Vertices3D = new List<Point3DData>(),
                Normals = new List<NormalData>(),
                Indices = new List<int>(),
                NormalsArray = new List<double>(),
                ColorsArray = new List<double>(),
                UVsArray = new List<double>(),
                Transform = new TransformData(),
                
                ColorIndex = mesh.Color.Index,
                LineTypeName = mesh.GetActiveLineType()?.Name ?? "",
                LineWeight = mesh.GetActiveLineWeightType().GetLineWeightValue(),
                VertexCount = mesh.Vertices?.Count ?? 0,
                FaceCount = mesh.Faces?.Count ?? 0,
                EdgeCount = mesh.Edges?.Count ?? 0,
                SubdivisionLevel = mesh.SubdivisionLevel,
                Version = mesh.Version,
                BlendCrease = mesh.BlendCrease,
                
                EntityType = "Mesh",
                Visible = !mesh.IsInvisible,
                LayerName = mesh.Layer?.Name ?? "",
                LayerIndex = 0,
                Handle = mesh.Handle.ToString(),
                
                Transparency = 0.0,
                MaterialName = mesh.Material?.Name ?? "",
                
                CastShadows = true,
                ReceiveShadows = true,
                
                GeometryType = "BufferGeometry",
                DoubleSided = false,
                FlatShading = false
            };
            
            polygonMeshData.Color = new ColorData(mesh.Color.Index);

            if (mesh.Vertices != null && mesh.Vertices.Count > 0)
            {
                double minX = mesh.Vertices[0].X, maxX = mesh.Vertices[0].X;
                double minY = mesh.Vertices[0].Y, maxY = mesh.Vertices[0].Y;
                double minZ = mesh.Vertices[0].Z, maxZ = mesh.Vertices[0].Z;
                double sumX = 0, sumY = 0, sumZ = 0;

                foreach (var vertex in mesh.Vertices)
                {
                    if (vertex.X < minX) minX = vertex.X;
                    if (vertex.X > maxX) maxX = vertex.X;
                    if (vertex.Y < minY) minY = vertex.Y;
                    if (vertex.Y > maxY) maxY = vertex.Y;
                    if (vertex.Z < minZ) minZ = vertex.Z;
                    if (vertex.Z > maxZ) maxZ = vertex.Z;

                    sumX += vertex.X;
                    sumY += vertex.Y;
                    sumZ += vertex.Z;
                }

                var minPoint = new Point3DData(minX, minY, minZ);
                var maxPoint = new Point3DData(maxX, maxY, maxZ);
                polygonMeshData.Bounds3D = new BoundsData3D(minPoint, maxPoint);

                polygonMeshData.Centroid = new Point3DData(
                    sumX / mesh.Vertices.Count,
                    sumY / mesh.Vertices.Count,
                    sumZ / mesh.Vertices.Count
                );
            }

            if (mesh.Vertices == null || mesh.Vertices.Count == 0)
                return polygonMeshData;

            foreach (var vertex in mesh.Vertices)
            {
                var vertexData = new VertexData
                {
                    Location = new Point3DData(vertex.X, vertex.Y, vertex.Z),
                    Normal = new NormalData(0, 0, 1),
                    Color = polygonMeshData.Color
                };
                
                polygonMeshData.Vertices.Add(vertexData);
                polygonMeshData.Vertices3D.Add(vertexData.Location);
            }

            if (mesh.Edges != null)
            {
                foreach (var edge in mesh.Edges)
                {
                    var edgeData = new EdgeData
                    {
                        StartIndex = edge.Start,
                        EndIndex = edge.End,
                        Crease = edge.Crease
                    };
                    polygonMeshData.Edges.Add(edgeData);
                }
            }

            if (mesh.Faces != null)
            {
                foreach (var face in mesh.Faces)
                {
                    if (face != null && face.Length >= 3)
                    {
                        var faceData = new FaceData
                        {
                            VertexIndices = new List<int>(face)
                        };
                        
                        // 添加索引到Indices列表（用于three.js）
                        if (face.Length == 3)
                        {
                            polygonMeshData.Indices.AddRange(face);
                        }
                        else if (face.Length == 4)
                        {
                            // 四边形拆分为两个三角形
                            polygonMeshData.Indices.Add(face[0]);
                            polygonMeshData.Indices.Add(face[1]);
                            polygonMeshData.Indices.Add(face[2]);
                            polygonMeshData.Indices.Add(face[0]);
                            polygonMeshData.Indices.Add(face[2]);
                            polygonMeshData.Indices.Add(face[3]);
                        }
                        
                        if (face.Length >= 3 && mesh.Vertices != null)
                        {
                            var v1 = mesh.Vertices[face[0]];
                            var v2 = mesh.Vertices[face[1]];
                            var v3 = mesh.Vertices[face[2]];
                            
                            var edge1 = new XYZ(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);
                            var edge2 = new XYZ(v3.X - v1.X, v3.Y - v1.Y, v3.Z - v1.Z);
                            
                            var normal = XYZ.Cross(edge1, edge2);
                            normal = normal.Normalize();
                            
                            faceData.Normal = new NormalData(normal.X, normal.Y, normal.Z);
                        }
                        
                        polygonMeshData.Faces.Add(faceData);
                    }
                }
            }

            foreach (var vertex in polygonMeshData.Vertices)
            {
                var vertexIndex = polygonMeshData.Vertices.IndexOf(vertex);
                var adjacentFaces = polygonMeshData.Faces.Where(f => f.VertexIndices.Contains(vertexIndex)).ToList();
                
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
            
            polygonMeshData.Normals = polygonMeshData.Vertices.Select(v => v.Normal).ToList();
            
            // 构建three.js兼容的法向量数组
            foreach (var normal in polygonMeshData.Normals)
            {
                polygonMeshData.NormalsArray.Add(normal.X);
                polygonMeshData.NormalsArray.Add(normal.Y);
                polygonMeshData.NormalsArray.Add(normal.Z);
            }
            
            // 构建three.js兼容的颜色数组
            var rgbColor = polygonMeshData.Color.ToRGB();
            foreach (var vertex in polygonMeshData.Vertices)
            {
                polygonMeshData.ColorsArray.Add(rgbColor[0]);
                polygonMeshData.ColorsArray.Add(rgbColor[1]);
                polygonMeshData.ColorsArray.Add(rgbColor[2]);
            }
            
            // 构建three.js兼容的UV数组（使用简单的平面映射）
            if (polygonMeshData.Bounds3D != null)
            {
                double sizeX = polygonMeshData.Bounds3D.Size.X;
                double sizeY = polygonMeshData.Bounds3D.Size.Y;
                double sizeZ = polygonMeshData.Bounds3D.Size.Z;
                double maxDim = Math.Max(Math.Max(sizeX, sizeY), sizeZ);
                
                foreach (var vertex in polygonMeshData.Vertices)
                {
                    double u = (vertex.Location.X - polygonMeshData.Bounds3D.Min.X) / maxDim;
                    double v = (vertex.Location.Y - polygonMeshData.Bounds3D.Min.Y) / maxDim;
                    polygonMeshData.UVsArray.Add(u);
                    polygonMeshData.UVsArray.Add(v);
                }
            }

            return polygonMeshData;
        }
    }
}