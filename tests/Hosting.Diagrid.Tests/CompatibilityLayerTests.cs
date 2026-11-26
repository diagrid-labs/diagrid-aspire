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

    // Source compatibility includes parameter names: a 1.0.0 AppHost may have called these with named
    // arguments. Renaming a forwarder's parameter turns the layer that exists to prevent a compile error
    // into one. Neither public-surface.txt nor the attribute test above can see parameter names.
    [Fact]
    public void ForwardersKeepTheParameterNamesTheOldApiHad()
    {
        var forwarders = typeof(global::Diagrid.Aspire.Hosting.ResourceBuilderExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name.Contains("Catalyst", StringComparison.Ordinal));

        foreach (var m in forwarders)
        {
            Assert.All(m.GetParameters(), p => Assert.False(
                p.Name!.StartsWith("diagrid", StringComparison.Ordinal),
                $"{m.Name} has a parameter named {p.Name}; forwarders must keep the 1.0.0 names so named arguments still compile."));
        }
    }

    [Fact]
    public void TheOldComponentInterfacesStillSatisfyTheNewOnes()
    {
        Assert.True(typeof(global::Diagrid.Aspire.Hosting.IDiagridComponent)
            .IsAssignableFrom(typeof(global::Diagrid.Aspire.Hosting.ICatalystComponent)));
        Assert.True(typeof(global::Diagrid.Aspire.Hosting.IDiagridComponent<>)
            .IsAssignableFrom(typeof(global::Diagrid.Aspire.Hosting.ICatalystComponent<>).GetInterfaces()[0].GetGenericTypeDefinition()));
    }
}
#pragma warning restore CS0618
