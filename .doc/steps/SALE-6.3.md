# Step 6.3 — MongoDB: Wire Read-Side into Application Handlers

**Branch:** `feature/SALE-6-mongo-readmodel`  
**Commit:** `feat(application): wire MongoDB read-side into GetSale and GetSales handlers`

---

## Context

`GetSaleHandler` and `GetSalesHandler` currently read from PostgreSQL via `ISaleRepository`. They should be updated to read from MongoDB instead. Write handlers (`CreateSaleHandler`, `UpdateSaleHandler`, `CancelSaleHandler`, `CancelSaleItemHandler`, `DeleteSaleHandler`) must project/upsert/delete the Mongo document alongside the PostgreSQL write.

---

## Files to Modify

### GetSaleHandler.cs

- **Remove** the `ISaleRepository` dependency entirely.
- Inject `IMongoSaleRepository`.
- Call `GetByIdAsync(command.Id)` on the Mongo repo.
- Throw `KeyNotFoundException` if the document is `null`.
- Map `SaleDocument` → `GetSaleResult` (update or add `GetSaleProfile` if needed).

### GetSalesHandler.cs

- **Remove** the `ISaleRepository` dependency entirely.
- Inject `IMongoSaleRepository`.
- Call `GetPagedAsync(page, size, order)`.
- Map results to `GetSalesResult`.

### CreateSaleHandler.cs

- After the PostgreSQL `CreateAsync`, map the resulting `Sale` entity to a `SaleDocument` and call `_mongoRepo.UpsertAsync(...)`.

### UpdateSaleHandler.cs

- After the PostgreSQL `UpdateAsync`, project the updated sale to a `SaleDocument` and call `UpsertAsync`.

### CancelSaleHandler.cs

- After cancellation is persisted to Postgres, call `UpsertAsync` with the updated document.

### CancelSaleItemHandler.cs

- Same as above — project and upsert after item cancellation.

### DeleteSaleHandler.cs

- After `DeleteAsync` on Postgres, call `_mongoRepo.DeleteAsync(command.Id)`.

---

## Mapping Notes

- Add an AutoMapper profile (or extend existing ones) to map `SaleDocument` → `GetSaleResult` and `SaleDocument` → `GetSalesResult` item.
- Add a mapping `Sale` → `SaleDocument` to use in write handlers.

---

## Definition of Done

- [ ] `GetSaleHandler` reads from MongoDB
- [ ] `GetSalesHandler` reads from MongoDB
- [ ] All write handlers project to MongoDB after successful Postgres write
- [ ] All tests still pass (mock `IMongoSaleRepository` in existing unit tests)
- [ ] Reviewed by developer
- [ ] Committed with `feat(application): wire MongoDB read-side into GetSale and GetSales handlers`
