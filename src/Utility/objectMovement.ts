import * as THREE from 'three'
import gsap from 'gsap'
import { pathManager, type PathMesh } from './path'

export interface ObjectMovementConfig {
  speed: number
  loops: number
  facingDirection?: 'none' | 'front' | 'back' | 'left' | 'right'
}

export interface ObjectMovementState {
  pathId: string | null
  speed: number
  loops: number
  currentLoop: number
  isMoving: boolean
  tween: gsap.core.Tween | null
  facingDirection?: 'none' | 'front' | 'back' | 'left' | 'right'
}

class ObjectMovementManager {
  private scene: THREE.Scene | null = null
  private movements: Map<string, ObjectMovementState> = new Map()
  private pathCache: Map<string, THREE.Vector3[]> = new Map()

  setScene(scene: THREE.Scene): void {
    this.scene = scene
  }

  isObjectInPath(objectId: string): boolean {
    const paths = pathManager.getAllPaths()
    for (const path of paths) {
      if (path.objectIds.includes(objectId)) {
        return true
      }
    }
    return false
  }

  getAvailablePaths(objectId: string): PathMesh[] {
    if (this.isObjectInPath(objectId)) {
      return []
    }
    return pathManager.getAllPaths()
  }

  startMovement(object: THREE.Object3D, pathId: string, config: ObjectMovementConfig): boolean {
    if (this.isObjectInPath(object.uuid)) {
      console.warn('[ObjectMovementManager] 对象在路径中，不能移动')
      return false
    }

    const pathMesh = pathManager.getPathById(pathId)
    if (!pathMesh) {
      console.error('[ObjectMovementManager] 路径不存在:', pathId)
      return false
    }

    const pathPoints = this.getPathPoints(pathMesh)
    if (pathPoints.length < 2) {
      console.error('[ObjectMovementManager] 路径点不足')
      return false
    }

    const existingState = this.movements.get(object.uuid)
    if (existingState && existingState.isMoving) {
      this.stopMovement(object.uuid)
    }

    const state: ObjectMovementState = {
      pathId,
      speed: config.speed,
      loops: config.loops,
      currentLoop: 0,
      isMoving: true,
      tween: null,
      facingDirection: config.facingDirection || 'none'
    }

    this.movements.set(object.uuid, state)

    const duration = this.calculateDuration(pathPoints, config.speed, config.loops)
    const progress = { value: 0 }

    state.tween = gsap.to(progress, {
      value: config.loops,
      duration: duration,
      ease: 'none',
      onUpdate: () => {
        const normalizedProgress = progress.value % 1
        const position = this.getPositionOnPath(pathPoints, normalizedProgress)
        object.position.copy(position)
        
        if (state.facingDirection && state.facingDirection !== 'none') {
          this.updateObjectFacing(object, pathPoints, normalizedProgress, state.facingDirection)
        }
      },
      onComplete: () => {
        state.isMoving = false
        state.tween = null
        console.log('[ObjectMovementManager] 对象移动完成:', object.name)
      }
    })

    console.log('[ObjectMovementManager] 开始移动对象:', object.name, '路径:', pathId, '速度:', config.speed, '圈数:', config.loops)
    return true
  }

  stopMovement(objectId: string): void {
    const state = this.movements.get(objectId)
    if (state && state.tween) {
      state.tween.kill()
      state.tween = null
      state.isMoving = false
      console.log('[ObjectMovementManager] 停止移动对象:', objectId)
    }
  }

  updateMovement(objectId: string, config: Partial<ObjectMovementConfig>): boolean {
    const state = this.movements.get(objectId)
    if (!state) {
      return false
    }

    if (config.speed !== undefined) {
      state.speed = config.speed
    }

    if (config.loops !== undefined) {
      state.loops = config.loops
    }

    if (config.facingDirection !== undefined) {
      state.facingDirection = config.facingDirection
    }

    if (state.isMoving && state.pathId) {
      const object = this.findObjectById(objectId)
      if (object) {
        this.stopMovement(objectId)
        this.startMovement(object, state.pathId, {
          speed: state.speed,
          loops: state.loops,
          facingDirection: state.facingDirection
        })
      }
    }

    return true
  }

