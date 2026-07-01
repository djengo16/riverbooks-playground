using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.Reporting.Interfaces;

namespace RiverBooks.Reporting.Integrations;

public class NewOrderCompletedIngestionHandler : INotificationHandler<OrderCompletedIntegrationEvent>
{
  private readonly ILogger<NewOrderCompletedIngestionHandler> _logger;
  private readonly OrderIngestionService _orderIngestionService;
  private readonly IBookDetailsCache _bookCache;

  public NewOrderCompletedIngestionHandler(
    ILogger<NewOrderCompletedIngestionHandler> logger,
    OrderIngestionService orderIngestionService,
    IBookDetailsCache bookCache)
  {
    _logger = logger;
    _orderIngestionService = orderIngestionService;
    _bookCache = bookCache;
  }

  public async Task Handle(OrderCompletedIntegrationEvent notification, CancellationToken ct)
  {
    _logger.LogInformation("Handling order completed event to populate reporting database...");

    var orderItems = notification.OrderDetails.OrderItems;
    int year = notification.OrderDetails.DateCreated.Year;
    int month = notification.OrderDetails.DateCreated.Month;

    foreach (var item in orderItems)
    {
      // look up book details to get author and title
      // TOODO: Implement Materialized View or other cache
      // DONE: Added cache option for books via read-through class that pulls the book from DB if doesn't exist in the cache
      // Also added domain & integration events when new book is created & updated so we can upsert it in the cache.
      //var bookDetailsQuery = new BookDetailsQuery(item.BookId);

      var result = await _bookCache.GetByIdAsync(item.BookId);

      if (!result.IsSuccess)
      {
        _logger.LogWarning("Issue loading book details for {id}", item.BookId);
        continue;
      }

      string author = result.Value.Author;
      string title = result.Value.Title;

      var sale = new BookSale
      {
        Author = author,
        BookId = item.BookId,
        Month = month,
        Title = title,
        Year = year,
        TotalSales = item.Quantity * item.UnitPrice,
        UnitsSold = item.Quantity
      };

      await _orderIngestionService.AddOrUpdateMonthlyBookSalesAsync(sale);
    }
  }
}
