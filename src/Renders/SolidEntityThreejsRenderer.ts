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

export interface SolidData {
  Points: Point3DData[];
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  LineTypeScale: number;
  Normal: Point3DData;
  Thickness: number;
  HasFourthCorner: boolean;
  Bounds: BoundsData;
  Centroid: Point3DData;
  Area: number;
  Perimeter: number;

  Transform: TransformData;
  Geometry: GeometryData;
  Material: MaterialData;
  Color: ColorData;
  VertexPositions: number[];
  VertexNormals: number[];
  VertexColors: number[];
  VertexUVs: number[];
  Indices: number[];
  IsTriangle: boolean;
  FirstCorner: Point3DData;
  SecondCorner: Point3DData;
  ThirdCorner: Point3DData;
  FourthCorner: Point3DData;
  VertexCount: number;
  FaceCount: number;
  IsFilled: boolean;
  IsExtruded: boolean;
  ExtrusionDepth: number;

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

export class SolidEntityThreejsRenderer {
  private static readonly DEFAULT_THICKNESS = 0.0;
  private static readonly DEFAULT_OPACITY = 1.0;
  private static readonly DEFAULT_COLOR = '#FFFFFF';

  private static solidCache = new Map<string, THREE.Object3D[]>();

  public static render(solidData: SolidData, scene: THREE.Scene): THREE.Object3D[] | null {
    if (!solidData || !solidData.Visible) {
      return null;
    }

    const objects: THREE.Object3D[] = [];

    const solidMesh = this.renderSolid(solidData);
    if (solidMesh) {
      objects.push(solidMesh);
    }

    const edges = this.renderEdges(solidData);
    if (edges) {
      objects.push(edges);
    }

    const bounds = this.renderBounds(solidData);
    if (bounds) {
      objects.push(bounds);
    }

    objects.forEach(obj => {
      obj.visible = solidData.Visible;
      obj.userData = {
        type: solidData.Type,
        handle: solidData.Handle,
        layerName: solidData.LayerName,
        layerIndex: solidData.LayerIndex,
        coordinateSystem: solidData.CoordinateSystem,
        points: solidData.Points,
        colorIndex: solidData.ColorIndex,
        lineTypeName: solidData.LineTypeName,
        lineWeight: solidData.LineWeight,
        lineTypeScale: solidData.LineTypeScale,
        normal: solidData.Normal,
        thickness: solidData.Thickness,
        hasFourthCorner: solidData.HasFourthCorner,
        bounds: solidData.Bounds,
        centroid: solidData.Centroid,
        area: solidData.Area,
        perimeter: solidData.Perimeter,
        transform: solidData.Transform,
        geometry: solidData.Geometry,
        material: solidData.Material,
        color: solidData.Color,
        vertexPositions: solidData.VertexPositions,
        vertexNormals: solidData.VertexNormals,
        vertexColors: solidData.VertexColors,
        vertexUVs: solidData.VertexUVs,
        indices: solidData.Indices,
        isTriangle: solidData.IsTriangle,
        firstCorner: solidData.FirstCorner,
        secondCorner: solidData.SecondCorner,
        thirdCorner: solidData.ThirdCorner,
        fourthCorner: solidData.FourthCorner,
        vertexCount: solidData.VertexCount,
        faceCount: solidData.FaceCount,
        isFilled: solidData.IsFilled,
        isExtruded: solidData.IsExtruded,
        extrusionDepth: solidData.ExtrusionDepth,
        opacity: solidData.Opacity,
        transparent: solidData.Transparent,
        depthTest: solidData.DepthTest,
        objectType: obj.userData.objectType || 'Solid'
      };

      if (solidData.Transform && solidData.Transform.Matrix) {
        const matrix = new THREE.Matrix4();
        matrix.fromArray(solidData.Transform.Matrix);
        obj.applyMatrix4(matrix);
      }
    });

    this.solidCache.set(solidData.Handle, objects);

    return objects;
  }

