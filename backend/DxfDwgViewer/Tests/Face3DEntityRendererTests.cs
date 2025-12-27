using Xunit;
using ACadSharp.Entities;
using DxfDwgViewer.RenderUtilities;
using CSMath;
using System;

namespace DxfDwgViewer.Tests
{
    public class Face3DEntityRendererTests
    {
        [Fact]
        public void Render_ValidTriangle_ReturnsValidFace3DData()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.Equal("Face3D", result.Type);
            Assert.Equal("3DFACE", result.EntityType);
            Assert.Equal(0, result.FirstCorner.X);
            Assert.Equal(0, result.FirstCorner.Y);
            Assert.Equal(0, result.FirstCorner.Z);
            Assert.Equal(10, result.SecondCorner.X);
            Assert.Equal(0, result.SecondCorner.Y);
            Assert.Equal(0, result.SecondCorner.Z);
            Assert.Equal(5, result.ThirdCorner.X);
            Assert.Equal(10, result.ThirdCorner.Y);
            Assert.Equal(0, result.ThirdCorner.Z);
        }

        [Fact]
        public void Render_ValidQuad_ReturnsValidFace3DDataWithFourthCorner()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(10, 10, 0),
                FourthCorner = new XYZ(0, 10, 0),
                Color = new ACadSharp.Color(2)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.Equal("Face3D", result.Type);
            Assert.Equal("3DFACE", result.EntityType);
            Assert.Equal(0, result.FirstCorner.X);
            Assert.Equal(0, result.FirstCorner.Y);
            Assert.Equal(0, result.FirstCorner.Z);
            Assert.Equal(10, result.SecondCorner.X);
            Assert.Equal(0, result.SecondCorner.Y);
            Assert.Equal(0, result.SecondCorner.Z);
            Assert.Equal(10, result.ThirdCorner.X);
            Assert.Equal(10, result.ThirdCorner.Y);
            Assert.Equal(0, result.ThirdCorner.Z);
            Assert.Equal(0, result.FourthCorner.X);
            Assert.Equal(10, result.FourthCorner.Y);
            Assert.Equal(0, result.FourthCorner.Z);
            Assert.True(result.HasFourthCorner);
        }

        [Fact]
        public void Render_TriangleWithCustomColor_ReturnsCorrectColorValues()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(3)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.Equal(3, result.ColorIndex);
            Assert.Equal("#00FF00", result.ColorHex);
            Assert.Equal(0, result.ColorR);
            Assert.Equal(255, result.ColorG);
            Assert.Equal(0, result.ColorB);
        }

        [Fact]
        public void Render_FaceWithRedColor_ReturnsCorrectColorValues()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.Equal(1, result.ColorIndex);
            Assert.Equal("#FF0000", result.ColorHex);
            Assert.Equal(255, result.ColorR);
            Assert.Equal(0, result.ColorG);
            Assert.Equal(0, result.ColorB);
        }

        [Fact]
        public void Render_Triangle_CalculatesCorrectArea()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.True(result.Area > 0);
            Assert.Equal(50, result.Area, 1);
        }

        [Fact]
        public void Render_Quad_CalculatesCorrectArea()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(10, 10, 0),
                FourthCorner = new XYZ(0, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.True(result.Area > 0);
            Assert.Equal(100, result.Area, 1);
        }

        [Fact]
        public void Render_Triangle_CalculatesCorrectCenter()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.NotNull(result.Center);
            Assert.Equal(5, result.Center.X, 1);
            Assert.Equal(3.33, result.Center.Y, 2);
            Assert.Equal(0, result.Center.Z, 1);
        }

        [Fact]
        public void Render_Quad_CalculatesCorrectCenter()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(10, 10, 0),
                FourthCorner = new XYZ(0, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.NotNull(result.Center);
            Assert.Equal(5, result.Center.X, 1);
            Assert.Equal(5, result.Center.Y, 1);
            Assert.Equal(0, result.Center.Z, 1);
        }

        [Fact]
        public void Render_Triangle_CalculatesCorrectNormal()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.NotNull(result.Normal);
            Assert.Equal(0, result.Normal.X, 1);
            Assert.Equal(0, result.Normal.Y, 1);
            Assert.Equal(1, result.Normal.Z, 1);
        }

        [Fact]
        public void Render_Triangle_SetsCorrectGeometryType()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.Equal("Triangle", result.GeometryType);
            Assert.False(result.HasFourthCorner);
        }

        [Fact]
        public void Render_Quad_SetsCorrectGeometryType()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(10, 10, 0),
                FourthCorner = new XYZ(0, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.Equal("Quad", result.GeometryType);
            Assert.True(result.HasFourthCorner);
        }

        [Fact]
        public void Render_Triangle_GeneratesCorrectVertices()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices);
            Assert.Equal(9, result.Vertices.Count);
            Assert.Equal(0, result.Vertices[0]);
            Assert.Equal(0, result.Vertices[1]);
            Assert.Equal(0, result.Vertices[2]);
            Assert.Equal(10, result.Vertices[3]);
            Assert.Equal(0, result.Vertices[4]);
            Assert.Equal(0, result.Vertices[5]);
            Assert.Equal(5, result.Vertices[6]);
            Assert.Equal(10, result.Vertices[7]);
            Assert.Equal(0, result.Vertices[8]);
        }

        [Fact]
        public void Render_Quad_GeneratesCorrectVertices()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(10, 10, 0),
                FourthCorner = new XYZ(0, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices);
            Assert.Equal(12, result.Vertices.Count);
            Assert.Equal(0, result.Vertices[0]);
            Assert.Equal(0, result.Vertices[1]);
            Assert.Equal(0, result.Vertices[2]);
            Assert.Equal(10, result.Vertices[3]);
            Assert.Equal(0, result.Vertices[4]);
            Assert.Equal(0, result.Vertices[5]);
            Assert.Equal(10, result.Vertices[6]);
            Assert.Equal(10, result.Vertices[7]);
            Assert.Equal(0, result.Vertices[8]);
            Assert.Equal(0, result.Vertices[9]);
            Assert.Equal(10, result.Vertices[10]);
            Assert.Equal(0, result.Vertices[11]);
        }

        [Fact]
        public void Render_Triangle_GeneratesCorrectIndices()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.NotNull(result.Indices);
            Assert.Equal(3, result.Indices.Count);
            Assert.Equal(0, result.Indices[0]);
            Assert.Equal(1, result.Indices[1]);
            Assert.Equal(2, result.Indices[2]);
        }

        [Fact]
        public void Render_Quad_GeneratesCorrectIndices()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(10, 10, 0),
                FourthCorner = new XYZ(0, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

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
        public void Render_GeneratesCorrectNormals()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.NotNull(result.Normals);
            Assert.Equal(9, result.Normals.Count);
        }

        [Fact]
        public void Render_CalculatesValidBounds()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);
            Assert.Equal(0, result.Bounds.Min.X, 1);
            Assert.Equal(0, result.Bounds.Min.Y, 1);
            Assert.Equal(0, result.Bounds.Min.Z, 1);
            Assert.Equal(10, result.Bounds.Max.X, 1);
            Assert.Equal(10, result.Bounds.Max.Y, 1);
            Assert.Equal(0, result.Bounds.Max.Z, 1);
        }

        [Fact]
        public void Render_SetsCorrectMaterialProperties()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.Equal("MeshBasicMaterial", result.MaterialType);
            Assert.True(result.MaterialDepthTest);
            Assert.True(result.MaterialDepthWrite);
            Assert.Equal(2, result.MaterialSide);
        }

        [Fact]
        public void Render_SetsCorrectTransformMatrix()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Matrix);
            Assert.Equal(16, result.Transform.Matrix.Length);
            Assert.Equal(1, result.Transform.Matrix[0]);
            Assert.Equal(1, result.Transform.Matrix[5]);
            Assert.Equal(1, result.Transform.Matrix[10]);
            Assert.Equal(1, result.Transform.Matrix[15]);
        }

        [Fact]
        public void Render_SetsCorrectCoordinateSystem()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.Equal("AutoCAD", result.CoordinateSystem);
            Assert.True(result.RequiresYAxisFlip);
        }

        [Fact]
        public void Render_SetsCorrectVisibilityProperties()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.True(result.Visible);
            Assert.True(result.CastShadow);
            Assert.True(result.ReceiveShadow);
            Assert.Equal(0, result.RenderOrder);
        }

        [Fact]
        public void Render_InvisibleFace_SetsIsInvisible()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };
            face3D.IsInvisible = true;

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.True(result.IsInvisible);
            Assert.False(result.Visible);
        }

        [Fact]
        public void Render_GeneratesUniqueUuid()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result1 = Face3DEntityRenderer.Render(face3D);
            var result2 = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotEqual(result1.Uuid, result2.Uuid);
        }

        [Fact]
        public void Render_3DTriangleInSpace_CalculatesCorrectNormal()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 5, 10),
                FourthCorner = new XYZ(5, 5, 10),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.NotNull(result.Normal);
            Assert.True(result.Normal.Z > 0);
        }

        [Fact]
        public void Render_QuadIn3DSpace_CalculatesCorrectArea()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 5),
                ThirdCorner = new XYZ(10, 10, 5),
                FourthCorner = new XYZ(0, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.True(result.Area > 0);
        }

        [Fact]
        public void Render_CalculatesValidCentroid()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1)
            };

            var result = Face3DEntityRenderer.Render(face3D);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);
            Assert.Equal(5, result.Centroid.X, 1);
            Assert.Equal(3.33, result.Centroid.Y, 2);
            Assert.Equal(0, result.Centroid.Z, 1);
        }
    }
}
