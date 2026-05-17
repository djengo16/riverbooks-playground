using FastEndpoints;
using MongoDB.Driver;

namespace RiverBooks.EmailSending.ListEmailsEndpoint;
internal class List :
  Endpoint<ListEmailsRequest, ListEmailsResponse>
{
  private readonly IMongoCollection<EmailOutboxEntity> _emailEntityCollection;

  public List(IMongoCollection<EmailOutboxEntity> emailEntityCollection)
  {
    _emailEntityCollection = emailEntityCollection;
  }

  public override void Configure()
  {
    Get("/emails");
    Claims("EmailAddress");
    // AllowAnonymous(); // TOODO: Lock this down, DONE
  }

  public override async Task HandleAsync(ListEmailsRequest listEmailsRequest,
    CancellationToken ct = default)
  {
    var page = listEmailsRequest.Page;
    var pageSize = listEmailsRequest.PageSize;

    // TOODO: Implement paging
    // DONE
    var filter = Builders<EmailOutboxEntity>.Filter.Empty;
    var emailEntities = await _emailEntityCollection
      .Find(filter)
      .Skip(page * pageSize)
      .Limit(pageSize)
      .Project(x => new EmailDto(x.Id, x.From, x.To, x.Subject, x.DateTimeUtcProcessed))
      .ToListAsync();

    var response = new ListEmailsResponse()
    {
      Count = emailEntities.Count,
      Emails = emailEntities // TOODO: Use a separate DTO, DONE
    };

    Response = response;
  }
}
