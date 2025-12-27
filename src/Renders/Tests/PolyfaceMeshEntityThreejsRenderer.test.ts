import * as THREE from 'three';
import { PolyfaceMeshEntityThreejsRenderer, PolyfaceMeshData, Point3DData, NormalData, ColorData, TransformData, BoundsData3D, VertexData, FaceData } from '../PolyfaceMeshEntityThreejsRenderer';

describe('PolyfaceMeshEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    PolyfaceMeshEntityThreejsRenderer.clearCache();
  });

  const createBasicPolyfaceMeshData = (): PolyfaceMeshData => ({
    Vertices: [
      { Location: { X: 0, Y: 0, Z: 0 }, Normal: { X: 0, Y: 0, Z: 1 }, Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 } },
      { Location: { X: 10, Y: 0, Z: 0 }, Normal: { X: 0, Y: 0, Z: 1 }, Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 } },
      { Location: { X: 5, Y: 10, Z: 0 }, Normal: { X: 0, Y: 0, Z: 1 }, Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 } }
    ],
    Faces: [
      { VertexIndices: [0, 1, 2], Normal: { X: 0, Y: 0, Z: 1 } }
    ],
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.03,
    VertexCount: 3,
    FaceCount: 1,
    Vertices3D: [
      { X: 0, Y: 0, Z: 0 },
      { X: 10, Y: 0, Z: 0 },
      { X: 5, Y: 10, Z: 0 }
    ],
    Indices: [0, 1, 2],
    NormalsArray: [0, 0, 1, 0, 0, 1, 0, 0, 1],
    ColorsArray: [1, 0, 0, 1, 0, 0, 1, 0, 0],
    UVsArray: [],
    Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 },
    Bounds3D: {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 10, Y: 10, Z: 0 }
    },
    Centroid: { X: 5, Y: 3.33, Z: 0 },
    Normals: [
      { X: 0, Y: 0, Z: 1 },
      { X: 0, Y: 0, Z: 1 },
      { X: 0, Y: 0, Z: 1 }
    ],
    Transform: {
      Position: { X: 0, Y: 0, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
    },
    EntityType: 'PolyfaceMesh',
    Visible: true,
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Handle: 'polyfacemesh-handle-1',
    Transparency: 0,
    MaterialName: 'Standard',
    CastShadows: true,
    ReceiveShadows: true,
    GeometryType: 'BufferGeometry',
    DoubleSided: false,
    FlatShading: false
  });

  describe('render', () => {
    it('should render a basic PolyfaceMesh', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.name).toBe('PolyfaceMesh_polyfacemesh-handle-1');
    });

    it('should not add invisible PolyfaceMesh to scene', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Visible = false;

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.visible).toBe(false);
      expect(scene.children.length).toBe(0);
    });

    it('should render PolyfaceMesh with correct Mesh object', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      expect(group.children.length).toBe(1);
      const mesh = group.children[0] as THREE.Mesh;
      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.name).toBe('PolyfaceMesh');
    });

    it('should render PolyfaceMesh with correct material', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.material).toBeDefined();
      expect(mesh.material).toBeInstanceOf(THREE.MeshStandardMaterial);
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.color.getHexString()).toBe('ff0000');
      expect(material.vertexColors).toBe(true);
      expect(material.side).toBe(THREE.FrontSide);
      expect(material.flatShading).toBe(false);
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(true);
    });

    it('should render PolyfaceMesh with correct geometry', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.geometry).toBeDefined();
      expect(mesh.geometry).toBeInstanceOf(THREE.BufferGeometry);
      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position).toBeDefined();
      expect(geometry.attributes.position.count).toBe(3);
      expect(geometry.attributes.normal).toBeDefined();
      expect(geometry.attributes.color).toBeDefined();
    });

    it('should render PolyfaceMesh with correct vertices', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(0);
      expect(positions[1]).toBe(0);
      expect(positions[2]).toBe(0);
      expect(positions[3]).toBe(10);
      expect(positions[4]).toBe(0);
      expect(positions[5]).toBe(0);
      expect(positions[6]).toBe(5);
      expect(positions[7]).toBe(10);
      expect(positions[8]).toBe(0);
    });

    it('should render PolyfaceMesh with correct normals', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const normals = geometry.attributes.normal.array as Float32Array;
      expect(normals[0]).toBe(0);
      expect(normals[1]).toBe(0);
      expect(normals[2]).toBe(1);
    });

    it('should render PolyfaceMesh with correct colors', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const colors = geometry.attributes.color.array as Float32Array;
      expect(colors[0]).toBe(1);
      expect(colors[1]).toBe(0);
      expect(colors[2]).toBe(0);
    });

    it('should render PolyfaceMesh with correct indices', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const indices = geometry.index?.array as Uint16Array;
      expect(indices).toBeDefined();
      expect(indices[0]).toBe(0);
      expect(indices[1]).toBe(1);
      expect(indices[2]).toBe(2);
    });

    it('should render PolyfaceMesh with correct color', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should render PolyfaceMesh with correct transparency', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Transparency = 0.5;

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should render PolyfaceMesh with double-sided material', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.DoubleSided = true;

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.side).toBe(THREE.DoubleSide);
    });

    it('should render PolyfaceMesh with flat shading', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.FlatShading = true;

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.flatShading).toBe(true);
    });

    it('should render PolyfaceMesh with correct shadow settings', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.castShadow).toBe(true);
      expect(mesh.receiveShadow).toBe(true);
    });

    it('should cache PolyfaceMesh by handle', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const cachedMesh = PolyfaceMeshEntityThreejsRenderer.getMeshGroup(polyfaceMeshData);
      expect(cachedMesh).toBeDefined();
      expect(cachedMesh).toBeInstanceOf(THREE.Mesh);
    });

    it('should render PolyfaceMesh with correct handle in userData', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.userData.handle).toBe('polyfacemesh-handle-1');
    });

    it('should render PolyfaceMesh with UV coordinates', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.UVsArray = [0, 0, 1, 0, 0.5, 1];

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.uv).toBeDefined();
      const uvs = geometry.attributes.uv.array as Float32Array;
      expect(uvs[0]).toBe(0);
      expect(uvs[1]).toBe(0);
      expect(uvs[2]).toBe(1);
      expect(uvs[3]).toBe(0);
      expect(uvs[4]).toBe(0.5);
      expect(uvs[5]).toBe(1);
    });

    it('should render PolyfaceMesh with multiple faces', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Vertices = [
        { Location: { X: 0, Y: 0, Z: 0 }, Normal: { X: 0, Y: 0, Z: 1 }, Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 } },
        { Location: { X: 10, Y: 0, Z: 0 }, Normal: { X: 0, Y: 0, Z: 1 }, Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 } },
        { Location: { X: 5, Y: 10, Z: 0 }, Normal: { X: 0, Y: 0, Z: 1 }, Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 } },
        { Location: { X: 10, Y: 10, Z: 0 }, Normal: { X: 0, Y: 0, Z: 1 }, Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 } }
      ];
      polyfaceMeshData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 0, Z: 0 },
        { X: 5, Y: 10, Z: 0 },
        { X: 10, Y: 10, Z: 0 }
      ];
      polyfaceMeshData.Faces = [
        { VertexIndices: [0, 1, 2], Normal: { X: 0, Y: 0, Z: 1 } },
        { VertexIndices: [1, 3, 2], Normal: { X: 0, Y: 0, Z: 1 } }
      ];
      polyfaceMeshData.Indices = [0, 1, 2, 1, 3, 2];
      polyfaceMeshData.NormalsArray = [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1];
      polyfaceMeshData.ColorsArray = [1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0];
      polyfaceMeshData.FaceCount = 2;
      polyfaceMeshData.VertexCount = 4;

      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position.count).toBe(4);
      const indices = geometry.index?.array as Uint16Array;
      expect(indices.length).toBe(6);
    });
  });

  describe('update', () => {
    it('should update existing PolyfaceMesh', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const updatedPolyfaceMeshData = { ...polyfaceMeshData };
      updatedPolyfaceMeshData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const result = PolyfaceMeshEntityThreejsRenderer.update(updatedPolyfaceMeshData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent PolyfaceMesh', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const result = PolyfaceMeshEntityThreejsRenderer.update(polyfaceMeshData, scene);

      expect(result).toBe(false);
    });

    it('should update PolyfaceMesh color', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const updatedPolyfaceMeshData = { ...polyfaceMeshData };
      updatedPolyfaceMeshData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const result = PolyfaceMeshEntityThreejsRenderer.update(updatedPolyfaceMeshData, scene);
      const mesh = PolyfaceMeshEntityThreejsRenderer.getMeshGroup(polyfaceMeshData) as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;

      expect(result).toBe(true);
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should update PolyfaceMesh transparency', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const updatedPolyfaceMeshData = { ...polyfaceMeshData };
      updatedPolyfaceMeshData.Transparency = 0.7;

      const result = PolyfaceMeshEntityThreejsRenderer.update(updatedPolyfaceMeshData, scene);
      const mesh = PolyfaceMeshEntityThreejsRenderer.getMeshGroup(polyfaceMeshData) as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;

      expect(result).toBe(true);
      expect(material.opacity).toBeCloseTo(0.3);
      expect(material.transparent).toBe(true);
    });

    it('should update PolyfaceMesh vertices', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const updatedPolyfaceMeshData = { ...polyfaceMeshData };
      updatedPolyfaceMeshData.Vertices3D = [
        { X: 10, Y: 10, Z: 10 },
        { X: 20, Y: 10, Z: 10 },
        { X: 15, Y: 20, Z: 10 }
      ];

      const result = PolyfaceMeshEntityThreejsRenderer.update(updatedPolyfaceMeshData, scene);

      expect(result).toBe(true);
    });

    it('should update PolyfaceMesh normals', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const updatedPolyfaceMeshData = { ...polyfaceMeshData };
      updatedPolyfaceMeshData.NormalsArray = [0, 1, 0, 0, 1, 0, 0, 1, 0];

      const result = PolyfaceMeshEntityThreejsRenderer.update(updatedPolyfaceMeshData, scene);

      expect(result).toBe(true);
    });

    it('should update PolyfaceMesh colors', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const updatedPolyfaceMeshData = { ...polyfaceMeshData };
      updatedPolyfaceMeshData.ColorsArray = [0, 1, 0, 0, 1, 0, 0, 1, 0];

      const result = PolyfaceMeshEntityThreejsRenderer.update(updatedPolyfaceMeshData, scene);

      expect(result).toBe(true);
    });

    it('should update PolyfaceMesh material properties', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const updatedPolyfaceMeshData = { ...polyfaceMeshData };
      updatedPolyfaceMeshData.DoubleSided = true;
      updatedPolyfaceMeshData.FlatShading = true;

      const result = PolyfaceMeshEntityThreejsRenderer.update(updatedPolyfaceMeshData, scene);
      const mesh = PolyfaceMeshEntityThreejsRenderer.getMeshGroup(polyfaceMeshData) as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;

      expect(result).toBe(true);
      expect(material.side).toBe(THREE.DoubleSide);
      expect(material.flatShading).toBe(true);
    });
  });

  describe('dispose', () => {
    it('should dispose PolyfaceMesh resources', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const result = PolyfaceMeshEntityThreejsRenderer.dispose(polyfaceMeshData, scene);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(0);
    });

    it('should return false for non-existent PolyfaceMesh', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const result = PolyfaceMeshEntityThreejsRenderer.dispose(polyfaceMeshData, scene);

      expect(result).toBe(false);
    });

    it('should remove PolyfaceMesh from cache', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      PolyfaceMeshEntityThreejsRenderer.dispose(polyfaceMeshData, scene);

      const cachedMesh = PolyfaceMeshEntityThreejsRenderer.getMeshGroup(polyfaceMeshData);
      expect(cachedMesh).toBeNull();
    });

    it('should dispose PolyfaceMesh geometry', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = PolyfaceMeshEntityThreejsRenderer.getMeshGroup(polyfaceMeshData) as THREE.Mesh;

      const geometry = mesh.geometry;
      const originalDispose = geometry.dispose.bind(geometry);
      let disposeCalled = false;
      geometry.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      PolyfaceMeshEntityThreejsRenderer.dispose(polyfaceMeshData, scene);

      expect(disposeCalled).toBe(true);
    });

    it('should dispose PolyfaceMesh material', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);
      const mesh = PolyfaceMeshEntityThreejsRenderer.getMeshGroup(polyfaceMeshData) as THREE.Mesh;

      const material = mesh.material;
      const originalDispose = material.dispose.bind(material);
      let disposeCalled = false;
      material.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      PolyfaceMeshEntityThreejsRenderer.dispose(polyfaceMeshData, scene);

      expect(disposeCalled).toBe(true);
    });
  });

  describe('setVisibility', () => {
    it('should set PolyfaceMesh visibility to true', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const result = PolyfaceMeshEntityThreejsRenderer.setVisibility(polyfaceMeshData, scene, true);

      expect(result).toBe(true);
      expect(group.visible).toBe(true);
    });

    it('should set PolyfaceMesh visibility to false', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const result = PolyfaceMeshEntityThreejsRenderer.setVisibility(polyfaceMeshData, scene, false);

      expect(result).toBe(true);
      expect(group.visible).toBe(false);
    });

    it('should return false for non-existent PolyfaceMesh', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const result = PolyfaceMeshEntityThreejsRenderer.setVisibility(polyfaceMeshData, scene, true);

      expect(result).toBe(false);
    });

    it('should add PolyfaceMesh to scene when setting visibility to true', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Visible = false;
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      PolyfaceMeshEntityThreejsRenderer.setVisibility(polyfaceMeshData, scene, true);

      expect(scene.children.length).toBe(1);
    });

    it('should remove PolyfaceMesh from scene when setting visibility to false', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      PolyfaceMeshEntityThreejsRenderer.setVisibility(polyfaceMeshData, scene, false);

      expect(scene.children.length).toBe(0);
    });
  });

  describe('setOpacity', () => {
    it('should set PolyfaceMesh opacity', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const result = PolyfaceMeshEntityThreejsRenderer.setOpacity(polyfaceMeshData, 0.5);

      expect(result).toBe(true);
      const mesh = PolyfaceMeshEntityThreejsRenderer.getMeshGroup(polyfaceMeshData) as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should return false for non-existent PolyfaceMesh', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const result = PolyfaceMeshEntityThreejsRenderer.setOpacity(polyfaceMeshData, 0.5);

      expect(result).toBe(false);
    });

    it('should set opacity to 1.0 and transparent to false', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const result = PolyfaceMeshEntityThreejsRenderer.setOpacity(polyfaceMeshData, 1.0);

      expect(result).toBe(true);
      const mesh = PolyfaceMeshEntityThreejsRenderer.getMeshGroup(polyfaceMeshData) as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
    });
  });

  describe('setColor', () => {
    it('should set PolyfaceMesh color', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const group = PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const color = new THREE.Color('#00FF00');
      const result = PolyfaceMeshEntityThreejsRenderer.setColor(polyfaceMeshData, color);

      expect(result).toBe(true);
      const mesh = PolyfaceMeshEntityThreejsRenderer.getMeshGroup(polyfaceMeshData) as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should return false for non-existent PolyfaceMesh', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      const color = new THREE.Color('#00FF00');

      const result = PolyfaceMeshEntityThreejsRenderer.setColor(polyfaceMeshData, color);

      expect(result).toBe(false);
    });
  });

  describe('getMeshGroup', () => {
    it('should return cached mesh', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      const mesh = PolyfaceMeshEntityThreejsRenderer.getMeshGroup(polyfaceMeshData);

      expect(mesh).toBeDefined();
      expect(mesh).toBeInstanceOf(THREE.Mesh);
    });

    it('should return null for non-existent mesh', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const mesh = PolyfaceMeshEntityThreejsRenderer.getMeshGroup(polyfaceMeshData);

      expect(mesh).toBeNull();
    });
  });

  describe('getVertices', () => {
    it('should return vertices as THREE.Vector3 array', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const vertices = PolyfaceMeshEntityThreejsRenderer.getVertices(polyfaceMeshData);

      expect(vertices).toHaveLength(3);
      expect(vertices[0]).toBeInstanceOf(THREE.Vector3);
      expect(vertices[0].x).toBe(0);
      expect(vertices[0].y).toBe(0);
      expect(vertices[0].z).toBe(0);
      expect(vertices[1].x).toBe(10);
      expect(vertices[1].y).toBe(0);
      expect(vertices[1].z).toBe(0);
      expect(vertices[2].x).toBe(5);
      expect(vertices[2].y).toBe(10);
      expect(vertices[2].z).toBe(0);
    });

    it('should return empty array for empty vertices', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Vertices3D = [];

      const vertices = PolyfaceMeshEntityThreejsRenderer.getVertices(polyfaceMeshData);

      expect(vertices).toHaveLength(0);
    });
  });

  describe('getIndices', () => {
    it('should return indices', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const indices = PolyfaceMeshEntityThreejsRenderer.getIndices(polyfaceMeshData);

      expect(indices).toEqual([0, 1, 2]);
    });

    it('should return empty array for no indices', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Indices = [];

      const indices = PolyfaceMeshEntityThreejsRenderer.getIndices(polyfaceMeshData);

      expect(indices).toEqual([]);
    });
  });

  describe('getNormals', () => {
    it('should return normals as THREE.Vector3 array', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const normals = PolyfaceMeshEntityThreejsRenderer.getNormals(polyfaceMeshData);

      expect(normals).toHaveLength(3);
      expect(normals[0]).toBeInstanceOf(THREE.Vector3);
      expect(normals[0].x).toBe(0);
      expect(normals[0].y).toBe(0);
      expect(normals[0].z).toBe(1);
    });

    it('should return empty array for empty normals', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Normals = [];

      const normals = PolyfaceMeshEntityThreejsRenderer.getNormals(polyfaceMeshData);

      expect(normals).toHaveLength(0);
    });
  });

  describe('getFaces', () => {
    it('should return faces', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const faces = PolyfaceMeshEntityThreejsRenderer.getFaces(polyfaceMeshData);

      expect(faces).toHaveLength(1);
      expect(faces[0].VertexIndices).toEqual([0, 1, 2]);
      expect(faces[0].Normal).toEqual({ X: 0, Y: 0, Z: 1 });
    });

    it('should return empty array for no faces', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Faces = [];

      const faces = PolyfaceMeshEntityThreejsRenderer.getFaces(polyfaceMeshData);

      expect(faces).toEqual([]);
    });
  });

  describe('getColor', () => {
    it('should return color as THREE.Color', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const color = PolyfaceMeshEntityThreejsRenderer.getColor(polyfaceMeshData);

      expect(color).toBeInstanceOf(THREE.Color);
      expect(color.getHexString()).toBe('ff0000');
    });
  });

  describe('getBounds', () => {
    it('should return bounds as THREE.Box3', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const bounds = PolyfaceMeshEntityThreejsRenderer.getBounds(polyfaceMeshData);

      expect(bounds).toBeInstanceOf(THREE.Box3);
      expect(bounds.min.x).toBe(0);
      expect(bounds.min.y).toBe(0);
      expect(bounds.min.z).toBe(0);
      expect(bounds.max.x).toBe(10);
      expect(bounds.max.y).toBe(10);
      expect(bounds.max.z).toBe(0);
    });
  });

  describe('getCentroid', () => {
    it('should return centroid as THREE.Vector3', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const centroid = PolyfaceMeshEntityThreejsRenderer.getCentroid(polyfaceMeshData);

      expect(centroid).toBeInstanceOf(THREE.Vector3);
      expect(centroid.x).toBe(5);
      expect(centroid.y).toBeCloseTo(3.33, 2);
      expect(centroid.z).toBe(0);
    });
  });

  describe('getVertexCount', () => {
    it('should return vertex count', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const count = PolyfaceMeshEntityThreejsRenderer.getVertexCount(polyfaceMeshData);

      expect(count).toBe(3);
    });

    it('should return 0 for no vertices', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.VertexCount = 0;

      const count = PolyfaceMeshEntityThreejsRenderer.getVertexCount(polyfaceMeshData);

      expect(count).toBe(0);
    });
  });

  describe('getFaceCount', () => {
    it('should return face count', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const count = PolyfaceMeshEntityThreejsRenderer.getFaceCount(polyfaceMeshData);

      expect(count).toBe(1);
    });

    it('should return 0 for no faces', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.FaceCount = 0;

      const count = PolyfaceMeshEntityThreejsRenderer.getFaceCount(polyfaceMeshData);

      expect(count).toBe(0);
    });
  });

  describe('clearCache', () => {
    it('should clear mesh cache', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      PolyfaceMeshEntityThreejsRenderer.render(polyfaceMeshData, scene);

      PolyfaceMeshEntityThreejsRenderer.clearCache();

      const mesh = PolyfaceMeshEntityThreejsRenderer.getMeshGroup(polyfaceMeshData);
      expect(mesh).toBeNull();
    });
  });

  describe('getTransform', () => {
    it('should return transform data', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const transform = PolyfaceMeshEntityThreejsRenderer.getTransform(polyfaceMeshData);

      expect(transform.Position).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(transform.Rotation).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(transform.Scale).toEqual({ X: 1, Y: 1, Z: 1 });
      expect(transform.Matrix).toEqual([1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]);
    });

    it('should return default transform when not set', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Transform = undefined as any;

      const transform = PolyfaceMeshEntityThreejsRenderer.getTransform(polyfaceMeshData);

      expect(transform.Position).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(transform.Rotation).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(transform.Scale).toEqual({ X: 1, Y: 1, Z: 1 });
    });
  });

  describe('getBounds3D', () => {
    it('should return bounds data', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const bounds = PolyfaceMeshEntityThreejsRenderer.getBounds3D(polyfaceMeshData);

      expect(bounds.Min).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(bounds.Max).toEqual({ X: 10, Y: 10, Z: 0 });
    });

    it('should return default bounds when not set', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Bounds3D = undefined as any;

      const bounds = PolyfaceMeshEntityThreejsRenderer.getBounds3D(polyfaceMeshData);

      expect(bounds.Min).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(bounds.Max).toEqual({ X: 0, Y: 0, Z: 0 });
    });
  });

  describe('getCentroid3D', () => {
    it('should return centroid data', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const centroid = PolyfaceMeshEntityThreejsRenderer.getCentroid3D(polyfaceMeshData);

      expect(centroid).toEqual({ X: 5, Y: 3.33, Z: 0 });
    });

    it('should return default centroid when not set', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Centroid = undefined as any;

      const centroid = PolyfaceMeshEntityThreejsRenderer.getCentroid3D(polyfaceMeshData);

      expect(centroid).toEqual({ X: 0, Y: 0, Z: 0 });
    });
  });

  describe('getHandle', () => {
    it('should return handle', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const handle = PolyfaceMeshEntityThreejsRenderer.getHandle(polyfaceMeshData);

      expect(handle).toBe('polyfacemesh-handle-1');
    });

    it('should return empty string when handle not set', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Handle = undefined as any;

      const handle = PolyfaceMeshEntityThreejsRenderer.getHandle(polyfaceMeshData);

      expect(handle).toBe('');
    });
  });

  describe('getLayerName', () => {
    it('should return layer name', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const layerName = PolyfaceMeshEntityThreejsRenderer.getLayerName(polyfaceMeshData);

      expect(layerName).toBe('TEST_LAYER');
    });

    it('should return empty string when layer name not set', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.LayerName = undefined as any;

      const layerName = PolyfaceMeshEntityThreejsRenderer.getLayerName(polyfaceMeshData);

      expect(layerName).toBe('');
    });
  });

  describe('getVisible', () => {
    it('should return true when visible', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const visible = PolyfaceMeshEntityThreejsRenderer.getVisible(polyfaceMeshData);

      expect(visible).toBe(true);
    });

    it('should return false when not visible', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Visible = false;

      const visible = PolyfaceMeshEntityThreejsRenderer.getVisible(polyfaceMeshData);

      expect(visible).toBe(false);
    });

    it('should return true when visibility not set', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Visible = undefined as any;

      const visible = PolyfaceMeshEntityThreejsRenderer.getVisible(polyfaceMeshData);

      expect(visible).toBe(true);
    });
  });

  describe('getOpacity', () => {
    it('should return opacity', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Transparency = 0.5;

      const opacity = PolyfaceMeshEntityThreejsRenderer.getOpacity(polyfaceMeshData);

      expect(opacity).toBe(0.5);
    });

    it('should return 1.0 when transparency not set', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Transparency = undefined as any;

      const opacity = PolyfaceMeshEntityThreejsRenderer.getOpacity(polyfaceMeshData);

      expect(opacity).toBe(1.0);
    });
  });

  describe('getTransparent', () => {
    it('should return true when transparent', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.Transparency = 0.5;

      const transparent = PolyfaceMeshEntityThreejsRenderer.getTransparent(polyfaceMeshData);

      expect(transparent).toBe(true);
    });

    it('should return false when not transparent', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const transparent = PolyfaceMeshEntityThreejsRenderer.getTransparent(polyfaceMeshData);

      expect(transparent).toBe(false);
    });
  });

  describe('getCastShadows', () => {
    it('should return true when cast shadows', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const castShadows = PolyfaceMeshEntityThreejsRenderer.getCastShadows(polyfaceMeshData);

      expect(castShadows).toBe(true);
    });

    it('should return false when not cast shadows', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.CastShadows = false;

      const castShadows = PolyfaceMeshEntityThreejsRenderer.getCastShadows(polyfaceMeshData);

      expect(castShadows).toBe(false);
    });

    it('should return true when cast shadows not set', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.CastShadows = undefined as any;

      const castShadows = PolyfaceMeshEntityThreejsRenderer.getCastShadows(polyfaceMeshData);

      expect(castShadows).toBe(true);
    });
  });

  describe('getReceiveShadows', () => {
    it('should return true when receive shadows', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const receiveShadows = PolyfaceMeshEntityThreejsRenderer.getReceiveShadows(polyfaceMeshData);

      expect(receiveShadows).toBe(true);
    });

    it('should return false when not receive shadows', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.ReceiveShadows = false;

      const receiveShadows = PolyfaceMeshEntityThreejsRenderer.getReceiveShadows(polyfaceMeshData);

      expect(receiveShadows).toBe(false);
    });

    it('should return true when receive shadows not set', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.ReceiveShadows = undefined as any;

      const receiveShadows = PolyfaceMeshEntityThreejsRenderer.getReceiveShadows(polyfaceMeshData);

      expect(receiveShadows).toBe(true);
    });
  });

  describe('getDoubleSided', () => {
    it('should return true when double sided', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.DoubleSided = true;

      const doubleSided = PolyfaceMeshEntityThreejsRenderer.getDoubleSided(polyfaceMeshData);

      expect(doubleSided).toBe(true);
    });

    it('should return false when not double sided', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const doubleSided = PolyfaceMeshEntityThreejsRenderer.getDoubleSided(polyfaceMeshData);

      expect(doubleSided).toBe(false);
    });
  });

  describe('getFlatShading', () => {
    it('should return true when flat shading', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.FlatShading = true;

      const flatShading = PolyfaceMeshEntityThreejsRenderer.getFlatShading(polyfaceMeshData);

      expect(flatShading).toBe(true);
    });

    it('should return false when not flat shading', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const flatShading = PolyfaceMeshEntityThreejsRenderer.getFlatShading(polyfaceMeshData);

      expect(flatShading).toBe(false);
    });
  });

  describe('getGeometryType', () => {
    it('should return geometry type', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const geometryType = PolyfaceMeshEntityThreejsRenderer.getGeometryType(polyfaceMeshData);

      expect(geometryType).toBe('BufferGeometry');
    });

    it('should return default geometry type when not set', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.GeometryType = undefined as any;

      const geometryType = PolyfaceMeshEntityThreejsRenderer.getGeometryType(polyfaceMeshData);

      expect(geometryType).toBe('BufferGeometry');
    });
  });

  describe('getEntityType', () => {
    it('should return entity type', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();

      const entityType = PolyfaceMeshEntityThreejsRenderer.getEntityType(polyfaceMeshData);

      expect(entityType).toBe('PolyfaceMesh');
    });

    it('should return default entity type when not set', () => {
      const polyfaceMeshData = createBasicPolyfaceMeshData();
      polyfaceMeshData.EntityType = undefined as any;

      const entityType = PolyfaceMeshEntityThreejsRenderer.getEntityType(polyfaceMeshData);

      expect(entityType).toBe('PolyfaceMesh');
    });
  });
});
