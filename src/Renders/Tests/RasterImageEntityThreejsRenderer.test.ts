import * as THREE from 'three';
import { RasterImageEntityThreejsRenderer, RasterImageData, Point3DData, ColorData, TransformData, BoundsData, MaterialData, GeometryData, TextureData } from '../RasterImageEntityThreejsRenderer';

describe('RasterImageEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    RasterImageEntityThreejsRenderer.clearCache();
  });

  const createBasicRasterImageData = (): RasterImageData => ({
    CornerPoints: [
      { X: 0, Y: 0, Z: 0 },
      { X: 100, Y: 0, Z: 0 },
      { X: 100, Y: 100, Z: 0 },
      { X: 0, Y: 100, Z: 0 }
    ],
    InsertPoint: { X: 0, Y: 0, Z: 0 },
    UVector: { X: 1, Y: 0, Z: 0 },
    VVector: { X: 0, Y: 1, Z: 0 },
    Size: { X: 100, Y: 100, Z: 0 },
    ColorIndex: 7,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.03,
    Brightness: 50,
    Contrast: 50,
    Fade: 0,
    Flags: 1,
    ClipType: 0,
    ClippingState: false,
    DefinitionFileName: 'image.png',
    DefinitionSize: { X: 100, Y: 100, Z: 0 },
    
    Bounds: {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 100, Y: 100, Z: 0 }
    },
    Centroid: { X: 50, Y: 50, Z: 0 },
    Area: 10000,
    
    Transform: {
      Position: { X: 0, Y: 0, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
    },
    Geometry: {
      Type: 'BufferGeometry',
      Position: { X: 0, Y: 0, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      BoundingBox: {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 100, Y: 100, Z: 0 }
      }
    },
    Material: {
      Type: 'MeshBasicMaterial',
      Color: { Index: 7, Hex: '#FFFFFF', R: 255, G: 255, B: 255, A: 1.0 },
      Opacity: 1.0,
      Transparent: false,
      Side: 2,
      DepthTest: true,
      DepthWrite: true
    },
    Texture: {
      ImagePath: 'image.png',
      Offset: { X: 0, Y: 0 },
      Repeat: { X: 1, Y: 1 },
      Rotation: 0,
      FlipY: true,
      WrapS: 1001,
      WrapT: 1001
    },
    VertexPositions: [0, 0, 0, 100, 0, 0, 100, 100, 0, 0, 100, 0],
    UVCoordinates: [0, 0, 1, 0, 1, 1, 0, 1],
    Indices: [0, 1, 2, 0, 2, 3],
    
    EntityType: 'RasterImage',
    Visible: true,
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Handle: 'rasterimage-handle-1',
    
    Transparency: 0,
    MaterialName: 'DEFAULT',
    
    CastShadows: false,
    ReceiveShadows: false,
    
    Color: { Index: 7, Hex: '#FFFFFF', R: 255, G: 255, B: 255, A: 1.0 }
  });

  describe('render', () => {
    it('should render a basic RasterImage', () => {
      const rasterImageData = createBasicRasterImageData();

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.name).toBe('RasterImage_rasterimage-handle-1');
    });

    it('should not add invisible RasterImage to scene', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.Visible = false;

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.visible).toBe(false);
      expect(scene.children.length).toBe(0);
    });

    it('should render RasterImage with correct Mesh object', () => {
      const rasterImageData = createBasicRasterImageData();

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      expect(group.children.length).toBe(1);
      const mesh = group.children[0] as THREE.Mesh;
      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.name).toBe('RasterImage');
    });

    it('should render RasterImage with correct material', () => {
      const rasterImageData = createBasicRasterImageData();

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.material).toBeDefined();
      expect(mesh.material).toBeInstanceOf(THREE.MeshBasicMaterial);
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.color.getHexString()).toBe('ffffff');
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
      expect(material.side).toBe(THREE.DoubleSide);
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(true);
    });

    it('should render RasterImage with correct geometry', () => {
      const rasterImageData = createBasicRasterImageData();

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.geometry).toBeDefined();
      expect(mesh.geometry).toBeInstanceOf(THREE.BufferGeometry);
      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position).toBeDefined();
      expect(geometry.attributes.position.count).toBe(4);
    });

    it('should render RasterImage with correct corner points', () => {
      const rasterImageData = createBasicRasterImageData();

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(0);
      expect(positions[1]).toBe(0);
      expect(positions[2]).toBe(0);
      expect(positions[3]).toBe(100);
      expect(positions[4]).toBe(0);
      expect(positions[5]).toBe(0);
      expect(positions[6]).toBe(100);
      expect(positions[7]).toBe(100);
      expect(positions[8]).toBe(0);
      expect(positions[9]).toBe(0);
      expect(positions[10]).toBe(100);
      expect(positions[11]).toBe(0);
    });

    it('should render RasterImage with correct UV coordinates', () => {
      const rasterImageData = createBasicRasterImageData();

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.uv).toBeDefined();
      const uvs = geometry.attributes.uv.array as Float32Array;
      expect(uvs[0]).toBe(0);
      expect(uvs[1]).toBe(0);
      expect(uvs[2]).toBe(1);
      expect(uvs[3]).toBe(0);
      expect(uvs[4]).toBe(1);
      expect(uvs[5]).toBe(1);
      expect(uvs[6]).toBe(0);
      expect(uvs[7]).toBe(1);
    });

    it('should render RasterImage with correct indices', () => {
      const rasterImageData = createBasicRasterImageData();

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.index).toBeDefined();
      const indices = geometry.index.array as Uint16Array;
      expect(indices[0]).toBe(0);
      expect(indices[1]).toBe(1);
      expect(indices[2]).toBe(2);
      expect(indices[3]).toBe(0);
      expect(indices[4]).toBe(2);
      expect(indices[5]).toBe(3);
    });

    it('should render RasterImage with correct color', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.Color = { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 };
      rasterImageData.Material.Color = { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 };

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.color.getHexString()).toBe('ff0000');
    });

    it('should render RasterImage with correct opacity', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.Transparency = 0.5;
      rasterImageData.Material.Opacity = 0.5;
      rasterImageData.Material.Transparent = true;

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should render RasterImage with correct depth settings', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.Material.DepthTest = false;
      rasterImageData.Material.DepthWrite = false;

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.depthTest).toBe(false);
      expect(material.depthWrite).toBe(false);
    });

    it('should render RasterImage with correct side', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.Material.Side = 0;

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.side).toBe(THREE.FrontSide);
    });

    it('should render RasterImage with correct shadow settings', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.CastShadows = true;
      rasterImageData.ReceiveShadows = true;

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.castShadow).toBe(true);
      expect(mesh.receiveShadow).toBe(true);
    });

    it('should cache RasterImage by handle', () => {
      const rasterImageData = createBasicRasterImageData();

      RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      const cachedGroup = (RasterImageEntityThreejsRenderer as any).rasterImageCache.get('rasterimage-handle-1');
      expect(cachedGroup).toBeDefined();
      expect(cachedGroup).toBeInstanceOf(THREE.Group);
    });

    it('should render RasterImage with zero size', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.Size = { X: 0, Y: 0, Z: 0 };
      rasterImageData.CornerPoints = [
        { X: 0, Y: 0, Z: 0 },
        { X: 0, Y: 0, Z: 0 },
        { X: 0, Y: 0, Z: 0 },
        { X: 0, Y: 0, Z: 0 }
      ];

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render RasterImage with negative coordinates', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.InsertPoint = { X: -10, Y: -20, Z: -30 };
      rasterImageData.CornerPoints = [
        { X: -10, Y: -20, Z: -30 },
        { X: 90, Y: -20, Z: -30 },
        { X: 90, Y: 80, Z: -30 },
        { X: -10, Y: 80, Z: -30 }
      ];

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(-10);
      expect(positions[1]).toBe(-20);
      expect(positions[2]).toBe(-30);
    });

    it('should render RasterImage with very large coordinates', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.InsertPoint = { X: 1000000, Y: 2000000, Z: 3000000 };
      rasterImageData.CornerPoints = [
        { X: 1000000, Y: 2000000, Z: 3000000 },
        { X: 1000100, Y: 2000000, Z: 3000000 },
        { X: 1000100, Y: 2000100, Z: 3000000 },
        { X: 1000000, Y: 2000100, Z: 3000000 }
      ];

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(1000000);
      expect(positions[1]).toBe(2000000);
      expect(positions[2]).toBe(3000000);
    });

    it('should render RasterImage with correct handle in userData', () => {
      const rasterImageData = createBasicRasterImageData();

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.userData.handle).toBe('rasterimage-handle-1');
    });

    it('should handle RasterImage with no corner points', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.CornerPoints = [];

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBe(0);
    });
  });

  describe('update', () => {
    it('should update existing RasterImage', () => {
      const rasterImageData = createBasicRasterImageData();
      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      const updatedRasterImageData = { ...rasterImageData };
      updatedRasterImageData.InsertPoint = { X: 20, Y: 20, Z: 0 };

      const result = RasterImageEntityThreejsRenderer.update(updatedRasterImageData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent RasterImage', () => {
      const rasterImageData = createBasicRasterImageData();

      const result = RasterImageEntityThreejsRenderer.update(rasterImageData, scene);

      expect(result).toBe(false);
    });

    it('should update RasterImage insert point', () => {
      const rasterImageData = createBasicRasterImageData();
      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      const updatedRasterImageData = { ...rasterImageData };
      updatedRasterImageData.InsertPoint = { X: 100, Y: 200, Z: 50 };

      const result = RasterImageEntityThreejsRenderer.update(updatedRasterImageData, scene);

      expect(result).toBe(true);
    });

    it('should update RasterImage color', () => {
      const rasterImageData = createBasicRasterImageData();
      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      const updatedRasterImageData = { ...rasterImageData };
      updatedRasterImageData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };
      updatedRasterImageData.Material.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const result = RasterImageEntityThreejsRenderer.update(updatedRasterImageData, scene);

      expect(result).toBe(true);
    });

    it('should update RasterImage opacity', () => {
      const rasterImageData = createBasicRasterImageData();
      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      const updatedRasterImageData = { ...rasterImageData };
      updatedRasterImageData.Transparency = 0.5;
      updatedRasterImageData.Material.Opacity = 0.5;
      updatedRasterImageData.Material.Transparent = true;

      const result = RasterImageEntityThreejsRenderer.update(updatedRasterImageData, scene);

      expect(result).toBe(true);
    });
  });

  describe('dispose', () => {
    it('should dispose RasterImage resources', () => {
      const rasterImageData = createBasicRasterImageData();
      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      RasterImageEntityThreejsRenderer.dispose(rasterImageData, scene);

      expect(scene.children.length).toBe(0);
    });

    it('should handle non-existent RasterImage gracefully', () => {
      const rasterImageData = createBasicRasterImageData();

      expect(() => {
        RasterImageEntityThreejsRenderer.dispose(rasterImageData, scene);
      }).not.toThrow();
    });

    it('should remove RasterImage from cache', () => {
      const rasterImageData = createBasicRasterImageData();
      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      RasterImageEntityThreejsRenderer.dispose(rasterImageData, scene);

      const cachedGroup = (RasterImageEntityThreejsRenderer as any).rasterImageCache.get('rasterimage-handle-1');
      expect(cachedGroup).toBeUndefined();
    });

    it('should dispose RasterImage geometry', () => {
      const rasterImageData = createBasicRasterImageData();
      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry;
      const originalDispose = geometry.dispose.bind(geometry);
      let disposeCalled = false;
      geometry.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      RasterImageEntityThreejsRenderer.dispose(rasterImageData, scene);

      expect(disposeCalled).toBe(true);
    });

    it('should dispose RasterImage material', () => {
      const rasterImageData = createBasicRasterImageData();
      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material;
      const originalDispose = material.dispose.bind(material);
      let disposeCalled = false;
      material.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      RasterImageEntityThreejsRenderer.dispose(rasterImageData, scene);

      expect(disposeCalled).toBe(true);
    });

    it('should dispose RasterImage texture if exists', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.Texture.ImagePath = 'test.png';

      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);
      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;

      if (material.map) {
        const texture = material.map;
        const originalDispose = texture.dispose.bind(texture);
        let disposeCalled = false;
        texture.dispose = () => {
          disposeCalled = true;
          originalDispose();
        };

        RasterImageEntityThreejsRenderer.dispose(rasterImageData, scene);

        expect(disposeCalled).toBe(true);
      }
    });
  });

  describe('setVisibility', () => {
    it('should set RasterImage visibility to true', () => {
      const rasterImageData = createBasicRasterImageData();
      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      RasterImageEntityThreejsRenderer.setVisibility(rasterImageData, scene, true);

      expect(group.visible).toBe(true);
    });

    it('should set RasterImage visibility to false', () => {
      const rasterImageData = createBasicRasterImageData();
      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      RasterImageEntityThreejsRenderer.setVisibility(rasterImageData, scene, false);

      expect(group.visible).toBe(false);
    });

    it('should handle non-existent RasterImage gracefully', () => {
      const rasterImageData = createBasicRasterImageData();

      expect(() => {
        RasterImageEntityThreejsRenderer.setVisibility(rasterImageData, scene, true);
      }).not.toThrow();
    });

    it('should add RasterImage to scene when setting visibility to true', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.Visible = false;
      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      RasterImageEntityThreejsRenderer.setVisibility(rasterImageData, scene, true);

      expect(scene.children.length).toBe(1);
    });

    it('should remove RasterImage from scene when setting visibility to false', () => {
      const rasterImageData = createBasicRasterImageData();
      const group = RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      RasterImageEntityThreejsRenderer.setVisibility(rasterImageData, scene, false);

      expect(scene.children.length).toBe(0);
    });
  });

  describe('getInsertPoint', () => {
    it('should return correct insert point', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.InsertPoint = { X: 100, Y: 200, Z: 50 };

      RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      const insertPoint = RasterImageEntityThreejsRenderer.getInsertPoint('rasterimage-handle-1');

      expect(insertPoint).toBeDefined();
      expect(insertPoint.x).toBe(100);
      expect(insertPoint.y).toBe(200);
      expect(insertPoint.z).toBe(50);
    });

    it('should return null for non-existent RasterImage', () => {
      const insertPoint = RasterImageEntityThreejsRenderer.getInsertPoint('non-existent-handle');

      expect(insertPoint).toBeNull();
    });
  });

  describe('getUVector', () => {
    it('should return correct U vector', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.UVector = { X: 1, Y: 0, Z: 0 };

      RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      const uVector = RasterImageEntityThreejsRenderer.getUVector('rasterimage-handle-1');

      expect(uVector).toBeDefined();
      expect(uVector.x).toBe(1);
      expect(uVector.y).toBe(0);
      expect(uVector.z).toBe(0);
    });

    it('should return null for non-existent RasterImage', () => {
      const uVector = RasterImageEntityThreejsRenderer.getUVector('non-existent-handle');

      expect(uVector).toBeNull();
    });
  });

  describe('getVVector', () => {
    it('should return correct V vector', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.VVector = { X: 0, Y: 1, Z: 0 };

      RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      const vVector = RasterImageEntityThreejsRenderer.getVVector('rasterimage-handle-1');

      expect(vVector).toBeDefined();
      expect(vVector.x).toBe(0);
      expect(vVector.y).toBe(1);
      expect(vVector.z).toBe(0);
    });

    it('should return null for non-existent RasterImage', () => {
      const vVector = RasterImageEntityThreejsRenderer.getVVector('non-existent-handle');

      expect(vVector).toBeNull();
    });
  });

  describe('getSize', () => {
    it('should return correct size', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.Size = { X: 100, Y: 200, Z: 0 };

      RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      const size = RasterImageEntityThreejsRenderer.getSize('rasterimage-handle-1');

      expect(size).toBeDefined();
      expect(size.x).toBe(100);
      expect(size.y).toBe(200);
      expect(size.z).toBe(0);
    });

    it('should return null for non-existent RasterImage', () => {
      const size = RasterImageEntityThreejsRenderer.getSize('non-existent-handle');

      expect(size).toBeNull();
    });
  });

  describe('getCornerPoints', () => {
    it('should return correct corner points', () => {
      const rasterImageData = createBasicRasterImageData();

      RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      const cornerPoints = RasterImageEntityThreejsRenderer.getCornerPoints('rasterimage-handle-1');

      expect(cornerPoints).toBeDefined();
      expect(cornerPoints.length).toBe(4);
      expect(cornerPoints[0].x).toBe(0);
      expect(cornerPoints[0].y).toBe(0);
      expect(cornerPoints[0].z).toBe(0);
      expect(cornerPoints[1].x).toBe(100);
      expect(cornerPoints[1].y).toBe(0);
      expect(cornerPoints[1].z).toBe(0);
      expect(cornerPoints[2].x).toBe(100);
      expect(cornerPoints[2].y).toBe(100);
      expect(cornerPoints[2].z).toBe(0);
      expect(cornerPoints[3].x).toBe(0);
      expect(cornerPoints[3].y).toBe(100);
      expect(cornerPoints[3].z).toBe(0);
    });

    it('should return null for non-existent RasterImage', () => {
      const cornerPoints = RasterImageEntityThreejsRenderer.getCornerPoints('non-existent-handle');

      expect(cornerPoints).toBeNull();
    });
  });

  describe('getArea', () => {
    it('should return correct area', () => {
      const rasterImageData = createBasicRasterImageData();
      rasterImageData.Area = 10000;

      RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      const area = RasterImageEntityThreejsRenderer.getArea('rasterimage-handle-1');

      expect(area).toBe(10000);
    });

    it('should return null for non-existent RasterImage', () => {
      const area = RasterImageEntityThreejsRenderer.getArea('non-existent-handle');

      expect(area).toBeNull();
    });
  });

  describe('clearCache', () => {
    it('should clear all cached RasterImages', () => {
      const rasterImageData = createBasicRasterImageData();
      RasterImageEntityThreejsRenderer.render(rasterImageData, scene);

      RasterImageEntityThreejsRenderer.clearCache();

      const cachedGroup = (RasterImageEntityThreejsRenderer as any).rasterImageCache.get('rasterimage-handle-1');
      expect(cachedGroup).toBeUndefined();
    });

    it('should clear cache and remove all RasterImages from scene', () => {
      const rasterImageData1 = createBasicRasterImageData();
      rasterImageData1.Handle = 'rasterimage-1';
      const rasterImageData2 = createBasicRasterImageData();
      rasterImageData2.Handle = 'rasterimage-2';

      RasterImageEntityThreejsRenderer.render(rasterImageData1, scene);
      RasterImageEntityThreejsRenderer.render(rasterImageData2, scene);

      expect(scene.children.length).toBe(2);

      RasterImageEntityThreejsRenderer.clearCache();

      expect(scene.children.length).toBe(0);
    });
  });
});
