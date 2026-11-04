using Shouldly;

using Xunit;

namespace Murmur.ArchitectureTests.Projects;

/// <summary>
/// Proves each project-reference rule can fail, using hand-built project graphs instead of real projects.
/// <see cref="ProjectReferenceTests"/> runs the same rules against the repository.
/// </summary>
public sealed class ProjectRulesTests
{
    private const string Kernel = "src/Shared/Murmur.SharedKernel/Murmur.SharedKernel.csproj";
    private const string BuildingBlocks = "src/Shared/Murmur.BuildingBlocks/Murmur.BuildingBlocks.csproj";
    private const string Api = "src/Murmur.Api/Murmur.Api.csproj";
    private const string IdentityContracts = "src/Modules/Identity/Murmur.Identity.Contracts/Murmur.Identity.Contracts.csproj";
    private const string Identity = "src/Modules/Identity/Murmur.Identity/Murmur.Identity.csproj";
    private const string PostsContracts = "src/Modules/Posts/Murmur.Posts.Contracts/Murmur.Posts.Contracts.csproj";
    private const string Posts = "src/Modules/Posts/Murmur.Posts/Murmur.Posts.csproj";
    private const string FeedContracts = "src/Modules/Feed/Murmur.Feed.Contracts/Murmur.Feed.Contracts.csproj";
    private const string Feed = "src/Modules/Feed/Murmur.Feed/Murmur.Feed.csproj";
    private const string Tests = "tests/Murmur.Api.IntegrationTests/Murmur.Api.IntegrationTests.csproj";

    [Fact]
    public void A_graph_that_follows_ADR_0029_has_no_violations()
    {
        IReadOnlyList<ProjectFile> projects = ValidGraph();

        AllViolations(projects).ShouldBeEmpty();
    }

    [Fact]
    public void SharedKernel_may_not_reference_another_project()
    {
        var projects = ValidGraph(Project(Kernel, references: [BuildingBlocks]));

        ProjectRules.SharedKernelReferencesBaseDotNetOnly(projects)
            .ShouldHaveSingleItem().ShouldContain("Murmur.BuildingBlocks");
    }

    [Fact]
    public void SharedKernel_may_not_reference_a_framework()
    {
        var projects = ValidGraph(Project(Kernel, frameworks: ["Microsoft.AspNetCore.App"]));

        ProjectRules.SharedKernelReferencesBaseDotNetOnly(projects)
            .ShouldHaveSingleItem().ShouldContain("Microsoft.AspNetCore.App");
    }

    [Fact]
    public void SharedKernel_may_not_use_an_SDK_that_brings_a_framework()
    {
        var projects = ValidGraph(Project(Kernel, sdk: "Microsoft.NET.Sdk.Web"));

        ProjectRules.SharedKernelReferencesBaseDotNetOnly(projects)
            .ShouldHaveSingleItem().ShouldContain("Microsoft.NET.Sdk.Web");
    }

    [Fact]
    public void SharedKernel_may_not_reference_a_runtime_package_or_assembly()
    {
        var projects = ValidGraph(Project(
            Kernel,
            packages: [new PackageReference("Newtonsoft.Json", PrivateAssetsAll: false)],
            assemblies: ["Some.Library"]));

        ProjectRules.SharedKernelReferencesBaseDotNetOnly(projects).Count.ShouldBe(2);
    }

    [Fact]
    public void SharedKernel_may_reference_a_compile_time_only_package()
    {
        var projects = ValidGraph(Project(
            Kernel,
            packages: [new PackageReference("Some.SourceGenerator", PrivateAssetsAll: true)]));

        ProjectRules.SharedKernelReferencesBaseDotNetOnly(projects).ShouldBeEmpty();
    }

    [Theory]
    [InlineData(BuildingBlocks)]
    [InlineData(IdentityContracts)]
    [InlineData(Identity)]
    public void Contracts_may_reference_only_SharedKernel(string reference)
    {
        var projects = ValidGraph(Project(PostsContracts, references: [Kernel, reference]));

        ProjectRules.ContractsReferenceOnlySharedKernel(projects)
            .ShouldHaveSingleItem().ShouldContain(Path.GetFileNameWithoutExtension(reference));
    }

    [Fact]
    public void Contracts_may_not_reference_a_framework_or_runtime_package()
    {
        var projects = ValidGraph(Project(
            PostsContracts,
            references: [Kernel],
            frameworks: ["Microsoft.AspNetCore.App"],
            packages: [new PackageReference("Microsoft.EntityFrameworkCore", PrivateAssetsAll: false)]));

        ProjectRules.ContractsReferenceOnlySharedKernel(projects).Count.ShouldBe(2);
    }

