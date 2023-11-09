using System.Text;
using System.Text.Json;

namespace YouTubeSummariser.WebApp.Services
{
    public interface IYouTubeService
    {
        Task<string> GetVideoSummaryAsync(string youTubeVideoUrl, string videoLanguageCode = "en", string summaryLanguageCode = "en");
    }

    public class YouTubeService : IYouTubeService
    {
        private readonly HttpClient _http;

        public YouTubeService(HttpClient http)
        {
            this._http = http ?? throw new ArgumentNullException(nameof(http));
        }

        public async Task<string> GetVideoSummaryAsync(string youTubeVideoUrl, string videoLanguageCode = "en", string summaryLanguageCode = "en")
        {
            //await Task.Delay(10000);
            //return "This is a summary of the video";

            var content = new StringContent(JsonSerializer.Serialize(new
            {
                videoUrl = youTubeVideoUrl,
                videoLanguageCode = videoLanguageCode,
                summaryLanguageCode = summaryLanguageCode
            }), Encoding.UTF8, "application/json");

            var response = await this._http.PostAsync("summary", content).ConfigureAwait(false);
            var summary = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return summary;
        }
    }
}
