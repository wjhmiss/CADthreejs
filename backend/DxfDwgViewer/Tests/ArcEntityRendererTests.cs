using Xunit;
using ACadSharp.Entities;
using DxfDwgViewer.RenderUtilities;
using CSMath;
using System;

namespace DxfDwgViewer.Tests
{
    public class ArcEntityRendererTests
    {
        [Fact]
        public void Render_ValidArc_ReturnsValidArcData()
        {
            var arc = new Arc
            {
                Center = new XYZ(10, 20, 0),
                Radius = 5,
                StartAngle = 0,
                EndAngle = Math.PI / 2,
                Thickness = 0,
                Normal = new XYZ(0, 0, 1)
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.NotNull(result);
            Assert.Equal("Arc", result.Type);
            Assert.Equal("ARC", result.EntityType);
            Assert.Equal(10, result.CenterX);
            Assert.Equal(20, result.CenterY);
            Assert.Equal(0, result.CenterZ);
            Assert.Equal(5, result.Radius);
            Assert.Equal(0, result.StartAngle);
            Assert.Equal(Math.PI / 2, result.EndAngle);
        }

        [Fact]
        public void Render_ArcWithCustomColor_ReturnsCorrectColorValues()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI,
                Color = new ACadSharp.Color(1)
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.Equal(1, result.ColorIndex);
            Assert.Equal(255, result.ColorR);
            Assert.Equal(0, result.ColorG);
            Assert.Equal(0, result.ColorB);
            Assert.Equal("#FF0000", result.ColorHex);
        }

        [Fact]
        public void Render_FullCircleArc_CalculatesCorrectVertices()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = 2 * Math.PI
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.NotNull(result.Vertices);
            Assert.True(result.Vertices.Count > 0);
            Assert.Equal(65 * 3, result.Vertices.Count);
        }

        [Fact]
        public void Render_ArcWithThickness_SetsThicknessProperty()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 5,
                StartAngle = 0,
                EndAngle = Math.PI / 4,
                Thickness = 2.5
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.Equal(2.5, result.Thickness);
        }

        [Fact]
        public void Render_ArcWithNormalVector_CreatesTransformMatrix()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI / 2,
                Normal = new XYZ(1, 0, 0)
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Matrix);
            Assert.Equal(16, result.Transform.Matrix.Length);
        }

        [Fact]
        public void Render_Arc_CalculatesCorrectStartAndEndPoints()
        {
            var arc = new Arc
            {
                Center = new XYZ(10, 10, 0),
                Radius = 5,
                StartAngle = 0,
                EndAngle = Math.PI / 2
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.Equal(15, result.StartX);
            Assert.Equal(10, result.StartY);
            Assert.Equal(10, result.EndX);
            Assert.Equal(15, result.EndY);
        }

        [Fact]
        public void Render_Arc_CalculatesCorrectArcLength()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI
            };

            var result = ArcEntityRenderer.Render(arc);

            var expectedLength = 10 * Math.PI;
            Assert.Equal(expectedLength, result.Length, 5);
        }

        [Fact]
        public void Render_Arc_CalculatesCorrectChordLength()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI
            };

            var result = ArcEntityRenderer.Render(arc);

            var expectedChordLength = 20;
            Assert.Equal(expectedChordLength, result.ChordLength, 5);
        }

        [Fact]
        public void Render_Arc_CalculatesCorrectSagitta()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.Equal(10, result.Sagitta, 5);
        }

        [Fact]
        public void Render_Arc_CalculatesCorrectMidPoint()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI / 2
            };

            var result = ArcEntityRenderer.Render(arc);

            var expectedMidX = 10 * Math.Cos(Math.PI / 4);
            var expectedMidY = 10 * Math.Sin(Math.PI / 4);
            Assert.Equal(expectedMidX, result.MidX, 5);
            Assert.Equal(expectedMidY, result.MidY, 5);
        }

        [Fact]
        public void Render_Arc_CalculatesCorrectBounds()
        {
            var arc = new Arc
            {
                Center = new XYZ(10, 10, 0),
                Radius = 5,
                StartAngle = 0,
                EndAngle = Math.PI / 2
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);
            Assert.True(result.Bounds.Min.X <= 10);
            Assert.True(result.Bounds.Max.X >= 15);
            Assert.True(result.Bounds.Min.Y <= 10);
            Assert.True(result.Bounds.Max.Y >= 15);
        }

        [Fact]
        public void Render_Arc_CalculatesCorrectCentroid()
        {
            var arc = new Arc
            {
                Center = new XYZ(10, 10, 0),
                Radius = 5,
                StartAngle = 0,
                EndAngle = Math.PI / 2
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.NotNull(result.Centroid);
            Assert.Equal(result.MidX, result.Centroid.X, 5);
            Assert.Equal(result.MidY, result.Centroid.Y, 5);
            Assert.Equal(result.MidZ, result.Centroid.Z, 5);
        }

        [Fact]
        public void Render_Arc_GeneratesIndices()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI / 2
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.NotNull(result.Indices);
            Assert.True(result.Indices.Count > 0);
            Assert.Equal(64 * 2, result.Indices.Count);
        }

        [Fact]
        public void Render_Arc_SetsDefaultMaterialProperties()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.Equal("LineBasicMaterial", result.MaterialType);
            Assert.True(result.MaterialDepthTest);
            Assert.True(result.MaterialDepthWrite);
            Assert.Equal(2, result.MaterialSide);
        }

        [Fact]
        public void Render_Arc_SetsCorrectEntityType()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.Equal("Arc", result.Type);
            Assert.Equal("ARC", result.EntityType);
        }

        [Fact]
        public void Render_Arc_GeneratesUniqueUuid()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI
            };

            var result1 = ArcEntityRenderer.Render(arc);
            var result2 = ArcEntityRenderer.Render(arc);

            Assert.NotEqual(result1.Uuid, result2.Uuid);
        }

        [Fact]
        public void Render_Arc_CalculatesCorrectSectorArea()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI
            };

            var result = ArcEntityRenderer.Render(arc);

            var expectedArea = 0.5 * 10 * 10 * Math.PI;
            Assert.Equal(expectedArea, result.Area, 5);
        }

        [Fact]
        public void Render_Arc_SetsCorrectCoordinateSystem()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.Equal("AutoCAD", result.CoordinateSystem);
            Assert.True(result.RequiresYAxisFlip);
        }

        [Fact]
        public void Render_Arc_SetsCorrectRenderProperties()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.True(result.Visible);
            Assert.False(result.CastShadow);
            Assert.False(result.ReceiveShadow);
            Assert.Equal(0, result.RenderOrder);
        }

        [Fact]
        public void Render_InvisibleArc_SetsVisibleToFalse()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI
            };
            arc.IsInvisible = true;

            var result = ArcEntityRenderer.Render(arc);

            Assert.True(result.IsInvisible);
            Assert.False(result.Visible);
        }

        [Fact]
        public void Render_Arc_GeneratesPointsList()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI / 2
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.NotNull(result.Points);
            Assert.True(result.Points.Count > 0);
            Assert.Equal(65, result.Points.Count);
        }

        [Fact]
        public void Render_Arc_CalculatesIsCCW()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI / 2
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.True(result.IsCCW);
        }

        [Fact]
        public void Render_Arc_CalculatesSweep()
        {
            var arc = new Arc
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                StartAngle = 0,
                EndAngle = Math.PI / 2
            };

            var result = ArcEntityRenderer.Render(arc);

            Assert.Equal(-Math.PI / 2, result.Sweep, 5);
        }
    }
}
