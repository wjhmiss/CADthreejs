import * as THREE from 'three';

export interface Point3DData {
  X: number;
  Y: number;
  Z: number;
}

export interface BoundsData {
  Min: Point3DData;
  Max: Point3DData;
}

export interface TransformData {
  Position: Point3DData;
  Rotation: Point3DData;
  Scale: Point3DData;
  Matrix: number[];
}

export interface ArcData {
  Type: string;
  Uuid: string;
  EntityType: string;
  CenterX: number;
  CenterY: number;
  CenterZ: number;
  Radius: number;
  StartAngle: number;
  EndAngle: number;
  Thickness: number;
  Sweep: number;
  StartX: number;
  StartY: number;
  StartZ: number;
  EndX: number;
  EndY: number;
  EndZ: number;
  NormalX: number;
  NormalY: number;
  NormalZ: number;
  Handle: string;
  LayerName: string;
  IsInvisible: boolean;
  LineTypeScale: number;
  Transparency: number;
  ColorIndex: number;
  ColorHex: string;
  ColorR: number;
  ColorG: number;
  ColorB: number;
  ColorA: number;
  LineTypeName: string;
  LineWeight: number;
  MaterialType: string;
  MaterialTransparent: boolean;
  MaterialOpacity: number;
  MaterialDepthTest: boolean;
  MaterialDepthWrite: boolean;
  MaterialSide: number;
  Points: Point3DData[];
  Vertices: number[];
  Indices: number[];
  IsCCW: boolean;
  Length: number;
  Area: number;
  ChordLength: number;
  Sagitta: number;
  MidAngle: number;
  MidX: number;
  MidY: number;
  MidZ: number;
  Bounds: BoundsData;
  Centroid: Point3DData;
  Transform: TransformData;
  CoordinateSystem: string;
  RequiresYAxisFlip: boolean;
  Parent: any;
  Visible: boolean;
  CastShadow: boolean;
  ReceiveShadow: boolean;
  RenderOrder: number;
}

export class ArcEntityThreejsRenderer {
  private static readonly DEFAULT_SEGMENTS = 64;

  public static render(arcData: ArcData, scene: THREE.Scene): THREE.Group | null {
    if (!arcData.Visible || arcData.IsInvisible) {
      return null;
    }

    const group = new THREE.Group();
    group.name = `Arc_${arcData.Handle || arcData.Uuid}`;

    const material = this.createMaterial(arcData);
    const geometry = this.createGeometry(arcData);

    if (!geometry) {
      return null;
    }

    const line = new THREE.Line(geometry, material);
    line.name = `Arc_${arcData.Handle || arcData.Uuid}`;
    line.uuid = arcData.Uuid;
    line.userData = {
      type: 'Arc',
      handle: arcData.Handle,
      layerName: arcData.LayerName,
      entityType: arcData.EntityType,
      arcData: arcData
    };

    this.applyTransform(line, arcData);
    this.applyRenderProperties(line, arcData);

    group.add(line);

    return group;
  }

  public static renderFromJson(jsonString: string, scene: THREE.Scene): THREE.Group | null {
    try {
      const arcData: ArcData = JSON.parse(jsonString);
      return this.render(arcData, scene);
    } catch (error) {
      console.error('Failed to parse Arc JSON:', error);
      return null;
    }
  }

  public static renderMultiple(arcDataList: ArcData[], scene: THREE.Scene): THREE.Group[] {
    const objects: THREE.Group[] = [];

    for (const arcData of arcDataList) {
      const obj = this.render(arcData, scene);
      if (obj) {
        objects.push(obj);
      }
    }

    return objects;
  }

  public static renderMultipleFromJson(jsonString: string, scene: THREE.Scene): THREE.Group[] {
    try {
      const arcDataList: ArcData[] = JSON.parse(jsonString);
      return this.renderMultiple(arcDataList, scene);
    } catch (error) {
      console.error('Failed to parse Arc JSON array:', error);
      return [];
    }
  }

  private static createMaterial(arcData: ArcData): THREE.Material {
    const color = new THREE.Color(
      arcData.ColorR / 255,
      arcData.ColorG / 255,
      arcData.ColorB / 255
    );

    const materialConfig: THREE.LineBasicMaterialParameters = {
      color: color,
      transparent: arcData.MaterialTransparent,
      opacity: arcData.MaterialOpacity,
      depthTest: arcData.MaterialDepthTest,
      depthWrite: arcData.MaterialDepthWrite,
      linewidth: arcData.LineWeight
    };

    return new THREE.LineBasicMaterial(materialConfig);
  }

