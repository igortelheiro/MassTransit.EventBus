using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IntegrationEventLogEF.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IntegrationEventLogEF.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IntegrationEventController : ControllerBase
    {
        private readonly IIntegrationEventLogService _logService;
        private readonly ILogger<IntegrationEventController> _logger;

        public IntegrationEventController(IIntegrationEventLogService logService, ILogger<IntegrationEventController> logger)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpGet("Pending")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<IntegrationEventLogEntry>>> GetPendingToPublishEvents([FromQuery] string transactionId = null)
        {
            try
            {
                var pendingEvents = await _logService.RetrieveEventLogsPendingToPublishAsync(transactionId);
                if (pendingEvents.Any())
                {
                    return Ok(pendingEvents);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                const string errorTitle = "Erro ao consultar eventos pendentes para publicação";
                _logger.LogError(ex, errorTitle);
                return NotFound(new ProblemDetails { Title = errorTitle, Detail = ex.Message });
            }
        }


        [HttpGet("Failed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<IntegrationEventLogEntry>>> GetFailedToPublishEvents([FromQuery] string transactionId = null)
        {
            try
            {
                var failedEvents = await _logService.RetrieveEventLogsFailedToPublishAsync(transactionId);
                if (failedEvents.Any())
                {
                    return Ok(failedEvents);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                const string errorTitle = "Erro ao consultar eventos com falha na publicação";
                _logger.LogError(ex, errorTitle);
                return NotFound(new ProblemDetails { Title = errorTitle, Detail = ex.Message });
            }
        }
    }
}
