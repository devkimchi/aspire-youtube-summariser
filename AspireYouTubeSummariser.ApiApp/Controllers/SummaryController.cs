using System.Net;

using AspireYouTubeSummariser.Shared;
using AspireYouTubeSummariser.Shared.Models;
using AspireYouTubeSummariser.Shared.Services;

using Azure.Data.Tables;

using Microsoft.AspNetCore.Mvc;

namespace AspireYouTubeSummariser.ApiApp.Controllers;

[ApiController]
[Route("[controller]")]
public class SummaryController : ControllerBase
{
    private readonly IYouTubeService _service;
    private readonly TableClient _tableClient;
    private readonly ILogger<SummaryController> _logger;

    public SummaryController(IYouTubeService service, TableServiceClient tableServiceClient, ILogger<SummaryController> logger)
    {
        this._service = service ?? throw new ArgumentNullException(nameof(service));
        this._tableClient = (tableServiceClient ?? throw new ArgumentNullException(nameof(tableServiceClient))).GetTableClient(ServiceNames.TableName);
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet(Name = "GetVideoSummaries")]
    public async Task<ActionResult<List<VideoDetails>>> GetAsync()
    {
        this._logger.LogInformation("List summary request received");

        var entities = new List<VideoDetails>();
        try
        {
            var results = this._tableClient.QueryAsync<VideoDetails>(x => x.PartitionKey == ServiceNames.TablePartitionKey);
            await foreach (var page in results.AsPages())
            {
                entities.AddRange(page.Values);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, ex.Message);

            return new ObjectResult(ex.Message) { StatusCode = (int)HttpStatusCode.InternalServerError };
        }

        return new OkObjectResult(entities);
    }

    [HttpPost(Name = "CompleteVideoSummaries")]
    public async Task<ActionResult<string>> PostAsync([FromBody] SummaryRequest req)
    {
        this._logger.LogInformation("Complete summary request received");

        var summary = default(string);
        try
        {
            summary = await this._service
                                .SummariseAsync(req.VideoUrl, req.VideoLanguageCode, req.SummaryLanguageCode);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, ex.Message);

            return new ObjectResult(ex.Message) { StatusCode = (int)HttpStatusCode.InternalServerError };
        }

        return new OkObjectResult(summary);
    }
}
