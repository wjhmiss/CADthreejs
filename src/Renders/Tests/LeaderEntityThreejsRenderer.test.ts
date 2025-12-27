import * as THREE from 'three';
import { LeaderEntityThreejsRenderer, LeaderData, Point3DData, PointData, BoundsData, ColorData, TransformData } from '../LeaderEntityThreejsRenderer';

describe('LeaderEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
  });

  const createBasicLeaderData = (): LeaderData => ({
    Type: 'Leader',
    Handle: 'handle-1',
    LayerName: 'TEST_LAYER',
    Visible: true,
    CoordinateSystem: 'World',
    Vertices: [],
    Vertices3D: [],
    PathType: 'StraightLineSegments',
    ArrowHeadEnabled: false,
    HasHookline: false,
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.25,
    ArrowHeadPoints: [],
    ArrowHeadPoints3D: [],
    HookLineStart: null as any,
    HookLineEnd: null as any,
    HookLineStart3D: null as any,
    HookLineEnd3D: null as any,
    TotalLength: 0,
    StartPoint: null as any,
    EndPoint: null as any,
    StartPoint3D: null as any,
    EndPoint3D: null as any,
    VertexCount: 0,
    Bounds: null as any,
    Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 },
    Transform3D: {
      Position: { X: 0, Y: 0, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
    },
    Normal: { X: 0, Y: 0, Z: 1 },
    HorizontalDirection: { X: 1, Y: 0, Z: 0 },
    AnnotationOffset: { X: 0, Y: 0, Z: 0 },
    BlockOffset: { X: 0, Y: 0, Z: 0 },
    CreationType: 'CreatedWithTextAnnotation',
    HookLineDirection: 'Opposite',
    TextHeight: 2.5,
    TextWidth: 5.0,
    StyleName: 'Standard',
    Opacity: 1.0,
    Transparent: false,
    MaterialType: 'LineBasicMaterial',
    DepthTest: true,
    DepthWrite: true
  });

  describe('render', () => {
    it('should render a basic leader', () => {
      const leaderData = createBasicLeaderData();

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.userData.type).toBe('Leader');
      expect(group.userData.handle).toBe('handle-1');
      expect(group.userData.layerName).toBe('TEST_LAYER');
    });

    it('should return null for invisible leader', () => {
      const leaderData = createBasicLeaderData();
      leaderData.Visible = false;

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeNull();
    });

    it('should return null for null data', () => {
      const group = LeaderEntityThreejsRenderer.render(null as any, scene);

      expect(group).toBeNull();
    });

    it('should render leader with vertices', () => {
      const leaderData = createBasicLeaderData();
      leaderData.Vertices = [
        { X: 0, Y: 0 },
        { X: 10, Y: 10 },
        { X: 20, Y: 10 }
      ];
      leaderData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 10, Z: 0 },
        { X: 20, Y: 10, Z: 0 }
      ];
      leaderData.VertexCount = 3;
      leaderData.TotalLength = 24.1421;
      leaderData.StartPoint = { X: 0, Y: 0 };
      leaderData.EndPoint = { X: 20, Y: 10 };
      leaderData.StartPoint3D = { X: 0, Y: 0, Z: 0 };
      leaderData.EndPoint3D = { X: 20, Y: 10, Z: 0 };
      leaderData.Bounds = {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 20, Y: 10, Z: 0 },
        Center: { X: 10, Y: 5, Z: 0 },
        Size: { X: 20, Y: 10, Z: 0 }
      };

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBeGreaterThan(0);
    });

    it('should render leader with arrow head', () => {
      const leaderData = createBasicLeaderData();
      leaderData.ArrowHeadEnabled = true;
      leaderData.Vertices = [
        { X: 0, Y: 0 },
        { X: 10, Y: 10 }
      ];
      leaderData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 10, Z: 0 }
      ];
      leaderData.ArrowHeadPoints = [
        { X: 0, Y: 0 },
        { X: -2, Y: 1 },
        { X: -2, Y: -1 }
      ];
      leaderData.ArrowHeadPoints3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: -2, Y: 1, Z: 0 },
        { X: -2, Y: -1, Z: 0 }
      ];
      leaderData.VertexCount = 2;

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBeGreaterThan(0);
    });

    it('should render leader with hook line', () => {
      const leaderData = createBasicLeaderData();
      leaderData.HasHookline = true;
      leaderData.Vertices = [
        { X: 0, Y: 0 },
        { X: 10, Y: 10 },
        { X: 30, Y: 10 }
      ];
      leaderData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 10, Z: 0 },
        { X: 30, Y: 10, Z: 0 }
      ];
      leaderData.HookLineStart = { X: 30, Y: 10 };
      leaderData.HookLineEnd = { X: 30, Y: 13 };
      leaderData.HookLineStart3D = { X: 30, Y: 10, Z: 0 };
      leaderData.HookLineEnd3D = { X: 30, Y: 13, Z: 0 };
      leaderData.VertexCount = 3;

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBeGreaterThan(0);
    });

    it('should render leader with both arrow head and hook line', () => {
      const leaderData = createBasicLeaderData();
      leaderData.ArrowHeadEnabled = true;
      leaderData.HasHookline = true;
      leaderData.Vertices = [
        { X: 0, Y: 0 },
        { X: 10, Y: 10 },
        { X: 30, Y: 10 }
      ];
      leaderData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 10, Z: 0 },
        { X: 30, Y: 10, Z: 0 }
      ];
      leaderData.ArrowHeadPoints = [
        { X: 0, Y: 0 },
        { X: -2, Y: 1 },
        { X: -2, Y: -1 }
      ];
      leaderData.ArrowHeadPoints3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: -2, Y: 1, Z: 0 },
        { X: -2, Y: -1, Z: 0 }
      ];
      leaderData.HookLineStart = { X: 30, Y: 10 };
      leaderData.HookLineEnd = { X: 30, Y: 13 };
      leaderData.HookLineStart3D = { X: 30, Y: 10, Z: 0 };
      leaderData.HookLineEnd3D = { X: 30, Y: 13, Z: 0 };
      leaderData.VertexCount = 3;

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBeGreaterThan(0);
    });

    it('should handle leader with spline path type', () => {
      const leaderData = createBasicLeaderData();
      leaderData.PathType = 'Spline';
      leaderData.Vertices = [
        { X: 0, Y: 0 },
        { X: 10, Y: 10 },
        { X: 20, Y: 10 }
      ];
      leaderData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 10, Z: 0 },
        { X: 20, Y: 10, Z: 0 }
      ];
      leaderData.VertexCount = 3;

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.pathType).toBe('Spline');
    });

    it('should handle leader with different colors', () => {
      const leaderData = createBasicLeaderData();
      leaderData.ColorIndex = 5;
      leaderData.Color = { Index: 5, Hex: '#0000FF', R: 0, G: 0, B: 255, A: 1.0 };

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.colorIndex).toBe(5);
    });

    it('should handle leader with transparency', () => {
      const leaderData = createBasicLeaderData();
      leaderData.Opacity = 0.5;
      leaderData.Transparent = true;

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.opacity).toBe(0.5);
      expect(group.userData.transparent).toBe(true);
    });

    it('should handle leader with 3D vertices', () => {
      const leaderData = createBasicLeaderData();
      leaderData.Vertices = [
        { X: 0, Y: 0 },
        { X: 10, Y: 10 }
      ];
      leaderData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 10, Z: 10 }
      ];
      leaderData.VertexCount = 2;
      leaderData.StartPoint3D = { X: 0, Y: 0, Z: 0 };
      leaderData.EndPoint3D = { X: 10, Y: 10, Z: 10 };

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBeGreaterThan(0);
    });

    it('should handle leader with bounds', () => {
      const leaderData = createBasicLeaderData();
      leaderData.Vertices = [
        { X: 0, Y: 0 },
        { X: 10, Y: 10 }
      ];
      leaderData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 10, Z: 0 }
      ];
      leaderData.VertexCount = 2;
      leaderData.Bounds = {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 10, Y: 10, Z: 0 },
        Center: { X: 5, Y: 5, Z: 0 },
        Size: { X: 10, Y: 10, Z: 0 }
      };

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.bounds).toBeDefined();
    });

    it('should handle leader with different line weights', () => {
      const leaderData = createBasicLeaderData();
      leaderData.LineWeight = 0.5;

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.lineWeight).toBe(0.5);
    });

    it('should handle leader with different line types', () => {
      const leaderData = createBasicLeaderData();
      leaderData.LineTypeName = 'DASHED';

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.lineTypeName).toBe('DASHED');
    });

    it('should handle leader with different creation types', () => {
      const leaderData = createBasicLeaderData();
      leaderData.CreationType = 'CreatedWithoutTextAnnotation';

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.creationType).toBe('CreatedWithoutTextAnnotation');
    });

    it('should handle leader with different hook line directions', () => {
      const leaderData = createBasicLeaderData();
      leaderData.HookLineDirection = 'Along';

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.hookLineDirection).toBe('Along');
    });

    it('should handle leader with text height and width', () => {
      const leaderData = createBasicLeaderData();
      leaderData.TextHeight = 3.5;
      leaderData.TextWidth = 7.0;

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.textHeight).toBe(3.5);
      expect(group.userData.textWidth).toBe(7.0);
    });

    it('should handle leader with different style names', () => {
      const leaderData = createBasicLeaderData();
      leaderData.StyleName = 'MY_STYLE';

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.styleName).toBe('MY_STYLE');
    });

    it('should handle leader with depth test and write settings', () => {
      const leaderData = createBasicLeaderData();
      leaderData.DepthTest = false;
      leaderData.DepthWrite = false;

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.depthTest).toBe(false);
      expect(group.userData.depthWrite).toBe(false);
    });

    it('should handle leader with normal vector', () => {
      const leaderData = createBasicLeaderData();
      leaderData.Normal = { X: 0, Y: 1, Z: 0 };

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.normal).toBeDefined();
    });

    it('should handle leader with horizontal direction', () => {
      const leaderData = createBasicLeaderData();
      leaderData.HorizontalDirection = { X: -1, Y: 0, Z: 0 };

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.horizontalDirection).toBeDefined();
    });

    it('should handle leader with annotation offset', () => {
      const leaderData = createBasicLeaderData();
      leaderData.AnnotationOffset = { X: 5, Y: 5, Z: 0 };

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.annotationOffset).toBeDefined();
    });

    it('should handle leader with block offset', () => {
      const leaderData = createBasicLeaderData();
      leaderData.BlockOffset = { X: 2, Y: 2, Z: 0 };

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.blockOffset).toBeDefined();
    });

    it('should handle leader with transform data', () => {
      const leaderData = createBasicLeaderData();
      leaderData.Transform3D = {
        Position: { X: 10, Y: 10, Z: 0 },
        Rotation: { X: 0, Y: 0, Z: 0.5 },
        Scale: { X: 2, Y: 2, Z: 1 },
        Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
      };

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.transform).toBeDefined();
    });

    it('should handle leader with total length', () => {
      const leaderData = createBasicLeaderData();
      leaderData.Vertices = [
        { X: 0, Y: 0 },
        { X: 10, Y: 10 }
      ];
      leaderData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 10, Z: 0 }
      ];
      leaderData.VertexCount = 2;
      leaderData.TotalLength = 14.1421;

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.totalLength).toBe(14.1421);
    });
  });

  describe('dispose', () => {
    it('should dispose leader and its children', () => {
      const leaderData = createBasicLeaderData();
      leaderData.Vertices = [
        { X: 0, Y: 0 },
        { X: 10, Y: 10 }
      ];
      leaderData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 10, Z: 0 }
      ];
      leaderData.VertexCount = 2;

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);
      expect(group).not.toBeNull();

      LeaderEntityThreejsRenderer.dispose(leaderData, scene);

      expect(group.children.length).toBe(0);
    });

    it('should handle null group', () => {
      expect(() => {
        LeaderEntityThreejsRenderer.dispose(null as any, scene);
      }).not.toThrow();
    });
  });

  describe('integration tests', () => {
    it('should render and dispose multiple leaders', () => {
      const leaderData1 = createBasicLeaderData();
      leaderData1.Handle = 'handle-1';
      leaderData1.Vertices = [
        { X: 0, Y: 0 },
        { X: 10, Y: 10 }
      ];
      leaderData1.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 10, Z: 0 }
      ];
      leaderData1.VertexCount = 2;

      const leaderData2 = createBasicLeaderData();
      leaderData2.Handle = 'handle-2';
      leaderData2.Vertices = [
        { X: 20, Y: 20 },
        { X: 30, Y: 30 }
      ];
      leaderData2.Vertices3D = [
        { X: 20, Y: 20, Z: 0 },
        { X: 30, Y: 30, Z: 0 }
      ];
      leaderData2.VertexCount = 2;

      const group1 = LeaderEntityThreejsRenderer.render(leaderData1, scene);
      const group2 = LeaderEntityThreejsRenderer.render(leaderData2, scene);

      expect(group1).toBeInstanceOf(THREE.Group);
      expect(group2).toBeInstanceOf(THREE.Group);
      expect(group1.userData.handle).toBe('handle-1');
      expect(group2.userData.handle).toBe('handle-2');

      LeaderEntityThreejsRenderer.dispose(leaderData1, scene);
      LeaderEntityThreejsRenderer.dispose(leaderData2, scene);

      expect(group1.children.length).toBe(0);
      expect(group2.children.length).toBe(0);
    });

    it('should handle complex leader with all features', () => {
      const leaderData = createBasicLeaderData();
      leaderData.ArrowHeadEnabled = true;
      leaderData.HasHookline = true;
      leaderData.Vertices = [
        { X: 0, Y: 0 },
        { X: 10, Y: 10 },
        { X: 30, Y: 10 }
      ];
      leaderData.Vertices3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 10, Y: 10, Z: 0 },
        { X: 30, Y: 10, Z: 0 }
      ];
      leaderData.ArrowHeadPoints = [
        { X: 0, Y: 0 },
        { X: -2, Y: 1 },
        { X: -2, Y: -1 }
      ];
      leaderData.ArrowHeadPoints3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: -2, Y: 1, Z: 0 },
        { X: -2, Y: -1, Z: 0 }
      ];
      leaderData.HookLineStart = { X: 30, Y: 10 };
      leaderData.HookLineEnd = { X: 30, Y: 13 };
      leaderData.HookLineStart3D = { X: 30, Y: 10, Z: 0 };
      leaderData.HookLineEnd3D = { X: 30, Y: 13, Z: 0 };
      leaderData.VertexCount = 3;
      leaderData.TotalLength = 34.1421;
      leaderData.StartPoint = { X: 0, Y: 0 };
      leaderData.EndPoint = { X: 30, Y: 10 };
      leaderData.StartPoint3D = { X: 0, Y: 0, Z: 0 };
      leaderData.EndPoint3D = { X: 30, Y: 10, Z: 0 };
      leaderData.Bounds = {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 30, Y: 13, Z: 0 },
        Center: { X: 15, Y: 6.5, Z: 0 },
        Size: { X: 30, Y: 13, Z: 0 }
      };
      leaderData.Opacity = 0.8;
      leaderData.Transparent = true;

      const group = LeaderEntityThreejsRenderer.render(leaderData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBeGreaterThan(0);
      expect(group.userData.arrowHeadEnabled).toBe(true);
      expect(group.userData.hasHookline).toBe(true);
      expect(group.userData.opacity).toBe(0.8);
      expect(group.userData.transparent).toBe(true);

      LeaderEntityThreejsRenderer.dispose(leaderData, scene);
      expect(group.children.length).toBe(0);
    });
  });
});
