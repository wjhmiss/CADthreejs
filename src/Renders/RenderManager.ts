import * as THREE from 'three';
import { LineEntityThreejsRenderer } from './LineEntityThreejsRenderer';
import { ArcEntityThreejsRenderer } from './ArcEntityThreejsRenderer';
import { CircleEntityThreejsRenderer } from './CircleEntityThreejsRenderer';
import { EllipseEntityThreejsRenderer } from './EllipseEntityThreejsRenderer';
import { LwPolylineEntityThreejsRenderer } from './LwPolylineEntityThreejsRenderer';
import { Polyline2DEntityThreejsRenderer } from './Polyline2DEntityThreejsRenderer';
import { Polyline3DEntityThreejsRenderer } from './Polyline3DEntityThreejsRenderer';
import { PointEntityThreejsRenderer } from './PointEntityThreejsRenderer';
import { TextEntityThreejsRenderer } from './TextEntityThreejsRenderer';
import { MTextEntityThreejsRenderer } from './MTextEntityThreejsRenderer';
import { SolidEntityThreejsRenderer } from './SolidEntityThreejsRenderer';
import { Face3DEntityThreejsRenderer } from './Face3DEntityThreejsRenderer';
import { PolyfaceMeshEntityThreejsRenderer } from './PolyfaceMeshEntityThreejsRenderer';
import { PolygonMeshEntityThreejsRenderer } from './PolygonMeshEntityThreejsRenderer';
import { SplineEntityThreejsRenderer } from './SplineEntityThreejsRenderer';
import { RayEntityThreejsRenderer } from './RayEntityThreejsRenderer';
import { XLineEntityThreejsRenderer } from './XLineEntityThreejsRenderer';
import { ShapeEntityThreejsRenderer } from './ShapeEntityThreejsRenderer';
import { RasterImageEntityThreejsRenderer } from './RasterImageEntityThreejsRenderer';
import { PdfUnderlayEntityThreejsRenderer } from './PdfUnderlayEntityThreejsRenderer';
import { WipeoutEntityThreejsRenderer } from './WipeoutEntityThreejsRenderer';
import { MeshEntityThreejsRenderer } from './MeshEntityThreejsRenderer';
import { MLineEntityThreejsRenderer } from './MLineEntityThreejsRenderer';
import { LeaderEntityThreejsRenderer } from './LeaderEntityThreejsRenderer';
import { HatchEntityThreejsRenderer } from './HatchEntityThreejsRenderer';
import { InsertEntityThreejsRenderer } from './InsertEntityThreejsRenderer';
import { DimensionEntityThreejsRenderer } from './DimensionEntityThreejsRenderer';

export interface DxfParseData {
  LineDatas?: any[];
  ArcDatas?: any[];
  CircleDatas?: any[];
  EllipseDatas?: any[];
  LwPolylineDatas?: any[];
  Polyline2DDatas?: any[];
  Polyline3DDatas?: any[];
  PointDatas?: any[];
  TextDatas?: any[];
  MTextDatas?: any[];
  SolidDatas?: any[];
  Face3DDatas?: any[];
  PolyfaceMeshDatas?: any[];
  PolygonMeshDatas?: any[];
  SplineDatas?: any[];
  RayDatas?: any[];
  XLineDatas?: any[];
  ShapeDatas?: any[];
  RasterImageDatas?: any[];
  PdfUnderlayDatas?: any[];
  WipeoutDatas?: any[];
  MeshDatas?: any[];
  MLineDatas?: any[];
  LeaderDatas?: any[];
  HatchDatas?: any[];
  InsertDatas?: any[];
  DimensionDatas?: any[];
  DimensionLinearDatas?: any[];
  DimensionAngular3PtDatas?: any[];
  DimensionAngular2LineDatas?: any[];
  DimensionRadiusDatas?: any[];
  DimensionDiameterDatas?: any[];
  DimensionOrdinateDatas?: any[];
  DimensionAlignedDatas?: any[];
}

export class RenderManager {
  private scene: THREE.Scene;
  private renderedObjects: Map<string, THREE.Group> = new Map();
  private entityCount: number = 0;
  private dxfGroup: THREE.Group;
  private boundingBox: THREE.Box3;

  constructor(scene: THREE.Scene) {
    this.scene = scene;
    this.dxfGroup = new THREE.Group();
    this.dxfGroup.name = 'DXF_Group';
    this.boundingBox = new THREE.Box3();
    this.scene.add(this.dxfGroup);
  }

