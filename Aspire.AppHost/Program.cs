var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();
var postgresdb = postgres.AddDatabase("postgresdb");


var apiService = builder.AddProject<Projects.Aspire_ApiService>("apiservice")
    .WithReference(cache)
    .WithReference(postgresdb);

builder.AddProject<Projects.Aspire_MigrationService>("migrations")
    .WithReference(postgresdb);

builder.AddProject<Projects.Aspire_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
