import * as THREE from 'three';

export interface Point3DData {
  X: number;
  Y: number;
  Z: number;
}

export interface NormalData {
  X: number;
  Y: number;
  Z: number;
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

export interface BoundsData3D {
  Min: Point3DData;
  Max: Point3DData;
}

export interface VertexData {
  Location: Point3DData;
  Normal: NormalData;
  Color: ColorData;
}

export interface FaceData {
  VertexIndices: number[];
  Normal: NormalData;
}

export interface PolyfaceMeshData {
  Vertices: VertexData[];
  Faces: FaceData[];
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  VertexCount: number;
  FaceCount: number;
  Vertices3D: Point3DData[];
  Indices: number[];
  NormalsArray: number[];
  ColorsArray: number[];
  UVsArray: number[];
  Color: ColorData;
  Bounds3D: BoundsData3D;
  Centroid: Point3DData;
  Normals: NormalData[];
  Transform: TransformData;
  EntityType: string;
  Visible: boolean;
  LayerName: string;
  LayerIndex: number;
  Handle: string;
  Transparency: number;
  MaterialName: string;
  CastShadows: boolean;
  ReceiveShadows: boolean;
  GeometryType: string;
  DoubleSided: boolean;
  FlatShading: boolean;
}

export class PolyfaceMeshEntityThreejsRenderer {
  private static readonly DEFAULT_OPACITY = 1.0;
  private static readonly DEFAULT_COLOR = '#FFFFFF';
  private static readonly meshCache = new Map<string, THREE.Mesh>();

  public static render(polyfaceMeshData: PolyfaceMeshData, scene: THREE.Scene): THREE.Group {
    const group = new THREE.Group();
    group.name = `PolyfaceMesh_${polyfaceMeshData.Handle}`;
    group.visible = polyfaceMeshData.Visible;

    const geometry = this.createMeshGeometry(polyfaceMeshData);
    const material = this.createMeshMaterial(polyfaceMeshData);
    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = 'PolyfaceMesh';
    mesh.userData = { handle: polyfaceMeshData.Handle };
    mesh.visible = polyfaceMeshData.Visible;
    mesh.castShadow = polyfaceMeshData.CastShadows;
    mesh.receiveShadow = polyfaceMeshData.ReceiveShadows;

    group.add(mesh);

    this.meshCache.set(polyfaceMeshData.Handle, mesh);

    if (polyfaceMeshData.Visible) {
      scene.add(group);
    }

    return group;
  }

  public static update(polyfaceMeshData: PolyfaceMeshData, scene: THREE.Scene): boolean {
    const existingMesh = this.meshCache.get(polyfaceMeshData.Handle);
    if (!existingMesh) {
      return false;
    }

    const material = existingMesh.material as THREE.MeshStandardMaterial;
    if (material) {
      material.color.setHex(parseInt(polyfaceMeshData.Color.Hex.replace('#', ''), 16));
      material.opacity = 1.0 - polyfaceMeshData.Transparency;
      material.transparent = polyfaceMeshData.Transparency > 0;
      material.side = polyfaceMeshData.DoubleSided ? THREE.DoubleSide : THREE.FrontSide;
      material.flatShading = polyfaceMeshData.FlatShading;
      material.needsUpdate = true;
    }

    const geometry = existingMesh.geometry as THREE.BufferGeometry;
    if (geometry) {
      const positions = geometry.attributes.position.array as Float32Array;
      const normals = geometry.attributes.normal.array as Float32Array;
      const colors = geometry.attributes.color.array as Float32Array;

      for (let i = 0; i < polyfaceMeshData.Vertices3D.length; i++) {
        positions[i * 3] = polyfaceMeshData.Vertices3D[i].X;
        positions[i * 3 + 1] = polyfaceMeshData.Vertices3D[i].Y;
        positions[i * 3 + 2] = polyfaceMeshData.Vertices3D[i].Z;

        normals[i * 3] = polyfaceMeshData.NormalsArray[i * 3];
        normals[i * 3 + 1] = polyfaceMeshData.NormalsArray[i * 3 + 1];
        normals[i * 3 + 2] = polyfaceMeshData.NormalsArray[i * 3 + 2];

        colors[i * 3] = polyfaceMeshData.ColorsArray[i * 3];
        colors[i * 3 + 1] = polyfaceMeshData.ColorsArray[i * 3 + 1];
        colors[i * 3 + 2] = polyfaceMeshData.ColorsArray[i * 3 + 2];
      }

      geometry.attributes.position.needsUpdate = true;
      geometry.attributes.normal.needsUpdate = true;
      geometry.attributes.color.needsUpdate = true;
    }

    return true;
  }

