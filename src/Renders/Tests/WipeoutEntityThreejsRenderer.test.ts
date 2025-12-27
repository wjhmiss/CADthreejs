import * as THREE from 'three';
import { WipeoutEntityThreejsRenderer, WipeoutData, Point3DData, ColorData, TransformData, NormalData, BoundsData, GeometryData, MaterialData, ClipType, ImageDisplayFlags } from '../WipeoutEntityThreejsRenderer';

describe('WipeoutEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    WipeoutEntityThreejsRenderer.clearCache();
  });

  const createBasicWipeoutData = (): WipeoutData => ({
    Handle: 'wipeout-handle-1',
    Visible: true,
    BoundaryPoints: [
      { X: 0, Y: 0, Z: 0 },
      { X: 10, Y: 0, Z: 0 },
      { X: 10, Y: 10, Z: 0 },
      { X: 0, Y: 10, Z: 0 }
    ],
    InsertPoint: { X: 0, Y: 0, Z: 0 },
    UVector: { X: 1, Y: 0, Z: 0 },
    VVector: { X: 0, Y: 1, Z: 0 },
    Size: { X: 10, Y: 10, Z: 0 },
    ClipType: ClipType.Rectangular,
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.03,
    Brightness: 50,
    Contrast: 50,
    Fade: 0,
    Flags: ImageDisplayFlags.ShowImage,
    ClippingState: true,
    Bounds: {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 10, Y: 10, Z: 0 }
    },
    Centroid: { X: 5, Y: 5, Z: 0 },
    BoundaryPointCount: 4,
    Area: 100,
    Transform: {
      Position: { X: 0, Y: 0, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
    },
    Geometry: {
      Type: 'PolygonGeometry',
      VertexCount: 4,
      FaceCount: 2,
      HasNormals: true,
      HasColors: true,
      HasUVs: true,
      HasIndices: true,
      PrimitiveType: 'Triangles',
      IndexCount: 6,
      IsClosed: true,
      IsPeriodic: false
    },
    Material: {
      Type: 'MeshBasicMaterial',
      Color: 16711680,
      Opacity: 1.0,
      Transparent: false,
      Wireframe: false,
      LineWidth: 1.0,
      VertexColors: true,
      Side: true
    },
    VertexPositions: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
    VertexColors: [1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0],
    VertexNormals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
    UV: [0, 0, 1, 0, 1, 1, 0, 1],
    Indices: [0, 1, 2, 0, 2, 3],
    VertexCount: 4,
    Opacity: 1.0,
    Transparent: false,
    IsMask: true,
    Normal: { X: 0, Y: 0, Z: 1 },
    Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 }
  });

  describe('render', () => {
    it('should render a basic Wipeout', () => {
      const wipeoutData = createBasicWipeoutData();

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.name).toBe('Wipeout_wipeout-handle-1');
    });

    it('should not add invisible Wipeout to scene', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Visible = false;

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.visible).toBe(false);
      expect(scene.children.length).toBe(0);
    });

    it('should render Wipeout with correct Mesh object', () => {
      const wipeoutData = createBasicWipeoutData();

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      expect(group.children.length).toBe(1);
      const mesh = group.children[0] as THREE.Mesh;
      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.name).toBe('Wipeout');
    });

    it('should render Wipeout with correct material', () => {
      const wipeoutData = createBasicWipeoutData();

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.material).toBeDefined();
      expect(mesh.material).toBeInstanceOf(THREE.MeshBasicMaterial);
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.color.getHexString()).toBe('ff0000');
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
      expect(material.depthTest).toBe(true);
      expect(material.side).toBe(THREE.DoubleSide);
    });

    it('should render Wipeout with correct geometry', () => {
      const wipeoutData = createBasicWipeoutData();

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.geometry).toBeDefined();
      expect(mesh.geometry).toBeInstanceOf(THREE.BufferGeometry);
      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position).toBeDefined();
      expect(geometry.attributes.position.count).toBe(4);
    });

    it('should render Wipeout with correct vertex positions', () => {
      const wipeoutData = createBasicWipeoutData();

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(0);
      expect(positions[1]).toBe(0);
      expect(positions[2]).toBe(0);
      expect(positions[3]).toBe(10);
      expect(positions[4]).toBe(0);
      expect(positions[5]).toBe(0);
    });

    it('should render Wipeout with correct color', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };
      wipeoutData.Material.Color = 0x00FF00;

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should render Wipeout with correct opacity', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Opacity = 0.5;
      wipeoutData.Transparent = true;

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should cache Wipeout by handle', () => {
      const wipeoutData = createBasicWipeoutData();

      WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      const cachedMesh = (WipeoutEntityThreejsRenderer as any).wipeoutCache.get('wipeout-handle-1');
      expect(cachedMesh).toBeDefined();
      expect(cachedMesh).toBeInstanceOf(THREE.Mesh);
    });

    it('should render Wipeout with correct handle in userData', () => {
      const wipeoutData = createBasicWipeoutData();

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.userData.handle).toBe('wipeout-handle-1');
      expect(mesh.userData.isMask).toBe(true);
    });

    it('should render Wipeout with default geometry when vertex positions not provided', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.VertexPositions = [];

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(0);
      expect(positions[1]).toBe(0);
      expect(positions[2]).toBe(0);
    });

    it('should render Wipeout with vertex colors', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.VertexColors = [1, 0, 0, 0, 1, 0];

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.color).toBeDefined();
      const colors = geometry.attributes.color.array as Float32Array;
      expect(colors[0]).toBeCloseTo(1.0);
      expect(colors[1]).toBeCloseTo(0.0);
      expect(colors[2]).toBeCloseTo(0.0);
    });

    it('should render Wipeout with vertex normals', () => {
      const wipeoutData = createBasicWipeoutData();

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.normal).toBeDefined();
      const normals = geometry.attributes.normal.array as Float32Array;
      expect(normals[0]).toBeCloseTo(0.0);
      expect(normals[1]).toBeCloseTo(0.0);
      expect(normals[2]).toBeCloseTo(1.0);
    });

    it('should render Wipeout with indices', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Indices = [0, 1, 2, 0, 2, 3];

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.index).toBeDefined();
      const indices = geometry.index.array as Uint16Array;
      expect(indices[0]).toBe(0);
      expect(indices[1]).toBe(1);
      expect(indices[2]).toBe(2);
    });

    it('should render Wipeout with UV coordinates', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.UV = [0, 0, 1, 0, 1, 1, 0, 1];

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.uv).toBeDefined();
      const uvs = geometry.attributes.uv.array as Float32Array;
      expect(uvs[0]).toBe(0);
      expect(uvs[1]).toBe(0);
      expect(uvs[2]).toBe(1);
    });

    it('should render Wipeout with wireframe material', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Material.Wireframe = true;

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.wireframe).toBe(true);
    });

    it('should render Wipeout with negative coordinates', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.VertexPositions = [-10, -20, -30, -40, -50, -60];

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(-10);
      expect(positions[1]).toBe(-20);
      expect(positions[2]).toBe(-30);
    });

    it('should render Wipeout with very large coordinates', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.VertexPositions = [1000000, 2000000, 3000000, 4000000, 5000000, 6000000];

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(1000000);
      expect(positions[1]).toBe(2000000);
      expect(positions[2]).toBe(3000000);
    });

    it('should render Wipeout with very small coordinates', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.VertexPositions = [0.0001, 0.0002, 0.0003, 0.0004, 0.0005, 0.0006];

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBeCloseTo(0.0001, 7);
      expect(positions[1]).toBeCloseTo(0.0002, 7);
      expect(positions[2]).toBeCloseTo(0.0003, 7);
    });

    it('should render Wipeout with different clip type', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.ClipType = ClipType.Polygonal;

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render Wipeout with different image display flags', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Flags = ImageDisplayFlags.ShowImage | ImageDisplayFlags.UseClipping;

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render Wipeout with clipping state disabled', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.ClippingState = false;

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render Wipeout with different brightness', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Brightness = 75;

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render Wipeout with different contrast', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Contrast = 80;

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render Wipeout with different fade', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Fade = 30;

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render Wipeout with different U vector', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.UVector = { X: 0.707, Y: 0.707, Z: 0 };

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render Wipeout with different V vector', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.VVector = { X: -0.707, Y: 0.707, Z: 0 };

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render Wipeout with different size', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Size = { X: 20, Y: 30, Z: 0 };

      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });
  });

  describe('renderMultiple', () => {
    it('should render multiple Wipeouts', () => {
      const wipeoutDataArray = [
        createBasicWipeoutData(),
        createBasicWipeoutData()
      ];
      wipeoutDataArray[0].Handle = 'wipeout-1';
      wipeoutDataArray[1].Handle = 'wipeout-2';

      const group = WipeoutEntityThreejsRenderer.renderMultiple(wipeoutDataArray, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('MultipleWipeouts');
      expect(group.children.length).toBe(2);
    });

    it('should render multiple Wipeouts with correct handles', () => {
      const wipeoutDataArray = [
        createBasicWipeoutData(),
        createBasicWipeoutData(),
        createBasicWipeoutData()
      ];
      wipeoutDataArray[0].Handle = 'wipeout-1';
      wipeoutDataArray[1].Handle = 'wipeout-2';
      wipeoutDataArray[2].Handle = 'wipeout-3';

      const group = WipeoutEntityThreejsRenderer.renderMultiple(wipeoutDataArray, scene);

      expect(group.children[0].name).toBe('Wipeout_wipeout-1');
      expect(group.children[1].name).toBe('Wipeout_wipeout-2');
      expect(group.children[2].name).toBe('Wipeout_wipeout-3');
    });

    it('should render empty array of Wipeouts', () => {
      const wipeoutDataArray: WipeoutData[] = [];

      const group = WipeoutEntityThreejsRenderer.renderMultiple(wipeoutDataArray, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('MultipleWipeouts');
      expect(group.children.length).toBe(0);
    });

    it('should render single Wipeout using renderMultiple', () => {
      const wipeoutDataArray = [createBasicWipeoutData()];
      wipeoutDataArray[0].Handle = 'wipeout-1';

      const group = WipeoutEntityThreejsRenderer.renderMultiple(wipeoutDataArray, scene);

      expect(group.children.length).toBe(1);
      expect(group.children[0].name).toBe('Wipeout_wipeout-1');
    });
  });

  describe('update', () => {
    it('should update existing Wipeout', () => {
      const wipeoutData = createBasicWipeoutData();
      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      const updatedWipeoutData = { ...wipeoutData };
      updatedWipeoutData.VertexPositions = [10, 20, 30, 40, 50, 60];

      const result = WipeoutEntityThreejsRenderer.update(updatedWipeoutData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent Wipeout', () => {
      const wipeoutData = createBasicWipeoutData();

      const result = WipeoutEntityThreejsRenderer.update(wipeoutData, scene);

      expect(result).toBe(false);
    });

    it('should update Wipeout color', () => {
      const wipeoutData = createBasicWipeoutData();
      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      const updatedWipeoutData = { ...wipeoutData };
      updatedWipeoutData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };
      updatedWipeoutData.Material.Color = 0x00FF00;

      const result = WipeoutEntityThreejsRenderer.update(updatedWipeoutData, scene);

      expect(result).toBe(true);
      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should update Wipeout opacity', () => {
      const wipeoutData = createBasicWipeoutData();
      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      const updatedWipeoutData = { ...wipeoutData };
      updatedWipeoutData.Opacity = 0.5;
      updatedWipeoutData.Transparent = true;

      const result = WipeoutEntityThreejsRenderer.update(updatedWipeoutData, scene);

      expect(result).toBe(true);
      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should update Wipeout vertex positions', () => {
      const wipeoutData = createBasicWipeoutData();
      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      const updatedWipeoutData = { ...wipeoutData };
      updatedWipeoutData.VertexPositions = [100, 200, 300, 400, 500, 600];

      const result = WipeoutEntityThreejsRenderer.update(updatedWipeoutData, scene);

      expect(result).toBe(true);
      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(100);
      expect(positions[1]).toBe(200);
      expect(positions[2]).toBe(300);
    });

    it('should update Wipeout wireframe', () => {
      const wipeoutData = createBasicWipeoutData();
      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      const updatedWipeoutData = { ...wipeoutData };
      updatedWipeoutData.Material.Wireframe = true;

      const result = WipeoutEntityThreejsRenderer.update(updatedWipeoutData, scene);

      expect(result).toBe(true);
      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.wireframe).toBe(true);
    });
  });

  describe('dispose', () => {
    it('should dispose Wipeout resources', () => {
      const wipeoutData = createBasicWipeoutData();
      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      const result = WipeoutEntityThreejsRenderer.dispose(wipeoutData, scene);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(0);
    });

    it('should return false for non-existent Wipeout', () => {
      const wipeoutData = createBasicWipeoutData();

      const result = WipeoutEntityThreejsRenderer.dispose(wipeoutData, scene);

      expect(result).toBe(false);
    });

    it('should remove Wipeout from cache', () => {
      const wipeoutData = createBasicWipeoutData();
      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      WipeoutEntityThreejsRenderer.dispose(wipeoutData, scene);

      const cachedMesh = (WipeoutEntityThreejsRenderer as any).wipeoutCache.get('wipeout-handle-1');
      expect(cachedMesh).toBeUndefined();
    });

    it('should dispose Wipeout geometry', () => {
      const wipeoutData = createBasicWipeoutData();
      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry;
      const originalDispose = geometry.dispose.bind(geometry);
      let disposeCalled = false;
      geometry.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      WipeoutEntityThreejsRenderer.dispose(wipeoutData, scene);

      expect(disposeCalled).toBe(true);
    });

    it('should dispose Wipeout material', () => {
      const wipeoutData = createBasicWipeoutData();
      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material;
      const originalDispose = material.dispose.bind(material);
      let disposeCalled = false;
      material.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      WipeoutEntityThreejsRenderer.dispose(wipeoutData, scene);

      expect(disposeCalled).toBe(true);
    });
  });

  describe('setVisibility', () => {
    it('should set Wipeout visibility to true', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Visible = false;
      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      const result = WipeoutEntityThreejsRenderer.setVisibility(wipeoutData, scene, true);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(1);
    });

    it('should set Wipeout visibility to false', () => {
      const wipeoutData = createBasicWipeoutData();
      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      const result = WipeoutEntityThreejsRenderer.setVisibility(wipeoutData, scene, false);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(0);
    });

    it('should return false for non-existent Wipeout', () => {
      const wipeoutData = createBasicWipeoutData();

      const result = WipeoutEntityThreejsRenderer.setVisibility(wipeoutData, scene, true);

      expect(result).toBe(false);
    });
  });

  describe('setOpacity', () => {
    it('should set Wipeout opacity', () => {
      const wipeoutData = createBasicWipeoutData();
      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      const result = WipeoutEntityThreejsRenderer.setOpacity(wipeoutData, 0.5);

      expect(result).toBe(true);
      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.opacity).toBe(0.5);
    });

    it('should set transparent when opacity < 1', () => {
      const wipeoutData = createBasicWipeoutData();
      const group = WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      WipeoutEntityThreejsRenderer.setOpacity(wipeoutData, 0.5);

      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.transparent).toBe(true);
    });

    it('should return false for non-existent Wipeout', () => {
      const wipeoutData = createBasicWipeoutData();

      const result = WipeoutEntityThreejsRenderer.setOpacity(wipeoutData, 0.5);

      expect(result).toBe(false);
    });
  });

  describe('getWipeoutMesh', () => {
    it('should return cached Wipeout mesh', () => {
      const wipeoutData = createBasicWipeoutData();
      WipeoutEntityThreejsRenderer.render(wipeoutData, scene);

      const mesh = WipeoutEntityThreejsRenderer.getWipeoutMesh(wipeoutData);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
    });

    it('should return null for non-existent Wipeout', () => {
      const wipeoutData = createBasicWipeoutData();

      const mesh = WipeoutEntityThreejsRenderer.getWipeoutMesh(wipeoutData);

      expect(mesh).toBeNull();
    });
  });

  describe('getInsertPoint', () => {
    it('should return correct insert point', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.InsertPoint = { X: 100, Y: 200, Z: 50 };

      const insertPoint = WipeoutEntityThreejsRenderer.getInsertPoint(wipeoutData);

      expect(insertPoint.x).toBe(100);
      expect(insertPoint.y).toBe(200);
      expect(insertPoint.z).toBe(50);
    });
  });

  describe('getUVector', () => {
    it('should return correct U vector', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.UVector = { X: 1, Y: 0, Z: 0 };

      const uVector = WipeoutEntityThreejsRenderer.getUVector(wipeoutData);

      expect(uVector.x).toBe(1);
      expect(uVector.y).toBe(0);
      expect(uVector.z).toBe(0);
    });
  });

  describe('getVVector', () => {
    it('should return correct V vector', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.VVector = { X: 0, Y: 1, Z: 0 };

      const vVector = WipeoutEntityThreejsRenderer.getVVector(wipeoutData);

      expect(vVector.x).toBe(0);
      expect(vVector.y).toBe(1);
      expect(vVector.z).toBe(0);
    });
  });

  describe('getSize', () => {
    it('should return correct size', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Size = { X: 10, Y: 20, Z: 0 };

      const size = WipeoutEntityThreejsRenderer.getSize(wipeoutData);

      expect(size.x).toBe(10);
      expect(size.y).toBe(20);
      expect(size.z).toBe(0);
    });
  });

  describe('getClipType', () => {
    it('should return correct clip type', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.ClipType = ClipType.Polygonal;

      const clipType = WipeoutEntityThreejsRenderer.getClipType(wipeoutData);

      expect(clipType).toBe(ClipType.Polygonal);
    });
  });

  describe('getBoundaryPoints', () => {
    it('should return correct boundary points', () => {
      const wipeoutData = createBasicWipeoutData();

      const boundaryPoints = WipeoutEntityThreejsRenderer.getBoundaryPoints(wipeoutData);

      expect(boundaryPoints.length).toBe(4);
      expect(boundaryPoints[0].x).toBe(0);
      expect(boundaryPoints[0].y).toBe(0);
      expect(boundaryPoints[0].z).toBe(0);
    });
  });

  describe('getArea', () => {
    it('should return correct area', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Area = 100;

      const area = WipeoutEntityThreejsRenderer.getArea(wipeoutData);

      expect(area).toBe(100);
    });
  });

  describe('getBounds', () => {
    it('should return correct bounds', () => {
      const wipeoutData = createBasicWipeoutData();

      const bounds = WipeoutEntityThreejsRenderer.getBounds(wipeoutData);

      expect(bounds).toBeInstanceOf(THREE.Box3);
      expect(bounds).not.toBeNull();
      expect(bounds.min.x).toBe(0);
      expect(bounds.min.y).toBe(0);
      expect(bounds.min.z).toBe(0);
      expect(bounds.max.x).toBe(10);
      expect(bounds.max.y).toBe(10);
      expect(bounds.max.z).toBe(0);
    });

    it('should return null when bounds not defined', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Bounds = undefined;

      const bounds = WipeoutEntityThreejsRenderer.getBounds(wipeoutData);

      expect(bounds).toBeNull();
    });
  });

  describe('getCentroid', () => {
    it('should return correct centroid', () => {
      const wipeoutData = createBasicWipeoutData();

      const centroid = WipeoutEntityThreejsRenderer.getCentroid(wipeoutData);

      expect(centroid).not.toBeNull();
      expect(centroid.x).toBe(5);
      expect(centroid.y).toBe(5);
      expect(centroid.z).toBe(0);
    });

    it('should return null when centroid not defined', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Centroid = undefined;

      const centroid = WipeoutEntityThreejsRenderer.getCentroid(wipeoutData);

      expect(centroid).toBeNull();
    });
  });

  describe('getNormal', () => {
    it('should return correct normal', () => {
      const wipeoutData = createBasicWipeoutData();

      const normal = WipeoutEntityThreejsRenderer.getNormal(wipeoutData);

      expect(normal.x).toBe(0);
      expect(normal.y).toBe(0);
      expect(normal.z).toBe(1);
    });
  });

  describe('getColor', () => {
    it('should return correct color', () => {
      const wipeoutData = createBasicWipeoutData();

      const color = WipeoutEntityThreejsRenderer.getColor(wipeoutData);

      expect(color.getHexString()).toBe('ff0000');
    });
  });

  describe('clearCache', () => {
    it('should clear all cached Wipeout meshes', () => {
      const wipeoutData1 = createBasicWipeoutData();
      wipeoutData1.Handle = 'wipeout-1';
      const wipeoutData2 = createBasicWipeoutData();
      wipeoutData2.Handle = 'wipeout-2';

      WipeoutEntityThreejsRenderer.render(wipeoutData1, scene);
      WipeoutEntityThreejsRenderer.render(wipeoutData2, scene);

      WipeoutEntityThreejsRenderer.clearCache();

      const cachedMesh1 = (WipeoutEntityThreejsRenderer as any).wipeoutCache.get('wipeout-1');
      const cachedMesh2 = (WipeoutEntityThreejsRenderer as any).wipeoutCache.get('wipeout-2');
      expect(cachedMesh1).toBeUndefined();
      expect(cachedMesh2).toBeUndefined();
    });
  });

  describe('disposeMultiple', () => {
    it('should dispose multiple Wipeouts', () => {
      const wipeoutDataArray = [
        createBasicWipeoutData(),
        createBasicWipeoutData()
      ];
      wipeoutDataArray[0].Handle = 'wipeout-1';
      wipeoutDataArray[1].Handle = 'wipeout-2';

      const group = WipeoutEntityThreejsRenderer.renderMultiple(wipeoutDataArray, scene);

      WipeoutEntityThreejsRenderer.disposeMultiple(group, scene);

      expect(scene.children.length).toBe(0);
    });

    it('should handle null group', () => {
      WipeoutEntityThreejsRenderer.disposeMultiple(null, scene);

      expect(scene.children.length).toBe(0);
    });
  });

  describe('getTransform', () => {
    it('should return correct transform', () => {
      const wipeoutData = createBasicWipeoutData();

      const transform = WipeoutEntityThreejsRenderer.getTransform(wipeoutData);

      expect(transform.ScaleX).toBe(1);
      expect(transform.ScaleY).toBe(1);
      expect(transform.ScaleZ).toBe(1);
      expect(transform.RotationX).toBe(0);
      expect(transform.RotationY).toBe(0);
      expect(transform.RotationZ).toBe(0);
      expect(transform.TranslateX).toBe(0);
      expect(transform.TranslateY).toBe(0);
      expect(transform.TranslateZ).toBe(0);
    });

    it('should return default transform when transform not defined', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Transform = undefined;

      const transform = WipeoutEntityThreejsRenderer.getTransform(wipeoutData);

      expect(transform.ScaleX).toBe(1);
      expect(transform.ScaleY).toBe(1);
      expect(transform.ScaleZ).toBe(1);
    });
  });

  describe('getGeometry', () => {
    it('should return correct geometry data', () => {
      const wipeoutData = createBasicWipeoutData();

      const geometry = WipeoutEntityThreejsRenderer.getGeometry(wipeoutData);

      expect(geometry.Type).toBe('PolygonGeometry');
      expect(geometry.VertexCount).toBe(4);
      expect(geometry.HasNormals).toBe(true);
      expect(geometry.HasColors).toBe(true);
      expect(geometry.HasUVs).toBe(true);
      expect(geometry.HasIndices).toBe(true);
    });
  });

  describe('getMaterial', () => {
    it('should return correct material data', () => {
      const wipeoutData = createBasicWipeoutData();

      const material = WipeoutEntityThreejsRenderer.getMaterial(wipeoutData);

      expect(material.Type).toBe('MeshBasicMaterial');
      expect(material.Color).toBe(16711680);
      expect(material.Opacity).toBe(1.0);
      expect(material.Transparent).toBe(false);
      expect(material.Wireframe).toBe(false);
      expect(material.VertexColors).toBe(true);
      expect(material.Side).toBe(true);
    });
  });

  describe('getIsMask', () => {
    it('should return correct is mask value', () => {
      const wipeoutData = createBasicWipeoutData();

      const isMask = WipeoutEntityThreejsRenderer.getIsMask(wipeoutData);

      expect(isMask).toBe(true);
    });
  });

  describe('getHandle', () => {
    it('should return correct handle', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Handle = 'test-handle';

      const handle = WipeoutEntityThreejsRenderer.getHandle(wipeoutData);

      expect(handle).toBe('test-handle');
    });

    it('should return empty string when handle not defined', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Handle = undefined;

      const handle = WipeoutEntityThreejsRenderer.getHandle(wipeoutData);

      expect(handle).toBe('');
    });
  });

  describe('getVisible', () => {
    it('should return true when visible', () => {
      const wipeoutData = createBasicWipeoutData();

      const visible = WipeoutEntityThreejsRenderer.getVisible(wipeoutData);

      expect(visible).toBe(true);
    });

    it('should return false when not visible', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Visible = false;

      const visible = WipeoutEntityThreejsRenderer.getVisible(wipeoutData);

      expect(visible).toBe(false);
    });
  });

  describe('getOpacity', () => {
    it('should return correct opacity', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Opacity = 0.5;

      const opacity = WipeoutEntityThreejsRenderer.getOpacity(wipeoutData);

      expect(opacity).toBe(0.5);
    });

    it('should return default opacity when not defined', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Opacity = undefined;

      const opacity = WipeoutEntityThreejsRenderer.getOpacity(wipeoutData);

      expect(opacity).toBe(1.0);
    });
  });

  describe('getTransparent', () => {
    it('should return correct transparent value', () => {
      const wipeoutData = createBasicWipeoutData();

      const transparent = WipeoutEntityThreejsRenderer.getTransparent(wipeoutData);

      expect(transparent).toBe(true);
    });

    it('should return false when transparent is false', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Transparent = false;

      const transparent = WipeoutEntityThreejsRenderer.getTransparent(wipeoutData);

      expect(transparent).toBe(false);
    });
  });

  describe('getClippingState', () => {
    it('should return correct clipping state', () => {
      const wipeoutData = createBasicWipeoutData();

      const clippingState = WipeoutEntityThreejsRenderer.getClippingState(wipeoutData);

      expect(clippingState).toBe(true);
    });
  });

  describe('getBrightness', () => {
    it('should return correct brightness', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Brightness = 75;

      const brightness = WipeoutEntityThreejsRenderer.getBrightness(wipeoutData);

      expect(brightness).toBe(75);
    });
  });

  describe('getContrast', () => {
    it('should return correct contrast', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Contrast = 80;

      const contrast = WipeoutEntityThreejsRenderer.getContrast(wipeoutData);

      expect(contrast).toBe(80);
    });
  });

  describe('getFade', () => {
    it('should return correct fade', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Fade = 30;

      const fade = WipeoutEntityThreejsRenderer.getFade(wipeoutData);

      expect(fade).toBe(30);
    });
  });

  describe('getFlags', () => {
    it('should return correct flags', () => {
      const wipeoutData = createBasicWipeoutData();
      wipeoutData.Flags = ImageDisplayFlags.ShowImage | ImageDisplayFlags.UseClipping;

      const flags = WipeoutEntityThreejsRenderer.getFlags(wipeoutData);

      expect(flags).toBe(ImageDisplayFlags.ShowImage | ImageDisplayFlags.UseClipping);
    });
  });
});
