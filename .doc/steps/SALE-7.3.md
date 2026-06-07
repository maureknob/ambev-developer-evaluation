# Step 7.3 — Redis: DI Registration and Configuration

**Branch:** `feature/SALE-7-redis-cache`  
**Commit:** `feat(ioc): register Redis distributed cache and ISaleCacheService in DI`

---

## Files to Modify

- `src/Ambev.DeveloperEvaluation.IoC/ModuleInitializers/InfrastructureModuleInitializer.cs` (or equivalent)
- `src/Ambev.DeveloperEvaluation.WebApi/appsettings.json`
- `src/Ambev.DeveloperEvaluation.WebApi/appsettings.Development.json`

---

## appsettings.json — add Redis section

```json
"Redis": {
  "ConnectionString": "ambev.developerevaluation.cache:6379,password=ev@luAt10n"
}
```

> `appsettings.json` uses the **Docker service hostname**. Override in `appsettings.Development.json` for local-without-Docker development:

```json
"Redis": {
  "ConnectionString": "localhost:6379,password=ev@luAt10n"
}
```

---

## DI Registration

```csharp
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration["Redis:ConnectionString"];
    options.InstanceName = "ambev_";
});

services.AddScoped<ISaleCacheService, RedisSaleCacheService>();
```

---

## Notes

- `InstanceName` prefixes all keys in Redis (e.g., `ambev_sale:{id}`).
- If Redis is unavailable at startup, the app must still start — `IDistributedCache` falls back gracefully; the cache service wraps errors.

---

## Definition of Done

- [ ] `AddStackExchangeRedisCache` registered with correct connection string
- [ ] `ISaleCacheService` registered as scoped
- [ ] Connection string matches docker-compose credentials
- [ ] App starts without errors even when Redis is down
- [ ] Reviewed by developer
- [ ] Committed with `feat(ioc): register Redis distributed cache and ISaleCacheService in DI`
