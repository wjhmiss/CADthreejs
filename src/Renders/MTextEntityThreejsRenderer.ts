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
  Center: Point3DData;
  Size: Point3DData;
}

export interface BoundsData3D {
  Min: Point3DData;
  Max: Point3DData;
}

export interface ColorData {
  Index: number;
  Hex: string;
  R: number;
  G: number;
  B: number;
  A: number;
}

export interface TransformData {
  Position: Point3DData;
  Rotation: Point3DData;
  Scale: Point3DData;
  Matrix: number[];
}

export interface NormalData {
  X: number;
  Y: number;
  Z: number;
}

export interface MTextData {
  Type: string;
  EntityType: string;
  Handle: string;
  LayerName: string;
  LayerIndex: number;
  Visible: boolean;
  CoordinateSystem: string;

  Lines: string[];
  InsertPoint: PointData;
  Height: number;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  RectangleWidth: number;
  LineSpacingStyle: number;
  LineSpacing: number;
  AttachmentPoint: number;
  Rotation: number;
  TextStyleName: string;
  WidthFactor: number;
  ObliqueAngle: number;

  Bounds: BoundsData;
  AlignmentPoint: PointData;
  LineCount: number;

  Vertices3D: Point3DData[];
  Centroid3D: Point3DData;
  Normals: Point3DData[];
  Bounds3D: BoundsData3D;
  Color: ColorData;
  Transform: TransformData;
  Normal: NormalData;

  Opacity: number;
  Transparent: boolean;
  MaterialType: string;
  DepthTest: boolean;
  DepthWrite: boolean;

  DrawingDirection: number;
  BackgroundColor: ColorData;
  BackgroundFillFlags: number;
  BackgroundScale: number;
  BackgroundTransparency: number;
  HorizontalWidth: number;
  RectangleHeight: number;
  VerticalHeight: number;
  IsAnnotative: boolean;
  PlainText: string;
  TextValue: string;
  InsertPoint3D: Point3DData;
  AlignmentPoint3D: Point3DData;
  TextWidth: number;
  TextHeight: number;
}

export class MTextEntityThreejsRenderer {
  private static canvas: HTMLCanvasElement | null = null;
  private static context: CanvasRenderingContext2D | null = null;

  public static render(mtextData: MTextData, scene: THREE.Scene): THREE.Object3D[] | null {
    if (!mtextData || !mtextData.Visible) {
      return null;
    }

    const objects: THREE.Object3D[] = [];

    this.renderText(mtextData, objects);
    this.renderBackground(mtextData, objects);
    const boundsLine = this.renderBounds(mtextData);
    if (boundsLine) {
      objects.push(boundsLine);
    }

    const userData = {
      type: mtextData.Type,
      entityType: mtextData.EntityType,
      handle: mtextData.Handle,
      layerName: mtextData.LayerName,
      layerIndex: mtextData.LayerIndex,
      coordinateSystem: mtextData.CoordinateSystem,
      lines: mtextData.Lines,
      insertPoint: mtextData.InsertPoint,
      height: mtextData.Height,
      colorIndex: mtextData.ColorIndex,
      lineTypeName: mtextData.LineTypeName,
      lineWeight: mtextData.LineWeight,
      rectangleWidth: mtextData.RectangleWidth,
      lineSpacingStyle: mtextData.LineSpacingStyle,
      lineSpacing: mtextData.LineSpacing,
      attachmentPoint: mtextData.AttachmentPoint,
      rotation: mtextData.Rotation,
      textStyleName: mtextData.TextStyleName,
      widthFactor: mtextData.WidthFactor,
      obliqueAngle: mtextData.ObliqueAngle,
      bounds: mtextData.Bounds,
      alignmentPoint: mtextData.AlignmentPoint,
      lineCount: mtextData.LineCount,
      vertices3D: mtextData.Vertices3D,
      centroid3D: mtextData.Centroid3D,
      normals: mtextData.Normals,
      bounds3D: mtextData.Bounds3D,
      color: mtextData.Color,
      transform: mtextData.Transform,
      normal: mtextData.Normal,
      opacity: mtextData.Opacity,
      transparent: mtextData.Transparent,
      materialType: mtextData.MaterialType,
      depthTest: mtextData.DepthTest,
      depthWrite: mtextData.DepthWrite,
      drawingDirection: mtextData.DrawingDirection,
      backgroundColor: mtextData.BackgroundColor,
      backgroundFillFlags: mtextData.BackgroundFillFlags,
      backgroundScale: mtextData.BackgroundScale,
      backgroundTransparency: mtextData.BackgroundTransparency,
      horizontalWidth: mtextData.HorizontalWidth,
      rectangleHeight: mtextData.RectangleHeight,
      verticalHeight: mtextData.VerticalHeight,
      isAnnotative: mtextData.IsAnnotative,
      plainText: mtextData.PlainText,
      textValue: mtextData.TextValue,
      insertPoint3D: mtextData.InsertPoint3D,
      alignmentPoint3D: mtextData.AlignmentPoint3D,
      textWidth: mtextData.TextWidth,
      textHeight: mtextData.TextHeight,
      objectType: 'MText'
    };

    objects.forEach(obj => {
      obj.userData = { ...userData, ...obj.userData };
      obj.name = `MText_${mtextData.Handle}_${obj.name}`;
      obj.visible = mtextData.Visible;

      if (mtextData.Transform && mtextData.Transform.Matrix) {
        const matrix = new THREE.Matrix4();
        matrix.fromArray(mtextData.Transform.Matrix);
        obj.applyMatrix4(matrix);
      }
    });

    return objects;
  }

