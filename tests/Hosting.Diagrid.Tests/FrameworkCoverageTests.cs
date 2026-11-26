using System.Xml.Linq;

namespace Hosting.Diagrid.Tests;

public class FrameworkCoverageTests
{
    // R3 exists because lib/net8.0 shipped untested: the suites targeted net10.0 only. Four csproj files
    // were hand-edited to agree, and nothing guarded that agreement, so adding a framework to the package
    // would silently reopen the same blind spot. PublicSurfaceTests cannot catch it, because it only ever
    // compares the frameworks it is itself built for.
    [Theory]
    [InlineData("Hosting.Diagrid.Tests.csproj")]
    [InlineData("Hosting.Diagrid.IntegrationTests.csproj")]
    [InlineData("Hosting.Diagrid.SmokeTests.csproj")]
    public void EverySuiteCoversEveryFrameworkThePackageShips(string suite)
    {
        var shipped = TargetFrameworks("Hosting.Diagrid.csproj");
        var covered = TargetFrameworks(suite);

        Assert.NotEmpty(shipped);
        var missing = shipped.Except(covered, StringComparer.Ordinal).ToArray();
        Assert.True(
            missing.Length == 0,
            $"{suite} does not run on {string.Join(", ", missing)}, which the package ships in lib/. Add them to its TargetFrameworks.");
    }

    // global.json installs the SDK, and with it one runtime. A suite that targets anything below that
    // band needs its own runtime installed in the workflow, because RollForward defaults to Minor and does
    // not cross majors. Without it the suite runs only on what the hosted image happens to preinstall, and
    // release.yaml gates packing on the suites, so a thin image blocks a release rather than warning.
    [Theory]
    [InlineData("ci.yaml")]
    [InlineData("release.yaml")]
    public void WorkflowsInstallARuntimeForEveryFrameworkBelowTheSdkBand(string workflow)
    {
        var sdkBand = SdkMajor();
        var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "packaging", workflow));

        var needed = new[] { "Hosting.Diagrid.Tests.csproj", "Hosting.Diagrid.IntegrationTests.csproj", "Hosting.Diagrid.SmokeTests.csproj" }
            .SelectMany(TargetFrameworks)
            .Select(tfm => tfm.Replace("net", string.Empty))
            .Where(version => !version.StartsWith(sdkBand + ".", StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        Assert.Contains("global-json-file: global.json", text, StringComparison.Ordinal);
        foreach (var version in needed)
        {
            Assert.True(
                text.Contains(version.Split('.')[0] + ".0.x", StringComparison.Ordinal),
                $"A suite targets net{version}, which the SDK in global.json does not carry a runtime for, and {workflow} does not install it. Add it to that step's dotnet-version.");
        }
    }

    private static string SdkMajor()
    {
        var globalJson = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "packaging", "global.json"));
        var marker = globalJson.IndexOf("\"version\"", StringComparison.Ordinal);
        var value = globalJson[(globalJson.IndexOf('"', globalJson.IndexOf(':', marker)) + 1)..];
        return value[..value.IndexOf('.', StringComparison.Ordinal)];
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
