# Step 5.3 — API: SalesController

**Branch:** `feature/SALE-5-api-endpoints`  
**Commit:** `feat(api): add SalesController`

---

## Files to Create

- `src/src/Ambev.DeveloperEvaluation.WebApi/Features/Sales/SalesController.cs`

---

## Endpoints (spec §3)

| Method   | Route                                   | Handler Command        | Success Code |
|----------|-----------------------------------------|------------------------|--------------|
| `POST`   | `/api/sales`                            | `CreateSaleCommand`    | `201`        |
| `GET`    | `/api/sales/{id}`                       | `GetSaleCommand`       | `200`        |
| `GET`    | `/api/sales`                            | `GetSalesCommand`      | `200`        |
| `PUT`    | `/api/sales/{id}`                       | `UpdateSaleCommand`    | `200`        |
| `DELETE` | `/api/sales/{id}`                       | `DeleteSaleCommand`    | `200`        |
| `PATCH`  | `/api/sales/{id}/cancel`                | `CancelSaleCommand`    | `200`        |
| `PATCH`  | `/api/sales/{id}/items/{itemId}/cancel` | `CancelSaleItemCommand`| `200`        |

## Error Handling

Map exceptions to HTTP status codes per spec §6:

| Exception                        | HTTP  |
|----------------------------------|-------|
| `ValidationException`            | `400` |
| `InvalidOperationException`      | `400` |
| `KeyNotFoundException`           | `404` |
| Duplicate `saleNumber`           | `409` |

Use the existing global exception handler / middleware from the template — do not handle exceptions inline in action methods.

## Response Envelope

All responses use `ApiResponse<T>` wrapper from the template:
```json
{ "success": true, "message": "...", "data": { ... } }
```

---

## Notes

- Inherit from the base controller in the template
- Inject `IMediator` via constructor
- Map request models → commands via AutoMapper before sending to MediatR
- Follow the same action method structure as `UsersController`

---

## Definition of Done

- [ ] Compiles with no errors
- [ ] All 7 endpoints implemented
- [ ] Correct HTTP status codes returned
- [ ] `ApiResponse<T>` wrapper used on all responses
- [ ] Reviewed by developer
- [ ] Committed with `feat(api): add SalesController`
