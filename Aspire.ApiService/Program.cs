using System.Text;
using System.Text.Json;
using Aspire.ApiService;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);


// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRedisDistributedCache("cache");
builder.AddNpgsqlDbContext<DatabaseContext>("postgresdb");

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async (IDistributedCache cache) =>
    {
        var cachedForecast = await cache.GetAsync("forecast");

        if (cachedForecast is null)
        {
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                .ToArray();

            await cache.SetAsync("forecast", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(forecast)), new ()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(10)
            }); ;

            return forecast;
        }

        return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(cachedForecast);
    })
    .WithName("GetWeatherForecast");

app.MapGet("/student", async (IDistributedCache cache, DatabaseContext db) =>
    {
        var cachedStudents = await cache.GetAsync("students");

        if (cachedStudents is null)
        {
            var students = db.Students;
            await cache.SetAsync("students", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(students)), new ()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(10)
            }); ;

            return students;
        }

        return JsonSerializer.Deserialize<IEnumerable<Student>>(cachedStudents);
    })
    .WithName("GetStudents");


app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
