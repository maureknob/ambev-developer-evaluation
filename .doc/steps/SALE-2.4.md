# Step 2.4 — Unit Tests: CancelSaleHandler and CancelSaleItemHandler

**Branch:** `feature/SALE-2-unit-tests`  
**Commit:** `test(application): add CancelSale and CancelSaleItem handler unit tests`

> ⚠️ Tests will be **Red** until Module 3 (Application Layer) is complete.

---

## Files to Create

- `src/tests/Ambev.DeveloperEvaluation.Unit/Application/Sales/CancelSaleHandlerTests.cs`
- `src/tests/Ambev.DeveloperEvaluation.Unit/Application/Sales/CancelSaleItemHandlerTests.cs`
- `src/tests/Ambev.DeveloperEvaluation.Unit/Application/Sales/TestData/CancelSaleHandlerTestData.cs`

---

## Test Scenarios

### CancelSaleHandler (spec §7.5 — A12–A14)

| ID  | Scenario               | Expected                                              |
|-----|------------------------|-------------------------------------------------------|
| A12 | Valid cancellation     | IsCancelled=true, `SaleCancelledEvent` logged         |
| A13 | Sale not found         | Throws `KeyNotFoundException`                         |
| A14 | Already cancelled      | Throws `InvalidOperationException`                    |

### CancelSaleItemHandler (spec §7.6 — A15–A19)

| ID  | Scenario                    | Expected                                              |
|-----|-----------------------------|-------------------------------------------------------|
| A15 | Valid item cancellation     | Item cancelled, total recalculated, `ItemCancelledEvent` logged |
| A16 | Sale not found              | Throws `KeyNotFoundException`                         |
| A17 | Item not found              | Throws `KeyNotFoundException`                         |
| A18 | Sale already cancelled      | Throws `InvalidOperationException`                    |
| A19 | Item already cancelled      | Throws `InvalidOperationException`                    |

---

## Notes

- `CancelSaleHandlerTestData.cs` covers both handlers — provide factory methods for: active sale, cancelled sale, sale with one active item, sale with one cancelled item
- Mock `ISaleRepository` for all scenarios
- Verify `UpdateAsync` is called once on success (A12, A15)

---

## Definition of Done

- [ ] All 8 test cases written
- [ ] Tests compile (even if Red)
- [ ] Shared test data covers all scenarios
- [ ] Reviewed by developer
- [ ] Committed with `test(application): add CancelSale and CancelSaleItem handler unit tests`
