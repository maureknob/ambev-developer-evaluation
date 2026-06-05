# Step 4.3 — ORM: Register Sale in DefaultContext

**Branch:** `feature/SALE-4-orm-persistence`  
**Commit:** `chore(orm): register Sale DbSet and configuration in DefaultContext`

---

## Files to Modify

- `src/src/Ambev.DeveloperEvaluation.ORM/DefaultContext.cs`

---

## Changes Required

1. Add `DbSet<Sale>` property:
```csharp
public DbSet<Sale> Sales { get; set; }
```

2. Apply `SaleConfiguration` in `OnModelCreating`:
```csharp
modelBuilder.ApplyConfiguration(new SaleConfiguration());
```

---

## Notes

- Follow the exact same pattern used for `Users` in `DefaultContext`
- This step is a prerequisite for Step 4.4 (migration generation)

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] `DbSet<Sale>` added
- [ ] `SaleConfiguration` applied in `OnModelCreating`
- [ ] Reviewed by developer
- [ ] Committed with `chore(orm): register Sale DbSet and configuration in DefaultContext`
