# Step 4.1 — ORM: SaleConfiguration (EF Core Mapping)

**Branch:** `feature/SALE-4-orm-persistence`  
**Commit:** `feat(orm): add EF Core mapping for Sale and SaleItem`

---

## Files to Create

- `src/src/Ambev.DeveloperEvaluation.ORM/Mapping/SaleConfiguration.cs`

---

## Mapping Requirements

### Sale table: `Sales`

| Column         | Type           | Constraints                  |
|----------------|----------------|------------------------------|
| `Id`           | `uuid`         | PK                           |
| `SaleNumber`   | `varchar(50)`  | NOT NULL, UNIQUE             |
| `SaleDate`     | `timestamp`    | NOT NULL                     |
| `CustomerId`   | `uuid`         | NOT NULL                     |
| `CustomerName` | `varchar(100)` | NOT NULL                     |
| `BranchId`     | `uuid`         | NOT NULL                     |
| `BranchName`   | `varchar(100)` | NOT NULL                     |
| `TotalAmount`  | `decimal(18,2)`| NOT NULL                     |
| `IsCancelled`  | `bool`         | NOT NULL, default false      |
| `CreatedAt`    | `timestamp`    | NOT NULL                     |
| `UpdatedAt`    | `timestamp`    | nullable                     |

### SaleItem table: `SaleItems`

| Column        | Type           | Constraints                  |
|---------------|----------------|------------------------------|
| `Id`          | `uuid`         | PK                           |
| `SaleId`      | `uuid`         | FK → Sales.Id, CASCADE DELETE|
| `ProductId`   | `uuid`         | NOT NULL                     |
| `ProductName` | `varchar(100)` | NOT NULL                     |
| `Quantity`    | `int`          | NOT NULL                     |
| `UnitPrice`   | `decimal(18,2)`| NOT NULL                     |
| `Discount`    | `decimal(5,2)` | NOT NULL                     |
| `TotalAmount` | `decimal(18,2)`| NOT NULL                     |
| `IsCancelled` | `bool`         | NOT NULL, default false      |

### Relationships

- `Sale` has many `SaleItem` (one-to-many, owned navigation)
- Cascade delete: deleting a `Sale` deletes its `SaleItem` records

---

## Notes

- Implement `IEntityTypeConfiguration<Sale>`
- Follow the same style as existing configuration classes in the template
- `SaleNumber` unique index: `.HasIndex(s => s.SaleNumber).IsUnique()`

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] All columns and constraints match the table above
- [ ] `SaleNumber` unique index configured
- [ ] FK relationship with cascade delete configured
- [ ] Reviewed by developer
- [ ] Committed with `feat(orm): add EF Core mapping for Sale and SaleItem`
