using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace WeatherAPI;

public static class WeatherEndpoint
{
    private static readonly string ApiKey;
    
    static WeatherEndpoint()
    {
        // Get API key from .env file
        ApiKey = Environment.GetEnvironmentVariable("VISUAL_CROSSING_API_KEY")
                     ?? throw new InvalidOperationException("VISUAL_CROSSING_API_KEY not set in .env file");
    }

    public static void Map(WebApplication app)
    {
        app.MapGet("/api/weather/{location}/{date1:datetime?}/{date2:datetime?}", GetWeather)
            .WithName("GetWeatherData")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Get Weather Data",
                Description = "Returns weather data from the Visual Crossing API",
                Tags = new List<OpenApiTag> { new() { Name = "Visual Crossing" }, new() { Name = "Weather" } },
            });
    }

    private static async Task<Results<Ok<WeatherResult>, BadRequest, InternalServerError>> GetWeather(
        IHttpClientFactory clientFactory, IConnectionMultiplexer muxer, string location, DateTime? date1,
        DateTime? date2)
    {
        date1 ??= DateTime.Now;
        date2 ??= date1.Value.AddDays(14);

        var client = clientFactory.CreateClient("WeatherAPI");
        var uri = $"/VisualCrossingWebServices/rest/services/timeline/{location}/{date1.Value:yyyy-MM-dd}/{date2.Value:yyyy-MM-dd}?unitGroup=metric&elements=datetime%2CdatetimeEpoch%2Cname%2Caddress%2CresolvedAddress%2Clatitude%2Clongitude%2Ctemp%2Cfeelslike%2Cdew%2Cprecip%2Cprecipprob%2Cprecipcover%2Cpreciptype%2Csnow%2Cwindspeed%2Cwinddir%2Cpressure%2Cconditions%2Cdescription%2Cicon&include=hours&key={ApiKey}&contentType=json";

        try
        {
            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
                
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var data = await response.Content.ReadFromJsonAsync<WeatherResult>(options);
            return TypedResults.Ok(data);
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode == HttpStatusCode.BadRequest)
                return TypedResults.BadRequest();

            return TypedResults.InternalServerError();
        }
    }
}