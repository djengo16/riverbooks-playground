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

  public CheckoutCartHandler(IApplicationUserRepository userRepository,
   // IMediator mediator,
    IPublishEndpoint publishEndpoint)
  {
    _userRepository = userRepository;
    //_mediator = mediator;
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
      new OrderItemDetails(item.BookId,
                           item.Quantity,
                           item.UnitPrice,
                           item.Description))
      .ToList();

    var createOrderMessage = new CreateOrderMessage(Guid.Parse(user.Id),
      request.shippingAddressId,
      request.billingAddressId,
      items);

    // TOODO: Consider replacing with a message-based approach for perf reasons
    // var result = await _mediator.Send(createOrderMessage); // synchronous

    // DONE: Instead of mediatr now we use message brocker, and to clear the cart the order created integration event is raised from PublishCreatedOrderIntegrationEventHandler
    // and consumed from OrderCreatedIntegrationEventConsumer where we clear the cart
    // this is done to achieve the full async flow because previously we got response there and then cleared the card

    await _publishEndpoint.Publish(createOrderMessage, cancellationToken);

    // Old approach, checking the result and clearing the cart, now that's handled from another consumer
    //if (!result.IsSuccess)
    //{
    //  // Change from a Result<OrderDetailsResponse> to Result<Guid>
    //  return result.Map(x => x.OrderId);
    //}

    //user.ClearCart();
    //await _userRepository.SaveChangesAsync();

    // send email to customer
    // TOODO: do this in an event handler (Currently this handler is doing two things, creating order and sending email,
    // Imagine if later you decide to send SMS, publish analytics, update loyality points, notify admins and more.
    // DONE: it was already implemented - SendConfirmationEmailOrderCreatedEventHandler - handles OrderCreatedEvent

    //var command = new SendEmailCommand()
    //{
    //  To = user.Email ?? "steve@test.com",
    //  From = "noreply@test.com",
    //  Subject = "Your RiverBooks Purchase",
    //  Body = $"You bought {createOrderMessage.OrderItems.Count} items."
    //};
    //Guid emailId = await _mediator.Send(command);

    return Result.Success();
  }
}
