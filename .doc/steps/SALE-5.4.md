# Step 5.4 — IoC: Dependency Injection Registration

**Branch:** `feature/SALE-5-api-endpoints`  
**Commit:** `feat(ioc): register Sales services in DI container`

---

## Files to Create / Modify

- Create or extend the Sales module initializer in `src/src/Ambev.DeveloperEvaluation.IoC/ModuleInitializers/`
- May also require modifying `src/src/Ambev.DeveloperEvaluation.IoC/DependencyResolver.cs` (or equivalent entry point — follow the template pattern)

---

## Registrations Required

| Service                  | Implementation       | Lifetime    |
|--------------------------|----------------------|-------------|
| `ISaleRepository`        | `SaleRepository`     | `Scoped`    |
| AutoMapper profiles      | All `Sales/*Profile` | (AutoMapper handles this via assembly scanning if already configured) |
| MediatR handlers         | All `Sales/*Handler` | (MediatR handles via assembly scanning if already configured)         |

---

## Notes

- Check how `IUserRepository` / `UserRepository` is registered — replicate the exact same pattern
- If the template uses assembly scanning for MediatR and AutoMapper, no per-handler registration is needed — only `ISaleRepository` → `SaleRepository` needs explicit registration
- Verify that the assembly containing `SaleRepository` is included in the scan

---

## Verification

After completing this step, the API should:
1. Compile with no errors
2. Start without runtime DI errors
3. All 7 endpoints should be reachable (smoke test via Swagger or curl)

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] `ISaleRepository` → `SaleRepository` registered
- [ ] API starts without DI errors
- [ ] Swagger shows all 7 Sales endpoints
- [ ] Reviewed by developer
- [ ] Committed with `feat(ioc): register Sales services in DI container`
