import * as THREE from 'three';
import { Polyline2DEntityThreejsRenderer, Polyline2DData, VertexData, PointData, Point3DData, ColorData, TransformData, NormalData, BoundsData, BoundsData3D, MaterialData, GeometryData, ArcSegmentData } from '../Polyline2DEntityThreejsRenderer';

describe('Polyline2DEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    Polyline2DEntityThreejsRenderer.clearCache();
  });

  const createBasicPolyline2DData = (): Polyline2DData => ({
    Vertices: [
      { Location: { X: 0, Y: 0, Z: 0 }, Bulge: 0, StartWidth: 0, EndWidth: 0 },
      { Location: { X: 10, Y: 0, Z: 0 }, Bulge: 0, StartWidth: 0, EndWidth: 0 },
      { Location: { X: 10, Y: 10, Z: 0 }, Bulge: 0, StartWidth: 0, EndWidth: 0 },
      { Location: { X: 0, Y: 10, Z: 0 }, Bulge: 0, StartWidth: 0, EndWidth: 0 }
    ],
    IsClosed: true,
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 1.0,
    LineTypeScale: 1.0,
    Elevation: 0,
    Thickness: 0,
    StartWidth: 0,
    EndWidth: 0,
    SmoothSurface: 0,
    VertexCount: 4,
    SegmentCount: 4,
    Bounds3D: {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 10, Y: 10, Z: 0 }
    },
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
    Vertices3D: [
      { X: 0, Y: 0, Z: 0 },
      { X: 10, Y: 0, Z: 0 },
      { X: 10, Y: 10, Z: 0 },
      { X: 0, Y: 10, Z: 0 }
    ],
    Indices: [],
    NormalsArray: [],
    ColorsArray: [],
    UVsArray: [],
    Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 },
    Normal: { X: 0, Y: 0, Z: 1 },
    Transform: {
      Position: { X: 0, Y: 0, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
    },
    EntityType: 'Polyline2D',
    Visible: true,
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Handle: 'polyline2d-handle-1',
    Transparency: 0,
    MaterialName: 'Default',
    Material: {
      Type: 'LineBasicMaterial',
      Color: 0xff0000,
      Opacity: 1.0,
      Transparent: false,
      Wireframe: false,
      LineWidth: 1.0,
      Side: true
    },
    CastShadows: false,
    ReceiveShadows: false,
    GeometryType: 'BufferGeometry',
    DoubleSided: false,
    FlatShading: false,
    HasArcSegments: false,
    ArcSegments: [],
    HasVariableWidth: false,
    Geometry: {
      Type: 'BufferGeometry',
      VertexCount: 4,
      FaceCount: 0,
      HasNormals: false,
      HasUVs: false,
      HasIndices: false
    },
    VertexPositions: [],
    VertexNormals: [],
    VertexUVs: []
  });

  describe('render', () => {
    it('should render a basic Polyline2D', () => {
      const polyline2DData = createBasicPolyline2DData();

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.name).toBe('Polyline2D_polyline2d-handle-1');
    });

    it('should not add invisible Polyline2D to scene', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.Visible = false;

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.visible).toBe(false);
      expect(scene.children.length).toBe(0);
    });

    it('should render Polyline2D with correct Line object', () => {
      const polyline2DData = createBasicPolyline2DData();

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      expect(group.children.length).toBe(1);
      const line = group.children[0] as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
      expect(line.name).toBe('Polyline2D');
    });

    it('should render Polyline2D with correct material', () => {
      const polyline2DData = createBasicPolyline2DData();

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
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

    it('should render Polyline2D with correct geometry', () => {
      const polyline2DData = createBasicPolyline2DData();

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
      const line = group.children[0] as THREE.Line;

      expect(line.geometry).toBeDefined();
      expect(line.geometry).toBeInstanceOf(THREE.BufferGeometry);
      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position).toBeDefined();
      expect(geometry.attributes.position.count).toBe(4);
    });

    it('should render Polyline2D with correct vertices', () => {
      const polyline2DData = createBasicPolyline2DData();

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
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

    it('should render Polyline2D with correct color', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should render Polyline2D with correct line weight', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.LineWeight = 2.5;

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.linewidth).toBe(2.5);
    });

    it('should render Polyline2D with correct transparency', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.Transparency = 0.5;

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
      expect(material.depthWrite).toBe(false);
    });

    it('should render Polyline2D with arc segments', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.Vertices = [
        { Location: { X: 0, Y: 0, Z: 0 }, Bulge: 0.5, StartWidth: 0, EndWidth: 0 },
        { Location: { X: 10, Y: 0, Z: 0 }, Bulge: 0, StartWidth: 0, EndWidth: 0 }
      ];
      polyline2DData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 0, Z: 0 }
      ];
      polyline2DData.HasArcSegments = true;
      polyline2DData.ArcSegments = [
        {
          StartPoint: { X: 0, Y: 0, Z: 0 },
          EndPoint: { X: 10, Y: 0, Z: 0 },
          CenterPoint: { X: 5, Y: 5, Z: 0 },
          Radius: 5,
          StartAngle: -Math.PI / 2,
          EndAngle: Math.PI / 2,
          SweepAngle: Math.PI,
          IsCounterClockwise: true,
          SegmentCount: 8
        }
      ];

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.children[0] as THREE.Line;
      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position.count).toBeGreaterThan(2);
    });

    it('should render Polyline2D with normals', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.NormalsArray = [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1];

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.normal).toBeDefined();
      expect(geometry.attributes.normal.count).toBe(4);
    });

    it('should render Polyline2D with colors', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.ColorsArray = [1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1];

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.color).toBeDefined();
      expect(geometry.attributes.color.count).toBe(4);
    });

    it('should render Polyline2D with UVs', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.UVsArray = [0, 0, 1, 0, 1, 1, 0, 1];

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.uv).toBeDefined();
      expect(geometry.attributes.uv.count).toBe(4);
    });

    it('should render Polyline2D with indices', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.Indices = [0, 1, 2, 3];

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.index).toBeDefined();
      expect(geometry.index.count).toBe(4);
    });

    it('should cache Polyline2D by handle', () => {
      const polyline2DData = createBasicPolyline2DData();

      Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      const cachedGroup = (Polyline2DEntityThreejsRenderer as any).polylineCache.get('polyline2d-handle-1');
      expect(cachedGroup).toBeDefined();
      expect(cachedGroup).toBeInstanceOf(THREE.Group);
    });

    it('should render Polyline2D with elevation', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.Elevation = 5;
      polyline2DData.Vertices3D = [
        { X: 0, Y: 0, Z: 5 },
        { X: 10, Y: 0, Z: 5 },
        { X: 10, Y: 10, Z: 5 },
        { X: 0, Y: 10, Z: 5 }
      ];

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[2]).toBe(5);
      expect(positions[5]).toBe(5);
      expect(positions[8]).toBe(5);
      expect(positions[11]).toBe(5);
    });

    it('should render Polyline2D with thickness', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.Thickness = 2;

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render open Polyline2D', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.IsClosed = false;
      polyline2DData.Vertices = [
        { Location: { X: 0, Y: 0, Z: 0 }, Bulge: 0, StartWidth: 0, EndWidth: 0 },
        { Location: { X: 10, Y: 0, Z: 0 }, Bulge: 0, StartWidth: 0, EndWidth: 0 },
        { Location: { X: 10, Y: 10, Z: 0 }, Bulge: 0, StartWidth: 0, EndWidth: 0 }
      ];
      polyline2DData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 0, Z: 0 },
        { X: 10, Y: 10, Z: 0 }
      ];

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position.count).toBe(3);
    });

    it('should render Polyline2D with variable width', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.HasVariableWidth = true;
      polyline2DData.Vertices = [
        { Location: { X: 0, Y: 0, Z: 0 }, Bulge: 0, StartWidth: 1, EndWidth: 2 },
        { Location: { X: 10, Y: 0, Z: 0 }, Bulge: 0, StartWidth: 2, EndWidth: 1 },
        { Location: { X: 10, Y: 10, Z: 0 }, Bulge: 0, StartWidth: 1, EndWidth: 2 },
        { Location: { X: 0, Y: 10, Z: 0 }, Bulge: 0, StartWidth: 2, EndWidth: 1 }
      ];

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should handle Polyline2D with no vertices', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.Vertices = [];
      polyline2DData.Vertices3D = [];

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBe(0);
    });

    it('should render Polyline2D with correct handle in userData', () => {
      const polyline2DData = createBasicPolyline2DData();

      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
      const line = group.children[0] as THREE.Line;

      expect(line.userData.handle).toBe('polyline2d-handle-1');
    });
  });

  describe('update', () => {
    it('should update existing Polyline2D', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      const updatedPolyline2DData = { ...polyline2DData };
      updatedPolyline2DData.Vertices3D = [
        { X: 5, Y: 5, Z: 0 },
        { X: 15, Y: 5, Z: 0 },
        { X: 15, Y: 15, Z: 0 },
        { X: 5, Y: 15, Z: 0 }
      ];

      const result = Polyline2DEntityThreejsRenderer.update(updatedPolyline2DData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent Polyline2D', () => {
      const polyline2DData = createBasicPolyline2DData();

      const result = Polyline2DEntityThreejsRenderer.update(polyline2DData, scene);

      expect(result).toBe(false);
    });

    it('should update Polyline2D color', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      const updatedPolyline2DData = { ...polyline2DData };
      updatedPolyline2DData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const result = Polyline2DEntityThreejsRenderer.update(updatedPolyline2DData, scene);

      expect(result).toBe(true);
    });

    it('should update Polyline2D transparency', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      const updatedPolyline2DData = { ...polyline2DData };
      updatedPolyline2DData.Transparency = 0.7;

      const result = Polyline2DEntityThreejsRenderer.update(updatedPolyline2DData, scene);

      expect(result).toBe(true);
    });

    it('should update Polyline2D line weight', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      const updatedPolyline2DData = { ...polyline2DData };
      updatedPolyline2DData.LineWeight = 3.0;

      const result = Polyline2DEntityThreejsRenderer.update(updatedPolyline2DData, scene);

      expect(result).toBe(true);
    });

    it('should update Polyline2D vertices', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      const updatedPolyline2DData = { ...polyline2DData };
      updatedPolyline2DData.Vertices3D = [
        { X: 100, Y: 100, Z: 0 },
        { X: 200, Y: 100, Z: 0 },
        { X: 200, Y: 200, Z: 0 },
        { X: 100, Y: 200, Z: 0 }
      ];

      const result = Polyline2DEntityThreejsRenderer.update(updatedPolyline2DData, scene);

      expect(result).toBe(true);
    });

    it('should update Polyline2D visibility', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      const updatedPolyline2DData = { ...polyline2DData };
      updatedPolyline2DData.Visible = false;

      const result = Polyline2DEntityThreejsRenderer.update(updatedPolyline2DData, scene);

      expect(result).toBe(true);
    });
  });

  describe('dispose', () => {
    it('should dispose Polyline2D resources', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      Polyline2DEntityThreejsRenderer.dispose(polyline2DData, scene);

      expect(scene.children.length).toBe(0);
    });

    it('should handle non-existent Polyline2D gracefully', () => {
      const polyline2DData = createBasicPolyline2DData();

      expect(() => {
        Polyline2DEntityThreejsRenderer.dispose(polyline2DData, scene);
      }).not.toThrow();
    });

    it('should remove Polyline2D from cache', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      Polyline2DEntityThreejsRenderer.dispose(polyline2DData, scene);

      const cachedGroup = (Polyline2DEntityThreejsRenderer as any).polylineCache.get('polyline2d-handle-1');
      expect(cachedGroup).toBeUndefined();
    });

    it('should dispose Polyline2D geometry', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry;
      const originalDispose = geometry.dispose.bind(geometry);
      let disposeCalled = false;
      geometry.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      Polyline2DEntityThreejsRenderer.dispose(polyline2DData, scene);

      expect(disposeCalled).toBe(true);
    });

    it('should dispose Polyline2D material', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material;
      const originalDispose = material.dispose.bind(material);
      let disposeCalled = false;
      material.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      Polyline2DEntityThreejsRenderer.dispose(polyline2DData, scene);

      expect(disposeCalled).toBe(true);
    });
  });

  describe('setVisibility', () => {
    it('should set Polyline2D visibility to true', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      Polyline2DEntityThreejsRenderer.setVisibility(polyline2DData, true);

      expect(group.visible).toBe(true);
    });

    it('should set Polyline2D visibility to false', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      Polyline2DEntityThreejsRenderer.setVisibility(polyline2DData, false);

      expect(group.visible).toBe(false);
    });

    it('should handle non-existent Polyline2D gracefully', () => {
      const polyline2DData = createBasicPolyline2DData();

      expect(() => {
        Polyline2DEntityThreejsRenderer.setVisibility(polyline2DData, true);
      }).not.toThrow();
    });
  });

  describe('setOpacity', () => {
    it('should set Polyline2D opacity', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      Polyline2DEntityThreejsRenderer.setOpacity(polyline2DData, 0.5);

      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should handle non-existent Polyline2D gracefully', () => {
      const polyline2DData = createBasicPolyline2DData();

      expect(() => {
        Polyline2DEntityThreejsRenderer.setOpacity(polyline2DData, 0.5);
      }).not.toThrow();
    });
  });

  describe('setColor', () => {
    it('should set Polyline2D color', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      Polyline2DEntityThreejsRenderer.setColor(polyline2DData, '#00FF00');

      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should handle non-existent Polyline2D gracefully', () => {
      const polyline2DData = createBasicPolyline2DData();

      expect(() => {
        Polyline2DEntityThreejsRenderer.setColor(polyline2DData, '#00FF00');
      }).not.toThrow();
    });
  });

  describe('getGroup', () => {
    it('should return correct group', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      const retrievedGroup = Polyline2DEntityThreejsRenderer.getGroup(polyline2DData);

      expect(retrievedGroup).toBe(group);
    });

    it('should return null for non-existent Polyline2D', () => {
      const polyline2DData = createBasicPolyline2DData();

      const retrievedGroup = Polyline2DEntityThreejsRenderer.getGroup(polyline2DData);

      expect(retrievedGroup).toBeNull();
    });
  });

  describe('getLine', () => {
    it('should return correct line', () => {
      const polyline2DData = createBasicPolyline2DData();
      const group = Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);
      const line = group.children[0] as THREE.Line;

      const retrievedLine = Polyline2DEntityThreejsRenderer.getLine(polyline2DData);

      expect(retrievedLine).toBe(line);
    });

    it('should return null for non-existent Polyline2D', () => {
      const polyline2DData = createBasicPolyline2DData();

      const retrievedLine = Polyline2DEntityThreejsRenderer.getLine(polyline2DData);

      expect(retrievedLine).toBeNull();
    });
  });

  describe('getVertices', () => {
    it('should return correct vertices', () => {
      const polyline2DData = createBasicPolyline2DData();

      const vertices = Polyline2DEntityThreejsRenderer.getVertices(polyline2DData);

      expect(vertices).toEqual(polyline2DData.Vertices3D);
    });
  });

  describe('getIndices', () => {
    it('should return correct indices', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.Indices = [0, 1, 2, 3];

      const indices = Polyline2DEntityThreejsRenderer.getIndices(polyline2DData);

      expect(indices).toEqual(polyline2DData.Indices);
    });
  });

  describe('getNormals', () => {
    it('should return correct normals', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.NormalsArray = [0, 0, 1, 0, 0, 1];

      const normals = Polyline2DEntityThreejsRenderer.getNormals(polyline2DData);

      expect(normals).toEqual(polyline2DData.NormalsArray);
    });
  });

  describe('getColors', () => {
    it('should return correct colors', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.ColorsArray = [1, 0, 0, 1, 0, 1, 0, 1];

      const colors = Polyline2DEntityThreejsRenderer.getColors(polyline2DData);

      expect(colors).toEqual(polyline2DData.ColorsArray);
    });
  });

  describe('getUVs', () => {
    it('should return correct UVs', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.UVsArray = [0, 0, 1, 0, 1, 1, 0, 1];

      const uvs = Polyline2DEntityThreejsRenderer.getUVs(polyline2DData);

      expect(uvs).toEqual(polyline2DData.UVsArray);
    });
  });

  describe('getColor', () => {
    it('should return correct color', () => {
      const polyline2DData = createBasicPolyline2DData();

      const color = Polyline2DEntityThreejsRenderer.getColor(polyline2DData);

      expect(color).toEqual(polyline2DData.Color);
    });
  });

  describe('getBounds', () => {
    it('should return correct bounds', () => {
      const polyline2DData = createBasicPolyline2DData();

      const bounds = Polyline2DEntityThreejsRenderer.getBounds(polyline2DData);

      expect(bounds).toEqual(polyline2DData.Bounds);
    });
  });

  describe('getBounds3D', () => {
    it('should return correct 3D bounds', () => {
      const polyline2DData = createBasicPolyline2DData();

      const bounds3D = Polyline2DEntityThreejsRenderer.getBounds3D(polyline2DData);

      expect(bounds3D).toEqual(polyline2DData.Bounds3D);
    });
  });

  describe('getCentroid', () => {
    it('should return correct centroid', () => {
      const polyline2DData = createBasicPolyline2DData();

      const centroid = Polyline2DEntityThreejsRenderer.getCentroid(polyline2DData);

      expect(centroid).toEqual(polyline2DData.Centroid);
    });
  });

  describe('getCentroid3D', () => {
    it('should return correct 3D centroid', () => {
      const polyline2DData = createBasicPolyline2DData();

      const centroid3D = Polyline2DEntityThreejsRenderer.getCentroid3D(polyline2DData);

      expect(centroid3D).toEqual(polyline2DData.Centroid);
    });
  });

  describe('getHandle', () => {
    it('should return correct handle', () => {
      const polyline2DData = createBasicPolyline2DData();

      const handle = Polyline2DEntityThreejsRenderer.getHandle(polyline2DData);

      expect(handle).toBe(polyline2DData.Handle);
    });
  });

  describe('getLayerName', () => {
    it('should return correct layer name', () => {
      const polyline2DData = createBasicPolyline2DData();

      const layerName = Polyline2DEntityThreejsRenderer.getLayerName(polyline2DData);

      expect(layerName).toBe(polyline2DData.LayerName);
    });
  });

  describe('getVisible', () => {
    it('should return correct visibility', () => {
      const polyline2DData = createBasicPolyline2DData();

      const visible = Polyline2DEntityThreejsRenderer.getVisible(polyline2DData);

      expect(visible).toBe(polyline2DData.Visible);
    });
  });

  describe('getOpacity', () => {
    it('should return correct opacity', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.Transparency = 0.3;

      const opacity = Polyline2DEntityThreejsRenderer.getOpacity(polyline2DData);

      expect(opacity).toBe(0.7);
    });
  });

  describe('getTransparent', () => {
    it('should return correct transparency status', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.Transparency = 0.5;

      const transparent = Polyline2DEntityThreejsRenderer.getTransparent(polyline2DData);

      expect(transparent).toBe(true);
    });

    it('should return false when transparency is 0', () => {
      const polyline2DData = createBasicPolyline2DData();
      polyline2DData.Transparency = 0;

      const transparent = Polyline2DEntityThreejsRenderer.getTransparent(polyline2DData);

      expect(transparent).toBe(false);
    });
  });

  describe('getCastShadows', () => {
    it('should return correct cast shadows status', () => {
      const polyline2DData = createBasicPolyline2DData();

      const castShadows = Polyline2DEntityThreejsRenderer.getCastShadows(polyline2DData);

      expect(castShadows).toBe(polyline2DData.CastShadows);
    });
  });

  describe('getReceiveShadows', () => {
    it('should return correct receive shadows status', () => {
      const polyline2DData = createBasicPolyline2DData();

      const receiveShadows = Polyline2DEntityThreejsRenderer.getReceiveShadows(polyline2DData);

      expect(receiveShadows).toBe(polyline2DData.ReceiveShadows);
    });
  });

  describe('getDoubleSided', () => {
    it('should return correct double sided status', () => {
      const polyline2DData = createBasicPolyline2DData();

      const doubleSided = Polyline2DEntityThreejsRenderer.getDoubleSided(polyline2DData);

      expect(doubleSided).toBe(polyline2DData.DoubleSided);
    });
  });

  describe('getFlatShading', () => {
    it('should return correct flat shading status', () => {
      const polyline2DData = createBasicPolyline2DData();

      const flatShading = Polyline2DEntityThreejsRenderer.getFlatShading(polyline2DData);

      expect(flatShading).toBe(polyline2DData.FlatShading);
    });
  });

  describe('getGeometryType', () => {
    it('should return correct geometry type', () => {
      const polyline2DData = createBasicPolyline2DData();

      const geometryType = Polyline2DEntityThreejsRenderer.getGeometryType(polyline2DData);

      expect(geometryType).toBe(polyline2DData.GeometryType);
    });
  });

  describe('getEntityType', () => {
    it('should return correct entity type', () => {
      const polyline2DData = createBasicPolyline2DData();

      const entityType = Polyline2DEntityThreejsRenderer.getEntityType(polyline2DData);

      expect(entityType).toBe(polyline2DData.EntityType);
    });
  });

  describe('getVertexCount', () => {
    it('should return correct vertex count', () => {
      const polyline2DData = createBasicPolyline2DData();

      const vertexCount = Polyline2DEntityThreejsRenderer.getVertexCount(polyline2DData);

      expect(vertexCount).toBe(polyline2DData.VertexCount);
    });
  });

  describe('getSegmentCount', () => {
    it('should return correct segment count', () => {
      const polyline2DData = createBasicPolyline2DData();

      const segmentCount = Polyline2DEntityThreejsRenderer.getSegmentCount(polyline2DData);

      expect(segmentCount).toBe(polyline2DData.SegmentCount);
    });
  });

  describe('getTotalLength', () => {
    it('should return correct total length', () => {
      const polyline2DData = createBasicPolyline2DData();

      const totalLength = Polyline2DEntityThreejsRenderer.getTotalLength(polyline2DData);

      expect(totalLength).toBe(polyline2DData.TotalLength);
    });
  });

  describe('getElevation', () => {
    it('should return correct elevation', () => {
      const polyline2DData = createBasicPolyline2DData();

      const elevation = Polyline2DEntityThreejsRenderer.getElevation(polyline2DData);

      expect(elevation).toBe(polyline2DData.Elevation);
    });
  });

  describe('getThickness', () => {
    it('should return correct thickness', () => {
      const polyline2DData = createBasicPolyline2DData();

      const thickness = Polyline2DEntityThreejsRenderer.getThickness(polyline2DData);

      expect(thickness).toBe(polyline2DData.Thickness);
    });
  });

  describe('getIsClosed', () => {
    it('should return correct closed status', () => {
      const polyline2DData = createBasicPolyline2DData();

      const isClosed = Polyline2DEntityThreejsRenderer.getIsClosed(polyline2DData);

      expect(isClosed).toBe(polyline2DData.IsClosed);
    });
  });

  describe('getHasArcSegments', () => {
    it('should return correct arc segments status', () => {
      const polyline2DData = createBasicPolyline2DData();

      const hasArcSegments = Polyline2DEntityThreejsRenderer.getHasArcSegments(polyline2DData);

      expect(hasArcSegments).toBe(polyline2DData.HasArcSegments);
    });
  });

  describe('getArcSegments', () => {
    it('should return correct arc segments', () => {
      const polyline2DData = createBasicPolyline2DData();

      const arcSegments = Polyline2DEntityThreejsRenderer.getArcSegments(polyline2DData);

      expect(arcSegments).toEqual(polyline2DData.ArcSegments);
    });
  });

  describe('getHasVariableWidth', () => {
    it('should return correct variable width status', () => {
      const polyline2DData = createBasicPolyline2DData();

      const hasVariableWidth = Polyline2DEntityThreejsRenderer.getHasVariableWidth(polyline2DData);

      expect(hasVariableWidth).toBe(polyline2DData.HasVariableWidth);
    });
  });

  describe('getTransform', () => {
    it('should return correct transform', () => {
      const polyline2DData = createBasicPolyline2DData();

      const transform = Polyline2DEntityThreejsRenderer.getTransform(polyline2DData);

      expect(transform).toEqual(polyline2DData.Transform);
    });
  });

  describe('clearCache', () => {
    it('should clear all cached polylines', () => {
      const polyline2DData1 = createBasicPolyline2DData();
      polyline2DData1.Handle = 'polyline-1';
      const polyline2DData2 = createBasicPolyline2DData();
      polyline2DData2.Handle = 'polyline-2';

      Polyline2DEntityThreejsRenderer.render(polyline2DData1, scene);
      Polyline2DEntityThreejsRenderer.render(polyline2DData2, scene);

      Polyline2DEntityThreejsRenderer.clearCache();

      const cachedGroup1 = (Polyline2DEntityThreejsRenderer as any).polylineCache.get('polyline-1');
      const cachedGroup2 = (Polyline2DEntityThreejsRenderer as any).polylineCache.get('polyline-2');
      expect(cachedGroup1).toBeUndefined();
      expect(cachedGroup2).toBeUndefined();
    });
  });

  describe('renderFromJson', () => {
    it('should render Polyline2D from JSON string', () => {
      const polyline2DData = createBasicPolyline2DData();
      const jsonString = JSON.stringify(polyline2DData);

      const group = Polyline2DEntityThreejsRenderer.renderFromJson(jsonString, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
    });

    it('should return null for invalid JSON', () => {
      const invalidJson = 'invalid json';

      const group = Polyline2DEntityThreejsRenderer.renderFromJson(invalidJson, scene);

      expect(group).toBeNull();
    });
  });

  describe('updateFromJson', () => {
    it('should update Polyline2D from JSON string', () => {
      const polyline2DData = createBasicPolyline2DData();
      Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      const updatedPolyline2DData = { ...polyline2DData };
      updatedPolyline2DData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };
      const jsonString = JSON.stringify(updatedPolyline2DData);

      const result = Polyline2DEntityThreejsRenderer.updateFromJson(jsonString, scene);

      expect(result).toBe(true);
    });

    it('should return false for invalid JSON', () => {
      const invalidJson = 'invalid json';

      const result = Polyline2DEntityThreejsRenderer.updateFromJson(invalidJson, scene);

      expect(result).toBe(false);
    });
  });

  describe('disposeFromJson', () => {
    it('should dispose Polyline2D from JSON string', () => {
      const polyline2DData = createBasicPolyline2DData();
      Polyline2DEntityThreejsRenderer.render(polyline2DData, scene);

      const jsonString = JSON.stringify(polyline2DData);

      Polyline2DEntityThreejsRenderer.disposeFromJson(jsonString, scene);

      expect(scene.children.length).toBe(0);
    });

    it('should handle invalid JSON gracefully', () => {
      const invalidJson = 'invalid json';

      expect(() => {
        Polyline2DEntityThreejsRenderer.disposeFromJson(invalidJson, scene);
      }).not.toThrow();
    });
  });
});
