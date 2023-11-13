using Aliencube.YouTubeSubtitlesExtractor.Abstractions;
using Aliencube.YouTubeSubtitlesExtractor;

using AspireYouTubeSummariser.Shared.Configurations;
using AspireYouTubeSummariser.Shared.Services;
using AspireYouTubeSummariser.Worker;

using Azure;
using Azure.AI.OpenAI;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureQueueService("queue");
builder.AddAzureTableService("table");

// Add services to the container.
builder.Services.AddSingleton<IYouTubeService, YouTubeService>();
builder.Services.AddSingleton<OpenAISettings>(p => p.GetService<IConfiguration>().GetSection(OpenAISettings.Name).Get<OpenAISettings>());
builder.Services.AddSingleton<PromptSettings>(p => p.GetService<IConfiguration>().GetSection(PromptSettings.Name).Get<PromptSettings>());
builder.Services.AddHttpClient<IYouTubeVideo, YouTubeVideo>();
builder.Services.AddSingleton<OpenAIClient>(p =>
{
    var openAISettings = p.GetService<OpenAISettings>();
    var endpoint = new Uri(openAISettings.Endpoint);
    var credential = new AzureKeyCredential(openAISettings.ApiKey);
    var openAIClient = new OpenAIClient(endpoint, credential);

    return openAIClient;
});
builder.Services.AddHostedService<Worker>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
