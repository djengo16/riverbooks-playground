using RiverBooks.SharedKernel;

namespace RiverBooks.Books.Contracts;

public record BookUpdatedIntegrationEvent(BookDetails BookDetails)
  : IntegrationEventBase;
