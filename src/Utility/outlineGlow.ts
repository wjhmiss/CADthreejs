import * as THREE from 'three'
import { EffectComposer } from 'three/examples/jsm/postprocessing/EffectComposer'
import { OutlinePass } from 'three/examples/jsm/postprocessing/OutlinePass'

export interface OutlineGlowConfig {
	enabled: boolean
	visibleEdgeColor: string
	hiddenEdgeColor: string
	edgeStrength: number
	edgeGlow: number
	edgeThickness: number
	pulsePeriod: number
	frequency: number
	gradient: boolean
}

const DEFAULT_CONFIG: OutlineGlowConfig = {
	enabled: false,
	visibleEdgeColor: '#ffffff',
	hiddenEdgeColor: '#190a05',
	edgeStrength: 3.0,
	edgeGlow: 0.0,
	edgeThickness: 1.0,
	pulsePeriod: 0,
	frequency: 1.0,
	gradient: false
}

class OutlineGlowManager {
	private outlinePass: OutlinePass | null = null
	private composer: EffectComposer | null = null
	private scene: THREE.Scene | null = null
	private camera: THREE.Camera | null = null
	private selectedObjects: THREE.Object3D[] = []
	private animationFrameId: number | null = null
	private time: number = 0
	private lastUpdateTime: number = 0
	private config: OutlineGlowConfig = { ...DEFAULT_CONFIG }
	private originalEdgeGlow: number = 0

	setComposer(composer: EffectComposer): void {
		this.composer = composer
	}

	setScene(scene: THREE.Scene): void {
		this.scene = scene
	}

	setCamera(camera: THREE.Camera): void {
		this.camera = camera
	}

	initOutlinePass(): void {
		if (!this.composer || !this.scene || !this.camera) {
			console.error('Cannot init OutlinePass: composer, scene, or camera not set')
			return
		}

		if (this.outlinePass) {
			console.warn('OutlinePass already initialized')
			return
		}

		this.outlinePass = new OutlinePass(
			new THREE.Vector2(window.innerWidth, window.innerHeight),
			this.scene,
			this.camera
		)

		this.outlinePass.edgeStrength = this.config.edgeStrength
		this.outlinePass.edgeGlow = this.config.edgeGlow
		this.outlinePass.edgeThickness = this.config.edgeThickness
		this.outlinePass.pulsePeriod = this.config.pulsePeriod
		this.outlinePass.visibleEdgeColor.set(this.config.visibleEdgeColor)
		this.outlinePass.hiddenEdgeColor.set(this.config.hiddenEdgeColor)
		this.outlinePass.selectedObjects = this.selectedObjects

		this.composer.addPass(this.outlinePass)
	}

	setSelectedObjects(objects: THREE.Object3D[]): void {
		this.selectedObjects = objects
		if (this.outlinePass) {
			this.outlinePass.selectedObjects = objects
		}
	}

	addSelectedObject(object: THREE.Object3D): void {
		if (!this.selectedObjects.includes(object)) {
			this.selectedObjects.push(object)
			if (this.outlinePass) {
				this.outlinePass.selectedObjects = this.selectedObjects
			}
		}
	}

	removeSelectedObject(object: THREE.Object3D): void {
		const index = this.selectedObjects.indexOf(object)
		if (index > -1) {
			this.selectedObjects.splice(index, 1)
			if (this.outlinePass) {
				this.outlinePass.selectedObjects = this.selectedObjects
			}
		}
	}

	clearSelectedObjects(): void {
		this.selectedObjects = []
		if (this.outlinePass) {
			this.outlinePass.selectedObjects = []
		}
	}

	setEnabled(enabled: boolean): void {
		this.config.enabled = enabled
		if (this.outlinePass) {
			this.outlinePass.enabled = enabled
		}

		if (enabled && this.config.frequency > 0 && !this.animationFrameId) {
			this.startAnimation()
		} else if (!enabled || this.config.frequency === 0) {
			this.stopAnimation()
		}
	}

