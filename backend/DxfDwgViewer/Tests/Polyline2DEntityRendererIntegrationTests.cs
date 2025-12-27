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
    public class Polyline2DEntityRendererIntegrationTests
    {
        private Polyline2D CreateBasicPolyline2D()
        {
            var polyline = new Polyline2D
            {
                IsClosed = false,
                Elevation = 0,
                Thickness = 0,
                StartWidth = 0,
                EndWidth = 0,
                SmoothSurface = SmoothSurfaceType.None
            };

            polyline.Vertices.Add(new Vertex2D
            {
                Location = new XY(0, 0),
                Bulge = 0,
                StartWidth = 0,
                EndWidth = 0
            });

            polyline.Vertices.Add(new Vertex2D
            {
                Location = new XY(10, 0),
                Bulge = 0,
                StartWidth = 0,
                EndWidth = 0
            });

            polyline.Vertices.Add(new Vertex2D
            {
                Location = new XY(10, 10),
                Bulge = 0,
                StartWidth = 0,
                EndWidth = 0
            });

            polyline.Color = new Color(1);
            polyline.LineTypeScale = 1.0;

            return polyline;
        }

        [Fact]
        public void LoadAndRenderPolyline2DFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var polylineEntities = new List<Polyline2D>();
            foreach (var entity in doc.Entities)
            {
                if (entity is Polyline2D polyline)
                {
                    polylineEntities.Add(polyline);
                }
            }

            if (polylineEntities.Count == 0)
            {
                return;
            }

            foreach (var polyline in polylineEntities)
            {
                var polylineData = Polyline2DEntityRenderer.Render(polyline);
                Assert.NotNull(polylineData);
                Assert.Equal("Polyline2D", polylineData.EntityType);
                Assert.NotNull(polylineData.Vertices);
                Assert.NotNull(polylineData.Bounds3D);
            }
        }

        [Fact]
        public void SerializePolyline2DDataToJson_Success()
        {
            var polyline = CreateBasicPolyline2D();

            var polylineData = Polyline2DEntityRenderer.Render(polyline);
            var json = JsonConvert.SerializeObject(polylineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Polyline2D", json);
            Assert.Contains("Vertices", json);
            Assert.Contains("Bounds3D", json);
        }

        [Fact]
        public void DeserializePolyline2DDataFromJson_Success()
        {
            var polyline = CreateBasicPolyline2D();

            var originalPolylineData = Polyline2DEntityRenderer.Render(polyline);
            var json = JsonConvert.SerializeObject(originalPolylineData);
            var deserializedPolylineData = JsonConvert.DeserializeObject<Polyline2DEntityRenderer.Polyline2DData>(json);

            Assert.NotNull(deserializedPolylineData);
            Assert.Equal(originalPolylineData.EntityType, deserializedPolylineData.EntityType);
            Assert.Equal(originalPolylineData.VertexCount, deserializedPolylineData.VertexCount);
            Assert.Equal(originalPolylineData.SegmentCount, deserializedPolylineData.SegmentCount);
            Assert.Equal(originalPolylineData.IsClosed, deserializedPolylineData.IsClosed);
            Assert.Equal(originalPolylineData.ColorIndex, deserializedPolylineData.ColorIndex);
            Assert.Equal(originalPolylineData.LineTypeScale, deserializedPolylineData.LineTypeScale);
            Assert.Equal(originalPolylineData.Elevation, deserializedPolylineData.Elevation);
            Assert.Equal(originalPolylineData.Thickness, deserializedPolylineData.Thickness);
            Assert.Equal(originalPolylineData.StartWidth, deserializedPolylineData.StartWidth);
            Assert.Equal(originalPolylineData.EndWidth, deserializedPolylineData.EndWidth);
            Assert.Equal(originalPolylineData.SmoothSurface, deserializedPolylineData.SmoothSurface);
            Assert.Equal(originalPolylineData.TotalLength, deserializedPolylineData.TotalLength);
            Assert.Equal(originalPolylineData.HasArcSegments, deserializedPolylineData.HasArcSegments);
            Assert.Equal(originalPolylineData.HasVariableWidth, deserializedPolylineData.HasVariableWidth);
        }

        [Fact]
        public void RenderMultiplePolylines2D_CollectAllPolylineData()
        {
            var polylines = new List<Polyline2D>
            {
                CreateBasicPolyline2D(),
                CreateBasicPolyline2D()
            };
            polylines[0].Vertices[0].Location = new XY(0, 0);
            polylines[1].Vertices[0].Location = new XY(20, 20);

            var polylineDataList = new List<Polyline2DEntityRenderer.Polyline2DData>();
            foreach (var polyline in polylines)
            {
                var polylineData = Polyline2DEntityRenderer.Render(polyline);
                polylineDataList.Add(polylineData);
            }

            Assert.Equal(2, polylineDataList.Count);
            Assert.Equal(0, polylineDataList[0].Vertices[0].Location.X);
            Assert.Equal(20, polylineDataList[1].Vertices[0].Location.X);
        }

        [Fact]
        public void RenderPolyline2DWithComplexProperties_PreservesAllProperties()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.IsClosed = true;
            polyline.Elevation = 5.5;
            polyline.Thickness = 3.0;
            polyline.StartWidth = 1.5;
            polyline.EndWidth = 2.0;
            polyline.SmoothSurface = SmoothSurfaceType.Quadratic;
            polyline.LineTypeScale = 2.5;
            polyline.Color = new Color(5);

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.True(polylineData.IsClosed);
            Assert.Equal(5.5, polylineData.Elevation);
            Assert.Equal(3.0, polylineData.Thickness);
            Assert.Equal(1.5, polylineData.StartWidth);
            Assert.Equal(2.0, polylineData.EndWidth);
            Assert.Equal(SmoothSurfaceType.Quadratic, polylineData.SmoothSurface);
            Assert.Equal(2.5, polylineData.LineTypeScale);
            Assert.Equal(5, polylineData.ColorIndex);
        }

        [Fact]
        public void RenderPolyline2DWithArcSegments_PreservesArcData()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.Vertices[0].Bulge = 1.0;

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.True(polylineData.HasArcSegments);
            Assert.Single(polylineData.ArcSegments);

            var arcSegment = polylineData.ArcSegments[0];
            Assert.NotNull(arcSegment.StartPoint);
            Assert.NotNull(arcSegment.EndPoint);
            Assert.NotNull(arcSegment.CenterPoint);
            Assert.True(arcSegment.Radius > 0);
            Assert.True(arcSegment.IsCounterClockwise);
            Assert.True(arcSegment.SegmentCount > 0);
        }

        [Fact]
        public void RenderPolyline2DWithVariableWidth_DetectsVariableWidth()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.Vertices[0].StartWidth = 1.0;
            polyline.Vertices[0].EndWidth = 2.0;

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.True(polylineData.HasVariableWidth);
        }

        [Fact]
        public void RenderClosedPolyline2D_CalculatesCorrectTotalLength()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.IsClosed = true;

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.Equal(30, polylineData.TotalLength);
        }

        [Fact]
        public void RenderPolyline2DWithElevation_CalculatesCorrectZValues()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.Elevation = 10.0;

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.Equal(10.0, polylineData.Elevation);

            foreach (var vertex in polylineData.Vertices3D)
            {
                Assert.Equal(10.0, vertex.Z);
            }
        }

        [Fact]
        public void RenderPolyline2D_CalculatesCorrectCentroid()
        {
            var polyline = CreateBasicPolyline2D();

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.Centroid);

            Assert.Equal(20.0 / 3.0, polylineData.Centroid.X, 2);
            Assert.Equal(10.0 / 3.0, polylineData.Centroid.Y, 2);
            Assert.Equal(0, polylineData.Centroid.Z);
        }

        [Fact]
        public void RenderPolyline2D_CalculatesCorrectBounds()
        {
            var polyline = CreateBasicPolyline2D();

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.Bounds3D);

            Assert.Equal(0, polylineData.Bounds3D.Min.X);
            Assert.Equal(0, polylineData.Bounds3D.Min.Y);
            Assert.Equal(0, polylineData.Bounds3D.Min.Z);

            Assert.Equal(10, polylineData.Bounds3D.Max.X);
            Assert.Equal(10, polylineData.Bounds3D.Max.Y);
            Assert.Equal(0, polylineData.Bounds3D.Max.Z);
        }

        [Fact]
        public void RenderPolyline2D_GeneratesCorrectIndices()
        {
            var polyline = CreateBasicPolyline2D();

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.Equal(4, polylineData.Indices.Count);
            Assert.Equal(0, polylineData.Indices[0]);
            Assert.Equal(1, polylineData.Indices[1]);
            Assert.Equal(1, polylineData.Indices[2]);
            Assert.Equal(2, polylineData.Indices[3]);
        }

        [Fact]
        public void RenderClosedPolyline2D_GeneratesCorrectIndices()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.IsClosed = true;

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.Equal(6, polylineData.Indices.Count);
            Assert.Equal(0, polylineData.Indices[0]);
            Assert.Equal(1, polylineData.Indices[1]);
            Assert.Equal(1, polylineData.Indices[2]);
            Assert.Equal(2, polylineData.Indices[3]);
            Assert.Equal(2, polylineData.Indices[4]);
            Assert.Equal(0, polylineData.Indices[5]);
        }

        [Fact]
        public void RenderPolyline2D_GeneratesCorrectNormals()
        {
            var polyline = CreateBasicPolyline2D();

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.Equal(9, polylineData.NormalsArray.Count);

            for (int i = 0; i < polylineData.NormalsArray.Count; i += 3)
            {
                Assert.Equal(0, polylineData.NormalsArray[i]);
                Assert.Equal(0, polylineData.NormalsArray[i + 1]);
                Assert.Equal(1, polylineData.NormalsArray[i + 2]);
            }
        }

        [Fact]
        public void RenderPolyline2D_GeneratesCorrectUVs()
        {
            var polyline = CreateBasicPolyline2D();

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.Equal(6, polylineData.UVsArray.Count);

            Assert.Equal(0, polylineData.UVsArray[0]);
            Assert.Equal(0, polylineData.UVsArray[1]);

            Assert.Equal(0.5, polylineData.UVsArray[2], 2);
            Assert.Equal(0, polylineData.UVsArray[3]);

            Assert.Equal(1, polylineData.UVsArray[4], 2);
            Assert.Equal(0, polylineData.UVsArray[5]);
        }

        [Fact]
        public void RenderPolyline2D_WithNegativeBulge_CreatesCounterClockwiseArc()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.Vertices[0].Bulge = -1.0;

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.True(polylineData.HasArcSegments);
            Assert.Single(polylineData.ArcSegments);

            var arcSegment = polylineData.ArcSegments[0];
            Assert.False(arcSegment.IsCounterClockwise);
        }

        [Fact]
        public void RenderPolyline2D_EmptyVertices_ReturnsEmptyData()
        {
            var polyline = new Polyline2D
            {
                IsClosed = false,
                Color = new Color(1)
            };

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.Equal(0, polylineData.VertexCount);
            Assert.Empty(polylineData.Vertices);
            Assert.Empty(polylineData.Vertices3D);
            Assert.Equal(0, polylineData.TotalLength);
        }

        [Fact]
        public void RenderPolyline2D_SingleVertex_ReturnsZeroSegments()
        {
            var polyline = new Polyline2D
            {
                IsClosed = false,
                Color = new Color(1)
            };

            polyline.Vertices.Add(new Vertex2D
            {
                Location = new XY(5, 5),
                Bulge = 0,
                StartWidth = 0,
                EndWidth = 0
            });

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.Equal(1, polylineData.VertexCount);
            Assert.Equal(0, polylineData.SegmentCount);
            Assert.Equal(0, polylineData.TotalLength);
        }

        [Fact]
        public void SerializeAndDeserializePolyline2DWithArcSegments_PreservesArcData()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.Vertices[0].Bulge = 1.0;

            var originalPolylineData = Polyline2DEntityRenderer.Render(polyline);
            var json = JsonConvert.SerializeObject(originalPolylineData);
            var deserializedPolylineData = JsonConvert.DeserializeObject<Polyline2DEntityRenderer.Polyline2DData>(json);

            Assert.NotNull(deserializedPolylineData);
            Assert.Equal(originalPolylineData.HasArcSegments, deserializedPolylineData.HasArcSegments);
            Assert.Equal(originalPolylineData.ArcSegments.Count, deserializedPolylineData.ArcSegments.Count);

            if (deserializedPolylineData.ArcSegments.Count > 0)
            {
                var originalArc = originalPolylineData.ArcSegments[0];
                var deserializedArc = deserializedPolylineData.ArcSegments[0];

                Assert.Equal(originalArc.Radius, deserializedArc.Radius);
                Assert.Equal(originalArc.StartAngle, deserializedArc.StartAngle);
                Assert.Equal(originalArc.EndAngle, deserializedArc.EndAngle);
                Assert.Equal(originalArc.SweepAngle, deserializedArc.SweepAngle);
                Assert.Equal(originalArc.IsCounterClockwise, deserializedArc.IsCounterClockwise);
            }
        }

        [Fact]
        public void RenderPolyline2DWithMultipleArcSegments_PreservesAllArcData()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.Vertices[0].Bulge = 1.0;
            polyline.Vertices[1].Bulge = 0.5;

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.True(polylineData.HasArcSegments);
            Assert.Equal(2, polylineData.ArcSegments.Count);
        }

        [Fact]
        public void RenderPolyline2D_CalculatesCorrectGeometryData()
        {
            var polyline = CreateBasicPolyline2D();

            var polylineData = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.Geometry);
            Assert.Equal("Polyline2D", polylineData.Geometry.Type);
            Assert.Equal(3, polylineData.Geometry.VertexCount);
            Assert.Equal(0, polylineData.Geometry.FaceCount);
            Assert.True(polylineData.Geometry.HasNormals);
            Assert.True(polylineData.Geometry.HasUVs);
            Assert.True(polylineData.Geometry.HasIndices);
        }
    }
}
