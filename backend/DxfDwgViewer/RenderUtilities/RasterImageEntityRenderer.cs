using System;
using ACadSharp.Entities;
using System.Collections.Generic;
using ACadSharp.Extensions;

namespace DxfDwgViewer.RenderUtilities
{
    public class RasterImageEntityRenderer
    {
        public class RasterImageData
        {
            public string EntityType { get; set; } = "RasterImage";
            public List<Point3DData> CornerPoints { get; set; }
            public Point3DData InsertPoint { get; set; }
            public Point3DData UVector { get; set; }
            public Point3DData VVector { get; set; }
            public Point3DData Size { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            public byte Brightness { get; set; }
            public byte Contrast { get; set; }
            public byte Fade { get; set; }
            public ImageDisplayFlags Flags { get; set; }
            public ClipType ClipType { get; set; }
            public bool ClippingState { get; set; }
            public string DefinitionFileName { get; set; }
            public Point3DData DefinitionSize { get; set; }
            
            public BoundsData Bounds { get; set; }
            public Point3DData Centroid { get; set; }
            public double Area { get; set; }
            
            public TransformData Transform { get; set; }
            public GeometryData Geometry { get; set; }
            public MaterialData Material { get; set; }
            public TextureData Texture { get; set; }
            public double[] VertexPositions { get; set; }
            public double[] UVCoordinates { get; set; }
            public int[] Indices { get; set; }
        }
        
        public class TextureData
        {
            public string ImagePath { get; set; }
            public Point3DData Offset { get; set; }
            public Point3DData Repeat { get; set; }
            public bool FlipY { get; set; }
            public double Rotation { get; set; }
            
            public TextureData()
            {
                ImagePath = "";
                Offset = new Point3DData { X = 0, Y = 0, Z = 0 };
                Repeat = new Point3DData { X = 1, Y = 1, Z = 1 };
                FlipY = true;
                Rotation = 0;
            }
        }

