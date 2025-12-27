using System;
using System.Linq;
using ACadSharp.Entities;
using System.Collections.Generic;
using ACadSharp.Extensions;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DxfDwgViewer.RenderUtilities
{
    public class LeaderEntityRenderer
    {
        public class LeaderData
        {
            // 原有属性
            public List<PointData> Vertices { get; set; }
            public string PathType { get; set; }
            public bool ArrowHeadEnabled { get; set; }
            public bool HasHookline { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            public List<PointData> ArrowHeadPoints { get; set; }
            public PointData HookLineStart { get; set; }
            public PointData HookLineEnd { get; set; }
            
            // 添加three.js友好的属性
            public double TotalLength { get; set; }
            public PointData StartPoint { get; set; }
            public PointData EndPoint { get; set; }
            public int VertexCount { get; set; }
            
            // three.js兼容性扩展属性
            public string Type { get; set; } = "Leader"; // 实体类型
            public string Handle { get; set; } // 实体句柄（唯一标识符）
            public string LayerName { get; set; } // 图层名称
            public bool Visible { get; set; } = true; // 可见性
            public string CoordinateSystem { get; set; } = "World"; // 坐标系
            
            // 3D坐标和几何信息
            public List<Point3DData> Vertices3D { get; set; } // 3D顶点列表
            public Point3DData StartPoint3D { get; set; } // 3D起点
            public Point3DData EndPoint3D { get; set; } // 3D终点
            public BoundsData Bounds { get; set; } // 边界框
            public ColorData Color { get; set; } // 颜色信息
            public TransformData Transform3D { get; set; } // 3D变换信息
            
            // 箭头和钩线的3D数据
            public List<Point3DData> ArrowHeadPoints3D { get; set; } // 3D箭头点
            public Point3DData HookLineStart3D { get; set; } // 3D钩线起点
            public Point3DData HookLineEnd3D { get; set; } // 3D钩线终点
            
            // 材质属性
            public double Opacity { get; set; } = 1.0; // 不透明度
            public bool Transparent { get; set; } = false; // 是否透明
            public string MaterialType { get; set; } = "LineBasicMaterial"; // 材质类型
            public bool DepthTest { get; set; } = true; // 深度测试
            public bool DepthWrite { get; set; } = true; // 深度写入
            
            // Leader特有属性
            public Point3DData Normal { get; set; } // 法向量
            public Point3DData HorizontalDirection { get; set; } // 水平方向
            public Point3DData AnnotationOffset { get; set; } // 注释偏移
            public Point3DData BlockOffset { get; set; } // 块偏移
            public string CreationType { get; set; } // 创建类型
            public string HookLineDirection { get; set; } // 钩线方向
            public double TextHeight { get; set; } // 文本高度
            public double TextWidth { get; set; } // 文本宽度
            public string StyleName { get; set; } // 样式名称
        }

        public static LeaderData Render(Leader leader)
        {
            var leaderData = new LeaderData
            {
                Vertices = new List<PointData>(),
                PathType = leader.PathType.ToString(),
                ArrowHeadEnabled = leader.ArrowHeadEnabled,
                HasHookline = leader.Vertices != null && leader.HasHookline,
                ColorIndex = leader.Color.Index,
                LineTypeName = leader.GetActiveLineType()?.Name ?? "",
                LineWeight = leader.GetActiveLineWeightType().GetLineWeightValue(),
                ArrowHeadPoints = new List<PointData>(),
                HookLineStart = null,
                HookLineEnd = null,
                
                // 初始化three.js兼容性属性
                Vertices3D = new List<Point3DData>(),
                ArrowHeadPoints3D = new List<Point3DData>(),
                LayerName = leader.Layer?.Name ?? "0",
                Type = "Leader",
                Handle = leader.Handle.ToString(),
                Visible = !leader.IsInvisible,
                CoordinateSystem = "World",
                Opacity = 1.0,
                Transparent = false,
                MaterialType = "LineBasicMaterial",
                DepthTest = true,
                DepthWrite = true,
                
                // 初始化Leader特有属性
                Normal = new Point3DData(leader.Normal.X, leader.Normal.Y, leader.Normal.Z),
                HorizontalDirection = new Point3DData(leader.HorizontalDirection.X, leader.HorizontalDirection.Y, leader.HorizontalDirection.Z),
                AnnotationOffset = new Point3DData(leader.AnnotationOffset.X, leader.AnnotationOffset.Y, leader.AnnotationOffset.Z),
                BlockOffset = new Point3DData(leader.BlockOffset.X, leader.BlockOffset.Y, leader.BlockOffset.Z),
                CreationType = leader.CreationType.ToString(),
                HookLineDirection = leader.HookLineDirection.ToString(),
                TextHeight = leader.TextHeight,
                TextWidth = leader.TextWidth,
                StyleName = leader.Style?.Name ?? "Standard"
            };

            try
            {
                // 检查是否有足够的顶点
                if (leader.Vertices == null || leader.Vertices.Count < 2)
                {
                    return leaderData;
                }

                // 保存顶点数据
                foreach (var vertex in leader.Vertices)
                {
                    leaderData.Vertices.Add(new PointData(vertex.X, vertex.Y));
                    // 同时添加3D顶点数据
                    leaderData.Vertices3D.Add(new Point3DData(vertex.X, vertex.Y, vertex.Z));
                }

                // 添加增强属性
                leaderData.VertexCount = leader.Vertices.Count;
                if (leader.Vertices.Count > 0)
                {
                    leaderData.StartPoint = new PointData(leader.Vertices[0].X, leader.Vertices[0].Y);
                    leaderData.EndPoint = new PointData(leader.Vertices[leader.Vertices.Count - 1].X, leader.Vertices[leader.Vertices.Count - 1].Y);
                    
                    // 设置3D起点和终点
                    leaderData.StartPoint3D = new Point3DData(leader.Vertices[0].X, leader.Vertices[0].Y, leader.Vertices[0].Z);
                    leaderData.EndPoint3D = new Point3DData(leader.Vertices[leader.Vertices.Count - 1].X, leader.Vertices[leader.Vertices.Count - 1].Y, leader.Vertices[leader.Vertices.Count - 1].Z);
                }

                // 计算总长度
                double totalLength = 0;
                for (int i = 0; i < leader.Vertices.Count - 1; i++)
                {
                    double dx = leader.Vertices[i + 1].X - leader.Vertices[i].X;
                    double dy = leader.Vertices[i + 1].Y - leader.Vertices[i].Y;
                    totalLength += Math.Sqrt(dx * dx + dy * dy);
                }
                leaderData.TotalLength = totalLength;

                // 计算边界框
                if (leader.Vertices.Count > 0)
                {
                    double minX = leader.Vertices[0].X;
                    double minY = leader.Vertices[0].Y;
                    double minZ = leader.Vertices[0].Z;
                    double maxX = leader.Vertices[0].X;
                    double maxY = leader.Vertices[0].Y;
                    double maxZ = leader.Vertices[0].Z;

                    foreach (var vertex in leader.Vertices)
                    {
                        if (vertex.X < minX) minX = vertex.X;
                        if (vertex.Y < minY) minY = vertex.Y;
                        if (vertex.Z < minZ) minZ = vertex.Z;
                        if (vertex.X > maxX) maxX = vertex.X;
                        if (vertex.Y > maxY) maxY = vertex.Y;
                        if (vertex.Z > maxZ) maxZ = vertex.Z;
                    }

                    leaderData.Bounds = new BoundsData(
                        new Point3DData(minX, minY, minZ),
                        new Point3DData(maxX, maxY, maxZ)
                    );
                }

                // 设置颜色数据
                leaderData.Color = new ColorData(leader.Color.Index);

                // 设置变换数据（使用默认单位矩阵）
                leaderData.Transform3D = new TransformData();

                // 绘制箭头
                if (leader.ArrowHeadEnabled && leader.Vertices.Count >= 2)
                {
                    var arrowHeadPoints = DrawArrowHead(leader.Vertices[0], leader.Vertices[1]);
                    leaderData.ArrowHeadPoints = arrowHeadPoints.Select(p => new PointData(p.X, p.Y)).ToList();
                    
                    // 同时填充3D箭头点数据
                    foreach (var point in arrowHeadPoints)
                    {
                        leaderData.ArrowHeadPoints3D.Add(new Point3DData(point.X, point.Y, leader.Vertices[0].Z));
                    }
                }

                // 绘制钩线（如果有）
                if (leader.HasHookline && leader.Vertices.Count >= 2)
                {
                    var hookLinePoints = DrawHookLine(leader.Vertices);
                    Console.WriteLine($"HasHookline: {leader.HasHookline}, Vertices.Count: {leader.Vertices.Count}, hookLinePoints.Length: {hookLinePoints?.Length ?? 0}");
                    if (hookLinePoints != null && hookLinePoints.Length == 2)
                    {
                        leaderData.HookLineStart = new PointData(hookLinePoints[0].X, hookLinePoints[0].Y);
                        leaderData.HookLineEnd = new PointData(hookLinePoints[1].X, hookLinePoints[1].Y);
                        
                        // 设置3D钩线点
                        int lastVertexIndex = leader.Vertices.Count - 1;
                        leaderData.HookLineStart3D = new Point3DData(hookLinePoints[0].X, hookLinePoints[0].Y, leader.Vertices[lastVertexIndex].Z);
                        leaderData.HookLineEnd3D = new Point3DData(hookLinePoints[1].X, hookLinePoints[1].Y, leader.Vertices[lastVertexIndex].Z);
                    }
                }
            }
            catch (Exception ex)
            {
                // 如果Leader处理失败，至少填充基本的3D数据
                if (leader.Vertices != null && leader.Vertices.Count > 0)
                {
                    leaderData.StartPoint3D = new Point3DData(leader.Vertices[0].X, leader.Vertices[0].Y, leader.Vertices[0].Z);
                    leaderData.EndPoint3D = new Point3DData(leader.Vertices[0].X, leader.Vertices[0].Y, leader.Vertices[0].Z);
                    leaderData.Bounds = new BoundsData(
                        new Point3DData(leader.Vertices[0].X - 5, leader.Vertices[0].Y - 5, leader.Vertices[0].Z - 5),
                        new Point3DData(leader.Vertices[0].X + 5, leader.Vertices[0].Y + 5, leader.Vertices[0].Z + 5)
                    );
                }
                
                // 确保颜色和变换数据始终被设置
                leaderData.Color = new ColorData(leader.Color.Index);
                leaderData.Transform3D = new TransformData();

                Console.WriteLine($"Error rendering leader: {ex.Message}");
            }

            return leaderData;
        }

        /// <summary>
        /// 测试LeaderData的JSON序列化和反序列化，验证three.js兼容性
        /// </summary>
        /// <param name="leaderData">要测试的LeaderData对象</param>
        /// <returns>反序列化后的LeaderData对象</returns>
        public static LeaderData TestJsonSerialization(LeaderData leaderData)
        {
            try
            {
                // 配置JSON序列化选项
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    IgnoreNullValues = true
                };

                // 序列化为JSON字符串
                string jsonString = JsonSerializer.Serialize(leaderData, options);
                Console.WriteLine($"LeaderData JSON序列化成功，长度: {jsonString.Length}");

                // 反序列化回对象
                LeaderData deserializedData = JsonSerializer.Deserialize<LeaderData>(jsonString, options)!;
                
                // 验证关键属性
                if (deserializedData == null)
                {
                    Console.WriteLine("错误：反序列化后的对象为null");
                    return null;
                }

                // 验证three.js兼容性关键属性
                bool isValid = true;
                isValid &= ValidateThreeJSCompatibility(deserializedData);

                Console.WriteLine($"LeaderData three.js兼容性验证: {(isValid ? "通过" : "失败")}");
                return deserializedData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LeaderData JSON序列化/反序列化失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 验证LeaderData的three.js兼容性
        /// </summary>
        /// <param name="leaderData">要验证的LeaderData对象</param>
        /// <returns>是否兼容</returns>
        private static bool ValidateThreeJSCompatibility(LeaderData leaderData)
        {
            // 检查基本属性
            if (string.IsNullOrEmpty(leaderData.Type) || leaderData.Type != "Leader")
            {
                Console.WriteLine("错误：Type属性缺失或不正确");
                return false;
            }

            if (string.IsNullOrEmpty(leaderData.Handle))
            {
                Console.WriteLine("错误：Handle属性缺失");
                return false;
            }

            if (leaderData.Vertices3D == null || leaderData.Vertices3D.Count == 0)
            {
                Console.WriteLine("错误：Vertices3D属性缺失或为空");
                return false;
            }

            if (leaderData.Color == null)
            {
                Console.WriteLine("错误：Color属性缺失");
                return false;
            }

            if (leaderData.Bounds == null)
            {
                Console.WriteLine("错误：Bounds属性缺失");
                return false;
            }

            if (leaderData.Transform3D == null)
            {
                Console.WriteLine("错误：Transform3D属性缺失");
                return false;
            }

            // 检查颜色数据完整性
            if (leaderData.Color.Hex == null || leaderData.Color.Hex.Length != 7)
            {
                Console.WriteLine("错误：颜色Hex值格式不正确");
                return false;
            }

            // 检查边界框数据完整性
            if (leaderData.Bounds.Min == null || leaderData.Bounds.Max == null || leaderData.Bounds.Center == null)
            {
                Console.WriteLine("错误：边界框数据不完整");
                return false;
            }

            // 检查变换矩阵
            if (leaderData.Transform3D.Matrix == null || leaderData.Transform3D.Matrix.Length != 16)
            {
                Console.WriteLine("错误：变换矩阵数据不正确");
                return false;
            }

            // 检查Leader特有属性
            if (leaderData.Normal == null)
            {
                Console.WriteLine("错误：Normal属性缺失");
                return false;
            }

            if (leaderData.HorizontalDirection == null)
            {
                Console.WriteLine("错误：HorizontalDirection属性缺失");
                return false;
            }

            // 检查材质属性
            if (leaderData.Opacity < 0 || leaderData.Opacity > 1)
            {
                Console.WriteLine("错误：Opacity值超出范围");
                return false;
            }

            // 所有检查通过
            return true;
        }

        /// <summary>
        /// 计算箭头点数据
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="nextPoint">下一个点</param>
        private static PointData[] DrawArrowHead(CSMath.XYZ startPoint, CSMath.XYZ nextPoint)
        {
            var arrowPoints = new List<PointData>();
            try
            {
                // 计算箭头方向
                double dx = nextPoint.X - startPoint.X;
                double dy = nextPoint.Y - startPoint.Y;
                double length = Math.Sqrt(dx * dx + dy * dy);
                
                if (length == 0) return arrowPoints.ToArray();
                
                // 标准化方向向量
                dx /= length;
                dy /= length;
                
                // 箭头大小
                double arrowSize = Math.Max(2.0, length * 0.1);
                
                // 计算箭头两边的点
                PointData arrowPoint1 = new PointData(
                    startPoint.X + arrowSize * (dx * 0.866 - dy * 0.5), // cos(30°) ≈ 0.866, sin(30°) = 0.5
                    startPoint.Y + arrowSize * (dy * 0.866 + dx * 0.5)
                );
                
                PointData arrowPoint2 = new PointData(
                    startPoint.X + arrowSize * (dx * 0.866 + dy * 0.5),
                    startPoint.Y + arrowSize * (dy * 0.866 - dx * 0.5)
                );
                
                // 返回箭头点数据
                arrowPoints.Add(new PointData(startPoint.X, startPoint.Y));
                arrowPoints.Add(arrowPoint1);
                arrowPoints.Add(arrowPoint2);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating arrow head: {ex.Message}");
            }
            
            return arrowPoints.ToArray();
        }

        /// <summary>
        /// 计算钩线点数据
        /// </summary>
        /// <param name="vertices">顶点列表</param>
        private static PointData[] DrawHookLine(List<CSMath.XYZ> vertices)
        {
            var hookLinePoints = new List<PointData>();
            try
            {
                Console.WriteLine($"DrawHookLine: vertices.Count = {vertices.Count}");
                // 钩线通常是从最后一个点延伸出去的一小段线
                if (vertices.Count < 2) return hookLinePoints.ToArray();
                
                CSMath.XYZ lastPoint = vertices[vertices.Count - 1];
                CSMath.XYZ prevPoint = vertices[vertices.Count - 2];
                
                Console.WriteLine($"DrawHookLine: lastPoint = ({lastPoint.X}, {lastPoint.Y}, {lastPoint.Z}), prevPoint = ({prevPoint.X}, {prevPoint.Y}, {prevPoint.Z})");
                
                // 计算钩线方向（与最后一条线段垂直）
                double dx = lastPoint.X - prevPoint.X;
                double dy = lastPoint.Y - prevPoint.Y;
                double length = Math.Sqrt(dx * dx + dy * dy);
                
                Console.WriteLine($"DrawHookLine: dx = {dx}, dy = {dy}, length = {length}");
                
                if (length == 0) return hookLinePoints.ToArray();
                
                // 标准化并旋转90度得到垂直方向
                dx /= length;
                dy /= length;
                
                // 钩线长度
                double hookLength = Math.Max(3.0, length * 0.05);
                
                Console.WriteLine($"DrawHookLine: normalized dx = {dx}, dy = {dy}, hookLength = {hookLength}");
                
                // 计算钩线终点
                PointData hookEndPoint = new PointData(
                    lastPoint.X - dy * hookLength, // 垂直方向
                    lastPoint.Y + dx * hookLength
                );
                
                Console.WriteLine($"DrawHookLine: hookEndPoint = ({hookEndPoint.X}, {hookEndPoint.Y})");
                
                // 返回钩线点数据
                hookLinePoints.Add(new PointData(lastPoint.X, lastPoint.Y));
                hookLinePoints.Add(hookEndPoint);
                
                Console.WriteLine($"DrawHookLine: hookLinePoints.Count = {hookLinePoints.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating hook line: {ex.Message}");
            }
            
            return hookLinePoints.ToArray();
        }
    }
}