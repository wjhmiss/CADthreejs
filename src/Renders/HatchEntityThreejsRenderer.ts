import * as THREE from 'three';

export interface PointData {
  X: number;
  Y: number;
}

export interface Point3DData {
  X: number;
  Y: number;
  Z: number;
}

export interface BoundsData {
  Min: Point3DData;
  Max: Point3DData;
}

export interface TransformData {
  Position: Point3DData;
  Rotation: Point3DData;
  Scale: Point3DData;
  Matrix: number[];
}

export interface PatternLine {
  Angle: number;
  BasePoint: PointData;
  Offset: PointData;
  DashLengths: number[];
}

export interface PatternData {
  Name: string;
  Lines: PatternLine[];
}

export interface LineEdgeData {
  Start: PointData;
  End: PointData;
  Length: number;
}

export interface ArcEdgeData {
  Center: PointData;
  Radius: number;
  StartAngle: number;
  EndAngle: number;
  IsCounterClockwise: boolean;
  Length: number;
  IsCCW: boolean;
}

export interface EdgeData {
  Type: string;
  LineEdge?: LineEdgeData;
  ArcEdge?: ArcEdgeData;
}

export interface PathData {
  Points: PointData[];
  Edges: EdgeData[];
  IsClosed: boolean;
  PathArea: number;
  PathCentroid: PointData;
}

export interface HatchData {
  Type: string;
  Uuid: string;
  EntityType: string;
  IsSolid: boolean;
  Pattern: PatternData;
  Paths: PathData[];
  ColorIndex: number;
  LineTypeName: string;
  LineWeight: number;
  Handle: string;
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
  Area: number;
  Centroid: PointData;
  Centroid3D: Point3DData;
  Bounds: BoundsData;
  NormalX: number;
  NormalY: number;
  NormalZ: number;
  Points: Point3DData[];
  Vertices: number[];
  Indices: number[];
  Normals: number[];
  PatternName: string;
  PatternAngle: number;
  PatternScale: number;
  PatternType: string;
  IsDouble: boolean;
  HasGradient: boolean;
  GradientColorName: string;
  PathCount: number;
  TotalEdges: number;
  EdgeTypes: string[];
  Elevation: number;
  CoordinateSystem: string;
  RequiresYAxisFlip: boolean;
  Transform: TransformData;
  Parent: any;
  Visible: boolean;
  CastShadow: boolean;
  ReceiveShadow: boolean;
  RenderOrder: number;
}

export class HatchEntityThreejsRenderer {
  public static render(hatchData: HatchData, scene: THREE.Scene): THREE.Mesh | null {
    if (!hatchData || !hatchData.Visible || hatchData.IsInvisible) {
      return null;
    }

    if (!hatchData.Vertices || hatchData.Vertices.length === 0) {
      return null;
    }

    const geometry = this.createGeometry(hatchData);
    if (!geometry) {
      return null;
    }

    const material = this.createMaterial(hatchData);
    const mesh = new THREE.Mesh(geometry, material);

    mesh.visible = hatchData.Visible;
    mesh.castShadow = hatchData.CastShadow;
    mesh.receiveShadow = hatchData.ReceiveShadow;
    mesh.renderOrder = hatchData.RenderOrder;

    mesh.userData = {
      type: hatchData.Type,
      uuid: hatchData.Uuid,
      entityType: hatchData.EntityType,
      handle: hatchData.Handle,
      layerName: hatchData.LayerName,
      colorIndex: hatchData.ColorIndex,
      isSolid: hatchData.IsSolid,
      patternName: hatchData.PatternName,
      patternAngle: hatchData.PatternAngle,
      patternScale: hatchData.PatternScale,
      hasGradient: hatchData.HasGradient,
      gradientColorName: hatchData.GradientColorName,
      isDouble: hatchData.IsDouble,
      area: hatchData.Area,
      pathCount: hatchData.PathCount,
      elevation: hatchData.Elevation
    };

    if (hatchData.Transform && hatchData.Transform.Matrix) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(hatchData.Transform.Matrix);
      mesh.applyMatrix4(matrix);
    }

