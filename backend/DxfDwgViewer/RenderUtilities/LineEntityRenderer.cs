using System;
using System.Collections.Generic;
using System.Linq;
using ACadSharp.Entities;
using ACadSharp.Extensions;

namespace DxfDwgViewer.RenderUtilities
{
    public class LineEntityRenderer
    {
        public class LineData
        {
            public double StartPointX { get; set; }
            public double StartPointY { get; set; }
            public double EndPointX { get; set; }
            public double EndPointY { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; } = "";
            public double LineWeight { get; set; }
            public double LineTypeScale { get; set; }
            
            public double Length { get; set; }
            public double Angle { get; set; }
            public double MidPointX { get; set; }
            public double MidPointY { get; set; }
            
            public string Type { get; set; } = "Line";
            public string EntityType { get; set; } = "Line";
            public string Handle { get; set; } = "";
            public string LayerName { get; set; } = "";
            public int LayerIndex { get; set; }
            public bool Visible { get; set; } = true;
            public string CoordinateSystem { get; set; } = "World";
            
            public Point3DData StartPoint3D { get; set; } = new Point3DData();
            public Point3DData EndPoint3D { get; set; } = new Point3DData();
            public Point3DData MidPoint3D { get; set; } = new Point3DData();
            public Point3DData Direction { get; set; } = new Point3DData();
            public BoundsData Bounds { get; set; } = new BoundsData();
            public ColorData Color { get; set; } = new ColorData();
            public TransformData Transform { get; set; } = new TransformData();
            public NormalData Normal { get; set; } = new NormalData();
            
            public double Opacity { get; set; } = 1.0;
            public bool Transparent { get; set; } = false;
            public string MaterialType { get; set; } = "LineBasicMaterial";
            public bool DepthTest { get; set; } = true;
            public bool DepthWrite { get; set; } = true;
            
            public double Thickness { get; set; }
        }

        public static LineData Render(Line line)
        {
            // 获取活动线型（考虑ByLayer和ByBlock的情况）
            var activeLineType = line.GetActiveLineType();
            
            // 计算线段长度
            double dx = line.EndPoint.X - line.StartPoint.X;
            double dy = line.EndPoint.Y - line.StartPoint.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);
            
            // 计算线段角度（弧度）
            double angle = Math.Atan2(dy, dx);
            
            // 计算中点坐标
            double midX = (line.StartPoint.X + line.EndPoint.X) / 2.0;
            double midY = (line.StartPoint.Y + line.EndPoint.Y) / 2.0;

            // 创建3D点数据
            var startPoint3D = new Point3DData(line.StartPoint.X, line.StartPoint.Y, line.StartPoint.Z);
            var endPoint3D = new Point3DData(line.EndPoint.X, line.EndPoint.Y, line.EndPoint.Z);
            var midPoint3D = new Point3DData(midX, midY, (line.StartPoint.Z + line.EndPoint.Z) / 2.0);
            
            // 创建颜色数据
            var colorData = new ColorData(line.Color.Index);
            
            // 创建边界框数据
            var minPoint = new Point3DData(
                Math.Min(line.StartPoint.X, line.EndPoint.X),
                Math.Min(line.StartPoint.Y, line.EndPoint.Y),
                Math.Min(line.StartPoint.Z, line.EndPoint.Z));
            var maxPoint = new Point3DData(
                Math.Max(line.StartPoint.X, line.EndPoint.X),
                Math.Max(line.StartPoint.Y, line.EndPoint.Y),
                Math.Max(line.StartPoint.Z, line.EndPoint.Z));
            var boundsData = new BoundsData(minPoint, maxPoint);
            
            // 创建法向量（使用Line的法向量）
            var normalData = new NormalData(line.Normal.X, line.Normal.Y, line.Normal.Z);
            
            // 创建方向向量（归一化）
            double dirX = line.EndPoint.X - line.StartPoint.X;
            double dirY = line.EndPoint.Y - line.StartPoint.Y;
            double dirZ = line.EndPoint.Z - line.StartPoint.Z;
            double dirLength = Math.Sqrt(dirX * dirX + dirY * dirY + dirZ * dirZ);
            if (dirLength > 0)
            {
                dirX /= dirLength;
                dirY /= dirLength;
                dirZ /= dirLength;
            }
            var directionData = new Point3DData(dirX, dirY, dirZ);
            
            // 创建变换数据（默认为单位矩阵）
            var transformData = new TransformData();

            var lineData = new LineData
            {
                // 基本线条属性
                StartPointX = line.StartPoint.X,
                StartPointY = line.StartPoint.Y,
                EndPointX = line.EndPoint.X,
                EndPointY = line.EndPoint.Y,
                ColorIndex = line.Color.Index,
                LineTypeName = line.GetActiveLineType()?.Name ?? string.Empty,
                LineWeight = line.GetActiveLineWeightType().GetLineWeightValue(),
                LineTypeScale = line.LineTypeScale,
                
                // 基本几何属性
                Length = length,
                Angle = angle,
                MidPointX = midX,
                MidPointY = midY,
                
                // three.js兼容性扩展属性
                Type = "Line",
                EntityType = "Line",
                Handle = line.Handle.ToString(),
                LayerName = line.Layer?.Name ?? string.Empty,
                LayerIndex = 0, // ACadSharp.Layer没有Index属性，使用默认值
                Visible = !line.IsInvisible,
                CoordinateSystem = "World",
                
                // 3D坐标和几何信息
                StartPoint3D = startPoint3D,
                EndPoint3D = endPoint3D,
                MidPoint3D = midPoint3D,
                Direction = directionData,
                Bounds = boundsData,
                Color = colorData,
                Transform = transformData,
                Normal = normalData,
                
                // 材质属性
                Opacity = 1.0,
                Transparent = false,
                MaterialType = "LineBasicMaterial",
                DepthTest = true,
                DepthWrite = true,
                
                // Line特有属性
                Thickness = line.Thickness
            };

            return lineData;
        }
    }
}