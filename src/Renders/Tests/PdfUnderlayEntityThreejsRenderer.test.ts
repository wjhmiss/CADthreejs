import * as THREE from 'three';
import { PdfUnderlayEntityThreejsRenderer, PdfUnderlayData, Point3DData, PointData, BoundsData, ColorData, TransformData, NormalData, UnderlayMaterialData, UnderlayGeometryData } from '../PdfUnderlayEntityThreejsRenderer';

describe('PdfUnderlayEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    
    global.document = {
      createElement: () => ({
        width: 1024,
        height: 1024,
        getContext: () => ({
          clearRect: () => {},
          fillStyle: '',
          fillRect: () => {},
          font: '',
          textAlign: '',
          textBaseline: '',
          fillText: () => {}
        })
      })
    } as any;
  });

  afterEach(() => {
    PdfUnderlayEntityThreejsRenderer.clearCache();
  });

  const createBasicPdfUnderlayData = (): PdfUnderlayData => ({
    Type: 'Underlay',
    EntityType: 'PdfUnderlay',
    Handle: 'pdfunderlay-handle-1',
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Visible: true,
    CoordinateSystem: 'WCS',
    BoundaryPoints: [
      { X: 0, Y: 0 },
      { X: 100, Y: 100 }
    ],
    InsertPoint: { X: 10, Y: 10 },
    XScale: 1.0,
    YScale: 1.0,
    ZScale: 1.0,
    Rotation: 0,
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.03,
    Contrast: 50,
    Fade: 0,
    Flags: 0,
    DefinitionFileName: 'test.pdf',
    BoundaryPoints3D: [
      { X: 0, Y: 0, Z: 0 },
      { X: 100, Y: 0, Z: 0 },
      { X: 100, Y: 100, Z: 0 },
      { X: 0, Y: 100, Z: 0 }
    ],
    InsertPoint3D: { X: 10, Y: 10, Z: 0 },
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
    DepthTest: true,
    PageNumber: '1',
    FilePath: 'test.pdf',
    ClippingBoundaryType: 'Rectangle',
    Material: {
      Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 },
      Opacity: 1.0,
      Transparent: false,
      Contrast: 50,
      Fade: 0,
      Type: 'UnderlayMaterial',
      DepthTest: true,
      Side: true
    },
    Geometry: {
      Vertices: [
        { X: 0, Y: 0, Z: 0 },
        { X: 100, Y: 0, Z: 0 },
        { X: 100, Y: 100, Z: 0 },
        { X: 0, Y: 100, Z: 0 }
      ],
      Indices: [0, 1, 2, 0, 2, 3],
      Position: { X: 10, Y: 10, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      Normal: { X: 0, Y: 0, Z: 1 },
      Type: 'UnderlayGeometry',
      Bounds: {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 100, Y: 100, Z: 0 },
        Center: { X: 50, Y: 50, Z: 0 },
        Size: { X: 100, Y: 100, Z: 0 }
      },
      VertexCount: 4,
      IndexCount: 6
    }
  });

  describe('render', () => {
    it('should render a basic PdfUnderlay', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.userData.type).toBe('Underlay');
      expect(group.userData.entityType).toBe('PdfUnderlay');
      expect(group.userData.handle).toBe('pdfunderlay-handle-1');
      expect(group.userData.layerName).toBe('TEST_LAYER');
    });

    it('should return null for invisible PdfUnderlay', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Visible = false;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeNull();
    });

    it('should return null for null data', () => {
      const group = PdfUnderlayEntityThreejsRenderer.render(null as any, scene);

      expect(group).toBeNull();
    });

    it('should return null for PdfUnderlay without file path', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.FilePath = '';

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeNull();
    });

    it('should render PdfUnderlay with correct underlay mesh', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBeGreaterThan(0);
      
      await new Promise(resolve => setTimeout(resolve, 100));
      
      const underlayMesh = group.children.find(child => child.name === 'Underlay');
      expect(underlayMesh).toBeDefined();
      expect(underlayMesh).toBeInstanceOf(THREE.Mesh);
    });

    it('should render PdfUnderlay with correct material', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);
      
      await new Promise(resolve => setTimeout(resolve, 100));
      
      const underlayMesh = group.children.find(child => child.name === 'Underlay') as THREE.Mesh;

      expect(underlayMesh.material).toBeDefined();
      expect(underlayMesh.material).toBeInstanceOf(THREE.MeshBasicMaterial);
      const material = underlayMesh.material as THREE.MeshBasicMaterial;
      expect(material.transparent).toBe(false);
      expect(material.opacity).toBe(1.0);
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(false);
    });

    it('should render PdfUnderlay with correct geometry', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);
      
      await new Promise(resolve => setTimeout(resolve, 100));
      
      const underlayMesh = group.children.find(child => child.name === 'Underlay') as THREE.Mesh;

      expect(underlayMesh.geometry).toBeDefined();
      expect(underlayMesh.geometry).toBeInstanceOf(THREE.BufferGeometry);
    });

    it('should apply transform matrix if provided', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Transform.Matrix = [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 10, 20, 30, 1];

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group.position.x).toBe(10);
      expect(group.position.y).toBe(20);
      expect(group.position.z).toBe(30);
    });

    it('should render PdfUnderlay with clipping boundary', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      
      const clippingBoundary = group.children.find(child => child.name === 'ClippingBoundary');
      expect(clippingBoundary).toBeDefined();
      expect(clippingBoundary).toBeInstanceOf(THREE.Line);
    });

    it('should render PdfUnderlay with bounds', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      
      const boundsLine = group.children.find(child => child.name === 'Bounds');
      expect(boundsLine).toBeDefined();
    });

    it('should render PdfUnderlay with correct rotation', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Rotation = Math.PI / 4;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.rotation).toBe(Math.PI / 4);
    });

    it('should render PdfUnderlay with correct scale', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.XScale = 2.0;
      pdfUnderlayData.YScale = 1.5;
      pdfUnderlayData.ZScale = 0.5;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.xScale).toBe(2.0);
      expect(group.userData.yScale).toBe(1.5);
      expect(group.userData.zScale).toBe(0.5);
    });

    it('should render PdfUnderlay with correct contrast', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Contrast = 75;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.contrast).toBe(75);
    });

    it('should render PdfUnderlay with correct fade', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Fade = 30;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.fade).toBe(30);
    });

    it('should render PdfUnderlay with correct page number', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.PageNumber = '3';

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.pageNumber).toBe('3');
    });

    it('should render PdfUnderlay with correct file path', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.FilePath = 'example.pdf';

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.filePath).toBe('example.pdf');
    });

    it('should render PdfUnderlay with correct clipping boundary type', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.ClippingBoundaryType = 'Polygon';

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.clippingBoundaryType).toBe('Polygon');
    });

    it('should render PdfUnderlay with correct 3D insert point', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.InsertPoint3D = { X: 10, Y: 20, Z: 30 };

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.insertPoint3D).toEqual({ X: 10, Y: 20, Z: 30 });
    });

    it('should render PdfUnderlay with correct normal', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Normal = { X: 0, Y: 1, Z: 0 };

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.normal).toEqual({ X: 0, Y: 1, Z: 0 });
    });

    it('should render PdfUnderlay with correct color', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.ColorIndex = 5;
      pdfUnderlayData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.colorIndex).toBe(5);
      expect(group.userData.color).toEqual({ Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 });
    });

    it('should render PdfUnderlay with correct opacity', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Opacity = 0.7;
      pdfUnderlayData.Transparent = true;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.opacity).toBe(0.7);
      expect(group.userData.transparent).toBe(true);
    });

    it('should render PdfUnderlay with correct depth settings', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.DepthTest = false;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.depthTest).toBe(false);
    });

    it('should render PdfUnderlay with polygon clipping boundary', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.ClippingBoundaryType = 'Polygon';
      pdfUnderlayData.BoundaryPoints3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 100, Y: 0, Z: 0 },
        { X: 100, Y: 50, Z: 0 },
        { X: 150, Y: 50, Z: 0 },
        { X: 150, Y: 100, Z: 0 },
        { X: 0, Y: 100, Z: 0 }
      ];

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.clippingBoundaryType).toBe('Polygon');
      expect(group.userData.boundaryPoints3D).toHaveLength(6);
    });

    it('should not render clipping boundary when type is None', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.ClippingBoundaryType = 'None';

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      
      const clippingBoundary = group.children.find(child => child.name === 'ClippingBoundary');
      expect(clippingBoundary).toBeUndefined();
    });

    it('should render PdfUnderlay with correct flags', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Flags = 15;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.flags).toBe(15);
    });

    it('should render PdfUnderlay with correct definition file name', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.DefinitionFileName = 'mydrawing.pdf';

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.definitionFileName).toBe('mydrawing.pdf');
    });

    it('should render PdfUnderlay with correct line type name', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.LineTypeName = 'DASHED';

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.lineTypeName).toBe('DASHED');
    });

    it('should render PdfUnderlay with correct line weight', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.LineWeight = 0.05;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.lineWeight).toBe(0.05);
    });

    it('should render PdfUnderlay with correct coordinate system', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.CoordinateSystem = 'OCS';

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.coordinateSystem).toBe('OCS');
    });

    it('should render PdfUnderlay with correct layer index', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.LayerIndex = 5;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.layerIndex).toBe(5);
    });

    it('should render PdfUnderlay with correct boundary points', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.BoundaryPoints = [
        { X: 0, Y: 0 },
        { X: 100, Y: 100 }
      ];

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.boundaryPoints).toHaveLength(2);
    });

    it('should render PdfUnderlay with correct insert point', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.InsertPoint = { X: 50, Y: 75 };

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.insertPoint).toEqual({ X: 50, Y: 75 });
    });

    it('should render PdfUnderlay with correct geometry bounds', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Geometry.Bounds = {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 200, Y: 150, Z: 0 },
        Center: { X: 100, Y: 75, Z: 0 },
        Size: { X: 200, Y: 150, Z: 0 }
      };

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.geometry.Bounds).toBeDefined();
      expect(group.userData.geometry.Bounds.Size.X).toBe(200);
      expect(group.userData.geometry.Bounds.Size.Y).toBe(150);
    });
  });

  describe('update', () => {
    it('should update existing PdfUnderlay', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      const updatedPdfUnderlayData = { ...pdfUnderlayData };
      updatedPdfUnderlayData.Opacity = 0.5;

      const result = PdfUnderlayEntityThreejsRenderer.update(updatedPdfUnderlayData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent PdfUnderlay', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const result = PdfUnderlayEntityThreejsRenderer.update(pdfUnderlayData, scene);

      expect(result).toBe(false);
    });

    it('should update PdfUnderlay visibility', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      const updatedPdfUnderlayData = { ...pdfUnderlayData };
      updatedPdfUnderlayData.Visible = false;

      const result = PdfUnderlayEntityThreejsRenderer.update(updatedPdfUnderlayData, scene);

      expect(result).toBe(true);
      expect(group.visible).toBe(false);
    });

    it('should update PdfUnderlay opacity', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      const updatedPdfUnderlayData = { ...pdfUnderlayData };
      updatedPdfUnderlayData.Opacity = 0.3;

      const result = PdfUnderlayEntityThreejsRenderer.update(updatedPdfUnderlayData, scene);

      expect(result).toBe(true);
    });

    it('should update PdfUnderlay transparency', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      const updatedPdfUnderlayData = { ...pdfUnderlayData };
      updatedPdfUnderlayData.Transparent = true;

      const result = PdfUnderlayEntityThreejsRenderer.update(updatedPdfUnderlayData, scene);

      expect(result).toBe(true);
    });
  });

  describe('dispose', () => {
    it('should dispose PdfUnderlay resources', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      const result = PdfUnderlayEntityThreejsRenderer.dispose(pdfUnderlayData, scene);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(0);
    });

    it('should return false for non-existent PdfUnderlay', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const result = PdfUnderlayEntityThreejsRenderer.dispose(pdfUnderlayData, scene);

      expect(result).toBe(false);
    });

    it('should handle null data gracefully', () => {
      const result = PdfUnderlayEntityThreejsRenderer.dispose(null as any, scene);

      expect(result).toBe(false);
    });

    it('should handle null scene gracefully', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const result = PdfUnderlayEntityThreejsRenderer.dispose(pdfUnderlayData, null as any);

      expect(result).toBe(false);
    });

    it('should dispose PdfUnderlay with multiple children', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      const result = PdfUnderlayEntityThreejsRenderer.dispose(pdfUnderlayData, scene);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(0);
    });
  });

  describe('setVisibility', () => {
    it('should set PdfUnderlay visibility to true', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      const result = PdfUnderlayEntityThreejsRenderer.setVisibility(pdfUnderlayData, scene, true);

      expect(result).toBe(true);
      expect(group.visible).toBe(true);
    });

    it('should set PdfUnderlay visibility to false', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      const result = PdfUnderlayEntityThreejsRenderer.setVisibility(pdfUnderlayData, scene, false);

      expect(result).toBe(true);
      expect(group.visible).toBe(false);
    });

    it('should return false for non-existent PdfUnderlay', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const result = PdfUnderlayEntityThreejsRenderer.setVisibility(pdfUnderlayData, scene, true);

      expect(result).toBe(false);
    });
  });

  describe('setOpacity', () => {
    it('should set PdfUnderlay opacity', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      const result = PdfUnderlayEntityThreejsRenderer.setOpacity(pdfUnderlayData, scene, 0.5);

      expect(result).toBe(true);
    });

    it('should return false for non-existent PdfUnderlay', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const result = PdfUnderlayEntityThreejsRenderer.setOpacity(pdfUnderlayData, scene, 0.5);

      expect(result).toBe(false);
    });

    it('should adjust opacity based on fade', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Fade = 40;
      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      const result = PdfUnderlayEntityThreejsRenderer.setOpacity(pdfUnderlayData, scene, 0.8);

      expect(result).toBe(true);
    });
  });

  describe('setContrast', () => {
    it('should set PdfUnderlay contrast', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      const result = PdfUnderlayEntityThreejsRenderer.setContrast(pdfUnderlayData, scene, 75);

      expect(result).toBe(true);
    });

    it('should return false for non-existent PdfUnderlay', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const result = PdfUnderlayEntityThreejsRenderer.setContrast(pdfUnderlayData, scene, 75);

      expect(result).toBe(false);
    });
  });

  describe('setFade', () => {
    it('should set PdfUnderlay fade', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      const result = PdfUnderlayEntityThreejsRenderer.setFade(pdfUnderlayData, scene, 30);

      expect(result).toBe(true);
    });

    it('should return false for non-existent PdfUnderlay', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const result = PdfUnderlayEntityThreejsRenderer.setFade(pdfUnderlayData, scene, 30);

      expect(result).toBe(false);
    });
  });

  describe('reloadTexture', () => {
    it('should reload texture for PdfUnderlay', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      const result = PdfUnderlayEntityThreejsRenderer.reloadTexture(pdfUnderlayData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent PdfUnderlay', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const result = PdfUnderlayEntityThreejsRenderer.reloadTexture(pdfUnderlayData, scene);

      expect(result).toBe(false);
    });
  });

  describe('getPdfUnderlayGroup', () => {
    it('should return existing PdfUnderlay group', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      const group = PdfUnderlayEntityThreejsRenderer.getPdfUnderlayGroup(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group?.name).toBe(`PdfUnderlay_${pdfUnderlayData.Handle}`);
    });

    it('should return null for non-existent PdfUnderlay', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();

      const group = PdfUnderlayEntityThreejsRenderer.getPdfUnderlayGroup(pdfUnderlayData, scene);

      expect(group).toBeNull();
    });
  });

  describe('getFilePath', () => {
    it('should return correct file path', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.FilePath = 'example.pdf';

      const filePath = PdfUnderlayEntityThreejsRenderer.getFilePath(pdfUnderlayData);

      expect(filePath).toBe('example.pdf');
    });

    it('should return empty string for null file path', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.FilePath = '';

      const filePath = PdfUnderlayEntityThreejsRenderer.getFilePath(pdfUnderlayData);

      expect(filePath).toBe('');
    });
  });

  describe('getPageNumber', () => {
    it('should return correct page number', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.PageNumber = '5';

      const pageNumber = PdfUnderlayEntityThreejsRenderer.getPageNumber(pdfUnderlayData);

      expect(pageNumber).toBe('5');
    });

    it('should return default page number for null', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.PageNumber = '';

      const pageNumber = PdfUnderlayEntityThreejsRenderer.getPageNumber(pdfUnderlayData);

      expect(pageNumber).toBe('1');
    });
  });

  describe('getClippingBoundaryType', () => {
    it('should return correct clipping boundary type', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.ClippingBoundaryType = 'Polygon';

      const clippingBoundaryType = PdfUnderlayEntityThreejsRenderer.getClippingBoundaryType(pdfUnderlayData);

      expect(clippingBoundaryType).toBe('Polygon');
    });

    it('should return None for null clipping boundary type', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.ClippingBoundaryType = '';

      const clippingBoundaryType = PdfUnderlayEntityThreejsRenderer.getClippingBoundaryType(pdfUnderlayData);

      expect(clippingBoundaryType).toBe('None');
    });
  });

  describe('getContrast', () => {
    it('should return correct contrast', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Contrast = 75;

      const contrast = PdfUnderlayEntityThreejsRenderer.getContrast(pdfUnderlayData);

      expect(contrast).toBe(75);
    });

    it('should return default contrast for null', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Contrast = null as any;

      const contrast = PdfUnderlayEntityThreejsRenderer.getContrast(pdfUnderlayData);

      expect(contrast).toBe(50);
    });
  });

  describe('getFade', () => {
    it('should return correct fade', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Fade = 40;

      const fade = PdfUnderlayEntityThreejsRenderer.getFade(pdfUnderlayData);

      expect(fade).toBe(40);
    });

    it('should return default fade for null', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Fade = 0;

      const fade = PdfUnderlayEntityThreejsRenderer.getFade(pdfUnderlayData);

      expect(fade).toBe(0);
    });
  });

  describe('getInsertPoint3D', () => {
    it('should return correct insert point', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.InsertPoint3D = { X: 10, Y: 20, Z: 30 };

      const insertPoint = PdfUnderlayEntityThreejsRenderer.getInsertPoint3D(pdfUnderlayData);

      expect(insertPoint).toEqual({ X: 10, Y: 20, Z: 30 });
    });

    it('should return default insert point for null', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.InsertPoint3D = null as any;

      const insertPoint = PdfUnderlayEntityThreejsRenderer.getInsertPoint3D(pdfUnderlayData);

      expect(insertPoint).toEqual({ X: 0, Y: 0, Z: 0 });
    });
  });

  describe('getScale', () => {
    it('should return correct scale', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.XScale = 2.0;
      pdfUnderlayData.YScale = 1.5;
      pdfUnderlayData.ZScale = 0.5;

      const scale = PdfUnderlayEntityThreejsRenderer.getScale(pdfUnderlayData);

      expect(scale).toEqual({ X: 2.0, Y: 1.5, Z: 0.5 });
    });

    it('should return default scale for null values', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.XScale = 0;
      pdfUnderlayData.YScale = 0;
      pdfUnderlayData.ZScale = 0;

      const scale = PdfUnderlayEntityThreejsRenderer.getScale(pdfUnderlayData);

      expect(scale).toEqual({ X: 1.0, Y: 1.0, Z: 1.0 });
    });
  });

  describe('getRotation', () => {
    it('should return correct rotation', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Rotation = Math.PI / 4;

      const rotation = PdfUnderlayEntityThreejsRenderer.getRotation(pdfUnderlayData);

      expect(rotation).toBe(Math.PI / 4);
    });

    it('should return 0 for no rotation', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Rotation = 0;

      const rotation = PdfUnderlayEntityThreejsRenderer.getRotation(pdfUnderlayData);

      expect(rotation).toBe(0);
    });
  });

  describe('getBounds', () => {
    it('should return correct bounds', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Geometry.Bounds = {
        Min: { X: 0, Y: 0, Z: 0 },
        Max: { X: 100, Y: 100, Z: 0 },
        Center: { X: 50, Y: 50, Z: 0 },
        Size: { X: 100, Y: 100, Z: 0 }
      };

      const bounds = PdfUnderlayEntityThreejsRenderer.getBounds(pdfUnderlayData);

      expect(bounds.Min).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(bounds.Max).toEqual({ X: 100, Y: 100, Z: 0 });
      expect(bounds.Center).toEqual({ X: 50, Y: 50, Z: 0 });
      expect(bounds.Size).toEqual({ X: 100, Y: 100, Z: 0 });
    });

    it('should return default bounds for null', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Geometry = null as any;

      const bounds = PdfUnderlayEntityThreejsRenderer.getBounds(pdfUnderlayData);

      expect(bounds.Min).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(bounds.Max).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(bounds.Center).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(bounds.Size).toEqual({ X: 0, Y: 0, Z: 0 });
    });
  });

  describe('getBoundaryPoints', () => {
    it('should return correct boundary points', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.BoundaryPoints3D = [
        { X: 0, Y: 0, Z: 0 },
        { X: 100, Y: 0, Z: 0 },
        { X: 100, Y: 100, Z: 0 },
        { X: 0, Y: 100, Z: 0 }
      ];

      const boundaryPoints = PdfUnderlayEntityThreejsRenderer.getBoundaryPoints(pdfUnderlayData);

      expect(boundaryPoints).toHaveLength(4);
      expect(boundaryPoints[0]).toEqual({ X: 0, Y: 0, Z: 0 });
      expect(boundaryPoints[3]).toEqual({ X: 0, Y: 100, Z: 0 });
    });

    it('should return empty array for null boundary points', () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.BoundaryPoints3D = null as any;

      const boundaryPoints = PdfUnderlayEntityThreejsRenderer.getBoundaryPoints(pdfUnderlayData);

      expect(boundaryPoints).toEqual([]);
    });
  });

  describe('clearCache', () => {
    it('should clear texture and PDF cache', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      PdfUnderlayEntityThreejsRenderer.clearCache();

      expect(scene.children.length).toBe(1);
    });

    it('should handle clearing empty cache', () => {
      expect(() => {
        PdfUnderlayEntityThreejsRenderer.clearCache();
      }).not.toThrow();
    });
  });

  describe('edge cases', () => {
    it('should handle PdfUnderlay with no geometry vertices', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Geometry.Vertices = [];

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should handle PdfUnderlay with no bounds', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Geometry.Bounds = null as any;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should handle PdfUnderlay with no boundary points', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.BoundaryPoints3D = [];

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should handle PdfUnderlay with zero opacity', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Opacity = 0;
      pdfUnderlayData.Transparent = true;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should handle PdfUnderlay with maximum fade', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Fade = 100;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should handle PdfUnderlay with maximum contrast', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Contrast = 100;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should handle PdfUnderlay with zero contrast', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Contrast = 0;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should handle PdfUnderlay with large scale values', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.XScale = 1000;
      pdfUnderlayData.YScale = 1000;
      pdfUnderlayData.ZScale = 1000;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should handle PdfUnderlay with negative scale values', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.XScale = -1.0;
      pdfUnderlayData.YScale = -1.0;
      pdfUnderlayData.ZScale = -1.0;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should handle PdfUnderlay with rotation greater than 2π', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Rotation = 3 * Math.PI;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should handle PdfUnderlay with negative rotation', async () => {
      const pdfUnderlayData = createBasicPdfUnderlayData();
      pdfUnderlayData.Rotation = -Math.PI / 2;

      const group = PdfUnderlayEntityThreejsRenderer.render(pdfUnderlayData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });
  });
});
