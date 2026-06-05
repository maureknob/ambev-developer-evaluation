# Step 3.3 — Application Layer: UpdateSale

**Branch:** `feature/SALE-3-application-layer`  
**Commit:** `feat(application): add UpdateSale command and handler`

---

## Files to Create

- `src/src/Ambev.DeveloperEvaluation.Application/Sales/UpdateSale/UpdateSaleCommand.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/UpdateSale/UpdateSaleCommandValidator.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/UpdateSale/UpdateSaleHandler.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/UpdateSale/UpdateSaleProfile.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/UpdateSale/UpdateSaleResult.cs`

---

## UpdateSaleCommand

Same fields as `CreateSaleCommand` plus `Id` (Guid — the sale to update).  
Items list uses `UpdateSaleItemCommand` (same fields as `CreateSaleItemCommand`).

## UpdateSaleCommandValidator

Same validation rules as `CreateSaleCommandValidator` (spec §4), plus `Id` must be non-empty GUID.

## UpdateSaleHandler

Logic (spec §3.3, §5.2):
1. Fetch sale by `Id` via `ISaleRepository.GetByIdAsync` → throw `KeyNotFoundException` if null
2. Throw `InvalidOperationException` if sale is cancelled (spec §2.4)
3. Capture `previousTotalAmount` before update
4. Check `saleNumber` uniqueness — `GetByNumberAsync` must return null or the same sale (spec §2.6)
5. Update header fields on the entity
6. Replace items: clear existing items, call `AddItem()` for each new item (domain re-applies business rules)
7. Set `UpdatedAt = DateTime.UtcNow`
8. Persist via `ISaleRepository.UpdateAsync`
9. Log `SaleModifiedEvent` with previous and new total
10. Return `UpdateSaleResult`

## UpdateSaleResult

Same shape as `CreateSaleResult`.

---

## Tests Turned Green by This Step

A06, A07, A08, A09

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] Cancelled sale check enforced before any mutation
- [ ] `SaleModifiedEvent` logged with both amounts
- [ ] `saleNumber` uniqueness check excludes the current sale
- [ ] Tests A06–A09 pass
- [ ] Reviewed by developer
- [ ] Committed with `feat(application): add UpdateSale command and handler`