        public static RasterImageData Render(RasterImage rasterImage)
        {
            // 计算角点
            double insertX = rasterImage.InsertPoint.X;
            double insertY = rasterImage.InsertPoint.Y;

            // 获取U向量和V向量确定图像范围
            double uX = rasterImage.UVector.X * rasterImage.Size.X;
            double uY = rasterImage.UVector.Y * rasterImage.Size.X;
            double vX = rasterImage.VVector.X * rasterImage.Size.Y;
            double vY = rasterImage.VVector.Y * rasterImage.Size.Y;

            // 计算图像四个角点
            var corners = new Point3DData[]
            {
                new Point3DData { X = insertX, Y = insertY, Z = rasterImage.InsertPoint.Z },
                new Point3DData { X = insertX + uX, Y = insertY + uY, Z = rasterImage.InsertPoint.Z },
                new Point3DData { X = insertX + uX + vX, Y = insertY + uY + vY, Z = rasterImage.InsertPoint.Z },
                new Point3DData { X = insertX + vX, Y = insertY + vY, Z = rasterImage.InsertPoint.Z }
            };

            // 计算边界框和质心
            double minX = corners[0].X, maxX = corners[0].X;
            double minY = corners[0].Y, maxY = corners[0].Y;
            
            foreach (var corner in corners)
            {
                if (corner.X < minX) minX = corner.X;
                if (corner.X > maxX) maxX = corner.X;
                if (corner.Y < minY) minY = corner.Y;
                if (corner.Y > maxY) maxY = corner.Y;
            }
            
            BoundsData bounds = new BoundsData(
                new Point3DData(minX, minY, rasterImage.InsertPoint.Z),
                new Point3DData(maxX, maxY, rasterImage.InsertPoint.Z)
            );
            
            Point3DData centroid = new Point3DData
            {
                X = (minX + maxX) / 2.0,
                Y = (minY + maxY) / 2.0,
                Z = rasterImage.InsertPoint.Z
            };
            
            // 计算面积（使用鞋带公式）
            double area = 0;
            for (int i = 0; i < corners.Length; i++)
            {
                int j = (i + 1) % corners.Length;
                area += corners[i].X * corners[j].Y;
                area -= corners[j].X * corners[i].Y;
            }
            area = Math.Abs(area) / 2.0;

            var rasterImageData = new RasterImageData
            {
                CornerPoints = new List<Point3DData>(),
                InsertPoint = new Point3DData { X = rasterImage.InsertPoint.X, Y = rasterImage.InsertPoint.Y, Z = rasterImage.InsertPoint.Z },
                UVector = new Point3DData { X = rasterImage.UVector.X, Y = rasterImage.UVector.Y, Z = rasterImage.UVector.Z },
                VVector = new Point3DData { X = rasterImage.VVector.X, Y = rasterImage.VVector.Y, Z = rasterImage.VVector.Z },
                Size = new Point3DData { X = rasterImage.Size.X, Y = rasterImage.Size.Y },
                ColorIndex = rasterImage.Color.Index,
                LineTypeName = rasterImage.GetActiveLineType()?.Name ?? "",
                LineWeight = rasterImage.GetActiveLineWeightType().GetLineWeightValue(),
                Brightness = rasterImage.Brightness,
                Contrast = rasterImage.Contrast,
                Fade = rasterImage.Fade,
                Flags = rasterImage.Flags,
                ClipType = rasterImage.ClipType,
                ClippingState = rasterImage.ClippingState,
                DefinitionFileName = rasterImage.Definition != null ? rasterImage.Definition.FileName : "",
                DefinitionSize = rasterImage.Definition != null ? new Point3DData { X = rasterImage.Definition.Size.X, Y = rasterImage.Definition.Size.Y } : new Point3DData(),
                Bounds = bounds,
                Centroid = centroid,
                Area = area,
                
                Transform = new TransformData(),
                Geometry = new GeometryData(),
                Material = new MaterialData(),
                Texture = new TextureData(),
                VertexPositions = new double[12],
                UVCoordinates = new double[8],
                Indices = new int[6]
            };
            
            rasterImageData.Transform.Position = new Point3DData { X = centroid.X, Y = centroid.Y, Z = rasterImage.InsertPoint.Z };
            
            double uLength = Math.Sqrt(rasterImage.UVector.X * rasterImage.UVector.X + rasterImage.UVector.Y * rasterImage.UVector.Y);
            if (uLength > 0.0001)
            {
                double cosAngle = rasterImage.UVector.X / uLength;
                double angle = Math.Acos(Math.Max(-1, Math.Min(1, cosAngle)));
                if (rasterImage.UVector.Y < 0) angle = -angle;
                rasterImageData.Transform.Rotation = new Point3DData { X = 0, Y = 0, Z = angle };
            }
            
            rasterImageData.Transform.Scale = new Point3DData 
            { 
                X = rasterImage.Size.X, 
                Y = rasterImage.Size.Y, 
                Z = 1 
            };
            
            UpdateTransformMatrix(rasterImageData.Transform);
            
            rasterImageData.Geometry.Type = "BufferGeometry";
            rasterImageData.Geometry.VertexCount = 4;
            rasterImageData.Geometry.FaceCount = 2;
            rasterImageData.Geometry.HasNormals = false;
            rasterImageData.Geometry.HasColors = false;
            rasterImageData.Geometry.HasUVs = true;
            rasterImageData.Geometry.HasIndices = true;
            rasterImageData.Geometry.PrimitiveType = "Triangles";
            rasterImageData.Geometry.IndexCount = 6;
            rasterImageData.Geometry.IsClosed = false;
            rasterImageData.Geometry.IsPeriodic = false;
            rasterImageData.Geometry.Degree = 0;
            
            rasterImageData.Material.Type = "MeshBasicMaterial";
            rasterImageData.Material.Color = GetColorByIndex(rasterImage.Color.Index);
            rasterImageData.Material.Opacity = rasterImage.Fade / 255.0;
            rasterImageData.Material.Transparent = rasterImage.Fade < 255;
            rasterImageData.Material.Side = true;
            
            rasterImageData.Texture.ImagePath = rasterImageData.DefinitionFileName;
            rasterImageData.Texture.FlipY = true;
            
            rasterImageData.VertexPositions[0] = corners[0].X;
            rasterImageData.VertexPositions[1] = corners[0].Y;
            rasterImageData.VertexPositions[2] = rasterImage.InsertPoint.Z;
            
            rasterImageData.VertexPositions[3] = corners[1].X;
            rasterImageData.VertexPositions[4] = corners[1].Y;
            rasterImageData.VertexPositions[5] = rasterImage.InsertPoint.Z;
            
            rasterImageData.VertexPositions[6] = corners[2].X;
            rasterImageData.VertexPositions[7] = corners[2].Y;
            rasterImageData.VertexPositions[8] = rasterImage.InsertPoint.Z;
            
            rasterImageData.VertexPositions[9] = corners[3].X;
            rasterImageData.VertexPositions[10] = corners[3].Y;
            rasterImageData.VertexPositions[11] = rasterImage.InsertPoint.Z;
            
            rasterImageData.UVCoordinates[0] = 0;
            rasterImageData.UVCoordinates[1] = 0;
            
            rasterImageData.UVCoordinates[2] = 1;
            rasterImageData.UVCoordinates[3] = 0;
            
            rasterImageData.UVCoordinates[4] = 1;
            rasterImageData.UVCoordinates[5] = 1;
            
            rasterImageData.UVCoordinates[6] = 0;
            rasterImageData.UVCoordinates[7] = 1;
            
            rasterImageData.Indices[0] = 0;
            rasterImageData.Indices[1] = 1;
            rasterImageData.Indices[2] = 2;
            
            rasterImageData.Indices[3] = 0;
            rasterImageData.Indices[4] = 2;
            rasterImageData.Indices[5] = 3;

            foreach (var corner in corners)
            {
                rasterImageData.CornerPoints.Add(new Point3DData { X = corner.X, Y = corner.Y, Z = corner.Z });
            }

            return rasterImageData;
        }
        
