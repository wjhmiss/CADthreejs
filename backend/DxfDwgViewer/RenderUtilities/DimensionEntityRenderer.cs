using System;
using System.Linq;
using ACadSharp.Entities;
using ACadSharp.Extensions;
using System.Collections.Generic;
using CSMath;

namespace DxfDwgViewer.RenderUtilities
{
    // 颜色信息类，支持多种格式
    public class ColorInfo
    {
        public short Index { get; set; }
        public string Hex { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }

    public class DimensionEntityRenderer
    {
        public class DimensionData
        {
            // 基本属性
            public string Type { get; set; }
            
            // 3D坐标支持
            public double DefinitionPointX { get; set; }
            public double DefinitionPointY { get; set; }
            public double DefinitionPointZ { get; set; } // 添加Z坐标
            
            // 法线向量（用于3D渲染）
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            
            // 多格式颜色支持
            public short ColorIndex { get; set; }
            public string ColorHex { get; set; } // 添加十六进制颜色值
            public int ColorR { get; set; } // 添加RGB颜色分量
            public int ColorG { get; set; }
            public int ColorB { get; set; }
            
            // 样式属性
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            
            // 子实体数据
            public EntityData[] Entities { get; set; }
            
            // 特定类型的标注数据
            public Angular3PtData Angular3PtData { get; set; }
            public Angular2LineData Angular2LineData { get; set; }
            public DiameterData DiameterData { get; set; }
            public RadiusData RadiusData { get; set; }
            public OrdinateData OrdinateData { get; set; }
            public AlignedData AlignedData { get; set; }
            public LinearData LinearData { get; set; }
            
            // three.js友好属性
            public BoundsData Bounds { get; set; } // 边界框
            public Point3DData Centroid { get; set; } // 质心
            public Point3DData[] Points { get; set; } // 用于绘制的点列表
            
            // 坐标系信息
            public string CoordinateSystem { get; set; } // 坐标系标识
            public bool RequiresYAxisFlip { get; set; } // 是否需要Y轴翻转
        }

        public class EntityData
        {
            public string Type { get; set; }
            public LineData LineData { get; set; }
            public TextData TextData { get; set; }
            public MTextData MTextData { get; set; }
            public SolidData SolidData { get; set; }
            public DimensionPointData PointData { get; set; }
            // 添加3D支持
            public Point3DData Position { get; set; } // 实体位置
            public BoundsData Bounds { get; set; } // 实体边界框
        }

        public class LineData
        {
            // 2D坐标（保持向后兼容）
            public double StartX { get; set; }
            public double StartY { get; set; }
            public double EndX { get; set; }
            public double EndY { get; set; }
            
            // 3D坐标支持
            public double StartZ { get; set; }
            public double EndZ { get; set; }
            
            // 法线向量（用于3D渲染）
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            
            // 颜色信息
            public short ColorIndex { get; set; }
            public string ColorHex { get; set; } // 添加十六进制颜色值
            public int ColorR { get; set; } // 添加RGB颜色分量
            public int ColorG { get; set; }
            public int ColorB { get; set; }
            
            // 样式属性
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            
            // three.js友好属性
            public BoundsData Bounds { get; set; } // 线段边界框
            public Point3DData MidPoint { get; set; } // 线段中点
            public double Length { get; set; } // 线段长度
        }

        public class TextData
        {
            // 文本内容
            public string Value { get; set; }
            
            // 2D位置（保持向后兼容）
            public double PositionX { get; set; }
            public double PositionY { get; set; }
            
            // 3D位置支持
            public double PositionZ { get; set; }
            
            // 法线向量（用于3D渲染）
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            
            // 方向向量（文本方向）
            public double DirectionX { get; set; }
            public double DirectionY { get; set; }
            public double DirectionZ { get; set; }
            
            // 颜色信息
            public short ColorIndex { get; set; }
            public string ColorHex { get; set; } // 添加十六进制颜色值
            public int ColorR { get; set; } // 添加RGB颜色分量
            public int ColorG { get; set; }
            public int ColorB { get; set; }
            
            // 样式属性
            public string Style { get; set; }
            public double Height { get; set; } // 文本高度
            public double Rotation { get; set; } // 旋转角度
            
            // three.js友好属性
            public BoundsData Bounds { get; set; } // 文本边界框
            public double Width { get; set; } // 文本宽度估算
        }

        public class MTextData
        {
            // 文本内容
            public string Value { get; set; }
            
            // 2D位置（保持向后兼容）
            public double PositionX { get; set; }
            public double PositionY { get; set; }
            
            // 3D位置支持
            public double PositionZ { get; set; }
            
            // 法线向量（用于3D渲染）
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            
            // 方向向量（文本方向）
            public double DirectionX { get; set; }
            public double DirectionY { get; set; }
            public double DirectionZ { get; set; }
            
            // 颜色信息
            public short ColorIndex { get; set; }
            public string ColorHex { get; set; } // 添加十六进制颜色值
            public int ColorR { get; set; } // 添加RGB颜色分量
            public int ColorG { get; set; }
            public int ColorB { get; set; }
            
            // 样式属性
            public string Style { get; set; }
            public double Height { get; set; } // 文本高度
            public double Width { get; set; } // 文本宽度
            public double Rotation { get; set; } // 旋转角度
            public double LineSpacing { get; set; } // 行间距
            
            // three.js友好属性
            public BoundsData Bounds { get; set; } // 文本边界框
            public string[] Lines { get; set; } // 分割后的文本行
        }

        public class SolidData
        {
            public Point3DData[] Points { get; set; }
            
            // 颜色信息
            public short ColorIndex { get; set; }
            public string ColorHex { get; set; } // 添加十六进制颜色值
            public int ColorR { get; set; } // 添加RGB颜色分量
            public int ColorG { get; set; }
            public int ColorB { get; set; }
            
