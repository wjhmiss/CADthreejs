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

export interface PointMaterialData {
  Color: ColorData;
  Opacity: number;
  Transparent: boolean;
  Type: string;
  DepthTest: boolean;
  SizeAttenuation: boolean;
  Size: number;
}

export interface PointGeometryData {
  Position: Point3DData;
  Rotation: Point3DData;
  Scale: Point3DData;
  Normal: NormalData;
  BoundingBox: BoundsData3D;
  Type: string;
  Size: number;
}

export interface PointData {
  Location: Point3DData;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  Thickness: number;
  Rotation: number;
  Size: number;

  Bounds3D: BoundsData3D;
  Centroid3D: Point3DData;

  Color: ColorData;
  Normal: NormalData;
  Transform: TransformData;
  EntityType: string;
  Visible: boolean;
  LayerIndex: number;
  LayerName: string;

  Type: string;
  Handle: string;
  CoordinateSystem: string;
  Opacity: number;
  Transparent: boolean;
  DepthTest: boolean;
  Material: PointMaterialData;
  Geometry: PointGeometryData;
}

export class PointEntityThreejsRenderer {
  private static readonly DEFAULT_POINT_SIZE = 6.0;
  private static readonly DEFAULT_OPACITY = 1.0;
  private static readonly DEFAULT_COLOR = '#000000';
  private static readonly pointCache = new Map<string, THREE.Points>();

  public static render(pointData: PointData, scene: THREE.Scene): THREE.Points | null {
    if (!pointData || !pointData.Visible) {
      return null;
    }

    const geometry = this.createPointGeometry(pointData);
    const material = this.createPointMaterial(pointData);
    const points = new THREE.Points(geometry, material);
    points.name = `Point_${pointData.Handle}`;
    points.userData = {
      type: pointData.EntityType,
      handle: pointData.Handle,
      layerName: pointData.LayerName,
      layerIndex: pointData.LayerIndex,
      entityType: pointData.EntityType,
      colorIndex: pointData.ColorIndex,
      lineTypeName: pointData.LineTypeName,
      lineWeight: pointData.LineWeight,
      thickness: pointData.Thickness,
      rotation: pointData.Rotation,
      size: pointData.Size,
      bounds3D: pointData.Bounds3D,
      centroid3D: pointData.Centroid3D,
      coordinateSystem: pointData.CoordinateSystem,
      opacity: pointData.Opacity,
      transparent: pointData.Transparent,
      depthTest: pointData.DepthTest,
      transform: pointData.Transform
    };
    points.visible = pointData.Visible;

    this.pointCache.set(pointData.Handle, points);

    return points;
  }

  public static update(pointData: PointData, scene: THREE.Scene): boolean {
    const existingPoints = this.pointCache.get(pointData.Handle);
    if (!existingPoints) {
      return false;
    }

    const material = existingPoints.material as THREE.PointsMaterial;
    if (material) {
      material.color.setHex(parseInt(pointData.Color.Hex.replace('#', ''), 16));
      material.opacity = pointData.Opacity;
      material.transparent = pointData.Transparent;
      material.size = pointData.Material.Size;
      material.sizeAttenuation = pointData.Material.SizeAttenuation;
      material.depthTest = pointData.DepthTest;
    }

    const geometry = existingPoints.geometry as THREE.BufferGeometry;
    if (geometry) {
      const positions = geometry.attributes.position.array as Float32Array;
      positions[0] = pointData.Location.X;
      positions[1] = pointData.Location.Y;
      positions[2] = pointData.Location.Z;
      geometry.attributes.position.needsUpdate = true;
    }

    return true;
  }

  public static dispose(pointData: PointData, scene: THREE.Scene): boolean {
    const existingPoints = this.pointCache.get(pointData.Handle);
    if (!existingPoints) {
      return false;
    }

    scene.remove(existingPoints);

    existingPoints.geometry.dispose();
    (existingPoints.material as THREE.Material).dispose();
    this.pointCache.delete(pointData.Handle);

    return true;
  }

  public static setVisibility(pointData: PointData, scene: THREE.Scene, visible: boolean): boolean {
    const existingPoints = this.pointCache.get(pointData.Handle);
    if (!existingPoints) {
      return false;
    }

    existingPoints.visible = visible;
    return true;
  }

