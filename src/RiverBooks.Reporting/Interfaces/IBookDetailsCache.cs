using Ardalis.Result;
using RiverBooks.Books.Contracts;

namespace RiverBooks.Reporting.Interfaces;

public interface IBookDetailsCache
{
  Task<Result<BookDetails>> GetByIdAsync(Guid id);
  Task<Result> UpsertAsync(BookDetails bookDetails);
}
