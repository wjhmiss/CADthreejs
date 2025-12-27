import * as THREE from 'three';
import { ShapeEntityThreejsRenderer, ShapeData, Point3DData, BoundsData, ColorData, TransformData, MaterialData, GeometryData } from '../ShapeEntityThreejsRenderer';

describe('ShapeEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    ShapeEntityThreejsRenderer.clearCache();
  });

  const createBasicShapeData = (): ShapeData => ({
    Type: 'Shape',
    EntityType: 'Shape',
    Handle: 'shape-handle-1',
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Visible: true,
    CoordinateSystem: 'WCS',
    InsertionPoint: { X: 10, Y: 10, Z: 0 },
    Size: 2.0,
    Rotation: 0,
    RelativeXScale: 1.0,
    ObliqueAngle: 0,
    Normal: { X: 0, Y: 0, Z: 1 },
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.03,
    LineTypeScale: 1.0,
    ShapeStyleName: 'SHAPE_STYLE',
    ShapeIndex: 0,
    BoundaryPoints: [
      { X: 9, Y: 9, Z: 0 },
      { X: 11, Y: 9, Z: 0 },
      { X: 11, Y: 11, Z: 0 },
      { X: 9, Y: 11, Z: 0 }
    ],
    Bounds: {
      Min: { X: 9, Y: 9, Z: 0, Center: { X: 10, Y: 10, Z: 0 }, Size: { X: 2, Y: 2, Z: 0 } },
      Max: { X: 11, Y: 11, Z: 0, Center: { X: 10, Y: 10, Z: 0 }, Size: { X: 2, Y: 2, Z: 0 } },
      Center: { X: 10, Y: 10, Z: 0 },
      Size: { X: 2, Y: 2, Z: 0 }
    },
    Centroid: { X: 10, Y: 10, Z: 0 },
    Width: 2.0,
    Height: 2.0,
    BoundaryPointCount: 4,
    Thickness: 0,
    Transform: {
      Position: { X: 10, Y: 10, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 2.0, Y: 2.0, Z: 1 },
      Matrix: [2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 1, 0, 10, 10, 0, 1]
    },
    Geometry: {
      Type: 'BufferGeometry',
      VertexCount: 4,
      IndexCount: 8,
      Bounds: {
        Min: { X: 9, Y: 9, Z: 0, Center: { X: 10, Y: 10, Z: 0 }, Size: { X: 2, Y: 2, Z: 0 } },
        Max: { X: 11, Y: 11, Z: 0, Center: { X: 10, Y: 10, Z: 0 }, Size: { X: 2, Y: 2, Z: 0 } },
        Center: { X: 10, Y: 10, Z: 0 },
        Size: { X: 2, Y: 2, Z: 0 }
      }
    },
    Material: {
      Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 },
      Opacity: 1.0,
      Transparent: false,
      Type: 'LineBasicMaterial',
      DepthTest: true,
      Side: true,
      Wireframe: true
    },
    Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 },
    VertexPositions: [9, 9, 0, 11, 9, 0, 11, 11, 0, 9, 11, 0],
    VertexNormals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
    VertexColors: [1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0],
    Indices: [0, 1, 1, 2, 2, 3, 3, 0],
    Opacity: 1.0,
    Transparent: false,
    DepthTest: true
  });

  describe('render', () => {
    it('should render a basic Shape', () => {
      const shapeData = createBasicShapeData();

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.name).toBe('Shape_shape-handle-1');
    });

    it('should not add invisible Shape to scene', () => {
      const shapeData = createBasicShapeData();
      shapeData.Visible = false;

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      expect(group).toBeNull();
      expect(scene.children.length).toBe(0);
    });

    it('should render Shape with correct Mesh object', () => {
      const shapeData = createBasicShapeData();

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBeGreaterThan(0);
      const mesh = group.children[0] as THREE.Mesh;
      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.name).toBe('Shape');
    });

    it('should render Shape with correct material', () => {
      const shapeData = createBasicShapeData();

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.Material;
      expect(material).toBeInstanceOf(THREE.Material);
      expect(material.type).toBe('LineBasicMaterial');
    });

    it('should render Shape with correct geometry', () => {
      const shapeData = createBasicShapeData();

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry).toBeInstanceOf(THREE.BufferGeometry);
      expect(geometry.attributes.position.count).toBe(4);
    });

    it('should render Shape with correct transform matrix', () => {
      const shapeData = createBasicShapeData();

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      const expectedMatrix = new THREE.Matrix4();
      expectedMatrix.fromArray(shapeData.Transform.Matrix);
      expect(group.matrix).toEqual(expectedMatrix);
    });

    it('should render Shape with correct user data', () => {
      const shapeData = createBasicShapeData();

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      expect(group.userData.type).toBe('Shape');
      expect(group.userData.handle).toBe('shape-handle-1');
      expect(group.userData.layerName).toBe('TEST_LAYER');
      expect(group.userData.layerIndex).toBe(0);
      expect(group.userData.insertionPoint).toEqual(shapeData.InsertionPoint);
      expect(group.userData.size).toBe(2.0);
      expect(group.userData.rotation).toBe(0);
      expect(group.userData.relativeXScale).toBe(1.0);
      expect(group.userData.obliqueAngle).toBe(0);
      expect(group.userData.normal).toEqual(shapeData.Normal);
      expect(group.userData.colorIndex).toBe(1);
      expect(group.userData.lineTypeName).toBe('CONTINUOUS');
      expect(group.userData.lineWeight).toBe(0.03);
      expect(group.userData.lineTypeScale).toBe(1.0);
      expect(group.userData.shapeStyleName).toBe('SHAPE_STYLE');
      expect(group.userData.shapeIndex).toBe(0);
      expect(group.userData.thickness).toBe(0);
      expect(group.userData.opacity).toBe(1.0);
      expect(group.userData.transparent).toBe(false);
      expect(group.userData.depthTest).toBe(true);
    });

    it('should render Shape with thickness using MeshBasicMaterial', () => {
      const shapeData = createBasicShapeData();
      shapeData.Thickness = 5.0;
      shapeData.Material.Type = 'MeshBasicMaterial';

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.Material;
      expect(material.type).toBe('MeshBasicMaterial');
    });

    it('should render Shape with correct boundary points', () => {
      const shapeData = createBasicShapeData();

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      expect(group.userData.boundaryPoints).toEqual(shapeData.BoundaryPoints);
      expect(group.userData.boundaryPointCount).toBe(4);
    });

    it('should render Shape with correct bounds', () => {
      const shapeData = createBasicShapeData();

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      expect(group.userData.bounds).toEqual(shapeData.Bounds);
      expect(group.userData.centroid).toEqual(shapeData.Centroid);
      expect(group.userData.width).toBe(2.0);
      expect(group.userData.height).toBe(2.0);
    });

    it('should render Shape with correct color', () => {
      const shapeData = createBasicShapeData();

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      expect(group.userData.color).toEqual(shapeData.Color);
      expect(group.userData.color.Hex).toBe('#FF0000');
    });

    it('should render Shape with correct vertex positions', () => {
      const shapeData = createBasicShapeData();

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions.length).toBe(12);
      expect(positions[0]).toBe(9);
      expect(positions[1]).toBe(9);
      expect(positions[2]).toBe(0);
    });

    it('should render Shape with correct vertex normals', () => {
      const shapeData = createBasicShapeData();

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry as THREE.BufferGeometry;
      const normals = geometry.attributes.normal.array as Float32Array;
      expect(normals.length).toBe(12);
      expect(normals[0]).toBe(0);
      expect(normals[1]).toBe(0);
      expect(normals[2]).toBe(1);
    });

    it('should render Shape with correct vertex colors', () => {
      const shapeData = createBasicShapeData();

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry as THREE.BufferGeometry;
      const colors = geometry.attributes.color.array as Float32Array;
      expect(colors.length).toBe(12);
      expect(colors[0]).toBe(1);
      expect(colors[1]).toBe(0);
      expect(colors[2]).toBe(0);
    });

    it('should render Shape with correct indices', () => {
      const shapeData = createBasicShapeData();

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry as THREE.BufferGeometry;
      const indices = geometry.index?.array as Uint16Array;
      expect(indices).toBeDefined();
      expect(indices.length).toBe(8);
      expect(indices[0]).toBe(0);
      expect(indices[1]).toBe(1);
    });

    it('should cache rendered Shape', () => {
      const shapeData = createBasicShapeData();

      const group1 = ShapeEntityThreejsRenderer.render(shapeData, scene);
      const group2 = ShapeEntityThreejsRenderer.render(shapeData, scene);

      expect(group1).toBe(group2);
      expect(scene.children.length).toBe(1);
    });

    it('should clear cache', () => {
      const shapeData = createBasicShapeData();

      ShapeEntityThreejsRenderer.render(shapeData, scene);
      ShapeEntityThreejsRenderer.clearCache();

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      expect(scene.children.length).toBe(1);
      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should handle Shape with no vertex positions', () => {
      const shapeData = createBasicShapeData();
      shapeData.VertexPositions = [];

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBe(0);
    });

    it('should handle Shape with different rotation', () => {
      const shapeData = createBasicShapeData();
      shapeData.Rotation = Math.PI / 4;
      shapeData.Transform.Rotation = { X: 0, Y: 0, Z: Math.PI / 4 };

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      expect(group.userData.rotation).toBe(Math.PI / 4);
    });

    it('should handle Shape with different scale', () => {
      const shapeData = createBasicShapeData();
      shapeData.RelativeXScale = 2.0;
      shapeData.Transform.Scale = { X: 4.0, Y: 2.0, Z: 1 };

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      expect(group.userData.relativeXScale).toBe(2.0);
    });

    it('should handle Shape with different color index', () => {
      const shapeData = createBasicShapeData();
      shapeData.ColorIndex = 3;
      shapeData.Color = { Index: 3, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const group = ShapeEntityThreejsRenderer.render(shapeData, scene);

      expect(group.userData.colorIndex).toBe(3);
      expect(group.userData.color.Hex).toBe('#00FF00');
    });
  });

  describe('clearCache', () => {
    it('should clear all cached Shapes', () => {
      const shapeData1 = createBasicShapeData();
      const shapeData2 = createBasicShapeData();
      shapeData2.Handle = 'shape-handle-2';

      ShapeEntityThreejsRenderer.render(shapeData1, scene);
      ShapeEntityThreejsRenderer.render(shapeData2, scene);

      expect(scene.children.length).toBe(2);

      ShapeEntityThreejsRenderer.clearCache();

      const group = ShapeEntityThreejsRenderer.render(shapeData1, scene);
      expect(scene.children.length).toBe(1);
      expect(group).toBeInstanceOf(THREE.Group);
    });
  });
});
