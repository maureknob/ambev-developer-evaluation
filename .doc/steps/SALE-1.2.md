# Step 1.2 — Domain Repository Interface: ISaleRepository

**Branch:** `feature/SALE-1-domain-entities`  
**Commit:** `feat(domain): add ISaleRepository interface`

---

## Files to Create

- `src/src/Ambev.DeveloperEvaluation.Domain/Repositories/ISaleRepository.cs`

---

## Interface Contract

```csharp
Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);
Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
Task<Sale?> GetByNumberAsync(string saleNumber, CancellationToken cancellationToken = default);
Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);
Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
Task<(IEnumerable<Sale> Items, int TotalCount)> GetPagedAsync(int page, int size, string order, CancellationToken cancellationToken = default);
```

### Notes

- `GetByNumberAsync` is used by handlers to enforce `saleNumber` uniqueness (spec §2.6)
- `GetPagedAsync` supports the List Sales endpoint (spec §3.7) — returns items for the current page and total count for pagination metadata
- Follow the same interface pattern as `IUserRepository` in the Users domain

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] Follows naming conventions from `IUserRepository`
- [ ] All methods needed by the 7 API endpoints are present
- [ ] Reviewed by developer
- [ ] Committed with `feat(domain): add ISaleRepository interface`
