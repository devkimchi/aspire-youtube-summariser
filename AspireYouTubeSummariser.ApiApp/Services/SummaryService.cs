﻿using Aliencube.YouTubeSubtitlesExtractor.Abstractions;

using AspireYouTubeSummariser.ApiApp.Configurations;

using Azure.AI.OpenAI;

namespace AspireYouTubeSummariser.ApiApp.Services;

public interface ISummaryService
{
    Task<string> ExecuteAsync(string videoUrl, string videoLanguageCode = "en", string summaryLanguageCode = "en");
}

public class SummaryService : ISummaryService
{
    private readonly IYouTubeVideo _youtube;
    private readonly OpenAIClient _openai;
    private readonly OpenAISettings _openAISettings;
    private readonly PromptSettings _promptSettings;
    private readonly ILogger<SummaryService> _logger;

    public SummaryService(IYouTubeVideo youtube, OpenAIClient openai, OpenAISettings openAISettings, PromptSettings promptSettings, ILogger<SummaryService> logger)
    {
        this._youtube = youtube ?? throw new ArgumentNullException(nameof(youtube));
        this._openai = openai ?? throw new ArgumentNullException(nameof(openai));
        this._openAISettings = openAISettings ?? throw new ArgumentNullException(nameof(openAISettings));
        this._promptSettings = promptSettings ?? throw new ArgumentNullException(nameof(promptSettings));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> ExecuteAsync(string videoUrl, string videoLanguageCode = "en", string summaryLanguageCode = "en")
    {
        this._logger.LogInformation($"Video URL: {videoUrl}");

        if (string.IsNullOrWhiteSpace(videoLanguageCode) == true)
        {
            videoLanguageCode = "en";
        }
        if (string.IsNullOrWhiteSpace(summaryLanguageCode) == true)
        {
            summaryLanguageCode = "en";
        }

        var transcript = await this.GetSubtitleAsync(videoUrl, videoLanguageCode)
                                   .ConfigureAwait(false);

        this._logger.LogInformation($"Transcript for ${videoUrl} extracted");

        var summary = await this.GetSummaryAsync(transcript, summaryLanguageCode)
                                .ConfigureAwait(false);

        this._logger.LogInformation($"Summary for ${videoUrl} generated");

        return summary;
    }

    private async Task<string> GetSubtitleAsync(string videoUrl, string videoLangaugeCode)
    {
        var subtitle = await this._youtube
                                 .ExtractSubtitleAsync(videoUrl, videoLangaugeCode)
                                 .ConfigureAwait(false);
        if (subtitle == null)
        {
            return string.Empty;
        }

        var transcript = subtitle.Content
                                 .Select(p => p.Text)
                                 .Aggregate((a, b) => $"{a}\n{b}");

        return transcript;
    }

    private async Task<string> GetSummaryAsync(string subtitle, string summaryLanguageCode)
    {
        var deploymentId = this._openAISettings.DeploymentId;
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            DeploymentName = deploymentId,
            Messages =
                {
                    new ChatMessage(ChatRole.System, this._promptSettings.System),
                    new ChatMessage(ChatRole.System, $"Here's the transcript. Summarise it in 5 bullet point items in the given language code of \"{summaryLanguageCode}\"."),
                    new ChatMessage(ChatRole.User, subtitle),
                },
            MaxTokens = this._promptSettings.MaxTokens,
            Temperature = this._promptSettings.Temperature,
        };

        var result = await this._openai
                               .GetChatCompletionsAsync(chatCompletionsOptions)
                               .ConfigureAwait(false);
        var summary = result.Value.Choices[0].Message.Content;

        return summary;
    }
}