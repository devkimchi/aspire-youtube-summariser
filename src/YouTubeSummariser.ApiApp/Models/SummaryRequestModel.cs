namespace YouTubeSummariser.ApiApp.Models;

public class SummaryRequestModel
{
    public string VideoUrl { get; set; }
    public string VideoLanguageCode { get; set; }
    public string SummaryLanguageCode { get; set; }
}
