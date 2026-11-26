namespace Diagrid.Aspire.Hosting.Cli.Options;

public record CreateComponentOptions
{
    public required string Project { get; init; }
    public bool Wait { get; init; } = true;
    public bool IgnoreIfExists { get; init; } = true;
}