            // 3D属性
            public double NormalX { get; set; } // 法线向量
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            
            // three.js友好属性
            public BoundsData Bounds { get; set; } // 实体边界框
            public Point3DData Centroid { get; set; } // 质心
            public double Area { get; set; } // 面积
        }

        public class DimensionPointData
        {
            // 2D坐标（保持向后兼容）
            public double X { get; set; }
            public double Y { get; set; }
            
            // 3D坐标支持
            public double Z { get; set; }
            
            // 颜色信息
            public short ColorIndex { get; set; }
            public string ColorHex { get; set; } // 添加十六进制颜色值
            public int ColorR { get; set; } // 添加RGB颜色分量
            public int ColorG { get; set; }
            public int ColorB { get; set; }
            
            // 点样式
            public int PointStyle { get; set; } // 点样式标识
            public double Size { get; set; } // 点大小
            
            // three.js友好属性
            public BoundsData Bounds { get; set; } // 点边界框（通常是一个小立方体）
        }

        public class Angular3PtData
        {
            // 2D坐标（保持向后兼容）
            public double VertexX { get; set; }
            public double VertexY { get; set; }
            public double FirstPointX { get; set; }
            public double FirstPointY { get; set; }
            public double SecondPointX { get; set; }
            public double SecondPointY { get; set; }
            
            // 3D坐标支持
            public double VertexZ { get; set; }
            public double FirstPointZ { get; set; }
            public double SecondPointZ { get; set; }
            
            // 法线向量（用于3D渲染）
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            
            // 测量属性
            public double Measurement { get; set; }
            public string Text { get; set; }
            
            // three.js友好的属性
            public double AngleInDegrees { get; set; }
            public double AngleInRadians { get; set; } // 添加弧度值
            public Point3DData ArcCenter { get; set; } // 弧心点
            public double Radius { get; set; } // 弧半径
            public BoundsData Bounds { get; set; } // 边界框
            public Point3DData[] ArcPoints { get; set; } // 弧上的点（用于渲染）
        }

        public class Angular2LineData
        {
            // 2D坐标（保持向后兼容）
            public double AngleVertexX { get; set; }
            public double AngleVertexY { get; set; }
            public double FirstPointX { get; set; }
            public double FirstPointY { get; set; }
            public double SecondPointX { get; set; }
            public double SecondPointY { get; set; }
            public double DefinitionPointX { get; set; }
            public double DefinitionPointY { get; set; }
            
            // 3D坐标支持
            public double AngleVertexZ { get; set; }
            public double FirstPointZ { get; set; }
            public double SecondPointZ { get; set; }
            public double DefinitionPointZ { get; set; }
            
            // 法线向量（用于3D渲染）
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            
            // 测量属性
            public double Measurement { get; set; }
            public string Text { get; set; }
            
            // three.js友好的属性
            public double AngleInDegrees { get; set; }
            public double AngleInRadians { get; set; } // 添加弧度值
            public Point3DData ArcCenter { get; set; } // 弧心点
            public double Radius { get; set; } // 弧半径
            public BoundsData Bounds { get; set; } // 边界框
            public Point3DData[] ArcPoints { get; set; } // 弧上的点（用于渲染）
            public Point3DData FirstLineDirection { get; set; } // 第一条线的方向向量
            public Point3DData SecondLineDirection { get; set; } // 第二条线的方向向量
        }

        public class DiameterData
        {
            // 2D坐标（保持向后兼容）
            public double AngleVertexX { get; set; }
            public double AngleVertexY { get; set; }
            public double DefinitionPointX { get; set; }
            public double DefinitionPointY { get; set; }
            public double CenterX { get; set; }
            public double CenterY { get; set; }
            
            // 3D坐标支持
            public double AngleVertexZ { get; set; }
            public double DefinitionPointZ { get; set; }
            public double CenterZ { get; set; }
            
            // 法线向量（用于3D渲染）
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            
            // 测量属性
            public double Measurement { get; set; }
            public string Text { get; set; }
            
            // three.js友好的属性
            public double Radius { get; set; }
            public double Diameter { get; set; } // 添加直径值
            public BoundsData Bounds { get; set; } // 边界框
            public Point3DData Centroid { get; set; } // 质心
            public Point3DData[] CirclePoints { get; set; } // 圆上的点（用于渲染）
            public double StartAngle { get; set; } // 起始角度
            public double EndAngle { get; set; } // 结束角度
        }

        public class RadiusData
        {
            // 2D坐标（保持向后兼容）
            public double DefinitionPointX { get; set; }
            public double DefinitionPointY { get; set; }
            public double AngleVertexX { get; set; }
            public double AngleVertexY { get; set; }
            
            // 3D坐标支持
            public double DefinitionPointZ { get; set; }
            public double AngleVertexZ { get; set; }
            public double CenterZ { get; set; } // 圆心Z坐标
            
            // 法线向量（用于3D渲染）
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            
            // 测量属性
            public double Measurement { get; set; }
            public string Text { get; set; }
            public double LeaderLength { get; set; }
            
            // three.js友好的属性
            public double Radius { get; set; }
            public Point3DData Center { get; set; } // 圆心点
            public BoundsData Bounds { get; set; } // 边界框
            public Point3DData Centroid { get; set; } // 质心
            public Point3DData[] ArcPoints { get; set; } // 弧上的点（用于渲染）
            public double StartAngle { get; set; } // 起始角度
            public double EndAngle { get; set; } // 结束角度
            public Point3DData LeaderDirection { get; set; } // 引线方向向量
        }

        public class OrdinateData
        {
            // 2D坐标（保持向后兼容）
            public double FeatureLocationX { get; set; }
            public double FeatureLocationY { get; set; }
            public double LeaderEndpointX { get; set; }
            public double LeaderEndpointY { get; set; }
            
            // 3D坐标支持
            public double FeatureLocationZ { get; set; }
            public double LeaderEndpointZ { get; set; }
            
