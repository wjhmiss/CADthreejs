using System;
using ACadSharp.Entities;
using ACadSharp.Extensions;
using System.Collections.Generic;
using CSMath;

namespace DxfDwgViewer.RenderUtilities
{
    public class ShapeEntityRenderer
    {
        public class ShapeData
        {
            public Point3DData InsertionPoint { get; set; }
            public double Size { get; set; }
            public double Rotation { get; set; }
            public double RelativeXScale { get; set; }
            public double ObliqueAngle { get; set; }
            public Point3DData Normal { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            public double LineTypeScale { get; set; }
            public string ShapeStyleName { get; set; }
            public ushort ShapeIndex { get; set; }
            
            public List<Point3DData> BoundaryPoints { get; set; }
            public BoundsData Bounds { get; set; }
            public Point3DData Centroid { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public int BoundaryPointCount { get; set; }
            public double Thickness { get; set; }
            
            public TransformData Transform { get; set; }
            public GeometryData Geometry { get; set; }
            public MaterialData Material { get; set; }
            public ColorData Color { get; set; }
            public List<double> VertexPositions { get; set; }
            public List<double> VertexNormals { get; set; }
            public List<double> VertexColors { get; set; }
            public List<int> Indices { get; set; }
            
            public ShapeData()
            {
                InsertionPoint = new Point3DData();
                Size = 1.0;
                Rotation = 0.0;
                RelativeXScale = 1.0;
                ObliqueAngle = 0.0;
                Normal = new Point3DData(0, 0, 1);
                ColorIndex = 7;
                LineTypeName = "";
                LineWeight = 0.0;
                LineTypeScale = 1.0;
                ShapeStyleName = "";
                ShapeIndex = 0;
                BoundaryPoints = new List<Point3DData>();
                Bounds = new BoundsData();
                Centroid = new Point3DData();
                Width = 0.0;
                Height = 0.0;
                BoundaryPointCount = 0;
                Thickness = 0.0;
                Transform = new TransformData();
                Geometry = new GeometryData();
                Material = new MaterialData();
                Color = new ColorData();
                VertexPositions = new List<double>();
                VertexNormals = new List<double>();
                VertexColors = new List<double>();
                Indices = new List<int>();
            }
        }
        
        public class GeometryData
        {
            public string Type { get; set; }
            public int VertexCount { get; set; }
            public int FaceCount { get; set; }
            public bool HasNormals { get; set; }
            public bool HasColors { get; set; }
            public bool HasUVs { get; set; }
            public bool HasIndices { get; set; }
            public string PrimitiveType { get; set; }
            public int IndexCount { get; set; }
            
            public GeometryData()
            {
                Type = "";
                VertexCount = 0;
                FaceCount = 0;
                HasNormals = false;
                HasColors = false;
                HasUVs = false;
                HasIndices = false;
                PrimitiveType = "";
                IndexCount = 0;
            }
        }
        
        public class MaterialData
        {
            public string Type { get; set; }
            public int Color { get; set; }
            public double Opacity { get; set; }
            public bool Transparent { get; set; }
            public bool Wireframe { get; set; }
            public double LineWidth { get; set; }
            public bool VertexColors { get; set; }
            public bool Side { get; set; }
            
            public MaterialData()
            {
                Type = "";
                Color = 0xFFFFFF;
                Opacity = 1.0;
                Transparent = false;
                Wireframe = false;
                LineWidth = 1.0;
                VertexColors = false;
                Side = true;
            }
        }

        public static ShapeData Render(Shape shape)
        {
            var shapeData = new ShapeData();
            
            int colorValue = GetColorRgbByIndex(shape.Color.Index);
            var colorData = new ColorData(shape.Color.Index);
            
            double size = shape.Size;
            double scaleX = shape.RelativeXScale;
            double rotation = shape.Rotation;
            double thickness = shape.Thickness;
            
            double halfWidth = (size * scaleX) / 2.0;
            double halfHeight = size / 2.0;
            
            double centerX = shape.InsertionPoint.X;
            double centerY = shape.InsertionPoint.Y;
            double centerZ = shape.InsertionPoint.Z;
            
            var boundaryPoints = new List<Point3DData>();
            
            var corners = new[]
            {
                new { X = -halfWidth, Y = -halfHeight },
                new { X = halfWidth, Y = -halfHeight },
                new { X = halfWidth, Y = halfHeight },
                new { X = -halfWidth, Y = halfHeight }
            };
            
            double cosRot = Math.Cos(rotation);
            double sinRot = Math.Sin(rotation);
            
            foreach (var corner in corners)
            {
                double rotatedX = corner.X * cosRot - corner.Y * sinRot;
                double rotatedY = corner.X * sinRot + corner.Y * cosRot;
                
                boundaryPoints.Add(new Point3DData(
                    centerX + rotatedX,
                    centerY + rotatedY,
                    centerZ
                ));
            }
            
            double minX = boundaryPoints[0].X, maxX = boundaryPoints[0].X;
            double minY = boundaryPoints[0].Y, maxY = boundaryPoints[0].Y;
            double minZ = boundaryPoints[0].Z, maxZ = boundaryPoints[0].Z;
            
            foreach (var point in boundaryPoints)
            {
                if (point.X < minX) minX = point.X;
                if (point.X > maxX) maxX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.Y > maxY) maxY = point.Y;
                if (point.Z < minZ) minZ = point.Z;
                if (point.Z > maxZ) maxZ = point.Z;
            }
            
            var bounds = new BoundsData(
                new Point3DData(minX, minY, minZ),
                new Point3DData(maxX, maxY, maxZ)
            );
            
            var centroid = new Point3DData(
                (minX + maxX) / 2.0,
                (minY + maxY) / 2.0,
                (minZ + maxZ) / 2.0
            );
            
            var transform = new TransformData();
            transform.Position = new Point3DData(centerX, centerY, centerZ);
            transform.Rotation = new Point3DData(0, 0, rotation);
            transform.Scale = new Point3DData(size * scaleX, size, thickness > 0 ? thickness : 1);
            UpdateTransformMatrix(transform);
            
            var geometry = new GeometryData
            {
                Type = thickness > 0 ? "ExtrudeGeometry" : "BufferGeometry",
                VertexCount = boundaryPoints.Count,
                FaceCount = thickness > 0 ? 2 : 0,
                HasNormals = true,
                HasColors = true,
                HasUVs = false,
                HasIndices = true,
                PrimitiveType = thickness > 0 ? "Triangles" : "LineLoop",
                IndexCount = boundaryPoints.Count * 2
            };
            
            var material = new MaterialData
            {
                Type = thickness > 0 ? "MeshBasicMaterial" : "LineBasicMaterial",
                Color = colorValue,
                Opacity = 1.0,
                Transparent = false,
                Wireframe = true,
                LineWidth = shape.GetActiveLineWeightType().GetLineWeightValue(),
                VertexColors = false,
                Side = true
            };
            
            var vertexPositions = new List<double>();
            var vertexNormals = new List<double>();
            var vertexColors = new List<double>();
            var indices = new List<int>();
            
            double normalX = shape.Normal.X;
            double normalY = shape.Normal.Y;
            double normalZ = shape.Normal.Z;
            double normalLength = Math.Sqrt(normalX * normalX + normalY * normalY + normalZ * normalZ);
            if (normalLength > 0)
            {
                normalX /= normalLength;
                normalY /= normalLength;
                normalZ /= normalLength;
            }
            else
            {
                normalX = 0; normalY = 0; normalZ = 1;
            }
            
            for (int i = 0; i < boundaryPoints.Count; i++)
            {
                var point = boundaryPoints[i];
                vertexPositions.Add(point.X);
                vertexPositions.Add(point.Y);
                vertexPositions.Add(point.Z);
                
                vertexNormals.Add(normalX);
                vertexNormals.Add(normalY);
                vertexNormals.Add(normalZ);
                
                vertexColors.Add(colorData.R / 255.0);
                vertexColors.Add(colorData.G / 255.0);
                vertexColors.Add(colorData.B / 255.0);
                
                indices.Add(i);
                indices.Add((i + 1) % boundaryPoints.Count);
            }
            
            shapeData.InsertionPoint = new Point3DData(shape.InsertionPoint.X, shape.InsertionPoint.Y, shape.InsertionPoint.Z);
            shapeData.Size = shape.Size;
            shapeData.Rotation = shape.Rotation;
            shapeData.RelativeXScale = shape.RelativeXScale;
            shapeData.ObliqueAngle = shape.ObliqueAngle;
            shapeData.Normal = new Point3DData(shape.Normal.X, shape.Normal.Y, shape.Normal.Z);
            shapeData.ColorIndex = shape.Color.Index;
            shapeData.LineTypeName = shape.GetActiveLineType()?.Name ?? "";
            shapeData.LineWeight = shape.GetActiveLineWeightType().GetLineWeightValue();
            shapeData.LineTypeScale = shape.LineTypeScale;
            shapeData.ShapeStyleName = shape.ShapeStyle?.Name ?? "";
            shapeData.BoundaryPoints = boundaryPoints;
            shapeData.Bounds = bounds;
            shapeData.Centroid = centroid;
            shapeData.Width = size * scaleX;
            shapeData.Height = size;
            shapeData.BoundaryPointCount = boundaryPoints.Count;
            shapeData.Thickness = shape.Thickness;
            shapeData.Transform = transform;
            shapeData.Geometry = geometry;
            shapeData.Material = material;
            shapeData.Color = colorData;
            shapeData.VertexPositions = vertexPositions;
            shapeData.VertexNormals = vertexNormals;
            shapeData.VertexColors = vertexColors;
            shapeData.Indices = indices;

            return shapeData;
        }

        private static void UpdateTransformMatrix(TransformData transform)
        {
            double scaleX = transform.Scale.X;
            double scaleY = transform.Scale.Y;
            double scaleZ = transform.Scale.Z;
            double rotation = transform.Rotation.Z;
            double translateX = transform.Position.X;
            double translateY = transform.Position.Y;
            double translateZ = transform.Position.Z;

            double cos = Math.Cos(rotation);
            double sin = Math.Sin(rotation);

            transform.Matrix = new double[16] {
                scaleX * cos, scaleX * sin, 0, 0,
                -scaleY * sin, scaleY * cos, 0, 0,
                0, 0, scaleZ, 0,
                translateX, translateY, translateZ, 1
            };
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
                default: return 0x000000;
            }
        }
    }
}