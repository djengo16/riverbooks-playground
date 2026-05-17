using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;
using RiverBooks.Users.UseCases.Cart.ListItems;

namespace RiverBooks.Users.CartEndpoints;

internal class ListCartItems :
  EndpointWithoutRequest<CartResponse>
{
  private readonly IMediator _MediatR;

  public ListCartItems(IMediator MediatR)
  {
    _MediatR = MediatR;
  }

  public override void Configure()
  {
    Get("/cart");
    Claims("EmailAddress");
  }

  public override async Task HandleAsync(
    CancellationToken ct = default)
  {
    var emailAddress = User.FindFirstValue("EmailAddress");

    var query = new ListCartItemsQuery(emailAddress!);

    var result = await _MediatR.Send(query, ct);

    if (result.Status == ResultStatus.Unauthorized)
    {
      await HttpContext.Response.SendUnauthorizedAsync();
    }
    else
    {
      var response = new CartResponse();
      response.CartItems = result.Value;
      await HttpContext.Response.SendAsync(response);
    }
  }
}
