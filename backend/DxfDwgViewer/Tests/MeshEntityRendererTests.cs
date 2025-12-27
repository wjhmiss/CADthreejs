using Xunit;
using ACadSharp.Entities;
using ACadSharp.Tables;
using DxfDwgViewer.RenderUtilities;
using CSMath;
using System;
using System.Collections.Generic;
using ACadSharp;

namespace DxfDwgViewer.Tests
{
    public class MeshEntityRendererTests
    {
        [Fact]
        public void Render_Mesh_ReturnsCorrectType()
        {
            var mesh = CreateBasicMesh();

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal("Mesh", result.Type);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectEntityType()
        {
            var mesh = CreateBasicMesh();

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal("Mesh", result.EntityType);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectHandle()
        {
            var mesh = CreateBasicMesh();

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.NotNull(result.Handle);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectLayerName()
        {
            var mesh = CreateBasicMesh();
            mesh.Layer = new Layer("MY_LAYER");

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal("MY_LAYER", result.LayerName);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectVisibility()
        {
            var mesh = CreateBasicMesh();
            mesh.IsInvisible = false;

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.True(result.Visible);
        }

        [Fact]
        public void Render_Mesh_WithInvisible_ReturnsCorrectVisibility()
        {
            var mesh = CreateBasicMesh();
            mesh.IsInvisible = true;

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.False(result.Visible);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectVertexCount()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(15, 20, 0));

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(3, result.VertexCount);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectVertices3D()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(10, 10, 5));
            mesh.Vertices.Add(new XYZ(20, 10, 5));
            mesh.Vertices.Add(new XYZ(15, 20, 5));

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(3, result.Vertices3D.Count);
            Assert.Equal(10, result.Vertices3D[0].X);
            Assert.Equal(10, result.Vertices3D[0].Y);
            Assert.Equal(5, result.Vertices3D[0].Z);
            Assert.Equal(20, result.Vertices3D[1].X);
            Assert.Equal(10, result.Vertices3D[1].Y);
            Assert.Equal(5, result.Vertices3D[1].Z);
            Assert.Equal(15, result.Vertices3D[2].X);
            Assert.Equal(20, result.Vertices3D[2].Y);
            Assert.Equal(5, result.Vertices3D[2].Z);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectFaceCount()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(15, 20, 0));
            mesh.Faces.Add(new int[] { 0, 1, 2 });

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(1, result.FaceCount);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectFaceIndices()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(15, 20, 0));
            mesh.Faces.Add(new int[] { 0, 1, 2 });

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Single(result.FaceIndices);
            Assert.Equal(3, result.FaceIndices[0].Length);
            Assert.Equal(0, result.FaceIndices[0][0]);
            Assert.Equal(1, result.FaceIndices[0][1]);
            Assert.Equal(2, result.FaceIndices[0][2]);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectEdgeCount()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(15, 20, 0));
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 0, End = 1 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 1, End = 2 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 2, End = 0 });

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(3, result.EdgeCount);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectEdgeIndices()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(15, 20, 0));
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 0, End = 1 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 1, End = 2 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 2, End = 0 });

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(3, result.EdgeIndices.Count);
            Assert.Equal(0, result.EdgeIndices[0][0]);
            Assert.Equal(1, result.EdgeIndices[0][1]);
            Assert.Equal(1, result.EdgeIndices[1][0]);
            Assert.Equal(2, result.EdgeIndices[1][1]);
            Assert.Equal(2, result.EdgeIndices[2][0]);
            Assert.Equal(0, result.EdgeIndices[2][1]);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectCentroid3D()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(15, 20, 0));

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(15, result.Centroid3D.X);
            Assert.Equal(13.333333333333334, result.Centroid3D.Y, 5);
            Assert.Equal(0, result.Centroid3D.Z);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectBounds3D()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(10, 10, 5));
            mesh.Vertices.Add(new XYZ(20, 10, 5));
            mesh.Vertices.Add(new XYZ(15, 20, 10));

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(10, result.Bounds3D.Min.X);
            Assert.Equal(10, result.Bounds3D.Min.Y);
            Assert.Equal(5, result.Bounds3D.Min.Z);
            Assert.Equal(20, result.Bounds3D.Max.X);
            Assert.Equal(20, result.Bounds3D.Max.Y);
            Assert.Equal(10, result.Bounds3D.Max.Z);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectColorIndex()
        {
            var mesh = CreateBasicMesh();
            mesh.Color = new ACadSharp.Color(5);

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectLineTypeName()
        {
            var mesh = CreateBasicMesh();
            mesh.LineType = new LineType("DASHED");

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal("DASHED", result.LineTypeName);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectLineWeight()
        {
            var mesh = CreateBasicMesh();
            mesh.LineWeight = LineWeightType.W50;

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(50, result.LineWeight);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectOpacity()
        {
            var mesh = CreateBasicMesh();

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(1.0, result.Opacity);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectTransparent()
        {
            var mesh = CreateBasicMesh();

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.False(result.Transparent);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectMaterialType()
        {
            var mesh = CreateBasicMesh();

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal("MeshStandardMaterial", result.MaterialType);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectDepthTest()
        {
            var mesh = CreateBasicMesh();

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.True(result.DepthTest);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectDepthWrite()
        {
            var mesh = CreateBasicMesh();

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.True(result.DepthWrite);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectSide()
        {
            var mesh = CreateBasicMesh();

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.True(result.Side);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectSubdivisionLevel()
        {
            var mesh = CreateBasicMesh();

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(0, result.SubdivisionLevel);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectVersion()
        {
            var mesh = CreateBasicMesh();

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(0, result.Version);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectBlendCrease()
        {
            var mesh = CreateBasicMesh();

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.False(result.BlendCrease);
        }

        [Fact]
        public void Render_Mesh_WithMultipleFaces_ReturnsCorrectFaceCount()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(15, 20, 0));
            mesh.Vertices.Add(new XYZ(15, 10, 10));
            mesh.Faces.Add(new int[] { 0, 1, 2 });
            mesh.Faces.Add(new int[] { 0, 1, 3 });

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(2, result.FaceCount);
        }

        [Fact]
        public void Render_Mesh_WithQuadrilateralFace_ReturnsCorrectFaceIndices()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 20, 0));
            mesh.Vertices.Add(new XYZ(10, 20, 0));
            mesh.Faces.Add(new int[] { 0, 1, 2, 3 });

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Single(result.FaceIndices);
            Assert.Equal(4, result.FaceIndices[0].Length);
            Assert.Equal(0, result.FaceIndices[0][0]);
            Assert.Equal(1, result.FaceIndices[0][1]);
            Assert.Equal(2, result.FaceIndices[0][2]);
            Assert.Equal(3, result.FaceIndices[0][3]);
        }

        [Fact]
        public void Render_Mesh_WithInvalidFace_IgnoresInvalidFace()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(15, 20, 0));
            mesh.Faces.Add(new int[] { 0, 1 });
            mesh.Faces.Add(new int[] { 0, 1, 2 });

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Single(result.FaceIndices);
            Assert.Equal(3, result.FaceIndices[0].Length);
        }

        [Fact]
        public void Render_Mesh_WithNoVertices_ReturnsEmptyVertices3D()
        {
            var mesh = CreateBasicMesh();

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Empty(result.Vertices3D);
        }

        [Fact]
        public void Render_Mesh_WithNoFaces_ReturnsEmptyFaceIndices()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(15, 20, 0));

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Empty(result.FaceIndices);
        }

        [Fact]
        public void Render_Mesh_WithNoEdges_ReturnsEmptyEdgeIndices()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(15, 20, 0));

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Empty(result.EdgeIndices);
        }

        [Fact]
        public void Render_Mesh_ReturnsCorrectCoordinateSystem()
        {
            var mesh = CreateBasicMesh();

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal("World", result.CoordinateSystem);
        }

        [Fact]
        public void Render_Mesh_WithNegativeCoordinates_ReturnsCorrectBounds3D()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(-10, -10, -5));
            mesh.Vertices.Add(new XYZ(20, 10, 5));

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(-10, result.Bounds3D.Min.X);
            Assert.Equal(-10, result.Bounds3D.Min.Y);
            Assert.Equal(-5, result.Bounds3D.Min.Z);
            Assert.Equal(20, result.Bounds3D.Max.X);
            Assert.Equal(10, result.Bounds3D.Max.Y);
            Assert.Equal(5, result.Bounds3D.Max.Z);
        }

        [Fact]
        public void Render_Mesh_WithLargeCoordinates_ReturnsCorrectCentroid3D()
        {
            var mesh = CreateBasicMesh();
            mesh.Vertices.Add(new XYZ(1000, 2000, 3000));
            mesh.Vertices.Add(new XYZ(2000, 3000, 4000));
            mesh.Vertices.Add(new XYZ(1500, 2500, 3500));

            var result = MeshEntityRenderer.Render(mesh);

            Assert.NotNull(result);
            Assert.Equal(1500, result.Centroid3D.X);
            Assert.Equal(2500, result.Centroid3D.Y);
            Assert.Equal(3500, result.Centroid3D.Z);
        }

        private Mesh CreateBasicMesh()
        {
            var mesh = new Mesh();
            mesh.Layer = new Layer("0");
            mesh.Color = new ACadSharp.Color(7);
            mesh.LineType = new LineType("CONTINUOUS");
            mesh.LineWeight = LineWeightType.ByLayer;
            mesh.IsInvisible = false;
            return mesh;
        }
    }
}
