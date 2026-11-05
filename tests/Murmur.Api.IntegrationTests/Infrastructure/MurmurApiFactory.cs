using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Murmur.Api.IntegrationTests.Infrastructure;

/// <summary>Runs the real Murmur.Api in memory, pointed at the given database.</summary>
public sealed class MurmurApiFactory(string connectionString) : WebApplicationFactory<Program>
{
    /// <summary>
    /// Nothing listens on port 1, so every connection is refused. The short timeout keeps the test fast
    /// even where a refused connection is retried (Windows).
    /// </summary>
    public const string UnreachableDatabase = "Host=127.0.0.1;Port=1;Database=murmur;Username=murmur;Password=unused;Timeout=1";

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        // The same key the AppHost injects for the "murmur" database resource.
        builder.UseSetting("ConnectionStrings:murmur", connectionString);
}
