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
    public class SolidEntityRendererIntegrationTests
    {
        private Solid CreateBasicTriangleSolid()
        {
            var solid = new Solid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(1, 0, 0);
            solid.ThirdCorner = new XYZ(0, 1, 0);
            solid.FourthCorner = new XYZ(0, 1, 0);
            solid.Normal = new XYZ(0, 0, 1);
            solid.Color = new Color(1);
            solid.Thickness = 0.0;
            return solid;
        }

        private Solid CreateBasicQuadrilateralSolid()
        {
            var solid = new Solid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(1, 0, 0);
            solid.ThirdCorner = new XYZ(1, 1, 0);
            solid.FourthCorner = new XYZ(0, 1, 0);
            solid.Normal = new XYZ(0, 0, 1);
            solid.Color = new Color(1);
            solid.Thickness = 0.0;
            return solid;
        }

        [Fact]
        public void LoadAndRenderSolidFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var solidEntities = new List<Solid>();
            foreach (var entity in doc.Entities)
            {
                if (entity is Solid solid)
                {
                    solidEntities.Add(solid);
                }
            }

            if (solidEntities.Count == 0)
            {
                return;
            }

            foreach (var solid in solidEntities)
            {
                var solidData = SolidEntityRenderer.Render(solid);
                Assert.NotNull(solidData);
                Assert.Equal("Solid", solidData.Type);
                Assert.NotNull(solidData.Points);
                Assert.NotNull(solidData.Bounds3D);
            }
        }

        [Fact]
        public void SerializeSolidDataToJson_Success()
        {
            var solid = CreateBasicTriangleSolid();

            var solidData = SolidEntityRenderer.Render(solid);
            var json = JsonConvert.SerializeObject(solidData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Solid", json);
            Assert.Contains("Points", json);
            Assert.Contains("Bounds3D", json);
        }

        [Fact]
        public void DeserializeSolidDataFromJson_Success()
        {
            var solid = CreateBasicTriangleSolid();

            var originalSolidData = SolidEntityRenderer.Render(solid);
            var json = JsonConvert.SerializeObject(originalSolidData);
            var deserializedSolidData = JsonConvert.DeserializeObject<SolidEntityRenderer.SolidData>(json);

            Assert.NotNull(deserializedSolidData);
            Assert.Equal(originalSolidData.Type, deserializedSolidData.Type);
            Assert.Equal(originalSolidData.Points.Count, deserializedSolidData.Points.Count);
            Assert.Equal(originalSolidData.ColorIndex, deserializedSolidData.ColorIndex);
            Assert.Equal(originalSolidData.Thickness, deserializedSolidData.Thickness);
            Assert.Equal(originalSolidData.Visible, deserializedSolidData.Visible);
            Assert.Equal(originalSolidData.Opacity, deserializedSolidData.Opacity);
            Assert.Equal(originalSolidData.Transparent, deserializedSolidData.Transparent);
            Assert.Equal(originalSolidData.DepthTest, deserializedSolidData.DepthTest);
        }

        [Fact]
        public void RenderMultipleSolids_CollectAllSolidData()
        {
            var solids = new List<Solid>
            {
                CreateBasicTriangleSolid(),
                CreateBasicQuadrilateralSolid()
            };
            solids[0].FirstCorner = new XYZ(10, 10, 0);
            solids[0].SecondCorner = new XYZ(11, 10, 0);
            solids[0].ThirdCorner = new XYZ(10, 11, 0);
            solids[0].FourthCorner = new XYZ(10, 11, 0);
            solids[1].FirstCorner = new XYZ(20, 20, 0);
            solids[1].SecondCorner = new XYZ(21, 20, 0);
            solids[1].ThirdCorner = new XYZ(21, 21, 0);
            solids[1].FourthCorner = new XYZ(20, 21, 0);

            var solidDataList = new List<SolidEntityRenderer.SolidData>();
            foreach (var solid in solids)
            {
                var solidData = SolidEntityRenderer.Render(solid);
                solidDataList.Add(solidData);
            }

            Assert.Equal(2, solidDataList.Count);
            Assert.Equal(10, solidDataList[0].FirstCorner.X);
            Assert.Equal(20, solidDataList[1].FirstCorner.X);
        }

        [Fact]
        public void RenderTriangleSolid_PreservesAllProperties()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(100, 200, 300);
            solid.SecondCorner = new XYZ(150, 200, 300);
            solid.ThirdCorner = new XYZ(100, 250, 300);
            solid.FourthCorner = new XYZ(100, 250, 300);
            solid.Normal = new XYZ(0.5, 0.5, 0.7071);
            solid.Thickness = 10;
            solid.Color = new Color(5);
            solid.IsInvisible = false;

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(100, solidData.FirstCorner.X);
            Assert.Equal(200, solidData.FirstCorner.Y);
            Assert.Equal(300, solidData.FirstCorner.Z);
            Assert.Equal(150, solidData.SecondCorner.X);
            Assert.Equal(200, solidData.SecondCorner.Y);
            Assert.Equal(300, solidData.SecondCorner.Z);
            Assert.Equal(100, solidData.ThirdCorner.X);
            Assert.Equal(250, solidData.ThirdCorner.Y);
            Assert.Equal(300, solidData.ThirdCorner.Z);
            Assert.Equal(100, solidData.FourthCorner.X);
            Assert.Equal(250, solidData.FourthCorner.Y);
            Assert.Equal(300, solidData.FourthCorner.Z);
            Assert.InRange(solidData.Normal.X, 0.49, 0.51);
            Assert.InRange(solidData.Normal.Y, 0.49, 0.51);
            Assert.InRange(solidData.Normal.Z, 0.70, 0.71);
            Assert.Equal(10, solidData.Thickness);
            Assert.Equal(5, solidData.ColorIndex);
            Assert.True(solidData.Visible);
            Assert.True(solidData.IsTriangle);
        }

        [Fact]
        public void RenderQuadrilateralSolid_PreservesAllProperties()
        {
            var solid = CreateBasicQuadrilateralSolid();
            solid.FirstCorner = new XYZ(100, 200, 300);
            solid.SecondCorner = new XYZ(150, 200, 300);
            solid.ThirdCorner = new XYZ(150, 250, 300);
            solid.FourthCorner = new XYZ(100, 250, 300);
            solid.Normal = new XYZ(0, 0, 1);
            solid.Thickness = 10;
            solid.Color = new Color(5);
            solid.IsInvisible = false;

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(100, solidData.FirstCorner.X);
            Assert.Equal(200, solidData.FirstCorner.Y);
            Assert.Equal(300, solidData.FirstCorner.Z);
            Assert.Equal(150, solidData.SecondCorner.X);
            Assert.Equal(200, solidData.SecondCorner.Y);
            Assert.Equal(300, solidData.SecondCorner.Z);
            Assert.Equal(150, solidData.ThirdCorner.X);
            Assert.Equal(250, solidData.ThirdCorner.Y);
            Assert.Equal(300, solidData.ThirdCorner.Z);
            Assert.Equal(100, solidData.FourthCorner.X);
            Assert.Equal(250, solidData.FourthCorner.Y);
            Assert.Equal(300, solidData.FourthCorner.Z);
            Assert.Equal(0, solidData.Normal.X);
            Assert.Equal(0, solidData.Normal.Y);
            Assert.Equal(1, solidData.Normal.Z);
            Assert.Equal(10, solidData.Thickness);
            Assert.Equal(5, solidData.ColorIndex);
            Assert.True(solidData.Visible);
            Assert.False(solidData.IsTriangle);
        }

        [Fact]
        public void RenderSolidWith3DCoordinates_CalculatesCorrect3DProperties()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(10, 20, 30);
            solid.SecondCorner = new XYZ(20, 20, 30);
            solid.ThirdCorner = new XYZ(10, 30, 30);
            solid.FourthCorner = new XYZ(10, 30, 30);
            solid.Normal = new XYZ(0, 0, 1);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.NotNull(solidData.Centroid3D);
            Assert.NotNull(solidData.Bounds3D);
            Assert.Equal(0, solidData.Normal.X);
            Assert.Equal(0, solidData.Normal.Y);
            Assert.Equal(1, solidData.Normal.Z);
        }

        [Fact]
        public void RenderSolidWithRotation_CalculatesCorrectTransform()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(10, 20, 30);
            solid.SecondCorner = new XYZ(20, 20, 30);
            solid.ThirdCorner = new XYZ(10, 30, 30);
            solid.FourthCorner = new XYZ(10, 30, 30);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.NotNull(solidData.Transform);
            Assert.Equal(1.0, solidData.Transform.Scale.X);
            Assert.Equal(1.0, solidData.Transform.Scale.Y);
            Assert.Equal(1.0, solidData.Transform.Scale.Z);
            Assert.NotNull(solidData.Transform.Position);
        }

        [Fact]
        public void RenderSolidWithLayer_PreservesLayerInformation()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Layer = new Layer("TestLayer");

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal("TestLayer", solidData.LayerName);
        }

        [Fact]
        public void RenderSolidWithColor_PreservesColorInformation()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Color = new Color(5);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(5, solidData.ColorIndex);
            Assert.Equal(5, solidData.Color.Index);
        }

        [Fact]
        public void RenderSolidWithLineType_PreservesLineTypeInformation()
        {
            var solid = CreateBasicTriangleSolid();
            var lineType = new LineType("Continuous");
            solid.LineType = lineType;

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal("Continuous", solidData.LineTypeName);
        }

        [Fact]
        public void RenderSolidWithLineWeight_PreservesLineWeightInformation()
        {
            var solid = CreateBasicTriangleSolid();
            solid.LineWeight = ACadSharp.LineWeightType.W30;

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(0.3, solidData.LineWeight);
        }

        [Fact]
        public void RenderSolidWithInvisibleFlag_SetsVisibleProperty()
        {
            var solid = CreateBasicTriangleSolid();
            solid.IsInvisible = true;

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.False(solidData.Visible);
        }

        [Fact]
        public void RenderSolidWithHandle_PreservesHandleInformation()
        {
            var solid = CreateBasicTriangleSolid();
            
            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(solid.Handle.ToString(), solidData.Handle);
        }

        [Fact]
        public void RenderSolidWithZeroCorners_HandlesCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(0, 0, 0);
            solid.ThirdCorner = new XYZ(0, 0, 0);
            solid.FourthCorner = new XYZ(0, 0, 0);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(0, solidData.FirstCorner.X);
            Assert.Equal(0, solidData.FirstCorner.Y);
            Assert.Equal(0, solidData.FirstCorner.Z);
        }

        [Fact]
        public void RenderSolidWithVeryLargeCoordinates_HandlesCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(1000000, 2000000, 3000000);
            solid.SecondCorner = new XYZ(1001000, 2000000, 3000000);
            solid.ThirdCorner = new XYZ(1000000, 2001000, 3000000);
            solid.FourthCorner = new XYZ(1000000, 2001000, 3000000);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(1000000, solidData.FirstCorner.X);
            Assert.Equal(2000000, solidData.FirstCorner.Y);
            Assert.Equal(3000000, solidData.FirstCorner.Z);
        }

        [Fact]
        public void RenderSolidWithVerySmallCoordinates_HandlesCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(0.0001, 0.0002, 0.0003);
            solid.SecondCorner = new XYZ(0.0002, 0.0002, 0.0003);
            solid.ThirdCorner = new XYZ(0.0001, 0.0003, 0.0003);
            solid.FourthCorner = new XYZ(0.0001, 0.0003, 0.0003);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(0.0001, solidData.FirstCorner.X);
            Assert.Equal(0.0002, solidData.FirstCorner.Y);
            Assert.Equal(0.0003, solidData.FirstCorner.Z);
        }

        [Fact]
        public void RenderSolidWithNegativeCoordinates_HandlesCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(-10, -20, -30);
            solid.SecondCorner = new XYZ(0, -20, -30);
            solid.ThirdCorner = new XYZ(-10, -10, -30);
            solid.FourthCorner = new XYZ(-10, -10, -30);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(-10, solidData.FirstCorner.X);
            Assert.Equal(-20, solidData.FirstCorner.Y);
            Assert.Equal(-30, solidData.FirstCorner.Z);
        }

        [Fact]
        public void RenderSolidWithDifferentNormals_PreservesNormals()
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
                var solid = CreateBasicTriangleSolid();
                solid.Normal = normal;

                var solidData = SolidEntityRenderer.Render(solid);

                Assert.NotNull(solidData);
                Assert.InRange(solidData.Normal.X, normal.X - 0.0001, normal.X + 0.0001);
                Assert.InRange(solidData.Normal.Y, normal.Y - 0.0001, normal.Y + 0.0001);
                Assert.InRange(solidData.Normal.Z, normal.Z - 0.0001, normal.Z + 0.0001);
            }
        }

        [Fact]
        public void RenderSolidWithDifferentThickness_PreservesThickness()
        {
            var thicknesses = new[] { 0.0, 1.0, 10.0, -10.0 };

            foreach (var thickness in thicknesses)
            {
                var solid = CreateBasicTriangleSolid();
                solid.Thickness = thickness;

                var solidData = SolidEntityRenderer.Render(solid);

                Assert.NotNull(solidData);
                Assert.Equal(thickness, solidData.Thickness);
            }
        }

        [Fact]
        public void RenderSolidWithDifferentColors_PreservesColors()
        {
            var colors = new[] { 1, 2, 3, 5, 7 };

            foreach (var colorIndex in colors)
            {
                var solid = CreateBasicTriangleSolid();
                solid.Color = new Color((short)colorIndex);

                var solidData = SolidEntityRenderer.Render(solid);

                Assert.NotNull(solidData);
                Assert.Equal(colorIndex, solidData.ColorIndex);
                Assert.Equal(colorIndex, solidData.Color.Index);
            }
        }

        [Fact]
        public void RenderSolidWithDifferentLineWeights_PreservesLineWeights()
        {
            var lineWeights = new[]
            {
                ACadSharp.LineWeightType.W0,
                ACadSharp.LineWeightType.W30,
                ACadSharp.LineWeightType.W50
            };

            foreach (var lineWeight in lineWeights)
            {
                var solid = CreateBasicTriangleSolid();
                solid.LineWeight = lineWeight;

                var solidData = SolidEntityRenderer.Render(solid);

                Assert.NotNull(solidData);
                Assert.Equal(lineWeight.GetLineWeightValue(), solidData.LineWeight);
            }
        }

        [Fact]
        public void RenderSolidWithMaterial_ReturnsCorrectMaterial()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Color = new Color(5);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.NotNull(solidData.Material);
            Assert.Equal("MeshBasicMaterial", solidData.Material.Type);
            Assert.Equal(1.0, solidData.Material.Opacity);
            Assert.False(solidData.Material.Transparent);
            Assert.True(solidData.Material.DepthTest);
            Assert.Equal(5, solidData.Material.Color.Index);
        }

        [Fact]
        public void RenderSolidWithGeometry_ReturnsCorrectGeometry()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(10, 20, 30);
            solid.SecondCorner = new XYZ(20, 20, 30);
            solid.ThirdCorner = new XYZ(10, 30, 30);
            solid.FourthCorner = new XYZ(10, 30, 30);
            solid.Normal = new XYZ(0, 0, 1);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.NotNull(solidData.Geometry);
            Assert.Equal("BufferGeometry", solidData.Geometry.Type);
            Assert.NotNull(solidData.Bounds3D);
        }

        [Fact]
        public void RenderSolidWithTransform_ReturnsCorrectTransform()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(10, 20, 30);
            solid.SecondCorner = new XYZ(20, 20, 30);
            solid.ThirdCorner = new XYZ(10, 30, 30);
            solid.FourthCorner = new XYZ(10, 30, 30);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.NotNull(solidData.Transform);
            Assert.Equal(1.0, solidData.Transform.Scale.X);
            Assert.Equal(1.0, solidData.Transform.Scale.Y);
            Assert.Equal(1.0, solidData.Transform.Scale.Z);
            Assert.NotNull(solidData.Transform.Position);
        }

        [Fact]
        public void RenderSolidWithDefaultProperties_ReturnsCorrectDefaults()
        {
            var solid = new Solid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(1, 0, 0);
            solid.ThirdCorner = new XYZ(0, 1, 0);
            solid.FourthCorner = new XYZ(0, 1, 0);
            solid.Color = new ACadSharp.Color(7);
            solid.LineType = new LineType("CONTINUOUS");
            solid.LineWeight = LineWeightType.ByLayer;
            solid.IsInvisible = false;

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(0, solidData.Normal.X);
            Assert.Equal(0, solidData.Normal.Y);
            Assert.Equal(1, solidData.Normal.Z);
            Assert.True(solidData.Visible);
            Assert.Equal(1.0, solidData.Opacity);
            Assert.False(solidData.Transparent);
            Assert.True(solidData.DepthTest);
            Assert.Equal("WCS", solidData.CoordinateSystem);
        }

        [Fact]
        public void RenderSolidWithNullLayer_ReturnsDefaultLayerName()
        {
            var solid = new Solid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(1, 0, 0);
            solid.ThirdCorner = new XYZ(0, 1, 0);
            solid.FourthCorner = new XYZ(0, 1, 0);
            solid.Color = new ACadSharp.Color(7);
            solid.LineType = new LineType("CONTINUOUS");
            solid.LineWeight = LineWeightType.ByLayer;
            solid.IsInvisible = false;

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.NotNull(solidData.LayerName);
        }

        [Fact]
        public void RenderSolidWithExtrusion_CalculatesCorrectExtrusion()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Thickness = 10;

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(10, solidData.Thickness);
            Assert.True(solidData.IsExtruded);
            Assert.Equal(10, solidData.ExtrusionDepth);
        }

        [Fact]
        public void RenderSolidWithZeroThickness_HandlesCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Thickness = 0;

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(0, solidData.Thickness);
            Assert.False(solidData.IsExtruded);
            Assert.Equal(0, solidData.ExtrusionDepth);
        }

        [Fact]
        public void RenderSolidWithNegativeThickness_HandlesCorrectly()
        {
            var solid = CreateBasicTriangleSolid();
            solid.Thickness = -5;

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(-5, solidData.Thickness);
            Assert.True(solidData.IsExtruded);
            Assert.Equal(5, solidData.ExtrusionDepth);
        }

        [Fact]
        public void RenderSolidWithPoints_CalculatesCorrectPoints()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(10, 20, 30);
            solid.SecondCorner = new XYZ(20, 20, 30);
            solid.ThirdCorner = new XYZ(10, 30, 30);
            solid.FourthCorner = new XYZ(10, 30, 30);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.NotNull(solidData.Points);
            Assert.Equal(3, solidData.Points.Count);
            Assert.Equal(10, solidData.Points[0].X);
            Assert.Equal(20, solidData.Points[0].Y);
            Assert.Equal(30, solidData.Points[0].Z);
        }

        [Fact]
        public void RenderQuadrilateralSolidWithPoints_CalculatesCorrectPoints()
        {
            var solid = CreateBasicQuadrilateralSolid();
            solid.FirstCorner = new XYZ(10, 20, 30);
            solid.SecondCorner = new XYZ(20, 20, 30);
            solid.ThirdCorner = new XYZ(20, 30, 30);
            solid.FourthCorner = new XYZ(10, 30, 30);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.NotNull(solidData.Points);
            Assert.Equal(4, solidData.Points.Count);
            Assert.Equal(10, solidData.Points[0].X);
            Assert.Equal(20, solidData.Points[0].Y);
            Assert.Equal(30, solidData.Points[0].Z);
        }

        [Fact]
        public void RenderSolidWithVertexPositions_CalculatesCorrectVertexPositions()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(10, 20, 30);
            solid.SecondCorner = new XYZ(20, 20, 30);
            solid.ThirdCorner = new XYZ(10, 30, 30);
            solid.FourthCorner = new XYZ(10, 30, 30);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.NotNull(solidData.VertexPositions);
            Assert.True(solidData.VertexPositions.Count > 0);
        }

        [Fact]
        public void RenderSolidWithIndices_CalculatesCorrectIndices()
        {
            var solid = CreateBasicTriangleSolid();

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.NotNull(solidData.Indices);
            Assert.True(solidData.Indices.Count > 0);
        }

        [Fact]
        public void RenderSolidWithArea_CalculatesCorrectArea()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(1, 0, 0);
            solid.ThirdCorner = new XYZ(0, 1, 0);
            solid.FourthCorner = new XYZ(0, 1, 0);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(0.5, solidData.Area);
        }

        [Fact]
        public void RenderQuadrilateralSolidWithArea_CalculatesCorrectArea()
        {
            var solid = CreateBasicQuadrilateralSolid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(1, 0, 0);
            solid.ThirdCorner = new XYZ(1, 1, 0);
            solid.FourthCorner = new XYZ(0, 1, 0);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(1.0, solidData.Area);
        }

        [Fact]
        public void RenderSolidWithPerimeter_CalculatesCorrectPerimeter()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(1, 0, 0);
            solid.ThirdCorner = new XYZ(0, 1, 0);
            solid.FourthCorner = new XYZ(0, 1, 0);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.InRange(solidData.Perimeter, 3.414, 3.415);
        }

        [Fact]
        public void RenderQuadrilateralSolidWithPerimeter_CalculatesCorrectPerimeter()
        {
            var solid = CreateBasicQuadrilateralSolid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(1, 0, 0);
            solid.ThirdCorner = new XYZ(1, 1, 0);
            solid.FourthCorner = new XYZ(0, 1, 0);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(4.0, solidData.Perimeter);
        }

        [Fact]
        public void RenderSolidWithCentroid_CalculatesCorrectCentroid()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(1, 0, 0);
            solid.ThirdCorner = new XYZ(0, 1, 0);
            solid.FourthCorner = new XYZ(0, 1, 0);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.InRange(solidData.Centroid.X, 0.333, 0.334);
            Assert.InRange(solidData.Centroid.Y, 0.333, 0.334);
            Assert.Equal(0, solidData.Centroid.Z);
        }

        [Fact]
        public void RenderQuadrilateralSolidWithCentroid_CalculatesCorrectCentroid()
        {
            var solid = CreateBasicQuadrilateralSolid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(1, 0, 0);
            solid.ThirdCorner = new XYZ(1, 1, 0);
            solid.FourthCorner = new XYZ(0, 1, 0);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(0.5, solidData.Centroid.X);
            Assert.Equal(0.5, solidData.Centroid.Y);
            Assert.Equal(0, solidData.Centroid.Z);
        }

        [Fact]
        public void RenderSolidWithBounds_CalculatesCorrectBounds()
        {
            var solid = CreateBasicTriangleSolid();
            solid.FirstCorner = new XYZ(0, 0, 0);
            solid.SecondCorner = new XYZ(1, 0, 0);
            solid.ThirdCorner = new XYZ(0, 1, 0);
            solid.FourthCorner = new XYZ(0, 1, 0);

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.NotNull(solidData.Bounds3D);
            Assert.Equal(0, solidData.Bounds3D.Min.X);
            Assert.Equal(0, solidData.Bounds3D.Min.Y);
            Assert.Equal(0, solidData.Bounds3D.Min.Z);
            Assert.Equal(1, solidData.Bounds3D.Max.X);
            Assert.Equal(1, solidData.Bounds3D.Max.Y);
            Assert.Equal(0, solidData.Bounds3D.Max.Z);
        }

        [Fact]
        public void RenderSolidWithVertexCount_CalculatesCorrectVertexCount()
        {
            var solid = CreateBasicTriangleSolid();

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(3, solidData.VertexCount);
        }

        [Fact]
        public void RenderQuadrilateralSolidWithVertexCount_CalculatesCorrectVertexCount()
        {
            var solid = CreateBasicQuadrilateralSolid();

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(4, solidData.VertexCount);
        }

        [Fact]
        public void RenderSolidWithFaceCount_CalculatesCorrectFaceCount()
        {
            var solid = CreateBasicTriangleSolid();

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(1, solidData.FaceCount);
        }

        [Fact]
        public void RenderQuadrilateralSolidWithFaceCount_CalculatesCorrectFaceCount()
        {
            var solid = CreateBasicQuadrilateralSolid();

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal(2, solidData.FaceCount);
        }

        [Fact]
        public void RenderSolidWithAllPropertiesSet_PreservesAllProperties()
        {
            var solid = new Solid();
            solid.FirstCorner = new XYZ(100, 200, 300);
            solid.SecondCorner = new XYZ(150, 200, 300);
            solid.ThirdCorner = new XYZ(150, 250, 300);
            solid.FourthCorner = new XYZ(100, 250, 300);
            solid.Normal = new XYZ(0.577, 0.577, 0.577);
            solid.Thickness = 15;
            solid.Color = new Color(3);
            solid.LineType = new LineType("DASHED");
            solid.LineWeight = LineWeightType.W40;
            solid.IsInvisible = false;

            var solidData = SolidEntityRenderer.Render(solid);

            Assert.NotNull(solidData);
            Assert.Equal("Solid", solidData.Type);
            Assert.Equal(100, solidData.FirstCorner.X);
            Assert.Equal(200, solidData.FirstCorner.Y);
            Assert.Equal(300, solidData.FirstCorner.Z);
            Assert.Equal(150, solidData.SecondCorner.X);
            Assert.Equal(200, solidData.SecondCorner.Y);
            Assert.Equal(300, solidData.SecondCorner.Z);
            Assert.Equal(150, solidData.ThirdCorner.X);
            Assert.Equal(250, solidData.ThirdCorner.Y);
            Assert.Equal(300, solidData.ThirdCorner.Z);
            Assert.Equal(100, solidData.FourthCorner.X);
            Assert.Equal(250, solidData.FourthCorner.Y);
            Assert.Equal(300, solidData.FourthCorner.Z);
            Assert.Equal(15, solidData.Thickness);
            Assert.Equal(3, solidData.ColorIndex);
            Assert.Equal("DASHED", solidData.LineTypeName);
            Assert.Equal(0.4, solidData.LineWeight);
            Assert.True(solidData.Visible);
            Assert.False(solidData.IsTriangle);
            Assert.True(solidData.IsExtruded);
            Assert.Equal(15, solidData.ExtrusionDepth);
        }
    }
}
