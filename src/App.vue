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
    <button @click="loadGLBFromPublicList">从public文件夹加载GLB</button>
    <select ref="publicGLBSelect" @change="handlePublicGLBSelection" style="display: none;">
      <option value="">选择GLB模型</option>
    </select>
    <button @click="exportScene">导出场景</button>
    <button @click="triggerImportInput">导入场景</button>
    <input type="file" ref="importInput" @change="handleImportScene" accept=".json" />

    <!-- 编辑模式工具栏 -->
    <div v-if="transformControlsRef?.object" class="edit-toolbar">
      <h4>编辑模式</h4>
      <div class="transform-buttons">
        <button @click="setTransformMode('translate')" :class="{ active: transformMode === 'translate' }">移动</button>
        <button @click="setTransformMode('rotate')" :class="{ active: transformMode === 'rotate' }">旋转</button>
        <button @click="setTransformMode('scale')" :class="{ active: transformMode === 'scale' }">缩放</button>
      </div>
    </div>
  </div>

  <div v-if="selectedObject" class="properties-panel">
    <h3>物体属性</h3>
    <div class="property-item">
      <span class="property-label">名称:</span>
      <span class="property-value">{{ selectedObject.name || '未命名' }}</span>
    </div>

    <!-- 位置属性 -->
    <div class="property-group">
      <h4>位置</h4>
      <div class="property-row">
        <span class="axis-label">X:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="0.1"
          v-model.number="position.x" @input="updateObjectPosition" class="property-input" />
        <span v-else class="property-value">{{ selectedObject.position.x.toFixed(2) }}</span>
      </div>
      <div class="property-row">
        <span class="axis-label">Y:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="0.1"
          v-model.number="position.y" @input="updateObjectPosition" class="property-input" />
        <span v-else class="property-value">{{ selectedObject.position.y.toFixed(2) }}</span>
      </div>
      <div class="property-row">
        <span class="axis-label">Z:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="0.1"
          v-model.number="position.z" @input="updateObjectPosition" class="property-input" />
        <span v-else class="property-value">{{ selectedObject.position.z.toFixed(2) }}</span>
      </div>
    </div>

    <!-- 旋转属性 -->
    <div class="property-group">
      <h4>旋转 (度)</h4>
      <div class="property-row">
        <span class="axis-label">X:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="1" v-model.number="rotation.x"
          @input="updateObjectRotation" class="property-input" />
        <span v-else class="property-value">{{ (selectedObject.rotation.x * 180 / Math.PI).toFixed(2) }}</span>
      </div>
      <div class="property-row">
        <span class="axis-label">Y:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="1" v-model.number="rotation.y"
          @input="updateObjectRotation" class="property-input" />
        <span v-else class="property-value">{{ (selectedObject.rotation.y * 180 / Math.PI).toFixed(2) }}</span>
      </div>
      <div class="property-row">
        <span class="axis-label">Z:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="1" v-model.number="rotation.z"
          @input="updateObjectRotation" class="property-input" />
        <span v-else class="property-value">{{ (selectedObject.rotation.z * 180 / Math.PI).toFixed(2) }}</span>
      </div>
    </div>

    <!-- 缩放属性 -->
    <div class="property-group">
      <h4>缩放</h4>
      <div class="property-row">
        <span class="axis-label">X:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="0.1" v-model.number="scale.x"
          @input="updateObjectScale" class="property-input" />
        <span v-else class="property-value">{{ selectedObject.scale.x.toFixed(2) }}</span>
      </div>
      <div class="property-row">
        <span class="axis-label">Y:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="0.1" v-model.number="scale.y"
          @input="updateObjectScale" class="property-input" />
        <span v-else class="property-value">{{ selectedObject.scale.y.toFixed(2) }}</span>
      </div>
      <div class="property-row">
        <span class="axis-label">Z:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="0.1" v-model.number="scale.z"
          @input="updateObjectScale" class="property-input" />
        <span v-else class="property-value">{{ selectedObject.scale.z.toFixed(2) }}</span>
      </div>
    </div>
  </div>

  <div v-if="contextMenuVisible" class="context-menu"
    :style="{ left: contextMenuPosition.x + 'px', top: contextMenuPosition.y + 'px' }">
    <div class="context-menu-item" @click="handleDeleteObject">删除</div>
    <div v-if="selectedObject && selectedObject.userData.needsGLBLoad" class="context-menu-item" @click="handleLoadGLB">
      加载GLB文件</div>
    <div v-if="transformControls.object === selectedObject" class="context-menu-item" @click="handleExitEditMode">取消编辑模式
    </div>
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
const publicGLBSelect = ref<HTMLSelectElement | null>(null)
const selectedObject = ref<THREE.Object3D | null>(null)
const objects = ref<THREE.Object3D[]>([])
const contextMenuVisible = ref(false)
const contextMenuPosition = ref({ x: 0, y: 0 })
const transformMode = ref('translate') // 变换模式：translate, rotate, scale

