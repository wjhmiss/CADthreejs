using Xunit;
using ACadSharp.Entities;
using ACadSharp.IO;
using ACadSharp.Tables;
using ACadSharp.Objects;
using DxfDwgViewer.RenderUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using CSMath;

namespace DxfDwgViewer.Tests
{
    public class MLineEntityRendererIntegrationTests
    {
        [Fact]
        public void LoadAndRenderMLineFromDwg_Success()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawing1.dwg");
            
            if (!File.Exists(filePath))
            {
                return;
            }

            using var reader = new DwgReader(filePath);
            var doc = reader.Read();

            Assert.NotNull(doc);

            var mlineEntities = new List<MLine>();
            foreach (var entity in doc.Entities)
            {
                if (entity is MLine mline)
                {
                    mlineEntities.Add(mline);
                }
            }

            if (mlineEntities.Count == 0)
            {
                return;
            }

            foreach (var mline in mlineEntities)
            {
                var mlineData = MLineEntityRenderer.Render(mline);
                Assert.NotNull(mlineData);
                Assert.Equal("MLine", mlineData.Type);
                Assert.True(mlineData.TotalLength >= 0);
                Assert.NotNull(mlineData.StartPoint);
                Assert.NotNull(mlineData.EndPoint);
                Assert.NotNull(mlineData.Elements);
            }
        }