    scene.add(mesh);
    return mesh;
  }

  private static createGeometry(hatchData: HatchData): THREE.BufferGeometry | null {
    if (!hatchData.Vertices || hatchData.Vertices.length === 0) {
      return null;
    }

    const geometry = new THREE.BufferGeometry();

    const vertices = new Float32Array(hatchData.Vertices);
    geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));

    if (hatchData.Normals && hatchData.Normals.length > 0) {
      const normals = new Float32Array(hatchData.Normals);
      geometry.setAttribute('normal', new THREE.BufferAttribute(normals, 3));
    }

    if (hatchData.Indices && hatchData.Indices.length > 0) {
      geometry.setIndex(hatchData.Indices);
    }

    geometry.computeBoundingSphere();
    geometry.computeBoundingBox();

    return geometry;
  }

  private static createMaterial(hatchData: HatchData): THREE.Material {
    const materialOptions: any = {
      side: hatchData.MaterialSide === 2 ? THREE.DoubleSide : THREE.FrontSide,
      depthTest: hatchData.MaterialDepthTest,
      depthWrite: hatchData.MaterialDepthWrite,
      transparent: hatchData.MaterialTransparent,
      opacity: hatchData.MaterialOpacity
    };

    if (hatchData.IsSolid) {
      materialOptions.color = new THREE.Color(hatchData.ColorHex);
      return new THREE.MeshBasicMaterial(materialOptions);
    } else {
      materialOptions.color = new THREE.Color(hatchData.ColorHex);
      materialOptions.shininess = 30;
      materialOptions.specular = new THREE.Color(0x444444);
      return new THREE.MeshPhongMaterial(materialOptions);
    }
  }

  public static createPatternTexture(hatchData: HatchData): THREE.Texture | null {
    if (!hatchData.Pattern || !hatchData.Pattern.Lines || hatchData.Pattern.Lines.length === 0) {
      return null;
    }

    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');
    if (!ctx) {
      return null;
    }

    const size = 512;
    canvas.width = size;
    canvas.height = size;

    ctx.fillStyle = '#ffffff';
    ctx.fillRect(0, 0, size, size);

    ctx.strokeStyle = '#000000';
    ctx.lineWidth = 2;

    hatchData.Pattern.Lines.forEach(line => {
      const angle = line.Angle;
      const baseX = line.BasePoint.X;
      const baseY = line.BasePoint.Y;
      const offsetX = line.Offset.X;
      const offsetY = line.Offset.Y;

      ctx.save();
      ctx.translate(baseX, baseY);
      ctx.rotate(angle);

      ctx.beginPath();
      ctx.moveTo(-size, 0);
      ctx.lineTo(size, 0);
      ctx.stroke();

      if (line.DashLengths && line.DashLengths.length > 0) {
        ctx.setLineDash(line.DashLengths);
        ctx.stroke();
      }

      ctx.restore();
    });

    const texture = new THREE.CanvasTexture(canvas);
    texture.wrapS = THREE.RepeatWrapping;
    texture.wrapT = THREE.RepeatWrapping;
    texture.repeat.set(hatchData.PatternScale, hatchData.PatternScale);
    texture.rotation = hatchData.PatternAngle;

    return texture;
  }

  public static renderPatternHatch(hatchData: HatchData, scene: THREE.Scene): THREE.Mesh | null {
    if (!hatchData || !hatchData.Visible || hatchData.IsInvisible) {
      return null;
    }

    if (hatchData.IsSolid) {
      return this.render(hatchData, scene);
    }

    const geometry = this.createGeometry(hatchData);
    if (!geometry) {
      return null;
    }

    const texture = this.createPatternTexture(hatchData);
    const materialOptions: any = {
      side: hatchData.MaterialSide === 2 ? THREE.DoubleSide : THREE.FrontSide,
      depthTest: hatchData.MaterialDepthTest,
      depthWrite: hatchData.MaterialDepthWrite,
      transparent: hatchData.MaterialTransparent,
      opacity: hatchData.MaterialOpacity,
      color: new THREE.Color(hatchData.ColorHex)
    };

    if (texture) {
      materialOptions.map = texture;
    }

    const material = new THREE.MeshPhongMaterial(materialOptions);
    const mesh = new THREE.Mesh(geometry, material);

    mesh.visible = hatchData.Visible;
    mesh.castShadow = hatchData.CastShadow;
    mesh.receiveShadow = hatchData.ReceiveShadow;
    mesh.renderOrder = hatchData.RenderOrder;

    mesh.userData = {
      type: hatchData.Type,
      uuid: hatchData.Uuid,
      entityType: hatchData.EntityType,
      handle: hatchData.Handle,
      layerName: hatchData.LayerName,
      colorIndex: hatchData.ColorIndex,
      isSolid: hatchData.IsSolid,
      patternName: hatchData.PatternName,
      area: hatchData.Area,
      pathCount: hatchData.PathCount
    };

    if (hatchData.Transform && hatchData.Transform.Matrix) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(hatchData.Transform.Matrix);
      mesh.applyMatrix4(matrix);
    }

    scene.add(mesh);
    return mesh;
  }

  public static renderGradientHatch(hatchData: HatchData, scene: THREE.Scene): THREE.Mesh | null {
    if (!hatchData || !hatchData.Visible || hatchData.IsInvisible) {
      return null;
    }

    if (!hatchData.HasGradient) {
      return this.render(hatchData, scene);
    }

    const geometry = this.createGeometry(hatchData);
    if (!geometry) {
      return null;
    }

    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');
    if (!ctx) {
      return null;
    }

    const size = 512;
    canvas.width = size;
    canvas.height = size;

    const gradient = ctx.createLinearGradient(0, 0, size, size);
    gradient.addColorStop(0, hatchData.ColorHex);
    gradient.addColorStop(1, '#ffffff');

    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, size, size);

    const texture = new THREE.CanvasTexture(canvas);

    const materialOptions: any = {
      side: hatchData.MaterialSide === 2 ? THREE.DoubleSide : THREE.FrontSide,
      depthTest: hatchData.MaterialDepthTest,
      depthWrite: hatchData.MaterialDepthWrite,
      transparent: hatchData.MaterialTransparent,
      opacity: hatchData.MaterialOpacity,
      map: texture
    };

    const material = new THREE.MeshPhongMaterial(materialOptions);
    const mesh = new THREE.Mesh(geometry, material);

    mesh.visible = hatchData.Visible;
    mesh.castShadow = hatchData.CastShadow;
    mesh.receiveShadow = hatchData.ReceiveShadow;
    mesh.renderOrder = hatchData.RenderOrder;

    mesh.userData = {
      type: hatchData.Type,
      uuid: hatchData.Uuid,
      entityType: hatchData.EntityType,
      handle: hatchData.Handle,
      layerName: hatchData.LayerName,
      colorIndex: hatchData.ColorIndex,
      isSolid: hatchData.IsSolid,
      patternName: hatchData.PatternName,
      area: hatchData.Area,
      pathCount: hatchData.PathCount,
      hasGradient: hatchData.HasGradient,
      gradientColorName: hatchData.GradientColorName
    };

    if (hatchData.Transform && hatchData.Transform.Matrix) {
      const matrix = new THREE.Matrix4();
      matrix.fromArray(hatchData.Transform.Matrix);
      mesh.applyMatrix4(matrix);
    }

    scene.add(mesh);
    return mesh;
  }

  public static renderBoundaryEdges(hatchData: HatchData, scene: THREE.Scene): THREE.LineSegments | null {
    if (!hatchData || !hatchData.Paths || hatchData.Paths.length === 0) {
      return null;
    }

    const vertices: number[] = [];

    hatchData.Paths.forEach(path => {
      if (!path.Edges || path.Edges.length === 0) {
        return;
      }

      path.Edges.forEach(edge => {
        if (edge.Type === 'Line' && edge.LineEdge) {
          vertices.push(
            edge.LineEdge.Start.X, edge.LineEdge.Start.Y, hatchData.Elevation,
            edge.LineEdge.End.X, edge.LineEdge.End.Y, hatchData.Elevation
          );
        } else if (edge.Type === 'Arc' && edge.ArcEdge) {
          const arc = edge.ArcEdge;
          const segments = 32;
          const startAngle = arc.StartAngle;
          const endAngle = arc.EndAngle;
          const angleStep = (endAngle - startAngle) / segments;

          for (let i = 0; i < segments; i++) {
            const angle1 = startAngle + angleStep * i;
            const angle2 = startAngle + angleStep * (i + 1);
            const x1 = arc.Center.X + arc.Radius * Math.cos(angle1);
            const y1 = arc.Center.Y + arc.Radius * Math.sin(angle1);
            const x2 = arc.Center.X + arc.Radius * Math.cos(angle2);
            const y2 = arc.Center.Y + arc.Radius * Math.sin(angle2);

            vertices.push(x1, y1, hatchData.Elevation);
            vertices.push(x2, y2, hatchData.Elevation);
          }
        }
      });
    });

    if (vertices.length === 0) {
      return null;
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));

    const material = new THREE.LineBasicMaterial({
      color: new THREE.Color(hatchData.ColorHex),
      linewidth: 1
    });

    const lineSegments = new THREE.LineSegments(geometry, material);

    lineSegments.userData = {
      type: 'HatchBoundary',
      uuid: hatchData.Uuid,
      handle: hatchData.Handle,
      layerName: hatchData.LayerName
    };

    scene.add(lineSegments);
    return lineSegments;
  }

  public static dispose(mesh: THREE.Mesh): void {
    if (mesh.geometry) {
      mesh.geometry.dispose();
    }

    if (mesh.material) {
      if (Array.isArray(mesh.material)) {
        mesh.material.forEach(m => m.dispose());
      } else {
        mesh.material.dispose();
      }
    }
  }
}
