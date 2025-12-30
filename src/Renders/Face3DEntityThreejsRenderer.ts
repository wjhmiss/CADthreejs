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

export interface Face3DData {
  Type: string;
  Uuid: string;
  EntityType: string;
  FirstCorner: Point3DData;
  SecondCorner: Point3DData;
  ThirdCorner: Point3DData;
  FourthCorner: Point3DData;
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
  HasFourthCorner: boolean;
  Area: number;
  Center: Point3DData;
  Normal: Point3DData;
  GeometryType: string;
  Points: Point3DData[];
  Vertices: number[];
  Indices: number[];
  Normals: number[];
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

export interface Face3DRenderResult {
  mesh: THREE.Mesh;
  material: THREE.Material;
  geometry: THREE.BufferGeometry;
}

export class Face3DEntityThreejsRenderer {
  private static readonly DEFAULT_COLOR = new THREE.Color(0x000000);

  public static render(face3DData: Face3DData, scene: THREE.Scene): THREE.Mesh | null {
    if (!face3DData.Visible || face3DData.IsInvisible) {
      return null;
    }

    const material = this.createMaterial(face3DData);
    const geometry = this.createGeometry(face3DData);

    if (!geometry) {
      return null;
    }

    const mesh = new THREE.Mesh(geometry, material);

    if (face3DData.Transform && face3DData.Transform.Matrix && face3DData.Transform.Matrix.length === 16) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(face3DData.Transform.Matrix);
      mesh.applyMatrix4(matrix);
    }

    mesh.userData = {
      type: face3DData.Type,
      uuid: face3DData.Uuid,
      entityType: face3DData.EntityType,
      handle: face3DData.Handle,
      layerName: face3DData.LayerName,
      objectType: 'Face3D'
    };

    mesh.visible = face3DData.Visible;
    mesh.castShadow = face3DData.CastShadow;
    mesh.receiveShadow = face3DData.ReceiveShadow;
    mesh.renderOrder = face3DData.RenderOrder;

    return mesh;
  }

  public static renderFromJson(jsonString: string, scene: THREE.Scene): THREE.Mesh | null {
    try {
      const face3DData: Face3DData = JSON.parse(jsonString);
      return this.render(face3DData, scene);
    } catch (error) {
      console.error('Failed to parse Face3D JSON:', error);
      return null;
    }
  }

  public static renderWithResult(face3DData: Face3DData, scene: THREE.Scene): Face3DRenderResult | null {
    if (!face3DData.Visible || face3DData.IsInvisible) {
      return null;
    }

    const material = this.createMaterial(face3DData);
    const geometry = this.createGeometry(face3DData);

    if (!geometry) {
      return null;
    }

    const mesh = new THREE.Mesh(geometry, material);

    if (face3DData.Transform && face3DData.Transform.Matrix && face3DData.Transform.Matrix.length === 16) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(face3DData.Transform.Matrix);
      mesh.applyMatrix4(matrix);
    }

    mesh.userData = {
      type: face3DData.Type,
      uuid: face3DData.Uuid,
      entityType: face3DData.EntityType,
      handle: face3DData.Handle,
      layerName: face3DData.LayerName,
      objectType: 'Face3D'
    };

    mesh.visible = face3DData.Visible;
    mesh.castShadow = face3DData.CastShadow;
    mesh.receiveShadow = face3DData.ReceiveShadow;
    mesh.renderOrder = face3DData.RenderOrder;

