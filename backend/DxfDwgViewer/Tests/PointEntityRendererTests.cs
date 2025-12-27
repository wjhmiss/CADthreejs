using System;
using Xunit;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using DxfDwgViewer.RenderUtilities;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class PointEntityRendererTests
    {
        private ACadSharp.Entities.Point CreateBasicPoint()
        {
            var point = new ACadSharp.Entities.Point
            {
                Location = new XYZ(10, 10, 0)
            };
            point.Color = new Color(1);
            return point;
        }

        [Fact]
        public void Render_Point_ReturnsCorrectType()
        {
            var point = CreateBasicPoint();
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal("Point", result.Type);
            Assert.Equal("Point", result.EntityType);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectLocation()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(100, 200, 50);
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal(100, result.Location.X);
            Assert.Equal(200, result.Location.Y);
            Assert.Equal(50, result.Location.Z);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectColorIndex()
        {
            var point = CreateBasicPoint();
            point.Color = new Color(5);
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectRotation()
        {
            var point = CreateBasicPoint();
            point.Rotation = Math.PI / 4;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 4, result.Rotation);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectSize()
        {
            var point = CreateBasicPoint();
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal(6.0, result.Size);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectBounds3D()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(10, 10, 0);
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds3D);
            Assert.Equal(7.0, result.Bounds3D.Min.X);
            Assert.Equal(7.0, result.Bounds3D.Min.Y);
            Assert.Equal(-3.0, result.Bounds3D.Min.Z);
            Assert.Equal(13.0, result.Bounds3D.Max.X);
            Assert.Equal(13.0, result.Bounds3D.Max.Y);
            Assert.Equal(3.0, result.Bounds3D.Max.Z);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectCentroid3D()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(100, 200, 50);
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid3D);
            Assert.Equal(100, result.Centroid3D.X);
            Assert.Equal(200, result.Centroid3D.Y);
            Assert.Equal(50, result.Centroid3D.Z);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectNormal()
        {
            var point = CreateBasicPoint();
            point.Normal = new XYZ(0, 0, 1);
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal(0, result.Normal.X);
            Assert.Equal(0, result.Normal.Y);
            Assert.Equal(1, result.Normal.Z);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectTransform()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(10, 20, 30);
            point.Rotation = Math.PI / 6;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.Equal(1.0, result.Transform.Scale.X);
            Assert.Equal(1.0, result.Transform.Scale.Y);
            Assert.Equal(1.0, result.Transform.Scale.Z);
            Assert.Equal(Math.PI / 6, result.Transform.Rotation.Z);
            Assert.Equal(10, result.Transform.Position.X);
            Assert.Equal(20, result.Transform.Position.Y);
            Assert.Equal(30, result.Transform.Position.Z);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectHandle()
        {
            var point = CreateBasicPoint();
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal(point.Handle.ToString(), result.Handle);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectLayerName()
        {
            var point = CreateBasicPoint();
            var layer = new Layer("TEST_LAYER");
            point.Layer = layer;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal("TEST_LAYER", result.LayerName);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectVisible()
        {
            var point = CreateBasicPoint();
            point.IsInvisible = false;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.True(result.Visible);
        }

        [Fact]
        public void Render_Point_Invisible_ReturnsCorrectVisible()
        {
            var point = CreateBasicPoint();
            point.IsInvisible = true;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.False(result.Visible);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectThickness()
        {
            var point = CreateBasicPoint();
            point.Thickness = 10;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal(10, result.Thickness);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectLineTypeName()
        {
            var point = CreateBasicPoint();
            var lineType = new LineType("DASHED");
            point.LineType = lineType;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal("DASHED", result.LineTypeName);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectLineWeight()
        {
            var point = CreateBasicPoint();
            point.LineWeight = LineWeightType.W30;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal(0.3, result.LineWeight);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectMaterial()
        {
            var point = CreateBasicPoint();
            point.Color = new Color(1);
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);
            Assert.Equal("PointsMaterial", result.Material.Type);
            Assert.Equal(1.0, result.Material.Opacity);
            Assert.False(result.Material.Transparent);
            Assert.True(result.Material.DepthTest);
            Assert.True(result.Material.SizeAttenuation);
            Assert.Equal(6.0, result.Material.Size);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectGeometry()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(10, 20, 30);
            point.Rotation = Math.PI / 4;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);
            Assert.Equal("BufferGeometry", result.Geometry.Type);
            Assert.Equal(10, result.Geometry.Position.X);
            Assert.Equal(20, result.Geometry.Position.Y);
            Assert.Equal(30, result.Geometry.Position.Z);
            Assert.Equal(0, result.Geometry.Rotation.X);
            Assert.Equal(0, result.Geometry.Rotation.Y);
            Assert.Equal(Math.PI / 4, result.Geometry.Rotation.Z);
            Assert.Equal(1.0, result.Geometry.Scale.X);
            Assert.Equal(1.0, result.Geometry.Scale.Y);
            Assert.Equal(1.0, result.Geometry.Scale.Z);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectColorData()
        {
            var point = CreateBasicPoint();
            point.Color = new Color(1);
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);
            Assert.Equal(1, result.Color.Index);
            Assert.Equal(255, result.Color.R);
            Assert.Equal(0, result.Color.G);
            Assert.Equal(0, result.Color.B);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectOpacity()
        {
            var point = CreateBasicPoint();
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal(1.0, result.Opacity);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectTransparent()
        {
            var point = CreateBasicPoint();
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.False(result.Transparent);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectDepthTest()
        {
            var point = CreateBasicPoint();
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.True(result.DepthTest);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectCoordinateSystem()
        {
            var point = CreateBasicPoint();
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal("WCS", result.CoordinateSystem);
        }

        [Fact]
        public void Render_Point_WithZeroLocation_ReturnsCorrectBounds()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(0, 0, 0);
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds3D);
            Assert.Equal(-3.0, result.Bounds3D.Min.X);
            Assert.Equal(-3.0, result.Bounds3D.Min.Y);
            Assert.Equal(-3.0, result.Bounds3D.Min.Z);
            Assert.Equal(3.0, result.Bounds3D.Max.X);
            Assert.Equal(3.0, result.Bounds3D.Max.Y);
            Assert.Equal(3.0, result.Bounds3D.Max.Z);
        }

        [Fact]
        public void Render_Point_WithNegativeLocation_ReturnsCorrectBounds()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(-10, -20, -30);
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds3D);
            Assert.Equal(-13.0, result.Bounds3D.Min.X);
            Assert.Equal(-23.0, result.Bounds3D.Min.Y);
            Assert.Equal(-33.0, result.Bounds3D.Min.Z);
            Assert.Equal(-7.0, result.Bounds3D.Max.X);
            Assert.Equal(-17.0, result.Bounds3D.Max.Y);
            Assert.Equal(-27.0, result.Bounds3D.Max.Z);
        }

        [Fact]
        public void Render_Point_WithFullRotation_HandlesCorrectly()
        {
            var point = CreateBasicPoint();
            point.Rotation = 2 * Math.PI;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.InRange(result.Rotation, 2 * Math.PI - 0.0001, 2 * Math.PI + 0.0001);
        }

        [Fact]
        public void Render_Point_WithCustomNormal_ReturnsCorrectNormal()
        {
            var point = CreateBasicPoint();
            point.Normal = new XYZ(0.5, 0.5, 0.7071);
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.InRange(result.Normal.X, 0.49, 0.51);
            Assert.InRange(result.Normal.Y, 0.49, 0.51);
            Assert.InRange(result.Normal.Z, 0.70, 0.71);
        }

        [Fact]
        public void Render_Point_WithNullLayer_ReturnsEmptyLayerName()
        {
            var point = new ACadSharp.Entities.Point();
            point.Location = new XYZ(10, 10, 0);
            point.Color = new ACadSharp.Color(7);
            point.LineType = new LineType("CONTINUOUS");
            point.LineWeight = LineWeightType.ByLayer;
            point.IsInvisible = false;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.LayerName);
        }

        [Fact]
        public void Render_Point_WithDefaultLineType_ReturnsDefaultLineTypeName()
        {
            var point = CreateBasicPoint();
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.LineTypeName);
        }

        [Fact]
        public void Render_Point_WithDefaultLineWeight_ReturnsDefaultLineWeight()
        {
            var point = CreateBasicPoint();
            point.LineWeight = LineWeightType.ByLayer;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal(0.03, result.LineWeight);
        }

        [Fact]
        public void Render_Point_WithZeroThickness_ReturnsCorrectThickness()
        {
            var point = CreateBasicPoint();
            point.Thickness = 0;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal(0, result.Thickness);
        }

        [Fact]
        public void Render_Point_WithNegativeThickness_ReturnsCorrectThickness()
        {
            var point = CreateBasicPoint();
            point.Thickness = -10;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal(-10, result.Thickness);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectGeometryBoundingBox()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(10, 20, 30);
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry.BoundingBox);
            Assert.Equal(7.0, result.Geometry.BoundingBox.Min.X);
            Assert.Equal(17.0, result.Geometry.BoundingBox.Min.Y);
            Assert.Equal(27.0, result.Geometry.BoundingBox.Min.Z);
            Assert.Equal(13.0, result.Geometry.BoundingBox.Max.X);
            Assert.Equal(23.0, result.Geometry.BoundingBox.Max.Y);
            Assert.Equal(33.0, result.Geometry.BoundingBox.Max.Z);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectGeometryNormal()
        {
            var point = CreateBasicPoint();
            point.Normal = new XYZ(0, 0, 1);
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry.Normal);
            Assert.Equal(0, result.Geometry.Normal.X);
            Assert.Equal(0, result.Geometry.Normal.Y);
            Assert.Equal(1, result.Geometry.Normal.Z);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectGeometrySize()
        {
            var point = CreateBasicPoint();
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.Equal(6.0, result.Geometry.Size);
        }

        [Fact]
        public void Render_Point_WithDifferentColors_ReturnsCorrectColorData()
        {
            var point = CreateBasicPoint();
            point.Color = new Color(3);
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);
            Assert.Equal(3, result.Color.Index);
            Assert.Equal(0, result.Color.R);
            Assert.Equal(255, result.Color.G);
            Assert.Equal(0, result.Color.B);
        }

        [Fact]
        public void Render_Point_ReturnsCorrectMaterialColor()
        {
            var point = CreateBasicPoint();
            point.Color = new Color(5);
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.Material.Color);
            Assert.Equal(5, result.Material.Color.Index);
        }

        [Fact]
        public void Render_Point_WithRotation_ReturnsCorrectGeometryRotation()
        {
            var point = CreateBasicPoint();
            point.Rotation = Math.PI / 2;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry.Rotation);
            Assert.Equal(0, result.Geometry.Rotation.X);
            Assert.Equal(0, result.Geometry.Rotation.Y);
            Assert.Equal(Math.PI / 2, result.Geometry.Rotation.Z);
        }

        [Fact]
        public void Render_Point_WithNoRotation_ReturnsZeroGeometryRotation()
        {
            var point = CreateBasicPoint();
            point.Rotation = 0;
            var result = PointEntityRenderer.Render(point);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry.Rotation);
            Assert.Equal(0, result.Geometry.Rotation.X);
            Assert.Equal(0, result.Geometry.Rotation.Y);
            Assert.Equal(0, result.Geometry.Rotation.Z);
        }
    }
}
