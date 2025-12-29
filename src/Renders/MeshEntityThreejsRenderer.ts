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
  Center: Point3DData;
  Size: Point3DData;
}

export interface BoundsData3D {
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

export interface VertexData {
  X: number;
  Y: number;
}

export interface FaceData {
  VertexIndices: number[];
}

export interface EdgeData {
  Start: number;
  End: number;
}

export interface MeshData {
  Type: string;
  EntityType: string;
  Handle: string;
  LayerName: string;
  LayerIndex: number;
  Visible: boolean;
  CoordinateSystem: string;
  
  Vertices: VertexData[];
  Faces: FaceData[];
  Edges: EdgeData[];
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  
  VertexCount: number;
  FaceCount: number;
  EdgeCount: number;
  Bounds: BoundsData;
  Centroid: PointData;
  
  Vertices3D: Point3DData[];
  Centroid3D: Point3DData;
  Normals: Point3DData[];
  Bounds3D: BoundsData3D;
  Color: ColorData;
  Transform: TransformData;
  Normal: NormalData;
  
  Opacity: number;
  Transparent: boolean;
  MaterialType: string;
  DepthTest: boolean;
  DepthWrite: boolean;
  Side: boolean;
  
  SubdivisionLevel: number;
  Version: number;
  BlendCrease: boolean;
  FaceIndices: number[][];
  EdgeIndices: number[][];
}

export class MeshEntityThreejsRenderer {
  private static readonly DEFAULT_COLOR = '#ffffff';
  private static readonly DEFAULT_OPACITY = 1.0;
  private static readonly DEFAULT_MATERIAL = 'MeshStandardMaterial';

  public static render(meshData: MeshData, scene: THREE.Scene): THREE.Group | null {
    if (!meshData || !meshData.Visible) {
      return null;
    }

    if (!meshData.Vertices3D || meshData.Vertices3D.length === 0) {
      console.warn(`Mesh ${meshData.Handle} has no vertices`);
      return null;
    }

    const group = new THREE.Group();
    group.name = `Mesh_${meshData.Handle}`;
    group.visible = meshData.Visible;

    group.userData = {
      type: meshData.Type,
      entityType: meshData.EntityType,
      handle: meshData.Handle,
      layerName: meshData.LayerName,
      layerIndex: meshData.LayerIndex,
      coordinateSystem: meshData.CoordinateSystem,
      vertices: meshData.Vertices,
      faces: meshData.Faces,
      edges: meshData.Edges,
      colorIndex: meshData.ColorIndex,
      lineTypeName: meshData.LineTypeName,
      lineWeight: meshData.LineWeight,
      vertexCount: meshData.VertexCount,
      faceCount: meshData.FaceCount,
      edgeCount: meshData.EdgeCount,
      bounds: meshData.Bounds,
      centroid: meshData.Centroid,
      vertices3D: meshData.Vertices3D,
      centroid3D: meshData.Centroid3D,
      normals: meshData.Normals,
      bounds3D: meshData.Bounds3D,
      color: meshData.Color,
      transform: meshData.Transform,
      normal: meshData.Normal,
      opacity: meshData.Opacity,
      transparent: meshData.Transparent,
      materialType: meshData.MaterialType,
      depthTest: meshData.DepthTest,
      depthWrite: meshData.DepthWrite,
      side: meshData.Side,
      subdivisionLevel: meshData.SubdivisionLevel,
      version: meshData.Version,
      blendCrease: meshData.BlendCrease,
      faceIndices: meshData.FaceIndices,
      edgeIndices: meshData.EdgeIndices
    };

    if (meshData.Transform && meshData.Transform.Matrix) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(meshData.Transform.Matrix);
      group.applyMatrix4(matrix);
    }

    this.renderMesh(meshData, group);
    this.renderEdges(meshData, group);

    return group;
  }

