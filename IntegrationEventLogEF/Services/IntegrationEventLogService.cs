using EventBus.Core.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationEventLogEF.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService, IDisposable
    {
        private readonly IntegrationEventLogContext _integrationEventLogContext;
        private readonly List<Type> _eventTypes;
        private volatile bool _disposedValue;

        public IntegrationEventLogService(IntegrationEventLogContext dbContext)
        {
            _integrationEventLogContext = dbContext;

            //TODO: Criar extensão de configuração para cadastrar eventos externos
            //_eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
            //    .GetTypes()
            //    .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
            //    .ToList();

            _eventTypes = typeof(IntegrationEvent).Assembly
                .GetTypes()
                .Where(t => t.Name.EndsWith("Event"))
                .ToList();
        }


        public async Task SaveEventLogAsync(IntegrationEvent @event, Guid transactionId, CancellationToken cancellationToken)
        {
            var eventLogEntry = new IntegrationEventLogEntry(@event, transactionId);

            await _integrationEventLogContext.IntegrationEventLogs.AddAsync(eventLogEntry, cancellationToken);

            await _integrationEventLogContext.SaveChangesAsync(cancellationToken);
        }


        public async Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken) =>
            await UpdateEventStatusAsync(eventId, EventStateEnum.Published, cancellationToken);


        public async Task MarkEventPublishAsFailedAsync(Guid eventId, CancellationToken cancellationToken) =>
            await UpdateEventStatusAsync(eventId, EventStateEnum.PublishFailed, cancellationToken);


        private async Task UpdateEventStatusAsync(Guid eventId, EventStateEnum status, CancellationToken cancellationToken)
        {
            var eventLogEntry = await _integrationEventLogContext.IntegrationEventLogs.SingleAsync(ie => ie.EventId == eventId, cancellationToken);
            eventLogEntry.State = status;
            eventLogEntry.TimesSent++;

            _integrationEventLogContext.IntegrationEventLogs.Update(eventLogEntry);

            await _integrationEventLogContext.SaveChangesAsync(cancellationToken);
        }


        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(string transactionId = null) =>
            await RetrieveEventLogsAsync(EventStateEnum.NotPublished, transactionId);


        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsFailedToPublishAsync(string transactionId = null) =>
            await RetrieveEventLogsAsync(EventStateEnum.PublishFailed, transactionId);


        private async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsAsync(EventStateEnum state, string transactionId = null, int maxCount = 50)
        {
            var shouldFilterByTransactionId = Guid.TryParse(transactionId, out var tid);
            if (!string.IsNullOrEmpty(transactionId) && !shouldFilterByTransactionId)
                throw new ArgumentException("TransactionId inválido");

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token;
            var result = await _integrationEventLogContext.IntegrationEventLogs
                .Where(e => (!shouldFilterByTransactionId || e.TransactionId == tid.ToString())
                            && e.State == state)
                .Take(maxCount)
                .ToListAsync(cancellationToken);

            if (result == null)
                return new List<IntegrationEventLogEntry>();

            return result.OrderBy(o => o.CreationTime)
                         .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)));
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _integrationEventLogContext?.Dispose();
                }


                _disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
