using System;
using System.Collections.Generic;
using Xunit;
using ACadSharp.Entities;
using CSMath;
using DxfDwgViewer.RenderUtilities;

namespace DxfDwgViewer.Tests
{
    public class LwPolylineEntityRendererIntegrationTests
    {
        [Fact]
        public void Render_LwPolyline_WithFullData_ReturnsCompleteData()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)) { Bulge = 0, StartWidth = 0.5, EndWidth = 0.5 });
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)) { Bulge = 0, StartWidth = 0.5, EndWidth = 0.5 });
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 10)) { Bulge = 0, StartWidth = 0.5, EndWidth = 0.5 });
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 10)) { Bulge = 0, StartWidth = 0.5, EndWidth = 0.5 });
            lwPolyline.IsClosed = true;
            lwPolyline.Elevation = 5;
            lwPolyline.ConstantWidth = 1.0;
            lwPolyline.Thickness = 2.0;
            lwPolyline.Normal = new XYZ(0, 0, 1);
            lwPolyline.LineTypeScale = 1.5;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result);
            Assert.Equal("LwPolyline", result.Type);
            Assert.Equal("LwPolyline", result.EntityType);
            Assert.NotNull(result.Handle);
            Assert.Equal(4, result.VertexCount);
            Assert.True(result.IsClosed);
            Assert.Equal(5, result.Elevation);
            Assert.Equal(1.0, result.ConstantWidth);
            Assert.Equal(2.0, result.Thickness);
            Assert.Equal(1.5, result.LineTypeScale);
        }

        [Fact]
        public void Render_LwPolyline_WithBulgeArc_CalculatesCorrectVertices()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)) { Bulge = 1 });
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)) { Bulge = 0 });
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 10)) { Bulge = 0 });
            lwPolyline.IsClosed = false;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Vertices);
            Assert.Equal(3, result.Vertices.Count);
            Assert.Equal(1, result.Vertices[0].Bulge);
            Assert.Equal(0, result.Vertices[1].Bulge);
            Assert.Equal(0, result.Vertices[2].Bulge);
        }

        [Fact]
        public void Render_LwPolyline_WithVariableWidth_CalculatesCorrectVertices()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)) { StartWidth = 0.25, EndWidth = 0.5 });
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)) { StartWidth = 0.5, EndWidth = 0.75 });
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 10)) { StartWidth = 0.75, EndWidth = 1.0 });
            lwPolyline.IsClosed = false;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Vertices);
            Assert.Equal(3, result.Vertices.Count);
            Assert.Equal(0.25, result.Vertices[0].StartWidth);
            Assert.Equal(0.5, result.Vertices[0].EndWidth);
            Assert.Equal(0.5, result.Vertices[1].StartWidth);
            Assert.Equal(0.75, result.Vertices[1].EndWidth);
            Assert.Equal(0.75, result.Vertices[2].StartWidth);
            Assert.Equal(1.0, result.Vertices[2].EndWidth);
        }

        [Fact]
        public void Render_LwPolyline_With3DCoordinates_CalculatesCorrectVertices3D()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 10)));
            lwPolyline.Elevation = 5;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Vertices3D);
            Assert.Equal(3, result.Vertices3D.Count);
            Assert.Equal(0, result.Vertices3D[0].X);
            Assert.Equal(0, result.Vertices3D[0].Y);
            Assert.Equal(5, result.Vertices3D[0].Z);
            Assert.Equal(10, result.Vertices3D[1].X);
            Assert.Equal(0, result.Vertices3D[1].Y);
            Assert.Equal(5, result.Vertices3D[1].Z);
            Assert.Equal(10, result.Vertices3D[2].X);
            Assert.Equal(10, result.Vertices3D[2].Y);
            Assert.Equal(5, result.Vertices3D[2].Z);
        }

        [Fact]
        public void Render_LwPolyline_WithClosedPolyline_CalculatesCorrectTotalLength()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 10)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 10)));
            lwPolyline.IsClosed = true;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(40, result.TotalLength);
        }

        [Fact]
        public void Render_LwPolyline_WithOpenPolyline_CalculatesCorrectTotalLength()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 10)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 10)));
            lwPolyline.IsClosed = false;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(30, result.TotalLength);
        }

        [Fact]
        public void Render_LwPolyline_WithMultipleVertices_CalculatesCorrectCentroid()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(20, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(20, 20)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 20)));
            lwPolyline.IsClosed = true;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(10, result.Centroid.X);
            Assert.Equal(10, result.Centroid.Y);
        }

        [Fact]
        public void Render_LwPolyline_WithElevation_CalculatesCorrectCentroid3D()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 10)));
            lwPolyline.Elevation = 10;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(6.666666666666667, result.Centroid3D.X);
            Assert.Equal(3.3333333333333335, result.Centroid3D.Y);
            Assert.Equal(10, result.Centroid3D.Z);
        }

        [Fact]
        public void Render_LwPolyline_CalculatesCorrectBounds()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 10)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 10)));
            lwPolyline.Elevation = 5;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(5, result.Bounds.Min.Z);
            Assert.Equal(10, result.Bounds.Max.X);
            Assert.Equal(10, result.Bounds.Max.Y);
            Assert.Equal(5, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_LwPolyline_WithNormal_SetsCorrectNormal()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            lwPolyline.Normal = new XYZ(0.5773502691896258, 0.5773502691896258, 0.5773502691896258);

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(0.5773502691896258, result.Normal.X);
            Assert.Equal(0.5773502691896258, result.Normal.Y);
            Assert.Equal(0.5773502691896258, result.Normal.Z);
        }

        [Fact]
        public void Render_LwPolyline_WithTransform_SetsCorrectTransform()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Position);
            Assert.NotNull(result.Transform.Rotation);
            Assert.NotNull(result.Transform.Scale);
            Assert.NotNull(result.Transform.Matrix);
        }

        [Fact]
        public void Render_LwPolyline_WithColor_SetsCorrectColor()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            lwPolyline.Color = new ACadSharp.Color(1);

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Color);
            Assert.Equal(1, result.Color.Index);
            Assert.Equal("#FF0000", result.Color.Hex);
            Assert.Equal(255, result.Color.R);
            Assert.Equal(0, result.Color.G);
            Assert.Equal(0, result.Color.B);
        }

        [Fact]
        public void Render_LwPolyline_SetsDefaultMaterialProperties()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(1.0, result.Opacity);
            Assert.False(result.Transparent);
            Assert.Equal("LineBasicMaterial", result.MaterialType);
            Assert.True(result.DepthTest);
            Assert.True(result.DepthWrite);
        }

        [Fact]
        public void Render_LwPolyline_WithComplexShape_CalculatesCorrectGeometry()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 5)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(5, 5)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(5, 10)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 10)));
            lwPolyline.IsClosed = true;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(6, result.VertexCount);
            Assert.Equal(40, result.TotalLength);
        }

        [Fact]
        public void Render_LwPolyline_WithVeryLargeCoordinates_CalculatesCorrectGeometry()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(1000000, 1000000)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(2000000, 1000000)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(2000000, 2000000)));
            lwPolyline.IsClosed = true;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(3414213.5623730952, result.TotalLength);
            Assert.Equal(1666666.6666666667, result.Centroid.X);
            Assert.Equal(1333333.3333333333, result.Centroid.Y);
        }

        [Fact]
        public void Render_LwPolyline_WithNegativeCoordinates_CalculatesCorrectBounds()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(-10, -10)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, -10)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 10)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(-10, 10)));
            lwPolyline.Elevation = -5;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(-10, result.Bounds.Min.X);
            Assert.Equal(-10, result.Bounds.Min.Y);
            Assert.Equal(-5, result.Bounds.Min.Z);
            Assert.Equal(10, result.Bounds.Max.X);
            Assert.Equal(10, result.Bounds.Max.Y);
            Assert.Equal(-5, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_LwPolyline_WithDirectionVectors_CalculatesCorrectDirections()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 10)));
            lwPolyline.IsClosed = true;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Direction);
            Assert.Equal(3, result.Direction.Count);
            Assert.Equal(1, result.Direction[0].X);
            Assert.Equal(0, result.Direction[0].Y);
            Assert.Equal(0, result.Direction[0].Z);
            Assert.Equal(0, result.Direction[1].X);
            Assert.Equal(1, result.Direction[1].Y);
            Assert.Equal(0, result.Direction[1].Z);
        }

        [Fact]
        public void Render_LwPolyline_WithPoints_CalculatesCorrectPoints()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 10)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 10)));
            lwPolyline.IsClosed = true;

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
        public void Render_LwPolyline_WithLineTypeName_SetsCorrectProperty()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.LineTypeName);
        }

        [Fact]
        public void Render_LwPolyline_WithLineWeight_SetsCorrectProperty()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(0.03, result.LineWeight);
        }

        [Fact]
        public void Render_LwPolyline_WithMultipleBulges_CalculatesCorrectVertices()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)) { Bulge = 0.5 });
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)) { Bulge = -0.5 });
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 10)) { Bulge = 0 });

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.NotNull(result.Vertices);
            Assert.Equal(3, result.Vertices.Count);
            Assert.Equal(0.5, result.Vertices[0].Bulge);
            Assert.Equal(-0.5, result.Vertices[1].Bulge);
            Assert.Equal(0, result.Vertices[2].Bulge);
        }

        [Fact]
        public void Render_LwPolyline_WithZeroConstantWidth_SetsCorrectProperty()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            lwPolyline.ConstantWidth = 0;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(0, result.ConstantWidth);
        }

        [Fact]
        public void Render_LwPolyline_WithZeroThickness_SetsCorrectProperty()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            lwPolyline.Thickness = 0;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(0, result.Thickness);
        }

        [Fact]
        public void Render_LwPolyline_WithZeroElevation_SetsCorrectProperty()
        {
            var lwPolyline = new LwPolyline();
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(0, 0)));
            lwPolyline.Vertices.Add(new LwPolyline.Vertex(new XY(10, 0)));
            lwPolyline.Elevation = 0;

            var result = LwPolylineEntityRenderer.Render(lwPolyline);

            Assert.Equal(0, result.Elevation);
        }
    }
}
