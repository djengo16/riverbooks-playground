namespace RiverBooks.PaymentProcessing.Contracts.Interfaces;

public interface IPaymentTokenizationService
{
  Task<string> TokenizeCardAsync(CardDetails card);
}
