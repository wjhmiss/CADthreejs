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

export interface EdgeData {
  StartIndex: number;
  EndIndex: number;
  Crease: number | null;
}

export interface PolygonMeshData {
  Vertices: VertexData[];
  Faces: FaceData[];
  Edges: EdgeData[];
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  VertexCount: number;
  FaceCount: number;
  EdgeCount: number;
  SubdivisionLevel: number;
  Version: number;
  BlendCrease: boolean;
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

export class PolygonMeshEntityThreejsRenderer {
  private static readonly DEFAULT_OPACITY = 1.0;
  private static readonly DEFAULT_COLOR = '#FFFFFF';
  private static readonly meshCache = new Map<string, THREE.Mesh>();
  private static readonly edgeCache = new Map<string, THREE.LineSegments>();

  public static render(polygonMeshData: PolygonMeshData, scene: THREE.Scene): THREE.Group | null {
    if (!polygonMeshData || !polygonMeshData.Visible) {
      return null;
    }

    const group = new THREE.Group();
    group.name = `PolygonMesh_${polygonMeshData.Handle}`;
    group.visible = polygonMeshData.Visible;

    const geometry = this.createMeshGeometry(polygonMeshData);
    const material = this.createMeshMaterial(polygonMeshData);
    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = 'PolygonMesh';
    mesh.userData = { handle: polygonMeshData.Handle };
    mesh.visible = polygonMeshData.Visible;
    mesh.castShadow = polygonMeshData.CastShadows;
    mesh.receiveShadow = polygonMeshData.ReceiveShadows;

    group.add(mesh);

    this.meshCache.set(polygonMeshData.Handle, mesh);

    if (polygonMeshData.Edges && polygonMeshData.Edges.length > 0) {
      const edgeGeometry = this.createEdgeGeometry(polygonMeshData);
      const edgeMaterial = this.createEdgeMaterial(polygonMeshData);
      const edges = new THREE.LineSegments(edgeGeometry, edgeMaterial);
      edges.name = 'PolygonMeshEdges';
      edges.visible = polygonMeshData.Visible;
      group.add(edges);
      this.edgeCache.set(polygonMeshData.Handle, edges);
    }

    return group;
  }

  public static update(polygonMeshData: PolygonMeshData, scene: THREE.Scene): boolean {
    const existingMesh = this.meshCache.get(polygonMeshData.Handle);
    if (!existingMesh) {
      return false;
    }

    const material = existingMesh.material as THREE.MeshStandardMaterial;
    if (material) {
      material.color.setHex(parseInt(polygonMeshData.Color.Hex.replace('#', ''), 16));
      material.opacity = 1.0 - polygonMeshData.Transparency;
      material.transparent = polygonMeshData.Transparency > 0;
      material.side = polygonMeshData.DoubleSided ? THREE.DoubleSide : THREE.FrontSide;
      material.flatShading = polygonMeshData.FlatShading;
      material.needsUpdate = true;
    }

    const geometry = existingMesh.geometry as THREE.BufferGeometry;
    if (geometry) {
      const positions = geometry.attributes.position.array as Float32Array;
      const normals = geometry.attributes.normal.array as Float32Array;
      const colors = geometry.attributes.color.array as Float32Array;

      for (let i = 0; i < polygonMeshData.Vertices3D.length; i++) {
        positions[i * 3] = polygonMeshData.Vertices3D[i].X;
        positions[i * 3 + 1] = polygonMeshData.Vertices3D[i].Y;
        positions[i * 3 + 2] = polygonMeshData.Vertices3D[i].Z;

        normals[i * 3] = polygonMeshData.NormalsArray[i * 3];
        normals[i * 3 + 1] = polygonMeshData.NormalsArray[i * 3 + 1];
        normals[i * 3 + 2] = polygonMeshData.NormalsArray[i * 3 + 2];

        colors[i * 3] = polygonMeshData.ColorsArray[i * 3];
        colors[i * 3 + 1] = polygonMeshData.ColorsArray[i * 3 + 1];
        colors[i * 3 + 2] = polygonMeshData.ColorsArray[i * 3 + 2];
      }

      geometry.attributes.position.needsUpdate = true;
      geometry.attributes.normal.needsUpdate = true;
      geometry.attributes.color.needsUpdate = true;
    }

    const existingEdges = this.edgeCache.get(polygonMeshData.Handle);
    if (existingEdges && polygonMeshData.Edges && polygonMeshData.Edges.length > 0) {
      const edgeGeometry = existingEdges.geometry as THREE.BufferGeometry;
      if (edgeGeometry) {
        const edgePositions = edgeGeometry.attributes.position.array as Float32Array;
        for (let i = 0; i < polygonMeshData.Edges.length; i++) {
          const edge = polygonMeshData.Edges[i];
          const startVertex = polygonMeshData.Vertices3D[edge.StartIndex];
          const endVertex = polygonMeshData.Vertices3D[edge.EndIndex];
          edgePositions[i * 6] = startVertex.X;
          edgePositions[i * 6 + 1] = startVertex.Y;
          edgePositions[i * 6 + 2] = startVertex.Z;
          edgePositions[i * 6 + 3] = endVertex.X;
          edgePositions[i * 6 + 4] = endVertex.Y;
          edgePositions[i * 6 + 5] = endVertex.Z;
        }
        edgeGeometry.attributes.position.needsUpdate = true;
      }
    }

    return true;
  }

