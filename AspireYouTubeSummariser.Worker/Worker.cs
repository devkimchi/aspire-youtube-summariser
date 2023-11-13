using System.Text.Json;

using AspireYouTubeSummariser.Shared;
using AspireYouTubeSummariser.Shared.Models;
using AspireYouTubeSummariser.Shared.Services;

using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace AspireYouTubeSummariser.Worker;

public class Worker : IHostedService
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly QueueClient _queueClient;
    private readonly TableClient _tableClient;
    private readonly IYouTubeService _youTubeService;
    private readonly ILogger<Worker> _logger;

    public Worker(QueueServiceClient queueServiceClient, TableServiceClient tableServiceClient, IYouTubeService youTubeService, ILogger<Worker> logger)
    {
        this._queueClient = (queueServiceClient ?? throw new ArgumentNullException(nameof(queueServiceClient))).GetQueueClient(ServiceNames.QueueName);
        this._tableClient = (tableServiceClient ?? throw new ArgumentNullException(nameof(tableServiceClient))).GetTableClient(ServiceNames.TableName);
        this._youTubeService = youTubeService ?? throw new ArgumentNullException(nameof(youTubeService));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            this._logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            QueueMessage[] messages = await this._queueClient.ReceiveMessagesAsync(maxMessages: 25);

            foreach (var message in messages)
            {
                var req = JsonSerializer.Deserialize<SummaryRequest>(message.MessageText, jsonSerializerOptions);
                this._logger.LogInformation($"Message from queue: {message.MessageId}");

                var videoId = this._youTubeService.GetVideoId(req.VideoUrl);
                var details = await this._youTubeService.GetVideoDetailsAsync(req.VideoUrl);
                var summary = await this._youTubeService.SummariseAsync(req.VideoUrl, req.VideoLanguageCode, req.SummaryLanguageCode);

                var entity = new VideoDetails(ServiceNames.TablePartitionKey, $"{videoId}::{req.SummaryLanguageCode}")
                {
                    VideoId = videoId,
                    VideoUrl = req.VideoUrl,
                    VideoLanguageCode = req.VideoLanguageCode,
                    SummaryLanguageCode = req.SummaryLanguageCode,
                    Title = details.Title,
                    Description = details.ShortDescription,
                    Summary = summary
                };
                await this._tableClient.UpsertEntityAsync(entity);

                await this._queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Worker stopping at: {time}", DateTimeOffset.Now);

        await Task.CompletedTask;
    }
}
