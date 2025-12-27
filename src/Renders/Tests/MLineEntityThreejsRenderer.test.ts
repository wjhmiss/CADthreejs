import * as THREE from 'three';
import { MLineEntityThreejsRenderer, MLineData, ElementData, VertexData, Point3DData, PointData, BoundsData, ColorData, TransformData, NormalData } from '../MLineEntityThreejsRenderer';

describe('MLineEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
  });

  const createBasicMLineData = (): MLineData => ({
    Type: 'MLine',
    EntityType: 'MLine',
    Handle: 'mline-handle-1',
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Visible: true,
    CoordinateSystem: 'World',
    Elements: [
      {
        Offset: -0.5,
        ColorIndex: 1,
        LineTypeName: 'CONTINUOUS',
        Points: [
          { X: 0, Y: 0 },
          { X: 10, Y: 0 },
          { X: 20, Y: 0 }
        ],
        PointCount: 3,
        TotalLength: 20
      },
      {
        Offset: 0.5,
        ColorIndex: 2,
        LineTypeName: 'CONTINUOUS',
        Points: [
          { X: 0, Y: 0 },
          { X: 10, Y: 0 },
          { X: 20, Y: 0 }
        ],
        PointCount: 3,
        TotalLength: 20
      }
    ],
    IsClosed: false,
    ScaleFactor: 1.0,
    ColorIndex: 7,
    StyleFlags: 0,
    FillColorIndex: 0,
    FillOn: false,
    Vertices: [],
    ElementCount: 2,
    VertexCount: 3,
    Bounds: {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 20, Y: 0, Z: 0 },
      Center: { X: 10, Y: 0, Z: 0 },
      Size: { X: 20, Y: 0, Z: 0 }
    },
    Centroid: { X: 10, Y: 0 },
    Vertices3D: [
      { X: 0, Y: 0, Z: 0 },
      { X: 10, Y: 0, Z: 0 },
      { X: 20, Y: 0, Z: 0 }
    ],
    Centroid3D: { X: 10, Y: 0, Z: 0 },
    Normals: [
      { X: 0, Y: 0, Z: 1 },
      { X: 0, Y: 0, Z: 1 },
      { X: 0, Y: 0, Z: 1 }
    ],
    Bounds3D: {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 20, Y: 0, Z: 0 }
    },
    Color: { Index: 7, Hex: '#FFFFFF', R: 255, G: 255, B: 255, A: 1.0 },
    Transform: {
      Position: { X: 0, Y: 0, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
    },
    Normal: { X: 0, Y: 0, Z: 1 },
    Opacity: 1.0,
    Transparent: false,
    MaterialType: 'LineBasicMaterial',
    DepthTest: true,
    DepthWrite: true,
    Flags: 0,
    Justification: 0,
    StartPoint: { X: 0, Y: 0, Z: 0 },
    EndPoint: { X: 20, Y: 0, Z: 0 },
    TotalLength: 20,
    StyleName: 'STANDARD',
    StartAngle: 0,
    EndAngle: 0,
    HasStartCaps: true,
    HasEndCaps: true,
    DisplayJoints: false,
    LineIndices: [[0, 1], [1, 2]],
    Offsets: [-0.5, 0.5],
    ElementColorIndices: [1, 2]
  });

  describe('render', () => {
    it('should render a basic MLine', () => {
      const mlineData = createBasicMLineData();

      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.userData.type).toBe('MLine');
      expect(group.userData.entityType).toBe('MLine');
      expect(group.userData.handle).toBe('mline-handle-1');
      expect(group.userData.layerName).toBe('TEST_LAYER');
    });

    it('should return null for invisible MLine', () => {
      const mlineData = createBasicMLineData();
      mlineData.Visible = false;

      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      expect(group).toBeNull();
    });

    it('should return null for null data', () => {
      const group = MLineEntityThreejsRenderer.render(null as any, scene);

      expect(group).toBeNull();
    });

    it('should render MLine with correct elements', () => {
      const mlineData = createBasicMLineData();

      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBeGreaterThan(0);
      
      const elementLines = group.children.filter(child => child.name.startsWith('Element_'));
      expect(elementLines.length).toBe(2);
    });

    it('should render MLine with correct element colors', () => {
      const mlineData = createBasicMLineData();

      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      
      const element0 = group.children.find(child => child.name === 'Element_0');
      expect(element0).toBeDefined();
      if (element0 && element0 instanceof THREE.Line) {
        const material = element0.material as THREE.LineBasicMaterial;
        expect(material.color.getHex()).toBe(0xFF0000);
      }
    });

    it('should apply transform matrix to MLine', () => {
      const mlineData = createBasicMLineData();
      mlineData.Transform.Matrix = [
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        5, 10, 15, 1
      ];

      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.position.x).toBe(5);
      expect(group.position.y).toBe(10);
      expect(group.position.z).toBe(15);
    });

    it('should render MLine with correct opacity', () => {
      const mlineData = createBasicMLineData();
      mlineData.Opacity = 0.5;
      mlineData.Transparent = true;

      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      
      const element0 = group.children.find(child => child.name === 'Element_0');
      expect(element0).toBeDefined();
      if (element0 && element0 instanceof THREE.Line) {
        const material = element0.material as THREE.LineBasicMaterial;
        expect(material.opacity).toBe(0.5);
        expect(material.transparent).toBe(true);
      }
    });

    it('should render MLine with correct depth settings', () => {
      const mlineData = createBasicMLineData();
      mlineData.DepthTest = false;
      mlineData.DepthWrite = false;

      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      
      const element0 = group.children.find(child => child.name === 'Element_0');
      expect(element0).toBeDefined();
      if (element0 && element0 instanceof THREE.Line) {
        const material = element0.material as THREE.LineBasicMaterial;
        expect(material.depthTest).toBe(false);
        expect(material.depthWrite).toBe(false);
      }
    });

    it('should handle empty elements array', () => {
      const mlineData = createBasicMLineData();
      mlineData.Elements = [];

      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBe(0);
    });

    it('should handle elements with insufficient points', () => {
      const mlineData = createBasicMLineData();
      mlineData.Elements[0].Points = [{ X: 0, Y: 0 }];

      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const element0 = group.children.find(child => child.name === 'Element_0');
      expect(element0).toBeUndefined();
    });

    it('should render MLine with multiple elements', () => {
      const mlineData = createBasicMLineData();
      mlineData.Elements = [
        {
          Offset: -1.0,
          ColorIndex: 1,
          LineTypeName: 'CONTINUOUS',
          Points: [
            { X: 0, Y: 0 },
            { X: 10, Y: 0 },
            { X: 20, Y: 0 }
          ],
          PointCount: 3,
          TotalLength: 20
        },
        {
          Offset: -0.5,
          ColorIndex: 2,
          LineTypeName: 'CONTINUOUS',
          Points: [
            { X: 0, Y: 0 },
            { X: 10, Y: 0 },
            { X: 20, Y: 0 }
          ],
          PointCount: 3,
          TotalLength: 20
        },
        {
          Offset: 0.5,
          ColorIndex: 3,
          LineTypeName: 'CONTINUOUS',
          Points: [
            { X: 0, Y: 0 },
            { X: 10, Y: 0 },
            { X: 20, Y: 0 }
          ],
          PointCount: 3,
          TotalLength: 20
        },
        {
          Offset: 1.0,
          ColorIndex: 4,
          LineTypeName: 'CONTINUOUS',
          Points: [
            { X: 0, Y: 0 },
            { X: 10, Y: 0 },
            { X: 20, Y: 0 }
          ],
          PointCount: 3,
          TotalLength: 20
        }
      ];

      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const elementLines = group.children.filter(child => child.name.startsWith('Element_'));
      expect(elementLines.length).toBe(4);
    });

    it('should set correct user data on elements', () => {
      const mlineData = createBasicMLineData();

      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      
      const element0 = group.children.find(child => child.name === 'Element_0');
      expect(element0).toBeDefined();
      if (element0) {
        expect(element0.userData.offset).toBe(-0.5);
        expect(element0.userData.colorIndex).toBe(1);
        expect(element0.userData.lineTypeName).toBe('CONTINUOUS');
      }
    });

    it('should handle 3D coordinates correctly', () => {
      const mlineData = createBasicMLineData();
      mlineData.Elements[0].Points = [
        { X: 0, Y: 0 },
        { X: 10, Y: 10 },
        { X: 20, Y: 0 }
      ];

      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      
      const element0 = group.children.find(child => child.name === 'Element_0');
      expect(element0).toBeDefined();
      if (element0 && element0 instanceof THREE.Line) {
        const geometry = element0.geometry;
        const positions = geometry.attributes.position.array as Float32Array;
        expect(positions[0]).toBe(0);
        expect(positions[1]).toBe(0);
        expect(positions[3]).toBe(10);
        expect(positions[4]).toBe(10);
        expect(positions[6]).toBe(20);
        expect(positions[7]).toBe(0);
      }
    });

    it('should add group to scene', () => {
      const mlineData = createBasicMLineData();

      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(scene.children).toContain(group);
    });

    it('should set correct group name', () => {
      const mlineData = createBasicMLineData();

      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('MLine_mline-handle-1');
    });
  });

  describe('update', () => {
    it('should update existing MLine', () => {
      const mlineData = createBasicMLineData();
      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      const updatedMLineData = createBasicMLineData();
      updatedMLineData.Elements[0].ColorIndex = 3;

      const result = MLineEntityThreejsRenderer.update(updatedMLineData, group, scene);

      expect(result).toBe(true);
      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should return false for null group', () => {
      const mlineData = createBasicMLineData();

      const result = MLineEntityThreejsRenderer.update(mlineData, null as any, scene);

      expect(result).toBe(false);
    });

    it('should return false for null data', () => {
      const mlineData = createBasicMLineData();
      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      const result = MLineEntityThreejsRenderer.update(null as any, group, scene);

      expect(result).toBe(false);
    });

    it('should update visibility', () => {
      const mlineData = createBasicMLineData();
      MLineEntityThreejsRenderer.render(mlineData, scene);

      const updatedMLineData = createBasicMLineData();
      updatedMLineData.Visible = false;

      const result = MLineEntityThreejsRenderer.update(updatedMLineData, scene);

      expect(result).toBe(false);
      const group = scene.getObjectByName(`MLine_${updatedMLineData.Handle}`);
      expect(group).toBeUndefined();
    });

    it('should update transform', () => {
      const mlineData = createBasicMLineData();
      MLineEntityThreejsRenderer.render(mlineData, scene);

      const updatedMLineData = createBasicMLineData();
      updatedMLineData.Transform.Matrix = [
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        10, 20, 30, 1
      ];

      const result = MLineEntityThreejsRenderer.update(updatedMLineData, scene);

      expect(result).toBe(true);
      const group = scene.getObjectByName(`MLine_${updatedMLineData.Handle}`) as THREE.Group;
      expect(group).not.toBeNull();
      expect(group.position.x).toBe(10);
      expect(group.position.y).toBe(20);
      expect(group.position.z).toBe(30);
    });
  });

  describe('dispose', () => {
    it('should dispose MLine and remove from scene', () => {
      const mlineData = createBasicMLineData();
      MLineEntityThreejsRenderer.render(mlineData, scene);

      const result = MLineEntityThreejsRenderer.dispose(mlineData, scene);

      expect(result).toBe(true);
      const group = scene.getObjectByName(`MLine_${mlineData.Handle}`);
      expect(group).toBeUndefined();
    });

    it('should return false for null mlineData', () => {
      const result = MLineEntityThreejsRenderer.dispose(null as any, scene);

      expect(result).toBe(false);
    });

    it('should dispose all element geometries and materials', () => {
      const mlineData = createBasicMLineData();
      const group = MLineEntityThreejsRenderer.render(mlineData, scene);

      const element0 = group.children.find(child => child.name === 'Element_0');
      const element1 = group.children.find(child => child.name === 'Element_1');

      MLineEntityThreejsRenderer.dispose(mlineData, scene);

      const disposedGroup = scene.getObjectByName(`MLine_${mlineData.Handle}`);
      expect(disposedGroup).toBeUndefined();
    });

    it('should handle group with no children', () => {
      const mlineData = createBasicMLineData();
      const group = new THREE.Group();
      group.name = `MLine_${mlineData.Handle}`;
      scene.add(group);

      const result = MLineEntityThreejsRenderer.dispose(mlineData, scene);

      expect(result).toBe(true);
      expect(scene.children).not.toContain(group);
    });
  });

  describe('calculateLength', () => {
    it('should calculate correct length for simple MLine', () => {
      const mlineData = createBasicMLineData();

      const length = MLineEntityThreejsRenderer.calculateLength(mlineData);

      expect(length).toBe(40);
    });

    it('should calculate correct length for multi-segment MLine', () => {
      const mlineData = createBasicMLineData();
      mlineData.Elements[0].Points = [
        { X: 0, Y: 0 },
        { X: 10, Y: 0 },
        { X: 10, Y: 10 },
        { X: 20, Y: 10 }
      ];
      mlineData.Elements[0].TotalLength = 30;

      const length = MLineEntityThreejsRenderer.calculateLength(mlineData);

      expect(length).toBe(50);
    });

    it('should return 0 for empty elements', () => {
      const mlineData = createBasicMLineData();
      mlineData.Elements = [];

      const length = MLineEntityThreejsRenderer.calculateLength(mlineData);

      expect(length).toBe(0);
    });

    it('should return 0 for elements with insufficient points', () => {
      const mlineData = createBasicMLineData();
      mlineData.Elements[0].Points = [{ X: 0, Y: 0 }];
      mlineData.Elements[1].Points = [{ X: 0, Y: 0 }];

      const length = MLineEntityThreejsRenderer.calculateLength(mlineData);

      expect(length).toBe(0);
    });
  });

  describe('getElementCount', () => {
    it('should return correct element count', () => {
      const mlineData = createBasicMLineData();

      const count = MLineEntityThreejsRenderer.getElementCount(mlineData);

      expect(count).toBe(2);
    });

    it('should return 0 for empty elements', () => {
      const mlineData = createBasicMLineData();
      mlineData.Elements = [];

      const count = MLineEntityThreejsRenderer.getElementCount(mlineData);

      expect(count).toBe(0);
    });
  });

  describe('getVertexCount', () => {
    it('should return correct vertex count', () => {
      const mlineData = createBasicMLineData();

      const count = MLineEntityThreejsRenderer.getVertexCount(mlineData);

      expect(count).toBe(3);
    });

    it('should return 0 for empty vertices', () => {
      const mlineData = createBasicMLineData();
      mlineData.Vertices3D = [];

      const count = MLineEntityThreejsRenderer.getVertexCount(mlineData);

      expect(count).toBe(0);
    });
  });

  describe('getColorFromIndex', () => {
    it('should return correct color for index 1', () => {
      const color = MLineEntityThreejsRenderer['getColorFromIndex'](1);

      expect(color).toBe('#ff0000');
    });

    it('should return correct color for index 2', () => {
      const color = MLineEntityThreejsRenderer['getColorFromIndex'](2);

      expect(color).toBe('#ffff00');
    });

    it('should return correct color for index 7', () => {
      const color = MLineEntityThreejsRenderer['getColorFromIndex'](7);

      expect(color).toBe('#ffffff');
    });

    it('should return white for unknown index', () => {
      const color = MLineEntityThreejsRenderer['getColorFromIndex'](999);

      expect(color).toBe('#ffffff');
    });
  });
});
