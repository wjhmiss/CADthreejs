<template>
  <DxfUpload v-show="showDxfUpload" @goBack="handleDxfDataReturn" />
  <div v-show="!showDxfUpload">
    <div class="canvas-container" ref="canvasContainer"></div>

    <button class="toolbar-toggle-btn" @click="isToolbarCollapsed = !isToolbarCollapsed">
      <span v-if="!isToolbarCollapsed">◀</span>
      <span v-else>▶</span>
    </button>

    <div class="toolbar" v-show="!isToolbarCollapsed">
      <div class="toolbar-header">
        <h3>工具栏</h3>
      </div>
      <div class="toolbar-content">
        <button @click="showDxfUpload = true" class="dxf-upload-btn">上传DXF文件</button>
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
    <button v-if="!isTopViewMode" @click="enterTopViewMode" class="top-view-btn">顶部视角</button>
    <button v-if="isTopViewMode" @click="exitTopViewMode" class="exit-top-view-btn">退出顶部视角</button>
    <button @click="toggleAxisHelper" class="axis-toggle-btn">{{ showAxisHelper ? '隐藏坐标轴' : '显示坐标轴' }}</button>
    <button @click="handleGroundObjects" class="ground-btn">落地</button>
    <button v-if="transformControlsRef?.object" @click="handleExitEditMode" class="exit-edit-btn">退出编辑模式 (ESC)</button>

    <!-- DXF缩放控件 -->
    <div class="dxf-scale-control">
      <span class="scale-label">DXF比例:</span>
      <input type="number" v-model.number="dxfScale" @input="handleDxfScaleChange" step="0.1" min="0.1" class="scale-input" />
    </div>

    <!-- DXF翻转控件 -->
    <div class="dxf-flip-control">
      <span class="flip-label">DXF翻转(度):</span>
      <div class="flip-inputs">
        <div class="flip-input-group">
          <span class="axis-label">X:</span>
          <input type="number" v-model.number="dxfFlipX" @input="handleDxfFlipChange" step="1" class="flip-input" />
        </div>
        <div class="flip-input-group">
          <span class="axis-label">Y:</span>
          <input type="number" v-model.number="dxfFlipY" @input="handleDxfFlipChange" step="1" class="flip-input" />
        </div>
        <div class="flip-input-group">
          <span class="axis-label">Z:</span>
          <input type="number" v-model.number="dxfFlipZ" @input="handleDxfFlipChange" step="1" class="flip-input" />
        </div>
      </div>
    </div>

    <!-- 场景颜色设置 -->
    <div class="scene-color-control">
      <div class="color-control-row">
        <span class="color-label">背景:</span>
        <input type="color" v-model="sceneBackgroundColor" @input="updateSceneBackgroundColor" class="color-input" />
        <span class="color-value">{{ sceneBackgroundColor }}</span>
      </div>
      <div class="color-control-row">
        <span class="color-label">DXF:</span>
        <input type="color" v-model="dxfObjectColor" @input="updateDxfObjectColor" class="color-input" />
        <span class="color-value">{{ dxfObjectColor }}</span>
      </div>
    </div>

    <!-- 编辑模式工具栏 -->
    <div v-if="transformControlsRef?.object" class="edit-toolbar">
      <h4>编辑模式</h4>
      <div class="transform-buttons">
        <button @click.stop="setTransformMode('translate')" :class="{ active: transformMode === 'translate' }">移动</button>
        <button @click.stop="setTransformMode('rotate')" :class="{ active: transformMode === 'rotate' }">旋转</button>
        <button @click.stop="setTransformMode('scale')" :class="{ active: transformMode === 'scale' }">缩放</button>
      </div>
    </div>
      </div>
  </div>

  <div v-if="selectedObject" class="properties-panel">
    <h3>物体属性</h3>
    <div class="property-item">
      <span class="property-label">名称:</span>
      <input v-if="transformControlsRef?.object === selectedObject" type="text" v-model="objectName"
        @input="updateObjectName" @click.stop class="property-input-name" />
      <span v-else class="property-value">{{ selectedObject.name || '未命名' }}</span>
    </div>

    <div class="property-item">
      <span class="property-label">颜色:</span>
      <input v-if="transformControlsRef?.object === selectedObject" type="color" v-model="objectColor"
        @input="updateObjectColor" @click.stop class="property-color-input" />
      <span v-else class="property-value">{{ getObjectColorHex() }}</span>
    </div>

    <!-- 位置属性 -->
    <div class="property-group">
      <h4>位置</h4>
      <div class="property-row">
        <span class="axis-label">X:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="0.1"
          v-model.number="position.x" @input="updateObjectPosition" @click.stop class="property-input" />
        <span v-else class="property-value">{{ selectedObject.position.x.toFixed(2) }}</span>
      </div>
      <div class="property-row">
        <span class="axis-label">Y:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="0.1"
          v-model.number="position.y" @input="updateObjectPosition" @click.stop class="property-input" />
        <span v-else class="property-value">{{ selectedObject.position.y.toFixed(2) }}</span>
      </div>
      <div class="property-row">
        <span class="axis-label">Z:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="0.1"
          v-model.number="position.z" @input="updateObjectPosition" @click.stop class="property-input" />
        <span v-else class="property-value">{{ selectedObject.position.z.toFixed(2) }}</span>
      </div>
    </div>

    <!-- 旋转属性 -->
    <div class="property-group">
      <h4>旋转 (度)</h4>
      <div class="property-row">
        <span class="axis-label">X:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="1" v-model.number="rotation.x"
          @input="updateObjectRotation" @click.stop class="property-input" />
        <span v-else class="property-value">{{ (selectedObject.rotation.x * 180 / Math.PI).toFixed(2) }}</span>
      </div>
      <div class="property-row">
        <span class="axis-label">Y:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="1" v-model.number="rotation.y"
          @input="updateObjectRotation" @click.stop class="property-input" />
        <span v-else class="property-value">{{ (selectedObject.rotation.y * 180 / Math.PI).toFixed(2) }}</span>
      </div>
      <div class="property-row">
        <span class="axis-label">Z:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="1" v-model.number="rotation.z"
          @input="updateObjectRotation" @click.stop class="property-input" />
        <span v-else class="property-value">{{ (selectedObject.rotation.z * 180 / Math.PI).toFixed(2) }}</span>
      </div>
    </div>

    <!-- 缩放属性 -->
    <div class="property-group">
      <h4>缩放</h4>
      <div class="property-row">
        <span class="axis-label">X:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="0.1" v-model.number="scale.x"
          @input="updateObjectScale" @click.stop class="property-input" />
        <span v-else class="property-value">{{ selectedObject.scale.x.toFixed(2) }}</span>
      </div>
      <div class="property-row">
        <span class="axis-label">Y:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="0.1" v-model.number="scale.y"
          @input="updateObjectScale" @click.stop class="property-input" />
        <span v-else class="property-value">{{ selectedObject.scale.y.toFixed(2) }}</span>
      </div>
      <div class="property-row">
        <span class="axis-label">Z:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="0.1" v-model.number="scale.z"
          @input="updateObjectScale" @click.stop class="property-input" />
        <span v-else class="property-value">{{ selectedObject.scale.z.toFixed(2) }}</span>
      </div>
    </div>

    <!-- 标签设置 -->
    <div class="property-group">
      <h4>标签设置</h4>
      <div class="property-row">
        <span class="property-label">大小:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="1" min="8" max="48"
          v-model.number="labelSize" @input="updateLabelSize" @click.stop class="property-input" />
        <span v-else class="property-value">{{ selectedObject.userData.labelSize || 16 }}px</span>
      </div>
      <div class="property-row">
        <span class="property-label">颜色:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="color" v-model="labelColor"
          @input="updateLabelColor" @click.stop class="property-color-input" />
        <span v-else class="property-value">{{ selectedObject.userData.labelColor || '#ffffff' }}</span>
      </div>
    </div>

    <!-- 内发光设置 -->
    <div class="property-group">
      <h4>内发光设置</h4>
      <div class="property-row">
        <span class="property-label">颜色:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="color" v-model="emissiveColor"
          @input="updateEmissiveColor" @click.stop class="property-color-input" />
        <span v-else class="property-value">{{ getEmissiveColorHex() }}</span>
      </div>
      <div class="property-row">
        <span class="property-label">强度:</span>
        <input v-if="transformControlsRef?.object === selectedObject" type="number" step="0.1" min="0" max="2"
          v-model.number="emissiveIntensity" @input="updateEmissiveIntensity" @click.stop class="property-input" />
        <span v-else class="property-value">{{ getEmissiveIntensity() }}</span>
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
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, nextTick } from 'vue'
import * as THREE from 'three'
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls'
import { TransformControls } from 'three/examples/jsm/controls/TransformControls'
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader'
import { DRACOLoader } from 'three/examples/jsm/loaders/DRACOLoader'
import { CSS2DRenderer, CSS2DObject } from 'three/examples/jsm/renderers/CSS2DRenderer'
import { EffectComposer } from 'three/examples/jsm/postprocessing/EffectComposer'
import { RenderPass } from 'three/examples/jsm/postprocessing/RenderPass'
import { UnrealBloomPass } from 'three/examples/jsm/postprocessing/UnrealBloomPass'
import DxfUpload from './DxfUpload.vue'
import { RenderManager } from './Renders/RenderManager'

