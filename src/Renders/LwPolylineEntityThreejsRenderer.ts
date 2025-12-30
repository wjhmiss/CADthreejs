import * as THREE from 'three';

export interface LwPolylineVertexData {
  X: number;
  Y: number;
  Bulge: number;
  StartWidth: number;
  EndWidth: number;
  Location3D: Point3DData;
}

export interface Point3DData {
  X: number;
  Y: number;
  Z: number;
}

export interface PointData {
  X: number;
  Y: number;
}

export interface ColorData {
  Index: number;
  Hex: string;
  R: number;
  G: number;
  B: number;
  A: number;
}

export interface NormalData {
  X: number;
  Y: number;
  Z: number;
}

export interface TransformData {
  Position: Point3DData;
  Rotation: Point3DData;
  Scale: Point3DData;
  Matrix: number[];
}

export interface BoundsData {
  Min: Point3DData;
  Max: Point3DData;
  Center: Point3DData;
  Size: Point3DData;
}

export interface LwPolylineData {
  Vertices: LwPolylineVertexData[];
  IsClosed: boolean;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  LineTypeScale: number;
  TotalLength: number;
  VertexCount: number;
  Centroid: PointData;
  Points: PointData[];
  Type: string;
  EntityType: string;
  Handle: string;
  LayerName: string;
  LayerIndex: number;
  Visible: boolean;
  CoordinateSystem: string;
  Vertices3D: Point3DData[];
  Centroid3D: Point3DData;
  Direction: Point3DData[];
  Bounds: BoundsData;
  Color: ColorData;
  Transform: TransformData;
  Normal: NormalData;
  Opacity: number;
  Transparent: boolean;
  MaterialType: string;
  DepthTest: boolean;
  DepthWrite: boolean;
  Elevation: number;
  ConstantWidth: number;
  Thickness: number;
}

export class LwPolylineEntityThreejsRenderer {
  private static readonly lineNamePrefix = 'lwPolyline-';

  public static render(lwPolylineData: LwPolylineData, scene: THREE.Scene): THREE.Line | null {
    if (!lwPolylineData || !lwPolylineData.Visible) {
      return null;
    }

    if (!lwPolylineData.Vertices || lwPolylineData.Vertices.length === 0) {
      return null;
    }

    const vertices = lwPolylineData.Vertices;
    const points: THREE.Vector3[] = [];

    for (let i = 0; i < vertices.length; i++) {
      const vertex = vertices[i];
      const z = vertex.Location3D?.Z ?? lwPolylineData.Elevation;
      const point = new THREE.Vector3(vertex.X, vertex.Y, z);
      points.push(point);

      if (vertex.Bulge !== 0 && i < vertices.length - 1) {
        const nextVertex = vertices[i + 1];
        const nextZ = nextVertex.Location3D?.Z ?? lwPolylineData.Elevation;
        const nextPoint = new THREE.Vector3(nextVertex.X, nextVertex.Y, nextZ);
        const arcPoints = this.calculateArcPoints(vertex, nextVertex, vertex.Bulge);
        points.pop();
        points.push(...arcPoints);
      }
    }

    if (lwPolylineData.IsClosed && points.length > 0) {
      points.push(points[0].clone());
    }

    if (points.length < 2) {
      return null;
    }

    const geometry = new THREE.BufferGeometry().setFromPoints(points);
    
    const material = this.createMaterial(lwPolylineData);
    
    const line = new THREE.Line(geometry, material);
    line.name = `${this.lineNamePrefix}${lwPolylineData.Handle || 'unknown'}`;
    
    this.applyTransform(line, lwPolylineData.Transform);
    this.setUserData(line, lwPolylineData);

    return line;
  }

