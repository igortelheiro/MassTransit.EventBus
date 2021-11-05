using EventBus.Core.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationEventLogEF.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService, IDisposable
    {
        private readonly IntegrationEventLogContext _integrationEventLogContext;
        private volatile DbConnection _dbConnection;
        private readonly List<Type> _eventTypes;
        private volatile bool _disposedValue;

        public IntegrationEventLogService(DbConnection dbConnection)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            
            _integrationEventLogContext = new IntegrationEventLogContext(
                new DbContextOptionsBuilder<IntegrationEventLogContext>()
                    .UseSqlServer(_dbConnection)
                    .Options);

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


        public async Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction, CancellationToken cancellationToken)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var eventLogEntry = new IntegrationEventLogEntry(@event, transaction.TransactionId);

            await _integrationEventLogContext.IntegrationEventLogs.AddAsync(eventLogEntry, cancellationToken);

            await _integrationEventLogContext.SaveChangesAsync(cancellationToken);
        }


        public async Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken) =>
            await UpdateEventStatus(eventId, EventStateEnum.Published, cancellationToken);


        public async Task MarkEventPublishAsFailedAsync(Guid eventId, CancellationToken cancellationToken) =>
            await UpdateEventStatus(eventId, EventStateEnum.PublishFailed, cancellationToken);


        private Task UpdateEventStatus(Guid eventId, EventStateEnum status, CancellationToken cancellationToken)
        {
            var eventLogEntry = _integrationEventLogContext.IntegrationEventLogs.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            _integrationEventLogContext.IntegrationEventLogs.Update(eventLogEntry);

            _integrationEventLogContext.SaveChanges();
            return Task.CompletedTask;
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

            if (result != null && result.Any())
            {
                return result.OrderBy(o => o.CreationTime)
                    .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)));
            }

            return new List<IntegrationEventLogEntry>();
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
