using MassTransit;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.PaymentProcessing.Contracts;

namespace RiverBooks.OrderProcessing.Integrations;

public class CreateOrderConsumer : IConsumer<CreateOrderMessage>
{
  private readonly IOrderRepository _orderRepository;
  private readonly IOrderAddressCache _addressCache;
  private readonly IPublishEndpoint _publishEndpoint;

  public CreateOrderConsumer(IOrderRepository orderRepository,
    IOrderAddressCache addressCache,
    IPublishEndpoint publishEndpoint)
  {
    _orderRepository = orderRepository;
    _addressCache = addressCache;
    _publishEndpoint = publishEndpoint;
  }

  public async Task Consume(ConsumeContext<CreateOrderMessage> context)
  {
    // need shipping and billing address info - how can we get it?
    // Option 1: Pass it into the command with everything else
    // Option 2: Pass in the IDs, look them up from the Users module's database
    // Option 3: Pass in the IDs, look them up via a Users module Query
    // Option 4: Pass in the IDs, look them up via a Users module HTTP API call
    // Option 5: Pass in the IDs, look them up in our local cache (materialized view)

    // Analysis:
    // Option 1 is a good one and a good default to use. Pass whatever is needed with the message/command.
    // Option 2 is bad; breaks modularity and adds tight coupling.
    // Option 3 is OK and is a good default to use with modular monoliths
    // Option 4 isn't ideal but is a common approach in microservices (where it adds temporal coupling)
    // Option 5 is ideal in microservices, less common in MM architecture but let's see what it looks like

    // need to look up user addresses in materialized view
    // IMPORTANT: addresses are immutable - if we have the id we know we have the correct address

    var command = context.Message;

    var billingAddress = await _addressCache.GetByIdAsync(command.BillingAddressId);
    var shippingAddress = await _addressCache.GetByIdAsync(command.ShippingAddressId);

    // no need to look up book details; description on items will work

    // business rule: we create the order exactly as the cart shows (using its price, description, etc.)
    var orderItems = new List<OrderItem>();

    foreach (var item in command.OrderItems)
    {
      orderItems.Add(new OrderItem(item.BookId, item.Quantity, item.UnitPrice, item.Description));
    }

    // need to create order - use OrderFactory

    var order = Order.Factory.Create(command.OrderId, command.UserId, billingAddress.Value.Address, shippingAddress.Value.Address, orderItems);

    await _orderRepository.AddAsync(order);
    await _orderRepository.SaveChangesAsync();

    var totalAmount = order.OrderItems.Sum(item => item.Quantity * item.UnitPrice);

    var paymentRequestedMessage = new PaymentRequestedMessage(order.Id, totalAmount, command.PaymentMethodToken);

    await _publishEndpoint.Publish(paymentRequestedMessage);
  }
}
