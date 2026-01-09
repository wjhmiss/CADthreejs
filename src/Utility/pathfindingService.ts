import axios from 'axios'
import * as THREE from 'three'
import { GridFloor } from './gridFloor'

export interface Waypoint {
  x: number
  y: number
}

export interface PathFindingRequest {
  waypoints: Waypoint[]
  obstacles: Waypoint[]
  gridWidth: number
  gridHeight: number
  allowDiagonal?: boolean
}

export interface PathFindingResponse {
  success: boolean
  path: Waypoint[]
  message?: string
  totalCost?: number
  nodesExplored?: number
  executionTimeMs?: number
}

export class PathfindingService {
  private api: any
  private baseUrl: string

  constructor(baseUrl: string = 'http://localhost:5001') {
    this.baseUrl = baseUrl
    this.api = axios.create({
      baseURL: this.baseUrl,
      timeout: 30000,
      headers: {
        'Content-Type': 'application/json',
      },
    })

    this.api.interceptors.response.use(
      (response) => response.data,
      (error) => {
        console.error('[PathfindingService] API Error:', error)
        return Promise.reject(error)
      }
    )
  }

  async findPathWithWaypoints(request: PathFindingRequest): Promise<PathFindingResponse> {
    try {
      console.log('[PathfindingService] ========== 路径规划请求开始 ==========')
      console.log('[PathfindingService] 原始请求参数:', request)
      
      const backendRequest = {
        waypoints: request.waypoints.map(wp => ({ X: wp.x, Y: wp.y })),
        obstacles: request.obstacles.map(ob => ({ X: ob.x, Y: ob.y })),
        gridWidth: request.gridWidth,
        gridHeight: request.gridHeight,
        allowDiagonal: request.allowDiagonal
      }
      
      console.log('[PathfindingService] 发送到后端的请求格式:')
      console.log('[PathfindingService]   坐标系统说明: 后端使用2D X,Y平面，前端使用3D X,Z平面')
      console.log('[PathfindingService]   路径点转换: 前端 waypoint.x -> 后端 X, 前端 waypoint.y -> 后端 Y')
      console.log('[PathfindingService]   障碍物转换: 前端 obstacle.x -> 后端 X, 前端 obstacle.y -> 后端 Y')
      console.log('[PathfindingService]   转换后的请求:', backendRequest)
      
      const response = await this.api.post('/api/pathfinding/find-with-waypoints', backendRequest)
      
      console.log('[PathfindingService] ========== 后端响应 ==========')
      console.log('[PathfindingService] 原始响应数据:', response)
      console.log('[PathfindingService] 响应成功:', response.success)
      console.log('[PathfindingService] 路径数据:', response.path)
      console.log('[PathfindingService] 路径数据类型:', typeof response.path)
      console.log('[PathfindingService] 路径数据长度:', response.path?.length)
      
      if (response.path && response.path.length > 0) {
        console.log('[PathfindingService] 第一个路径点:', response.path[0])
        console.log('[PathfindingService] 路径点示例:', response.path.slice(0, 3))
      }
      
      const pathFindingResponse: PathFindingResponse = {
        success: response.success,
        path: response.path ? response.path.map((p: any) => ({ x: p.x, y: p.y })) : [],
        message: response.message,
        totalCost: response.totalCost,
        nodesExplored: response.nodesExplored,
        executionTimeMs: response.executionTimeMs
      }
      
      console.log('[PathfindingService] 转换后的路径数据:')
      console.log('[PathfindingService]   坐标系统说明: 后端2D X,Y -> 前端3D X,Z')
      console.log('[PathfindingService]   路径点转换: 后端 x -> 前端 waypoint.x (3D X), 后端 y -> 前端 waypoint.y (3D Z)')
      console.log('[PathfindingService]   转换结果:', pathFindingResponse.path)
      console.log('[PathfindingService] ========== 路径规划请求结束 ==========')
      
      return pathFindingResponse
    } catch (error: any) {
      console.error('[PathfindingService] 路径规划失败:', error)
      throw new Error(error.response?.data?.error || error.message || '路径规划失败')
    }
  }