const showDxfUpload = ref(false)
const isTopViewMode = ref(false)
const showAxisHelper = ref(true)
const isToolbarCollapsed = ref(false)
let renderManager: RenderManager | null = null

// 响应式变量
const canvasContainer = ref<HTMLElement | null>(null)
const fileInput = ref<HTMLInputElement | null>(null)
const importInput = ref<HTMLInputElement | null>(null)
const publicGLBSelect = ref<HTMLSelectElement | null>(null)
const selectedObject = ref<THREE.Object3D | null>(null)
const objects = [] as THREE.Object3D[]
const contextMenuVisible = ref(false)
const contextMenuPosition = ref({ x: 0, y: 0 })
const transformMode = ref('translate')
const dxfScale = ref(1.0)
const dxfFlipX = ref(0)
const dxfFlipY = ref(0)
const dxfFlipZ = ref(0)

const centeringOptions = ref({
  centerX: true,
  centerY: true,
  centerZ: true,
  applyTransform: true
})

// 场景背景颜色和DXF对象颜色
const sceneBackgroundColor = ref('#000000')
const dxfObjectColor = ref('#ffffff')

// GLB模型坐标转换相关变量
let glbBoundingBox: THREE.Box3 = new THREE.Box3()
let glbOriginalCenter: THREE.Vector3 = new THREE.Vector3()
let glbOriginalPositions: Map<THREE.Object3D, THREE.Vector3> = new Map()
let glbIsCentered: boolean = false
let glbRotation: { x: number; y: number; z: number } = { x: 0, y: 0, z: 0 }

// 对象属性的响应式变量
const position = ref({ x: 0, y: 0, z: 0 })
const rotation = ref({ x: 0, y: 0, z: 0 })
const scale = ref({ x: 1, y: 1, z: 1 })
const objectName = ref('')
const objectColor = ref('#ffffff')

// 标签设置的响应式变量
const labelSize = ref(16)
const labelColor = ref('#ffffff')

// 内发光设置的响应式变量
const emissiveColor = ref('#444444')
const emissiveIntensity = ref(1.2)

// Three.js 变量
let scene: THREE.Scene
let camera: THREE.PerspectiveCamera
let renderer: THREE.WebGLRenderer
let labelRenderer: CSS2DRenderer
let composer: EffectComposer
let controls: OrbitControls
let transformControls: TransformControls
const transformControlsRef = ref<TransformControls | null>(null)
let raycaster: THREE.Raycaster
let mouse: THREE.Vector2
let animationId: number

// 顶部视角相关变量
let originalCameraPosition: THREE.Vector3
let originalCameraTarget: THREE.Vector3
let originalCameraZoom: number

// XYZ辅助线变量
let xAxis: THREE.Mesh
let xArrow: THREE.Mesh
let xLabelSprite: THREE.Sprite
let yAxis: THREE.Mesh
let yArrow: THREE.Mesh
let yLabelSprite: THREE.Sprite
let zAxis: THREE.Mesh
let zArrow: THREE.Mesh
let zLabelSprite: THREE.Sprite

