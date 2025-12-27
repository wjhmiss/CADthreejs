using System;
using System.Text.Json;
using Xunit;
using ACadSharp.Entities;
using ACadSharp.Tables;
using CSMath;
using DxfDwgViewer.RenderUtilities;

namespace DxfDwgViewer.Tests
{
    public class LeaderEntityRendererIntegrationTests
    {
        [Fact]
        public void SerializeAndDeserialize_Leader_ShouldPreserveAllProperties()
        {
            var leader = CreateComplexLeader();
            var result = LeaderEntityRenderer.Render(leader);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<LeaderEntityRenderer.LeaderData>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(result.Type, deserialized.Type);
            Assert.Equal(result.Handle, deserialized.Handle);
            Assert.Equal(result.LayerName, deserialized.LayerName);
            Assert.Equal(result.Visible, deserialized.Visible);
            Assert.Equal(result.CoordinateSystem, deserialized.CoordinateSystem);
            Assert.Equal(result.PathType, deserialized.PathType);
            Assert.Equal(result.ArrowHeadEnabled, deserialized.ArrowHeadEnabled);
            Assert.Equal(result.HasHookline, deserialized.HasHookline);
            Assert.Equal(result.ColorIndex, deserialized.ColorIndex);
            Assert.Equal(result.LineTypeName, deserialized.LineTypeName);
            Assert.Equal(result.LineWeight, deserialized.LineWeight);
            Assert.Equal(result.TotalLength, deserialized.TotalLength);
            Assert.Equal(result.VertexCount, deserialized.VertexCount);
            Assert.Equal(result.CreationType, deserialized.CreationType);
            Assert.Equal(result.HookLineDirection, deserialized.HookLineDirection);
            Assert.Equal(result.TextHeight, deserialized.TextHeight);
            Assert.Equal(result.TextWidth, deserialized.TextWidth);
            Assert.Equal(result.StyleName, deserialized.StyleName);
            Assert.Equal(result.Opacity, deserialized.Opacity);
            Assert.Equal(result.Transparent, deserialized.Transparent);
            Assert.Equal(result.MaterialType, deserialized.MaterialType);
            Assert.Equal(result.DepthTest, deserialized.DepthTest);
            Assert.Equal(result.DepthWrite, deserialized.DepthWrite);
        }

        [Fact]
        public void SerializeAndDeserialize_Leader_ShouldPreserveVertices()
        {
            var leader = CreateComplexLeader();
            var result = LeaderEntityRenderer.Render(leader);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<LeaderEntityRenderer.LeaderData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.Vertices);
            Assert.NotNull(deserialized.Vertices3D);
            Assert.Equal(result.Vertices.Count, deserialized.Vertices.Count);
            Assert.Equal(result.Vertices3D.Count, deserialized.Vertices3D.Count);

            for (int i = 0; i < result.Vertices.Count; i++)
            {
                Assert.Equal(result.Vertices[i].X, deserialized.Vertices[i].X);
                Assert.Equal(result.Vertices[i].Y, deserialized.Vertices[i].Y);
            }

            for (int i = 0; i < result.Vertices3D.Count; i++)
            {
                Assert.Equal(result.Vertices3D[i].X, deserialized.Vertices3D[i].X);
                Assert.Equal(result.Vertices3D[i].Y, deserialized.Vertices3D[i].Y);
                Assert.Equal(result.Vertices3D[i].Z, deserialized.Vertices3D[i].Z);
            }
        }

        [Fact]
        public void SerializeAndDeserialize_Leader_ShouldPreserveStartAndEndPoints()
        {
            var leader = CreateComplexLeader();
            var result = LeaderEntityRenderer.Render(leader);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<LeaderEntityRenderer.LeaderData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.StartPoint);
            Assert.NotNull(deserialized.EndPoint);
            Assert.NotNull(deserialized.StartPoint3D);
            Assert.NotNull(deserialized.EndPoint3D);

            Assert.Equal(result.StartPoint.X, deserialized.StartPoint.X);
            Assert.Equal(result.StartPoint.Y, deserialized.StartPoint.Y);
            Assert.Equal(result.EndPoint.X, deserialized.EndPoint.X);
            Assert.Equal(result.EndPoint.Y, deserialized.EndPoint.Y);

