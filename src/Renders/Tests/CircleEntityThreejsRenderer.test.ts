import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import * as THREE from 'three';
import { CircleEntityThreejsRenderer, CircleData } from '../CircleEntityThreejsRenderer';

describe('CircleEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
  });

  afterEach(() => {
    scene.clear();
  });

  describe('render', () => {
    it('should render a valid circle', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-1',
        EntityType: 'CIRCLE',
        CenterX: 10,
        CenterY: 20,
        CenterZ: 0,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-1',
        LayerName: '0',
        IsInvisible: false,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: 5, Y: 15, Z: 0 },
          Max: { X: 15, Y: 25, Z: 0 }
        },
        Centroid: { X: 10, Y: 20, Z: 0 },
        Transform: {
          Position: { X: 10, Y: 20, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 10, 20, 0, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const result = CircleEntityThreejsRenderer.render(circleData, scene);

      expect(result).not.toBeNull();
      expect(result).toBeInstanceOf(THREE.Line);
      expect(result.name).toBe('Circle_test-handle-1');
      expect(result.uuid).toBe('test-uuid-1');
      expect(result.userData.type).toBe('Circle');
      expect(result.userData.handle).toBe('test-handle-1');
      expect(result.userData.layerName).toBe('0');
      expect(result.userData.entityType).toBe('CIRCLE');
    });

    it('should return null for invisible circle', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-2',
        EntityType: 'CIRCLE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-2',
        LayerName: '0',
        IsInvisible: true,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: -5, Y: -5, Z: 0 },
          Max: { X: 5, Y: 5, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Transform: {
          Position: { X: 0, Y: 0, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: false,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const result = CircleEntityThreejsRenderer.render(circleData, scene);

      expect(result).toBeNull();
    });

    it('should create correct material from circle data', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-3',
        EntityType: 'CIRCLE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-3',
        LayerName: '0',
        IsInvisible: false,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 2,
        ColorHex: '#FFFF00',
        ColorR: 255,
        ColorG: 255,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 2,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: -5, Y: -5, Z: 0 },
          Max: { X: 5, Y: 5, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Transform: {
          Position: { X: 0, Y: 0, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const result = CircleEntityThreejsRenderer.render(circleData, scene);
      const material = result!.material as THREE.LineBasicMaterial;

      expect(material.color.r).toBeCloseTo(1);
      expect(material.color.g).toBeCloseTo(1);
      expect(material.color.b).toBeCloseTo(0);
      expect(material.transparent).toBe(false);
      expect(material.opacity).toBe(1);
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(true);
    });

    it('should create geometry from circle data when vertices are empty', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-4',
        EntityType: 'CIRCLE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-4',
        LayerName: '0',
        IsInvisible: false,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: -5, Y: -5, Z: 0 },
          Max: { X: 5, Y: 5, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Transform: {
          Position: { X: 0, Y: 0, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const result = CircleEntityThreejsRenderer.render(circleData, scene);
      const geometry = result!.geometry;

      expect(geometry).toBeInstanceOf(THREE.BufferGeometry);
      expect(geometry.attributes.position.count).toBeGreaterThan(0);
    });

    it('should apply transform matrix to the object', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-5',
        EntityType: 'CIRCLE',
        CenterX: 10,
        CenterY: 20,
        CenterZ: 5,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0.5,
        NormalY: 0.5,
        NormalZ: 0.7071,
        Handle: 'test-handle-5',
        LayerName: '0',
        IsInvisible: false,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: 5, Y: 15, Z: 5 },
          Max: { X: 15, Y: 25, Z: 5 }
        },
        Centroid: { X: 10, Y: 20, Z: 5 },
        Transform: {
          Position: { X: 10, Y: 20, Z: 5 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 10, 20, 5, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const result = CircleEntityThreejsRenderer.render(circleData, scene);

      expect(result).not.toBeNull();
      expect(result!.position.x).toBeCloseTo(10);
      expect(result!.position.y).toBeCloseTo(20);
      expect(result!.position.z).toBeCloseTo(5);
    });
  });

  describe('renderFromJson', () => {
    it('should render circle from valid JSON string', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-6',
        EntityType: 'CIRCLE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-6',
        LayerName: '0',
        IsInvisible: false,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: -5, Y: -5, Z: 0 },
          Max: { X: 5, Y: 5, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Transform: {
          Position: { X: 0, Y: 0, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const jsonString = JSON.stringify(circleData);
      const result = CircleEntityThreejsRenderer.renderFromJson(jsonString, scene);

      expect(result).not.toBeNull();
      expect(result).toBeInstanceOf(THREE.Line);
    });

    it('should return null for invalid JSON string', () => {
      const result = CircleEntityThreejsRenderer.renderFromJson('invalid json', scene);

      expect(result).toBeNull();
    });
  });

  describe('renderMultiple', () => {
    it('should render multiple circles', () => {
      const circleDataList: CircleData[] = [
        {
          Type: 'Circle',
          Uuid: 'test-uuid-7',
          EntityType: 'CIRCLE',
          CenterX: 0,
          CenterY: 0,
          CenterZ: 0,
          Radius: 5,
          Thickness: 0,
          Diameter: 10,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Handle: 'test-handle-7',
          LayerName: '0',
          IsInvisible: false,
          LineTypeScale: 1,
          Transparency: 0,
          ColorIndex: 1,
          ColorHex: '#FF0000',
          ColorR: 255,
          ColorG: 0,
          ColorB: 0,
          ColorA: 255,
          LineTypeName: 'CONTINUOUS',
          LineWeight: 1,
          MaterialType: 'LineBasicMaterial',
          MaterialTransparent: false,
          MaterialOpacity: 1,
          MaterialDepthTest: true,
          MaterialDepthWrite: true,
          MaterialSide: 2,
          Points: [],
          Vertices: [],
          Indices: [],
          Circumference: 2 * Math.PI * 5,
          Area: Math.PI * 5 * 5,
          Bounds: {
            Min: { X: -5, Y: -5, Z: 0 },
            Max: { X: 5, Y: 5, Z: 0 }
          },
          Centroid: { X: 0, Y: 0, Z: 0 },
          Transform: {
            Position: { X: 0, Y: 0, Z: 0 },
            Rotation: { X: 0, Y: 0, Z: 0 },
            Scale: { X: 1, Y: 1, Z: 1 },
            Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
          },
          CoordinateSystem: 'AutoCAD',
          RequiresYAxisFlip: true,
          Parent: null,
          Visible: true,
          CastShadow: false,
          ReceiveShadow: false,
          RenderOrder: 0
        },
        {
          Type: 'Circle',
          Uuid: 'test-uuid-8',
          EntityType: 'CIRCLE',
          CenterX: 10,
          CenterY: 10,
          CenterZ: 0,
          Radius: 3,
          Thickness: 0,
          Diameter: 6,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Handle: 'test-handle-8',
          LayerName: '0',
          IsInvisible: false,
          LineTypeScale: 1,
          Transparency: 0,
          ColorIndex: 2,
          ColorHex: '#FFFF00',
          ColorR: 255,
          ColorG: 255,
          ColorB: 0,
          ColorA: 255,
          LineTypeName: 'CONTINUOUS',
          LineWeight: 1,
          MaterialType: 'LineBasicMaterial',
          MaterialTransparent: false,
          MaterialOpacity: 1,
          MaterialDepthTest: true,
          MaterialDepthWrite: true,
          MaterialSide: 2,
          Points: [],
          Vertices: [],
          Indices: [],
          Circumference: 2 * Math.PI * 3,
          Area: Math.PI * 3 * 3,
          Bounds: {
            Min: { X: 7, Y: 7, Z: 0 },
            Max: { X: 13, Y: 13, Z: 0 }
          },
          Centroid: { X: 10, Y: 10, Z: 0 },
          Transform: {
            Position: { X: 10, Y: 10, Z: 0 },
            Rotation: { X: 0, Y: 0, Z: 0 },
            Scale: { X: 1, Y: 1, Z: 1 },
            Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 10, 10, 0, 1]
          },
          CoordinateSystem: 'AutoCAD',
          RequiresYAxisFlip: true,
          Parent: null,
          Visible: true,
          CastShadow: false,
          ReceiveShadow: false,
          RenderOrder: 0
        }
      ];

      const results = CircleEntityThreejsRenderer.renderMultiple(circleDataList, scene);

      expect(results.length).toBe(2);
      expect(results[0]).toBeInstanceOf(THREE.Line);
      expect(results[1]).toBeInstanceOf(THREE.Line);
    });

    it('should skip invisible circles', () => {
      const circleDataList: CircleData[] = [
        {
          Type: 'Circle',
          Uuid: 'test-uuid-9',
          EntityType: 'CIRCLE',
          CenterX: 0,
          CenterY: 0,
          CenterZ: 0,
          Radius: 5,
          Thickness: 0,
          Diameter: 10,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Handle: 'test-handle-9',
          LayerName: '0',
          IsInvisible: false,
          LineTypeScale: 1,
          Transparency: 0,
          ColorIndex: 1,
          ColorHex: '#FF0000',
          ColorR: 255,
          ColorG: 0,
          ColorB: 0,
          ColorA: 255,
          LineTypeName: 'CONTINUOUS',
          LineWeight: 1,
          MaterialType: 'LineBasicMaterial',
          MaterialTransparent: false,
          MaterialOpacity: 1,
          MaterialDepthTest: true,
          MaterialDepthWrite: true,
          MaterialSide: 2,
          Points: [],
          Vertices: [],
          Indices: [],
          Circumference: 2 * Math.PI * 5,
          Area: Math.PI * 5 * 5,
          Bounds: {
            Min: { X: -5, Y: -5, Z: 0 },
            Max: { X: 5, Y: 5, Z: 0 }
          },
          Centroid: { X: 0, Y: 0, Z: 0 },
          Transform: {
            Position: { X: 0, Y: 0, Z: 0 },
            Rotation: { X: 0, Y: 0, Z: 0 },
            Scale: { X: 1, Y: 1, Z: 1 },
            Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
          },
          CoordinateSystem: 'AutoCAD',
          RequiresYAxisFlip: true,
          Parent: null,
          Visible: true,
          CastShadow: false,
          ReceiveShadow: false,
          RenderOrder: 0
        },
        {
          Type: 'Circle',
          Uuid: 'test-uuid-10',
          EntityType: 'CIRCLE',
          CenterX: 10,
          CenterY: 10,
          CenterZ: 0,
          Radius: 3,
          Thickness: 0,
          Diameter: 6,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Handle: 'test-handle-10',
          LayerName: '0',
          IsInvisible: true,
          LineTypeScale: 1,
          Transparency: 0,
          ColorIndex: 2,
          ColorHex: '#FFFF00',
          ColorR: 255,
          ColorG: 255,
          ColorB: 0,
          ColorA: 255,
          LineTypeName: 'CONTINUOUS',
          LineWeight: 1,
          MaterialType: 'LineBasicMaterial',
          MaterialTransparent: false,
          MaterialOpacity: 1,
          MaterialDepthTest: true,
          MaterialDepthWrite: true,
          MaterialSide: 2,
          Points: [],
          Vertices: [],
          Indices: [],
          Circumference: 2 * Math.PI * 3,
          Area: Math.PI * 3 * 3,
          Bounds: {
            Min: { X: 7, Y: 7, Z: 0 },
            Max: { X: 13, Y: 13, Z: 0 }
          },
          Centroid: { X: 10, Y: 10, Z: 0 },
          Transform: {
            Position: { X: 10, Y: 10, Z: 0 },
            Rotation: { X: 0, Y: 0, Z: 0 },
            Scale: { X: 1, Y: 1, Z: 1 },
            Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 10, 10, 0, 1]
          },
          CoordinateSystem: 'AutoCAD',
          RequiresYAxisFlip: true,
          Parent: null,
          Visible: false,
          CastShadow: false,
          ReceiveShadow: false,
          RenderOrder: 0
        }
      ];

      const results = CircleEntityThreejsRenderer.renderMultiple(circleDataList, scene);

      expect(results.length).toBe(1);
      expect(results[0]).toBeInstanceOf(THREE.Line);
    });
  });

  describe('calculateCircumference', () => {
    it('should calculate correct circumference', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-11',
        EntityType: 'CIRCLE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-11',
        LayerName: '0',
        IsInvisible: false,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: -5, Y: -5, Z: 0 },
          Max: { X: 5, Y: 5, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Transform: {
          Position: { X: 0, Y: 0, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const circumference = CircleEntityThreejsRenderer.calculateCircumference(circleData);

      expect(circumference).toBeCloseTo(2 * Math.PI * 5);
    });
  });

  describe('calculateArea', () => {
    it('should calculate correct area', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-12',
        EntityType: 'CIRCLE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-12',
        LayerName: '0',
        IsInvisible: false,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: -5, Y: -5, Z: 0 },
          Max: { X: 5, Y: 5, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Transform: {
          Position: { X: 0, Y: 0, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const area = CircleEntityThreejsRenderer.calculateArea(circleData);

      expect(area).toBeCloseTo(Math.PI * 5 * 5);
    });
  });

  describe('calculateDiameter', () => {
    it('should calculate correct diameter', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-13',
        EntityType: 'CIRCLE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-13',
        LayerName: '0',
        IsInvisible: false,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: -5, Y: -5, Z: 0 },
          Max: { X: 5, Y: 5, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Transform: {
          Position: { X: 0, Y: 0, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const diameter = CircleEntityThreejsRenderer.calculateDiameter(circleData);

      expect(diameter).toBeCloseTo(10);
    });
  });

  describe('isPointOnCircle', () => {
    it('should return true for point on circle', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-14',
        EntityType: 'CIRCLE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-14',
        LayerName: '0',
        IsInvisible: false,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: -5, Y: -5, Z: 0 },
          Max: { X: 5, Y: 5, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Transform: {
          Position: { X: 0, Y: 0, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const point = new THREE.Vector3(5, 0, 0);
      const result = CircleEntityThreejsRenderer.isPointOnCircle(circleData, point);

      expect(result).toBe(true);
    });

    it('should return false for point not on circle', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-15',
        EntityType: 'CIRCLE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-15',
        LayerName: '0',
        IsInvisible: false,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: -5, Y: -5, Z: 0 },
          Max: { X: 5, Y: 5, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Transform: {
          Position: { X: 0, Y: 0, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const point = new THREE.Vector3(10, 0, 0);
      const result = CircleEntityThreejsRenderer.isPointOnCircle(circleData, point);

      expect(result).toBe(false);
    });
  });

  describe('isPointInsideCircle', () => {
    it('should return true for point inside circle', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-16',
        EntityType: 'CIRCLE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-16',
        LayerName: '0',
        IsInvisible: false,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: -5, Y: -5, Z: 0 },
          Max: { X: 5, Y: 5, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Transform: {
          Position: { X: 0, Y: 0, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const point = new THREE.Vector3(3, 3, 0);
      const result = CircleEntityThreejsRenderer.isPointInsideCircle(circleData, point);

      expect(result).toBe(true);
    });

    it('should return false for point outside circle', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-17',
        EntityType: 'CIRCLE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-17',
        LayerName: '0',
        IsInvisible: false,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: -5, Y: -5, Z: 0 },
          Max: { X: 5, Y: 5, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Transform: {
          Position: { X: 0, Y: 0, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const point = new THREE.Vector3(10, 10, 0);
      const result = CircleEntityThreejsRenderer.isPointInsideCircle(circleData, point);

      expect(result).toBe(false);
    });
  });

  describe('getCircleDataFromObject', () => {
    it('should return circle data from object', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-18',
        EntityType: 'CIRCLE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-18',
        LayerName: '0',
        IsInvisible: false,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: -5, Y: -5, Z: 0 },
          Max: { X: 5, Y: 5, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Transform: {
          Position: { X: 0, Y: 0, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const result = CircleEntityThreejsRenderer.render(circleData, scene);
      const retrievedData = CircleEntityThreejsRenderer.getCircleDataFromObject(result!);

      expect(retrievedData).not.toBeNull();
      expect(retrievedData!.Uuid).toBe('test-uuid-18');
    });

    it('should return null for object without circle data', () => {
      const line = new THREE.Line();
      const result = CircleEntityThreejsRenderer.getCircleDataFromObject(line);

      expect(result).toBeNull();
    });
  });

  describe('dispose', () => {
    it('should dispose geometry and material', () => {
      const circleData: CircleData = {
        Type: 'Circle',
        Uuid: 'test-uuid-19',
        EntityType: 'CIRCLE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 5,
        Thickness: 0,
        Diameter: 10,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-19',
        LayerName: '0',
        IsInvisible: false,
        LineTypeScale: 1,
        Transparency: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        ColorA: 255,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        MaterialType: 'LineBasicMaterial',
        MaterialTransparent: false,
        MaterialOpacity: 1,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialSide: 2,
        Points: [],
        Vertices: [],
        Indices: [],
        Circumference: 2 * Math.PI * 5,
        Area: Math.PI * 5 * 5,
        Bounds: {
          Min: { X: -5, Y: -5, Z: 0 },
          Max: { X: 5, Y: 5, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Transform: {
          Position: { X: 0, Y: 0, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const result = CircleEntityThreejsRenderer.render(circleData, scene);

      expect(result).not.toBeNull();
      expect(result!.geometry).toBeDefined();
      expect(result!.material).toBeDefined();

      CircleEntityThreejsRenderer.dispose(result!);
    });
  });
});