// 初始化Three.js场景
const initThreeJS = () => {
  console.log('initThreeJS called, canvasContainer:', canvasContainer.value)
  
  if (!canvasContainer.value) {
    console.error('canvasContainer is null or undefined')
    return
  }
  
  // 创建场景
  scene = new THREE.Scene()
  scene.background = new THREE.Color(sceneBackgroundColor.value)

  // 创建相机
  camera = new THREE.PerspectiveCamera(
    60,
    window.innerWidth / window.innerHeight,
    0.1,
    10000
  )
  camera.position.set(12, 12, 12)

  // 创建渲染器
  renderer = new THREE.WebGLRenderer({ antialias: true })
  renderer.setSize(window.innerWidth, window.innerHeight)
  renderer.shadowMap.enabled = true
  renderer.shadowMap.type = THREE.PCFSoftShadowMap
  canvasContainer.value.appendChild(renderer.domElement)
  console.log('Renderer domElement appended to canvasContainer')

  // 创建后处理效果（Bloom发光效果）
  const renderScene = new RenderPass(scene, camera)

  const bloomPass = new UnrealBloomPass(
    new THREE.Vector2(window.innerWidth, window.innerHeight),
    1.5,
    0.4,
    0.85
  )
  bloomPass.threshold = 1.1
  bloomPass.strength = 1.5
  bloomPass.radius = 0.5

  composer = new EffectComposer(renderer)
  composer.addPass(renderScene)
  composer.addPass(bloomPass)

  // 创建CSS2D渲染器（用于标签）
  labelRenderer = new CSS2DRenderer()
  labelRenderer.setSize(window.innerWidth, window.innerHeight)
  labelRenderer.domElement.style.position = 'absolute'
  labelRenderer.domElement.style.top = '0'
  labelRenderer.domElement.style.pointerEvents = 'none'
  canvasContainer.value.appendChild(labelRenderer.domElement)
  console.log('LabelRenderer domElement appended to canvasContainer')

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

  // 添加XYZ轴辅助线（带标签和箭头）
  const baseAxisLength = 6
  const axisRadius = 0.01
  const arrowRadius = 0.03
  const arrowLength = 0.8

  // 创建X轴（红色）
  const xAxisGeometry = new THREE.CylinderGeometry(axisRadius, axisRadius, baseAxisLength, 8)
  const xAxisMaterial = new THREE.MeshBasicMaterial({ color: 0xff0000 })
  xAxis = new THREE.Mesh(xAxisGeometry, xAxisMaterial)
  xAxis.rotation.z = -Math.PI / 2
  xAxis.position.set(baseAxisLength / 2, 0, 0)
  xAxis.userData.isTransformable = false
  xAxis.userData.baseLength = baseAxisLength
  xAxis.userData.isAxis = true
  scene.add(xAxis)

  // X轴箭头
  const xArrowGeometry = new THREE.ConeGeometry(arrowRadius, arrowLength, 8)
  const xArrowMaterial = new THREE.MeshBasicMaterial({ color: 0xff0000 })
  xArrow = new THREE.Mesh(xArrowGeometry, xArrowMaterial)
  xArrow.rotation.z = -Math.PI / 2
  xArrow.position.set(baseAxisLength + arrowLength / 2, 0, 0)
  xArrow.userData.isTransformable = false
  xArrow.userData.baseLength = baseAxisLength
  xArrow.userData.isAxis = true
  scene.add(xArrow)

  // 创建Y轴（绿色）
  const yAxisGeometry = new THREE.CylinderGeometry(axisRadius, axisRadius, baseAxisLength, 8)
  const yAxisMaterial = new THREE.MeshBasicMaterial({ color: 0x00ff00 })
  yAxis = new THREE.Mesh(yAxisGeometry, yAxisMaterial)
  yAxis.position.set(0, baseAxisLength / 2, 0)
  yAxis.userData.isTransformable = false
  yAxis.userData.baseLength = baseAxisLength
  yAxis.userData.isAxis = true
  scene.add(yAxis)

  // Y轴箭头
  const yArrowGeometry = new THREE.ConeGeometry(arrowRadius, arrowLength, 8)
  const yArrowMaterial = new THREE.MeshBasicMaterial({ color: 0x00ff00 })
  yArrow = new THREE.Mesh(yArrowGeometry, yArrowMaterial)
  yArrow.position.set(0, baseAxisLength + arrowLength / 2, 0)
  yArrow.userData.isTransformable = false
  yArrow.userData.baseLength = baseAxisLength
  yArrow.userData.isAxis = true
  scene.add(yArrow)

  // 创建Z轴（蓝色）
  const zAxisGeometry = new THREE.CylinderGeometry(axisRadius, axisRadius, baseAxisLength, 8)
  const zAxisMaterial = new THREE.MeshBasicMaterial({ color: 0x0000ff })
  zAxis = new THREE.Mesh(zAxisGeometry, zAxisMaterial)
  zAxis.rotation.x = Math.PI / 2
  zAxis.position.set(0, 0, baseAxisLength / 2)
  zAxis.userData.isTransformable = false
  zAxis.userData.baseLength = baseAxisLength
  zAxis.userData.isAxis = true
  scene.add(zAxis)

  // Z轴箭头
  const zArrowGeometry = new THREE.ConeGeometry(arrowRadius, arrowLength, 8)
  const zArrowMaterial = new THREE.MeshBasicMaterial({ color: 0x0000ff })
  zArrow = new THREE.Mesh(zArrowGeometry, zArrowMaterial)
  zArrow.rotation.x = Math.PI / 2
  zArrow.position.set(0, 0, baseAxisLength + arrowLength / 2)
  zArrow.userData.isTransformable = false
  zArrow.userData.baseLength = baseAxisLength
  zArrow.userData.isAxis = true
  scene.add(zArrow)

  // 创建X轴标签
  const xLabelCanvas = document.createElement('canvas')
  const xLabelCtx = xLabelCanvas.getContext('2d')
  xLabelCanvas.width = 64
  xLabelCanvas.height = 64
  xLabelCtx!.fillStyle = '#ff0000'
  xLabelCtx!.font = 'bold 48px Arial'
  xLabelCtx!.textAlign = 'center'
  xLabelCtx!.textBaseline = 'middle'
  xLabelCtx!.fillText('X', 32, 32)
  const xLabelTexture = new THREE.CanvasTexture(xLabelCanvas)
  const xLabelMaterial = new THREE.SpriteMaterial({ map: xLabelTexture })
  xLabelSprite = new THREE.Sprite(xLabelMaterial)
  xLabelSprite.position.set(baseAxisLength + arrowLength + 0.5, 0, 0)
  xLabelSprite.scale.set(0.8, 0.8, 0.8)
  xLabelSprite.userData.isTransformable = false
  xLabelSprite.userData.baseLength = baseAxisLength
  xLabelSprite.userData.isAxis = true
  scene.add(xLabelSprite)

  // 创建Y轴标签
  const yLabelCanvas = document.createElement('canvas')
  const yLabelCtx = yLabelCanvas.getContext('2d')
  yLabelCanvas.width = 64
  yLabelCanvas.height = 64
  yLabelCtx!.fillStyle = '#00ff00'
  yLabelCtx!.font = 'bold 48px Arial'
  yLabelCtx!.textAlign = 'center'
  yLabelCtx!.textBaseline = 'middle'
  yLabelCtx!.fillText('Y', 32, 32)
  const yLabelTexture = new THREE.CanvasTexture(yLabelCanvas)
  const yLabelMaterial = new THREE.SpriteMaterial({ map: yLabelTexture })
  yLabelSprite = new THREE.Sprite(yLabelMaterial)
  yLabelSprite.position.set(0, baseAxisLength + arrowLength + 0.5, 0)
  yLabelSprite.scale.set(0.8, 0.8, 0.8)
  yLabelSprite.userData.isTransformable = false
  yLabelSprite.userData.baseLength = baseAxisLength
  yLabelSprite.userData.isAxis = true
  scene.add(yLabelSprite)

  // 创建Z轴标签
  const zLabelCanvas = document.createElement('canvas')
  const zLabelCtx = zLabelCanvas.getContext('2d')
  zLabelCanvas.width = 64
  zLabelCanvas.height = 64
  zLabelCtx!.fillStyle = '#0000ff'
  zLabelCtx!.font = 'bold 48px Arial'
  zLabelCtx!.textAlign = 'center'
  zLabelCtx!.textBaseline = 'middle'
  zLabelCtx!.fillText('Z', 32, 32)
  const zLabelTexture = new THREE.CanvasTexture(zLabelCanvas)
  const zLabelMaterial = new THREE.SpriteMaterial({ map: zLabelTexture })
  zLabelSprite = new THREE.Sprite(zLabelMaterial)
  zLabelSprite.position.set(0, 0, baseAxisLength + arrowLength + 0.5)
  zLabelSprite.scale.set(0.8, 0.8, 0.8)
  zLabelSprite.userData.isTransformable = false
  zLabelSprite.userData.baseLength = baseAxisLength
  zLabelSprite.userData.isAxis = true
  scene.add(zLabelSprite)

  // 添加环境光
  const ambientLight = new THREE.AmbientLight(0xffffff, 0.4)
  scene.add(ambientLight)

  // 添加主方向光
  const directionalLight = new THREE.DirectionalLight(0xffffff, 1.5)
  directionalLight.position.set(10, 20, 10)
  directionalLight.castShadow = true
  directionalLight.shadow.mapSize.width = 2048
  directionalLight.shadow.mapSize.height = 2048
  directionalLight.shadow.camera.near = 0.1
  directionalLight.shadow.camera.far = 100
  directionalLight.shadow.camera.left = -20
  directionalLight.shadow.camera.right = 20
  directionalLight.shadow.camera.top = 20
  directionalLight.shadow.camera.bottom = -20
  directionalLight.shadow.bias = -0.0001
  scene.add(directionalLight)

  // 添加辅助方向光（侧面补光）
  const secondaryLight = new THREE.DirectionalLight(0xffffff, 0.5)
  secondaryLight.position.set(-10, 10, -10)
  scene.add(secondaryLight)

  // 添加底部补光
  const bottomLight = new THREE.DirectionalLight(0xffffff, 0.3)
  bottomLight.position.set(0, -10, 0)
  scene.add(bottomLight)

  // 创建射线投射器和鼠标向量
  raycaster = new THREE.Raycaster()
  mouse = new THREE.Vector2()

  // 创建渲染管理器
  renderManager = new RenderManager(scene)

  // 添加事件监听器
  window.addEventListener('resize', onWindowResize)
  renderer.domElement.addEventListener('click', onMouseClick)
  renderer.domElement.addEventListener('contextmenu', onRightClick)
  document.addEventListener('click', onDocumentClick)
  // 添加ESC键事件监听器
  window.addEventListener('keydown', onKeyDown)

}

// 窗口大小改变时更新渲染
const onWindowResize = () => {
  camera.aspect = window.innerWidth / window.innerHeight
  camera.updateProjectionMatrix()
  renderer.setSize(window.innerWidth, window.innerHeight)
  labelRenderer.setSize(window.innerWidth, window.innerHeight)
}

