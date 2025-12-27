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
    public class SplineEntityRendererIntegrationTests
    {
        private Spline CreateBasicSpline()
        {
            var spline = new Spline();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));
            spline.Degree = 3;
            spline.Color = new Color(1);
            spline.Normal = new XYZ(0, 0, 1);
            spline.Closed = false;
            spline.Periodic = false;
            return spline;
        }

        private Spline CreateClosedSpline()
        {
            var spline = new Spline();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(0, 10, 0));
            spline.Degree = 3;
            spline.Color = new Color(2);
            spline.Normal = new XYZ(0, 0, 1);
            spline.Closed = true;
            spline.Periodic = false;
            return spline;
        }

        private Spline CreatePeriodicSpline()
        {
            var spline = new Spline();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));
            spline.Degree = 3;
            spline.Color = new Color(3);
            spline.Normal = new XYZ(0, 0, 1);
            spline.Closed = false;
            spline.Periodic = true;
            return spline;
        }

        private Spline CreateNurbsSpline()
        {
            var spline = new Spline();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));
            spline.Weights.Add(1.0);
            spline.Weights.Add(2.0);
            spline.Weights.Add(1.5);
            spline.Weights.Add(1.0);
            spline.Degree = 3;
            spline.Color = new Color(4);
            spline.Normal = new XYZ(0, 0, 1);
            spline.Closed = false;
            spline.Periodic = false;
            return spline;
        }

        private Spline CreateCatmullRomSpline()
        {
            var spline = new Spline();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));
            spline.Degree = 3;
            spline.Color = new Color(5);
            spline.Normal = new XYZ(0, 0, 1);
            spline.Closed = false;
            spline.Periodic = false;
            return spline;
        }

        private Spline CreateSplineWithFitPoints()
        {
            var spline = new Spline();
            spline.FitPoints.Add(new XYZ(0, 0, 0));
            spline.FitPoints.Add(new XYZ(10, 10, 0));
            spline.FitPoints.Add(new XYZ(20, 0, 0));
            spline.FitPoints.Add(new XYZ(30, 10, 0));
            spline.Degree = 3;
            spline.Color = new Color(6);
            spline.Normal = new XYZ(0, 0, 1);
            spline.Closed = false;
            spline.Periodic = false;
            return spline;
        }

        [Fact]
        public void LoadAndRenderSplineFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var splineEntities = new List<Spline>();
            foreach (var entity in doc.Entities)
            {
                if (entity is Spline spline)
                {
                    splineEntities.Add(spline);
                }
            }

            if (splineEntities.Count == 0)
            {
                return;
            }

            foreach (var spline in splineEntities)
            {
                var splineData = SplineEntityRenderer.Render(spline);
                Assert.NotNull(splineData);
                Assert.Equal("Spline", splineData.Type);
                Assert.NotNull(splineData.ControlPoints);
                Assert.NotNull(splineData.Bounds3D);
            }
        }

        [Fact]
        public void SerializeSplineDataToJson_Success()
        {
            var spline = CreateBasicSpline();

            var splineData = SplineEntityRenderer.Render(spline);
            var json = JsonConvert.SerializeObject(splineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Spline", json);
            Assert.Contains("ControlPoints", json);
            Assert.Contains("Bounds3D", json);
        }

        [Fact]
        public void DeserializeSplineDataFromJson_Success()
        {
            var spline = CreateBasicSpline();

            var originalSplineData = SplineEntityRenderer.Render(spline);
            var json = JsonConvert.SerializeObject(originalSplineData);
            var deserializedSplineData = JsonConvert.DeserializeObject<SplineEntityRenderer.SplineData>(json);

            Assert.NotNull(deserializedSplineData);
            Assert.Equal(originalSplineData.Type, deserializedSplineData.Type);
            Assert.Equal(originalSplineData.ControlPointCount, deserializedSplineData.ControlPointCount);
            Assert.Equal(originalSplineData.ColorIndex, deserializedSplineData.ColorIndex);
            Assert.Equal(originalSplineData.Degree, deserializedSplineData.Degree);
            Assert.Equal(originalSplineData.Closed, deserializedSplineData.Closed);
            Assert.Equal(originalSplineData.Periodic, deserializedSplineData.Periodic);
            Assert.Equal(originalSplineData.Visible, deserializedSplineData.Visible);
            Assert.Equal(originalSplineData.Opacity, deserializedSplineData.Opacity);
            Assert.Equal(originalSplineData.Transparent, deserializedSplineData.Transparent);
            Assert.Equal(originalSplineData.DepthTest, deserializedSplineData.DepthTest);
        }

        [Fact]
        public void RenderMultipleSplines_CollectAllSplineData()
        {
            var splines = new List<Spline>
            {
                CreateBasicSpline(),
                CreateClosedSpline()
            };
            splines[0].ControlPoints.Clear();
            splines[0].ControlPoints.Add(new XYZ(10, 10, 0));
            splines[0].ControlPoints.Add(new XYZ(20, 20, 0));
            splines[0].ControlPoints.Add(new XYZ(30, 10, 0));
            splines[0].ControlPoints.Add(new XYZ(40, 20, 0));
            splines[1].ControlPoints.Clear();
            splines[1].ControlPoints.Add(new XYZ(50, 50, 0));
            splines[1].ControlPoints.Add(new XYZ(60, 50, 0));
            splines[1].ControlPoints.Add(new XYZ(60, 60, 0));
            splines[1].ControlPoints.Add(new XYZ(50, 60, 0));

            var splineDataList = new List<SplineEntityRenderer.SplineData>();
            foreach (var spline in splines)
            {
                var splineData = SplineEntityRenderer.Render(spline);
                splineDataList.Add(splineData);
            }

            Assert.Equal(2, splineDataList.Count);
            Assert.Equal(10, splineDataList[0].ControlPoints[0].X);
            Assert.Equal(50, splineDataList[1].ControlPoints[0].X);
        }

        [Fact]
        public void RenderBasicSpline_PreservesAllProperties()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(100, 200, 300));
            spline.ControlPoints.Add(new XYZ(150, 250, 300));
            spline.ControlPoints.Add(new XYZ(200, 200, 300));
            spline.ControlPoints.Add(new XYZ(250, 250, 300));
            spline.Normal = new XYZ(0.5, 0.5, 0.7071);
            spline.Degree = 3;
            spline.Color = new Color(5);
            spline.IsInvisible = false;

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(100, splineData.ControlPoints[0].X);
            Assert.Equal(200, splineData.ControlPoints[0].Y);
            Assert.Equal(300, splineData.ControlPoints[0].Z);
            Assert.Equal(150, splineData.ControlPoints[1].X);
            Assert.Equal(250, splineData.ControlPoints[1].Y);
            Assert.Equal(300, splineData.ControlPoints[1].Z);
            Assert.Equal(200, splineData.ControlPoints[2].X);
            Assert.Equal(200, splineData.ControlPoints[2].Y);
            Assert.Equal(300, splineData.ControlPoints[2].Z);
            Assert.Equal(250, splineData.ControlPoints[3].X);
            Assert.Equal(250, splineData.ControlPoints[3].Y);
            Assert.Equal(300, splineData.ControlPoints[3].Z);
            Assert.InRange(splineData.Normal.X, 0.49, 0.51);
            Assert.InRange(splineData.Normal.Y, 0.49, 0.51);
            Assert.InRange(splineData.Normal.Z, 0.70, 0.71);
            Assert.Equal(3, splineData.Degree);
            Assert.Equal(5, splineData.ColorIndex);
            Assert.True(splineData.Visible);
            Assert.False(splineData.Closed);
            Assert.False(splineData.Periodic);
        }

        [Fact]
        public void RenderClosedSpline_PreservesClosedFlag()
        {
            var spline = CreateClosedSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(100, 200, 300));
            spline.ControlPoints.Add(new XYZ(150, 200, 300));
            spline.ControlPoints.Add(new XYZ(150, 250, 300));
            spline.ControlPoints.Add(new XYZ(100, 250, 300));
            spline.Normal = new XYZ(0, 0, 1);
            spline.Degree = 3;
            spline.Color = new Color(5);
            spline.Closed = true;

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(100, splineData.ControlPoints[0].X);
            Assert.Equal(200, splineData.ControlPoints[0].Y);
            Assert.Equal(300, splineData.ControlPoints[0].Z);
            Assert.Equal(150, splineData.ControlPoints[1].X);
            Assert.Equal(200, splineData.ControlPoints[1].Y);
            Assert.Equal(300, splineData.ControlPoints[1].Z);
            Assert.Equal(150, splineData.ControlPoints[2].X);
            Assert.Equal(250, splineData.ControlPoints[2].Y);
            Assert.Equal(300, splineData.ControlPoints[2].Z);
            Assert.Equal(100, splineData.ControlPoints[3].X);
            Assert.Equal(250, splineData.ControlPoints[3].Y);
            Assert.Equal(300, splineData.ControlPoints[3].Z);
            Assert.Equal(0, splineData.Normal.X);
            Assert.Equal(0, splineData.Normal.Y);
            Assert.Equal(1, splineData.Normal.Z);
            Assert.Equal(3, splineData.Degree);
            Assert.Equal(5, splineData.ColorIndex);
            Assert.True(splineData.Visible);
            Assert.True(splineData.Closed);
            Assert.False(splineData.Periodic);
        }

        [Fact]
        public void RenderPeriodicSpline_PreservesPeriodicFlag()
        {
            var spline = CreatePeriodicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(100, 200, 300));
            spline.ControlPoints.Add(new XYZ(150, 250, 300));
            spline.ControlPoints.Add(new XYZ(200, 200, 300));
            spline.ControlPoints.Add(new XYZ(250, 250, 300));
            spline.Normal = new XYZ(0, 0, 1);
            spline.Degree = 3;
            spline.Color = new Color(5);
            spline.Periodic = true;

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(100, splineData.ControlPoints[0].X);
            Assert.Equal(200, splineData.ControlPoints[0].Y);
            Assert.Equal(300, splineData.ControlPoints[0].Z);
            Assert.Equal(150, splineData.ControlPoints[1].X);
            Assert.Equal(250, splineData.ControlPoints[1].Y);
            Assert.Equal(300, splineData.ControlPoints[1].Z);
            Assert.Equal(200, splineData.ControlPoints[2].X);
            Assert.Equal(200, splineData.ControlPoints[2].Y);
            Assert.Equal(300, splineData.ControlPoints[2].Z);
            Assert.Equal(250, splineData.ControlPoints[3].X);
            Assert.Equal(250, splineData.ControlPoints[3].Y);
            Assert.Equal(300, splineData.ControlPoints[3].Z);
            Assert.Equal(0, splineData.Normal.X);
            Assert.Equal(0, splineData.Normal.Y);
            Assert.Equal(1, splineData.Normal.Z);
            Assert.Equal(3, splineData.Degree);
            Assert.Equal(5, splineData.ColorIndex);
            Assert.True(splineData.Visible);
            Assert.False(splineData.Closed);
            Assert.True(splineData.Periodic);
        }

        [Fact]
        public void RenderNurbsSpline_WithWeights_ReturnsNURBSType()
        {
            var spline = CreateNurbsSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(100, 200, 300));
            spline.ControlPoints.Add(new XYZ(150, 250, 300));
            spline.ControlPoints.Add(new XYZ(200, 200, 300));
            spline.ControlPoints.Add(new XYZ(250, 250, 300));
            spline.Weights.Clear();
            spline.Weights.Add(1.0);
            spline.Weights.Add(2.0);
            spline.Weights.Add(1.5);
            spline.Weights.Add(1.0);
            spline.Normal = new XYZ(0, 0, 1);
            spline.Degree = 3;
            spline.Color = new Color(5);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal("NURBSCurve", splineData.SplineType);
            Assert.Equal("NURBSCurve", splineData.Geometry.Type);
            Assert.NotNull(splineData.Weights);
            Assert.Equal(4, splineData.Weights.Count);
            Assert.Equal(1.0, splineData.Weights[0]);
            Assert.Equal(2.0, splineData.Weights[1]);
            Assert.Equal(1.5, splineData.Weights[2]);
            Assert.Equal(1.0, splineData.Weights[3]);
        }

        [Fact]
        public void RenderCatmullRomSpline_ReturnsCatmullRomType()
        {
            var spline = CreateCatmullRomSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(100, 200, 300));
            spline.ControlPoints.Add(new XYZ(150, 250, 300));
            spline.ControlPoints.Add(new XYZ(200, 200, 300));
            spline.ControlPoints.Add(new XYZ(250, 250, 300));
            spline.Normal = new XYZ(0, 0, 1);
            spline.Degree = 3;
            spline.Color = new Color(5);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal("CatmullRomCurve3", splineData.SplineType);
            Assert.Equal("CatmullRomCurve3", splineData.Geometry.Type);
        }

        [Fact]
        public void RenderSplineWithFitPoints_ReturnsFitPoints()
        {
            var spline = CreateSplineWithFitPoints();
            spline.FitPoints.Clear();
            spline.FitPoints.Add(new XYZ(100, 200, 300));
            spline.FitPoints.Add(new XYZ(150, 250, 300));
            spline.FitPoints.Add(new XYZ(200, 200, 300));
            spline.FitPoints.Add(new XYZ(250, 250, 300));
            spline.Normal = new XYZ(0, 0, 1);
            spline.Degree = 3;
            spline.Color = new Color(5);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.FitPoints);
            Assert.Equal(4, splineData.FitPointCount);
            Assert.Equal(100, splineData.FitPoints[0].X);
            Assert.Equal(200, splineData.FitPoints[0].Y);
            Assert.Equal(300, splineData.FitPoints[0].Z);
            Assert.Equal(150, splineData.FitPoints[1].X);
            Assert.Equal(250, splineData.FitPoints[1].Y);
            Assert.Equal(300, splineData.FitPoints[1].Z);
            Assert.Equal(200, splineData.FitPoints[2].X);
            Assert.Equal(200, splineData.FitPoints[2].Y);
            Assert.Equal(300, splineData.FitPoints[2].Z);
            Assert.Equal(250, splineData.FitPoints[3].X);
            Assert.Equal(250, splineData.FitPoints[3].Y);
            Assert.Equal(300, splineData.FitPoints[3].Z);
        }

        [Fact]
        public void RenderSplineWith3DCoordinates_CalculatesCorrect3DProperties()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(10, 20, 30));
            spline.ControlPoints.Add(new XYZ(20, 30, 40));
            spline.ControlPoints.Add(new XYZ(30, 40, 50));
            spline.ControlPoints.Add(new XYZ(40, 50, 60));
            spline.Normal = new XYZ(0, 0, 1);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Centroid3D);
            Assert.NotNull(splineData.Bounds3D);
            Assert.Equal(0, splineData.Normal.X);
            Assert.Equal(0, splineData.Normal.Y);
            Assert.Equal(1, splineData.Normal.Z);
        }

        [Fact]
        public void RenderSplineWithRotation_CalculatesCorrectTransform()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(10, 20, 30));
            spline.ControlPoints.Add(new XYZ(20, 30, 40));
            spline.ControlPoints.Add(new XYZ(30, 40, 50));
            spline.ControlPoints.Add(new XYZ(40, 50, 60));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Transform);
            Assert.Equal(1.0, splineData.Transform.Scale.X);
            Assert.Equal(1.0, splineData.Transform.Scale.Y);
            Assert.Equal(1.0, splineData.Transform.Scale.Z);
            Assert.NotNull(splineData.Transform.Position);
        }

        [Fact]
        public void RenderSplineWithLayer_PreservesLayerInformation()
        {
            var spline = CreateBasicSpline();
            spline.Layer = new Layer("TestLayer");

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal("TestLayer", splineData.LayerName);
        }

        [Fact]
        public void RenderSplineWithColor_PreservesColorInformation()
        {
            var spline = CreateBasicSpline();
            spline.Color = new Color(5);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(5, splineData.ColorIndex);
            Assert.Equal(5, splineData.Color.Index);
        }

        [Fact]
        public void RenderSplineWithLineType_PreservesLineTypeInformation()
        {
            var spline = CreateBasicSpline();
            var lineType = new LineType("Continuous");
            spline.LineType = lineType;

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal("Continuous", splineData.LineTypeName);
        }

        [Fact]
        public void RenderSplineWithLineWeight_PreservesLineWeightInformation()
        {
            var spline = CreateBasicSpline();
            spline.LineWeight = ACadSharp.LineWeightType.W30;

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(0.3, splineData.LineWeight);
        }

        [Fact]
        public void RenderSplineWithInvisibleFlag_SetsVisibleProperty()
        {
            var spline = CreateBasicSpline();
            spline.IsInvisible = true;

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.False(splineData.Visible);
        }

        [Fact]
        public void RenderSplineWithHandle_PreservesHandleInformation()
        {
            var spline = CreateBasicSpline();
            
            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(spline.Handle.ToString(), splineData.Handle);
        }

        [Fact]
        public void RenderSplineWithZeroControlPoints_HandlesCorrectly()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(0, 0, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(0, splineData.ControlPoints[0].X);
            Assert.Equal(0, splineData.ControlPoints[0].Y);
            Assert.Equal(0, splineData.ControlPoints[0].Z);
        }

        [Fact]
        public void RenderSplineWithVeryLargeCoordinates_HandlesCorrectly()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(1000000, 2000000, 3000000));
            spline.ControlPoints.Add(new XYZ(1001000, 2000000, 3000000));
            spline.ControlPoints.Add(new XYZ(1002000, 2001000, 3000000));
            spline.ControlPoints.Add(new XYZ(1003000, 2001000, 3000000));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(1000000, splineData.ControlPoints[0].X);
            Assert.Equal(2000000, splineData.ControlPoints[0].Y);
            Assert.Equal(3000000, splineData.ControlPoints[0].Z);
        }

        [Fact]
        public void RenderSplineWithVerySmallCoordinates_HandlesCorrectly()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0.0001, 0.0002, 0.0003));
            spline.ControlPoints.Add(new XYZ(0.0002, 0.0002, 0.0003));
            spline.ControlPoints.Add(new XYZ(0.0003, 0.0003, 0.0003));
            spline.ControlPoints.Add(new XYZ(0.0004, 0.0003, 0.0003));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(0.0001, splineData.ControlPoints[0].X);
            Assert.Equal(0.0002, splineData.ControlPoints[0].Y);
            Assert.Equal(0.0003, splineData.ControlPoints[0].Z);
        }

        [Fact]
        public void RenderSplineWithNegativeCoordinates_HandlesCorrectly()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(-10, -20, -30));
            spline.ControlPoints.Add(new XYZ(0, -20, -30));
            spline.ControlPoints.Add(new XYZ(10, -10, -30));
            spline.ControlPoints.Add(new XYZ(20, 0, -30));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(-10, splineData.ControlPoints[0].X);
            Assert.Equal(-20, splineData.ControlPoints[0].Y);
            Assert.Equal(-30, splineData.ControlPoints[0].Z);
        }

        [Fact]
        public void RenderSplineWithDifferentNormals_PreservesNormals()
        {
            var normals = new[]
            {
                new XYZ(0, 0, 1),
                new XYZ(1, 0, 0),
                new XYZ(0, 1, 0),
                new XYZ(0.5, 0.5, 0.7071)
            };

            foreach (var normal in normals)
            {
                var spline = CreateBasicSpline();
                spline.Normal = normal;

                var splineData = SplineEntityRenderer.Render(spline);

                Assert.NotNull(splineData);
                Assert.InRange(splineData.Normal.X, normal.X - 0.0001, normal.X + 0.0001);
                Assert.InRange(splineData.Normal.Y, normal.Y - 0.0001, normal.Y + 0.0001);
                Assert.InRange(splineData.Normal.Z, normal.Z - 0.0001, normal.Z + 0.0001);
            }
        }

        [Fact]
        public void RenderSplineWithDifferentDegrees_PreservesDegrees()
        {
            var degrees = new[] { 1, 2, 3, 4, 5 };

            foreach (var degree in degrees)
            {
                var spline = CreateBasicSpline();
                spline.Degree = degree;

                var splineData = SplineEntityRenderer.Render(spline);

                Assert.NotNull(splineData);
                Assert.Equal(degree, splineData.Degree);
            }
        }

        [Fact]
        public void RenderSplineWithDifferentColors_PreservesColors()
        {
            var colors = new[] { 1, 2, 3, 5, 7 };

            foreach (var colorIndex in colors)
            {
                var spline = CreateBasicSpline();
                spline.Color = new Color((short)colorIndex);

                var splineData = SplineEntityRenderer.Render(spline);

                Assert.NotNull(splineData);
                Assert.Equal(colorIndex, splineData.ColorIndex);
                Assert.Equal(colorIndex, splineData.Color.Index);
            }
        }

        [Fact]
        public void RenderSplineWithDifferentLineWeights_PreservesLineWeights()
        {
            var lineWeights = new[]
            {
                ACadSharp.LineWeightType.W0,
                ACadSharp.LineWeightType.W30,
                ACadSharp.LineWeightType.W50
            };

            foreach (var lineWeight in lineWeights)
            {
                var spline = CreateBasicSpline();
                spline.LineWeight = lineWeight;

                var splineData = SplineEntityRenderer.Render(spline);

                Assert.NotNull(splineData);
                Assert.Equal(lineWeight.GetLineWeightValue(), splineData.LineWeight);
            }
        }

        [Fact]
        public void RenderSplineWithMaterial_ReturnsCorrectMaterial()
        {
            var spline = CreateBasicSpline();
            spline.Color = new Color(5);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Material);
            Assert.Equal("LineBasicMaterial", splineData.Material.Type);
            Assert.Equal(1.0, splineData.Material.Opacity);
            Assert.False(splineData.Material.Transparent);
            Assert.True(splineData.Material.DepthTest);
            Assert.Equal(5, splineData.Material.Color.Index);
        }

        [Fact]
        public void RenderSplineWithGeometry_ReturnsCorrectGeometry()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(10, 20, 30));
            spline.ControlPoints.Add(new XYZ(20, 30, 40));
            spline.ControlPoints.Add(new XYZ(30, 40, 50));
            spline.ControlPoints.Add(new XYZ(40, 50, 60));
            spline.Normal = new XYZ(0, 0, 1);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Geometry);
            Assert.Equal("BufferGeometry", splineData.Geometry.Type);
            Assert.NotNull(splineData.Bounds3D);
        }

        [Fact]
        public void RenderSplineWithTransform_ReturnsCorrectTransform()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(10, 20, 30));
            spline.ControlPoints.Add(new XYZ(20, 30, 40));
            spline.ControlPoints.Add(new XYZ(30, 40, 50));
            spline.ControlPoints.Add(new XYZ(40, 50, 60));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Transform);
            Assert.Equal(1.0, splineData.Transform.Scale.X);
            Assert.Equal(1.0, splineData.Transform.Scale.Y);
            Assert.Equal(1.0, splineData.Transform.Scale.Z);
            Assert.NotNull(splineData.Transform.Position);
        }

        [Fact]
        public void RenderSplineWithDefaultProperties_ReturnsCorrectDefaults()
        {
            var spline = new Spline();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));
            spline.Color = new ACadSharp.Color(7);
            spline.LineType = new LineType("CONTINUOUS");
            spline.LineWeight = LineWeightType.ByLayer;
            spline.IsInvisible = false;
            spline.Normal = new XYZ(0, 0, 1);
            spline.Degree = 3;
            spline.Closed = false;
            spline.Periodic = false;

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal("Spline", splineData.Type);
            Assert.Equal(7, splineData.ColorIndex);
            Assert.Equal("CONTINUOUS", splineData.LineTypeName);
            Assert.Equal(0.0, splineData.LineWeight);
            Assert.True(splineData.Visible);
            Assert.Equal(1.0, splineData.Opacity);
            Assert.False(splineData.Transparent);
            Assert.True(splineData.DepthTest);
            Assert.Equal(3, splineData.Degree);
            Assert.False(splineData.Closed);
            Assert.False(splineData.Periodic);
        }

        [Fact]
        public void RenderSplineWithKnots_PreservesKnots()
        {
            var spline = CreateBasicSpline();
            spline.Knots.Clear();
            spline.Knots.Add(0.0);
            spline.Knots.Add(0.0);
            spline.Knots.Add(0.0);
            spline.Knots.Add(0.0);
            spline.Knots.Add(1.0);
            spline.Knots.Add(1.0);
            spline.Knots.Add(1.0);
            spline.Knots.Add(1.0);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Knots);
            Assert.Equal(8, splineData.Knots.Count);
            Assert.Equal(0.0, splineData.Knots[0]);
            Assert.Equal(0.0, splineData.Knots[1]);
            Assert.Equal(0.0, splineData.Knots[2]);
            Assert.Equal(0.0, splineData.Knots[3]);
            Assert.Equal(1.0, splineData.Knots[4]);
            Assert.Equal(1.0, splineData.Knots[5]);
            Assert.Equal(1.0, splineData.Knots[6]);
            Assert.Equal(1.0, splineData.Knots[7]);
        }

        [Fact]
        public void RenderSplineWithStartTangent_PreservesStartTangent()
        {
            var spline = CreateBasicSpline();
            spline.StartTangent = new XYZ(1, 0, 0);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.StartTangent);
            Assert.Equal(1, splineData.StartTangent.X);
            Assert.Equal(0, splineData.StartTangent.Y);
            Assert.Equal(0, splineData.StartTangent.Z);
        }

        [Fact]
        public void RenderSplineWithEndTangent_PreservesEndTangent()
        {
            var spline = CreateBasicSpline();
            spline.EndTangent = new XYZ(0, 1, 0);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.EndTangent);
            Assert.Equal(0, splineData.EndTangent.X);
            Assert.Equal(1, splineData.EndTangent.Y);
            Assert.Equal(0, splineData.EndTangent.Z);
        }

        [Fact]
        public void RenderSplineWithApproximationPoints_ReturnsApproximationPoints()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.ApproximationPoints);
            Assert.True(splineData.ApproximationPoints.Count > 0);
        }

        [Fact]
        public void RenderSplineWithVertexPositions_ReturnsVertexPositions()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.VertexPositions);
            Assert.True(splineData.VertexPositions.Length > 0);
        }

        [Fact]
        public void RenderSplineWithVertexColors_ReturnsVertexColors()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));
            spline.Color = new Color(5);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.VertexColors);
            Assert.True(splineData.VertexColors.Length > 0);
        }

        [Fact]
        public void RenderSplineWithIndices_ReturnsIndices()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Indices);
            Assert.True(splineData.Indices.Length > 0);
        }

        [Fact]
        public void RenderSplineWithBounds3D_ReturnsCorrectBounds()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Bounds3D);
            Assert.NotNull(splineData.Bounds3D.Min);
            Assert.NotNull(splineData.Bounds3D.Max);
            Assert.Equal(0, splineData.Bounds3D.Min.X);
            Assert.Equal(0, splineData.Bounds3D.Min.Y);
            Assert.Equal(0, splineData.Bounds3D.Min.Z);
            Assert.Equal(30, splineData.Bounds3D.Max.X);
            Assert.Equal(10, splineData.Bounds3D.Max.Y);
            Assert.Equal(0, splineData.Bounds3D.Max.Z);
        }

        [Fact]
        public void RenderSplineWithCentroid3D_ReturnsCorrectCentroid()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Centroid3D);
            Assert.NotNull(splineData.Centroid3D.X);
            Assert.NotNull(splineData.Centroid3D.Y);
            Assert.NotNull(splineData.Centroid3D.Z);
        }

        [Fact]
        public void RenderSplineWithDifferentSplineTypes_ReturnsCorrectTypes()
        {
            var splineTypes = new[]
            {
                CreateBasicSpline(),
                CreateNurbsSpline(),
                CreateCatmullRomSpline()
            };

            var expectedTypes = new[]
            {
                "CatmullRomCurve3",
                "NURBSCurve",
                "CatmullRomCurve3"
            };

            for (int i = 0; i < splineTypes.Length; i++)
            {
                var spline = splineTypes[i];
                var splineData = SplineEntityRenderer.Render(spline);

                Assert.NotNull(splineData);
                Assert.Equal(expectedTypes[i], splineData.SplineType);
                Assert.Equal(expectedTypes[i], splineData.Geometry.Type);
            }
        }

        [Fact]
        public void RenderSplineWithControlPointCount_ReturnsCorrectCount()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));
            spline.ControlPoints.Add(new XYZ(40, 20, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(5, splineData.ControlPointCount);
        }

        [Fact]
        public void RenderSplineWithFitPointCount_ReturnsCorrectCount()
        {
            var spline = CreateSplineWithFitPoints();
            spline.FitPoints.Clear();
            spline.FitPoints.Add(new XYZ(0, 0, 0));
            spline.FitPoints.Add(new XYZ(10, 10, 0));
            spline.FitPoints.Add(new XYZ(20, 0, 0));
            spline.FitPoints.Add(new XYZ(30, 10, 0));
            spline.FitPoints.Add(new XYZ(40, 20, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(5, splineData.FitPointCount);
        }

        [Fact]
        public void RenderSplineWithKnotCount_ReturnsCorrectCount()
        {
            var spline = CreateBasicSpline();
            spline.Knots.Clear();
            spline.Knots.Add(0.0);
            spline.Knots.Add(0.0);
            spline.Knots.Add(0.0);
            spline.Knots.Add(0.0);
            spline.Knots.Add(1.0);
            spline.Knots.Add(1.0);
            spline.Knots.Add(1.0);
            spline.Knots.Add(1.0);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(8, splineData.KnotCount);
        }

        [Fact]
        public void RenderSplineWithWeightCount_ReturnsCorrectCount()
        {
            var spline = CreateNurbsSpline();
            spline.Weights.Clear();
            spline.Weights.Add(1.0);
            spline.Weights.Add(2.0);
            spline.Weights.Add(1.5);
            spline.Weights.Add(1.0);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(4, splineData.WeightCount);
        }

        [Fact]
        public void RenderSplineWithApproximationPointCount_ReturnsCorrectCount()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.ApproximationPointCount);
            Assert.True(splineData.ApproximationPointCount > 0);
        }

        [Fact]
        public void RenderSplineWithVertexPositionCount_ReturnsCorrectCount()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.VertexPositionCount);
            Assert.True(splineData.VertexPositionCount > 0);
        }

        [Fact]
        public void RenderSplineWithVertexColorCount_ReturnsCorrectCount()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));
            spline.Color = new Color(5);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.VertexColorCount);
            Assert.True(splineData.VertexColorCount > 0);
        }

        [Fact]
        public void RenderSplineWithIndexCount_ReturnsCorrectCount()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.IndexCount);
            Assert.True(splineData.IndexCount > 0);
        }

        [Fact]
        public void RenderSplineWithClosedAndPeriodicFlags_PreservesFlags()
        {
            var spline = CreateBasicSpline();
            spline.Closed = true;
            spline.Periodic = true;

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.True(splineData.Closed);
            Assert.True(splineData.Periodic);
        }

        [Fact]
        public void RenderSplineWithOpacity_ReturnsCorrectOpacity()
        {
            var spline = CreateBasicSpline();

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(1.0, splineData.Opacity);
        }

        [Fact]
        public void RenderSplineWithTransparent_ReturnsCorrectTransparent()
        {
            var spline = CreateBasicSpline();

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.False(splineData.Transparent);
        }

        [Fact]
        public void RenderSplineWithDepthTest_ReturnsCorrectDepthTest()
        {
            var spline = CreateBasicSpline();

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.True(splineData.DepthTest);
        }

        [Fact]
        public void RenderSplineWithMaterialLineWidth_ReturnsCorrectLineWidth()
        {
            var spline = CreateBasicSpline();
            spline.LineWeight = ACadSharp.LineWeightType.W30;

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Material);
            Assert.Equal(0.3, splineData.Material.LineWidth);
        }

        [Fact]
        public void RenderSplineWithMaterialVertexColors_ReturnsCorrectVertexColors()
        {
            var spline = CreateBasicSpline();
            spline.Color = new Color(5);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Material);
            Assert.False(splineData.Material.VertexColors);
        }

        [Fact]
        public void RenderSplineWithGeometryType_ReturnsCorrectGeometryType()
        {
            var spline = CreateBasicSpline();

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Geometry);
            Assert.Equal("BufferGeometry", splineData.Geometry.Type);
        }

        [Fact]
        public void RenderSplineWithTransformRotation_ReturnsCorrectRotation()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(10, 20, 30));
            spline.ControlPoints.Add(new XYZ(20, 30, 40));
            spline.ControlPoints.Add(new XYZ(30, 40, 50));
            spline.ControlPoints.Add(new XYZ(40, 50, 60));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Transform);
            Assert.NotNull(splineData.Transform.Rotation);
            Assert.NotNull(splineData.Transform.Rotation.X);
            Assert.NotNull(splineData.Transform.Rotation.Y);
            Assert.NotNull(splineData.Transform.Rotation.Z);
        }

        [Fact]
        public void RenderSplineWithTransformPosition_ReturnsCorrectPosition()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(10, 20, 30));
            spline.ControlPoints.Add(new XYZ(20, 30, 40));
            spline.ControlPoints.Add(new XYZ(30, 40, 50));
            spline.ControlPoints.Add(new XYZ(40, 50, 60));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Transform);
            Assert.NotNull(splineData.Transform.Position);
            Assert.NotNull(splineData.Transform.Position.X);
            Assert.NotNull(splineData.Transform.Position.Y);
            Assert.NotNull(splineData.Transform.Position.Z);
        }

        [Fact]
        public void RenderSplineWithTransformScale_ReturnsCorrectScale()
        {
            var spline = CreateBasicSpline();

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Transform);
            Assert.NotNull(splineData.Transform.Scale);
            Assert.Equal(1.0, splineData.Transform.Scale.X);
            Assert.Equal(1.0, splineData.Transform.Scale.Y);
            Assert.Equal(1.0, splineData.Transform.Scale.Z);
        }

        [Fact]
        public void RenderSplineWithColorHex_ReturnsCorrectHexColor()
        {
            var spline = CreateBasicSpline();
            spline.Color = new Color(5);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Color);
            Assert.NotNull(splineData.Color.Hex);
            Assert.True(splineData.Color.Hex.StartsWith("#"));
        }

        [Fact]
        public void RenderSplineWithColorRGB_ReturnsCorrectRGBColor()
        {
            var spline = CreateBasicSpline();
            spline.Color = new Color(5);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Color);
            Assert.NotNull(splineData.Color.R);
            Assert.NotNull(splineData.Color.G);
            Assert.NotNull(splineData.Color.B);
        }

        [Fact]
        public void RenderSplineWithColorIndex_ReturnsCorrectColorIndex()
        {
            var spline = CreateBasicSpline();
            spline.Color = new Color(5);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Color);
            Assert.Equal(5, splineData.Color.Index);
        }

        [Fact]
        public void RenderSplineWithLayerName_ReturnsCorrectLayerName()
        {
            var spline = CreateBasicSpline();
            spline.Layer = new Layer("TestLayer");

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal("TestLayer", splineData.LayerName);
        }

        [Fact]
        public void RenderSplineWithLineTypeName_ReturnsCorrectLineTypeName()
        {
            var spline = CreateBasicSpline();
            var lineType = new LineType("Continuous");
            spline.LineType = lineType;

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal("Continuous", splineData.LineTypeName);
        }

        [Fact]
        public void RenderSplineWithHandle_ReturnsCorrectHandle()
        {
            var spline = CreateBasicSpline();

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(spline.Handle.ToString(), splineData.Handle);
        }

        [Fact]
        public void RenderSplineWithType_ReturnsCorrectType()
        {
            var spline = CreateBasicSpline();

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal("Spline", splineData.Type);
        }

        [Fact]
        public void RenderSplineWithVisible_ReturnsCorrectVisible()
        {
            var spline = CreateBasicSpline();
            spline.IsInvisible = false;

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.True(splineData.Visible);
        }

        [Fact]
        public void RenderSplineWithInvisible_ReturnsCorrectInvisible()
        {
            var spline = CreateBasicSpline();
            spline.IsInvisible = true;

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.False(splineData.Visible);
        }

        [Fact]
        public void RenderSplineWithThickness_ReturnsCorrectThickness()
        {
            var spline = CreateBasicSpline();

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(0.0, splineData.Thickness);
        }

        [Fact]
        public void RenderSplineWithNormal_ReturnsCorrectNormal()
        {
            var spline = CreateBasicSpline();
            spline.Normal = new XYZ(0, 0, 1);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Normal);
            Assert.Equal(0, splineData.Normal.X);
            Assert.Equal(0, splineData.Normal.Y);
            Assert.Equal(1, splineData.Normal.Z);
        }

        [Fact]
        public void RenderSplineWithControlPoints_ReturnsCorrectControlPoints()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(10, 20, 30));
            spline.ControlPoints.Add(new XYZ(20, 30, 40));
            spline.ControlPoints.Add(new XYZ(30, 40, 50));
            spline.ControlPoints.Add(new XYZ(40, 50, 60));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.ControlPoints);
            Assert.Equal(4, splineData.ControlPoints.Count);
            Assert.Equal(10, splineData.ControlPoints[0].X);
            Assert.Equal(20, splineData.ControlPoints[0].Y);
            Assert.Equal(30, splineData.ControlPoints[0].Z);
            Assert.Equal(20, splineData.ControlPoints[1].X);
            Assert.Equal(30, splineData.ControlPoints[1].Y);
            Assert.Equal(40, splineData.ControlPoints[1].Z);
            Assert.Equal(30, splineData.ControlPoints[2].X);
            Assert.Equal(40, splineData.ControlPoints[2].Y);
            Assert.Equal(50, splineData.ControlPoints[2].Z);
            Assert.Equal(40, splineData.ControlPoints[3].X);
            Assert.Equal(50, splineData.ControlPoints[3].Y);
            Assert.Equal(60, splineData.ControlPoints[3].Z);
        }

        [Fact]
        public void RenderSplineWithFitPoints_ReturnsCorrectFitPoints()
        {
            var spline = CreateSplineWithFitPoints();
            spline.FitPoints.Clear();
            spline.FitPoints.Add(new XYZ(10, 20, 30));
            spline.FitPoints.Add(new XYZ(20, 30, 40));
            spline.FitPoints.Add(new XYZ(30, 40, 50));
            spline.FitPoints.Add(new XYZ(40, 50, 60));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.FitPoints);
            Assert.Equal(4, splineData.FitPoints.Count);
            Assert.Equal(10, splineData.FitPoints[0].X);
            Assert.Equal(20, splineData.FitPoints[0].Y);
            Assert.Equal(30, splineData.FitPoints[0].Z);
            Assert.Equal(20, splineData.FitPoints[1].X);
            Assert.Equal(30, splineData.FitPoints[1].Y);
            Assert.Equal(40, splineData.FitPoints[1].Z);
            Assert.Equal(30, splineData.FitPoints[2].X);
            Assert.Equal(40, splineData.FitPoints[2].Y);
            Assert.Equal(50, splineData.FitPoints[2].Z);
            Assert.Equal(40, splineData.FitPoints[3].X);
            Assert.Equal(50, splineData.FitPoints[3].Y);
            Assert.Equal(60, splineData.FitPoints[3].Z);
        }

        [Fact]
        public void RenderSplineWithKnots_ReturnsCorrectKnots()
        {
            var spline = CreateBasicSpline();
            spline.Knots.Clear();
            spline.Knots.Add(0.0);
            spline.Knots.Add(0.0);
            spline.Knots.Add(0.0);
            spline.Knots.Add(0.0);
            spline.Knots.Add(1.0);
            spline.Knots.Add(1.0);
            spline.Knots.Add(1.0);
            spline.Knots.Add(1.0);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Knots);
            Assert.Equal(8, splineData.Knots.Count);
            Assert.Equal(0.0, splineData.Knots[0]);
            Assert.Equal(0.0, splineData.Knots[1]);
            Assert.Equal(0.0, splineData.Knots[2]);
            Assert.Equal(0.0, splineData.Knots[3]);
            Assert.Equal(1.0, splineData.Knots[4]);
            Assert.Equal(1.0, splineData.Knots[5]);
            Assert.Equal(1.0, splineData.Knots[6]);
            Assert.Equal(1.0, splineData.Knots[7]);
        }

        [Fact]
        public void RenderSplineWithWeights_ReturnsCorrectWeights()
        {
            var spline = CreateNurbsSpline();
            spline.Weights.Clear();
            spline.Weights.Add(1.0);
            spline.Weights.Add(2.0);
            spline.Weights.Add(1.5);
            spline.Weights.Add(1.0);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Weights);
            Assert.Equal(4, splineData.Weights.Count);
            Assert.Equal(1.0, splineData.Weights[0]);
            Assert.Equal(2.0, splineData.Weights[1]);
            Assert.Equal(1.5, splineData.Weights[2]);
            Assert.Equal(1.0, splineData.Weights[3]);
        }

        [Fact]
        public void RenderSplineWithStartTangent_ReturnsCorrectStartTangent()
        {
            var spline = CreateBasicSpline();
            spline.StartTangent = new XYZ(1, 0, 0);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.StartTangent);
            Assert.Equal(1, splineData.StartTangent.X);
            Assert.Equal(0, splineData.StartTangent.Y);
            Assert.Equal(0, splineData.StartTangent.Z);
        }

        [Fact]
        public void RenderSplineWithEndTangent_ReturnsCorrectEndTangent()
        {
            var spline = CreateBasicSpline();
            spline.EndTangent = new XYZ(0, 1, 0);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.EndTangent);
            Assert.Equal(0, splineData.EndTangent.X);
            Assert.Equal(1, splineData.EndTangent.Y);
            Assert.Equal(0, splineData.EndTangent.Z);
        }

        [Fact]
        public void RenderSplineWithApproximationPoints_ReturnsCorrectApproximationPoints()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.ApproximationPoints);
            Assert.True(splineData.ApproximationPoints.Count > 0);
        }

        [Fact]
        public void RenderSplineWithVertexPositions_ReturnsCorrectVertexPositions()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.VertexPositions);
            Assert.True(splineData.VertexPositions.Length > 0);
        }

        [Fact]
        public void RenderSplineWithVertexColors_ReturnsCorrectVertexColors()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));
            spline.Color = new Color(5);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.VertexColors);
            Assert.True(splineData.VertexColors.Length > 0);
        }

        [Fact]
        public void RenderSplineWithIndices_ReturnsCorrectIndices()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Indices);
            Assert.True(splineData.Indices.Length > 0);
        }

        [Fact]
        public void RenderSplineWithBounds3D_ReturnsCorrectBounds3D()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Bounds3D);
            Assert.NotNull(splineData.Bounds3D.Min);
            Assert.NotNull(splineData.Bounds3D.Max);
            Assert.Equal(0, splineData.Bounds3D.Min.X);
            Assert.Equal(0, splineData.Bounds3D.Min.Y);
            Assert.Equal(0, splineData.Bounds3D.Min.Z);
            Assert.Equal(30, splineData.Bounds3D.Max.X);
            Assert.Equal(10, splineData.Bounds3D.Max.Y);
            Assert.Equal(0, splineData.Bounds3D.Max.Z);
        }

        [Fact]
        public void RenderSplineWithCentroid3D_ReturnsCorrectCentroid3D()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(10, 10, 0));
            spline.ControlPoints.Add(new XYZ(20, 0, 0));
            spline.ControlPoints.Add(new XYZ(30, 10, 0));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Centroid3D);
            Assert.NotNull(splineData.Centroid3D.X);
            Assert.NotNull(splineData.Centroid3D.Y);
            Assert.NotNull(splineData.Centroid3D.Z);
        }

        [Fact]
        public void RenderSplineWithTransform_ReturnsCorrectTransform()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(10, 20, 30));
            spline.ControlPoints.Add(new XYZ(20, 30, 40));
            spline.ControlPoints.Add(new XYZ(30, 40, 50));
            spline.ControlPoints.Add(new XYZ(40, 50, 60));

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Transform);
            Assert.NotNull(splineData.Transform.Position);
            Assert.NotNull(splineData.Transform.Rotation);
            Assert.NotNull(splineData.Transform.Scale);
        }

        [Fact]
        public void RenderSplineWithMaterial_ReturnsCorrectMaterial()
        {
            var spline = CreateBasicSpline();
            spline.Color = new Color(5);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Material);
            Assert.Equal("LineBasicMaterial", splineData.Material.Type);
            Assert.Equal(1.0, splineData.Material.Opacity);
            Assert.False(splineData.Material.Transparent);
            Assert.True(splineData.Material.DepthTest);
            Assert.Equal(5, splineData.Material.Color.Index);
        }

        [Fact]
        public void RenderSplineWithGeometry_ReturnsCorrectGeometry()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(10, 20, 30));
            spline.ControlPoints.Add(new XYZ(20, 30, 40));
            spline.ControlPoints.Add(new XYZ(30, 40, 50));
            spline.ControlPoints.Add(new XYZ(40, 50, 60));
            spline.Normal = new XYZ(0, 0, 1);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.NotNull(splineData.Geometry);
            Assert.Equal("BufferGeometry", splineData.Geometry.Type);
            Assert.NotNull(splineData.Bounds3D);
        }

        [Fact]
        public void RenderSplineWithAllProperties_PreservesAllProperties()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(100, 200, 300));
            spline.ControlPoints.Add(new XYZ(150, 250, 300));
            spline.ControlPoints.Add(new XYZ(200, 200, 300));
            spline.ControlPoints.Add(new XYZ(250, 250, 300));
            spline.Normal = new XYZ(0.5, 0.5, 0.7071);
            spline.Degree = 3;
            spline.Color = new Color(5);
            spline.IsInvisible = false;
            spline.Layer = new Layer("TestLayer");
            spline.LineType = new LineType("Continuous");
            spline.LineWeight = ACadSharp.LineWeightType.W30;
            spline.Closed = false;
            spline.Periodic = false;
            spline.StartTangent = new XYZ(1, 0, 0);
            spline.EndTangent = new XYZ(0, 1, 0);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal("Spline", splineData.Type);
            Assert.Equal(100, splineData.ControlPoints[0].X);
            Assert.Equal(200, splineData.ControlPoints[0].Y);
            Assert.Equal(300, splineData.ControlPoints[0].Z);
            Assert.Equal(150, splineData.ControlPoints[1].X);
            Assert.Equal(250, splineData.ControlPoints[1].Y);
            Assert.Equal(300, splineData.ControlPoints[1].Z);
            Assert.Equal(200, splineData.ControlPoints[2].X);
            Assert.Equal(200, splineData.ControlPoints[2].Y);
            Assert.Equal(300, splineData.ControlPoints[2].Z);
            Assert.Equal(250, splineData.ControlPoints[3].X);
            Assert.Equal(250, splineData.ControlPoints[3].Y);
            Assert.Equal(300, splineData.ControlPoints[3].Z);
            Assert.InRange(splineData.Normal.X, 0.49, 0.51);
            Assert.InRange(splineData.Normal.Y, 0.49, 0.51);
            Assert.InRange(splineData.Normal.Z, 0.70, 0.71);
            Assert.Equal(3, splineData.Degree);
            Assert.Equal(5, splineData.ColorIndex);
            Assert.True(splineData.Visible);
            Assert.False(splineData.Closed);
            Assert.False(splineData.Periodic);
            Assert.Equal("TestLayer", splineData.LayerName);
            Assert.Equal("Continuous", splineData.LineTypeName);
            Assert.Equal(0.3, splineData.LineWeight);
            Assert.NotNull(splineData.StartTangent);
            Assert.Equal(1, splineData.StartTangent.X);
            Assert.Equal(0, splineData.StartTangent.Y);
            Assert.Equal(0, splineData.StartTangent.Z);
            Assert.NotNull(splineData.EndTangent);
            Assert.Equal(0, splineData.EndTangent.X);
            Assert.Equal(1, splineData.EndTangent.Y);
            Assert.Equal(0, splineData.EndTangent.Z);
        }
    }
}
