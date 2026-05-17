using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RiverBooks.Reporting.Infrastructure;
using RiverBooks.Reporting.Integrations;
using RiverBooks.Reporting.Interfaces;
using Serilog;

namespace RiverBooks.Reporting;

public static class ReportingModuleServicesExtensions
{
  public static IServiceCollection AddReportingModuleServices(this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger,
    List<System.Reflection.Assembly> mediatRAssemblies)
  {
    // configure module services
    services.TryAddScoped<ITopSellingBooksReportService, TopSellingBooksReportService>();
    services.TryAddScoped<ISalesReportService, DefaultSalesReportService>();
    services.TryAddScoped<OrderIngestionService>();
    services.TryAddScoped<RedisBookDetailsCache>();
    services.TryAddScoped<IBookDetailsCache, ReadThroughBookDetailsCache>();

    // if using MediatR in this module, add any assemblies that contain handlers to the list
    mediatRAssemblies.Add(typeof(ReportingModuleServicesExtensions).Assembly);

    logger.Information("{Module} module services registered", "Reporting");
    return services;
  }
}
