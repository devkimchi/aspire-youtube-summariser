using System.Net;

using AspireYouTubeSummariser.ApiApp.Models;
using AspireYouTubeSummariser.ApiApp.Services;

using Microsoft.AspNetCore.Mvc;

namespace AspireYouTubeSummariser.ApiApp.Controllers;

[ApiController]
[Route("[controller]")]
public class SummaryController : ControllerBase
{
    private readonly ISummaryService _service;
    private readonly ILogger<SummaryController> _logger;

    public SummaryController(ISummaryService service, ILogger<SummaryController> logger)
    {
        this._service = service ?? throw new ArgumentNullException(nameof(service));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost(Name = "CompleteVideoSummaries")]
    public async Task<ActionResult<string>> PostAsync([FromBody] SummaryRequest req)
    {
        this._logger.LogInformation("Complete summary request received");

        var summary = default(string);
        try
        {
            summary = await this._service
                                .ExecuteAsync(req.VideoUrl, req.VideoLanguageCode, req.SummaryLanguageCode);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, ex.Message);

            return new ObjectResult(ex.Message) { StatusCode = (int)HttpStatusCode.InternalServerError };
        }

        return new OkObjectResult(summary);
    }
}
