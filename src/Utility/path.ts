import * as THREE from 'three'
import { PathGeometry, PathPointList, PathTubeGeometry } from './shaderPath/build/three.path.module.js'

export interface PathConfig {
  points: THREE.Vector3[]
  width?: number
  cornerRadius?: number
  cornerSplit?: number
  up?: THREE.Vector3
  close?: boolean
  color?: number
  texture?: THREE.Texture | string
  transparent?: boolean
  opacity?: number
  blending?: THREE.Blending
  side?: THREE.Side
  arrow?: boolean
  speed?: number
}

export interface TubePathConfig extends PathConfig {
  radius?: number
  radialSegments?: number
  startRad?: number
}

export interface PathMesh {
  id: string
  points: THREE.Vector3[]
  geometry: THREE.BufferGeometry
  material: THREE.Material
  mesh: THREE.Mesh
  pathPointList: PathPointList
  updateParam: any
  texture?: THREE.Texture
  speed?: number
}

export interface TextureConfig {
  name: string
  url: string
  wrapS?: THREE.Wrapping
  wrapT?: THREE.Wrapping
  repeat?: THREE.Vector2
  offset?: THREE.Vector2
  anisotropy?: number
}

export type TexturePreset = 'none' | 'diffuse' | 'light' | 'path_007_18' | 'path_007_19' | 'path_007_20' | 'path_007_21'

class PathManager {
  private scene: THREE.Scene | null = null
  private renderer: THREE.WebGLRenderer | null = null
  private paths: Map<string, PathMesh> = new Map()
  private textureCache: Map<string, THREE.Texture> = new Map()
  private texturePresets: Map<TexturePreset, TextureConfig> = new Map()

  constructor() {
    this._initTexturePresets()
  }

  setScene(scene: THREE.Scene): void {
    this.scene = scene
  }

  setRenderer(renderer: THREE.WebGLRenderer): void {
    this.renderer = renderer
  }

  private _initTexturePresets(): void {
    this.texturePresets.set('none', {
      name: 'None',
      url: ''
    })

    this.texturePresets.set('diffuse', {
      name: 'Diffuse',
      url: '/Utility/shaderPath/examples/images/diffuse.jpg'
    })

    this.texturePresets.set('light', {
      name: 'Light',
      url: '/Utility/shaderPath/examples/images/light.png'
    })

    this.texturePresets.set('path_007_18', {
      name: 'Path 007-18',
      url: '/Utility/shaderPath/examples/images/path_007_18.png'
    })

    this.texturePresets.set('path_007_19', {
      name: 'Path 007-19',
      url: '/Utility/shaderPath/examples/images/path_007_19.png'
    })

    this.texturePresets.set('path_007_20', {
      name: 'Path 007-20',
      url: '/Utility/shaderPath/examples/images/path_007_20.png'
    })

    this.texturePresets.set('path_007_21', {
      name: 'Path 007-21',
      url: '/Utility/shaderPath/examples/images/path_007_21.png'
    })
  }

  getTexturePresets(): Map<TexturePreset, TextureConfig> {
    return this.texturePresets
  }

  getTexturePreset(preset: TexturePreset): TextureConfig | undefined {
    return this.texturePresets.get(preset)
  }

  loadTexture(url: string, onComplete?: (texture: THREE.Texture) => void): THREE.Texture | null {
    if (!url || url === '') {
      return null
    }

    if (this.textureCache.has(url)) {
      const texture = this.textureCache.get(url)!
      if (onComplete) {
        onComplete(texture)
      }
      return texture
    }

    const texture = new THREE.TextureLoader().load(url, (loadedTexture) => {
      loadedTexture.wrapS = THREE.RepeatWrapping
      loadedTexture.wrapT = THREE.RepeatWrapping
      loadedTexture.repeat.x = 1
      loadedTexture.repeat.y = 1

      if (this.renderer) {
        loadedTexture.anisotropy = this.renderer.capabilities.getMaxAnisotropy()
      }

      if (onComplete) {
        onComplete(loadedTexture)
      }
    })

    this.textureCache.set(url, texture)
    return texture
  }

  loadTextureFromPreset(preset: TexturePreset, onComplete?: (texture: THREE.Texture) => void): THREE.Texture | null {
    const config = this.texturePresets.get(preset)
    if (!config || preset === 'none') {
      return null
    }
    return this.loadTexture(config.url, onComplete)
  }

  private _generateId(points: THREE.Vector3[]): string {
    return JSON.stringify(points.map(p => ({ x: p.x, y: p.y, z: p.z })))
  }

