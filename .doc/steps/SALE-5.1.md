# Step 5.1 — API: CreateSale, GetSale, GetSales Request/Response Models

**Branch:** `feature/SALE-5-api-endpoints`  
**Commit:** `feat(api): add CreateSale, GetSale, and GetSales request and response models`

---

## Files to Create

### CreateSale
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/CreateSale/CreateSaleRequest.cs`
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/CreateSale/CreateSaleRequestValidator.cs`
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/CreateSale/CreateSaleItemRequest.cs`
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/CreateSale/CreateSaleProfile.cs`
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/CreateSale/CreateSaleResponse.cs`

### GetSale
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/GetSale/GetSaleRequest.cs`
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/GetSale/GetSaleProfile.cs`
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/GetSale/GetSaleResponse.cs`

### GetSales
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/GetSales/GetSalesRequest.cs`
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/GetSales/GetSalesProfile.cs`
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/GetSales/GetSalesResponse.cs`

---

## Model Notes

### CreateSaleRequest / CreateSaleItemRequest

Match the API request body from spec §3.1. `Discount` and `TotalAmount` are **not** included in the request — they are computed by the domain.

### CreateSaleRequestValidator

Validates `CreateSaleRequest` using FluentValidation — same rules as spec §4.

### CreateSaleResponse

Match the `data` object from spec §3.1 success response. Include items with computed `Discount` and `TotalAmount`.

### GetSaleRequest

Single field: `Id` (Guid, from route parameter).

### GetSaleResponse

Same shape as `CreateSaleResponse`.

### GetSalesRequest

Fields: `Page` (int), `Size` (int), `Order` (string) — from query parameters.

### GetSalesResponse

```csharp
public IEnumerable<GetSaleResponse> Data { get; init; }
public int TotalItems { get; init; }
public int CurrentPage { get; init; }
public int TotalPages { get; init; }
```

### Profiles

AutoMapper profiles mapping:
- `CreateSaleRequest` → `CreateSaleCommand`
- `CreateSaleResult` → `CreateSaleResponse`
- Same pattern for GetSale and GetSales

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] Request models do not expose `Discount` or `TotalAmount` as inputs
- [ ] Validators match spec §4
- [ ] AutoMapper profiles cover all mappings
- [ ] Reviewed by developer
- [ ] Committed with `feat(api): add CreateSale, GetSale, and GetSales request and response models`
