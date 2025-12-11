<template>
  <div class="canvas-container" ref="canvasContainer"></div>
  
  <div class="toolbar">
    <h3>工具栏</h3>
    <button @click="addBasicShape('cube')">添加方块</button>
    <button @click="addBasicShape('sphere')">添加球体</button>
    <button @click="addBasicShape('cylinder')">添加圆柱</button>
    <button @click="addBasicShape('torus')">添加环</button>
    <button @click="triggerFileInput">添加GLB模型</button>
    <input type="file" ref="fileInput" @change="handleFileUpload" accept=".glb,.gltf" />
    <button @click="exportScene">导出场景</button>
    <button @click="triggerImportInput">导入场景</button>
    <input type="file" ref="importInput" @change="handleImportScene" accept=".json" />
  </div>
  
  <div v-if="selectedObject" class="properties-panel">
    <h3>物体属性</h3>
    <div class="property-item">
      <span class="property-label">名称:</span>
      <span class="property-value">{{ selectedObject.name || '未命名' }}</span>
    </div>
    <div class="property-item">
      <span class="property-label">位置:</span>
      <span class="property-value">{{ formatVector(selectedObject.position) }}</span>
    </div>
    <div class="property-item">
      <span class="property-label">旋转:</span>
      <span class="property-value">{{ formatVector(selectedObject.rotation) }}</span>
    </div>
    <div class="property-item">
      <span class="property-label">缩放:</span>
      <span class="property-value">{{ formatVector(selectedObject.scale) }}</span>
    </div>
  </div>
  
  <div v-if="contextMenuVisible" class="context-menu" :style="{ left: contextMenuPosition.x + 'px', top: contextMenuPosition.y + 'px' }">
    <div class="context-menu-item" @click="handleDeleteObject">删除</div>
    <div v-if="transformControls.object === selectedObject" class="context-menu-item" @click="handleExitEditMode">取消编辑模式</div>
    <div v-else class="context-menu-item" @click="handleEnterEditMode">启动编辑模式</div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, nextTick } from 'vue'
import * as THREE from 'three'
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls'
import { TransformControls } from 'three/examples/jsm/controls/TransformControls'
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader'
import { DRACOLoader } from 'three/examples/jsm/loaders/DRACOLoader'

// 响应式变量
const canvasContainer = ref<HTMLElement | null>(null)
const fileInput = ref<HTMLInputElement | null>(null)
const importInput = ref<HTMLInputElement | null>(null)
const selectedObject = ref<THREE.Object3D | null>(null)
const contextMenuVisible = ref(false)
const contextMenuPosition = ref({ x: 0, y: 0 })

// Three.js 变量
let scene: THREE.Scene
let camera: THREE.PerspectiveCamera
let renderer: THREE.WebGLRenderer
let controls: OrbitControls
let transformControls: TransformControls
let raycaster: THREE.Raycaster
let mouse: THREE.Vector2
let ground: THREE.Mesh
let animationId: number

