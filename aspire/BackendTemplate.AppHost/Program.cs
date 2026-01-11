var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server
var sqlServer = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Persistent);

var database = sqlServer.AddDatabase("backenddb");

// Add API project
var apiService = builder.AddProject<Projects.BackendTemplate_API>("backendtemplate-api")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();
