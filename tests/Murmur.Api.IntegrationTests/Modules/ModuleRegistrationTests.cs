using Microsoft.Extensions.DependencyInjection;

using Murmur.Api.IntegrationTests.Infrastructure;
using Murmur.BuildingBlocks.Modules;

using Shouldly;

using Xunit;

namespace Murmur.Api.IntegrationTests.Modules;

public sealed class ModuleRegistrationTests
{
    /// <summary>
    /// Program.cs lists modules by hand (no assembly scanning). This catches a module project that was added
    /// on disk but never registered, and a registration left behind after a module was removed.
    /// </summary>
    [Fact]
    public async Task Registered_modules_match_the_module_projects_on_disk()
    {
        // Building the service provider never opens a connection, so no database is needed.
        await using var factory = new MurmurApiFactory(MurmurApiFactory.UnreachableDatabase);

        string[] registered =
        [
            .. factory.Services.GetRequiredService<IReadOnlyList<IModule>>()
                .Select(module => module.GetType().Assembly.GetName().Name!)
                .Order(StringComparer.Ordinal),
        ];

        registered.ShouldBe(ModuleProjectsOnDisk());
    }

    /// <summary>The implementation project of each module: src/Modules/{Name}/Murmur.{Name}.</summary>
    private static string[] ModuleProjectsOnDisk() =>
    [
        .. Directory.EnumerateDirectories(Path.Combine(FindRepositoryRoot(), "src", "Modules"))
            .Select(moduleFolder => $"Murmur.{Path.GetFileName(moduleFolder)}")
            .Order(StringComparer.Ordinal),
    ];

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Murmur.slnx")))
            {
                return directory.FullName;
            }
        }

        throw new InvalidOperationException($"Murmur.slnx not found in any folder above {AppContext.BaseDirectory}.");
    }
}
