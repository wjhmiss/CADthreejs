import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import * as THREE from 'three';
import { ArcEntityThreejsRenderer, ArcData } from '../ArcEntityThreejsRenderer';

describe('ArcEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
  });

  afterEach(() => {
    scene.clear();
  });

  describe('render', () => {
    it('should render a valid arc', () => {
      const arcData: ArcData = {
        Type: 'Arc',
        Uuid: 'test-uuid-1',
        EntityType: 'ARC',
        CenterX: 10,
        CenterY: 20,
        CenterZ: 0,
        Radius: 5,
        StartAngle: 0,
        EndAngle: Math.PI / 2,
        Thickness: 0,
        Sweep: Math.PI / 2,
        StartX: 15,
        StartY: 20,
        StartZ: 0,
        EndX: 10,
        EndY: 25,
        EndZ: 0,
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
        IsCCW: true,
        Length: 5 * Math.PI / 2,
        Area: 0.5 * 5 * 5 * Math.PI / 2,
        ChordLength: Math.sqrt(Math.pow(10 - 15, 2) + Math.pow(25 - 20, 2)),
        Sagitta: 5 - Math.sqrt(Math.pow(5, 2) - Math.pow(Math.sqrt(Math.pow(10 - 15, 2) + Math.pow(25 - 20, 2)) / 2, 2)),
        MidAngle: Math.PI / 4,
        MidX: 10 + 5 * Math.cos(Math.PI / 4),
        MidY: 20 + 5 * Math.sin(Math.PI / 4),
        MidZ: 0,
        Bounds: {
          Min: { X: 10, Y: 20, Z: 0 },
          Max: { X: 15, Y: 25, Z: 0 }
        },
        Centroid: { X: 10 + 5 * Math.cos(Math.PI / 4), Y: 20 + 5 * Math.sin(Math.PI / 4), Z: 0 },
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

      const result = ArcEntityThreejsRenderer.render(arcData, scene);

      expect(result).not.toBeNull();
      expect(result).toBeInstanceOf(THREE.Line);
      expect(result.name).toBe('Arc_test-handle-1');
      expect(result.uuid).toBe('test-uuid-1');
      expect(result.userData.type).toBe('Arc');
      expect(result.userData.handle).toBe('test-handle-1');
      expect(result.userData.layerName).toBe('0');
      expect(result.userData.entityType).toBe('ARC');
    });

    it('should return null for invisible arc', () => {
      const arcData: ArcData = {
        Type: 'Arc',
        Uuid: 'test-uuid-2',
        EntityType: 'ARC',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 10,
        StartAngle: 0,
        EndAngle: Math.PI,
        Thickness: 0,
        Sweep: Math.PI,
        StartX: 10,
        StartY: 0,
        StartZ: 0,
        EndX: -10,
        EndY: 0,
        EndZ: 0,
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
        IsCCW: true,
        Length: 10 * Math.PI,
        Area: 0.5 * 10 * 10 * Math.PI,
        ChordLength: 20,
        Sagitta: 10,
        MidAngle: Math.PI / 2,
        MidX: 0,
        MidY: 10,
        MidZ: 0,
        Bounds: {
          Min: { X: -10, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 0, Y: 10, Z: 0 },
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

      const result = ArcEntityThreejsRenderer.render(arcData, scene);

      expect(result).toBeNull();
    });

    it('should create correct material from arc data', () => {
      const arcData: ArcData = {
        Type: 'Arc',
        Uuid: 'test-uuid-3',
        EntityType: 'ARC',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 10,
        StartAngle: 0,
        EndAngle: Math.PI / 2,
        Thickness: 0,
        Sweep: Math.PI / 2,
        StartX: 10,
        StartY: 0,
        StartZ: 0,
        EndX: 0,
        EndY: 10,
        EndZ: 0,
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
        IsCCW: true,
        Length: 10 * Math.PI / 2,
        Area: 0.5 * 10 * 10 * Math.PI / 2,
        ChordLength: Math.sqrt(200),
        Sagitta: 10 - Math.sqrt(50),
        MidAngle: Math.PI / 4,
        MidX: 10 * Math.cos(Math.PI / 4),
        MidY: 10 * Math.sin(Math.PI / 4),
        MidZ: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 10 * Math.cos(Math.PI / 4), Y: 10 * Math.sin(Math.PI / 4), Z: 0 },
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

      const result = ArcEntityThreejsRenderer.render(arcData, scene) as THREE.Line;

      expect(result).not.toBeNull();
      expect(result.material).toBeInstanceOf(THREE.LineBasicMaterial);
      const material = result.material as THREE.LineBasicMaterial;
      expect(material.color.r).toBeCloseTo(1);
      expect(material.color.g).toBeCloseTo(1);
      expect(material.color.b).toBeCloseTo(0);
      expect(material.transparent).toBe(false);
      expect(material.opacity).toBe(1);
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(true);
    });

    it('should create geometry from arc data when vertices are empty', () => {
      const arcData: ArcData = {
        Type: 'Arc',
        Uuid: 'test-uuid-4',
        EntityType: 'ARC',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 10,
        StartAngle: 0,
        EndAngle: Math.PI / 2,
        Thickness: 0,
        Sweep: Math.PI / 2,
        StartX: 10,
        StartY: 0,
        StartZ: 0,
        EndX: 0,
        EndY: 10,
        EndZ: 0,
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
        IsCCW: true,
        Length: 10 * Math.PI / 2,
        Area: 0.5 * 10 * 10 * Math.PI / 2,
        ChordLength: Math.sqrt(200),
        Sagitta: 10 - Math.sqrt(50),
        MidAngle: Math.PI / 4,
        MidX: 10 * Math.cos(Math.PI / 4),
        MidY: 10 * Math.sin(Math.PI / 4),
        MidZ: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 10 * Math.cos(Math.PI / 4), Y: 10 * Math.sin(Math.PI / 4), Z: 0 },
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

      const result = ArcEntityThreejsRenderer.render(arcData, scene) as THREE.Line;

      expect(result).not.toBeNull();
      expect(result.geometry).toBeInstanceOf(THREE.BufferGeometry);
      const geometry = result.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position).toBeDefined();
      expect(geometry.attributes.position.count).toBeGreaterThan(0);
    });

    it('should apply transform matrix to the object', () => {
      const arcData: ArcData = {
        Type: 'Arc',
        Uuid: 'test-uuid-5',
        EntityType: 'ARC',
        CenterX: 10,
        CenterY: 20,
        CenterZ: 5,
        Radius: 10,
        StartAngle: 0,
        EndAngle: Math.PI / 2,
        Thickness: 0,
        Sweep: Math.PI / 2,
        StartX: 20,
        StartY: 20,
        StartZ: 5,
        EndX: 10,
        EndY: 30,
        EndZ: 5,
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
        IsCCW: true,
        Length: 10 * Math.PI / 2,
        Area: 0.5 * 10 * 10 * Math.PI / 2,
        ChordLength: Math.sqrt(200),
        Sagitta: 10 - Math.sqrt(50),
        MidAngle: Math.PI / 4,
        MidX: 10 + 10 * Math.cos(Math.PI / 4),
        MidY: 20 + 10 * Math.sin(Math.PI / 4),
        MidZ: 5,
        Bounds: {
          Min: { X: 10, Y: 20, Z: 5 },
          Max: { X: 20, Y: 30, Z: 5 }
        },
        Centroid: { X: 10 + 10 * Math.cos(Math.PI / 4), Y: 20 + 10 * Math.sin(Math.PI / 4), Z: 5 },
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

      const result = ArcEntityThreejsRenderer.render(arcData, scene) as THREE.Line;

      expect(result).not.toBeNull();
      expect(result.position.x).toBeCloseTo(10);
      expect(result.position.y).toBeCloseTo(20);
      expect(result.position.z).toBeCloseTo(5);
    });
  });

  describe('renderFromJson', () => {
    it('should render arc from valid JSON string', () => {
      const arcData: ArcData = {
        Type: 'Arc',
        Uuid: 'test-uuid-json',
        EntityType: 'ARC',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 10,
        StartAngle: 0,
        EndAngle: Math.PI / 2,
        Thickness: 0,
        Sweep: Math.PI / 2,
        StartX: 10,
        StartY: 0,
        StartZ: 0,
        EndX: 0,
        EndY: 10,
        EndZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-json',
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
        IsCCW: true,
        Length: 10 * Math.PI / 2,
        Area: 0.5 * 10 * 10 * Math.PI / 2,
        ChordLength: Math.sqrt(200),
        Sagitta: 10 - Math.sqrt(50),
        MidAngle: Math.PI / 4,
        MidX: 10 * Math.cos(Math.PI / 4),
        MidY: 10 * Math.sin(Math.PI / 4),
        MidZ: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 10 * Math.cos(Math.PI / 4), Y: 10 * Math.sin(Math.PI / 4), Z: 0 },
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

      const jsonString = JSON.stringify(arcData);
      const result = ArcEntityThreejsRenderer.renderFromJson(jsonString, scene);

      expect(result).not.toBeNull();
      expect(result).toBeInstanceOf(THREE.Line);
    });

    it('should return null for invalid JSON string', () => {
      const invalidJson = '{ invalid json }';
      const result = ArcEntityThreejsRenderer.renderFromJson(invalidJson, scene);

      expect(result).toBeNull();
    });
  });

  describe('renderMultiple', () => {
    it('should render multiple arcs', () => {
      const arcDataList: ArcData[] = [
        {
          Type: 'Arc',
          Uuid: 'test-uuid-multi-1',
          EntityType: 'ARC',
          CenterX: 0,
          CenterY: 0,
          CenterZ: 0,
          Radius: 10,
          StartAngle: 0,
          EndAngle: Math.PI / 2,
          Thickness: 0,
          Sweep: Math.PI / 2,
          StartX: 10,
          StartY: 0,
          StartZ: 0,
          EndX: 0,
          EndY: 10,
          EndZ: 0,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Handle: 'test-handle-multi-1',
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
          IsCCW: true,
          Length: 10 * Math.PI / 2,
          Area: 0.5 * 10 * 10 * Math.PI / 2,
          ChordLength: Math.sqrt(200),
          Sagitta: 10 - Math.sqrt(50),
          MidAngle: Math.PI / 4,
          MidX: 10 * Math.cos(Math.PI / 4),
          MidY: 10 * Math.sin(Math.PI / 4),
          MidZ: 0,
          Bounds: {
            Min: { X: 0, Y: 0, Z: 0 },
            Max: { X: 10, Y: 10, Z: 0 }
          },
          Centroid: { X: 10 * Math.cos(Math.PI / 4), Y: 10 * Math.sin(Math.PI / 4), Z: 0 },
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
          Type: 'Arc',
          Uuid: 'test-uuid-multi-2',
          EntityType: 'ARC',
          CenterX: 20,
          CenterY: 20,
          CenterZ: 0,
          Radius: 15,
          StartAngle: 0,
          EndAngle: Math.PI,
          Thickness: 0,
          Sweep: Math.PI,
          StartX: 35,
          StartY: 20,
          StartZ: 0,
          EndX: 5,
          EndY: 20,
          EndZ: 0,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Handle: 'test-handle-multi-2',
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
          IsCCW: true,
          Length: 15 * Math.PI,
          Area: 0.5 * 15 * 15 * Math.PI,
          ChordLength: 30,
          Sagitta: 15,
          MidAngle: Math.PI / 2,
          MidX: 20,
          MidY: 35,
          MidZ: 0,
          Bounds: {
            Min: { X: 5, Y: 20, Z: 0 },
            Max: { X: 35, Y: 35, Z: 0 }
          },
          Centroid: { X: 20, Y: 35, Z: 0 },
          Transform: {
            Position: { X: 20, Y: 20, Z: 0 },
            Rotation: { X: 0, Y: 0, Z: 0 },
            Scale: { X: 1, Y: 1, Z: 1 },
            Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 20, 20, 0, 1]
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

      const results = ArcEntityThreejsRenderer.renderMultiple(arcDataList, scene);

      expect(results).toHaveLength(2);
      expect(results[0]).toBeInstanceOf(THREE.Line);
      expect(results[1]).toBeInstanceOf(THREE.Line);
    });

    it('should skip invisible arcs', () => {
      const arcDataList: ArcData[] = [
        {
          Type: 'Arc',
          Uuid: 'test-uuid-visible',
          EntityType: 'ARC',
          CenterX: 0,
          CenterY: 0,
          CenterZ: 0,
          Radius: 10,
          StartAngle: 0,
          EndAngle: Math.PI / 2,
          Thickness: 0,
          Sweep: Math.PI / 2,
          StartX: 10,
          StartY: 0,
          StartZ: 0,
          EndX: 0,
          EndY: 10,
          EndZ: 0,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Handle: 'test-handle-visible',
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
          IsCCW: true,
          Length: 10 * Math.PI / 2,
          Area: 0.5 * 10 * 10 * Math.PI / 2,
          ChordLength: Math.sqrt(200),
          Sagitta: 10 - Math.sqrt(50),
          MidAngle: Math.PI / 4,
          MidX: 10 * Math.cos(Math.PI / 4),
          MidY: 10 * Math.sin(Math.PI / 4),
          MidZ: 0,
          Bounds: {
            Min: { X: 0, Y: 0, Z: 0 },
            Max: { X: 10, Y: 10, Z: 0 }
          },
          Centroid: { X: 10 * Math.cos(Math.PI / 4), Y: 10 * Math.sin(Math.PI / 4), Z: 0 },
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
          Type: 'Arc',
          Uuid: 'test-uuid-invisible',
          EntityType: 'ARC',
          CenterX: 20,
          CenterY: 20,
          CenterZ: 0,
          Radius: 10,
          StartAngle: 0,
          EndAngle: Math.PI / 2,
          Thickness: 0,
          Sweep: Math.PI / 2,
          StartX: 30,
          StartY: 20,
          StartZ: 0,
          EndX: 20,
          EndY: 30,
          EndZ: 0,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Handle: 'test-handle-invisible',
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
          IsCCW: true,
          Length: 10 * Math.PI / 2,
          Area: 0.5 * 10 * 10 * Math.PI / 2,
          ChordLength: Math.sqrt(200),
          Sagitta: 10 - Math.sqrt(50),
          MidAngle: Math.PI / 4,
          MidX: 20 + 10 * Math.cos(Math.PI / 4),
          MidY: 20 + 10 * Math.sin(Math.PI / 4),
          MidZ: 0,
          Bounds: {
            Min: { X: 20, Y: 20, Z: 0 },
            Max: { X: 30, Y: 30, Z: 0 }
          },
          Centroid: { X: 20 + 10 * Math.cos(Math.PI / 4), Y: 20 + 10 * Math.sin(Math.PI / 4), Z: 0 },
          Transform: {
            Position: { X: 20, Y: 20, Z: 0 },
            Rotation: { X: 0, Y: 0, Z: 0 },
            Scale: { X: 1, Y: 1, Z: 1 },
            Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 20, 20, 0, 1]
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

      const results = ArcEntityThreejsRenderer.renderMultiple(arcDataList, scene);

      expect(results).toHaveLength(1);
      expect(results[0]).toBeInstanceOf(THREE.Line);
    });
  });

  describe('calculateArcLength', () => {
    it('should calculate correct arc length', () => {
      const arcData: ArcData = {
        Type: 'Arc',
        Uuid: 'test-uuid-length',
        EntityType: 'ARC',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 10,
        StartAngle: 0,
        EndAngle: Math.PI,
        Thickness: 0,
        Sweep: Math.PI,
        StartX: 10,
        StartY: 0,
        StartZ: 0,
        EndX: -10,
        EndY: 0,
        EndZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-length',
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
        IsCCW: true,
        Length: 10 * Math.PI,
        Area: 0.5 * 10 * 10 * Math.PI,
        ChordLength: 20,
        Sagitta: 10,
        MidAngle: Math.PI / 2,
        MidX: 0,
        MidY: 10,
        MidZ: 0,
        Bounds: {
          Min: { X: -10, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 0, Y: 10, Z: 0 },
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

      const length = ArcEntityThreejsRenderer.calculateArcLength(arcData);

      expect(length).toBeCloseTo(10 * Math.PI, 5);
    });
  });

  describe('calculateChordLength', () => {
    it('should calculate correct chord length', () => {
      const arcData: ArcData = {
        Type: 'Arc',
        Uuid: 'test-uuid-chord',
        EntityType: 'ARC',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 10,
        StartAngle: 0,
        EndAngle: Math.PI,
        Thickness: 0,
        Sweep: Math.PI,
        StartX: 10,
        StartY: 0,
        StartZ: 0,
        EndX: -10,
        EndY: 0,
        EndZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-chord',
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
        IsCCW: true,
        Length: 10 * Math.PI,
        Area: 0.5 * 10 * 10 * Math.PI,
        ChordLength: 20,
        Sagitta: 10,
        MidAngle: Math.PI / 2,
        MidX: 0,
        MidY: 10,
        MidZ: 0,
        Bounds: {
          Min: { X: -10, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 0, Y: 10, Z: 0 },
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

      const chordLength = ArcEntityThreejsRenderer.calculateChordLength(arcData);

      expect(chordLength).toBeCloseTo(20, 5);
    });
  });

  describe('calculateSagitta', () => {
    it('should calculate correct sagitta', () => {
      const arcData: ArcData = {
        Type: 'Arc',
        Uuid: 'test-uuid-sagitta',
        EntityType: 'ARC',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 10,
        StartAngle: 0,
        EndAngle: Math.PI,
        Thickness: 0,
        Sweep: Math.PI,
        StartX: 10,
        StartY: 0,
        StartZ: 0,
        EndX: -10,
        EndY: 0,
        EndZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-sagitta',
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
        IsCCW: true,
        Length: 10 * Math.PI,
        Area: 0.5 * 10 * 10 * Math.PI,
        ChordLength: 20,
        Sagitta: 10,
        MidAngle: Math.PI / 2,
        MidX: 0,
        MidY: 10,
        MidZ: 0,
        Bounds: {
          Min: { X: -10, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 0, Y: 10, Z: 0 },
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

      const sagitta = ArcEntityThreejsRenderer.calculateSagitta(arcData);

      expect(sagitta).toBeCloseTo(10, 5);
    });
  });

  describe('isPointOnArc', () => {
    it('should return true for point on arc', () => {
      const arcData: ArcData = {
        Type: 'Arc',
        Uuid: 'test-uuid-point',
        EntityType: 'ARC',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 10,
        StartAngle: 0,
        EndAngle: Math.PI / 2,
        Thickness: 0,
        Sweep: Math.PI / 2,
        StartX: 10,
        StartY: 0,
        StartZ: 0,
        EndX: 0,
        EndY: 10,
        EndZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-point',
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
        IsCCW: true,
        Length: 10 * Math.PI / 2,
        Area: 0.5 * 10 * 10 * Math.PI / 2,
        ChordLength: Math.sqrt(200),
        Sagitta: 10 - Math.sqrt(50),
        MidAngle: Math.PI / 4,
        MidX: 10 * Math.cos(Math.PI / 4),
        MidY: 10 * Math.sin(Math.PI / 4),
        MidZ: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 10 * Math.cos(Math.PI / 4), Y: 10 * Math.sin(Math.PI / 4), Z: 0 },
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
      const result = ArcEntityThreejsRenderer.isPointOnArc(arcData, point);

      expect(result).toBe(true);
    });

    it('should return false for point not on arc', () => {
      const arcData: ArcData = {
        Type: 'Arc',
        Uuid: 'test-uuid-point-not',
        EntityType: 'ARC',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 10,
        StartAngle: 0,
        EndAngle: Math.PI / 2,
        Thickness: 0,
        Sweep: Math.PI / 2,
        StartX: 10,
        StartY: 0,
        StartZ: 0,
        EndX: 0,
        EndY: 10,
        EndZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-point-not',
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
        IsCCW: true,
        Length: 10 * Math.PI / 2,
        Area: 0.5 * 10 * 10 * Math.PI / 2,
        ChordLength: Math.sqrt(200),
        Sagitta: 10 - Math.sqrt(50),
        MidAngle: Math.PI / 4,
        MidX: 10 * Math.cos(Math.PI / 4),
        MidY: 10 * Math.sin(Math.PI / 4),
        MidZ: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 10 * Math.cos(Math.PI / 4), Y: 10 * Math.sin(Math.PI / 4), Z: 0 },
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

      const point = new THREE.Vector3(20, 20, 0);
      const result = ArcEntityThreejsRenderer.isPointOnArc(arcData, point);

      expect(result).toBe(false);
    });
  });

  describe('getArcDataFromObject', () => {
    it('should return arc data from object', () => {
      const arcData: ArcData = {
        Type: 'Arc',
        Uuid: 'test-uuid-get',
        EntityType: 'ARC',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 10,
        StartAngle: 0,
        EndAngle: Math.PI / 2,
        Thickness: 0,
        Sweep: Math.PI / 2,
        StartX: 10,
        StartY: 0,
        StartZ: 0,
        EndX: 0,
        EndY: 10,
        EndZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-get',
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
        IsCCW: true,
        Length: 10 * Math.PI / 2,
        Area: 0.5 * 10 * 10 * Math.PI / 2,
        ChordLength: Math.sqrt(200),
        Sagitta: 10 - Math.sqrt(50),
        MidAngle: Math.PI / 4,
        MidX: 10 * Math.cos(Math.PI / 4),
        MidY: 10 * Math.sin(Math.PI / 4),
        MidZ: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 10 * Math.cos(Math.PI / 4), Y: 10 * Math.sin(Math.PI / 4), Z: 0 },
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

      const line = new THREE.Line();
      line.userData = { arcData: arcData };

      const result = ArcEntityThreejsRenderer.getArcDataFromObject(line);

      expect(result).not.toBeNull();
      expect(result?.Uuid).toBe('test-uuid-get');
    });

    it('should return null for object without arc data', () => {
      const line = new THREE.Line();
      line.userData = {};

      const result = ArcEntityThreejsRenderer.getArcDataFromObject(line);

      expect(result).toBeNull();
    });
  });

  describe('dispose', () => {
    it('should dispose geometry and material', () => {
      const arcData: ArcData = {
        Type: 'Arc',
        Uuid: 'test-uuid-dispose',
        EntityType: 'ARC',
        CenterX: 0,
        CenterY: 0,
        CenterZ: 0,
        Radius: 10,
        StartAngle: 0,
        EndAngle: Math.PI / 2,
        Thickness: 0,
        Sweep: Math.PI / 2,
        StartX: 10,
        StartY: 0,
        StartZ: 0,
        EndX: 0,
        EndY: 10,
        EndZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Handle: 'test-handle-dispose',
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
        IsCCW: true,
        Length: 10 * Math.PI / 2,
        Area: 0.5 * 10 * 10 * Math.PI / 2,
        ChordLength: Math.sqrt(200),
        Sagitta: 10 - Math.sqrt(50),
        MidAngle: Math.PI / 4,
        MidX: 10 * Math.cos(Math.PI / 4),
        MidY: 10 * Math.sin(Math.PI / 4),
        MidZ: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 10 * Math.cos(Math.PI / 4), Y: 10 * Math.sin(Math.PI / 4), Z: 0 },
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

      const line = ArcEntityThreejsRenderer.render(arcData, scene) as THREE.Line;

      expect(line).not.toBeNull();
      expect(line.geometry).toBeDefined();
      expect(line.material).toBeDefined();

      ArcEntityThreejsRenderer.dispose(line);
    });
  });
});
