using Testcontainers.PostgreSql;

using Xunit;

[assembly: AssemblyFixture(typeof(Murmur.Api.IntegrationTests.Infrastructure.PostgresFixture))]

namespace Murmur.Api.IntegrationTests.Infrastructure;

/// <summary>One Postgres container for the whole test run, started once and shared by every test.</summary>
public sealed class PostgresFixture : IAsyncLifetime
{
    // Same image the Aspire AppHost runs locally (Aspire.Hosting.PostgreSQL 13.6.4).
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:18.3").Build();

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask InitializeAsync() => await _container.StartAsync();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync();
}
