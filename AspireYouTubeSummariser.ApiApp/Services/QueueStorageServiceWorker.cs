using System.Text.Json;

using AspireYouTubeSummariser.ApiApp.Models;

using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace AspireYouTubeSummariser.ApiApp.Services;

public class QueueStorageServiceWorker : IHostedService
{
    private const string QueueName = "summaries";
    private const string TableName = "videos";
    private const string PartitionKey = "75198dc5-8463-415c-9478-ff67e1b78c98";

    private static readonly JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly QueueClient _queueClient;
    private readonly TableClient _tableClient;
    private readonly ILogger<QueueStorageServiceWorker> _logger;

    public QueueStorageServiceWorker(IServiceScopeFactory serviceScopeFactory, QueueServiceClient queueServiceClient, TableServiceClient tableServiceClient, ILogger<QueueStorageServiceWorker> logger)
    {
        this._serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        this._queueClient = (queueServiceClient ?? throw new ArgumentNullException(nameof(queueServiceClient))).GetQueueClient(QueueName);
        this._tableClient = (tableServiceClient ?? throw new ArgumentNullException(nameof(tableServiceClient))).GetTableClient(TableName);
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

                using (var scope = this._serviceScopeFactory.CreateScope())
                {
                    var summaryService = scope.ServiceProvider.GetService<ISummaryService>();
                    var videoId = summaryService.GetVideoId(req.VideoUrl);
                    var details = await summaryService.GetVideoDetailsAsync(req.VideoUrl);
                    var summary = await summaryService.ExecuteAsync(req.VideoUrl, req.VideoLanguageCode, req.SummaryLanguageCode);

                    var entity = new VideoDetails(PartitionKey, $"{videoId}::{req.SummaryLanguageCode}")
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
                }

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
