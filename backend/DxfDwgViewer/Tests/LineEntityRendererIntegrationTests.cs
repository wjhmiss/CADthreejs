using Xunit;
using ACadSharp.Entities;
using ACadSharp.IO;
using ACadSharp.Tables;
using DxfDwgViewer.RenderUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class LineEntityRendererIntegrationTests
    {
        [Fact]
        public void LoadAndRenderLineFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var lineEntities = new List<Line>();
            foreach (var entity in doc.Entities)
            {
                if (entity is Line line)
                {
                    lineEntities.Add(line);
                }
            }

            if (lineEntities.Count == 0)
            {
                return;
            }

            foreach (var line in lineEntities)
            {
                var lineData = LineEntityRenderer.Render(line);
                Assert.NotNull(lineData);
                Assert.Equal("Line", lineData.Type);
                Assert.True(lineData.Length >= 0);
                Assert.NotNull(lineData.StartPoint3D);
                Assert.NotNull(lineData.EndPoint3D);
            }
        }

        [Fact]
        public void SerializeLineDataToJson_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 10, 0),
                Color = new ACadSharp.Color(1),
                Thickness = 1.5
            };

            var lineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(lineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Line", json);
            Assert.Contains("StartPointX", json);
            Assert.Contains("EndPointX", json);
            Assert.Contains("Length", json);
        }

        [Fact]
        public void DeserializeLineDataFromJson_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(5, 10, 0),
                EndPoint = new XYZ(15, 20, 0),
                Color = new ACadSharp.Color(5),
                Thickness = 2.5
            };

            var originalLineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(originalLineData);
            var deserializedLineData = JsonConvert.DeserializeObject<LineEntityRenderer.LineData>(json);

            Assert.NotNull(deserializedLineData);
            Assert.Equal(originalLineData.Type, deserializedLineData.Type);
            Assert.Equal(originalLineData.StartPointX, deserializedLineData.StartPointX);
            Assert.Equal(originalLineData.StartPointY, deserializedLineData.StartPointY);
            Assert.Equal(originalLineData.StartPoint3D.Z, deserializedLineData.StartPoint3D.Z);
            Assert.Equal(originalLineData.EndPointX, deserializedLineData.EndPointX);
            Assert.Equal(originalLineData.EndPointY, deserializedLineData.EndPointY);
            Assert.Equal(originalLineData.EndPoint3D.Z, deserializedLineData.EndPoint3D.Z);
            Assert.Equal(originalLineData.Length, deserializedLineData.Length);
            Assert.Equal(originalLineData.Angle, deserializedLineData.Angle);
            Assert.Equal(originalLineData.Thickness, deserializedLineData.Thickness);
            Assert.Equal(originalLineData.ColorIndex, deserializedLineData.ColorIndex);
        }

        [Fact]
        public void RenderMultipleLines_CollectAllLineData()
        {
            var lines = new List<Line>
            {
                new Line
                {
                    StartPoint = new XYZ(0, 0, 0),
                    EndPoint = new XYZ(10, 0, 0),
                    Color = new ACadSharp.Color(1)
                },
                new Line
                {
                    StartPoint = new XYZ(10, 0, 0),
                    EndPoint = new XYZ(10, 10, 0),
                    Color = new ACadSharp.Color(2)
                },
                new Line
                {
                    StartPoint = new XYZ(10, 10, 0),
                    EndPoint = new XYZ(0, 10, 0),
                    Color = new ACadSharp.Color(3)
                }
            };

            var lineDataList = new List<LineEntityRenderer.LineData>();
            foreach (var line in lines)
            {
                lineDataList.Add(LineEntityRenderer.Render(line));
            }

            Assert.Equal(3, lineDataList.Count);
            Assert.All(lineDataList, data =>
            {
                Assert.NotNull(data);
                Assert.Equal("Line", data.Type);
                Assert.True(data.Length >= 0);
                Assert.NotNull(data.StartPoint3D);
                Assert.NotNull(data.EndPoint3D);
            });
        }

        [Fact]
        public void SerializeMultipleLineDataToJson_Success()
        {
            var lines = new List<Line>
            {
                new Line
                {
                    StartPoint = new XYZ(0, 0, 0),
                    EndPoint = new XYZ(10, 10, 0)
                },
                new Line
                {
                    StartPoint = new XYZ(20, 20, 0),
                    EndPoint = new XYZ(30, 30, 0)
                }
            };

            var lineDataList = new List<LineEntityRenderer.LineData>();
            foreach (var line in lines)
            {
                lineDataList.Add(LineEntityRenderer.Render(line));
            }

            var json = JsonConvert.SerializeObject(lineDataList, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Line", json);
            Assert.Contains("StartPointX", json);
            Assert.Contains("EndPointX", json);
        }

        [Fact]
        public void DeserializeMultipleLineDataFromJson_Success()
        {
            var lines = new List<Line>
            {
                new Line
                {
                    StartPoint = new XYZ(0, 0, 0),
                    EndPoint = new XYZ(10, 10, 0),
                    Color = new ACadSharp.Color(1)
                },
                new Line
                {
                    StartPoint = new XYZ(20, 20, 0),
                    EndPoint = new XYZ(30, 30, 0),
                    Color = new ACadSharp.Color(2)
                }
            };

            var originalLineDataList = new List<LineEntityRenderer.LineData>();
            foreach (var line in lines)
            {
                originalLineDataList.Add(LineEntityRenderer.Render(line));
            }

            var json = JsonConvert.SerializeObject(originalLineDataList);
            var deserializedLineDataList = JsonConvert.DeserializeObject<List<LineEntityRenderer.LineData>>(json);

            Assert.NotNull(deserializedLineDataList);
            Assert.Equal(2, deserializedLineDataList.Count);

            for (int i = 0; i < originalLineDataList.Count; i++)
            {
                Assert.Equal(originalLineDataList[i].Type, deserializedLineDataList[i].Type);
                Assert.Equal(originalLineDataList[i].StartPointX, deserializedLineDataList[i].StartPointX);
                Assert.Equal(originalLineDataList[i].StartPointY, deserializedLineDataList[i].StartPointY);
                Assert.Equal(originalLineDataList[i].EndPointX, deserializedLineDataList[i].EndPointX);
                Assert.Equal(originalLineDataList[i].EndPointY, deserializedLineDataList[i].EndPointY);
                Assert.Equal(originalLineDataList[i].Length, deserializedLineDataList[i].Length);
                Assert.Equal(originalLineDataList[i].Angle, deserializedLineDataList[i].Angle);
            }
        }

        [Fact]
        public void SerializeAndDeserializeComplexLineData_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(-10.5, -20.5, -5.5),
                EndPoint = new XYZ(15.5, 30.5, 10.5),
                Color = new ACadSharp.Color(7),
                Thickness = 3.5,
                Normal = new XYZ(0, 0, 1),
                LineType = new LineType("DASHED"),
                LineWeight = ACadSharp.LineWeightType.W50,
                LineTypeScale = 2.0
            };

            var originalLineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(originalLineData, Formatting.Indented);
            var deserializedLineData = JsonConvert.DeserializeObject<LineEntityRenderer.LineData>(json);

            Assert.NotNull(deserializedLineData);
            Assert.Equal(originalLineData.Type, deserializedLineData.Type);
            Assert.Equal(originalLineData.StartPointX, deserializedLineData.StartPointX);
            Assert.Equal(originalLineData.StartPointY, deserializedLineData.StartPointY);
            Assert.Equal(originalLineData.StartPoint3D.Z, deserializedLineData.StartPoint3D.Z);
            Assert.Equal(originalLineData.EndPointX, deserializedLineData.EndPointX);
            Assert.Equal(originalLineData.EndPointY, deserializedLineData.EndPointY);
            Assert.Equal(originalLineData.EndPoint3D.Z, deserializedLineData.EndPoint3D.Z);
            Assert.Equal(originalLineData.Length, deserializedLineData.Length);
            Assert.Equal(originalLineData.Angle, deserializedLineData.Angle);
            Assert.Equal(originalLineData.Thickness, deserializedLineData.Thickness);
            Assert.Equal(originalLineData.ColorIndex, deserializedLineData.ColorIndex);
            Assert.Equal(originalLineData.LineTypeName, deserializedLineData.LineTypeName);
            Assert.Equal(originalLineData.LineWeight, deserializedLineData.LineWeight);
            Assert.Equal(originalLineData.LineTypeScale, deserializedLineData.LineTypeScale);
            Assert.Equal(originalLineData.Normal.X, deserializedLineData.Normal.X);
            Assert.Equal(originalLineData.Normal.Y, deserializedLineData.Normal.Y);
            Assert.Equal(originalLineData.Normal.Z, deserializedLineData.Normal.Z);
        }

        [Fact]
        public void SerializeLineDataWithTransform_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 10, 0)
            };

            var lineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(lineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Transform", json);
            Assert.Contains("Matrix", json);
        }

        [Fact]
        public void DeserializeLineDataWithTransform_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 10, 0)
            };

            var originalLineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(originalLineData);
            var deserializedLineData = JsonConvert.DeserializeObject<LineEntityRenderer.LineData>(json);

            Assert.NotNull(deserializedLineData);
            Assert.NotNull(deserializedLineData.Transform);
            Assert.NotNull(deserializedLineData.Transform.Matrix);
            Assert.Equal(16, deserializedLineData.Transform.Matrix.Length);
        }

        [Fact]
        public void SerializeLineDataWithBounds_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 20, 5)
            };

            var lineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(lineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Bounds", json);
            Assert.Contains("Min", json);
            Assert.Contains("Max", json);
        }

        [Fact]
        public void DeserializeLineDataWithBounds_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 20, 5)
            };

            var originalLineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(originalLineData);
            var deserializedLineData = JsonConvert.DeserializeObject<LineEntityRenderer.LineData>(json);

            Assert.NotNull(deserializedLineData);
            Assert.NotNull(deserializedLineData.Bounds);
            Assert.NotNull(deserializedLineData.Bounds.Min);
            Assert.NotNull(deserializedLineData.Bounds.Max);
            Assert.Equal(originalLineData.Bounds.Min.X, deserializedLineData.Bounds.Min.X);
            Assert.Equal(originalLineData.Bounds.Min.Y, deserializedLineData.Bounds.Min.Y);
            Assert.Equal(originalLineData.Bounds.Min.Z, deserializedLineData.Bounds.Min.Z);
            Assert.Equal(originalLineData.Bounds.Max.X, deserializedLineData.Bounds.Max.X);
            Assert.Equal(originalLineData.Bounds.Max.Y, deserializedLineData.Bounds.Max.Y);
            Assert.Equal(originalLineData.Bounds.Max.Z, deserializedLineData.Bounds.Max.Z);
        }

        [Fact]
        public void SerializeLineDataWithColor_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 10, 0),
                Color = new ACadSharp.Color(5)
            };

            var lineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(lineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Color", json);
            Assert.Contains("Index", json);
            Assert.Contains("Hex", json);
        }

        [Fact]
        public void DeserializeLineDataWithColor_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 10, 0),
                Color = new ACadSharp.Color(5)
            };

            var originalLineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(originalLineData);
            var deserializedLineData = JsonConvert.DeserializeObject<LineEntityRenderer.LineData>(json);

            Assert.NotNull(deserializedLineData);
            Assert.NotNull(deserializedLineData.Color);
            Assert.Equal(originalLineData.Color.Index, deserializedLineData.Color.Index);
            Assert.Equal(originalLineData.Color.Hex, deserializedLineData.Color.Hex);
        }

        [Fact]
        public void SerializeLineDataWithMaterialProperties_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 10, 0)
            };

            var lineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(lineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("MaterialType", json);
            Assert.Contains("Opacity", json);
            Assert.Contains("Transparent", json);
        }

        [Fact]
        public void DeserializeLineDataWithMaterialProperties_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 10, 0)
            };

            var originalLineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(originalLineData);
            var deserializedLineData = JsonConvert.DeserializeObject<LineEntityRenderer.LineData>(json);

            Assert.NotNull(deserializedLineData);
            Assert.Equal(originalLineData.MaterialType, deserializedLineData.MaterialType);
            Assert.Equal(originalLineData.Opacity, deserializedLineData.Opacity);
            Assert.Equal(originalLineData.Transparent, deserializedLineData.Transparent);
            Assert.Equal(originalLineData.DepthTest, deserializedLineData.DepthTest);
            Assert.Equal(originalLineData.DepthWrite, deserializedLineData.DepthWrite);
        }

        [Fact]
        public void SerializeLineDataWithMidPoint_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 20, 0)
            };

            var lineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(lineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("MidPointX", json);
            Assert.Contains("MidPointY", json);
        }

        [Fact]
        public void DeserializeLineDataWithMidPoint_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(0, 0, 0),
                EndPoint = new XYZ(10, 20, 0)
            };

            var originalLineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(originalLineData);
            var deserializedLineData = JsonConvert.DeserializeObject<LineEntityRenderer.LineData>(json);

            Assert.NotNull(deserializedLineData);
            Assert.Equal(originalLineData.MidPointX, deserializedLineData.MidPointX);
            Assert.Equal(originalLineData.MidPointY, deserializedLineData.MidPointY);
        }

        [Fact]
        public void SerializeLineDataWith3DPoints_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(5, 10, 15),
                EndPoint = new XYZ(15, 20, 25)
            };

            var lineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(lineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("StartPoint3D", json);
            Assert.Contains("EndPoint3D", json);
        }

        [Fact]
        public void DeserializeLineDataWith3DPoints_Success()
        {
            var line = new Line
            {
                StartPoint = new XYZ(5, 10, 15),
                EndPoint = new XYZ(15, 20, 25)
            };

            var originalLineData = LineEntityRenderer.Render(line);
            var json = JsonConvert.SerializeObject(originalLineData);
            var deserializedLineData = JsonConvert.DeserializeObject<LineEntityRenderer.LineData>(json);

            Assert.NotNull(deserializedLineData);
            Assert.NotNull(deserializedLineData.StartPoint3D);
            Assert.NotNull(deserializedLineData.EndPoint3D);
            Assert.Equal(originalLineData.StartPoint3D.X, deserializedLineData.StartPoint3D.X);
            Assert.Equal(originalLineData.StartPoint3D.Y, deserializedLineData.StartPoint3D.Y);
            Assert.Equal(originalLineData.StartPoint3D.Z, deserializedLineData.StartPoint3D.Z);
            Assert.Equal(originalLineData.EndPoint3D.X, deserializedLineData.EndPoint3D.X);
            Assert.Equal(originalLineData.EndPoint3D.Y, deserializedLineData.EndPoint3D.Y);
            Assert.Equal(originalLineData.EndPoint3D.Z, deserializedLineData.EndPoint3D.Z);
        }
    }
}
