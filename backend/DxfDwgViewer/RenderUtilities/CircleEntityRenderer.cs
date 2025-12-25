using System;
using ACadSharp.Entities;
using ACadSharp.Extensions;
using System.Collections.Generic;
using CSMath;

namespace DxfDwgViewer.RenderUtilities
{
    public class CircleEntityRenderer
    {
        public class CircleData
        {
            public string Type { get; set; } = "";
            public string Uuid { get; set; } = "";
            public string EntityType { get; set; } = "";
            
            public double CenterX { get; set; }
            public double CenterY { get; set; }
            public double CenterZ { get; set; }
            public double Radius { get; set; }
            public double Thickness { get; set; }
            
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            
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
            
            public List<Point3DData> Points { get; set; } = new List<Point3DData>();
            public List<double> Vertices { get; set; } = new List<double>();
            public List<int> Indices { get; set; } = new List<int>();
            
            public double Circumference { get; set; }
            public double Area { get; set; }
            public double Diameter { get; set; }
            
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

        public static CircleData Render(Circle circle)
        {
            // 获取颜色信息
            var colorInfo = GetColorInfo(circle.Color.Index);
            
            // 获取圆的法线向量（默认为Z轴正方向）
            XYZ normal = circle.Normal;
            
            // 生成用于绘制的点列表（圆形的近似点）
            var points = new List<Point3DData>();
            var vertices = new List<double>();
            int segments = 64; // 圆形分割段数，提供更多细节
            for (int i = 0; i <= segments; i++)
            {
                double angle = 2 * Math.PI * i / segments;
                double x = circle.Center.X + circle.Radius * Math.Cos(angle);
                double y = circle.Center.Y + circle.Radius * Math.Sin(angle);
                double z = circle.Center.Z; // 圆形在同一平面上
                points.Add(new Point3DData(x, y, z));
                vertices.Add(x);
                vertices.Add(y);
                vertices.Add(z);
            }

            // 生成索引（用于Line或LineLoop）
            var indices = new List<int>();
            for (int i = 0; i < segments; i++)
            {
                indices.Add(i);
                indices.Add(i + 1);
            }

            // 计算周长、直径和面积
            double circumference = 2 * Math.PI * circle.Radius;
            double diameter = 2 * circle.Radius;
            double area = Math.PI * circle.Radius * circle.Radius;
            
            // 计算边界框
            var bounds = CalculateBounds(circle);
            
            // 计算质心（对于圆就是中心点）
            var centroid = new Point3DData(
                circle.Center.X, 
                circle.Center.Y, 
                circle.Center.Z
            );

            // 获取透明度
            double transparency = 1.0;
            if (circle.Transparency.IsByLayer)
            {
                transparency = 0.0;
            }
            else if (circle.Transparency.IsByBlock)
            {
                transparency = 0.0;
            }
            else
            {
                transparency = circle.Transparency.Value / 100.0;
            }
            double opacity = 1.0 - transparency;

            // 获取实体属性
            string handle = circle.Handle.ToString();
            string layerName = circle.Layer?.Name ?? "0";
            bool isInvisible = circle.IsInvisible;
            double lineTypeScale = circle.LineTypeScale;

            // 创建变换矩阵
            var transform = new TransformData();
            if (normal.X != 0 || normal.Y != 0 || normal.Z != 1)
            {
                transform = CreateTransformFromNormal(normal.X, normal.Y, normal.Z, circle.Center.X, circle.Center.Y, circle.Center.Z);
            }

            var circleData = new CircleData
            {
                // three.js 对象类型标识
                Type = "Circle",
                Uuid = Guid.NewGuid().ToString(),
                EntityType = "CIRCLE",
                
                // 基本几何属性
                CenterX = circle.Center.X,
                CenterY = circle.Center.Y,
                CenterZ = circle.Center.Z,
                Radius = circle.Radius,
                Thickness = circle.Thickness,
                Diameter = diameter,
                
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
                ColorIndex = circle.Color.Index,
                ColorHex = colorInfo.Hex,
                ColorR = colorInfo.R,
                ColorG = colorInfo.G,
                ColorB = colorInfo.B,
                ColorA = (int)(opacity * 255),
                LineTypeName = circle.GetActiveLineType().Name,
                LineWeight = circle.GetActiveLineWeightType().GetLineWeightValue(),
                
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
                
                // 圆形计算属性
                Circumference = circumference,
                Area = area,
                
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

            return circleData;
        }
        
        private static (string Hex, int R, int G, int B) GetColorInfo(short colorIndex)
        {
            int colorRGB = GetColorRGB(colorIndex);
            int r = (colorRGB >> 16) & 0xFF;
            int g = (colorRGB >> 8) & 0xFF;
            int b = colorRGB & 0xFF;
            string hex = $"#{r:X2}{g:X2}{b:X2}";
            return (hex, r, g, b);
        }
        
        private static int GetColorRGB(short colorIndex)
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
        
        private static BoundsData CalculateBounds(Circle circle)
        {
            return new BoundsData(
                new Point3DData(
                    circle.Center.X - circle.Radius,
                    circle.Center.Y - circle.Radius,
                    circle.Center.Z - circle.Thickness / 2
                ),
                new Point3DData(
                    circle.Center.X + circle.Radius,
                    circle.Center.Y + circle.Radius,
                    circle.Center.Z + circle.Thickness / 2
                )
            );
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