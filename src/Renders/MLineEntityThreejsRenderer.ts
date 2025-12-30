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

export interface ElementData {
  Offset: number;
  ColorIndex: number;
  LineTypeName: string;
  Points: PointData[];
  PointCount: number;
  TotalLength: number;
}

export interface VertexData {
  Position: PointData;
  Direction: PointData;
  Miter: PointData;
  Segments: SegmentData[];
}

export interface SegmentData {
  Parameters: number[];
}

export interface MLineData {
  Type: string;
  EntityType: string;
  Handle: string;
  LayerName: string;
  LayerIndex: number;
  Visible: boolean;
  CoordinateSystem: string;

  Elements: ElementData[];
  IsClosed: boolean;
  ScaleFactor: number;
  ColorIndex: number;
  StyleFlags: number;
  FillColorIndex: number;
  FillOn: boolean;
  Vertices: VertexData[];

  ElementCount: number;
  VertexCount: number;
  Bounds: BoundsData;
  Centroid: PointData;

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

  Flags: number;
  Justification: number;
  StartPoint: Point3DData;
  EndPoint: Point3DData;
  TotalLength: number;
  StyleName: string;
  StartAngle: number;
  EndAngle: number;
  HasStartCaps: boolean;
  HasEndCaps: boolean;
  DisplayJoints: boolean;
  LineIndices: number[][];
  Offsets: number[];
  ElementColorIndices: number[];
}

export class MLineEntityThreejsRenderer {
  public static render(mlineData: MLineData, scene: THREE.Scene): THREE.Object3D[] | null {
    if (!mlineData || !mlineData.Visible) {
      return null;
    }

    const objects: THREE.Object3D[] = [];

    const elements = this.renderElements(mlineData);
    elements.forEach((element, index) => {
      element.userData = {
        type: mlineData.Type,
        entityType: mlineData.EntityType,
        handle: mlineData.Handle,
        layerName: mlineData.LayerName,
        layerIndex: mlineData.LayerIndex,
        coordinateSystem: mlineData.CoordinateSystem,
        isClosed: mlineData.IsClosed,
        scaleFactor: mlineData.ScaleFactor,
        colorIndex: mlineData.ColorIndex,
        styleFlags: mlineData.StyleFlags,
        fillColorIndex: mlineData.FillColorIndex,
        fillOn: mlineData.FillOn,
        elementCount: mlineData.ElementCount,
        vertexCount: mlineData.VertexCount,
        bounds: mlineData.Bounds,
        centroid: mlineData.Centroid,
        vertices3D: mlineData.Vertices3D,
        centroid3D: mlineData.Centroid3D,
        normals: mlineData.Normals,
        bounds3D: mlineData.Bounds3D,
        color: mlineData.Color,
        transform: mlineData.Transform,
        normal: mlineData.Normal,
        opacity: mlineData.Opacity,
        transparent: mlineData.Transparent,
        materialType: mlineData.MaterialType,
        depthTest: mlineData.DepthTest,
        depthWrite: mlineData.DepthWrite,
        flags: mlineData.Flags,
        justification: mlineData.Justification,
        startPoint: mlineData.StartPoint,
        endPoint: mlineData.EndPoint,
        totalLength: mlineData.TotalLength,
        styleName: mlineData.StyleName,
        startAngle: mlineData.StartAngle,
        endAngle: mlineData.EndAngle,
        hasStartCaps: mlineData.HasStartCaps,
        hasEndCaps: mlineData.HasEndCaps,
        displayJoints: mlineData.DisplayJoints,
        lineIndices: mlineData.LineIndices,
        offsets: mlineData.Offsets,
        elementColorIndices: mlineData.ElementColorIndices,
        objectType: 'MLineElement',
        elementIndex: index,
        offset: mlineData.Elements[index]?.Offset,
        elementColorIndex: mlineData.Elements[index]?.ColorIndex,
        lineTypeName: mlineData.Elements[index]?.LineTypeName,
        pointCount: mlineData.Elements[index]?.PointCount,
        elementTotalLength: mlineData.Elements[index]?.TotalLength
      };
      objects.push(element);
    });

    const fill = this.renderFill(mlineData);
    if (fill) {
      fill.userData = {
        type: mlineData.Type,
        entityType: mlineData.EntityType,
        handle: mlineData.Handle,
        layerName: mlineData.LayerName,
        layerIndex: mlineData.LayerIndex,
        objectType: 'MLineFill',
        fillColorIndex: mlineData.FillColorIndex
      };
      objects.push(fill);
    }

    const startCap = this.renderStartCap(mlineData);
    if (startCap) {
      startCap.userData = {
        type: mlineData.Type,
        entityType: mlineData.EntityType,
        handle: mlineData.Handle,
        layerName: mlineData.LayerName,
        layerIndex: mlineData.LayerIndex,
        objectType: 'MLineCap',
        capType: 'Start'
      };
      objects.push(startCap);
    }

    const endCap = this.renderEndCap(mlineData);
    if (endCap) {
      endCap.userData = {
        type: mlineData.Type,
        entityType: mlineData.EntityType,
        handle: mlineData.Handle,
        layerName: mlineData.LayerName,
        layerIndex: mlineData.LayerIndex,
        objectType: 'MLineCap',
        capType: 'End'
      };
      objects.push(endCap);
    }

    const joints = this.renderJoints(mlineData);
    joints.forEach(joint => {
      objects.push(joint);
    });

    if (mlineData.Transform && mlineData.Transform.Matrix) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(mlineData.Transform.Matrix);
      objects.forEach(obj => obj.applyMatrix4(matrix));
    }