  private static renderText(mtextData: MTextData, objects: THREE.Object3D[]): void {
    if (!mtextData.Lines || mtextData.Lines.length === 0) {
      return;
    }

    const texture = this.createTextTexture(mtextData);
    if (!texture) {
      return;
    }

    const geometry = new THREE.PlaneGeometry(mtextData.TextWidth, mtextData.TextHeight);
    const material = new THREE.MeshBasicMaterial({
      map: texture,
      transparent: true,
      opacity: mtextData.Opacity,
      depthTest: mtextData.DepthTest,
      depthWrite: mtextData.DepthWrite,
      side: THREE.DoubleSide
    });

    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = 'Text';
    mesh.userData = {
      type: 'MTextText',
      lineCount: mtextData.LineCount,
      textValue: mtextData.TextValue
    };

    objects.push(mesh);
  }

  private static createTextTexture(mtextData: MTextData): THREE.CanvasTexture | null {
    if (typeof document === 'undefined') {
      return null;
    }
    
    if (!this.canvas) {
      this.canvas = document.createElement('canvas');
      this.context = this.canvas.getContext('2d');
    }

    if (!this.context) {
      return null;
    }

    const padding = 10;
    const fontSize = Math.max(12, mtextData.Height);
    const lineHeight = fontSize * mtextData.LineSpacing;
    const maxWidth = mtextData.TextWidth;
    const maxHeight = mtextData.TextHeight;

    this.canvas.width = Math.ceil(maxWidth + padding * 2);
    this.canvas.height = Math.ceil(maxHeight + padding * 2);

    const ctx = this.context;
    ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
    ctx.font = `${fontSize}px Arial`;
    ctx.textBaseline = 'top';
    ctx.fillStyle = this.getColorFromIndex(mtextData.ColorIndex);

    let yOffset = padding;
    mtextData.Lines.forEach((line, index) => {
      let xOffset = padding;
      
      switch (mtextData.AttachmentPoint) {
        case 1:
          xOffset = padding;
          break;
        case 2:
          xOffset = padding + (maxWidth - this.measureTextWidth(ctx, line)) / 2;
          break;
        case 3:
          xOffset = padding + maxWidth - this.measureTextWidth(ctx, line);
          break;
        case 4:
          xOffset = padding;
          yOffset = padding + (maxHeight - mtextData.LineCount * lineHeight) / 2;
          break;
        case 5:
          xOffset = padding + (maxWidth - this.measureTextWidth(ctx, line)) / 2;
          yOffset = padding + (maxHeight - mtextData.LineCount * lineHeight) / 2;
          break;
        case 6:
          xOffset = padding + maxWidth - this.measureTextWidth(ctx, line);
          yOffset = padding + (maxHeight - mtextData.LineCount * lineHeight) / 2;
          break;
        case 7:
          xOffset = padding;
          yOffset = padding + maxHeight - mtextData.LineCount * lineHeight;
          break;
        case 8:
          xOffset = padding + (maxWidth - this.measureTextWidth(ctx, line)) / 2;
          yOffset = padding + maxHeight - mtextData.LineCount * lineHeight;
          break;
        case 9:
          xOffset = padding + maxWidth - this.measureTextWidth(ctx, line);
          yOffset = padding + maxHeight - mtextData.LineCount * lineHeight;
          break;
      }

      ctx.fillText(line, xOffset, yOffset + index * lineHeight);
    });

    const texture = new THREE.CanvasTexture(this.canvas);
    texture.needsUpdate = true;
    return texture;
  }

