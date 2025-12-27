import * as THREE from 'three';
import { MTextEntityThreejsRenderer, MTextData, Point3DData, PointData, BoundsData, ColorData, TransformData, NormalData } from '../MTextEntityThreejsRenderer';

describe('MTextEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
    
    global.document = {
      createElement: () => ({
        width: 0,
        height: 0,
        getContext: () => ({
          clearRect: () => {},
          font: '',
          textBaseline: '',
          fillStyle: '',
          fillText: () => {},
          measureText: () => ({ width: 0 })
        })
      })
    } as any;
  });

  const createBasicMTextData = (): MTextData => ({
    Type: 'MText',
    EntityType: 'MText',
    Handle: 'mtext-handle-1',
    LayerName: 'TEST_LAYER',
    LayerIndex: 0,
    Visible: true,
    CoordinateSystem: 'World',
    Lines: ['Test Text'],
    InsertPoint: { X: 10, Y: 10 },
    Height: 5,
    ColorIndex: 1,
    LineTypeName: 'CONTINUOUS',
    LineWeight: 0.03,
    RectangleWidth: 0,
    LineSpacingStyle: 1,
    LineSpacing: 1.0,
    AttachmentPoint: 1,
    Rotation: 0,
    TextStyleName: 'STANDARD',
    WidthFactor: 1.0,
    ObliqueAngle: 0.0,
    Bounds: {
      Min: { X: 10, Y: 10, Z: 0 },
      Max: { X: 50, Y: 15, Z: 0 },
      Center: { X: 30, Y: 12.5, Z: 0 },
      Size: { X: 40, Y: 5, Z: 0 }
    },
    AlignmentPoint: { X: 10, Y: 10 },
    LineCount: 1,
    Vertices3D: [],
    Centroid3D: { X: 10, Y: 10, Z: 0 },
    Normals: [],
    Bounds3D: {
      Min: { X: 10, Y: 10, Z: 0 },
      Max: { X: 50, Y: 15, Z: 0 }
    },
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
    MaterialType: 'TextMaterial',
    DepthTest: true,
    DepthWrite: true,
    DrawingDirection: 1,
    BackgroundColor: { Index: 0, Hex: '#000000', R: 0, G: 0, B: 0, A: 1.0 },
    BackgroundFillFlags: 0,
    BackgroundScale: 1.0,
    BackgroundTransparency: 0.0,
    HorizontalWidth: 0,
    RectangleHeight: 0,
    VerticalHeight: 0,
    IsAnnotative: false,
    PlainText: 'Test Text',
    TextValue: 'Test Text',
    InsertPoint3D: { X: 10, Y: 10, Z: 0 },
    AlignmentPoint3D: { X: 10, Y: 10, Z: 0 },
    TextWidth: 40,
    TextHeight: 5
  });

  describe('render', () => {
    it('should render a basic MText', () => {
      const mtextData = createBasicMTextData();

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.userData.type).toBe('MText');
      expect(group.userData.entityType).toBe('MText');
      expect(group.userData.handle).toBe('mtext-handle-1');
      expect(group.userData.layerName).toBe('TEST_LAYER');
    });

    it('should return null for invisible MText', () => {
      const mtextData = createBasicMTextData();
      mtextData.Visible = false;

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeNull();
    });

    it('should return null for null data', () => {
      const group = MTextEntityThreejsRenderer.render(null as any, scene);

      expect(group).toBeNull();
    });

    it('should render MText with correct text mesh', () => {
      const mtextData = createBasicMTextData();

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.children.length).toBeGreaterThan(0);
      
      const textMesh = group.children.find(child => child.name === 'Text');
      expect(textMesh).toBeDefined();
      expect(textMesh).toBeInstanceOf(THREE.Mesh);
    });

    it('should render MText with correct material', () => {
      const mtextData = createBasicMTextData();

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);
      const textMesh = group.children.find(child => child.name === 'Text') as THREE.Mesh;

      expect(textMesh.material).toBeDefined();
      expect(textMesh.material).toBeInstanceOf(THREE.MeshBasicMaterial);
      const material = textMesh.material as THREE.MeshBasicMaterial;
      expect(material.transparent).toBe(true);
      expect(material.opacity).toBe(1.0);
      expect(material.depthTest).toBe(true);
      expect(material.depthWrite).toBe(true);
    });

    it('should render MText with correct geometry', () => {
      const mtextData = createBasicMTextData();

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);
      const textMesh = group.children.find(child => child.name === 'Text') as THREE.Mesh;

      expect(textMesh.geometry).toBeDefined();
      expect(textMesh.geometry).toBeInstanceOf(THREE.PlaneGeometry);
    });

    it('should apply transform matrix if provided', () => {
      const mtextData = createBasicMTextData();
      mtextData.Transform.Matrix = [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 10, 20, 30, 1];

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group.position.x).toBe(10);
      expect(group.position.y).toBe(20);
      expect(group.position.z).toBe(30);
    });

    it('should render MText with multiple lines', () => {
      const mtextData = createBasicMTextData();
      mtextData.Lines = ['Line 1', 'Line 2', 'Line 3'];
      mtextData.LineCount = 3;

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.lineCount).toBe(3);
    });

    it('should render MText with background fill', () => {
      const mtextData = createBasicMTextData();
      mtextData.BackgroundFillFlags = 1;
      mtextData.BackgroundColor = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      
      const backgroundMesh = group.children.find(child => child.name === 'Background');
      expect(backgroundMesh).toBeDefined();
      expect(backgroundMesh).toBeInstanceOf(THREE.Mesh);
    });

    it('should render MText with bounds', () => {
      const mtextData = createBasicMTextData();

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      
      const boundsLine = group.children.find(child => child.name === 'Bounds');
      expect(boundsLine).toBeDefined();
    });

    it('should render MText with correct rotation', () => {
      const mtextData = createBasicMTextData();
      mtextData.Rotation = Math.PI / 4;

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.rotation).toBe(Math.PI / 4);
    });

    it('should render MText with correct height', () => {
      const mtextData = createBasicMTextData();
      mtextData.Height = 10;

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.height).toBe(10);
    });

    it('should render MText with correct attachment point', () => {
      const mtextData = createBasicMTextData();
      mtextData.AttachmentPoint = 5;

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.attachmentPoint).toBe(5);
    });

    it('should render MText with correct line spacing', () => {
      const mtextData = createBasicMTextData();
      mtextData.LineSpacing = 1.5;
      mtextData.LineSpacingStyle = 1;

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.lineSpacing).toBe(1.5);
      expect(group.userData.lineSpacingStyle).toBe(1);
    });

    it('should render MText with correct text style', () => {
      const mtextData = createBasicMTextData();
      mtextData.TextStyleName = 'MyStyle';
      mtextData.WidthFactor = 1.5;
      mtextData.ObliqueAngle = Math.PI / 6;

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.textStyleName).toBe('MyStyle');
      expect(group.userData.widthFactor).toBe(1.5);
      expect(group.userData.obliqueAngle).toBe(Math.PI / 6);
    });

    it('should render MText with correct drawing direction', () => {
      const mtextData = createBasicMTextData();
      mtextData.DrawingDirection = 2;

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.drawingDirection).toBe(2);
    });

    it('should render MText with correct background properties', () => {
      const mtextData = createBasicMTextData();
      mtextData.BackgroundFillFlags = 1;
      mtextData.BackgroundColor = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };
      mtextData.BackgroundScale = 1.2;
      mtextData.BackgroundTransparency = 0.5;

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.backgroundFillFlags).toBe(1);
      expect(group.userData.backgroundColor).toEqual({ Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 });
      expect(group.userData.backgroundScale).toBe(1.2);
      expect(group.userData.backgroundTransparency).toBe(0.5);
    });

    it('should render MText with correct 3D coordinates', () => {
      const mtextData = createBasicMTextData();
      mtextData.InsertPoint3D = { X: 10, Y: 20, Z: 30 };
      mtextData.AlignmentPoint3D = { X: 15, Y: 25, Z: 35 };

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.insertPoint3D).toEqual({ X: 10, Y: 20, Z: 30 });
      expect(group.userData.alignmentPoint3D).toEqual({ X: 15, Y: 25, Z: 35 });
    });

    it('should render MText with correct normal', () => {
      const mtextData = createBasicMTextData();
      mtextData.Normal = { X: 0, Y: 1, Z: 0 };

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.normal).toEqual({ X: 0, Y: 1, Z: 0 });
    });

    it('should render MText with correct color', () => {
      const mtextData = createBasicMTextData();
      mtextData.ColorIndex = 5;
      mtextData.Color = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.colorIndex).toBe(5);
      expect(group.userData.color).toEqual({ Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 });
    });

    it('should render MText with correct opacity', () => {
      const mtextData = createBasicMTextData();
      mtextData.Opacity = 0.5;
      mtextData.Transparent = true;

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.opacity).toBe(0.5);
      expect(group.userData.transparent).toBe(true);
    });

    it('should render MText with correct depth settings', () => {
      const mtextData = createBasicMTextData();
      mtextData.DepthTest = false;
      mtextData.DepthWrite = false;

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.depthTest).toBe(false);
      expect(group.userData.depthWrite).toBe(false);
    });

    it('should render MText with correct annotative flag', () => {
      const mtextData = createBasicMTextData();
      mtextData.IsAnnotative = true;

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.isAnnotative).toBe(true);
    });

    it('should render MText with correct text value', () => {
      const mtextData = createBasicMTextData();
      mtextData.TextValue = 'Custom Text';
      mtextData.PlainText = 'Custom Text';

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.textValue).toBe('Custom Text');
      expect(group.userData.plainText).toBe('Custom Text');
    });

    it('should render MText with correct text dimensions', () => {
      const mtextData = createBasicMTextData();
      mtextData.TextWidth = 100;
      mtextData.TextHeight = 20;

      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.userData.textWidth).toBe(100);
      expect(group.userData.textHeight).toBe(20);
    });
  });

  describe('update', () => {
    it('should update existing MText', () => {
      const mtextData = createBasicMTextData();
      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      const updatedMTextData = { ...mtextData };
      updatedMTextData.TextValue = 'Updated Text';
      updatedMTextData.Lines = ['Updated Text'];

      const result = MTextEntityThreejsRenderer.update(updatedMTextData, scene);

      expect(result).toBe(true);
    });

    it('should return false for non-existent MText', () => {
      const mtextData = createBasicMTextData();

      const result = MTextEntityThreejsRenderer.update(mtextData, scene);

      expect(result).toBe(false);
    });

    it('should update MText visibility', () => {
      const mtextData = createBasicMTextData();
      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      const updatedMTextData = { ...mtextData };
      updatedMTextData.Visible = false;

      const result = MTextEntityThreejsRenderer.update(updatedMTextData, scene);

      expect(result).toBe(true);
      expect(group.visible).toBe(false);
    });

    it('should update MText text', () => {
      const mtextData = createBasicMTextData();
      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      const updatedMTextData = { ...mtextData };
      updatedMTextData.TextValue = 'New Text';
      updatedMTextData.Lines = ['New Text'];

      const result = MTextEntityThreejsRenderer.update(updatedMTextData, scene);

      expect(result).toBe(true);
    });
  });

  describe('dispose', () => {
    it('should dispose MText resources', () => {
      const mtextData = createBasicMTextData();
      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      MTextEntityThreejsRenderer.dispose(mtextData, scene);

      expect(scene.children.length).toBe(0);
    });

    it('should handle non-existent MText gracefully', () => {
      const mtextData = createBasicMTextData();

      expect(() => {
        MTextEntityThreejsRenderer.dispose(mtextData, scene);
      }).not.toThrow();
    });

    it('should dispose MText with multiple children', () => {
      const mtextData = createBasicMTextData();
      mtextData.BackgroundFillFlags = 1;
      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      MTextEntityThreejsRenderer.dispose(mtextData, scene);

      expect(scene.children.length).toBe(0);
    });
  });

  describe('setVisibility', () => {
    it('should set MText visibility to true', () => {
      const mtextData = createBasicMTextData();
      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      MTextEntityThreejsRenderer.setVisibility(mtextData, scene, true);

      expect(group.visible).toBe(true);
    });

    it('should set MText visibility to false', () => {
      const mtextData = createBasicMTextData();
      const group = MTextEntityThreejsRenderer.render(mtextData, scene);

      MTextEntityThreejsRenderer.setVisibility(mtextData, scene, false);

      expect(group.visible).toBe(false);
    });

    it('should handle non-existent MText gracefully', () => {
      const mtextData = createBasicMTextData();

      expect(() => {
        MTextEntityThreejsRenderer.setVisibility(mtextData, scene, true);
      }).not.toThrow();
    });
  });

  describe('getLineCount', () => {
    it('should return correct line count', () => {
      const mtextData = createBasicMTextData();
      mtextData.LineCount = 3;

      const lineCount = MTextEntityThreejsRenderer.getLineCount(mtextData);

      expect(lineCount).toBe(3);
    });

    it('should return 0 for empty MText', () => {
      const mtextData = createBasicMTextData();
      mtextData.LineCount = 0;

      const lineCount = MTextEntityThreejsRenderer.getLineCount(mtextData);

      expect(lineCount).toBe(0);
    });
  });

  describe('getTextValue', () => {
    it('should return correct text value', () => {
      const mtextData = createBasicMTextData();
      mtextData.TextValue = 'Test Text';

      const textValue = MTextEntityThreejsRenderer.getTextValue(mtextData);

      expect(textValue).toBe('Test Text');
    });

    it('should return empty string for null text value', () => {
      const mtextData = createBasicMTextData();
      mtextData.TextValue = '';

      const textValue = MTextEntityThreejsRenderer.getTextValue(mtextData);

      expect(textValue).toBe('');
    });
  });

  describe('getPlainText', () => {
    it('should return correct plain text', () => {
      const mtextData = createBasicMTextData();
      mtextData.PlainText = 'Plain Text';

      const plainText = MTextEntityThreejsRenderer.getPlainText(mtextData);

      expect(plainText).toBe('Plain Text');
    });

    it('should return empty string for null plain text', () => {
      const mtextData = createBasicMTextData();
      mtextData.PlainText = '';

      const plainText = MTextEntityThreejsRenderer.getPlainText(mtextData);

      expect(plainText).toBe('');
    });
  });

  describe('getInsertPoint', () => {
    it('should return correct insert point', () => {
      const mtextData = createBasicMTextData();
      mtextData.InsertPoint = { X: 10, Y: 20 };

      const insertPoint = MTextEntityThreejsRenderer.getInsertPoint(mtextData);

      expect(insertPoint).toEqual({ X: 10, Y: 20 });
    });
  });

  describe('getAlignmentPoint', () => {
    it('should return correct alignment point', () => {
      const mtextData = createBasicMTextData();
      mtextData.AlignmentPoint = { X: 15, Y: 25 };

      const alignmentPoint = MTextEntityThreejsRenderer.getAlignmentPoint(mtextData);

      expect(alignmentPoint).toEqual({ X: 15, Y: 25 });
    });
  });

  describe('getRotation', () => {
    it('should return correct rotation', () => {
      const mtextData = createBasicMTextData();
      mtextData.Rotation = Math.PI / 4;

      const rotation = MTextEntityThreejsRenderer.getRotation(mtextData);

      expect(rotation).toBe(Math.PI / 4);
    });

    it('should return 0 for no rotation', () => {
      const mtextData = createBasicMTextData();
      mtextData.Rotation = 0;

      const rotation = MTextEntityThreejsRenderer.getRotation(mtextData);

      expect(rotation).toBe(0);
    });
  });

  describe('getHeight', () => {
    it('should return correct height', () => {
      const mtextData = createBasicMTextData();
      mtextData.Height = 10;

      const height = MTextEntityThreejsRenderer.getHeight(mtextData);

      expect(height).toBe(10);
    });
  });

  describe('getAttachmentPoint', () => {
    it('should return correct attachment point', () => {
      const mtextData = createBasicMTextData();
      mtextData.AttachmentPoint = 5;

      const attachmentPoint = MTextEntityThreejsRenderer.getAttachmentPoint(mtextData);

      expect(attachmentPoint).toBe(5);
    });
  });

  describe('getLineSpacing', () => {
    it('should return correct line spacing', () => {
      const mtextData = createBasicMTextData();
      mtextData.LineSpacing = 1.5;

      const lineSpacing = MTextEntityThreejsRenderer.getLineSpacing(mtextData);

      expect(lineSpacing).toBe(1.5);
    });
  });

  describe('getLineSpacingStyle', () => {
    it('should return correct line spacing style', () => {
      const mtextData = createBasicMTextData();
      mtextData.LineSpacingStyle = 1;

      const lineSpacingStyle = MTextEntityThreejsRenderer.getLineSpacingStyle(mtextData);

      expect(lineSpacingStyle).toBe(1);
    });
  });

  describe('getTextStyleName', () => {
    it('should return correct text style name', () => {
      const mtextData = createBasicMTextData();
      mtextData.TextStyleName = 'MyStyle';

      const textStyleName = MTextEntityThreejsRenderer.getTextStyleName(mtextData);

      expect(textStyleName).toBe('MyStyle');
    });
  });

  describe('getWidthFactor', () => {
    it('should return correct width factor', () => {
      const mtextData = createBasicMTextData();
      mtextData.WidthFactor = 1.5;

      const widthFactor = MTextEntityThreejsRenderer.getWidthFactor(mtextData);

      expect(widthFactor).toBe(1.5);
    });
  });

  describe('getObliqueAngle', () => {
    it('should return correct oblique angle', () => {
      const mtextData = createBasicMTextData();
      mtextData.ObliqueAngle = Math.PI / 6;

      const obliqueAngle = MTextEntityThreejsRenderer.getObliqueAngle(mtextData);

      expect(obliqueAngle).toBe(Math.PI / 6);
    });
  });

  describe('getDrawingDirection', () => {
    it('should return correct drawing direction', () => {
      const mtextData = createBasicMTextData();
      mtextData.DrawingDirection = 2;

      const drawingDirection = MTextEntityThreejsRenderer.getDrawingDirection(mtextData);

      expect(drawingDirection).toBe(2);
    });
  });

  describe('getBackgroundFillFlags', () => {
    it('should return correct background fill flags', () => {
      const mtextData = createBasicMTextData();
      mtextData.BackgroundFillFlags = 1;

      const backgroundFillFlags = MTextEntityThreejsRenderer.getBackgroundFillFlags(mtextData);

      expect(backgroundFillFlags).toBe(1);
    });
  });

  describe('getBackgroundColor', () => {
    it('should return correct background color', () => {
      const mtextData = createBasicMTextData();
      mtextData.BackgroundColor = { Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 };

      const backgroundColor = MTextEntityThreejsRenderer.getBackgroundColor(mtextData);

      expect(backgroundColor).toEqual({ Index: 5, Hex: '#00FF00', R: 0, G: 255, B: 0, A: 1.0 });
    });
  });

  describe('getBackgroundScale', () => {
    it('should return correct background scale', () => {
      const mtextData = createBasicMTextData();
      mtextData.BackgroundScale = 1.2;

      const backgroundScale = MTextEntityThreejsRenderer.getBackgroundScale(mtextData);

      expect(backgroundScale).toBe(1.2);
    });
  });

  describe('getBackgroundTransparency', () => {
    it('should return correct background transparency', () => {
      const mtextData = createBasicMTextData();
      mtextData.BackgroundTransparency = 0.5;

      const backgroundTransparency = MTextEntityThreejsRenderer.getBackgroundTransparency(mtextData);

      expect(backgroundTransparency).toBe(0.5);
    });
  });

  describe('getIsAnnotative', () => {
    it('should return correct annotative flag', () => {
      const mtextData = createBasicMTextData();
      mtextData.IsAnnotative = true;

      const isAnnotative = MTextEntityThreejsRenderer.getIsAnnotative(mtextData);

      expect(isAnnotative).toBe(true);
    });
  });

  describe('getBounds', () => {
    it('should return correct bounds', () => {
      const mtextData = createBasicMTextData();
      mtextData.Bounds = {
        Min: { X: 10, Y: 10, Z: 0 },
        Max: { X: 50, Y: 15, Z: 0 },
        Center: { X: 30, Y: 12.5, Z: 0 },
        Size: { X: 40, Y: 5, Z: 0 }
      };

      const bounds = MTextEntityThreejsRenderer.getBounds(mtextData);

      expect(bounds).toEqual({
        Min: { X: 10, Y: 10, Z: 0 },
        Max: { X: 50, Y: 15, Z: 0 },
        Center: { X: 30, Y: 12.5, Z: 0 },
        Size: { X: 40, Y: 5, Z: 0 }
      });
    });
  });

  describe('getInsertPoint3D', () => {
    it('should return correct 3D insert point', () => {
      const mtextData = createBasicMTextData();
      mtextData.InsertPoint3D = { X: 10, Y: 20, Z: 30 };

      const insertPoint3D = MTextEntityThreejsRenderer.getInsertPoint3D(mtextData);

      expect(insertPoint3D).toEqual({ X: 10, Y: 20, Z: 30 });
    });
  });

  describe('getAlignmentPoint3D', () => {
    it('should return correct 3D alignment point', () => {
      const mtextData = createBasicMTextData();
      mtextData.AlignmentPoint3D = { X: 15, Y: 25, Z: 35 };

      const alignmentPoint3D = MTextEntityThreejsRenderer.getAlignmentPoint3D(mtextData);

      expect(alignmentPoint3D).toEqual({ X: 15, Y: 25, Z: 35 });
    });
  });
});