using Aspire.Web.Components.Pages;

namespace Aspire.Web;

public class ApiServiceClient(HttpClient httpClient)
{
    public async Task<WeatherForecast[]> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<WeatherForecast>? forecasts = null;

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/weatherforecast", cancellationToken))
        {
            if (forecasts?.Count >= maxItems)
            {
                break;
            }
            if (forecast is not null)
            {
                forecasts ??= [];
                forecasts.Add(forecast);
            }
        }

        return forecasts?.ToArray() ?? [];
    }
    
    public async Task<Student[]> GetStudentsAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<Student>? students = null;

        await foreach (var student in httpClient.GetFromJsonAsAsyncEnumerable<Student>("/student", cancellationToken))
        {
            if (students?.Count >= maxItems)
            {
                break;
            }
            if (student is not null)
            {
                students ??= [];
                students.Add(student);
            }
        }

        return students?.ToArray() ?? [];
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class Student
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }   
    public string Email { get; set; }  
    public DateOnly Birthday { get; set; }
}