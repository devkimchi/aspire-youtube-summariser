using AspireYouTubeSummariser.Shared.Configurations;

using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedisContainer("cache");
var storage = builder.AddAzureStorage("storage");
var queue = storage.AddQueues("queue");
var table = storage.AddTables("table");

var openAISettings = builder.Configuration.GetSection(OpenAISettings.Name).Get<OpenAISettings>();

var apiapp = builder.AddProject<Projects.AspireYouTubeSummariser_ApiApp>("apiapp")
                    .WithEnvironment("OpenAI__Endpoint", openAISettings.Endpoint)
                    .WithEnvironment("OpenAI__ApiKey", openAISettings.ApiKey)
                    .WithEnvironment("OpenAI__DeploymentId", openAISettings.DeploymentId)
                    .WithReference(table);

builder.AddProject<Projects.AspireYouTubeSummariser_WebApp>("webapp")
       .WithReference(cache)
       .WithReference(queue)
       .WithReference(apiapp);

builder.AddProject<Projects.AspireYouTubeSummariser_Worker>("worker")
       .WithEnvironment("OpenAI__Endpoint", openAISettings.Endpoint)
       .WithEnvironment("OpenAI__ApiKey", openAISettings.ApiKey)
       .WithEnvironment("OpenAI__DeploymentId", openAISettings.DeploymentId)
       .WithReference(queue)
       .WithReference(table);

builder.Build().Run();
