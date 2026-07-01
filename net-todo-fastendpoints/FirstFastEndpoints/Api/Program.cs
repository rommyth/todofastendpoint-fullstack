using FastEndpoints;
using FastEndpoints.Swagger;
using FirstFastEndpoints.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using FastEndpoints.Security;
using FirstFastEndpoints.Shared.Caching;
using Microsoft.AspNetCore.Authentication.JwtBearer;


var builder = WebApplication.CreateBuilder(args);

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

builder.WebHost.UseUrls("http://0.0.0.0:5031");

var app = builder.Build();

app.UseAuthentication();

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



app.Run();
