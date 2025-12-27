using System;
using Xunit;
using ACadSharp.Entities;
using ACadSharp.Tables;
using ACadSharp;
using CSMath;
using DxfDwgViewer.RenderUtilities;

namespace DxfDwgViewer.Tests
{
    public class TextEntityRendererTests
    {
        private TextEntity CreateBasicText()
        {
            var textStyle = new TextStyle("TextStyle");
            var text = new TextEntity(textStyle);
            text.Value = "Test";
            text.InsertPoint = new XYZ(0, 0, 0);
            text.AlignmentPoint = new XYZ(0, 0, 0);
            text.Height = 1.0;
            text.Rotation = 0.0;
            text.ObliqueAngle = 0.0;
            text.WidthFactor = 1.0;
            text.Normal = new XYZ(0, 0, 1);
            text.Color = new Color(256);
            text.Thickness = 0.0;
            return text;
        }

        [Fact]
        public void Render_Text_ReturnsCorrectValue()
        {
            var text = CreateBasicText();
            text.Value = "Hello World";
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal("Hello World", result.Value);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectInsertPoint()
        {
            var text = CreateBasicText();
            text.InsertPoint = new XYZ(100, 200, 50);
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(100, result.InsertPoint.X);
            Assert.Equal(200, result.InsertPoint.Y);
            Assert.Equal(50, result.InsertPoint.Z);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectAlignmentPoint()
        {
            var text = CreateBasicText();
            text.AlignmentPoint = new XYZ(150, 250, 75);
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(150, result.AlignmentPoint.X);
            Assert.Equal(250, result.AlignmentPoint.Y);
            Assert.Equal(75, result.AlignmentPoint.Z);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectHeight()
        {
            var text = CreateBasicText();
            text.Height = 5.0;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(5.0, result.Height);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectRotation()
        {
            var text = CreateBasicText();
            text.Rotation = Math.PI / 4;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 4, result.Rotation);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectObliqueAngle()
        {
            var text = CreateBasicText();
            text.ObliqueAngle = Math.PI / 6;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 6, result.ObliqueAngle);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectWidthFactor()
        {
            var text = CreateBasicText();
            text.WidthFactor = 1.5;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(1.5, result.WidthFactor);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectHorizontalAlignment()
        {
            var text = CreateBasicText();
            text.HorizontalAlignment = TextHorizontalAlignment.Center;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(TextHorizontalAlignment.Center, result.HorizontalAlignment);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectVerticalAlignment()
        {
            var text = CreateBasicText();
            text.VerticalAlignment = TextVerticalAlignmentType.Middle;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(TextVerticalAlignmentType.Middle, result.VerticalAlignment);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectMirror()
        {
            var text = CreateBasicText();
            text.Mirror = TextMirrorFlag.MirrorX;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(TextMirrorFlag.MirrorX, result.Mirror);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectNormal()
        {
            var text = CreateBasicText();
            text.Normal = new XYZ(0.577, 0.577, 0.577);
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.InRange(result.Normal.X, 0.57, 0.58);
            Assert.InRange(result.Normal.Y, 0.57, 0.58);
            Assert.InRange(result.Normal.Z, 0.57, 0.58);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectColorIndex()
        {
            var text = CreateBasicText();
            text.Color = new Color(1);
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(1, result.ColorIndex);
        }

        [Fact]
        public void Render_Text_CreatesColorCorrectly()
        {
            var text = CreateBasicText();
            text.Color = new Color(1);
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);

            Assert.Equal(1, result.Color.Index);
            Assert.Equal("#FF0000", result.Color.Hex);
            Assert.Equal((byte)255, result.Color.R);
            Assert.Equal((byte)0, result.Color.G);
            Assert.Equal((byte)0, result.Color.B);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectLineTypeName()
        {
            var text = CreateBasicText();
            var lineType = new LineType("DASHED");
            text.LineType = lineType;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal("DASHED", result.LineTypeName);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectLineWeight()
        {
            var text = CreateBasicText();
            text.LineWeight = LineWeightType.W40;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(0.4, result.LineWeight);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectThickness()
        {
            var text = CreateBasicText();
            text.Thickness = 5.0;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(5.0, result.Thickness);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectStyleName()
        {
            var text = CreateBasicText();
            text.Style.Name = "MyStyle";
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal("MyStyle", result.StyleName);
        }

        [Fact]
        public void Render_Text_CalculatesCorrectWidth()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            text.Height = 10.0;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(24.0, result.Width);
        }

        [Fact]
        public void Render_Text_CalculatesCorrectArea()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            text.Height = 10.0;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(240.0, result.Area);
        }

        [Fact]
        public void Render_Text_CalculatesCorrectBounds()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            text.Height = 10.0;
            text.InsertPoint = new XYZ(0, 0, 0);
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);
            Assert.NotNull(result.Bounds.Center);
            Assert.NotNull(result.Bounds.Size);
        }

        [Fact]
        public void Render_Text_CalculatesCorrectCentroid()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            text.Height = 10.0;
            text.InsertPoint = new XYZ(0, 0, 0);
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);
        }

        [Fact]
        public void Render_Text_WithLeftAlignment_CalculatesCorrectBounds()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            text.Height = 10.0;
            text.InsertPoint = new XYZ(0, 0, 0);
            text.HorizontalAlignment = TextHorizontalAlignment.Left;
            text.VerticalAlignment = TextVerticalAlignmentType.Bottom;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(24.0, result.Bounds.Max.X);
            Assert.Equal(10.0, result.Bounds.Max.Y);
        }

        [Fact]
        public void Render_Text_WithCenterAlignment_CalculatesCorrectBounds()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            text.Height = 10.0;
            text.InsertPoint = new XYZ(0, 0, 0);
            text.HorizontalAlignment = TextHorizontalAlignment.Center;
            text.VerticalAlignment = TextVerticalAlignmentType.Middle;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(-12.0, result.Bounds.Min.X);
            Assert.Equal(-5.0, result.Bounds.Min.Y);
            Assert.Equal(12.0, result.Bounds.Max.X);
            Assert.Equal(5.0, result.Bounds.Max.Y);
        }

        [Fact]
        public void Render_Text_WithRightAlignment_CalculatesCorrectBounds()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            text.Height = 10.0;
            text.InsertPoint = new XYZ(0, 0, 0);
            text.HorizontalAlignment = TextHorizontalAlignment.Right;
            text.VerticalAlignment = TextVerticalAlignmentType.Top;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(-24.0, result.Bounds.Min.X);
            Assert.Equal(-10.0, result.Bounds.Min.Y);
            Assert.Equal(0, result.Bounds.Max.X);
            Assert.Equal(0, result.Bounds.Max.Y);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectTransform()
        {
            var text = CreateBasicText();
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Position);
            Assert.NotNull(result.Transform.Rotation);
            Assert.NotNull(result.Transform.Scale);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectGeometry()
        {
            var text = CreateBasicText();
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);
            Assert.Equal("TextGeometry", result.Geometry.Type);
            Assert.Equal(4, result.Geometry.VertexCount);
            Assert.True(result.Geometry.HasNormals);
            Assert.True(result.Geometry.HasColors);
            Assert.True(result.Geometry.HasUVs);
            Assert.True(result.Geometry.HasIndices);
            Assert.Equal("Triangles", result.Geometry.PrimitiveType);
            Assert.Equal(6, result.Geometry.IndexCount);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectMaterial()
        {
            var text = CreateBasicText();
            text.Color = new Color(1);
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);
            Assert.Equal("MeshBasicMaterial", result.Material.Type);
            Assert.Equal(0xFF0000, result.Material.Color);
            Assert.Equal(1.0, result.Material.Opacity);
            Assert.False(result.Material.Transparent);
            Assert.False(result.Material.Wireframe);
            Assert.True(result.Material.VertexColors);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectVertexPositions()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            text.Height = 10.0;
            text.InsertPoint = new XYZ(0, 0, 0);
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexPositions);
            Assert.Equal(12, result.VertexPositions.Length);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectVertexColors()
        {
            var text = CreateBasicText();
            text.Color = new Color(1);
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexColors);
            Assert.Equal(12, result.VertexColors.Length);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectIndices()
        {
            var text = CreateBasicText();
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.NotNull(result.Indices);
            Assert.Equal(6, result.Indices.Length);
            Assert.Equal(0, result.Indices[0]);
            Assert.Equal(1, result.Indices[1]);
            Assert.Equal(2, result.Indices[2]);
            Assert.Equal(0, result.Indices[3]);
            Assert.Equal(2, result.Indices[4]);
            Assert.Equal(3, result.Indices[5]);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectVertexCount()
        {
            var text = CreateBasicText();
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(4, result.VertexCount);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectFontFamily()
        {
            var text = CreateBasicText();
            text.Style.Name = "Arial";
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal("Arial", result.FontFamily);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectFontSize()
        {
            var text = CreateBasicText();
            text.Height = 15.0;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(15.0, result.FontSize);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectTextAlignment()
        {
            var text = CreateBasicText();
            text.HorizontalAlignment = TextHorizontalAlignment.Center;
            text.VerticalAlignment = TextVerticalAlignmentType.Middle;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal("Center-Middle", result.TextAlignment);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectOpacity()
        {
            var text = CreateBasicText();
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(1.0, result.Opacity);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectTransparent()
        {
            var text = CreateBasicText();
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.False(result.Transparent);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectTangent()
        {
            var text = CreateBasicText();
            text.Rotation = Math.PI / 4;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.NotNull(result.Tangent);
            Assert.InRange(result.Tangent.X, 0.70, 0.71);
            Assert.InRange(result.Tangent.Y, 0.70, 0.71);
            Assert.Equal(0, result.Tangent.Z);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectBinormal()
        {
            var text = CreateBasicText();
            text.Rotation = Math.PI / 4;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.NotNull(result.Binormal);
            Assert.Equal(3, result.Binormal.Length);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectUV()
        {
            var text = CreateBasicText();
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.NotNull(result.UV);
            Assert.Equal(8, result.UV.Length);
            Assert.Equal(0, result.UV[0]);
            Assert.Equal(0, result.UV[1]);
            Assert.Equal(1, result.UV[2]);
            Assert.Equal(0, result.UV[3]);
            Assert.Equal(1, result.UV[4]);
            Assert.Equal(1, result.UV[5]);
            Assert.Equal(0, result.UV[6]);
            Assert.Equal(1, result.UV[7]);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectIsMirrored()
        {
            var text = CreateBasicText();
            text.Mirror = TextMirrorFlag.MirrorX;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.True(result.IsMirrored);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectIsUpsideDown()
        {
            var text = CreateBasicText();
            text.Rotation = Math.PI;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.True(result.IsUpsideDown);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectTextLength()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            text.Height = 10.0;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(24.0, result.TextLength);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectAscent()
        {
            var text = CreateBasicText();
            text.Height = 10.0;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(8.0, result.Ascent);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectDescent()
        {
            var text = CreateBasicText();
            text.Height = 10.0;
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(2.0, result.Descent);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectCharacterCount()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal(4, result.CharacterCount);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectFontStyle()
        {
            var text = CreateBasicText();
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.Equal("Regular", result.FontStyle);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectIsBold()
        {
            var text = CreateBasicText();
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.False(result.IsBold);
        }

        [Fact]
        public void Render_Text_ReturnsCorrectIsItalic()
        {
            var text = CreateBasicText();
            var result = TextEntityRenderer.Render(text);

            Assert.NotNull(result);
            Assert.False(result.IsItalic);
        }

        [Fact]
        public void Render_Text_WithAllPropertiesSet_PreservesAllProperties()
        {
            var textStyle = new TextStyle("CompleteTextStyle");
            var text = new TextEntity(textStyle);
            text.Value = "Complete Text";
            text.InsertPoint = new XYZ(100, 200, 300);
            text.AlignmentPoint = new XYZ(150, 250, 350);
            text.Height = 5.0;
            text.Rotation = Math.PI / 3;
            text.ObliqueAngle = Math.PI / 8;
            text.WidthFactor = 1.5;
            text.HorizontalAlignment = TextHorizontalAlignment.Center;
            text.VerticalAlignment = TextVerticalAlignmentType.Middle;
            text.Mirror = TextMirrorFlag.MirrorX;
            text.Normal = new XYZ(0.577, 0.577, 0.577);
            text.Color = new Color(3);
            text.LineType = new LineType("DASHED");
            text.LineWeight = LineWeightType.W40;
            text.Thickness = 10.0;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal("Complete Text", textData.Value);
            Assert.Equal(100, textData.InsertPoint.X);
            Assert.Equal(200, textData.InsertPoint.Y);
            Assert.Equal(300, textData.InsertPoint.Z);
            Assert.Equal(150, textData.AlignmentPoint.X);
            Assert.Equal(250, textData.AlignmentPoint.Y);
            Assert.Equal(350, textData.AlignmentPoint.Z);
            Assert.Equal(5.0, textData.Height);
            Assert.Equal(Math.PI / 3, textData.Rotation);
            Assert.Equal(Math.PI / 8, textData.ObliqueAngle);
            Assert.Equal(1.5, textData.WidthFactor);
            Assert.Equal(TextHorizontalAlignment.Center, textData.HorizontalAlignment);
            Assert.Equal(TextVerticalAlignmentType.Middle, textData.VerticalAlignment);
            Assert.Equal(TextMirrorFlag.MirrorX, textData.Mirror);
            Assert.InRange(textData.Normal.X, 0.57, 0.58);
            Assert.InRange(textData.Normal.Y, 0.57, 0.58);
            Assert.InRange(textData.Normal.Z, 0.57, 0.58);
            Assert.Equal(3, textData.ColorIndex);
            Assert.Equal("DASHED", textData.LineTypeName);
            Assert.Equal(0.4, textData.LineWeight);
            Assert.Equal(10.0, textData.Thickness);
            Assert.Equal("CompleteTextStyle", textData.StyleName);
            Assert.True(textData.IsMirrored);
        }
    }
}
