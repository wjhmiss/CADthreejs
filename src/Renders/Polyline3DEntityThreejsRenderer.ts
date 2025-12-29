import * as THREE from 'three';

export interface Point3DData {
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
  Location: Point3DData;
}

export interface SegmentData {
  Start: Point3DData;
  End: Point3DData;
  Length: number;
  Direction: Point3DData;
}

export interface GeometryData {
  Type: string;
  VertexCount: number;
  FaceCount: number;
  HasNormals: boolean;
  HasUVs: boolean;
  HasIndices: boolean;
  PrimitiveType: string;
  IndexCount: number;
  IsClosed: boolean;
  IsPeriodic: boolean;
  Degree: number;
}

export interface MaterialData {
  Type: string;
  Color: number;
  Opacity: number;
  Transparent: boolean;
  Wireframe: boolean;
  LineWidth: number;
  Side: boolean;
  VertexColors: boolean;
}

export interface Polyline3DData {
  Vertices: VertexData[];
  IsClosed: boolean;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  Elevation: number;
  Thickness: number;
  VertexCount: number;
  Bounds: BoundsData;
  Centroid: Point3DData;
  TotalLength: number;
  
  VertexPositions: number[];
  VertexNormals: number[];
  VertexUVs: number[];
  Indices: number[];
  Transform: TransformData;
  Material: MaterialData;
  Geometry: GeometryData;
  HasSegments: boolean;
  Segments: SegmentData[];
  
  EntityType: string;
  Visible: boolean;
  LayerName: string;
  LayerIndex: number;
  Handle: string;
  
  Transparency: number;
  MaterialName: string;
  
  CastShadows: boolean;
  ReceiveShadows: boolean;
  
  Color: ColorData;
  Normal: NormalData;
}

export class Polyline3DEntityThreejsRenderer {
  private static readonly DEFAULT_LINE_WIDTH = 1.0;
  private static readonly DEFAULT_OPACITY = 1.0;
  private static readonly DEFAULT_COLOR = '#FFFFFF';
  private static readonly polylineCache = new Map<string, THREE.Group>();

  public static render(polyline3DData: Polyline3DData, scene: THREE.Scene): THREE.Group | null {
    if (!polyline3DData || !polyline3DData.Visible) {
      return null;
    }

    const group = new THREE.Group();
    group.name = `Polyline3D_${polyline3DData.Handle}`;
    group.visible = polyline3DData.Visible;

    if (polyline3DData.Vertices.length === 0) {
      console.warn(`Polyline3D ${polyline3DData.Handle} has no vertices`);
      return group;
    }

    const geometry = this.createPolylineGeometry(polyline3DData);
    const material = this.createPolylineMaterial(polyline3DData);
    const line = new THREE.Line(geometry, material);
    line.name = 'Polyline3D';
    line.userData = { handle: polyline3DData.Handle };
    line.visible = polyline3DData.Visible;

    group.add(line);

    this.polylineCache.set(polyline3DData.Handle, group);

    return group;
  }

  public static update(polyline3DData: Polyline3DData, scene: THREE.Scene): boolean {
    const existingGroup = this.polylineCache.get(polyline3DData.Handle);
    if (!existingGroup) {
      return false;
    }

    const line = existingGroup.children[0] as THREE.Line;
    if (!line) {
      return false;
    }

    const material = line.material as THREE.LineBasicMaterial;
    if (material) {
      material.color.setHex(polyline3DData.Material.Color);
      material.opacity = polyline3DData.Material.Opacity;
      material.transparent = polyline3DData.Material.Transparent;
      material.linewidth = polyline3DData.Material.LineWidth;
    }

    const geometry = line.geometry as THREE.BufferGeometry;
    if (geometry) {
      const positions = geometry.attributes.position.array as Float32Array;
      for (let i = 0; i < polyline3DData.VertexPositions.length / 3; i++) {
        positions[i * 3] = polyline3DData.VertexPositions[i * 3];
        positions[i * 3 + 1] = polyline3DData.VertexPositions[i * 3 + 1];
        positions[i * 3 + 2] = polyline3DData.VertexPositions[i * 3 + 2];
      }
      geometry.attributes.position.needsUpdate = true;

      if (polyline3DData.VertexNormals && polyline3DData.VertexNormals.length > 0) {
        const normals = geometry.attributes.normal?.array as Float32Array;
        if (normals) {
          for (let i = 0; i < polyline3DData.VertexNormals.length; i++) {
            normals[i] = polyline3DData.VertexNormals[i];
          }
          geometry.attributes.normal.needsUpdate = true;
        }
      }
    }

    existingGroup.visible = polyline3DData.Visible;

    return true;
  }

