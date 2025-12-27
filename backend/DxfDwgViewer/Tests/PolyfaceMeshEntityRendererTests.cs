using System;
using Xunit;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using DxfDwgViewer.RenderUtilities;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class PolyfaceMeshEntityRendererTests
    {
        private PolyfaceMesh CreateBasicPolyfaceMesh()
        {
            var polyfaceMesh = new PolyfaceMesh();
            polyfaceMesh.Color = new Color(1);

            var vertex1 = new VertexFaceMesh
            {
                Location = new XYZ(0, 0, 0)
            };
            var vertex2 = new VertexFaceMesh
            {
                Location = new XYZ(10, 0, 0)
            };
            var vertex3 = new VertexFaceMesh
            {
                Location = new XYZ(5, 10, 0)
            };
            var vertex4 = new VertexFaceMesh
            {
                Location = new XYZ(5, 5, 5)
            };

            polyfaceMesh.Vertices.Add(vertex1);
            polyfaceMesh.Vertices.Add(vertex2);
            polyfaceMesh.Vertices.Add(vertex3);
            polyfaceMesh.Vertices.Add(vertex4);

            var face1 = new VertexFaceRecord();
            face1.Index1 = 1;
            face1.Index2 = 2;
            face1.Index3 = 3;
            polyfaceMesh.Faces.Add(face1);

            var face2 = new VertexFaceRecord();
            face2.Index1 = 1;
            face2.Index2 = 2;
            face2.Index3 = 4;
            polyfaceMesh.Faces.Add(face2);

            return polyfaceMesh;
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectType()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal("PolyfaceMesh", result.EntityType);
            Assert.Equal("BufferGeometry", result.GeometryType);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectVertexCount()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal(4, result.VertexCount);
            Assert.Equal(4, result.Vertices.Count);
            Assert.Equal(4, result.Vertices3D.Count);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectFaceCount()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal(2, result.FaceCount);
            Assert.Equal(2, result.Faces.Count);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectVertices()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal(0, result.Vertices[0].Location.X);
            Assert.Equal(0, result.Vertices[0].Location.Y);
            Assert.Equal(0, result.Vertices[0].Location.Z);

            Assert.Equal(10, result.Vertices[1].Location.X);
            Assert.Equal(0, result.Vertices[1].Location.Y);
            Assert.Equal(0, result.Vertices[1].Location.Z);

            Assert.Equal(5, result.Vertices[2].Location.X);
            Assert.Equal(10, result.Vertices[2].Location.Y);
            Assert.Equal(0, result.Vertices[2].Location.Z);

            Assert.Equal(5, result.Vertices[3].Location.X);
            Assert.Equal(5, result.Vertices[3].Location.Y);
            Assert.Equal(5, result.Vertices[3].Location.Z);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectIndices()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal(6, result.Indices.Count);
            Assert.Equal(0, result.Indices[0]);
            Assert.Equal(1, result.Indices[1]);
            Assert.Equal(2, result.Indices[2]);
            Assert.Equal(0, result.Indices[3]);
            Assert.Equal(1, result.Indices[4]);
            Assert.Equal(3, result.Indices[5]);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectColorIndex()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            polyfaceMesh.Color = new Color(5);
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectBounds3D()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds3D);
            Assert.Equal(0, result.Bounds3D.Min.X);
            Assert.Equal(0, result.Bounds3D.Min.Y);
            Assert.Equal(0, result.Bounds3D.Min.Z);
            Assert.Equal(10, result.Bounds3D.Max.X);
            Assert.Equal(10, result.Bounds3D.Max.Y);
            Assert.Equal(5, result.Bounds3D.Max.Z);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectCentroid()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);
            Assert.Equal(5, result.Centroid.X);
            Assert.Equal(3.75, result.Centroid.Y);
            Assert.Equal(1.25, result.Centroid.Z);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectNormals()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal(4, result.Normals.Count);
            Assert.Equal(4, result.NormalsArray.Count / 3);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectColorsArray()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal(12, result.ColorsArray.Count);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectUVsArray()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal(8, result.UVsArray.Count);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectTransform()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.Equal(1.0, result.Transform.Scale.X);
            Assert.Equal(1.0, result.Transform.Scale.Y);
            Assert.Equal(1.0, result.Transform.Scale.Z);
            Assert.Equal(0, result.Transform.Rotation.X);
            Assert.Equal(0, result.Transform.Rotation.Y);
            Assert.Equal(0, result.Transform.Rotation.Z);
            Assert.Equal(0, result.Transform.Position.X);
            Assert.Equal(0, result.Transform.Position.Y);
            Assert.Equal(0, result.Transform.Position.Z);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectHandle()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal(polyfaceMesh.Handle.ToString(), result.Handle);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectVisible()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.True(result.Visible);
        }

        [Fact]
        public void Render_PolyfaceMesh_Invisible_ReturnsCorrectVisible()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            polyfaceMesh.IsInvisible = true;
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.False(result.Visible);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectLayerName()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal("0", result.LayerName);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectCastShadows()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.True(result.CastShadows);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectReceiveShadows()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.True(result.ReceiveShadows);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectDoubleSided()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.False(result.DoubleSided);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectFlatShading()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.False(result.FlatShading);
        }

        [Fact]
        public void Render_PolyfaceMesh_NullVertices_ReturnsEmptyData()
        {
            var polyfaceMesh = new PolyfaceMesh();
            polyfaceMesh.Vertices.Clear();
            polyfaceMesh.Faces.Clear();

            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal(0, result.VertexCount);
            Assert.Equal(0, result.FaceCount);
            Assert.Empty(result.Vertices);
            Assert.Empty(result.Vertices3D);
        }

        [Fact]
        public void Render_PolyfaceMesh_NullFaces_ReturnsEmptyFaces()
        {
            var polyfaceMesh = new PolyfaceMesh();
            var vertex1 = new VertexFaceMesh
            {
                Location = new XYZ(0, 0, 0)
            };
            polyfaceMesh.Vertices.Add(vertex1);
            polyfaceMesh.Faces.Clear();

            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal(1, result.VertexCount);
            Assert.Equal(0, result.FaceCount);
            Assert.Single(result.Vertices);
            Assert.Empty(result.Faces);
        }

        [Fact]
        public void Render_PolyfaceMesh_QuadFace_SplitsIntoTwoTriangles()
        {
            var polyfaceMesh = new PolyfaceMesh();
            polyfaceMesh.Color = new Color(1);

            var vertex1 = new VertexFaceMesh { Location = new XYZ(0, 0, 0) };
            var vertex2 = new VertexFaceMesh { Location = new XYZ(10, 0, 0) };
            var vertex3 = new VertexFaceMesh { Location = new XYZ(10, 10, 0) };
            var vertex4 = new VertexFaceMesh { Location = new XYZ(0, 10, 0) };

            polyfaceMesh.Vertices.Add(vertex1);
            polyfaceMesh.Vertices.Add(vertex2);
            polyfaceMesh.Vertices.Add(vertex3);
            polyfaceMesh.Vertices.Add(vertex4);

            var face1 = new VertexFaceRecord();
            face1.Index1 = 1;
            face1.Index2 = 2;
            face1.Index3 = 3;
            face1.Index4 = 4;
            polyfaceMesh.Faces.Add(face1);

            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal(6, result.Indices.Count);
            Assert.Equal(0, result.Indices[0]);
            Assert.Equal(1, result.Indices[1]);
            Assert.Equal(2, result.Indices[2]);
            Assert.Equal(0, result.Indices[3]);
            Assert.Equal(2, result.Indices[4]);
            Assert.Equal(3, result.Indices[5]);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectTransparency()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal(0.0, result.Transparency);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectMaterialName()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal("", result.MaterialName);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectLineTypeName()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal("Continuous", result.LineTypeName);
        }

        [Fact]
        public void Render_PolyfaceMesh_ReturnsCorrectLineWeight()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            var result = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(result);
            Assert.Equal(0.03, result.LineWeight, 2);
        }
    }
}
