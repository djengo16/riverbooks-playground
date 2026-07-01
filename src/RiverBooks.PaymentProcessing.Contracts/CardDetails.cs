namespace RiverBooks.PaymentProcessing.Contracts;

public record CardDetails(
  string CardNumber,
  string CardHolderName,
  string ExpirationMonth,
  string ExpirationYear,
  string Cvv);
