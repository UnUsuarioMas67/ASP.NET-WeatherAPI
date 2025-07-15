using System.Threading.RateLimiting;
using DotNetEnv;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;
using StackExchange.Redis;
using WeatherAPI;

// Load .env file
Env.Load();

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

var endpoint = Environment.GetEnvironmentVariable("REDIS_ENDPOINT")
               ?? throw new InvalidOperationException("REDIS_ENDPOINT not set in .env file");
var user = Environment.GetEnvironmentVariable("REDIS_USERNAME") 
           ?? throw new InvalidOperationException("REDIS_USERNAME not set in .env file");
var password = Environment.GetEnvironmentVariable("REDIS_PASSWORD") 
               ?? throw new InvalidOperationException("REDIS_PASSWORD not set in .env file");

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(new ConfigurationOptions
{
    EndPoints = { endpoint },
    User = user,
    Password = password,
}));
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromSeconds(10);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;

    });
});

var app = builder.Build();

app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

WeatherEndpoint.Map(app);

app.Run();