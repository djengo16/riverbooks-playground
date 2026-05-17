using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;
using RiverBooks.Users.UseCases.User.ListAddresses;

namespace RiverBooks.Users.UserEndpoints;
internal class ListAddresses :
  EndpointWithoutRequest<AddressListResponse>
{
  private readonly IMediator _MediatR;

  public ListAddresses(IMediator MediatR)
  {
    _MediatR = MediatR;
  }

  public override void Configure()
  {
    Get("/users/addresses");
    Claims("EmailAddress");
  }

  public override async Task HandleAsync(
    CancellationToken ct = default)
  {
    var emailAddress = User.FindFirstValue("EmailAddress");

    var query = new ListAddressesQuery(emailAddress!);

    var result = await _MediatR.Send(query, ct);

    if (result.Status == ResultStatus.Unauthorized)
    {
      await HttpContext.Response.SendUnauthorizedAsync();
    }
    else
    {
      var response = new AddressListResponse();

      response.Addresses = result.Value;

      await HttpContext.Response.SendAsync(response);
    }
  }
}
