using System.Text.Json;

using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace AspireYouTubeSummariser.WebApp.Clients;

public interface IQueueServiceClientWrapper
{
    Task<Response<SendReceipt>> SendMessageAsync(string videoUrl, string videoLanguageCode = "en", string summaryLanguageCode = "en");
}

public class QueueServiceClientWrapper : IQueueServiceClientWrapper
{
    private const string QueueName = "summaries";

    private readonly QueueClient _queueClient;

    public QueueServiceClientWrapper(QueueServiceClient queueServiceClient)
    {
        this._queueClient = (queueServiceClient ?? throw new ArgumentNullException(nameof(queueServiceClient))).GetQueueClient(QueueName);
    }

    public async Task<Response<SendReceipt>> SendMessageAsync(string videoUrl, string videoLanguageCode = "en", string summaryLanguageCode = "en")
    {
        var msg = new { videoUrl = videoUrl, videoLanguageCode = videoLanguageCode, summaryLanguageCode = summaryLanguageCode };

        return await this._queueClient.SendMessageAsync(JsonSerializer.Serialize(msg))
                                      .ConfigureAwait(false);
    }
}