  public static dispose(polygonMeshData: PolygonMeshData, scene: THREE.Scene): boolean {
    const existingMesh = this.meshCache.get(polygonMeshData.Handle);
    if (!existingMesh) {
      return false;
    }

    const group = existingMesh.parent;
    if (group) {
      scene.remove(group);
    }

    existingMesh.geometry.dispose();
    (existingMesh.material as THREE.Material).dispose();
    this.meshCache.delete(polygonMeshData.Handle);

    const existingEdges = this.edgeCache.get(polygonMeshData.Handle);
    if (existingEdges) {
      existingEdges.geometry.dispose();
      (existingEdges.material as THREE.Material).dispose();
      this.edgeCache.delete(polygonMeshData.Handle);
    }

    return true;
  }

  public static setVisibility(polygonMeshData: PolygonMeshData, scene: THREE.Scene, visible: boolean): boolean {
    const existingMesh = this.meshCache.get(polygonMeshData.Handle);
    if (!existingMesh) {
      return false;
    }

    const group = existingMesh.parent;
    if (group) {
      group.visible = visible;
    }
    existingMesh.visible = visible;

    const existingEdges = this.edgeCache.get(polygonMeshData.Handle);
    if (existingEdges) {
      existingEdges.visible = visible;
    }

    return true;
  }

