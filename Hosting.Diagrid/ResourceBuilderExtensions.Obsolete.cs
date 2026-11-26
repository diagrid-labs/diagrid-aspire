using System;
using System.Collections.Generic;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Diagrid.Aspire.Hosting.Model;

namespace Diagrid.Aspire.Hosting;

// The old names, forwarding to the new ones, for one minor release. Catalyst is now Diagrid.
// Deleting this file is how the compatibility layer is removed.
//
// No warning suppression is needed here: defining an obsolete member emits nothing, and each forwarder
// calls the NEW, non-obsolete method. A forwarder that accidentally called an obsolete member would fail
// this project's build, because TreatWarningsAsErrors is on for it.
// The parameters deliberately keep their 1.0.0 names. A caller may have used named arguments, and a
// renamed parameter turns the layer that exists to prevent a compile error into one.
public static partial class ResourceBuilderExtensions
{
    [Obsolete("Renamed to AddDiagridProject. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<DiagridProject> AddCatalystProject(this IDistributedApplicationBuilder applicationBuilder, string? customProjectName = null, ProjectDescriptor? descriptor = null)
        => applicationBuilder.AddDiagridProject(customProjectName, descriptor);

    [Obsolete("Renamed to WithDiagrid. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<TResource> WithCatalyst<TResource>(this IResourceBuilder<TResource> resourceBuilder, IResourceBuilder<DiagridProject>? catalystProjectBuilder = null)
        where TResource : Resource, IResourceWithEnvironment, IResourceWithWaitSupport
        => resourceBuilder.WithDiagrid(catalystProjectBuilder);

    [Obsolete("Renamed to WithDiagridApp. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<TResource> WithCatalystApp<TResource>(this IResourceBuilder<TResource> resourceBuilder, IResourceBuilder<DiagridProject>? catalystProjectBuilder = null)
        where TResource : Resource, IResourceWithEnvironment, IResourceWithWaitSupport
        => resourceBuilder.WithDiagridApp(catalystProjectBuilder);

    [Obsolete("Renamed to WithDiagridAgent. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<TResource> WithCatalystAgent<TResource>(this IResourceBuilder<TResource> resourceBuilder, IResourceBuilder<DiagridProject>? catalystProjectBuilder = null)
        where TResource : Resource, IResourceWithEnvironment, IResourceWithWaitSupport
        => resourceBuilder.WithDiagridAgent(catalystProjectBuilder);

    [Obsolete("Renamed to WithDiagridPubSub. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<DiagridProject> WithCatalystPubSub(this IResourceBuilder<DiagridProject> catalystProject)
        => catalystProject.WithDiagridPubSub();

    [Obsolete("Renamed to WithDiagridPubSub. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<DiagridProject> WithCatalystPubSub(this IResourceBuilder<DiagridProject> catalystProject, string name, IList<string>? scopes = null)
        => catalystProject.WithDiagridPubSub(name, scopes);

    [Obsolete("Renamed to WithDiagridKvStore. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<DiagridProject> WithCatalystKvStore(this IResourceBuilder<DiagridProject> catalystProject)
        => catalystProject.WithDiagridKvStore();

    [Obsolete("Renamed to WithDiagridKvStore. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<DiagridProject> WithCatalystKvStore(this IResourceBuilder<DiagridProject> catalystProject, string name, IList<string>? scopes = null)
        => catalystProject.WithDiagridKvStore(name, scopes);
}
