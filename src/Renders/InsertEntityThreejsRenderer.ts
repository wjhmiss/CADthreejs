import * as THREE from 'three';

export interface Point3DData {
  X: number;
  Y: number;
  Z: number;
}

export interface PointData {
  X: number;
  Y: number;
}

export interface BoundsData {
  Min: Point3DData;
  Max: Point3DData;
}

export interface MatrixData {
  M11: number;
  M12: number;
  M13: number;
  M14: number;
  M21: number;
  M22: number;
  M23: number;
  M24: number;
  M31: number;
  M32: number;
  M33: number;
  M34: number;
  M41: number;
  M42: number;
  M43: number;
  M44: number;
}

export interface TransformData {
  Position: Point3DData;
  Rotation: Point3DData;
  Scale: Point3DData;
  Matrix: number[];
}

export interface AttributeData {
  Tag: string;
  Value: string;
  Position: Point3DData;
  Height: number;
  ColorIndex: number;
  Style: string;
  Rotation: number;
  WidthFactor: number;
  ObliqueAngle: number;
}

export interface TextData {
  Value: string;
  PositionX: number;
  PositionY: number;
  Height: number;
  ColorIndex: number;
  Style: string;
}

export interface EntityData {
  Type: string;
  LineData?: any;
  ArcData?: any;
  CircleData?: any;
  LwPolylineData?: any;
  TextData?: TextData;
  EllipseData?: any;
  Face3DData?: any;
}

export interface InsertData {
  Type: string;
  Uuid: string;
  EntityType: string;
  InsertPointX: number;
  InsertPointY: number;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  Entities: EntityData[];
  Transform: MatrixData;
  ScaleX: number;
  ScaleY: number;
  ScaleZ: number;
  Rotation: number;
  EntityCount: number;
  Handle: string;
  BlockName: string;
  LayerName: string;
  IsInvisible: boolean;
  LineTypeScale: number;
  Transparency: number;
  ColorHex: string;
  ColorR: number;
  ColorG: number;
  ColorB: number;
  ColorA: number;
  MaterialType: string;
  MaterialTransparent: boolean;
  MaterialOpacity: number;
  MaterialDepthTest: boolean;
  MaterialDepthWrite: boolean;
  MaterialSide: number;
  Position: Point3DData;
  Bounds: BoundsData;
  Centroid: Point3DData;
  NormalX: number;
  NormalY: number;
  NormalZ: number;
  Vertices: Point3DData[];
  VerticesArray: number[];
  Indices: number[];
  Normals: number[];
  UVs: number[];
  CoordinateSystem: string;
  RequiresYAxisFlip: boolean;
  Transform3D: TransformData;
  Attributes: AttributeData[];
  RowCount: number;
  ColumnCount: number;
  RowSpacing: number;
  ColumnSpacing: number;
  IsMultiple: boolean;
  HasSpatialFilter: boolean;
  SpatialFilterName: string;
  Parent: any;
  Visible: boolean;
  CastShadow: boolean;
  ReceiveShadow: boolean;
  RenderOrder: number;
}

export class InsertEntityThreejsRenderer {
  private static insertCache = new Map<string, THREE.Object3D[]>();