  private static createGeometry(arcData: ArcData): THREE.BufferGeometry | null {
    if (!arcData.Vertices || arcData.Vertices.length === 0) {
      return this.generateGeometryFromArcData(arcData);
    }

    const geometry = new THREE.BufferGeometry();
    const vertices = new Float32Array(arcData.Vertices);
    geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));

    if (arcData.Indices && arcData.Indices.length > 0) {
      geometry.setIndex(arcData.Indices);
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static generateGeometryFromArcData(arcData: ArcData): THREE.BufferGeometry | null {
    const segments = this.DEFAULT_SEGMENTS;
    const vertices: number[] = [];

    for (let i = 0; i <= segments; i++) {
      const angle = arcData.StartAngle + (arcData.EndAngle - arcData.StartAngle) * i / segments;
      const x = arcData.CenterX + arcData.Radius * Math.cos(angle);
      const y = arcData.CenterY + arcData.Radius * Math.sin(angle);
      const z = arcData.CenterZ;
      vertices.push(x, y, z);
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));
    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static applyTransform(object: THREE.Object3D, arcData: ArcData): void {
    if (arcData.Transform && arcData.Transform.Matrix) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(arcData.Transform.Matrix);
      object.applyMatrix4(matrix);
    } else {
      object.position.set(arcData.CenterX, arcData.CenterY, arcData.CenterZ);
    }
  }

  private static applyRenderProperties(object: THREE.Object3D, arcData: ArcData): void {
    object.visible = arcData.Visible;
    object.castShadow = arcData.CastShadow;
    object.receiveShadow = arcData.ReceiveShadow;
    object.renderOrder = arcData.RenderOrder;
  }

  public static updateArc(object: THREE.Object3D, arcData: ArcData): void {
    const material = object.material as THREE.LineBasicMaterial;
    const color = new THREE.Color(
      arcData.ColorR / 255,
      arcData.ColorG / 255,
      arcData.ColorB / 255
    );

    material.color = color;
    material.transparent = arcData.MaterialTransparent;
    material.opacity = arcData.MaterialOpacity;

    const geometry = this.createGeometry(arcData);
    if (geometry) {
      object.geometry.dispose();
      object.geometry = geometry;
    }

    this.applyTransform(object, arcData);
    this.applyRenderProperties(object, arcData);
  }

  public static getArcDataFromObject(object: THREE.Object3D): ArcData | null {
    if (object.userData && object.userData.arcData) {
      return object.userData.arcData as ArcData;
    }
    return null;
  }

  public static calculateArcLength(arcData: ArcData): number {
    return arcData.Radius * Math.abs(arcData.EndAngle - arcData.StartAngle);
  }

  public static calculateChordLength(arcData: ArcData): number {
    const dx = arcData.EndX - arcData.StartX;
    const dy = arcData.EndY - arcData.StartY;
    return Math.sqrt(dx * dx + dy * dy);
  }

  public static calculateSagitta(arcData: ArcData): number {
    const chordLength = this.calculateChordLength(arcData);
    return arcData.Radius - Math.sqrt(Math.pow(arcData.Radius, 2) - Math.pow(chordLength / 2, 2));
  }

  public static isPointOnArc(arcData: ArcData, point: THREE.Vector3, tolerance: number = 0.01): boolean {
    const dx = point.x - arcData.CenterX;
    const dy = point.y - arcData.CenterY;
    const distance = Math.sqrt(dx * dx + dy * dy);

    if (Math.abs(distance - arcData.Radius) > tolerance) {
      return false;
    }

    const angle = Math.atan2(dy, dx);
    const normalizedAngle = this.normalizeAngle(angle);
    const normalizedStart = this.normalizeAngle(arcData.StartAngle);
    const normalizedEnd = this.normalizeAngle(arcData.EndAngle);

    if (normalizedStart <= normalizedEnd) {
      return normalizedAngle >= normalizedStart && normalizedAngle <= normalizedEnd;
    } else {
      return normalizedAngle >= normalizedStart || normalizedAngle <= normalizedEnd;
    }
  }

  private static normalizeAngle(angle: number): number {
    while (angle < 0) angle += 2 * Math.PI;
    while (angle >= 2 * Math.PI) angle -= 2 * Math.PI;
    return angle;
  }

  public static dispose(object: THREE.Object3D): void {
    if (object.geometry) {
      object.geometry.dispose();
    }
    if (object.material) {
      if (Array.isArray(object.material)) {
        object.material.forEach(m => m.dispose());
      } else {
        object.material.dispose();
      }
    }
  }
}
