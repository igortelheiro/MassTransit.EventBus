namespace EventBus.Core.Events
{
    public record EmailRequestedEvent : IntegrationEvent
    {
        public string DestinationEmail { get; init; }
        public string Subject { get; init; }
        public string Content { get; init; }
        public string Template { get; init; }
    }
}
