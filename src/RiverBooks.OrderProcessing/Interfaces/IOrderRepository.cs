using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing.Interfaces;
public interface IOrderRepository
{
  Task<List<Order>> ListAsync();
  Task AddAsync(Order order);
  Task<Order> GetAsync(Guid id);
  Task SaveChangesAsync();
}
