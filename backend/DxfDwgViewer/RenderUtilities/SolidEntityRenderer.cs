using ACadSharp.Entities;
using System.Collections.Generic;
using ACadSharp.Extensions;
using System.Linq;
using CSMath;

namespace DxfDwgViewer.RenderUtilities
{
    public class SolidEntityRenderer
    {
        public class SolidData
        {
            public List<Point3DData> Points { get; set; } = new List<Point3DData>();
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; } = "";
            public double LineWeight { get; set; }
            public double LineTypeScale { get; set; }
            public Point3DData Normal { get; set; } = new Point3DData();
            public double Thickness { get; set; }
            public bool HasFourthCorner { get; set; }
            public BoundsData Bounds { get; set; } = new BoundsData();
            public Point3DData Centroid { get; set; } = new Point3DData();
            public double Area { get; set; }
            public double Perimeter { get; set; }
            
            public TransformData Transform { get; set; } = new TransformData();
            public GeometryData Geometry { get; set; } = new GeometryData();
            public MaterialData Material { get; set; } = new MaterialData();
            public ColorData Color { get; set; } = new ColorData();
            public List<double> VertexPositions { get; set; } = new List<double>();
            public List<double> VertexNormals { get; set; } = new List<double>();
            public List<double> VertexColors { get; set; } = new List<double>();
            public List<double> VertexUVs { get; set; } = new List<double>();
            public List<int> Indices { get; set; } = new List<int>();
            public bool IsTriangle { get; set; }
            public Point3DData FirstCorner { get; set; } = new Point3DData();
            public Point3DData SecondCorner { get; set; } = new Point3DData();
            public Point3DData ThirdCorner { get; set; } = new Point3DData();
            public Point3DData FourthCorner { get; set; } = new Point3DData();
            
            public int VertexCount { get; set; }
            public int FaceCount { get; set; }
            public bool IsFilled { get; set; }
            public bool IsExtruded { get; set; }
            public double ExtrusionDepth { get; set; }
        }

        public static SolidData Render(Solid solid)
        {
            var solidData = new SolidData();
            
            int colorValue = GetColorRgbByIndex(solid.Color.Index);
            var colorData = new ColorData(solid.Color.Index);
            
            bool hasFourthCorner = !solid.ThirdCorner.Equals(solid.FourthCorner);
            bool isTriangle = !hasFourthCorner;
            bool isExtruded = solid.Thickness > 0;
            
            var points = ConstructPoints(solid);
            
            var points3D = new List<Point3DData>();
            foreach (var point in points)
            {
                points3D.Add(new Point3DData(point.X, point.Y, solid.FirstCorner.Z));
            }
            
            var minX = points[0].X;
            var maxX = points[0].X;
            var minY = points[0].Y;
            var maxY = points[0].Y;
            var minZ = solid.FirstCorner.Z;
            var maxZ = isExtruded ? solid.FirstCorner.Z + solid.Thickness : solid.FirstCorner.Z;
            
            foreach (var point in points)
            {
                if (point.X < minX) minX = point.X;
                if (point.X > maxX) maxX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.Y > maxY) maxY = point.Y;
            }
            
            var bounds = new BoundsData(
                new Point3DData(minX, minY, minZ),
                new Point3DData(maxX, maxY, maxZ)
            );
            
            double sumX = 0, sumY = 0, sumZ = 0;
            foreach (var point in points3D)
            {
                sumX += point.X;
                sumY += point.Y;
                sumZ += point.Z;
            }
            
            var centroid = new Point3DData(
                sumX / points3D.Count,
                sumY / points3D.Count,
                sumZ / points3D.Count
            );
            
            double area = 0;
            for (int i = 0; i < points.Length; i++)
            {
                int j = (i + 1) % points.Length;
                area += points[i].X * points[j].Y;
                area -= points[j].X * points[i].Y;
            }
            area = Math.Abs(area) / 2.0;
            
            double perimeter = 0;
            for (int i = 0; i < points.Length; i++)
            {
                int j = (i + 1) % points.Length;
                double dx = points[j].X - points[i].X;
                double dy = points[j].Y - points[i].Y;
                perimeter += Math.Sqrt(dx * dx + dy * dy);
            }
            
