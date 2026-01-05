import * as THREE from 'three'
import { pathManager, type PathConfig } from './path'

export interface PathObject {
  id: string
  object: THREE.Object3D
  name: string
  bottomCenter: THREE.Vector3
}

export interface PathPanelConfig {
  width?: number
  height?: number
  position?: { x: number; y: number }
  backgroundColor?: string
  textColor?: string
  itemBackgroundColor?: string
  itemHoverColor?: string
}

class PathPanelManager {
  private objects: PathObject[] = []
  private panelVisible: boolean = false
  private panelElement: HTMLElement | null = null
  private toggleButton: HTMLElement | null = null
  private config: Required<PathPanelConfig>
  private currentPathId: string = ''
  private onPathUpdateCallback: ((pathId: string) => void) | null = null

  constructor(config: PathPanelConfig = {}) {
    this.config = {
      width: config.width || 250,
      height: config.height || 300,
      position: config.position || { x: 10, y: 10 },
      backgroundColor: config.backgroundColor || 'rgba(255, 255, 255, 0.9)',
      textColor: config.textColor || '#333',
      itemBackgroundColor: config.itemBackgroundColor || '#f5f5f5',
      itemHoverColor: config.itemHoverColor || '#e0e0e0'
    }
  }

  initialize(container: HTMLElement): void {
    this.createToggleButton(container)
    this.createPanel(container)
    this.addEventListeners()
  }

  private createToggleButton(container: HTMLElement): void {
    this.toggleButton = document.createElement('button')
    this.toggleButton.className = 'path-panel-toggle-btn'
    this.toggleButton.innerHTML = '◀'
    this.toggleButton.style.cssText = `
      position: fixed;
      bottom: 10px;
      left: 10px;
      width: 30px;
      height: 30px;
      padding: 0;
      background-color: #607D8B;
      color: white;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 16px;
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1002;
      box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
      transition: all 0.3s ease;
    `

    this.toggleButton.addEventListener('mouseenter', () => {
      this.toggleButton!.style.backgroundColor = '#455A64'
    })

    this.toggleButton.addEventListener('mouseleave', () => {
      this.toggleButton!.style.backgroundColor = '#607D8B'
    })

    this.toggleButton.addEventListener('click', () => {
      this.togglePanel()
    })

    container.appendChild(this.toggleButton)
  }

  private createPanel(container: HTMLElement): void {
    this.panelElement = document.createElement('div')
    this.panelElement.className = 'path-panel'
    this.panelElement.style.cssText = `
      position: fixed;
      bottom: 50px;
      left: 10px;
      width: ${this.config.width}px;
      max-height: ${this.config.height}px;
      background-color: ${this.config.backgroundColor};
      border-radius: 5px;
      box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
      z-index: 1001;
      display: none;
      flex-direction: column;
      overflow: hidden;
      transition: all 0.3s ease;
      opacity: 0;
      transform: translateY(10px);
    `

    const header = document.createElement('div')
    header.className = 'path-panel-header'
    header.style.cssText = `
      padding: 10px 15px;
      background-color: #607D8B;
      color: white;
      font-weight: bold;
      display: flex;
      justify-content: space-between;
      align-items: center;
    `
    header.innerHTML = `
      <span>路径对象</span>
      <button class="close-panel-btn" style="
        background: none;
        border: none;
        color: white;
        font-size: 18px;
        cursor: pointer;
        padding: 0;
        width: 20px;
        height: 20px;
        display: flex;
        align-items: center;
        justify-content: center;
      ">×</button>
    `

    const content = document.createElement('div')
    content.className = 'path-panel-content'
    content.style.cssText = `
      flex: 1;
      overflow-y: auto;
      padding: 10px;
    `

    const objectList = document.createElement('div')
    objectList.className = 'path-object-list'
    objectList.style.cssText = `
      display: flex;
      flex-direction: column;
      gap: 8px;
    `

    content.appendChild(objectList)
    this.panelElement.appendChild(header)
    this.panelElement.appendChild(content)
    container.appendChild(this.panelElement)

    const closeBtn = header.querySelector('.close-panel-btn') as HTMLElement
    closeBtn.addEventListener('click', () => {
      this.togglePanel()
    })
  }

