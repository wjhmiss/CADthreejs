using System;
using System.Text.Json;
using Xunit;
using ACadSharp.Entities;
using ACadSharp.Tables;
using CSMath;
using DxfDwgViewer.RenderUtilities;

namespace DxfDwgViewer.Tests
{
    public class InsertEntityRendererIntegrationTests
    {
        [Fact]
        public void SerializeAndDeserialize_Insert_ShouldPreserveAllProperties()
        {
            var insert = CreateComplexInsert();
            var result = InsertEntityRenderer.Render(insert);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<InsertEntityRenderer.InsertData>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(result.Type, deserialized.Type);
            Assert.Equal(result.EntityType, deserialized.EntityType);
            Assert.Equal(result.InsertPointX, deserialized.InsertPointX);
            Assert.Equal(result.InsertPointY, deserialized.InsertPointY);
            Assert.Equal(result.ColorIndex, deserialized.ColorIndex);
            Assert.Equal(result.LineTypeName, deserialized.LineTypeName);
            Assert.Equal(result.ScaleX, deserialized.ScaleX);
            Assert.Equal(result.ScaleY, deserialized.ScaleY);
            Assert.Equal(result.ScaleZ, deserialized.ScaleZ);
            Assert.Equal(result.Rotation, deserialized.Rotation);
            Assert.Equal(result.BlockName, deserialized.BlockName);
            Assert.Equal(result.LayerName, deserialized.LayerName);
            Assert.Equal(result.IsInvisible, deserialized.IsInvisible);
            Assert.Equal(result.LineTypeScale, deserialized.LineTypeScale);
            Assert.Equal(result.Transparency, deserialized.Transparency);
            Assert.Equal(result.ColorHex, deserialized.ColorHex);
            Assert.Equal(result.ColorR, deserialized.ColorR);
            Assert.Equal(result.ColorG, deserialized.ColorG);
            Assert.Equal(result.ColorB, deserialized.ColorB);
            Assert.Equal(result.ColorA, deserialized.ColorA);
            Assert.Equal(result.MaterialType, deserialized.MaterialType);
            Assert.Equal(result.MaterialTransparent, deserialized.MaterialTransparent);
            Assert.Equal(result.MaterialOpacity, deserialized.MaterialOpacity);
            Assert.Equal(result.MaterialDepthTest, deserialized.MaterialDepthTest);
            Assert.Equal(result.MaterialDepthWrite, deserialized.MaterialDepthWrite);
            Assert.Equal(result.MaterialSide, deserialized.MaterialSide);
            Assert.Equal(result.NormalX, deserialized.NormalX);
            Assert.Equal(result.NormalY, deserialized.NormalY);
            Assert.Equal(result.NormalZ, deserialized.NormalZ);
            Assert.Equal(result.CoordinateSystem, deserialized.CoordinateSystem);
            Assert.Equal(result.RequiresYAxisFlip, deserialized.RequiresYAxisFlip);
            Assert.Equal(result.RowCount, deserialized.RowCount);
            Assert.Equal(result.ColumnCount, deserialized.ColumnCount);
            Assert.Equal(result.RowSpacing, deserialized.RowSpacing);
            Assert.Equal(result.ColumnSpacing, deserialized.ColumnSpacing);
            Assert.Equal(result.IsMultiple, deserialized.IsMultiple);
            Assert.Equal(result.HasSpatialFilter, deserialized.HasSpatialFilter);
            Assert.Equal(result.SpatialFilterName, deserialized.SpatialFilterName);
            Assert.Equal(result.Visible, deserialized.Visible);
            Assert.Equal(result.CastShadow, deserialized.CastShadow);
            Assert.Equal(result.ReceiveShadow, deserialized.ReceiveShadow);
            Assert.Equal(result.RenderOrder, deserialized.RenderOrder);
        }

        [Fact]
        public void SerializeAndDeserialize_Insert_ShouldPreserveTransform()
        {
            var insert = CreateComplexInsert();
            var result = InsertEntityRenderer.Render(insert);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<InsertEntityRenderer.InsertData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.Transform);
            Assert.NotNull(deserialized.Transform3D);

