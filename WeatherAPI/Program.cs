using System.Net;
using System.Text.Json;
using DotNetEnv;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

WeatherEndpoint.Map(app);

app.Run();