import * as THREE from 'three'
import { pathManager, type PathConfig, type PathMesh } from './path'

export interface PathObject {
  id: string
  object: THREE.Object3D
  name: string
  bottomCenter: THREE.Vector3
  selected: boolean
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

export interface PathInfo {
  id: string
  name: string
  objectIds: string[]
  config: PathConfig
}

class PathPanelManager {
  private objects: PathObject[] = []
  private paths: PathInfo[] = []
  private panelVisible: boolean = false
  private panelElement: HTMLElement | null = null
  private toggleButton: HTMLElement | null = null
  private config: Required<PathPanelConfig>
  private selectedPathId: string = ''
  private onPathUpdateCallback: ((pathId: string) => void) | null = null

  constructor(config: PathPanelConfig = {}) {
    this.config = {
      width: config.width || 320,
      height: config.height || 500,
      position: config.position || { x: 10, y: 10 },
      backgroundColor: config.backgroundColor || 'rgba(255, 255, 255, 0.95)',
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
      <span>路径管理</span>
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
      display: flex;
      flex-direction: column;
      gap: 10px;
    `

    const tabs = document.createElement('div')
    tabs.className = 'path-panel-tabs'
    tabs.style.cssText = `
      display: flex;
      gap: 5px;
      margin-bottom: 10px;
    `
    tabs.innerHTML = `
      <button class="tab-btn" data-tab="objects" style="
        flex: 1;
        padding: 8px;
        background-color: #2196F3;
        color: white;
        border: none;
        border-radius: 4px;
        cursor: pointer;
        font-weight: bold;
      ">对象列表</button>
      <button class="tab-btn" data-tab="paths" style="
        flex: 1;
        padding: 8px;
        background-color: #9E9E9E;
        color: white;
        border: none;
        border-radius: 4px;
        cursor: pointer;
        font-weight: bold;
      ">路径列表</button>
    `

    const objectsPanel = document.createElement('div')
    objectsPanel.className = 'objects-panel'
    objectsPanel.style.cssText = `
      display: flex;
      flex-direction: column;
      gap: 10px;
    `

    const objectsHeader = document.createElement('div')
    objectsHeader.className = 'objects-header'
    objectsHeader.style.cssText = `
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 10px;
    `
    objectsHeader.innerHTML = `
      <button class="select-all-btn" style="
        padding: 6px 12px;
        background-color: #4CAF50;
        color: white;
        border: none;
        border-radius: 4px;
        cursor: pointer;
        font-size: 12px;
      ">全选</button>
      <button class="deselect-all-btn" style="
        padding: 6px 12px;
        background-color: #f44336;
        color: white;
        border: none;
        border-radius: 4px;
        cursor: pointer;
        font-size: 12px;
      ">取消全选</button>
      <button class="generate-path-btn" style="
        padding: 6px 12px;
        background-color: #2196F3;
        color: white;
        border: none;
        border-radius: 4px;
        cursor: pointer;
        font-weight: bold;
        font-size: 12px;
      ">生成路径</button>
    `

    const objectList = document.createElement('div')
    objectList.className = 'path-object-list'
    objectList.style.cssText = `
      display: flex;
      flex-direction: column;
      gap: 8px;
      max-height: 300px;
      overflow-y: auto;
    `

    objectsPanel.appendChild(objectsHeader)
    objectsPanel.appendChild(objectList)

    const pathsPanel = document.createElement('div')
    pathsPanel.className = 'paths-panel'
    pathsPanel.style.cssText = `
      display: none;
      flex-direction: column;
      gap: 10px;
    `

    const pathList = document.createElement('div')
    pathList.className = 'path-list'
    pathList.style.cssText = `
      display: flex;
      flex-direction: column;
      gap: 8px;
      max-height: 200px;
      overflow-y: auto;
    `

    const pathConfigPanel = document.createElement('div')
    pathConfigPanel.className = 'path-config-panel'
    pathConfigPanel.style.cssText = `
      display: none;
      flex-direction: column;
      gap: 10px;
      padding: 10px;
      background-color: #f9f9f9;
      border-radius: 4px;
      border: 1px solid #ddd;
    `

    pathsPanel.appendChild(pathList)
    pathsPanel.appendChild(pathConfigPanel)

    content.appendChild(tabs)
    content.appendChild(objectsPanel)
    content.appendChild(pathsPanel)
    this.panelElement.appendChild(header)
    this.panelElement.appendChild(content)
    container.appendChild(this.panelElement)

    const closeBtn = header.querySelector('.close-panel-btn') as HTMLElement
    closeBtn.addEventListener('click', () => {
      this.togglePanel()
    })

    const tabButtons = tabs.querySelectorAll('.tab-btn')
    tabButtons.forEach(btn => {
      btn.addEventListener('click', () => {
        const tab = (btn as HTMLElement).dataset.tab
        this.switchTab(tab || 'objects')
      })
    })

    const selectAllBtn = objectsHeader.querySelector('.select-all-btn') as HTMLElement
    selectAllBtn.addEventListener('click', () => {
      this.selectAllObjects()
    })

    const deselectAllBtn = objectsHeader.querySelector('.deselect-all-btn') as HTMLElement
    deselectAllBtn.addEventListener('click', () => {
      this.deselectAllObjects()
    })

    const generatePathBtn = objectsHeader.querySelector('.generate-path-btn') as HTMLElement
    generatePathBtn.addEventListener('click', () => {
      this.generatePathFromSelectedObjects()
    })
  }

  private switchTab(tab: string): void {
    const tabButtons = this.panelElement?.querySelectorAll('.tab-btn')
    const objectsPanel = this.panelElement?.querySelector('.objects-panel') as HTMLElement
    const pathsPanel = this.panelElement?.querySelector('.paths-panel') as HTMLElement

    tabButtons?.forEach(btn => {
      const btnTab = (btn as HTMLElement).dataset.tab
      if (btnTab === tab) {
        (btn as HTMLElement).style.backgroundColor = '#2196F3'
      } else {
        (btn as HTMLElement).style.backgroundColor = '#9E9E9E'
      }
    })

    if (objectsPanel && pathsPanel) {
      if (tab === 'objects') {
        objectsPanel.style.display = 'flex'
        pathsPanel.style.display = 'none'
      } else {
        objectsPanel.style.display = 'none'
        pathsPanel.style.display = 'flex'
      }
    }
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
      bottomCenter,
      selected: false
    }

    this.objects.push(pathObject)
    this.updateObjectList()
  }

  removeObject(objectId: string): void {
    const index = this.objects.findIndex(obj => obj.id === objectId)
    if (index !== -1) {
      this.objects.splice(index, 1)
      this.updateObjectList()
    }
  }

  updateObject(objectId: string): void {
    const pathObject = this.objects.find(obj => obj.id === objectId)
    if (pathObject) {
      pathObject.bottomCenter = this.calculateBottomCenter(pathObject.object)
      
      const paths = pathManager.findPathsByObjectId(objectId)
      paths.forEach(path => {
        this.updatePathPoints(path.id)
      })
    }
  }

  clearObjects(): void {
    this.objects = []
    this.updateObjectList()
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
      this.updateObjectList()
    }
  }

  private updateObjectList(): void {
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
        background-color: ${pathObject.selected ? '#E3F2FD' : this.config.itemBackgroundColor};
        border-radius: 4px;
        cursor: move;
        display: flex;
        align-items: center;
        gap: 10px;
        transition: background-color 0.2s;
        border: ${pathObject.selected ? '2px solid #2196F3' : 'none'};
      `

      const checkbox = document.createElement('input')
      checkbox.type = 'checkbox'
      checkbox.checked = pathObject.selected
      checkbox.style.cssText = `
        cursor: pointer;
        width: 16px;
        height: 16px;
      `
      checkbox.addEventListener('change', () => {
        pathObject.selected = checkbox.checked
        item.style.backgroundColor = pathObject.selected ? '#E3F2FD' : this.config.itemBackgroundColor
        item.style.border = pathObject.selected ? '2px solid #2196F3' : 'none'
      })

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

      item.appendChild(checkbox)
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
        item.style.backgroundColor = pathObject.selected ? '#E3F2FD' : this.config.itemHoverColor
      })