  private static measureTextWidth(ctx: CanvasRenderingContext2D, text: string): number {
    return ctx.measureText(text).width;
  }

  private static renderBackground(mtextData: MTextData, objects: THREE.Object3D[]): void {
    if (mtextData.BackgroundFillFlags === 0 || !mtextData.BackgroundColor) {
      return;
    }

    const geometry = new THREE.PlaneGeometry(mtextData.TextWidth, mtextData.TextHeight);
    const color = this.getColorFromIndex(mtextData.BackgroundColor.Index);
    const material = new THREE.MeshBasicMaterial({
      color: color,
      transparent: mtextData.Transparent,
      opacity: mtextData.Opacity * 0.3,
      side: THREE.DoubleSide,
      depthTest: mtextData.DepthTest,
      depthWrite: mtextData.DepthWrite
    });

    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = 'Background';
    mesh.userData = {
      type: 'MTextBackground',
      backgroundColorIndex: mtextData.BackgroundColor.Index
    };

    objects.push(mesh);
  }

  private static renderBounds(mtextData: MTextData): THREE.Line | null {
    const vertices: number[] = [];
    
    if (mtextData.Vertices3D && mtextData.Vertices3D.length >= 4) {
      mtextData.Vertices3D.forEach(v => {
        vertices.push(v.X, v.Y, v.Z);
      });
    } else {
      const minX = mtextData.Bounds.Min.X;
      const minY = mtextData.Bounds.Min.Y;
      const maxX = mtextData.Bounds.Max.X;
      const maxY = mtextData.Bounds.Max.Y;
      const z = mtextData.InsertPoint3D ? mtextData.InsertPoint3D.Z : 0;
      
      vertices.push(minX, minY, z);
      vertices.push(maxX, minY, z);
      vertices.push(maxX, maxY, z);
      vertices.push(minX, maxY, z);
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));
    geometry.setIndex([0, 1, 2, 0, 2, 3]);
    geometry.computeVertexNormals();

    const color = this.getColorFromIndex(mtextData.ColorIndex);
    const material = new THREE.LineBasicMaterial({
      color: color,
      transparent: mtextData.Transparent,
      opacity: mtextData.Opacity,
      depthTest: mtextData.DepthTest,
      depthWrite: mtextData.DepthWrite
    });

    const line = new THREE.Line(geometry, material);
    line.name = 'Bounds';
    line.visible = mtextData.Visible;

