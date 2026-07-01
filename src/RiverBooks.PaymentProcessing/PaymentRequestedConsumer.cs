using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.PaymentProcessing.Contracts;
using RiverBooks.PaymentProcessing.Contracts.Interfaces;

namespace RiverBooks.PaymentProcessing;

public class PaymentRequestedConsumer : IConsumer<PaymentRequestedMessage>
{
  private readonly IMediator _mediator;
  private readonly ILogger<PaymentRequestedConsumer> _logger;
  private readonly IPaymentProcessor _paymentProcessor;

  public PaymentRequestedConsumer(
    IMediator mediator,
    ILogger<PaymentRequestedConsumer> logger,
    IPaymentProcessor paymentProcessor)
  {
    _mediator = mediator;
    _logger = logger;
    _paymentProcessor = paymentProcessor;
  }

  public async Task Consume(ConsumeContext<PaymentRequestedMessage> context)
  {
    var message = context.Message;

    _logger.LogInformation("Payment requested for order with ID {OrderId}", message.OrderId);

    var paymentResult = await _paymentProcessor.ProcessPaymentAsync(message.PaymentMethodToken, message.Amount);

    if (paymentResult.IsSuccess)
    {
      await _mediator.Publish(new OrderPaymentSucceededEvent(message.OrderId));
    }
    else
    {
      // Currently nothing handles that but we may send email to the user/admin to notify about the failure
      await _mediator.Publish(new OrderPaymentFailedEvent(message.OrderId, paymentResult.Errors.First()));
    }
  }
}
