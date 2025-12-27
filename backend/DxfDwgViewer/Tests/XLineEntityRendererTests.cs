using System;
using Xunit;
using ACadSharp.Entities;
using ACadSharp;
using CSMath;
using DxfDwgViewer.RenderUtilities;

namespace DxfDwgViewer.Tests
{
    public class XLineEntityRendererTests
    {
        private XLine CreateBasicXLine()
        {
            var xline = new XLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);
            xline.Color = new Color(1);
            return xline;
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectFirstPoint()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(100, 200, 50);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.Equal(100, result.FirstPoint.X);
            Assert.Equal(200, result.FirstPoint.Y);
            Assert.Equal(50, result.FirstPoint.Z);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectDirection()
        {
            var xline = CreateBasicXLine();
            xline.Direction = new XYZ(0.5, 0.866, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.Equal(0.5, result.Direction.X);
            Assert.Equal(0.866, result.Direction.Y);
            Assert.Equal(0, result.Direction.Z);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectColorIndex()
        {
            var xline = CreateBasicXLine();
            xline.Color = new Color(1);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.Equal(1, result.ColorIndex);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectLineTypeName()
        {
            var xline = CreateBasicXLine();
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.LineTypeName);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectLineWeight()
        {
            var xline = CreateBasicXLine();
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.True(result.LineWeight >= 0);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectSecondPoint()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.SecondPoint);
            Assert.Equal(1000, result.SecondPoint.X);
            Assert.Equal(0, result.SecondPoint.Y);
            Assert.Equal(0, result.SecondPoint.Z);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectLength()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.Equal(1000, result.Length);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectAngle()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.Equal(0, result.Angle);
        }

        [Fact]
        public void Render_XLine_DiagonalDirection_ReturnsCorrectAngle()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 1, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 4, result.Angle);
        }

        [Fact]
        public void Render_XLine_VerticalDirection_ReturnsCorrectAngle()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(0, 1, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 2, result.Angle);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectTransform()
        {
            var xline = CreateBasicXLine();
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Position);
            Assert.NotNull(result.Transform.Rotation);
            Assert.NotNull(result.Transform.Scale);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectGeometry()
        {
            var xline = CreateBasicXLine();
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);
            Assert.Equal("BufferGeometry", result.Geometry.Type);
            Assert.Equal(2, result.Geometry.VertexCount);
            Assert.True(result.Geometry.HasColors);
            Assert.True(result.Geometry.HasIndices);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectMaterial()
        {
            var xline = CreateBasicXLine();
            xline.Color = new Color(1);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);
            Assert.Equal("LineBasicMaterial", result.Material.Type);
            Assert.Equal(0xFF0000, result.Material.Color);
            Assert.Equal(1.0, result.Material.Opacity);
            Assert.False(result.Material.Transparent);
            Assert.True(result.Material.Side);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectVertexPositions()
        {
            var xline = CreateBasicXLine();
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexPositions);
            Assert.Equal(6, result.VertexPositions.Length);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectVertexColors()
        {
            var xline = CreateBasicXLine();
            xline.Color = new Color(1);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexColors);
            Assert.Equal(6, result.VertexColors.Length);
            Assert.Equal(1.0, result.VertexColors[0]);
            Assert.Equal(0.0, result.VertexColors[1]);
            Assert.Equal(0.0, result.VertexColors[2]);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectIndices()
        {
            var xline = CreateBasicXLine();
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.Indices);
            Assert.Equal(2, result.Indices.Length);
            Assert.Equal(0, result.Indices[0]);
            Assert.Equal(1, result.Indices[1]);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectVertexCount()
        {
            var xline = CreateBasicXLine();
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.Equal(2, result.VertexCount);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectOpacity()
        {
            var xline = CreateBasicXLine();
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.Equal(1.0, result.Opacity);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectTransparent()
        {
            var xline = CreateBasicXLine();
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.False(result.Transparent);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectNormal()
        {
            var xline = CreateBasicXLine();
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.Normal);
            Assert.Equal(0, result.Normal.X);
            Assert.Equal(0, result.Normal.Y);
            Assert.Equal(1, result.Normal.Z);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectBounds()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);
            Assert.Equal(-1000, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(0, result.Bounds.Min.Z);
            Assert.Equal(1000, result.Bounds.Max.X);
            Assert.Equal(0, result.Bounds.Max.Y);
            Assert.Equal(0, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectCenter()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.Center);
            Assert.Equal(0, result.Center.X);
            Assert.Equal(0, result.Center.Y);
            Assert.Equal(0, result.Center.Z);
        }

        [Fact]
        public void Render_XLine_WithDifferentColorIndex_CreatesCorrectColor()
        {
            var xline = CreateBasicXLine();
            xline.Color = new Color(3);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.Equal(3, result.ColorIndex);
            Assert.Equal(0x00FF00, result.Material.Color);
        }

        [Fact]
        public void Render_XLine_WithByLayerColor_CreatesCorrectColor()
        {
            var xline = CreateBasicXLine();
            xline.Color = new Color(256);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.Equal(256, result.ColorIndex);
            Assert.Equal(0xB3B3B3, result.Material.Color);
        }

        [Fact]
        public void Render_XLine_WithDifferentFirstPoint_CalculatesCorrectCenter()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(100, 200, 50);
            xline.Direction = new XYZ(1, 0, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.Center);
            Assert.Equal(100, result.Center.X);
            Assert.Equal(200, result.Center.Y);
            Assert.Equal(50, result.Center.Z);
        }

        [Fact]
        public void Render_XLine_WithDiagonalDirection_CalculatesCorrectSecondPoint()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(0.707, 0.707, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.SecondPoint);
            Assert.Equal(707, result.SecondPoint.X, 1);
            Assert.Equal(707, result.SecondPoint.Y, 1);
            Assert.Equal(0, result.SecondPoint.Z);
        }

