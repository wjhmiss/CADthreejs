using System;
using ACadSharp.Entities;
using System.Collections.Generic;
using System.Linq;
using ACadSharp.Extensions;

namespace DxfDwgViewer.RenderUtilities
{
    public class MTextEntityRenderer
    {
        public class MTextData
        {
            public List<string> Lines { get; set; }
            public PointData InsertPoint { get; set; }
            public double Height { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            public double RectangleWidth { get; set; }
            public LineSpacingStyleType LineSpacingStyle { get; set; }
            public double LineSpacing { get; set; }
            public AttachmentPointType AttachmentPoint { get; set; }
            public double Rotation { get; set; }
            public string TextStyleName { get; set; }
            public double WidthFactor { get; set; }
            public double ObliqueAngle { get; set; }
            
            // 基本几何属性
            public BoundsData Bounds { get; set; }
            public PointData AlignmentPoint { get; set; }
            public int LineCount { get; set; }
            
            // three.js兼容性扩展属性
            public string Type { get; set; } = "MText";
            public string EntityType { get; set; } = "MText";
            public string Handle { get; set; }
            public string LayerName { get; set; }
            public int LayerIndex { get; set; }
            public bool Visible { get; set; } = true;
            public string CoordinateSystem { get; set; } = "World";
            
            // 3D坐标和几何信息
            public List<Point3DData> Vertices3D { get; set; }
            public Point3DData Centroid3D { get; set; }
            public List<Point3DData> Normals { get; set; }
            public BoundsData3D Bounds3D { get; set; }
            public ColorData Color { get; set; }
            public TransformData Transform { get; set; }
            public NormalData Normal { get; set; }
            
            // 材质属性
            public double Opacity { get; set; } = 1.0;
            public bool Transparent { get; set; } = false;
            public string MaterialType { get; set; } = "TextMaterial";
            public bool DepthTest { get; set; } = true;
            public bool DepthWrite { get; set; } = true;
            
            // MText特有属性
            public DrawingDirectionType DrawingDirection { get; set; }
            public ColorData BackgroundColor { get; set; }
            public BackgroundFillFlags BackgroundFillFlags { get; set; }
            public double BackgroundScale { get; set; }
            public double BackgroundTransparency { get; set; }
            public double HorizontalWidth { get; set; }
            public double RectangleHeight { get; set; }
            public double VerticalHeight { get; set; }
            public bool IsAnnotative { get; set; }
            public string PlainText { get; set; }
            public string TextValue { get; set; }
            public Point3DData InsertPoint3D { get; set; }
            public Point3DData AlignmentPoint3D { get; set; }
            public double TextWidth { get; set; }
            public double TextHeight { get; set; }
        }

