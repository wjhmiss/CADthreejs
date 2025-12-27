using Xunit;
using ACadSharp.Entities;
using ACadSharp.IO;
using ACadSharp.Tables;
using ACadSharp.Objects;
using ACadSharp;
using DxfDwgViewer.RenderUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class MTextEntityRendererIntegrationTests
    {
        [Fact]
        public void LoadAndRenderMTextFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var mtextEntities = new List<MText>();
            foreach (var entity in doc.Entities)
            {
                if (entity is MText mtext)
                {
                    mtextEntities.Add(mtext);
                }
            }

            if (mtextEntities.Count == 0)
            {
                return;
            }

            foreach (var mtext in mtextEntities)
            {
                var mtextData = MTextEntityRenderer.Render(mtext);
                Assert.NotNull(mtextData);
                Assert.Equal("MText", mtextData.Type);
                Assert.True(mtextData.Height > 0);
                Assert.NotNull(mtextData.InsertPoint);
                Assert.NotNull(mtextData.Lines);
                Assert.NotNull(mtextData.Bounds);
            }
        }

        [Fact]
        public void SerializeMTextDataToJson_Success()
        {
            var mtext = CreateBasicMText();

            var mtextData = MTextEntityRenderer.Render(mtext);
            var json = JsonConvert.SerializeObject(mtextData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("MText", json);
            Assert.Contains("Lines", json);
            Assert.Contains("InsertPoint", json);
            Assert.Contains("Height", json);
        }

        [Fact]
        public void DeserializeMTextDataFromJson_Success()
        {
            var mtext = CreateBasicMText();

            var originalMTextData = MTextEntityRenderer.Render(mtext);
            var json = JsonConvert.SerializeObject(originalMTextData);
            var deserializedMTextData = JsonConvert.DeserializeObject<MTextEntityRenderer.MTextData>(json);

            Assert.NotNull(deserializedMTextData);
            Assert.Equal(originalMTextData.Type, deserializedMTextData.Type);
            Assert.Equal(originalMTextData.LineCount, deserializedMTextData.LineCount);
            Assert.Equal(originalMTextData.Height, deserializedMTextData.Height);
            Assert.Equal(originalMTextData.ColorIndex, deserializedMTextData.ColorIndex);
            Assert.Equal(originalMTextData.LineWeight, deserializedMTextData.LineWeight);
            Assert.Equal(originalMTextData.RectangleWidth, deserializedMTextData.RectangleWidth);
            Assert.Equal(originalMTextData.LineSpacingStyle, deserializedMTextData.LineSpacingStyle);
            Assert.Equal(originalMTextData.LineSpacing, deserializedMTextData.LineSpacing);
            Assert.Equal(originalMTextData.AttachmentPoint, deserializedMTextData.AttachmentPoint);
            Assert.Equal(originalMTextData.Rotation, deserializedMTextData.Rotation);
            Assert.Equal(originalMTextData.TextStyleName, deserializedMTextData.TextStyleName);
            Assert.Equal(originalMTextData.WidthFactor, deserializedMTextData.WidthFactor);
            Assert.Equal(originalMTextData.ObliqueAngle, deserializedMTextData.ObliqueAngle);
            Assert.Equal(originalMTextData.DrawingDirection, deserializedMTextData.DrawingDirection);
            Assert.Equal(originalMTextData.BackgroundFillFlags, deserializedMTextData.BackgroundFillFlags);
            Assert.Equal(originalMTextData.BackgroundScale, deserializedMTextData.BackgroundScale);
            Assert.Equal(originalMTextData.BackgroundTransparency, deserializedMTextData.BackgroundTransparency);
            Assert.Equal(originalMTextData.HorizontalWidth, deserializedMTextData.HorizontalWidth);
            Assert.Equal(originalMTextData.RectangleHeight, deserializedMTextData.RectangleHeight);
            Assert.Equal(originalMTextData.VerticalHeight, deserializedMTextData.VerticalHeight);
            Assert.Equal(originalMTextData.IsAnnotative, deserializedMTextData.IsAnnotative);
            Assert.Equal(originalMTextData.PlainText, deserializedMTextData.PlainText);
            Assert.Equal(originalMTextData.TextValue, deserializedMTextData.TextValue);
        }

        [Fact]
        public void RenderMultipleMTexts_CollectAllMTextData()
        {
            var mtexts = new List<MText>
            {
                CreateBasicMText(),
                CreateBasicMText()
            };
            mtexts[0].Value = "First MText";
            mtexts[0].InsertPoint = new XYZ(10, 10, 0);
            mtexts[1].Value = "Second MText";
            mtexts[1].InsertPoint = new XYZ(20, 20, 0);

            var mtextDataList = new List<MTextEntityRenderer.MTextData>();
            foreach (var mtext in mtexts)
            {
                var mtextData = MTextEntityRenderer.Render(mtext);
                mtextDataList.Add(mtextData);
            }

            Assert.Equal(2, mtextDataList.Count);
            Assert.Equal("First MText", mtextDataList[0].TextValue);
            Assert.Equal("Second MText", mtextDataList[1].TextValue);
            Assert.Equal(10, mtextDataList[0].InsertPoint.X);
            Assert.Equal(20, mtextDataList[1].InsertPoint.X);
        }

        [Fact]
        public void RenderMTextWithComplexFormatting_PreservesAllProperties()
        {
            var mtext = CreateBasicMText();
            mtext.Value = "Line1\\PLine2\\PLine3";
            mtext.Height = 10;
            mtext.AlignmentPoint = new XYZ(Math.Cos(Math.PI / 4), Math.Sin(Math.PI / 4), 0);
            mtext.LineSpacingStyle = LineSpacingStyleType.AtLeast;
            mtext.LineSpacing = 1.5;
            mtext.AttachmentPoint = AttachmentPointType.MiddleCenter;
            mtext.DrawingDirection = DrawingDirectionType.LeftToRight;
            mtext.BackgroundFillFlags = BackgroundFillFlags.UseBackgroundFillColor;
            mtext.BackgroundColor = new Color(5);
            mtext.BackgroundScale = 1.2;
            mtext.BackgroundTransparency = new ACadSharp.Transparency(50);
            mtext.HorizontalWidth = 100;
            mtext.RectangleHeight = 50;
            mtext.VerticalHeight = 60;
            mtext.IsAnnotative = true;

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.Equal(3, mtextData.LineCount);
            Assert.Equal("Line1", mtextData.Lines[0]);
            Assert.Equal("Line2", mtextData.Lines[1]);
            Assert.Equal("Line3", mtextData.Lines[2]);
            Assert.Equal(10, mtextData.Height);
            Assert.Equal(Math.PI / 4, mtextData.Rotation);
            Assert.Equal(LineSpacingStyleType.AtLeast, mtextData.LineSpacingStyle);
            Assert.Equal(1.5, mtextData.LineSpacing);
            Assert.Equal(AttachmentPointType.MiddleCenter, mtextData.AttachmentPoint);
            Assert.Equal(DrawingDirectionType.LeftToRight, mtextData.DrawingDirection);
            Assert.Equal(BackgroundFillFlags.UseBackgroundFillColor, mtextData.BackgroundFillFlags);
            Assert.Equal(5, mtextData.BackgroundColor.Index);
            Assert.Equal(1.2, mtextData.BackgroundScale);
            Assert.Equal(50, mtextData.BackgroundTransparency);
            Assert.Equal(100, mtextData.HorizontalWidth);
            Assert.Equal(50, mtextData.RectangleHeight);
            Assert.Equal(60, mtextData.VerticalHeight);
            Assert.True(mtextData.IsAnnotative);
        }

        [Fact]
        public void RenderMTextWith3DCoordinates_CalculatesCorrect3DProperties()
        {
            var mtext = CreateBasicMText();
            mtext.InsertPoint = new XYZ(10, 20, 30);
            mtext.AlignmentPoint = new XYZ(15, 25, 35);
            mtext.Normal = new XYZ(0, 0, 1);
            mtext.Value = "3D Text";

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.Equal(10, mtextData.InsertPoint3D.X);
            Assert.Equal(20, mtextData.InsertPoint3D.Y);
            Assert.Equal(30, mtextData.InsertPoint3D.Z);
            Assert.Equal(15, mtextData.AlignmentPoint3D.X);
            Assert.Equal(25, mtextData.AlignmentPoint3D.Y);
            Assert.Equal(35, mtextData.AlignmentPoint3D.Z);
            Assert.Equal(0, mtextData.Normal.X);
            Assert.Equal(0, mtextData.Normal.Y);
            Assert.Equal(1, mtextData.Normal.Z);
        }

        [Fact]
        public void RenderMTextWithRotation_CalculatesCorrectAlignmentPoint()
        {
            var mtext = CreateBasicMText();
            mtext.Value = "Rotated Text";
            mtext.AlignmentPoint = new XYZ(Math.Cos(Math.PI / 2), Math.Sin(Math.PI / 2), 0);
            mtext.Height = 10;
            mtext.AttachmentPoint = AttachmentPointType.TopLeft;

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.NotNull(mtextData.AlignmentPoint);
            Assert.Equal(Math.PI / 2, mtextData.Rotation);
        }

        [Fact]
        public void RenderMTextWithDifferentAttachmentPoints_CalculatesCorrectBounds()
        {
            var attachmentPoints = new[]
            {
                AttachmentPointType.TopLeft,
                AttachmentPointType.TopCenter,
                AttachmentPointType.TopRight,
                AttachmentPointType.MiddleLeft,
                AttachmentPointType.MiddleCenter,
                AttachmentPointType.MiddleRight,
                AttachmentPointType.BottomLeft,
                AttachmentPointType.BottomCenter,
                AttachmentPointType.BottomRight
            };

            foreach (var attachmentPoint in attachmentPoints)
            {
                var mtext = CreateBasicMText();
                mtext.Value = "Test Text";
                mtext.AttachmentPoint = attachmentPoint;

                var mtextData = MTextEntityRenderer.Render(mtext);

                Assert.NotNull(mtextData);
                Assert.Equal(attachmentPoint, mtextData.AttachmentPoint);
                Assert.NotNull(mtextData.Bounds);
                Assert.True(mtextData.Bounds.Min.X <= mtextData.Bounds.Max.X);
                Assert.True(mtextData.Bounds.Min.Y <= mtextData.Bounds.Max.Y);
            }
        }

        [Fact]
        public void RenderMTextWithBackgroundFill_CalculatesCorrectBackgroundProperties()
        {
            var mtext = CreateBasicMText();
            mtext.Value = "Background Text";
            mtext.BackgroundFillFlags = BackgroundFillFlags.UseBackgroundFillColor;
            mtext.BackgroundColor = new Color(3);
            mtext.BackgroundScale = 1.5;
            mtext.BackgroundTransparency = new ACadSharp.Transparency(80);

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.Equal(BackgroundFillFlags.UseBackgroundFillColor, mtextData.BackgroundFillFlags);
            Assert.Equal(3, mtextData.BackgroundColor.Index);
            Assert.Equal(1.5, mtextData.BackgroundScale);
            Assert.Equal(80, mtextData.BackgroundTransparency);
        }

        [Fact]
        public void RenderMTextWithTextStyle_PreservesTextStyleProperties()
        {
            var mtext = CreateBasicMText();
            var textStyle = new TextStyle("MyStyle");
            textStyle.Width = 1.5;
            textStyle.ObliqueAngle = Math.PI / 6;
            mtext.Style = textStyle;

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.Equal("MyStyle", mtextData.TextStyleName);
            Assert.Equal(1.5, mtextData.WidthFactor);
            Assert.Equal(Math.PI / 6, mtextData.ObliqueAngle);
        }

        [Fact]
        public void RenderMTextWithSpecialCharacters_ParsesCorrectly()
        {
            var mtext = CreateBasicMText();
            mtext.Value = "Line1\\PLine2\\PLine3\\P\\PLine5";

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.Equal(5, mtextData.LineCount);
            Assert.Equal("Line1", mtextData.Lines[0]);
            Assert.Equal("Line2", mtextData.Lines[1]);
            Assert.Equal("Line3", mtextData.Lines[2]);
            Assert.Equal("", mtextData.Lines[3]);
            Assert.Equal("Line5", mtextData.Lines[4]);
        }

        [Fact]
        public void RenderMTextWithEmptyText_HandlesGracefully()
        {
            var mtext = CreateBasicMText();
            mtext.Value = "";

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.Equal(1, mtextData.LineCount);
            Assert.Equal("", mtextData.PlainText);
            Assert.Equal("", mtextData.TextValue);
        }

        [Fact]
        public void RenderMTextWithVeryLargeCoordinates_HandlesCorrectly()
        {
            var mtext = CreateBasicMText();
            mtext.InsertPoint = new XYZ(1000000, 2000000, 3000000);
            mtext.Value = "Large Coordinates";

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.Equal(1000000, mtextData.InsertPoint.X);
            Assert.Equal(2000000, mtextData.InsertPoint.Y);
            Assert.Equal(1000000, mtextData.InsertPoint3D.X);
            Assert.Equal(2000000, mtextData.InsertPoint3D.Y);
            Assert.Equal(3000000, mtextData.InsertPoint3D.Z);
        }

        [Fact]
        public void RenderMTextWithVerySmallCoordinates_HandlesCorrectly()
        {
            var mtext = CreateBasicMText();
            mtext.InsertPoint = new XYZ(0.0001, 0.0002, 0.0003);
            mtext.Value = "Small Coordinates";

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.Equal(0.0001, mtextData.InsertPoint.X);
            Assert.Equal(0.0002, mtextData.InsertPoint.Y);
            Assert.Equal(0.0001, mtextData.InsertPoint3D.X);
            Assert.Equal(0.0002, mtextData.InsertPoint3D.Y);
            Assert.Equal(0.0003, mtextData.InsertPoint3D.Z);
        }

        [Fact]
        public void RenderMTextWithNegativeCoordinates_HandlesCorrectly()
        {
            var mtext = CreateBasicMText();
            mtext.InsertPoint = new XYZ(-10, -20, -30);
            mtext.Value = "Negative Coordinates";

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.Equal(-10, mtextData.InsertPoint.X);
            Assert.Equal(-20, mtextData.InsertPoint.Y);
            Assert.Equal(-10, mtextData.InsertPoint3D.X);
            Assert.Equal(-20, mtextData.InsertPoint3D.Y);
            Assert.Equal(-30, mtextData.InsertPoint3D.Z);
        }

        [Fact]
        public void RenderMTextWithZeroRotation_HandlesCorrectly()
        {
            var mtext = CreateBasicMText();
            mtext.AlignmentPoint = new XYZ(1, 0, 0);
            mtext.Value = "Zero Rotation";

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.Equal(0, mtextData.Rotation);
        }

        [Fact]
        public void RenderMTextWithFullRotation_HandlesCorrectly()
        {
            var mtext = CreateBasicMText();
            mtext.AlignmentPoint = new XYZ(Math.Cos(2 * Math.PI), Math.Sin(2 * Math.PI), 0);
            mtext.Value = "Full Rotation";

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.InRange(mtextData.Rotation, -0.0001, 0.0001);
        }

        [Fact]
        public void RenderMTextWithDifferentLineSpacingStyles_PreservesStyles()
        {
            var lineSpacingStyles = new[]
            {
                LineSpacingStyleType.AtLeast,
                LineSpacingStyleType.Exact
            };

            foreach (var lineSpacingStyle in lineSpacingStyles)
            {
                var mtext = CreateBasicMText();
                mtext.LineSpacingStyle = lineSpacingStyle;
                mtext.LineSpacing = 1.5;

                var mtextData = MTextEntityRenderer.Render(mtext);

                Assert.NotNull(mtextData);
                Assert.Equal(lineSpacingStyle, mtextData.LineSpacingStyle);
                Assert.Equal(1.5, mtextData.LineSpacing);
            }
        }

        [Fact]
        public void RenderMTextWithDifferentDrawingDirections_PreservesDirections()
        {
            var drawingDirections = new[]
            {
                DrawingDirectionType.LeftToRight,
                DrawingDirectionType.TopToBottom,
                DrawingDirectionType.ByStyle
            };

            foreach (var drawingDirection in drawingDirections)
            {
                var mtext = CreateBasicMText();
                mtext.DrawingDirection = drawingDirection;

                var mtextData = MTextEntityRenderer.Render(mtext);

                Assert.NotNull(mtextData);
                Assert.Equal(drawingDirection, mtextData.DrawingDirection);
            }
        }

        [Fact]
        public void RenderMTextWithDifferentBackgroundFillTypes_PreservesTypes()
        {
            var backgroundFillTypes = new[]
            {
                BackgroundFillFlags.None,
                BackgroundFillFlags.UseBackgroundFillColor,
                BackgroundFillFlags.UseDrawingWindowColor
            };

            foreach (var backgroundFillType in backgroundFillTypes)
            {
                var mtext = CreateBasicMText();
                mtext.BackgroundFillFlags = backgroundFillType;

                var mtextData = MTextEntityRenderer.Render(mtext);

                Assert.NotNull(mtextData);
                Assert.Equal(backgroundFillType, mtextData.BackgroundFillFlags);
            }
        }

        [Fact]
        public void RenderMTextWithLayer_PreservesLayerInformation()
        {
            var mtext = CreateBasicMText();
            mtext.Layer = new Layer("TestLayer");

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.Equal("TestLayer", mtextData.LayerName);
        }

        [Fact]
        public void RenderMTextWithColor_PreservesColorInformation()
        {
            var mtext = CreateBasicMText();
            mtext.Color = new Color(5);

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.Equal(5, mtextData.ColorIndex);
            Assert.Equal(5, mtextData.Color.Index);
        }

        [Fact]
        public void RenderMTextWithLineType_PreservesLineTypeInformation()
        {
            var mtext = CreateBasicMText();
            var lineType = new LineType("Continuous");
            mtext.LineType = lineType;

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.Equal("Continuous", mtextData.LineTypeName);
        }

        [Fact]
        public void RenderMTextWithLineWeight_PreservesLineWeightInformation()
        {
            var mtext = CreateBasicMText();
            mtext.LineWeight = ACadSharp.LineWeightType.W30;

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.Equal(0.3, mtextData.LineWeight);
        }

        [Fact]
        public void RenderMTextWithInvisibleFlag_SetsVisibleProperty()
        {
            var mtext = CreateBasicMText();
            mtext.IsInvisible = true;

            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.False(mtextData.Visible);
        }

        [Fact]
        public void RenderMTextWithHandle_PreservesHandleInformation()
        {
            var mtext = CreateBasicMText();
            
            var mtextData = MTextEntityRenderer.Render(mtext);

            Assert.NotNull(mtextData);
            Assert.NotNull(mtextData.Handle);
        }

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
    }
}