  public static dispose(polyline3DData: Polyline3DData, scene: THREE.Scene): void {
    const group = this.polylineCache.get(polyline3DData.Handle);
    if (!group) {
      return;
    }

    scene.remove(group);

    group.children.forEach(child => {
      if (child instanceof THREE.Line) {
        if (child.geometry) {
          child.geometry.dispose();
        }
        if (child.material) {
          if (Array.isArray(child.material)) {
            child.material.forEach(m => m.dispose());
          } else {
            child.material.dispose();
          }
        }
      }
    });

    this.polylineCache.delete(polyline3DData.Handle);
  }

  public static setVisibility(polyline3DData: Polyline3DData, visible: boolean): void {
    const group = this.polylineCache.get(polyline3DData.Handle);
    if (group) {
      group.visible = visible;
      polyline3DData.Visible = visible;
    }
  }

  public static setOpacity(polyline3DData: Polyline3DData, opacity: number): void {
    const group = this.polylineCache.get(polyline3DData.Handle);
    if (group) {
      const line = group.children[0] as THREE.Line;
      if (line && line.material) {
        const material = line.material as THREE.LineBasicMaterial;
        material.opacity = opacity;
        material.transparent = opacity < 1.0;
        polyline3DData.Material.Opacity = opacity;
        polyline3DData.Material.Transparent = opacity < 1.0;
      }
    }
  }

  public static setColor(polyline3DData: Polyline3DData, color: number): void {
    const group = this.polylineCache.get(polyline3DData.Handle);
    if (group) {
      const line = group.children[0] as THREE.Line;
      if (line && line.material) {
        const material = line.material as THREE.LineBasicMaterial;
        material.color.setHex(color);
        polyline3DData.Material.Color = color;
      }
    }
  }

  public static getGroup(polyline3DData: Polyline3DData): THREE.Group | null {
    return this.polylineCache.get(polyline3DData.Handle) || null;
  }

  public static getLine(polyline3DData: Polyline3DData): THREE.Line | null {
    const group = this.polylineCache.get(polyline3DData.Handle);
    if (group && group.children.length > 0) {
      return group.children[0] as THREE.Line;
    }
    return null;
  }

  public static getVertices(polyline3DData: Polyline3DData): VertexData[] {
    return polyline3DData.Vertices;
  }

  public static getVertexPositions(polyline3DData: Polyline3DData): number[] {
    return polyline3DData.VertexPositions;
  }

  public static getVertexNormals(polyline3DData: Polyline3DData): number[] {
    return polyline3DData.VertexNormals;
  }

  public static getVertexUVs(polyline3DData: Polyline3DData): number[] {
    return polyline3DData.VertexUVs;
  }

  public static getIndices(polyline3DData: Polyline3DData): number[] {
    return polyline3DData.Indices;
  }

  public static getSegments(polyline3DData: Polyline3DData): SegmentData[] {
    return polyline3DData.Segments;
  }

  public static getSegment(polyline3DData: Polyline3DData, index: number): SegmentData | null {
    if (index >= 0 && index < polyline3DData.Segments.length) {
      return polyline3DData.Segments[index];
    }
    return null;
  }

  public static getSegmentLength(polyline3DData: Polyline3DData, index: number): number {
    const segment = this.getSegment(polyline3DData, index);
    return segment ? segment.Length : 0;
  }

  public static getSegmentDirection(polyline3DData: Polyline3DData, index: number): Point3DData | null {
    const segment = this.getSegment(polyline3DData, index);
    return segment ? segment.Direction : null;
  }

  public static getColor(polyline3DData: Polyline3DData): ColorData {
    return polyline3DData.Color;
  }

  public static getBounds(polyline3DData: Polyline3DData): BoundsData {
    return polyline3DData.Bounds;
  }

  public static getCentroid(polyline3DData: Polyline3DData): Point3DData {
    return polyline3DData.Centroid;
  }

  public static getHandle(polyline3DData: Polyline3DData): string {
    return polyline3DData.Handle;
  }

  public static getLayerName(polyline3DData: Polyline3DData): string {
    return polyline3DData.LayerName;
  }

  public static getVisible(polyline3DData: Polyline3DData): boolean {
    return polyline3DData.Visible;
  }

