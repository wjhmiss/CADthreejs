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

export interface TransformData {
  Position: Point3DData;
  Rotation: Point3DData;
  Scale: Point3DData;
  Matrix: number[];
}

export interface EllipseData {
  Type: string;
  Uuid: string;
  EntityType: string;
  CenterX: number;
  CenterY: number;
  CenterZ: number;
  MajorAxisEndPointX: number;
  MajorAxisEndPointY: number;
  MajorAxisEndPointZ: number;
  RadiusRatio: number;
  StartParameter: number;
  EndParameter: number;
  Thickness: number;
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
  MajorRadius: number;
  MinorRadius: number;
  RotationAngle: number;
  Length: number;
  Area: number;
  IsFullEllipse: boolean;
  IsCounterClockwise: boolean;
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

export interface EllipseRenderResult {
  mesh: THREE.Line | THREE.LineLoop;
  material: THREE.Material;
  geometry: THREE.BufferGeometry;
}

export class EllipseEntityThreejsRenderer {
  private static readonly DEFAULT_COLOR = new THREE.Color(0x000000);
  private static readonly DEFAULT_SEGMENTS = 64;

  public static render(ellipseData: EllipseData, scene: THREE.Scene): THREE.Object3D[] | null {
    if (!ellipseData.Visible || ellipseData.IsInvisible) {
      return null;
    }

    const objects: THREE.Object3D[] = [];

    const material = this.createMaterial(ellipseData);
    const geometry = this.createGeometry(ellipseData);

    if (!geometry) {
      return null;
    }

    const line = ellipseData.IsFullEllipse ? new THREE.LineLoop(geometry, material) : new THREE.Line(geometry, material);

    if (ellipseData.Transform && ellipseData.Transform.Matrix && ellipseData.Transform.Matrix.length === 16) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(ellipseData.Transform.Matrix);
      line.applyMatrix4(matrix);
    } else {
      line.position.set(ellipseData.CenterX, ellipseData.CenterY, ellipseData.CenterZ);
    }

    line.userData = {
      type: ellipseData.Type,
      uuid: ellipseData.Uuid,
      entityType: ellipseData.EntityType,
      handle: ellipseData.Handle,
      layerName: ellipseData.LayerName,
      objectType: 'Ellipse'
    };

    line.visible = ellipseData.Visible;
    line.castShadow = ellipseData.CastShadow;
    line.receiveShadow = ellipseData.ReceiveShadow;
    line.renderOrder = ellipseData.RenderOrder;

    objects.push(line);

    return objects;
  }

  public static renderFromJson(jsonString: string, scene: THREE.Scene): THREE.Object3D[] | null {
    try {
      const ellipseData: EllipseData = JSON.parse(jsonString);
      return this.render(ellipseData, scene);
    } catch (error) {
      console.error('Failed to parse Ellipse JSON:', error);
      return null;
    }
  }

  public static renderWithResult(ellipseData: EllipseData, scene: THREE.Scene): EllipseRenderResult | null {
    if (!ellipseData.Visible || ellipseData.IsInvisible) {
      return null;
    }

    const material = this.createMaterial(ellipseData);
    const geometry = this.createGeometry(ellipseData);

    if (!geometry) {
      return null;
    }

    const line = ellipseData.IsFullEllipse ? new THREE.LineLoop(geometry, material) : new THREE.Line(geometry, material);

    if (ellipseData.Transform && ellipseData.Transform.Matrix && ellipseData.Transform.Matrix.length === 16) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(ellipseData.Transform.Matrix);
      line.applyMatrix4(matrix);
    } else {
      line.position.set(ellipseData.CenterX, ellipseData.CenterY, ellipseData.CenterZ);
    }

    line.userData = {
      type: ellipseData.Type,
      uuid: ellipseData.Uuid,
      entityType: ellipseData.EntityType,
      handle: ellipseData.Handle,
      layerName: ellipseData.LayerName,
      objectType: 'Ellipse'
    };

    line.visible = ellipseData.Visible;
    line.castShadow = ellipseData.CastShadow;
    line.receiveShadow = ellipseData.ReceiveShadow;
    line.renderOrder = ellipseData.RenderOrder;

