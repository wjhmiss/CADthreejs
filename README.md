# CAD Three.js 项目

基于 Three.js 和 Vue 3 的 CAD 文件查看和 3D 场景编辑器项目，支持 DXF/DWG 文件导入、3D 模型加载、路径规划等功能。

## 项目结构

### 前端项目

前端项目基于 Vue 3 + TypeScript + Vite 构建，使用 Three.js 进行 3D 渲染。

#### 主要文件夹说明

- **src/**: 源代码目录
  - **App.vue**: 主应用组件，包含场景管理、对象操作、导入导出等核心功能
  - **main.ts**: 应用入口文件
  - **style.css**: 全局样式文件
  - **vite-env.d.ts**: Vite 类型定义文件

- **src/Renders/**: CAD 实体渲染器
  - 包含各种 CAD 实体类型的 Three.js 渲染实现
  - 支持的实体类型：Arc、Circle、Dimension、Ellipse、Hatch、Line、Polyline、Spline、Text 等
  - **RenderManager.ts**: 渲染管理器，负责协调所有渲染器

- **src/Utility/**: 工具类和辅助功能
  - **path.ts**: 路径生成和管理核心功能
  - **pathPanel.ts**: 路径管理面板 UI 组件
  - **nameValidator.ts**: 对象名称验证器
  - **objectMovement.ts**: 对象移动控制
  - **outlineGlow.ts**: 轮廓发光效果
  - **breathingLight.ts**: 呼吸灯效果
  - **shaderPath/**: 着色器路径相关代码
    - **src/**: 着色器核心实现（PathGeometry.js、PathPoint.js、PathTubeGeometry.js）
    - **examples/**: 示例代码和演示

- **src/services/**: 服务层
  - **api.ts**: API 接口封装，与后端通信

- **src/sample/**: 示例文件
  - 包含 WebGL 后处理效果示例

- **public/**: 静态资源目录
  - **draco/**: Draco 压缩库，用于 GLB/GLTF 模型压缩
  - **images/**: 图片资源
  - **cangku.glb**: 示例 3D 模型文件

#### 前端技术栈

- **Vue 3**: 渐进式 JavaScript 框架
- **TypeScript**: JavaScript 的超集，提供类型安全
- **Vite**: 下一代前端构建工具
- **Three.js**: 3D 图形库
- **three-stdlib**: Three.js 标准库扩展
- **GSAP**: 高性能动画库
- **Axios**: HTTP 客户端

### 后端项目

后端项目基于 .NET 6.0 和 ASP.NET Core 构建，提供 CAD 文件解析 API 服务。

#### 主要文件夹说明

- **backend/ACadSharp/**: ACadSharp 核心库
  - **Attributes/**: CAD 属性定义
  - **Blocks/**: 图块相关类
  - **Classes/**: DXF 类定义
  - **Entities/**: CAD 实体类型定义
    - 包含所有 CAD 实体类型的实现（Arc、Circle、Line、Polyline、Spline、Text 等）
  - **Header/**: CAD 文件头信息
  - **IO/**: 文件读写操作
    - **DWG/**: DWG 文件读取器
  - **Extensions/**: 扩展方法
  - **Exceptions/**: 异常定义

- **backend/DxfDwgViewer/**: CAD 文件查看器应用
  - **Program.cs**: 主程序入口，配置 Web API 服务器
  - **CadDocumentLoader.cs**: CAD 文档加载器
  - **JsonDataGenerator.cs**: JSON 数据生成器
  - **RenderUtilities/**: 渲染工具类
    - 包含各种实体类型的渲染器实现
  - **Drawing1.dwg**: 示例 DWG 文件
  - **Drawing2.dwg**: 示例 DWG 文件
  - **Drawingall.dwg**: 示例 DWG 文件

#### 后端技术栈

- **.NET 6.0**: 跨平台开发框架
- **ASP.NET Core**: Web 框架
- **ACadSharp**: CAD 文件读写库（支持 DXF/DWG）
- **Newtonsoft.Json**: JSON 序列化库
- **Swashbuckle.AspNetCore**: Swagger API 文档生成器

## 启动设置

### 前端启动

1. **安装依赖**
   ```bash
   npm install
   ```

2. **启动开发服务器**
   ```bash
   npm run dev
   ```
   - 默认端口：5273
   - 访问地址：http://localhost:5273

3. **其他可用命令**
   ```bash
   # 构建生产版本
   npm run build

   # 类型检查
   npm run build:check

   # 预览生产构建
   npm run preview

   # 运行测试
   npm run test
   ```

#### 前端配置

- **vite.config.ts**: Vite 配置文件
  - 开发服务器端口：5273
  - API 代理配置：将 `/api` 请求代理到 `http://localhost:5001`
  - 自定义中间件：
    - `/api/save-glb`: 保存 GLB 文件到 public 文件夹
    - `/api/list-glbs`: 列出 public 文件夹中的 GLB 文件

### 后端启动

1. **使用 Visual Studio 启动**
   - 打开 `backend/ACadSharp.sln` 解决方案
   - 设置 `DxfDwgViewer` 为启动项目
   - 按 F5 或点击"启动"按钮

2. **使用命令行启动**
   ```bash
   cd backend/DxfDwgViewer
   dotnet run
   ```
   - 默认端口：5001
   - 访问地址：http://localhost:5001

3. **其他可用命令**
   ```bash
   # 构建项目
   dotnet build

   # 发布项目
   dotnet publish -c Release
   ```

#### 后端配置

- **Program.cs**: 主程序配置
  - CORS 配置：允许所有来源、方法和头部
  - Swagger 配置：提供 API 文档界面
  - 监听端口：5001

#### 后端 API 端点

- `GET /`: 服务状态检查
- `POST /api/parse`: 解析指定路径的 CAD 文件
- `POST /api/parse/upload`: 上传并解析 CAD 文件
- `GET /api/parse/drawing1`: 解析内置的 Drawing1.dwg 文件
- `GET /swagger`: Swagger API 文档界面

## 功能特性

### 前端功能

1. **3D 场景编辑**
   - 添加基础几何体（方块、球体、圆柱、环）
   - 导入 GLB/GLTF 3D 模型
   - 对象选择、移动、旋转、缩放
   - 场景导入/导出

2. **路径规划**
   - 对象路径生成
   - 路径可视化
   - 路径管理面板

3. **CAD 文件导入**
   - 支持 DXF/DWG 文件上传
   - 自动解析并渲染 CAD 实体
   - 实体类型识别和渲染

4. **视觉效果**
   - 轮廓发光效果
   - 呼吸灯效果
   - 阴影渲染

### 后端功能

1. **CAD 文件解析**
   - 支持 DXF 文件读取
   - 支持 DWG 文件读取
   - 实体提取和数据转换

2. **JSON 数据导出**
   - 将 CAD 实体转换为 JSON 格式
   - 支持多种实体类型

3. **RESTful API**
   - 文件上传接口
   - 文件解析接口
   - Swagger 文档支持

## 开发说明

### 前端开发

- 使用 TypeScript 进行类型安全开发
- 遵循 Vue 3 Composition API 规范
- 使用 Three.js 进行 3D 渲染
- 通过 Axios 与后端 API 通信

### 后端开发

- 使用 .NET 6.0 和 C# 开发
- 遵循 ASP.NET Core Web API 规范
- 使用 ACadSharp 库处理 CAD 文件
- 通过 Swagger 提供 API 文档

## 注意事项

1. **端口配置**
   - 前端默认端口：5273
   - 后端默认端口：5001
   - 确保端口未被占用

2. **依赖关系**
   - 前端依赖后端 API 服务
   - 启动前端前需确保后端服务已启动

3. **文件路径**
   - GLB 模型文件存放在 `public/` 目录
   - CAD 示例文件存放在 `backend/DxfDwgViewer/` 目录

4. **跨域配置**
   - 后端已配置 CORS 允许所有来源
   - 前端通过 Vite 代理访问后端 API

## 许可证

MIT License
