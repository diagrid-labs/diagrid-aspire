using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Diagrid.Aspire.Hosting;
using Diagrid.Aspire.Hosting.Model;

namespace Hosting.Diagrid.Tests;

public class AgentTests
{
    // Mutation: WithDiagridAgent passes IdentityKind.App -> fails.
    [Fact]
    public void Registers_the_resource_as_an_agent_identity()
    {
        var project = Build(b => b.AddExecutable("my-agent", "dotnet", ".").WithDiagridAgent());

        var identity = Assert.Single(project.Identities);
        Assert.Equal("my-agent", identity.Key.Name);
        Assert.Equal(IdentityKind.Agent, identity.Value.Kind);
    }

    // Mutation: WithDiagridApp passes IdentityKind.Agent (or both share a hard-coded kind) -> the app assertion fails.
    [Fact]
    public void An_app_and_an_agent_in_one_project_keep_their_own_kinds()
    {
        var project = Build(b =>
        {
            b.AddExecutable("my-agent", "dotnet", ".").WithDiagridAgent();
            b.AddExecutable("my-app", "dotnet", ".").WithDiagridApp();
        });

        Assert.Equal(IdentityKind.Agent, project.Identities.Single(i => i.Key.Name == "my-agent").Value.Kind);
        Assert.Equal(IdentityKind.App, project.Identities.Single(i => i.Key.Name == "my-app").Value.Kind);
    }

    // The proxy wiring is the same for an agent as for an app (dev run skips provisioning but still tunnels).
    // Mutation: omit the proxy for agents -> Single() throws; change its args -> the equality fails.
    [Fact]
    public void An_agent_gets_the_same_dev_run_proxy_as_an_app()
    {
        var agent = Build(b => b.AddContainer("same-name", "img").WithHttpEndpoint(targetPort: 8080).WithDiagridAgent());
        var app = Build(b => b.AddContainer("same-name", "img").WithHttpEndpoint(targetPort: 8080).WithDiagridApp());

        var agentArgs = ProxyArgs(agent);
        Assert.Equal(ProxyArgs(app), agentArgs);
        Assert.Equal("same-name", agentArgs[agentArgs.IndexOf("--id") + 1]);
        Assert.Contains("--app-port", agentArgs);
    }

    [Fact]
    public void Rejects_an_invalid_resource_name_for_an_agent()
    {
        var builder = DistributedApplication.CreateBuilder();
        builder.AddDiagridProject("demo");
        var bad = builder.AddExecutable("MyAgent", "dotnet", ".");

        var ex = Assert.Throws<ArgumentException>(() => bad.WithDiagridAgent());
        Assert.Contains("MyAgent", ex.Message, StringComparison.Ordinal);
    }

    // The multiple-projects error names the public method the developer called.
    [Fact]
    public void The_multiple_projects_error_names_WithDiagridAgent()
    {
        var builder = DistributedApplication.CreateBuilder();
        builder.AddDiagridProject("one");
        builder.AddDiagridProject("two");

        var ex = Assert.Throws<InvalidOperationException>(() => builder.AddExecutable("my-agent", "dotnet", ".").WithDiagridAgent());
        Assert.Contains("WithDiagridAgent", ex.Message, StringComparison.Ordinal);
    }

    private static DiagridProject Build(Action<IDistributedApplicationBuilder> configure)
    {
        var builder = DistributedApplication.CreateBuilder();
        var diagrid = builder.AddDiagridProject("demo");
        configure(builder);
        return diagrid.Resource;
    }

    private static List<string> ProxyArgs(DiagridProject project)
    {
        var owner = project.Identities.Keys.Single();
        var proxy = owner.Annotations.OfType<ResourceRelationshipAnnotation>()
            .Select(a => a.Resource)
            .OfType<ExecutableResource>()
            .Single(r => r.Name == $"{owner.Name}-diagrid-proxy");

        var args = new List<object>();
        var context = new CommandLineArgsCallbackContext(args, proxy);
        foreach (var annotation in proxy.Annotations.OfType<CommandLineArgsCallbackAnnotation>())
        {
            annotation.Callback(context).GetAwaiter().GetResult();
        }

        return args.Select(a => a is IManifestExpressionProvider m ? m.ValueExpression : a.ToString()!).ToList();
    }
}
