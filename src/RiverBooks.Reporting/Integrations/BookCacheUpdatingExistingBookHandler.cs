using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Books.Contracts;
using RiverBooks.Reporting.Interfaces;

namespace RiverBooks.Reporting.Integrations;

internal class BookCacheUpdatingExistingBookHandler : INotificationHandler<BookUpdatedIntegrationEvent>
{
  IBookDetailsCache _bookCache;
  private readonly ILogger<BookCacheAddingNewBookHandler> _logger;

  public BookCacheUpdatingExistingBookHandler(IBookDetailsCache mediator, ILogger<BookCacheAddingNewBookHandler> logger)
  {
    _bookCache = mediator;
    _logger = logger;
  }

  public async Task Handle(BookUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
  {
    var bookDetails = new BookDetails(
      notification.BookDetails.BookId,
      notification.BookDetails.Title,
      notification.BookDetails.Author,
      notification.BookDetails.Price);

    await _bookCache.UpsertAsync(bookDetails);

    _logger.LogInformation("Cache updated with existing book {book}", bookDetails);
  }
}