  private static findScene(object: THREE.Object3D): THREE.Scene | null {
    let parent = object.parent;
    while (parent) {
      if (parent instanceof THREE.Scene) {
        return parent;
      }
      parent = parent.parent;
    }
    return null;
  }

  public static setOpacity(pointData: PointData, opacity: number): boolean {
    const existingPoints = this.pointCache.get(pointData.Handle);
    if (!existingPoints) {
      return false;
    }

    const material = existingPoints.material as THREE.PointsMaterial;
    if (material) {
      material.opacity = opacity;
      material.transparent = opacity < 1.0;
    }

    return true;
  }

  public static setSize(pointData: PointData, size: number): boolean {
    const existingPoints = this.pointCache.get(pointData.Handle);
    if (!existingPoints) {
      return false;
    }

    const material = existingPoints.material as THREE.PointsMaterial;
    if (material) {
      material.size = size;
    }

    return true;
  }

  public static getPointFromCache(pointData: PointData): THREE.Points | null {
    return this.pointCache.get(pointData.Handle) || null;
  }

  public static getLocation(pointData: PointData): THREE.Vector3 {
    return new THREE.Vector3(pointData.Location.X, pointData.Location.Y, pointData.Location.Z);
  }

  public static getSize(pointData: PointData): number {
    const size = pointData.Size || this.DEFAULT_POINT_SIZE;
    return size === 0 ? this.DEFAULT_POINT_SIZE : size;
  }

  public static getRotation(pointData: PointData): number {
    return pointData.Rotation || 0;
  }

  public static getColor(pointData: PointData): THREE.Color {
    return new THREE.Color(pointData.Color.Hex);
  }

  public static getBounds(pointData: PointData): THREE.Box3 {
    const min = new THREE.Vector3(pointData.Bounds3D.Min.X, pointData.Bounds3D.Min.Y, pointData.Bounds3D.Min.Z);
    const max = new THREE.Vector3(pointData.Bounds3D.Max.X, pointData.Bounds3D.Max.Y, pointData.Bounds3D.Max.Z);
    return new THREE.Box3(min, max);
  }

  public static clearCache(): void {
    this.pointCache.forEach((points) => {
      points.geometry.dispose();
      (points.material as THREE.Material).dispose();
    });
    this.pointCache.clear();
  }

  private static createPointGeometry(pointData: PointData): THREE.BufferGeometry {
    const geometry = new THREE.BufferGeometry();
    const positions = new Float32Array([pointData.Location.X, pointData.Location.Y, pointData.Location.Z]);
    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    geometry.computeBoundingSphere();
    return geometry;
  }

  private static createPointMaterial(pointData: PointData): THREE.PointsMaterial {
    const material = new THREE.PointsMaterial({
      color: pointData.Color.Hex,
      size: pointData.Material.Size,
      sizeAttenuation: pointData.Material.SizeAttenuation,
      opacity: pointData.Opacity,
      transparent: pointData.Transparent,
      depthTest: pointData.DepthTest,
      depthWrite: false
    });

    return material;
  }

