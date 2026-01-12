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

export interface OutlineGlowState {
	originalEdgeGlow: number
	config: OutlineGlowConfig
}

const DEFAULT_CONFIG: OutlineGlowConfig = {
	enabled: false,
	visibleEdgeColor: '#00ff00',
	hiddenEdgeColor: '#000000',
	edgeStrength: 4.8,
	edgeGlow: 1.0,
	edgeThickness: 3.4,
	pulsePeriod: 2.0,
	frequency: 0,
	gradient: false
}

class OutlineGlowManager {
	private outlinePassMap: Map<string, OutlinePass> = new Map()
	private stateMap: Map<string, OutlineGlowState> = new Map()
	private composer: EffectComposer | null = null
	private scene: THREE.Scene | null = null
	private camera: THREE.Camera | null = null
	private animationFrameId: number | null = null
	private time: number = 0
	private lastUpdateTime: number = 0

	setComposer(composer: EffectComposer): void {
		this.composer = composer
	}

	setScene(scene: THREE.Scene): void {
		this.scene = scene
	}

	setCamera(camera: THREE.Camera): void {
		this.camera = camera
	}

	private createOutlinePass(object: THREE.Object3D, config: OutlineGlowConfig): OutlinePass | null {
		if (!this.composer || !this.scene || !this.camera) {
			console.error('Cannot create OutlinePass: composer, scene, or camera not set')
			return null
		}

		const outlinePass = new OutlinePass(
			new THREE.Vector2(window.innerWidth, window.innerHeight),
			this.scene,
			this.camera
		)

		outlinePass.edgeStrength = config.edgeStrength
		outlinePass.edgeGlow = config.edgeGlow
		outlinePass.edgeThickness = config.edgeThickness
		outlinePass.pulsePeriod = config.pulsePeriod
		outlinePass.visibleEdgeColor.set(config.visibleEdgeColor)
		outlinePass.hiddenEdgeColor.set(config.hiddenEdgeColor)
		outlinePass.selectedObjects = [object]
		outlinePass.enabled = config.enabled

		this.composer.addPass(outlinePass)
		return outlinePass
	}

	startOutlineGlow(
		object: THREE.Object3D,
		config: Partial<OutlineGlowConfig> = {}
	): void {
		const objectId = object.uuid
		const fullConfig = { ...DEFAULT_CONFIG, ...config }

		if (!this.stateMap.has(objectId)) {
			const state: OutlineGlowState = {
				originalEdgeGlow: fullConfig.edgeGlow,
				config: fullConfig
			}

			const outlinePass = this.createOutlinePass(object, fullConfig)
			if (outlinePass) {
				this.outlinePassMap.set(objectId, outlinePass)
				this.stateMap.set(objectId, state)
			}
		} else {
			this.updateOutlineGlowConfig(object, config)
		}

		if (this.hasAnyEnabledOutlineGlow() && !this.animationFrameId) {
			this.startAnimation()
		}
	}

	stopOutlineGlow(object: THREE.Object3D): void {
		const objectId = object.uuid
		const outlinePass = this.outlinePassMap.get(objectId)

		if (outlinePass && this.composer) {
			this.composer.removePass(outlinePass)
			outlinePass.dispose()
		}

		this.outlinePassMap.delete(objectId)
		this.stateMap.delete(objectId)

		if (!this.hasAnyEnabledOutlineGlow() && this.animationFrameId) {
			this.stopAnimation()
		}
	}

