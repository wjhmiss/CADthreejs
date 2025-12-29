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
  FaceCount: number;
  HasNormals: boolean;
  HasColors: boolean;
  HasUVs: boolean;
  HasIndices: boolean;
  PrimitiveType: string;
  IndexCount: number;
  IsClosed: boolean;
  IsPeriodic: boolean;
  Degree: number;
}

export interface MaterialData {
  Type: string;
  Color: number;
  Opacity: number;
  Transparent: boolean;
  Side: boolean;
}

export interface TextureData {
  ImagePath: string;
  Offset: Point3DData;
  Repeat: Point3DData;
  FlipY: boolean;
  Rotation: number;
}

export interface RasterImageData {
  CornerPoints: Point3DData[];
  InsertPoint: Point3DData;
  UVector: Point3DData;
  VVector: Point3DData;
  Size: Point3DData;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  Brightness: number;
  Contrast: number;
  Fade: number;
  Flags: number;
  ClipType: number;
  ClippingState: boolean;
  DefinitionFileName: string;
  DefinitionSize: Point3DData;
  
  Bounds: BoundsData;
  Centroid: Point3DData;
  Area: number;
  
  Transform: TransformData;
  Geometry: GeometryData;
  Material: MaterialData;
  Texture: TextureData;
  VertexPositions: number[];
  UVCoordinates: number[];
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
  
  Color: ColorData;
}

export class RasterImageEntityThreejsRenderer {
  private static readonly rasterImageCache: Map<string, THREE.Group> = new Map();
  private static readonly textureLoader: THREE.TextureLoader = new THREE.TextureLoader();
  private static readonly textureCache: Map<string, THREE.Texture> = new Map();

  public static render(rasterImageData: RasterImageData, scene: THREE.Scene): THREE.Group | null {
    if (!rasterImageData || !rasterImageData.Visible) {
      return null;
    }

    const group = new THREE.Group();
    group.name = `RasterImage_${rasterImageData.Handle}`;
    group.visible = rasterImageData.Visible;

    if (rasterImageData.CornerPoints.length === 0) {
      console.warn(`RasterImage ${rasterImageData.Handle} has no corner points`);
      return group;
    }

    const geometry = this.createRasterImageGeometry(rasterImageData);
    const material = this.createRasterImageMaterial(rasterImageData);
    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = 'RasterImage';
    mesh.userData = { handle: rasterImageData.Handle };
    mesh.visible = rasterImageData.Visible;

    mesh.castShadow = rasterImageData.CastShadows;
    mesh.receiveShadow = rasterImageData.ReceiveShadows;

    this.applyTransform(mesh, rasterImageData.Transform);

    group.add(mesh);

    this.rasterImageCache.set(rasterImageData.Handle, group);

    return group;
  }

  public static update(rasterImageData: RasterImageData, scene: THREE.Scene): THREE.Group {
    this.dispose(rasterImageData.Handle);
    return this.render(rasterImageData, scene);
  }

  public static dispose(handle: string): void {
    const cachedGroup = this.rasterImageCache.get(handle);
    if (cachedGroup) {
      cachedGroup.traverse((child) => {
        if (child instanceof THREE.Mesh) {
          if (child.geometry) {
            child.geometry.dispose();
          }
          if (child.material) {
            if (child.material instanceof THREE.Material) {
              if ('map' in child.material && child.material.map) {
                child.material.map.dispose();
              }
              child.material.dispose();
            }
          }
        }
      });
      this.rasterImageCache.delete(handle);
    }
  }

  public static disposeAll(): void {
    this.rasterImageCache.forEach((group, handle) => {
      this.dispose(handle);
    });
    this.textureCache.forEach((texture) => {
      texture.dispose();
    });
    this.textureCache.clear();
  }

