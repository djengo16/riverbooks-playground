using System.Diagnostics;
using System.Reflection;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace RiverBooks.SharedKernel;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
  private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

  public LoggingBehavior(
      ILogger<LoggingBehavior<TRequest, TResponse>> logger)
  {
    _logger = logger;
  }

  public async Task<TResponse> Handle(
      TRequest message,
      RequestHandlerDelegate<TResponse> next,
      CancellationToken cancellationToken)
  {
    Guard.Against.Null(message);

    if (_logger.IsEnabled(LogLevel.Information))
    {
      _logger.LogInformation(
          "Handling {RequestName}",
          typeof(TRequest).Name);

      // Reflection! Could be a performance concern
      Type myType = message.GetType();

      IList<PropertyInfo> props =
          new List<PropertyInfo>(myType.GetProperties());

      foreach (PropertyInfo prop in props)
      {
        object? propValue =
            prop?.GetValue(message, null);

        _logger.LogInformation(
            "Property {Property} : {@Value}",
            prop?.Name,
            propValue);
      }
    }

    var sw = Stopwatch.StartNew();

    var response = await next();

    sw.Stop();

    _logger.LogInformation(
        "Handled {RequestName} with {Response} in {ms} ms",
        typeof(TRequest).Name,
        response,
        sw.ElapsedMilliseconds);

    return response;
  }
}
