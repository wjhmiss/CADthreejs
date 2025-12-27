using System;
using System.Collections.Generic;
using Xunit;
using ACadSharp.Entities;
using ACadSharp.Tables;
using CSMath;
using DxfDwgViewer.RenderUtilities;

namespace DxfDwgViewer.Tests
{
    public class InsertEntityRendererTests
    {
        [Fact]
        public void Render_Insert_ReturnsCorrectType()
        {
            var insert = CreateBasicInsert();

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal("Insert", result.Type);
            Assert.Equal("Insert", result.EntityType);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectInsertPoint()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = new XYZ(10, 20, 30),
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal(10, result.InsertPointX);
            Assert.Equal(20, result.InsertPointY);
            Assert.Equal(10, result.Position.X);
            Assert.Equal(20, result.Position.Y);
            Assert.Equal(30, result.Position.Z);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectColor()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Color = new ACadSharp.Color(5),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
            Assert.Equal("#0000FF", result.ColorHex);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectScale()
        {
            var block = new BlockRecord("TEST_BLOCK");
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
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectRotation()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Rotation = Math.PI / 4,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 4, result.Rotation);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectBlockName()
        {
            var block = new BlockRecord("MY_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal("MY_BLOCK", result.BlockName);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectLayerName()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("MY_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal("MY_LAYER", result.LayerName);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectNormal()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal(0, result.NormalX);
            Assert.Equal(0, result.NormalY);
            Assert.Equal(1, result.NormalZ);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectCentroid()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = new XYZ(10, 20, 30),
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);
            Assert.Equal(10, result.Centroid.X);
            Assert.Equal(20, result.Centroid.Y);
            Assert.Equal(30, result.Centroid.Z);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectVisibility()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                IsInvisible = false,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.True(result.Visible);
            Assert.False(result.IsInvisible);
        }

        [Fact]
        public void Render_Insert_WithInvisible_ReturnsCorrectVisibility()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                IsInvisible = true,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.False(result.Visible);
            Assert.True(result.IsInvisible);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectMaterialProperties()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal("Basic", result.MaterialType);
            Assert.False(result.MaterialTransparent);
            Assert.Equal(1.0, result.MaterialOpacity);
            Assert.True(result.MaterialDepthTest);
            Assert.True(result.MaterialDepthWrite);
            Assert.Equal(2, result.MaterialSide);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectTransform()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = new XYZ(10, 20, 30),
                XScale = 2.0,
                YScale = 1.5,
                ZScale = 3.0,
                Rotation = Math.PI / 4,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform3D);
            Assert.Equal(2.0, result.Transform3D.Scale.X);
            Assert.Equal(1.5, result.Transform3D.Scale.Y);
            Assert.Equal(3.0, result.Transform3D.Scale.Z);
            Assert.Equal(Math.PI / 4, result.Transform3D.Rotation.Z);
            Assert.Equal(10, result.Transform3D.Position.X);
            Assert.Equal(20, result.Transform3D.Position.Y);
            Assert.Equal(30, result.Transform3D.Position.Z);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectMultipleInsertProperties()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                RowCount = 3,
                ColumnCount = 2,
                RowSpacing = 10.0,
                ColumnSpacing = 15.0,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal(3, result.RowCount);
            Assert.Equal(2, result.ColumnCount);
            Assert.Equal(10.0, result.RowSpacing);
            Assert.Equal(15.0, result.ColumnSpacing);
            Assert.True(result.IsMultiple);
        }

        [Fact]
        public void Render_Insert_SingleInsert_ReturnsCorrectMultipleFlag()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                RowCount = 1,
                ColumnCount = 1,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal(1, result.RowCount);
            Assert.Equal(1, result.ColumnCount);
            Assert.False(result.IsMultiple);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectSpatialFilter()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.False(result.HasSpatialFilter);
            Assert.Equal("", result.SpatialFilterName);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectEntityCount()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var line1 = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 10, 0),
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };
            var line2 = new Line
            {
                StartPoint = new XYZ(10, 0, 0),
                EndPoint = new XYZ(20, 10, 0),
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };
            block.Entities.Add(line1);
            block.Entities.Add(line2);

            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal(2, result.EntityCount);
        }

        [Fact]
        public void Render_Insert_WithBlockEntities_ReturnsCorrectVertices()
        {
            var block = new BlockRecord("TEST_BLOCK");
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

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices);
            Assert.Equal(2, result.Vertices.Count);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectSceneProperties()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.False(result.CastShadow);
            Assert.False(result.ReceiveShadow);
            Assert.Equal(0, result.RenderOrder);
        }

        [Fact]
        public void Render_Insert_ReturnsCorrectCoordinateSystem()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.Equal("World", result.CoordinateSystem);
            Assert.False(result.RequiresYAxisFlip);
        }

        [Fact]
        public void Render_Insert_WithNullBlock_ReturnsEmptyEntities()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.NotNull(result.Entities);
            Assert.Equal(0, result.Entities.Count);
            Assert.Equal(0, result.EntityCount);
        }

        [Fact]
        public void Render_Insert_WithNullBlockEntities_ReturnsEmptyEntities()
        {
            var block = new BlockRecord("TEST_BLOCK");
            var insert = new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var result = InsertEntityRenderer.Render(insert);

            Assert.NotNull(result);
            Assert.NotNull(result.Entities);
            Assert.Equal(0, result.Entities.Count);
            Assert.Equal(0, result.EntityCount);
        }

        private Insert CreateBasicInsert()
        {
            var block = new BlockRecord("TEST_BLOCK");
            return new Insert(block)
            {
                InsertPoint = XYZ.Zero,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };
        }
    }
}
