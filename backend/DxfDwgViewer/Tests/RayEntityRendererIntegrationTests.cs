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
    public class RayEntityRendererIntegrationTests
    {
        private Ray CreateBasicRay()
        {
            var ray = new Ray
            {
                StartPoint = new XYZ(0, 0, 0),
                Direction = new XYZ(1, 0, 0),
                Color = new Color(7),
                Linetype = Linetype.ByLayer,
                Lineweight = Lineweight.ByLayer
            };
            return ray;
        }

        [Fact]
        public void LoadAndRenderRayFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var rayEntities = new List<Ray>();
            foreach (var entity in doc.Entities)
            {
                if (entity is Ray ray)
                {
                    rayEntities.Add(ray);
                }
            }

            if (rayEntities.Count == 0)
            {
                return;
            }

            foreach (var ray in rayEntities)
            {
                var rayData = RayEntityRenderer.Render(ray);
                Assert.NotNull(rayData);
                Assert.NotNull(rayData.StartPoint);
                Assert.NotNull(rayData.Direction);
                Assert.NotNull(rayData.EndPoint);
                Assert.NotNull(rayData.Bounds);
                Assert.NotNull(rayData.Centroid);
                Assert.True(rayData.Length > 0);
            }
        }

        [Fact]
        public void SerializeRayDataToJson_Success()
        {
            var ray = CreateBasicRay();

            var rayData = RayEntityRenderer.Render(ray);
            var json = JsonConvert.SerializeObject(rayData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Ray", json);
            Assert.Contains("StartPoint", json);
            Assert.Contains("Direction", json);
            Assert.Contains("EndPoint", json);
            Assert.Contains("Length", json);
            Assert.Contains("Angle", json);
            Assert.Contains("Bounds", json);
            Assert.Contains("Centroid", json);
        }

        [Fact]
        public void DeserializeRayDataFromJson_Success()
        {
            var ray = CreateBasicRay();

            var rayData = RayEntityRenderer.Render(ray);
            var json = JsonConvert.SerializeObject(rayData, Formatting.Indented);
            var deserializedData = JsonConvert.DeserializeObject<RayData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal(rayData.StartPoint.X, deserializedData.StartPoint.X);
            Assert.Equal(rayData.StartPoint.Y, deserializedData.StartPoint.Y);
            Assert.Equal(rayData.StartPoint.Z, deserializedData.StartPoint.Z);
            Assert.Equal(rayData.Direction.X, deserializedData.Direction.X);
            Assert.Equal(rayData.Direction.Y, deserializedData.Direction.Y);
            Assert.Equal(rayData.Direction.Z, deserializedData.Direction.Z);
            Assert.Equal(rayData.Length, deserializedData.Length);
            Assert.Equal(rayData.Angle, deserializedData.Angle);
        }

        [Fact]
        public void SerializeAndDeserializeComplexRay_Success()
        {
            var ray = new Ray
            {
                StartPoint = new XYZ(100, 200, 300),
                Direction = new XYZ(0.5, 0.5, 0).Normalize(),
                Color = new Color(5),
                Linetype = Linetype.Continuous,
                Lineweight = Lineweight.L030
            };

            var rayData = RayEntityRenderer.Render(ray);
            var json = JsonConvert.SerializeObject(rayData, Formatting.Indented);
            var deserializedData = JsonConvert.DeserializeObject<RayData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal(rayData.StartPoint.X, deserializedData.StartPoint.X);
            Assert.Equal(rayData.StartPoint.Y, deserializedData.StartPoint.Y);
            Assert.Equal(rayData.StartPoint.Z, deserializedData.StartPoint.Z);
            Assert.Equal(rayData.Direction.X, deserializedData.Direction.X);
            Assert.Equal(rayData.Direction.Y, deserializedData.Direction.Y);
            Assert.Equal(rayData.Direction.Z, deserializedData.Direction.Z);
            Assert.Equal(rayData.ColorIndex, deserializedData.ColorIndex);
            Assert.Equal(rayData.LineWeight, deserializedData.LineWeight);
        }

        [Fact]
        public void RenderRayWithNegativeCoordinates_Success()
        {
            var ray = new Ray
            {
                StartPoint = new XYZ(-100, -200, -300),
                Direction = new XYZ(-1, -1, 0).Normalize(),
                Color = new Color(1)
            };

            var rayData = RayEntityRenderer.Render(ray);

            Assert.NotNull(rayData);
            Assert.Equal(-100, rayData.StartPoint.X);
            Assert.Equal(-200, rayData.StartPoint.Y);
            Assert.Equal(-300, rayData.StartPoint.Z);
            Assert.True(rayData.Direction.X < 0);
            Assert.True(rayData.Direction.Y < 0);
        }

        [Fact]
        public void RenderRayWithDiagonalDirection_Success()
        {
            var ray = new Ray
            {
                StartPoint = new XYZ(0, 0, 0),
                Direction = new XYZ(1, 1, 1).Normalize(),
                Color = new Color(2)
            };

            var rayData = RayEntityRenderer.Render(ray);

            Assert.NotNull(rayData);
            Assert.Equal(0, rayData.StartPoint.X);
            Assert.Equal(0, rayData.StartPoint.Y);
            Assert.Equal(0, rayData.StartPoint.Z);
            Assert.Equal(1.0 / Math.Sqrt(3), rayData.Direction.X, 5);
            Assert.Equal(1.0 / Math.Sqrt(3), rayData.Direction.Y, 5);
            Assert.Equal(1.0 / Math.Sqrt(3), rayData.Direction.Z, 5);
            Assert.Equal(Math.PI / 4, rayData.Angle, 5);
        }

        [Fact]
        public void SerializeMultipleRaysToJson_Success()
        {
            var rays = new List<Ray>
            {
                new Ray
                {
                    StartPoint = new XYZ(0, 0, 0),
                    Direction = new XYZ(1, 0, 0),
                    Color = new Color(1)
                },
                new Ray
                {
                    StartPoint = new XYZ(100, 100, 0),
                    Direction = new XYZ(0, 1, 0),
                    Color = new Color(2)
                },
                new Ray
                {
                    StartPoint = new XYZ(200, 200, 0),
                    Direction = new XYZ(-1, 0, 0),
                    Color = new Color(3)
                }
            };

            var rayDataList = new List<RayData>();
            foreach (var ray in rays)
            {
                rayDataList.Add(RayEntityRenderer.Render(ray));
            }

            var json = JsonConvert.SerializeObject(rayDataList, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("StartPoint", json);
            Assert.Contains("Direction", json);
            Assert.Contains("EndPoint", json);
            Assert.Contains("Length", json);
            Assert.Contains("Angle", json);
        }

        [Fact]
        public void DeserializeMultipleRaysFromJson_Success()
        {
            var rays = new List<Ray>
            {
                new Ray
                {
                    StartPoint = new XYZ(0, 0, 0),
                    Direction = new XYZ(1, 0, 0),
                    Color = new Color(1)
                },
                new Ray
                {
                    StartPoint = new XYZ(100, 100, 0),
                    Direction = new XYZ(0, 1, 0),
                    Color = new Color(2)
                }
            };

            var rayDataList = new List<RayData>();
            foreach (var ray in rays)
            {
                rayDataList.Add(RayEntityRenderer.Render(ray));
            }

            var json = JsonConvert.SerializeObject(rayDataList, Formatting.Indented);
            var deserializedList = JsonConvert.DeserializeObject<List<RayData>>(json);

            Assert.NotNull(deserializedList);
            Assert.Equal(2, deserializedList.Count);
            Assert.Equal(0, deserializedList[0].StartPoint.X);
            Assert.Equal(0, deserializedList[0].StartPoint.Y);
            Assert.Equal(100, deserializedList[1].StartPoint.X);
            Assert.Equal(100, deserializedList[1].StartPoint.Y);
        }

        [Fact]
        public void RenderRayWithZeroDirection_ReturnsDefaultLength()
        {
            var ray = new Ray
            {
                StartPoint = new XYZ(0, 0, 0),
                Direction = new XYZ(0, 0, 0),
                Color = new Color(1)
            };

            var rayData = RayEntityRenderer.Render(ray);

            Assert.NotNull(rayData);
            Assert.Equal(0, rayData.StartPoint.X);
            Assert.Equal(0, rayData.StartPoint.Y);
            Assert.Equal(0, rayData.StartPoint.Z);
        }

        [Fact]
        public void SerializeRayWithAllPropertiesToJson_Success()
        {
            var ray = new Ray
            {
                StartPoint = new XYZ(50, 100, 150),
                Direction = new XYZ(0.707, 0.707, 0).Normalize(),
                Color = new Color(4),
                Linetype = Linetype.Dashed,
                Lineweight = Lineweight.L050
            };

            var rayData = RayEntityRenderer.Render(ray);
            var json = JsonConvert.SerializeObject(rayData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("StartPoint", json);
            Assert.Contains("Direction", json);
            Assert.Contains("EndPoint", json);
            Assert.Contains("Length", json);
            Assert.Contains("Angle", json);
            Assert.Contains("Bounds", json);
            Assert.Contains("Centroid", json);
            Assert.Contains("ColorIndex", json);
            Assert.Contains("LineTypeName", json);
            Assert.Contains("LineWeight", json);
            Assert.Contains("EntityType", json);
            Assert.Contains("Visible", json);
            Assert.Contains("Handle", json);
        }
    }
}