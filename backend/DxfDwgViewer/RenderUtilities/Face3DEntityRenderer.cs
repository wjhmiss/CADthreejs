using ACadSharp.Entities;
using ACadSharp.Extensions;
using System.Collections.Generic;
using ACadSharp;
using CSMath;

namespace DxfDwgViewer.RenderUtilities
{
    public class Face3DEntityRenderer
    {
        public class Face3DData
        {
            public string Type { get; set; } = "";
            public string Uuid { get; set; } = "";
            public string EntityType { get; set; } = "";
            
            public Point3DData FirstCorner { get; set; } = new Point3DData();
            public Point3DData SecondCorner { get; set; } = new Point3DData();
            public Point3DData ThirdCorner { get; set; } = new Point3DData();
            public Point3DData FourthCorner { get; set; } = new Point3DData();
            
            public string Handle { get; set; } = "";
            public string LayerName { get; set; } = "";
            public bool IsInvisible { get; set; }
            public double LineTypeScale { get; set; }
            public double Transparency { get; set; }
            
            public short ColorIndex { get; set; }
            public string ColorHex { get; set; } = "";
            public int ColorR { get; set; }
            public int ColorG { get; set; }
            public int ColorB { get; set; }
            public int ColorA { get; set; }
            public string LineTypeName { get; set; } = "";
            public double LineWeight { get; set; }
            
            public string MaterialType { get; set; } = "";
            public bool MaterialTransparent { get; set; }
            public double MaterialOpacity { get; set; }
            public bool MaterialDepthTest { get; set; }
            public bool MaterialDepthWrite { get; set; }
            public int MaterialSide { get; set; }
            
            public bool HasFourthCorner { get; set; }
            public double Area { get; set; }
            public Point3DData Center { get; set; } = new Point3DData();
            public Point3DData Normal { get; set; } = new Point3DData();
            public string GeometryType { get; set; } = "";
            
            public List<Point3DData> Points { get; set; } = new List<Point3DData>();
            public List<double> Vertices { get; set; } = new List<double>();
            public List<int> Indices { get; set; } = new List<int>();
            public List<double> Normals { get; set; } = new List<double>();
            
            public BoundsData Bounds { get; set; } = new BoundsData();
            public Point3DData Centroid { get; set; } = new Point3DData();
            public TransformData Transform { get; set; } = new TransformData();
            
            public string CoordinateSystem { get; set; } = "";
            public bool RequiresYAxisFlip { get; set; }
            
            public object Parent { get; set; }
            public bool Visible { get; set; }
            public bool CastShadow { get; set; }
            public bool ReceiveShadow { get; set; }
            public int RenderOrder { get; set; }
        }

