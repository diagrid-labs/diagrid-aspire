namespace Hosting.Catalyst.Tests;

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
        var artifactPath = lines.Single(l => l.TrimStart().StartsWith("path: ./Hosting.Catalyst/nupkg/", StringComparison.Ordinal));
        Assert.Contains("*.*nupkg", artifactPath, StringComparison.Ordinal);
    }

    // The workflows resolve the SDK from global.json, which installs 10.0.100 and nothing else. A
    // framework-dependent net8.0 or net9.0 test host needs its own runtime (RollForward defaults to Minor
    // and does not cross majors), so without these the suites run only on whatever the hosted image happens
    // to preinstall. release.yaml gates packing on those suites, so a missing runtime blocks a release.
    [Theory]
    [InlineData("ci.yaml")]
    [InlineData("release.yaml")]
    public void WorkflowsInstallEveryRuntimeTheSuitesNeed(string workflow)
    {
        // Read whole: unlike the artifact-path assertion below, none of these needles span a line, so there
        // is nothing for a CRLF checkout to break.
        var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "packaging", workflow));

        Assert.Contains("8.0.x", text, StringComparison.Ordinal);
        Assert.Contains("9.0.x", text, StringComparison.Ordinal);
        Assert.Contains("global-json-file: global.json", text, StringComparison.Ordinal);
    }
}
