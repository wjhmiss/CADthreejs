using Xunit;
using ACadSharp.Entities;
using DxfDwgViewer.RenderUtilities;
using CSMath;
using System;

namespace DxfDwgViewer.Tests
{
    public class EllipseEntityRendererTests
    {
        [Fact]
        public void Render_ValidEllipse_ReturnsValidEllipseData()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(10, 20, 0),
                MajorAxisEndPoint = new XYZ(15, 20, 0),
                RadiusRatio = 0.6,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Thickness = 0,
                Normal = new XYZ(0, 0, 1)
            };

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.Equal("Ellipse", result.Type);
            Assert.Equal("ELLIPSE", result.EntityType);
            Assert.Equal(10, result.CenterX);
            Assert.Equal(20, result.CenterY);
            Assert.Equal(0, result.CenterZ);
            Assert.Equal(15, result.MajorAxisEndPointX);
            Assert.Equal(20, result.MajorAxisEndPointY);
            Assert.Equal(0, result.MajorAxisEndPointZ);
            Assert.Equal(0.6, result.RadiusRatio);
            Assert.Equal(0, result.StartParameter);
            Assert.Equal(2 * Math.PI, result.EndParameter);
        }

        [Fact]
        public void Render_EllipseWithCustomColor_ReturnsCorrectColorValues()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Color = new ACadSharp.Color(1)
            };

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.Equal(1, result.ColorIndex);
            Assert.NotNull(result.ColorHex);
            Assert.Equal(255, result.ColorR);
            Assert.Equal(0, result.ColorG);
            Assert.Equal(0, result.ColorB);
        }

        [Fact]
        public void Render_EllipseWithDifferentColors_ReturnsCorrectColorIndex()
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

                var result = EllipseEntityRenderer.Render(ellipse);

                Assert.NotNull(result);
                Assert.Equal(colorIndex, result.ColorIndex);
                Assert.NotNull(result.ColorHex);
            }
        }

        [Fact]
        public void Render_EllipseWithMaxColorIndex_HandlesCorrectly()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Color = new ACadSharp.Color(257)
            };

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.Equal(257, result.ColorIndex);
            Assert.NotNull(result.ColorHex);
        }

        [Fact]
        public void Render_FullEllipse_SetsIsFullEllipseTrue()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.True(result.IsFullEllipse);
        }

        [Fact]
        public void Render_EllipseArc_SetsIsFullEllipseFalse()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = Math.PI,
                Normal = new XYZ(0, 0, 1)
            };

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.False(result.IsFullEllipse);
        }

        [Fact]
        public void Render_Ellipse_CalculatesCorrectMajorRadius()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.Equal(10, result.MajorRadius);
        }

        [Fact]
        public void Render_Ellipse_CalculatesCorrectMinorRadius()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.Equal(5, result.MinorRadius);
        }

        [Fact]
        public void Render_Ellipse_CalculatesCorrectRotationAngle()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 10, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Normal = new XYZ(0, 0, 1)
            };

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 4, result.RotationAngle);
        }

        [Fact]
        public void Render_Ellipse_CalculatesCorrectArea()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            double expectedArea = Math.PI * 10 * 5;
            Assert.Equal(expectedArea, result.Area, 4);
        }

        [Fact]
        public void Render_Ellipse_CalculatesCorrectLength()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }

        [Fact]
        public void Render_EllipseWithCustomNormal_SetsCorrectNormal()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Normal = new XYZ(1, 0, 0)
            };

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.Equal(1, result.NormalX);
            Assert.Equal(0, result.NormalY);
            Assert.Equal(0, result.NormalZ);
        }

        [Fact]
        public void Render_Ellipse_GeneratesPointsArray()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.NotNull(result.Points);
            Assert.NotEmpty(result.Points);
        }

        [Fact]
        public void Render_Ellipse_GeneratesVerticesArray()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices);
            Assert.NotEmpty(result.Vertices);
        }

        [Fact]
        public void Render_Ellipse_GeneratesIndicesArray()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.NotNull(result.Indices);
            Assert.NotEmpty(result.Indices);
        }

        [Fact]
        public void Render_Ellipse_CalculatesBounds()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);
        }

        [Fact]
        public void Render_Ellipse_CalculatesCentroid()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);
            Assert.Equal(10.23, result.Centroid.X, 2);
            Assert.Equal(20.31, result.Centroid.Y, 2);
            Assert.Equal(5, result.Centroid.Z, 2);
        }

        [Fact]
        public void Render_EllipseWithThickness_SetsThickness()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                Thickness = 2.5,
                Normal = new XYZ(0, 0, 1)
            };

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.Equal(2.5, result.Thickness);
        }

        [Fact]
        public void Render_EllipseWithInvisible_SetsInvisible()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = 2 * Math.PI,
                IsInvisible = true,
                Normal = new XYZ(0, 0, 1)
            };

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.True(result.IsInvisible);
            Assert.False(result.Visible);
        }

        [Fact]
        public void Render_Ellipse_SetsCorrectEntityType()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.Equal("ELLIPSE", result.EntityType);
        }

        [Fact]
        public void Render_Ellipse_GeneratesUuid()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.NotNull(result.Uuid);
            Assert.NotEmpty(result.Uuid);
        }

        [Fact]
        public void Render_Ellipse_SetsCoordinateSystem()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.Equal("AutoCAD", result.CoordinateSystem);
        }

        [Fact]
        public void Render_Ellipse_SetsRequiresYAxisFlip()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.True(result.RequiresYAxisFlip);
        }

        [Fact]
        public void Render_Ellipse_SetsMaterialProperties()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.Equal("LineBasicMaterial", result.MaterialType);
            Assert.True(result.MaterialDepthTest);
            Assert.True(result.MaterialDepthWrite);
            Assert.Equal(2, result.MaterialSide);
        }

        [Fact]
        public void Render_EllipseWithTransparency_SetsOpacity()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.True(result.MaterialOpacity > 0);
            Assert.True(result.MaterialOpacity <= 1);
        }

        [Fact]
        public void Render_EllipseArc_CalculatesCorrectLength()
        {
            var ellipse = new Ellipse
            {
                Center = new XYZ(0, 0, 0),
                MajorAxisEndPoint = new XYZ(10, 0, 0),
                RadiusRatio = 0.5,
                StartParameter = 0,
                EndParameter = Math.PI,
                Normal = new XYZ(0, 0, 1)
            };

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }

        [Fact]
        public void Render_Ellipse_SetsCorrectLineType()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.NotNull(result.LineTypeName);
        }

        [Fact]
        public void Render_Ellipse_SetsCorrectLineWeight()
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

            var result = EllipseEntityRenderer.Render(ellipse);

            Assert.NotNull(result);
            Assert.True(result.LineWeight >= 0);
        }
    }
}
