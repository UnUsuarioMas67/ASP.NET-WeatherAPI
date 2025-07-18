using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;
using WeatherAPI.Models;

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
            })
            .RequireRateLimiting("fixed");
    }

    private static async Task<Results<Ok<WeatherData>, BadRequest, InternalServerError>> GetWeather(
        IHttpClientFactory clientFactory, IConnectionMultiplexer muxer, string location)
    {
        var redisKey = location.ToLower();
        
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var db = muxer.GetDatabase();
        var weatherData = await db.JSON().GetAsync<WeatherData>(redisKey, serializerOptions: jsonOptions);
        
        if (weatherData == null)
        {
            var client = clientFactory.CreateClient();
            var uri =
                $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{location}?unitGroup=metric&key=CKWTDCRXG7KFL6TL6RBE8JJZ4&contentType=json";
            try
            {
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<VisualCrossingResponse>(jsonOptions);
                weatherData = WeatherData.FromVisualCrossingResponse(result!);
                
                await db.JSON().SetAsync(redisKey, "$", weatherData, serializerOptions: jsonOptions);
                await db.KeyExpireAsync(redisKey, TimeSpan.FromHours(12));
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode == HttpStatusCode.BadRequest)
                    return TypedResults.BadRequest();

                return TypedResults.InternalServerError();
            }
        }
        
        return TypedResults.Ok(weatherData);
    }
}