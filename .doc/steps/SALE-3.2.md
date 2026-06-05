# Step 3.2 — Application Layer: GetSale and GetSales

**Branch:** `feature/SALE-3-application-layer`  
**Commit:** `feat(application): add GetSale and GetSales queries`

---

## Files to Create

### GetSale (spec §3.2)
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/GetSale/GetSaleCommand.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/GetSale/GetSaleHandler.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/GetSale/GetSaleProfile.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/GetSale/GetSaleResult.cs`

### GetSales (spec §3.7)
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/GetSales/GetSalesCommand.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/GetSales/GetSalesHandler.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/GetSales/GetSalesProfile.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/GetSales/GetSalesResult.cs`

---

## GetSaleCommand

- Single field: `Id` (Guid)
- Implements `IRequest<GetSaleResult>`

## GetSaleHandler

1. Call `ISaleRepository.GetByIdAsync(command.Id)`
2. Throw `KeyNotFoundException` if null
3. Map to `GetSaleResult` and return

## GetSaleResult

Same shape as `CreateSaleResult` — full sale with items.

---

## GetSalesCommand

Fields (spec §3.7):
- `Page` (int, default 1)
- `Size` (int, default 10, max 100)
- `Order` (string, default "saleDate desc")
- Implements `IRequest<GetSalesResult>`

## GetSalesHandler

1. Call `ISaleRepository.GetPagedAsync(page, size, order)`
2. Map results to `GetSalesResult`

## GetSalesResult

```csharp
public IEnumerable<GetSaleResult> Data { get; init; }
public int TotalItems { get; init; }
public int CurrentPage { get; init; }
public int TotalPages { get; init; }
```

---

## Tests Turned Green by This Step

A20, A21

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] `KeyNotFoundException` thrown when sale not found
- [ ] Pagination fields correctly populated in `GetSalesResult`
- [ ] Tests A20, A21 pass
- [ ] Reviewed by developer
- [ ] Committed with `feat(application): add GetSale and GetSales queries`
