using System.Reflection;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using RiverBooks.Books;
using RiverBooks.EmailSending;
using RiverBooks.OrderProcessing.Integrations;
using RiverBooks.Reporting;
using RiverBooks.SharedKernel;
using RiverBooks.Users;
using RiverBooks.Users.Integrations;
using RiverBooks.Users.UseCases.Cart.AddItem;
using Serilog;

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (OpenTelemetry, health checks, etc.)
builder.AddServiceDefaults();

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));
builder.Services.AddHttpLogging(o => { });

builder.Services.AddFastEndpoints()
    .AddAuthenticationJwtBearer(s =>
    {
      s.SigningKey = builder.Configuration["Auth:JwtSecret"];
    })
    .AddAuthorization()
    .SwaggerDocument();

// Add Module Services
List<Assembly> mediatRAssemblies = [typeof(Program).Assembly];
builder.Services.AddBookModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddEmailSendingModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddOrderProcessingModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddReportingModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddUsersModuleServices(builder.Configuration, logger, mediatRAssemblies);

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

// EmailSending depends on MongoDB running
// docker run --name mongodb -d -p 27017:27017 mongo

// OrderProcessing depends on Redis running
// docker run --name my-redis -p 6379:6379 -d redis

// Set up MediatR (source generator based)
builder.Services.AddMediatR(cfg =>
{
  cfg.RegisterServicesFromAssemblies(mediatRAssemblies.ToArray());
});
builder.Services.AddMediatRLoggingBehavior();
builder.Services.AddMediatRFluentValidationBehavior();
builder.Services.AddValidatorsFromAssemblyContaining<AddItemToCartCommandValidator>();
builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>(); // domain events

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateOrderConsumer>();
    x.AddConsumer<OrderCreatedIntegrationEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost");

        cfg.ConfigureEndpoints(context);
    });
});

// TOODO: Add a check that certain services are only registered once to avoid multiple modules 
// stepping on one another's service wirings
// DONE: Added "TryAdd" which ensures service is registered only if it already hasn't
var app = builder.Build();

app.UseHttpLogging();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication()
   .UseAuthorization();

app.UseFastEndpoints()
    .UseSwaggerGen();

// Map Aspire default endpoints (health checks)
app.MapDefaultEndpoints();

app.Run();

public partial class Program { }