  private static renderSolid(solidData: SolidData): THREE.Mesh | null {
    if (!solidData.VertexPositions || solidData.VertexPositions.length === 0) {
      console.warn(`Solid ${solidData.Handle} has no vertex positions`);
      return null;
    }

    const geometry = this.createGeometry(solidData);
    if (!geometry) {
      return null;
    }

    const material = this.createMaterial(solidData);

    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = `Solid_${solidData.Handle}`;
    mesh.userData = {
      type: 'Solid',
      handle: solidData.Handle,
      isTriangle: solidData.IsTriangle,
      isExtruded: solidData.IsExtruded,
      extrusionDepth: solidData.ExtrusionDepth,
      objectType: 'Solid'
    };

    if (solidData.Material && solidData.Material.Side) {
      mesh.material.side = THREE.DoubleSide;
    }

    return mesh;
  }

  private static createGeometry(solidData: SolidData): THREE.BufferGeometry | null {
    const vertices = solidData.VertexPositions || [];
    const indices = solidData.Indices || [];
    const normals = solidData.VertexNormals || [];
    const colors = solidData.VertexColors || [];
    const uvs = solidData.VertexUVs || [];

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

    if (uvs.length > 0) {
      geometry.setAttribute('uv', new THREE.Float32BufferAttribute(uvs, 2));
    }

    if (indices.length > 0) {
      geometry.setIndex(indices);
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static createMaterial(solidData: SolidData): THREE.Material {
    const materialData = solidData.Material;
    const color = materialData?.Color?.Hex || solidData.Color?.Hex || this.DEFAULT_COLOR;
    const opacity = materialData?.Opacity ?? solidData.Opacity ?? this.DEFAULT_OPACITY;
    const transparent = materialData?.Transparent ?? solidData.Transparent ?? (opacity < 1.0);
    const wireframe = materialData?.Wireframe ?? false;

    const material = new THREE.MeshBasicMaterial({
      color: color,
      transparent: transparent,
      opacity: opacity,
      wireframe: wireframe,
      depthTest: materialData?.DepthTest ?? solidData.DepthTest ?? true,
      depthWrite: false,
      side: materialData?.Side ? THREE.DoubleSide : THREE.FrontSide
    });

    return material;
  }

  private static renderEdges(solidData: SolidData): THREE.Line | null {
    if (!solidData.Points || solidData.Points.length === 0) {
      return null;
    }

    const vertices: number[] = [];
    solidData.Points.forEach(v => {
      vertices.push(v.X, v.Y, v.Z);
    });

    if (solidData.IsTriangle) {
      vertices.push(solidData.Points[0].X, solidData.Points[0].Y, solidData.Points[0].Z);
    } else {
      vertices.push(solidData.Points[0].X, solidData.Points[0].Y, solidData.Points[0].Z);
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));

    const color = this.getColorFromIndex(solidData.ColorIndex);
    const material = new THREE.LineBasicMaterial({
      color: color,
      transparent: solidData.Transparent,
      opacity: solidData.Opacity * 0.5,
      depthTest: solidData.DepthTest,
      depthWrite: false
    });

    const line = new THREE.Line(geometry, material);
    line.userData = {
      type: 'Edges',
      isTriangle: solidData.IsTriangle,
      vertexCount: solidData.Points.length,
      objectType: 'SolidEdges'
    };

    return line;
  }

  private static renderBounds(solidData: SolidData): THREE.Line | null {
    if (!solidData.Bounds) {
      return null;
    }

    const bounds = solidData.Bounds;
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

    const color = this.getColorFromIndex(solidData.ColorIndex);
    const material = new THREE.LineBasicMaterial({
      color: color,
      transparent: solidData.Transparent,
      opacity: solidData.Opacity * 0.3,
      depthTest: solidData.DepthTest,
      depthWrite: false
    });

    const line = new THREE.Line(geometry, material);
    line.userData = {
      type: 'Bounds',
      size: bounds.Size,
      objectType: 'SolidBounds'
    };

    return line;
  }

  public static dispose(solidData: SolidData, scene: THREE.Scene): boolean {
    if (!solidData || !scene) {
      return false;
    }

    const objects = this.solidCache.get(solidData.Handle);
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
    this.solidCache.delete(solidData.Handle);

    return true;
  }

  public static update(solidData: SolidData, scene: THREE.Scene): boolean {
    if (!solidData || !scene) {
      return false;
    }

    const objects = this.solidCache.get(solidData.Handle);
    if (!objects) {
      return false;
    }

    objects.forEach(obj => {
      obj.visible = solidData.Visible;

      if (obj instanceof THREE.Mesh || obj instanceof THREE.Line) {
        if (obj.material instanceof THREE.MeshBasicMaterial || 
            obj.material instanceof THREE.LineBasicMaterial) {
          obj.material.opacity = solidData.Opacity;
          obj.material.transparent = solidData.Transparent;
        }
      }
    });

    return true;
  }

  public static getSolidObjects(solidData: SolidData, scene: THREE.Scene): THREE.Object3D[] | null {
    return this.solidCache.get(solidData.Handle) || null;
  }

  public static setVisibility(solidData: SolidData, scene: THREE.Scene, visible: boolean): boolean {
    const objects = this.solidCache.get(solidData.Handle);
    if (objects) {
      objects.forEach(obj => {
        obj.visible = visible;
      });
      return true;
    }
    return false;
  }

  public static setOpacity(solidData: SolidData, scene: THREE.Scene, opacity: number): boolean {
    const objects = this.solidCache.get(solidData.Handle);
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

  public static getPoints(solidData: SolidData): Point3DData[] {
    return solidData.Points || [];
  }

  public static getThickness(solidData: SolidData): number {
    return solidData.Thickness ?? this.DEFAULT_THICKNESS;
  }

  public static getBounds(solidData: SolidData): BoundsData {
    return solidData.Bounds || {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 0, Y: 0, Z: 0 },
      Center: { X: 0, Y: 0, Z: 0 },
      Size: { X: 0, Y: 0, Z: 0 }
    };
  }

  public static getCentroid(solidData: SolidData): Point3DData {
    return solidData.Centroid || { X: 0, Y: 0, Z: 0 };
  }

  public static getArea(solidData: SolidData): number {
    return solidData.Area ?? 0;
  }

  public static getPerimeter(solidData: SolidData): number {
    return solidData.Perimeter ?? 0;
  }

  public static isTriangle(solidData: SolidData): boolean {
    return solidData.IsTriangle ?? false;
  }

  public static isExtruded(solidData: SolidData): boolean {
    return solidData.IsExtruded ?? false;
  }

  public static getExtrusionDepth(solidData: SolidData): number {
    return solidData.ExtrusionDepth ?? 0;
  }

  public static getFirstCorner(solidData: SolidData): Point3DData {
    return solidData.FirstCorner || { X: 0, Y: 0, Z: 0 };
  }

  public static getSecondCorner(solidData: SolidData): Point3DData {
    return solidData.SecondCorner || { X: 0, Y: 0, Z: 0 };
  }

  public static getThirdCorner(solidData: SolidData): Point3DData {
    return solidData.ThirdCorner || { X: 0, Y: 0, Z: 0 };
  }

  public static getFourthCorner(solidData: SolidData): Point3DData {
    return solidData.FourthCorner || { X: 0, Y: 0, Z: 0 };
  }

  public static getNormal(solidData: SolidData): Point3DData {
    if (!solidData.Normal || (solidData.Normal.X === 0 && solidData.Normal.Y === 0 && solidData.Normal.Z === 0)) {
      return { X: 0, Y: 0, Z: 1 };
    }
    return solidData.Normal;
  }

  public static getColor(solidData: SolidData): THREE.Color {
    return new THREE.Color(solidData.Color?.Hex || this.DEFAULT_COLOR);
  }

  public static getHandle(solidData: SolidData): string {
    return solidData.Handle || '';
  }

  public static getLayerName(solidData: SolidData): string {
    return solidData.LayerName || '';
  }

  public static getVisible(solidData: SolidData): boolean {
    return solidData.Visible !== false;
  }

  public static getOpacity(solidData: SolidData): number {
    return solidData.Opacity !== undefined ? solidData.Opacity : this.DEFAULT_OPACITY;
  }

  public static getTransparent(solidData: SolidData): boolean {
    return solidData.Transparent !== false;
  }

  public static getDepthTest(solidData: SolidData): boolean {
    return solidData.DepthTest !== false;
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
    this.solidCache.forEach((objects) => {
      objects.forEach((obj) => {
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
    this.solidCache.clear();
  }

  public static renderMultiple(solidDataArray: SolidData[], scene: THREE.Scene): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];

    solidDataArray.forEach((solidData) => {
      const solidObjects = this.render(solidData, scene);
      if (solidObjects) {
        objects.push(...solidObjects);
      }
    });

    return objects;
  }

  public static disposeMultiple(objects: THREE.Object3D[], scene: THREE.Scene): void {
    if (!objects || objects.length === 0) {
      return;
    }

    objects.forEach((obj) => {
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

  public static getTransform(solidData: SolidData): any {
    if (!solidData.Transform) {
      return {
        ScaleX: 1.0,
        ScaleY: 1.0,
        ScaleZ: 1.0,
        Rotation: 0,
        TranslateX: solidData.Centroid.X,
        TranslateY: solidData.Centroid.Y,
        TranslateZ: solidData.Centroid.Z
      };
    }
    return {
      ScaleX: solidData.Transform.Scale.X,
      ScaleY: solidData.Transform.Scale.Y,
      ScaleZ: solidData.Transform.Scale.Z,
      Rotation: solidData.Transform.Rotation.Z,
      TranslateX: solidData.Transform.Position.X,
      TranslateY: solidData.Transform.Position.Y,
      TranslateZ: solidData.Transform.Position.Z
    };
  }

  public static getMaterial(solidData: SolidData): MaterialData {
    if (!solidData.Material) {
      return {
        Type: 'MeshBasicMaterial',
        Color: solidData.Color,
        Opacity: solidData.Opacity ?? this.DEFAULT_OPACITY,
        Transparent: solidData.Transparent ?? false,
        DepthTest: solidData.DepthTest !== false,
        Side: false,
        Wireframe: false
      };
    }
    return solidData.Material;
  }

  public static getGeometry(solidData: SolidData): GeometryData {
    if (!solidData.Geometry) {
      return {
        Type: 'BufferGeometry',
        VertexCount: solidData.VertexPositions?.length / 3 || 0,
        IndexCount: solidData.Indices?.length || 0,
        Bounds: this.getBounds(solidData)
      };
    }
    return solidData.Geometry;
  }

  public static getVertexPositions(solidData: SolidData): number[] {
    return solidData.VertexPositions || [];
  }

  public static getVertexNormals(solidData: SolidData): number[] {
    return solidData.VertexNormals || [];
  }

  public static getVertexColors(solidData: SolidData): number[] {
    return solidData.VertexColors || [];
  }

  public static getVertexUVs(solidData: SolidData): number[] {
    return solidData.VertexUVs || [];
  }

  public static getIndices(solidData: SolidData): number[] {
    return solidData.Indices || [];
  }

  public static getVertexCount(solidData: SolidData): number {
    return solidData.VertexCount ?? 0;
  }

  public static getFaceCount(solidData: SolidData): number {
    return solidData.FaceCount ?? 0;
  }

  public static isFilled(solidData: SolidData): boolean {
    return solidData.IsFilled ?? true;
  }

  public static hasFourthCorner(solidData: SolidData): boolean {
    return solidData.HasFourthCorner ?? false;
  }
}
