import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import * as THREE from 'three';
import { LwPolylineEntityThreejsRenderer, LwPolylineData, LwPolylineVertexData, Point3DData, PointData, ColorData, NormalData, TransformData, BoundsData } from '../LwPolylineEntityThreejsRenderer';

describe('LwPolylineEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
  });

  afterEach(() => {
    while (scene.children.length > 0) {
      const child = scene.children[0];
      if (child instanceof THREE.Group) {
        child.traverse((obj) => {
          if (obj instanceof THREE.Line) {
            if (obj.geometry) obj.geometry.dispose();
            if (obj.material) {
              if (Array.isArray(obj.material)) {
                obj.material.forEach((mat) => mat.dispose());
              } else {
                obj.material.dispose();
              }
            }
          }
        });
      }
      scene.remove(child);
    }
  });

  const createBasicLwPolylineData = (): LwPolylineData => ({
    Type: 'LwPolyline',
    EntityType: 'LwPolyline',
    Handle: 'handle-1',
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Visible: true,
    CoordinateSystem: 'World',
    Vertices: [
      { X: 0, Y: 0, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 0, Y: 0, Z: 0 } },
      { X: 10, Y: 0, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 10, Y: 0, Z: 0 } },
      { X: 10, Y: 10, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 10, Y: 10, Z: 0 } },
      { X: 0, Y: 10, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 0, Y: 10, Z: 0 } }
    ],
    IsClosed: true,
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.25,
    LineTypeScale: 1.0,
    TotalLength: 40,
    VertexCount: 4,
    Centroid: { X: 5, Y: 5 },
    Points: [
      { X: 0, Y: 0 },
      { X: 10, Y: 0 },
      { X: 10, Y: 10 },
      { X: 0, Y: 10 }
    ],
    Vertices3D: [
      { X: 0, Y: 0, Z: 0 },
      { X: 10, Y: 0, Z: 0 },
      { X: 10, Y: 10, Z: 0 },
      { X: 0, Y: 10, Z: 0 }
    ],
    Centroid3D: { X: 5, Y: 5, Z: 0 },
    Direction: [
      { X: 1, Y: 0, Z: 0 },
      { X: 0, Y: 1, Z: 0 },
      { X: -1, Y: 0, Z: 0 },
      { X: 0, Y: -1, Z: 0 }
    ],
    Bounds: {
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
    MaterialType: 'LineBasicMaterial',
    DepthTest: true,
    DepthWrite: true,
    Elevation: 0,
    ConstantWidth: 0,
    Thickness: 0
  });

  describe('render', () => {
    it('should render a basic lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();
      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('lwPolyline-handle-1');
      expect(group.children.length).toBeGreaterThan(0);
      expect(scene.children).toContain(group);
    });

    it('should return null for invisible lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Visible = false;

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeNull();
    });

    it('should return null for null data', () => {
      const group = LwPolylineEntityThreejsRenderer.render(null as any, scene);

      expect(group).toBeNull();
    });

    it('should return null for empty vertices', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Vertices = [];

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeNull();
    });

    it('should render lwPolyline with correct vertices', () => {
      const lwPolylineData = createBasicLwPolylineData();
      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.getObjectByName('lwPolyline-line') as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
      
      const positions = line.geometry.attributes.position.array as Float32Array;
      expect(positions.length).toBeGreaterThan(0);
    });

    it('should render lwPolyline with correct color', () => {
      const lwPolylineData = createBasicLwPolylineData();
      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.getObjectByName('lwPolyline-line') as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
      
      if (line.material instanceof THREE.LineBasicMaterial) {
        expect(line.material.color.getHex()).toBe(0xff0000);
      }
    });

    it('should render lwPolyline with correct opacity', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Opacity = 0.5;
      lwPolylineData.Transparent = true;

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.getObjectByName('lwPolyline-line') as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
      
      if (line.material instanceof THREE.LineBasicMaterial) {
        expect(line.material.opacity).toBe(0.5);
        expect(line.material.transparent).toBe(true);
      }
    });

    it('should render lwPolyline with correct line weight', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.ConstantWidth = 2.0;

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.getObjectByName('lwPolyline-line') as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
      
      if (line.material instanceof THREE.LineBasicMaterial) {
        expect((line.material as any).linewidth).toBe(2.0);
      }
    });

    it('should render lwPolyline with transform matrix', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Transform.Matrix = [2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 10, 10, 10, 1];

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const matrix = group.matrix;
      expect(matrix.elements[0]).toBe(2);
      expect(matrix.elements[5]).toBe(2);
      expect(matrix.elements[10]).toBe(2);
    });

    it('should render lwPolyline with depth test enabled', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.DepthTest = true;

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.getObjectByName('lwPolyline-line') as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
      
      if (line.material instanceof THREE.LineBasicMaterial) {
        expect(line.material.depthTest).toBe(true);
      }
    });

    it('should render lwPolyline with depth test disabled', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.DepthTest = false;

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.getObjectByName('lwPolyline-line') as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
      
      if (line.material instanceof THREE.LineBasicMaterial) {
        expect(line.material.depthTest).toBe(false);
      }
    });

    it('should render lwPolyline with 3D coordinates', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Vertices = [
        { X: 0, Y: 0, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 0, Y: 0, Z: 5 } },
        { X: 10, Y: 0, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 10, Y: 0, Z: 5 } },
        { X: 10, Y: 10, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 10, Y: 10, Z: 5 } },
        { X: 0, Y: 10, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 0, Y: 10, Z: 5 } }
      ];

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.getObjectByName('lwPolyline-line') as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
      
      const positions = line.geometry.attributes.position.array as Float32Array;
      expect(positions[2]).toBe(5);
    });

    it('should render lwPolyline with correct user data', () => {
      const lwPolylineData = createBasicLwPolylineData();

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.type).toBe('LwPolyline');
      expect(group.userData.entityType).toBe('LwPolyline');
      expect(group.userData.handle).toBe('handle-1');
      expect(group.userData.layerName).toBe('TEST_LAYER');
      expect(group.userData.visible).toBe(true);
    });

    it('should render lwPolyline with bounds in user data', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Bounds = {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 10, Y: 10, Z: 0 },
        Center: { X: 5, Y: 5, Z: 0 },
        Size: { X: 10, Y: 10, Z: 0 }
      };

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.bounds).toBeDefined();
      expect(group.userData.bounds.Min).toBeDefined();
      expect(group.userData.bounds.Max).toBeDefined();
    });

    it('should render lwPolyline with transform in user data', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Transform = {
        Position: { X: 0, Y: 0, Z: 0 },
        Rotation: { X: 0, Y: 0, Z: 0 },
        Scale: { X: 1, Y: 1, Z: 1 },
        Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
      };

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.transform).toBeDefined();
      expect(group.userData.transform.Matrix).toBeDefined();
    });

    it('should add lwPolyline to scene', () => {
      const lwPolylineData = createBasicLwPolylineData();
      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(scene.children).toContain(group);
    });

    it('should set correct group name', () => {
      const lwPolylineData = createBasicLwPolylineData();
      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group.name).toBe('lwPolyline-handle-1');
    });

    it('should render lwPolyline with zero length', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.IsClosed = false;
      lwPolylineData.Vertices = [
        { X: 0, Y: 0, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 0, Y: 0, Z: 0 } }
      ];

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeNull();
    });

    it('should render lwPolyline with negative coordinates', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Vertices = [
        { X: -10, Y: -10, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: -10, Y: -10, Z: 0 } },
        { X: 10, Y: -10, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 10, Y: -10, Z: 0 } },
        { X: 10, Y: 10, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 10, Y: 10, Z: 0 } },
        { X: -10, Y: 10, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: -10, Y: 10, Z: 0 } }
      ];

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render lwPolyline with bulge', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Vertices = [
        { X: 0, Y: 0, Bulge: 1, StartWidth: 0, EndWidth: 0, Location3D: { X: 0, Y: 0, Z: 0 } },
        { X: 10, Y: 0, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 10, Y: 0, Z: 0 } }
      ];

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.getObjectByName('lwPolyline-line') as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
    });

    it('should render lwPolyline with variable width', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Vertices = [
        { X: 0, Y: 0, Bulge: 0, StartWidth: 0.5, EndWidth: 1.0, Location3D: { X: 0, Y: 0, Z: 0 } },
        { X: 10, Y: 0, Bulge: 0, StartWidth: 1.0, EndWidth: 0.5, Location3D: { X: 10, Y: 0, Z: 0 } }
      ];

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render lwPolyline with elevation', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Elevation = 5;
      lwPolylineData.Vertices = [
        { X: 0, Y: 0, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 0, Y: 0, Z: 5 } },
        { X: 10, Y: 0, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 10, Y: 0, Z: 5 } },
        { X: 10, Y: 10, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 10, Y: 10, Z: 5 } },
        { X: 0, Y: 10, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 0, Y: 10, Z: 5 } }
      ];

      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      const line = group.getObjectByName('lwPolyline-line') as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
      
      const positions = line.geometry.attributes.position.array as Float32Array;
      expect(positions[2]).toBe(5);
    });
  });

  describe('update', () => {
    it('should update existing lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      lwPolylineData.Color.Hex = '#00FF00';
      const result = LwPolylineEntityThreejsRenderer.update(lwPolylineData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();
      const result = LwPolylineEntityThreejsRenderer.update(lwPolylineData, scene);

      expect(result).toBe(false);
    });

    it('should update lwPolyline color', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      lwPolylineData.Color.Hex = '#00FF00';
      LwPolylineEntityThreejsRenderer.update(lwPolylineData, scene);

      const group = LwPolylineEntityThreejsRenderer.getLwPolylineGroup(lwPolylineData, scene);
      const line = group?.getObjectByName('lwPolyline-line') as THREE.Line;
      
      if (line?.material instanceof THREE.LineBasicMaterial) {
        expect(line.material.color.getHex()).toBe(0x00ff00);
      }
    });

    it('should update lwPolyline opacity', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      lwPolylineData.Opacity = 0.3;
      LwPolylineEntityThreejsRenderer.update(lwPolylineData, scene);

      const group = LwPolylineEntityThreejsRenderer.getLwPolylineGroup(lwPolylineData, scene);
      const line = group?.getObjectByName('lwPolyline-line') as THREE.Line;
      
      if (line?.material instanceof THREE.LineBasicMaterial) {
        expect(line.material.opacity).toBe(0.3);
      }
    });
  });

  describe('getLwPolylineGroup', () => {
    it('should retrieve existing lwPolyline group', () => {
      const lwPolylineData = createBasicLwPolylineData();
      const renderedGroup = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const group = LwPolylineEntityThreejsRenderer.getLwPolylineGroup(lwPolylineData, scene);

      expect(group).toBe(renderedGroup);
    });

    it('should return null for non-existent lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();

      const group = LwPolylineEntityThreejsRenderer.getLwPolylineGroup(lwPolylineData, scene);

      expect(group).toBeNull();
    });
  });

  describe('setVisibility', () => {
    it('should set lwPolyline visibility to true', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const result = LwPolylineEntityThreejsRenderer.setVisibility(lwPolylineData, scene, true);

      expect(result).toBe(true);
      const group = LwPolylineEntityThreejsRenderer.getLwPolylineGroup(lwPolylineData, scene);
      expect(group?.visible).toBe(true);
    });

    it('should set lwPolyline visibility to false', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const result = LwPolylineEntityThreejsRenderer.setVisibility(lwPolylineData, scene, false);

      expect(result).toBe(true);
      const group = LwPolylineEntityThreejsRenderer.getLwPolylineGroup(lwPolylineData, scene);
      expect(group?.visible).toBe(false);
    });

    it('should return false for non-existent lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();

      const result = LwPolylineEntityThreejsRenderer.setVisibility(lwPolylineData, scene, true);

      expect(result).toBe(false);
    });
  });

  describe('setColor', () => {
    it('should set lwPolyline color', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const newColor = new THREE.Color(0x00ff00);
      const result = LwPolylineEntityThreejsRenderer.setColor(lwPolylineData, scene, newColor);

      expect(result).toBe(true);
      const group = LwPolylineEntityThreejsRenderer.getLwPolylineGroup(lwPolylineData, scene);
      const line = group?.getObjectByName('lwPolyline-line') as THREE.Line;
      
      if (line?.material instanceof THREE.LineBasicMaterial) {
        expect(line.material.color.getHex()).toBe(0x00ff00);
      }
    });

    it('should return false for non-existent lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();
      const newColor = new THREE.Color(0x00ff00);

      const result = LwPolylineEntityThreejsRenderer.setColor(lwPolylineData, scene, newColor);

      expect(result).toBe(false);
    });
  });

  describe('setOpacity', () => {
    it('should set lwPolyline opacity', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const result = LwPolylineEntityThreejsRenderer.setOpacity(lwPolylineData, scene, 0.5);

      expect(result).toBe(true);
      const group = LwPolylineEntityThreejsRenderer.getLwPolylineGroup(lwPolylineData, scene);
      const line = group?.getObjectByName('lwPolyline-line') as THREE.Line;
      
      if (line?.material instanceof THREE.LineBasicMaterial) {
        expect(line.material.opacity).toBe(0.5);
      }
    });

    it('should return false for non-existent lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();

      const result = LwPolylineEntityThreejsRenderer.setOpacity(lwPolylineData, scene, 0.5);

      expect(result).toBe(false);
    });
  });

  describe('setLineWidth', () => {
    it('should set lwPolyline line width', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const result = LwPolylineEntityThreejsRenderer.setLineWidth(lwPolylineData, scene, 2.0);

      expect(result).toBe(true);
      const group = LwPolylineEntityThreejsRenderer.getLwPolylineGroup(lwPolylineData, scene);
      const line = group?.getObjectByName('lwPolyline-line') as THREE.Line;
      
      if (line?.material instanceof THREE.LineBasicMaterial) {
        expect((line.material as any).linewidth).toBe(2.0);
      }
    });

    it('should return false for non-existent lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();

      const result = LwPolylineEntityThreejsRenderer.setLineWidth(lwPolylineData, scene, 2.0);

      expect(result).toBe(false);
    });
  });

  describe('isPointOnLwPolyline', () => {
    it('should return true for point on lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const point = new THREE.Vector3(5, 0, 0);
      const result = LwPolylineEntityThreejsRenderer.isPointOnLwPolyline(lwPolylineData, scene, point, 0.1);

      expect(result).toBe(true);
    });

    it('should return false for point not on lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const point = new THREE.Vector3(100, 100, 100);
      const result = LwPolylineEntityThreejsRenderer.isPointOnLwPolyline(lwPolylineData, scene, point, 0.1);

      expect(result).toBe(false);
    });

    it('should handle point outside lwPolyline segment', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const point = new THREE.Vector3(5, 5, 0);
      const result = LwPolylineEntityThreejsRenderer.isPointOnLwPolyline(lwPolylineData, scene, point, 0.1);

      expect(result).toBe(false);
    });
  });

  describe('getPointAtParameter', () => {
    it('should return start point at parameter 0', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const point = LwPolylineEntityThreejsRenderer.getPointAtParameter(lwPolylineData, scene, 0);

      expect(point).toBeInstanceOf(THREE.Vector3);
      expect(point?.x).toBeCloseTo(0);
      expect(point?.y).toBeCloseTo(0);
    });

    it('should return end point at parameter 1', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const point = LwPolylineEntityThreejsRenderer.getPointAtParameter(lwPolylineData, scene, 1);

      expect(point).toBeInstanceOf(THREE.Vector3);
      expect(point?.x).toBeCloseTo(0);
      expect(point?.y).toBeCloseTo(10);
    });

    it('should return mid point at parameter 0.5', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const point = LwPolylineEntityThreejsRenderer.getPointAtParameter(lwPolylineData, scene, 0.5);

      expect(point).toBeInstanceOf(THREE.Vector3);
    });

    it('should return null for invalid parameter', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const point = LwPolylineEntityThreejsRenderer.getPointAtParameter(lwPolylineData, scene, 2);

      expect(point).toBeNull();
    });
  });

  describe('getClosestPoint', () => {
    it('should return closest point on lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const point = new THREE.Vector3(5, 5, 0);
      const closestPoint = LwPolylineEntityThreejsRenderer.getClosestPoint(lwPolylineData, scene, point);

      expect(closestPoint).toBeInstanceOf(THREE.Vector3);
    });

    it('should handle point at start of lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const point = new THREE.Vector3(0, 0, 0);
      const closestPoint = LwPolylineEntityThreejsRenderer.getClosestPoint(lwPolylineData, scene, point);

      expect(closestPoint).toBeInstanceOf(THREE.Vector3);
      expect(closestPoint?.x).toBeCloseTo(0);
      expect(closestPoint?.y).toBeCloseTo(0);
    });

    it('should handle point at end of lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const point = new THREE.Vector3(0, 10, 0);
      const closestPoint = LwPolylineEntityThreejsRenderer.getClosestPoint(lwPolylineData, scene, point);

      expect(closestPoint).toBeInstanceOf(THREE.Vector3);
      expect(closestPoint?.x).toBeCloseTo(0);
      expect(closestPoint?.y).toBeCloseTo(10);
    });
  });

  describe('dispose', () => {
    it('should dispose lwPolyline resources', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      const result = LwPolylineEntityThreejsRenderer.dispose(lwPolylineData, scene);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(0);
    });

    it('should return false for non-existent lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();

      const result = LwPolylineEntityThreejsRenderer.dispose(lwPolylineData, scene);

      expect(result).toBe(false);
    });

    it('should handle null lwPolyline data', () => {
      const result = LwPolylineEntityThreejsRenderer.dispose(null as any, scene);

      expect(result).toBe(false);
    });

    it('should remove group from scene', () => {
      const lwPolylineData = createBasicLwPolylineData();
      LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);

      LwPolylineEntityThreejsRenderer.dispose(lwPolylineData, scene);

      const group = LwPolylineEntityThreejsRenderer.getLwPolylineGroup(lwPolylineData, scene);
      expect(group).toBeNull();
    });

    it('should dispose geometry and material', () => {
      const lwPolylineData = createBasicLwPolylineData();
      const group = LwPolylineEntityThreejsRenderer.render(lwPolylineData, scene);
      
      const line = group?.getObjectByName('lwPolyline-line') as THREE.Line;
      const geometry = line?.geometry;
      const material = line?.material;

      LwPolylineEntityThreejsRenderer.dispose(lwPolylineData, scene);

      expect(scene.children.length).toBe(0);
    });
  });

  describe('calculateLength', () => {
    it('should calculate correct length', () => {
      const lwPolylineData = createBasicLwPolylineData();
      const length = LwPolylineEntityThreejsRenderer.calculateLength(lwPolylineData);

      expect(length).toBe(40);
    });

    it('should calculate length for diagonal lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.IsClosed = false;
      lwPolylineData.Vertices = [
        { X: 0, Y: 0, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 0, Y: 0, Z: 0 } },
        { X: 3, Y: 4, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 3, Y: 4, Z: 0 } }
      ];

      const length = LwPolylineEntityThreejsRenderer.calculateLength(lwPolylineData);

      expect(length).toBe(5);
    });

    it('should calculate length for 3D lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.IsClosed = false;
      lwPolylineData.Vertices = [
        { X: 0, Y: 0, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 0, Y: 0, Z: 0 } },
        { X: 3, Y: 4, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 3, Y: 4, Z: 5 } }
      ];

      const length = LwPolylineEntityThreejsRenderer.calculateLength(lwPolylineData);

      expect(length).toBeCloseTo(7.07, 2);
    });

    it('should return 0 for empty vertices', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Vertices = [];

      const length = LwPolylineEntityThreejsRenderer.calculateLength(lwPolylineData);

      expect(length).toBe(0);
    });

    it('should return 0 for single vertex', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Vertices = [
        { X: 0, Y: 0, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 0, Y: 0, Z: 0 } }
      ];

      const length = LwPolylineEntityThreejsRenderer.calculateLength(lwPolylineData);

      expect(length).toBe(0);
    });

    it('should calculate length for closed lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.IsClosed = true;

      const length = LwPolylineEntityThreejsRenderer.calculateLength(lwPolylineData);

      expect(length).toBe(40);
    });

    it('should calculate length for open lwPolyline', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.IsClosed = false;

      const length = LwPolylineEntityThreejsRenderer.calculateLength(lwPolylineData);

      expect(length).toBe(30);
    });
  });

  describe('calculateArea', () => {
    it('should calculate correct area for square', () => {
      const lwPolylineData = createBasicLwPolylineData();
      const area = LwPolylineEntityThreejsRenderer.calculateArea(lwPolylineData);

      expect(area).toBe(100);
    });

    it('should return 0 for empty vertices', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Vertices = [];

      const area = LwPolylineEntityThreejsRenderer.calculateArea(lwPolylineData);

      expect(area).toBe(0);
    });

    it('should return 0 for single vertex', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Vertices = [
        { X: 0, Y: 0, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 0, Y: 0, Z: 0 } }
      ];

      const area = LwPolylineEntityThreejsRenderer.calculateArea(lwPolylineData);

      expect(area).toBe(0);
    });

    it('should return 0 for two vertices', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Vertices = [
        { X: 0, Y: 0, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 0, Y: 0, Z: 0 } },
        { X: 10, Y: 0, Bulge: 0, StartWidth: 0, EndWidth: 0, Location3D: { X: 10, Y: 0, Z: 0 } }
      ];

      const area = LwPolylineEntityThreejsRenderer.calculateArea(lwPolylineData);

      expect(area).toBe(0);
    });
  });

  describe('getVertexCount', () => {
    it('should return correct vertex count', () => {
      const lwPolylineData = createBasicLwPolylineData();
      const count = LwPolylineEntityThreejsRenderer.getVertexCount(lwPolylineData);

      expect(count).toBe(4);
    });

    it('should return 0 for null data', () => {
      const count = LwPolylineEntityThreejsRenderer.getVertexCount(null as any);

      expect(count).toBe(0);
    });

    it('should return 0 for empty vertices', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Vertices = [];

      const count = LwPolylineEntityThreejsRenderer.getVertexCount(lwPolylineData);

      expect(count).toBe(0);
    });
  });

  describe('getCentroid', () => {
    it('should return correct centroid', () => {
      const lwPolylineData = createBasicLwPolylineData();
      const centroid = LwPolylineEntityThreejsRenderer.getCentroid(lwPolylineData);

      expect(centroid).toBeInstanceOf(THREE.Vector3);
      expect(centroid?.x).toBe(5);
      expect(centroid?.y).toBe(5);
      expect(centroid?.z).toBe(0);
    });

    it('should return null for null data', () => {
      const centroid = LwPolylineEntityThreejsRenderer.getCentroid(null as any);

      expect(centroid).toBeNull();
    });

    it('should return null for null centroid3D', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Centroid3D = null as any;

      const centroid = LwPolylineEntityThreejsRenderer.getCentroid(lwPolylineData);

      expect(centroid).toBeNull();
    });
  });

  describe('getBounds', () => {
    it('should return correct bounds', () => {
      const lwPolylineData = createBasicLwPolylineData();
      const bounds = LwPolylineEntityThreejsRenderer.getBounds(lwPolylineData);

      expect(bounds).toBeInstanceOf(THREE.Box3);
      expect(bounds?.min.x).toBe(0);
      expect(bounds?.min.y).toBe(0);
      expect(bounds?.min.z).toBe(0);
      expect(bounds?.max.x).toBe(10);
      expect(bounds?.max.y).toBe(10);
      expect(bounds?.max.z).toBe(0);
    });

    it('should return null for null data', () => {
      const bounds = LwPolylineEntityThreejsRenderer.getBounds(null as any);

      expect(bounds).toBeNull();
    });

    it('should return null for null bounds', () => {
      const lwPolylineData = createBasicLwPolylineData();
      lwPolylineData.Bounds = null as any;

      const bounds = LwPolylineEntityThreejsRenderer.getBounds(lwPolylineData);

      expect(bounds).toBeNull();
    });
  });
});
