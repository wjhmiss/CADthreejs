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

export interface LeaderData {
  Type: string;
  Handle: string;
  LayerName: string;
  Visible: boolean;
  CoordinateSystem: string;
  
  Vertices: PointData[];
  Vertices3D: Point3DData[];
  PathType: string;
  ArrowHeadEnabled: boolean;
  HasHookline: boolean;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  
  ArrowHeadPoints: PointData[];
  ArrowHeadPoints3D: Point3DData[];
  HookLineStart: PointData;
  HookLineEnd: PointData;
  HookLineStart3D: Point3DData;
  HookLineEnd3D: Point3DData;
  
  TotalLength: number;
  StartPoint: PointData;
  EndPoint: PointData;
  StartPoint3D: Point3DData;
  EndPoint3D: Point3DData;
  VertexCount: number;
  
  Bounds: BoundsData;
  Color: ColorData;
  Transform3D: TransformData;
  
  Normal: Point3DData;
  HorizontalDirection: Point3DData;
  AnnotationOffset: Point3DData;
  BlockOffset: Point3DData;
  CreationType: string;
  HookLineDirection: string;
  TextHeight: number;
  TextWidth: number;
  StyleName: string;
  
  Opacity: number;
  Transparent: boolean;
  MaterialType: string;
  DepthTest: boolean;
  DepthWrite: boolean;
}

export class LeaderEntityThreejsRenderer {
  public static render(leaderData: LeaderData, scene: THREE.Scene): THREE.Object3D[] | null {
    if (!leaderData || !leaderData.Visible) {
      return null;
    }

    const objects: THREE.Object3D[] = [];

    const userData = {
      type: leaderData.Type,
      handle: leaderData.Handle,
      layerName: leaderData.LayerName,
      pathType: leaderData.PathType,
      arrowHeadEnabled: leaderData.ArrowHeadEnabled,
      hasHookline: leaderData.HasHookline,
      colorIndex: leaderData.ColorIndex,
      lineTypeName: leaderData.LineTypeName,
      lineWeight: leaderData.LineWeight,
      totalLength: leaderData.TotalLength,
      vertexCount: leaderData.VertexCount,
      creationType: leaderData.CreationType,
      hookLineDirection: leaderData.HookLineDirection,
      textHeight: leaderData.TextHeight,
      textWidth: leaderData.TextWidth,
      styleName: leaderData.StyleName,
      opacity: leaderData.Opacity,
      transparent: leaderData.Transparent,
      materialType: leaderData.MaterialType,
      depthTest: leaderData.DepthTest,
      depthWrite: leaderData.DepthWrite,
      normal: leaderData.Normal,
      horizontalDirection: leaderData.HorizontalDirection,
      annotationOffset: leaderData.AnnotationOffset,
      blockOffset: leaderData.BlockOffset,
      bounds: leaderData.Bounds,
      transform: leaderData.Transform3D,
      objectType: 'Leader'
    };

    this.renderLeaderLine(leaderData, objects);

    if (leaderData.ArrowHeadEnabled && leaderData.ArrowHeadPoints3D && leaderData.ArrowHeadPoints3D.length > 0) {
      this.renderArrowHead(leaderData, objects);
    }

    if (leaderData.HasHookline && leaderData.HookLineStart3D && leaderData.HookLineEnd3D) {
      this.renderHookLine(leaderData, objects);
    }

    objects.forEach(obj => {
      obj.userData = { ...userData, ...obj.userData };
      obj.name = `Leader_${leaderData.Handle}_${obj.name}`;
      obj.visible = leaderData.Visible;

      if (leaderData.Transform3D && leaderData.Transform3D.Matrix) {
        const matrix = new THREE.Matrix4();
        matrix.fromArray(leaderData.Transform3D.Matrix);
        obj.applyMatrix4(matrix);
      }
    });

    return objects;
  }

  private static renderLeaderLine(leaderData: LeaderData, objects: THREE.Object3D[]): void {
    if (!leaderData.Vertices3D || leaderData.Vertices3D.length < 2) {
      return;
    }

    const vertices: number[] = [];
    for (const vertex of leaderData.Vertices3D) {
      vertices.push(vertex.X, vertex.Y, vertex.Z);
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));

    const material = new THREE.LineBasicMaterial({
      color: leaderData.Color?.Hex || '#000000',
      transparent: leaderData.Transparent || false,
      opacity: leaderData.Opacity || 1.0,
      depthTest: leaderData.DepthTest !== false,
      depthWrite: leaderData.DepthWrite !== false
    });

