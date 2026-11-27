using System.Xml.Linq;

namespace Hosting.Diagrid.Tests;

public class PackagedArtifactTests
{
    // Directory.Build.props gates IncludeSymbols, SymbolPackageFormat and GenerateDocumentationFile on
    // the packable project's NAME. Rename that project without updating the condition and the build gets
    // greener, not redder: pack still succeeds and silently ships a package with no .snupkg and no XML
    // documentation. Nothing else in the suite notices, so this test is the guard.
    [Fact]
    public void PackagingPropertiesSurviveTheProjectName()
    {
        var packable = PackableProjectPath();
        var projectName = Path.GetFileNameWithoutExtension(packable);

        var props = XDocument.Load(Path.Combine(Path.GetDirectoryName(packable)!, "Directory.Build.props"));
        var gated = props.Descendants("PropertyGroup").Single(g => g.Descendants("IncludeSymbols").Any());
        var condition = (string?)gated.Attribute("Condition") ?? string.Empty;

        Assert.Contains($"== '{projectName}'", condition, StringComparison.Ordinal);
        Assert.Contains("snupkg", gated.Descendants("SymbolPackageFormat").Single().Value, StringComparison.Ordinal);
        Assert.Equal("true", gated.Descendants("GenerateDocumentationFile").Single().Value);
    }

    // Both URLs are packed into the nuspec, and a published version can never be re-pushed, so an owner
    // that has gone stale is permanent on nuget.org. The repository moved from diagrid-labs to diagridio
    // on 2026-11-27 and these two lines did not move with it. Pinned so the next move fails here, before
    // a release, rather than shipping a package whose links depend on a GitHub redirect that disappears
    // the moment anyone recreates the old name.
    [Fact]
    public void ThePackageUrlsNameTheRepositoryThatHostsIt()
    {
        const string repository = "https://github.com/diagridio/diagrid-aspire";

        var packable = XDocument.Load(PackableProjectPath());

        Assert.Equal(repository, packable.Descendants("RepositoryUrl").Single().Value);
        Assert.Equal(repository, packable.Descendants("PackageProjectUrl").Single().Value);
    }

    // Renaming the packable project leaves the old copy behind in bin/packaging next to the new one,
    // because MSBuild does not clean renamed Content. If this throws on "more than one match" after a
    // rename, the build output is stale: delete bin and obj. CI always builds clean.
    private static string PackableProjectPath() =>
        Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "packaging"), "*.csproj")
            .Single(f => XDocument.Load(f).Descendants("PackageId").Any());
}
