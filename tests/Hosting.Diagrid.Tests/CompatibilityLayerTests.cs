using System.Reflection;
using Diagrid.Aspire.Hosting;

namespace Hosting.Diagrid.Tests;

// The renamed package ships the old names for one minor release. public-surface.txt renders names only,
// so it cannot tell whether a forwarder is actually marked obsolete or whether its message is useful.
#pragma warning disable CS0618 // The point of these tests is the obsolete surface.
public class CompatibilityLayerTests
{
    [Fact]
    public void ObsoleteForwardersCarryAMessageNamingTheirReplacement()
    {
        var forwarders = typeof(global::Diagrid.Aspire.Hosting.ResourceBuilderExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name.Contains("Catalyst", StringComparison.Ordinal))
            .ToArray();

        Assert.Equal(8, forwarders.Length);
        Assert.All(forwarders, m =>
        {
            var obsolete = m.GetCustomAttribute<ObsoleteAttribute>();
            Assert.NotNull(obsolete);
            Assert.Contains("Diagrid", obsolete!.Message, StringComparison.Ordinal);
            Assert.False(obsolete.IsError);
        });
    }

    [Fact]
    public void TheOldExceptionStillCatchesWhatTheLibraryThrows()
    {
        static void Throw() => throw new global::Diagrid.Aspire.Hosting.DiagridCliException(["x"], 1, string.Empty, "boom");

        var caught = Record.Exception(Throw);

        Assert.IsType<global::Diagrid.Aspire.Hosting.DiagridCliException>(caught);
        Assert.IsAssignableFrom<global::Diagrid.Aspire.Hosting.CatalystCliException>(caught);
        Assert.Equal(1, ((global::Diagrid.Aspire.Hosting.CatalystCliException)caught!).ExitCode);
    }

    [Fact]
    public void TheOldProvisionerInterfaceStillSatisfiesTheNewOne()
        => Assert.True(typeof(global::Diagrid.Aspire.Hosting.IDiagridProvisioner)
            .IsAssignableFrom(typeof(global::Diagrid.Aspire.Hosting.ICatalystProvisioner)));
}
#pragma warning restore CS0618
