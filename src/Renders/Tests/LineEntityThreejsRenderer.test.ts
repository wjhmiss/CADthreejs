import * as THREE from 'three';
import { LineEntityThreejsRenderer, LineData, Point3DData, PointData, BoundsData, ColorData, TransformData } from '../LineEntityThreejsRenderer';

describe('LineEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
  });

  const createBasicLineData = (): LineData => ({
    Type: 'Line',
    EntityType: 'LINE',
    Handle: 'handle-1',
    LayerName: 'TEST_LAYER',
    Visible: true,
    CoordinateSystem: 'World',
    StartPointX: 0,
    StartPointY: 0,
    StartPointZ: 0,
    EndPointX: 10,
    EndPointY: 0,
    EndPointZ: 0,
    StartPoint3D: { X: 0, Y: 0, Z: 0 },
    EndPoint3D: { X: 10, Y: 0, Z: 0 },
    MidPoint3D: { X: 5, Y: 0, Z: 0 },
    Length: 10,
    Angle: 0,
    MidPointX: 5,
    MidPointY: 0,
    Thickness: 0,
    ColorIndex: 1,
    Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 },
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.25,
    LineTypeScale: 1.0,
    Normal: { X: 0, Y: 0, Z: 1 },
    Transform: {
      Position: { X: 0, Y: 0, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
    },
    Bounds: {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 10, Y: 0, Z: 0 },
      Center: { X: 5, Y: 0, Z: 0 },
      Size: { X: 10, Y: 0, Z: 0 }
    },
    Opacity: 1.0,
    Transparent: false,
    MaterialType: 'LineBasicMaterial',
    DepthTest: true,
    DepthWrite: true
  });

  describe('render', () => {
    it('should render a basic line', () => {
      const lineData = createBasicLineData();

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.userData.type).toBe('Line');
      expect(group.userData.entityType).toBe('LINE');
      expect(group.userData.handle).toBe('handle-1');
      expect(group.userData.layerName).toBe('TEST_LAYER');
    });

    it('should return null for invisible line', () => {
      const lineData = createBasicLineData();
      lineData.Visible = false;

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeNull();
    });

    it('should return null for null data', () => {
      const group = LineEntityThreejsRenderer.render(null as any, scene);

      expect(group).toBeNull();
    });

    it('should render line with correct vertices', () => {
      const lineData = createBasicLineData();
      lineData.StartPointX = 5;
      lineData.StartPointY = 10;
      lineData.StartPointZ = 0;
      lineData.EndPointX = 15;
      lineData.EndPointY = 20;
      lineData.EndPointZ = 0;
      lineData.StartPoint3D = { X: 5, Y: 10, Z: 0 };
      lineData.EndPoint3D = { X: 15, Y: 20, Z: 0 };
      lineData.Length = Math.sqrt(200);
      lineData.Angle = Math.PI / 4;
      lineData.MidPointX = 10;
      lineData.MidPointY = 15;
      lineData.Bounds = {
        Min: { X: 5, Y: 10, Z: 0 },
        Max: { X: 15, Y: 20, Z: 0 },
        Center: { X: 10, Y: 15, Z: 0 },
        Size: { X: 10, Y: 10, Z: 0 }
      };

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBeGreaterThan(0);
      const line = group.children[0] as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
    });

    it('should render line with correct color', () => {
      const lineData = createBasicLineData();
      lineData.Color = { Index: 5, Hex: '#0000FF', R: 0, G: 0, B: 255, A: 1.0 };
      lineData.ColorIndex = 5;

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('0000ff');
    });

    it('should render line with correct opacity', () => {
      const lineData = createBasicLineData();
      lineData.Opacity = 0.5;
      lineData.Transparent = true;

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should render line with correct line weight', () => {
      const lineData = createBasicLineData();
      lineData.LineWeight = 0.5;

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.linewidth).toBe(0.5);
    });

    it('should render line with transform matrix', () => {
      const lineData = createBasicLineData();
      lineData.Transform.Matrix = [2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 10, 10, 10, 1];

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const matrix = group.matrix;
      expect(matrix.elements[0]).toBe(2);
      expect(matrix.elements[5]).toBe(2);
      expect(matrix.elements[10]).toBe(2);
    });

    it('should render line with depth test enabled', () => {
      const lineData = createBasicLineData();
      lineData.DepthTest = true;
      lineData.DepthWrite = true;

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(true);
    });

    it('should render line with depth test disabled', () => {
      const lineData = createBasicLineData();
      lineData.DepthTest = false;
      lineData.DepthWrite = false;

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.depthTest).toBe(false);
      expect(material.depthWrite).toBe(false);
    });

    it('should render line with 3D coordinates', () => {
      const lineData = createBasicLineData();
      lineData.StartPointX = 0;
      lineData.StartPointY = 0;
      lineData.StartPointZ = 0;
      lineData.EndPointX = 10;
      lineData.EndPointY = 10;
      lineData.EndPointZ = 10;
      lineData.StartPoint3D = { X: 0, Y: 0, Z: 0 };
      lineData.EndPoint3D = { X: 10, Y: 10, Z: 10 };
      lineData.Length = Math.sqrt(300);
      lineData.Angle = Math.PI / 4;
      lineData.Bounds = {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 10, Y: 10, Z: 10 },
        Center: { X: 5, Y: 5, Z: 5 },
        Size: { X: 10, Y: 10, Z: 10 }
      };

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.children[0] as THREE.Line;
      const geometry = line.geometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(0);
      expect(positions[1]).toBe(0);
      expect(positions[2]).toBe(0);
      expect(positions[3]).toBe(10);
      expect(positions[4]).toBe(10);
      expect(positions[5]).toBe(10);
    });

    it('should render line with thickness', () => {
      const lineData = createBasicLineData();
      lineData.Thickness = 2.5;

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.children[0] as THREE.Line;
      expect(line.userData.thickness).toBe(2.5);
    });

    it('should render line with correct user data', () => {
      const lineData = createBasicLineData();

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.type).toBe('Line');
      expect(group.userData.entityType).toBe('LINE');
      expect(group.userData.handle).toBe('handle-1');
      expect(group.userData.layerName).toBe('TEST_LAYER');
      expect(group.userData.length).toBe(10);
      expect(group.userData.angle).toBe(0);
      expect(group.userData.thickness).toBe(0);
    });

    it('should render line with bounds in user data', () => {
      const lineData = createBasicLineData();
      lineData.Bounds = {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 10, Y: 0, Z: 0 },
        Center: { X: 5, Y: 0, Z: 0 },
        Size: { X: 10, Y: 0, Z: 0 }
      };

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.bounds).toBeDefined();
      expect(group.userData.bounds.Min).toBeDefined();
      expect(group.userData.bounds.Max).toBeDefined();
    });

    it('should render line with transform in user data', () => {
      const lineData = createBasicLineData();

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.transform).toBeDefined();
      expect(group.userData.transform.Matrix).toBeDefined();
    });

    it('should add line to scene', () => {
      const lineData = createBasicLineData();

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(scene.children).toContain(group);
    });

    it('should set correct group name', () => {
      const lineData = createBasicLineData();

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group.name).toBe('Line_handle-1');
    });

    it('should render line with zero length', () => {
      const lineData = createBasicLineData();
      lineData.StartPointX = 5;
      lineData.StartPointY = 5;
      lineData.StartPointZ = 0;
      lineData.EndPointX = 5;
      lineData.EndPointY = 5;
      lineData.EndPointZ = 0;
      lineData.StartPoint3D = { X: 5, Y: 5, Z: 0 };
      lineData.EndPoint3D = { X: 5, Y: 5, Z: 0 };
      lineData.Length = 0;
      lineData.Angle = 0;
      lineData.MidPointX = 5;
      lineData.MidPointY = 5;
      lineData.Bounds = {
        Min: { X: 5, Y: 5, Z: 0 },
        Max: { X: 5, Y: 5, Z: 0 },
        Center: { X: 5, Y: 5, Z: 0 },
        Size: { X: 0, Y: 0, Z: 0 }
      };

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render line with negative coordinates', () => {
      const lineData = createBasicLineData();
      lineData.StartPointX = -10;
      lineData.StartPointY = -10;
      lineData.StartPointZ = 0;
      lineData.EndPointX = 10;
      lineData.EndPointY = 10;
      lineData.EndPointZ = 0;
      lineData.StartPoint3D = { X: -10, Y: -10, Z: 0 };
      lineData.EndPoint3D = { X: 10, Y: 10, Z: 0 };
      lineData.Length = Math.sqrt(800);
      lineData.Angle = Math.PI / 4;
      lineData.MidPointX = 0;
      lineData.MidPointY = 0;
      lineData.Bounds = {
        Min: { X: -10, Y: -10, Z: 0 },
        Max: { X: 10, Y: 10, Z: 0 },
        Center: { X: 0, Y: 0, Z: 0 },
        Size: { X: 20, Y: 20, Z: 0 }
      };

      const group = LineEntityThreejsRenderer.render(lineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.children[0] as THREE.Line;
      const geometry = line.geometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(-10);
      expect(positions[1]).toBe(-10);
      expect(positions[2]).toBe(0);
      expect(positions[3]).toBe(10);
      expect(positions[4]).toBe(10);
      expect(positions[5]).toBe(0);
    });
  });

  describe('update', () => {
    it('should update existing line', () => {
      const lineData = createBasicLineData();
      const group = LineEntityThreejsRenderer.render(lineData, scene);

      const updatedLineData = createBasicLineData();
      updatedLineData.EndPointX = 20;
      updatedLineData.EndPointY = 0;
      updatedLineData.EndPointZ = 0;
      updatedLineData.EndPoint3D = { X: 20, Y: 0, Z: 0 };
      updatedLineData.Length = 20;
      updatedLineData.Bounds = {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 20, Y: 0, Z: 0 },
        Center: { X: 10, Y: 0, Z: 0 },
        Size: { X: 20, Y: 0, Z: 0 }
      };

      const result = LineEntityThreejsRenderer.update(updatedLineData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent line', () => {
      const lineData = createBasicLineData();

      const result = LineEntityThreejsRenderer.update(lineData, scene);

      expect(result).toBe(false);
    });

    it('should update line color', () => {
      const lineData = createBasicLineData();
      LineEntityThreejsRenderer.render(lineData, scene);

      const updatedLineData = createBasicLineData();
      updatedLineData.Color = { Index: 2, Hex: '#FFFF00', R: 255, G: 255, B: 0, A: 1.0 };
      updatedLineData.ColorIndex = 2;

      LineEntityThreejsRenderer.update(updatedLineData, scene);

      const group = LineEntityThreejsRenderer.getLineGroup(lineData, scene);
      const line = group?.children[0] as THREE.Line;
      const material = line?.material as THREE.LineBasicMaterial;
      expect(material?.color.getHexString()).toBe('ffff00');
    });

    it('should update line opacity', () => {
      const lineData = createBasicLineData();
      LineEntityThreejsRenderer.render(lineData, scene);

      const updatedLineData = createBasicLineData();
      updatedLineData.Opacity = 0.3;
      updatedLineData.Transparent = true;

      LineEntityThreejsRenderer.update(updatedLineData, scene);

      const group = LineEntityThreejsRenderer.getLineGroup(lineData, scene);
      const line = group?.children[0] as THREE.Line;
      const material = line?.material as THREE.LineBasicMaterial;
      expect(material?.opacity).toBe(0.3);
      expect(material?.transparent).toBe(true);
    });
  });

  describe('getLineGroup', () => {
    it('should retrieve existing line group', () => {
      const lineData = createBasicLineData();
      const renderedGroup = LineEntityThreejsRenderer.render(lineData, scene);

      const group = LineEntityThreejsRenderer.getLineGroup(lineData, scene);

      expect(group).toBe(renderedGroup);
    });

    it('should return null for non-existent line', () => {
      const lineData = createBasicLineData();

      const group = LineEntityThreejsRenderer.getLineGroup(lineData, scene);

      expect(group).toBeNull();
    });
  });

  describe('setVisibility', () => {
    it('should set line visibility to true', () => {
      const lineData = createBasicLineData();
      LineEntityThreejsRenderer.render(lineData, scene);

      const result = LineEntityThreejsRenderer.setVisibility(lineData, scene, true);

      expect(result).toBe(true);
      const group = LineEntityThreejsRenderer.getLineGroup(lineData, scene);
      expect(group?.visible).toBe(true);
    });

    it('should set line visibility to false', () => {
      const lineData = createBasicLineData();
      LineEntityThreejsRenderer.render(lineData, scene);

      const result = LineEntityThreejsRenderer.setVisibility(lineData, scene, false);

      expect(result).toBe(true);
      const group = LineEntityThreejsRenderer.getLineGroup(lineData, scene);
      expect(group?.visible).toBe(false);
    });

    it('should return false for non-existent line', () => {
      const lineData = createBasicLineData();

      const result = LineEntityThreejsRenderer.setVisibility(lineData, scene, true);

      expect(result).toBe(false);
    });
  });

  describe('setColor', () => {
    it('should set line color', () => {
      const lineData = createBasicLineData();
      LineEntityThreejsRenderer.render(lineData, scene);

      const result = LineEntityThreejsRenderer.setColor(lineData, scene, '#00FF00');

      expect(result).toBe(true);
      const group = LineEntityThreejsRenderer.getLineGroup(lineData, scene);
      const line = group?.children[0] as THREE.Line;
      const material = line?.material as THREE.LineBasicMaterial;
      expect(material?.color.getHexString()).toBe('00ff00');
    });

    it('should return false for non-existent line', () => {
      const lineData = createBasicLineData();

      const result = LineEntityThreejsRenderer.setColor(lineData, scene, '#00FF00');

      expect(result).toBe(false);
    });
  });

  describe('setOpacity', () => {
    it('should set line opacity', () => {
      const lineData = createBasicLineData();
      LineEntityThreejsRenderer.render(lineData, scene);

      const result = LineEntityThreejsRenderer.setOpacity(lineData, scene, 0.7);

      expect(result).toBe(true);
      const group = LineEntityThreejsRenderer.getLineGroup(lineData, scene);
      const line = group?.children[0] as THREE.Line;
      const material = line?.material as THREE.LineBasicMaterial;
      expect(material?.opacity).toBe(0.7);
      expect(material?.transparent).toBe(true);
    });

    it('should return false for non-existent line', () => {
      const lineData = createBasicLineData();

      const result = LineEntityThreejsRenderer.setOpacity(lineData, scene, 0.7);

      expect(result).toBe(false);
    });
  });

  describe('setLineWidth', () => {
    it('should set line width', () => {
      const lineData = createBasicLineData();
      LineEntityThreejsRenderer.render(lineData, scene);

      const result = LineEntityThreejsRenderer.setLineWidth(lineData, scene, 2.0);

      expect(result).toBe(true);
      const group = LineEntityThreejsRenderer.getLineGroup(lineData, scene);
      const line = group?.children[0] as THREE.Line;
      const material = line?.material as THREE.LineBasicMaterial;
      expect(material?.linewidth).toBe(2.0);
    });

    it('should return false for non-existent line', () => {
      const lineData = createBasicLineData();

      const result = LineEntityThreejsRenderer.setLineWidth(lineData, scene, 2.0);

      expect(result).toBe(false);
    });
  });

  describe('isPointOnLine', () => {
    it('should return true for point on line', () => {
      const lineData = createBasicLineData();
      const point = { X: 5, Y: 0, Z: 0 };

      const result = LineEntityThreejsRenderer.isPointOnLine(lineData, point, 0.1);

      expect(result).toBe(true);
    });

    it('should return false for point not on line', () => {
      const lineData = createBasicLineData();
      const point = { X: 5, Y: 10, Z: 0 };

      const result = LineEntityThreejsRenderer.isPointOnLine(lineData, point, 0.1);

      expect(result).toBe(false);
    });

    it('should handle point outside line segment', () => {
      const lineData = createBasicLineData();
      const point = { X: 15, Y: 0, Z: 0 };

      const result = LineEntityThreejsRenderer.isPointOnLine(lineData, point, 0.1);

      expect(result).toBe(false);
    });
  });

  describe('getPointAtParameter', () => {
    it('should return start point at parameter 0', () => {
      const lineData = createBasicLineData();

      const point = LineEntityThreejsRenderer.getPointAtParameter(lineData, 0);

      expect(point).toBeDefined();
      expect(point?.X).toBe(0);
      expect(point?.Y).toBe(0);
      expect(point?.Z).toBe(0);
    });

    it('should return end point at parameter 1', () => {
      const lineData = createBasicLineData();

      const point = LineEntityThreejsRenderer.getPointAtParameter(lineData, 1);

      expect(point).toBeDefined();
      expect(point?.X).toBe(10);
      expect(point?.Y).toBe(0);
      expect(point?.Z).toBe(0);
    });

    it('should return mid point at parameter 0.5', () => {
      const lineData = createBasicLineData();

      const point = LineEntityThreejsRenderer.getPointAtParameter(lineData, 0.5);

      expect(point).toBeDefined();
      expect(point?.X).toBe(5);
      expect(point?.Y).toBe(0);
      expect(point?.Z).toBe(0);
    });

    it('should return null for invalid parameter', () => {
      const lineData = createBasicLineData();

      const point = LineEntityThreejsRenderer.getPointAtParameter(lineData, 1.5);

      expect(point).toBeNull();
    });
  });

  describe('getClosestPoint', () => {
    it('should return closest point on line', () => {
      const lineData = createBasicLineData();
      const point = { X: 5, Y: 5, Z: 0 };

      const closestPoint = LineEntityThreejsRenderer.getClosestPoint(lineData, point);

      expect(closestPoint).toBeDefined();
      expect(closestPoint?.X).toBe(5);
      expect(closestPoint?.Y).toBe(0);
      expect(closestPoint?.Z).toBe(0);
    });

    it('should handle point at start of line', () => {
      const lineData = createBasicLineData();
      const point = { X: 0, Y: 0, Z: 0 };

      const closestPoint = LineEntityThreejsRenderer.getClosestPoint(lineData, point);

      expect(closestPoint).toBeDefined();
      expect(closestPoint?.X).toBe(0);
      expect(closestPoint?.Y).toBe(0);
      expect(closestPoint?.Z).toBe(0);
    });

    it('should handle point at end of line', () => {
      const lineData = createBasicLineData();
      const point = { X: 10, Y: 0, Z: 0 };

      const closestPoint = LineEntityThreejsRenderer.getClosestPoint(lineData, point);

      expect(closestPoint).toBeDefined();
      expect(closestPoint?.X).toBe(10);
      expect(closestPoint?.Y).toBe(0);
      expect(closestPoint?.Z).toBe(0);
    });
  });

  describe('intersectLine', () => {
    it('should find intersection of two lines', () => {
      const lineData1 = createBasicLineData();
      const lineData2 = createBasicLineData();
      lineData2.StartPointX = 0;
      lineData2.StartPointY = -10;
      lineData2.StartPointZ = 0;
      lineData2.EndPointX = 0;
      lineData2.EndPointY = 10;
      lineData2.EndPointZ = 0;
      lineData2.StartPoint3D = { X: 0, Y: -10, Z: 0 };
      lineData2.EndPoint3D = { X: 0, Y: 10, Z: 0 };

      const intersection = LineEntityThreejsRenderer.intersectLine(lineData1, lineData2);

      expect(intersection).toBeDefined();
      expect(intersection?.X).toBe(0);
      expect(intersection?.Y).toBe(0);
      expect(intersection?.Z).toBe(0);
    });

    it('should return null for parallel lines', () => {
      const lineData1 = createBasicLineData();
      const lineData2 = createBasicLineData();
      lineData2.StartPointX = 0;
      lineData2.StartPointY = 10;
      lineData2.StartPointZ = 0;
      lineData2.EndPointX = 10;
      lineData2.EndPointY = 10;
      lineData2.EndPointZ = 0;
      lineData2.StartPoint3D = { X: 0, Y: 10, Z: 0 };
      lineData2.EndPoint3D = { X: 10, Y: 10, Z: 0 };

      const intersection = LineEntityThreejsRenderer.intersectLine(lineData1, lineData2);

      expect(intersection).toBeNull();
    });

    it('should return null for non-intersecting lines', () => {
      const lineData1 = createBasicLineData();
      const lineData2 = createBasicLineData();
      lineData2.StartPointX = 20;
      lineData2.StartPointY = 0;
      lineData2.StartPointZ = 0;
      lineData2.EndPointX = 30;
      lineData2.EndPointY = 0;
      lineData2.EndPointZ = 0;
      lineData2.StartPoint3D = { X: 20, Y: 0, Z: 0 };
      lineData2.EndPoint3D = { X: 30, Y: 0, Z: 0 };

      const intersection = LineEntityThreejsRenderer.intersectLine(lineData1, lineData2);

      expect(intersection).toBeNull();
    });
  });

  describe('dispose', () => {
    it('should dispose line resources', () => {
      const lineData = createBasicLineData();
      const group = LineEntityThreejsRenderer.render(lineData, scene);

      const result = LineEntityThreejsRenderer.dispose(lineData, scene);

      expect(result).toBe(true);
      expect(scene.children).not.toContain(group);
    });

    it('should return false for non-existent line', () => {
      const lineData = createBasicLineData();

      const result = LineEntityThreejsRenderer.dispose(lineData, scene);

      expect(result).toBe(false);
    });

    it('should handle null line data', () => {
      const result = LineEntityThreejsRenderer.dispose(null as any, scene);

      expect(result).toBe(false);
    });

    it('should remove group from scene', () => {
      const lineData = createBasicLineData();
      LineEntityThreejsRenderer.render(lineData, scene);

      LineEntityThreejsRenderer.dispose(lineData, scene);

      const group = LineEntityThreejsRenderer.getLineGroup(lineData, scene);
      expect(group).toBeNull();
    });

    it('should dispose geometry and material', () => {
      const lineData = createBasicLineData();
      const group = LineEntityThreejsRenderer.render(lineData, scene);
      const line = group.children[0] as THREE.Line;
      const geometry = line.geometry;
      const material = line.material;

      LineEntityThreejsRenderer.dispose(lineData, scene);

      expect(group.children.length).toBe(0);
    });
  });

  describe('calculateLength', () => {
    it('should calculate correct length', () => {
      const lineData = createBasicLineData();

      const length = LineEntityThreejsRenderer.calculateLength(lineData);

      expect(length).toBe(10);
    });

    it('should calculate length for diagonal line', () => {
      const lineData = createBasicLineData();
      lineData.StartPointX = 0;
      lineData.StartPointY = 0;
      lineData.StartPointZ = 0;
      lineData.EndPointX = 3;
      lineData.EndPointY = 4;
      lineData.EndPointZ = 0;
      lineData.StartPoint3D = { X: 0, Y: 0, Z: 0 };
      lineData.EndPoint3D = { X: 3, Y: 4, Z: 0 };

      const length = LineEntityThreejsRenderer.calculateLength(lineData);

      expect(length).toBe(5);
    });

    it('should calculate length for 3D line', () => {
      const lineData = createBasicLineData();
      lineData.StartPointX = 0;
      lineData.StartPointY = 0;
      lineData.StartPointZ = 0;
      lineData.EndPointX = 3;
      lineData.EndPointY = 4;
      lineData.EndPointZ = 12;
      lineData.StartPoint3D = { X: 0, Y: 0, Z: 0 };
      lineData.EndPoint3D = { X: 3, Y: 4, Z: 12 };

      const length = LineEntityThreejsRenderer.calculateLength(lineData);

      expect(length).toBe(13);
    });
  });

  describe('calculateAngle', () => {
    it('should calculate correct angle for horizontal line', () => {
      const lineData = createBasicLineData();

      const angle = LineEntityThreejsRenderer.calculateAngle(lineData);

      expect(angle).toBe(0);
    });

    it('should calculate correct angle for 45 degree line', () => {
      const lineData = createBasicLineData();
      lineData.StartPointX = 0;
      lineData.StartPointY = 0;
      lineData.StartPointZ = 0;
      lineData.EndPointX = 10;
      lineData.EndPointY = 10;
      lineData.EndPointZ = 0;
      lineData.StartPoint3D = { X: 0, Y: 0, Z: 0 };
      lineData.EndPoint3D = { X: 10, Y: 10, Z: 0 };

      const angle = LineEntityThreejsRenderer.calculateAngle(lineData);

      expect(angle).toBeCloseTo(Math.PI / 4);
    });

    it('should calculate correct angle for vertical line', () => {
      const lineData = createBasicLineData();
      lineData.StartPointX = 0;
      lineData.StartPointY = 0;
      lineData.StartPointZ = 0;
      lineData.EndPointX = 0;
      lineData.EndPointY = 10;
      lineData.EndPointZ = 0;
      lineData.StartPoint3D = { X: 0, Y: 0, Z: 0 };
      lineData.EndPoint3D = { X: 0, Y: 10, Z: 0 };

      const angle = LineEntityThreejsRenderer.calculateAngle(lineData);

      expect(angle).toBeCloseTo(Math.PI / 2);
    });
  });
});
