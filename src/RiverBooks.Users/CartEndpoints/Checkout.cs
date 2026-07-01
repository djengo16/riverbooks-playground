using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;
using RiverBooks.PaymentProcessing.Contracts.Interfaces;
using RiverBooks.PaymentProcessing.Contracts;
using RiverBooks.Users.UseCases.Cart.AddItem;

namespace RiverBooks.Users.CartEndpoints;

internal class Checkout : Endpoint<CheckoutRequest, CheckoutResponse>
{
  private readonly IMediator _mediator;
  private readonly IPaymentTokenizationService _paymentTokenizationService;

  public Checkout(IMediator mediator, IPaymentTokenizationService paymentTokenizationService)
  {
    _mediator = mediator;
    _paymentTokenizationService = paymentTokenizationService;
  }

  public override void Configure()
  {
    Post("/cart/checkout");
    Claims("EmailAddress");
  }

  public override async Task HandleAsync(CheckoutRequest request, CancellationToken ct = default)
  {
    var emailAddress = User.FindFirstValue("EmailAddress");

    var cardDetails = new CardDetails(
      request.CardNumber,
      request.CardHolderName,
      request.ExpirationMonth,
      request.ExpirationYear,
      request.Cvv);

    var paymentMethodToken = await _paymentTokenizationService.TokenizeCardAsync(cardDetails);

    var paymentDetails = new PaymentDetails(paymentMethodToken);

    var command = new CheckoutCartCommand(
      emailAddress!,
      request.ShippingAddressId,
      request.BillingAddressId,
      paymentDetails);

    var result = await _mediator.Send(command, ct);

    if (result.Status == ResultStatus.Unauthorized)
    {
      await HttpContext.Response.SendUnauthorizedAsync();
    }
    else
    {
      await HttpContext.Response.SendOkAsync();
    }
  }
}
