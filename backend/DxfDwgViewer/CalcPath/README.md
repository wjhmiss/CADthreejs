# A* 路径规划算法

## 概述

本模块实现了经典的A*（A-Star）路径规划算法，用于在二维网格中寻找从起点到终点的最优路径。A*算法是一种启发式搜索算法，结合了Dijkstra算法的完备性和贪婪最佳优先搜索的效率，是游戏开发和机器人导航中最常用的路径规划算法之一。

## 算法原理

### 核心概念

A*算法通过评估每个节点的总代价F来选择下一个要探索的节点：

```
F = G + H
```

其中：
- **G值**：从起点到当前节点的实际代价
- **H值**：从当前节点到终点的启发式估计代价
- **F值**：总代价，用于优先级队列排序

### 启发式函数

本实现使用欧几里得距离作为启发式函数：

```
H = √((x2 - x1)² + (y2 - y1)²)
```

对于不允许对角线移动的情况，使用曼哈顿距离：

```
H = |x2 - x1| + |y2 - y1|
```

### 算法流程

1. 将起点加入开放列表（Open Set）
2. 从开放列表中选择F值最小的节点作为当前节点
3. 如果当前节点是终点，则路径规划成功，回溯路径
4. 将当前节点从开放列表移到关闭列表（Closed Set）
5. 检查当前节点的所有邻居节点：
   - 如果邻居不可通行或已在关闭列表中，跳过
   - 计算从起点经过当前节点到邻居的代价
   - 如果新路径更优，更新邻居的G值、H值和父节点
   - 将邻居加入开放列表
6. 重复步骤2-5，直到找到终点或开放列表为空

### 时间复杂度

- 最优情况：O(b^d)，其中b是分支因子，d是解的深度
- 最坏情况：O(b^m)，其中m是搜索空间的最大深度
- 实际性能：通常介于O(b^d)和O(b^m)之间，取决于启发式函数的质量

### 空间复杂度

- O(n)，其中n是网格中的节点数量

## 项目结构

```
CalcPath/
├── Node.cs                      # 节点类，表示网格中的每个点
├── GridMap.cs                   # 网格地图类，管理整个网格系统
├── PathFindingResult.cs         # 路径规划结果类
├── AStarPathFinder.cs           # A*算法核心实现
└── PathFindingService.cs        # 路径规划服务类
```

## 核心类说明

### 1. Node.cs

表示网格中的单个节点。

**属性：**
- `X` - 节点的X坐标
- `Y` - 节点的Y坐标
- `G` - 从起点到当前节点的实际代价
- `H` - 从当前节点到终点的启发式估计代价
- `F` - 总代价（G + H），只读属性
- `Parent` - 父节点，用于路径回溯
- `IsWalkable` - 节点是否可通行

**方法：**
- `Equals()` - 判断两个节点是否相同
- `GetHashCode()` - 返回节点的哈希值
- `ToString()` - 返回节点的字符串表示

### 2. GridMap.cs

管理整个网格地图系统。

**属性：**
- `Width` - 网格宽度
- `Height` - 网格高度

**方法：**
- `GetNode(int x, int y)` - 获取指定坐标的节点
- `SetWalkable(int x, int y, bool walkable)` - 设置节点是否可通行
- `GetNeighbors(Node node, bool allowDiagonal)` - 获取节点的邻居
- `Reset()` - 重置所有节点的状态

### 3. PathFindingResult.cs

路径规划结果类。

**属性：**
- `Success` - 是否成功找到路径
- `Path` - 路径点列表（从起点到终点）
- `TotalCost` - 路径总代价
- `NodesExplored` - 探索的节点数量
- `Message` - 结果消息
- `ExecutionTimeMs` - 执行时间（毫秒）

### 4. AStarPathFinder.cs

A*算法核心实现类。

**构造函数：**
- `AStarPathFinder(GridMap gridMap, bool allowDiagonal = true)` - 创建A*路径规划器

**方法：**
- `FindPath(int startX, int startY, int endX, int endY)` - 执行路径规划

**私有方法：**
- `CalculateHeuristic(Node a, Node b)` - 计算启发式值
- `CalculateDistance(Node a, Node b)` - 计算两点间距离
- `ReconstructPath(Node endNode)` - 回溯并构建路径
- `IsNodeInOpenSet(PriorityQueue<Node, double> openSet, Node node)` - 检查节点是否在开放列表中