            var transform = new TransformData
            {
                Position = centroid,
                Rotation = new Point3DData(0, 0, 0),
                Scale = new Point3DData(1, 1, isExtruded ? solid.Thickness : 1),
                Matrix = new double[16]
            };
            
            UpdateTransformMatrix(transform);
            
            var geometry = new GeometryData
            {
                Type = isExtruded ? "ExtrudeGeometry" : "BufferGeometry",
                VertexCount = points3D.Count,
                FaceCount = isTriangle ? 1 : 2,
                HasNormals = true,
                HasColors = true,
                HasUVs = true,
                HasIndices = true,
                PrimitiveType = isExtruded ? "Triangles" : "TriangleFan",
                IndexCount = isTriangle ? 3 : 6
            };
            
            var material = new MaterialData
            {
                Type = isExtruded ? "MeshBasicMaterial" : "MeshBasicMaterial",
                Color = colorValue,
                Opacity = 1.0,
                Transparent = false,
                Wireframe = false,
                LineWidth = solid.GetActiveLineWeightType().GetLineWeightValue(),
                Side = true,
                VertexColors = false
            };
            
            var vertexPositions = new List<double>();
            foreach (var point in points3D)
            {
                vertexPositions.Add(point.X);
                vertexPositions.Add(point.Y);
                vertexPositions.Add(point.Z);
            }
            
            var faceNormals = new List<Point3DData>();
            if (isTriangle)
            {
                var v0 = points3D[0];
                var v1 = points3D[1];
                var v2 = points3D[2];
                
                var edge1 = new Point3DData(v1.X - v0.X, v1.Y - v0.Y, v1.Z - v0.Z);
                var edge2 = new Point3DData(v2.X - v0.X, v2.Y - v0.Y, v2.Z - v0.Z);
                
                var nx = edge1.Y * edge2.Z - edge1.Z * edge2.Y;
                var ny = edge1.Z * edge2.X - edge1.X * edge2.Z;
                var nz = edge1.X * edge2.Y - edge1.Y * edge2.X;
                
                var length = Math.Sqrt(nx * nx + ny * ny + nz * nz);
                if (length > 0)
                {
                    faceNormals.Add(new Point3DData(nx / length, ny / length, nz / length));
                }
                else
                {
                    faceNormals.Add(new Point3DData(0, 0, 1));
                }
            }
            else
            {
                var v0 = points3D[0];
                var v1 = points3D[1];
                var v2 = points3D[2];
                
                var edge1 = new Point3DData(v1.X - v0.X, v1.Y - v0.Y, v1.Z - v0.Z);
                var edge2 = new Point3DData(v2.X - v0.X, v2.Y - v0.Y, v2.Z - v0.Z);
                
                var nx = edge1.Y * edge2.Z - edge1.Z * edge2.Y;
                var ny = edge1.Z * edge2.X - edge1.X * edge2.Z;
                var nz = edge1.X * edge2.Y - edge1.Y * edge2.X;
                
                var length = Math.Sqrt(nx * nx + ny * ny + nz * nz);
                Point3DData normal;
                if (length > 0)
                {
                    normal = new Point3DData(nx / length, ny / length, nz / length);
                }
                else
                {
                    normal = new Point3DData(0, 0, 1);
                }
                
                faceNormals.Add(normal);
                faceNormals.Add(normal);
            }
            
            var vertexNormals = new List<double>();
            for (int i = 0; i < points3D.Count; i++)
            {
                var avgNormal = new Point3DData(0, 0, 0);
                int count = 0;
                
                if (isTriangle)
                {
                    avgNormal = faceNormals[0];
                    count = 1;
                }
                else
                {
                    avgNormal = faceNormals[0];
                    count = 2;
                }
                
                if (count > 0)
                {
                    vertexNormals.Add(avgNormal.X / count);
                    vertexNormals.Add(avgNormal.Y / count);
                    vertexNormals.Add(avgNormal.Z / count);
                }
                else
                {
                    vertexNormals.Add(0);
                    vertexNormals.Add(0);
                    vertexNormals.Add(1);
                }
            }
            
