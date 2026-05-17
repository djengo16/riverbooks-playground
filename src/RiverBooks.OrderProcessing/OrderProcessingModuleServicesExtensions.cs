using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RiverBooks.OrderProcessing.Infrastructure;
using RiverBooks.OrderProcessing.Infrastructure.Data;
using RiverBooks.OrderProcessing.Interfaces;
using Serilog;
using StackExchange.Redis;

namespace RiverBooks.Users;

public static class OrderProcessingModuleServicesExtensions
{
  public static IServiceCollection AddOrderProcessingModuleServices(this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger,
    List<System.Reflection.Assembly> mediatRAssemblies)
  {
    string? connectionString = config.GetConnectionString("OrderProcessingConnectionString");
    services.AddDbContext<OrderProcessingDbContext>(config =>
      config.UseSqlServer(connectionString));

    services.TryAddScoped<IOrderRepository, EfOrderRepository>();
    services.TryAddScoped<RedisOrderAddressCache>();
    services.TryAddScoped<IOrderAddressCache, ReadThroughOrderAddressCache>();

    services.TryAddSingleton<IConnectionMultiplexer>(sp =>
    {
      return ConnectionMultiplexer.Connect(config.GetSection("Redis").GetValue<string>("ConnectionString"));
    });

    // if using MediatR in this module, add any assemblies that contain handlers to the list
    mediatRAssemblies.Add(typeof(OrderProcessingModuleServicesExtensions).Assembly);

    logger.Information("{Module} module services registered", "Order Processing");
    return services;
  }
}
