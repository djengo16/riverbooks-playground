using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.Users.Contracts;

namespace RiverBooks.OrderProcessing.Domain;

public class SendConfirmationEmailOrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
  private readonly IMediator _mediator;
  private readonly ILogger _logger;

  public SendConfirmationEmailOrderCreatedEventHandler(IMediator mediator, ILogger<SendConfirmationEmailOrderCreatedEventHandler> logger)
  {
    _mediator = mediator;
    _logger = logger;
  }

  public async Task Handle(OrderCreatedEvent notification, CancellationToken ct)
  {
    // get user email from id
    var userByIdQuery = new UserDetailsByIdQuery(notification.Order.UserId);

    var result = await _mediator.Send(userByIdQuery);

    if (!result.IsSuccess)
    {
      // TOODO: Add logging
      // DONE

      _logger.LogWarning($"User details could not be found for user with ID: {userByIdQuery.UserId}");
      return;
    }

    var userEmail = result.Value.EmailAddress;

    var command = new SendEmailCommand()
    {
      To = userEmail,
      From = "noreply@test.com",
      Subject = "Your RiverBooks Purchase",
      Body = $"You bought {notification.Order.OrderItems.Count} items."
    };

    Guid emailId = await _mediator.Send(command);

    // TOODO: store emailId
    // DONE: It was already stored, we use it in an outbox pattern way, store the id in mongo
  }
}
