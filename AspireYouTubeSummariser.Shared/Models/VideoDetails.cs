using System.Runtime.Serialization;

using Azure;
using Azure.Data.Tables;

namespace AspireYouTubeSummariser.Shared.Models;

public class VideoDetails : ITableEntity
{
    public VideoDetails()
    {
    }

    public VideoDetails(string partitionKey, string rowKey)
    {
        this.PartitionKey = partitionKey;
        this.RowKey = rowKey;
    }

    [IgnoreDataMember]
    public string PartitionKey { get; set; }
    [IgnoreDataMember]
    public string RowKey { get; set; }

    public string VideoId { get; set; }
    public string VideoUrl { get; set; }
    public string VideoLanguageCode { get; set; }
    public string SummaryLanguageCode { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }

    [IgnoreDataMember]
    public DateTimeOffset? Timestamp { get; set; }
    [IgnoreDataMember]
    public ETag ETag { get; set; }
}