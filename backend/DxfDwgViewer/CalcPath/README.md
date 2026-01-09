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
- `FindPathWithWaypoints(List<(int X, int Y)> waypoints, List<(int X, int Y)> obstacles = null)` - 多路径点路径规划

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

### 2. 多路径点路径规划

**接口：** `POST /api/pathfinding/find-with-waypoints`

**描述：** 执行多路径点路径规划，路径会按顺序经过所有指定的路径点（起点→路径点1→路径点2→...→终点）

**请求参数：**
```json
{
  "waypoints": [
    {"X": 10, "Y": 10},
    {"X": 30, "Y": 30},
    {"X": 50, "Y": 20},
    {"X": 80, "Y": 80}
  ],
  "obstacles": [
    {"X": 40, "Y": 40},
    {"X": 41, "Y": 40}
  ]
}
```

**参数说明：**
- `waypoints` (array) - 路径点数组，第一个为起点，最后一个为终点，中间为途经点（必填，至少2个点）
  - 每个元素是一个对象，包含 `X` 和 `Y` 坐标
  - 坐标必须为整数，表示网格坐标
  - 坐标必须在网格范围内（0 ≤ X < width, 0 ≤ Y < height）
  - 路径点按顺序排列，AGV会按此顺序依次经过
  - 示例：`[{"X": 10, "Y": 10}, {"X": 30, "Y": 30}, {"X": 50, "Y": 20}, {"X": 80, "Y": 80}]`
  - 表示从(10,10)出发，途经(30,30)和(50,20)，最后到达(80,80)
  
- `obstacles` (array) - 障碍物列表（可选）
  - 每个元素是一个对象，包含 `X` 和 `Y` 坐标
  - 坐标必须为整数，表示网格坐标
  - 坐标必须在网格范围内（0 ≤ X < width, 0 ≤ Y < height）
  - 障碍物表示AGV无法通过的网格单元
  - 如果不提供此参数，则使用当前已设置的障碍物
  - 如果提供此参数，会先清除所有现有障碍物再设置新的
  - 示例：`[{"X": 40, "Y": 40}, {"X": 41, "Y": 40}, {"X": 42, "Y": 40}]`
  - 表示在(40,40)、(41,40)、(42,40)位置设置障碍物

**请求示例：**
```json
{
  "waypoints": [
    {"X": 10, "Y": 10},
    {"X": 30, "Y": 30},
    {"X": 50, "Y": 20},
    {"X": 80, "Y": 80}
  ],
  "obstacles": [
    {"X": 40, "Y": 40},
    {"X": 41, "Y": 40}
  ]
}
```

**请求注意事项：**
1. `waypoints` 数组必须至少包含2个点（起点和终点）
2. 所有坐标必须在网格范围内，否则会返回错误
3. 起点和终点不能设置为障碍物
4. 如果 `obstacles` 为空数组 `[]`，会清除所有现有障碍物
5. 如果 `obstacles` 为 `null` 或不提供，则使用当前已设置的障碍物
6. 路径点之间不能有重复（除相邻路径段的连接点外）

**响应示例：**
```json
{
  "success": true,
  "path": [
    {"X": 10, "Y": 10},
    {"X": 11, "Y": 11},
    {"X": 12, "Y": 12},
    {"X": 13, "Y": 13},
    {"X": 14, "Y": 14},
    {"X": 15, "Y": 15},
    {"X": 16, "Y": 16},
    {"X": 17, "Y": 17},
    {"X": 18, "Y": 18},
    {"X": 19, "Y": 19},
    {"X": 20, "Y": 20},
    {"X": 21, "Y": 21},
    {"X": 22, "Y": 22},
    {"X": 23, "Y": 23},
    {"X": 24, "Y": 24},
    {"X": 25, "Y": 25},
    {"X": 26, "Y": 26},
    {"X": 27, "Y": 27},
    {"X": 28, "Y": 28},
    {"X": 29, "Y": 29},
    {"X": 30, "Y": 30},
    {"X": 31, "Y": 29},
    {"X": 32, "Y": 28},
    {"X": 33, "Y": 27},
    {"X": 34, "Y": 26},
    {"X": 35, "Y": 25},
    {"X": 36, "Y": 24},
    {"X": 37, "Y": 23},
    {"X": 38, "Y": 22},
    {"X": 39, "Y": 21},
    {"X": 40, "Y": 20},
    {"X": 41, "Y": 20},
    {"X": 42, "Y": 20},
    {"X": 43, "Y": 20},
    {"X": 44, "Y": 20},
    {"X": 45, "Y": 20},
    {"X": 46, "Y": 20},
    {"X": 47, "Y": 20},
    {"X": 48, "Y": 20},
    {"X": 49, "Y": 20},
    {"X": 50, "Y": 20},
    {"X": 51, "Y": 21},
    {"X": 52, "Y": 22},
    {"X": 53, "Y": 23},
    {"X": 54, "Y": 24},
    {"X": 55, "Y": 25},
    {"X": 56, "Y": 26},
    {"X": 57, "Y": 27},
    {"X": 58, "Y": 28},
    {"X": 59, "Y": 29},
    {"X": 60, "Y": 30},
    {"X": 61, "Y": 31},
    {"X": 62, "Y": 32},
    {"X": 63, "Y": 33},
    {"X": 64, "Y": 34},
    {"X": 65, "Y": 35},
    {"X": 66, "Y": 36},
    {"X": 67, "Y": 37},
    {"X": 68, "Y": 38},
    {"X": 69, "Y": 39},
    {"X": 70, "Y": 40},
    {"X": 71, "Y": 41},
    {"X": 72, "Y": 42},
    {"X": 73, "Y": 43},
    {"X": 74, "Y": 44},
    {"X": 75, "Y": 45},
    {"X": 76, "Y": 46},
    {"X": 77, "Y": 47},
    {"X": 78, "Y": 48},
    {"X": 79, "Y": 49},
    {"X": 80, "Y": 80}
  ],
  "totalCost": 185.41019662496845,
  "nodesExplored": 245,
  "message": "成功规划经过 4 个点的路径",
  "executionTimeMs": 12
}
```