  getMovementState(objectId: string): ObjectMovementState | undefined {
    return this.movements.get(objectId)
  }

  private getPathPoints(pathMesh: PathMesh): THREE.Vector3[] {
    if (this.pathCache.has(pathMesh.id)) {
      return this.pathCache.get(pathMesh.id)!
    }

    const points: THREE.Vector3[] = []
    const pathPointList = pathMesh.pathPointList

    for (let i = 0; i < pathPointList.count; i++) {
      const point = pathPointList.array[i].pos
      points.push(point.clone())
    }

    this.pathCache.set(pathMesh.id, points)
    return points
  }

  private calculateDuration(pathPoints: THREE.Vector3[], speed: number, loops: number): number {
    let totalDistance = 0
    for (let i = 1; i < pathPoints.length; i++) {
      totalDistance += pathPoints[i].distanceTo(pathPoints[i - 1])
    }
    return (totalDistance / speed) * loops
  }

  private getPositionOnPath(pathPoints: THREE.Vector3[], progress: number): THREE.Vector3 {
    const totalSegments = pathPoints.length - 1
    const segmentProgress = progress * totalSegments
    const segmentIndex = Math.floor(segmentProgress)
    const localProgress = segmentProgress - segmentIndex

    if (segmentIndex >= totalSegments) {
      return pathPoints[pathPoints.length - 1].clone()
    }

    const startPoint = pathPoints[segmentIndex]
    const endPoint = pathPoints[segmentIndex + 1]

    return new THREE.Vector3(
      startPoint.x + (endPoint.x - startPoint.x) * localProgress,
      startPoint.y + (endPoint.y - startPoint.y) * localProgress,
      startPoint.z + (endPoint.z - startPoint.z) * localProgress
    )
  }

  private findObjectById(objectId: string): THREE.Object3D | null {
    if (!this.scene) {
      return null
    }

    let foundObject: THREE.Object3D | null = null
    this.scene.traverse((child) => {
      if (child.uuid === objectId) {
        foundObject = child
      }
    })

    return foundObject
  }

  private updateObjectFacing(object: THREE.Object3D, pathPoints: THREE.Vector3[], progress: number, facingDirection: 'front' | 'back' | 'left' | 'right'): void {
    const totalSegments = pathPoints.length - 1
    const segmentProgress = progress * totalSegments
    const segmentIndex = Math.floor(segmentProgress)
    const localProgress = segmentProgress - segmentIndex

    let currentPoint: THREE.Vector3
    let nextPoint: THREE.Vector3

    if (segmentIndex >= totalSegments - 1) {
      currentPoint = pathPoints[pathPoints.length - 2]
      nextPoint = pathPoints[pathPoints.length - 1]
    } else {
      currentPoint = pathPoints[segmentIndex]
      nextPoint = pathPoints[segmentIndex + 1]
    }

    const direction = new THREE.Vector3().subVectors(nextPoint, currentPoint).normalize()
    const up = new THREE.Vector3(0, 1, 0)
    const quaternion = new THREE.Quaternion()

    const forward = new THREE.Vector3(0, 0, -1)
    const back = new THREE.Vector3(0, 0, 1)
    const left = new THREE.Vector3(-1, 0, 0)
    const right = new THREE.Vector3(1, 0, 0)

    let targetDirection: THREE.Vector3

    switch (facingDirection) {
      case 'front':
        targetDirection = forward
        break
      case 'back':
        targetDirection = back
        break
      case 'left':
        targetDirection = left
        break
      case 'right':
        targetDirection = right
        break
      default:
        targetDirection = forward
    }

    quaternion.setFromUnitVectors(targetDirection, direction)
    object.quaternion.copy(quaternion)
  }

  clearPathCache(): void {
    this.pathCache.clear()
  }

  clearAllMovements(): void {
    this.movements.forEach((state, objectId) => {
      this.stopMovement(objectId)
    })
    this.movements.clear()
  }
}

export const objectMovementManager = new ObjectMovementManager()
