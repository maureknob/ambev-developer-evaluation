# Step 2.5 — Unit Tests: DeleteSaleHandler and GetSaleHandler

**Branch:** `feature/SALE-2-unit-tests`  
**Commit:** `test(application): add DeleteSale and GetSale handler unit tests`

> ⚠️ Tests will be **Red** until Module 3 (Application Layer) is complete.

---

## Files to Create

- `src/tests/Ambev.DeveloperEvaluation.Unit/Application/Sales/DeleteSaleHandlerTests.cs`
- `src/tests/Ambev.DeveloperEvaluation.Unit/Application/Sales/GetSaleHandlerTests.cs`

---

## Test Scenarios

### DeleteSaleHandler (spec §7.4 — A10–A11)

| ID  | Scenario         | Expected                        |
|-----|------------------|---------------------------------|
| A10 | Valid delete     | Returns success response        |
| A11 | Sale not found   | Throws `KeyNotFoundException`   |

### GetSaleHandler (spec §7.7 — A20–A21)

| ID  | Scenario       | Expected                          |
|-----|----------------|-----------------------------------|
| A20 | Valid Id       | Returns full `GetSaleResult`      |
| A21 | Not found      | Throws `KeyNotFoundException`     |

---

## Notes

- No new TestData file needed — reuse helpers from previous steps or inline simple builders
- Mock `ISaleRepository`: `GetByIdAsync` returns a sale for success cases, `null` for not-found cases
- For A10, verify `DeleteAsync` is called once
- For A20, verify the returned result maps all sale fields correctly (spot-check key fields)

---

## Definition of Done

- [ ] All 4 test cases written
- [ ] Tests compile (even if Red)
- [ ] Reviewed by developer
- [ ] Committed with `test(application): add DeleteSale and GetSale handler unit tests`