        [Fact]
        public void SerializeMLineDataToJson_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("MLine", json);
            Assert.Contains("Elements", json);
            Assert.Contains("Vertices", json);
            Assert.Contains("TotalLength", json);
        }

        [Fact]
        public void DeserializeMLineDataFromJson_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.Equal(originalMLineData.Type, deserializedMLineData.Type);
            Assert.Equal(originalMLineData.ElementCount, deserializedMLineData.ElementCount);
            Assert.Equal(originalMLineData.VertexCount, deserializedMLineData.VertexCount);
            Assert.Equal(originalMLineData.ScaleFactor, deserializedMLineData.ScaleFactor);
            Assert.Equal(originalMLineData.ColorIndex, deserializedMLineData.ColorIndex);
            Assert.Equal(originalMLineData.FillColorIndex, deserializedMLineData.FillColorIndex);
            Assert.Equal(originalMLineData.FillOn, deserializedMLineData.FillOn);
            Assert.Equal(originalMLineData.IsClosed, deserializedMLineData.IsClosed);
            Assert.Equal(originalMLineData.TotalLength, deserializedMLineData.TotalLength);
        }

        [Fact]
        public void RenderMultipleMLines_CollectAllMLineData()
        {
            var mlines = new List<MLine>
            {
                CreateBasicMLine(),
                CreateBasicMLine()
            };

            var mlineDataList = new List<MLineEntityRenderer.MLineData>();
            foreach (var mline in mlines)
            {
                mlineDataList.Add(MLineEntityRenderer.Render(mline));
            }

            Assert.Equal(2, mlineDataList.Count);
            Assert.All(mlineDataList, data =>
            {
                Assert.NotNull(data);
                Assert.Equal("MLine", data.Type);
                Assert.True(data.TotalLength >= 0);
                Assert.NotNull(data.StartPoint);
                Assert.NotNull(data.EndPoint);
                Assert.NotNull(data.Elements);
            });
        }

        [Fact]
        public void SerializeMultipleMLineDataToJson_Success()
        {
            var mlines = new List<MLine>
            {
                CreateBasicMLine(),
                CreateBasicMLine()
            };

            var mlineDataList = new List<MLineEntityRenderer.MLineData>();
            foreach (var mline in mlines)
            {
                mlineDataList.Add(MLineEntityRenderer.Render(mline));
            }

            var json = JsonConvert.SerializeObject(mlineDataList, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("MLine", json);
            Assert.Contains("Elements", json);
            Assert.Contains("Vertices", json);
        }

        [Fact]
        public void DeserializeMultipleMLineDataFromJson_Success()
        {
            var mlines = new List<MLine>
            {
                CreateBasicMLine(),
                CreateBasicMLine()
            };

            var originalMLineDataList = new List<MLineEntityRenderer.MLineData>();
            foreach (var mline in mlines)
            {
                originalMLineDataList.Add(MLineEntityRenderer.Render(mline));
            }

            var json = JsonConvert.SerializeObject(originalMLineDataList);
            var deserializedMLineDataList = JsonConvert.DeserializeObject<List<MLineEntityRenderer.MLineData>>(json);

            Assert.NotNull(deserializedMLineDataList);
            Assert.Equal(2, deserializedMLineDataList.Count);

            for (int i = 0; i < originalMLineDataList.Count; i++)
            {
                Assert.Equal(originalMLineDataList[i].Type, deserializedMLineDataList[i].Type);
                Assert.Equal(originalMLineDataList[i].ElementCount, deserializedMLineDataList[i].ElementCount);
                Assert.Equal(originalMLineDataList[i].VertexCount, deserializedMLineDataList[i].VertexCount);
                Assert.Equal(originalMLineDataList[i].ScaleFactor, deserializedMLineDataList[i].ScaleFactor);
                Assert.Equal(originalMLineDataList[i].TotalLength, deserializedMLineDataList[i].TotalLength);
            }
        }

        [Fact]
        public void SerializeAndDeserializeComplexMLineData_Success()
        {
            var mline = CreateComplexMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData, Formatting.Indented);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.Equal(originalMLineData.Type, deserializedMLineData.Type);
            Assert.Equal(originalMLineData.ElementCount, deserializedMLineData.ElementCount);
            Assert.Equal(originalMLineData.VertexCount, deserializedMLineData.VertexCount);
            Assert.Equal(originalMLineData.ScaleFactor, deserializedMLineData.ScaleFactor);
            Assert.Equal(originalMLineData.ColorIndex, deserializedMLineData.ColorIndex);
            Assert.Equal(originalMLineData.FillColorIndex, deserializedMLineData.FillColorIndex);
            Assert.Equal(originalMLineData.FillOn, deserializedMLineData.FillOn);
            Assert.Equal(originalMLineData.IsClosed, deserializedMLineData.IsClosed);
            Assert.Equal(originalMLineData.Justification, deserializedMLineData.Justification);
            Assert.Equal(originalMLineData.TotalLength, deserializedMLineData.TotalLength);
            Assert.Equal(originalMLineData.StyleName, deserializedMLineData.StyleName);
            Assert.Equal(originalMLineData.StartAngle, deserializedMLineData.StartAngle);
            Assert.Equal(originalMLineData.EndAngle, deserializedMLineData.EndAngle);
            Assert.Equal(originalMLineData.HasStartCaps, deserializedMLineData.HasStartCaps);
            Assert.Equal(originalMLineData.HasEndCaps, deserializedMLineData.HasEndCaps);
            Assert.Equal(originalMLineData.DisplayJoints, deserializedMLineData.DisplayJoints);
        }

        [Fact]
        public void SerializeMLineDataWithTransform_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Transform", json);
            Assert.Contains("Matrix", json);
        }

        [Fact]
        public void DeserializeMLineDataWithTransform_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.Transform);
            Assert.NotNull(deserializedMLineData.Transform.Matrix);
            Assert.Equal(16, deserializedMLineData.Transform.Matrix.Length);
        }

        [Fact]
        public void SerializeMLineDataWithBounds_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Bounds", json);
            Assert.Contains("Min", json);
            Assert.Contains("Max", json);
        }

        [Fact]
        public void DeserializeMLineDataWithBounds_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.Bounds);
            Assert.NotNull(deserializedMLineData.Bounds.Min);
            Assert.NotNull(deserializedMLineData.Bounds.Max);
            Assert.Equal(originalMLineData.Bounds.Min.X, deserializedMLineData.Bounds.Min.X);
            Assert.Equal(originalMLineData.Bounds.Min.Y, deserializedMLineData.Bounds.Min.Y);
            Assert.Equal(originalMLineData.Bounds.Min.Z, deserializedMLineData.Bounds.Min.Z);
            Assert.Equal(originalMLineData.Bounds.Max.X, deserializedMLineData.Bounds.Max.X);
            Assert.Equal(originalMLineData.Bounds.Max.Y, deserializedMLineData.Bounds.Max.Y);
            Assert.Equal(originalMLineData.Bounds.Max.Z, deserializedMLineData.Bounds.Max.Z);
        }

        [Fact]
        public void SerializeMLineDataWithBounds3D_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Bounds3D", json);
            Assert.Contains("Min", json);
            Assert.Contains("Max", json);
        }

        [Fact]
        public void DeserializeMLineDataWithBounds3D_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.Bounds3D);
            Assert.NotNull(deserializedMLineData.Bounds3D.Min);
            Assert.NotNull(deserializedMLineData.Bounds3D.Max);
            Assert.Equal(originalMLineData.Bounds3D.Min.X, deserializedMLineData.Bounds3D.Min.X);
            Assert.Equal(originalMLineData.Bounds3D.Min.Y, deserializedMLineData.Bounds3D.Min.Y);
            Assert.Equal(originalMLineData.Bounds3D.Min.Z, deserializedMLineData.Bounds3D.Min.Z);
            Assert.Equal(originalMLineData.Bounds3D.Max.X, deserializedMLineData.Bounds3D.Max.X);
            Assert.Equal(originalMLineData.Bounds3D.Max.Y, deserializedMLineData.Bounds3D.Max.Y);
            Assert.Equal(originalMLineData.Bounds3D.Max.Z, deserializedMLineData.Bounds3D.Max.Z);
        }

        [Fact]
        public void SerializeMLineDataWithCentroid_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Centroid", json);
        }

        [Fact]
        public void DeserializeMLineDataWithCentroid_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.Centroid);
            Assert.Equal(originalMLineData.Centroid.X, deserializedMLineData.Centroid.X);
            Assert.Equal(originalMLineData.Centroid.Y, deserializedMLineData.Centroid.Y);
        }

        [Fact]
        public void SerializeMLineDataWithCentroid3D_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Centroid3D", json);
        }

        [Fact]
        public void DeserializeMLineDataWithCentroid3D_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.Centroid3D);
            Assert.Equal(originalMLineData.Centroid3D.X, deserializedMLineData.Centroid3D.X);
            Assert.Equal(originalMLineData.Centroid3D.Y, deserializedMLineData.Centroid3D.Y);
            Assert.Equal(originalMLineData.Centroid3D.Z, deserializedMLineData.Centroid3D.Z);
        }

        [Fact]
        public void SerializeMLineDataWithVertices3D_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Vertices3D", json);
        }

        [Fact]
        public void DeserializeMLineDataWithVertices3D_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.Vertices3D);
            Assert.Equal(originalMLineData.Vertices3D.Count, deserializedMLineData.Vertices3D.Count);
            for (int i = 0; i < originalMLineData.Vertices3D.Count; i++)
            {
                Assert.Equal(originalMLineData.Vertices3D[i].X, deserializedMLineData.Vertices3D[i].X);
                Assert.Equal(originalMLineData.Vertices3D[i].Y, deserializedMLineData.Vertices3D[i].Y);
                Assert.Equal(originalMLineData.Vertices3D[i].Z, deserializedMLineData.Vertices3D[i].Z);
            }
        }

        [Fact]
        public void SerializeMLineDataWithNormals_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Normals", json);
        }

        [Fact]
        public void DeserializeMLineDataWithNormals_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.Normals);
            Assert.Equal(originalMLineData.Normals.Count, deserializedMLineData.Normals.Count);
            for (int i = 0; i < originalMLineData.Normals.Count; i++)
            {
                Assert.Equal(originalMLineData.Normals[i].X, deserializedMLineData.Normals[i].X);
                Assert.Equal(originalMLineData.Normals[i].Y, deserializedMLineData.Normals[i].Y);
                Assert.Equal(originalMLineData.Normals[i].Z, deserializedMLineData.Normals[i].Z);
            }
        }

        [Fact]
        public void SerializeMLineDataWithLineIndices_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("LineIndices", json);
        }

        [Fact]
        public void DeserializeMLineDataWithLineIndices_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.LineIndices);
            Assert.Equal(originalMLineData.LineIndices.Count, deserializedMLineData.LineIndices.Count);
            for (int i = 0; i < originalMLineData.LineIndices.Count; i++)
            {
                Assert.Equal(originalMLineData.LineIndices[i].Length, deserializedMLineData.LineIndices[i].Length);
                for (int j = 0; j < originalMLineData.LineIndices[i].Length; j++)
                {
                    Assert.Equal(originalMLineData.LineIndices[i][j], deserializedMLineData.LineIndices[i][j]);
                }
            }
        }

        [Fact]
        public void SerializeMLineDataWithOffsets_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Offsets", json);
        }

        [Fact]
        public void DeserializeMLineDataWithOffsets_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.Offsets);
            Assert.Equal(originalMLineData.Offsets.Count, deserializedMLineData.Offsets.Count);
            for (int i = 0; i < originalMLineData.Offsets.Count; i++)
            {
                Assert.Equal(originalMLineData.Offsets[i], deserializedMLineData.Offsets[i]);
            }
        }

        [Fact]
        public void SerializeMLineDataWithElementColorIndices_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("ElementColorIndices", json);
        }

        [Fact]
        public void DeserializeMLineDataWithElementColorIndices_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.ElementColorIndices);
            Assert.Equal(originalMLineData.ElementColorIndices.Count, deserializedMLineData.ElementColorIndices.Count);
            for (int i = 0; i < originalMLineData.ElementColorIndices.Count; i++)
            {
                Assert.Equal(originalMLineData.ElementColorIndices[i], deserializedMLineData.ElementColorIndices[i]);
            }
        }

        [Fact]
        public void SerializeMLineDataWithElements_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Elements", json);
        }

        [Fact]
        public void DeserializeMLineDataWithElements_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.Elements);
            Assert.Equal(originalMLineData.Elements.Count, deserializedMLineData.Elements.Count);
            for (int i = 0; i < originalMLineData.Elements.Count; i++)
            {
                Assert.Equal(originalMLineData.Elements[i].Offset, deserializedMLineData.Elements[i].Offset);
                Assert.Equal(originalMLineData.Elements[i].ColorIndex, deserializedMLineData.Elements[i].ColorIndex);
                Assert.Equal(originalMLineData.Elements[i].PointCount, deserializedMLineData.Elements[i].PointCount);
                Assert.Equal(originalMLineData.Elements[i].TotalLength, deserializedMLineData.Elements[i].TotalLength);
            }
        }

        [Fact]
        public void SerializeMLineDataWithVertices_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Vertices", json);
        }

        [Fact]
        public void DeserializeMLineDataWithVertices_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.Vertices);
            Assert.Equal(originalMLineData.Vertices.Count, deserializedMLineData.Vertices.Count);
            for (int i = 0; i < originalMLineData.Vertices.Count; i++)
            {
                Assert.Equal(originalMLineData.Vertices[i].Position.X, deserializedMLineData.Vertices[i].Position.X);
                Assert.Equal(originalMLineData.Vertices[i].Position.Y, deserializedMLineData.Vertices[i].Position.Y);
                Assert.Equal(originalMLineData.Vertices[i].Direction.X, deserializedMLineData.Vertices[i].Direction.X);
                Assert.Equal(originalMLineData.Vertices[i].Direction.Y, deserializedMLineData.Vertices[i].Direction.Y);
                Assert.Equal(originalMLineData.Vertices[i].Miter.X, deserializedMLineData.Vertices[i].Miter.X);
                Assert.Equal(originalMLineData.Vertices[i].Miter.Y, deserializedMLineData.Vertices[i].Miter.Y);
            }
        }

        [Fact]
        public void SerializeMLineDataWithColor_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Color", json);
            Assert.Contains("Index", json);
            Assert.Contains("Hex", json);
        }

        [Fact]
        public void DeserializeMLineDataWithColor_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.Color);
            Assert.Equal(originalMLineData.Color.Index, deserializedMLineData.Color.Index);
            Assert.Equal(originalMLineData.Color.Hex, deserializedMLineData.Color.Hex);
        }

        [Fact]
        public void SerializeMLineDataWithMaterialProperties_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("MaterialType", json);
            Assert.Contains("Opacity", json);
            Assert.Contains("Transparent", json);
        }

        [Fact]
        public void DeserializeMLineDataWithMaterialProperties_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.Equal(originalMLineData.MaterialType, deserializedMLineData.MaterialType);
            Assert.Equal(originalMLineData.Opacity, deserializedMLineData.Opacity);
            Assert.Equal(originalMLineData.Transparent, deserializedMLineData.Transparent);
            Assert.Equal(originalMLineData.DepthTest, deserializedMLineData.DepthTest);
            Assert.Equal(originalMLineData.DepthWrite, deserializedMLineData.DepthWrite);
        }

        [Fact]
        public void SerializeMLineDataWithNormal_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("Normal", json);
        }

        [Fact]
        public void DeserializeMLineDataWithNormal_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.Normal);
            Assert.Equal(originalMLineData.Normal.X, deserializedMLineData.Normal.X);
            Assert.Equal(originalMLineData.Normal.Y, deserializedMLineData.Normal.Y);
            Assert.Equal(originalMLineData.Normal.Z, deserializedMLineData.Normal.Z);
        }

        [Fact]
        public void SerializeMLineDataWithStartPoint_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("StartPoint", json);
        }

        [Fact]
        public void DeserializeMLineDataWithStartPoint_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.StartPoint);
            Assert.Equal(originalMLineData.StartPoint.X, deserializedMLineData.StartPoint.X);
            Assert.Equal(originalMLineData.StartPoint.Y, deserializedMLineData.StartPoint.Y);
            Assert.Equal(originalMLineData.StartPoint.Z, deserializedMLineData.StartPoint.Z);
        }

        [Fact]
        public void SerializeMLineDataWithEndPoint_Success()
        {
            var mline = CreateBasicMLine();

            var mlineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(mlineData, Formatting.Indented);

            Assert.NotNull(json);
            Assert.Contains("EndPoint", json);
        }

        [Fact]
        public void DeserializeMLineDataWithEndPoint_Success()
        {
            var mline = CreateBasicMLine();

            var originalMLineData = MLineEntityRenderer.Render(mline);
            var json = JsonConvert.SerializeObject(originalMLineData);
            var deserializedMLineData = JsonConvert.DeserializeObject<MLineEntityRenderer.MLineData>(json);

            Assert.NotNull(deserializedMLineData);
            Assert.NotNull(deserializedMLineData.EndPoint);
            Assert.Equal(originalMLineData.EndPoint.X, deserializedMLineData.EndPoint.X);
            Assert.Equal(originalMLineData.EndPoint.Y, deserializedMLineData.EndPoint.Y);
            Assert.Equal(originalMLineData.EndPoint.Z, deserializedMLineData.EndPoint.Z);
        }

        private MLine CreateBasicMLine()
        {
            var mline = new MLine();
            mline.ScaleFactor = 1.0;
            mline.Color = new ACadSharp.Color(7);
            mline.Normal = new XYZ(0, 0, 1);
            mline.Justification = MLineJustification.Zero;
            mline.Flags = 0;
            mline.Style.Name = "STANDARD";
            mline.Style.StartAngle = 0;
            mline.Style.EndAngle = 0;
            mline.Style.Flags = MLineStyleFlags.None;
            mline.Style.FillColor = new ACadSharp.Color(0);

            mline.Style.AddElement(new MLineStyle.Element
            {
                Offset = -0.5,
                Color = new ACadSharp.Color(1),
                LineType = new LineType("CONTINUOUS")
            });

            mline.Style.AddElement(new MLineStyle.Element
            {
                Offset = 0.5,
                Color = new ACadSharp.Color(2),
                LineType = new LineType("CONTINUOUS")
            });

            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(0, 0, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });

            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(10, 0, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });

            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(20, 0, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });

            return mline;
        }

        private MLine CreateComplexMLine()
        {
            var mline = new MLine();
            mline.ScaleFactor = 2.0;
            mline.Color = new ACadSharp.Color(5);
            mline.Normal = new XYZ(0, 0, 1);
            mline.Justification = MLineJustification.Top;
            mline.Flags = MLineFlags.Closed;
            mline.Style.Name = "COMPLEX";
            mline.Style.StartAngle = Math.PI / 4;
            mline.Style.EndAngle = Math.PI / 2;
            mline.Style.Flags = MLineStyleFlags.FillOn | MLineStyleFlags.DisplayJoints;
            mline.Style.FillColor = new ACadSharp.Color(3);

            mline.Style.AddElement(new MLineStyle.Element
            {
                Offset = -1.0,
                Color = new ACadSharp.Color(1),
                LineType = new LineType("DASHED")
            });

            mline.Style.AddElement(new MLineStyle.Element
            {
                Offset = 0.0,
                Color = new ACadSharp.Color(2),
                LineType = new LineType("CONTINUOUS")
            });

            mline.Style.AddElement(new MLineStyle.Element
            {
                Offset = 1.0,
                Color = new ACadSharp.Color(3),
                LineType = new LineType("DASHED")
            });

            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(0, 0, 0),
                Direction = new XYZ(1, 0, 0),
                Miter = new XYZ(0, 1, 0)
            });

            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(10, 0, 0),
                Direction = new XYZ(0, 1, 0),
                Miter = new XYZ(-1, 0, 0)
            });

            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(10, 10, 0),
                Direction = new XYZ(-1, 0, 0),
                Miter = new XYZ(0, -1, 0)
            });

            mline.Vertices.Add(new MLine.Vertex
            {
                Position = new XYZ(0, 10, 0),
                Direction = new XYZ(0, -1, 0),
                Miter = new XYZ(1, 0, 0)
            });

            return mline;
        }
    }
}
