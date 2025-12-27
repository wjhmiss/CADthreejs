import * as THREE from 'three';
import { SolidEntityThreejsRenderer, SolidData, Point3DData, BoundsData, ColorData, TransformData, MaterialData, GeometryData } from '../SolidEntityThreejsRenderer';

describe('SolidEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    SolidEntityThreejsRenderer.clearCache();
  });

  afterEach(() => {
    SolidEntityThreejsRenderer.clearCache();
  });

  function createBasicSolidData(): SolidData {
    return {
      Points: [
        { X: 0, Y: 0, Z: 0 },
        { X: 1, Y: 0, Z: 0 },
        { X: 0, Y: 1, Z: 0 }
      ],
      ColorIndex: 1,
      LineTypeName: 'CONTINUOUS',
      LineWeight: 1.0,
      LineTypeScale: 1.0,
      Normal: { X: 0, Y: 0, Z: 1 },
      Thickness: 0.0,
      HasFourthCorner: false,
      Bounds: {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 1, Y: 1, Z: 0 },
        Center: { X: 0.5, Y: 0.5, Z: 0 },
        Size: { X: 1, Y: 1, Z: 0 }
      },
      Centroid: { X: 0.333, Y: 0.333, Z: 0 },
      Area: 0.5,
      Perimeter: 3.414,
      Transform: {
        Position: { X: 0, Y: 0, Z: 0 },
        Rotation: { X: 0, Y: 0, Z: 0 },
        Scale: { X: 1, Y: 1, Z: 1 },
        Matrix: [
          1, 0, 0, 0,
          0, 1, 0, 0,
          0, 0, 1, 0,
          0, 0, 0, 1
        ]
      },
      Geometry: {
        Type: 'BufferGeometry',
        VertexCount: 3,
        IndexCount: 3,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 1, Y: 1, Z: 0 },
          Center: { X: 0.5, Y: 0.5, Z: 0 },
          Size: { X: 1, Y: 1, Z: 0 }
        }
      },
      Material: {
        Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1 },
        Opacity: 1.0,
        Transparent: false,
        Type: 'MeshBasicMaterial',
        DepthTest: true,
        Side: false,
        Wireframe: false
      },
      Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1 },
      VertexPositions: [0, 0, 0, 1, 0, 0, 0, 1, 0],
      VertexNormals: [0, 0, 1, 0, 0, 1, 0, 0, 1],
      VertexColors: [],
      VertexUVs: [],
      Indices: [0, 1, 2],
      IsTriangle: true,
      FirstCorner: { X: 0, Y: 0, Z: 0 },
      SecondCorner: { X: 1, Y: 0, Z: 0 },
      ThirdCorner: { X: 0, Y: 1, Z: 0 },
      FourthCorner: { X: 0, Y: 1, Z: 0 },
      VertexCount: 3,
      FaceCount: 1,
      IsFilled: true,
      IsExtruded: false,
      ExtrusionDepth: 0,
      Type: 'Solid',
      Handle: 'solid-handle-1',
      LayerName: 'Layer0',
      LayerIndex: 0,
      Visible: true,
      CoordinateSystem: 'WCS',
      Opacity: 1.0,
      Transparent: false,
      DepthTest: true
    };
  }

  describe('render', () => {
    it('should render a basic Solid', () => {
      const solidData = createBasicSolidData();

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.name).toBe('Solid_solid-handle-1');
    });

    it('should not add invisible Solid to scene', () => {
      const solidData = createBasicSolidData();
      solidData.Visible = false;

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      expect(group).toBeNull();
      expect(scene.children.length).toBe(0);
    });

    it('should render Solid with correct Mesh object', () => {
      const solidData = createBasicSolidData();

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBeGreaterThan(0);
      const mesh = group.children[0] as THREE.Mesh;
      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.name).toBe('Solid');
    });

    it('should render Solid with correct material', () => {
      const solidData = createBasicSolidData();

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.Material;
      expect(material).toBeInstanceOf(THREE.Material);
      expect(material.type).toBe('MeshBasicMaterial');
    });

    it('should render Solid with correct geometry', () => {
      const solidData = createBasicSolidData();

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry;
      expect(geometry).toBeInstanceOf(THREE.BufferGeometry);
      expect(geometry.attributes.position.count).toBe(3);
    });

    it('should render Solid with edges', () => {
      const solidData = createBasicSolidData();

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const edges = group.children.find(child => child.name === 'Edges');
      expect(edges).toBeDefined();
      expect(edges).toBeInstanceOf(THREE.Line);
    });

    it('should render Solid with bounds', () => {
      const solidData = createBasicSolidData();

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const bounds = group.children.find(child => child.name === 'Bounds');
      expect(bounds).toBeDefined();
      expect(bounds).toBeInstanceOf(THREE.Line);
    });

    it('should apply transform matrix to group', () => {
      const solidData = createBasicSolidData();
      solidData.Transform.Matrix = [
        2, 0, 0, 0,
        0, 2, 0, 0,
        0, 0, 2, 0,
        1, 1, 1, 1
      ];

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const matrix = new THREE.Matrix4();
      matrix.fromArray(solidData.Transform.Matrix);
      expect(group.matrix.equals(matrix)).toBe(true);
    });

    it('should set correct user data on group', () => {
      const solidData = createBasicSolidData();

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      expect(group.userData.type).toBe('Solid');
      expect(group.userData.handle).toBe('solid-handle-1');
      expect(group.userData.layerName).toBe('Layer0');
      expect(group.userData.layerIndex).toBe(0);
      expect(group.userData.coordinateSystem).toBe('WCS');
    });

    it('should handle null solid data', () => {
      const group = SolidEntityThreejsRenderer.render(null as any, scene);

      expect(group).toBeNull();
      expect(scene.children.length).toBe(0);
    });

    it('should handle undefined solid data', () => {
      const group = SolidEntityThreejsRenderer.render(undefined as any, scene);

      expect(group).toBeNull();
      expect(scene.children.length).toBe(0);
    });

    it('should add group to scene', () => {
      const solidData = createBasicSolidData();

      SolidEntityThreejsRenderer.render(solidData, scene);

      expect(scene.children.length).toBeGreaterThan(0);
      expect(scene.children[0]).toBeInstanceOf(THREE.Group);
    });

    it('should cache rendered solid', () => {
      const solidData = createBasicSolidData();

      SolidEntityThreejsRenderer.render(solidData, scene);
      const cachedGroup = SolidEntityThreejsRenderer.getSolidGroup(solidData, scene);

      expect(cachedGroup).not.toBeNull();
      expect(cachedGroup).toBeInstanceOf(THREE.Group);
    });

    it('should handle solid with no vertex positions', () => {
      const solidData = createBasicSolidData();
      solidData.VertexPositions = [];

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const mesh = group.children.find(child => child.name === 'Solid');
      expect(mesh).toBeUndefined();
    });

    it('should render triangle solid correctly', () => {
      const solidData = createBasicSolidData();
      solidData.IsTriangle = true;

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const mesh = group.children[0] as THREE.Mesh;
      expect(mesh.userData.isTriangle).toBe(true);
    });

    it('should render quadrilateral solid correctly', () => {
      const solidData = createBasicSolidData();
      solidData.IsTriangle = false;
      solidData.HasFourthCorner = true;
      solidData.FourthCorner = { X: 1, Y: 1, Z: 0 };
      solidData.Points = [
        { X: 0, Y: 0, Z: 0 },
        { X: 1, Y: 0, Z: 0 },
        { X: 1, Y: 1, Z: 0 },
        { X: 0, Y: 1, Z: 0 }
      ];

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const mesh = group.children[0] as THREE.Mesh;
      expect(mesh.userData.isTriangle).toBe(false);
    });

    it('should render extruded solid correctly', () => {
      const solidData = createBasicSolidData();
      solidData.IsExtruded = true;
      solidData.ExtrusionDepth = 5.0;

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const mesh = group.children[0] as THREE.Mesh;
      expect(mesh.userData.isExtruded).toBe(true);
      expect(mesh.userData.extrusionDepth).toBe(5.0);
    });

    it('should handle solid with no points', () => {
      const solidData = createBasicSolidData();
      solidData.Points = [];

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const edges = group.children.find(child => child.name === 'Edges');
      expect(edges).toBeUndefined();
    });

    it('should handle solid with no bounds', () => {
      const solidData = createBasicSolidData();
      solidData.Bounds = undefined as any;

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const bounds = group.children.find(child => child.name === 'Bounds');
      expect(bounds).toBeUndefined();
    });

    it('should render solid with different color indices', () => {
      const solidData = createBasicSolidData();

      solidData.ColorIndex = 1;
      solidData.Color = { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1 };
      const redResult = SolidEntityThreejsRenderer.render(solidData, scene);
      const redMesh = redResult.children[0] as THREE.Mesh;
      expect((redMesh.material as THREE.MeshBasicMaterial).color.getHexString()).toBe('ff0000');

      SolidEntityThreejsRenderer.clearCache();
      scene.clear();

      solidData.ColorIndex = 3;
      solidData.Color = { Index: 3, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1 };
      const greenResult = SolidEntityThreejsRenderer.render(solidData, scene);
      const greenMesh = greenResult.children[0] as THREE.Mesh;
      expect((greenMesh.material as THREE.MeshBasicMaterial).color.getHexString()).toBe('00ff00');

      SolidEntityThreejsRenderer.clearCache();
      scene.clear();

      solidData.ColorIndex = 5;
      solidData.Color = { Index: 5, Hex: '#0000FF', R: 0, G: 0, B: 255, A: 1 };
      const blueResult = SolidEntityThreejsRenderer.render(solidData, scene);
      const blueMesh = blueResult.children[0] as THREE.Mesh;
      expect((blueMesh.material as THREE.MeshBasicMaterial).color.getHexString()).toBe('0000ff');
    });

    it('should render solid with different rotations', () => {
      const solidData = createBasicSolidData();

      solidData.Transform.Rotation.Z = Math.PI / 4;
      solidData.Transform.Matrix = [
        0.707, -0.707, 0, 0,
        0.707, 0.707, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1
      ];

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const matrix = new THREE.Matrix4();
      matrix.fromArray(solidData.Transform.Matrix);
      expect(group.matrix.equals(matrix)).toBe(true);
    });

    it('should render solid with different scales', () => {
      const solidData = createBasicSolidData();

      solidData.Transform.Scale.X = 2.0;
      solidData.Transform.Scale.Y = 3.0;
      solidData.Transform.Scale.Z = 1.0;
      solidData.Transform.Matrix = [
        2, 0, 0, 0,
        0, 3, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1
      ];

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const matrix = new THREE.Matrix4();
      matrix.fromArray(solidData.Transform.Matrix);
      expect(group.matrix.equals(matrix)).toBe(true);
    });

    it('should handle wireframe material', () => {
      const solidData = createBasicSolidData();
      solidData.Material.Wireframe = true;

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.wireframe).toBe(true);
    });

    it('should handle transparent material', () => {
      const solidData = createBasicSolidData();
      solidData.Material.Transparent = true;
      solidData.Material.Opacity = 0.5;

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.transparent).toBe(true);
      expect(material.opacity).toBe(0.5);
    });

    it('should handle double-sided material', () => {
      const solidData = createBasicSolidData();
      solidData.Material.Side = true;

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.side).toBe(THREE.DoubleSide);
    });

    it('should handle depth test disabled', () => {
      const solidData = createBasicSolidData();
      solidData.Material.DepthTest = false;

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.depthTest).toBe(false);
    });

    it('should compute vertex normals when not provided', () => {
      const solidData = createBasicSolidData();
      solidData.VertexNormals = [];

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry;
      expect(geometry.attributes.normal).toBeDefined();
      expect(geometry.attributes.normal.count).toBe(3);
    });

    it('should use provided vertex normals', () => {
      const solidData = createBasicSolidData();
      solidData.VertexNormals = [0, 0, 1, 0, 0, 1, 0, 0, 1];

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry;
      expect(geometry.attributes.normal).toBeDefined();
      expect(geometry.attributes.normal.count).toBe(3);
    });

    it('should handle vertex colors', () => {
      const solidData = createBasicSolidData();
      solidData.VertexColors = [1, 0, 0, 0, 1, 0, 0, 0, 1];

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry;
      expect(geometry.attributes.color).toBeDefined();
      expect(geometry.attributes.color.count).toBe(3);
    });

    it('should handle vertex UVs', () => {
      const solidData = createBasicSolidData();
      solidData.VertexUVs = [0, 0, 1, 0, 0, 1];

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry;
      expect(geometry.attributes.uv).toBeDefined();
      expect(geometry.attributes.uv.count).toBe(3);
    });

    it('should use provided indices', () => {
      const solidData = createBasicSolidData();
      solidData.Indices = [0, 1, 2];

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry;
      expect(geometry.index).toBeDefined();
      expect(geometry.index.count).toBe(3);
    });

    it('should compute bounding sphere and box', () => {
      const solidData = createBasicSolidData();

      const group = SolidEntityThreejsRenderer.render(solidData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry;
      expect(geometry.boundingSphere).toBeDefined();
      expect(geometry.boundingBox).toBeDefined();
    });
  });

  describe('dispose', () => {
    it('should dispose a rendered solid', () => {
      const solidData = createBasicSolidData();
      SolidEntityThreejsRenderer.render(solidData, scene);

      const result = SolidEntityThreejsRenderer.dispose(solidData, scene);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(0);
    });

    it('should return false for non-existent solid', () => {
      const solidData = createBasicSolidData();

      const result = SolidEntityThreejsRenderer.dispose(solidData, scene);

      expect(result).toBe(false);
    });

    it('should return false for null solid data', () => {
      const result = SolidEntityThreejsRenderer.dispose(null as any, scene);

      expect(result).toBe(false);
    });

    it('should return false for null scene', () => {
      const solidData = createBasicSolidData();

      const result = SolidEntityThreejsRenderer.dispose(solidData, null as any);

      expect(result).toBe(false);
    });

    it('should remove solid from cache', () => {
      const solidData = createBasicSolidData();
      SolidEntityThreejsRenderer.render(solidData, scene);

      SolidEntityThreejsRenderer.dispose(solidData, scene);
      const cachedGroup = SolidEntityThreejsRenderer.getSolidGroup(solidData, scene);

      expect(cachedGroup).toBeNull();
    });

    it('should dispose geometry and material', () => {
      const solidData = createBasicSolidData();
      const group = SolidEntityThreejsRenderer.render(solidData, scene);
      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry;
      const material = mesh.material;

      SolidEntityThreejsRenderer.dispose(solidData, scene);

      expect(geometry.isDisposed).toBe(true);
      expect(material.isDisposed).toBe(true);
    });
  });

  describe('update', () => {
    it('should update a rendered solid', () => {
      const solidData = createBasicSolidData();
      SolidEntityThreejsRenderer.render(solidData, scene);

      solidData.Visible = false;
      const result = SolidEntityThreejsRenderer.update(solidData, scene);

      expect(result).toBe(true);
      const group = SolidEntityThreejsRenderer.getSolidGroup(solidData, scene);
      expect(group.visible).toBe(false);
    });

    it('should return false for non-existent solid', () => {
      const solidData = createBasicSolidData();

      const result = SolidEntityThreejsRenderer.update(solidData, scene);

      expect(result).toBe(false);
    });

    it('should return false for null solid data', () => {
      const result = SolidEntityThreejsRenderer.update(null as any, scene);

      expect(result).toBe(false);
    });

    it('should return false for null scene', () => {
      const solidData = createBasicSolidData();

      const result = SolidEntityThreejsRenderer.update(solidData, null as any);

      expect(result).toBe(false);
    });

    it('should update opacity', () => {
      const solidData = createBasicSolidData();
      SolidEntityThreejsRenderer.render(solidData, scene);

      solidData.Opacity = 0.5;
      solidData.Transparent = true;
      SolidEntityThreejsRenderer.update(solidData, scene);

      const group = SolidEntityThreejsRenderer.getSolidGroup(solidData, scene);
      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });
  });

  describe('getSolidGroup', () => {
    it('should return cached solid group', () => {
      const solidData = createBasicSolidData();
      const renderedGroup = SolidEntityThreejsRenderer.render(solidData, scene);

      const cachedGroup = SolidEntityThreejsRenderer.getSolidGroup(solidData, scene);

      expect(cachedGroup).toBe(renderedGroup);
    });

    it('should return null for non-existent solid', () => {
      const solidData = createBasicSolidData();

      const cachedGroup = SolidEntityThreejsRenderer.getSolidGroup(solidData, scene);

      expect(cachedGroup).toBeNull();
    });
  });

  describe('setVisibility', () => {
    it('should set visibility of rendered solid', () => {
      const solidData = createBasicSolidData();
      SolidEntityThreejsRenderer.render(solidData, scene);

      const result = SolidEntityThreejsRenderer.setVisibility(solidData, scene, false);

      expect(result).toBe(true);
      const group = SolidEntityThreejsRenderer.getSolidGroup(solidData, scene);
      expect(group.visible).toBe(false);
    });

    it('should return false for non-existent solid', () => {
      const solidData = createBasicSolidData();

      const result = SolidEntityThreejsRenderer.setVisibility(solidData, scene, false);

      expect(result).toBe(false);
    });
  });

  describe('setOpacity', () => {
    it('should set opacity of rendered solid', () => {
      const solidData = createBasicSolidData();
      SolidEntityThreejsRenderer.render(solidData, scene);

      const result = SolidEntityThreejsRenderer.setOpacity(solidData, scene, 0.5);

      expect(result).toBe(true);
      const group = SolidEntityThreejsRenderer.getSolidGroup(solidData, scene);
      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should return false for non-existent solid', () => {
      const solidData = createBasicSolidData();

      const result = SolidEntityThreejsRenderer.setOpacity(solidData, scene, 0.5);

      expect(result).toBe(false);
    });
  });

  describe('getPoints', () => {
    it('should return points', () => {
      const solidData = createBasicSolidData();

      const points = SolidEntityThreejsRenderer.getPoints(solidData);

      expect(points).toEqual(solidData.Points);
    });

    it('should return empty array for null points', () => {
      const solidData = createBasicSolidData();
      solidData.Points = undefined as any;

      const points = SolidEntityThreejsRenderer.getPoints(solidData);

      expect(points).toEqual([]);
    });
  });

  describe('getThickness', () => {
    it('should return thickness', () => {
      const solidData = createBasicSolidData();
      solidData.Thickness = 5.0;

      const thickness = SolidEntityThreejsRenderer.getThickness(solidData);

      expect(thickness).toBe(5.0);
    });

    it('should return default thickness for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.Thickness = undefined as any;

      const thickness = SolidEntityThreejsRenderer.getThickness(solidData);

      expect(thickness).toBe(0.0);
    });
  });

  describe('getBounds', () => {
    it('should return bounds', () => {
      const solidData = createBasicSolidData();

      const bounds = SolidEntityThreejsRenderer.getBounds(solidData);

      expect(bounds).toEqual(solidData.Bounds);
    });

    it('should return default bounds for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.Bounds = undefined as any;

      const bounds = SolidEntityThreejsRenderer.getBounds(solidData);

      expect(bounds).toEqual({
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 0, Y: 0, Z: 0 },
        Center: { X: 0, Y: 0, Z: 0 },
        Size: { X: 0, Y: 0, Z: 0 }
      });
    });
  });

  describe('getCentroid', () => {
    it('should return centroid', () => {
      const solidData = createBasicSolidData();

      const centroid = SolidEntityThreejsRenderer.getCentroid(solidData);

      expect(centroid).toEqual(solidData.Centroid);
    });

    it('should return default centroid for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.Centroid = undefined as any;

      const centroid = SolidEntityThreejsRenderer.getCentroid(solidData);

      expect(centroid).toEqual({ X: 0, Y: 0, Z: 0 });
    });
  });

  describe('getArea', () => {
    it('should return area', () => {
      const solidData = createBasicSolidData();
      solidData.Area = 10.5;

      const area = SolidEntityThreejsRenderer.getArea(solidData);

      expect(area).toBe(10.5);
    });

    it('should return 0 for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.Area = undefined as any;

      const area = SolidEntityThreejsRenderer.getArea(solidData);

      expect(area).toBe(0);
    });
  });

  describe('getPerimeter', () => {
    it('should return perimeter', () => {
      const solidData = createBasicSolidData();
      solidData.Perimeter = 20.5;

      const perimeter = SolidEntityThreejsRenderer.getPerimeter(solidData);

      expect(perimeter).toBe(20.5);
    });

    it('should return 0 for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.Perimeter = undefined as any;

      const perimeter = SolidEntityThreejsRenderer.getPerimeter(solidData);

      expect(perimeter).toBe(0);
    });
  });

  describe('isTriangle', () => {
    it('should return true for triangle', () => {
      const solidData = createBasicSolidData();
      solidData.IsTriangle = true;

      const result = SolidEntityThreejsRenderer.isTriangle(solidData);

      expect(result).toBe(true);
    });

    it('should return false for quadrilateral', () => {
      const solidData = createBasicSolidData();
      solidData.IsTriangle = false;

      const result = SolidEntityThreejsRenderer.isTriangle(solidData);

      expect(result).toBe(false);
    });

    it('should return false for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.IsTriangle = undefined as any;

      const result = SolidEntityThreejsRenderer.isTriangle(solidData);

      expect(result).toBe(false);
    });
  });

  describe('isExtruded', () => {
    it('should return true for extruded solid', () => {
      const solidData = createBasicSolidData();
      solidData.IsExtruded = true;

      const result = SolidEntityThreejsRenderer.isExtruded(solidData);

      expect(result).toBe(true);
    });

    it('should return false for non-extruded solid', () => {
      const solidData = createBasicSolidData();
      solidData.IsExtruded = false;

      const result = SolidEntityThreejsRenderer.isExtruded(solidData);

      expect(result).toBe(false);
    });

    it('should return false for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.IsExtruded = undefined as any;

      const result = SolidEntityThreejsRenderer.isExtruded(solidData);

      expect(result).toBe(false);
    });
  });

  describe('getExtrusionDepth', () => {
    it('should return extrusion depth', () => {
      const solidData = createBasicSolidData();
      solidData.ExtrusionDepth = 10.0;

      const depth = SolidEntityThreejsRenderer.getExtrusionDepth(solidData);

      expect(depth).toBe(10.0);
    });

    it('should return 0 for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.ExtrusionDepth = undefined as any;

      const depth = SolidEntityThreejsRenderer.getExtrusionDepth(solidData);

      expect(depth).toBe(0);
    });
  });

  describe('getFirstCorner', () => {
    it('should return first corner', () => {
      const solidData = createBasicSolidData();

      const corner = SolidEntityThreejsRenderer.getFirstCorner(solidData);

      expect(corner).toEqual(solidData.FirstCorner);
    });

    it('should return default for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.FirstCorner = undefined as any;

      const corner = SolidEntityThreejsRenderer.getFirstCorner(solidData);

      expect(corner).toEqual({ X: 0, Y: 0, Z: 0 });
    });
  });

  describe('getSecondCorner', () => {
    it('should return second corner', () => {
      const solidData = createBasicSolidData();

      const corner = SolidEntityThreejsRenderer.getSecondCorner(solidData);

      expect(corner).toEqual(solidData.SecondCorner);
    });

    it('should return default for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.SecondCorner = undefined as any;

      const corner = SolidEntityThreejsRenderer.getSecondCorner(solidData);

      expect(corner).toEqual({ X: 0, Y: 0, Z: 0 });
    });
  });

  describe('getThirdCorner', () => {
    it('should return third corner', () => {
      const solidData = createBasicSolidData();

      const corner = SolidEntityThreejsRenderer.getThirdCorner(solidData);

      expect(corner).toEqual(solidData.ThirdCorner);
    });

    it('should return default for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.ThirdCorner = undefined as any;

      const corner = SolidEntityThreejsRenderer.getThirdCorner(solidData);

      expect(corner).toEqual({ X: 0, Y: 0, Z: 0 });
    });
  });

  describe('getFourthCorner', () => {
    it('should return fourth corner', () => {
      const solidData = createBasicSolidData();

      const corner = SolidEntityThreejsRenderer.getFourthCorner(solidData);

      expect(corner).toEqual(solidData.FourthCorner);
    });

    it('should return default for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.FourthCorner = undefined as any;

      const corner = SolidEntityThreejsRenderer.getFourthCorner(solidData);

      expect(corner).toEqual({ X: 0, Y: 0, Z: 0 });
    });
  });

  describe('getNormal', () => {
    it('should return normal', () => {
      const solidData = createBasicSolidData();

      const normal = SolidEntityThreejsRenderer.getNormal(solidData);

      expect(normal).toEqual(solidData.Normal);
    });

    it('should return default for zero normal', () => {
      const solidData = createBasicSolidData();
      solidData.Normal = { X: 0, Y: 0, Z: 0 };

      const normal = SolidEntityThreejsRenderer.getNormal(solidData);

      expect(normal).toEqual({ X: 0, Y: 0, Z: 1 });
    });

    it('should return default for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.Normal = undefined as any;

      const normal = SolidEntityThreejsRenderer.getNormal(solidData);

      expect(normal).toEqual({ X: 0, Y: 0, Z: 1 });
    });
  });

  describe('getColor', () => {
    it('should return color', () => {
      const solidData = createBasicSolidData();

      const color = SolidEntityThreejsRenderer.getColor(solidData);

      expect(color).toBeInstanceOf(THREE.Color);
      expect(color.getHexString()).toBe('ff0000');
    });

    it('should return default color for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.Color = undefined as any;

      const color = SolidEntityThreejsRenderer.getColor(solidData);

      expect(color).toBeInstanceOf(THREE.Color);
      expect(color.getHexString()).toBe('ffffff');
    });
  });

  describe('getHandle', () => {
    it('should return handle', () => {
      const solidData = createBasicSolidData();

      const handle = SolidEntityThreejsRenderer.getHandle(solidData);

      expect(handle).toBe('solid-handle-1');
    });

    it('should return empty string for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.Handle = undefined as any;

      const handle = SolidEntityThreejsRenderer.getHandle(solidData);

      expect(handle).toBe('');
    });
  });

  describe('getLayerName', () => {
    it('should return layer name', () => {
      const solidData = createBasicSolidData();

      const layerName = SolidEntityThreejsRenderer.getLayerName(solidData);

      expect(layerName).toBe('Layer0');
    });

    it('should return empty string for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.LayerName = undefined as any;

      const layerName = SolidEntityThreejsRenderer.getLayerName(solidData);

      expect(layerName).toBe('');
    });
  });

  describe('getVisible', () => {
    it('should return true for visible solid', () => {
      const solidData = createBasicSolidData();
      solidData.Visible = true;

      const visible = SolidEntityThreejsRenderer.getVisible(solidData);

      expect(visible).toBe(true);
    });

    it('should return false for invisible solid', () => {
      const solidData = createBasicSolidData();
      solidData.Visible = false;

      const visible = SolidEntityThreejsRenderer.getVisible(solidData);

      expect(visible).toBe(false);
    });

    it('should return true for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.Visible = undefined as any;

      const visible = SolidEntityThreejsRenderer.getVisible(solidData);

      expect(visible).toBe(true);
    });
  });

  describe('getOpacity', () => {
    it('should return opacity', () => {
      const solidData = createBasicSolidData();
      solidData.Opacity = 0.5;

      const opacity = SolidEntityThreejsRenderer.getOpacity(solidData);

      expect(opacity).toBe(0.5);
    });

    it('should return default for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.Opacity = undefined as any;

      const opacity = SolidEntityThreejsRenderer.getOpacity(solidData);

      expect(opacity).toBe(1.0);
    });
  });

  describe('getTransparent', () => {
    it('should return transparent', () => {
      const solidData = createBasicSolidData();
      solidData.Transparent = true;

      const transparent = SolidEntityThreejsRenderer.getTransparent(solidData);

      expect(transparent).toBe(true);
    });

    it('should return false for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.Transparent = undefined as any;

      const transparent = SolidEntityThreejsRenderer.getTransparent(solidData);

      expect(transparent).toBe(false);
    });
  });

  describe('getDepthTest', () => {
    it('should return depth test', () => {
      const solidData = createBasicSolidData();
      solidData.DepthTest = true;

      const depthTest = SolidEntityThreejsRenderer.getDepthTest(solidData);

      expect(depthTest).toBe(true);
    });

    it('should return true for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.DepthTest = undefined as any;

      const depthTest = SolidEntityThreejsRenderer.getDepthTest(solidData);

      expect(depthTest).toBe(true);
    });
  });

  describe('clearCache', () => {
    it('should clear cache', () => {
      const solidData = createBasicSolidData();
      SolidEntityThreejsRenderer.render(solidData, scene);

      SolidEntityThreejsRenderer.clearCache();
      const cachedGroup = SolidEntityThreejsRenderer.getSolidGroup(solidData, scene);

      expect(cachedGroup).toBeNull();
    });

    it('should dispose all cached solids', () => {
      const solidData1 = createBasicSolidData();
      solidData1.Handle = 'solid-1';
      const solidData2 = createBasicSolidData();
      solidData2.Handle = 'solid-2';

      SolidEntityThreejsRenderer.render(solidData1, scene);
      SolidEntityThreejsRenderer.render(solidData2, scene);

      SolidEntityThreejsRenderer.clearCache();

      expect(SolidEntityThreejsRenderer.getSolidGroup(solidData1, scene)).toBeNull();
      expect(SolidEntityThreejsRenderer.getSolidGroup(solidData2, scene)).toBeNull();
    });
  });

  describe('renderMultiple', () => {
    it('should render multiple solids', () => {
      const solidData1 = createBasicSolidData();
      solidData1.Handle = 'solid-1';
      const solidData2 = createBasicSolidData();
      solidData2.Handle = 'solid-2';

      const group = SolidEntityThreejsRenderer.renderMultiple([solidData1, solidData2], scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('MultipleSolids');
      expect(group.children.length).toBe(2);
    });

    it('should skip invisible solids', () => {
      const solidData1 = createBasicSolidData();
      solidData1.Handle = 'solid-1';
      const solidData2 = createBasicSolidData();
      solidData2.Handle = 'solid-2';
      solidData2.Visible = false;

      const group = SolidEntityThreejsRenderer.renderMultiple([solidData1, solidData2], scene);

      expect(group.children.length).toBe(1);
    });

    it('should handle empty array', () => {
      const group = SolidEntityThreejsRenderer.renderMultiple([], scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('MultipleSolids');
      expect(group.children.length).toBe(0);
    });
  });

  describe('disposeMultiple', () => {
    it('should dispose multiple solids', () => {
      const solidData1 = createBasicSolidData();
      solidData1.Handle = 'solid-1';
      const solidData2 = createBasicSolidData();
      solidData2.Handle = 'solid-2';

      const group = SolidEntityThreejsRenderer.renderMultiple([solidData1, solidData2], scene);

      SolidEntityThreejsRenderer.disposeMultiple(group, scene);

      expect(scene.children.length).toBe(0);
    });

    it('should handle null group', () => {
      expect(() => {
        SolidEntityThreejsRenderer.disposeMultiple(null as any, scene);
      }).not.toThrow();
    });
  });

  describe('getTransform', () => {
    it('should return transform', () => {
      const solidData = createBasicSolidData();

      const transform = SolidEntityThreejsRenderer.getTransform(solidData);

      expect(transform.ScaleX).toBe(1.0);
      expect(transform.ScaleY).toBe(1.0);
      expect(transform.ScaleZ).toBe(1.0);
      expect(transform.Rotation).toBe(0);
      expect(transform.TranslateX).toBe(solidData.Centroid.X);
      expect(transform.TranslateY).toBe(solidData.Centroid.Y);
      expect(transform.TranslateZ).toBe(solidData.Centroid.Z);
    });

    it('should return transform from Transform data', () => {
      const solidData = createBasicSolidData();
      solidData.Transform.Scale.X = 2.0;
      solidData.Transform.Scale.Y = 3.0;
      solidData.Transform.Scale.Z = 1.0;
      solidData.Transform.Rotation.Z = Math.PI / 4;
      solidData.Transform.Position.X = 1.0;
      solidData.Transform.Position.Y = 2.0;
      solidData.Transform.Position.Z = 3.0;

      const transform = SolidEntityThreejsRenderer.getTransform(solidData);

      expect(transform.ScaleX).toBe(2.0);
      expect(transform.ScaleY).toBe(3.0);
      expect(transform.ScaleZ).toBe(1.0);
      expect(transform.Rotation).toBe(Math.PI / 4);
      expect(transform.TranslateX).toBe(1.0);
      expect(transform.TranslateY).toBe(2.0);
      expect(transform.TranslateZ).toBe(3.0);
    });
  });

  describe('getMaterial', () => {
    it('should return material', () => {
      const solidData = createBasicSolidData();

      const material = SolidEntityThreejsRenderer.getMaterial(solidData);

      expect(material).toEqual(solidData.Material);
    });

    it('should return default material for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.Material = undefined as any;

      const material = SolidEntityThreejsRenderer.getMaterial(solidData);

      expect(material.Type).toBe('MeshBasicMaterial');
      expect(material.Color).toEqual(solidData.Color);
      expect(material.Opacity).toBe(1.0);
      expect(material.Transparent).toBe(false);
      expect(material.DepthTest).toBe(true);
      expect(material.Side).toBe(false);
      expect(material.Wireframe).toBe(false);
    });
  });

  describe('getGeometry', () => {
    it('should return geometry', () => {
      const solidData = createBasicSolidData();

      const geometry = SolidEntityThreejsRenderer.getGeometry(solidData);

      expect(geometry).toEqual(solidData.Geometry);
    });

    it('should return default geometry for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.Geometry = undefined as any;

      const geometry = SolidEntityThreejsRenderer.getGeometry(solidData);

      expect(geometry.Type).toBe('BufferGeometry');
      expect(geometry.VertexCount).toBe(3);
      expect(geometry.IndexCount).toBe(3);
      expect(geometry.Bounds).toEqual(solidData.Bounds);
    });
  });

  describe('getVertexPositions', () => {
    it('should return vertex positions', () => {
      const solidData = createBasicSolidData();

      const positions = SolidEntityThreejsRenderer.getVertexPositions(solidData);

      expect(positions).toEqual(solidData.VertexPositions);
    });

    it('should return empty array for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.VertexPositions = undefined as any;

      const positions = SolidEntityThreejsRenderer.getVertexPositions(solidData);

      expect(positions).toEqual([]);
    });
  });

  describe('getVertexNormals', () => {
    it('should return vertex normals', () => {
      const solidData = createBasicSolidData();

      const normals = SolidEntityThreejsRenderer.getVertexNormals(solidData);

      expect(normals).toEqual(solidData.VertexNormals);
    });

    it('should return empty array for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.VertexNormals = undefined as any;

      const normals = SolidEntityThreejsRenderer.getVertexNormals(solidData);

      expect(normals).toEqual([]);
    });
  });

  describe('getVertexColors', () => {
    it('should return vertex colors', () => {
      const solidData = createBasicSolidData();

      const colors = SolidEntityThreejsRenderer.getVertexColors(solidData);

      expect(colors).toEqual(solidData.VertexColors);
    });

    it('should return empty array for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.VertexColors = undefined as any;

      const colors = SolidEntityThreejsRenderer.getVertexColors(solidData);

      expect(colors).toEqual([]);
    });
  });

  describe('getVertexUVs', () => {
    it('should return vertex UVs', () => {
      const solidData = createBasicSolidData();

      const uvs = SolidEntityThreejsRenderer.getVertexUVs(solidData);

      expect(uvs).toEqual(solidData.VertexUVs);
    });

    it('should return empty array for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.VertexUVs = undefined as any;

      const uvs = SolidEntityThreejsRenderer.getVertexUVs(solidData);

      expect(uvs).toEqual([]);
    });
  });

  describe('getIndices', () => {
    it('should return indices', () => {
      const solidData = createBasicSolidData();

      const indices = SolidEntityThreejsRenderer.getIndices(solidData);

      expect(indices).toEqual(solidData.Indices);
    });

    it('should return empty array for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.Indices = undefined as any;

      const indices = SolidEntityThreejsRenderer.getIndices(solidData);

      expect(indices).toEqual([]);
    });
  });

  describe('getVertexCount', () => {
    it('should return vertex count', () => {
      const solidData = createBasicSolidData();
      solidData.VertexCount = 10;

      const count = SolidEntityThreejsRenderer.getVertexCount(solidData);

      expect(count).toBe(10);
    });

    it('should return 0 for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.VertexCount = undefined as any;

      const count = SolidEntityThreejsRenderer.getVertexCount(solidData);

      expect(count).toBe(0);
    });
  });

  describe('getFaceCount', () => {
    it('should return face count', () => {
      const solidData = createBasicSolidData();
      solidData.FaceCount = 5;

      const count = SolidEntityThreejsRenderer.getFaceCount(solidData);

      expect(count).toBe(5);
    });

    it('should return 0 for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.FaceCount = undefined as any;

      const count = SolidEntityThreejsRenderer.getFaceCount(solidData);

      expect(count).toBe(0);
    });
  });

  describe('isFilled', () => {
    it('should return true for filled solid', () => {
      const solidData = createBasicSolidData();
      solidData.IsFilled = true;

      const result = SolidEntityThreejsRenderer.isFilled(solidData);

      expect(result).toBe(true);
    });

    it('should return false for unfilled solid', () => {
      const solidData = createBasicSolidData();
      solidData.IsFilled = false;

      const result = SolidEntityThreejsRenderer.isFilled(solidData);

      expect(result).toBe(false);
    });

    it('should return true for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.IsFilled = undefined as any;

      const result = SolidEntityThreejsRenderer.isFilled(solidData);

      expect(result).toBe(true);
    });
  });

  describe('hasFourthCorner', () => {
    it('should return true when has fourth corner', () => {
      const solidData = createBasicSolidData();
      solidData.HasFourthCorner = true;

      const result = SolidEntityThreejsRenderer.hasFourthCorner(solidData);

      expect(result).toBe(true);
    });

    it('should return false when no fourth corner', () => {
      const solidData = createBasicSolidData();
      solidData.HasFourthCorner = false;

      const result = SolidEntityThreejsRenderer.hasFourthCorner(solidData);

      expect(result).toBe(false);
    });

    it('should return false for undefined', () => {
      const solidData = createBasicSolidData();
      solidData.HasFourthCorner = undefined as any;

      const result = SolidEntityThreejsRenderer.hasFourthCorner(solidData);

      expect(result).toBe(false);
    });
  });
});
