import * as THREE from 'three';
import { RayEntityThreejsRenderer, RayData, Point3DData, ColorData, TransformData, BoundsData, MaterialData, GeometryData } from '../RayEntityThreejsRenderer';

describe('RayEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    RayEntityThreejsRenderer.disposeAll();
  });

  const createBasicRayData = (): RayData => ({
    StartPoint: { X: 0, Y: 0, Z: 0 },
    Direction: { X: 1, Y: 0, Z: 0 },
    ColorIndex: 7,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.03,
    
    EndPoint: { X: 1000, Y: 0, Z: 0 },
    Length: 1000,
    Angle: 0,
    
    Bounds: {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 1000, Y: 0, Z: 0 }
    },
    Centroid: { X: 500, Y: 0, Z: 0 },
    
    Transform: {
      Position: { X: 0, Y: 0, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
    },
    Geometry: {
      Type: 'BufferGeometry',
      VertexCount: 2,
      HasColors: false,
      HasNormals: false,
      HasIndices: true,
      PrimitiveType: 'LinePieces',
      IndexCount: 2
    },
    Material: {
      Type: 'LineBasicMaterial',
      Color: 0xffffff,
      Opacity: 1.0,
      Transparent: false,
      LineWidth: 1.0,
      VertexColors: false
    },
    Color: { Index: 7, Hex: '#FFFFFF', R: 255, G: 255, B: 255, A: 1.0 },
    VertexPositions: [0, 0, 0, 1000, 0, 0],
    VertexNormals: [0, 0, 1, 0, 0, 1],
    VertexColors: [1, 1, 1, 1, 1, 1],
    Indices: [0, 1],
    
    EntityType: 'Ray',
    Visible: true,
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Handle: 'ray-handle-1',
    
    Transparency: 0,
    MaterialName: 'DEFAULT',
    
    CastShadows: false,
    ReceiveShadows: false
  });

  describe('render', () => {
    it('should render a basic Ray', () => {
      const rayData = createBasicRayData();

      const group = RayEntityThreejsRenderer.render(rayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.name).toBe('Ray_ray-handle-1');
    });

    it('should not add invisible Ray to scene', () => {
      const rayData = createBasicRayData();
      rayData.Visible = false;

      const group = RayEntityThreejsRenderer.render(rayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.visible).toBe(false);
      expect(scene.children.length).toBe(0);
    });

    it('should render Ray with correct Line object', () => {
      const rayData = createBasicRayData();

      const group = RayEntityThreejsRenderer.render(rayData, scene);

      expect(group.children.length).toBe(1);
      const line = group.children[0] as THREE.Line;
      expect(line).toBeInstanceOf(THREE.Line);
      expect(line.name).toBe('Ray');
    });

    it('should render Ray with correct material', () => {
      const rayData = createBasicRayData();

      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      expect(line.material).toBeDefined();
      expect(line.material).toBeInstanceOf(THREE.LineBasicMaterial);
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('ffffff');
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
      expect(material.linewidth).toBe(1.0);
      expect(material.vertexColors).toBe(false);
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(false);
    });

    it('should render Ray with correct geometry', () => {
      const rayData = createBasicRayData();

      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      expect(line.geometry).toBeDefined();
      expect(line.geometry).toBeInstanceOf(THREE.BufferGeometry);
      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position).toBeDefined();
      expect(geometry.attributes.position.count).toBe(2);
    });

    it('should render Ray with correct vertex positions', () => {
      const rayData = createBasicRayData();

      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(0);
      expect(positions[1]).toBe(0);
      expect(positions[2]).toBe(0);
      expect(positions[3]).toBe(1000);
      expect(positions[4]).toBe(0);
      expect(positions[5]).toBe(0);
    });

    it('should render Ray with correct vertex colors', () => {
      const rayData = createBasicRayData();
      rayData.Material.VertexColors = true;
      rayData.VertexColors = [1, 0, 0, 0, 1, 0];

      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.color).toBeDefined();
      const colors = geometry.attributes.color.array as Float32Array;
      expect(colors[0]).toBe(1);
      expect(colors[1]).toBe(0);
      expect(colors[2]).toBe(0);
      expect(colors[3]).toBe(0);
      expect(colors[4]).toBe(1);
      expect(colors[5]).toBe(0);
    });

    it('should render Ray with correct vertex normals', () => {
      const rayData = createBasicRayData();

      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.normal).toBeDefined();
      const normals = geometry.attributes.normal.array as Float32Array;
      expect(normals[0]).toBe(0);
      expect(normals[1]).toBe(0);
      expect(normals[2]).toBe(1);
    });

    it('should render Ray with correct indices', () => {
      const rayData = createBasicRayData();

      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      expect(geometry.index).toBeDefined();
      const indices = geometry.index.array as Uint16Array;
      expect(indices[0]).toBe(0);
      expect(indices[1]).toBe(1);
    });

    it('should render Ray with correct color', () => {
      const rayData = createBasicRayData();
      rayData.Color = { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 };
      rayData.Material.Color = 0xff0000;

      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('ff0000');
    });

    it('should render Ray with correct opacity', () => {
      const rayData = createBasicRayData();
      rayData.Transparency = 0.5;
      rayData.Material.Opacity = 0.5;
      rayData.Material.Transparent = true;

      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should render Ray with correct line width', () => {
      const rayData = createBasicRayData();
      rayData.Material.LineWidth = 2.0;

      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.linewidth).toBe(2.0);
    });

    it('should render Ray with correct depth settings', () => {
      const rayData = createBasicRayData();

      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material as THREE.LineBasicMaterial;
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(false);
    });

    it('should cache Ray by handle', () => {
      const rayData = createBasicRayData();

      RayEntityThreejsRenderer.render(rayData, scene);

      const cachedGroup = RayEntityThreejsRenderer.getRayByHandle('ray-handle-1');
      expect(cachedGroup).toBeDefined();
      expect(cachedGroup).toBeInstanceOf(THREE.Group);
    });

    it('should render Ray with zero direction', () => {
      const rayData = createBasicRayData();
      rayData.Direction = { X: 0, Y: 0, Z: 0 };
      rayData.EndPoint = { X: 0, Y: 0, Z: 0 };
      rayData.VertexPositions = [0, 0, 0, 0, 0, 0];

      const group = RayEntityThreejsRenderer.render(rayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render Ray with negative coordinates', () => {
      const rayData = createBasicRayData();
      rayData.StartPoint = { X: -10, Y: -20, Z: -30 };
      rayData.EndPoint = { X: 990, Y: -20, Z: -30 };
      rayData.VertexPositions = [-10, -20, -30, 990, -20, -30];

      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(-10);
      expect(positions[1]).toBe(-20);
      expect(positions[2]).toBe(-30);
    });

    it('should render Ray with very large coordinates', () => {
      const rayData = createBasicRayData();
      rayData.StartPoint = { X: 1000000, Y: 2000000, Z: 3000000 };
      rayData.EndPoint = { X: 1001000, Y: 2000000, Z: 3000000 };
      rayData.VertexPositions = [1000000, 2000000, 3000000, 1001000, 2000000, 3000000];

      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(1000000);
      expect(positions[1]).toBe(2000000);
      expect(positions[2]).toBe(3000000);
    });

    it('should render Ray with correct handle in userData', () => {
      const rayData = createBasicRayData();

      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      expect(line.userData.handle).toBe('ray-handle-1');
    });

    it('should handle Ray with invalid start point', () => {
      const rayData = createBasicRayData();
      rayData.StartPoint = null as any;

      const group = RayEntityThreejsRenderer.render(rayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBe(0);
    });

    it('should handle Ray with invalid direction', () => {
      const rayData = createBasicRayData();
      rayData.Direction = null as any;

      const group = RayEntityThreejsRenderer.render(rayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBe(0);
    });
  });

  describe('update', () => {
    it('should update existing Ray', () => {
      const rayData = createBasicRayData();
      const group = RayEntityThreejsRenderer.render(rayData, scene);

      const updatedRayData = { ...rayData };
      updatedRayData.StartPoint = { X: 20, Y: 20, Z: 0 };

      const result = RayEntityThreejsRenderer.update(updatedRayData, scene);

      expect(result).toBeInstanceOf(THREE.Group);
    });

    it('should update Ray start point', () => {
      const rayData = createBasicRayData();
      const group = RayEntityThreejsRenderer.render(rayData, scene);

      const updatedRayData = { ...rayData };
      updatedRayData.StartPoint = { X: 100, Y: 200, Z: 50 };
      updatedRayData.EndPoint = { X: 1100, Y: 200, Z: 50 };
      updatedRayData.VertexPositions = [100, 200, 50, 1100, 200, 50];

      const result = RayEntityThreejsRenderer.update(updatedRayData, scene);

      expect(result).toBeInstanceOf(THREE.Group);
    });

    it('should update Ray color', () => {
      const rayData = createBasicRayData();
      const group = RayEntityThreejsRenderer.render(rayData, scene);

      const updatedRayData = { ...rayData };
      updatedRayData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };
      updatedRayData.Material.Color = 0x00ff00;

      const result = RayEntityThreejsRenderer.update(updatedRayData, scene);

      expect(result).toBeInstanceOf(THREE.Group);
    });

    it('should update Ray opacity', () => {
      const rayData = createBasicRayData();
      const group = RayEntityThreejsRenderer.render(rayData, scene);

      const updatedRayData = { ...rayData };
      updatedRayData.Transparency = 0.5;
      updatedRayData.Material.Opacity = 0.5;
      updatedRayData.Material.Transparent = true;

      const result = RayEntityThreejsRenderer.update(updatedRayData, scene);

      expect(result).toBeInstanceOf(THREE.Group);
    });
  });

  describe('dispose', () => {
    it('should dispose Ray resources', () => {
      const rayData = createBasicRayData();
      const group = RayEntityThreejsRenderer.render(rayData, scene);

      RayEntityThreejsRenderer.disposeFromScene(rayData.Handle, scene);

      expect(scene.children.length).toBe(0);
    });

    it('should handle non-existent Ray gracefully', () => {
      expect(() => {
        RayEntityThreejsRenderer.disposeFromScene('non-existent-handle', scene);
      }).not.toThrow();
    });

    it('should remove Ray from cache', () => {
      const rayData = createBasicRayData();
      const group = RayEntityThreejsRenderer.render(rayData, scene);

      RayEntityThreejsRenderer.disposeFromScene(rayData.Handle, scene);

      const cachedGroup = RayEntityThreejsRenderer.getRayByHandle('ray-handle-1');
      expect(cachedGroup).toBeNull();
    });

    it('should dispose Ray geometry', () => {
      const rayData = createBasicRayData();
      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      const geometry = line.geometry;
      const originalDispose = geometry.dispose.bind(geometry);
      let disposeCalled = false;
      geometry.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      RayEntityThreejsRenderer.dispose(rayData.Handle);

      expect(disposeCalled).toBe(true);
    });

    it('should dispose Ray material', () => {
      const rayData = createBasicRayData();
      const group = RayEntityThreejsRenderer.render(rayData, scene);
      const line = group.children[0] as THREE.Line;

      const material = line.material;
      const originalDispose = material.dispose.bind(material);
      let disposeCalled = false;
      material.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      RayEntityThreejsRenderer.dispose(rayData.Handle);

      expect(disposeCalled).toBe(true);
    });
  });

  describe('disposeAll', () => {
    it('should dispose all Rays', () => {
      const rayData1 = createBasicRayData();
      rayData1.Handle = 'ray-1';
      const rayData2 = createBasicRayData();
      rayData2.Handle = 'ray-2';

      RayEntityThreejsRenderer.render(rayData1, scene);
      RayEntityThreejsRenderer.render(rayData2, scene);

      expect(scene.children.length).toBe(2);

      RayEntityThreejsRenderer.disposeAllFromScene(scene);

      expect(scene.children.length).toBe(0);
    });

    it('should clear all cached Rays', () => {
      const rayData1 = createBasicRayData();
      rayData1.Handle = 'ray-1';
      const rayData2 = createBasicRayData();
      rayData2.Handle = 'ray-2';

      RayEntityThreejsRenderer.render(rayData1, scene);
      RayEntityThreejsRenderer.render(rayData2, scene);

      expect(RayEntityThreejsRenderer.getRayCount()).toBe(2);

      RayEntityThreejsRenderer.disposeAll();

      expect(RayEntityThreejsRenderer.getRayCount()).toBe(0);
    });
  });

  describe('getRayByHandle', () => {
    it('should return correct Ray by handle', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      const group = RayEntityThreejsRenderer.getRayByHandle('ray-handle-1');

      expect(group).toBeDefined();
      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('Ray_ray-handle-1');
    });

    it('should return null for non-existent handle', () => {
      const group = RayEntityThreejsRenderer.getRayByHandle('non-existent-handle');

      expect(group).toBeNull();
    });
  });

  describe('getRayCount', () => {
    it('should return correct Ray count', () => {
      const rayData1 = createBasicRayData();
      rayData1.Handle = 'ray-1';
      const rayData2 = createBasicRayData();
      rayData2.Handle = 'ray-2';

      RayEntityThreejsRenderer.render(rayData1, scene);
      RayEntityThreejsRenderer.render(rayData2, scene);

      expect(RayEntityThreejsRenderer.getRayCount()).toBe(2);
    });

    it('should return 0 when no Rays are rendered', () => {
      expect(RayEntityThreejsRenderer.getRayCount()).toBe(0);
    });
  });

  describe('setVisibility', () => {
    it('should set Ray visibility to true', () => {
      const rayData = createBasicRayData();
      const group = RayEntityThreejsRenderer.render(rayData, scene);

      RayEntityThreejsRenderer.setVisibility('ray-handle-1', true);

      expect(group.visible).toBe(true);
    });

    it('should set Ray visibility to false', () => {
      const rayData = createBasicRayData();
      const group = RayEntityThreejsRenderer.render(rayData, scene);

      RayEntityThreejsRenderer.setVisibility('ray-handle-1', false);

      expect(group.visible).toBe(false);
    });

    it('should handle non-existent Ray gracefully', () => {
      expect(() => {
        RayEntityThreejsRenderer.setVisibility('non-existent-handle', true);
      }).not.toThrow();
    });
  });

  describe('setOpacity', () => {
    it('should set Ray opacity', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      RayEntityThreejsRenderer.setOpacity('ray-handle-1', 0.5);

      const group = RayEntityThreejsRenderer.getRayByHandle('ray-handle-1');
      const line = group!.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should handle non-existent Ray gracefully', () => {
      expect(() => {
        RayEntityThreejsRenderer.setOpacity('non-existent-handle', 0.5);
      }).not.toThrow();
    });
  });

  describe('setColor', () => {
    it('should set Ray color', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      RayEntityThreejsRenderer.setColor('ray-handle-1', 0xff0000);

      const group = RayEntityThreejsRenderer.getRayByHandle('ray-handle-1');
      const line = group!.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.color.getHexString()).toBe('ff0000');
    });

    it('should handle non-existent Ray gracefully', () => {
      expect(() => {
        RayEntityThreejsRenderer.setColor('non-existent-handle', 0xff0000);
      }).not.toThrow();
    });
  });

  describe('setLineWidth', () => {
    it('should set Ray line width', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      RayEntityThreejsRenderer.setLineWidth('ray-handle-1', 2.0);

      const group = RayEntityThreejsRenderer.getRayByHandle('ray-handle-1');
      const line = group!.children[0] as THREE.Line;
      const material = line.material as THREE.LineBasicMaterial;
      expect(material.linewidth).toBe(2.0);
    });

    it('should handle non-existent Ray gracefully', () => {
      expect(() => {
        RayEntityThreejsRenderer.setLineWidth('non-existent-handle', 2.0);
      }).not.toThrow();
    });
  });

  describe('getBounds', () => {
    it('should return correct bounds', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      const bounds = RayEntityThreejsRenderer.getBounds('ray-handle-1');

      expect(bounds).toBeDefined();
      expect(bounds).toBeInstanceOf(THREE.Box3);
      expect(bounds.min.x).toBe(0);
      expect(bounds.min.y).toBe(0);
      expect(bounds.min.z).toBe(0);
      expect(bounds.max.x).toBe(1000);
      expect(bounds.max.y).toBe(0);
      expect(bounds.max.z).toBe(0);
    });

    it('should return null for non-existent Ray', () => {
      const bounds = RayEntityThreejsRenderer.getBounds('non-existent-handle');

      expect(bounds).toBeNull();
    });
  });

  describe('getCentroid', () => {
    it('should return correct centroid', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      const centroid = RayEntityThreejsRenderer.getCentroid('ray-handle-1');

      expect(centroid).toBeDefined();
      expect(centroid.x).toBe(500);
      expect(centroid.y).toBe(0);
      expect(centroid.z).toBe(0);
    });

    it('should return null for non-existent Ray', () => {
      const centroid = RayEntityThreejsRenderer.getCentroid('non-existent-handle');

      expect(centroid).toBeNull();
    });
  });

  describe('getLength', () => {
    it('should return correct length', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      const length = RayEntityThreejsRenderer.getLength('ray-handle-1');

      expect(length).toBe(1000);
    });

    it('should return null for non-existent Ray', () => {
      const length = RayEntityThreejsRenderer.getLength('non-existent-handle');

      expect(length).toBe(0);
    });
  });

  describe('getAngle', () => {
    it('should return correct angle', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      const angle = RayEntityThreejsRenderer.getAngle('ray-handle-1');

      expect(angle).toBe(0);
    });

    it('should return null for non-existent Ray', () => {
      const angle = RayEntityThreejsRenderer.getAngle('non-existent-handle');

      expect(angle).toBe(0);
    });
  });

  describe('getStartPoint', () => {
    it('should return correct start point', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      const startPoint = RayEntityThreejsRenderer.getStartPoint('ray-handle-1');

      expect(startPoint).toBeDefined();
      expect(startPoint.x).toBe(0);
      expect(startPoint.y).toBe(0);
      expect(startPoint.z).toBe(0);
    });

    it('should return null for non-existent Ray', () => {
      const startPoint = RayEntityThreejsRenderer.getStartPoint('non-existent-handle');

      expect(startPoint).toBeNull();
    });
  });

  describe('getEndPoint', () => {
    it('should return correct end point', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      const endPoint = RayEntityThreejsRenderer.getEndPoint('ray-handle-1');

      expect(endPoint).toBeDefined();
      expect(endPoint.x).toBe(1000);
      expect(endPoint.y).toBe(0);
      expect(endPoint.z).toBe(0);
    });

    it('should return null for non-existent Ray', () => {
      const endPoint = RayEntityThreejsRenderer.getEndPoint('non-existent-handle');

      expect(endPoint).toBeNull();
    });
  });

  describe('getDirection', () => {
    it('should return correct direction', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      const direction = RayEntityThreejsRenderer.getDirection('ray-handle-1');

      expect(direction).toBeDefined();
      expect(direction.x).toBe(1);
      expect(direction.y).toBe(0);
      expect(direction.z).toBe(0);
    });

    it('should return null for non-existent Ray', () => {
      const direction = RayEntityThreejsRenderer.getDirection('non-existent-handle');

      expect(direction).toBeNull();
    });
  });

  describe('isRayVisible', () => {
    it('should return true for visible Ray', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      const isVisible = RayEntityThreejsRenderer.isRayVisible('ray-handle-1');

      expect(isVisible).toBe(true);
    });

    it('should return false for invisible Ray', () => {
      const rayData = createBasicRayData();
      rayData.Visible = false;
      RayEntityThreejsRenderer.render(rayData, scene);

      const isVisible = RayEntityThreejsRenderer.isRayVisible('ray-handle-1');

      expect(isVisible).toBe(false);
    });

    it('should return false for non-existent Ray', () => {
      const isVisible = RayEntityThreejsRenderer.isRayVisible('non-existent-handle');

      expect(isVisible).toBe(false);
    });
  });

  describe('getPointAtDistance', () => {
    it('should return correct point at distance', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      const point = RayEntityThreejsRenderer.getPointAtDistance('ray-handle-1', 500);

      expect(point).toBeDefined();
      expect(point.x).toBe(500);
      expect(point.y).toBe(0);
      expect(point.z).toBe(0);
    });

    it('should return null for non-existent Ray', () => {
      const point = RayEntityThreejsRenderer.getPointAtDistance('non-existent-handle', 500);

      expect(point).toBeNull();
    });
  });

  describe('getDistanceToPoint', () => {
    it('should return correct distance to point', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      const distance = RayEntityThreejsRenderer.getDistanceToPoint('ray-handle-1', new THREE.Vector3(0, 100, 0));

      expect(distance).toBe(100);
    });

    it('should return null for non-existent Ray', () => {
      const distance = RayEntityThreejsRenderer.getDistanceToPoint('non-existent-handle', new THREE.Vector3(0, 100, 0));

      expect(distance).toBeNull();
    });
  });

  describe('intersectsRay', () => {
    it('should detect ray intersection', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      const otherRay = new THREE.Ray(new THREE.Vector3(0, 100, 0), new THREE.Vector3(0, -1, 0));
      const intersects = RayEntityThreejsRenderer.intersectsRay('ray-handle-1', otherRay);

      expect(intersects).toBe(true);
    });

    it('should return false for non-existent Ray', () => {
      const otherRay = new THREE.Ray(new THREE.Vector3(0, 100, 0), new THREE.Vector3(0, -1, 0));
      const intersects = RayEntityThreejsRenderer.intersectsRay('non-existent-handle', otherRay);

      expect(intersects).toBe(false);
    });
  });

  describe('getRayData', () => {
    it('should return correct Ray data', () => {
      const rayData = createBasicRayData();
      RayEntityThreejsRenderer.render(rayData, scene);

      const returnedRayData = RayEntityThreejsRenderer.getRayData('ray-handle-1');

      expect(returnedRayData).toBeDefined();
      expect(returnedRayData.StartPoint.X).toBe(0);
      expect(returnedRayData.StartPoint.Y).toBe(0);
      expect(returnedRayData.StartPoint.Z).toBe(0);
      expect(returnedRayData.EndPoint.X).toBe(1000);
      expect(returnedRayData.EndPoint.Y).toBe(0);
      expect(returnedRayData.EndPoint.Z).toBe(0);
    });

    it('should return null for non-existent Ray', () => {
      const returnedRayData = RayEntityThreejsRenderer.getRayData('non-existent-handle');

      expect(returnedRayData).toBeNull();
    });
  });
});
