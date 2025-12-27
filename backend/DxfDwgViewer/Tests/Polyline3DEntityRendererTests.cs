using System;
using Xunit;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using DxfDwgViewer.RenderUtilities;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class Polyline3DEntityRendererTests
    {
        private Polyline3D CreateBasicPolyline3D()
        {
            var polyline = new Polyline3D
            {
                IsClosed = false,
                Elevation = 0,
                Thickness = 0
            };

            polyline.Vertices.Add(new Vertex3D
            {
                Location = new XYZ(0, 0, 0)
            });

            polyline.Vertices.Add(new Vertex3D
            {
                Location = new XYZ(10, 0, 0)
            });

            polyline.Vertices.Add(new Vertex3D
            {
                Location = new XYZ(10, 10, 5)
            });

            polyline.Color = new Color(1);
            polyline.LineWeight = ACadSharp.LineWeightType.W25;

            return polyline;
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectVertexCount()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(3, result.VertexCount);
            Assert.Equal(3, result.Vertices.Count);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectVertices()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(3, result.Vertices.Count);

            Assert.Equal(0, result.Vertices[0].Location.X);
            Assert.Equal(0, result.Vertices[0].Location.Y);
            Assert.Equal(0, result.Vertices[0].Location.Z);

            Assert.Equal(10, result.Vertices[1].Location.X);
            Assert.Equal(0, result.Vertices[1].Location.Y);
            Assert.Equal(0, result.Vertices[1].Location.Z);

            Assert.Equal(10, result.Vertices[2].Location.X);
            Assert.Equal(10, result.Vertices[2].Location.Y);
            Assert.Equal(5, result.Vertices[2].Location.Z);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectVertexPositions()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexPositions);
            Assert.Equal(9, result.VertexPositions.Length);

            Assert.Equal(0, result.VertexPositions[0]);
            Assert.Equal(0, result.VertexPositions[1]);
            Assert.Equal(0, result.VertexPositions[2]);

            Assert.Equal(10, result.VertexPositions[3]);
            Assert.Equal(0, result.VertexPositions[4]);
            Assert.Equal(0, result.VertexPositions[5]);

            Assert.Equal(10, result.VertexPositions[6]);
            Assert.Equal(10, result.VertexPositions[7]);
            Assert.Equal(5, result.VertexPositions[8]);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectVertexNormals()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexNormals);
            Assert.Equal(9, result.VertexNormals.Length);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectVertexUVs()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexUVs);
            Assert.Equal(6, result.VertexUVs.Length);

            Assert.Equal(0, result.VertexUVs[0]);
            Assert.Equal(0, result.VertexUVs[1]);

            Assert.Equal(0.5, result.VertexUVs[2], 2);
            Assert.Equal(0, result.VertexUVs[3]);

            Assert.Equal(1, result.VertexUVs[4], 2);
            Assert.Equal(0, result.VertexUVs[5]);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectIndices()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Indices);
            Assert.Equal(4, result.Indices.Length);

            Assert.Equal(0, result.Indices[0]);
            Assert.Equal(1, result.Indices[1]);
            Assert.Equal(1, result.Indices[2]);
            Assert.Equal(2, result.Indices[3]);
        }

        [Fact]
        public void Render_Polyline3D_Closed_ReturnsCorrectIndices()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.IsClosed = true;
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Indices);
            Assert.Equal(6, result.Indices.Length);

            Assert.Equal(0, result.Indices[0]);
            Assert.Equal(1, result.Indices[1]);
            Assert.Equal(1, result.Indices[2]);
            Assert.Equal(2, result.Indices[3]);
            Assert.Equal(2, result.Indices[4]);
            Assert.Equal(0, result.Indices[5]);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectTotalLength()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            double expectedLength = 10 + Math.Sqrt(10 * 10 + 5 * 5);
            Assert.Equal(expectedLength, result.TotalLength, 2);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectCentroid()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);

            Assert.Equal(20.0 / 3.0, result.Centroid.X, 2);
            Assert.Equal(10.0 / 3.0, result.Centroid.Y, 2);
            Assert.Equal(5.0 / 3.0, result.Centroid.Z, 2);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectBounds()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);

            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(0, result.Bounds.Min.Z);

            Assert.Equal(10, result.Bounds.Max.X);
            Assert.Equal(10, result.Bounds.Max.Y);
            Assert.Equal(5, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectIsClosed()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.IsClosed = true;
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.True(result.IsClosed);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectColorIndex()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.Color = new Color(5);
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectLineWeight()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.LineWeight = ACadSharp.LineWeightType.W50;
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(0.50, result.LineWeight);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectElevation()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.Elevation = 10.0;
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(10.0, result.Elevation);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectThickness()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.Thickness = 5.0;
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(5.0, result.Thickness);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectTransform()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);

            Assert.Equal(20.0 / 3.0, result.Transform.Position.X, 2);
            Assert.Equal(10.0 / 3.0, result.Transform.Position.Y, 2);
            Assert.Equal(5.0 / 3.0, result.Transform.Position.Z, 2);

            Assert.Equal(0, result.Transform.Rotation.X);
            Assert.Equal(0, result.Transform.Rotation.Y);
            Assert.Equal(0, result.Transform.Rotation.Z);

            Assert.Equal(1, result.Transform.Scale.X);
            Assert.Equal(1, result.Transform.Scale.Y);
            Assert.Equal(1, result.Transform.Scale.Z);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectMaterial()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);

            Assert.Equal("LineBasicMaterial", result.Material.Type);
            Assert.Equal(0.25, result.Material.LineWidth);
            Assert.Equal(1.0, result.Material.Opacity);
            Assert.False(result.Material.Transparent);
            Assert.False(result.Material.Wireframe);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectGeometry()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);

            Assert.Equal("BufferGeometry", result.Geometry.Type);
            Assert.Equal(3, result.Geometry.VertexCount);
            Assert.Equal(0, result.Geometry.FaceCount);
            Assert.True(result.Geometry.HasNormals);
            Assert.True(result.Geometry.HasUVs);
            Assert.True(result.Geometry.HasIndices);
            Assert.Equal("Line", result.Geometry.PrimitiveType);
            Assert.Equal(4, result.Geometry.IndexCount);
            Assert.False(result.Geometry.IsClosed);
            Assert.False(result.Geometry.IsPeriodic);
            Assert.Equal(1, result.Geometry.Degree);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectHasSegments()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.True(result.HasSegments);
        }

        [Fact]
        public void Render_Polyline3D_ReturnsCorrectSegments()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Segments);
            Assert.Equal(2, result.Segments.Count);

            var segment1 = result.Segments[0];
            Assert.Equal(0, segment1.Start.X);
            Assert.Equal(0, segment1.Start.Y);
            Assert.Equal(0, segment1.Start.Z);
            Assert.Equal(10, segment1.End.X);
            Assert.Equal(0, segment1.End.Y);
            Assert.Equal(0, segment1.End.Z);
            Assert.Equal(10, segment1.Length);
            Assert.Equal(1, segment1.Direction.X);
            Assert.Equal(0, segment1.Direction.Y);
            Assert.Equal(0, segment1.Direction.Z);

            var segment2 = result.Segments[1];
            Assert.Equal(10, segment2.Start.X);
            Assert.Equal(0, segment2.Start.Y);
            Assert.Equal(0, segment2.Start.Z);
            Assert.Equal(10, segment2.End.X);
            Assert.Equal(10, segment2.End.Y);
            Assert.Equal(5, segment2.End.Z);
            Assert.Equal(Math.Sqrt(10 * 10 + 5 * 5), segment2.Length, 2);
        }

        [Fact]
        public void Render_Polyline3D_Closed_ReturnsCorrectSegments()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.IsClosed = true;
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Segments);
            Assert.Equal(3, result.Segments.Count);

            var segment3 = result.Segments[2];
            Assert.Equal(10, segment3.Start.X);
            Assert.Equal(10, segment3.Start.Y);
            Assert.Equal(5, segment3.Start.Z);
            Assert.Equal(0, segment3.End.X);
            Assert.Equal(0, segment3.End.Y);
            Assert.Equal(0, segment3.End.Z);
        }

        [Fact]
        public void Render_Polyline3D_EmptyVertices_ReturnsEmptyData()
        {
            var polyline = new Polyline3D
            {
                IsClosed = false,
                Color = new Color(1)
            };

            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(0, result.VertexCount);
            Assert.Empty(result.Vertices);
            Assert.Empty(result.VertexPositions);
            Assert.Empty(result.VertexNormals);
            Assert.Empty(result.VertexUVs);
            Assert.Empty(result.Indices);
            Assert.Equal(0, result.TotalLength);
            Assert.False(result.HasSegments);
            Assert.Empty(result.Segments);
        }

        [Fact]
        public void Render_Polyline3D_SingleVertex_ReturnsNoSegments()
        {
            var polyline = new Polyline3D
            {
                IsClosed = false,
                Color = new Color(1)
            };

            polyline.Vertices.Add(new Vertex3D
            {
                Location = new XYZ(5, 5, 5)
            });

            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(1, result.VertexCount);
            Assert.Single(result.Vertices);
            Assert.Equal(3, result.VertexPositions.Length);
            Assert.Empty(result.Indices);
            Assert.Equal(0, result.TotalLength);
            Assert.False(result.HasSegments);
            Assert.Empty(result.Segments);
        }

        [Fact]
        public void Render_Polyline3D_Closed_SingleVertex_ReturnsNoSegments()
        {
            var polyline = new Polyline3D
            {
                IsClosed = true,
                Color = new Color(1)
            };

            polyline.Vertices.Add(new Vertex3D
            {
                Location = new XYZ(5, 5, 5)
            });

            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(1, result.VertexCount);
            Assert.Single(result.Vertices);
            Assert.Equal(3, result.VertexPositions.Length);
            Assert.Empty(result.Indices);
            Assert.Equal(0, result.TotalLength);
            Assert.False(result.HasSegments);
            Assert.Empty(result.Segments);
        }

        [Fact]
        public void Render_Polyline3D_Closed_ReturnsCorrectTotalLength()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.IsClosed = true;
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            double expectedLength = 10 + Math.Sqrt(10 * 10 + 5 * 5) + Math.Sqrt(10 * 10 + 5 * 5);
            Assert.Equal(expectedLength, result.TotalLength, 2);
        }

        [Fact]
        public void Render_Polyline3D_WithDifferentLineWeight_ReturnsCorrectLineWidth()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.LineWeight = ACadSharp.LineWeightType.W100;
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(1.00, result.Material.LineWidth);
        }

        [Fact]
        public void Render_Polyline3D_WithByLayerLineWeight_ReturnsDefaultLineWidth()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.LineWeight = ACadSharp.LineWeightType.ByLayer;
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(1.0, result.Material.LineWidth);
        }

        [Fact]
        public void Render_Polyline3D_WithByBlockLineWeight_ReturnsDefaultLineWidth()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.LineWeight = ACadSharp.LineWeightType.ByBlock;
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(1.0, result.Material.LineWidth);
        }

        [Fact]
        public void Render_Polyline3D_WithDefaultLineWeight_ReturnsDefaultLineWidth()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.LineWeight = ACadSharp.LineWeightType.Default;
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.Equal(1.0, result.Material.LineWidth);
        }

        [Fact]
        public void Render_Polyline3D_Closed_GeometryIsClosed()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.IsClosed = true;
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.True(result.Geometry.IsClosed);
        }

        [Fact]
        public void Render_Polyline3D_NotClosed_GeometryNotClosed()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.IsClosed = false;
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.False(result.Geometry.IsClosed);
        }

        [Fact]
        public void Render_Polyline3D_VertexNormalsAreNormalized()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexNormals);

            for (int i = 0; i < result.VertexNormals.Length; i += 3)
            {
                double x = result.VertexNormals[i];
                double y = result.VertexNormals[i + 1];
                double z = result.VertexNormals[i + 2];
                double length = Math.Sqrt(x * x + y * y + z * z);
                Assert.True(length <= 1.01);
            }
        }

        [Fact]
        public void Render_Polyline3D_SegmentDirectionsAreNormalized()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Segments);

            foreach (var segment in result.Segments)
            {
                double x = segment.Direction.X;
                double y = segment.Direction.Y;
                double z = segment.Direction.Z;
                double length = Math.Sqrt(x * x + y * y + z * z);
                Assert.True(length <= 1.01);
            }
        }

        [Fact]
        public void Render_Polyline3D_SegmentLengthsAreCorrect()
        {
            var polyline = CreateBasicPolyline3D();
            var result = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(result);
            Assert.NotNull(result.Segments);

            foreach (var segment in result.Segments)
            {
                double dx = segment.End.X - segment.Start.X;
                double dy = segment.End.Y - segment.Start.Y;
                double dz = segment.End.Z - segment.Start.Z;
                double expectedLength = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                Assert.Equal(expectedLength, segment.Length, 2);
            }
        }
    }
}
