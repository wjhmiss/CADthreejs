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
  useTexture?: boolean
  transparent?: boolean
  opacity?: number
  blending?: THREE.Blending
  side?: THREE.Side
  arrow?: boolean
  scrollUV?: boolean
  scrollSpeed?: number
  progress?: number
  playSpeed?: number
  speed?: number
  parallelToXZ?: boolean
  loopProgress?: boolean
  textureWrapS?: THREE.Wrapping
  textureWrapT?: THREE.Wrapping
  textureRepeatX?: number
  textureRepeatY?: number
  textureOffsetX?: number
  textureOffsetY?: number
}

export interface TubePathConfig extends PathConfig {
  radius?: number
  radialSegments?: number
  startRad?: number
}

export interface PathMesh {
  id: string
  name: string
  points: THREE.Vector3[]
  objectIds: string[]
  geometry: THREE.BufferGeometry
  material: THREE.Material
  mesh: THREE.Mesh
  pathPointList: PathPointList
  updateParam: any
  texture?: THREE.Texture
  useTexture?: boolean
  scrollUV?: boolean
  scrollSpeed?: number
  progress?: number
  playSpeed?: number
  speed?: number
  parallelToXZ?: boolean
  loopProgress?: boolean
  textureWrapS?: THREE.Wrapping
  textureWrapT?: THREE.Wrapping
  textureRepeatX?: number
  textureRepeatY?: number
  textureOffsetX?: number
  textureOffsetY?: number
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
  private objectToPathsMap: Map<string, Set<string>> = new Map()
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
      url: '/images/diffuse.jpg'
    })

    this.texturePresets.set('light', {
      name: 'Light',
      url: '/images/light.png'
    })

    this.texturePresets.set('path_007_18', {
      name: 'Path 007-18',
      url: '/images/path_007_18.png'
    })

    this.texturePresets.set('path_007_19', {
      name: 'Path 007-19',
      url: '/images/path_007_19.png'
    })

    this.texturePresets.set('path_007_20', {
      name: 'Path 007-20',
      url: '/images/path_007_20.png'
    })

    this.texturePresets.set('path_007_21', {
      name: 'Path 007-21',
      url: '/images/path_007_21.png'
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

  createPath(config: PathConfig, name?: string, objectIds?: string[]): string {
    console.log('[PathManager] createPath() 开始执行')
    console.log('[PathManager] 接收到的配置:', config)
    console.log('[PathManager] 路径名称:', name)
    console.log('[PathManager] 关联对象ID:', objectIds)

    if (!this.scene) {
      console.error('[PathManager] 错误：scene 未设置')
      return ''
    }
    console.log('[PathManager] scene 已设置')

    const id = this._generateId(config.points)
    console.log('[PathManager] 生成的路径ID:', id)

    if (this.paths.has(id)) {
      console.warn('[PathManager] 警告：相同点的路径已存在，返回现有路径ID:', id)
      return id
    }

    let processedPoints = config.points
    if (config.parallelToXZ) {
      processedPoints = config.points.map(point => new THREE.Vector3(point.x, config.points[0].y, point.z))
      console.log('[PathManager] 路径已调整为平行XZ平面，所有Y坐标设置为:', config.points[0].y)
    }

    console.log('[PathManager] 创建 PathPointList...')
    const pathPointList = new PathPointList()
    pathPointList.set(
      processedPoints,
      config.cornerRadius || 0,
      config.cornerSplit || 0,
      config.up || null,
      config.close || false
    )
    console.log('[PathManager] PathPointList 创建完成')

    const updateParam = {
      width: config.width || 1,
      arrow: config.arrow !== undefined ? config.arrow : false,
      progress: config.progress !== undefined ? config.progress : 1,
      side: config.side || 'both',
      cornerRadius: config.cornerRadius || 0,
      cornerSplit: config.cornerSplit || 0
    }
    console.log('[PathManager] 更新参数:', updateParam)

    console.log('[PathManager] 创建 PathGeometry...')
    const geometry = new PathGeometry({
      pathPointList: pathPointList,
      options: updateParam
    })
    console.log('[PathManager] PathGeometry 创建完成')

    const resolvedTexture = this._resolveTexture(config.texture)
    console.log('[PathManager] 解析后的纹理:', resolvedTexture)
    
    const material = this._createPathMaterial(config, resolvedTexture)
    console.log('[PathManager] 材质创建完成:', material)

    console.log('[PathManager] 创建 Mesh...')
    const mesh = new THREE.Mesh(geometry, material)
    console.log('[PathManager] Mesh 创建完成，添加到场景')
    this.scene.add(mesh)
    console.log('[PathManager] Mesh 已添加到场景')

    const pathMesh: PathMesh = {
      id,
      name: name || `路径_${this.paths.size + 1}`,
      points: processedPoints,
      objectIds: objectIds || [],
      geometry,
      material,
      mesh,
      pathPointList,
      updateParam,
      texture: resolvedTexture,
      useTexture: config.useTexture !== undefined ? config.useTexture : false,
      scrollUV: config.scrollUV !== undefined ? config.scrollUV : false,
      scrollSpeed: config.scrollSpeed !== undefined ? config.scrollSpeed : 0.8,
      progress: config.progress !== undefined ? config.progress : 1,
      playSpeed: config.playSpeed !== undefined ? config.playSpeed : 0.14,
      speed: config.speed !== undefined ? config.speed : 0.48,
      parallelToXZ: config.parallelToXZ || false,
      loopProgress: config.loopProgress !== undefined ? config.loopProgress : false,
      textureWrapS: config.textureWrapS !== undefined ? config.textureWrapS : THREE.RepeatWrapping,
      textureWrapT: config.textureWrapT !== undefined ? config.textureWrapT : THREE.RepeatWrapping,
      textureRepeatX: config.textureRepeatX !== undefined ? config.textureRepeatX : 1,
      textureRepeatY: config.textureRepeatY !== undefined ? config.textureRepeatY : 1,
      textureOffsetX: config.textureOffsetX !== undefined ? config.textureOffsetX : 0,
      textureOffsetY: config.textureOffsetY !== undefined ? config.textureOffsetY : 0
    }

    this.paths.set(id, pathMesh)
    
    if (objectIds && objectIds.length > 0) {
      objectIds.forEach(objId => {
        if (!this.objectToPathsMap.has(objId)) {
          this.objectToPathsMap.set(objId, new Set())
        }
        this.objectToPathsMap.get(objId)!.add(id)
      })
    }
    
    console.log('[PathManager] 路径已保存到 paths Map')
    console.log('[PathManager] 当前所有路径数量:', this.paths.size)
    console.log('[PathManager] createPath() 执行完成，返回ID:', id)

    return id
  }

  createPathWithPreset(config: PathConfig, texturePreset: TexturePreset, name?: string, objectIds?: string[]): string {
    const texture = this.loadTextureFromPreset(texturePreset)
    return this.createPath({
      ...config,
      texture: texture || undefined
    }, name, objectIds)
  }

  createTubePath(config: TubePathConfig, name?: string, objectIds?: string[]): string {
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
      width: config.width || 0.5,
      radialSegments: config.radialSegments || 8,
      startRad: config.startRad || 0,
      progress: config.progress !== undefined ? config.progress : 1
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
      name: name || `路径_${this.paths.size + 1}`,
      points: config.points,
      objectIds: objectIds || [],
      geometry,
      material,
      mesh,
      pathPointList,
      updateParam,
      texture: resolvedTexture,
      useTexture: config.useTexture !== undefined ? config.useTexture : false,
      scrollUV: config.scrollUV !== undefined ? config.scrollUV : false,
      scrollSpeed: config.scrollSpeed !== undefined ? config.scrollSpeed : 0.03,
      progress: config.progress !== undefined ? config.progress : 1,
      playSpeed: config.playSpeed !== undefined ? config.playSpeed : 0.14,
      speed: config.speed !== undefined ? config.speed : 0.02,
      parallelToXZ: config.parallelToXZ || false
    }

    this.paths.set(id, pathMesh)
    
    if (objectIds && objectIds.length > 0) {
      objectIds.forEach(objId => {
        if (!this.objectToPathsMap.has(objId)) {
          this.objectToPathsMap.set(objId, new Set())
        }
        this.objectToPathsMap.get(objId)!.add(id)
      })
    }

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
      
      pathMesh.objectIds.forEach(objId => {
        const pathSet = this.objectToPathsMap.get(objId)
        if (pathSet) {
          pathSet.delete(id)
          if (pathSet.size === 0) {
            this.objectToPathsMap.delete(objId)
          }
        }
      })
      
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
      pathMesh.material.needsUpdate = true
    }
  }

  updatePathPointsById(id: string, points: THREE.Vector3[]): void {
    const pathMesh = this.paths.get(id)
    if (pathMesh) {
      pathMesh.points = points
      const up = pathMesh.parallelToXZ ? new THREE.Vector3(0, 1, 0) : null
      pathMesh.pathPointList.set(
        points,
        pathMesh.updateParam.cornerRadius || 0,
        pathMesh.updateParam.cornerSplit || 0,
        up,
        false
      )
      pathMesh.geometry.update(pathMesh.pathPointList, pathMesh.updateParam)
      pathMesh.geometry.computeBoundingBox()
      pathMesh.geometry.computeBoundingSphere()
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
      if (pathMesh.scrollUV && pathMesh.texture && pathMesh.scrollSpeed !== undefined) {
        pathMesh.texture.offset.x -= delta * pathMesh.scrollSpeed
      }

      if (pathMesh.playSpeed !== undefined && pathMesh.progress !== undefined) {
        pathMesh.progress += delta * pathMesh.playSpeed
        if (pathMesh.loopProgress && pathMesh.progress > 1) {
          pathMesh.progress = 0
        }
        pathMesh.updateParam.progress = pathMesh.progress
        pathMesh.geometry.update(pathMesh.pathPointList, pathMesh.updateParam)
      }
    })
  }

  getAllPaths(): PathMesh[] {
    return Array.from(this.paths.values())
  }

  findPathsByObjectId(objectId: string): PathMesh[] {
    const pathIds = this.objectToPathsMap.get(objectId)
    if (!pathIds) {
      return []
    }
    return Array.from(pathIds).map(id => this.paths.get(id)).filter((p): p is PathMesh => p !== undefined)
  }

  findPathByName(name: string): PathMesh | undefined {
    return Array.from(this.paths.values()).find(p => p.name === name)
  }

  getPathById(id: string): PathMesh | undefined {
    return this.paths.get(id)
  }

  updatePathName(id: string, name: string): boolean {
    const pathMesh = this.paths.get(id)
    if (pathMesh) {
      pathMesh.name = name
      return true
    }
    return false
  }

  getPathConfig(id: string): PathConfig | undefined {
    const pathMesh = this.paths.get(id)
    if (!pathMesh) {
      return undefined
    }

    const config: PathConfig = {
      points: pathMesh.points,
      width: pathMesh.updateParam.width,
      cornerRadius: 0,
      cornerSplit: 0,
      color: !Array.isArray(pathMesh.material) && 'color' in pathMesh.material ? pathMesh.material.color.getHex() : 0xffffff,
      texture: pathMesh.texture,
      useTexture: pathMesh.useTexture,
      transparent: !Array.isArray(pathMesh.material) && 'transparent' in pathMesh.material ? pathMesh.material.transparent : true,
      opacity: !Array.isArray(pathMesh.material) && 'opacity' in pathMesh.material ? pathMesh.material.opacity : 1,
      blending: !Array.isArray(pathMesh.material) && 'blending' in pathMesh.material ? pathMesh.material.blending : THREE.AdditiveBlending,
      side: !Array.isArray(pathMesh.material) && 'side' in pathMesh.material ? pathMesh.material.side : THREE.DoubleSide,
      arrow: pathMesh.updateParam.arrow,
      scrollUV: pathMesh.scrollUV,
      scrollSpeed: pathMesh.scrollSpeed,
      progress: pathMesh.progress,
      playSpeed: pathMesh.playSpeed,
      speed: pathMesh.speed,
      parallelToXZ: pathMesh.parallelToXZ
    }

    return config
  }

  updatePathConfig(id: string, config: Partial<PathConfig>): boolean {
    const pathMesh = this.paths.get(id)
    if (!pathMesh) {
      return false
    }

    if (config.width !== undefined) {
      this.updatePathWidthById(id, config.width)
    }

    if (config.color !== undefined) {
      this.updatePathColorById(id, config.color)
    }

    if (config.texture !== undefined) {
      this.updatePathTextureById(id, config.texture)
    }

    if (config.useTexture !== undefined) {
      pathMesh.useTexture = config.useTexture
      if (pathMesh.texture) {
        if (!Array.isArray(pathMesh.material) && 'map' in pathMesh.material) {
          pathMesh.material.map = config.useTexture ? pathMesh.texture : null
          pathMesh.material.needsUpdate = true
        }
      }
    }

    if (config.scrollUV !== undefined) {
      pathMesh.scrollUV = config.scrollUV
    }

    if (config.scrollSpeed !== undefined) {
      pathMesh.scrollSpeed = config.scrollSpeed
    }

    if (config.progress !== undefined) {
      pathMesh.progress = config.progress
      pathMesh.updateParam.progress = config.progress
      pathMesh.geometry.update(pathMesh.pathPointList, pathMesh.updateParam)
    }

    if (config.playSpeed !== undefined) {
      pathMesh.playSpeed = config.playSpeed
    }

    if (config.speed !== undefined) {
      this.setPathSpeedById(id, config.speed)
    }

    if (config.arrow !== undefined) {
      pathMesh.updateParam.arrow = config.arrow
      pathMesh.geometry.update(pathMesh.pathPointList, pathMesh.updateParam)
    }

    if (config.transparent !== undefined && !Array.isArray(pathMesh.material) && 'transparent' in pathMesh.material) {
      pathMesh.material.transparent = config.transparent
      pathMesh.material.needsUpdate = true
    }

    if (config.opacity !== undefined && !Array.isArray(pathMesh.material) && 'opacity' in pathMesh.material) {
      pathMesh.material.opacity = config.opacity
      pathMesh.material.needsUpdate = true
    }

    if (config.blending !== undefined && !Array.isArray(pathMesh.material) && 'blending' in pathMesh.material) {
      pathMesh.material.blending = config.blending
      pathMesh.material.needsUpdate = true
    }

    if (config.side !== undefined && !Array.isArray(pathMesh.material) && 'side' in pathMesh.material) {
      pathMesh.material.side = config.side
      pathMesh.material.needsUpdate = true
    }

    if (config.cornerRadius !== undefined || config.cornerSplit !== undefined) {
      const cornerRadius = config.cornerRadius !== undefined ? config.cornerRadius : 0
      const cornerSplit = config.cornerSplit !== undefined ? config.cornerSplit : 0
      const up = pathMesh.parallelToXZ ? new THREE.Vector3(0, 1, 0) : null
      pathMesh.updateParam.cornerRadius = cornerRadius
      pathMesh.updateParam.cornerSplit = cornerSplit
      pathMesh.pathPointList.set(
        pathMesh.points,
        cornerRadius,
        cornerSplit,
        up,
        false
      )
      pathMesh.geometry.update(pathMesh.pathPointList, pathMesh.updateParam)
    }

    if (config.parallelToXZ !== undefined) {
      pathMesh.parallelToXZ = config.parallelToXZ
      const up = config.parallelToXZ ? new THREE.Vector3(0, 1, 0) : null
      pathMesh.pathPointList.set(
        pathMesh.points,
        pathMesh.updateParam.cornerRadius || 0,
        pathMesh.updateParam.cornerSplit || 0,
        up,
        false
      )
      pathMesh.geometry.update(pathMesh.pathPointList, pathMesh.updateParam)
    }

    if (config.loopProgress !== undefined) {
      pathMesh.loopProgress = config.loopProgress
    }

    if (config.textureWrapS !== undefined && pathMesh.texture) {
      pathMesh.textureWrapS = config.textureWrapS
      pathMesh.texture.wrapS = config.textureWrapS
      pathMesh.texture.needsUpdate = true
    }

    if (config.textureWrapT !== undefined && pathMesh.texture) {
      pathMesh.textureWrapT = config.textureWrapT
      pathMesh.texture.wrapT = config.textureWrapT
      pathMesh.texture.needsUpdate = true
    }

    if (config.textureRepeatX !== undefined && pathMesh.texture) {
      pathMesh.textureRepeatX = config.textureRepeatX
      pathMesh.texture.repeat.x = config.textureRepeatX
      pathMesh.texture.needsUpdate = true
    }

    if (config.textureRepeatY !== undefined && pathMesh.texture) {
      pathMesh.textureRepeatY = config.textureRepeatY
      pathMesh.texture.repeat.y = config.textureRepeatY
      pathMesh.texture.needsUpdate = true
    }

    if (config.textureOffsetX !== undefined && pathMesh.texture) {
      pathMesh.textureOffsetX = config.textureOffsetX
      pathMesh.texture.offset.x = config.textureOffsetX
      pathMesh.texture.needsUpdate = true
    }

    if (config.textureOffsetY !== undefined && pathMesh.texture) {
      pathMesh.textureOffsetY = config.textureOffsetY
      pathMesh.texture.offset.y = config.textureOffsetY
      pathMesh.texture.needsUpdate = true
    }

    return true
  }

  updatePathTextureWrap(id: string, wrapS: THREE.Wrapping | null, wrapT: THREE.Wrapping | null): void {
    const pathMesh = this.paths.get(id)
    if (!pathMesh || !pathMesh.texture) return

    if (wrapS !== null) {
      pathMesh.texture.wrapS = wrapS
    }
    if (wrapT !== null) {
      pathMesh.texture.wrapT = wrapT
    }
    pathMesh.texture.needsUpdate = true
  }

  updatePathTextureRepeat(id: string, repeatX: number | null, repeatY: number | null): void {
    const pathMesh = this.paths.get(id)
    if (!pathMesh || !pathMesh.texture) return

    if (repeatX !== null) {
      pathMesh.texture.repeat.x = repeatX
    }
    if (repeatY !== null) {
      pathMesh.texture.repeat.y = repeatY
    }
    pathMesh.texture.needsUpdate = true
  }

  updatePathTextureOffset(id: string, offsetX: number | null, offsetY: number | null): void {
    const pathMesh = this.paths.get(id)
    if (!pathMesh || !pathMesh.texture) return

    if (offsetX !== null) {
      pathMesh.texture.offset.x = offsetX
    }
    if (offsetY !== null) {
      pathMesh.texture.offset.y = offsetY
    }
    pathMesh.texture.needsUpdate = true
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
    
    if (texture) {
      texture.wrapS = config.textureWrapS !== undefined ? config.textureWrapS : THREE.RepeatWrapping
      texture.wrapT = config.textureWrapT !== undefined ? config.textureWrapT : THREE.RepeatWrapping
      texture.repeat.x = config.textureRepeatX !== undefined ? config.textureRepeatX : 1
      texture.repeat.y = config.textureRepeatY !== undefined ? config.textureRepeatY : 1
      texture.offset.x = config.textureOffsetX !== undefined ? config.textureOffsetX : 0
      texture.offset.y = config.textureOffsetY !== undefined ? config.textureOffsetY : 0
    }
    
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