**返回数据格式说明：**
- `success` (boolean) - 路径规划是否成功
  - `true` - 成功找到路径
  - `false` - 未找到路径（可能原因：起点或终点在障碍物上、路径被完全阻挡等）
  
- `path` (array) - 路径点数组，包含从起点到终点的所有网格坐标
  - 每个元素是一个对象，包含 `X` 和 `Y` 坐标
  - 数组第一个元素为起点，最后一个元素为终点
  - 路径点按顺序排列，可以直接用于AGV导航
  - 如果 `success` 为 `false`，此数组可能为空或包含部分路径
  
- `totalCost` (number) - 路径总代价
  - 使用欧几里得距离计算（对角线移动代价为√2，直线移动代价为1）
  - 值越小表示路径越短
  - 可用于评估路径质量
  
- `nodesExplored` (integer) - 算法探索的节点数量
  - 表示A*算法在寻找路径过程中检查的网格节点总数
  - 可用于评估算法性能
  - 值越大表示计算量越大
  
- `message` (string) - 执行结果消息
  - 成功时：`"成功规划经过 N 个点的路径"`（N为路径点数量）
  - 失败时：`"无法从点 (X1, Y1) 到点 (X2, Y2) 找到路径: 具体原因"`
  - 包含详细的错误信息，便于调试
  
- `executionTimeMs` (integer) - 执行时间（毫秒）
  - 路径规划算法的总执行时间
  - 可用于性能监控和优化
  - 通常在1-20ms之间，取决于路径复杂度

**失败响应示例：**
```json
{
  "success": false,
  "path": [
    {"X": 10, "Y": 10},
    {"X": 11, "Y": 11},
    {"X": 12, "Y": 12}
  ],
  "totalCost": 2.8284271247461903,
  "nodesExplored": 15,
  "message": "无法从点 (12, 12) 到点 (50, 50) 找到路径: 无法到达目标点",
  "executionTimeMs": 5
}
```

**注意事项：**
1. 路径点坐标为整数，表示网格坐标
2. 路径会包含起点和终点
3. 如果某个路径段无法找到路径，会返回失败，并包含已规划的部分路径
4. 路径点顺序为：起点 → 途经点1 → 途经点2 → ... → 终点
5. 每个路径段都是独立规划的，相邻路径段的连接点会自动合并（避免重复）

### 5. 设置障碍物

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

**返回数据格式说明：**
- `success` (boolean) - 操作是否成功
  - `true` - 成功设置障碍物
  - `false` - 设置失败（可能原因：坐标超出网格范围等）
  
- `message` (string) - 执行结果消息
  - 成功时：`"成功设置 N 个障碍物"`（N为障碍物数量）
  - 失败时：`"设置障碍物失败: 具体原因"`
  
- `gridSize` (object) - 网格尺寸信息
  - `width` (integer) - 网格宽度
  - `height` (integer) - 网格高度
  - 可用于验证障碍物坐标是否在有效范围内

**失败响应示例：**
```json
{
  "success": false,
  "message": "设置障碍物失败: 坐标 (150, 50) 超出网格范围",
  "gridSize": {
    "width": 100,
    "height": 100
  }
}
```

**注意事项：**
1. 障碍物坐标必须在网格范围内（0 ≤ X < width, 0 ≤ Y < height）
2. 如果 `clearExisting` 为 `true`，会先清除所有现有障碍物再设置新的
3. 如果 `clearExisting` 为 `false`，新的障碍物会添加到现有障碍物列表中
4. 重复设置相同坐标的障碍物不会产生错误
5. 起点和终点不能设置为障碍物，否则路径规划会失败

## 使用示例

### 示例1：多路径点路径规划

