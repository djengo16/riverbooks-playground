using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.Users.Contracts;

namespace RiverBooks.OrderProcessing.Domain;

public class SendConfirmationEmailOrderCompletedEventHandler : INotificationHandler<OrderCompletedEvent>
{
  private readonly IMediator _mediator;
  private readonly ILogger _logger;

  public SendConfirmationEmailOrderCompletedEventHandler(IMediator mediator, ILogger<SendConfirmationEmailOrderCompletedEventHandler> logger)
  {
    _mediator = mediator;
    _logger = logger;
  }

  public async Task Handle(OrderCompletedEvent notification, CancellationToken ct)
  {
    // get user email from id
    var userByIdQuery = new UserDetailsByIdQuery(notification.Order.UserId);

    var result = await _mediator.Send(userByIdQuery);

    if (!result.IsSuccess)
    {
      // TODO: Add logging
      // DONE.

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

    // TODO: store emailId
    // DONE. It was already stored, we use it in an outbox pattern way, storing the id in mongo
  }
}
