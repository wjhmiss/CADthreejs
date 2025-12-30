import * as THREE from 'three';

export interface Point3DData {
  X: number;
  Y: number;
  Z: number;
}

export interface BoundsData {
  Min: Point3DData;
  Max: Point3DData;
  Center: Point3DData;
  Size: Point3DData;
}

export interface LineData {
  StartX: number;
  StartY: number;
  EndX: number;
  EndY: number;
  StartZ?: number;
  EndZ?: number;
  NormalX?: number;
  NormalY?: number;
  NormalZ?: number;
  ColorIndex?: number;
  ColorHex?: string;
  ColorR?: number;
  ColorG?: number;
  ColorB?: number;
  LineTypeName?: string;
  LineWeight?: number;
  Bounds?: BoundsData;
  MidPoint?: Point3DData;
  Length?: number;
}

export interface TextData {
  Value: string;
  PositionX: number;
  PositionY: number;
  PositionZ?: number;
  NormalX?: number;
  NormalY?: number;
  NormalZ?: number;
  DirectionX?: number;
  DirectionY?: number;
  DirectionZ?: number;
  ColorIndex?: number;
  ColorHex?: string;
  ColorR?: number;
  ColorG?: number;
  ColorB?: number;
  Style?: string;
  Height?: number;
  Rotation?: number;
  Bounds?: BoundsData;
  Width?: number;
}

export interface MTextData {
  Value: string;
  PositionX: number;
  PositionY: number;
  PositionZ?: number;
  NormalX?: number;
  NormalY?: number;
  NormalZ?: number;
  DirectionX?: number;
  DirectionY?: number;
  DirectionZ?: number;
  ColorIndex?: number;
  ColorHex?: string;
  ColorR?: number;
  ColorG?: number;
  ColorB?: number;
  Style?: string;
  Height?: number;
  Width?: number;
  Rotation?: number;
  LineSpacing?: number;
  Bounds?: BoundsData;
  Lines?: string[];
}

export interface SolidData {
  Points: Point3DData[];
  ColorIndex?: number;
  ColorHex?: string;
  ColorR?: number;
  ColorG?: number;
  ColorB?: number;
  NormalX?: number;
  NormalY?: number;
  NormalZ?: number;
  Bounds?: BoundsData;
  Centroid?: Point3DData;
  Area?: number;
}

export interface DimensionPointData {
  X: number;
  Y: number;
  Z?: number;
  ColorIndex?: number;
  ColorHex?: string;
  ColorR?: number;
  ColorG?: number;
  ColorB?: number;
  PointStyle?: number;
  Size?: number;
  Bounds?: BoundsData;
}

export interface Angular3PtData {
  VertexX: number;
  VertexY: number;
  FirstPointX: number;
  FirstPointY: number;
  SecondPointX: number;
  SecondPointY: number;
  VertexZ?: number;
  FirstPointZ?: number;
  SecondPointZ?: number;
  NormalX?: number;
  NormalY?: number;
  NormalZ?: number;
  Measurement?: number;
  Text?: string;
  AngleInDegrees?: number;
  AngleInRadians?: number;
  ArcCenter?: Point3DData;
  Radius?: number;
  Bounds?: BoundsData;
  ArcPoints?: Point3DData[];
}

export interface Angular2LineData {
  AngleVertexX: number;
  AngleVertexY: number;
  FirstPointX: number;
  FirstPointY: number;
  SecondPointX: number;
  SecondPointY: number;
  DefinitionPointX: number;
  DefinitionPointY: number;
  AngleVertexZ?: number;
  FirstPointZ?: number;
  SecondPointZ?: number;
  DefinitionPointZ?: number;
  NormalX?: number;
  NormalY?: number;
  NormalZ?: number;
  Measurement?: number;
  Text?: string;
  AngleInDegrees?: number;
  AngleInRadians?: number;
  ArcCenter?: Point3DData;
  Radius?: number;
  Bounds?: BoundsData;
  ArcPoints?: Point3DData[];
  FirstLineDirection?: Point3DData;
  SecondLineDirection?: Point3DData;
}

export interface DiameterData {
  AngleVertexX: number;
  AngleVertexY: number;
  DefinitionPointX: number;
  DefinitionPointY: number;
  CenterX: number;
  CenterY: number;
  AngleVertexZ?: number;
  DefinitionPointZ?: number;
  CenterZ?: number;
  NormalX?: number;
  NormalY?: number;
  NormalZ?: number;
  Measurement?: number;
  Text?: string;
  Radius?: number;
  Diameter?: number;
  Bounds?: BoundsData;
  Centroid?: Point3DData;
  CirclePoints?: Point3DData[];
  StartAngle?: number;
  EndAngle?: number;
}