  public static renderMultiple(pointDataArray: PointData[], scene: THREE.Scene): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];

    const geometry = new THREE.BufferGeometry();
    const positions: number[] = [];
    const colors: number[] = [];

    pointDataArray.forEach((pointData) => {
      positions.push(pointData.Location.X, pointData.Location.Y, pointData.Location.Z);
      const color = new THREE.Color(pointData.Color.Hex);
      colors.push(color.r, color.g, color.b);
    });

    geometry.setAttribute('position', new THREE.Float32BufferAttribute(positions, 3));
    geometry.setAttribute('color', new THREE.Float32BufferAttribute(colors, 3));
    geometry.computeBoundingSphere();

    const material = new THREE.PointsMaterial({
      size: this.DEFAULT_POINT_SIZE,
      sizeAttenuation: true,
      vertexColors: true,
      transparent: true,
      opacity: this.DEFAULT_OPACITY,
      depthTest: true,
      depthWrite: false
    });

    const points = new THREE.Points(geometry, material);
    points.name = 'MultiplePoints';
    points.userData = {
      objectType: 'MultiplePoints',
      pointCount: pointDataArray.length
    };
    objects.push(points);

    return objects;
  }

  public static disposeMultiple(objects: THREE.Object3D[], scene: THREE.Scene): void {
    if (!objects || objects.length === 0) {
      return;
    }

    objects.forEach((obj) => {
      scene.remove(obj);
      if (obj instanceof THREE.Points) {
        obj.geometry.dispose();
        (obj.material as THREE.Material).dispose();
      }
    });
  }

  public static getNormal(pointData: PointData): NormalData {
    if (!pointData.Normal || (pointData.Normal.X === 0 && pointData.Normal.Y === 0 && pointData.Normal.Z === 0)) {
      return { X: 0, Y: 0, Z: 1 };
    }
    return pointData.Normal;
  }

  public static getTransform(pointData: PointData): any {
    if (!pointData.Transform) {
      return {
        ScaleX: 1.0,
        ScaleY: 1.0,
        ScaleZ: 1.0,
        Rotation: 0,
        TranslateX: pointData.Location.X,
        TranslateY: pointData.Location.Y,
        TranslateZ: pointData.Location.Z
      };
    }
    return {
      ScaleX: pointData.Transform.Scale.X,
      ScaleY: pointData.Transform.Scale.Y,
      ScaleZ: pointData.Transform.Scale.Z,
      Rotation: pointData.Transform.Rotation.Z,
      TranslateX: pointData.Transform.Position.X,
      TranslateY: pointData.Transform.Position.Y,
      TranslateZ: pointData.Transform.Position.Z
    };
  }

  public static getBounds3D(pointData: PointData): BoundsData3D {
    if (!pointData.Bounds3D) {
      const halfSize = (pointData.Size || this.DEFAULT_POINT_SIZE) / 2;
      return {
        Min: { X: pointData.Location.X - halfSize, Y: pointData.Location.Y - halfSize, Z: pointData.Location.Z - halfSize },
        Max: { X: pointData.Location.X + halfSize, Y: pointData.Location.Y + halfSize, Z: pointData.Location.Z + halfSize }
      };
    }
    return pointData.Bounds3D;
  }

  public static getCentroid3D(pointData: PointData): Point3DData {
    if (!pointData.Centroid3D) {
      return pointData.Location;
    }
    return pointData.Centroid3D;
  }

  public static getMaterial(pointData: PointData): PointMaterialData {
    if (!pointData.Material) {
      return {
        Type: 'PointsMaterial',
        Color: pointData.Color,
        Opacity: pointData.Opacity || this.DEFAULT_OPACITY,
        Transparent: pointData.Transparent || false,
        DepthTest: pointData.DepthTest !== false,
        SizeAttenuation: true,
        Size: pointData.Size || this.DEFAULT_POINT_SIZE
      };
    }
    return pointData.Material;
  }

  public static getGeometry(pointData: PointData): PointGeometryData {
    if (!pointData.Geometry) {
      const halfSize = (pointData.Size || this.DEFAULT_POINT_SIZE) / 2;
      return {
        Type: 'BufferGeometry',
        Position: pointData.Location,
        Rotation: { X: 0, Y: 0, Z: pointData.Rotation || 0 },
        Scale: { X: 1.0, Y: 1.0, Z: 1.0 },
        Normal: pointData.Normal || { X: 0, Y: 0, Z: 1 },
        BoundingBox: this.getBounds3D(pointData),
        Size: pointData.Size || this.DEFAULT_POINT_SIZE
      };
    }
    return pointData.Geometry;
  }

  public static getDepthTest(pointData: PointData): boolean {
    return pointData.DepthTest !== false;
  }

  public static getHandle(pointData: PointData): string {
    return pointData.Handle || '';
  }

  public static getLayerName(pointData: PointData): string {
    return pointData.LayerName || '';
  }

  public static getVisible(pointData: PointData): boolean {
    return pointData.Visible !== false;
  }

  public static getOpacity(pointData: PointData): number {
    return pointData.Opacity !== undefined ? pointData.Opacity : this.DEFAULT_OPACITY;
  }

  public static getTransparent(pointData: PointData): boolean {
    return pointData.Transparent !== false;
  }
}
