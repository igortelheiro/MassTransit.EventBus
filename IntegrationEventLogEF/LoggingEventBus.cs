using EventBus.Core.Events;
using EventBus.Core.Interfaces;
using IntegrationEventLogEF.Services;
using IntegrationEventLogEF.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace IntegrationEventLogEF
{
    public abstract class LoggingEventBus : IEventBus
    {
        private readonly IIntegrationEventLogService _logService;
        private readonly IntegrationEventLogContext _logContext;

        protected LoggingEventBus(IServiceProvider serviceProvider)
        {
            _logService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();
            _logContext = serviceProvider.GetRequiredService<IntegrationEventLogContext>();
        }


        public abstract Task PublishAsync<TEvent>(TEvent @event) where TEvent : IntegrationEvent;


        protected async Task LogAndPublishAsync<TEvent>(TEvent @event, Func<CancellationToken, Task> publisher) where TEvent : IntegrationEvent
        {
            var currentTransaction = _logContext.Database.CurrentTransaction;
            if (currentTransaction == null)
            {
                await ResilientTransaction.New(_logContext).ExecuteAsync(async cancellationToken =>
                    await SaveEventLogAndPublishAsync(@event, publisher, _logContext.Database.CurrentTransaction, cancellationToken));
                return;
            }
            
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token;
            await _logContext.Database.UseTransactionAsync(currentTransaction.GetDbTransaction(), cancellationToken);
            await SaveEventLogAndPublishAsync(@event, publisher, currentTransaction, cancellationToken);
        }


        private async Task SaveEventLogAndPublishAsync<TEvent>(TEvent @event,
                                                               Func<CancellationToken, Task> publisher,
                                                               IDbContextTransaction transaction,
                                                               CancellationToken cancellationToken) where TEvent : IntegrationEvent
        {
            try
            {
                await _logContext.SaveChangesAsync(cancellationToken);
                await _logService.SaveEventAsync(@event, transaction, cancellationToken);
                await transaction.CreateSavepointAsync("eventSaved", cancellationToken);
                await publisher(cancellationToken);
                await _logService.MarkEventAsPublishedAsync(@event.Id, cancellationToken);
            }
            catch
            {
                await transaction.RollbackToSavepointAsync("eventSaved", cancellationToken);
                await _logService.MarkEventPublishAsFailedAsync(@event.Id, cancellationToken);
            }
        }


        public abstract void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}
