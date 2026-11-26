using System.Text.Json;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Diagrid.Aspire.Hosting;
using Diagrid.Aspire.Hosting.ComponentSpec;

namespace Hosting.Diagrid.Tests;

public class ManagedServiceTests
{
    private static DiagridProject Build(Func<IResourceBuilder<DiagridProject>, IResourceBuilder<DiagridProject>> configure)
    {
        var builder = DistributedApplication.CreateBuilder();
        return configure(builder.AddDiagridProject("demo")).Resource;
    }

    // The default is the Diagrid resource's own name. It is expected to be overridden, because the store has to
    // match whatever Dapr state store component the services already address — see DemoWiring, which names it.
    // Mutation: change the constant to "statestore" and the demo's explicit name silently becomes redundant,
    // hiding the step a developer moving off Dapr OSS actually has to take.
    [Fact]
    public void Default_kv_store_is_named_kvstore()
        => Assert.Equal("kvstore", global::Diagrid.Aspire.Hosting.ResourceBuilderExtensions.SampleKvName);

    [Fact]
    public void Default_pubsub_is_unchanged_and_differs_from_the_kv_name()
    {
        Assert.Equal("pubsub", global::Diagrid.Aspire.Hosting.ResourceBuilderExtensions.SamplePubsubName);
        Assert.NotEqual(global::Diagrid.Aspire.Hosting.ResourceBuilderExtensions.SampleKvName, global::Diagrid.Aspire.Hosting.ResourceBuilderExtensions.SamplePubsubName);
    }

    // Mutation: re-add `.WithComponent(...)` to the convenience overload (kv create makes the component itself,
    // so a second `component create` conflicts).
    [Fact]
    public void Kv_convenience_overload_registers_the_store_but_no_duplicate_component()
    {
        var p = Build(b => b.WithDiagridKvStore());

        Assert.True(p.KvStores.ContainsKey("kvstore"));
        Assert.Empty(p.PubSubs);
        Assert.Empty(p.Components);
    }

    [Fact]
    public void Pubsub_convenience_overload_registers_the_broker_but_no_duplicate_component()
    {
        var p = Build(b => b.WithDiagridPubSub());

        Assert.True(p.PubSubs.ContainsKey("pubsub"));
        Assert.Empty(p.KvStores);
        Assert.Empty(p.Components);
    }

    // Mutation: generate a component with ActorStateStore = true again. Paired with positive assertions so an
    // empty component set cannot make this pass vacuously.
    [Fact]
    public void Convenience_overloads_do_not_claim_the_actor_state_store_role()
    {
        var p = Build(b => b.WithDiagridKvStore().WithDiagridPubSub());

        Assert.True(p.KvStores.ContainsKey("kvstore"));
        Assert.True(p.PubSubs.ContainsKey("pubsub"));
        Assert.DoesNotContain(p.Components.Values, c => c.Metadata.ContainsKey("actorStateStore"));
    }

    [Fact]
    public void Actor_state_store_is_still_available_explicitly()
    {
        var p = Build(b => b.WithDiagridKvStore("statestore")
            .WithComponent("statestore", new DiagridStateStore
            {
                Metadata = new() { State = "statestore", ActorStateStore = true },
            }));

        var flag = Assert.IsType<JsonElement>(p.Components["statestore"].Metadata["actorStateStore"]);
        Assert.True(flag.GetBoolean());
    }
}
