# Step 5.2 — API: UpdateSale, DeleteSale, CancelSale, CancelSaleItem Models

**Branch:** `feature/SALE-5-api-endpoints`  
**Commit:** `feat(api): add UpdateSale, DeleteSale, CancelSale, and CancelSaleItem request and response models`

---

## Files to Create

### UpdateSale
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/UpdateSale/UpdateSaleRequest.cs`
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/UpdateSale/UpdateSaleRequestValidator.cs`
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/UpdateSale/UpdateSaleItemRequest.cs`
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/UpdateSale/UpdateSaleProfile.cs`
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/UpdateSale/UpdateSaleResponse.cs`

### DeleteSale
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/DeleteSale/DeleteSaleRequest.cs`

### CancelSale
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/CancelSale/CancelSaleRequest.cs`

### CancelSaleItem
- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/CancelSaleItem/CancelSaleItemRequest.cs`

---

## Model Notes

### UpdateSaleRequest / UpdateSaleItemRequest

Same shape as `CreateSaleRequest` / `CreateSaleItemRequest`. `Id` comes from the route parameter (not the body).

### UpdateSaleRequestValidator

Same rules as `CreateSaleRequestValidator`.

### UpdateSaleResponse

Same shape as `CreateSaleResponse`.

### DeleteSaleRequest

Single field: `Id` (Guid, from route).

### CancelSaleRequest

Single field: `Id` (Guid, from route).

### CancelSaleItemRequest

Two fields: `Id` (Guid, sale id from route), `ItemId` (Guid, from route).

### UpdateSaleProfile

Maps `UpdateSaleRequest` → `UpdateSaleCommand`, `UpdateSaleResult` → `UpdateSaleResponse`.

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] All request models use route parameters (not body) for id fields
- [ ] `UpdateSaleRequestValidator` enforces spec §4 rules
- [ ] AutoMapper profile covers update mappings
- [ ] Reviewed by developer
- [ ] Committed with `feat(api): add UpdateSale, DeleteSale, CancelSale, and CancelSaleItem request and response models`
