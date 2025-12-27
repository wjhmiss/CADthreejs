using Xunit;
using ACadSharp.Entities;
using ACadSharp.IO;
using ACadSharp.Tables;
using ACadSharp.Objects;
using ACadSharp;
using ACadSharp.Extensions;
using DxfDwgViewer.RenderUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class TextEntityRendererIntegrationTests
    {
        private TextEntity CreateBasicText()
        {
            var text = new TextEntity();
            text.Value = "Hello World";
            text.InsertPoint = new XYZ(0, 0, 0);
            text.Height = 10.0;
            text.Rotation = 0.0;
            text.ObliqueAngle = 0.0;
            text.WidthFactor = 1.0;
            text.HorizontalAlignment = TextHorizontalAlignment.Left;
            text.VerticalAlignment = TextVerticalAlignmentType.Baseline;
            text.Mirror = TextMirrorFlag.None;
            text.Normal = new XYZ(0, 0, 1);
            text.Color = new Color(1);
            return text;
        }

        private TextEntity CreateCenteredText()
        {
            var text = new TextEntity();
            text.Value = "Centered Text";
            text.InsertPoint = new XYZ(0, 0, 0);
            text.Height = 10.0;
            text.Rotation = 0.0;
            text.ObliqueAngle = 0.0;
            text.WidthFactor = 1.0;
            text.HorizontalAlignment = TextHorizontalAlignment.Center;
            text.VerticalAlignment = TextVerticalAlignmentType.Middle;
            text.Mirror = TextMirrorFlag.None;
            text.Normal = new XYZ(0, 0, 1);
            text.Color = new Color(2);
            return text;
        }

        private TextEntity CreateRightAlignedText()
        {
            var text = new TextEntity();
            text.Value = "Right Aligned";
            text.InsertPoint = new XYZ(0, 0, 0);
            text.Height = 10.0;
            text.Rotation = 0.0;
            text.ObliqueAngle = 0.0;
            text.WidthFactor = 1.0;
            text.HorizontalAlignment = TextHorizontalAlignment.Right;
            text.VerticalAlignment = TextVerticalAlignmentType.Baseline;
            text.Mirror = TextMirrorFlag.None;
            text.Normal = new XYZ(0, 0, 1);
            text.Color = new Color(3);
            return text;
        }

        private TextEntity CreateRotatedText()
        {
            var text = new TextEntity();
            text.Value = "Rotated Text";
            text.InsertPoint = new XYZ(0, 0, 0);
            text.Height = 10.0;
            text.Rotation = Math.PI / 4;
            text.ObliqueAngle = 0.0;
            text.WidthFactor = 1.0;
            text.HorizontalAlignment = TextHorizontalAlignment.Left;
            text.VerticalAlignment = TextVerticalAlignmentType.Baseline;
            text.Mirror = TextMirrorFlag.None;
            text.Normal = new XYZ(0, 0, 1);
            text.Color = new Color(4);
            return text;
        }

        private TextEntity CreateObliqueText()
        {
            var text = new TextEntity();
            text.Value = "Oblique Text";
            text.InsertPoint = new XYZ(0, 0, 0);
            text.Height = 10.0;
            text.Rotation = 0.0;
            text.ObliqueAngle = Math.PI / 12;
            text.WidthFactor = 1.0;
            text.HorizontalAlignment = TextHorizontalAlignment.Left;
            text.VerticalAlignment = TextVerticalAlignmentType.Baseline;
            text.Mirror = TextMirrorFlag.None;
            text.Normal = new XYZ(0, 0, 1);
            text.Color = new Color(5);
            return text;
        }

        private TextEntity CreateMirroredText()
        {
            var text = new TextEntity();
            text.Value = "Mirrored Text";
            text.InsertPoint = new XYZ(0, 0, 0);
            text.Height = 10.0;
            text.Rotation = 0.0;
            text.ObliqueAngle = 0.0;
            text.WidthFactor = 1.0;
            text.HorizontalAlignment = TextHorizontalAlignment.Left;
            text.VerticalAlignment = TextVerticalAlignmentType.Baseline;
            text.Mirror = TextMirrorFlag.X;
            text.Normal = new XYZ(0, 0, 1);
            text.Color = new Color(6);
            return text;
        }

        private TextEntity CreateTextWithWidthFactor()
        {
            var text = new TextEntity();
            text.Value = "Wide Text";
            text.InsertPoint = new XYZ(0, 0, 0);
            text.Height = 10.0;
            text.Rotation = 0.0;
            text.ObliqueAngle = 0.0;
            text.WidthFactor = 2.0;
            text.HorizontalAlignment = TextHorizontalAlignment.Left;
            text.VerticalAlignment = TextVerticalAlignmentType.Baseline;
            text.Mirror = TextMirrorFlag.None;
            text.Normal = new XYZ(0, 0, 1);
            text.Color = new Color(7);
            return text;
        }

        private TextEntity CreateTopAlignedText()
        {
            var text = new TextEntity();
            text.Value = "Top Aligned";
            text.InsertPoint = new XYZ(0, 0, 0);
            text.Height = 10.0;
            text.Rotation = 0.0;
            text.ObliqueAngle = 0.0;
            text.WidthFactor = 1.0;
            text.HorizontalAlignment = TextHorizontalAlignment.Left;
            text.VerticalAlignment = TextVerticalAlignmentType.Top;
            text.Mirror = TextMirrorFlag.None;
            text.Normal = new XYZ(0, 0, 1);
            text.Color = new Color(1);
            return text;
        }

        private TextEntity CreateBottomAlignedText()
        {
            var text = new TextEntity();
            text.Value = "Bottom Aligned";
            text.InsertPoint = new XYZ(0, 0, 0);
            text.Height = 10.0;
            text.Rotation = 0.0;
            text.ObliqueAngle = 0.0;
            text.WidthFactor = 1.0;
            text.HorizontalAlignment = TextHorizontalAlignment.Left;
            text.VerticalAlignment = TextVerticalAlignmentType.Bottom;
            text.Mirror = TextMirrorFlag.None;
            text.Normal = new XYZ(0, 0, 1);
            text.Color = new Color(2);
            return text;
        }

        [Fact]
        public void LoadAndRenderTextFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var textEntities = new List<TextEntity>();
            foreach (var entity in doc.Entities)
            {
                if (entity is TextEntity text)
                {
                    textEntities.Add(text);
                }
            }

            if (textEntities.Count == 0)
            {
                return;
            }

            foreach (var text in textEntities)
            {
                var textData = TextEntityRenderer.Render(text);
                Assert.NotNull(textData);
                Assert.Equal("Text", textData.Type);
                Assert.NotNull(textData.Value);
                Assert.NotNull(textData.Bounds);
            }
        }

        [Fact]
        public void SerializeTextDataToJson_Success()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);
            var json = JsonConvert.SerializeObject(textData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Text", json);
            Assert.Contains("Value", json);
            Assert.Contains("Bounds", json);
        }

        [Fact]
        public void DeserializeTextDataFromJson_Success()
        {
            var text = CreateBasicText();

            var originalTextData = TextEntityRenderer.Render(text);
            var json = JsonConvert.SerializeObject(originalTextData);
            var deserializedTextData = JsonConvert.DeserializeObject<TextEntityRenderer.TextData>(json);

            Assert.NotNull(deserializedTextData);
            Assert.Equal(originalTextData.Value, deserializedTextData.Value);
            Assert.Equal(originalTextData.Height, deserializedTextData.Height);
            Assert.Equal(originalTextData.Rotation, deserializedTextData.Rotation);
            Assert.Equal(originalTextData.ColorIndex, deserializedTextData.ColorIndex);
            Assert.Equal(originalTextData.HorizontalAlignment, deserializedTextData.HorizontalAlignment);
            Assert.Equal(originalTextData.VerticalAlignment, deserializedTextData.VerticalAlignment);
            Assert.Equal(originalTextData.Visible, deserializedTextData.Visible);
            Assert.Equal(originalTextData.Opacity, deserializedTextData.Opacity);
            Assert.Equal(originalTextData.Transparent, deserializedTextData.Transparent);
            Assert.Equal(originalTextData.DepthTest, deserializedTextData.DepthTest);
        }

        [Fact]
        public void RenderMultipleTexts_CollectAllTextData()
        {
            var texts = new List<TextEntity>
            {
                CreateBasicText(),
                CreateCenteredText()
            };
            texts[0].InsertPoint = new XYZ(10, 10, 0);
            texts[0].Value = "First Text";
            texts[1].InsertPoint = new XYZ(20, 20, 0);
            texts[1].Value = "Second Text";

            var textDataList = new List<TextEntityRenderer.TextData>();
            foreach (var text in texts)
            {
                var textData = TextEntityRenderer.Render(text);
                textDataList.Add(textData);
            }

            Assert.Equal(2, textDataList.Count);
            Assert.Equal("First Text", textDataList[0].Value);
            Assert.Equal("Second Text", textDataList[1].Value);
        }

        [Fact]
        public void RenderBasicText_PreservesAllProperties()
        {
            var text = CreateBasicText();
            text.Value = "Hello World";
            text.InsertPoint = new XYZ(100, 200, 300);
            text.Height = 15.0;
            text.Rotation = Math.PI / 6;
            text.ObliqueAngle = Math.PI / 12;
            text.WidthFactor = 1.5;
            text.Color = new Color(5);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal("Hello World", textData.Value);
            Assert.Equal(100, textData.InsertPoint.X);
            Assert.Equal(200, textData.InsertPoint.Y);
            Assert.Equal(300, textData.InsertPoint.Z);
            Assert.Equal(15.0, textData.Height);
            Assert.Equal(Math.PI / 6, textData.Rotation);
            Assert.Equal(Math.PI / 12, textData.ObliqueAngle);
            Assert.Equal(1.5, textData.WidthFactor);
            Assert.Equal(5, textData.ColorIndex);
            Assert.Equal(TextHorizontalAlignment.Left, textData.HorizontalAlignment);
            Assert.Equal(TextVerticalAlignmentType.Baseline, textData.VerticalAlignment);
            Assert.Equal(TextMirrorFlag.None, textData.Mirror);
        }

        [Fact]
        public void RenderCenteredText_CalculatesCorrectBounds()
        {
            var text = CreateCenteredText();
            text.Value = "Test";
            text.Height = 10.0;
            text.InsertPoint = new XYZ(0, 0, 0);
            text.HorizontalAlignment = TextHorizontalAlignment.Center;
            text.VerticalAlignment = TextVerticalAlignmentType.Middle;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(-12.0, textData.Bounds.Min.X);
            Assert.Equal(-5.0, textData.Bounds.Min.Y);
            Assert.Equal(12.0, textData.Bounds.Max.X);
            Assert.Equal(5.0, textData.Bounds.Max.Y);
        }

        [Fact]
        public void RenderRightAlignedText_CalculatesCorrectBounds()
        {
            var text = CreateRightAlignedText();
            text.Value = "Test";
            text.Height = 10.0;
            text.InsertPoint = new XYZ(0, 0, 0);
            text.HorizontalAlignment = TextHorizontalAlignment.Right;
            text.VerticalAlignment = TextVerticalAlignmentType.Baseline;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(-24.0, textData.Bounds.Min.X);
            Assert.Equal(0, textData.Bounds.Min.Y);
            Assert.Equal(0, textData.Bounds.Max.X);
            Assert.Equal(10.0, textData.Bounds.Max.Y);
        }

        [Fact]
        public void RenderRotatedText_CalculatesCorrectTransform()
        {
            var text = CreateRotatedText();
            text.Rotation = Math.PI / 4;
            text.InsertPoint = new XYZ(10, 20, 30);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Transform);
            Assert.InRange(Math.Abs(textData.Transform.Rotation.Z - Math.PI / 4), 0, 0.0001);
        }

        [Fact]
        public void RenderObliqueText_PreservesObliqueAngle()
        {
            var text = CreateObliqueText();
            text.ObliqueAngle = Math.PI / 12;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(Math.PI / 12, textData.ObliqueAngle);
        }

        [Fact]
        public void RenderMirroredText_SetsMirroredFlag()
        {
            var text = CreateMirroredText();
            text.Mirror = TextMirrorFlag.X;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.True(textData.IsMirrored);
            Assert.Equal(TextMirrorFlag.X, textData.Mirror);
        }

        [Fact]
        public void RenderTextWithWidthFactor_PreservesWidthFactor()
        {
            var text = CreateTextWithWidthFactor();
            text.WidthFactor = 2.0;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(2.0, textData.WidthFactor);
        }

        [Fact]
        public void RenderTopAlignedText_CalculatesCorrectBounds()
        {
            var text = CreateTopAlignedText();
            text.Value = "Test";
            text.Height = 10.0;
            text.InsertPoint = new XYZ(0, 0, 0);
            text.VerticalAlignment = TextVerticalAlignmentType.Top;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(0, textData.Bounds.Min.X);
            Assert.Equal(-10.0, textData.Bounds.Min.Y);
            Assert.Equal(24.0, textData.Bounds.Max.X);
            Assert.Equal(0, textData.Bounds.Max.Y);
        }

        [Fact]
        public void RenderBottomAlignedText_CalculatesCorrectBounds()
        {
            var text = CreateBottomAlignedText();
            text.Value = "Test";
            text.Height = 10.0;
            text.InsertPoint = new XYZ(0, 0, 0);
            text.VerticalAlignment = TextVerticalAlignmentType.Bottom;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(0, textData.Bounds.Min.X);
            Assert.Equal(0, textData.Bounds.Min.Y);
            Assert.Equal(24.0, textData.Bounds.Max.X);
            Assert.Equal(10.0, textData.Bounds.Max.Y);
        }

        [Fact]
        public void RenderTextWith3DCoordinates_CalculatesCorrect3DProperties()
        {
            var text = CreateBasicText();
            text.InsertPoint = new XYZ(10, 20, 30);
            text.Height = 10.0;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Centroid);
            Assert.NotNull(textData.Bounds);
            Assert.Equal(10, textData.InsertPoint.X);
            Assert.Equal(20, textData.InsertPoint.Y);
            Assert.Equal(30, textData.InsertPoint.Z);
        }

        [Fact]
        public void RenderTextWithLayer_PreservesLayerInformation()
        {
            var text = CreateBasicText();
            text.Layer = new Layer("TestLayer");

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal("TestLayer", textData.LayerName);
        }

        [Fact]
        public void RenderTextWithColor_PreservesColorInformation()
        {
            var text = CreateBasicText();
            text.Color = new Color(5);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(5, textData.ColorIndex);
            Assert.Equal(5, textData.Color.Index);
        }

        [Fact]
        public void RenderTextWithLineType_PreservesLineTypeInformation()
        {
            var text = CreateBasicText();
            var lineType = new LineType("Continuous");
            text.LineType = lineType;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal("Continuous", textData.LineTypeName);
        }

        [Fact]
        public void RenderTextWithLineWeight_PreservesLineWeightInformation()
        {
            var text = CreateBasicText();
            text.LineWeight = ACadSharp.LineWeightType.W30;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(0.3, textData.LineWeight);
        }

        [Fact]
        public void RenderTextWithInvisibleFlag_SetsVisibleProperty()
        {
            var text = CreateBasicText();
            text.IsInvisible = true;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.False(textData.Visible);
        }

        [Fact]
        public void RenderTextWithHandle_PreservesHandleInformation()
        {
            var text = CreateBasicText();
            
            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(text.Handle.ToString(), textData.Handle);
        }

        [Fact]
        public void RenderTextWithZeroInsertPoint_HandlesCorrectly()
        {
            var text = CreateBasicText();
            text.InsertPoint = new XYZ(0, 0, 0);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(0, textData.InsertPoint.X);
            Assert.Equal(0, textData.InsertPoint.Y);
            Assert.Equal(0, textData.InsertPoint.Z);
        }

        [Fact]
        public void RenderTextWithVeryLargeCoordinates_HandlesCorrectly()
        {
            var text = CreateBasicText();
            text.InsertPoint = new XYZ(1000000, 2000000, 3000000);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(1000000, textData.InsertPoint.X);
            Assert.Equal(2000000, textData.InsertPoint.Y);
            Assert.Equal(3000000, textData.InsertPoint.Z);
        }

        [Fact]
        public void RenderTextWithVerySmallCoordinates_HandlesCorrectly()
        {
            var text = CreateBasicText();
            text.InsertPoint = new XYZ(0.0001, 0.0002, 0.0003);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(0.0001, textData.InsertPoint.X);
            Assert.Equal(0.0002, textData.InsertPoint.Y);
            Assert.Equal(0.0003, textData.InsertPoint.Z);
        }

        [Fact]
        public void RenderTextWithNegativeCoordinates_HandlesCorrectly()
        {
            var text = CreateBasicText();
            text.InsertPoint = new XYZ(-10, -20, -30);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(-10, textData.InsertPoint.X);
            Assert.Equal(-20, textData.InsertPoint.Y);
            Assert.Equal(-30, textData.InsertPoint.Z);
        }

        [Fact]
        public void RenderTextWithDifferentNormals_PreservesNormals()
        {
            var normals = new[]
            {
                new XYZ(0, 0, 1),
                new XYZ(1, 0, 0),
                new XYZ(0, 1, 0),
                new XYZ(0.5, 0.5, 0.7071)
            };

            foreach (var normal in normals)
            {
                var text = CreateBasicText();
                text.Normal = normal;

                var textData = TextEntityRenderer.Render(text);

                Assert.NotNull(textData);
                Assert.InRange(textData.Normal.X, normal.X - 0.0001, normal.X + 0.0001);
                Assert.InRange(textData.Normal.Y, normal.Y - 0.0001, normal.Y + 0.0001);
                Assert.InRange(textData.Normal.Z, normal.Z - 0.0001, normal.Z + 0.0001);
            }
        }

        [Fact]
        public void RenderTextWithDifferentHeights_PreservesHeights()
        {
            var heights = new[] { 1.0, 5.0, 10.0, 20.0, 50.0 };

            foreach (var height in heights)
            {
                var text = CreateBasicText();
                text.Height = height;

                var textData = TextEntityRenderer.Render(text);

                Assert.NotNull(textData);
                Assert.Equal(height, textData.Height);
            }
        }

        [Fact]
        public void RenderTextWithDifferentRotations_PreservesRotations()
        {
            var rotations = new[] { 0.0, Math.PI / 4, Math.PI / 2, Math.PI, 2 * Math.PI };

            foreach (var rotation in rotations)
            {
                var text = CreateBasicText();
                text.Rotation = rotation;

                var textData = TextEntityRenderer.Render(text);

                Assert.NotNull(textData);
                Assert.Equal(rotation, textData.Rotation);
            }
        }

        [Fact]
        public void RenderTextWithDifferentColors_PreservesColors()
        {
            var colors = new[] { 1, 2, 3, 5, 7 };

            foreach (var colorIndex in colors)
            {
                var text = CreateBasicText();
                text.Color = new Color((short)colorIndex);

                var textData = TextEntityRenderer.Render(text);

                Assert.NotNull(textData);
                Assert.Equal(colorIndex, textData.ColorIndex);
                Assert.Equal(colorIndex, textData.Color.Index);
            }
        }

        [Fact]
        public void RenderTextWithDifferentLineWeights_PreservesLineWeights()
        {
            var lineWeights = new[]
            {
                ACadSharp.LineWeightType.W0,
                ACadSharp.LineWeightType.W30,
                ACadSharp.LineWeightType.W50
            };

            foreach (var lineWeight in lineWeights)
            {
                var text = CreateBasicText();
                text.LineWeight = lineWeight;

                var textData = TextEntityRenderer.Render(text);

                Assert.NotNull(textData);
                Assert.Equal(lineWeight.GetLineWeightValue(), textData.LineWeight);
            }
        }

        [Fact]
        public void RenderTextWithMaterial_ReturnsCorrectMaterial()
        {
            var text = CreateBasicText();
            text.Color = new Color(5);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Material);
            Assert.Equal("MeshBasicMaterial", textData.Material.Type);
            Assert.Equal(1.0, textData.Material.Opacity);
            Assert.False(textData.Material.Transparent);
            Assert.True(textData.Material.DepthTest);
            Assert.Equal(5, textData.Material.Color.Index);
        }

        [Fact]
        public void RenderTextWithGeometry_ReturnsCorrectGeometry()
        {
            var text = CreateBasicText();
            text.InsertPoint = new XYZ(10, 20, 30);
            text.Height = 10.0;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Geometry);
            Assert.Equal("TextGeometry", textData.Geometry.Type);
            Assert.NotNull(textData.Bounds);
        }

        [Fact]
        public void RenderTextWithTransform_ReturnsCorrectTransform()
        {
            var text = CreateBasicText();
            text.InsertPoint = new XYZ(10, 20, 30);
            text.Rotation = Math.PI / 4;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Transform);
            Assert.Equal(1.0, textData.Transform.Scale.X);
            Assert.Equal(1.0, textData.Transform.Scale.Y);
            Assert.Equal(1.0, textData.Transform.Scale.Z);
            Assert.NotNull(textData.Transform.Position);
        }

        [Fact]
        public void RenderTextWithDefaultProperties_ReturnsCorrectDefaults()
        {
            var text = new TextEntity();
            text.Value = "Test";
            text.InsertPoint = new XYZ(0, 0, 0);
            text.Height = 10.0;
            text.Rotation = 0.0;
            text.ObliqueAngle = 0.0;
            text.WidthFactor = 1.0;
            text.Color = new ACadSharp.Color(7);
            text.LineType = new LineType("CONTINUOUS");
            text.LineWeight = LineWeightType.ByLayer;
            text.IsInvisible = false;
            text.Normal = new XYZ(0, 0, 1);
            text.HorizontalAlignment = TextHorizontalAlignment.Left;
            text.VerticalAlignment = TextVerticalAlignmentType.Baseline;
            text.Mirror = TextMirrorFlag.None;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal("Test", textData.Value);
            Assert.Equal(7, textData.ColorIndex);
            Assert.Equal("CONTINUOUS", textData.LineTypeName);
            Assert.Equal(0.0, textData.LineWeight);
            Assert.True(textData.Visible);
            Assert.Equal(1.0, textData.Opacity);
            Assert.False(textData.Transparent);
            Assert.True(textData.DepthTest);
            Assert.Equal(0.0, textData.Rotation);
            Assert.Equal(0.0, textData.ObliqueAngle);
            Assert.Equal(1.0, textData.WidthFactor);
            Assert.Equal(TextHorizontalAlignment.Left, textData.HorizontalAlignment);
            Assert.Equal(TextVerticalAlignmentType.Baseline, textData.VerticalAlignment);
            Assert.Equal(TextMirrorFlag.None, textData.Mirror);
        }

        [Fact]
        public void RenderTextWithVertexPositions_ReturnsVertexPositions()
        {
            var text = CreateBasicText();
            text.InsertPoint = new XYZ(10, 20, 30);
            text.Height = 10.0;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.VertexPositions);
            Assert.True(textData.VertexPositions.Length > 0);
        }

        [Fact]
        public void RenderTextWithVertexColors_ReturnsVertexColors()
        {
            var text = CreateBasicText();
            text.Color = new Color(5);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.VertexColors);
            Assert.True(textData.VertexColors.Length > 0);
        }

        [Fact]
        public void RenderTextWithIndices_ReturnsIndices()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Indices);
            Assert.True(textData.Indices.Length > 0);
        }

        [Fact]
        public void RenderTextWithBounds_ReturnsCorrectBounds()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            text.Height = 10.0;
            text.InsertPoint = new XYZ(0, 0, 0);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Bounds);
            Assert.NotNull(textData.Bounds.Min);
            Assert.NotNull(textData.Bounds.Max);
            Assert.Equal(0, textData.Bounds.Min.X);
            Assert.Equal(0, textData.Bounds.Min.Y);
            Assert.Equal(0, textData.Bounds.Min.Z);
            Assert.Equal(24.0, textData.Bounds.Max.X);
            Assert.Equal(10.0, textData.Bounds.Max.Y);
            Assert.Equal(0, textData.Bounds.Max.Z);
        }

        [Fact]
        public void RenderTextWithCentroid_ReturnsCorrectCentroid()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            text.Height = 10.0;
            text.InsertPoint = new XYZ(0, 0, 0);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Centroid);
            Assert.NotNull(textData.Centroid.X);
            Assert.NotNull(textData.Centroid.Y);
            Assert.NotNull(textData.Centroid.Z);
        }

        [Fact]
        public void RenderTextWithWidth_ReturnsCorrectWidth()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            text.Height = 10.0;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(24.0, textData.Width);
        }

        [Fact]
        public void RenderTextWithArea_ReturnsCorrectArea()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            text.Height = 10.0;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(240.0, textData.Area);
        }

        [Fact]
        public void RenderTextWithFontFamily_ReturnsCorrectFontFamily()
        {
            var text = CreateBasicText();
            text.Style = new TextStyle("Arial");

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal("Arial", textData.FontFamily);
        }

        [Fact]
        public void RenderTextWithFontSize_ReturnsCorrectFontSize()
        {
            var text = CreateBasicText();
            text.Height = 15.0;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(15.0, textData.FontSize);
        }

        [Fact]
        public void RenderTextWithTextAlignment_ReturnsCorrectTextAlignment()
        {
            var text = CreateBasicText();
            text.HorizontalAlignment = TextHorizontalAlignment.Center;
            text.VerticalAlignment = TextVerticalAlignmentType.Middle;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal("Center-Middle", textData.TextAlignment);
        }

        [Fact]
        public void RenderTextWithOpacity_ReturnsCorrectOpacity()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(1.0, textData.Opacity);
        }

        [Fact]
        public void RenderTextWithTransparent_ReturnsCorrectTransparent()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.False(textData.Transparent);
        }

        [Fact]
        public void RenderTextWithDepthTest_ReturnsCorrectDepthTest()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.True(textData.DepthTest);
        }

        [Fact]
        public void RenderTextWithColorHex_ReturnsCorrectHexColor()
        {
            var text = CreateBasicText();
            text.Color = new Color(5);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Color);
            Assert.NotNull(textData.Color.Hex);
            Assert.True(textData.Color.Hex.StartsWith("#"));
        }

        [Fact]
        public void RenderTextWithColorRGB_ReturnsCorrectRGBColor()
        {
            var text = CreateBasicText();
            text.Color = new Color(5);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Color);
            Assert.NotNull(textData.Color.R);
            Assert.NotNull(textData.Color.G);
            Assert.NotNull(textData.Color.B);
        }

        [Fact]
        public void RenderTextWithColorIndex_ReturnsCorrectColorIndex()
        {
            var text = CreateBasicText();
            text.Color = new Color(5);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Color);
            Assert.Equal(5, textData.Color.Index);
        }

        [Fact]
        public void RenderTextWithTangent_ReturnsCorrectTangent()
        {
            var text = CreateBasicText();
            text.Rotation = Math.PI / 4;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Tangent);
            Assert.InRange(Math.Abs(textData.Tangent.X - Math.Cos(Math.PI / 4)), 0, 0.0001);
            Assert.InRange(Math.Abs(textData.Tangent.Y - Math.Sin(Math.PI / 4)), 0, 0.0001);
        }

        [Fact]
        public void RenderTextWithBinormal_ReturnsCorrectBinormal()
        {
            var text = CreateBasicText();
            text.Rotation = Math.PI / 4;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Binormal);
            Assert.True(textData.Binormal.Length > 0);
        }

        [Fact]
        public void RenderTextWithUV_ReturnsCorrectUV()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.UV);
            Assert.True(textData.UV.Length > 0);
        }

        [Fact]
        public void RenderTextWithIsMirrored_ReturnsCorrectIsMirrored()
        {
            var text = CreateBasicText();
            text.Mirror = TextMirrorFlag.X;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.True(textData.IsMirrored);
        }

        [Fact]
        public void RenderTextWithIsUpsideDown_ReturnsCorrectIsUpsideDown()
        {
            var text = CreateBasicText();
            text.Rotation = Math.PI;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.True(textData.IsUpsideDown);
        }

        [Fact]
        public void RenderTextWithTextLength_ReturnsCorrectTextLength()
        {
            var text = CreateBasicText();
            text.Value = "Test";
            text.Height = 10.0;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(24.0, textData.TextLength);
        }

        [Fact]
        public void RenderTextWithAscent_ReturnsCorrectAscent()
        {
            var text = CreateBasicText();
            text.Height = 10.0;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(8.0, textData.Ascent);
        }

        [Fact]
        public void RenderTextWithDescent_ReturnsCorrectDescent()
        {
            var text = CreateBasicText();
            text.Height = 10.0;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(2.0, textData.Descent);
        }

        [Fact]
        public void RenderTextWithCharacterCount_ReturnsCorrectCharacterCount()
        {
            var text = CreateBasicText();
            text.Value = "Hello World";

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(11, textData.CharacterCount);
        }

        [Fact]
        public void RenderTextWithFontStyle_ReturnsCorrectFontStyle()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal("Regular", textData.FontStyle);
        }

        [Fact]
        public void RenderTextWithIsBold_ReturnsCorrectIsBold()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.False(textData.IsBold);
        }

        [Fact]
        public void RenderTextWithIsItalic_ReturnsCorrectIsItalic()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.False(textData.IsItalic);
        }

        [Fact]
        public void RenderTextWithVertexCount_ReturnsCorrectVertexCount()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(4, textData.VertexCount);
        }

        [Fact]
        public void RenderTextWithTransformMatrix_ReturnsCorrectTransformMatrix()
        {
            var text = CreateBasicText();
            text.InsertPoint = new XYZ(10, 20, 30);
            text.Rotation = Math.PI / 4;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Transform);
            Assert.NotNull(textData.Transform.Matrix);
            Assert.Equal(16, textData.Transform.Matrix.Length);
        }

        [Fact]
        public void RenderTextWithGeometryType_ReturnsCorrectGeometryType()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Geometry);
            Assert.Equal("TextGeometry", textData.Geometry.Type);
        }

        [Fact]
        public void RenderTextWithGeometryVertexCount_ReturnsCorrectVertexCount()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Geometry);
            Assert.Equal(4, textData.Geometry.VertexCount);
        }

        [Fact]
        public void RenderTextWithGeometryFaceCount_ReturnsCorrectFaceCount()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Geometry);
            Assert.Equal(2, textData.Geometry.FaceCount);
        }

        [Fact]
        public void RenderTextWithGeometryHasNormals_ReturnsCorrectHasNormals()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Geometry);
            Assert.True(textData.Geometry.HasNormals);
        }

        [Fact]
        public void RenderTextWithGeometryHasColors_ReturnsCorrectHasColors()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Geometry);
            Assert.True(textData.Geometry.HasColors);
        }

        [Fact]
        public void RenderTextWithGeometryHasUVs_ReturnsCorrectHasUVs()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Geometry);
            Assert.True(textData.Geometry.HasUVs);
        }

        [Fact]
        public void RenderTextWithGeometryHasIndices_ReturnsCorrectHasIndices()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Geometry);
            Assert.True(textData.Geometry.HasIndices);
        }

        [Fact]
        public void RenderTextWithGeometryPrimitiveType_ReturnsCorrectPrimitiveType()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Geometry);
            Assert.Equal("Triangles", textData.Geometry.PrimitiveType);
        }

        [Fact]
        public void RenderTextWithGeometryIndexCount_ReturnsCorrectIndexCount()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Geometry);
            Assert.Equal(6, textData.Geometry.IndexCount);
        }

        [Fact]
        public void RenderTextWithGeometryIsClosed_ReturnsCorrectIsClosed()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Geometry);
            Assert.False(textData.Geometry.IsClosed);
        }

        [Fact]
        public void RenderTextWithGeometryIsPeriodic_ReturnsCorrectIsPeriodic()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Geometry);
            Assert.False(textData.Geometry.IsPeriodic);
        }

        [Fact]
        public void RenderTextWithMaterialType_ReturnsCorrectMaterialType()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Material);
            Assert.Equal("MeshBasicMaterial", textData.Material.Type);
        }

        [Fact]
        public void RenderTextWithMaterialOpacity_ReturnsCorrectOpacity()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Material);
            Assert.Equal(1.0, textData.Material.Opacity);
        }

        [Fact]
        public void RenderTextWithMaterialTransparent_ReturnsCorrectTransparent()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Material);
            Assert.False(textData.Material.Transparent);
        }

        [Fact]
        public void RenderTextWithMaterialWireframe_ReturnsCorrectWireframe()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Material);
            Assert.False(textData.Material.Wireframe);
        }

        [Fact]
        public void RenderTextWithMaterialLineWidth_ReturnsCorrectLineWidth()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Material);
            Assert.Equal(1.0, textData.Material.LineWidth);
        }

        [Fact]
        public void RenderTextWithMaterialVertexColors_ReturnsCorrectVertexColors()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Material);
            Assert.True(textData.Material.VertexColors);
        }

        [Fact]
        public void RenderTextWithMaterialSide_ReturnsCorrectSide()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Material);
            Assert.True(textData.Material.Side);
        }

        [Fact]
        public void RenderTextWithMaterialDepthTest_ReturnsCorrectDepthTest()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Material);
            Assert.True(textData.Material.DepthTest);
        }

        [Fact]
        public void RenderTextWithMaterialColor_ReturnsCorrectColor()
        {
            var text = CreateBasicText();
            text.Color = new Color(5);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Material);
            Assert.NotNull(textData.Material.Color);
            Assert.Equal(5, textData.Material.Color.Index);
        }

        [Fact]
        public void RenderTextWithTransformRotation_ReturnsCorrectRotation()
        {
            var text = CreateBasicText();
            text.Rotation = Math.PI / 4;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Transform);
            Assert.NotNull(textData.Transform.Rotation);
            Assert.InRange(Math.Abs(textData.Transform.Rotation.Z - Math.PI / 4), 0, 0.0001);
        }

        [Fact]
        public void RenderTextWithTransformPosition_ReturnsCorrectPosition()
        {
            var text = CreateBasicText();
            text.InsertPoint = new XYZ(10, 20, 30);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Transform);
            Assert.NotNull(textData.Transform.Position);
            Assert.NotNull(textData.Transform.Position.X);
            Assert.NotNull(textData.Transform.Position.Y);
            Assert.NotNull(textData.Transform.Position.Z);
        }

        [Fact]
        public void RenderTextWithTransformScale_ReturnsCorrectScale()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Transform);
            Assert.NotNull(textData.Transform.Scale);
            Assert.Equal(1.0, textData.Transform.Scale.X);
            Assert.Equal(1.0, textData.Transform.Scale.Y);
            Assert.Equal(1.0, textData.Transform.Scale.Z);
        }

        [Fact]
        public void RenderTextWithLayerName_ReturnsCorrectLayerName()
        {
            var text = CreateBasicText();
            text.Layer = new Layer("TestLayer");

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal("TestLayer", textData.LayerName);
        }

        [Fact]
        public void RenderTextWithLineTypeName_ReturnsCorrectLineTypeName()
        {
            var text = CreateBasicText();
            var lineType = new LineType("Continuous");
            text.LineType = lineType;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal("Continuous", textData.LineTypeName);
        }

        [Fact]
        public void RenderTextWithHandle_ReturnsCorrectHandle()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(text.Handle.ToString(), textData.Handle);
        }

        [Fact]
        public void RenderTextWithType_ReturnsCorrectType()
        {
            var text = CreateBasicText();

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal("Text", textData.Type);
        }

        [Fact]
        public void RenderTextWithVisible_ReturnsCorrectVisible()
        {
            var text = CreateBasicText();
            text.IsInvisible = false;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.True(textData.Visible);
        }

        [Fact]
        public void RenderTextWithInvisible_ReturnsCorrectInvisible()
        {
            var text = CreateBasicText();
            text.IsInvisible = true;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.False(textData.Visible);
        }

        [Fact]
        public void RenderTextWithThickness_ReturnsCorrectThickness()
        {
            var text = CreateBasicText();
            text.Thickness = 5.0;

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal(5.0, textData.Thickness);
        }

        [Fact]
        public void RenderTextWithNormal_ReturnsCorrectNormal()
        {
            var text = CreateBasicText();
            text.Normal = new XYZ(0, 0, 1);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.Normal);
            Assert.Equal(0, textData.Normal.X);
            Assert.Equal(0, textData.Normal.Y);
            Assert.Equal(1, textData.Normal.Z);
        }

        [Fact]
        public void RenderTextWithInsertPoint_ReturnsCorrectInsertPoint()
        {
            var text = CreateBasicText();
            text.InsertPoint = new XYZ(10, 20, 30);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.InsertPoint);
            Assert.Equal(10, textData.InsertPoint.X);
            Assert.Equal(20, textData.InsertPoint.Y);
            Assert.Equal(30, textData.InsertPoint.Z);
        }

        [Fact]
        public void RenderTextWithAlignmentPoint_ReturnsCorrectAlignmentPoint()
        {
            var text = CreateBasicText();
            text.AlignmentPoint = new XYZ(10, 20, 30);

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.NotNull(textData.AlignmentPoint);
            Assert.Equal(10, textData.AlignmentPoint.X);
            Assert.Equal(20, textData.AlignmentPoint.Y);
            Assert.Equal(30, textData.AlignmentPoint.Z);
        }

        [Fact]
        public void RenderTextWithAllProperties_PreservesAllProperties()
        {
            var text = CreateBasicText();
            text.Value = "Complete Test";
            text.InsertPoint = new XYZ(100, 200, 300);
            text.AlignmentPoint = new XYZ(110, 210, 310);
            text.Height = 15.0;
            text.Rotation = Math.PI / 6;
            text.ObliqueAngle = Math.PI / 12;
            text.WidthFactor = 1.5;
            text.Color = new Color(5);
            text.Layer = new Layer("TestLayer");
            text.LineType = new LineType("Continuous");
            text.LineWeight = ACadSharp.LineWeightType.W30;
            text.IsInvisible = false;
            text.Normal = new XYZ(0, 0, 1);
            text.HorizontalAlignment = TextHorizontalAlignment.Center;
            text.VerticalAlignment = TextVerticalAlignmentType.Middle;
            text.Mirror = TextMirrorFlag.None;
            text.Thickness = 5.0;
            text.Style = new TextStyle("Arial");

            var textData = TextEntityRenderer.Render(text);

            Assert.NotNull(textData);
            Assert.Equal("Complete Test", textData.Value);
            Assert.Equal(100, textData.InsertPoint.X);
            Assert.Equal(200, textData.InsertPoint.Y);
            Assert.Equal(300, textData.InsertPoint.Z);
            Assert.Equal(110, textData.AlignmentPoint.X);
            Assert.Equal(210, textData.AlignmentPoint.Y);
            Assert.Equal(310, textData.AlignmentPoint.Z);
            Assert.Equal(15.0, textData.Height);
            Assert.Equal(Math.PI / 6, textData.Rotation);
            Assert.Equal(Math.PI / 12, textData.ObliqueAngle);
            Assert.Equal(1.5, textData.WidthFactor);
            Assert.Equal(5, textData.ColorIndex);
            Assert.Equal("TestLayer", textData.LayerName);
            Assert.Equal("Continuous", textData.LineTypeName);
            Assert.Equal(0.3, textData.LineWeight);
            Assert.True(textData.Visible);
            Assert.Equal(0, textData.Normal.X);
            Assert.Equal(0, textData.Normal.Y);
            Assert.Equal(1, textData.Normal.Z);
            Assert.Equal(TextHorizontalAlignment.Center, textData.HorizontalAlignment);
            Assert.Equal(TextVerticalAlignmentType.Middle, textData.VerticalAlignment);
            Assert.Equal(TextMirrorFlag.None, textData.Mirror);
            Assert.Equal(5.0, textData.Thickness);
            Assert.Equal("Arial", textData.FontFamily);
        }
    }
}
