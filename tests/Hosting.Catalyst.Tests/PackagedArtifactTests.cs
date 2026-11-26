using System.Xml.Linq;

namespace Hosting.Catalyst.Tests;

public class PackagedArtifactTests
{
    // Directory.Build.props gates IncludeSymbols, SymbolPackageFormat and GenerateDocumentationFile on
    // the packable project's NAME. Rename that project without updating the condition and the build gets
    // greener, not redder: pack still succeeds and silently ships a package with no .snupkg and no XML
    // documentation. Nothing else in the suite notices, so this test is the guard.
    [Fact]
    public void PackagingPropertiesSurviveTheProjectName()
    {
        var packagingDir = Path.Combine(AppContext.BaseDirectory, "packaging");
        var packable = Directory.GetFiles(packagingDir, "*.csproj")
            .Single(f => XDocument.Load(f).Descendants("PackageId").Any());
        var projectName = Path.GetFileNameWithoutExtension(packable);

        var props = XDocument.Load(Path.Combine(packagingDir, "Directory.Build.props"));
        var gated = props.Descendants("PropertyGroup").Single(g => g.Descendants("IncludeSymbols").Any());
        var condition = (string?)gated.Attribute("Condition") ?? string.Empty;

        Assert.Contains($"== '{projectName}'", condition, StringComparison.Ordinal);
        Assert.Contains("snupkg", gated.Descendants("SymbolPackageFormat").Single().Value, StringComparison.Ordinal);
        Assert.Equal("true", gated.Descendants("GenerateDocumentationFile").Single().Value);
    }
}
