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
    public class PolygonMeshEntityRendererIntegrationTests
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

            return mesh;
        }

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
                var meshData = PolygonMeshEntityRenderer.Render(mesh);
                Assert.NotNull(meshData);
                Assert.Equal("PolygonMesh", meshData.EntityType);
                Assert.NotNull(meshData.Vertices);
                Assert.NotNull(meshData.Faces);
                Assert.NotNull(meshData.Bounds3D);
            }
        }

        [Fact]
        public void SerializeMeshDataToJson_Success()
        {
            var mesh = CreateBasicMesh();

            var meshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(meshData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("PolygonMesh", json);
            Assert.Contains("Vertices", json);
            Assert.Contains("Faces", json);
            Assert.Contains("Bounds3D", json);
        }

        [Fact]
        public void DeserializeMeshDataFromJson_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.Equal(originalMeshData.EntityType, deserializedMeshData.EntityType);
            Assert.Equal(originalMeshData.VertexCount, deserializedMeshData.VertexCount);
            Assert.Equal(originalMeshData.FaceCount, deserializedMeshData.FaceCount);
            Assert.Equal(originalMeshData.EdgeCount, deserializedMeshData.EdgeCount);
            Assert.Equal(originalMeshData.ColorIndex, deserializedMeshData.ColorIndex);
            Assert.Equal(originalMeshData.LineTypeName, deserializedMeshData.LineTypeName);
            Assert.Equal(originalMeshData.LineWeight, deserializedMeshData.LineWeight);
            Assert.Equal(originalMeshData.Visible, deserializedMeshData.Visible);
            Assert.Equal(originalMeshData.LayerName, deserializedMeshData.LayerName);
            Assert.Equal(originalMeshData.Transparency, deserializedMeshData.Transparency);
            Assert.Equal(originalMeshData.MaterialName, deserializedMeshData.MaterialName);
            Assert.Equal(originalMeshData.CastShadows, deserializedMeshData.CastShadows);
            Assert.Equal(originalMeshData.ReceiveShadows, deserializedMeshData.ReceiveShadows);
            Assert.Equal(originalMeshData.DoubleSided, deserializedMeshData.DoubleSided);
            Assert.Equal(originalMeshData.FlatShading, deserializedMeshData.FlatShading);
        }

        [Fact]
        public void DeserializeMeshData_VerifyVertices_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.Vertices);
            Assert.Equal(originalMeshData.Vertices.Count, deserializedMeshData.Vertices.Count);

            for (int i = 0; i < originalMeshData.Vertices.Count; i++)
            {
                Assert.Equal(originalMeshData.Vertices[i].Location.X, deserializedMeshData.Vertices[i].Location.X);
                Assert.Equal(originalMeshData.Vertices[i].Location.Y, deserializedMeshData.Vertices[i].Location.Y);
                Assert.Equal(originalMeshData.Vertices[i].Location.Z, deserializedMeshData.Vertices[i].Location.Z);
            }
        }

        [Fact]
        public void DeserializeMeshData_VerifyFaces_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.Faces);
            Assert.Equal(originalMeshData.Faces.Count, deserializedMeshData.Faces.Count);

            for (int i = 0; i < originalMeshData.Faces.Count; i++)
            {
                Assert.Equal(originalMeshData.Faces[i].VertexIndices.Count, deserializedMeshData.Faces[i].VertexIndices.Count);
                for (int j = 0; j < originalMeshData.Faces[i].VertexIndices.Count; j++)
                {
                    Assert.Equal(originalMeshData.Faces[i].VertexIndices[j], deserializedMeshData.Faces[i].VertexIndices[j]);
                }
            }
        }

        [Fact]
        public void DeserializeMeshData_VerifyEdges_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.Edges);
            Assert.Equal(originalMeshData.Edges.Count, deserializedMeshData.Edges.Count);

            for (int i = 0; i < originalMeshData.Edges.Count; i++)
            {
                Assert.Equal(originalMeshData.Edges[i].VertexIndex1, deserializedMeshData.Edges[i].VertexIndex1);
                Assert.Equal(originalMeshData.Edges[i].VertexIndex2, deserializedMeshData.Edges[i].VertexIndex2);
                Assert.Equal(originalMeshData.Edges[i].Crease, deserializedMeshData.Edges[i].Crease);
            }
        }

        [Fact]
        public void DeserializeMeshData_VerifyIndices_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.Indices);
            Assert.Equal(originalMeshData.Indices.Count, deserializedMeshData.Indices.Count);

            for (int i = 0; i < originalMeshData.Indices.Count; i++)
            {
                Assert.Equal(originalMeshData.Indices[i], deserializedMeshData.Indices[i]);
            }
        }

        [Fact]
        public void DeserializeMeshData_VerifyNormalsArray_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.NormalsArray);
            Assert.Equal(originalMeshData.NormalsArray.Count, deserializedMeshData.NormalsArray.Count);

            for (int i = 0; i < originalMeshData.NormalsArray.Count; i++)
            {
                Assert.Equal(originalMeshData.NormalsArray[i], deserializedMeshData.NormalsArray[i]);
            }
        }

        [Fact]
        public void DeserializeMeshData_VerifyColorsArray_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.ColorsArray);
            Assert.Equal(originalMeshData.ColorsArray.Count, deserializedMeshData.ColorsArray.Count);

            for (int i = 0; i < originalMeshData.ColorsArray.Count; i++)
            {
                Assert.Equal(originalMeshData.ColorsArray[i], deserializedMeshData.ColorsArray[i]);
            }
        }

        [Fact]
        public void DeserializeMeshData_VerifyUVsArray_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.UVsArray);
            Assert.Equal(originalMeshData.UVsArray.Count, deserializedMeshData.UVsArray.Count);

            for (int i = 0; i < originalMeshData.UVsArray.Count; i++)
            {
                Assert.Equal(originalMeshData.UVsArray[i], deserializedMeshData.UVsArray[i]);
            }
        }

        [Fact]
        public void DeserializeMeshData_VerifyTransform_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.Transform);
            Assert.Equal(originalMeshData.Transform.Position.X, deserializedMeshData.Transform.Position.X);
            Assert.Equal(originalMeshData.Transform.Position.Y, deserializedMeshData.Transform.Position.Y);
            Assert.Equal(originalMeshData.Transform.Position.Z, deserializedMeshData.Transform.Position.Z);
            Assert.Equal(originalMeshData.Transform.Rotation.X, deserializedMeshData.Transform.Rotation.X);
            Assert.Equal(originalMeshData.Transform.Rotation.Y, deserializedMeshData.Transform.Rotation.Y);
            Assert.Equal(originalMeshData.Transform.Rotation.Z, deserializedMeshData.Transform.Rotation.Z);
            Assert.Equal(originalMeshData.Transform.Scale.X, deserializedMeshData.Transform.Scale.X);
            Assert.Equal(originalMeshData.Transform.Scale.Y, deserializedMeshData.Transform.Scale.Y);
            Assert.Equal(originalMeshData.Transform.Scale.Z, deserializedMeshData.Transform.Scale.Z);
        }

        [Fact]
        public void DeserializeMeshData_VerifyBounds3D_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

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
        public void DeserializeMeshData_VerifyCentroid_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.Centroid);
            Assert.Equal(originalMeshData.Centroid.X, deserializedMeshData.Centroid.X);
            Assert.Equal(originalMeshData.Centroid.Y, deserializedMeshData.Centroid.Y);
            Assert.Equal(originalMeshData.Centroid.Z, deserializedMeshData.Centroid.Z);
        }

        [Fact]
        public void DeserializeMeshData_VerifyVertices3D_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

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
        public void DeserializeMeshData_VerifyNormals_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.Normals);
            Assert.Equal(originalMeshData.Normals.Count, deserializedMeshData.Normals.Count);

            for (int i = 0; i < originalMeshData.Normals.Count; i++)
            {
                Assert.Equal(originalMeshData.Normals[i].X, deserializedMeshData.Normals[i].X);
                Assert.Equal(originalMeshData.Normals[i].Y, deserializedMeshData.Normals[i].Y);
                Assert.Equal(originalMeshData.Normals[i].Z, deserializedMeshData.Normals[i].Z);
            }
        }

        [Fact]
        public void DeserializeMeshData_VerifyColor_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.NotNull(deserializedMeshData.Color);
            Assert.Equal(originalMeshData.Color.R, deserializedMeshData.Color.R);
            Assert.Equal(originalMeshData.Color.G, deserializedMeshData.Color.G);
            Assert.Equal(originalMeshData.Color.B, deserializedMeshData.Color.B);
        }

        [Fact]
        public void DeserializeMeshData_VerifyHandle_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.Equal(originalMeshData.Handle, deserializedMeshData.Handle);
        }

        [Fact]
        public void DeserializeMeshData_VerifyGeometryType_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.Equal(originalMeshData.GeometryType, deserializedMeshData.GeometryType);
        }

        [Fact]
        public void DeserializeMeshData_VerifyLayerIndex_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.Equal(originalMeshData.LayerIndex, deserializedMeshData.LayerIndex);
        }

        [Fact]
        public void DeserializeMeshData_VerifySubdivisionLevel_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.Equal(originalMeshData.SubdivisionLevel, deserializedMeshData.SubdivisionLevel);
        }

        [Fact]
        public void DeserializeMeshData_VerifyVersion_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.Equal(originalMeshData.Version, deserializedMeshData.Version);
        }

        [Fact]
        public void DeserializeMeshData_VerifyBlendCrease_Success()
        {
            var mesh = CreateBasicMesh();

            var originalMeshData = PolygonMeshEntityRenderer.Render(mesh);
            var json = JsonConvert.SerializeObject(originalMeshData);
            var deserializedMeshData = JsonConvert.DeserializeObject<PolygonMeshEntityRenderer.PolygonMeshData>(json);

            Assert.NotNull(deserializedMeshData);
            Assert.Equal(originalMeshData.BlendCrease, deserializedMeshData.BlendCrease);
        }

        [Fact]
        public void RenderMeshWithMultipleFaces_Success()
        {
            var mesh = new Mesh();
            mesh.Color = new Color(1);

            for (int i = 0; i < 10; i++)
            {
                mesh.Vertices.Add(new XYZ(i * 10, 0, 0));
            }

            for (int i = 0; i < 8; i++)
            {
                mesh.Faces.Add(new int[] { i, i + 1, i + 2 });
            }

            var meshData = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(meshData);
            Assert.Equal(10, meshData.VertexCount);
            Assert.Equal(8, meshData.FaceCount);
            Assert.Equal(10, meshData.Vertices.Count);
            Assert.Equal(8, meshData.Faces.Count);
        }

        [Fact]
        public void RenderMeshWithEdges_Success()
        {
            var mesh = new Mesh();
            mesh.Color = new Color(1);

            mesh.Vertices.Add(new XYZ(0, 0, 0));
            mesh.Vertices.Add(new XYZ(10, 0, 0));
            mesh.Vertices.Add(new XYZ(10, 10, 0));

            mesh.Faces.Add(new int[] { 0, 1, 2 });

            mesh.Edges.Add(new Mesh.Edge { VertexIndex1 = 0, VertexIndex2 = 1, Crease = 0.5 });
            mesh.Edges.Add(new Mesh.Edge { VertexIndex1 = 1, VertexIndex2 = 2, Crease = 0.5 });
            mesh.Edges.Add(new Mesh.Edge { VertexIndex1 = 2, VertexIndex2 = 0, Crease = 0.5 });

            var meshData = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(meshData);
            Assert.Equal(3, meshData.Vertices.Count);
            Assert.Equal(1, meshData.Faces.Count);
            Assert.Equal(3, meshData.Edges.Count);
        }

        [Fact]
        public void RenderMeshWithInvisibleFlag_Success()
        {
            var mesh = CreateBasicMesh();
            mesh.IsInvisible = true;

            var meshData = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(meshData);
            Assert.False(meshData.Visible);
        }

        [Fact]
        public void RenderMeshWithCustomColor_Success()
        {
            var mesh = CreateBasicMesh();
            mesh.Color = new Color(5);

            var meshData = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(meshData);
            Assert.Equal(5, meshData.ColorIndex);
        }

        [Fact]
        public void RenderMeshWithSubdivisionLevel_Success()
        {
            var mesh = CreateBasicMesh();
            mesh.SubdivisionLevel = 2;

            var meshData = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(meshData);
            Assert.Equal(2, meshData.SubdivisionLevel);
        }

        [Fact]
        public void RenderMeshWithBlendCrease_Success()
        {
            var mesh = CreateBasicMesh();
            mesh.BlendCrease = 0.75;

            var meshData = PolygonMeshEntityRenderer.Render(mesh);

            Assert.NotNull(meshData);
            Assert.Equal(0.75, meshData.BlendCrease);
        }
    }
}