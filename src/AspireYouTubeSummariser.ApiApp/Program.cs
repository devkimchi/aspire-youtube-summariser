using Aliencube.YouTubeSubtitlesExtractor;
using Aliencube.YouTubeSubtitlesExtractor.Abstractions;

using Azure;
using Azure.AI.OpenAI;

using AspireYouTubeSummariser.ApiApp.Configurations;
using AspireYouTubeSummariser.ApiApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Summariser dependencies
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
builder.Services.AddScoped<ISummaryService, SummaryService>();

var app = builder.Build();

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