  private static createMaterial(lwPolylineData: LwPolylineData): THREE.Material {
    const color = new THREE.Color(lwPolylineData.Color.Hex);
    
    const materialConfig: THREE.LineBasicMaterialParameters = {
      color: color,
      transparent: lwPolylineData.Transparent || lwPolylineData.Opacity < 1.0,
      opacity: lwPolylineData.Opacity,
      depthTest: lwPolylineData.DepthTest,
      depthWrite: lwPolylineData.DepthWrite,
    };

    const lineWidth = this.calculateLineWidth(lwPolylineData);
    if (lineWidth !== undefined) {
      (materialConfig as any).linewidth = lineWidth;
    }

    return new THREE.LineBasicMaterial(materialConfig);
  }

  private static calculateLineWidth(lwPolylineData: LwPolylineData): number | undefined {
    if (lwPolylineData.ConstantWidth > 0) {
      return lwPolylineData.ConstantWidth;
    }

    if (lwPolylineData.LineWeight > 0) {
      return lwPolylineData.LineWeight;
    }

    return undefined;
  }

  private static applyTransform(line: THREE.Line, transform: TransformData): void {
    if (!transform) return;

    const matrix = new THREE.Matrix4();
    if (transform.Matrix && transform.Matrix.length === 16) {
      matrix.fromArray(transform.Matrix);
    } else {
      const position = new THREE.Vector3(transform.Position.X, transform.Position.Y, transform.Position.Z);
      const rotation = new THREE.Euler(transform.Rotation.X, transform.Rotation.Y, transform.Rotation.Z);
      const scale = new THREE.Vector3(transform.Scale.X, transform.Scale.Y, transform.Scale.Z);
      
      matrix.compose(position, new THREE.Quaternion().setFromEuler(rotation), scale);
    }

    line.applyMatrix4(matrix);
  }

  private static setUserData(line: THREE.Line, lwPolylineData: LwPolylineData): void {
    line.userData = {
      type: lwPolylineData.Type,
      entityType: lwPolylineData.EntityType,
      handle: lwPolylineData.Handle,
      layerName: lwPolylineData.LayerName,
      layerIndex: lwPolylineData.LayerIndex,
      visible: lwPolylineData.Visible,
      coordinateSystem: lwPolylineData.CoordinateSystem,
      vertices: lwPolylineData.Vertices,
      isClosed: lwPolylineData.IsClosed,
      totalLength: lwPolylineData.TotalLength,
      vertexCount: lwPolylineData.VertexCount,
      centroid: lwPolylineData.Centroid,
      centroid3D: lwPolylineData.Centroid3D,
      direction: lwPolylineData.Direction,
      bounds: lwPolylineData.Bounds,
      color: lwPolylineData.Color,
      transform: lwPolylineData.Transform,
      normal: lwPolylineData.Normal,
      opacity: lwPolylineData.Opacity,
      transparent: lwPolylineData.Transparent,
      materialType: lwPolylineData.MaterialType,
      depthTest: lwPolylineData.DepthTest,
      depthWrite: lwPolylineData.DepthWrite,
      elevation: lwPolylineData.Elevation,
      constantWidth: lwPolylineData.ConstantWidth,
      thickness: lwPolylineData.Thickness,
      lineTypeName: lwPolylineData.LineTypeName,
      lineWeight: lwPolylineData.LineWeight,
      lineTypeScale: lwPolylineData.LineTypeScale,
    };
  }

  private static calculateArcPoints(startVertex: LwPolylineVertexData, endVertex: LwPolylineVertexData, bulge: number): THREE.Vector3[] {
    const startPoint = new THREE.Vector3(startVertex.X, startVertex.Y, startVertex.Location3D.Z);
    const endPoint = new THREE.Vector3(endVertex.X, endVertex.Y, endVertex.Location3D.Z);

    const chord = startPoint.distanceTo(endPoint);
    const sagitta = chord * Math.abs(bulge) / 2;
    
    const midPoint = new THREE.Vector3().addVectors(startPoint, endPoint).multiplyScalar(0.5);
    const direction = new THREE.Vector3().subVectors(endPoint, startPoint).normalize();
    const perpendicular = new THREE.Vector3(-direction.y, direction.x, 0);
    
    const center = midPoint.clone().add(perpendicular.multiplyScalar(sagitta * Math.sign(bulge)));
    const radius = center.distanceTo(startPoint);
    
    const startAngle = Math.atan2(startPoint.y - center.y, startPoint.x - center.x);
    const endAngle = Math.atan2(endPoint.y - center.y, endPoint.x - center.x);
    
    const numSegments = 16;
    const arcPoints: THREE.Vector3[] = [];
    
    for (let i = 0; i <= numSegments; i++) {
      const t = i / numSegments;
      const angle = startAngle + (endAngle - startAngle) * t;
      const x = center.x + radius * Math.cos(angle);
      const y = center.y + radius * Math.sin(angle);
      const z = startPoint.z + (endPoint.z - startPoint.z) * t;
      arcPoints.push(new THREE.Vector3(x, y, z));
    }
    
    return arcPoints;
  }

