using System.Text.Json;

using AspireYouTubeSummariser.Shared;
using AspireYouTubeSummariser.Shared.Models;

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
    private readonly QueueClient _queueClient;

    public QueueServiceClientWrapper(QueueServiceClient queueServiceClient)
    {
        this._queueClient = (queueServiceClient ?? throw new ArgumentNullException(nameof(queueServiceClient))).GetQueueClient(ServiceNames.QueueName);
    }

    public async Task<Response<SendReceipt>> SendMessageAsync(string videoUrl, string videoLanguageCode = "en", string summaryLanguageCode = "en")
    {
        var msg = new SummaryRequest() { VideoUrl = videoUrl, VideoLanguageCode = videoLanguageCode, SummaryLanguageCode = summaryLanguageCode };

        return await this._queueClient.SendMessageAsync(JsonSerializer.Serialize(msg))
                                      .ConfigureAwait(false);
    }
}
