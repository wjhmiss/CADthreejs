using System.Linq;
using ACadSharp.Types;
using ACadSharp.Entities;
using System.Collections.Generic;
using ACadSharp.Extensions;
using static ACadSharp.Entities.Polyline3D;

namespace DxfDwgViewer.RenderUtilities
{
    public class Polyline3DEntityRenderer
    {
        public class Polyline3DData
        {
            public List<VertexData> Vertices { get; set; }
            public bool IsClosed { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            public double Elevation { get; set; }
            public double Thickness { get; set; }
            public int VertexCount { get; set; }
            public BoundsData Bounds { get; set; }
            public Point3DData Centroid { get; set; }
            public double TotalLength { get; set; }
            
            public double[] VertexPositions { get; set; }
            public double[] VertexNormals { get; set; }
            public double[] VertexUVs { get; set; }
            public int[] Indices { get; set; }
            public TransformData Transform { get; set; }
            public MaterialData Material { get; set; }
            public GeometryData Geometry { get; set; }
            public bool HasSegments { get; set; }
            public List<SegmentData> Segments { get; set; }
        }

        public class VertexData
        {
            public Point3DData Location { get; set; }
        }

        public static Polyline3DData Render(Polyline3D polyline3D)
        {
            Point3DData? centroid = null;
            double totalLength = 0;
            List<double> vertexPositionsList = new List<double>();
            List<double> vertexNormalsList = new List<double>();
            List<double> vertexUVsList = new List<double>();
            List<int> indicesList = new List<int>();
            
            if (polyline3D.Vertices != null && polyline3D.Vertices.Any())
            {
                var vertices = polyline3D.Vertices.ToList();
                if (vertices.Count > 0)
                {
                    double minX = vertices[0].Location.X, maxX = vertices[0].Location.X;
                    double minY = vertices[0].Location.Y, maxY = vertices[0].Location.Y;
                    double minZ = vertices[0].Location.Z, maxZ = vertices[0].Location.Z;
                    double sumX = 0, sumY = 0, sumZ = 0;
                    
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        var currentVertex = vertices[i];
                        var nextVertex = (i + 1 < vertices.Count) ? vertices[i + 1] : null;
                        
                        if (currentVertex.Location.X < minX) minX = currentVertex.Location.X;
                        if (currentVertex.Location.X > maxX) maxX = currentVertex.Location.X;
                        if (currentVertex.Location.Y < minY) minY = currentVertex.Location.Y;
                        if (currentVertex.Location.Y > maxY) maxY = currentVertex.Location.Y;
                        if (currentVertex.Location.Z < minZ) minZ = currentVertex.Location.Z;
                        if (currentVertex.Location.Z > maxZ) maxZ = currentVertex.Location.Z;
                        
                        sumX += currentVertex.Location.X;
                        sumY += currentVertex.Location.Y;
                        sumZ += currentVertex.Location.Z;
                        
                        if (nextVertex != null)
                        {
                            double dx = nextVertex.Location.X - currentVertex.Location.X;
                            double dy = nextVertex.Location.Y - currentVertex.Location.Y;
                            double dz = nextVertex.Location.Z - currentVertex.Location.Z;
                            totalLength += Math.Sqrt(dx * dx + dy * dy + dz * dz);
                        }
                        else if (polyline3D.IsClosed && vertices.Count > 1)
                        {
                            var firstVertex = vertices[0];
                            double dx = firstVertex.Location.X - currentVertex.Location.X;
                            double dy = firstVertex.Location.Y - currentVertex.Location.Y;
                            double dz = firstVertex.Location.Z - currentVertex.Location.Z;
                            totalLength += Math.Sqrt(dx * dx + dy * dy + dz * dz);
                        }
                    }
                    
                    centroid = new Point3DData
                    {
                        X = sumX / vertices.Count,
                        Y = sumY / vertices.Count,
                        Z = sumZ / vertices.Count
                    };
                }
            }

