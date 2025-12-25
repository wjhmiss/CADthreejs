using System.Collections.Generic;
using ACadSharp.Entities;
using DxfDwgViewer.RenderUtilities;
using Newtonsoft.Json;

namespace DxfDwgViewer
{
    public class CadData
    {
        public List<LineEntityRenderer.LineData> LineDatas { get; set; } = new List<LineEntityRenderer.LineData>();
        public List<ArcEntityRenderer.ArcData> ArcDatas { get; set; } = new List<ArcEntityRenderer.ArcData>();
        public List<CircleEntityRenderer.CircleData> CircleDatas { get; set; } = new List<CircleEntityRenderer.CircleData>();
        public List<LwPolylineEntityRenderer.LwPolylineData> LwPolylineDatas { get; set; } = new List<LwPolylineEntityRenderer.LwPolylineData>();
        public List<TextEntityRenderer.TextData> TextDatas { get; set; } = new List<TextEntityRenderer.TextData>();
        public List<EllipseEntityRenderer.EllipseData> EllipseDatas { get; set; } = new List<EllipseEntityRenderer.EllipseData>();
        public List<Face3DEntityRenderer.Face3DData> Face3DDatas { get; set; } = new List<Face3DEntityRenderer.Face3DData>();
        public List<HatchEntityRenderer.HatchData> HatchDatas { get; set; } = new List<HatchEntityRenderer.HatchData>();
        public List<PointEntityRenderer.PointData> PointDatas { get; set; } = new List<PointEntityRenderer.PointData>();
        public List<Polyline2DEntityRenderer.Polyline2DData> Polyline2DDatas { get; set; } = new List<Polyline2DEntityRenderer.Polyline2DData>();
        public List<Polyline3DEntityRenderer.Polyline3DData> Polyline3DDatas { get; set; } = new List<Polyline3DEntityRenderer.Polyline3DData>();
        public List<SolidEntityRenderer.SolidData> SolidDatas { get; set; } = new List<SolidEntityRenderer.SolidData>();
        public List<SplineEntityRenderer.SplineData> SplineDatas { get; set; } = new List<SplineEntityRenderer.SplineData>();
        public List<MLineEntityRenderer.MLineData> MLineDatas { get; set; } = new List<MLineEntityRenderer.MLineData>();
        public List<RayEntityRenderer.RayData> RayDatas { get; set; } = new List<RayEntityRenderer.RayData>();
        public List<XLineEntityRenderer.XLineData> XLineDatas { get; set; } = new List<XLineEntityRenderer.XLineData>();
        public List<InsertEntityRenderer.InsertData> InsertDatas { get; set; } = new List<InsertEntityRenderer.InsertData>();
        public List<RasterImageEntityRenderer.RasterImageData> RasterImageDatas { get; set; } = new List<RasterImageEntityRenderer.RasterImageData>();
        public List<MeshEntityRenderer.MeshData> MeshDatas { get; set; } = new List<MeshEntityRenderer.MeshData>();
        public List<MTextEntityRenderer.MTextData> MTextDatas { get; set; } = new List<MTextEntityRenderer.MTextData>();
        public List<WipeoutEntityRenderer.WipeoutData> WipeoutDatas { get; set; } = new List<WipeoutEntityRenderer.WipeoutData>();
        public List<PolyfaceMeshEntityRenderer.PolyfaceMeshData> PolyfaceMeshDatas { get; set; } = new List<PolyfaceMeshEntityRenderer.PolyfaceMeshData>();
        public List<ShapeEntityRenderer.ShapeData> ShapeDatas { get; set; } = new List<ShapeEntityRenderer.ShapeData>();
        public List<DimensionEntityRenderer.DimensionData> DimensionLinearDatas { get; set; } = new List<DimensionEntityRenderer.DimensionData>();
        public List<DimensionEntityRenderer.DimensionData> DimensionAngular3PtDatas { get; set; } = new List<DimensionEntityRenderer.DimensionData>();
        public List<DimensionEntityRenderer.DimensionData> DimensionAngular2LineDatas { get; set; } = new List<DimensionEntityRenderer.DimensionData>();
        public List<DimensionEntityRenderer.DimensionData> DimensionRadiusDatas { get; set; } = new List<DimensionEntityRenderer.DimensionData>();
        public List<DimensionEntityRenderer.DimensionData> DimensionDiameterDatas { get; set; } = new List<DimensionEntityRenderer.DimensionData>();
        public List<DimensionEntityRenderer.DimensionData> DimensionOrdinateDatas { get; set; } = new List<DimensionEntityRenderer.DimensionData>();
        public List<DimensionEntityRenderer.DimensionData> DimensionAlignedDatas { get; set; } = new List<DimensionEntityRenderer.DimensionData>();
        public List<LeaderEntityRenderer.LeaderData> LeaderDatas { get; set; } = new List<LeaderEntityRenderer.LeaderData>();
        public List<PdfUnderlayEntityRenderer.PdfUnderlayData> PdfUnderlayDatas { get; set; } = new List<PdfUnderlayEntityRenderer.PdfUnderlayData>();
    }

