using System.Text.Json;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "redis";
var redisPort = Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379";
var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME") ?? "sim_queue";
var resultPrefix = Environment.GetEnvironmentVariable("RESULT_PREFIX") ?? "sim:result:";

var mux = await ConnectionMultiplexer.ConnectAsync($"{redisHost}:{redisPort}");
var db = mux.GetDatabase();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/simulate", async (string type, int iterations = 100000) =>
{
    var id = Guid.NewGuid().ToString();
    var payload = new
    {
        id,
        type,
        iterations
    };
    await db.ListRightPushAsync(queueName, JsonSerializer.Serialize(payload));
    await db.StringSetAsync($"{resultPrefix}{id}:status", "pending");
    return Results.Ok(new { task_id = id, status = "queued" });
});

app.MapGet("/result/{id}", async (string id) =>
{
    var value = await db.StringGetAsync($"{resultPrefix}{id}");
    if (value.HasValue)
    {
        return Results.Ok(JsonSerializer.Deserialize<object>(value!));
    }
    var status = await db.StringGetAsync($"{resultPrefix}{id}:status");
    if (status.HasValue)
    {
        return Results.Ok(new { id, status = status.ToString() });
    }
    return Results.NotFound();
});

app.Run();

