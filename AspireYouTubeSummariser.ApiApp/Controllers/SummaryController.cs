using Microsoft.AspNetCore.Mvc;

using AspireYouTubeSummariser.ApiApp.Models;
using AspireYouTubeSummariser.ApiApp.Services;

namespace AspireYouTubeSummariser.ApiApp.Controllers
{
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

        [HttpPost(Name = "Summarise")]
        public async Task<string> PostAsync([FromBody] SummaryRequestModel req)
        {
            this._logger.LogInformation("Summary request received");

            var summary = default(string);
            try
            {
                summary = await this._service
                                    .ExecuteAsync(req);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                summary = ex.Message;

                return summary;
            }

            return summary;
        }
    }
}
