# Step 2.3 — Unit Tests: UpdateSaleHandler

**Branch:** `feature/SALE-2-unit-tests`  
**Commit:** `test(application): add UpdateSaleHandler unit tests`

> ⚠️ Tests will be **Red** until Module 3 (Application Layer) is complete.

---

## Files to Create

- `src/tests/Ambev.DeveloperEvaluation.Unit/Application/Sales/UpdateSaleHandlerTests.cs`
- `src/tests/Ambev.DeveloperEvaluation.Unit/Application/Sales/TestData/UpdateSaleHandlerTestData.cs`

---

## Test Scenarios (spec §7.3 — A06–A09)

| ID  | Scenario                        | Expected                                        |
|-----|---------------------------------|-------------------------------------------------|
| A06 | Valid update                    | Returns updated `UpdateSaleResult`              |
| A07 | Sale not found                  | Throws `KeyNotFoundException`                   |
| A08 | Update cancelled sale           | Throws `InvalidOperationException`              |
| A09 | `SaleModified` event logged     | Event logged on success                         |

---

## Notes

- Mock `ISaleRepository` — `GetByIdAsync` returns existing sale for A06/A08/A09, returns `null` for A07
- `UpdateSaleHandlerTestData.cs` should provide: valid `UpdateSaleCommand`, a cancelled sale entity
- Verify `UpdateAsync` is called once on success (A06)
- Follow the same handler test structure as the Users domain

---

## Definition of Done

- [ ] All 4 test cases written
- [ ] Tests compile (even if Red)
- [ ] Reviewed by developer
- [ ] Committed with `test(application): add UpdateSaleHandler unit tests`
