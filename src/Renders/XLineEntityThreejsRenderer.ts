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

export interface MaterialData {
  Type: string;
  Color: number;
  Opacity: number;
  Transparent: boolean;
  LineWidth: number;
  Side: boolean;
}

export interface GeometryData {
  Type: string;
  VertexCount: number;
  HasColors: boolean;
  HasIndices: boolean;
}

export interface XLineData {
  FirstPoint: Point3DData;
  Direction: Point3DData;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  LinePoints: Point3DData[];
  SecondPoint: Point3DData;
  Length: number;
  Angle: number;
  Transform: TransformData;
  Geometry: GeometryData;
  Material: MaterialData;
  VertexPositions: number[];
  VertexColors: number[];
  Indices: number[];
  VertexCount: number;
  Opacity: number;
  Transparent: boolean;
  Normal: Point3DData;
  Bounds: BoundsData;
  Center: Point3DData;
}

export class XLineEntityThreejsRenderer {
  private static readonly xlineCache = new Map<string, THREE.Line>();

  public static render(xlineData: XLineData, scene: THREE.Scene): THREE.Group {
    const group = new THREE.Group();
    group.name = `XLine_${xlineData.Handle}`;
    group.visible = xlineData.Visible !== false;

    const geometry = this.createXLineGeometry(xlineData);
    const material = this.createXLineMaterial(xlineData);
    const line = new THREE.Line(geometry, material);
    line.name = 'XLine';
    line.userData = { handle: xlineData.Handle };
    line.visible = xlineData.Visible !== false;

    group.add(line);

    this.xlineCache.set(xlineData.Handle, line);

    if (xlineData.Visible !== false) {
      scene.add(group);
    }

    return group;
  }

  public static update(xlineData: XLineData, scene: THREE.Scene): boolean {
    const existingLine = this.xlineCache.get(xlineData.Handle);
    if (!existingLine) {
      return false;
    }

    const material = existingLine.material as THREE.LineBasicMaterial;
    if (material) {
      const color = new THREE.Color(xlineData.Material.Color);
      material.color = color;
      material.opacity = xlineData.Opacity;
      material.transparent = xlineData.Transparent;
      material.linewidth = xlineData.Material.LineWidth;
    }

    const geometry = existingLine.geometry as THREE.BufferGeometry;
    if (geometry && xlineData.VertexPositions) {
      const positions = geometry.attributes.position.array as Float32Array;
      for (let i = 0; i < xlineData.VertexPositions.length && i < positions.length; i++) {
        positions[i] = xlineData.VertexPositions[i];
      }
      geometry.attributes.position.needsUpdate = true;
    }

    return true;
  }

  public static dispose(xlineData: XLineData, scene: THREE.Scene): boolean {
    const existingLine = this.xlineCache.get(xlineData.Handle);
    if (!existingLine) {
      return false;
    }

    const group = existingLine.parent;
    if (group) {
      scene.remove(group);
    }

    existingLine.geometry.dispose();
    (existingLine.material as THREE.Material).dispose();
    this.xlineCache.delete(xlineData.Handle);

    return true;
  }

  public static setVisibility(xlineData: XLineData, scene: THREE.Scene, visible: boolean): boolean {
    const existingLine = this.xlineCache.get(xlineData.Handle);
    if (!existingLine) {
      return false;
    }

    const group = existingLine.parent;
    if (group) {
      if (visible) {
        if (!group.parent) {
          scene.add(group);
        }
      } else {
        if (group.parent) {
          group.parent.remove(group);
        }
      }
      group.visible = visible;
    }
    existingLine.visible = visible;
    return true;
  }

  public static setOpacity(xlineData: XLineData, opacity: number): boolean {
    const existingLine = this.xlineCache.get(xlineData.Handle);
    if (!existingLine) {
      return false;
    }

    const material = existingLine.material as THREE.LineBasicMaterial;
    if (material) {
      material.opacity = opacity;
      material.transparent = opacity < 1.0;
    }

    return true;
  }

  public static setLineWidth(xlineData: XLineData, lineWidth: number): boolean {
    const existingLine = this.xlineCache.get(xlineData.Handle);
    if (!existingLine) {
      return false;
    }

    const material = existingLine.material as THREE.LineBasicMaterial;
    if (material) {
      material.linewidth = lineWidth;
    }

    return true;
  }

  public static getXLine(xlineData: XLineData): THREE.Line | null {
    return this.xlineCache.get(xlineData.Handle) || null;
  }

  public static getFirstPoint(xlineData: XLineData): THREE.Vector3 {
    return new THREE.Vector3(xlineData.FirstPoint.X, xlineData.FirstPoint.Y, xlineData.FirstPoint.Z);
  }

  public static getDirection(xlineData: XLineData): THREE.Vector3 {
    return new THREE.Vector3(xlineData.Direction.X, xlineData.Direction.Y, xlineData.Direction.Z);
  }

  public static getSecondPoint(xlineData: XLineData): THREE.Vector3 {
    return new THREE.Vector3(xlineData.SecondPoint.X, xlineData.SecondPoint.Y, xlineData.SecondPoint.Z);
  }

  public static getLength(xlineData: XLineData): number {
    return xlineData.Length;
  }

  public static getAngle(xlineData: XLineData): number {
    return xlineData.Angle;
  }

