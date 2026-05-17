using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Books.Contracts;
using RiverBooks.Reporting.Interfaces;

namespace RiverBooks.Reporting.Infrastructure;

internal class ReadThroughBookDetailsCache : IBookDetailsCache
{
  private readonly RedisBookDetailsCache _redisCache;
  private readonly IMediator _mediator;
  private readonly ILogger<RedisBookDetailsCache> _logger;

  public ReadThroughBookDetailsCache(RedisBookDetailsCache redisCache,
    IMediator mediator,
    ILogger<RedisBookDetailsCache> logger)
  {
    _redisCache = redisCache;
    _mediator = mediator;
    _logger = logger;
  }

  public async Task<Result<BookDetails>> GetByIdAsync(Guid id)
  {
    var result = await _redisCache.GetByIdAsync(id);

    if (result.IsSuccess) return result;

    if(result.Status == ResultStatus.NotFound)
    {
      // fetch data from source
      _logger.LogInformation("Book {id} not found; fetching from source.", id);

      var query = new BookDetailsQuery(id);

      var queryResult = await _mediator.Send(query);

      if (queryResult.IsSuccess)
      {
        var dto = queryResult.Value;

        var book = new BookDetails(
          dto.BookId,
          dto.Title,
          dto.Author,
          dto.Price);

        await UpsertAsync(book);

        return book;
      }
    }

    return Result.NotFound();
  }

  public Task<Result> UpsertAsync(BookDetails bookDetails)
  {
    return _redisCache.UpsertAsync(bookDetails);
  }
}
