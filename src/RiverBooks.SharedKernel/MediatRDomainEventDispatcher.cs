using MediatR;

namespace RiverBooks.SharedKernel;

public class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
  private readonly IMediator _MediatR;

  public MediatRDomainEventDispatcher(IMediator MediatR)
  {
    _MediatR = MediatR;
  }

  public async Task DispatchAndClearEvents(IEnumerable<IHaveDomainEvents> entitiesWithEvents)
  {
    foreach (var entity in entitiesWithEvents)
    {
      var events = entity.DomainEvents.ToArray();
      entity.ClearDomainEvents();
      foreach (var domainEvent in events)
      {
        await _MediatR.Publish(domainEvent).ConfigureAwait(false);
      }
    }
  }
}