    return {
      mesh: line,
      material,
      geometry
    };
  }

  public static dispose(result: EllipseRenderResult): void {
    if (result.geometry) {
      result.geometry.dispose();
    }
    if (result.material) {
      result.material.dispose();
    }
  }

  private static createMaterial(ellipseData: EllipseData): THREE.Material {
    const color = this.getColor(ellipseData);

    const materialOptions: THREE.LineBasicMaterialParameters = {
      color: color,
      transparent: ellipseData.MaterialTransparent || ellipseData.Transparency > 0,
      opacity: ellipseData.MaterialOpacity || (1 - ellipseData.Transparency),
      depthTest: ellipseData.MaterialDepthTest !== false,
      depthWrite: ellipseData.MaterialDepthWrite !== false,
      linewidth: ellipseData.LineWeight || 1
    };

    return new THREE.LineBasicMaterial(materialOptions);
  }

  private static getColor(ellipseData: EllipseData): THREE.Color {
    if (ellipseData.ColorHex) {
      return new THREE.Color(ellipseData.ColorHex);
    }
    if (ellipseData.ColorR !== undefined && ellipseData.ColorG !== undefined && ellipseData.ColorB !== undefined) {
      return new THREE.Color(`rgb(${ellipseData.ColorR}, ${ellipseData.ColorG}, ${ellipseData.ColorB})`);
    }
    return EllipseEntityThreejsRenderer.DEFAULT_COLOR;
  }

  private static createGeometry(ellipseData: EllipseData): THREE.BufferGeometry | null {
    if (!ellipseData.Vertices || ellipseData.Vertices.length === 0) {
      return this.generateEllipseGeometry(ellipseData);
    }

    const geometry = new THREE.BufferGeometry();
    const vertices = new Float32Array(ellipseData.Vertices);
    geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));

    if (ellipseData.Indices && ellipseData.Indices.length > 0) {
      geometry.setIndex(ellipseData.Indices);
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static generateEllipseGeometry(ellipseData: EllipseData): THREE.BufferGeometry | null {
    const segments = EllipseEntityThreejsRenderer.DEFAULT_SEGMENTS;
    const vertices: number[] = [];

    const centerX = ellipseData.CenterX;
    const centerY = ellipseData.CenterY;
    const centerZ = ellipseData.CenterZ;
    const majorRadius = ellipseData.MajorRadius;
    const minorRadius = ellipseData.MinorRadius;
    const rotationAngle = ellipseData.RotationAngle;

    if (ellipseData.IsFullEllipse) {
      for (let i = 0; i <= segments; i++) {
        const angle = (2 * Math.PI * i) / segments;
        const x = centerX + majorRadius * Math.cos(angle) * Math.cos(rotationAngle) - minorRadius * Math.sin(angle) * Math.sin(rotationAngle);
        const y = centerY + majorRadius * Math.cos(angle) * Math.sin(rotationAngle) + minorRadius * Math.sin(angle) * Math.cos(rotationAngle);
        const z = centerZ;
        vertices.push(x, y, z);
      }
    } else {
      const startParameter = ellipseData.StartParameter;
      const endParameter = ellipseData.EndParameter;
      let endParam = endParameter;

      if (endParam < startParameter) {
        endParam += 2 * Math.PI;
      }

      for (let i = 0; i <= segments; i++) {
        const angle = startParameter + ((endParam - startParameter) * i) / segments;
        const x = centerX + majorRadius * Math.cos(angle) * Math.cos(rotationAngle) - minorRadius * Math.sin(angle) * Math.sin(rotationAngle);
        const y = centerY + majorRadius * Math.cos(angle) * Math.sin(rotationAngle) + minorRadius * Math.sin(angle) * Math.cos(rotationAngle);
        const z = centerZ;
        vertices.push(x, y, z);
      }
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));
    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  public static createEllipseFromData(ellipseData: EllipseData): THREE.Object3D[] | null {
    const scene = new THREE.Scene();
    return this.render(ellipseData, scene);
  }

  public static createEllipseObjects(ellipseData: EllipseData): THREE.Object3D[] | null {
    const scene = new THREE.Scene();
    return this.render(ellipseData, scene);
  }

  public static getEllipseProperties(ellipseData: EllipseData): {
    majorRadius: number;
    minorRadius: number;
    area: number;
    length: number;
    isFullEllipse: boolean;
  } {
    return {
      majorRadius: ellipseData.MajorRadius,
      minorRadius: ellipseData.MinorRadius,
      area: ellipseData.Area,
      length: ellipseData.Length,
      isFullEllipse: ellipseData.IsFullEllipse
    };
  }

  public static validateEllipseData(ellipseData: EllipseData): boolean {
    if (!ellipseData) {
      return false;
    }

    if (ellipseData.MajorRadius <= 0 || ellipseData.MinorRadius <= 0) {
      return false;
    }

    if (ellipseData.RadiusRatio <= 0 || ellipseData.RadiusRatio > 1) {
      return false;
    }

    if (!ellipseData.IsFullEllipse) {
      if (ellipseData.StartParameter === ellipseData.EndParameter) {
        return false;
      }
    }

    return true;
  }

  public static getBoundingBox(ellipseData: EllipseData): THREE.Box3 | null {
    if (!ellipseData.Bounds) {
      return null;
    }

    const min = new THREE.Vector3(ellipseData.Bounds.Min.X, ellipseData.Bounds.Min.Y, ellipseData.Bounds.Min.Z);
    const max = new THREE.Vector3(ellipseData.Bounds.Max.X, ellipseData.Bounds.Max.Y, ellipseData.Bounds.Max.Z);

    return new THREE.Box3(min, max);
  }

  public static getCentroid(ellipseData: EllipseData): THREE.Vector3 | null {
    if (!ellipseData.Centroid) {
      return null;
    }

    return new THREE.Vector3(ellipseData.Centroid.X, ellipseData.Centroid.Y, ellipseData.Centroid.Z);
  }
}
