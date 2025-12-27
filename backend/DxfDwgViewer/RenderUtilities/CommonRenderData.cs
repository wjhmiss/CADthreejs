using System;
using System.Collections.Generic;

namespace DxfDwgViewer.RenderUtilities
{
    /// <summary>
    /// 2D点数据结构
    /// </summary>
    public class PointData
    {
        public double X { get; set; }
        public double Y { get; set; }
        
        public PointData(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// 3D点数据结构，用于three.js兼容性
    /// </summary>
    public class Point3DData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point3DData()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public Point3DData(double x, double y, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double[] ToArray()
        {
            return new double[] { X, Y, Z };
        }
    }

    /// <summary>
    /// 边界框数据结构，用于three.js兼容性
    /// </summary>
    public class BoundsData
    {
        public Point3DData Min { get; set; }
        public Point3DData Max { get; set; }
        public Point3DData Center { get; set; }
        public Point3DData Size { get; set; }

        public BoundsData()
        {
            Min = new Point3DData(0, 0, 0);
            Max = new Point3DData(0, 0, 0);
            Center = new Point3DData(0, 0, 0);
            Size = new Point3DData(0, 0, 0);
        }

        public BoundsData(Point3DData min, Point3DData max)
        {
            Min = min;
            Max = max;
            Center = new Point3DData(
                (min.X + max.X) / 2,
                (min.Y + max.Y) / 2,
                (min.Z + max.Z) / 2
            );
            Size = new Point3DData(
                max.X - min.X,
                max.Y - min.Y,
                max.Z - min.Z
            );
        }
    }

    /// <summary>
    /// 3D边界框数据结构，用于three.js兼容性
    /// </summary>
    public class BoundsData3D
    {
        public Point3DData Min { get; set; }
        public Point3DData Max { get; set; }
        public Point3DData Center { get; set; }
        public Point3DData Size { get; set; }

        public BoundsData3D(Point3DData min, Point3DData max)
        {
            Min = min;
            Max = max;
            Center = new Point3DData(
                (min.X + max.X) / 2,
                (min.Y + max.Y) / 2,
                (min.Z + max.Z) / 2
            );
            Size = new Point3DData(
                max.X - min.X,
                max.Y - min.Y,
                max.Z - min.Z
            );
        }
    }

    /// <summary>
    /// 颜色数据结构，用于three.js兼容性
    /// </summary>
    public class ColorData
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public double A { get; set; }
        public string Hex { get; set; }

        // 无参构造函数，用于JSON序列化/反序列化
        public ColorData()
        {
            Index = 0;
            Name = "White";
            R = 255;
            G = 255;
            B = 255;
            A = 1.0;
            Hex = "#FFFFFF";
        }

        public ColorData(int colorIndex)
        {
            Index = colorIndex;
            
            // 根据颜色索引获取实际RGB值
            // 这里使用简化的颜色映射，实际应该根据CAD颜色表获取
            switch (colorIndex)
            {
                case 1: // 红色
                    R = 255; G = 0; B = 0; Name = "Red"; break;
                case 2: // 黄色
                    R = 255; G = 255; B = 0; Name = "Yellow"; break;
                case 3: // 绿色
                    R = 0; G = 255; B = 0; Name = "Green"; break;
                case 4: // 青色
                    R = 0; G = 255; B = 255; Name = "Cyan"; break;
                case 5: // 蓝色
                    R = 0; G = 0; B = 255; Name = "Blue"; break;
                case 6: // 洋红色
                    R = 255; G = 0; B = 255; Name = "Magenta"; break;
                case 7: // 白色
                case 0: // 随层/随块
                    R = 255; G = 255; B = 255; Name = "White"; break;
                default:
                    R = 128; G = 128; B = 128; Name = "Gray"; break;
            }
            
            A = 1.0;
            Hex = $"#{R:X2}{G:X2}{B:X2}";
        }

        public ColorData(byte r, byte g, byte b)
        {
            Index = 0;
            R = r;
            G = g;
            B = b;
            A = 1.0;
            Hex = $"#{R:X2}{G:X2}{B:X2}";
            Name = "Custom";
        }

        public double[] ToRGB()
        {
            return new double[] { R / 255.0, G / 255.0, B / 255.0 };
        }

        public double[] ToRGBA()
        {
            return new double[] { R / 255.0, G / 255.0, B / 255.0, A };
        }
    }

    /// <summary>
    /// 法向量数据结构，用于three.js兼容性
    /// </summary>
    public class NormalData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public NormalData(double x = 0, double y = 0, double z = 1)
        {
            // 归一化
            double length = Math.Sqrt(x * x + y * y + z * z);
            if (length > 0)
            {
                X = x / length;
                Y = y / length;
                Z = z / length;
            }
            else
            {
                X = 0; Y = 0; Z = 1;
            }
        }

