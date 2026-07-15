using FastEndpoints;
using FastEndpoints.Swagger;
using FirstFastEndpoints.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using FastEndpoints.Security;
using FirstFastEndpoints.Shared.Caching;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Prometheus;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Logger: Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// DB
builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("MyMomory"));

// Service Scope Injection
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Redis Scope Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("Redis:Host");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAuthenticationJwtBearer(option =>
{
    option.SigningKey = builder.Configuration["Jwt:Key"];
});

builder.Services.PostConfigure<JwtBearerOptions>(
    JwtBearerDefaults.AuthenticationScheme,
    options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var redis = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

                var jti = context.Principal?.FindFirst("jti")?.Value;

                if (string.IsNullOrEmpty(jti))
                {
                    context.Fail("Missing jti.");
                    return;
                }
                var keys = CacheKeys.TokenBlockId(Guid.Parse(jti));
                if (await redis.KeyExistAsync(keys))
                {
                    context.Fail("Token has been revoked");
                }
            }
        };
    }
);

builder.Services
    .AddAuthorization()
    .AddFastEndpoints()
    .AddResponseCaching()
    .SwaggerDocument(option => option.DocumentSettings = s =>
    {
        s.Title = "FastEndpoint API";
        s.Version = "v1";
    });

// Helath Checker
builder.Services
    .AddHealthChecks()
    .AddRedis(builder.Configuration.GetValue<string>("Redis:Host")!);

builder.WebHost.UseUrls(builder.Configuration["Server:Host"]!);

var app = builder.Build();

// Map Health Response
 app.MapHealthChecks("/health", new HealthCheckOptions {
     ResponseWriter = async (context, report) => {
         context.Response.ContentType = "application/json";

         var response = new
         {
             Status = report.Status.ToString(),
             TimeStamp = DateTime.UtcNow,
             TotalDuration = report.TotalDuration,
             Checks = report.Entries.ToDictionary(
                 entry => entry.Key,
                 entry => new {
                     Status = entry.Value.Status.ToString(),
                     Description = entry.Value.Description,
                     Duration = entry.Value.Duration,
                     Exception = entry.Value.Exception?.Message
             })
         };

         await context.Response.WriteAsync(
             JsonSerializer.Serialize(response, new JsonSerializerOptions
             {
                 WriteIndented = true
             })
             );
     }
 });

app.UseSerilogRequestLogging();

app.UseAuthentication();

// Prometheus
app.UseHttpMetrics();
app.MapMetrics();

app.UseResponseCaching()
   .UseFastEndpoints(config =>
   {
       config.Endpoints.RoutePrefix = "api";
   })
   .UseCors("AllowAll");

//if (app.Environment.IsDevelopment())
//{
app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");
app.MapScalarApiReference(o => o.AddDocument("v1"));
//}


Log.Information("Application Starting");
Log.Information("Running on http://0.0.0.0:5030");
app.Run();
