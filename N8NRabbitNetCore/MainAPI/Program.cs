
using System.Reflection;
using System.Text.Json;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Serilog;
using StackExchange.Redis;
using MainAPI.Middlewares;
using Refit;
using MainAPI.Services.N8N;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// disributed cache injections for redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("Redis:ConnectionString");
});
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>("Redis:ConnectionString")));

// Http context injections
builder.Services.AddHttpContextAccessor();

// Controller feature injections
builder.Services.AddControllers();

// Swagger options
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
.AddRefitClient<IN8NAPI>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("n8n:baseURL"));
        c.DefaultRequestHeaders.Add("Authorization", "123456");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// logging, security, exception handling middlewares
app.MiddlewareSetup();

app.MapControllers();


// logging options for serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "MainAPI")
    .ReadFrom.Configuration(app.Configuration)
    .CreateLogger();

try
{
    Log.Information("Application {MainAPI} Starting...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application {MainAPI} stopped unexpectedly.");
}
finally
{
    Log.Information("Application {MainAPI} stopped.");
    Log.CloseAndFlush();
}