  public static getLinePoints(xlineData: XLineData): THREE.Vector3[] {
    return xlineData.LinePoints.map(p => new THREE.Vector3(p.X, p.Y, p.Z));
  }

  public static getCenter(xlineData: XLineData): THREE.Vector3 {
    return new THREE.Vector3(xlineData.Center.X, xlineData.Center.Y, xlineData.Center.Z);
  }

  public static getBounds(xlineData: XLineData): THREE.Box3 {
    const min = new THREE.Vector3(xlineData.Bounds.Min.X, xlineData.Bounds.Min.Y, xlineData.Bounds.Min.Z);
    const max = new THREE.Vector3(xlineData.Bounds.Max.X, xlineData.Bounds.Max.Y, xlineData.Bounds.Max.Z);
    return new THREE.Box3(min, max);
  }

  public static getNormal(xlineData: XLineData): THREE.Vector3 {
    return new THREE.Vector3(xlineData.Normal.X, xlineData.Normal.Y, xlineData.Normal.Z);
  }

  public static getColor(xlineData: XLineData): THREE.Color {
    return new THREE.Color(xlineData.Material.Color);
  }

  public static clearCache(): void {
    this.xlineCache.forEach((line) => {
      line.geometry.dispose();
      (line.material as THREE.Material).dispose();
    });
    this.xlineCache.clear();
  }

  private static createXLineGeometry(xlineData: XLineData): THREE.BufferGeometry {
    const geometry = new THREE.BufferGeometry();

    if (xlineData.VertexPositions && xlineData.VertexPositions.length > 0) {
      const positions = new Float32Array(xlineData.VertexPositions);
      geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    } else {
      const positions = new Float32Array([0, 0, 0, 0, 0, 0]);
      geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    }

    if (xlineData.VertexColors && xlineData.VertexColors.length > 0) {
      const colors = new Float32Array(xlineData.VertexColors);
      geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3));
    }

    if (xlineData.Indices && xlineData.Indices.length > 0) {
      geometry.setIndex(new THREE.BufferAttribute(new Uint16Array(xlineData.Indices), 1));
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static createXLineMaterial(xlineData: XLineData): THREE.LineBasicMaterial {
    const material = new THREE.LineBasicMaterial({
      color: xlineData.Material.Color,
      opacity: xlineData.Opacity,
      transparent: xlineData.Transparent,
      linewidth: xlineData.Material.LineWidth,
      depthTest: true,
      depthWrite: false
    });

    return material;
  }

  public static renderMultiple(xlineDataArray: XLineData[], scene: THREE.Scene): THREE.Group {
    const group = new THREE.Group();
    group.name = 'MultipleXLines';

    xlineDataArray.forEach((xlineData) => {
      const xlineGroup = this.render(xlineData, scene);
      group.add(xlineGroup);
    });

    scene.add(group);

    return group;
  }

  public static disposeMultiple(group: THREE.Group, scene: THREE.Scene): void {
    if (!group) {
      return;
    }

    scene.remove(group);
    group.children.forEach((child) => {
      if (child instanceof THREE.Group) {
        child.children.forEach((line) => {
          if (line instanceof THREE.Line) {
            line.geometry.dispose();
            (line.material as THREE.Material).dispose();
            this.xlineCache.delete(line.userData.handle);
          }
        });
      }
    });
  }

  public static getTransform(xlineData: XLineData): any {
    if (!xlineData.Transform) {
      return {
        ScaleX: 1.0,
        ScaleY: 1.0,
        ScaleZ: 1.0,
        RotationX: 0,
        RotationY: 0,
        RotationZ: xlineData.Angle,
        TranslateX: xlineData.Center.X,
        TranslateY: xlineData.Center.Y,
        TranslateZ: xlineData.Center.Z
      };
    }
    return {
      ScaleX: xlineData.Transform.Scale.X,
      ScaleY: xlineData.Transform.Scale.Y,
      ScaleZ: xlineData.Transform.Scale.Z,
      RotationX: xlineData.Transform.Rotation.X,
      RotationY: xlineData.Transform.Rotation.Y,
      RotationZ: xlineData.Transform.Rotation.Z,
      TranslateX: xlineData.Transform.Position.X,
      TranslateY: xlineData.Transform.Position.Y,
      TranslateZ: xlineData.Transform.Position.Z
    };
  }

  public static getGeometry(xlineData: XLineData): GeometryData {
    return xlineData.Geometry;
  }

  public static getMaterial(xlineData: XLineData): MaterialData {
    return xlineData.Material;
  }

  public static getHandle(xlineData: XLineData): string {
    return xlineData.Handle || '';
  }

  public static getVisible(xlineData: XLineData): boolean {
    return xlineData.Visible !== false;
  }

  public static getOpacity(xlineData: XLineData): number {
    return xlineData.Opacity !== undefined ? xlineData.Opacity : 1.0;
  }

  public static getTransparent(xlineData: XLineData): boolean {
    return xlineData.Transparent !== false;
  }

  public static getLineTypeName(xlineData: XLineData): string {
    return xlineData.LineTypeName || '';
  }

  public static getLineWeight(xlineData: XLineData): number {
    return xlineData.LineWeight;
  }

  public static getColorIndex(xlineData: XLineData): number {
    return xlineData.ColorIndex;
  }
}
