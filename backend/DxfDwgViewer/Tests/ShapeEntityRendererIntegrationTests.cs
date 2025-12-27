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
    public class ShapeEntityRendererIntegrationTests
    {
        private Shape CreateBasicShape()
        {
            var textStyle = new TextStyle("ShapeStyle");
            textStyle.Flags = StyleFlags.IsShape;

            var shape = new Shape(textStyle);
            shape.InsertionPoint = new XYZ(0, 0, 0);
            shape.Size = 1.0;
            shape.Rotation = 0.0;
            shape.RelativeXScale = 1.0;
            shape.ObliqueAngle = 0.0;
            shape.Normal = new XYZ(0, 0, 1);
            shape.Color = new Color(1);
            shape.Thickness = 0.0;
            return shape;
        }

        [Fact]
        public void LoadAndRenderShapeFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var shapeEntities = new List<Shape>();
            foreach (var entity in doc.Entities)
            {
                if (entity is Shape shape)
                {
                    shapeEntities.Add(shape);
                }
            }

            if (shapeEntities.Count == 0)
            {
                return;
            }

            foreach (var shape in shapeEntities)
            {
                var shapeData = ShapeEntityRenderer.Render(shape);
                Assert.NotNull(shapeData);
                Assert.Equal("Shape", shapeData.Type);
                Assert.NotNull(shapeData.InsertionPoint);
                Assert.NotNull(shapeData.Bounds3D);
            }
        }

        [Fact]
        public void SerializeShapeDataToJson_Success()
        {
            var shape = CreateBasicShape();

            var shapeData = ShapeEntityRenderer.Render(shape);
            var json = JsonConvert.SerializeObject(shapeData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Shape", json);
            Assert.Contains("InsertionPoint", json);
            Assert.Contains("Bounds3D", json);
        }

        [Fact]
        public void DeserializeShapeDataFromJson_Success()
        {
            var shape = CreateBasicShape();

            var originalShapeData = ShapeEntityRenderer.Render(shape);
            var json = JsonConvert.SerializeObject(originalShapeData);
            var deserializedShapeData = JsonConvert.DeserializeObject<ShapeEntityRenderer.ShapeData>(json);

            Assert.NotNull(deserializedShapeData);
            Assert.Equal(originalShapeData.Type, deserializedShapeData.Type);
            Assert.Equal(originalShapeData.InsertionPoint.X, deserializedShapeData.InsertionPoint.X);
            Assert.Equal(originalShapeData.InsertionPoint.Y, deserializedShapeData.InsertionPoint.Y);
            Assert.Equal(originalShapeData.InsertionPoint.Z, deserializedShapeData.InsertionPoint.Z);
            Assert.Equal(originalShapeData.ColorIndex, deserializedShapeData.ColorIndex);
            Assert.Equal(originalShapeData.Size, deserializedShapeData.Size);
            Assert.Equal(originalShapeData.Rotation, deserializedShapeData.Rotation);
            Assert.Equal(originalShapeData.RelativeXScale, deserializedShapeData.RelativeXScale);
            Assert.Equal(originalShapeData.Thickness, deserializedShapeData.Thickness);
            Assert.Equal(originalShapeData.Visible, deserializedShapeData.Visible);
            Assert.Equal(originalShapeData.Opacity, deserializedShapeData.Opacity);
            Assert.Equal(originalShapeData.Transparent, deserializedShapeData.Transparent);
            Assert.Equal(originalShapeData.DepthTest, deserializedShapeData.DepthTest);
        }

        [Fact]
        public void RenderMultipleShapes_CollectAllShapeData()
        {
            var shapes = new List<Shape>
            {
                CreateBasicShape(),
                CreateBasicShape()
            };
            shapes[0].InsertionPoint = new XYZ(10, 10, 0);
            shapes[1].InsertionPoint = new XYZ(20, 20, 0);

            var shapeDataList = new List<ShapeEntityRenderer.ShapeData>();
            foreach (var shape in shapes)
            {
                var shapeData = ShapeEntityRenderer.Render(shape);
                shapeDataList.Add(shapeData);
            }

            Assert.Equal(2, shapeDataList.Count);
            Assert.Equal(10, shapeDataList[0].InsertionPoint.X);
            Assert.Equal(20, shapeDataList[1].InsertionPoint.X);
        }

        [Fact]
        public void RenderShapeWithComplexProperties_PreservesAllProperties()
        {
            var shape = CreateBasicShape();
            shape.InsertionPoint = new XYZ(100, 200, 300);
            shape.Size = 2.5;
            shape.Rotation = Math.PI / 4;
            shape.RelativeXScale = 1.5;
            shape.ObliqueAngle = Math.PI / 6;
            shape.Normal = new XYZ(0.5, 0.5, 0.7071);
            shape.Thickness = 10;
            shape.Color = new Color(5);
            shape.IsInvisible = false;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(100, shapeData.InsertionPoint.X);
            Assert.Equal(200, shapeData.InsertionPoint.Y);
            Assert.Equal(300, shapeData.InsertionPoint.Z);
            Assert.Equal(2.5, shapeData.Size);
            Assert.Equal(Math.PI / 4, shapeData.Rotation);
            Assert.Equal(1.5, shapeData.RelativeXScale);
            Assert.Equal(Math.PI / 6, shapeData.ObliqueAngle);
            Assert.InRange(shapeData.Normal.X, 0.49, 0.51);
            Assert.InRange(shapeData.Normal.Y, 0.49, 0.51);
            Assert.InRange(shapeData.Normal.Z, 0.70, 0.71);
            Assert.Equal(10, shapeData.Thickness);
            Assert.Equal(5, shapeData.ColorIndex);
            Assert.True(shapeData.Visible);
        }

        [Fact]
        public void RenderShapeWith3DCoordinates_CalculatesCorrect3DProperties()
        {
            var shape = CreateBasicShape();
            shape.InsertionPoint = new XYZ(10, 20, 30);
            shape.Normal = new XYZ(0, 0, 1);

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(10, shapeData.InsertionPoint.X);
            Assert.Equal(20, shapeData.InsertionPoint.Y);
            Assert.Equal(30, shapeData.InsertionPoint.Z);
            Assert.Equal(10, shapeData.Centroid3D.X);
            Assert.Equal(20, shapeData.Centroid3D.Y);
            Assert.Equal(30, shapeData.Centroid3D.Z);
            Assert.Equal(0, shapeData.Normal.X);
            Assert.Equal(0, shapeData.Normal.Y);
            Assert.Equal(1, shapeData.Normal.Z);
        }

        [Fact]
        public void RenderShapeWithRotation_CalculatesCorrectTransform()
        {
            var shape = CreateBasicShape();
            shape.InsertionPoint = new XYZ(10, 20, 30);
            shape.Rotation = Math.PI / 2;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.NotNull(shapeData.Transform);
            Assert.Equal(1.0, shapeData.Transform.Scale.X);
            Assert.Equal(1.0, shapeData.Transform.Scale.Y);
            Assert.Equal(1.0, shapeData.Transform.Scale.Z);
            Assert.Equal(Math.PI / 2, shapeData.Transform.Rotation.Z);
            Assert.Equal(10, shapeData.Transform.Position.X);
            Assert.Equal(20, shapeData.Transform.Position.Y);
            Assert.Equal(30, shapeData.Transform.Position.Z);
        }

        [Fact]
        public void RenderShapeWithDifferentRotations_CalculatesCorrectTransform()
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
                var shape = CreateBasicShape();
                shape.Rotation = rotation;

                var shapeData = ShapeEntityRenderer.Render(shape);

                Assert.NotNull(shapeData);
                Assert.NotNull(shapeData.Transform);
                Assert.Equal(rotation, shapeData.Transform.Rotation.Z);
            }
        }

        [Fact]
        public void RenderShapeWithDifferentSizes_PreservesSizes()
        {
            var sizes = new[] { 0.5, 1.0, 2.0, 5.0 };

            foreach (var size in sizes)
            {
                var shape = CreateBasicShape();
                shape.Size = size;

                var shapeData = ShapeEntityRenderer.Render(shape);

                Assert.NotNull(shapeData);
                Assert.Equal(size, shapeData.Size);
            }
        }

        [Fact]
        public void RenderShapeWithDifferentScales_PreservesScales()
        {
            var scales = new[] { 0.5, 1.0, 1.5, 2.0 };

            foreach (var scale in scales)
            {
                var shape = CreateBasicShape();
                shape.RelativeXScale = scale;

                var shapeData = ShapeEntityRenderer.Render(shape);

                Assert.NotNull(shapeData);
                Assert.Equal(scale, shapeData.RelativeXScale);
            }
        }

        [Fact]
        public void RenderShapeWithLayer_PreservesLayerInformation()
        {
            var shape = CreateBasicShape();
            shape.Layer = new Layer("TestLayer");

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal("TestLayer", shapeData.LayerName);
        }

        [Fact]
        public void RenderShapeWithColor_PreservesColorInformation()
        {
            var shape = CreateBasicShape();
            shape.Color = new Color(5);

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(5, shapeData.ColorIndex);
            Assert.Equal(5, shapeData.Color.Index);
        }

        [Fact]
        public void RenderShapeWithLineType_PreservesLineTypeInformation()
        {
            var shape = CreateBasicShape();
            var lineType = new LineType("Continuous");
            shape.LineType = lineType;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal("Continuous", shapeData.LineTypeName);
        }

        [Fact]
        public void RenderShapeWithLineWeight_PreservesLineWeightInformation()
        {
            var shape = CreateBasicShape();
            shape.LineWeight = ACadSharp.LineWeightType.W30;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(0.3, shapeData.LineWeight);
        }

        [Fact]
        public void RenderShapeWithInvisibleFlag_SetsVisibleProperty()
        {
            var shape = CreateBasicShape();
            shape.IsInvisible = true;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.False(shapeData.Visible);
        }

        [Fact]
        public void RenderShapeWithHandle_PreservesHandleInformation()
        {
            var shape = CreateBasicShape();
            
            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(shape.Handle.ToString(), shapeData.Handle);
        }

        [Fact]
        public void RenderShapeWithZeroInsertionPoint_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.InsertionPoint = new XYZ(0, 0, 0);

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(0, shapeData.InsertionPoint.X);
            Assert.Equal(0, shapeData.InsertionPoint.Y);
            Assert.Equal(0, shapeData.InsertionPoint.Z);
        }

        [Fact]
        public void RenderShapeWithVeryLargeCoordinates_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.InsertionPoint = new XYZ(1000000, 2000000, 3000000);

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(1000000, shapeData.InsertionPoint.X);
            Assert.Equal(2000000, shapeData.InsertionPoint.Y);
            Assert.Equal(3000000, shapeData.InsertionPoint.Z);
        }

        [Fact]
        public void RenderShapeWithVerySmallCoordinates_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.InsertionPoint = new XYZ(0.0001, 0.0002, 0.0003);

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(0.0001, shapeData.InsertionPoint.X);
            Assert.Equal(0.0002, shapeData.InsertionPoint.Y);
            Assert.Equal(0.0003, shapeData.InsertionPoint.Z);
        }

        [Fact]
        public void RenderShapeWithNegativeCoordinates_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.InsertionPoint = new XYZ(-10, -20, -30);

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(-10, shapeData.InsertionPoint.X);
            Assert.Equal(-20, shapeData.InsertionPoint.Y);
            Assert.Equal(-30, shapeData.InsertionPoint.Z);
        }

        [Fact]
        public void RenderShapeWithZeroRotation_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Rotation = 0;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(0, shapeData.Rotation);
        }

        [Fact]
        public void RenderShapeWithFullRotation_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Rotation = 2 * Math.PI;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.InRange(shapeData.Rotation, 2 * Math.PI - 0.0001, 2 * Math.PI + 0.0001);
        }

        [Fact]
        public void RenderShapeWithDifferentNormals_PreservesNormals()
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
                var shape = CreateBasicShape();
                shape.Normal = normal;

                var shapeData = ShapeEntityRenderer.Render(shape);

                Assert.NotNull(shapeData);
                Assert.InRange(shapeData.Normal.X, normal.X - 0.0001, normal.X + 0.0001);
                Assert.InRange(shapeData.Normal.Y, normal.Y - 0.0001, normal.Y + 0.0001);
                Assert.InRange(shapeData.Normal.Z, normal.Z - 0.0001, normal.Z + 0.0001);
            }
        }

        [Fact]
        public void RenderShapeWithDifferentThickness_PreservesThickness()
        {
            var thicknesses = new[] { 0.0, 1.0, 10.0, -10.0 };

            foreach (var thickness in thicknesses)
            {
                var shape = CreateBasicShape();
                shape.Thickness = thickness;

                var shapeData = ShapeEntityRenderer.Render(shape);

                Assert.NotNull(shapeData);
                Assert.Equal(thickness, shapeData.Thickness);
            }
        }

        [Fact]
        public void RenderShapeWithDifferentColors_PreservesColors()
        {
            var colors = new[] { 1, 2, 3, 5, 7 };

            foreach (var colorIndex in colors)
            {
                var shape = CreateBasicShape();
                shape.Color = new Color((short)colorIndex);

                var shapeData = ShapeEntityRenderer.Render(shape);

                Assert.NotNull(shapeData);
                Assert.Equal(colorIndex, shapeData.ColorIndex);
                Assert.Equal(colorIndex, shapeData.Color.Index);
            }
        }

        [Fact]
        public void RenderShapeWithDifferentLineWeights_PreservesLineWeights()
        {
            var lineWeights = new[]
            {
                ACadSharp.LineWeightType.W0,
                ACadSharp.LineWeightType.W30,
                ACadSharp.LineWeightType.W50
            };

            foreach (var lineWeight in lineWeights)
            {
                var shape = CreateBasicShape();
                shape.LineWeight = lineWeight;

                var shapeData = ShapeEntityRenderer.Render(shape);

                Assert.NotNull(shapeData);
                Assert.Equal(lineWeight.GetLineWeightValue(), shapeData.LineWeight);
            }
        }

        [Fact]
        public void RenderShapeWithMaterial_ReturnsCorrectMaterial()
        {
            var shape = CreateBasicShape();
            shape.Color = new Color(5);

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.NotNull(shapeData.Material);
            Assert.Equal("LineBasicMaterial", shapeData.Material.Type);
            Assert.Equal(1.0, shapeData.Material.Opacity);
            Assert.False(shapeData.Material.Transparent);
            Assert.True(shapeData.Material.DepthTest);
            Assert.Equal(5, shapeData.Material.Color.Index);
        }

        [Fact]
        public void RenderShapeWithGeometry_ReturnsCorrectGeometry()
        {
            var shape = CreateBasicShape();
            shape.InsertionPoint = new XYZ(10, 20, 30);
            shape.Rotation = Math.PI / 4;
            shape.Normal = new XYZ(0, 0, 1);

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.NotNull(shapeData.Geometry);
            Assert.Equal("BufferGeometry", shapeData.Geometry.Type);
            Assert.Equal(10, shapeData.Geometry.Position.X);
            Assert.Equal(20, shapeData.Geometry.Position.Y);
            Assert.Equal(30, shapeData.Geometry.Position.Z);
            Assert.Equal(0, shapeData.Geometry.Rotation.X);
            Assert.Equal(0, shapeData.Geometry.Rotation.Y);
            Assert.Equal(Math.PI / 4, shapeData.Geometry.Rotation.Z);
            Assert.Equal(1.0, shapeData.Geometry.Scale.X);
            Assert.Equal(1.0, shapeData.Geometry.Scale.Y);
            Assert.Equal(1.0, shapeData.Geometry.Scale.Z);
            Assert.Equal(0, shapeData.Geometry.Normal.X);
            Assert.Equal(0, shapeData.Geometry.Normal.Y);
            Assert.Equal(1, shapeData.Geometry.Normal.Z);
        }

        [Fact]
        public void RenderShapeWithTransform_ReturnsCorrectTransform()
        {
            var shape = CreateBasicShape();
            shape.InsertionPoint = new XYZ(10, 20, 30);
            shape.Rotation = Math.PI / 3;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.NotNull(shapeData.Transform);
            Assert.Equal(1.0, shapeData.Transform.Scale.X);
            Assert.Equal(1.0, shapeData.Transform.Scale.Y);
            Assert.Equal(1.0, shapeData.Transform.Scale.Z);
            Assert.Equal(Math.PI / 3, shapeData.Transform.Rotation.Z);
            Assert.Equal(10, shapeData.Transform.Position.X);
            Assert.Equal(20, shapeData.Transform.Position.Y);
            Assert.Equal(30, shapeData.Transform.Position.Z);
        }

        [Fact]
        public void RenderShapeWithDefaultProperties_ReturnsCorrectDefaults()
        {
            var textStyle = new TextStyle("ShapeStyle");
            textStyle.Flags = StyleFlags.IsShape;

            var shape = new Shape(textStyle);
            shape.InsertionPoint = new XYZ(10, 10, 0);
            shape.Color = new ACadSharp.Color(7);
            shape.LineType = new LineType("CONTINUOUS");
            shape.LineWeight = LineWeightType.ByLayer;
            shape.IsInvisible = false;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(0, shapeData.Rotation);
            Assert.Equal(0, shapeData.Thickness);
            Assert.Equal(0, shapeData.Normal.X);
            Assert.Equal(0, shapeData.Normal.Y);
            Assert.Equal(1, shapeData.Normal.Z);
            Assert.True(shapeData.Visible);
            Assert.Equal(1.0, shapeData.Opacity);
            Assert.False(shapeData.Transparent);
            Assert.True(shapeData.DepthTest);
            Assert.Equal("WCS", shapeData.CoordinateSystem);
        }

        [Fact]
        public void RenderShapeWithNullLayer_ReturnsDefaultLayerName()
        {
            var textStyle = new TextStyle("ShapeStyle");
            textStyle.Flags = StyleFlags.IsShape;

            var shape = new Shape(textStyle);
            shape.InsertionPoint = new XYZ(10, 10, 0);
            shape.Color = new ACadSharp.Color(7);
            shape.LineType = new LineType("CONTINUOUS");
            shape.LineWeight = LineWeightType.ByLayer;
            shape.IsInvisible = false;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.NotNull(shapeData.LayerName);
        }

        [Fact]
        public void RenderShapeWithObliqueAngle_PreservesObliqueAngle()
        {
            var shape = CreateBasicShape();
            shape.ObliqueAngle = Math.PI / 6;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(Math.PI / 6, shapeData.ObliqueAngle);
        }

        [Fact]
        public void RenderShapeWithZeroSize_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Size = 0;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(0, shapeData.Size);
        }

        [Fact]
        public void RenderShapeWithVeryLargeSize_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.Size = 1000;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(1000, shapeData.Size);
        }

        [Fact]
        public void RenderShapeWithZeroScale_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.RelativeXScale = 0;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(0, shapeData.RelativeXScale);
        }

        [Fact]
        public void RenderShapeWithVeryLargeScale_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.RelativeXScale = 10;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(10, shapeData.RelativeXScale);
        }

        [Fact]
        public void RenderShapeWithNegativeScale_HandlesCorrectly()
        {
            var shape = CreateBasicShape();
            shape.RelativeXScale = -1.5;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal(-1.5, shapeData.RelativeXScale);
        }

        [Fact]
        public void RenderShapeWithTextStyle_PreservesTextStyleName()
        {
            var textStyle = new TextStyle("MyShapeStyle");
            textStyle.Flags = StyleFlags.IsShape;

            var shape = new Shape(textStyle);
            shape.InsertionPoint = new XYZ(10, 10, 0);
            shape.Color = new ACadSharp.Color(7);
            shape.LineType = new LineType("CONTINUOUS");
            shape.LineWeight = LineWeightType.ByLayer;
            shape.IsInvisible = false;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal("MyShapeStyle", shapeData.TextStyleName);
        }

        [Fact]
        public void RenderShapeWithBoundaryPoints_CalculatesCorrectBounds()
        {
            var shape = CreateBasicShape();
            shape.InsertionPoint = new XYZ(10, 10, 0);
            shape.Size = 2.0;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.NotNull(shapeData.Bounds3D);
            Assert.NotNull(shapeData.Bounds3D.Min);
            Assert.NotNull(shapeData.Bounds3D.Max);
            Assert.NotNull(shapeData.Bounds3D.Center);
            Assert.NotNull(shapeData.Bounds3D.Size);
        }

        [Fact]
        public void RenderShapeWithDifferentObliqueAngles_PreservesObliqueAngles()
        {
            var obliqueAngles = new[]
            {
                0.0,
                Math.PI / 12,
                Math.PI / 6,
                Math.PI / 4
            };

            foreach (var obliqueAngle in obliqueAngles)
            {
                var shape = CreateBasicShape();
                shape.ObliqueAngle = obliqueAngle;

                var shapeData = ShapeEntityRenderer.Render(shape);

                Assert.NotNull(shapeData);
                Assert.Equal(obliqueAngle, shapeData.ObliqueAngle);
            }
        }

        [Fact]
        public void RenderShapeWithAllPropertiesSet_PreservesAllProperties()
        {
            var textStyle = new TextStyle("CompleteShapeStyle");
            textStyle.Flags = StyleFlags.IsShape;

            var shape = new Shape(textStyle);
            shape.InsertionPoint = new XYZ(100, 200, 300);
            shape.Size = 3.5;
            shape.Rotation = Math.PI / 3;
            shape.RelativeXScale = 2.0;
            shape.ObliqueAngle = Math.PI / 8;
            shape.Normal = new XYZ(0.577, 0.577, 0.577);
            shape.Thickness = 15;
            shape.Color = new Color(3);
            shape.LineType = new LineType("DASHED");
            shape.LineWeight = LineWeightType.W40;
            shape.IsInvisible = false;

            var shapeData = ShapeEntityRenderer.Render(shape);

            Assert.NotNull(shapeData);
            Assert.Equal("Shape", shapeData.Type);
            Assert.Equal(100, shapeData.InsertionPoint.X);
            Assert.Equal(200, shapeData.InsertionPoint.Y);
            Assert.Equal(300, shapeData.InsertionPoint.Z);
            Assert.Equal(3.5, shapeData.Size);
            Assert.Equal(Math.PI / 3, shapeData.Rotation);
            Assert.Equal(2.0, shapeData.RelativeXScale);
            Assert.Equal(Math.PI / 8, shapeData.ObliqueAngle);
            Assert.Equal(15, shapeData.Thickness);
            Assert.Equal(3, shapeData.ColorIndex);
            Assert.Equal("DASHED", shapeData.LineTypeName);
            Assert.Equal(0.4, shapeData.LineWeight);
            Assert.True(shapeData.Visible);
            Assert.Equal("CompleteShapeStyle", shapeData.TextStyleName);
        }
    }
}
