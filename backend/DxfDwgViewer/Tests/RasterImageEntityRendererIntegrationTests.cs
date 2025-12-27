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
    public class RasterImageEntityRendererIntegrationTests
    {
        private RasterImage CreateBasicRasterImage()
        {
            var rasterImage = new RasterImage
            {
                InsertPoint = new XYZ(0, 0, 0),
                UVector = new XYZ(1, 0, 0),
                VVector = new XYZ(0, 1, 0),
                Size = new XY(100, 100),
                Color = new Color(7),
                Brightness = 50,
                Contrast = 50,
                Fade = 0,
                Flags = ImageDisplayFlags.ShowImage,
                ClipType = ClipType.Rectangular,
                ClippingState = false
            };
            return rasterImage;
        }

        [Fact]
        public void LoadAndRenderRasterImageFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var rasterImageEntities = new List<RasterImage>();
            foreach (var entity in doc.Entities)
            {
                if (entity is RasterImage rasterImage)
                {
                    rasterImageEntities.Add(rasterImage);
                }
            }

            if (rasterImageEntities.Count == 0)
            {
                return;
            }

            foreach (var rasterImage in rasterImageEntities)
            {
                var rasterImageData = RasterImageEntityRenderer.Render(rasterImage);
                Assert.NotNull(rasterImageData);
                Assert.NotNull(rasterImageData.InsertPoint);
                Assert.NotNull(rasterImageData.UVector);
                Assert.NotNull(rasterImageData.VVector);
                Assert.NotNull(rasterImageData.Size);
                Assert.NotNull(rasterImageData.Bounds);
                Assert.NotNull(rasterImageData.Centroid);
                Assert.NotNull(rasterImageData.CornerPoints);
                Assert.Equal(4, rasterImageData.CornerPoints.Count);
            }
        }

        [Fact]
        public void SerializeRasterImageDataToJson_Success()
        {
            var rasterImage = CreateBasicRasterImage();

            var rasterImageData = RasterImageEntityRenderer.Render(rasterImage);
            var json = JsonConvert.SerializeObject(rasterImageData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("RasterImage", json);
            Assert.Contains("InsertPoint", json);
            Assert.Contains("UVector", json);
            Assert.Contains("VVector", json);
            Assert.Contains("Size", json);
            Assert.Contains("Bounds", json);
            Assert.Contains("Centroid", json);
            Assert.Contains("CornerPoints", json);
        }

        [Fact]
        public void DeserializeRasterImageDataFromJson_Success()
        {
            var rasterImage = CreateBasicRasterImage();

            var originalRasterImageData = RasterImageEntityRenderer.Render(rasterImage);
            var json = JsonConvert.SerializeObject(originalRasterImageData);
            var deserializedRasterImageData = JsonConvert.DeserializeObject<RasterImageEntityRenderer.RasterImageData>(json);

            Assert.NotNull(deserializedRasterImageData);
            Assert.Equal(originalRasterImageData.InsertPoint.X, deserializedRasterImageData.InsertPoint.X);
            Assert.Equal(originalRasterImageData.InsertPoint.Y, deserializedRasterImageData.InsertPoint.Y);
            Assert.Equal(originalRasterImageData.InsertPoint.Z, deserializedRasterImageData.InsertPoint.Z);
            Assert.Equal(originalRasterImageData.UVector.X, deserializedRasterImageData.UVector.X);
            Assert.Equal(originalRasterImageData.UVector.Y, deserializedRasterImageData.UVector.Y);
            Assert.Equal(originalRasterImageData.UVector.Z, deserializedRasterImageData.UVector.Z);
            Assert.Equal(originalRasterImageData.VVector.X, deserializedRasterImageData.VVector.X);
            Assert.Equal(originalRasterImageData.VVector.Y, deserializedRasterImageData.VVector.Y);
            Assert.Equal(originalRasterImageData.VVector.Z, deserializedRasterImageData.VVector.Z);
            Assert.Equal(originalRasterImageData.Size.X, deserializedRasterImageData.Size.X);
            Assert.Equal(originalRasterImageData.Size.Y, deserializedRasterImageData.Size.Y);
            Assert.Equal(originalRasterImageData.ColorIndex, deserializedRasterImageData.ColorIndex);
            Assert.Equal(originalRasterImageData.Brightness, deserializedRasterImageData.Brightness);
            Assert.Equal(originalRasterImageData.Contrast, deserializedRasterImageData.Contrast);
            Assert.Equal(originalRasterImageData.Fade, deserializedRasterImageData.Fade);
            Assert.Equal(originalRasterImageData.Flags, deserializedRasterImageData.Flags);
            Assert.Equal(originalRasterImageData.ClipType, deserializedRasterImageData.ClipType);
            Assert.Equal(originalRasterImageData.ClippingState, deserializedRasterImageData.ClippingState);
            Assert.Equal(originalRasterImageData.Area, deserializedRasterImageData.Area);
        }

        [Fact]
        public void RenderRasterImageWithDifferentProperties_Success()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.InsertPoint = new XYZ(50, 100, 20);
            rasterImage.UVector = new XYZ(Math.Cos(Math.PI / 4), Math.Sin(Math.PI / 4), 0);
            rasterImage.VVector = new XYZ(-Math.Sin(Math.PI / 4), Math.Cos(Math.PI / 4), 0);
            rasterImage.Size = new XY(200, 150);
            rasterImage.Color = new Color(3);
            rasterImage.Brightness = 75;
            rasterImage.Contrast = 80;
            rasterImage.Fade = 100;
            rasterImage.Flags = ImageDisplayFlags.ShowImage | ImageDisplayFlags.ShowClipping;
            rasterImage.ClipType = ClipType.Polygonal;
            rasterImage.ClippingState = true;

            var rasterImageData = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(rasterImageData);
            Assert.Equal(50, rasterImageData.InsertPoint.X);
            Assert.Equal(100, rasterImageData.InsertPoint.Y);
            Assert.Equal(20, rasterImageData.InsertPoint.Z);
            Assert.Equal(200, rasterImageData.Size.X);
            Assert.Equal(150, rasterImageData.Size.Y);
            Assert.Equal(3, rasterImageData.ColorIndex);
            Assert.Equal(75, rasterImageData.Brightness);
            Assert.Equal(80, rasterImageData.Contrast);
            Assert.Equal(100, rasterImageData.Fade);
            Assert.Equal(ImageDisplayFlags.ShowImage | ImageDisplayFlags.ShowClipping, rasterImageData.Flags);
            Assert.Equal(ClipType.Polygonal, rasterImageData.ClipType);
            Assert.True(rasterImageData.ClippingState);
        }

        [Fact]
        public void RenderRasterImageWithZeroSize_ReturnsZeroArea()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.Size = new XY(0, 0);

            var rasterImageData = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(rasterImageData);
            Assert.Equal(0, rasterImageData.Area);
        }

        [Fact]
        public void RenderRasterImageWithLargeSize_ReturnsCorrectArea()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.Size = new XY(1000, 500);

            var rasterImageData = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(rasterImageData);
            Assert.Equal(500000, rasterImageData.Area);
        }

        [Fact]
        public void RenderRasterImageWithRotation_ReturnsCorrectTransform()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.UVector = new XYZ(Math.Cos(Math.PI / 6), Math.Sin(Math.PI / 6), 0);
            rasterImage.VVector = new XYZ(-Math.Sin(Math.PI / 6), Math.Cos(Math.PI / 6), 0);

            var rasterImageData = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(rasterImageData);
            Assert.NotNull(rasterImageData.Transform);
            Assert.Equal(Math.PI / 6, rasterImageData.Transform.Rotation.Z, 3);
        }

        [Fact]
        public void SerializeAndDeserializeRasterImageDataWithAllProperties_Success()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.InsertPoint = new XYZ(100, 200, 50);
            rasterImage.UVector = new XYZ(0.707, 0.707, 0);
            rasterImage.VVector = new XYZ(-0.707, 0.707, 0);
            rasterImage.Size = new XY(300, 200);
            rasterImage.Color = new Color(5);
            rasterImage.Brightness = 60;
            rasterImage.Contrast = 70;
            rasterImage.Fade = 50;

            var originalRasterImageData = RasterImageEntityRenderer.Render(rasterImage);
            var json = JsonConvert.SerializeObject(originalRasterImageData, Formatting.Indented);
            var deserializedRasterImageData = JsonConvert.DeserializeObject<RasterImageEntityRenderer.RasterImageData>(json);

            Assert.NotNull(deserializedRasterImageData);
            Assert.Equal(originalRasterImageData.InsertPoint.X, deserializedRasterImageData.InsertPoint.X);
            Assert.Equal(originalRasterImageData.InsertPoint.Y, deserializedRasterImageData.InsertPoint.Y);
            Assert.Equal(originalRasterImageData.InsertPoint.Z, deserializedRasterImageData.InsertPoint.Z);
            Assert.Equal(originalRasterImageData.UVector.X, deserializedRasterImageData.UVector.X, 3);
            Assert.Equal(originalRasterImageData.UVector.Y, deserializedRasterImageData.UVector.Y, 3);
            Assert.Equal(originalRasterImageData.VVector.X, deserializedRasterImageData.VVector.X, 3);
            Assert.Equal(originalRasterImageData.VVector.Y, deserializedRasterImageData.VVector.Y, 3);
            Assert.Equal(originalRasterImageData.Size.X, deserializedRasterImageData.Size.X);
            Assert.Equal(originalRasterImageData.Size.Y, deserializedRasterImageData.Size.Y);
            Assert.Equal(originalRasterImageData.ColorIndex, deserializedRasterImageData.ColorIndex);
            Assert.Equal(originalRasterImageData.Brightness, deserializedRasterImageData.Brightness);
            Assert.Equal(originalRasterImageData.Contrast, deserializedRasterImageData.Contrast);
            Assert.Equal(originalRasterImageData.Fade, deserializedRasterImageData.Fade);
            Assert.Equal(originalRasterImageData.Area, deserializedRasterImageData.Area);
        }

        [Fact]
        public void RenderRasterImageWithDifferentFadeValues_ReturnsCorrectOpacity()
        {
            var rasterImage = CreateBasicRasterImage();
            
            rasterImage.Fade = 0;
            var rasterImageData1 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.Equal(0, rasterImageData1.Material.Opacity);
            
            rasterImage.Fade = 127;
            var rasterImageData2 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.Equal(127 / 255.0, rasterImageData2.Material.Opacity, 3);
            
            rasterImage.Fade = 255;
            var rasterImageData3 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.Equal(1.0, rasterImageData3.Material.Opacity);
        }

        [Fact]
        public void RenderRasterImageWithDifferentColorIndices_ReturnsCorrectColors()
        {
            var rasterImage = CreateBasicRasterImage();
            
            rasterImage.Color = new Color(1);
            var rasterImageData1 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.Equal(0xff0000, rasterImageData1.Material.Color);
            
            rasterImage.Color = new Color(3);
            var rasterImageData2 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.Equal(0x00ff00, rasterImageData2.Material.Color);
            
            rasterImage.Color = new Color(5);
            var rasterImageData3 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.Equal(0x0000ff, rasterImageData3.Material.Color);
        }

        [Fact]
        public void RenderRasterImageWithDifferentClipTypes_ReturnsCorrectClipType()
        {
            var rasterImage = CreateBasicRasterImage();
            
            rasterImage.ClipType = ClipType.Rectangular;
            var rasterImageData1 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.Equal(ClipType.Rectangular, rasterImageData1.ClipType);
            
            rasterImage.ClipType = ClipType.Polygonal;
            var rasterImageData2 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.Equal(ClipType.Polygonal, rasterImageData2.ClipType);
        }

        [Fact]
        public void RenderRasterImageWithClippingState_ReturnsCorrectState()
        {
            var rasterImage = CreateBasicRasterImage();
            
            rasterImage.ClippingState = false;
            var rasterImageData1 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.False(rasterImageData1.ClippingState);
            
            rasterImage.ClippingState = true;
            var rasterImageData2 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.True(rasterImageData2.ClippingState);
        }

        [Fact]
        public void RenderRasterImageWithDifferentDisplayFlags_ReturnsCorrectFlags()
        {
            var rasterImage = CreateBasicRasterImage();
            
            rasterImage.Flags = ImageDisplayFlags.ShowImage;
            var rasterImageData1 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.Equal(ImageDisplayFlags.ShowImage, rasterImageData1.Flags);
            
            rasterImage.Flags = ImageDisplayFlags.ShowImage | ImageDisplayFlags.ShowClipping;
            var rasterImageData2 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.Equal(ImageDisplayFlags.ShowImage | ImageDisplayFlags.ShowClipping, rasterImageData2.Flags);
        }

        [Fact]
        public void RenderRasterImageWithDifferentInsertPoints_ReturnsCorrectCentroid()
        {
            var rasterImage = CreateBasicRasterImage();
            
            rasterImage.InsertPoint = new XYZ(0, 0, 0);
            rasterImage.Size = new XY(100, 100);
            var rasterImageData1 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.Equal(50, rasterImageData1.Centroid.X);
            Assert.Equal(50, rasterImageData1.Centroid.Y);
            
            rasterImage.InsertPoint = new XYZ(100, 200, 0);
            rasterImage.Size = new XY(200, 150);
            var rasterImageData2 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.Equal(200, rasterImageData2.Centroid.X);
            Assert.Equal(275, rasterImageData2.Centroid.Y);
        }

        [Fact]
        public void RenderRasterImageWithDifferentSizes_ReturnsCorrectCornerPoints()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.InsertPoint = new XYZ(0, 0, 0);
            rasterImage.Size = new XY(100, 100);
            
            var rasterImageData = RasterImageEntityRenderer.Render(rasterImage);
            
            Assert.NotNull(rasterImageData.CornerPoints);
            Assert.Equal(4, rasterImageData.CornerPoints.Count);
            Assert.Equal(0, rasterImageData.CornerPoints[0].X);
            Assert.Equal(0, rasterImageData.CornerPoints[0].Y);
            Assert.Equal(100, rasterImageData.CornerPoints[1].X);
            Assert.Equal(0, rasterImageData.CornerPoints[1].Y);
            Assert.Equal(100, rasterImageData.CornerPoints[2].X);
            Assert.Equal(100, rasterImageData.CornerPoints[2].Y);
            Assert.Equal(0, rasterImageData.CornerPoints[3].X);
            Assert.Equal(100, rasterImageData.CornerPoints[3].Y);
        }

        [Fact]
        public void RenderRasterImageWithDifferentUVVectors_ReturnsCorrectRotation()
        {
            var rasterImage = CreateBasicRasterImage();
            
            rasterImage.UVector = new XYZ(1, 0, 0);
            rasterImage.VVector = new XYZ(0, 1, 0);
            var rasterImageData1 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.Equal(0, rasterImageData1.Transform.Rotation.Z, 3);
            
            rasterImage.UVector = new XYZ(0, 1, 0);
            rasterImage.VVector = new XYZ(-1, 0, 0);
            var rasterImageData2 = RasterImageEntityRenderer.Render(rasterImage);
            Assert.Equal(Math.PI / 2, rasterImageData2.Transform.Rotation.Z, 3);
        }

        [Fact]
        public void RenderRasterImageWithVertexPositions_ReturnsCorrectArray()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.InsertPoint = new XYZ(0, 0, 0);
            rasterImage.Size = new XY(100, 100);
            
            var rasterImageData = RasterImageEntityRenderer.Render(rasterImage);
            
            Assert.NotNull(rasterImageData.VertexPositions);
            Assert.Equal(12, rasterImageData.VertexPositions.Length);
        }

        [Fact]
        public void RenderRasterImageWithUVCoordinates_ReturnsCorrectArray()
        {
            var rasterImage = CreateBasicRasterImage();
            
            var rasterImageData = RasterImageEntityRenderer.Render(rasterImage);
            
            Assert.NotNull(rasterImageData.UVCoordinates);
            Assert.Equal(8, rasterImageData.UVCoordinates.Length);
            Assert.Equal(0, rasterImageData.UVCoordinates[0]);
            Assert.Equal(0, rasterImageData.UVCoordinates[1]);
            Assert.Equal(1, rasterImageData.UVCoordinates[2]);
            Assert.Equal(0, rasterImageData.UVCoordinates[3]);
            Assert.Equal(1, rasterImageData.UVCoordinates[4]);
            Assert.Equal(1, rasterImageData.UVCoordinates[5]);
            Assert.Equal(0, rasterImageData.UVCoordinates[6]);
            Assert.Equal(1, rasterImageData.UVCoordinates[7]);
        }

        [Fact]
        public void RenderRasterImageWithIndices_ReturnsCorrectArray()
        {
            var rasterImage = CreateBasicRasterImage();
            
            var rasterImageData = RasterImageEntityRenderer.Render(rasterImage);
            
            Assert.NotNull(rasterImageData.Indices);
            Assert.Equal(6, rasterImageData.Indices.Length);
            Assert.Equal(0, rasterImageData.Indices[0]);
            Assert.Equal(1, rasterImageData.Indices[1]);
            Assert.Equal(2, rasterImageData.Indices[2]);
            Assert.Equal(0, rasterImageData.Indices[3]);
            Assert.Equal(2, rasterImageData.Indices[4]);
            Assert.Equal(3, rasterImageData.Indices[5]);
        }
    }
}
