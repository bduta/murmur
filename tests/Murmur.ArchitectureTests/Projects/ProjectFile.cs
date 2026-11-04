namespace Murmur.ArchitectureTests.Projects;

/// <summary>
/// What one .csproj declares. Paths are relative to the repository root and use '/' separators.
/// </summary>
internal sealed record ProjectFile(
    string Path,
    string Sdk,
    IReadOnlyList<string> ProjectReferences,
    IReadOnlyList<PackageReference> PackageReferences,
    IReadOnlyList<string> FrameworkReferences,
    IReadOnlyList<string> AssemblyReferences)
{
    public string Name => ProjectPaths.NameOf(Path);

    public ProjectKind Kind => ProjectPaths.KindOf(Path);

    public string? Module => ProjectPaths.ModuleOf(Path);
}

/// <param name="PrivateAssetsAll">True for compile-time-only packages (analyzers, source generators).</param>
internal sealed record PackageReference(string Name, bool PrivateAssetsAll);

internal enum ProjectKind
{
    SharedKernel,
    BuildingBlocks,
    ModuleContracts,
    ModuleImplementation,
    Host,
    Test,
    Other,
}

internal static class ProjectPaths
{
    public const string SharedKernel = "src/Shared/Murmur.SharedKernel/Murmur.SharedKernel.csproj";
    public const string BuildingBlocks = "src/Shared/Murmur.BuildingBlocks/Murmur.BuildingBlocks.csproj";
    public const string Host = "src/Murmur.Api/Murmur.Api.csproj";
    public const string ModulesFolder = "src/Modules/";

    public static string NameOf(string path) => System.IO.Path.GetFileNameWithoutExtension(path);

    /// <summary>The module folder name ("Posts") for any project under <c>src/Modules/</c>; otherwise null.</summary>
    public static string? ModuleOf(string path) =>
        path.StartsWith(ModulesFolder, StringComparison.Ordinal) ? path.Split('/')[2] : null;

    public static ProjectKind KindOf(string path) => path switch
    {
        SharedKernel => ProjectKind.SharedKernel,
        BuildingBlocks => ProjectKind.BuildingBlocks,
        Host => ProjectKind.Host,
        _ when path.StartsWith("tests/", StringComparison.Ordinal) => ProjectKind.Test,
        _ when ModuleOf(path) is not null => NameOf(path).EndsWith(".Contracts", StringComparison.Ordinal)
            ? ProjectKind.ModuleContracts
            : ProjectKind.ModuleImplementation,
        _ => ProjectKind.Other,
    };
}
