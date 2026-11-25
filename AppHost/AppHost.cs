using Aspire.Hosting;
using Diagrid.Aspire.Test.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

// USE_CATALYST=false runs Dapr OSS (CommunityToolkit); true runs Diagrid. TestApi is identical either way.
DemoWiring.Configure(builder, DemoWiring.UseCatalyst(builder), "../TestApi/TestApi.csproj");

builder.Build().Run();