            // 法线向量（用于3D渲染）
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            
            // 测量属性
            public bool IsOrdinateTypeX { get; set; }
            public double Measurement { get; set; }
            public string Text { get; set; }
            
            // three.js友好的属性
            public BoundsData Bounds { get; set; } // 边界框
            public Point3DData Centroid { get; set; } // 质心
            public Point3DData FeatureLocation { get; set; } // 特征位置3D点
            public Point3DData LeaderEndpoint { get; set; } // 引线端点3D点
            public Point3DData LeaderDirection { get; set; } // 引线方向向量
            public double LeaderLength { get; set; } // 引线长度
        }

        public class AlignedData
        {
            // 2D坐标（保持向后兼容）
            public double FirstPointX { get; set; }
            public double FirstPointY { get; set; }
            public double SecondPointX { get; set; }
            public double SecondPointY { get; set; }
            public double DefinitionPointX { get; set; }
            public double DefinitionPointY { get; set; }
            
            // 3D坐标支持
            public double FirstPointZ { get; set; }
            public double SecondPointZ { get; set; }
            public double DefinitionPointZ { get; set; }
            
            // 法线向量（用于3D渲染）
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            
            // 测量属性
            public double Measurement { get; set; }
            public string Text { get; set; }
            public double ExtLineRotation { get; set; }
            
            // three.js友好的属性
            public BoundsData Bounds { get; set; } // 边界框
            public Point3DData Centroid { get; set; } // 质心
            public Point3DData FirstPoint { get; set; } // 第一个点3D坐标
            public Point3DData SecondPoint { get; set; } // 第二个点3D坐标
            public Point3DData DefinitionPoint { get; set; } // 定义点3D坐标
            public double Length { get; set; } // 线段长度
            public Point3DData Direction { get; set; } // 方向向量
        }

        public class LinearData
        {
            // 2D坐标（保持向后兼容）
            public double DefinitionPointX { get; set; }
            public double DefinitionPointY { get; set; }
            public double FirstPointX { get; set; }
            public double FirstPointY { get; set; }
            public double SecondPointX { get; set; }
            public double SecondPointY { get; set; }
            
            // 3D坐标支持
            public double DefinitionPointZ { get; set; }
            public double FirstPointZ { get; set; }
            public double SecondPointZ { get; set; }
            
            // 法线向量（用于3D渲染）
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            
            // 测量属性
            public double Measurement { get; set; }
            public string Text { get; set; }
            public double Rotation { get; set; }
            public double ExtLineRotation { get; set; }
            
            // three.js友好的属性
            public BoundsData Bounds { get; set; } // 边界框
            public Point3DData Centroid { get; set; } // 质心
            public Point3DData DefinitionPoint { get; set; } // 定义点3D坐标
            public Point3DData FirstPoint { get; set; } // 第一个点3D坐标
            public Point3DData SecondPoint { get; set; } // 第二个点3D坐标
            public double Length { get; set; } // 线段长度
            public Point3DData Direction { get; set; } // 方向向量
            public double RotationInRadians { get; set; } // 旋转角度（弧度）
            public double ExtLineRotationInRadians { get; set; } // 延伸线旋转角度（弧度）
        }

