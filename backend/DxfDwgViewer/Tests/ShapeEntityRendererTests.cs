using System;
using Xunit;
using ACadSharp.Entities;
using ACadSharp.Tables;
using ACadSharp;
using CSMath;
using DxfDwgViewer.RenderUtilities;

namespace DxfDwgViewer.Tests
{
    public class ShapeEntityRendererTests
    {
        private Shape CreateBasicShape()
        {
            var textStyle = new TextStyle("ShapeStyle");
            textStyle.Flags = StyleFlags.IsShape;
            
            var shape = new Shape(textStyle);
            shape.InsertionPoint = new XYZ(0, 0, 0);
            shape.Size = 1.0;
            shape.Rotation = 0.0;
            shape.RelativeXScale = 1.0;
            shape.ObliqueAngle = 0.0;
            shape.Normal = new XYZ(0, 0, 1);
            shape.Color = new Color(256);
            shape.Thickness = 0.0;
            return shape;
        }

        [Fact]
        public void Render_Shape_ReturnsCorrectType()
        {
            var shape = CreateBasicShape();
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(0, result.ShapeIndex);
        }

        [Fact]
        public void Render_Shape_ReturnsCorrectInsertionPoint()
        {
            var shape = CreateBasicShape();
            shape.InsertionPoint = new XYZ(100, 200, 50);
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(100, result.InsertionPoint.X);
            Assert.Equal(200, result.InsertionPoint.Y);
            Assert.Equal(50, result.InsertionPoint.Z);
        }

        [Fact]
        public void Render_Shape_ReturnsCorrectSize()
        {
            var shape = CreateBasicShape();
            shape.Size = 5.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(5.0, result.Size);
        }

        [Fact]
        public void Render_Shape_ReturnsCorrectRotation()
        {
            var shape = CreateBasicShape();
            shape.Rotation = Math.PI / 4;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 4, result.Rotation);
        }

        [Fact]
        public void Render_Shape_ReturnsCorrectRelativeXScale()
        {
            var shape = CreateBasicShape();
            shape.RelativeXScale = 2.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(2.0, result.RelativeXScale);
        }

