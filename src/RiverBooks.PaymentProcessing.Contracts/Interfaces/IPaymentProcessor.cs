using Ardalis.Result;

namespace RiverBooks.PaymentProcessing.Contracts.Interfaces;

public interface IPaymentProcessor
{
  Task<Result> ProcessPaymentAsync(string paymentMethodToken, decimal amount);
}
