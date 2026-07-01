using RiverBooks.SharedKernel;

namespace RiverBooks.OrderProcessing.Domain;

public class OrderCompletedEvent : DomainEventBase
{
  public OrderCompletedEvent(Order order)
  {
    this.Order = order;
  }

  public Order Order { get; }
}