  public renderDxfData(dxfData: DxfParseData): void {
    console.log('RenderManager.renderDxfData called with:', dxfData);
    this.clearAll();
    this.boundingBox.makeEmpty();

    if (dxfData.LineDatas && dxfData.LineDatas.length > 0) {
      this.renderEntities(dxfData.LineDatas, 'Line', LineEntityThreejsRenderer);
    }

    if (dxfData.ArcDatas && dxfData.ArcDatas.length > 0) {
      this.renderEntities(dxfData.ArcDatas, 'Arc', ArcEntityThreejsRenderer);
    }

    if (dxfData.CircleDatas && dxfData.CircleDatas.length > 0) {
      this.renderEntities(dxfData.CircleDatas, 'Circle', CircleEntityThreejsRenderer);
    }

    if (dxfData.EllipseDatas && dxfData.EllipseDatas.length > 0) {
      this.renderEntities(dxfData.EllipseDatas, 'Ellipse', EllipseEntityThreejsRenderer);
    }

    if (dxfData.LwPolylineDatas && dxfData.LwPolylineDatas.length > 0) {
      this.renderEntities(dxfData.LwPolylineDatas, 'LwPolyline', LwPolylineEntityThreejsRenderer);
    }

    if (dxfData.Polyline2DDatas && dxfData.Polyline2DDatas.length > 0) {
      this.renderEntities(dxfData.Polyline2DDatas, 'Polyline2D', Polyline2DEntityThreejsRenderer);
    }

    if (dxfData.Polyline3DDatas && dxfData.Polyline3DDatas.length > 0) {
      this.renderEntities(dxfData.Polyline3DDatas, 'Polyline3D', Polyline3DEntityThreejsRenderer);
    }

    if (dxfData.PointDatas && dxfData.PointDatas.length > 0) {
      this.renderEntities(dxfData.PointDatas, 'Point', PointEntityThreejsRenderer);
    }

    if (dxfData.TextDatas && dxfData.TextDatas.length > 0) {
      this.renderEntities(dxfData.TextDatas, 'Text', TextEntityThreejsRenderer);
    }

    if (dxfData.MTextDatas && dxfData.MTextDatas.length > 0) {
      this.renderEntities(dxfData.MTextDatas, 'MText', MTextEntityThreejsRenderer);
    }

    if (dxfData.SolidDatas && dxfData.SolidDatas.length > 0) {
      this.renderEntities(dxfData.SolidDatas, 'Solid', SolidEntityThreejsRenderer);
    }

    if (dxfData.Face3DDatas && dxfData.Face3DDatas.length > 0) {
      this.renderEntities(dxfData.Face3DDatas, 'Face3D', Face3DEntityThreejsRenderer);
    }

    if (dxfData.PolyfaceMeshDatas && dxfData.PolyfaceMeshDatas.length > 0) {
      this.renderEntities(dxfData.PolyfaceMeshDatas, 'PolyfaceMesh', PolyfaceMeshEntityThreejsRenderer);
    }

    if (dxfData.PolygonMeshDatas && dxfData.PolygonMeshDatas.length > 0) {
      this.renderEntities(dxfData.PolygonMeshDatas, 'PolygonMesh', PolygonMeshEntityThreejsRenderer);
    }

    if (dxfData.SplineDatas && dxfData.SplineDatas.length > 0) {
      this.renderEntities(dxfData.SplineDatas, 'Spline', SplineEntityThreejsRenderer);
    }

    if (dxfData.RayDatas && dxfData.RayDatas.length > 0) {
      this.renderEntities(dxfData.RayDatas, 'Ray', RayEntityThreejsRenderer);
    }

    if (dxfData.XLineDatas && dxfData.XLineDatas.length > 0) {
      this.renderEntities(dxfData.XLineDatas, 'XLine', XLineEntityThreejsRenderer);
    }

    if (dxfData.ShapeDatas && dxfData.ShapeDatas.length > 0) {
      this.renderEntities(dxfData.ShapeDatas, 'Shape', ShapeEntityThreejsRenderer);
    }

    if (dxfData.RasterImageDatas && dxfData.RasterImageDatas.length > 0) {
      this.renderEntities(dxfData.RasterImageDatas, 'RasterImage', RasterImageEntityThreejsRenderer);
    }

    if (dxfData.PdfUnderlayDatas && dxfData.PdfUnderlayDatas.length > 0) {
      this.renderEntities(dxfData.PdfUnderlayDatas, 'PdfUnderlay', PdfUnderlayEntityThreejsRenderer);
    }

    if (dxfData.WipeoutDatas && dxfData.WipeoutDatas.length > 0) {
      this.renderEntities(dxfData.WipeoutDatas, 'Wipeout', WipeoutEntityThreejsRenderer);
    }

    if (dxfData.MeshDatas && dxfData.MeshDatas.length > 0) {
      this.renderEntities(dxfData.MeshDatas, 'Mesh', MeshEntityThreejsRenderer);
    }

    if (dxfData.MLineDatas && dxfData.MLineDatas.length > 0) {
      this.renderEntities(dxfData.MLineDatas, 'MLine', MLineEntityThreejsRenderer);
    }

    if (dxfData.LeaderDatas && dxfData.LeaderDatas.length > 0) {
      this.renderEntities(dxfData.LeaderDatas, 'Leader', LeaderEntityThreejsRenderer);
    }

    if (dxfData.HatchDatas && dxfData.HatchDatas.length > 0) {
      this.renderEntities(dxfData.HatchDatas, 'Hatch', HatchEntityThreejsRenderer);
    }

    if (dxfData.InsertDatas && dxfData.InsertDatas.length > 0) {
      this.renderEntities(dxfData.InsertDatas, 'Insert', InsertEntityThreejsRenderer);
    }

    if (dxfData.DimensionDatas && dxfData.DimensionDatas.length > 0) {
      this.renderEntities(dxfData.DimensionDatas, 'Dimension', DimensionEntityThreejsRenderer);
    }

    if (dxfData.DimensionLinearDatas && dxfData.DimensionLinearDatas.length > 0) {
      this.renderEntities(dxfData.DimensionLinearDatas, 'DimensionLinear', DimensionEntityThreejsRenderer);
    }

    if (dxfData.DimensionAngular3PtDatas && dxfData.DimensionAngular3PtDatas.length > 0) {
      this.renderEntities(dxfData.DimensionAngular3PtDatas, 'DimensionAngular3Pt', DimensionEntityThreejsRenderer);
    }

    if (dxfData.DimensionAngular2LineDatas && dxfData.DimensionAngular2LineDatas.length > 0) {
      this.renderEntities(dxfData.DimensionAngular2LineDatas, 'DimensionAngular2Line', DimensionEntityThreejsRenderer);
    }

    if (dxfData.DimensionRadiusDatas && dxfData.DimensionRadiusDatas.length > 0) {
      this.renderEntities(dxfData.DimensionRadiusDatas, 'DimensionRadius', DimensionEntityThreejsRenderer);
    }

    if (dxfData.DimensionDiameterDatas && dxfData.DimensionDiameterDatas.length > 0) {
      this.renderEntities(dxfData.DimensionDiameterDatas, 'DimensionDiameter', DimensionEntityThreejsRenderer);
    }

    if (dxfData.DimensionOrdinateDatas && dxfData.DimensionOrdinateDatas.length > 0) {
      this.renderEntities(dxfData.DimensionOrdinateDatas, 'DimensionOrdinate', DimensionEntityThreejsRenderer);
    }

    if (dxfData.DimensionAlignedDatas && dxfData.DimensionAlignedDatas.length > 0) {
      this.renderEntities(dxfData.DimensionAlignedDatas, 'DimensionAligned', DimensionEntityThreejsRenderer);
    }

    console.log(`RenderManager: Total entities rendered: ${this.entityCount}`);
    console.log(`RenderManager: DXF group children count: ${this.dxfGroup.children.length}`);
    this.centerDxfGroup();
  }

