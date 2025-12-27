using System;
using Xunit;
using ACadSharp.Entities;
using ACadSharp.Tables;
using ACadSharp;
using CSMath;
using DxfDwgViewer.RenderUtilities;

namespace DxfDwgViewer.Tests
{
    public class SolidEntityRendererTests
    {
        private Solid CreateBasicTriangleSolid()
        {
            var solid = new Solid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(1, 0, 0);
            solid.ThirdCorner = new XYZ(0, 1, 0);
            solid.FourthCorner = new XYZ(0, 1, 0);
            solid.Normal = new XYZ(0, 0, 1);
            solid.Color = new Color(256);
            solid.Thickness = 0.0;
            return solid;
        }

        private Solid CreateBasicQuadSolid()
        {
            var solid = new Solid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(1, 0, 0);
            solid.ThirdCorner = new XYZ(1, 1, 0);
            solid.FourthCorner = new XYZ(0, 1, 0);
            solid.Normal = new XYZ(0, 0, 1);
            solid.Color = new Color(256);
            solid.Thickness = 0.0;
            return solid;
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectType()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.True(result.IsTriangle);
            Assert.False(result.HasFourthCorner);
        }

        [Fact]
        public void Render_QuadSolid_ReturnsCorrectType()
        {
            var solid = CreateBasicQuadSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.False(result.IsTriangle);
            Assert.True(result.HasFourthCorner);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectPoints()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Points);
            Assert.Equal(3, result.Points.Count);
            Assert.Equal(3, result.VertexCount);

            Assert.Equal(0.0, result.Points[0].X);
            Assert.Equal(0.0, result.Points[0].Y);
            Assert.Equal(0.0, result.Points[0].Z);

            Assert.Equal(1.0, result.Points[1].X);
            Assert.Equal(0.0, result.Points[1].Y);
            Assert.Equal(0.0, result.Points[1].Z);

            Assert.Equal(0.0, result.Points[2].X);
            Assert.Equal(1.0, result.Points[2].Y);
            Assert.Equal(0.0, result.Points[2].Z);
        }

        [Fact]
        public void Render_QuadSolid_ReturnsCorrectPoints()
        {
            var solid = CreateBasicQuadSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Points);
            Assert.Equal(4, result.Points.Count);
            Assert.Equal(4, result.VertexCount);

            Assert.Equal(0.0, result.Points[0].X);
            Assert.Equal(0.0, result.Points[0].Y);

            Assert.Equal(1.0, result.Points[1].X);
            Assert.Equal(0.0, result.Points[1].Y);

            Assert.Equal(0.0, result.Points[2].X);
            Assert.Equal(1.0, result.Points[2].Y);

