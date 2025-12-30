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

export interface MaterialData {
  Color: ColorData;
  Opacity: number;
  Transparent: boolean;
  Type: string;
  DepthTest: boolean;
  Side: boolean;
  Wireframe: boolean;
}

export interface GeometryData {
  Type: string;
  VertexCount: number;
  IndexCount: number;
  Bounds: BoundsData;
}

export interface ShapeData {
  InsertionPoint: Point3DData;
  Size: number;
  Rotation: number;
  RelativeXScale: number;
  ObliqueAngle: number;
  Normal: Point3DData;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  LineTypeScale: number;
  ShapeStyleName: string;
  ShapeIndex: number;

  BoundaryPoints: Point3DData[];
  Bounds: BoundsData;
  Centroid: Point3DData;
  Width: number;
  Height: number;
  BoundaryPointCount: number;
  Thickness: number;

  Transform: TransformData;
  Geometry: GeometryData;
  Material: MaterialData;
  Color: ColorData;
  VertexPositions: number[];
  VertexNormals: number[];
  VertexColors: number[];
  Indices: number[];

  Type: string;
  Handle: string;
  LayerName: string;
  LayerIndex: number;
  Visible: boolean;
  CoordinateSystem: string;
  Opacity: number;
  Transparent: boolean;
  DepthTest: boolean;
}

export class ShapeEntityThreejsRenderer {
  private static readonly DEFAULT_SIZE = 1.0;
  private static readonly DEFAULT_ROTATION = 0.0;
  private static readonly DEFAULT_RELATIVE_X_SCALE = 1.0;
  private static readonly DEFAULT_THICKNESS = 0.0;
  private static readonly DEFAULT_OPACITY = 1.0;
  private static readonly DEFAULT_COLOR = '#FFFFFF';

  private static shapeCache = new Map<string, THREE.Object3D[]>();

  public static render(shapeData: ShapeData, scene: THREE.Scene): THREE.Object3D[] | null {
    if (!shapeData || !shapeData.Visible) {
      return null;
    }

    const objects: THREE.Object3D[] = [];

    const shapeMesh = this.renderShape(shapeData);
    if (shapeMesh) {
      shapeMesh.name = `Shape_${shapeData.Handle}`;
      shapeMesh.userData = {
        type: shapeData.Type,
        handle: shapeData.Handle,
        layerName: shapeData.LayerName,
        layerIndex: shapeData.LayerIndex,
        coordinateSystem: shapeData.CoordinateSystem,
        insertionPoint: shapeData.InsertionPoint,
        size: shapeData.Size,
        rotation: shapeData.Rotation,
        relativeXScale: shapeData.RelativeXScale,
        obliqueAngle: shapeData.ObliqueAngle,
        normal: shapeData.Normal,
        colorIndex: shapeData.ColorIndex,
        lineTypeName: shapeData.LineTypeName,
        lineWeight: shapeData.LineWeight,
        lineTypeScale: shapeData.LineTypeScale,
        shapeStyleName: shapeData.ShapeStyleName,
        shapeIndex: shapeData.ShapeIndex,
        boundaryPoints: shapeData.BoundaryPoints,
        bounds: shapeData.Bounds,
        centroid: shapeData.Centroid,
        width: shapeData.Width,
        height: shapeData.Height,
        boundaryPointCount: shapeData.BoundaryPointCount,
        thickness: shapeData.Thickness,
        transform: shapeData.Transform,
        geometry: shapeData.Geometry,
        material: shapeData.Material,
        color: shapeData.Color,
        vertexPositions: shapeData.VertexPositions,
        vertexNormals: shapeData.VertexNormals,
        vertexColors: shapeData.VertexColors,
        indices: shapeData.Indices,
        opacity: shapeData.Opacity,
        transparent: shapeData.Transparent,
        depthTest: shapeData.DepthTest
      };
      objects.push(shapeMesh);
    }

    const boundaryLine = this.renderBoundary(shapeData);
    if (boundaryLine) {
      boundaryLine.name = `ShapeBoundary_${shapeData.Handle}`;
      boundaryLine.userData = {
        type: 'Boundary',
        handle: shapeData.Handle,
        boundaryPointCount: shapeData.BoundaryPoints.length
      };
      objects.push(boundaryLine);
    }

    const boundsLine = this.renderBounds(shapeData);
    if (boundsLine) {
      boundsLine.name = `ShapeBounds_${shapeData.Handle}`;
      boundsLine.userData = {
        type: 'Bounds',
        handle: shapeData.Handle,
        size: shapeData.Bounds?.Size
      };
      objects.push(boundsLine);
    }

    if (shapeData.Transform && shapeData.Transform.Matrix) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(shapeData.Transform.Matrix);
      objects.forEach(obj => obj.applyMatrix4(matrix));
    }