        public static MTextData Render(MText mtext)
        {
            List<Point3DData> vertices3D = new List<Point3DData>();
            List<Point3DData> normals = new List<Point3DData>();
            
            var mtextData = new MTextData
            {
                Lines = new List<string>(),
                InsertPoint = new PointData(mtext.InsertPoint.X, mtext.InsertPoint.Y),
                Height = mtext.Height,
                ColorIndex = mtext.Color.Index,
                LineTypeName = mtext.GetActiveLineType()?.Name ?? "",
                LineWeight = mtext.GetActiveLineWeightType().GetLineWeightValue(),
                RectangleWidth = mtext.RectangleWidth,
                LineSpacingStyle = mtext.LineSpacingStyle,
                LineSpacing = mtext.LineSpacing,
                AttachmentPoint = mtext.AttachmentPoint,
                Rotation = mtext.Rotation,
                TextStyleName = mtext.Style?.Name ?? "",
                WidthFactor = mtext.Style != null ? mtext.Style.Width : 1.0,
                ObliqueAngle = mtext.Style != null ? mtext.Style.ObliqueAngle : 0.0,
                
                // 初始化three.js兼容性扩展属性
                Type = "MText",
                EntityType = "MText",
                Handle = mtext.Handle.ToString(),
                LayerName = mtext.Layer?.Name ?? "",
                LayerIndex = 0,
                Visible = !mtext.IsInvisible,
                CoordinateSystem = "World",
                
                // 3D坐标和几何信息
                Vertices3D = vertices3D,
                Normals = normals,
                Color = new ColorData(mtext.Color.Index),
                Normal = new NormalData(mtext.Normal.X, mtext.Normal.Y, mtext.Normal.Z),
                Transform = new TransformData(),
                
                // 材质属性
                Opacity = 1.0,
                Transparent = false,
                MaterialType = "TextMaterial",
                DepthTest = true,
                DepthWrite = true,
                
                // MText特有属性
                DrawingDirection = mtext.DrawingDirection,
                BackgroundColor = new ColorData(mtext.BackgroundColor.Index),
                BackgroundFillFlags = mtext.BackgroundFillFlags,
                BackgroundScale = mtext.BackgroundScale,
                BackgroundTransparency = mtext.BackgroundTransparency.Value,
                HorizontalWidth = mtext.HorizontalWidth,
                RectangleHeight = mtext.RectangleHeight,
                VerticalHeight = mtext.VerticalHeight,
                IsAnnotative = mtext.IsAnnotative,
                PlainText = mtext.PlainText,
                TextValue = mtext.Value,
                InsertPoint3D = new Point3DData(mtext.InsertPoint.X, mtext.InsertPoint.Y, mtext.InsertPoint.Z),
                AlignmentPoint3D = new Point3DData(mtext.AlignmentPoint.X, mtext.AlignmentPoint.Y, mtext.AlignmentPoint.Z)
            };

            string[] lines = mtext.GetPlainTextLines();

            mtextData.Lines.AddRange(lines);
            mtextData.LineCount = lines.Length;

            double fontSize = mtext.Height;
            double lineHeight = fontSize;
            if (mtext.LineSpacingStyle == LineSpacingStyleType.AtLeast ||
                mtext.LineSpacingStyle == LineSpacingStyleType.Exact)
            {
                lineHeight *= mtext.LineSpacing;
            }

            double x = mtext.InsertPoint.X;
            double y = mtext.InsertPoint.Y;

            double minX = x, maxX = x, minY = y, maxY = y;
            if (lines.Length > 0)
            {
                double maxWidth = 0;
                foreach (string lineText in lines)
                {
                    double estimatedWidth = lineText.Length * fontSize * mtextData.WidthFactor * 0.6;
                    if (estimatedWidth > maxWidth)
                        maxWidth = estimatedWidth;
                }

                if (mtext.RectangleWidth > 0)
                {
                    maxWidth = mtext.RectangleWidth;
                }

                switch (mtext.AttachmentPoint)
                {
                    case AttachmentPointType.TopLeft:
                        minX = x;
                        maxX = x + maxWidth;
                        minY = y - (lines.Length - 1) * lineHeight;
                        maxY = y + lineHeight;
                        break;
                    case AttachmentPointType.TopCenter:
                        minX = x - maxWidth / 2;
                        maxX = x + maxWidth / 2;
                        minY = y - (lines.Length - 1) * lineHeight;
                        maxY = y + lineHeight;
                        break;
                    case AttachmentPointType.TopRight:
                        minX = x - maxWidth;
                        maxX = x;
                        minY = y - (lines.Length - 1) * lineHeight;
                        maxY = y + lineHeight;
                        break;
                    case AttachmentPointType.MiddleLeft:
                        minX = x;
                        maxX = x + maxWidth;
                        minY = y - (lines.Length / 2.0) * lineHeight;
                        maxY = y + (lines.Length / 2.0) * lineHeight;
                        break;
                    case AttachmentPointType.MiddleCenter:
                        minX = x - maxWidth / 2;
                        maxX = x + maxWidth / 2;
                        minY = y - (lines.Length / 2.0) * lineHeight;
                        maxY = y + (lines.Length / 2.0) * lineHeight;
                        break;
                    case AttachmentPointType.MiddleRight:
                        minX = x - maxWidth;
                        maxX = x;
                        minY = y - (lines.Length / 2.0) * lineHeight;
                        maxY = y + (lines.Length / 2.0) * lineHeight;
                        break;
                    case AttachmentPointType.BottomLeft:
                        minX = x;
                        maxX = x + maxWidth;
                        minY = y - lineHeight;
                        maxY = y + (lines.Length - 1) * lineHeight;
                        break;
                    case AttachmentPointType.BottomCenter:
                        minX = x - maxWidth / 2;
                        maxX = x + maxWidth / 2;
                        minY = y - lineHeight;
                        maxY = y + (lines.Length - 1) * lineHeight;
                        break;
                    case AttachmentPointType.BottomRight:
                        minX = x - maxWidth;
                        maxX = x;
                        minY = y - lineHeight;
                        maxY = y + (lines.Length - 1) * lineHeight;
                        break;
                }
            }

            mtextData.Bounds = new BoundsData(
                new Point3DData(minX, minY, mtext.InsertPoint.Z),
                new Point3DData(maxX, maxY, mtext.InsertPoint.Z)
            );
            
            double textWidth = maxX - minX;
            double textHeight = maxY - minY;
            mtextData.TextWidth = textWidth;
            mtextData.TextHeight = textHeight;
            
            PointData centroid = new PointData(
                (minX + maxX) / 2,
                (minY + maxY) / 2
            );
            
            vertices3D.Add(new Point3DData(minX, minY, mtext.InsertPoint.Z));
            vertices3D.Add(new Point3DData(maxX, minY, mtext.InsertPoint.Z));
            vertices3D.Add(new Point3DData(maxX, maxY, mtext.InsertPoint.Z));
            vertices3D.Add(new Point3DData(minX, maxY, mtext.InsertPoint.Z));
            
            normals.Add(new Point3DData(0, 0, 1));
            normals.Add(new Point3DData(0, 0, 1));
            normals.Add(new Point3DData(0, 0, 1));
            normals.Add(new Point3DData(0, 0, 1));
            
            Point3DData centroid3D = new Point3DData(
                (minX + maxX) / 2,
                (minY + maxY) / 2,
                mtext.InsertPoint.Z
            );
            
            BoundsData3D bounds3D = new BoundsData3D(
                new Point3DData(minX, minY, mtext.InsertPoint.Z),
                new Point3DData(maxX, maxY, mtext.InsertPoint.Z)
            );

            switch (mtext.AttachmentPoint)
            {
                case AttachmentPointType.TopLeft:
                    mtextData.AlignmentPoint = new PointData(minX, maxY);
                    break;
                case AttachmentPointType.TopCenter:
                    mtextData.AlignmentPoint = new PointData((minX + maxX) / 2, maxY);
                    break;
                case AttachmentPointType.TopRight:
                    mtextData.AlignmentPoint = new PointData(maxX, maxY);
                    break;
                case AttachmentPointType.MiddleLeft:
                    mtextData.AlignmentPoint = new PointData(minX, (minY + maxY) / 2);
                    break;
                case AttachmentPointType.MiddleCenter:
                    mtextData.AlignmentPoint = new PointData((minX + maxX) / 2, (minY + maxY) / 2);
                    break;
                case AttachmentPointType.MiddleRight:
                    mtextData.AlignmentPoint = new PointData(maxX, (minY + maxY) / 2);
                    break;
                case AttachmentPointType.BottomLeft:
                    mtextData.AlignmentPoint = new PointData(minX, minY);
                    break;
                case AttachmentPointType.BottomCenter:
                    mtextData.AlignmentPoint = new PointData((minX + maxX) / 2, minY);
                    break;
                case AttachmentPointType.BottomRight:
                    mtextData.AlignmentPoint = new PointData(maxX, minY);
                    break;
            }

            mtextData.Bounds = mtextData.Bounds;
            mtextData.Bounds3D = bounds3D;
            mtextData.Centroid3D = centroid3D;
            
            mtextData.Transform = new TransformData(
                1.0, 1.0, 1.0,
                mtextData.Rotation,
                mtextData.InsertPoint.X, mtextData.InsertPoint.Y, mtext.InsertPoint.Z
            );
            
            if (mtext.BackgroundTransparency.Value != -1)
            {
                mtextData.Transparent = true;
                mtextData.Opacity = (100.0 - mtext.BackgroundTransparency.Value) / 100.0;
            }

            return mtextData;
        }
    }
}
