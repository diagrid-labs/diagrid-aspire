using System;

namespace Diagrid.Aspire.Hosting.Model;

public record ProjectDetails
{
    public required Uri HttpEndpoint { get; init; }

    public required Uri GrpcEndpoint { get; init; }
}