  public static update(lwPolylineData: LwPolylineData, scene: THREE.Scene): boolean {
    const line = this.getLwPolylineLine(lwPolylineData, scene);
    if (!line) {
      return false;
    }
    this.dispose(lwPolylineData, scene);
    const result = this.render(lwPolylineData, scene);
    return result !== null;
  }

  public static getLwPolylineLine(lwPolylineData: LwPolylineData, scene: THREE.Scene): THREE.Line | null {
    if (!lwPolylineData || !lwPolylineData.Handle) {
      return null;
    }

    const lineName = `${this.lineNamePrefix}${lwPolylineData.Handle}`;
    const line = scene.getObjectByName(lineName) as THREE.Line;
    
    return line || null;
  }

  public static setVisibility(lwPolylineData: LwPolylineData, scene: THREE.Scene, visible: boolean): boolean {
    const line = this.getLwPolylineLine(lwPolylineData, scene);
    if (!line) {
      return false;
    }

    line.visible = visible;
    line.userData.visible = visible;
    
    return true;
  }

  public static setColor(lwPolylineData: LwPolylineData, scene: THREE.Scene, color: THREE.Color): boolean {
    const line = this.getLwPolylineLine(lwPolylineData, scene);
    if (!line) {
      return false;
    }

    if (line.material instanceof THREE.LineBasicMaterial) {
      line.material.color = color;
    }

    line.userData.color = {
      Index: lwPolylineData.Color.Index,
      Hex: '#' + color.getHexString(),
      R: Math.round(color.r * 255),
      G: Math.round(color.g * 255),
      B: Math.round(color.b * 255),
      A: lwPolylineData.Color.A,
    };

    return true;
  }

  public static setOpacity(lwPolylineData: LwPolylineData, scene: THREE.Scene, opacity: number): boolean {
    const line = this.getLwPolylineLine(lwPolylineData, scene);
    if (!line) {
      return false;
    }

    if (line.material instanceof THREE.LineBasicMaterial) {
      line.material.opacity = opacity;
      line.material.transparent = opacity < 1.0;
    }

    line.userData.opacity = opacity;
    line.userData.transparent = opacity < 1.0;

    return true;
  }

  public static setLineWidth(lwPolylineData: LwPolylineData, scene: THREE.Scene, lineWidth: number): boolean {
    const line = this.getLwPolylineLine(lwPolylineData, scene);
    if (!line) {
      return false;
    }

    if (line.material instanceof THREE.LineBasicMaterial) {
      (line.material as any).linewidth = lineWidth;
    }

    line.userData.lineWeight = lineWidth;

    return true;
  }

  public static isPointOnLwPolyline(lwPolylineData: LwPolylineData, scene: THREE.Scene, point: THREE.Vector3, tolerance: number = 0.01): boolean {
    const line = this.getLwPolylineLine(lwPolylineData, scene);
    if (!line) {
      return false;
    }

    const positions = line.geometry.attributes.position.array as Float32Array;
    const pointCount = positions.length / 3;

    for (let i = 0; i < pointCount - 1; i++) {
      const p1 = new THREE.Vector3(positions[i * 3], positions[i * 3 + 1], positions[i * 3 + 2]);
      const p2 = new THREE.Vector3(positions[(i + 1) * 3], positions[(i + 1) * 3 + 1], positions[(i + 1) * 3 + 2]);

      const closestPoint = this.getClosestPointOnSegment(point, p1, p2);
      if (closestPoint.distanceTo(point) < tolerance) {
        return true;
      }
    }

    return false;
  }