    return {
      mesh,
      material,
      geometry
    };
  }

  public static dispose(result: Face3DRenderResult): void {
    if (result.geometry) {
      result.geometry.dispose();
    }
    if (result.material) {
      result.material.dispose();
    }
  }

  private static createMaterial(face3DData: Face3DData): THREE.Material {
    const color = this.getColor(face3DData);
    const side = this.getMaterialSide(face3DData.MaterialSide);

    const materialOptions: THREE.MeshPhongMaterialParameters = {
      color: color,
      transparent: face3DData.MaterialTransparent || face3DData.Transparency > 0,
      opacity: face3DData.MaterialOpacity || (1 - face3DData.Transparency),
      depthTest: face3DData.MaterialDepthTest !== false,
      depthWrite: face3DData.MaterialDepthWrite !== false,
      side: side
    };

    return new THREE.MeshPhongMaterial(materialOptions);
  }

  private static getColor(face3DData: Face3DData): THREE.Color {
    if (face3DData.ColorHex) {
      return new THREE.Color(face3DData.ColorHex);
    }
    if (face3DData.ColorR !== undefined && face3DData.ColorG !== undefined && face3DData.ColorB !== undefined) {
      return new THREE.Color(`rgb(${face3DData.ColorR}, ${face3DData.ColorG}, ${face3DData.ColorB})`);
    }
    return Face3DEntityThreejsRenderer.DEFAULT_COLOR;
  }

  private static getMaterialSide(sideValue: number): THREE.Side {
    switch (sideValue) {
      case 0:
        return THREE.BackSide;
      case 1:
        return THREE.FrontSide;
      case 2:
      default:
        return THREE.DoubleSide;
    }
  }

  private static createGeometry(face3DData: Face3DData): THREE.BufferGeometry | null {
    if (!face3DData.Vertices || face3DData.Vertices.length === 0) {
      return this.generateFaceGeometry(face3DData);
    }

    const geometry = new THREE.BufferGeometry();
    const vertices = new Float32Array(face3DData.Vertices);
    geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));

    if (face3DData.Indices && face3DData.Indices.length > 0) {
      geometry.setIndex(face3DData.Indices);
    }

    if (face3DData.Normals && face3DData.Normals.length > 0) {
      const normals = new Float32Array(face3DData.Normals);
      geometry.setAttribute('normal', new THREE.BufferAttribute(normals, 3));
    } else {
      geometry.computeVertexNormals();
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static generateFaceGeometry(face3DData: Face3DData): THREE.BufferGeometry | null {
    if (!face3DData.FirstCorner || !face3DData.SecondCorner || !face3DData.ThirdCorner) {
      return null;
    }

    const vertices: number[] = [];
    const indices: number[] = [];

    const p1 = face3DData.FirstCorner;
    const p2 = face3DData.SecondCorner;
    const p3 = face3DData.ThirdCorner;
    const p4 = face3DData.FourthCorner;

    vertices.push(p1.X, p1.Y, p1.Z);
    vertices.push(p2.X, p2.Y, p2.Z);
    vertices.push(p3.X, p3.Y, p3.Z);

    if (face3DData.HasFourthCorner) {
      vertices.push(p4.X, p4.Y, p4.Z);
      indices.push(0, 1, 2);
      indices.push(0, 2, 3);
    } else {
      indices.push(0, 1, 2);
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));
    geometry.setIndex(indices);
    geometry.computeVertexNormals();
    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  public static createFaceFromData(face3DData: Face3DData): THREE.Mesh | null {
    const scene = new THREE.Scene();
    return this.render(face3DData, scene);
  }

  public static createFaceMesh(face3DData: Face3DData): THREE.Mesh | null {
    const scene = new THREE.Scene();
    return this.render(face3DData, scene);
  }

  public static getFaceProperties(face3DData: Face3DData): {
    area: number;
    hasFourthCorner: boolean;
    geometryType: string;
    center: THREE.Vector3;
    normal: THREE.Vector3;
  } {
    return {
      area: face3DData.Area,
      hasFourthCorner: face3DData.HasFourthCorner,
      geometryType: face3DData.GeometryType,
      center: new THREE.Vector3(face3DData.Center.X, face3DData.Center.Y, face3DData.Center.Z),
      normal: new THREE.Vector3(face3DData.Normal.X, face3DData.Normal.Y, face3DData.Normal.Z)
    };
  }

  public static validateFace3DData(face3DData: Face3DData): boolean {
    if (!face3DData) {
      return false;
    }

    if (!face3DData.FirstCorner || !face3DData.SecondCorner || !face3DData.ThirdCorner) {
      return false;
    }

    if (face3DData.HasFourthCorner && !face3DData.FourthCorner) {
      return false;
    }

    if (face3DData.Area < 0) {
      return false;
    }

    return true;
  }

  public static getBoundingBox(face3DData: Face3DData): THREE.Box3 | null {
    if (!face3DData.Bounds) {
      return null;
    }

    const min = new THREE.Vector3(face3DData.Bounds.Min.X, face3DData.Bounds.Min.Y, face3DData.Bounds.Min.Z);
    const max = new THREE.Vector3(face3DData.Bounds.Max.X, face3DData.Bounds.Max.Y, face3DData.Bounds.Max.Z);

    return new THREE.Box3(min, max);
  }

  public static getCentroid(face3DData: Face3DData): THREE.Vector3 | null {
    if (!face3DData.Centroid) {
      return null;
    }

    return new THREE.Vector3(face3DData.Centroid.X, face3DData.Centroid.Y, face3DData.Centroid.Z);
  }

  public static getNormal(face3DData: Face3DData): THREE.Vector3 | null {
    if (!face3DData.Normal) {
      return null;
    }

    return new THREE.Vector3(face3DData.Normal.X, face3DData.Normal.Y, face3DData.Normal.Z);
  }

  public static getCenter(face3DData: Face3DData): THREE.Vector3 | null {
    if (!face3DData.Center) {
      return null;
    }

    return new THREE.Vector3(face3DData.Center.X, face3DData.Center.Y, face3DData.Center.Z);
  }

  public static calculateFaceNormal(p1: THREE.Vector3, p2: THREE.Vector3, p3: THREE.Vector3): THREE.Vector3 {
    const v1 = new THREE.Vector3().subVectors(p2, p1);
    const v2 = new THREE.Vector3().subVectors(p3, p1);
    const normal = new THREE.Vector3().crossVectors(v1, v2).normalize();
    return normal;
  }

  public static calculateTriangleArea(p1: THREE.Vector3, p2: THREE.Vector3, p3: THREE.Vector3): number {
    const v1 = new THREE.Vector3().subVectors(p2, p1);
    const v2 = new THREE.Vector3().subVectors(p3, p1);
    const cross = new THREE.Vector3().crossVectors(v1, v2);
    return 0.5 * cross.length();
  }

  public static disposeFromScene(face3DData: Face3DData, scene: THREE.Scene): void {
    if (!face3DData || !scene) {
      return;
    }
    
    const objectsToRemove: THREE.Object3D[] = [];
    scene.traverse((object) => {
      if (object.userData && 
          (object.userData.handle === face3DData.Handle || 
           object.userData.uuid === face3DData.Uuid)) {
        objectsToRemove.push(object);
      }
    });
    
    objectsToRemove.forEach(obj => {
      scene.remove(obj);
      if (obj instanceof THREE.Mesh) {
        if (obj.geometry) {
          obj.geometry.dispose();
        }
        if (obj.material) {
          if (Array.isArray(obj.material)) {
            obj.material.forEach(material => material.dispose());
          } else {
            obj.material.dispose();
          }
        }
      }
    });
  }

  public static update(face3DData: Face3DData, scene: THREE.Scene): THREE.Mesh | null {
    this.disposeFromScene(face3DData, scene);
    return this.render(face3DData, scene);
  }

  public static getFace3DObjects(face3DData: Face3DData, scene: THREE.Scene): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];
    scene.traverse((object) => {
      if (object.userData && 
          (object.userData.handle === face3DData.Handle || 
           object.userData.uuid === face3DData.Uuid)) {
        objects.push(object);
      }
    });
    return objects;
  }

  public static setVisibility(face3DData: Face3DData, scene: THREE.Scene, visible: boolean): void {
    const objects = this.getFace3DObjects(face3DData, scene);
    objects.forEach(obj => {
      obj.visible = visible;
    });
  }

  public static setColor(face3DData: Face3DData, scene: THREE.Scene, color: string): void {
    const objects = this.getFace3DObjects(face3DData, scene);
    objects.forEach(obj => {
      if (obj instanceof THREE.Mesh) {
        if (obj.material instanceof THREE.MeshBasicMaterial || 
            obj.material instanceof THREE.MeshPhongMaterial) {
          obj.material.color.set(color);
        }
      }
    });
  }

  public static setOpacity(face3DData: Face3DData, scene: THREE.Scene, opacity: number): void {
    const objects = this.getFace3DObjects(face3DData, scene);
    objects.forEach(obj => {
      if (obj instanceof THREE.Mesh) {
        if (obj.material instanceof THREE.MeshBasicMaterial || 
            obj.material instanceof THREE.MeshPhongMaterial) {
          obj.material.opacity = opacity;
          obj.material.transparent = opacity < 1.0;
        }
      }
    });
  }
}
