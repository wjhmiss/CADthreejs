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

export interface GeometryData {
  Type: string;
  VertexCount: number;
  HasColors: boolean;
  HasNormals: boolean;
  HasIndices: boolean;
  PrimitiveType: string;
  IndexCount: number;
}

export interface MaterialData {
  Type: string;
  Color: number;
  Opacity: number;
  Transparent: boolean;
  LineWidth: number;
  VertexColors: boolean;
}

export interface RayData {
  StartPoint: Point3DData;
  Direction: Point3DData;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  
  EndPoint: Point3DData;
  Length: number;
  Angle: number;
  
  Bounds: BoundsData;
  Centroid: Point3DData;
  
  Transform: TransformData;
  Geometry: GeometryData;
  Material: MaterialData;
  Color: ColorData;
  VertexPositions: number[];
  VertexNormals: number[];
  VertexColors: number[];
  Indices: number[];
  
  EntityType: string;
  Visible: boolean;
  LayerName: string;
  LayerIndex: number;
  Handle: string;
  
  Transparency: number;
  MaterialName: string;
  
  CastShadows: boolean;
  ReceiveShadows: boolean;
}

export class RayEntityThreejsRenderer {
  private static readonly rayCache: Map<string, THREE.Group> = new Map();
  private static readonly DEFAULT_RAY_LENGTH = 1000.0;
  private static readonly DEFAULT_LINE_WIDTH = 1.0;

  public static render(rayData: RayData, scene: THREE.Scene): THREE.Group {
    const group = new THREE.Group();
    group.name = `Ray_${rayData.Handle}`;
    group.visible = rayData.Visible;

    if (!rayData.StartPoint || !rayData.Direction) {
      console.warn(`Ray ${rayData.Handle} has invalid start point or direction`);
      return group;
    }

    const geometry = this.createRayGeometry(rayData);
    const material = this.createRayMaterial(rayData);
    const line = new THREE.Line(geometry, material);
    line.name = 'Ray';
    line.userData = { handle: rayData.Handle };
    line.visible = rayData.Visible;

    this.applyTransform(line, rayData.Transform);

    group.add(line);

    this.rayCache.set(rayData.Handle, group);

    if (rayData.Visible) {
      scene.add(group);
    }

    return group;
  }

  public static update(rayData: RayData, scene: THREE.Scene): THREE.Group {
    this.dispose(rayData.Handle);
    return this.render(rayData, scene);
  }

  public static dispose(handle: string): void {
    const cachedGroup = this.rayCache.get(handle);
    if (cachedGroup) {
      cachedGroup.traverse((child) => {
        if (child instanceof THREE.Line) {
          if (child.geometry) {
            child.geometry.dispose();
          }
          if (child.material) {
            child.material.dispose();
          }
        }
      });
      this.rayCache.delete(handle);
    }
  }

  public static disposeFromScene(handle: string, scene: THREE.Scene): void {
    const cachedGroup = this.rayCache.get(handle);
    if (cachedGroup) {
      scene.remove(cachedGroup);
      this.dispose(handle);
    }
  }

  public static disposeAll(): void {
    this.rayCache.forEach((group, handle) => {
      this.dispose(handle);
    });
    this.rayCache.clear();
  }

  public static disposeAllFromScene(scene: THREE.Scene): void {
    this.rayCache.forEach((group) => {
      scene.remove(group);
    });
    this.disposeAll();
  }

