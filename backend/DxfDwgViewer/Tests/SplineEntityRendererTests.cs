using System;
using Xunit;
using ACadSharp.Entities;
using ACadSharp.Tables;
using ACadSharp;
using CSMath;
using DxfDwgViewer.RenderUtilities;

namespace DxfDwgViewer.Tests
{
    public class SplineEntityRendererTests
    {
        private Spline CreateBasicSpline()
        {
            var spline = new Spline();
            spline.ControlPoints.Add(new XYZ(0, 0, 0));
            spline.ControlPoints.Add(new XYZ(1, 0, 0));
            spline.ControlPoints.Add(new XYZ(1, 1, 0));
            spline.ControlPoints.Add(new XYZ(0, 1, 0));
            spline.Degree = 3;
            spline.Normal = new XYZ(0, 0, 1);
            spline.Color = new Color(256);
            spline.Thickness = 0.0;
            spline.StartTangent = new XYZ(1, 0, 0);
            spline.EndTangent = new XYZ(0, 1, 0);
            return spline;
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectType()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal("CatmullRomCurve3", result.SplineType);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectControlPoints()
        {
            var spline = CreateBasicSpline();
            spline.ControlPoints.Clear();
            spline.ControlPoints.Add(new XYZ(10, 20, 30));
            spline.ControlPoints.Add(new XYZ(40, 50, 60));
            spline.ControlPoints.Add(new XYZ(70, 80, 90));
            
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal(3, result.ControlPointCount);
            Assert.Equal(10, result.ControlPoints[0].X);
            Assert.Equal(20, result.ControlPoints[0].Y);
            Assert.Equal(30, result.ControlPoints[0].Z);
            Assert.Equal(40, result.ControlPoints[1].X);
            Assert.Equal(50, result.ControlPoints[1].Y);
            Assert.Equal(60, result.ControlPoints[1].Z);
            Assert.Equal(70, result.ControlPoints[2].X);
            Assert.Equal(80, result.ControlPoints[2].Y);
            Assert.Equal(90, result.ControlPoints[2].Z);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectFitPoints()
        {
            var spline = CreateBasicSpline();
            spline.FitPoints.Add(new XYZ(5, 10, 15));
            spline.FitPoints.Add(new XYZ(15, 20, 25));
            spline.FitPoints.Add(new XYZ(25, 30, 35));
            
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal(3, result.FitPointCount);
            Assert.Equal(5, result.FitPoints[0].X);
            Assert.Equal(10, result.FitPoints[0].Y);
            Assert.Equal(15, result.FitPoints[0].Z);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectDegree()
        {
            var spline = CreateBasicSpline();
            spline.Degree = 2;
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal(2, result.Degree);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectIsClosed()
        {
            var spline = CreateBasicSpline();
            spline.IsClosed = true;
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.True(result.IsClosed);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectIsPeriodic()
        {
            var spline = CreateBasicSpline();
            spline.IsPeriodic = true;
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.True(result.IsPeriodic);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectNormal()
        {
            var spline = CreateBasicSpline();
            spline.Normal = new XYZ(0.577, 0.577, 0.577);
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.InRange(result.Normal.X, 0.57, 0.58);
            Assert.InRange(result.Normal.Y, 0.57, 0.58);
            Assert.InRange(result.Normal.Z, 0.57, 0.58);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectStartTangent()
        {
            var spline = CreateBasicSpline();
            spline.StartTangent = new XYZ(0.707, 0.707, 0);
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.InRange(result.StartTangent.X, 0.70, 0.71);
            Assert.InRange(result.StartTangent.Y, 0.70, 0.71);
            Assert.Equal(0, result.StartTangent.Z);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectEndTangent()
        {
            var spline = CreateBasicSpline();
            spline.EndTangent = new XYZ(0, 0.707, 0.707);
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal(0, result.EndTangent.X);
            Assert.InRange(result.EndTangent.Y, 0.70, 0.71);
            Assert.InRange(result.EndTangent.Z, 0.70, 0.71);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectColorIndex()
        {
            var spline = CreateBasicSpline();
            spline.Color = new Color(1);
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal(1, result.ColorIndex);
        }

        [Fact]
        public void Render_Spline_CreatesColorCorrectly()
        {
            var spline = CreateBasicSpline();
            spline.Color = new Color(1);
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.NotNull(result.Color);

            Assert.Equal(1, result.Color.Index);
            Assert.Equal("#FF0000", result.Color.Hex);
            Assert.Equal((byte)255, result.Color.R);
            Assert.Equal((byte)0, result.Color.G);
            Assert.Equal((byte)0, result.Color.B);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectLineTypeName()
        {
            var spline = CreateBasicSpline();
            var lineType = new LineType("DASHED");
            spline.LineType = lineType;
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal("DASHED", result.LineTypeName);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectLineWeight()
        {
            var spline = CreateBasicSpline();
            spline.LineWeight = LineWeightType.W40;
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal(0.4, result.LineWeight);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectControlPointTolerance()
        {
            var spline = CreateBasicSpline();
            spline.ControlPointTolerance = 0.001;
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal(0.001, result.ControlPointTolerance);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectFitTolerance()
        {
            var spline = CreateBasicSpline();
            spline.FitTolerance = 0.01;
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal(0.01, result.FitTolerance);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectKnotTolerance()
        {
            var spline = CreateBasicSpline();
            spline.KnotTolerance = 0.0001;
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal(0.0001, result.KnotTolerance);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectKnots()
        {
            var spline = CreateBasicSpline();
            spline.Knots.Clear();
            spline.Knots.Add(0.0);
            spline.Knots.Add(0.25);
            spline.Knots.Add(0.5);
            spline.Knots.Add(0.75);
            spline.Knots.Add(1.0);
            
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal(5, result.KnotCount);
            Assert.Equal(0.0, result.Knots[0]);
            Assert.Equal(0.25, result.Knots[1]);
            Assert.Equal(0.5, result.Knots[2]);
            Assert.Equal(0.75, result.Knots[3]);
            Assert.Equal(1.0, result.Knots[4]);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectWeights()
        {
            var spline = CreateBasicSpline();
            spline.Weights.Clear();
            spline.Weights.Add(1.0);
            spline.Weights.Add(2.0);
            spline.Weights.Add(1.5);
            spline.Weights.Add(1.0);
            
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal(4, result.Weights.Count);
            Assert.Equal(1.0, result.Weights[0]);
            Assert.Equal(2.0, result.Weights[1]);
            Assert.Equal(1.5, result.Weights[2]);
            Assert.Equal(1.0, result.Weights[3]);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectIsRational()
        {
            var spline = CreateBasicSpline();
            spline.Weights.Add(1.0);
            spline.Weights.Add(2.0);
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.True(result.IsRational);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectApproximationPoints()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.NotNull(result.ApproximationPoints);
            Assert.True(result.ApproximationPointCount > 0);
        }

        [Fact]
        public void Render_Spline_CalculatesCorrectBounds()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.NotNull(result.Bounds);
            Assert.NotNull(result.Bounds.Min);
            Assert.NotNull(result.Bounds.Max);
            Assert.NotNull(result.Bounds.Center);
            Assert.NotNull(result.Bounds.Size);
        }

        [Fact]
        public void Render_Spline_CalculatesCorrectCentroid()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.NotNull(result.Centroid);
        }

        [Fact]
        public void Render_Spline_CalculatesCorrectLength()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectArcLength()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal(result.Length, result.ArcLength);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectSampleCount()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal(result.ApproximationPointCount, result.SampleCount);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectTension()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal(0.5, result.Tension);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectTransform()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.NotNull(result.Transform);
            Assert.NotNull(result.Transform.Position);
            Assert.NotNull(result.Transform.Rotation);
            Assert.NotNull(result.Transform.Scale);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectGeometry()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.NotNull(result.Geometry);
            Assert.Equal("CatmullRomCurve3", result.Geometry.Type);
            Assert.Equal(result.ApproximationPointCount, result.Geometry.VertexCount);
            Assert.True(result.Geometry.HasColors);
            Assert.True(result.Geometry.HasIndices);
            Assert.Equal("LineLoop", result.Geometry.PrimitiveType);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectMaterial()
        {
            var spline = CreateBasicSpline();
            spline.Color = new Color(1);
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.NotNull(result.Material);
            Assert.Equal("LineBasicMaterial", result.Material.Type);
            Assert.Equal(0xFF0000, result.Material.Color);
            Assert.Equal(1.0, result.Material.Opacity);
            Assert.False(result.Material.Transparent);
            Assert.False(result.Material.Wireframe);
            Assert.True(result.Material.VertexColors);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectVertexPositions()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexPositions);
            Assert.True(result.VertexPositions.Length > 0);
            Assert.Equal(result.ApproximationPointCount * 3, result.VertexPositions.Length);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectVertexColors()
        {
            var spline = CreateBasicSpline();
            spline.Color = new Color(1);
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.NotNull(result.VertexColors);
            Assert.True(result.VertexColors.Length > 0);
            Assert.Equal(result.ApproximationPointCount * 3, result.VertexColors.Length);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectIndices()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.NotNull(result.Indices);
            Assert.True(result.Indices.Length > 0);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectTangent()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.NotNull(result.Tangent);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectBinormal()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.NotNull(result.Binormal);
            Assert.Equal(3, result.Binormal.Length);
        }

        [Fact]
        public void Render_Spline_ReturnsCorrectUV()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.NotNull(result.UV);
            Assert.True(result.UV.Length > 0);
            Assert.Equal(result.ApproximationPointCount * 2, result.UV.Length);
        }

        [Fact]
        public void Render_Spline_CalculatesCorrectCurvature()
        {
            var spline = CreateBasicSpline();
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.True(result.Curvature >= 0);
        }

        [Fact]
        public void Render_Spline_WithWeights_ReturnsNURBSType()
        {
            var spline = CreateBasicSpline();
            spline.Weights.Add(1.0);
            spline.Weights.Add(2.0);
            spline.Weights.Add(1.5);
            spline.Weights.Add(1.0);
            
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.Equal("NURBSCurve", result.SplineType);
            Assert.Equal("NURBSCurve", result.Geometry.Type);
        }

        [Fact]
        public void Render_Spline_WithClosedSpline_CreatesClosingIndices()
        {
            var spline = CreateBasicSpline();
            spline.IsClosed = true;
            var result = SplineEntityRenderer.Render(spline);

            Assert.NotNull(result);
            Assert.NotNull(result.Indices);
            Assert.True(result.Indices.Length >= 2);
            Assert.Equal(result.ApproximationPointCount - 1, result.Indices[result.Indices.Length - 2]);
            Assert.Equal(0, result.Indices[result.Indices.Length - 1]);
        }

        [Fact]
        public void Render_Spline_WithAllPropertiesSet_PreservesAllProperties()
        {
            var spline = new Spline();
            spline.ControlPoints.Add(new XYZ(10, 20, 30));
            spline.ControlPoints.Add(new XYZ(40, 50, 60));
            spline.ControlPoints.Add(new XYZ(70, 80, 90));
            spline.FitPoints.Add(new XYZ(15, 25, 35));
            spline.FitPoints.Add(new XYZ(45, 55, 65));
            spline.Degree = 3;
            spline.IsClosed = true;
            spline.IsPeriodic = false;
            spline.StartTangent = new XYZ(0.707, 0.707, 0);
            spline.EndTangent = new XYZ(0, 0.707, 0.707);
            spline.Normal = new XYZ(0.577, 0.577, 0.577);
            spline.Color = new Color(3);
            spline.LineType = new LineType("DASHED");
            spline.LineWeight = LineWeightType.W40;
            spline.ControlPointTolerance = 0.001;
            spline.FitTolerance = 0.01;
            spline.KnotTolerance = 0.0001;
            spline.Knots.Add(0.0);
            spline.Knots.Add(0.5);
            spline.Knots.Add(1.0);
            spline.Weights.Add(1.0);
            spline.Weights.Add(2.0);
            spline.Weights.Add(1.5);

            var splineData = SplineEntityRenderer.Render(spline);

            Assert.NotNull(splineData);
            Assert.Equal(3, splineData.ControlPointCount);
            Assert.Equal(2, splineData.FitPointCount);
            Assert.Equal(3, splineData.Degree);
            Assert.True(splineData.IsClosed);
            Assert.False(splineData.IsPeriodic);
            Assert.InRange(splineData.StartTangent.X, 0.70, 0.71);
            Assert.InRange(splineData.StartTangent.Y, 0.70, 0.71);
            Assert.Equal(0, splineData.StartTangent.Z);
            Assert.Equal(0, splineData.EndTangent.X);
            Assert.InRange(splineData.EndTangent.Y, 0.70, 0.71);
            Assert.InRange(splineData.EndTangent.Z, 0.70, 0.71);
            Assert.InRange(splineData.Normal.X, 0.57, 0.58);
            Assert.InRange(splineData.Normal.Y, 0.57, 0.58);
            Assert.InRange(splineData.Normal.Z, 0.57, 0.58);
            Assert.Equal(3, splineData.ColorIndex);
            Assert.Equal("DASHED", splineData.LineTypeName);
            Assert.Equal(0.4, splineData.LineWeight);
            Assert.Equal(0.001, splineData.ControlPointTolerance);
            Assert.Equal(0.01, splineData.FitTolerance);
            Assert.Equal(0.0001, splineData.KnotTolerance);
            Assert.Equal(3, splineData.KnotCount);
            Assert.Equal(3, splineData.Weights.Count);
            Assert.True(splineData.IsRational);
            Assert.Equal("NURBSCurve", splineData.SplineType);
        }
    }
}
