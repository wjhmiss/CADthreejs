using System;
using System.Text.Json;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using CSMath;
using DxfDwgViewer.RenderUtilities;
using Xunit;

namespace DxfDwgViewer.Tests
{
    public class HatchEntityRendererIntegrationTests
    {
        [Fact]
        public void SerializeAndDeserialize_SolidHatch_ShouldPreserveAllProperties()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TEST_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.True(deserializedData.IsSolid);
            Assert.Equal(1, deserializedData.ColorIndex);
            Assert.Equal("TEST_LAYER", deserializedData.LayerName);
            Assert.Equal("Hatch", deserializedData.Type);
            Assert.Equal("HATCH", deserializedData.EntityType);
        }

        [Fact]
        public void SerializeAndDeserialize_PatternHatch_ShouldPreservePatternProperties()
        {
            var hatch = new Hatch
            {
                IsSolid = false,
                Pattern = new ACadSharp.Entities.HatchPattern("ANSI31"),
                PatternAngle = Math.PI / 4,
                PatternScale = 2.0,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(2),
                Layer = new Layer("PATTERN_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.False(deserializedData.IsSolid);
            Assert.Equal("ANSI31", deserializedData.PatternName);
            Assert.Equal(Math.PI / 4, deserializedData.PatternAngle);
            Assert.Equal(2.0, deserializedData.PatternScale);
            Assert.Equal(2, deserializedData.ColorIndex);
            Assert.Equal("PATTERN_LAYER", deserializedData.LayerName);
        }

        [Fact]
        public void SerializeAndDeserialize_HatchWithTransparency_ShouldPreserveTransparency()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Transparency = new ACadSharp.Transparency(50),
                Layer = new Layer("TRANSPARENT_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal(0.5, deserializedData.Transparency);
            Assert.Equal(0.5, deserializedData.MaterialOpacity);
            Assert.True(deserializedData.MaterialTransparent);
        }

        [Fact]
        public void SerializeAndDeserialize_HatchWithElevation_ShouldPreserveZCoordinates()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 10,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("ELEVATION_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal(10, deserializedData.Elevation);
            Assert.Equal(10, deserializedData.Centroid3D.Z);
            Assert.All(deserializedData.Points, p => Assert.Equal(10, p.Z));
        }

        [Fact]
        public void SerializeAndDeserialize_HatchWithCustomNormal_ShouldPreserveNormalVector()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(1),
                Layer = new Layer("NORMAL_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal(0, deserializedData.NormalX);
            Assert.Equal(0, deserializedData.NormalY);
            Assert.Equal(1, deserializedData.NormalZ);
        }

        [Fact]
        public void SerializeAndDeserialize_HatchWithMultiplePaths_ShouldPreservePathCount()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("MULTIPATH_LAYER")
            };

            var boundaryPath1 = new Hatch.BoundaryPath();
            var lineEdge1 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath1.Edges.Add(lineEdge1);
            boundaryPath1.Edges.Add(lineEdge2);
            boundaryPath1.Edges.Add(lineEdge3);
            boundaryPath1.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath1);

