import * as THREE from 'three';
import { Face3DEntityThreejsRenderer, Face3DData } from '../Face3DEntityThreejsRenderer';

describe('Face3DEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
  });

  describe('render', () => {
    it('should render a basic triangle face', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-1',
        EntityType: 'Face3D',
        Handle: 'handle-1',
        LayerName: 'TEST_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 5,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
      expect(mesh.visible).toBe(true);
      expect(mesh.castShadow).toBe(true);
      expect(mesh.receiveShadow).toBe(true);
      expect(mesh.userData.type).toBe('Face3D');
      expect(mesh.userData.uuid).toBe('test-uuid-1');
      expect(mesh.userData.entityType).toBe('Face3D');
      expect(mesh.userData.handle).toBe('handle-1');
      expect(mesh.userData.layerName).toBe('TEST_LAYER');
    });

    it('should render a quad face', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-2',
        EntityType: 'Face3D',
        Handle: 'handle-2',
        LayerName: 'QUAD_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 10,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 5, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
      expect(mesh.visible).toBe(true);
    });

    it('should return null for invisible face', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-3',
        EntityType: 'Face3D',
        Handle: 'handle-3',
        LayerName: 'INVISIBLE_LAYER',
        Visible: true,
        IsInvisible: true,
        ColorIndex: 1,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeNull();
    });

    it('should return null for non-visible face', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-4',
        EntityType: 'Face3D',
        Handle: 'handle-4',
        LayerName: 'HIDDEN_LAYER',
        Visible: false,
        IsInvisible: false,
        ColorIndex: 1,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeNull();
    });

    it('should render 3D oriented face', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-5',
        EntityType: 'Face3D',
        Handle: 'handle-5',
        LayerName: '3D_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 15,
        Vertices: [0, 0, 5, 10, 0, 5, 10, 10, 10, 0, 10, 10],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 5, Z: 7.5 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 5 },
          Max: { X: 10, Y: 10, Z: 10 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
      expect(mesh.visible).toBe(true);
    });

    it('should apply transform matrix when provided', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-6',
        EntityType: 'Face3D',
        Handle: 'handle-6',
        LayerName: 'TRANSFORM_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 20,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0,
        Transform: {
          Matrix: [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            10, 20, 30, 1
          ]
        }
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
      expect(mesh.position.x).toBe(10);
      expect(mesh.position.y).toBe(20);
      expect(mesh.position.z).toBe(30);
    });

    it('should use default color when color index is invalid', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-7',
        EntityType: 'Face3D',
        Handle: 'handle-7',
        LayerName: 'DEFAULT_COLOR_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 300,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
    });

    it('should handle face with large coordinates', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-8',
        EntityType: 'Face3D',
        Handle: 'handle-8',
        LayerName: 'LARGE_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 25,
        Vertices: [1000, 1000, 1000, 2000, 1000, 1000, 1500, 2000, 1000],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 1500, Y: 1500, Z: 1000 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 500000,
        Bounds: {
          Min: { X: 1000, Y: 1000, Z: 1000 },
          Max: { X: 2000, Y: 2000, Z: 1000 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
    });

    it('should handle face with negative coordinates', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-9',
        EntityType: 'Face3D',
        Handle: 'handle-9',
        LayerName: 'NEGATIVE_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 30,
        Vertices: [-10, -10, -10, 0, -10, -10, -5, 0, -10],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: -5, Y: -6.67, Z: -10 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: -10, Y: -10, Z: -10 },
          Max: { X: 0, Y: 0, Z: -10 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
    });

    it('should set render order correctly', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-10',
        EntityType: 'Face3D',
        Handle: 'handle-10',
        LayerName: 'RENDER_ORDER_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 35,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 100
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
      expect(mesh.renderOrder).toBe(100);
    });

    it('should handle face without indices', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-11',
        EntityType: 'Face3D',
        Handle: 'handle-11',
        LayerName: 'NO_INDICES_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 40,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
    });

    it('should handle face without normals', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-12',
        EntityType: 'Face3D',
        Handle: 'handle-12',
        LayerName: 'NO_NORMALS_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 45,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Indices: [0, 1, 2],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
      const geometry = mesh.geometry;
      expect(geometry.attributes.normal).toBeDefined();
    });

    it('should handle face without vertices', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-13',
        EntityType: 'Face3D',
        Handle: 'handle-13',
        LayerName: 'NO_VERTICES_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 50,
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeNull();
    });
  });

  describe('createMaterial', () => {
    it('should create material with correct color index', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-14',
        EntityType: 'Face3D',
        Handle: 'handle-14',
        LayerName: 'MATERIAL_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 5,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.material).toBeInstanceOf(THREE.MeshPhongMaterial);
    });

    it('should create material with default color when color index is 0', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-15',
        EntityType: 'Face3D',
        Handle: 'handle-15',
        LayerName: 'DEFAULT_COLOR_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 0,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.material).toBeInstanceOf(THREE.MeshPhongMaterial);
    });

    it('should create material with default color when color index is 256', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-16',
        EntityType: 'Face3D',
        Handle: 'handle-16',
        LayerName: 'LAYER_COLOR_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 256,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        EdgeVisibility: [true, true, true],
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.material).toBeInstanceOf(THREE.MeshPhongMaterial);
    });

    it('should create material with default color when color index is 257', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-17',
        EntityType: 'Face3D',
        Handle: 'handle-17',
        LayerName: 'BLOCK_COLOR_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 257,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        EdgeVisibility: [true, true, true],
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.material).toBeInstanceOf(THREE.MeshPhongMaterial);
    });
  });

  describe('createGeometry', () => {
    it('should create geometry with correct vertex positions', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-18',
        EntityType: 'Face3D',
        Handle: 'handle-18',
        LayerName: 'GEOMETRY_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 55,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        EdgeVisibility: [true, true, true],
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      const geometry = mesh.geometry;
      expect(geometry).toBeInstanceOf(THREE.BufferGeometry);
      expect(geometry.attributes.position).toBeDefined();
      expect(geometry.attributes.position.count).toBe(3);
    });

    it('should create geometry with correct indices', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-19',
        EntityType: 'Face3D',
        Handle: 'handle-19',
        LayerName: 'INDICES_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 60,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 5, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        EdgeVisibility: [true, true, true, true],
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      const geometry = mesh.geometry;
      expect(geometry).toBeInstanceOf(THREE.BufferGeometry);
      expect(geometry.index).toBeDefined();
      expect(geometry.index.count).toBe(6);
    });

    it('should create geometry with correct normals', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-20',
        EntityType: 'Face3D',
        Handle: 'handle-20',
        LayerName: 'NORMALS_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 65,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        EdgeVisibility: [true, true, true],
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      const geometry = mesh.geometry;
      expect(geometry).toBeInstanceOf(THREE.BufferGeometry);
      expect(geometry.attributes.normal).toBeDefined();
      expect(geometry.attributes.normal.count).toBe(3);
    });

    it('should compute bounding sphere and bounding box', () => {
      const faceData: Face3DData = {
        Type: 'Face3D',
        Uuid: 'test-uuid-21',
        EntityType: 'Face3D',
        Handle: 'handle-21',
        LayerName: 'BOUNDS_LAYER',
        Visible: true,
        IsInvisible: false,
        ColorIndex: 70,
        Vertices: [0, 0, 0, 10, 0, 0, 5, 10, 0],
        Indices: [0, 1, 2],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
        Center: { X: 5, Y: 3.33, Z: 0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        Area: 50,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        EdgeVisibility: [true, true, true],
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = Face3DEntityThreejsRenderer.render(faceData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      const geometry = mesh.geometry;
      expect(geometry.boundingSphere).toBeDefined();
      expect(geometry.boundingBox).toBeDefined();
    });
  });
});