// 初始化Three.js场景
const initThreeJS = () => {
  // 创建场景
  scene = new THREE.Scene()
  scene.background = new THREE.Color(0xf0f0f0)

  // 创建相机
  camera = new THREE.PerspectiveCamera(
    75,
    window.innerWidth / window.innerHeight,
    0.1,
    1000
  )
  camera.position.set(5, 5, 5)

  // 创建渲染器
  renderer = new THREE.WebGLRenderer({ antialias: true })
  renderer.setSize(window.innerWidth, window.innerHeight)
  renderer.shadowMap.enabled = true
  renderer.shadowMap.type = THREE.PCFSoftShadowMap
  canvasContainer.value?.appendChild(renderer.domElement)

  // 创建轨道控制器
  controls = new OrbitControls(camera, renderer.domElement)
  controls.enableDamping = true
  controls.dampingFactor = 0.05
  // 确保控制器始终可用
  controls.enabled = true

  // 创建变换控制器
  transformControls = new TransformControls(camera, renderer.domElement)
  transformControls.addEventListener('dragging-changed', (event) => {
    // 当开始拖拽时禁用轨道控制器，结束时启用
    controls.enabled = !event.value
  })
  transformControls.addEventListener('objectChange', () => {
    // 当对象发生变化时，更新属性面板
    if (selectedObject.value) {
      // 触发Vue的响应式更新
      selectedObject.value = { ...selectedObject.value }
    }
  })
  scene.add(transformControls)

  // 创建地面
  const groundGeometry = new THREE.PlaneGeometry(20, 20)
  const groundMaterial = new THREE.MeshStandardMaterial({ 
    color: 0xcccccc,
    roughness: 0.8,
    metalness: 0.2
  })
  ground = new THREE.Mesh(groundGeometry, groundMaterial)
  ground.rotation.x = -Math.PI / 2
  ground.receiveShadow = true
  // 标记地面为不可变换的对象
  ground.userData.isTransformable = false
  scene.add(ground)

  // 添加网格辅助线
  const gridHelper = new THREE.GridHelper(20, 20, 0x888888, 0xdddddd)
  // 标记网格辅助线为不可变换的对象
  gridHelper.userData.isTransformable = false
  scene.add(gridHelper)

  // 添加环境光
  const ambientLight = new THREE.AmbientLight(0xffffff, 0.6)
  scene.add(ambientLight)

  // 添加方向光
  const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8)
  directionalLight.position.set(10, 10, 5)
  directionalLight.castShadow = true
  directionalLight.shadow.camera.near = 0.1
  directionalLight.shadow.camera.far = 50
  directionalLight.shadow.camera.left = -10
  directionalLight.shadow.camera.right = 10
  directionalLight.shadow.camera.top = 10
  directionalLight.shadow.camera.bottom = -10
  scene.add(directionalLight)

  // 创建射线投射器和鼠标向量
  raycaster = new THREE.Raycaster()
  mouse = new THREE.Vector2()

  // 添加事件监听器
  window.addEventListener('resize', onWindowResize)
  renderer.domElement.addEventListener('click', onMouseClick)
  renderer.domElement.addEventListener('contextmenu', onRightClick)
  document.addEventListener('click', onDocumentClick)
}

// 窗口大小改变时更新渲染
const onWindowResize = () => {
  camera.aspect = window.innerWidth / window.innerHeight
  camera.updateProjectionMatrix()
  renderer.setSize(window.innerWidth, window.innerHeight)
}

// 鼠标点击事件
const onMouseClick = (event: MouseEvent) => {
  if (event.button === 0) { // 左键点击
    mouse.x = (event.clientX / window.innerWidth) * 2 - 1
    mouse.y = -(event.clientY / window.innerHeight) * 2 + 1

    raycaster.setFromCamera(mouse, camera)
    const intersects = raycaster.intersectObjects(scene.children, true)

    if (intersects.length > 0) {
      // 找到第一个可变换的相交对象
      let targetObject: THREE.Object3D | null = null
      for (const intersect of intersects) {
        let obj = intersect.object
        while (obj.parent && obj.parent.type !== 'Scene') {
          obj = obj.parent
        }
        
        // 检查对象是否可变换
        if (obj.userData.isTransformable === true) {
          targetObject = obj
          break
        }
      }

      if (targetObject) {
        selectObject(targetObject)
      } else {
        // 点击了不可变换的对象（如地面），确保取消选择并恢复视角控制
        deselectObject()
        // 确保OrbitControls可用
        controls.enabled = true
        // 强制更新OrbitControls
        controls.update()
      }
    } else {
      // 点击了空白区域，确保取消选择并恢复视角控制
      deselectObject()
      // 确保OrbitControls可用
      controls.enabled = true
      // 强制更新OrbitControls
      controls.update()
    }
  }
}

