using System;
using Xunit;
using ACadSharp.Entities;
using ACadSharp;
using CSMath;
using DxfDwgViewer.RenderUtilities;

namespace DxfDwgViewer.Tests
{
    public class WipeoutEntityRendererTests
    {
        private Wipeout CreateBasicWipeout()
        {
            var wipeout = new Wipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.UVector = new XYZ(1, 0, 0);
            wipeout.VVector = new XYZ(0, 1, 0);
            wipeout.Size = new XY(10, 10);
            wipeout.ClipType = ClipType.Rectangular;
            wipeout.Color = new Color(256);
            wipeout.Brightness = 50;
            wipeout.Contrast = 50;
            wipeout.Fade = 0;
            wipeout.Flags = ImageDisplayFlags.ShowImage;
            wipeout.ClippingState = true;
            return wipeout;
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectInsertPoint()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(100, 200, 50);
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(100, result.InsertPoint.X);
            Assert.Equal(200, result.InsertPoint.Y);
            Assert.Equal(50, result.InsertPoint.Z);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectUVector()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.UVector = new XYZ(2, 0, 0);
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(2, result.UVector.X);
            Assert.Equal(0, result.UVector.Y);
            Assert.Equal(0, result.UVector.Z);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectVVector()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.VVector = new XYZ(0, 3, 0);
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(0, result.VVector.X);
            Assert.Equal(3, result.VVector.Y);
            Assert.Equal(0, result.VVector.Z);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectSize()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Size = new XY(20, 30);
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(20, result.Size.X);
            Assert.Equal(30, result.Size.Y);
            Assert.Equal(0, result.Size.Z);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectClipType()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.ClipType = ClipType.Polygonal;
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(ClipType.Polygonal, result.ClipType);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectColorIndex()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Color = new Color(1);
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(1, result.ColorIndex);
        }

        [Fact]
        public void Render_Wipeout_CreatesColorCorrectly()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Color = new Color(1);
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);

