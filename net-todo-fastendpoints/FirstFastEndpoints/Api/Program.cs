using FastEndpoints;
using FastEndpoints.Swagger;
using FirstFastEndpoints.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("MyMomory"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services
    .AddFastEndpoints()
    .AddResponseCaching()
    .SwaggerDocument(option => option.DocumentSettings = s => {
        s.Title = "FastEndpoint API";
        s.Version = "v1";
    });

builder.WebHost.UseUrls("http://0.0.0.0:5030");

var app = builder.Build();

app.UseResponseCaching()
   .UseFastEndpoints(config => {
       config.Endpoints.RoutePrefix = "api";
   })
   .UseCors("AllowAll");

//if (app.Environment.IsDevelopment())
//{
//    app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");
//    app.MapScalarApiReference(o => o.AddDocument("v1"));
//}

app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");
app.MapScalarApiReference(o => o.AddDocument("v1"));

app.Run();
