# A* 路径规划算法 API文档

## API接口说明

### 多路径点路径规划

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
  ],
  "gridWidth": 100,
  "gridHeight": 100,
  "allowDiagonal": true
}
```

**参数说明：**
- `waypoints` (array) - 路径点数组，第一个为起点，最后一个为终点，中间为途经点（必填，至少2个点）
  - 每个元素是一个对象，包含 `X` 和 `Y` 坐标
  - 坐标必须为整数，表示网格坐标
  - 坐标必须在网格范围内（0 ≤ X < gridWidth, 0 ≤ Y < gridHeight）
  - 路径点按顺序排列，AGV会按此顺序依次经过
  - 示例：`[{"X": 10, "Y": 10}, {"X": 30, "Y": 30}, {"X": 50, "Y": 20}, {"X": 80, "Y": 80}]`
  - 表示从(10,10)出发，途经(30,30)和(50,20)，最后到达(80,80)
  
- `obstacles` (array) - 障碍物列表（可选）
  - 每个元素是一个对象，包含 `X` 和 `Y` 坐标
  - 坐标必须为整数，表示网格坐标
  - 坐标必须在网格范围内（0 ≤ X < gridWidth, 0 ≤ Y < gridHeight）
  - 障碍物表示AGV无法通过的网格单元
  - 如果不提供此参数，则地图上没有障碍物
  - 示例：`[{"X": 40, "Y": 40}, {"X": 41, "Y": 40}, {"X": 42, "Y": 40}]`
  - 表示在(40,40)、(41,40)、(42,40)位置设置障碍物

- `gridWidth` (integer) - 地图网格宽度（可选，默认值：100）
  - 表示地图在X方向的网格数量
  - 必须为正整数
  - 所有路径点和障碍物的X坐标必须小于此值
  - 示例：`100` 表示地图宽度为100个网格单元

- `gridHeight` (integer) - 地图网格高度（可选，默认值：100）
  - 表示地图在Y方向的网格数量
  - 必须为正整数
  - 所有路径点和障碍物的Y坐标必须小于此值
  - 示例：`100` 表示地图高度为100个网格单元

- `allowDiagonal` (boolean) - 是否允许对角线移动（可选，默认值：true）
  - `true` - 允许AGV沿对角线方向移动（8个方向）
  - `false` - 只允许AGV沿上下左右四个方向移动（4个方向）
  - 对角线移动的代价为√2（约1.414），直线移动的代价为1
  - 示例：`true` 表示允许对角线移动，路径可能更短但计算量稍大

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
  ],
  "gridWidth": 100,
  "gridHeight": 100,
  "allowDiagonal": true
}
```

**请求注意事项：**
1. `waypoints` 数组必须至少包含2个点（起点和终点）
2. 所有坐标必须在网格范围内，否则会返回错误
3. 起点和终点不能设置为障碍物
4. `gridWidth` 和 `gridHeight` 必须为正整数，默认值为100
5. 如果 `obstacles` 为 `null` 或不提供，则地图上没有障碍物
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

## 使用示例

### 示例1：多路径点路径规划

```csharp
// 创建路径规划服务
var service = new PathFindingService();

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

// 定义地图参数
int gridWidth = 100;
int gridHeight = 100;
bool allowDiagonal = true;

// 执行多路径点路径规划
var result = service.FindPathWithWaypoints(waypoints, obstacles, gridWidth, gridHeight, allowDiagonal);

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

### 示例2：多次独立路径规划

```csharp
// 第一次路径规划
var service1 = new PathFindingService();
var waypoints1 = new List<(int, int)>
{
    (10, 10),
    (50, 50),
    (90, 90)
};
var obstacles1 = new List<(int, int)>
{
    (30, 30), (31, 30), (30, 31)
};
var result1 = service1.FindPathWithWaypoints(waypoints1, obstacles1, 100, 100, true);
Console.WriteLine($"第一次规划: {result1.Success}");

// 第二次路径规划（独立的地图实例，使用不同的地图参数）
var service2 = new PathFindingService();
var waypoints2 = new List<(int, int)>
{
    (5, 5),
    (40, 60),
    (80, 20)
};
var obstacles2 = new List<(int, int)>
{
    (20, 20), (21, 20)
};
var result2 = service2.FindPathWithWaypoints(waypoints2, obstacles2, 150, 120, false);
Console.WriteLine($"第二次规划: {result2.Success}");
```

### 示例3：使用PowerShell调用API

```powershell
# 多路径点路径规划
$body = @{
    waypoints = @(
        @{ X = 10; Y = 10 },
        @{ X = 30; Y = 30 },
        @{ X = 50; Y = 20 },
        @{ X = 80; Y = 80 }
    )
    obstacles = @(
        @{ X = 40; Y = 40 },
        @{ X = 41; Y = 40 }
    )
    gridWidth = 100
    gridHeight = 100
    allowDiagonal = $true
} | ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri "http://localhost:5001/api/pathfinding/find-with-waypoints" -Method Post -Body $body -ContentType "application/json"

# 带多个途经点的路径规划（使用不同的地图参数）
$body = @{
    waypoints = @(
        @{ X = 5; Y = 5 },
        @{ X = 20; Y = 30 },
        @{ X = 40; Y = 15 },
        @{ X = 60; Y = 45 },
        @{ X = 90; Y = 90 }
    )
    obstacles = @(
        @{ X = 25; Y = 25 },
        @{ X = 26; Y = 25 },
        @{ X = 27; Y = 25 }
    )
    gridWidth = 150
    gridHeight = 120
    allowDiagonal = $false
} | ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri "http://localhost:5001/api/pathfinding/find-with-waypoints" -Method Post -Body $body -ContentType "application/json"
```

### 示例4：使用JavaScript调用API

```javascript
// 多路径点路径规划
async function findPathWithWaypoints(waypoints, obstacles = null, gridWidth = 100, gridHeight = 100, allowDiagonal = true) {
    const response = await fetch('http://localhost:5001/api/pathfinding/find-with-waypoints', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            waypoints: waypoints,
            obstacles: obstacles,
            gridWidth: gridWidth,
            gridHeight: gridHeight,
            allowDiagonal: allowDiagonal
        })
    });
    
    return await response.json();
}

// 使用示例
const waypoints = [
    { X: 10, Y: 10 },
    { X: 30, Y: 30 },
    { X: 50, Y: 20 },
    { X: 80, Y: 80 }
];

const obstacles = [
    { X: 40, Y: 40 },
    { X: 41, Y: 40 }
];

const result = await findPathWithWaypoints(waypoints, obstacles, 100, 100, true);
console.log('路径规划结果:', result);

if (result.success) {
    console.log('路径点总数:', result.path.length);
    console.log('总代价:', result.totalCost);
    console.log('探索节点数:', result.nodesExplored);
    console.log('执行时间:', result.executionTimeMs, 'ms');
} else {
    console.error('路径规划失败:', result.message);
}
```
