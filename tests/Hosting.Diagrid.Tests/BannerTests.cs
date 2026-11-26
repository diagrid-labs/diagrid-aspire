namespace Hosting.Diagrid.Tests;

public class BannerTests
{
    // The banner is printed at AppHost boot and nothing else covers it. The failure this guards is silent
    // in the build and loud at run time: the resource name is derived from the root namespace, so if the
    // two fall out of step the lookup finds nothing and the handler throws while starting the AppHost.
    [Fact]
    public void TheBannerResourceIsFoundAndSaysDiagrid()
    {
        var banner = global::Diagrid.Aspire.Hosting.Logo.LogoPicker.PickLogo();

        Assert.NotEqual("D i a g r i d", banner);
        Assert.DoesNotContain("Catalyst", banner, StringComparison.OrdinalIgnoreCase);
    }
}
