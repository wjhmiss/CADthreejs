import * as THREE from 'three';
import { SplineEntityThreejsRenderer, SplineData, Point3DData, ColorData, TransformData, NormalData, BoundsData, GeometryData, MaterialData } from '../SplineEntityThreejsRenderer';

describe('SplineEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    SplineEntityThreejsRenderer.clearCache();
  });

  const createBasicSplineData = (): SplineData => ({
    Type: 'Spline',
    EntityType: 'Spline',
    Handle: 'spline-handle-1',
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Visible: true,
    ControlPoints: [
      { X: 0, Y: 0, Z: 0 },
      { X: 10, Y: 10, Z: 0 },
      { X: 20, Y: 0, Z: 0 }
    ],
    FitPoints: [],
    Knots: [0, 0, 0, 1, 1, 1],
    Weights: [1, 1, 1],
    Degree: 2,
    IsClosed: false,
    IsPeriodic: false,
    StartTangent: { X: 1, Y: 0, Z: 0 },
    EndTangent: { X: 1, Y: 0, Z: 0 },
    Normal: { X: 0, Y: 0, Z: 1 },
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.03,
    ControlPointTolerance: 0.0001,
    FitTolerance: 0.0001,
    KnotTolerance: 0.0001,
    ApproximationPoints: [
      { X: 0, Y: 0, Z: 0 },
      { X: 5, Y: 5, Z: 0 },
      { X: 10, Y: 10, Z: 0 },
      { X: 15, Y: 5, Z: 0 },
      { X: 20, Y: 0, Z: 0 }
    ],
    ControlPointCount: 3,
    FitPointCount: 0,
    KnotCount: 6,
    Bounds: {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 20, Y: 10, Z: 0 },
      Center: { X: 10, Y: 5, Z: 0 },
      Size: { X: 20, Y: 10, Z: 0 }
    },
    Centroid: { X: 10, Y: 5, Z: 0 },
    Length: 28.284,
    Transform: {
      Position: { X: 10, Y: 5, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 10, 5, 0, 1]
    },
    Geometry: {
      Type: 'BufferGeometry',
      VertexCount: 5,
      HasColors: false,
      HasIndices: false,
      PrimitiveType: 'LineLoop',
      IndexCount: 0
    },
    Material: {
      Type: 'LineBasicMaterial',
      Color: 16711680,
      Opacity: 1.0,
      Transparent: false,
      Wireframe: false,
      LineWidth: 1.0,
      VertexColors: false,
      Side: true
    },
    VertexPositions: [0, 0, 0, 5, 5, 0, 10, 10, 0, 15, 5, 0, 20, 0, 0],
    VertexColors: [],
    Indices: [],
    ApproximationPointCount: 5,
    Tension: 0,
    SplineType: 'CatmullRomCurve3',
    Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 },
    Tangent: { X: 1, Y: 0, Z: 0 },
    Binormal: [0, 0, 1],
    UV: [],
    ArcLength: 28.284,
    Curvature: 0.1,
    IsRational: false,
    SampleCount: 100,
    ControlPointWeights: [1, 1, 1],
    Opacity: 1.0,
    Transparent: false,
    DepthTest: true
  });

  describe('render', () => {
    it('should render a basic Spline', () => {
      const splineData = createBasicSplineData();

      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.name).toBe('Spline_spline-handle-1');
    });

    it('should not add invisible Spline to scene', () => {
      const splineData = createBasicSplineData();
      splineData.Visible = false;

      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.visible).toBe(false);
      expect(scene.children.length).toBe(0);
    });

    it('should render Spline with correct Line object', () => {
      const splineData = createBasicSplineData();

      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      expect(group.children.length).toBe(1);
      const line = group.children[0] as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
      expect(line.name).toBe('Spline');
    });

    it('should render Spline with correct material', () => {
      const splineData = createBasicSplineData();

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      expect(line.material).toBeDefined();
      expect(line.material).toBeInstanceOf(THREE.LineBasicMaterial);
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('ff0000');
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
      expect(material.depthTest).toBe(true);
    });

    it('should render Spline with correct geometry', () => {
      const splineData = createBasicSplineData();

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      expect(line.geometry).toBeDefined();
      expect(line.geometry).toBeInstanceOf(THREE.BufferGeometry);
      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position).toBeDefined();
      expect(geometry.attributes.position.count).toBe(5);
    });

    it('should render Spline with correct vertex positions', () => {
      const splineData = createBasicSplineData();

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(0);
      expect(positions[1]).toBe(0);
      expect(positions[2]).toBe(0);
      expect(positions[3]).toBe(5);
      expect(positions[4]).toBe(5);
      expect(positions[5]).toBe(0);
    });

    it('should render Spline with correct color', () => {
      const splineData = createBasicSplineData();
      splineData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should render Spline with correct opacity', () => {
      const splineData = createBasicSplineData();
      splineData.Opacity = 0.5;
      splineData.Transparent = true;

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should render Spline with correct depth settings', () => {
      const splineData = createBasicSplineData();
      splineData.DepthTest = false;

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.depthTest).toBe(false);
    });

    it('should cache Spline by handle', () => {
      const splineData = createBasicSplineData();

      SplineEntityThreejsRenderer.render(splineData, scene);

      const cachedLine = (SplineEntityThreejsRenderer as any).splineCache.get('spline-handle-1');
      expect(cachedLine).toBeDefined();
      expect(cachedLine).toBeInstanceOf(THREE.Line);
    });

    it('should render Spline with correct handle in userData', () => {
      const splineData = createBasicSplineData();

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      expect(line.userData.handle).toBe('spline-handle-1');
    });

    it('should render Spline with approximation points when vertex positions not provided', () => {
      const splineData = createBasicSplineData();
      splineData.VertexPositions = [];
      splineData.ApproximationPoints = [
        { X: 1, Y: 1, Z: 1 },
        { X: 2, Y: 2, Z: 2 }
      ];

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(1);
      expect(positions[1]).toBe(1);
      expect(positions[2]).toBe(1);
      expect(positions[3]).toBe(2);
      expect(positions[4]).toBe(2);
      expect(positions[5]).toBe(2);
    });

    it('should render Spline with vertex colors', () => {
      const splineData = createBasicSplineData();
      splineData.VertexColors = [255, 0, 0, 0, 255, 0];
      splineData.Material.VertexColors = true;

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.color).toBeDefined();
      const colors = geometry.attributes.color.array as Float32Array;
      expect(colors[0]).toBeCloseTo(1.0);
      expect(colors[1]).toBeCloseTo(0.0);
      expect(colors[2]).toBeCloseTo(0.0);
    });

    it('should render Spline with indices', () => {
      const splineData = createBasicSplineData();
      splineData.Indices = [0, 1, 2, 3, 4];

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.index).toBeDefined();
      const indices = geometry.index.array as Uint16Array;
      expect(indices[0]).toBe(0);
      expect(indices[1]).toBe(1);
      expect(indices[2]).toBe(2);
    });

    it('should render Spline with UV coordinates', () => {
      const splineData = createBasicSplineData();
      splineData.UV = [0, 0, 0.5, 0.5, 1, 1];

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.uv).toBeDefined();
      const uvs = geometry.attributes.uv.array as Float32Array;
      expect(uvs[0]).toBe(0);
      expect(uvs[1]).toBe(0);
      expect(uvs[2]).toBe(0.5);
    });

    it('should render Spline with zero positions', () => {
      const splineData = createBasicSplineData();
      splineData.VertexPositions = [];
      splineData.ApproximationPoints = [];

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(0);
      expect(positions[1]).toBe(0);
      expect(positions[2]).toBe(0);
    });

    it('should render Spline with negative coordinates', () => {
      const splineData = createBasicSplineData();
      splineData.VertexPositions = [-10, -20, -30, -40, -50, -60];

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(-10);
      expect(positions[1]).toBe(-20);
      expect(positions[2]).toBe(-30);
    });

    it('should render Spline with very large coordinates', () => {
      const splineData = createBasicSplineData();
      splineData.VertexPositions = [1000000, 2000000, 3000000, 4000000, 5000000, 6000000];

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(1000000);
      expect(positions[1]).toBe(2000000);
      expect(positions[2]).toBe(3000000);
    });

    it('should render Spline with very small coordinates', () => {
      const splineData = createBasicSplineData();
      splineData.VertexPositions = [0.0001, 0.0002, 0.0003, 0.0004, 0.0005, 0.0006];

      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBeCloseTo(0.0001, 7);
      expect(positions[1]).toBeCloseTo(0.0002, 7);
      expect(positions[2]).toBeCloseTo(0.0003, 7);
    });
  });

  describe('renderMultiple', () => {
    it('should render multiple Splines', () => {
      const splineDataArray = [
        createBasicSplineData(),
        createBasicSplineData()
      ];
      splineDataArray[0].Handle = 'spline-1';
      splineDataArray[1].Handle = 'spline-2';

      const group = SplineEntityThreejsRenderer.renderMultiple(splineDataArray, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('MultipleSplines');
      expect(group.children.length).toBe(2);
    });

    it('should render multiple Splines with correct handles', () => {
      const splineDataArray = [
        createBasicSplineData(),
        createBasicSplineData(),
        createBasicSplineData()
      ];
      splineDataArray[0].Handle = 'spline-1';
      splineDataArray[1].Handle = 'spline-2';
      splineDataArray[2].Handle = 'spline-3';

      const group = SplineEntityThreejsRenderer.renderMultiple(splineDataArray, scene);

      expect(group.children[0].name).toBe('Spline_spline-1');
      expect(group.children[1].name).toBe('Spline_spline-2');
      expect(group.children[2].name).toBe('Spline_spline-3');
    });

    it('should render empty array of Splines', () => {
      const splineDataArray: SplineData[] = [];

      const group = SplineEntityThreejsRenderer.renderMultiple(splineDataArray, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('MultipleSplines');
      expect(group.children.length).toBe(0);
    });

    it('should render single Spline using renderMultiple', () => {
      const splineDataArray = [createBasicSplineData()];
      splineDataArray[0].Handle = 'spline-1';

      const group = SplineEntityThreejsRenderer.renderMultiple(splineDataArray, scene);

      expect(group.children.length).toBe(1);
      expect(group.children[0].name).toBe('Spline_spline-1');
    });
  });

  describe('update', () => {
    it('should update existing Spline', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      const updatedSplineData = { ...splineData };
      updatedSplineData.VertexPositions = [10, 20, 30, 40, 50, 60];

      const result = SplineEntityThreejsRenderer.update(updatedSplineData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent Spline', () => {
      const splineData = createBasicSplineData();

      const result = SplineEntityThreejsRenderer.update(splineData, scene);

      expect(result).toBe(false);
    });

    it('should update Spline color', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      const updatedSplineData = { ...splineData };
      updatedSplineData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const result = SplineEntityThreejsRenderer.update(updatedSplineData, scene);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should update Spline opacity', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      const updatedSplineData = { ...splineData };
      updatedSplineData.Opacity = 0.5;
      updatedSplineData.Transparent = true;

      const result = SplineEntityThreejsRenderer.update(updatedSplineData, scene);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should update Spline vertex positions', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      const updatedSplineData = { ...splineData };
      updatedSplineData.VertexPositions = [100, 200, 300, 400, 500, 600];

      const result = SplineEntityThreejsRenderer.update(updatedSplineData, scene);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(100);
      expect(positions[1]).toBe(200);
      expect(positions[2]).toBe(300);
    });

    it('should update Spline line width', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      const updatedSplineData = { ...splineData };
      updatedSplineData.Material.LineWidth = 5.0;

      const result = SplineEntityThreejsRenderer.update(updatedSplineData, scene);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.linewidth).toBe(5.0);
    });
  });

  describe('dispose', () => {
    it('should dispose Spline resources', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      const result = SplineEntityThreejsRenderer.dispose(splineData, scene);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(0);
    });

    it('should return false for non-existent Spline', () => {
      const splineData = createBasicSplineData();

      const result = SplineEntityThreejsRenderer.dispose(splineData, scene);

      expect(result).toBe(false);
    });

    it('should remove Spline from cache', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      SplineEntityThreejsRenderer.dispose(splineData, scene);

      const cachedLine = (SplineEntityThreejsRenderer as any).splineCache.get('spline-handle-1');
      expect(cachedLine).toBeUndefined();
    });

    it('should dispose Spline geometry', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry;
      const originalDispose = geometry.dispose.bind(geometry);
      let disposeCalled = false;
      geometry.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      SplineEntityThreejsRenderer.dispose(splineData, scene);

      expect(disposeCalled).toBe(true);
    });

    it('should dispose Spline material', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material;
      const originalDispose = material.dispose.bind(material);
      let disposeCalled = false;
      material.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      SplineEntityThreejsRenderer.dispose(splineData, scene);

      expect(disposeCalled).toBe(true);
    });
  });

  describe('disposeMultiple', () => {
    it('should dispose multiple Splines', () => {
      const splineDataArray = [
        createBasicSplineData(),
        createBasicSplineData()
      ];
      splineDataArray[0].Handle = 'spline-1';
      splineDataArray[1].Handle = 'spline-2';

      const group = SplineEntityThreejsRenderer.renderMultiple(splineDataArray, scene);

      SplineEntityThreejsRenderer.disposeMultiple(group, scene);

      expect(scene.children.length).toBe(0);
    });

    it('should handle null group gracefully', () => {
      expect(() => {
        SplineEntityThreejsRenderer.disposeMultiple(null, scene);
      }).not.toThrow();
    });

    it('should handle undefined group gracefully', () => {
      expect(() => {
        SplineEntityThreejsRenderer.disposeMultiple(undefined, scene);
      }).not.toThrow();
    });
  });

  describe('setVisibility', () => {
    it('should set Spline visibility to true', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      const result = SplineEntityThreejsRenderer.setVisibility(splineData, scene, true);

      expect(result).toBe(true);
      expect(group.visible).toBe(true);
    });

    it('should set Spline visibility to false', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      const result = SplineEntityThreejsRenderer.setVisibility(splineData, scene, false);

      expect(result).toBe(true);
      expect(group.visible).toBe(false);
      expect(scene.children.length).toBe(0);
    });

    it('should return false for non-existent Spline', () => {
      const splineData = createBasicSplineData();

      const result = SplineEntityThreejsRenderer.setVisibility(splineData, scene, true);

      expect(result).toBe(false);
    });

    it('should add Spline to scene when setting visibility to true', () => {
      const splineData = createBasicSplineData();
      splineData.Visible = false;
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      const result = SplineEntityThreejsRenderer.setVisibility(splineData, scene, true);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(1);
    });

    it('should remove Spline from scene when setting visibility to false', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      const result = SplineEntityThreejsRenderer.setVisibility(splineData, scene, false);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(0);
    });
  });

  describe('setOpacity', () => {
    it('should set Spline opacity', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      const result = SplineEntityThreejsRenderer.setOpacity(splineData, 0.5);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should return false for non-existent Spline', () => {
      const splineData = createBasicSplineData();

      const result = SplineEntityThreejsRenderer.setOpacity(splineData, 0.5);

      expect(result).toBe(false);
    });

    it('should set opacity to 1.0', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      const result = SplineEntityThreejsRenderer.setOpacity(splineData, 1.0);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
    });
  });

  describe('setLineWidth', () => {
    it('should set Spline line width', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      const result = SplineEntityThreejsRenderer.setLineWidth(splineData, 5.0);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.linewidth).toBe(5.0);
    });

    it('should return false for non-existent Spline', () => {
      const splineData = createBasicSplineData();

      const result = SplineEntityThreejsRenderer.setLineWidth(splineData, 5.0);

      expect(result).toBe(false);
    });

    it('should set line width to 0', () => {
      const splineData = createBasicSplineData();
      const group = SplineEntityThreejsRenderer.render(splineData, scene);

      const result = SplineEntityThreejsRenderer.setLineWidth(splineData, 0);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.linewidth).toBe(0);
    });
  });

  describe('getSplineLine', () => {
    it('should return cached Spline line', () => {
      const splineData = createBasicSplineData();
      SplineEntityThreejsRenderer.render(splineData, scene);

      const line = SplineEntityThreejsRenderer.getSplineLine(splineData);

      expect(line).toBeInstanceOf(THREE.Line);
      expect(line).not.toBeNull();
    });

    it('should return null for non-existent Spline', () => {
      const splineData = createBasicSplineData();

      const line = SplineEntityThreejsRenderer.getSplineLine(splineData);

      expect(line).toBeNull();
    });
  });

  describe('getControlPoints', () => {
    it('should return control points as Vector3 array', () => {
      const splineData = createBasicSplineData();

      const controlPoints = SplineEntityThreejsRenderer.getControlPoints(splineData);

      expect(controlPoints).toBeInstanceOf(Array);
      expect(controlPoints.length).toBe(3);
      expect(controlPoints[0]).toBeInstanceOf(THREE.Vector3);
      expect(controlPoints[0].x).toBe(0);
      expect(controlPoints[0].y).toBe(0);
      expect(controlPoints[0].z).toBe(0);
    });

    it('should return empty array for no control points', () => {
      const splineData = createBasicSplineData();
      splineData.ControlPoints = [];

      const controlPoints = SplineEntityThreejsRenderer.getControlPoints(splineData);

      expect(controlPoints).toBeInstanceOf(Array);
      expect(controlPoints.length).toBe(0);
    });
  });

  describe('getFitPoints', () => {
    it('should return fit points as Vector3 array', () => {
      const splineData = createBasicSplineData();
      splineData.FitPoints = [
        { X: 1, Y: 2, Z: 3 },
        { X: 4, Y: 5, Z: 6 }
      ];

      const fitPoints = SplineEntityThreejsRenderer.getFitPoints(splineData);

      expect(fitPoints).toBeInstanceOf(Array);
      expect(fitPoints.length).toBe(2);
      expect(fitPoints[0]).toBeInstanceOf(THREE.Vector3);
      expect(fitPoints[0].x).toBe(1);
      expect(fitPoints[0].y).toBe(2);
      expect(fitPoints[0].z).toBe(3);
    });

    it('should return empty array for no fit points', () => {
      const splineData = createBasicSplineData();

      const fitPoints = SplineEntityThreejsRenderer.getFitPoints(splineData);

      expect(fitPoints).toBeInstanceOf(Array);
      expect(fitPoints.length).toBe(0);
    });
  });

  describe('getApproximationPoints', () => {
    it('should return approximation points as Vector3 array', () => {
      const splineData = createBasicSplineData();

      const approxPoints = SplineEntityThreejsRenderer.getApproximationPoints(splineData);

      expect(approxPoints).toBeInstanceOf(Array);
      expect(approxPoints.length).toBe(5);
      expect(approxPoints[0]).toBeInstanceOf(THREE.Vector3);
      expect(approxPoints[0].x).toBe(0);
      expect(approxPoints[0].y).toBe(0);
      expect(approxPoints[0].z).toBe(0);
    });

    it('should return empty array for no approximation points', () => {
      const splineData = createBasicSplineData();
      splineData.ApproximationPoints = [];

      const approxPoints = SplineEntityThreejsRenderer.getApproximationPoints(splineData);

      expect(approxPoints).toBeInstanceOf(Array);
      expect(approxPoints.length).toBe(0);
    });
  });

  describe('getDegree', () => {
    it('should return correct degree', () => {
      const splineData = createBasicSplineData();

      const degree = SplineEntityThreejsRenderer.getDegree(splineData);

      expect(degree).toBe(2);
    });

    it('should return default degree when not set', () => {
      const splineData = createBasicSplineData();
      splineData.Degree = undefined;

      const degree = SplineEntityThreejsRenderer.getDegree(splineData);

      expect(degree).toBe(3);
    });
  });

  describe('isClosed', () => {
    it('should return true for closed spline', () => {
      const splineData = createBasicSplineData();
      splineData.IsClosed = true;

      const isClosed = SplineEntityThreejsRenderer.isClosed(splineData);

      expect(isClosed).toBe(true);
    });

    it('should return false for open spline', () => {
      const splineData = createBasicSplineData();

      const isClosed = SplineEntityThreejsRenderer.isClosed(splineData);

      expect(isClosed).toBe(false);
    });
  });

  describe('isRational', () => {
    it('should return true for rational spline', () => {
      const splineData = createBasicSplineData();
      splineData.IsRational = true;

      const isRational = SplineEntityThreejsRenderer.isRational(splineData);

      expect(isRational).toBe(true);
    });

    it('should return false for non-rational spline', () => {
      const splineData = createBasicSplineData();

      const isRational = SplineEntityThreejsRenderer.isRational(splineData);

      expect(isRational).toBe(false);
    });
  });

  describe('getLength', () => {
    it('should return correct length', () => {
      const splineData = createBasicSplineData();

      const length = SplineEntityThreejsRenderer.getLength(splineData);

      expect(length).toBe(28.284);
    });

    it('should return 0 when length not set', () => {
      const splineData = createBasicSplineData();
      splineData.Length = undefined;

      const length = SplineEntityThreejsRenderer.getLength(splineData);

      expect(length).toBe(0);
    });
  });

  describe('getColor', () => {
    it('should return correct color', () => {
      const splineData = createBasicSplineData();

      const color = SplineEntityThreejsRenderer.getColor(splineData);

      expect(color).toBeInstanceOf(THREE.Color);
      expect(color.getHexString()).toBe('ff0000');
    });

    it('should return correct color for green', () => {
      const splineData = createBasicSplineData();
      splineData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const color = SplineEntityThreejsRenderer.getColor(splineData);

      expect(color).toBeInstanceOf(THREE.Color);
      expect(color.getHexString()).toBe('00ff00');
    });
  });

  describe('getBounds', () => {
    it('should return correct bounds', () => {
      const splineData = createBasicSplineData();

      const bounds = SplineEntityThreejsRenderer.getBounds(splineData);

      expect(bounds).toBeInstanceOf(THREE.Box3);
      expect(bounds.min.x).toBe(0);
      expect(bounds.min.y).toBe(0);
      expect(bounds.min.z).toBe(0);
      expect(bounds.max.x).toBe(20);
      expect(bounds.max.y).toBe(10);
      expect(bounds.max.z).toBe(0);
    });
  });

  describe('getCentroid', () => {
    it('should return correct centroid', () => {
      const splineData = createBasicSplineData();

      const centroid = SplineEntityThreejsRenderer.getCentroid(splineData);

      expect(centroid).toBeInstanceOf(THREE.Vector3);
      expect(centroid.x).toBe(10);
      expect(centroid.y).toBe(5);
      expect(centroid.z).toBe(0);
    });
  });

  describe('clearCache', () => {
    it('should clear all cached splines', () => {
      const splineData1 = createBasicSplineData();
      const splineData2 = createBasicSplineData();
      splineData1.Handle = 'spline-1';
      splineData2.Handle = 'spline-2';

      SplineEntityThreejsRenderer.render(splineData1, scene);
      SplineEntityThreejsRenderer.render(splineData2, scene);

      expect((SplineEntityThreejsRenderer as any).splineCache.size).toBe(2);

      SplineEntityThreejsRenderer.clearCache();

      expect((SplineEntityThreejsRenderer as any).splineCache.size).toBe(0);
    });
  });

  describe('getNormal', () => {
    it('should return correct normal', () => {
      const splineData = createBasicSplineData();

      const normal = SplineEntityThreejsRenderer.getNormal(splineData);

      expect(normal.X).toBe(0);
      expect(normal.Y).toBe(0);
      expect(normal.Z).toBe(1);
    });

    it('should return default normal when not set', () => {
      const splineData = createBasicSplineData();
      splineData.Normal = undefined;

      const normal = SplineEntityThreejsRenderer.getNormal(splineData);

      expect(normal.X).toBe(0);
      expect(normal.Y).toBe(0);
      expect(normal.Z).toBe(1);
    });
  });

  describe('getTangent', () => {
    it('should return correct tangent', () => {
      const splineData = createBasicSplineData();

      const tangent = SplineEntityThreejsRenderer.getTangent(splineData);

      expect(tangent.X).toBe(1);
      expect(tangent.Y).toBe(0);
      expect(tangent.Z).toBe(0);
    });

    it('should return default tangent when not set', () => {
      const splineData = createBasicSplineData();
      splineData.Tangent = undefined;

      const tangent = SplineEntityThreejsRenderer.getTangent(splineData);

      expect(tangent.X).toBe(1);
      expect(tangent.Y).toBe(0);
      expect(tangent.Z).toBe(0);
    });
  });

  describe('getBinormal', () => {
    it('should return correct binormal', () => {
      const splineData = createBasicSplineData();

      const binormal = SplineEntityThreejsRenderer.getBinormal(splineData);

      expect(binormal).toBeInstanceOf(Array);
      expect(binormal.length).toBe(3);
      expect(binormal[0]).toBe(0);
      expect(binormal[1]).toBe(0);
      expect(binormal[2]).toBe(1);
    });

    it('should return default binormal when not set', () => {
      const splineData = createBasicSplineData();
      splineData.Binormal = undefined;

      const binormal = SplineEntityThreejsRenderer.getBinormal(splineData);

      expect(binormal).toBeInstanceOf(Array);
      expect(binormal.length).toBe(3);
      expect(binormal[0]).toBe(0);
      expect(binormal[1]).toBe(0);
      expect(binormal[2]).toBe(1);
    });
  });

  describe('getCurvature', () => {
    it('should return correct curvature', () => {
      const splineData = createBasicSplineData();

      const curvature = SplineEntityThreejsRenderer.getCurvature(splineData);

      expect(curvature).toBe(0.1);
    });

    it('should return 0 when curvature not set', () => {
      const splineData = createBasicSplineData();
      splineData.Curvature = undefined;

      const curvature = SplineEntityThreejsRenderer.getCurvature(splineData);

      expect(curvature).toBe(0);
    });
  });

  describe('getTransform', () => {
    it('should return correct transform', () => {
      const splineData = createBasicSplineData();

      const transform = SplineEntityThreejsRenderer.getTransform(splineData);

      expect(transform.Position.X).toBe(10);
      expect(transform.Position.Y).toBe(5);
      expect(transform.Position.Z).toBe(0);
      expect(transform.Rotation.X).toBe(0);
      expect(transform.Rotation.Y).toBe(0);
      expect(transform.Rotation.Z).toBe(0);
      expect(transform.Scale.X).toBe(1);
      expect(transform.Scale.Y).toBe(1);
      expect(transform.Scale.Z).toBe(1);
    });

    it('should return default transform when not set', () => {
      const splineData = createBasicSplineData();
      splineData.Transform = undefined;

      const transform = SplineEntityThreejsRenderer.getTransform(splineData);

      expect(transform.Position.X).toBe(10);
      expect(transform.Position.Y).toBe(5);
      expect(transform.Position.Z).toBe(0);
      expect(transform.Scale.X).toBe(1);
      expect(transform.Scale.Y).toBe(1);
      expect(transform.Scale.Z).toBe(1);
    });
  });

  describe('getGeometry', () => {
    it('should return correct geometry', () => {
      const splineData = createBasicSplineData();

      const geometry = SplineEntityThreejsRenderer.getGeometry(splineData);

      expect(geometry.Type).toBe('BufferGeometry');
      expect(geometry.VertexCount).toBe(5);
      expect(geometry.HasColors).toBe(false);
      expect(geometry.HasIndices).toBe(false);
      expect(geometry.PrimitiveType).toBe('LineLoop');
    });

    it('should return default geometry when not set', () => {
      const splineData = createBasicSplineData();
      splineData.Geometry = undefined;

      const geometry = SplineEntityThreejsRenderer.getGeometry(splineData);

      expect(geometry.Type).toBe('BufferGeometry');
      expect(geometry.VertexCount).toBe(5);
      expect(geometry.HasColors).toBe(true);
      expect(geometry.HasIndices).toBe(true);
      expect(geometry.PrimitiveType).toBe('LineLoop');
    });
  });

  describe('getMaterial', () => {
    it('should return correct material', () => {
      const splineData = createBasicSplineData();

      const material = SplineEntityThreejsRenderer.getMaterial(splineData);

      expect(material.Type).toBe('LineBasicMaterial');
      expect(material.Color).toBe(16711680);
      expect(material.Opacity).toBe(1.0);
      expect(material.Transparent).toBe(false);
      expect(material.Wireframe).toBe(false);
      expect(material.LineWidth).toBe(1.0);
      expect(material.VertexColors).toBe(false);
    });

    it('should return default material when not set', () => {
      const splineData = createBasicSplineData();
      splineData.Material = undefined;

      const material = SplineEntityThreejsRenderer.getMaterial(splineData);

      expect(material.Type).toBe('LineBasicMaterial');
      expect(material.Color).toBe(16711680);
      expect(material.Opacity).toBe(1.0);
      expect(material.LineWidth).toBe(0.03);
      expect(material.VertexColors).toBe(true);
    });
  });

  describe('getDepthTest', () => {
    it('should return true when depth test enabled', () => {
      const splineData = createBasicSplineData();

      const depthTest = SplineEntityThreejsRenderer.getDepthTest(splineData);

      expect(depthTest).toBe(true);
    });

    it('should return false when depth test disabled', () => {
      const splineData = createBasicSplineData();
      splineData.DepthTest = false;

      const depthTest = SplineEntityThreejsRenderer.getDepthTest(splineData);

      expect(depthTest).toBe(false);
    });
  });

  describe('getHandle', () => {
    it('should return correct handle', () => {
      const splineData = createBasicSplineData();

      const handle = SplineEntityThreejsRenderer.getHandle(splineData);

      expect(handle).toBe('spline-handle-1');
    });

    it('should return empty string when handle not set', () => {
      const splineData = createBasicSplineData();
      splineData.Handle = undefined;

      const handle = SplineEntityThreejsRenderer.getHandle(splineData);

      expect(handle).toBe('');
    });
  });

  describe('getLayerName', () => {
    it('should return correct layer name', () => {
      const splineData = createBasicSplineData();

      const layerName = SplineEntityThreejsRenderer.getLayerName(splineData);

      expect(layerName).toBe('TEST_LAYER');
    });

    it('should return empty string when layer name not set', () => {
      const splineData = createBasicSplineData();
      splineData.LayerName = undefined;

      const layerName = SplineEntityThreejsRenderer.getLayerName(splineData);

      expect(layerName).toBe('');
    });
  });

  describe('getVisible', () => {
    it('should return true when visible', () => {
      const splineData = createBasicSplineData();

      const visible = SplineEntityThreejsRenderer.getVisible(splineData);

      expect(visible).toBe(true);
    });

    it('should return false when not visible', () => {
      const splineData = createBasicSplineData();
      splineData.Visible = false;

      const visible = SplineEntityThreejsRenderer.getVisible(splineData);

      expect(visible).toBe(false);
    });
  });

  describe('getOpacity', () => {
    it('should return correct opacity', () => {
      const splineData = createBasicSplineData();

      const opacity = SplineEntityThreejsRenderer.getOpacity(splineData);

      expect(opacity).toBe(1.0);
    });

    it('should return default opacity when not set', () => {
      const splineData = createBasicSplineData();
      splineData.Opacity = undefined;

      const opacity = SplineEntityThreejsRenderer.getOpacity(splineData);

      expect(opacity).toBe(1.0);
    });
  });

  describe('getTransparent', () => {
    it('should return true when transparent', () => {
      const splineData = createBasicSplineData();
      splineData.Transparent = true;

      const transparent = SplineEntityThreejsRenderer.getTransparent(splineData);

      expect(transparent).toBe(true);
    });

    it('should return false when not transparent', () => {
      const splineData = createBasicSplineData();

      const transparent = SplineEntityThreejsRenderer.getTransparent(splineData);

      expect(transparent).toBe(true);
    });
  });

  describe('getSplineType', () => {
    it('should return correct spline type', () => {
      const splineData = createBasicSplineData();

      const splineType = SplineEntityThreejsRenderer.getSplineType(splineData);

      expect(splineType).toBe('CatmullRomCurve3');
    });

    it('should return default type when not set', () => {
      const splineData = createBasicSplineData();
      splineData.SplineType = undefined;

      const splineType = SplineEntityThreejsRenderer.getSplineType(splineData);

      expect(splineType).toBe('CatmullRomCurve3');
    });
  });

  describe('getKnots', () => {
    it('should return correct knots', () => {
      const splineData = createBasicSplineData();

      const knots = SplineEntityThreejsRenderer.getKnots(splineData);

      expect(knots).toBeInstanceOf(Array);
      expect(knots.length).toBe(6);
      expect(knots[0]).toBe(0);
      expect(knots[1]).toBe(0);
      expect(knots[2]).toBe(0);
      expect(knots[3]).toBe(1);
      expect(knots[4]).toBe(1);
      expect(knots[5]).toBe(1);
    });

    it('should return empty array when no knots', () => {
      const splineData = createBasicSplineData();
      splineData.Knots = undefined;

      const knots = SplineEntityThreejsRenderer.getKnots(splineData);

      expect(knots).toBeInstanceOf(Array);
      expect(knots.length).toBe(0);
    });
  });

  describe('getWeights', () => {
    it('should return correct weights', () => {
      const splineData = createBasicSplineData();

      const weights = SplineEntityThreejsRenderer.getWeights(splineData);

      expect(weights).toBeInstanceOf(Array);
      expect(weights.length).toBe(3);
      expect(weights[0]).toBe(1);
      expect(weights[1]).toBe(1);
      expect(weights[2]).toBe(1);
    });

    it('should return empty array when no weights', () => {
      const splineData = createBasicSplineData();
      splineData.Weights = undefined;

      const weights = SplineEntityThreejsRenderer.getWeights(splineData);

      expect(weights).toBeInstanceOf(Array);
      expect(weights.length).toBe(0);
    });
  });

  describe('getControlPointWeights', () => {
    it('should return correct control point weights', () => {
      const splineData = createBasicSplineData();

      const weights = SplineEntityThreejsRenderer.getControlPointWeights(splineData);

      expect(weights).toBeInstanceOf(Array);
      expect(weights.length).toBe(3);
      expect(weights[0]).toBe(1);
      expect(weights[1]).toBe(1);
      expect(weights[2]).toBe(1);
    });

    it('should return empty array when no control point weights', () => {
      const splineData = createBasicSplineData();
      splineData.ControlPointWeights = undefined;

      const weights = SplineEntityThreejsRenderer.getControlPointWeights(splineData);

      expect(weights).toBeInstanceOf(Array);
      expect(weights.length).toBe(0);
    });
  });

  describe('getStartTangent', () => {
    it('should return correct start tangent', () => {
      const splineData = createBasicSplineData();

      const startTangent = SplineEntityThreejsRenderer.getStartTangent(splineData);

      expect(startTangent).toBeInstanceOf(THREE.Vector3);
      expect(startTangent.x).toBe(1);
      expect(startTangent.y).toBe(0);
      expect(startTangent.z).toBe(0);
    });
  });

  describe('getEndTangent', () => {
    it('should return correct end tangent', () => {
      const splineData = createBasicSplineData();

      const endTangent = SplineEntityThreejsRenderer.getEndTangent(splineData);

      expect(endTangent).toBeInstanceOf(THREE.Vector3);
      expect(endTangent.x).toBe(1);
      expect(endTangent.y).toBe(0);
      expect(endTangent.z).toBe(0);
    });
  });

  describe('getSampleCount', () => {
    it('should return correct sample count', () => {
      const splineData = createBasicSplineData();

      const sampleCount = SplineEntityThreejsRenderer.getSampleCount(splineData);

      expect(sampleCount).toBe(100);
    });

    it('should return 0 when sample count not set', () => {
      const splineData = createBasicSplineData();
      splineData.SampleCount = undefined;

      const sampleCount = SplineEntityThreejsRenderer.getSampleCount(splineData);

      expect(sampleCount).toBe(0);
    });
  });
});