            var vertexColors = new List<double>();
            for (int i = 0; i < points3D.Count; i++)
            {
                vertexColors.Add(colorData.R / 255.0);
                vertexColors.Add(colorData.G / 255.0);
                vertexColors.Add(colorData.B / 255.0);
            }
            
            var vertexUVs = new List<double>();
            double width = maxX - minX;
            double height = maxY - minY;
            if (width > 0 && height > 0)
            {
                foreach (var point in points3D)
                {
                    vertexUVs.Add((point.X - minX) / width);
                    vertexUVs.Add((point.Y - minY) / height);
                }
            }
            else
            {
                for (int i = 0; i < points3D.Count; i++)
                {
                    vertexUVs.Add(0);
                    vertexUVs.Add(0);
                }
            }
            
            var indices = new List<int>();
            if (isTriangle)
            {
                indices.Add(0);
                indices.Add(1);
                indices.Add(2);
            }
            else
            {
                indices.Add(0);
                indices.Add(1);
                indices.Add(2);
                
                indices.Add(0);
                indices.Add(2);
                indices.Add(3);
            }

            solidData.Points = points3D;
            solidData.ColorIndex = solid.Color.Index;
            solidData.LineTypeName = solid.GetActiveLineType()?.Name ?? "";
            solidData.LineWeight = solid.GetActiveLineWeightType().GetLineWeightValue();
            solidData.LineTypeScale = 1.0;
            solidData.Normal = new Point3DData(solid.Normal.X, solid.Normal.Y, solid.Normal.Z);
            solidData.Thickness = solid.Thickness;
            solidData.HasFourthCorner = hasFourthCorner;
            solidData.Bounds = bounds;
            solidData.Centroid = centroid;
            solidData.Area = area;
            solidData.Perimeter = perimeter;
            solidData.Transform = transform;
            solidData.Geometry = geometry;
            solidData.Material = material;
            solidData.Color = colorData;
            solidData.VertexPositions = vertexPositions;
            solidData.VertexNormals = vertexNormals;
            solidData.VertexColors = vertexColors;
            solidData.VertexUVs = vertexUVs;
            solidData.Indices = indices;
            solidData.IsTriangle = isTriangle;
            solidData.FirstCorner = new Point3DData(solid.FirstCorner.X, solid.FirstCorner.Y, solid.FirstCorner.Z);
            solidData.SecondCorner = new Point3DData(solid.SecondCorner.X, solid.SecondCorner.Y, solid.SecondCorner.Z);
            solidData.ThirdCorner = new Point3DData(solid.ThirdCorner.X, solid.ThirdCorner.Y, solid.ThirdCorner.Z);
            solidData.FourthCorner = new Point3DData(solid.FourthCorner.X, solid.FourthCorner.Y, solid.FourthCorner.Z);
            solidData.VertexCount = points3D.Count;
            solidData.FaceCount = isTriangle ? 1 : 2;
            solidData.IsFilled = true;
            solidData.IsExtruded = isExtruded;
            solidData.ExtrusionDepth = solid.Thickness;

