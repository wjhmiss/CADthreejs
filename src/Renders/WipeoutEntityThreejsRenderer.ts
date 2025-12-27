import * as THREE from 'three';

export interface Point3DData {
  X: number;
  Y: number;
  Z: number;
}

export interface PointData {
  X: number;
  Y: number;
}

export interface BoundsData {
  Min: Point3DData;
  Max: Point3DData;
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

export interface GeometryData {
  Type: string;
  VertexCount: number;
  FaceCount: number;
  HasNormals: boolean;
  HasColors: boolean;
  HasUVs: boolean;
  HasIndices: boolean;
  PrimitiveType: string;
  IndexCount: number;
  IsClosed: boolean;
  IsPeriodic: boolean;
}

export enum ClipType {
  Rectangular = 0,
  Polygonal = 1
}

export enum ImageDisplayFlags {
  ShowImage = 1,
  ShowWhenAligned = 2,
  UseClipping = 4,
  Transparency = 8
}

export interface WipeoutData {
  BoundaryPoints: Point3DData[];
  InsertPoint: Point3DData;
  UVector: Point3DData;
  VVector: Point3DData;
  Size: Point3DData;
  ClipType: ClipType;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  Brightness: number;
  Contrast: number;
  Fade: number;
  Flags: ImageDisplayFlags;
  ClippingState: boolean;
  Bounds?: BoundsData;
  Centroid?: Point3DData;
  BoundaryPointCount: number;
  Area: number;
  Transform: TransformData;
  Geometry: GeometryData;
  Material: MaterialData;
  VertexPositions: number[];
  VertexColors: number[];
  VertexNormals: number[];
  UV: number[];
  Indices: number[];
  VertexCount: number;
  Opacity: number;
  Transparent: boolean;
  IsMask: boolean;
  Normal: NormalData;
  Color: ColorData;
}

export class WipeoutEntityThreejsRenderer {
  private static readonly wipeoutCache = new Map<string, THREE.Mesh>();

  public static render(wipeoutData: WipeoutData, scene: THREE.Scene): THREE.Group {
    const group = new THREE.Group();
    group.name = `Wipeout_${wipeoutData.Handle}`;
    group.visible = wipeoutData.Visible !== false;

    const geometry = this.createWipeoutGeometry(wipeoutData);
    const material = this.createWipeoutMaterial(wipeoutData);
    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = 'Wipeout';
    mesh.userData = { handle: wipeoutData.Handle, isMask: wipeoutData.IsMask };
    mesh.visible = wipeoutData.Visible !== false;

    group.add(mesh);

    this.wipeoutCache.set(wipeoutData.Handle, mesh);

    if (wipeoutData.Visible !== false) {
      scene.add(group);
    }

    return group;
  }

  public static update(wipeoutData: WipeoutData, scene: THREE.Scene): boolean {
    const existingMesh = this.wipeoutCache.get(wipeoutData.Handle);
    if (!existingMesh) {
      return false;
    }

    const material = existingMesh.material as THREE.MeshBasicMaterial;
    if (material) {
      const color = new THREE.Color(wipeoutData.Material.Color);
      material.color = color;
      material.opacity = wipeoutData.Opacity;
      material.transparent = wipeoutData.Transparent;
      material.wireframe = wipeoutData.Material.Wireframe;
    }

    const geometry = existingMesh.geometry as THREE.BufferGeometry;
    if (geometry && wipeoutData.VertexPositions) {
      const positions = geometry.attributes.position.array as Float32Array;
      for (let i = 0; i < wipeoutData.VertexPositions.length && i < positions.length; i++) {
        positions[i] = wipeoutData.VertexPositions[i];
      }
      geometry.attributes.position.needsUpdate = true;
    }

    return true;
  }

  public static dispose(wipeoutData: WipeoutData, scene: THREE.Scene): boolean {
    const existingMesh = this.wipeoutCache.get(wipeoutData.Handle);
    if (!existingMesh) {
      return false;
    }

    const group = existingMesh.parent;
    if (group) {
      scene.remove(group);
    }

    existingMesh.geometry.dispose();
    (existingMesh.material as THREE.Material).dispose();
    this.wipeoutCache.delete(wipeoutData.Handle);

    return true;
  }

  public static setVisibility(wipeoutData: WipeoutData, scene: THREE.Scene, visible: boolean): boolean {
    const existingMesh = this.wipeoutCache.get(wipeoutData.Handle);
    if (!existingMesh) {
      return false;
    }

    const group = existingMesh.parent;
    if (group) {
      if (visible) {
        if (!group.parent) {
          scene.add(group);
        }
      } else {
        if (group.parent) {
          group.parent.remove(group);
        }
      }
      group.visible = visible;
    }
    existingMesh.visible = visible;
    return true;
  }

