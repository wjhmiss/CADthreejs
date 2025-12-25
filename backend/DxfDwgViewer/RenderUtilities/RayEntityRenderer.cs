using ACadSharp.Entities;
using ACadSharp.Extensions;
using CSMath;

namespace DxfDwgViewer.RenderUtilities
{
    public class RayEntityRenderer
    {
        public class RayData
        {
            public Point3DData StartPoint { get; set; }
            public Point3DData Direction { get; set; }
            public short ColorIndex { get; set; }
            public string LineTypeName { get; set; }
            public double LineWeight { get; set; }
            
            public Point3DData EndPoint { get; set; }
            public double Length { get; set; }
            public double Angle { get; set; }
            
            public BoundsData Bounds { get; set; }
            public Point3DData Centroid { get; set; }
            
            public TransformData Transform { get; set; }
            public GeometryData Geometry { get; set; }
            public MaterialData Material { get; set; }
            public ColorData Color { get; set; }
            public List<double> VertexPositions { get; set; }
            public List<double> VertexNormals { get; set; }
            public List<double> VertexColors { get; set; }
            public List<int> Indices { get; set; }
            
            public RayData()
            {
                StartPoint = new Point3DData();
                Direction = new Point3DData();
                EndPoint = new Point3DData();
                ColorIndex = 0;
                LineTypeName = "";
                LineWeight = 0.0;
                Length = 0.0;
                Angle = 0.0;
                Bounds = new BoundsData();
                Centroid = new Point3DData();
                Transform = new TransformData();
                Geometry = new GeometryData();
                Material = new MaterialData();
                Color = new ColorData();
                VertexPositions = new List<double>();
                VertexNormals = new List<double>();
                VertexColors = new List<double>();
                Indices = new List<int>();
            }
        }

        public class GeometryData
        {
            public string Type { get; set; }
            public int VertexCount { get; set; }
            public bool HasColors { get; set; }
            public bool HasNormals { get; set; }
            public bool HasIndices { get; set; }
            public string PrimitiveType { get; set; }
            public int IndexCount { get; set; }
            
            public GeometryData()
            {
                Type = "BufferGeometry";
                VertexCount = 0;
                HasColors = false;
                HasNormals = false;
                HasIndices = false;
                PrimitiveType = "LinePieces";
                IndexCount = 0;
            }
        }
        
        public class MaterialData
        {
            public string Type { get; set; }
            public int Color { get; set; }
            public double Opacity { get; set; }
            public bool Transparent { get; set; }
            public double LineWidth { get; set; }
            public bool VertexColors { get; set; }
            
            public MaterialData()
            {
                Type = "LineBasicMaterial";
                Color = 0xffffff;
                Opacity = 1.0;
                Transparent = false;
                LineWidth = 1.0;
                VertexColors = false;
            }
        }

