using System.Globalization;
using System.Net;
using System.Text.Json;
using DotNetEnv;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using WeatherAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient("WeatherAPI", client =>
{
    client.BaseAddress = new Uri("https://weather.visualcrossing.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

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

// Get API key from .env file
string apiKey = Environment.GetEnvironmentVariable("API_KEY")!;

app.MapGet("/api/weather/{location}/{date1:datetime?}/{date2:datetime?}",
        async Task<Results<Ok<WeatherResult>, BadRequest, InternalServerError>>
        (IHttpClientFactory factory, string location, DateTime? date1, DateTime? date2, string unitGroup = "metric",
            string lang = "en") =>
        {
            date1 ??= DateTime.Now;
            date2 ??= DateTime.Now.AddDays(14);

            var client = factory.CreateClient("WeatherAPI");
            var uri = $"/VisualCrossingWebServices/rest/services/timeline/{location}/{date1.Value:yyyy-MM-dd}/{date2.Value:yyyy-MM-dd}?unitGroup={unitGroup}&elements=datetime%2CdatetimeEpoch%2Cname%2Caddress%2CresolvedAddress%2Clatitude%2Clongitude%2Ctemp%2Cfeelslike%2Cdew%2Cprecip%2Cprecipprob%2Cprecipcover%2Cpreciptype%2Csnow%2Cwindspeed%2Cwinddir%2Cpressure%2Cconditions%2Cdescription%2Cicon&include=hours&key={apiKey}&contentType=json&lang={lang}";

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
        })
    .WithName("GetWeatherData")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get Weather Data",
        Description = "Returns weather data from the Visual Crossing API",
        Tags = new List<OpenApiTag> { new() { Name = "Visual Crossing" }, new() { Name = "Weather" } }
    });

app.Run();