  private addEventListeners(): void {
    if (!this.panelElement) return

    const objectList = this.panelElement.querySelector('.path-object-list') as HTMLElement
    if (!objectList) return

    objectList.addEventListener('dragover', (e) => {
      e.preventDefault()
      const target = e.target as HTMLElement
      const item = target.closest('.path-object-item') as HTMLElement
      if (item) {
        item.style.borderTop = '2px solid #2196F3'
      }
    })

    objectList.addEventListener('dragleave', (e) => {
      const target = e.target as HTMLElement
      const item = target.closest('.path-object-item') as HTMLElement
      if (item) {
        item.style.borderTop = 'none'
      }
    })

    objectList.addEventListener('drop', (e) => {
      e.preventDefault()
      const target = e.target as HTMLElement
      const item = target.closest('.path-object-item') as HTMLElement
      if (item) {
        item.style.borderTop = 'none'
        const draggedId = (e as DragEvent).dataTransfer?.getData('text/plain')
        if (draggedId) {
          this.moveObject(draggedId, item.dataset.id || '')
        }
      }
    })
  }

  private togglePanel(): void {
    this.panelVisible = !this.panelVisible

    if (this.panelElement) {
      if (this.panelVisible) {
        this.panelElement.style.display = 'flex'
        setTimeout(() => {
          this.panelElement!.style.opacity = '1'
          this.panelElement!.style.transform = 'translateY(0)'
        }, 10)
      } else {
        this.panelElement.style.opacity = '0'
        this.panelElement.style.transform = 'translateY(10px)'
        setTimeout(() => {
          this.panelElement!.style.display = 'none'
        }, 300)
      }
    }

    if (this.toggleButton) {
      this.toggleButton.innerHTML = this.panelVisible ? '▼' : '◀'
    }
  }

  addObject(object: THREE.Object3D): void {
    const bottomCenter = this.calculateBottomCenter(object)
    const pathObject: PathObject = {
      id: object.uuid,
      object,
      name: object.name || '未命名对象',
      bottomCenter
    }

    this.objects.push(pathObject)
    this.updatePanel()
    this.updatePath()
  }

  removeObject(objectId: string): void {
    const index = this.objects.findIndex(obj => obj.id === objectId)
    if (index !== -1) {
      this.objects.splice(index, 1)
      this.updatePanel()
      this.updatePath()
    }
  }

  updateObject(objectId: string): void {
    const pathObject = this.objects.find(obj => obj.id === objectId)
    if (pathObject) {
      pathObject.bottomCenter = this.calculateBottomCenter(pathObject.object)
      this.updatePath()
    }
  }

  clearObjects(): void {
    this.objects = []
    this.updatePanel()
    this.clearPath()
  }

  private calculateBottomCenter(object: THREE.Object3D): THREE.Vector3 {
    const box = new THREE.Box3().setFromObject(object)
    const center = new THREE.Vector3()
    box.getCenter(center)
    center.y = box.min.y
    return center
  }

  private moveObject(draggedId: string, targetId: string): void {
    if (draggedId === targetId) return

    const draggedIndex = this.objects.findIndex(obj => obj.id === draggedId)
    const targetIndex = this.objects.findIndex(obj => obj.id === targetId)

    if (draggedIndex !== -1 && targetIndex !== -1) {
      const [draggedObject] = this.objects.splice(draggedIndex, 1)
      this.objects.splice(targetIndex, 0, draggedObject)
      this.updatePanel()
      this.updatePath()
    }
  }

