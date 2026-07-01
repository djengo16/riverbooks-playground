using RiverBooks.SharedKernel;

namespace RiverBooks.PaymentProcessing.Contracts;

public record PaymentRequestedMessage(
  Guid OrderId,
  decimal Amount,
  string PaymentMethodToken) : IntegrationEventBase;
