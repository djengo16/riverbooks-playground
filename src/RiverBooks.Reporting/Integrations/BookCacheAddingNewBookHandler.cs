using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Books.Contracts;
using RiverBooks.Reporting.Interfaces;

namespace RiverBooks.Reporting.Integrations;

internal class BookCacheAddingNewBookHandler : INotificationHandler<NewBookAddedIntegrationEvent>
{
  IBookDetailsCache _bookCache;
  private readonly ILogger<BookCacheAddingNewBookHandler> _logger;

  public BookCacheAddingNewBookHandler(IBookDetailsCache mediator, ILogger<BookCacheAddingNewBookHandler> logger)
  {
    _bookCache = mediator;
    _logger = logger;
  }

  public async Task Handle(NewBookAddedIntegrationEvent notification, CancellationToken ct)
  {
    var bookDetails = new BookDetails(
      notification.BookDetails.BookId,
      notification.BookDetails.Title,
      notification.BookDetails.Author,
      notification.BookDetails.Price);

    await _bookCache.UpsertAsync(bookDetails);

    _logger.LogInformation("Cache updated with new book {book}", bookDetails);
  }
}
