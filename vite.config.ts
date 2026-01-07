import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// 使用动态导入来处理JavaScript插件
export default defineConfig({
  plugins: [
    vue(),
    {
      name: 'save-glb-plugin',
      configureServer(server) {
        // API端点：保存GLB文件到public文件夹
        server.middlewares.use('/api/save-glb', (req, res, next) => {
          if (req.method !== 'POST') {
            res.statusCode = 405
            res.end('Method Not Allowed')
            return
          }

          let body = Buffer.alloc(0)
          req.on('data', chunk => {
            body = Buffer.concat([body, chunk])
          })

          req.on('end', () => {
            try {
              // 检查请求是否包含multipart/form-data
              const contentType = req.headers['content-type']
              if (!contentType || !contentType.includes('multipart/form-data')) {
                res.statusCode = 400
                res.end(JSON.stringify({
                  success: false,
                  error: 'Content-Type must be multipart/form-data'
                }))
                return
              }
              
              // 解析multipart/form-data
              const boundary = contentType.split('boundary=')[1]
              if (!boundary) {
                res.statusCode = 400
                res.end(JSON.stringify({
                  success: false,
                  error: 'No boundary found in Content-Type'
                }))
                return
              }
              
              // 将Buffer转换为字符串进行解析
              const bodyStr = body.toString('binary')
              const parts = bodyStr.split(`--${boundary}`)
              
              for (const part of parts) {
                if (part.includes('Content-Disposition: form-data') && part.includes('filename=')) {
                  // 提取文件名
                  const filenameMatch = part.match(/filename="([^"]+)"/)
                  if (!filenameMatch) continue
                  
                  let filename = filenameMatch[1]
                  
                  // 找到文件内容的开始位置
                  const headerEnd = part.indexOf('\r\n\r\n')
                  if (headerEnd === -1) continue
                  
                  // 计算文件内容在原始Buffer中的位置
                  const partStart = body.indexOf(Buffer.from(`--${boundary}\r\n`))
                  const partIndex = body.indexOf(Buffer.from(part.substring(0, 50)), partStart)
                  const contentStart = partIndex + headerEnd + 4
                  const contentEnd = partIndex + part.length - 2 // 减去末尾的\r\n
                  
                  // 提取文件内容
                  const fileContent = body.slice(contentStart, contentEnd)
                  
                  // 确保public目录存在
                  const fs = require('fs')
                  const path = require('path')
                  const publicDir = path.resolve(process.cwd(), 'public')
                  if (!fs.existsSync(publicDir)) {
                    fs.mkdirSync(publicDir, { recursive: true })
                  }
                  
                  // 直接使用原始文件名，如果文件已存在则覆盖
                  const filePath = path.join(publicDir, filename)
                  
                  // 写入文件（会覆盖同名文件）
                  fs.writeFileSync(filePath, fileContent)
                  
                  // 保存原始文件名
                  const originalFileName = filename
                  
                  // 返回成功响应
                  res.setHeader('Content-Type', 'application/json')
                  res.end(JSON.stringify({
                    success: true,
                    fileName: originalFileName
                  }))
                  return
                }
              }
              
              // 如果没有找到文件
              res.statusCode = 400
              res.end(JSON.stringify({
                success: false,
                error: 'No file found in request'
              }))
            } catch (error) {
              console.error('Error saving GLB file:', error)
              res.statusCode = 500
              res.end(JSON.stringify({
                success: false,
                error: 'Internal server error: ' + error.message
              }))
            }
          })
        })
        
        // API端点：列出public文件夹中的GLB文件
        server.middlewares.use('/api/list-glbs', (req, res, next) => {
          if (req.method !== 'GET') {
            res.statusCode = 405
            res.end('Method Not Allowed')
            return
          }

          try {
            const fs = require('fs')
            const path = require('path')
            const publicDir = path.resolve(process.cwd(), 'public')
            
            // 确保public目录存在
            if (!fs.existsSync(publicDir)) {
              fs.mkdirSync(publicDir, { recursive: true })
            }
            
            // 读取目录中的文件
            const files = fs.readdirSync(publicDir)
            
            // 过滤出GLB文件
            const glbFiles = files.filter(file => 
              file.toLowerCase().endsWith('.glb') || file.toLowerCase().endsWith('.gltf')
            )
            
            // 返回文件列表
            res.setHeader('Content-Type', 'application/json')
            res.end(JSON.stringify({
              success: true,
              files: glbFiles
            }))
          } catch (error) {
            console.error('Error listing GLB files:', error)
            res.statusCode = 500
            res.end(JSON.stringify({
              success: false,
              error: 'Internal server error'
            }))
          }
        })
      }
    }
  ],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: [],
  },
  server: {
    port: 5273,
    proxy: {
      '/api': {
        target: 'http://localhost:5001',
        changeOrigin: true,
        secure: false,
      }
    }
  },
  preview: {
    port: 5273
  }
})