        public static RayData Render(Ray ray)
        {
            var rayData = new RayData();
            
            double rayLength = 1000.0;
            
            double endX = ray.StartPoint.X + ray.Direction.X * rayLength;
            double endY = ray.StartPoint.Y + ray.Direction.Y * rayLength;
            double endZ = ray.StartPoint.Z + ray.Direction.Z * rayLength;
            
            double length = Math.Sqrt(
                Math.Pow(ray.Direction.X * rayLength, 2) +
                Math.Pow(ray.Direction.Y * rayLength, 2) +
                Math.Pow(ray.Direction.Z * rayLength, 2)
            );
            
            double angle = Math.Atan2(ray.Direction.Y, ray.Direction.X);
            
            int colorValue = GetColorRgbByIndex(ray.Color.Index);
            
            var colorData = new ColorData(ray.Color.Index);
            
            var transform = new TransformData();
            transform.Position = new Point3DData(ray.StartPoint.X, ray.StartPoint.Y, ray.StartPoint.Z);
            transform.Rotation = new Point3DData(0, 0, angle);
            transform.Scale = new Point3DData(1, 1, 1);
            UpdateTransformMatrix(transform, ray.StartPoint, ray.Direction, rayLength);
            
            var geometry = new GeometryData
            {
                Type = "BufferGeometry",
                VertexCount = 2,
                HasColors = true,
                HasNormals = false,
                HasIndices = true,
                PrimitiveType = "LinePieces",
                IndexCount = 2
            };
            
            var material = new MaterialData
            {
                Type = "LineBasicMaterial",
                Color = colorValue,
                Opacity = 1.0,
                Transparent = false,
                LineWidth = ray.GetActiveLineWeightType().GetLineWeightValue(),
                VertexColors = false
            };
            
            var vertexPositions = new List<double>
            {
                ray.StartPoint.X, ray.StartPoint.Y, ray.StartPoint.Z,
                endX, endY, endZ
            };
            
            var vertexColors = new List<double>
            {
                colorData.R / 255.0, colorData.G / 255.0, colorData.B / 255.0,
                colorData.R / 255.0, colorData.G / 255.0, colorData.B / 255.0
            };
            
            var vertexNormals = new List<double>
            {
                0, 0, 1,
                0, 0, 1
            };
            
            var indices = new List<int> { 0, 1 };
            
            var boundsMin = new Point3DData(
                Math.Min(ray.StartPoint.X, endX),
                Math.Min(ray.StartPoint.Y, endY),
                Math.Min(ray.StartPoint.Z, endZ)
            );
            var boundsMax = new Point3DData(
                Math.Max(ray.StartPoint.X, endX),
                Math.Max(ray.StartPoint.Y, endY),
                Math.Max(ray.StartPoint.Z, endZ)
            );
            var bounds = new BoundsData(boundsMin, boundsMax);
            
            var centroid = new Point3DData(
                (ray.StartPoint.X + endX) / 2,
                (ray.StartPoint.Y + endY) / 2,
                (ray.StartPoint.Z + endZ) / 2
            );

            rayData.StartPoint = new Point3DData(ray.StartPoint.X, ray.StartPoint.Y, ray.StartPoint.Z);
            rayData.Direction = new Point3DData(ray.Direction.X, ray.Direction.Y, ray.Direction.Z);
            rayData.ColorIndex = ray.Color.Index;
            rayData.LineTypeName = ray.GetActiveLineType()?.Name ?? "";
            rayData.LineWeight = ray.GetActiveLineWeightType().GetLineWeightValue();
            rayData.EndPoint = new Point3DData(endX, endY, endZ);
            rayData.Length = length;
            rayData.Angle = angle;
            rayData.Bounds = bounds;
            rayData.Centroid = centroid;
            rayData.Transform = transform;
            rayData.Geometry = geometry;
            rayData.Material = material;
            rayData.Color = colorData;
            rayData.VertexPositions = vertexPositions;
            rayData.VertexNormals = vertexNormals;
            rayData.VertexColors = vertexColors;
            rayData.Indices = indices;

            return rayData;
        }

        private static void UpdateTransformMatrix(TransformData transform, XYZ startPoint, XYZ direction, double length)
        {
            double dirLength = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);
            double normX = direction.X / dirLength;
            double normY = direction.Y / dirLength;
            double normZ = direction.Z / dirLength;
            
            double cosZ = Math.Cos(transform.Rotation.Z);
            double sinZ = Math.Sin(transform.Rotation.Z);
            
            transform.Matrix = new double[16] { 
                cosZ, sinZ, 0, 0,
                -sinZ, cosZ, 0, 0,
                0, 0, 1, 0,
                transform.Position.X, transform.Position.Y, transform.Position.Z, 1
            };
        }

        private static int GetColorRgbByIndex(short colorIndex)
        {
            // ACI colors (AutoCAD Color Index) - 返回RGB整数值
            switch (colorIndex)
            {
                case 1: return 0xff0000; // Red
                case 2: return 0xffff00; // Yellow
                case 3: return 0x00ff00; // Green
                case 4: return 0x00ffff; // Cyan
                case 5: return 0x0000ff; // Blue
                case 6: return 0xff00ff; // Magenta
                case 7: return 0xffffff; // White
                default: return 0x000000; // Black
            }
        }
    }
}