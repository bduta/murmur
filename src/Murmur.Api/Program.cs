using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Murmur.Api.Health;
using Murmur.Api.Modules;
using Murmur.BuildingBlocks.Modules;
using Murmur.Identity;
using Murmur.Posts;

// Explicit list, no assembly scanning: adding a module is a visible one-line change.
IModule[] modules = [new IdentityModule(), new PostsModule()];

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Registers NpgsqlDataSource plus a Postgres health check, tracing and metrics.
builder.AddNpgsqlDataSource("murmur");

// The single source of "now" (ADR-0028). Tests swap in FakeTimeProvider.
builder.Services.AddSingleton(TimeProvider.System);

builder.AddModules(modules);

// Lets tests check that every module project on disk is registered above.
builder.Services.AddSingleton<IReadOnlyList<IModule>>(modules);

var app = builder.Build();

app.MapDefaultEndpoints();

// Public readiness endpoint: overall status, version and per-check status only — never exception details.
app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = HealthResponseWriter.WriteAsync,
});

app.MapModules(modules);

app.Run();
