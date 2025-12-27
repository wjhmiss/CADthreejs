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

export interface UnderlayMaterialData {
  Color: ColorData;
  Opacity: number;
  Transparent: boolean;
  Contrast: number;
  Fade: number;
  Type: string;
  DepthTest: boolean;
  Side: boolean;
}

export interface UnderlayGeometryData {
  Vertices: Point3DData[];
  Indices: number[];
  Position: Point3DData;
  Rotation: Point3DData;
  Scale: Point3DData;
  Normal: NormalData;
  Type: string;
  Bounds: BoundsData3D;
  VertexCount: number;
  IndexCount: number;
}

export interface PdfUnderlayData {
  Type: string;
  EntityType: string;
  Handle: string;
  LayerName: string;
  LayerIndex: number;
  Visible: boolean;
  CoordinateSystem: string;

  BoundaryPoints: PointData[];
  InsertPoint: PointData;
  XScale: number;
  YScale: number;
  ZScale: number;
  Rotation: number;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  Contrast: number;
  Fade: number;
  Flags: number;
  DefinitionFileName: string;

  BoundaryPoints3D: Point3DData[];
  InsertPoint3D: Point3DData;
  Color: ColorData;
  Transform: TransformData;
  Normal: NormalData;

  Opacity: number;
  Transparent: boolean;
  DepthTest: boolean;

  PageNumber: string;
  FilePath: string;
  ClippingBoundaryType: string;

  Material: UnderlayMaterialData;
  Geometry: UnderlayGeometryData;
}

export class PdfUnderlayEntityThreejsRenderer {
  private static readonly DEFAULT_COLOR = '#ffffff';
  private static readonly DEFAULT_OPACITY = 1.0;
  private static readonly DEFAULT_CONTRAST = 50;
  private static readonly DEFAULT_FADE = 0;

  private static textureCache: Map<string, THREE.Texture> = new Map();
  private static pdfCache: Map<string, any> = new Map();

  public static render(pdfUnderlayData: PdfUnderlayData, scene: THREE.Scene): THREE.Group | null {
    if (!pdfUnderlayData || !pdfUnderlayData.Visible) {
      return null;
    }

    if (!pdfUnderlayData.FilePath || pdfUnderlayData.FilePath.length === 0) {
      console.warn(`PdfUnderlay ${pdfUnderlayData.Handle} has no file path`);
      return null;
    }

    const group = new THREE.Group();
    group.name = `PdfUnderlay_${pdfUnderlayData.Handle}`;
    group.visible = pdfUnderlayData.Visible;

    group.userData = {
      type: pdfUnderlayData.Type,
      entityType: pdfUnderlayData.EntityType,
      handle: pdfUnderlayData.Handle,
      layerName: pdfUnderlayData.LayerName,
      layerIndex: pdfUnderlayData.LayerIndex,
      coordinateSystem: pdfUnderlayData.CoordinateSystem,
      boundaryPoints: pdfUnderlayData.BoundaryPoints,
      insertPoint: pdfUnderlayData.InsertPoint,
      xScale: pdfUnderlayData.XScale,
      yScale: pdfUnderlayData.YScale,
      zScale: pdfUnderlayData.ZScale,
      rotation: pdfUnderlayData.Rotation,
      colorIndex: pdfUnderlayData.ColorIndex,
      lineTypeName: pdfUnderlayData.LineTypeName,
      lineWeight: pdfUnderlayData.LineWeight,
      contrast: pdfUnderlayData.Contrast,
      fade: pdfUnderlayData.Fade,
      flags: pdfUnderlayData.Flags,
      definitionFileName: pdfUnderlayData.DefinitionFileName,
      boundaryPoints3D: pdfUnderlayData.BoundaryPoints3D,
      insertPoint3D: pdfUnderlayData.InsertPoint3D,
      color: pdfUnderlayData.Color,
      transform: pdfUnderlayData.Transform,
      normal: pdfUnderlayData.Normal,
      opacity: pdfUnderlayData.Opacity,
      transparent: pdfUnderlayData.Transparent,
      depthTest: pdfUnderlayData.DepthTest,
      pageNumber: pdfUnderlayData.PageNumber,
      filePath: pdfUnderlayData.FilePath,
      clippingBoundaryType: pdfUnderlayData.ClippingBoundaryType,
      material: pdfUnderlayData.Material,
      geometry: pdfUnderlayData.Geometry
    };

    if (pdfUnderlayData.Transform && pdfUnderlayData.Transform.Matrix) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(pdfUnderlayData.Transform.Matrix);
      group.applyMatrix4(matrix);
    }

