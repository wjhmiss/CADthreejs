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

export interface GeometryData {
  Type: string;
  VertexCount: number;
  FaceCount: number;
  HasNormals: boolean;
  HasColors: boolean;
  HasUVs: boolean;
  HasIndices: boolean;
  PrimitiveType: string;
  IndexCount: number;
  IsClosed: boolean;
  IsPeriodic: boolean;
}

export interface MaterialData {
  Type: string;
  Color: number;
  Opacity: number;
  Transparent: boolean;
  Wireframe: boolean;
  LineWidth: number;
  VertexColors: boolean;
  Side: boolean;
}

export enum TextHorizontalAlignment {
  Left = 0,
  Center = 1,
  Right = 2
}

export enum TextVerticalAlignmentType {
  Baseline = 0,
  Bottom = 1,
  Middle = 2,
  Top = 3
}

export enum TextMirrorFlag {
  None = 0,
  MirrorX = 2,
  MirrorY = 4
}

export interface TextData {
  Value: string;
  InsertPoint: Point3DData;
  AlignmentPoint: Point3DData;
  Height: number;
  Rotation: number;
  ObliqueAngle: number;
  WidthFactor: number;
  HorizontalAlignment: TextHorizontalAlignment;
  VerticalAlignment: TextVerticalAlignmentType;
  Mirror: TextMirrorFlag;
  Normal: Point3DData;
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  Thickness: number;
  StyleName: string;
  BoundaryPoints: Point3DData[];
  Bounds: BoundsData;
  Centroid: Point3DData;
  Width: number;
  Area: number;
  Transform: TransformData;
  Geometry: GeometryData;
  Material: MaterialData;
  VertexPositions: number[];
  VertexColors: number[];
  Indices: number[];
  VertexCount: number;
  FontFamily: string;
  FontSize: number;
  TextAlignment: string;
  Opacity: number;
  Transparent: boolean;
  Color: ColorData;
  Tangent: NormalData;
  Binormal: number[];
  UV: number[];
  IsMirrored: boolean;
  IsUpsideDown: boolean;
  TextLength: number;
  Ascent: number;
  Descent: number;
  CharacterCount: number;
  FontStyle: string;
  IsBold: boolean;
  IsItalic: boolean;
  Type: string;
  Handle: string;
  Visible: boolean;
  LayerIndex: number;
  LayerName: string;
  EntityType: string;
  DepthTest: boolean;
}

export class TextEntityThreejsRenderer {
  private static readonly DEFAULT_FONT_SIZE = 12.0;
  private static readonly DEFAULT_OPACITY = 1.0;
  private static readonly DEFAULT_COLOR = '#FFFFFF';
  private static readonly textCache = new Map<string, THREE.Mesh>();

  public static render(textData: TextData, scene: THREE.Scene): THREE.Group | null {
    if (!textData || !textData.Visible) {
      return null;
    }

    const group = new THREE.Group();
    group.name = `Text_${textData.Handle}`;
    group.visible = textData.Visible;

    const geometry = this.createTextGeometry(textData);
    const material = this.createTextMaterial(textData);
    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = 'Text';
    mesh.userData = { handle: textData.Handle, text: textData.Value };
    mesh.visible = textData.Visible;

    group.add(mesh);

    this.textCache.set(textData.Handle, mesh);

    return group;
  }

  public static update(textData: TextData, scene: THREE.Scene): boolean {
    const existingMesh = this.textCache.get(textData.Handle);
    if (!existingMesh) {
      return false;
    }

    const material = existingMesh.material as THREE.MeshBasicMaterial;
    if (material) {
      material.color.setHex(parseInt(textData.Color.Hex.replace('#', ''), 16));
      material.opacity = textData.Opacity;
      material.transparent = textData.Transparent;
    }

    const geometry = existingMesh.geometry as THREE.BufferGeometry;
    if (geometry && textData.VertexPositions && textData.VertexPositions.length > 0) {
      const positions = geometry.attributes.position.array as Float32Array;
      for (let i = 0; i < textData.VertexPositions.length && i < positions.length; i++) {
        positions[i] = textData.VertexPositions[i];
      }
      geometry.attributes.position.needsUpdate = true;
    }

    return true;
  }

  public static dispose(textData: TextData, scene: THREE.Scene): boolean {
    const existingMesh = this.textCache.get(textData.Handle);
    if (!existingMesh) {
      return false;
    }

    const group = existingMesh.parent;
    if (group) {
      scene.remove(group);
    }

    existingMesh.geometry.dispose();
    (existingMesh.material as THREE.Material).dispose();
    this.textCache.delete(textData.Handle);

    return true;
  }

  public static setVisibility(textData: TextData, scene: THREE.Scene, visible: boolean): boolean {
    const existingMesh = this.textCache.get(textData.Handle);
    if (!existingMesh) {
      return false;
    }

    const group = existingMesh.parent;
    if (group) {
      group.visible = visible;
    }
    existingMesh.visible = visible;
    return true;
  }