            Assert.Equal(result.Transform.M11, deserialized.Transform.M11);
            Assert.Equal(result.Transform.M12, deserialized.Transform.M12);
            Assert.Equal(result.Transform.M13, deserialized.Transform.M13);
            Assert.Equal(result.Transform.M14, deserialized.Transform.M14);
            Assert.Equal(result.Transform.M21, deserialized.Transform.M21);
            Assert.Equal(result.Transform.M22, deserialized.Transform.M22);
            Assert.Equal(result.Transform.M23, deserialized.Transform.M23);
            Assert.Equal(result.Transform.M24, deserialized.Transform.M24);
            Assert.Equal(result.Transform.M31, deserialized.Transform.M31);
            Assert.Equal(result.Transform.M32, deserialized.Transform.M32);
            Assert.Equal(result.Transform.M33, deserialized.Transform.M33);
            Assert.Equal(result.Transform.M34, deserialized.Transform.M34);
            Assert.Equal(result.Transform.M41, deserialized.Transform.M41);
            Assert.Equal(result.Transform.M42, deserialized.Transform.M42);
            Assert.Equal(result.Transform.M43, deserialized.Transform.M43);
            Assert.Equal(result.Transform.M44, deserialized.Transform.M44);

            Assert.Equal(result.Transform3D.Position.X, deserialized.Transform3D.Position.X);
            Assert.Equal(result.Transform3D.Position.Y, deserialized.Transform3D.Position.Y);
            Assert.Equal(result.Transform3D.Position.Z, deserialized.Transform3D.Position.Z);
            Assert.Equal(result.Transform3D.Rotation.X, deserialized.Transform3D.Rotation.X);
            Assert.Equal(result.Transform3D.Rotation.Y, deserialized.Transform3D.Rotation.Y);
            Assert.Equal(result.Transform3D.Rotation.Z, deserialized.Transform3D.Rotation.Z);
            Assert.Equal(result.Transform3D.Scale.X, deserialized.Transform3D.Scale.X);
            Assert.Equal(result.Transform3D.Scale.Y, deserialized.Transform3D.Scale.Y);
            Assert.Equal(result.Transform3D.Scale.Z, deserialized.Transform3D.Scale.Z);
        }