    const line = new THREE.Line(geometry, material);
    line.name = 'LeaderLine';
    line.userData = {
      type: 'LeaderLine',
      pathType: leaderData.PathType,
      vertexCount: leaderData.VertexCount,
      totalLength: leaderData.TotalLength
    };

    objects.push(line);
  }

  private static renderArrowHead(leaderData: LeaderData, objects: THREE.Object3D[]): void {
    if (!leaderData.ArrowHeadPoints3D || leaderData.ArrowHeadPoints3D.length < 3) {
      return;
    }

    const vertices: number[] = [];
    for (const point of leaderData.ArrowHeadPoints3D) {
      vertices.push(point.X, point.Y, point.Z);
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));
    geometry.setIndex([0, 1, 2]);

    const material = new THREE.MeshBasicMaterial({
      color: leaderData.Color?.Hex || '#000000',
      transparent: leaderData.Transparent || false,
      opacity: leaderData.Opacity || 1.0,
      side: THREE.DoubleSide,
      depthTest: leaderData.DepthTest !== false,
      depthWrite: leaderData.DepthWrite !== false
    });

    const arrowHead = new THREE.Mesh(geometry, material);
    arrowHead.name = 'ArrowHead';
    arrowHead.userData = {
      type: 'ArrowHead',
      enabled: leaderData.ArrowHeadEnabled
    };

    objects.push(arrowHead);
  }

  private static renderHookLine(leaderData: LeaderData, objects: THREE.Object3D[]): void {
    if (!leaderData.HookLineStart3D || !leaderData.HookLineEnd3D) {
      return;
    }

    const vertices = [
      leaderData.HookLineStart3D.X, leaderData.HookLineStart3D.Y, leaderData.HookLineStart3D.Z,
      leaderData.HookLineEnd3D.X, leaderData.HookLineEnd3D.Y, leaderData.HookLineEnd3D.Z
    ];

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));

    const material = new THREE.LineBasicMaterial({
      color: leaderData.Color?.Hex || '#000000',
      transparent: leaderData.Transparent || false,
      opacity: leaderData.Opacity || 1.0,
      depthTest: leaderData.DepthTest !== false,
      depthWrite: leaderData.DepthWrite !== false
    });

    const hookLine = new THREE.Line(geometry, material);
    hookLine.name = 'HookLine';
    hookLine.userData = {
      type: 'HookLine',
      direction: leaderData.HookLineDirection
    };

    objects.push(hookLine);
  }

  public static dispose(leaderData: LeaderData, scene: THREE.Scene): void {
    if (!leaderData || !scene) {
      return;
    }
    
    const objectsToRemove: THREE.Object3D[] = [];
    scene.traverse((object) => {
      if (object.userData && object.userData.handle === leaderData.Handle) {
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
  }

  public static update(leaderData: LeaderData, scene: THREE.Scene): THREE.Object3D[] | null {
    this.dispose(leaderData, scene);
    return this.render(leaderData, scene);
  }

  public static getLeaderObjects(leaderData: LeaderData, scene: THREE.Scene): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];
    scene.traverse((object) => {
      if (object.userData && object.userData.handle === leaderData.Handle) {
        objects.push(object);
      }
    });
    return objects;
  }

  public static setVisibility(leaderData: LeaderData, scene: THREE.Scene, visible: boolean): void {
    const objects = this.getLeaderObjects(leaderData, scene);
    objects.forEach(obj => {
      obj.visible = visible;
    });
  }

  public static setColor(leaderData: LeaderData, scene: THREE.Scene, color: string): void {
    const objects = this.getLeaderObjects(leaderData, scene);
    objects.forEach(obj => {
      if (obj instanceof THREE.Mesh || obj instanceof THREE.Line) {
        if (obj.material instanceof THREE.MeshBasicMaterial || obj.material instanceof THREE.LineBasicMaterial) {
          obj.material.color.set(color);
        }
      }
    });
  }

  public static setOpacity(leaderData: LeaderData, scene: THREE.Scene, opacity: number): void {
    const objects = this.getLeaderObjects(leaderData, scene);
    objects.forEach(obj => {
      if (obj instanceof THREE.Mesh || obj instanceof THREE.Line) {
        if (obj.material instanceof THREE.MeshBasicMaterial || obj.material instanceof THREE.LineBasicMaterial) {
          obj.material.opacity = opacity;
          obj.material.transparent = opacity < 1.0;
        }
      }
    });
  }
}