// 右键点击事件
const onRightClick = (event: MouseEvent) => {
  event.preventDefault()
  
  mouse.x = (event.clientX / window.innerWidth) * 2 - 1
  mouse.y = -(event.clientY / window.innerHeight) * 2 + 1

  raycaster.setFromCamera(mouse, camera)
  const intersects = raycaster.intersectObjects(scene.children, true)

  if (intersects.length > 0) {
    // 找到第一个可变换的相交对象
    let targetObject: THREE.Object3D | null = null
    for (const intersect of intersects) {
      let obj = intersect.object
      while (obj.parent && obj.parent.type !== 'Scene') {
        obj = obj.parent
      }
      
      // 检查对象是否可变换
      if (obj.userData.isTransformable === true) {
        targetObject = obj
        break
      }
    }

    if (targetObject) {
      // 如果当前有对象处于编辑模式，不执行选择操作，直接显示右键菜单
      if (transformControls.object) {
        // 如果右键点击的是当前正在编辑的对象，确保selectedObject正确设置并显示右键菜单
        if (transformControls.object === targetObject) {
          selectedObject.value = targetObject
          showContextMenu(event.clientX, event.clientY)
        }
        // 如果右键点击的是其他对象，选择新对象并显示右键菜单
        else {
          selectedObject.value = targetObject
          showContextMenu(event.clientX, event.clientY)
        }
        return
      }
      
      // 确保selectedObject被正确设置
      selectedObject.value = targetObject
      showContextMenu(event.clientX, event.clientY)
    } else {
      // 点击了不可变换的对象（如地面），确保取消选择并恢复视角控制
      deselectObject()
      // 确保OrbitControls可用
      controls.enabled = true
      // 强制更新OrbitControls
      controls.update()
    }
  } else {
    // 点击了空白区域，确保取消选择并恢复视角控制
    deselectObject()
    // 确保OrbitControls可用
    controls.enabled = true
    // 强制更新OrbitControls
    controls.update()
  }
}

// 文档点击事件（用于隐藏右键菜单）
const onDocumentClick = (event: Event) => {
  // 检查点击是否在右键菜单内部
  const target = event.target as HTMLElement
  if (target.closest('.context-menu')) {
    return // 如果点击的是右键菜单内部，不隐藏菜单
  }
  
  contextMenuVisible.value = false
}

// 选择对象
const selectObject = (object: THREE.Object3D) => {
  // 只选择可变换的对象
  if (object.userData.isTransformable !== true) {
    deselectObject()
    return
  }
  
  // 如果当前有对象处于编辑模式，不允许选择其他对象
  if (transformControls.object && transformControls.object !== object) {
    return
  }
  
  selectedObject.value = object
  
  // 不自动附加变换控制器，只选择对象但不进入编辑模式
  // 确保轨道控制器保持可用
  controls.enabled = true
}

// 取消选择对象
const deselectObject = () => {
  // 如果当前有对象处于编辑模式，不执行取消选择操作
  if (transformControls.object) {
    return
  }
  
  selectedObject.value = null
  // 确保OrbitControls可用
  controls.enabled = true
  // 强制更新OrbitControls
  controls.update()
  // 隐藏右键菜单
  contextMenuVisible.value = false
}

// 显示右键菜单
const showContextMenu = (x: number, y: number) => {
  contextMenuPosition.value = { x, y }
  contextMenuVisible.value = true
}

// 添加基础形状
const addBasicShape = (shapeType: string) => {
  let geometry: THREE.BufferGeometry
  let material = new THREE.MeshStandardMaterial({ 
    color: Math.random() * 0xffffff,
    roughness: 0.5,
    metalness: 0.3
  })
  let mesh: THREE.Mesh

  switch (shapeType) {
    case 'cube':
      geometry = new THREE.BoxGeometry(1, 1, 1)
      mesh = new THREE.Mesh(geometry, material)
      mesh.name = '方块'
      break
    case 'sphere':
      geometry = new THREE.SphereGeometry(0.5, 32, 16)
      mesh = new THREE.Mesh(geometry, material)
      mesh.name = '球体'
      break
    case 'cylinder':
      geometry = new THREE.CylinderGeometry(0.5, 0.5, 1, 32)
      mesh = new THREE.Mesh(geometry, material)
      mesh.name = '圆柱'
      break
    case 'torus':
      geometry = new THREE.TorusGeometry(0.5, 0.2, 16, 100)
      mesh = new THREE.Mesh(geometry, material)
      mesh.name = '环'
      break
    default:
      return
  }

  mesh.position.y = 0.5
  mesh.castShadow = true
  mesh.receiveShadow = true
  // 标记为可变换的对象
  mesh.userData.isTransformable = true
  scene.add(mesh)
}

