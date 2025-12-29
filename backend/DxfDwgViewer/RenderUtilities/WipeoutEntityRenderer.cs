using System;
using System.Linq;
using ACadSharp.Entities;
using System.Collections.Generic;
using ACadSharp.Extensions;
using CSMath;

namespace DxfDwgViewer.RenderUtilities
{
    public class WipeoutEntityRenderer
    {
        public class WipeoutData
        {
            public List<Point3DData> BoundaryPoints { get; set; }
            public Point3DData InsertPoint { get; set; }
            public Point3DData UVector { get; set; }
            public Point3DData VVector { get; set; }
            public Point3DData Size { get; set; }
            public ClipType ClipType { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            public byte Brightness { get; set; }
            public byte Contrast { get; set; }
            public byte Fade { get; set; }
            public ImageDisplayFlags Flags { get; set; }
            public bool ClippingState { get; set; }
            
            public BoundsData? Bounds { get; set; }
            public Point3DData? Centroid { get; set; }
            public int BoundaryPointCount { get; set; }
            public double Area { get; set; }
            
            public TransformData Transform { get; set; }
            public GeometryData Geometry { get; set; }
            public MaterialData Material { get; set; }
            public double[] VertexPositions { get; set; }
            public double[] VertexColors { get; set; }
            public double[] VertexNormals { get; set; }
            public double[] UV { get; set; }
            public int[] Indices { get; set; }
            public int VertexCount { get; set; }
            public double Opacity { get; set; }
            public bool Transparent { get; set; }
            public bool IsMask { get; set; }
            public NormalData Normal { get; set; }
            public ColorData Color { get; set; }
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

        private static void UpdateTransformMatrix(TransformData transform, double rotationZ = 0)
        {
            double cos = Math.Cos(rotationZ);
            double sin = Math.Sin(rotationZ);

            transform.Matrix = new double[16]
            {
                transform.Scale.X * cos, transform.Scale.X * sin, 0, transform.Position.X,
                -transform.Scale.Y * sin, transform.Scale.Y * cos, 0, transform.Position.Y,
                0, 0, transform.Scale.Z, transform.Position.Z,
                0, 0, 0, 1
            };
        }

        private static int[] GeneratePolygonIndices(int vertexCount)
        {
            var indices = new List<int>();
            for (int i = 1; i < vertexCount - 1; i++)
            {
                indices.Add(0);
                indices.Add(i);
                indices.Add(i + 1);
            }
            return indices.ToArray();
        }

        private static NormalData CalculatePolygonNormal(List<Point3DData> points)
        {
            if (points.Count < 3)
                return new NormalData(0, 0, 1);

            double sumX = 0, sumY = 0, sumZ = 0;
            for (int i = 0; i < points.Count; i++)
            {
                int j = (i + 1) % points.Count;
                sumX += (points[i].Y - points[j].Y) * (points[i].Z + points[j].Z);
                sumY += (points[i].Z - points[j].Z) * (points[i].X + points[j].X);
                sumZ += (points[i].X - points[j].X) * (points[i].Y + points[j].Y);
            }

            return new NormalData(sumX, sumY, sumZ);
        }

        public static WipeoutData Render(Wipeout wipeout)
        {
            BoundsData? bounds = null;
            Point3DData? centroid = null;
            double area = 0;
            
            var wipeoutData = new WipeoutData
            {
                BoundaryPoints = new List<Point3DData>(),
                InsertPoint = new Point3DData(wipeout.InsertPoint.X, wipeout.InsertPoint.Y, wipeout.InsertPoint.Z),
                UVector = new Point3DData(wipeout.UVector.X, wipeout.UVector.Y, wipeout.UVector.Z),
                VVector = new Point3DData(wipeout.VVector.X, wipeout.VVector.Y, wipeout.VVector.Z),
                Size = new Point3DData(wipeout.Size.X, wipeout.Size.Y, 0),
                ClipType = wipeout.ClipType,
                ColorIndex = wipeout.Color.Index,
                LineTypeName = wipeout.GetActiveLineType()?.Name ?? "",
                LineWeight = wipeout.GetActiveLineWeightType().GetLineWeightValue(),
                Brightness = wipeout.Brightness,
                Contrast = wipeout.Contrast,
                Fade = wipeout.Fade,
                Flags = wipeout.Flags,
                ClippingState = wipeout.ClippingState
            };

            if (wipeout.ClipType == ClipType.Rectangular)
            {
                double insertX = wipeout.InsertPoint.X;
                double insertY = wipeout.InsertPoint.Y;

                double uX = wipeout.UVector.X * wipeout.Size.X;
                double uY = wipeout.UVector.Y * wipeout.Size.X;
                double vX = wipeout.VVector.X * wipeout.Size.Y;
                double vY = wipeout.VVector.Y * wipeout.Size.Y;

                var corners = new Point3DData[]
                {
                    new Point3DData(insertX, insertY, wipeout.InsertPoint.Z),
                    new Point3DData(insertX + uX, insertY + uY, wipeout.InsertPoint.Z),
                    new Point3DData(insertX + uX + vX, insertY + uY + vY, wipeout.InsertPoint.Z),
                    new Point3DData(insertX + vX, insertY + vY, wipeout.InsertPoint.Z)
                };

                foreach (var corner in corners)
                {
                    wipeoutData.BoundaryPoints.Add(corner);
                }

                double minX = corners[0].X, maxX = corners[0].X;
                double minY = corners[0].Y, maxY = corners[0].Y;
                foreach (var point in corners)
                {
                    if (point.X < minX) minX = point.X;
                    if (point.X > maxX) maxX = point.X;
                    if (point.Y < minY) minY = point.Y;
                    if (point.Y > maxY) maxY = point.Y;
                }
                
                bounds = new BoundsData(
                    new Point3DData(minX, minY, wipeout.InsertPoint.Z),
                    new Point3DData(maxX, maxY, wipeout.InsertPoint.Z)
                );
                
                centroid = new Point3DData(
                    (minX + maxX) / 2.0,
                    (minY + maxY) / 2.0,
                    wipeout.InsertPoint.Z
                );
                
                for (int i = 0; i < corners.Length; i++)
                {
                    int j = (i + 1) % corners.Length;
                    area += corners[i].X * corners[j].Y;
                    area -= corners[j].X * corners[i].Y;
                }
                area = Math.Abs(area) / 2.0;
            }
            else if (wipeout.ClipType == ClipType.Polygonal)
            {
                if (wipeout.ClipBoundaryVertices != null && wipeout.ClipBoundaryVertices.Count >= 3)
                {
                    var boundaryPoints = wipeout.ClipBoundaryVertices
                        .Select(v => new Point3DData(
                            v.X + wipeout.InsertPoint.X,
                            v.Y + wipeout.InsertPoint.Y,
                            wipeout.InsertPoint.Z))
                        .ToArray();

                    foreach (var point in boundaryPoints)
                    {
                        wipeoutData.BoundaryPoints.Add(point);
                    }
                    
                    double minX = boundaryPoints[0].X, maxX = boundaryPoints[0].X;
                    double minY = boundaryPoints[0].Y, maxY = boundaryPoints[0].Y;
                    foreach (var point in boundaryPoints)
                    {
                        if (point.X < minX) minX = point.X;
                        if (point.X > maxX) maxX = point.X;
                        if (point.Y < minY) minY = point.Y;
                        if (point.Y > maxY) maxY = point.Y;
                    }
                    
                    bounds = new BoundsData(
                        new Point3DData(minX, minY, wipeout.InsertPoint.Z),
                        new Point3DData(maxX, maxY, wipeout.InsertPoint.Z)
                    );
                    
                    double sumX = 0, sumY = 0;
                    foreach (var point in boundaryPoints)
                    {
                        sumX += point.X;
                        sumY += point.Y;
                    }
                    
                    centroid = new Point3DData(
                        sumX / boundaryPoints.Length,
                        sumY / boundaryPoints.Length,
                        wipeout.InsertPoint.Z
                    );
                    
                    for (int i = 0; i < boundaryPoints.Length; i++)
                    {
                        int j = (i + 1) % boundaryPoints.Length;
                        area += boundaryPoints[i].X * boundaryPoints[j].Y;
                        area -= boundaryPoints[j].X * boundaryPoints[i].Y;
                    }
                    area = Math.Abs(area) / 2.0;
                }
            }
            else
            {
                double x = wipeout.InsertPoint.X;
                double y = wipeout.InsertPoint.Y;
                
                wipeoutData.BoundaryPoints.Add(new Point3DData(x - 10, y - 10, wipeout.InsertPoint.Z));
                wipeoutData.BoundaryPoints.Add(new Point3DData(x + 10, y - 10, wipeout.InsertPoint.Z));
                wipeoutData.BoundaryPoints.Add(new Point3DData(x + 10, y + 10, wipeout.InsertPoint.Z));
                wipeoutData.BoundaryPoints.Add(new Point3DData(x - 10, y + 10, wipeout.InsertPoint.Z));
                
                bounds = new BoundsData(
                    new Point3DData(x - 10, y - 10, wipeout.InsertPoint.Z),
                    new Point3DData(x + 10, y + 10, wipeout.InsertPoint.Z)
                );
                
                centroid = new Point3DData(x, y, wipeout.InsertPoint.Z);
                
                area = 400;
            }
            
            wipeoutData.Bounds = bounds;
            wipeoutData.Centroid = centroid;
            wipeoutData.BoundaryPointCount = wipeoutData.BoundaryPoints.Count;
            wipeoutData.Area = area;
            
            int colorValue = GetColorRgbByIndex(wipeoutData.ColorIndex);
            
            var transform = new TransformData();
            transform.Position = centroid ?? new Point3DData(wipeout.InsertPoint.X, wipeout.InsertPoint.Y, wipeout.InsertPoint.Z);
            transform.Rotation = new Point3DData(0, 0, 0);
            transform.Scale = new Point3DData(1, 1, 1);
            
            UpdateTransformMatrix(transform);
            
            int faceCount = wipeoutData.BoundaryPoints.Count >= 3 ? wipeoutData.BoundaryPoints.Count - 2 : 0;
            int indexCount = faceCount * 3;
            
            var geometry = new GeometryData
            {
                Type = "PolygonGeometry",
                VertexCount = wipeoutData.BoundaryPoints.Count,
                FaceCount = faceCount,
                HasNormals = true,
                HasColors = true,
                HasUVs = true,
                HasIndices = true,
                PrimitiveType = "Triangles",
                IndexCount = indexCount,
                IsClosed = true,
                IsPeriodic = false
            };
            
            var material = new MaterialData
            {
                Type = "MeshBasicMaterial",
                Color = colorValue,
                Opacity = 1.0,
                Transparent = false,
                Wireframe = false,
                LineWidth = 1.0,
                VertexColors = true,
                Side = true
            };
            
            var vertexPositions = new List<double>();
            foreach (var point in wipeoutData.BoundaryPoints)
            {
                vertexPositions.Add(point.X);
                vertexPositions.Add(point.Y);
                vertexPositions.Add(point.Z);
            }
            
            var vertexColors = new List<double>();
            for (int i = 0; i < wipeoutData.BoundaryPoints.Count; i++)
            {
                double r = ((colorValue >> 16) & 0xFF) / 255.0;
                double g = ((colorValue >> 8) & 0xFF) / 255.0;
                double b = (colorValue & 0xFF) / 255.0;
                vertexColors.Add(r);
                vertexColors.Add(g);
                vertexColors.Add(b);
            }
            
            var normal = CalculatePolygonNormal(wipeoutData.BoundaryPoints);
            var vertexNormals = new List<double>();
            for (int i = 0; i < wipeoutData.BoundaryPoints.Count; i++)
            {
                vertexNormals.Add(normal.X);
                vertexNormals.Add(normal.Y);
                vertexNormals.Add(normal.Z);
            }
            
            var uv = new List<double>();
            if (bounds != null)
            {
                double width = bounds.Max.X - bounds.Min.X;
                double height = bounds.Max.Y - bounds.Min.Y;
                foreach (var point in wipeoutData.BoundaryPoints)
                {
                    double u = width > 0 ? (point.X - bounds.Min.X) / width : 0;
                    double v = height > 0 ? (point.Y - bounds.Min.Y) / height : 0;
                    uv.Add(u);
                    uv.Add(v);
                }
            }
            else
            {
                for (int i = 0; i < wipeoutData.BoundaryPoints.Count; i++)
                {
                    uv.Add(0);
                    uv.Add(0);
                }
            }
            
            int[] indices = GeneratePolygonIndices(wipeoutData.BoundaryPoints.Count);
            
            wipeoutData.Transform = transform;
            wipeoutData.Geometry = geometry;
            wipeoutData.Material = material;
            wipeoutData.VertexPositions = vertexPositions.ToArray();
            wipeoutData.VertexColors = vertexColors.ToArray();
            wipeoutData.VertexNormals = vertexNormals.ToArray();
            wipeoutData.UV = uv.ToArray();
            wipeoutData.Indices = indices;
            wipeoutData.VertexCount = wipeoutData.BoundaryPoints.Count;
            wipeoutData.Opacity = 1.0;
            wipeoutData.Transparent = false;
            wipeoutData.IsMask = true;
            wipeoutData.Normal = normal;
            wipeoutData.Color = new ColorData(wipeoutData.ColorIndex);
            
            return wipeoutData;
        }
    }
}
