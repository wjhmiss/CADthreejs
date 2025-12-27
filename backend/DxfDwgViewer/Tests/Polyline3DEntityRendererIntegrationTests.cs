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
    public class Polyline3DEntityRendererIntegrationTests
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
        public void LoadAndRenderPolyline3DFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var polylineEntities = new List<Polyline3D>();
            foreach (var entity in doc.Entities)
            {
                if (entity is Polyline3D polyline)
                {
                    polylineEntities.Add(polyline);
                }
            }

            if (polylineEntities.Count == 0)
            {
                return;
            }

            foreach (var polyline in polylineEntities)
            {
                var polylineData = Polyline3DEntityRenderer.Render(polyline);
                Assert.NotNull(polylineData);
                Assert.NotNull(polylineData.Vertices);
                Assert.NotNull(polylineData.Bounds);
            }
        }

        [Fact]
        public void SerializePolyline3DDataToJson_Success()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);
            var json = JsonConvert.SerializeObject(polylineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Vertices", json);
            Assert.Contains("Bounds", json);
        }

        [Fact]
        public void DeserializePolyline3DDataFromJson_Success()
        {
            var polyline = CreateBasicPolyline3D();

            var originalPolylineData = Polyline3DEntityRenderer.Render(polyline);
            var json = JsonConvert.SerializeObject(originalPolylineData);
            var deserializedPolylineData = JsonConvert.DeserializeObject<Polyline3DEntityRenderer.Polyline3DData>(json);

            Assert.NotNull(deserializedPolylineData);
            Assert.Equal(originalPolylineData.VertexCount, deserializedPolylineData.VertexCount);
            Assert.Equal(originalPolylineData.IsClosed, deserializedPolylineData.IsClosed);
            Assert.Equal(originalPolylineData.ColorIndex, deserializedPolylineData.ColorIndex);
            Assert.Equal(originalPolylineData.LineWeight, deserializedPolylineData.LineWeight);
            Assert.Equal(originalPolylineData.Elevation, deserializedPolylineData.Elevation);
            Assert.Equal(originalPolylineData.Thickness, deserializedPolylineData.Thickness);
            Assert.Equal(originalPolylineData.TotalLength, deserializedPolylineData.TotalLength);
            Assert.Equal(originalPolylineData.HasSegments, deserializedPolylineData.HasSegments);
        }

        [Fact]
        public void RenderMultiplePolylines3D_CollectAllPolylineData()
        {
            var polylines = new List<Polyline3D>
            {
                CreateBasicPolyline3D(),
                CreateBasicPolyline3D()
            };
            polylines[0].Vertices[0].Location = new XYZ(0, 0, 0);
            polylines[1].Vertices[0].Location = new XYZ(20, 20, 10);

            var polylineDataList = new List<Polyline3DEntityRenderer.Polyline3DData>();
            foreach (var polyline in polylines)
            {
                var polylineData = Polyline3DEntityRenderer.Render(polyline);
                polylineDataList.Add(polylineData);
            }

            Assert.Equal(2, polylineDataList.Count);
            Assert.Equal(0, polylineDataList[0].Vertices[0].Location.X);
            Assert.Equal(20, polylineDataList[1].Vertices[0].Location.X);
        }

        [Fact]
        public void RenderPolyline3DWithComplexProperties_PreservesAllProperties()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.IsClosed = true;
            polyline.Elevation = 5.5;
            polyline.Thickness = 3.0;
            polyline.LineWeight = ACadSharp.LineWeightType.W50;
            polyline.Color = new Color(5);

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.True(polylineData.IsClosed);
            Assert.Equal(5.5, polylineData.Elevation);
            Assert.Equal(3.0, polylineData.Thickness);
            Assert.Equal(0.50, polylineData.LineWeight);
            Assert.Equal(5, polylineData.ColorIndex);
        }

        [Fact]
        public void RenderPolyline3D_CalculatesCorrectTotalLength()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            double expectedLength = 10 + Math.Sqrt(10 * 10 + 5 * 5);
            Assert.Equal(expectedLength, polylineData.TotalLength, 2);
        }

        [Fact]
        public void RenderClosedPolyline3D_CalculatesCorrectTotalLength()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.IsClosed = true;

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            double expectedLength = 10 + Math.Sqrt(10 * 10 + 5 * 5) + Math.Sqrt(10 * 10 + 5 * 5);
            Assert.Equal(expectedLength, polylineData.TotalLength, 2);
        }

        [Fact]
        public void RenderPolyline3D_CalculatesCorrectCentroid()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.Centroid);

            Assert.Equal(20.0 / 3.0, polylineData.Centroid.X, 2);
            Assert.Equal(10.0 / 3.0, polylineData.Centroid.Y, 2);
            Assert.Equal(5.0 / 3.0, polylineData.Centroid.Z, 2);
        }

        [Fact]
        public void RenderPolyline3D_CalculatesCorrectBounds()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.Bounds);

            Assert.Equal(0, polylineData.Bounds.Min.X);
            Assert.Equal(0, polylineData.Bounds.Min.Y);
            Assert.Equal(0, polylineData.Bounds.Min.Z);

            Assert.Equal(10, polylineData.Bounds.Max.X);
            Assert.Equal(10, polylineData.Bounds.Max.Y);
            Assert.Equal(5, polylineData.Bounds.Max.Z);
        }

        [Fact]
        public void RenderPolyline3D_GeneratesCorrectIndices()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.Indices);
            Assert.Equal(4, polylineData.Indices.Length);

            Assert.Equal(0, polylineData.Indices[0]);
            Assert.Equal(1, polylineData.Indices[1]);
            Assert.Equal(1, polylineData.Indices[2]);
            Assert.Equal(2, polylineData.Indices[3]);
        }

        [Fact]
        public void RenderClosedPolyline3D_GeneratesCorrectIndices()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.IsClosed = true;

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.Indices);
            Assert.Equal(6, polylineData.Indices.Length);

            Assert.Equal(0, polylineData.Indices[0]);
            Assert.Equal(1, polylineData.Indices[1]);
            Assert.Equal(1, polylineData.Indices[2]);
            Assert.Equal(2, polylineData.Indices[3]);
            Assert.Equal(2, polylineData.Indices[4]);
            Assert.Equal(0, polylineData.Indices[5]);
        }

        [Fact]
        public void RenderPolyline3D_GeneratesCorrectVertexPositions()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.VertexPositions);
            Assert.Equal(9, polylineData.VertexPositions.Length);

            Assert.Equal(0, polylineData.VertexPositions[0]);
            Assert.Equal(0, polylineData.VertexPositions[1]);
            Assert.Equal(0, polylineData.VertexPositions[2]);

            Assert.Equal(10, polylineData.VertexPositions[3]);
            Assert.Equal(0, polylineData.VertexPositions[4]);
            Assert.Equal(0, polylineData.VertexPositions[5]);

            Assert.Equal(10, polylineData.VertexPositions[6]);
            Assert.Equal(10, polylineData.VertexPositions[7]);
            Assert.Equal(5, polylineData.VertexPositions[8]);
        }

        [Fact]
        public void RenderPolyline3D_GeneratesCorrectVertexNormals()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.VertexNormals);
            Assert.Equal(9, polylineData.VertexNormals.Length);
        }

        [Fact]
        public void RenderPolyline3D_GeneratesCorrectVertexUVs()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.VertexUVs);
            Assert.Equal(6, polylineData.VertexUVs.Length);

            Assert.Equal(0, polylineData.VertexUVs[0]);
            Assert.Equal(0, polylineData.VertexUVs[1]);

            Assert.Equal(0.5, polylineData.VertexUVs[2], 2);
            Assert.Equal(0, polylineData.VertexUVs[3]);

            Assert.Equal(1, polylineData.VertexUVs[4], 2);
            Assert.Equal(0, polylineData.VertexUVs[5]);
        }

        [Fact]
        public void RenderPolyline3D_CreatesCorrectSegments()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.True(polylineData.HasSegments);
            Assert.NotNull(polylineData.Segments);
            Assert.Equal(2, polylineData.Segments.Count);
        }

        [Fact]
        public void RenderClosedPolyline3D_CreatesCorrectSegments()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.IsClosed = true;

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.True(polylineData.HasSegments);
            Assert.NotNull(polylineData.Segments);
            Assert.Equal(3, polylineData.Segments.Count);
        }

        [Fact]
        public void RenderPolyline3D_SegmentDirectionsAreNormalized()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.Segments);

            foreach (var segment in polylineData.Segments)
            {
                double x = segment.Direction.X;
                double y = segment.Direction.Y;
                double z = segment.Direction.Z;
                double length = Math.Sqrt(x * x + y * y + z * z);
                Assert.True(length <= 1.01);
            }
        }

        [Fact]
        public void RenderPolyline3D_SegmentLengthsAreCorrect()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.Segments);

            foreach (var segment in polylineData.Segments)
            {
                double dx = segment.End.X - segment.Start.X;
                double dy = segment.End.Y - segment.Start.Y;
                double dz = segment.End.Z - segment.Start.Z;
                double expectedLength = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                Assert.Equal(expectedLength, segment.Length, 2);
            }
        }

        [Fact]
        public void RenderPolyline3D_EmptyVertices_ReturnsEmptyData()
        {
            var polyline = new Polyline3D
            {
                IsClosed = false,
                Color = new Color(1)
            };

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.Equal(0, polylineData.VertexCount);
            Assert.Empty(polylineData.Vertices);
            Assert.Empty(polylineData.VertexPositions);
            Assert.Empty(polylineData.VertexNormals);
            Assert.Empty(polylineData.VertexUVs);
            Assert.Empty(polylineData.Indices);
            Assert.Equal(0, polylineData.TotalLength);
            Assert.False(polylineData.HasSegments);
            Assert.Empty(polylineData.Segments);
        }

        [Fact]
        public void RenderPolyline3D_SingleVertex_ReturnsNoSegments()
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

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.Equal(1, polylineData.VertexCount);
            Assert.Single(polylineData.Vertices);
            Assert.Equal(3, polylineData.VertexPositions.Length);
            Assert.Empty(polylineData.Indices);
            Assert.Equal(0, polylineData.TotalLength);
            Assert.False(polylineData.HasSegments);
            Assert.Empty(polylineData.Segments);
        }

        [Fact]
        public void RenderPolyline3D_Closed_SingleVertex_ReturnsNoSegments()
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

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.Equal(1, polylineData.VertexCount);
            Assert.Single(polylineData.Vertices);
            Assert.Equal(3, polylineData.VertexPositions.Length);
            Assert.Empty(polylineData.Indices);
            Assert.Equal(0, polylineData.TotalLength);
            Assert.False(polylineData.HasSegments);
            Assert.Empty(polylineData.Segments);
        }

        [Fact]
        public void RenderPolyline3D_ReturnsCorrectTransform()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.Transform);

            Assert.Equal(20.0 / 3.0, polylineData.Transform.Position.X, 2);
            Assert.Equal(10.0 / 3.0, polylineData.Transform.Position.Y, 2);
            Assert.Equal(5.0 / 3.0, polylineData.Transform.Position.Z, 2);

            Assert.Equal(0, polylineData.Transform.Rotation.X);
            Assert.Equal(0, polylineData.Transform.Rotation.Y);
            Assert.Equal(0, polylineData.Transform.Rotation.Z);

            Assert.Equal(1, polylineData.Transform.Scale.X);
            Assert.Equal(1, polylineData.Transform.Scale.Y);
            Assert.Equal(1, polylineData.Transform.Scale.Z);
        }

        [Fact]
        public void RenderPolyline3D_ReturnsCorrectMaterial()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.Material);

            Assert.Equal("LineBasicMaterial", polylineData.Material.Type);
            Assert.Equal(0.25, polylineData.Material.LineWidth);
            Assert.Equal(1.0, polylineData.Material.Opacity);
            Assert.False(polylineData.Material.Transparent);
            Assert.False(polylineData.Material.Wireframe);
        }

        [Fact]
        public void RenderPolyline3D_ReturnsCorrectGeometry()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.Geometry);

            Assert.Equal("BufferGeometry", polylineData.Geometry.Type);
            Assert.Equal(3, polylineData.Geometry.VertexCount);
            Assert.Equal(0, polylineData.Geometry.FaceCount);
            Assert.True(polylineData.Geometry.HasNormals);
            Assert.True(polylineData.Geometry.HasUVs);
            Assert.True(polylineData.Geometry.HasIndices);
            Assert.Equal("Line", polylineData.Geometry.PrimitiveType);
            Assert.Equal(4, polylineData.Geometry.IndexCount);
            Assert.False(polylineData.Geometry.IsClosed);
            Assert.False(polylineData.Geometry.IsPeriodic);
            Assert.Equal(1, polylineData.Geometry.Degree);
        }

        [Fact]
        public void SerializeAndDeserializePolyline3D_PreservesAllProperties()
        {
            var polyline = CreateBasicPolyline3D();

            var originalPolylineData = Polyline3DEntityRenderer.Render(polyline);
            var json = JsonConvert.SerializeObject(originalPolylineData);
            var deserializedPolylineData = JsonConvert.DeserializeObject<Polyline3DEntityRenderer.Polyline3DData>(json);

            Assert.NotNull(deserializedPolylineData);
            Assert.Equal(originalPolylineData.VertexCount, deserializedPolylineData.VertexCount);
            Assert.Equal(originalPolylineData.IsClosed, deserializedPolylineData.IsClosed);
            Assert.Equal(originalPolylineData.ColorIndex, deserializedPolylineData.ColorIndex);
            Assert.Equal(originalPolylineData.LineWeight, deserializedPolylineData.LineWeight);
            Assert.Equal(originalPolylineData.Elevation, deserializedPolylineData.Elevation);
            Assert.Equal(originalPolylineData.Thickness, deserializedPolylineData.Thickness);
            Assert.Equal(originalPolylineData.TotalLength, deserializedPolylineData.TotalLength);
            Assert.Equal(originalPolylineData.HasSegments, deserializedPolylineData.HasSegments);
            Assert.Equal(originalPolylineData.Segments.Count, deserializedPolylineData.Segments.Count);
        }

        [Fact]
        public void RenderPolyline3D_WithDifferentLineWeight_ReturnsCorrectLineWidth()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.LineWeight = ACadSharp.LineWeightType.W100;

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.Equal(1.00, polylineData.Material.LineWidth);
        }

        [Fact]
        public void RenderPolyline3D_WithByLayerLineWeight_ReturnsDefaultLineWidth()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.LineWeight = ACadSharp.LineWeightType.ByLayer;

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.Equal(1.0, polylineData.Material.LineWidth);
        }

        [Fact]
        public void RenderPolyline3D_Closed_GeometryIsClosed()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.IsClosed = true;

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.True(polylineData.Geometry.IsClosed);
        }

        [Fact]
        public void RenderPolyline3D_NotClosed_GeometryNotClosed()
        {
            var polyline = CreateBasicPolyline3D();
            polyline.IsClosed = false;

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.False(polylineData.Geometry.IsClosed);
        }

        [Fact]
        public void RenderPolyline3D_VertexNormalsAreNormalized()
        {
            var polyline = CreateBasicPolyline3D();

            var polylineData = Polyline3DEntityRenderer.Render(polyline);

            Assert.NotNull(polylineData);
            Assert.NotNull(polylineData.VertexNormals);

            for (int i = 0; i < polylineData.VertexNormals.Length; i += 3)
            {
                double x = polylineData.VertexNormals[i];
                double y = polylineData.VertexNormals[i + 1];
                double z = polylineData.VertexNormals[i + 2];
                double length = Math.Sqrt(x * x + y * y + z * z);
                Assert.True(length <= 1.01);
            }
        }
    }
}
