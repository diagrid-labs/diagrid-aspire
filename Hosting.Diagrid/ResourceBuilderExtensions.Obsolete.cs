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
public static partial class ResourceBuilderExtensions
{
    [Obsolete("Renamed to AddDiagridProject. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<DiagridProject> AddCatalystProject(this IDistributedApplicationBuilder applicationBuilder, string? customProjectName = null, ProjectDescriptor? descriptor = null)
        => applicationBuilder.AddDiagridProject(customProjectName, descriptor);

    [Obsolete("Renamed to WithDiagrid. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<TResource> WithCatalyst<TResource>(this IResourceBuilder<TResource> resourceBuilder, IResourceBuilder<DiagridProject>? diagridProjectBuilder = null)
        where TResource : Resource, IResourceWithEnvironment, IResourceWithWaitSupport
        => resourceBuilder.WithDiagrid(diagridProjectBuilder);

    [Obsolete("Renamed to WithDiagridApp. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<TResource> WithCatalystApp<TResource>(this IResourceBuilder<TResource> resourceBuilder, IResourceBuilder<DiagridProject>? diagridProjectBuilder = null)
        where TResource : Resource, IResourceWithEnvironment, IResourceWithWaitSupport
        => resourceBuilder.WithDiagridApp(diagridProjectBuilder);

    [Obsolete("Renamed to WithDiagridAgent. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<TResource> WithCatalystAgent<TResource>(this IResourceBuilder<TResource> resourceBuilder, IResourceBuilder<DiagridProject>? diagridProjectBuilder = null)
        where TResource : Resource, IResourceWithEnvironment, IResourceWithWaitSupport
        => resourceBuilder.WithDiagridAgent(diagridProjectBuilder);

    [Obsolete("Renamed to WithDiagridPubSub. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<DiagridProject> WithCatalystPubSub(this IResourceBuilder<DiagridProject> diagridProject)
        => diagridProject.WithDiagridPubSub();

    [Obsolete("Renamed to WithDiagridPubSub. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<DiagridProject> WithCatalystPubSub(this IResourceBuilder<DiagridProject> diagridProject, string name, IList<string>? scopes = null)
        => diagridProject.WithDiagridPubSub(name, scopes);

    [Obsolete("Renamed to WithDiagridKvStore. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<DiagridProject> WithCatalystKvStore(this IResourceBuilder<DiagridProject> diagridProject)
        => diagridProject.WithDiagridKvStore();

    [Obsolete("Renamed to WithDiagridKvStore. Catalyst is now Diagrid; this forwarder is removed in the next minor.")]
    public static IResourceBuilder<DiagridProject> WithCatalystKvStore(this IResourceBuilder<DiagridProject> diagridProject, string name, IList<string>? scopes = null)
        => diagridProject.WithDiagridKvStore(name, scopes);
}
