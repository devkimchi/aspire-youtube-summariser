using Aliencube.YouTubeSubtitlesExtractor.Abstractions;
using Aliencube.YouTubeSubtitlesExtractor;

using AspireYouTubeSummariser.ApiApp.Configurations;
using AspireYouTubeSummariser.ApiApp.Services;

using Azure.AI.OpenAI;

using Azure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureQueueService("queue");
builder.AddAzureTableService("table");

// Add services to the container.

builder.Services.AddScoped<ISummaryService, SummaryService>();
builder.Services.AddSingleton<OpenAISettings>(p => p.GetService<IConfiguration>().GetSection(OpenAISettings.Name).Get<OpenAISettings>());
builder.Services.AddSingleton<PromptSettings>(p => p.GetService<IConfiguration>().GetSection(PromptSettings.Name).Get<PromptSettings>());
builder.Services.AddHttpClient<IYouTubeVideo, YouTubeVideo>();
builder.Services.AddScoped<OpenAIClient>(p =>
{
    var openAISettings = p.GetService<OpenAISettings>();
    var endpoint = new Uri(openAISettings.Endpoint);
    var credential = new AzureKeyCredential(openAISettings.ApiKey);
    var openAIClient = new OpenAIClient(endpoint, credential);

    return openAIClient;
});
//builder.Services.AddHostedService<QueueStorageServiceWorker>();

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