// 触发文件输入
const triggerFileInput = () => {
  fileInput.value?.click()
}

// 处理文件上传
const handleFileUpload = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (!target.files || target.files.length === 0) return

  const file = target.files[0]
  const loader = new GLTFLoader()
  
  // 初始化DRACOLoader
  const dracoLoader = new DRACOLoader()
  // 设置DRACOLoader的解码器路径
  dracoLoader.setDecoderPath('https://www.gstatic.com/draco/versioned/decoders/1.5.6/')
  // 将DRACOLoader与GLTFLoader关联
  loader.setDRACOLoader(dracoLoader)
  
  loader.load(
    URL.createObjectURL(file),
    (gltf) => {
      const model = gltf.scene
      model.position.y = 0.5
      model.castShadow = true
      model.receiveShadow = true
      // 标记为可变换的对象
      model.userData.isTransformable = true
      model.traverse((child) => {
        if (child instanceof THREE.Mesh) {
          child.castShadow = true
          child.receiveShadow = true
          // 标记子网格为可变换的对象
          child.userData.isTransformable = true
        }
      })
      scene.add(model)
    },
    undefined,
    (error) => {
      console.error('加载GLB模型时出错:', error)
    }
  )
}

// 导出场景
const exportScene = () => {
  const sceneData = {
    objects: []
  }

  scene.traverse((child) => {
    if (child instanceof THREE.Mesh && child.userData.isTransformable !== false) {
      const objectData = {
        name: child.name,
        type: child.geometry.type,
        position: child.position.toArray(),
        rotation: child.rotation.toArray(),
        scale: child.scale.toArray(),
        color: (child.material as THREE.MeshStandardMaterial).color.getHex()
      }
      sceneData.objects.push(objectData)
    }
  })

  const dataStr = JSON.stringify(sceneData, null, 2)
  const dataUri = 'data:application/json;charset=utf-8,'+ encodeURIComponent(dataStr)
  
  const exportFileDefaultName = 'scene.json'

  const linkElement = document.createElement('a')
  linkElement.setAttribute('href', dataUri)
  linkElement.setAttribute('download', exportFileDefaultName)
  linkElement.click()
}

// 触发导入输入
const triggerImportInput = () => {
  importInput.value?.click()
}

// 处理场景导入
const handleImportScene = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (!target.files || target.files.length === 0) return

  const file = target.files[0]
  const reader = new FileReader()
  
  reader.onload = (e) => {
    try {
      const sceneData = JSON.parse(e.target?.result as string)
      
      // 清除当前场景中的所有对象（除了地面和网格）
      const objectsToRemove: THREE.Object3D[] = []
      scene.traverse((child) => {
        if (child instanceof THREE.Mesh && child !== ground) {
          objectsToRemove.push(child)
        }
      })
      
      objectsToRemove.forEach(obj => {
        scene.remove(obj)
      })

      // 重新创建对象
      sceneData.objects.forEach((objData: any) => {
        let geometry: THREE.BufferGeometry
        const material = new THREE.MeshStandardMaterial({ 
          color: objData.color,
          roughness: 0.5,
          metalness: 0.3
        })
        let mesh: THREE.Mesh

        switch (objData.type) {
          case 'BoxGeometry':
            geometry = new THREE.BoxGeometry(1, 1, 1)
            break
          case 'SphereGeometry':
            geometry = new THREE.SphereGeometry(0.5, 32, 16)
            break
          case 'CylinderGeometry':
            geometry = new THREE.CylinderGeometry(0.5, 0.5, 1, 32)
            break
          case 'TorusGeometry':
            geometry = new THREE.TorusGeometry(0.5, 0.2, 16, 100)
            break
          default:
            return
        }

        mesh = new THREE.Mesh(geometry, material)
        mesh.name = objData.name
        mesh.position.fromArray(objData.position)
        mesh.rotation.fromArray(objData.rotation)
        mesh.scale.fromArray(objData.scale)
        mesh.castShadow = true
        mesh.receiveShadow = true
        // 标记为可变换的对象
        mesh.userData.isTransformable = true
        scene.add(mesh)
      })
    } catch (error) {
      console.error('导入场景时出错:', error)
    }
  }
  
  reader.readAsText(file)
}