  public static getOpacity(polyline3DData: Polyline3DData): number {
    return polyline3DData.Material.Opacity;
  }

  public static getTransparent(polyline3DData: Polyline3DData): boolean {
    return polyline3DData.Material.Transparent;
  }

  public static getCastShadows(polyline3DData: Polyline3DData): boolean {
    return polyline3DData.CastShadows;
  }

  public static getReceiveShadows(polyline3DData: Polyline3DData): boolean {
    return polyline3DData.ReceiveShadows;
  }

  public static getTransform(polyline3DData: Polyline3DData): TransformData {
    return polyline3DData.Transform;
  }

  public static getGeometry(polyline3DData: Polyline3DData): GeometryData {
    return polyline3DData.Geometry;
  }

  public static getMaterial(polyline3DData: Polyline3DData): MaterialData {
    return polyline3DData.Material;
  }

  public static getVertexCount(polyline3DData: Polyline3DData): number {
    return polyline3DData.VertexCount;
  }

  public static getTotalLength(polyline3DData: Polyline3DData): number {
    return polyline3DData.TotalLength;
  }

  public static getElevation(polyline3DData: Polyline3DData): number {
    return polyline3DData.Elevation;
  }

  public static getThickness(polyline3DData: Polyline3DData): number {
    return polyline3DData.Thickness;
  }

  public static getIsClosed(polyline3DData: Polyline3DData): boolean {
    return polyline3DData.IsClosed;
  }

  public static getHasSegments(polyline3DData: Polyline3DData): boolean {
    return polyline3DData.HasSegments;
  }

  public static getLineTypeName(polyline3DData: Polyline3DData): string {
    return polyline3DData.LineTypeName;
  }

  public static getLineWeight(polyline3DData: Polyline3DData): number {
    return polyline3DData.LineWeight;
  }

  public static getEntityType(polyline3DData: Polyline3DData): string {
    return polyline3DData.EntityType;
  }

  public static clearCache(): void {
    this.polylineCache.forEach((group, handle) => {
      group.children.forEach(child => {
        if (child instanceof THREE.Line) {
          if (child.geometry) {
            child.geometry.dispose();
          }
          if (child.material) {
            if (Array.isArray(child.material)) {
              child.material.forEach(m => m.dispose());
            } else {
              child.material.dispose();
            }
          }
        }
      });
    });
    this.polylineCache.clear();
  }