        private static void UpdateTransformMatrix(TransformData transform)
        {
            double cosX = Math.Cos(transform.Rotation.X);
            double sinX = Math.Sin(transform.Rotation.X);
            double cosY = Math.Cos(transform.Rotation.Y);
            double sinY = Math.Sin(transform.Rotation.Y);
            double cosZ = Math.Cos(transform.Rotation.Z);
            double sinZ = Math.Sin(transform.Rotation.Z);
            
            transform.Matrix = new double[16] { 
                cosZ * transform.Scale.X, -sinZ * transform.Scale.Y, 0, 0,
                sinZ * transform.Scale.X, cosZ * transform.Scale.Y, 0, 0,
                0, 0, transform.Scale.Z, 0,
                transform.Position.X, transform.Position.Y, transform.Position.Z, 1
            };
        }
        
        /// <summary>
        /// 根据颜色索引获取颜色值
        /// </summary>
        /// <param name="colorIndex">颜色索引</param>
        /// <returns>颜色值（RGB格式）</returns>
        private static int GetColorByIndex(short colorIndex)
        {
            // ACI colors (AutoCAD Color Index)
            switch (colorIndex)
            {
                case 1: return 0xff0000; // Red
                case 2: return 0xffff00; // Yellow
                case 3: return 0x00ff00; // Green
                case 4: return 0x00ffff; // Cyan
                case 5: return 0x0000ff; // Blue
                case 6: return 0xff00ff; // Magenta
                case 7: return 0xffffff; // White
                default: return 0x000000; // Black
            }
        }
    }
}