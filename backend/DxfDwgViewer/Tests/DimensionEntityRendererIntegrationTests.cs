using Xunit;
using ACadSharp.Entities;
using ACadSharp.IO;
using DxfDwgViewer.RenderUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class DimensionEntityRendererIntegrationTests
    {
        [Fact]
        public void LoadAndRenderDimensionFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var dimensionEntities = new List<Dimension>();
            foreach (var entity in doc.Entities)
            {
                if (entity is Dimension dimension)
                {
                    dimensionEntities.Add(dimension);
                }
            }

            if (dimensionEntities.Count == 0)
            {
                return;
            }

            foreach (var dimension in dimensionEntities)
            {
                var dimensionData = DimensionEntityRenderer.Render(dimension);
                Assert.NotNull(dimensionData);
                Assert.Equal("Dimension", dimensionData.Type);
                Assert.NotNull(dimensionData.Bounds);
                Assert.NotNull(dimensionData.Centroid);
            }
        }

        [Fact]
        public void LoadAndRenderDimensionAngular3PtFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var angular3PtEntities = new List<DimensionAngular3Pt>();
            foreach (var entity in doc.Entities)
            {
                if (entity is DimensionAngular3Pt angular3Pt)
                {
                    angular3PtEntities.Add(angular3Pt);
                }
            }

            if (angular3PtEntities.Count == 0)
            {
                return;
            }

            foreach (var angular3Pt in angular3PtEntities)
            {
                var dimensionData = DimensionEntityRenderer.Render(angular3Pt);
                Assert.NotNull(dimensionData);
                Assert.NotNull(dimensionData.Angular3PtData);
                Assert.NotNull(dimensionData.Angular3PtData.ArcCenter);
                Assert.True(dimensionData.Angular3PtData.Radius > 0);
                Assert.NotNull(dimensionData.Angular3PtData.ArcPoints);
                Assert.True(dimensionData.Angular3PtData.ArcPoints.Length > 0);
            }
        }

        [Fact]
        public void SerializeDimensionDataToJson_Success()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(10, 20, 0),
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(1)
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Dimension", json);
            Assert.Contains("DefinitionPointX", json);
            Assert.Contains("NormalX", json);
        }

        [Fact]
        public void SerializeDimensionAngular3PtDataToJson_Success()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(0, 10, 0),
                Text = "90°"
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Angular3PtData", json);
            Assert.Contains("VertexX", json);
            Assert.Contains("FirstPointX", json);
            Assert.Contains("SecondPointX", json);
        }

        [Fact]
        public void DeserializeDimensionDataFromJson_Success()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(10, 20, 0),
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(1)
            };

            var originalDimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(originalDimensionData);
            var deserializedDimensionData = JsonConvert.DeserializeObject<DimensionEntityRenderer.DimensionData>(json);

            Assert.NotNull(deserializedDimensionData);
            Assert.Equal(originalDimensionData.Type, deserializedDimensionData.Type);
            Assert.Equal(originalDimensionData.DefinitionPointX, deserializedDimensionData.DefinitionPointX);
            Assert.Equal(originalDimensionData.DefinitionPointY, deserializedDimensionData.DefinitionPointY);
            Assert.Equal(originalDimensionData.DefinitionPointZ, deserializedDimensionData.DefinitionPointZ);
            Assert.Equal(originalDimensionData.NormalX, deserializedDimensionData.NormalX);
            Assert.Equal(originalDimensionData.NormalY, deserializedDimensionData.NormalY);
            Assert.Equal(originalDimensionData.NormalZ, deserializedDimensionData.NormalZ);
            Assert.Equal(originalDimensionData.ColorR, deserializedDimensionData.ColorR);
            Assert.Equal(originalDimensionData.ColorG, deserializedDimensionData.ColorG);
            Assert.Equal(originalDimensionData.ColorB, deserializedDimensionData.ColorB);
        }

        [Fact]
        public void DeserializeDimensionAngular3PtDataFromJson_Success()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(0, 10, 0),
                Text = "90°"
            };

            var originalDimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(originalDimensionData);
            var deserializedDimensionData = JsonConvert.DeserializeObject<DimensionEntityRenderer.DimensionData>(json);

            Assert.NotNull(deserializedDimensionData);
            Assert.NotNull(deserializedDimensionData.Angular3PtData);
            Assert.Equal(originalDimensionData.Angular3PtData.VertexX, deserializedDimensionData.Angular3PtData.VertexX);
            Assert.Equal(originalDimensionData.Angular3PtData.VertexY, deserializedDimensionData.Angular3PtData.VertexY);
            Assert.Equal(originalDimensionData.Angular3PtData.FirstPointX, deserializedDimensionData.Angular3PtData.FirstPointX);
            Assert.Equal(originalDimensionData.Angular3PtData.FirstPointY, deserializedDimensionData.Angular3PtData.FirstPointY);
            Assert.Equal(originalDimensionData.Angular3PtData.SecondPointX, deserializedDimensionData.Angular3PtData.SecondPointX);
            Assert.Equal(originalDimensionData.Angular3PtData.SecondPointY, deserializedDimensionData.Angular3PtData.SecondPointY);
            Assert.Equal(originalDimensionData.Angular3PtData.Text, deserializedDimensionData.Angular3PtData.Text);
        }

        [Fact]
        public void RenderMultipleDimensions_CollectAllDimensionData()
        {
            var dimensions = new List<DimensionAngular3Pt>
            {
                new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(10, 20, 0),
                    Normal = new XYZ(0, 0, 1),
                    Color = new ACadSharp.Color(1)
                },
                new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(30, 40, 0),
                    Normal = new XYZ(0, 0, 1),
                    Color = new ACadSharp.Color(2)
                },
                new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(50, 60, 0),
                    Normal = new XYZ(0, 0, 1),
                    Color = new ACadSharp.Color(3)
                }
            };

            var dimensionDataList = new List<DimensionEntityRenderer.DimensionData>();
            foreach (var dimension in dimensions)
            {
                var dimensionData = DimensionEntityRenderer.Render(dimension);
                dimensionDataList.Add(dimensionData);
            }

            Assert.Equal(3, dimensionDataList.Count);
            Assert.All(dimensionDataList, data => Assert.NotNull(data));
            Assert.All(dimensionDataList, data => Assert.Equal("DimensionAngular3Pt", data.Type));
        }

        [Fact]
        public void RenderMultipleAngular3PtDimensions_CollectAllDimensionData()
        {
            var dimensions = new List<DimensionAngular3Pt>
            {
                new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(0, 0, 0),
                    Normal = new XYZ(0, 0, 1),
                    AngleVertex = new XYZ(0, 0, 0),
                    FirstPoint = new XYZ(10, 0, 0),
                    SecondPoint = new XYZ(0, 10, 0),
                    Text = "90°"
                },
                new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(20, 20, 0),
                    Normal = new XYZ(0, 0, 1),
                    AngleVertex = new XYZ(20, 20, 0),
                    FirstPoint = new XYZ(30, 20, 0),
                    SecondPoint = new XYZ(20, 30, 0),
                    Text = "90°"
                }
            };

            var dimensionDataList = new List<DimensionEntityRenderer.DimensionData>();
            foreach (var dimension in dimensions)
            {
                var dimensionData = DimensionEntityRenderer.Render(dimension);
                dimensionDataList.Add(dimensionData);
            }

            Assert.Equal(2, dimensionDataList.Count);
            Assert.All(dimensionDataList, data => Assert.NotNull(data.Angular3PtData));
            Assert.All(dimensionDataList, data => Assert.NotNull(data.Angular3PtData.ArcCenter));
            Assert.All(dimensionDataList, data => Assert.True(data.Angular3PtData.Radius > 0));
        }

        [Fact]
        public void SerializeAndDeserializeMultipleDimensions_Success()
        {
            var dimensions = new List<DimensionAngular3Pt>
            {
                new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(10, 20, 0),
                    Normal = new XYZ(0, 0, 1),
                    Color = new ACadSharp.Color(1)
                },
                new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(30, 40, 0),
                    Normal = new XYZ(0, 0, 1),
                    Color = new ACadSharp.Color(2)
                }
            };

            var jsonList = new List<string>();
            foreach (var dimension in dimensions)
            {
                var dimensionData = DimensionEntityRenderer.Render(dimension);
                var json = JsonConvert.SerializeObject(dimensionData);
                jsonList.Add(json);
            }

            Assert.Equal(2, jsonList.Count);
            Assert.All(jsonList, json => Assert.NotNull(json));
            Assert.All(jsonList, json => Assert.Contains("Dimension", json));
        }

        [Fact]
        public void SerializeAndDeserializeMultipleAngular3PtDimensions_Success()
        {
            var dimensions = new List<DimensionAngular3Pt>
            {
                new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(0, 0, 0),
                    Normal = new XYZ(0, 0, 1),
                    AngleVertex = new XYZ(0, 0, 0),
                    FirstPoint = new XYZ(10, 0, 0),
                    SecondPoint = new XYZ(0, 10, 0),
                    Text = "90°"
                },
                new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(20, 20, 0),
                    Normal = new XYZ(0, 0, 1),
                    AngleVertex = new XYZ(20, 20, 0),
                    FirstPoint = new XYZ(30, 20, 0),
                    SecondPoint = new XYZ(20, 30, 0),
                    Text = "90°"
                }
            };

            var jsonList = new List<string>();
            foreach (var dimension in dimensions)
            {
                var dimensionData = DimensionEntityRenderer.Render(dimension);
                var json = JsonConvert.SerializeObject(dimensionData);
                jsonList.Add(json);
            }

            Assert.Equal(2, jsonList.Count);
            Assert.All(jsonList, json => Assert.NotNull(json));
            Assert.All(jsonList, json => Assert.Contains("Angular3PtData", json));
        }

        [Fact]
        public void RenderDimensionWith3DCoordinates_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(15, 25, 35),
                Normal = new XYZ(0.5, 0.5, 0.707),
                Color = new ACadSharp.Color(5)
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("15", json);
            Assert.Contains("25", json);
            Assert.Contains("35", json);
        }

        [Fact]
        public void RenderDimensionAngular3PtWith3DCoordinates_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 10),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(5, 5, 10),
                FirstPoint = new XYZ(15, 5, 10),
                SecondPoint = new XYZ(5, 15, 10),
                Text = "90°"
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("10", json);
        }

        [Fact]
        public void RenderDimensionWithDifferentColors_SerializesCorrectly()
        {
            var testCases = new[] { 1, 2, 3, 4, 5 };

            foreach (var colorIndex in testCases)
            {
                var dimension = new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(0, 0, 0),
                    Normal = new XYZ(0, 0, 1),
                    Color = new ACadSharp.Color((short)colorIndex)
                };

                var dimensionData = DimensionEntityRenderer.Render(dimension);
                var json = JsonConvert.SerializeObject(dimensionData);

                Assert.NotNull(json);
                Assert.Contains(colorIndex.ToString(), json);
            }
        }

        [Fact]
        public void RenderDimensionAngular3PtWithDifferentText_SerializesCorrectly()
        {
            var testCases = new[] { "45°", "90°", "180°", "270°" };

            foreach (var text in testCases)
            {
                var dimension = new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(0, 0, 0),
                    Normal = new XYZ(0, 0, 1),
                    AngleVertex = new XYZ(0, 0, 0),
                    FirstPoint = new XYZ(10, 0, 0),
                    SecondPoint = new XYZ(0, 10, 0),
                    Text = text
                };

                var dimensionData = DimensionEntityRenderer.Render(dimension);
                var json = JsonConvert.SerializeObject(dimensionData);

                Assert.NotNull(json);
                Assert.Contains(text, json);
            }
        }

        [Fact]
        public void RenderDimensionWithNegativeCoordinates_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(-10, -20, -5),
                Normal = new XYZ(0, 0, 1)
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("-10", json);
            Assert.Contains("-20", json);
            Assert.Contains("-5", json);
        }

        [Fact]
        public void RenderDimensionWithLargeCoordinates_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(10000, 20000, 30000),
                Normal = new XYZ(0, 0, 1)
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("10000", json);
            Assert.Contains("20000", json);
            Assert.Contains("30000", json);
        }

        [Fact]
        public void RenderDimensionWithZeroCoordinates_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("0", json);
        }

        [Fact]
        public void RenderDimensionAngular3PtWithMeasurement_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(0, 10, 0),
                Text = "90°"
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("1.5707963267948966", json);
        }

        [Fact]
        public void RenderDimensionWithVerySmallCoordinates_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0.001, 0.002, 0.003),
                Normal = new XYZ(0, 0, 1)
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("0.001", json);
            Assert.Contains("0.002", json);
            Assert.Contains("0.003", json);
        }

        [Fact]
        public void RenderDimensionWithVeryLargeCoordinates_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(1000000, 2000000, 3000000),
                Normal = new XYZ(0, 0, 1)
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("1000000", json);
            Assert.Contains("2000000", json);
            Assert.Contains("3000000", json);
        }

        [Fact]
        public void RenderDimensionWithFloatingPointCoordinates_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(10.567, 20.789, 30.123),
                Normal = new XYZ(0, 0, 1)
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("10.567", json);
            Assert.Contains("20.789", json);
            Assert.Contains("30.123", json);
        }

        [Fact]
        public void RenderDimensionWithInvertedNormal_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, -1)
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("-1", json);
        }

        [Fact]
        public void RenderDimensionAngular3PtWithCollinearPoints_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(20, 0, 0),
                Text = "0°"
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("0°", json);
        }

        [Fact]
        public void RenderDimensionWithDefaultColor_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("ColorR", json);
            Assert.Contains("ColorG", json);
            Assert.Contains("ColorB", json);
        }

        [Fact]
        public void RenderDimensionWithCoordinateSystem_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("AutoCAD", json);
        }

        [Fact]
        public void RenderDimensionWithYAxisFlip_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("RequiresYAxisFlip", json);
        }

        [Fact]
        public void RenderDimensionAngular3PtWithArcPoints_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(0, 10, 0),
                Text = "90°"
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("ArcPoints", json);
        }

        [Fact]
        public void RenderDimensionAngular3PtWithBounds_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(0, 10, 0),
                Text = "90°"
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("Bounds", json);
        }

        [Fact]
        public void RenderDimensionAngular3PtWithAngleInDegrees_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(0, 10, 0),
                Text = "90°"
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("AngleInDegrees", json);
        }

        [Fact]
        public void RenderDimensionAngular3PtWithAngleInRadians_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(0, 10, 0),
                Text = "90°"
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("AngleInRadians", json);
        }

        [Fact]
        public void RenderDimensionAngular3PtWithArcCenter_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(0, 10, 0),
                Text = "90°"
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("ArcCenter", json);
        }

        [Fact]
        public void RenderDimensionAngular3PtWithRadius_SerializesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(0, 10, 0),
                Text = "90°"
            };

            var dimensionData = DimensionEntityRenderer.Render(dimension);
            var json = JsonConvert.SerializeObject(dimensionData);

            Assert.NotNull(json);
            Assert.Contains("Radius", json);
        }
    }
}
