import * as THREE from 'three';

export interface Point3DData {
  X: number;
  Y: number;
  Z: number;
}

export interface PointData {
  X: number;
  Y: number;
  Z: number;
}

export interface BoundsData {
  MinX: number;
  MaxX: number;
  MinY: number;
  MaxY: number;
  MinZ: number;
  MaxZ: number;
}

export interface BoundsData3D {
  Min: Point3DData;
  Max: Point3DData;
}

export interface ColorData {
  Index: number;
  Hex: string;
  R: number;
  G: number;
  B: number;
  A: number;
}

export interface TransformData {
  Position: Point3DData;
  Rotation: Point3DData;
  Scale: Point3DData;
  Matrix: number[];
}

export interface NormalData {
  X: number;
  Y: number;
  Z: number;
}

export interface VertexData {
  Location: PointData;
  Bulge: number;
  StartWidth: number;
  EndWidth: number;
}

export interface ArcSegmentData {
  StartPoint: PointData;
  EndPoint: PointData;
  CenterPoint: PointData;
  Radius: number;
  StartAngle: number;
  EndAngle: number;
  SweepAngle: number;
  IsCounterClockwise: boolean;
  SegmentCount: number;
}

export interface GeometryData {
  Type: string;
  VertexCount: number;
  FaceCount: number;
  HasNormals: boolean;
  HasUVs: boolean;
  HasIndices: boolean;
}

export interface MaterialData {
  Type: string;
  Color: number;
  Opacity: number;
  Transparent: boolean;
  Wireframe: boolean;
  LineWidth: number;
  Side: boolean;
}

export interface Polyline2DData {
  Vertices: VertexData[];
  IsClosed: boolean;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  LineTypeScale: number;
  Elevation: number;
  Thickness: number;
  StartWidth: number;
  EndWidth: number;
  SmoothSurface: number;
  
  VertexCount: number;
  SegmentCount: number;
  Bounds3D: BoundsData3D | null;
  Bounds: BoundsData | null;
  Centroid: Point3DData | null;
  TotalLength: number;
  
  Vertices3D: Point3DData[];
  Indices: number[];
  NormalsArray: number[];
  ColorsArray: number[];
  UVsArray: number[];
  Color: ColorData;
  Normal: NormalData;
  Transform: TransformData;
  
  EntityType: string;
  Visible: boolean;
  LayerName: string;
  LayerIndex: number;
  Handle: string;
  
  Transparency: number;
  MaterialName: string;
  Material: MaterialData;
  
  CastShadows: boolean;
  ReceiveShadows: boolean;
  
  GeometryType: string;
  DoubleSided: boolean;
  FlatShading: boolean;
  
  HasArcSegments: boolean;
  ArcSegments: ArcSegmentData[];
  
  HasVariableWidth: boolean;
  
  Geometry: GeometryData;
  
  VertexPositions: number[];
  VertexNormals: number[];
  VertexUVs: number[];
}

export class Polyline2DEntityThreejsRenderer {
  private static readonly DEFAULT_LINE_WIDTH = 1.0;
  private static readonly DEFAULT_OPACITY = 1.0;
  private static readonly DEFAULT_COLOR = '#000000';
  private static readonly ARC_SEGMENTS_PER_DEGREE = 0.5;
  private static readonly polylineCache = new Map<string, THREE.Line>();

  public static render(polyline2DData: Polyline2DData, scene: THREE.Scene): THREE.Line | null {
    if (!polyline2DData || !polyline2DData.Visible) {
      return null;
    }

    if (polyline2DData.Vertices3D.length === 0) {
      console.warn(`Polyline2D ${polyline2DData.Handle} has no vertices`);
      return null;
    }

    const geometry = this.createPolylineGeometry(polyline2DData);
    const material = this.createPolylineMaterial(polyline2DData);
    const line = new THREE.Line(geometry, material);
    line.name = `Polyline2D_${polyline2DData.Handle}`;
    line.userData = {
      type: polyline2DData.Type,
      entityType: polyline2DData.EntityType,
      handle: polyline2DData.Handle,
      layerName: polyline2DData.LayerName
    };
    line.visible = polyline2DData.Visible;

    this.polylineCache.set(polyline2DData.Handle, line);

    return line;
  }