        public static DimensionData Render(Dimension dimension)
        {
            Console.WriteLine($"DimensionEntityRenderer.Render被调用，类型: {dimension.GetType().Name}");
            // 获取3D坐标和法线向量
            XYZ definitionPoint = dimension.DefinitionPoint;
            XYZ normal = dimension.Normal;
            
            // 创建颜色信息
            var colorInfo = CreateColorInfo(dimension.Color.Index);
            
            var dimensionData = new DimensionData
            {
                Type = dimension.GetType().Name,
                DefinitionPointX = definitionPoint.X,
                DefinitionPointY = definitionPoint.Y,
                DefinitionPointZ = definitionPoint.Z, // 添加Z坐标
                
                // 法线向量
                NormalX = normal.X,
                NormalY = normal.Y,
                NormalZ = normal.Z,
                
                // 颜色信息
                ColorIndex = dimension.Color.Index,
                ColorHex = colorInfo.Hex,
                ColorR = colorInfo.R,
                ColorG = colorInfo.G,
                ColorB = colorInfo.B,
                
                // 样式属性
                LineTypeName = dimension.GetActiveLineType().Name,
                LineWeight = dimension.GetActiveLineWeightType().GetLineWeightValue(),
                
                // 初始化three.js友好属性
                Bounds = new BoundsData(new Point3DData(0, 0, 0), new Point3DData(0, 0, 0)),
                Centroid = new Point3DData(0, 0, 0),
                Points = new Point3DData[0],
                
                // 坐标系信息
                CoordinateSystem = "AutoCAD",
                RequiresYAxisFlip = true, // AutoCAD Y轴方向与three.js相反
                
                Entities = new EntityData[0]
            };

            try
            {
                // 特殊处理DimensionAngular3Pt类型
                if (dimension is DimensionAngular3Pt angular3Pt)
                {
                    // 获取3D坐标
                    var vertex = angular3Pt.AngleVertex;
                    var firstPoint = angular3Pt.FirstPoint;
                    var secondPoint = angular3Pt.SecondPoint;
                    
                    // 获取法线向量
                    var angularNormal = angular3Pt.Normal;
                    
                    // 创建3D点数据
                    var vertex3D = new Point3DData(vertex.X, vertex.Y, vertex.Z);
                    var firstPoint3D = new Point3DData(firstPoint.X, firstPoint.Y, firstPoint.Z);
                    var secondPoint3D = new Point3DData(secondPoint.X, secondPoint.Y, secondPoint.Z);
                    
                    // 计算圆弧中心和半径
                    var arcCenter = CalculateArcCenter(vertex3D, firstPoint3D, secondPoint3D);
                    var angularRadius = CalculateDistance(arcCenter, vertex3D);
                    
                    // 计算角度
                    var angle1 = Math.Atan2(vertex.Y - arcCenter.Y, vertex.X - arcCenter.X);
                    var angle2 = Math.Atan2(firstPoint.Y - arcCenter.Y, firstPoint.X - arcCenter.X);
                    var angle3 = Math.Atan2(secondPoint.Y - arcCenter.Y, secondPoint.X - arcCenter.X);
                    
                    // 确保角度在0-2π范围内
                    if (angle1 < 0) angle1 += 2 * Math.PI;
                    if (angle2 < 0) angle2 += 2 * Math.PI;
                    if (angle3 < 0) angle3 += 2 * Math.PI;
                    
                    // 计算起始和结束角度
                    var startAngle = Math.Min(Math.Min(angle1, angle2), angle3);
                    var endAngle = Math.Max(Math.Max(angle1, angle2), angle3);
                    
                    // 生成圆弧上的点
                    var arcPoints = GenerateArcPoints(arcCenter, angularRadius, startAngle, endAngle);
                    
                    // 计算边界框和质心
                    var allPoints = new List<Point3DData> { vertex3D, firstPoint3D, secondPoint3D };
                    allPoints.AddRange(arcPoints);
                    var bounds = CalculateBounds(allPoints);
                    var centroid = CalculateCentroid(allPoints);
                    
                    dimensionData.Angular3PtData = new Angular3PtData
                    {
                        // 2D坐标（保持向后兼容）
                        VertexX = vertex.X,
                        VertexY = vertex.Y,
                        FirstPointX = firstPoint.X,
                        FirstPointY = firstPoint.Y,
                        SecondPointX = secondPoint.X,
                        SecondPointY = secondPoint.Y,
                        
                        // 3D坐标支持
                        VertexZ = vertex.Z,
                        FirstPointZ = firstPoint.Z,
                        SecondPointZ = secondPoint.Z,
                        
                        // 法线向量（用于3D渲染）
                        NormalX = angularNormal.X,
                        NormalY = angularNormal.Y,
                        NormalZ = angularNormal.Z,
                        
                        // 测量属性
                        Measurement = angular3Pt.Measurement,
                        Text = angular3Pt.Text,
                        
                        // three.js友好的属性
                        AngleInDegrees = angular3Pt.Measurement * 180 / Math.PI,
                        AngleInRadians = angular3Pt.Measurement,
                        ArcCenter = arcCenter,
                        Radius = angularRadius,
                        Bounds = bounds,
                        ArcPoints = arcPoints.ToArray()
                    };
                    
                    // 更新DimensionData的几何属性
                    dimensionData.Bounds = bounds;
                    dimensionData.Centroid = centroid;
                    dimensionData.Points = allPoints.ToArray();
                    
                    return dimensionData;
                }

                // 特殊处理DimensionAngular2Line类型
                if (dimension is DimensionAngular2Line angular2Line)
                {
                    // 获取3D坐标
                    var angleVertex = angular2Line.AngleVertex;
                    var firstPoint = angular2Line.FirstPoint;
                    var secondPoint = angular2Line.SecondPoint;
                    var angularDefPoint = angular2Line.DefinitionPoint;
                    
                    // 获取法线向量
                    var angular2LineNormal = angular2Line.Normal;
                    
                    // 创建3D点数据
                    var angleVertex3D = new Point3DData(angleVertex.X, angleVertex.Y, angleVertex.Z);
                    var firstPoint3D = new Point3DData(firstPoint.X, firstPoint.Y, firstPoint.Z);
                    var secondPoint3D = new Point3DData(secondPoint.X, secondPoint.Y, secondPoint.Z);
                    var angularDefPoint3D = new Point3DData(angularDefPoint.X, angularDefPoint.Y, angularDefPoint.Z);
                    
                    // 计算角度（与RenderAngular2LineDimension中一致）
                    double angle1 = Math.Atan2(firstPoint.Y - angleVertex.Y, firstPoint.X - angleVertex.X);
                    double angle2 = Math.Atan2(secondPoint.Y - angularDefPoint.Y, secondPoint.X - angularDefPoint.X);
                    
                    if (angle1 < 0) angle1 += 2 * Math.PI;
                    if (angle2 < 0) angle2 += 2 * Math.PI;
                    
                    double deltaAngle = Math.Abs(angle1 - angle2);
                    if (deltaAngle > Math.PI)
                    {
                        deltaAngle = 2 * Math.PI - deltaAngle;
                    }
                    
                    double angleInDegrees = deltaAngle * 180 / Math.PI;
                    
                    // 计算圆弧中心和半径
                    var arcCenter = CalculateArcCenter(angleVertex3D, firstPoint3D, secondPoint3D);
                    var angular2LineRadius = CalculateDistance(arcCenter, angleVertex3D);
                    
                    // 计算起始和结束角度
                    var startAngle = Math.Min(angle1, angle2);
                    var endAngle = Math.Max(angle1, angle2);
                    
                    // 生成圆弧上的点
                    var arcPoints = GenerateArcPoints(arcCenter, angular2LineRadius, startAngle, endAngle);
                    
                    // 计算方向向量
                    var firstLineDirection = CalculateDirection(angleVertex3D, firstPoint3D);
                    var secondLineDirection = CalculateDirection(angularDefPoint3D, secondPoint3D);
                    
                    // 计算边界框和质心
                    var allPoints = new List<Point3DData> { angleVertex3D, firstPoint3D, secondPoint3D, angularDefPoint3D };
                    allPoints.AddRange(arcPoints);
                    var bounds = CalculateBounds(allPoints);
                    var centroid = CalculateCentroid(allPoints);

                    dimensionData.Angular2LineData = new Angular2LineData
                    {
                        // 2D坐标（保持向后兼容）
                        AngleVertexX = angleVertex.X,
                        AngleVertexY = angleVertex.Y,
                        FirstPointX = firstPoint.X,
                        FirstPointY = firstPoint.Y,
                        SecondPointX = secondPoint.X,
                        SecondPointY = secondPoint.Y,
                        DefinitionPointX = angularDefPoint.X,
                        DefinitionPointY = angularDefPoint.Y,
                        
                        // 3D坐标支持
                        AngleVertexZ = angleVertex.Z,
                        FirstPointZ = firstPoint.Z,
                        SecondPointZ = secondPoint.Z,
                        DefinitionPointZ = angularDefPoint.Z,
                        
                        // 法线向量（用于3D渲染）
                        NormalX = angular2LineNormal.X,
                        NormalY = angular2LineNormal.Y,
                        NormalZ = angular2LineNormal.Z,
                        
                        // 测量属性
                        Measurement = angular2Line.Measurement,
                        Text = angular2Line.Text,
                        
                        // three.js友好的属性
                        AngleInDegrees = angleInDegrees,
                        AngleInRadians = deltaAngle,
                        ArcCenter = arcCenter,
                        Radius = angular2LineRadius,
                        Bounds = bounds,
                        ArcPoints = arcPoints.ToArray(),
                        FirstLineDirection = firstLineDirection,
                        SecondLineDirection = secondLineDirection
                    };
                    
                    // 更新DimensionData的几何属性
                    dimensionData.Bounds = bounds;
                    dimensionData.Centroid = centroid;
                    dimensionData.Points = allPoints.ToArray();
                    
                    return dimensionData;
                }

                // 特殊处理DimensionDiameter类型
                if (dimension is DimensionDiameter diameter)
                {
                    // 获取3D坐标
                    var angleVertex = diameter.AngleVertex;
                    var diameterDefPoint = diameter.DefinitionPoint;
                    var center = diameter.Center;
                    var diameterRadius = diameter.Measurement / 2.0;
                    
                    // 获取法线向量
                    var diameterNormal = diameter.Normal;
                    
                    // 创建3D点数据
                    var angleVertex3D = new Point3DData(angleVertex.X, angleVertex.Y, angleVertex.Z);
                    var diameterDefPoint3D = new Point3DData(diameterDefPoint.X, diameterDefPoint.Y, diameterDefPoint.Z);
                    var center3D = new Point3DData(center.X, center.Y, center.Z);
                    
                    // 生成圆上的点
                    var circlePoints = GenerateCirclePoints(center3D, diameterRadius);
                    
                    // 计算边界框和质心
                    var allPoints = new List<Point3DData> { angleVertex3D, diameterDefPoint3D, center3D };
                    allPoints.AddRange(circlePoints);
                    var bounds = CalculateBounds(allPoints);
                    var centroid = CalculateCentroid(allPoints);
                    
                    // 计算起始和结束角度（基于角度顶点和定义点）
                    var startAngle = Math.Atan2(angleVertex.Y - center.Y, angleVertex.X - center.X);
                    var endAngle = Math.Atan2(diameterDefPoint.Y - center.Y, diameterDefPoint.X - center.X);
                    
                    dimensionData.DiameterData = new DiameterData
                    {
                        // 2D坐标（保持向后兼容）
                        AngleVertexX = angleVertex.X,
                        AngleVertexY = angleVertex.Y,
                        DefinitionPointX = diameterDefPoint.X,
                        DefinitionPointY = diameterDefPoint.Y,
                        CenterX = center.X,
                        CenterY = center.Y,
                        
                        // 3D坐标支持
                        AngleVertexZ = angleVertex.Z,
                        DefinitionPointZ = diameterDefPoint.Z,
                        CenterZ = center.Z,
                        
                        // 法线向量（用于3D渲染）
                        NormalX = diameterNormal.X,
                        NormalY = diameterNormal.Y,
                        NormalZ = diameterNormal.Z,
                        
                        // 测量属性
                        Measurement = diameter.Measurement,
                        Text = diameter.Text,
                        
                        // three.js友好的属性
                        Radius = diameterRadius,
                        Diameter = diameter.Measurement,
                        Bounds = bounds,
                        Centroid = centroid,
                        CirclePoints = circlePoints.ToArray(),
                        StartAngle = startAngle,
                        EndAngle = endAngle
                    };
                    
                    // 更新DimensionData的几何属性
                    dimensionData.Bounds = bounds;
                    dimensionData.Centroid = centroid;
                    dimensionData.Points = allPoints.ToArray();
                    
                    return dimensionData;
                }

                // 特殊处理DimensionRadius类型
                if (dimension is DimensionRadius radiusDim)
                {
                    // 获取3D坐标
                    var radiusDefPoint = radiusDim.DefinitionPoint;
                    var angleVertex = radiusDim.AngleVertex;
                    var radiusValue = radiusDim.Measurement;
                    
                    // 获取法线向量
                    var radiusNormal = radiusDim.Normal;
                    
                    // 创建3D点数据
                    var radiusDefPoint3D = new Point3DData(radiusDefPoint.X, radiusDefPoint.Y, radiusDefPoint.Z);
                    var angleVertex3D = new Point3DData(angleVertex.X, angleVertex.Y, angleVertex.Z);
                    
                    // 计算圆心（假设定义点在圆上，角度顶点是圆心）
                    var center3D = angleVertex3D;
                    
                    // 生成圆弧上的点（半圆）
                    var startAngle = Math.Atan2(radiusDefPoint.Y - center3D.Y, radiusDefPoint.X - center3D.X);
                    var endAngle = startAngle + Math.PI; // 半圆
                    var arcPoints = GenerateArcPoints(center3D, radiusValue, startAngle, endAngle);
                    
                    // 计算引线方向
                    var leaderDirection = CalculateDirection(center3D, radiusDefPoint3D);
                    
                    // 计算边界框和质心
                    var allPoints = new List<Point3DData> { radiusDefPoint3D, angleVertex3D, center3D };
                    allPoints.AddRange(arcPoints);
                    var bounds = CalculateBounds(allPoints);
                    var centroid = CalculateCentroid(allPoints);

                    dimensionData.RadiusData = new RadiusData
                    {
                        // 2D坐标（保持向后兼容）
                        DefinitionPointX = radiusDefPoint.X,
                        DefinitionPointY = radiusDefPoint.Y,
                        AngleVertexX = angleVertex.X,
                        AngleVertexY = angleVertex.Y,
                        
                        // 3D坐标支持
                        DefinitionPointZ = radiusDefPoint.Z,
                        AngleVertexZ = angleVertex.Z,
                        CenterZ = center3D.Z,
                        
                        // 法线向量（用于3D渲染）
                        NormalX = radiusNormal.X,
                        NormalY = radiusNormal.Y,
                        NormalZ = radiusNormal.Z,
                        
                        // 测量属性
                        Measurement = radiusDim.Measurement,
                        Text = radiusDim.Text,
                        LeaderLength = radiusDim.LeaderLength,
                        
                        // three.js友好的属性
                        Radius = radiusValue,
                        Center = center3D,
                        Bounds = bounds,
                        Centroid = centroid,
                        ArcPoints = arcPoints.ToArray(),
                        StartAngle = startAngle,
                        EndAngle = endAngle,
                        LeaderDirection = leaderDirection
                    };
                    
                    // 更新DimensionData的几何属性
                    dimensionData.Bounds = bounds;
                    dimensionData.Centroid = centroid;
                    dimensionData.Points = allPoints.ToArray();
                    
                    return dimensionData;
                }

                // 特殊处理DimensionOrdinate类型
                if (dimension is DimensionOrdinate ordinate)
                {
                    // 获取3D坐标
                    var featureLocation = ordinate.FeatureLocation;
                    var leaderEndpoint = ordinate.LeaderEndpoint;
                    
                    // 获取法线向量
                    var ordinateNormal = ordinate.Normal;
                    
                    // 创建3D点数据
                    var featureLocation3D = new Point3DData(featureLocation.X, featureLocation.Y, featureLocation.Z);
                    var leaderEndpoint3D = new Point3DData(leaderEndpoint.X, leaderEndpoint.Y, leaderEndpoint.Z);
                    
                    // 计算引线方向和长度
                    var leaderDirection = CalculateDirection(featureLocation3D, leaderEndpoint3D);
                    var leaderLength = CalculateDistance(featureLocation3D, leaderEndpoint3D);
                    
                    // 计算边界框和质心
                    var allPoints = new List<Point3DData> { featureLocation3D, leaderEndpoint3D };
                    var bounds = CalculateBounds(allPoints);
                    var centroid = CalculateCentroid(allPoints);
                    
                    dimensionData.OrdinateData = new OrdinateData
                    {
                        // 2D坐标（保持向后兼容）
                        FeatureLocationX = featureLocation.X,
                        FeatureLocationY = featureLocation.Y,
                        LeaderEndpointX = leaderEndpoint.X,
                        LeaderEndpointY = leaderEndpoint.Y,
                        
                        // 3D坐标支持
                        FeatureLocationZ = featureLocation.Z,
                        LeaderEndpointZ = leaderEndpoint.Z,
                        
                        // 法线向量（用于3D渲染）
                        NormalX = ordinateNormal.X,
                        NormalY = ordinateNormal.Y,
                        NormalZ = ordinateNormal.Z,
                        
                        // 测量属性
                        IsOrdinateTypeX = ordinate.IsOrdinateTypeX,
                        Measurement = ordinate.Measurement,
                        Text = ordinate.Text,
                        
                        // three.js友好的属性
                        Bounds = bounds,
                        Centroid = centroid,
                        FeatureLocation = featureLocation3D,
                        LeaderEndpoint = leaderEndpoint3D,
                        LeaderDirection = leaderDirection,
                        LeaderLength = leaderLength
                    };
                    
                    // 更新DimensionData的几何属性
                    dimensionData.Bounds = bounds;
                    dimensionData.Centroid = centroid;
                    dimensionData.Points = allPoints.ToArray();
                    
                    return dimensionData;
                }

                // 特殊处理DimensionLinear类型 (必须在DimensionAligned之前检查)
                Console.WriteLine($"检查DimensionLinear: 类型={dimension.GetType().Name}, 是否DimensionLinear={dimension is DimensionLinear}");
                if (dimension is DimensionLinear linear)
                {
                    Console.WriteLine("进入DimensionLinear处理分支");
                    // 获取基本点坐标
                    var linearDefPoint = new Point3DData(linear.DefinitionPoint.X, linear.DefinitionPoint.Y, linear.DefinitionPoint.Z);
                    var firstPoint = new Point3DData(linear.FirstPoint.X, linear.FirstPoint.Y, linear.FirstPoint.Z);
                    var secondPoint = new Point3DData(linear.SecondPoint.X, linear.SecondPoint.Y, linear.SecondPoint.Z);
                    
                    // 获取法线向量
                    var linearNormal = linear.Normal;
                    
                    // 计算几何属性
                    var allPoints = new List<Point3DData> { linearDefPoint, firstPoint, secondPoint };
                    var bounds = CalculateBounds(allPoints);
                    var centroid = CalculateCentroid(allPoints);
                    var length = CalculateDistance(firstPoint, secondPoint);
                    
                    // 计算方向向量
                    var direction = new Point3DData(
                        secondPoint.X - firstPoint.X,
                        secondPoint.Y - firstPoint.Y,
                        secondPoint.Z - firstPoint.Z
                    );
                    
                    dimensionData.LinearData = new LinearData
                    {
                        // 2D坐标（保持向后兼容）
                        DefinitionPointX = linear.DefinitionPoint.X,
                        DefinitionPointY = linear.DefinitionPoint.Y,
                        FirstPointX = linear.FirstPoint.X,
                        FirstPointY = linear.FirstPoint.Y,
                        SecondPointX = linear.SecondPoint.X,
                        SecondPointY = linear.SecondPoint.Y,
                        
                        // 3D坐标支持
                        DefinitionPointZ = linear.DefinitionPoint.Z,
                        FirstPointZ = linear.FirstPoint.Z,
                        SecondPointZ = linear.SecondPoint.Z,
                        
                        // 法线向量（用于3D渲染）
                        NormalX = linearNormal.X,
                        NormalY = linearNormal.Y,
                        NormalZ = linearNormal.Z,
                        
                        // 测量属性
                        Measurement = linear.Measurement,
                        Text = linear.Text,
                        Rotation = linear.Rotation,
                        ExtLineRotation = linear.ExtLineRotation,
                        
                        // three.js友好的属性
                        Bounds = bounds,
                        Centroid = centroid,
                        DefinitionPoint = linearDefPoint,
                        FirstPoint = firstPoint,
                        SecondPoint = secondPoint,
                        Length = length,
                        Direction = direction
                    };
                    
                    // 更新DimensionData的几何属性
                    dimensionData.Bounds = bounds;
                    dimensionData.Centroid = centroid;
                    dimensionData.Points = allPoints.ToArray();
                    
                    return dimensionData;
                }

                // 特殊处理DimensionAligned类型
                Console.WriteLine($"检查DimensionAligned: 类型={dimension.GetType().Name}, 是否DimensionAligned={dimension is DimensionAligned}");
                if (dimension is DimensionAligned aligned)
                {
                    Console.WriteLine("进入DimensionAligned处理分支");
                    // 获取基本点坐标
                    var firstPoint = new Point3DData(aligned.FirstPoint.X, aligned.FirstPoint.Y, aligned.FirstPoint.Z);
                    var secondPoint = new Point3DData(aligned.SecondPoint.X, aligned.SecondPoint.Y, aligned.SecondPoint.Z);
                    var alignedDefPoint = new Point3DData(aligned.DefinitionPoint.X, aligned.DefinitionPoint.Y, aligned.DefinitionPoint.Z);
                    
                    // 获取法线向量
                    var alignedNormal = aligned.Normal;
                    
                    // 计算几何属性
                    var allPoints = new List<Point3DData> { firstPoint, secondPoint, alignedDefPoint };
                    var bounds = CalculateBounds(allPoints);
                    var centroid = CalculateCentroid(allPoints);
                    var length = CalculateDistance(firstPoint, secondPoint);
                    
                    // 计算方向向量
                    var direction = new Point3DData(
                        secondPoint.X - firstPoint.X,
                        secondPoint.Y - firstPoint.Y,
                        secondPoint.Z - firstPoint.Z
                    );
                    
                    dimensionData.AlignedData = new AlignedData
                    {
                        // 2D坐标（保持向后兼容）
                        FirstPointX = aligned.FirstPoint.X,
                        FirstPointY = aligned.FirstPoint.Y,
                        SecondPointX = aligned.SecondPoint.X,
                        SecondPointY = aligned.SecondPoint.Y,
                        DefinitionPointX = aligned.DefinitionPoint.X,
                        DefinitionPointY = aligned.DefinitionPoint.Y,
                        
                        // 3D坐标支持
                        FirstPointZ = aligned.FirstPoint.Z,
                        SecondPointZ = aligned.SecondPoint.Z,
                        DefinitionPointZ = aligned.DefinitionPoint.Z,
                        
                        // 法线向量（用于3D渲染）
                        NormalX = alignedNormal.X,
                        NormalY = alignedNormal.Y,
                        NormalZ = alignedNormal.Z,
                        
                        // 测量属性
                        Measurement = aligned.Measurement,
                        Text = aligned.Text,
                        ExtLineRotation = aligned.ExtLineRotation,
                        
                        // three.js友好的属性
                        Bounds = bounds,
                        Centroid = centroid,
                        FirstPoint = firstPoint,
                        SecondPoint = secondPoint,
                        DefinitionPoint = alignedDefPoint,
                        Length = length,
                        Direction = direction
                    };
                    
                    // 更新DimensionData的几何属性
                    dimensionData.Bounds = bounds;
                    dimensionData.Centroid = centroid;
                    dimensionData.Points = allPoints.ToArray();
                    
                    return dimensionData;
                }



                // Dimension对象通过Block属性来表示其图形内容
                // 我们需要遍历Block中的所有实体并分别渲染它们
                if (dimension.Block == null || dimension.Block.Entities == null)
                {
                    return dimensionData;
                }

                // 遍历Block中的所有实体并渲染它们
                var entitiesList = new List<EntityData>();
                foreach (var entity in dimension.Block.Entities)
                {
                    var entityData = new EntityData { Type = entity.GetType().Name };

                    // 根据实体类型调用相应的渲染器
                    if (entity is Line line)
                    {
                        entityData.LineData = new LineData
                        {
                            StartX = line.StartPoint.X,
                            StartY = line.StartPoint.Y,
                            EndX = line.EndPoint.X,
                            EndY = line.EndPoint.Y,
                            ColorIndex = line.Color.Index,
                            LineTypeName = line.GetActiveLineType().Name,
                            LineWeight = line.GetActiveLineWeightType().GetLineWeightValue()
                        };
                        LineEntityRenderer.Render(line);
                    }
                    else if (entity is MText mtext)
                    {
                        entityData.MTextData = new MTextData
                        {
                            Value = mtext.Value,
                            PositionX = mtext.InsertPoint.X,
                            PositionY = mtext.InsertPoint.Y,
                            ColorIndex = mtext.Color.Index,
                            Style = mtext.Style?.Name ?? ""
                        };
                    }
                    else if (entity is TextEntity text)
                    {
                        entityData.TextData = new TextData
                        {
                            Value = text.Value,
                            PositionX = text.InsertPoint.X,
                            PositionY = text.InsertPoint.Y,
                            ColorIndex = text.Color.Index,
                            Style = text.Style?.Name ?? ""
                        };
                    }
                    else if (entity is Solid solid)
                    {
                        entityData.SolidData = new SolidData
                        {
                            Points = new Point3DData[]
                            {
                                new Point3DData { X = solid.FirstCorner.X, Y = solid.FirstCorner.Y },
                                new Point3DData { X = solid.SecondCorner.X, Y = solid.SecondCorner.Y },
                                new Point3DData { X = solid.ThirdCorner.X, Y = solid.ThirdCorner.Y },
                                new Point3DData { X = solid.FourthCorner.X, Y = solid.FourthCorner.Y }
                            },
                            ColorIndex = solid.Color.Index
                        };
                    }
                    else if (entity is ACadSharp.Entities.Point point)
                    {
                        entityData.PointData = new DimensionPointData
                        {
                            X = point.Location.X,
                            Y = point.Location.Y,
                            ColorIndex = point.Color.Index
                        };
                    }
                    // 其他实体类型可以根据需要添加

                    entitiesList.Add(entityData);
                }
                
                // 将列表转换为数组
                dimensionData.Entities = entitiesList.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rendering dimension: {ex.Message}");
            }

            return dimensionData;
        }









