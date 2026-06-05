# Step 3.4 — Application Layer: DeleteSale

**Branch:** `feature/SALE-3-application-layer`  
**Commit:** `feat(application): add DeleteSale command and handler`

---

## Files to Create

- `src/src/Ambev.DeveloperEvaluation.Application/Sales/DeleteSale/DeleteSaleCommand.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/DeleteSale/DeleteSaleHandler.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/DeleteSale/DeleteSaleResult.cs`

---

## DeleteSaleCommand

- Single field: `Id` (Guid)
- Implements `IRequest<DeleteSaleResult>`

## DeleteSaleHandler

Logic (spec §3.4):
1. Call `ISaleRepository.DeleteAsync(command.Id)`
2. Throw `KeyNotFoundException` if returns false (not found)
3. Return `DeleteSaleResult { Success = true }`

## DeleteSaleResult

```csharp
public bool Success { get; init; }
```

---

## Tests Turned Green by This Step

A10, A11

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] `KeyNotFoundException` thrown when sale not found
- [ ] Tests A10, A11 pass
- [ ] Reviewed by developer
- [ ] Committed with `feat(application): add DeleteSale command and handler`