  public static update(polyline2DData: Polyline2DData, scene: THREE.Scene): boolean {
    const existingLine = this.polylineCache.get(polyline2DData.Handle);
    if (!existingLine) {
      return false;
    }

    const material = existingLine.material as THREE.LineBasicMaterial;
    if (material) {
      material.color.setHex(parseInt(polyline2DData.Color.Hex.replace('#', ''), 16));
      material.opacity = 1.0 - polyline2DData.Transparency;
      material.transparent = polyline2DData.Transparency > 0;
      material.linewidth = polyline2DData.LineWeight;
    }

    const geometry = existingLine.geometry as THREE.BufferGeometry;
    if (geometry) {
      const positions = geometry.attributes.position.array as Float32Array;
      for (let i = 0; i < polyline2DData.Vertices3D.length; i++) {
        positions[i * 3] = polyline2DData.Vertices3D[i].X;
        positions[i * 3 + 1] = polyline2DData.Vertices3D[i].Y;
        positions[i * 3 + 2] = polyline2DData.Vertices3D[i].Z;
      }
      geometry.attributes.position.needsUpdate = true;
    }

    existingLine.visible = polyline2DData.Visible;

    return true;
  }

  public static dispose(polyline2DData: Polyline2DData, scene: THREE.Scene): void {
    const line = this.polylineCache.get(polyline2DData.Handle);
    if (!line) {
      return;
    }

    scene.remove(line);

    if (line.geometry) {
      line.geometry.dispose();
    }
    if (line.material) {
      if (Array.isArray(line.material)) {
        line.material.forEach(m => m.dispose());
      } else {
        line.material.dispose();
      }
    }

    this.polylineCache.delete(polyline2DData.Handle);
  }

  public static setVisibility(polyline2DData: Polyline2DData, visible: boolean): void {
    const line = this.polylineCache.get(polyline2DData.Handle);
    if (line) {
      line.visible = visible;
      polyline2DData.Visible = visible;
    }
  }

  public static setOpacity(polyline2DData: Polyline2DData, opacity: number): void {
    const line = this.polylineCache.get(polyline2DData.Handle);
    if (line && line.material) {
      const material = line.material as THREE.LineBasicMaterial;
      material.opacity = opacity;
      material.transparent = opacity < 1.0;
    }
  }

  public static setColor(polyline2DData: Polyline2DData, color: string): void {
    const line = this.polylineCache.get(polyline2DData.Handle);
    if (line && line.material) {
      const material = line.material as THREE.LineBasicMaterial;
      material.color.set(color);
    }
  }

  public static getLine(polyline2DData: Polyline2DData): THREE.Line | null {
    return this.polylineCache.get(polyline2DData.Handle) || null;
  }

  public static getVertices(polyline2DData: Polyline2DData): Point3DData[] {
    return polyline2DData.Vertices3D;
  }

  public static getIndices(polyline2DData: Polyline2DData): number[] {
    return polyline2DData.Indices;
  }

  public static getNormals(polyline2DData: Polyline2DData): number[] {
    return polyline2DData.NormalsArray;
  }

  public static getColors(polyline2DData: Polyline2DData): number[] {
    return polyline2DData.ColorsArray;
  }

  public static getUVs(polyline2DData: Polyline2DData): number[] {
    return polyline2DData.UVsArray;
  }

  public static getColor(polyline2DData: Polyline2DData): ColorData {
    return polyline2DData.Color;
  }

  public static getBounds(polyline2DData: Polyline2DData): BoundsData | null {
    return polyline2DData.Bounds;
  }

  public static getBounds3D(polyline2DData: Polyline2DData): BoundsData3D | null {
    return polyline2DData.Bounds3D;
  }

  public static getCentroid(polyline2DData: Polyline2DData): Point3DData | null {
    return polyline2DData.Centroid;
  }

  public static getCentroid3D(polyline2DData: Polyline2DData): Point3DData | null {
    return polyline2DData.Centroid;
  }

  public static getHandle(polyline2DData: Polyline2DData): string {
    return polyline2DData.Handle;
  }

  public static getLayerName(polyline2DData: Polyline2DData): string {
    return polyline2DData.LayerName;
  }

  public static getVisible(polyline2DData: Polyline2DData): boolean {
    return polyline2DData.Visible;
  }

  public static getOpacity(polyline2DData: Polyline2DData): number {
    return 1.0 - polyline2DData.Transparency;
  }

  public static getTransparent(polyline2DData: Polyline2DData): boolean {
    return polyline2DData.Transparency > 0;
  }

  public static getCastShadows(polyline2DData: Polyline2DData): boolean {
    return polyline2DData.CastShadows;
  }

  public static getReceiveShadows(polyline2DData: Polyline2DData): boolean {
    return polyline2DData.ReceiveShadows;
  }

