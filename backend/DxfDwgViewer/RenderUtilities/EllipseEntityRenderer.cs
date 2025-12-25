using System;
using ACadSharp.Entities;
using ACadSharp.Extensions;
using System.Collections.Generic;
using CSMath;

namespace DxfDwgViewer.RenderUtilities
{
    public class EllipseEntityRenderer
    {
        public class EllipseData
        {
            // three.js 对象类型标识
            public string Type { get; set; }
            public string Uuid { get; set; }
            public string EntityType { get; set; }
            
            // 基本几何属性
            public double CenterX { get; set; }
            public double CenterY { get; set; }
            public double CenterZ { get; set; }
            public double MajorAxisEndPointX { get; set; }
            public double MajorAxisEndPointY { get; set; }
            public double MajorAxisEndPointZ { get; set; }
            public double RadiusRatio { get; set; }
            public double StartParameter { get; set; }
            public double EndParameter { get; set; }
            public double Thickness { get; set; }
            
            // 法线向量（用于3D渲染）
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            
            // 实体属性
            public string Handle { get; set; }
            public string LayerName { get; set; }
            public bool IsInvisible { get; set; }
            public double LineTypeScale { get; set; }
            public double Transparency { get; set; }
            
            // 样式属性
            public short ColorIndex { get; set; }
            public string ColorHex { get; set; }
            public int ColorR { get; set; }
            public int ColorG { get; set; }
            public int ColorB { get; set; }
            public int ColorA { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            
            // 材质属性
            public string MaterialType { get; set; }
            public bool MaterialTransparent { get; set; }
            public double MaterialOpacity { get; set; }
            public bool MaterialDepthTest { get; set; }
            public bool MaterialDepthWrite { get; set; }
            public int MaterialSide { get; set; }
            
            // 用于three.js绘制的点列表
            public List<Point3DData> Points { get; set; }
            public List<double> Vertices { get; set; }
            public List<int> Indices { get; set; }
            
            // 椭圆计算属性
            public double MajorRadius { get; set; }
            public double MinorRadius { get; set; }
            public double RotationAngle { get; set; }
            public double Length { get; set; }
            public double Area { get; set; }
            public bool IsFullEllipse { get; set; }
            public bool IsCounterClockwise { get; set; }
            
            // 边界和变换属性
            public BoundsData Bounds { get; set; }
            public Point3DData Centroid { get; set; }
            public TransformData Transform { get; set; }
            
            // 坐标系信息
            public string CoordinateSystem { get; set; }
            public bool RequiresYAxisFlip { get; set; }
            
            // three.js 场景属性
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

        // 计算两点之间的距离
        private static double CalculateDistance(Point3DData p1, Point3DData p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2) + Math.Pow(p2.Z - p1.Z, 2));
        }

        // 计算点列表的边界框
        private static BoundsData CalculateBounds(List<Point3DData> points)
        {
            if (points == null || points.Count == 0)
                return new BoundsData(new Point3DData(0, 0, 0), new Point3DData(0, 0, 0));

            var minX = points.Min(p => p.X);
            var minY = points.Min(p => p.Y);
            var minZ = points.Min(p => p.Z);
            var maxX = points.Max(p => p.X);
            var maxY = points.Max(p => p.Y);
            var maxZ = points.Max(p => p.Z);
            
            return new BoundsData(
                new Point3DData(minX, minY, minZ),
                new Point3DData(maxX, maxY, maxZ)
            );
        }

        // 计算点列表的质心
        private static Point3DData CalculateCentroid(List<Point3DData> points)
        {
            if (points == null || points.Count == 0)
                return new Point3DData(0, 0, 0);

            double sumX = 0, sumY = 0, sumZ = 0;
            foreach (var point in points)
            {
                sumX += point.X;
                sumY += point.Y;
                sumZ += point.Z;
            }

            return new Point3DData(sumX / points.Count, sumY / points.Count, sumZ / points.Count);
        }

