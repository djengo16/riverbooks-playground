using Ardalis.Result;
using RiverBooks.PaymentProcessing.Contracts.Interfaces;
using Stripe;

namespace RiverBooks.PaymentProcessing.Contracts.Infrastructure;

public class StripePaymentProcessor : IPaymentProcessor, IPaymentTokenizationService
{
  private readonly PaymentIntentService _paymentIntentService;
  private readonly PaymentMethodService _paymentMethodService;

  public StripePaymentProcessor(
    PaymentIntentService paymentIntentService,
    PaymentMethodService paymentMethodService)
  {
    _paymentIntentService = paymentIntentService;
    _paymentMethodService = paymentMethodService;
  }

  public async Task<string> TokenizeCardAsync(CardDetails card)
  {
    return "pm_card_visa";
    //var paymentMethod = await _paymentMethodService.CreateAsync(
    //  new PaymentMethodCreateOptions
    //  {
    //    Type = "card",
    //    Card = new PaymentMethodCardOptions
    //    {
    //      Number = card.CardNumber,
    //      ExpMonth = long.Parse(card.ExpirationMonth),
    //      ExpYear = long.Parse(card.ExpirationYear),
    //      Cvc = card.Cvv
    //    }
    //  });

    //return paymentMethod.Id;
  }

  public async Task<Result> ProcessPaymentAsync(string paymentMethodToken, decimal amount)
  {
    var options = new PaymentIntentCreateOptions
    {
      Amount = (long)(amount * 100),
      Currency = "usd",
      PaymentMethod = paymentMethodToken,
      Confirm = true,
      AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
      {
        Enabled = true,
        AllowRedirects = "never"
      }
    };

    try
    {
      var result = await _paymentIntentService.CreateAsync(options);

      if (result.Status == "succeeded")
      {
        return Result.Success();
      }

      return Result.Error(
        $"Payment failed with status: {result.Status}");
    }
    catch (Exception ex)
    {
      return Result.Error(ex.Message);
    }
  }
}
