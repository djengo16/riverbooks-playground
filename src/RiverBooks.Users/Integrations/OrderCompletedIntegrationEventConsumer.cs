using MassTransit;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.Integrations;

public class OrderCompletedIntegrationEventConsumer : IConsumer<OrderCompletedIntegrationEvent>
{
  private readonly IApplicationUserRepository _userRepository;
  private readonly ILogger<OrderCompletedIntegrationEventConsumer> _logger;

  public OrderCompletedIntegrationEventConsumer(IApplicationUserRepository userRepository, ILogger<OrderCompletedIntegrationEventConsumer> logger)
  {
    _userRepository = userRepository;
    _logger = logger;
  }
  public async Task Consume(ConsumeContext<OrderCompletedIntegrationEvent> context)
  {
    var message = context.Message;
    var userId = message.OrderDetails.UserId;

    var user = await _userRepository.GetUserByIdAsync(userId);

    if (user is null)
    {
      _logger.LogWarning($"Cart cannot be cleared. User with Id: {userId} does not exist.");
      return;
    }

    user.ClearCart();
    await _userRepository.SaveChangesAsync();

    _logger.LogWarning($"Cart for User with Id: {userId} was successfully cleared.");
  }
}
