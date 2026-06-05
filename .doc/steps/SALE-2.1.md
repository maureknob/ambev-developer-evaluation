# Step 2.1 — Unit Tests: Sale Domain Entity

**Branch:** `feature/SALE-2-unit-tests`  
**Commit:** `test(domain): add Sale entity unit tests`

> ⚠️ Tests will be **Red** until Module 3 (Application Layer) is complete. This is intentional — TDD Red phase.

---

## Files to Create

- `src/tests/Ambev.DeveloperEvaluation.Unit/Domain/Entities/SaleTests.cs`
- `src/tests/Ambev.DeveloperEvaluation.Unit/Domain/Entities/TestData/SaleTestData.cs`

---

## Test Scenarios to Cover

### Discount Tiers (spec §7.8 — B01–B07)

| ID  | Input     | Expected                       |
|-----|-----------|--------------------------------|
| B01 | qty=1     | Discount=0%                    |
| B02 | qty=3     | Discount=0%                    |
| B03 | qty=4     | Discount=10%                   |
| B04 | qty=9     | Discount=10%                   |
| B05 | qty=10    | Discount=20%                   |
| B06 | qty=20    | Discount=20%                   |
| B07 | qty=21    | throws `InvalidOperationException` |

### Sale Entity (spec §7.1 — D01–D14)

| ID  | Scenario                                              | Expected                                         |
|-----|-------------------------------------------------------|--------------------------------------------------|
| D01 | 1 item, qty=3, price=10                               | TotalAmount=30, Discount=0                       |
| D02 | 1 item, qty=4, price=10                               | TotalAmount=36, Discount=0.10                    |
| D03 | 1 item, qty=10, price=10                              | TotalAmount=80, Discount=0.20                    |
| D04 | 1 item, qty=20, price=10                              | TotalAmount=160, Discount=0.20                   |
| D05 | Item qty=21                                           | throws `InvalidOperationException`               |
| D06 | Item qty=0                                            | throws validation error                          |
| D07 | Item unitPrice=0                                      | throws validation error                          |
| D08 | Cancel a sale                                         | IsCancelled=true on sale and all items           |
| D09 | Cancel already-cancelled sale                         | throws `InvalidOperationException`               |
| D10 | Cancel a single item                                  | item IsCancelled=true, TotalAmount recalculated  |
| D11 | Cancel already-cancelled item                         | throws `InvalidOperationException`               |
| D12 | Modify a cancelled sale (add item)                    | throws `InvalidOperationException`               |
| D13 | TotalAmount with one cancelled item                   | Only active items counted                        |
| D14 | Add item with duplicate productId                     | throws `InvalidOperationException`               |

---

## Notes

- Use `SaleTestData.cs` for reusable factory methods (valid sale, valid item, etc.)
- Follow the same test structure as existing User entity tests in the template
- Use xUnit (`[Fact]`, `[Theory]`) and FluentAssertions

---

## Definition of Done

- [ ] All 21 test cases written
- [ ] Tests compile (even if Red)
- [ ] Test data helpers cover all boundary values
- [ ] Reviewed by developer
- [ ] Committed with `test(domain): add Sale entity unit tests`
