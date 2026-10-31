using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Murmur.Api.Health;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Registers NpgsqlDataSource plus a Postgres health check, tracing and metrics.
builder.AddNpgsqlDataSource("murmur");

var app = builder.Build();

app.MapDefaultEndpoints();

// Public readiness endpoint: overall status, version and per-check status only — never exception details.
app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = HealthResponseWriter.WriteAsync,
});

app.Run();