  public static getPointAtParameter(lwPolylineData: LwPolylineData, scene: THREE.Scene, parameter: number): THREE.Vector3 | null {
    if (parameter < 0 || parameter > 1) {
      return null;
    }

    const line = this.getLwPolylineLine(lwPolylineData, scene);
    if (!line) {
      return null;
    }

    const positions = line.geometry.attributes.position.array as Float32Array;
    const pointCount = positions.length / 3;

    if (pointCount < 2) {
      return null;
    }

    let effectivePointCount = pointCount;
    if (lwPolylineData.IsClosed && pointCount > 1) {
      effectivePointCount = pointCount - 1;
    }

    const index = Math.floor(parameter * (effectivePointCount - 1));
    const nextIndex = Math.min(index + 1, effectivePointCount - 1);
    const t = (parameter * (effectivePointCount - 1)) - index;

    const currentPoint = new THREE.Vector3(
      positions[index * 3],
      positions[index * 3 + 1],
      positions[index * 3 + 2]
    );
    const nextPoint = new THREE.Vector3(
      positions[nextIndex * 3],
      positions[nextIndex * 3 + 1],
      positions[nextIndex * 3 + 2]
    );

    return new THREE.Vector3().lerpVectors(currentPoint, nextPoint, t);
  }

  public static getClosestPoint(lwPolylineData: LwPolylineData, scene: THREE.Scene, point: THREE.Vector3): THREE.Vector3 | null {
    const line = this.getLwPolylineLine(lwPolylineData, scene);
    if (!line) {
      return null;
    }

    const positions = line.geometry.attributes.position.array as Float32Array;
    const pointCount = positions.length / 3;

    if (pointCount < 2) {
      return null;
    }

    let closestPoint: THREE.Vector3 | null = null;
    let minDistance = Infinity;

    for (let i = 0; i < pointCount - 1; i++) {
      const p1 = new THREE.Vector3(positions[i * 3], positions[i * 3 + 1], positions[i * 3 + 2]);
      const p2 = new THREE.Vector3(positions[(i + 1) * 3], positions[(i + 1) * 3 + 1], positions[(i + 1) * 3 + 2]);

      const segmentClosest = this.getClosestPointOnSegment(point, p1, p2);
      const distance = segmentClosest.distanceTo(point);

      if (distance < minDistance) {
        minDistance = distance;
        closestPoint = segmentClosest;
      }
    }

    return closestPoint;
  }

  private static getClosestPointOnSegment(point: THREE.Vector3, p1: THREE.Vector3, p2: THREE.Vector3): THREE.Vector3 {
    const segment = new THREE.Vector3().subVectors(p2, p1);
    const length = segment.length();
    
    if (length === 0) {
      return p1.clone();
    }

    const t = Math.max(0, Math.min(1, new THREE.Vector3().subVectors(point, p1).dot(segment) / (length * length)));
    return new THREE.Vector3().addVectors(p1, segment.multiplyScalar(t));
  }

  public static dispose(lwPolylineData: LwPolylineData, scene: THREE.Scene): boolean {
    if (!lwPolylineData) {
      return false;
    }

    const line = this.getLwPolylineLine(lwPolylineData, scene);
    if (!line) {
      return false;
    }

    if (line.geometry) {
      line.geometry.dispose();
    }
    if (line.material) {
      if (Array.isArray(line.material)) {
        line.material.forEach((mat) => mat.dispose());
      } else {
        line.material.dispose();
      }
    }

    scene.remove(line);

    return true;
  }