  public static setOpacity(polygonMeshData: PolygonMeshData, opacity: number): boolean {
    const existingMesh = this.meshCache.get(polygonMeshData.Handle);
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

  public static setColor(polygonMeshData: PolygonMeshData, color: THREE.Color): boolean {
    const existingMesh = this.meshCache.get(polygonMeshData.Handle);
    if (!existingMesh) {
      return false;
    }

    const material = existingMesh.material as THREE.MeshStandardMaterial;
    if (material) {
      material.color = color;
    }

    return true;
  }

  public static getMeshGroup(polygonMeshData: PolygonMeshData): THREE.Mesh | null {
    return this.meshCache.get(polygonMeshData.Handle) || null;
  }

  public static getEdgesGroup(polygonMeshData: PolygonMeshData): THREE.LineSegments | null {
    return this.edgeCache.get(polygonMeshData.Handle) || null;
  }

  public static getVertices(polygonMeshData: PolygonMeshData): THREE.Vector3[] {
    return polygonMeshData.Vertices3D.map(v => new THREE.Vector3(v.X, v.Y, v.Z));
  }

  public static getIndices(polygonMeshData: PolygonMeshData): number[] {
    return polygonMeshData.Indices || [];
  }

  public static getNormals(polygonMeshData: PolygonMeshData): THREE.Vector3[] {
    return polygonMeshData.Normals.map(n => new THREE.Vector3(n.X, n.Y, n.Z));
  }

  public static getFaces(polygonMeshData: PolygonMeshData): FaceData[] {
    return polygonMeshData.Faces || [];
  }

  public static getEdges(polygonMeshData: PolygonMeshData): EdgeData[] {
    return polygonMeshData.Edges || [];
  }

  public static getColor(polygonMeshData: PolygonMeshData): THREE.Color {
    return new THREE.Color(polygonMeshData.Color.Hex);
  }

  public static getBounds(polygonMeshData: PolygonMeshData): THREE.Box3 {
    const min = new THREE.Vector3(polygonMeshData.Bounds3D.Min.X, polygonMeshData.Bounds3D.Min.Y, polygonMeshData.Bounds3D.Min.Z);
    const max = new THREE.Vector3(polygonMeshData.Bounds3D.Max.X, polygonMeshData.Bounds3D.Max.Y, polygonMeshData.Bounds3D.Max.Z);
    return new THREE.Box3(min, max);
  }

  public static getCentroid(polygonMeshData: PolygonMeshData): THREE.Vector3 {
    return new THREE.Vector3(polygonMeshData.Centroid.X, polygonMeshData.Centroid.Y, polygonMeshData.Centroid.Z);
  }

  public static getVertexCount(polygonMeshData: PolygonMeshData): number {
    return polygonMeshData.VertexCount || 0;
  }

  public static getFaceCount(polygonMeshData: PolygonMeshData): number {
    return polygonMeshData.FaceCount || 0;
  }

  public static getEdgeCount(polygonMeshData: PolygonMeshData): number {
    return polygonMeshData.EdgeCount || 0;
  }

  public static getSubdivisionLevel(polygonMeshData: PolygonMeshData): number {
    return polygonMeshData.SubdivisionLevel || 0;
  }

  public static getVersion(polygonMeshData: PolygonMeshData): number {
    return polygonMeshData.Version || 0;
  }

  public static getBlendCrease(polygonMeshData: PolygonMeshData): boolean {
    return polygonMeshData.BlendCrease || false;
  }

  public static clearCache(): void {
    this.meshCache.forEach((mesh) => {
      mesh.geometry.dispose();
      (mesh.material as THREE.Material).dispose();
    });
    this.meshCache.clear();

    this.edgeCache.forEach((edges) => {
      edges.geometry.dispose();
      (edges.material as THREE.Material).dispose();
    });
    this.edgeCache.clear();
  }

  private static createMeshGeometry(polygonMeshData: PolygonMeshData): THREE.BufferGeometry {
    const geometry = new THREE.BufferGeometry();

    const positions = new Float32Array(polygonMeshData.Vertices3D.length * 3);
    const normals = new Float32Array(polygonMeshData.NormalsArray.length);
    const colors = new Float32Array(polygonMeshData.ColorsArray.length);

    for (let i = 0; i < polygonMeshData.Vertices3D.length; i++) {
      positions[i * 3] = polygonMeshData.Vertices3D[i].X;
      positions[i * 3 + 1] = polygonMeshData.Vertices3D[i].Y;
      positions[i * 3 + 2] = polygonMeshData.Vertices3D[i].Z;
    }

    for (let i = 0; i < polygonMeshData.NormalsArray.length; i++) {
      normals[i] = polygonMeshData.NormalsArray[i];
    }

    for (let i = 0; i < polygonMeshData.ColorsArray.length; i++) {
      colors[i] = polygonMeshData.ColorsArray[i];
    }

    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    geometry.setAttribute('normal', new THREE.BufferAttribute(normals, 3));
    geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3));

    if (polygonMeshData.Indices && polygonMeshData.Indices.length > 0) {
      geometry.setIndex(polygonMeshData.Indices);
    }

