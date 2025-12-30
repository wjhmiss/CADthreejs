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

export interface CircleData {
  Type: string;
  Uuid: string;
  EntityType: string;
  CenterX: number;
  CenterY: number;
  CenterZ: number;
  Radius: number;
  Thickness: number;
  Diameter: number;
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
  Circumference: number;
  Area: number;
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

export class CircleEntityThreejsRenderer {
  private static readonly DEFAULT_SEGMENTS = 64;

  public static render(circleData: CircleData, scene: THREE.Scene): THREE.Line | null {
    if (!circleData.Visible || circleData.IsInvisible) {
      return null;
    }

    const material = this.createMaterial(circleData);
    const geometry = this.createGeometry(circleData);

    if (!geometry) {
      return null;
    }

    const line = new THREE.Line(geometry, material);
    line.name = `Circle_${circleData.Handle || circleData.Uuid}`;
    line.uuid = circleData.Uuid;
    line.userData = {
      type: 'Circle',
      handle: circleData.Handle,
      layerName: circleData.LayerName,
      entityType: circleData.EntityType,
      circleData: circleData
    };

    this.applyTransform(line, circleData);
    this.applyRenderProperties(line, circleData);

    return line;
  }

  public static renderFromJson(jsonString: string, scene: THREE.Scene): THREE.Line | null {
    try {
      const circleData: CircleData = JSON.parse(jsonString);
      return this.render(circleData, scene);
    } catch (error) {
      console.error('Failed to parse Circle JSON:', error);
      return null;
    }
  }

  public static renderMultiple(circleDataList: CircleData[], scene: THREE.Scene): THREE.Line[] {
    const objects: THREE.Line[] = [];

    for (const circleData of circleDataList) {
      const obj = this.render(circleData, scene);
      if (obj) {
        objects.push(obj);
      }
    }

    return objects;
  }

  public static renderMultipleFromJson(jsonString: string, scene: THREE.Scene): THREE.Line[] {
    try {
      const circleDataList: CircleData[] = JSON.parse(jsonString);
      return this.renderMultiple(circleDataList, scene);
    } catch (error) {
      console.error('Failed to parse Circle JSON array:', error);
      return [];
    }
  }

  private static createMaterial(circleData: CircleData): THREE.Material {
    const color = new THREE.Color(
      circleData.ColorR / 255,
      circleData.ColorG / 255,
      circleData.ColorB / 255
    );

    const materialConfig: THREE.LineBasicMaterialParameters = {
      color: color,
      transparent: circleData.MaterialTransparent,
      opacity: circleData.MaterialOpacity,
      depthTest: circleData.MaterialDepthTest,
      depthWrite: circleData.MaterialDepthWrite,
      linewidth: circleData.LineWeight
    };

    return new THREE.LineBasicMaterial(materialConfig);
  }

  private static createGeometry(circleData: CircleData): THREE.BufferGeometry | null {
    if (!circleData.Vertices || circleData.Vertices.length === 0) {
      return this.generateGeometryFromCircleData(circleData);
    }

    const geometry = new THREE.BufferGeometry();
    const vertices = new Float32Array(circleData.Vertices);
    geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));

    if (circleData.Indices && circleData.Indices.length > 0) {
      geometry.setIndex(circleData.Indices);
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static generateGeometryFromCircleData(circleData: CircleData): THREE.BufferGeometry | null {
    const segments = this.DEFAULT_SEGMENTS;
    const vertices: number[] = [];

    for (let i = 0; i <= segments; i++) {
      const angle = 2 * Math.PI * i / segments;
      const x = circleData.CenterX + circleData.Radius * Math.cos(angle);
      const y = circleData.CenterY + circleData.Radius * Math.sin(angle);
      const z = circleData.CenterZ;
      vertices.push(x, y, z);
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));
    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static applyTransform(object: THREE.Object3D, circleData: CircleData): void {
    if (circleData.Transform && circleData.Transform.Matrix) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(circleData.Transform.Matrix);
      object.applyMatrix4(matrix);
    } else {
      object.position.set(circleData.CenterX, circleData.CenterY, circleData.CenterZ);
    }
  }

  private static applyRenderProperties(object: THREE.Object3D, circleData: CircleData): void {
    object.visible = circleData.Visible;
    object.castShadow = circleData.CastShadow;
    object.receiveShadow = circleData.ReceiveShadow;
    object.renderOrder = circleData.RenderOrder;
  }

  public static updateCircle(object: THREE.Object3D, circleData: CircleData): void {
    const material = object.material as THREE.LineBasicMaterial;
    const color = new THREE.Color(
      circleData.ColorR / 255,
      circleData.ColorG / 255,
      circleData.ColorB / 255
    );

    material.color = color;
    material.transparent = circleData.MaterialTransparent;
    material.opacity = circleData.MaterialOpacity;

    const geometry = this.createGeometry(circleData);
    if (geometry) {
      object.geometry.dispose();
      object.geometry = geometry;
    }

    this.applyTransform(object, circleData);
    this.applyRenderProperties(object, circleData);
  }

  public static getCircleDataFromObject(object: THREE.Object3D): CircleData | null {
    if (object.userData && object.userData.circleData) {
      return object.userData.circleData as CircleData;
    }
    return null;
  }

  public static calculateCircumference(circleData: CircleData): number {
    return 2 * Math.PI * circleData.Radius;
  }

  public static calculateArea(circleData: CircleData): number {
    return Math.PI * circleData.Radius * circleData.Radius;
  }

  public static calculateDiameter(circleData: CircleData): number {
    return 2 * circleData.Radius;
  }

  public static isPointOnCircle(circleData: CircleData, point: THREE.Vector3, tolerance: number = 0.01): boolean {
    const dx = point.x - circleData.CenterX;
    const dy = point.y - circleData.CenterY;
    const distance = Math.sqrt(dx * dx + dy * dy);

    return Math.abs(distance - circleData.Radius) <= tolerance;
  }

  public static isPointInsideCircle(circleData: CircleData, point: THREE.Vector3): boolean {
    const dx = point.x - circleData.CenterX;
    const dy = point.y - circleData.CenterY;
    const distance = Math.sqrt(dx * dx + dy * dy);

    return distance <= circleData.Radius;
  }

  public static getPointOnCircle(circleData: CircleData, angle: number): THREE.Vector3 {
    const x = circleData.CenterX + circleData.Radius * Math.cos(angle);
    const y = circleData.CenterY + circleData.Radius * Math.sin(angle);
    const z = circleData.CenterZ;
    return new THREE.Vector3(x, y, z);
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
