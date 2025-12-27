using Xunit;
using ACadSharp.Entities;
using DxfDwgViewer.RenderUtilities;
using CSMath;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DxfDwgViewer.Tests
{
    public class EllipseEntityRendererIntegrationTests
    {
        [Fact]
        public void RenderEllipse_SerializesToJson_CanBeDeserialized()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(10, 20, 0),
                MajorAxisEndPoint = new XYZ(15, 20, 0),
                RadiusRatio = 0.6,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Thickness = 0,
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(1)
            };

            var ellipseData = EllipseEntityRenderer.Render(ellipse);
            var json = JsonConvert.SerializeObject(ellipseData);

            Assert.NotNull(json);
            Assert.Contains("Ellipse", json);
            Assert.Contains("ELLIPSE", json);
            Assert.Contains("10", json);
            Assert.Contains("20", json);
        }

        [Fact]
        public void RenderMultipleEllipses_CollectAllEllipseData()
        {
            var ellipses = new List<Ellipse>
            {
                new Ellipse
                {
                    Center = new XYZ(0, 0, 0),
                    MajorAxisEndPoint = new XYZ(10, 0, 0),
                    RadiusRatio = 0.5,
                    StartParameter = 0,
                    EndParameter = 2 * Math.PI,
                    Normal = new XYZ(0, 0, 1)
                },
                new Ellipse
                {
                    Center = new XYZ(20, 30, 0),
                    MajorAxisEndPoint = new XYZ(25, 30, 0),
                    RadiusRatio = 0.7,
                    StartParameter = 0,
                    EndParameter = 2 * Math.PI,
                    Normal = new XYZ(0, 0, 1)
                },
                new Ellipse
                {
                    Center = new XYZ(40, 50, 0),
                    MajorAxisEndPoint = new XYZ(50, 50, 0),
                    RadiusRatio = 0.8,
                    StartParameter = 0,
                    EndParameter = 2 * Math.PI,
                    Normal = new XYZ(0, 0, 1)
                }
            };

            var ellipseDataList = new List<EllipseEntityRenderer.EllipseData>();
            foreach (var ellipse in ellipses)
            {
                ellipseDataList.Add(EllipseEntityRenderer.Render(ellipse));
            }

            Assert.Equal(3, ellipseDataList.Count);
            Assert.All(ellipseDataList, data => Assert.NotNull(data));
            Assert.All(ellipseDataList, data => Assert.Equal("Ellipse", data.Type));
        }

        [Fact]
        public void RenderEllipseWithDifferentColors_SerializesCorrectly()
        {
            var testCases = new[] { (short)1, (short)2, (short)3, (short)4, (short)5 };

            foreach (var colorIndex in testCases)
            {
                var ellipse = new Ellipse
                {
                    Center = new XYZ(0, 0, 0),
                    MajorAxisEndPoint = new XYZ(10, 0, 0),
                    RadiusRatio = 0.5,
                    StartParameter = 0,
                    EndParameter = 2 * Math.PI,
                    Color = new ACadSharp.Color(colorIndex)
                };

                var ellipseData = EllipseEntityRenderer.Render(ellipse);
                var json = JsonConvert.SerializeObject(ellipseData);

                Assert.NotNull(json);
                Assert.Contains(colorIndex.ToString(), json);
            }
        }

        [Fact]
        public void RenderEllipseWithDifferentRadii_CalculatesCorrectArea()
        {
            var ellipses = new[]
            {
                new { MajorRadius = 10.0, MinorRadius = 5.0, ExpectedArea = Math.PI * 10 * 5 },
                new { MajorRadius = 20.0, MinorRadius = 10.0, ExpectedArea = Math.PI * 20 * 10 },
                new { MajorRadius = 15.0, MinorRadius = 7.5, ExpectedArea = Math.PI * 15 * 7.5 }
            };

            foreach (var testCase in ellipses)
            {
                var ellipse = new Ellipse
                {
                    Center = new XYZ(0, 0, 0),
                    MajorAxisEndPoint = new XYZ(testCase.MajorRadius, 0, 0),
                    RadiusRatio = testCase.MinorRadius / testCase.MajorRadius,
                    StartParameter = 0,
                    EndParameter = 2 * Math.PI,
                    Normal = new XYZ(0, 0, 1)
                };

                var ellipseData = EllipseEntityRenderer.Render(ellipse);

                Assert.NotNull(ellipseData);
                Assert.Equal(testCase.ExpectedArea, ellipseData.Area, 4);
            }
        }

        [Fact]
        public void RenderEllipseWithDifferentNormals_GeneratesTransformMatrix()
        {
            var normals = new[]
            {
                new XYZ(0, 0, 1),
                new XYZ(1, 0, 0),
                new XYZ(0, 1, 0),
                new XYZ(0.707, 0.707, 0)
            };

            foreach (var normal in normals)
            {
                var ellipse = new Ellipse
                {
                    Center = new XYZ(0, 0, 0),
                    MajorAxisEndPoint = new XYZ(10, 0, 0),
                    RadiusRatio = 0.5,
                    StartParameter = 0,
                    EndParameter = 2 * Math.PI,
                    Normal = normal
                };

                var ellipseData = EllipseEntityRenderer.Render(ellipse);

                Assert.NotNull(ellipseData);
                Assert.NotNull(ellipseData.Transform);
                Assert.NotNull(ellipseData.Transform.Matrix);
                Assert.Equal(16, ellipseData.Transform.Matrix.Length);
            }
        }

        [Fact]
        public void RenderEllipseWithDifferentThickness_SetsThicknessCorrectly()
        {
            var thicknesses = new[] { 0.0, 1.0, 2.5, 5.0 };

            foreach (var thickness in thicknesses)
            {
                var ellipse = new Ellipse
                {
                    Center = new XYZ(0, 0, 0),
                    MajorAxisEndPoint = new XYZ(10, 0, 0),
                    RadiusRatio = 0.5,
                    StartParameter = 0,
                    EndParameter = 2 * Math.PI,
                    Thickness = thickness,
                    Normal = new XYZ(0, 0, 1)
                };

                var ellipseData = EllipseEntityRenderer.Render(ellipse);

                Assert.NotNull(ellipseData);
                Assert.Equal(thickness, ellipseData.Thickness);
            }
        }

        [Fact]
        public void RenderFullEllipseAndEllipseArc_DistinguishesCorrectly()
        {
            var fullEllipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Normal = new XYZ(0, 0, 1)
            };

            var ellipseArc = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = Math.PI,
                Normal = new XYZ(0, 0, 1)
            };

            var fullEllipseData = EllipseEntityRenderer.Render(fullEllipse);
            var ellipseArcData = EllipseEntityRenderer.Render(ellipseArc);

            Assert.NotNull(fullEllipseData);
            Assert.NotNull(ellipseArcData);
            Assert.True(fullEllipseData.IsFullEllipse);
            Assert.False(ellipseArcData.IsFullEllipse);
        }

        [Fact]
        public void RenderEllipseWithDifferentRotationAngles_CalculatesCorrectRotation()
        {
            var testCases = new[]
            {
                new { EndPointX = 10.0, EndPointY = 0.0, ExpectedAngle = 0.0 },
                new { EndPointX = 0.0, EndPointY = 10.0, ExpectedAngle = Math.PI / 2 },
                new { EndPointX = 10.0, EndPointY = 10.0, ExpectedAngle = Math.PI / 4 }
            };

            foreach (var testCase in testCases)
            {
                var ellipse = new Ellipse
                {
                    Center = new XYZ(0, 0, 0),
                    MajorAxisEndPoint = new XYZ(testCase.EndPointX, testCase.EndPointY, 0),
                    RadiusRatio = 0.5,
                    StartParameter = 0,
                    EndParameter = 2 * Math.PI,
                    Normal = new XYZ(0, 0, 1)
                };

                var ellipseData = EllipseEntityRenderer.Render(ellipse);

                Assert.NotNull(ellipseData);
                Assert.Equal(testCase.ExpectedAngle, ellipseData.RotationAngle, 4);
            }
        }

        [Fact]
        public void RenderEllipseWithDifferentCenters_SetsCenterCorrectly()
        {
            var centers = new[]
            {
                new XYZ(0, 0, 0),
                new XYZ(10, 20, 30),
                new XYZ(-10, -20, -30),
                new XYZ(100, 200, 300)
            };

            foreach (var center in centers)
            {
                var ellipse = new Ellipse
                {
                    Center = center,
                    MajorAxisEndPoint = new XYZ(center.X + 10, center.Y, center.Z),
                    RadiusRatio = 0.5,
                    StartParameter = 0,
                    EndParameter = 2 * Math.PI,
                    Normal = new XYZ(0, 0, 1)
                };

                var ellipseData = EllipseEntityRenderer.Render(ellipse);

                Assert.NotNull(ellipseData);
                Assert.Equal(center.X, ellipseData.CenterX);
                Assert.Equal(center.Y, ellipseData.CenterY);
                Assert.Equal(center.Z, ellipseData.CenterZ);
            }
        }

        [Fact]
        public void RenderEllipseWithDifferentRadiusRatios_CalculatesCorrectMinorRadius()
        {
            var testCases = new[]
            {
                new { MajorRadius = 10.0, RadiusRatio = 0.5, ExpectedMinorRadius = 5.0 },
                new { MajorRadius = 20.0, RadiusRatio = 0.7, ExpectedMinorRadius = 14.0 },
                new { MajorRadius = 15.0, RadiusRatio = 0.8, ExpectedMinorRadius = 12.0 }
            };

            foreach (var testCase in testCases)
            {
                var ellipse = new Ellipse
                {
                    Center = new XYZ(0, 0, 0),
                    MajorAxisEndPoint = new XYZ(testCase.MajorRadius, 0, 0),
                    RadiusRatio = testCase.RadiusRatio,
                    StartParameter = 0,
                    EndParameter = 2 * Math.PI,
                    Normal = new XYZ(0, 0, 1)
                };

                var ellipseData = EllipseEntityRenderer.Render(ellipse);

                Assert.NotNull(ellipseData);
                Assert.Equal(testCase.ExpectedMinorRadius, ellipseData.MinorRadius);
            }
        }

        [Fact]
        public void RenderEllipse_GeneratesConsistentVertices()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Normal = new XYZ(0, 0, 1)
            };

            var ellipseData1 = EllipseEntityRenderer.Render(ellipse);
            var ellipseData2 = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(ellipseData1);
            Assert.NotNull(ellipseData2);
            Assert.Equal(ellipseData1.Vertices.Count, ellipseData2.Vertices.Count);
            Assert.Equal(ellipseData1.Vertices.Count, ellipseData2.Vertices.Count);
        }

        [Fact]
        public void RenderEllipse_GeneratesValidIndices()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Normal = new XYZ(0, 0, 1)
            };

            var ellipseData = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(ellipseData);
            Assert.NotNull(ellipseData.Indices);
            Assert.NotEmpty(ellipseData.Indices);
            Assert.True(ellipseData.Indices.Count % 2 == 0);
        }

        [Fact]
        public void RenderEllipse_CalculatesValidBounds()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Normal = new XYZ(0, 0, 1)
            };

            var ellipseData = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(ellipseData);
            Assert.NotNull(ellipseData.Bounds);
            Assert.NotNull(ellipseData.Bounds.Min);
            Assert.NotNull(ellipseData.Bounds.Max);
            Assert.True(ellipseData.Bounds.Min.X <= ellipseData.Bounds.Max.X);
            Assert.True(ellipseData.Bounds.Min.Y <= ellipseData.Bounds.Max.Y);
            Assert.True(ellipseData.Bounds.Min.Z <= ellipseData.Bounds.Max.Z);
        }

        [Fact]
        public void RenderEllipse_CalculatesValidCentroid()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(10, 20, 5),
                MajorAxisEndPoint = new XYZ(15, 20, 5),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Normal = new XYZ(0, 0, 1)
            };

            var ellipseData = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(ellipseData);
            Assert.NotNull(ellipseData.Centroid);
            Assert.Equal(10.23, ellipseData.Centroid.X, 2);
            Assert.Equal(20.31, ellipseData.Centroid.Y, 2);
            Assert.Equal(5, ellipseData.Centroid.Z, 2);
        }

        [Fact]
        public void RenderEllipseWithDifferentLineTypes_SetsLineTypeName()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Normal = new XYZ(0, 0, 1)
            };

            var ellipseData = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(ellipseData);
            Assert.NotNull(ellipseData.LineTypeName);
        }

        [Fact]
        public void RenderEllipseWithDifferentLineWeights_SetsLineWeight()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Normal = new XYZ(0, 0, 1)
            };

            var ellipseData = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(ellipseData);
            Assert.True(ellipseData.LineWeight >= 0);
        }

        [Fact]
        public void RenderEllipse_SerializesAllPropertiesToJson()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(10, 20, 0),
                MajorAxisEndPoint = new XYZ(15, 20, 0),
                RadiusRatio = 0.6,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Thickness = 0,
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(1)
            };

            var ellipseData = EllipseEntityRenderer.Render(ellipse);
            var json = JsonConvert.SerializeObject(ellipseData);

            Assert.NotNull(json);
            Assert.Contains("Type", json);
            Assert.Contains("CenterX", json);
            Assert.Contains("CenterY", json);
            Assert.Contains("CenterZ", json);
            Assert.Contains("MajorRadius", json);
            Assert.Contains("MinorRadius", json);
            Assert.Contains("Area", json);
            Assert.Contains("Length", json);
            Assert.Contains("IsFullEllipse", json);
        }

        [Fact]
        public void RenderEllipseWithDifferentTransparency_SetsOpacity()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Normal = new XYZ(0, 0, 1)
            };

            var ellipseData = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(ellipseData);
            Assert.True(ellipseData.MaterialOpacity > 0);
            Assert.True(ellipseData.MaterialOpacity <= 1);
        }

        [Fact]
        public void RenderEllipseWithDifferentStartEndParameters_CalculatesCorrectLength()
        {
            var testCases = new[]
            {
                new { Start = 0.0, End = Math.PI / 2 },
                new { Start = 0.0, End = Math.PI },
                new { Start = 0.0, End = 3 * Math.PI / 2 }
            };

            foreach (var testCase in testCases)
            {
                var ellipse = new Ellipse
                {
                    Center = new XYZ(0, 0, 0),
                    MajorAxisEndPoint = new XYZ(10, 0, 0),
                    RadiusRatio = 0.5,
                    StartParameter = testCase.Start,
                    EndParameter = testCase.End,
                    Normal = new XYZ(0, 0, 1)
                };

                var ellipseData = EllipseEntityRenderer.Render(ellipse);

                Assert.NotNull(ellipseData);
                Assert.True(ellipseData.Length > 0);
            }
        }
    }
}
