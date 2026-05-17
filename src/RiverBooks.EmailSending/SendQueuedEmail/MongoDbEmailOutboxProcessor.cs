using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace RiverBooks.EmailSending.SendQueuedEmail;

public class MongoDbEmailOutboxProcessor(
  IMongoCollection<EmailOutboxEntity> emailEntityCollection,
  ISendEmail emailSender,
  ILogger<MongoDbEmailOutboxProcessor> logger) : IOutboxProcessor
{
  private readonly IMongoCollection<EmailOutboxEntity> _emailEntityCollection = emailEntityCollection;
  private readonly ISendEmail _emailSender = emailSender;
  private readonly ILogger<MongoDbEmailOutboxProcessor> _logger = logger;
  private const int BatchLimit = 50;

  /// <summary>
  /// Checks for any emails that haven't 
  /// </summary>
  /// <returns></returns>
  public async Task CheckForEmailsToSend()
  {
    // Clear the data
    if (false)
    {
      //await ClearAllDataAsync();
    }
    var filter = Builders<EmailOutboxEntity>.Filter
      .Eq(entity => entity.DateTimeUtcProcessed, null);

    for (int i = 0; i < BatchLimit; i++)
    {
      var unsentEmailEntity = await _emailEntityCollection
      .Find(filter).FirstOrDefaultAsync();

      // TOODO: Change this to a while loop so it processes more than 1 each time
      // DONE: Instead I use for loop with limit to prevent endless loop
      if (unsentEmailEntity != null)
      {
        try
        {
          _logger.LogInformation("Sending email {id}", unsentEmailEntity.Id);

          await _emailSender.SendEmailAsync(unsentEmailEntity.To,
            unsentEmailEntity.From,
            unsentEmailEntity.Subject,
            unsentEmailEntity.Body);

          var stopwatch = System.Diagnostics.Stopwatch.StartNew();
          var updateFilter = Builders<EmailOutboxEntity>.Filter.Eq(x => x.Id, unsentEmailEntity.Id);
          var update = Builders<EmailOutboxEntity>.Update.Set("DateTimeUtcProcessed", DateTime.UtcNow);
          var updateResult = await _emailEntityCollection.UpdateOneAsync(updateFilter, update);
          var timeTaken = stopwatch.Elapsed;
          stopwatch.Stop();

          _logger.LogInformation("UpdateResult: {result} records modified in {time}ms",
            updateResult.ModifiedCount,
            timeTaken.TotalMilliseconds);
        }
        catch (Exception ex)
        {
          // TOODO: Log more details
          // DONE: Logged more details
          _logger.LogError($"Failed to send email " +
            $"from {unsentEmailEntity.From}, " +
            $"to: {unsentEmailEntity.To}, " +
            $"with Email ID {unsentEmailEntity.Id}, with exception: {ex.Message}");

          // FIX:
          // Start a local test email server
          // docker run --name=papercut -p 25:25 -p 37408:37408 jijiechen/papercut:latest -d
        }
      }
      else
      {
        _logger.LogInformation("No emails to send; sleeping...");
        break;
      }
    }
    
  }

  private async Task ClearAllDataAsync()
  {
    var filter = Builders<EmailOutboxEntity>.Filter.Empty;
    await _emailEntityCollection.DeleteManyAsync(filter);
    _logger.LogWarning("ALL MongoDB Data Deleted!");
  }
}