  private static createRasterImageGeometry(rasterImageData: RasterImageData): THREE.BufferGeometry {
    const geometry = new THREE.BufferGeometry();

    const positions = new Float32Array(rasterImageData.VertexPositions);
    const uvs = new Float32Array(rasterImageData.UVCoordinates);
    const indices = new Uint16Array(rasterImageData.Indices);

    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    geometry.setAttribute('uv', new THREE.BufferAttribute(uvs, 2));
    geometry.setIndex(new THREE.BufferAttribute(indices, 1));

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static createRasterImageMaterial(rasterImageData: RasterImageData): THREE.Material {
    let texture: THREE.Texture | null = null;

    if (rasterImageData.Texture.ImagePath) {
      texture = this.loadTexture(rasterImageData.Texture.ImagePath);
    }

    const material = new THREE.MeshBasicMaterial({
      color: rasterImageData.Material.Color,
      transparent: rasterImageData.Material.Transparent,
      opacity: rasterImageData.Material.Opacity,
      side: rasterImageData.Material.Side ? THREE.DoubleSide : THREE.FrontSide,
      map: texture,
      depthTest: true,
      depthWrite: true
    });

    if (texture) {
      texture.offset.set(rasterImageData.Texture.Offset.X, rasterImageData.Texture.Offset.Y);
      texture.repeat.set(rasterImageData.Texture.Repeat.X, rasterImageData.Texture.Repeat.Y);
      texture.center.set(0.5, 0.5);
      texture.rotation = rasterImageData.Texture.Rotation;
      texture.flipY = rasterImageData.Texture.FlipY;
      texture.needsUpdate = true;

      this.applyImageEffects(material, rasterImageData.Brightness, rasterImageData.Contrast, rasterImageData.Fade);
    }

    return material;
  }

  private static loadTexture(imagePath: string): THREE.Texture | null {
    if (!imagePath) {
      return null;
    }

    const cachedTexture = this.textureCache.get(imagePath);
    if (cachedTexture) {
      return cachedTexture;
    }

    try {
      const texture = this.textureLoader.load(imagePath);
      this.textureCache.set(imagePath, texture);
      return texture;
    } catch (error) {
      console.error(`Failed to load texture from ${imagePath}:`, error);
      return null;
    }
  }

  private static applyImageEffects(material: THREE.Material, brightness: number, contrast: number, fade: number): void {
    if (material instanceof THREE.MeshBasicMaterial) {
      material.opacity = fade / 255.0;
      material.transparent = fade < 255;

      if (material.map) {
        material.map.colorSpace = THREE.SRGBColorSpace;
      }
    }
  }

  private static applyTransform(mesh: THREE.Mesh, transform: TransformData): void {
    mesh.position.set(transform.Position.X, transform.Position.Y, transform.Position.Z);
    mesh.rotation.set(transform.Rotation.X, transform.Rotation.Y, transform.Rotation.Z);
    mesh.scale.set(transform.Scale.X, transform.Scale.Y, transform.Scale.Z);

    if (transform.Matrix && transform.Matrix.length === 16) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(transform.Matrix);
      mesh.applyMatrix4(matrix);
    }
  }

  public static getRasterImageByHandle(handle: string): THREE.Group | null {
    return this.rasterImageCache.get(handle) || null;
  }

  public static getRasterImageCount(): number {
    return this.rasterImageCache.size;
  }

  public static getTextureCount(): number {
    return this.textureCache.size;
  }

  public static setVisibility(handle: string, visible: boolean): void {
    const group = this.rasterImageCache.get(handle);
    if (group) {
      group.visible = visible;
      group.traverse((child) => {
        if (child instanceof THREE.Mesh) {
          child.visible = visible;
        }
      });
    }
  }

  public static setOpacity(handle: string, opacity: number): void {
    const group = this.rasterImageCache.get(handle);
    if (group) {
      group.traverse((child) => {
        if (child instanceof THREE.Mesh && child.material instanceof THREE.MeshBasicMaterial) {
          child.material.opacity = opacity;
          child.material.transparent = opacity < 1.0;
          child.material.needsUpdate = true;
        }
      });
    }
  }

  public static setBrightness(handle: string, brightness: number): void {
    const group = this.rasterImageCache.get(handle);
    if (group) {
      group.traverse((child) => {
        if (child instanceof THREE.Mesh && child.material instanceof THREE.MeshBasicMaterial) {
          if (child.material.map) {
            child.material.map.colorSpace = brightness > 50 ? THREE.SRGBColorSpace : THREE.LinearSRGBColorSpace;
            child.material.needsUpdate = true;
          }
        }
      });
    }
  }