export interface RadiusData {
  DefinitionPointX: number;
  DefinitionPointY: number;
  AngleVertexX: number;
  AngleVertexY: number;
  DefinitionPointZ?: number;
  AngleVertexZ?: number;
  CenterZ?: number;
  NormalX?: number;
  NormalY?: number;
  NormalZ?: number;
  Measurement?: number;
  Text?: string;
  LeaderLength?: number;
  Radius?: number;
  Center?: Point3DData;
  Bounds?: BoundsData;
  Centroid?: Point3DData;
  ArcPoints?: Point3DData[];
  StartAngle?: number;
  EndAngle?: number;
  LeaderDirection?: Point3DData;
}

export interface OrdinateData {
  FeatureLocationX: number;
  FeatureLocationY: number;
  LeaderEndpointX: number;
  LeaderEndpointY: number;
  FeatureLocationZ?: number;
  LeaderEndpointZ?: number;
  NormalX?: number;
  NormalY?: number;
  NormalZ?: number;
  IsOrdinateTypeX?: boolean;
  Measurement?: number;
  Text?: string;
  Bounds?: BoundsData;
  Centroid?: Point3DData;
  FeatureLocation?: Point3DData;
  LeaderEndpoint?: Point3DData;
  LeaderDirection?: Point3DData;
  LeaderLength?: number;
}

export interface AlignedData {
  FirstPointX: number;
  FirstPointY: number;
  SecondPointX: number;
  SecondPointY: number;
  DefinitionPointX: number;
  DefinitionPointY: number;
  FirstPointZ?: number;
  SecondPointZ?: number;
  DefinitionPointZ?: number;
  NormalX?: number;
  NormalY?: number;
  NormalZ?: number;
  Measurement?: number;
  Text?: string;
  ExtLineRotation?: number;
  Bounds?: BoundsData;
  Centroid?: Point3DData;
  FirstPoint?: Point3DData;
  SecondPoint?: Point3DData;
  DefinitionPoint?: Point3DData;
  Length?: number;
  Direction?: Point3DData;
}

export interface LinearData {
  DefinitionPointX: number;
  DefinitionPointY: number;
  FirstPointX: number;
  FirstPointY: number;
  SecondPointX: number;
  SecondPointY: number;
  DefinitionPointZ?: number;
  FirstPointZ?: number;
  SecondPointZ?: number;
  NormalX?: number;
  NormalY?: number;
  NormalZ?: number;
  Measurement?: number;
  Text?: string;
  Rotation?: number;
  ExtLineRotation?: number;
  Bounds?: BoundsData;
  Centroid?: Point3DData;
  DefinitionPoint?: Point3DData;
  FirstPoint?: Point3DData;
  SecondPoint?: Point3DData;
  Length?: number;
  Direction?: Point3DData;
  RotationInRadians?: number;
  ExtLineRotationInRadians?: number;
}

export interface EntityData {
  Type: string;
  LineData?: LineData;
  TextData?: TextData;
  MTextData?: MTextData;
  SolidData?: SolidData;
  PointData?: DimensionPointData;
  Position?: Point3DData;
  Bounds?: BoundsData;
}

export interface DimensionData {
  Type: string;
  DefinitionPointX: number;
  DefinitionPointY: number;
  DefinitionPointZ?: number;
  NormalX?: number;
  NormalY?: number;
  NormalZ?: number;
  ColorIndex?: number;
  ColorHex?: string;
  ColorR?: number;
  ColorG?: number;
  ColorB?: number;
  LineTypeName?: string;
  LineWeight?: number;
  Entities?: EntityData[];
  Angular3PtData?: Angular3PtData;
  Angular2LineData?: Angular2LineData;
  DiameterData?: DiameterData;
  RadiusData?: RadiusData;
  OrdinateData?: OrdinateData;
  AlignedData?: AlignedData;
  LinearData?: LinearData;
  Bounds?: BoundsData;
  Centroid?: Point3DData;
  Points?: Point3DData[];
  CoordinateSystem?: string;
  RequiresYAxisFlip?: boolean;
}

export interface DimensionRenderResult {
  objects: THREE.Object3D[];
  dimensionType: string;
  bounds: THREE.Box3;
}

