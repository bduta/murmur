using System.Reflection;

using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Dependencies;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnitV3;

using Murmur.ArchitectureTests.Projects;
using Murmur.BuildingBlocks.Modules;

using Shouldly;

using Xunit;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

using Assembly = System.Reflection.Assembly;

namespace Murmur.ArchitectureTests.Code;

/// <summary>ADR-0029 §7 rules 6 (visibility) and 7 (clock), checked on compiled assemblies.</summary>
public sealed class CodeRulesTests
{
    private static readonly IReadOnlyList<ProjectFile> Projects = Repository.LoadProjects();

    /// <summary>Assemblies the clock ban covers (ADR-0028, amended): everything in src/ except ServiceDefaults and AppHost.</summary>
    private static readonly ProjectKind[] ClockBanKinds =
    [
        ProjectKind.SharedKernel, ProjectKind.BuildingBlocks, ProjectKind.ModuleContracts,
        ProjectKind.ModuleImplementation, ProjectKind.Host,
    ];

    private static readonly string[] BannedClockProperties =
        ["System.DateTime.get_Now", "System.DateTime.get_UtcNow", "System.DateTime.get_Today",
         "System.DateTimeOffset.get_Now", "System.DateTimeOffset.get_UtcNow"];

    /// <summary>EF Core generates these as public; the one exemption ADR-0029 §6 allows.</summary>
    private static readonly string[] EfGeneratedBaseClasses =
        ["Microsoft.EntityFrameworkCore.Migrations.Migration", "Microsoft.EntityFrameworkCore.Infrastructure.ModelSnapshot"];

    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies([.. Projects.Select(project => TryLoad(project.Name)).OfType<Assembly>()])
        .Build();

    [Fact]
    public void Every_checked_project_is_loadable()
    {
        // A module the host doesn't reference never reaches this test's output folder and would silently skip the rules.
        var missing = Projects
            .Where(project => ClockBanKinds.Contains(project.Kind))
            .Where(project => TryLoad(project.Name) is null)
            .Select(project => $"{project.Name}: assembly not found; reference it from Murmur.Api");

        missing.ShouldBeEmpty();
    }

    [Fact]
    public void Only_the_IModule_class_is_public_in_module_implementations()
    {
        var rule = Types().That().ResideInAssembly(First(ModuleImplementations), [.. ModuleImplementations.Skip(1)])
            .And().ArePublic()
            .Should().FollowCustomCondition(
                type => type.ImplementsInterface(typeof(IModule).FullName!)
                    || EfGeneratedBaseClasses.Any(type.IsAssignableTo),
                "be the module's IModule class or an EF Core migration/model snapshot",
                "is public; make it internal (ADR-0029 §6)");

        rule.Check(Architecture);
    }

    [Fact]
    public void Code_reads_the_time_from_TimeProvider_only()
    {
        var assemblies = Assemblies(ClockBanKinds);
        var rule = Types().That().ResideInAssembly(First(assemblies), [.. assemblies.Skip(1)])
            .Should().FollowCustomCondition(
                type =>
                {
                    string[] calls =
                    [
                        .. type.Dependencies.OfType<MethodCallDependency>()
                            .Select(call => $"{call.Target.FullName}.{call.TargetMember.Name.Split('(')[0]}")
                            .Where(BannedClockProperties.Contains)
                            .Distinct(),
                    ];

                    return new ConditionResult(
                        type,
                        calls.Length == 0,
                        $"reads {string.Join(", ", calls)}; inject TimeProvider and call GetUtcNow() (ADR-0028)");
                },
                "not read the system clock directly");

        rule.Check(Architecture);
    }

    private static Assembly[] ModuleImplementations => Assemblies(ProjectKind.ModuleImplementation);

    private static Assembly[] Assemblies(params ProjectKind[] kinds) =>
    [
        .. Projects.Where(project => kinds.Contains(project.Kind)).Select(project => TryLoad(project.Name)).OfType<Assembly>(),
    ];

    private static Assembly First(Assembly[] assemblies) =>
        assemblies.Length > 0 ? assemblies[0] : throw new InvalidOperationException("No assemblies to check.");

    private static Assembly? TryLoad(string name)
    {
        try
        {
            return Assembly.Load(new AssemblyName(name));
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }
}
