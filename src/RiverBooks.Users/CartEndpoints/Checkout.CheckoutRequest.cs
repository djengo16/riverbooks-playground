namespace RiverBooks.Users.CartEndpoints;

public record CheckoutRequest(
    Guid ShippingAddressId,
    Guid BillingAddressId,
    string CardNumber,
    string CardHolderName,
    string ExpirationMonth,
    string ExpirationYear,
    string Cvv);
