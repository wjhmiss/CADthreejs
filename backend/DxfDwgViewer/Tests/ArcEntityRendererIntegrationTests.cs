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
    public class ArcEntityRendererIntegrationTests
    {
        [Fact]
        public void LoadAndRenderArcFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var arcEntities = new List<Arc>();
            foreach (var entity in doc.Entities)
            {
                if (entity is Arc arc)
                {
                    arcEntities.Add(arc);
                }
            }

            if (arcEntities.Count == 0)
            {
                return;
            }

            foreach (var arc in arcEntities)
            {
                var arcData = ArcEntityRenderer.Render(arc);
                Assert.NotNull(arcData);
                Assert.Equal("Arc", arcData.Type);
                Assert.True(arcData.Radius > 0);
                Assert.NotNull(arcData.Vertices);
                Assert.True(arcData.Vertices.Count > 0);
            }
        }

        [Fact]
        public void SerializeArcDataToJson_Success()
        {
            var arc = new Arc
            {
                Center = new XYZ(10, 20, 0),
                Radius = 5,
                StartAngle = 0,
                EndAngle = Math.PI / 2,
                Color = new ACadSharp.Color(1)
            };

            var arcData = ArcEntityRenderer.Render(arc);
            var json = JsonConvert.SerializeObject(arcData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Arc", json);
            Assert.Contains("CenterX", json);
            Assert.Contains("Radius", json);
        }

        [Fact]
        public void DeserializeArcDataFromJson_Success()
        {
            var arc = new Arc
            {
                Center = new XYZ(10, 20, 0),
                Radius = 5,
                StartAngle = 0,
                EndAngle = Math.PI / 2,
                Color = new ACadSharp.Color(1)
            };

            var originalArcData = ArcEntityRenderer.Render(arc);
            var json = JsonConvert.SerializeObject(originalArcData);
            var deserializedArcData = JsonConvert.DeserializeObject<ArcEntityRenderer.ArcData>(json);

            Assert.NotNull(deserializedArcData);
            Assert.Equal(originalArcData.Type, deserializedArcData.Type);
            Assert.Equal(originalArcData.CenterX, deserializedArcData.CenterX);
            Assert.Equal(originalArcData.CenterY, deserializedArcData.CenterY);
            Assert.Equal(originalArcData.Radius, deserializedArcData.Radius);
            Assert.Equal(originalArcData.StartAngle, deserializedArcData.StartAngle);
            Assert.Equal(originalArcData.EndAngle, deserializedArcData.EndAngle);
            Assert.Equal(originalArcData.ColorR, deserializedArcData.ColorR);
            Assert.Equal(originalArcData.ColorG, deserializedArcData.ColorG);
            Assert.Equal(originalArcData.ColorB, deserializedArcData.ColorB);
        }

        [Fact]
        public void RenderMultipleArcs_CollectAllArcData()
        {
            var arcs = new List<Arc>
            {
                new Arc
                {
                    Center = new XYZ(0, 0, 0),
                    Radius = 10,
                    StartAngle = 0,
                    EndAngle = Math.PI / 2
                },
                new Arc
                {
                    Center = new XYZ(20, 20, 0),
                    Radius = 15,
                    StartAngle = Math.PI / 4,
                    EndAngle = Math.PI
                },
                new Arc
                {
                    Center = new XYZ(-10, 10, 0),
                    Radius = 8,
                    StartAngle = Math.PI / 2,
                    EndAngle = 3 * Math.PI / 2
                }
            };

            var arcDataList = new List<ArcEntityRenderer.ArcData>();
            foreach (var arc in arcs)
            {
                arcDataList.Add(ArcEntityRenderer.Render(arc));
            }

            Assert.Equal(3, arcDataList.Count);
            Assert.All(arcDataList, data =>
            {
                Assert.NotNull(data);
                Assert.Equal("Arc", data.Type);
                Assert.True(data.Radius > 0);
                Assert.NotNull(data.Vertices);
                Assert.True(data.Vertices.Count > 0);
            });
        }

        [Fact]
        public void SerializeMultipleArcDataToJson_Success()
        {
            var arcs = new List<Arc>
            {
                new Arc
                {
                    Center = new XYZ(0, 0, 0),
                    Radius = 10,
                    StartAngle = 0,
                    EndAngle = Math.PI / 2
                },
                new Arc
                {
                    Center = new XYZ(20, 20, 0),
                    Radius = 15,
                    StartAngle = Math.PI / 4,
                    EndAngle = Math.PI
                }
            };

            var arcDataList = new List<ArcEntityRenderer.ArcData>();
            foreach (var arc in arcs)
            {
                arcDataList.Add(ArcEntityRenderer.Render(arc));
            }

            var json = JsonConvert.SerializeObject(arcDataList, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("CenterX", json);
            Assert.Contains("Radius", json);
        }

        [Fact]
        public void DeserializeMultipleArcDataFromJson_Success()
        {
            var arcs = new List<Arc>
            {
                new Arc
                {
                    Center = new XYZ(0, 0, 0),
                    Radius = 10,
                    StartAngle = 0,
                    EndAngle = Math.PI / 2
                },
                new Arc
                {
                    Center = new XYZ(20, 20, 0),
                    Radius = 15,
                    StartAngle = Math.PI / 4,
                    EndAngle = Math.PI
                }
            };

            var originalArcDataList = new List<ArcEntityRenderer.ArcData>();
            foreach (var arc in arcs)
            {
                originalArcDataList.Add(ArcEntityRenderer.Render(arc));
            }

            var json = JsonConvert.SerializeObject(originalArcDataList);
            var deserializedArcDataList = JsonConvert.DeserializeObject<List<ArcEntityRenderer.ArcData>>(json);

            Assert.NotNull(deserializedArcDataList);
            Assert.Equal(originalArcDataList.Count, deserializedArcDataList.Count);

            for (int i = 0; i < originalArcDataList.Count; i++)
            {
                Assert.Equal(originalArcDataList[i].Type, deserializedArcDataList[i].Type);
                Assert.Equal(originalArcDataList[i].CenterX, deserializedArcDataList[i].CenterX);
                Assert.Equal(originalArcDataList[i].CenterY, deserializedArcDataList[i].CenterY);
                Assert.Equal(originalArcDataList[i].Radius, deserializedArcDataList[i].Radius);
            }
        }

        [Fact]
        public void GenerateJsonDataForFrontend_Success()
        {
            var arcs = new List<Arc>
            {
                new Arc
                {
                    Center = new XYZ(0, 0, 0),
                    Radius = 10,
                    StartAngle = 0,
                    EndAngle = Math.PI / 2,
                    Color = new ACadSharp.Color(1)
                },
                new Arc
                {
                    Center = new XYZ(20, 20, 0),
                    Radius = 15,
                    StartAngle = Math.PI / 4,
                    EndAngle = Math.PI,
                    Color = new ACadSharp.Color(2)
                }
            };

            var arcDataList = new List<ArcEntityRenderer.ArcData>();
            foreach (var arc in arcs)
            {
                arcDataList.Add(ArcEntityRenderer.Render(arc));
            }

            var jsonData = new
            {
                ArcDatas = arcDataList
            };

            var json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("ArcDatas", json);
            Assert.Contains("Type", json);
            Assert.Contains("CenterX", json);
            Assert.Contains("Radius", json);
            Assert.Contains("ColorR", json);
        }

        [Fact]
        public void ArcDataContainsAllRequiredProperties_Success()
        {
            var arc = new Arc
            {
                Center = new XYZ(10, 20, 0),
                Radius = 5,
                StartAngle = 0,
                EndAngle = Math.PI / 2,
                Thickness = 2,
                Color = new ACadSharp.Color(3)
            };

            var arcData = ArcEntityRenderer.Render(arc);

            Assert.NotNull(arcData.Type);
            Assert.NotNull(arcData.Uuid);
            Assert.NotNull(arcData.EntityType);
            Assert.NotNull(arcData.Handle);
            Assert.NotNull(arcData.LayerName);
            Assert.NotNull(arcData.ColorHex);
            Assert.NotNull(arcData.LineTypeName);
            Assert.NotNull(arcData.MaterialType);
            Assert.NotNull(arcData.Points);
            Assert.NotNull(arcData.Vertices);
            Assert.NotNull(arcData.Indices);
            Assert.NotNull(arcData.Bounds);
            Assert.NotNull(arcData.Centroid);
            Assert.NotNull(arcData.Transform);
            Assert.NotNull(arcData.CoordinateSystem);
        }

        [Fact]
        public void ArcDataCalculationsAreAccurate_Success()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI
            };

            var arcData = ArcEntityRenderer.Render(arc);

            var expectedArcLength = 10 * Math.PI;
            var expectedChordLength = 20;
            var expectedSagitta = 10;
            var expectedArea = 0.5 * 10 * 10 * Math.PI;

            Assert.Equal(expectedArcLength, arcData.Length, 5);
            Assert.Equal(expectedChordLength, arcData.ChordLength, 5);
            Assert.Equal(expectedSagitta, arcData.Sagitta, 5);
            Assert.Equal(expectedArea, arcData.Area, 5);
        }

        [Fact]
        public void ArcDataVerticesAreCorrect_Success()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI / 2
            };

            var arcData = ArcEntityRenderer.Render(arc);

            Assert.NotNull(arcData.Vertices);
            Assert.Equal(65 * 3, arcData.Vertices.Count);

            var startX = arcData.Vertices[0];
            var startY = arcData.Vertices[1];
            var startZ = arcData.Vertices[2];

            Assert.Equal(10, startX, 5);
            Assert.Equal(0, startY, 5);
            Assert.Equal(0, startZ, 5);

            var endX = arcData.Vertices[arcData.Vertices.Count - 3];
            var endY = arcData.Vertices[arcData.Vertices.Count - 2];
            var endZ = arcData.Vertices[arcData.Vertices.Count - 1];

            Assert.Equal(0, endX, 5);
            Assert.Equal(10, endY, 5);
            Assert.Equal(0, endZ, 5);
        }

        [Fact]
        public void ArcDataIndicesAreCorrect_Success()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI / 2
            };

            var arcData = ArcEntityRenderer.Render(arc);

            Assert.NotNull(arcData.Indices);
            Assert.Equal(64 * 2, arcData.Indices.Count);
            Assert.Equal(0, arcData.Indices[0]);
            Assert.Equal(1, arcData.Indices[1]);
            Assert.Equal(63, arcData.Indices[arcData.Indices.Count - 2]);
            Assert.Equal(64, arcData.Indices[arcData.Indices.Count - 1]);
        }

        [Fact]
        public void ArcDataTransformMatrixIsCorrect_Success()
        {
            var arc = new Arc
            {
                Center = new XYZ(10, 20, 5),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI / 2,
                Normal = new XYZ(0.5, 0.5, 0.7071)
            };

            var arcData = ArcEntityRenderer.Render(arc);

            Assert.NotNull(arcData.Transform);
            Assert.NotNull(arcData.Transform.Matrix);
            Assert.Equal(16, arcData.Transform.Matrix.Length);
            Assert.Equal(1, arcData.Transform.Matrix[15]);
            Assert.Equal(10, arcData.Transform.Matrix[12], 5);
            Assert.Equal(20, arcData.Transform.Matrix[13], 5);
            Assert.Equal(5, arcData.Transform.Matrix[14], 5);
        }

        [Fact]
        public void ArcDataBoundsAreCorrect_Success()
        {
            var arc = new Arc
            {
                Center = new XYZ(10, 10, 0),
                Radius = 5,
                StartAngle = 0,
                EndAngle = Math.PI / 2
            };

            var arcData = ArcEntityRenderer.Render(arc);

            Assert.NotNull(arcData.Bounds);
            Assert.NotNull(arcData.Bounds.Min);
            Assert.NotNull(arcData.Bounds.Max);
            Assert.True(arcData.Bounds.Min.X <= 10);
            Assert.True(arcData.Bounds.Max.X >= 15);
            Assert.True(arcData.Bounds.Min.Y <= 10);
            Assert.True(arcData.Bounds.Max.Y >= 15);
            Assert.Equal(0, arcData.Bounds.Min.Z);
            Assert.Equal(0, arcData.Bounds.Max.Z);
        }

        [Fact]
        public void ArcDataCentroidIsCorrect_Success()
        {
            var arc = new Arc
            {
                Center = new XYZ(10, 10, 0),
                Radius = 5,
                StartAngle = 0,
                EndAngle = Math.PI / 2
            };

            var arcData = ArcEntityRenderer.Render(arc);

            Assert.NotNull(arcData.Centroid);
            Assert.Equal(arcData.MidX, arcData.Centroid.X, 5);
            Assert.Equal(arcData.MidY, arcData.Centroid.Y, 5);
            Assert.Equal(arcData.MidZ, arcData.Centroid.Z, 5);
        }

        [Fact]
        public void ArcDataMaterialPropertiesAreCorrect_Success()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI,
                Transparency = new ACadSharp.Transparency { Value = 50 }
            };

            var arcData = ArcEntityRenderer.Render(arc);

            Assert.Equal("LineBasicMaterial", arcData.MaterialType);
            Assert.True(arcData.MaterialTransparent);
            Assert.Equal(0.5, arcData.MaterialOpacity, 5);
            Assert.True(arcData.MaterialDepthTest);
            Assert.True(arcData.MaterialDepthWrite);
            Assert.Equal(2, arcData.MaterialSide);
        }
    }
}
