# OrderService (.NET 10 / Clean Architecture)

Микросервис обработки заказов, построенный по принципам Clean Architecture с интеграцией PostgreSQL, Apache Kafka и интеграционным тестированием через Testcontainers.

## Стек технологий
* **Backend:** C#, .NET 10, ASP.NET Core Web API
* **База данных:** PostgreSQL, Entity Framework Core
* **Брокер сообщений:** Apache Kafka (Confluent.Kafka)
* **Тестирование:** xUnit, Testcontainers (PostgreSQL), WebApplicationFactory
* **Инфраструктура:** Docker, Docker Compose

## Архитектура проекта
* `OrderService.Domain` — доменные сущности, бизнес-валидация и интерфейсы репозиториев/брокера.
* `OrderService.Application` — сценарии использования (Use Cases) и DTO.
* `OrderService.Infrastructure` — реализация доступа к данным (EF Core) и отправки событий в Kafka.
* `OrderService.API` — REST-контроллеры, конфигурация DI и Swagger.
* `OrderService.IntegrationTests` — сквозные интеграционные тесты с автоматическим поднятием PostgreSQL в Docker.

## Запуск проекта

1. Поднять инфраструктуру (PostgreSQL и Kafka):
```bash
docker-compose up -d
```

2. Запустить API:
```bash
dotnet run --project OrderService.API
```

3. Запустить интеграционные тесты (требуется запущенный Docker):
```bash
dotnet test
```
