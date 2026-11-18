namespace OrderService.Application.DTOs;

public record CreateOrderRequest(Guid CustomerId, decimal TotalAmount);
