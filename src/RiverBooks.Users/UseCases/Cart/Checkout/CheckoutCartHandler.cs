using Ardalis.Result;
using MassTransit;
using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.Users.Interfaces;
using RiverBooks.Users.UseCases.Cart.AddItem;

namespace RiverBooks.Users.UseCases.Cart.Checkout;

public class CheckoutCartHandler : IRequestHandler<CheckoutCartCommand, Result<Guid>>
{
  private readonly IApplicationUserRepository _userRepository;
  private readonly IPublishEndpoint _publishEndpoint;

  public CheckoutCartHandler(IApplicationUserRepository userRepository, IPublishEndpoint publishEndpoint)
  {
    _userRepository = userRepository;
    _publishEndpoint = publishEndpoint;
  }

  public async Task<Result<Guid>> Handle(CheckoutCartCommand request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

    if (user is null)
    {
      return Result.Unauthorized();
    }

    var items = user.CartItems.Select(item =>
      new OrderItemDetails(
        item.BookId,
        item.Quantity,
        item.UnitPrice,
        item.Description))
      .ToList();

    var orderId = Guid.NewGuid();

    var createOrderMessage = new CreateOrderMessage(
      orderId,
      Guid.Parse(user.Id),
      request.ShippingAddress,
      request.BillingAddress,
      items,
      request.PaymentDetails.PaymentMethodToken);

    try
    {
      await _publishEndpoint.Publish(createOrderMessage, cancellationToken);
    }
    catch(Exception e)
    {
      Console.WriteLine(e);
    }

    return Result.Success(orderId);
  }
}
