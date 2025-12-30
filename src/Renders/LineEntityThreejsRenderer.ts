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

export interface LineData {
  Type: string;
  EntityType: string;
  Handle: string;
  LayerName: string;
  LayerIndex: number;
  Visible: boolean;
  CoordinateSystem: string;
  
  StartPointX: number;
  StartPointY: number;
  EndPointX: number;
  EndPointY: number;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  LineTypeScale: number;
  
  Length: number;
  Angle: number;
  MidPointX: number;
  MidPointY: number;
  
  StartPoint3D: Point3DData;
  EndPoint3D: Point3DData;
  MidPoint3D: Point3DData;
  Direction: Point3DData;
  Bounds: BoundsData;
  Color: ColorData;
  Transform: TransformData;
  Normal: NormalData;
  
  Opacity: number;
  Transparent: boolean;
  MaterialType: string;
  DepthTest: boolean;
  DepthWrite: boolean;
  
  Thickness: number;
}

export class LineEntityThreejsRenderer {
  public static render(lineData: LineData, scene: THREE.Scene): THREE.Line | null {
    if (!lineData || !lineData.Visible) {
      return null;
    }

    if (!lineData.StartPoint3D || !lineData.EndPoint3D) {
      return null;
    }

    const vertices: number[] = [
      lineData.StartPoint3D.X, lineData.StartPoint3D.Y, lineData.StartPoint3D.Z,
      lineData.EndPoint3D.X, lineData.EndPoint3D.Y, lineData.EndPoint3D.Z
    ];

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));

    const material = new THREE.LineBasicMaterial({
      color: lineData.Color?.Hex || '#000000',
      transparent: lineData.Transparent || false,
      opacity: lineData.Opacity || 1.0,
      depthTest: lineData.DepthTest !== false,
      depthWrite: lineData.DepthWrite !== false,
      linewidth: lineData.LineWeight || 1.0
    });

    const line = new THREE.Line(geometry, material);
    line.name = `Line_${lineData.Handle}`;
    line.visible = lineData.Visible;
    line.userData = {
      type: lineData.Type,
      entityType: lineData.EntityType,
      handle: lineData.Handle,
      layerName: lineData.LayerName,
      layerIndex: lineData.LayerIndex,
      coordinateSystem: lineData.CoordinateSystem,
      startPointX: lineData.StartPointX,
      startPointY: lineData.StartPointY,
      endPointX: lineData.EndPointX,
      endPointY: lineData.EndPointY,
      colorIndex: lineData.ColorIndex,
      lineTypeName: lineData.LineTypeName,
      lineWeight: lineData.LineWeight,
      lineTypeScale: lineData.LineTypeScale,
      length: lineData.Length,
      angle: lineData.Angle,
      midPointX: lineData.MidPointX,
      midPointY: lineData.MidPointY,
      startPoint3D: lineData.StartPoint3D,
      endPoint3D: lineData.EndPoint3D,
      midPoint3D: lineData.MidPoint3D,
      direction: lineData.Direction,
      bounds: lineData.Bounds,
      color: lineData.Color,
      transform: lineData.Transform,
      normal: lineData.Normal,
      opacity: lineData.Opacity,
      transparent: lineData.Transparent,
      materialType: lineData.MaterialType,
      depthTest: lineData.DepthTest,
      depthWrite: lineData.DepthWrite,
      thickness: lineData.Thickness
    };

    if (lineData.Transform && lineData.Transform.Matrix) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(lineData.Transform.Matrix);
      line.applyMatrix4(matrix);
    }

    return line;
  }

  public static dispose(lineData: LineData, scene: THREE.Scene): boolean {
    if (!lineData || !scene) {
      return false;
    }
    const line = scene.getObjectByName(`Line_${lineData.Handle}`) as THREE.Line;
    if (line) {
      scene.remove(line);
      if (line.geometry) {
        line.geometry.dispose();
      }
      if (line.material) {
        if (Array.isArray(line.material)) {
          line.material.forEach(material => material.dispose());
        } else {
          line.material.dispose();
        }
      }
      return true;
    }
    return false;
  }

  public static update(lineData: LineData, scene: THREE.Scene): boolean {
    const line = this.getLine(lineData, scene);
    if (!line) {
      return false;
    }
    this.dispose(lineData, scene);
    const result = this.render(lineData, scene);
    return result !== null;
  }

  public static getLine(lineData: LineData, scene: THREE.Scene): THREE.Line | null {
    return scene.getObjectByName(`Line_${lineData.Handle}`) as THREE.Line || null;
  }

  public static setVisibility(lineData: LineData, scene: THREE.Scene, visible: boolean): boolean {
    const line = this.getLine(lineData, scene);
    if (line) {
      line.visible = visible;
      return true;
    }
    return false;
  }

  public static setColor(lineData: LineData, scene: THREE.Scene, color: string): boolean {
    const line = this.getLine(lineData, scene);
    if (line) {
      if (line.material instanceof THREE.LineBasicMaterial) {
        line.material.color.set(color);
      }
      return true;
    }
    return false;
  }

  public static setOpacity(lineData: LineData, scene: THREE.Scene, opacity: number): boolean {
    const line = this.getLine(lineData, scene);
    if (line) {
      if (line.material instanceof THREE.LineBasicMaterial) {
        line.material.opacity = opacity;
        line.material.transparent = opacity < 1.0;
      }
      return true;
    }
    return false;
  }

  public static setLineWidth(lineData: LineData, scene: THREE.Scene, width: number): boolean {
    const line = this.getLine(lineData, scene);
    if (line) {
      if (line.material instanceof THREE.LineBasicMaterial) {
        line.material.linewidth = width;
      }
      return true;
    }
    return false;
  }

  public static calculateLength(lineData: LineData): number {
    if (!lineData.StartPoint3D || !lineData.EndPoint3D) {
      return 0;
    }

    const dx = lineData.EndPoint3D.X - lineData.StartPoint3D.X;
    const dy = lineData.EndPoint3D.Y - lineData.StartPoint3D.Y;
    const dz = lineData.EndPoint3D.Z - lineData.StartPoint3D.Z;

    return Math.sqrt(dx * dx + dy * dy + dz * dz);
  }

  public static calculateAngle(lineData: LineData): number {
    if (!lineData.StartPoint3D || !lineData.EndPoint3D) {
      return 0;
    }

    const dx = lineData.EndPoint3D.X - lineData.StartPoint3D.X;
    const dy = lineData.EndPoint3D.Y - lineData.StartPoint3D.Y;

    return Math.atan2(dy, dx);
  }

  public static getLength(lineData: LineData): number {
    return lineData.Length || 0;
  }

  public static getAngle(lineData: LineData): number {
    return lineData.Angle || 0;
  }

  public static getMidPoint(lineData: LineData): Point3DData {
    return lineData.MidPoint3D || { X: 0, Y: 0, Z: 0 };
  }

  public static getDirection(lineData: LineData): Point3DData {
    return lineData.Direction || { X: 0, Y: 0, Z: 0 };
  }

  public static getBounds(lineData: LineData): BoundsData {
    return lineData.Bounds || {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 0, Y: 0, Z: 0 },
      Center: { X: 0, Y: 0, Z: 0 },
      Size: { X: 0, Y: 0, Z: 0 }
    };
  }

  public static isPointOnLine(lineData: LineData, point: Point3DData, tolerance: number = 0.001): boolean {
    if (!lineData.StartPoint3D || !lineData.EndPoint3D) {
      return false;
    }

    const start = new THREE.Vector3(lineData.StartPoint3D.X, lineData.StartPoint3D.Y, lineData.StartPoint3D.Z);
    const end = new THREE.Vector3(lineData.EndPoint3D.X, lineData.EndPoint3D.Y, lineData.EndPoint3D.Z);
    const testPoint = new THREE.Vector3(point.X, point.Y, point.Z);

    const lineLength = start.distanceTo(end);
    const distToStart = start.distanceTo(testPoint);
    const distToEnd = end.distanceTo(testPoint);

    return Math.abs(distToStart + distToEnd - lineLength) < tolerance;
  }

  public static getPointAtParameter(lineData: LineData, parameter: number): Point3DData | null {
    if (parameter < 0 || parameter > 1) {
      return null;
    }

    if (!lineData.StartPoint3D || !lineData.EndPoint3D) {
      return null;
    }

    const x = lineData.StartPoint3D.X + parameter * (lineData.EndPoint3D.X - lineData.StartPoint3D.X);
    const y = lineData.StartPoint3D.Y + parameter * (lineData.EndPoint3D.Y - lineData.StartPoint3D.Y);
    const z = lineData.StartPoint3D.Z + parameter * (lineData.EndPoint3D.Z - lineData.StartPoint3D.Z);

    return { X: x, Y: y, Z: z };
  }

  public static getClosestPoint(lineData: LineData, point: Point3DData): Point3DData | null {
    if (!lineData.StartPoint3D || !lineData.EndPoint3D) {
      return null;
    }

    const start = new THREE.Vector3(lineData.StartPoint3D.X, lineData.StartPoint3D.Y, lineData.StartPoint3D.Z);
    const end = new THREE.Vector3(lineData.EndPoint3D.X, lineData.EndPoint3D.Y, lineData.EndPoint3D.Z);
    const testPoint = new THREE.Vector3(point.X, point.Y, point.Z);

    const lineVec = new THREE.Vector3().subVectors(end, start);
    const pointVec = new THREE.Vector3().subVectors(testPoint, start);

    const lineLength = lineVec.length();
    if (lineLength === 0) {
      return { X: start.x, Y: start.y, Z: start.z };
    }

    const t = Math.max(0, Math.min(1, pointVec.dot(lineVec) / (lineLength * lineLength)));
    const closestPoint = start.clone().add(lineVec.multiplyScalar(t));

    return { X: closestPoint.x, Y: closestPoint.y, Z: closestPoint.z };
  }

  public static getDistanceToPoint(lineData: LineData, point: Point3DData): number {
    const closestPoint = this.getClosestPoint(lineData, point);
    if (!closestPoint) {
      return Infinity;
    }

    const testPoint = new THREE.Vector3(point.X, point.Y, point.Z);
    const closest = new THREE.Vector3(closestPoint.X, closestPoint.Y, closestPoint.Z);

    return testPoint.distanceTo(closest);
  }

  public static intersectLine(lineData1: LineData, lineData2: LineData): Point3DData | null {
    if (!lineData1.StartPoint3D || !lineData1.EndPoint3D || 
        !lineData2.StartPoint3D || !lineData2.EndPoint3D) {
      return null;
    }

    const p1 = new THREE.Vector3(lineData1.StartPoint3D.X, lineData1.StartPoint3D.Y, lineData1.StartPoint3D.Z);
    const p2 = new THREE.Vector3(lineData1.EndPoint3D.X, lineData1.EndPoint3D.Y, lineData1.EndPoint3D.Z);
    const p3 = new THREE.Vector3(lineData2.StartPoint3D.X, lineData2.StartPoint3D.Y, lineData2.StartPoint3D.Z);
    const p4 = new THREE.Vector3(lineData2.EndPoint3D.X, lineData2.EndPoint3D.Y, lineData2.EndPoint3D.Z);

    const denom = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);

    if (denom === 0) {
      return null;
    }

    const ua = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denom;
    const ub = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denom;

    if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1) {
      const x = p1.x + ua * (p2.x - p1.x);
      const y = p1.y + ua * (p2.y - p1.y);
      const z = p1.z + ua * (p2.z - p1.z);
      return { X: x, Y: y, Z: z };
    }

    return null;
  }

  public static clone(lineData: LineData, newHandle: string): LineData {
    const cloned = JSON.parse(JSON.stringify(lineData));
    cloned.Handle = newHandle;
    return cloned;
  }

  public static transform(lineData: LineData, matrix: THREE.Matrix4): LineData {
    const transformed = JSON.parse(JSON.stringify(lineData));

    const startPoint = new THREE.Vector3(lineData.StartPoint3D.X, lineData.StartPoint3D.Y, lineData.StartPoint3D.Z);
    const endPoint = new THREE.Vector3(lineData.EndPoint3D.X, lineData.EndPoint3D.Y, lineData.EndPoint3D.Z);
    const midPoint = new THREE.Vector3(lineData.MidPoint3D.X, lineData.MidPoint3D.Y, lineData.MidPoint3D.Z);

    startPoint.applyMatrix4(matrix);
    endPoint.applyMatrix4(matrix);
    midPoint.applyMatrix4(matrix);

    transformed.StartPoint3D = { X: startPoint.x, Y: startPoint.y, Z: startPoint.z };
    transformed.EndPoint3D = { X: endPoint.x, Y: endPoint.y, Z: endPoint.z };
    transformed.MidPoint3D = { X: midPoint.x, Y: midPoint.y, Z: midPoint.z };

    transformed.StartPointX = startPoint.x;
    transformed.StartPointY = startPoint.y;
    transformed.EndPointX = endPoint.x;
    transformed.EndPointY = endPoint.y;
    transformed.MidPointX = midPoint.x;
    transformed.MidPointY = midPoint.y;

    const dx = endPoint.x - startPoint.x;
    const dy = endPoint.y - startPoint.y;
    transformed.Length = Math.sqrt(dx * dx + dy * dy);
    transformed.Angle = Math.atan2(dy, dx);

    return transformed;
  }
}