    if (polygonMeshData.UVsArray && polygonMeshData.UVsArray.length > 0) {
      const uvs = new Float32Array(polygonMeshData.UVsArray.length);
      for (let i = 0; i < polygonMeshData.UVsArray.length; i++) {
        uvs[i] = polygonMeshData.UVsArray[i];
      }
      geometry.setAttribute('uv', new THREE.BufferAttribute(uvs, 2));
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static createEdgeGeometry(polygonMeshData: PolygonMeshData): THREE.BufferGeometry {
    const geometry = new THREE.BufferGeometry();
    const positions = new Float32Array(polygonMeshData.Edges.length * 6);

    for (let i = 0; i < polygonMeshData.Edges.length; i++) {
      const edge = polygonMeshData.Edges[i];
      const startVertex = polygonMeshData.Vertices3D[edge.StartIndex];
      const endVertex = polygonMeshData.Vertices3D[edge.EndIndex];
      positions[i * 6] = startVertex.X;
      positions[i * 6 + 1] = startVertex.Y;
      positions[i * 6 + 2] = startVertex.Z;
      positions[i * 6 + 3] = endVertex.X;
      positions[i * 6 + 4] = endVertex.Y;
      positions[i * 6 + 5] = endVertex.Z;
    }

    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    geometry.computeBoundingSphere();

    return geometry;
  }

  private static createMeshMaterial(polygonMeshData: PolygonMeshData): THREE.MeshStandardMaterial {
    const material = new THREE.MeshStandardMaterial({
      color: polygonMeshData.Color.Hex,
      vertexColors: polygonMeshData.ColorsArray.length > 0,
      side: polygonMeshData.DoubleSided ? THREE.DoubleSide : THREE.FrontSide,
      flatShading: polygonMeshData.FlatShading,
      opacity: 1.0 - polygonMeshData.Transparency,
      transparent: polygonMeshData.Transparency > 0,
      depthTest: true,
      depthWrite: true
    });

    return material;
  }

  private static createEdgeMaterial(polygonMeshData: PolygonMeshData): THREE.LineBasicMaterial {
    const material = new THREE.LineBasicMaterial({
      color: 0x000000,
      opacity: 0.5,
      transparent: true,
      depthTest: true,
      depthWrite: false
    });

    return material;
  }

  public static getTransform(polygonMeshData: PolygonMeshData): TransformData {
    if (!polygonMeshData.Transform) {
      return {
        Position: { X: 0, Y: 0, Z: 0 },
        Rotation: { X: 0, Y: 0, Z: 0 },
        Scale: { X: 1, Y: 1, Z: 1 },
        Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
      };
    }
    return polygonMeshData.Transform;
  }

  public static getBounds3D(polygonMeshData: PolygonMeshData): BoundsData3D {
    if (!polygonMeshData.Bounds3D) {
      return {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 0, Y: 0, Z: 0 }
      };
    }
    return polygonMeshData.Bounds3D;
  }

  public static getCentroid3D(polygonMeshData: PolygonMeshData): Point3DData {
    if (!polygonMeshData.Centroid) {
      return { X: 0, Y: 0, Z: 0 };
    }
    return polygonMeshData.Centroid;
  }

  public static getHandle(polygonMeshData: PolygonMeshData): string {
    return polygonMeshData.Handle || '';
  }

  public static getLayerName(polygonMeshData: PolygonMeshData): string {
    return polygonMeshData.LayerName || '';
  }

  public static getVisible(polygonMeshData: PolygonMeshData): boolean {
    return polygonMeshData.Visible !== false;
  }

  public static getOpacity(polygonMeshData: PolygonMeshData): number {
    return 1.0 - (polygonMeshData.Transparency || 0);
  }

  public static getTransparent(polygonMeshData: PolygonMeshData): boolean {
    return (polygonMeshData.Transparency || 0) > 0;
  }

  public static getCastShadows(polygonMeshData: PolygonMeshData): boolean {
    return polygonMeshData.CastShadows !== false;
  }

  public static getReceiveShadows(polygonMeshData: PolygonMeshData): boolean {
    return polygonMeshData.ReceiveShadows !== false;
  }

  public static getDoubleSided(polygonMeshData: PolygonMeshData): boolean {
    return polygonMeshData.DoubleSided || false;
  }

  public static getFlatShading(polygonMeshData: PolygonMeshData): boolean {
    return polygonMeshData.FlatShading || false;
  }

  public static getGeometryType(polygonMeshData: PolygonMeshData): string {
    return polygonMeshData.GeometryType || 'BufferGeometry';
  }

  public static getEntityType(polygonMeshData: PolygonMeshData): string {
    return polygonMeshData.EntityType || 'Mesh';
  }
}
