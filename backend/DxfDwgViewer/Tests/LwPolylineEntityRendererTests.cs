using System;
using System.Collections.Generic;
using Xunit;
using ACadSharp.Entities;
using CSMath;
using DxfDwgViewer.RenderUtilities;

namespace DxfDwgViewer.Tests
{
    public class LwPolylineEntityRendererTests
    {
        private LwPolyline CreateBasicLwPolyline()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 10)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 10)));
            lwPolyline.IsClosed = true;
            lwPolyline.Elevation = 0;
            lwPolyline.ConstantWidth = 0;
            lwPolyline.Thickness = 0;
            return lwPolyline;
        }

        [Fact]
        public void Render_LwPolyline_ReturnsCorrectData()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result);
            Assert.Equal("LwPolyline", result.Type);
            Assert.Equal("LwPolyline", result.EntityType);
            Assert.Equal(4, result.VertexCount);
            Assert.True(result.IsClosed);
        }

        [Fact]
        public void Render_LwPolyline_WithVertices_CalculatesCorrectPoints()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Points);
            Assert.Equal(4, result.Points.Count);
            Assert.Equal(0, result.Points[0].X);
            Assert.Equal(0, result.Points[0].Y);
            Assert.Equal(10, result.Points[1].X);
            Assert.Equal(0, result.Points[1].Y);
            Assert.Equal(10, result.Points[2].X);
            Assert.Equal(10, result.Points[2].Y);
            Assert.Equal(0, result.Points[3].X);
            Assert.Equal(10, result.Points[3].Y);
        }

        [Fact]
        public void Render_LwPolyline_WithVertices_CalculatesCorrect3DVertices()
        {
            var lwPolyline = CreateBasicLwPolyline();
            lwPolyline.Elevation = 5;
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Vertices3D);
            Assert.Equal(4, result.Vertices3D.Count);
            Assert.Equal(0, result.Vertices3D[0].X);
            Assert.Equal(0, result.Vertices3D[0].Y);
            Assert.Equal(5, result.Vertices3D[0].Z);
            Assert.Equal(10, result.Vertices3D[1].X);
            Assert.Equal(0, result.Vertices3D[1].Y);
            Assert.Equal(5, result.Vertices3D[1].Z);
        }

        [Fact]
        public void Render_LwPolyline_WithClosedFlag_CalculatesCorrectLength()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(40, result.TotalLength);
        }

        [Fact]
        public void Render_LwPolyline_WithOpenFlag_CalculatesCorrectLength()
        {
            var lwPolyline = CreateBasicLwPolyline();
            lwPolyline.IsClosed = false;
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(30, result.TotalLength);
        }

        [Fact]
        public void Render_LwPolyline_CalculatesCorrectCentroid()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Centroid);
            Assert.Equal(5, result.Centroid.X);
            Assert.Equal(5, result.Centroid.Y);
        }

        [Fact]
        public void Render_LwPolyline_WithElevation_CalculatesCorrectCentroid3D()
        {
            var lwPolyline = CreateBasicLwPolyline();
            lwPolyline.Elevation = 10;
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Centroid3D);
            Assert.Equal(5, result.Centroid3D.X);
            Assert.Equal(5, result.Centroid3D.Y);
            Assert.Equal(10, result.Centroid3D.Z);
        }

        [Fact]
        public void Render_LwPolyline_CalculatesCorrectBounds()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Bounds);
            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(0, result.Bounds.Min.Z);
            Assert.Equal(10, result.Bounds.Max.X);
            Assert.Equal(10, result.Bounds.Max.Y);
            Assert.Equal(0, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_LwPolyline_WithElevation_CalculatesCorrectBounds()
        {
            var lwPolyline = CreateBasicLwPolyline();
            lwPolyline.Elevation = 5;
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Bounds);
            Assert.Equal(5, result.Bounds.Min.Z);
            Assert.Equal(5, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_LwPolyline_WithBulge_CalculatesCorrectVertices()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)) { Bulge = 1 });
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Vertices);
            Assert.Equal(2, result.Vertices.Count);
            Assert.Equal(1, result.Vertices[0].Bulge);
            Assert.Equal(0, result.Vertices[1].Bulge);
        }

        [Fact]
        public void Render_LwPolyline_WithVariableWidth_CalculatesCorrectVertices()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)) { StartWidth = 0.5, EndWidth = 1.0 });
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)) { StartWidth = 1.0, EndWidth = 0.5 });
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Vertices);
            Assert.Equal(2, result.Vertices.Count);
            Assert.Equal(0.5, result.Vertices[0].StartWidth);
            Assert.Equal(1.0, result.Vertices[0].EndWidth);
            Assert.Equal(1.0, result.Vertices[1].StartWidth);
            Assert.Equal(0.5, result.Vertices[1].EndWidth);
        }

        [Fact]
        public void Render_LwPolyline_WithConstantWidth_SetsCorrectProperty()
        {
            var lwPolyline = CreateBasicLwPolyline();
            lwPolyline.ConstantWidth = 2.0;
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(2.0, result.ConstantWidth);
        }

        [Fact]
        public void Render_LwPolyline_WithThickness_SetsCorrectProperty()
        {
            var lwPolyline = CreateBasicLwPolyline();
            lwPolyline.Thickness = 1.5;
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(1.5, result.Thickness);
        }

        [Fact]
        public void Render_LwPolyline_WithElevation_SetsCorrectProperty()
        {
            var lwPolyline = CreateBasicLwPolyline();
            lwPolyline.Elevation = 3.0;
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(3.0, result.Elevation);
        }

        [Fact]
        public void Render_LwPolyline_WithDefaultLineWeight_ReturnsDefaultWeight()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(0.03, result.LineWeight);
        }

        [Fact]
        public void Render_LwPolyline_WithLineTypeScale_SetsCorrectProperty()
        {
            var lwPolyline = CreateBasicLwPolyline();
            lwPolyline.LineTypeScale = 2.0;
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(2.0, result.LineTypeScale);
        }

        [Fact]
        public void Render_LwPolyline_WithNormal_SetsCorrectProperty()
        {
            var lwPolyline = CreateBasicLwPolyline();
            lwPolyline.Normal = new XYZ(0, 0, 1);
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Normal);
            Assert.Equal(0, result.Normal.X);
            Assert.Equal(0, result.Normal.Y);
            Assert.Equal(1, result.Normal.Z);
        }

        [Fact]
        public void Render_LwPolyline_WithEmptyVertices_ReturnsZeroLength()
        {
            var lwPolyline = new LwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(0, result.TotalLength);
            Assert.Equal(0, result.VertexCount);
        }

        [Fact]
        public void Render_LwPolyline_WithSingleVertex_ReturnsZeroLength()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(5, 5)));
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(0, result.TotalLength);
            Assert.Equal(1, result.VertexCount);
        }

        [Fact]
        public void Render_LwPolyline_WithDiagonalLine_CalculatesCorrectLength()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(3, 4)));
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(5, result.TotalLength);
        }

        [Fact]
        public void Render_LwPolyline_WithNegativeCoordinates_CalculatesCorrectCentroid()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(-10, -10)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, -10)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 10)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(-10, 10)));
            lwPolyline.IsClosed = true;
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(0, result.Centroid.X);
            Assert.Equal(0, result.Centroid.Y);
        }

        [Fact]
        public void Render_LwPolyline_WithVerySmallCoordinates_CalculatesCorrectLength()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0.0001, 0.0001)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0.0002, 0.0002)));
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.InRange(result.TotalLength, 0.00014, 0.00015);
        }

        [Fact]
        public void Render_LwPolyline_SetsCorrectType()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal("LwPolyline", result.Type);
        }

        [Fact]
        public void Render_LwPolyline_SetsCorrectEntityType()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal("LwPolyline", result.EntityType);
        }

        [Fact]
        public void Render_LwPolyline_SetsDefaultVisible()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.True(result.Visible);
        }

        [Fact]
        public void Render_LwPolyline_SetsDefaultCoordinateSystem()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal("World", result.CoordinateSystem);
        }

        [Fact]
        public void Render_LwPolyline_SetsDefaultOpacity()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(1.0, result.Opacity);
        }

        [Fact]
        public void Render_LwPolyline_SetsDefaultTransparent()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.False(result.Transparent);
        }

        [Fact]
        public void Render_LwPolyline_SetsDefaultMaterialType()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal("LineBasicMaterial", result.MaterialType);
        }

        [Fact]
        public void Render_LwPolyline_SetsDefaultDepthTest()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.True(result.DepthTest);
        }

        [Fact]
        public void Render_LwPolyline_SetsDefaultDepthWrite()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.True(result.DepthWrite);
        }

        [Fact]
        public void Render_LwPolyline_CalculatesDirectionVectors()
        {
            var lwPolyline = CreateBasicLwPolyline();
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Direction);
            Assert.Equal(4, result.Direction.Count);
            Assert.Equal(1, result.Direction[0].X);
            Assert.Equal(0, result.Direction[0].Y);
            Assert.Equal(0, result.Direction[0].Z);
        }

        [Fact]
        public void Render_LwPolyline_WithOpenPolyline_CalculatesDirectionVectors()
        {
            var lwPolyline = CreateBasicLwPolyline();
            lwPolyline.IsClosed = false;
            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Direction);
            Assert.Equal(3, result.Direction.Count);
        }
    }
}
