# Step 8.1 — Rebus: Domain Event Publishing

**Branch:** `feature/SALE-8-rebus-events`  
**Commit:** `feat(application): publish domain events via Rebus`

---

## Context

The challenge lists **Rebus** in its frameworks. The four domain events (`SaleCreated`, `SaleModified`, `SaleCancelled`, `ItemCancelled`) are currently only logged via `ILogger`. This step wires them through Rebus using its **in-memory transport** (no real broker required — the challenge explicitly states publishing is not required, only the plumbing).

---

## Files to Modify

- `src/Ambev.DeveloperEvaluation.Application/Sales/CreateSale/CreateSaleHandler.cs`
- `src/Ambev.DeveloperEvaluation.Application/Sales/UpdateSale/UpdateSaleHandler.cs`
- `src/Ambev.DeveloperEvaluation.Application/Sales/CancelSale/CancelSaleHandler.cs`
- `src/Ambev.DeveloperEvaluation.Application/Sales/CancelSaleItem/CancelSaleItemHandler.cs`

---

## NuGet Packages

Add to `Application` project only:

```
Rebus
```

> `Rebus.ServiceProvider` is an IoC/host concern and belongs only in the `IoC` / `WebApi` project (SALE-8.2). The `Application` layer only needs the core `Rebus` package for the `IBus` abstraction.

---

## Handler Changes

Replace or supplement `ILogger` event logging with `IBus.Publish(...)`:

```csharp
// In CreateSaleHandler, after successful persist:
await _bus.Publish(new SaleCreatedEvent
{
    SaleId = sale.Id,
    SaleNumber = sale.SaleNumber,
    CustomerId = sale.CustomerId,
    TotalAmount = sale.TotalAmount,
    OccurredAt = DateTime.UtcNow
});
```

Apply the same pattern for:
- `UpdateSaleHandler` → publish `SaleModifiedEvent`
- `CancelSaleHandler` → publish `SaleCancelledEvent`
- `CancelSaleItemHandler` → publish `ItemCancelledEvent`

Keep the `ILogger` log alongside the publish (belt-and-suspenders).

---

## Notes

- The `IBus` is injected from DI — configured in SALE-8.2.
- If `IBus.Publish` fails, log the error but do not let it fail the handler (fire-and-forget semantics for events).

---

## Definition of Done

- [ ] All four handlers publish their respective events via `IBus`
- [ ] `ILogger` calls retained alongside
- [ ] Event publish errors swallowed with log
- [ ] Compiles with no errors
- [ ] Reviewed by developer
- [ ] Committed with `feat(application): publish domain events via Rebus`