            var boundaryPath2 = new Hatch.BoundaryPath();
            var lineEdge5 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(20, 20),
                End = new XY(30, 20)
            };
            var lineEdge6 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(30, 20),
                End = new XY(30, 30)
            };
            var lineEdge7 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(30, 30),
                End = new XY(20, 30)
            };
            var lineEdge8 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(20, 30),
                End = new XY(20, 20)
            };
            boundaryPath2.Edges.Add(lineEdge5);
            boundaryPath2.Edges.Add(lineEdge6);
            boundaryPath2.Edges.Add(lineEdge7);
            boundaryPath2.Edges.Add(lineEdge8);
            hatch.Paths.Add(boundaryPath2);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal(2, deserializedData.PathCount);
        }

        [Fact]
        public void SerializeAndDeserialize_HatchWithArcEdges_ShouldPreserveEdgeTypes()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("ARC_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var arcEdge = new Hatch.BoundaryPath.Arc
            {
                Center = new XY(5, 5),
                Radius = 5,
                StartAngle = 0,
                EndAngle = Math.PI * 2,
                CounterClockWise = true
            };
            boundaryPath.Edges.Add(arcEdge);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.Contains("Arc", deserializedData.EdgeTypes);
            Assert.Equal(1, deserializedData.TotalEdges);
        }

        [Fact]
        public void SerializeAndDeserialize_HatchWithGradient_ShouldPreserveGradientProperties()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                GradientColor = new HatchGradientPattern { Name = "GRADIENT1" },
                Layer = new Layer("GRADIENT_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.True(deserializedData.HasGradient);
            Assert.Equal("GRADIENT1", deserializedData.GradientColorName);
        }

        [Fact]
        public void SerializeAndDeserialize_HatchWithDoublePattern_ShouldPreserveDoubleFlag()
        {
            var hatch = new Hatch
            {
                IsSolid = false,
                Pattern = new ACadSharp.Entities.HatchPattern("ANSI31"),
                IsDouble = true,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("DOUBLE_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.True(deserializedData.IsDouble);
        }

        [Fact]
        public void SerializeAndDeserialize_Hatch_ShouldPreserveTransformMatrix()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("TRANSFORM_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.NotNull(deserializedData.Transform);
            Assert.NotNull(deserializedData.Transform.Matrix);
            Assert.Equal(16, deserializedData.Transform.Matrix.Length);
        }

        [Fact]
        public void SerializeAndDeserialize_Hatch_ShouldPreserveMaterialProperties()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("MATERIAL_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal("MeshBasicMaterial", deserializedData.MaterialType);
            Assert.True(deserializedData.MaterialDepthTest);
            Assert.True(deserializedData.MaterialDepthWrite);
            Assert.Equal(2, deserializedData.MaterialSide);
        }

        [Fact]
        public void SerializeAndDeserialize_Hatch_ShouldPreserveBounds()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("BOUNDS_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.NotNull(deserializedData.Bounds);
            Assert.Equal(0, deserializedData.Bounds.Min.X);
            Assert.Equal(0, deserializedData.Bounds.Min.Y);
            Assert.Equal(0, deserializedData.Bounds.Min.Z);
            Assert.Equal(10, deserializedData.Bounds.Max.X);
            Assert.Equal(10, deserializedData.Bounds.Max.Y);
            Assert.Equal(0, deserializedData.Bounds.Max.Z);
        }

        [Fact]
        public void SerializeAndDeserialize_Hatch_ShouldPreserveCentroid()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("CENTROID_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.NotNull(deserializedData.Centroid);
            Assert.NotNull(deserializedData.Centroid3D);
            Assert.Equal(2.5, deserializedData.Centroid.X);
            Assert.Equal(2.5, deserializedData.Centroid.Y);
            Assert.Equal(2.5, deserializedData.Centroid3D.X);
            Assert.Equal(2.5, deserializedData.Centroid3D.Y);
            Assert.Equal(0, deserializedData.Centroid3D.Z);
        }

        [Fact]
        public void SerializeAndDeserialize_Hatch_ShouldPreserveArea()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("AREA_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal(100, deserializedData.Area);
        }

        [Fact]
        public void SerializeAndDeserialize_Hatch_ShouldPreserveVerticesAndNormals()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("VERTICES_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.NotNull(deserializedData.Vertices);
            Assert.NotNull(deserializedData.Normals);
            Assert.True(deserializedData.Vertices.Count > 0);
            Assert.True(deserializedData.Normals.Count > 0);
        }

        [Fact]
        public void SerializeAndDeserialize_Hatch_ShouldPreserveIndices()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("INDICES_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.NotNull(deserializedData.Indices);
            Assert.True(deserializedData.Indices.Count > 0);
        }

        [Fact]
        public void SerializeAndDeserialize_Hatch_ShouldPreserveCoordinateSystem()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("COORDINATE_LAYER")
            };

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.Equal("AutoCAD", deserializedData.CoordinateSystem);
            Assert.True(deserializedData.RequiresYAxisFlip);
        }

        [Fact]
        public void SerializeAndDeserialize_MultipleHatches_ShouldMaintainDataIntegrity()
        {
            var hatches = new[]
            {
                new Hatch
                {
                    IsSolid = true,
                    Pattern = ACadSharp.Entities.HatchPattern.Solid,
                    Elevation = 0,
                    Normal = XYZ.AxisZ,
                    Color = new ACadSharp.Color(1),
                    Layer = new Layer("HATCH_1")
                },
                new Hatch
                {
                    IsSolid = false,
                    Pattern = new ACadSharp.Entities.HatchPattern("ANSI31"),
                    Elevation = 0,
                    Normal = XYZ.AxisZ,
                    Color = new ACadSharp.Color(2),
                    Layer = new Layer("HATCH_2")
                },
                new Hatch
                {
                    IsSolid = true,
                    Pattern = ACadSharp.Entities.HatchPattern.Solid,
                    Elevation = 10,
                    Normal = XYZ.AxisZ,
                    Color = new ACadSharp.Color(3),
                    Layer = new Layer("HATCH_3")
                }
            };

            foreach (var hatch in hatches)
            {
                var boundaryPath = new Hatch.BoundaryPath();
                var lineEdge = new Hatch.BoundaryPath.Line
                {
                    Start = new XY(0, 0),
                    End = new XY(10, 0)
                };
                var lineEdge2 = new Hatch.BoundaryPath.Line
                {
                    Start = new XY(10, 0),
                    End = new XY(10, 10)
                };
                var lineEdge3 = new Hatch.BoundaryPath.Line
                {
                    Start = new XY(10, 10),
                    End = new XY(0, 10)
                };
                var lineEdge4 = new Hatch.BoundaryPath.Line
                {
                    Start = new XY(0, 10),
                    End = new XY(0, 0)
                };
                boundaryPath.Edges.Add(lineEdge);
                boundaryPath.Edges.Add(lineEdge2);
                boundaryPath.Edges.Add(lineEdge3);
                boundaryPath.Edges.Add(lineEdge4);
                hatch.Paths.Add(boundaryPath);
            }

            var renderDataArray = new HatchEntityRenderer.HatchData[hatches.Length];
            for (int i = 0; i < hatches.Length; i++)
            {
                renderDataArray[i] = HatchEntityRenderer.Render(hatches[i]);
            }

            var json = JsonSerializer.Serialize(renderDataArray);
            var deserializedDataArray = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData[]>(json);

            Assert.NotNull(deserializedDataArray);
            Assert.Equal(hatches.Length, deserializedDataArray.Length);
            Assert.Equal(3, deserializedDataArray.Length);
        }

        [Fact]
        public void SerializeAndDeserialize_HatchWithInvisibleFlag_ShouldPreserveVisibility()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Layer = new Layer("INVISIBLE_HATCH")
            };
            hatch.IsInvisible = true;

            var boundaryPath = new Hatch.BoundaryPath();
            var lineEdge = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 0),
                End = new XY(10, 0)
            };
            var lineEdge2 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 0),
                End = new XY(10, 10)
            };
            var lineEdge3 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(10, 10),
                End = new XY(0, 10)
            };
            var lineEdge4 = new Hatch.BoundaryPath.Line
            {
                Start = new XY(0, 10),
                End = new XY(0, 0)
            };
            boundaryPath.Edges.Add(lineEdge);
            boundaryPath.Edges.Add(lineEdge2);
            boundaryPath.Edges.Add(lineEdge3);
            boundaryPath.Edges.Add(lineEdge4);
            hatch.Paths.Add(boundaryPath);

            var renderData = HatchEntityRenderer.Render(hatch);
            var json = JsonSerializer.Serialize(renderData);
            var deserializedData = JsonSerializer.Deserialize<HatchEntityRenderer.HatchData>(json);

            Assert.NotNull(deserializedData);
            Assert.True(deserializedData.IsInvisible);
            Assert.False(deserializedData.Visible);
        }
    }
}
