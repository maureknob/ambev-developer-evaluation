# Step 7.1 — Redis: Cache Service Interface and Implementation

**Branch:** `feature/SALE-7-redis-cache`  
**Commit:** `feat(cache): add ISaleCacheService and RedisSaleCacheService`

---

## Context

Redis is declared in docker-compose as `ambev_developer_evaluation_cache`. The cache layer sits in front of `GetSaleHandler`: on a cache hit, skip MongoDB entirely; on a miss, fetch from MongoDB and populate the cache. Cache entries are invalidated on any write (create, update, cancel, delete).

---

## Files to Create

- `src/Ambev.DeveloperEvaluation.Application/Sales/Common/ISaleCacheService.cs`
- `src/Ambev.DeveloperEvaluation.Common/Cache/RedisSaleCacheService.cs`

> **Why Application, not Common?** `ISaleCacheService` takes `SaleDocument` as a parameter. Placing it in `Common` would force `Common` to depend on the `NoSQL` project, reversing the dependency direction. `Application` already depends on `NoSQL` (for `IMongoSaleRepository`), so the interface belongs there. `RedisSaleCacheService` (the implementation) stays in `Common` because it only depends on `IDistributedCache` and `SaleDocument` — `Common` references the `NoSQL` project for documents only.  
> Alternatively, if referencing `NoSQL` from `Common` is undesirable, move both to `Application`.

---

## ISaleCacheService

```csharp
public interface ISaleCacheService
{
    Task<SaleDocument?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task SetAsync(SaleDocument document, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);
}
```

---

## RedisSaleCacheService

- Inject `IDistributedCache` (from `Microsoft.Extensions.Caching.StackExchangeRedis`).
- Key pattern: `"sale:{id}"`.
- Serialize/deserialize with `System.Text.Json`.
- TTL: **5 minutes** (`AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)`).

```csharp
public class RedisSaleCacheService : ISaleCacheService
{
    private readonly IDistributedCache _cache;
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);
    private static string Key(Guid id) => $"sale:{id}";

    public RedisSaleCacheService(IDistributedCache cache) => _cache = cache;

    public async Task<SaleDocument?> GetAsync(Guid id, CancellationToken ct)
    {
        var bytes = await _cache.GetAsync(Key(id), ct);
        return bytes is null ? null : JsonSerializer.Deserialize<SaleDocument>(bytes);
    }

    public async Task SetAsync(SaleDocument doc, CancellationToken ct)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(doc);
        await _cache.SetAsync(Key(doc.Id), bytes,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = Ttl }, ct);
    }

    public Task RemoveAsync(Guid id, CancellationToken ct) =>
        _cache.RemoveAsync(Key(id), ct);
}
```

---

## NuGet Package

Add to `Common` (or whichever project hosts the cache service):

```
Microsoft.Extensions.Caching.StackExchangeRedis
```

---

## Definition of Done

- [ ] `ISaleCacheService` defined with three methods
- [ ] `RedisSaleCacheService` serializes to JSON, uses 5-min TTL
- [ ] NuGet package referenced
- [ ] Compiles with no errors
- [ ] Reviewed by developer
- [ ] Committed with `feat(cache): add ISaleCacheService and RedisSaleCacheService`
