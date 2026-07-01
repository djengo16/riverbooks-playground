using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.PaymentProcessing.Contracts;

namespace RiverBooks.OrderProcessing.Integrations;

public class OrderPaymentSucceededEventHandler : INotificationHandler<OrderPaymentSucceededEvent>
{
  private readonly IOrderRepository _orderRepository;
  private readonly ILogger<OrderPaymentSucceededEventHandler> _logger;

  public OrderPaymentSucceededEventHandler(IOrderRepository orderRepository, ILogger<OrderPaymentSucceededEventHandler> logger)
  {
    _orderRepository = orderRepository;
    _logger = logger;
  }

  public async Task Handle(OrderPaymentSucceededEvent notification, CancellationToken cancellationToken)
  {
    var order = await _orderRepository.GetAsync(notification.OrderId);

    order.MarkAsPaid();

    await _orderRepository.SaveChangesAsync();

    _logger.LogInformation("OrderPaymentSucceededEventHandler executed.");
  }
}