  public static calculateLength(lwPolylineData: LwPolylineData): number {
    if (!lwPolylineData || !lwPolylineData.Vertices || lwPolylineData.Vertices.length < 2) {
      return 0;
    }

    let totalLength = 0;
    const vertices = lwPolylineData.Vertices;

    for (let i = 0; i < vertices.length - 1; i++) {
      const current = vertices[i];
      const next = vertices[i + 1];

      if (current.Bulge !== 0) {
        const arcLength = this.calculateArcLength(current, next, current.Bulge);
        totalLength += arcLength;
      } else {
        const dx = next.X - current.X;
        const dy = next.Y - current.Y;
        const dz = next.Location3D.Z - current.Location3D.Z;
        const segmentLength = Math.sqrt(dx * dx + dy * dy + dz * dz);
        totalLength += segmentLength;
      }
    }

    if (lwPolylineData.IsClosed && vertices.length > 1) {
      const first = vertices[0];
      const last = vertices[vertices.length - 1];
      const dx = first.X - last.X;
      const dy = first.Y - last.Y;
      const dz = first.Location3D.Z - last.Location3D.Z;
      const segmentLength = Math.sqrt(dx * dx + dy * dy + dz * dz);
      totalLength += segmentLength;
    }

    return totalLength;
  }

  private static calculateArcLength(startVertex: LwPolylineVertexData, endVertex: LwPolylineVertexData, bulge: number): number {
    const startPoint = new THREE.Vector3(startVertex.X, startVertex.Y, startVertex.Location3D.Z);
    const endPoint = new THREE.Vector3(endVertex.X, endVertex.Y, endVertex.Location3D.Z);

    const chord = startPoint.distanceTo(endPoint);
    const sagitta = chord * Math.abs(bulge) / 2;
    
    const midPoint = new THREE.Vector3().addVectors(startPoint, endPoint).multiplyScalar(0.5);
    const direction = new THREE.Vector3().subVectors(endPoint, startPoint).normalize();
    const perpendicular = new THREE.Vector3(-direction.y, direction.x, 0);
    
    const center = midPoint.clone().add(perpendicular.multiplyScalar(sagitta * Math.sign(bulge)));
    const radius = center.distanceTo(startPoint);
    
    const includedAngle = 4 * Math.atan(Math.abs(bulge));
    const arcLength = radius * includedAngle;

    return arcLength;
  }

  public static calculateArea(lwPolylineData: LwPolylineData): number {
    if (!lwPolylineData || !lwPolylineData.Vertices || lwPolylineData.Vertices.length < 3) {
      return 0;
    }

    const vertices = lwPolylineData.Vertices;
    let area = 0;

    for (let i = 0; i < vertices.length; i++) {
      const current = vertices[i];
      const next = vertices[(i + 1) % vertices.length];
      
      area += current.X * next.Y - next.X * current.Y;
    }

    return Math.abs(area) / 2;
  }

  public static getVertexCount(lwPolylineData: LwPolylineData): number {
    return lwPolylineData?.Vertices?.length || 0;
  }

  public static getCentroid(lwPolylineData: LwPolylineData): THREE.Vector3 | null {
    if (!lwPolylineData || !lwPolylineData.Centroid3D) {
      return null;
    }

    return new THREE.Vector3(
      lwPolylineData.Centroid3D.X,
      lwPolylineData.Centroid3D.Y,
      lwPolylineData.Centroid3D.Z
    );
  }

  public static getBounds(lwPolylineData: LwPolylineData): THREE.Box3 | null {
    if (!lwPolylineData || !lwPolylineData.Bounds) {
      return null;
    }

    const min = new THREE.Vector3(
      lwPolylineData.Bounds.Min.X,
      lwPolylineData.Bounds.Min.Y,
      lwPolylineData.Bounds.Min.Z
    );
    const max = new THREE.Vector3(
      lwPolylineData.Bounds.Max.X,
      lwPolylineData.Bounds.Max.Y,
      lwPolylineData.Bounds.Max.Z
    );

    return new THREE.Box3(min, max);
  }
}