  public static render(insertData: InsertData, scene: THREE.Scene): THREE.Object3D[] | null {
    if (!insertData || !insertData.Visible || insertData.IsInvisible) {
      return null;
    }

    const objects: THREE.Object3D[] = [];

    const entityObjects = this.renderEntities(insertData);
    if (entityObjects && entityObjects.length > 0) {
      objects.push(...entityObjects);
    }

    const attributeObjects = this.renderAttributes(insertData);
    if (attributeObjects && attributeObjects.length > 0) {
      objects.push(...attributeObjects);
    }

    const insertDataUserData = {
      type: insertData.Type,
      uuid: insertData.Uuid,
      entityType: insertData.EntityType,
      handle: insertData.Handle,
      blockName: insertData.BlockName,
      layerName: insertData.LayerName,
      colorIndex: insertData.ColorIndex,
      scaleX: insertData.ScaleX,
      scaleY: insertData.ScaleY,
      scaleZ: insertData.ScaleZ,
      rotation: insertData.Rotation,
      entityCount: insertData.EntityCount,
      isMultiple: insertData.IsMultiple,
      rowCount: insertData.RowCount,
      columnCount: insertData.ColumnCount,
      rowSpacing: insertData.RowSpacing,
      columnSpacing: insertData.ColumnSpacing,
      hasSpatialFilter: insertData.HasSpatialFilter,
      spatialFilterName: insertData.SpatialFilterName,
      attributes: insertData.Attributes.map(attr => ({
        tag: attr.Tag,
        value: attr.Value,
        position: attr.Position,
        height: attr.Height,
        colorIndex: attr.ColorIndex,
        style: attr.Style,
        rotation: attr.Rotation,
        widthFactor: attr.WidthFactor,
        obliqueAngle: attr.ObliqueAngle
      })),
      elevation: insertData.InsertPointZ,
      transparency: insertData.Transparency,
      objectType: 'Insert'
    };

    for (const obj of objects) {
      obj.visible = insertData.Visible;
      obj.castShadow = insertData.CastShadow;
      obj.receiveShadow = insertData.ReceiveShadow;
      obj.renderOrder = insertData.RenderOrder;
      obj.userData = { ...insertDataUserData, ...obj.userData };
      obj.name = `Insert_${insertData.BlockName}_${insertData.Uuid}_${obj.name}`;

      if (insertData.Transform3D && insertData.Transform3D.Matrix) {
        const matrix = new THREE.Matrix4();
        matrix.fromArray(insertData.Transform3D.Matrix);
        obj.applyMatrix4(matrix);
      }
    }

    this.insertCache.set(insertData.Uuid, objects);

    return objects;
  }

  private static renderEntities(insertData: InsertData): THREE.Object3D[] {
    if (!insertData.Entities || insertData.Entities.length === 0) {
      return [];
    }

    const objects: THREE.Object3D[] = [];

    for (const entityData of insertData.Entities) {
      try {
        const entityObject = this.renderEntity(entityData);
        if (entityObject) {
          objects.push(entityObject);
        }
      } catch (error) {
        console.error(`Error rendering entity of type ${entityData.Type}:`, error);
      }
    }

    return objects;
  }

  private static renderEntity(entityData: EntityData): THREE.Object3D | null {
    if (!entityData) {
      return null;
    }

    switch (entityData.Type) {
      case 'Line':
        return this.renderLine(entityData.LineData);
      case 'Arc':
        return this.renderArc(entityData.ArcData);
      case 'Circle':
        return this.renderCircle(entityData.CircleData);
      case 'LwPolyline':
        return this.renderLwPolyline(entityData.LwPolylineData);
      case 'TextEntity':
        return this.renderText(entityData.TextData);
      case 'Ellipse':
        return this.renderEllipse(entityData.EllipseData);
      case 'Face3D':
        return this.renderFace3D(entityData.Face3DData);
      default:
        console.warn(`Unsupported entity type: ${entityData.Type}`);
        return null;
    }
  }