        public static EllipseData Render(Ellipse ellipse)
        {
            // 获取颜色信息
            var colorInfo = GetColorInfo(ellipse.Color.Index);
            
            // 获取椭圆的法线向量（默认为Z轴正方向）
            XYZ normal = ellipse.Normal;
            
            // 计算椭圆的基本参数
            double major = Math.Sqrt(
                ellipse.MajorAxisEndPoint.X * ellipse.MajorAxisEndPoint.X +
                ellipse.MajorAxisEndPoint.Y * ellipse.MajorAxisEndPoint.Y);
            double minor = major * ellipse.RadiusRatio;
            double angleOfMajorAxis = Math.Atan2(ellipse.MajorAxisEndPoint.Y, ellipse.MajorAxisEndPoint.X);
            
            // 判断是否为完整椭圆
            bool isFullEllipse = ellipse.IsFullEllipse;
            
            // 计算周长/弧长
            double length;
            if (isFullEllipse)
            {
                // 使用拉马努金近似公式计算椭圆周长
                double h = Math.Pow(major - minor, 2) / Math.Pow(major + minor, 2);
                length = Math.PI * (major + minor) * (1 + (3 * h) / (10 + Math.Sqrt(4 - 3 * h)));
            }
            else
            {
                // 计算椭圆弧长度（简化计算）
                double startParam = ellipse.StartParameter;
                double endParam = ellipse.EndParameter;
                if (endParam < startParam)
                {
                    endParam += 2 * Math.PI;
                }
                double paramDiff = endParam - startParam;
                // 简化估算：使用平均半径乘以角度差
                double avgRadius = (major + minor) / 2;
                length = avgRadius * paramDiff;
            }
            
            // 计算椭圆面积
            double area = Math.PI * major * minor;
            
            // 生成用于绘制的点列表（3D椭圆的近似点）
            var points = new List<Point3DData>();
            var vertices = new List<double>();
            int segments = 64;
            
            // 对于完整的椭圆
            if (isFullEllipse)
            {
                for (int i = 0; i <= segments; i++)
                {
                    double angle = 2 * Math.PI * i / segments;
                    // 计算椭圆上的点（3D）
                    double x = ellipse.Center.X + major * Math.Cos(angle) * Math.Cos(angleOfMajorAxis) - 
                               minor * Math.Sin(angle) * Math.Sin(angleOfMajorAxis);
                    double y = ellipse.Center.Y + major * Math.Cos(angle) * Math.Sin(angleOfMajorAxis) + 
                               minor * Math.Sin(angle) * Math.Cos(angleOfMajorAxis);
                    
                    // 计算Z坐标（考虑法线向量）
                    double z = ellipse.Center.Z;
                    
                    points.Add(new Point3DData(x, y, z));
                    vertices.Add(x);
                    vertices.Add(y);
                    vertices.Add(z);
                }
            }
            else
            {
                // 对于椭圆弧
                double startParameter = ellipse.StartParameter;
                double endParameter = ellipse.EndParameter;
                
                // 确保角度在正确的范围内
                if (endParameter < startParameter)
                {
                    endParameter += 2 * Math.PI;
                }
                
                for (int i = 0; i <= segments; i++)
                {
                    double angle = startParameter + (endParameter - startParameter) * i / segments;
                    // 计算椭圆弧上的点（3D）
                    double x = ellipse.Center.X + major * Math.Cos(angle) * Math.Cos(angleOfMajorAxis) - 
                               minor * Math.Sin(angle) * Math.Sin(angleOfMajorAxis);
                    double y = ellipse.Center.Y + major * Math.Cos(angle) * Math.Sin(angleOfMajorAxis) + 
                               minor * Math.Sin(angle) * Math.Cos(angleOfMajorAxis);
                    
                    // 计算Z坐标（考虑法线向量）
                    double z = ellipse.Center.Z;
                    
                    points.Add(new Point3DData(x, y, z));
                    vertices.Add(x);
                    vertices.Add(y);
                    vertices.Add(z);
                }
            }

            // 生成索引（用于Line或LineLoop）
            var indices = new List<int>();
            for (int i = 0; i < segments; i++)
            {
                indices.Add(i);
                indices.Add(i + 1);
            }
            
            // 计算边界框
            var bounds = CalculateBounds(points);
            
            // 计算质心
            var centroid = CalculateCentroid(points);
            
            // 获取透明度
            double transparency = 1.0;
            if (ellipse.Transparency.IsByLayer)
            {
                transparency = 0.0;
            }
            else if (ellipse.Transparency.IsByBlock)
            {
                transparency = 0.0;
            }
            else
            {
                transparency = ellipse.Transparency.Value / 100.0;
            }
            double opacity = 1.0 - transparency;

            // 获取实体属性
            string handle = ellipse.Handle.ToString();
            string layerName = ellipse.Layer?.Name ?? "0";
            bool isInvisible = ellipse.IsInvisible;
            double lineTypeScale = ellipse.LineTypeScale;

            // 创建变换矩阵
            var transform = new TransformData();
            if (normal.X != 0 || normal.Y != 0 || normal.Z != 1)
            {
                transform = CreateTransformFromNormal(normal.X, normal.Y, normal.Z, ellipse.Center.X, ellipse.Center.Y, ellipse.Center.Z);
            }

            // 创建椭圆数据结构
            var ellipseData = new EllipseData
            {
                // three.js 对象类型标识
                Type = "Ellipse",
                Uuid = Guid.NewGuid().ToString(),
                EntityType = "ELLIPSE",
                
                // 基本几何属性
                CenterX = ellipse.Center.X,
                CenterY = ellipse.Center.Y,
                CenterZ = ellipse.Center.Z,
                MajorAxisEndPointX = ellipse.MajorAxisEndPoint.X,
                MajorAxisEndPointY = ellipse.MajorAxisEndPoint.Y,
                MajorAxisEndPointZ = ellipse.MajorAxisEndPoint.Z,
                RadiusRatio = ellipse.RadiusRatio,
                StartParameter = ellipse.StartParameter,
                EndParameter = ellipse.EndParameter,
                Thickness = ellipse.Thickness,
                
                // 法线向量
                NormalX = normal.X,
                NormalY = normal.Y,
                NormalZ = normal.Z,
                
                // 实体属性
                Handle = handle,
                LayerName = layerName,
                IsInvisible = isInvisible,
                LineTypeScale = lineTypeScale,
                Transparency = transparency,
                
                // 样式属性
                ColorIndex = ellipse.Color.Index,
                ColorHex = colorInfo.Hex,
                ColorR = colorInfo.R,
                ColorG = colorInfo.G,
                ColorB = colorInfo.B,
                ColorA = (int)(opacity * 255),
                LineTypeName = ellipse.GetActiveLineType().Name,
                LineWeight = ellipse.GetActiveLineWeightType().GetLineWeightValue(),
                
                // 材质属性
                MaterialType = "LineBasicMaterial",
                MaterialTransparent = opacity < 1.0,
                MaterialOpacity = opacity,
                MaterialDepthTest = true,
                MaterialDepthWrite = true,
                MaterialSide = 2, // DoubleSide
                
                // 几何属性
                Points = points,
                Vertices = vertices,
                Indices = indices,
                
                // 椭圆计算属性
                MajorRadius = major,
                MinorRadius = minor,
                RotationAngle = angleOfMajorAxis,
                Length = length,
                Area = area,
                IsFullEllipse = isFullEllipse,
                IsCounterClockwise = ellipse.EndParameter > ellipse.StartParameter,
                
                // 边界和变换属性
                Bounds = bounds,
                Centroid = centroid,
                Transform = transform,
                
                // 坐标系信息
                CoordinateSystem = "AutoCAD",
                RequiresYAxisFlip = true,
                
                // three.js 场景属性
                Parent = null,
                Visible = !isInvisible,
                CastShadow = false,
                ReceiveShadow = false,
                RenderOrder = 0
            };

            return ellipseData;
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
        
        private static (string Hex, int R, int G, int B) GetColorInfo(short colorIndex)
        {
            ColorData color = GetColorByIndex(colorIndex);
            string hex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            return (hex, color.R, color.G, color.B);
        }

        private static TransformData CreateTransformFromNormal(double nx, double ny, double nz, double cx, double cy, double cz)
        {
            // 归一化法线向量
            double length = Math.Sqrt(nx * nx + ny * ny + nz * nz);
            if (length == 0)
            {
                return new TransformData();
            }
            nx /= length;
            ny /= length;
            nz /= length;

            // 计算旋转矩阵（将Z轴旋转到法线方向）
            // 使用四元数或轴角旋转
            double angle = Math.Acos(nz);
            double sinAngle = Math.Sin(angle);
            double cosAngle = Math.Cos(angle);

            // 旋转轴（Z轴与法线的叉积）
            double axisX = -ny;
            double axisY = nx;
            double axisZ = 0;

            // 归一化旋转轴
            double axisLength = Math.Sqrt(axisX * axisX + axisY * axisY + axisZ * axisZ);
            if (axisLength > 0)
            {
                axisX /= axisLength;
                axisY /= axisLength;
                axisZ /= axisLength;
            }

            // 构建旋转矩阵（罗德里格旋转公式）
            double m11 = cosAngle + axisX * axisX * (1 - cosAngle);
            double m12 = axisX * axisY * (1 - cosAngle) - axisZ * sinAngle;
            double m13 = axisX * axisZ * (1 - cosAngle) + axisY * sinAngle;

            double m21 = axisY * axisX * (1 - cosAngle) + axisZ * sinAngle;
            double m22 = cosAngle + axisY * axisY * (1 - cosAngle);
            double m23 = axisY * axisZ * (1 - cosAngle) - axisX * sinAngle;

            double m31 = axisZ * axisX * (1 - cosAngle) - axisY * sinAngle;
            double m32 = axisZ * axisY * (1 - cosAngle) + axisX * sinAngle;
            double m33 = cosAngle + axisZ * axisZ * (1 - cosAngle);

            return new TransformData
            {
                Matrix = new double[]
                {
                    m11, m12, m13, 0,
                    m21, m22, m23, 0,
                    m31, m32, m33, 0,
                    cx, cy, cz, 1
                }
            };
        }
    }
}