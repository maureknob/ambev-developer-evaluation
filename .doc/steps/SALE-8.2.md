# Step 8.2 — Rebus: DI Registration (In-Memory Transport)

**Branch:** `feature/SALE-8-rebus-events`  
**Commit:** `feat(ioc): register Rebus with in-memory transport for domain event publishing`

---

## Files to Modify

- `src/Ambev.DeveloperEvaluation.IoC/ModuleInitializers/ApplicationModuleInitializer.cs` (or equivalent)
- `src/Ambev.DeveloperEvaluation.WebApi/appsettings.json` (no new config needed for in-memory)

---

## NuGet Packages

Add to `IoC` / `WebApi` project:

```
Rebus.ServiceProvider
```

---

## DI Registration

```csharp
services.AddRebus(configure => configure
    .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "sales-events"))
    .Logging(l => l.MicrosoftExtensionsLogging(loggerFactory))
);

services.AutoRegisterHandlersFromAssemblyOf<CreateSaleHandler>();
```

> `InMemNetwork` keeps messages in process — no external broker needed. Swap for RabbitMQ/Azure Service Bus by changing only the transport line.

---

## Notes

- `AddRebus` must be called before `app.UseRebus()` (or `services.AddRebusHandler<>` pattern).
- `UseInMemoryTransport` requires the `Rebus` package (core); `Rebus.ServiceProvider` provides `AddRebus`.
- There are no Rebus message handlers to register for this evaluation (publish-only); the handler registration call is a no-op but demonstrates correct Rebus setup.

---

## Definition of Done

- [ ] Rebus registered with in-memory transport
- [ ] `IBus` can be resolved from DI in handlers
- [ ] App starts without errors
- [ ] Reviewed by developer
- [ ] Committed with `feat(ioc): register Rebus with in-memory transport for domain event publishing`