  public static setOpacity(textData: TextData, opacity: number): boolean {
    const existingMesh = this.textCache.get(textData.Handle);
    if (!existingMesh) {
      return false;
    }

    const material = existingMesh.material as THREE.MeshBasicMaterial;
    if (material) {
      material.opacity = opacity;
      material.transparent = opacity < 1.0;
    }

    return true;
  }

  public static setText(textData: TextData, text: string): boolean {
    const existingMesh = this.textCache.get(textData.Handle);
    if (!existingMesh) {
      return false;
    }

    existingMesh.userData.text = text;

    return true;
  }

  public static setFontSize(textData: TextData, fontSize: number): boolean {
    const existingMesh = this.textCache.get(textData.Handle);
    if (!existingMesh) {
      return false;
    }

    const group = existingMesh.parent;
    if (group) {
      const scale = fontSize / textData.FontSize;
      group.scale.set(scale, scale, scale);
    }

    return true;
  }

  public static getTextMesh(textData: TextData): THREE.Mesh | null {
    return this.textCache.get(textData.Handle) || null;
  }

  public static getText(textData: TextData): string {
    return textData.Value || '';
  }

  public static getInsertPoint(textData: TextData): THREE.Vector3 {
    return new THREE.Vector3(textData.InsertPoint.X, textData.InsertPoint.Y, textData.InsertPoint.Z);
  }

  public static getAlignmentPoint(textData: TextData): THREE.Vector3 {
    return new THREE.Vector3(textData.AlignmentPoint.X, textData.AlignmentPoint.Y, textData.AlignmentPoint.Z);
  }

  public static getHeight(textData: TextData): number {
    return textData.Height || this.DEFAULT_FONT_SIZE;
  }

  public static getRotation(textData: TextData): number {
    return textData.Rotation || 0;
  }

  public static getObliqueAngle(textData: TextData): number {
    return textData.ObliqueAngle || 0;
  }

  public static getWidthFactor(textData: TextData): number {
    return textData.WidthFactor || 1.0;
  }

  public static getHorizontalAlignment(textData: TextData): TextHorizontalAlignment {
    return textData.HorizontalAlignment || TextHorizontalAlignment.Left;
  }

  public static getVerticalAlignment(textData: TextData): TextVerticalAlignmentType {
    return textData.VerticalAlignment || TextVerticalAlignmentType.Baseline;
  }

  public static isMirrored(textData: TextData): boolean {
    return textData.IsMirrored || false;
  }

  public static isUpsideDown(textData: TextData): boolean {
    return textData.IsUpsideDown || false;
  }

  public static getColor(textData: TextData): THREE.Color {
    return new THREE.Color(textData.Color.Hex);
  }

  public static getBounds(textData: TextData): THREE.Box3 {
    const min = new THREE.Vector3(textData.Bounds.Min.X, textData.Bounds.Min.Y, textData.Bounds.Min.Z);
    const max = new THREE.Vector3(textData.Bounds.Max.X, textData.Bounds.Max.Y, textData.Bounds.Max.Z);
    return new THREE.Box3(min, max);
  }

  public static getCentroid(textData: TextData): THREE.Vector3 {
    return new THREE.Vector3(textData.Centroid.X, textData.Centroid.Y, textData.Centroid.Z);
  }

  public static getWidth(textData: TextData): number {
    return textData.Width || 0;
  }

  public static getArea(textData: TextData): number {
    return textData.Area || 0;
  }

  public static clearCache(): void {
    this.textCache.forEach((mesh) => {
      mesh.geometry.dispose();
      (mesh.material as THREE.Material).dispose();
    });
    this.textCache.clear();
  }