            var polyline3DData = new Polyline3DData
            {
                Vertices = new List<VertexData>(),
                IsClosed = polyline3D.IsClosed,
                ColorIndex = polyline3D.Color.Index,
                LineTypeName = polyline3D.GetActiveLineType()?.Name ?? "",
                LineWeight = polyline3D.GetActiveLineWeightType().GetLineWeightValue(),
                Elevation = polyline3D.Elevation,
                Thickness = polyline3D.Thickness,
                VertexCount = polyline3D.Vertices?.Count() ?? 0,
                Centroid = centroid!,
                TotalLength = totalLength,
                Transform = new TransformData(),
                Material = new MaterialData(),
                Geometry = new GeometryData(),
                HasSegments = polyline3D.Vertices != null && polyline3D.Vertices.Count() > 1,
                Segments = new List<SegmentData>()
            };
            
            int vertexIndex = 0;
            if (polyline3D.Vertices != null)
            {
                foreach (var vertex in polyline3D.Vertices)
                {
                    polyline3DData.Vertices.Add(new VertexData
                    {
                        Location = new Point3DData
                        {
                            X = vertex.Location.X,
                            Y = vertex.Location.Y,
                            Z = vertex.Location.Z
                        }
                    });
                    
                    vertexPositionsList.Add(vertex.Location.X);
                    vertexPositionsList.Add(vertex.Location.Y);
                    vertexPositionsList.Add(vertex.Location.Z);
                    
                    var normal = CalculateVertexNormal(vertexIndex, polyline3D.Vertices.ToList(), polyline3D.IsClosed);
                    vertexNormalsList.Add(normal.X);
                    vertexNormalsList.Add(normal.Y);
                    vertexNormalsList.Add(normal.Z);
                    
                    double u = vertexIndex > 0 ? (double)vertexIndex / (polyline3D.Vertices.Count() - 1) : 0.0;
                    vertexUVsList.Add(u);
                    vertexUVsList.Add(0.0);
                    
                    if (vertexIndex < polyline3D.Vertices.Count() - 1)
                    {
                        indicesList.Add(vertexIndex);
                        indicesList.Add(vertexIndex + 1);
                        
                        var segment = new SegmentData
                        {
                            Start = new Point3DData { X = vertex.Location.X, Y = vertex.Location.Y, Z = vertex.Location.Z },
                            End = new Point3DData { 
                                X = polyline3D.Vertices.ElementAt(vertexIndex + 1).Location.X, 
                                Y = polyline3D.Vertices.ElementAt(vertexIndex + 1).Location.Y,
                                Z = polyline3D.Vertices.ElementAt(vertexIndex + 1).Location.Z
                            }
                        };
                        
                        double dx = segment.End.X - segment.Start.X;
                        double dy = segment.End.Y - segment.Start.Y;
                        double dz = segment.End.Z - segment.Start.Z;
                        segment.Length = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                        
                        if (segment.Length > 0)
                        {
                            segment.Direction = new Point3DData 
                            { 
                                X = dx / segment.Length, 
                                Y = dy / segment.Length, 
                                Z = dz / segment.Length 
                            };
                        }
                        
                        polyline3DData.Segments.Add(segment);
                    }
                    else if (polyline3D.IsClosed && polyline3D.Vertices.Count() > 1)
                    {
                        indicesList.Add(vertexIndex);
                        indicesList.Add(0);
                        
                        var segment = new SegmentData
                        {
                            Start = new Point3DData { X = vertex.Location.X, Y = vertex.Location.Y, Z = vertex.Location.Z },
                            End = new Point3DData { 
                                X = polyline3D.Vertices.First().Location.X, 
                                Y = polyline3D.Vertices.First().Location.Y,
                                Z = polyline3D.Vertices.First().Location.Z
                            }
                        };
                        
                        double dx = segment.End.X - segment.Start.X;
                        double dy = segment.End.Y - segment.Start.Y;
                        double dz = segment.End.Z - segment.Start.Z;
                        segment.Length = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                        
                        if (segment.Length > 0)
                        {
                            segment.Direction = new Point3DData 
                            { 
                                X = dx / segment.Length, 
                                Y = dy / segment.Length, 
                                Z = dz / segment.Length 
                            };
                        }
                        
                        polyline3DData.Segments.Add(segment);
                    }
                    
                    vertexIndex++;
                }
            }

