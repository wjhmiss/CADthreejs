<template>
  <div class="dxf-upload-container">
    <div class="header">
      <h1>CAD 文件解析器</h1>
      <button @click="goBack" class="back-btn">返回</button>
    </div>

    <div class="main-content">
      <div v-if="parseResult" class="result-section">
        <div class="result-header">
          <h2>解析结果</h2>
          <div class="result-summary">
            <span class="summary-item">文件名: {{ parseResult.fileName }}</span>
            <span class="summary-item">实体数量: {{ parseResult.entityCount }}</span>
          </div>
        </div>

        <div class="json-container">
          <div class="json-header">
            <h3>JSON 数据</h3>
            <div class="json-actions">
              <button @click="copyJson" class="action-btn">复制</button>
              <button @click="downloadJson" class="action-btn">下载</button>
              <button @click="toggleJson" class="action-btn">
                {{ showJson ? '收起' : '展开' }}
              </button>
            </div>
          </div>
          <div v-if="showJson" class="json-content">
            <pre>{{ formattedJson }}</pre>
          </div>
        </div>

        <div class="entity-stats">
          <h3>实体统计</h3>
          <div class="stats-grid">
            <div 
              v-for="(count, type) in entityCounts" 
              :key="type"
              class="stat-card"
            >
              <div class="stat-type">{{ type }}</div>
              <div class="stat-count">{{ count }}</div>
            </div>
          </div>
        </div>
      </div>

      <div class="upload-section">
        <div class="upload-card">
          <div class="upload-icon">📁</div>
          <h2>上传 DXF/DWG 文件</h2>
          <p class="description">支持 .dxf 和 .dwg 格式的 CAD 文件</p>
          
          <div 
            class="drop-zone" 
            :class="{ 'drag-over': isDragOver, 'uploading': isUploading }"
            @dragover.prevent="onDragOver"
            @dragleave.prevent="onDragLeave"
            @drop.prevent="onDrop"
            @click="triggerFileInput"
          >
            <input 
              type="file" 
              ref="fileInput" 
              @change="handleFileSelect" 
              accept=".dxf,.dwg" 
              style="display: none;"
            />
            <div v-if="!selectedFile" class="drop-content">
              <p>点击或拖拽文件到此处上传</p>
            </div>
            <div v-else class="file-info">
              <p class="file-name">{{ selectedFile.name }}</p>
              <p class="file-size">{{ formatFileSize(selectedFile.size) }}</p>
              <button @click.stop="clearFile" class="clear-btn">清除</button>
            </div>
          </div>

          <button 
            @click="uploadFile" 
            :disabled="!selectedFile || isUploading"
            class="upload-btn"
          >
            {{ isUploading ? '上传中...' : '开始解析' }}
          </button>

          <div v-if="uploadProgress > 0 && uploadProgress < 100" class="progress-bar">
            <div class="progress-fill" :style="{ width: uploadProgress + '%' }"></div>
            <span class="progress-text">{{ uploadProgress }}%</span>
          </div>
        </div>
      </div>
    </div>

    <div v-if="error" class="error-message">
      <div class="error-icon">⚠️</div>
      <p>{{ error }}</p>
      <button @click="error = ''" class="close-btn">关闭</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { fileApi } from './services/api'

interface ParseResult {
  success: boolean
  fileName: string
  entityCount: number
  data: string
}

const emit = defineEmits(['goBack'])

const fileInput = ref<HTMLInputElement | null>(null)
const selectedFile = ref<File | null>(null)
const isUploading = ref(false)
const uploadProgress = ref(0)
const parseResult = ref<ParseResult | null>(null)
const error = ref('')
const isDragOver = ref(false)
const showJson = ref(true)

const formattedJson = computed(() => {
  if (!parseResult.value?.data) return ''
  try {
    const data = JSON.parse(parseResult.value.data)
    return JSON.stringify(data, null, 2)
  } catch {
    return parseResult.value.data
  }
})

const entityCounts = computed(() => {
  if (!parseResult.value?.data) return {}
  try {
    const data = JSON.parse(parseResult.value.data)
    const counts: Record<string, number> = {}
    
    Object.keys(data).forEach(key => {
      if (Array.isArray(data[key]) && data[key].length > 0) {
        const typeName = key.replace('Datas', '')
        counts[typeName] = data[key].length
      }
    })
    
    return counts
  } catch {
    return {}
  }
})

const triggerFileInput = () => {
  fileInput.value?.click()
}

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files && target.files.length > 0) {
    selectedFile.value = target.files[0]
  }
}

const onDragOver = () => {
  isDragOver.value = true
}

const onDragLeave = () => {
  isDragOver.value = false
}

const onDrop = (event: DragEvent) => {
  isDragOver.value = false
  if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
    const file = event.dataTransfer.files[0]
    const extension = file.name.toLowerCase().split('.').pop()
    if (extension === 'dxf' || extension === 'dwg') {
      selectedFile.value = file
    } else {
      error.value = '仅支持 .dxf 和 .dwg 格式的文件'
    }
  }
}

const clearFile = () => {
  selectedFile.value = null
  if (fileInput.value) {
    fileInput.value.value = ''
  }
}

const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
}

const uploadFile = async () => {
  if (!selectedFile.value) return

  isUploading.value = true
  uploadProgress.value = 0
  error.value = ''
  parseResult.value = null

  try {
    const result = await fileApi.uploadFile({
      file: selectedFile.value,
      onProgress: (progress: number) => {
        uploadProgress.value = progress
      }
    })
    
    parseResult.value = result
    uploadProgress.value = 100
  } catch (err) {
    error.value = err instanceof Error ? err.message : '上传失败，请重试'
    console.error('Upload error:', err)
  } finally {
    isUploading.value = false
  }
}