	updateOutlineGlowConfig(
		object: THREE.Object3D,
		config: Partial<OutlineGlowConfig>
	): void {
		const objectId = object.uuid
		const state = this.stateMap.get(objectId)
		const outlinePass = this.outlinePassMap.get(objectId)

		if (state && outlinePass) {
			state.config = { ...state.config, ...config }

			if (config.visibleEdgeColor !== undefined) {
				outlinePass.visibleEdgeColor.set(config.visibleEdgeColor)
			}
			if (config.hiddenEdgeColor !== undefined) {
				outlinePass.hiddenEdgeColor.set(config.hiddenEdgeColor)
			}
			if (config.edgeStrength !== undefined) {
				outlinePass.edgeStrength = config.edgeStrength
			}
			if (config.edgeGlow !== undefined) {
				state.originalEdgeGlow = config.edgeGlow
				outlinePass.edgeGlow = config.edgeGlow
			}
			if (config.edgeThickness !== undefined) {
				outlinePass.edgeThickness = config.edgeThickness
			}
			if (config.pulsePeriod !== undefined) {
				outlinePass.pulsePeriod = config.pulsePeriod
			}
			if (config.enabled !== undefined) {
				outlinePass.enabled = config.enabled
			}
			if (config.gradient !== undefined) {
				outlinePass.usePatternTexture = config.gradient
			}

			if (config.frequency !== undefined && config.frequency > 0 && config.enabled && !this.animationFrameId) {
				this.startAnimation()
			}
		}
	}

	setOutlineGlowEnabled(object: THREE.Object3D, enabled: boolean): void {
		const objectId = object.uuid
		const state = this.stateMap.get(objectId)
		const outlinePass = this.outlinePassMap.get(objectId)

		if (state && outlinePass) {
			state.config.enabled = enabled
			outlinePass.enabled = enabled

			if (enabled && state.config.frequency > 0 && !this.animationFrameId && this.hasAnyEnabledOutlineGlow()) {
				this.startAnimation()
			} else if (!enabled && !this.hasAnyEnabledOutlineGlow()) {
				this.stopAnimation()
			}
		}
	}

	getOutlineGlowConfig(object: THREE.Object3D): OutlineGlowConfig | null {
		const objectId = object.uuid
		const state = this.stateMap.get(objectId)
		return state ? { ...state.config } : null
	}

	getAllOutlineGlowObjects(): THREE.Object3D[] {
		const objects: THREE.Object3D[] = []
		for (const objectId of this.outlinePassMap.keys()) {
			const object = this.findObjectById(objectId)
			if (object) {
				objects.push(object)
			}
		}
		return objects
	}

	private hasAnyEnabledOutlineGlow(): boolean {
		for (const state of this.stateMap.values()) {
			if (state.config.enabled && state.config.frequency > 0) {
				return true
			}
		}
		return false
	}

	private startAnimation(): void {
		const animate = (currentTime: number) => {
			const deltaTime = (currentTime - this.lastUpdateTime) / 1000
			this.lastUpdateTime = currentTime

			if (deltaTime > 0) {
				this.time += deltaTime
				this.updateGlowEffects()
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

	private updateGlowEffects(): void {
		for (const [objectId, state] of this.stateMap) {
			if (!state.config.enabled || state.config.frequency === 0) continue

			const outlinePass = this.outlinePassMap.get(objectId)
			if (outlinePass) {
				const phase = Math.sin(this.time * state.config.frequency * Math.PI * 2)
				const glowVariation = (phase + 1) / 2 * state.config.edgeGlow
				outlinePass.edgeGlow = glowVariation
			}
		}
	}

	private findObjectById(objectId: string): THREE.Object3D | null {
		if (!this.scene) return null
		
		let foundObject: THREE.Object3D | null = null
		
		this.scene.traverse((child) => {
			if (child.uuid === objectId) {
				foundObject = child
				return true
			}
			return false
		})
		
		return foundObject
	}

	dispose(): void {
		this.stopAnimation()
		
		for (const [objectId, outlinePass] of this.outlinePassMap) {
			if (this.composer) {
				this.composer.removePass(outlinePass)
			}
			outlinePass.dispose()
		}

		this.outlinePassMap.clear()
		this.stateMap.clear()
	}
}

const outlineGlowManager = new OutlineGlowManager()

export { outlineGlowManager, DEFAULT_CONFIG }
export type { OutlineGlowState }
