using Xunit;
using ACadSharp.Entities;
using ACadSharp.IO;
using ACadSharp.Tables;
using ACadSharp;
using DxfDwgViewer.RenderUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class MeshEntityRendererIntegrationTests
    {
        [Fact]
        public void LoadAndRenderMeshFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var meshEntities = new List<Mesh>();
            foreach (var entity in doc.Entities)
            {
                if (entity is Mesh mesh)
                {
                    meshEntities.Add(mesh);
                }
            }

            if (meshEntities.Count == 0)
            {
                return;
            }

            foreach (var mesh in meshEntities)
            {
                var meshData = MeshEntityRenderer.Render(mesh);
                Assert.NotNull(meshData);
                Assert.Equal("Mesh", meshData.Type);
                Assert.True(meshData.VertexCount >= 0);
                Assert.True(meshData.FaceCount >= 0);
                Assert.True(meshData.EdgeCount >= 0);
                Assert.NotNull(meshData.Vertices3D);
                Assert.NotNull(meshData.FaceIndices);
                Assert.NotNull(meshData.EdgeIndices);
            }
        }

        [Fact]
        public void LoadAndRenderMeshFromDxf_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dxf");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DxfReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var meshEntities = new List<Mesh>();
            foreach (var entity in doc.Entities)
            {
                if (entity is Mesh mesh)
                {
                    meshEntities.Add(mesh);
                }
            }

            if (meshEntities.Count == 0)
            {
                return;
            }

            foreach (var mesh in meshEntities)
            {
                var meshData = MeshEntityRenderer.Render(mesh);
                Assert.NotNull(meshData);
                Assert.Equal("Mesh", meshData.Type);
                Assert.True(meshData.VertexCount >= 0);
                Assert.True(meshData.FaceCount >= 0);
                Assert.True(meshData.EdgeCount >= 0);
                Assert.NotNull(meshData.Vertices3D);
                Assert.NotNull(meshData.FaceIndices);
                Assert.NotNull(meshData.EdgeIndices);
            }
        }

        [Fact]
        public void SerializeMeshDataToJson_Success()
        {
            var mesh = new Mesh();
            mesh.Layer = new Layer("MESH_LAYER");
            mesh.Color = new ACadSharp.Color(5);
            mesh.LineType = new LineType("CONTINUOUS");
            mesh.LineWeight = LineWeightType.W25;
            mesh.IsInvisible = false;

            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(15, 20, 0));
            mesh.Faces.Add(new int[] { 0, 1, 2 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 0, End = 1 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 1, End = 2 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 2, End = 0 });

            var meshData = MeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(meshData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Mesh", json);
            Assert.Contains("Vertices3D", json);
            Assert.Contains("FaceIndices", json);
            Assert.Contains("EdgeIndices", json);
            Assert.Contains("VertexCount", json);
            Assert.Contains("FaceCount", json);
            Assert.Contains("EdgeCount", json);
        }

        [Fact]
        public void DeserializeMeshDataFromJson_Success()
        {
            var mesh = new Mesh();
            mesh.Layer = new Layer("MESH_LAYER");
            mesh.Color = new ACadSharp.Color(7);
            mesh.LineType = new LineType("DASHED");
            mesh.LineWeight = LineWeightType.W50;
            mesh.IsInvisible = false;

            mesh.Vertices.Add(new XYZ(5, 5, 0));
            mesh.Vertices.Add(new XYZ(15, 5, 0));
            mesh.Vertices.Add(new XYZ(10, 15, 0));
            mesh.Faces.Add(new int[] { 0, 1, 2 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 0, End = 1 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 1, End = 2 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 2, End = 0 });

            var originalMeshData = MeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<MeshEntityRenderer.MeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.Equal(originalMeshData.Type, deserializedMeshData.Type);
            Assert.Equal(originalMeshData.EntityType, deserializedMeshData.EntityType);
            Assert.Equal(originalMeshData.Handle, deserializedMeshData.Handle);
            Assert.Equal(originalMeshData.LayerName, deserializedMeshData.LayerName);
            Assert.Equal(originalMeshData.ColorIndex, deserializedMeshData.ColorIndex);
            Assert.Equal(originalMeshData.LineTypeName, deserializedMeshData.LineTypeName);
            Assert.Equal(originalMeshData.LineWeight, deserializedMeshData.LineWeight);
            Assert.Equal(originalMeshData.Visible, deserializedMeshData.Visible);
            Assert.Equal(originalMeshData.VertexCount, deserializedMeshData.VertexCount);
            Assert.Equal(originalMeshData.FaceCount, deserializedMeshData.FaceCount);
            Assert.Equal(originalMeshData.EdgeCount, deserializedMeshData.EdgeCount);
            Assert.Equal(originalMeshData.Opacity, deserializedMeshData.Opacity);
            Assert.Equal(originalMeshData.Transparent, deserializedMeshData.Transparent);
            Assert.Equal(originalMeshData.MaterialType, deserializedMeshData.MaterialType);
            Assert.Equal(originalMeshData.DepthTest, deserializedMeshData.DepthTest);
            Assert.Equal(originalMeshData.DepthWrite, deserializedMeshData.DepthWrite);
            Assert.Equal(originalMeshData.Side, deserializedMeshData.Side);
            Assert.Equal(originalMeshData.SubdivisionLevel, deserializedMeshData.SubdivisionLevel);
            Assert.Equal(originalMeshData.Version, deserializedMeshData.Version);
            Assert.Equal(originalMeshData.BlendCrease, deserializedMeshData.BlendCrease);
        }

        [Fact]
        public void DeserializeMeshDataFromJson_Vertices3D_Success()
        {
            var mesh = new Mesh();
            mesh.Layer = new Layer("MESH_LAYER");
            mesh.Color = new ACadSharp.Color(3);
            mesh.LineType = new LineType("CONTINUOUS");
            mesh.LineWeight = LineWeightType.ByLayer;
            mesh.IsInvisible = false;

            mesh.Vertices.Add(new XYZ(10, 10, 5));
            mesh.Vertices.Add(new XYZ(20, 10, 5));
            mesh.Vertices.Add(new XYZ(15, 20, 10));
            mesh.Faces.Add(new int[] { 0, 1, 2 });

            var originalMeshData = MeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<MeshEntityRenderer.MeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.Vertices3D);
            Assert.Equal(originalMeshData.Vertices3D.Count, deserializedMeshData.Vertices3D.Count);
            
            for (int i = 0; i < originalMeshData.Vertices3D.Count; i++)
            {
                Assert.Equal(originalMeshData.Vertices3D[i].X, deserializedMeshData.Vertices3D[i].X);
                Assert.Equal(originalMeshData.Vertices3D[i].Y, deserializedMeshData.Vertices3D[i].Y);
                Assert.Equal(originalMeshData.Vertices3D[i].Z, deserializedMeshData.Vertices3D[i].Z);
            }
        }

        [Fact]
        public void DeserializeMeshDataFromJson_FaceIndices_Success()
        {
            var mesh = new Mesh();
            mesh.Layer = new Layer("MESH_LAYER");
            mesh.Color = new ACadSharp.Color(1);
            mesh.LineType = new LineType("CONTINUOUS");
            mesh.LineWeight = LineWeightType.ByLayer;
            mesh.IsInvisible = false;

            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(15, 20, 0));
            mesh.Vertices.Add(new XYZ(15, 10, 10));
            mesh.Faces.Add(new int[] { 0, 1, 2 });
            mesh.Faces.Add(new int[] { 0, 1, 3 });

            var originalMeshData = MeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<MeshEntityRenderer.MeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.FaceIndices);
            Assert.Equal(originalMeshData.FaceIndices.Count, deserializedMeshData.FaceIndices.Count);
            
            for (int i = 0; i < originalMeshData.FaceIndices.Count; i++)
            {
                Assert.Equal(originalMeshData.FaceIndices[i].Length, deserializedMeshData.FaceIndices[i].Length);
                for (int j = 0; j < originalMeshData.FaceIndices[i].Length; j++)
                {
                    Assert.Equal(originalMeshData.FaceIndices[i][j], deserializedMeshData.FaceIndices[i][j]);
                }
            }
        }

        [Fact]
        public void DeserializeMeshDataFromJson_EdgeIndices_Success()
        {
            var mesh = new Mesh();
            mesh.Layer = new Layer("MESH_LAYER");
            mesh.Color = new ACadSharp.Color(2);
            mesh.LineType = new LineType("CONTINUOUS");
            mesh.LineWeight = LineWeightType.ByLayer;
            mesh.IsInvisible = false;

            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(15, 20, 0));
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 0, End = 1 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 1, End = 2 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 2, End = 0 });

            var originalMeshData = MeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<MeshEntityRenderer.MeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.EdgeIndices);
            Assert.Equal(originalMeshData.EdgeIndices.Count, deserializedMeshData.EdgeIndices.Count);
            
            for (int i = 0; i < originalMeshData.EdgeIndices.Count; i++)
            {
                Assert.Equal(originalMeshData.EdgeIndices[i].Length, deserializedMeshData.EdgeIndices[i].Length);
                for (int j = 0; j < originalMeshData.EdgeIndices[i].Length; j++)
                {
                    Assert.Equal(originalMeshData.EdgeIndices[i][j], deserializedMeshData.EdgeIndices[i][j]);
                }
            }
        }

        [Fact]
        public void DeserializeMeshDataFromJson_Bounds3D_Success()
        {
            var mesh = new Mesh();
            mesh.Layer = new Layer("MESH_LAYER");
            mesh.Color = new ACadSharp.Color(4);
            mesh.LineType = new LineType("CONTINUOUS");
            mesh.LineWeight = LineWeightType.ByLayer;
            mesh.IsInvisible = false;

            mesh.Vertices.Add(new XYZ(10, 10, 5));
            mesh.Vertices.Add(new XYZ(20, 10, 5));
            mesh.Vertices.Add(new XYZ(15, 20, 10));
            mesh.Faces.Add(new int[] { 0, 1, 2 });

            var originalMeshData = MeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<MeshEntityRenderer.MeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.Bounds3D);
            Assert.Equal(originalMeshData.Bounds3D.Min.X, deserializedMeshData.Bounds3D.Min.X);
            Assert.Equal(originalMeshData.Bounds3D.Min.Y, deserializedMeshData.Bounds3D.Min.Y);
            Assert.Equal(originalMeshData.Bounds3D.Min.Z, deserializedMeshData.Bounds3D.Min.Z);
            Assert.Equal(originalMeshData.Bounds3D.Max.X, deserializedMeshData.Bounds3D.Max.X);
            Assert.Equal(originalMeshData.Bounds3D.Max.Y, deserializedMeshData.Bounds3D.Max.Y);
            Assert.Equal(originalMeshData.Bounds3D.Max.Z, deserializedMeshData.Bounds3D.Max.Z);
        }

        [Fact]
        public void DeserializeMeshDataFromJson_Centroid3D_Success()
        {
            var mesh = new Mesh();
            mesh.Layer = new Layer("MESH_LAYER");
            mesh.Color = new ACadSharp.Color(6);
            mesh.LineType = new LineType("CONTINUOUS");
            mesh.LineWeight = LineWeightType.W0;
            mesh.IsInvisible = false;

            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(15, 20, 0));
            mesh.Faces.Add(new int[] { 0, 1, 2 });

            var originalMeshData = MeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<MeshEntityRenderer.MeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.Centroid3D);
            Assert.Equal(originalMeshData.Centroid3D.X, deserializedMeshData.Centroid3D.X);
            Assert.Equal(originalMeshData.Centroid3D.Y, deserializedMeshData.Centroid3D.Y);
            Assert.Equal(originalMeshData.Centroid3D.Z, deserializedMeshData.Centroid3D.Z);
        }

        [Fact]
        public void RenderMultipleMeshes_Success()
        {
            var meshes = new List<Mesh>();

            for (int i = 0; i < 5; i++)
            {
                var mesh = new Mesh();
                mesh.Layer = new Layer("MESH_LAYER");
                mesh.Color = new ACadSharp.Color((short)(i + 1));
                mesh.LineType = new LineType("CONTINUOUS");
                mesh.LineWeight = LineWeightType.W0;
                mesh.IsInvisible = false;

                mesh.Vertices.Add(new XYZ(10 + i * 10, 10, 0));
                mesh.Vertices.Add(new XYZ(20 + i * 10, 10, 0));
                mesh.Vertices.Add(new XYZ(15 + i * 10, 20, 0));
                mesh.Faces.Add(new int[] { 0, 1, 2 });
                mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 0, End = 1 });
                mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 1, End = 2 });
                mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 2, End = 0 });

                meshes.Add(mesh);
            }

            foreach (var mesh in meshes)
            {
                var meshData = MeshEntityRenderer.Render(mesh);
                Assert.NotNull(meshData);
                Assert.Equal("Mesh", meshData.Type);
                Assert.Equal(3, meshData.VertexCount);
                Assert.Equal(1, meshData.FaceCount);
                Assert.Equal(3, meshData.EdgeCount);
            }
        }

        [Fact]
        public void SerializeAndDeserializeComplexMesh_Success()
        {
            var mesh = new Mesh();
            mesh.Layer = new Layer("COMPLEX_LAYER");
            mesh.Color = new ACadSharp.Color(8);
            mesh.LineType = new LineType("DASHED");
            mesh.LineWeight = LineWeightType.W70;
            mesh.IsInvisible = false;

            mesh.Vertices.Add(new XYZ(10, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 10, 0));
            mesh.Vertices.Add(new XYZ(20, 20, 0));
            mesh.Vertices.Add(new XYZ(10, 20, 0));
            mesh.Vertices.Add(new XYZ(15, 15, 10));
            
            mesh.Faces.Add(new int[] { 0, 1, 4 });
            mesh.Faces.Add(new int[] { 1, 2, 4 });
            mesh.Faces.Add(new int[] { 2, 3, 4 });
            mesh.Faces.Add(new int[] { 3, 0, 4 });
            
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 0, End = 1 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 1, End = 2 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 2, End = 3 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 3, End = 0 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 0, End = 4 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 1, End = 4 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 2, End = 4 });
            mesh.Edges.Add(new ACadSharp.Entities.Mesh.Edge { Start = 3, End = 4 });

            var originalMeshData = MeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData, Formatting.Indented);
            var deserializedMeshData = JsonConvert.DeserializeObject<MeshEntityRenderer.MeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.Equal(originalMeshData.VertexCount, deserializedMeshData.VertexCount);
            Assert.Equal(originalMeshData.FaceCount, deserializedMeshData.FaceCount);
            Assert.Equal(originalMeshData.EdgeCount, deserializedMeshData.EdgeCount);
            Assert.Equal(originalMeshData.Vertices3D.Count, deserializedMeshData.Vertices3D.Count);
            Assert.Equal(originalMeshData.FaceIndices.Count, deserializedMeshData.FaceIndices.Count);
            Assert.Equal(originalMeshData.EdgeIndices.Count, deserializedMeshData.EdgeIndices.Count);
        }
    }
}
