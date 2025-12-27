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
    public class CircleEntityRendererIntegrationTests
    {
        [Fact]
        public void LoadAndRenderCircleFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var circleEntities = new List<Circle>();
            foreach (var entity in doc.Entities)
            {
                if (entity is Circle circle)
                {
                    circleEntities.Add(circle);
                }
            }

            if (circleEntities.Count == 0)
            {
                return;
            }

            foreach (var circle in circleEntities)
            {
                var circleData = CircleEntityRenderer.Render(circle);
                Assert.NotNull(circleData);
                Assert.Equal("Circle", circleData.Type);
                Assert.True(circleData.Radius > 0);
                Assert.NotNull(circleData.Vertices);
                Assert.True(circleData.Vertices.Count > 0);
            }
        }

        [Fact]
        public void SerializeCircleDataToJson_Success()
        {
            var circle = new Circle
            {
                Center = new XYZ(10, 20, 0),
                Radius = 5,
                Color = new ACadSharp.Color(1)
            };

            var circleData = CircleEntityRenderer.Render(circle);
            var json = JsonConvert.SerializeObject(circleData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Circle", json);
            Assert.Contains("CenterX", json);
            Assert.Contains("Radius", json);
        }

        [Fact]
        public void DeserializeCircleDataFromJson_Success()
        {
            var circle = new Circle
            {
                Center = new XYZ(10, 20, 0),
                Radius = 5,
                Color = new ACadSharp.Color(1)
            };

            var originalCircleData = CircleEntityRenderer.Render(circle);
            var json = JsonConvert.SerializeObject(originalCircleData);
            var deserializedCircleData = JsonConvert.DeserializeObject<CircleEntityRenderer.CircleData>(json);

            Assert.NotNull(deserializedCircleData);
            Assert.Equal(originalCircleData.Type, deserializedCircleData.Type);
            Assert.Equal(originalCircleData.CenterX, deserializedCircleData.CenterX);
            Assert.Equal(originalCircleData.CenterY, deserializedCircleData.CenterY);
            Assert.Equal(originalCircleData.Radius, deserializedCircleData.Radius);
            Assert.Equal(originalCircleData.ColorR, deserializedCircleData.ColorR);
            Assert.Equal(originalCircleData.ColorG, deserializedCircleData.ColorG);
            Assert.Equal(originalCircleData.ColorB, deserializedCircleData.ColorB);
        }

        [Fact]
        public void RenderMultipleCircles_CollectAllCircleData()
        {
            var circles = new List<Circle>
            {
                new Circle
                {
                    Center = new XYZ(0, 0, 0),
                    Radius = 10
                },
                new Circle
                {
                    Center = new XYZ(20, 20, 0),
                    Radius = 15
                },
                new Circle
                {
                    Center = new XYZ(-10, 10, 0),
                    Radius = 8
                }
            };

            var circleDataList = new List<CircleEntityRenderer.CircleData>();
            foreach (var circle in circles)
            {
                circleDataList.Add(CircleEntityRenderer.Render(circle));
            }

            Assert.Equal(3, circleDataList.Count);
            Assert.All(circleDataList, data =>
            {
                Assert.NotNull(data);
                Assert.Equal("Circle", data.Type);
                Assert.True(data.Radius > 0);
                Assert.NotNull(data.Vertices);
                Assert.True(data.Vertices.Count > 0);
            });
        }

        [Fact]
        public void SerializeMultipleCircleDataToJson_Success()
        {
            var circles = new List<Circle>
            {
                new Circle
                {
                    Center = new XYZ(0, 0, 0),
                    Radius = 10
                },
                new Circle
                {
                    Center = new XYZ(20, 20, 0),
                    Radius = 15
                }
            };

            var circleDataList = new List<CircleEntityRenderer.CircleData>();
            foreach (var circle in circles)
            {
                circleDataList.Add(CircleEntityRenderer.Render(circle));
            }

            var json = JsonConvert.SerializeObject(circleDataList, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("CenterX", json);
            Assert.Contains("Radius", json);
        }

        [Fact]
        public void DeserializeMultipleCircleDataFromJson_Success()
        {
            var circles = new List<Circle>
            {
                new Circle
                {
                    Center = new XYZ(0, 0, 0),
                    Radius = 10
                },
                new Circle
                {
                    Center = new XYZ(20, 20, 0),
                    Radius = 15
                }
            };

            var originalCircleDataList = new List<CircleEntityRenderer.CircleData>();
            foreach (var circle in circles)
            {
                originalCircleDataList.Add(CircleEntityRenderer.Render(circle));
            }

            var json = JsonConvert.SerializeObject(originalCircleDataList);
            var deserializedCircleDataList = JsonConvert.DeserializeObject<List<CircleEntityRenderer.CircleData>>(json);

            Assert.NotNull(deserializedCircleDataList);
            Assert.Equal(originalCircleDataList.Count, deserializedCircleDataList.Count);

            for (int i = 0; i < originalCircleDataList.Count; i++)
            {
                Assert.Equal(originalCircleDataList[i].Type, deserializedCircleDataList[i].Type);
                Assert.Equal(originalCircleDataList[i].CenterX, deserializedCircleDataList[i].CenterX);
                Assert.Equal(originalCircleDataList[i].CenterY, deserializedCircleDataList[i].CenterY);
                Assert.Equal(originalCircleDataList[i].Radius, deserializedCircleDataList[i].Radius);
            }
        }

        [Fact]
        public void GenerateJsonDataForFrontend_Success()
        {
            var circles = new List<Circle>
            {
                new Circle
                {
                    Center = new XYZ(0, 0, 0),
                    Radius = 10,
                    Color = new ACadSharp.Color(1)
                },
                new Circle
                {
                    Center = new XYZ(20, 20, 0),
                    Radius = 15,
                    Color = new ACadSharp.Color(2)
                }
            };

            var circleDataList = new List<CircleEntityRenderer.CircleData>();
            foreach (var circle in circles)
            {
                circleDataList.Add(CircleEntityRenderer.Render(circle));
            }

            var jsonData = new
            {
                CircleDatas = circleDataList
            };

            var json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("CircleDatas", json);
            Assert.Contains("Type", json);
            Assert.Contains("CenterX", json);
            Assert.Contains("Radius", json);
            Assert.Contains("ColorR", json);
        }

        [Fact]
        public void CircleDataContainsAllRequiredProperties_Success()
        {
            var circle = new Circle
            {
                Center = new XYZ(10, 20, 0),
                Radius = 5,
                Thickness = 2,
                Color = new ACadSharp.Color(3)
            };

            var circleData = CircleEntityRenderer.Render(circle);

            Assert.NotNull(circleData.Type);
            Assert.NotNull(circleData.Uuid);
            Assert.NotNull(circleData.EntityType);
            Assert.NotNull(circleData.Handle);
            Assert.NotNull(circleData.LayerName);
            Assert.NotNull(circleData.ColorHex);
            Assert.NotNull(circleData.LineTypeName);
            Assert.NotNull(circleData.MaterialType);
            Assert.NotNull(circleData.Points);
            Assert.NotNull(circleData.Vertices);
            Assert.NotNull(circleData.Indices);
            Assert.NotNull(circleData.Bounds);
            Assert.NotNull(circleData.Centroid);
            Assert.NotNull(circleData.Transform);
            Assert.NotNull(circleData.CoordinateSystem);
        }

        [Fact]
        public void CircleDataCalculationsAreAccurate_Success()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var circleData = CircleEntityRenderer.Render(circle);

            var expectedCircumference = 2 * Math.PI * 10;
            var expectedDiameter = 20;
            var expectedArea = Math.PI * 10 * 10;

            Assert.Equal(expectedCircumference, circleData.Circumference, 5);
            Assert.Equal(expectedDiameter, circleData.Diameter);
            Assert.Equal(expectedArea, circleData.Area, 5);
        }

        [Fact]
        public void CircleDataVerticesAreCorrect_Success()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var circleData = CircleEntityRenderer.Render(circle);

            Assert.NotNull(circleData.Vertices);
            Assert.Equal(65 * 3, circleData.Vertices.Count);

            var firstX = circleData.Vertices[0];
            var firstY = circleData.Vertices[1];
            var firstZ = circleData.Vertices[2];

            Assert.Equal(10, firstX, 5);
            Assert.Equal(0, firstY, 5);
            Assert.Equal(0, firstZ, 5);

            var lastX = circleData.Vertices[circleData.Vertices.Count - 3];
            var lastY = circleData.Vertices[circleData.Vertices.Count - 2];
            var lastZ = circleData.Vertices[circleData.Vertices.Count - 1];

            Assert.Equal(10, lastX, 5);
            Assert.Equal(0, lastY, 5);
            Assert.Equal(0, lastZ, 5);
        }

        [Fact]
        public void CircleDataIndicesAreCorrect_Success()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var circleData = CircleEntityRenderer.Render(circle);

            Assert.NotNull(circleData.Indices);
            Assert.Equal(64 * 2, circleData.Indices.Count);
            Assert.Equal(0, circleData.Indices[0]);
            Assert.Equal(1, circleData.Indices[1]);
            Assert.Equal(63, circleData.Indices[circleData.Indices.Count - 2]);
            Assert.Equal(64, circleData.Indices[circleData.Indices.Count - 1]);
        }

        [Fact]
        public void CircleDataTransformMatrixIsCorrect_Success()
        {
            var circle = new Circle
            {
                Center = new XYZ(10, 20, 5),
                Radius = 10,
                Normal = new XYZ(0.5, 0.5, 0.7071)
            };

            var circleData = CircleEntityRenderer.Render(circle);

            Assert.NotNull(circleData.Transform);
            Assert.NotNull(circleData.Transform.Matrix);
            Assert.Equal(16, circleData.Transform.Matrix.Length);
            Assert.Equal(1, circleData.Transform.Matrix[15]);
            Assert.Equal(10, circleData.Transform.Matrix[12], 5);
            Assert.Equal(20, circleData.Transform.Matrix[13], 5);
            Assert.Equal(5, circleData.Transform.Matrix[14], 5);
        }

        [Fact]
        public void CircleDataBoundsAreCorrect_Success()
        {
            var circle = new Circle
            {
                Center = new XYZ(10, 10, 0),
                Radius = 5,
                Thickness = 2
            };

            var circleData = CircleEntityRenderer.Render(circle);

            Assert.NotNull(circleData.Bounds);
            Assert.NotNull(circleData.Bounds.Min);
            Assert.NotNull(circleData.Bounds.Max);
            Assert.Equal(5, circleData.Bounds.Min.X);
            Assert.Equal(5, circleData.Bounds.Min.Y);
            Assert.Equal(-1, circleData.Bounds.Min.Z);
            Assert.Equal(15, circleData.Bounds.Max.X);
            Assert.Equal(15, circleData.Bounds.Max.Y);
            Assert.Equal(1, circleData.Bounds.Max.Z);
        }

        [Fact]
        public void CircleDataCentroidIsCorrect_Success()
        {
            var circle = new Circle
            {
                Center = new XYZ(15, 25, 10),
                Radius = 7
            };

            var circleData = CircleEntityRenderer.Render(circle);

            Assert.NotNull(circleData.Centroid);
            Assert.Equal(circleData.CenterX, circleData.Centroid.X);
            Assert.Equal(circleData.CenterY, circleData.Centroid.Y);
            Assert.Equal(circleData.CenterZ, circleData.Centroid.Z);
        }

        [Fact]
        public void CircleDataMaterialPropertiesAreCorrect_Success()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                Transparency = new ACadSharp.Transparency { Value = 50 }
            };

            var circleData = CircleEntityRenderer.Render(circle);

            Assert.Equal("LineBasicMaterial", circleData.MaterialType);
            Assert.True(circleData.MaterialTransparent);
            Assert.Equal(0.5, circleData.MaterialOpacity, 5);
            Assert.True(circleData.MaterialDepthTest);
            Assert.True(circleData.MaterialDepthWrite);
            Assert.Equal(2, circleData.MaterialSide);
        }

        [Fact]
        public void CircleDataPointsListIsCorrect_Success()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var circleData = CircleEntityRenderer.Render(circle);

            Assert.NotNull(circleData.Points);
            Assert.Equal(65, circleData.Points.Count);

            var firstPoint = circleData.Points[0];
            var lastPoint = circleData.Points[circleData.Points.Count - 1];

            Assert.Equal(firstPoint.X, lastPoint.X, 5);
            Assert.Equal(firstPoint.Y, lastPoint.Y, 5);
            Assert.Equal(firstPoint.Z, lastPoint.Z, 5);
        }

        [Fact]
        public void CircleDataWithDifferentNormalVectors_Success()
        {
            var circles = new List<Circle>
            {
                new Circle
                {
                    Center = new XYZ(0, 0, 0),
                    Radius = 10,
                    Normal = new XYZ(0, 0, 1)
                },
                new Circle
                {
                    Center = new XYZ(10, 10, 10),
                    Radius = 5,
                    Normal = new XYZ(1, 0, 0)
                },
                new Circle
                {
                    Center = new XYZ(-5, -5, -5),
                    Radius = 8,
                    Normal = new XYZ(0, 1, 0)
                }
            };

            var circleDataList = new List<CircleEntityRenderer.CircleData>();
            foreach (var circle in circles)
            {
                circleDataList.Add(CircleEntityRenderer.Render(circle));
            }

            Assert.Equal(3, circleDataList.Count);
            Assert.All(circleDataList, data =>
            {
                Assert.NotNull(data);
                Assert.NotNull(data.Transform);
                Assert.NotNull(data.Transform.Matrix);
                Assert.Equal(16, data.Transform.Matrix.Length);
            });
        }

        [Fact]
        public void CircleDataWithDifferentColors_Success()
        {
            var circles = new List<Circle>
            {
                new Circle
                {
                    Center = new XYZ(0, 0, 0),
                    Radius = 10,
                    Color = new ACadSharp.Color(1)
                },
                new Circle
                {
                    Center = new XYZ(10, 10, 0),
                    Radius = 10,
                    Color = new ACadSharp.Color(2)
                },
                new Circle
                {
                    Center = new XYZ(20, 20, 0),
                    Radius = 10,
                    Color = new ACadSharp.Color(3)
                }
            };

            var circleDataList = new List<CircleEntityRenderer.CircleData>();
            foreach (var circle in circles)
            {
                circleDataList.Add(CircleEntityRenderer.Render(circle));
            }

            Assert.Equal(3, circleDataList.Count);
            Assert.Equal(1, circleDataList[0].ColorIndex);
            Assert.Equal(2, circleDataList[1].ColorIndex);
            Assert.Equal(3, circleDataList[2].ColorIndex);
            Assert.Equal("#FF0000", circleDataList[0].ColorHex);
            Assert.Equal("#FFFF00", circleDataList[1].ColorHex);
            Assert.Equal("#00FF00", circleDataList[2].ColorHex);
        }

        [Fact]
        public void CircleDataWithDifferentThickness_Success()
        {
            var circles = new List<Circle>
            {
                new Circle
                {
                    Center = new XYZ(0, 0, 0),
                    Radius = 10,
                    Thickness = 0
                },
                new Circle
                {
                    Center = new XYZ(10, 10, 0),
                    Radius = 10,
                    Thickness = 5
                },
                new Circle
                {
                    Center = new XYZ(20, 20, 0),
                    Radius = 10,
                    Thickness = -3
                }
            };

            var circleDataList = new List<CircleEntityRenderer.CircleData>();
            foreach (var circle in circles)
            {
                circleDataList.Add(CircleEntityRenderer.Render(circle));
            }

            Assert.Equal(3, circleDataList.Count);
            Assert.Equal(0, circleDataList[0].Thickness);
            Assert.Equal(5, circleDataList[1].Thickness);
            Assert.Equal(-3, circleDataList[2].Thickness);
        }

        [Fact]
        public void CircleDataWithDifferentLineTypeScale_Success()
        {
            var circles = new List<Circle>
            {
                new Circle
                {
                    Center = new XYZ(0, 0, 0),
                    Radius = 10,
                    LineTypeScale = 1.0
                },
                new Circle
                {
                    Center = new XYZ(10, 10, 0),
                    Radius = 10,
                    LineTypeScale = 2.5
                },
                new Circle
                {
                    Center = new XYZ(20, 20, 0),
                    Radius = 10,
                    LineTypeScale = 0.5
                }
            };

            var circleDataList = new List<CircleEntityRenderer.CircleData>();
            foreach (var circle in circles)
            {
                circleDataList.Add(CircleEntityRenderer.Render(circle));
            }

            Assert.Equal(3, circleDataList.Count);
            Assert.Equal(1.0, circleDataList[0].LineTypeScale);
            Assert.Equal(2.5, circleDataList[1].LineTypeScale);
            Assert.Equal(0.5, circleDataList[2].LineTypeScale);
        }

        [Fact]
        public void CircleDataWithInvisibleFlag_Success()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };
            circle.IsInvisible = true;

            var circleData = CircleEntityRenderer.Render(circle);

            Assert.True(circleData.IsInvisible);
            Assert.False(circleData.Visible);
        }

        [Fact]
        public void CircleDataWithDifferentRadii_Success()
        {
            var circles = new List<Circle>
            {
                new Circle
                {
                    Center = new XYZ(0, 0, 0),
                    Radius = 1
                },
                new Circle
                {
                    Center = new XYZ(10, 10, 0),
                    Radius = 100
                },
                new Circle
                {
                    Center = new XYZ(20, 20, 0),
                    Radius = 1000
                }
            };

            var circleDataList = new List<CircleEntityRenderer.CircleData>();
            foreach (var circle in circles)
            {
                circleDataList.Add(CircleEntityRenderer.Render(circle));
            }

            Assert.Equal(3, circleDataList.Count);
            Assert.Equal(1, circleDataList[0].Radius);
            Assert.Equal(100, circleDataList[1].Radius);
            Assert.Equal(1000, circleDataList[2].Radius);
            Assert.Equal(2, circleDataList[0].Diameter);
            Assert.Equal(200, circleDataList[1].Diameter);
            Assert.Equal(2000, circleDataList[2].Diameter);
        }

        [Fact]
        public void CircleDataVerticesAreOnCircle_Success()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var circleData = CircleEntityRenderer.Render(circle);

            for (int i = 0; i < circleData.Vertices.Count; i += 3)
            {
                double x = circleData.Vertices[i];
                double y = circleData.Vertices[i + 1];
                double z = circleData.Vertices[i + 2];

                double distance = Math.Sqrt(x * x + y * y);
                Assert.Equal(10, distance, 1);
                Assert.Equal(0, z);
            }
        }

        [Fact]
        public void CircleDataIndicesAreSequential_Success()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var circleData = CircleEntityRenderer.Render(circle);

            for (int i = 0; i < circleData.Indices.Count; i += 2)
            {
                int current = circleData.Indices[i];
                int next = circleData.Indices[i + 1];
                Assert.Equal(current + 1, next);
            }
        }
    }
}