  public static setOpacity(wipeoutData: WipeoutData, opacity: number): boolean {
    const existingMesh = this.wipeoutCache.get(wipeoutData.Handle);
    if (!existingMesh) {
      return false;
    }

    const material = existingMesh.material as THREE.MeshBasicMaterial;
    if (material) {
      material.opacity = opacity;
      material.transparent = opacity < 1.0;
    }

    return true;
  }

  public static getWipeoutMesh(wipeoutData: WipeoutData): THREE.Mesh | null {
    return this.wipeoutCache.get(wipeoutData.Handle) || null;
  }

  public static getInsertPoint(wipeoutData: WipeoutData): THREE.Vector3 {
    return new THREE.Vector3(wipeoutData.InsertPoint.X, wipeoutData.InsertPoint.Y, wipeoutData.InsertPoint.Z);
  }

  public static getUVector(wipeoutData: WipeoutData): THREE.Vector3 {
    return new THREE.Vector3(wipeoutData.UVector.X, wipeoutData.UVector.Y, wipeoutData.UVector.Z);
  }

  public static getVVector(wipeoutData: WipeoutData): THREE.Vector3 {
    return new THREE.Vector3(wipeoutData.VVector.X, wipeoutData.VVector.Y, wipeoutData.VVector.Z);
  }

  public static getSize(wipeoutData: WipeoutData): THREE.Vector3 {
    return new THREE.Vector3(wipeoutData.Size.X, wipeoutData.Size.Y, wipeoutData.Size.Z);
  }

  public static getClipType(wipeoutData: WipeoutData): ClipType {
    return wipeoutData.ClipType;
  }

  public static getBoundaryPoints(wipeoutData: WipeoutData): THREE.Vector3[] {
    return wipeoutData.BoundaryPoints.map(p => new THREE.Vector3(p.X, p.Y, p.Z));
  }

  public static getArea(wipeoutData: WipeoutData): number {
    return wipeoutData.Area;
  }

  public static getBounds(wipeoutData: WipeoutData): THREE.Box3 | null {
    if (!wipeoutData.Bounds) {
      return null;
    }
    const min = new THREE.Vector3(wipeoutData.Bounds.Min.X, wipeoutData.Bounds.Min.Y, wipeoutData.Bounds.Min.Z);
    const max = new THREE.Vector3(wipeoutData.Bounds.Max.X, wipeoutData.Bounds.Max.Y, wipeoutData.Bounds.Max.Z);
    return new THREE.Box3(min, max);
  }

  public static getCentroid(wipeoutData: WipeoutData): THREE.Vector3 | null {
    if (!wipeoutData.Centroid) {
      return null;
    }
    return new THREE.Vector3(wipeoutData.Centroid.X, wipeoutData.Centroid.Y, wipeoutData.Centroid.Z);
  }

  public static getNormal(wipeoutData: WipeoutData): THREE.Vector3 {
    return new THREE.Vector3(wipeoutData.Normal.X, wipeoutData.Normal.Y, wipeoutData.Normal.Z);
  }

  public static getColor(wipeoutData: WipeoutData): THREE.Color {
    return new THREE.Color(wipeoutData.Material.Color);
  }

  public static clearCache(): void {
    this.wipeoutCache.forEach((mesh) => {
      mesh.geometry.dispose();
      (mesh.material as THREE.Material).dispose();
    });
    this.wipeoutCache.clear();
  }

  private static createWipeoutGeometry(wipeoutData: WipeoutData): THREE.BufferGeometry {
    const geometry = new THREE.BufferGeometry();

    if (wipeoutData.VertexPositions && wipeoutData.VertexPositions.length > 0) {
      const positions = new Float32Array(wipeoutData.VertexPositions);
      geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    } else {
      const positions = new Float32Array([0, 0, 0]);
      geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    }

    if (wipeoutData.VertexColors && wipeoutData.VertexColors.length > 0) {
      const colors = new Float32Array(wipeoutData.VertexColors);
      geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3));
    }

    if (wipeoutData.VertexNormals && wipeoutData.VertexNormals.length > 0) {
      const normals = new Float32Array(wipeoutData.VertexNormals);
      geometry.setAttribute('normal', new THREE.BufferAttribute(normals, 3));
    }

