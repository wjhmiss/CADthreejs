using System;
using Xunit;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using DxfDwgViewer.RenderUtilities;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class RasterImageEntityRendererTests
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
        public void Render_RasterImage_ReturnsCorrectType()
        {
            var rasterImage = CreateBasicRasterImage();
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.Equal("RasterImage", result.EntityType);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectInsertPoint()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.InsertPoint = new XYZ(50, 100, 20);
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.Equal(50, result.InsertPoint.X);
            Assert.Equal(100, result.InsertPoint.Y);
            Assert.Equal(20, result.InsertPoint.Z);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectUVector()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.UVector = new XYZ(1, 0, 0);
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.Equal(1, result.UVector.X);
            Assert.Equal(0, result.UVector.Y);
            Assert.Equal(0, result.UVector.Z);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectVVector()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.VVector = new XYZ(0, 1, 0);
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.Equal(0, result.VVector.X);
            Assert.Equal(1, result.VVector.Y);
            Assert.Equal(0, result.VVector.Z);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectSize()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.Size = new XY(200, 150);
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.Equal(200, result.Size.X);
            Assert.Equal(150, result.Size.Y);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectColorIndex()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.Color = new Color(3);
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.Equal(3, result.ColorIndex);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectBrightness()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.Brightness = 75;
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.Equal(75, result.Brightness);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectContrast()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.Contrast = 80;
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.Equal(80, result.Contrast);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectFade()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.Fade = 100;
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.Equal(100, result.Fade);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectFlags()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.Flags = ImageDisplayFlags.ShowImage | ImageDisplayFlags.ShowClipping;
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.Equal(ImageDisplayFlags.ShowImage | ImageDisplayFlags.ShowClipping, result.Flags);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectClipType()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.ClipType = ClipType.Polygonal;
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.Equal(ClipType.Polygonal, result.ClipType);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectClippingState()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.ClippingState = true;
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.True(result.ClippingState);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectCornerPoints()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.InsertPoint = new XYZ(0, 0, 0);
            rasterImage.UVector = new XYZ(1, 0, 0);
            rasterImage.VVector = new XYZ(0, 1, 0);
            rasterImage.Size = new XY(100, 100);
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.NotNull(result.CornerPoints);
            Assert.Equal(4, result.CornerPoints.Count);
            
            Assert.Equal(0, result.CornerPoints[0].X);
            Assert.Equal(0, result.CornerPoints[0].Y);
            
            Assert.Equal(100, result.CornerPoints[1].X);
            Assert.Equal(0, result.CornerPoints[1].Y);
            
            Assert.Equal(100, result.CornerPoints[2].X);
            Assert.Equal(100, result.CornerPoints[2].Y);
            
            Assert.Equal(0, result.CornerPoints[3].X);
            Assert.Equal(100, result.CornerPoints[3].Y);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectBounds()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.InsertPoint = new XYZ(0, 0, 0);
            rasterImage.UVector = new XYZ(1, 0, 0);
            rasterImage.VVector = new XYZ(0, 1, 0);
            rasterImage.Size = new XY(100, 100);
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(100, result.Bounds.Max.X);
            Assert.Equal(100, result.Bounds.Max.Y);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectCentroid()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.InsertPoint = new XYZ(0, 0, 0);
            rasterImage.UVector = new XYZ(1, 0, 0);
            rasterImage.VVector = new XYZ(0, 1, 0);
            rasterImage.Size = new XY(100, 100);
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);
            Assert.Equal(50, result.Centroid.X);
            Assert.Equal(50, result.Centroid.Y);
            Assert.Equal(0, result.Centroid.Z);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectArea()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.InsertPoint = new XYZ(0, 0, 0);
            rasterImage.UVector = new XYZ(1, 0, 0);
            rasterImage.VVector = new XYZ(0, 1, 0);
            rasterImage.Size = new XY(100, 100);
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.Equal(10000, result.Area);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectTransform()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.InsertPoint = new XYZ(50, 50, 0);
            rasterImage.UVector = new XYZ(1, 0, 0);
            rasterImage.VVector = new XYZ(0, 1, 0);
            rasterImage.Size = new XY(100, 100);
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.Equal(50, result.Transform.Position.X);
            Assert.Equal(50, result.Transform.Position.Y);
            Assert.Equal(0, result.Transform.Position.Z);
            Assert.Equal(0, result.Transform.Rotation.X);
            Assert.Equal(0, result.Transform.Rotation.Y);
            Assert.Equal(0, result.Transform.Rotation.Z);
            Assert.Equal(100, result.Transform.Scale.X);
            Assert.Equal(100, result.Transform.Scale.Y);
            Assert.Equal(1, result.Transform.Scale.Z);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectGeometry()
        {
            var rasterImage = CreateBasicRasterImage();
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);
            Assert.Equal("BufferGeometry", result.Geometry.Type);
            Assert.Equal(4, result.Geometry.VertexCount);
            Assert.Equal(2, result.Geometry.FaceCount);
            Assert.False(result.Geometry.HasNormals);
            Assert.False(result.Geometry.HasColors);
            Assert.True(result.Geometry.HasUVs);
            Assert.True(result.Geometry.HasIndices);
            Assert.Equal("Triangles", result.Geometry.PrimitiveType);
            Assert.Equal(6, result.Geometry.IndexCount);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectMaterial()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.Color = new Color(1);
            rasterImage.Fade = 100;
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);
            Assert.Equal("MeshBasicMaterial", result.Material.Type);
            Assert.Equal(0xff0000, result.Material.Color);
            Assert.Equal(100 / 255.0, result.Material.Opacity);
            Assert.True(result.Material.Transparent);
            Assert.True(result.Material.Side);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectTexture()
        {
            var rasterImage = CreateBasicRasterImage();
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.NotNull(result.Texture);
            Assert.Equal("", result.Texture.ImagePath);
            Assert.Equal(0, result.Texture.Offset.X);
            Assert.Equal(0, result.Texture.Offset.Y);
            Assert.Equal(1, result.Texture.Repeat.X);
            Assert.Equal(1, result.Texture.Repeat.Y);
            Assert.True(result.Texture.FlipY);
            Assert.Equal(0, result.Texture.Rotation);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectVertexPositions()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.InsertPoint = new XYZ(0, 0, 0);
            rasterImage.UVector = new XYZ(1, 0, 0);
            rasterImage.VVector = new XYZ(0, 1, 0);
            rasterImage.Size = new XY(100, 100);
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexPositions);
            Assert.Equal(12, result.VertexPositions.Length);
            
            Assert.Equal(0, result.VertexPositions[0]);
            Assert.Equal(0, result.VertexPositions[1]);
            Assert.Equal(0, result.VertexPositions[2]);
            
            Assert.Equal(100, result.VertexPositions[3]);
            Assert.Equal(0, result.VertexPositions[4]);
            Assert.Equal(0, result.VertexPositions[5]);
            
            Assert.Equal(100, result.VertexPositions[6]);
            Assert.Equal(100, result.VertexPositions[7]);
            Assert.Equal(0, result.VertexPositions[8]);
            
            Assert.Equal(0, result.VertexPositions[9]);
            Assert.Equal(100, result.VertexPositions[10]);
            Assert.Equal(0, result.VertexPositions[11]);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectUVCoordinates()
        {
            var rasterImage = CreateBasicRasterImage();
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.NotNull(result.UVCoordinates);
            Assert.Equal(8, result.UVCoordinates.Length);
            
            Assert.Equal(0, result.UVCoordinates[0]);
            Assert.Equal(0, result.UVCoordinates[1]);
            
            Assert.Equal(1, result.UVCoordinates[2]);
            Assert.Equal(0, result.UVCoordinates[3]);
            
            Assert.Equal(1, result.UVCoordinates[4]);
            Assert.Equal(1, result.UVCoordinates[5]);
            
            Assert.Equal(0, result.UVCoordinates[6]);
            Assert.Equal(1, result.UVCoordinates[7]);
        }

        [Fact]
        public void Render_RasterImage_ReturnsCorrectIndices()
        {
            var rasterImage = CreateBasicRasterImage();
            var result = RasterImageEntityRenderer.Render(rasterImage);

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
        public void Render_RasterImage_WithRotation_ReturnsCorrectTransform()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.InsertPoint = new XYZ(0, 0, 0);
            rasterImage.UVector = new XYZ(Math.Cos(Math.PI / 4), Math.Sin(Math.PI / 4), 0);
            rasterImage.VVector = new XYZ(-Math.Sin(Math.PI / 4), Math.Cos(Math.PI / 4), 0);
            rasterImage.Size = new XY(100, 100);
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.Equal(0, result.Transform.Position.X);
            Assert.Equal(0, result.Transform.Position.Y);
            Assert.Equal(0, result.Transform.Position.Z);
            Assert.Equal(0, result.Transform.Rotation.X);
            Assert.Equal(0, result.Transform.Rotation.Y);
            Assert.Equal(Math.PI / 4, result.Transform.Rotation.Z);
        }

        [Fact]
        public void Render_RasterImage_WithDifferentSize_ReturnsCorrectArea()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.InsertPoint = new XYZ(0, 0, 0);
            rasterImage.UVector = new XYZ(1, 0, 0);
            rasterImage.VVector = new XYZ(0, 1, 0);
            rasterImage.Size = new XY(200, 150);
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.Equal(30000, result.Area);
        }

        [Fact]
        public void Render_RasterImage_WithZeroFade_ReturnsOpaqueMaterial()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.Fade = 0;
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);
            Assert.Equal(0, result.Material.Opacity);
            Assert.True(result.Material.Transparent);
        }

        [Fact]
        public void Render_RasterImage_WithMaxFade_ReturnsTransparentMaterial()
        {
            var rasterImage = CreateBasicRasterImage();
            rasterImage.Fade = 255;
            var result = RasterImageEntityRenderer.Render(rasterImage);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);
            Assert.Equal(1.0, result.Material.Opacity);
            Assert.False(result.Material.Transparent);
        }
    }
}
