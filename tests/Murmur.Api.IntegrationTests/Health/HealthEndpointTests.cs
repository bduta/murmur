using System.Net;
using System.Text.Json;

using Murmur.Api.IntegrationTests.Infrastructure;

using Shouldly;

using Xunit;

namespace Murmur.Api.IntegrationTests.Health;

public sealed class HealthEndpointTests(PostgresFixture postgres)
{
    [Fact]
    public async Task Returns_200_Healthy_with_version_and_database_check_when_database_is_reachable()
    {
        await using var factory = new MurmurApiFactory(postgres.ConnectionString);

        (HttpStatusCode status, JsonElement body, _) = await GetHealthAsync(factory);

        status.ShouldBe(HttpStatusCode.OK);
        body.GetProperty("status").GetString().ShouldBe("Healthy");
        body.GetProperty("version").GetString().ShouldNotBeNullOrWhiteSpace();
        body.GetProperty("checks").GetProperty("PostgreSql").GetString().ShouldBe("Healthy");
    }

    // Intended: readiness reports the database. Liveness (/alive) stays up; see the M0b note in the M0a milestone.
    [Fact]
    public async Task Returns_503_Unhealthy_when_database_is_unreachable()
    {
        await using var factory = new MurmurApiFactory(MurmurApiFactory.UnreachableDatabase);

        (HttpStatusCode status, JsonElement body, _) = await GetHealthAsync(factory);

        status.ShouldBe(HttpStatusCode.ServiceUnavailable);
        body.GetProperty("status").GetString().ShouldBe("Unhealthy");
        body.GetProperty("checks").GetProperty("PostgreSql").GetString().ShouldBe("Unhealthy");
    }

    [Fact]
    public async Task Unhealthy_response_contains_no_exception_details()
    {
        await using var factory = new MurmurApiFactory(MurmurApiFactory.UnreachableDatabase);

        (_, JsonElement body, string raw) = await GetHealthAsync(factory);

        body.EnumerateObject().Select(property => property.Name).ShouldBe(["status", "version", "checks"]);
        body.GetProperty("checks").EnumerateObject().ShouldAllBe(check => check.Value.ValueKind == JsonValueKind.String);
        raw.ShouldNotContain("Exception", Case.Insensitive);
        raw.ShouldNotContain("Npgsql", Case.Insensitive);
        raw.ShouldNotContain("127.0.0.1");
        raw.ShouldNotContain("Port", Case.Insensitive);
    }

    private static async Task<(HttpStatusCode Status, JsonElement Body, string Raw)> GetHealthAsync(MurmurApiFactory factory)
    {
        using HttpClient client = factory.CreateClient();
        using HttpResponseMessage response = await client.GetAsync("/api/health", TestContext.Current.CancellationToken);
        string raw = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        using var document = JsonDocument.Parse(raw);
        return (response.StatusCode, document.RootElement.Clone(), raw);
    }
}
