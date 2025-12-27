import * as THREE from 'three';
import { MeshEntityThreejsRenderer, MeshData, Point3DData, PointData, BoundsData, BoundsData3D, ColorData, TransformData, NormalData, VertexData, FaceData, EdgeData } from '../MeshEntityThreejsRenderer';

describe('MeshEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
  });

  const createBasicMeshData = (): MeshData => ({
    Type: 'Mesh',
    EntityType: 'MESH',
    Handle: 'handle-1',
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Visible: true,
    CoordinateSystem: 'World',
    Vertices: [
      { X: 0, Y: 0 },
      { X: 10, Y: 0 },
      { X: 5, Y: 10 }
    ],
    Faces: [
      { VertexIndices: [0, 1, 2] }
    ],
    Edges: [
      { Start: 0, End: 1 },
      { Start: 1, End: 2 },
      { Start: 2, End: 0 }
    ],
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.25,
    VertexCount: 3,
    FaceCount: 1,
    EdgeCount: 3,
    Bounds: {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 10, Y: 10, Z: 0 },
      Center: { X: 5, Y: 5, Z: 0 },
      Size: { X: 10, Y: 10, Z: 0 }
    },
    Centroid: { X: 5, Y: 3.3333333333333335 },
    Vertices3D: [
      { X: 0, Y: 0, Z: 0 },
      { X: 10, Y: 0, Z: 0 },
      { X: 5, Y: 10, Z: 0 }
    ],
    Centroid3D: { X: 5, Y: 3.3333333333333335, Z: 0 },
    Normals: [
      { X: 0, Y: 0, Z: 1 },
      { X: 0, Y: 0, Z: 1 },
      { X: 0, Y: 0, Z: 1 }
    ],
    Bounds3D: {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 10, Y: 10, Z: 0 },
      Center: { X: 5, Y: 5, Z: 0 },
      Size: { X: 10, Y: 10, Z: 0 }
    },
    Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 },
    Transform: {
      Position: { X: 0, Y: 0, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
    },
    Normal: { X: 0, Y: 0, Z: 1 },
    Opacity: 1.0,
    Transparent: false,
    MaterialType: 'MeshStandardMaterial',
    DepthTest: true,
    DepthWrite: true,
    Side: true,
    SubdivisionLevel: 0,
    Version: 0,
    BlendCrease: false,
    FaceIndices: [
      [0, 1, 2]
    ],
    EdgeIndices: [
      [0, 1],
      [1, 2],
      [2, 0]
    ]
  });

  describe('render', () => {
    it('should render a basic mesh', () => {
      const meshData = createBasicMeshData();

      const group = MeshEntityThreejsRenderer.render(meshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.userData.type).toBe('Mesh');
      expect(group.userData.entityType).toBe('MESH');
      expect(group.userData.handle).toBe('handle-1');
      expect(group.userData.layerName).toBe('TEST_LAYER');
    });

    it('should return null for invisible mesh', () => {
      const meshData = createBasicMeshData();
      meshData.Visible = false;

      const group = MeshEntityThreejsRenderer.render(meshData, scene);

      expect(group).toBeNull();
    });

    it('should return null for null data', () => {
      const group = MeshEntityThreejsRenderer.render(null as any, scene);

      expect(group).toBeNull();
    });

    it('should return null for mesh with no vertices', () => {
      const meshData = createBasicMeshData();
      meshData.Vertices3D = [];
      meshData.VertexCount = 0;

      const group = MeshEntityThreejsRenderer.render(meshData, scene);

      expect(group).toBeNull();
    });

    it('should render mesh with correct vertices', () => {
      const meshData = createBasicMeshData();
      meshData.Vertices3D = [
        { X: 5, Y: 5, Z: 0 },
        { X: 15, Y: 5, Z: 0 },
        { X: 10, Y: 15, Z: 0 }
      ];

      const group = MeshEntityThreejsRenderer.render(meshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBeGreaterThan(0);
      const mesh = group.children.find(child => child.name === 'Mesh');
      expect(mesh).toBeInstanceOf(THREE.Mesh);
    });

    it('should render mesh with correct color', () => {
      const meshData = createBasicMeshData();
      meshData.Color = { Index: 5, Hex: '#0000FF', R: 0, G: 0, B: 255, A: 1.0 };
      meshData.ColorIndex = 5;

      const group = MeshEntityThreejsRenderer.render(meshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const mesh = group.children.find(child => child.name === 'Mesh') as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.color.getHexString()).toBe('0000ff');
    });

    it('should render mesh with correct opacity', () => {
      const meshData = createBasicMeshData();
      meshData.Opacity = 0.5;
      meshData.Transparent = true;

      const group = MeshEntityThreejsRenderer.render(meshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const mesh = group.children.find(child => child.name === 'Mesh') as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should render mesh with correct material type', () => {
      const meshData = createBasicMeshData();
      meshData.MaterialType = 'MeshBasicMaterial';

      const group = MeshEntityThreejsRenderer.render(meshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const mesh = group.children.find(child => child.name === 'Mesh') as THREE.Mesh;
      expect(mesh.material).toBeInstanceOf(THREE.MeshBasicMaterial);
    });

    it('should render mesh with double side', () => {
      const meshData = createBasicMeshData();
      meshData.Side = true;

      const group = MeshEntityThreejsRenderer.render(meshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const mesh = group.children.find(child => child.name === 'Mesh') as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.side).toBe(THREE.DoubleSide);
    });

    it('should render mesh with edges', () => {
      const meshData = createBasicMeshData();

      const group = MeshEntityThreejsRenderer.render(meshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const edges = group.children.find(child => child.name === 'Edges');
      expect(edges).toBeInstanceOf(THREE.LineSegments);
    });

    it('should apply transform matrix', () => {
      const meshData = createBasicMeshData();
      meshData.Transform.Matrix = [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 5, 5, 5, 1];

      const group = MeshEntityThreejsRenderer.render(meshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.position.x).toBe(5);
      expect(group.position.y).toBe(5);
      expect(group.position.z).toBe(5);
    });

    it('should render mesh with quadrilateral face', () => {
      const meshData = createBasicMeshData();
      meshData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 0, Z: 0 },
        { X: 10, Y: 10, Z: 0 },
        { X: 0, Y: 10, Z: 0 }
      ];
      meshData.FaceIndices = [[0, 1, 2, 3]];
      meshData.VertexCount = 4;
      meshData.FaceCount = 1;

      const group = MeshEntityThreejsRenderer.render(meshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const mesh = group.children.find(child => child.name === 'Mesh') as THREE.Mesh;
      expect(mesh).toBeInstanceOf(THREE.Mesh);
    });

    it('should compute normals if not provided', () => {
      const meshData = createBasicMeshData();
      meshData.Normals = [];

      const group = MeshEntityThreejsRenderer.render(meshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const mesh = group.children.find(child => child.name === 'Mesh') as THREE.Mesh;
      expect(mesh.geometry.attributes.normal).toBeDefined();
    });

    it('should use provided normals', () => {
      const meshData = createBasicMeshData();
      meshData.Normals = [
        { X: 0, Y: 0, Z: 1 },
        { X: 0, Y: 0, Z: 1 },
        { X: 0, Y: 0, Z: 1 }
      ];

      const group = MeshEntityThreejsRenderer.render(meshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const mesh = group.children.find(child => child.name === 'Mesh') as THREE.Mesh;
      expect(mesh.geometry.attributes.normal).toBeDefined();
    });
  });

  describe('dispose', () => {
    it('should dispose mesh from scene', () => {
      const meshData = createBasicMeshData();
      MeshEntityThreejsRenderer.render(meshData, scene);

      const result = MeshEntityThreejsRenderer.dispose(meshData, scene);

      expect(result).toBe(true);
      const group = scene.getObjectByName(`Mesh_${meshData.Handle}`);
      expect(group).toBeUndefined();
    });

    it('should return false for null data', () => {
      const result = MeshEntityThreejsRenderer.dispose(null as any, scene);

      expect(result).toBe(false);
    });

    it('should return false for null scene', () => {
      const meshData = createBasicMeshData();
      const result = MeshEntityThreejsRenderer.dispose(meshData, null as any);

      expect(result).toBe(false);
    });

    it('should return false for non-existent mesh', () => {
      const meshData = createBasicMeshData();
      meshData.Handle = 'non-existent';

      const result = MeshEntityThreejsRenderer.dispose(meshData, scene);

      expect(result).toBe(false);
    });
  });

  describe('update', () => {
    it('should update existing mesh', () => {
      const meshData = createBasicMeshData();
      MeshEntityThreejsRenderer.render(meshData, scene);

      meshData.Color = { Index: 5, Hex: '#0000FF', R: 0, G: 0, B: 255, A: 1.0 };

      const result = MeshEntityThreejsRenderer.update(meshData, scene);

      expect(result).toBe(true);
      const group = scene.getObjectByName(`Mesh_${meshData.Handle}`);
      expect(group).not.toBeNull();
    });

    it('should return false for non-existent mesh', () => {
      const meshData = createBasicMeshData();

      const result = MeshEntityThreejsRenderer.update(meshData, scene);

      expect(result).toBe(false);
    });
  });

  describe('getMeshGroup', () => {
    it('should get mesh group from scene', () => {
      const meshData = createBasicMeshData();
      MeshEntityThreejsRenderer.render(meshData, scene);

      const group = MeshEntityThreejsRenderer.getMeshGroup(meshData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group?.name).toBe(`Mesh_${meshData.Handle}`);
    });

    it('should return null for non-existent mesh', () => {
      const meshData = createBasicMeshData();

      const group = MeshEntityThreejsRenderer.getMeshGroup(meshData, scene);

      expect(group).toBeNull();
    });
  });

  describe('setVisibility', () => {
    it('should set mesh visibility to true', () => {
      const meshData = createBasicMeshData();
      MeshEntityThreejsRenderer.render(meshData, scene);

      const result = MeshEntityThreejsRenderer.setVisibility(meshData, scene, true);

      expect(result).toBe(true);
      const group = MeshEntityThreejsRenderer.getMeshGroup(meshData, scene);
      expect(group?.visible).toBe(true);
    });

    it('should set mesh visibility to false', () => {
      const meshData = createBasicMeshData();
      MeshEntityThreejsRenderer.render(meshData, scene);

      const result = MeshEntityThreejsRenderer.setVisibility(meshData, scene, false);

      expect(result).toBe(true);
      const group = MeshEntityThreejsRenderer.getMeshGroup(meshData, scene);
      expect(group?.visible).toBe(false);
    });

    it('should return false for non-existent mesh', () => {
      const meshData = createBasicMeshData();

      const result = MeshEntityThreejsRenderer.setVisibility(meshData, scene, false);

      expect(result).toBe(false);
    });
  });

  describe('setColor', () => {
    it('should set mesh color', () => {
      const meshData = createBasicMeshData();
      MeshEntityThreejsRenderer.render(meshData, scene);

      const result = MeshEntityThreejsRenderer.setColor(meshData, scene, '#00FF00');

      expect(result).toBe(true);
      const group = MeshEntityThreejsRenderer.getMeshGroup(meshData, scene);
      const mesh = group?.children.find(child => child.name === 'Mesh') as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should return false for non-existent mesh', () => {
      const meshData = createBasicMeshData();

      const result = MeshEntityThreejsRenderer.setColor(meshData, scene, '#00FF00');

      expect(result).toBe(false);
    });
  });

  describe('setOpacity', () => {
    it('should set mesh opacity', () => {
      const meshData = createBasicMeshData();
      MeshEntityThreejsRenderer.render(meshData, scene);

      const result = MeshEntityThreejsRenderer.setOpacity(meshData, scene, 0.7);

      expect(result).toBe(true);
      const group = MeshEntityThreejsRenderer.getMeshGroup(meshData, scene);
      const mesh = group?.children.find(child => child.name === 'Mesh') as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.opacity).toBe(0.7);
      expect(material.transparent).toBe(true);
    });

    it('should set mesh opacity to 1.0 and transparent to false', () => {
      const meshData = createBasicMeshData();
      meshData.Opacity = 0.5;
      meshData.Transparent = true;
      MeshEntityThreejsRenderer.render(meshData, scene);

      const result = MeshEntityThreejsRenderer.setOpacity(meshData, scene, 1.0);

      expect(result).toBe(true);
      const group = MeshEntityThreejsRenderer.getMeshGroup(meshData, scene);
      const mesh = group?.children.find(child => child.name === 'Mesh') as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
    });

    it('should return false for non-existent mesh', () => {
      const meshData = createBasicMeshData();

      const result = MeshEntityThreejsRenderer.setOpacity(meshData, scene, 0.5);

      expect(result).toBe(false);
    });
  });

  describe('setWireframe', () => {
    it('should set mesh wireframe to true', () => {
      const meshData = createBasicMeshData();
      MeshEntityThreejsRenderer.render(meshData, scene);

      const result = MeshEntityThreejsRenderer.setWireframe(meshData, scene, true);

      expect(result).toBe(true);
      const group = MeshEntityThreejsRenderer.getMeshGroup(meshData, scene);
      const mesh = group?.children.find(child => child.name === 'Mesh') as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.wireframe).toBe(true);
    });

    it('should set mesh wireframe to false', () => {
      const meshData = createBasicMeshData();
      MeshEntityThreejsRenderer.render(meshData, scene);
      MeshEntityThreejsRenderer.setWireframe(meshData, scene, true);

      const result = MeshEntityThreejsRenderer.setWireframe(meshData, scene, false);

      expect(result).toBe(true);
      const group = MeshEntityThreejsRenderer.getMeshGroup(meshData, scene);
      const mesh = group?.children.find(child => child.name === 'Mesh') as THREE.Mesh;
      const material = mesh.material as THREE.MeshStandardMaterial;
      expect(material.wireframe).toBe(false);
    });

    it('should return false for non-existent mesh', () => {
      const meshData = createBasicMeshData();

      const result = MeshEntityThreejsRenderer.setWireframe(meshData, scene, true);

      expect(result).toBe(false);
    });
  });

  describe('getVertexCount', () => {
    it('should return correct vertex count', () => {
      const meshData = createBasicMeshData();
      meshData.VertexCount = 10;

      const count = MeshEntityThreejsRenderer.getVertexCount(meshData);

      expect(count).toBe(10);
    });

    it('should return 0 for undefined vertex count', () => {
      const meshData = createBasicMeshData();
      meshData.VertexCount = undefined as any;

      const count = MeshEntityThreejsRenderer.getVertexCount(meshData);

      expect(count).toBe(0);
    });
  });

  describe('getFaceCount', () => {
    it('should return correct face count', () => {
      const meshData = createBasicMeshData();
      meshData.FaceCount = 5;

      const count = MeshEntityThreejsRenderer.getFaceCount(meshData);

      expect(count).toBe(5);
    });

    it('should return 0 for undefined face count', () => {
      const meshData = createBasicMeshData();
      meshData.FaceCount = undefined as any;

      const count = MeshEntityThreejsRenderer.getFaceCount(meshData);

      expect(count).toBe(0);
    });
  });

  describe('getEdgeCount', () => {
    it('should return correct edge count', () => {
      const meshData = createBasicMeshData();
      meshData.EdgeCount = 8;

      const count = MeshEntityThreejsRenderer.getEdgeCount(meshData);

      expect(count).toBe(8);
    });

    it('should return 0 for undefined edge count', () => {
      const meshData = createBasicMeshData();
      meshData.EdgeCount = undefined as any;

      const count = MeshEntityThreejsRenderer.getEdgeCount(meshData);

      expect(count).toBe(0);
    });
  });

  describe('getCentroid', () => {
    it('should return correct centroid', () => {
      const meshData = createBasicMeshData();
      meshData.Centroid3D = { X: 10, Y: 20, Z: 30 };

      const centroid = MeshEntityThreejsRenderer.getCentroid(meshData);

      expect(centroid.X).toBe(10);
      expect(centroid.Y).toBe(20);
      expect(centroid.Z).toBe(30);
    });

    it('should return default centroid for undefined centroid', () => {
      const meshData = createBasicMeshData();
      meshData.Centroid3D = undefined as any;

      const centroid = MeshEntityThreejsRenderer.getCentroid(meshData);

      expect(centroid.X).toBe(0);
      expect(centroid.Y).toBe(0);
      expect(centroid.Z).toBe(0);
    });
  });

  describe('getBounds', () => {
    it('should return correct bounds', () => {
      const meshData = createBasicMeshData();
      meshData.Bounds3D = {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 10, Y: 10, Z: 10 },
        Center: { X: 5, Y: 5, Z: 5 },
        Size: { X: 10, Y: 10, Z: 10 }
      };

      const bounds = MeshEntityThreejsRenderer.getBounds(meshData);

      expect(bounds.Min.X).toBe(0);
      expect(bounds.Max.X).toBe(10);
      expect(bounds.Center.X).toBe(5);
      expect(bounds.Size.X).toBe(10);
    });

    it('should return default bounds for undefined bounds', () => {
      const meshData = createBasicMeshData();
      meshData.Bounds3D = undefined as any;

      const bounds = MeshEntityThreejsRenderer.getBounds(meshData);

      expect(bounds.Min.X).toBe(0);
      expect(bounds.Max.X).toBe(0);
      expect(bounds.Center.X).toBe(0);
      expect(bounds.Size.X).toBe(0);
    });
  });

  describe('getNormals', () => {
    it('should return correct normals', () => {
      const meshData = createBasicMeshData();
      meshData.Normals = [
        { X: 0, Y: 0, Z: 1 },
        { X: 0, Y: 0, Z: 1 },
        { X: 0, Y: 0, Z: 1 }
      ];

      const normals = MeshEntityThreejsRenderer.getNormals(meshData);

      expect(normals.length).toBe(3);
      expect(normals[0].X).toBe(0);
      expect(normals[0].Y).toBe(0);
      expect(normals[0].Z).toBe(1);
    });

    it('should return empty array for undefined normals', () => {
      const meshData = createBasicMeshData();
      meshData.Normals = undefined as any;

      const normals = MeshEntityThreejsRenderer.getNormals(meshData);

      expect(normals.length).toBe(0);
    });
  });

  describe('calculateVolume', () => {
    it('should calculate volume for tetrahedron', () => {
      const meshData = createBasicMeshData();
      meshData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 1, Y: 0, Z: 0 },
        { X: 0, Y: 1, Z: 0 },
        { X: 0, Y: 0, Z: 1 }
      ];
      meshData.FaceIndices = [[0, 1, 2], [0, 1, 3], [0, 2, 3], [1, 2, 3]];
      meshData.Centroid3D = { X: 0.25, Y: 0.25, Z: 0.25 };

      const volume = MeshEntityThreejsRenderer.calculateVolume(meshData);

      expect(volume).toBeGreaterThan(0);
    });

    it('should return 0 for mesh with no vertices', () => {
      const meshData = createBasicMeshData();
      meshData.Vertices3D = [];

      const volume = MeshEntityThreejsRenderer.calculateVolume(meshData);

      expect(volume).toBe(0);
    });

    it('should return 0 for mesh with no faces', () => {
      const meshData = createBasicMeshData();
      meshData.FaceIndices = [];

      const volume = MeshEntityThreejsRenderer.calculateVolume(meshData);

      expect(volume).toBe(0);
    });
  });

  describe('calculateSurfaceArea', () => {
    it('should calculate surface area for triangle', () => {
      const meshData = createBasicMeshData();
      meshData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 1, Y: 0, Z: 0 },
        { X: 0, Y: 1, Z: 0 }
      ];
      meshData.FaceIndices = [[0, 1, 2]];

      const area = MeshEntityThreejsRenderer.calculateSurfaceArea(meshData);

      expect(area).toBe(0.5);
    });

    it('should return 0 for mesh with no vertices', () => {
      const meshData = createBasicMeshData();
      meshData.Vertices3D = [];

      const area = MeshEntityThreejsRenderer.calculateSurfaceArea(meshData);

      expect(area).toBe(0);
    });

    it('should return 0 for mesh with no faces', () => {
      const meshData = createBasicMeshData();
      meshData.FaceIndices = [];

      const area = MeshEntityThreejsRenderer.calculateSurfaceArea(meshData);

      expect(area).toBe(0);
    });
  });

  describe('isPointInside', () => {
    it('should return true for point inside mesh', () => {
      const meshData = createBasicMeshData();
      meshData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 0, Z: 0 },
        { X: 10, Y: 10, Z: 0 },
        { X: 0, Y: 10, Z: 0 },
        { X: 5, Y: 5, Z: 10 }
      ];
      meshData.FaceIndices = [
        [0, 1, 4],
        [1, 2, 4],
        [2, 3, 4],
        [3, 0, 4],
        [0, 1, 2],
        [2, 3, 0]
      ];
      meshData.Bounds3D = {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 10, Y: 10, Z: 10 },
        Center: { X: 5, Y: 5, Z: 5 },
        Size: { X: 10, Y: 10, Z: 10 }
      };

      const inside = MeshEntityThreejsRenderer.isPointInside(meshData, { X: 5, Y: 5, Z: 5 });

      expect(inside).toBe(true);
    });

    it('should return false for point outside bounds', () => {
      const meshData = createBasicMeshData();
      meshData.Bounds3D = {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 10, Y: 10, Z: 10 },
        Center: { X: 5, Y: 5, Z: 5 },
        Size: { X: 10, Y: 10, Z: 10 }
      };

      const inside = MeshEntityThreejsRenderer.isPointInside(meshData, { X: 20, Y: 20, Z: 20 });

      expect(inside).toBe(false);
    });

    it('should return false for undefined bounds', () => {
      const meshData = createBasicMeshData();
      meshData.Bounds3D = undefined as any;

      const inside = MeshEntityThreejsRenderer.isPointInside(meshData, { X: 5, Y: 5, Z: 5 });

      expect(inside).toBe(false);
    });
  });

  describe('getClosestPoint', () => {
    it('should return closest point on mesh', () => {
      const meshData = createBasicMeshData();
      meshData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 0, Z: 0 },
        { X: 5, Y: 10, Z: 0 }
      ];
      meshData.FaceIndices = [[0, 1, 2]];

      const closest = MeshEntityThreejsRenderer.getClosestPoint(meshData, { X: 5, Y: 5, Z: 10 });

      expect(closest).not.toBeNull();
      expect(closest?.Z).toBe(0);
    });

    it('should return null for mesh with no vertices', () => {
      const meshData = createBasicMeshData();
      meshData.Vertices3D = [];

      const closest = MeshEntityThreejsRenderer.getClosestPoint(meshData, { X: 5, Y: 5, Z: 5 });

      expect(closest).toBeNull();
    });
  });

  describe('getDistanceToPoint', () => {
    it('should return distance to point', () => {
      const meshData = createBasicMeshData();
      meshData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 0, Z: 0 },
        { X: 5, Y: 10, Z: 0 }
      ];
      meshData.FaceIndices = [[0, 1, 2]];

      const distance = MeshEntityThreejsRenderer.getDistanceToPoint(meshData, { X: 5, Y: 5, Z: 10 });

      expect(distance).toBe(10);
    });

    it('should return Infinity for mesh with no vertices', () => {
      const meshData = createBasicMeshData();
      meshData.Vertices3D = [];

      const distance = MeshEntityThreejsRenderer.getDistanceToPoint(meshData, { X: 5, Y: 5, Z: 5 });

      expect(distance).toBe(Infinity);
    });
  });

  describe('clone', () => {
    it('should clone mesh data with new handle', () => {
      const meshData = createBasicMeshData();

      const cloned = MeshEntityThreejsRenderer.clone(meshData, 'new-handle');

      expect(cloned.Handle).toBe('new-handle');
      expect(cloned.Type).toBe(meshData.Type);
      expect(cloned.EntityType).toBe(meshData.EntityType);
      expect(cloned.LayerName).toBe(meshData.LayerName);
      expect(cloned.VertexCount).toBe(meshData.VertexCount);
      expect(cloned.FaceCount).toBe(meshData.FaceCount);
      expect(cloned.EdgeCount).toBe(meshData.EdgeCount);
    });
  });

  describe('transform', () => {
    it('should transform mesh vertices', () => {
      const meshData = createBasicMeshData();
      meshData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 0, Z: 0 },
        { X: 5, Y: 10, Z: 0 }
      ];
      meshData.Centroid3D = { X: 5, Y: 3.3333333333333335, Z: 0 };
      meshData.Bounds3D = {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 10, Y: 10, Z: 0 },
        Center: { X: 5, Y: 5, Z: 0 },
        Size: { X: 10, Y: 10, Z: 0 }
      };

      const matrix = new THREE.Matrix4().makeTranslation(5, 5, 5);

      const transformed = MeshEntityThreejsRenderer.transform(meshData, matrix);

      expect(transformed.Vertices3D[0].X).toBe(5);
      expect(transformed.Vertices3D[0].Y).toBe(5);
      expect(transformed.Vertices3D[0].Z).toBe(5);
      expect(transformed.Centroid3D.X).toBe(10);
      expect(transformed.Centroid3D.Y).toBe(8.333333333333334);
      expect(transformed.Centroid3D.Z).toBe(5);
    });

    it('should transform normals', () => {
      const meshData = createBasicMeshData();
      meshData.Normals = [
        { X: 0, Y: 0, Z: 1 },
        { X: 0, Y: 0, Z: 1 },
        { X: 0, Y: 0, Z: 1 }
      ];

      const matrix = new THREE.Matrix4().makeRotationX(Math.PI / 2);

      const transformed = MeshEntityThreejsRenderer.transform(meshData, matrix);

      expect(transformed.Normals[0].X).toBeCloseTo(0);
      expect(transformed.Normals[0].Y).toBeCloseTo(-1);
      expect(transformed.Normals[0].Z).toBeCloseTo(0);
    });
  });

  describe('getSubdivisionLevel', () => {
    it('should return correct subdivision level', () => {
      const meshData = createBasicMeshData();
      meshData.SubdivisionLevel = 3;

      const level = MeshEntityThreejsRenderer.getSubdivisionLevel(meshData);

      expect(level).toBe(3);
    });

    it('should return 0 for undefined subdivision level', () => {
      const meshData = createBasicMeshData();
      meshData.SubdivisionLevel = undefined as any;

      const level = MeshEntityThreejsRenderer.getSubdivisionLevel(meshData);

      expect(level).toBe(0);
    });
  });

  describe('setSubdivisionLevel', () => {
    it('should set subdivision level and re-render', () => {
      const meshData = createBasicMeshData();
      MeshEntityThreejsRenderer.render(meshData, scene);

      const result = MeshEntityThreejsRenderer.setSubdivisionLevel(meshData, scene, 2);

      expect(result).toBe(true);
      expect(meshData.SubdivisionLevel).toBe(2);
      const group = MeshEntityThreejsRenderer.getMeshGroup(meshData, scene);
      expect(group).not.toBeNull();
    });

    it('should return false for non-existent mesh', () => {
      const meshData = createBasicMeshData();

      const result = MeshEntityThreejsRenderer.setSubdivisionLevel(meshData, scene, 2);

      expect(result).toBe(false);
    });
  });

  describe('getMaterialType', () => {
    it('should return correct material type', () => {
      const meshData = createBasicMeshData();
      meshData.MaterialType = 'MeshPhongMaterial';

      const type = MeshEntityThreejsRenderer.getMaterialType(meshData);

      expect(type).toBe('MeshPhongMaterial');
    });

    it('should return default material type for undefined type', () => {
      const meshData = createBasicMeshData();
      meshData.MaterialType = undefined as any;

      const type = MeshEntityThreejsRenderer.getMaterialType(meshData);

      expect(type).toBe('MeshStandardMaterial');
    });
  });

  describe('setMaterialType', () => {
    it('should set material type and re-render', () => {
      const meshData = createBasicMeshData();
      MeshEntityThreejsRenderer.render(meshData, scene);

      const result = MeshEntityThreejsRenderer.setMaterialType(meshData, scene, 'MeshPhongMaterial');

      expect(result).toBe(true);
      expect(meshData.MaterialType).toBe('MeshPhongMaterial');
      const group = MeshEntityThreejsRenderer.getMeshGroup(meshData, scene);
      expect(group).not.toBeNull();
    });

    it('should return false for non-existent mesh', () => {
      const meshData = createBasicMeshData();

      const result = MeshEntityThreejsRenderer.setMaterialType(meshData, scene, 'MeshPhongMaterial');

      expect(result).toBe(false);
    });
  });
});
