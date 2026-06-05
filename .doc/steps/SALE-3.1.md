# Step 3.1 — Application Layer: CreateSale

**Branch:** `feature/SALE-3-application-layer`  
**Commit:** `feat(application): add CreateSale command and handler`

---

## Files to Create

- `src/src/Ambev.DeveloperEvaluation.Application/Sales/CreateSale/CreateSaleCommand.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/CreateSale/CreateSaleCommandValidator.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/CreateSale/CreateSaleHandler.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/CreateSale/CreateSaleProfile.cs`
- `src/src/Ambev.DeveloperEvaluation.Application/Sales/CreateSale/CreateSaleResult.cs`

---

## CreateSaleCommand

Fields mirror the API request body (spec §3.1):
- `SaleNumber`, `SaleDate`, `CustomerId`, `CustomerName`, `BranchId`, `BranchName`, `Items`
- `Items` is a list of `CreateSaleItemCommand` (nested record with `ProductId`, `ProductName`, `Quantity`, `UnitPrice`)
- Implements `IRequest<CreateSaleResult>`

## CreateSaleCommandValidator

- Validates all fields per spec §4 (same rules as `SaleValidator` but on the command)
- Use FluentValidation

## CreateSaleHandler

Logic (spec §3.1, §2, §5.1):
1. Validate command (pipeline behavior handles this via MediatR)
2. Check `saleNumber` uniqueness via `ISaleRepository.GetByNumberAsync` → throw `InvalidOperationException` if taken
3. Construct `Sale` entity; call `AddItem()` for each command item (domain enforces discount + business rules)
4. Persist via `ISaleRepository.CreateAsync`
5. Log `SaleCreatedEvent`
6. Return `CreateSaleResult`

## CreateSaleResult

Fields: full sale representation — `Id`, `SaleNumber`, `SaleDate`, `CustomerId`, `CustomerName`, `BranchId`, `BranchName`, `TotalAmount`, `IsCancelled`, `CreatedAt`, `Items` (list with all item fields including computed `Discount` and `TotalAmount`)

## CreateSaleProfile

AutoMapper profile mapping `Sale` → `CreateSaleResult` (and `SaleItem` → result item)

---

## Tests Turned Green by This Step

A01, A02, A03, A04, A05

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] Handler logic matches spec §3.1 exactly
- [ ] Domain entity methods used (not manual field assignment)
- [ ] `SaleCreatedEvent` logged on success
- [ ] Tests A01–A05 pass
- [ ] Reviewed by developer
- [ ] Committed with `feat(application): add CreateSale command and handler`