    this.renderUnderlay(pdfUnderlayData, group);
    this.renderClippingBoundary(pdfUnderlayData, group);
    this.renderBounds(pdfUnderlayData, group);

    scene.add(group);
    return group;
  }

  private static async renderUnderlay(pdfUnderlayData: PdfUnderlayData, group: THREE.Group): Promise<void> {
    if (!pdfUnderlayData.Geometry || !pdfUnderlayData.Geometry.Vertices || pdfUnderlayData.Geometry.Vertices.length === 0) {
      console.warn(`PdfUnderlay ${pdfUnderlayData.Handle} has no geometry vertices`);
      return;
    }

    const texture = await this.loadPdfTexture(pdfUnderlayData);
    if (!texture) {
      console.warn(`PdfUnderlay ${pdfUnderlayData.Handle} failed to load PDF texture`);
      return;
    }

    const geometry = this.createGeometry(pdfUnderlayData);
    if (!geometry) {
      return;
    }

    const material = this.createMaterial(pdfUnderlayData, texture);

    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = 'Underlay';
    mesh.userData = {
      type: 'PdfUnderlay',
      filePath: pdfUnderlayData.FilePath,
      pageNumber: pdfUnderlayData.PageNumber,
      clippingBoundaryType: pdfUnderlayData.ClippingBoundaryType
    };

    if (pdfUnderlayData.Material && pdfUnderlayData.Material.Side) {
      mesh.material.side = THREE.DoubleSide;
    }

    group.add(mesh);
  }

  private static createGeometry(pdfUnderlayData: PdfUnderlayData): THREE.BufferGeometry | null {
    const vertices: number[] = [];
    const indices: number[] = [];
    const uvs: number[] = [];

    const geometryData = pdfUnderlayData.Geometry;

    if (geometryData.Vertices && geometryData.Vertices.length >= 4) {
      geometryData.Vertices.forEach(v => {
        vertices.push(v.X, v.Y, v.Z);
      });

      if (geometryData.Indices && geometryData.Indices.length > 0) {
        indices.push(...geometryData.Indices);
      } else {
        indices.push(0, 1, 2, 0, 2, 3);
      }

      const bounds = geometryData.Bounds;
      if (bounds) {
        const min = bounds.Min;
        const max = bounds.Max;
        const width = max.X - min.X;
        const height = max.Y - min.Y;

        uvs.push(0, 1);
        uvs.push(1, 1);
        uvs.push(1, 0);
        uvs.push(0, 0);
      }
    }

    if (vertices.length === 0) {
      return null;
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));
    geometry.setAttribute('uv', new THREE.Float32BufferAttribute(uvs, 2));

    if (geometryData.Normal) {
      const normals: number[] = [];
      for (let i = 0; i < vertices.length / 3; i++) {
        normals.push(geometryData.Normal.X, geometryData.Normal.Y, geometryData.Normal.Z);
      }
      geometry.setAttribute('normal', new THREE.Float32BufferAttribute(normals, 3));
    } else {
      geometry.computeVertexNormals();
    }

    if (indices.length > 0) {
      geometry.setIndex(indices);
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static createMaterial(pdfUnderlayData: PdfUnderlayData, texture: THREE.Texture): THREE.Material {
    const materialData = pdfUnderlayData.Material;
    const color = materialData?.Color?.Hex || this.DEFAULT_COLOR;
    const opacity = materialData?.Opacity ?? pdfUnderlayData.Opacity ?? this.DEFAULT_OPACITY;
    const transparent = materialData?.Transparent ?? pdfUnderlayData.Transparent ?? (opacity < 1.0);
    const contrast = materialData?.Contrast ?? pdfUnderlayData.Contrast ?? this.DEFAULT_CONTRAST;
    const fade = materialData?.Fade ?? pdfUnderlayData.Fade ?? this.DEFAULT_FADE;

    const adjustedOpacity = this.calculateAdjustedOpacity(opacity, fade);
    const adjustedColor = this.adjustColorForContrast(color, contrast);

    const material = new THREE.MeshBasicMaterial({
      map: texture,
      color: adjustedColor,
      transparent: transparent,
      opacity: adjustedOpacity,
      depthTest: materialData?.DepthTest ?? pdfUnderlayData.DepthTest ?? true,
      depthWrite: false,
      side: materialData?.Side ? THREE.DoubleSide : THREE.FrontSide
    });

    return material;
  }

  private static calculateAdjustedOpacity(baseOpacity: number, fade: number): number {
    const fadeFactor = fade / 100.0;
    return Math.max(0, Math.min(1, baseOpacity * (1.0 - fadeFactor)));
  }

  private static adjustColorForContrast(color: string, contrast: number): number {
    const contrastFactor = contrast / 50.0;
    const colorObj = new THREE.Color(color);
    
    if (contrastFactor > 1.0) {
      colorObj.multiplyScalar(contrastFactor);
    } else if (contrastFactor < 1.0) {
      colorObj.multiplyScalar(contrastFactor);
    }
    
    return colorObj;
  }

  private static async loadPdfTexture(pdfUnderlayData: PdfUnderlayData): Promise<THREE.Texture | null> {
    const cacheKey = `${pdfUnderlayData.FilePath}_${pdfUnderlayData.PageNumber}`;

    if (this.textureCache.has(cacheKey)) {
      return this.textureCache.get(cacheKey)!;
    }

    try {
      const canvas = await this.renderPdfToCanvas(pdfUnderlayData);
      if (!canvas) {
        return null;
      }

      const texture = new THREE.CanvasTexture(canvas);
      texture.needsUpdate = true;
      texture.colorSpace = THREE.SRGBColorSpace;
      texture.minFilter = THREE.LinearFilter;
      texture.magFilter = THREE.LinearFilter;
      texture.wrapS = THREE.ClampToEdgeWrapping;
      texture.wrapT = THREE.ClampToEdgeWrapping;

      this.textureCache.set(cacheKey, texture);
      return texture;
    } catch (error) {
      console.error(`Failed to load PDF texture for ${pdfUnderlayData.FilePath}:`, error);
      return null;
    }
  }

  private static async renderPdfToCanvas(pdfUnderlayData: PdfUnderlayData): Promise<HTMLCanvasElement | null> {
    if (typeof window === 'undefined' || typeof document === 'undefined') {
      console.warn('PDF rendering not available in non-browser environment');
      return null;
    }

    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');
    if (!ctx) {
      return null;
    }

    const bounds = pdfUnderlayData.Geometry?.Bounds;
    if (bounds) {
      canvas.width = Math.ceil(bounds.Size.X);
      canvas.height = Math.ceil(bounds.Size.Y);
    } else {
      canvas.width = 1024;
      canvas.height = 1024;
    }

    ctx.fillStyle = '#ffffff';
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    ctx.fillStyle = '#cccccc';
    ctx.font = '14px Arial';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(`PDF: ${pdfUnderlayData.FilePath}`, canvas.width / 2, canvas.height / 2 - 20);
    ctx.fillText(`Page: ${pdfUnderlayData.PageNumber}`, canvas.width / 2, canvas.height / 2 + 20);

    return canvas;
  }

  private static renderClippingBoundary(pdfUnderlayData: PdfUnderlayData, group: THREE.Group): void {
    if (!pdfUnderlayData.ClippingBoundaryType || pdfUnderlayData.ClippingBoundaryType === 'None') {
      return;
    }

    if (!pdfUnderlayData.BoundaryPoints3D || pdfUnderlayData.BoundaryPoints3D.length === 0) {
      return;
    }

    const vertices: number[] = [];
    pdfUnderlayData.BoundaryPoints3D.forEach(v => {
      vertices.push(v.X, v.Y, v.Z);
    });

    if (vertices.length === 0) {
      return;
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));

    const color = this.getColorFromIndex(pdfUnderlayData.ColorIndex);
    const material = new THREE.LineBasicMaterial({
      color: color,
      transparent: pdfUnderlayData.Transparent,
      opacity: pdfUnderlayData.Opacity * 0.5,
      depthTest: pdfUnderlayData.DepthTest,
      depthWrite: false
    });

    const line = new THREE.Line(geometry, material);
    line.name = 'ClippingBoundary';
    line.userData = {
      type: 'ClippingBoundary',
      clippingBoundaryType: pdfUnderlayData.ClippingBoundaryType,
      vertexCount: pdfUnderlayData.BoundaryPoints3D.length
    };

    group.add(line);
  }

  private static renderBounds(pdfUnderlayData: PdfUnderlayData, group: THREE.Group): void {
    if (!pdfUnderlayData.Geometry || !pdfUnderlayData.Geometry.Bounds) {
      return;
    }

    const bounds = pdfUnderlayData.Geometry.Bounds;
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

    const color = this.getColorFromIndex(pdfUnderlayData.ColorIndex);
    const material = new THREE.LineBasicMaterial({
      color: color,
      transparent: pdfUnderlayData.Transparent,
      opacity: pdfUnderlayData.Opacity * 0.3,
      depthTest: pdfUnderlayData.DepthTest,
      depthWrite: false
    });

    const line = new THREE.Line(geometry, material);
    line.name = 'Bounds';
    line.userData = {
      type: 'Bounds',
      size: bounds.Size
    };

    group.add(line);
  }

  public static dispose(pdfUnderlayData: PdfUnderlayData, scene: THREE.Scene): boolean {
    if (!pdfUnderlayData || !scene) {
      return false;
    }

    const group = scene.getObjectByName(`PdfUnderlay_${pdfUnderlayData.Handle}`);
    if (group) {
      scene.remove(group);
      this.disposeGroup(group);
      return true;
    }
    return false;
  }

  private static disposeGroup(group: THREE.Group): void {
    group.traverse((object) => {
      if (object instanceof THREE.Mesh || object instanceof THREE.Line) {
        if (object.geometry) {
          object.geometry.dispose();
        }
        if (object.material) {
          if (object.material instanceof THREE.MeshBasicMaterial) {
            if (object.material.map) {
              object.material.map.dispose();
            }
            object.material.dispose();
          } else if (object.material instanceof THREE.LineBasicMaterial) {
            object.material.dispose();
          }
        }
      }
    });
    group.clear();
  }

  public static clearCache(): void {
    this.textureCache.forEach(texture => texture.dispose());
    this.textureCache.clear();
    this.pdfCache.clear();
  }

  public static update(pdfUnderlayData: PdfUnderlayData, scene: THREE.Scene): boolean {
    if (!pdfUnderlayData || !scene) {
      return false;
    }

    const group = this.getPdfUnderlayGroup(pdfUnderlayData, scene);
    if (!group) {
      return false;
    }

    group.visible = pdfUnderlayData.Visible;
    group.userData.visible = pdfUnderlayData.Visible;

    group.traverse((object) => {
      if (object instanceof THREE.Mesh || object instanceof THREE.Line) {
        if (object.material instanceof THREE.MeshBasicMaterial || 
            object.material instanceof THREE.LineBasicMaterial) {
          object.material.opacity = pdfUnderlayData.Opacity;
          object.material.transparent = pdfUnderlayData.Transparent;
        }
      }
    });

    return true;
  }

  public static getPdfUnderlayGroup(pdfUnderlayData: PdfUnderlayData, scene: THREE.Scene): THREE.Group | null {
    return scene.getObjectByName(`PdfUnderlay_${pdfUnderlayData.Handle}`) as THREE.Group || null;
  }

  public static setVisibility(pdfUnderlayData: PdfUnderlayData, scene: THREE.Scene, visible: boolean): boolean {
    const group = this.getPdfUnderlayGroup(pdfUnderlayData, scene);
    if (group) {
      group.visible = visible;
      return true;
    }
    return false;
  }

  public static setOpacity(pdfUnderlayData: PdfUnderlayData, scene: THREE.Scene, opacity: number): boolean {
    const group = this.getPdfUnderlayGroup(pdfUnderlayData, scene);
    if (group) {
      const adjustedOpacity = this.calculateAdjustedOpacity(opacity, pdfUnderlayData.Fade);
      group.traverse((object) => {
        if (object instanceof THREE.Mesh || object instanceof THREE.Line) {
          if (object.material instanceof THREE.MeshBasicMaterial || 
              object.material instanceof THREE.LineBasicMaterial) {
            object.material.opacity = adjustedOpacity;
            object.material.transparent = adjustedOpacity < 1.0;
          }
        }
      });
      return true;
    }
    return false;
  }

  public static setContrast(pdfUnderlayData: PdfUnderlayData, scene: THREE.Scene, contrast: number): boolean {
    const group = this.getPdfUnderlayGroup(pdfUnderlayData, scene);
    if (group) {
      const adjustedColor = this.adjustColorForContrast(pdfUnderlayData.Color?.Hex || this.DEFAULT_COLOR, contrast);
      group.traverse((object) => {
        if (object instanceof THREE.Mesh) {
          if (object.material instanceof THREE.MeshBasicMaterial) {
            object.material.color.set(adjustedColor);
          }
        }
      });
      return true;
    }
    return false;
  }

  public static setFade(pdfUnderlayData: PdfUnderlayData, scene: THREE.Scene, fade: number): boolean {
    const group = this.getPdfUnderlayGroup(pdfUnderlayData, scene);
    if (group) {
      const adjustedOpacity = this.calculateAdjustedOpacity(pdfUnderlayData.Opacity, fade);
      group.traverse((object) => {
        if (object instanceof THREE.Mesh || object instanceof THREE.Line) {
          if (object.material instanceof THREE.MeshBasicMaterial || 
              object.material instanceof THREE.LineBasicMaterial) {
            object.material.opacity = adjustedOpacity;
            object.material.transparent = adjustedOpacity < 1.0;
          }
        }
      });
      return true;
    }
    return false;
  }

  public static reloadTexture(pdfUnderlayData: PdfUnderlayData, scene: THREE.Scene): boolean {
    const group = this.getPdfUnderlayGroup(pdfUnderlayData, scene);
    if (!group) {
      return false;
    }

    const cacheKey = `${pdfUnderlayData.FilePath}_${pdfUnderlayData.PageNumber}`;
    this.textureCache.delete(cacheKey);

    this.dispose(pdfUnderlayData, scene);
    const result = this.render(pdfUnderlayData, scene);
    return result !== null;
  }

  public static getFilePath(pdfUnderlayData: PdfUnderlayData): string {
    return pdfUnderlayData.FilePath || '';
  }

  public static getPageNumber(pdfUnderlayData: PdfUnderlayData): string {
    return pdfUnderlayData.PageNumber || '1';
  }

  public static getClippingBoundaryType(pdfUnderlayData: PdfUnderlayData): string {
    return pdfUnderlayData.ClippingBoundaryType || 'None';
  }

  public static getContrast(pdfUnderlayData: PdfUnderlayData): number {
    if (pdfUnderlayData.Contrast === undefined || pdfUnderlayData.Contrast === null) {
      return this.DEFAULT_CONTRAST;
    }
    return pdfUnderlayData.Contrast;
  }

  public static getFade(pdfUnderlayData: PdfUnderlayData): number {
    return pdfUnderlayData.Fade ?? this.DEFAULT_FADE;
  }

  public static getInsertPoint3D(pdfUnderlayData: PdfUnderlayData): Point3DData {
    return pdfUnderlayData.InsertPoint3D || { X: 0, Y: 0, Z: 0 };
  }

  public static getScale(pdfUnderlayData: PdfUnderlayData): Point3DData {
    return {
      X: pdfUnderlayData.XScale || 1.0,
      Y: pdfUnderlayData.YScale || 1.0,
      Z: pdfUnderlayData.ZScale || 1.0
    };
  }

  public static getRotation(pdfUnderlayData: PdfUnderlayData): number {
    return pdfUnderlayData.Rotation || 0;
  }

  public static getBounds(pdfUnderlayData: PdfUnderlayData): BoundsData3D {
    return pdfUnderlayData.Geometry?.Bounds || {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 0, Y: 0, Z: 0 },
      Center: { X: 0, Y: 0, Z: 0 },
      Size: { X: 0, Y: 0, Z: 0 }
    };
  }

  public static getBoundaryPoints(pdfUnderlayData: PdfUnderlayData): Point3DData[] {
    return pdfUnderlayData.BoundaryPoints3D || [];
  }

  public static isPointInUnderlay(pdfUnderlayData: PdfUnderlayData, point: Point3DData, tolerance: number = 0.001): boolean {
    const bounds = this.getBounds(pdfUnderlayData);
    const min = bounds.Min;
    const max = bounds.Max;

    return point.X >= min.X - tolerance &&
           point.X <= max.X + tolerance &&
           point.Y >= min.Y - tolerance &&
           point.Y <= max.Y + tolerance &&
           point.Z >= min.Z - tolerance &&
           point.Z <= max.Z + tolerance;
  }

  public static getClosestPoint(pdfUnderlayData: PdfUnderlayData, point: Point3DData): Point3DData | null {
    const bounds = this.getBounds(pdfUnderlayData);
    const min = bounds.Min;
    const max = bounds.Max;

    const closestX = Math.max(min.X, Math.min(max.X, point.X));
    const closestY = Math.max(min.Y, Math.min(max.Y, point.Y));
    const closestZ = Math.max(min.Z, Math.min(max.Z, point.Z));

    return { X: closestX, Y: closestY, Z: closestZ };
  }

  public static getDistanceToPoint(pdfUnderlayData: PdfUnderlayData, point: Point3DData): number {
    const closestPoint = this.getClosestPoint(pdfUnderlayData, point);
    if (!closestPoint) {
      return Infinity;
    }

    const testPoint = new THREE.Vector3(point.X, point.Y, point.Z);
    const closest = new THREE.Vector3(closestPoint.X, closestPoint.Y, closestPoint.Z);

    return testPoint.distanceTo(closest);
  }

  public static clone(pdfUnderlayData: PdfUnderlayData, newHandle: string): PdfUnderlayData {
    const cloned = JSON.parse(JSON.stringify(pdfUnderlayData));
    cloned.Handle = newHandle;
    return cloned;
  }

  public static transform(pdfUnderlayData: PdfUnderlayData, matrix: THREE.Matrix4): PdfUnderlayData {
    const transformed = JSON.parse(JSON.stringify(pdfUnderlayData));

    if (transformed.InsertPoint3D) {
      const insertPoint = new THREE.Vector3(
        transformed.InsertPoint3D.X,
        transformed.InsertPoint3D.Y,
        transformed.InsertPoint3D.Z
      );
      insertPoint.applyMatrix4(matrix);
      transformed.InsertPoint3D = { X: insertPoint.x, Y: insertPoint.y, Z: insertPoint.z };
    }

    if (transformed.BoundaryPoints3D) {
      transformed.BoundaryPoints3D = transformed.BoundaryPoints3D.map(v => {
        const vertex = new THREE.Vector3(v.X, v.Y, v.Z);
        vertex.applyMatrix4(matrix);
        return { X: vertex.x, Y: vertex.y, Z: vertex.z };
      });
    }

    if (transformed.Geometry?.Bounds) {
      const bounds = transformed.Geometry.Bounds;
      const min = new THREE.Vector3(bounds.Min.X, bounds.Min.Y, bounds.Min.Z);
      const max = new THREE.Vector3(bounds.Max.X, bounds.Max.Y, bounds.Max.Z);
      min.applyMatrix4(matrix);
      max.applyMatrix4(matrix);
      transformed.Geometry.Bounds = {
        Min: { X: Math.min(min.x, max.x), Y: Math.min(min.y, max.y), Z: Math.min(min.z, max.z) },
        Max: { X: Math.max(min.x, max.x), Y: Math.max(min.y, max.y), Z: Math.max(min.z, max.z) },
        Center: { X: (min.x + max.x) / 2, Y: (min.y + max.y) / 2, Z: (min.z + max.z) / 2 },
        Size: { X: Math.abs(max.x - min.x), Y: Math.abs(max.y - min.y), Z: Math.abs(max.z - min.z) }
      };
    }

    if (transformed.Geometry?.Vertices) {
      transformed.Geometry.Vertices = transformed.Geometry.Vertices.map(v => {
        const vertex = new THREE.Vector3(v.X, v.Y, v.Z);
        vertex.applyMatrix4(matrix);
        return { X: vertex.x, Y: vertex.y, Z: vertex.z };
      });
    }

    return transformed;
  }

  private static getColorFromIndex(colorIndex: number): string {
    const colorMap: { [key: number]: string } = {
      0: '#000000',
      1: '#ff0000',
      2: '#ffff00',
      3: '#00ff00',
      4: '#00ffff',
      5: '#0000ff',
      6: '#ff00ff',
      7: '#ffffff',
      8: '#808080',
      9: '#c0c0c0'
    };
    return colorMap[colorIndex] || '#ffffff';
  }
}
