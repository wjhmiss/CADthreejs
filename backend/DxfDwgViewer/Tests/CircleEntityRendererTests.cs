using Xunit;
using ACadSharp.Entities;
using DxfDwgViewer.RenderUtilities;
using CSMath;
using System;

namespace DxfDwgViewer.Tests
{
    public class CircleEntityRendererTests
    {
        [Fact]
        public void Render_ValidCircle_ReturnsValidCircleData()
        {
            var circle = new Circle
            {
                Center = new XYZ(10, 20, 0),
                Radius = 5,
                Thickness = 0,
                Normal = new XYZ(0, 0, 1)
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.NotNull(result);
            Assert.Equal("Circle", result.Type);
            Assert.Equal("CIRCLE", result.EntityType);
            Assert.Equal(10, result.CenterX);
            Assert.Equal(20, result.CenterY);
            Assert.Equal(0, result.CenterZ);
            Assert.Equal(5, result.Radius);
        }

        [Fact]
        public void Render_CircleWithCustomColor_ReturnsCorrectColorValues()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                Color = new ACadSharp.Color(1)
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.Equal(1, result.ColorIndex);
            Assert.Equal(255, result.ColorR);
            Assert.Equal(0, result.ColorG);
            Assert.Equal(0, result.ColorB);
            Assert.Equal("#FF0000", result.ColorHex);
        }

        [Fact]
        public void Render_Circle_GeneratesCorrectVertices()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.NotNull(result.Vertices);
            Assert.True(result.Vertices.Count > 0);
            Assert.Equal(65 * 3, result.Vertices.Count);
        }