            polyline3DData.VertexPositions = vertexPositionsList.ToArray();
            polyline3DData.VertexNormals = vertexNormalsList.ToArray();
            polyline3DData.VertexUVs = vertexUVsList.ToArray();
            polyline3DData.Indices = indicesList.ToArray();

            if (polyline3DData.Vertices.Count > 0)
            {
                double minX = polyline3DData.Vertices[0].Location.X, maxX = polyline3DData.Vertices[0].Location.X;
                double minY = polyline3DData.Vertices[0].Location.Y, maxY = polyline3DData.Vertices[0].Location.Y;
                double minZ = polyline3DData.Vertices[0].Location.Z, maxZ = polyline3DData.Vertices[0].Location.Z;
                
                foreach (var v in polyline3DData.Vertices)
                {
                    if (v.Location.X < minX) minX = v.Location.X;
                    if (v.Location.X > maxX) maxX = v.Location.X;
                    if (v.Location.Y < minY) minY = v.Location.Y;
                    if (v.Location.Y > maxY) maxY = v.Location.Y;
                    if (v.Location.Z < minZ) minZ = v.Location.Z;
                    if (v.Location.Z > maxZ) maxZ = v.Location.Z;
                }
                
                polyline3DData.Bounds = new BoundsData(
                    new Point3DData(minX, minY, minZ),
                    new Point3DData(maxX, maxY, maxZ)
                );
            }

            polyline3DData.Material.Type = "LineBasicMaterial";
            var color = GetColorRGB(polyline3D.Color.Index);
            polyline3DData.Material.Color = color;
            polyline3DData.Material.Opacity = 1.0;
            polyline3DData.Material.LineWidth = polyline3D.LineWeight switch
            {
                ACadSharp.LineWeightType.W0 => 0.0,
                ACadSharp.LineWeightType.W5 => 0.05,
                ACadSharp.LineWeightType.W9 => 0.09,
                ACadSharp.LineWeightType.W13 => 0.13,
                ACadSharp.LineWeightType.W15 => 0.15,
                ACadSharp.LineWeightType.W18 => 0.18,
                ACadSharp.LineWeightType.W20 => 0.20,
                ACadSharp.LineWeightType.W25 => 0.25,
                ACadSharp.LineWeightType.W30 => 0.30,
                ACadSharp.LineWeightType.W35 => 0.35,
                ACadSharp.LineWeightType.W40 => 0.40,
                ACadSharp.LineWeightType.W50 => 0.50,
                ACadSharp.LineWeightType.W53 => 0.53,
                ACadSharp.LineWeightType.W60 => 0.60,
                ACadSharp.LineWeightType.W70 => 0.70,
                ACadSharp.LineWeightType.W80 => 0.80,
                ACadSharp.LineWeightType.W90 => 0.90,
                ACadSharp.LineWeightType.W100 => 1.00,
                ACadSharp.LineWeightType.W106 => 1.06,
                ACadSharp.LineWeightType.W120 => 1.20,
                ACadSharp.LineWeightType.W140 => 1.40,
                ACadSharp.LineWeightType.W158 => 1.58,
                ACadSharp.LineWeightType.W200 => 2.00,
                ACadSharp.LineWeightType.W211 => 2.11,
                ACadSharp.LineWeightType.ByLayer => 1.0,
                ACadSharp.LineWeightType.ByBlock => 1.0,
                ACadSharp.LineWeightType.Default => 1.0,
                _ => 1.0
            };
            polyline3DData.Material.Transparent = false;
            polyline3DData.Material.Wireframe = false;
            
            polyline3DData.Geometry.Type = "BufferGeometry";
            polyline3DData.Geometry.VertexCount = polyline3D.Vertices?.Count() ?? 0;
            polyline3DData.Geometry.FaceCount = 0;
            polyline3DData.Geometry.HasNormals = polyline3DData.VertexNormals != null && polyline3DData.VertexNormals.Length > 0;
            polyline3DData.Geometry.HasUVs = polyline3DData.VertexUVs != null && polyline3DData.VertexUVs.Length > 0;
            polyline3DData.Geometry.HasIndices = polyline3DData.Indices != null && polyline3DData.Indices.Length > 0;
            polyline3DData.Geometry.PrimitiveType = "Line";
            polyline3DData.Geometry.IndexCount = polyline3DData.Indices != null ? polyline3DData.Indices.Length : 0;
            polyline3DData.Geometry.IsClosed = polyline3D.IsClosed;
            polyline3DData.Geometry.IsPeriodic = false;
            polyline3DData.Geometry.Degree = 1;
            
