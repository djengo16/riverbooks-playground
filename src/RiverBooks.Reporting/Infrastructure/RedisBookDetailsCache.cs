using System.Text.Json;
using Ardalis.Result;
using Microsoft.Extensions.Logging;
using RiverBooks.Books.Contracts;
using RiverBooks.Reporting.Interfaces;
using StackExchange.Redis;

namespace RiverBooks.Reporting.Infrastructure;

internal class RedisBookDetailsCache : IBookDetailsCache
{
  private readonly IDatabase _db;
  private readonly ILogger<RedisBookDetailsCache> _logger;

  public RedisBookDetailsCache(ILogger<RedisBookDetailsCache> logger, IConnectionMultiplexer connectionMultiplexer)
  {
    _db = connectionMultiplexer.GetDatabase();
    _logger = logger;
  }

  public async Task<Result<BookDetails>> GetByIdAsync(Guid id)
  {
    string? fetchedJson = await _db.StringGetAsync(id.ToString());

    if (fetchedJson is null)
    {
      _logger.LogWarning("Book {id} not found in {db}", id, "REDIS");
      return Result.NotFound();
    }
    var bookDetails = JsonSerializer.Deserialize<BookDetails>(fetchedJson);

    if (bookDetails is null) return Result.NotFound();

    _logger.LogInformation("Book {id} returned from {db}", id, "REDIS");
    return Result.Success(bookDetails);
  }

  public async Task<Result> UpsertAsync(BookDetails bookDetails)
  {
    var key = bookDetails.BookId.ToString();
    var bookJson = JsonSerializer.Serialize(bookDetails);

    await _db.StringSetAsync(key, bookJson);
    _logger.LogInformation("Book {id} stored in {db}", bookDetails.BookId, "REDIS");

    return Result.Success();
  }
}
