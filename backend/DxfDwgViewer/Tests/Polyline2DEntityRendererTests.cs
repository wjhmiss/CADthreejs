using System;
using Xunit;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using DxfDwgViewer.RenderUtilities;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class Polyline2DEntityRendererTests
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
        public void Render_Polyline2D_ReturnsCorrectType()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal("Polyline2D", result.EntityType);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectVertexCount()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(3, result.VertexCount);
            Assert.Equal(3, result.Vertices.Count);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectSegmentCount()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(2, result.SegmentCount);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectVertices()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(3, result.Vertices.Count);

            Assert.Equal(0, result.Vertices[0].Location.X);
            Assert.Equal(0, result.Vertices[0].Location.Y);
            Assert.Equal(0, result.Vertices[0].Location.Z);
            Assert.Equal(0, result.Vertices[0].Bulge);

            Assert.Equal(10, result.Vertices[1].Location.X);
            Assert.Equal(0, result.Vertices[1].Location.Y);
            Assert.Equal(0, result.Vertices[1].Location.Z);
            Assert.Equal(0, result.Vertices[1].Bulge);

            Assert.Equal(10, result.Vertices[2].Location.X);
            Assert.Equal(10, result.Vertices[2].Location.Y);
            Assert.Equal(0, result.Vertices[2].Location.Z);
            Assert.Equal(0, result.Vertices[2].Bulge);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectVertices3D()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(3, result.Vertices3D.Count);

            Assert.Equal(0, result.Vertices3D[0].X);
            Assert.Equal(0, result.Vertices3D[0].Y);
            Assert.Equal(0, result.Vertices3D[0].Z);

            Assert.Equal(10, result.Vertices3D[1].X);
            Assert.Equal(0, result.Vertices3D[1].Y);
            Assert.Equal(0, result.Vertices3D[1].Z);

            Assert.Equal(10, result.Vertices3D[2].X);
            Assert.Equal(10, result.Vertices3D[2].Y);
            Assert.Equal(0, result.Vertices3D[2].Z);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectTotalLength()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(20, result.TotalLength);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectBounds3D()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds3D);

            Assert.Equal(0, result.Bounds3D.Min.X);
            Assert.Equal(0, result.Bounds3D.Min.Y);
            Assert.Equal(0, result.Bounds3D.Min.Z);

            Assert.Equal(10, result.Bounds3D.Max.X);
            Assert.Equal(10, result.Bounds3D.Max.Y);
            Assert.Equal(0, result.Bounds3D.Max.Z);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectBounds()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);

            Assert.Equal(0, result.Bounds.MinX);
            Assert.Equal(10, result.Bounds.MaxX);
            Assert.Equal(0, result.Bounds.MinY);
            Assert.Equal(10, result.Bounds.MaxY);
            Assert.Equal(0, result.Bounds.MinZ);
            Assert.Equal(0, result.Bounds.MaxZ);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectCentroid()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);

            Assert.Equal(20.0 / 3.0, result.Centroid.X, 2);
            Assert.Equal(10.0 / 3.0, result.Centroid.Y, 2);
            Assert.Equal(0, result.Centroid.Z);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectIsClosed()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.IsClosed = true;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.True(result.IsClosed);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectColorIndex()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.Color = new Color(5);
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectLineTypeScale()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.LineTypeScale = 2.5;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(2.5, result.LineTypeScale);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectElevation()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.Elevation = 5.5;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(5.5, result.Elevation);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectThickness()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.Thickness = 3.0;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(3.0, result.Thickness);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectStartWidth()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.StartWidth = 1.5;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(1.5, result.StartWidth);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectEndWidth()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.EndWidth = 2.0;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(2.0, result.EndWidth);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectSmoothSurface()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.SmoothSurface = SmoothSurfaceType.Quadratic;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(SmoothSurfaceType.Quadratic, result.SmoothSurface);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectIndices()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(4, result.Indices.Count);
            Assert.Equal(0, result.Indices[0]);
            Assert.Equal(1, result.Indices[1]);
            Assert.Equal(1, result.Indices[2]);
            Assert.Equal(2, result.Indices[3]);
        }

        [Fact]
        public void Render_Polyline2D_Closed_ReturnsCorrectIndices()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.IsClosed = true;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(6, result.Indices.Count);
            Assert.Equal(0, result.Indices[0]);
            Assert.Equal(1, result.Indices[1]);
            Assert.Equal(1, result.Indices[2]);
            Assert.Equal(2, result.Indices[3]);
            Assert.Equal(2, result.Indices[4]);
            Assert.Equal(0, result.Indices[5]);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectNormalsArray()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(9, result.NormalsArray.Count);

            Assert.Equal(0, result.NormalsArray[0]);
            Assert.Equal(0, result.NormalsArray[1]);
            Assert.Equal(1, result.NormalsArray[2]);

            Assert.Equal(0, result.NormalsArray[3]);
            Assert.Equal(0, result.NormalsArray[4]);
            Assert.Equal(1, result.NormalsArray[5]);

            Assert.Equal(0, result.NormalsArray[6]);
            Assert.Equal(0, result.NormalsArray[7]);
            Assert.Equal(1, result.NormalsArray[8]);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectUVsArray()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(6, result.UVsArray.Count);

            Assert.Equal(0, result.UVsArray[0]);
            Assert.Equal(0, result.UVsArray[1]);

            Assert.Equal(0.5, result.UVsArray[2], 2);
            Assert.Equal(0, result.UVsArray[3]);

            Assert.Equal(1, result.UVsArray[4], 2);
            Assert.Equal(0, result.UVsArray[5]);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectHandle()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(polyline.Handle.ToString(), result.Handle);
        }

        [Fact]
        public void Render_Polyline2D_WithVariableWidth_ReturnsTrue()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.Vertices[0].StartWidth = 1.0;
            polyline.Vertices[0].EndWidth = 2.0;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.True(result.HasVariableWidth);
        }

        [Fact]
        public void Render_Polyline2D_WithoutVariableWidth_ReturnsFalse()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.False(result.HasVariableWidth);
        }

        [Fact]
        public void Render_Polyline2D_WithArcSegments_ReturnsTrue()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.Vertices[0].Bulge = 1.0;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.True(result.HasArcSegments);
        }

        [Fact]
        public void Render_Polyline2D_WithoutArcSegments_ReturnsFalse()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.False(result.HasArcSegments);
        }

        [Fact]
        public void Render_Polyline2D_WithArcSegments_ReturnsCorrectArcSegments()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.Vertices[0].Bulge = 1.0;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.True(result.HasArcSegments);
            Assert.Single(result.ArcSegments);

            var arcSegment = result.ArcSegments[0];
            Assert.NotNull(arcSegment.StartPoint);
            Assert.NotNull(arcSegment.EndPoint);
            Assert.NotNull(arcSegment.CenterPoint);
            Assert.True(arcSegment.Radius > 0);
            Assert.True(arcSegment.IsCounterClockwise);
        }

        [Fact]
        public void Render_Polyline2D_Closed_WithArcSegments_ReturnsCorrectSegmentCount()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.IsClosed = true;
            polyline.Vertices[2].Bulge = 0.5;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(3, result.SegmentCount);
        }

        [Fact]
        public void Render_Polyline2D_WithNegativeBulge_ReturnsCorrectArcDirection()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.Vertices[0].Bulge = -1.0;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.True(result.HasArcSegments);
            Assert.Single(result.ArcSegments);

            var arcSegment = result.ArcSegments[0];
            Assert.False(arcSegment.IsCounterClockwise);
        }

        [Fact]
        public void Render_Polyline2D_EmptyVertices_ReturnsEmptyData()
        {
            var polyline = new Polyline2D
            {
                IsClosed = false,
                Color = new Color(1)
            };

            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(0, result.VertexCount);
            Assert.Equal(0, result.SegmentCount);
            Assert.Empty(result.Vertices);
            Assert.Empty(result.Vertices3D);
            Assert.Equal(0, result.TotalLength);
        }

        [Fact]
        public void Render_Polyline2D_SingleVertex_ReturnsZeroSegments()
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

            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(1, result.VertexCount);
            Assert.Equal(0, result.SegmentCount);
            Assert.Equal(0, result.TotalLength);
        }

        [Fact]
        public void Render_Polyline2D_Closed_SingleVertex_ReturnsZeroSegments()
        {
            var polyline = new Polyline2D
            {
                IsClosed = true,
                Color = new Color(1)
            };

            polyline.Vertices.Add(new Vertex2D
            {
                Location = new XY(5, 5),
                Bulge = 0,
                StartWidth = 0,
                EndWidth = 0
            });

            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(1, result.VertexCount);
            Assert.Equal(0, result.SegmentCount);
            Assert.Equal(0, result.TotalLength);
        }

        [Fact]
        public void Render_Polyline2D_WithElevation_ReturnsCorrectZValues()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.Elevation = 10.0;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(10.0, result.Elevation);

            Assert.Equal(10.0, result.Vertices3D[0].Z);
            Assert.Equal(10.0, result.Vertices3D[1].Z);
            Assert.Equal(10.0, result.Vertices3D[2].Z);
        }

        [Fact]
        public void Render_Polyline2D_Closed_ReturnsCorrectTotalLength()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.IsClosed = true;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(30, result.TotalLength);
        }

        [Fact]
        public void Render_Polyline2D_Closed_WithArcSegments_ReturnsCorrectTotalLength()
        {
            var polyline = CreateBasicPolyline2D();
            polyline.IsClosed = true;
            polyline.Vertices[2].Bulge = 1.0;
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.True(result.TotalLength > 20);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectVisible()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.True(result.Visible);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectGeometryType()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal("Polyline2D", result.GeometryType);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectVertexPositions()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(9, result.VertexPositions.Count);

            Assert.Equal(0, result.VertexPositions[0]);
            Assert.Equal(0, result.VertexPositions[1]);
            Assert.Equal(0, result.VertexPositions[2]);

            Assert.Equal(10, result.VertexPositions[3]);
            Assert.Equal(0, result.VertexPositions[4]);
            Assert.Equal(0, result.VertexPositions[5]);

            Assert.Equal(10, result.VertexPositions[6]);
            Assert.Equal(10, result.VertexPositions[7]);
            Assert.Equal(0, result.VertexPositions[8]);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectVertexNormals()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(9, result.VertexNormals.Count);

            Assert.Equal(0, result.VertexNormals[0]);
            Assert.Equal(0, result.VertexNormals[1]);
            Assert.Equal(1, result.VertexNormals[2]);

            Assert.Equal(0, result.VertexNormals[3]);
            Assert.Equal(0, result.VertexNormals[4]);
            Assert.Equal(1, result.VertexNormals[5]);

            Assert.Equal(0, result.VertexNormals[6]);
            Assert.Equal(0, result.VertexNormals[7]);
            Assert.Equal(1, result.VertexNormals[8]);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectVertexUVs()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(6, result.VertexUVs.Count);

            Assert.Equal(0, result.VertexUVs[0]);
            Assert.Equal(0, result.VertexUVs[1]);

            Assert.Equal(0.5, result.VertexUVs[2], 2);
            Assert.Equal(0, result.VertexUVs[3]);

            Assert.Equal(1, result.VertexUVs[4], 2);
            Assert.Equal(0, result.VertexUVs[5]);
        }

        [Fact]
        public void Render_Polyline2D_ReturnsCorrectGeometryData()
        {
            var polyline = CreateBasicPolyline2D();
            var result = Polyline2DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);
            Assert.Equal("Polyline2D", result.Geometry.Type);
            Assert.Equal(3, result.Geometry.VertexCount);
            Assert.Equal(0, result.Geometry.FaceCount);
            Assert.True(result.Geometry.HasNormals);
            Assert.True(result.Geometry.HasUVs);
            Assert.True(result.Geometry.HasIndices);
        }
    }
}