            return solidData;
        }

        /// <summary>
        /// 构造Solid实体的点数组，按照AutoCAD的标准顺序处理点
        /// AutoCAD中SOLID实体的点存储顺序特殊，最后两个点是颠倒的
        /// 点顺序为: 1, 2, 4, 3 (而不是 1, 2, 3, 4)
        /// </summary>
        /// <param name="solid">Solid实体</param>
        /// <returns>点数组</returns>
        private static PointData[] ConstructPoints(Solid solid)
        {
            // 检查是否为三点情况（第四点与第三点相同）
            bool isTriangle = solid.ThirdCorner.Equals(solid.FourthCorner);
            
            if (isTriangle)
            {
                // 三点情况 - 构造三角形
                // AutoCAD中三角形的点顺序为: 1, 2, 3
                return new PointData[] {
                    new PointData(solid.FirstCorner.X, solid.FirstCorner.Y),
                    new PointData(solid.SecondCorner.X, solid.SecondCorner.Y),
                    new PointData(solid.ThirdCorner.X, solid.ThirdCorner.Y)
                };
            }
            else
            {
                // 四点情况 - 构造四边形
                // AutoCAD中四边形的点顺序为: 1, 2, 4, 3 (注意3和4是颠倒的)
                return new PointData[] {
                    new PointData(solid.FirstCorner.X, solid.FirstCorner.Y),
                    new PointData(solid.SecondCorner.X, solid.SecondCorner.Y),
                    new PointData(solid.FourthCorner.X, solid.FourthCorner.Y),
                    new PointData(solid.ThirdCorner.X, solid.ThirdCorner.Y)
                };
            }
        }

        /// <summary>
        /// 更新变换矩阵
        /// </summary>
        /// <param name="transform">变换数据</param>
        private static void UpdateTransformMatrix(TransformData transform)
        {
            // 创建4x4单位矩阵
            double[] matrix = new double[16];
            
            // 初始化为单位矩阵
            matrix[0] = 1; matrix[5] = 1; matrix[10] = 1; matrix[15] = 1;
            
            // 应用缩放
            matrix[0] *= transform.Scale.X;
            matrix[5] *= transform.Scale.Y;
            matrix[10] *= transform.Scale.Z;
            
            // 应用旋转（这里简化处理，只考虑Z轴旋转）
            double cosZ = Math.Cos(transform.Rotation.Z);
            double sinZ = Math.Sin(transform.Rotation.Z);
            double m00 = matrix[0], m01 = matrix[4], m10 = matrix[1], m11 = matrix[5];
            
            matrix[0] = m00 * cosZ - m01 * sinZ;
            matrix[4] = m00 * sinZ + m01 * cosZ;
            matrix[1] = m10 * cosZ - m11 * sinZ;
            matrix[5] = m10 * sinZ + m11 * cosZ;
            
            // 应用平移
            matrix[12] = transform.Position.X;
            matrix[13] = transform.Position.Y;
            matrix[14] = transform.Position.Z;
            
            transform.Matrix = matrix;
        }
        
        /// <summary>
        /// 根据颜色索引获取RGB颜色值
        /// </summary>
        /// <param name="colorIndex">颜色索引</param>
        /// <returns>RGB颜色值</returns>
        private static int GetColorRgbByIndex(short colorIndex)
        {
            // ACI colors (AutoCAD Color Index) to RGB conversion
            switch (colorIndex)
            {
                case 1: return 0xFF0000; // Red
                case 2: return 0xFFFF00; // Yellow
                case 3: return 0x00FF00; // Green
                case 4: return 0x00FFFF; // Cyan
                case 5: return 0x0000FF; // Blue
                case 6: return 0xFF00FF; // Magenta
                case 7: return 0xFFFFFF; // White
                case 8: return 0x808080; // Dark Gray
                case 9: return 0x404040; // Light Gray
                case 10: return 0xFF0000; // Red
                case 11: return 0xFF7F00; // Orange
                case 12: return 0xFFFF00; // Yellow
                case 13: return 0x7FFF00; // Yellow-Green
                case 14: return 0x00FF00; // Green
                case 15: return 0x00FF7F; // Green-Cyan
                case 16: return 0x00FFFF; // Cyan
                case 17: return 0x007FFF; // Cyan-Blue
                case 18: return 0x0000FF; // Blue
                case 19: return 0x7F00FF; // Blue-Magenta
                case 20: return 0xFF00FF; // Magenta
                case 21: return 0xFF007F; // Magenta-Red
                case 22: return 0x7F0000; // Dark Red
                case 23: return 0x7F3F00; // Dark Orange
                case 24: return 0x7F7F00; // Dark Yellow
                case 25: return 0x3F7F00; // Dark Yellow-Green
                case 26: return 0x007F00; // Dark Green
                case 27: return 0x007F3F; // Dark Green-Cyan
                case 28: return 0x007F7F; // Dark Cyan
                case 29: return 0x003F7F; // Dark Cyan-Blue
                case 30: return 0x00007F; // Dark Blue
                case 31: return 0x3F007F; // Dark Blue-Magenta
                case 32: return 0x7F007F; // Dark Magenta
                case 33: return 0x7F003F; // Dark Magenta-Red
                case 34: return 0x333333; // Dark Gray
                case 35: return 0x4F4F4F; // Light Gray
                case 36: return 0x666666; // Medium Gray
                case 37: return 0x7F7F7F; // Gray
                case 38: return 0x999999; // Light Gray
                case 39: return 0xB3B3B3; // Light Gray
                case 40: return 0xCCCCCC; // Light Gray
                case 41: return 0xE6E6E6; // Very Light Gray
                case 42: return 0xFF0000; // Red
                case 43: return 0xFF7F00; // Orange
                case 44: return 0xFFFF00; // Yellow
                case 45: return 0x7FFF00; // Yellow-Green
                case 46: return 0x00FF00; // Green
                case 47: return 0x00FF7F; // Green-Cyan
                case 48: return 0x00FFFF; // Cyan
                case 49: return 0x007FFF; // Cyan-Blue
                case 50: return 0x0000FF; // Blue
                case 51: return 0x7F00FF; // Blue-Magenta
                case 52: return 0xFF00FF; // Magenta
                case 53: return 0xFF007F; // Magenta-Red
                case 54: return 0x7F0000; // Dark Red
                case 55: return 0x7F3F00; // Dark Orange
                case 56: return 0x7F7F00; // Dark Yellow
                case 57: return 0x3F7F00; // Dark Yellow-Green
                case 58: return 0x007F00; // Dark Green
                case 59: return 0x007F3F; // Dark Green-Cyan
                case 60: return 0x007F7F; // Dark Cyan
                case 61: return 0x003F7F; // Dark Cyan-Blue
                case 62: return 0x00007F; // Dark Blue
                case 63: return 0x3F007F; // Dark Blue-Magenta
                case 64: return 0x7F007F; // Dark Magenta
                case 65: return 0x7F003F; // Dark Magenta-Red
                case 66: return 0x4C4C4C; // Dark Gray
                case 67: return 0x666666; // Medium Gray
                case 68: return 0x7F7F7F; // Gray
                case 69: return 0x999999; // Light Gray
                case 70: return 0xB3B3B3; // Light Gray
                case 71: return 0xCCCCCC; // Light Gray
                case 72: return 0xE6E6E6; // Very Light Gray
                case 73: return 0xFF0000; // Red
                case 74: return 0xFF7F00; // Orange
                case 75: return 0xFFFF00; // Yellow
                case 76: return 0x7FFF00; // Yellow-Green
                case 77: return 0x00FF00; // Green
                case 78: return 0x00FF7F; // Green-Cyan
                case 79: return 0x00FFFF; // Cyan
                case 80: return 0x007FFF; // Cyan-Blue
                case 81: return 0x0000FF; // Blue
                case 82: return 0x7F00FF; // Blue-Magenta
                case 83: return 0xFF00FF; // Magenta
                case 84: return 0xFF007F; // Magenta-Red
                case 85: return 0x7F0000; // Dark Red
                case 86: return 0x7F3F00; // Dark Orange
                case 87: return 0x7F7F00; // Dark Yellow
                case 88: return 0x3F7F00; // Dark Yellow-Green
                case 89: return 0x007F00; // Dark Green
                case 90: return 0x007F3F; // Dark Green-Cyan
                case 91: return 0x007F7F; // Dark Cyan
                case 92: return 0x003F7F; // Dark Cyan-Blue
                case 93: return 0x00007F; // Dark Blue
                case 94: return 0x3F007F; // Dark Blue-Magenta
                case 95: return 0x7F007F; // Dark Magenta
                case 96: return 0x7F003F; // Dark Magenta-Red
                case 97: return 0x4C4C4C; // Dark Gray
                case 98: return 0x666666; // Medium Gray
                case 99: return 0x7F7F7F; // Gray
                case 100: return 0x999999; // Light Gray
                case 101: return 0xB3B3B3; // Light Gray
                case 102: return 0xCCCCCC; // Light Gray
                case 103: return 0xE6E6E6; // Very Light Gray
                case 104: return 0xFF0000; // Red
                case 105: return 0xFF7F00; // Orange
                case 106: return 0xFFFF00; // Yellow
                case 107: return 0x7FFF00; // Yellow-Green
                case 108: return 0x00FF00; // Green
                case 109: return 0x00FF7F; // Green-Cyan
                case 110: return 0x00FFFF; // Cyan
                case 111: return 0x007FFF; // Cyan-Blue
                case 112: return 0x0000FF; // Blue
                case 113: return 0x7F00FF; // Blue-Magenta
                case 114: return 0xFF00FF; // Magenta
                case 115: return 0xFF007F; // Magenta-Red
                case 116: return 0x7F0000; // Dark Red
                case 117: return 0x7F3F00; // Dark Orange
                case 118: return 0x7F7F00; // Dark Yellow
                case 119: return 0x3F7F00; // Dark Yellow-Green
                case 120: return 0x007F00; // Dark Green
                case 121: return 0x007F3F; // Dark Green-Cyan
                case 122: return 0x007F7F; // Dark Cyan
                case 123: return 0x003F7F; // Dark Cyan-Blue
                case 124: return 0x00007F; // Dark Blue
                case 125: return 0x3F007F; // Dark Blue-Magenta
                case 126: return 0x7F007F; // Dark Magenta
                case 127: return 0x7F003F; // Dark Magenta-Red
                case 128: return 0x4C4C4C; // Dark Gray
                case 129: return 0x666666; // Medium Gray
                case 130: return 0x7F7F7F; // Gray
                case 131: return 0x999999; // Light Gray
                case 132: return 0xB3B3B3; // Light Gray
                case 133: return 0xCCCCCC; // Light Gray
                case 134: return 0xE6E6E6; // Very Light Gray
                case 135: return 0xFF0000; // Red
                case 136: return 0xFF7F00; // Orange
                case 137: return 0xFFFF00; // Yellow
                case 138: return 0x7FFF00; // Yellow-Green
                case 139: return 0x00FF00; // Green
                case 140: return 0x00FF7F; // Green-Cyan
                case 141: return 0x00FFFF; // Cyan
                case 142: return 0x007FFF; // Cyan-Blue
                case 143: return 0x0000FF; // Blue
                case 144: return 0x7F00FF; // Blue-Magenta
                case 145: return 0xFF00FF; // Magenta
                case 146: return 0xFF007F; // Magenta-Red
                case 147: return 0x7F0000; // Dark Red
                case 148: return 0x7F3F00; // Dark Orange
                case 149: return 0x7F7F00; // Dark Yellow
                case 150: return 0x3F7F00; // Dark Yellow-Green
                case 151: return 0x007F00; // Dark Green
                case 152: return 0x007F3F; // Dark Green-Cyan
                case 153: return 0x007F7F; // Dark Cyan
                case 154: return 0x003F7F; // Dark Cyan-Blue
                case 155: return 0x00007F; // Dark Blue
                case 156: return 0x3F007F; // Dark Blue-Magenta
                case 157: return 0x7F007F; // Dark Magenta
                case 158: return 0x7F003F; // Dark Magenta-Red
                case 159: return 0x4C4C4C; // Dark Gray
                case 160: return 0x666666; // Medium Gray
                case 161: return 0x7F7F7F; // Gray
                case 162: return 0x999999; // Light Gray
                case 163: return 0xB3B3B3; // Light Gray
                case 164: return 0xCCCCCC; // Light Gray
                case 165: return 0xE6E6E6; // Very Light Gray
                case 166: return 0xFF0000; // Red
                case 167: return 0xFF7F00; // Orange
                case 168: return 0xFFFF00; // Yellow
                case 169: return 0x7FFF00; // Yellow-Green
                case 170: return 0x00FF00; // Green
                case 171: return 0x00FF7F; // Green-Cyan
                case 172: return 0x00FFFF; // Cyan
                case 173: return 0x007FFF; // Cyan-Blue
                case 174: return 0x0000FF; // Blue
                case 175: return 0x7F00FF; // Blue-Magenta
                case 176: return 0xFF00FF; // Magenta
                case 177: return 0xFF007F; // Magenta-Red
                case 178: return 0x7F0000; // Dark Red
                case 179: return 0x7F3F00; // Dark Orange
                case 180: return 0x7F7F00; // Dark Yellow
                case 181: return 0x3F7F00; // Dark Yellow-Green
                case 182: return 0x007F00; // Dark Green
                case 183: return 0x007F3F; // Dark Green-Cyan
                case 184: return 0x007F7F; // Dark Cyan
                case 185: return 0x003F7F; // Dark Cyan-Blue
                case 186: return 0x00007F; // Dark Blue
                case 187: return 0x3F007F; // Dark Blue-Magenta
                case 188: return 0x7F007F; // Dark Magenta
                case 189: return 0x7F003F; // Dark Magenta-Red
                case 190: return 0x4C4C4C; // Dark Gray
                case 191: return 0x666666; // Medium Gray
                case 192: return 0x7F7F7F; // Gray
                case 193: return 0x999999; // Light Gray
                case 194: return 0xB3B3B3; // Light Gray
                case 195: return 0xCCCCCC; // Light Gray
                case 196: return 0xE6E6E6; // Very Light Gray
                case 197: return 0xFF0000; // Red
                case 198: return 0xFF7F00; // Orange
                case 199: return 0xFFFF00; // Yellow
                case 200: return 0x7FFF00; // Yellow-Green
                case 201: return 0x00FF00; // Green
                case 202: return 0x00FF7F; // Green-Cyan
                case 203: return 0x00FFFF; // Cyan
                case 204: return 0x007FFF; // Cyan-Blue
                case 205: return 0x0000FF; // Blue
                case 206: return 0x7F00FF; // Blue-Magenta
                case 207: return 0xFF00FF; // Magenta
                case 208: return 0xFF007F; // Magenta-Red
                case 209: return 0x7F0000; // Dark Red
                case 210: return 0x7F3F00; // Dark Orange
                case 211: return 0x7F7F00; // Dark Yellow
                case 212: return 0x3F7F00; // Dark Yellow-Green
                case 213: return 0x007F00; // Dark Green
                case 214: return 0x007F3F; // Dark Green-Cyan
                case 215: return 0x007F7F; // Dark Cyan
                case 216: return 0x003F7F; // Dark Cyan-Blue
                case 217: return 0x00007F; // Dark Blue
                case 218: return 0x3F007F; // Dark Blue-Magenta
                case 219: return 0x7F007F; // Dark Magenta
                case 220: return 0x7F003F; // Dark Magenta-Red
                case 221: return 0x4C4C4C; // Dark Gray
                case 222: return 0x666666; // Medium Gray
                case 223: return 0x7F7F7F; // Gray
                case 224: return 0x999999; // Light Gray
                case 225: return 0xB3B3B3; // Light Gray
                case 226: return 0xCCCCCC; // Light Gray
                case 227: return 0xE6E6E6; // Very Light Gray
                case 228: return 0xFF0000; // Red
                case 229: return 0xFF7F00; // Orange
                case 230: return 0xFFFF00; // Yellow
                case 231: return 0x7FFF00; // Yellow-Green
                case 232: return 0x00FF00; // Green
                case 233: return 0x00FF7F; // Green-Cyan
                case 234: return 0x00FFFF; // Cyan
                case 235: return 0x007FFF; // Cyan-Blue
                case 236: return 0x0000FF; // Blue
                case 237: return 0x7F00FF; // Blue-Magenta
                case 238: return 0xFF00FF; // Magenta
                case 239: return 0xFF007F; // Magenta-Red
                case 240: return 0x7F0000; // Dark Red
                case 241: return 0x7F3F00; // Dark Orange
                case 242: return 0x7F7F00; // Dark Yellow
                case 243: return 0x3F7F00; // Dark Yellow-Green
                case 244: return 0x007F00; // Dark Green
                case 245: return 0x007F3F; // Dark Green-Cyan
                case 246: return 0x007F7F; // Dark Cyan
                case 247: return 0x003F7F; // Dark Cyan-Blue
                case 248: return 0x00007F; // Dark Blue
                case 249: return 0x3F007F; // Dark Blue-Magenta
                case 250: return 0x7F007F; // Dark Magenta
                case 251: return 0x7F003F; // Dark Magenta-Red
                case 252: return 0x4C4C4C; // Dark Gray
                case 253: return 0x666666; // Medium Gray
                case 254: return 0x7F7F7F; // Gray
                case 255: return 0x999999; // Light Gray
                case 256: return 0xB3B3B3; // Light Gray
                case 257: return 0xCCCCCC; // Light Gray
                default: return 0x000000; // Black for ByLayer and others
            }
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