const copyJson = () => {
  if (formattedJson.value) {
    navigator.clipboard.writeText(formattedJson.value)
    alert('已复制到剪贴板')
  }
}

const downloadJson = () => {
  if (formattedJson.value) {
    const blob = new Blob([formattedJson.value], { type: 'application/json' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `${parseResult.value?.fileName || 'result'}.json`
    a.click()
    URL.revokeObjectURL(url)
  }
}

const toggleJson = () => {
  showJson.value = !showJson.value
}

const goBack = () => {
  emit('goBack', parseResult.value?.data)
}
</script>

<style scoped>
.dxf-upload-container {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
  z-index: 1000;
  overflow-y: auto;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  color: white;
}

.header h1 {
  margin: 0;
  font-size: 28px;
  font-weight: 600;
}

.back-btn {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 1px solid rgba(255, 255, 255, 0.3);
  padding: 10px 20px;
  border-radius: 8px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.3s;
}

.back-btn:hover {
  background: rgba(255, 255, 255, 0.3);
}

.main-content {
  display: flex;
  gap: 20px;
  align-items: flex-start;
}

.result-section {
  flex: 1;
  min-width: 0;
}

.result-header {
  background: white;
  border-radius: 8px;
  padding: 12px;
  margin-bottom: 10px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.result-header h2 {
  margin: 0 0 8px 0;
  color: #333;
  font-size: 16px;
}

.result-summary {
  display: flex;
  gap: 20px;
  flex-wrap: wrap;
}

.summary-item {
  background: #f0f4ff;
  color: #667eea;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
}

.json-container {
  background: white;
  border-radius: 8px;
  margin-bottom: 10px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.json-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 15px;
  background: #f8f9fa;
  border-bottom: 1px solid #e9ecef;
}

.json-header h3 {
  margin: 0;
  color: #333;
  font-size: 14px;
}

.json-actions {
  display: flex;
  gap: 10px;
}

.action-btn {
  background: #667eea;
  color: white;
  border: none;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
  transition: background 0.3s;
}

.action-btn:hover {
  background: #5568d3;
}

.json-content {
  padding: 15px;
  max-height: 800px;
  overflow-y: auto;
  background: #f8f9fa;
}

.json-content pre {
  margin: 0;
  font-family: 'Courier New', monospace;
  font-size: 11px;
  line-height: 1.4;
  color: #333;
  white-space: pre-wrap;
  word-wrap: break-word;
}

.entity-stats {
  background: white;
  border-radius: 8px;
  padding: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.entity-stats h3 {
  margin: 0 0 10px 0;
  color: #333;
  font-size: 16px;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  gap: 10px;
}

.stat-card {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 12px;
  border-radius: 6px;
  text-align: center;
}

.stat-type {
  font-size: 11px;
  opacity: 0.9;
  margin-bottom: 5px;
}

.stat-count {
  font-size: 20px;
  font-weight: 700;
}

.upload-section {
  width: 400px;
  flex-shrink: 0;
  position: sticky;
  top: 20px;
}

.upload-card {
  background: white;
  border-radius: 8px;
  padding: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  text-align: center;
}

.upload-icon {
  font-size: 28px;
  margin-bottom: 5px;
}

.upload-card h2 {
  margin: 0 0 3px 0;
  color: #333;
  font-size: 14px;
}

.description {
  color: #666;
  margin-bottom: 10px;
  font-size: 11px;
}

.drop-zone {
  border: 2px dashed #ddd;
  border-radius: 8px;
  padding: 20px 15px;
  margin-bottom: 10px;
  cursor: pointer;
  transition: all 0.3s;
  background: #fafafa;
}

.drop-zone:hover {
  border-color: #667eea;
  background: #f0f4ff;
}

.drop-zone.drag-over {
  border-color: #667eea;
  background: #e8edff;
  transform: scale(1.02);
}

.drop-zone.uploading {
  pointer-events: none;
  opacity: 0.6;
}

.drop-content p {
  margin: 0;
  color: #999;
  font-size: 16px;
}

.file-info {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.file-name {
  margin: 0;
  color: #333;
  font-weight: 600;
  font-size: 13px;
  word-break: break-all;
}

.file-size {
  margin: 0;
  color: #999;
  font-size: 11px;
}

.clear-btn {
  background: #ff4757;
  color: white;
  border: none;
  padding: 5px 10px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 11px;
  margin-top: 5px;
  transition: background 0.3s;
}

.clear-btn:hover {
  background: #ff6b7a;
}

.upload-btn {
  width: 100%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  padding: 10px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 13px;
  font-weight: 600;
  transition: all 0.3s;
}

.upload-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.upload-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.progress-bar {
  position: relative;
  height: 30px;
  background: #f0f0f0;
  border-radius: 15px;
  overflow: hidden;
  margin-top: 20px;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #667eea, #764ba2);
  transition: width 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.progress-text {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: white;
  font-weight: 600;
  font-size: 12px;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
}

.error-message {
  position: fixed;
  top: 20px;
  right: 20px;
  background: #ff4757;
  color: white;
  padding: 20px;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
  display: flex;
  align-items: center;
  gap: 15px;
  max-width: 400px;
  z-index: 1000;
  animation: slideIn 0.3s;
}

@keyframes slideIn {
  from {
    transform: translateX(400px);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

.error-icon {
  font-size: 24px;
}

.error-message p {
  margin: 0;
  flex: 1;
  font-size: 14px;
}

.close-btn {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: none;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
  transition: background 0.3s;
}

.close-btn:hover {
  background: rgba(255, 255, 255, 0.3);
}
</style>