  public static dispose(polyfaceMeshData: PolyfaceMeshData, scene: THREE.Scene): boolean {
    const existingMesh = this.meshCache.get(polyfaceMeshData.Handle);
    if (!existingMesh) {
      return false;
    }

    const group = existingMesh.parent;
    if (group) {
      scene.remove(group);
    }

    existingMesh.geometry.dispose();
    (existingMesh.material as THREE.Material).dispose();
    this.meshCache.delete(polyfaceMeshData.Handle);

    return true;
  }

  public static setVisibility(polyfaceMeshData: PolyfaceMeshData, scene: THREE.Scene, visible: boolean): boolean {
    const existingMesh = this.meshCache.get(polyfaceMeshData.Handle);
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

  public static setOpacity(polyfaceMeshData: PolyfaceMeshData, opacity: number): boolean {
    const existingMesh = this.meshCache.get(polyfaceMeshData.Handle);
    if (!existingMesh) {
      return false;
    }

    const material = existingMesh.material as THREE.MeshStandardMaterial;
    if (material) {
      material.opacity = opacity;
      material.transparent = opacity < 1.0;
    }

    return true;
  }

  public static setColor(polyfaceMeshData: PolyfaceMeshData, color: THREE.Color): boolean {
    const existingMesh = this.meshCache.get(polyfaceMeshData.Handle);
    if (!existingMesh) {
      return false;
    }

    const material = existingMesh.material as THREE.MeshStandardMaterial;
    if (material) {
      material.color = color;
    }

    return true;
  }

  public static getMeshGroup(polyfaceMeshData: PolyfaceMeshData): THREE.Mesh | null {
    return this.meshCache.get(polyfaceMeshData.Handle) || null;
  }

  public static getVertices(polyfaceMeshData: PolyfaceMeshData): THREE.Vector3[] {
    return polyfaceMeshData.Vertices3D.map(v => new THREE.Vector3(v.X, v.Y, v.Z));
  }

  public static getIndices(polyfaceMeshData: PolyfaceMeshData): number[] {
    return polyfaceMeshData.Indices || [];
  }

  public static getNormals(polyfaceMeshData: PolyfaceMeshData): THREE.Vector3[] {
    return polyfaceMeshData.Normals.map(n => new THREE.Vector3(n.X, n.Y, n.Z));
  }

  public static getFaces(polyfaceMeshData: PolyfaceMeshData): FaceData[] {
    return polyfaceMeshData.Faces || [];
  }

  public static getColor(polyfaceMeshData: PolyfaceMeshData): THREE.Color {
    return new THREE.Color(polyfaceMeshData.Color.Hex);
  }

  public static getBounds(polyfaceMeshData: PolyfaceMeshData): THREE.Box3 {
    const min = new THREE.Vector3(polyfaceMeshData.Bounds3D.Min.X, polyfaceMeshData.Bounds3D.Min.Y, polyfaceMeshData.Bounds3D.Min.Z);
    const max = new THREE.Vector3(polyfaceMeshData.Bounds3D.Max.X, polyfaceMeshData.Bounds3D.Max.Y, polyfaceMeshData.Bounds3D.Max.Z);
    return new THREE.Box3(min, max);
  }

  public static getCentroid(polyfaceMeshData: PolyfaceMeshData): THREE.Vector3 {
    return new THREE.Vector3(polyfaceMeshData.Centroid.X, polyfaceMeshData.Centroid.Y, polyfaceMeshData.Centroid.Z);
  }

  public static getVertexCount(polyfaceMeshData: PolyfaceMeshData): number {
    return polyfaceMeshData.VertexCount || 0;
  }

  public static getFaceCount(polyfaceMeshData: PolyfaceMeshData): number {
    return polyfaceMeshData.FaceCount || 0;
  }

  public static clearCache(): void {
    this.meshCache.forEach((mesh) => {
      mesh.geometry.dispose();
      (mesh.material as THREE.Material).dispose();
    });
    this.meshCache.clear();
  }

  private static createMeshGeometry(polyfaceMeshData: PolyfaceMeshData): THREE.BufferGeometry {
    const geometry = new THREE.BufferGeometry();

    const positions = new Float32Array(polyfaceMeshData.Vertices3D.length * 3);
    const normals = new Float32Array(polyfaceMeshData.NormalsArray.length);
    const colors = new Float32Array(polyfaceMeshData.ColorsArray.length);

    for (let i = 0; i < polyfaceMeshData.Vertices3D.length; i++) {
      positions[i * 3] = polyfaceMeshData.Vertices3D[i].X;
      positions[i * 3 + 1] = polyfaceMeshData.Vertices3D[i].Y;
      positions[i * 3 + 2] = polyfaceMeshData.Vertices3D[i].Z;
    }

    for (let i = 0; i < polyfaceMeshData.NormalsArray.length; i++) {
      normals[i] = polyfaceMeshData.NormalsArray[i];
    }

    for (let i = 0; i < polyfaceMeshData.ColorsArray.length; i++) {
      colors[i] = polyfaceMeshData.ColorsArray[i];
    }

    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    geometry.setAttribute('normal', new THREE.BufferAttribute(normals, 3));
    geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3));

