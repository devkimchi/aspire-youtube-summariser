using AspireYouTubeSummariser.WebApp.Models;

namespace AspireYouTubeSummariser.WebApp.Clients;

public interface IApiAppClient
{
    Task<List<WeatherForecast>> GetWeatherForecastsAsync();
    Task<string> GetVideoSummaryAsync(string videoUrl, string videoLanguageCode = "en", string summaryLanguageCode = "en");
    Task<List<VideoDetails>> GetVideoSummariesAsync();
}

public class ApiAppClient : IApiAppClient
{
    private readonly HttpClient _http;
    public ApiAppClient(HttpClient httpClient)
    {
        this._http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<WeatherForecast>> GetWeatherForecastsAsync()
    {
        var result = await this._http.GetFromJsonAsync<List<WeatherForecast>>("WeatherForecast")
                                     .ConfigureAwait(false);
        return result;
    }

    public async Task<string> GetVideoSummaryAsync(string videoUrl, string videoLanguageCode = "en", string summaryLanguageCode = "en")
    {
        var req = new { videoUrl = videoUrl, videoLanguageCode = videoLanguageCode, summaryLanguageCode = summaryLanguageCode };
        var result = await this._http.PostAsJsonAsync("Summary", req)
                                     .ConfigureAwait(false);
        var summary = await result.Content.ReadAsStringAsync()
                                  .ConfigureAwait(false);
        return summary;
    }

    public async Task<List<VideoDetails>> GetVideoSummariesAsync()
    {
        var results = await this._http.GetFromJsonAsync<List<VideoDetails>>("Summary")
                                      .ConfigureAwait(false);
        return results;
    }
}
