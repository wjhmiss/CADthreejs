import * as THREE from 'three';

export interface Point3DData {
  X: number;
  Y: number;
  Z: number;
}

export interface BoundsData {
  Min: Point3DData;
  Max: Point3DData;
  Center: Point3DData;
  Size: Point3DData;
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

export interface GeometryData {
  Type: string;
  VertexCount: number;
  FaceCount?: number;
  HasNormals?: boolean;
  HasColors: boolean;
  HasUVs?: boolean;
  HasIndices: boolean;
  PrimitiveType: string;
  IndexCount: number;
  IsClosed?: boolean;
  IsPeriodic?: boolean;
}

export interface MaterialData {
  Type: string;
  Color: number;
  Opacity: number;
  Transparent: boolean;
  Wireframe: boolean;
  LineWidth: number;
  VertexColors: boolean;
  Side: boolean;
}

export interface SplineData {
  ControlPoints: Point3DData[];
  FitPoints: Point3DData[];
  Knots: number[];
  Weights: number[];
  Degree: number;
  IsClosed: boolean;
  IsPeriodic: boolean;
  StartTangent: Point3DData;
  EndTangent: Point3DData;
  Normal: Point3DData;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  ControlPointTolerance: number;
  FitTolerance: number;
  KnotTolerance: number;
  ApproximationPoints: Point3DData[];
  ControlPointCount: number;
  FitPointCount: number;
  KnotCount: number;
  Bounds: BoundsData;
  Centroid: Point3DData;
  Length: number;
  Transform: TransformData;
  Geometry: GeometryData;
  Material: MaterialData;
  VertexPositions: number[];
  VertexColors: number[];
  Indices: number[];
  ApproximationPointCount: number;
  Tension: number;
  SplineType: string;
  Color: ColorData;
  Tangent: NormalData;
  Binormal: number[];
  UV: number[];
  ArcLength: number;
  Curvature: number;
  IsRational: boolean;
  SampleCount: number;
  ControlPointWeights: number[];
  Type: string;
  Handle: string;
  Visible: boolean;
  LayerIndex: number;
  LayerName: string;
  EntityType: string;
  Opacity: number;
  Transparent: boolean;
  DepthTest: boolean;
}

export class SplineEntityThreejsRenderer {
  private static readonly DEFAULT_LINE_WIDTH = 1.0;
  private static readonly DEFAULT_OPACITY = 1.0;
  private static readonly DEFAULT_COLOR = '#000000';
  private static readonly splineCache = new Map<string, THREE.Line>();

  public static render(splineData: SplineData, scene: THREE.Scene): THREE.Line | null {
    if (!splineData || !splineData.Visible) {
      return null;
    }

    const geometry = this.createSplineGeometry(splineData);
    const material = this.createSplineMaterial(splineData);
    const line = new THREE.Line(geometry, material);
    line.name = `Spline_${splineData.Handle}`;
    line.visible = splineData.Visible;
    line.userData = {
      type: splineData.Type,
      entityType: splineData.EntityType,
      handle: splineData.Handle,
      layerName: splineData.LayerName,
      layerIndex: splineData.LayerIndex,
      visible: splineData.Visible,
      opacity: splineData.Opacity,
      transparent: splineData.Transparent,
      depthTest: splineData.DepthTest,
      splineData: splineData,
      objectType: 'Spline'
    };

    this.splineCache.set(splineData.Handle, line);

    return line;
  }

  public static update(splineData: SplineData, scene: THREE.Scene): boolean {
    const existingLine = this.splineCache.get(splineData.Handle);
    if (!existingLine) {
      return false;
    }

    const material = existingLine.material as THREE.LineBasicMaterial;
    if (material) {
      material.color.setHex(parseInt(splineData.Color.Hex.replace('#', ''), 16));
      material.opacity = splineData.Opacity;
      material.transparent = splineData.Transparent;
      material.linewidth = splineData.Material.LineWidth;
    }

    const geometry = existingLine.geometry as THREE.BufferGeometry;
    if (geometry && splineData.VertexPositions && splineData.VertexPositions.length > 0) {
      const positions = geometry.attributes.position.array as Float32Array;
      for (let i = 0; i < splineData.VertexPositions.length && i < positions.length; i++) {
        positions[i] = splineData.VertexPositions[i];
      }
      geometry.attributes.position.needsUpdate = true;
    }

    return true;
  }

  public static dispose(splineData: SplineData, scene: THREE.Scene): boolean {
    const existingLine = this.splineCache.get(splineData.Handle);
    if (!existingLine) {
      return false;
    }

    scene.remove(existingLine);
    existingLine.geometry.dispose();
    (existingLine.material as THREE.Material).dispose();
    this.splineCache.delete(splineData.Handle);

    return true;
  }

