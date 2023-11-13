namespace AspireYouTubeSummariser.Shared.Models;

public class SummaryRequest
{
    public string VideoUrl { get; set; }
    public string VideoLanguageCode { get; set; }
    public string SummaryLanguageCode { get; set; }
}