        public double[] ToArray()
        {
            return new double[] { X, Y, Z };
        }

        public static NormalData Cross(NormalData a, NormalData b)
        {
            return new NormalData(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );
        }

        public static double Dot(NormalData a, NormalData b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
    }

    /// <summary>
    /// 变换矩阵数据结构，用于three.js兼容性
    /// </summary>
    public class TransformData
    {
        public Point3DData Position { get; set; }
        public Point3DData Rotation { get; set; }
        public Point3DData Scale { get; set; }
        public double[] Matrix { get; set; }
        
        /// <summary>
        /// 获取一个值，指示此变换是否为单位矩阵
        /// </summary>
        public bool IsIdentity 
        { 
            get 
            {
                if (Matrix == null || Matrix.Length != 16)
                    return false;
                    
                // 检查是否为单位矩阵
                return Math.Abs(Matrix[0] - 1) < 1e-10 && Math.Abs(Matrix[1]) < 1e-10 && Math.Abs(Matrix[2]) < 1e-10 && Math.Abs(Matrix[3]) < 1e-10 &&
                       Math.Abs(Matrix[4]) < 1e-10 && Math.Abs(Matrix[5] - 1) < 1e-10 && Math.Abs(Matrix[6]) < 1e-10 && Math.Abs(Matrix[7]) < 1e-10 &&
                       Math.Abs(Matrix[8]) < 1e-10 && Math.Abs(Matrix[9]) < 1e-10 && Math.Abs(Matrix[10] - 1) < 1e-10 && Math.Abs(Matrix[11]) < 1e-10 &&
                       Math.Abs(Matrix[12]) < 1e-10 && Math.Abs(Matrix[13]) < 1e-10 && Math.Abs(Matrix[14]) < 1e-10 && Math.Abs(Matrix[15] - 1) < 1e-10;
            }
        }

        public TransformData()
        {
            Position = new Point3DData { X = 0, Y = 0, Z = 0 };
            Rotation = new Point3DData { X = 0, Y = 0, Z = 0 };
            Scale = new Point3DData { X = 1, Y = 1, Z = 1 };
            // 初始化为单位矩阵
            Matrix = new double[16] {
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            };
        }

        public TransformData(double scaleX, double scaleY, double scaleZ, double rotation, double translateX, double translateY, double translateZ)
        {
            Position = new Point3DData { X = translateX, Y = translateY, Z = translateZ };
            Rotation = new Point3DData { X = 0, Y = 0, Z = rotation };
            Scale = new Point3DData { X = scaleX, Y = scaleY, Z = scaleZ };
            
            // 创建组合变换矩阵：先缩放，再旋转，最后平移
            double cos = Math.Cos(rotation);
            double sin = Math.Sin(rotation);

            Matrix = new double[16] {
                scaleX * cos, scaleX * sin, 0, 0,
                -scaleY * sin, scaleY * cos, 0, 0,
                0, 0, scaleZ, 0,
                translateX, translateY, translateZ, 1
            };
        }
    }

    /// <summary>
    /// 矩阵数据结构，用于CAD变换
    /// </summary>
    public class MatrixData
    {
        public double M11 { get; set; }
        public double M12 { get; set; }
        public double M13 { get; set; }
        public double M14 { get; set; }
        public double M21 { get; set; }
        public double M22 { get; set; }
        public double M23 { get; set; }
        public double M24 { get; set; }
        public double M31 { get; set; }
        public double M32 { get; set; }
        public double M33 { get; set; }
        public double M34 { get; set; }
        public double M41 { get; set; }
        public double M42 { get; set; }
        public double M43 { get; set; }
        public double M44 { get; set; }

        public MatrixData()
        {
            // 初始化为单位矩阵
            M11 = 1; M12 = 0; M13 = 0; M14 = 0;
            M21 = 0; M22 = 1; M23 = 0; M24 = 0;
            M31 = 0; M32 = 0; M33 = 1; M34 = 0;
            M41 = 0; M42 = 0; M43 = 0; M44 = 1;
        }
    }

    /// <summary>
    /// 几何体数据结构，用于three.js兼容性
    /// </summary>
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
        public bool IsClosed { get; set; }
        public bool IsPeriodic { get; set; }
        public int Degree { get; set; }
        
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
            IsClosed = false;
            IsPeriodic = false;
            Degree = 0;
        }
    }

    /// <summary>
    /// 材质数据结构，用于three.js兼容性
    /// </summary>
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
}