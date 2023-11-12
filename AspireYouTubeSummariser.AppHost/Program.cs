var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedisContainer("cache");

var apiapp = builder.AddProject<Projects.AspireYouTubeSummariser_ApiApp>("apiapp");

builder.AddProject<Projects.AspireYouTubeSummariser_WebApp>("webapp")
       .WithReference(cache)
       .WithReference(apiapp);

builder.Build().Run();