    return line;
  }

  public static dispose(mtextData: MTextData, scene: THREE.Scene): boolean {
    if (!mtextData || !scene) {
      return false;
    }
    const objects = this.getMTextObjects(mtextData, scene);
    if (objects.length === 0) {
      return false;
    }
    objects.forEach(obj => {
      scene.remove(obj);
      if (obj instanceof THREE.Mesh || obj instanceof THREE.Line) {
        if (obj.geometry) {
          obj.geometry.dispose();
        }
        if (obj.material) {
          if (obj.material instanceof THREE.MeshBasicMaterial) {
            if (obj.material.map) {
              obj.material.map.dispose();
            }
            obj.material.dispose();
          } else if (obj.material instanceof THREE.LineBasicMaterial) {
            obj.material.dispose();
          }
        }
      }
    });
    return true;
  }

  public static update(mtextData: MTextData, scene: THREE.Scene): boolean {
    if (!mtextData || !scene) {
      return false;
    }
    this.dispose(mtextData, scene);
    const result = this.render(mtextData, scene);
    return result !== null;
  }

  public static getMTextObjects(mtextData: MTextData, scene: THREE.Scene): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];
    scene.traverse((object) => {
      if (object.userData && object.userData.handle === mtextData.Handle) {
        objects.push(object);
      }
    });
    return objects;
  }

  public static setVisibility(mtextData: MTextData, scene: THREE.Scene, visible: boolean): boolean {
    const objects = this.getMTextObjects(mtextData, scene);
    objects.forEach(obj => {
      obj.visible = visible;
    });
    return objects.length > 0;
  }

  public static setColor(mtextData: MTextData, scene: THREE.Scene, color: string): boolean {
    const objects = this.getMTextObjects(mtextData, scene);
    objects.forEach(obj => {
      if (obj instanceof THREE.Mesh && obj.name === 'Text') {
        if (obj.material instanceof THREE.MeshBasicMaterial) {
          obj.material.color.set(color);
        }
      }
    });
    return objects.length > 0;
  }

  public static setOpacity(mtextData: MTextData, scene: THREE.Scene, opacity: number): boolean {
    const objects = this.getMTextObjects(mtextData, scene);
    objects.forEach(obj => {
      if (obj instanceof THREE.Mesh || obj instanceof THREE.Line) {
        if (obj.material instanceof THREE.MeshBasicMaterial || 
            obj.material instanceof THREE.LineBasicMaterial) {
          obj.material.opacity = opacity;
          obj.material.transparent = opacity < 1.0;
        }
      }
    });
    return objects.length > 0;
  }

  public static getLineCount(mtextData: MTextData): number {
    return mtextData.LineCount || 0;
  }

  public static getTextWidth(mtextData: MTextData): number {
    return mtextData.TextWidth || 0;
  }

  public static getTextHeight(mtextData: MTextData): number {
    return mtextData.TextHeight || 0;
  }

  public static getCentroid(mtextData: MTextData): Point3DData {
    return mtextData.Centroid3D || { X: 0, Y: 0, Z: 0 };
  }

  public static getBounds(mtextData: MTextData): BoundsData {
    return mtextData.Bounds || {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 0, Y: 0, Z: 0 },
      Center: { X: 0, Y: 0, Z: 0 },
      Size: { X: 0, Y: 0, Z: 0 }
    };
  }

  public static isPointInText(mtextData: MTextData, point: Point3DData, tolerance: number = 0.001): boolean {
    if (!mtextData.Bounds) {
      return false;
    }

    const min = mtextData.Bounds.Min;
    const max = mtextData.Bounds.Max;

    return point.X >= min.X - tolerance &&
           point.X <= max.X + tolerance &&
           point.Y >= min.Y - tolerance &&
           point.Y <= max.Y + tolerance;
  }

  public static getClosestPoint(mtextData: MTextData, point: Point3DData): Point3DData | null {
    if (!mtextData.Bounds) {
      return null;
    }

    const min = mtextData.Bounds.Min;
    const max = mtextData.Bounds.Max;

    const closestX = Math.max(min.X, Math.min(max.X, point.X));
    const closestY = Math.max(min.Y, Math.min(max.Y, point.Y));
    const closestZ = mtextData.InsertPoint3D ? mtextData.InsertPoint3D.Z : 0;

    return { X: closestX, Y: closestY, Z: closestZ };
  }

  public static getDistanceToPoint(mtextData: MTextData, point: Point3DData): number {
    const closestPoint = this.getClosestPoint(mtextData, point);
    if (!closestPoint) {
      return Infinity;
    }

    const testPoint = new THREE.Vector3(point.X, point.Y, point.Z);
    const closest = new THREE.Vector3(closestPoint.X, closestPoint.Y, closestPoint.Z);

    return testPoint.distanceTo(closest);
  }

  public static getBackgroundColor(mtextData: MTextData): ColorData {
    return mtextData.BackgroundColor || { Index: 0, Hex: '#000000', R: 0, G: 0, B: 0, A: 1.0 };
  }

  public static getBackgroundScale(mtextData: MTextData): number {
    return mtextData.BackgroundScale || 1.0;
  }

  public static getBackgroundTransparency(mtextData: MTextData): number {
    return mtextData.BackgroundTransparency || 0.0;
  }

  public static getIsAnnotative(mtextData: MTextData): boolean {
    return mtextData.IsAnnotative || false;
  }

  public static getInsertPoint3D(mtextData: MTextData): Point3DData {
    return mtextData.InsertPoint3D || { X: 0, Y: 0, Z: 0 };
  }

  public static getAlignmentPoint3D(mtextData: MTextData): Point3DData {
    return mtextData.AlignmentPoint3D || { X: 0, Y: 0, Z: 0 };
  }

  public static getLineSpacingStyle(mtextData: MTextData): number {
    return mtextData.LineSpacingStyle || 1;
  }

  public static getTextStyleName(mtextData: MTextData): string {
    return mtextData.TextStyleName || 'Standard';
  }

  public static getWidthFactor(mtextData: MTextData): number {
    return mtextData.WidthFactor || 1.0;
  }

  public static getObliqueAngle(mtextData: MTextData): number {
    return mtextData.ObliqueAngle || 0;
  }

  public static getDrawingDirection(mtextData: MTextData): number {
    return mtextData.DrawingDirection || 1;
  }

  public static getBackgroundFillFlags(mtextData: MTextData): number {
    return mtextData.BackgroundFillFlags || 0;
  }

  public static getAlignmentPoint(mtextData: MTextData): PointData {
    return mtextData.AlignmentPoint || { X: 0, Y: 0 };
  }

  public static getRotation(mtextData: MTextData): number {
    return mtextData.Rotation || 0;
  }

  public static getHeight(mtextData: MTextData): number {
    return mtextData.Height || 10;
  }

  public static getAttachmentPoint(mtextData: MTextData): number {
    return mtextData.AttachmentPoint || 1;
  }

  public static getLineSpacing(mtextData: MTextData): number {
    return mtextData.LineSpacing || 1.0;
  }

  public static getTextValue(mtextData: MTextData): string {
    return mtextData.TextValue || '';
  }

  public static getPlainText(mtextData: MTextData): string {
    return mtextData.PlainText || '';
  }

  public static getInsertPoint(mtextData: MTextData): PointData {
    return mtextData.InsertPoint || { X: 0, Y: 0 };
  }

  public static clone(mtextData: MTextData, newHandle: string): MTextData {
    const cloned = JSON.parse(JSON.stringify(mtextData));
    cloned.Handle = newHandle;
    return cloned;
  }

  public static transform(mtextData: MTextData, matrix: THREE.Matrix4): MTextData {
    const transformed = JSON.parse(JSON.stringify(mtextData));
    
    if (transformed.InsertPoint) {
      const insertPoint = new THREE.Vector3(
        transformed.InsertPoint.X,
        transformed.InsertPoint.Y,
        transformed.InsertPoint3D?.Z || 0
      );
      insertPoint.applyMatrix4(matrix);
      transformed.InsertPoint.X = insertPoint.x;
      transformed.InsertPoint.Y = insertPoint.y;
      if (transformed.InsertPoint3D) {
        transformed.InsertPoint3D.Z = insertPoint.z;
      }
    }

    if (transformed.Bounds) {
      const min = new THREE.Vector3(
        transformed.Bounds.Min.X,
        transformed.Bounds.Min.Y,
        transformed.Bounds.Min.Z
      );
      const max = new THREE.Vector3(
        transformed.Bounds.Max.X,
        transformed.Bounds.Max.Y,
        transformed.Bounds.Max.Z
      );
      min.applyMatrix4(matrix);
      max.applyMatrix4(matrix);
      transformed.Bounds.Min.X = min.x;
      transformed.Bounds.Min.Y = min.y;
      transformed.Bounds.Min.Z = min.z;
      transformed.Bounds.Max.X = max.x;
      transformed.Bounds.Max.Y = max.y;
      transformed.Bounds.Max.Z = max.z;
    }

    return transformed;
  }

  private static getColorFromIndex(colorIndex: number): string {
    const colorMap: { [key: number]: string } = {
      0: '#000000',
      1: '#ff0000',
      2: '#ffff00',
      3: '#00ff00',
      4: '#00ffff',
      5: '#0000ff',
      6: '#ff00ff',
      7: '#ffffff',
      8: '#808080',
      9: '#c0c0c0'
    };
    return colorMap[colorIndex] || '#ffffff';
  }
}