  public static setContrast(handle: string, contrast: number): void {
    const group = this.rasterImageCache.get(handle);
    if (group) {
      group.traverse((child) => {
        if (child instanceof THREE.Mesh && child.material instanceof THREE.MeshBasicMaterial) {
          if (child.material.map) {
            child.material.needsUpdate = true;
          }
        }
      });
    }
  }

  public static getBounds(handle: string): THREE.Box3 | null {
    const group = this.rasterImageCache.get(handle);
    if (group) {
      const box = new THREE.Box3();
      group.traverse((child) => {
        if (child instanceof THREE.Mesh) {
          box.expandByObject(child);
        }
      });
      return box;
    }
    return null;
  }

  public static getCentroid(handle: string): THREE.Vector3 | null {
    const bounds = this.getBounds(handle);
    if (bounds) {
      return new THREE.Vector3().addVectors(bounds.min, bounds.max).multiplyScalar(0.5);
    }
    return null;
  }

  public static getArea(handle: string): number {
    const group = this.rasterImageCache.get(handle);
    if (group) {
      let area = 0;
      group.traverse((child) => {
        if (child instanceof THREE.Mesh && child.geometry) {
          const positions = child.geometry.attributes.position;
          if (positions && positions.count >= 4) {
            const v0 = new THREE.Vector3(positions.getX(0), positions.getY(0), positions.getZ(0));
            const v1 = new THREE.Vector3(positions.getX(1), positions.getY(1), positions.getZ(1));
            const v2 = new THREE.Vector3(positions.getX(2), positions.getY(2), positions.getZ(2));
            const v3 = new THREE.Vector3(positions.getX(3), positions.getY(3), positions.getZ(3));

            const triangle1 = this.calculateTriangleArea(v0, v1, v2);
            const triangle2 = this.calculateTriangleArea(v0, v2, v3);
            area += triangle1 + triangle2;
          }
        }
      });
      return area;
    }
    return 0;
  }

  private static calculateTriangleArea(v0: THREE.Vector3, v1: THREE.Vector3, v2: THREE.Vector3): number {
    const edge1 = new THREE.Vector3().subVectors(v1, v0);
    const edge2 = new THREE.Vector3().subVectors(v2, v0);
    const cross = new THREE.Vector3().crossVectors(edge1, edge2);
    return cross.length() * 0.5;
  }

  public static getCornerPoints(handle: string): THREE.Vector3[] | null {
    const group = this.rasterImageCache.get(handle);
    if (group) {
      const corners: THREE.Vector3[] = [];
      group.traverse((child) => {
        if (child instanceof THREE.Mesh && child.geometry) {
          const positions = child.geometry.attributes.position;
          if (positions && positions.count >= 4) {
            for (let i = 0; i < 4; i++) {
              corners.push(new THREE.Vector3(
                positions.getX(i),
                positions.getY(i),
                positions.getZ(i)
              ));
            }
          }
        }
      });
      return corners;
    }
    return null;
  }

  public static isRasterImageVisible(handle: string): boolean {
    const group = this.rasterImageCache.get(handle);
    return group ? group.visible : false;
  }

  public static hasTexture(handle: string): boolean {
    const group = this.rasterImageCache.get(handle);
    if (group) {
      let hasTexture = false;
      group.traverse((child) => {
        if (child instanceof THREE.Mesh && child.material instanceof THREE.MeshBasicMaterial) {
          hasTexture = child.material.map !== null;
        }
      });
      return hasTexture;
    }
    return false;
  }

  public static getTexturePath(handle: string): string | null {
    const group = this.rasterImageCache.get(handle);
    if (group) {
      let texturePath: string | null = null;
      group.traverse((child) => {
        if (child instanceof THREE.Mesh && child.material instanceof THREE.MeshBasicMaterial) {
          if (child.material.map && child.material.map instanceof THREE.Texture) {
            texturePath = child.material.map.source.data?.src || null;
          }
        }
      });
      return texturePath;
    }
    return null;
  }
}