    if (wipeoutData.UV && wipeoutData.UV.length > 0) {
      const uv = new Float32Array(wipeoutData.UV);
      geometry.setAttribute('uv', new THREE.BufferAttribute(uv, 2));
    }

    if (wipeoutData.Indices && wipeoutData.Indices.length > 0) {
      geometry.setIndex(new THREE.BufferAttribute(new Uint16Array(wipeoutData.Indices), 1));
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static createWipeoutMaterial(wipeoutData: WipeoutData): THREE.MeshBasicMaterial {
    const material = new THREE.MeshBasicMaterial({
      color: wipeoutData.Material.Color,
      opacity: wipeoutData.Opacity,
      transparent: wipeoutData.Transparent,
      wireframe: wipeoutData.Material.Wireframe,
      side: wipeoutData.Material.Side ? THREE.DoubleSide : THREE.FrontSide,
      vertexColors: wipeoutData.Material.VertexColors,
      depthWrite: !wipeoutData.Transparent,
      depthTest: true
    });

    return material;
  }

  public static renderMultiple(wipeoutDataArray: WipeoutData[], scene: THREE.Scene): THREE.Group {
    const group = new THREE.Group();
    group.name = 'MultipleWipeouts';

    wipeoutDataArray.forEach((wipeoutData) => {
      const wipeoutGroup = this.render(wipeoutData, scene);
      group.add(wipeoutGroup);
    });

    scene.add(group);

    return group;
  }

  public static disposeMultiple(group: THREE.Group, scene: THREE.Scene): void {
    if (!group) {
      return;
    }

    scene.remove(group);
    group.children.forEach((child) => {
      if (child instanceof THREE.Group) {
        child.children.forEach((mesh) => {
          if (mesh instanceof THREE.Mesh) {
            mesh.geometry.dispose();
            (mesh.material as THREE.Material).dispose();
            this.wipeoutCache.delete(mesh.userData.handle);
          }
        });
      }
    });
  }

  public static getTransform(wipeoutData: WipeoutData): any {
    if (!wipeoutData.Transform) {
      return {
        ScaleX: 1.0,
        ScaleY: 1.0,
        ScaleZ: 1.0,
        RotationX: 0,
        RotationY: 0,
        RotationZ: 0,
        TranslateX: wipeoutData.InsertPoint.X,
        TranslateY: wipeoutData.InsertPoint.Y,
        TranslateZ: wipeoutData.InsertPoint.Z
      };
    }
    return {
      ScaleX: wipeoutData.Transform.Scale.X,
      ScaleY: wipeoutData.Transform.Scale.Y,
      ScaleZ: wipeoutData.Transform.Scale.Z,
      RotationX: wipeoutData.Transform.Rotation.X,
      RotationY: wipeoutData.Transform.Rotation.Y,
      RotationZ: wipeoutData.Transform.Rotation.Z,
      TranslateX: wipeoutData.Transform.Position.X,
      TranslateY: wipeoutData.Transform.Position.Y,
      TranslateZ: wipeoutData.Transform.Position.Z
    };
  }

  public static getGeometry(wipeoutData: WipeoutData): GeometryData {
    return wipeoutData.Geometry;
  }

  public static getMaterial(wipeoutData: WipeoutData): MaterialData {
    return wipeoutData.Material;
  }

  public static getIsMask(wipeoutData: WipeoutData): boolean {
    return wipeoutData.IsMask;
  }

  public static getHandle(wipeoutData: WipeoutData): string {
    return wipeoutData.Handle || '';
  }

  public static getVisible(wipeoutData: WipeoutData): boolean {
    return wipeoutData.Visible !== false;
  }

  public static getOpacity(wipeoutData: WipeoutData): number {
    return wipeoutData.Opacity !== undefined ? wipeoutData.Opacity : 1.0;
  }

  public static getTransparent(wipeoutData: WipeoutData): boolean {
    return wipeoutData.Transparent !== false;
  }

  public static getClippingState(wipeoutData: WipeoutData): boolean {
    return wipeoutData.ClippingState;
  }

  public static getBrightness(wipeoutData: WipeoutData): number {
    return wipeoutData.Brightness;
  }

  public static getContrast(wipeoutData: WipeoutData): number {
    return wipeoutData.Contrast;
  }

  public static getFade(wipeoutData: WipeoutData): number {
    return wipeoutData.Fade;
  }

  public static getFlags(wipeoutData: WipeoutData): ImageDisplayFlags {
    return wipeoutData.Flags;
  }
}