### 5. PathFindingService.cs

路径规划服务类，提供高级接口。

**构造函数：**
- `PathFindingService(int width, int height, bool allowDiagonal = true)` - 创建路径规划服务

**方法：**
- `SetObstacle(int x, int y, bool isObstacle)` - 设置单个障碍物
- `SetObstacles(List<(int X, int Y)> obstacles)` - 批量设置障碍物
- `ClearObstacles()` - 清除所有障碍物
- `FindPath(int startX, int startY, int endX, int endY)` - 执行路径规划
- `FindPathWithObstacles(int startX, int startY, int endX, int endY, List<(int X, int Y)> obstacles)` - 带障碍物的路径规划

**属性：**
- `GridWidth` - 网格宽度
- `GridHeight` - 网格高度

## API接口说明

### 1. 获取网格信息

**接口：** `GET /api/pathfinding/grid-info`

**描述：** 获取当前网格的尺寸信息

**响应示例：**
```json
{
  "width": 100,
  "height": 100,
  "message": "网格信息获取成功"
}
```

### 2. 基本路径规划

**接口：** `POST /api/pathfinding/find`

**描述：** 执行基本的路径规划，可选择是否包含障碍物

**请求参数：**
```json
{
  "startX": 10,
  "startY": 10,
  "endX": 90,
  "endY": 90,
  "obstacles": [
    {"X": 50, "Y": 50},
    {"X": 51, "Y": 50}
  ]
}
```

**参数说明：**
- `startX` - 起点X坐标（必填）
- `startY` - 起点Y坐标（必填）
- `endX` - 终点X坐标（必填）
- `endY` - 终点Y坐标（必填）
- `obstacles` - 障碍物列表（可选）

**响应示例：**
```json
{
  "success": true,
  "path": [
    {"X": 10, "Y": 10},
    {"X": 11, "Y": 11},
    ...
    {"X": 90, "Y": 90}
  ],
  "totalCost": 113.13708498984747,
  "nodesExplored": 81,
  "message": "路径规划成功",
  "executionTimeMs": 6
}
```

### 3. 带障碍物的路径规划

**接口：** `POST /api/pathfinding/find-with-obstacles`

**描述：** 执行带障碍物的路径规划，每次调用会清除之前的障碍物

**请求参数：**
```json
{
  "startX": 10,
  "startY": 10,
  "endX": 90,
  "endY": 90,
  "obstacles": [
    {"X": 50, "Y": 50},
    {"X": 51, "Y": 50},
    {"X": 52, "Y": 50}
  ]
}
```

**参数说明：**
- `startX` - 起点X坐标（必填）
- `startY` - 起点Y坐标（必填）
- `endX` - 终点X坐标（必填）
- `endY` - 终点Y坐标（必填）
- `obstacles` - 障碍物列表（可选）

**响应示例：**
```json
{
  "success": true,
  "path": [
    {"X": 10, "Y": 10},
    {"X": 11, "Y": 11},
    ...
    {"X": 90, "Y": 90}
  ],
  "totalCost": 113.13708498984747,
  "nodesExplored": 81,
  "message": "路径规划成功",
  "executionTimeMs": 1
}
```

### 4. 设置障碍物

**接口：** `POST /api/pathfinding/set-obstacles`

**描述：** 设置障碍物，可选择是否清除之前的障碍物

**请求参数：**
```json
{
  "obstacles": [
    {"X": 30, "Y": 30},
    {"X": 31, "Y": 30},
    {"X": 32, "Y": 30}
  ],
  "clearExisting": false
}
```

**参数说明：**
- `obstacles` - 障碍物列表（可选）
- `clearExisting` - 是否清除之前的障碍物（默认：false）

**响应示例：**
```json
{
  "success": true,
  "message": "成功设置 3 个障碍物",
  "gridSize": {
    "width": 100,
    "height": 100
  }
}
```

## 使用示例

### 示例1：基本路径规划

