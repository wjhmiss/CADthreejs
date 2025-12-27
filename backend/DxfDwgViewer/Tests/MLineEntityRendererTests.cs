using Xunit;
using ACadSharp.Entities;
using ACadSharp.Tables;
using ACadSharp.Objects;
using DxfDwgViewer.RenderUtilities;
using CSMath;
using System;

namespace DxfDwgViewer.Tests
{
    public class MLineEntityRendererTests
    {
        [Fact]
        public void Render_MLine_ReturnsCorrectType()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal("MLine", result.Type);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectEntityType()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal("MLine", result.EntityType);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectHandle()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Handle);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectLayerName()
        {
            var mline = CreateBasicMLine();
            mline.Layer = new Layer("MY_LAYER");

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal("MY_LAYER", result.LayerName);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectVisibility()
        {
            var mline = CreateBasicMLine();
            mline.IsInvisible = false;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.True(result.Visible);
        }

        [Fact]
        public void Render_MLine_WithInvisible_ReturnsCorrectVisibility()
        {
            var mline = CreateBasicMLine();
            mline.IsInvisible = true;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.False(result.Visible);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectElementCount()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal(4, result.ElementCount);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectVertexCount()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal(3, result.VertexCount);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectScaleFactor()
        {
            var mline = CreateBasicMLine();
            mline.ScaleFactor = 2.5;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal(2.5, result.ScaleFactor);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectColorIndex()
        {
            var mline = CreateBasicMLine();
            mline.Color = new ACadSharp.Color(5);

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectColorData()
        {
            var mline = CreateBasicMLine();
            mline.Color = new ACadSharp.Color(5);

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);
            Assert.Equal(5, result.Color.Index);
            Assert.Equal("#0000FF", result.Color.Hex);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectFillColorIndex()
        {
            var mline = CreateBasicMLine();
            mline.Style.FillColor = new ACadSharp.Color(3);

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal(3, result.FillColorIndex);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectFillOn()
        {
            var mline = CreateBasicMLine();
            mline.Style.Flags = MLineStyleFlags.FillOn;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.True(result.FillOn);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectFillOff()
        {
            var mline = CreateBasicMLine();
            mline.Style.Flags = MLineStyleFlags.None;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.False(result.FillOn);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectIsClosed()
        {
            var mline = CreateBasicMLine();
            mline.Flags = MLineFlags.Closed;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.True(result.IsClosed);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectIsNotClosed()
        {
            var mline = CreateBasicMLine();
            mline.Flags = 0;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.False(result.IsClosed);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectJustification()
        {
            var mline = CreateBasicMLine();
            mline.Justification = MLineJustification.Top;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal(MLineJustification.Top, result.Justification);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectJustification_Zero()
        {
            var mline = CreateBasicMLine();
            mline.Justification = MLineJustification.Zero;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal(MLineJustification.Zero, result.Justification);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectJustification_Bottom()
        {
            var mline = CreateBasicMLine();
            mline.Justification = MLineJustification.Bottom;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal(MLineJustification.Bottom, result.Justification);
        }

        [Fact]
        public void Render_MLine_CalculatesCorrectTotalLength()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal(20, result.TotalLength);
        }

        [Fact]
        public void Render_MLine_CalculatesCorrectStartPoint()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.StartPoint);
            Assert.Equal(0, result.StartPoint.X);
            Assert.Equal(0, result.StartPoint.Y);
            Assert.Equal(0, result.StartPoint.Z);
        }

        [Fact]
        public void Render_MLine_CalculatesCorrectEndPoint()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.EndPoint);
            Assert.Equal(20, result.EndPoint.X);
            Assert.Equal(0, result.EndPoint.Y);
            Assert.Equal(0, result.EndPoint.Z);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectStyleName()
        {
            var mline = CreateBasicMLine();
            mline.Style.Name = "MY_STYLE";

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal("MY_STYLE", result.StyleName);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectStartAngle()
        {
            var mline = CreateBasicMLine();
            mline.Style.StartAngle = Math.PI / 4;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 4, result.StartAngle);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectEndAngle()
        {
            var mline = CreateBasicMLine();
            mline.Style.EndAngle = Math.PI / 2;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 2, result.EndAngle);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectHasStartCaps()
        {
            var mline = CreateBasicMLine();
            mline.Flags = 0;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.True(result.HasStartCaps);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectNoStartCaps()
        {
            var mline = CreateBasicMLine();
            mline.Flags = MLineFlags.NoStartCaps;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.False(result.HasStartCaps);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectHasEndCaps()
        {
            var mline = CreateBasicMLine();
            mline.Flags = 0;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.True(result.HasEndCaps);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectNoEndCaps()
        {
            var mline = CreateBasicMLine();
            mline.Flags = MLineFlags.NoEndCaps;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.False(result.HasEndCaps);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectDisplayJoints()
        {
            var mline = CreateBasicMLine();
            mline.Style.Flags = MLineStyleFlags.DisplayJoints;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.True(result.DisplayJoints);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectNoDisplayJoints()
        {
            var mline = CreateBasicMLine();
            mline.Style.Flags = MLineStyleFlags.None;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.False(result.DisplayJoints);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectNormal()
        {
            var mline = CreateBasicMLine();
            mline.Normal = new XYZ(0, 0, 1);

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Normal);
            Assert.Equal(0, result.Normal.X);
            Assert.Equal(0, result.Normal.Y);
            Assert.Equal(1, result.Normal.Z);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectTransform()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Matrix);
            Assert.Equal(16, result.Transform.Matrix.Length);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectBounds()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);
            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(0, result.Bounds.Min.Z);
            Assert.Equal(20, result.Bounds.Max.X);
            Assert.Equal(0, result.Bounds.Max.Y);
            Assert.Equal(0, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectBounds3D()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds3D);
            Assert.NotNull(result.Bounds3D.Min);
            Assert.NotNull(result.Bounds3D.Max);
            Assert.Equal(0, result.Bounds3D.Min.X);
            Assert.Equal(0, result.Bounds3D.Min.Y);
            Assert.Equal(0, result.Bounds3D.Min.Z);
            Assert.Equal(20, result.Bounds3D.Max.X);
            Assert.Equal(0, result.Bounds3D.Max.Y);
            Assert.Equal(0, result.Bounds3D.Max.Z);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectCentroid()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);
            Assert.Equal(10, result.Centroid.X);
            Assert.Equal(0, result.Centroid.Y);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectCentroid3D()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid3D);
            Assert.Equal(10, result.Centroid3D.X);
            Assert.Equal(0, result.Centroid3D.Y);
            Assert.Equal(0, result.Centroid3D.Z);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectVertices3D()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices3D);
            Assert.Equal(3, result.Vertices3D.Count);
            Assert.Equal(0, result.Vertices3D[0].X);
            Assert.Equal(0, result.Vertices3D[0].Y);
            Assert.Equal(0, result.Vertices3D[0].Z);
            Assert.Equal(10, result.Vertices3D[1].X);
            Assert.Equal(0, result.Vertices3D[1].Y);
            Assert.Equal(0, result.Vertices3D[1].Z);
            Assert.Equal(20, result.Vertices3D[2].X);
            Assert.Equal(0, result.Vertices3D[2].Y);
            Assert.Equal(0, result.Vertices3D[2].Z);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectNormals()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Normals);
            Assert.Equal(3, result.Normals.Count);
            Assert.Equal(0, result.Normals[0].X);
            Assert.Equal(0, result.Normals[0].Y);
            Assert.Equal(1, result.Normals[0].Z);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectLineIndices()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.LineIndices);
            Assert.Equal(2, result.LineIndices.Count);
            Assert.Equal(0, result.LineIndices[0][0]);
            Assert.Equal(1, result.LineIndices[0][1]);
            Assert.Equal(1, result.LineIndices[1][0]);
            Assert.Equal(2, result.LineIndices[1][1]);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectOffsets()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Offsets);
            Assert.Equal(4, result.Offsets.Count);
            Assert.Equal(0.5, result.Offsets[0]);
            Assert.Equal(-0.5, result.Offsets[1]);
            Assert.Equal(-0.5, result.Offsets[2]);
            Assert.Equal(0.5, result.Offsets[3]);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectElementColorIndices()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.ElementColorIndices);
            Assert.Equal(4, result.ElementColorIndices.Count);
            Assert.Equal(256, result.ElementColorIndices[0]);
            Assert.Equal(256, result.ElementColorIndices[1]);
            Assert.Equal(1, result.ElementColorIndices[2]);
            Assert.Equal(2, result.ElementColorIndices[3]);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectMaterialProperties()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal("LineBasicMaterial", result.MaterialType);
            Assert.Equal(1.0, result.Opacity);
            Assert.False(result.Transparent);
            Assert.True(result.DepthTest);
            Assert.True(result.DepthWrite);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectCoordinateSystem()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal("World", result.CoordinateSystem);
        }

        [Fact]
        public void Render_MLine_WithZeroVertices_ReturnsEmptyElements()
        {
            var mline = CreateBasicMLine();
            mline.Vertices.Clear();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Elements);
            Assert.Empty(result.Elements);
        }

        [Fact]
        public void Render_MLine_WithSingleVertex_ReturnsEmptyElements()
        {
            var mline = CreateBasicMLine();
            mline.Vertices.Clear();
            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(0, 0, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Elements);
            Assert.Empty(result.Elements);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectElements()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Elements);
            Assert.Equal(4, result.Elements.Count);
            Assert.Equal(0.5, result.Elements[0].Offset);
            Assert.Equal(-0.5, result.Elements[1].Offset);
            Assert.Equal(-0.5, result.Elements[2].Offset);
            Assert.Equal(0.5, result.Elements[3].Offset);
            Assert.Equal(256, result.Elements[0].ColorIndex);
            Assert.Equal(256, result.Elements[1].ColorIndex);
            Assert.Equal(1, result.Elements[2].ColorIndex);
            Assert.Equal(2, result.Elements[3].ColorIndex);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectElementPoints()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Elements);
            Assert.Equal(4, result.Elements.Count);
            Assert.NotNull(result.Elements[0].Points);
            Assert.Equal(3, result.Elements[0].Points.Count);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectElementPointCount()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Elements);
            Assert.Equal(4, result.Elements.Count);
            Assert.Equal(3, result.Elements[0].PointCount);
            Assert.Equal(3, result.Elements[1].PointCount);
            Assert.Equal(3, result.Elements[2].PointCount);
            Assert.Equal(3, result.Elements[3].PointCount);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectElementTotalLength()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Elements);
            Assert.Equal(4, result.Elements.Count);
            Assert.Equal(20, result.Elements[0].TotalLength);
            Assert.Equal(20, result.Elements[1].TotalLength);
            Assert.Equal(20, result.Elements[2].TotalLength);
            Assert.Equal(20, result.Elements[3].TotalLength);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectVertices()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices);
            Assert.Equal(3, result.Vertices.Count);
            Assert.NotNull(result.Vertices[0].Position);
            Assert.Equal(0, result.Vertices[0].Position.X);
            Assert.Equal(0, result.Vertices[0].Position.Y);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectVertexDirection()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices);
            Assert.Equal(3, result.Vertices.Count);
            Assert.NotNull(result.Vertices[0].Direction);
            Assert.Equal(1, result.Vertices[0].Direction.X);
            Assert.Equal(0, result.Vertices[0].Direction.Y);
        }

        [Fact]
        public void Render_MLine_ReturnsCorrectVertexMiter()
        {
            var mline = CreateBasicMLine();

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices);
            Assert.Equal(3, result.Vertices.Count);
            Assert.NotNull(result.Vertices[0].Miter);
            Assert.Equal(0, result.Vertices[0].Miter.X);
            Assert.Equal(1, result.Vertices[0].Miter.Y);
        }

        [Fact]
        public void Render_MLine_With3DCoordinates_CalculatesCorrectLength()
        {
            var mline = CreateBasicMLine();
            mline.Vertices.Clear();
            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(0, 0, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });
            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(10, 0, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });
            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(20, 0, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal(20, result.TotalLength);
        }

        [Fact]
        public void Render_MLine_WithNegativeCoordinates_CalculatesCorrectBounds()
        {
            var mline = CreateBasicMLine();
            mline.Vertices.Clear();
            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(-10, -10, -5),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });
            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(0, 0, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });
            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(10, 10, 5),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.Equal(-10, result.Bounds.Min.X);
            Assert.Equal(-10, result.Bounds.Min.Y);
            Assert.Equal(-5, result.Bounds.Min.Z);
            Assert.Equal(10, result.Bounds.Max.X);
            Assert.Equal(10, result.Bounds.Max.Y);
            Assert.Equal(5, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_MLine_WithDifferentScaleFactor_CalculatesCorrectBounds()
        {
            var mline = CreateBasicMLine();
            mline.ScaleFactor = 2.0;

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(0, result.Bounds.Min.Z);
            Assert.Equal(20, result.Bounds.Max.X);
            Assert.Equal(0, result.Bounds.Max.Y);
            Assert.Equal(0, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_MLine_WithCustomNormal_CreatesCorrectTransform()
        {
            var mline = CreateBasicMLine();
            mline.Normal = new XYZ(0, 1, 0);

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Normal);
            Assert.Equal(0, result.Normal.X);
            Assert.Equal(1, result.Normal.Y);
            Assert.Equal(0, result.Normal.Z);
        }

        [Fact]
        public void Render_MLine_WithLayerByBlockColor_ReturnsCorrectColor()
        {
            var mline = CreateBasicMLine();
            mline.Color = new ACadSharp.Color(0);

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);
            Assert.Equal(0, result.Color.Index);
            Assert.Equal("#FFFFFF", result.Color.Hex);
        }

        [Fact]
        public void Render_MLine_WithDifferentColorIndex_ReturnsCorrectColor()
        {
            var mline = CreateBasicMLine();
            mline.Color = new ACadSharp.Color(1);

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);
            Assert.Equal(1, result.Color.Index);
            Assert.Equal("#FF0000", result.Color.Hex);
        }

        [Fact]
        public void Render_MLine_WithLargeCoordinates_CalculatesCorrectLength()
        {
            var mline = CreateBasicMLine();
            mline.Vertices.Clear();
            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(1000, 2000, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });
            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(1500, 2000, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });
            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(2000, 2000, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.Equal(1000, result.TotalLength);
        }

        [Fact]
        public void Render_MLine_WithVerySmallCoordinates_CalculatesCorrectLength()
        {
            var mline = CreateBasicMLine();
            mline.Vertices.Clear();
            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(0.001, 0.001, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });
            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(0.0015, 0.001, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });
            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(0.002, 0.001, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });

            var result = MLineEntityRenderer.Render(mline);

            Assert.NotNull(result);
            Assert.InRange(result.TotalLength, 0.0009, 0.0011);
        }

        private MLine CreateBasicMLine()
        {
            var mline = new MLine();
            mline.ScaleFactor = 1.0;
            mline.Color = new ACadSharp.Color(7);
            mline.Normal = new XYZ(0, 0, 1);
            mline.Justification = MLineJustification.Zero;
            mline.Flags = 0;
            mline.Style.Name = "STANDARD";
            mline.Style.StartAngle = 0;
            mline.Style.EndAngle = 0;
            mline.Style.Flags = MLineStyleFlags.None;
            mline.Style.FillColor = new ACadSharp.Color(0);

            mline.Style.AddElement(new MLineStyle.Element
            {
                Offset = -0.5,
                Color = new ACadSharp.Color(1),
                LineType = new LineType("CONTINUOUS")
            });

            mline.Style.AddElement(new MLineStyle.Element
            {
                Offset = 0.5,
                Color = new ACadSharp.Color(2),
                LineType = new LineType("CONTINUOUS")
            });

            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(0, 0, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });

            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(10, 0, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });

            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(20, 0, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });

            return mline;
        }
    }
}
