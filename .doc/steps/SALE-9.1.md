# Step 9.1 — Startup: Automatic Database Migrations and Infrastructure Seeding

**Branch:** `feature/SALE-9-startup-migrations`  
**Commit:** `chore(startup): apply EF Core migrations, create MongoDB indexes, and verify Redis on startup`

---

## Context

The evaluator must be able to `docker compose up` and have the API work without any manual DB setup steps. This step adds a hosted startup routine that:

1. **PostgreSQL** — applies pending EF Core migrations automatically.
2. **MongoDB** — creates required indexes (unique on `saleNumber`, compound on `customerId + saleDate`) so the read-side is query-ready.
3. **Redis** — performs a `PING` health-check and logs the result; does not fail startup on Redis unavailability.

---

## Files to Create

- `src/Ambev.DeveloperEvaluation.WebApi/Infrastructure/DatabaseMigrationService.cs`

## Files to Modify

- `src/Ambev.DeveloperEvaluation.WebApi/Program.cs` (or `Startup.cs`)

---

## DatabaseMigrationService

```csharp
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

        // 1. PostgreSQL — EF Core migrations
        await ApplyPostgresAsync(scope, cancellationToken);

        // 2. MongoDB — indexes
        await EnsureMongoIndexesAsync(scope, cancellationToken);

        // 3. Redis — connectivity check
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
            throw; // startup should fail if Postgres is unavailable
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
            // Non-fatal: app can serve traffic; indexes are for performance/uniqueness enforcement
        }
    }

    private async Task CheckRedisAsync(IServiceScope scope, CancellationToken ct)
    {
        try
        {
            var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
            await cache.GetAsync("__ping__", ct); // no-op read as connectivity probe
            _logger.LogInformation("Redis connection verified.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis is unavailable — cache will be bypassed.");
            // Non-fatal: cache service already swallows errors at call sites
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
```

---

## Program.cs Registration

```csharp
builder.Services.AddHostedService<DatabaseMigrationService>();
```

Add **before** `app.Run()`.

---

## docker-compose.override.yml — health-check dependencies (optional but recommended)

```yaml
ambev.developerevaluation.webapi:
  depends_on:
    ambev.developerevaluation.database:
      condition: service_started
    ambev.developerevaluation.nosql:
      condition: service_started
    ambev.developerevaluation.cache:
      condition: service_started
```

This ensures the API container starts after its dependencies, reducing the chance of migration failures on cold start.

---

## Definition of Done

- [ ] `DatabaseMigrationService` registered as `IHostedService`
- [ ] EF Core migrations applied automatically on startup
- [ ] MongoDB `uk_saleNumber` unique index created (idempotent — `CreateManyAsync` skips existing indexes)
- [ ] Redis health-check logged on startup
- [ ] Postgres failure throws (hard fail); Mongo/Redis failures log warnings only (soft fail)
- [ ] `docker compose up` brings up a fully functional API with no manual steps
- [ ] Reviewed by developer
- [ ] Committed with `chore(startup): apply EF Core migrations, create MongoDB indexes, and verify Redis on startup`