    this.shapeCache.set(shapeData.Handle, objects);

    return objects;
  }

  private static renderShape(shapeData: ShapeData): THREE.Mesh | null {
    if (!shapeData.VertexPositions || shapeData.VertexPositions.length === 0) {
      console.warn(`Shape ${shapeData.Handle} has no vertex positions`);
      return null;
    }

    const geometry = this.createGeometry(shapeData);
    if (!geometry) {
      return null;
    }

    const material = this.createMaterial(shapeData);

    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = 'Shape';
    mesh.userData = {
      type: 'Shape',
      handle: shapeData.Handle,
      shapeIndex: shapeData.ShapeIndex,
      shapeStyleName: shapeData.ShapeStyleName
    };

    if (shapeData.Material && shapeData.Material.Side) {
      mesh.material.side = THREE.DoubleSide;
    }

    return mesh;
  }

  private static createGeometry(shapeData: ShapeData): THREE.BufferGeometry | null {
    const vertices = shapeData.VertexPositions || [];
    const indices = shapeData.Indices || [];
    const normals = shapeData.VertexNormals || [];
    const colors = shapeData.VertexColors || [];

    if (vertices.length === 0) {
      return null;
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));

    if (normals.length > 0) {
      geometry.setAttribute('normal', new THREE.Float32BufferAttribute(normals, 3));
    } else {
      geometry.computeVertexNormals();
    }

    if (colors.length > 0) {
      geometry.setAttribute('color', new THREE.Float32BufferAttribute(colors, 3));
    }

    if (indices.length > 0) {
      geometry.setIndex(indices);
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static createMaterial(shapeData: ShapeData): THREE.Material {
    const materialData = shapeData.Material;
    const color = materialData?.Color?.Hex || shapeData.Color?.Hex || this.DEFAULT_COLOR;
    const opacity = materialData?.Opacity ?? shapeData.Opacity ?? this.DEFAULT_OPACITY;
    const transparent = materialData?.Transparent ?? shapeData.Transparent ?? (opacity < 1.0);
    const wireframe = materialData?.Wireframe ?? false;

    const material = new THREE.MeshBasicMaterial({
      color: color,
      transparent: transparent,
      opacity: opacity,
      wireframe: wireframe,
      depthTest: materialData?.DepthTest ?? shapeData.DepthTest ?? true,
      depthWrite: false,
      side: materialData?.Side ? THREE.DoubleSide : THREE.FrontSide
    });

    return material;
  }

  private static renderBoundary(shapeData: ShapeData): THREE.Line | null {
    if (!shapeData.BoundaryPoints || shapeData.BoundaryPoints.length === 0) {
      return null;
    }

    const vertices: number[] = [];
    shapeData.BoundaryPoints.forEach(v => {
      vertices.push(v.X, v.Y, v.Z);
    });

    if (vertices.length === 0) {
      return null;
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));

    const color = this.getColorFromIndex(shapeData.ColorIndex);
    const material = new THREE.LineBasicMaterial({
      color: color,
      transparent: shapeData.Transparent,
      opacity: shapeData.Opacity * 0.5,
      depthTest: shapeData.DepthTest,
      depthWrite: false
    });

    const line = new THREE.Line(geometry, material);
    line.name = 'Boundary';
    line.userData = {
      type: 'Boundary',
      boundaryPointCount: shapeData.BoundaryPoints.length
    };

    return line;
  }

  private static renderBounds(shapeData: ShapeData): THREE.Line | null {
    if (!shapeData.Bounds) {
      return null;
    }

    const bounds = shapeData.Bounds;
    const vertices: number[] = [];

    const min = bounds.Min;
    const max = bounds.Max;

    vertices.push(min.X, min.Y, min.Z);
    vertices.push(max.X, min.Y, min.Z);
    vertices.push(max.X, max.Y, min.Z);
    vertices.push(min.X, max.Y, min.Z);
    vertices.push(min.X, min.Y, min.Z);

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));

    const color = this.getColorFromIndex(shapeData.ColorIndex);
    const material = new THREE.LineBasicMaterial({
      color: color,
      transparent: shapeData.Transparent,
      opacity: shapeData.Opacity * 0.3,
      depthTest: shapeData.DepthTest,
      depthWrite: false
    });

    const line = new THREE.Line(geometry, material);
    line.name = 'Bounds';
    line.userData = {
      type: 'Bounds',
      size: bounds.Size
    };

    return line;
  }

  public static dispose(shapeData: ShapeData, scene: THREE.Scene): boolean {
    if (!shapeData || !scene) {
      return false;
    }

    const objects = this.shapeCache.get(shapeData.Handle);
    if (!objects) {
      return false;
    }

    objects.forEach(obj => {
      scene.remove(obj);
      if (obj instanceof THREE.Mesh || obj instanceof THREE.Line) {
        if (obj.geometry) {
          obj.geometry.dispose();
        }
        if (obj.material) {
          obj.material.dispose();
        }
      }
    });
    this.shapeCache.delete(shapeData.Handle);

    return true;
  }



  public static update(shapeData: ShapeData, scene: THREE.Scene): boolean {
    if (!shapeData || !scene) {
      return false;
    }

    const objects = this.shapeCache.get(shapeData.Handle);
    if (!objects) {
      return false;
    }

    objects.forEach(obj => {
      obj.visible = shapeData.Visible;
      if (obj.userData) {
        obj.userData.visible = shapeData.Visible;
      }
      if (obj instanceof THREE.Mesh || obj instanceof THREE.Line) {
        if (obj.material instanceof THREE.MeshBasicMaterial || 
            obj.material instanceof THREE.LineBasicMaterial) {
          obj.material.opacity = shapeData.Opacity;
          obj.material.transparent = shapeData.Transparent;
        }
      }
    });

    return true;
  }

  public static getShapeObjects(shapeData: ShapeData, scene: THREE.Scene): THREE.Object3D[] | null {
    return this.shapeCache.get(shapeData.Handle) || null;
  }

  public static setVisibility(shapeData: ShapeData, scene: THREE.Scene, visible: boolean): boolean {
    const objects = this.shapeCache.get(shapeData.Handle);
    if (objects) {
      objects.forEach(obj => {
        obj.visible = visible;
      });
      return true;
    }
    return false;
  }

  public static setOpacity(shapeData: ShapeData, scene: THREE.Scene, opacity: number): boolean {
    const objects = this.shapeCache.get(shapeData.Handle);
    if (objects) {
      objects.forEach(obj => {
        if (obj instanceof THREE.Mesh || obj instanceof THREE.Line) {
          if (obj.material instanceof THREE.MeshBasicMaterial || 
              obj.material instanceof THREE.LineBasicMaterial) {
            obj.material.opacity = opacity;
            obj.material.transparent = opacity < 1.0;
          }
        }
      });
      return true;
    }
    return false;
  }

  public static getInsertionPoint(shapeData: ShapeData): Point3DData {
    return shapeData.InsertionPoint || { X: 0, Y: 0, Z: 0 };
  }

  public static getSize(shapeData: ShapeData): number {
    return shapeData.Size ?? this.DEFAULT_SIZE;
  }

  public static getRotation(shapeData: ShapeData): number {
    return shapeData.Rotation ?? this.DEFAULT_ROTATION;
  }

  public static getRelativeXScale(shapeData: ShapeData): number {
    return shapeData.RelativeXScale ?? this.DEFAULT_RELATIVE_X_SCALE;
  }

  public static getThickness(shapeData: ShapeData): number {
    return shapeData.Thickness ?? this.DEFAULT_THICKNESS;
  }

  public static getBoundaryPoints(shapeData: ShapeData): Point3DData[] {
    return shapeData.BoundaryPoints || [];
  }

  public static getBounds(shapeData: ShapeData): BoundsData {
    return shapeData.Bounds || {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 0, Y: 0, Z: 0 },
      Center: { X: 0, Y: 0, Z: 0 },
      Size: { X: 0, Y: 0, Z: 0 }
    };
  }

  public static getCentroid(shapeData: ShapeData): Point3DData {
    return shapeData.Centroid || { X: 0, Y: 0, Z: 0 };
  }

  public static getWidth(shapeData: ShapeData): number {
    return shapeData.Width ?? 0;
  }

  public static getHeight(shapeData: ShapeData): number {
    return shapeData.Height ?? 0;
  }

  public static getShapeIndex(shapeData: ShapeData): number {
    return shapeData.ShapeIndex ?? 0;
  }

  public static getShapeStyleName(shapeData: ShapeData): string {
    return shapeData.ShapeStyleName || '';
  }

  public static getNormal(shapeData: ShapeData): Point3DData {
    if (!shapeData.Normal || (shapeData.Normal.X === 0 && shapeData.Normal.Y === 0 && shapeData.Normal.Z === 0)) {
      return { X: 0, Y: 0, Z: 1 };
    }
    return shapeData.Normal;
  }

  public static getColor(shapeData: ShapeData): THREE.Color {
    return new THREE.Color(shapeData.Color?.Hex || this.DEFAULT_COLOR);
  }

  public static getHandle(shapeData: ShapeData): string {
    return shapeData.Handle || '';
  }

  public static getLayerName(shapeData: ShapeData): string {
    return shapeData.LayerName || '';
  }

  public static getVisible(shapeData: ShapeData): boolean {
    return shapeData.Visible !== false;
  }

  public static getOpacity(shapeData: ShapeData): number {
    return shapeData.Opacity !== undefined ? shapeData.Opacity : this.DEFAULT_OPACITY;
  }

  public static getTransparent(shapeData: ShapeData): boolean {
    return shapeData.Transparent !== false;
  }

  public static getDepthTest(shapeData: ShapeData): boolean {
    return shapeData.DepthTest !== false;
  }

  private static getColorFromIndex(colorIndex: number): string {
    const colorMap: { [key: number]: string } = {
      0: '#FFFFFF',
      1: '#FF0000',
      2: '#FFFF00',
      3: '#00FF00',
      4: '#00FFFF',
      5: '#0000FF',
      6: '#FF00FF',
      7: '#FFFFFF'
    };
    return colorMap[colorIndex] || this.DEFAULT_COLOR;
  }

  public static clearCache(): void {
    this.shapeCache.forEach((objects) => {
      objects.forEach(obj => {
        if (obj instanceof THREE.Mesh || obj instanceof THREE.Line) {
          if (obj.geometry) {
            obj.geometry.dispose();
          }
          if (obj.material) {
            obj.material.dispose();
          }
        }
      });
    });
    this.shapeCache.clear();
  }

  public static renderMultiple(shapeDataArray: ShapeData[], scene: THREE.Scene): THREE.Object3D[] {
    const allObjects: THREE.Object3D[] = [];

    shapeDataArray.forEach((shapeData) => {
      const shapeObjects = this.render(shapeData, scene);
      if (shapeObjects) {
        allObjects.push(...shapeObjects);
      }
    });

    return allObjects;
  }

  public static disposeMultiple(objects: THREE.Object3D[], scene: THREE.Scene): void {
    if (!objects || objects.length === 0) {
      return;
    }

    objects.forEach(obj => {
      scene.remove(obj);
      if (obj instanceof THREE.Mesh || obj instanceof THREE.Line) {
        if (obj.geometry) {
          obj.geometry.dispose();
        }
        if (obj.material) {
          obj.material.dispose();
        }
      }
    });
  }

  public static getTransform(shapeData: ShapeData): any {
    if (!shapeData.Transform) {
      return {
        ScaleX: 1.0,
        ScaleY: 1.0,
        ScaleZ: 1.0,
        Rotation: 0,
        TranslateX: shapeData.InsertionPoint.X,
        TranslateY: shapeData.InsertionPoint.Y,
        TranslateZ: shapeData.InsertionPoint.Z
      };
    }
    return {
      ScaleX: shapeData.Transform.Scale.X,
      ScaleY: shapeData.Transform.Scale.Y,
      ScaleZ: shapeData.Transform.Scale.Z,
      Rotation: shapeData.Transform.Rotation.Z,
      TranslateX: shapeData.Transform.Position.X,
      TranslateY: shapeData.Transform.Position.Y,
      TranslateZ: shapeData.Transform.Position.Z
    };
  }

  public static getMaterial(shapeData: ShapeData): MaterialData {
    if (!shapeData.Material) {
      return {
        Type: 'MeshBasicMaterial',
        Color: shapeData.Color,
        Opacity: shapeData.Opacity ?? this.DEFAULT_OPACITY,
        Transparent: shapeData.Transparent ?? false,
        DepthTest: shapeData.DepthTest !== false,
        Side: false,
        Wireframe: false
      };
    }
    return shapeData.Material;
  }

  public static getGeometry(shapeData: ShapeData): GeometryData {
    if (!shapeData.Geometry) {
      return {
        Type: 'BufferGeometry',
        VertexCount: shapeData.VertexPositions?.length / 3 || 0,
        IndexCount: shapeData.Indices?.length || 0,
        Bounds: this.getBounds(shapeData)
      };
    }
    return shapeData.Geometry;
  }

  public static getVertexPositions(shapeData: ShapeData): number[] {
    return shapeData.VertexPositions || [];
  }

  public static getVertexNormals(shapeData: ShapeData): number[] {
    return shapeData.VertexNormals || [];
  }

  public static getVertexColors(shapeData: ShapeData): number[] {
    return shapeData.VertexColors || [];
  }

  public static getIndices(shapeData: ShapeData): number[] {
    return shapeData.Indices || [];
  }
}
