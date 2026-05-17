using RiverBooks.SharedKernel;

namespace RiverBooks.Books.Domain;

internal class BookUpdatedEvent : DomainEventBase
{
  public BookUpdatedEvent(Book book)
  {
    this.Book = book;
  }
  public Book Book { get; }
}