  public static setVisibility(splineData: SplineData, scene: THREE.Scene, visible: boolean): boolean {
    const existingLine = this.splineCache.get(splineData.Handle);
    if (!existingLine) {
      return false;
    }

    existingLine.visible = visible;
    return true;
  }

  public static setOpacity(splineData: SplineData, opacity: number): boolean {
    const existingLine = this.splineCache.get(splineData.Handle);
    if (!existingLine) {
      return false;
    }

    const material = existingLine.material as THREE.LineBasicMaterial;
    if (material) {
      material.opacity = opacity;
      material.transparent = opacity < 1.0;
    }

    return true;
  }

  public static setLineWidth(splineData: SplineData, lineWidth: number): boolean {
    const existingLine = this.splineCache.get(splineData.Handle);
    if (!existingLine) {
      return false;
    }

    const material = existingLine.material as THREE.LineBasicMaterial;
    if (material) {
      material.linewidth = lineWidth;
    }

    return true;
  }

  public static getSplineLine(splineData: SplineData): THREE.Line | null {
    return this.splineCache.get(splineData.Handle) || null;
  }

  public static getControlPoints(splineData: SplineData): THREE.Vector3[] {
    return splineData.ControlPoints.map(p => new THREE.Vector3(p.X, p.Y, p.Z));
  }

  public static getFitPoints(splineData: SplineData): THREE.Vector3[] {
    return splineData.FitPoints.map(p => new THREE.Vector3(p.X, p.Y, p.Z));
  }

  public static getApproximationPoints(splineData: SplineData): THREE.Vector3[] {
    return splineData.ApproximationPoints.map(p => new THREE.Vector3(p.X, p.Y, p.Z));
  }

  public static getDegree(splineData: SplineData): number {
    return splineData.Degree || 3;
  }

  public static isClosed(splineData: SplineData): boolean {
    return splineData.IsClosed || false;
  }

  public static isRational(splineData: SplineData): boolean {
    return splineData.IsRational || false;
  }

  public static getLength(splineData: SplineData): number {
    return splineData.Length || 0;
  }

  public static getColor(splineData: SplineData): THREE.Color {
    return new THREE.Color(splineData.Color.Hex);
  }

  public static getBounds(splineData: SplineData): THREE.Box3 {
    const min = new THREE.Vector3(splineData.Bounds.Min.X, splineData.Bounds.Min.Y, splineData.Bounds.Min.Z);
    const max = new THREE.Vector3(splineData.Bounds.Max.X, splineData.Bounds.Max.Y, splineData.Bounds.Max.Z);
    return new THREE.Box3(min, max);
  }

  public static getCentroid(splineData: SplineData): THREE.Vector3 {
    return new THREE.Vector3(splineData.Centroid.X, splineData.Centroid.Y, splineData.Centroid.Z);
  }

  public static clearCache(): void {
    this.splineCache.forEach((line) => {
      line.geometry.dispose();
      (line.material as THREE.Material).dispose();
    });
    this.splineCache.clear();
  }

  private static createSplineGeometry(splineData: SplineData): THREE.BufferGeometry {
    const geometry = new THREE.BufferGeometry();

    if (splineData.VertexPositions && splineData.VertexPositions.length > 0) {
      const positions = new Float32Array(splineData.VertexPositions);
      geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    } else if (splineData.ApproximationPoints && splineData.ApproximationPoints.length > 0) {
      const positions: number[] = [];
      splineData.ApproximationPoints.forEach(point => {
        positions.push(point.X, point.Y, point.Z);
      });
      geometry.setAttribute('position', new THREE.BufferAttribute(new Float32Array(positions), 3));
    } else {
      const positions = new Float32Array([0, 0, 0]);
      geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    }

    if (splineData.VertexColors && splineData.VertexColors.length > 0) {
      const colors = new Float32Array(splineData.VertexColors.map(c => c / 255));
      geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3));
    }

    if (splineData.Indices && splineData.Indices.length > 0) {
      geometry.setIndex(new THREE.BufferAttribute(new Uint16Array(splineData.Indices), 1));
    }

    if (splineData.UV && splineData.UV.length > 0) {
      geometry.setAttribute('uv', new THREE.BufferAttribute(new Float32Array(splineData.UV), 2));
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static createSplineMaterial(splineData: SplineData): THREE.LineBasicMaterial {
    const material = new THREE.LineBasicMaterial({
      color: splineData.Color.Hex,
      linewidth: splineData.Material.LineWidth,
      opacity: splineData.Opacity,
      transparent: splineData.Transparent,
      depthTest: splineData.DepthTest !== false,
      vertexColors: splineData.Material.VertexColors
    });

    return material;
  }

  public static renderMultiple(splineDataArray: SplineData[], scene: THREE.Scene): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];

