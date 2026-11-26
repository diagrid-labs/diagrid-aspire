using System;
using System.Collections.Generic;

namespace Diagrid.Aspire.Hosting;

public interface IDiagridComponent
{
}

public interface IDiagridComponent<TMetadata> : IDiagridComponent
{
    public string Type { get; }

    public IList<string> Scopes { get; }

    public TMetadata Metadata { get; }
}

/// <summary>Renamed to <see cref="IDiagridComponent"/>. Kept for one minor release so an existing
/// implementation still compiles and still satisfies the new interface.</summary>
[Obsolete("Renamed to IDiagridComponent. Catalyst is now Diagrid; this type is removed in the next minor.")]
public interface ICatalystComponent : IDiagridComponent
{
}

/// <summary>Renamed to <see cref="IDiagridComponent{TMetadata}"/>. Kept for one minor release.</summary>
[Obsolete("Renamed to IDiagridComponent<TMetadata>. Catalyst is now Diagrid; this type is removed in the next minor.")]
public interface ICatalystComponent<TMetadata> : IDiagridComponent<TMetadata>
{
}
