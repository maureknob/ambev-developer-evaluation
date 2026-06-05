# Step 4.4 — ORM: EF Core Migration

**Branch:** `feature/SALE-4-orm-persistence`  
**Commit:** `chore(orm): add EF Core migration for Sales and SaleItems tables`

---

## Action Required

Generate and verify the EF Core migration for the Sales schema.

```bash
dotnet ef migrations add AddSalesTables \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

Then verify the generated migration matches the expected schema from Step 4.1.

---

## Migration Must Include

- `Sales` table with all columns and the `SaleNumber` unique index
- `SaleItems` table with FK to `Sales` and CASCADE DELETE
- No unexpected changes to existing tables (Users domain must be untouched)

---

## Notes

- Do **not** apply the migration to a production database — the migration file is committed, and `dotnet ef database update` is run locally for development
- If the migration touches existing tables unexpectedly, investigate `DefaultContext` before committing

---

## Definition of Done

- [ ] Migration generated without errors
- [ ] Generated migration reviewed — matches expected schema
- [ ] No unintended changes to existing tables
- [ ] Migration files committed
- [ ] Committed with `chore(orm): add EF Core migration for Sales and SaleItems tables`