  public static getDoubleSided(polyline2DData: Polyline2DData): boolean {
    return polyline2DData.DoubleSided;
  }

  public static getFlatShading(polyline2DData: Polyline2DData): boolean {
    return polyline2DData.FlatShading;
  }

  public static getGeometryType(polyline2DData: Polyline2DData): string {
    return polyline2DData.GeometryType;
  }

  public static getEntityType(polyline2DData: Polyline2DData): string {
    return polyline2DData.EntityType;
  }

  public static getVertexCount(polyline2DData: Polyline2DData): number {
    return polyline2DData.VertexCount;
  }

  public static getSegmentCount(polyline2DData: Polyline2DData): number {
    return polyline2DData.SegmentCount;
  }

  public static getTotalLength(polyline2DData: Polyline2DData): number {
    return polyline2DData.TotalLength;
  }

  public static getElevation(polyline2DData: Polyline2DData): number {
    return polyline2DData.Elevation;
  }

  public static getThickness(polyline2DData: Polyline2DData): number {
    return polyline2DData.Thickness;
  }

  public static getIsClosed(polyline2DData: Polyline2DData): boolean {
    return polyline2DData.IsClosed;
  }

  public static getHasArcSegments(polyline2DData: Polyline2DData): boolean {
    return polyline2DData.HasArcSegments;
  }

  public static getArcSegments(polyline2DData: Polyline2DData): ArcSegmentData[] {
    return polyline2DData.ArcSegments;
  }

  public static getHasVariableWidth(polyline2DData: Polyline2DData): boolean {
    return polyline2DData.HasVariableWidth;
  }

  public static getTransform(polyline2DData: Polyline2DData): TransformData {
    return polyline2DData.Transform;
  }

  public static clearCache(): void {
    this.polylineCache.forEach((line, handle) => {
      if (line.geometry) {
        line.geometry.dispose();
      }
      if (line.material) {
        if (Array.isArray(line.material)) {
          line.material.forEach(m => m.dispose());
        } else {
          line.material.dispose();
        }
      }
    });
    this.polylineCache.clear();
  }