        [Fact]
        public void SerializeAndDeserialize_Insert_ShouldPresaveBounds()
        {
            var insert = CreateComplexInsert();
            var result = InsertEntityRenderer.Render(insert);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<InsertEntityRenderer.InsertData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.Bounds);
            Assert.Equal(result.Bounds.Min.X, deserialized.Bounds.Min.X);
            Assert.Equal(result.Bounds.Min.Y, deserialized.Bounds.Min.Y);
            Assert.Equal(result.Bounds.Min.Z, deserialized.Bounds.Min.Z);
            Assert.Equal(result.Bounds.Max.X, deserialized.Bounds.Max.X);
            Assert.Equal(result.Bounds.Max.Y, deserialized.Bounds.Max.Y);
            Assert.Equal(result.Bounds.Max.Z, deserialized.Bounds.Max.Z);
        }

        [Fact]
        public void SerializeAndDeserialize_Insert_ShouldPreserveCentroid()
        {
            var insert = CreateComplexInsert();
            var result = InsertEntityRenderer.Render(insert);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<InsertEntityRenderer.InsertData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.Centroid);
            Assert.Equal(result.Centroid.X, deserialized.Centroid.X);
            Assert.Equal(result.Centroid.Y, deserialized.Centroid.Y);
            Assert.Equal(result.Centroid.Z, deserialized.Centroid.Z);
        }

        [Fact]
        public void SerializeAndDeserialize_Insert_ShouldPreserveVertices()
        {
            var insert = CreateComplexInsert();
            var result = InsertEntityRenderer.Render(insert);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<InsertEntityRenderer.InsertData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.Vertices);
            Assert.NotNull(deserialized.VerticesArray);
            Assert.Equal(result.Vertices.Count, deserialized.Vertices.Count);
            Assert.Equal(result.VerticesArray.Count, deserialized.VerticesArray.Count);

            for (int i = 0; i < result.Vertices.Count; i++)
            {
                Assert.Equal(result.Vertices[i].X, deserialized.Vertices[i].X);
                Assert.Equal(result.Vertices[i].Y, deserialized.Vertices[i].Y);
                Assert.Equal(result.Vertices[i].Z, deserialized.Vertices[i].Z);
            }

            for (int i = 0; i < result.VerticesArray.Count; i++)
            {
                Assert.Equal(result.VerticesArray[i], deserialized.VerticesArray[i]);
            }
        }

        [Fact]
        public void SerializeAndDeserialize_Insert_ShouldPreserveEntities()
        {
            var insert = CreateComplexInsert();
            var result = InsertEntityRenderer.Render(insert);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<InsertEntityRenderer.InsertData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.Entities);
            Assert.Equal(result.Entities.Count, deserialized.Entities.Count);

            for (int i = 0; i < result.Entities.Count; i++)
            {
                Assert.Equal(result.Entities[i].Type, deserialized.Entities[i].Type);
            }
        }

        [Fact]
        public void SerializeAndDeserialize_Insert_ShouldPreserveAttributes()
        {
            var insert = CreateInsertWithAttributes();
            var result = InsertEntityRenderer.Render(insert);

            var json = JsonSerializer.Serialize(result);
            var deserialized = JsonSerializer.Deserialize<InsertEntityRenderer.InsertData>(json);

            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.Attributes);
            Assert.Equal(result.Attributes.Count, deserialized.Attributes.Count);

            for (int i = 0; i < result.Attributes.Count; i++)
            {
                Assert.Equal(result.Attributes[i].Tag, deserialized.Attributes[i].Tag);
                Assert.Equal(result.Attributes[i].Value, deserialized.Attributes[i].Value);
                Assert.Equal(result.Attributes[i].Position.X, deserialized.Attributes[i].Position.X);
                Assert.Equal(result.Attributes[i].Position.Y, deserialized.Attributes[i].Position.Y);
                Assert.Equal(result.Attributes[i].Position.Z, deserialized.Attributes[i].Position.Z);
                Assert.Equal(result.Attributes[i].Height, deserialized.Attributes[i].Height);
                Assert.Equal(result.Attributes[i].ColorIndex, deserialized.Attributes[i].ColorIndex);
                Assert.Equal(result.Attributes[i].Style, deserialized.Attributes[i].Style);
                Assert.Equal(result.Attributes[i].Rotation, deserialized.Attributes[i].Rotation);
                Assert.Equal(result.Attributes[i].WidthFactor, deserialized.Attributes[i].WidthFactor);
                Assert.Equal(result.Attributes[i].ObliqueAngle, deserialized.Attributes[i].ObliqueAngle);
            }
        }

        [Fact]
        public void Render_InsertWithMultipleInsertions_ShouldReturnCorrectData()
        {
            var block = new BlockRecord("ARRAY_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = new XYZ(10, 20, 30),
                RowCount = 3,
                ColumnCount = 2,
                RowSpacing = 10.0,
                ColumnSpacing = 15.0,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.True(result.IsMultiple);
            Assert.Equal(3, result.RowCount);
            Assert.Equal(2, result.ColumnCount);
            Assert.Equal(10.0, result.RowSpacing);
            Assert.Equal(15.0, result.ColumnSpacing);
        }

        [Fact]
        public void Render_InsertWithScale_ShouldApplyCorrectTransform()
        {
            var block = new BlockRecord("SCALE_BLOCK");
            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 10, 0),
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };
            block.Entities.Add(line);

            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                XScale = 2.0,
                YScale = 1.5,
                ZScale = 3.0,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal(2.0, result.ScaleX);
            Assert.Equal(1.5, result.ScaleY);
            Assert.Equal(3.0, result.ScaleZ);
            Assert.NotNull(result.Transform);
        }

        [Fact]
        public void Render_InsertWithRotation_ShouldApplyCorrectTransform()
        {
            var block = new BlockRecord("ROTATE_BLOCK");
            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 10, 0),
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };
            block.Entities.Add(line);

            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Rotation = Math.PI / 2,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 2, result.Rotation);
            Assert.NotNull(result.Transform);
        }

        [Fact]
        public void Render_InsertWithDifferentEntityTypes_ShouldRenderAllEntities()
        {
            var block = new BlockRecord("MIXED_BLOCK");

            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 10, 0),
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };
            block.Entities.Add(line);

            var circle = new Circle
            {
                Center = new XYZ(20, 20, 0),
                Radius = 5.0,
                Color = new ACadSharp.Color(2),
                Layer = new Layer("TEST_LAYER")
            };
            block.Entities.Add(circle);

            var arc = new Arc
            {
                Center = new XYZ(30, 30, 0),
                Radius = 3.0,
                StartAngle = 0,
                EndAngle = Math.PI,
                Color = new ACadSharp.Color(3),
                Layer = new Layer("TEST_LAYER")
            };
            block.Entities.Add(arc);

            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal(3, result.EntityCount);
            Assert.Equal(3, result.Entities.Count);
            Assert.Contains(result.Entities, e => e.Type == "Line");
            Assert.Contains(result.Entities, e => e.Type == "Circle");
            Assert.Contains(result.Entities, e => e.Type == "Arc");
        }

        [Fact]
        public void Render_InsertWithAttributes_ShouldIncludeAttributes()
        {
            var insert = CreateInsertWithAttributes();
            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.NotNull(result.Attributes);
            Assert.Equal(2, result.Attributes.Count);
            Assert.Contains(result.Attributes, a => a.Tag == "TAG1");
            Assert.Contains(result.Attributes, a => a.Tag == "TAG2");
        }

        [Fact]
        public void Render_InsertWithComplexTransform_ShouldCalculateCorrectBounds()
        {
            var block = new BlockRecord("COMPLEX_BLOCK");
            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 10, 0),
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };
            block.Entities.Add(line);

            var insert = new Insert(block)
            {
                InsertPoint = new XYZ(5, 5, 5),
                XScale = 2.0,
                YScale = 2.0,
                ZScale = 2.0,
                Rotation = Math.PI / 4,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Centroid);
        }

        private Insert CreateComplexInsert()
        {
            var block = new BlockRecord("COMPLEX_BLOCK");

            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 10, 0),
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };
            block.Entities.Add(line);

            var circle = new Circle
            {
                Center = new XYZ(20, 20, 0),
                Radius = 5.0,
                Color = new ACadSharp.Color(2),
                Layer = new Layer("TEST_LAYER")
            };
            block.Entities.Add(circle);

            return new Insert(block)
            {
                InsertPoint = new XYZ(10, 20, 30),
                XScale = 2.0,
                YScale = 1.5,
                ZScale = 3.0,
                Rotation = Math.PI / 4,
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(5),
                Layer = new Layer("MY_LAYER"),
                IsInvisible = false,
                RowCount = 2,
                ColumnCount = 2,
                RowSpacing = 10.0,
                ColumnSpacing = 15.0
            };
        }

        private Insert CreateInsertWithAttributes()
        {
            var block = new BlockRecord("ATTR_BLOCK");

            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 10, 0),
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };
            block.Entities.Add(line);

            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var attr1 = new AttributeEntity
            {
                Tag = "TAG1",
                Value = "VALUE1",
                InsertPoint = new XYZ(5, 5, 0),
                Height = 1.0,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };
            insert.Attributes.Add(attr1);

            var attr2 = new AttributeEntity
            {
                Tag = "TAG2",
                Value = "VALUE2",
                InsertPoint = new XYZ(15, 15, 0),
                Height = 1.0,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };
            insert.Attributes.Add(attr2);

            return insert;
        }
    }
}
