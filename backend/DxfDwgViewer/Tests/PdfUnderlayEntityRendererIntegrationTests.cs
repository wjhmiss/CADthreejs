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
    public class PdfUnderlayEntityRendererIntegrationTests
    {
        [Fact]
        public void LoadAndRenderPdfUnderlayFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var pdfUnderlayEntities = new List<PdfUnderlay>();
            foreach (var entity in doc.Entities)
            {
                if (entity is PdfUnderlay pdfUnderlay)
                {
                    pdfUnderlayEntities.Add(pdfUnderlay);
                }
            }

            if (pdfUnderlayEntities.Count == 0)
            {
                return;
            }

            foreach (var pdfUnderlay in pdfUnderlayEntities)
            {
                var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);
                Assert.NotNull(pdfUnderlayData);
                Assert.Equal("Underlay", pdfUnderlayData.Type);
                Assert.Equal("PdfUnderlay", pdfUnderlayData.EntityType);
                Assert.NotNull(pdfUnderlayData.InsertPoint);
                Assert.NotNull(pdfUnderlayData.BoundaryPoints);
                Assert.NotNull(pdfUnderlayData.Bounds);
                Assert.NotNull(pdfUnderlayData.Material);
                Assert.NotNull(pdfUnderlayData.Geometry);
            }
        }

        [Fact]
        public void LoadAndRenderPdfUnderlayFromDxf_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dxf");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DxfReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var pdfUnderlayEntities = new List<PdfUnderlay>();
            foreach (var entity in doc.Entities)
            {
                if (entity is PdfUnderlay pdfUnderlay)
                {
                    pdfUnderlayEntities.Add(pdfUnderlay);
                }
            }

            if (pdfUnderlayEntities.Count == 0)
            {
                return;
            }

            foreach (var pdfUnderlay in pdfUnderlayEntities)
            {
                var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);
                Assert.NotNull(pdfUnderlayData);
                Assert.Equal("Underlay", pdfUnderlayData.Type);
                Assert.Equal("PdfUnderlay", pdfUnderlayData.EntityType);
                Assert.True(pdfUnderlayData.XScale > 0);
                Assert.True(pdfUnderlayData.YScale > 0);
                Assert.True(pdfUnderlayData.ZScale > 0);
            }
        }

        [Fact]
        public void SerializePdfUnderlayDataToJson_Success()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);
            var json = JsonConvert.SerializeObject(pdfUnderlayData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Underlay", json);
            Assert.Contains("PdfUnderlay", json);
            Assert.Contains("InsertPoint", json);
            Assert.Contains("XScale", json);
            Assert.Contains("YScale", json);
            Assert.Contains("ZScale", json);
            Assert.Contains("Rotation", json);
            Assert.Contains("Contrast", json);
            Assert.Contains("Fade", json);
        }

        [Fact]
        public void DeserializePdfUnderlayDataFromJson_Success()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();

            var originalPdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);
            var json = JsonConvert.SerializeObject(originalPdfUnderlayData);
            var deserializedPdfUnderlayData = JsonConvert.DeserializeObject<PdfUnderlayEntityRenderer.PdfUnderlayData>(json);

            Assert.NotNull(deserializedPdfUnderlayData);
            Assert.Equal(originalPdfUnderlayData.Type, deserializedPdfUnderlayData.Type);
            Assert.Equal(originalPdfUnderlayData.EntityType, deserializedPdfUnderlayData.EntityType);
            Assert.Equal(originalPdfUnderlayData.InsertPoint.X, deserializedPdfUnderlayData.InsertPoint.X);
            Assert.Equal(originalPdfUnderlayData.InsertPoint.Y, deserializedPdfUnderlayData.InsertPoint.Y);
            Assert.Equal(originalPdfUnderlayData.XScale, deserializedPdfUnderlayData.XScale);
            Assert.Equal(originalPdfUnderlayData.YScale, deserializedPdfUnderlayData.YScale);
            Assert.Equal(originalPdfUnderlayData.ZScale, deserializedPdfUnderlayData.ZScale);
            Assert.Equal(originalPdfUnderlayData.Rotation, deserializedPdfUnderlayData.Rotation);
            Assert.Equal(originalPdfUnderlayData.ColorIndex, deserializedPdfUnderlayData.ColorIndex);
            Assert.Equal(originalPdfUnderlayData.LineWeight, deserializedPdfUnderlayData.LineWeight);
            Assert.Equal(originalPdfUnderlayData.Contrast, deserializedPdfUnderlayData.Contrast);
            Assert.Equal(originalPdfUnderlayData.Fade, deserializedPdfUnderlayData.Fade);
            Assert.Equal(originalPdfUnderlayData.Opacity, deserializedPdfUnderlayData.Opacity);
            Assert.Equal(originalPdfUnderlayData.Transparent, deserializedPdfUnderlayData.Transparent);
            Assert.Equal(originalPdfUnderlayData.Visible, deserializedPdfUnderlayData.Visible);
            Assert.Equal(originalPdfUnderlayData.ClippingBoundaryType, deserializedPdfUnderlayData.ClippingBoundaryType);
        }

        [Fact]
        public void RenderPdfUnderlayWithComplexBoundary_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new List<XY>
            {
                new XY(0, 0),
                new XY(100, 0),
                new XY(100, 50),
                new XY(150, 50),
                new XY(150, 100),
                new XY(0, 100)
            };

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal("Polygon", pdfUnderlayData.ClippingBoundaryType);
            Assert.Equal(6, pdfUnderlayData.BoundaryPointCount);
            Assert.Equal(6, pdfUnderlayData.BoundaryPoints.Count);
            Assert.Equal(6, pdfUnderlayData.BoundaryPoints3D.Count);
            Assert.True(pdfUnderlayData.Area > 0);
        }

        [Fact]
        public void RenderPdfUnderlayWithDifferentScales_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.XScale = 2.5;
            pdfUnderlay.YScale = 1.5;
            pdfUnderlay.ZScale = 0.5;
            pdfUnderlay.ClipBoundaryVertices = new List<XY>
            {
                new XY(0, 0),
                new XY(100, 100)
            };

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(2.5, pdfUnderlayData.XScale);
            Assert.Equal(1.5, pdfUnderlayData.YScale);
            Assert.Equal(0.5, pdfUnderlayData.ZScale);
            Assert.Equal(2.5, pdfUnderlayData.Transform.Scale.X);
            Assert.Equal(1.5, pdfUnderlayData.Transform.Scale.Y);
            Assert.Equal(0.5, pdfUnderlayData.Transform.Scale.Z);
            Assert.Equal(2.5, pdfUnderlayData.Geometry.Scale.X);
            Assert.Equal(1.5, pdfUnderlayData.Geometry.Scale.Y);
            Assert.Equal(0.5, pdfUnderlayData.Geometry.Scale.Z);
        }

        [Fact]
        public void RenderPdfUnderlayWithRotation_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Rotation = Math.PI / 3;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(Math.PI / 3, pdfUnderlayData.Rotation);
            Assert.Equal(Math.PI / 3, pdfUnderlayData.Transform.Rotation.Z);
            Assert.Equal(Math.PI / 3, pdfUnderlayData.Geometry.Rotation.Z);
        }

        [Fact]
        public void RenderPdfUnderlayWithFullRotation_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Rotation = 2 * Math.PI;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(2 * Math.PI, pdfUnderlayData.Rotation);
        }

        [Fact]
        public void RenderPdfUnderlayWithZeroRotation_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Rotation = 0;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(0, pdfUnderlayData.Rotation);
        }

        [Fact]
        public void RenderPdfUnderlayWithContrast_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Contrast = 90;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(90, pdfUnderlayData.Contrast);
            Assert.Equal(90, pdfUnderlayData.Material.Contrast);
        }

        [Fact]
        public void RenderPdfUnderlayWithFade_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Fade = 50;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(50, pdfUnderlayData.Fade);
            Assert.Equal(50, pdfUnderlayData.Material.Fade);
            Assert.Equal(0.5, pdfUnderlayData.Opacity);
            Assert.Equal(0.5, pdfUnderlayData.Material.Opacity);
            Assert.True(pdfUnderlayData.Transparent);
            Assert.True(pdfUnderlayData.Material.Transparent);
        }

        [Fact]
        public void RenderPdfUnderlayWithFullFade_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Fade = 100;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(100, pdfUnderlayData.Fade);
            Assert.Equal(0, pdfUnderlayData.Opacity);
            Assert.Equal(0, pdfUnderlayData.Material.Opacity);
            Assert.True(pdfUnderlayData.Transparent);
            Assert.True(pdfUnderlayData.Material.Transparent);
        }

        [Fact]
        public void RenderPdfUnderlayWithNoFade_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Fade = 0;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(0, pdfUnderlayData.Fade);
            Assert.Equal(1.0, pdfUnderlayData.Opacity);
            Assert.Equal(1.0, pdfUnderlayData.Material.Opacity);
            Assert.False(pdfUnderlayData.Transparent);
            Assert.False(pdfUnderlayData.Material.Transparent);
        }

        [Fact]
        public void RenderPdfUnderlayWithInvisibleFlag_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.IsInvisible = true;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.False(pdfUnderlayData.Visible);
        }

        [Fact]
        public void RenderPdfUnderlayWithVisibleFlag_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.IsInvisible = false;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.True(pdfUnderlayData.Visible);
        }

        [Fact]
        public void RenderPdfUnderlayWithDifferentColors_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Color = new Color(5);

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(5, pdfUnderlayData.ColorIndex);
            Assert.Equal(5, pdfUnderlayData.Color.Index);
            Assert.Equal(5, pdfUnderlayData.Material.Color.Index);
        }

        [Fact]
        public void RenderPdfUnderlayWithLineWeight_PreservesLineWeightInformation()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.LineWeight = ACadSharp.LineWeightType.W30;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(0.3, pdfUnderlayData.LineWeight);
        }

        [Fact]
        public void RenderPdfUnderlayWithDefinition_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            var definition = new PdfUnderlayDefinition
            {
                File = "test.pdf",
                Page = "2"
            };
            pdfUnderlay.Definition = definition;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal("test.pdf", pdfUnderlayData.FilePath);
            Assert.Equal("test.pdf", pdfUnderlayData.DefinitionFileName);
            Assert.Equal("2", pdfUnderlayData.PageNumber);
        }

        [Fact]
        public void RenderPdfUnderlayWithNullDefinition_HandlesGracefully()
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

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal("", pdfUnderlayData.FilePath);
            Assert.Equal("", pdfUnderlayData.DefinitionFileName);
            Assert.Equal("1", pdfUnderlayData.PageNumber);
        }

        [Fact]
        public void RenderPdfUnderlayWithNullClipBoundaryVertices_HandlesGracefully()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = null;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal("None", pdfUnderlayData.ClippingBoundaryType);
            Assert.Equal(0, pdfUnderlayData.BoundaryPointCount);
            Assert.Empty(pdfUnderlayData.BoundaryPoints);
            Assert.Empty(pdfUnderlayData.BoundaryPoints3D);
        }

        [Fact]
        public void RenderPdfUnderlayWithEmptyClipBoundaryVertices_HandlesGracefully()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new List<XY>();

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal("None", pdfUnderlayData.ClippingBoundaryType);
            Assert.Equal(0, pdfUnderlayData.BoundaryPointCount);
        }

        [Fact]
        public void RenderPdfUnderlayWithSingleClipBoundaryVertex_HandlesGracefully()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new List<XY>
            {
                new XY(50, 50)
            };

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal("None", pdfUnderlayData.ClippingBoundaryType);
            Assert.Equal(0, pdfUnderlayData.BoundaryPointCount);
        }

        [Fact]
        public void RenderPdfUnderlayWithRectangleBoundary_CalculatesCorrectArea()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new List<XY>
            {
                new XY(0, 0),
                new XY(100, 100)
            };

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(10000, pdfUnderlayData.Area);
        }

        [Fact]
        public void RenderPdfUnderlayWithPolygonBoundary_CalculatesCorrectArea()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new List<XY>
            {
                new XY(0, 0),
                new XY(100, 0),
                new XY(100, 100),
                new XY(0, 100)
            };

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(10000, pdfUnderlayData.Area);
        }

        [Fact]
        public void RenderPdfUnderlayWithBoundary_CalculatesCorrectBounds()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new List<XY>
            {
                new XY(0, 0),
                new XY(100, 100)
            };

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.NotNull(pdfUnderlayData.Bounds);
            Assert.NotNull(pdfUnderlayData.Bounds3D);
            Assert.NotNull(pdfUnderlayData.Geometry.BoundingBox);

            Assert.Equal(10, pdfUnderlayData.Bounds.Min.X);
            Assert.Equal(10, pdfUnderlayData.Bounds.Min.Y);
            Assert.Equal(110, pdfUnderlayData.Bounds.Max.X);
            Assert.Equal(110, pdfUnderlayData.Bounds.Max.Y);
        }

        [Fact]
        public void RenderPdfUnderlayWithBoundary_CalculatesCorrectCentroid()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new List<XY>
            {
                new XY(0, 0),
                new XY(100, 100)
            };

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.NotNull(pdfUnderlayData.Centroid);
            Assert.NotNull(pdfUnderlayData.Centroid3D);

            Assert.Equal(60, pdfUnderlayData.Centroid.X);
            Assert.Equal(60, pdfUnderlayData.Centroid.Y);
            Assert.Equal(60, pdfUnderlayData.Centroid3D.X);
            Assert.Equal(60, pdfUnderlayData.Centroid3D.Y);
        }

        [Fact]
        public void RenderPdfUnderlayWithBoundary_CreatesCorrectGeometryIndices()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new List<XY>
            {
                new XY(0, 0),
                new XY(100, 0),
                new XY(100, 100),
                new XY(0, 100)
            };

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.NotNull(pdfUnderlayData.Geometry.Indices);
            Assert.Equal(6, pdfUnderlayData.Geometry.Indices.Count);
        }

        [Fact]
        public void RenderPdfUnderlayWithBoundary_CreatesCorrectGeometryVertices()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.ClipBoundaryVertices = new List<XY>
            {
                new XY(0, 0),
                new XY(100, 100)
            };

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.NotNull(pdfUnderlayData.Geometry.Vertices);
            Assert.Equal(4, pdfUnderlayData.Geometry.Vertices.Count);
        }

        [Fact]
        public void RenderPdfUnderlayWithNormal_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.Normal = new XYZ(0, 0, 1);

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(0, pdfUnderlayData.Normal.X);
            Assert.Equal(0, pdfUnderlayData.Normal.Y);
            Assert.Equal(1, pdfUnderlayData.Normal.Z);
            Assert.Equal(0, pdfUnderlayData.Geometry.Normal.X);
            Assert.Equal(0, pdfUnderlayData.Geometry.Normal.Y);
            Assert.Equal(1, pdfUnderlayData.Geometry.Normal.Z);
        }

        [Fact]
        public void RenderPdfUnderlayWithDifferentInsertPoint_HandlesCorrectly()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.InsertPoint = new XYZ(100, 200, 300);

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal(100, pdfUnderlayData.InsertPoint.X);
            Assert.Equal(200, pdfUnderlayData.InsertPoint.Y);
            Assert.Equal(300, pdfUnderlayData.InsertPoint3D.Z);
            Assert.Equal(100, pdfUnderlayData.Transform.Position.X);
            Assert.Equal(200, pdfUnderlayData.Transform.Position.Y);
            Assert.Equal(300, pdfUnderlayData.Transform.Position.Z);
            Assert.Equal(100, pdfUnderlayData.Geometry.Position.X);
            Assert.Equal(200, pdfUnderlayData.Geometry.Position.Y);
            Assert.Equal(300, pdfUnderlayData.Geometry.Position.Z);
        }

        [Fact]
        public void RenderPdfUnderlayWithNullLayer_HandlesGracefully()
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

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.NotNull(pdfUnderlayData.LayerName);
        }

        [Fact]
        public void RenderPdfUnderlayWithNullLineType_HandlesGracefully()
        {
            var definition = new PdfUnderlayDefinition
            {
                File = "test.pdf",
                Page = "1"
            };
            var pdfUnderlay = new PdfUnderlay(definition);
            pdfUnderlay.Color = new ACadSharp.Color(7);
            pdfUnderlay.LineWeight = LineWeightType.ByLayer;
            pdfUnderlay.IsInvisible = false;

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.NotNull(pdfUnderlayData.LineTypeName);
        }

        [Fact]
        public void RenderPdfUnderlayWithAllProperties_PreservesAllInformation()
        {
            var pdfUnderlay = CreateBasicPdfUnderlay();
            pdfUnderlay.InsertPoint = new XYZ(100, 200, 300);
            pdfUnderlay.XScale = 2.0;
            pdfUnderlay.YScale = 3.0;
            pdfUnderlay.ZScale = 4.0;
            pdfUnderlay.Rotation = Math.PI / 4;
            pdfUnderlay.Color = new Color(5);
            pdfUnderlay.Contrast = 75;
            pdfUnderlay.Fade = 25;
            pdfUnderlay.IsInvisible = false;
            pdfUnderlay.Normal = new XYZ(0, 0, 1);
            pdfUnderlay.ClipBoundaryVertices = new List<XY>
            {
                new XY(0, 0),
                new XY(100, 100)
            };

            var pdfUnderlayData = PdfUnderlayEntityRenderer.Render(pdfUnderlay);

            Assert.NotNull(pdfUnderlayData);
            Assert.Equal("Underlay", pdfUnderlayData.Type);
            Assert.Equal("PdfUnderlay", pdfUnderlayData.EntityType);
            Assert.Equal(100, pdfUnderlayData.InsertPoint.X);
            Assert.Equal(200, pdfUnderlayData.InsertPoint.Y);
            Assert.Equal(300, pdfUnderlayData.InsertPoint3D.Z);
            Assert.Equal(2.0, pdfUnderlayData.XScale);
            Assert.Equal(3.0, pdfUnderlayData.YScale);
            Assert.Equal(4.0, pdfUnderlayData.ZScale);
            Assert.Equal(Math.PI / 4, pdfUnderlayData.Rotation);
            Assert.Equal(5, pdfUnderlayData.ColorIndex);
            Assert.Equal(75, pdfUnderlayData.Contrast);
            Assert.Equal(25, pdfUnderlayData.Fade);
            Assert.True(pdfUnderlayData.Visible);
            Assert.Equal("Rectangle", pdfUnderlayData.ClippingBoundaryType);
            Assert.Equal(4, pdfUnderlayData.BoundaryPointCount);
            Assert.True(pdfUnderlayData.Area > 0);
        }

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
    }
}
