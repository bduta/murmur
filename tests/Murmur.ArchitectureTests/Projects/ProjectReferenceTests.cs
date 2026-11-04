using Shouldly;

using Xunit;

namespace Murmur.ArchitectureTests.Projects;

/// <summary>ADR-0029 §7 rules 1–5 against the repository's real project files.</summary>
public sealed class ProjectReferenceTests
{
    private static readonly IReadOnlyList<ProjectFile> Projects = Repository.LoadProjects();

    [Fact]
    public void The_loader_finds_the_projects_the_rules_are_about()
    {
        // Guards against every rule passing because the loader found nothing.
        Projects.ShouldContain(project => project.Kind == ProjectKind.SharedKernel);
        Projects.ShouldContain(project => project.Kind == ProjectKind.BuildingBlocks);
        Projects.ShouldContain(project => project.Kind == ProjectKind.Host);
        Projects.ShouldContain(project => project.Kind == ProjectKind.ModuleContracts);
        Projects.ShouldContain(project => project.Kind == ProjectKind.ModuleImplementation);
    }

    [Fact]
    public void Shared_build_files_add_no_references() =>
        Repository.ReferencesInSharedBuildFiles().ShouldBeEmpty();

    [Fact]
    public void Module_projects_follow_the_folder_layout() =>
        ProjectRules.ModuleProjectsFollowLayout(Projects).ShouldBeEmpty();

    [Fact]
    public void SharedKernel_references_base_dotnet_only() =>
        ProjectRules.SharedKernelReferencesBaseDotNetOnly(Projects).ShouldBeEmpty();

    [Fact]
    public void Contracts_reference_only_SharedKernel() =>
        ProjectRules.ContractsReferenceOnlySharedKernel(Projects).ShouldBeEmpty();

    [Fact]
    public void BuildingBlocks_references_no_module() =>
        ProjectRules.BuildingBlocksReferencesNoModule(Projects).ShouldBeEmpty();

    [Fact]
    public void Module_implementations_are_referenced_only_by_the_host_and_tests() =>
        ProjectRules.ModuleImplementationsReferencedOnlyByHostAndTests(Projects).ShouldBeEmpty();

    [Fact]
    public void Modules_have_no_cycles() =>
        ProjectRules.ModulesHaveNoCycles(Projects).ShouldBeEmpty();
}
