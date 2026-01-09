import * as THREE from 'three'
import { GridFloor } from './gridFloor'

export interface MapOrganizerConfig {
  intersectedEdgeColor: string
}

export class MapOrganizer {
  private gridFloor: GridFloor
  private config: MapOrganizerConfig

  constructor(gridFloor: GridFloor, config: Partial<MapOrganizerConfig> = {}) {
    this.gridFloor = gridFloor
    this.config = {
      intersectedEdgeColor: '#ff0000',
      ...config
    }
  }

  organize(objects: THREE.Object3D[]): { intersectedCount: number } {
    if (!this.gridFloor) {
      console.log('网格地面未初始化')
      return { intersectedCount: 0 }
    }

    console.log('开始地图整理，检测DXF对象与网格边框线条的相交情况...')

    const floorConfig = this.gridFloor.getConfig()
    const cellSize = floorConfig.cellSize
    const gridSize = floorConfig.gridSize
    const halfSize = (gridSize * cellSize) / 2
    const offsetX = floorConfig.offsetX
    const offsetZ = floorConfig.offsetZ

    let intersectedCount = 0

    this.resetColors()

    console.log(`总共检测 ${objects.length} 个对象`)
    console.log(`网格偏移: offsetX=${offsetX}, offsetZ=${offsetZ}`)

    objects.forEach((obj) => {
      const box = new THREE.Box3().setFromObject(obj)
      
      if (box.isEmpty()) {
        return
      }

      const minX = box.min.x
      const maxX = box.max.x
      const minZ = box.min.z
      const maxZ = box.max.z

      const startCol = Math.floor((minX - offsetX + halfSize) / cellSize)
      const endCol = Math.floor((maxX - offsetX + halfSize) / cellSize)
      const startRow = Math.floor((minZ - offsetZ + halfSize) / cellSize)
      const endRow = Math.floor((maxZ - offsetZ + halfSize) / cellSize)

      for (let row = Math.max(0, startRow); row <= Math.min(gridSize - 1, endRow); row++) {
        for (let col = Math.max(0, startCol); col <= Math.min(gridSize - 1, endCol); col++) {
          const cellMinX = (col * cellSize) - halfSize + offsetX
          const cellMaxX = ((col + 1) * cellSize) - halfSize + offsetX
          const cellMinZ = (row * cellSize) - halfSize + offsetZ
          const cellMaxZ = ((row + 1) * cellSize) - halfSize + offsetZ

          const xOverlap = !(cellMaxX < minX || cellMinX > maxX)
          const zOverlap = !(cellMaxZ < minZ || cellMinZ > maxZ)

          if (xOverlap && zOverlap) {
            if (this.checkEdgeIntersection(obj, cellMinX, cellMaxX, cellMinZ, cellMaxZ)) {
              this.gridFloor.setCellEdgeColor(row, col, this.config.intersectedEdgeColor)
              this.gridFloor.setCellVisible(row, col, true)
              intersectedCount++
            }
          }
        }
      }
    })

    console.log(`地图整理完成，共检测到 ${intersectedCount} 个相交边框`)
    return { intersectedCount }
  }

  private checkEdgeIntersection(obj: THREE.Object3D, cellMinX: number, cellMaxX: number, cellMinZ: number, cellMaxZ: number): boolean {
    const edges = [
      { start: new THREE.Vector3(cellMinX, 0, cellMinZ), end: new THREE.Vector3(cellMaxX, 0, cellMinZ) },
      { start: new THREE.Vector3(cellMaxX, 0, cellMinZ), end: new THREE.Vector3(cellMaxX, 0, cellMaxZ) },
      { start: new THREE.Vector3(cellMaxX, 0, cellMaxZ), end: new THREE.Vector3(cellMinX, 0, cellMaxZ) },
      { start: new THREE.Vector3(cellMinX, 0, cellMaxZ), end: new THREE.Vector3(cellMinX, 0, cellMinZ) }
    ]

    const raycaster = new THREE.Raycaster()
    const epsilon = 0.001

    for (const edge of edges) {
      const direction = new THREE.Vector3().subVectors(edge.end, edge.start).normalize()
      const distance = edge.start.distanceTo(edge.end)

      raycaster.set(edge.start, direction)
      raycaster.far = distance
      raycaster.near = epsilon

      const intersects = raycaster.intersectObject(obj, true)

      if (intersects.length > 0) {
        return true
      }
    }

    return false
  }

  resetColors(): void {
    if (!this.gridFloor) {
      console.log('网格地面未初始化')
      return
    }

    console.log('重置所有边框颜色...')

    const config = this.gridFloor.getConfig()
    
    this.gridFloor.setAllCellsEdgeColor(config.defaultEdgeColor)
    this.gridFloor.setAllCellsVisible(false)
  }

  setIntersectedColor(edgeColor: string): void {
    this.config.intersectedEdgeColor = edgeColor
  }
}
