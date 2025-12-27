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
    public class PolyfaceMeshEntityRendererIntegrationTests
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
        public void LoadAndRenderPolyfaceMeshFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var polyfaceMeshEntities = new List<PolyfaceMesh>();
            foreach (var entity in doc.Entities)
            {
                if (entity is PolyfaceMesh polyfaceMesh)
                {
                    polyfaceMeshEntities.Add(polyfaceMesh);
                }
            }

            if (polyfaceMeshEntities.Count == 0)
            {
                return;
            }

            foreach (var polyfaceMesh in polyfaceMeshEntities)
            {
                var polyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
                Assert.NotNull(polyfaceMeshData);
                Assert.Equal("PolyfaceMesh", polyfaceMeshData.EntityType);
                Assert.NotNull(polyfaceMeshData.Vertices);
                Assert.NotNull(polyfaceMeshData.Faces);
                Assert.NotNull(polyfaceMeshData.Bounds3D);
            }
        }

        [Fact]
        public void SerializePolyfaceMeshDataToJson_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var polyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(polyfaceMeshData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("PolyfaceMesh", json);
            Assert.Contains("Vertices", json);
            Assert.Contains("Faces", json);
            Assert.Contains("Bounds3D", json);
        }

        [Fact]
        public void DeserializePolyfaceMeshDataFromJson_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.Equal(originalPolyfaceMeshData.EntityType, deserializedPolyfaceMeshData.EntityType);
            Assert.Equal(originalPolyfaceMeshData.VertexCount, deserializedPolyfaceMeshData.VertexCount);
            Assert.Equal(originalPolyfaceMeshData.FaceCount, deserializedPolyfaceMeshData.FaceCount);
            Assert.Equal(originalPolyfaceMeshData.ColorIndex, deserializedPolyfaceMeshData.ColorIndex);
            Assert.Equal(originalPolyfaceMeshData.LineTypeName, deserializedPolyfaceMeshData.LineTypeName);
            Assert.Equal(originalPolyfaceMeshData.LineWeight, deserializedPolyfaceMeshData.LineWeight);
            Assert.Equal(originalPolyfaceMeshData.Visible, deserializedPolyfaceMeshData.Visible);
            Assert.Equal(originalPolyfaceMeshData.LayerName, deserializedPolyfaceMeshData.LayerName);
            Assert.Equal(originalPolyfaceMeshData.Transparency, deserializedPolyfaceMeshData.Transparency);
            Assert.Equal(originalPolyfaceMeshData.MaterialName, deserializedPolyfaceMeshData.MaterialName);
            Assert.Equal(originalPolyfaceMeshData.CastShadows, deserializedPolyfaceMeshData.CastShadows);
            Assert.Equal(originalPolyfaceMeshData.ReceiveShadows, deserializedPolyfaceMeshData.ReceiveShadows);
            Assert.Equal(originalPolyfaceMeshData.DoubleSided, deserializedPolyfaceMeshData.DoubleSided);
            Assert.Equal(originalPolyfaceMeshData.FlatShading, deserializedPolyfaceMeshData.FlatShading);
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyVertices_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.NotNull(deserializedPolyfaceMeshData.Vertices);
            Assert.Equal(originalPolyfaceMeshData.Vertices.Count, deserializedPolyfaceMeshData.Vertices.Count);

            for (int i = 0; i < originalPolyfaceMeshData.Vertices.Count; i++)
            {
                Assert.Equal(originalPolyfaceMeshData.Vertices[i].Location.X, deserializedPolyfaceMeshData.Vertices[i].Location.X);
                Assert.Equal(originalPolyfaceMeshData.Vertices[i].Location.Y, deserializedPolyfaceMeshData.Vertices[i].Location.Y);
                Assert.Equal(originalPolyfaceMeshData.Vertices[i].Location.Z, deserializedPolyfaceMeshData.Vertices[i].Location.Z);
            }
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyFaces_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.NotNull(deserializedPolyfaceMeshData.Faces);
            Assert.Equal(originalPolyfaceMeshData.Faces.Count, deserializedPolyfaceMeshData.Faces.Count);

            for (int i = 0; i < originalPolyfaceMeshData.Faces.Count; i++)
            {
                Assert.Equal(originalPolyfaceMeshData.Faces[i].VertexIndices.Count, deserializedPolyfaceMeshData.Faces[i].VertexIndices.Count);
                for (int j = 0; j < originalPolyfaceMeshData.Faces[i].VertexIndices.Count; j++)
                {
                    Assert.Equal(originalPolyfaceMeshData.Faces[i].VertexIndices[j], deserializedPolyfaceMeshData.Faces[i].VertexIndices[j]);
                }
            }
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyIndices_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.NotNull(deserializedPolyfaceMeshData.Indices);
            Assert.Equal(originalPolyfaceMeshData.Indices.Count, deserializedPolyfaceMeshData.Indices.Count);

            for (int i = 0; i < originalPolyfaceMeshData.Indices.Count; i++)
            {
                Assert.Equal(originalPolyfaceMeshData.Indices[i], deserializedPolyfaceMeshData.Indices[i]);
            }
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyNormalsArray_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.NotNull(deserializedPolyfaceMeshData.NormalsArray);
            Assert.Equal(originalPolyfaceMeshData.NormalsArray.Count, deserializedPolyfaceMeshData.NormalsArray.Count);

            for (int i = 0; i < originalPolyfaceMeshData.NormalsArray.Count; i++)
            {
                Assert.Equal(originalPolyfaceMeshData.NormalsArray[i], deserializedPolyfaceMeshData.NormalsArray[i]);
            }
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyColorsArray_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.NotNull(deserializedPolyfaceMeshData.ColorsArray);
            Assert.Equal(originalPolyfaceMeshData.ColorsArray.Count, deserializedPolyfaceMeshData.ColorsArray.Count);

            for (int i = 0; i < originalPolyfaceMeshData.ColorsArray.Count; i++)
            {
                Assert.Equal(originalPolyfaceMeshData.ColorsArray[i], deserializedPolyfaceMeshData.ColorsArray[i]);
            }
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyUVsArray_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.NotNull(deserializedPolyfaceMeshData.UVsArray);
            Assert.Equal(originalPolyfaceMeshData.UVsArray.Count, deserializedPolyfaceMeshData.UVsArray.Count);

            for (int i = 0; i < originalPolyfaceMeshData.UVsArray.Count; i++)
            {
                Assert.Equal(originalPolyfaceMeshData.UVsArray[i], deserializedPolyfaceMeshData.UVsArray[i]);
            }
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyTransform_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.NotNull(deserializedPolyfaceMeshData.Transform);
            Assert.Equal(originalPolyfaceMeshData.Transform.Position.X, deserializedPolyfaceMeshData.Transform.Position.X);
            Assert.Equal(originalPolyfaceMeshData.Transform.Position.Y, deserializedPolyfaceMeshData.Transform.Position.Y);
            Assert.Equal(originalPolyfaceMeshData.Transform.Position.Z, deserializedPolyfaceMeshData.Transform.Position.Z);
            Assert.Equal(originalPolyfaceMeshData.Transform.Rotation.X, deserializedPolyfaceMeshData.Transform.Rotation.X);
            Assert.Equal(originalPolyfaceMeshData.Transform.Rotation.Y, deserializedPolyfaceMeshData.Transform.Rotation.Y);
            Assert.Equal(originalPolyfaceMeshData.Transform.Rotation.Z, deserializedPolyfaceMeshData.Transform.Rotation.Z);
            Assert.Equal(originalPolyfaceMeshData.Transform.Scale.X, deserializedPolyfaceMeshData.Transform.Scale.X);
            Assert.Equal(originalPolyfaceMeshData.Transform.Scale.Y, deserializedPolyfaceMeshData.Transform.Scale.Y);
            Assert.Equal(originalPolyfaceMeshData.Transform.Scale.Z, deserializedPolyfaceMeshData.Transform.Scale.Z);
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyBounds3D_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.NotNull(deserializedPolyfaceMeshData.Bounds3D);
            Assert.Equal(originalPolyfaceMeshData.Bounds3D.Min.X, deserializedPolyfaceMeshData.Bounds3D.Min.X);
            Assert.Equal(originalPolyfaceMeshData.Bounds3D.Min.Y, deserializedPolyfaceMeshData.Bounds3D.Min.Y);
            Assert.Equal(originalPolyfaceMeshData.Bounds3D.Min.Z, deserializedPolyfaceMeshData.Bounds3D.Min.Z);
            Assert.Equal(originalPolyfaceMeshData.Bounds3D.Max.X, deserializedPolyfaceMeshData.Bounds3D.Max.X);
            Assert.Equal(originalPolyfaceMeshData.Bounds3D.Max.Y, deserializedPolyfaceMeshData.Bounds3D.Max.Y);
            Assert.Equal(originalPolyfaceMeshData.Bounds3D.Max.Z, deserializedPolyfaceMeshData.Bounds3D.Max.Z);
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyCentroid_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.NotNull(deserializedPolyfaceMeshData.Centroid);
            Assert.Equal(originalPolyfaceMeshData.Centroid.X, deserializedPolyfaceMeshData.Centroid.X);
            Assert.Equal(originalPolyfaceMeshData.Centroid.Y, deserializedPolyfaceMeshData.Centroid.Y);
            Assert.Equal(originalPolyfaceMeshData.Centroid.Z, deserializedPolyfaceMeshData.Centroid.Z);
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyVertices3D_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.NotNull(deserializedPolyfaceMeshData.Vertices3D);
            Assert.Equal(originalPolyfaceMeshData.Vertices3D.Count, deserializedPolyfaceMeshData.Vertices3D.Count);

            for (int i = 0; i < originalPolyfaceMeshData.Vertices3D.Count; i++)
            {
                Assert.Equal(originalPolyfaceMeshData.Vertices3D[i].X, deserializedPolyfaceMeshData.Vertices3D[i].X);
                Assert.Equal(originalPolyfaceMeshData.Vertices3D[i].Y, deserializedPolyfaceMeshData.Vertices3D[i].Y);
                Assert.Equal(originalPolyfaceMeshData.Vertices3D[i].Z, deserializedPolyfaceMeshData.Vertices3D[i].Z);
            }
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyNormals_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.NotNull(deserializedPolyfaceMeshData.Normals);
            Assert.Equal(originalPolyfaceMeshData.Normals.Count, deserializedPolyfaceMeshData.Normals.Count);

            for (int i = 0; i < originalPolyfaceMeshData.Normals.Count; i++)
            {
                Assert.Equal(originalPolyfaceMeshData.Normals[i].X, deserializedPolyfaceMeshData.Normals[i].X);
                Assert.Equal(originalPolyfaceMeshData.Normals[i].Y, deserializedPolyfaceMeshData.Normals[i].Y);
                Assert.Equal(originalPolyfaceMeshData.Normals[i].Z, deserializedPolyfaceMeshData.Normals[i].Z);
            }
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyColor_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.NotNull(deserializedPolyfaceMeshData.Color);
            Assert.Equal(originalPolyfaceMeshData.Color.R, deserializedPolyfaceMeshData.Color.R);
            Assert.Equal(originalPolyfaceMeshData.Color.G, deserializedPolyfaceMeshData.Color.G);
            Assert.Equal(originalPolyfaceMeshData.Color.B, deserializedPolyfaceMeshData.Color.B);
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyHandle_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.Equal(originalPolyfaceMeshData.Handle, deserializedPolyfaceMeshData.Handle);
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyGeometryType_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.Equal(originalPolyfaceMeshData.GeometryType, deserializedPolyfaceMeshData.GeometryType);
        }

        [Fact]
        public void DeserializePolyfaceMeshData_VerifyLayerIndex_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();

            var originalPolyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
            var json = JsonConvert.SerializeObject(originalPolyfaceMeshData);
            var deserializedPolyfaceMeshData = JsonConvert.DeserializeObject<PolyfaceMeshEntityRenderer.PolyfaceMeshData>(json);

            Assert.NotNull(deserializedPolyfaceMeshData);
            Assert.Equal(originalPolyfaceMeshData.LayerIndex, deserializedPolyfaceMeshData.LayerIndex);
        }

        [Fact]
        public void RenderPolyfaceMeshWithMultipleFaces_Success()
        {
            var polyfaceMesh = new PolyfaceMesh();
            polyfaceMesh.Color = new Color(1);

            for (int i = 0; i < 10; i++)
            {
                var vertex = new VertexFaceMesh
                {
                    Location = new XYZ(i * 10, 0, 0)
                };
                polyfaceMesh.Vertices.Add(vertex);
            }

            for (int i = 0; i < 8; i++)
            {
                var face = new VertexFaceRecord();
                face.Index1 = (short)(i + 1);
                face.Index2 = (short)(i + 2);
                face.Index3 = (short)(i + 3);
                polyfaceMesh.Faces.Add(face);
            }

            var polyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(polyfaceMeshData);
            Assert.Equal(10, polyfaceMeshData.VertexCount);
            Assert.Equal(8, polyfaceMeshData.FaceCount);
            Assert.Equal(10, polyfaceMeshData.Vertices.Count);
            Assert.Equal(8, polyfaceMeshData.Faces.Count);
        }

        [Fact]
        public void RenderPolyfaceMeshWithQuadFaces_Success()
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

            var polyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(polyfaceMeshData);
            Assert.Equal(4, polyfaceMeshData.VertexCount);
            Assert.Equal(1, polyfaceMeshData.FaceCount);
            Assert.Equal(6, polyfaceMeshData.Indices.Count);
        }

        [Fact]
        public void RenderPolyfaceMeshWithInvisibleFlag_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            polyfaceMesh.IsInvisible = true;

            var polyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(polyfaceMeshData);
            Assert.False(polyfaceMeshData.Visible);
        }

        [Fact]
        public void RenderPolyfaceMeshWithCustomColor_Success()
        {
            var polyfaceMesh = CreateBasicPolyfaceMesh();
            polyfaceMesh.Color = new Color(5);

            var polyfaceMeshData = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);

            Assert.NotNull(polyfaceMeshData);
            Assert.Equal(5, polyfaceMeshData.ColorIndex);
        }
    }
}