using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

public record CheckoutCartCommand(
  string EmailAddress,
  Guid ShippingAddress,
  Guid BillingAddress,
  PaymentDetails PaymentDetails) : IRequest<Result<Guid>>;

public record PaymentDetails(
  string PaymentMethodToken);
