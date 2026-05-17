namespace RiverBooks.Books.Contracts;

public record BookDetails(Guid BookId, string Title,
  string Author, decimal Price);