  private _resolveTexture(texture: THREE.Texture | string | undefined): THREE.Texture | undefined {
    if (!texture) {
      return undefined
    }

    if (texture instanceof THREE.Texture) {
      return texture
    }

    if (typeof texture === 'string') {
      return this.loadTexture(texture) || undefined
    }

    return undefined
  }

  createPath(config: PathConfig): string {
    if (!this.scene) {
      console.error('PathManager: scene not set')
      return ''
    }

    const id = this._generateId(config.points)

    if (this.paths.has(id)) {
      console.warn('PathManager: path with same points already exists, returning existing path id:', id)
      return id
    }

    const pathPointList = new PathPointList()
    pathPointList.set(
      config.points,
      config.cornerRadius || 0,
      config.cornerSplit || 0,
      config.up || null,
      config.close || false
    )

    const updateParam = {
      width: config.width || 2,
      arrow: config.arrow !== undefined ? config.arrow : false,
      progress: 0
    }

    const geometry = new PathGeometry({
      pathPointList: pathPointList,
      options: updateParam
    })

    const resolvedTexture = this._resolveTexture(config.texture)
    const material = this._createPathMaterial(config, resolvedTexture)

    const mesh = new THREE.Mesh(geometry, material)
    this.scene.add(mesh)

    const pathMesh: PathMesh = {
      id,
      points: config.points,
      geometry,
      material,
      mesh,
      pathPointList,
      updateParam,
      texture: resolvedTexture,
      speed: config.speed !== undefined ? config.speed : 0.02
    }

    this.paths.set(id, pathMesh)

    return id
  }

  createPathWithPreset(config: PathConfig, texturePreset: TexturePreset): string {
    const texture = this.loadTextureFromPreset(texturePreset)
    return this.createPath({
      ...config,
      texture: texture || undefined
    })
  }

  createTubePath(config: TubePathConfig): string {
    if (!this.scene) {
      console.error('PathManager: scene not set')
      return ''
    }

    const id = this._generateId(config.points)

    if (this.paths.has(id)) {
      console.warn('PathManager: path with same points already exists, returning existing path id:', id)
      return id
    }

    const pathPointList = new PathPointList()
    pathPointList.set(
      config.points,
      config.cornerRadius || 0,
      config.cornerSplit || 0,
      config.up || null,
      config.close || false
    )

    const updateParam = {
      width: config.radius || 0.5,
      radialSegments: config.radialSegments || 8,
      startRad: config.startRad || 0,
      progress: 0
    }

    const geometry = new PathTubeGeometry({
      pathPointList: pathPointList,
      options: updateParam
    })

    const resolvedTexture = this._resolveTexture(config.texture)
    const material = this._createTubeMaterial(config, resolvedTexture)

    const mesh = new THREE.Mesh(geometry, material)
    this.scene.add(mesh)

    const pathMesh: PathMesh = {
      id,
      points: config.points,
      geometry,
      material,
      mesh,
      pathPointList,
      updateParam,
      texture: resolvedTexture,
      speed: config.speed !== undefined ? config.speed : 0.02
    }

    this.paths.set(id, pathMesh)

    return id
  }

  createTubePathWithPreset(config: TubePathConfig, texturePreset: TexturePreset): string {
    const texture = this.loadTextureFromPreset(texturePreset)
    return this.createTubePath({
      ...config,
      texture: texture || undefined
    })
  }

  removePath(points: THREE.Vector3[]): boolean {
    const id = this._generateId(points)
    return this.removePathById(id)
  }

  removePathById(id: string): boolean {
    const pathMesh = this.paths.get(id)
    if (pathMesh) {
      if (this.scene) {
        this.scene.remove(pathMesh.mesh)
      }
      pathMesh.geometry.dispose()
      if (Array.isArray(pathMesh.material)) {
        pathMesh.material.forEach(m => m.dispose())
      } else {
        pathMesh.material.dispose()
      }
      this.paths.delete(id)
      return true
    }
    return false
  }

  clearAllPaths(): void {
    this.paths.forEach((pathMesh) => {
      if (this.scene) {
        this.scene.remove(pathMesh.mesh)
      }
      pathMesh.geometry.dispose()
      if (Array.isArray(pathMesh.material)) {
        pathMesh.material.forEach(m => m.dispose())
      } else {
        pathMesh.material.dispose()
      }
    })
    this.paths.clear()
  }

  clearTextureCache(): void {
    this.textureCache.forEach((texture) => {
      texture.dispose()
    })
    this.textureCache.clear()
  }

  findPath(points: THREE.Vector3[]): PathMesh | undefined {
    const id = this._generateId(points)
    return this.findPathById(id)
  }

