# Step 1.1 — Domain Entities: Sale and SaleItem

**Branch:** `feature/SALE-1-domain-entities`  
**Commit:** `feat(domain): add Sale and SaleItem entities`

---

## Files to Create

- `src/src/Ambev.DeveloperEvaluation.Domain/Entities/SaleItem.cs`
- `src/src/Ambev.DeveloperEvaluation.Domain/Entities/Sale.cs`

---

## SaleItem

Fields (spec §1.2):

| Field         | Type      | Notes                          |
|---------------|-----------|--------------------------------|
| `Id`          | `Guid`    | Auto-generated                 |
| `SaleId`      | `Guid`    | FK to parent Sale              |
| `ProductId`   | `Guid`    |                                |
| `ProductName` | `string`  |                                |
| `Quantity`    | `int`     |                                |
| `UnitPrice`   | `decimal` |                                |
| `Discount`    | `decimal` | Computed — never set by caller |
| `TotalAmount` | `decimal` | Computed — never set by caller |
| `IsCancelled` | `bool`    |                                |

Discount tiers (spec §2.1):

| Quantity  | Discount |
|-----------|----------|
| 1–3       | 0%       |
| 4–9       | 10%      |
| 10–20     | 20%      |
| > 20      | throws `InvalidOperationException` |

Calculation (spec §2.2):
```
Discount    = DetermineDiscount(Quantity)
TotalAmount = Quantity × UnitPrice × (1 - Discount)
```

---

## Sale

Fields (spec §1.1):

| Field          | Type             | Notes                    |
|----------------|------------------|--------------------------|
| `Id`           | `Guid`           | Auto-generated           |
| `SaleNumber`   | `string`         |                          |
| `SaleDate`     | `DateTime`       | UTC                      |
| `CustomerId`   | `Guid`           |                          |
| `CustomerName` | `string`         |                          |
| `BranchId`     | `Guid`           |                          |
| `BranchName`   | `string`         |                          |
| `Items`        | `List<SaleItem>` | Min 1                    |
| `TotalAmount`  | `decimal`        | Computed — never set directly |
| `IsCancelled`  | `bool`           |                          |
| `CreatedAt`    | `DateTime`       | UTC                      |
| `UpdatedAt`    | `DateTime?`      | UTC                      |

`TotalAmount` rule (spec §2.3):
```
Sale.TotalAmount = SUM(item.TotalAmount) for all non-cancelled items
```

Methods to implement:

- `AddItem(SaleItem)` — enforces no duplicate `ProductId` (spec §2.5); throws if sale is cancelled (spec §2.4)
- `Cancel()` — sets `IsCancelled = true` on sale and all items; throws if already cancelled (spec §2.4)
- `CancelItem(Guid itemId)` — sets item `IsCancelled = true`; recalculates `TotalAmount`; throws if sale or item already cancelled (spec §2.4)

---

## Test Scenarios Covered by This Step

From spec §7.1 and §7.8 — these tests must pass after Step 3 (Application Layer), but the domain logic lives here:

| ID  | Scenario |
|-----|----------|
| D01 | qty=3, price=10 → TotalAmount=30, Discount=0 |
| D02 | qty=4, price=10 → TotalAmount=36, Discount=0.10 |
| D03 | qty=10, price=10 → TotalAmount=80, Discount=0.20 |
| D04 | qty=20, price=10 → TotalAmount=160, Discount=0.20 |
| D05 | qty=21 → throws `InvalidOperationException` |
| D08 | Cancel sale → IsCancelled=true on sale and all items |
| D09 | Cancel already-cancelled sale → throws |
| D10 | Cancel single item → item cancelled, TotalAmount recalculated |
| D11 | Cancel already-cancelled item → throws |
| D12 | Modify cancelled sale → throws |
| D13 | TotalAmount excludes cancelled items |
| D14 | Duplicate productId → throws |
| B01–B07 | Discount tier boundary values |

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] Follows naming and structure conventions from the Users domain
- [ ] All business rules from spec §2 are enforced in the entity methods
- [ ] `Discount` and `TotalAmount` on `SaleItem` are always computed, never settable by caller
- [ ] `TotalAmount` on `Sale` is always derived, never settable by caller
- [ ] Reviewed by developer
- [ ] Committed with `feat(domain): add Sale and SaleItem entities`
