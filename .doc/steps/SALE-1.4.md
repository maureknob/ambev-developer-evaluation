# Step 1.4 — Domain Events

**Branch:** `feature/SALE-1-domain-entities`  
**Commit:** `feat(domain): add Sale domain events`

---

## Files to Create

- `src/src/Ambev.DeveloperEvaluation.Domain/Events/SaleCreatedEvent.cs`
- `src/src/Ambev.DeveloperEvaluation.Domain/Events/SaleModifiedEvent.cs`
- `src/src/Ambev.DeveloperEvaluation.Domain/Events/SaleCancelledEvent.cs`
- `src/src/Ambev.DeveloperEvaluation.Domain/Events/ItemCancelledEvent.cs`

---

## Event Shapes (spec §5)

### SaleCreatedEvent
```csharp
public Guid SaleId { get; init; }
public string SaleNumber { get; init; }
public Guid CustomerId { get; init; }
public decimal TotalAmount { get; init; }
public DateTime OccurredAt { get; init; }
```

### SaleModifiedEvent
```csharp
public Guid SaleId { get; init; }
public string SaleNumber { get; init; }
public decimal PreviousTotalAmount { get; init; }
public decimal NewTotalAmount { get; init; }
public DateTime OccurredAt { get; init; }
```

### SaleCancelledEvent
```csharp
public Guid SaleId { get; init; }
public string SaleNumber { get; init; }
public DateTime OccurredAt { get; init; }
```

### ItemCancelledEvent
```csharp
public Guid SaleId { get; init; }
public Guid ItemId { get; init; }
public Guid ProductId { get; init; }
public string ProductName { get; init; }
public DateTime OccurredAt { get; init; }
```

### Notes

- These are plain data records — no behavior
- All events are logged via `ILogger` in the handlers (spec §5); no real message broker
- Use `record` or `class` with `init` properties, consistent with the template

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] All 4 events match spec §5 exactly
- [ ] Reviewed by developer
- [ ] Committed with `feat(domain): add Sale domain events`