        [Fact]
        public void Render_XLine_WithDiagonalDirection_CalculatesCorrectLength()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(0.707, 0.707, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.Equal(1000, result.Length, 1);
        }

        [Fact]
        public void Render_XLine_WithNegativeDirection_CalculatesCorrectAngle()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(-1, 0, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.Equal(Math.PI, result.Angle);
        }

        [Fact]
        public void Render_XLine_ReturnsCorrectLinePoints()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.LinePoints);
            Assert.Equal(2, result.LinePoints.Length);
            Assert.Equal(-1000, result.LinePoints[0].X);
            Assert.Equal(0, result.LinePoints[0].Y);
            Assert.Equal(0, result.LinePoints[0].Z);
            Assert.Equal(1000, result.LinePoints[1].X);
            Assert.Equal(0, result.LinePoints[1].Y);
            Assert.Equal(0, result.LinePoints[1].Z);
        }

        [Fact]
        public void Render_XLine_WithZComponent_CalculatesCorrectCenter()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 100);
            xline.Direction = new XYZ(1, 0, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.Center);
            Assert.Equal(0, result.Center.X);
            Assert.Equal(0, result.Center.Y);
            Assert.Equal(100, result.Center.Z);
        }

        [Fact]
        public void Render_XLine_CalculatesCorrectBoundsSize()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Size);
            Assert.Equal(2000, result.Bounds.Size.X);
            Assert.Equal(0, result.Bounds.Size.Y);
            Assert.Equal(0, result.Bounds.Size.Z);
        }

        [Fact]
        public void Render_XLine_CalculatesCorrectBoundsCenter()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(100, 200, 50);
            xline.Direction = new XYZ(1, 0, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Center);
            Assert.Equal(100, result.Bounds.Center.X);
            Assert.Equal(200, result.Bounds.Center.Y);
            Assert.Equal(50, result.Bounds.Center.Z);
        }

        [Fact]
        public void Render_XLine_VerticalDirection_CalculatesCorrectBounds()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(0, 1, 0);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(-1000, result.Bounds.Min.Y);
            Assert.Equal(0, result.Bounds.Min.Z);
            Assert.Equal(0, result.Bounds.Max.X);
            Assert.Equal(1000, result.Bounds.Max.Y);
            Assert.Equal(0, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_XLine_WithDifferentColorIndices_ReturnsCorrectVertexColors()
        {
            var xline = CreateBasicXLine();
            xline.Color = new Color(3);
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexColors);
            Assert.Equal(0.0, result.VertexColors[0]);
            Assert.Equal(1.0, result.VertexColors[1]);
            Assert.Equal(0.0, result.VertexColors[2]);
        }

        [Fact]
        public void Render_XLine_TransformMatrix_IsCorrectlyCalculated()
        {
            var xline = CreateBasicXLine();
            var result = XLineEntityRenderer.Render(xline);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Matrix);
            Assert.Equal(16, result.Transform.Matrix.Length);
            Assert.Equal(1, result.Transform.Matrix[0]);
            Assert.Equal(1, result.Transform.Matrix[5]);
            Assert.Equal(1, result.Transform.Matrix[10]);
            Assert.Equal(1, result.Transform.Matrix[15]);
        }
    }
}
