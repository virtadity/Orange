using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace OrderService.IntegrationTests;

public class OrdersIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:18").Build();

    public async Task InitializeAsync() => await _postgres.StartAsync();
    public async Task DisposeAsync() => await _postgres.DisposeAsync();

    [Fact]
    public async Task CreateOrder_ShouldSaveToPostgres_AndReturnCreated()
    {
        // Подменяем БД на Testcontainers, а Kafka — на заглушку
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<AppDbContext>>();
                    services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(_postgres.GetConnectionString()));

                    services.RemoveAll<IEventPublisher>();
                    services.AddSingleton<IEventPublisher, FakeEventPublisher>();
                });
            });

        // Создаём схему БД внутри контейнера
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureCreatedAsync();
        }

        var client = factory.CreateClient();
        var request = new CreateOrderRequest(Guid.NewGuid(), 1500.50m);

        // Act
        var response = await client.PostAsJsonAsync("/api/orders", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdOrder = await response.Content.ReadFromJsonAsync<Order>();
        Assert.NotNull(createdOrder);
        Assert.Equal(1500.50m, createdOrder.TotalAmount);
    }

    private class FakeEventPublisher : IEventPublisher
    {
        public Task PublishAsync<T>(string topic, T message, CancellationToken ct = default) => Task.CompletedTask;
    }
}
