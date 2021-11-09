namespace EventBus.Core.Events;

public record UserCreatedEvent : IntegrationEvent
{
    public string UserId { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
}