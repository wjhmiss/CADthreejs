import * as THREE from 'three'

export interface GridCell {
  visible: boolean
  edgeColor: string
  line: THREE.Line | null
}

export interface GridFloorConfig {
  gridSize: number
  cellSize: number
  defaultEdgeColor: string
  offsetX: number
  offsetZ: number
}

export class GridFloor {
  private scene: THREE.Scene
  private config: GridFloorConfig
  private cells: GridCell[][]
  private group: THREE.Group

  constructor(scene: THREE.Scene, config: Partial<GridFloorConfig> = {}) {
    this.scene = scene
    this.config = {
      gridSize: 20,
      cellSize: 1,
      defaultEdgeColor: '#808080',
      offsetX: 0,
      offsetZ: 0,
      ...config
    }
    this.group = new THREE.Group()
    this.cells = []
    this.initialize()
  }

  private initialize(): void {
    const halfSize = (this.config.gridSize * this.config.cellSize) / 2

    for (let i = 0; i < this.config.gridSize; i++) {
      this.cells[i] = []
      for (let j = 0; j < this.config.gridSize; j++) {
        const cell = this.createCell(i, j, halfSize)
        this.cells[i][j] = cell
        if (cell.line) {
          this.group.add(cell.line)
        }
      }
    }

    this.group.position.set(this.config.offsetX, 0, this.config.offsetZ)
    this.scene.add(this.group)
  }

  private createCell(row: number, col: number, halfSize: number): GridCell {
    const x = (col * this.config.cellSize) - halfSize + (this.config.cellSize / 2)
    const z = (row * this.config.cellSize) - halfSize + (this.config.cellSize / 2)

    const size = this.config.cellSize
    const halfCell = size / 2

    const points = [
      new THREE.Vector3(-halfCell, 0, -halfCell),
      new THREE.Vector3(halfCell, 0, -halfCell),
      new THREE.Vector3(halfCell, 0, halfCell),
      new THREE.Vector3(-halfCell, 0, halfCell),
      new THREE.Vector3(-halfCell, 0, -halfCell)
    ]

    const geometry = new THREE.BufferGeometry().setFromPoints(points)
    const material = new THREE.LineBasicMaterial({
      color: this.config.defaultEdgeColor
    })

    const line = new THREE.Line(geometry, material)
    line.position.set(x, 0, z)
    line.userData = { row, col }

    return {
      visible: false,
      edgeColor: this.config.defaultEdgeColor,
      line
    }
  }

  getCell(row: number, col: number): GridCell | null {
    if (row >= 0 && row < this.config.gridSize && col >= 0 && col < this.config.gridSize) {
      return this.cells[row][col]
    }
    return null
  }

  setCellVisible(row: number, col: number, visible: boolean): void {
    const cell = this.getCell(row, col)
    if (cell && cell.line) {
      cell.visible = visible
      cell.line.visible = visible
    }
  }

  setCellEdgeColor(row: number, col: number, color: string): void {
    const cell = this.getCell(row, col)
    if (cell && cell.line && cell.line.material) {
      cell.edgeColor = color
      const material = cell.line.material as THREE.LineBasicMaterial
      material.color = new THREE.Color(color)
    }
  }

  setAllCellsVisible(visible: boolean): void {
    for (let i = 0; i < this.config.gridSize; i++) {
      for (let j = 0; j < this.config.gridSize; j++) {
        this.setCellVisible(i, j, visible)
      }
    }
  }

  setAllCellsEdgeColor(color: string): void {
    for (let i = 0; i < this.config.gridSize; i++) {
      for (let j = 0; j < this.config.gridSize; j++) {
        this.setCellEdgeColor(i, j, color)
      }
    }
  }

  setCellsRange(startRow: number, endRow: number, startCol: number, endCol: number, edgeColor: string): void {
    for (let row = startRow; row <= endRow; row++) {
      for (let col = startCol; col <= endCol; col++) {
        if (row >= 0 && row < this.config.gridSize && col >= 0 && col < this.config.gridSize) {
          this.setCellEdgeColor(row, col, edgeColor)
          this.setCellVisible(row, col, true)
        }
      }
    }
  }

  updateCellSize(newSize: number): void {
    this.config.cellSize = newSize
    this.clear()
    this.initialize()
  }

  updateGridSize(newSize: number): void {
    this.config.gridSize = newSize
    this.clear()
    this.initialize()
  }

  clear(): void {
    this.scene.remove(this.group)
    this.cells = []
    while (this.group.children.length > 0) {
      const child = this.group.children[0]
      if (child instanceof THREE.Line) {
        child.geometry.dispose()
        if (child.material instanceof THREE.Material) {
          child.material.dispose()
        }
      }
      this.group.remove(child)
    }
  }

  dispose(): void {
    this.clear()
  }

  getGroup(): THREE.Group {
    return this.group
  }

  getConfig(): GridFloorConfig {
    return { ...this.config }
  }

  getCells(): GridCell[][] {
    return this.cells
  }

  updateOffset(offsetX: number, offsetZ: number): void {
    this.config.offsetX = offsetX
    this.config.offsetZ = offsetZ
    this.group.position.set(offsetX, 0, offsetZ)
  }

  getOffset(): { offsetX: number; offsetZ: number } {
    return {
      offsetX: this.config.offsetX,
      offsetZ: this.config.offsetZ
    }
  }
}
