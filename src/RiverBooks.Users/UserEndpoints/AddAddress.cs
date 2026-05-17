using Ardalis.Result;
using System.Security.Claims;
using FastEndpoints;
using MediatR;
using RiverBooks.Users.UseCases.User.AddAddress;

namespace RiverBooks.Users.UserEndpoints;

internal sealed class AddAddress : Endpoint<AddAddressRequest>
{
  private readonly IMediator _MediatR;

  public AddAddress(IMediator MediatR)
  {
    _MediatR = MediatR;
  }

  public override void Configure()
  {
    Post("/users/addresses");
    Claims("EmailAddress");
  }

  public override async Task HandleAsync(AddAddressRequest request,
           CancellationToken cancellationToken = default)
  {
    var emailAddress = User.FindFirstValue("EmailAddress");

    var command = new AddAddressToUserCommand(emailAddress!,
      request.Street1,
      request.Street2,
      request.City,
      request.State,
      request.PostalCode,
      request.Country);

    var result = await _MediatR.Send(command);

    if (result.Status == ResultStatus.Unauthorized)
    {
      await HttpContext.Response.SendUnauthorizedAsync();
    }
    else
    {
      await HttpContext.Response.SendOkAsync();
    }
  }
}
