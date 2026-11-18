namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order() { } // Для EF Core

    public Order(Guid customerId, decimal totalAmount)
    {
        if (totalAmount <= 0)
            throw new ArgumentException("Order amount must be greater than zero.", nameof(totalAmount));

        Id = Guid.NewGuid();
        CustomerId = customerId;
        TotalAmount = totalAmount;
        CreatedAt = DateTime.UtcNow;
    }
}
