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
  DimensionDatas?: any[];
  DimensionLinearDatas?: any[];
  DimensionAngular3PtDatas?: any[];
  DimensionAngular2LineDatas?: any[];
  DimensionRadiusDatas?: any[];
  DimensionDiameterDatas?: any[];
  DimensionOrdinateDatas?: any[];
  DimensionAlignedDatas?: any[];
}

export interface CenteringOptions {
  centerX: boolean;
  centerY: boolean;
  centerZ: boolean;
  applyTransform: boolean;
}

export interface TransformInfo {
  originalCenter: THREE.Vector3;
  originalSize: THREE.Vector3;
  newCenter: THREE.Vector3;
  newOffset: THREE.Vector3;
  appliedOffset: THREE.Vector3;
  isCentered: boolean;
}

export class RenderManager {
  private scene: THREE.Scene;
  private renderedObjects: Map<string, THREE.Object3D[]> = new Map();
  private entityCount: number = 0;
  private boundingBox: THREE.Box3;
  private originalBoundingBox: THREE.Box3;
  private scale: number = 1;
  private rotation: { x: number; y: number; z: number } = { x: 0, y: 0, z: 0 };
  private centerOffset: THREE.Vector3 = new THREE.Vector3(0, 0, 0);
  private isCentered: boolean = false;
  private centeringOptions: CenteringOptions = {
    centerX: true,
    centerY: true,
    centerZ: true,
    applyTransform: true
  };
  private dxfObjectColor: string = '#ffffff';

  constructor(scene: THREE.Scene) {
    this.scene = scene;
    this.boundingBox = new THREE.Box3();
    this.originalBoundingBox = new THREE.Box3();
  }

  public renderDxfData(dxfData: DxfParseData): void {
    console.log('RenderManager.renderDxfData called with:', dxfData);
    this.clearAll();
    this.boundingBox.makeEmpty();
    this.originalBoundingBox.makeEmpty();
    this.isCentered = false;

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
    
    this.originalBoundingBox.copy(this.boundingBox);
    console.log('RenderManager: Original bounding box:', this.originalBoundingBox);
    
    if (this.centeringOptions.applyTransform) {
      this.centerRenderedObjects();
    }

    this.setDxfObjectColor(this.dxfObjectColor);
    
    return this.getBoundingBox();
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
    
    this.clearTransformableObjects();
    console.log('RenderManager: All entities cleared');
  }

