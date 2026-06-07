# Step 7.2 — Redis: Wire Cache into Application Handlers

**Branch:** `feature/SALE-7-redis-cache`  
**Commit:** `feat(application): wire Redis cache into GetSale handler and write-side invalidation`

---

## Files to Modify

### GetSaleHandler.cs

Add cache-aside pattern:

```csharp
// 1. Try cache
var cached = await _cache.GetAsync(command.Id, cancellationToken);
if (cached is not null)
    return _mapper.Map<GetSaleResult>(cached);

// 2. Miss — read from MongoDB
var document = await _mongoRepo.GetByIdAsync(command.Id, cancellationToken);
if (document is null)
    throw new KeyNotFoundException($"Sale {command.Id} not found.");

// 3. Populate cache
await _cache.SetAsync(document, cancellationToken);
return _mapper.Map<GetSaleResult>(document);
```

### CreateSaleHandler.cs

After MongoDB upsert:
```csharp
await _cache.SetAsync(saleDocument, cancellationToken);
```

### UpdateSaleHandler.cs

After MongoDB upsert:
```csharp
await _cache.SetAsync(saleDocument, cancellationToken);
```

### CancelSaleHandler.cs

After MongoDB upsert, repopulate the cache with the fresh document (avoids a cache miss on the next GET):
```csharp
await _cache.SetAsync(saleDocument, cancellationToken);
```

### CancelSaleItemHandler.cs

Same — repopulate after upsert:
```csharp
await _cache.SetAsync(saleDocument, cancellationToken);
```

### DeleteSaleHandler.cs

After MongoDB delete, invalidate (the document is gone):
```csharp
await _cache.RemoveAsync(command.Id, cancellationToken);
```

---

## Notes

- `GetSalesHandler` (list query) is **not cached** — paginated lists are volatile and not worth the invalidation complexity for this evaluation.
- All write operations except Delete repopulate the cache eagerly, since the updated document is already available from the Mongo upsert step immediately before.
- Cache errors should not bubble up to the caller — wrap each cache call in try/catch and log if Redis is unavailable.

---

## Definition of Done

- [ ] `GetSaleHandler` implements cache-aside (hit → return, miss → Mongo → cache → return)
- [ ] All write handlers invalidate or repopulate the cache
- [ ] Cache errors swallowed gracefully (log + continue)
- [ ] Unit tests updated to mock `ISaleCacheService`
- [ ] Reviewed by developer
- [ ] Committed with `feat(application): wire Redis cache into GetSale handler and write-side invalidation`