  private static createRayGeometry(rayData: RayData): THREE.BufferGeometry {
    const geometry = new THREE.BufferGeometry();

    const positions = new Float32Array(rayData.VertexPositions);
    const colors = new Float32Array(rayData.VertexColors);
    const normals = new Float32Array(rayData.VertexNormals);
    const indices = new Uint16Array(rayData.Indices);

    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3));
    geometry.setAttribute('normal', new THREE.BufferAttribute(normals, 3));
    geometry.setIndex(new THREE.BufferAttribute(indices, 1));

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static createRayMaterial(rayData: RayData): THREE.Material {
    const material = new THREE.LineBasicMaterial({
      color: rayData.Material.Color,
      transparent: rayData.Material.Transparent,
      opacity: rayData.Material.Opacity,
      linewidth: rayData.Material.LineWidth,
      vertexColors: rayData.Material.VertexColors,
      depthTest: true,
      depthWrite: false
    });

    return material;
  }

  private static applyTransform(line: THREE.Line, transform: TransformData): void {
    line.position.set(transform.Position.X, transform.Position.Y, transform.Position.Z);
    line.rotation.set(transform.Rotation.X, transform.Rotation.Y, transform.Rotation.Z);
    line.scale.set(transform.Scale.X, transform.Scale.Y, transform.Scale.Z);

    if (transform.Matrix && transform.Matrix.length === 16) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(transform.Matrix);
      line.applyMatrix4(matrix);
    }
  }

  public static getRayByHandle(handle: string): THREE.Group | null {
    return this.rayCache.get(handle) || null;
  }

  public static getRayCount(): number {
    return this.rayCache.size;
  }

  public static setVisibility(handle: string, visible: boolean): void {
    const group = this.rayCache.get(handle);
    if (group) {
      group.visible = visible;
      group.traverse((child) => {
        if (child instanceof THREE.Line) {
          child.visible = visible;
        }
      });
    }
  }

  public static setOpacity(handle: string, opacity: number): void {
    const group = this.rayCache.get(handle);
    if (group) {
      group.traverse((child) => {
        if (child instanceof THREE.Line && child.material instanceof THREE.LineBasicMaterial) {
          child.material.opacity = opacity;
          child.material.transparent = opacity < 1.0;
          child.material.needsUpdate = true;
        }
      });
    }
  }

  public static setColor(handle: string, color: number): void {
    const group = this.rayCache.get(handle);
    if (group) {
      group.traverse((child) => {
        if (child instanceof THREE.Line && child.material instanceof THREE.LineBasicMaterial) {
          child.material.color.setHex(color);
          child.material.needsUpdate = true;
        }
      });
    }
  }

  public static setLineWidth(handle: string, lineWidth: number): void {
    const group = this.rayCache.get(handle);
    if (group) {
      group.traverse((child) => {
        if (child instanceof THREE.Line && child.material instanceof THREE.LineBasicMaterial) {
          child.material.linewidth = lineWidth;
          child.material.needsUpdate = true;
        }
      });
    }
  }

  public static getBounds(handle: string): THREE.Box3 | null {
    const group = this.rayCache.get(handle);
    if (group) {
      const box = new THREE.Box3();
      group.traverse((child) => {
        if (child instanceof THREE.Line) {
          box.expandByObject(child);
        }
      });
      return box;
    }
    return null;
  }

  public static getCentroid(handle: string): THREE.Vector3 | null {
    const group = this.rayCache.get(handle);
    if (group) {
      const centroid = new THREE.Vector3();
      group.traverse((child) => {
        if (child instanceof THREE.Line && child.geometry) {
          const positions = child.geometry.attributes.position;
          if (positions && positions.count >= 2) {
            const v0 = new THREE.Vector3(positions.getX(0), positions.getY(0), positions.getZ(0));
            const v1 = new THREE.Vector3(positions.getX(1), positions.getY(1), positions.getZ(1));
            centroid.add(v0).add(v1).multiplyScalar(0.5);
          }
        }
      });
      return centroid;
    }
    return null;
  }

  public static getLength(handle: string): number {
    const group = this.rayCache.get(handle);
    if (group) {
      let length = 0;
      group.traverse((child) => {
        if (child instanceof THREE.Line && child.geometry) {
          const positions = child.geometry.attributes.position;
          if (positions && positions.count >= 2) {
            const v0 = new THREE.Vector3(positions.getX(0), positions.getY(0), positions.getZ(0));
            const v1 = new THREE.Vector3(positions.getX(1), positions.getY(1), positions.getZ(1));
            length += v0.distanceTo(v1);
          }
        }
      });
      return length;
    }
    return 0;
  }

  public static getAngle(handle: string): number {
    const group = this.rayCache.get(handle);
    if (group) {
      let angle = 0;
      group.traverse((child) => {
        if (child instanceof THREE.Line && child.geometry) {
          const positions = child.geometry.attributes.position;
          if (positions && positions.count >= 2) {
            const v0 = new THREE.Vector3(positions.getX(0), positions.getY(0), positions.getZ(0));
            const v1 = new THREE.Vector3(positions.getX(1), positions.getY(1), positions.getZ(1));
            const direction = new THREE.Vector3().subVectors(v1, v0).normalize();
            angle = Math.atan2(direction.y, direction.x);
          }
        }
      });
      return angle;
    }
    return 0;
  }

  public static getStartPoint(handle: string): THREE.Vector3 | null {
    const group = this.rayCache.get(handle);
    if (group) {
      let startPoint: THREE.Vector3 | null = null;
      group.traverse((child) => {
        if (child instanceof THREE.Line && child.geometry && !startPoint) {
          const positions = child.geometry.attributes.position;
          if (positions && positions.count >= 1) {
            startPoint = new THREE.Vector3(
              positions.getX(0),
              positions.getY(0),
              positions.getZ(0)
            );
          }
        }
      });
      return startPoint;
    }
    return null;
  }

  public static getEndPoint(handle: string): THREE.Vector3 | null {
    const group = this.rayCache.get(handle);
    if (group) {
      let endPoint: THREE.Vector3 | null = null;
      group.traverse((child) => {
        if (child instanceof THREE.Line && child.geometry && !endPoint) {
          const positions = child.geometry.attributes.position;
          if (positions && positions.count >= 2) {
            endPoint = new THREE.Vector3(
              positions.getX(1),
              positions.getY(1),
              positions.getZ(1)
            );
          }
        }
      });
      return endPoint;
    }
    return null;
  }

  public static getDirection(handle: string): THREE.Vector3 | null {
    const startPoint = this.getStartPoint(handle);
    const endPoint = this.getEndPoint(handle);
    
    if (startPoint && endPoint) {
      return new THREE.Vector3().subVectors(endPoint, startPoint).normalize();
    }
    return null;
  }

  public static isRayVisible(handle: string): boolean {
    const group = this.rayCache.get(handle);
    return group ? group.visible : false;
  }

  public static getPointAtDistance(handle: string, distance: number): THREE.Vector3 | null {
    const startPoint = this.getStartPoint(handle);
    const direction = this.getDirection(handle);
    
    if (startPoint && direction) {
      const point = startPoint.clone();
      point.add(direction.clone().multiplyScalar(distance));
      return point;
    }
    return null;
  }

  public static getDistanceToPoint(handle: string, point: THREE.Vector3): number | null {
    const startPoint = this.getStartPoint(handle);
    const direction = this.getDirection(handle);
    
    if (startPoint && direction) {
      const toPoint = new THREE.Vector3().subVectors(point, startPoint);
      const projection = toPoint.dot(direction);
      const closestPoint = startPoint.clone().add(direction.clone().multiplyScalar(projection));
      return closestPoint.distanceTo(point);
    }
    return null;
  }

  public static intersectsRay(handle: string, otherRay: THREE.Ray): boolean {
    const startPoint = this.getStartPoint(handle);
    const direction = this.getDirection(handle);
    
    if (startPoint && direction) {
      const ray = new THREE.Ray(startPoint, direction);
      
      const v1 = ray.direction.clone();
      const v2 = otherRay.direction.clone();
      const w = new THREE.Vector3().subVectors(ray.origin, otherRay.origin);
      
      const a = v1.dot(v1);
      const b = v1.dot(v2);
      const c = v2.dot(v2);
      const d = v1.dot(w);
      const e = v2.dot(w);
      
      const denominator = a * c - b * b;
      
      if (Math.abs(denominator) < 0.0001) {
        return false;
      }
      
      const sc = (b * e - c * d) / denominator;
      const tc = (a * e - b * d) / denominator;
      
      if (sc >= 0 && tc >= 0) {
        const p1 = ray.origin.clone().add(v1.clone().multiplyScalar(sc));
        const p2 = otherRay.origin.clone().add(v2.clone().multiplyScalar(tc));
        return p1.distanceTo(p2) < 0.0001;
      }
      
      return false;
    }
    return false;
  }

  public static getRayData(handle: string): RayData | null {
    const group = this.rayCache.get(handle);
    if (group) {
      const startPoint = this.getStartPoint(handle);
      const endPoint = this.getEndPoint(handle);
      const direction = this.getDirection(handle);
      const length = this.getLength(handle);
      const angle = this.getAngle(handle);
      const bounds = this.getBounds(handle);
      const centroid = this.getCentroid(handle);
      
      if (startPoint && endPoint && direction && bounds && centroid) {
        return {
          StartPoint: { X: startPoint.x, Y: startPoint.y, Z: startPoint.z },
          Direction: { X: direction.x, Y: direction.y, Z: direction.z },
          EndPoint: { X: endPoint.x, Y: endPoint.y, Z: endPoint.z },
          Length: length,
          Angle: angle,
          Bounds: {
            Min: { X: bounds.min.x, Y: bounds.min.y, Z: bounds.min.z },
            Max: { X: bounds.max.x, Y: bounds.max.y, Z: bounds.max.z }
          },
          Centroid: { X: centroid.x, Y: centroid.y, Z: centroid.z },
          ColorIndex: 0,
          LineTypeName: '',
          LineWeight: 1.0,
          Transform: { Position: { X: 0, Y: 0, Z: 0 }, Rotation: { X: 0, Y: 0, Z: 0 }, Scale: { X: 1, Y: 1, Z: 1 }, Matrix: [] },
          Geometry: { Type: 'BufferGeometry', VertexCount: 2, HasColors: false, HasNormals: false, HasIndices: true, PrimitiveType: 'LinePieces', IndexCount: 2 },
          Material: { Type: 'LineBasicMaterial', Color: 0xffffff, Opacity: 1.0, Transparent: false, LineWidth: 1.0, VertexColors: false },
          Color: { Index: 0, Hex: '#000000', R: 0, G: 0, B: 0, A: 1 },
          VertexPositions: [],
          VertexNormals: [],
          VertexColors: [],
          Indices: [],
          EntityType: 'Ray',
          Visible: true,
          LayerName: '',
          LayerIndex: 0,
          Handle: handle,
          Transparency: 0,
          MaterialName: '',
          CastShadows: false,
          ReceiveShadows: false
        };
      }
    }
    return null;
  }
}
