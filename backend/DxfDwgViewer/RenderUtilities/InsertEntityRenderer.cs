using System;
using System.Linq;
using ACadSharp.Entities;
using ACadSharp.Extensions;
using System.Collections.Generic;

namespace DxfDwgViewer.RenderUtilities
{
    public class InsertEntityRenderer
    {
        public class InsertData
        {
            // three.js 对象类型标识
            public string Type { get; set; } = "Insert";
            public string Uuid { get; set; } = Guid.NewGuid().ToString();
            public string EntityType { get; set; } = "Insert";

            // 基本属性
            public double InsertPointX { get; set; }
            public double InsertPointY { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            public List<EntityData> Entities { get; set; }
            public MatrixData Transform { get; set; }

            // 缩放和旋转属性
            public double ScaleX { get; set; }
            public double ScaleY { get; set; }
            public double ScaleZ { get; set; }
            public double Rotation { get; set; }
            public int EntityCount { get; set; }

            // 实体属性
            public string Handle { get; set; }
            public string BlockName { get; set; }
            public string LayerName { get; set; }
            public bool IsInvisible { get; set; }
            public double LineTypeScale { get; set; }
            public double Transparency { get; set; }

            // 样式属性
            public string ColorHex { get; set; }
            public int ColorR { get; set; }
            public int ColorG { get; set; }
            public int ColorB { get; set; }
            public int ColorA { get; set; }

            // 材质属性
            public string MaterialType { get; set; } = "Basic";
            public bool MaterialTransparent { get; set; }
            public double MaterialOpacity { get; set; } = 1.0;
            public bool MaterialDepthTest { get; set; } = true;
            public bool MaterialDepthWrite { get; set; } = true;
            public int MaterialSide { get; set; } = 2;

            // 几何属性
            public Point3DData Position { get; set; }
            public BoundsData Bounds { get; set; }
            public Point3DData Centroid { get; set; }

            // 法线向量（用于3D渲染）
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }

            // 用于three.js绘制的点列表
            public List<Point3DData> Vertices { get; set; }
            public List<double> VerticesArray { get; set; }
            public List<int> Indices { get; set; }
            public List<double> Normals { get; set; }
            public List<double> UVs { get; set; }

            // 坐标系和变换
            public string CoordinateSystem { get; set; } = "World";
            public bool RequiresYAxisFlip { get; set; } = false;
            public TransformData Transform3D { get; set; }

            // 属性列表
            public List<AttributeData> Attributes { get; set; }

            // 多重插入属性
            public ushort RowCount { get; set; } = 1;
            public ushort ColumnCount { get; set; } = 1;
            public double RowSpacing { get; set; } = 0;
            public double ColumnSpacing { get; set; } = 0;
            public bool IsMultiple { get; set; } = false;

            // 空间过滤器
            public bool HasSpatialFilter { get; set; } = false;
            public string SpatialFilterName { get; set; } = "";

            // three.js 场景属性
            public object Parent { get; set; }
            public bool Visible { get; set; } = true;
            public bool CastShadow { get; set; } = false;
            public bool ReceiveShadow { get; set; } = false;
            public int RenderOrder { get; set; } = 0;
        }

        public class EntityData
        {
            public string Type { get; set; }
            public LineEntityRenderer.LineData LineData { get; set; }
            public ArcEntityRenderer.ArcData ArcData { get; set; }
            public CircleEntityRenderer.CircleData CircleData { get; set; }
            public LwPolylineEntityRenderer.LwPolylineData LwPolylineData { get; set; }
            public TextData TextData { get; set; }
            public EllipseEntityRenderer.EllipseData EllipseData { get; set; }
            public Face3DEntityRenderer.Face3DData Face3DData { get; set; }
        }

        public class AttributeData
        {
            public string Tag { get; set; }
            public string Value { get; set; }
            public Point3DData Position { get; set; }
            public double Height { get; set; }
            public short ColorIndex { get; set; }
            public string Style { get; set; }
            public double Rotation { get; set; }
            public double WidthFactor { get; set; }
            public double ObliqueAngle { get; set; }
        }