  private static createTextGeometry(textData: TextData): THREE.BufferGeometry {
    const geometry = new THREE.BufferGeometry();

    if (textData.VertexPositions && textData.VertexPositions.length > 0) {
      const positions = new Float32Array(textData.VertexPositions);
      geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    } else {
      const halfWidth = textData.Width / 2;
      const halfHeight = textData.Height / 2;
      const positions = new Float32Array([
        textData.InsertPoint.X - halfWidth, textData.InsertPoint.Y - halfHeight, textData.InsertPoint.Z,
        textData.InsertPoint.X + halfWidth, textData.InsertPoint.Y - halfHeight, textData.InsertPoint.Z,
        textData.InsertPoint.X + halfWidth, textData.InsertPoint.Y + halfHeight, textData.InsertPoint.Z,
        textData.InsertPoint.X - halfWidth, textData.InsertPoint.Y + halfHeight, textData.InsertPoint.Z
      ]);
      geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    }

    if (textData.VertexColors && textData.VertexColors.length > 0) {
      const colors = new Float32Array(textData.VertexColors.map(c => c / 255));
      geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3));
    }

    if (textData.Indices && textData.Indices.length > 0) {
      geometry.setIndex(new THREE.BufferAttribute(new Uint16Array(textData.Indices), 1));
    }

    if (textData.UV && textData.UV.length > 0) {
      geometry.setAttribute('uv', new THREE.BufferAttribute(new Float32Array(textData.UV), 2));
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static createTextMaterial(textData: TextData): THREE.MeshBasicMaterial {
    const material = new THREE.MeshBasicMaterial({
      color: textData.Color.Hex,
      opacity: textData.Opacity,
      transparent: textData.Transparent,
      side: THREE.DoubleSide,
      depthTest: textData.DepthTest !== false,
      wireframe: textData.Material.Wireframe
    });

    return material;
  }

  public static renderMultiple(textDataArray: TextData[], scene: THREE.Scene): THREE.Group {
    const group = new THREE.Group();
    group.name = 'MultipleTexts';

    textDataArray.forEach((textData) => {
      const textGroup = this.render(textData, scene);
      group.add(textGroup);
    });

    return group;
  }

  public static disposeMultiple(group: THREE.Group, scene: THREE.Scene): void {
    if (!group) {
      return;
    }

    scene.remove(group);
    group.children.forEach((child) => {
      if (child instanceof THREE.Group) {
        const mesh = child.children[0] as THREE.Mesh;
        if (mesh) {
          mesh.geometry.dispose();
          (mesh.material as THREE.Material).dispose();
        }
      }
    });
  }

  public static getNormal(textData: TextData): NormalData {
    if (!textData.Normal || (textData.Normal.X === 0 && textData.Normal.Y === 0 && textData.Normal.Z === 0)) {
      return { X: 0, Y: 0, Z: 1 };
    }
    return textData.Normal;
  }

  public static getTangent(textData: TextData): NormalData {
    if (!textData.Tangent || (textData.Tangent.X === 0 && textData.Tangent.Y === 0 && textData.Tangent.Z === 0)) {
      return { X: 1, Y: 0, Z: 0 };
    }
    return textData.Tangent;
  }

  public static getBinormal(textData: TextData): number[] {
    if (!textData.Binormal || textData.Binormal.length === 0) {
      return [0, 0, 1];
    }
    return textData.Binormal;
  }

  public static getTransform(textData: TextData): TransformData {
    if (!textData.Transform) {
      return {
        Position: textData.Centroid || { X: textData.InsertPoint.X, Y: textData.InsertPoint.Y, Z: textData.InsertPoint.Z },
        Rotation: { X: 0, Y: 0, Z: textData.Rotation || 0 },
        Scale: { X: 1, Y: 1, Z: 1 },
        Matrix: [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]
      };
    }
    return textData.Transform;
  }

  public static getGeometry(textData: TextData): GeometryData {
    if (!textData.Geometry) {
      return {
        Type: 'TextGeometry',
        VertexCount: textData.VertexCount || 4,
        FaceCount: 2,
        HasNormals: true,
        HasColors: true,
        HasUVs: true,
        HasIndices: true,
        PrimitiveType: 'Triangles',
        IndexCount: 6,
        IsClosed: false,
        IsPeriodic: false
      };
    }
    return textData.Geometry;
  }

  public static getMaterial(textData: TextData): MaterialData {
    if (!textData.Material) {
      return {
        Type: 'MeshBasicMaterial',
        Color: parseInt(textData.Color.Hex.replace('#', ''), 16),
        Opacity: textData.Opacity || this.DEFAULT_OPACITY,
        Transparent: textData.Transparent || false,
        Wireframe: false,
        LineWidth: 1.0,
        VertexColors: true,
        Side: true
      };
    }
    return textData.Material;
  }

  public static getDepthTest(textData: TextData): boolean {
    return textData.DepthTest !== false;
  }

  public static getHandle(textData: TextData): string {
    return textData.Handle || '';
  }

  public static getLayerName(textData: TextData): string {
    return textData.LayerName || '';
  }

  public static getVisible(textData: TextData): boolean {
    return textData.Visible !== false;
  }

  public static getOpacity(textData: TextData): number {
    return textData.Opacity !== undefined ? textData.Opacity : this.DEFAULT_OPACITY;
  }

  public static getTransparent(textData: TextData): boolean {
    return textData.Transparent !== false;
  }

  public static getFontFamily(textData: TextData): string {
    return textData.FontFamily || 'Arial';
  }

  public static getFontSize(textData: TextData): number {
    return textData.FontSize || this.DEFAULT_FONT_SIZE;
  }

  public static getTextAlignment(textData: TextData): string {
    return textData.TextAlignment || 'Left-Baseline';
  }

  public static getStyleName(textData: TextData): string {
    return textData.StyleName || '';
  }

  public static getThickness(textData: TextData): number {
    return textData.Thickness || 0;
  }

  public static getLineWeight(textData: TextData): number {
    return textData.LineWeight || 0;
  }

  public static getLineTypeName(textData: TextData): string {
    return textData.LineTypeName || '';
  }

  public static getTextLength(textData: TextData): number {
    return textData.TextLength || 0;
  }

  public static getAscent(textData: TextData): number {
    return textData.Ascent || 0;
  }

  public static getDescent(textData: TextData): number {
    return textData.Descent || 0;
  }

  public static getCharacterCount(textData: TextData): number {
    return textData.CharacterCount || 0;
  }

  public static getFontStyle(textData: TextData): string {
    return textData.FontStyle || 'Regular';
  }

  public static isBold(textData: TextData): boolean {
    return textData.IsBold || false;
  }

  public static isItalic(textData: TextData): boolean {
    return textData.IsItalic || false;
  }
}