  private static renderLine(lineData: any): THREE.Line | null {
    if (!lineData || !lineData.VerticesArray || lineData.VerticesArray.length < 6) {
      return null;
    }

    const geometry = new THREE.BufferGeometry();
    const vertices = new Float32Array(lineData.VerticesArray);
    geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));

    const material = new THREE.LineBasicMaterial({
      color: lineData.ColorHex || 0x000000,
      transparent: lineData.MaterialTransparent || false,
      opacity: lineData.MaterialOpacity || 1.0,
      depthTest: lineData.MaterialDepthTest !== false,
      depthWrite: lineData.MaterialDepthWrite !== false
    });

    const line = new THREE.Line(geometry, material);
    line.userData = lineData;

    return line;
  }

  private static renderArc(arcData: any): THREE.Line | null {
    if (!arcData || !arcData.VerticesArray || arcData.VerticesArray.length === 0) {
      return null;
    }

    const geometry = new THREE.BufferGeometry();
    const vertices = new Float32Array(arcData.VerticesArray);
    geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));

    const material = new THREE.LineBasicMaterial({
      color: arcData.ColorHex || 0x000000,
      transparent: arcData.MaterialTransparent || false,
      opacity: arcData.MaterialOpacity || 1.0,
      depthTest: arcData.MaterialDepthTest !== false,
      depthWrite: arcData.MaterialDepthWrite !== false
    });

    const arc = new THREE.Line(geometry, material);
    arc.userData = arcData;

    return arc;
  }

  private static renderCircle(circleData: any): THREE.Line | null {
    if (!circleData || !circleData.VerticesArray || circleData.VerticesArray.length === 0) {
      return null;
    }

    const geometry = new THREE.BufferGeometry();
    const vertices = new Float32Array(circleData.VerticesArray);
    geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));

    const material = new THREE.LineBasicMaterial({
      color: circleData.ColorHex || 0x000000,
      transparent: circleData.MaterialTransparent || false,
      opacity: circleData.MaterialOpacity || 1.0,
      depthTest: circleData.MaterialDepthTest !== false,
      depthWrite: circleData.MaterialDepthWrite !== false
    });

    const circle = new THREE.Line(geometry, material);
    circle.userData = circleData;

    return circle;
  }

  private static renderLwPolyline(lwPolylineData: any): THREE.Line | null {
    if (!lwPolylineData || !lwPolylineData.VerticesArray || lwPolylineData.VerticesArray.length < 6) {
      return null;
    }

    const geometry = new THREE.BufferGeometry();
    const vertices = new Float32Array(lwPolylineData.VerticesArray);
    geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));

    const material = new THREE.LineBasicMaterial({
      color: lwPolylineData.ColorHex || 0x000000,
      transparent: lwPolylineData.MaterialTransparent || false,
      opacity: lwPolylineData.MaterialOpacity || 1.0,
      depthTest: lwPolylineData.MaterialDepthTest !== false,
      depthWrite: lwPolylineData.MaterialDepthWrite !== false
    });

    const polyline = new THREE.Line(geometry, material);
    polyline.userData = lwPolylineData;

    return polyline;
  }

  private static renderText(textData: TextData | undefined): THREE.Sprite | null {
    if (!textData || !textData.Value) {
      return null;
    }

    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d');
    if (!context) {
      return null;
    }

    const fontSize = textData.Height * 10 || 48;
    context.font = `${fontSize}px Arial`;
    const textWidth = context.measureText(textData.Value).width;

    canvas.width = textWidth + 20;
    canvas.height = fontSize + 20;

    context.font = `${fontSize}px Arial`;
    context.fillStyle = '#ffffff';
    context.textAlign = 'center';
    context.textBaseline = 'middle';
    context.fillText(textData.Value, canvas.width / 2, canvas.height / 2);

    const texture = new THREE.CanvasTexture(canvas);
    const material = new THREE.SpriteMaterial({ map: texture, transparent: true });
    const sprite = new THREE.Sprite(material);

    sprite.scale.set(textWidth / 10, fontSize / 10, 1);
    sprite.position.set(textData.PositionX, textData.PositionY, 0);

    sprite.userData = textData;

    return sprite;
  }

  private static renderEllipse(ellipseData: any): THREE.Line | null {
    if (!ellipseData || !ellipseData.VerticesArray || ellipseData.VerticesArray.length === 0) {
      return null;
    }

    const geometry = new THREE.BufferGeometry();
    const vertices = new Float32Array(ellipseData.VerticesArray);
    geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));

    const material = new THREE.LineBasicMaterial({
      color: ellipseData.ColorHex || 0x000000,
      transparent: ellipseData.MaterialTransparent || false,
      opacity: ellipseData.MaterialOpacity || 1.0,
      depthTest: ellipseData.MaterialDepthTest !== false,
      depthWrite: ellipseData.MaterialDepthWrite !== false
    });

    const ellipse = new THREE.Line(geometry, material);
    ellipse.userData = ellipseData;

    return ellipse;
  }

  private static renderFace3D(face3DData: any): THREE.Mesh | null {
    if (!face3DData || !face3DData.VerticesArray || face3DData.VerticesArray.length < 9) {
      return null;
    }

    const geometry = new THREE.BufferGeometry();
    const vertices = new Float32Array(face3DData.VerticesArray);
    geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));

    if (face3DData.Normals && face3DData.Normals.length > 0) {
      const normals = new Float32Array(face3DData.Normals);
      geometry.setAttribute('normal', new THREE.BufferAttribute(normals, 3));
    } else {
      geometry.computeVertexNormals();
    }

    if (face3DData.Indices && face3DData.Indices.length > 0) {
      geometry.setIndex(face3DData.Indices);
    }

    const material = new THREE.MeshBasicMaterial({
      color: face3DData.ColorHex || 0x000000,
      side: face3DData.MaterialSide === 2 ? THREE.DoubleSide : THREE.FrontSide,
      transparent: face3DData.MaterialTransparent || false,
      opacity: face3DData.MaterialOpacity || 1.0,
      depthTest: face3DData.MaterialDepthTest !== false,
      depthWrite: face3DData.MaterialDepthWrite !== false
    });

    const mesh = new THREE.Mesh(geometry, material);
    mesh.userData = face3DData;

    return mesh;
  }

  private static renderAttributes(insertData: InsertData): THREE.Object3D[] {
    if (!insertData.Attributes || insertData.Attributes.length === 0) {
      return [];
    }

    const objects: THREE.Object3D[] = [];

    for (const attribute of insertData.Attributes) {
      try {
        const attributeObject = this.renderAttribute(attribute);
        if (attributeObject) {
          objects.push(attributeObject);
        }
      } catch (error) {
        console.error(`Error rendering attribute ${attribute.Tag}:`, error);
      }
    }

    return objects;
  }

  private static renderAttribute(attributeData: AttributeData): THREE.Sprite | null {
    if (!attributeData || !attributeData.Value) {
      return null;
    }

    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d');
    if (!context) {
      return null;
    }

    const fontSize = attributeData.Height * 10 || 48;
    context.font = `${fontSize}px Arial`;
    const textWidth = context.measureText(attributeData.Value).width;

    canvas.width = textWidth + 20;
    canvas.height = fontSize + 20;

    context.font = `${fontSize}px Arial`;
    context.fillStyle = '#ffffff';
    context.textAlign = 'center';
    context.textBaseline = 'middle';
    context.fillText(attributeData.Value, canvas.width / 2, canvas.height / 2);

    const texture = new THREE.CanvasTexture(canvas);
    const material = new THREE.SpriteMaterial({ map: texture, transparent: true });
    const sprite = new THREE.Sprite(material);

    sprite.scale.set(textWidth / 10, fontSize / 10, 1);
    sprite.position.set(
      attributeData.Position.X,
      attributeData.Position.Y,
      attributeData.Position.Z
    );

    sprite.userData = attributeData;

    return sprite;
  }

  public static dispose(insertData: InsertData, scene: THREE.Scene): void {
    const objects = this.insertCache.get(insertData.Uuid);
    if (objects) {
      for (const obj of objects) {
        scene.remove(obj);
        this.disposeObject(obj);
      }
      this.insertCache.delete(insertData.Uuid);
    }
  }

  private static disposeObject(object: THREE.Object3D): void {
    if (object instanceof THREE.Mesh || object instanceof THREE.Line) {
      if (object.geometry) {
        object.geometry.dispose();
      }
      if (object.material) {
        if (Array.isArray(object.material)) {
          object.material.forEach(material => material.dispose());
        } else {
          object.material.dispose();
        }
      }
    } else if (object instanceof THREE.Sprite) {
      if (object.material && object.material.map) {
        object.material.map.dispose();
      }
      object.material.dispose();
    }
  }
}
