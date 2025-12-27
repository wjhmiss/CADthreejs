import * as THREE from 'three';
import { TextEntityThreejsRenderer, TextData, Point3DData, ColorData, TransformData, NormalData, BoundsData, GeometryData, MaterialData, TextHorizontalAlignment, TextVerticalAlignmentType, TextMirrorFlag } from '../TextEntityThreejsRenderer';

describe('TextEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    TextEntityThreejsRenderer.clearCache();
  });

  const createBasicTextData = (): TextData => ({
    Type: 'Text',
    EntityType: 'Text',
    Handle: 'text-handle-1',
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Visible: true,
    Value: 'Hello World',
    InsertPoint: { X: 0, Y: 0, Z: 0 },
    AlignmentPoint: { X: 0, Y: 0, Z: 0 },
    Height: 10.0,
    Rotation: 0,
    ObliqueAngle: 0,
    WidthFactor: 1.0,
    HorizontalAlignment: TextHorizontalAlignment.Left,
    VerticalAlignment: TextVerticalAlignmentType.Baseline,
    Mirror: TextMirrorFlag.None,
    Normal: { X: 0, Y: 0, Z: 1 },
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.03,
    Thickness: 0,
    StyleName: 'Standard',
    BoundaryPoints: [
      { X: -12, Y: -5, Z: 0 },
      { X: 12, Y: -5, Z: 0 },
      { X: 12, Y: 5, Z: 0 },
      { X: -12, Y: 5, Z: 0 }
    ],
    Bounds: {
      Min: { X: -12, Y: -5, Z: 0 },
      Max: { X: 12, Y: 5, Z: 0 },
      Center: { X: 0, Y: 0, Z: 0 },
      Size: { X: 24, Y: 10, Z: 0 }
    },
    Centroid: { X: 0, Y: 0, Z: 0 },
    Width: 24.0,
    Area: 240.0,
    Transform: {
      Position: { X: 0, Y: 0, Z: 0 },
      Rotation: { X: 0, Y: 0, Z: 0 },
      Scale: { X: 1, Y: 1, Z: 1 },
      Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
    },
    Geometry: {
      Type: 'TextGeometry',
      VertexCount: 4,
      FaceCount: 2,
      HasNormals: true,
      HasColors: false,
      HasUVs: false,
      HasIndices: false,
      PrimitiveType: 'Triangles',
      IndexCount: 0,
      IsClosed: false,
      IsPeriodic: false
    },
    Material: {
      Type: 'MeshBasicMaterial',
      Color: 16711680,
      Opacity: 1.0,
      Transparent: false,
      Wireframe: false,
      LineWidth: 1.0,
      VertexColors: false,
      Side: true
    },
    VertexPositions: [-12, -5, 0, 12, -5, 0, 12, 5, 0, -12, 5, 0],
    VertexColors: [],
    Indices: [],
    VertexCount: 4,
    FontFamily: 'Arial',
    FontSize: 10.0,
    TextAlignment: 'Left-Baseline',
    Opacity: 1.0,
    Transparent: false,
    Color: { Index: 1, Hex: '#FF0000', R: 255, G: 0, B: 0, A: 1.0 },
    Tangent: { X: 1, Y: 0, Z: 0 },
    Binormal: [0, 0, 1],
    UV: [],
    IsMirrored: false,
    IsUpsideDown: false,
    TextLength: 11,
    Ascent: 8.0,
    Descent: 2.0,
    CharacterCount: 11,
    FontStyle: 'Regular',
    IsBold: false,
    IsItalic: false,
    DepthTest: true
  });

  describe('render', () => {
    it('should render a basic Text', () => {
      const textData = createBasicTextData();

      const group = TextEntityThreejsRenderer.render(textData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.name).toBe('Text_text-handle-1');
    });

    it('should not add invisible Text to scene', () => {
      const textData = createBasicTextData();
      textData.Visible = false;

      const group = TextEntityThreejsRenderer.render(textData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.visible).toBe(false);
      expect(scene.children.length).toBe(0);
    });

    it('should render Text with correct Mesh object', () => {
      const textData = createBasicTextData();

      const group = TextEntityThreejsRenderer.render(textData, scene);

      expect(group.children.length).toBe(1);
      const mesh = group.children[0] as THREE.Mesh;
      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh.name).toBe('Text');
    });

    it('should render Text with correct material', () => {
      const textData = createBasicTextData();

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.material).toBeDefined();
      expect(mesh.material).toBeInstanceOf(THREE.MeshBasicMaterial);
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.color.getHexString()).toBe('ff0000');
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
      expect(material.depthTest).toBe(true);
      expect(material.side).toBe(THREE.DoubleSide);
    });

    it('should render Text with correct geometry', () => {
      const textData = createBasicTextData();

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.geometry).toBeDefined();
      expect(mesh.geometry).toBeInstanceOf(THREE.BufferGeometry);
      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.position).toBeDefined();
      expect(geometry.attributes.position.count).toBe(4);
    });

    it('should render Text with correct vertex positions', () => {
      const textData = createBasicTextData();

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(-12);
      expect(positions[1]).toBe(-5);
      expect(positions[2]).toBe(0);
      expect(positions[3]).toBe(12);
      expect(positions[4]).toBe(-5);
      expect(positions[5]).toBe(0);
    });

    it('should render Text with correct color', () => {
      const textData = createBasicTextData();
      textData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should render Text with correct opacity', () => {
      const textData = createBasicTextData();
      textData.Opacity = 0.5;
      textData.Transparent = true;

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should render Text with correct depth settings', () => {
      const textData = createBasicTextData();
      textData.DepthTest = false;

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.depthTest).toBe(false);
    });

    it('should cache Text by handle', () => {
      const textData = createBasicTextData();

      TextEntityThreejsRenderer.render(textData, scene);

      const cachedMesh = (TextEntityThreejsRenderer as any).textCache.get('text-handle-1');
      expect(cachedMesh).toBeDefined();
      expect(cachedMesh).toBeInstanceOf(THREE.Mesh);
    });

    it('should render Text with correct handle in userData', () => {
      const textData = createBasicTextData();

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      expect(mesh.userData.handle).toBe('text-handle-1');
      expect(mesh.userData.text).toBe('Hello World');
    });

    it('should render Text with default geometry when vertex positions not provided', () => {
      const textData = createBasicTextData();
      textData.VertexPositions = [];
      textData.Width = 24.0;
      textData.Height = 10.0;
      textData.InsertPoint = { X: 0, Y: 0, Z: 0 };

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(-12);
      expect(positions[1]).toBe(-5);
      expect(positions[2]).toBe(0);
      expect(positions[3]).toBe(12);
      expect(positions[4]).toBe(-5);
      expect(positions[5]).toBe(0);
    });

    it('should render Text with vertex colors', () => {
      const textData = createBasicTextData();
      textData.VertexColors = [255, 0, 0, 0, 255, 0];

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.color).toBeDefined();
      const colors = geometry.attributes.color.array as Float32Array;
      expect(colors[0]).toBeCloseTo(1.0);
      expect(colors[1]).toBeCloseTo(0.0);
      expect(colors[2]).toBeCloseTo(0.0);
    });

    it('should render Text with indices', () => {
      const textData = createBasicTextData();
      textData.Indices = [0, 1, 2, 0, 2, 3];

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.index).toBeDefined();
      const indices = geometry.index.array as Uint16Array;
      expect(indices[0]).toBe(0);
      expect(indices[1]).toBe(1);
      expect(indices[2]).toBe(2);
    });

    it('should render Text with UV coordinates', () => {
      const textData = createBasicTextData();
      textData.UV = [0, 0, 1, 0, 1, 1, 0, 1];

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      expect(geometry.attributes.uv).toBeDefined();
      const uvs = geometry.attributes.uv.array as Float32Array;
      expect(uvs[0]).toBe(0);
      expect(uvs[1]).toBe(0);
      expect(uvs[2]).toBe(1);
    });

    it('should render Text with wireframe material', () => {
      const textData = createBasicTextData();
      textData.Material.Wireframe = true;

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.wireframe).toBe(true);
    });

    it('should render Text with negative coordinates', () => {
      const textData = createBasicTextData();
      textData.VertexPositions = [-10, -20, -30, -40, -50, -60];

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(-10);
      expect(positions[1]).toBe(-20);
      expect(positions[2]).toBe(-30);
    });

    it('should render Text with very large coordinates', () => {
      const textData = createBasicTextData();
      textData.VertexPositions = [1000000, 2000000, 3000000, 4000000, 5000000, 6000000];

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(1000000);
      expect(positions[1]).toBe(2000000);
      expect(positions[2]).toBe(3000000);
    });

    it('should render Text with very small coordinates', () => {
      const textData = createBasicTextData();
      textData.VertexPositions = [0.0001, 0.0002, 0.0003, 0.0004, 0.0005, 0.0006];

      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBeCloseTo(0.0001, 7);
      expect(positions[1]).toBeCloseTo(0.0002, 7);
      expect(positions[2]).toBeCloseTo(0.0003, 7);
    });

    it('should render Text with different horizontal alignment', () => {
      const textData = createBasicTextData();
      textData.HorizontalAlignment = TextHorizontalAlignment.Center;

      const group = TextEntityThreejsRenderer.render(textData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render Text with different vertical alignment', () => {
      const textData = createBasicTextData();
      textData.VerticalAlignment = TextVerticalAlignmentType.Middle;

      const group = TextEntityThreejsRenderer.render(textData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render Text with mirror flags', () => {
      const textData = createBasicTextData();
      textData.Mirror = TextMirrorFlag.MirrorX;

      const group = TextEntityThreejsRenderer.render(textData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render Text with rotation', () => {
      const textData = createBasicTextData();
      textData.Rotation = Math.PI / 4;

      const group = TextEntityThreejsRenderer.render(textData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render Text with oblique angle', () => {
      const textData = createBasicTextData();
      textData.ObliqueAngle = Math.PI / 6;

      const group = TextEntityThreejsRenderer.render(textData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });

    it('should render Text with width factor', () => {
      const textData = createBasicTextData();
      textData.WidthFactor = 1.5;

      const group = TextEntityThreejsRenderer.render(textData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
    });
  });

  describe('renderMultiple', () => {
    it('should render multiple Texts', () => {
      const textDataArray = [
        createBasicTextData(),
        createBasicTextData()
      ];
      textDataArray[0].Handle = 'text-1';
      textDataArray[1].Handle = 'text-2';

      const group = TextEntityThreejsRenderer.renderMultiple(textDataArray, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('MultipleTexts');
      expect(group.children.length).toBe(2);
    });

    it('should render multiple Texts with correct handles', () => {
      const textDataArray = [
        createBasicTextData(),
        createBasicTextData(),
        createBasicTextData()
      ];
      textDataArray[0].Handle = 'text-1';
      textDataArray[1].Handle = 'text-2';
      textDataArray[2].Handle = 'text-3';

      const group = TextEntityThreejsRenderer.renderMultiple(textDataArray, scene);

      expect(group.children[0].name).toBe('Text_text-1');
      expect(group.children[1].name).toBe('Text_text-2');
      expect(group.children[2].name).toBe('Text_text-3');
    });

    it('should render empty array of Texts', () => {
      const textDataArray: TextData[] = [];

      const group = TextEntityThreejsRenderer.renderMultiple(textDataArray, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('MultipleTexts');
      expect(group.children.length).toBe(0);
    });

    it('should render single Text using renderMultiple', () => {
      const textDataArray = [createBasicTextData()];
      textDataArray[0].Handle = 'text-1';

      const group = TextEntityThreejsRenderer.renderMultiple(textDataArray, scene);

      expect(group.children.length).toBe(1);
      expect(group.children[0].name).toBe('Text_text-1');
    });
  });

  describe('update', () => {
    it('should update existing Text', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);

      const updatedTextData = { ...textData };
      updatedTextData.VertexPositions = [10, 20, 30, 40, 50, 60];

      const result = TextEntityThreejsRenderer.update(updatedTextData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent Text', () => {
      const textData = createBasicTextData();

      const result = TextEntityThreejsRenderer.update(textData, scene);

      expect(result).toBe(false);
    });

    it('should update Text color', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);

      const updatedTextData = { ...textData };
      updatedTextData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const result = TextEntityThreejsRenderer.update(updatedTextData, scene);

      expect(result).toBe(true);
      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.color.getHexString()).toBe('00ff00');
    });

    it('should update Text opacity', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);

      const updatedTextData = { ...textData };
      updatedTextData.Opacity = 0.5;
      updatedTextData.Transparent = true;

      const result = TextEntityThreejsRenderer.update(updatedTextData, scene);

      expect(result).toBe(true);
      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should update Text vertex positions', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);

      const updatedTextData = { ...textData };
      updatedTextData.VertexPositions = [100, 200, 300, 400, 500, 600];

      const result = TextEntityThreejsRenderer.update(updatedTextData, scene);

      expect(result).toBe(true);
      const mesh = group.children[0] as THREE.Mesh;
      const geometry = mesh.geometry as THREE.BufferGeometry;
      const positions = geometry.attributes.position.array as Float32Array;
      expect(positions[0]).toBe(100);
      expect(positions[1]).toBe(200);
      expect(positions[2]).toBe(300);
    });
  });

  describe('dispose', () => {
    it('should dispose Text resources', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);

      const result = TextEntityThreejsRenderer.dispose(textData, scene);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(0);
    });

    it('should return false for non-existent Text', () => {
      const textData = createBasicTextData();

      const result = TextEntityThreejsRenderer.dispose(textData, scene);

      expect(result).toBe(false);
    });

    it('should remove Text from cache', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);

      TextEntityThreejsRenderer.dispose(textData, scene);

      const cachedMesh = (TextEntityThreejsRenderer as any).textCache.get('text-handle-1');
      expect(cachedMesh).toBeUndefined();
    });

    it('should dispose Text geometry', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const geometry = mesh.geometry;
      const originalDispose = geometry.dispose.bind(geometry);
      let disposeCalled = false;
      geometry.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      TextEntityThreejsRenderer.dispose(textData, scene);

      expect(disposeCalled).toBe(true);
    });

    it('should dispose Text material', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);
      const mesh = group.children[0] as THREE.Mesh;

      const material = mesh.material;
      const originalDispose = material.dispose.bind(material);
      let disposeCalled = false;
      material.dispose = () => {
        disposeCalled = true;
        originalDispose();
      };

      TextEntityThreejsRenderer.dispose(textData, scene);

      expect(disposeCalled).toBe(true);
    });
  });

  describe('disposeMultiple', () => {
    it('should dispose multiple Texts', () => {
      const textDataArray = [
        createBasicTextData(),
        createBasicTextData()
      ];
      textDataArray[0].Handle = 'text-1';
      textDataArray[1].Handle = 'text-2';

      const group = TextEntityThreejsRenderer.renderMultiple(textDataArray, scene);

      TextEntityThreejsRenderer.disposeMultiple(group, scene);

      expect(scene.children.length).toBe(0);
    });

    it('should handle null group gracefully', () => {
      expect(() => {
        TextEntityThreejsRenderer.disposeMultiple(null, scene);
      }).not.toThrow();
    });

    it('should handle undefined group gracefully', () => {
      expect(() => {
        TextEntityThreejsRenderer.disposeMultiple(undefined, scene);
      }).not.toThrow();
    });
  });

  describe('setVisibility', () => {
    it('should set Text visibility to true', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);

      const result = TextEntityThreejsRenderer.setVisibility(textData, scene, true);

      expect(result).toBe(true);
      expect(group.visible).toBe(true);
    });

    it('should set Text visibility to false', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);

      const result = TextEntityThreejsRenderer.setVisibility(textData, scene, false);

      expect(result).toBe(true);
      expect(group.visible).toBe(false);
      expect(scene.children.length).toBe(0);
    });

    it('should return false for non-existent Text', () => {
      const textData = createBasicTextData();

      const result = TextEntityThreejsRenderer.setVisibility(textData, scene, true);

      expect(result).toBe(false);
    });

    it('should add Text to scene when setting visibility to true', () => {
      const textData = createBasicTextData();
      textData.Visible = false;
      const group = TextEntityThreejsRenderer.render(textData, scene);

      const result = TextEntityThreejsRenderer.setVisibility(textData, scene, true);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(1);
    });

    it('should remove Text from scene when setting visibility to false', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);

      const result = TextEntityThreejsRenderer.setVisibility(textData, scene, false);

      expect(result).toBe(true);
      expect(scene.children.length).toBe(0);
    });
  });

  describe('setOpacity', () => {
    it('should set Text opacity', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);

      const result = TextEntityThreejsRenderer.setOpacity(textData, 0.5);

      expect(result).toBe(true);
      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.opacity).toBe(0.5);
      expect(material.transparent).toBe(true);
    });

    it('should return false for non-existent Text', () => {
      const textData = createBasicTextData();

      const result = TextEntityThreejsRenderer.setOpacity(textData, 0.5);

      expect(result).toBe(false);
    });

    it('should set opacity to 1.0', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);

      const result = TextEntityThreejsRenderer.setOpacity(textData, 1.0);

      expect(result).toBe(true);
      const mesh = group.children[0] as THREE.Mesh;
      const material = mesh.material as THREE.MeshBasicMaterial;
      expect(material.opacity).toBe(1.0);
      expect(material.transparent).toBe(false);
    });
  });

  describe('setText', () => {
    it('should set Text value', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);

      const result = TextEntityThreejsRenderer.setText(textData, 'New Text');

      expect(result).toBe(true);
      const mesh = group.children[0] as THREE.Mesh;
      expect(mesh.userData.text).toBe('New Text');
    });

    it('should return false for non-existent Text', () => {
      const textData = createBasicTextData();

      const result = TextEntityThreejsRenderer.setText(textData, 'New Text');

      expect(result).toBe(false);
    });
  });

  describe('setFontSize', () => {
    it('should set font size', () => {
      const textData = createBasicTextData();
      const group = TextEntityThreejsRenderer.render(textData, scene);

      const result = TextEntityThreejsRenderer.setFontSize(textData, 20.0);

      expect(result).toBe(true);
      expect(group.scale.x).toBe(2.0);
      expect(group.scale.y).toBe(2.0);
      expect(group.scale.z).toBe(2.0);
    });

    it('should return false for non-existent Text', () => {
      const textData = createBasicTextData();

      const result = TextEntityThreejsRenderer.setFontSize(textData, 20.0);

      expect(result).toBe(false);
    });
  });

  describe('getTextMesh', () => {
    it('should return cached Text mesh', () => {
      const textData = createBasicTextData();
      TextEntityThreejsRenderer.render(textData, scene);

      const mesh = TextEntityThreejsRenderer.getTextMesh(textData);

      expect(mesh).toBeInstanceOf(THREE.Mesh);
      expect(mesh).not.toBeNull();
    });

    it('should return null for non-existent Text', () => {
      const textData = createBasicTextData();

      const mesh = TextEntityThreejsRenderer.getTextMesh(textData);

      expect(mesh).toBeNull();
    });
  });

  describe('getText', () => {
    it('should return correct text value', () => {
      const textData = createBasicTextData();

      const text = TextEntityThreejsRenderer.getText(textData);

      expect(text).toBe('Hello World');
    });

    it('should return empty string when text not set', () => {
      const textData = createBasicTextData();
      textData.Value = undefined;

      const text = TextEntityThreejsRenderer.getText(textData);

      expect(text).toBe('');
    });
  });

  describe('getInsertPoint', () => {
    it('should return correct insert point', () => {
      const textData = createBasicTextData();

      const insertPoint = TextEntityThreejsRenderer.getInsertPoint(textData);

      expect(insertPoint).toBeInstanceOf(THREE.Vector3);
      expect(insertPoint.x).toBe(0);
      expect(insertPoint.y).toBe(0);
      expect(insertPoint.z).toBe(0);
    });

    it('should return correct insert point with custom values', () => {
      const textData = createBasicTextData();
      textData.InsertPoint = { X: 10, Y: 20, Z: 30 };

      const insertPoint = TextEntityThreejsRenderer.getInsertPoint(textData);

      expect(insertPoint.x).toBe(10);
      expect(insertPoint.y).toBe(20);
      expect(insertPoint.z).toBe(30);
    });
  });

  describe('getAlignmentPoint', () => {
    it('should return correct alignment point', () => {
      const textData = createBasicTextData();

      const alignmentPoint = TextEntityThreejsRenderer.getAlignmentPoint(textData);

      expect(alignmentPoint).toBeInstanceOf(THREE.Vector3);
      expect(alignmentPoint.x).toBe(0);
      expect(alignmentPoint.y).toBe(0);
      expect(alignmentPoint.z).toBe(0);
    });

    it('should return correct alignment point with custom values', () => {
      const textData = createBasicTextData();
      textData.AlignmentPoint = { X: 15, Y: 25, Z: 35 };

      const alignmentPoint = TextEntityThreejsRenderer.getAlignmentPoint(textData);

      expect(alignmentPoint.x).toBe(15);
      expect(alignmentPoint.y).toBe(25);
      expect(alignmentPoint.z).toBe(35);
    });
  });

  describe('getHeight', () => {
    it('should return correct height', () => {
      const textData = createBasicTextData();

      const height = TextEntityThreejsRenderer.getHeight(textData);

      expect(height).toBe(10.0);
    });

    it('should return default height when not set', () => {
      const textData = createBasicTextData();
      textData.Height = undefined;

      const height = TextEntityThreejsRenderer.getHeight(textData);

      expect(height).toBe(12.0);
    });
  });

  describe('getRotation', () => {
    it('should return correct rotation', () => {
      const textData = createBasicTextData();

      const rotation = TextEntityThreejsRenderer.getRotation(textData);

      expect(rotation).toBe(0);
    });

    it('should return correct rotation with custom value', () => {
      const textData = createBasicTextData();
      textData.Rotation = Math.PI / 2;

      const rotation = TextEntityThreejsRenderer.getRotation(textData);

      expect(rotation).toBe(Math.PI / 2);
    });

    it('should return 0 when rotation not set', () => {
      const textData = createBasicTextData();
      textData.Rotation = undefined;

      const rotation = TextEntityThreejsRenderer.getRotation(textData);

      expect(rotation).toBe(0);
    });
  });

  describe('getObliqueAngle', () => {
    it('should return correct oblique angle', () => {
      const textData = createBasicTextData();

      const obliqueAngle = TextEntityThreejsRenderer.getObliqueAngle(textData);

      expect(obliqueAngle).toBe(0);
    });

    it('should return correct oblique angle with custom value', () => {
      const textData = createBasicTextData();
      textData.ObliqueAngle = Math.PI / 6;

      const obliqueAngle = TextEntityThreejsRenderer.getObliqueAngle(textData);

      expect(obliqueAngle).toBe(Math.PI / 6);
    });

    it('should return 0 when oblique angle not set', () => {
      const textData = createBasicTextData();
      textData.ObliqueAngle = undefined;

      const obliqueAngle = TextEntityThreejsRenderer.getObliqueAngle(textData);

      expect(obliqueAngle).toBe(0);
    });
  });

  describe('getWidthFactor', () => {
    it('should return correct width factor', () => {
      const textData = createBasicTextData();

      const widthFactor = TextEntityThreejsRenderer.getWidthFactor(textData);

      expect(widthFactor).toBe(1.0);
    });

    it('should return correct width factor with custom value', () => {
      const textData = createBasicTextData();
      textData.WidthFactor = 1.5;

      const widthFactor = TextEntityThreejsRenderer.getWidthFactor(textData);

      expect(widthFactor).toBe(1.5);
    });

    it('should return 1.0 when width factor not set', () => {
      const textData = createBasicTextData();
      textData.WidthFactor = undefined;

      const widthFactor = TextEntityThreejsRenderer.getWidthFactor(textData);

      expect(widthFactor).toBe(1.0);
    });
  });

  describe('getHorizontalAlignment', () => {
    it('should return correct horizontal alignment', () => {
      const textData = createBasicTextData();

      const alignment = TextEntityThreejsRenderer.getHorizontalAlignment(textData);

      expect(alignment).toBe(TextHorizontalAlignment.Left);
    });

    it('should return correct horizontal alignment with custom value', () => {
      const textData = createBasicTextData();
      textData.HorizontalAlignment = TextHorizontalAlignment.Center;

      const alignment = TextEntityThreejsRenderer.getHorizontalAlignment(textData);

      expect(alignment).toBe(TextHorizontalAlignment.Center);
    });

    it('should return Left when alignment not set', () => {
      const textData = createBasicTextData();
      textData.HorizontalAlignment = undefined;

      const alignment = TextEntityThreejsRenderer.getHorizontalAlignment(textData);

      expect(alignment).toBe(TextHorizontalAlignment.Left);
    });
  });

  describe('getVerticalAlignment', () => {
    it('should return correct vertical alignment', () => {
      const textData = createBasicTextData();

      const alignment = TextEntityThreejsRenderer.getVerticalAlignment(textData);

      expect(alignment).toBe(TextVerticalAlignmentType.Baseline);
    });

    it('should return correct vertical alignment with custom value', () => {
      const textData = createBasicTextData();
      textData.VerticalAlignment = TextVerticalAlignmentType.Middle;

      const alignment = TextEntityThreejsRenderer.getVerticalAlignment(textData);

      expect(alignment).toBe(TextVerticalAlignmentType.Middle);
    });

    it('should return Baseline when alignment not set', () => {
      const textData = createBasicTextData();
      textData.VerticalAlignment = undefined;

      const alignment = TextEntityThreejsRenderer.getVerticalAlignment(textData);

      expect(alignment).toBe(TextVerticalAlignmentType.Baseline);
    });
  });

  describe('isMirrored', () => {
    it('should return true when mirrored', () => {
      const textData = createBasicTextData();
      textData.IsMirrored = true;

      const isMirrored = TextEntityThreejsRenderer.isMirrored(textData);

      expect(isMirrored).toBe(true);
    });

    it('should return false when not mirrored', () => {
      const textData = createBasicTextData();

      const isMirrored = TextEntityThreejsRenderer.isMirrored(textData);

      expect(isMirrored).toBe(false);
    });
  });

  describe('isUpsideDown', () => {
    it('should return true when upside down', () => {
      const textData = createBasicTextData();
      textData.IsUpsideDown = true;

      const isUpsideDown = TextEntityThreejsRenderer.isUpsideDown(textData);

      expect(isUpsideDown).toBe(true);
    });

    it('should return false when not upside down', () => {
      const textData = createBasicTextData();

      const isUpsideDown = TextEntityThreejsRenderer.isUpsideDown(textData);

      expect(isUpsideDown).toBe(false);
    });
  });

  describe('getColor', () => {
    it('should return correct color', () => {
      const textData = createBasicTextData();

      const color = TextEntityThreejsRenderer.getColor(textData);

      expect(color).toBeInstanceOf(THREE.Color);
      expect(color.getHexString()).toBe('ff0000');
    });

    it('should return correct color for green', () => {
      const textData = createBasicTextData();
      textData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const color = TextEntityThreejsRenderer.getColor(textData);

      expect(color).toBeInstanceOf(THREE.Color);
      expect(color.getHexString()).toBe('00ff00');
    });
  });

  describe('getBounds', () => {
    it('should return correct bounds', () => {
      const textData = createBasicTextData();

      const bounds = TextEntityThreejsRenderer.getBounds(textData);

      expect(bounds).toBeInstanceOf(THREE.Box3);
      expect(bounds.min.x).toBe(-12);
      expect(bounds.min.y).toBe(-5);
      expect(bounds.min.z).toBe(0);
      expect(bounds.max.x).toBe(12);
      expect(bounds.max.y).toBe(5);
      expect(bounds.max.z).toBe(0);
    });
  });

  describe('getCentroid', () => {
    it('should return correct centroid', () => {
      const textData = createBasicTextData();

      const centroid = TextEntityThreejsRenderer.getCentroid(textData);

      expect(centroid).toBeInstanceOf(THREE.Vector3);
      expect(centroid.x).toBe(0);
      expect(centroid.y).toBe(0);
      expect(centroid.z).toBe(0);
    });
  });

  describe('getWidth', () => {
    it('should return correct width', () => {
      const textData = createBasicTextData();

      const width = TextEntityThreejsRenderer.getWidth(textData);

      expect(width).toBe(24.0);
    });

    it('should return 0 when width not set', () => {
      const textData = createBasicTextData();
      textData.Width = undefined;

      const width = TextEntityThreejsRenderer.getWidth(textData);

      expect(width).toBe(0);
    });
  });

  describe('getArea', () => {
    it('should return correct area', () => {
      const textData = createBasicTextData();

      const area = TextEntityThreejsRenderer.getArea(textData);

      expect(area).toBe(240.0);
    });

    it('should return 0 when area not set', () => {
      const textData = createBasicTextData();
      textData.Area = undefined;

      const area = TextEntityThreejsRenderer.getArea(textData);

      expect(area).toBe(0);
    });
  });

  describe('clearCache', () => {
    it('should clear all cached texts', () => {
      const textData1 = createBasicTextData();
      const textData2 = createBasicTextData();
      textData1.Handle = 'text-1';
      textData2.Handle = 'text-2';

      TextEntityThreejsRenderer.render(textData1, scene);
      TextEntityThreejsRenderer.render(textData2, scene);

      expect((TextEntityThreejsRenderer as any).textCache.size).toBe(2);

      TextEntityThreejsRenderer.clearCache();

      expect((TextEntityThreejsRenderer as any).textCache.size).toBe(0);
    });
  });

  describe('getNormal', () => {
    it('should return correct normal', () => {
      const textData = createBasicTextData();

      const normal = TextEntityThreejsRenderer.getNormal(textData);

      expect(normal.X).toBe(0);
      expect(normal.Y).toBe(0);
      expect(normal.Z).toBe(1);
    });

    it('should return default normal when not set', () => {
      const textData = createBasicTextData();
      textData.Normal = undefined;

      const normal = TextEntityThreejsRenderer.getNormal(textData);

      expect(normal.X).toBe(0);
      expect(normal.Y).toBe(0);
      expect(normal.Z).toBe(1);
    });
  });

  describe('getTangent', () => {
    it('should return correct tangent', () => {
      const textData = createBasicTextData();

      const tangent = TextEntityThreejsRenderer.getTangent(textData);

      expect(tangent.X).toBe(1);
      expect(tangent.Y).toBe(0);
      expect(tangent.Z).toBe(0);
    });

    it('should return default tangent when not set', () => {
      const textData = createBasicTextData();
      textData.Tangent = undefined;

      const tangent = TextEntityThreejsRenderer.getTangent(textData);

      expect(tangent.X).toBe(1);
      expect(tangent.Y).toBe(0);
      expect(tangent.Z).toBe(0);
    });
  });

  describe('getBinormal', () => {
    it('should return correct binormal', () => {
      const textData = createBasicTextData();

      const binormal = TextEntityThreejsRenderer.getBinormal(textData);

      expect(binormal).toBeInstanceOf(Array);
      expect(binormal.length).toBe(3);
      expect(binormal[0]).toBe(0);
      expect(binormal[1]).toBe(0);
      expect(binormal[2]).toBe(1);
    });

    it('should return default binormal when not set', () => {
      const textData = createBasicTextData();
      textData.Binormal = undefined;

      const binormal = TextEntityThreejsRenderer.getBinormal(textData);

      expect(binormal).toBeInstanceOf(Array);
      expect(binormal.length).toBe(3);
      expect(binormal[0]).toBe(0);
      expect(binormal[1]).toBe(0);
      expect(binormal[2]).toBe(1);
    });
  });

  describe('getTransform', () => {
    it('should return correct transform', () => {
      const textData = createBasicTextData();

      const transform = TextEntityThreejsRenderer.getTransform(textData);

      expect(transform.Position.X).toBe(0);
      expect(transform.Position.Y).toBe(0);
      expect(transform.Position.Z).toBe(0);
      expect(transform.Rotation.X).toBe(0);
      expect(transform.Rotation.Y).toBe(0);
      expect(transform.Rotation.Z).toBe(0);
      expect(transform.Scale.X).toBe(1);
      expect(transform.Scale.Y).toBe(1);
      expect(transform.Scale.Z).toBe(1);
    });

    it('should return default transform when not set', () => {
      const textData = createBasicTextData();
      textData.Transform = undefined;

      const transform = TextEntityThreejsRenderer.getTransform(textData);

      expect(transform.Position.X).toBe(0);
      expect(transform.Position.Y).toBe(0);
      expect(transform.Position.Z).toBe(0);
      expect(transform.Scale.X).toBe(1);
      expect(transform.Scale.Y).toBe(1);
      expect(transform.Scale.Z).toBe(1);
    });
  });

  describe('getGeometry', () => {
    it('should return correct geometry', () => {
      const textData = createBasicTextData();

      const geometry = TextEntityThreejsRenderer.getGeometry(textData);

      expect(geometry.Type).toBe('TextGeometry');
      expect(geometry.VertexCount).toBe(4);
      expect(geometry.FaceCount).toBe(2);
      expect(geometry.HasNormals).toBe(true);
      expect(geometry.HasColors).toBe(false);
      expect(geometry.HasUVs).toBe(false);
      expect(geometry.HasIndices).toBe(false);
      expect(geometry.PrimitiveType).toBe('Triangles');
    });

    it('should return default geometry when not set', () => {
      const textData = createBasicTextData();
      textData.Geometry = undefined;

      const geometry = TextEntityThreejsRenderer.getGeometry(textData);

      expect(geometry.Type).toBe('TextGeometry');
      expect(geometry.VertexCount).toBe(4);
      expect(geometry.FaceCount).toBe(2);
      expect(geometry.HasNormals).toBe(true);
      expect(geometry.HasColors).toBe(true);
      expect(geometry.HasUVs).toBe(true);
      expect(geometry.HasIndices).toBe(true);
      expect(geometry.PrimitiveType).toBe('Triangles');
    });
  });

  describe('getMaterial', () => {
    it('should return correct material', () => {
      const textData = createBasicTextData();

      const material = TextEntityThreejsRenderer.getMaterial(textData);

      expect(material.Type).toBe('MeshBasicMaterial');
      expect(material.Color).toBe(16711680);
      expect(material.Opacity).toBe(1.0);
      expect(material.Transparent).toBe(false);
      expect(material.Wireframe).toBe(false);
      expect(material.LineWidth).toBe(1.0);
      expect(material.VertexColors).toBe(false);
    });

    it('should return default material when not set', () => {
      const textData = createBasicTextData();
      textData.Material = undefined;

      const material = TextEntityThreejsRenderer.getMaterial(textData);

      expect(material.Type).toBe('MeshBasicMaterial');
      expect(material.Color).toBe(16711680);
      expect(material.Opacity).toBe(1.0);
      expect(material.LineWidth).toBe(1.0);
      expect(material.VertexColors).toBe(true);
    });
  });

  describe('getDepthTest', () => {
    it('should return true when depth test enabled', () => {
      const textData = createBasicTextData();

      const depthTest = TextEntityThreejsRenderer.getDepthTest(textData);

      expect(depthTest).toBe(true);
    });

    it('should return false when depth test disabled', () => {
      const textData = createBasicTextData();
      textData.DepthTest = false;

      const depthTest = TextEntityThreejsRenderer.getDepthTest(textData);

      expect(depthTest).toBe(false);
    });
  });

  describe('getHandle', () => {
    it('should return correct handle', () => {
      const textData = createBasicTextData();

      const handle = TextEntityThreejsRenderer.getHandle(textData);

      expect(handle).toBe('text-handle-1');
    });

    it('should return empty string when handle not set', () => {
      const textData = createBasicTextData();
      textData.Handle = undefined;

      const handle = TextEntityThreejsRenderer.getHandle(textData);

      expect(handle).toBe('');
    });
  });

  describe('getLayerName', () => {
    it('should return correct layer name', () => {
      const textData = createBasicTextData();

      const layerName = TextEntityThreejsRenderer.getLayerName(textData);

      expect(layerName).toBe('TEST_LAYER');
    });

    it('should return empty string when layer name not set', () => {
      const textData = createBasicTextData();
      textData.LayerName = undefined;

      const layerName = TextEntityThreejsRenderer.getLayerName(textData);

      expect(layerName).toBe('');
    });
  });

  describe('getVisible', () => {
    it('should return true when visible', () => {
      const textData = createBasicTextData();

      const visible = TextEntityThreejsRenderer.getVisible(textData);

      expect(visible).toBe(true);
    });

    it('should return false when not visible', () => {
      const textData = createBasicTextData();
      textData.Visible = false;

      const visible = TextEntityThreejsRenderer.getVisible(textData);

      expect(visible).toBe(false);
    });
  });

  describe('getOpacity', () => {
    it('should return correct opacity', () => {
      const textData = createBasicTextData();

      const opacity = TextEntityThreejsRenderer.getOpacity(textData);

      expect(opacity).toBe(1.0);
    });

    it('should return default opacity when not set', () => {
      const textData = createBasicTextData();
      textData.Opacity = undefined;

      const opacity = TextEntityThreejsRenderer.getOpacity(textData);

      expect(opacity).toBe(1.0);
    });
  });

  describe('getTransparent', () => {
    it('should return true when transparent', () => {
      const textData = createBasicTextData();
      textData.Transparent = true;

      const transparent = TextEntityThreejsRenderer.getTransparent(textData);

      expect(transparent).toBe(true);
    });

    it('should return true when not transparent', () => {
      const textData = createBasicTextData();

      const transparent = TextEntityThreejsRenderer.getTransparent(textData);

      expect(transparent).toBe(true);
    });
  });

  describe('getFontFamily', () => {
    it('should return correct font family', () => {
      const textData = createBasicTextData();

      const fontFamily = TextEntityThreejsRenderer.getFontFamily(textData);

      expect(fontFamily).toBe('Arial');
    });

    it('should return default font family when not set', () => {
      const textData = createBasicTextData();
      textData.FontFamily = undefined;

      const fontFamily = TextEntityThreejsRenderer.getFontFamily(textData);

      expect(fontFamily).toBe('Arial');
    });
  });

  describe('getFontSize', () => {
    it('should return correct font size', () => {
      const textData = createBasicTextData();

      const fontSize = TextEntityThreejsRenderer.getFontSize(textData);

      expect(fontSize).toBe(10.0);
    });

    it('should return default font size when not set', () => {
      const textData = createBasicTextData();
      textData.FontSize = undefined;

      const fontSize = TextEntityThreejsRenderer.getFontSize(textData);

      expect(fontSize).toBe(12.0);
    });
  });

  describe('getTextAlignment', () => {
    it('should return correct text alignment', () => {
      const textData = createBasicTextData();

      const textAlignment = TextEntityThreejsRenderer.getTextAlignment(textData);

      expect(textAlignment).toBe('Left-Baseline');
    });

    it('should return default text alignment when not set', () => {
      const textData = createBasicTextData();
      textData.TextAlignment = undefined;

      const textAlignment = TextEntityThreejsRenderer.getTextAlignment(textData);

      expect(textAlignment).toBe('Left-Baseline');
    });
  });

  describe('getStyleName', () => {
    it('should return correct style name', () => {
      const textData = createBasicTextData();

      const styleName = TextEntityThreejsRenderer.getStyleName(textData);

      expect(styleName).toBe('Standard');
    });

    it('should return empty string when style name not set', () => {
      const textData = createBasicTextData();
      textData.StyleName = undefined;

      const styleName = TextEntityThreejsRenderer.getStyleName(textData);

      expect(styleName).toBe('');
    });
  });

  describe('getThickness', () => {
    it('should return correct thickness', () => {
      const textData = createBasicTextData();

      const thickness = TextEntityThreejsRenderer.getThickness(textData);

      expect(thickness).toBe(0);
    });

    it('should return 0 when thickness not set', () => {
      const textData = createBasicTextData();
      textData.Thickness = undefined;

      const thickness = TextEntityThreejsRenderer.getThickness(textData);

      expect(thickness).toBe(0);
    });
  });

  describe('getLineWeight', () => {
    it('should return correct line weight', () => {
      const textData = createBasicTextData();

      const lineWeight = TextEntityThreejsRenderer.getLineWeight(textData);

      expect(lineWeight).toBe(0.03);
    });

    it('should return 0 when line weight not set', () => {
      const textData = createBasicTextData();
      textData.LineWeight = undefined;

      const lineWeight = TextEntityThreejsRenderer.getLineWeight(textData);

      expect(lineWeight).toBe(0);
    });
  });

  describe('getLineTypeName', () => {
    it('should return correct line type name', () => {
      const textData = createBasicTextData();

      const lineTypeName = TextEntityThreejsRenderer.getLineTypeName(textData);

      expect(lineTypeName).toBe('CONTINUOUS');
    });

    it('should return empty string when line type name not set', () => {
      const textData = createBasicTextData();
      textData.LineTypeName = undefined;

      const lineTypeName = TextEntityThreejsRenderer.getLineTypeName(textData);

      expect(lineTypeName).toBe('');
    });
  });

  describe('getTextLength', () => {
    it('should return correct text length', () => {
      const textData = createBasicTextData();

      const textLength = TextEntityThreejsRenderer.getTextLength(textData);

      expect(textLength).toBe(11);
    });

    it('should return 0 when text length not set', () => {
      const textData = createBasicTextData();
      textData.TextLength = undefined;

      const textLength = TextEntityThreejsRenderer.getTextLength(textData);

      expect(textLength).toBe(0);
    });
  });

  describe('getAscent', () => {
    it('should return correct ascent', () => {
      const textData = createBasicTextData();

      const ascent = TextEntityThreejsRenderer.getAscent(textData);

      expect(ascent).toBe(8.0);
    });

    it('should return 0 when ascent not set', () => {
      const textData = createBasicTextData();
      textData.Ascent = undefined;

      const ascent = TextEntityThreejsRenderer.getAscent(textData);

      expect(ascent).toBe(0);
    });
  });

  describe('getDescent', () => {
    it('should return correct descent', () => {
      const textData = createBasicTextData();

      const descent = TextEntityThreejsRenderer.getDescent(textData);

      expect(descent).toBe(2.0);
    });

    it('should return 0 when descent not set', () => {
      const textData = createBasicTextData();
      textData.Descent = undefined;

      const descent = TextEntityThreejsRenderer.getDescent(textData);

      expect(descent).toBe(0);
    });
  });

  describe('getCharacterCount', () => {
    it('should return correct character count', () => {
      const textData = createBasicTextData();

      const characterCount = TextEntityThreejsRenderer.getCharacterCount(textData);

      expect(characterCount).toBe(11);
    });

    it('should return 0 when character count not set', () => {
      const textData = createBasicTextData();
      textData.CharacterCount = undefined;

      const characterCount = TextEntityThreejsRenderer.getCharacterCount(textData);

      expect(characterCount).toBe(0);
    });
  });

  describe('getFontStyle', () => {
    it('should return correct font style', () => {
      const textData = createBasicTextData();

      const fontStyle = TextEntityThreejsRenderer.getFontStyle(textData);

      expect(fontStyle).toBe('Regular');
    });

    it('should return default font style when not set', () => {
      const textData = createBasicTextData();
      textData.FontStyle = undefined;

      const fontStyle = TextEntityThreejsRenderer.getFontStyle(textData);

      expect(fontStyle).toBe('Regular');
    });
  });

  describe('isBold', () => {
    it('should return true when bold', () => {
      const textData = createBasicTextData();
      textData.IsBold = true;

      const isBold = TextEntityThreejsRenderer.isBold(textData);

      expect(isBold).toBe(true);
    });

    it('should return false when not bold', () => {
      const textData = createBasicTextData();

      const isBold = TextEntityThreejsRenderer.isBold(textData);

      expect(isBold).toBe(false);
    });
  });

  describe('isItalic', () => {
    it('should return true when italic', () => {
      const textData = createBasicTextData();
      textData.IsItalic = true;

      const isItalic = TextEntityThreejsRenderer.isItalic(textData);

      expect(isItalic).toBe(true);
    });

    it('should return false when not italic', () => {
      const textData = createBasicTextData();

      const isItalic = TextEntityThreejsRenderer.isItalic(textData);

      expect(isItalic).toBe(false);
    });
  });
});