  private updatePanel(): void {
    const objectList = this.panelElement?.querySelector('.path-object-list') as HTMLElement
    if (!objectList) return

    objectList.innerHTML = ''

    this.objects.forEach((pathObject, index) => {
      const item = document.createElement('div')
      item.className = 'path-object-item'
      item.dataset.id = pathObject.id
      item.draggable = true
      item.style.cssText = `
        padding: 10px;
        background-color: ${this.config.itemBackgroundColor};
        border-radius: 4px;
        cursor: move;
        display: flex;
        align-items: center;
        gap: 10px;
        transition: background-color 0.2s;
      `

      const indexSpan = document.createElement('span')
      indexSpan.style.cssText = `
        font-weight: bold;
        color: #666;
        min-width: 20px;
      `
      indexSpan.textContent = `${index + 1}.`

      const nameSpan = document.createElement('span')
      nameSpan.style.cssText = `
        flex: 1;
        color: ${this.config.textColor};
      `
      nameSpan.textContent = pathObject.name

      const moveUpBtn = document.createElement('button')
      moveUpBtn.innerHTML = '↑'
      moveUpBtn.style.cssText = `
        background: none;
        border: 1px solid #ccc;
        border-radius: 3px;
        cursor: pointer;
        padding: 2px 6px;
        font-size: 12px;
      `
      moveUpBtn.addEventListener('click', () => {
        this.moveObjectUp(pathObject.id)
      })

      const moveDownBtn = document.createElement('button')
      moveDownBtn.innerHTML = '↓'
      moveDownBtn.style.cssText = `
        background: none;
        border: 1px solid #ccc;
        border-radius: 3px;
        cursor: pointer;
        padding: 2px 6px;
        font-size: 12px;
      `
      moveDownBtn.addEventListener('click', () => {
        this.moveObjectDown(pathObject.id)
      })

      item.appendChild(indexSpan)
      item.appendChild(nameSpan)
      item.appendChild(moveUpBtn)
      item.appendChild(moveDownBtn)

      item.addEventListener('dragstart', (e) => {
        (e as DragEvent).dataTransfer?.setData('text/plain', pathObject.id)
        item.style.opacity = '0.5'
      })

      item.addEventListener('dragend', () => {
        item.style.opacity = '1'
      })

      item.addEventListener('mouseenter', () => {
        item.style.backgroundColor = this.config.itemHoverColor
      })

      item.addEventListener('mouseleave', () => {
        item.style.backgroundColor = this.config.itemBackgroundColor
      })

      objectList.appendChild(item)
    })
  }

  private moveObjectUp(objectId: string): void {
    const index = this.objects.findIndex(obj => obj.id === objectId)
    if (index > 0) {
      const [object] = this.objects.splice(index, 1)
      this.objects.splice(index - 1, 0, object)
      this.updatePanel()
      this.updatePath()
    }
  }

  private moveObjectDown(objectId: string): void {
    const index = this.objects.findIndex(obj => obj.id === objectId)
    if (index < this.objects.length - 1) {
      const [object] = this.objects.splice(index, 1)
      this.objects.splice(index + 1, 0, object)
      this.updatePanel()
      this.updatePath()
    }
  }

  private updatePath(): void {
    if (this.currentPathId) {
      pathManager.removePathById(this.currentPathId)
    }

    if (this.objects.length < 2) {
      this.currentPathId = ''
      return
    }

    const points = this.objects.map(obj => obj.bottomCenter)
    const config: PathConfig = {
      points,
      width: 0.3,
      cornerRadius: 0.2,
      cornerSplit: 10,
      color: 0x2196F3,
      transparent: true,
      opacity: 0.8,
      arrow: true
    }

    this.currentPathId = pathManager.createPath(config)

    if (this.onPathUpdateCallback) {
      this.onPathUpdateCallback(this.currentPathId)
    }
  }

  private clearPath(): void {
    if (this.currentPathId) {
      pathManager.removePathById(this.currentPathId)
      this.currentPathId = ''
    }
  }

  onPathUpdate(callback: (pathId: string) => void): void {
    this.onPathUpdateCallback = callback
  }

  getObjects(): PathObject[] {
    return [...this.objects]
  }

  isVisible(): boolean {
    return this.panelVisible
  }

  show(): void {
    if (!this.panelVisible) {
      this.togglePanel()
    }
  }

  hide(): void {
    if (this.panelVisible) {
      this.togglePanel()
    }
  }

  dispose(): void {
    this.clearPath()
    if (this.panelElement && this.panelElement.parentNode) {
      this.panelElement.parentNode.removeChild(this.panelElement)
    }
    if (this.toggleButton && this.toggleButton.parentNode) {
      this.toggleButton.parentNode.removeChild(this.toggleButton)
    }
    this.objects = []
  }
}

const pathPanelManager = new PathPanelManager()

export { PathPanelManager, pathPanelManager }
export type { PathObject, PathPanelConfig }
