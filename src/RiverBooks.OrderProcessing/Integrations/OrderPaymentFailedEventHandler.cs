using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;

namespace RiverBooks.OrderProcessing.Integrations;

public class OrderPaymentFailedEventHandler : INotificationHandler<OrderFailedEvent>
{
  private readonly IOrderRepository _orderRepository;
  private readonly ILogger<OrderPaymentFailedEventHandler> _logger;

  public OrderPaymentFailedEventHandler(IOrderRepository orderRepository, ILogger<OrderPaymentFailedEventHandler> logger)
  {
    _orderRepository = orderRepository;
    _logger = logger;
  }

  public async Task Handle(OrderFailedEvent notification, CancellationToken cancellationToken)
  {
    var order = await _orderRepository.GetAsync(notification.OrderId);

    order.MarkAsFailed(notification.FailedReason);

    await _orderRepository.SaveChangesAsync();

    _logger.LogError("Payment failed for Order with ID: {orderId}, Failure reason: {failedReason}", notification.OrderId, notification.FailedReason);
  }
}
