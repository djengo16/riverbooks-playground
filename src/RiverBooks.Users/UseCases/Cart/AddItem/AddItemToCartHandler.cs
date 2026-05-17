using Ardalis.Result;
using MediatR;
using RiverBooks.Books.Contracts;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

public class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, Result>
{
  private readonly IApplicationUserRepository _userRepository;
  private readonly IMediator _MediatR;

  public AddItemToCartHandler(IApplicationUserRepository userRepository,
    IMediator MediatR)
  {
    _userRepository = userRepository;
    _MediatR = MediatR;
  }

  public async Task<Result> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

    if (user is null)
    {
      return Result.Unauthorized();
    }

    var bookDetailsQuery = new BookDetailsQuery(request.BookId);
    var result = await _MediatR.Send(bookDetailsQuery);

    var bookDetails = result.Value;

    var newCartItem = new CartItem(request.BookId, request.Quantity, bookDetails.Price, $"{bookDetails.Title} by {bookDetails.Author}");

    user!.AddItemToCart(newCartItem);

    await _userRepository.SaveChangesAsync();

    return Result.Success();

  }

}