  findPathById(id: string): PathMesh | undefined {
    return this.paths.get(id)
  }

  updatePathProgress(points: THREE.Vector3[], progress: number): void {
    const id = this._generateId(points)
    this.updatePathProgressById(id, progress)
  }

  updatePathProgressById(id: string, progress: number): void {
    const pathMesh = this.paths.get(id)
    if (pathMesh) {
      pathMesh.updateParam.progress = progress
      pathMesh.geometry.update(pathMesh.pathPointList, pathMesh.updateParam)
      pathMesh.geometry.computeBoundingBox()
      pathMesh.geometry.computeBoundingSphere()
    }
  }

  updatePathWidth(points: THREE.Vector3[], width: number): void {
    const id = this._generateId(points)
    this.updatePathWidthById(id, width)
  }

  updatePathWidthById(id: string, width: number): void {
    const pathMesh = this.paths.get(id)
    if (pathMesh) {
      pathMesh.updateParam.width = width
      pathMesh.geometry.update(pathMesh.pathPointList, pathMesh.updateParam)
      pathMesh.geometry.computeBoundingBox()
      pathMesh.geometry.computeBoundingSphere()
    }
  }

  updatePathColor(points: THREE.Vector3[], color: number): void {
    const id = this._generateId(points)
    this.updatePathColorById(id, color)
  }

  updatePathColorById(id: string, color: number): void {
    const pathMesh = this.paths.get(id)
    if (pathMesh && !Array.isArray(pathMesh.material)) {
      pathMesh.material.color.setHex(color)
    }
  }

  updatePathTexture(points: THREE.Vector3[], texture: THREE.Texture | string): void {
    const id = this._generateId(points)
    this.updatePathTextureById(id, texture)
  }

  updatePathTextureById(id: string, texture: THREE.Texture | string): void {
    const pathMesh = this.paths.get(id)
    if (pathMesh && !Array.isArray(pathMesh.material) && 'map' in pathMesh.material) {
      const resolvedTexture = this._resolveTexture(texture)
      pathMesh.texture = resolvedTexture
      pathMesh.material.map = resolvedTexture
      pathMesh.material.needsUpdate = true
    }
  }

  updatePathTextureWithPreset(points: THREE.Vector3[], preset: TexturePreset): void {
    const id = this._generateId(points)
    this.updatePathTextureByIdWithPreset(id, preset)
  }

  updatePathTextureByIdWithPreset(id: string, preset: TexturePreset): void {
    const pathMesh = this.paths.get(id)
    if (pathMesh && !Array.isArray(pathMesh.material) && 'map' in pathMesh.material) {
      const texture = this.loadTextureFromPreset(preset)
      pathMesh.texture = texture || undefined
      pathMesh.material.map = texture || null
      pathMesh.material.needsUpdate = true
    }
  }

  setPathSpeed(points: THREE.Vector3[], speed: number): void {
    const id = this._generateId(points)
    this.setPathSpeedById(id, speed)
  }

  setPathSpeedById(id: string, speed: number): void {
    const pathMesh = this.paths.get(id)
    if (pathMesh) {
      pathMesh.speed = speed
    }
  }

  update(delta: number): void {
    this.paths.forEach((pathMesh) => {
      if (pathMesh.texture && pathMesh.speed !== undefined) {
        pathMesh.texture.offset.x -= delta * pathMesh.speed
      }
    })
  }

  getAllPaths(): PathMesh[] {
    return Array.from(this.paths.values())
  }

  private _createPathMaterial(config: PathConfig, texture?: THREE.Texture): THREE.Material {
    const material = new THREE.MeshBasicMaterial({
      color: config.color || 0xffffff,
      depthWrite: false,
      depthTest: true,
      transparent: config.transparent !== undefined ? config.transparent : true,
      opacity: config.opacity || 1,
      blending: config.blending || THREE.AdditiveBlending,
      side: config.side || THREE.DoubleSide,
      map: texture
    })
    return material
  }

  private _createTubeMaterial(config: TubePathConfig, texture?: THREE.Texture): THREE.Material {
    const material = new THREE.MeshBasicMaterial({
      color: config.color || 0xffffff,
      transparent: config.transparent !== undefined ? config.transparent : true,
      opacity: config.opacity || 1,
      side: config.side || THREE.DoubleSide,
      map: texture
    })
    return material
  }

  dispose(): void {
    this.clearAllPaths()
    this.clearTextureCache()
  }
}

const pathManager = new PathManager()

export { PathManager, pathManager }
export type { PathConfig, TubePathConfig, PathMesh, TextureConfig, TexturePreset }
