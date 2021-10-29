﻿using System;
using System.Threading.Tasks;
using EventBus.Core.Events;
using EventBus.Core.Interfaces;
using MassTransit;

namespace RabbitMQEventBus
{
    public class MassTransitConsumer<TMessage> : IConsumer<TMessage>
        where TMessage : IntegrationEvent
    {
        #region Initialize
        private readonly IIntegrationEventHandler<TMessage> _handler;

        public MassTransitConsumer(IIntegrationEventHandler<TMessage> handler)
        {
            _handler = handler ?? throw new ArgumentNullException($"Nenhum handler foi encontrado para: {typeof(TMessage).Name}");
        }
        #endregion


        public async Task Consume(ConsumeContext<TMessage> context)
        {
            await _handler.Handle(context.Message);
        }
    }
}