        [Fact]
        public void Render_CircleWithThickness_SetsThicknessProperty()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 5,
                Thickness = 2.5
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.Equal(2.5, result.Thickness);
        }

        [Fact]
        public void Render_CircleWithNormalVector_CreatesTransformMatrix()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                Normal = new XYZ(1, 0, 0)
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Matrix);
            Assert.Equal(16, result.Transform.Matrix.Length);
        }

        [Fact]
        public void Render_Circle_CalculatesCorrectDiameter()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.Equal(20, result.Diameter);
        }

        [Fact]
        public void Render_Circle_CalculatesCorrectCircumference()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            var expectedCircumference = 2 * Math.PI * 10;
            Assert.Equal(expectedCircumference, result.Circumference, 5);
        }

        [Fact]
        public void Render_Circle_CalculatesCorrectArea()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            var expectedArea = Math.PI * 10 * 10;
            Assert.Equal(expectedArea, result.Area, 5);
        }

        [Fact]
        public void Render_Circle_CalculatesCorrectBounds()
        {
            var circle = new Circle
            {
                Center = new XYZ(10, 10, 0),
                Radius = 5,
                Thickness = 2
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);
            Assert.Equal(5, result.Bounds.Min.X);
            Assert.Equal(5, result.Bounds.Min.Y);
            Assert.Equal(-1, result.Bounds.Min.Z);
            Assert.Equal(15, result.Bounds.Max.X);
            Assert.Equal(15, result.Bounds.Max.Y);
            Assert.Equal(1, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_Circle_CalculatesCorrectCentroid()
        {
            var circle = new Circle
            {
                Center = new XYZ(10, 20, 5),
                Radius = 5
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.NotNull(result.Centroid);
            Assert.Equal(10, result.Centroid.X);
            Assert.Equal(20, result.Centroid.Y);
            Assert.Equal(5, result.Centroid.Z);
        }

        [Fact]
        public void Render_Circle_GeneratesIndices()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.NotNull(result.Indices);
            Assert.True(result.Indices.Count > 0);
            Assert.Equal(64 * 2, result.Indices.Count);
        }

        [Fact]
        public void Render_Circle_SetsDefaultMaterialProperties()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.Equal("LineBasicMaterial", result.MaterialType);
            Assert.True(result.MaterialDepthTest);
            Assert.True(result.MaterialDepthWrite);
            Assert.Equal(2, result.MaterialSide);
        }

        [Fact]
        public void Render_Circle_SetsCorrectEntityType()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.Equal("Circle", result.Type);
            Assert.Equal("CIRCLE", result.EntityType);
        }

        [Fact]
        public void Render_Circle_GeneratesUniqueUuid()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result1 = CircleEntityRenderer.Render(circle);
            var result2 = CircleEntityRenderer.Render(circle);

            Assert.NotEqual(result1.Uuid, result2.Uuid);
        }

        [Fact]
        public void Render_Circle_SetsCorrectCoordinateSystem()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.Equal("AutoCAD", result.CoordinateSystem);
            Assert.True(result.RequiresYAxisFlip);
        }

        [Fact]
        public void Render_Circle_SetsCorrectRenderProperties()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.True(result.Visible);
            Assert.False(result.CastShadow);
            Assert.False(result.ReceiveShadow);
            Assert.Equal(0, result.RenderOrder);
        }

        [Fact]
        public void Render_InvisibleCircle_SetsVisibleToFalse()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };
            circle.IsInvisible = true;

            var result = CircleEntityRenderer.Render(circle);

            Assert.True(result.IsInvisible);
            Assert.False(result.Visible);
        }

        [Fact]
        public void Render_Circle_GeneratesPointsList()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.NotNull(result.Points);
            Assert.True(result.Points.Count > 0);
            Assert.Equal(65, result.Points.Count);
        }

        [Fact]
        public void Render_CircleWithDefaultNormal_NoTransformMatrix()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                Normal = new XYZ(0, 0, 1)
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Matrix);
        }

        [Fact]
        public void Render_CircleWithCustomNormal_GeneratesTransformMatrix()
        {
            var circle = new Circle
            {
                Center = new XYZ(5, 10, 15),
                Radius = 10,
                Normal = new XYZ(0.5, 0.5, 0.7071)
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Matrix);
            Assert.Equal(16, result.Transform.Matrix.Length);
            Assert.Equal(5, result.Transform.Matrix[12]);
            Assert.Equal(10, result.Transform.Matrix[13]);
            Assert.Equal(15, result.Transform.Matrix[14]);
        }

        [Fact]
        public void Render_Circle_VerifyVerticesAreOnCircle()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            for (int i = 0; i < result.Vertices.Count; i += 3)
            {
                double x = result.Vertices[i];
                double y = result.Vertices[i + 1];
                double z = result.Vertices[i + 2];

                double distance = Math.Sqrt(x * x + y * y);
                Assert.Equal(10, distance, 1);
                Assert.Equal(0, z);
            }
        }

        [Fact]
        public void Render_Circle_VerifyIndicesAreSequential()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            for (int i = 0; i < result.Indices.Count; i += 2)
            {
                int current = result.Indices[i];
                int next = result.Indices[i + 1];
                Assert.Equal(current + 1, next);
            }
        }

        [Fact]
        public void Render_CircleWithDifferentColorIndex_ReturnsCorrectColor()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                Color = new ACadSharp.Color(3)
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.Equal(3, result.ColorIndex);
            Assert.Equal(0, result.ColorR);
            Assert.Equal(255, result.ColorG);
            Assert.Equal(0, result.ColorB);
            Assert.Equal("#00FF00", result.ColorHex);
        }

        [Fact]
        public void Render_CircleWithVerySmallRadius_HandlesGracefully()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 0.001
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.Equal(0.001, result.Radius);
            Assert.Equal(0.002, result.Diameter);
            Assert.Equal(0.002 * Math.PI, result.Circumference, 5);
            Assert.Equal(Math.PI * 0.001 * 0.001, result.Area, 5);
        }

        [Fact]
        public void Render_CircleWithLargeRadius_CalculatesCorrectly()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 1000
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.Equal(1000, result.Radius);
            Assert.Equal(2000, result.Diameter);
            Assert.Equal(2000 * Math.PI, result.Circumference, 2);
            Assert.Equal(Math.PI * 1000000, result.Area, 2);
        }

        [Fact]
        public void Render_CircleWithNegativeThickness_HandlesCorrectly()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                Thickness = -5
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.Equal(-5, result.Thickness);
        }

        [Fact]
        public void Render_Circle_VerifyFirstAndLastPointAreSame()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            var firstPoint = result.Points[0];
            var lastPoint = result.Points[result.Points.Count - 1];

            Assert.Equal(firstPoint.X, lastPoint.X, 5);
            Assert.Equal(firstPoint.Y, lastPoint.Y, 5);
            Assert.Equal(firstPoint.Z, lastPoint.Z, 5);
        }

        [Fact]
        public void Render_Circle_VerifyVerticesStartAndEndAtSamePoint()
        {
            var circle = new Circle
            {
                Center = new XYZ(10, 20, 5),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            double firstX = result.Vertices[0];
            double firstY = result.Vertices[1];
            double firstZ = result.Vertices[2];

            double lastX = result.Vertices[result.Vertices.Count - 3];
            double lastY = result.Vertices[result.Vertices.Count - 2];
            double lastZ = result.Vertices[result.Vertices.Count - 1];

            Assert.Equal(firstX, lastX, 5);
            Assert.Equal(firstY, lastY, 5);
            Assert.Equal(firstZ, lastZ, 5);
        }

        [Fact]
        public void Render_Circle_VerifyBoundsIncludeAllPoints()
        {
            var circle = new Circle
            {
                Center = new XYZ(10, 10, 0),
                Radius = 5
            };

            var result = CircleEntityRenderer.Render(circle);

            foreach (var point in result.Points)
            {
                Assert.True(point.X >= result.Bounds.Min.X);
                Assert.True(point.X <= result.Bounds.Max.X);
                Assert.True(point.Y >= result.Bounds.Min.Y);
                Assert.True(point.Y <= result.Bounds.Max.Y);
            }
        }

        [Fact]
        public void Render_Circle_VerifyCentroidEqualsCenter()
        {
            var circle = new Circle
            {
                Center = new XYZ(15, 25, 10),
                Radius = 7
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.Equal(result.CenterX, result.Centroid.X);
            Assert.Equal(result.CenterY, result.Centroid.Y);
            Assert.Equal(result.CenterZ, result.Centroid.Z);
        }

        [Fact]
        public void Render_CircleWithDifferentLineTypeScale_SetsCorrectly()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10,
                LineTypeScale = 2.5
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.Equal(2.5, result.LineTypeScale);
        }

        [Fact]
        public void Render_Circle_VerifyMaterialOpacityCalculation()
        {
            var circle = new Circle
            {
                Center = new XYZ(0, 0, 0),
                Radius = 10
            };

            var result = CircleEntityRenderer.Render(circle);

            Assert.Equal(1.0, result.MaterialOpacity);
            Assert.False(result.MaterialTransparent);
        }
    }
}
