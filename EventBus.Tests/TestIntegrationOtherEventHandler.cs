﻿using System.Threading.Tasks;
using EventBus.Core.Interfaces;

namespace EventBus.Tests
{
    public class TestIntegrationOtherEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
    {
        public bool Handled { get; private set; }

        public TestIntegrationOtherEventHandler()
        {
            Handled = false;
        }

        public Task Handle(TestIntegrationEvent @event)
        {
            Handled = true;
            return Task.CompletedTask;
        }
    }
}
