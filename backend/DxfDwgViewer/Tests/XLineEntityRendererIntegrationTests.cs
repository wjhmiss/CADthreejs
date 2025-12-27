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
    public class XLineEntityRendererIntegrationTests
    {
        private XLine CreateBasicXLine()
        {
            var xline = new XLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);
            xline.Color = new Color(1);
            return xline;
        }

        private XLine CreateXLineWithDirection()
        {
            var xline = new XLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(0.707, 0.707, 0);
            xline.Color = new Color(2);
            return xline;
        }

        private XLine CreateXLineWithOffset()
        {
            var xline = new XLine();
            xline.FirstPoint = new XYZ(100, 200, 50);
            xline.Direction = new XYZ(1, 0, 0);
            xline.Color = new Color(3);
            return xline;
        }

        private XLine CreateXLineWithNegativeDirection()
        {
            var xline = new XLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(-1, 0, 0);
            xline.Color = new Color(4);
            return xline;
        }

        private XLine CreateXLineWith3DDirection()
        {
            var xline = new XLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(0.5, 0.5, 0.707);
            xline.Color = new Color(5);
            return xline;
        }

        private XLine CreateXLineWithDifferentColor()
        {
            var xline = new XLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);
            xline.Color = new Color(7);
            return xline;
        }

        private XLine CreateXLineWithVerticalDirection()
        {
            var xline = new XLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(0, 1, 0);
            xline.Color = new Color(6);
            return xline;
        }

        private XLine CreateXLineWithDiagonalDirection()
        {
            var xline = new XLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(0.5, 0.866, 0);
            xline.Color = new Color(1);
            return xline;
        }

        [Fact]
        public void LoadAndRenderXLineFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var xlineEntities = new List<XLine>();
            foreach (var entity in doc.Entities)
            {
                if (entity is XLine xline)
                {
                    xlineEntities.Add(xline);
                }
            }

            if (xlineEntities.Count == 0)
            {
                return;
            }

            foreach (var xline in xlineEntities)
            {
                var xlineData = XLineEntityRenderer.Render(xline);
                Assert.NotNull(xlineData);
                Assert.NotNull(xlineData.FirstPoint);
                Assert.NotNull(xlineData.Direction);
            }
        }

        [Fact]
        public void SerializeXLineDataToJson_Success()
        {
            var xline = CreateBasicXLine();

            var xlineData = XLineEntityRenderer.Render(xline);
            var json = JsonConvert.SerializeObject(xlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("FirstPoint", json);
            Assert.Contains("Direction", json);
            Assert.Contains("ColorIndex", json);
        }

        [Fact]
        public void DeserializeXLineDataFromJson_Success()
        {
            var xline = CreateBasicXLine();

            var originalXLineData = XLineEntityRenderer.Render(xline);
            var json = JsonConvert.SerializeObject(originalXLineData);
            var deserializedXLineData = JsonConvert.DeserializeObject<XLineEntityRenderer.XLineData>(json);

            Assert.NotNull(deserializedXLineData);
            Assert.Equal(originalXLineData.FirstPoint.X, deserializedXLineData.FirstPoint.X);
            Assert.Equal(originalXLineData.FirstPoint.Y, deserializedXLineData.FirstPoint.Y);
            Assert.Equal(originalXLineData.FirstPoint.Z, deserializedXLineData.FirstPoint.Z);
            Assert.Equal(originalXLineData.Direction.X, deserializedXLineData.Direction.X);
            Assert.Equal(originalXLineData.Direction.Y, deserializedXLineData.Direction.Y);
            Assert.Equal(originalXLineData.Direction.Z, deserializedXLineData.Direction.Z);
            Assert.Equal(originalXLineData.ColorIndex, deserializedXLineData.ColorIndex);
            Assert.Equal(originalXLineData.Length, deserializedXLineData.Length);
            Assert.Equal(originalXLineData.Angle, deserializedXLineData.Angle);
        }

        [Fact]
        public void RenderMultipleXLines_CollectAllXLineData()
        {
            var xlines = new List<XLine>
            {
                CreateBasicXLine(),
                CreateXLineWithDirection()
            };
            xlines[0].FirstPoint = new XYZ(10, 10, 0);
            xlines[1].FirstPoint = new XYZ(20, 20, 0);

            var xlineDataList = new List<XLineEntityRenderer.XLineData>();
            foreach (var xline in xlines)
            {
                var xlineData = XLineEntityRenderer.Render(xline);
                xlineDataList.Add(xlineData);
            }

            Assert.Equal(2, xlineDataList.Count);
            Assert.Equal(10, xlineDataList[0].FirstPoint.X);
            Assert.Equal(20, xlineDataList[1].FirstPoint.X);
        }

        [Fact]
        public void RenderBasicXLine_PreservesAllProperties()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(100, 200, 300);
            xline.Direction = new XYZ(0.5, 0.866, 0);
            xline.Color = new Color(5);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal(100, xlineData.FirstPoint.X);
            Assert.Equal(200, xlineData.FirstPoint.Y);
            Assert.Equal(300, xlineData.FirstPoint.Z);
            Assert.Equal(0.5, xlineData.Direction.X);
            Assert.Equal(0.866, xlineData.Direction.Y);
            Assert.Equal(0, xlineData.Direction.Z);
            Assert.Equal(5, xlineData.ColorIndex);
        }

        [Fact]
        public void RenderXLineWithDirection_CalculatesCorrectAngle()
        {
            var xline = CreateXLineWithDirection();
            xline.Direction = new XYZ(0.707, 0.707, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.InRange(Math.Abs(xlineData.Angle - Math.PI / 4), 0, 0.0001);
        }

        [Fact]
        public void RenderXLineWithOffset_PreservesFirstPoint()
        {
            var xline = CreateXLineWithOffset();
            xline.FirstPoint = new XYZ(100, 200, 50);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal(100, xlineData.FirstPoint.X);
            Assert.Equal(200, xlineData.FirstPoint.Y);
            Assert.Equal(50, xlineData.FirstPoint.Z);
        }

        [Fact]
        public void RenderXLineWithNegativeDirection_CalculatesCorrectAngle()
        {
            var xline = CreateXLineWithNegativeDirection();
            xline.Direction = new XYZ(-1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.InRange(Math.Abs(xlineData.Angle - Math.PI), 0, 0.0001);
        }

        [Fact]
        public void RenderXLineWith3DDirection_PreservesDirection()
        {
            var xline = CreateXLineWith3DDirection();
            xline.Direction = new XYZ(0.5, 0.5, 0.707);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal(0.5, xlineData.Direction.X);
            Assert.Equal(0.5, xlineData.Direction.Y);
            Assert.Equal(0.707, xlineData.Direction.Z);
        }

        [Fact]
        public void RenderXLineWithDifferentColor_PreservesColor()
        {
            var xline = CreateXLineWithDifferentColor();
            xline.Color = new Color(7);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal(7, xlineData.ColorIndex);
        }

        [Fact]
        public void RenderXLineWithVerticalDirection_CalculatesCorrectAngle()
        {
            var xline = CreateXLineWithVerticalDirection();
            xline.Direction = new XYZ(0, 1, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.InRange(Math.Abs(xlineData.Angle - Math.PI / 2), 0, 0.0001);
        }

        [Fact]
        public void RenderXLineWithDiagonalDirection_CalculatesCorrectAngle()
        {
            var xline = CreateXLineWithDiagonalDirection();
            xline.Direction = new XYZ(0.5, 0.866, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.InRange(Math.Abs(xlineData.Angle - Math.PI / 3), 0, 0.0001);
        }

        [Fact]
        public void RenderXLine_CalculatesCorrectLength()
        {
            var xline = CreateBasicXLine();
            xline.Direction = new XYZ(1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal(2000, xlineData.Length);
        }

        [Fact]
        public void RenderXLine_GeneratesCorrectSecondPoint()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.SecondPoint);
            Assert.Equal(1000, xlineData.SecondPoint.X);
            Assert.Equal(0, xlineData.SecondPoint.Y);
            Assert.Equal(0, xlineData.SecondPoint.Z);
        }

        [Fact]
        public void RenderXLine_GeneratesCorrectLinePoints()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.LinePoints);
            Assert.Equal(2, xlineData.LinePoints.Length);
            Assert.Equal(-1000, xlineData.LinePoints[0].X);
            Assert.Equal(0, xlineData.LinePoints[0].Y);
            Assert.Equal(0, xlineData.LinePoints[0].Z);
            Assert.Equal(1000, xlineData.LinePoints[1].X);
            Assert.Equal(0, xlineData.LinePoints[1].Y);
            Assert.Equal(0, xlineData.LinePoints[1].Z);
        }

        [Fact]
        public void RenderXLine_GeneratesCorrectVertexPositions()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.VertexPositions);
            Assert.Equal(6, xlineData.VertexPositions.Length);
            Assert.Equal(-1000, xlineData.VertexPositions[0]);
            Assert.Equal(0, xlineData.VertexPositions[1]);
            Assert.Equal(0, xlineData.VertexPositions[2]);
            Assert.Equal(1000, xlineData.VertexPositions[3]);
            Assert.Equal(0, xlineData.VertexPositions[4]);
            Assert.Equal(0, xlineData.VertexPositions[5]);
        }

        [Fact]
        public void RenderXLine_GeneratesCorrectCenter()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.Center);
            Assert.Equal(0, xlineData.Center.X);
            Assert.Equal(0, xlineData.Center.Y);
            Assert.Equal(0, xlineData.Center.Z);
        }

        [Fact]
        public void RenderXLine_GeneratesCorrectBounds()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.Bounds);
            Assert.Equal(-1000, xlineData.Bounds.Min.X);
            Assert.Equal(0, xlineData.Bounds.Min.Y);
            Assert.Equal(0, xlineData.Bounds.Min.Z);
            Assert.Equal(1000, xlineData.Bounds.Max.X);
            Assert.Equal(0, xlineData.Bounds.Max.Y);
            Assert.Equal(0, xlineData.Bounds.Max.Z);
        }

        [Fact]
        public void RenderXLineWithLayer_PreservesLayerInformation()
        {
            var xline = CreateBasicXLine();
            xline.Layer = new Layer("TestLayer");

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal("TestLayer", xlineData.LineTypeName);
        }

        [Fact]
        public void RenderXLineWithColor_PreservesColorInformation()
        {
            var xline = CreateBasicXLine();
            xline.Color = new Color(5);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal(5, xlineData.ColorIndex);
        }

        [Fact]
        public void RenderXLineWithLineType_PreservesLineTypeInformation()
        {
            var xline = CreateBasicXLine();
            var lineType = new LineType("Continuous");
            xline.LineType = lineType;

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal("Continuous", xlineData.LineTypeName);
        }

        [Fact]
        public void RenderXLineWithLineWeight_PreservesLineWeightInformation()
        {
            var xline = CreateBasicXLine();
            xline.LineWeight = ACadSharp.LineWeightType.W30;

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal(0.3, xlineData.LineWeight);
        }

        [Fact]
        public void RenderXLineWithInvisibleFlag_SetsVisibleProperty()
        {
            var xline = CreateBasicXLine();
            xline.IsInvisible = true;

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal(1.0, xlineData.Opacity);
            Assert.False(xlineData.Transparent);
        }

        [Fact]
        public void RenderXLineWithHandle_PreservesHandleInformation()
        {
            var xline = CreateBasicXLine();
            
            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.Handle);
        }

        [Fact]
        public void RenderXLineWithZeroFirstPoint_HandlesCorrectly()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal(0, xlineData.FirstPoint.X);
            Assert.Equal(0, xlineData.FirstPoint.Y);
            Assert.Equal(0, xlineData.FirstPoint.Z);
        }

        [Fact]
        public void RenderXLineWithVeryLargeCoordinates_HandlesCorrectly()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(1000000, 2000000, 3000000);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal(1000000, xlineData.FirstPoint.X);
            Assert.Equal(2000000, xlineData.FirstPoint.Y);
            Assert.Equal(3000000, xlineData.FirstPoint.Z);
        }

        [Fact]
        public void RenderXLineWithVerySmallCoordinates_HandlesCorrectly()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0.0001, 0.0002, 0.0003);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal(0.0001, xlineData.FirstPoint.X);
            Assert.Equal(0.0002, xlineData.FirstPoint.Y);
            Assert.Equal(0.0003, xlineData.FirstPoint.Z);
        }

        [Fact]
        public void RenderXLineWithNegativeCoordinates_HandlesCorrectly()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(-10, -20, -30);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal(-10, xlineData.FirstPoint.X);
            Assert.Equal(-20, xlineData.FirstPoint.Y);
            Assert.Equal(-30, xlineData.FirstPoint.Z);
        }

        [Fact]
        public void RenderXLineWithDifferentDirections_PreservesDirections()
        {
            var directions = new[]
            {
                new XYZ(1, 0, 0),
                new XYZ(0, 1, 0),
                new XYZ(0.707, 0.707, 0),
                new XYZ(0.5, 0.866, 0),
                new XYZ(-1, 0, 0),
                new XYZ(0, -1, 0)
            };

            foreach (var direction in directions)
            {
                var xline = CreateBasicXLine();
                xline.Direction = direction;

                var xlineData = XLineEntityRenderer.Render(xline);

                Assert.NotNull(xlineData);
                Assert.Equal(direction.X, xlineData.Direction.X);
                Assert.Equal(direction.Y, xlineData.Direction.Y);
                Assert.Equal(direction.Z, xlineData.Direction.Z);
            }
        }

        [Fact]
        public void RenderXLineWith3DCoordinates_CalculatesCorrect3DProperties()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(10, 20, 30);
            xline.Direction = new XYZ(0.5, 0.5, 0.707);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.Center);
            Assert.NotNull(xlineData.Bounds);
            Assert.Equal(10, xlineData.FirstPoint.X);
            Assert.Equal(20, xlineData.FirstPoint.Y);
            Assert.Equal(30, xlineData.FirstPoint.Z);
        }

        [Fact]
        public void RenderXLine_GeneratesCorrectTransform()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.Transform);
            Assert.Equal(0, xlineData.Transform.Position.X);
            Assert.Equal(0, xlineData.Transform.Position.Y);
            Assert.Equal(0, xlineData.Transform.Position.Z);
        }

        [Fact]
        public void RenderXLine_GeneratesCorrectGeometry()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.Geometry);
            Assert.Equal("LineGeometry", xlineData.Geometry.Type);
            Assert.Equal(2, xlineData.Geometry.VertexCount);
            Assert.False(xlineData.Geometry.HasColors);
            Assert.False(xlineData.Geometry.HasIndices);
        }

        [Fact]
        public void RenderXLine_GeneratesCorrectMaterial()
        {
            var xline = CreateBasicXLine();
            xline.Color = new Color(1);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.Material);
            Assert.Equal("LineBasicMaterial", xlineData.Material.Type);
            Assert.Equal(0xFF0000, xlineData.Material.Color);
            Assert.Equal(1.0, xlineData.Material.Opacity);
            Assert.False(xlineData.Material.Transparent);
            Assert.Equal(1.0, xlineData.Material.LineWidth);
            Assert.False(xlineData.Material.Side);
        }

        [Fact]
        public void RenderXLine_GeneratesCorrectNormal()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.Normal);
            Assert.Equal(0, xlineData.Normal.X);
            Assert.Equal(0, xlineData.Normal.Y);
            Assert.Equal(1, xlineData.Normal.Z);
        }

        [Fact]
        public void RenderXLineWithDifferentColorIndex_GeneratesCorrectMaterial()
        {
            var colorIndices = new[] { 1, 2, 3, 4, 5, 6, 7 };

            foreach (var colorIndex in colorIndices)
            {
                var xline = CreateBasicXLine();
                xline.Color = new Color(colorIndex);

                var xlineData = XLineEntityRenderer.Render(xline);

                Assert.NotNull(xlineData);
                Assert.Equal(colorIndex, xlineData.ColorIndex);
            }
        }

        [Fact]
        public void RenderXLine_CalculatesCorrectAngleForHorizontalLine()
        {
            var xline = CreateBasicXLine();
            xline.Direction = new XYZ(1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.InRange(Math.Abs(xlineData.Angle - 0), 0, 0.0001);
        }

        [Fact]
        public void RenderXLine_CalculatesCorrectAngleForVerticalLine()
        {
            var xline = CreateBasicXLine();
            xline.Direction = new XYZ(0, 1, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.InRange(Math.Abs(xlineData.Angle - Math.PI / 2), 0, 0.0001);
        }

        [Fact]
        public void RenderXLine_CalculatesCorrectAngleFor45DegreeLine()
        {
            var xline = CreateBasicXLine();
            xline.Direction = new XYZ(0.707, 0.707, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.InRange(Math.Abs(xlineData.Angle - Math.PI / 4), 0, 0.0001);
        }

        [Fact]
        public void RenderXLine_CalculatesCorrectAngleFor135DegreeLine()
        {
            var xline = CreateBasicXLine();
            xline.Direction = new XYZ(-0.707, 0.707, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.InRange(Math.Abs(xlineData.Angle - 3 * Math.PI / 4), 0, 0.0001);
        }

        [Fact]
        public void RenderXLine_GeneratesCorrectVertexCount()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal(2, xlineData.VertexCount);
        }

        [Fact]
        public void RenderXLine_GeneratesCorrectOpacity()
        {
            var xline = CreateBasicXLine();

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.Equal(1.0, xlineData.Opacity);
        }

        [Fact]
        public void RenderXLine_GeneratesCorrectTransparent()
        {
            var xline = CreateBasicXLine();

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.False(xlineData.Transparent);
        }

        [Fact]
        public void RenderXLineWithOffsetFirstPoint_CalculatesCorrectCenter()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(100, 200, 50);
            xline.Direction = new XYZ(1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.Center);
            Assert.Equal(100, xlineData.Center.X);
            Assert.Equal(200, xlineData.Center.Y);
            Assert.Equal(50, xlineData.Center.Z);
        }

        [Fact]
        public void RenderXLineWithOffsetFirstPoint_CalculatesCorrectBounds()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(100, 200, 50);
            xline.Direction = new XYZ(1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.Bounds);
            Assert.Equal(-900, xlineData.Bounds.Min.X);
            Assert.Equal(200, xlineData.Bounds.Min.Y);
            Assert.Equal(50, xlineData.Bounds.Min.Z);
            Assert.Equal(1100, xlineData.Bounds.Max.X);
            Assert.Equal(200, xlineData.Bounds.Max.Y);
            Assert.Equal(50, xlineData.Bounds.Max.Z);
        }

        [Fact]
        public void RenderXLineWithOffsetFirstPoint_GeneratesCorrectLinePoints()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(100, 200, 50);
            xline.Direction = new XYZ(1, 0, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.LinePoints);
            Assert.Equal(-900, xlineData.LinePoints[0].X);
            Assert.Equal(200, xlineData.LinePoints[0].Y);
            Assert.Equal(50, xlineData.LinePoints[0].Z);
            Assert.Equal(1100, xlineData.LinePoints[1].X);
            Assert.Equal(200, xlineData.LinePoints[1].Y);
            Assert.Equal(50, xlineData.LinePoints[1].Z);
        }

        [Fact]
        public void RenderXLineWithDiagonalDirection_GeneratesCorrectLinePoints()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(0.707, 0.707, 0);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.LinePoints);
            Assert.Equal(2, xlineData.LinePoints.Length);
        }

        [Fact]
        public void RenderXLineWith3DDirection_GeneratesCorrectLinePoints()
        {
            var xline = CreateBasicXLine();
            xline.FirstPoint = new XYZ(0, 0, 0);
            xline.Direction = new XYZ(0.5, 0.5, 0.707);

            var xlineData = XLineEntityRenderer.Render(xline);

            Assert.NotNull(xlineData);
            Assert.NotNull(xlineData.LinePoints);
            Assert.Equal(2, xlineData.LinePoints.Length);
        }
    }
}
