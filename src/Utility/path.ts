import * as THREE from 'three';
import { PathGeometry } from 'three/examples/jsm/geometries/PathGeometry.js';
import { PathPointList } from 'three/examples/jsm/geometries/PathPointList.js';

export class PathGenerator {
  private scene: THREE.Scene;
  private renderer: THREE.WebGLRenderer;
  private pathMesh: THREE.Mesh | null = null;
  private pathGeometry: PathGeometry | null = null;
  private pathMaterial: THREE.Material | null = null;
  private pathTexture: THREE.Texture | null = null;
  private pathPointList: PathPointList | null = null;
  private points: THREE.Vector3[] = [];
  private animationEnabled: boolean = false;
  private animationSpeed: number = 1;

  private _pathWidth: number;
  private _pathColor: number;
  private _pathOpacity: number;
  private _depthTest: boolean;
  private _depthWrite: boolean;
  private _transparent: boolean;
  private _side: THREE.Side;
  private _blending: THREE.Blending;
  private _arrow: boolean;
  private _progress: number;

  constructor(params: {
    scene: THREE.Scene;
    renderer: THREE.WebGLRenderer;
    pathWidth?: number;
    pathColor?: number;
    pathOpacity?: number;
    depthTest?: boolean;
    depthWrite?: boolean;
    transparent?: boolean;
    side?: THREE.Side;
    blending?: THREE.Blending;
    arrow?: boolean;
    progress?: number;
    animationSpeed?: number;
  }) {
    this.scene = params.scene;
    this.renderer = params.renderer;

    this._pathWidth = params.pathWidth !== undefined ? params.pathWidth : 0.5;
    this._pathColor = params.pathColor !== undefined ? params.pathColor : 0x00ff00;
    this._pathOpacity = params.pathOpacity !== undefined ? params.pathOpacity : 0.9;
    this._depthTest = params.depthTest !== undefined ? params.depthTest : false;
    this._depthWrite = params.depthWrite !== undefined ? params.depthWrite : true;
    this._transparent = params.transparent !== undefined ? params.transparent : true;
    this._side = params.side !== undefined ? params.side : THREE.DoubleSide;
    this._blending = params.blending !== undefined ? params.blending : THREE.NormalBlending;
    this._arrow = params.arrow !== undefined ? params.arrow : false;
    this._progress = params.progress !== undefined ? params.progress : 1;
    this.animationSpeed = params.animationSpeed !== undefined ? params.animationSpeed : 1;
  }

  createPath(points: THREE.Vector3[]): void {
    this.points = points;

    if (points.length < 2) {
      console.warn('Path requires at least 2 points');
      return;
    }

    this.pathPointList = new PathPointList();
    points.forEach((point) => {
      this.pathPointList!.push(point);
    });

    this.pathGeometry = new PathGeometry();
    this.pathGeometry!.update(this.pathPointList, {
      width: this._pathWidth,
      arrow: this._arrow,
      progress: this._progress,
    });

    this.pathMaterial = new THREE.MeshPhongMaterial({
      color: this._pathColor,
      depthTest: this._depthTest,
      depthWrite: this._depthWrite,
      transparent: this._transparent,
      side: this._side,
      opacity: this._pathOpacity,
      blending: this._blending,
    });

    this.pathMesh = new THREE.Mesh(this.pathGeometry, this.pathMaterial);
    this.scene.add(this.pathMesh);
  }

  deletePath(): void {
    if (this.pathMesh) {
      this.scene.remove(this.pathMesh);
      this.pathMesh.geometry.dispose();
      if (Array.isArray(this.pathMesh.material)) {
        this.pathMesh.material.forEach((mat) => mat.dispose());
      } else {
        this.pathMesh.material.dispose();
      }
      this.pathMesh = null;
    }
    this.pathGeometry = null;
    this.pathMaterial = null;
    this.pathPointList = null;
    this.points = [];
  }

  exportPath(): string {
    if (this.points.length === 0) {
      return '';
    }

    const pathString = this.points
      .map((point) => `${point.x},${point.y},${point.z}`)
      .join(';');
    return pathString;
  }

  exportPoints(): string {
    if (this.points.length === 0) {
      return '';
    }

    const pointsString = this.points
      .map((point) => `Point(${point.x}, ${point.y}, ${point.z})`)
      .join('\n');
    return pointsString;
  }

  enableAnimation(): void {
    this.animationEnabled = true;
  }

  disableAnimation(): void {
    this.animationEnabled = false;
  }

  update(delta: number): void {
    if (!this.animationEnabled || !this.pathTexture) {
      return;
    }

    this.pathTexture.offset.x -= delta * 0.02 * this.animationSpeed;
    this.pathTexture.repeat.x = 1 / 24;
  }

  setPathWidth(width: number): void {
    this._pathWidth = width;
    if (this.pathGeometry && this.pathPointList) {
      this.pathGeometry.update(this.pathPointList, {
        width: this._pathWidth,
        arrow: this._arrow,
        progress: this._progress,
      });
    }
  }

  setPathColor(color: number): void {
    this._pathColor = color;
    if (this.pathMaterial) {
      (this.pathMaterial as THREE.MeshPhongMaterial).color.setHex(color);
    }
  }

  setPathOpacity(opacity: number): void {
    this._pathOpacity = opacity;
    if (this.pathMaterial) {
      (this.pathMaterial as THREE.MeshPhongMaterial).opacity = opacity;
    }
  }

  setAnimationSpeed(speed: number): void {
    this.animationSpeed = speed;
  }

  getMesh(): THREE.Mesh | null {
    return this.pathMesh;
  }

  getPoints(): THREE.Vector3[] {
    return this.points;
  }

  dispose(): void {
    this.deletePath();
  }
}

export function initPath(points: THREE.Vector3[], renderer: THREE.WebGLRenderer, scene: THREE.Scene) {
  const pathGenerator = new PathGenerator({
    scene,
    renderer,
  });
  pathGenerator.createPath(points);
  return pathGenerator;
}

export function deletePath(pathGenerator: PathGenerator) {
  pathGenerator.deletePath();
}

export function exportPath(pathGenerator: PathGenerator) {
  return pathGenerator.exportPath();
}

export function exportPoints(pathGenerator: PathGenerator) {
  return pathGenerator.exportPoints();
}

export function animate(pathGenerator: PathGenerator) {
  pathGenerator.enableAnimation();
}

export function updateAnimation(pathGenerator: PathGenerator, delta: number) {
  pathGenerator.update(delta);
}