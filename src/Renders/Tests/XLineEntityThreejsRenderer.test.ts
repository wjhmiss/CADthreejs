import * as THREE from 'three';
import { XLineEntityThreejsRenderer, XLineData, Point3DData, ColorData, TransformData, NormalData, BoundsData, GeometryData, MaterialData } from '../XLineEntityThreejsRenderer';

describe('XLineEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    XLineEntityThreejsRenderer.clearCache();
  });

  const createBasicXLineData = (): XLineData => ({
    Handle: 'xline-handle-1',
    Visible: true,
    FirstPoint: { X: 0, Y: 0, Z: 0 },
    Direction: { X: 1, Y: 0, Z: 0 },
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.03,
    LinePoints: [
      { X: -1000, Y: 0, Z: 0 },
      { X: 1000, Y: 0, Z: 0 }
    ],
    SecondPoint: { X: 1000, Y: 0, Z: 0 },
    Length: 2000,
    Angle: 0,
    Transform: {
      Position: { X: 0, Y: 0, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
    },
    Geometry: {
      Type: 'LineGeometry',
      VertexCount: 2,
      HasColors: false,
      HasIndices: false
    },
    Material: {
      Type: 'LineBasicMaterial',
      Color: 16711680,
      Opacity: 1.0,
      Transparent: false,
      LineWidth: 1.0,
      Side: false
    },
    VertexPositions: [-1000, 0, 0, 1000, 0, 0],
    VertexColors: [],
    Indices: [],
    VertexCount: 2,
    Opacity: 1.0,
    Transparent: false,
    Normal: { X: 0, Y: 0, Z: 1 },
    Bounds: {
      Min: { X: -1000, Y: 0, Z: 0 },
      Max: { X: 1000, Y: 0, Z: 0 },
      Center: { X: 0, Y: 0, Z: 0 },
      Size: { X: 2000, Y: 0, Z: 0 }
    },
    Center: { X: 0, Y: 0, Z: 0 }
  });

  describe('render', () => {
    it('should render a basic XLine', () => {
      const xlineData = createBasicXLineData();

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.name).toBe('XLine_xline-handle-1');
    });

    it('should not add invisible XLine to scene', () => {
      const xlineData = createBasicXLineData();
      xlineData.Visible = false;

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.visible).toBe(false);
      expect(scene.children.length).toBe(0);
    });

    it('should render XLine with correct Line object', () => {
      const xlineData = createBasicXLineData();

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      expect(group.children.length).toBe(1);
      const line = group.children[0] as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
      expect(line.name).toBe('XLine');
    });

    it('should render XLine with correct material', () => {
      const xlineData = createBasicXLineData();

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      expect(line.material).toBeDefined();
      expect(line.material).toBeInstanceOf(THREE.LineBasicMaterial);
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('ff0000');
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(false);
    });

    it('should render XLine with correct geometry', () => {
      const xlineData = createBasicXLineData();

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      expect(line.geometry).toBeDefined();
      expect(line.geometry).toBeInstanceOf(THREE.BufferGeometry);
      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position).toBeDefined();
      expect(geometry.attributes.position.count).toBe(2);
    });

    it('should render XLine with correct vertex positions', () => {
      const xlineData = createBasicXLineData();

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(-1000);
      expect(positions[1]).toBe(0);
      expect(positions[2]).toBe(0);
      expect(positions[3]).toBe(1000);
      expect(positions[4]).toBe(0);
      expect(positions[5]).toBe(0);
    });

    it('should render XLine with correct color', () => {
      const xlineData = createBasicXLineData();
      xlineData.Material.Color = 0x00FF00;

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should render XLine with correct opacity', () => {
      const xlineData = createBasicXLineData();
      xlineData.Opacity = 0.5;
      xlineData.Transparent = true;

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should cache XLine by handle', () => {
      const xlineData = createBasicXLineData();

      XLineEntityThreejsRenderer.render(xlineData, scene);

      const cachedLine = (XLineEntityThreejsRenderer as any).xlineCache.get('xline-handle-1');
      expect(cachedLine).toBeDefined();
      expect(cachedLine).toBeInstanceOf(THREE.Line);
    });

    it('should render XLine with correct handle in userData', () => {
      const xlineData = createBasicXLineData();

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      expect(line.userData.handle).toBe('xline-handle-1');
    });

    it('should render XLine with default geometry when vertex positions not provided', () => {
      const xlineData = createBasicXLineData();
      xlineData.VertexPositions = [];

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(0);
      expect(positions[1]).toBe(0);
      expect(positions[2]).toBe(0);
    });

    it('should render XLine with vertex colors', () => {
      const xlineData = createBasicXLineData();
      xlineData.VertexColors = [1, 0, 0, 0, 1, 0];

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.color).toBeDefined();
      const colors = geometry.attributes.color.array as Float32Array;
      expect(colors[0]).toBeCloseTo(1.0);
      expect(colors[1]).toBeCloseTo(0.0);
      expect(colors[2]).toBeCloseTo(0.0);
    });

    it('should render XLine with indices', () => {
      const xlineData = createBasicXLineData();
      xlineData.Indices = [0, 1];

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.index).toBeDefined();
      const indices = geometry.index.array as Uint16Array;
      expect(indices[0]).toBe(0);
      expect(indices[1]).toBe(1);
    });

    it('should render XLine with negative coordinates', () => {
      const xlineData = createBasicXLineData();
      xlineData.VertexPositions = [-1000, -2000, -3000, -4000, -5000, -6000];

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(-1000);
      expect(positions[1]).toBe(-2000);
      expect(positions[2]).toBe(-3000);
    });

    it('should render XLine with very large coordinates', () => {
      const xlineData = createBasicXLineData();
      xlineData.VertexPositions = [1000000, 2000000, 3000000, 4000000, 5000000, 6000000];

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(1000000);
      expect(positions[1]).toBe(2000000);
      expect(positions[2]).toBe(3000000);
    });

    it('should render XLine with very small coordinates', () => {
      const xlineData = createBasicXLineData();
      xlineData.VertexPositions = [0.0001, 0.0002, 0.0003, 0.0004, 0.0005, 0.0006];

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBeCloseTo(0.0001, 7);
      expect(positions[1]).toBeCloseTo(0.0002, 7);
      expect(positions[2]).toBeCloseTo(0.0003, 7);
    });

    it('should render XLine with different direction', () => {
      const xlineData = createBasicXLineData();
      xlineData.Direction = { X: 0.707, Y: 0.707, Z: 0 };

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render XLine with different first point', () => {
      const xlineData = createBasicXLineData();
      xlineData.FirstPoint = { X: 100, Y: 200, Z: 50 };

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render XLine with different second point', () => {
      const xlineData = createBasicXLineData();
      xlineData.SecondPoint = { X: 500, Y: 600, Z: 100 };

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render XLine with different length', () => {
      const xlineData = createBasicXLineData();
      xlineData.Length = 5000;

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render XLine with different angle', () => {
      const xlineData = createBasicXLineData();
      xlineData.Angle = Math.PI / 4;

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render XLine with different line type name', () => {
      const xlineData = createBasicXLineData();
      xlineData.LineTypeName = 'DASHED';

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render XLine with different line weight', () => {
      const xlineData = createBasicXLineData();
      xlineData.LineWeight = 0.05;

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render XLine with different color index', () => {
      const xlineData = createBasicXLineData();
      xlineData.ColorIndex = 5;

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render XLine with 3D coordinates', () => {
      const xlineData = createBasicXLineData();
      xlineData.FirstPoint = { X: 0, Y: 0, Z: 100 };
      xlineData.Direction = { X: 1, Y: 0, Z: 0.5 };

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render XLine with line width', () => {
      const xlineData = createBasicXLineData();
      xlineData.Material.LineWidth = 2.0;

      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.linewidth).toBe(2.0);
    });
  });

  describe('renderMultiple', () => {
    it('should render multiple XLines', () => {
      const xlineDataArray = [
        createBasicXLineData(),
        createBasicXLineData()
      ];
      xlineDataArray[0].Handle = 'xline-1';
      xlineDataArray[1].Handle = 'xline-2';

      const group = XLineEntityThreejsRenderer.renderMultiple(xlineDataArray, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('MultipleXLines');
      expect(group.children.length).toBe(2);
    });

    it('should render multiple XLines with correct handles', () => {
      const xlineDataArray = [
        createBasicXLineData(),
        createBasicXLineData(),
        createBasicXLineData()
      ];
      xlineDataArray[0].Handle = 'xline-1';
      xlineDataArray[1].Handle = 'xline-2';
      xlineDataArray[2].Handle = 'xline-3';

      const group = XLineEntityThreejsRenderer.renderMultiple(xlineDataArray, scene);

      expect(group.children[0].name).toBe('XLine_xline-1');
      expect(group.children[1].name).toBe('XLine_xline-2');
      expect(group.children[2].name).toBe('XLine_xline-3');
    });

    it('should render empty array of XLines', () => {
      const xlineDataArray: XLineData[] = [];

      const group = XLineEntityThreejsRenderer.renderMultiple(xlineDataArray, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('MultipleXLines');
      expect(group.children.length).toBe(0);
    });

    it('should render single XLine using renderMultiple', () => {
      const xlineDataArray = [createBasicXLineData()];
      xlineDataArray[0].Handle = 'xline-1';

      const group = XLineEntityThreejsRenderer.renderMultiple(xlineDataArray, scene);

      expect(group.children.length).toBe(1);
      expect(group.children[0].name).toBe('XLine_xline-1');
    });
  });

  describe('update', () => {
    it('should update existing XLine', () => {
      const xlineData = createBasicXLineData();
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      const updatedXLineData = { ...xlineData };
      updatedXLineData.VertexPositions = [-500, 0, 0, 500, 0, 0];

      const result = XLineEntityThreejsRenderer.update(updatedXLineData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent XLine', () => {
      const xlineData = createBasicXLineData();

      const result = XLineEntityThreejsRenderer.update(xlineData, scene);

      expect(result).toBe(false);
    });

    it('should update XLine color', () => {
      const xlineData = createBasicXLineData();
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      const updatedXLineData = { ...xlineData };
      updatedXLineData.Material.Color = 0x00FF00;

      const result = XLineEntityThreejsRenderer.update(updatedXLineData, scene);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should update XLine opacity', () => {
      const xlineData = createBasicXLineData();
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      const updatedXLineData = { ...xlineData };
      updatedXLineData.Opacity = 0.5;
      updatedXLineData.Transparent = true;

      const result = XLineEntityThreejsRenderer.update(updatedXLineData, scene);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should update XLine vertex positions', () => {
      const xlineData = createBasicXLineData();
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      const updatedXLineData = { ...xlineData };
      updatedXLineData.VertexPositions = [-2000, 0, 0, 2000, 0, 0];

      const result = XLineEntityThreejsRenderer.update(updatedXLineData, scene);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(-2000);
      expect(positions[1]).toBe(0);
      expect(positions[2]).toBe(0);
    });

    it('should update XLine line width', () => {
      const xlineData = createBasicXLineData();
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      const updatedXLineData = { ...xlineData };
      updatedXLineData.Material.LineWidth = 3.0;

      const result = XLineEntityThreejsRenderer.update(updatedXLineData, scene);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.linewidth).toBe(3.0);
    });
  });

  describe('dispose', () => {
    it('should dispose XLine resources', () => {
      const xlineData = createBasicXLineData();
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      const result = XLineEntityThreejsRenderer.dispose(xlineData, scene);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(0);
    });

    it('should return false for non-existent XLine', () => {
      const xlineData = createBasicXLineData();

      const result = XLineEntityThreejsRenderer.dispose(xlineData, scene);

      expect(result).toBe(false);
    });

    it('should remove XLine from cache', () => {
      const xlineData = createBasicXLineData();
      XLineEntityThreejsRenderer.render(xlineData, scene);

      XLineEntityThreejsRenderer.dispose(xlineData, scene);

      const cachedLine = (XLineEntityThreejsRenderer as any).xlineCache.get('xline-handle-1');
      expect(cachedLine).toBeUndefined();
    });

    it('should dispose geometry and material', () => {
      const xlineData = createBasicXLineData();
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometrySpy = jest.spyOn(line.geometry, 'dispose');
      const materialSpy = jest.spyOn(line.material, 'dispose');

      XLineEntityThreejsRenderer.dispose(xlineData, scene);

      expect(geometrySpy).toHaveBeenCalled();
      expect(materialSpy).toHaveBeenCalled();
    });
  });

  describe('disposeMultiple', () => {
    it('should dispose multiple XLines', () => {
      const xlineDataArray = [
        createBasicXLineData(),
        createBasicXLineData()
      ];
      xlineDataArray[0].Handle = 'xline-1';
      xlineDataArray[1].Handle = 'xline-2';

      const group = XLineEntityThreejsRenderer.renderMultiple(xlineDataArray, scene);

      XLineEntityThreejsRenderer.disposeMultiple(group, scene);

      expect(scene.children.length).toBe(0);
    });

    it('should handle null group', () => {
      XLineEntityThreejsRenderer.disposeMultiple(null, scene);

      expect(scene.children.length).toBe(0);
    });

    it('should remove all XLines from cache', () => {
      const xlineDataArray = [
        createBasicXLineData(),
        createBasicXLineData()
      ];
      xlineDataArray[0].Handle = 'xline-1';
      xlineDataArray[1].Handle = 'xline-2';

      const group = XLineEntityThreejsRenderer.renderMultiple(xlineDataArray, scene);

      XLineEntityThreejsRenderer.disposeMultiple(group, scene);

      const cachedLine1 = (XLineEntityThreejsRenderer as any).xlineCache.get('xline-1');
      const cachedLine2 = (XLineEntityThreejsRenderer as any).xlineCache.get('xline-2');
      expect(cachedLine1).toBeUndefined();
      expect(cachedLine2).toBeUndefined();
    });
  });

  describe('setVisibility', () => {
    it('should set XLine visibility to true', () => {
      const xlineData = createBasicXLineData();
      xlineData.Visible = false;
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      const result = XLineEntityThreejsRenderer.setVisibility(xlineData, scene, true);

      expect(result).toBe(true);
      expect(group.visible).toBe(true);
      expect(scene.children.length).toBe(1);
    });

    it('should set XLine visibility to false', () => {
      const xlineData = createBasicXLineData();
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      const result = XLineEntityThreejsRenderer.setVisibility(xlineData, scene, false);

      expect(result).toBe(true);
      expect(group.visible).toBe(false);
      expect(scene.children.length).toBe(0);
    });

    it('should return false for non-existent XLine', () => {
      const xlineData = createBasicXLineData();

      const result = XLineEntityThreejsRenderer.setVisibility(xlineData, scene, true);

      expect(result).toBe(false);
    });
  });

  describe('setOpacity', () => {
    it('should set XLine opacity', () => {
      const xlineData = createBasicXLineData();
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      const result = XLineEntityThreejsRenderer.setOpacity(xlineData, 0.5);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(0.5);
    });

    it('should set XLine opacity to 1.0', () => {
      const xlineData = createBasicXLineData();
      xlineData.Opacity = 0.5;
      xlineData.Transparent = true;
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      const result = XLineEntityThreejsRenderer.setOpacity(xlineData, 1.0);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
    });

    it('should set XLine opacity to 0.0', () => {
      const xlineData = createBasicXLineData();
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      const result = XLineEntityThreejsRenderer.setOpacity(xlineData, 0.0);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(0.0);
      expect(material.transparent).toBe(true);
    });

    it('should return false for non-existent XLine', () => {
      const xlineData = createBasicXLineData();

      const result = XLineEntityThreejsRenderer.setOpacity(xlineData, 0.5);

      expect(result).toBe(false);
    });
  });

  describe('setLineWidth', () => {
    it('should set XLine line width', () => {
      const xlineData = createBasicXLineData();
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      const result = XLineEntityThreejsRenderer.setLineWidth(xlineData, 3.0);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.linewidth).toBe(3.0);
    });

    it('should set XLine line width to 0.0', () => {
      const xlineData = createBasicXLineData();
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);

      const result = XLineEntityThreejsRenderer.setLineWidth(xlineData, 0.0);

      expect(result).toBe(true);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.linewidth).toBe(0.0);
    });

    it('should return false for non-existent XLine', () => {
      const xlineData = createBasicXLineData();

      const result = XLineEntityThreejsRenderer.setLineWidth(xlineData, 3.0);

      expect(result).toBe(false);
    });
  });

  describe('getXLine', () => {
    it('should get cached XLine', () => {
      const xlineData = createBasicXLineData();
      XLineEntityThreejsRenderer.render(xlineData, scene);

      const line = XLineEntityThreejsRenderer.getXLine(xlineData);

      expect(line).toBeInstanceOf(THREE.Line);
      expect(line).not.toBeNull();
    });

    it('should return null for non-existent XLine', () => {
      const xlineData = createBasicXLineData();

      const line = XLineEntityThreejsRenderer.getXLine(xlineData);

      expect(line).toBeNull();
    });
  });

  describe('getFirstPoint', () => {
    it('should get first point as Vector3', () => {
      const xlineData = createBasicXLineData();
      xlineData.FirstPoint = { X: 100, Y: 200, Z: 50 };

      const point = XLineEntityThreejsRenderer.getFirstPoint(xlineData);

      expect(point).toBeInstanceOf(THREE.Vector3);
      expect(point.x).toBe(100);
      expect(point.y).toBe(200);
      expect(point.z).toBe(50);
    });

    it('should get first point with negative values', () => {
      const xlineData = createBasicXLineData();
      xlineData.FirstPoint = { X: -100, Y: -200, Z: -50 };

      const point = XLineEntityThreejsRenderer.getFirstPoint(xlineData);

      expect(point.x).toBe(-100);
      expect(point.y).toBe(-200);
      expect(point.z).toBe(-50);
    });
  });

  describe('getDirection', () => {
    it('should get direction as Vector3', () => {
      const xlineData = createBasicXLineData();
      xlineData.Direction = { X: 0.707, Y: 0.707, Z: 0 };

      const direction = XLineEntityThreejsRenderer.getDirection(xlineData);

      expect(direction).toBeInstanceOf(THREE.Vector3);
      expect(direction.x).toBeCloseTo(0.707, 3);
      expect(direction.y).toBeCloseTo(0.707, 3);
      expect(direction.z).toBe(0);
    });

    it('should get direction with negative values', () => {
      const xlineData = createBasicXLineData();
      xlineData.Direction = { X: -0.5, Y: -0.866, Z: 0 };

      const direction = XLineEntityThreejsRenderer.getDirection(xlineData);

      expect(direction.x).toBe(-0.5);
      expect(direction.y).toBe(-0.866);
      expect(direction.z).toBe(0);
    });
  });

  describe('getSecondPoint', () => {
    it('should get second point as Vector3', () => {
      const xlineData = createBasicXLineData();
      xlineData.SecondPoint = { X: 1000, Y: 0, Z: 0 };

      const point = XLineEntityThreejsRenderer.getSecondPoint(xlineData);

      expect(point).toBeInstanceOf(THREE.Vector3);
      expect(point.x).toBe(1000);
      expect(point.y).toBe(0);
      expect(point.z).toBe(0);
    });

    it('should get second point with negative values', () => {
      const xlineData = createBasicXLineData();
      xlineData.SecondPoint = { X: -1000, Y: -500, Z: -100 };

      const point = XLineEntityThreejsRenderer.getSecondPoint(xlineData);

      expect(point.x).toBe(-1000);
      expect(point.y).toBe(-500);
      expect(point.z).toBe(-100);
    });
  });

  describe('getLength', () => {
    it('should get XLine length', () => {
      const xlineData = createBasicXLineData();
      xlineData.Length = 5000;

      const length = XLineEntityThreejsRenderer.getLength(xlineData);

      expect(length).toBe(5000);
    });

    it('should get XLine length with decimal value', () => {
      const xlineData = createBasicXLineData();
      xlineData.Length = 1234.567;

      const length = XLineEntityThreejsRenderer.getLength(xlineData);

      expect(length).toBe(1234.567);
    });
  });

  describe('getAngle', () => {
    it('should get XLine angle', () => {
      const xlineData = createBasicXLineData();
      xlineData.Angle = Math.PI / 4;

      const angle = XLineEntityThreejsRenderer.getAngle(xlineData);

      expect(angle).toBe(Math.PI / 4);
    });

    it('should get XLine angle with negative value', () => {
      const xlineData = createBasicXLineData();
      xlineData.Angle = -Math.PI / 2;

      const angle = XLineEntityThreejsRenderer.getAngle(xlineData);

      expect(angle).toBe(-Math.PI / 2);
    });
  });

  describe('getLinePoints', () => {
    it('should get line points as Vector3 array', () => {
      const xlineData = createBasicXLineData();
      xlineData.LinePoints = [
        { X: -1000, Y: 0, Z: 0 },
        { X: 1000, Y: 0, Z: 0 }
      ];

      const points = XLineEntityThreejsRenderer.getLinePoints(xlineData);

      expect(points).toBeInstanceOf(Array);
      expect(points.length).toBe(2);
      expect(points[0]).toBeInstanceOf(THREE.Vector3);
      expect(points[0].x).toBe(-1000);
      expect(points[0].y).toBe(0);
      expect(points[0].z).toBe(0);
      expect(points[1].x).toBe(1000);
      expect(points[1].y).toBe(0);
      expect(points[1].z).toBe(0);
    });

    it('should get line points with 3D coordinates', () => {
      const xlineData = createBasicXLineData();
      xlineData.LinePoints = [
        { X: 0, Y: 0, Z: 100 },
        { X: 1000, Y: 0, Z: 200 }
      ];

      const points = XLineEntityThreejsRenderer.getLinePoints(xlineData);

      expect(points[0].z).toBe(100);
      expect(points[1].z).toBe(200);
    });
  });

  describe('getCenter', () => {
    it('should get center as Vector3', () => {
      const xlineData = createBasicXLineData();
      xlineData.Center = { X: 0, Y: 0, Z: 0 };

      const center = XLineEntityThreejsRenderer.getCenter(xlineData);

      expect(center).toBeInstanceOf(THREE.Vector3);
      expect(center.x).toBe(0);
      expect(center.y).toBe(0);
      expect(center.z).toBe(0);
    });

    it('should get center with non-zero values', () => {
      const xlineData = createBasicXLineData();
      xlineData.Center = { X: 500, Y: 250, Z: 100 };

      const center = XLineEntityThreejsRenderer.getCenter(xlineData);

      expect(center.x).toBe(500);
      expect(center.y).toBe(250);
      expect(center.z).toBe(100);
    });
  });

  describe('getBounds', () => {
    it('should get bounds as Box3', () => {
      const xlineData = createBasicXLineData();
      xlineData.Bounds = {
        Min: { X: -1000, Y: 0, Z: 0 },
        Max: { X: 1000, Y: 0, Z: 0 },
        Center: { X: 0, Y: 0, Z: 0 },
        Size: { X: 2000, Y: 0, Z: 0 }
      };

      const bounds = XLineEntityThreejsRenderer.getBounds(xlineData);

      expect(bounds).toBeInstanceOf(THREE.Box3);
      expect(bounds.min.x).toBe(-1000);
      expect(bounds.min.y).toBe(0);
      expect(bounds.min.z).toBe(0);
      expect(bounds.max.x).toBe(1000);
      expect(bounds.max.y).toBe(0);
      expect(bounds.max.z).toBe(0);
    });

    it('should get bounds with 3D extent', () => {
      const xlineData = createBasicXLineData();
      xlineData.Bounds = {
        Min: { X: -1000, Y: -500, Z: -100 },
        Max: { X: 1000, Y: 500, Z: 100 },
        Center: { X: 0, Y: 0, Z: 0 },
        Size: { X: 2000, Y: 1000, Z: 200 }
      };

      const bounds = XLineEntityThreejsRenderer.getBounds(xlineData);

      expect(bounds.min.x).toBe(-1000);
      expect(bounds.min.y).toBe(-500);
      expect(bounds.min.z).toBe(-100);
      expect(bounds.max.x).toBe(1000);
      expect(bounds.max.y).toBe(500);
      expect(bounds.max.z).toBe(100);
    });
  });

  describe('getNormal', () => {
    it('should get normal as Vector3', () => {
      const xlineData = createBasicXLineData();
      xlineData.Normal = { X: 0, Y: 0, Z: 1 };

      const normal = XLineEntityThreejsRenderer.getNormal(xlineData);

      expect(normal).toBeInstanceOf(THREE.Vector3);
      expect(normal.x).toBe(0);
      expect(normal.y).toBe(0);
      expect(normal.z).toBe(1);
    });

    it('should get normal with different values', () => {
      const xlineData = createBasicXLineData();
      xlineData.Normal = { X: 0.707, Y: 0, Z: 0.707 };

      const normal = XLineEntityThreejsRenderer.getNormal(xlineData);

      expect(normal.x).toBeCloseTo(0.707, 3);
      expect(normal.y).toBe(0);
      expect(normal.z).toBeCloseTo(0.707, 3);
    });
  });

  describe('getColor', () => {
    it('should get color as Color', () => {
      const xlineData = createBasicXLineData();
      xlineData.Material.Color = 0xFF0000;

      const color = XLineEntityThreejsRenderer.getColor(xlineData);

      expect(color).toBeInstanceOf(THREE.Color);
      expect(color.getHexString()).toBe('ff0000');
    });

    it('should get color with different value', () => {
      const xlineData = createBasicXLineData();
      xlineData.Material.Color = 0x00FF00;

      const color = XLineEntityThreejsRenderer.getColor(xlineData);

      expect(color.getHexString()).toBe('00ff00');
    });
  });

  describe('clearCache', () => {
    it('should clear all cached XLines', () => {
      const xlineData1 = createBasicXLineData();
      xlineData1.Handle = 'xline-1';
      const xlineData2 = createBasicXLineData();
      xlineData2.Handle = 'xline-2';

      XLineEntityThreejsRenderer.render(xlineData1, scene);
      XLineEntityThreejsRenderer.render(xlineData2, scene);

      XLineEntityThreejsRenderer.clearCache();

      const cachedLine1 = (XLineEntityThreejsRenderer as any).xlineCache.get('xline-1');
      const cachedLine2 = (XLineEntityThreejsRenderer as any).xlineCache.get('xline-2');
      expect(cachedLine1).toBeUndefined();
      expect(cachedLine2).toBeUndefined();
    });

    it('should dispose all cached XLines', () => {
      const xlineData = createBasicXLineData();
      const group = XLineEntityThreejsRenderer.render(xlineData, scene);
      const line = group.children[0] as THREE.Line;

      const geometrySpy = jest.spyOn(line.geometry, 'dispose');
      const materialSpy = jest.spyOn(line.material, 'dispose');

      XLineEntityThreejsRenderer.clearCache();

      expect(geometrySpy).toHaveBeenCalled();
      expect(materialSpy).toHaveBeenCalled();
    });
  });

  describe('getTransform', () => {
    it('should get transform with default values', () => {
      const xlineData = createBasicXLineData();
      xlineData.Transform = undefined;

      const transform = XLineEntityThreejsRenderer.getTransform(xlineData);

      expect(transform.ScaleX).toBe(1.0);
      expect(transform.ScaleY).toBe(1.0);
      expect(transform.ScaleZ).toBe(1.0);
      expect(transform.RotationX).toBe(0);
      expect(transform.RotationY).toBe(0);
      expect(transform.RotationZ).toBe(xlineData.Angle);
      expect(transform.TranslateX).toBe(xlineData.Center.X);
      expect(transform.TranslateY).toBe(xlineData.Center.Y);
      expect(transform.TranslateZ).toBe(xlineData.Center.Z);
    });

    it('should get transform with custom values', () => {
      const xlineData = createBasicXLineData();
      xlineData.Transform = {
        Position: { X: 100, Y: 200, Z: 50 },
        Rotation: { X: 0.1, Y: 0.2, Z: 0.3 },
        Scale: { X: 2, Y: 2, Z: 2 },
        Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
      };

      const transform = XLineEntityThreejsRenderer.getTransform(xlineData);

      expect(transform.ScaleX).toBe(2);
      expect(transform.ScaleY).toBe(2);
      expect(transform.ScaleZ).toBe(2);
      expect(transform.RotationX).toBe(0.1);
      expect(transform.RotationY).toBe(0.2);
      expect(transform.RotationZ).toBe(0.3);
      expect(transform.TranslateX).toBe(100);
      expect(transform.TranslateY).toBe(200);
      expect(transform.TranslateZ).toBe(50);
    });
  });

  describe('getGeometry', () => {
    it('should get geometry data', () => {
      const xlineData = createBasicXLineData();
      xlineData.Geometry = {
        Type: 'LineGeometry',
        VertexCount: 2,
        HasColors: false,
        HasIndices: false
      };

      const geometry = XLineEntityThreejsRenderer.getGeometry(xlineData);

      expect(geometry.Type).toBe('LineGeometry');
      expect(geometry.VertexCount).toBe(2);
      expect(geometry.HasColors).toBe(false);
      expect(geometry.HasIndices).toBe(false);
    });

    it('should get geometry data with colors', () => {
      const xlineData = createBasicXLineData();
      xlineData.Geometry = {
        Type: 'LineGeometry',
        VertexCount: 2,
        HasColors: true,
        HasIndices: false
      };

      const geometry = XLineEntityThreejsRenderer.getGeometry(xlineData);

      expect(geometry.HasColors).toBe(true);
    });
  });

  describe('getMaterial', () => {
    it('should get material data', () => {
      const xlineData = createBasicXLineData();
      xlineData.Material = {
        Type: 'LineBasicMaterial',
        Color: 0xFF0000,
        Opacity: 1.0,
        Transparent: false,
        LineWidth: 1.0,
        Side: false
      };

      const material = XLineEntityThreejsRenderer.getMaterial(xlineData);

      expect(material.Type).toBe('LineBasicMaterial');
      expect(material.Color).toBe(0xFF0000);
      expect(material.Opacity).toBe(1.0);
      expect(material.Transparent).toBe(false);
      expect(material.LineWidth).toBe(1.0);
      expect(material.Side).toBe(false);
    });
  });

  describe('getHandle', () => {
    it('should get handle', () => {
      const xlineData = createBasicXLineData();
      xlineData.Handle = 'xline-123';

      const handle = XLineEntityThreejsRenderer.getHandle(xlineData);

      expect(handle).toBe('xline-123');
    });

    it('should return empty string for undefined handle', () => {
      const xlineData = createBasicXLineData();
      xlineData.Handle = undefined;

      const handle = XLineEntityThreejsRenderer.getHandle(xlineData);

      expect(handle).toBe('');
    });
  });

  describe('getVisible', () => {
    it('should return true for visible XLine', () => {
      const xlineData = createBasicXLineData();
      xlineData.Visible = true;

      const visible = XLineEntityThreejsRenderer.getVisible(xlineData);

      expect(visible).toBe(true);
    });

    it('should return false for invisible XLine', () => {
      const xlineData = createBasicXLineData();
      xlineData.Visible = false;

      const visible = XLineEntityThreejsRenderer.getVisible(xlineData);

      expect(visible).toBe(false);
    });

    it('should return true for undefined visibility', () => {
      const xlineData = createBasicXLineData();
      xlineData.Visible = undefined;

      const visible = XLineEntityThreejsRenderer.getVisible(xlineData);

      expect(visible).toBe(true);
    });
  });

  describe('getOpacity', () => {
    it('should get opacity', () => {
      const xlineData = createBasicXLineData();
      xlineData.Opacity = 0.5;

      const opacity = XLineEntityThreejsRenderer.getOpacity(xlineData);

      expect(opacity).toBe(0.5);
    });

    it('should return default opacity for undefined value', () => {
      const xlineData = createBasicXLineData();
      xlineData.Opacity = undefined;

      const opacity = XLineEntityThreejsRenderer.getOpacity(xlineData);

      expect(opacity).toBe(1.0);
    });
  });

  describe('getTransparent', () => {
    it('should return true for transparent XLine', () => {
      const xlineData = createBasicXLineData();
      xlineData.Transparent = true;

      const transparent = XLineEntityThreejsRenderer.getTransparent(xlineData);

      expect(transparent).toBe(true);
    });

    it('should return false for opaque XLine', () => {
      const xlineData = createBasicXLineData();
      xlineData.Transparent = false;

      const transparent = XLineEntityThreejsRenderer.getTransparent(xlineData);

      expect(transparent).toBe(false);
    });

    it('should return true for undefined transparency', () => {
      const xlineData = createBasicXLineData();
      xlineData.Transparent = undefined;

      const transparent = XLineEntityThreejsRenderer.getTransparent(xlineData);

      expect(transparent).toBe(true);
    });
  });

  describe('getLineTypeName', () => {
    it('should get line type name', () => {
      const xlineData = createBasicXLineData();
      xlineData.LineTypeName = 'DASHED';

      const lineTypeName = XLineEntityThreejsRenderer.getLineTypeName(xlineData);

      expect(lineTypeName).toBe('DASHED');
    });

    it('should return empty string for undefined line type name', () => {
      const xlineData = createBasicXLineData();
      xlineData.LineTypeName = undefined;

      const lineTypeName = XLineEntityThreejsRenderer.getLineTypeName(xlineData);

      expect(lineTypeName).toBe('');
    });
  });

  describe('getLineWeight', () => {
    it('should get line weight', () => {
      const xlineData = createBasicXLineData();
      xlineData.LineWeight = 0.05;

      const lineWeight = XLineEntityThreejsRenderer.getLineWeight(xlineData);

      expect(lineWeight).toBe(0.05);
    });

    it('should get line weight with decimal value', () => {
      const xlineData = createBasicXLineData();
      xlineData.LineWeight = 0.0125;

      const lineWeight = XLineEntityThreejsRenderer.getLineWeight(xlineData);

      expect(lineWeight).toBe(0.0125);
    });
  });

  describe('getColorIndex', () => {
    it('should get color index', () => {
      const xlineData = createBasicXLineData();
      xlineData.ColorIndex = 5;

      const colorIndex = XLineEntityThreejsRenderer.getColorIndex(xlineData);

      expect(colorIndex).toBe(5);
    });

    it('should get color index with large value', () => {
      const xlineData = createBasicXLineData();
      xlineData.ColorIndex = 255;

      const colorIndex = XLineEntityThreejsRenderer.getColorIndex(xlineData);

      expect(colorIndex).toBe(255);
    });
  });
});
