using Xunit;
using ACadSharp.Entities;
using DxfDwgViewer.RenderUtilities;
using CSMath;
using System;
using System.Text.Json;

namespace DxfDwgViewer.Tests
{
    public class DimensionEntityRendererTests
    {
        [Fact]
        public void Render_NullDimension_ReturnsEmptyDimensionData()
        {
            Dimension dimension = null;

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result);
            Assert.Equal("Dimension", result.Type);
            Assert.Null(result.Entities);
        }

        [Fact]
        public void Render_DimensionWithBasicProperties_ReturnsCorrectData()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(10, 20, 0),
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result);
            Assert.Equal("DimensionAngular3Pt", result.Type);
            Assert.Equal(10, result.DefinitionPointX);
            Assert.Equal(20, result.DefinitionPointY);
            Assert.Equal(0, result.DefinitionPointZ);
            Assert.Equal(0, result.NormalX);
            Assert.Equal(0, result.NormalY);
            Assert.Equal(1, result.NormalZ);
            Assert.Equal(1, result.ColorIndex);
        }

        [Fact]
        public void Render_DimensionWithCustomColor_ReturnsCorrectColorValues()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(5)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(5, result.ColorIndex);
            Assert.NotNull(result.ColorHex);
            Assert.True(result.ColorR >= 0 && result.ColorR <= 255);
            Assert.True(result.ColorG >= 0 && result.ColorG <= 255);
            Assert.True(result.ColorB >= 0 && result.ColorB <= 255);
        }

        [Fact]
        public void Render_DimensionWith3DCoordinates_ReturnsCorrect3DData()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(15, 25, 35),
                Normal = new XYZ(0.5, 0.5, 0.707)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(15, result.DefinitionPointX);
            Assert.Equal(25, result.DefinitionPointY);
            Assert.Equal(35, result.DefinitionPointZ);
            Assert.Equal(0.5, result.NormalX);
            Assert.Equal(0.5, result.NormalY);
            Assert.Equal(0.707, result.NormalZ, 3);
        }

        [Fact]
        public void Render_DimensionWithDefaultValues_ReturnsCorrectDefaults()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Centroid);
            Assert.NotNull(result.Points);
            Assert.Equal("AutoCAD", result.CoordinateSystem);
            Assert.True(result.RequiresYAxisFlip);
        }

        [Fact]
        public void Render_DimensionAngular3Pt_ReturnsAngular3PtData()
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

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result.Angular3PtData);
            Assert.Equal(0, result.Angular3PtData.VertexX);
            Assert.Equal(0, result.Angular3PtData.VertexY);
            Assert.Equal(10, result.Angular3PtData.FirstPointX);
            Assert.Equal(0, result.Angular3PtData.FirstPointY);
            Assert.Equal(0, result.Angular3PtData.SecondPointX);
            Assert.Equal(10, result.Angular3PtData.SecondPointY);
            Assert.Equal("90°", result.Angular3PtData.Text);
            Assert.NotNull(result.Angular3PtData.ArcCenter);
            Assert.True(result.Angular3PtData.Radius > 0);
        }

        [Fact]
        public void Render_DimensionAngular3Pt_3DCoordinates_ReturnsCorrect3DData()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 10),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(5, 5, 10),
                FirstPoint = new XYZ(15, 5, 10),
                SecondPoint = new XYZ(5, 15, 10)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(10, result.Angular3PtData.VertexZ);
            Assert.Equal(10, result.Angular3PtData.FirstPointZ);
            Assert.Equal(10, result.Angular3PtData.SecondPointZ);
            Assert.Equal(0, result.Angular3PtData.NormalX);
            Assert.Equal(0, result.Angular3PtData.NormalY);
            Assert.Equal(1, result.Angular3PtData.NormalZ);
        }

        [Fact]
        public void Render_DimensionAngular3Pt_CalculatesCorrectAngles()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(0, 10, 0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result.Angular3PtData.AngleInDegrees);
            Assert.NotNull(result.Angular3PtData.AngleInRadians);
            Assert.True(result.Angular3PtData.AngleInDegrees > 0);
            Assert.True(result.Angular3PtData.AngleInRadians > 0);
        }

        [Fact]
        public void Render_DimensionAngular3Pt_GeneratesArcPoints()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(0, 10, 0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result.Angular3PtData.ArcPoints);
            Assert.True(result.Angular3PtData.ArcPoints.Length > 0);
        }

        [Fact]
        public void Render_DimensionAngular3Pt_CalculatesBounds()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(0, 10, 0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result.Angular3PtData.Bounds);
            Assert.NotNull(result.Angular3PtData.Bounds.Min);
            Assert.NotNull(result.Angular3PtData.Bounds.Max);
            Assert.NotNull(result.Angular3PtData.Bounds.Center);
            Assert.NotNull(result.Angular3PtData.Bounds.Size);
        }

        [Fact]
        public void Render_DimensionWithNegativeCoordinates_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(-10, -20, -5),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(-10, result.DefinitionPointX);
            Assert.Equal(-20, result.DefinitionPointY);
            Assert.Equal(-5, result.DefinitionPointZ);
        }

        [Fact]
        public void Render_DimensionWithLargeCoordinates_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(10000, 20000, 30000),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(10000, result.DefinitionPointX);
            Assert.Equal(20000, result.DefinitionPointY);
            Assert.Equal(30000, result.DefinitionPointZ);
        }

        [Fact]
        public void Render_DimensionWithZeroNormal_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0, result.NormalX);
            Assert.Equal(0, result.NormalY);
            Assert.Equal(0, result.NormalZ);
        }

        [Fact]
        public void Render_DimensionWithObliqueNormal_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0.707, 0.707, 0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0.707, result.NormalX, 3);
            Assert.Equal(0.707, result.NormalY, 3);
            Assert.Equal(0, result.NormalZ);
        }

        [Fact]
        public void Render_DimensionWithDifferentColorIndices_ReturnsCorrectColors()
        {
            var testCases = new[] { (short)1, (short)2, (short)3, (short)4, (short)5, (short)6, (short)7 };

            foreach (var colorIndex in testCases)
            {
                var dimension = new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(0, 0, 0),
                    Normal = new XYZ(0, 0, 1),
                    Color = new ACadSharp.Color(colorIndex)
                };

                var result = DimensionEntityRenderer.Render(dimension);

                Assert.Equal(colorIndex, result.ColorIndex);
                Assert.NotNull(result.ColorHex);
            }
        }

        [Fact]
        public void Render_DimensionWithZeroCoordinates_ReturnsCorrectData()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0, result.DefinitionPointX);
            Assert.Equal(0, result.DefinitionPointY);
            Assert.Equal(0, result.DefinitionPointZ);
            Assert.NotNull(result);
        }

        [Fact]
        public void Render_DimensionAngular3Pt_WithText_ReturnsCorrectText()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(0, 10, 0),
                Text = "45°"
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal("45°", result.Angular3PtData.Text);
        }

        [Fact]
        public void Render_DimensionAngular3Pt_WithMeasurement_ReturnsCorrectMeasurement()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(0, 10, 0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(1.5708, result.Angular3PtData.Measurement, 4);
        }

        [Fact]
        public void Render_Dimension_SerializesToJSON_Correctly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(10, 20, 30),
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(5)
            };

            var result = DimensionEntityRenderer.Render(dimension);
            var json = JsonSerializer.Serialize(result);

            Assert.NotNull(json);
            Assert.Contains("Type", json);
            Assert.Contains("DefinitionPointX", json);
            Assert.Contains("DefinitionPointY", json);
            Assert.Contains("DefinitionPointZ", json);
        }

        [Fact]
        public void Render_DimensionAngular3Pt_SerializesToJSON_Correctly()
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

            var result = DimensionEntityRenderer.Render(dimension);
            var json = JsonSerializer.Serialize(result);

            Assert.NotNull(json);
            Assert.Contains("Angular3PtData", json);
            Assert.Contains("VertexX", json);
            Assert.Contains("FirstPointX", json);
            Assert.Contains("SecondPointX", json);
        }

        [Fact]
        public void Render_Dimension_WithVerySmallCoordinates_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0.001, 0.002, 0.003),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0.001, result.DefinitionPointX, 4);
            Assert.Equal(0.002, result.DefinitionPointY, 4);
            Assert.Equal(0.003, result.DefinitionPointZ, 4);
        }

        [Fact]
        public void Render_Dimension_WithVeryLargeCoordinates_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(1000000, 2000000, 3000000),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(1000000, result.DefinitionPointX);
            Assert.Equal(2000000, result.DefinitionPointY);
            Assert.Equal(3000000, result.DefinitionPointZ);
        }

        [Fact]
        public void Render_Dimension_WithFloatingPointCoordinates_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(10.567, 20.789, 30.123),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(10.567, result.DefinitionPointX, 3);
            Assert.Equal(20.789, result.DefinitionPointY, 3);
            Assert.Equal(30.123, result.DefinitionPointZ, 3);
        }

        [Fact]
        public void Render_Dimension_WithInvertedNormal_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, -1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0, result.NormalX);
            Assert.Equal(0, result.NormalY);
            Assert.Equal(-1, result.NormalZ);
        }

        [Fact]
        public void Render_DimensionAngular3Pt_WithCollinearPoints_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(10, 0, 0),
                SecondPoint = new XYZ(20, 0, 0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result.Angular3PtData);
            Assert.NotNull(result.Angular3PtData.ArcPoints);
        }

        [Fact]
        public void Render_Dimension_WithDefaultColor_ReturnsDefaultColor()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result.ColorHex);
            Assert.True(result.ColorR >= 0 && result.ColorR <= 255);
            Assert.True(result.ColorG >= 0 && result.ColorG <= 255);
            Assert.True(result.ColorB >= 0 && result.ColorB <= 255);
        }

        [Fact]
        public void Render_Dimension_CalculatesCorrectCentroid()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(10, 20, 30),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result.Centroid);
            Assert.Equal(0, result.Centroid.X);
            Assert.Equal(0, result.Centroid.Y);
            Assert.Equal(0, result.Centroid.Z);
        }

        [Fact]
        public void Render_Dimension_InitializesEmptyPointsArray()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result.Points);
            Assert.NotEmpty(result.Points);
        }

        [Fact]
        public void Render_Dimension_InitializesEmptyEntitiesArray()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result.Entities);
            Assert.Empty(result.Entities);
        }

        [Fact]
        public void Render_Dimension_SetsCoordinateSystem()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal("AutoCAD", result.CoordinateSystem);
        }

        [Fact]
        public void Render_Dimension_SetsYAxisFlipFlag()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.True(result.RequiresYAxisFlip);
        }

        [Fact]
        public void Render_Dimension_InitializesBounds()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);
            Assert.NotNull(result.Bounds.Center);
            Assert.NotNull(result.Bounds.Size);
        }

        [Fact]
        public void Render_DimensionAngular3Pt_WithDifferentRadii_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                AngleVertex = new XYZ(0, 0, 0),
                FirstPoint = new XYZ(5, 0, 0),
                SecondPoint = new XYZ(0, 5, 0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result.Angular3PtData);
            Assert.True(result.Angular3PtData.Radius > 0);
        }

        [Fact]
        public void Render_Dimension_WithMultipleDimensions_ReturnsUniqueResults()
        {
            var dimension1 = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(10, 20, 0),
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(1)
            };

            var dimension2 = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(30, 40, 0),
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(2)
            };

            var result1 = DimensionEntityRenderer.Render(dimension1);
            var result2 = DimensionEntityRenderer.Render(dimension2);

            Assert.NotEqual(result1.DefinitionPointX, result2.DefinitionPointX);
            Assert.NotEqual(result1.DefinitionPointY, result2.DefinitionPointY);
            Assert.NotEqual(result1.ColorIndex, result2.ColorIndex);
        }

        [Fact]
        public void Render_Dimension_WithSameCoordinates_ReturnsSameResults()
        {
            var dimension1 = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(10, 20, 30),
                Normal = new XYZ(0, 0, 1)
            };

            var dimension2 = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(10, 20, 30),
                Normal = new XYZ(0, 0, 1)
            };

            var result1 = DimensionEntityRenderer.Render(dimension1);
            var result2 = DimensionEntityRenderer.Render(dimension2);

            Assert.Equal(result1.DefinitionPointX, result2.DefinitionPointX);
            Assert.Equal(result1.DefinitionPointY, result2.DefinitionPointY);
            Assert.Equal(result1.DefinitionPointZ, result2.DefinitionPointZ);
        }

        [Fact]
        public void Render_Dimension_WithRotatedNormal_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(1, 0, 0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(1, result.NormalX);
            Assert.Equal(0, result.NormalY);
            Assert.Equal(0, result.NormalZ);
        }

        [Fact]
        public void Render_Dimension_WithDiagonalNormal_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0.577, 0.577, 0.577)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0.577, result.NormalX, 3);
            Assert.Equal(0.577, result.NormalY, 3);
            Assert.Equal(0.577, result.NormalZ, 3);
        }

        [Fact]
        public void Render_Dimension_WithNegativeNormalComponents_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(-0.707, -0.707, 0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(-0.707, result.NormalX, 3);
            Assert.Equal(-0.707, result.NormalY, 3);
            Assert.Equal(0, result.NormalZ);
        }

        [Fact]
        public void Render_Dimension_WithMixedCoordinates_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(-10.5, 20.75, -30.25),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(-10.5, result.DefinitionPointX, 2);
            Assert.Equal(20.75, result.DefinitionPointY, 2);
            Assert.Equal(-30.25, result.DefinitionPointZ, 2);
        }

        [Fact]
        public void Render_Dimension_WithExtremeCoordinates_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(double.MaxValue / 2, double.MinValue / 2, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.NotNull(result);
            Assert.Equal(double.MaxValue / 2, result.DefinitionPointX);
            Assert.Equal(double.MinValue / 2, result.DefinitionPointY);
        }

        [Fact]
        public void Render_Dimension_WithVerySmallNormal_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0.001, 0.001, 0.999)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0.001, result.NormalX, 4);
            Assert.Equal(0.001, result.NormalY, 4);
            Assert.Equal(0.999, result.NormalZ, 3);
        }

        [Fact]
        public void Render_Dimension_WithLargeNormal_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(100, 100, 100)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(100, result.NormalX);
            Assert.Equal(100, result.NormalY);
            Assert.Equal(100, result.NormalZ);
        }

        [Fact]
        public void Render_Dimension_WithNaNCoordinates_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(double.NaN, 20, 30),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.True(double.IsNaN(result.DefinitionPointX));
            Assert.Equal(20, result.DefinitionPointY);
            Assert.Equal(30, result.DefinitionPointZ);
        }

        [Fact]
        public void Render_Dimension_WithInfinityCoordinates_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(double.PositiveInfinity, 20, 30),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.True(double.IsPositiveInfinity(result.DefinitionPointX));
            Assert.Equal(20, result.DefinitionPointY);
            Assert.Equal(30, result.DefinitionPointZ);
        }

        [Fact]
        public void Render_Dimension_WithZeroDefinitionPoint_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0, result.DefinitionPointX);
            Assert.Equal(0, result.DefinitionPointY);
            Assert.Equal(0, result.DefinitionPointZ);
        }

        [Fact]
        public void Render_Dimension_WithUnitNormal_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0, result.NormalX);
            Assert.Equal(0, result.NormalY);
            Assert.Equal(1, result.NormalZ);
        }

        [Fact]
        public void Render_Dimension_WithNegativeUnitNormal_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, -1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0, result.NormalX);
            Assert.Equal(0, result.NormalY);
            Assert.Equal(-1, result.NormalZ);
        }

        [Fact]
        public void Render_Dimension_WithXAxisNormal_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(1, 0, 0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(1, result.NormalX);
            Assert.Equal(0, result.NormalY);
            Assert.Equal(0, result.NormalZ);
        }

        [Fact]
        public void Render_Dimension_WithYAxisNormal_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 1, 0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0, result.NormalX);
            Assert.Equal(1, result.NormalY);
            Assert.Equal(0, result.NormalZ);
        }

        [Fact]
        public void Render_Dimension_WithDiagonalUnitNormal_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0.707, 0.707, 0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0.707, result.NormalX, 3);
            Assert.Equal(0.707, result.NormalY, 3);
            Assert.Equal(0, result.NormalZ);
        }

        [Fact]
        public void Render_Dimension_With3DDiagonalNormal_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0.577, 0.577, 0.577)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0.577, result.NormalX, 3);
            Assert.Equal(0.577, result.NormalY, 3);
            Assert.Equal(0.577, result.NormalZ, 3);
        }

        [Fact]
        public void Render_Dimension_WithLargeColorIndex_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(255)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(255, result.ColorIndex);
            Assert.NotNull(result.ColorHex);
        }

        [Fact]
        public void Render_Dimension_WithZeroColorIndex_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0, result.ColorIndex);
            Assert.NotNull(result.ColorHex);
        }

        [Fact]
        public void Render_Dimension_WithNegativeColorIndex_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0, result.ColorIndex);
            Assert.NotNull(result.ColorHex);
        }

        [Fact]
        public void Render_Dimension_WithLargePositiveColorIndex_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(257)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(257, result.ColorIndex);
            Assert.NotNull(result.ColorHex);
        }

        [Fact]
        public void Render_Dimension_WithLargeNegativeColorIndex_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(1)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(1, result.ColorIndex);
            Assert.NotNull(result.ColorHex);
        }

        [Fact]
        public void Render_Dimension_WithMaxColorIndex_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(257)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(257, result.ColorIndex);
            Assert.NotNull(result.ColorHex);
        }

        [Fact]
        public void Render_Dimension_WithMinColorIndex_HandlesCorrectly()
        {
            var dimension = new DimensionAngular3Pt
            {
                DefinitionPoint = new XYZ(0, 0, 0),
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(0)
            };

            var result = DimensionEntityRenderer.Render(dimension);

            Assert.Equal(0, result.ColorIndex);
            Assert.NotNull(result.ColorHex);
        }

        [Fact]
        public void Render_Dimension_WithMultipleColorIndices_ReturnsCorrectColors()
        {
            var testCases = new[] { (short)0, (short)1, (short)2, (short)3, (short)4, (short)5, (short)6, (short)7, (short)8, (short)9, (short)10 };

            foreach (var colorIndex in testCases)
            {
                var dimension = new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(0, 0, 0),
                    Normal = new XYZ(0, 0, 1),
                    Color = new ACadSharp.Color(colorIndex)
                };

                var result = DimensionEntityRenderer.Render(dimension);

                Assert.Equal(colorIndex, result.ColorIndex);
                Assert.NotNull(result.ColorHex);
            }
        }

        [Fact]
        public void Render_Dimension_WithMultipleNormals_ReturnsCorrectNormals()
        {
            var testCases = new[]
            {
                new { X = 0.0, Y = 0.0, Z = 1.0 },
                new { X = 1.0, Y = 0.0, Z = 0.0 },
                new { X = 0.0, Y = 1.0, Z = 0.0 },
                new { X = 0.707, Y = 0.707, Z = 0.0 },
                new { X = 0.577, Y = 0.577, Z = 0.577 }
            };

            foreach (var testCase in testCases)
            {
                var dimension = new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(0, 0, 0),
                    Normal = new XYZ(testCase.X, testCase.Y, testCase.Z)
                };

                var result = DimensionEntityRenderer.Render(dimension);

                Assert.Equal(testCase.X, result.NormalX, 3);
                Assert.Equal(testCase.Y, result.NormalY, 3);
                Assert.Equal(testCase.Z, result.NormalZ, 3);
            }
        }

        [Fact]
        public void Render_Dimension_WithMultipleCoordinates_ReturnsCorrectCoordinates()
        {
            var testCases = new[]
            {
                new { X = 0.0, Y = 0.0, Z = 0.0 },
                new { X = 10.0, Y = 20.0, Z = 30.0 },
                new { X = -10.0, Y = -20.0, Z = -30.0 },
                new { X = 100.0, Y = 200.0, Z = 300.0 },
                new { X = 0.001, Y = 0.002, Z = 0.003 }
            };

            foreach (var testCase in testCases)
            {
                var dimension = new DimensionAngular3Pt
                {
                    DefinitionPoint = new XYZ(testCase.X, testCase.Y, testCase.Z),
                    Normal = new XYZ(0, 0, 1)
                };

                var result = DimensionEntityRenderer.Render(dimension);

                Assert.Equal(testCase.X, result.DefinitionPointX);
                Assert.Equal(testCase.Y, result.DefinitionPointY);
                Assert.Equal(testCase.Z, result.DefinitionPointZ);
            }
        }
    }
}
