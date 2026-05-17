using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Books.Contracts;
using RiverBooks.Books.Domain;

namespace RiverBooks.Books.Integrations;

internal class BookIntegrationEventDispatcherHandler : INotificationHandler<BookAddedEvent>
{
  private readonly IMediator _mediator;
  private readonly ILogger<BookIntegrationEventDispatcherHandler> _logger;

  public BookIntegrationEventDispatcherHandler(
    IMediator mediator,
    ILogger<BookIntegrationEventDispatcherHandler> logger)
  {
    _mediator = mediator;
    _logger = logger;
  }
  public async Task Handle(BookAddedEvent notification, CancellationToken cancellationToken)
  {
    var bookDetails = new BookDetails(
      notification.Book.Id,
      notification.Book.Title,
      notification.Book.Author,
      notification.Book.Price);

    await _mediator!.Publish(new NewBookAddedIntegrationEvent(bookDetails));

    _logger.LogInformation($"[DE Handler]New book integration event sent for {notification.Book.Title}.");
  }
}
