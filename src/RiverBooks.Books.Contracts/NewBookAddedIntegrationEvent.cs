using RiverBooks.SharedKernel;

namespace RiverBooks.Books.Contracts;

public record NewBookAddedIntegrationEvent(BookDetails BookDetails)
  : IntegrationEventBase;
