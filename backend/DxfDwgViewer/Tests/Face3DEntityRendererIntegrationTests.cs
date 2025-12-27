using System;
using System.Text.Json;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using CSMath;
using DxfDwgViewer.RenderUtilities;
using Xunit;

namespace DxfDwgViewer.Tests
{
    public class Face3DEntityRendererIntegrationTests
    {
        [Fact]
        public void SerializeAndDeserialize_TriangleFace_ShouldPreserveAllProperties()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(5),
                Layer = new Layer("TEST_LAYER")
            };

            var renderData = Face3DEntityRenderer.Render(face3D);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<Face3DEntityRenderer.Face3DData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal(3, deserializedData.Vertices.Count / 3);
            Assert.Equal(5, deserializedData.ColorIndex);
            Assert.Equal("TEST_LAYER", deserializedData.LayerName);
        }

        [Fact]
        public void SerializeAndDeserialize_QuadFace_ShouldPreserveAllProperties()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(10, 10, 0),
                FourthCorner = new XYZ(0, 10, 0),
                Color = new ACadSharp.Color(10),
                Layer = new Layer("QUAD_LAYER")
            };

            var renderData = Face3DEntityRenderer.Render(face3D);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<Face3DEntityRenderer.Face3DData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal(12, deserializedData.Vertices.Count);
            Assert.Equal(10, deserializedData.ColorIndex);
            Assert.Equal("QUAD_LAYER", deserializedData.LayerName);
        }

        [Fact]
        public void SerializeAndDeserialize_InvisibleFace_ShouldPreserveVisibility()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(1),
                Layer = new Layer("INVISIBLE_LAYER")
            };
            face3D.IsInvisible = true;

            var renderData = Face3DEntityRenderer.Render(face3D);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<Face3DEntityRenderer.Face3DData>(json);

            Assert.NotNull(deserializedData);
            Assert.True(deserializedData.IsInvisible);
            Assert.False(deserializedData.Visible);
        }

        [Fact]
        public void SerializeAndDeserialize_3DOrientedFace_ShouldPreserve3DCoordinates()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 5),
                SecondCorner = new XYZ(10, 0, 5),
                ThirdCorner = new XYZ(10, 10, 10),
                FourthCorner = new XYZ(0, 10, 10),
                Color = new ACadSharp.Color(15),
                Layer = new Layer("3D_LAYER")
            };

            var renderData = Face3DEntityRenderer.Render(face3D);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<Face3DEntityRenderer.Face3DData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal(12, deserializedData.Vertices.Count);
            Assert.Equal(0, deserializedData.Vertices[0]);
            Assert.Equal(0, deserializedData.Vertices[1]);
            Assert.Equal(5, deserializedData.Vertices[2]);
            Assert.Equal(10, deserializedData.Vertices[3]);
            Assert.Equal(0, deserializedData.Vertices[4]);
            Assert.Equal(5, deserializedData.Vertices[5]);
        }

        [Fact]
        public void SerializeAndDeserialize_MultipleFaces_ShouldMaintainDataIntegrity()
        {
            var faces = new[]
            {
                new Face3D
                {
                    FirstCorner = new XYZ(0, 0, 0),
                    SecondCorner = new XYZ(10, 0, 0),
                    ThirdCorner = new XYZ(5, 10, 0),
                    FourthCorner = new XYZ(5, 10, 0),
                    Color = new ACadSharp.Color(1),
                    Layer = new Layer("FACE_1")
                },
                new Face3D
                {
                    FirstCorner = new XYZ(10, 0, 0),
                    SecondCorner = new XYZ(20, 0, 0),
                    ThirdCorner = new XYZ(20, 10, 0),
                    FourthCorner = new XYZ(10, 10, 0),
                    Color = new ACadSharp.Color(2),
                    Layer = new Layer("FACE_2")
                },
                new Face3D
                {
                    FirstCorner = new XYZ(0, 10, 0),
                    SecondCorner = new XYZ(10, 10, 0),
                    ThirdCorner = new XYZ(5, 20, 0),
                    FourthCorner = new XYZ(5, 20, 0),
                    Color = new ACadSharp.Color(3),
                    Layer = new Layer("FACE_3")
                }
            };

            var renderDataArray = new Face3DEntityRenderer.Face3DData[faces.Length];
            for (int i = 0; i < faces.Length; i++)
            {
                renderDataArray[i] = Face3DEntityRenderer.Render(faces[i]);
            }

            var json = JsonSerializer.Serialize(renderDataArray);
            var deserializedDataArray = JsonSerializer.Deserialize<Face3DEntityRenderer.Face3DData[]>(json);

            Assert.NotNull(deserializedDataArray);
            Assert.Equal(3, deserializedDataArray.Length);

            for (int i = 0; i < faces.Length; i++)
            {
                Assert.NotNull(deserializedDataArray[i]);
                Assert.Equal(i + 1, deserializedDataArray[i].ColorIndex);
                Assert.Equal($"FACE_{i + 1}", deserializedDataArray[i].LayerName);
            }
        }

        [Fact]
        public void SerializeAndDeserialize_FaceWithNormals_ShouldPreserveNormals()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(7),
                Layer = new Layer("NORMALS_LAYER")
            };

            var renderData = Face3DEntityRenderer.Render(face3D);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<Face3DEntityRenderer.Face3DData>(json);

            Assert.NotNull(deserializedData);
            Assert.NotNull(deserializedData.Normals);
            Assert.True(deserializedData.Normals.Count > 0);
        }

        [Fact]
        public void SerializeAndDeserialize_FaceWithIndices_ShouldPreserveIndices()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(10, 10, 0),
                FourthCorner = new XYZ(0, 10, 0),
                Color = new ACadSharp.Color(8),
                Layer = new Layer("INDICES_LAYER")
            };

            var renderData = Face3DEntityRenderer.Render(face3D);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<Face3DEntityRenderer.Face3DData>(json);

            Assert.NotNull(deserializedData);
            Assert.NotNull(deserializedData.Indices);
            Assert.True(deserializedData.Indices.Count > 0);
        }

        [Fact]
        public void SerializeAndDeserialize_FaceWithTransform_ShouldPreserveTransform()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(12),
                Layer = new Layer("TRANSFORM_LAYER")
            };

            var renderData = Face3DEntityRenderer.Render(face3D);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<Face3DEntityRenderer.Face3DData>(json);

            Assert.NotNull(deserializedData);
            Assert.NotNull(deserializedData.Transform);
            Assert.NotNull(deserializedData.Transform.Matrix);
            Assert.Equal(16, deserializedData.Transform.Matrix.Length);
        }

        [Fact]
        public void SerializeAndDeserialize_FaceWithBounds_ShouldPreserveBounds()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(10, 10, 0),
                FourthCorner = new XYZ(0, 10, 0),
                Color = new ACadSharp.Color(20),
                Layer = new Layer("BOUNDS_LAYER")
            };

            var renderData = Face3DEntityRenderer.Render(face3D);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<Face3DEntityRenderer.Face3DData>(json);

            Assert.NotNull(deserializedData);
            Assert.NotNull(deserializedData.Bounds);
            Assert.NotNull(deserializedData.Bounds.Min);
            Assert.NotNull(deserializedData.Bounds.Max);
            Assert.Equal(0, deserializedData.Bounds.Min.X);
            Assert.Equal(0, deserializedData.Bounds.Min.Y);
            Assert.Equal(0, deserializedData.Bounds.Min.Z);
            Assert.Equal(10, deserializedData.Bounds.Max.X);
            Assert.Equal(10, deserializedData.Bounds.Max.Y);
            Assert.Equal(0, deserializedData.Bounds.Max.Z);
        }

        [Fact]
        public void SerializeAndDeserialize_FaceWithGeometricProperties_ShouldPreserveAllProperties()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(10, 10, 0),
                FourthCorner = new XYZ(0, 10, 0),
                Color = new ACadSharp.Color(25),
                Layer = new Layer("GEOMETRY_LAYER")
            };

            var renderData = Face3DEntityRenderer.Render(face3D);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<Face3DEntityRenderer.Face3DData>(json);

            Assert.NotNull(deserializedData);
            Assert.NotNull(deserializedData.Center);
            Assert.NotNull(deserializedData.Normal);
            Assert.True(deserializedData.Area > 0);
            Assert.Equal(5, deserializedData.Center.X);
            Assert.Equal(5, deserializedData.Center.Y);
            Assert.Equal(0, deserializedData.Center.Z);
        }

        [Fact]
        public void SerializeAndDeserialize_LargeFace_ShouldHandleLargeCoordinates()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(1000, 1000, 1000),
                SecondCorner = new XYZ(2000, 1000, 1000),
                ThirdCorner = new XYZ(2000, 2000, 1000),
                FourthCorner = new XYZ(1000, 2000, 1000),
                Color = new ACadSharp.Color(30),
                Layer = new Layer("LARGE_LAYER")
            };

            var renderData = Face3DEntityRenderer.Render(face3D);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<Face3DEntityRenderer.Face3DData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal(12, deserializedData.Vertices.Count);
            Assert.Equal(1000, deserializedData.Vertices[0]);
            Assert.Equal(1000, deserializedData.Vertices[1]);
            Assert.Equal(1000, deserializedData.Vertices[2]);
        }

        [Fact]
        public void SerializeAndDeserialize_NegativeCoordinates_ShouldHandleNegativeValues()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(-10, -10, -10),
                SecondCorner = new XYZ(0, -10, -10),
                ThirdCorner = new XYZ(0, 0, -10),
                FourthCorner = new XYZ(-10, 0, -10),
                Color = new ACadSharp.Color(35),
                Layer = new Layer("NEGATIVE_LAYER")
            };

            var renderData = Face3DEntityRenderer.Render(face3D);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<Face3DEntityRenderer.Face3DData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal(12, deserializedData.Vertices.Count);
            Assert.Equal(-10, deserializedData.Vertices[0]);
            Assert.Equal(-10, deserializedData.Vertices[1]);
            Assert.Equal(-10, deserializedData.Vertices[2]);
        }

        [Fact]
        public void SerializeAndDeserialize_FaceWithShadows_ShouldPreserveShadowProperties()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(40),
                Layer = new Layer("SHADOW_LAYER")
            };

            var renderData = Face3DEntityRenderer.Render(face3D);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<Face3DEntityRenderer.Face3DData>(json);

            Assert.NotNull(deserializedData);
            Assert.True(deserializedData.CastShadow);
            Assert.True(deserializedData.ReceiveShadow);
        }

        [Fact]
        public void SerializeAndDeserialize_FaceWithRenderOrder_ShouldPreserveRenderOrder()
        {
            var face3D = new Face3D
            {
                FirstCorner = new XYZ(0, 0, 0),
                SecondCorner = new XYZ(10, 0, 0),
                ThirdCorner = new XYZ(5, 10, 0),
                FourthCorner = new XYZ(5, 10, 0),
                Color = new ACadSharp.Color(50),
                Layer = new Layer("RENDER_ORDER_LAYER")
            };

            var renderData = Face3DEntityRenderer.Render(face3D);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<Face3DEntityRenderer.Face3DData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal(0, deserializedData.RenderOrder);
        }
    }
}