    if (polyfaceMeshData.Indices && polyfaceMeshData.Indices.length > 0) {
      geometry.setIndex(polyfaceMeshData.Indices);
    }

    if (polyfaceMeshData.UVsArray && polyfaceMeshData.UVsArray.length > 0) {
      const uvs = new Float32Array(polyfaceMeshData.UVsArray.length);
      for (let i = 0; i < polyfaceMeshData.UVsArray.length; i++) {
        uvs[i] = polyfaceMeshData.UVsArray[i];
      }
      geometry.setAttribute('uv', new THREE.BufferAttribute(uvs, 2));
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static createMeshMaterial(polyfaceMeshData: PolyfaceMeshData): THREE.MeshStandardMaterial {
    const material = new THREE.MeshStandardMaterial({
      color: polyfaceMeshData.Color.Hex,
      vertexColors: polyfaceMeshData.ColorsArray.length > 0,
      side: polyfaceMeshData.DoubleSided ? THREE.DoubleSide : THREE.FrontSide,
      flatShading: polyfaceMeshData.FlatShading,
      opacity: 1.0 - polyfaceMeshData.Transparency,
      transparent: polyfaceMeshData.Transparency > 0,
      depthTest: true,
      depthWrite: true
    });

    return material;
  }

  public static getTransform(polyfaceMeshData: PolyfaceMeshData): TransformData {
    if (!polyfaceMeshData.Transform) {
      return {
        Position: { X: 0, Y: 0, Z: 0 },
        Rotation: { X: 0, Y: 0, Z: 0 },
        Scale: { X: 1, Y: 1, Z: 1 },
        Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
      };
    }
    return polyfaceMeshData.Transform;
  }

  public static getBounds3D(polyfaceMeshData: PolyfaceMeshData): BoundsData3D {
    if (!polyfaceMeshData.Bounds3D) {
      return {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 0, Y: 0, Z: 0 }
      };
    }
    return polyfaceMeshData.Bounds3D;
  }

  public static getCentroid3D(polyfaceMeshData: PolyfaceMeshData): Point3DData {
    if (!polyfaceMeshData.Centroid) {
      return { X: 0, Y: 0, Z: 0 };
    }
    return polyfaceMeshData.Centroid;
  }

  public static getHandle(polyfaceMeshData: PolyfaceMeshData): string {
    return polyfaceMeshData.Handle || '';
  }

  public static getLayerName(polyfaceMeshData: PolyfaceMeshData): string {
    return polyfaceMeshData.LayerName || '';
  }

  public static getVisible(polyfaceMeshData: PolyfaceMeshData): boolean {
    return polyfaceMeshData.Visible !== false;
  }

  public static getOpacity(polyfaceMeshData: PolyfaceMeshData): number {
    return 1.0 - (polyfaceMeshData.Transparency || 0);
  }

  public static getTransparent(polyfaceMeshData: PolyfaceMeshData): boolean {
    return (polyfaceMeshData.Transparency || 0) > 0;
  }

  public static getCastShadows(polyfaceMeshData: PolyfaceMeshData): boolean {
    return polyfaceMeshData.CastShadows !== false;
  }

  public static getReceiveShadows(polyfaceMeshData: PolyfaceMeshData): boolean {
    return polyfaceMeshData.ReceiveShadows !== false;
  }

  public static getDoubleSided(polyfaceMeshData: PolyfaceMeshData): boolean {
    return polyfaceMeshData.DoubleSided || false;
  }

  public static getFlatShading(polyfaceMeshData: PolyfaceMeshData): boolean {
    return polyfaceMeshData.FlatShading || false;
  }

  public static getGeometryType(polyfaceMeshData: PolyfaceMeshData): string {
    return polyfaceMeshData.GeometryType || 'BufferGeometry';
  }

  public static getEntityType(polyfaceMeshData: PolyfaceMeshData): string {
    return polyfaceMeshData.EntityType || 'PolyfaceMesh';
  }
}