  private clearTransformableObjects(): void {
    const objectsToRemove: THREE.Object3D[] = [];
    
    this.scene.traverse((obj) => {
      if (obj.userData && obj.userData.isTransformable === true) {
        objectsToRemove.push(obj);
      }
    });
    
    objectsToRemove.forEach((obj) => {
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
    
    console.log(`RenderManager: Cleared ${objectsToRemove.length} transformable objects`);
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
      console.warn('RenderManager: No entities to center');
      return;
    }

    const center = new THREE.Vector3();
    this.boundingBox.getCenter(center);

    console.log(`RenderManager: Original center: (${center.x.toFixed(2)}, ${center.y.toFixed(2)}, ${center.z.toFixed(2)})`);

    const offset = new THREE.Vector3(0, 0, 0);
    if (this.centeringOptions.centerX) {
      offset.x = -center.x;
    }
    if (this.centeringOptions.centerY) {
      offset.y = -center.y;
    }
    if (this.centeringOptions.centerZ) {
      offset.z = -center.z;
    }

    this.centerOffset.copy(offset);

    this.renderedObjects.forEach((objects) => {
      objects.forEach((obj) => {
        obj.position.add(offset);
      });
    });

    this.isCentered = true;

    this.boundingBox.translate(offset);

    console.log(`RenderManager: Objects centered with offset: (${offset.x.toFixed(2)}, ${offset.y.toFixed(2)}, ${offset.z.toFixed(2)})`);
    console.log(`RenderManager: New bounding box center: (${this.boundingBox.getCenter(new THREE.Vector3()).x.toFixed(2)}, ${this.boundingBox.getCenter(new THREE.Vector3()).y.toFixed(2)}, ${this.boundingBox.getCenter(new THREE.Vector3()).z.toFixed(2)})`);
    console.log(`RenderManager: Centering options: X=${this.centeringOptions.centerX}, Y=${this.centeringOptions.centerY}, Z=${this.centeringOptions.centerZ}`);
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
        obj.position.multiplyScalar(scaleRatio);
      });
    });

    this.scale = scale;
    
    this.boundingBox.min.multiplyScalar(scaleRatio);
    this.boundingBox.max.multiplyScalar(scaleRatio);
    
    console.log(`RenderManager: Objects scaled to ${scale} around scene center`);
    console.log(`RenderManager: Bounding box after scaling:`, this.boundingBox);
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
        const originalPosition = obj.position.clone();
        
        obj.rotation.x += rotationDiff.x;
        obj.rotation.y += rotationDiff.y;
        obj.rotation.z += rotationDiff.z;
        
        obj.position.applyAxisAngle(new THREE.Vector3(1, 0, 0), rotationDiff.x);
        obj.position.applyAxisAngle(new THREE.Vector3(0, 1, 0), rotationDiff.y);
        obj.position.applyAxisAngle(new THREE.Vector3(0, 0, 1), rotationDiff.z);
      });
    });
    this.rotation = { x, y, z };
    console.log(`RenderManager: Objects rotation set to (${x}, ${y}, ${z}) around origin`);
  }

  public getFlipRotation(): { x: number; y: number; z: number } {
    return this.rotation;
  }

  public dispose(): void {
    this.clearAll();
  }

  public getBoundingBox(): THREE.Box3 {
    return this.boundingBox.clone();
  }

  public getOriginalBoundingBox(): THREE.Box3 {
    return this.originalBoundingBox.clone();
  }

  public getCenterOffset(): THREE.Vector3 {
    return this.centerOffset.clone();
  }

  public isObjectCentered(): boolean {
    return this.isCentered;
  }

  public setCenteringOptions(options: Partial<CenteringOptions>): void {
    this.centeringOptions = { ...this.centeringOptions, ...options };
    console.log('RenderManager: Centering options updated:', this.centeringOptions);
  }

  public getCenteringOptions(): CenteringOptions {
    return { ...this.centeringOptions };
  }

  public getTransformInfo(): TransformInfo {
    const originalCenter = new THREE.Vector3();
    const originalSize = new THREE.Vector3();
    const newCenter = new THREE.Vector3();

    this.originalBoundingBox.getCenter(originalCenter);
    this.originalBoundingBox.getSize(originalSize);
    this.boundingBox.getCenter(newCenter);

    return {
      originalCenter: originalCenter.clone(),
      originalSize: originalSize.clone(),
      newCenter: newCenter.clone(),
      newOffset: new THREE.Vector3(
        this.centeringOptions.centerX ? -originalCenter.x : 0,
        this.centeringOptions.centerY ? -originalCenter.y : 0,
        this.centeringOptions.centerZ ? -originalCenter.z : 0
      ),
      appliedOffset: this.centerOffset.clone(),
      isCentered: this.isCentered
    };
  }

  public printTransformInfo(): void {
    const info = this.getTransformInfo();
    console.log('========== 坐标转换信息 ==========');
    console.log('原始边界框中心:', `(${info.originalCenter.x.toFixed(2)}, ${info.originalCenter.y.toFixed(2)}, ${info.originalCenter.z.toFixed(2)})`);
    console.log('原始边界框尺寸:', `(${info.originalSize.x.toFixed(2)}, ${info.originalSize.y.toFixed(2)}, ${info.originalSize.z.toFixed(2)})`);
    console.log('新边界框中心:', `(${info.newCenter.x.toFixed(2)}, ${info.newCenter.y.toFixed(2)}, ${info.newCenter.z.toFixed(2)})`);
    console.log('计算偏移量:', `(${info.newOffset.x.toFixed(2)}, ${info.newOffset.y.toFixed(2)}, ${info.newOffset.z.toFixed(2)})`);
    console.log('应用偏移量:', `(${info.appliedOffset.x.toFixed(2)}, ${info.appliedOffset.y.toFixed(2)}, ${info.appliedOffset.z.toFixed(2)})`);
    console.log('是否已中心化:', info.isCentered);
    console.log('中心化选项:', `X=${this.centeringOptions.centerX}, Y=${this.centeringOptions.centerY}, Z=${this.centeringOptions.centerZ}`);
    console.log('==================================');
  }

  public resetToOriginalPosition(): void {
    if (!this.isCentered) {
      console.warn('RenderManager: Objects are not centered, no reset needed');
      return;
    }

    const reverseOffset = this.centerOffset.clone().negate();

    this.renderedObjects.forEach((objects) => {
      objects.forEach((obj) => {
        obj.position.add(reverseOffset);
      });
    });

    this.isCentered = false;
    console.log(`RenderManager: Objects reset to original position with offset: (${reverseOffset.x.toFixed(2)}, ${reverseOffset.y.toFixed(2)}, ${reverseOffset.z.toFixed(2)})`);
  }

  public applyCentering(): void {
    if (this.isCentered) {
      console.warn('RenderManager: Objects are already centered');
      return;
    }

    this.centerRenderedObjects();
  }

  public verifyCentering(): boolean {
    if (this.entityCount === 0) {
      console.warn('RenderManager: No entities to verify');
      return false;
    }

    const currentCenter = new THREE.Vector3();
    this.boundingBox.getCenter(currentCenter);

    const tolerance = 0.001;
    const isXCentered = Math.abs(currentCenter.x) < tolerance;
    const isYCentered = Math.abs(currentCenter.y) < tolerance;
    const isZCentered = Math.abs(currentCenter.z) < tolerance;

    const expectedXCentered = this.centeringOptions.centerX;
    const expectedYCentered = this.centeringOptions.centerY;
    const expectedZCentered = this.centeringOptions.centerZ;

    const isValid = (isXCentered === expectedXCentered) && 
                    (isYCentered === expectedYCentered) && 
                    (isZCentered === expectedZCentered);

    console.log('========== 中心化验证 ==========');
    console.log('当前中心:', `(${currentCenter.x.toFixed(6)}, ${currentCenter.y.toFixed(6)}, ${currentCenter.z.toFixed(6)})`);
    console.log('X轴中心化:', isXCentered, '(期望:', expectedXCentered, ')');
    console.log('Y轴中心化:', isYCentered, '(期望:', expectedYCentered, ')');
    console.log('Z轴中心化:', isZCentered, '(期望:', expectedZCentered, ')');
    console.log('验证结果:', isValid ? '通过' : '失败');
    console.log('===============================');

    return isValid;
  }

  public setDxfObjectColor(color: string): void {
    this.dxfObjectColor = color;
    console.log(`RenderManager: Setting DXF object color to ${color}`);

    this.renderedObjects.forEach((objects) => {
      objects.forEach((obj) => {
        if (obj instanceof THREE.Line || obj instanceof THREE.Mesh) {
          if (obj.material) {
            if (Array.isArray(obj.material)) {
              obj.material.forEach(material => {
                if (material instanceof THREE.LineBasicMaterial || 
                    material instanceof THREE.MeshBasicMaterial ||
                    material instanceof THREE.MeshLambertMaterial ||
                    material instanceof THREE.MeshPhongMaterial) {
                  material.color.set(color);
                }
              });
            } else {
              if (obj.material instanceof THREE.LineBasicMaterial || 
                  obj.material instanceof THREE.MeshBasicMaterial ||
                  obj.material instanceof THREE.MeshLambertMaterial ||
                  obj.material instanceof THREE.MeshPhongMaterial) {
                obj.material.color.set(color);
              }
            }
          }
        }
      });
    });

    console.log(`RenderManager: Updated color for ${this.entityCount} entities`);
  }

  public getDxfObjectColor(): string {
    return this.dxfObjectColor;
  }
}