```csharp
// 创建路径规划服务（100x100网格，允许对角线移动）
var service = new PathFindingService(100, 100, true);

// 执行路径规划
var result = service.FindPath(10, 10, 90, 90);

if (result.Success)
{
    Console.WriteLine($"路径规划成功！");
    Console.WriteLine($"总代价: {result.TotalCost}");
    Console.WriteLine($"探索节点数: {result.NodesExplored}");
    Console.WriteLine($"执行时间: {result.ExecutionTimeMs}ms");
    
    Console.WriteLine("路径点:");
    foreach (var point in result.Path)
    {
        Console.WriteLine($"({point.X}, {point.Y})");
    }
}
else
{
    Console.WriteLine($"路径规划失败: {result.Message}");
}
```

### 示例2：带障碍物的路径规划

```csharp
// 创建路径规划服务
var service = new PathFindingService(100, 100, true);

// 定义障碍物
var obstacles = new List<(int, int)>
{
    (50, 50), (51, 50), (52, 50),
    (50, 51), (51, 51), (52, 51),
    (50, 52), (51, 52), (52, 52)
};

// 设置障碍物
service.SetObstacles(obstacles);

// 执行路径规划
var result = service.FindPath(10, 10, 90, 90);

if (result.Success)
{
    Console.WriteLine($"路径规划成功，绕过了障碍物！");
    Console.WriteLine($"路径点数量: {result.Path.Count}");
}
```

### 示例3：一次性设置障碍物并规划路径

```csharp
// 创建路径规划服务
var service = new PathFindingService(100, 100, true);

// 定义障碍物
var obstacles = new List<(int, int)>
{
    (30, 30), (31, 30), (32, 30),
    (40, 40), (41, 40), (42, 40)
};

// 一次性设置障碍物并规划路径
var result = service.FindPathWithObstacles(10, 10, 90, 90, obstacles);

if (result.Success)
{
    Console.WriteLine($"路径规划成功！");
    Console.WriteLine($"总代价: {result.TotalCost:F2}");
}
```

### 示例4：动态添加障碍物

```csharp
// 创建路径规划服务
var service = new PathFindingService(100, 100, true);

// 先规划一条路径
var result1 = service.FindPath(10, 10, 90, 90);
Console.WriteLine($"第一次规划: {result1.Success}");

// 添加障碍物
service.SetObstacle(50, 50, true);
service.SetObstacle(51, 50, true);
service.SetObstacle(50, 51, true);

// 再次规划路径
var result2 = service.FindPath(10, 10, 90, 90);
Console.WriteLine($"第二次规划: {result2.Success}");

// 清除所有障碍物
service.ClearObstacles();

// 第三次规划路径
var result3 = service.FindPath(10, 10, 90, 90);
Console.WriteLine($"第三次规划: {result3.Success}");
```

### 示例5：使用PowerShell调用API

```powershell
# 获取网格信息
Invoke-RestMethod -Uri "http://localhost:5001/api/pathfinding/grid-info" -Method Get

# 基本路径规划
$body = @{
    startX = 10
    startY = 10
    endX = 90
    endY = 90
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5001/api/pathfinding/find" -Method Post -Body $body -ContentType "application/json"

# 带障碍物的路径规划
$body = @{
    startX = 10
    startY = 10
    endX = 90
    endY = 90
    obstacles = @(
        @{X = 50; Y = 50},
        @{X = 51; Y = 50},
        @{X = 52; Y = 50}
    )
} | ConvertTo-Json -Depth 3

Invoke-RestMethod -Uri "http://localhost:5001/api/pathfinding/find-with-obstacles" -Method Post -Body $body -ContentType "application/json"

# 设置障碍物
$body = @{
    obstacles = @(
        @{X = 30; Y = 30},
        @{X = 31; Y = 30}
    )
    clearExisting = $false
} | ConvertTo-Json -Depth 3

Invoke-RestMethod -Uri "http://localhost:5001/api/pathfinding/set-obstacles" -Method Post -Body $body -ContentType "application/json"
```

### 示例6：使用JavaScript/Fetch调用API

