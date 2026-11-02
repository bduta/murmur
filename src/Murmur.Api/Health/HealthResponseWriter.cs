using System.Reflection;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Murmur.Api.Health;

internal static class HealthResponseWriter
{
    // e.g. "0.1.0+494bf60…" — the SDK appends the git commit to the informational version.
    private static readonly string Version =
        typeof(HealthResponseWriter).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? "unknown";

    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        var response = new HealthResponse(
            report.Status.ToString(),
            Version,
            report.Entries.ToDictionary(entry => entry.Key, entry => entry.Value.Status.ToString()));

        return context.Response.WriteAsJsonAsync(response, context.RequestAborted);
    }

    private sealed record HealthResponse(string Status, string Version, IReadOnlyDictionary<string, string> Checks);
}
