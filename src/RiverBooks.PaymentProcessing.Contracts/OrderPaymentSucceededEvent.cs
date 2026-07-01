using RiverBooks.SharedKernel;

namespace RiverBooks.PaymentProcessing.Contracts;

public record OrderPaymentSucceededEvent(Guid OrderId)
  : IntegrationEventBase;