  private static createPolylineGeometry(polyline3DData: Polyline3DData): THREE.BufferGeometry {
    const geometry = new THREE.BufferGeometry();

    const positions = new Float32Array(polyline3DData.VertexPositions);
    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));

    if (polyline3DData.VertexNormals && polyline3DData.VertexNormals.length > 0) {
      const normals = new Float32Array(polyline3DData.VertexNormals);
      geometry.setAttribute('normal', new THREE.BufferAttribute(normals, 3));
    }

    if (polyline3DData.VertexUVs && polyline3DData.VertexUVs.length > 0) {
      const uvs = new Float32Array(polyline3DData.VertexUVs);
      geometry.setAttribute('uv', new THREE.BufferAttribute(uvs, 2));
    }

    if (polyline3DData.Indices && polyline3DData.Indices.length > 0) {
      geometry.setIndex(polyline3DData.Indices);
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static createPolylineMaterial(polyline3DData: Polyline3DData): THREE.LineBasicMaterial {
    const material = new THREE.LineBasicMaterial({
      color: polyline3DData.Material.Color,
      linewidth: polyline3DData.Material.LineWidth,
      opacity: polyline3DData.Material.Opacity,
      transparent: polyline3DData.Material.Transparent,
      depthTest: true,
      depthWrite: !polyline3DData.Material.Transparent
    });

    return material;
  }

  public static renderFromJson(jsonString: string, scene: THREE.Scene): THREE.Group | null {
    try {
      const polyline3DData: Polyline3DData = JSON.parse(jsonString);
      return this.render(polyline3DData, scene);
    } catch (error) {
      console.error('Failed to parse Polyline3D JSON:', error);
      return null;
    }
  }

  public static updateFromJson(jsonString: string, scene: THREE.Scene): boolean {
    try {
      const polyline3DData: Polyline3DData = JSON.parse(jsonString);
      return this.update(polyline3DData, scene);
    } catch (error) {
      console.error('Failed to parse Polyline3D JSON:', error);
      return false;
    }
  }

  public static disposeFromJson(jsonString: string, scene: THREE.Scene): void {
    try {
      const polyline3DData: Polyline3DData = JSON.parse(jsonString);
      this.dispose(polyline3DData, scene);
    } catch (error) {
      console.error('Failed to parse Polyline3D JSON:', error);
    }
  }

  public static getSegmentPointAt(polyline3DData: Polyline3DData, segmentIndex: number, t: number): Point3DData | null {
    const segment = this.getSegment(polyline3DData, segmentIndex);
    if (!segment || t < 0 || t > 1) {
      return null;
    }

    return {
      X: segment.Start.X + (segment.End.X - segment.Start.X) * t,
      Y: segment.Start.Y + (segment.End.Y - segment.Start.Y) * t,
      Z: segment.Start.Z + (segment.End.Z - segment.Start.Z) * t
    };
  }

  public static getPointAtLength(polyline3DData: Polyline3DData, length: number): Point3DData | null {
    if (length < 0 || length > polyline3DData.TotalLength) {
      return null;
    }

    let accumulatedLength = 0;
    for (let i = 0; i < polyline3DData.Segments.length; i++) {
      const segment = polyline3DData.Segments[i];
      if (accumulatedLength + segment.Length >= length) {
        const t = (length - accumulatedLength) / segment.Length;
        return this.getSegmentPointAt(polyline3DData, i, t);
      }
      accumulatedLength += segment.Length;
    }

    return polyline3DData.Vertices[polyline3DData.Vertices.length - 1].Location;
  }

  public static getClosestPoint(polyline3DData: Polyline3DData, point: Point3DData): Point3DData | null {
    let closestPoint: Point3DData | null = null;
    let minDistance = Infinity;

    for (let i = 0; i < polyline3DData.Segments.length; i++) {
      const segment = polyline3DData.Segments[i];
      const segmentClosestPoint = this.getClosestPointOnSegment(segment, point);
      const distance = this.calculateDistance(point, segmentClosestPoint);

      if (distance < minDistance) {
        minDistance = distance;
        closestPoint = segmentClosestPoint;
      }
    }

    return closestPoint;
  }

  private static getClosestPointOnSegment(segment: SegmentData, point: Point3DData): Point3DData {
    const dx = segment.End.X - segment.Start.X;
    const dy = segment.End.Y - segment.Start.Y;
    const dz = segment.End.Z - segment.Start.Z;

    const t = Math.max(0, Math.min(1, 
      ((point.X - segment.Start.X) * dx + 
       (point.Y - segment.Start.Y) * dy + 
       (point.Z - segment.Start.Z) * dz) / 
      (dx * dx + dy * dy + dz * dz)
    ));

    return {
      X: segment.Start.X + dx * t,
      Y: segment.Start.Y + dy * t,
      Z: segment.Start.Z + dz * t
    };
  }

  private static calculateDistance(p1: Point3DData, p2: Point3DData): number {
    const dx = p2.X - p1.X;
    const dy = p2.Y - p1.Y;
    const dz = p2.Z - p1.Z;
    return Math.sqrt(dx * dx + dy * dy + dz * dz);
  }

  public static getSegmentCount(polyline3DData: Polyline3DData): number {
    return polyline3DData.Segments.length;
  }

  public static getAverageSegmentLength(polyline3DData: Polyline3DData): number {
    if (polyline3DData.Segments.length === 0) {
      return 0;
    }
    return polyline3DData.TotalLength / polyline3DData.Segments.length;
  }

  public static getLongestSegment(polyline3DData: Polyline3DData): SegmentData | null {
    if (polyline3DData.Segments.length === 0) {
      return null;
    }

    let longestSegment = polyline3DData.Segments[0];
    for (let i = 1; i < polyline3DData.Segments.length; i++) {
      if (polyline3DData.Segments[i].Length > longestSegment.Length) {
        longestSegment = polyline3DData.Segments[i];
      }
    }

    return longestSegment;
  }

  public static getShortestSegment(polyline3DData: Polyline3DData): SegmentData | null {
    if (polyline3DData.Segments.length === 0) {
      return null;
    }

    let shortestSegment = polyline3DData.Segments[0];
    for (let i = 1; i < polyline3DData.Segments.length; i++) {
      if (polyline3DData.Segments[i].Length < shortestSegment.Length) {
        shortestSegment = polyline3DData.Segments[i];
      }
    }

    return shortestSegment;
  }
}