        [Fact]
        public void Render_Shape_ReturnsCorrectObliqueAngle()
        {
            var shape = CreateBasicShape();
            shape.ObliqueAngle = Math.PI / 6;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 6, result.ObliqueAngle);
        }

        [Fact]
        public void Render_Shape_ReturnsCorrectNormal()
        {
            var shape = CreateBasicShape();
            shape.Normal = new XYZ(0.5, 0.5, 0.7071);
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(0.5, result.Normal.X, 4);
            Assert.Equal(0.5, result.Normal.Y, 4);
            Assert.Equal(0.7071, result.Normal.Z, 4);
        }

        [Fact]
        public void Render_Shape_ReturnsCorrectColorIndex()
        {
            var shape = CreateBasicShape();
            shape.Color = new Color(1);
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(1, result.ColorIndex);
        }

        [Fact]
        public void Render_Shape_ReturnsCorrectLineTypeName()
        {
            var shape = CreateBasicShape();
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal("", result.LineTypeName);
        }

        [Fact]
        public void Render_Shape_ReturnsCorrectLineWeight()
        {
            var shape = CreateBasicShape();
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(0.0, result.LineWeight);
        }

        [Fact]
        public void Render_Shape_ReturnsCorrectLineTypeScale()
        {
            var shape = CreateBasicShape();
            shape.LineTypeScale = 2.5;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(2.5, result.LineTypeScale);
        }

        [Fact]
        public void Render_Shape_ReturnsCorrectShapeStyleName()
        {
            var shape = CreateBasicShape();
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal("", result.ShapeStyleName);
        }

        [Fact]
        public void Render_Shape_ReturnsCorrectThickness()
        {
            var shape = CreateBasicShape();
            shape.Thickness = 10.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(10.0, result.Thickness);
        }

        [Fact]
        public void Render_Shape_CalculatesBoundaryPointsCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Size = 2.0;
            shape.RelativeXScale = 1.0;
            shape.Rotation = 0.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(4, result.BoundaryPointCount);
            Assert.Equal(4, result.BoundaryPoints.Count);

            Assert.Equal(-1.0, result.BoundaryPoints[0].X);
            Assert.Equal(-1.0, result.BoundaryPoints[0].Y);
            Assert.Equal(0.0, result.BoundaryPoints[0].Z);

            Assert.Equal(1.0, result.BoundaryPoints[1].X);
            Assert.Equal(-1.0, result.BoundaryPoints[1].Y);
            Assert.Equal(0.0, result.BoundaryPoints[1].Z);

            Assert.Equal(1.0, result.BoundaryPoints[2].X);
            Assert.Equal(1.0, result.BoundaryPoints[2].Y);
            Assert.Equal(0.0, result.BoundaryPoints[2].Z);

            Assert.Equal(-1.0, result.BoundaryPoints[3].X);
            Assert.Equal(1.0, result.BoundaryPoints[3].Y);
            Assert.Equal(0.0, result.BoundaryPoints[3].Z);
        }

        [Fact]
        public void Render_Shape_CalculatesRotatedBoundaryPointsCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Size = 2.0;
            shape.RelativeXScale = 1.0;
            shape.Rotation = Math.PI / 2;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(4, result.BoundaryPointCount);

            Assert.Equal(1.0, result.BoundaryPoints[0].X, 4);
            Assert.Equal(-1.0, result.BoundaryPoints[0].Y, 4);

            Assert.Equal(1.0, result.BoundaryPoints[1].X, 4);
            Assert.Equal(1.0, result.BoundaryPoints[1].Y, 4);

            Assert.Equal(-1.0, result.BoundaryPoints[2].X, 4);
            Assert.Equal(1.0, result.BoundaryPoints[2].Y, 4);

            Assert.Equal(-1.0, result.BoundaryPoints[3].X, 4);
            Assert.Equal(-1.0, result.BoundaryPoints[3].Y, 4);
        }

        [Fact]
        public void Render_Shape_CalculatesScaledBoundaryPointsCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Size = 2.0;
            shape.RelativeXScale = 2.0;
            shape.Rotation = 0.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(4, result.BoundaryPointCount);

            Assert.Equal(-2.0, result.BoundaryPoints[0].X);
            Assert.Equal(-1.0, result.BoundaryPoints[0].Y);

            Assert.Equal(2.0, result.BoundaryPoints[1].X);
            Assert.Equal(-1.0, result.BoundaryPoints[1].Y);

            Assert.Equal(2.0, result.BoundaryPoints[2].X);
            Assert.Equal(1.0, result.BoundaryPoints[2].Y);

            Assert.Equal(-2.0, result.BoundaryPoints[3].X);
            Assert.Equal(1.0, result.BoundaryPoints[3].Y);
        }

        [Fact]
        public void Render_Shape_CalculatesBoundsCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Size = 2.0;
            shape.RelativeXScale = 1.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);

            Assert.Equal(-1.0, result.Bounds.Min.X);
            Assert.Equal(-1.0, result.Bounds.Min.Y);
            Assert.Equal(0.0, result.Bounds.Min.Z);

            Assert.Equal(1.0, result.Bounds.Max.X);
            Assert.Equal(1.0, result.Bounds.Max.Y);
            Assert.Equal(0.0, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_Shape_CalculatesCentroidCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Size = 2.0;
            shape.RelativeXScale = 1.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);

            Assert.Equal(0.0, result.Centroid.X);
            Assert.Equal(0.0, result.Centroid.Y);
            Assert.Equal(0.0, result.Centroid.Z);
        }

        [Fact]
        public void Render_Shape_CalculatesWidthCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Size = 4.0;
            shape.RelativeXScale = 2.5;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(10.0, result.Width);
        }

        [Fact]
        public void Render_Shape_CalculatesHeightCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Size = 4.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(4.0, result.Height);
        }

        [Fact]
        public void Render_Shape_CreatesTransformCorrectly()
        {
            var shape = CreateBasicShape();
            shape.InsertionPoint = new XYZ(10, 20, 30);
            shape.Size = 2.0;
            shape.RelativeXScale = 1.5;
            shape.Rotation = Math.PI / 4;
            shape.Thickness = 5.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Position);
            Assert.NotNull(result.Transform.Rotation);
            Assert.NotNull(result.Transform.Scale);

            Assert.Equal(10.0, result.Transform.Position.X);
            Assert.Equal(20.0, result.Transform.Position.Y);
            Assert.Equal(30.0, result.Transform.Position.Z);

            Assert.Equal(0.0, result.Transform.Rotation.X);
            Assert.Equal(0.0, result.Transform.Rotation.Y);
            Assert.Equal(Math.PI / 4, result.Transform.Rotation.Z);

            Assert.Equal(3.0, result.Transform.Scale.X);
            Assert.Equal(2.0, result.Transform.Scale.Y);
            Assert.Equal(5.0, result.Transform.Scale.Z);
        }

        [Fact]
        public void Render_Shape_CreatesTransformMatrixCorrectly()
        {
            var shape = CreateBasicShape();
            shape.InsertionPoint = new XYZ(10, 20, 30);
            shape.Size = 2.0;
            shape.RelativeXScale = 1.5;
            shape.Rotation = Math.PI / 4;
            shape.Thickness = 5.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform.Matrix);
            Assert.Equal(16, result.Transform.Matrix.Length);

            double cos = Math.Cos(Math.PI / 4);
            double sin = Math.Sin(Math.PI / 4);

            Assert.Equal(3.0 * cos, result.Transform.Matrix[0], 4);
            Assert.Equal(3.0 * sin, result.Transform.Matrix[1], 4);
            Assert.Equal(0.0, result.Transform.Matrix[2]);
            Assert.Equal(0.0, result.Transform.Matrix[3]);

            Assert.Equal(-2.0 * sin, result.Transform.Matrix[4], 4);
            Assert.Equal(2.0 * cos, result.Transform.Matrix[5], 4);
            Assert.Equal(0.0, result.Transform.Matrix[6]);
            Assert.Equal(0.0, result.Transform.Matrix[7]);

            Assert.Equal(0.0, result.Transform.Matrix[8]);
            Assert.Equal(0.0, result.Transform.Matrix[9]);
            Assert.Equal(5.0, result.Transform.Matrix[10]);
            Assert.Equal(0.0, result.Transform.Matrix[11]);

            Assert.Equal(10.0, result.Transform.Matrix[12]);
            Assert.Equal(20.0, result.Transform.Matrix[13]);
            Assert.Equal(30.0, result.Transform.Matrix[14]);
            Assert.Equal(1.0, result.Transform.Matrix[15]);
        }

        [Fact]
        public void Render_Shape_CreatesGeometryCorrectly_WithoutThickness()
        {
            var shape = CreateBasicShape();
            shape.Size = 2.0;
            shape.Thickness = 0.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);

            Assert.Equal("BufferGeometry", result.Geometry.Type);
            Assert.Equal(4, result.Geometry.VertexCount);
            Assert.Equal(0, result.Geometry.FaceCount);
            Assert.True(result.Geometry.HasNormals);
            Assert.True(result.Geometry.HasColors);
            Assert.False(result.Geometry.HasUVs);
            Assert.True(result.Geometry.HasIndices);
            Assert.Equal("LineLoop", result.Geometry.PrimitiveType);
            Assert.Equal(8, result.Geometry.IndexCount);
        }

        [Fact]
        public void Render_Shape_CreatesGeometryCorrectly_WithThickness()
        {
            var shape = CreateBasicShape();
            shape.Size = 2.0;
            shape.Thickness = 5.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);

            Assert.Equal("ExtrudeGeometry", result.Geometry.Type);
            Assert.Equal(4, result.Geometry.VertexCount);
            Assert.Equal(2, result.Geometry.FaceCount);
            Assert.True(result.Geometry.HasNormals);
            Assert.True(result.Geometry.HasColors);
            Assert.False(result.Geometry.HasUVs);
            Assert.True(result.Geometry.HasIndices);
            Assert.Equal("Triangles", result.Geometry.PrimitiveType);
            Assert.Equal(8, result.Geometry.IndexCount);
        }

        [Fact]
        public void Render_Shape_CreatesMaterialCorrectly_WithoutThickness()
        {
            var shape = CreateBasicShape();
            shape.Thickness = 0.0;
            shape.Color = new Color(1);
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);

            Assert.Equal("LineBasicMaterial", result.Material.Type);
            Assert.Equal(0xFF0000, result.Material.Color);
            Assert.Equal(1.0, result.Material.Opacity);
            Assert.False(result.Material.Transparent);
            Assert.True(result.Material.Wireframe);
            Assert.False(result.Material.VertexColors);
            Assert.True(result.Material.Side);
        }

        [Fact]
        public void Render_Shape_CreatesMaterialCorrectly_WithThickness()
        {
            var shape = CreateBasicShape();
            shape.Thickness = 5.0;
            shape.Color = new Color(1);
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);

            Assert.Equal("MeshBasicMaterial", result.Material.Type);
            Assert.Equal(0xFF0000, result.Material.Color);
            Assert.Equal(1.0, result.Material.Opacity);
            Assert.False(result.Material.Transparent);
            Assert.True(result.Material.Wireframe);
            Assert.False(result.Material.VertexColors);
            Assert.True(result.Material.Side);
        }

        [Fact]
        public void Render_Shape_CreatesColorCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Color = new Color(1);
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);

            Assert.Equal(1, result.Color.Index);
            Assert.Equal("#FF0000", result.Color.Hex);
            Assert.Equal((byte)255, result.Color.R);
            Assert.Equal((byte)0, result.Color.G);
            Assert.Equal((byte)0, result.Color.B);
        }

        [Fact]
        public void Render_Shape_CreatesVertexPositionsCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Size = 2.0;
            shape.RelativeXScale = 1.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexPositions);
            Assert.Equal(12, result.VertexPositions.Count);

            Assert.Equal(-1.0, result.VertexPositions[0]);
            Assert.Equal(-1.0, result.VertexPositions[1]);
            Assert.Equal(0.0, result.VertexPositions[2]);

            Assert.Equal(1.0, result.VertexPositions[3]);
            Assert.Equal(-1.0, result.VertexPositions[4]);
            Assert.Equal(0.0, result.VertexPositions[5]);

            Assert.Equal(1.0, result.VertexPositions[6]);
            Assert.Equal(1.0, result.VertexPositions[7]);
            Assert.Equal(0.0, result.VertexPositions[8]);

            Assert.Equal(-1.0, result.VertexPositions[9]);
            Assert.Equal(1.0, result.VertexPositions[10]);
            Assert.Equal(0.0, result.VertexPositions[11]);
        }

        [Fact]
        public void Render_Shape_CreatesVertexNormalsCorrectly()
        {
            var shape = CreateBasicShape();
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexNormals);
            Assert.Equal(12, result.VertexNormals.Count);

            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(0.0, result.VertexNormals[i * 3]);
                Assert.Equal(0.0, result.VertexNormals[i * 3 + 1]);
                Assert.Equal(1.0, result.VertexNormals[i * 3 + 2]);
            }
        }

        [Fact]
        public void Render_Shape_CreatesVertexColorsCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Color = new Color(1);
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexColors);
            Assert.Equal(12, result.VertexColors.Count);

            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(1.0, result.VertexColors[i * 3]);
                Assert.Equal(0.0, result.VertexColors[i * 3 + 1]);
                Assert.Equal(0.0, result.VertexColors[i * 3 + 2]);
            }
        }

        [Fact]
        public void Render_Shape_CreatesIndicesCorrectly()
        {
            var shape = CreateBasicShape();
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.Indices);
            Assert.Equal(8, result.Indices.Count);

            Assert.Equal(0, result.Indices[0]);
            Assert.Equal(1, result.Indices[1]);
            Assert.Equal(1, result.Indices[2]);
            Assert.Equal(2, result.Indices[3]);
            Assert.Equal(2, result.Indices[4]);
            Assert.Equal(3, result.Indices[5]);
            Assert.Equal(3, result.Indices[6]);
            Assert.Equal(0, result.Indices[7]);
        }

        [Fact]
        public void Render_Shape_WithDifferentColors_ReturnsCorrectColor()
        {
            var shape = CreateBasicShape();

            shape.Color = new Color(1);
            var redResult = ShapeEntityRenderer.Render(shape);
            Assert.Equal("#FF0000", redResult.Color.Hex);

            shape.Color = new Color(3);
            var greenResult = ShapeEntityRenderer.Render(shape);
            Assert.Equal("#00FF00", greenResult.Color.Hex);

            shape.Color = new Color(5);
            var blueResult = ShapeEntityRenderer.Render(shape);
            Assert.Equal("#0000FF", blueResult.Color.Hex);

            shape.Color = new Color(7);
            var whiteResult = ShapeEntityRenderer.Render(shape);
            Assert.Equal("#FFFFFF", whiteResult.Color.Hex);
        }

        [Fact]
        public void Render_Shape_WithZeroSize_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Size = 0.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(0.0, result.Size);
            Assert.Equal(0.0, result.Width);
            Assert.Equal(0.0, result.Height);
            Assert.Equal(4, result.BoundaryPointCount);
        }

        [Fact]
        public void Render_Shape_WithNegativeRotation_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Size = 2.0;
            shape.Rotation = -Math.PI / 4;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(-Math.PI / 4, result.Rotation);
            Assert.Equal(4, result.BoundaryPointCount);
        }

        [Fact]
        public void Render_Shape_WithLargeScale_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Size = 100.0;
            shape.RelativeXScale = 10.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(1000.0, result.Width);
            Assert.Equal(100.0, result.Height);
            Assert.Equal(4, result.BoundaryPointCount);
        }

        [Fact]
        public void Render_Shape_WithZeroNormal_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Normal = new XYZ(0, 0, 0);
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(0.0, result.Normal.X);
            Assert.Equal(0.0, result.Normal.Y);
            Assert.Equal(1.0, result.Normal.Z);
        }

        [Fact]
        public void Render_Shape_WithNonZeroInsertionPoint_TranslatesBoundaryPointsCorrectly()
        {
            var shape = CreateBasicShape();
            shape.InsertionPoint = new XYZ(100, 200, 50);
            shape.Size = 2.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.Equal(4, result.BoundaryPointCount);

            Assert.Equal(99.0, result.BoundaryPoints[0].X);
            Assert.Equal(199.0, result.BoundaryPoints[0].Y);
            Assert.Equal(50.0, result.BoundaryPoints[0].Z);
        }

        [Fact]
        public void Render_Shape_WithThickness_UpdatesTransformScaleZ()
        {
            var shape = CreateBasicShape();
            shape.Size = 2.0;
            shape.Thickness = 10.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.Equal(10.0, result.Transform.Scale.Z);
        }

        [Fact]
        public void Render_Shape_WithoutThickness_UsesDefaultScaleZ()
        {
            var shape = CreateBasicShape();
            shape.Size = 2.0;
            shape.Thickness = 0.0;
            var result = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.Equal(1.0, result.Transform.Scale.Z);
        }
    }
}
