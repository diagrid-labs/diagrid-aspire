namespace Hosting.Diagrid.Tests;

public class ReadmeTests
{
    // readme.md is the package's PackageReadmeFile, so it is the front page on nuget.org and is immutable
    // once a version ships. If it teaches the obsolete names, someone installing the package for the first
    // time writes them, gets CS0618 on day one, and stops compiling at the next minor.
    //
    // Only the live documentation is checked. Everything from "What is still called Catalyst" onward names
    // the old API on purpose: the standing note, the upgrade mapping, and the frozen release-note history.
    [Fact]
    public void TheLiveDocumentationDoesNotTeachTheObsoleteNames()
    {
        var readme = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "packaging", "readme.md"));
        var boundary = readme.IndexOf("## What is still called Catalyst", StringComparison.Ordinal);
        Assert.True(boundary > 0, "The readme no longer has the section that separates live docs from the upgrade notes.");

        var live = readme[..boundary];

        Assert.DoesNotContain("AddCatalystProject", live, StringComparison.Ordinal);
        Assert.DoesNotContain("WithCatalyst", live, StringComparison.Ordinal);
    }
}
