using System.ComponentModel.DataAnnotations.Schema;
using Ardalis.GuardClauses;
using RiverBooks.Books.Domain;
using RiverBooks.SharedKernel;

namespace RiverBooks.Books;

internal class Book : IHaveDomainEvents
{
  public Guid Id { get; private set; } = Guid.NewGuid();
  public string Title { get; private set; } = string.Empty;
  public string Author { get; private set; } = string.Empty;
  public decimal Price { get; private set; }
  private List<DomainEventBase> _domainEvents = new();

  [NotMapped]
  public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();
  protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);

  internal Book(Guid id, string title, string author, decimal price)
  {
    Id = Guard.Against.Default(id);
    Title = Guard.Against.NullOrEmpty(title);
    Author = Guard.Against.NullOrEmpty(author);
    Price = Guard.Against.Negative(price);

    RegisterDomainEvent(new BookAddedEvent(this));
  }

  internal void UpdateTitle(string newTitle)
  {
    Title = Guard.Against.NullOrEmpty(newTitle);
    RegisterDomainEvent(new BookUpdatedEvent(this));
  }

  internal void UpdateAuthor(string newAuthor)
  {
    Author = Guard.Against.NullOrEmpty(newAuthor);
    RegisterDomainEvent(new BookUpdatedEvent(this));
  }

  internal void UpdatePrice(decimal newPrice)
  {
    Price = Guard.Against.Negative(newPrice);
    RegisterDomainEvent(new BookUpdatedEvent(this));
  }
  void IHaveDomainEvents.ClearDomainEvents() => _domainEvents.Clear();
}
