namespace AspireYouTubeSummariser.WebApp.Models;

public class VideoDetails
{
    public string VideoId { get; set; }
    public string VideoUrl { get; set; }
    public string VideoLanguageCode { get; set; }
    public string SummaryLanguageCode { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
}