import * as THREE from 'three';
import { InsertEntityThreejsRenderer, InsertData, EntityData, AttributeData } from '../InsertEntityThreejsRenderer';

describe('InsertEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
  });

  describe('render', () => {
    it('should render a basic insert', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-1',
        EntityType: 'INSERT',
        Handle: 'handle-1',
        LayerName: 'TEST_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 1,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'TEST_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.visible).toBe(true);
      expect(group.castShadow).toBe(true);
      expect(group.receiveShadow).toBe(true);
      expect(group.userData.type).toBe('Insert');
      expect(group.userData.uuid).toBe('test-uuid-1');
      expect(group.userData.entityType).toBe('INSERT');
      expect(group.userData.handle).toBe('handle-1');
      expect(group.userData.layerName).toBe('TEST_LAYER');
      expect(group.userData.blockName).toBe('TEST_BLOCK');
    });

    it('should render insert with scale', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-2',
        EntityType: 'INSERT',
        Handle: 'handle-2',
        LayerName: 'SCALE_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 2,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 2,
        ScaleY: 3,
        ScaleZ: 1.5,
        Rotation: 0,
        BlockName: 'SCALE_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.userData.scaleX).toBe(2);
      expect(group.userData.scaleY).toBe(3);
      expect(group.userData.scaleZ).toBe(1.5);
    });

    it('should render insert with rotation', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-3',
        EntityType: 'INSERT',
        Handle: 'handle-3',
        LayerName: 'ROTATION_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 3,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: Math.PI / 4,
        BlockName: 'ROTATION_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.userData.rotation).toBe(Math.PI / 4);
    });

    it('should return null for invisible insert', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-4',
        EntityType: 'INSERT',
        Handle: 'handle-4',
        LayerName: 'INVISIBLE_LAYER',
        Visible: true,
        IsInvisible: true,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 1,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'INVISIBLE_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeNull();
    });

    it('should return null for non-visible insert', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-5',
        EntityType: 'INSERT',
        Handle: 'handle-5',
        LayerName: 'HIDDEN_LAYER',
        Visible: false,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 1,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'HIDDEN_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeNull();
    });

    it('should render insert with multiple insertions', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-6',
        EntityType: 'INSERT',
        Handle: 'handle-6',
        LayerName: 'MULTIPLE_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 4,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'MULTIPLE_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 3,
        ColumnCount: 2,
        RowSpacing: 10,
        ColumnSpacing: 15,
        IsMultiple: true,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.userData.isMultiple).toBe(true);
      expect(group.userData.rowCount).toBe(3);
      expect(group.userData.columnCount).toBe(2);
      expect(group.userData.rowSpacing).toBe(10);
      expect(group.userData.columnSpacing).toBe(15);
    });

    it('should render insert with attributes', () => {
      const attributeData: AttributeData = {
        Tag: 'TAG1',
        Value: 'VALUE1',
        Position: { X: 5, Y: 5, Z: 0 },
        Height: 2.5,
        ColorIndex: 1,
        Style: 'STANDARD',
        Rotation: 0,
        WidthFactor: 1,
        ObliqueAngle: 0
      };

      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-7',
        EntityType: 'INSERT',
        Handle: 'handle-7',
        LayerName: 'ATTRIBUTE_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 5,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'ATTRIBUTE_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [attributeData],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.userData.attributes).toBeDefined();
      expect(group.userData.attributes.length).toBe(1);
      expect(group.userData.attributes[0].tag).toBe('TAG1');
      expect(group.userData.attributes[0].value).toBe('VALUE1');
    });

    it('should apply transform matrix when provided', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-8',
        EntityType: 'INSERT',
        Handle: 'handle-8',
        LayerName: 'TRANSFORM_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 6,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'TRANSFORM_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
        Transparency: 0,
        MaterialType: 'MeshBasicMaterial',
        MaterialSide: 2,
        MaterialDepthTest: true,
        MaterialDepthWrite: true,
        MaterialTransparent: false,
        MaterialOpacity: 1,
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true,
        Transform3D: {
          Matrix: [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            10, 20, 30, 1
          ],
          Position: { X: 10, Y: 20, Z: 30 },
          Rotation: { X: 0, Y: 0, Z: 0 },
          Scale: { X: 1, Y: 1, Z: 1 }
        },
        CastShadow: true,
        ReceiveShadow: true,
        RenderOrder: 0
      };

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.position.x).toBe(10);
      expect(group.position.y).toBe(20);
      expect(group.position.z).toBe(30);
    });

    it('should render insert with elevation', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-9',
        EntityType: 'INSERT',
        Handle: 'handle-9',
        LayerName: 'ELEVATION_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 10,
        ColorIndex: 7,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'ELEVATION_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 10 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 10,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 10 },
          Max: { X: 0, Y: 0, Z: 10 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.userData.elevation).toBe(10);
    });

    it('should render insert with transparency', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-10',
        EntityType: 'INSERT',
        Handle: 'handle-10',
        LayerName: 'TRANSPARENT_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 8,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'TRANSPARENT_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.userData.transparency).toBe(0.5);
    });

    it('should set render order correctly', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-11',
        EntityType: 'INSERT',
        Handle: 'handle-11',
        LayerName: 'RENDER_ORDER_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 9,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'RENDER_ORDER_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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
        RenderOrder: 100
      };

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.renderOrder).toBe(100);
    });

    it('should render insert with entities', () => {
      const entityData: EntityData = {
        Type: 'Line',
        LineData: {
          Type: 'Line',
          Uuid: 'entity-uuid-1',
          EntityType: 'LINE',
          Handle: 'entity-handle-1',
          LayerName: 'ENTITY_LAYER',
          Visible: true,
          IsInvisible: false,
          StartX: 0,
          StartY: 0,
          StartZ: 0,
          EndX: 10,
          EndY: 10,
          EndZ: 0,
          ColorIndex: 1,
          LineTypeName: 'CONTINUOUS',
          LineWeight: 0.25,
          LineTypeScale: 1.0,
          Length: 14.14,
          Centroid: { X: 5, Y: 5 },
          Centroid3D: { X: 5, Y: 5, Z: 0 },
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Elevation: 0,
          Bounds: {
            Min: { X: 0, Y: 0, Z: 0 },
            Max: { X: 10, Y: 10, Z: 0 }
          },
          Vertices: [0, 0, 0, 10, 10, 0],
          Transparency: 0,
          MaterialType: 'LineBasicMaterial',
          MaterialDepthTest: true,
          MaterialDepthWrite: true,
          MaterialTransparent: false,
          MaterialOpacity: 1,
          CoordinateSystem: 'AutoCAD',
          RequiresYAxisFlip: true,
          CastShadow: true,
          ReceiveShadow: true,
          RenderOrder: 0
        }
      };

      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-12',
        EntityType: 'INSERT',
        Handle: 'handle-12',
        LayerName: 'ENTITIES_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 10,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'ENTITIES_BLOCK',
        EntityCount: 1,
        Entities: [entityData],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 5, Y: 5 },
        Centroid3D: { X: 5, Y: 5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        Vertices: [0, 0, 0, 10, 10, 0],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.userData.entityCount).toBe(1);
      expect(group.children.length).toBeGreaterThan(0);
    });

    it('should handle insert with null data', () => {
      const group = InsertEntityThreejsRenderer.render(null as any, scene);

      expect(group).toBeNull();
    });

    it('should handle insert with undefined data', () => {
      const group = InsertEntityThreejsRenderer.render(undefined as any, scene);

      expect(group).toBeNull();
    });

    it('should add group to scene', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-13',
        EntityType: 'INSERT',
        Handle: 'handle-13',
        LayerName: 'SCENE_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 11,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'SCENE_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(scene.children).toContain(group);
    });

    it('should set correct group name', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-14',
        EntityType: 'INSERT',
        Handle: 'handle-14',
        LayerName: 'NAME_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 12,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'NAME_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group.name).toBe('Insert_NAME_BLOCK_test-uuid-14');
    });

    it('should handle insert with custom normal', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-15',
        EntityType: 'INSERT',
        Handle: 'handle-15',
        LayerName: 'NORMAL_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 13,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'NORMAL_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0.577,
        NormalY: 0.577,
        NormalZ: 0.577,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
    });

    it('should handle insert with large coordinates', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-16',
        EntityType: 'INSERT',
        Handle: 'handle-16',
        LayerName: 'LARGE_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 1000,
        InsertPointY: 2000,
        InsertPointZ: 3000,
        ColorIndex: 14,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'LARGE_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 1000, Y: 2000 },
        Centroid3D: { X: 1000, Y: 2000, Z: 3000 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 3000,
        Bounds: {
          Min: { X: 1000, Y: 2000, Z: 3000 },
          Max: { X: 1000, Y: 2000, Z: 3000 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
    });

    it('should handle insert with negative coordinates', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-17',
        EntityType: 'INSERT',
        Handle: 'handle-17',
        LayerName: 'NEGATIVE_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: -100,
        InsertPointY: -200,
        InsertPointZ: -300,
        ColorIndex: 15,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'NEGATIVE_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: -100, Y: -200 },
        Centroid3D: { X: -100, Y: -200, Z: -300 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: -300,
        Bounds: {
          Min: { X: -100, Y: -200, Z: -300 },
          Max: { X: -100, Y: -200, Z: -300 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
    });

    it('should handle insert with zero scale', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-18',
        EntityType: 'INSERT',
        Handle: 'handle-18',
        LayerName: 'ZERO_SCALE_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 16,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 0,
        ScaleY: 0,
        ScaleZ: 0,
        Rotation: 0,
        BlockName: 'ZERO_SCALE_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.userData.scaleX).toBe(0);
      expect(group.userData.scaleY).toBe(0);
      expect(group.userData.scaleZ).toBe(0);
    });

    it('should handle insert with rotation 360 degrees', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-19',
        EntityType: 'INSERT',
        Handle: 'handle-19',
        LayerName: 'FULL_ROTATION_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 17,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 2 * Math.PI,
        BlockName: 'FULL_ROTATION_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.userData.rotation).toBe(2 * Math.PI);
    });

    it('should handle insert with negative rotation', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-20',
        EntityType: 'INSERT',
        Handle: 'handle-20',
        LayerName: 'NEGATIVE_ROTATION_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 18,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: -Math.PI / 4,
        BlockName: 'NEGATIVE_ROTATION_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.userData.rotation).toBe(-Math.PI / 4);
    });

    it('should handle insert with multiple attributes', () => {
      const attributeData1: AttributeData = {
        Tag: 'TAG1',
        Value: 'VALUE1',
        Position: { X: 5, Y: 5, Z: 0 },
        Height: 2.5,
        ColorIndex: 1,
        Style: 'STANDARD',
        Rotation: 0,
        WidthFactor: 1,
        ObliqueAngle: 0
      };

      const attributeData2: AttributeData = {
        Tag: 'TAG2',
        Value: 'VALUE2',
        Position: { X: 10, Y: 10, Z: 0 },
        Height: 3.0,
        ColorIndex: 2,
        Style: 'ROMANS',
        Rotation: Math.PI / 6,
        WidthFactor: 1.2,
        ObliqueAngle: 0.1
      };

      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-21',
        EntityType: 'INSERT',
        Handle: 'handle-21',
        LayerName: 'MULTIPLE_ATTRIBUTES_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 19,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'MULTIPLE_ATTRIBUTES_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [attributeData1, attributeData2],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.userData.attributes).toBeDefined();
      expect(group.userData.attributes.length).toBe(2);
      expect(group.userData.attributes[0].tag).toBe('TAG1');
      expect(group.userData.attributes[1].tag).toBe('TAG2');
    });
  });

  describe('renderEntities', () => {
    it('should render empty entities list', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-22',
        EntityType: 'INSERT',
        Handle: 'handle-22',
        LayerName: 'EMPTY_ENTITIES_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 20,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'EMPTY_ENTITIES_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.userData.entityCount).toBe(0);
    });

    it('should render multiple entities', () => {
      const entityData1: EntityData = {
        Type: 'Line',
        LineData: {
          Type: 'Line',
          Uuid: 'entity-uuid-1',
          EntityType: 'LINE',
          Handle: 'entity-handle-1',
          LayerName: 'ENTITY_LAYER',
          Visible: true,
          IsInvisible: false,
          StartX: 0,
          StartY: 0,
          StartZ: 0,
          EndX: 10,
          EndY: 0,
          EndZ: 0,
          ColorIndex: 1,
          LineTypeName: 'CONTINUOUS',
          LineWeight: 0.25,
          LineTypeScale: 1.0,
          Length: 10,
          Centroid: { X: 5, Y: 0 },
          Centroid3D: { X: 5, Y: 0, Z: 0 },
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Elevation: 0,
          Bounds: {
            Min: { X: 0, Y: 0, Z: 0 },
            Max: { X: 10, Y: 0, Z: 0 }
          },
          Vertices: [0, 0, 0, 10, 0, 0],
          Transparency: 0,
          MaterialType: 'LineBasicMaterial',
          MaterialDepthTest: true,
          MaterialDepthWrite: true,
          MaterialTransparent: false,
          MaterialOpacity: 1,
          CoordinateSystem: 'AutoCAD',
          RequiresYAxisFlip: true,
          CastShadow: true,
          ReceiveShadow: true,
          RenderOrder: 0
        }
      };

      const entityData2: EntityData = {
        Type: 'Line',
        LineData: {
          Type: 'Line',
          Uuid: 'entity-uuid-2',
          EntityType: 'LINE',
          Handle: 'entity-handle-2',
          LayerName: 'ENTITY_LAYER',
          Visible: true,
          IsInvisible: false,
          StartX: 0,
          StartY: 0,
          StartZ: 0,
          EndX: 0,
          EndY: 10,
          EndZ: 0,
          ColorIndex: 2,
          LineTypeName: 'CONTINUOUS',
          LineWeight: 0.25,
          LineTypeScale: 1.0,
          Length: 10,
          Centroid: { X: 0, Y: 5 },
          Centroid3D: { X: 0, Y: 5, Z: 0 },
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Elevation: 0,
          Bounds: {
            Min: { X: 0, Y: 0, Z: 0 },
            Max: { X: 0, Y: 10, Z: 0 }
          },
          Vertices: [0, 0, 0, 0, 10, 0],
          Transparency: 0,
          MaterialType: 'LineBasicMaterial',
          MaterialDepthTest: true,
          MaterialDepthWrite: true,
          MaterialTransparent: false,
          MaterialOpacity: 1,
          CoordinateSystem: 'AutoCAD',
          RequiresYAxisFlip: true,
          CastShadow: true,
          ReceiveShadow: true,
          RenderOrder: 0
        }
      };

      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-23',
        EntityType: 'INSERT',
        Handle: 'handle-23',
        LayerName: 'MULTIPLE_ENTITIES_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 21,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'MULTIPLE_ENTITIES_BLOCK',
        EntityCount: 2,
        Entities: [entityData1, entityData2],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 2.5, Y: 2.5 },
        Centroid3D: { X: 2.5, Y: 2.5, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 }
        },
        Vertices: [0, 0, 0, 10, 0, 0, 0, 0, 0, 0, 10, 0],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.userData.entityCount).toBe(2);
    });
  });

  describe('renderAttributes', () => {
    it('should render empty attributes list', () => {
      const insertData: InsertData = {
        Type: 'Insert',
        Uuid: 'test-uuid-24',
        EntityType: 'INSERT',
        Handle: 'handle-24',
        LayerName: 'EMPTY_ATTRIBUTES_LAYER',
        Visible: true,
        IsInvisible: false,
        InsertPointX: 0,
        InsertPointY: 0,
        InsertPointZ: 0,
        ColorIndex: 22,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 0.25,
        LineTypeScale: 1.0,
        ScaleX: 1,
        ScaleY: 1,
        ScaleZ: 1,
        Rotation: 0,
        BlockName: 'EMPTY_ATTRIBUTES_BLOCK',
        EntityCount: 0,
        Entities: [],
        Attributes: [],
        RowCount: 1,
        ColumnCount: 1,
        RowSpacing: 0,
        ColumnSpacing: 0,
        IsMultiple: false,
        Centroid: { X: 0, Y: 0 },
        Centroid3D: { X: 0, Y: 0, Z: 0 },
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        Elevation: 0,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 }
        },
        Vertices: [],
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

      const group = InsertEntityThreejsRenderer.render(insertData, scene);

      expect(group).toBeInstanceOf(THREE.Group);
      expect(group).not.toBeNull();
      expect(group.userData.attributes).toBeDefined();
      expect(group.userData.attributes.length).toBe(0);
    });
  });
});
