using System;
using Xunit;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using ACadSharp.Objects;
using DxfDwgViewer.RenderUtilities;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class PdfUnderlayEntityRendererTests
    {
        private PdfUnderlay CreateBasicPdfUnderlay()
        {
            var definition = new PdfUnderlayDefinition
            {
                File = "test.pdf",
                Page = "1"
            };
            var pdfUnderlay = new PdfUnderlay(definition)
            {
                InsertPoint = new XYZ(10, 10, 0),
                XScale = 1.0,
                YScale = 1.0,
                ZScale = 1.0,
                Rotation = 0,
                Contrast = 50,
                Fade = 0
            };
            pdfUnderlay.Color = new Color(7);
            pdfUnderlay.LineType = new LineType("CONTINUOUS");
            pdfUnderlay.LineWeight = LineWeightType.ByLayer;
            pdfUnderlay.IsInvisible = false;
            return pdfUnderlay;
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectType()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal("Underlay", result.Type);
            Assert.Equal("PdfUnderlay", result.EntityType);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectInsertPoint()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.InsertPoint = new XYZ(100, 200, 50);
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(100, result.InsertPoint.X);
            Assert.Equal(200, result.InsertPoint.Y);
            Assert.Equal(50, result.InsertPoint3D.Z);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectScale()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.XScale = 2.0;
            pdfUnderlay.YScale = 3.0;
            pdfUnderlay.ZScale = 4.0;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(2.0, result.XScale);
            Assert.Equal(3.0, result.YScale);
            Assert.Equal(4.0, result.ZScale);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectRotation()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Rotation = Math.PI / 4;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 4, result.Rotation);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectColorIndex()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Color = new Color(5);
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectContrast()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Contrast = 75;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(75, result.Contrast);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectFade()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Fade = 30;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(30, result.Fade);
        }

        [Fact]
        public void Render_PdfUnderlay_CalculatesCorrectOpacity()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Fade = 40;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(0.6, result.Opacity);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectTransparentFlag()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Fade = 20;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.True(result.Transparent);
        }

        [Fact]
        public void Render_PdfUnderlay_WithZeroFade_ReturnsNotTransparent()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Fade = 0;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.False(result.Transparent);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectVisibleFlag()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.IsInvisible = false;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.True(result.Visible);
        }

        [Fact]
        public void Render_PdfUnderlay_WithInvisible_ReturnsNotVisible()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.IsInvisible = true;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.False(result.Visible);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectCoordinateSystem()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal("WCS", result.CoordinateSystem);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectHandle()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.NotNull(result.Handle);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectMaterialType()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);
            Assert.Equal("UnderlayMaterial", result.Material.Type);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectGeometryType()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);
            Assert.Equal("UnderlayGeometry", result.Geometry.Type);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectDepthTest()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.True(result.DepthTest);
            Assert.True(result.Material.DepthTest);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectSide()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.True(result.Material.Side);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectNormal()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Normal = new XYZ(0, 0, 1);
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(0, result.Normal.X);
            Assert.Equal(0, result.Normal.Y);
            Assert.Equal(1, result.Normal.Z);
        }

        [Fact]
        public void Render_PdfUnderlay_WithNullLayer_ReturnsEmptyLayerName()
        {
            var definition = new PdfUnderlayDefinition
            {
                File = "test.pdf",
                Page = "1"
            };
            var pdfUnderlay = new PdfUnderlay(definition);
            pdfUnderlay.Color = new ACadSharp.Color(7);
            pdfUnderlay.LineType = new LineType("CONTINUOUS");
            pdfUnderlay.LineWeight = LineWeightType.ByLayer;
            pdfUnderlay.IsInvisible = false;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.NotNull(result.LayerName);
        }

        [Fact]
        public void Render_PdfUnderlay_WithNullDefinition_ReturnsEmptyFilePath()
        {
            var definition = new PdfUnderlayDefinition
            {
                File = null,
                Page = "1"
            };
            var pdfUnderlay = new PdfUnderlay(definition);
            pdfUnderlay.Color = new ACadSharp.Color(7);
            pdfUnderlay.LineType = new LineType("CONTINUOUS");
            pdfUnderlay.LineWeight = LineWeightType.ByLayer;
            pdfUnderlay.IsInvisible = false;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal("", result.FilePath);
            Assert.Equal("", result.DefinitionFileName);
            Assert.Equal("1", result.PageNumber);
        }

        [Fact]
        public void Render_PdfUnderlay_WithNullClipBoundaryVertices_ReturnsNoClippingBoundary()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = null;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal("None", result.ClippingBoundaryType);
            Assert.Equal(0, result.BoundaryPointCount);
            Assert.Empty(result.BoundaryPoints);
            Assert.Empty(result.BoundaryPoints3D);
        }

        [Fact]
        public void Render_PdfUnderlay_WithEmptyClipBoundaryVertices_ReturnsNoClippingBoundary()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new System.Collections.Generic.List<XY>();
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal("None", result.ClippingBoundaryType);
            Assert.Equal(0, result.BoundaryPointCount);
        }

        [Fact]
        public void Render_PdfUnderlay_WithTwoClipBoundaryVertices_ReturnsRectangleBoundary()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new System.Collections.Generic.List<XY>
            {
                new XY(0, 0),
                new XY(100, 100)
            };
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal("Rectangle", result.ClippingBoundaryType);
            Assert.Equal(4, result.BoundaryPointCount);
            Assert.Equal(4, result.BoundaryPoints.Count);
            Assert.Equal(4, result.BoundaryPoints3D.Count);
        }

        [Fact]
        public void Render_PdfUnderlay_WithMultipleClipBoundaryVertices_ReturnsPolygonBoundary()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new System.Collections.Generic.List<XY>
            {
                new XY(0, 0),
                new XY(100, 0),
                new XY(100, 100),
                new XY(0, 100)
            };
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal("Polygon", result.ClippingBoundaryType);
            Assert.Equal(4, result.BoundaryPointCount);
        }

        [Fact]
        public void Render_PdfUnderlay_WithRectangleBoundary_CreatesCorrectVertices()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.InsertPoint = new XYZ(10, 10, 0);
            pdfUnderlay.ClipBoundaryVertices = new System.Collections.Generic.List<XY>
            {
                new XY(0, 0),
                new XY(100, 100)
            };
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(4, result.BoundaryPoints.Count);
            Assert.Equal(4, result.BoundaryPoints3D.Count);

            Assert.Equal(10, result.BoundaryPoints[0].X);
            Assert.Equal(10, result.BoundaryPoints[0].Y);
            Assert.Equal(110, result.BoundaryPoints[1].X);
            Assert.Equal(10, result.BoundaryPoints[1].Y);
            Assert.Equal(110, result.BoundaryPoints[2].X);
            Assert.Equal(110, result.BoundaryPoints[2].Y);
            Assert.Equal(10, result.BoundaryPoints[3].X);
            Assert.Equal(110, result.BoundaryPoints[3].Y);
        }

        [Fact]
        public void Render_PdfUnderlay_WithScale_AppliesScaleToBoundary()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.InsertPoint = new XYZ(10, 10, 0);
            pdfUnderlay.XScale = 2.0;
            pdfUnderlay.YScale = 3.0;
            pdfUnderlay.ClipBoundaryVertices = new System.Collections.Generic.List<XY>
            {
                new XY(0, 0),
                new XY(50, 50)
            };
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(10, result.BoundaryPoints[0].X);
            Assert.Equal(10, result.BoundaryPoints[0].Y);
            Assert.Equal(110, result.BoundaryPoints[1].X);
            Assert.Equal(10, result.BoundaryPoints[1].Y);
            Assert.Equal(110, result.BoundaryPoints[2].X);
            Assert.Equal(160, result.BoundaryPoints[2].Y);
            Assert.Equal(10, result.BoundaryPoints[3].X);
            Assert.Equal(160, result.BoundaryPoints[3].Y);
        }

        [Fact]
        public void Render_PdfUnderlay_WithBoundary_CalculatesCorrectBounds()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new System.Collections.Generic.List<XY>
            {
                new XY(0, 0),
                new XY(100, 100)
            };
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds3D);
            Assert.NotNull(result.Geometry.BoundingBox);

            Assert.Equal(10, result.Bounds.Min.X);
            Assert.Equal(10, result.Bounds.Min.Y);
            Assert.Equal(110, result.Bounds.Max.X);
            Assert.Equal(110, result.Bounds.Max.Y);
        }

        [Fact]
        public void Render_PdfUnderlay_WithBoundary_CalculatesCorrectCentroid()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new System.Collections.Generic.List<XY>
            {
                new XY(0, 0),
                new XY(100, 100)
            };
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);
            Assert.NotNull(result.Centroid3D);

            Assert.Equal(60, result.Centroid.X);
            Assert.Equal(60, result.Centroid.Y);
            Assert.Equal(60, result.Centroid3D.X);
            Assert.Equal(60, result.Centroid3D.Y);
        }

        [Fact]
        public void Render_PdfUnderlay_WithRectangleBoundary_CalculatesCorrectArea()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new System.Collections.Generic.List<XY>
            {
                new XY(0, 0),
                new XY(100, 100)
            };
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(10000, result.Area);
        }

        [Fact]
        public void Render_PdfUnderlay_WithPolygonBoundary_CreatesCorrectIndices()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new System.Collections.Generic.List<XY>
            {
                new XY(0, 0),
                new XY(100, 0),
                new XY(100, 100),
                new XY(0, 100)
            };
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry.Indices);
            Assert.Equal(6, result.Geometry.Indices.Count);
            Assert.Equal(0, result.Geometry.Indices[0]);
            Assert.Equal(1, result.Geometry.Indices[1]);
            Assert.Equal(2, result.Geometry.Indices[2]);
            Assert.Equal(0, result.Geometry.Indices[3]);
            Assert.Equal(2, result.Geometry.Indices[4]);
            Assert.Equal(3, result.Geometry.Indices[5]);
        }

        [Fact]
        public void Render_PdfUnderlay_WithPolygonBoundary_CalculatesCorrectArea()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new System.Collections.Generic.List<XY>
            {
                new XY(0, 0),
                new XY(100, 0),
                new XY(100, 100),
                new XY(0, 100)
            };
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(10000, result.Area);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectTransform()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.InsertPoint = new XYZ(10, 20, 30);
            pdfUnderlay.XScale = 2.0;
            pdfUnderlay.YScale = 3.0;
            pdfUnderlay.ZScale = 4.0;
            pdfUnderlay.Rotation = Math.PI / 2;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Position);
            Assert.NotNull(result.Transform.Rotation);
            Assert.NotNull(result.Transform.Scale);

            Assert.Equal(10, result.Transform.Position.X);
            Assert.Equal(20, result.Transform.Position.Y);
            Assert.Equal(30, result.Transform.Position.Z);
            Assert.Equal(2.0, result.Transform.Scale.X);
            Assert.Equal(3.0, result.Transform.Scale.Y);
            Assert.Equal(4.0, result.Transform.Scale.Z);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectGeometryTransform()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.InsertPoint = new XYZ(10, 20, 30);
            pdfUnderlay.XScale = 2.0;
            pdfUnderlay.YScale = 3.0;
            pdfUnderlay.ZScale = 4.0;
            pdfUnderlay.Rotation = Math.PI / 2;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);
            Assert.NotNull(result.Geometry.Position);
            Assert.NotNull(result.Geometry.Rotation);
            Assert.NotNull(result.Geometry.Scale);

            Assert.Equal(10, result.Geometry.Position.X);
            Assert.Equal(20, result.Geometry.Position.Y);
            Assert.Equal(30, result.Geometry.Position.Z);
            Assert.Equal(2.0, result.Geometry.Scale.X);
            Assert.Equal(3.0, result.Geometry.Scale.Y);
            Assert.Equal(4.0, result.Geometry.Scale.Z);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectMaterialOpacity()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Fade = 30;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);
            Assert.Equal(0.7, result.Material.Opacity);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectMaterialContrast()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Contrast = 80;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);
            Assert.Equal(80, result.Material.Contrast);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectMaterialFade()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Fade = 40;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);
            Assert.Equal(40, result.Material.Fade);
        }

        [Fact]
        public void Render_PdfUnderlay_WithFullFade_ReturnsZeroOpacity()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Fade = 100;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(0, result.Opacity);
            Assert.Equal(0, result.Material.Opacity);
        }

        [Fact]
        public void Render_PdfUnderlay_WithNoFade_ReturnsFullOpacity()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Fade = 0;
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(1.0, result.Opacity);
            Assert.Equal(1.0, result.Material.Opacity);
        }

        [Fact]
        public void Render_PdfUnderlay_ReturnsCorrectLayerIndex()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            var result = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(result);
            Assert.Equal(0, result.LayerIndex);
        }

        [Fact]
        public void Render_PdfUnderlay_WithLineWeight_PreservesLineWeightInformation()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.LineWeight = ACadSharp.LineWeightType.W30;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(0.3, pdfUnderlayData.LineWeight);
        }

        [Fact]
        public void Render_PdfUnderlay_WithRotation_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Rotation = Math.PI / 2;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(Math.PI / 2, pdfUnderlayData.Rotation);
        }

        [Fact]
        public void Render_PdfUnderlay_WithFullRotation_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Rotation = 2 * Math.PI;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(2 * Math.PI, pdfUnderlayData.Rotation);
        }
    }
}
