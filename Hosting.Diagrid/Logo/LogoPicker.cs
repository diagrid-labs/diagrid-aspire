using System;
using System.IO;
using System.Linq;

namespace Diagrid.Aspire.Hosting.Logo;

internal static class LogoPicker
{
    public static string PickLogo()
    {
        var assembly = typeof(LogoPicker).Assembly;

        // Derived from this type's own namespace rather than a hardcoded string, so renaming the root
        // namespace cannot leave the lookup pointing at a prefix that no longer exists. That failure is
        // invisible to the build and throws at AppHost boot.
        var prefix = typeof(LogoPicker).Namespace + ".";

        var logoResourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name =>
                name.StartsWith(prefix, StringComparison.Ordinal)
                && name.EndsWith(".txt", StringComparison.Ordinal));

        var logoStream = logoResourceName is not null
            ? assembly.GetManifestResourceStream(logoResourceName)
            : null;

        return logoStream is not null
            ? new StreamReader(logoStream).ReadToEnd()
            : "D i a g r i d";
    }
}
