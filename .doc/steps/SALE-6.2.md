# Step 6.2 — MongoDB: MongoSaleRepository Implementation

**Branch:** `feature/SALE-6-mongo-readmodel`  
**Commit:** `feat(nosql): add MongoSaleRepository implementation`

---

## Files to Create

- `src/Ambev.DeveloperEvaluation.NoSQL/Repositories/MongoSaleRepository.cs`

---

## Implementation Requirements

### Constructor injection

```csharp
public class MongoSaleRepository : IMongoSaleRepository
{
    private readonly IMongoCollection<SaleDocument> _collection;

    public MongoSaleRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<SaleDocument>("sales");
    }
}
```

### UpsertAsync

- Use `ReplaceOneAsync` with `IsUpsert = true`, filter on `Id`.

### GetByIdAsync

- `FindAsync` by `Id`, return first or null.

### GetPagedAsync

- Apply `order` string (e.g., `"saleDate desc"`) by parsing field name and direction.
- Supported sort fields: `saleDate`, `saleNumber`, `totalAmount`, `createdAt`. Default: `saleDate desc`.
- Return `(items, totalCount)` using `CountDocumentsAsync` + `Find(...).Skip(...).Limit(...).ToListAsync()`.

### DeleteAsync

- `DeleteOneAsync` by `Id`.

---

## Notes

- Collection name: `"sales"` (lowercase).
- All methods must be async and accept `CancellationToken`.
- Do not throw if document not found in `DeleteAsync` — let the handler decide.

---

## Definition of Done

- [ ] All four interface methods implemented
- [ ] Upsert uses replace-with-upsert (not insert)
- [ ] Paged query respects `order` parameter
- [ ] Compiles with no errors
- [ ] Reviewed by developer
- [ ] Committed with `feat(nosql): add MongoSaleRepository implementation`
