import * as THREE from 'three';
import { PolygonMeshEntityThreejsRenderer, PolygonMeshData, Point3DData, NormalData, ColorData, TransformData, BoundsData3D, VertexData, FaceData, EdgeData } from '../PolygonMeshEntityThreejsRenderer';

describe('PolygonMeshEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    PolygonMeshEntityThreejsRenderer.clearCache();
  });

  const createBasicPolygonMeshData = (): PolygonMeshData => ({
    Vertices: [
      { Location: { X: 0, Y: 0, Z: 0 }, Normal: { X: 0, Y: 0, Z: 1 }, Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 } },
      { Location: { X: 10, Y: 0, Z: 0 }, Normal: { X: 0, Y: 0, Z: 1 }, Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 } },
      { Location: { X: 5, Y: 10, Z: 0 }, Normal: { X: 0, Y: 0, Z: 1 }, Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 } }
    ],
    Faces: [
      { VertexIndices: [0, 1, 2], Normal: { X: 0, Y: 0, Z: 1 } }
    ],
    Edges: [
      { StartIndex: 0, EndIndex: 1, Crease: 0 },
      { StartIndex: 1, EndIndex: 2, Crease: 0 },
      { StartIndex: 2, EndIndex: 0, Crease: 0 }
    ],
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.03,
    VertexCount: 3,
    FaceCount: 1,
    EdgeCount: 3,
    SubdivisionLevel: 0,
    Version: 1,
    BlendCrease: false,
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
    EntityType: 'Mesh',
    Visible: true,
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Handle: 'polygonmesh-handle-1',
    Transparency: 0,
    MaterialName: 'Standard',
    CastShadows: true,
    ReceiveShadows: true,
    GeometryType: 'BufferGeometry',
    DoubleSided: false,
    FlatShading: false
  });

  describe('render', () => {
    it('should render a basic PolygonMesh', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.name).toBe('PolygonMesh_polygonmesh-handle-1');
    });

    it('should not add invisible PolygonMesh to scene', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Visible = false;

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.visible).toBe(false);
      expect(scene.children.length).toBe(0);
    });

    it('should render PolygonMesh with correct Mesh object', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      expect(group.children.length).toBe(2);
      const mesh = group.children[0] as THREE.Mesh;
      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.name).toBe('PolygonMesh');
    });

    it('should render PolygonMesh with correct material', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
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

    it('should render PolygonMesh with correct geometry', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.geometry).toBeDefined();
      expect(mesh.geometry).toBeInstanceOf(THREE.BufferGeometry);
      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position).toBeDefined();
      expect(geometry.attributes.position.count).toBe(3);
      expect(geometry.attributes.normal).toBeDefined();
      expect(geometry.attributes.color).toBeDefined();
    });

    it('should render PolygonMesh with correct vertices', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
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

    it('should render PolygonMesh with correct normals', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const normals = geometry.attributes.normal.array as Float32Array;
      expect(normals[0]).toBe(0);
      expect(normals[1]).toBe(0);
      expect(normals[2]).toBe(1);
    });

    it('should render PolygonMesh with correct colors', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const colors = geometry.attributes.color.array as Float32Array;
      expect(colors[0]).toBe(1);
      expect(colors[1]).toBe(0);
      expect(colors[2]).toBe(0);
    });

    it('should render PolygonMesh with correct indices', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const indices = geometry.index?.array as Uint16Array;
      expect(indices).toBeDefined();
      expect(indices[0]).toBe(0);
      expect(indices[1]).toBe(1);
      expect(indices[2]).toBe(2);
    });

    it('should render PolygonMesh with edges', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      expect(group.children.length).toBe(2);
      const edges = group.children[1] as THREE.LineSegments;
      expect(edges).toBeInstanceOf(THREE.LineSegments);
      expect(edges.name).toBe('PolygonMeshEdges');
    });

    it('should render PolygonMesh with correct edge geometry', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const edges = group.children[1] as THREE.LineSegments;

      const geometry = edges.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position).toBeDefined();
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions.length).toBe(18);
      expect(positions[0]).toBe(0);
      expect(positions[1]).toBe(0);
      expect(positions[2]).toBe(0);
      expect(positions[3]).toBe(10);
      expect(positions[4]).toBe(0);
      expect(positions[5]).toBe(0);
    });

    it('should render PolygonMesh with correct edge material', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const edges = group.children[1] as THREE.LineSegments;

      expect(edges.material).toBeDefined();
      expect(edges.material).toBeInstanceOf(THREE.LineBasicMaterial);
      const material = edges.material as THREE.LineBasicMaterial;
      expect(material.color.getHex()).toBe(0x000000);
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(false);
    });

    it('should render PolygonMesh without edges when no edges defined', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Edges = [];

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      expect(group.children.length).toBe(1);
      const edges = PolygonMeshEntityThreejsRenderer.getEdgesGroup(polygonMeshData);
      expect(edges).toBeNull();
    });

    it('should render PolygonMesh with correct color', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should render PolygonMesh with correct transparency', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Transparency = 0.5;

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should render PolygonMesh with double-sided material', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.DoubleSided = true;

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.side).toBe(THREE.DoubleSide);
    });

    it('should render PolygonMesh with flat shading', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.FlatShading = true;

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.flatShading).toBe(true);
    });

    it('should render PolygonMesh with correct shadow settings', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.castShadow).toBe(true);
      expect(mesh.receiveShadow).toBe(true);
    });

    it('should cache PolygonMesh by handle', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const cachedMesh = PolygonMeshEntityThreejsRenderer.getMeshGroup(polygonMeshData);
      expect(cachedMesh).toBeDefined();
      expect(cachedMesh).toBeInstanceOf(THREE.Mesh);
    });

    it('should cache PolygonMesh edges by handle', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const cachedEdges = PolygonMeshEntityThreejsRenderer.getEdgesGroup(polygonMeshData);
      expect(cachedEdges).toBeDefined();
      expect(cachedEdges).toBeInstanceOf(THREE.LineSegments);
    });

    it('should render PolygonMesh with correct handle in userData', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.userData.handle).toBe('polygonmesh-handle-1');
    });

    it('should render PolygonMesh with UV coordinates', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.UVsArray = [0, 0, 1, 0, 0.5, 1];

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
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

    it('should render PolygonMesh with multiple faces', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Vertices = [
        { Location: { X: 0, Y: 0, Z: 0 }, Normal: { X: 0, Y: 0, Z: 1 }, Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 } },
        { Location: { X: 10, Y: 0, Z: 0 }, Normal: { X: 0, Y: 0, Z: 1 }, Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 } },
        { Location: { X: 5, Y: 10, Z: 0 }, Normal: { X: 0, Y: 0, Z: 1 }, Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 } },
        { Location: { X: 10, Y: 10, Z: 0 }, Normal: { X: 0, Y: 0, Z: 1 }, Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 } }
      ];
      polygonMeshData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 0, Z: 0 },
        { X: 5, Y: 10, Z: 0 },
        { X: 10, Y: 10, Z: 0 }
      ];
      polygonMeshData.Faces = [
        { VertexIndices: [0, 1, 2], Normal: { X: 0, Y: 0, Z: 1 } },
        { VertexIndices: [1, 3, 2], Normal: { X: 0, Y: 0, Z: 1 } }
      ];
      polygonMeshData.Edges = [
        { StartIndex: 0, EndIndex: 1, Crease: 0 },
        { StartIndex: 1, EndIndex: 2, Crease: 0 },
        { StartIndex: 2, EndIndex: 0, Crease: 0 },
        { StartIndex: 1, EndIndex: 3, Crease: 0 },
        { StartIndex: 3, EndIndex: 2, Crease: 0 }
      ];
      polygonMeshData.Indices = [0, 1, 2, 1, 3, 2];
      polygonMeshData.NormalsArray = [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1];
      polygonMeshData.ColorsArray = [1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0];
      polygonMeshData.FaceCount = 2;
      polygonMeshData.VertexCount = 4;
      polygonMeshData.EdgeCount = 5;

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position.count).toBe(4);
      const indices = geometry.index?.array as Uint16Array;
      expect(indices.length).toBe(6);
    });

    it('should render PolygonMesh with subdivision level', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.SubdivisionLevel = 1;

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render PolygonMesh with version', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Version = 2;

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render PolygonMesh with blend crease', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.BlendCrease = true;

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render PolygonMesh with edge crease values', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Edges = [
        { StartIndex: 0, EndIndex: 1, Crease: 1.0 },
        { StartIndex: 1, EndIndex: 2, Crease: 0.5 },
        { StartIndex: 2, EndIndex: 0, Crease: 0.0 }
      ];

      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const edges = group.children[1] as THREE.LineSegments;
      expect(edges).toBeInstanceOf(THREE.LineSegments);
    });
  });

  describe('update', () => {
    it('should update existing PolygonMesh', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const updatedPolygonMeshData = { ...polygonMeshData };
      updatedPolygonMeshData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const result = PolygonMeshEntityThreejsRenderer.update(updatedPolygonMeshData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent PolygonMesh', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const result = PolygonMeshEntityThreejsRenderer.update(polygonMeshData, scene);

      expect(result).toBe(false);
    });

    it('should update PolygonMesh color', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const updatedPolygonMeshData = { ...polygonMeshData };
      updatedPolygonMeshData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const result = PolygonMeshEntityThreejsRenderer.update(updatedPolygonMeshData, scene);
      const mesh = PolygonMeshEntityThreejsRenderer.getMeshGroup(polygonMeshData) as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;

      expect(result).toBe(true);
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should update PolygonMesh transparency', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const updatedPolygonMeshData = { ...polygonMeshData };
      updatedPolygonMeshData.Transparency = 0.7;

      const result = PolygonMeshEntityThreejsRenderer.update(updatedPolygonMeshData, scene);
      const mesh = PolygonMeshEntityThreejsRenderer.getMeshGroup(polygonMeshData) as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;

      expect(result).toBe(true);
      expect(material.opacity).toBeCloseTo(0.3);
      expect(material.transparent).toBe(true);
    });

    it('should update PolygonMesh vertices', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const updatedPolygonMeshData = { ...polygonMeshData };
      updatedPolygonMeshData.Vertices3D = [
        { X: 10, Y: 10, Z: 10 },
        { X: 20, Y: 10, Z: 10 },
        { X: 15, Y: 20, Z: 10 }
      ];

      const result = PolygonMeshEntityThreejsRenderer.update(updatedPolygonMeshData, scene);

      expect(result).toBe(true);
    });

    it('should update PolygonMesh normals', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const updatedPolygonMeshData = { ...polygonMeshData };
      updatedPolygonMeshData.NormalsArray = [0, 1, 0, 0, 1, 0, 0, 1, 0];

      const result = PolygonMeshEntityThreejsRenderer.update(updatedPolygonMeshData, scene);

      expect(result).toBe(true);
    });

    it('should update PolygonMesh colors', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const updatedPolygonMeshData = { ...polygonMeshData };
      updatedPolygonMeshData.ColorsArray = [0, 1, 0, 0, 1, 0, 0, 1, 0];

      const result = PolygonMeshEntityThreejsRenderer.update(updatedPolygonMeshData, scene);

      expect(result).toBe(true);
    });

    it('should update PolygonMesh material properties', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const updatedPolygonMeshData = { ...polygonMeshData };
      updatedPolygonMeshData.DoubleSided = true;
      updatedPolygonMeshData.FlatShading = true;

      const result = PolygonMeshEntityThreejsRenderer.update(updatedPolygonMeshData, scene);
      const mesh = PolygonMeshEntityThreejsRenderer.getMeshGroup(polygonMeshData) as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;

      expect(result).toBe(true);
      expect(material.side).toBe(THREE.DoubleSide);
      expect(material.flatShading).toBe(true);
    });

    it('should update PolygonMesh edges', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const updatedPolygonMeshData = { ...polygonMeshData };
      updatedPolygonMeshData.Edges = [
        { StartIndex: 0, EndIndex: 1, Crease: 1.0 },
        { StartIndex: 1, EndIndex: 2, Crease: 0.5 },
        { StartIndex: 2, EndIndex: 0, Crease: 0.0 }
      ];

      const result = PolygonMeshEntityThreejsRenderer.update(updatedPolygonMeshData, scene);

      expect(result).toBe(true);
    });
  });

  describe('dispose', () => {
    it('should dispose PolygonMesh resources', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const result = PolygonMeshEntityThreejsRenderer.dispose(polygonMeshData, scene);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(0);
    });

    it('should return false for non-existent PolygonMesh', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const result = PolygonMeshEntityThreejsRenderer.dispose(polygonMeshData, scene);

      expect(result).toBe(false);
    });

    it('should remove PolygonMesh from cache', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      PolygonMeshEntityThreejsRenderer.dispose(polygonMeshData, scene);

      const cachedMesh = PolygonMeshEntityThreejsRenderer.getMeshGroup(polygonMeshData);
      expect(cachedMesh).toBeNull();
    });

    it('should remove PolygonMesh edges from cache', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      PolygonMeshEntityThreejsRenderer.dispose(polygonMeshData, scene);

      const cachedEdges = PolygonMeshEntityThreejsRenderer.getEdgesGroup(polygonMeshData);
      expect(cachedEdges).toBeNull();
    });

    it('should dispose PolygonMesh geometry', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const mesh = PolygonMeshEntityThreejsRenderer.getMeshGroup(polygonMeshData) as THREE.Mesh;

      const geometry = mesh.geometry;
      const originalDispose = geometry.dispose.bind(geometry);
      let disposeCalled = false;
      geometry.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      PolygonMeshEntityThreejsRenderer.dispose(polygonMeshData, scene);

      expect(disposeCalled).toBe(true);
    });

    it('should dispose PolygonMesh material', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const mesh = PolygonMeshEntityThreejsRenderer.getMeshGroup(polygonMeshData) as THREE.Mesh;

      const material = mesh.material;
      const originalDispose = material.dispose.bind(material);
      let disposeCalled = false;
      material.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      PolygonMeshEntityThreejsRenderer.dispose(polygonMeshData, scene);

      expect(disposeCalled).toBe(true);
    });

    it('should dispose PolygonMesh edges geometry', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const edges = PolygonMeshEntityThreejsRenderer.getEdgesGroup(polygonMeshData) as THREE.LineSegments;

      const geometry = edges.geometry;
      const originalDispose = geometry.dispose.bind(geometry);
      let disposeCalled = false;
      geometry.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      PolygonMeshEntityThreejsRenderer.dispose(polygonMeshData, scene);

      expect(disposeCalled).toBe(true);
    });

    it('should dispose PolygonMesh edges material', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);
      const edges = PolygonMeshEntityThreejsRenderer.getEdgesGroup(polygonMeshData) as THREE.LineSegments;

      const material = edges.material;
      const originalDispose = material.dispose.bind(material);
      let disposeCalled = false;
      material.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      PolygonMeshEntityThreejsRenderer.dispose(polygonMeshData, scene);

      expect(disposeCalled).toBe(true);
    });
  });

  describe('setVisibility', () => {
    it('should set PolygonMesh visibility to true', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const result = PolygonMeshEntityThreejsRenderer.setVisibility(polygonMeshData, scene, true);

      expect(result).toBe(true);
      expect(group.visible).toBe(true);
    });

    it('should set PolygonMesh visibility to false', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const result = PolygonMeshEntityThreejsRenderer.setVisibility(polygonMeshData, scene, false);

      expect(result).toBe(true);
      expect(group.visible).toBe(false);
    });

    it('should return false for non-existent PolygonMesh', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const result = PolygonMeshEntityThreejsRenderer.setVisibility(polygonMeshData, scene, true);

      expect(result).toBe(false);
    });

    it('should add PolygonMesh to scene when setting visibility to true', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Visible = false;
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      PolygonMeshEntityThreejsRenderer.setVisibility(polygonMeshData, scene, true);

      expect(scene.children.length).toBe(1);
    });

    it('should remove PolygonMesh from scene when setting visibility to false', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      PolygonMeshEntityThreejsRenderer.setVisibility(polygonMeshData, scene, false);

      expect(scene.children.length).toBe(0);
    });

    it('should set edges visibility', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      PolygonMeshEntityThreejsRenderer.setVisibility(polygonMeshData, scene, false);

      const edges = PolygonMeshEntityThreejsRenderer.getEdgesGroup(polygonMeshData);
      expect(edges).not.toBeNull();
      expect(edges?.visible).toBe(false);
    });
  });

  describe('setOpacity', () => {
    it('should set PolygonMesh opacity', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const result = PolygonMeshEntityThreejsRenderer.setOpacity(polygonMeshData, 0.5);

      expect(result).toBe(true);
      const mesh = PolygonMeshEntityThreejsRenderer.getMeshGroup(polygonMeshData) as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should return false for non-existent PolygonMesh', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const result = PolygonMeshEntityThreejsRenderer.setOpacity(polygonMeshData, 0.5);

      expect(result).toBe(false);
    });

    it('should set opacity to 1.0 and transparent to false', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const result = PolygonMeshEntityThreejsRenderer.setOpacity(polygonMeshData, 1.0);

      expect(result).toBe(true);
      const mesh = PolygonMeshEntityThreejsRenderer.getMeshGroup(polygonMeshData) as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
    });
  });

  describe('setColor', () => {
    it('should set PolygonMesh color', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const group = PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const color = new THREE.Color('#00FF00');
      const result = PolygonMeshEntityThreejsRenderer.setColor(polygonMeshData, color);

      expect(result).toBe(true);
      const mesh = PolygonMeshEntityThreejsRenderer.getMeshGroup(polygonMeshData) as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should return false for non-existent PolygonMesh', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      const color = new THREE.Color('#00FF00');

      const result = PolygonMeshEntityThreejsRenderer.setColor(polygonMeshData, color);

      expect(result).toBe(false);
    });
  });

  describe('getMeshGroup', () => {
    it('should return cached mesh', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const mesh = PolygonMeshEntityThreejsRenderer.getMeshGroup(polygonMeshData);

      expect(mesh).toBeDefined();
      expect(mesh).toBeInstanceOf(THREE.Mesh);
    });

    it('should return null for non-existent mesh', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const mesh = PolygonMeshEntityThreejsRenderer.getMeshGroup(polygonMeshData);

      expect(mesh).toBeNull();
    });
  });

  describe('getEdgesGroup', () => {
    it('should return cached edges', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      const edges = PolygonMeshEntityThreejsRenderer.getEdgesGroup(polygonMeshData);

      expect(edges).toBeDefined();
      expect(edges).toBeInstanceOf(THREE.LineSegments);
    });

    it('should return null for non-existent edges', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const edges = PolygonMeshEntityThreejsRenderer.getEdgesGroup(polygonMeshData);

      expect(edges).toBeNull();
    });
  });

  describe('getVertices', () => {
    it('should return vertices as THREE.Vector3 array', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const vertices = PolygonMeshEntityThreejsRenderer.getVertices(polygonMeshData);

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
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Vertices3D = [];

      const vertices = PolygonMeshEntityThreejsRenderer.getVertices(polygonMeshData);

      expect(vertices).toHaveLength(0);
    });
  });

  describe('getIndices', () => {
    it('should return indices', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const indices = PolygonMeshEntityThreejsRenderer.getIndices(polygonMeshData);

      expect(indices).toEqual([0, 1, 2]);
    });

    it('should return empty array for no indices', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Indices = [];

      const indices = PolygonMeshEntityThreejsRenderer.getIndices(polygonMeshData);

      expect(indices).toEqual([]);
    });
  });

  describe('getNormals', () => {
    it('should return normals as THREE.Vector3 array', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const normals = PolygonMeshEntityThreejsRenderer.getNormals(polygonMeshData);

      expect(normals).toHaveLength(3);
      expect(normals[0]).toBeInstanceOf(THREE.Vector3);
      expect(normals[0].x).toBe(0);
      expect(normals[0].y).toBe(0);
      expect(normals[0].z).toBe(1);
    });

    it('should return empty array for empty normals', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Normals = [];

      const normals = PolygonMeshEntityThreejsRenderer.getNormals(polygonMeshData);

      expect(normals).toHaveLength(0);
    });
  });

  describe('getFaces', () => {
    it('should return faces', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const faces = PolygonMeshEntityThreejsRenderer.getFaces(polygonMeshData);

      expect(faces).toHaveLength(1);
      expect(faces[0].VertexIndices).toEqual([0, 1, 2]);
      expect(faces[0].Normal).toEqual({ X: 0, Y: 0, Z: 1 });
    });

    it('should return empty array for no faces', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Faces = [];

      const faces = PolygonMeshEntityThreejsRenderer.getFaces(polygonMeshData);

      expect(faces).toEqual([]);
    });
  });

  describe('getEdges', () => {
    it('should return edges', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const edges = PolygonMeshEntityThreejsRenderer.getEdges(polygonMeshData);

      expect(edges).toHaveLength(3);
      expect(edges[0].StartIndex).toBe(0);
      expect(edges[0].EndIndex).toBe(1);
      expect(edges[0].Crease).toBe(0);
    });

    it('should return empty array for no edges', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Edges = [];

      const edges = PolygonMeshEntityThreejsRenderer.getEdges(polygonMeshData);

      expect(edges).toEqual([]);
    });
  });

  describe('getColor', () => {
    it('should return color as THREE.Color', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const color = PolygonMeshEntityThreejsRenderer.getColor(polygonMeshData);

      expect(color).toBeInstanceOf(THREE.Color);
      expect(color.getHexString()).toBe('ff0000');
    });
  });

  describe('getBounds', () => {
    it('should return bounds as THREE.Box3', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const bounds = PolygonMeshEntityThreejsRenderer.getBounds(polygonMeshData);

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
      const polygonMeshData = createBasicPolygonMeshData();

      const centroid = PolygonMeshEntityThreejsRenderer.getCentroid(polygonMeshData);

      expect(centroid).toBeInstanceOf(THREE.Vector3);
      expect(centroid.x).toBe(5);
      expect(centroid.y).toBeCloseTo(3.33, 2);
      expect(centroid.z).toBe(0);
    });
  });

  describe('getVertexCount', () => {
    it('should return vertex count', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const count = PolygonMeshEntityThreejsRenderer.getVertexCount(polygonMeshData);

      expect(count).toBe(3);
    });

    it('should return 0 for no vertices', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.VertexCount = 0;

      const count = PolygonMeshEntityThreejsRenderer.getVertexCount(polygonMeshData);

      expect(count).toBe(0);
    });
  });

  describe('getFaceCount', () => {
    it('should return face count', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const count = PolygonMeshEntityThreejsRenderer.getFaceCount(polygonMeshData);

      expect(count).toBe(1);
    });

    it('should return 0 for no faces', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.FaceCount = 0;

      const count = PolygonMeshEntityThreejsRenderer.getFaceCount(polygonMeshData);

      expect(count).toBe(0);
    });
  });

  describe('getEdgeCount', () => {
    it('should return edge count', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const count = PolygonMeshEntityThreejsRenderer.getEdgeCount(polygonMeshData);

      expect(count).toBe(3);
    });

    it('should return 0 for no edges', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.EdgeCount = 0;

      const count = PolygonMeshEntityThreejsRenderer.getEdgeCount(polygonMeshData);

      expect(count).toBe(0);
    });
  });

  describe('getSubdivisionLevel', () => {
    it('should return subdivision level', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.SubdivisionLevel = 2;

      const level = PolygonMeshEntityThreejsRenderer.getSubdivisionLevel(polygonMeshData);

      expect(level).toBe(2);
    });

    it('should return 0 for no subdivision level', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.SubdivisionLevel = 0;

      const level = PolygonMeshEntityThreejsRenderer.getSubdivisionLevel(polygonMeshData);

      expect(level).toBe(0);
    });
  });

  describe('getVersion', () => {
    it('should return version', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Version = 3;

      const version = PolygonMeshEntityThreejsRenderer.getVersion(polygonMeshData);

      expect(version).toBe(3);
    });

    it('should return 0 for no version', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Version = 0;

      const version = PolygonMeshEntityThreejsRenderer.getVersion(polygonMeshData);

      expect(version).toBe(0);
    });
  });

  describe('getBlendCrease', () => {
    it('should return blend crease', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.BlendCrease = true;

      const blendCrease = PolygonMeshEntityThreejsRenderer.getBlendCrease(polygonMeshData);

      expect(blendCrease).toBe(true);
    });

    it('should return false for no blend crease', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.BlendCrease = false;

      const blendCrease = PolygonMeshEntityThreejsRenderer.getBlendCrease(polygonMeshData);

      expect(blendCrease).toBe(false);
    });
  });

  describe('clearCache', () => {
    it('should clear mesh cache', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      PolygonMeshEntityThreejsRenderer.clearCache();

      const mesh = PolygonMeshEntityThreejsRenderer.getMeshGroup(polygonMeshData);
      expect(mesh).toBeNull();
    });

    it('should clear edges cache', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      PolygonMeshEntityThreejsRenderer.render(polygonMeshData, scene);

      PolygonMeshEntityThreejsRenderer.clearCache();

      const edges = PolygonMeshEntityThreejsRenderer.getEdgesGroup(polygonMeshData);
      expect(edges).toBeNull();
    });
  });

  describe('getTransform', () => {
    it('should return transform data', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const transform = PolygonMeshEntityThreejsRenderer.getTransform(polygonMeshData);

      expect(transform.Position).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(transform.Rotation).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(transform.Scale).toEqual({ X: 1, Y: 1, Z: 1 });
      expect(transform.Matrix).toEqual([1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]);
    });

    it('should return default transform when not set', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Transform = undefined as any;

      const transform = PolygonMeshEntityThreejsRenderer.getTransform(polygonMeshData);

      expect(transform.Position).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(transform.Rotation).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(transform.Scale).toEqual({ X: 1, Y: 1, Z: 1 });
    });
  });

  describe('getBounds3D', () => {
    it('should return bounds data', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const bounds = PolygonMeshEntityThreejsRenderer.getBounds3D(polygonMeshData);

      expect(bounds.Min).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(bounds.Max).toEqual({ X: 10, Y: 10, Z: 0 });
    });

    it('should return default bounds when not set', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Bounds3D = undefined as any;

      const bounds = PolygonMeshEntityThreejsRenderer.getBounds3D(polygonMeshData);

      expect(bounds.Min).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(bounds.Max).toEqual({ X: 0, Y: 0, Z: 0 });
    });
  });

  describe('getCentroid3D', () => {
    it('should return centroid data', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const centroid = PolygonMeshEntityThreejsRenderer.getCentroid3D(polygonMeshData);

      expect(centroid).toEqual({ X: 5, Y: 3.33, Z: 0 });
    });

    it('should return default centroid when not set', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Centroid = undefined as any;

      const centroid = PolygonMeshEntityThreejsRenderer.getCentroid3D(polygonMeshData);

      expect(centroid).toEqual({ X: 0, Y: 0, Z: 0 });
    });
  });

  describe('getHandle', () => {
    it('should return handle', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const handle = PolygonMeshEntityThreejsRenderer.getHandle(polygonMeshData);

      expect(handle).toBe('polygonmesh-handle-1');
    });

    it('should return empty string when handle not set', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Handle = undefined as any;

      const handle = PolygonMeshEntityThreejsRenderer.getHandle(polygonMeshData);

      expect(handle).toBe('');
    });
  });

  describe('getLayerName', () => {
    it('should return layer name', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const layerName = PolygonMeshEntityThreejsRenderer.getLayerName(polygonMeshData);

      expect(layerName).toBe('TEST_LAYER');
    });

    it('should return empty string when layer name not set', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.LayerName = undefined as any;

      const layerName = PolygonMeshEntityThreejsRenderer.getLayerName(polygonMeshData);

      expect(layerName).toBe('');
    });
  });

  describe('getVisible', () => {
    it('should return true when visible', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const visible = PolygonMeshEntityThreejsRenderer.getVisible(polygonMeshData);

      expect(visible).toBe(true);
    });

    it('should return false when not visible', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Visible = false;

      const visible = PolygonMeshEntityThreejsRenderer.getVisible(polygonMeshData);

      expect(visible).toBe(false);
    });

    it('should return true when visibility not set', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Visible = undefined as any;

      const visible = PolygonMeshEntityThreejsRenderer.getVisible(polygonMeshData);

      expect(visible).toBe(true);
    });
  });

  describe('getOpacity', () => {
    it('should return opacity', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Transparency = 0.5;

      const opacity = PolygonMeshEntityThreejsRenderer.getOpacity(polygonMeshData);

      expect(opacity).toBe(0.5);
    });

    it('should return 1.0 when transparency not set', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Transparency = undefined as any;

      const opacity = PolygonMeshEntityThreejsRenderer.getOpacity(polygonMeshData);

      expect(opacity).toBe(1.0);
    });
  });

  describe('getTransparent', () => {
    it('should return true when transparent', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.Transparency = 0.5;

      const transparent = PolygonMeshEntityThreejsRenderer.getTransparent(polygonMeshData);

      expect(transparent).toBe(true);
    });

    it('should return false when not transparent', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const transparent = PolygonMeshEntityThreejsRenderer.getTransparent(polygonMeshData);

      expect(transparent).toBe(false);
    });
  });

  describe('getCastShadows', () => {
    it('should return true when cast shadows', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const castShadows = PolygonMeshEntityThreejsRenderer.getCastShadows(polygonMeshData);

      expect(castShadows).toBe(true);
    });

    it('should return false when not cast shadows', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.CastShadows = false;

      const castShadows = PolygonMeshEntityThreejsRenderer.getCastShadows(polygonMeshData);

      expect(castShadows).toBe(false);
    });

    it('should return true when cast shadows not set', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.CastShadows = undefined as any;

      const castShadows = PolygonMeshEntityThreejsRenderer.getCastShadows(polygonMeshData);

      expect(castShadows).toBe(true);
    });
  });

  describe('getReceiveShadows', () => {
    it('should return true when receive shadows', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const receiveShadows = PolygonMeshEntityThreejsRenderer.getReceiveShadows(polygonMeshData);

      expect(receiveShadows).toBe(true);
    });

    it('should return false when not receive shadows', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.ReceiveShadows = false;

      const receiveShadows = PolygonMeshEntityThreejsRenderer.getReceiveShadows(polygonMeshData);

      expect(receiveShadows).toBe(false);
    });

    it('should return true when receive shadows not set', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.ReceiveShadows = undefined as any;

      const receiveShadows = PolygonMeshEntityThreejsRenderer.getReceiveShadows(polygonMeshData);

      expect(receiveShadows).toBe(true);
    });
  });

  describe('getDoubleSided', () => {
    it('should return true when double sided', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.DoubleSided = true;

      const doubleSided = PolygonMeshEntityThreejsRenderer.getDoubleSided(polygonMeshData);

      expect(doubleSided).toBe(true);
    });

    it('should return false when not double sided', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const doubleSided = PolygonMeshEntityThreejsRenderer.getDoubleSided(polygonMeshData);

      expect(doubleSided).toBe(false);
    });
  });

  describe('getFlatShading', () => {
    it('should return true when flat shading', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.FlatShading = true;

      const flatShading = PolygonMeshEntityThreejsRenderer.getFlatShading(polygonMeshData);

      expect(flatShading).toBe(true);
    });

    it('should return false when not flat shading', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const flatShading = PolygonMeshEntityThreejsRenderer.getFlatShading(polygonMeshData);

      expect(flatShading).toBe(false);
    });
  });

  describe('getGeometryType', () => {
    it('should return geometry type', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const geometryType = PolygonMeshEntityThreejsRenderer.getGeometryType(polygonMeshData);

      expect(geometryType).toBe('BufferGeometry');
    });

    it('should return default geometry type when not set', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.GeometryType = undefined as any;

      const geometryType = PolygonMeshEntityThreejsRenderer.getGeometryType(polygonMeshData);

      expect(geometryType).toBe('BufferGeometry');
    });
  });

  describe('getEntityType', () => {
    it('should return entity type', () => {
      const polygonMeshData = createBasicPolygonMeshData();

      const entityType = PolygonMeshEntityThreejsRenderer.getEntityType(polygonMeshData);

      expect(entityType).toBe('Mesh');
    });

    it('should return default entity type when not set', () => {
      const polygonMeshData = createBasicPolygonMeshData();
      polygonMeshData.EntityType = undefined as any;

      const entityType = PolygonMeshEntityThreejsRenderer.getEntityType(polygonMeshData);

      expect(entityType).toBe('Mesh');
    });
  });
});
