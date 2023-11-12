using AspireYouTubeSummariser.WebApp.Models;

namespace AspireYouTubeSummariser.WebApp.Clients;

public interface IApiAppClient
{
    Task<List<WeatherForecast>> GetWeatherForecastsAsync();
}

public class ApiAppClient : IApiAppClient
{
    private readonly HttpClient _http;
    public ApiAppClient(HttpClient httpClient)
    {
        this._http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public Task<List<WeatherForecast>> GetWeatherForecastsAsync()
    {
        var result = this._http.GetFromJsonAsync<List<WeatherForecast>>("WeatherForecast");
        return result;
    }
}
