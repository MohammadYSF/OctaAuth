namespace Auth.Core;
public interface IEventHandler<T> where T : DomainEvent
{
    Task HandleAsync(T @event);
}
