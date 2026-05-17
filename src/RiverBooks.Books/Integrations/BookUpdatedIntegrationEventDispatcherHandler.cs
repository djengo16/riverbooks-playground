using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Books.Contracts;
using RiverBooks.Books.Domain;

namespace RiverBooks.Books.Integrations;

internal class BookUpdatedIntegrationEventDispatcherHandler : INotificationHandler<BookUpdatedEvent>
{
  private readonly IMediator _mediator;
  private readonly ILogger<BookUpdatedIntegrationEventDispatcherHandler> _logger;

  public BookUpdatedIntegrationEventDispatcherHandler(
    IMediator mediator,
    ILogger<BookUpdatedIntegrationEventDispatcherHandler> logger)
  {
    _mediator = mediator;
    _logger = logger;
  }

  public async Task Handle(BookUpdatedEvent notification, CancellationToken cancellationToken)
  {
    var bookDetails = new BookDetails(
      notification.Book.Id,
      notification.Book.Title,
      notification.Book.Author,
      notification.Book.Price);

    await _mediator!.Publish(new BookUpdatedIntegrationEvent(bookDetails));

    _logger.LogInformation($"[DE Handler]Book updated integration event sent for {notification.Book.Title}.");
  }
}
