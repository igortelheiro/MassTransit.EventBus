using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Core.Events;

namespace IntegrationEventLogEF.Services
{
    public interface IIntegrationEventLogService
    {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(string transactionId = null);
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsFailedToPublishAsync(string transactionId = null);
        Task SaveEventLogAsync(IntegrationEvent @event, Guid transactionId, CancellationToken cancellationToken);
        Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken);
        Task MarkEventPublishAsFailedAsync(Guid eventId, CancellationToken cancellationToken);
    }
}