export class DimensionEntityThreejsRenderer {
  private static readonly DEFAULT_COLOR = new THREE.Color(0xffffff);
  private static readonly DEFAULT_LINE_WIDTH = 1;
  private static readonly DEFAULT_TEXT_HEIGHT = 1;

  public static render(dimensionData: DimensionData, scene?: THREE.Scene): DimensionRenderResult {
    const objects: THREE.Object3D[] = [];

    const color = this.getColor(dimensionData);

    if (dimensionData.Entities && dimensionData.Entities.length > 0) {
      const entityObjects = this.renderEntities(dimensionData.Entities, color);
      objects.push(...entityObjects);
    }

    if (dimensionData.Angular3PtData) {
      const angularObjects = this.renderAngular3Pt(dimensionData.Angular3PtData, color);
      objects.push(...angularObjects);
    }

    if (dimensionData.Angular2LineData) {
      const angular2LineObjects = this.renderAngular2Line(dimensionData.Angular2LineData, color);
      objects.push(...angular2LineObjects);
    }

    if (dimensionData.DiameterData) {
      const diameterObjects = this.renderDiameter(dimensionData.DiameterData, color);
      objects.push(...diameterObjects);
    }

    if (dimensionData.RadiusData) {
      const radiusObjects = this.renderRadius(dimensionData.RadiusData, color);
      objects.push(...radiusObjects);
    }

    if (dimensionData.OrdinateData) {
      const ordinateObjects = this.renderOrdinate(dimensionData.OrdinateData, color);
      objects.push(...ordinateObjects);
    }

    if (dimensionData.AlignedData) {
      const alignedObjects = this.renderAligned(dimensionData.AlignedData, color);
      objects.push(...alignedObjects);
    }

    if (dimensionData.LinearData) {
      const linearObjects = this.renderLinear(dimensionData.LinearData, color);
      objects.push(...linearObjects);
    }

    const bounds = new THREE.Box3();
    for (const obj of objects) {
      bounds.expandByObject(obj);
    }

    return {
      objects,
      dimensionType: dimensionData.Type,
      bounds
    };
  }

