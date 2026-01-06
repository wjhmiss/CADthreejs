import * as THREE from 'three'

export interface NameValidationResult {
  isValid: boolean
  errorMessage?: string
}

export class NameValidator {
  private static instance: NameValidator
  private usedNames: Set<string> = new Set()
  private objects: THREE.Object3D[] = []

  private constructor() {}

  static getInstance(): NameValidator {
    if (!NameValidator.instance) {
      NameValidator.instance = new NameValidator()
    }
    return NameValidator.instance
  }

  registerObject(object: THREE.Object3D): void {
    if (object.name) {
      this.usedNames.add(object.name)
      this.objects.push(object)
    }
  }

  unregisterObject(object: THREE.Object3D): void {
    if (object.name) {
      this.usedNames.delete(object.name)
      const index = this.objects.indexOf(object)
      if (index > -1) {
        this.objects.splice(index, 1)
      }
    }
  }

  updateObjectName(object: THREE.Object3D, oldName: string, newName: string): NameValidationResult {
    if (!newName || newName.trim() === '') {
      return {
        isValid: false,
        errorMessage: '名称不能为空'
      }
    }

    const trimmedName = newName.trim()

    if (trimmedName === oldName) {
      return {
        isValid: true
      }
    }

    if (this.usedNames.has(trimmedName)) {
      return {
        isValid: false,
        errorMessage: `名称 "${trimmedName}" 已存在，请使用其他名称`
      }
    }

    this.usedNames.delete(oldName)
    this.usedNames.add(trimmedName)
    object.name = trimmedName

    return {
      isValid: true
    }
  }

  validateName(name: string, excludeObject?: THREE.Object3D): NameValidationResult {
    if (!name || name.trim() === '') {
      return {
        isValid: false,
        errorMessage: '名称不能为空'
      }
    }

    const trimmedName = name.trim()

    if (excludeObject && excludeObject.name === trimmedName) {
      return {
        isValid: true
      }
    }

    if (this.usedNames.has(trimmedName)) {
      return {
        isValid: false,
        errorMessage: `名称 "${trimmedName}" 已存在，请使用其他名称`
      }
    }

    return {
      isValid: true
    }
  }

  generateUniqueName(baseName: string): string {
    let counter = 1
    let uniqueName = baseName

    while (this.usedNames.has(uniqueName)) {
      uniqueName = `${baseName}_${counter}`
      counter++
    }

    return uniqueName
  }

  isNameUsed(name: string): boolean {
    return this.usedNames.has(name)
  }

  getAllNames(): string[] {
    return Array.from(this.usedNames)
  }

  clear(): void {
    this.usedNames.clear()
    this.objects = []
  }
}

export const nameValidator = NameValidator.getInstance()
