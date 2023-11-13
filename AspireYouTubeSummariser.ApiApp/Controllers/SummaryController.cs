using System.Net;

using AspireYouTubeSummariser.ApiApp.Models;
using AspireYouTubeSummariser.ApiApp.Services;

using Azure.Data.Tables;

using Microsoft.AspNetCore.Mvc;

namespace AspireYouTubeSummariser.ApiApp.Controllers;

[ApiController]
[Route("[controller]")]
public class SummaryController : ControllerBase
{
    private const string PartitionKey = "75198dc5-8463-415c-9478-ff67e1b78c98";
    private const string TableName = "videos";

    private readonly ISummaryService _service;
    private readonly TableClient _tableClient;
    private readonly ILogger<SummaryController> _logger;

    public SummaryController(ISummaryService service, TableServiceClient tableServiceClient, ILogger<SummaryController> logger)
    {
        this._service = service ?? throw new ArgumentNullException(nameof(service));
        this._tableClient = (tableServiceClient ?? throw new ArgumentNullException(nameof(tableServiceClient))).GetTableClient(TableName);
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet(Name = "GetVideoSummaries")]
    public async Task<ActionResult<List<VideoDetails>>> GetAsync()
    {
        this._logger.LogInformation("List summary request received");

        var entities = new List<VideoDetails>();
        try
        {
            var results = this._tableClient.QueryAsync<VideoDetails>(x => x.PartitionKey == PartitionKey);
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
