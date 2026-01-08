import * as THREE from 'three'
import { GridFloor } from './gridFloor'

export interface MapOrganizerConfig {
  intersectedFillColor: string
  intersectedEdgeColor: string
}

export class MapOrganizer {
  private gridFloor: GridFloor
  private config: MapOrganizerConfig

  constructor(gridFloor: GridFloor, config: Partial<MapOrganizerConfig> = {}) {
    this.gridFloor = gridFloor
    this.config = {
      intersectedFillColor: '#ff0000',
      intersectedEdgeColor: '#ff0000',
      ...config
    }
  }

  organize(objects: THREE.Object3D[]): { intersectedCount: number } {
    if (!this.gridFloor) {
      console.log('网格地面未初始化')
      return { intersectedCount: 0 }
    }

    console.log('开始地图整理，检测DXF对象与地板方格的相交情况...')

    const floorConfig = this.gridFloor.getConfig()
    const cellSize = floorConfig.cellSize
    const gridSize = floorConfig.gridSize
    const halfSize = (gridSize * cellSize) / 2

    let intersectedCount = 0

    this.resetColors()

    console.log(`总共检测 ${objects.length} 个对象`)

    objects.forEach((obj) => {
      const box = new THREE.Box3().setFromObject(obj)
      
      if (box.isEmpty()) {
        return
      }

      const minX = box.min.x
      const maxX = box.max.x
      const minZ = box.min.z
      const maxZ = box.max.z

      const startCol = Math.floor((minX + halfSize) / cellSize)
      const endCol = Math.floor((maxX + halfSize) / cellSize)
      const startRow = Math.floor((minZ + halfSize) / cellSize)
      const endRow = Math.floor((maxZ + halfSize) / cellSize)

      for (let row = Math.max(0, startRow); row <= Math.min(gridSize - 1, endRow); row++) {
        for (let col = Math.max(0, startCol); col <= Math.min(gridSize - 1, endCol); col++) {
          const cellMinX = (col * cellSize) - halfSize
          const cellMaxX = ((col + 1) * cellSize) - halfSize
          const cellMinZ = (row * cellSize) - halfSize
          const cellMaxZ = ((row + 1) * cellSize) - halfSize

          const xOverlap = !(cellMaxX < minX || cellMinX > maxX)
          const zOverlap = !(cellMaxZ < minZ || cellMinZ > maxZ)

          if (xOverlap && zOverlap) {
            this.gridFloor.setCellFillColor(row, col, this.config.intersectedFillColor)
            this.gridFloor.setCellEdgeColor(row, col, this.config.intersectedEdgeColor)
            this.gridFloor.setCellVisible(row, col, true)
            intersectedCount++
          }
        }
      }
    })

    console.log(`地图整理完成，共检测到 ${intersectedCount} 个相交方块`)
    return { intersectedCount }
  }

  resetColors(): void {
    if (!this.gridFloor) {
      console.log('网格地面未初始化')
      return
    }

    console.log('重置所有方块颜色...')

    const config = this.gridFloor.getConfig()
    
    this.gridFloor.setAllCellsFillColor(config.defaultFillColor)
    this.gridFloor.setAllCellsEdgeColor(config.defaultEdgeColor)
    this.gridFloor.setAllCellsVisible(false)
  }

  setIntersectedColor(fillColor: string, edgeColor: string): void {
    this.config.intersectedFillColor = fillColor
    this.config.intersectedEdgeColor = edgeColor
  }
}
