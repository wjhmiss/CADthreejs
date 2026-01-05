import axios from 'axios';
import type { 
  CadFile, 
  CadParseResult, 
  CadEntity, 
  ApiResponse, 
  UploadRequest 
} from '../../shared/types';

// 创建axios实例
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5001',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// 请求拦截器
api.interceptors.request.use(
  (config) => {
    // 可以在这里添加认证token等
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// 响应拦截器
api.interceptors.response.use(
  (response) => {
    return response.data;
  },
  (error) => {
    console.error('API Error:', error);
    return Promise.reject(error);
  }
);

// 文件API
export const fileApi = {
  // 上传文件
  async uploadFile(request: UploadRequest): Promise<any> {
    const formData = new FormData();
    formData.append('file', request.file);
    
    return api.post('/api/parse/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
      onUploadProgress: (progressEvent) => {
        if (request.onProgress && progressEvent.total) {
          const progress = Math.round((progressEvent.loaded * 100) / progressEvent.total);
          request.onProgress(progress);
        }
      },
    });
  },

  // 获取文件列表
  async getFiles(): Promise<ApiResponse<CadFile[]>> {
    return api.get('/api/files');
  },

  // 解析CAD文件
  async parseFile(fileId: string): Promise<ApiResponse<CadParseResult>> {
    return api.get(`/api/files/${fileId}/parse`);
  },

  // 获取文件实体
  async getFileEntities(fileId: string): Promise<ApiResponse<CadEntity[]>> {
    return api.get(`/api/files/${fileId}/entities`);
  },

  // 删除文件
  async deleteFile(fileId: string): Promise<ApiResponse<null>> {
    return api.delete(`/api/files/${fileId}`);
  },
};

// 导出默认实例
export default api;