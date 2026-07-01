namespace RiverBooks.PaymentProcessing;

public record CardDetails(
  string CardNumber,
  string ExpirationMonth,
  string ExpirationYear,
  string Cvv);
