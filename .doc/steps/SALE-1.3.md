# Step 1.3 — Domain Validation: SaleValidator

**Branch:** `feature/SALE-1-domain-entities`  
**Commit:** `feat(domain): add SaleValidator`

---

## Files to Create

- `src/src/Ambev.DeveloperEvaluation.Domain/Validation/SaleValidator.cs`

---

## Validation Rules

### Sale-Level (spec §4.1)

| Field          | Rule                                              |
|----------------|---------------------------------------------------|
| `SaleNumber`   | Required, non-empty, max 50 chars                 |
| `SaleDate`     | Required, valid UTC datetime                      |
| `CustomerId`   | Required, non-empty GUID                          |
| `CustomerName` | Required, non-empty, max 100 chars                |
| `BranchId`     | Required, non-empty GUID                          |
| `BranchName`   | Required, non-empty, max 100 chars                |
| `Items`        | Required, must contain at least 1 item            |
| `Items`        | No duplicate `ProductId` values                   |

### SaleItem-Level (spec §4.2)

| Field         | Rule                                              |
|---------------|---------------------------------------------------|
| `ProductId`   | Required, non-empty GUID                          |
| `ProductName` | Required, non-empty, max 100 chars                |
| `Quantity`    | Required, integer, min: 1, max: 20                |
| `UnitPrice`   | Required, decimal, must be > 0                    |

### Notes

- Use FluentValidation (`AbstractValidator<Sale>`)
- Duplicate `ProductId` check: use `.Must()` rule on `Items`
- Follow the same pattern as `UserValidator` in the Users domain

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] All rules from spec §4 are implemented
- [ ] Duplicate `ProductId` check is present
- [ ] Reviewed by developer
- [ ] Committed with `feat(domain): add SaleValidator`