        /// <summary>
        /// 创建颜色信息对象，支持多种颜色格式
        /// </summary>
        private static ColorInfo CreateColorInfo(short colorIndex)
        {
            int colorRGB = GetColorRGB(colorIndex);
            return new ColorInfo
            {
                Index = colorIndex,
                Hex = $"#{colorRGB:X6}",
                R = (colorRGB >> 16) & 0xFF,
                G = (colorRGB >> 8) & 0xFF,
                B = colorRGB & 0xFF
            };
        }
        
        /// <summary>
        /// 根据颜色索引获取RGB颜色值
        /// </summary>
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
        
        /// <summary>
        /// 计算边界框
        /// </summary>
        private static BoundsData CalculateBounds(IEnumerable<Point3DData> points)
        {
            var pointList = points.ToList();
            if (!pointList.Any())
            {
                return new BoundsData(
                    new Point3DData(0, 0, 0),
                    new Point3DData(0, 0, 0)
                );
            }
            
            var minX = pointList.Min(p => p.X);
            var minY = pointList.Min(p => p.Y);
            var minZ = pointList.Min(p => p.Z);
            var maxX = pointList.Max(p => p.X);
            var maxY = pointList.Max(p => p.Y);
            var maxZ = pointList.Max(p => p.Z);
            
            return new BoundsData(
                new Point3DData(minX, minY, minZ),
                new Point3DData(maxX, maxY, maxZ)
            );
        }
        