  private static renderMesh(meshData: MeshData, group: THREE.Group): void {
    if (!meshData.Vertices3D || meshData.Vertices3D.length === 0) {
      return;
    }

    if (!meshData.FaceIndices || meshData.FaceIndices.length === 0) {
      console.warn(`Mesh ${meshData.Handle} has no faces`);
      return;
    }

    const geometry = new THREE.BufferGeometry();

    const vertices: number[] = [];
    meshData.Vertices3D.forEach(v => {
      vertices.push(v.X, v.Y, v.Z);
    });

    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));

    if (meshData.Normals && meshData.Normals.length > 0) {
      const normals: number[] = [];
      meshData.Normals.forEach(n => {
        normals.push(n.X, n.Y, n.Z);
      });
      geometry.setAttribute('normal', new THREE.Float32BufferAttribute(normals, 3));
    } else {
      geometry.computeVertexNormals();
    }

    const indices: number[] = [];
    meshData.FaceIndices.forEach(face => {
      if (face.length >= 3) {
        for (let i = 1; i < face.length - 1; i++) {
          indices.push(face[0], face[i], face[i + 1]);
        }
      }
    });

    if (indices.length > 0) {
      geometry.setIndex(indices);
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    const material = this.createMaterial(meshData);

    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = 'Mesh';
    mesh.userData = {
      type: 'Mesh',
      vertexCount: meshData.VertexCount,
      faceCount: meshData.FaceCount,
      subdivisionLevel: meshData.SubdivisionLevel
    };

    if (meshData.Side) {
      mesh.material.side = THREE.DoubleSide;
    }

    group.add(mesh);
  }

  private static renderEdges(meshData: MeshData, group: THREE.Group): void {
    if (!meshData.EdgeIndices || meshData.EdgeIndices.length === 0) {
      return;
    }

    if (!meshData.Vertices3D || meshData.Vertices3D.length === 0) {
      return;
    }

    const vertices: number[] = [];
    meshData.EdgeIndices.forEach(edge => {
      if (edge.length >= 2) {
        const startVertex = meshData.Vertices3D[edge[0]];
        const endVertex = meshData.Vertices3D[edge[1]];
        if (startVertex && endVertex) {
          vertices.push(startVertex.X, startVertex.Y, startVertex.Z);
          vertices.push(endVertex.X, endVertex.Y, endVertex.Z);
        }
      }
    });

    if (vertices.length === 0) {
      return;
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));

    const material = new THREE.LineBasicMaterial({
      color: meshData.Color?.Hex || this.DEFAULT_COLOR,
      transparent: meshData.Transparent || false,
      opacity: (meshData.Opacity || this.DEFAULT_OPACITY) * 0.5,
      depthTest: meshData.DepthTest !== false,
      depthWrite: meshData.DepthWrite !== false
    });

    const edges = new THREE.LineSegments(geometry, material);
    edges.name = 'Edges';
    edges.userData = {
      type: 'Edges',
      edgeCount: meshData.EdgeCount
    };

    group.add(edges);
  }

  private static createMaterial(meshData: MeshData): THREE.Material {
    const color = meshData.Color?.Hex || this.DEFAULT_COLOR;
    const opacity = meshData.Opacity || this.DEFAULT_OPACITY;
    const transparent = meshData.Transparent || opacity < 1.0;
    const materialType = meshData.MaterialType || this.DEFAULT_MATERIAL;

    const materialOptions: THREE.MeshStandardMaterialParameters = {
      color: color,
      transparent: transparent,
      opacity: opacity,
      depthTest: meshData.DepthTest !== false,
      depthWrite: meshData.DepthWrite !== false,
      side: meshData.Side ? THREE.DoubleSide : THREE.FrontSide
    };

    switch (materialType) {
      case 'MeshBasicMaterial':
        return new THREE.MeshBasicMaterial(materialOptions);
      case 'MeshPhongMaterial':
        return new THREE.MeshPhongMaterial(materialOptions);
      case 'MeshLambertMaterial':
        return new THREE.MeshLambertMaterial(materialOptions);
      case 'MeshPhysicalMaterial':
        return new THREE.MeshPhysicalMaterial(materialOptions);
      case 'MeshToonMaterial':
        return new THREE.MeshToonMaterial(materialOptions);
      case 'MeshStandardMaterial':
      default:
        return new THREE.MeshStandardMaterial(materialOptions);
    }
  }

  public static dispose(meshData: MeshData, scene: THREE.Scene): boolean {
    if (!meshData || !scene) {
      return false;
    }
    const group = scene.getObjectByName(`Mesh_${meshData.Handle}`);
    if (group) {
      scene.remove(group);
      this.disposeGroup(group);
      return true;
    }
    return false;
  }

  private static disposeGroup(group: THREE.Group): void {
    group.traverse((object) => {
      if (object instanceof THREE.Mesh || object instanceof THREE.LineSegments) {
        if (object.geometry) {
          object.geometry.dispose();
        }
        if (object.material) {
          if (Array.isArray(object.material)) {
            object.material.forEach(material => material.dispose());
          } else {
            object.material.dispose();
          }
        }
      }
    });
    group.clear();
  }

  public static update(meshData: MeshData, scene: THREE.Scene): boolean {
    const group = this.getMeshGroup(meshData, scene);
    if (!group) {
      return false;
    }
    this.dispose(meshData, scene);
    const result = this.render(meshData, scene);
    return result !== null;
  }

  public static getMeshGroup(meshData: MeshData, scene: THREE.Scene): THREE.Group | null {
    return scene.getObjectByName(`Mesh_${meshData.Handle}`) as THREE.Group || null;
  }

  public static setVisibility(meshData: MeshData, scene: THREE.Scene, visible: boolean): boolean {
    const group = this.getMeshGroup(meshData, scene);
    if (group) {
      group.visible = visible;
      return true;
    }
    return false;
  }

  public static setColor(meshData: MeshData, scene: THREE.Scene, color: string): boolean {
    const group = this.getMeshGroup(meshData, scene);
    if (group) {
      group.traverse((object) => {
        if (object instanceof THREE.Mesh) {
          if (object.material instanceof THREE.MeshStandardMaterial || 
              object.material instanceof THREE.MeshBasicMaterial ||
              object.material instanceof THREE.MeshPhongMaterial ||
              object.material instanceof THREE.MeshLambertMaterial) {
            object.material.color.set(color);
          }
        }
      });
      return true;
    }
    return false;
  }

  public static setOpacity(meshData: MeshData, scene: THREE.Scene, opacity: number): boolean {
    const group = this.getMeshGroup(meshData, scene);
    if (group) {
      const transparent = opacity < 1.0;
      group.traverse((object) => {
        if (object instanceof THREE.Mesh || object instanceof THREE.LineSegments) {
          if (object.material instanceof THREE.MeshStandardMaterial || 
              object.material instanceof THREE.MeshBasicMaterial ||
              object.material instanceof THREE.MeshPhongMaterial ||
              object.material instanceof THREE.MeshLambertMaterial ||
              object.material instanceof THREE.LineBasicMaterial) {
            object.material.opacity = opacity;
            object.material.transparent = transparent;
          }
        }
      });
      return true;
    }
    return false;
  }

  public static setWireframe(meshData: MeshData, scene: THREE.Scene, wireframe: boolean): boolean {
    const group = this.getMeshGroup(meshData, scene);
    if (group) {
      group.traverse((object) => {
        if (object instanceof THREE.Mesh) {
          if (object.material instanceof THREE.MeshStandardMaterial || 
              object.material instanceof THREE.MeshBasicMaterial ||
              object.material instanceof THREE.MeshPhongMaterial ||
              object.material instanceof THREE.MeshLambertMaterial) {
            object.material.wireframe = wireframe;
          }
        }
      });
      return true;
    }
    return false;
  }

  public static getVertexCount(meshData: MeshData): number {
    return meshData.VertexCount || 0;
  }

  public static getFaceCount(meshData: MeshData): number {
    return meshData.FaceCount || 0;
  }

  public static getEdgeCount(meshData: MeshData): number {
    return meshData.EdgeCount || 0;
  }

  public static getCentroid(meshData: MeshData): Point3DData {
    return meshData.Centroid3D || { X: 0, Y: 0, Z: 0 };
  }

  public static getBounds(meshData: MeshData): BoundsData3D {
    return meshData.Bounds3D || {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 0, Y: 0, Z: 0 },
      Center: { X: 0, Y: 0, Z: 0 },
      Size: { X: 0, Y: 0, Z: 0 }
    };
  }

  public static getNormals(meshData: MeshData): Point3DData[] {
    return meshData.Normals || [];
  }

  public static calculateVolume(meshData: MeshData): number {
    if (!meshData.Vertices3D || meshData.Vertices3D.length === 0) {
      return 0;
    }

    if (!meshData.FaceIndices || meshData.FaceIndices.length === 0) {
      return 0;
    }

    let volume = 0;
    const origin = new THREE.Vector3(0, 0, 0);

    meshData.FaceIndices.forEach(face => {
      if (face.length >= 3) {
        for (let i = 1; i < face.length - 1; i++) {
          const v0 = meshData.Vertices3D[face[0]];
          const v1 = meshData.Vertices3D[face[i]];
          const v2 = meshData.Vertices3D[face[i + 1]];

          if (v0 && v1 && v2) {
            const p0 = new THREE.Vector3(v0.X, v0.Y, v0.Z);
            const p1 = new THREE.Vector3(v1.X, v1.Y, v1.Z);
            const p2 = new THREE.Vector3(v2.X, v2.Y, v2.Z);

            const tetraVolume = p0.dot(p1.cross(p2)) / 6;
            volume += tetraVolume;
          }
        }
      }
    });

    return Math.abs(volume);
  }

  public static calculateSurfaceArea(meshData: MeshData): number {
    if (!meshData.Vertices3D || meshData.Vertices3D.length === 0) {
      return 0;
    }

    if (!meshData.FaceIndices || meshData.FaceIndices.length === 0) {
      return 0;
    }

    let area = 0;

    meshData.FaceIndices.forEach(face => {
      if (face.length >= 3) {
        for (let i = 1; i < face.length - 1; i++) {
          const v0 = meshData.Vertices3D[face[0]];
          const v1 = meshData.Vertices3D[face[i]];
          const v2 = meshData.Vertices3D[face[i + 1]];

          if (v0 && v1 && v2) {
            const p0 = new THREE.Vector3(v0.X, v0.Y, v0.Z);
            const p1 = new THREE.Vector3(v1.X, v1.Y, v1.Z);
            const p2 = new THREE.Vector3(v2.X, v2.Y, v2.Z);

            const edge1 = new THREE.Vector3().subVectors(p1, p0);
            const edge2 = new THREE.Vector3().subVectors(p2, p0);
            const faceArea = edge1.cross(edge2).length() / 2;
            area += faceArea;
          }
        }
      }
    });

    return area;
  }

  public static isPointInside(meshData: MeshData, point: Point3DData, tolerance: number = 0.001): boolean {
    if (!meshData.Bounds3D) {
      return false;
    }

    const bounds = meshData.Bounds3D;
    if (point.X < bounds.Min.X - tolerance || point.X > bounds.Max.X + tolerance ||
        point.Y < bounds.Min.Y - tolerance || point.Y > bounds.Max.Y + tolerance ||
        point.Z < bounds.Min.Z - tolerance || point.Z > bounds.Max.Z + tolerance) {
      return false;
    }

    const testPoint = new THREE.Vector3(point.X, point.Y, point.Z);
    const rayOrigin = testPoint.clone();
    const rayDirection = new THREE.Vector3(1, 0, 0);

    let intersectionCount = 0;

    meshData.FaceIndices.forEach(face => {
      if (face.length >= 3) {
        for (let i = 1; i < face.length - 1; i++) {
          const v0 = meshData.Vertices3D[face[0]];
          const v1 = meshData.Vertices3D[face[i]];
          const v2 = meshData.Vertices3D[face[i + 1]];

          if (v0 && v1 && v2) {
            const p0 = new THREE.Vector3(v0.X, v0.Y, v0.Z);
            const p1 = new THREE.Vector3(v1.X, v1.Y, v1.Z);
            const p2 = new THREE.Vector3(v2.X, v2.Y, v2.Z);

            const triangle = new THREE.Triangle(p0, p1, p2);
            const plane = new THREE.Plane();
            triangle.getPlane(plane);
            
            const intersection = new THREE.Vector3();
            const ray = new THREE.Ray(rayOrigin, rayDirection);
            const result = ray.intersectPlane(plane, intersection);
            
            if (result && triangle.containsPoint(intersection)) {
              intersectionCount++;
            }
          }
        }
      }
    });

    return intersectionCount % 2 === 1;
  }

  public static getClosestPoint(meshData: MeshData, point: Point3DData): Point3DData | null {
    if (!meshData.Vertices3D || meshData.Vertices3D.length === 0) {
      return null;
    }

    let minDistance = Infinity;
    let closestPoint: Point3DData | null = null;

    meshData.FaceIndices.forEach(face => {
      if (face.length >= 3) {
        for (let i = 1; i < face.length - 1; i++) {
          const v0 = meshData.Vertices3D[face[0]];
          const v1 = meshData.Vertices3D[face[i]];
          const v2 = meshData.Vertices3D[face[i + 1]];

          if (v0 && v1 && v2) {
            const p0 = new THREE.Vector3(v0.X, v0.Y, v0.Z);
            const p1 = new THREE.Vector3(v1.X, v1.Y, v1.Z);
            const p2 = new THREE.Vector3(v2.X, v2.Y, v2.Z);

            const triangle = new THREE.Triangle(p0, p1, p2);
            const testPoint = new THREE.Vector3(point.X, point.Y, point.Z);
            const closest = new THREE.Vector3();
            triangle.closestPointToPoint(testPoint, closest);
            
            const distance = testPoint.distanceTo(closest);

            if (distance < minDistance) {
              minDistance = distance;
              closestPoint = { X: closest.x, Y: closest.y, Z: closest.z };
            }
          }
        }
      }
    });

    return closestPoint;
  }

  public static getDistanceToPoint(meshData: MeshData, point: Point3DData): number {
    const closestPoint = this.getClosestPoint(meshData, point);
    if (!closestPoint) {
      return Infinity;
    }

    const testPoint = new THREE.Vector3(point.X, point.Y, point.Z);
    const closest = new THREE.Vector3(closestPoint.X, closestPoint.Y, closestPoint.Z);

    return testPoint.distanceTo(closest);
  }

  public static clone(meshData: MeshData, newHandle: string): MeshData {
    const cloned = JSON.parse(JSON.stringify(meshData));
    cloned.Handle = newHandle;
    return cloned;
  }

  public static transform(meshData: MeshData, matrix: THREE.Matrix4): MeshData {
    const transformed = JSON.parse(JSON.stringify(meshData));

    if (meshData.Vertices3D) {
      transformed.Vertices3D = meshData.Vertices3D.map(v => {
        const vertex = new THREE.Vector3(v.X, v.Y, v.Z);
        vertex.applyMatrix4(matrix);
        return { X: vertex.x, Y: vertex.y, Z: vertex.z };
      });
    }

    if (meshData.Centroid3D) {
      const centroid = new THREE.Vector3(meshData.Centroid3D.X, meshData.Centroid3D.Y, meshData.Centroid3D.Z);
      centroid.applyMatrix4(matrix);
      transformed.Centroid3D = { X: centroid.x, Y: centroid.y, Z: centroid.z };
    }

    if (meshData.Normals) {
      const normalMatrix = new THREE.Matrix3().getNormalMatrix(matrix);
      transformed.Normals = meshData.Normals.map(n => {
        const normal = new THREE.Vector3(n.X, n.Y, n.Z);
        normal.applyMatrix3(normalMatrix).normalize();
        return { X: normal.x, Y: normal.y, Z: normal.z };
      });
    }

    if (meshData.Bounds3D) {
      const min = new THREE.Vector3(meshData.Bounds3D.Min.X, meshData.Bounds3D.Min.Y, meshData.Bounds3D.Min.Z);
      const max = new THREE.Vector3(meshData.Bounds3D.Max.X, meshData.Bounds3D.Max.Y, meshData.Bounds3D.Max.Z);
      min.applyMatrix4(matrix);
      max.applyMatrix4(matrix);
      transformed.Bounds3D = {
        Min: { X: Math.min(min.x, max.x), Y: Math.min(min.y, max.y), Z: Math.min(min.z, max.z) },
        Max: { X: Math.max(min.x, max.x), Y: Math.max(min.y, max.y), Z: Math.max(min.z, max.z) },
        Center: { X: (min.x + max.x) / 2, Y: (min.y + max.y) / 2, Z: (min.z + max.z) / 2 },
        Size: { X: Math.abs(max.x - min.x), Y: Math.abs(max.y - min.y), Z: Math.abs(max.z - min.z) }
      };
    }

    return transformed;
  }

  public static getSubdivisionLevel(meshData: MeshData): number {
    return meshData.SubdivisionLevel || 0;
  }

  public static setSubdivisionLevel(meshData: MeshData, scene: THREE.Scene, level: number): boolean {
    const group = this.getMeshGroup(meshData, scene);
    if (!group) {
      return false;
    }

    meshData.SubdivisionLevel = level;
    this.dispose(meshData, scene);
    const result = this.render(meshData, scene);
    return result !== null;
  }

  public static getMaterialType(meshData: MeshData): string {
    return meshData.MaterialType || this.DEFAULT_MATERIAL;
  }

  public static setMaterialType(meshData: MeshData, scene: THREE.Scene, materialType: string): boolean {
    const group = this.getMeshGroup(meshData, scene);
    if (!group) {
      return false;
    }

    meshData.MaterialType = materialType;
    this.dispose(meshData, scene);
    const result = this.render(meshData, scene);
    return result !== null;
  }
}
