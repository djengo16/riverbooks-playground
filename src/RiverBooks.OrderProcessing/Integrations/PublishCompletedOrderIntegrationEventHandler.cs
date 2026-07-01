using MassTransit;
using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing.Integrations;

public class PublishCompletedOrderIntegrationEventHandler :
  INotificationHandler<OrderCompletedEvent>
{
  private readonly IMediator _mediator;
  private readonly IPublishEndpoint _publishEndpoint;

  public PublishCompletedOrderIntegrationEventHandler(
    IMediator mediator,
    IPublishEndpoint publishEndpoint)
  {
    _mediator = mediator;
    _publishEndpoint = publishEndpoint;
  }

  public async Task Handle(OrderCompletedEvent notification, CancellationToken cancellationToken)
  {
    var dto = new OrderDetailsDto()
    {
      DateCreated = notification.Order.DateCreated,
      OrderId = notification.Order.Id,
      UserId = notification.Order.UserId,
      Country = notification.Order.ShippingAddress.Country,
      City = notification.Order.ShippingAddress.City,
      State = notification.Order.ShippingAddress.State,
      PostalCode = notification.Order.ShippingAddress.PostalCode,
      OrderItems = notification.Order.OrderItems
      .Select(oi => new OrderItemDetails(oi.BookId,
                                         oi.Quantity,
                                         oi.UnitPrice,
                                         oi.Description))
      .ToList()
    };

    var integrationEvent = new OrderCompletedIntegrationEvent(dto);

    await _mediator.Publish(integrationEvent, cancellationToken);
    await _publishEndpoint.Publish(integrationEvent, cancellationToken);
  }
}
