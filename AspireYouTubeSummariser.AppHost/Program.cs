var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedisContainer("cache");
var queue = builder.AddAzureStorage("storage").AddQueues("queue");

var apiapp = builder.AddProject<Projects.AspireYouTubeSummariser_ApiApp>("apiapp");

builder.AddProject<Projects.AspireYouTubeSummariser_WebApp>("webapp")
       .WithReference(cache)
       .WithReference(queue)
       .WithReference(apiapp);

builder.Build().Run();
