using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.PaymentProcessing.Contracts.Infrastructure;
using RiverBooks.PaymentProcessing.Contracts.Interfaces;
using Serilog;
using Stripe;

namespace RiverBooks.PaymentProcessing;

public static class PaymentProcessingModuleServicesExtensions
{
  public static IServiceCollection AddPaymentProcessingModuleServices(this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger,
    List<System.Reflection.Assembly> mediatRAssemblies)
  {
    StripeConfiguration.ApiKey = "secret goes here";
    services.AddScoped<PaymentIntentService>();
    services.AddScoped<PaymentMethodService>();
    services.AddScoped<StripePaymentProcessor>();

    services.AddScoped<IPaymentProcessor>(sp =>
        sp.GetRequiredService<StripePaymentProcessor>());

    services.AddScoped<IPaymentTokenizationService>(sp =>
        sp.GetRequiredService<StripePaymentProcessor>());

    mediatRAssemblies.Add(typeof(PaymentProcessingModuleServicesExtensions).Assembly);

    return services;
  }
}