  private static createPolylineGeometry(polyline2DData: Polyline2DData): THREE.BufferGeometry {
    const geometry = new THREE.BufferGeometry();

    if (polyline2DData.HasArcSegments) {
      return this.createArcSegmentGeometry(polyline2DData);
    }

    const positions = new Float32Array(polyline2DData.Vertices3D.length * 3);
    for (let i = 0; i < polyline2DData.Vertices3D.length; i++) {
      positions[i * 3] = polyline2DData.Vertices3D[i].X;
      positions[i * 3 + 1] = polyline2DData.Vertices3D[i].Y;
      positions[i * 3 + 2] = polyline2DData.Vertices3D[i].Z;
    }
    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));

    if (polyline2DData.NormalsArray.length > 0) {
      const normals = new Float32Array(polyline2DData.NormalsArray);
      geometry.setAttribute('normal', new THREE.BufferAttribute(normals, 3));
    }

    if (polyline2DData.ColorsArray.length > 0) {
      const colors = new Float32Array(polyline2DData.ColorsArray);
      geometry.setAttribute('color', new THREE.BufferAttribute(colors, 4));
    }

    if (polyline2DData.UVsArray.length > 0) {
      const uvs = new Float32Array(polyline2DData.UVsArray);
      geometry.setAttribute('uv', new THREE.BufferAttribute(uvs, 2));
    }

    if (polyline2DData.Indices.length > 0) {
      geometry.setIndex(polyline2DData.Indices);
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static createArcSegmentGeometry(polyline2DData: Polyline2DData): THREE.BufferGeometry {
    const geometry = new THREE.BufferGeometry();
    const vertices: number[] = [];

    for (let i = 0; i < polyline2DData.Vertices.length; i++) {
      const vertex = polyline2DData.Vertices[i];
      const nextVertexIndex = (i + 1) % polyline2DData.Vertices.length;
      const nextVertex = polyline2DData.Vertices[nextVertexIndex];

      if (vertex.Bulge === 0) {
        vertices.push(vertex.Location.X, vertex.Location.Y, vertex.Location.Z);
      } else {
        const arcVertices = this.generateArcVertices(vertex, nextVertex, polyline2DData.IsClosed && i === polyline2DData.Vertices.length - 1);
        vertices.push(...arcVertices);
      }
    }

    if (polyline2DData.IsClosed && polyline2DData.Vertices.length > 0) {
      const firstVertex = polyline2DData.Vertices[0];
      vertices.push(firstVertex.Location.X, firstVertex.Location.Y, firstVertex.Location.Z);
    }

    const positions = new Float32Array(vertices);
    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));

    if (polyline2DData.NormalsArray.length > 0) {
      const normals = new Float32Array(polyline2DData.NormalsArray);
      geometry.setAttribute('normal', new THREE.BufferAttribute(normals, 3));
    }

    if (polyline2DData.ColorsArray.length > 0) {
      const colors = new Float32Array(polyline2DData.ColorsArray);
      geometry.setAttribute('color', new THREE.BufferAttribute(colors, 4));
    }

    if (polyline2DData.UVsArray.length > 0) {
      const uvs = new Float32Array(polyline2DData.UVsArray);
      geometry.setAttribute('uv', new THREE.BufferAttribute(uvs, 2));
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static generateArcVertices(startVertex: VertexData, endVertex: VertexData, isLastSegment: boolean): number[] {
    const vertices: number[] = [];
    const bulge = startVertex.Bulge;
    const startPoint = { x: startVertex.Location.X, y: startVertex.Location.Y };
    const endPoint = { x: endVertex.Location.X, y: endVertex.Location.Y };

    const alpha = Math.atan(bulge) * 4.0;
    const tanAlphaHalf = Math.tan(alpha / 2.0);

    const middlePoint = {
      x: startPoint.x + (endPoint.x - startPoint.x) / 2.0,
      y: startPoint.y + (endPoint.y - startPoint.y) / 2.0
    };

    const perpendicular = {
      x: -(endPoint.y - startPoint.y),
      y: endPoint.x - startPoint.x
    };

    const centerPoint = {
      x: middlePoint.x - perpendicular.x * tanAlphaHalf,
      y: middlePoint.y - perpendicular.y * tanAlphaHalf
    };

    const dx = startPoint.x - centerPoint.x;
    const dy = startPoint.y - centerPoint.y;
    const radius = Math.sqrt(dx * dx + dy * dy);

    let startAngle = Math.atan2(startPoint.y - centerPoint.y, startPoint.x - centerPoint.x);
    let endAngle = Math.atan2(endPoint.y - centerPoint.y, endPoint.x - centerPoint.x);

    let sweepAngle = endAngle - startAngle;

    if (bulge < 0 && sweepAngle > 0) {
      sweepAngle -= 2 * Math.PI;
    } else if (bulge > 0 && sweepAngle < 0) {
      sweepAngle += 2 * Math.PI;
    }

    const segmentCount = Math.max(8, Math.floor(Math.abs(sweepAngle) * 180 / Math.PI * this.ARC_SEGMENTS_PER_DEGREE));

    for (let i = 0; i <= segmentCount; i++) {
      const t = i / segmentCount;
      const angle = startAngle + sweepAngle * t;
      const x = centerPoint.x + radius * Math.cos(angle);
      const y = centerPoint.y + radius * Math.sin(angle);
      const z = startVertex.Location.Z;
      vertices.push(x, y, z);
    }

    return vertices;
  }

  private static createPolylineMaterial(polyline2DData: Polyline2DData): THREE.LineBasicMaterial {
    const material = new THREE.LineBasicMaterial({
      color: parseInt(polyline2DData.Color.Hex.replace('#', ''), 16),
      linewidth: polyline2DData.LineWeight,
      opacity: 1.0 - polyline2DData.Transparency,
      transparent: polyline2DData.Transparency > 0,
      depthTest: true,
      depthWrite: polyline2DData.Transparency <= 0
    });

    return material;
  }

  public static renderFromJson(jsonString: string, scene: THREE.Scene): THREE.Line | null {
    try {
      const polyline2DData: Polyline2DData = JSON.parse(jsonString);
      return this.render(polyline2DData, scene);
    } catch (error) {
      console.error('Failed to parse Polyline2D JSON:', error);
      return null;
    }
  }

  public static updateFromJson(jsonString: string, scene: THREE.Scene): boolean {
    try {
      const polyline2DData: Polyline2DData = JSON.parse(jsonString);
      return this.update(polyline2DData, scene);
    } catch (error) {
      console.error('Failed to parse Polyline2D JSON:', error);
      return false;
    }
  }

  public static disposeFromJson(jsonString: string, scene: THREE.Scene): void {
    try {
      const polyline2DData: Polyline2DData = JSON.parse(jsonString);
      this.dispose(polyline2DData, scene);
    } catch (error) {
      console.error('Failed to parse Polyline2D JSON:', error);
    }
  }
}