  private renderEntities<T>(
    entities: T[],
    entityType: string,
    renderer: any
  ): void {
    console.log(`RenderManager: Rendering ${entities.length} ${entityType} entities`);

    entities.forEach((entityData) => {
      try {
        const group = renderer.render(entityData, this.scene);
        if (group) {
          const key = `${entityType}_${entityData.Handle}`;
          this.renderedObjects.set(key, group);
          
          group.traverse((child) => {
            if (child instanceof THREE.Mesh || child instanceof THREE.Line) {
              this.boundingBox.expandByObject(child);
            }
          });
          
          this.dxfGroup.add(group);
          this.entityCount++;
        }
      } catch (error) {
        console.error(`RenderManager: Error rendering ${entityType} entity:`, error);
      }
    });
  }

  public clearAll(): void {
    this.renderedObjects.forEach((group, key) => {
      this.dxfGroup.remove(group);
      this.disposeGroup(group);
    });
    this.renderedObjects.clear();
    this.entityCount = 0;
    this.boundingBox.makeEmpty();
    this.dxfGroup.position.set(0, 0, 0);
    this.dxfGroup.scale.set(1, 1, 1);
    console.log('RenderManager: All entities cleared');
  }

  private disposeGroup(group: THREE.Group): void {
    group.traverse((object) => {
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
    });
    group.clear();
  }

  public getRenderedObjects(): Map<string, THREE.Group> {
    return this.renderedObjects;
  }

  public getEntityCount(): number {
    return this.entityCount;
  }

  public getObjectsForRaycasting(): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];
    this.renderedObjects.forEach((group) => {
      group.traverse((child) => {
        if (child instanceof THREE.Mesh || child instanceof THREE.Line) {
          objects.push(child);
        }
      });
    });
    return objects;
  }

  private centerDxfGroup(): void {
    if (this.entityCount === 0) {
      return;
    }

    const center = new THREE.Vector3();
    this.boundingBox.getCenter(center);

    this.dxfGroup.position.x = -center.x;
    this.dxfGroup.position.z = -center.z;
    this.dxfGroup.position.y = 0;

    console.log(`RenderManager: DXF group centered at (${this.dxfGroup.position.x}, ${this.dxfGroup.position.y}, ${this.dxfGroup.position.z})`);
  }

  public setScale(scale: number): void {
    if (this.entityCount === 0) {
      console.warn('RenderManager: No entities to scale');
      return;
    }

    this.dxfGroup.scale.set(scale, scale, scale);
    console.log(`RenderManager: DXF group scaled to ${scale}`);
  }

  public getDxfGroup(): THREE.Group {
    return this.dxfGroup;
  }

  public getCurrentScale(): number {
    return this.dxfGroup.scale.x;
  }

  public dispose(): void {
    this.clearAll();
    if (this.dxfGroup) {
      this.scene.remove(this.dxfGroup);
    }
  }
}
