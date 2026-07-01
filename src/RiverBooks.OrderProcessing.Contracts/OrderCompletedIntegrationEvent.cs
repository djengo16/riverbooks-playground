using MediatR;

namespace RiverBooks.OrderProcessing.Contracts;

public class OrderCompletedIntegrationEvent : INotification
{
  public DateTimeOffset DateCompleted { get; set; } = DateTimeOffset.Now;
  public OrderDetailsDto OrderDetails { get; set; } = default!;

  public OrderCompletedIntegrationEvent()
  {
  }

  public OrderCompletedIntegrationEvent(OrderDetailsDto orderDetailsDto)
  {
    OrderDetails = orderDetailsDto;
  }
}
