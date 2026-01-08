import * as THREE from 'three'

export interface GridCell {
  visible: boolean
  fillColor: string
  edgeColor: string
  mesh: THREE.Mesh | null
}

export interface GridFloorConfig {
  gridSize: number
  cellSize: number
  defaultEdgeColor: string
  defaultFillColor: string
  opacity: number
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
      defaultFillColor: 'transparent',
      opacity: 0,
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
        if (cell.mesh) {
          this.group.add(cell.mesh)
        }
      }
    }

    this.scene.add(this.group)
  }

  private createCell(row: number, col: number, halfSize: number): GridCell {
    const x = (col * this.config.cellSize) - halfSize + (this.config.cellSize / 2)
    const z = (row * this.config.cellSize) - halfSize + (this.config.cellSize / 2)

    const geometry = new THREE.PlaneGeometry(this.config.cellSize, this.config.cellSize)
    const material = new THREE.MeshBasicMaterial({
      color: this.config.defaultFillColor,
      transparent: true,
      opacity: this.config.opacity,
      side: THREE.DoubleSide
    })

    const mesh = new THREE.Mesh(geometry, material)
    mesh.rotation.x = -Math.PI / 2
    mesh.position.set(x, 0, z)
    mesh.userData = { row, col }

    const edgesGeometry = new THREE.EdgesGeometry(geometry)
    const edgesMaterial = new THREE.LineBasicMaterial({
      color: this.config.defaultEdgeColor
    })
    const edges = new THREE.LineSegments(edgesGeometry, edgesMaterial)
    mesh.add(edges)

    return {
      visible: false,
      fillColor: this.config.defaultFillColor,
      edgeColor: this.config.defaultEdgeColor,
      mesh
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
    if (cell && cell.mesh) {
      cell.visible = visible
      cell.mesh.visible = true
      cell.mesh.material.opacity = visible ? 1 : this.config.opacity
    }
  }

  setCellFillColor(row: number, col: number, color: string): void {
    const cell = this.getCell(row, col)
    if (cell && cell.mesh) {
      cell.fillColor = color
      cell.mesh.material.color.set(color)
    }
  }

  setCellEdgeColor(row: number, col: number, color: string): void {
    const cell = this.getCell(row, col)
    if (cell && cell.mesh) {
      cell.edgeColor = color
      const edges = cell.mesh.children[0] as THREE.LineSegments
      if (edges && edges.material) {
        (edges.material as THREE.LineBasicMaterial).color.set(color)
      }
    }
  }

  setAllCellsVisible(visible: boolean): void {
    for (let i = 0; i < this.config.gridSize; i++) {
      for (let j = 0; j < this.config.gridSize; j++) {
        this.setCellVisible(i, j, visible)
      }
    }
  }

  setAllCellsFillColor(color: string): void {
    for (let i = 0; i < this.config.gridSize; i++) {
      for (let j = 0; j < this.config.gridSize; j++) {
        this.setCellFillColor(i, j, color)
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

  setCellsRange(startRow: number, endRow: number, startCol: number, endCol: number, fillColor: string, edgeColor: string): void {
    for (let row = startRow; row <= endRow; row++) {
      for (let col = startCol; col <= endCol; col++) {
        if (row >= 0 && row < this.config.gridSize && col >= 0 && col < this.config.gridSize) {
          this.setCellFillColor(row, col, fillColor)
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
      if (child instanceof THREE.Mesh) {
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
}
