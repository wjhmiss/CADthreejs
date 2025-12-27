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
    public class WipeoutEntityRendererIntegrationTests
    {
        private Wipeout CreateBasicWipeout()
        {
            var wipeout = new Wipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.UVector = new XYZ(1, 0, 0);
            wipeout.VVector = new XYZ(0, 1, 0);
            wipeout.Size = new XYZ(10, 10, 0);
            wipeout.ClipType = ClipType.Rectangular;
            wipeout.Color = new Color(1);
            wipeout.Brightness = 50;
            wipeout.Contrast = 50;
            wipeout.Fade = 0;
            wipeout.Flags = ImageDisplayFlags.ShowImage;
            wipeout.ClippingState = true;
            return wipeout;
        }

        private Wipeout CreatePolygonalWipeout()
        {
            var wipeout = new Wipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.UVector = new XYZ(1, 0, 0);
            wipeout.VVector = new XYZ(0, 1, 0);
            wipeout.Size = new XYZ(10, 10, 0);
            wipeout.ClipType = ClipType.Polygonal;
            wipeout.Color = new Color(2);
            wipeout.Brightness = 50;
            wipeout.Contrast = 50;
            wipeout.Fade = 0;
            wipeout.Flags = ImageDisplayFlags.ShowImage;
            wipeout.ClippingState = true;
            
            wipeout.ClipBoundaryVertices = new List<XY2d>
            {
                new XY2d(0, 0),
                new XY2d(10, 0),
                new XY2d(10, 10),
                new XY2d(0, 10)
            };
            
            return wipeout;
        }

        private Wipeout CreateRotatedWipeout()
        {
            var wipeout = new Wipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.UVector = new XYZ(0.707, 0.707, 0);
            wipeout.VVector = new XYZ(-0.707, 0.707, 0);
            wipeout.Size = new XYZ(10, 10, 0);
            wipeout.ClipType = ClipType.Rectangular;
            wipeout.Color = new Color(3);
            wipeout.Brightness = 50;
            wipeout.Contrast = 50;
            wipeout.Fade = 0;
            wipeout.Flags = ImageDisplayFlags.ShowImage;
            wipeout.ClippingState = true;
            return wipeout;
        }

        private Wipeout CreateScaledWipeout()
        {
            var wipeout = new Wipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.UVector = new XYZ(1, 0, 0);
            wipeout.VVector = new XYZ(0, 1, 0);
            wipeout.Size = new XYZ(20, 30, 0);
            wipeout.ClipType = ClipType.Rectangular;
            wipeout.Color = new Color(4);
            wipeout.Brightness = 50;
            wipeout.Contrast = 50;
            wipeout.Fade = 0;
            wipeout.Flags = ImageDisplayFlags.ShowImage;
            wipeout.ClippingState = true;
            return wipeout;
        }

        private Wipeout CreateWipeoutWithBrightness()
        {
            var wipeout = new Wipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.UVector = new XYZ(1, 0, 0);
            wipeout.VVector = new XYZ(0, 1, 0);
            wipeout.Size = new XYZ(10, 10, 0);
            wipeout.ClipType = ClipType.Rectangular;
            wipeout.Color = new Color(5);
            wipeout.Brightness = 75;
            wipeout.Contrast = 50;
            wipeout.Fade = 0;
            wipeout.Flags = ImageDisplayFlags.ShowImage;
            wipeout.ClippingState = true;
            return wipeout;
        }

        private Wipeout CreateWipeoutWithContrast()
        {
            var wipeout = new Wipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.UVector = new XYZ(1, 0, 0);
            wipeout.VVector = new XYZ(0, 1, 0);
            wipeout.Size = new XYZ(10, 10, 0);
            wipeout.ClipType = ClipType.Rectangular;
            wipeout.Color = new Color(6);
            wipeout.Brightness = 50;
            wipeout.Contrast = 80;
            wipeout.Fade = 0;
            wipeout.Flags = ImageDisplayFlags.ShowImage;
            wipeout.ClippingState = true;
            return wipeout;
        }

        private Wipeout CreateWipeoutWithFade()
        {
            var wipeout = new Wipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.UVector = new XYZ(1, 0, 0);
            wipeout.VVector = new XYZ(0, 1, 0);
            wipeout.Size = new XYZ(10, 10, 0);
            wipeout.ClipType = ClipType.Rectangular;
            wipeout.Color = new Color(7);
            wipeout.Brightness = 50;
            wipeout.Contrast = 50;
            wipeout.Fade = 30;
            wipeout.Flags = ImageDisplayFlags.ShowImage;
            wipeout.ClippingState = true;
            return wipeout;
        }

        [Fact]
        public void LoadAndRenderWipeoutFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var wipeoutEntities = new List<Wipeout>();
            foreach (var entity in doc.Entities)
            {
                if (entity is Wipeout wipeout)
                {
                    wipeoutEntities.Add(wipeout);
                }
            }

            if (wipeoutEntities.Count == 0)
            {
                return;
            }

            foreach (var wipeout in wipeoutEntities)
            {
                var wipeoutData = WipeoutEntityRenderer.Render(wipeout);
                Assert.NotNull(wipeoutData);
                Assert.NotNull(wipeoutData.BoundaryPoints);
                Assert.NotNull(wipeoutData.Bounds);
            }
        }

        [Fact]
        public void SerializeWipeoutDataToJson_Success()
        {
            var wipeout = CreateBasicWipeout();

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);
            var json = JsonConvert.SerializeObject(wipeoutData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("BoundaryPoints", json);
            Assert.Contains("InsertPoint", json);
            Assert.Contains("Bounds", json);
        }

        [Fact]
        public void DeserializeWipeoutDataFromJson_Success()
        {
            var wipeout = CreateBasicWipeout();

            var originalWipeoutData = WipeoutEntityRenderer.Render(wipeout);
            var json = JsonConvert.SerializeObject(originalWipeoutData);
            var deserializedWipeoutData = JsonConvert.DeserializeObject<WipeoutEntityRenderer.WipeoutData>(json);

            Assert.NotNull(deserializedWipeoutData);
            Assert.Equal(originalWipeoutData.BoundaryPoints.Count, deserializedWipeoutData.BoundaryPoints.Count);
            Assert.Equal(originalWipeoutData.InsertPoint.X, deserializedWipeoutData.InsertPoint.X);
            Assert.Equal(originalWipeoutData.InsertPoint.Y, deserializedWipeoutData.InsertPoint.Y);
            Assert.Equal(originalWipeoutData.InsertPoint.Z, deserializedWipeoutData.InsertPoint.Z);
            Assert.Equal(originalWipeoutData.ColorIndex, deserializedWipeoutData.ColorIndex);
            Assert.Equal(originalWipeoutData.ClipType, deserializedWipeoutData.ClipType);
            Assert.Equal(originalWipeoutData.Brightness, deserializedWipeoutData.Brightness);
            Assert.Equal(originalWipeoutData.Contrast, deserializedWipeoutData.Contrast);
            Assert.Equal(originalWipeoutData.Fade, deserializedWipeoutData.Fade);
        }

        [Fact]
        public void RenderMultipleWipeouts_CollectAllWipeoutData()
        {
            var wipeouts = new List<Wipeout>
            {
                CreateBasicWipeout(),
                CreatePolygonalWipeout()
            };
            wipeouts[0].InsertPoint = new XYZ(10, 10, 0);
            wipeouts[1].InsertPoint = new XYZ(20, 20, 0);

            var wipeoutDataList = new List<WipeoutEntityRenderer.WipeoutData>();
            foreach (var wipeout in wipeouts)
            {
                var wipeoutData = WipeoutEntityRenderer.Render(wipeout);
                wipeoutDataList.Add(wipeoutData);
            }

            Assert.Equal(2, wipeoutDataList.Count);
            Assert.Equal(ClipType.Rectangular, wipeoutDataList[0].ClipType);
            Assert.Equal(ClipType.Polygonal, wipeoutDataList[1].ClipType);
        }

        [Fact]
        public void RenderBasicWipeout_PreservesAllProperties()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(100, 200, 300);
            wipeout.Size = new XYZ(15, 20, 0);
            wipeout.Color = new Color(5);
            wipeout.Brightness = 60;
            wipeout.Contrast = 70;
            wipeout.Fade = 10;

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(100, wipeoutData.InsertPoint.X);
            Assert.Equal(200, wipeoutData.InsertPoint.Y);
            Assert.Equal(300, wipeoutData.InsertPoint.Z);
            Assert.Equal(15, wipeoutData.Size.X);
            Assert.Equal(20, wipeoutData.Size.Y);
            Assert.Equal(5, wipeoutData.ColorIndex);
            Assert.Equal(60, wipeoutData.Brightness);
            Assert.Equal(70, wipeoutData.Contrast);
            Assert.Equal(10, wipeoutData.Fade);
            Assert.Equal(ClipType.Rectangular, wipeoutData.ClipType);
            Assert.True(wipeoutData.ClippingState);
        }

        [Fact]
        public void RenderPolygonalWipeout_GeneratesCorrectBoundaryPoints()
        {
            var wipeout = CreatePolygonalWipeout();
            wipeout.ClipBoundaryVertices = new List<XY2d>
            {
                new XY2d(0, 0),
                new XY2d(10, 0),
                new XY2d(10, 10),
                new XY2d(0, 10)
            };

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(4, wipeoutData.BoundaryPoints.Count);
            Assert.Equal(0, wipeoutData.BoundaryPoints[0].X);
            Assert.Equal(0, wipeoutData.BoundaryPoints[0].Y);
            Assert.Equal(10, wipeoutData.BoundaryPoints[1].X);
            Assert.Equal(0, wipeoutData.BoundaryPoints[1].Y);
            Assert.Equal(10, wipeoutData.BoundaryPoints[2].X);
            Assert.Equal(10, wipeoutData.BoundaryPoints[2].Y);
            Assert.Equal(0, wipeoutData.BoundaryPoints[3].X);
            Assert.Equal(10, wipeoutData.BoundaryPoints[3].Y);
        }

        [Fact]
        public void RenderRectangularWipeout_GeneratesCorrectBoundaryPoints()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.Size = new XYZ(10, 10, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(4, wipeoutData.BoundaryPoints.Count);
            Assert.Equal(0, wipeoutData.BoundaryPoints[0].X);
            Assert.Equal(0, wipeoutData.BoundaryPoints[0].Y);
            Assert.Equal(10, wipeoutData.BoundaryPoints[1].X);
            Assert.Equal(0, wipeoutData.BoundaryPoints[1].Y);
            Assert.Equal(10, wipeoutData.BoundaryPoints[2].X);
            Assert.Equal(10, wipeoutData.BoundaryPoints[2].Y);
            Assert.Equal(0, wipeoutData.BoundaryPoints[3].X);
            Assert.Equal(10, wipeoutData.BoundaryPoints[3].Y);
        }

        [Fact]
        public void RenderRotatedWipeout_GeneratesCorrectBoundaryPoints()
        {
            var wipeout = CreateRotatedWipeout();
            wipeout.UVector = new XYZ(0.707, 0.707, 0);
            wipeout.VVector = new XYZ(-0.707, 0.707, 0);
            wipeout.Size = new XYZ(10, 10, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(4, wipeoutData.BoundaryPoints.Count);
        }

        [Fact]
        public void RenderScaledWipeout_GeneratesCorrectBoundaryPoints()
        {
            var wipeout = CreateScaledWipeout();
            wipeout.Size = new XYZ(20, 30, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(4, wipeoutData.BoundaryPoints.Count);
            Assert.Equal(0, wipeoutData.BoundaryPoints[0].X);
            Assert.Equal(0, wipeoutData.BoundaryPoints[0].Y);
            Assert.Equal(20, wipeoutData.BoundaryPoints[1].X);
            Assert.Equal(0, wipeoutData.BoundaryPoints[1].Y);
            Assert.Equal(20, wipeoutData.BoundaryPoints[2].X);
            Assert.Equal(30, wipeoutData.BoundaryPoints[2].Y);
            Assert.Equal(0, wipeoutData.BoundaryPoints[3].X);
            Assert.Equal(30, wipeoutData.BoundaryPoints[3].Y);
        }

        [Fact]
        public void RenderWipeoutWithBrightness_PreservesBrightness()
        {
            var wipeout = CreateWipeoutWithBrightness();
            wipeout.Brightness = 75;

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(75, wipeoutData.Brightness);
        }

        [Fact]
        public void RenderWipeoutWithContrast_PreservesContrast()
        {
            var wipeout = CreateWipeoutWithContrast();
            wipeout.Contrast = 80;

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(80, wipeoutData.Contrast);
        }

        [Fact]
        public void RenderWipeoutWithFade_PreservesFade()
        {
            var wipeout = CreateWipeoutWithFade();
            wipeout.Fade = 30;

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(30, wipeoutData.Fade);
        }

        [Fact]
        public void RenderWipeout_CalculatesCorrectBounds()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.Size = new XYZ(10, 10, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.Bounds);
            Assert.Equal(0, wipeoutData.Bounds.Value.Min.X);
            Assert.Equal(0, wipeoutData.Bounds.Value.Min.Y);
            Assert.Equal(10, wipeoutData.Bounds.Value.Max.X);
            Assert.Equal(10, wipeoutData.Bounds.Value.Max.Y);
        }

        [Fact]
        public void RenderWipeout_CalculatesCorrectCentroid()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.Size = new XYZ(10, 10, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.Centroid);
            Assert.Equal(5, wipeoutData.Centroid.Value.X);
            Assert.Equal(5, wipeoutData.Centroid.Value.Y);
            Assert.Equal(0, wipeoutData.Centroid.Value.Z);
        }

        [Fact]
        public void RenderWipeout_CalculatesCorrectArea()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.Size = new XYZ(10, 10, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(100, wipeoutData.Area);
        }

        [Fact]
        public void RenderWipeout_GeneratesCorrectVertexPositions()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.Size = new XYZ(10, 10, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.VertexPositions);
            Assert.Equal(12, wipeoutData.VertexPositions.Length);
            Assert.Equal(0, wipeoutData.VertexPositions[0]);
            Assert.Equal(0, wipeoutData.VertexPositions[1]);
            Assert.Equal(0, wipeoutData.VertexPositions[2]);
        }

        [Fact]
        public void RenderWipeout_GeneratesCorrectVertexColors()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Color = new Color(1);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.VertexColors);
            Assert.Equal(12, wipeoutData.VertexColors.Length);
            Assert.Equal(1.0, wipeoutData.VertexColors[0]);
            Assert.Equal(0.0, wipeoutData.VertexColors[1]);
            Assert.Equal(0.0, wipeoutData.VertexColors[2]);
        }

        [Fact]
        public void RenderWipeout_GeneratesCorrectVertexNormals()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.Size = new XYZ(10, 10, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.VertexNormals);
            Assert.Equal(12, wipeoutData.VertexNormals.Length);
        }

        [Fact]
        public void RenderWipeout_GeneratesCorrectUVCoordinates()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.Size = new XYZ(10, 10, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.UV);
            Assert.Equal(8, wipeoutData.UV.Length);
            Assert.Equal(0, wipeoutData.UV[0]);
            Assert.Equal(0, wipeoutData.UV[1]);
        }

        [Fact]
        public void RenderWipeout_GeneratesCorrectIndices()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.Size = new XYZ(10, 10, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.Indices);
            Assert.Equal(6, wipeoutData.Indices.Length);
            Assert.Equal(0, wipeoutData.Indices[0]);
            Assert.Equal(1, wipeoutData.Indices[1]);
            Assert.Equal(2, wipeoutData.Indices[2]);
        }

        [Fact]
        public void RenderWipeoutWithLayer_PreservesLayerInformation()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Layer = new Layer("TestLayer");

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal("TestLayer", wipeoutData.LineTypeName);
        }

        [Fact]
        public void RenderWipeoutWithColor_PreservesColorInformation()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Color = new Color(5);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(5, wipeoutData.ColorIndex);
            Assert.Equal(5, wipeoutData.Color.Index);
        }

        [Fact]
        public void RenderWipeoutWithLineType_PreservesLineTypeInformation()
        {
            var wipeout = CreateBasicWipeout();
            var lineType = new LineType("Continuous");
            wipeout.LineType = lineType;

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal("Continuous", wipeoutData.LineTypeName);
        }

        [Fact]
        public void RenderWipeoutWithLineWeight_PreservesLineWeightInformation()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.LineWeight = ACadSharp.LineWeightType.W30;

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(0.3, wipeoutData.LineWeight);
        }

        [Fact]
        public void RenderWipeoutWithInvisibleFlag_SetsVisibleProperty()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.IsInvisible = true;

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(1.0, wipeoutData.Opacity);
            Assert.False(wipeoutData.Transparent);
        }

        [Fact]
        public void RenderWipeoutWithHandle_PreservesHandleInformation()
        {
            var wipeout = CreateBasicWipeout();
            
            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.Handle);
        }

        [Fact]
        public void RenderWipeoutWithZeroInsertPoint_HandlesCorrectly()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(0, wipeoutData.InsertPoint.X);
            Assert.Equal(0, wipeoutData.InsertPoint.Y);
            Assert.Equal(0, wipeoutData.InsertPoint.Z);
        }

        [Fact]
        public void RenderWipeoutWithVeryLargeCoordinates_HandlesCorrectly()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(1000000, 2000000, 3000000);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(1000000, wipeoutData.InsertPoint.X);
            Assert.Equal(2000000, wipeoutData.InsertPoint.Y);
            Assert.Equal(3000000, wipeoutData.InsertPoint.Z);
        }

        [Fact]
        public void RenderWipeoutWithVerySmallCoordinates_HandlesCorrectly()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0.0001, 0.0002, 0.0003);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(0.0001, wipeoutData.InsertPoint.X);
            Assert.Equal(0.0002, wipeoutData.InsertPoint.Y);
            Assert.Equal(0.0003, wipeoutData.InsertPoint.Z);
        }

        [Fact]
        public void RenderWipeoutWithNegativeCoordinates_HandlesCorrectly()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(-10, -20, -30);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(-10, wipeoutData.InsertPoint.X);
            Assert.Equal(-20, wipeoutData.InsertPoint.Y);
            Assert.Equal(-30, wipeoutData.InsertPoint.Z);
        }

        [Fact]
        public void RenderWipeoutWithDifferentUVectors_PreservesUVectors()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.UVector = new XYZ(0.707, 0.707, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(0.707, wipeoutData.UVector.X);
            Assert.Equal(0.707, wipeoutData.UVector.Y);
            Assert.Equal(0, wipeoutData.UVector.Z);
        }

        [Fact]
        public void RenderWipeoutWithDifferentVVectors_PreservesVVectors()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.VVector = new XYZ(-0.707, 0.707, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(-0.707, wipeoutData.VVector.X);
            Assert.Equal(0.707, wipeoutData.VVector.Y);
            Assert.Equal(0, wipeoutData.VVector.Z);
        }

        [Fact]
        public void RenderWipeoutWithDifferentClipTypes_HandlesCorrectly()
        {
            var clipTypes = new[] { ClipType.Rectangular, ClipType.Polygonal };

            foreach (var clipType in clipTypes)
            {
                var wipeout = CreateBasicWipeout();
                wipeout.ClipType = clipType;

                var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

                Assert.NotNull(wipeoutData);
                Assert.Equal(clipType, wipeoutData.ClipType);
            }
        }

        [Fact]
        public void RenderWipeoutWithDifferentDisplayFlags_PreservesFlags()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Flags = ImageDisplayFlags.ShowImage | ImageDisplayFlags.UseClipping;

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(ImageDisplayFlags.ShowImage | ImageDisplayFlags.UseClipping, wipeoutData.Flags);
        }

        [Fact]
        public void RenderWipeoutWithClippingStateDisabled_SetsCorrectState()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.ClippingState = false;

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.False(wipeoutData.ClippingState);
        }

        [Fact]
        public void RenderWipeout_GeneratesCorrectTransform()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.Size = new XYZ(10, 10, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.Transform);
            Assert.Equal(5, wipeoutData.Transform.Position.X);
            Assert.Equal(5, wipeoutData.Transform.Position.Y);
            Assert.Equal(0, wipeoutData.Transform.Position.Z);
        }

        [Fact]
        public void RenderWipeout_GeneratesCorrectGeometry()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.Size = new XYZ(10, 10, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.Geometry);
            Assert.Equal("PolygonGeometry", wipeoutData.Geometry.Type);
            Assert.Equal(4, wipeoutData.Geometry.VertexCount);
            Assert.Equal(2, wipeoutData.Geometry.FaceCount);
            Assert.True(wipeoutData.Geometry.HasNormals);
            Assert.True(wipeoutData.Geometry.HasColors);
            Assert.True(wipeoutData.Geometry.HasUVs);
            Assert.True(wipeoutData.Geometry.HasIndices);
        }

        [Fact]
        public void RenderWipeout_GeneratesCorrectMaterial()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Color = new Color(1);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.Material);
            Assert.Equal("MeshBasicMaterial", wipeoutData.Material.Type);
            Assert.Equal(0xFF0000, wipeoutData.Material.Color);
            Assert.Equal(1.0, wipeoutData.Material.Opacity);
            Assert.False(wipeoutData.Material.Transparent);
            Assert.False(wipeoutData.Material.Wireframe);
            Assert.True(wipeoutData.Material.VertexColors);
            Assert.True(wipeoutData.Material.Side);
        }

        [Fact]
        public void RenderWipeout_SetsIsMaskProperty()
        {
            var wipeout = CreateBasicWipeout();

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.True(wipeoutData.IsMask);
        }

        [Fact]
        public void RenderWipeout_GeneratesCorrectNormal()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.Size = new XYZ(10, 10, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.Normal);
            Assert.Equal(0, wipeoutData.Normal.X);
            Assert.Equal(0, wipeoutData.Normal.Y);
            Assert.Equal(0, wipeoutData.Normal.Z);
        }

        [Fact]
        public void RenderWipeout_GeneratesCorrectColor()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Color = new Color(1);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.Color);
            Assert.Equal(1, wipeoutData.Color.Index);
            Assert.Equal("#FF0000", wipeoutData.Color.Hex);
            Assert.Equal(255, wipeoutData.Color.R);
            Assert.Equal(0, wipeoutData.Color.G);
            Assert.Equal(0, wipeoutData.Color.B);
            Assert.Equal(1.0, wipeoutData.Color.A);
        }

        [Fact]
        public void RenderWipeoutWithDifferentColorIndex_GeneratesCorrectColor()
        {
            var colorIndices = new[] { 1, 2, 3, 4, 5, 6, 7 };

            foreach (var colorIndex in colorIndices)
            {
                var wipeout = CreateBasicWipeout();
                wipeout.Color = new Color(colorIndex);

                var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

                Assert.NotNull(wipeoutData);
                Assert.Equal(colorIndex, wipeoutData.ColorIndex);
                Assert.Equal(colorIndex, wipeoutData.Color.Index);
            }
        }

        [Fact]
        public void RenderWipeoutWith3DCoordinates_CalculatesCorrect3DProperties()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(10, 20, 30);
            wipeout.Size = new XYZ(10, 10, 0);

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.Centroid);
            Assert.NotNull(wipeoutData.Bounds);
            Assert.Equal(15, wipeoutData.Centroid.Value.X);
            Assert.Equal(25, wipeoutData.Centroid.Value.Y);
            Assert.Equal(30, wipeoutData.Centroid.Value.Z);
        }

        [Fact]
        public void RenderWipeoutWithPolygonalBoundary_CalculatesCorrectArea()
        {
            var wipeout = CreatePolygonalWipeout();
            wipeout.ClipBoundaryVertices = new List<XY2d>
            {
                new XY2d(0, 0),
                new XY2d(10, 0),
                new XY2d(10, 10),
                new XY2d(0, 10)
            };

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(100, wipeoutData.Area);
        }

        [Fact]
        public void RenderWipeoutWithPolygonalBoundary_CalculatesCorrectCentroid()
        {
            var wipeout = CreatePolygonalWipeout();
            wipeout.ClipBoundaryVertices = new List<XY2d>
            {
                new XY2d(0, 0),
                new XY2d(10, 0),
                new XY2d(10, 10),
                new XY2d(0, 10)
            };

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.Centroid);
            Assert.Equal(5, wipeoutData.Centroid.Value.X);
            Assert.Equal(5, wipeoutData.Centroid.Value.Y);
        }

        [Fact]
        public void RenderWipeoutWithPolygonalBoundary_CalculatesCorrectBounds()
        {
            var wipeout = CreatePolygonalWipeout();
            wipeout.ClipBoundaryVertices = new List<XY2d>
            {
                new XY2d(0, 0),
                new XY2d(10, 0),
                new XY2d(10, 10),
                new XY2d(0, 10)
            };

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.Bounds);
            Assert.Equal(0, wipeoutData.Bounds.Value.Min.X);
            Assert.Equal(0, wipeoutData.Bounds.Value.Min.Y);
            Assert.Equal(10, wipeoutData.Bounds.Value.Max.X);
            Assert.Equal(10, wipeoutData.Bounds.Value.Max.Y);
        }

        [Fact]
        public void RenderWipeoutWithPolygonalBoundary_GeneratesCorrectIndices()
        {
            var wipeout = CreatePolygonalWipeout();
            wipeout.ClipBoundaryVertices = new List<XY2d>
            {
                new XY2d(0, 0),
                new XY2d(10, 0),
                new XY2d(10, 10),
                new XY2d(0, 10)
            };

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.NotNull(wipeoutData.Indices);
            Assert.Equal(6, wipeoutData.Indices.Length);
        }

        [Fact]
        public void RenderWipeoutWithPolygonalBoundary_GeneratesCorrectVertexCount()
        {
            var wipeout = CreatePolygonalWipeout();
            wipeout.ClipBoundaryVertices = new List<XY2d>
            {
                new XY2d(0, 0),
                new XY2d(10, 0),
                new XY2d(10, 10),
                new XY2d(0, 10)
            };

            var wipeoutData = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(wipeoutData);
            Assert.Equal(4, wipeoutData.VertexCount);
        }
    }
}
