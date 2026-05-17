using RiverBooks.SharedKernel;

namespace RiverBooks.Books.Domain;

internal class BookAddedEvent : DomainEventBase
{
  public BookAddedEvent(Book book)
  {
    this.Book = book;
  }
  public Book Book { get; set; }
}
