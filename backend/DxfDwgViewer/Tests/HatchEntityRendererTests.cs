using Xunit;
using ACadSharp.Entities;
using DxfDwgViewer.RenderUtilities;
using CSMath;
using System;
using System.Collections.Generic;

namespace DxfDwgViewer.Tests
{
    public class HatchEntityRendererTests
    {
        [Fact]
        public void Render_ValidSolidHatch_ReturnsValidHatchData()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.Equal("Hatch", result.Type);
            Assert.Equal("HATCH", result.EntityType);
            Assert.True(result.IsSolid);
            Assert.Equal(1, result.ColorIndex);
            Assert.Equal("#FF0000", result.ColorHex);
            Assert.Equal(255, result.ColorR);
            Assert.Equal(0, result.ColorG);
            Assert.Equal(0, result.ColorB);
        }

        [Fact]
        public void Render_PatternHatch_ReturnsValidHatchDataWithPattern()
        {
            var hatch = new Hatch
            {
                IsSolid = false,
                Pattern = new ACadSharp.Entities.HatchPattern("ANSI31"),
                PatternAngle = Math.PI / 4,
                PatternScale = 1.0,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(2)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.Equal("Hatch", result.Type);
            Assert.Equal("HATCH", result.EntityType);
            Assert.False(result.IsSolid);
            Assert.Equal("ANSI31", result.PatternName);
            Assert.Equal(Math.PI / 4, result.PatternAngle);
            Assert.Equal(1.0, result.PatternScale);
            Assert.Equal(2, result.ColorIndex);
            Assert.Equal("#FFFF00", result.ColorHex);
        }

        [Fact]
        public void Render_HatchWithCustomColor_ReturnsCorrectColorValues()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(3)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.Equal(3, result.ColorIndex);
            Assert.Equal("#00FF00", result.ColorHex);
            Assert.Equal(0, result.ColorR);
            Assert.Equal(255, result.ColorG);
            Assert.Equal(0, result.ColorB);
        }

        [Fact]
        public void Render_HatchWithBlueColor_ReturnsCorrectColorValues()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(5)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.Equal(5, result.ColorIndex);
            Assert.Equal("#0000FF", result.ColorHex);
            Assert.Equal(0, result.ColorR);
            Assert.Equal(0, result.ColorG);
            Assert.Equal(255, result.ColorB);
        }

        [Fact]
        public void Render_HatchWithTransparency_ReturnsCorrectOpacity()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                Transparency = new ACadSharp.Transparency(50)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.Equal(0.5, result.Transparency);
            Assert.Equal(0.5, result.MaterialOpacity);
            Assert.True(result.MaterialTransparent);
        }

        [Fact]
        public void Render_HatchWithElevation_ReturnsCorrectZCoordinates()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 10,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.Equal(10, result.Elevation);
            Assert.Equal(10, result.Centroid3D.Z);
            Assert.All(result.Points, p => Assert.Equal(10, p.Z));
        }

        [Fact]
        public void Render_HatchWithCustomNormal_ReturnsCorrectNormalVector()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = new XYZ(0, 0, 1),
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.Equal(0, result.NormalX);
            Assert.Equal(0, result.NormalY);
            Assert.Equal(1, result.NormalZ);
        }

        [Fact]
        public void Render_HatchWithMultiplePaths_ReturnsCorrectPathCount()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.Equal(2, result.PathCount);
        }

        [Fact]
        public void Render_HatchWithArcEdges_ReturnsCorrectEdgeTypes()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.Contains("Arc", result.EdgeTypes);
            Assert.Equal(1, result.TotalEdges);
        }

        [Fact]
        public void Render_HatchWithGradient_ReturnsCorrectGradientProperties()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1),
                GradientColor = new HatchGradientPattern { Name = "GRADIENT1" }
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.True(result.HasGradient);
            Assert.Equal("GRADIENT1", result.GradientColorName);
        }

        [Fact]
        public void Render_HatchWithDoublePattern_ReturnsCorrectDoubleFlag()
        {
            var hatch = new Hatch
            {
                IsSolid = false,
                Pattern = new ACadSharp.Entities.HatchPattern("ANSI31"),
                IsDouble = true,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.True(result.IsDouble);
        }

        [Fact]
        public void Render_Hatch_ReturnsCorrectTransformMatrix()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Matrix);
            Assert.Equal(16, result.Transform.Matrix.Length);
        }

        [Fact]
        public void Render_Hatch_ReturnsCorrectMaterialProperties()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.Equal("MeshBasicMaterial", result.MaterialType);
            Assert.True(result.MaterialDepthTest);
            Assert.True(result.MaterialDepthWrite);
            Assert.Equal(2, result.MaterialSide);
        }

        [Fact]
        public void Render_HatchWithPattern_ReturnsCorrectMaterialType()
        {
            var hatch = new Hatch
            {
                IsSolid = false,
                Pattern = new ACadSharp.Entities.HatchPattern("ANSI31"),
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.Equal("MeshPhongMaterial", result.MaterialType);
        }

        [Fact]
        public void Render_Hatch_ReturnsCorrectBounds()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.Equal(0, result.Bounds.Min.X);
            Assert.Equal(0, result.Bounds.Min.Y);
            Assert.Equal(0, result.Bounds.Min.Z);
            Assert.Equal(10, result.Bounds.Max.X);
            Assert.Equal(10, result.Bounds.Max.Y);
            Assert.Equal(0, result.Bounds.Max.Z);
        }

        [Fact]
        public void Render_Hatch_ReturnsCorrectCentroid()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);
            Assert.NotNull(result.Centroid3D);
            Assert.Equal(2.5, result.Centroid.X);
            Assert.Equal(2.5, result.Centroid.Y);
            Assert.Equal(2.5, result.Centroid3D.X);
            Assert.Equal(2.5, result.Centroid3D.Y);
            Assert.Equal(0, result.Centroid3D.Z);
        }

        [Fact]
        public void Render_Hatch_ReturnsCorrectArea()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.Equal(100, result.Area);
        }

        [Fact]
        public void Render_Hatch_ReturnsCorrectVerticesAndNormals()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.NotNull(result.Vertices);
            Assert.NotNull(result.Normals);
            Assert.True(result.Vertices.Count > 0);
            Assert.True(result.Normals.Count > 0);
        }

        [Fact]
        public void Render_Hatch_ReturnsCorrectIndices()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.NotNull(result.Indices);
            Assert.True(result.Indices.Count > 0);
        }

        [Fact]
        public void Render_Hatch_ReturnsCorrectCoordinateSystem()
        {
            var hatch = new Hatch
            {
                IsSolid = true,
                Pattern = ACadSharp.Entities.HatchPattern.Solid,
                Elevation = 0,
                Normal = XYZ.AxisZ,
                Color = new ACadSharp.Color(1)
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

            var result = HatchEntityRenderer.Render(hatch);

            Assert.NotNull(result);
            Assert.Equal("AutoCAD", result.CoordinateSystem);
            Assert.True(result.RequiresYAxisFlip);
        }
    }
}