// 对象属性的响应式变量
const position = ref({ x: 0, y: 0, z: 0 })
const rotation = ref({ x: 0, y: 0, z: 0 })
const scale = ref({ x: 1, y: 1, z: 1 })

// Three.js 变量
let scene: THREE.Scene
let camera: THREE.PerspectiveCamera
let renderer: THREE.WebGLRenderer
let controls: OrbitControls
let transformControls: TransformControls
const transformControlsRef = ref<TransformControls | null>(null)
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
  transformControlsRef.value = transformControls
  transformControls.addEventListener('dragging-changed', (event) => {
    // 当开始拖拽时禁用轨道控制器，结束时启用
    controls.enabled = !event.value
  })
  transformControls.addEventListener('objectChange', () => {
    // 当对象发生变化时，更新属性面板
    if (selectedObject.value) {
      // 更新响应式变量
      position.value = {
        x: selectedObject.value.position.x,
        y: selectedObject.value.position.y,
        z: selectedObject.value.position.z
      }
      rotation.value = {
        x: selectedObject.value.rotation.x,
        y: selectedObject.value.rotation.y,
        z: selectedObject.value.rotation.z
      }
      scale.value = {
        x: selectedObject.value.scale.x,
        y: selectedObject.value.scale.y,
        z: selectedObject.value.scale.z
      }
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
  console.log(0);

  if (event.button === 0) { // 左键点击
    mouse.x = (event.clientX / window.innerWidth) * 2 - 1
    mouse.y = -(event.clientY / window.innerHeight) * 2 + 1

    raycaster.setFromCamera(mouse, camera)
    // 使用objects数组进行射线检测
    const intersects = raycaster.intersectObjects(objects.value)
    console.log(1);
    if (intersects.length > 0) {
      // 找到第一个可变换的相交对象
      let targetObject: THREE.Object3D | null = null
      targetObject = intersects[0].object
      console.log(2);

      if (targetObject) {
        selectObject(targetObject)
        console.log(3);
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

  // 计算鼠标位置
  const mouse = new THREE.Vector2()
  mouse.x = (event.clientX / window.innerWidth) * 2 - 1
  mouse.y = -(event.clientY / window.innerHeight) * 2 + 1

  // 创建射线
  raycaster.setFromCamera(mouse, camera)
  console.log(2);
  // 使用objects数组进行射线检测
  const intersects = raycaster.intersectObjects(objects.value)
  console.log(3);

  if (intersects.length > 0) {
    // 获取第一个相交的对象
    const intersectedObject = intersects[0].object

    // 找到最顶层的可变换对象
    let targetObject: THREE.Object3D | null = intersectedObject

    if (targetObject) {
      // 选中对象
      selectedObject.value = targetObject

      // 更新响应式变量
      position.value = {
        x: targetObject.position.x,
        y: targetObject.position.y,
        z: targetObject.position.z
      }
      // 将弧度转换为度数
      rotation.value = {
        x: targetObject.rotation.x * 180 / Math.PI,
        y: targetObject.rotation.y * 180 / Math.PI,
        z: targetObject.rotation.z * 180 / Math.PI
      }
      scale.value = {
        x: targetObject.scale.x,
        y: targetObject.scale.y,
        z: targetObject.scale.z
      }

      // 显示右键菜单
      showContextMenu(event.clientX, event.clientY)

    }
  } else {
    // 点击了空白区域，取消选择
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

  // 更新响应式变量
  position.value = {
    x: object.position.x,
    y: object.position.y,
    z: object.position.z
  }
  // 将弧度转换为度数
  rotation.value = {
    x: object.rotation.x * 180 / Math.PI,
    y: object.rotation.y * 180 / Math.PI,
    z: object.rotation.z * 180 / Math.PI
  }
  scale.value = {
    x: object.scale.x,
    y: object.scale.y,
    z: object.scale.z
  }

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

  // 将mesh对象添加到objects数组中
  objects.value.push(mesh)
}

// 触发文件输入
const triggerFileInput = () => {
  fileInput.value?.click()
}

// 处理文件上传
const handleFileUpload = async (event: Event) => {
  const target = event.target as HTMLInputElement
  if (!target.files || target.files.length === 0) return

  const file = target.files[0]

  try {
    // 1. 先将文件保存到public文件夹（会覆盖同名文件）
    const savedFileName = await saveGLBToPublic(file)

    // 2. 检查是否是客户端处理的情况
    if (savedFileName === 'CLIENT_HANDLED') {
      // 客户端已经处理了模型加载，不需要再执行后续步骤
      console.log('GLB模型已通过客户端方式加载')
      return
    }

    // 3. 从public文件夹加载GLB模型
    await loadGLBFromPublic(`/${savedFileName}`, savedFileName)

  } catch (error) {
    console.error('处理GLB文件时出错:', error)
    alert('加载GLB文件失败，请检查文件是否有效。')
  }

  // 重置文件输入
  target.value = ''
}

// 保存GLB文件到public文件夹
const saveGLBToPublic = (file: File): Promise<string> => {
  return new Promise((resolve, reject) => {
    // 创建FormData对象
    const formData = new FormData()
    formData.append('glbFile', file)

    // 使用fetch发送文件到服务器
    fetch('/api/save-glb', {
      method: 'POST',
      body: formData
    })
      .then(response => {
        if (!response.ok) {
          throw new Error('服务器响应错误')
        }
        return response.json()
      })
      .then(data => {
        if (data.success) {
          resolve(data.fileName)
        } else {
          throw new Error(data.error || '保存文件失败')
        }
      })
      .catch(error => {
        console.error('保存GLB文件到public文件夹时出错:', error)

        // 客户端处理模型加载
        console.log('使用客户端方式加载GLB模型')

        // 直接在客户端加载GLB文件
        const loader = new GLTFLoader()

        // 初始化DRACOLoader
        const dracoLoader = new DRACOLoader()
        // 设置DRACOLoader的解码器路径为本地路径
        dracoLoader.setDecoderPath('./draco/')
        // 将DRACOLoader与GLTFLoader关联
        loader.setDRACOLoader(dracoLoader)

        loader.load(
          URL.createObjectURL(file),
          (gltf) => {
            const model = gltf.scene
            const meshObjects: THREE.Object3D[] = []

            // 遍历模型中的所有对象，找到所有的Mesh对象
            model.traverse((child) => {
              if (child instanceof THREE.Mesh) {
                // 克隆mesh对象，以便可以独立添加到场景中
                const mesh = child.clone()
                mesh.name = `${file.name.replace(/\.[^/.]+$/, "")}_${mesh.name || 'mesh'}`
                mesh.castShadow = true
                mesh.receiveShadow = true

                // 标记为可变换的对象
                mesh.userData.isTransformable = true
                // 标记为GLB模型的一部分
                mesh.userData.isGLB = true
                // 保存原始文件名
                mesh.userData.originalFileName = file.name

                // 直接将mesh对象添加到场景中
                scene.add(mesh)
                meshObjects.push(mesh)

                // 将mesh对象添加到objects数组中
                objects.value.push(mesh)
              }
            })
            console.log(`客户端成功加载GLB模型: ${file.name}，包含 ${meshObjects.length} 个网格对象`)
          },
          undefined,
          (error) => {
            console.error('客户端加载GLB模型时出错:', error)
            alert('加载GLB文件失败，请检查文件是否有效。')
          }
        )

        reject('CLIENT_HANDLED')
      })
  })
}

// 导出场景
const exportScene = () => {
  const sceneData = {
    objects: []
  }

  // 从objects数组中导出对象，而不是从scene中遍历
  objects.value.forEach((child) => {
    // 只导出可变换的对象（不包括地面和网格辅助线）
    if (child.userData.isTransformable === true) {
      let objectData: any = {
        name: child.name || '未命名对象',
        position: child.position.toArray(),
        rotation: child.rotation.toArray(),
        scale: child.scale.toArray(),
        visible: child.visible,
        castShadow: child.castShadow,
        receiveShadow: child.receiveShadow,
        userData: { ...child.userData }
      }

      // 判断对象类型
      if (child instanceof THREE.Mesh) {
        // 基础几何体
        objectData.type = 'basic'
        objectData.geometryType = child.geometry.type

        // 保存材质信息
        if (child.material instanceof THREE.MeshStandardMaterial) {
          objectData.material = {
            type: 'MeshStandardMaterial',
            color: child.material.color.getHex(),
            roughness: child.material.roughness,
            metalness: child.material.metalness,
            transparent: child.material.transparent,
            opacity: child.material.opacity
          }
        }
      } else if (child.userData.isGLB === true) {
        // 对于GLB模型的mesh对象
        objectData.type = 'gltf'

        // 保存GLB模型的原始文件名（如果有）
        if (child.userData.originalFileName) {
          objectData.originalFileName = child.userData.originalFileName
          // 保存模型路径，假设模型在public文件夹中
          objectData.modelPath = `/${child.userData.originalFileName}`
        }
      }

      sceneData.objects.push(objectData)
    }
  })

  const dataStr = JSON.stringify(sceneData, null, 2)
  const dataUri = 'data:application/json;charset=utf-8,' + encodeURIComponent(dataStr)

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

      // 清除当前场景中的所有可变换对象（除了地面和网格辅助线）
      const objectsToRemove: THREE.Object3D[] = []
      scene.traverse((child) => {
        if (child.userData.isTransformable === true) {
          objectsToRemove.push(child)
        }
      })

      objectsToRemove.forEach(obj => {
        scene.remove(obj)
        // 从objects数组中移除对象
        const index = objects.value.indexOf(obj)
        if (index !== -1) {
          objects.value.splice(index, 1)
        }
      })

      // 清空objects数组
      objects.value = []

      // 重新创建对象
      sceneData.objects.forEach((objData: any) => {
        if (objData.type === 'basic') {
          // 创建基础几何体
          let geometry: THREE.BufferGeometry
          let material: THREE.MeshStandardMaterial

          // 创建材质
          if (objData.material && objData.material.type === 'MeshStandardMaterial') {
            material = new THREE.MeshStandardMaterial({
              color: objData.material.color,
              roughness: objData.material.roughness,
              metalness: objData.material.metalness,
              transparent: objData.material.transparent,
              opacity: objData.material.opacity
            })
          } else {
            // 默认材质
            material = new THREE.MeshStandardMaterial({
              roughness: 0.5,
              metalness: 0.3
            })
          }

          // 根据几何体类型创建几何体
          switch (objData.geometryType) {
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
              console.warn(`未知的几何体类型: ${objData.geometryType}`)
              return
          }

          const mesh = new THREE.Mesh(geometry, material)
          mesh.name = objData.name
          mesh.position.fromArray(objData.position)
          mesh.rotation.fromArray(objData.rotation)
          mesh.scale.fromArray(objData.scale)
          mesh.visible = objData.visible !== undefined ? objData.visible : true
          mesh.castShadow = objData.castShadow !== undefined ? objData.castShadow : true
          mesh.receiveShadow = objData.receiveShadow !== undefined ? objData.receiveShadow : true

          // 恢复userData
          mesh.userData = { ...objData.userData }
          mesh.userData.isTransformable = true

          scene.add(mesh)
          // 将mesh对象添加到objects数组中
          objects.value.push(mesh)
        } else if (objData.type === 'gltf') {
          // 对于GLB模型，首先尝试从public文件夹自动加载
          if (objData.modelPath && objData.originalFileName) {
            // 尝试从public文件夹加载GLB模型
            loadGLBFromPublic(objData.modelPath, objData.originalFileName)
              .then(meshes => {
                // 设置每个mesh对象的位置、旋转和缩放
                meshes.forEach(mesh => {
                  mesh.position.fromArray(objData.position)
                  mesh.rotation.fromArray(objData.rotation)
                  mesh.scale.fromArray(objData.scale)
                  mesh.visible = objData.visible !== undefined ? objData.visible : true
                  mesh.castShadow = objData.castShadow !== undefined ? objData.castShadow : true
                  mesh.receiveShadow = objData.receiveShadow !== undefined ? objData.receiveShadow : true

                  // 恢复userData
                  mesh.userData = { ...mesh.userData, ...objData.userData }
                  mesh.userData.isTransformable = true
                  mesh.userData.isGLB = true
                })

                console.log(`成功从public文件夹加载GLB模型: ${objData.originalFileName}，包含 ${meshes.length} 个网格对象`)
              })
              .catch(error => {
                console.warn(`无法从public文件夹加载GLB模型: ${objData.originalFileName}`, error)

                // 如果自动加载失败，创建一个占位符
                const placeholderGeometry = new THREE.BoxGeometry(1, 1, 1)
                const placeholderMaterial = new THREE.MeshStandardMaterial({
                  color: 0x00ff00,
                  wireframe: true
                })
                const placeholderMesh = new THREE.Mesh(placeholderGeometry, placeholderMaterial)
                placeholderMesh.name = objData.name || 'GLB占位符'
                placeholderMesh.position.fromArray(objData.position)
                placeholderMesh.rotation.fromArray(objData.rotation)
                placeholderMesh.scale.fromArray(objData.scale)
                placeholderMesh.visible = objData.visible !== undefined ? objData.visible : true
                placeholderMesh.castShadow = objData.castShadow !== undefined ? objData.castShadow : true
                placeholderMesh.receiveShadow = objData.receiveShadow !== undefined ? objData.receiveShadow : true

                // 恢复userData
                placeholderMesh.userData = { ...objData.userData }
                placeholderMesh.userData.isTransformable = true
                placeholderMesh.userData.isGLB = true
                placeholderMesh.userData.originalFileName = objData.originalFileName
                placeholderMesh.userData.needsGLBLoad = true

                scene.add(placeholderMesh)
                // 将占位符mesh对象添加到objects数组中
                objects.value.push(placeholderMesh)

                console.log(`已为GLB模型创建占位符: ${objData.originalFileName}`)
              })
          } else {
            // 如果没有模型路径信息，创建一个占位符
            const placeholderGeometry = new THREE.BoxGeometry(1, 1, 1)
            const placeholderMaterial = new THREE.MeshStandardMaterial({
              color: 0x00ff00,
              wireframe: true
            })
            const placeholderMesh = new THREE.Mesh(placeholderGeometry, placeholderMaterial)
            placeholderMesh.name = objData.name || 'GLB占位符'
            placeholderMesh.position.fromArray(objData.position)
            placeholderMesh.rotation.fromArray(objData.rotation)
            placeholderMesh.scale.fromArray(objData.scale)
            placeholderMesh.visible = objData.visible !== undefined ? objData.visible : true
            placeholderMesh.castShadow = objData.castShadow !== undefined ? objData.castShadow : true
            placeholderMesh.receiveShadow = objData.receiveShadow !== undefined ? objData.receiveShadow : true

            // 恢复userData
            placeholderMesh.userData = { ...objData.userData }
            placeholderMesh.userData.isTransformable = true
            placeholderMesh.userData.isGLB = true

            // 保存原始文件名
            if (objData.originalFileName) {
              placeholderMesh.userData.originalFileName = objData.originalFileName
              placeholderMesh.userData.needsGLBLoad = true
            }

            scene.add(placeholderMesh)
          }
        }
      })

      // 检查是否有需要加载GLB文件的对象
      const needsGLBLoad = sceneData.objects.some((obj: any) =>
        obj.type === 'gltf' && obj.originalFileName
      )

      if (needsGLBLoad) {
        // 显示提示，告知用户需要手动加载GLB文件
        alert('场景中包含GLB模型。请右键点击绿色线框占位符，然后选择"加载GLB文件"来恢复原始模型。')
      }
    } catch (error) {
      console.error('导入场景时出错:', error)
    }
  }

  reader.readAsText(file)
}

// 从public文件夹加载GLB模型
const loadGLBFromPublic = (modelPath: string, fileName: string): Promise<THREE.Object3D[]> => {
  return new Promise((resolve, reject) => {
    const loader = new GLTFLoader()

    // 初始化DRACOLoader
    const dracoLoader = new DRACOLoader()
    // 设置DRACOLoader的解码器路径为本地路径
    dracoLoader.setDecoderPath('./draco/')
    // 将DRACOLoader与GLTFLoader关联
    loader.setDRACOLoader(dracoLoader)

    loader.load(
      modelPath,
      (gltf) => {
        const model = gltf.scene
        const meshObjects: THREE.Object3D[] = []

        // 遍历模型中的所有对象，找到所有的Mesh对象
        model.traverse((child) => {
          if (child instanceof THREE.Mesh) {
            // 克隆mesh对象，以便可以独立添加到场景中
            const mesh = child.clone()
            mesh.name = `${fileName.replace(/\.[^/.]+$/, "")}_${mesh.name || 'mesh'}`
            mesh.castShadow = true
            mesh.receiveShadow = true

            // 标记为可变换的对象
            mesh.userData.isTransformable = true
            // 标记为GLB模型的一部分
            mesh.userData.isGLB = true
            // 保存原始文件名
            mesh.userData.originalFileName = fileName

            // 直接将mesh对象添加到场景中
            scene.add(mesh)
            meshObjects.push(mesh)

            // 将mesh对象添加到objects数组中
            objects.value.push(mesh)
          }
        })
        console.log(`成功加载GLB模型: ${fileName}，包含 ${meshObjects.length} 个网格对象`)
        resolve(meshObjects)
      },
      undefined,
      (error) => {
        console.error(`从public文件夹加载GLB模型时出错 (${modelPath}):`, error)
        reject(error)
      }
    )
  })
}

// 加载GLB文件到场景中
const loadGLBToScene = (file: File) => {
  const loader = new GLTFLoader()

  // 初始化DRACOLoader
  const dracoLoader = new DRACOLoader()
  // 设置DRACOLoader的解码器路径为本地路径
  dracoLoader.setDecoderPath('./draco/')
  // 将DRACOLoader与GLTFLoader关联
  loader.setDRACOLoader(dracoLoader)

  loader.load(
    URL.createObjectURL(file),
    (gltf) => {
      const model = gltf.scene

      // 遍历模型中的所有对象，找到所有的Mesh对象
      model.traverse((child) => {
        if (child instanceof THREE.Mesh) {
          // 克隆mesh对象，以便可以独立添加到场景中
          const mesh = child.clone()
          mesh.name = `${file.name.replace(/\.[^/.]+$/, "")}_${mesh.name || 'mesh'}`
          mesh.castShadow = true
          mesh.receiveShadow = true

          // 标记为可变换的对象
          mesh.userData.isTransformable = true
          // 标记为GLB模型的一部分
          mesh.userData.isGLB = true
          // 保存原始文件名
          mesh.userData.originalFileName = file.name

          // 直接将mesh对象添加到场景中
          scene.add(mesh)
          // 将mesh对象添加到objects数组中
          objects.value.push(mesh)
        }
      })

      console.log('GLB文件加载成功:', file.name)
    },
    undefined,
    (error) => {
      console.error('加载GLB模型时出错:', error)
      alert('加载GLB文件失败，请检查文件是否有效。')
    }
  )
}

// 处理加载GLB文件
const handleLoadGLB = () => {
  if (!selectedObject.value || !selectedObject.value.userData.needsGLBLoad) {
    return
  }

  // 创建一个隐藏的文件输入元素
  const fileInput = document.createElement('input')
  fileInput.type = 'file'
  fileInput.accept = '.glb,.gltf'
  fileInput.style.display = 'none'

  // 添加到DOM
  document.body.appendChild(fileInput)

  // 监听文件选择
  fileInput.addEventListener('change', (event) => {
    const target = event.target as HTMLInputElement
    if (!target.files || target.files.length === 0) return

    const file = target.files[0]

    // 直接加载GLB文件到场景中
    loadGLBToScene(file)

    // 删除占位符对象
    if (selectedObject.value) {
      scene.remove(selectedObject.value)
      // 从objects数组中移除占位符对象
      const index = objects.value.indexOf(selectedObject.value)
      if (index !== -1) {
        objects.value.splice(index, 1)
      }
    }

    // 清理
    document.body.removeChild(fileInput)
  })

  // 触发文件选择对话框
  fileInput.click()

  // 隐藏右键菜单
  contextMenuVisible.value = false
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
  if (!selectedObject.value) return

  // 检查对象是否可以被删除
  if (selectedObject.value.userData.isTransformable === false) return

  // 释放对象的资源
  if (selectedObject.value instanceof THREE.Mesh) {
    // 如果是网格对象，释放其资源
    selectedObject.value.geometry.dispose()
    if (selectedObject.value.material instanceof THREE.Material) {
      selectedObject.value.material.dispose()
    } else if (Array.isArray(selectedObject.value.material)) {
      selectedObject.value.material.forEach(material => material.dispose())
    }
  }

  // 从场景中移除对象
  scene.remove(selectedObject.value)

  // 如果对象有父对象，也从父对象中移除
  if (selectedObject.value.parent) {
    selectedObject.value.parent.remove(selectedObject.value)
  }

  // 从objects数组中移除对象
  const index = objects.value.indexOf(selectedObject.value)
  if (index !== -1) {
    objects.value.splice(index, 1)
  }

  // 取消选择
  deselectObject()
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
  transformControls.setMode(transformMode.value as any)
  // 禁用轨道控制器，避免冲突
  controls.enabled = false

  // 更新响应式变量
  position.value = {
    x: selectedObject.value.position.x,
    y: selectedObject.value.position.y,
    z: selectedObject.value.position.z
  }
  // 将弧度转换为度数
  rotation.value = {
    x: selectedObject.value.rotation.x * 180 / Math.PI,
    y: selectedObject.value.rotation.y * 180 / Math.PI,
    z: selectedObject.value.rotation.z * 180 / Math.PI
  }
  scale.value = {
    x: selectedObject.value.scale.x,
    y: selectedObject.value.scale.y,
    z: selectedObject.value.scale.z
  }
}

// 设置变换模式
const setTransformMode = (mode: string) => {
  transformMode.value = mode
  if (transformControlsRef.value?.object) {
    transformControlsRef.value.setMode(mode as any)
  }
}

// 更新对象位置
const updateObjectPosition = () => {
  if (selectedObject.value && transformControlsRef.value?.object === selectedObject.value) {
    selectedObject.value.position.set(position.value.x, position.value.y, position.value.z)
    // 触发变换控制器的更新
    transformControlsRef.value.updateMatrixWorld()
  }
}

// 更新对象旋转
const updateObjectRotation = () => {
  if (selectedObject.value && transformControlsRef.value?.object === selectedObject.value) {
    // 将度数转换为弧度
    selectedObject.value.rotation.set(
      rotation.value.x * Math.PI / 180,
      rotation.value.y * Math.PI / 180,
      rotation.value.z * Math.PI / 180
    )
    // 触发变换控制器的更新
    transformControlsRef.value.updateMatrixWorld()
  }
}

// 更新对象缩放
const updateObjectScale = () => {
  if (selectedObject.value && transformControlsRef.value?.object === selectedObject.value) {
    selectedObject.value.scale.set(scale.value.x, scale.value.y, scale.value.z)
    // 触发变换控制器的更新
    transformControlsRef.value.updateMatrixWorld()
  }
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

// 从public文件夹加载GLB模型列表
const loadGLBFromPublicList = async () => {
  try {
    // 从API获取public文件夹中的GLB文件列表
    const response = await fetch('/api/list-glbs')
    if (!response.ok) {
      throw new Error('获取GLB文件列表失败')
    }

    const data = await response.json()
    const publicGLBFiles = data.files || []

    // 如果没有找到GLB文件，使用默认的cangku.glb
    if (publicGLBFiles.length === 0) {
      publicGLBFiles.push('cangku.glb')
    }

    // 清空选择框
    if (publicGLBSelect.value) {
      publicGLBSelect.value.innerHTML = '<option value="">选择GLB模型</option>'

      // 添加GLB文件选项
      publicGLBFiles.forEach(file => {
        const option = document.createElement('option')
        option.value = file
        option.textContent = file
        publicGLBSelect.value?.appendChild(option)
      })

      // 显示选择框
      publicGLBSelect.value.style.display = 'block'
    }
  } catch (error) {
    console.error('获取GLB文件列表时出错:', error)

    // 如果API调用失败，使用默认的cangku.glb
    const publicGLBFiles = ['cangku.glb']

    // 清空选择框
    if (publicGLBSelect.value) {
      publicGLBSelect.value.innerHTML = '<option value="">选择GLB模型</option>'

      // 添加GLB文件选项
      publicGLBFiles.forEach(file => {
        const option = document.createElement('option')
        option.value = file
        option.textContent = file
        publicGLBSelect.value?.appendChild(option)
      })

      // 显示选择框
      publicGLBSelect.value.style.display = 'block'
    }
  }
}

// 处理从public文件夹选择GLB模型
const handlePublicGLBSelection = () => {
  if (!publicGLBSelect.value) return

  const selectedFile = publicGLBSelect.value.value
  if (!selectedFile) return

  // 从public文件夹加载GLB模型
  loadGLBFromPublic(`/${selectedFile}`, selectedFile)
    .then(meshes => {
      // 设置每个mesh对象的位置
      meshes.forEach(mesh => {
        mesh.position.y = 0.5
      })
      console.log(`成功从public文件夹加载GLB模型: ${selectedFile}，包含 ${meshes.length} 个网格对象`)
    })
    .catch(error => {
      console.error(`无法从public文件夹加载GLB模型: ${selectedFile}`, error)
      alert(`无法加载GLB模型: ${selectedFile}`)
    })

  // 隐藏选择框
  publicGLBSelect.value.style.display = 'none'
}
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

.edit-toolbar {
  margin-top: 15px;
  padding-top: 10px;
  border-top: 1px solid #ddd;
}

.edit-toolbar h4 {
  margin: 0 0 8px 0;
  font-size: 14px;
  color: #333;
}

.transform-buttons {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.transform-buttons button {
  padding: 6px 10px;
  font-size: 13px;
  background-color: #2196F3;
}

.transform-buttons button:hover {
  background-color: #0b7dda;
}

.transform-buttons button.active {
  background-color: #0b7dda;
  box-shadow: 0 0 5px rgba(0, 0, 0, 0.3);
}

.toolbar input[type="file"] {
  display: none;
}

.toolbar select {
  width: 100%;
  padding: 6px 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  background-color: white;
  font-size: 14px;
  margin-top: 5px;
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

.property-group {
  margin-bottom: 15px;
}

.property-group h4 {
  margin: 0 0 8px 0;
  font-size: 14px;
  color: #333;
  border-bottom: 1px solid #eee;
  padding-bottom: 3px;
}

.property-row {
  display: flex;
  align-items: center;
  margin-bottom: 5px;
}

.axis-label {
  width: 20px;
  font-weight: bold;
  margin-right: 10px;
  color: #555;
}

.property-input {
  width: 80px;
  padding: 3px 6px;
  border: 1px solid #ddd;
  border-radius: 3px;
  font-size: 13px;
}

.property-input:focus {
  outline: none;
  border-color: #2196F3;
  box-shadow: 0 0 3px rgba(33, 150, 243, 0.3);
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