      item.addEventListener('mouseleave', () => {
        item.style.backgroundColor = pathObject.selected ? '#E3F2FD' : this.config.itemBackgroundColor
      })

      objectList.appendChild(item)
    })
  }

  private selectAllObjects(): void {
    this.objects.forEach(obj => obj.selected = true)
    this.updateObjectList()
  }

  private deselectAllObjects(): void {
    this.objects.forEach(obj => obj.selected = false)
    this.updateObjectList()
  }

  private moveObjectUp(objectId: string): void {
    const index = this.objects.findIndex(obj => obj.id === objectId)
    if (index > 0) {
      const [object] = this.objects.splice(index, 1)
      this.objects.splice(index - 1, 0, object)
      this.updateObjectList()
    }
  }

  private moveObjectDown(objectId: string): void {
    const index = this.objects.findIndex(obj => obj.id === objectId)
    if (index < this.objects.length - 1) {
      const [object] = this.objects.splice(index, 1)
      this.objects.splice(index + 1, 0, object)
      this.updateObjectList()
    }
  }

  private generatePathFromSelectedObjects(): void {
    const selectedObjects = this.objects.filter(obj => obj.selected)
    
    if (selectedObjects.length < 2) {
      alert('请至少选择2个对象来生成路径')
      return
    }

    const points = selectedObjects.map(obj => obj.bottomCenter)
    const objectIds = selectedObjects.map(obj => obj.id)
    const defaultName = selectedObjects.map(obj => obj.name).join(' -> ')

    console.log('[PathPanel] 生成路径，点数:', points.length)
    console.log('[PathPanel] 路径点:', points)

    const config: PathConfig = {
      points,
      width: 0.2,
      cornerRadius: 1,
      cornerSplit: 10,
      color: 0x58dede,
      transparent: true,
      opacity: 0.9,
      blending: THREE.NormalBlending,
      side: THREE.FrontSide,
      arrow: false,
      texture: undefined,
      useTexture: true,
      scrollUV: true,
      scrollSpeed: 0.03,
      progress: 1,
      playSpeed: 0.14,
      speed: 0.48,
      parallelToXZ: true
    }

    const pathId = pathManager.createPath(config, defaultName, objectIds)
    
    console.log('[PathPanel] 创建的路径ID:', pathId)
    
    if (pathId) {
      const pathInfo: PathInfo = {
        id: pathId,
        name: defaultName,
        objectIds,
        config
      }
      
      this.paths.push(pathInfo)
      this.updatePathList()
      this.switchTab('paths')
      
      if (this.onPathUpdateCallback) {
        this.onPathUpdateCallback(pathId)
      }
    }
  }

  private updatePathPoints(pathId: string): void {
    const pathInfo = this.paths.find(p => p.id === pathId)
    if (!pathInfo) return

    const points = pathInfo.objectIds.map(objId => {
      const pathObject = this.objects.find(obj => obj.id === objId)
      return pathObject ? pathObject.bottomCenter : new THREE.Vector3()
    })

    pathManager.removePathById(pathId)
    pathManager.createPath({ ...pathInfo.config, points }, pathInfo.name, pathInfo.objectIds)
  }

  private updatePathList(): void {
    const pathList = this.panelElement?.querySelector('.path-list') as HTMLElement
    if (!pathList) return

    pathList.innerHTML = ''

    this.paths.forEach(pathInfo => {
      const item = document.createElement('div')
      item.className = 'path-list-item'
      item.dataset.id = pathInfo.id
      item.style.cssText = `
        padding: 10px;
        background-color: ${this.selectedPathId === pathInfo.id ? '#E3F2FD' : this.config.itemBackgroundColor};
        border-radius: 4px;
        cursor: pointer;
        display: flex;
        align-items: center;
        gap: 10px;
        transition: background-color 0.2s;
        border: ${this.selectedPathId === pathInfo.id ? '2px solid #2196F3' : 'none'};
      `

      const nameInput = document.createElement('input')
      nameInput.type = 'text'
      nameInput.value = pathInfo.name
      nameInput.style.cssText = `
        flex: 1;
        padding: 5px;
        border: 1px solid #ccc;
        border-radius: 3px;
        font-size: 12px;
      `
      nameInput.addEventListener('change', () => {
        pathInfo.name = nameInput.value
        pathManager.updatePathName(pathInfo.id, pathInfo.name)
      })
      nameInput.addEventListener('click', (e) => {
        e.stopPropagation()
      })

      const deleteBtn = document.createElement('button')
      deleteBtn.innerHTML = '删除'
      deleteBtn.style.cssText = `
        padding: 5px 10px;
        background-color: #f44336;
        color: white;
        border: none;
        border-radius: 3px;
        cursor: pointer;
        font-size: 12px;
      `
      deleteBtn.addEventListener('click', (e) => {
        e.stopPropagation()
        this.deletePath(pathInfo.id)
      })

      item.appendChild(nameInput)
      item.appendChild(deleteBtn)

      item.addEventListener('click', () => {
        this.selectPath(pathInfo.id)
      })

      item.addEventListener('mouseenter', () => {
        item.style.backgroundColor = this.selectedPathId === pathInfo.id ? '#E3F2FD' : this.config.itemHoverColor
      })

      item.addEventListener('mouseleave', () => {
        item.style.backgroundColor = this.selectedPathId === pathInfo.id ? '#E3F2FD' : this.config.itemBackgroundColor
      })

      pathList.appendChild(item)
    })
  }

  private selectPath(pathId: string): void {
    this.selectedPathId = pathId
    this.updatePathList()
    this.showPathConfig(pathId)
  }

  private showPathConfig(pathId: string): void {
    const pathConfigPanel = this.panelElement?.querySelector('.path-config-panel') as HTMLElement
    if (!pathConfigPanel) return

    const pathInfo = this.paths.find(p => p.id === pathId)
    if (!pathInfo) {
      pathConfigPanel.style.display = 'none'
      return
    }

    pathConfigPanel.style.display = 'flex'

    const config = pathInfo.config
    const texturePresets = pathManager.getTexturePresets()

    const blendingOptions = [
      { value: THREE.NormalBlending, label: 'Normal' },
      { value: THREE.AdditiveBlending, label: 'Additive' },
      { value: THREE.SubtractiveBlending, label: 'Subtractive' },
      { value: THREE.MultiplyBlending, label: 'Multiply' }
    ]

    const sideOptions = [
      { value: THREE.FrontSide, label: 'Front' },
      { value: THREE.BackSide, label: 'Back' },
      { value: THREE.DoubleSide, label: 'Double' }
    ]

    const wrapOptions = [
      { value: THREE.ClampToEdgeWrapping, label: 'Clamp' },
      { value: THREE.RepeatWrapping, label: 'Repeat' },
      { value: THREE.MirroredRepeatWrapping, label: 'Mirror' }
    ]

    pathConfigPanel.innerHTML = `
      <h4 style="margin: 0 0 10px 0; font-size: 14px;">路径配置</h4>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">宽度:</label>
        <input type="number" step="0.1" min="0.1" value="${config.width}" 
          class="config-width" style="flex: 1; padding: 5px; border: 1px solid #ccc; border-radius: 3px;">
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">圆角半径:</label>
        <input type="number" step="0.1" min="0" value="${config.cornerRadius || 0}" 
          class="config-cornerRadius" style="flex: 1; padding: 5px; border: 1px solid #ccc; border-radius: 3px;">
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">圆角分段:</label>
        <input type="number" step="1" min="0" value="${config.cornerSplit || 0}" 
          class="config-cornerSplit" style="flex: 1; padding: 5px; border: 1px solid #ccc; border-radius: 3px;">
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">颜色:</label>
        <input type="color" value="#${config.color?.toString(16).padStart(6, '0') || 'ffffff'}" 
          class="config-color" style="width: 50px; height: 30px; border: 1px solid #ccc; border-radius: 3px;">
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">透明:</label>
        <input type="checkbox" ${config.transparent ? 'checked' : ''} class="config-transparent" style="width: 16px; height: 16px;">
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">透明度:</label>
        <input type="range" min="0" max="1" step="0.1" value="${config.opacity || 1}" 
          class="config-opacity" style="flex: 1;">
        <span class="opacity-value" style="font-size: 12px; min-width: 30px;">${(config.opacity || 1).toFixed(1)}</span>
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">混合模式:</label>
        <select class="config-blending" style="flex: 1; padding: 5px; border: 1px solid #ccc; border-radius: 3px;">
          ${blendingOptions.map(opt => `<option value="${opt.value}" ${config.blending === opt.value ? 'selected' : ''}>${opt.label}</option>`).join('')}
        </select>
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">渲染面:</label>
        <select class="config-side" style="flex: 1; padding: 5px; border: 1px solid #ccc; border-radius: 3px;">
          ${sideOptions.map(opt => `<option value="${opt.value}" ${config.side === opt.value ? 'selected' : ''}>${opt.label}</option>`).join('')}
        </select>
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">速度:</label>
        <input type="range" min="0" max="2" step="0.01" value="${config.speed || 0.02}" 
          class="config-speed" style="flex: 1;">
        <span class="speed-value" style="font-size: 12px; min-width: 30px;">${(config.speed || 0.02).toFixed(2)}</span>
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">箭头:</label>
        <input type="checkbox" ${config.arrow ? 'checked' : ''} class="config-arrow" style="width: 16px; height: 16px;">
      </div>
      
      <hr style="margin: 10px 0; border: none; border-top: 1px solid #ddd;">
      
      <h4 style="margin: 0 0 10px 0; font-size: 14px;">纹理配置</h4>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">使用纹理:</label>
        <input type="checkbox" ${config.useTexture ? 'checked' : ''} class="config-useTexture" style="width: 16px; height: 16px;">
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">滚动UV:</label>
        <input type="checkbox" ${config.scrollUV ? 'checked' : ''} class="config-scrollUV" style="width: 16px; height: 16px;">
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">滚动速度:</label>
        <input type="range" min="-0.1" max="0.1" step="0.001" value="${config.scrollSpeed || 0.03}" 
          class="config-scrollSpeed" style="flex: 1;">
        <span class="scrollSpeed-value" style="font-size: 12px; min-width: 30px;">${(config.scrollSpeed || 0.03).toFixed(3)}</span>
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">进度:</label>
        <input type="range" min="0" max="1" step="0.01" value="${config.progress || 1}" 
          class="config-progress" style="flex: 1;">
        <span class="progress-value" style="font-size: 12px; min-width: 30px;">${(config.progress || 1).toFixed(2)}</span>
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">播放速度:</label>
        <input type="range" min="0.01" max="0.2" step="0.01" value="${config.playSpeed || 0.14}" 
          class="config-playSpeed" style="flex: 1;">
        <span class="playSpeed-value" style="font-size: 12px; min-width: 30px;">${(config.playSpeed || 0.14).toFixed(2)}</span>
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">平行XZ平面:</label>
        <input type="checkbox" ${config.parallelToXZ ? 'checked' : ''} class="config-parallelToXZ" style="width: 16px; height: 16px;">
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">纹理:</label>
        <select class="config-texture" style="flex: 1; padding: 5px; border: 1px solid #ccc; border-radius: 3px;">
          <option value="none">无纹理</option>
          ${Array.from(texturePresets.entries())
            .filter(([key]) => key !== 'none')
            .map(([key, value]) => `<option value="${key}">${value.name}</option>`)
            .join('')}
        </select>
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">S轴包裹:</label>
        <select class="config-wrapS" style="flex: 1; padding: 5px; border: 1px solid #ccc; border-radius: 3px;">
          ${wrapOptions.map(opt => `<option value="${opt.value}">${opt.label}</option>`).join('')}
        </select>
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">T轴包裹:</label>
        <select class="config-wrapT" style="flex: 1; padding: 5px; border: 1px solid #ccc; border-radius: 3px;">
          ${wrapOptions.map(opt => `<option value="${opt.value}">${opt.label}</option>`).join('')}
        </select>
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">X重复:</label>
        <input type="number" step="0.1" min="0.1" value="1" 
          class="config-repeatX" style="flex: 1; padding: 5px; border: 1px solid #ccc; border-radius: 3px;">
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">Y重复:</label>
        <input type="number" step="0.1" min="0.1" value="1" 
          class="config-repeatY" style="flex: 1; padding: 5px; border: 1px solid #ccc; border-radius: 3px;">
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">X偏移:</label>
        <input type="number" step="0.1" value="0" 
          class="config-offsetX" style="flex: 1; padding: 5px; border: 1px solid #ccc; border-radius: 3px;">
      </div>
      
      <div style="display: flex; align-items: center; gap: 10px;">
        <label style="font-size: 12px; min-width: 80px;">Y偏移:</label>
        <input type="number" step="0.1" value="0" 
          class="config-offsetY" style="flex: 1; padding: 5px; border: 1px solid #ccc; border-radius: 3px;">
      </div>
    `

    const widthInput = pathConfigPanel.querySelector('.config-width') as HTMLInputElement
    widthInput.addEventListener('change', () => {
      const width = parseFloat(widthInput.value)
      pathInfo.config.width = width
      pathManager.updatePathWidthById(pathId, width)
    })

    const cornerRadiusInput = pathConfigPanel.querySelector('.config-cornerRadius') as HTMLInputElement
    cornerRadiusInput.addEventListener('change', () => {
      const cornerRadius = parseFloat(cornerRadiusInput.value)
      pathInfo.config.cornerRadius = cornerRadius
      pathManager.updatePathConfig(pathId, { cornerRadius })
    })

    const cornerSplitInput = pathConfigPanel.querySelector('.config-cornerSplit') as HTMLInputElement
    cornerSplitInput.addEventListener('change', () => {
      const cornerSplit = parseFloat(cornerSplitInput.value)
      pathInfo.config.cornerSplit = cornerSplit
      pathManager.updatePathConfig(pathId, { cornerSplit })
    })

    const colorInput = pathConfigPanel.querySelector('.config-color') as HTMLInputElement
    colorInput.addEventListener('change', () => {
      const color = parseInt(colorInput.value.replace('#', ''), 16)
      pathInfo.config.color = color
      pathManager.updatePathColorById(pathId, color)
    })

    const transparentInput = pathConfigPanel.querySelector('.config-transparent') as HTMLInputElement
    transparentInput.addEventListener('change', () => {
      const transparent = transparentInput.checked
      pathInfo.config.transparent = transparent
      pathManager.updatePathConfig(pathId, { transparent })
    })

    const opacityInput = pathConfigPanel.querySelector('.config-opacity') as HTMLInputElement
    const opacityValue = pathConfigPanel.querySelector('.opacity-value') as HTMLElement
    opacityInput.addEventListener('input', () => {
      const opacity = parseFloat(opacityInput.value)
      opacityValue.textContent = opacity.toFixed(1)
      pathInfo.config.opacity = opacity
      pathManager.updatePathConfig(pathId, { opacity })
    })

    const blendingInput = pathConfigPanel.querySelector('.config-blending') as HTMLSelectElement
    blendingInput.addEventListener('change', () => {
      const blending = parseInt(blendingInput.value) as THREE.Blending
      pathInfo.config.blending = blending
      pathManager.updatePathConfig(pathId, { blending })
    })

    const sideInput = pathConfigPanel.querySelector('.config-side') as HTMLSelectElement
    sideInput.addEventListener('change', () => {
      const side = parseInt(sideInput.value) as THREE.Side
      pathInfo.config.side = side
      pathManager.updatePathConfig(pathId, { side })
    })

    const speedInput = pathConfigPanel.querySelector('.config-speed') as HTMLInputElement
    const speedValue = pathConfigPanel.querySelector('.speed-value') as HTMLElement
    speedInput.addEventListener('input', () => {
      const speed = parseFloat(speedInput.value)
      speedValue.textContent = speed.toFixed(2)
      pathInfo.config.speed = speed
      pathManager.setPathSpeedById(pathId, speed)
    })

    const arrowInput = pathConfigPanel.querySelector('.config-arrow') as HTMLInputElement
    arrowInput.addEventListener('change', () => {
      const arrow = arrowInput.checked
      pathInfo.config.arrow = arrow
      pathManager.updatePathConfig(pathId, { arrow })
    })

    const textureInput = pathConfigPanel.querySelector('.config-texture') as HTMLSelectElement
    textureInput.addEventListener('change', () => {
      const preset = textureInput.value as any
      const texture = pathManager.loadTextureFromPreset(preset)
      pathInfo.config.texture = texture || undefined
      pathManager.updatePathConfig(pathId, { texture: texture || undefined })
    })

    const useTextureInput = pathConfigPanel.querySelector('.config-useTexture') as HTMLInputElement
    useTextureInput.addEventListener('change', () => {
      const useTexture = useTextureInput.checked
      pathInfo.config.useTexture = useTexture
      pathManager.updatePathConfig(pathId, { useTexture })
    })

    const scrollUVInput = pathConfigPanel.querySelector('.config-scrollUV') as HTMLInputElement
    scrollUVInput.addEventListener('change', () => {
      const scrollUV = scrollUVInput.checked
      pathInfo.config.scrollUV = scrollUV
      pathManager.updatePathConfig(pathId, { scrollUV })
    })

    const scrollSpeedInput = pathConfigPanel.querySelector('.config-scrollSpeed') as HTMLInputElement
    const scrollSpeedValue = pathConfigPanel.querySelector('.scrollSpeed-value') as HTMLElement
    scrollSpeedInput.addEventListener('input', () => {
      const scrollSpeed = parseFloat(scrollSpeedInput.value)
      scrollSpeedValue.textContent = scrollSpeed.toFixed(3)
      pathInfo.config.scrollSpeed = scrollSpeed
      pathManager.updatePathConfig(pathId, { scrollSpeed })
    })

    const progressInput = pathConfigPanel.querySelector('.config-progress') as HTMLInputElement
    const progressValue = pathConfigPanel.querySelector('.progress-value') as HTMLElement
    progressInput.addEventListener('input', () => {
      const progress = parseFloat(progressInput.value)
      progressValue.textContent = progress.toFixed(2)
      pathInfo.config.progress = progress
      pathManager.updatePathConfig(pathId, { progress })
    })

    const playSpeedInput = pathConfigPanel.querySelector('.config-playSpeed') as HTMLInputElement
    const playSpeedValue = pathConfigPanel.querySelector('.playSpeed-value') as HTMLElement
    playSpeedInput.addEventListener('input', () => {
      const playSpeed = parseFloat(playSpeedInput.value)
      playSpeedValue.textContent = playSpeed.toFixed(2)
      pathInfo.config.playSpeed = playSpeed
      pathManager.updatePathConfig(pathId, { playSpeed })
    })

    const parallelToXZInput = pathConfigPanel.querySelector('.config-parallelToXZ') as HTMLInputElement
    parallelToXZInput.addEventListener('change', () => {
      const parallelToXZ = parallelToXZInput.checked
      pathInfo.config.parallelToXZ = parallelToXZ
      pathManager.updatePathConfig(pathId, { parallelToXZ })
    })

    const wrapSInput = pathConfigPanel.querySelector('.config-wrapS') as HTMLSelectElement
    wrapSInput.addEventListener('change', () => {
      const wrapS = parseInt(wrapSInput.value) as THREE.Wrapping
      pathManager.updatePathTextureWrap(pathId, wrapS, null)
    })

    const wrapTInput = pathConfigPanel.querySelector('.config-wrapT') as HTMLSelectElement
    wrapTInput.addEventListener('change', () => {
      const wrapT = parseInt(wrapTInput.value) as THREE.Wrapping
      pathManager.updatePathTextureWrap(pathId, null, wrapT)
    })

    const repeatXInput = pathConfigPanel.querySelector('.config-repeatX') as HTMLInputElement
    repeatXInput.addEventListener('change', () => {
      const repeatX = parseFloat(repeatXInput.value)
      pathManager.updatePathTextureRepeat(pathId, repeatX, null)
    })

    const repeatYInput = pathConfigPanel.querySelector('.config-repeatY') as HTMLInputElement
    repeatYInput.addEventListener('change', () => {
      const repeatY = parseFloat(repeatYInput.value)
      pathManager.updatePathTextureRepeat(pathId, null, repeatY)
    })

    const offsetXInput = pathConfigPanel.querySelector('.config-offsetX') as HTMLInputElement
    offsetXInput.addEventListener('change', () => {
      const offsetX = parseFloat(offsetXInput.value)
      pathManager.updatePathTextureOffset(pathId, offsetX, null)
    })

    const offsetYInput = pathConfigPanel.querySelector('.config-offsetY') as HTMLInputElement
    offsetYInput.addEventListener('change', () => {
      const offsetY = parseFloat(offsetYInput.value)
      pathManager.updatePathTextureOffset(pathId, null, offsetY)
    })
  }

  private deletePath(pathId: string): void {
    pathManager.removePathById(pathId)
    const index = this.paths.findIndex(p => p.id === pathId)
    if (index !== -1) {
      this.paths.splice(index, 1)
    }
    
    if (this.selectedPathId === pathId) {
      this.selectedPathId = ''
      const pathConfigPanel = this.panelElement?.querySelector('.path-config-panel') as HTMLElement
      if (pathConfigPanel) {
        pathConfigPanel.style.display = 'none'
      }
    }
    
    this.updatePathList()
  }

  onPathUpdate(callback: (pathId: string) => void): void {
    this.onPathUpdateCallback = callback
  }

  getObjects(): PathObject[] {
    return [...this.objects]
  }

  getPaths(): PathInfo[] {
    return [...this.paths]
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

  update(delta: number): void {
    pathManager.update(delta)
  }

  dispose(): void {
    this.paths.forEach(pathInfo => {
      pathManager.removePathById(pathInfo.id)
    })
    this.paths = []
    
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
export type { PathObject, PathPanelConfig, PathInfo }
