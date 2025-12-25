using ACadSharp.Entities;
using ACadSharp.Extensions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DxfDwgViewer.RenderUtilities
{
    public class XLineEntityRenderer
    {
        public class XLineData
        {
            // 基本属性
            public Point3DData FirstPoint { get; set; }
            public Point3DData Direction { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            public Point3DData[] LinePoints { get; set; }
            
            // 添加three.js友好的属性
            public Point3DData SecondPoint { get; set; }
            public double Length { get; set; }
            public double Angle { get; set; }
            
            // three.js兼容性增强属性
            public TransformData Transform { get; set; }
            public GeometryData Geometry { get; set; }
            public MaterialData Material { get; set; }
            public double[] VertexPositions { get; set; } // 顶点位置数组（扁平化）
            public double[] VertexColors { get; set; } // 顶点颜色数组（扁平化）
            public int[] Indices { get; set; } // 索引数组
            public int VertexCount { get; set; } // 顶点数量
            public double Opacity { get; set; } // 不透明度
            public bool Transparent { get; set; } // 是否透明
            public Point3DData Normal { get; set; } // 法向量
            public BoundsData Bounds { get; set; } // 边界框
            public Point3DData Center { get; set; } // 中心点
        }

        public static XLineData Render(XLine xline)
        {
            // 计算线段长度和角度
            double length = 0;
            double angle = 0;
            
            var xlineData = new XLineData
            {
                FirstPoint = new Point3DData 
                { 
                    X = xline.FirstPoint.X, 
                    Y = xline.FirstPoint.Y, 
                    Z = xline.FirstPoint.Z 
                },
                Direction = new Point3DData 
                { 
                    X = xline.Direction.X, 
                    Y = xline.Direction.Y, 
                    Z = xline.Direction.Z 
                },
                ColorIndex = xline.Color.Index,
                LineTypeName = xline.GetActiveLineType()?.Name ?? "",
                LineWeight = xline.GetActiveLineWeightType().GetLineWeightValue(),
                LinePoints = new Point3DData[2],
                
                // 初始化three.js兼容性属性
                Opacity = 1.0,
                Transparent = false,
                Normal = new Point3DData { X = 0, Y = 0, Z = 1 }
            };

            // 绘制构造线（无限长直线），在视图范围内绘制
            float x1 = (float)xline.FirstPoint.X;
            float y1 = (float)xline.FirstPoint.Y;
            float x2 = x1 + (float)xline.Direction.X * 1000;
            float y2 = y1 + (float)xline.Direction.Y * 1000;
            float x3 = x1 - (float)xline.Direction.X * 1000;
            float y3 = y1 - (float)xline.Direction.Y * 1000;

            // 计算长度和角度
            float dx = x2 - x1;
            float dy = y2 - y1;
            length = Math.Sqrt(dx * dx + dy * dy);
            angle = Math.Atan2(dy, dx);

            // 添加线段端点到数据对象中
            var point3 = new Point3DData { X = x3, Y = y3, Z = 0 };
            var point2 = new Point3DData { X = x2, Y = y2, Z = 0 };
            
            xlineData.LinePoints[0] = point3;
            xlineData.LinePoints[1] = point2;
            
            // 设置增强属性
            xlineData.SecondPoint = point2;
            xlineData.Length = length;
            xlineData.Angle = angle;

            // 填充three.js兼容性数据
            PopulateThreeJSData(xlineData);

            return xlineData;
        }

        private static void PopulateThreeJSData(XLineData xlineData)
        {
            // 计算中心点
            double centerX = (xlineData.FirstPoint.X + xlineData.SecondPoint.X) / 2;
            double centerY = (xlineData.FirstPoint.Y + xlineData.SecondPoint.Y) / 2;
            double centerZ = (xlineData.FirstPoint.Z + xlineData.SecondPoint.Z) / 2;
            
            xlineData.Center = new Point3DData { X = centerX, Y = centerY, Z = centerZ };

            // 设置变换数据
            xlineData.Transform = new TransformData
            {
                Position = new Point3DData { X = centerX, Y = centerY, Z = centerZ },
                Rotation = new Point3DData { X = 0, Y = 0, Z = xlineData.Angle },
                Scale = new Point3DData { X = 1, Y = 1, Z = 1 }
            };
            UpdateTransformMatrix(xlineData.Transform);

            // 设置几何体数据
            xlineData.Geometry = new GeometryData
            {
                Type = "BufferGeometry",
                VertexCount = xlineData.LinePoints.Length,
                HasColors = true,
                HasIndices = true
            };

            // 设置材质数据
            int rgbColor = GetColorRgbByIndex(xlineData.ColorIndex);
            xlineData.Material = new MaterialData
            {
                Type = "LineBasicMaterial",
                Color = rgbColor,
                Opacity = xlineData.Opacity,
                Transparent = xlineData.Transparent,
                LineWidth = xlineData.LineWeight,
                Side = true // 双面渲染
            };

            // 填充顶点位置数组
            xlineData.VertexPositions = new double[xlineData.LinePoints.Length * 3];
            int posIndex = 0;
            foreach (var point in xlineData.LinePoints)
            {
                xlineData.VertexPositions[posIndex++] = point.X;
                xlineData.VertexPositions[posIndex++] = point.Y;
                xlineData.VertexPositions[posIndex++] = point.Z;
            }

            // 填充顶点颜色数组
            int r = (rgbColor >> 16) & 0xFF;
            int g = (rgbColor >> 8) & 0xFF;
            int b = rgbColor & 0xFF;
            
            xlineData.VertexColors = new double[xlineData.LinePoints.Length * 3];
            int colorIndex = 0;
            for (int i = 0; i < xlineData.LinePoints.Length; i++)
            {
                xlineData.VertexColors[colorIndex++] = r / 255.0;
                xlineData.VertexColors[colorIndex++] = g / 255.0;
                xlineData.VertexColors[colorIndex++] = b / 255.0;
            }

            // 填充索引数组
            xlineData.Indices = new int[xlineData.LinePoints.Length];
            for (int i = 0; i < xlineData.LinePoints.Length; i++)
            {
                xlineData.Indices[i] = i;
            }

            // 设置顶点数量
            xlineData.VertexCount = xlineData.LinePoints.Length;

            // 计算边界框
            CalculateBounds(xlineData);
        }

        private static void CalculateBounds(XLineData xlineData)
        {
            double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;

            foreach (var point in xlineData.LinePoints)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                minZ = Math.Min(minZ, point.Z);
                
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
                maxZ = Math.Max(maxZ, point.Z);
            }

            xlineData.Bounds = new BoundsData
            {
                Min = new Point3DData { X = minX, Y = minY, Z = minZ },
                Max = new Point3DData { X = maxX, Y = maxY, Z = maxZ },
                Size = new Point3DData { X = maxX - minX, Y = maxY - minY, Z = maxZ - minZ },
                Center = new Point3DData { X = (minX + maxX) / 2, Y = (minY + maxY) / 2, Z = (minZ + maxZ) / 2 }
            };
        }

        private static int GetColorRgbByIndex(short colorIndex)
        {
            // ACI colors (AutoCAD Color Index) - 返回RGB格式
            switch (colorIndex)
            {
                case 1: return 0xFF0000; // Red
                case 2: return 0xFFFF00; // Yellow
                case 3: return 0x00FF00; // Green
                case 4: return 0x00FFFF; // Cyan
                case 5: return 0x0000FF; // Blue
                case 6: return 0xFF00FF; // Magenta
                case 7: return 0xFFFFFF; // White
                case 256: return 0xB3B3B3; // ByLayer (灰色)
                default: return 0x000000; // Black
            }
        }

        private static void UpdateTransformMatrix(TransformData transform)
        {
            // 创建4x4变换矩阵
            transform.Matrix = new double[16]
            {
                // 第一行
                transform.Scale.X, 0, 0, transform.Position.X,
                // 第二行
                0, transform.Scale.Y, 0, transform.Position.Y,
                // 第三行
                0, 0, transform.Scale.Z, transform.Position.Z,
                // 第四行
                0, 0, 0, 1
            };
        }
    }
}
