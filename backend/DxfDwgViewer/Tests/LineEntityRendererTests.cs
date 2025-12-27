using Xunit;
using ACadSharp.Entities;
using ACadSharp.Tables;
using DxfDwgViewer.RenderUtilities;
using CSMath;
using System;

namespace DxfDwgViewer.Tests
{
    public class LineEntityRendererTests
    {
        [Fact]
        public void Render_Line_ReturnsCorrectType()
        {
            var line = CreateBasicLine();

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal("Line", result.Type);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectEntityType()
        {
            var line = CreateBasicLine();

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal("Line", result.EntityType);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectHandle()
        {
            var line = CreateBasicLine();

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.NotNull(result.Handle);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectLayerName()
        {
            var line = CreateBasicLine();
            line.Layer = new Layer("MY_LAYER");

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal("MY_LAYER", result.LayerName);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectVisibility()
        {
            var line = CreateBasicLine();
            line.IsInvisible = false;

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.True(result.Visible);
        }

        [Fact]
        public void Render_Line_WithInvisible_ReturnsCorrectVisibility()
        {
            var line = CreateBasicLine();
            line.IsInvisible = true;

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.False(result.Visible);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectStartPoint()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(5, 10, 0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(5, result.StartPointX);
            Assert.Equal(10, result.StartPointY);
            Assert.Equal(0, result.StartPoint3D.Z);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectStartPoint3D()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(5, 10, 15);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.NotNull(result.StartPoint3D);
            Assert.Equal(5, result.StartPoint3D.X);
            Assert.Equal(10, result.StartPoint3D.Y);
            Assert.Equal(15, result.StartPoint3D.Z);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectEndPoint()
        {
            var line = CreateBasicLine();
            line.EndPoint = new XYZ(15, 20, 0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(15, result.EndPointX);
            Assert.Equal(20, result.EndPointY);
            Assert.Equal(0, result.EndPoint3D.Z);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectEndPoint3D()
        {
            var line = CreateBasicLine();
            line.EndPoint = new XYZ(15, 20, 25);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.NotNull(result.EndPoint3D);
            Assert.Equal(15, result.EndPoint3D.X);
            Assert.Equal(20, result.EndPoint3D.Y);
            Assert.Equal(25, result.EndPoint3D.Z);
        }

        [Fact]
        public void Render_Line_CalculatesCorrectLength()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(0, 0, 0);
            line.EndPoint = new XYZ(10, 0, 0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(10, result.Length);
        }

        [Fact]
        public void Render_Line_CalculatesCorrectLength_Diagonal()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(0, 0, 0);
            line.EndPoint = new XYZ(3, 4, 0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(5, result.Length);
        }

        [Fact]
        public void Render_Line_CalculatesCorrectAngle()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(0, 0, 0);
            line.EndPoint = new XYZ(10, 0, 0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(0, result.Angle);
        }

        [Fact]
        public void Render_Line_CalculatesCorrectAngle_45Degrees()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(0, 0, 0);
            line.EndPoint = new XYZ(10, 10, 0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 4, result.Angle);
        }

        [Fact]
        public void Render_Line_CalculatesCorrectAngle_90Degrees()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(0, 0, 0);
            line.EndPoint = new XYZ(0, 10, 0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 2, result.Angle);
        }

        [Fact]
        public void Render_Line_CalculatesCorrectMidPoint()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(0, 0, 0);
            line.EndPoint = new XYZ(10, 20, 0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(5, result.MidPointX);
            Assert.Equal(10, result.MidPointY);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectThickness()
        {
            var line = CreateBasicLine();
            line.Thickness = 2.5;

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(2.5, result.Thickness);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectColorIndex()
        {
            var line = CreateBasicLine();
            line.Color = new ACadSharp.Color(5);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectColorData()
        {
            var line = CreateBasicLine();
            line.Color = new ACadSharp.Color(5);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);
            Assert.Equal(5, result.Color.Index);
            Assert.Equal("#0000FF", result.Color.Hex);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectLineTypeName()
        {
            var line = CreateBasicLine();
            var lineType = new LineType("CONTINUOUS");
            line.LineType = lineType;

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal("CONTINUOUS", result.LineTypeName);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectLineWeight()
        {
            var line = CreateBasicLine();
            line.LineWeight = ACadSharp.LineWeightType.W50;

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(0.5, result.LineWeight);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectLineWeight_W30()
        {
            var line = CreateBasicLine();
            line.LineWeight = ACadSharp.LineWeightType.W30;

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(0.3, result.LineWeight);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectLineTypeScale()
        {
            var line = CreateBasicLine();
            line.LineTypeScale = 2.5;

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(2.5, result.LineTypeScale);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectNormal()
        {
            var line = CreateBasicLine();
            line.Normal = new XYZ(0, 0, 1);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.NotNull(result.Normal);
            Assert.Equal(0, result.Normal.X);
            Assert.Equal(0, result.Normal.Y);
            Assert.Equal(1, result.Normal.Z);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectTransform3D()
        {
            var line = CreateBasicLine();

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Matrix);
            Assert.Equal(16, result.Transform.Matrix.Length);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectBounds()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(0, 0, 0);
            line.EndPoint = new XYZ(10, 20, 5);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);
            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(0, result.Bounds.Min.Z);
            Assert.Equal(10, result.Bounds.Max.X);
            Assert.Equal(20, result.Bounds.Max.Y);
            Assert.Equal(5, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectMaterialProperties()
        {
            var line = CreateBasicLine();

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal("LineBasicMaterial", result.MaterialType);
            Assert.Equal(1.0, result.Opacity);
            Assert.False(result.Transparent);
            Assert.True(result.DepthTest);
            Assert.True(result.DepthWrite);
        }

        [Fact]
        public void Render_Line_ReturnsCorrectCoordinateSystem()
        {
            var line = CreateBasicLine();

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal("World", result.CoordinateSystem);
        }

        [Fact]
        public void Render_Line_WithZeroLength_CalculatesCorrectLength()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(5, 5, 0);
            line.EndPoint = new XYZ(5, 5, 0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(0, result.Length);
        }

        [Fact]
        public void Render_Line_WithNegativeCoordinates_CalculatesCorrectLength()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(-10, -10, 0);
            line.EndPoint = new XYZ(10, 10, 0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(Math.Sqrt(800), result.Length);
        }

        [Fact]
        public void Render_Line_With3DCoordinates_CalculatesCorrectLength()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(0, 0, 0);
            line.EndPoint = new XYZ(3, 4, 12);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(5, result.Length);
        }

        [Fact]
        public void Render_Line_WithNegativeAngle_CalculatesCorrectAngle()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(10, 0, 0);
            line.EndPoint = new XYZ(0, 0, 0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(Math.PI, result.Angle);
        }

        [Fact]
        public void Render_Line_WithCustomNormal_CreatesCorrectTransform()
        {
            var line = CreateBasicLine();
            line.Normal = new XYZ(0, 1, 0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.NotNull(result.Normal);
            Assert.Equal(0, result.Normal.X);
            Assert.Equal(1, result.Normal.Y);
            Assert.Equal(0, result.Normal.Z);
        }

        [Fact]
        public void Render_Line_WithDifferentColorIndex_ReturnsCorrectColor()
        {
            var line = CreateBasicLine();
            line.Color = new ACadSharp.Color(1);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);
            Assert.Equal(1, result.Color.Index);
            Assert.Equal("#FF0000", result.Color.Hex);
        }

        [Fact]
        public void Render_Line_WithLayerByBlockColor_ReturnsCorrectColor()
        {
            var line = CreateBasicLine();
            line.Color = new ACadSharp.Color(0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);
            Assert.Equal(0, result.Color.Index);
            Assert.Equal("#FFFFFF", result.Color.Hex);
        }

        [Fact]
        public void Render_Line_WithDifferentLineWeight_ReturnsCorrectWeight()
        {
            var line = CreateBasicLine();
            line.LineWeight = ACadSharp.LineWeightType.W25;

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(0.25, result.LineWeight);
        }

        [Fact]
        public void Render_Line_WithDefaultLineWeight_ReturnsDefaultWeight()
        {
            var line = CreateBasicLine();
            line.LineWeight = ACadSharp.LineWeightType.ByLayer;

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(0.03, result.LineWeight);
        }

        [Fact]
        public void Render_Line_WithCustomLineTypeScale_ReturnsCorrectScale()
        {
            var line = CreateBasicLine();
            line.LineTypeScale = 5.0;

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(5.0, result.LineTypeScale);
        }

        [Fact]
        public void Render_Line_WithNegativeCoordinates_CalculatesCorrectBounds()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(-10, -20, -5);
            line.EndPoint = new XYZ(10, 20, 5);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.Equal(-10, result.Bounds.Min.X);
            Assert.Equal(-20, result.Bounds.Min.Y);
            Assert.Equal(-5, result.Bounds.Min.Z);
            Assert.Equal(10, result.Bounds.Max.X);
            Assert.Equal(20, result.Bounds.Max.Y);
            Assert.Equal(5, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_Line_WithSinglePointBounds_CalculatesCorrectBounds()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(5, 5, 5);
            line.EndPoint = new XYZ(5, 5, 5);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.Equal(5, result.Bounds.Min.X);
            Assert.Equal(5, result.Bounds.Min.Y);
            Assert.Equal(5, result.Bounds.Min.Z);
            Assert.Equal(5, result.Bounds.Max.X);
            Assert.Equal(5, result.Bounds.Max.Y);
            Assert.Equal(5, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_Line_WithLargeCoordinates_CalculatesCorrectLength()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(1000, 2000, 0);
            line.EndPoint = new XYZ(2000, 4000, 0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.Equal(1000 * Math.Sqrt(5), result.Length);
        }

        [Fact]
        public void Render_Line_WithVerySmallCoordinates_CalculatesCorrectLength()
        {
            var line = CreateBasicLine();
            line.StartPoint = new XYZ(0.001, 0.001, 0);
            line.EndPoint = new XYZ(0.002, 0.002, 0);

            var result = LineEntityRenderer.Render(line);

            Assert.NotNull(result);
            Assert.InRange(result.Length, 0.0014, 0.0015);
        }

        private Line CreateBasicLine()
        {
            return new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 0, 0),
                Thickness = 0,
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(7),
                LineType = new LineType("CONTINUOUS"),
                LineWeight = ACadSharp.LineWeightType.W25,
                LineTypeScale = 1.0
            };
        }
    }
}
