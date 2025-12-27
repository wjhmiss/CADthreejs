import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import * as THREE from 'three';
import { EllipseEntityThreejsRenderer, EllipseData } from '../EllipseEntityThreejsRenderer';

describe('EllipseEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
  });

  afterEach(() => {
    scene.clear();
  });

  describe('render', () => {
    it('should render a valid full ellipse', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-1',
        EntityType: 'ELLIPSE',
        CenterX: 10,
        CenterY: 20,
        CenterZ: 0,
        MajorAxisEndPointX: 15,
        MajorAxisEndPointY: 20,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 0.6,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
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
        MajorRadius: 5,
        MinorRadius: 3,
        RotationAngle: 0,
        Length: 2 * Math.PI * Math.sqrt((5 * 5 + 3 * 3) / 2),
        Area: Math.PI * 5 * 3,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: 5, Y: 17, Z: 0 },
          Max: { X: 15, Y: 23, Z: 0 },
          Center: { X: 10, Y: 20, Z: 0 },
          Size: { X: 10, Y: 6, Z: 0 }
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

      const result = EllipseEntityThreejsRenderer.render(ellipseData, scene);

      expect(result).not.toBeNull();
      expect(result).toBeInstanceOf(THREE.LineLoop);
      expect(result.userData.type).toBe('Ellipse');
      expect(result.userData.handle).toBe('test-handle-1');
      expect(result.userData.layerName).toBe('0');
      expect(result.userData.entityType).toBe('ELLIPSE');
    });

    it('should render an ellipse arc', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-2',
        EntityType: 'ELLIPSE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        MajorAxisEndPointX: 10,
        MajorAxisEndPointY: 0,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: Math.PI,
        Thickness: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-2',
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
        MajorRadius: 10,
        MinorRadius: 5,
        RotationAngle: 0,
        Length: Math.PI * 7.5,
        Area: Math.PI * 10 * 5,
        IsFullEllipse: false,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: -10, Y: -5, Z: 0 },
          Max: { X: 10, Y: 5, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 20, Y: 10, Z: 0 }
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

      const result = EllipseEntityThreejsRenderer.render(ellipseData, scene);

      expect(result).not.toBeNull();
      expect(result).toBeInstanceOf(THREE.Line);
      expect(result.userData.type).toBe('Ellipse');
    });

    it('should return null for invisible ellipse', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-3',
        EntityType: 'ELLIPSE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        MajorAxisEndPointX: 10,
        MajorAxisEndPointY: 0,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-3',
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
        MajorRadius: 10,
        MinorRadius: 5,
        RotationAngle: 0,
        Length: 2 * Math.PI * 7.5,
        Area: Math.PI * 10 * 5,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: -10, Y: -5, Z: 0 },
          Max: { X: 10, Y: 5, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 20, Y: 10, Z: 0 }
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

      const result = EllipseEntityThreejsRenderer.render(ellipseData, scene);

      expect(result).toBeNull();
    });

    it('should apply transform matrix if provided', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-4',
        EntityType: 'ELLIPSE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        MajorAxisEndPointX: 10,
        MajorAxisEndPointY: 0,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
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
        MajorRadius: 10,
        MinorRadius: 5,
        RotationAngle: 0,
        Length: 2 * Math.PI * 7.5,
        Area: Math.PI * 10 * 5,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: -10, Y: -5, Z: 0 },
          Max: { X: 10, Y: 5, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 20, Y: 10, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Transform: {
          Position: { X: 10, Y: 20, Z: 30 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 10, 20, 30, 1]
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const result = EllipseEntityThreejsRenderer.render(ellipseData, scene);

      expect(result).not.toBeNull();
      expect(result.position.x).toBeCloseTo(10);
      expect(result.position.y).toBeCloseTo(20);
      expect(result.position.z).toBeCloseTo(30);
    });

    it('should use center position if no transform matrix', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-5',
        EntityType: 'ELLIPSE',
        CenterX: 15,
        CenterY: 25,
        CenterZ: 5,
        MajorAxisEndPointX: 20,
        MajorAxisEndPointY: 25,
        MajorAxisEndPointZ: 5,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
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
        MajorRadius: 5,
        MinorRadius: 2.5,
        RotationAngle: 0,
        Length: 2 * Math.PI * 3.75,
        Area: Math.PI * 5 * 2.5,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: 10, Y: 22.5, Z: 5 },
          Max: { X: 20, Y: 27.5, Z: 5 },
          Center: { X: 15, Y: 25, Z: 5 },
          Size: { X: 10, Y: 5, Z: 0 }
        },
        Centroid: { X: 15, Y: 25, Z: 5 },
        Transform: {
          Position: { X: 0, Y: 0, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 },
          Matrix: []
        },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Parent: null,
        Visible: true,
        CastShadow: false,
        ReceiveShadow: false,
        RenderOrder: 0
      };

      const result = EllipseEntityThreejsRenderer.render(ellipseData, scene);

      expect(result).not.toBeNull();
      expect(result.position.x).toBeCloseTo(15);
      expect(result.position.y).toBeCloseTo(25);
      expect(result.position.z).toBeCloseTo(5);
    });
  });

  describe('renderFromJson', () => {
    it('should render ellipse from valid JSON string', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-6',
        EntityType: 'ELLIPSE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        MajorAxisEndPointX: 10,
        MajorAxisEndPointY: 0,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
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
        MajorRadius: 10,
        MinorRadius: 5,
        RotationAngle: 0,
        Length: 2 * Math.PI * 7.5,
        Area: Math.PI * 10 * 5,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: -10, Y: -5, Z: 0 },
          Max: { X: 10, Y: 5, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 20, Y: 10, Z: 0 }
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

      const json = JSON.stringify(ellipseData);
      const result = EllipseEntityThreejsRenderer.renderFromJson(json, scene);

      expect(result).not.toBeNull();
      expect(result.userData.type).toBe('Ellipse');
    });

    it('should return null for invalid JSON string', () => {
      const invalidJson = 'invalid json';
      const result = EllipseEntityThreejsRenderer.renderFromJson(invalidJson, scene);

      expect(result).toBeNull();
    });

    it('should return null for malformed JSON', () => {
      const malformedJson = '{ "Type": "Ellipse", "CenterX": 0, }';
      const result = EllipseEntityThreejsRenderer.renderFromJson(malformedJson, scene);

      expect(result).toBeNull();
    });
  });

  describe('renderWithGroup', () => {
    it('should render ellipse with group', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-7',
        EntityType: 'ELLIPSE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        MajorAxisEndPointX: 10,
        MajorAxisEndPointY: 0,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
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
        MajorRadius: 10,
        MinorRadius: 5,
        RotationAngle: 0,
        Length: 2 * Math.PI * 7.5,
        Area: Math.PI * 10 * 5,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: -10, Y: -5, Z: 0 },
          Max: { X: 10, Y: 5, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 20, Y: 10, Z: 0 }
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

      const result = EllipseEntityThreejsRenderer.renderWithGroup(ellipseData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
      expect(result.mesh).toBeInstanceOf(THREE.LineLoop);
      expect(result.geometry).toBeInstanceOf(THREE.BufferGeometry);
      expect(result.material).toBeInstanceOf(THREE.LineBasicMaterial);
    });

    it('should return null for invisible ellipse', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-8',
        EntityType: 'ELLIPSE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        MajorAxisEndPointX: 10,
        MajorAxisEndPointY: 0,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-8',
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
        MajorRadius: 10,
        MinorRadius: 5,
        RotationAngle: 0,
        Length: 2 * Math.PI * 7.5,
        Area: Math.PI * 10 * 5,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: -10, Y: -5, Z: 0 },
          Max: { X: 10, Y: 5, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 20, Y: 10, Z: 0 }
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

      const result = EllipseEntityThreejsRenderer.renderWithGroup(ellipseData, scene);

      expect(result).toBeNull();
    });
  });

  describe('dispose', () => {
    it('should dispose geometry, material, and group', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-9',
        EntityType: 'ELLIPSE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        MajorAxisEndPointX: 10,
        MajorAxisEndPointY: 0,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
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
        MajorRadius: 10,
        MinorRadius: 5,
        RotationAngle: 0,
        Length: 2 * Math.PI * 7.5,
        Area: Math.PI * 10 * 5,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: -10, Y: -5, Z: 0 },
          Max: { X: 10, Y: 5, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 20, Y: 10, Z: 0 }
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

      const result = EllipseEntityThreejsRenderer.renderWithGroup(ellipseData, scene);

      expect(result).not.toBeNull();
      expect(result.geometry).toBeDefined();
      expect(result.material).toBeDefined();
      expect(result.group).toBeDefined();

      EllipseEntityThreejsRenderer.dispose(result);

      expect(result.group.children.length).toBe(0);
    });
  });

  describe('createEllipseFromData', () => {
    it('should create ellipse from data', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-10',
        EntityType: 'ELLIPSE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        MajorAxisEndPointX: 10,
        MajorAxisEndPointY: 0,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-10',
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
        MajorRadius: 10,
        MinorRadius: 5,
        RotationAngle: 0,
        Length: 2 * Math.PI * 7.5,
        Area: Math.PI * 10 * 5,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: -10, Y: -5, Z: 0 },
          Max: { X: 10, Y: 5, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 20, Y: 10, Z: 0 }
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

      const result = EllipseEntityThreejsRenderer.createEllipseFromData(ellipseData);

      expect(result).not.toBeNull();
      expect(result).toBeInstanceOf(THREE.LineLoop);
    });
  });

  describe('createEllipseGroup', () => {
    it('should create ellipse group from data', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-11',
        EntityType: 'ELLIPSE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        MajorAxisEndPointX: 10,
        MajorAxisEndPointY: 0,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
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
        MajorRadius: 10,
        MinorRadius: 5,
        RotationAngle: 0,
        Length: 2 * Math.PI * 7.5,
        Area: Math.PI * 10 * 5,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: -10, Y: -5, Z: 0 },
          Max: { X: 10, Y: 5, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 20, Y: 10, Z: 0 }
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

      const result = EllipseEntityThreejsRenderer.createEllipseGroup(ellipseData);

      expect(result).not.toBeNull();
      expect(result).toBeInstanceOf(THREE.Group);
    });
  });

  describe('getEllipseProperties', () => {
    it('should return ellipse properties', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-12',
        EntityType: 'ELLIPSE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        MajorAxisEndPointX: 10,
        MajorAxisEndPointY: 0,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
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
        MajorRadius: 10,
        MinorRadius: 5,
        RotationAngle: 0,
        Length: 2 * Math.PI * 7.5,
        Area: Math.PI * 10 * 5,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: -10, Y: -5, Z: 0 },
          Max: { X: 10, Y: 5, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 20, Y: 10, Z: 0 }
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

      const properties = EllipseEntityThreejsRenderer.getEllipseProperties(ellipseData);

      expect(properties.majorRadius).toBe(10);
      expect(properties.minorRadius).toBe(5);
      expect(properties.area).toBeCloseTo(Math.PI * 10 * 5);
      expect(properties.length).toBeCloseTo(2 * Math.PI * 7.5);
      expect(properties.isFullEllipse).toBe(true);
    });
  });

  describe('validateEllipseData', () => {
    it('should validate valid ellipse data', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-13',
        EntityType: 'ELLIPSE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        MajorAxisEndPointX: 10,
        MajorAxisEndPointY: 0,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
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
        MajorRadius: 10,
        MinorRadius: 5,
        RotationAngle: 0,
        Length: 2 * Math.PI * 7.5,
        Area: Math.PI * 10 * 5,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: -10, Y: -5, Z: 0 },
          Max: { X: 10, Y: 5, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 20, Y: 10, Z: 0 }
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

      const isValid = EllipseEntityThreejsRenderer.validateEllipseData(ellipseData);

      expect(isValid).toBe(true);
    });

    it('should invalidate ellipse with zero major radius', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-14',
        EntityType: 'ELLIPSE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        MajorAxisEndPointX: 10,
        MajorAxisEndPointY: 0,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
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
        MajorRadius: 0,
        MinorRadius: 0,
        RotationAngle: 0,
        Length: 0,
        Area: 0,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 0, Y: 0, Z: 0 }
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

      const isValid = EllipseEntityThreejsRenderer.validateEllipseData(ellipseData);

      expect(isValid).toBe(false);
    });

    it('should invalidate ellipse with invalid radius ratio', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-15',
        EntityType: 'ELLIPSE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        MajorAxisEndPointX: 10,
        MajorAxisEndPointY: 0,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 1.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
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
        MajorRadius: 10,
        MinorRadius: 15,
        RotationAngle: 0,
        Length: 0,
        Area: 0,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: -10, Y: -15, Z: 0 },
          Max: { X: 10, Y: 15, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 20, Y: 30, Z: 0 }
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

      const isValid = EllipseEntityThreejsRenderer.validateEllipseData(ellipseData);

      expect(isValid).toBe(false);
    });
  });

  describe('getBoundingBox', () => {
    it('should return bounding box', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-16',
        EntityType: 'ELLIPSE',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        MajorAxisEndPointX: 10,
        MajorAxisEndPointY: 0,
        MajorAxisEndPointZ: 0,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
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
        MajorRadius: 10,
        MinorRadius: 5,
        RotationAngle: 0,
        Length: 2 * Math.PI * 7.5,
        Area: Math.PI * 10 * 5,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: -10, Y: -5, Z: 0 },
          Max: { X: 10, Y: 5, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 20, Y: 10, Z: 0 }
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

      const boundingBox = EllipseEntityThreejsRenderer.getBoundingBox(ellipseData);

      expect(boundingBox).not.toBeNull();
      expect(boundingBox).toBeInstanceOf(THREE.Box3);
      expect(boundingBox.min.x).toBeCloseTo(-10);
      expect(boundingBox.min.y).toBeCloseTo(-5);
      expect(boundingBox.min.z).toBeCloseTo(0);
      expect(boundingBox.max.x).toBeCloseTo(10);
      expect(boundingBox.max.y).toBeCloseTo(5);
      expect(boundingBox.max.z).toBeCloseTo(0);
    });
  });

  describe('getCentroid', () => {
    it('should return centroid', () => {
      const ellipseData: EllipseData = {
        Type: 'Ellipse',
        Uuid: 'test-uuid-17',
        EntityType: 'ELLIPSE',
        CenterX: 10,
        CenterY: 20,
        CenterZ: 5,
        MajorAxisEndPointX: 15,
        MajorAxisEndPointY: 20,
        MajorAxisEndPointZ: 5,
        RadiusRatio: 0.5,
        StartParameter: 0,
        EndParameter: 2 * Math.PI,
        Thickness: 0,
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
        MajorRadius: 5,
        MinorRadius: 2.5,
        RotationAngle: 0,
        Length: 2 * Math.PI * 3.75,
        Area: Math.PI * 5 * 2.5,
        IsFullEllipse: true,
        IsCounterClockwise: true,
        Bounds: {
          Min: { X: 5, Y: 17.5, Z: 5 },
          Max: { X: 15, Y: 22.5, Z: 5 },
          Center: { X: 10, Y: 20, Z: 5 },
          Size: { X: 10, Y: 5, Z: 0 }
        },
        Centroid: { X: 10, Y: 20, Z: 5 },
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

      const centroid = EllipseEntityThreejsRenderer.getCentroid(ellipseData);

      expect(centroid).not.toBeNull();
      expect(centroid).toBeInstanceOf(THREE.Vector3);
      expect(centroid.x).toBeCloseTo(10);
      expect(centroid.y).toBeCloseTo(20);
      expect(centroid.z).toBeCloseTo(5);
    });
  });
});
