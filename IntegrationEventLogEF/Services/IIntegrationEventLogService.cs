using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Core.Events;
using Microsoft.EntityFrameworkCore.Storage;

namespace IntegrationEventLogEF.Services
{
    public interface IIntegrationEventLogService
    {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(string transactionId = null);
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsFailedToPublishAsync(string transactionId = null);
        Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction, CancellationToken cancellationToken);
        Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken);
        Task MarkEventPublishAsFailedAsync(Guid eventId, CancellationToken cancellationToken);
    }
}