    [Theory]
    [InlineData(PostsContracts)]
    [InlineData(Posts)]
    public void BuildingBlocks_may_not_reference_a_module(string reference)
    {
        var projects = ValidGraph(Project(BuildingBlocks, references: [Kernel, reference]));

        ProjectRules.BuildingBlocksReferencesNoModule(projects)
            .ShouldHaveSingleItem().ShouldContain(Path.GetFileNameWithoutExtension(reference));
    }

    [Fact]
    public void A_module_implementation_may_not_be_referenced_by_another_module()
    {
        var projects = ValidGraph(Project(Feed, references: [Kernel, BuildingBlocks, FeedContracts, Posts]));

        ProjectRules.ModuleImplementationsReferencedOnlyByHostAndTests(projects)
            .ShouldHaveSingleItem().ShouldContain("Murmur.Feed");
    }

    [Fact]
    public void Test_projects_may_reference_module_implementations()
    {
        var projects = ValidGraph(Project(Tests, references: [Api, Posts]));

        ProjectRules.ModuleImplementationsReferencedOnlyByHostAndTests(projects).ShouldBeEmpty();
    }

    [Fact]
    public void Two_modules_may_not_depend_on_each_other()
    {
        var projects = ValidGraph(Project(Posts, references: [Kernel, BuildingBlocks, PostsContracts, FeedContracts]));

        ProjectRules.ModulesHaveNoCycles(projects)
            .ShouldHaveSingleItem().ShouldContain("Feed → Posts → Feed");
    }

    [Fact]
    public void A_cycle_through_three_modules_is_reported()
    {
        // ValidGraph: Feed → Posts → Identity. Closing the loop: Identity → Feed.
        var projects = ValidGraph(Project(Identity, references: [Kernel, BuildingBlocks, IdentityContracts, FeedContracts]));

        ProjectRules.ModulesHaveNoCycles(projects)
            .ShouldHaveSingleItem().ShouldContain("Feed → Posts → Identity → Feed");
    }

    [Theory]
    [InlineData("src/Modules/Posts/Murmur.Posts/Internal/Murmur.Posts.Internal.csproj")]
    [InlineData("src/Modules/Posts/Murmur.Timeline/Murmur.Timeline.csproj")]
    [InlineData("src/Modules/Posts/Murmur.Posts.Contracts/Murmur.Posts.Contract.csproj")]
    public void Module_projects_must_follow_the_folder_layout(string path)
    {
        var projects = ValidGraph(Project(path));

        ProjectRules.ModuleProjectsFollowLayout(projects)
            .ShouldHaveSingleItem().ShouldContain(path);
    }

    private static IEnumerable<string> AllViolations(IReadOnlyList<ProjectFile> projects) =>
    [
        .. ProjectRules.ModuleProjectsFollowLayout(projects),
        .. ProjectRules.SharedKernelReferencesBaseDotNetOnly(projects),
        .. ProjectRules.ContractsReferenceOnlySharedKernel(projects),
        .. ProjectRules.BuildingBlocksReferencesNoModule(projects),
        .. ProjectRules.ModuleImplementationsReferencedOnlyByHostAndTests(projects),
        .. ProjectRules.ModulesHaveNoCycles(projects),
    ];

    /// <summary>
    /// Three modules layered Identity ← Posts ← Feed, as ADR-0029 §5 expects.
    /// <paramref name="replacements"/> swap in projects (matched by path) or add new ones.
    /// </summary>
    private static List<ProjectFile> ValidGraph(params ProjectFile[] replacements)
    {
        List<ProjectFile> projects =
        [
            Project(Kernel),
            Project(BuildingBlocks, references: [Kernel], frameworks: ["Microsoft.AspNetCore.App"]),
            Project(IdentityContracts, references: [Kernel]),
            Project(Identity, references: [Kernel, BuildingBlocks, IdentityContracts]),
            Project(PostsContracts, references: [Kernel]),
            Project(Posts, references: [Kernel, BuildingBlocks, PostsContracts, IdentityContracts]),
            Project(FeedContracts, references: [Kernel]),
            Project(Feed, references: [Kernel, BuildingBlocks, FeedContracts, PostsContracts]),
            Project(Api, sdk: "Microsoft.NET.Sdk.Web", references: [Kernel, BuildingBlocks, Identity, Posts, Feed]),
            Project(Tests, references: [Api]),
        ];

        foreach (var replacement in replacements)
        {
            projects.RemoveAll(project => project.Path == replacement.Path);
            projects.Add(replacement);
        }

        return projects;
    }

    private static ProjectFile Project(
        string path,
        string sdk = "Microsoft.NET.Sdk",
        string[]? references = null,
        PackageReference[]? packages = null,
        string[]? frameworks = null,
        string[]? assemblies = null) =>
        new(path, sdk, references ?? [], packages ?? [], frameworks ?? [], assemblies ?? []);
}
