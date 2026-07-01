using Ardalis.Result;
using MediatR;

namespace RiverBooks.OrderProcessing.Contracts;

public record CreateOrderMessage(Guid OrderId,
                                 Guid UserId,
                                 Guid ShippingAddressId,
                                 Guid BillingAddressId,
                                 List<OrderItemDetails> OrderItems,
                                 string PaymentMethodToken) :
    IRequest<Result<OrderDetailsResponse>>;