    public class JsonDataGenerator
    {
        public string GenerateJsonData(List<Entity> entities)
        {
            var cadData = new CadData();

            foreach (var entity in entities)
                {
                    if (entity is Line line)
                    {
                        var data = LineEntityRenderer.Render(line);
                        cadData.LineDatas.Add(data);
                    }
                    else if (entity is Arc arc)
                    {
                        var data = ArcEntityRenderer.Render(arc);
                        cadData.ArcDatas.Add(data);
                    }
                    else if (entity is Circle circle)
                    {
                        var data = CircleEntityRenderer.Render(circle);
                        cadData.CircleDatas.Add(data);
                    }
                    else if (entity is LwPolyline lwPolyline)
                    {
                        var data = LwPolylineEntityRenderer.Render(lwPolyline);
                        cadData.LwPolylineDatas.Add(data);
                    }
                    else if (entity is TextEntity text)
                    {
                        var data = TextEntityRenderer.Render(text);
                        cadData.TextDatas.Add(data);
                    }
                    else if (entity is Ellipse ellipse)
                    {
                        var data = EllipseEntityRenderer.Render(ellipse);
                        cadData.EllipseDatas.Add(data);
                    }
                    else if (entity is Face3D face3D)
                    {
                        var data = Face3DEntityRenderer.Render(face3D);
                        cadData.Face3DDatas.Add(data);
                    }
                    else if (entity is Hatch hatch)
                    {
                        var data = HatchEntityRenderer.Render(hatch);
                        cadData.HatchDatas.Add(data);
                    }
                    else if (entity is ACadSharp.Entities.Point point)
                    {
                        var data = PointEntityRenderer.Render(point);
                        cadData.PointDatas.Add(data);
                    }
                    else if (entity is Polyline2D polyline2D)
                    {
                        var data = Polyline2DEntityRenderer.Render(polyline2D);
                        cadData.Polyline2DDatas.Add(data);
                    }
                    else if (entity is Polyline3D polyline3D)
                    {
                        var data = Polyline3DEntityRenderer.Render(polyline3D);
                        cadData.Polyline3DDatas.Add(data);
                    }
                    else if (entity is Solid solid)
                    {
                        var data = SolidEntityRenderer.Render(solid);
                        cadData.SolidDatas.Add(data);
                    }
                    else if (entity is Spline spline)
                    {
                        var data = SplineEntityRenderer.Render(spline);
                        cadData.SplineDatas.Add(data);
                    }
                    else if (entity is MLine mline)
                    {
                        var data = MLineEntityRenderer.Render(mline);
                        cadData.MLineDatas.Add(data);
                    }
                    else if (entity is Ray ray)
                    {
                        var data = RayEntityRenderer.Render(ray);
                        cadData.RayDatas.Add(data);
                    }
                    else if (entity is XLine xline)
                    {
                        var data = XLineEntityRenderer.Render(xline);
                        cadData.XLineDatas.Add(data);
                    }
                    else if (entity is Insert insert)
                    {
                        var data = InsertEntityRenderer.Render(insert);
                        cadData.InsertDatas.Add(data);
                    }
                    else if (entity is RasterImage rasterImage)
                    {
                        var data = RasterImageEntityRenderer.Render(rasterImage);
                        cadData.RasterImageDatas.Add(data);
                    }
                    else if (entity is Mesh mesh)
                    {
                        var data = MeshEntityRenderer.Render(mesh);
                        cadData.MeshDatas.Add(data);
                    }
                    else if (entity is MText mtext)
                    {
                        var data = MTextEntityRenderer.Render(mtext);
                        cadData.MTextDatas.Add(data);
                    }
                    else if (entity is Wipeout wipeout)
                    {
                        var data = WipeoutEntityRenderer.Render(wipeout);
                        cadData.WipeoutDatas.Add(data);
                    }
                    else if (entity is PolyfaceMesh polyfaceMesh)
                    {
                        var data = PolyfaceMeshEntityRenderer.Render(polyfaceMesh);
                        cadData.PolyfaceMeshDatas.Add(data);
                    }
                    else if (entity is Shape shape)
                    {
                        var data = ShapeEntityRenderer.Render(shape);
                        cadData.ShapeDatas.Add(data);
                    }
                    else if (entity is DimensionLinear dimensionLinear)
                    {
                        var data = DimensionEntityRenderer.Render(dimensionLinear);
                        cadData.DimensionLinearDatas.Add(data);
                    }
                    else if (entity is DimensionAngular3Pt dimensionAngular3Pt)
                    {
                        var data = DimensionEntityRenderer.Render(dimensionAngular3Pt);
                        cadData.DimensionAngular3PtDatas.Add(data);
                    }
                    else if (entity is DimensionAngular2Line dimensionAngular2Line)
                    {
                        var data = DimensionEntityRenderer.Render(dimensionAngular2Line);
                        cadData.DimensionAngular2LineDatas.Add(data);
                    }
                    else if (entity is DimensionRadius dimensionRadius)
                    {
                        var data = DimensionEntityRenderer.Render(dimensionRadius);
                        cadData.DimensionRadiusDatas.Add(data);
                    }
                    else if (entity is DimensionDiameter dimensionDiameter)
                    {
                        var data = DimensionEntityRenderer.Render(dimensionDiameter);
                        cadData.DimensionDiameterDatas.Add(data);
                    }
                    else if (entity is DimensionOrdinate dimensionOrdinate)
                    {
                        var data = DimensionEntityRenderer.Render(dimensionOrdinate);
                        cadData.DimensionOrdinateDatas.Add(data);
                    }
                    else if (entity is DimensionAligned dimensionAligned)
                    {
                        var data = DimensionEntityRenderer.Render(dimensionAligned);
                        cadData.DimensionAlignedDatas.Add(data);
                    }
                    else if (entity is Leader leader)
                    {
                        var data = LeaderEntityRenderer.Render(leader);
                        cadData.LeaderDatas.Add(data);
                    }
                    else if (entity is PdfUnderlay pdfUnderlay)
                    {
                        var data = PdfUnderlayEntityRenderer.Render(pdfUnderlay);
                        cadData.PdfUnderlayDatas.Add(data);
                    }
                    else
                    {
                        // 未支持的实体类型
                        Console.WriteLine($"Unsupported entity type: {entity.GetType().Name}");
                    }
                }

            // var settings = new JsonSerializerSettings
            // {
            //     ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //     Formatting = Formatting.Indented
            // };

            // var json = JsonConvert.SerializeObject(cadData, settings);
            var json = JsonConvert.SerializeObject(cadData);
            return json;
        }
    }
}