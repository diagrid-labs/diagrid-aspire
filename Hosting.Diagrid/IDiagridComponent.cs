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
