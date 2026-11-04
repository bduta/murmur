using System.Xml.Linq;

namespace Murmur.ArchitectureTests.Projects;

/// <summary>Reads the repository's project files from disk.</summary>
internal static class Repository
{
    public static string Root { get; } = FindRoot();

    /// <summary>Every .csproj under src/ and tests/, so a new module is checked without editing the tests.</summary>
    public static IReadOnlyList<ProjectFile> LoadProjects() =>
    [
        .. new[] { "src", "tests" }
            .SelectMany(folder => Directory.EnumerateFiles(Path.Combine(Root, folder), "*.csproj", SearchOption.AllDirectories))
            .Where(file => !IsBuildOutput(file))
            .Select(Parse)
            .OrderBy(project => project.Path, StringComparer.Ordinal),
    ];

    /// <summary>
    /// Directory.Build.* and Directory.Packages.props apply to every project below them. A reference added there
    /// wouldn't appear in any .csproj and would slip past <see cref="ProjectRules"/>, so they may add none.
    /// </summary>
    public static IReadOnlyList<string> ReferencesInSharedBuildFiles()
    {
        string[] sharedFileNames = ["Directory.Build.props", "Directory.Build.targets", "Directory.Packages.props"];
        string[] referenceItems = ["ProjectReference", "PackageReference", "GlobalPackageReference", "FrameworkReference", "Reference"];

        return
        [
            .. from file in Directory.EnumerateFiles(Root, "Directory.*", SearchOption.AllDirectories)
               where sharedFileNames.Contains(Path.GetFileName(file)) && !IsBuildOutput(file)
               from element in XDocument.Load(file).Descendants()
               where referenceItems.Contains(element.Name.LocalName)
               select $"{RelativePath(file)}: adds {element.Name.LocalName} {(string?)element.Attribute("Include")} to every project below it",
        ];
    }

    private static ProjectFile Parse(string file)
    {
        var document = XDocument.Load(file);
        var directory = Path.GetDirectoryName(file)!;

        IEnumerable<XElement> Items(string name) =>
            document.Descendants(name).Where(item => item.Attribute("Include") is not null);

        static string Include(XElement item) => (string)item.Attribute("Include")!;

        return new ProjectFile(
            RelativePath(file),
            (string?)document.Root?.Attribute("Sdk") ?? "",
            [.. Items("ProjectReference").Select(item => RelativePath(Path.Combine(directory, Include(item).Replace('\\', '/'))))],
            [.. Items("PackageReference").Select(item => new PackageReference(Include(item), IsPrivateAssetsAll(item)))],
            [.. Items("FrameworkReference").Select(Include)],
            [.. Items("Reference").Select(Include)]);
    }

    private static bool IsPrivateAssetsAll(XElement packageReference) =>
        string.Equals(
            ((string?)packageReference.Attribute("PrivateAssets") ?? (string?)packageReference.Element("PrivateAssets"))?.Trim(),
            "all",
            StringComparison.OrdinalIgnoreCase);

    private static string RelativePath(string path) =>
        Path.GetRelativePath(Root, Path.GetFullPath(path)).Replace('\\', '/');

    private static bool IsBuildOutput(string path) =>
        RelativePath(path).Split('/') is var segments
        && (segments.Contains("bin") || segments.Contains("obj") || segments.Contains("node_modules"));

    private static string FindRoot()
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
