# Step 3.5 — Application Layer: CancelSale and CancelSaleItem

**Branch:** `feature/SALE-3-application-layer`  
**Commit:** `feat(application): add CancelSale and CancelSaleItem commands and handlers`

---

## Files to Create

### CancelSale
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/CancelSale/CancelSaleCommand.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/CancelSale/CancelSaleHandler.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/CancelSale/CancelSaleResult.cs`

### CancelSaleItem
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/CancelSaleItem/CancelSaleItemCommand.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/CancelSaleItem/CancelSaleItemHandler.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/CancelSaleItem/CancelSaleItemResult.cs`

---

## CancelSaleCommand

- Single field: `Id` (Guid)
- Implements `IRequest<CancelSaleResult>`

## CancelSaleHandler

Logic (spec §3.5, §2.4, §5.3):
1. Fetch sale by `Id` → throw `KeyNotFoundException` if null
2. Call `sale.Cancel()` (domain method — throws `InvalidOperationException` if already cancelled)
3. Persist via `ISaleRepository.UpdateAsync`
4. Log `SaleCancelledEvent`
5. Return `CancelSaleResult` (full sale object)

## CancelSaleResult

Same shape as `CreateSaleResult`.

---

## CancelSaleItemCommand

Fields:
- `SaleId` (Guid)
- `ItemId` (Guid)
- Implements `IRequest<CancelSaleItemResult>`

## CancelSaleItemHandler

Logic (spec §3.6, §2.4, §5.4):
1. Fetch sale by `SaleId` → throw `KeyNotFoundException` if null
2. Call `sale.CancelItem(command.ItemId)` (domain method — throws if sale cancelled, item not found, or item already cancelled)
3. Persist via `ISaleRepository.UpdateAsync`
4. Log `ItemCancelledEvent`
5. Return `CancelSaleItemResult` (full sale object with updated `TotalAmount`)

## CancelSaleItemResult

Same shape as `CreateSaleResult`.

---

## Tests Turned Green by This Step

A12, A13, A14, A15, A16, A17, A18, A19

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] Domain `Cancel()` and `CancelItem()` methods used — no manual flag-setting in handlers
- [ ] Correct events logged for each handler
- [ ] Tests A12–A19 all pass
- [ ] Reviewed by developer
- [ ] Committed with `feat(application): add CancelSale and CancelSaleItem commands and handlers`
