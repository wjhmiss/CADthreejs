import * as THREE from 'three';
import { Polyline3DEntityThreejsRenderer, Polyline3DData, VertexData, Point3DData, ColorData, TransformData, NormalData, BoundsData, MaterialData, GeometryData, SegmentData } from '../Polyline3DEntityThreejsRenderer';

describe('Polyline3DEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    Polyline3DEntityThreejsRenderer.clearCache();
  });

  const createBasicPolyline3DData = (): Polyline3DData => ({
    Vertices: [
      { Location: { X: 0, Y: 0, Z: 0 } },
      { Location: { X: 10, Y: 0, Z: 0 } },
      { Location: { X: 10, Y: 10, Z: 0 } },
      { Location: { X: 0, Y: 10, Z: 0 } }
    ],
    IsClosed: true,
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 1.0,
    Elevation: 0,
    Thickness: 0,
    VertexCount: 4,
    Bounds: {
      MinX: 0,
      MaxX: 10,
      MinY: 0,
      MaxY: 10,
      MinZ: 0,
      MaxZ: 0
    },
    Centroid: { X: 5, Y: 5, Z: 0 },
    TotalLength: 40,
    VertexPositions: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
    VertexNormals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
    VertexUVs: [],
    Indices: [],
    Transform: {
      Position: { X: 0, Y: 0, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
    },
    Material: {
      Type: 'LineBasicMaterial',
      Color: 0xff0000,
      Opacity: 1.0,
      Transparent: false,
      Wireframe: false,
      LineWidth: 1.0,
      Side: true,
      VertexColors: false
    },
    Geometry: {
      Type: 'BufferGeometry',
      VertexCount: 4,
      FaceCount: 0,
      HasNormals: true,
      HasUVs: false,
      HasIndices: false,
      PrimitiveType: 'Line',
      IndexCount: 0,
      IsClosed: true,
      IsPeriodic: false,
      Degree: 1
    },
    HasSegments: true,
    Segments: [
      {
        Start: { X: 0, Y: 0, Z: 0 },
        End: { X: 10, Y: 0, Z: 0 },
        Length: 10,
        Direction: { X: 1, Y: 0, Z: 0 }
      },
      {
        Start: { X: 10, Y: 0, Z: 0 },
        End: { X: 10, Y: 10, Z: 0 },
        Length: 10,
        Direction: { X: 0, Y: 1, Z: 0 }
      },
      {
        Start: { X: 10, Y: 10, Z: 0 },
        End: { X: 0, Y: 10, Z: 0 },
        Length: 10,
        Direction: { X: -1, Y: 0, Z: 0 }
      },
      {
        Start: { X: 0, Y: 10, Z: 0 },
        End: { X: 0, Y: 0, Z: 0 },
        Length: 10,
        Direction: { X: 0, Y: -1, Z: 0 }
      }
    ],
    EntityType: 'Polyline3D',
    Visible: true,
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Handle: 'polyline3d-handle-1',
    Transparency: 0,
    MaterialName: 'Default',
    CastShadows: false,
    ReceiveShadows: false,
    Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 },
    Normal: { X: 0, Y: 0, Z: 1 }
  });

  describe('render', () => {
    it('should render a basic Polyline3D', () => {
      const polyline3DData = createBasicPolyline3DData();

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.name).toBe('Polyline3D_polyline3d-handle-1');
    });

    it('should not add invisible Polyline3D to scene', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.Visible = false;

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.visible).toBe(false);
      expect(scene.children.length).toBe(0);
    });

    it('should render Polyline3D with correct Line object', () => {
      const polyline3DData = createBasicPolyline3DData();

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      expect(group.children.length).toBe(1);
      const line = group.children[0] as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
      expect(line.name).toBe('Polyline3D');
    });

    it('should render Polyline3D with correct material', () => {
      const polyline3DData = createBasicPolyline3DData();

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);
      const line = group.children[0] as THREE.Line;

      expect(line.material).toBeDefined();
      expect(line.material).toBeInstanceOf(THREE.LineBasicMaterial);
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('ff0000');
      expect(material.linewidth).toBe(1.0);
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(true);
    });

    it('should render Polyline3D with correct geometry', () => {
      const polyline3DData = createBasicPolyline3DData();

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);
      const line = group.children[0] as THREE.Line;

      expect(line.geometry).toBeDefined();
      expect(line.geometry).toBeInstanceOf(THREE.BufferGeometry);
      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position).toBeDefined();
      expect(geometry.attributes.position.count).toBe(4);
    });

    it('should render Polyline3D with correct vertices', () => {
      const polyline3DData = createBasicPolyline3DData();

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(0);
      expect(positions[1]).toBe(0);
      expect(positions[2]).toBe(0);
      expect(positions[3]).toBe(10);
      expect(positions[4]).toBe(0);
      expect(positions[5]).toBe(0);
      expect(positions[6]).toBe(10);
      expect(positions[7]).toBe(10);
      expect(positions[8]).toBe(0);
      expect(positions[9]).toBe(0);
      expect(positions[10]).toBe(10);
      expect(positions[11]).toBe(0);
    });

    it('should render Polyline3D with correct color', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.Material.Color = 0x00ff00;

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should render Polyline3D with correct line weight', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.Material.LineWidth = 2.5;

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.linewidth).toBe(2.5);
    });

    it('should render Polyline3D with correct transparency', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.Material.Opacity = 0.5;
      polyline3DData.Material.Transparent = true;

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
      expect(material.depthWrite).toBe(false);
    });

    it('should render Polyline3D with normals', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.VertexNormals = [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1];

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.normal).toBeDefined();
      expect(geometry.attributes.normal.count).toBe(4);
    });

    it('should render Polyline3D with UVs', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.VertexUVs = [0, 0, 1, 0, 1, 1, 0, 1];

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.uv).toBeDefined();
      expect(geometry.attributes.uv.count).toBe(4);
    });

    it('should render Polyline3D with indices', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.Indices = [0, 1, 2, 3];

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.index).toBeDefined();
      expect(geometry.index.count).toBe(4);
    });

    it('should cache Polyline3D by handle', () => {
      const polyline3DData = createBasicPolyline3DData();

      Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      const cachedGroup = (Polyline3DEntityThreejsRenderer as any).polylineCache.get('polyline3d-handle-1');
      expect(cachedGroup).toBeDefined();
      expect(cachedGroup).toBeInstanceOf(THREE.Group);
    });

    it('should render open Polyline3D', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.IsClosed = false;
      polyline3DData.Vertices = [
        { Location: { X: 0, Y: 0, Z: 0 } },
        { Location: { X: 10, Y: 0, Z: 0 } },
        { Location: { X: 10, Y: 10, Z: 0 } }
      ];
      polyline3DData.VertexPositions = [0, 0, 0, 10, 0, 0, 10, 10, 0];
      polyline3DData.VertexCount = 3;
      polyline3DData.Segments = [
        {
          Start: { X: 0, Y: 0, Z: 0 },
          End: { X: 10, Y: 0, Z: 0 },
          Length: 10,
          Direction: { X: 1, Y: 0, Z: 0 }
        },
        {
          Start: { X: 10, Y: 0, Z: 0 },
          End: { X: 10, Y: 10, Z: 0 },
          Length: 10,
          Direction: { X: 0, Y: 1, Z: 0 }
        }
      ];

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position.count).toBe(3);
    });

    it('should handle Polyline3D with no vertices', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.Vertices = [];
      polyline3DData.VertexPositions = [];
      polyline3DData.VertexCount = 0;
      polyline3DData.Segments = [];

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBe(0);
    });

    it('should render Polyline3D with correct handle in userData', () => {
      const polyline3DData = createBasicPolyline3DData();

      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);
      const line = group.children[0] as THREE.Line;

      expect(line.userData.handle).toBe('polyline3d-handle-1');
    });
  });

  describe('update', () => {
    it('should update existing Polyline3D', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      const updatedPolyline3DData = { ...polyline3DData };
      updatedPolyline3DData.VertexPositions = [5, 5, 0, 15, 5, 0, 15, 15, 0, 5, 15, 0];

      const result = Polyline3DEntityThreejsRenderer.update(updatedPolyline3DData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent Polyline3D', () => {
      const polyline3DData = createBasicPolyline3DData();

      const result = Polyline3DEntityThreejsRenderer.update(polyline3DData, scene);

      expect(result).toBe(false);
    });

    it('should update Polyline3D color', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      const updatedPolyline3DData = { ...polyline3DData };
      updatedPolyline3DData.Material.Color = 0x00ff00;

      const result = Polyline3DEntityThreejsRenderer.update(updatedPolyline3DData, scene);

      expect(result).toBe(true);
    });

    it('should update Polyline3D opacity', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      const updatedPolyline3DData = { ...polyline3DData };
      updatedPolyline3DData.Material.Opacity = 0.7;
      updatedPolyline3DData.Material.Transparent = true;

      const result = Polyline3DEntityThreejsRenderer.update(updatedPolyline3DData, scene);

      expect(result).toBe(true);
    });

    it('should update Polyline3D line weight', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      const updatedPolyline3DData = { ...polyline3DData };
      updatedPolyline3DData.Material.LineWidth = 3.0;

      const result = Polyline3DEntityThreejsRenderer.update(updatedPolyline3DData, scene);

      expect(result).toBe(true);
    });

    it('should update Polyline3D vertices', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      const updatedPolyline3DData = { ...polyline3DData };
      updatedPolyline3DData.VertexPositions = [100, 100, 0, 200, 100, 0, 200, 200, 0, 100, 200, 0];

      const result = Polyline3DEntityThreejsRenderer.update(updatedPolyline3DData, scene);

      expect(result).toBe(true);
    });

    it('should update Polyline3D visibility', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      const updatedPolyline3DData = { ...polyline3DData };
      updatedPolyline3DData.Visible = false;

      const result = Polyline3DEntityThreejsRenderer.update(updatedPolyline3DData, scene);

      expect(result).toBe(true);
    });
  });

  describe('dispose', () => {
    it('should dispose Polyline3D resources', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      Polyline3DEntityThreejsRenderer.dispose(polyline3DData, scene);

      expect(scene.children.length).toBe(0);
    });

    it('should handle non-existent Polyline3D gracefully', () => {
      const polyline3DData = createBasicPolyline3DData();

      expect(() => {
        Polyline3DEntityThreejsRenderer.dispose(polyline3DData, scene);
      }).not.toThrow();
    });

    it('should remove Polyline3D from cache', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      Polyline3DEntityThreejsRenderer.dispose(polyline3DData, scene);

      const cachedGroup = (Polyline3DEntityThreejsRenderer as any).polylineCache.get('polyline3d-handle-1');
      expect(cachedGroup).toBeUndefined();
    });

    it('should dispose Polyline3D geometry', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry;
      const originalDispose = geometry.dispose.bind(geometry);
      let disposeCalled = false;
      geometry.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      Polyline3DEntityThreejsRenderer.dispose(polyline3DData, scene);

      expect(disposeCalled).toBe(true);
    });

    it('should dispose Polyline3D material', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material;
      const originalDispose = material.dispose.bind(material);
      let disposeCalled = false;
      material.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      Polyline3DEntityThreejsRenderer.dispose(polyline3DData, scene);

      expect(disposeCalled).toBe(true);
    });
  });

  describe('setVisibility', () => {
    it('should set Polyline3D visibility to true', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      Polyline3DEntityThreejsRenderer.setVisibility(polyline3DData, true);

      expect(group.visible).toBe(true);
    });

    it('should set Polyline3D visibility to false', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      Polyline3DEntityThreejsRenderer.setVisibility(polyline3DData, false);

      expect(group.visible).toBe(false);
    });

    it('should handle non-existent Polyline3D gracefully', () => {
      const polyline3DData = createBasicPolyline3DData();

      expect(() => {
        Polyline3DEntityThreejsRenderer.setVisibility(polyline3DData, true);
      }).not.toThrow();
    });
  });

  describe('setOpacity', () => {
    it('should set Polyline3D opacity', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      Polyline3DEntityThreejsRenderer.setOpacity(polyline3DData, 0.5);

      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should handle non-existent Polyline3D gracefully', () => {
      const polyline3DData = createBasicPolyline3DData();

      expect(() => {
        Polyline3DEntityThreejsRenderer.setOpacity(polyline3DData, 0.5);
      }).not.toThrow();
    });
  });

  describe('setColor', () => {
    it('should set Polyline3D color', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      Polyline3DEntityThreejsRenderer.setColor(polyline3DData, 0x00ff00);

      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should handle non-existent Polyline3D gracefully', () => {
      const polyline3DData = createBasicPolyline3DData();

      expect(() => {
        Polyline3DEntityThreejsRenderer.setColor(polyline3DData, 0x00ff00);
      }).not.toThrow();
    });
  });

  describe('getGroup', () => {
    it('should return correct group', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      const retrievedGroup = Polyline3DEntityThreejsRenderer.getGroup(polyline3DData);

      expect(retrievedGroup).toBe(group);
    });

    it('should return null for non-existent Polyline3D', () => {
      const polyline3DData = createBasicPolyline3DData();

      const retrievedGroup = Polyline3DEntityThreejsRenderer.getGroup(polyline3DData);

      expect(retrievedGroup).toBeNull();
    });
  });

  describe('getLine', () => {
    it('should return correct line', () => {
      const polyline3DData = createBasicPolyline3DData();
      const group = Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);
      const line = group.children[0] as THREE.Line;

      const retrievedLine = Polyline3DEntityThreejsRenderer.getLine(polyline3DData);

      expect(retrievedLine).toBe(line);
    });

    it('should return null for non-existent Polyline3D', () => {
      const polyline3DData = createBasicPolyline3DData();

      const retrievedLine = Polyline3DEntityThreejsRenderer.getLine(polyline3DData);

      expect(retrievedLine).toBeNull();
    });
  });

  describe('getVertices', () => {
    it('should return correct vertices', () => {
      const polyline3DData = createBasicPolyline3DData();

      const vertices = Polyline3DEntityThreejsRenderer.getVertices(polyline3DData);

      expect(vertices).toEqual(polyline3DData.Vertices);
    });
  });

  describe('getVertexPositions', () => {
    it('should return correct vertex positions', () => {
      const polyline3DData = createBasicPolyline3DData();

      const vertexPositions = Polyline3DEntityThreejsRenderer.getVertexPositions(polyline3DData);

      expect(vertexPositions).toEqual(polyline3DData.VertexPositions);
    });
  });

  describe('getVertexNormals', () => {
    it('should return correct vertex normals', () => {
      const polyline3DData = createBasicPolyline3DData();

      const vertexNormals = Polyline3DEntityThreejsRenderer.getVertexNormals(polyline3DData);

      expect(vertexNormals).toEqual(polyline3DData.VertexNormals);
    });
  });

  describe('getVertexUVs', () => {
    it('should return correct vertex UVs', () => {
      const polyline3DData = createBasicPolyline3DData();

      const vertexUVs = Polyline3DEntityThreejsRenderer.getVertexUVs(polyline3DData);

      expect(vertexUVs).toEqual(polyline3DData.VertexUVs);
    });
  });

  describe('getIndices', () => {
    it('should return correct indices', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.Indices = [0, 1, 2, 3];

      const indices = Polyline3DEntityThreejsRenderer.getIndices(polyline3DData);

      expect(indices).toEqual(polyline3DData.Indices);
    });
  });

  describe('getSegments', () => {
    it('should return correct segments', () => {
      const polyline3DData = createBasicPolyline3DData();

      const segments = Polyline3DEntityThreejsRenderer.getSegments(polyline3DData);

      expect(segments).toEqual(polyline3DData.Segments);
    });
  });

  describe('getSegment', () => {
    it('should return correct segment', () => {
      const polyline3DData = createBasicPolyline3DData();

      const segment = Polyline3DEntityThreejsRenderer.getSegment(polyline3DData, 0);

      expect(segment).toEqual(polyline3DData.Segments[0]);
    });

    it('should return null for invalid index', () => {
      const polyline3DData = createBasicPolyline3DData();

      const segment = Polyline3DEntityThreejsRenderer.getSegment(polyline3DData, 10);

      expect(segment).toBeNull();
    });
  });

  describe('getSegmentLength', () => {
    it('should return correct segment length', () => {
      const polyline3DData = createBasicPolyline3DData();

      const segmentLength = Polyline3DEntityThreejsRenderer.getSegmentLength(polyline3DData, 0);

      expect(segmentLength).toBe(10);
    });

    it('should return 0 for invalid index', () => {
      const polyline3DData = createBasicPolyline3DData();

      const segmentLength = Polyline3DEntityThreejsRenderer.getSegmentLength(polyline3DData, 10);

      expect(segmentLength).toBe(0);
    });
  });

  describe('getSegmentDirection', () => {
    it('should return correct segment direction', () => {
      const polyline3DData = createBasicPolyline3DData();

      const segmentDirection = Polyline3DEntityThreejsRenderer.getSegmentDirection(polyline3DData, 0);

      expect(segmentDirection).toEqual(polyline3DData.Segments[0].Direction);
    });

    it('should return null for invalid index', () => {
      const polyline3DData = createBasicPolyline3DData();

      const segmentDirection = Polyline3DEntityThreejsRenderer.getSegmentDirection(polyline3DData, 10);

      expect(segmentDirection).toBeNull();
    });
  });

  describe('getColor', () => {
    it('should return correct color', () => {
      const polyline3DData = createBasicPolyline3DData();

      const color = Polyline3DEntityThreejsRenderer.getColor(polyline3DData);

      expect(color).toEqual(polyline3DData.Color);
    });
  });

  describe('getBounds', () => {
    it('should return correct bounds', () => {
      const polyline3DData = createBasicPolyline3DData();

      const bounds = Polyline3DEntityThreejsRenderer.getBounds(polyline3DData);

      expect(bounds).toEqual(polyline3DData.Bounds);
    });
  });

  describe('getCentroid', () => {
    it('should return correct centroid', () => {
      const polyline3DData = createBasicPolyline3DData();

      const centroid = Polyline3DEntityThreejsRenderer.getCentroid(polyline3DData);

      expect(centroid).toEqual(polyline3DData.Centroid);
    });
  });

  describe('getHandle', () => {
    it('should return correct handle', () => {
      const polyline3DData = createBasicPolyline3DData();

      const handle = Polyline3DEntityThreejsRenderer.getHandle(polyline3DData);

      expect(handle).toBe(polyline3DData.Handle);
    });
  });

  describe('getLayerName', () => {
    it('should return correct layer name', () => {
      const polyline3DData = createBasicPolyline3DData();

      const layerName = Polyline3DEntityThreejsRenderer.getLayerName(polyline3DData);

      expect(layerName).toBe(polyline3DData.LayerName);
    });
  });

  describe('getVisible', () => {
    it('should return correct visibility', () => {
      const polyline3DData = createBasicPolyline3DData();

      const visible = Polyline3DEntityThreejsRenderer.getVisible(polyline3DData);

      expect(visible).toBe(polyline3DData.Visible);
    });
  });

  describe('getOpacity', () => {
    it('should return correct opacity', () => {
      const polyline3DData = createBasicPolyline3DData();

      const opacity = Polyline3DEntityThreejsRenderer.getOpacity(polyline3DData);

      expect(opacity).toBe(polyline3DData.Material.Opacity);
    });
  });

  describe('getTransparent', () => {
    it('should return correct transparency status', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.Material.Transparent = true;

      const transparent = Polyline3DEntityThreejsRenderer.getTransparent(polyline3DData);

      expect(transparent).toBe(true);
    });
  });

  describe('getCastShadows', () => {
    it('should return correct cast shadows status', () => {
      const polyline3DData = createBasicPolyline3DData();

      const castShadows = Polyline3DEntityThreejsRenderer.getCastShadows(polyline3DData);

      expect(castShadows).toBe(polyline3DData.CastShadows);
    });
  });

  describe('getReceiveShadows', () => {
    it('should return correct receive shadows status', () => {
      const polyline3DData = createBasicPolyline3DData();

      const receiveShadows = Polyline3DEntityThreejsRenderer.getReceiveShadows(polyline3DData);

      expect(receiveShadows).toBe(polyline3DData.ReceiveShadows);
    });
  });

  describe('getTransform', () => {
    it('should return correct transform', () => {
      const polyline3DData = createBasicPolyline3DData();

      const transform = Polyline3DEntityThreejsRenderer.getTransform(polyline3DData);

      expect(transform).toEqual(polyline3DData.Transform);
    });
  });

  describe('getGeometry', () => {
    it('should return correct geometry', () => {
      const polyline3DData = createBasicPolyline3DData();

      const geometry = Polyline3DEntityThreejsRenderer.getGeometry(polyline3DData);

      expect(geometry).toEqual(polyline3DData.Geometry);
    });
  });

  describe('getMaterial', () => {
    it('should return correct material', () => {
      const polyline3DData = createBasicPolyline3DData();

      const material = Polyline3DEntityThreejsRenderer.getMaterial(polyline3DData);

      expect(material).toEqual(polyline3DData.Material);
    });
  });

  describe('getVertexCount', () => {
    it('should return correct vertex count', () => {
      const polyline3DData = createBasicPolyline3DData();

      const vertexCount = Polyline3DEntityThreejsRenderer.getVertexCount(polyline3DData);

      expect(vertexCount).toBe(polyline3DData.VertexCount);
    });
  });

  describe('getTotalLength', () => {
    it('should return correct total length', () => {
      const polyline3DData = createBasicPolyline3DData();

      const totalLength = Polyline3DEntityThreejsRenderer.getTotalLength(polyline3DData);

      expect(totalLength).toBe(polyline3DData.TotalLength);
    });
  });

  describe('getElevation', () => {
    it('should return correct elevation', () => {
      const polyline3DData = createBasicPolyline3DData();

      const elevation = Polyline3DEntityThreejsRenderer.getElevation(polyline3DData);

      expect(elevation).toBe(polyline3DData.Elevation);
    });
  });

  describe('getThickness', () => {
    it('should return correct thickness', () => {
      const polyline3DData = createBasicPolyline3DData();

      const thickness = Polyline3DEntityThreejsRenderer.getThickness(polyline3DData);

      expect(thickness).toBe(polyline3DData.Thickness);
    });
  });

  describe('getIsClosed', () => {
    it('should return correct closed status', () => {
      const polyline3DData = createBasicPolyline3DData();

      const isClosed = Polyline3DEntityThreejsRenderer.getIsClosed(polyline3DData);

      expect(isClosed).toBe(polyline3DData.IsClosed);
    });
  });

  describe('getHasSegments', () => {
    it('should return correct segments status', () => {
      const polyline3DData = createBasicPolyline3DData();

      const hasSegments = Polyline3DEntityThreejsRenderer.getHasSegments(polyline3DData);

      expect(hasSegments).toBe(polyline3DData.HasSegments);
    });
  });

  describe('getLineTypeName', () => {
    it('should return correct line type name', () => {
      const polyline3DData = createBasicPolyline3DData();

      const lineTypeName = Polyline3DEntityThreejsRenderer.getLineTypeName(polyline3DData);

      expect(lineTypeName).toBe(polyline3DData.LineTypeName);
    });
  });

  describe('getLineWeight', () => {
    it('should return correct line weight', () => {
      const polyline3DData = createBasicPolyline3DData();

      const lineWeight = Polyline3DEntityThreejsRenderer.getLineWeight(polyline3DData);

      expect(lineWeight).toBe(polyline3DData.LineWeight);
    });
  });

  describe('getEntityType', () => {
    it('should return correct entity type', () => {
      const polyline3DData = createBasicPolyline3DData();

      const entityType = Polyline3DEntityThreejsRenderer.getEntityType(polyline3DData);

      expect(entityType).toBe(polyline3DData.EntityType);
    });
  });

  describe('clearCache', () => {
    it('should clear all cached polylines', () => {
      const polyline3DData1 = createBasicPolyline3DData();
      polyline3DData1.Handle = 'polyline-1';
      const polyline3DData2 = createBasicPolyline3DData();
      polyline3DData2.Handle = 'polyline-2';

      Polyline3DEntityThreejsRenderer.render(polyline3DData1, scene);
      Polyline3DEntityThreejsRenderer.render(polyline3DData2, scene);

      Polyline3DEntityThreejsRenderer.clearCache();

      const cachedGroup1 = (Polyline3DEntityThreejsRenderer as any).polylineCache.get('polyline-1');
      const cachedGroup2 = (Polyline3DEntityThreejsRenderer as any).polylineCache.get('polyline-2');
      expect(cachedGroup1).toBeUndefined();
      expect(cachedGroup2).toBeUndefined();
    });
  });

  describe('renderFromJson', () => {
    it('should render Polyline3D from JSON string', () => {
      const polyline3DData = createBasicPolyline3DData();
      const jsonString = JSON.stringify(polyline3DData);

      const group = Polyline3DEntityThreejsRenderer.renderFromJson(jsonString, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
    });

    it('should return null for invalid JSON', () => {
      const invalidJson = 'invalid json';

      const group = Polyline3DEntityThreejsRenderer.renderFromJson(invalidJson, scene);

      expect(group).toBeNull();
    });
  });

  describe('updateFromJson', () => {
    it('should update Polyline3D from JSON string', () => {
      const polyline3DData = createBasicPolyline3DData();
      Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      const updatedPolyline3DData = { ...polyline3DData };
      updatedPolyline3DData.Material.Color = 0x00ff00;
      const jsonString = JSON.stringify(updatedPolyline3DData);

      const result = Polyline3DEntityThreejsRenderer.updateFromJson(jsonString, scene);

      expect(result).toBe(true);
    });

    it('should return false for invalid JSON', () => {
      const invalidJson = 'invalid json';

      const result = Polyline3DEntityThreejsRenderer.updateFromJson(invalidJson, scene);

      expect(result).toBe(false);
    });
  });

  describe('disposeFromJson', () => {
    it('should dispose Polyline3D from JSON string', () => {
      const polyline3DData = createBasicPolyline3DData();
      Polyline3DEntityThreejsRenderer.render(polyline3DData, scene);

      const jsonString = JSON.stringify(polyline3DData);

      Polyline3DEntityThreejsRenderer.disposeFromJson(jsonString, scene);

      expect(scene.children.length).toBe(0);
    });

    it('should handle invalid JSON gracefully', () => {
      const invalidJson = 'invalid json';

      expect(() => {
        Polyline3DEntityThreejsRenderer.disposeFromJson(invalidJson, scene);
      }).not.toThrow();
    });
  });

  describe('getSegmentPointAt', () => {
    it('should return correct point at segment', () => {
      const polyline3DData = createBasicPolyline3DData();

      const point = Polyline3DEntityThreejsRenderer.getSegmentPointAt(polyline3DData, 0, 0.5);

      expect(point).not.toBeNull();
      expect(point?.X).toBe(5);
      expect(point?.Y).toBe(0);
      expect(point?.Z).toBe(0);
    });

    it('should return null for invalid segment index', () => {
      const polyline3DData = createBasicPolyline3DData();

      const point = Polyline3DEntityThreejsRenderer.getSegmentPointAt(polyline3DData, 10, 0.5);

      expect(point).toBeNull();
    });

    it('should return null for invalid t value', () => {
      const polyline3DData = createBasicPolyline3DData();

      const point = Polyline3DEntityThreejsRenderer.getSegmentPointAt(polyline3DData, 0, 1.5);

      expect(point).toBeNull();
    });
  });

  describe('getPointAtLength', () => {
    it('should return correct point at length', () => {
      const polyline3DData = createBasicPolyline3DData();

      const point = Polyline3DEntityThreejsRenderer.getPointAtLength(polyline3DData, 15);

      expect(point).not.toBeNull();
      expect(point?.X).toBe(10);
      expect(point?.Y).toBe(5);
      expect(point?.Z).toBe(0);
    });

    it('should return null for invalid length', () => {
      const polyline3DData = createBasicPolyline3DData();

      const point = Polyline3DEntityThreejsRenderer.getPointAtLength(polyline3DData, 100);

      expect(point).toBeNull();
    });
  });

  describe('getClosestPoint', () => {
    it('should return closest point on polyline', () => {
      const polyline3DData = createBasicPolyline3DData();
      const testPoint = { X: 5, Y: 5, Z: 0 };

      const closestPoint = Polyline3DEntityThreejsRenderer.getClosestPoint(polyline3DData, testPoint);

      expect(closestPoint).not.toBeNull();
    });

    it('should handle empty segments', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.Segments = [];
      const testPoint = { X: 5, Y: 5, Z: 0 };

      const closestPoint = Polyline3DEntityThreejsRenderer.getClosestPoint(polyline3DData, testPoint);

      expect(closestPoint).toBeNull();
    });
  });

  describe('getSegmentCount', () => {
    it('should return correct segment count', () => {
      const polyline3DData = createBasicPolyline3DData();

      const segmentCount = Polyline3DEntityThreejsRenderer.getSegmentCount(polyline3DData);

      expect(segmentCount).toBe(4);
    });
  });

  describe('getAverageSegmentLength', () => {
    it('should return correct average segment length', () => {
      const polyline3DData = createBasicPolyline3DData();

      const averageLength = Polyline3DEntityThreejsRenderer.getAverageSegmentLength(polyline3DData);

      expect(averageLength).toBe(10);
    });

    it('should return 0 for empty segments', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.Segments = [];

      const averageLength = Polyline3DEntityThreejsRenderer.getAverageSegmentLength(polyline3DData);

      expect(averageLength).toBe(0);
    });
  });

  describe('getLongestSegment', () => {
    it('should return longest segment', () => {
      const polyline3DData = createBasicPolyline3DData();

      const longestSegment = Polyline3DEntityThreejsRenderer.getLongestSegment(polyline3DData);

      expect(longestSegment).not.toBeNull();
      expect(longestSegment?.Length).toBe(10);
    });

    it('should return null for empty segments', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.Segments = [];

      const longestSegment = Polyline3DEntityThreejsRenderer.getLongestSegment(polyline3DData);

      expect(longestSegment).toBeNull();
    });
  });

  describe('getShortestSegment', () => {
    it('should return shortest segment', () => {
      const polyline3DData = createBasicPolyline3DData();

      const shortestSegment = Polyline3DEntityThreejsRenderer.getShortestSegment(polyline3DData);

      expect(shortestSegment).not.toBeNull();
      expect(shortestSegment?.Length).toBe(10);
    });

    it('should return null for empty segments', () => {
      const polyline3DData = createBasicPolyline3DData();
      polyline3DData.Segments = [];

      const shortestSegment = Polyline3DEntityThreejsRenderer.getShortestSegment(polyline3DData);

      expect(shortestSegment).toBeNull();
    });
  });
});
