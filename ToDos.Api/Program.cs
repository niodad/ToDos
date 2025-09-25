using ToDos.Api.Extensions;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add validation services
builder.Services.AddProblemDetails();

// Add response compression
builder.Services.AddResponseCompression();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

builder.Services.AddToDosServices(builder.Configuration);

var app = builder.Build();

app.AddToDosMiddleware();

// Add performance monitoring
app.UseMiddleware<ToDos.Api.Middleware.PerformanceMonitoringMiddleware>();

// Add response compression
app.UseResponseCompression();

// Add API key authentication middleware
app.UseMiddleware<ToDos.Api.Middleware.ApiKeyAuthenticationMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapToDoRoutes();

// Add health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

app.UseHttpsRedirection();

app.Run();