    splineDataArray.forEach((splineData) => {
      const splineLine = this.render(splineData, scene);
      if (splineLine) {
        objects.push(splineLine);
      }
    });

    return objects;
  }

  public static disposeMultiple(objects: THREE.Object3D[], scene: THREE.Scene): void {
    if (!objects || objects.length === 0) {
      return;
    }

    objects.forEach((obj) => {
      if (obj instanceof THREE.Line) {
        scene.remove(obj);
        obj.geometry.dispose();
        (obj.material as THREE.Material).dispose();
      }
    });
  }

  public static getNormal(splineData: SplineData): NormalData {
    if (!splineData.Normal || (splineData.Normal.X === 0 && splineData.Normal.Y === 0 && splineData.Normal.Z === 0)) {
      return { X: 0, Y: 0, Z: 1 };
    }
    return splineData.Normal;
  }

  public static getTangent(splineData: SplineData): NormalData {
    if (!splineData.Tangent || (splineData.Tangent.X === 0 && splineData.Tangent.Y === 0 && splineData.Tangent.Z === 0)) {
      return { X: 1, Y: 0, Z: 0 };
    }
    return splineData.Tangent;
  }

  public static getBinormal(splineData: SplineData): number[] {
    if (!splineData.Binormal || splineData.Binormal.length === 0) {
      return [0, 0, 1];
    }
    return splineData.Binormal;
  }

  public static getCurvature(splineData: SplineData): number {
    return splineData.Curvature || 0;
  }

  public static getTransform(splineData: SplineData): TransformData {
    if (!splineData.Transform) {
      return {
        Position: splineData.Centroid || { X: 0, Y: 0, Z: 0 },
        Rotation: { X: 0, Y: 0, Z: 0 },
        Scale: { X: 1, Y: 1, Z: 1 },
        Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
      };
    }
    return splineData.Transform;
  }

  public static getGeometry(splineData: SplineData): GeometryData {
    if (!splineData.Geometry) {
      return {
        Type: 'BufferGeometry',
        VertexCount: splineData.ApproximationPointCount || 0,
        HasColors: true,
        HasIndices: true,
        PrimitiveType: 'LineLoop',
        IndexCount: (splineData.ApproximationPointCount || 0) * 2
      };
    }
    return splineData.Geometry;
  }

  public static getMaterial(splineData: SplineData): MaterialData {
    if (!splineData.Material) {
      return {
        Type: 'LineBasicMaterial',
        Color: parseInt(splineData.Color.Hex.replace('#', ''), 16),
        Opacity: splineData.Opacity || this.DEFAULT_OPACITY,
        Transparent: splineData.Transparent || false,
        Wireframe: false,
        LineWidth: splineData.LineWeight || this.DEFAULT_LINE_WIDTH,
        VertexColors: true,
        Side: true
      };
    }
    return splineData.Material;
  }

  public static getDepthTest(splineData: SplineData): boolean {
    return splineData.DepthTest !== false;
  }

  public static getHandle(splineData: SplineData): string {
    return splineData.Handle || '';
  }

  public static getLayerName(splineData: SplineData): string {
    return splineData.LayerName || '';
  }

  public static getVisible(splineData: SplineData): boolean {
    return splineData.Visible !== false;
  }

  public static getOpacity(splineData: SplineData): number {
    return splineData.Opacity !== undefined ? splineData.Opacity : this.DEFAULT_OPACITY;
  }

  public static getTransparent(splineData: SplineData): boolean {
    return splineData.Transparent !== false;
  }

  public static getSplineType(splineData: SplineData): string {
    return splineData.SplineType || 'CatmullRomCurve3';
  }

  public static getKnots(splineData: SplineData): number[] {
    return splineData.Knots || [];
  }

  public static getWeights(splineData: SplineData): number[] {
    return splineData.Weights || [];
  }

  public static getControlPointWeights(splineData: SplineData): number[] {
    return splineData.ControlPointWeights || [];
  }

  public static getStartTangent(splineData: SplineData): THREE.Vector3 {
    return new THREE.Vector3(splineData.StartTangent.X, splineData.StartTangent.Y, splineData.StartTangent.Z);
  }

  public static getEndTangent(splineData: SplineData): THREE.Vector3 {
    return new THREE.Vector3(splineData.EndTangent.X, splineData.EndTangent.Y, splineData.EndTangent.Z);
  }

  public static getSampleCount(splineData: SplineData): number {
    return splineData.SampleCount || 0;
  }
}
