using System.Text.Json;
using StackExchange.Redis;

var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "redis";
var redisPort = Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379";
var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME") ?? "sim_queue";
var resultPrefix = Environment.GetEnvironmentVariable("RESULT_PREFIX") ?? "sim:result:";

var mux = await ConnectionMultiplexer.ConnectAsync($"{redisHost}:{redisPort}");
var db = mux.GetDatabase();

Console.WriteLine("Worker iniciado...");

while (true)
{
    var item = await db.ListLeftPopAsync(queueName);
    if (!item.HasValue)
    {
        await Task.Delay(500);
        continue;
    }

    var task = JsonSerializer.Deserialize<SimulationTask>(item!);
    if (task == null)
        continue;

    await db.StringSetAsync($"{resultPrefix}{task.id}:status", "processing");

    var result = task.type switch
    {
        "pi" => new { pi = EstimatePi(task.iterations) },
        "montecarlo" => new { mean = MonteCarlo(task.iterations) },
        "probability" => new { probability = ProbabilitySim(task.iterations) },
        _ => new { error = "unknown simulation type" }
    };

    var payload = new
    {
        id = task.id,
        type = task.type,
        iterations = task.iterations,
        status = "done",
        result
    };

    await db.StringSetAsync($"{resultPrefix}{task.id}", JsonSerializer.Serialize(payload));
    await db.StringSetAsync($"{resultPrefix}{task.id}:status", "done");
}

double EstimatePi(int n)
{
    var rnd = Random.Shared;
    int inside = 0;
    for (int i = 0; i < n; i++)
    {
        var x = rnd.NextDouble();
        var y = rnd.NextDouble();
        if (x * x + y * y <= 1.0) inside++;
    }
    return 4.0 * inside / n;
}

double MonteCarlo(int n)
{
    var rnd = Random.Shared;
    double sum = 0;
    for (int i = 0; i < n; i++)
    {
        sum += rnd.NextDouble();
    }
    return sum / n;
}

double ProbabilitySim(int n)
{
    var rnd = Random.Shared;
    int success = 0;
    for (int i = 0; i < n; i++)
    {
        if (rnd.NextDouble() > 0.7) success++;
    }
    return (double)success / n;
}

record SimulationTask(string id, string type, int iterations);