            Assert.Equal(1.0, result.Points[3].X);
            Assert.Equal(1.0, result.Points[3].Y);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectColorIndex()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Color = new Color(1);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(1, result.ColorIndex);
        }

        [Fact]
        public void Render_QuadSolid_ReturnsCorrectColorIndex()
        {
            var solid = CreateBasicQuadSolid();
            solid.Color = new Color(5);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectLineTypeName()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal("", result.LineTypeName);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectLineWeight()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(0.0, result.LineWeight);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectLineTypeScale()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(1.0, result.LineTypeScale);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectNormal()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Normal = new XYZ(0.5, 0.5, 0.7071);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(0.5, result.Normal.X, 4);
            Assert.Equal(0.5, result.Normal.Y, 4);
            Assert.Equal(0.7071, result.Normal.Z, 4);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectThickness()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Thickness = 10.0;
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(10.0, result.Thickness);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectHasFourthCorner()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.False(result.HasFourthCorner);
        }

        [Fact]
        public void Render_QuadSolid_ReturnsCorrectHasFourthCorner()
        {
            var solid = CreateBasicQuadSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.True(result.HasFourthCorner);
        }

        [Fact]
        public void Render_TriangleSolid_CalculatesBoundsCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);

            Assert.Equal(0.0, result.Bounds.Min.X);
            Assert.Equal(0.0, result.Bounds.Min.Y);
            Assert.Equal(0.0, result.Bounds.Min.Z);

            Assert.Equal(1.0, result.Bounds.Max.X);
            Assert.Equal(1.0, result.Bounds.Max.Y);
            Assert.Equal(0.0, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_QuadSolid_CalculatesBoundsCorrectly()
        {
            var solid = CreateBasicQuadSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);

            Assert.Equal(0.0, result.Bounds.Min.X);
            Assert.Equal(0.0, result.Bounds.Min.Y);
            Assert.Equal(0.0, result.Bounds.Min.Z);

            Assert.Equal(1.0, result.Bounds.Max.X);
            Assert.Equal(1.0, result.Bounds.Max.Y);
            Assert.Equal(0.0, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_TriangleSolid_CalculatesCentroidCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);

            Assert.Equal(1.0 / 3.0, result.Centroid.X, 4);
            Assert.Equal(1.0 / 3.0, result.Centroid.Y, 4);
            Assert.Equal(0.0, result.Centroid.Z);
        }

        [Fact]
        public void Render_QuadSolid_CalculatesCentroidCorrectly()
        {
            var solid = CreateBasicQuadSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);

            Assert.Equal(0.5, result.Centroid.X);
            Assert.Equal(0.5, result.Centroid.Y);
            Assert.Equal(0.0, result.Centroid.Z);
        }

        [Fact]
        public void Render_TriangleSolid_CalculatesAreaCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(0.5, result.Area);
        }

        [Fact]
        public void Render_QuadSolid_CalculatesAreaCorrectly()
        {
            var solid = CreateBasicQuadSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(1.0, result.Area);
        }

        [Fact]
        public void Render_TriangleSolid_CalculatesPerimeterCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(1.0 + Math.Sqrt(2.0) + 1.0, result.Perimeter, 4);
        }

        [Fact]
        public void Render_QuadSolid_CalculatesPerimeterCorrectly()
        {
            var solid = CreateBasicQuadSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(4.0, result.Perimeter);
        }

        [Fact]
        public void Render_TriangleSolid_CreatesTransformCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Position);
            Assert.NotNull(result.Transform.Rotation);
            Assert.NotNull(result.Transform.Scale);

            Assert.Equal(1.0 / 3.0, result.Transform.Position.X, 4);
            Assert.Equal(1.0 / 3.0, result.Transform.Position.Y, 4);
            Assert.Equal(0.0, result.Transform.Position.Z);

            Assert.Equal(0.0, result.Transform.Rotation.X);
            Assert.Equal(0.0, result.Transform.Rotation.Y);
            Assert.Equal(0.0, result.Transform.Rotation.Z);

            Assert.Equal(1.0, result.Transform.Scale.X);
            Assert.Equal(1.0, result.Transform.Scale.Y);
            Assert.Equal(1.0, result.Transform.Scale.Z);
        }

        [Fact]
        public void Render_TriangleSolid_WithThickness_CreatesTransformCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Thickness = 10.0;
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);

            Assert.Equal(10.0, result.Transform.Scale.Z);
        }

        [Fact]
        public void Render_TriangleSolid_CreatesTransformMatrixCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform.Matrix);
            Assert.Equal(16, result.Transform.Matrix.Length);

            Assert.Equal(1.0, result.Transform.Matrix[0]);
            Assert.Equal(0.0, result.Transform.Matrix[1]);
            Assert.Equal(0.0, result.Transform.Matrix[2]);
            Assert.Equal(0.0, result.Transform.Matrix[3]);

            Assert.Equal(0.0, result.Transform.Matrix[4]);
            Assert.Equal(1.0, result.Transform.Matrix[5]);
            Assert.Equal(0.0, result.Transform.Matrix[6]);
            Assert.Equal(0.0, result.Transform.Matrix[7]);

            Assert.Equal(0.0, result.Transform.Matrix[8]);
            Assert.Equal(0.0, result.Transform.Matrix[9]);
            Assert.Equal(1.0, result.Transform.Matrix[10]);
            Assert.Equal(0.0, result.Transform.Matrix[11]);

            Assert.Equal(1.0 / 3.0, result.Transform.Matrix[12], 4);
            Assert.Equal(1.0 / 3.0, result.Transform.Matrix[13], 4);
            Assert.Equal(0.0, result.Transform.Matrix[14]);
            Assert.Equal(1.0, result.Transform.Matrix[15]);
        }

        [Fact]
        public void Render_TriangleSolid_CreatesGeometryCorrectly_WithoutThickness()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Thickness = 0.0;
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);

            Assert.Equal("BufferGeometry", result.Geometry.Type);
            Assert.Equal(3, result.Geometry.VertexCount);
            Assert.Equal(1, result.Geometry.FaceCount);
            Assert.True(result.Geometry.HasNormals);
            Assert.True(result.Geometry.HasColors);
            Assert.True(result.Geometry.HasUVs);
            Assert.True(result.Geometry.HasIndices);
            Assert.Equal("TriangleFan", result.Geometry.PrimitiveType);
            Assert.Equal(3, result.Geometry.IndexCount);
        }

        [Fact]
        public void Render_TriangleSolid_CreatesGeometryCorrectly_WithThickness()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Thickness = 5.0;
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);

            Assert.Equal("ExtrudeGeometry", result.Geometry.Type);
            Assert.Equal(3, result.Geometry.VertexCount);
            Assert.Equal(1, result.Geometry.FaceCount);
            Assert.True(result.Geometry.HasNormals);
            Assert.True(result.Geometry.HasColors);
            Assert.True(result.Geometry.HasUVs);
            Assert.True(result.Geometry.HasIndices);
            Assert.Equal("Triangles", result.Geometry.PrimitiveType);
            Assert.Equal(3, result.Geometry.IndexCount);
        }

        [Fact]
        public void Render_QuadSolid_CreatesGeometryCorrectly_WithoutThickness()
        {
            var solid = CreateBasicQuadSolid();
            solid.Thickness = 0.0;
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);

            Assert.Equal("BufferGeometry", result.Geometry.Type);
            Assert.Equal(4, result.Geometry.VertexCount);
            Assert.Equal(2, result.Geometry.FaceCount);
            Assert.True(result.Geometry.HasNormals);
            Assert.True(result.Geometry.HasColors);
            Assert.True(result.Geometry.HasUVs);
            Assert.True(result.Geometry.HasIndices);
            Assert.Equal("TriangleFan", result.Geometry.PrimitiveType);
            Assert.Equal(6, result.Geometry.IndexCount);
        }

        [Fact]
        public void Render_QuadSolid_CreatesGeometryCorrectly_WithThickness()
        {
            var solid = CreateBasicQuadSolid();
            solid.Thickness = 5.0;
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);

            Assert.Equal("ExtrudeGeometry", result.Geometry.Type);
            Assert.Equal(4, result.Geometry.VertexCount);
            Assert.Equal(2, result.Geometry.FaceCount);
            Assert.True(result.Geometry.HasNormals);
            Assert.True(result.Geometry.HasColors);
            Assert.True(result.Geometry.HasUVs);
            Assert.True(result.Geometry.HasIndices);
            Assert.Equal("Triangles", result.Geometry.PrimitiveType);
            Assert.Equal(6, result.Geometry.IndexCount);
        }

        [Fact]
        public void Render_TriangleSolid_CreatesMaterialCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Color = new Color(1);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);

            Assert.Equal("MeshBasicMaterial", result.Material.Type);
            Assert.Equal(0xFF0000, result.Material.Color);
            Assert.Equal(1.0, result.Material.Opacity);
            Assert.False(result.Material.Transparent);
            Assert.False(result.Material.Wireframe);
            Assert.False(result.Material.VertexColors);
            Assert.True(result.Material.Side);
        }

        [Fact]
        public void Render_TriangleSolid_CreatesColorCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Color = new Color(1);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);

            Assert.Equal(1, result.Color.Index);
            Assert.Equal("#FF0000", result.Color.Hex);
            Assert.Equal((byte)255, result.Color.R);
            Assert.Equal((byte)0, result.Color.G);
            Assert.Equal((byte)0, result.Color.B);
        }

        [Fact]
        public void Render_TriangleSolid_CreatesVertexPositionsCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexPositions);
            Assert.Equal(9, result.VertexPositions.Count);

            Assert.Equal(0.0, result.VertexPositions[0]);
            Assert.Equal(0.0, result.VertexPositions[1]);
            Assert.Equal(0.0, result.VertexPositions[2]);

            Assert.Equal(1.0, result.VertexPositions[3]);
            Assert.Equal(0.0, result.VertexPositions[4]);
            Assert.Equal(0.0, result.VertexPositions[5]);

            Assert.Equal(0.0, result.VertexPositions[6]);
            Assert.Equal(1.0, result.VertexPositions[7]);
            Assert.Equal(0.0, result.VertexPositions[8]);
        }

        [Fact]
        public void Render_TriangleSolid_CreatesVertexNormalsCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexNormals);
            Assert.Equal(9, result.VertexNormals.Count);

            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(0.0, result.VertexNormals[i * 3]);
                Assert.Equal(0.0, result.VertexNormals[i * 3 + 1]);
                Assert.Equal(1.0, result.VertexNormals[i * 3 + 2]);
            }
        }

        [Fact]
        public void Render_TriangleSolid_CreatesVertexColorsCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Color = new Color(1);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexColors);
            Assert.Equal(9, result.VertexColors.Count);

            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(1.0, result.VertexColors[i * 3]);
                Assert.Equal(0.0, result.VertexColors[i * 3 + 1]);
                Assert.Equal(0.0, result.VertexColors[i * 3 + 2]);
            }
        }

        [Fact]
        public void Render_TriangleSolid_CreatesVertexUVsCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexUVs);
            Assert.Equal(6, result.VertexUVs.Count);

            Assert.Equal(0.0, result.VertexUVs[0]);
            Assert.Equal(0.0, result.VertexUVs[1]);

            Assert.Equal(1.0, result.VertexUVs[2]);
            Assert.Equal(0.0, result.VertexUVs[3]);

            Assert.Equal(0.0, result.VertexUVs[4]);
            Assert.Equal(1.0, result.VertexUVs[5]);
        }

        [Fact]
        public void Render_TriangleSolid_CreatesIndicesCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Indices);
            Assert.Equal(3, result.Indices.Count);

            Assert.Equal(0, result.Indices[0]);
            Assert.Equal(1, result.Indices[1]);
            Assert.Equal(2, result.Indices[2]);
        }

        [Fact]
        public void Render_QuadSolid_CreatesIndicesCorrectly()
        {
            var solid = CreateBasicQuadSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.Indices);
            Assert.Equal(6, result.Indices.Count);

            Assert.Equal(0, result.Indices[0]);
            Assert.Equal(1, result.Indices[1]);
            Assert.Equal(2, result.Indices[2]);

            Assert.Equal(0, result.Indices[3]);
            Assert.Equal(2, result.Indices[4]);
            Assert.Equal(3, result.Indices[5]);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectFirstCorner()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(10, 20, 30);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(10.0, result.FirstCorner.X);
            Assert.Equal(20.0, result.FirstCorner.Y);
            Assert.Equal(30.0, result.FirstCorner.Z);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectSecondCorner()
        {
            var solid = CreateBasicTriangleSolid();
            solid.SecondCorner = new XYZ(15, 25, 35);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(15.0, result.SecondCorner.X);
            Assert.Equal(25.0, result.SecondCorner.Y);
            Assert.Equal(35.0, result.SecondCorner.Z);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectThirdCorner()
        {
            var solid = CreateBasicTriangleSolid();
            solid.ThirdCorner = new XYZ(20, 30, 40);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(20.0, result.ThirdCorner.X);
            Assert.Equal(30.0, result.ThirdCorner.Y);
            Assert.Equal(40.0, result.ThirdCorner.Z);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectFourthCorner()
        {
            var solid = CreateBasicTriangleSolid();
            solid.ThirdCorner = new XYZ(20, 30, 40);
            solid.FourthCorner = new XYZ(20, 30, 40);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(20.0, result.FourthCorner.X);
            Assert.Equal(30.0, result.FourthCorner.Y);
            Assert.Equal(40.0, result.FourthCorner.Z);
        }

        [Fact]
        public void Render_QuadSolid_ReturnsCorrectFourthCorner()
        {
            var solid = CreateBasicQuadSolid();
            solid.FourthCorner = new XYZ(0, 1, 0);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(0.0, result.FourthCorner.X);
            Assert.Equal(1.0, result.FourthCorner.Y);
            Assert.Equal(0.0, result.FourthCorner.Z);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectVertexCount()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(3, result.VertexCount);
        }

        [Fact]
        public void Render_QuadSolid_ReturnsCorrectVertexCount()
        {
            var solid = CreateBasicQuadSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(4, result.VertexCount);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectFaceCount()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(1, result.FaceCount);
        }

        [Fact]
        public void Render_QuadSolid_ReturnsCorrectFaceCount()
        {
            var solid = CreateBasicQuadSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(2, result.FaceCount);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectIsFilled()
        {
            var solid = CreateBasicTriangleSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.True(result.IsFilled);
        }

        [Fact]
        public void Render_TriangleSolid_WithoutThickness_ReturnsCorrectIsExtruded()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Thickness = 0.0;
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.False(result.IsExtruded);
        }

        [Fact]
        public void Render_TriangleSolid_WithThickness_ReturnsCorrectIsExtruded()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Thickness = 10.0;
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.True(result.IsExtruded);
        }

        [Fact]
        public void Render_TriangleSolid_ReturnsCorrectExtrusionDepth()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Thickness = 15.0;
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(15.0, result.ExtrusionDepth);
        }

        [Fact]
        public void Render_TriangleSolid_WithDifferentColors_ReturnsCorrectColor()
        {
            var solid = CreateBasicTriangleSolid();

            solid.Color = new Color(1);
            var redResult = SolidEntityRenderer.Render(solid);
            Assert.Equal("#FF0000", redResult.Color.Hex);

            solid.Color = new Color(3);
            var greenResult = SolidEntityRenderer.Render(solid);
            Assert.Equal("#00FF00", greenResult.Color.Hex);

            solid.Color = new Color(5);
            var blueResult = SolidEntityRenderer.Render(solid);
            Assert.Equal("#0000FF", blueResult.Color.Hex);

            solid.Color = new Color(7);
            var whiteResult = SolidEntityRenderer.Render(solid);
            Assert.Equal("#FFFFFF", whiteResult.Color.Hex);
        }

        [Fact]
        public void Render_TriangleSolid_WithZeroArea_HandlesCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(0, 0, 0);
            solid.ThirdCorner = new XYZ(0, 0, 0);
            solid.FourthCorner = new XYZ(0, 0, 0);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(0.0, result.Area);
            Assert.Equal(0.0, result.Perimeter);
        }

        [Fact]
        public void Render_TriangleSolid_WithNonZeroZ_TranslatesPointsCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(0, 0, 10);
            solid.SecondCorner = new XYZ(1, 0, 10);
            solid.ThirdCorner = new XYZ(0, 1, 10);
            solid.FourthCorner = new XYZ(0, 1, 10);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(10.0, result.Points[0].Z);
            Assert.Equal(10.0, result.Points[1].Z);
            Assert.Equal(10.0, result.Points[2].Z);
        }

        [Fact]
        public void Render_TriangleSolid_WithThickness_UpdatesBoundsMaxZ()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Thickness = 10.0;
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(10.0, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_QuadSolid_WithThickness_UpdatesBoundsMaxZ()
        {
            var solid = CreateBasicQuadSolid();
            solid.Thickness = 10.0;
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(10.0, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_TriangleSolid_WithZeroNormal_HandlesCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Normal = new XYZ(0, 0, 0);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(0.0, result.Normal.X);
            Assert.Equal(0.0, result.Normal.Y);
            Assert.Equal(0.0, result.Normal.Z);
        }

        [Fact]
        public void Render_QuadSolid_CreatesVertexUVsCorrectly()
        {
            var solid = CreateBasicQuadSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexUVs);
            Assert.Equal(8, result.VertexUVs.Count);

            Assert.Equal(0.0, result.VertexUVs[0]);
            Assert.Equal(0.0, result.VertexUVs[1]);

            Assert.Equal(1.0, result.VertexUVs[2]);
            Assert.Equal(0.0, result.VertexUVs[3]);

            Assert.Equal(0.0, result.VertexUVs[4]);
            Assert.Equal(1.0, result.VertexUVs[5]);

            Assert.Equal(1.0, result.VertexUVs[6]);
            Assert.Equal(1.0, result.VertexUVs[7]);
        }

        [Fact]
        public void Render_QuadSolid_CreatesVertexNormalsCorrectly()
        {
            var solid = CreateBasicQuadSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexNormals);
            Assert.Equal(12, result.VertexNormals.Count);

            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(0.0, result.VertexNormals[i * 3]);
                Assert.Equal(0.0, result.VertexNormals[i * 3 + 1]);
                Assert.Equal(1.0, result.VertexNormals[i * 3 + 2]);
            }
        }

        [Fact]
        public void Render_QuadSolid_CreatesVertexColorsCorrectly()
        {
            var solid = CreateBasicQuadSolid();
            solid.Color = new Color(5);
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexColors);
            Assert.Equal(12, result.VertexColors.Count);

            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(0.0, result.VertexColors[i * 3]);
                Assert.Equal(0.0, result.VertexColors[i * 3 + 1]);
                Assert.Equal(1.0, result.VertexColors[i * 3 + 2]);
            }
        }

        [Fact]
        public void Render_QuadSolid_CreatesVertexPositionsCorrectly()
        {
            var solid = CreateBasicQuadSolid();
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexPositions);
            Assert.Equal(12, result.VertexPositions.Count);

            Assert.Equal(0.0, result.VertexPositions[0]);
            Assert.Equal(0.0, result.VertexPositions[1]);
            Assert.Equal(0.0, result.VertexPositions[2]);

            Assert.Equal(1.0, result.VertexPositions[3]);
            Assert.Equal(0.0, result.VertexPositions[4]);
            Assert.Equal(0.0, result.VertexPositions[5]);

            Assert.Equal(0.0, result.VertexPositions[6]);
            Assert.Equal(1.0, result.VertexPositions[7]);
            Assert.Equal(0.0, result.VertexPositions[8]);

            Assert.Equal(1.0, result.VertexPositions[9]);
            Assert.Equal(1.0, result.VertexPositions[10]);
            Assert.Equal(0.0, result.VertexPositions[11]);
        }

        [Fact]
        public void Render_TriangleSolid_WithNegativeThickness_HandlesCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Thickness = -5.0;
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(-5.0, result.Thickness);
            Assert.False(result.IsExtruded);
        }

        [Fact]
        public void Render_QuadSolid_WithNegativeThickness_HandlesCorrectly()
        {
            var solid = CreateBasicQuadSolid();
            solid.Thickness = -5.0;
            var result = SolidEntityRenderer.Render(solid);

            Assert.NotNull(result);
            Assert.Equal(-5.0, result.Thickness);
            Assert.False(result.IsExtruded);
        }
    }
}