            Assert.Equal(result.StartPoint3D.X, deserialized.StartPoint3D.X);
            Assert.Equal(result.StartPoint3D.Y, deserialized.StartPoint3D.Y);
            Assert.Equal(result.StartPoint3D.Z, deserialized.StartPoint3D.Z);
            Assert.Equal(result.EndPoint3D.X, deserialized.EndPoint3D.X);
            Assert.Equal(result.EndPoint3D.Y, deserialized.EndPoint3D.Y);
            Assert.Equal(result.EndPoint3D.Z, deserialized.EndPoint3D.Z);
        }

        [Fact]
        public void SerializeAndDeserialize_Leader_ShouldPreserveArrowHeadPoints()
        {
            var leader = CreateLeaderWithArrowHead();
            var result = LeaderEntityRenderer.Render(leader);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<LeaderEntityRenderer.LeaderData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.ArrowHeadPoints);
            Assert.NotNull(deserialized.ArrowHeadPoints3D);
            Assert.Equal(result.ArrowHeadPoints.Count, deserialized.ArrowHeadPoints.Count);
            Assert.Equal(result.ArrowHeadPoints3D.Count, deserialized.ArrowHeadPoints3D.Count);

            for (int i = 0; i < result.ArrowHeadPoints.Count; i++)
            {
                Assert.Equal(result.ArrowHeadPoints[i].X, deserialized.ArrowHeadPoints[i].X);
                Assert.Equal(result.ArrowHeadPoints[i].Y, deserialized.ArrowHeadPoints[i].Y);
            }

            for (int i = 0; i < result.ArrowHeadPoints3D.Count; i++)
            {
                Assert.Equal(result.ArrowHeadPoints3D[i].X, deserialized.ArrowHeadPoints3D[i].X);
                Assert.Equal(result.ArrowHeadPoints3D[i].Y, deserialized.ArrowHeadPoints3D[i].Y);
                Assert.Equal(result.ArrowHeadPoints3D[i].Z, deserialized.ArrowHeadPoints3D[i].Z);
            }
        }

        [Fact]
        public void SerializeAndDeserialize_Leader_ShouldPreserveHookLinePoints()
        {
            var leader = CreateLeaderWithHookLine();
            var result = LeaderEntityRenderer.Render(leader);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<LeaderEntityRenderer.LeaderData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.HookLineStart);
            Assert.NotNull(deserialized.HookLineEnd);
            Assert.NotNull(deserialized.HookLineStart3D);
            Assert.NotNull(deserialized.HookLineEnd3D);

            Assert.Equal(result.HookLineStart.X, deserialized.HookLineStart.X);
            Assert.Equal(result.HookLineStart.Y, deserialized.HookLineStart.Y);
            Assert.Equal(result.HookLineEnd.X, deserialized.HookLineEnd.X);
            Assert.Equal(result.HookLineEnd.Y, deserialized.HookLineEnd.Y);

            Assert.Equal(result.HookLineStart3D.X, deserialized.HookLineStart3D.X);
            Assert.Equal(result.HookLineStart3D.Y, deserialized.HookLineStart3D.Y);
            Assert.Equal(result.HookLineStart3D.Z, deserialized.HookLineStart3D.Z);
            Assert.Equal(result.HookLineEnd3D.X, deserialized.HookLineEnd3D.X);
            Assert.Equal(result.HookLineEnd3D.Y, deserialized.HookLineEnd3D.Y);
            Assert.Equal(result.HookLineEnd3D.Z, deserialized.HookLineEnd3D.Z);
        }

        [Fact]
        public void SerializeAndDeserialize_Leader_ShouldPreserveBounds()
        {
            var leader = CreateComplexLeader();
            var result = LeaderEntityRenderer.Render(leader);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<LeaderEntityRenderer.LeaderData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.Bounds);
            Assert.NotNull(deserialized.Bounds.Min);
            Assert.NotNull(deserialized.Bounds.Max);
            Assert.NotNull(deserialized.Bounds.Center);

            Assert.Equal(result.Bounds.Min.X, deserialized.Bounds.Min.X);
            Assert.Equal(result.Bounds.Min.Y, deserialized.Bounds.Min.Y);
            Assert.Equal(result.Bounds.Min.Z, deserialized.Bounds.Min.Z);
            Assert.Equal(result.Bounds.Max.X, deserialized.Bounds.Max.X);
            Assert.Equal(result.Bounds.Max.Y, deserialized.Bounds.Max.Y);
            Assert.Equal(result.Bounds.Max.Z, deserialized.Bounds.Max.Z);
            Assert.Equal(result.Bounds.Center.X, deserialized.Bounds.Center.X);
            Assert.Equal(result.Bounds.Center.Y, deserialized.Bounds.Center.Y);
            Assert.Equal(result.Bounds.Center.Z, deserialized.Bounds.Center.Z);
        }

        [Fact]
        public void SerializeAndDeserialize_Leader_ShouldPreserveColor()
        {
            var leader = CreateComplexLeader();
            var result = LeaderEntityRenderer.Render(leader);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<LeaderEntityRenderer.LeaderData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.Color);

            Assert.Equal(result.Color.Index, deserialized.Color.Index);
            Assert.Equal(result.Color.Hex, deserialized.Color.Hex);
            Assert.Equal(result.Color.R, deserialized.Color.R);
            Assert.Equal(result.Color.G, deserialized.Color.G);
            Assert.Equal(result.Color.B, deserialized.Color.B);
            Assert.Equal(result.Color.A, deserialized.Color.A);
        }

        [Fact]
        public void SerializeAndDeserialize_Leader_ShouldPreserveTransform3D()
        {
            var leader = CreateComplexLeader();
            var result = LeaderEntityRenderer.Render(leader);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<LeaderEntityRenderer.LeaderData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.Transform3D);

            Assert.Equal(result.Transform3D.Position.X, deserialized.Transform3D.Position.X);
            Assert.Equal(result.Transform3D.Position.Y, deserialized.Transform3D.Position.Y);
            Assert.Equal(result.Transform3D.Position.Z, deserialized.Transform3D.Position.Z);
            Assert.Equal(result.Transform3D.Rotation.X, deserialized.Transform3D.Rotation.X);
            Assert.Equal(result.Transform3D.Rotation.Y, deserialized.Transform3D.Rotation.Y);
            Assert.Equal(result.Transform3D.Rotation.Z, deserialized.Transform3D.Rotation.Z);
            Assert.Equal(result.Transform3D.Scale.X, deserialized.Transform3D.Scale.X);
            Assert.Equal(result.Transform3D.Scale.Y, deserialized.Transform3D.Scale.Y);
            Assert.Equal(result.Transform3D.Scale.Z, deserialized.Transform3D.Scale.Z);

            Assert.Equal(result.Transform3D.Matrix.Length, deserialized.Transform3D.Matrix.Length);
            for (int i = 0; i < result.Transform3D.Matrix.Length; i++)
            {
                Assert.Equal(result.Transform3D.Matrix[i], deserialized.Transform3D.Matrix[i]);
            }
        }

        [Fact]
        public void SerializeAndDeserialize_Leader_ShouldPreserveNormal()
        {
            var leader = CreateComplexLeader();
            var result = LeaderEntityRenderer.Render(leader);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<LeaderEntityRenderer.LeaderData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.Normal);

            Assert.Equal(result.Normal.X, deserialized.Normal.X);
            Assert.Equal(result.Normal.Y, deserialized.Normal.Y);
            Assert.Equal(result.Normal.Z, deserialized.Normal.Z);
        }

        [Fact]
        public void SerializeAndDeserialize_Leader_ShouldPreserveHorizontalDirection()
        {
            var leader = CreateComplexLeader();
            var result = LeaderEntityRenderer.Render(leader);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<LeaderEntityRenderer.LeaderData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.HorizontalDirection);

            Assert.Equal(result.HorizontalDirection.X, deserialized.HorizontalDirection.X);
            Assert.Equal(result.HorizontalDirection.Y, deserialized.HorizontalDirection.Y);
            Assert.Equal(result.HorizontalDirection.Z, deserialized.HorizontalDirection.Z);
        }

        [Fact]
        public void SerializeAndDeserialize_Leader_ShouldPreserveAnnotationOffset()
        {
            var leader = CreateComplexLeader();
            var result = LeaderEntityRenderer.Render(leader);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<LeaderEntityRenderer.LeaderData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.AnnotationOffset);

            Assert.Equal(result.AnnotationOffset.X, deserialized.AnnotationOffset.X);
            Assert.Equal(result.AnnotationOffset.Y, deserialized.AnnotationOffset.Y);
            Assert.Equal(result.AnnotationOffset.Z, deserialized.AnnotationOffset.Z);
        }

        [Fact]
        public void SerializeAndDeserialize_Leader_ShouldPreserveBlockOffset()
        {
            var leader = CreateComplexLeader();
            var result = LeaderEntityRenderer.Render(leader);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<LeaderEntityRenderer.LeaderData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.BlockOffset);

            Assert.Equal(result.BlockOffset.X, deserialized.BlockOffset.X);
            Assert.Equal(result.BlockOffset.Y, deserialized.BlockOffset.Y);
            Assert.Equal(result.BlockOffset.Z, deserialized.BlockOffset.Z);
        }

        [Fact]
        public void Render_LeaderWithMultipleVertices_ShouldCalculateCorrectTotalLength()
        {
            var leader = CreateComplexLeader();
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal(30, result.TotalLength);
        }

        [Fact]
        public void Render_LeaderWithArrowHead_ShouldIncludeArrowHeadPoints()
        {
            var leader = CreateLeaderWithArrowHead();
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.True(result.ArrowHeadEnabled);
            Assert.NotNull(result.ArrowHeadPoints);
            Assert.True(result.ArrowHeadPoints.Count > 0);
            Assert.NotNull(result.ArrowHeadPoints3D);
            Assert.True(result.ArrowHeadPoints3D.Count > 0);
        }

        [Fact]
        public void Render_LeaderWithHookLine_ShouldIncludeHookLinePoints()
        {
            var leader = CreateLeaderWithHookLine();
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.True(result.HasHookline);
            Assert.NotNull(result.HookLineStart);
            Assert.NotNull(result.HookLineEnd);
            Assert.NotNull(result.HookLineStart3D);
            Assert.NotNull(result.HookLineEnd3D);
        }

        [Fact]
        public void Render_LeaderWithSplinePathType_ShouldReturnCorrectPathType()
        {
            var leader = CreateComplexLeader();
            leader.PathType = LeaderPathType.Spline;
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal("Spline", result.PathType);
        }

        [Fact]
        public void Render_LeaderWithStraightLinePathType_ShouldReturnCorrectPathType()
        {
            var leader = CreateComplexLeader();
            leader.PathType = LeaderPathType.StraightLineSegments;
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal("StraightLineSegments", result.PathType);
        }

        [Fact]
        public void Render_LeaderWithDifferentCreationTypes_ShouldReturnCorrectCreationType()
        {
            var leader = CreateComplexLeader();

            leader.CreationType = LeaderCreationType.CreatedWithTextAnnotation;
            var result1 = LeaderEntityRenderer.Render(leader);
            Assert.Equal("CreatedWithTextAnnotation", result1.CreationType);

            leader.CreationType = LeaderCreationType.CreatedWithoutAnnotation;
            var result2 = LeaderEntityRenderer.Render(leader);
            Assert.Equal("CreatedWithoutAnnotation", result2.CreationType);
        }

        [Fact]
        public void Render_LeaderWithDifferentHookLineDirections_ShouldReturnCorrectDirection()
        {
            var leader = CreateComplexLeader();

            leader.HookLineDirection = HookLineDirection.Opposite;
            var result1 = LeaderEntityRenderer.Render(leader);
            Assert.Equal("Opposite", result1.HookLineDirection);

            leader.HookLineDirection = HookLineDirection.Same;
            var result2 = LeaderEntityRenderer.Render(leader);
            Assert.Equal("Same", result2.HookLineDirection);
        }

        [Fact]
        public void Render_LeaderWithDifferentTextProperties_ShouldReturnCorrectProperties()
        {
            var leader = CreateComplexLeader();
            leader.TextHeight = 5.0;
            leader.TextWidth = 10.0;
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal(5.0, result.TextHeight);
            Assert.Equal(10.0, result.TextWidth);
        }

        [Fact]
        public void Render_LeaderWithDifferentStyles_ShouldReturnCorrectStyleName()
        {
            var leader = CreateComplexLeader();
            var style = new DimensionStyle("MY_STYLE");
            leader.Style = style;
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal("MY_STYLE", result.StyleName);
        }

        [Fact]
        public void Render_LeaderWithDifferentColors_ShouldReturnCorrectColorData()
        {
            var leader = CreateComplexLeader();
            leader.Color = new ACadSharp.Color(5);
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
            Assert.Equal("#0000FF", result.Color.Hex);
            Assert.Equal(0, result.Color.R);
            Assert.Equal(0, result.Color.G);
            Assert.Equal(255, result.Color.B);
            Assert.Equal(1.0, result.Color.A);
        }

        [Fact]
        public void Render_LeaderWithDifferentLineTypes_ShouldReturnCorrectLineTypeName()
        {
            var leader = CreateComplexLeader();
            var lineType = new LineType("DASHED");
            leader.LineType = lineType;
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal("DASHED", result.LineTypeName);
        }

        [Fact]
        public void Render_LeaderWithDifferentLineWeights_ShouldReturnCorrectLineWeight()
        {
            var leader = CreateComplexLeader();
            leader.LineWeight = ACadSharp.LineWeightType.W50;
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal(0.5, result.LineWeight);
        }

        [Fact]
        public void Render_LeaderWithDifferentLayers_ShouldReturnCorrectLayerName()
        {
            var leader = CreateComplexLeader();
            leader.Layer = new Layer("MY_LAYER");
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.Equal("MY_LAYER", result.LayerName);
        }

        [Fact]
        public void Render_LeaderWithInvisibleFlag_ShouldReturnCorrectVisibility()
        {
            var leader = CreateComplexLeader();
            leader.IsInvisible = true;
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.False(result.Visible);
        }

        [Fact]
        public void Render_LeaderWith3DVertices_ShouldCalculateCorrectBounds()
        {
            var leader = CreateComplexLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 5),
                new XYZ(20, 5, 10),
                new XYZ(30, 15, 15)
            };
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(0, result.Bounds.Min.Z);
            Assert.Equal(30, result.Bounds.Max.X);
            Assert.Equal(15, result.Bounds.Max.Y);
            Assert.Equal(15, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_LeaderWithSingleVertex_ShouldHandleGracefully()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(10, 10, 0)
            };
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices);
            Assert.Equal(0, result.Vertices.Count);
        }

        [Fact]
        public void Render_LeaderWithNullVertices_ShouldHandleGracefully()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = null;
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices);
            Assert.Equal(0, result.Vertices.Count);
        }

        [Fact]
        public void Render_LeaderWithArrowHeadAndHookLine_ShouldIncludeBoth()
        {
            var leader = CreateComplexLeader();
            leader.ArrowHeadEnabled = true;
            leader.HorizontalDirection = new XYZ(-1, 0, 0);
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 0),
                new XYZ(30, 10, 0)
            };
            var result = LeaderEntityRenderer.Render(leader);

            Assert.NotNull(result);
            Assert.True(result.ArrowHeadEnabled);
            Assert.True(result.HasHookline);
            Assert.NotNull(result.ArrowHeadPoints);
            Assert.True(result.ArrowHeadPoints.Count > 0);
            Assert.NotNull(result.HookLineStart);
            Assert.NotNull(result.HookLineEnd);
        }

        [Fact]
        public void TestJsonSerialization_LeaderData_ShouldPreserveThreeJSCompatibility()
        {
            var leader = CreateComplexLeader();
            var result = LeaderEntityRenderer.Render(leader);

            var deserialized = LeaderEntityRenderer.TestJsonSerialization(result);

            Assert.NotNull(deserialized);
            Assert.Equal(result.Type, deserialized.Type);
            Assert.Equal(result.Handle, deserialized.Handle);
            Assert.NotNull(deserialized.Vertices3D);
            Assert.True(deserialized.Vertices3D.Count > 0);
            Assert.NotNull(deserialized.Color);
            Assert.NotNull(deserialized.Bounds);
            Assert.NotNull(deserialized.Transform3D);
            Assert.NotNull(deserialized.Normal);
            Assert.NotNull(deserialized.HorizontalDirection);
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

            return leader;
        }

        private Leader CreateComplexLeader()
        {
            var leader = CreateBasicLeader();
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 0, 0),
                new XYZ(10, 10, 0),
                new XYZ(20, 10, 0)
            };
            leader.Color = new ACadSharp.Color(3);
            leader.Layer = new Layer("MY_LAYER");
            leader.LineType = new LineType("CONTINUOUS");
            leader.LineWeight = ACadSharp.LineWeightType.W30;
            leader.Style = new DimensionStyle("MY_STYLE");

            return leader;
        }

        private Leader CreateLeaderWithArrowHead()
        {
            var leader = CreateComplexLeader();
            leader.ArrowHeadEnabled = true;
            return leader;
        }

        private Leader CreateLeaderWithHookLine()
        {
            var leader = CreateComplexLeader();
            leader.HorizontalDirection = new XYZ(-1, 0, 0);
            leader.Vertices = new List<XYZ>
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 10, 0),
                new XYZ(30, 10, 0)
            };
            return leader;
        }
    }
}
