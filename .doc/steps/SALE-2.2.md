# Step 2.2 — Unit Tests: CreateSaleHandler

**Branch:** `feature/SALE-2-unit-tests`  
**Commit:** `test(application): add CreateSaleHandler unit tests`

> ⚠️ Tests will be **Red** until Module 3 (Application Layer) is complete.

---

## Files to Create

- `src/tests/Ambev.DeveloperEvaluation.Unit/Application/Sales/CreateSaleHandlerTests.cs`
- `src/tests/Ambev.DeveloperEvaluation.Unit/Application/Sales/TestData/CreateSaleHandlerTestData.cs`

---

## Test Scenarios (spec §7.2 — A01–A05)

| ID  | Scenario                         | Expected                                          |
|-----|----------------------------------|---------------------------------------------------|
| A01 | Valid command                    | Returns `CreateSaleResult` with correct Id        |
| A02 | Command with invalid fields      | Throws `ValidationException`                      |
| A03 | Duplicate `saleNumber`           | Throws `InvalidOperationException`                |
| A04 | Repository called once           | `CreateAsync` called exactly once                 |
| A05 | Event published on success       | `SaleCreatedEvent` logged                         |

---

## Notes

- Mock `ISaleRepository` using NSubstitute (consistent with template)
- Mock `ILogger` to verify event logging for A05
- `CreateSaleHandlerTestData.cs` should provide a valid `CreateSaleCommand` factory method and an invalid one (missing required fields)
- Follow the same handler test structure as `CreateUserHandlerTests` in the Users domain

---

## Definition of Done

- [ ] All 5 test cases written
- [ ] Tests compile (even if Red)
- [ ] Mocks set up correctly
- [ ] Reviewed by developer
- [ ] Committed with `test(application): add CreateSaleHandler unit tests`
