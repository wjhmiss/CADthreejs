import * as THREE from 'three';
import { HatchEntityThreejsRenderer, HatchData } from '../HatchEntityThreejsRenderer';

describe('HatchEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
  });

  describe('render', () => {
    it('should render a solid hatch', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-1',
        EntityType: 'HATCH',
        Handle: 'handle-1',
        LayerName: 'TEST_LAYER',
        Visible: true,
        IsInvisible: false,
        IsSolid: true,
        PatternName: 'SOLID',
        PatternAngle: 0,
        PatternScale: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        PathCount: 1,
        TotalEdges: 4,
        EdgeTypes: ['Line', 'Line', 'Line', 'Line'],
        Points: [
          { X: 0, Y: 0, Z: 0 },
          { X: 10, Y: 0, Z: 0 },
          { X: 10, Y: 10, Z: 0 },
          { X: 0, Y: 10, Z: 0 }
        ],
        Paths: [],
        HasGradient: false,
        GradientColorName: '',
        IsDouble: false,
        Transparency: 0,
        MaterialType: 'MeshBasicMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
      expect(mesh.visible).toBe(true);
      expect(mesh.castShadow).toBe(true);
      expect(mesh.receiveShadow).toBe(true);
      expect(mesh.userData.type).toBe('Hatch');
      expect(mesh.userData.uuid).toBe('test-uuid-1');
      expect(mesh.userData.entityType).toBe('HATCH');
      expect(mesh.userData.handle).toBe('handle-1');
      expect(mesh.userData.layerName).toBe('TEST_LAYER');
    });

    it('should render a pattern hatch', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-2',
        EntityType: 'HATCH',
        Handle: 'handle-2',
        LayerName: 'PATTERN_LAYER',
        Visible: true,
        IsInvisible: false,
        IsSolid: false,
        PatternName: 'ANSI31',
        PatternAngle: Math.PI / 4,
        PatternScale: 2.0,
        ColorIndex: 2,
        ColorHex: '#FFFF00',
        ColorR: 255,
        ColorG: 255,
        ColorB: 0,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        PathCount: 1,
        TotalEdges: 4,
        EdgeTypes: ['Line', 'Line', 'Line', 'Line'],
        Points: [
          { X: 0, Y: 0, Z: 0 },
          { X: 10, Y: 0, Z: 0 },
          { X: 10, Y: 10, Z: 0 },
          { X: 0, Y: 10, Z: 0 }
        ],
        Paths: [],
        HasGradient: false,
        GradientColorName: '',
        IsDouble: false,
        Transparency: 0,
        MaterialType: 'MeshPhongMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
      expect(mesh.visible).toBe(true);
      expect(mesh.userData.patternName).toBe('ANSI31');
      expect(mesh.userData.patternAngle).toBe(Math.PI / 4);
      expect(mesh.userData.patternScale).toBe(2.0);
    });

    it('should return null for invisible hatch', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-3',
        EntityType: 'HATCH',
        Handle: 'handle-3',
        LayerName: 'INVISIBLE_LAYER',
        Visible: true,
        IsInvisible: true,
        IsSolid: true,
        PatternName: 'SOLID',
        PatternAngle: 0,
        PatternScale: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        PathCount: 1,
        TotalEdges: 4,
        EdgeTypes: ['Line', 'Line', 'Line', 'Line'],
        Points: [
          { X: 0, Y: 0, Z: 0 },
          { X: 10, Y: 0, Z: 0 },
          { X: 10, Y: 10, Z: 0 },
          { X: 0, Y: 10, Z: 0 }
        ],
        Paths: [],
        HasGradient: false,
        GradientColorName: '',
        IsDouble: false,
        Transparency: 0,
        MaterialType: 'MeshBasicMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeNull();
    });

    it('should return null for non-visible hatch', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-4',
        EntityType: 'HATCH',
        Handle: 'handle-4',
        LayerName: 'HIDDEN_LAYER',
        Visible: false,
        IsInvisible: false,
        IsSolid: true,
        PatternName: 'SOLID',
        PatternAngle: 0,
        PatternScale: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        PathCount: 1,
        TotalEdges: 4,
        EdgeTypes: ['Line', 'Line', 'Line', 'Line'],
        Points: [
          { X: 0, Y: 0, Z: 0 },
          { X: 10, Y: 0, Z: 0 },
          { X: 10, Y: 10, Z: 0 },
          { X: 0, Y: 10, Z: 0 }
        ],
        Paths: [],
        HasGradient: false,
        GradientColorName: '',
        IsDouble: false,
        Transparency: 0,
        MaterialType: 'MeshBasicMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeNull();
    });

    it('should return null for hatch with no vertices', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-5',
        EntityType: 'HATCH',
        Handle: 'handle-5',
        LayerName: 'NO_VERTICES_LAYER',
        Visible: true,
        IsInvisible: false,
        IsSolid: true,
        PatternName: 'SOLID',
        PatternAngle: 0,
        PatternScale: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        Vertices: [],
        Indices: [],
        Normals: [],
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Area: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        PathCount: 0,
        TotalEdges: 0,
        EdgeTypes: [],
        Points: [],
        Paths: [],
        HasGradient: false,
        GradientColorName: '',
        IsDouble: false,
        Transparency: 0,
        MaterialType: 'MeshBasicMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeNull();
    });

    it('should render hatch with elevation', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-6',
        EntityType: 'HATCH',
        Handle: 'handle-6',
        LayerName: 'ELEVATION_LAYER',
        Visible: true,
        IsInvisible: false,
        IsSolid: true,
        PatternName: 'SOLID',
        PatternAngle: 0,
        PatternScale: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        Vertices: [0, 0, 10, 10, 0, 10, 10, 10, 10, 0, 10, 10],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 10 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 10,
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 10 },
          Max: { X: 10, Y: 10, Z: 10 }
        },
        PathCount: 1,
        TotalEdges: 4,
        EdgeTypes: ['Line', 'Line', 'Line', 'Line'],
        Points: [
          { X: 0, Y: 0, Z: 10 },
          { X: 10, Y: 0, Z: 10 },
          { X: 10, Y: 10, Z: 10 },
          { X: 0, Y: 10, Z: 10 }
        ],
        Paths: [],
        HasGradient: false,
        GradientColorName: '',
        IsDouble: false,
        Transparency: 0,
        MaterialType: 'MeshBasicMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
      expect(mesh.visible).toBe(true);
      expect(mesh.userData.elevation).toBe(10);
    });

    it('should render hatch with transparency', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-7',
        EntityType: 'HATCH',
        Handle: 'handle-7',
        LayerName: 'TRANSPARENT_LAYER',
        Visible: true,
        IsInvisible: false,
        IsSolid: true,
        PatternName: 'SOLID',
        PatternAngle: 0,
        PatternScale: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        PathCount: 1,
        TotalEdges: 4,
        EdgeTypes: ['Line', 'Line', 'Line', 'Line'],
        Points: [
          { X: 0, Y: 0, Z: 0 },
          { X: 10, Y: 0, Z: 0 },
          { X: 10, Y: 10, Z: 0 },
          { X: 0, Y: 10, Z: 0 }
        ],
        Paths: [],
        HasGradient: false,
        GradientColorName: '',
        IsDouble: false,
        Transparency: 0.5,
        MaterialType: 'MeshBasicMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: true,
        MaterialOpacity: 0.5,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
      expect(mesh.visible).toBe(true);
      expect(mesh.material).toBeInstanceOf(THREE.MeshBasicMaterial);
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.transparent).toBe(true);
      expect(material.opacity).toBe(0.5);
    });

    it('should render hatch with gradient', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-8',
        EntityType: 'HATCH',
        Handle: 'handle-8',
        LayerName: 'GRADIENT_LAYER',
        Visible: true,
        IsInvisible: false,
        IsSolid: true,
        PatternName: 'SOLID',
        PatternAngle: 0,
        PatternScale: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        PathCount: 1,
        TotalEdges: 4,
        EdgeTypes: ['Line', 'Line', 'Line', 'Line'],
        Points: [
          { X: 0, Y: 0, Z: 0 },
          { X: 10, Y: 0, Z: 0 },
          { X: 10, Y: 10, Z: 0 },
          { X: 0, Y: 10, Z: 0 }
        ],
        Paths: [],
        HasGradient: true,
        GradientColorName: 'GRADIENT1',
        IsDouble: false,
        Transparency: 0,
        MaterialType: 'MeshBasicMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
      expect(mesh.visible).toBe(true);
      expect(mesh.userData.hasGradient).toBe(true);
      expect(mesh.userData.gradientColorName).toBe('GRADIENT1');
    });

    it('should render hatch with double pattern', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-9',
        EntityType: 'HATCH',
        Handle: 'handle-9',
        LayerName: 'DOUBLE_LAYER',
        Visible: true,
        IsInvisible: false,
        IsSolid: false,
        PatternName: 'ANSI31',
        PatternAngle: 0,
        PatternScale: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        PathCount: 1,
        TotalEdges: 4,
        EdgeTypes: ['Line', 'Line', 'Line', 'Line'],
        Points: [
          { X: 0, Y: 0, Z: 0 },
          { X: 10, Y: 0, Z: 0 },
          { X: 10, Y: 10, Z: 0 },
          { X: 0, Y: 10, Z: 0 }
        ],
        Paths: [],
        HasGradient: false,
        GradientColorName: '',
        IsDouble: true,
        Transparency: 0,
        MaterialType: 'MeshPhongMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
      expect(mesh.visible).toBe(true);
      expect(mesh.userData.isDouble).toBe(true);
    });

    it('should apply transform matrix when provided', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-10',
        EntityType: 'HATCH',
        Handle: 'handle-10',
        LayerName: 'TRANSFORM_LAYER',
        Visible: true,
        IsInvisible: false,
        IsSolid: true,
        PatternName: 'SOLID',
        PatternAngle: 0,
        PatternScale: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        PathCount: 1,
        TotalEdges: 4,
        EdgeTypes: ['Line', 'Line', 'Line', 'Line'],
        Points: [
          { X: 0, Y: 0, Z: 0 },
          { X: 10, Y: 0, Z: 0 },
          { X: 10, Y: 10, Z: 0 },
          { X: 0, Y: 10, Z: 0 }
        ],
        Paths: [],
        HasGradient: false,
        GradientColorName: '',
        IsDouble: false,
        Transparency: 0,
        MaterialType: 'MeshBasicMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Transform: {
          Matrix: [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            5, 5, 0, 1
          ],
          Position: { X: 5, Y: 5, Z: 0 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
      expect(mesh.visible).toBe(true);
      expect(mesh.position.x).toBe(5);
      expect(mesh.position.y).toBe(5);
      expect(mesh.position.z).toBe(0);
    });

    it('should create correct geometry for hatch', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-11',
        EntityType: 'HATCH',
        Handle: 'handle-11',
        LayerName: 'GEOMETRY_LAYER',
        Visible: true,
        IsInvisible: false,
        IsSolid: true,
        PatternName: 'SOLID',
        PatternAngle: 0,
        PatternScale: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        PathCount: 1,
        TotalEdges: 4,
        EdgeTypes: ['Line', 'Line', 'Line', 'Line'],
        Points: [
          { X: 0, Y: 0, Z: 0 },
          { X: 10, Y: 0, Z: 0 },
          { X: 10, Y: 10, Z: 0 },
          { X: 0, Y: 10, Z: 0 }
        ],
        Paths: [],
        HasGradient: false,
        GradientColorName: '',
        IsDouble: false,
        Transparency: 0,
        MaterialType: 'MeshBasicMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.geometry).toBeInstanceOf(THREE.BufferGeometry);
      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positionAttribute = geometry.getAttribute('position');
      expect(positionAttribute).toBeDefined();
      expect(positionAttribute.count).toBe(4);
      const normalAttribute = geometry.getAttribute('normal');
      expect(normalAttribute).toBeDefined();
      expect(normalAttribute.count).toBe(4);
    });

    it('should create correct material for solid hatch', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-12',
        EntityType: 'HATCH',
        Handle: 'handle-12',
        LayerName: 'SOLID_MATERIAL_LAYER',
        Visible: true,
        IsInvisible: false,
        IsSolid: true,
        PatternName: 'SOLID',
        PatternAngle: 0,
        PatternScale: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        PathCount: 1,
        TotalEdges: 4,
        EdgeTypes: ['Line', 'Line', 'Line', 'Line'],
        Points: [
          { X: 0, Y: 0, Z: 0 },
          { X: 10, Y: 0, Z: 0 },
          { X: 10, Y: 10, Z: 0 },
          { X: 0, Y: 10, Z: 0 }
        ],
        Paths: [],
        HasGradient: false,
        GradientColorName: '',
        IsDouble: false,
        Transparency: 0,
        MaterialType: 'MeshBasicMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.material).toBeInstanceOf(THREE.MeshBasicMaterial);
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.color.getHex()).toBe(0xFF0000);
      expect(material.side).toBe(THREE.DoubleSide);
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(true);
    });

    it('should create correct material for pattern hatch', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-13',
        EntityType: 'HATCH',
        Handle: 'handle-13',
        LayerName: 'PATTERN_MATERIAL_LAYER',
        Visible: true,
        IsInvisible: false,
        IsSolid: false,
        PatternName: 'ANSI31',
        PatternAngle: Math.PI / 4,
        PatternScale: 2.0,
        ColorIndex: 2,
        ColorHex: '#FFFF00',
        ColorR: 255,
        ColorG: 255,
        ColorB: 0,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        PathCount: 1,
        TotalEdges: 4,
        EdgeTypes: ['Line', 'Line', 'Line', 'Line'],
        Points: [
          { X: 0, Y: 0, Z: 0 },
          { X: 10, Y: 0, Z: 0 },
          { X: 10, Y: 10, Z: 0 },
          { X: 0, Y: 10, Z: 0 }
        ],
        Paths: [],
        HasGradient: false,
        GradientColorName: '',
        IsDouble: false,
        Transparency: 0,
        MaterialType: 'MeshPhongMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.material).toBeInstanceOf(THREE.MeshPhongMaterial);
      const material = mesh.material as THREE.MeshPhongMaterial;
      expect(material.color.getHex()).toBe(0xFFFF00);
      expect(material.side).toBe(THREE.DoubleSide);
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(true);
    });

    it('should add mesh to scene', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-14',
        EntityType: 'HATCH',
        Handle: 'handle-14',
        LayerName: 'SCENE_LAYER',
        Visible: true,
        IsInvisible: false,
        IsSolid: true,
        PatternName: 'SOLID',
        PatternAngle: 0,
        PatternScale: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        PathCount: 1,
        TotalEdges: 4,
        EdgeTypes: ['Line', 'Line', 'Line', 'Line'],
        Points: [
          { X: 0, Y: 0, Z: 0 },
          { X: 10, Y: 0, Z: 0 },
          { X: 10, Y: 10, Z: 0 },
          { X: 0, Y: 10, Z: 0 }
        ],
        Paths: [],
        HasGradient: false,
        GradientColorName: '',
        IsDouble: false,
        Transparency: 0,
        MaterialType: 'MeshBasicMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(scene.children).toContain(mesh);
    });

    it('should handle null or undefined hatch data', () => {
      const mesh1 = HatchEntityThreejsRenderer.render(null as any, scene);
      expect(mesh1).toBeNull();

      const mesh2 = HatchEntityThreejsRenderer.render(undefined as any, scene);
      expect(mesh2).toBeNull();
    });

    it('should set correct render order', () => {
      const hatchData: HatchData = {
        Type: 'Hatch',
        Uuid: 'test-uuid-15',
        EntityType: 'HATCH',
        Handle: 'handle-15',
        LayerName: 'RENDER_ORDER_LAYER',
        Visible: true,
        IsInvisible: false,
        IsSolid: true,
        PatternName: 'SOLID',
        PatternAngle: 0,
        PatternScale: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        Vertices: [0, 0, 0, 10, 0, 0, 10, 10, 0, 0, 10, 0],
        Indices: [0, 1, 2, 0, 2, 3],
        Normals: [0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Area: 100,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        PathCount: 1,
        TotalEdges: 4,
        EdgeTypes: ['Line', 'Line', 'Line', 'Line'],
        Points: [
          { X: 0, Y: 0, Z: 0 },
          { X: 10, Y: 0, Z: 0 },
          { X: 10, Y: 10, Z: 0 },
          { X: 0, Y: 10, Z: 0 }
        ],
        Paths: [],
        HasGradient: false,
        GradientColorName: '',
        IsDouble: false,
        Transparency: 0,
        MaterialType: 'MeshBasicMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 5
      };

      const mesh = HatchEntityThreejsRenderer.render(hatchData, scene);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.renderOrder).toBe(5);
    });
  });
});