```javascript
// 获取网格信息
async function getGridInfo() {
    const response = await fetch('http://localhost:5001/api/pathfinding/grid-info');
    const data = await response.json();
    console.log('网格信息:', data);
}

// 基本路径规划
async function findPath(startX, startY, endX, endY) {
    const response = await fetch('http://localhost:5001/api/pathfinding/find', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            startX: startX,
            startY: startY,
            endX: endX,
            endY: endY
        })
    });
    const data = await response.json();
    console.log('路径规划结果:', data);
    return data;
}

// 带障碍物的路径规划
async function findPathWithObstacles(startX, startY, endX, endY, obstacles) {
    const response = await fetch('http://localhost:5001/api/pathfinding/find-with-obstacles', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            startX: startX,
            startY: startY,
            endX: endX,
            endY: endY,
            obstacles: obstacles
        })
    });
    const data = await response.json();
    console.log('路径规划结果:', data);
    return data;
}

// 设置障碍物
async function setObstacles(obstacles, clearExisting = false) {
    const response = await fetch('http://localhost:5001/api/pathfinding/set-obstacles', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            obstacles: obstacles,
            clearExisting: clearExisting
        })
    });
    const data = await response.json();
    console.log('设置障碍物结果:', data);
    return data;
}

// 使用示例
(async () => {
    // 获取网格信息
    await getGridInfo();
    
    // 基本路径规划
    const result1 = await findPath(10, 10, 90, 90);
    
    // 带障碍物的路径规划
    const obstacles = [
        {X: 50, Y: 50},
        {X: 51, Y: 50},
        {X: 52, Y: 50}
    ];
    const result2 = await findPathWithObstacles(10, 10, 90, 90, obstacles);
    
    // 设置障碍物
    await setObstacles(obstacles, false);
})();
```

## 性能优化建议

### 1. 网格大小

- 小型网格（< 50x50）：适合实时交互，响应时间 < 1ms
- 中型网格（50-200x200）：适合大多数应用场景，响应时间 1-10ms
- 大型网格（> 200x200）：需要考虑分层路径规划或区域划分

### 2. 障碍物处理

- 预先设置障碍物，避免在每次路径规划时重复设置
- 使用障碍物缓存，减少重复计算
- 对于静态障碍物，可以考虑预计算可达性图

### 3. 启发式函数

- 欧几里得距离：适合允许对角线移动的场景
- 曼哈顿距离：适合不允许对角线移动的场景
- 切比雪夫距离：适合八方向移动的场景

### 4. 对角线移动

- 允许对角线移动：路径更短，但计算量增加
- 禁止对角线移动：路径较长，但计算量减少

## 常见问题

### Q1: 路径规划失败怎么办？

**A:** 检查以下几点：
1. 起点和终点是否在网格范围内
2. 起点和终点是否可通行
3. 是否存在从起点到终点的可行路径
4. 检查返回的Message字段获取详细错误信息

### Q2: 如何提高路径规划速度？

**A:** 可以尝试以下方法：
1. 减小网格尺寸
2. 禁用对角线移动
3. 优化启发式函数
4. 使用分层路径规划
5. 预计算可达性图

### Q3: 如何处理动态障碍物？

**A:** 有两种方法：
1. 每次路径规划时清除并重新设置障碍物
2. 使用PathFindingService的动态障碍物管理功能

### Q4: 路径不是最短的怎么办？

**A:** A*算法保证找到的是最短路径，如果路径不是最短的，可能是因为：
1. 启发式函数选择不当
2. 对角线移动的代价计算有误
3. 障碍物设置导致路径必须绕行

### Q5: 如何可视化路径？

**A:** 可以使用以下方法：
1. 在网格上绘制路径点
2. 使用不同颜色标记起点、终点和障碍物
3. 显示路径的序号或方向
4. 使用动画展示路径规划过程

## 扩展功能

### 1. 多目标路径规划

可以扩展算法以支持多个目标点，找到访问所有目标点的最优路径（旅行商问题）。

### 2. 动态权重

可以为不同的网格单元设置不同的移动代价（例如：草地、水域、山地等）。

### 3. 路径平滑

可以对生成的路径进行平滑处理，减少路径中的锯齿。

### 4. 实时重规划

当环境发生变化时，可以实时重新规划路径。

### 5. 分层路径规划

对于大型地图，可以使用分层路径规划来提高性能。

## 技术支持

如有问题或建议，请联系开发团队。

## 版本历史

- v1.0.0 - 初始版本，实现基本的A*路径规划算法
  - 支持基本的路径规划
  - 支持障碍物设置
  - 提供REST API接口
  - 支持对角线移动

## 许可证

本项目采用MIT许可证。