```csharp
// 创建路径规划服务
var service = new PathFindingService(100, 100, true);

// 定义路径点（起点→路径点1→路径点2→终点）
var waypoints = new List<(int, int)>
{
    (10, 10),  // 起点
    (30, 30),  // 路径点1
    (50, 20),  // 路径点2
    (80, 80)   // 终点
};

// 定义障碍物
var obstacles = new List<(int, int)>
{
    (40, 40), (41, 40), (42, 40)
};

// 执行多路径点路径规划
var result = service.FindPathWithWaypoints(waypoints, obstacles);

if (result.Success)
{
    Console.WriteLine($"多路径点规划成功！");
    Console.WriteLine($"经过 {waypoints.Count} 个路径点");
    Console.WriteLine($"总代价: {result.TotalCost:F2}");
    Console.WriteLine($"探索节点数: {result.NodesExplored}");
    Console.WriteLine($"执行时间: {result.ExecutionTimeMs}ms");
    Console.WriteLine($"路径点总数: {result.Path.Count}");
}
else
{
    Console.WriteLine($"多路径点规划失败: {result.Message}");
}
```

### 示例2：动态添加障碍物

```csharp
// 创建路径规划服务
var service = new PathFindingService(100, 100, true);

// 定义路径点
var waypoints = new List<(int, int)>
{
    (10, 10),
    (50, 50),
    (90, 90)
};

// 先规划一条路径
var result1 = service.FindPathWithWaypoints(waypoints);
Console.WriteLine($"第一次规划: {result1.Success}");

// 添加障碍物
service.SetObstacle(30, 30, true);
service.SetObstacle(31, 30, true);
service.SetObstacle(30, 31, true);

// 再次规划路径
var result2 = service.FindPathWithWaypoints(waypoints);
Console.WriteLine($"第二次规划: {result2.Success}");

// 清除所有障碍物
service.ClearObstacles();

// 第三次规划路径
var result3 = service.FindPathWithWaypoints(waypoints);
Console.WriteLine($"第三次规划: {result3.Success}");
```

### 示例3：使用PowerShell调用API

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

# 多路径点路径规划
$body = @{
    waypoints = @(
        @{X = 10; Y = 10},
        @{X = 30; Y = 30},
        @{X = 50; Y = 20},
        @{X = 80; Y = 80}
    )
    obstacles = @(
        @{X = 40; Y = 40},
        @{X = 41; Y = 40}
    )
} | ConvertTo-Json -Depth 3

Invoke-RestMethod -Uri "http://localhost:5001/api/pathfinding/find-with-waypoints" -Method Post -Body $body -ContentType "application/json"
```

### 示例4：使用JavaScript/Fetch调用API

```javascript
// 获取网格信息
async function getGridInfo() {
    const response = await fetch('http://localhost:5001/api/pathfinding/grid-info');
    const data = await response.json();
    console.log('网格信息:', data);
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

// 多路径点路径规划
async function findPathWithWaypoints(waypoints, obstacles = null) {
    const response = await fetch('http://localhost:5001/api/pathfinding/find-with-waypoints', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            waypoints: waypoints,
            obstacles: obstacles
        })
    });
    const data = await response.json();
    console.log('多路径点路径规划结果:', data);
    return data;
}

// 使用示例
(async () => {
    // 获取网格信息
    await getGridInfo();
    
    // 设置障碍物
    const obstacles = [
        {X: 50, Y: 50},
        {X: 51, Y: 50},
        {X: 52, Y: 50}
    ];
    await setObstacles(obstacles, false);
    
    // 多路径点路径规划
    const waypoints = [
        {X: 10, Y: 10},
        {X: 30, Y: 30},
        {X: 50, Y: 20},
        {X: 80, Y: 80}
    ];
    const result = await findPathWithWaypoints(waypoints, obstacles);
    console.log('最终结果:', result);
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

- v1.2.0 - 简化API，统一使用多路径点路径规划
  - 移除 `FindPath` 和 `FindPathWithObstacles` 方法
  - 统一使用 `FindPathWithWaypoints` 方法进行路径规划
  - 移除 `/api/pathfinding/find` 和 `/api/pathfinding/find-with-obstacles` API端点
  - 简化API设计，所有路径规划都使用多路径点接口
  - 更新文档，移除旧API的使用示例

- v1.1.0 - 新增多路径点路径规划功能
  - 新增 `FindPathWithWaypoints` 方法，支持传入多个路径点
  - 路径会按顺序经过所有指定的路径点（起点→路径点1→路径点2→...→终点）
  - 新增 API 端点 `/api/pathfinding/find-with-waypoints`
  - 支持在多路径点规划时设置障碍物
  - 更新文档，添加多路径点使用示例

- v1.0.0 - 初始版本，实现基本的A*路径规划算法
  - 支持基本的路径规划
  - 支持障碍物设置
  - 提供REST API接口
  - 支持对角线移动

## 许可证

本项目采用MIT许可证。
