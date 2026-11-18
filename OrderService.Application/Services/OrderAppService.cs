using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Services;

public class OrderAppService
{
    private readonly IOrderRepository _repository;
    private readonly IEventPublisher _eventPublisher;

    public OrderAppService(IOrderRepository repository, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request, CancellationToken ct = default)
    {
        var order = new Order(request.CustomerId, request.TotalAmount);
        
        await _repository.AddAsync(order, ct);
        await _eventPublisher.PublishAsync("orders-created", new { order.Id, order.CustomerId, order.TotalAmount }, ct);

        return order;
    }

    public Task<Order?> GetOrderAsync(Guid id, CancellationToken ct = default) =>
        _repository.GetByIdAsync(id, ct);
}
