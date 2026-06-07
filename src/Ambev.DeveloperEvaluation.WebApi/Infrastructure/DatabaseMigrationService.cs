using Ambev.DeveloperEvaluation.Domain.ReadModel;
using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.WebApi.Infrastructure;

public class DatabaseMigrationService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DatabaseMigrationService> _logger;

    public DatabaseMigrationService(IServiceProvider services, ILogger<DatabaseMigrationService> logger)
    {
        _services = services;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();

        await ApplyPostgresAsync(scope, cancellationToken);
        await EnsureMongoIndexesAsync(scope, cancellationToken);
        await CheckRedisAsync(scope, cancellationToken);
    }

    private async Task ApplyPostgresAsync(IServiceScope scope, CancellationToken ct)
    {
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();
            _logger.LogInformation("Applying EF Core migrations...");
            await db.Database.MigrateAsync(ct);
            _logger.LogInformation("EF Core migrations applied.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply EF Core migrations.");
            throw;
        }
    }

    private async Task EnsureMongoIndexesAsync(IServiceScope scope, CancellationToken ct)
    {
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
            var collection = db.GetCollection<SaleDocument>("sales");

            var indexModels = new[]
            {
                new CreateIndexModel<SaleDocument>(
                    Builders<SaleDocument>.IndexKeys.Ascending(s => s.SaleNumber),
                    new CreateIndexOptions { Unique = true, Name = "uk_saleNumber" }),

                new CreateIndexModel<SaleDocument>(
                    Builders<SaleDocument>.IndexKeys
                        .Ascending(s => s.CustomerId)
                        .Descending(s => s.SaleDate),
                    new CreateIndexOptions { Name = "ix_customerId_saleDate" })
            };

            await collection.Indexes.CreateManyAsync(indexModels, ct);
            _logger.LogInformation("MongoDB indexes ensured.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not ensure MongoDB indexes — continuing startup.");
        }
    }

    private async Task CheckRedisAsync(IServiceScope scope, CancellationToken ct)
    {
        try
        {
            var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
            await cache.GetAsync("__ping__", ct);
            _logger.LogInformation("Redis connection verified.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis is unavailable — cache will be bypassed.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
