using System.Xml.Linq;

namespace Hosting.Catalyst.Tests;

public class FrameworkCoverageTests
{
    // R3 exists because lib/net8.0 shipped untested: the suites targeted net10.0 only. Four csproj files
    // were hand-edited to agree, and nothing guarded that agreement, so adding a framework to the package
    // would silently reopen the same blind spot. PublicSurfaceTests cannot catch it, because it only ever
    // compares the frameworks it is itself built for.
    [Theory]
    [InlineData("Hosting.Catalyst.Tests.csproj")]
    [InlineData("Hosting.Catalyst.IntegrationTests.csproj")]
    [InlineData("Hosting.Catalyst.SmokeTests.csproj")]
    public void EverySuiteCoversEveryFrameworkThePackageShips(string suite)
    {
        var shipped = TargetFrameworks("Hosting.Catalyst.csproj");
        var covered = TargetFrameworks(suite);

        Assert.NotEmpty(shipped);
        var missing = shipped.Except(covered, StringComparer.Ordinal).ToArray();
        Assert.True(
            missing.Length == 0,
            $"{suite} does not run on {string.Join(", ", missing)}, which the package ships in lib/. Add them to its TargetFrameworks.");
    }

    private static string[] TargetFrameworks(string fileName)
    {
        var csproj = XDocument.Load(Path.Combine(AppContext.BaseDirectory, "packaging", fileName));
        var value = csproj.Descendants("TargetFrameworks").Select(e => e.Value)
            .Concat(csproj.Descendants("TargetFramework").Select(e => e.Value))
            .First();
        return value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