	setVisibleEdgeColor(color: string): void {
		this.config.visibleEdgeColor = color
		if (this.outlinePass) {
			this.outlinePass.visibleEdgeColor.set(color)
		}
	}

	setHiddenEdgeColor(color: string): void {
		this.config.hiddenEdgeColor = color
		if (this.outlinePass) {
			this.outlinePass.hiddenEdgeColor.set(color)
		}
	}

	setEdgeStrength(strength: number): void {
		this.config.edgeStrength = strength
		if (this.outlinePass) {
			this.outlinePass.edgeStrength = strength
		}
	}

	setEdgeGlow(glow: number): void {
		this.config.edgeGlow = glow
		this.originalEdgeGlow = glow
		if (this.outlinePass) {
			this.outlinePass.edgeGlow = glow
		}
	}

	setEdgeThickness(thickness: number): void {
		this.config.edgeThickness = thickness
		if (this.outlinePass) {
			this.outlinePass.edgeThickness = thickness
		}
	}

	setPulsePeriod(period: number): void {
		this.config.pulsePeriod = period
		if (this.outlinePass) {
			this.outlinePass.pulsePeriod = period
		}
	}

	setFrequency(frequency: number): void {
		this.config.frequency = frequency

		if (frequency > 0 && this.config.enabled && !this.animationFrameId) {
			this.startAnimation()
		} else if (frequency === 0) {
			this.stopAnimation()
			if (this.outlinePass) {
				this.outlinePass.edgeGlow = this.originalEdgeGlow
			}
		}
	}

	setGradient(enabled: boolean): void {
		this.config.gradient = enabled
		if (this.outlinePass) {
			this.outlinePass.usePatternTexture = enabled
		}
	}

	setConfig(config: Partial<OutlineGlowConfig>): void {
		if (config.enabled !== undefined) this.setEnabled(config.enabled)
		if (config.visibleEdgeColor !== undefined) this.setVisibleEdgeColor(config.visibleEdgeColor)
		if (config.hiddenEdgeColor !== undefined) this.setHiddenEdgeColor(config.hiddenEdgeColor)
		if (config.edgeStrength !== undefined) this.setEdgeStrength(config.edgeStrength)
		if (config.edgeGlow !== undefined) this.setEdgeGlow(config.edgeGlow)
		if (config.edgeThickness !== undefined) this.setEdgeThickness(config.edgeThickness)
		if (config.pulsePeriod !== undefined) this.setPulsePeriod(config.pulsePeriod)
		if (config.frequency !== undefined) this.setFrequency(config.frequency)
		if (config.gradient !== undefined) this.setGradient(config.gradient)
	}

	getConfig(): OutlineGlowConfig {
		return { ...this.config }
	}

	getSelectedObjects(): THREE.Object3D[] {
		return [...this.selectedObjects]
	}

	private startAnimation(): void {
		const animate = (currentTime: number) => {
			const deltaTime = (currentTime - this.lastUpdateTime) / 1000
			this.lastUpdateTime = currentTime

			if (deltaTime > 0) {
				this.time += deltaTime
				this.updateGlowEffect()
			}

			this.animationFrameId = requestAnimationFrame(animate)
		}

		this.animationFrameId = requestAnimationFrame(animate)
	}

	private stopAnimation(): void {
		if (this.animationFrameId) {
			cancelAnimationFrame(this.animationFrameId)
			this.animationFrameId = null
		}
		this.time = 0
	}

	private updateGlowEffect(): void {
		if (!this.outlinePass || this.config.frequency === 0) return

		const phase = Math.sin(this.time * this.config.frequency * Math.PI * 2)
		const glowVariation = (phase + 1) / 2 * this.config.edgeGlow
		this.outlinePass.edgeGlow = glowVariation
	}

	dispose(): void {
		this.stopAnimation()
		this.clearSelectedObjects()
		if (this.outlinePass && this.composer) {
			this.composer.removePass(this.outlinePass)
			this.outlinePass.dispose()
			this.outlinePass = null
		}
	}
}

const outlineGlowManager = new OutlineGlowManager()

export { outlineGlowManager, DEFAULT_CONFIG }
export type { OutlineGlowConfig }
