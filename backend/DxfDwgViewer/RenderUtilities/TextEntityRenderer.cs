using ACadSharp.Entities;
using ACadSharp.Extensions;
using System.Collections.Generic;

namespace DxfDwgViewer.RenderUtilities
{
    public class TextEntityRenderer
    {
        public class TextData
        {
            public string Value { get; set; }
            public Point3DData InsertPoint { get; set; }
            public Point3DData AlignmentPoint { get; set; }
            public double Height { get; set; }
            public double Rotation { get; set; }
            public double ObliqueAngle { get; set; }
            public double WidthFactor { get; set; }
            public TextHorizontalAlignment HorizontalAlignment { get; set; }
            public TextVerticalAlignmentType VerticalAlignment { get; set; }
            public TextMirrorFlag Mirror { get; set; }
            public Point3DData Normal { get; set; } = new Point3DData(0, 0, 1);
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; } = "";
            public double LineWeight { get; set; }
            public double Thickness { get; set; }
            public string StyleName { get; set; } = "";
            public List<Point3DData> BoundaryPoints { get; set; } = new List<Point3DData>();
            public BoundsData Bounds { get; set; } = new BoundsData();
            public Point3DData Centroid { get; set; } = new Point3DData();
            public double Width { get; set; }
            public double Area { get; set; }
            
            public TransformData Transform { get; set; } = new TransformData();
            public GeometryData Geometry { get; set; } = new GeometryData();
            public MaterialData Material { get; set; } = new MaterialData();
            public double[] VertexPositions { get; set; } = Array.Empty<double>();
            public double[] VertexColors { get; set; } = Array.Empty<double>();
            public int[] Indices { get; set; } = Array.Empty<int>();
            public int VertexCount { get; set; }
            public string FontFamily { get; set; } = "";
            public double FontSize { get; set; }
            public string TextAlignment { get; set; } = "";
            public double Opacity { get; set; }
            public bool Transparent { get; set; }
            public ColorData Color { get; set; } = new ColorData();
            public NormalData Tangent { get; set; } = new NormalData();
            public double[] Binormal { get; set; } = Array.Empty<double>();
            public double[] UV { get; set; } = Array.Empty<double>();
            public bool IsMirrored { get; set; }
            public bool IsUpsideDown { get; set; }
            public double TextLength { get; set; }
            public double Ascent { get; set; }
            public double Descent { get; set; }
            public int CharacterCount { get; set; }
            public string FontStyle { get; set; } = "";
            public bool IsBold { get; set; }
            public bool IsItalic { get; set; }
        }

