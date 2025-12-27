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
    public class PointEntityRendererIntegrationTests
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
        public void LoadAndRenderPointFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var pointEntities = new List<ACadSharp.Entities.Point>();
            foreach (var entity in doc.Entities)
            {
                if (entity is ACadSharp.Entities.Point point)
                {
                    pointEntities.Add(point);
                }
            }

            if (pointEntities.Count == 0)
            {
                return;
            }

            foreach (var point in pointEntities)
            {
                var pointData = PointEntityRenderer.Render(point);
                Assert.NotNull(pointData);
                Assert.Equal("Point", pointData.Type);
                Assert.NotNull(pointData.Location);
                Assert.NotNull(pointData.Bounds3D);
            }
        }

        [Fact]
        public void SerializePointDataToJson_Success()
        {
            var point = CreateBasicPoint();

            var pointData = PointEntityRenderer.Render(point);
            var json = JsonConvert.SerializeObject(pointData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Point", json);
            Assert.Contains("Location", json);
            Assert.Contains("Bounds3D", json);
        }

        [Fact]
        public void DeserializePointDataFromJson_Success()
        {
            var point = CreateBasicPoint();

            var originalPointData = PointEntityRenderer.Render(point);
            var json = JsonConvert.SerializeObject(originalPointData);
            var deserializedPointData = JsonConvert.DeserializeObject<PointEntityRenderer.PointData>(json);

            Assert.NotNull(deserializedPointData);
            Assert.Equal(originalPointData.Type, deserializedPointData.Type);
            Assert.Equal(originalPointData.Location.X, deserializedPointData.Location.X);
            Assert.Equal(originalPointData.Location.Y, deserializedPointData.Location.Y);
            Assert.Equal(originalPointData.Location.Z, deserializedPointData.Location.Z);
            Assert.Equal(originalPointData.ColorIndex, deserializedPointData.ColorIndex);
            Assert.Equal(originalPointData.Rotation, deserializedPointData.Rotation);
            Assert.Equal(originalPointData.Size, deserializedPointData.Size);
            Assert.Equal(originalPointData.Thickness, deserializedPointData.Thickness);
            Assert.Equal(originalPointData.Visible, deserializedPointData.Visible);
            Assert.Equal(originalPointData.Opacity, deserializedPointData.Opacity);
            Assert.Equal(originalPointData.Transparent, deserializedPointData.Transparent);
            Assert.Equal(originalPointData.DepthTest, deserializedPointData.DepthTest);
        }

        [Fact]
        public void RenderMultiplePoints_CollectAllPointData()
        {
            var points = new List<ACadSharp.Entities.Point>
            {
                CreateBasicPoint(),
                CreateBasicPoint()
            };
            points[0].Location = new XYZ(10, 10, 0);
            points[1].Location = new XYZ(20, 20, 0);

            var pointDataList = new List<PointEntityRenderer.PointData>();
            foreach (var point in points)
            {
                var pointData = PointEntityRenderer.Render(point);
                pointDataList.Add(pointData);
            }

            Assert.Equal(2, pointDataList.Count);
            Assert.Equal(10, pointDataList[0].Location.X);
            Assert.Equal(20, pointDataList[1].Location.X);
        }

        [Fact]
        public void RenderPointWithComplexProperties_PreservesAllProperties()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(100, 200, 300);
            point.Rotation = Math.PI / 4;
            point.Normal = new XYZ(0.5, 0.5, 0.7071);
            point.Thickness = 10;
            point.Color = new Color(5);
            point.IsInvisible = false;

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal(100, pointData.Location.X);
            Assert.Equal(200, pointData.Location.Y);
            Assert.Equal(300, pointData.Location.Z);
            Assert.Equal(Math.PI / 4, pointData.Rotation);
            Assert.InRange(pointData.Normal.X, 0.49, 0.51);
            Assert.InRange(pointData.Normal.Y, 0.49, 0.51);
            Assert.InRange(pointData.Normal.Z, 0.70, 0.71);
            Assert.Equal(10, pointData.Thickness);
            Assert.Equal(5, pointData.ColorIndex);
            Assert.True(pointData.Visible);
        }

        [Fact]
        public void RenderPointWith3DCoordinates_CalculatesCorrect3DProperties()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(10, 20, 30);
            point.Normal = new XYZ(0, 0, 1);

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal(10, pointData.Location.X);
            Assert.Equal(20, pointData.Location.Y);
            Assert.Equal(30, pointData.Location.Z);
            Assert.Equal(10, pointData.Centroid3D.X);
            Assert.Equal(20, pointData.Centroid3D.Y);
            Assert.Equal(30, pointData.Centroid3D.Z);
            Assert.Equal(0, pointData.Normal.X);
            Assert.Equal(0, pointData.Normal.Y);
            Assert.Equal(1, pointData.Normal.Z);
        }

        [Fact]
        public void RenderPointWithRotation_CalculatesCorrectTransform()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(10, 20, 30);
            point.Rotation = Math.PI / 2;

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.NotNull(pointData.Transform);
            Assert.Equal(1.0, pointData.Transform.Scale.X);
            Assert.Equal(1.0, pointData.Transform.Scale.Y);
            Assert.Equal(1.0, pointData.Transform.Scale.Z);
            Assert.Equal(Math.PI / 2, pointData.Transform.Rotation.Z);
            Assert.Equal(10, pointData.Transform.Position.X);
            Assert.Equal(20, pointData.Transform.Position.Y);
            Assert.Equal(30, pointData.Transform.Position.Z);
        }

        [Fact]
        public void RenderPointWithDifferentRotations_CalculatesCorrectTransform()
        {
            var rotations = new[]
            {
                0.0,
                Math.PI / 4,
                Math.PI / 2,
                Math.PI,
                2 * Math.PI
            };

            foreach (var rotation in rotations)
            {
                var point = CreateBasicPoint();
                point.Rotation = rotation;

                var pointData = PointEntityRenderer.Render(point);

                Assert.NotNull(pointData);
                Assert.NotNull(pointData.Transform);
                Assert.Equal(rotation, pointData.Transform.Rotation.Z);
            }
        }

        [Fact]
        public void RenderPointWithLayer_PreservesLayerInformation()
        {
            var point = CreateBasicPoint();
            point.Layer = new Layer("TestLayer");

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal("TestLayer", pointData.LayerName);
        }

        [Fact]
        public void RenderPointWithColor_PreservesColorInformation()
        {
            var point = CreateBasicPoint();
            point.Color = new Color(5);

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal(5, pointData.ColorIndex);
            Assert.Equal(5, pointData.Color.Index);
        }

        [Fact]
        public void RenderPointWithLineType_PreservesLineTypeInformation()
        {
            var point = CreateBasicPoint();
            var lineType = new LineType("Continuous");
            point.LineType = lineType;

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal("Continuous", pointData.LineTypeName);
        }

        [Fact]
        public void RenderPointWithLineWeight_PreservesLineWeightInformation()
        {
            var point = CreateBasicPoint();
            point.LineWeight = ACadSharp.LineWeightType.W30;

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal(0.3, pointData.LineWeight);
        }

        [Fact]
        public void RenderPointWithInvisibleFlag_SetsVisibleProperty()
        {
            var point = CreateBasicPoint();
            point.IsInvisible = true;

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.False(pointData.Visible);
        }

        [Fact]
        public void RenderPointWithHandle_PreservesHandleInformation()
        {
            var point = CreateBasicPoint();
            
            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal(point.Handle.ToString(), pointData.Handle);
        }

        [Fact]
        public void RenderPointWithZeroLocation_HandlesCorrectly()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(0, 0, 0);

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal(0, pointData.Location.X);
            Assert.Equal(0, pointData.Location.Y);
            Assert.Equal(0, pointData.Location.Z);
            Assert.Equal(-3.0, pointData.Bounds3D.Min.X);
            Assert.Equal(-3.0, pointData.Bounds3D.Min.Y);
            Assert.Equal(-3.0, pointData.Bounds3D.Min.Z);
            Assert.Equal(3.0, pointData.Bounds3D.Max.X);
            Assert.Equal(3.0, pointData.Bounds3D.Max.Y);
            Assert.Equal(3.0, pointData.Bounds3D.Max.Z);
        }

        [Fact]
        public void RenderPointWithVeryLargeCoordinates_HandlesCorrectly()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(1000000, 2000000, 3000000);

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal(1000000, pointData.Location.X);
            Assert.Equal(2000000, pointData.Location.Y);
            Assert.Equal(3000000, pointData.Location.Z);
        }

        [Fact]
        public void RenderPointWithVerySmallCoordinates_HandlesCorrectly()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(0.0001, 0.0002, 0.0003);

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal(0.0001, pointData.Location.X);
            Assert.Equal(0.0002, pointData.Location.Y);
            Assert.Equal(0.0003, pointData.Location.Z);
        }

        [Fact]
        public void RenderPointWithNegativeCoordinates_HandlesCorrectly()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(-10, -20, -30);

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal(-10, pointData.Location.X);
            Assert.Equal(-20, pointData.Location.Y);
            Assert.Equal(-30, pointData.Location.Z);
            Assert.Equal(-13.0, pointData.Bounds3D.Min.X);
            Assert.Equal(-23.0, pointData.Bounds3D.Min.Y);
            Assert.Equal(-33.0, pointData.Bounds3D.Min.Z);
        }

        [Fact]
        public void RenderPointWithZeroRotation_HandlesCorrectly()
        {
            var point = CreateBasicPoint();
            point.Rotation = 0;

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal(0, pointData.Rotation);
        }

        [Fact]
        public void RenderPointWithFullRotation_HandlesCorrectly()
        {
            var point = CreateBasicPoint();
            point.Rotation = 2 * Math.PI;

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.InRange(pointData.Rotation, 2 * Math.PI - 0.0001, 2 * Math.PI + 0.0001);
        }

        [Fact]
        public void RenderPointWithDifferentNormals_PreservesNormals()
        {
            var normals = new[]
            {
                new XYZ(0, 0, 1),
                new XYZ(1, 0, 0),
                new XYZ(0, 1, 0),
                new XYZ(0.5, 0.5, 0.7071)
            };

            foreach (var normal in normals)
            {
                var point = CreateBasicPoint();
                point.Normal = normal;

                var pointData = PointEntityRenderer.Render(point);

                Assert.NotNull(pointData);
                Assert.InRange(pointData.Normal.X, normal.X - 0.0001, normal.X + 0.0001);
                Assert.InRange(pointData.Normal.Y, normal.Y - 0.0001, normal.Y + 0.0001);
                Assert.InRange(pointData.Normal.Z, normal.Z - 0.0001, normal.Z + 0.0001);
            }
        }

        [Fact]
        public void RenderPointWithDifferentThickness_PreservesThickness()
        {
            var thicknesses = new[] { 0.0, 1.0, 10.0, -10.0 };

            foreach (var thickness in thicknesses)
            {
                var point = CreateBasicPoint();
                point.Thickness = thickness;

                var pointData = PointEntityRenderer.Render(point);

                Assert.NotNull(pointData);
                Assert.Equal(thickness, pointData.Thickness);
            }
        }

        [Fact]
        public void RenderPointWithDifferentColors_PreservesColors()
        {
            var colors = new[] { 1, 2, 3, 5, 7 };

            foreach (var colorIndex in colors)
            {
                var point = CreateBasicPoint();
                point.Color = new Color((short)colorIndex);

                var pointData = PointEntityRenderer.Render(point);

                Assert.NotNull(pointData);
                Assert.Equal(colorIndex, pointData.ColorIndex);
                Assert.Equal(colorIndex, pointData.Color.Index);
            }
        }

        [Fact]
        public void RenderPointWithDifferentLineWeights_PreservesLineWeights()
        {
            var lineWeights = new[]
            {
                ACadSharp.LineWeightType.W0,
                ACadSharp.LineWeightType.W30,
                ACadSharp.LineWeightType.W50
            };

            foreach (var lineWeight in lineWeights)
            {
                var point = CreateBasicPoint();
                point.LineWeight = lineWeight;

                var pointData = PointEntityRenderer.Render(point);

                Assert.NotNull(pointData);
                Assert.Equal(lineWeight.GetLineWeightValue(), pointData.LineWeight);
            }
        }

        [Fact]
        public void RenderPointWithMaterial_ReturnsCorrectMaterial()
        {
            var point = CreateBasicPoint();
            point.Color = new Color(5);

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.NotNull(pointData.Material);
            Assert.Equal("PointsMaterial", pointData.Material.Type);
            Assert.Equal(1.0, pointData.Material.Opacity);
            Assert.False(pointData.Material.Transparent);
            Assert.True(pointData.Material.DepthTest);
            Assert.True(pointData.Material.SizeAttenuation);
            Assert.Equal(6.0, pointData.Material.Size);
            Assert.Equal(5, pointData.Material.Color.Index);
        }

        [Fact]
        public void RenderPointWithGeometry_ReturnsCorrectGeometry()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(10, 20, 30);
            point.Rotation = Math.PI / 4;
            point.Normal = new XYZ(0, 0, 1);

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.NotNull(pointData.Geometry);
            Assert.Equal("BufferGeometry", pointData.Geometry.Type);
            Assert.Equal(10, pointData.Geometry.Position.X);
            Assert.Equal(20, pointData.Geometry.Position.Y);
            Assert.Equal(30, pointData.Geometry.Position.Z);
            Assert.Equal(0, pointData.Geometry.Rotation.X);
            Assert.Equal(0, pointData.Geometry.Rotation.Y);
            Assert.Equal(Math.PI / 4, pointData.Geometry.Rotation.Z);
            Assert.Equal(1.0, pointData.Geometry.Scale.X);
            Assert.Equal(1.0, pointData.Geometry.Scale.Y);
            Assert.Equal(1.0, pointData.Geometry.Scale.Z);
            Assert.Equal(0, pointData.Geometry.Normal.X);
            Assert.Equal(0, pointData.Geometry.Normal.Y);
            Assert.Equal(1, pointData.Geometry.Normal.Z);
            Assert.Equal(6.0, pointData.Geometry.Size);
        }

        [Fact]
        public void RenderPointWithTransform_ReturnsCorrectTransform()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(10, 20, 30);
            point.Rotation = Math.PI / 3;

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.NotNull(pointData.Transform);
            Assert.Equal(1.0, pointData.Transform.Scale.X);
            Assert.Equal(1.0, pointData.Transform.Scale.Y);
            Assert.Equal(1.0, pointData.Transform.Scale.Z);
            Assert.Equal(Math.PI / 3, pointData.Transform.Rotation.Z);
            Assert.Equal(10, pointData.Transform.Position.X);
            Assert.Equal(20, pointData.Transform.Position.Y);
            Assert.Equal(30, pointData.Transform.Position.Z);
        }

        [Fact]
        public void RenderPointWithDefaultProperties_ReturnsCorrectDefaults()
        {
            var point = new ACadSharp.Entities.Point();
            point.Location = new XYZ(10, 10, 0);
            point.Color = new ACadSharp.Color(7);
            point.LineType = new LineType("CONTINUOUS");
            point.LineWeight = LineWeightType.ByLayer;
            point.IsInvisible = false;

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal(0, pointData.Rotation);
            Assert.Equal(0, pointData.Thickness);
            Assert.Equal(0, pointData.Normal.X);
            Assert.Equal(0, pointData.Normal.Y);
            Assert.Equal(1, pointData.Normal.Z);
            Assert.True(pointData.Visible);
            Assert.Equal(1.0, pointData.Opacity);
            Assert.False(pointData.Transparent);
            Assert.True(pointData.DepthTest);
            Assert.Equal("WCS", pointData.CoordinateSystem);
        }

        [Fact]
        public void RenderPointWithNullLayer_ReturnsDefaultLayerName()
        {
            var point = new ACadSharp.Entities.Point();
            point.Location = new XYZ(10, 10, 0);
            point.Color = new ACadSharp.Color(7);
            point.LineType = new LineType("CONTINUOUS");
            point.LineWeight = LineWeightType.ByLayer;
            point.IsInvisible = false;

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.NotNull(pointData.LayerName);
        }

        [Fact]
        public void RenderPointWithDefaultLineType_ReturnsDefaultLineTypeName()
        {
            var point = CreateBasicPoint();

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.NotNull(pointData.LineTypeName);
        }

        [Fact]
        public void RenderPointWithByLayerLineWeight_ReturnsCorrectValue()
        {
            var point = CreateBasicPoint();
            point.LineWeight = LineWeightType.ByLayer;

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal(0.03, pointData.LineWeight);
        }

        [Fact]
        public void RenderPointWithCentroid_ReturnsCorrectCentroid()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(100, 200, 300);

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.NotNull(pointData.Centroid3D);
            Assert.Equal(100, pointData.Centroid3D.X);
            Assert.Equal(200, pointData.Centroid3D.Y);
            Assert.Equal(300, pointData.Centroid3D.Z);
        }

        [Fact]
        public void RenderPointWithBounds3D_ReturnsCorrectBounds()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(10, 20, 30);

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.NotNull(pointData.Bounds3D);
            Assert.Equal(7.0, pointData.Bounds3D.Min.X);
            Assert.Equal(17.0, pointData.Bounds3D.Min.Y);
            Assert.Equal(27.0, pointData.Bounds3D.Min.Z);
            Assert.Equal(13.0, pointData.Bounds3D.Max.X);
            Assert.Equal(23.0, pointData.Bounds3D.Max.Y);
            Assert.Equal(33.0, pointData.Bounds3D.Max.Z);
        }

        [Fact]
        public void RenderPointWithGeometryBoundingBox_ReturnsCorrectBoundingBox()
        {
            var point = CreateBasicPoint();
            point.Location = new XYZ(10, 20, 30);

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.NotNull(pointData.Geometry.BoundingBox);
            Assert.Equal(7.0, pointData.Geometry.BoundingBox.Min.X);
            Assert.Equal(17.0, pointData.Geometry.BoundingBox.Min.Y);
            Assert.Equal(27.0, pointData.Geometry.BoundingBox.Min.Z);
            Assert.Equal(13.0, pointData.Geometry.BoundingBox.Max.X);
            Assert.Equal(23.0, pointData.Geometry.BoundingBox.Max.Y);
            Assert.Equal(33.0, pointData.Geometry.BoundingBox.Max.Z);
        }

        [Fact]
        public void RenderPointWithCoordinateSystem_ReturnsCorrectCoordinateSystem()
        {
            var point = CreateBasicPoint();

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal("WCS", pointData.CoordinateSystem);
        }

        [Fact]
        public void RenderPointWithDepthSettings_ReturnsCorrectDepthSettings()
        {
            var point = CreateBasicPoint();

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.True(pointData.DepthTest);
            Assert.False(pointData.Transparent);
            Assert.Equal(1.0, pointData.Opacity);
        }

        [Fact]
        public void RenderPointWithMaterialSizeAttenuation_ReturnsCorrectSetting()
        {
            var point = CreateBasicPoint();

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.NotNull(pointData.Material);
            Assert.True(pointData.Material.SizeAttenuation);
        }

        [Fact]
        public void RenderPointWithMaterialDepthTest_ReturnsCorrectSetting()
        {
            var point = CreateBasicPoint();

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.NotNull(pointData.Material);
            Assert.True(pointData.Material.DepthTest);
        }

        [Fact]
        public void RenderPointWithGeometryType_ReturnsCorrectType()
        {
            var point = CreateBasicPoint();

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.NotNull(pointData.Geometry);
            Assert.Equal("BufferGeometry", pointData.Geometry.Type);
        }

        [Fact]
        public void RenderPointWithMaterialType_ReturnsCorrectType()
        {
            var point = CreateBasicPoint();

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.NotNull(pointData.Material);
            Assert.Equal("PointsMaterial", pointData.Material.Type);
        }

        [Fact]
        public void RenderPointWithEntityTypes_ReturnsCorrectTypes()
        {
            var point = CreateBasicPoint();

            var pointData = PointEntityRenderer.Render(point);

            Assert.NotNull(pointData);
            Assert.Equal("Point", pointData.Type);
            Assert.Equal("Point", pointData.EntityType);
        }
    }
}