        public class PointData
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        public static Face3DData Render(Face3D face3D)
        {
            // 判断是否有第四个角点（即是否为四边形）
            bool hasFourthCorner = !(face3D.FourthCorner.X == face3D.ThirdCorner.X && 
                                   face3D.FourthCorner.Y == face3D.ThirdCorner.Y &&
                                   face3D.FourthCorner.Z == face3D.ThirdCorner.Z);
            
            // 创建3D顶点数据
            var firstCorner = new Point3DData(face3D.FirstCorner.X, face3D.FirstCorner.Y, face3D.FirstCorner.Z);
            var secondCorner = new Point3DData(face3D.SecondCorner.X, face3D.SecondCorner.Y, face3D.SecondCorner.Z);
            var thirdCorner = new Point3DData(face3D.ThirdCorner.X, face3D.ThirdCorner.Y, face3D.ThirdCorner.Z);
            var fourthCorner = new Point3DData(face3D.FourthCorner.X, face3D.FourthCorner.Y, face3D.FourthCorner.Z);
            
            // 计算中心点（3D）
            Point3DData center;
            if (hasFourthCorner)
            {
                center = new Point3DData(
                    (firstCorner.X + secondCorner.X + thirdCorner.X + fourthCorner.X) / 4,
                    (firstCorner.Y + secondCorner.Y + thirdCorner.Y + fourthCorner.Y) / 4,
                    (firstCorner.Z + secondCorner.Z + thirdCorner.Z + fourthCorner.Z) / 4
                );
            }
            else
            {
                center = new Point3DData(
                    (firstCorner.X + secondCorner.X + thirdCorner.X) / 3,
                    (firstCorner.Y + secondCorner.Y + thirdCorner.Y) / 3,
                    (firstCorner.Z + secondCorner.Z + thirdCorner.Z) / 3
                );
            }
            
            // 计算法线向量
            var normal = CalculateFaceNormal(firstCorner, secondCorner, thirdCorner);
            
            // 计算面积（3D空间中的面积）
            double area = CalculateFaceArea(firstCorner, secondCorner, thirdCorner, hasFourthCorner ? fourthCorner : null);
            
            // 计算边界框
            var bounds = CalculateBounds(firstCorner, secondCorner, thirdCorner, hasFourthCorner ? fourthCorner : null);
            
            // 创建顶点列表
            var points = new List<Point3DData> { firstCorner, secondCorner, thirdCorner };
            if (hasFourthCorner)
            {
                points.Add(fourthCorner);
            }
            
            // 获取颜色信息
            var colorInfo = GetColorInfo(face3D.Color.Index);
            
            // 获取透明度
            double transparency = 1.0;
            if (face3D.Transparency.IsByLayer)
            {
                transparency = 0.0;
            }
            else if (face3D.Transparency.IsByBlock)
            {
                transparency = 0.0;
            }
            else
            {
                transparency = face3D.Transparency.Value / 100.0;
            }
            double opacity = 1.0 - transparency;
            
            // 获取实体属性
            string handle = face3D.Handle.ToString();
            string layerName = face3D.Layer?.Name ?? "0";
            bool isInvisible = face3D.IsInvisible;
            double lineTypeScale = face3D.LineTypeScale;
            
            // 生成three.js兼容的顶点数组
            var vertices = new List<double>();
            foreach (var point in points)
            {
                vertices.Add(point.X);
                vertices.Add(point.Y);
                vertices.Add(point.Z);
            }
            
            // 生成索引数组
            var indices = new List<int>();
            if (hasFourthCorner)
            {
                // 四边形分解为两个三角形
                indices.AddRange(new[] { 0, 1, 2 });
                indices.AddRange(new[] { 0, 2, 3 });
            }
            else
            {
                // 三角形
                indices.AddRange(new[] { 0, 1, 2 });
            }
            
            // 生成法线数组（每个顶点使用相同的面法线）
            var normals = new List<double>();
            for (int i = 0; i < points.Count; i++)
            {
                normals.Add(normal.X);
                normals.Add(normal.Y);
                normals.Add(normal.Z);
            }
            
            // 创建变换矩阵（单位矩阵）
            var transform = new TransformData
            {
                Matrix = new double[]
                {
                    1, 0, 0, 0,
                    0, 1, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1
                }
            };
            
            var face3DData = new Face3DData
            {
                // three.js 对象类型标识
                Type = "Face3D",
                Uuid = Guid.NewGuid().ToString(),
                EntityType = "3DFACE",
                
                // 基本几何属性
                FirstCorner = firstCorner,
                SecondCorner = secondCorner,
                ThirdCorner = thirdCorner,
                FourthCorner = fourthCorner,
                
                // 实体属性
                Handle = handle,
                LayerName = layerName,
                IsInvisible = isInvisible,
                LineTypeScale = lineTypeScale,
                Transparency = transparency,
                
                // 样式属性
                ColorIndex = face3D.Color.Index,
                ColorHex = colorInfo.Hex,
                ColorR = colorInfo.R,
                ColorG = colorInfo.G,
                ColorB = colorInfo.B,
                ColorA = (int)(opacity * 255),
                LineTypeName = face3D.GetActiveLineType().Name,
                LineWeight = face3D.GetActiveLineWeightType().GetLineWeightValue(),
                
                // 材质属性
                MaterialType = "MeshBasicMaterial",
                MaterialTransparent = opacity < 1.0,
                MaterialOpacity = opacity,
                MaterialDepthTest = true,
                MaterialDepthWrite = true,
                MaterialSide = 2,
                
                // 几何属性
                HasFourthCorner = hasFourthCorner,
                Area = area,
                Center = center,
                Normal = normal,
                GeometryType = hasFourthCorner ? "Quad" : "Triangle",
                
                // 用于three.js绘制的顶点列表
                Points = points,
                Vertices = vertices,
                Indices = indices,
                Normals = normals,
                
                // 边界和变换属性
                Bounds = bounds,
                Centroid = center,
                Transform = transform,
                
                // 坐标系信息
                CoordinateSystem = "AutoCAD",
                RequiresYAxisFlip = true,
                
                // three.js 场景属性
                Parent = null,
                Visible = !isInvisible,
                CastShadow = true,
                ReceiveShadow = true,
                RenderOrder = 0
            };

            return face3DData;
        }
        
