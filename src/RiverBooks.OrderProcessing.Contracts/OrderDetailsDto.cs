namespace RiverBooks.OrderProcessing.Contracts;

/// <summary>
/// Basic details of the order
/// TOODO: Include address info for geographic specific reports to use
/// DONE: Extended with address details that will be populated
/// </summary>
 
public class OrderDetailsDto
{
  public Guid OrderId { get; set; }
  public Guid UserId { get; set; }
  public DateTimeOffset DateCreated { get; set; }
  public List<OrderItemDetails> OrderItems { get; set; } = new();
  public string Country { get; set; } = "";
  public string City { get; set; } = "";
  public string State { get; set; } = "";
  public string PostalCode { get; set; } = "";
}
