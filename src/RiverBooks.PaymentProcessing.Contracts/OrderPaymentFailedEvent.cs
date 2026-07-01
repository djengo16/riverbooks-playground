using RiverBooks.SharedKernel;

namespace RiverBooks.PaymentProcessing.Contracts;

public record OrderPaymentFailedEvent(Guid orderId, string failedReason)
  : IntegrationEventBase;
