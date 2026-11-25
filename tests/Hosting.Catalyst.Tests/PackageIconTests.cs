using System.Xml.Linq;

namespace Hosting.Catalyst.Tests;

public class PackageIconTests
{
    // A PackageIcon whose value does not match an archive entry packs cleanly and renders nothing on
    // nuget.org, which is how every published version ended up with a default icon. The csproj is read
    // as data from the test output, the way the fixtures are, so the assertion survives the output-path
    // differences between the three target frameworks.
    [Fact]
    public void PackageIconPathHasNoLeadingSlashAndIsPacked()
    {
        var csproj = XDocument.Load(Path.Combine(AppContext.BaseDirectory, "packaging", "Hosting.Catalyst.csproj"));
        var icon = csproj.Descendants("PackageIcon").Single().Value;

        Assert.False(icon.StartsWith('/'), "PackageIcon must not start with '/': nuget.org cannot resolve it and the package renders a default icon.");
        Assert.Equal("logo.png", icon);

        var packaged = csproj.Descendants("None").Concat(csproj.Descendants("Content"))
            .Where(e => (string?)e.Attribute("Pack") == "true")
            .Select(e => ((string?)e.Attribute("PackagePath") ?? string.Empty, (string?)e.Attribute("Include") ?? string.Empty));

        // Both halves matter: the archive entry path comes from PackagePath, so packing the right file to
        // "icons/" would make the nuspec's logo.png unresolvable again while an Include-only check stayed green.
        Assert.Contains(packaged, p => p.Item2.EndsWith(icon, StringComparison.Ordinal) && (p.Item1 == "/" || p.Item1.Length == 0));
    }
}