  convert3DToGridCoordinates(position: THREE.Vector3, gridFloor: GridFloor): Waypoint {
    console.log('[convert3DToGridCoordinates] 输入3D位置:', position)
    
    const config = gridFloor.getConfig()
    const cellSize = config.cellSize
    const gridSize = config.gridSize
    const halfSize = (gridSize * cellSize) / 2
    
    console.log('[convert3DToGridCoordinates] cellSize:', cellSize, 'gridSize:', gridSize, 'halfSize:', halfSize)

    const col = Math.floor((position.x + halfSize) / cellSize)
    const row = Math.floor((position.z + halfSize) / cellSize)
    
    console.log('[convert3DToGridCoordinates] 计算结果 - col (X轴):', col, 'row (Y轴):', row)
    console.log('[convert3DToGridCoordinates] 坐标映射: 3D X(', position.x, ') -> 2D X(', col, '), 3D Z(', position.z, ') -> 2D Y(', row, ')')

    const result = {
      x: Math.max(0, Math.min(gridSize - 1, col)),
      y: Math.max(0, Math.min(gridSize - 1, row))
    }
    
    console.log('[convert3DToGridCoordinates] 最终网格坐标:', result)
    
    return result
  }

  convert3DPointsToWaypoints(positions: THREE.Vector3[], gridFloor: GridFloor): Waypoint[] {
    console.log('[convert3DPointsToWaypoints] 开始转换，3D点数量:', positions.length)
    const result = positions.map(pos => this.convert3DToGridCoordinates(pos, gridFloor))
    console.log('[convert3DPointsToWaypoints] 转换结果:', result)
    return result
  }

  convertGridTo3DCoordinates(waypoint: Waypoint, gridFloor: GridFloor, height: number = 0): THREE.Vector3 {
    console.log('[convertGridTo3DCoordinates] 输入2D网格坐标:', waypoint)
    console.log('[convertGridTo3DCoordinates] 高度(Y轴):', height)
    console.log('[convertGridTo3DCoordinates] gridFloor对象:', gridFloor)
    
    const config = gridFloor.getConfig()
    console.log('[convertGridTo3DCoordinates] gridFloor配置:', config)
    console.log('[convertGridTo3DCoordinates] config.cellSize:', config.cellSize)
    console.log('[convertGridTo3DCoordinates] config.gridSize:', config.gridSize)
    
    const cellSize = config.cellSize
    const gridSize = config.gridSize
    const halfSize = (gridSize * cellSize) / 2
    
    console.log('[convertGridTo3DCoordinates] cellSize:', cellSize, 'gridSize:', gridSize, 'halfSize:', halfSize)
    console.log('[convertGridTo3DCoordinates] 2D坐标: waypoint.x (对应3D X轴):', waypoint.x, 'waypoint.y (对应3D Z轴):', waypoint.y)
    console.log('[convertGridTo3DCoordinates] 计算公式: 3D X = (', waypoint.x, '*', cellSize, ') -', halfSize, '+ (', cellSize, '/ 2)')
    console.log('[convertGridTo3DCoordinates] 计算公式: 3D Z = (', waypoint.y, '*', cellSize, ') -', halfSize, '+ (', cellSize, '/ 2)')

    const x = (waypoint.x * cellSize) - halfSize + (cellSize / 2)
    const z = (waypoint.y * cellSize) - halfSize + (cellSize / 2)
    
    console.log('[convertGridTo3DCoordinates] 计算结果 - 3D X:', x, '3D Z:', z)
    console.log('[convertGridTo3DCoordinates] 坐标映射: 2D X(', waypoint.x, ') -> 3D X(', x, '), 2D Y(', waypoint.y, ') -> 3D Z(', z, ')')
    console.log('[convertGridTo3DCoordinates] isNaN(x):', isNaN(x), 'isNaN(z):', isNaN(z))

    const result = new THREE.Vector3(x, height, z)
    console.log('[convertGridTo3DCoordinates] 最终3D坐标:', result)
    
    return result
  }

  convertWaypointsTo3DPoints(waypoints: Waypoint[], gridFloor: GridFloor, height: number = 0): THREE.Vector3[] {
    console.log('[convertWaypointsTo3DPoints] 开始转换，waypoints数量:', waypoints.length)
    console.log('[convertWaypointsTo3DPoints] 输入waypoints:', waypoints)
    
    const result = waypoints.map(wp => this.convertGridTo3DCoordinates(wp, gridFloor, height))
    
    console.log('[convertWaypointsTo3DPoints] 转换结果:', result)
    
    return result
  }

  getObstacleCells(gridFloor: GridFloor): Waypoint[] {
    const cells = gridFloor.getCells()
    const obstacles: Waypoint[] = []
    const gridSize = gridFloor.getConfig().gridSize

    for (let row = 0; row < gridSize; row++) {
      for (let col = 0; col < gridSize; col++) {
        const cell = cells[row][col]
        if (cell.visible && cell.edgeColor !== '#808080') {
          obstacles.push({ x: col, y: row })
        }
      }
    }

    return obstacles
  }
}

const pathfindingService = new PathfindingService()

export { pathfindingService }