        /// <summary>
        /// 计算质心
        /// </summary>
        private static Point3DData CalculateCentroid(IEnumerable<Point3DData> points)
        {
            var pointList = points.ToList();
            if (!pointList.Any())
            {
                return new Point3DData(0, 0, 0);
            }
            
            return new Point3DData(
                pointList.Average(p => p.X),
                pointList.Average(p => p.Y),
                pointList.Average(p => p.Z)
            );
        }
        
        /// <summary>
        /// 计算两点之间的距离
        /// </summary>
        private static double CalculateDistance(Point3DData point1, Point3DData point2)
        {
            var dx = point2.X - point1.X;
            var dy = point2.Y - point1.Y;
            var dz = point2.Z - point1.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        
        /// <summary>
        /// 计算线段的中点
        /// </summary>
        private static Point3DData CalculateMidPoint(Point3DData point1, Point3DData point2)
        {
            return new Point3DData(
                (point1.X + point2.X) / 2,
                (point1.Y + point2.Y) / 2,
                (point1.Z + point2.Z) / 2
            );
        }
        
        /// <summary>
        /// 计算单位方向向量
        /// </summary>
        private static Point3DData CalculateDirection(Point3DData from, Point3DData to)
        {
            var dx = to.X - from.X;
            var dy = to.Y - from.Y;
            var dz = to.Z - from.Z;
            var length = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            
            if (length == 0)
            {
                return new Point3DData(0, 0, 0);
            }
            
            return new Point3DData(dx / length, dy / length, dz / length);
        }
        
        /// <summary>
        /// 生成圆弧上的点
        /// </summary>
        private static List<Point3DData> GenerateArcPoints(Point3DData center, double radius, double startAngle, double endAngle, int segments = 20)
        {
            var points = new List<Point3DData>();
            var angleRange = endAngle - startAngle;
            
            for (int i = 0; i <= segments; i++)
            {
                var angle = startAngle + (angleRange * i / segments);
                var x = center.X + radius * Math.Cos(angle);
                var y = center.Y + radius * Math.Sin(angle);
                points.Add(new Point3DData(x, y, center.Z));
            }
            
            return points;
        }
        
        /// <summary>
        /// 生成圆上的点
        /// </summary>
        private static List<Point3DData> GenerateCirclePoints(Point3DData center, double radius, int segments = 36)
        {
            return GenerateArcPoints(center, radius, 0, 2 * Math.PI, segments);
        }
        
        /// <summary>
        /// 计算圆弧的圆心
        /// </summary>
        private static Point3DData CalculateArcCenter(Point3DData p1, Point3DData p2, Point3DData p3)
        {
            // 使用三点法计算圆弧圆心
            var ax = p1.X; var ay = p1.Y;
            var bx = p2.X; var by = p2.Y;
            var cx = p3.X; var cy = p3.Y;
            
            var d = 2 * (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));
            if (Math.Abs(d) < 1e-10)
            {
                // 三点共线，返回中间点
                return CalculateMidPoint(p1, p3);
            }
            
            var ux = ((ax * ax + ay * ay) * (by - cy) + (bx * bx + by * by) * (cy - ay) + (cx * cx + cy * cy) * (ay - by)) / d;
            var uy = ((ax * ax + ay * ay) * (cx - bx) + (bx * bx + by * by) * (ax - cx) + (cx * cx + cy * cy) * (bx - ax)) / d;
            
            return new Point3DData(ux, uy, (p1.Z + p2.Z + p3.Z) / 3);
        }
    }
}