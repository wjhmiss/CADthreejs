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
  bottomOffset?: THREE.Vector3
  currentProgress?: number
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
    console.log('[ObjectMovementManager] startMovement() 被调用')
    console.log('[ObjectMovementManager] 对象ID:', object.uuid, '路径ID:', pathId)
    
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
      console.error('[ObjectMovementManager] 路径点不足，实际点数:', pathPoints.length)
      return false
    }

    const existingState = this.movements.get(object.uuid)
    console.log('[ObjectMovementManager] 现有状态:', existingState)
    
    let startProgress = 0
    if (existingState) {
      if (existingState.isMoving) {
        this.stopMovement(object.uuid)
      }
      if (existingState.currentProgress !== undefined) {
        startProgress = existingState.currentProgress
        console.log('[ObjectMovementManager] 从保存的进度继续:', startProgress)
      }
    }

    const bottomOffset = this.calculateBottomOffset(object)

    const state: ObjectMovementState = {
      pathId,
      speed: config.speed,
      loops: config.loops,
      currentLoop: 0,
      isMoving: true,
      tween: null,
      facingDirection: config.facingDirection || 'none',
      bottomOffset,
      currentProgress: startProgress
    }

    this.movements.set(object.uuid, state)

    const duration = this.calculateDuration(pathPoints, config.speed, config.loops)
    const progress = { value: startProgress }

    state.tween = gsap.to(progress, {
      value: config.loops,
      duration: duration,
      ease: 'none',
      onUpdate: () => {
        const normalizedProgress = progress.value % 1
        const position = this.getPositionOnPath(pathPoints, normalizedProgress)
        
        if (state.bottomOffset) {
          position.add(state.bottomOffset)
        }
        
        object.position.copy(position)
        
        if (state.facingDirection && state.facingDirection !== 'none') {
          this.updateObjectFacing(object, pathPoints, normalizedProgress, state.facingDirection)
        }
      },
      onComplete: () => {
        state.isMoving = false
        state.tween = null
        state.currentProgress = 0
        console.log('[ObjectMovementManager] 对象移动完成:', object.name)
      }
    })

    console.log('[ObjectMovementManager] 开始移动对象:', object.name, '路径:', pathId, '速度:', config.speed, '圈数:', config.loops, '起始进度:', startProgress)
    return true
  }

  stopMovement(objectId: string): void {
    console.log('[ObjectMovementManager] stopMovement() 被调用，对象ID:', objectId)
    const state = this.movements.get(objectId)
    console.log('[ObjectMovementManager] 找到的状态:', state)
    if (state) {
      if (state.tween) {
        state.currentProgress = (state.tween as any).targets()[0].value
        state.tween.kill()
        state.tween = null
      }
      state.isMoving = false
      console.log('[ObjectMovementManager] 停止移动对象:', objectId, '保存进度:', state.currentProgress)
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
    if (pathPoints.length < 2) {
      return pathPoints[0].clone()
    }

    const totalDistance = this.calculateTotalDistance(pathPoints)
    const targetDistance = progress * totalDistance

    let accumulatedDistance = 0
    for (let i = 0; i < pathPoints.length - 1; i++) {
      const segmentDistance = pathPoints[i + 1].distanceTo(pathPoints[i])
      
      if (accumulatedDistance + segmentDistance >= targetDistance) {
        const localProgress = (targetDistance - accumulatedDistance) / segmentDistance
        const startPoint = pathPoints[i]
        const endPoint = pathPoints[i + 1]

        return new THREE.Vector3(
          startPoint.x + (endPoint.x - startPoint.x) * localProgress,
          startPoint.y + (endPoint.y - startPoint.y) * localProgress,
          startPoint.z + (endPoint.z - startPoint.z) * localProgress
        )
      }
      
      accumulatedDistance += segmentDistance
    }

    return pathPoints[pathPoints.length - 1].clone()
  }

  private calculateTotalDistance(pathPoints: THREE.Vector3[]): number {
    let totalDistance = 0
    for (let i = 1; i < pathPoints.length; i++) {
      totalDistance += pathPoints[i].distanceTo(pathPoints[i - 1])
    }
    return totalDistance
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
    if (pathPoints.length < 2) {
      return
    }

    const totalDistance = this.calculateTotalDistance(pathPoints)
    const targetDistance = progress * totalDistance

    let accumulatedDistance = 0
    let currentPoint: THREE.Vector3
    let nextPoint: THREE.Vector3

    for (let i = 0; i < pathPoints.length - 1; i++) {
      const segmentDistance = pathPoints[i + 1].distanceTo(pathPoints[i])
      
      if (accumulatedDistance + segmentDistance >= targetDistance) {
        currentPoint = pathPoints[i]
        nextPoint = pathPoints[i + 1]
        break
      }
      
      accumulatedDistance += segmentDistance
    }

    if (!currentPoint || !nextPoint) {
      currentPoint = pathPoints[pathPoints.length - 2]
      nextPoint = pathPoints[pathPoints.length - 1]
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

  private calculateBottomOffset(object: THREE.Object3D): THREE.Vector3 {
    const box = new THREE.Box3().setFromObject(object)
    const center = new THREE.Vector3()
    box.getCenter(center)
    
    const bottom = new THREE.Vector3(center.x, box.min.y, center.z)
    
    const offset = new THREE.Vector3().subVectors(center, bottom)
    
    return offset
  }

  clearPathCache(): void {
    this.pathCache.clear()
  }

  clearPathCacheById(pathId: string): void {
    this.pathCache.delete(pathId)
  }

  refreshMovementByPathId(pathId: string): void {
    this.clearPathCacheById(pathId)
    
    this.movements.forEach((state, objectId) => {
      if (state.pathId === pathId && state.isMoving) {
        const object = this.findObjectById(objectId)
        if (object) {
          this.stopMovement(objectId)
          this.startMovement(object, pathId, {
            speed: state.speed,
            loops: state.loops,
            facingDirection: state.facingDirection
          })
        }
      }
    })
  }

  clearAllMovements(): void {
    this.movements.forEach((state, objectId) => {
      this.stopMovement(objectId)
    })
    this.movements.clear()
  }

  setMovementState(object: THREE.Object3D, pathId: string, config: ObjectMovementConfig, options: {
    currentProgress?: number
    currentPosition?: number[]
    isMoving?: boolean
  }): boolean {
    console.log('[ObjectMovementManager] setMovementState() 被调用')
    console.log('[ObjectMovementManager] 对象ID:', object.uuid, '路径ID:', pathId)
    
    const pathMesh = pathManager.getPathById(pathId)
    if (!pathMesh) {
      console.error('[ObjectMovementManager] 路径不存在:', pathId)
      return false
    }

    const pathPoints = this.getPathPoints(pathMesh)
    if (pathPoints.length < 2) {
      console.error('[ObjectMovementManager] 路径点不足，实际点数:', pathPoints.length)
      return false
    }

    const bottomOffset = this.calculateBottomOffset(object)

    const state: ObjectMovementState = {
      pathId,
      speed: config.speed,
      loops: config.loops,
      currentLoop: 0,
      isMoving: options.isMoving || false,
      tween: null,
      facingDirection: config.facingDirection || 'none',
      bottomOffset,
      currentProgress: options.currentProgress || 0
    }

    this.movements.set(object.uuid, state)

    if (options.currentPosition && Array.isArray(options.currentPosition) && options.currentPosition.length === 3) {
      object.position.set(options.currentPosition[0], options.currentPosition[1], options.currentPosition[2])
      console.log('[ObjectMovementManager] 恢复对象位置:', object.position)
    } else if (options.currentProgress !== undefined) {
      const position = this.getPositionOnPath(pathPoints, options.currentProgress)
      if (bottomOffset) {
        position.add(bottomOffset)
      }
      object.position.copy(position)
      console.log('[ObjectMovementManager] 根据进度恢复对象位置:', object.position)
    }

    if (state.facingDirection && state.facingDirection !== 'none') {
      this.updateObjectFacing(object, pathPoints, state.currentProgress || 0, state.facingDirection)
    }

    if (options.isMoving) {
      const duration = this.calculateDuration(pathPoints, config.speed, config.loops)
      const progress = { value: state.currentProgress || 0 }

      state.tween = gsap.to(progress, {
        value: config.loops,
        duration: duration,
        ease: 'none',
        onUpdate: () => {
          const normalizedProgress = progress.value % 1
          const position = this.getPositionOnPath(pathPoints, normalizedProgress)
          
          if (state.bottomOffset) {
            position.add(state.bottomOffset)
          }
          
          object.position.copy(position)
          
          if (state.facingDirection && state.facingDirection !== 'none') {
            this.updateObjectFacing(object, pathPoints, normalizedProgress, state.facingDirection)
          }
        },
        onComplete: () => {
          state.isMoving = false
          state.tween = null
          state.currentProgress = 0
          console.log('[ObjectMovementManager] 对象移动完成:', object.name)
        }
      })
      console.log('[ObjectMovementManager] 开始移动对象:', object.name)
    } else {
      console.log('[ObjectMovementManager] 对象已设置移动状态，但未开始移动')
    }

    return true
  }
}

export const objectMovementManager = new ObjectMovementManager()
