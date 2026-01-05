import * as THREE from 'three'

export interface BreathingLightConfig {
	enabled: boolean
	color: string
	frequency: number
	intensity: number
}

export interface BreathingLightState {
	originalEmissive: THREE.Color
	originalEmissiveIntensity: number
	config: BreathingLightConfig
}

const DEFAULT_CONFIG: BreathingLightConfig = {
	enabled: false,
	color: '#ff6600',
	frequency: 1.0,
	intensity: 1.0
}

class BreathingLightManager {
	private breathingLightMap: Map<string, BreathingLightState> = new Map()
	private animationFrameId: number | null = null
	private time: number = 0
	private lastUpdateTime: number = 0
	private scene: THREE.Scene | null = null

	setScene(scene: THREE.Scene): void {
		this.scene = scene
	}

	startBreathingLight(
		object: THREE.Object3D,
		config: Partial<BreathingLightConfig> = {}
	): void {
		const objectId = object.uuid
		const fullConfig = { ...DEFAULT_CONFIG, ...config }

		if (!this.breathingLightMap.has(objectId)) {
			const state: BreathingLightState = {
				originalEmissive: new THREE.Color(),
				originalEmissiveIntensity: 0,
				config: fullConfig
			}

			object.traverse((child) => {
				if (child instanceof THREE.Mesh && child.material) {
					const material = child.material as THREE.MeshStandardMaterial
					state.originalEmissive.copy(material.emissive)
					state.originalEmissiveIntensity = material.emissiveIntensity
				}
			})

			this.breathingLightMap.set(objectId, state)
		} else {
			const state = this.breathingLightMap.get(objectId)!
			state.config = { ...state.config, ...config }
		}

		if (this.hasAnyEnabledBreathingLight() && !this.animationFrameId) {
			this.startAnimation()
		}
	}

	stopBreathingLight(object: THREE.Object3D): void {
		const objectId = object.uuid
		const state = this.breathingLightMap.get(objectId)

		if (state) {
			object.traverse((child) => {
				if (child instanceof THREE.Mesh && child.material) {
					const material = child.material as THREE.MeshStandardMaterial
					material.emissive.copy(state.originalEmissive)
					material.emissiveIntensity = state.originalEmissiveIntensity
				}
			})

			this.breathingLightMap.delete(objectId)
		}

		if (!this.hasAnyEnabledBreathingLight() && this.animationFrameId) {
			this.stopAnimation()
		}
	}

	updateBreathingLightConfig(
		object: THREE.Object3D,
		config: Partial<BreathingLightConfig>
	): void {
		const objectId = object.uuid
		const state = this.breathingLightMap.get(objectId)

		if (state) {
			state.config = { ...state.config, ...config }
		}
	}

	setBreathingLightEnabled(object: THREE.Object3D, enabled: boolean): void {
		const objectId = object.uuid
		const state = this.breathingLightMap.get(objectId)

		if (state) {
			state.config.enabled = enabled

			if (enabled && !this.animationFrameId && this.hasAnyEnabledBreathingLight()) {
				this.startAnimation()
			} else if (!enabled && !this.hasAnyEnabledBreathingLight()) {
				this.stopAnimation()
			}
		}
	}

	getBreathingLightConfig(object: THREE.Object3D): BreathingLightConfig | null {
		const objectId = object.uuid
		const state = this.breathingLightMap.get(objectId)
		return state ? { ...state.config } : null
	}

	private hasAnyEnabledBreathingLight(): boolean {
		for (const state of this.breathingLightMap.values()) {
			if (state.config.enabled) {
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
				this.updateBreathingEffects()
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

	private updateBreathingEffects(): void {
		for (const [objectId, state] of this.breathingLightMap) {
			if (!state.config.enabled) continue

			const targetColor = new THREE.Color(state.config.color)
			const phase = Math.sin(this.time * state.config.frequency * Math.PI * 2)
			const intensity = (phase + 1) / 2 * state.config.intensity

			const object = this.findObjectById(objectId)
			if (object) {
				object.traverse((child) => {
					if (child instanceof THREE.Mesh && child.material) {
						const material = child.material as THREE.MeshStandardMaterial
						material.emissive.copy(targetColor)
						material.emissiveIntensity = intensity
					}
				})
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
		this.breathingLightMap.clear()
	}
}

const breathingLightManager = new BreathingLightManager()

export { breathingLightManager, DEFAULT_CONFIG }
export type { BreathingLightState }