// 处理删除对象
const handleDeleteObject = () => {
  deleteSelectedObject()
  contextMenuVisible.value = false
}

// 处理进入编辑模式
const handleEnterEditMode = () => {
  // 确保selectedObject存在
  if (!selectedObject.value) {
    // 如果没有选中的对象，尝试从右键点击的位置获取
    return
  }
  
  toggleTransformControls()
  contextMenuVisible.value = false
}

// 处理退出编辑模式
const handleExitEditMode = () => {
  exitEditMode()
  contextMenuVisible.value = false
}

// 删除选中的对象
const deleteSelectedObject = () => {
  if (selectedObject.value && selectedObject.value.userData.isTransformable !== false) {
    scene.remove(selectedObject.value)
    deselectObject()
  }
}

// 退出编辑模式
const exitEditMode = () => {
  if (transformControls.object) {
    transformControls.detach()
    // 确保轨道控制器可用
    controls.enabled = true
  }
}

// 切换变换控制器
const toggleTransformControls = () => {
  if (!selectedObject.value || selectedObject.value.userData.isTransformable !== true) {
    return
  }
  
  // 进入编辑模式
  transformControls.attach(selectedObject.value)
  transformControls.setMode('translate')
  // 禁用轨道控制器，避免冲突
  controls.enabled = false
}

// 格式化向量显示
const formatVector = (vector: THREE.Vector3 | THREE.Euler) => {
  return `(${vector.x.toFixed(2)}, ${vector.y.toFixed(2)}, ${vector.z.toFixed(2)})`
}

// 动画循环
const animate = () => {
  animationId = requestAnimationFrame(animate)
  controls.update()
  renderer.render(scene, camera)
}

// 组件挂载时初始化
onMounted(() => {
  nextTick(() => {
    initThreeJS()
    animate()
  })
})

// 组件卸载前清理
onBeforeUnmount(() => {
  if (animationId) {
    cancelAnimationFrame(animationId)
  }
  
  window.removeEventListener('resize', onWindowResize)
  renderer.domElement.removeEventListener('click', onMouseClick)
  renderer.domElement.removeEventListener('contextmenu', onRightClick)
  document.removeEventListener('click', onDocumentClick)
  
  renderer.dispose()
})
</script>

<style>
body {
  margin: 0;
  padding: 0;
  overflow: hidden;
  font-family: Arial, sans-serif;
}

.canvas-container {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 1;
}

.toolbar {
  position: fixed;
  top: 10px;
  left: 10px;
  background-color: rgba(255, 255, 255, 0.8);
  padding: 10px;
  border-radius: 5px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
  z-index: 10;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.toolbar h3 {
  margin: 0 0 5px 0;
  font-size: 16px;
}

.toolbar button {
  padding: 8px 12px;
  background-color: #4CAF50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.toolbar button:hover {
  background-color: #45a049;
}

.toolbar input[type="file"] {
  display: none;
}

.properties-panel {
  position: fixed;
  top: 10px;
  right: 10px;
  background-color: rgba(255, 255, 255, 0.8);
  padding: 15px;
  border-radius: 5px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
  z-index: 10;
  min-width: 200px;
}

.properties-panel h3 {
  margin: 0 0 10px 0;
  font-size: 16px;
}

.property-item {
  margin-bottom: 8px;
  display: flex;
  justify-content: space-between;
}

.property-label {
  font-weight: bold;
  margin-right: 10px;
}

.property-value {
  color: #333;
}

.context-menu {
  position: fixed;
  background-color: white;
  border: 1px solid #ccc;
  border-radius: 4px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
  z-index: 20;
  padding: 5px 0;
}

.context-menu-item {
  padding: 8px 15px;
  cursor: pointer;
}

.context-menu-item:hover {
  background-color: #f0f0f0;
}
</style>