  private static renderEntities(entities: EntityData[], color: THREE.Color): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];
    entities.forEach((entity, index) => {
      if (entity.LineData) {
        const lineMesh = this.createLine(entity.LineData, color);
        lineMesh.name = `Line_${index}`;
        objects.push(lineMesh);
      }

      if (entity.TextData) {
        const textMesh = this.createText(entity.TextData, color);
        textMesh.name = `Text_${index}`;
        objects.push(textMesh);
      }

      if (entity.MTextData) {
        const mTextMesh = this.createMText(entity.MTextData, color);
        mTextMesh.name = `MText_${index}`;
        objects.push(mTextMesh);
      }

      if (entity.SolidData) {
        const solidMesh = this.createSolid(entity.SolidData, color);
        solidMesh.name = `Solid_${index}`;
        objects.push(solidMesh);
      }

      if (entity.PointData) {
        const pointMesh = this.createPoint(entity.PointData, color);
        pointMesh.name = `Point_${index}`;
        objects.push(pointMesh);
      }
    });
    return objects;
  }

  private static createLine(lineData: LineData, color: THREE.Color): THREE.Line {
    const startZ = lineData.StartZ ?? 0;
    const endZ = lineData.EndZ ?? 0;

    const points = [
      new THREE.Vector3(lineData.StartX, lineData.StartY, startZ),
      new THREE.Vector3(lineData.EndX, lineData.EndY, endZ)
    ];

    const geometry = new THREE.BufferGeometry().setFromPoints(points);
    const material = new THREE.LineBasicMaterial({
      color: color,
      linewidth: lineData.LineWeight ?? DEFAULT_LINE_WIDTH
    });

    return new THREE.Line(geometry, material);
  }

  private static createText(textData: TextData, color: THREE.Color): THREE.Sprite {
    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d');
    if (!context) {
      throw new Error('Failed to get 2D context');
    }

    const fontSize = (textData.Height ?? DEFAULT_TEXT_HEIGHT) * 64;
    const font = `${fontSize}px Arial`;
    context.font = font;

    const metrics = context.measureText(textData.Value);
    const textWidth = metrics.width;
    const textHeight = fontSize;

    canvas.width = textWidth + 10;
    canvas.height = textHeight + 10;

    context.font = font;
    context.fillStyle = `rgb(${color.r * 255}, ${color.g * 255}, ${color.b * 255})`;
    context.textBaseline = 'middle';
    context.textAlign = 'center';
    context.fillText(textData.Value, canvas.width / 2, canvas.height / 2);

    const texture = new THREE.CanvasTexture(canvas);
    const material = new THREE.SpriteMaterial({ map: texture, transparent: true });

    const sprite = new THREE.Sprite(material);
    const positionZ = textData.PositionZ ?? 0;
    sprite.position.set(textData.PositionX, textData.PositionY, positionZ);

    const scaleX = canvas.width / 100;
    const scaleY = canvas.height / 100;
    sprite.scale.set(scaleX, scaleY, 1);

    if (textData.Rotation !== undefined) {
      sprite.material.rotation = textData.Rotation;
    }

    return sprite;
  }

  private static createMText(mTextData: MTextData, color: THREE.Color): THREE.Sprite {
    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d');
    if (!context) {
      throw new Error('Failed to get 2D context');
    }

    const fontSize = (mTextData.Height ?? DEFAULT_TEXT_HEIGHT) * 64;
    const font = `${fontSize}px Arial`;
    context.font = font;

    const lines = mTextData.Lines ?? mTextData.Value.split('\n');
    const lineHeight = fontSize * (mTextData.LineSpacing ?? 1.2);
    const maxWidth = Math.max(...lines.map(line => context.measureText(line).width));

    const totalHeight = lines.length * lineHeight;

    canvas.width = maxWidth + 20;
    canvas.height = totalHeight + 20;

    context.font = font;
    context.fillStyle = `rgb(${color.r * 255}, ${color.g * 255}, ${color.b * 255})`;
    context.textBaseline = 'top';
    context.textAlign = 'center';

    lines.forEach((line, index) => {
      const y = 10 + index * lineHeight + lineHeight / 2;
      context.fillText(line, canvas.width / 2, y);
    });

    const texture = new THREE.CanvasTexture(canvas);
    const material = new THREE.SpriteMaterial({ map: texture, transparent: true });

    const sprite = new THREE.Sprite(material);
    const positionZ = mTextData.PositionZ ?? 0;
    sprite.position.set(mTextData.PositionX, mTextData.PositionY, positionZ);

    const scaleX = canvas.width / 100;
    const scaleY = canvas.height / 100;
    sprite.scale.set(scaleX, scaleY, 1);

    if (mTextData.Rotation !== undefined) {
      sprite.material.rotation = mTextData.Rotation;
    }

    return sprite;
  }

  private static createSolid(solidData: SolidData, color: THREE.Color): THREE.Mesh {
    if (solidData.Points.length < 3) {
      throw new Error('Solid must have at least 3 points');
    }

    const shape = new THREE.Shape();
    shape.moveTo(solidData.Points[0].X, solidData.Points[0].Y);

    for (let i = 1; i < solidData.Points.length; i++) {
      shape.lineTo(solidData.Points[i].X, solidData.Points[i].Y);
    }

    const geometry = new THREE.ShapeGeometry(shape);
    const material = new THREE.MeshBasicMaterial({
      color: color,
      side: THREE.DoubleSide
    });

    const mesh = new THREE.Mesh(geometry, material);
    mesh.position.z = solidData.Points[0].Z ?? 0;

    return mesh;
  }

  private static createPoint(pointData: DimensionPointData, color: THREE.Color): THREE.Mesh {
    const size = pointData.Size ?? 2;
    const geometry = new THREE.CircleGeometry(size / 2, 16);
    const material = new THREE.MeshBasicMaterial({ color: color });

    const mesh = new THREE.Mesh(geometry, material);
    const positionZ = pointData.Z ?? 0;
    mesh.position.set(pointData.X, pointData.Y, positionZ);

    return mesh;
  }

  private static renderAngular3Pt(angularData: Angular3PtData, color: THREE.Color): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];
    const vertexZ = angularData.VertexZ ?? 0;
    const firstPointZ = angularData.FirstPointZ ?? 0;
    const secondPointZ = angularData.SecondPointZ ?? 0;

    const points = [
      new THREE.Vector3(angularData.VertexX, angularData.VertexY, vertexZ),
      new THREE.Vector3(angularData.FirstPointX, angularData.FirstPointY, firstPointZ),
      new THREE.Vector3(angularData.SecondPointX, angularData.SecondPointY, secondPointZ)
    ];

    const geometry = new THREE.BufferGeometry().setFromPoints(points);
    const material = new THREE.LineBasicMaterial({ color: color });
    const line = new THREE.Line(geometry, material);
    line.name = 'Angular3Pt_Lines';
    objects.push(line);

    if (angularData.ArcPoints && angularData.ArcPoints.length > 0) {
      const arcPoints = angularData.ArcPoints.map(p => new THREE.Vector3(p.X, p.Y, p.Z));
      const arcGeometry = new THREE.BufferGeometry().setFromPoints(arcPoints);
      const arcMaterial = new THREE.LineBasicMaterial({ color: color });
      const arcLine = new THREE.Line(arcGeometry, arcMaterial);
      arcLine.name = 'Angular3Pt_Arc';
      objects.push(arcLine);
    }
    return objects;
  }

  private static renderAngular2Line(angularData: Angular2LineData, color: THREE.Color): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];
    const vertexZ = angularData.AngleVertexZ ?? 0;
    const firstPointZ = angularData.FirstPointZ ?? 0;
    const secondPointZ = angularData.SecondPointZ ?? 0;
    const definitionPointZ = angularData.DefinitionPointZ ?? 0;

    const points = [
      new THREE.Vector3(angularData.AngleVertexX, angularData.AngleVertexY, vertexZ),
      new THREE.Vector3(angularData.FirstPointX, angularData.FirstPointY, firstPointZ),
      new THREE.Vector3(angularData.AngleVertexX, angularData.AngleVertexY, vertexZ),
      new THREE.Vector3(angularData.SecondPointX, angularData.SecondPointY, secondPointZ)
    ];

    const geometry = new THREE.BufferGeometry().setFromPoints(points);
    const material = new THREE.LineBasicMaterial({ color: color });
    const line = new THREE.Line(geometry, material);
    line.name = 'Angular2Line_Lines';
    objects.push(line);

    if (angularData.ArcPoints && angularData.ArcPoints.length > 0) {
      const arcPoints = angularData.ArcPoints.map(p => new THREE.Vector3(p.X, p.Y, p.Z));
      const arcGeometry = new THREE.BufferGeometry().setFromPoints(arcPoints);
      const arcMaterial = new THREE.LineBasicMaterial({ color: color });
      const arcLine = new THREE.Line(arcGeometry, arcMaterial);
      arcLine.name = 'Angular2Line_Arc';
      objects.push(arcLine);
    }
    return objects;
  }

  private static renderDiameter(diameterData: DiameterData, color: THREE.Color): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];
    const centerZ = diameterData.CenterZ ?? 0;
    const definitionPointZ = diameterData.DefinitionPointZ ?? 0;
    const angleVertexZ = diameterData.AngleVertexZ ?? 0;

    const center = new THREE.Vector3(diameterData.CenterX, diameterData.CenterY, centerZ);
    const definitionPoint = new THREE.Vector3(diameterData.DefinitionPointX, diameterData.DefinitionPointY, definitionPointZ);
    const angleVertex = new THREE.Vector3(diameterData.AngleVertexX, diameterData.AngleVertexY, angleVertexZ);

    const points = [center, definitionPoint, angleVertex];
    const geometry = new THREE.BufferGeometry().setFromPoints(points);
    const material = new THREE.LineBasicMaterial({ color: color });
    const line = new THREE.Line(geometry, material);
    line.name = 'Diameter_Lines';
    objects.push(line);

    if (diameterData.CirclePoints && diameterData.CirclePoints.length > 0) {
      const circlePoints = diameterData.CirclePoints.map(p => new THREE.Vector3(p.X, p.Y, p.Z));
      const circleGeometry = new THREE.BufferGeometry().setFromPoints(circlePoints);
      const circleMaterial = new THREE.LineBasicMaterial({ color: color });
      const circleLine = new THREE.Line(circleGeometry, circleMaterial);
      circleLine.name = 'Diameter_Circle';
      objects.push(circleLine);
    }
    return objects;
  }

  private static renderRadius(radiusData: RadiusData, color: THREE.Color): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];
    const centerZ = radiusData.CenterZ ?? 0;
    const definitionPointZ = radiusData.DefinitionPointZ ?? 0;
    const angleVertexZ = radiusData.AngleVertexZ ?? 0;

    const definitionPoint = new THREE.Vector3(radiusData.DefinitionPointX, radiusData.DefinitionPointY, definitionPointZ);
    const angleVertex = new THREE.Vector3(radiusData.AngleVertexX, radiusData.AngleVertexY, angleVertexZ);

    const points = [definitionPoint, angleVertex];
    const geometry = new THREE.BufferGeometry().setFromPoints(points);
    const material = new THREE.LineBasicMaterial({ color: color });
    const line = new THREE.Line(geometry, material);
    line.name = 'Radius_Lines';
    objects.push(line);

    if (radiusData.ArcPoints && radiusData.ArcPoints.length > 0) {
      const arcPoints = radiusData.ArcPoints.map(p => new THREE.Vector3(p.X, p.Y, p.Z));
      const arcGeometry = new THREE.BufferGeometry().setFromPoints(arcPoints);
      const arcMaterial = new THREE.LineBasicMaterial({ color: color });
      const arcLine = new THREE.Line(arcGeometry, arcMaterial);
      arcLine.name = 'Radius_Arc';
      objects.push(arcLine);
    }
    return objects;
  }

  private static renderOrdinate(ordinateData: OrdinateData, color: THREE.Color): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];
    const featureLocationZ = ordinateData.FeatureLocationZ ?? 0;
    const leaderEndpointZ = ordinateData.LeaderEndpointZ ?? 0;

    const points = [
      new THREE.Vector3(ordinateData.FeatureLocationX, ordinateData.FeatureLocationY, featureLocationZ),
      new THREE.Vector3(ordinateData.LeaderEndpointX, ordinateData.LeaderEndpointY, leaderEndpointZ)
    ];

    const geometry = new THREE.BufferGeometry().setFromPoints(points);
    const material = new THREE.LineBasicMaterial({ color: color });
    const line = new THREE.Line(geometry, material);
    line.name = 'Ordinate_Lines';
    objects.push(line);
    return objects;
  }

  private static renderAligned(alignedData: AlignedData, color: THREE.Color): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];
    const firstPointZ = alignedData.FirstPointZ ?? 0;
    const secondPointZ = alignedData.SecondPointZ ?? 0;
    const definitionPointZ = alignedData.DefinitionPointZ ?? 0;

    const points = [
      new THREE.Vector3(alignedData.FirstPointX, alignedData.FirstPointY, firstPointZ),
      new THREE.Vector3(alignedData.SecondPointX, alignedData.SecondPointY, secondPointZ),
      new THREE.Vector3(alignedData.DefinitionPointX, alignedData.DefinitionPointY, definitionPointZ)
    ];

    const geometry = new THREE.BufferGeometry().setFromPoints(points);
    const material = new THREE.LineBasicMaterial({ color: color });
    const line = new THREE.Line(geometry, material);
    line.name = 'Aligned_Lines';
    objects.push(line);
    return objects;
  }

  private static renderLinear(linearData: LinearData, color: THREE.Color): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];
    const definitionPointZ = linearData.DefinitionPointZ ?? 0;
    const firstPointZ = linearData.FirstPointZ ?? 0;
    const secondPointZ = linearData.SecondPointZ ?? 0;

    const points = [
      new THREE.Vector3(linearData.DefinitionPointX, linearData.DefinitionPointY, definitionPointZ),
      new THREE.Vector3(linearData.FirstPointX, linearData.FirstPointY, firstPointZ),
      new THREE.Vector3(linearData.SecondPointX, linearData.SecondPointY, secondPointZ)
    ];

    const geometry = new THREE.BufferGeometry().setFromPoints(points);
    const material = new THREE.LineBasicMaterial({ color: color });
    const line = new THREE.Line(geometry, material);
    line.name = 'Linear_Lines';
    objects.push(line);
    return objects;
  }

  private static getColor(dimensionData: DimensionData): THREE.Color {
    if (dimensionData.ColorHex) {
      return new THREE.Color(dimensionData.ColorHex);
    }
    if (dimensionData.ColorR !== undefined && dimensionData.ColorG !== undefined && dimensionData.ColorB !== undefined) {
      return new THREE.Color(`rgb(${dimensionData.ColorR}, ${dimensionData.ColorG}, ${dimensionData.ColorB})`);
    }
    return DimensionEntityThreejsRenderer.DEFAULT_COLOR;
  }

  public static dispose(result: DimensionRenderResult): void {
    for (const object of result.objects) {
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
      }
      if (object instanceof THREE.Sprite) {
        if (object.material.map) {
          object.material.map.dispose();
        }
        object.material.dispose();
      }
    }
  }
}