    return objects.length > 0 ? objects : null;
  }

  private static renderElements(mlineData: MLineData): THREE.Line[] {
    const lines: THREE.Line[] = [];
    if (!mlineData.Elements || mlineData.Elements.length === 0) {
      return lines;
    }

    mlineData.Elements.forEach((element, index) => {
      if (!element.Points || element.Points.length < 2) {
        return;
      }

      const vertices: number[] = [];
      element.Points.forEach(point => {
        vertices.push(point.X, point.Y, 0);
      });

      const geometry = new THREE.BufferGeometry();
      geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));

      const color = this.getColorFromIndex(element.ColorIndex);
      const material = new THREE.LineBasicMaterial({
        color: color,
        transparent: mlineData.Transparent || false,
        opacity: mlineData.Opacity || 1.0,
        depthTest: mlineData.DepthTest !== false,
        depthWrite: mlineData.DepthWrite !== false,
        linewidth: 1.0
      });

      const line = new THREE.Line(geometry, material);
      line.name = `Element_${index}`;
      line.visible = mlineData.Visible;
      lines.push(line);
    });

    return lines;
  }

  private static renderFill(mlineData: MLineData): THREE.Mesh | null {
    if (!mlineData.FillOn || !mlineData.Elements || mlineData.Elements.length < 2) {
      return null;
    }

    const firstElement = mlineData.Elements[0];
    const lastElement = mlineData.Elements[mlineData.Elements.length - 1];

    if (!firstElement.Points || !lastElement.Points || 
        firstElement.Points.length !== lastElement.Points.length) {
      return null;
    }

    const vertices: number[] = [];
    const pointCount = firstElement.Points.length;

    for (let i = 0; i < pointCount; i++) {
      vertices.push(firstElement.Points[i].X, firstElement.Points[i].Y, 0);
    }

    for (let i = pointCount - 1; i >= 0; i--) {
      vertices.push(lastElement.Points[i].X, lastElement.Points[i].Y, 0);
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));
    geometry.computeVertexNormals();

    const fillColor = this.getColorFromIndex(mlineData.FillColorIndex);
    const material = new THREE.MeshBasicMaterial({
      color: fillColor,
      transparent: mlineData.Transparent || false,
      opacity: (mlineData.Opacity || 1.0) * 0.5,
      side: THREE.DoubleSide,
      depthTest: mlineData.DepthTest !== false,
      depthWrite: mlineData.DepthWrite !== false
    });

    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = 'Fill';
    mesh.visible = mlineData.Visible;
    mesh.userData = {
      type: 'MLineFill',
      fillColorIndex: mlineData.FillColorIndex
    };

    return mesh;
  }

  private static renderStartCap(mlineData: MLineData): THREE.Mesh | null {
    if (!mlineData.Elements || mlineData.Elements.length === 0) {
      return null;
    }

    const firstElement = mlineData.Elements[0];
    const lastElement = mlineData.Elements[mlineData.Elements.length - 1];

    if (!firstElement.Points || !lastElement.Points || 
        firstElement.Points.length === 0 || lastElement.Points.length === 0) {
      return null;
    }

    const startPoint1 = firstElement.Points[0];
    const startPoint2 = lastElement.Points[0];

    const vertices = new Float32Array([
      startPoint1.X, startPoint1.Y, 0,
      startPoint2.X, startPoint2.Y, 0,
      startPoint1.X, startPoint1.Y, 0.1,
      startPoint1.X, startPoint1.Y, 0,
      startPoint2.X, startPoint2.Y, 0,
      startPoint2.X, startPoint2.Y, 0.1
    ]);

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));

    const color = this.getColorFromIndex(mlineData.ColorIndex);
    const material = new THREE.MeshBasicMaterial({
      color: color,
      transparent: mlineData.Transparent || false,
      opacity: mlineData.Opacity || 1.0,
      side: THREE.DoubleSide
    });

    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = 'StartCap';
    mesh.visible = mlineData.Visible;
    mesh.userData = {
      type: 'MLineCap',
      capType: 'Start'
    };

    return mesh;
  }

  private static renderEndCap(mlineData: MLineData): THREE.Mesh | null {
    if (!mlineData.Elements || mlineData.Elements.length === 0) {
      return null;
    }

    const firstElement = mlineData.Elements[0];
    const lastElement = mlineData.Elements[mlineData.Elements.length - 1];

    if (!firstElement.Points || !lastElement.Points || 
        firstElement.Points.length === 0 || lastElement.Points.length === 0) {
      return null;
    }

    const endPoint1 = firstElement.Points[firstElement.Points.length - 1];
    const endPoint2 = lastElement.Points[lastElement.Points.length - 1];

    const vertices = new Float32Array([
      endPoint1.X, endPoint1.Y, 0,
      endPoint2.X, endPoint2.Y, 0,
      endPoint1.X, endPoint1.Y, 0.1,
      endPoint1.X, endPoint1.Y, 0,
      endPoint2.X, endPoint2.Y, 0,
      endPoint2.X, endPoint2.Y, 0.1
    ]);

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));

    const color = this.getColorFromIndex(mlineData.ColorIndex);
    const material = new THREE.MeshBasicMaterial({
      color: color,
      transparent: mlineData.Transparent || false,
      opacity: mlineData.Opacity || 1.0,
      side: THREE.DoubleSide
    });

    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = 'EndCap';
    mesh.visible = mlineData.Visible;
    mesh.userData = {
      type: 'MLineCap',
      capType: 'End'
    };

    return mesh;
  }

  private static renderJoints(mlineData: MLineData): THREE.Mesh[] {
    const joints: THREE.Mesh[] = [];
    if (!mlineData.DisplayJoints || !mlineData.Elements || mlineData.Elements.length === 0) {
      return joints;
    }

    mlineData.Elements.forEach((element, elementIndex) => {
      if (!element.Points || element.Points.length < 3) {
        return;
      }

      for (let i = 1; i < element.Points.length - 1; i++) {
        const point = element.Points[i];

        const geometry = new THREE.CircleGeometry(2, 16);
        const color = this.getColorFromIndex(element.ColorIndex);
        const material = new THREE.MeshBasicMaterial({
          color: color,
          transparent: mlineData.Transparent || false,
          opacity: mlineData.Opacity || 1.0
        });

        const circle = new THREE.Mesh(geometry, material);
        circle.position.set(point.X, point.Y, 0.1);
        circle.name = `Joint_${elementIndex}_${i}`;
        circle.userData = {
          type: mlineData.Type,
          entityType: mlineData.EntityType,
          handle: mlineData.Handle,
          layerName: mlineData.LayerName,
          layerIndex: mlineData.LayerIndex,
          objectType: 'MLineJoint',
          elementIndex: elementIndex,
          pointIndex: i
        };
        circle.visible = mlineData.Visible;
        joints.push(circle);
      }
    });

    return joints;
  }

  public static dispose(mlineData: MLineData, scene: THREE.Scene): boolean {
    if (!mlineData || !scene) {
      return false;
    }
    
    const objectsToRemove: THREE.Object3D[] = [];
    scene.traverse((object) => {
      if (object.userData && object.userData.handle === mlineData.Handle) {
        objectsToRemove.push(object);
      }
    });
    
    objectsToRemove.forEach(obj => {
      scene.remove(obj);
      if (obj instanceof THREE.Mesh || obj instanceof THREE.Line) {
        if (obj.geometry) {
          obj.geometry.dispose();
        }
        if (obj.material) {
          if (Array.isArray(obj.material)) {
            obj.material.forEach(material => material.dispose());
          } else {
            obj.material.dispose();
          }
        }
      }
    });
    
    return objectsToRemove.length > 0;
  }

  public static update(mlineData: MLineData, scene: THREE.Scene): boolean {
    if (!mlineData || !scene) {
      return false;
    }
    this.dispose(mlineData, scene);
    const result = this.render(mlineData, scene);
    return result !== null;
  }

  public static getMLineObjects(mlineData: MLineData, scene: THREE.Scene): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];
    scene.traverse((object) => {
      if (object.userData && object.userData.handle === mlineData.Handle) {
        objects.push(object);
      }
    });
    return objects;
  }

  public static setVisibility(mlineData: MLineData, scene: THREE.Scene, visible: boolean): boolean {
    const objects = this.getMLineObjects(mlineData, scene);
    objects.forEach(obj => {
      obj.visible = visible;
    });
    return objects.length > 0;
  }

  public static setColor(mlineData: MLineData, scene: THREE.Scene, color: string): boolean {
    const objects = this.getMLineObjects(mlineData, scene);
    objects.forEach(obj => {
      if (obj instanceof THREE.Line) {
        if (obj.material instanceof THREE.LineBasicMaterial) {
          obj.material.color.set(color);
        }
      }
    });
    return objects.length > 0;
  }

  public static setElementColor(mlineData: MLineData, scene: THREE.Scene, elementIndex: number, color: string): boolean {
    const objects = this.getMLineObjects(mlineData, scene);
    const element = objects.find(obj => obj.name === `Element_${elementIndex}`);
    if (element instanceof THREE.Line && element.material instanceof THREE.LineBasicMaterial) {
      element.material.color.set(color);
      return true;
    }
    return false;
  }

  public static setOpacity(mlineData: MLineData, scene: THREE.Scene, opacity: number): boolean {
    const objects = this.getMLineObjects(mlineData, scene);
    if (objects.length === 0) {
      return false;
    }
    objects.forEach((object) => {
      if (object instanceof THREE.Line || object instanceof THREE.Mesh) {
        if (object.material instanceof THREE.LineBasicMaterial || 
            object.material instanceof THREE.MeshBasicMaterial) {
          object.material.opacity = opacity;
          object.material.transparent = opacity < 1.0;
        }
      }
    });
    return true;
  }

  public static setFillOpacity(mlineData: MLineData, scene: THREE.Scene, opacity: number): boolean {
    const objects = this.getMLineObjects(mlineData, scene);
    const fill = objects.find(obj => obj.name === 'Fill');
    if (fill instanceof THREE.Mesh && fill.material instanceof THREE.MeshBasicMaterial) {
      fill.material.opacity = opacity;
      fill.material.transparent = opacity < 1.0;
      return true;
    }
    return false;
  }

  public static calculateLength(mlineData: MLineData): number {
    if (!mlineData.Elements || mlineData.Elements.length === 0) {
      return 0;
    }

    let totalLength = 0;
    mlineData.Elements.forEach(element => {
      if (element.Points && element.Points.length > 1) {
        for (let i = 0; i < element.Points.length - 1; i++) {
          const dx = element.Points[i + 1].X - element.Points[i].X;
          const dy = element.Points[i + 1].Y - element.Points[i].Y;
          totalLength += Math.sqrt(dx * dx + dy * dy);
        }
      }
    });

    return totalLength;
  }

  public static calculateElementLength(mlineData: MLineData, elementIndex: number): number {
    if (!mlineData.Elements || elementIndex < 0 || elementIndex >= mlineData.Elements.length) {
      return 0;
    }

    const element = mlineData.Elements[elementIndex];
    if (!element.Points || element.Points.length < 2) {
      return 0;
    }

    let length = 0;
    for (let i = 0; i < element.Points.length - 1; i++) {
      const dx = element.Points[i + 1].X - element.Points[i].X;
      const dy = element.Points[i + 1].Y - element.Points[i].Y;
      length += Math.sqrt(dx * dx + dy * dy);
    }

    return length;
  }

  public static getLength(mlineData: MLineData): number {
    return mlineData.TotalLength || 0;
  }

  public static getElementLength(mlineData: MLineData, elementIndex: number): number {
    if (!mlineData.Elements || elementIndex < 0 || elementIndex >= mlineData.Elements.length) {
      return 0;
    }
    return mlineData.Elements[elementIndex].TotalLength || 0;
  }

  public static getCentroid(mlineData: MLineData): Point3DData {
    return mlineData.Centroid3D || { X: 0, Y: 0, Z: 0 };
  }

  public static getBounds(mlineData: MLineData): BoundsData {
    return mlineData.Bounds || {
      Min: { X: 0, Y: 0, Z: 0 },
      Max: { X: 0, Y: 0, Z: 0 },
      Center: { X: 0, Y: 0, Z: 0 },
      Size: { X: 0, Y: 0, Z: 0 }
    };
  }

  public static isPointOnMLine(mlineData: MLineData, point: Point3DData, tolerance: number = 0.001): boolean {
    if (!mlineData.Elements || mlineData.Elements.length === 0) {
      return false;
    }

    for (const element of mlineData.Elements) {
      if (!element.Points || element.Points.length < 2) {
        continue;
      }

      for (let i = 0; i < element.Points.length - 1; i++) {
        const start = new THREE.Vector3(element.Points[i].X, element.Points[i].Y, 0);
        const end = new THREE.Vector3(element.Points[i + 1].X, element.Points[i + 1].Y, 0);
        const testPoint = new THREE.Vector3(point.X, point.Y, 0);

        const lineLength = start.distanceTo(end);
        const distToStart = start.distanceTo(testPoint);
        const distToEnd = end.distanceTo(testPoint);

        if (Math.abs(distToStart + distToEnd - lineLength) < tolerance) {
          return true;
        }
      }
    }

    return false;
  }

  public static getClosestPoint(mlineData: MLineData, point: Point3DData): Point3DData | null {
    if (!mlineData.Elements || mlineData.Elements.length === 0) {
      return null;
    }

    let closestPoint: Point3DData | null = null;
    let minDistance = Infinity;

    for (const element of mlineData.Elements) {
      if (!element.Points || element.Points.length < 2) {
        continue;
      }

      for (let i = 0; i < element.Points.length - 1; i++) {
        const start = new THREE.Vector3(element.Points[i].X, element.Points[i].Y, 0);
        const end = new THREE.Vector3(element.Points[i + 1].X, element.Points[i + 1].Y, 0);
        const testPoint = new THREE.Vector3(point.X, point.Y, 0);

        const lineVec = new THREE.Vector3().subVectors(end, start);
        const pointVec = new THREE.Vector3().subVectors(testPoint, start);

        const lineLength = lineVec.length();
        if (lineLength === 0) {
          continue;
        }

        const t = Math.max(0, Math.min(1, pointVec.dot(lineVec) / (lineLength * lineLength)));
        const currentClosestPoint = start.clone().add(lineVec.multiplyScalar(t));

        const distance = testPoint.distanceTo(currentClosestPoint);
        if (distance < minDistance) {
          minDistance = distance;
          closestPoint = { X: currentClosestPoint.x, Y: currentClosestPoint.y, Z: 0 };
        }
      }
    }

    return closestPoint;
  }

  public static getDistanceToPoint(mlineData: MLineData, point: Point3DData): number {
    const closestPoint = this.getClosestPoint(mlineData, point);
    if (!closestPoint) {
      return Infinity;
    }

    const testPoint = new THREE.Vector3(point.X, point.Y, point.Z);
    const closest = new THREE.Vector3(closestPoint.X, closestPoint.Y, closestPoint.Z);

    return testPoint.distanceTo(closest);
  }

  public static clone(mlineData: MLineData, newHandle: string): MLineData {
    const cloned = JSON.parse(JSON.stringify(mlineData));
    cloned.Handle = newHandle;
    return cloned;
  }

  public static transform(mlineData: MLineData, matrix: THREE.Matrix4): MLineData {
    const transformed = JSON.parse(JSON.stringify(mlineData));

    if (transformed.StartPoint) {
      const startPoint = new THREE.Vector3(mlineData.StartPoint.X, mlineData.StartPoint.Y, mlineData.StartPoint.Z);
      startPoint.applyMatrix4(matrix);
      transformed.StartPoint = { X: startPoint.x, Y: startPoint.y, Z: startPoint.z };
    }

    if (transformed.EndPoint) {
      const endPoint = new THREE.Vector3(mlineData.EndPoint.X, mlineData.EndPoint.Y, mlineData.EndPoint.Z);
      endPoint.applyMatrix4(matrix);
      transformed.EndPoint = { X: endPoint.x, Y: endPoint.y, Z: endPoint.z };
    }

    if (transformed.Centroid3D) {
      const centroid = new THREE.Vector3(mlineData.Centroid3D.X, mlineData.Centroid3D.Y, mlineData.Centroid3D.Z);
      centroid.applyMatrix4(matrix);
      transformed.Centroid3D = { X: centroid.x, Y: centroid.y, Z: centroid.z };
    }

    if (transformed.Vertices3D) {
      transformed.Vertices3D = mlineData.Vertices3D.map(v => {
        const vertex = new THREE.Vector3(v.X, v.Y, v.Z);
        vertex.applyMatrix4(matrix);
        return { X: vertex.x, Y: vertex.y, Z: vertex.z };
      });
    }

    if (transformed.Elements) {
      transformed.Elements = mlineData.Elements.map(element => {
        return {
          ...element,
          Points: element.Points.map(p => {
            const point = new THREE.Vector3(p.X, p.Y, 0);
            point.applyMatrix4(matrix);
            return { X: point.x, Y: point.y };
          })
        };
      });
    }

    transformed.TotalLength = this.calculateLength(transformed);

    return transformed;
  }

  private static getColorFromIndex(colorIndex: number): string {
    const colors: { [key: number]: string } = {
      0: '#ffffff',
      1: '#ff0000',
      2: '#ffff00',
      3: '#00ff00',
      4: '#00ffff',
      5: '#0000ff',
      6: '#ff00ff',
      7: '#ffffff'
    };
    return colors[colorIndex] || '#ffffff';
  }

  public static getElementCount(mlineData: MLineData): number {
    if (!mlineData.Elements || mlineData.Elements.length === 0) {
      return 0;
    }
    return mlineData.Elements.length;
  }

  public static getVertexCount(mlineData: MLineData): number {
    if (!mlineData.Vertices3D || mlineData.Vertices3D.length === 0) {
      return 0;
    }
    return mlineData.Vertices3D.length;
  }
}
