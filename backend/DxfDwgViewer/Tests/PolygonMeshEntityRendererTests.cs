using System;
using Xunit;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using DxfDwgViewer.RenderUtilities;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class PolygonMeshEntityRendererTests
    {
        private Mesh CreateBasicMesh()
        {
            var mesh = new Mesh();
            mesh.Color = new Color(1);

            mesh.Vertices.Add(new XYZ(0, 0, 0));
            mesh.Vertices.Add(new XYZ(10, 0, 0));
            mesh.Vertices.Add(new XYZ(5, 10, 0));
            mesh.Vertices.Add(new XYZ(5, 5, 5));

            mesh.Faces.Add(new int[] { 0, 1, 2 });
            mesh.Faces.Add(new int[] { 0, 1, 3 });

            mesh.Edges.Add(new Mesh.Edge(0, 1));
            mesh.Edges.Add(new Mesh.Edge(1, 2));
            mesh.Edges.Add(new Mesh.Edge(2, 0));
            mesh.Edges.Add(new Mesh.Edge(0, 3));
            mesh.Edges.Add(new Mesh.Edge(1, 3));

            mesh.SubdivisionLevel = 0;
            mesh.BlendCrease = false;

            return mesh;
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectType()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal("Mesh", result.EntityType);
            Assert.Equal("BufferGeometry", result.GeometryType);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectVertexCount()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(4, result.VertexCount);
            Assert.Equal(4, result.Vertices.Count);
            Assert.Equal(4, result.Vertices3D.Count);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectFaceCount()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(2, result.FaceCount);
            Assert.Equal(2, result.Faces.Count);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectEdgeCount()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(5, result.EdgeCount);
            Assert.Equal(5, result.Edges.Count);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectVertices()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

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
        public void Render_Mesh_ReturnsCorrectIndices()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

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
        public void Render_Mesh_ReturnsCorrectEdges()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(5, result.Edges.Count);
            Assert.Equal(0, result.Edges[0].StartIndex);
            Assert.Equal(1, result.Edges[0].EndIndex);
            Assert.Null(result.Edges[0].Crease);

            Assert.Equal(1, result.Edges[1].StartIndex);
            Assert.Equal(2, result.Edges[1].EndIndex);

            Assert.Equal(2, result.Edges[2].StartIndex);
            Assert.Equal(0, result.Edges[2].EndIndex);

            Assert.Equal(0, result.Edges[3].StartIndex);
            Assert.Equal(3, result.Edges[3].EndIndex);

            Assert.Equal(1, result.Edges[4].StartIndex);
            Assert.Equal(3, result.Edges[4].EndIndex);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectColorIndex()
        {
            var mesh = CreateBasicMesh();
            mesh.Color = new Color(5);
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectBounds3D()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

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
        public void Render_Mesh_ReturnsCorrectCentroid()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);
            Assert.Equal(5, result.Centroid.X);
            Assert.Equal(3.75, result.Centroid.Y);
            Assert.Equal(1.25, result.Centroid.Z);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectNormals()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(4, result.Normals.Count);
            Assert.Equal(12, result.NormalsArray.Count);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectColorsArray()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(12, result.ColorsArray.Count);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectUVsArray()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(8, result.UVsArray.Count);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectTransform()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

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
        public void Render_Mesh_ReturnsCorrectHandle()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(mesh.Handle.ToString(), result.Handle);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectVisible()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.True(result.Visible);
        }

        [Fact]
        public void Render_Mesh_Invisible_ReturnsCorrectVisible()
        {
            var mesh = CreateBasicMesh();
            mesh.IsInvisible = true;
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.False(result.Visible);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectLayerName()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal("0", result.LayerName);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectCastShadows()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.True(result.CastShadows);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectReceiveShadows()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.True(result.ReceiveShadows);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectDoubleSided()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.False(result.DoubleSided);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectFlatShading()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.False(result.FlatShading);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectSubdivisionLevel()
        {
            var mesh = CreateBasicMesh();
            mesh.SubdivisionLevel = 3;
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(3, result.SubdivisionLevel);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectVersion()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(0, result.Version);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectBlendCrease()
        {
            var mesh = CreateBasicMesh();
            mesh.BlendCrease = true;
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.True(result.BlendCrease);
        }

        [Fact]
        public void Render_Mesh_NullVertices_ReturnsEmptyData()
        {
            var mesh = new Mesh();
            mesh.Vertices.Clear();
            mesh.Faces.Clear();
            mesh.Edges.Clear();

            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(0, result.VertexCount);
            Assert.Equal(0, result.FaceCount);
            Assert.Equal(0, result.EdgeCount);
            Assert.Empty(result.Vertices);
            Assert.Empty(result.Vertices3D);
        }

        [Fact]
        public void Render_Mesh_NullFaces_ReturnsEmptyFaces()
        {
            var mesh = new Mesh();
            mesh.Vertices.Add(new XYZ(0, 0, 0));
            mesh.Faces.Clear();
            mesh.Edges.Clear();

            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(1, result.VertexCount);
            Assert.Equal(0, result.FaceCount);
            Assert.Single(result.Vertices);
            Assert.Empty(result.Faces);
        }

        [Fact]
        public void Render_Mesh_NullEdges_ReturnsEmptyEdges()
        {
            var mesh = new Mesh();
            mesh.Vertices.Add(new XYZ(0, 0, 0));
            mesh.Faces.Add(new int[] { 0 });
            mesh.Edges.Clear();

            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(1, result.VertexCount);
            Assert.Equal(0, result.EdgeCount);
            Assert.Empty(result.Edges);
        }

        [Fact]
        public void Render_Mesh_QuadFace_SplitsIntoTwoTriangles()
        {
            var mesh = new Mesh();
            mesh.Color = new Color(1);

            mesh.Vertices.Add(new XYZ(0, 0, 0));
            mesh.Vertices.Add(new XYZ(10, 0, 0));
            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(0, 10, 0));

            mesh.Faces.Add(new int[] { 0, 1, 2, 3 });

            var result = PolygonMeshEntityRenderer.Render(mesh);

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
        public void Render_Mesh_EdgeWithCrease_ReturnsCorrectCrease()
        {
            var mesh = new Mesh();
            mesh.Color = new Color(1);

            mesh.Vertices.Add(new XYZ(0, 0, 0));
            mesh.Vertices.Add(new XYZ(10, 0, 0));

            var edge = new Mesh.Edge(0, 1);
            edge.Crease = 0.5;
            mesh.Edges.Add(edge);

            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Single(result.Edges);
            Assert.Equal(0, result.Edges[0].StartIndex);
            Assert.Equal(1, result.Edges[0].EndIndex);
            Assert.Equal(0.5, result.Edges[0].Crease);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectTransparency()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(0.0, result.Transparency);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectMaterialName()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal("", result.MaterialName);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectLineTypeName()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal("Continuous", result.LineTypeName);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectLineWeight()
        {
            var mesh = CreateBasicMesh();
            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(0.03, result.LineWeight, 2);
        }

        [Fact]
        public void Render_Mesh_EmptyVertices_ReturnsEmptyData()
        {
            var mesh = new Mesh();
            mesh.Color = new Color(1);
            mesh.Vertices.Clear();
            mesh.Faces.Clear();
            mesh.Edges.Clear();

            var result = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(0, result.VertexCount);
            Assert.Equal(0, result.FaceCount);
            Assert.Equal(0, result.EdgeCount);
            Assert.Empty(result.Vertices);
            Assert.Empty(result.Vertices3D);
            Assert.Empty(result.Faces);
            Assert.Empty(result.Edges);
        }
    }
}
