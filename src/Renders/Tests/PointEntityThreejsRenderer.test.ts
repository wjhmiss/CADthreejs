import * as THREE from 'three';
import { PointEntityThreejsRenderer, PointData, Point3DData, ColorData, TransformData, NormalData, BoundsData3D, PointMaterialData, PointGeometryData } from '../PointEntityThreejsRenderer';

describe('PointEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    PointEntityThreejsRenderer.clearCache();
  });

  const createBasicPointData = (): PointData => ({
    Type: 'Point',
    EntityType: 'Point',
    Handle: 'point-handle-1',
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Visible: true,
    CoordinateSystem: 'WCS',
    Location: { X: 10, Y: 10, Z: 0 },
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.03,
    Thickness: 0,
    Rotation: 0,
    Size: 6.0,
    Bounds3D: {
      Min: { X: 7, Y: 7, Z: -3 },
      Max: { X: 13, Y: 13, Z: 3 }
    },
    Centroid3D: { X: 10, Y: 10, Z: 0 },
    Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 },
    Normal: { X: 0, Y: 0, Z: 1 },
    Transform: {
      Position: { X: 10, Y: 10, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1.0, Y: 1.0, Z: 1.0 },
      Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 10, 10, 0, 1]
    },
    Opacity: 1.0,
    Transparent: false,
    DepthTest: true,
    Material: {
      Type: 'PointsMaterial',
      Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 },
      Opacity: 1.0,
      Transparent: false,
      DepthTest: true,
      SizeAttenuation: true,
      Size: 6.0
    },
    Geometry: {
      Type: 'BufferGeometry',
      Position: { X: 10, Y: 10, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1.0, Y: 1.0, Z: 1.0 },
      Normal: { X: 0, Y: 0, Z: 1 },
      BoundingBox: {
        Min: { X: 7, Y: 7, Z: -3 },
        Max: { X: 13, Y: 13, Z: 3 }
      },
      Size: 6.0
    }
  });

  describe('render', () => {
    it('should render a basic Point', () => {
      const pointData = createBasicPointData();

      const group = PointEntityThreejsRenderer.render(pointData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.name).toBe('Point_point-handle-1');
    });

    it('should not add invisible Point to scene', () => {
      const pointData = createBasicPointData();
      pointData.Visible = false;

      const group = PointEntityThreejsRenderer.render(pointData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.visible).toBe(false);
      expect(scene.children.length).toBe(0);
    });

    it('should render Point with correct Points object', () => {
      const pointData = createBasicPointData();

      const group = PointEntityThreejsRenderer.render(pointData, scene);

      expect(group.children.length).toBe(1);
      const points = group.children[0] as THREE.Points;
      expect(points).toBeInstanceOf(THREE.Points);
      expect(points.name).toBe('Point');
    });

    it('should render Point with correct material', () => {
      const pointData = createBasicPointData();

      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      expect(points.material).toBeDefined();
      expect(points.material).toBeInstanceOf(THREE.PointsMaterial);
      const material = points.material as THREE.PointsMaterial;
      expect(material.color.getHexString()).toBe('ff0000');
      expect(material.size).toBe(6.0);
      expect(material.sizeAttenuation).toBe(true);
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(false);
    });

    it('should render Point with correct geometry', () => {
      const pointData = createBasicPointData();

      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      expect(points.geometry).toBeDefined();
      expect(points.geometry).toBeInstanceOf(THREE.BufferGeometry);
      const geometry = points.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position).toBeDefined();
      expect(geometry.attributes.position.count).toBe(1);
    });

    it('should render Point with correct position', () => {
      const pointData = createBasicPointData();
      pointData.Location = { X: 100, Y: 200, Z: 50 };

      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      const geometry = points.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(100);
      expect(positions[1]).toBe(200);
      expect(positions[2]).toBe(50);
    });

    it('should render Point with correct color', () => {
      const pointData = createBasicPointData();
      pointData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      const material = points.material as THREE.PointsMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should render Point with correct size', () => {
      const pointData = createBasicPointData();
      pointData.Size = 10.0;
      pointData.Material.Size = 10.0;

      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      const material = points.material as THREE.PointsMaterial;
      expect(material.size).toBe(10.0);
    });

    it('should render Point with correct opacity', () => {
      const pointData = createBasicPointData();
      pointData.Opacity = 0.5;
      pointData.Transparent = true;
      pointData.Material.Opacity = 0.5;
      pointData.Material.Transparent = true;

      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      const material = points.material as THREE.PointsMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should render Point with correct depth settings', () => {
      const pointData = createBasicPointData();
      pointData.DepthTest = false;
      pointData.Material.DepthTest = false;

      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      const material = points.material as THREE.PointsMaterial;
      expect(material.depthTest).toBe(false);
    });

    it('should render Point with correct size attenuation', () => {
      const pointData = createBasicPointData();
      pointData.Material.SizeAttenuation = false;

      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      const material = points.material as THREE.PointsMaterial;
      expect(material.sizeAttenuation).toBe(false);
    });

    it('should cache Point by handle', () => {
      const pointData = createBasicPointData();

      PointEntityThreejsRenderer.render(pointData, scene);

      const cachedPoints = (PointEntityThreejsRenderer as any).pointCache.get('point-handle-1');
      expect(cachedPoints).toBeDefined();
      expect(cachedPoints).toBeInstanceOf(THREE.Points);
    });

    it('should render Point with zero location', () => {
      const pointData = createBasicPointData();
      pointData.Location = { X: 0, Y: 0, Z: 0 };

      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      const geometry = points.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(0);
      expect(positions[1]).toBe(0);
      expect(positions[2]).toBe(0);
    });

    it('should render Point with negative coordinates', () => {
      const pointData = createBasicPointData();
      pointData.Location = { X: -10, Y: -20, Z: -30 };

      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      const geometry = points.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(-10);
      expect(positions[1]).toBe(-20);
      expect(positions[2]).toBe(-30);
    });

    it('should render Point with very large coordinates', () => {
      const pointData = createBasicPointData();
      pointData.Location = { X: 1000000, Y: 2000000, Z: 3000000 };

      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      const geometry = points.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(1000000);
      expect(positions[1]).toBe(2000000);
      expect(positions[2]).toBe(3000000);
    });

    it('should render Point with very small coordinates', () => {
      const pointData = createBasicPointData();
      pointData.Location = { X: 0.0001, Y: 0.0002, Z: 0.0003 };

      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      const geometry = points.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBeCloseTo(0.0001, 7);
      expect(positions[1]).toBeCloseTo(0.0002, 7);
      expect(positions[2]).toBeCloseTo(0.0003, 7);
    });

    it('should render Point with correct handle in userData', () => {
      const pointData = createBasicPointData();

      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      expect(points.userData.handle).toBe('point-handle-1');
    });
  });

  describe('renderMultiple', () => {
    it('should render multiple Points', () => {
      const pointDataArray = [
        createBasicPointData(),
        createBasicPointData()
      ];
      pointDataArray[0].Handle = 'point-1';
      pointDataArray[0].Location = { X: 10, Y: 10, Z: 0 };
      pointDataArray[1].Handle = 'point-2';
      pointDataArray[1].Location = { X: 20, Y: 20, Z: 0 };

      const group = PointEntityThreejsRenderer.renderMultiple(pointDataArray, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('MultiplePoints');
      expect(group.children.length).toBe(1);
    });

    it('should render multiple Points with correct geometry', () => {
      const pointDataArray = [
        createBasicPointData(),
        createBasicPointData(),
        createBasicPointData()
      ];
      pointDataArray[0].Handle = 'point-1';
      pointDataArray[0].Location = { X: 10, Y: 10, Z: 0 };
      pointDataArray[1].Handle = 'point-2';
      pointDataArray[1].Location = { X: 20, Y: 20, Z: 0 };
      pointDataArray[2].Handle = 'point-3';
      pointDataArray[2].Location = { X: 30, Y: 30, Z: 0 };

      const group = PointEntityThreejsRenderer.renderMultiple(pointDataArray, scene);
      const points = group.children[0] as THREE.Points;

      const geometry = points.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position.count).toBe(3);
    });

    it('should render multiple Points with correct positions', () => {
      const pointDataArray = [
        createBasicPointData(),
        createBasicPointData()
      ];
      pointDataArray[0].Handle = 'point-1';
      pointDataArray[0].Location = { X: 10, Y: 10, Z: 0 };
      pointDataArray[1].Handle = 'point-2';
      pointDataArray[1].Location = { X: 20, Y: 20, Z: 0 };

      const group = PointEntityThreejsRenderer.renderMultiple(pointDataArray, scene);
      const points = group.children[0] as THREE.Points;

      const geometry = points.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(10);
      expect(positions[1]).toBe(10);
      expect(positions[2]).toBe(0);
      expect(positions[3]).toBe(20);
      expect(positions[4]).toBe(20);
      expect(positions[5]).toBe(0);
    });

    it('should render multiple Points with vertex colors', () => {
      const pointDataArray = [
        createBasicPointData(),
        createBasicPointData()
      ];
      pointDataArray[0].Handle = 'point-1';
      pointDataArray[0].Color = { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 };
      pointDataArray[1].Handle = 'point-2';
      pointDataArray[1].Color = { Index: 2, Hex: '#FFFF00', R: 255, G: 255, B: 0, A: 1.0 };

      const group = PointEntityThreejsRenderer.renderMultiple(pointDataArray, scene);
      const points = group.children[0] as THREE.Points;

      const geometry = points.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.color).toBeDefined();
      expect(geometry.attributes.color.count).toBe(2);
    });

    it('should render multiple Points with correct material', () => {
      const pointDataArray = [
        createBasicPointData(),
        createBasicPointData()
      ];
      pointDataArray[0].Handle = 'point-1';
      pointDataArray[1].Handle = 'point-2';

      const group = PointEntityThreejsRenderer.renderMultiple(pointDataArray, scene);
      const points = group.children[0] as THREE.Points;

      const material = points.material as THREE.PointsMaterial;
      expect(material.vertexColors).toBe(true);
      expect(material.size).toBe(6.0);
      expect(material.sizeAttenuation).toBe(true);
      expect(material.transparent).toBe(true);
      expect(material.opacity).toBe(1.0);
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(false);
    });

    it('should render empty array of Points', () => {
      const pointDataArray: PointData[] = [];

      const group = PointEntityThreejsRenderer.renderMultiple(pointDataArray, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('MultiplePoints');
    });

    it('should render single Point using renderMultiple', () => {
      const pointDataArray = [createBasicPointData()];
      pointDataArray[0].Handle = 'point-1';

      const group = PointEntityThreejsRenderer.renderMultiple(pointDataArray, scene);
      const points = group.children[0] as THREE.Points;

      const geometry = points.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position.count).toBe(1);
    });
  });

  describe('update', () => {
    it('should update existing Point', () => {
      const pointData = createBasicPointData();
      const group = PointEntityThreejsRenderer.render(pointData, scene);

      const updatedPointData = { ...pointData };
      updatedPointData.Location = { X: 20, Y: 20, Z: 0 };

      const result = PointEntityThreejsRenderer.update(updatedPointData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent Point', () => {
      const pointData = createBasicPointData();

      const result = PointEntityThreejsRenderer.update(pointData, scene);

      expect(result).toBe(false);
    });

    it('should update Point location', () => {
      const pointData = createBasicPointData();
      const group = PointEntityThreejsRenderer.render(pointData, scene);

      const updatedPointData = { ...pointData };
      updatedPointData.Location = { X: 100, Y: 200, Z: 50 };

      const result = PointEntityThreejsRenderer.update(updatedPointData, scene);

      expect(result).toBe(true);
    });

    it('should update Point color', () => {
      const pointData = createBasicPointData();
      const group = PointEntityThreejsRenderer.render(pointData, scene);

      const updatedPointData = { ...pointData };
      updatedPointData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const result = PointEntityThreejsRenderer.update(updatedPointData, scene);

      expect(result).toBe(true);
    });

    it('should update Point size', () => {
      const pointData = createBasicPointData();
      const group = PointEntityThreejsRenderer.render(pointData, scene);

      const updatedPointData = { ...pointData };
      updatedPointData.Size = 10.0;
      updatedPointData.Material.Size = 10.0;

      const result = PointEntityThreejsRenderer.update(updatedPointData, scene);

      expect(result).toBe(true);
    });
  });

  describe('dispose', () => {
    it('should dispose Point resources', () => {
      const pointData = createBasicPointData();
      const group = PointEntityThreejsRenderer.render(pointData, scene);

      PointEntityThreejsRenderer.dispose(pointData, scene);

      expect(scene.children.length).toBe(0);
    });

    it('should handle non-existent Point gracefully', () => {
      const pointData = createBasicPointData();

      expect(() => {
        PointEntityThreejsRenderer.dispose(pointData, scene);
      }).not.toThrow();
    });

    it('should remove Point from cache', () => {
      const pointData = createBasicPointData();
      const group = PointEntityThreejsRenderer.render(pointData, scene);

      PointEntityThreejsRenderer.dispose(pointData, scene);

      const cachedPoints = (PointEntityThreejsRenderer as any).pointCache.get('point-handle-1');
      expect(cachedPoints).toBeUndefined();
    });

    it('should dispose Point geometry', () => {
      const pointData = createBasicPointData();
      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      const geometry = points.geometry;
      const originalDispose = geometry.dispose.bind(geometry);
      let disposeCalled = false;
      geometry.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      PointEntityThreejsRenderer.dispose(pointData, scene);

      expect(disposeCalled).toBe(true);
    });

    it('should dispose Point material', () => {
      const pointData = createBasicPointData();
      const group = PointEntityThreejsRenderer.render(pointData, scene);
      const points = group.children[0] as THREE.Points;

      const material = points.material;
      const originalDispose = material.dispose.bind(material);
      let disposeCalled = false;
      material.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      PointEntityThreejsRenderer.dispose(pointData, scene);

      expect(disposeCalled).toBe(true);
    });
  });

  describe('setVisibility', () => {
    it('should set Point visibility to true', () => {
      const pointData = createBasicPointData();
      const group = PointEntityThreejsRenderer.render(pointData, scene);

      PointEntityThreejsRenderer.setVisibility(pointData, scene, true);

      expect(group.visible).toBe(true);
    });

    it('should set Point visibility to false', () => {
      const pointData = createBasicPointData();
      const group = PointEntityThreejsRenderer.render(pointData, scene);

      PointEntityThreejsRenderer.setVisibility(pointData, scene, false);

      expect(group.visible).toBe(false);
    });

    it('should handle non-existent Point gracefully', () => {
      const pointData = createBasicPointData();

      expect(() => {
        PointEntityThreejsRenderer.setVisibility(pointData, scene, true);
      }).not.toThrow();
    });

    it('should add Point to scene when setting visibility to true', () => {
      const pointData = createBasicPointData();
      pointData.Visible = false;
      const group = PointEntityThreejsRenderer.render(pointData, scene);

      PointEntityThreejsRenderer.setVisibility(pointData, scene, true);

      expect(scene.children.length).toBe(1);
    });

    it('should remove Point from scene when setting visibility to false', () => {
      const pointData = createBasicPointData();
      const group = PointEntityThreejsRenderer.render(pointData, scene);

      PointEntityThreejsRenderer.setVisibility(pointData, scene, false);

      expect(scene.children.length).toBe(0);
    });
  });

  describe('getLocation', () => {
    it('should return correct location', () => {
      const pointData = createBasicPointData();
      pointData.Location = { X: 100, Y: 200, Z: 50 };

      const location = PointEntityThreejsRenderer.getLocation(pointData);

      expect(location).toBeInstanceOf(THREE.Vector3);
      expect(location.x).toBe(100);
      expect(location.y).toBe(200);
      expect(location.z).toBe(50);
    });

    it('should return default location for null location', () => {
      const pointData = createBasicPointData();
      pointData.Location = { X: 0, Y: 0, Z: 0 };

      const location = PointEntityThreejsRenderer.getLocation(pointData);

      expect(location).toBeInstanceOf(THREE.Vector3);
      expect(location.x).toBe(0);
      expect(location.y).toBe(0);
      expect(location.z).toBe(0);
    });
  });

  describe('getColor', () => {
    it('should return correct color', () => {
      const pointData = createBasicPointData();
      pointData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const color = PointEntityThreejsRenderer.getColor(pointData);

      expect(color).toBeInstanceOf(THREE.Color);
      expect(color.getHexString()).toBe('00ff00');
    });
  });

  describe('getSize', () => {
    it('should return correct size', () => {
      const pointData = createBasicPointData();
      pointData.Size = 10.0;

      const size = PointEntityThreejsRenderer.getSize(pointData);

      expect(size).toBe(10.0);
    });

    it('should return default size for zero size', () => {
      const pointData = createBasicPointData();
      pointData.Size = 0;

      const size = PointEntityThreejsRenderer.getSize(pointData);

      expect(size).toBe(6.0);
    });
  });

  describe('getRotation', () => {
    it('should return correct rotation', () => {
      const pointData = createBasicPointData();
      pointData.Rotation = Math.PI / 4;

      const rotation = PointEntityThreejsRenderer.getRotation(pointData);

      expect(rotation).toBe(Math.PI / 4);
    });

    it('should return 0 for no rotation', () => {
      const pointData = createBasicPointData();
      pointData.Rotation = 0;

      const rotation = PointEntityThreejsRenderer.getRotation(pointData);

      expect(rotation).toBe(0);
    });
  });

  describe('getOpacity', () => {
    it('should return correct opacity', () => {
      const pointData = createBasicPointData();
      pointData.Opacity = 0.5;

      const opacity = PointEntityThreejsRenderer.getOpacity(pointData);

      expect(opacity).toBe(0.5);
    });

    it('should return 1.0 for default opacity', () => {
      const pointData = createBasicPointData();
      pointData.Opacity = 1.0;

      const opacity = PointEntityThreejsRenderer.getOpacity(pointData);

      expect(opacity).toBe(1.0);
    });
  });

  describe('getTransparent', () => {
    it('should return correct transparent flag', () => {
      const pointData = createBasicPointData();
      pointData.Transparent = true;

      const transparent = PointEntityThreejsRenderer.getTransparent(pointData);

      expect(transparent).toBe(true);
    });

    it('should return false for opaque Point', () => {
      const pointData = createBasicPointData();
      pointData.Transparent = false;

      const transparent = PointEntityThreejsRenderer.getTransparent(pointData);

      expect(transparent).toBe(false);
    });
  });

  describe('getDepthTest', () => {
    it('should return correct depth test flag', () => {
      const pointData = createBasicPointData();
      pointData.DepthTest = true;

      const depthTest = PointEntityThreejsRenderer.getDepthTest(pointData);

      expect(depthTest).toBe(true);
    });

    it('should return false when depth test is disabled', () => {
      const pointData = createBasicPointData();
      pointData.DepthTest = false;

      const depthTest = PointEntityThreejsRenderer.getDepthTest(pointData);

      expect(depthTest).toBe(false);
    });
  });

  describe('getHandle', () => {
    it('should return correct handle', () => {
      const pointData = createBasicPointData();
      pointData.Handle = 'point-handle-123';

      const handle = PointEntityThreejsRenderer.getHandle(pointData);

      expect(handle).toBe('point-handle-123');
    });
  });

  describe('getLayerName', () => {
    it('should return correct layer name', () => {
      const pointData = createBasicPointData();
      pointData.LayerName = 'MY_LAYER';

      const layerName = PointEntityThreejsRenderer.getLayerName(pointData);

      expect(layerName).toBe('MY_LAYER');
    });
  });

  describe('getVisible', () => {
    it('should return correct visible flag', () => {
      const pointData = createBasicPointData();
      pointData.Visible = true;

      const visible = PointEntityThreejsRenderer.getVisible(pointData);

      expect(visible).toBe(true);
    });

    it('should return false for invisible Point', () => {
      const pointData = createBasicPointData();
      pointData.Visible = false;

      const visible = PointEntityThreejsRenderer.getVisible(pointData);

      expect(visible).toBe(false);
    });
  });

  describe('getNormal', () => {
    it('should return correct normal', () => {
      const pointData = createBasicPointData();
      pointData.Normal = { X: 0, Y: 1, Z: 0 };

      const normal = PointEntityThreejsRenderer.getNormal(pointData);

      expect(normal).toEqual({ X: 0, Y: 1, Z: 0 });
    });

    it('should return default normal for zero normal', () => {
      const pointData = createBasicPointData();
      pointData.Normal = { X: 0, Y: 0, Z: 1 };

      const normal = PointEntityThreejsRenderer.getNormal(pointData);

      expect(normal).toEqual({ X: 0, Y: 0, Z: 1 });
    });
  });

  describe('getTransform', () => {
    it('should return correct transform', () => {
      const pointData = createBasicPointData();
      pointData.Transform = {
        Position: { X: 10, Y: 20, Z: 30 },
        Rotation: { X: 0, Y: 0, Z: Math.PI / 2 },
        Scale: { X: 2.0, Y: 2.0, Z: 2.0 },
        Matrix: [2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 10, 20, 30, 1]
      };

      const transform = PointEntityThreejsRenderer.getTransform(pointData);

      expect(transform.ScaleX).toBe(2.0);
      expect(transform.ScaleY).toBe(2.0);
      expect(transform.ScaleZ).toBe(2.0);
      expect(transform.Rotation).toBe(Math.PI / 2);
      expect(transform.TranslateX).toBe(10);
      expect(transform.TranslateY).toBe(20);
      expect(transform.TranslateZ).toBe(30);
    });
  });

  describe('getBounds3D', () => {
    it('should return correct 3D bounds', () => {
      const pointData = createBasicPointData();
      pointData.Bounds3D = {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 10, Y: 10, Z: 10 }
      };

      const bounds3D = PointEntityThreejsRenderer.getBounds3D(pointData);

      expect(bounds3D.Min).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(bounds3D.Max).toEqual({ X: 10, Y: 10, Z: 10 });
    });
  });

  describe('getCentroid3D', () => {
    it('should return correct 3D centroid', () => {
      const pointData = createBasicPointData();
      pointData.Centroid3D = { X: 5, Y: 5, Z: 5 };

      const centroid3D = PointEntityThreejsRenderer.getCentroid3D(pointData);

      expect(centroid3D).toEqual({ X: 5, Y: 5, Z: 5 });
    });
  });

  describe('getMaterial', () => {
    it('should return correct material', () => {
      const pointData = createBasicPointData();
      pointData.Material = {
        Type: 'PointsMaterial',
        Color: { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 },
        Opacity: 0.8,
        Transparent: true,
        DepthTest: true,
        SizeAttenuation: false,
        Size: 10.0
      };

      const material = PointEntityThreejsRenderer.getMaterial(pointData);

      expect(material.Type).toBe('PointsMaterial');
      expect(material.Color.Index).toBe(5);
      expect(material.Opacity).toBe(0.8);
      expect(material.Transparent).toBe(true);
      expect(material.DepthTest).toBe(true);
      expect(material.SizeAttenuation).toBe(false);
      expect(material.Size).toBe(10.0);
    });
  });

  describe('getGeometry', () => {
    it('should return correct geometry', () => {
      const pointData = createBasicPointData();
      pointData.Geometry = {
        Type: 'BufferGeometry',
        Position: { X: 10, Y: 20, Z: 30 },
        Rotation: { X: 0, Y: 0, Z: Math.PI / 4 },
        Scale: { X: 1.0, Y: 1.0, Z: 1.0 },
        Normal: { X: 0, Y: 0, Z: 1 },
        BoundingBox: {
          Min: { X: 7, Y: 17, Z: 27 },
          Max: { X: 13, Y: 23, Z: 33 }
        },
        Size: 6.0
      };

      const geometry = PointEntityThreejsRenderer.getGeometry(pointData);

      expect(geometry.Type).toBe('BufferGeometry');
      expect(geometry.Position).toEqual({ X: 10, Y: 20, Z: 30 });
      expect(geometry.Rotation.Z).toBe(Math.PI / 4);
      expect(geometry.Scale).toEqual({ X: 1.0, Y: 1.0, Z: 1.0 });
      expect(geometry.Normal).toEqual({ X: 0, Y: 0, Z: 1 });
      expect(geometry.Size).toBe(6.0);
    });
  });
});