        public static TextData Render(TextEntity text)
        {
            double width = text.Value.Length * text.Height * 0.6;
            double textLength = width;
            double ascent = text.Height * 0.8;
            double descent = text.Height * 0.2;
            
            double minX, maxX, minY, maxY;
            
            switch (text.HorizontalAlignment)
            {
                case TextHorizontalAlignment.Left:
                    minX = text.InsertPoint.X;
                    maxX = text.InsertPoint.X + width;
                    break;
                case TextHorizontalAlignment.Center:
                    minX = text.InsertPoint.X - width / 2;
                    maxX = text.InsertPoint.X + width / 2;
                    break;
                case TextHorizontalAlignment.Right:
                    minX = text.InsertPoint.X - width;
                    maxX = text.InsertPoint.X;
                    break;
                default:
                    minX = text.InsertPoint.X;
                    maxX = text.InsertPoint.X + width;
                    break;
            }
            
            switch (text.VerticalAlignment)
            {
                case TextVerticalAlignmentType.Bottom:
                    minY = text.InsertPoint.Y;
                    maxY = text.InsertPoint.Y + text.Height;
                    break;
                case TextVerticalAlignmentType.Middle:
                    minY = text.InsertPoint.Y - text.Height / 2;
                    maxY = text.InsertPoint.Y + text.Height / 2;
                    break;
                case TextVerticalAlignmentType.Top:
                    minY = text.InsertPoint.Y - text.Height;
                    maxY = text.InsertPoint.Y;
                    break;
                default:
                    minY = text.InsertPoint.Y;
                    maxY = text.InsertPoint.Y + text.Height;
                    break;
            }
            
            var bounds = new BoundsData(
                new Point3DData(minX, minY, text.InsertPoint.Z),
                new Point3DData(maxX, maxY, text.InsertPoint.Z)
            );
            
            var centroid = new Point3DData(
                (minX + maxX) / 2.0,
                (minY + maxY) / 2.0,
                text.InsertPoint.Z
            );
            
            double area = width * text.Height;

            int colorValue = GetColorRgbByIndex(text.Color.Index);
            
            var transform = new TransformData();
            transform.Position = centroid ?? new Point3DData(text.InsertPoint.X, text.InsertPoint.Y, text.InsertPoint.Z);
            transform.Rotation = new Point3DData(0, 0, text.Rotation);
            transform.Scale = new Point3DData(1, 1, 1);
            
            UpdateTransformMatrix(transform, text.Rotation);
            
            var geometry = new GeometryData
            {
                Type = "TextGeometry",
                VertexCount = 4,
                FaceCount = 2,
                HasNormals = true,
                HasColors = true,
                HasUVs = true,
                HasIndices = true,
                PrimitiveType = "Triangles",
                IndexCount = 6,
                IsClosed = false,
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
            if (bounds != null)
            {
                vertexPositions.Add(bounds.Min.X);
                vertexPositions.Add(bounds.Min.Y);
                vertexPositions.Add(bounds.Min.Z);
                
                vertexPositions.Add(bounds.Max.X);
                vertexPositions.Add(bounds.Min.Y);
                vertexPositions.Add(bounds.Min.Z);
                
                vertexPositions.Add(bounds.Max.X);
                vertexPositions.Add(bounds.Max.Y);
                vertexPositions.Add(bounds.Min.Z);
                
                vertexPositions.Add(bounds.Min.X);
                vertexPositions.Add(bounds.Max.Y);
                vertexPositions.Add(bounds.Min.Z);
            }
            else
            {
                double halfWidth = text.Value.Length * text.Height * 0.3;
                double halfHeight = text.Height * 0.5;
                
                vertexPositions.Add(text.InsertPoint.X - halfWidth);
                vertexPositions.Add(text.InsertPoint.Y - halfHeight);
                vertexPositions.Add(text.InsertPoint.Z);
                
                vertexPositions.Add(text.InsertPoint.X + halfWidth);
                vertexPositions.Add(text.InsertPoint.Y - halfHeight);
                vertexPositions.Add(text.InsertPoint.Z);
                
                vertexPositions.Add(text.InsertPoint.X + halfWidth);
                vertexPositions.Add(text.InsertPoint.Y + halfHeight);
                vertexPositions.Add(text.InsertPoint.Z);
                
                vertexPositions.Add(text.InsertPoint.X - halfWidth);
                vertexPositions.Add(text.InsertPoint.Y + halfHeight);
                vertexPositions.Add(text.InsertPoint.Z);
            }
            
            var vertexColors = new List<double>();
            for (int i = 0; i < 4; i++)
            {
                vertexColors.Add((colorValue >> 16) & 0xFF);
                vertexColors.Add((colorValue >> 8) & 0xFF);
                vertexColors.Add(colorValue & 0xFF);
            }
            
            var indices = new List<int> { 0, 1, 2, 0, 2, 3 };
            
            var uv = new List<double>();
            uv.Add(0); uv.Add(0);
            uv.Add(1); uv.Add(0);
            uv.Add(1); uv.Add(1);
            uv.Add(0); uv.Add(1);
            
            string textAlignment = $"{text.HorizontalAlignment}-{text.VerticalAlignment}";
            
            bool isMirrored = text.Mirror != TextMirrorFlag.None;
            bool isUpsideDown = (text.Rotation > Math.PI / 2 && text.Rotation < 3 * Math.PI / 2);
            
            var tangent = new NormalData(Math.Cos(text.Rotation), Math.Sin(text.Rotation), 0);
            var binormal = new NormalData(-Math.Sin(text.Rotation), Math.Cos(text.Rotation), 0).ToArray();
            
            var colorData = new ColorData(text.Color.Index);
            
            var textData = new TextData
            {
                Value = text.Value,
                InsertPoint = new Point3DData(text.InsertPoint.X, text.InsertPoint.Y, text.InsertPoint.Z),
                AlignmentPoint = new Point3DData(text.AlignmentPoint.X, text.AlignmentPoint.Y, text.AlignmentPoint.Z),
                Height = text.Height,
                Rotation = text.Rotation,
                ObliqueAngle = text.ObliqueAngle,
                WidthFactor = text.WidthFactor,
                HorizontalAlignment = text.HorizontalAlignment,
                VerticalAlignment = text.VerticalAlignment,
                Mirror = text.Mirror,
                Normal = new Point3DData(text.Normal.X, text.Normal.Y, text.Normal.Z),
                ColorIndex = text.Color.Index,
                LineTypeName = text.GetActiveLineType()?.Name ?? "",
                LineWeight = text.GetActiveLineWeightType().GetLineWeightValue(),
                Thickness = text.Thickness,
                StyleName = text.Style?.Name ?? "",
                BoundaryPoints = new List<Point3DData>(),
                Bounds = bounds!,
                Centroid = centroid!,
                Width = width,
                Area = area,
                Transform = transform,
                Geometry = geometry,
                Material = material,
                VertexPositions = vertexPositions.ToArray(),
                VertexColors = vertexColors.ToArray(),
                Indices = indices.ToArray(),
                VertexCount = 4,
                FontFamily = text.Style?.Name ?? "Arial",
                FontSize = text.Height,
                TextAlignment = textAlignment,
                Opacity = 1.0,
                Transparent = false,
                Color = colorData,
                Tangent = tangent,
                Binormal = binormal,
                UV = uv.ToArray(),
                IsMirrored = isMirrored,
                IsUpsideDown = isUpsideDown,
                TextLength = textLength,
                Ascent = ascent,
                Descent = descent,
                CharacterCount = text.Value.Length,
                FontStyle = "Regular",
                IsBold = false,
                IsItalic = false
            };

            return textData;
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
        
        private static void UpdateTransformMatrix(TransformData transform, double rotation)
        {
            double cos = Math.Cos(rotation);
            double sin = Math.Sin(rotation);
            
            transform.Matrix = new double[16]
            {
                cos, sin, 0, transform.Position.X,
                -sin, cos, 0, transform.Position.Y,
                0, 0, 1, transform.Position.Z,
                0, 0, 0, 1
            };
        }
    }
}
