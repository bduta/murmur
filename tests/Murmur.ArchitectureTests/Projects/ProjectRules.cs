namespace Murmur.ArchitectureTests.Projects;

/// <summary>
/// ADR-0029 §7 rules 1–5, checked on declared project references. Each returns one message per violation.
/// Pure functions over <see cref="ProjectFile"/>s, so <see cref="ProjectRulesTests"/> can prove each can fail.
/// </summary>
internal static class ProjectRules
{
    private const string PlainSdk = "Microsoft.NET.Sdk";

    /// <summary>
    /// Every project under src/Modules is <c>src/Modules/&lt;Name&gt;/Murmur.&lt;Name&gt;[.Contracts]/…csproj</c>.
    /// Without this, a misplaced project would escape the rules below.
    /// </summary>
    public static IReadOnlyList<string> ModuleProjectsFollowLayout(IReadOnlyList<ProjectFile> projects) =>
    [
        .. from project in projects
           let module = project.Module
           where module is not null
              && project.Path !=$"src/Modules/{module}/Murmur.{module}/Murmur.{module}.csproj"
              && project.Path != $"src/Modules/{module}/Murmur.{module}.Contracts/Murmur.{module}.Contracts.csproj"
           select $"{project.Path}: module projects must be src/Modules/{module}/Murmur.{module}[.Contracts]/Murmur.{module}[.Contracts].csproj",
    ];

    /// <summary>Rule 1: SharedKernel references base .NET only.</summary>
    public static IReadOnlyList<string> SharedKernelReferencesBaseDotNetOnly(IReadOnlyList<ProjectFile> projects) =>
    [
        .. projects
            .Where(project => project.Kind == ProjectKind.SharedKernel)
            .SelectMany(project => BaseDotNetOnly(project, allowedProject: null)),
    ];

    /// <summary>Rule 2: .Contracts reference only SharedKernel — no BuildingBlocks, no framework types.</summary>
    public static IReadOnlyList<string> ContractsReferenceOnlySharedKernel(IReadOnlyList<ProjectFile> projects) =>
    [
        .. projects
            .Where(project => project.Kind == ProjectKind.ModuleContracts)
            .SelectMany(project => BaseDotNetOnly(project, allowedProject: ProjectKind.SharedKernel)),
    ];

    /// <summary>Rule 3: BuildingBlocks references no module.</summary>
    public static IReadOnlyList<string> BuildingBlocksReferencesNoModule(IReadOnlyList<ProjectFile> projects) =>
    [
        .. from project in projects
           where project.Kind == ProjectKind.BuildingBlocks
           from reference in project.ProjectReferences
           where ProjectPaths.ModuleOf(reference) is not null
           select $"{project.Name}: references module project {ProjectPaths.NameOf(reference)}",
    ];

    /// <summary>Rule 4: module implementations are referenced only by Murmur.Api and test projects.</summary>
    public static IReadOnlyList<string> ModuleImplementationsReferencedOnlyByHostAndTests(IReadOnlyList<ProjectFile> projects) =>
    [
        .. from project in projects
           where project.Kind is not (ProjectKind.Host or ProjectKind.Test)
           from reference in project.ProjectReferences
           where ProjectPaths.KindOf(reference) == ProjectKind.ModuleImplementation
           select $"{project.Name}: references module implementation {ProjectPaths.NameOf(reference)}; reference its .Contracts project instead",
    ];

    /// <summary>
    /// Rule 5: modules form a one-way graph. Module X depends on Y when any X project references any Y project.
    /// MSBuild only rejects cycles between projects; Posts → Feed.Contracts plus Feed → Posts.Contracts builds fine.
    /// </summary>
    public static IReadOnlyList<string> ModulesHaveNoCycles(IReadOnlyList<ProjectFile> projects)
    {
        Dictionary<string, List<string>> dependencies = projects
            .Where(project => project.Module is not null)
            .GroupBy(project => project.Module!)
            .ToDictionary(
                module => module.Key,
                module => module
                    .SelectMany(project => project.ProjectReferences)
                    .Select(ProjectPaths.ModuleOf)
                    .OfType<string>()
                    .Where(dependency => dependency != module.Key)
                    .Distinct()
                    .Order(StringComparer.Ordinal)
                    .ToList());

        List<string> cycles = [];
        HashSet<string> finished = [];
        List<string> path = [];

        foreach (var module in dependencies.Keys.Order(StringComparer.Ordinal))
        {
            Visit(module);
        }

        return cycles;

        void Visit(string module)
        {
            if (finished.Contains(module))
            {
                return;
            }

            var start = path.IndexOf(module);
            if (start >= 0)
            {
                cycles.Add($"module cycle: {string.Join(" → ", path[start..].Append(module))}; use an integration event for the reverse direction");
                return;
            }

            path.Add(module);
            foreach (var dependency in dependencies.GetValueOrDefault(module) ?? [])
            {
                Visit(dependency);
            }

            path.RemoveAt(path.Count - 1);
            finished.Add(module);
        }
    }

    private static IEnumerable<string> BaseDotNetOnly(ProjectFile project, ProjectKind? allowedProject)
    {
        if (project.Sdk != PlainSdk)
        {
            yield return $"{project.Name}: uses SDK '{project.Sdk}', which brings in a framework; use {PlainSdk}";
        }

        foreach (var reference in project.ProjectReferences.Where(reference => ProjectPaths.KindOf(reference) != allowedProject))
        {
            yield return $"{project.Name}: references project {ProjectPaths.NameOf(reference)}";
        }

        foreach (var framework in project.FrameworkReferences)
        {
            yield return $"{project.Name}: references framework {framework}";
        }

        foreach (var package in project.PackageReferences.Where(package => !package.PrivateAssetsAll))
        {
            yield return $"{project.Name}: references package {package.Name} at runtime (only PrivateAssets=\"all\" packages are allowed)";
        }

        foreach (var assembly in project.AssemblyReferences)
        {
            yield return $"{project.Name}: references assembly {assembly}";
        }
    }
}
