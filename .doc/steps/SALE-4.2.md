# Step 4.2 — ORM: SaleRepository

**Branch:** `feature/SALE-4-orm-persistence`  
**Commit:** `feat(orm): add SaleRepository`

---

## Files to Create

- `src/src/Ambev.DeveloperEvaluation.ORM/Repositories/SaleRepository.cs`

---

## Implementation Contract

Implements `ISaleRepository` (defined in Step 1.2).

### Method Notes

| Method            | Implementation Notes                                                        |
|-------------------|-----------------------------------------------------------------------------|
| `CreateAsync`     | `context.Sales.AddAsync(sale)` + `SaveChangesAsync`                        |
| `GetByIdAsync`    | `context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id)` |
| `GetByNumberAsync`| `FirstOrDefaultAsync(s => s.SaleNumber == saleNumber)`                     |
| `UpdateAsync`     | `context.Sales.Update(sale)` + `SaveChangesAsync`                          |
| `DeleteAsync`     | Fetch by id, remove, save; return `false` if not found                      |
| `GetPagedAsync`   | Apply ordering, `Skip`/`Take` for pagination, return items + total count    |

### Ordering in `GetPagedAsync`

Parse the `order` string (e.g. `"saleDate desc"`) to apply dynamic ordering. Support at minimum: `saleDate`, `saleNumber`, `totalAmount` — each with `asc`/`desc`.

---

## Notes

- Inject `DefaultContext` via constructor
- Follow the same structure as `UserRepository`
- Always include `Items` in queries that return a full `Sale` object

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] All 6 interface methods implemented
- [ ] `Items` included in all full-sale queries
- [ ] `GetPagedAsync` returns correct total count (not just page count)
- [ ] Reviewed by developer
- [ ] Committed with `feat(orm): add SaleRepository`
