using System;
using ACadSharp.Entities;
using ACadSharp.Extensions;
using System.Collections.Generic;

namespace DxfDwgViewer.RenderUtilities
{
    public class ArcEntityRenderer
    {
        public class ArcData
        {
            public string Type { get; set; } = "";
            public string Uuid { get; set; } = "";
            public string EntityType { get; set; } = "";
            
            public double CenterX { get; set; }
            public double CenterY { get; set; }
            public double CenterZ { get; set; }
            public double Radius { get; set; }
            public double StartAngle { get; set; }
            public double EndAngle { get; set; }
            public double Thickness { get; set; }
            public double Sweep { get; set; }
            
            public double StartX { get; set; }
            public double StartY { get; set; }
            public double StartZ { get; set; }
            public double EndX { get; set; }
            public double EndY { get; set; }
            public double EndZ { get; set; }
            
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
            
            public bool IsCCW { get; set; }
            public double Length { get; set; }
            public double Area { get; set; }
            public double ChordLength { get; set; }
            public double Sagitta { get; set; }
            public double MidAngle { get; set; }
            public double MidX { get; set; }
            public double MidY { get; set; }
            public double MidZ { get; set; }
            
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

        public static ArcData Render(Arc arc)
        {
            float startAngle = (float)(arc.StartAngle * 180 / Math.PI);
            float sweepAngle = (float)((arc.EndAngle - arc.StartAngle) * 180 / Math.PI);

            // 计算起点和终点的坐标
            double startX = arc.Center.X + arc.Radius * Math.Cos(arc.StartAngle);
            double startY = arc.Center.Y + arc.Radius * Math.Sin(arc.StartAngle);
            double endX = arc.Center.X + arc.Radius * Math.Cos(arc.EndAngle);
            double endY = arc.Center.Y + arc.Radius * Math.Sin(arc.EndAngle);

            // 获取Z坐标（AutoCAD中Arc可能有Z坐标）
            double centerZ = arc.Center.Z;
            double startZ = centerZ;
            double endZ = centerZ;

            // 获取厚度（如果有）
            double thickness = arc.Thickness;

            // 获取法线向量（默认为Z轴正方向）
            double normalX = arc.Normal.X;
            double normalY = arc.Normal.Y;
            double normalZ = arc.Normal.Z;

            // 判断是否逆时针绘制
            bool isCCW = arc.EndAngle > arc.StartAngle;
            
            // 计算弧长
            double arcLength = arc.Radius * Math.Abs(arc.EndAngle - arc.StartAngle);
            
            // 计算扇形面积
            double sectorArea = 0.5 * arc.Radius * arc.Radius * Math.Abs(arc.EndAngle - arc.StartAngle);
            
            // 计算弦长
            double chordLength = Math.Sqrt(Math.Pow(endX - startX, 2) + Math.Pow(endY - startY, 2));
            
            // 计算矢高（弧到弦的最大距离）
            double sagitta = arc.Radius - Math.Sqrt(Math.Pow(arc.Radius, 2) - Math.Pow(chordLength / 2, 2));
            
            // 计算中点角度和坐标
            double midAngle = (arc.StartAngle + arc.EndAngle) / 2;
            double midX = arc.Center.X + arc.Radius * Math.Cos(midAngle);
            double midY = arc.Center.Y + arc.Radius * Math.Sin(midAngle);
            double midZ = centerZ;

            // 生成用于绘制的点列表（3D点）
            var points = new List<Point3DData>();
            var vertices = new List<double>();
            int segments = 64;
            for (int i = 0; i <= segments; i++)
            {
                double angle = arc.StartAngle + (arc.EndAngle - arc.StartAngle) * i / segments;
                double x = arc.Center.X + arc.Radius * Math.Cos(angle);
                double y = arc.Center.Y + arc.Radius * Math.Sin(angle);
                double z = centerZ;
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

            // 计算边界框
            var bounds = CalculateBounds(arc.Center.X, arc.Center.Y, centerZ, arc.Radius, arc.StartAngle, arc.EndAngle);
            
            // 计算质心
            var centroid = CalculateCentroid(arc.Center.X, arc.Center.Y, centerZ, arc.Radius, arc.StartAngle, arc.EndAngle);

            // 获取颜色信息
            var colorInfo = GetColorInfo(arc.Color.Index);
            
            // 获取透明度
            double transparency = 1.0;
            if (arc.Transparency.IsByLayer)
            {
                transparency = 0.0;
            }
            else if (arc.Transparency.IsByBlock)
            {
                transparency = 0.0;
            }
            else
            {
                transparency = arc.Transparency.Value / 100.0;
            }
            double opacity = 1.0 - transparency;

            // 获取实体属性
            string handle = arc.Handle.ToString();
            string layerName = arc.Layer?.Name ?? "0";
            bool isInvisible = arc.IsInvisible;
            double lineTypeScale = arc.LineTypeScale;

            // 创建变换矩阵
            var transform = new TransformData();
            if (normalX != 0 || normalY != 0 || normalZ != 1)
            {
                transform = CreateTransformFromNormal(normalX, normalY, normalZ, arc.Center.X, arc.Center.Y, arc.Center.Z);
            }

            var arcData = new ArcData
            {
                // three.js 对象类型标识
                Type = "Arc",
                Uuid = Guid.NewGuid().ToString(),
                EntityType = "ARC",
                
                // 基本几何属性
                CenterX = arc.Center.X,
                CenterY = arc.Center.Y,
                CenterZ = centerZ,
                Radius = arc.Radius,
                StartAngle = arc.StartAngle,
                EndAngle = arc.EndAngle,
                Thickness = thickness,
                Sweep = arc.Sweep,
                
                // 起点和终点坐标
                StartX = startX,
                StartY = startY,
                StartZ = startZ,
                EndX = endX,
                EndY = endY,
                EndZ = endZ,
                
                // 法线向量
                NormalX = normalX,
                NormalY = normalY,
                NormalZ = normalZ,
                
                // 实体属性
                Handle = handle,
                LayerName = layerName,
                IsInvisible = isInvisible,
                LineTypeScale = lineTypeScale,
                Transparency = transparency,
                
                // 样式属性
                ColorIndex = arc.Color.Index,
                ColorHex = colorInfo.Hex,
                ColorR = colorInfo.R,
                ColorG = colorInfo.G,
                ColorB = colorInfo.B,
                ColorA = (int)(opacity * 255),
                LineTypeName = arc.GetActiveLineType().Name,
                LineWeight = arc.GetActiveLineWeightType().GetLineWeightValue(),
                
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
                
                // 弧形计算属性
                IsCCW = isCCW,
                Length = arcLength,
                Area = sectorArea,
                ChordLength = chordLength,
                Sagitta = sagitta,
                MidAngle = midAngle,
                MidX = midX,
                MidY = midY,
                MidZ = midZ,
                
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

            return arcData;
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

        // 计算弧形的边界框
        private static BoundsData CalculateBounds(double centerX, double centerY, double centerZ, double radius, double startAngle, double endAngle)
        {
            double minX = centerX - radius;
            double maxX = centerX + radius;
            double minY = centerY - radius;
            double maxY = centerY + radius;
            double minZ = centerZ;
            double maxZ = centerZ;

            // 如果弧形不包含整个圆，需要更精确的边界计算
            if (Math.Abs(endAngle - startAngle) < 2 * Math.PI)
            {
                // 检查弧形是否包含极值点
                bool containsLeft = false, containsRight = false, containsTop = false, containsBottom = false;
                
                // 标准化角度到[0, 2π]范围
                double normalizedStart = NormalizeAngle(startAngle);
                double normalizedEnd = NormalizeAngle(endAngle);
                
                // 如果起始角度大于结束角度，说明跨越了0度
                if (normalizedStart > normalizedEnd)
                {
                    if (normalizedStart <= Math.PI || normalizedEnd >= Math.PI) containsLeft = true;
                    if (normalizedStart <= 0 || normalizedEnd >= 0) containsRight = true;
                    if (normalizedStart <= Math.PI / 2 || normalizedEnd >= Math.PI / 2) containsTop = true;
                    if (normalizedStart <= 3 * Math.PI / 2 || normalizedEnd >= 3 * Math.PI / 2) containsBottom = true;
                }
                else
                {
                    if (normalizedStart <= Math.PI && normalizedEnd >= Math.PI) containsLeft = true;
                    if (normalizedStart <= 0 && normalizedEnd >= 0) containsRight = true;
                    if (normalizedStart <= Math.PI / 2 && normalizedEnd >= Math.PI / 2) containsTop = true;
                    if (normalizedStart <= 3 * Math.PI / 2 && normalizedEnd >= 3 * Math.PI / 2) containsBottom = true;
                }
                
                // 如果不包含极值点，则边界由起点和终点决定
                if (!containsLeft) minX = Math.Min(minX, centerX + radius * Math.Cos(startAngle));
                if (!containsLeft) minX = Math.Min(minX, centerX + radius * Math.Cos(endAngle));
                if (!containsRight) maxX = Math.Max(maxX, centerX + radius * Math.Cos(startAngle));
                if (!containsRight) maxX = Math.Max(maxX, centerX + radius * Math.Cos(endAngle));
                if (!containsTop) maxY = Math.Max(maxY, centerY + radius * Math.Sin(startAngle));
                if (!containsTop) maxY = Math.Max(maxY, centerY + radius * Math.Sin(endAngle));
                if (!containsBottom) minY = Math.Min(minY, centerY + radius * Math.Sin(startAngle));
                if (!containsBottom) minY = Math.Min(minY, centerY + radius * Math.Sin(endAngle));
            }

            return new BoundsData(
                new Point3DData(minX, minY, minZ),
                new Point3DData(maxX, maxY, maxZ)
            );
        }

        // 计算弧形的质心
        private static Point3DData CalculateCentroid(double centerX, double centerY, double centerZ, double radius, double startAngle, double endAngle)
        {
            // 对于弧形，质心位于弧形的中点
            double midAngle = (startAngle + endAngle) / 2;
            double midX = centerX + radius * Math.Cos(midAngle);
            double midY = centerY + radius * Math.Sin(midAngle);
            
            return new Point3DData(midX, midY, centerZ);
        }

        // 获取颜色信息（包括RGB值和十六进制值）
        private static (string Hex, int R, int G, int B) GetColorInfo(short colorIndex)
        {
            ColorData color = GetColorByIndex(colorIndex);
            return (
                $"#{color.R:X2}{color.G:X2}{color.B:X2}",
                color.R,
                color.G,
                color.B
            );
        }

        // 标准化角度到[0, 2π]范围
        private static double NormalizeAngle(double angle)
        {
            while (angle < 0) angle += 2 * Math.PI;
            while (angle >= 2 * Math.PI) angle -= 2 * Math.PI;
            return angle;
        }

        // 根据法线向量创建变换矩阵（用于three.js）
        private static TransformData CreateTransformFromNormal(double nx, double ny, double nz, double cx, double cy, double cz)
        {
            var transform = new TransformData();
            
            // 归一化法线向量
            double length = Math.Sqrt(nx * nx + ny * ny + nz * nz);
            if (length > 0)
            {
                nx /= length;
                ny /= length;
                nz /= length;
            }
            
            // 如果法线向量接近Z轴，使用简单的平移变换
            if (Math.Abs(nx) < 1e-10 && Math.Abs(ny) < 1e-10)
            {
                transform.Matrix[12] = cx;
                transform.Matrix[13] = cy;
                transform.Matrix[14] = cz;
                return transform;
            }
            
            // 计算旋转角度和旋转轴
            double upX = 0, upY = 0, upZ = 1;
            double dot = nz;
            double angle = Math.Acos(Math.Max(-1, Math.Min(1, dot)));
            
            // 计算旋转轴（叉积）
            double axisX = upY * nz - upZ * ny;
            double axisY = upZ * nx - upX * nz;
            double axisZ = upX * ny - upY * nx;
            
            // 归一化旋转轴
            double axisLength = Math.Sqrt(axisX * axisX + axisY * axisY + axisZ * axisZ);
            if (axisLength > 1e-10)
            {
                axisX /= axisLength;
                axisY /= axisLength;
                axisZ /= axisLength;
            }
            
            // 使用罗德里格斯旋转公式创建旋转矩阵
            double c = Math.Cos(angle);
            double s = Math.Sin(angle);
            double t = 1 - c;
            
            // 旋转矩阵
            double r00 = t * axisX * axisX + c;
            double r01 = t * axisX * axisY - s * axisZ;
            double r02 = t * axisX * axisZ + s * axisY;
            double r10 = t * axisX * axisY + s * axisZ;
            double r11 = t * axisY * axisY + c;
            double r12 = t * axisY * axisZ - s * axisX;
            double r20 = t * axisX * axisZ - s * axisY;
            double r21 = t * axisY * axisZ + s * axisX;
            double r22 = t * axisZ * axisZ + c;
            
            // 应用平移
            transform.Matrix[0] = r00;
            transform.Matrix[1] = r01;
            transform.Matrix[2] = r02;
            transform.Matrix[3] = 0;
            transform.Matrix[4] = r10;
            transform.Matrix[5] = r11;
            transform.Matrix[6] = r12;
            transform.Matrix[7] = 0;
            transform.Matrix[8] = r20;
            transform.Matrix[9] = r21;
            transform.Matrix[10] = r22;
            transform.Matrix[11] = 0;
            transform.Matrix[12] = cx;
            transform.Matrix[13] = cy;
            transform.Matrix[14] = cz;
            transform.Matrix[15] = 1;
            
            return transform;
        }
    }
}