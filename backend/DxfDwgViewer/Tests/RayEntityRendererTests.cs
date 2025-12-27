using System;
using Xunit;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using DxfDwgViewer.RenderUtilities;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class RayEntityRendererTests
    {
        private Ray CreateBasicRay()
        {
            var ray = new Ray
            {
                StartPoint = new XYZ(0, 0, 0),
                Direction = new XYZ(1, 0, 0),
                Color = new Color(1)
            };
            return ray;
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectType()
        {
            var ray = CreateBasicRay();
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.Equal("Ray", result.EntityType);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectStartPoint()
        {
            var ray = CreateBasicRay();
            ray.StartPoint = new XYZ(50, 100, 20);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.Equal(50, result.StartPoint.X);
            Assert.Equal(100, result.StartPoint.Y);
            Assert.Equal(20, result.StartPoint.Z);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectDirection()
        {
            var ray = CreateBasicRay();
            ray.Direction = new XYZ(0, 1, 0);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.Equal(0, result.Direction.X);
            Assert.Equal(1, result.Direction.Y);
            Assert.Equal(0, result.Direction.Z);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectColorIndex()
        {
            var ray = CreateBasicRay();
            ray.Color = new Color(3);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.Equal(3, result.ColorIndex);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectEndPoint()
        {
            var ray = CreateBasicRay();
            ray.StartPoint = new XYZ(0, 0, 0);
            ray.Direction = new XYZ(1, 0, 0);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.Equal(1000, result.EndPoint.X);
            Assert.Equal(0, result.EndPoint.Y);
            Assert.Equal(0, result.EndPoint.Z);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectLength()
        {
            var ray = CreateBasicRay();
            ray.StartPoint = new XYZ(0, 0, 0);
            ray.Direction = new XYZ(1, 0, 0);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.Equal(1000, result.Length);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectAngle()
        {
            var ray = CreateBasicRay();
            ray.StartPoint = new XYZ(0, 0, 0);
            ray.Direction = new XYZ(1, 0, 0);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.Equal(0, result.Angle);
        }

        [Fact]
        public void Render_Ray_WithDiagonalDirection_ReturnsCorrectAngle()
        {
            var ray = CreateBasicRay();
            ray.StartPoint = new XYZ(0, 0, 0);
            ray.Direction = new XYZ(1, 1, 0).Normalize();
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 4, result.Angle);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectBounds()
        {
            var ray = CreateBasicRay();
            ray.StartPoint = new XYZ(0, 0, 0);
            ray.Direction = new XYZ(1, 0, 0);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(0, result.Bounds.Min.Z);
            Assert.Equal(1000, result.Bounds.Max.X);
            Assert.Equal(0, result.Bounds.Max.Y);
            Assert.Equal(0, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectCentroid()
        {
            var ray = CreateBasicRay();
            ray.StartPoint = new XYZ(0, 0, 0);
            ray.Direction = new XYZ(1, 0, 0);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);
            Assert.Equal(500, result.Centroid.X);
            Assert.Equal(0, result.Centroid.Y);
            Assert.Equal(0, result.Centroid.Z);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectTransform()
        {
            var ray = CreateBasicRay();
            ray.StartPoint = new XYZ(10, 20, 30);
            ray.Direction = new XYZ(1, 0, 0);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.Equal(10, result.Transform.Position.X);
            Assert.Equal(20, result.Transform.Position.Y);
            Assert.Equal(30, result.Transform.Position.Z);
            Assert.Equal(0, result.Transform.Rotation.X);
            Assert.Equal(0, result.Transform.Rotation.Y);
            Assert.Equal(0, result.Transform.Rotation.Z);
            Assert.Equal(1, result.Transform.Scale.X);
            Assert.Equal(1, result.Transform.Scale.Y);
            Assert.Equal(1, result.Transform.Scale.Z);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectGeometry()
        {
            var ray = CreateBasicRay();
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);
            Assert.Equal("BufferGeometry", result.Geometry.Type);
            Assert.Equal(2, result.Geometry.VertexCount);
            Assert.True(result.Geometry.HasColors);
            Assert.False(result.Geometry.HasNormals);
            Assert.True(result.Geometry.HasIndices);
            Assert.Equal("LinePieces", result.Geometry.PrimitiveType);
            Assert.Equal(2, result.Geometry.IndexCount);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectMaterial()
        {
            var ray = CreateBasicRay();
            ray.Color = new Color(1);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);
            Assert.Equal("LineBasicMaterial", result.Material.Type);
            Assert.Equal(0xff0000, result.Material.Color);
            Assert.Equal(1.0, result.Material.Opacity);
            Assert.False(result.Material.Transparent);
            Assert.False(result.Material.VertexColors);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectVertexPositions()
        {
            var ray = CreateBasicRay();
            ray.StartPoint = new XYZ(0, 0, 0);
            ray.Direction = new XYZ(1, 0, 0);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexPositions);
            Assert.Equal(6, result.VertexPositions.Count);
            
            Assert.Equal(0, result.VertexPositions[0]);
            Assert.Equal(0, result.VertexPositions[1]);
            Assert.Equal(0, result.VertexPositions[2]);
            
            Assert.Equal(1000, result.VertexPositions[3]);
            Assert.Equal(0, result.VertexPositions[4]);
            Assert.Equal(0, result.VertexPositions[5]);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectVertexColors()
        {
            var ray = CreateBasicRay();
            ray.Color = new Color(1);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexColors);
            Assert.Equal(6, result.VertexColors.Count);
            
            Assert.Equal(1.0, result.VertexColors[0]);
            Assert.Equal(0.0, result.VertexColors[1]);
            Assert.Equal(0.0, result.VertexColors[2]);
            
            Assert.Equal(1.0, result.VertexColors[3]);
            Assert.Equal(0.0, result.VertexColors[4]);
            Assert.Equal(0.0, result.VertexColors[5]);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectVertexNormals()
        {
            var ray = CreateBasicRay();
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexNormals);
            Assert.Equal(6, result.VertexNormals.Count);
            
            Assert.Equal(0, result.VertexNormals[0]);
            Assert.Equal(0, result.VertexNormals[1]);
            Assert.Equal(1, result.VertexNormals[2]);
            
            Assert.Equal(0, result.VertexNormals[3]);
            Assert.Equal(0, result.VertexNormals[4]);
            Assert.Equal(1, result.VertexNormals[5]);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectIndices()
        {
            var ray = CreateBasicRay();
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.NotNull(result.Indices);
            Assert.Equal(2, result.Indices.Count);
            
            Assert.Equal(0, result.Indices[0]);
            Assert.Equal(1, result.Indices[1]);
        }

        [Fact]
        public void Render_Ray_WithDifferentStartPoint_ReturnsCorrectTransform()
        {
            var ray = CreateBasicRay();
            ray.StartPoint = new XYZ(100, 200, 50);
            ray.Direction = new XYZ(1, 0, 0);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.Equal(100, result.Transform.Position.X);
            Assert.Equal(200, result.Transform.Position.Y);
            Assert.Equal(50, result.Transform.Position.Z);
        }

        [Fact]
        public void Render_Ray_WithVerticalDirection_ReturnsCorrectAngle()
        {
            var ray = CreateBasicRay();
            ray.StartPoint = new XYZ(0, 0, 0);
            ray.Direction = new XYZ(0, 1, 0);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.Equal(Math.PI / 2, result.Angle);
        }

        [Fact]
        public void Render_Ray_WithNegativeDirection_ReturnsCorrectAngle()
        {
            var ray = CreateBasicRay();
            ray.StartPoint = new XYZ(0, 0, 0);
            ray.Direction = new XYZ(-1, 0, 0);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.Equal(Math.PI, result.Angle);
        }

        [Fact]
        public void Render_Ray_With3DDirection_ReturnsCorrectEndPoint()
        {
            var ray = CreateBasicRay();
            ray.StartPoint = new XYZ(0, 0, 0);
            ray.Direction = new XYZ(1, 1, 1).Normalize();
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.Equal(1000 / Math.Sqrt(3), result.EndPoint.X);
            Assert.Equal(1000 / Math.Sqrt(3), result.EndPoint.Y);
            Assert.Equal(1000 / Math.Sqrt(3), result.EndPoint.Z);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectColorData()
        {
            var ray = CreateBasicRay();
            ray.Color = new Color(2);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);
            Assert.Equal(2, result.Color.Index);
            Assert.Equal(255, result.Color.R);
            Assert.Equal(255, result.Color.G);
            Assert.Equal(0, result.Color.B);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectLineWeight()
        {
            var ray = CreateBasicRay();
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.Equal(0.03, result.LineWeight, 2);
        }

        [Fact]
        public void Render_Ray_ReturnsCorrectLineTypeName()
        {
            var ray = CreateBasicRay();
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.Equal("Continuous", result.LineTypeName);
        }

        [Fact]
        public void Render_Ray_WithDifferentColorIndex_ReturnsCorrectColor()
        {
            var ray = CreateBasicRay();
            ray.Color = new Color(5);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.Equal(0x0000ff, result.Material.Color);
        }

        [Fact]
        public void Render_Ray_WithStartPointOffset_ReturnsCorrectBounds()
        {
            var ray = CreateBasicRay();
            ray.StartPoint = new XYZ(100, 100, 100);
            ray.Direction = new XYZ(1, 0, 0);
            var result = RayEntityRenderer.Render(ray);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.Equal(100, result.Bounds.Min.X);
            Assert.Equal(100, result.Bounds.Min.Y);
            Assert.Equal(100, result.Bounds.Min.Z);
            Assert.Equal(1100, result.Bounds.Max.X);
            Assert.Equal(100, result.Bounds.Max.Y);
            Assert.Equal(100, result.Bounds.Max.Z);
        }
    }
}
