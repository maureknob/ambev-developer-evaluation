# Sales Domain Specification

> **Status:** Draft v1.0  
> **Purpose:** Authoritative spec for spec-driven development of the Sales domain.  
> All tests, implementations, and reviews must trace back to this document.

---

## Table of Contents

1. [Domain Model](#1-domain-model)
2. [Business Rules](#2-business-rules)
3. [API Contract](#3-api-contract)
4. [Validation Rules](#4-validation-rules)
5. [Domain Events](#5-domain-events)
6. [Error Handling](#6-error-handling)
7. [Test Scenarios](#7-test-scenarios)
8. [Architecture & File Map](#8-architecture--file-map)

---

## 1. Domain Model

### 1.1 Sale (Aggregate Root)

| Field          | Type          | Nullable | Description                                      |
|----------------|---------------|----------|--------------------------------------------------|
| `Id`           | `Guid`        | No       | Unique identifier (auto-generated)               |
| `SaleNumber`   | `string`      | No       | Human-readable sale identifier (e.g. "SALE-001") |
| `SaleDate`     | `DateTime`    | No       | UTC date/time when the sale was made             |
| `CustomerId`   | `Guid`        | No       | External identity — customer reference           |
| `CustomerName` | `string`      | No       | Denormalized customer name (External Identity)   |
| `BranchId`     | `Guid`        | No       | External identity — branch reference             |
| `BranchName`   | `string`      | No       | Denormalized branch name (External Identity)     |
| `Items`        | `List<SaleItem>` | No    | One or more sale items (min: 1)                  |
| `TotalAmount`  | `decimal`     | No       | Sum of all `SaleItem.TotalAmount` (computed)     |
| `IsCancelled`  | `bool`        | No       | Whether the entire sale has been cancelled       |
| `CreatedAt`    | `DateTime`    | No       | UTC creation timestamp                           |
| `UpdatedAt`    | `DateTime?`   | Yes      | UTC last-update timestamp                        |

> **Note:** `TotalAmount` is always derived — never set directly. It is recalculated whenever items change.

### 1.2 SaleItem (Entity)

| Field          | Type      | Nullable | Description                                                   |
|----------------|-----------|----------|---------------------------------------------------------------|
| `Id`           | `Guid`    | No       | Unique identifier (auto-generated)                            |
| `SaleId`       | `Guid`    | No       | FK to parent `Sale`                                           |
| `ProductId`    | `Guid`    | No       | External identity — product reference                         |
| `ProductName`  | `string`  | No       | Denormalized product name (External Identity)                 |
| `Quantity`     | `int`     | No       | Number of units (1–20)                                        |
| `UnitPrice`    | `decimal` | No       | Price per unit at time of sale (> 0)                          |
| `Discount`     | `decimal` | No       | Discount percentage applied (0, 0.10, or 0.20)               |
| `TotalAmount`  | `decimal` | No       | Computed: `Quantity × UnitPrice × (1 - Discount)`            |
| `IsCancelled`  | `bool`    | No       | Whether this specific item has been cancelled                 |

> **Note:** `Discount` and `TotalAmount` are always derived from `Quantity` via business rules. They are never set directly by the caller.

---

## 2. Business Rules

### 2.1 Discount Tiers (applied per SaleItem)

| Quantity        | Discount | Rule                                      |
|-----------------|----------|-------------------------------------------|
| 1 – 3 items     | 0%       | No discount allowed                       |
| 4 – 9 items     | 10%      | Discount automatically applied            |
| 10 – 20 items   | 20%      | Discount automatically applied            |
| > 20 items      | —        | **Not allowed** — must throw domain error |

### 2.2 Item Calculation

```
Discount     = DetermineDiscount(Quantity)   // per table above
TotalAmount  = Quantity × UnitPrice × (1 - Discount)
```

### 2.3 Sale Total Calculation

```
Sale.TotalAmount = SUM(item.TotalAmount) for all non-cancelled items
```

### 2.4 Cancellation Rules

- A **Sale** can be cancelled at any time (sets `IsCancelled = true`, raises `SaleCancelled` event).
- A **SaleItem** can be individually cancelled (sets `item.IsCancelled = true`, raises `ItemCancelled` event).
- Cancelled items are **excluded** from `Sale.TotalAmount` recalculation.
- A cancelled sale **cannot** be modified (adding/removing items must throw domain error).
- A cancelled sale **cannot** be re-activated.

### 2.5 Duplicate Products

- A sale **may not** contain two `SaleItem` entries with the same `ProductId`.  
  Callers must consolidate quantities before submitting.

### 2.6 Sale Number

- `SaleNumber` must be **unique** across all sales.
- Format is free-form string (the caller provides it); the API does not auto-generate it.

---

## 3. API Contract

Base path: `/api/sales`

---

### 3.1 Create Sale

**`POST /api/sales`**

#### Request Body

```json
{
  "saleNumber": "SALE-2024-001",
  "saleDate": "2024-10-15T14:30:00Z",
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "customerName": "John Doe",
  "branchId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "branchName": "Branch North",
  "items": [
    {
      "productId": "b5a4e3f2-1234-5678-9abc-def012345678",
      "productName": "Widget A",
      "quantity": 10,
      "unitPrice": 25.00
    }
  ]
}
```

#### Validation
- See [Section 4 — Validation Rules](#4-validation-rules)

#### Success Response — `201 Created`

```json
{
  "success": true,
  "message": "Sale created successfully",
  "data": {
    "id": "...",
    "saleNumber": "SALE-2024-001",
    "saleDate": "2024-10-15T14:30:00Z",
    "customerId": "...",
    "customerName": "John Doe",
    "branchId": "...",
    "branchName": "Branch North",
    "totalAmount": 200.00,
    "isCancelled": false,
    "createdAt": "...",
    "items": [
      {
        "id": "...",
        "productId": "...",
        "productName": "Widget A",
        "quantity": 10,
        "unitPrice": 25.00,
        "discount": 0.20,
        "totalAmount": 200.00,
        "isCancelled": false
      }
    ]
  }
}
```

#### Error Responses
| HTTP Code | Condition |
|-----------|-----------|
| `400`     | Validation failure (missing fields, quantity > 20, etc.) |
| `409`     | `saleNumber` already exists |

---

### 3.2 Get Sale by ID

**`GET /api/sales/{id}`**

#### Path Parameter
| Name | Type   | Description         |
|------|--------|---------------------|
| `id` | `Guid` | The sale's unique ID |

#### Success Response — `200 OK`

Same shape as Create Sale `data` object.

#### Error Responses
| HTTP Code | Condition |
|-----------|-----------|
| `400`     | Invalid GUID format |
| `404`     | Sale not found |

---

### 3.3 Update Sale

**`PUT /api/sales/{id}`**

Updates sale header fields and replaces the full item list.

#### Request Body

Same shape as Create Sale request body (all fields required).

#### Rules
- Cannot update a cancelled sale → `400`
- `saleNumber` uniqueness enforced (excluding current sale) → `409`
- Business rules re-applied to all items

#### Success Response — `200 OK`

```json
{
  "success": true,
  "message": "Sale updated successfully",
  "data": { /* full sale object */ }
}
```

#### Error Responses
| HTTP Code | Condition |
|-----------|-----------|
| `400`     | Validation failure or sale is cancelled |
| `404`     | Sale not found |
| `409`     | `saleNumber` already taken by another sale |

---

### 3.4 Delete Sale

**`DELETE /api/sales/{id}`**

Permanently removes the sale record.

#### Success Response — `200 OK`

```json
{
  "success": true,
  "message": "Sale deleted successfully"
}
```

#### Error Responses
| HTTP Code | Condition |
|-----------|-----------|
| `400`     | Invalid GUID |
| `404`     | Sale not found |

---

### 3.5 Cancel Sale

**`PATCH /api/sales/{id}/cancel`**

Cancels the entire sale. Sets `IsCancelled = true` on the sale and all its items.

#### Success Response — `200 OK`

```json
{
  "success": true,
  "message": "Sale cancelled successfully",
  "data": { /* full sale object with isCancelled: true */ }
}
```

#### Error Responses
| HTTP Code | Condition |
|-----------|-----------|
| `400`     | Sale is already cancelled |
| `404`     | Sale not found |

---

### 3.6 Cancel Sale Item

**`PATCH /api/sales/{id}/items/{itemId}/cancel`**

Cancels a single item within a sale. Recalculates `Sale.TotalAmount`.

#### Success Response — `200 OK`

```json
{
  "success": true,
  "message": "Item cancelled successfully",
  "data": { /* full sale object with updated totalAmount */ }
}
```

#### Error Responses
| HTTP Code | Condition |
|-----------|-----------|
| `400`     | Sale is cancelled, or item is already cancelled |
| `404`     | Sale or item not found |

---

### 3.7 List Sales (Paginated)

**`GET /api/sales?page=1&size=10&order=saleDate desc`**

#### Query Parameters

| Name    | Type     | Default | Description                          |
|---------|----------|---------|--------------------------------------|
| `page`  | `int`    | `1`     | Page number (1-based)                |
| `size`  | `int`    | `10`    | Items per page (max: 100)            |
| `order` | `string` | `"saleDate desc"` | Field + direction to sort  |

#### Success Response — `200 OK`

```json
{
  "success": true,
  "message": "Sales retrieved successfully",
  "data": [ /* array of sale objects */ ],
  "totalItems": 42,
  "currentPage": 1,
  "totalPages": 5
}
```

---

## 4. Validation Rules

### 4.1 Sale-Level

| Field          | Rule                                                       |
|----------------|------------------------------------------------------------|
| `saleNumber`   | Required, non-empty, max 50 chars                          |
| `saleDate`     | Required, valid UTC datetime                               |
| `customerId`   | Required, non-empty GUID                                   |
| `customerName` | Required, non-empty, max 100 chars                         |
| `branchId`     | Required, non-empty GUID                                   |
| `branchName`   | Required, non-empty, max 100 chars                         |
| `items`        | Required, must contain at least 1 item                     |
| `items`        | No duplicate `productId` values within the same sale       |

### 4.2 SaleItem-Level

| Field         | Rule                                                        |
|---------------|-------------------------------------------------------------|
| `productId`   | Required, non-empty GUID                                    |
| `productName` | Required, non-empty, max 100 chars                          |
| `quantity`    | Required, integer, min: 1, max: 20                          |
| `unitPrice`   | Required, decimal, must be > 0                              |
| `discount`    | **Not accepted from caller** — always computed by domain    |
| `totalAmount` | **Not accepted from caller** — always computed by domain    |

---

## 5. Domain Events

All events are logged via `ILogger` (no real broker required for this implementation).

### 5.1 SaleCreated

Raised when: `CreateSaleHandler` successfully persists a new sale.

```csharp
public class SaleCreatedEvent
{
    public Guid SaleId { get; init; }
    public string SaleNumber { get; init; }
    public Guid CustomerId { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime OccurredAt { get; init; }
}
```

### 5.2 SaleModified

Raised when: `UpdateSaleHandler` successfully updates a sale.

```csharp
public class SaleModifiedEvent
{
    public Guid SaleId { get; init; }
    public string SaleNumber { get; init; }
    public decimal PreviousTotalAmount { get; init; }
    public decimal NewTotalAmount { get; init; }
    public DateTime OccurredAt { get; init; }
}
```

### 5.3 SaleCancelled

Raised when: `CancelSaleHandler` successfully cancels a sale.

```csharp
public class SaleCancelledEvent
{
    public Guid SaleId { get; init; }
    public string SaleNumber { get; init; }
    public DateTime OccurredAt { get; init; }
}
```

### 5.4 ItemCancelled

Raised when: `CancelSaleItemHandler` successfully cancels a single item.

```csharp
public class ItemCancelledEvent
{
    public Guid SaleId { get; init; }
    public Guid ItemId { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; }
    public DateTime OccurredAt { get; init; }
}
```

---

## 6. Error Handling

All errors follow the existing `ApiResponse` pattern from the template:

```json
{
  "success": false,
  "message": "Human-readable summary",
  "errors": [
    { "error": "FieldName", "detail": "Specific problem description" }
  ]
}
```

### Exception → HTTP Status Mapping

| Exception Type                  | HTTP Status | Usage                                      |
|---------------------------------|-------------|--------------------------------------------|
| `FluentValidation.ValidationException` | `400` | Field-level validation failures       |
| `InvalidOperationException`     | `400`       | Business rule violation (e.g. cancelled sale, quantity > 20) |
| `KeyNotFoundException`          | `404`       | Sale or item not found                     |
| `DomainException`               | `422`       | Domain invariant violation (reserved)      |

---

## 7. Test Scenarios

### 7.1 Domain Entity — `Sale`

| # | Scenario | Expected |
|---|----------|----------|
| D01 | Create sale with 1 item, qty=3, price=10 | `TotalAmount=30`, `Discount=0` |
| D02 | Create sale with 1 item, qty=4, price=10 | `TotalAmount=36`, `Discount=0.10` |
| D03 | Create sale with 1 item, qty=10, price=10 | `TotalAmount=80`, `Discount=0.20` |
| D04 | Create sale with 1 item, qty=20, price=10 | `TotalAmount=160`, `Discount=0.20` |
| D05 | Create sale item with qty=21 | Throws `InvalidOperationException` |
| D06 | Create sale item with qty=0 | Throws validation error |
| D07 | Create sale item with unitPrice=0 | Throws validation error |
| D08 | Cancel a sale | `IsCancelled=true`, all items `IsCancelled=true` |
| D09 | Cancel already-cancelled sale | Throws `InvalidOperationException` |
| D10 | Cancel a single item | Item `IsCancelled=true`, `Sale.TotalAmount` recalculated |
| D11 | Cancel already-cancelled item | Throws `InvalidOperationException` |
| D12 | Modify a cancelled sale | Throws `InvalidOperationException` |
| D13 | `TotalAmount` excludes cancelled items | Only active items counted |
| D14 | Sale with duplicate productId | Throws `InvalidOperationException` |

### 7.2 Application Handler — `CreateSaleHandler`

| # | Scenario | Expected |
|---|----------|----------|
| A01 | Valid command | Returns `CreateSaleResult` with correct Id |
| A02 | Command with invalid fields | Throws `ValidationException` |
| A03 | Duplicate `saleNumber` | Throws `InvalidOperationException` |
| A04 | Repository called once | `CreateAsync` called exactly once |
| A05 | Event published on success | `SaleCreatedEvent` logged |

### 7.3 Application Handler — `UpdateSaleHandler`

| # | Scenario | Expected |
|---|----------|----------|
| A06 | Valid update | Returns updated `UpdateSaleResult` |
| A07 | Sale not found | Throws `KeyNotFoundException` |
| A08 | Update cancelled sale | Throws `InvalidOperationException` |
| A09 | `SaleModified` event logged | Event logged on success |

### 7.4 Application Handler — `DeleteSaleHandler`

| # | Scenario | Expected |
|---|----------|----------|
| A10 | Valid delete | Returns success response |
| A11 | Sale not found | Throws `KeyNotFoundException` |

### 7.5 Application Handler — `CancelSaleHandler`

| # | Scenario | Expected |
|---|----------|----------|
| A12 | Valid cancellation | `IsCancelled=true`, `SaleCancelledEvent` logged |
| A13 | Sale not found | Throws `KeyNotFoundException` |
| A14 | Already cancelled | Throws `InvalidOperationException` |

### 7.6 Application Handler — `CancelSaleItemHandler`

| # | Scenario | Expected |
|---|----------|----------|
| A15 | Valid item cancellation | Item cancelled, total recalculated, `ItemCancelledEvent` logged |
| A16 | Sale not found | Throws `KeyNotFoundException` |
| A17 | Item not found | Throws `KeyNotFoundException` |
| A18 | Sale already cancelled | Throws `InvalidOperationException` |
| A19 | Item already cancelled | Throws `InvalidOperationException` |

### 7.7 Application Handler — `GetSaleHandler`

| # | Scenario | Expected |
|---|----------|----------|
| A20 | Valid Id | Returns full `GetSaleResult` |
| A21 | Not found | Throws `KeyNotFoundException` |

### 7.8 Discount Business Rule (unit)

| # | Scenario | Expected |
|---|----------|----------|
| B01 | qty=1 → discount | 0% |
| B02 | qty=3 → discount | 0% |
| B03 | qty=4 → discount | 10% |
| B04 | qty=9 → discount | 10% |
| B05 | qty=10 → discount | 20% |
| B06 | qty=20 → discount | 20% |
| B07 | qty=21 → throws | `InvalidOperationException` |

---

## 8. Architecture & File Map

Following the existing template conventions exactly:

```
src/src/
├── Ambev.DeveloperEvaluation.Domain/
│   ├── Entities/
│   │   ├── Sale.cs
│   │   └── SaleItem.cs
│   ├── Repositories/
│   │   └── ISaleRepository.cs
│   ├── Events/
│   │   ├── SaleCreatedEvent.cs
│   │   ├── SaleModifiedEvent.cs
│   │   ├── SaleCancelledEvent.cs
│   │   └── ItemCancelledEvent.cs
│   └── Validation/
│       └── SaleValidator.cs
│
├── Ambev.DeveloperEvaluation.Application/
│   └── Sales/
│       ├── CreateSale/
│       │   ├── CreateSaleCommand.cs
│       │   ├── CreateSaleCommandValidator.cs
│       │   ├── CreateSaleHandler.cs
│       │   ├── CreateSaleProfile.cs
│       │   └── CreateSaleResult.cs
│       ├── GetSale/
│       │   ├── GetSaleCommand.cs
│       │   ├── GetSaleHandler.cs
│       │   ├── GetSaleProfile.cs
│       │   └── GetSaleResult.cs
│       ├── GetSales/
│       │   ├── GetSalesCommand.cs
│       │   ├── GetSalesHandler.cs
│       │   ├── GetSalesProfile.cs
│       │   └── GetSalesResult.cs
│       ├── UpdateSale/
│       │   ├── UpdateSaleCommand.cs
│       │   ├── UpdateSaleCommandValidator.cs
│       │   ├── UpdateSaleHandler.cs
│       │   ├── UpdateSaleProfile.cs
│       │   └── UpdateSaleResult.cs
│       ├── DeleteSale/
│       │   ├── DeleteSaleCommand.cs
│       │   ├── DeleteSaleHandler.cs
│       │   └── DeleteSaleResult.cs
│       ├── CancelSale/
│       │   ├── CancelSaleCommand.cs
│       │   ├── CancelSaleHandler.cs
│       │   └── CancelSaleResult.cs
│       └── CancelSaleItem/
│           ├── CancelSaleItemCommand.cs
│           ├── CancelSaleItemHandler.cs
│           └── CancelSaleItemResult.cs
│
├── Ambev.DeveloperEvaluation.ORM/
│   ├── Mapping/
│   │   └── SaleConfiguration.cs
│   └── Repositories/
│       └── SaleRepository.cs
│
└── Ambev.DeveloperEvaluation.WebApi/
    └── Features/
        └── Sales/
            ├── SalesController.cs
            ├── CreateSale/
            │   ├── CreateSaleRequest.cs
            │   ├── CreateSaleRequestValidator.cs
            │   ├── CreateSaleItemRequest.cs
            │   ├── CreateSaleProfile.cs
            │   └── CreateSaleResponse.cs
            ├── GetSale/
            │   ├── GetSaleRequest.cs
            │   ├── GetSaleProfile.cs
            │   └── GetSaleResponse.cs
            ├── GetSales/
            │   ├── GetSalesRequest.cs
            │   ├── GetSalesProfile.cs
            │   └── GetSalesResponse.cs
            ├── UpdateSale/
            │   ├── UpdateSaleRequest.cs
            │   ├── UpdateSaleRequestValidator.cs
            │   ├── UpdateSaleItemRequest.cs
            │   ├── UpdateSaleProfile.cs
            │   └── UpdateSaleResponse.cs
            ├── DeleteSale/
            │   └── DeleteSaleRequest.cs
            ├── CancelSale/
            │   └── CancelSaleRequest.cs
            └── CancelSaleItem/
                └── CancelSaleItemRequest.cs

src/tests/
└── Ambev.DeveloperEvaluation.Unit/
    └── Application/
        └── Sales/
            ├── CreateSaleHandlerTests.cs
            ├── UpdateSaleHandlerTests.cs
            ├── DeleteSaleHandlerTests.cs
            ├── CancelSaleHandlerTests.cs
            ├── CancelSaleItemHandlerTests.cs
            ├── GetSaleHandlerTests.cs
            └── TestData/
                ├── CreateSaleHandlerTestData.cs
                ├── UpdateSaleHandlerTestData.cs
                └── CancelSaleHandlerTestData.cs
    └── Domain/
        └── Entities/
            ├── SaleTests.cs
            ├── SaleItemTests.cs
            └── TestData/
                └── SaleTestData.cs
```
