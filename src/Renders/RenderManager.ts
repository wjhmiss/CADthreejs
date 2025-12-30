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
  private renderedObjects: Map<string, THREE.Object3D[]> = new Map();
  private entityCount: number = 0;
  private boundingBox: THREE.Box3;
  private scale: number = 1;
  private rotation: { x: number; y: number; z: number } = { x: 0, y: 0, z: 0 };
  private centerOffset: THREE.Vector3 = new THREE.Vector3(0, 0, 0);

  constructor(scene: THREE.Scene) {
    this.scene = scene;
    this.boundingBox = new THREE.Box3();
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
    console.log(`RenderManager: Total rendered objects: ${this.renderedObjects.size}`);
    this.centerRenderedObjects();
  }

  private renderEntities<T>(
    entities: T[],
    entityType: string,
    renderer: any
  ): void {
    console.log(`RenderManager: Rendering ${entities.length} ${entityType} entities`);

    entities.forEach((entityData) => {
      try {
        const result = renderer.render(entityData, this.scene);
        let objectsToAdd: THREE.Object3D[] = [];

        if (result) {
          if (Array.isArray(result)) {
            objectsToAdd = result.filter(obj => 
              obj instanceof THREE.Mesh || obj instanceof THREE.Line || obj instanceof THREE.Object3D
            );
          } else if (result instanceof THREE.Mesh || result instanceof THREE.Line || result instanceof THREE.Object3D) {
            objectsToAdd.push(result);
          }
        }

        if (objectsToAdd.length > 0) {
          const key = `${entityType}_${entityData.Handle}`;
          const objects: THREE.Object3D[] = [];

          objectsToAdd.forEach((obj) => {
            if (!obj.userData.entityType) {
              obj.userData = {
                ...obj.userData,
                entityType: entityType,
                handle: entityData.Handle,
                key: key
              };
            }
            
            this.boundingBox.expandByObject(obj);
            this.scene.add(obj);
            objects.push(obj);
          });
          
          this.renderedObjects.set(key, objects);
          this.entityCount++;
        }
      } catch (error) {
        console.error(`RenderManager: Error rendering ${entityType} entity:`, error);
      }
    });
  }

  public clearAll(): void {
    this.renderedObjects.forEach((objects) => {
      objects.forEach((obj) => {
        this.scene.remove(obj);
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
    });
    this.renderedObjects.clear();
    this.entityCount = 0;
    this.boundingBox.makeEmpty();
    this.centerOffset.set(0, 0, 0);
    this.scale = 1;
    this.rotation = { x: 0, y: 0, z: 0 };
    console.log('RenderManager: All entities cleared');
  }

  public getRenderedObjects(): Map<string, THREE.Object3D[]> {
    return this.renderedObjects;
  }

  public getEntityCount(): number {
    return this.entityCount;
  }

  public getObjectsForRaycasting(): THREE.Object3D[] {
    const objects: THREE.Object3D[] = [];
    this.renderedObjects.forEach((objArray) => {
      objArray.forEach((obj) => {
        if (obj instanceof THREE.Mesh || obj instanceof THREE.Line) {
          objects.push(obj);
        }
      });
    });
    return objects;
  }

  private centerRenderedObjects(): void {
    if (this.entityCount === 0) {
      return;
    }

    const center = new THREE.Vector3();
    this.boundingBox.getCenter(center);

    this.centerOffset.set(-center.x, 0, -center.z);

    this.renderedObjects.forEach((objects) => {
      objects.forEach((obj) => {
        obj.position.add(this.centerOffset);
      });
    });

    console.log(`RenderManager: Objects centered at (${this.centerOffset.x}, ${this.centerOffset.y}, ${this.centerOffset.z})`);
  }

  public setScale(scale: number): void {
    if (this.entityCount === 0) {
      console.warn('RenderManager: No entities to scale');
      return;
    }

    const scaleRatio = scale / this.scale;
    this.renderedObjects.forEach((objects) => {
      objects.forEach((obj) => {
        obj.scale.multiplyScalar(scaleRatio);
      });
    });
    this.scale = scale;
    console.log(`RenderManager: Objects scaled to ${scale}`);
  }

  public getCurrentScale(): number {
    return this.scale;
  }

  public setFlipX(enabled: boolean): void {
    if (this.entityCount === 0) {
      console.warn('RenderManager: No entities to flip');
      return;
    }
    const targetRotation = enabled ? Math.PI : 0;
    const rotationDiff = targetRotation - this.rotation.x;
    this.renderedObjects.forEach((objects) => {
      objects.forEach((obj) => {
        obj.rotation.x += rotationDiff;
      });
    });
    this.rotation.x = targetRotation;
    console.log(`RenderManager: Objects X flip ${enabled ? 'enabled' : 'disabled'}`);
  }

  public setFlipY(enabled: boolean): void {
    if (this.entityCount === 0) {
      console.warn('RenderManager: No entities to flip');
      return;
    }
    const targetRotation = enabled ? Math.PI : 0;
    const rotationDiff = targetRotation - this.rotation.y;
    this.renderedObjects.forEach((objects) => {
      objects.forEach((obj) => {
        obj.rotation.y += rotationDiff;
      });
    });
    this.rotation.y = targetRotation;
    console.log(`RenderManager: Objects Y flip ${enabled ? 'enabled' : 'disabled'}`);
  }

  public setFlipZ(enabled: boolean): void {
    if (this.entityCount === 0) {
      console.warn('RenderManager: No entities to flip');
      return;
    }
    const targetRotation = enabled ? Math.PI : 0;
    const rotationDiff = targetRotation - this.rotation.z;
    this.renderedObjects.forEach((objects) => {
      objects.forEach((obj) => {
        obj.rotation.z += rotationDiff;
      });
    });
    this.rotation.z = targetRotation;
    console.log(`RenderManager: Objects Z flip ${enabled ? 'enabled' : 'disabled'}`);
  }

  public setFlipRotation(x: number, y: number, z: number): void {
    if (this.entityCount === 0) {
      console.warn('RenderManager: No entities to rotate');
      return;
    }
    const rotationDiff = {
      x: x - this.rotation.x,
      y: y - this.rotation.y,
      z: z - this.rotation.z
    };
    this.renderedObjects.forEach((objects) => {
      objects.forEach((obj) => {
        obj.rotation.x += rotationDiff.x;
        obj.rotation.y += rotationDiff.y;
        obj.rotation.z += rotationDiff.z;
      });
    });
    this.rotation = { x, y, z };
    console.log(`RenderManager: Objects rotation set to (${x}, ${y}, ${z})`);
  }

  public getFlipRotation(): { x: number; y: number; z: number } {
    return this.rotation;
  }

  public dispose(): void {
    this.clearAll();
  }
}
