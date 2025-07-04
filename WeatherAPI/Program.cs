using System.Globalization;
using System.Net;
using System.Text.Json;
using DotNetEnv;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Load .env file
Env.Load();

// Get API key from environment variables
string apiKey = Environment.GetEnvironmentVariable("API_KEY")!;

app.MapGet("/api/weather/{location}/{date1:datetime?}/{date2:datetime?}",
        async Task<Results<Ok<object>, BadRequest, InternalServerError>> (string location, DateTime? date1, DateTime? date2, [FromQuery] string unitGroup = "metric",
            [FromQuery] string lang = "en") =>
        {
            date1 ??= DateTime.UtcNow;
            date2 ??= DateTime.UtcNow.AddDays(14);
            
            var client = new HttpClient();
            var uri = new Uri(
                $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{location}/{date1.Value:yyyy-MM-dd}/{date2.Value:yyyy-MM-dd}?unitGroup={unitGroup}&lang={lang}&include=days%2Chours&key={apiKey}&contentType=json");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            try
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                
                var result = JsonSerializer.Deserialize<object>(content);
                return TypedResults.Ok(result);
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode == HttpStatusCode.BadRequest)
                    return TypedResults.BadRequest();
                
                return TypedResults.InternalServerError();
            }
        })
    .WithName("GetWeatherData")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get Weather Data",
        Description = "Returns weather data from the Visual Crossing API",
        Tags = new List<OpenApiTag> { new() { Name = "Visual Crossing" }, new() { Name = "Weather" } }
    });

app.Run();