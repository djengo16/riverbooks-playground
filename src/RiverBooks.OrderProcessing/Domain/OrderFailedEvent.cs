using RiverBooks.SharedKernel;

namespace RiverBooks.OrderProcessing.Domain;

public class OrderFailedEvent : DomainEventBase
{
  public OrderFailedEvent(Guid orderId, string? failedReason = null)
  {
    this.OrderId = orderId;
    this.FailedReason = failedReason ?? string.Empty;
  }
  public Guid OrderId { get; }
  public string FailedReason { get; }
}