        /// <summary>
        /// 计算面的法线向量
        /// </summary>
        private static Point3DData CalculateFaceNormal(Point3DData p1, Point3DData p2, Point3DData p3)
        {
            // 计算两个向量
            var v1 = new Point3DData(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            var v2 = new Point3DData(p3.X - p1.X, p3.Y - p1.Y, p3.Z - p1.Z);
            
            // 计算叉积
            var crossX = v1.Y * v2.Z - v1.Z * v2.Y;
            var crossY = v1.Z * v2.X - v1.X * v2.Z;
            var crossZ = v1.X * v2.Y - v1.Y * v2.X;
            
            // 计算长度
            var length = Math.Sqrt(crossX * crossX + crossY * crossY + crossZ * crossZ);
            
            // 归一化
            if (length > 0.0001) // 避免除以零
            {
                return new Point3DData(crossX / length, crossY / length, crossZ / length);
            }
            
            // 如果面积为零，返回默认法线
            return new Point3DData(0, 0, 1);
        }
        
        /// <summary>
        /// 计算面的面积（3D空间中）
        /// </summary>
        private static double CalculateFaceArea(Point3DData p1, Point3DData p2, Point3DData p3, Point3DData? p4 = null)
        {
            if (p4 != null)
            {
                // 四边形分解为两个三角形计算面积
                var area1 = CalculateTriangleArea(p1, p2, p3);
                var area2 = CalculateTriangleArea(p1, p3, p4);
                return area1 + area2;
            }
            else
            {
                // 三角形面积
                return CalculateTriangleArea(p1, p2, p3);
            }
        }
        
        /// <summary>
        /// 计算三角形面积（3D空间中）
        /// </summary>
        private static double CalculateTriangleArea(Point3DData p1, Point3DData p2, Point3DData p3)
        {
            // 计算两个向量
            var v1 = new Point3DData(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            var v2 = new Point3DData(p3.X - p1.X, p3.Y - p1.Y, p3.Z - p1.Z);
            
            // 计算叉积
            var crossX = v1.Y * v2.Z - v1.Z * v2.Y;
            var crossY = v1.Z * v2.X - v1.X * v2.Z;
            var crossZ = v1.X * v2.Y - v1.Y * v2.X;
            
            // 叉积的长度的一半即为三角形面积
            var crossLength = Math.Sqrt(crossX * crossX + crossY * crossY + crossZ * crossZ);
            return 0.5 * crossLength;
        }
        
        /// <summary>
        /// 计算边界框
        /// </summary>
        private static BoundsData CalculateBounds(Point3DData p1, Point3DData p2, Point3DData p3, Point3DData? p4 = null)
        {
            var minX = Math.Min(Math.Min(p1.X, p2.X), p3.X);
            var minY = Math.Min(Math.Min(p1.Y, p2.Y), p3.Y);
            var minZ = Math.Min(Math.Min(p1.Z, p2.Z), p3.Z);
            
            var maxX = Math.Max(Math.Max(p1.X, p2.X), p3.X);
            var maxY = Math.Max(Math.Max(p1.Y, p2.Y), p3.Y);
            var maxZ = Math.Max(Math.Max(p1.Z, p2.Z), p3.Z);
            
            if (p4 != null)
            {
                minX = Math.Min(minX, p4.X);
                minY = Math.Min(minY, p4.Y);
                minZ = Math.Min(minZ, p4.Z);
                
                maxX = Math.Max(maxX, p4.X);
                maxY = Math.Max(maxY, p4.Y);
                maxZ = Math.Max(maxZ, p4.Z);
            }
            
            return new BoundsData(
                new Point3DData(minX, minY, minZ),
                new Point3DData(maxX, maxY, maxZ)
            );
        }

        private static (short Index, string Hex, int R, int G, int B) GetColorInfo(short colorIndex)
        {
            int r, g, b;
            switch (colorIndex)
            {
                case 1:
                    r = 255; g = 0; b = 0;
                    break;
                case 2:
                    r = 255; g = 255; b = 0;
                    break;
                case 3:
                    r = 0; g = 255; b = 0;
                    break;
                case 4:
                    r = 0; g = 255; b = 255;
                    break;
                case 5:
                    r = 0; g = 0; b = 255;
                    break;
                case 6:
                    r = 255; g = 0; b = 255;
                    break;
                case 7:
                    r = 255; g = 255; b = 255;
                    break;
                default:
                    r = 0; g = 0; b = 0;
                    break;
            }
            return (colorIndex, $"#{r:X2}{g:X2}{b:X2}", r, g, b);
        }
    }
}