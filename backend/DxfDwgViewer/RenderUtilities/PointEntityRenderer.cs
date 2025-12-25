using ACadSharp.Entities;
using ACadSharp.Extensions;
using System.Collections.Generic;

namespace DxfDwgViewer.RenderUtilities
{
    public class PointEntityRenderer
    {
        public class PointData
    {
        // 基本属性
        public Point3DData Location { get; set; }
        public short ColorIndex { get; set; }
        public string LineTypeName { get; set; }
        public double LineWeight { get; set; }
        public double Thickness { get; set; }
        public double Rotation { get; set; }
        public double Size { get; set; }
        
        // 基本几何属性
        public BoundsData3D Bounds3D { get; set; }
        public Point3DData Centroid3D { get; set; }
        
        // three.js兼容性属性
        public ColorData Color { get; set; }
        public NormalData Normal { get; set; }
        public TransformData Transform { get; set; }
        public string EntityType { get; set; }
        public bool Visible { get; set; }
        public int LayerIndex { get; set; }
        public string LayerName { get; set; }
        
        // three.js增强属性
        public string Type { get; set; }
        public string Handle { get; set; }
        public string CoordinateSystem { get; set; }
        public double Opacity { get; set; }
        public bool Transparent { get; set; }
        public bool DepthTest { get; set; }
        public PointMaterialData Material { get; set; }
        public PointGeometryData Geometry { get; set; }
    }
    
    public class PointMaterialData
    {
        public ColorData Color { get; set; }
        public double Opacity { get; set; }
        public bool Transparent { get; set; }
        public string Type { get; set; }
        public bool DepthTest { get; set; }
        public bool SizeAttenuation { get; set; }
        public double Size { get; set; }
    }
    
    public class PointGeometryData
    {
        public Point3DData Position { get; set; }
        public Point3DData Rotation { get; set; }
        public Point3DData Scale { get; set; }
        public NormalData Normal { get; set; }
        public BoundsData3D BoundingBox { get; set; }
        public string Type { get; set; }
        public double Size { get; set; }
    }

        public static PointData Render(ACadSharp.Entities.Point point)
        {
            double scale = 1.0;
            
            // 创建3D位置点
            var location3D = new Point3DData(point.Location.X, point.Location.Y, point.Location.Z);
            
            // 计算点的边界（点本身没有大小，但考虑显示大小）
            var pointSize = 6.0 / scale;
            var halfSize = pointSize / 2.0;
            var minBounds = new Point3DData(point.Location.X - halfSize, point.Location.Y - halfSize, point.Location.Z - halfSize);
            var maxBounds = new Point3DData(point.Location.X + halfSize, point.Location.Y + halfSize, point.Location.Z + halfSize);
            var bounds3D = new BoundsData3D(minBounds, maxBounds);
            
            // 创建颜色数据
            var colorData = new ColorData(point.Color.Index);
            
            // 创建法向量（使用点的实际法向量）
            var normal = new NormalData(point.Normal.X, point.Normal.Y, point.Normal.Z);
            
            // 创建变换矩阵（考虑点的旋转和位置）
            var transform = new TransformData(1.0, 1.0, 1.0, point.Rotation, point.Location.X, point.Location.Y, point.Location.Z);
            
            var pointData = new PointData
            {
                Location = location3D,
                ColorIndex = point.Color.Index,
                LineTypeName = point.GetActiveLineType()?.Name ?? "",
                LineWeight = point.GetActiveLineWeightType().GetLineWeightValue(),
                Thickness = point.Thickness,
                Rotation = point.Rotation,
                Size = pointSize,
                Bounds3D = bounds3D,
                Centroid3D = location3D,
                Color = colorData,
                Normal = normal,
                Transform = transform,
                EntityType = "Point",
                Visible = !point.IsInvisible,
                LayerIndex = 0,
                LayerName = point.Layer?.Name ?? "",
                
                // three.js增强属性
                Type = "Point",
                Handle = point.Handle.ToString(),
                CoordinateSystem = "WCS",
                Opacity = 1.0,
                Transparent = false,
                DepthTest = true,
                
                // 初始化材质和几何数据
                Material = new PointMaterialData
                {
                    Color = colorData,
                    Opacity = 1.0,
                    Transparent = false,
                    Type = "PointsMaterial",
                    DepthTest = true,
                    SizeAttenuation = true,
                    Size = pointSize
                },
                Geometry = new PointGeometryData
                {
                    Position = location3D,
                    Rotation = new Point3DData(0, 0, point.Rotation),
                    Scale = new Point3DData(1.0, 1.0, 1.0),
                    Normal = normal,
                    BoundingBox = bounds3D,
                    Type = "BufferGeometry",
                    Size = pointSize
                }
            };

            return pointData;
        }
    }
}