// 鼠标点击事件
const onMouseClick = (event: MouseEvent) => {

  if (event.button === 0) { // 左键点击
    mouse.x = (event.clientX / window.innerWidth) * 2 - 1
    mouse.y = -(event.clientY / window.innerHeight) * 2 + 1

    raycaster.setFromCamera(mouse, camera)
    // 使用objects数组进行射线检测
    console.log(objects);
    const intersects = raycaster.intersectObjects(objects,false)
    if (intersects.length > 0) {
      // 找到第一个可变换的相交对象
      let targetObject: THREE.Object3D | null = null
      targetObject = intersects[0].object

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

  // 计算鼠标位置
  const mouse = new THREE.Vector2()
  mouse.x = (event.clientX / window.innerWidth) * 2 - 1
  mouse.y = -(event.clientY / window.innerHeight) * 2 + 1

  // 创建射线
  raycaster.setFromCamera(mouse, camera)
  // 使用objects数组进行射线检测
  const intersects = raycaster.intersectObjects(objects)

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

// 键盘事件处理
const onKeyDown = (event: KeyboardEvent) => {
  // 按下ESC键时退出编辑模式
  if (event.key === 'Escape' && transformControls.object) {
    exitEditMode()
    // 取消选择对象，确保UI完全退出编辑状态
    deselectObject()
  }
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
  // 更新对象名称
  objectName.value = object.name || ''
  // 更新对象颜色
  if (object instanceof THREE.Mesh) {
    const material = object.material
    if (material instanceof THREE.Material && 'color' in material) {
      objectColor.value = '#' + material.color.getHexString()
    }
    // 更新内发光颜色和强度
    if (material instanceof THREE.MeshStandardMaterial) {
      emissiveColor.value = '#' + material.emissive.getHexString()
      material.emissiveIntensity = 1.2
      emissiveIntensity.value = 1.2
    }
  }
  // 更新标签设置
  labelSize.value = object.userData.labelSize || 16
  labelColor.value = object.userData.labelColor || '#ffffff'

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

// 通用函数：在执行操作前退出编辑模式
const ensureExitEditMode = () => {
  if (transformControls.object) {
    exitEditMode()
    deselectObject()
  }
}

// 创建标签函数
const createLabel = (object: THREE.Object3D, text: string) => {
  const div = document.createElement('div')
  div.className = 'object-label'
  div.textContent = text
  div.style.color = object.userData.labelColor || '#ffffff'
  div.style.fontSize = (object.userData.labelSize || 16) + 'px'
  div.style.fontWeight = 'bold'
  div.style.textShadow = '1px 1px 2px rgba(0,0,0,0.8)'
  div.style.pointerEvents = 'none'
  div.style.whiteSpace = 'nowrap'

  const label = new CSS2DObject(div)
  
  if (object instanceof THREE.Mesh) {
    object.geometry.computeBoundingBox()
    const boundingBox = object.geometry.boundingBox
    const height = boundingBox.max.y - boundingBox.min.y
    label.position.set(0, height / 2 + 0.2, 0)
  } else {
    const box = new THREE.Box3().setFromObject(object)
    label.position.set(0, box.max.y + 0.2, 0)
  }
  
  object.add(label)
  
  return label
}

// 更新标签函数
const updateLabel = (object: THREE.Object3D) => {
  const label = object.children.find(child => child instanceof CSS2DObject)
  if (label && label.element instanceof HTMLElement) {
    label.element.textContent = object.name || '未命名'
    label.element.style.color = object.userData.labelColor || '#ffffff'
    label.element.style.fontSize = (object.userData.labelSize || 16) + 'px'
  }
}

// 将材质转换为纯金属材质
const convertToMetalMaterial = (mesh: THREE.Mesh) => {
  if (mesh.material) {
    if (Array.isArray(mesh.material)) {
      mesh.material.forEach((mat) => {
        if (mat instanceof THREE.MeshStandardMaterial) {
          mat.roughness = 0.5
          mat.metalness = 1.0
          mat.emissive = new THREE.Color(0x444444)
          mat.emissiveIntensity = 1.2
        }
      })
    } else if (mesh.material instanceof THREE.MeshStandardMaterial) {
      mesh.material.roughness = 0.5
      mesh.material.metalness = 1.0
      mesh.material.emissive = new THREE.Color(0x444444)
      mesh.material.emissiveIntensity = 1.2
    }
  }
}

// 添加基础形状
const addBasicShape = (shapeType: string) => {
  // 确保退出编辑模式
  ensureExitEditMode()
  
  let geometry: THREE.BufferGeometry
  const randomColor = Math.random() * 0xffffff
  let material = new THREE.MeshStandardMaterial({
    color: randomColor,
    roughness: 0.5,
    metalness: 1.0,
    emissive: new THREE.Color(randomColor),
    emissiveIntensity: 1.2
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

  // 创建标签
  createLabel(mesh, mesh.name)

  // 将mesh对象添加到objects数组中
  objects.push(mesh)
}

// 触发文件输入对话框
const triggerFileInput = () => {
  // 确保退出编辑模式
  ensureExitEditMode()
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

                // 将材质转换为纯金属材质
                convertToMetalMaterial(mesh)

                // 计算并调整模型几何中心到原点，使变换操作以几何中心为基准
                mesh.geometry.computeBoundingBox()
                const boundingBox = mesh.geometry.boundingBox
                const center = new THREE.Vector3()
                boundingBox.getCenter(center)
                mesh.geometry.translate(-center.x, -center.y, -center.z)
                mesh.position.set(0, 0, 0)

                // 标记为可变换的对象
                mesh.userData.isTransformable = true
                // 标记为GLB模型的一部分
                mesh.userData.isGLB = true
                // 保存原始文件名
                mesh.userData.originalFileName = file.name
                // 保存public路径信息，假设文件已保存到public文件夹
                mesh.userData.publicPath = `/${file.name}`

                // 直接将mesh对象添加到场景中
                scene.add(mesh)
                meshObjects.push(mesh)

                // 将mesh对象添加到objects数组中
                objects.push(mesh)
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
  // 确保退出编辑模式
  ensureExitEditMode()
  
  const sceneData = {
    objects: []
  }

  // 从objects数组中导出对象，而不是从scene中遍历
  objects.forEach((child) => {
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
      if (child.userData.isGLB === true) {
        // 对于GLB模型的mesh对象
        objectData.type = 'gltf'

        // 保存GLB模型的原始文件名（如果有）
        if (child.userData.originalFileName) {
          objectData.originalFileName = child.userData.originalFileName
          // 保存模型路径，确保路径格式正确
          objectData.modelPath = `/${child.userData.originalFileName}`
          
          // 检查是否有其他路径信息
          if (child.userData.publicPath) {
            objectData.modelPath = child.userData.publicPath
          }
        }
      } else if (child instanceof THREE.Mesh) {
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
            emissive: child.material.emissive.getHex(),
            emissiveIntensity: child.material.emissiveIntensity,
            transparent: child.material.transparent,
            opacity: child.material.opacity
          }
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

// 触发导入输入对话框
const triggerImportInput = () => {
  // 确保退出编辑模式
  ensureExitEditMode()
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
        // 删除对象的所有标签（CSS2DObject）
        const labelsToRemove: THREE.Object3D[] = []
        obj.traverse((child) => {
          if (child instanceof CSS2DObject) {
            labelsToRemove.push(child)
          }
        })
        labelsToRemove.forEach(label => {
          label.parent?.remove(label)
        })

        scene.remove(obj)
        // 从objects数组中移除对象
        const index = objects.indexOf(obj)
        if (index !== -1) {
          objects.splice(index, 1)
        }
      })

      // 清空objects数组
      objects.length = 0

      // 记录需要加载的GLB模型数量
      const glbModelsToLoad = sceneData.objects.filter((obj: any) =>
        obj.type === 'gltf' && obj.originalFileName
      ).length

      // 记录成功加载的GLB模型数量
      let successfullyLoadedGLBCount = 0
      
      // 记录加载失败的GLB模型
      const failedGLBModels: string[] = []

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
              roughness: 0.5,
              metalness: 1.0,
              emissive: objData.material.emissive !== undefined ? objData.material.emissive : 0x444444,
              emissiveIntensity: objData.material.emissiveIntensity !== undefined ? objData.material.emissiveIntensity : 1.2,
              transparent: objData.material.transparent,
              opacity: objData.material.opacity
            })
          } else {
            // 默认材质
            material = new THREE.MeshStandardMaterial({
              roughness: 0.5,
              metalness: 1.0,
              emissive: 0x444444,
              emissiveIntensity: 1.2
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
          // 确保基础几何体位于地面上方
          if (mesh.position.y <= 0) {
            mesh.position.y = 0.5
          }
          mesh.rotation.fromArray(objData.rotation)
          mesh.scale.fromArray(objData.scale)
          mesh.visible = objData.visible !== undefined ? objData.visible : true
          mesh.castShadow = objData.castShadow !== undefined ? objData.castShadow : true
          mesh.receiveShadow = objData.receiveShadow !== undefined ? objData.receiveShadow : true

          // 恢复userData
          mesh.userData = { ...objData.userData }
          mesh.userData.isTransformable = true

          scene.add(mesh)
          
          // 创建标签
          createLabel(mesh, mesh.name)
          
          // 将mesh对象添加到objects数组中
          objects.push(mesh)
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
                  
                  // 创建标签
                  createLabel(mesh, mesh.name)
                  
                  // 将加载的GLB模型添加到objects数组中
                  objects.push(mesh)
                })

                successfullyLoadedGLBCount++
                console.log(`成功从public文件夹加载GLB模型: ${objData.originalFileName}，包含 ${meshes.length} 个网格对象`)
              })
              .catch(error => {
                console.warn(`无法从public文件夹加载GLB模型: ${objData.originalFileName}`, error)
                
                // 记录需要手动加载的模型
                if (!failedGLBModels.includes(objData.originalFileName)) {
                  failedGLBModels.push(objData.originalFileName)
                }
              })
          } else {
            // 如果没有模型路径信息，创建一个占位符
            const placeholderGeometry = new THREE.BoxGeometry(1, 1, 1)
            const placeholderMaterial = new THREE.MeshStandardMaterial({
              color: 0x00ff00,
              roughness: 0.5,
              metalness: 1.0,
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
            
            // 创建标签
            createLabel(placeholderMesh, placeholderMesh.name)
            
            // 将占位符mesh对象添加到objects数组中
            objects.push(placeholderMesh)
          }
        }
      })

      // 延迟检查，等待所有GLB模型加载完成
      setTimeout(() => {
        // 只有当有GLB模型需要加载但没有全部成功加载时，才显示提示
        if (failedGLBModels.length > 0) {
          const modelList = failedGLBModels.join(', ')
          alert(`以下GLB模型未找到，请手动添加到public文件夹中：\n${modelList}`)
        }
      }, 1000)
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

    // 确保路径格式正确
    let path = modelPath
    if (!path.startsWith('/') && !path.startsWith('./')) {
      path = './' + path
    }
    
    console.log(`尝试从路径加载GLB模型: ${path}, 文件名: ${fileName}`)

    loader.load(
      path,
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

            // 将材质转换为纯金属材质
            convertToMetalMaterial(mesh)

            // 计算并调整模型几何中心到原点，使变换操作以几何中心为基准
            mesh.geometry.computeBoundingBox()
            const boundingBox = mesh.geometry.boundingBox
            const center = new THREE.Vector3()
            boundingBox.getCenter(center)
            mesh.geometry.translate(-center.x, -center.y, -center.z)
            mesh.position.set(0, 0, 0)

            // 标记为可变换的对象
            mesh.userData.isTransformable = true
            // 标记为GLB模型的一部分
            mesh.userData.isGLB = true
            // 保存原始文件名
            mesh.userData.originalFileName = fileName
            // 保存public路径信息，以便导出时使用
            mesh.userData.publicPath = modelPath

            // 直接将mesh对象添加到场景中
            scene.add(mesh)
            
            // 创建标签
            createLabel(mesh, mesh.name)
            
            meshObjects.push(mesh)

            // 将mesh对象添加到objects数组中
            objects.push(mesh)
          }
        })
        console.log(`成功加载GLB模型: ${fileName}，包含 ${meshObjects.length} 个网格对象`)
        resolve(meshObjects)
      },
      undefined,
      (error) => {
        console.error(`从public文件夹加载GLB模型时出错 (${path}):`, error)
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

          // 将材质转换为纯金属材质
          convertToMetalMaterial(mesh)

          // 计算并调整模型几何中心到原点，使变换操作以几何中心为基准
          mesh.geometry.computeBoundingBox()
          const boundingBox = mesh.geometry.boundingBox
          const center = new THREE.Vector3()
          boundingBox.getCenter(center)
          mesh.geometry.translate(-center.x, -center.y, -center.z)
          mesh.position.set(0, 0, 0)

          // 标记为可变换的对象
          mesh.userData.isTransformable = true
          // 标记为GLB模型的一部分
          mesh.userData.isGLB = true
          // 保存原始文件名
          mesh.userData.originalFileName = file.name
          // 保存public路径信息，假设文件已保存到public文件夹
          mesh.userData.publicPath = `/${file.name}`

          // 直接将mesh对象添加到场景中
          scene.add(mesh)
          
          // 创建标签
          createLabel(mesh, mesh.name)
          
          // 将mesh对象添加到objects数组中
          objects.push(mesh)
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
      const index = objects.indexOf(selectedObject.value)
      if (index !== -1) {
        objects.splice(index, 1)
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
  // 取消选择对象，确保UI完全退出编辑状态
  deselectObject()
  contextMenuVisible.value = false
}

// 删除选中的对象
const deleteSelectedObject = () => {
  if (!selectedObject.value) return

  // 检查对象是否可以被删除
  if (selectedObject.value.userData.isTransformable === false) return

  // 如果当前处于编辑模式，先退出编辑模式
  if (transformControls.object) {
    exitEditMode()
  }

  // 删除对象的所有标签（CSS2DObject）
  const labelsToRemove: THREE.Object3D[] = []
  selectedObject.value.traverse((child) => {
    if (child instanceof CSS2DObject) {
      labelsToRemove.push(child)
    }
  })
  labelsToRemove.forEach(label => {
    label.parent?.remove(label)
  })

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
  const index = objects.indexOf(selectedObject.value)
  if (index !== -1) {
    objects.splice(index, 1)
  }

  // 取消选择
  deselectObject()
}

// 退出编辑模式
const exitEditMode = () => {
  if (transformControls.object) {
    // 在退出编辑模式前，对当前对象进行落地操作
    const currentObject = transformControls.object
    groundObject(currentObject)
    
    transformControls.detach()
    // 确保轨道控制器可用
    controls.enabled = true
    // 清空transformControlsRef的object引用，确保UI正确更新
    transformControlsRef.value = null
    // 重新创建TransformControls实例以备下次使用
    transformControlsRef.value = transformControls
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
  // 更新对象名称
  objectName.value = selectedObject.value.name || ''
  // 更新对象颜色
  if (selectedObject.value instanceof THREE.Mesh) {
    const material = selectedObject.value.material
    if (material instanceof THREE.Material && 'color' in material) {
      objectColor.value = '#' + material.color.getHexString()
    }
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

// 更新对象名称
const updateObjectName = () => {
  if (selectedObject.value && transformControlsRef.value?.object === selectedObject.value) {
    selectedObject.value.name = objectName.value
    // 更新标签
    updateLabel(selectedObject.value)
  }
}

// 更新对象颜色
const updateObjectColor = () => {
  if (selectedObject.value && transformControlsRef.value?.object === selectedObject.value) {
    if (selectedObject.value instanceof THREE.Mesh) {
      const material = selectedObject.value.material
      if (material instanceof THREE.Material) {
        material.color.set(objectColor.value)
      } else if (Array.isArray(material)) {
        material.forEach(mat => mat.color.set(objectColor.value))
      }
    }
  }
}

// 更新标签大小
const updateLabelSize = () => {
  if (selectedObject.value && transformControlsRef.value?.object === selectedObject.value) {
    selectedObject.value.userData.labelSize = labelSize.value
    // 更新标签
    updateLabel(selectedObject.value)
  }
}

// 更新标签颜色
const updateLabelColor = () => {
  if (selectedObject.value && transformControlsRef.value?.object === selectedObject.value) {
    selectedObject.value.userData.labelColor = labelColor.value
    // 更新标签
    updateLabel(selectedObject.value)
  }
}

// 获取对象颜色的十六进制值
const getObjectColorHex = (): string => {
  if (selectedObject.value && selectedObject.value instanceof THREE.Mesh) {
    const material = selectedObject.value.material
    if (material instanceof THREE.Material && 'color' in material) {
      return '#' + material.color.getHexString()
    }
  }
  return '#ffffff'
}

// 更新内发光颜色
const updateEmissiveColor = () => {
  if (selectedObject.value && transformControlsRef.value?.object === selectedObject.value && selectedObject.value instanceof THREE.Mesh) {
    const material = selectedObject.value.material
    if (material instanceof THREE.MeshStandardMaterial) {
      material.emissive = new THREE.Color(emissiveColor.value)
    }
  }
}

// 更新内发光强度
const updateEmissiveIntensity = () => {
  if (selectedObject.value && transformControlsRef.value?.object === selectedObject.value && selectedObject.value instanceof THREE.Mesh) {
    const material = selectedObject.value.material
    if (material instanceof THREE.MeshStandardMaterial) {
      material.emissiveIntensity = emissiveIntensity.value
    }
  }
}

// 获取内发光颜色的十六进制值
const getEmissiveColorHex = (): string => {
  if (selectedObject.value && selectedObject.value instanceof THREE.Mesh) {
    const material = selectedObject.value.material
    if (material instanceof THREE.MeshStandardMaterial) {
      return '#' + material.emissive.getHexString()
    }
  }
  return '#444444'
}

// 获取内发光强度
const getEmissiveIntensity = (): string => {
  if (selectedObject.value && selectedObject.value instanceof THREE.Mesh) {
    const material = selectedObject.value.material
    if (material instanceof THREE.MeshStandardMaterial) {
      return material.emissiveIntensity.toFixed(1)
    }
  }
  return '0.5'
}

// 格式化向量显示
const formatVector = (vector: THREE.Vector3 | THREE.Euler) => {
  return `(${vector.x.toFixed(2)}, ${vector.y.toFixed(2)}, ${vector.z.toFixed(2)})`
}

// 动画循环
const updateAxisSize = () => {
  if (!camera || !xAxis || !yAxis || !zAxis) return

  const distance = camera.position.distanceTo(new THREE.Vector3(0, 0, 0))
  const baseAxisLength = 6
  const minScale = 0.5
  const maxScale = 10
  const referenceDistance = 12

  let scale = distance / referenceDistance
  scale = Math.max(minScale, Math.min(maxScale, scale))

  const currentAxisLength = baseAxisLength * scale
  const arrowLength = 0.8 * scale
  const arrowRadius = 0.03 * scale
  const axisRadius = 0.01 * scale

  xAxis.scale.set(scale, 1, 1)
  xAxis.position.set(currentAxisLength / 2, 0, 0)
  xArrow.scale.set(scale, scale, scale)
  xArrow.position.set(currentAxisLength + arrowLength / 2, 0, 0)
  xLabelSprite.position.set(currentAxisLength + arrowLength + 0.5 * scale, 0, 0)
  xLabelSprite.scale.set(0.8 * scale, 0.8 * scale, 0.8 * scale)

  yAxis.scale.set(1, scale, 1)
  yAxis.position.set(0, currentAxisLength / 2, 0)
  yArrow.scale.set(scale, scale, scale)
  yArrow.position.set(0, currentAxisLength + arrowLength / 2, 0)
  yLabelSprite.position.set(0, currentAxisLength + arrowLength + 0.5 * scale, 0)
  yLabelSprite.scale.set(0.8 * scale, 0.8 * scale, 0.8 * scale)

  zAxis.scale.set(1, 1, scale)
  zAxis.position.set(0, 0, currentAxisLength / 2)
  zArrow.scale.set(scale, scale, scale)
  zArrow.position.set(0, 0, currentAxisLength + arrowLength / 2)
  zLabelSprite.position.set(0, 0, currentAxisLength + arrowLength + 0.5 * scale)
  zLabelSprite.scale.set(0.8 * scale, 0.8 * scale, 0.8 * scale)
}

const calculateAutoScaleForView = (camera: THREE.PerspectiveCamera, objectSize: THREE.Vector3): number => {
  if (!camera) {
    return 1.0
  }

  const maxDimension = Math.max(objectSize.x, objectSize.y, objectSize.z)
  
  if (maxDimension === 0) {
    return 1.0
  }

  const groundSize = 10
  const targetScale = groundSize / maxDimension
  
  const clampedScale = Math.max(0.001, Math.min(targetScale, 100))
  
  console.log(`calculateAutoScaleForView: maxDimension=${maxDimension}, groundSize=${groundSize}, targetScale=${targetScale}, clampedScale=${clampedScale}`)
  
  return clampedScale
}

const animate = () => {
  animationId = requestAnimationFrame(animate)
  controls.update()
  updateAxisSize()
  composer.render()
  labelRenderer.render(scene, camera)
}

// 组件挂载时初始化
onMounted(() => {
  console.log('App mounted, showDxfUpload:', showDxfUpload.value)
  nextTick(() => {
    console.log('nextTick called, canvasContainer:', canvasContainer.value)
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
  window.removeEventListener('keydown', onKeyDown)

  // 清理渲染管理器
  if (renderManager) {
    renderManager.dispose()
    renderManager = null
  }

  renderer.dispose()
})

// 处理DXF数据返回
const handleDxfDataReturn = (dxfData: string | undefined) => {
  showDxfUpload.value = false
  
  // 强制更新渲染器大小，确保场景正常显示
  nextTick(() => {
    if (renderer && camera) {
      camera.aspect = window.innerWidth / window.innerHeight
      camera.updateProjectionMatrix()
      renderer.setSize(window.innerWidth, window.innerHeight)
      renderer.render(scene, camera)
    }
  })
  
  if (!dxfData) {
    console.warn('No DXF data received')
    return
  }

  try {
    const parsedData = JSON.parse(dxfData)
    console.log('Received DXF data:', parsedData)
    console.log('DXF data keys:', Object.keys(parsedData))
    
    // 检查数据格式
    Object.keys(parsedData).forEach(key => {
      if (Array.isArray(parsedData[key]) && parsedData[key].length > 0) {
        console.log(`  ${key}: ${parsedData[key].length} items`)
        console.log(`  First item of ${key}:`, parsedData[key][0])
      }
    })

    // 使用RenderManager渲染DXF数据
    if (renderManager) {
      // 强制启用Y轴居中，确保对象Y轴中心在原点
      const centeringOptionsForImport = {
        ...centeringOptions.value,
        centerY: true
      }
      renderManager.setCenteringOptions(centeringOptionsForImport)
      
      const boundingBox = renderManager.renderDxfData(parsedData)
      console.log('Bounding box after render:', boundingBox)
      
      // 应用DXF对象颜色设置
      updateDxfObjectColor()
      
      const size = new THREE.Vector3()
      boundingBox.getSize(size)
      console.log('Object size before scaling:', size)
      
      const autoScale = calculateAutoScaleForView(camera, size)
      console.log('Auto scale calculated:', autoScale)
      
      if (autoScale !== 1.0) {
        renderManager.setScale(autoScale)
        dxfScale.value = autoScale
        console.log(`Auto-scaled objects to ${autoScale.toFixed(4)} to fit in view`)
        
        const newBoundingBox = renderManager.getBoundingBox()
        const newSize = new THREE.Vector3()
        newBoundingBox.getSize(newSize)
        console.log('Object size after scaling:', newSize)
      }
      
      dxfFlipX.value = -90
      dxfFlipY.value = 0
      dxfFlipZ.value = 0
      renderManager.setFlipRotation(-Math.PI / 2, 0, 0)
      
      // 强制更新场景渲染
      console.log('Forcing scene update after DXF render')
      if (renderer && camera) {
        renderer.render(scene, camera)
      }
    }
  } catch (error) {
    console.error('Error parsing DXF data:', error)
    alert('解析DXF数据失败')
  }
}

// 处理DXF比例变化
const handleDxfScaleChange = () => {
  // 实时预览缩放效果
  if (renderManager && dxfScale.value > 0) {
    renderManager.setScale(dxfScale.value)
  }
}

// 应用DXF比例
const applyDxfScale = () => {
  if (renderManager && dxfScale.value > 0) {
    renderManager.setScale(dxfScale.value)
    console.log(`Applied DXF scale: ${dxfScale.value}`)
  }
}

// 处理DXF翻转变化
const handleDxfFlipChange = () => {
  // 实时预览翻转效果
  if (renderManager) {
    const xRad = (dxfFlipX.value * Math.PI) / 180
    const yRad = (dxfFlipY.value * Math.PI) / 180
    const zRad = (dxfFlipZ.value * Math.PI) / 180
    renderManager.setFlipRotation(xRad, yRad, zRad)
  }
}

// 更新场景背景颜色
const updateSceneBackgroundColor = () => {
  if (scene) {
    scene.background = new THREE.Color(sceneBackgroundColor.value)
    console.log('Scene background color updated:', sceneBackgroundColor.value)
  }
}

// 更新DXF对象颜色
const updateDxfObjectColor = () => {
  if (renderManager) {
    renderManager.setDxfObjectColor(dxfObjectColor.value)
    console.log('DXF object color updated:', dxfObjectColor.value)
  }
}

// 应用DXF翻转
const applyDxfFlip = () => {
  if (renderManager) {
    const xRad = (dxfFlipX.value * Math.PI) / 180
    const yRad = (dxfFlipY.value * Math.PI) / 180
    const zRad = (dxfFlipZ.value * Math.PI) / 180
    renderManager.setFlipRotation(xRad, yRad, zRad)
    console.log(`Applied DXF flip: X=${dxfFlipX.value}°, Y=${dxfFlipY.value}°, Z=${dxfFlipZ.value}°`)
  }
  
  applyGLBRotation()
}

// 重置DXF翻转
const resetDxfFlip = () => {
  dxfFlipX.value = 0
  dxfFlipY.value = 0
  dxfFlipZ.value = 0
  if (renderManager) {
    renderManager.setFlipRotation(0, 0, 0)
    console.log('Reset DXF flip to (0, 0, 0)')
  }
  
  resetGLBRotation()
}

// 应用GLB模型旋转（以原点为中心）
const applyGLBRotation = () => {
  const xRad = (dxfFlipX.value * Math.PI) / 180
  const yRad = (dxfFlipY.value * Math.PI) / 180
  const zRad = (dxfFlipZ.value * Math.PI) / 180
  
  const rotationDiff = {
    x: xRad - glbRotation.x,
    y: yRad - glbRotation.y,
    z: zRad - glbRotation.z
  }
  
  const glbObjects = objects.filter(obj => obj.userData.isGLB === true)
  glbObjects.forEach(obj => {
    obj.rotation.x += rotationDiff.x
    obj.rotation.y += rotationDiff.y
    obj.rotation.z += rotationDiff.z
    
    obj.position.applyAxisAngle(new THREE.Vector3(1, 0, 0), rotationDiff.x)
    obj.position.applyAxisAngle(new THREE.Vector3(0, 1, 0), rotationDiff.y)
    obj.position.applyAxisAngle(new THREE.Vector3(0, 0, 1), rotationDiff.z)
  })
  
  glbRotation = { x: xRad, y: yRad, z: zRad }
  console.log(`Applied GLB rotation: X=${dxfFlipX.value}°, Y=${dxfFlipY.value}°, Z=${dxfFlipZ.value}° around origin`)
}

// 重置GLB模型旋转
const resetGLBRotation = () => {
  const rotationDiff = {
    x: 0 - glbRotation.x,
    y: 0 - glbRotation.y,
    z: 0 - glbRotation.z
  }
  
  const glbObjects = objects.filter(obj => obj.userData.isGLB === true)
  glbObjects.forEach(obj => {
    obj.rotation.x += rotationDiff.x
    obj.rotation.y += rotationDiff.y
    obj.rotation.z += rotationDiff.z
    
    obj.position.applyAxisAngle(new THREE.Vector3(1, 0, 0), rotationDiff.x)
    obj.position.applyAxisAngle(new THREE.Vector3(0, 1, 0), rotationDiff.y)
    obj.position.applyAxisAngle(new THREE.Vector3(0, 0, 1), rotationDiff.z)
  })
  
  glbRotation = { x: 0, y: 0, z: 0 }
  console.log('Reset GLB rotation to (0, 0, 0)')
}

// 计算GLB模型的边界框
const calculateGLBBoundingBox = () => {
  glbBoundingBox.makeEmpty()
  const glbObjects = objects.filter(obj => obj.userData.isGLB === true)
  
  if (glbObjects.length === 0) {
    console.warn('没有找到GLB模型对象')
    return false
  }
  
  glbObjects.forEach(obj => {
    glbBoundingBox.expandByObject(obj)
  })
  
  return true
}

// 保存GLB模型的原始位置
const saveGLBOriginalPositions = () => {
  glbOriginalPositions.clear()
  const glbObjects = objects.filter(obj => obj.userData.isGLB === true)
  
  glbObjects.forEach(obj => {
    glbOriginalPositions.set(obj, obj.position.clone())
  })
  
  console.log(`保存了 ${glbOriginalPositions.size} 个GLB对象的原始位置`)
}

// 应用GLB模型中心化
const applyGLBCentering = () => {
  if (!calculateGLBBoundingBox()) {
    console.warn('无法计算GLB模型边界框')
    return
  }
  
  if (glbIsCentered) {
    console.warn('GLB模型已经中心化')
    return
  }
  
  glbBoundingBox.getCenter(glbOriginalCenter)
  console.log(`GLB模型原始中心: (${glbOriginalCenter.x.toFixed(2)}, ${glbOriginalCenter.y.toFixed(2)}, ${glbOriginalCenter.z.toFixed(2)})`)
  
  const offset = new THREE.Vector3(0, 0, 0)
  if (centeringOptions.value.centerX) {
    offset.x = -glbOriginalCenter.x
  }
  if (centeringOptions.value.centerY) {
    offset.y = -glbOriginalCenter.y
  }
  if (centeringOptions.value.centerZ) {
    offset.z = -glbOriginalCenter.z
  }
  
  saveGLBOriginalPositions()
  
  const glbObjects = objects.filter(obj => obj.userData.isGLB === true)
  glbObjects.forEach(obj => {
    obj.position.add(offset)
  })
  
  glbIsCentered = true
  console.log(`GLB模型中心化完成，偏移量: (${offset.x.toFixed(2)}, ${offset.y.toFixed(2)}, ${offset.z.toFixed(2)})`)
}

// 重置GLB模型到原始位置
const resetGLBToOriginalPosition = () => {
  if (!glbIsCentered) {
    console.warn('GLB模型未中心化，无需重置')
    return
  }
  
  glbOriginalPositions.forEach((originalPos, obj) => {
    obj.position.copy(originalPos)
  })
  
  glbIsCentered = false
  console.log('GLB模型已重置到原始位置')
}

// 打印GLB模型转换信息
const printGLBTransformInfo = () => {
  if (!calculateGLBBoundingBox()) {
    console.warn('无法计算GLB模型边界框')
    return
  }
  
  const currentCenter = new THREE.Vector3()
  glbBoundingBox.getCenter(currentCenter)
  
  const size = new THREE.Vector3()
  glbBoundingBox.getSize(size)
  
  console.log('========== GLB模型坐标转换信息 ==========')
  console.log('原始中心:', `(${glbOriginalCenter.x.toFixed(2)}, ${glbOriginalCenter.y.toFixed(2)}, ${glbOriginalCenter.z.toFixed(2)})`)
  console.log('当前中心:', `(${currentCenter.x.toFixed(2)}, ${currentCenter.y.toFixed(2)}, ${currentCenter.z.toFixed(2)})`)
  console.log('边界框尺寸:', `(${size.x.toFixed(2)}, ${size.y.toFixed(2)}, ${size.z.toFixed(2)})`)
  console.log('是否已中心化:', glbIsCentered)
  console.log('中心化选项:', `X=${centeringOptions.value.centerX}, Y=${centeringOptions.value.centerY}, Z=${centeringOptions.value.centerZ}`)
  console.log('=========================================')
}

// 验证GLB模型中心化
const verifyGLBCentering = (): boolean => {
  if (!calculateGLBBoundingBox()) {
    console.warn('无法计算GLB模型边界框')
    return false
  }
  
  const currentCenter = new THREE.Vector3()
  glbBoundingBox.getCenter(currentCenter)
  
  const tolerance = 0.001
  const isXCentered = Math.abs(currentCenter.x) < tolerance
  const isYCentered = Math.abs(currentCenter.y) < tolerance
  const isZCentered = Math.abs(currentCenter.z) < tolerance
  
  const expectedXCentered = centeringOptions.value.centerX
  const expectedYCentered = centeringOptions.value.centerY
  const expectedZCentered = centeringOptions.value.centerZ
  
  const isValid = (isXCentered === expectedXCentered) && 
                  (isYCentered === expectedYCentered) && 
                  (isZCentered === expectedZCentered)
  
  console.log('========== GLB模型中心化验证 ==========')
  console.log('当前中心:', `(${currentCenter.x.toFixed(6)}, ${currentCenter.y.toFixed(6)}, ${currentCenter.z.toFixed(6)})`)
  console.log('X轴中心化:', isXCentered, '(期望:', expectedXCentered, ')')
  console.log('Y轴中心化:', isYCentered, '(期望:', expectedYCentered, ')')
  console.log('Z轴中心化:', isZCentered, '(期望:', expectedZCentered, ')')
  console.log('验证结果:', isValid ? '通过' : '失败')
  console.log('=====================================')
  
  return isValid
}

// 应用中心化（同时处理DXF和GLB）
const applyCentering = () => {
  if (renderManager) {
    renderManager.setCenteringOptions(centeringOptions.value)
    renderManager.applyCentering()
    console.log('Applied DXF centering with options:', centeringOptions.value)
  }
  
  applyGLBCentering()
}

// 重置到原始位置（同时处理DXF和GLB）
const resetToOriginalPosition = () => {
  if (renderManager) {
    renderManager.resetToOriginalPosition()
    console.log('Reset DXF to original position')
  }
  
  resetGLBToOriginalPosition()
}

// 显示转换信息（同时处理DXF和GLB）
const printTransformInfo = () => {
  if (renderManager) {
    renderManager.printTransformInfo()
  }
  
  printGLBTransformInfo()
}

// 验证中心化（同时处理DXF和GLB）
const verifyCentering = () => {
  let dxfValid = true
  let glbValid = true
  
  if (renderManager) {
    dxfValid = renderManager.verifyCentering()
  }
  
  glbValid = verifyGLBCentering()
  
  const isValid = dxfValid && glbValid
  if (isValid) {
    alert('中心化验证通过！')
  } else {
    alert('中心化验证失败！请查看控制台获取详细信息。')
  }
  
  return isValid
}

// 从public文件夹加载GLB模型列表
const loadGLBFromPublicList = async () => {
  // 确保退出编辑模式
  ensureExitEditMode()
  
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

// 进入顶部视角模式
const enterTopViewMode = () => {
  if (isTopViewMode.value) {
    console.warn('已经在顶部视角模式中')
    return
  }

  // 保存当前相机状态
  originalCameraPosition = camera.position.clone()
  originalCameraTarget = controls.target.clone()
  originalCameraZoom = camera.zoom

  // 计算场景中所有对象的边界框
  const boundingBox = new THREE.Box3()
  objects.forEach(obj => {
    if (obj.userData.isTransformable !== false) {
      boundingBox.expandByObject(obj)
    }
  })

  // 如果没有对象，使用默认的边界框
  if (boundingBox.isEmpty()) {
    boundingBox.setFromCenterAndSize(new THREE.Vector3(0, 0, 0), new THREE.Vector3(10, 10, 10))
  }

  // 获取边界框的中心和尺寸
  const center = new THREE.Vector3()
  const size = new THREE.Vector3()
  boundingBox.getCenter(center)
  boundingBox.getSize(size)

  // 计算相机位置（Y轴正上方）
  const maxDimension = Math.max(size.x, size.y, size.z)
  const cameraDistance = maxDimension * 1.5

  // 设置相机到顶部视角位置
  camera.position.set(center.x, center.y + cameraDistance, center.z)

  // 重置相机的旋转和 up 向量，确保标准顶部视角
  // 设置 up 向量为 (0, 0, 1)，使 Z 轴在屏幕上垂直，X 轴水平
  camera.rotation.set(0, 0, 0)
  camera.up.set(0, 0, 1)

  // 设置相机朝向场景中心
  camera.lookAt(center)

  // 更新OrbitControls的目标点
  controls.target.copy(center)

  // 限制OrbitControls以保持顶部视角（禁用旋转，只允许平移）
  controls.minPolarAngle = 0
  controls.maxPolarAngle = 0
  controls.enableRotate = false

  // 允许平移但限制在水平面上
  controls.enablePan = true

  // 重新配置鼠标按键：左键用于平移，右键也用于平移
  controls.mouseButtons = {
    LEFT: THREE.MOUSE.PAN,
    MIDDLE: THREE.MOUSE.DOLLY,
    RIGHT: THREE.MOUSE.PAN
  }

  // 更新控制器
  controls.update()

  // 设置顶部视角模式标志
  isTopViewMode.value = true

  console.log('进入顶部视角模式')
  console.log('场景中心:', center)
  console.log('场景尺寸:', size)
  console.log('相机位置:', camera.position)
}

// 退出顶部视角模式
const exitTopViewMode = () => {
  if (!isTopViewMode.value) {
    console.warn('不在顶部视角模式中')
    return
  }

  // 恢复之前的相机状态
  camera.position.copy(originalCameraPosition)
  controls.target.copy(originalCameraTarget)
  camera.zoom = originalCameraZoom

  // 恢复相机的 up 向量为默认值 (0, 1, 0)
  camera.up.set(0, 1, 0)

  // 恢复OrbitControls的默认设置
  controls.minPolarAngle = 0
  controls.maxPolarAngle = Math.PI
  controls.enableRotate = true
  controls.enablePan = true

  // 恢复默认鼠标按键配置：左键旋转，中键缩放，右键平移
  controls.mouseButtons = {
    LEFT: THREE.MOUSE.ROTATE,
    MIDDLE: THREE.MOUSE.DOLLY,
    RIGHT: THREE.MOUSE.PAN
  }

  // 更新控制器
  controls.update()

  // 清除顶部视角模式标志
  isTopViewMode.value = false

  console.log('退出顶部视角模式')
  console.log('相机位置已恢复:', camera.position)
}

// 切换XYZ辅助坐标轴的显示/隐藏
const toggleAxisHelper = () => {
  showAxisHelper.value = !showAxisHelper.value
  
  // 设置XYZ轴的可见性
  xAxis.visible = showAxisHelper.value
  xArrow.visible = showAxisHelper.value
  xLabelSprite.visible = showAxisHelper.value
  
  yAxis.visible = showAxisHelper.value
  yArrow.visible = showAxisHelper.value
  yLabelSprite.visible = showAxisHelper.value
  
  zAxis.visible = showAxisHelper.value
  zArrow.visible = showAxisHelper.value
  zLabelSprite.visible = showAxisHelper.value
  
  console.log(showAxisHelper.value ? '显示XYZ辅助坐标轴' : '隐藏XYZ辅助坐标轴')
}

// 落地单个对象 （确保Y轴为0）
const groundObject = (obj: THREE.Object3D) => {
  if (obj instanceof THREE.Mesh) {
    const box = new THREE.Box3().setFromObject(obj)
    const minY = box.min.y
    const currentY = obj.position.y
    
    if (minY !== 0) {
      const yOffset = -minY
      obj.position.y += yOffset
      console.log(`${obj.name || '未命名物体'}: Y位置从 ${currentY.toFixed(2)} 调整到 ${obj.position.y.toFixed(2)}`)
    }
  }
}

const handleGroundObjects = () => {
  console.log('开始落地操作...')
  
  let groundedCount = 0
  
  objects.forEach((obj) => {
    if (obj instanceof THREE.Mesh) {
      const box = new THREE.Box3().setFromObject(obj)
      const minY = box.min.y
      const currentY = obj.position.y
      
      if (minY < 0) {
        const yOffset = -minY
        obj.position.y += yOffset
        console.log(`${obj.name || '未命名物体'}: Y位置从 ${currentY.toFixed(2)} 调整到 ${obj.position.y.toFixed(2)}`)
        groundedCount++
      } else if (minY > 0) {
        const yOffset = -minY
        obj.position.y += yOffset
        console.log(`${obj.name || '未命名物体'}: Y位置从 ${currentY.toFixed(2)} 调整到 ${obj.position.y.toFixed(2)}`)
        groundedCount++
      }
    }
  })
  
  console.log(`落地操作完成，共调整了 ${groundedCount} 个物体的位置`)
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

.toolbar-toggle-btn {
  position: absolute;
  top: 10px;
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
}

.toolbar-toggle-btn:hover {
  background-color: #455A64;
}

.toolbar {
  position: fixed;
  top: 10px;
  left: 10px;
  background-color: rgba(255, 255, 255, 0.8);
  padding: 10px;
  border-radius: 5px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
  z-index: 1000;
  display: flex;
  flex-direction: column;
  gap: 10px;
  max-width: 220px;
  max-height: calc(100vh - 20px);
  overflow-y: auto;
  transition: all 0.3s ease;
}

.toolbar-header {
  display: flex;
  align-items: center;
  gap: 10px;
}

.toolbar h3 {
  margin: 0;
  font-size: 16px;
  white-space: nowrap;
}

.toolbar-content {
  display: flex;
  flex-direction: column;
  gap: 10px;
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

.toolbar button.dxf-upload-btn {
  background-color: #667eea;
}

.toolbar button:hover {
  background-color: #45a049;
}

.exit-edit-btn {
  background-color: #f44336 !important;
  margin-top: 10px;
}

.exit-edit-btn:hover {
  background-color: #d32f2f !important;
}

.top-view-btn {
  background-color: #9C27B0 !important;
  margin-top: 10px;
}

.top-view-btn:hover {
  background-color: #7B1FA2 !important;
}

.exit-top-view-btn {
  background-color: #FF9800 !important;
  margin-top: 10px;
}

.exit-top-view-btn:hover {
  background-color: #F57C00 !important;
}

.axis-toggle-btn {
  background-color: #607D8B !important;
  margin-top: 10px;
}

.axis-toggle-btn:hover {
  background-color: #455A64 !important;
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

.dxf-scale-control {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-top: 10px;
  padding: 8px;
  background-color: rgba(255, 255, 255, 0.5);
  border-radius: 4px;
}

.scale-label {
  font-size: 14px;
  font-weight: bold;
  color: #333;
  white-space: nowrap;
}

.scale-input {
  width: 100%;
  padding: 6px 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  background-color: white;
  font-size: 14px;
  box-sizing: border-box;
}

.scale-apply-btn {
  padding: 6px 12px;
  background-color: #4CAF50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.3s;
}

.scale-apply-btn:hover {
  background-color: #45a049;
}

.dxf-flip-control {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-top: 10px;
  padding: 8px;
  background-color: rgba(255, 255, 255, 0.5);
  border-radius: 4px;
}

.flip-label {
  font-size: 14px;
  font-weight: bold;
  color: #333;
  white-space: nowrap;
}

.flip-inputs {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.flip-input-group {
  display: flex;
  align-items: center;
  gap: 4px;
}

.flip-input-group .axis-label {
  font-size: 12px;
  font-weight: bold;
  color: #555;
}

.flip-input {
  width: 100%;
  padding: 6px 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
  background-color: white;
  font-size: 14px;
  box-sizing: border-box;
}

.flip-apply-btn {
  padding: 6px 12px;
  background-color: #2196F3;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.3s;
}

.flip-apply-btn:hover {
  background-color: #0b7dda;
}

.scene-color-control {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-top: 10px;
  padding: 8px;
  background-color: rgba(255, 255, 255, 0.5);
  border-radius: 4px;
}

.scene-color-control .color-label {
  font-size: 12px;
  font-weight: bold;
  color: #555;
  min-width: 40px;
}

.coordinate-transform-control {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-top: 10px;
  padding: 8px;
  background-color: rgba(255, 255, 255, 0.5);
  border-radius: 4px;
}

.transform-label {
  font-size: 14px;
  font-weight: bold;
  color: #333;
  white-space: nowrap;
}

.transform-checkboxes {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.transform-checkbox {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  color: #333;
  cursor: pointer;
}

.transform-checkbox input[type="checkbox"] {
  cursor: pointer;
}

.transform-buttons {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.transform-apply-btn,
.transform-reset-btn,
.transform-info-btn,
.transform-verify-btn {
  padding: 6px 12px;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
  transition: background-color 0.3s;
}

.transform-apply-btn {
  background-color: #4CAF50;
}

.transform-apply-btn:hover {
  background-color: #45a049;
}

.transform-reset-btn {
  background-color: #FF9800;
}

.transform-reset-btn:hover {
  background-color: #F57C00;
}

.transform-info-btn {
  background-color: #2196F3;
}

.transform-info-btn:hover {
  background-color: #0b7dda;
}

.transform-verify-btn {
  background-color: #9C27B0;
}

.transform-verify-btn:hover {
  background-color: #7B1FA2;
}

.color-control-row {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 5px;
}

.color-input {
  width: 50px;
  height: 30px;
  border: 1px solid #ccc;
  border-radius: 3px;
  cursor: pointer;
}

.color-value {
  font-size: 12px;
  color: #666;
  font-family: monospace;
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

.property-input-name {
  flex: 1;
  padding: 3px 6px;
  border: 1px solid #ddd;
  border-radius: 3px;
  font-size: 13px;
}

.property-input-name:focus {
  outline: none;
  border-color: #2196F3;
  box-shadow: 0 0 3px rgba(33, 150, 243, 0.3);
}

.property-color-input {
  width: 50px;
  height: 30px;
  border: 1px solid #ddd;
  border-radius: 3px;
  cursor: pointer;
  padding: 0;
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