        public class TextData
        {
            public string Value { get; set; }
            public double PositionX { get; set; }
            public double PositionY { get; set; }
            public double Height { get; set; }
            public short ColorIndex { get; set; }
            public string Style { get; set; }
        }

        public static InsertData Render(Insert insert)
        {
            var insertData = new InsertData
            {
                // 基本属性
                InsertPointX = insert.InsertPoint.X,
                InsertPointY = insert.InsertPoint.Y,
                ColorIndex = insert.Color.Index,
                LineTypeName = insert.GetActiveLineType().Name,
                LineWeight = insert.GetActiveLineWeightType().GetLineWeightValue(),
                Entities = new List<EntityData>(),
                
                // 缩放和旋转属性
                ScaleX = insert.XScale,
                ScaleY = insert.YScale,
                ScaleZ = insert.ZScale,
                Rotation = insert.Rotation,
                EntityCount = insert.Block?.Entities?.Count() ?? 0,
                
                // 实体属性
                Handle = insert.Handle.ToString(),
                BlockName = insert.Block?.Name ?? "",
                LayerName = insert.Layer?.Name ?? "",
                IsInvisible = insert.IsInvisible,
                LineTypeScale = 1.0,
                Transparency = 0.0,
                
                // 样式属性
                ColorHex = $"#{insert.Color.R:X2}{insert.Color.G:X2}{insert.Color.B:X2}",
                ColorR = insert.Color.R,
                ColorG = insert.Color.G,
                ColorB = insert.Color.B,
                ColorA = 255,
                
                // 材质属性
                MaterialType = "Basic",
                MaterialTransparent = false,
                MaterialOpacity = 1.0,
                MaterialDepthTest = true,
                MaterialDepthWrite = true,
                MaterialSide = 2,
                
                // 几何属性
                Position = new Point3DData(insert.InsertPoint.X, insert.InsertPoint.Y, insert.InsertPoint.Z),
                Centroid = new Point3DData(insert.InsertPoint.X, insert.InsertPoint.Y, insert.InsertPoint.Z),
                
                // 法线向量
                NormalX = insert.Normal.X,
                NormalY = insert.Normal.Y,
                NormalZ = insert.Normal.Z,
                
                // 用于three.js绘制的点列表
                Vertices = new List<Point3DData>(),
                VerticesArray = new List<double>(),
                Indices = new List<int>(),
                Normals = new List<double>(),
                UVs = new List<double>(),
                
                // 坐标系和变换
                CoordinateSystem = "World",
                RequiresYAxisFlip = false,
                
                // 属性列表
                Attributes = new List<AttributeData>(),
                
                // 多重插入属性
                RowCount = insert.RowCount,
                ColumnCount = insert.ColumnCount,
                RowSpacing = insert.RowSpacing,
                ColumnSpacing = insert.ColumnSpacing,
                IsMultiple = insert.IsMultiple,
                
                // 空间过滤器
                HasSpatialFilter = insert.SpatialFilter != null,
                SpatialFilterName = insert.SpatialFilter?.Name ?? "",
                
                // three.js 场景属性
                Visible = !insert.IsInvisible,
                CastShadow = false,
                ReceiveShadow = false,
                RenderOrder = 0
            };

            // 初始化边界框
            double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;

            // 获取块的变换矩阵
            var transform = insert.GetTransform();

            // 保存变换矩阵数据
            insertData.Transform = new MatrixData
            {
                M11 = transform.Matrix.M00,
                M12 = transform.Matrix.M01,
                M13 = transform.Matrix.M02,
                M14 = transform.Matrix.M03,
                M21 = transform.Matrix.M10,
                M22 = transform.Matrix.M11,
                M23 = transform.Matrix.M12,
                M24 = transform.Matrix.M13,
                M31 = transform.Matrix.M20,
                M32 = transform.Matrix.M21,
                M33 = transform.Matrix.M22,
                M34 = transform.Matrix.M23,
                M41 = transform.Matrix.M30,
                M42 = transform.Matrix.M31,
                M43 = transform.Matrix.M32,
                M44 = transform.Matrix.M33
            };

            // 创建3D变换信息
            insertData.Transform3D = new TransformData(
                insert.XScale, insert.YScale, insert.ZScale,
                insert.Rotation,
                insert.InsertPoint.X, insert.InsertPoint.Y, insert.InsertPoint.Z
            );

            // 处理属性
            if (insert.Attributes != null && insert.Attributes.Count > 0)
            {
                foreach (var attribute in insert.Attributes)
                {
                    insertData.Attributes.Add(new AttributeData
                    {
                        Tag = attribute.Tag,
                        Value = attribute.Value,
                        Position = new Point3DData(attribute.InsertPoint.X, attribute.InsertPoint.Y, attribute.InsertPoint.Z),
                        Height = attribute.Height,
                        ColorIndex = attribute.Color.Index,
                        Style = attribute.Style?.Name ?? "",
                        Rotation = attribute.Rotation,
                        WidthFactor = attribute.WidthFactor,
                        ObliqueAngle = attribute.ObliqueAngle
                    });
                }
            }

            // 绘制块中的每个实体
            if (insert.Block?.Entities != null)
            {
                foreach (var blockEntity in insert.Block.Entities)
            {
                try
                {
                    // 检查实体是否为null
                    if (blockEntity == null)
                    {
                        Console.WriteLine("Warning: Block entity is null");
                        continue;
                    }

                    // 克隆实体并应用变换
                    Entity clonedEntity = blockEntity.CloneTyped();
                    if (clonedEntity == null)
                    {
                        Console.WriteLine($"Warning: Failed to clone entity of type {blockEntity.GetType().Name}");
                        continue;
                    }

                    clonedEntity.ApplyTransform(transform);

                    var entityData = new EntityData { Type = clonedEntity.GetType().Name };

                    // 根据实体类型调用相应的渲染方法
                    if (clonedEntity is Line blockLine)
                    {
                        try
                        {
                            entityData.LineData = LineEntityRenderer.Render(blockLine);
                            
                            // 收集顶点并更新边界框
                            if (entityData.LineData != null)
                            {
                                insertData.Vertices.Add(new Point3DData(blockLine.StartPoint.X, blockLine.StartPoint.Y, blockLine.StartPoint.Z));
                                insertData.Vertices.Add(new Point3DData(blockLine.EndPoint.X, blockLine.EndPoint.Y, blockLine.EndPoint.Z));
                                
                                UpdateBounds(ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ, 
                                            blockLine.StartPoint.X, blockLine.StartPoint.Y, blockLine.StartPoint.Z);
                                UpdateBounds(ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ, 
                                            blockLine.EndPoint.X, blockLine.EndPoint.Y, blockLine.EndPoint.Z);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error rendering line entity: {ex.Message}");
                        }
                    }
                    else if (clonedEntity is Arc blockArc)
                    {
                        try
                        {
                            entityData.ArcData = ArcEntityRenderer.Render(blockArc);
                            
                            // 收集圆弧的关键点并更新边界框
                            if (entityData.ArcData != null)
                            {
                                // 添加圆弧的起点、中点和终点
                                double startAngle = blockArc.StartAngle;
                                double endAngle = blockArc.EndAngle;
                                double midAngle = (startAngle + endAngle) / 2;
                                
                                insertData.Vertices.Add(new Point3DData(blockArc.Center.X + blockArc.Radius * Math.Cos(startAngle), 
                                                                     blockArc.Center.Y + blockArc.Radius * Math.Sin(startAngle), 
                                                                     blockArc.Center.Z));
                                insertData.Vertices.Add(new Point3DData(blockArc.Center.X + blockArc.Radius * Math.Cos(midAngle), 
                                                                     blockArc.Center.Y + blockArc.Radius * Math.Sin(midAngle), 
                                                                     blockArc.Center.Z));
                                insertData.Vertices.Add(new Point3DData(blockArc.Center.X + blockArc.Radius * Math.Cos(endAngle), 
                                                                     blockArc.Center.Y + blockArc.Radius * Math.Sin(endAngle), 
                                                                     blockArc.Center.Z));
                                
                                // 更新边界框
                                UpdateBounds(ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ, 
                                            blockArc.Center.X - blockArc.Radius, blockArc.Center.Y - blockArc.Radius, blockArc.Center.Z);
                                UpdateBounds(ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ, 
                                            blockArc.Center.X + blockArc.Radius, blockArc.Center.Y + blockArc.Radius, blockArc.Center.Z);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error rendering arc entity: {ex.Message}");
                        }
                    }
                    else if (clonedEntity is Circle blockCircle)
                    {
                        try
                        {
                            entityData.CircleData = CircleEntityRenderer.Render(blockCircle);
                            
                            // 收集圆的关键点并更新边界框
                            if (entityData.CircleData != null)
                            {
                                // 添加圆上的几个关键点
                                for (int i = 0; i < 8; i++)
                                {
                                    double angle = 2 * Math.PI * i / 8;
                                    insertData.Vertices.Add(new Point3DData(blockCircle.Center.X + blockCircle.Radius * Math.Cos(angle), 
                                                                         blockCircle.Center.Y + blockCircle.Radius * Math.Sin(angle), 
                                                                         blockCircle.Center.Z));
                                }
                                
                                // 更新边界框
                                UpdateBounds(ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ, 
                                            blockCircle.Center.X - blockCircle.Radius, blockCircle.Center.Y - blockCircle.Radius, blockCircle.Center.Z);
                                UpdateBounds(ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ, 
                                            blockCircle.Center.X + blockCircle.Radius, blockCircle.Center.Y + blockCircle.Radius, blockCircle.Center.Z);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error rendering circle entity: {ex.Message}");
                        }
                    }
                    else if (clonedEntity is LwPolyline blockLwPolyline)
                    {
                        try
                        {
                            entityData.LwPolylineData = LwPolylineEntityRenderer.Render(blockLwPolyline);
                            
                            // 收集多段线顶点并更新边界框
                            if (entityData.LwPolylineData != null && blockLwPolyline.Vertices != null)
                            {
                                foreach (var vertex in blockLwPolyline.Vertices)
                                {
                                    insertData.Vertices.Add(new Point3DData(vertex.Location.X, vertex.Location.Y, 0));
                                    UpdateBounds(ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ, 
                                                vertex.Location.X, vertex.Location.Y, 0);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error rendering LwPolyline entity: {ex.Message}");
                        }
                    }
                    else if (clonedEntity is TextEntity blockText)
                    {
                        try
                        {
                            entityData.TextData = new TextData
                            {
                                Value = blockText.Value,
                                PositionX = blockText.InsertPoint.X,
                                PositionY = blockText.InsertPoint.Y,
                                Height = blockText.Height,
                                ColorIndex = blockText.Color.Index,
                                Style = blockText.Style?.Name ?? ""
                            };
                            
                            // 添加文本位置点并更新边界框
                            insertData.Vertices.Add(new Point3DData(blockText.InsertPoint.X, blockText.InsertPoint.Y, blockText.InsertPoint.Z));
                            UpdateBounds(ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ, 
                                        blockText.InsertPoint.X, blockText.InsertPoint.Y, blockText.InsertPoint.Z);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error rendering text entity: {ex.Message}");
                        }
                    }
                    else if (clonedEntity is Ellipse blockEllipse)
                    {
                        try
                        {
                            entityData.EllipseData = EllipseEntityRenderer.Render(blockEllipse);
                            
                            // 收集椭圆的关键点并更新边界框
                            if (entityData.EllipseData != null)
                            {
                                // 添加椭圆上的几个关键点
                                for (int i = 0; i < 8; i++)
                                {
                                    double angle = 2 * Math.PI * i / 8;
                                    double x = blockEllipse.Center.X + blockEllipse.MajorAxis * Math.Cos(angle) * Math.Cos(blockEllipse.Rotation) 
                                             - blockEllipse.MinorAxis * Math.Sin(angle) * Math.Sin(blockEllipse.Rotation);
                                    double y = blockEllipse.Center.Y + blockEllipse.MajorAxis * Math.Cos(angle) * Math.Sin(blockEllipse.Rotation) 
                                             + blockEllipse.MinorAxis * Math.Sin(angle) * Math.Cos(blockEllipse.Rotation);
                                    
                                    insertData.Vertices.Add(new Point3DData(x, y, blockEllipse.Center.Z));
                                    UpdateBounds(ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ, x, y, blockEllipse.Center.Z);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error rendering ellipse entity: {ex.Message}");
                        }
                    }
                    else if (clonedEntity is Face3D blockFace3D)
                    {
                        try
                        {
                            entityData.Face3DData = Face3DEntityRenderer.Render(blockFace3D);
                            
                            // 收集3D面的顶点并更新边界框
                            if (entityData.Face3DData != null)
                            {
                                insertData.Vertices.Add(new Point3DData(blockFace3D.FirstCorner.X, blockFace3D.FirstCorner.Y, blockFace3D.FirstCorner.Z));
                                insertData.Vertices.Add(new Point3DData(blockFace3D.SecondCorner.X, blockFace3D.SecondCorner.Y, blockFace3D.SecondCorner.Z));
                                insertData.Vertices.Add(new Point3DData(blockFace3D.ThirdCorner.X, blockFace3D.ThirdCorner.Y, blockFace3D.ThirdCorner.Z));
                                
                                UpdateBounds(ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ, 
                                            blockFace3D.FirstCorner.X, blockFace3D.FirstCorner.Y, blockFace3D.FirstCorner.Z);
                                UpdateBounds(ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ, 
                                            blockFace3D.SecondCorner.X, blockFace3D.SecondCorner.Y, blockFace3D.SecondCorner.Z);
                                UpdateBounds(ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ, 
                                            blockFace3D.ThirdCorner.X, blockFace3D.ThirdCorner.Y, blockFace3D.ThirdCorner.Z);
                                
                                insertData.Vertices.Add(new Point3DData(blockFace3D.FourthCorner.X, blockFace3D.FourthCorner.Y, blockFace3D.FourthCorner.Z));
                                UpdateBounds(ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ, 
                                            blockFace3D.FourthCorner.X, blockFace3D.FourthCorner.Y, blockFace3D.FourthCorner.Z);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error rendering Face3D entity: {ex.Message}");
                        }
                    }

                    insertData.Entities.Add(entityData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error rendering entity in block: {ex.Message}");
                }
            }
            }
            
            // 设置边界框
            if (minX != double.MaxValue && maxX != double.MinValue)
            {
                insertData.Bounds = new BoundsData(
                    new Point3DData(minX, minY, minZ),
                    new Point3DData(maxX, maxY, maxZ)
                );
            }

            // 填充three.js顶点数组
            foreach (var vertex in insertData.Vertices)
            {
                insertData.VerticesArray.Add(vertex.X);
                insertData.VerticesArray.Add(vertex.Y);
                insertData.VerticesArray.Add(vertex.Z);
                
                // 添加法线（默认为Z轴向上）
                insertData.Normals.Add(insertData.NormalX);
                insertData.Normals.Add(insertData.NormalY);
                insertData.Normals.Add(insertData.NormalZ);
                
                // 添加UV坐标（简单映射）
                insertData.UVs.Add(0);
                insertData.UVs.Add(0);
            }

            // 生成索引（简单的三角形索引）
            for (int i = 0; i < insertData.Vertices.Count - 2; i++)
            {
                insertData.Indices.Add(0);
                insertData.Indices.Add(i + 1);
                insertData.Indices.Add(i + 2);
            }

            return insertData;
        }
        
        /// <summary>
        /// 更新边界框值
        /// </summary>
        private static void UpdateBounds(ref double minX, ref double minY, ref double minZ, 
                                        ref double maxX, ref double maxY, ref double maxZ,
                                        double x, double y, double z)
        {
            if (x < minX) minX = x;
            if (y < minY) minY = y;
            if (z < minZ) minZ = z;
            if (x > maxX) maxX = x;
            if (y > maxY) maxY = y;
            if (z > maxZ) maxZ = z;
        }
    }
}