            polyline3DData.Transform.Position = centroid ?? new Point3DData();
            polyline3DData.Transform.Rotation = new Point3DData { X = 0, Y = 0, Z = 0 };
            polyline3DData.Transform.Scale = new Point3DData { X = 1, Y = 1, Z = 1 };
            
            return polyline3DData;
        }

        private static Point3DData CalculateVertexNormal(int vertexIndex, List<Vertex3D> vertices, bool isClosed)
        {
            if (vertices == null || vertices.Count == 0)
                return new Point3DData { X = 0, Y = 0, Z = 1 };
                
            if (vertices.Count == 1)
                return new Point3DData { X = 0, Y = 0, Z = 1 };
            
            Vertex3D prevVertex, nextVertex;
            
            if (isClosed)
            {
                prevVertex = vertices[(vertexIndex - 1 + vertices.Count) % vertices.Count];
                nextVertex = vertices[(vertexIndex + 1) % vertices.Count];
            }
            else
            {
                if (vertexIndex == 0)
                {
                    prevVertex = vertices[vertexIndex];
                    nextVertex = vertices[vertexIndex + 1];
                }
                else if (vertexIndex == vertices.Count - 1)
                {
                    prevVertex = vertices[vertexIndex - 1];
                    nextVertex = vertices[vertexIndex];
                }
                else
                {
                    prevVertex = vertices[vertexIndex - 1];
                    nextVertex = vertices[vertexIndex + 1];
                }
            }
            
            var v1 = new Point3DData
            {
                X = prevVertex.Location.X - vertices[vertexIndex].Location.X,
                Y = prevVertex.Location.Y - vertices[vertexIndex].Location.Y,
                Z = prevVertex.Location.Z - vertices[vertexIndex].Location.Z
            };
            
            var v2 = new Point3DData
            {
                X = nextVertex.Location.X - vertices[vertexIndex].Location.X,
                Y = nextVertex.Location.Y - vertices[vertexIndex].Location.Y,
                Z = nextVertex.Location.Z - vertices[vertexIndex].Location.Z
            };
            
            var normal = new Point3DData
            {
                X = v1.Y * v2.Z - v1.Z * v2.Y,
                Y = v1.Z * v2.X - v1.X * v2.Z,
                Z = v1.X * v2.Y - v1.Y * v2.X
            };
            
            var length = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
            if (length > 0.0001)
            {
                normal.X /= length;
                normal.Y /= length;
                normal.Z /= length;
            }
            else
            {
                normal = new Point3DData { X = 0, Y = 0, Z = 1 };
            }
            
            return normal;
        }
        
        /// <summary>
        /// 根据颜色索引获取RGB颜色值
        /// </summary>
        /// <param name="colorIndex">颜色索引</param>
        /// <returns>RGB颜色值</returns>
        private static int GetColorRGB(short colorIndex)
        {
            // ACI colors (AutoCAD Color Index)
            switch (colorIndex)
            {
                case 1: return 0xFF0000;
                case 2: return 0xFFFF00;
                case 3: return 0x00FF00;
                case 4: return 0x00FFFF;
                case 5: return 0x0000FF;
                case 6: return 0xFF00FF;
                case 7: return 0xFFFFFF;
                default: return 0x000000;
            }
        }
        
        public class SegmentData
        {
            public Point3DData Start { get; set; }
            public Point3DData End { get; set; }
            public double Length { get; set; }
            public Point3DData Direction { get; set; }
            
            public SegmentData()
            {
                Start = new Point3DData();
                End = new Point3DData();
                Length = 0;
                Direction = new Point3DData { X = 1, Y = 0, Z = 0 };
            }
        }
    }
}