            Assert.Equal(1, result.Color.Index);
            Assert.Equal("#FF0000", result.Color.Hex);
            Assert.Equal((byte)255, result.Color.R);
            Assert.Equal((byte)0, result.Color.G);
            Assert.Equal((byte)0, result.Color.B);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectBrightness()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Brightness = 75;
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(75, result.Brightness);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectContrast()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Contrast = 80;
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(80, result.Contrast);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectFade()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Fade = 30;
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(30, result.Fade);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectFlags()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Flags = ImageDisplayFlags.ShowImage | ImageDisplayFlags.UseClipping;
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(ImageDisplayFlags.ShowImage | ImageDisplayFlags.UseClipping, result.Flags);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectClippingState()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.ClippingState = false;
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.False(result.ClippingState);
        }

        [Fact]
        public void Render_Wipeout_RectangularClipType_CreatesCorrectBoundaryPoints()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.UVector = new XYZ(1, 0, 0);
            wipeout.VVector = new XYZ(0, 1, 0);
            wipeout.Size = new XY(10, 10);
            wipeout.ClipType = ClipType.Rectangular;
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.BoundaryPoints);
            Assert.Equal(4, result.BoundaryPoints.Count);
        }

        [Fact]
        public void Render_Wipeout_RectangularClipType_CalculatesCorrectBounds()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.UVector = new XYZ(1, 0, 0);
            wipeout.VVector = new XYZ(0, 1, 0);
            wipeout.Size = new XY(10, 10);
            wipeout.ClipType = ClipType.Rectangular;
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);
            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(10, result.Bounds.Max.X);
            Assert.Equal(10, result.Bounds.Max.Y);
        }

        [Fact]
        public void Render_Wipeout_RectangularClipType_CalculatesCorrectCentroid()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.UVector = new XYZ(1, 0, 0);
            wipeout.VVector = new XYZ(0, 1, 0);
            wipeout.Size = new XY(10, 10);
            wipeout.ClipType = ClipType.Rectangular;
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);
            Assert.Equal(5, result.Centroid.X);
            Assert.Equal(5, result.Centroid.Y);
            Assert.Equal(0, result.Centroid.Z);
        }

        [Fact]
        public void Render_Wipeout_RectangularClipType_CalculatesCorrectArea()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.UVector = new XYZ(1, 0, 0);
            wipeout.VVector = new XYZ(0, 1, 0);
            wipeout.Size = new XY(10, 10);
            wipeout.ClipType = ClipType.Rectangular;
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(100, result.Area);
        }

        [Fact]
        public void Render_Wipeout_PolygonalClipType_CreatesCorrectBoundaryPoints()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.ClipType = ClipType.Polygonal;
            wipeout.ClipBoundaryVertices = new System.Collections.Generic.List<XY>
            {
                new XY(0, 0),
                new XY(10, 0),
                new XY(10, 10),
                new XY(0, 10)
            };
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.BoundaryPoints);
            Assert.Equal(4, result.BoundaryPoints.Count);
        }

        [Fact]
        public void Render_Wipeout_PolygonalClipType_CalculatesCorrectBounds()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.ClipType = ClipType.Polygonal;
            wipeout.ClipBoundaryVertices = new System.Collections.Generic.List<XY>
            {
                new XY(0, 0),
                new XY(10, 0),
                new XY(10, 10),
                new XY(0, 10)
            };
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(10, result.Bounds.Max.X);
            Assert.Equal(10, result.Bounds.Max.Y);
        }

        [Fact]
        public void Render_Wipeout_PolygonalClipType_CalculatesCorrectCentroid()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.ClipType = ClipType.Polygonal;
            wipeout.ClipBoundaryVertices = new System.Collections.Generic.List<XY>
            {
                new XY(0, 0),
                new XY(10, 0),
                new XY(10, 10),
                new XY(0, 10)
            };
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);
            Assert.Equal(5, result.Centroid.X);
            Assert.Equal(5, result.Centroid.Y);
        }

        [Fact]
        public void Render_Wipeout_PolygonalClipType_CalculatesCorrectArea()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.ClipType = ClipType.Polygonal;
            wipeout.ClipBoundaryVertices = new System.Collections.Generic.List<XY>
            {
                new XY(0, 0),
                new XY(10, 0),
                new XY(10, 10),
                new XY(0, 10)
            };
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(100, result.Area);
        }

        [Fact]
        public void Render_Wipeout_DefaultClipType_CreatesDefaultBoundaryPoints()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.ClipType = (ClipType)99;
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.BoundaryPoints);
            Assert.Equal(4, result.BoundaryPoints.Count);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectBoundaryPointCount()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.ClipType = ClipType.Rectangular;
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(4, result.BoundaryPointCount);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectTransform()
        {
            var wipeout = CreateBasicWipeout();
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Position);
            Assert.NotNull(result.Transform.Rotation);
            Assert.NotNull(result.Transform.Scale);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectGeometry()
        {
            var wipeout = CreateBasicWipeout();
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);
            Assert.Equal("PolygonGeometry", result.Geometry.Type);
            Assert.Equal(4, result.Geometry.VertexCount);
            Assert.True(result.Geometry.HasNormals);
            Assert.True(result.Geometry.HasColors);
            Assert.True(result.Geometry.HasUVs);
            Assert.True(result.Geometry.HasIndices);
            Assert.Equal("Triangles", result.Geometry.PrimitiveType);
            Assert.True(result.Geometry.IsClosed);
            Assert.False(result.Geometry.IsPeriodic);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectMaterial()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Color = new Color(1);
            var result = WipeoutEntityRenderer.Render(wipeout);

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
        public void Render_Wipeout_ReturnsCorrectVertexPositions()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.ClipType = ClipType.Rectangular;
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexPositions);
            Assert.Equal(12, result.VertexPositions.Length);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectVertexColors()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Color = new Color(1);
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexColors);
            Assert.Equal(12, result.VertexColors.Length);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectVertexNormals()
        {
            var wipeout = CreateBasicWipeout();
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexNormals);
            Assert.Equal(12, result.VertexNormals.Length);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectUV()
        {
            var wipeout = CreateBasicWipeout();
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.UV);
            Assert.Equal(8, result.UV.Length);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectIndices()
        {
            var wipeout = CreateBasicWipeout();
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.Indices);
            Assert.Equal(6, result.Indices.Length);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectVertexCount()
        {
            var wipeout = CreateBasicWipeout();
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(4, result.VertexCount);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectOpacity()
        {
            var wipeout = CreateBasicWipeout();
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(1.0, result.Opacity);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectTransparent()
        {
            var wipeout = CreateBasicWipeout();
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.False(result.Transparent);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectIsMask()
        {
            var wipeout = CreateBasicWipeout();
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.True(result.IsMask);
        }

        [Fact]
        public void Render_Wipeout_ReturnsCorrectNormal()
        {
            var wipeout = CreateBasicWipeout();
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.Normal);
            Assert.Equal(0, result.Normal.X);
            Assert.Equal(0, result.Normal.Y);
            Assert.Equal(0, result.Normal.Z);
        }

        [Fact]
        public void Render_Wipeout_PolygonalWithMorePoints_CalculatesCorrectArea()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.ClipType = ClipType.Polygonal;
            wipeout.ClipBoundaryVertices = new System.Collections.Generic.List<XY>
            {
                new XY(0, 0),
                new XY(20, 0),
                new XY(20, 10),
                new XY(10, 15),
                new XY(0, 10)
            };
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(5, result.BoundaryPoints.Count);
            Assert.True(result.Area > 0);
        }

        [Fact]
        public void Render_Wipeout_WithRotatedUVectors_CalculatesCorrectBoundary()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.InsertPoint = new XYZ(0, 0, 0);
            wipeout.UVector = new XYZ(0.707, 0.707, 0);
            wipeout.VVector = new XYZ(-0.707, 0.707, 0);
            wipeout.Size = new XY(10, 10);
            wipeout.ClipType = ClipType.Rectangular;
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.NotNull(result.BoundaryPoints);
            Assert.Equal(4, result.BoundaryPoints.Count);
        }

        [Fact]
        public void Render_Wipeout_WithDifferentColorIndex_CreatesCorrectColor()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Color = new Color(3);
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(3, result.Color.Index);
            Assert.Equal("#00FF00", result.Color.Hex);
            Assert.Equal((byte)0, result.Color.R);
            Assert.Equal((byte)255, result.Color.G);
            Assert.Equal((byte)0, result.Color.B);
        }

        [Fact]
        public void Render_Wipeout_WithByLayerColor_CreatesCorrectColor()
        {
            var wipeout = CreateBasicWipeout();
            wipeout.Color = new Color(256);
            var result = WipeoutEntityRenderer.Render(wipeout);

            Assert.NotNull(result);
            Assert.Equal(256, result.Color.Index);
            Assert.Equal("#B3B3B3", result.Color.Hex);
            Assert.Equal((byte)179, result.Color.R);
            Assert.Equal((byte)179, result.Color.G);
            Assert.Equal((byte)179, result.Color.B);
        }
    }
}
