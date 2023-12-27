
using System.Reflection;
using System.Text.Json;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using DeveloperAPI.Attributes;
using DeveloperAPI.Core.Application.DTOs.SystemUser;
using DeveloperAPI.Core.Application.Utilities.Security;
using DeveloperAPI.Data;
using DeveloperAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// static mock data initialize
Data.DB = JsonSerializer.Deserialize<DBModel>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Content/db.json")));

// current user injections 
builder.Services.AddScoped<ICurrentUser>((IServiceProvider provider) =>
{
    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

    if (httpContextAccessor.HttpContext is not null && httpContextAccessor.HttpContext.Items.ContainsKey("CurrentUser"))
    {
        var user = httpContextAccessor.HttpContext.Items["CurrentUser"] as SystemUserDTO;
        return new CurrentUser()
        {
            Id = user.Id,
            Username = user.Username,
            Fullname = user.Fullname,
            Role = user.Role
        };
    }

    return null;
});

// disributed cache injections for redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("Redis:ConnectionString");
});
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>("Redis:ConnectionString")));

// mediatr config
builder.Services.AddMediatR(opt =>
{
    opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

// security, token injections
builder.Services.AddSingleton<IJWTService, JWTManager>();

// Attribute filters
builder.Services.AddScoped<APIAuthorizeFilter>();

// Http context injections
builder.Services.AddHttpContextAccessor();

// Controller feature injections
builder.Services.AddControllers();

// Swagger options
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    // Bearer token authentication
    var securityDefinition = new OpenApiSecurityScheme()
    {
        Name = "Bearer",
        BearerFormat = "JWT",
        Scheme = "bearer",
        Description = "Specify the authorization token.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
    };
    option.AddSecurityDefinition("jwt_auth", securityDefinition);

    // Make sure swagger UI requires a Bearer token specified
    var securityScheme = new OpenApiSecurityScheme()
    {
        Reference = new OpenApiReference()
        {
            Id = "jwt_auth",
            Type = ReferenceType.SecurityScheme
        }
    };

    var securityRequirements = new OpenApiSecurityRequirement()
    {
        {   securityScheme, new string[] { }    },
    };
    option.AddSecurityRequirement(securityRequirements);

    // Aynı class adının tekrar kullanıbilir olması için eklendi
    option.CustomSchemaIds(type => type.ToString());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// authentication / authorization setup for filtering
app.UseAuthentication();
app.UseAuthorization();

// logging, security, exception handling middlewares
app.MiddlewareSetup();

app.MapControllers();


// logging options for serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "DeveloperAPI")
    .ReadFrom.Configuration(app.Configuration)
    .CreateLogger();

try
{
    Log.Information("Application {DeveloperAPI} Starting...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application {DeveloperAPI} stopped unexpectedly.");
}
finally
{
    Log.Information("Application {DeveloperAPI} stopped.");
    Log.CloseAndFlush();
}
