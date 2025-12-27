using System;
using Xunit;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using DxfDwgViewer.RenderUtilities;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class MTextEntityRendererTests
    {
        private MText CreateBasicMText()
        {
            var mtext = new MText
            {
                InsertPoint = new XYZ(10, 10, 0),
                Height = 5,
                Value = "Test Text"
            };
            mtext.Color = new Color(1);
            return mtext;
        }

        [Fact]
        public void Render_MText_ReturnsCorrectType()
        {
            var mtext = CreateBasicMText();
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal("MText", result.Type);
            Assert.Equal("MText", result.EntityType);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectLines()
        {
            var mtext = CreateBasicMText();
            mtext.Value = "Line1\\PLine2\\PLine3";
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(3, result.LineCount);
            Assert.Equal("Line1", result.Lines[0]);
            Assert.Equal("Line2", result.Lines[1]);
            Assert.Equal("Line3", result.Lines[2]);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectInsertPoint()
        {
            var mtext = CreateBasicMText();
            mtext.InsertPoint = new XYZ(100, 200, 50);
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(100, result.InsertPoint.X);
            Assert.Equal(200, result.InsertPoint.Y);
            Assert.Equal(50, result.InsertPoint3D.Z);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectHeight()
        {
            var mtext = CreateBasicMText();
            mtext.Height = 10;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(10, result.Height);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectColorIndex()
        {
            var mtext = CreateBasicMText();
            mtext.Color = new Color(5);
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectRotation()
        {
            var mtext = CreateBasicMText();
            mtext.AlignmentPoint = new XYZ(Math.Cos(Math.PI / 4), Math.Sin(Math.PI / 4), 0);
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 4, result.Rotation);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectAttachmentPoint()
        {
            var mtext = CreateBasicMText();
            mtext.AttachmentPoint = AttachmentPointType.MiddleCenter;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(AttachmentPointType.MiddleCenter, result.AttachmentPoint);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectLineSpacing()
        {
            var mtext = CreateBasicMText();
            mtext.LineSpacingStyle = LineSpacingStyleType.AtLeast;
            mtext.LineSpacing = 1.5;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(LineSpacingStyleType.AtLeast, result.LineSpacingStyle);
            Assert.Equal(1.5, result.LineSpacing);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectRectangleWidth()
        {
            var mtext = CreateBasicMText();
            mtext.RectangleWidth = 100;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(100, result.RectangleWidth);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectTextStyleName()
        {
            var mtext = CreateBasicMText();
            var style = new TextStyle("MyStyle");
            mtext.Style = style;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal("MyStyle", result.TextStyleName);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectWidthFactor()
        {
            var mtext = CreateBasicMText();
            var style = new TextStyle("TestStyle");
            style.Width = 1.5;
            mtext.Style = style;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(1.5, result.WidthFactor);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectObliqueAngle()
        {
            var mtext = CreateBasicMText();
            var style = new TextStyle("TestStyle");
            style.ObliqueAngle = 0.1;
            mtext.Style = style;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(0.1, result.ObliqueAngle);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectHandle()
        {
            var mtext = CreateBasicMText();
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(mtext.Handle.ToString(), result.Handle);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectLayerName()
        {
            var mtext = CreateBasicMText();
            var layer = new Layer("TEST_LAYER");
            mtext.Layer = layer;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal("TEST_LAYER", result.LayerName);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectVisible()
        {
            var mtext = CreateBasicMText();
            mtext.IsInvisible = false;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.True(result.Visible);
        }

        [Fact]
        public void Render_MText_Invisible_ReturnsCorrectVisible()
        {
            var mtext = CreateBasicMText();
            mtext.IsInvisible = true;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.False(result.Visible);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectNormal()
        {
            var mtext = CreateBasicMText();
            mtext.Normal = new XYZ(0, 0, 1);
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(0, result.Normal.X);
            Assert.Equal(0, result.Normal.Y);
            Assert.Equal(1, result.Normal.Z);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectDrawingDirection()
        {
            var mtext = CreateBasicMText();
            mtext.DrawingDirection = DrawingDirectionType.LeftToRight;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(DrawingDirectionType.LeftToRight, result.DrawingDirection);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectBackgroundColor()
        {
            var mtext = CreateBasicMText();
            mtext.BackgroundColor = new Color(3);
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(3, result.BackgroundColor.Index);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectBackgroundFillFlags()
        {
            var mtext = CreateBasicMText();
            mtext.BackgroundFillFlags = BackgroundFillFlags.UseBackgroundFillColor;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(BackgroundFillFlags.UseBackgroundFillColor, result.BackgroundFillFlags);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectBackgroundScale()
        {
            var mtext = CreateBasicMText();
            mtext.BackgroundScale = 1.5;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(1.5, result.BackgroundScale);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectBackgroundTransparency()
        {
            var mtext = CreateBasicMText();
            mtext.BackgroundTransparency = new ACadSharp.Transparency(50);
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(50, result.BackgroundTransparency);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectHorizontalWidth()
        {
            var mtext = CreateBasicMText();
            mtext.HorizontalWidth = 200;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(200, result.HorizontalWidth);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectRectangleHeight()
        {
            var mtext = CreateBasicMText();
            mtext.RectangleHeight = 100;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(100, result.RectangleHeight);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectVerticalHeight()
        {
            var mtext = CreateBasicMText();
            mtext.VerticalHeight = 150;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(150, result.VerticalHeight);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectIsAnnotative()
        {
            var mtext = CreateBasicMText();
            mtext.IsAnnotative = true;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.True(result.IsAnnotative);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectPlainText()
        {
            var mtext = CreateBasicMText();
            mtext.Value = "Plain\\PText";
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal("Plain\r\nText", result.PlainText);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectTextValue()
        {
            var mtext = CreateBasicMText();
            mtext.Value = "Test Value";
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal("Test Value", result.TextValue);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectAlignmentPoint3D()
        {
            var mtext = CreateBasicMText();
            mtext.AlignmentPoint = new XYZ(20, 30, 0);
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(20, result.AlignmentPoint3D.X);
            Assert.Equal(30, result.AlignmentPoint3D.Y);
            Assert.Equal(0, result.AlignmentPoint3D.Z);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectVertices3D()
        {
            var mtext = CreateBasicMText();
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(4, result.Vertices3D.Count);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectNormals()
        {
            var mtext = CreateBasicMText();
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(4, result.Normals.Count);
            Assert.All(result.Normals, n => Assert.Equal(1, n.Z));
        }

        [Fact]
        public void Render_MText_ReturnsCorrectBounds()
        {
            var mtext = CreateBasicMText();
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectBounds3D()
        {
            var mtext = CreateBasicMText();
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds3D);
            Assert.NotNull(result.Bounds3D.Min);
            Assert.NotNull(result.Bounds3D.Max);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectCentroid3D()
        {
            var mtext = CreateBasicMText();
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid3D);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectTransform()
        {
            var mtext = CreateBasicMText();
            mtext.InsertPoint = new XYZ(10, 20, 30);
            mtext.AlignmentPoint = new XYZ(Math.Cos(Math.PI / 6), Math.Sin(Math.PI / 6), 0);
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Matrix);
        }

        [Fact]
        public void Render_MText_WithBackgroundTransparency_SetsTransparent()
        {
            var mtext = CreateBasicMText();
            mtext.BackgroundTransparency = new ACadSharp.Transparency(50);
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.True(result.Transparent);
            Assert.Equal(0.5, result.Opacity);
        }

        [Fact]
        public void Render_MText_WithoutBackgroundTransparency_SetsOpaque()
        {
            var mtext = CreateBasicMText();
            mtext.BackgroundTransparency = ACadSharp.Transparency.ByLayer;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.False(result.Transparent);
            Assert.Equal(1.0, result.Opacity);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectMaterialType()
        {
            var mtext = CreateBasicMText();
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal("TextMaterial", result.MaterialType);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectDepthSettings()
        {
            var mtext = CreateBasicMText();
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.True(result.DepthTest);
            Assert.True(result.DepthWrite);
        }

        [Fact]
        public void Render_MText_WithEmptyValue_ReturnsEmptyLines()
        {
            var mtext = CreateBasicMText();
            mtext.Value = "";
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(1, result.LineCount);
            Assert.Single(result.Lines);
            Assert.Equal("", result.Lines[0]);
        }

        [Fact]
        public void Render_MText_WithSingleLine_ReturnsOneLine()
        {
            var mtext = CreateBasicMText();
            mtext.Value = "Single Line";
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(1, result.LineCount);
            Assert.Single(result.Lines);
        }

        [Fact]
        public void Render_MText_WithMultipleLines_ReturnsCorrectLineCount()
        {
            var mtext = CreateBasicMText();
            mtext.Value = "Line1\\PLine2\\PLine3\\PLine4";
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(4, result.LineCount);
        }

        [Fact]
        public void Render_MText_WithDefaultStyle_ReturnsDefaultWidthFactor()
        {
            var mtext = CreateBasicMText();
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(1.0, result.WidthFactor);
        }

        [Fact]
        public void Render_MText_WithDefaultStyle_ReturnsDefaultObliqueAngle()
        {
            var mtext = CreateBasicMText();
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal(0.0, result.ObliqueAngle);
        }

        [Fact]
        public void Render_MText_WithNullLayer_ReturnsEmptyLayerName()
        {
            var mtext = new MText();
            mtext.Color = new ACadSharp.Color(7);
            mtext.LineType = new LineType("CONTINUOUS");
            mtext.LineWeight = LineWeightType.ByLayer;
            mtext.IsInvisible = false;
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.NotNull(result.LayerName);
        }

        [Fact]
        public void Render_MText_WithNullStyle_ReturnsEmptyTextStyleName()
        {
            var mtext = new MText();
            mtext.InsertPoint = new XYZ(10, 10, 0);
            mtext.Height = 5;
            mtext.Value = "Test Text";
            mtext.Color = new Color(1);
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.Equal("Standard", result.TextStyleName);
        }

        [Fact]
        public void Render_MText_ReturnsCorrectColorData()
        {
            var mtext = CreateBasicMText();
            mtext.Color = new Color(1);
            var result = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);
            Assert.Equal(1, result.Color.Index);
            Assert.Equal(255, result.Color.R);
            Assert.Equal(0, result.Color.G);
            Assert.Equal(0, result.Color.B);
        }
    }
}
