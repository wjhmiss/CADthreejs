using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using DxfDwgViewer;
using DxfDwgViewer.CalcPath;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "CAD文件解析API", 
        Version = "v1",
        Description = "提供DXF/DWG文件解析和JSON数据导出服务"
    });
    
    c.OperationFilter<FileUploadOperationFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();

app.Urls.Add("http://localhost:5001");

app.MapGet("/", () => "CAD文件解析API服务运行中");

app.MapPost("/api/parse", (ParseRequest request) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(request.FilePath))
        {
            return Results.BadRequest(new { error = "文件路径不能为空" });
        }

        if (!File.Exists(request.FilePath))
        {
            return Results.NotFound(new { error = "文件不存在", filePath = request.FilePath });
        }

        var loader = new CadDocumentLoader();
        var document = loader.LoadDocument(request.FilePath);
        var entities = loader.ExtractEntities(document);

        var generator = new JsonDataGenerator();
        var json = generator.GenerateJsonData(entities);

        return Results.Ok(new 
        { 
            success = true,
            filePath = request.FilePath,
            entityCount = entities.Count,
            data = json
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "解析文件时发生错误"
        );
    }
})
.WithName("ParseCadFile");

app.MapPost("/api/parse/upload", async (HttpContext context, ILogger<Program> logger) =>
{
    try
    {
        var form = await context.Request.ReadFormAsync();
        var file = form.Files.FirstOrDefault();

        logger.LogInformation($"收到文件上传请求，文件名: {file?.FileName}, 文件大小: {file?.Length}");
        
        if (file == null || file.Length == 0)
        {
            logger.LogWarning("文件为空或未上传");
            return Results.BadRequest(new { error = "未上传文件" });
        }

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (extension != ".dxf" && extension != ".dwg")
        {
            logger.LogWarning($"不支持的文件类型: {extension}");
            return Results.BadRequest(new { error = "仅支持DXF或DWG文件", fileName = file.FileName });
        }

        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + extension);
        logger.LogInformation($"临时文件路径: {tempPath}");
        
        using (var stream = new FileStream(tempPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        try
        {
            var loader = new CadDocumentLoader();
            var document = loader.LoadDocument(tempPath);
            var entities = loader.ExtractEntities(document);

            var generator = new JsonDataGenerator();
            var json = generator.GenerateJsonData(entities);

            logger.LogInformation($"文件解析成功，实体数量: {entities.Count}");
            return Results.Ok(new 
            { 
                success = true,
                fileName = file.FileName,
                entityCount = entities.Count,
                data = json
            });
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "解析文件时发生错误");
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "解析文件时发生错误"
        );
    }
})
.WithName("ParseUploadedCadFile");

app.MapGet("/api/parse/drawing1", (ILogger<Program> logger) =>
{
    try
    {
        var drawing1Path = Path.Combine(AppContext.BaseDirectory, "Drawing1.dwg");
        
        logger.LogInformation($"解析Drawing1.dwg，路径: {drawing1Path}");
        
        if (!File.Exists(drawing1Path))
        {
            logger.LogWarning($"Drawing1.dwg文件不存在: {drawing1Path}");
            return Results.NotFound(new { error = "Drawing1.dwg文件不存在", filePath = drawing1Path });
        }

        var loader = new CadDocumentLoader();
        var document = loader.LoadDocument(drawing1Path);
        var entities = loader.ExtractEntities(document);

        var generator = new JsonDataGenerator();
        var json = generator.GenerateJsonData(entities);

        logger.LogInformation($"Drawing1.dwg解析成功，实体数量: {entities.Count}");
        return Results.Ok(new 
        { 
            success = true,
            fileName = "Drawing1.dwg",
            entityCount = entities.Count,
            data = json
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "解析Drawing1.dwg时发生错误");
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "解析Drawing1.dwg时发生错误"
        );
    }
})
.WithName("ParseDrawing1");

app.MapPost("/api/pathfinding/find-with-waypoints", (PathFindingWithWaypointsRequest request, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation($"收到多路径点规划请求: 路径点数量={request.Waypoints?.Count ?? 0}, 障碍物数量={request.Obstacles?.Count ?? 0}, 地图大小={request.GridWidth}x{request.GridHeight}, 允许对角线={request.AllowDiagonal}");
        
        if (request.Waypoints == null || request.Waypoints.Count < 2)
        {
            return Results.BadRequest(new { error = "路径点数量必须大于等于2" });
        }

        var pathFindingService = new PathFindingService();
        var result = pathFindingService.FindPathWithWaypoints(
            request.Waypoints,
            request.Obstacles,
            request.GridWidth,
            request.GridHeight,
            request.AllowDiagonal);
        
        logger.LogInformation($"多路径点规划完成: 成功={result.Success}, 路径长度={result.Path?.Count ?? 0}, 节点数={result.NodesExplored}, 耗时={result.ExecutionTimeMs}ms");
        
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "多路径点规划时发生错误");
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title = "多路径点规划时发生错误"
        );
    }
})
.WithName("FindPathWithWaypoints");

app.Run();

public class ParseRequest
{
    public string FilePath { get; set; } = string.Empty;
}

public class PathFindingWithWaypointsRequest
{
    public List<(int X, int Y)> Waypoints { get; set; } = new List<(int, int)>();
    public List<(int X, int Y)>? Obstacles { get; set; }
    public int GridWidth { get; set; } = 100;
    public int GridHeight { get; set; } = 100;
    public bool AllowDiagonal { get; set; } = true;
}

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileUploadMime = "multipart/form-data";
        if (operation.RequestBody == null || !operation.RequestBody.Content.Any(x => x.Key.Equals(fileUploadMime, StringComparison.InvariantCultureIgnoreCase)))
            return;

        operation.RequestBody.Content[fileUploadMime].Schema.Properties =
            new Dictionary<string, OpenApiSchema>
            {
                { "file", new OpenApiSchema() { Type = "string", Format = "binary", Description = "上传的DXF或DWG文件" } }
            };
    }
}