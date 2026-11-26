namespace Hosting.Diagrid.Tests;

public class PackagedVersionTests
{
    // Two contracts in release.yaml that a later edit could break without anything noticing: the packed
    // version comes from the release tag, and the artifact upload carries the symbols package as well as
    // the package. Asserted line by line rather than against the whole file, because this suite also runs
    // on windows-latest and a literal \n would not match a CRLF checkout.
    [Fact]
    public void ReleaseWorkflowPacksTheResolvedVersionAndUploadsSymbols()
    {
        var lines = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "packaging", "release.yaml"));

        Assert.Contains(lines, l => l.Contains("/p:PackageVersion=${{ steps.choose_version.outputs.version }}", StringComparison.Ordinal));

        // '*.nupkg' does not glob '.snupkg', so an artifact path of '*.nupkg' silently drops the symbols package.
        var artifactPath = lines.Single(l => l.TrimStart().StartsWith("path: ./Hosting.Diagrid/nupkg/", StringComparison.Ordinal));
        Assert.Contains("*.*nupkg", artifactPath, StringComparison.Ordinal);
    }
}
