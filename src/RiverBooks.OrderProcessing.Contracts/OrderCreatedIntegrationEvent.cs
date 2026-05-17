using MediatR;

namespace RiverBooks.OrderProcessing.Contracts;

public class OrderCreatedIntegrationEvent : INotification
{
  public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.Now;
  public OrderDetailsDto OrderDetails { get; set; } = default!;

  public OrderCreatedIntegrationEvent()
  {
  }

  public OrderCreatedIntegrationEvent(OrderDetailsDto orderDetailsDto)
  {
    OrderDetails = orderDetailsDto;
  }
}
