using System;
using System.Collections.Generic;
using Xunit;
using ACadSharp.Entities;
using ACadSharp.Tables;
using CSMath;
using DxfDwgViewer.RenderUtilities;

namespace DxfDwgViewer.Tests
{
    public class LeaderEntityRendererTests
    {
        [Fact]
        public void Render_Leader_ReturnsCorrectType()
        {
            var leader = CreateBasicLeader();

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal("Leader", result.Type);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectHandle()
        {
            var leader = CreateBasicLeader();

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.Handle);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectLayerName()
        {
            var leader = CreateBasicLeader();
            leader.Layer = new Layer("MY_LAYER");

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal("MY_LAYER", result.LayerName);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectVisibility()
        {
            var leader = CreateBasicLeader();
            leader.IsInvisible = false;

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.True(result.Visible);
        }

        [Fact]
        public void Render_Leader_WithInvisible_ReturnsCorrectVisibility()
        {
            var leader = CreateBasicLeader();
            leader.IsInvisible = true;

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.False(result.Visible);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectVertices()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 0),
                new XYZ(20, 5, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices);
            Assert.Equal(3, result.Vertices.Count);
            Assert.Equal(0, result.Vertices[0].X);
            Assert.Equal(0, result.Vertices[0].Y);
            Assert.Equal(10, result.Vertices[1].X);
            Assert.Equal(10, result.Vertices[1].Y);
            Assert.Equal(20, result.Vertices[2].X);
            Assert.Equal(5, result.Vertices[2].Y);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectVertices3D()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 5),
                new XYZ(20, 5, 10)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices3D);
            Assert.Equal(3, result.Vertices3D.Count);
            Assert.Equal(0, result.Vertices3D[0].X);
            Assert.Equal(0, result.Vertices3D[0].Y);
            Assert.Equal(0, result.Vertices3D[0].Z);
            Assert.Equal(10, result.Vertices3D[1].X);
            Assert.Equal(10, result.Vertices3D[1].Y);
            Assert.Equal(5, result.Vertices3D[1].Z);
            Assert.Equal(20, result.Vertices3D[2].X);
            Assert.Equal(5, result.Vertices3D[2].Y);
            Assert.Equal(10, result.Vertices3D[2].Z);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectVertexCount()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 0),
                new XYZ(20, 5, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal(3, result.VertexCount);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectStartPoint()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(5, 10, 0),
                new XYZ(15, 20, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.StartPoint);
            Assert.Equal(5, result.StartPoint.X);
            Assert.Equal(10, result.StartPoint.Y);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectStartPoint3D()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(5, 10, 15),
                new XYZ(15, 20, 25)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.StartPoint3D);
            Assert.Equal(5, result.StartPoint3D.X);
            Assert.Equal(10, result.StartPoint3D.Y);
            Assert.Equal(15, result.StartPoint3D.Z);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectEndPoint()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(5, 10, 0),
                new XYZ(15, 20, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.EndPoint);
            Assert.Equal(15, result.EndPoint.X);
            Assert.Equal(20, result.EndPoint.Y);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectEndPoint3D()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(5, 10, 15),
                new XYZ(15, 20, 25)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.EndPoint3D);
            Assert.Equal(15, result.EndPoint3D.X);
            Assert.Equal(20, result.EndPoint3D.Y);
            Assert.Equal(25, result.EndPoint3D.Z);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectTotalLength()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 0, 0),
                new XYZ(10, 10, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal(20, result.TotalLength);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectPathType()
        {
            var leader = CreateBasicLeader();
            leader.PathType = LeaderPathType.Spline;

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal("Spline", result.PathType);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectArrowHeadEnabled()
        {
            var leader = CreateBasicLeader();
            leader.ArrowHeadEnabled = true;

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.True(result.ArrowHeadEnabled);
        }

        [Fact]
        public void Render_Leader_WithArrowHeadEnabled_ReturnsArrowHeadPoints()
        {
            var leader = CreateBasicLeader();
            leader.ArrowHeadEnabled = true;
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.ArrowHeadPoints);
            Assert.True(result.ArrowHeadPoints.Count > 0);
        }

        [Fact]
        public void Render_Leader_WithArrowHeadEnabled_ReturnsArrowHeadPoints3D()
        {
            var leader = CreateBasicLeader();
            leader.ArrowHeadEnabled = true;
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.ArrowHeadPoints3D);
            Assert.True(result.ArrowHeadPoints3D.Count > 0);
        }

        [Fact]
        public void Render_Leader_WithoutArrowHeadEnabled_ReturnsEmptyArrowHeadPoints()
        {
            var leader = CreateBasicLeader();
            leader.ArrowHeadEnabled = false;
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.ArrowHeadPoints);
            Assert.Equal(0, result.ArrowHeadPoints.Count);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectHasHookline()
        {
            var leader = CreateBasicLeader();
            leader.HorizontalDirection = new XYZ(-1, 0, 0);
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 0),
                new XYZ(30, 10, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.True(result.HasHookline);
        }

        [Fact]
        public void Render_Leader_WithHasHookline_ReturnsHookLinePoints()
        {
            var leader = CreateBasicLeader();
            leader.HorizontalDirection = new XYZ(-1, 0, 0);
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 0),
                new XYZ(30, 10, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.HookLineStart);
            Assert.NotNull(result.HookLineEnd);
        }

        [Fact]
        public void Render_Leader_WithHasHookline_ReturnsHookLinePoints3D()
        {
            var leader = CreateBasicLeader();
            leader.HorizontalDirection = new XYZ(-1, 0, 0);
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 10),
                new XYZ(30, 10, 10)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.HookLineStart3D);
            Assert.NotNull(result.HookLineEnd3D);
        }

        [Fact]
        public void Render_Leader_WithoutHasHookline_ReturnsNullHookLinePoints()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Null(result.HookLineStart);
            Assert.Null(result.HookLineEnd);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectColorIndex()
        {
            var leader = CreateBasicLeader();
            leader.Color = new ACadSharp.Color(5);

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectColorData()
        {
            var leader = CreateBasicLeader();
            leader.Color = new ACadSharp.Color(5);

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);
            Assert.Equal(5, result.Color.Index);
            Assert.Equal("#0000FF", result.Color.Hex);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectLineTypeName()
        {
            var leader = CreateBasicLeader();
            var lineType = new LineType("CONTINUOUS");
            leader.LineType = lineType;

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal("CONTINUOUS", result.LineTypeName);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectLineWeight()
        {
            var leader = CreateBasicLeader();
            leader.LineWeight = ACadSharp.LineWeightType.W50;

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal(0.5, result.LineWeight);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectBounds()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 20, 5),
                new XYZ(20, 10, 10)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);
            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(0, result.Bounds.Min.Z);
            Assert.Equal(20, result.Bounds.Max.X);
            Assert.Equal(20, result.Bounds.Max.Y);
            Assert.Equal(10, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectTransform3D()
        {
            var leader = CreateBasicLeader();

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform3D);
            Assert.NotNull(result.Transform3D.Matrix);
            Assert.Equal(16, result.Transform3D.Matrix.Length);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectNormal()
        {
            var leader = CreateBasicLeader();
            leader.Normal = new XYZ(0, 0, 1);

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.Normal);
            Assert.Equal(0, result.Normal.X);
            Assert.Equal(0, result.Normal.Y);
            Assert.Equal(1, result.Normal.Z);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectHorizontalDirection()
        {
            var leader = CreateBasicLeader();
            leader.HorizontalDirection = new XYZ(1, 0, 0);

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.HorizontalDirection);
            Assert.Equal(1, result.HorizontalDirection.X);
            Assert.Equal(0, result.HorizontalDirection.Y);
            Assert.Equal(0, result.HorizontalDirection.Z);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectAnnotationOffset()
        {
            var leader = CreateBasicLeader();
            leader.AnnotationOffset = new XYZ(5, 10, 15);

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.AnnotationOffset);
            Assert.Equal(5, result.AnnotationOffset.X);
            Assert.Equal(10, result.AnnotationOffset.Y);
            Assert.Equal(15, result.AnnotationOffset.Z);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectBlockOffset()
        {
            var leader = CreateBasicLeader();
            leader.BlockOffset = new XYZ(3, 6, 9);

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.BlockOffset);
            Assert.Equal(3, result.BlockOffset.X);
            Assert.Equal(6, result.BlockOffset.Y);
            Assert.Equal(9, result.BlockOffset.Z);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectCreationType()
        {
            var leader = CreateBasicLeader();
            leader.CreationType = LeaderCreationType.CreatedWithTextAnnotation;

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal("CreatedWithTextAnnotation", result.CreationType);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectHookLineDirection()
        {
            var leader = CreateBasicLeader();
            leader.HookLineDirection = HookLineDirection.Opposite;

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal("Opposite", result.HookLineDirection);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectTextHeight()
        {
            var leader = CreateBasicLeader();
            leader.TextHeight = 5.0;

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal(5.0, result.TextHeight);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectTextWidth()
        {
            var leader = CreateBasicLeader();
            leader.TextWidth = 10.0;

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal(10.0, result.TextWidth);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectStyleName()
        {
            var leader = CreateBasicLeader();
            var style = new DimensionStyle("MY_STYLE");
            leader.Style = style;

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal("MY_STYLE", result.StyleName);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectMaterialProperties()
        {
            var leader = CreateBasicLeader();

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal("LineBasicMaterial", result.MaterialType);
            Assert.Equal(1.0, result.Opacity);
            Assert.False(result.Transparent);
            Assert.True(result.DepthTest);
            Assert.True(result.DepthWrite);
        }

        [Fact]
        public void Render_Leader_ReturnsCorrectCoordinateSystem()
        {
            var leader = CreateBasicLeader();

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal("World", result.CoordinateSystem);
        }

        [Fact]
        public void Render_Leader_WithNullVertices_ReturnsEmptyVertices()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = null;

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices);
            Assert.Equal(0, result.Vertices.Count);
        }

        [Fact]
        public void Render_Leader_WithLessThanTwoVertices_ReturnsEmptyVertices()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices);
            Assert.Equal(0, result.Vertices.Count);
        }

        [Fact]
        public void Render_Leader_WithNullVertices_ReturnsDefaultBounds()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = null;

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Null(result.Bounds);
        }

        [Fact]
        public void Render_Leader_WithLessThanTwoVertices_ReturnsDefaultBounds()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Null(result.Bounds);
        }

        [Fact]
        public void Render_Leader_WithLessThanTwoVertices_ReturnsEmptyArrowHeadPoints()
        {
            var leader = CreateBasicLeader();
            leader.ArrowHeadEnabled = true;
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.ArrowHeadPoints);
            Assert.Equal(0, result.ArrowHeadPoints.Count);
        }

        [Fact]
        public void Render_Leader_WithLessThanTwoVertices_ReturnsNullHookLinePoints()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0)
            };

            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Null(result.HookLineStart);
            Assert.Null(result.HookLineEnd);
        }

        [Fact]
        public void Test_HasHookline_Calculation()
        {
            var leader = CreateBasicLeader();
            leader.HorizontalDirection = new XYZ(-1, 0, 0);
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 0),
                new XYZ(30, 10, 0)
            };

            var lastVertex = leader.Vertices[leader.Vertices.Count - 1];
            var prevVertex = leader.Vertices[leader.Vertices.Count - 2];
            var direction = prevVertex - lastVertex;
            var angle = direction.AngleBetweenVectors(leader.HorizontalDirection);
            
            Console.WriteLine($"Direction: ({direction.X}, {direction.Y}, {direction.Z})");
            Console.WriteLine($"HorizontalDirection: ({leader.HorizontalDirection.X}, {leader.HorizontalDirection.Y}, {leader.HorizontalDirection.Z})");
            Console.WriteLine($"Angle: {angle}");
            Console.WriteLine($"Angle in degrees: {angle * 180 / Math.PI}");
            Console.WriteLine($"IsZero(angle): {CSMath.MathHelper.IsZero(angle)}");
            Console.WriteLine($"HasHookline: {leader.HasHookline}");
            
            Assert.NotNull(leader.Vertices);
            Assert.Equal(3, leader.Vertices.Count);
            Assert.Equal(new XYZ(-20, 0, 0), direction);
            Assert.Equal(0, angle);
            Assert.True(leader.HasHookline);
        }

        private Leader CreateBasicLeader()
        {
            var leader = new Leader
            {
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER"),
                Normal = new XYZ(0, 0, 1),
                HorizontalDirection = new XYZ(1, 0, 0),
                AnnotationOffset = new XYZ(0, 0, 0),
                BlockOffset = new XYZ(0, 0, 0),
                CreationType = LeaderCreationType.CreatedWithTextAnnotation,
                HookLineDirection = HookLineDirection.Opposite,
                TextHeight = 2.5,
                TextWidth = 5.0,
                Style = new DimensionStyle("Standard"),
                PathType = LeaderPathType.StraightLineSegments,
                ArrowHeadEnabled = false,
                LineWeight = ACadSharp.LineWeightType.W25
            };
            
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 0)
            };

            return leader;
        }
    }
}
