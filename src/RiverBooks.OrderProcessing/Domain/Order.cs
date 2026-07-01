using System.ComponentModel.DataAnnotations.Schema;
using Azure.Identity;
using RiverBooks.SharedKernel;

namespace RiverBooks.OrderProcessing.Domain;

public class Order : IHaveDomainEvents
{
  private Order() { }

  public Guid Id { get; private set; }
  public Guid UserId { get; private set; }
  public Address ShippingAddress { get; private set; } = default!;
  public Address BillingAddress { get; private set; } = default!;
  public OrderStatus Status { get; private set; }
  private readonly List<OrderItem> _orderItems = new();
  public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

  public DateTimeOffset DateCreated { get; private set; } = DateTimeOffset.Now;

  private void AddOrderItem(OrderItem item) => _orderItems.Add(item);

  private List<DomainEventBase> _domainEvents = new();
  [NotMapped]
  public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

  protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);
  void IHaveDomainEvents.ClearDomainEvents() => _domainEvents.Clear();

  public class Factory
  {
    public static Order Create(
                               Guid? orderId,
                               Guid userId,
                               Address shippingAddress,
                               Address billingAddress,
                               IEnumerable<OrderItem> orderItems)
    {
      if (!orderItems.Any())
      {
        throw new ArgumentException("Must have some order items", nameof(orderItems));
      }
      var order = new Order();
      order.Id = orderId ?? Guid.NewGuid();
      order.UserId = userId;
      order.ShippingAddress = shippingAddress;
      order.BillingAddress = billingAddress;
      order.Status = OrderStatus.PendingPayment;

      foreach (var item in orderItems)
      {
        order.AddOrderItem(item);
      }
      // uncomment this to make archunit test fail
      //var db = new OrderProcessingDbContext(
      //  new Microsoft.EntityFrameworkCore.DbContextOptions<OrderProcessingDbContext>());

      var orderCreatedEvent = new OrderCreatedEvent(order);
      order.RegisterDomainEvent(orderCreatedEvent);

      return order;
    }

  }

  public void MarkAsPaid()
  {
    this.Status = OrderStatus.Paid;
    // TODO: Test if that would work for all the flows
    this.RegisterDomainEvent(new OrderCompletedEvent(this));
  }

  public void MarkAsFailed(string failedReason)
  {
    this.Status = OrderStatus.Failed;
    this.RegisterDomainEvent(new OrderFailedEvent(this.Id, failedReason));
  }
}
