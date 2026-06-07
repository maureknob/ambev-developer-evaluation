# Developer Evaluation Project

`READ CAREFULLY`

## Instructions
**The test below will have up to 7 calendar days to be delivered from the date of receipt of this manual.**

- The code must be versioned in a public Github repository and a link must be sent for evaluation once completed
- Upload this template to your repository and start working from it
- Read the instructions carefully and make sure all requirements are being addressed
- The repository must provide instructions on how to configure, execute and test the project
- Documentation and overall organization will also be taken into consideration

## Use Case
**You are a developer on the DeveloperStore team. Now we need to implement the API prototypes.**

As we work with `DDD`, to reference entities from other domains, we use the `External Identities` pattern with denormalization of entity descriptions.

Therefore, you will write an API (complete CRUD) that handles sales records. The API needs to be able to inform:

* Sale number
* Date when the sale was made
* Customer
* Total sale amount
* Branch where the sale was made
* Products
* Quantities
* Unit prices
* Discounts
* Total amount for each item
* Cancelled/Not Cancelled

It's not mandatory, but it would be a differential to build code for publishing events of:
* SaleCreated
* SaleModified
* SaleCancelled
* ItemCancelled

If you write the code, **it's not required** to actually publish to any Message Broker. You can log a message in the application log or however you find most convenient.

### Business Rules

* Purchases above 4 identical items have a 10% discount
* Purchases between 10 and 20 identical items have a 20% discount
* It's not possible to sell above 20 identical items
* Purchases below 4 items cannot have a discount

These business rules define quantity-based discounting tiers and limitations:

1. Discount Tiers:
   - 4+ items: 10% discount
   - 10-20 items: 20% discount

2. Restrictions:
   - Maximum limit: 20 items per product
   - No discounts allowed for quantities below 4 items

## Overview
This section provides a high-level overview of the project and the various skills and competencies it aims to assess for developer candidates. 

See [Overview](/.doc/overview.md)

## Tech Stack
This section lists the key technologies used in the project, including the backend, testing, frontend, and database components. 

See [Tech Stack](/.doc/tech-stack.md)

## Frameworks
This section outlines the frameworks and libraries that are leveraged in the project to enhance development productivity and maintainability. 

See [Frameworks](/.doc/frameworks.md)

<!-- 
## API Structure
This section includes links to the detailed documentation for the different API resources:
- [API General](./docs/general-api.md)
- [Products API](/.doc/products-api.md)
- [Carts API](/.doc/carts-api.md)
- [Users API](/.doc/users-api.md)
- [Auth API](/.doc/auth-api.md)
-->

## Project Structure
This section describes the overall structure and organization of the project files and directories. 

See [Project Structure](/.doc/project-structure.md)

---

## Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8) — only needed to run locally outside Docker

### Clone and Run

```bash
git clone https://github.com/maureknob/ambev-developer-evaluation.git
cd ambev-developer-evaluation

docker compose up -d --build
```

This starts four containers:

| Container | Role | Port |
|-----------|------|------|
| `ambev_developer_evaluation_webapi` | ASP.NET 8 API | `http://localhost:8080` |
| `ambev_developer_evaluation_database` | PostgreSQL 13 | `5432` |
| `ambev_developer_evaluation_nosql` | MongoDB 8 | `27017` |
| `ambev_developer_evaluation_cache` | Redis 7 | `6379` |

On first startup the API automatically applies EF Core migrations, creates MongoDB indexes, and seeds a test user. The API is ready when you see `Now listening on: http://[::]:8080` in the logs:

```bash
docker compose logs -f ambev.developerevaluation.webapi
```

### Explore the API

Open Swagger UI at **http://localhost:8080/swagger**.

All Sales endpoints require a JWT token. A test user is seeded automatically on first startup — **these credentials exist only for evaluation purposes and would never appear in a real project**:

**`POST /api/auth`**
```json
{
  "email": "test@ambev.com",
  "password": "Test@1234"
}
```

Copy the `token` from the response, click **Authorize** in Swagger, and paste it as `Bearer <token>`.

#### Example: Create a Sale

**`POST /api/sales`**
```json
{
  "saleNumber": "SALE-001",
  "saleDate": "2026-06-07T00:00:00Z",
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "customerName": "Acme Corp",
  "branchId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "branchName": "Downtown Branch",
  "items": [
    {
      "productId": "1a2b3c4d-0000-0000-0000-000000000001",
      "productName": "Brahma 600ml",
      "quantity": 10,
      "unitPrice": 8.50
    },
    {
      "productId": "1a2b3c4d-0000-0000-0000-000000000002",
      "productName": "Skol 350ml",
      "quantity": 4,
      "unitPrice": 5.00
    },
    {
      "productId": "1a2b3c4d-0000-0000-0000-000000000003",
      "productName": "Antarctica 473ml",
      "quantity": 2,
      "unitPrice": 6.00
    }
  ]
}
```

Discounts are applied automatically by the domain:
- 10 units of Brahma → **20% discount** (10–20 tier)
- 4 units of Skol → **10% discount** (4–9 tier)
- 2 units of Antarctica → **no discount** (below 4)

---

## Running Tests

```bash
dotnet test tests/Ambev.DeveloperEvaluation.Unit
```

The unit test suite covers 89 test cases across all Sales handlers and domain entities (commands, queries, business rule validation, cancellation flows, and error paths). All tests run in-memory with NSubstitute mocks — no Docker required.

---

## Infrastructure

### PostgreSQL — Write Side (Source of Truth)

All write operations (`CreateSale`, `UpdateSale`, `CancelSale`, `DeleteSale`) persist to PostgreSQL via EF Core. This is the authoritative store — if the read model ever drifts, Postgres is what you restore from. The schema is managed by EF Core migrations applied automatically on startup.

### MongoDB — Read Model (CQRS Read Side)

Read queries (`GetSale`, `GetSales`) are served from a MongoDB `sales` collection rather than querying Postgres directly. The motivation is separation of concerns: reads and writes have different scaling and shape requirements. Every write handler upserts the denormalized sale document into MongoDB immediately after the Postgres write, keeping the read model in sync without a separate sync job or event consumer.

### Redis — Cache Aside

`GetSaleHandler` checks Redis before hitting MongoDB. On a cache miss it fetches from MongoDB and populates Redis with a 5-minute TTL (`ambev_sale:{id}`). All write handlers invalidate or repopulate the cache entry after each mutation, so repeated reads are served in-memory and stay consistent with the latest write.

### Domain Events — Rebus (In-Memory)

All four domain events required by the spec are implemented and published via [Rebus](https://github.com/rebus-org/Rebus) using an in-memory transport (no external broker required):

| Event | Triggered by |
|-------|-------------|
| `SaleCreated` | `CreateSaleHandler` |
| `SaleModified` | `UpdateSaleHandler` |
| `SaleCancelled` | `CancelSaleHandler` |
| `ItemCancelled` | `CancelSaleItemHandler` |

Events are visible in the application logs. In a production system the transport would be swapped to RabbitMQ or Azure Service Bus with a single configuration change — the handlers and event contracts remain unchanged.

---

## Architecture

This project follows **Domain-Driven Design (DDD)** with **CQRS** implemented via MediatR.

```
Ambev.DeveloperEvaluation.Domain       → Entities, business rules, domain events, read model contracts
Ambev.DeveloperEvaluation.Application  → MediatR commands, queries, handlers, cache service
Ambev.DeveloperEvaluation.ORM          → EF Core mappings, Postgres repository, migrations
Ambev.DeveloperEvaluation.Common       → JWT, security, validation pipeline
Ambev.DeveloperEvaluation.IoC          → Dependency injection wiring (Postgres, MongoDB, Redis, Rebus)
Ambev.DeveloperEvaluation.WebApi       → Controllers, request/response models, startup migration service
```

The write path (commands) and read path (queries) are intentionally separated: commands write to Postgres and project to MongoDB; queries read from MongoDB via Redis. This means the read side can evolve independently — different shape, different indexes, different scaling — without touching the write model.

For a detailed breakdown see [Project Structure](/.doc/project-structure.md) and [Sales Domain Spec](/.doc/sales-domain-spec.md).

---

## API Reference

Full API contract with request/response shapes: [Sales Domain Spec](/.doc/sales-domain-spec.md)

### Sales Endpoints

| Method   | Endpoint                                | Description                 |
|----------|-----------------------------------------|-----------------------------|
| `POST`   | `/api/sales`                            | Create a new sale           |
| `GET`    | `/api/sales/{id}`                       | Get a sale by ID            |
| `GET`    | `/api/sales?page=1&size=10`             | List sales (paginated)      |
| `PUT`    | `/api/sales/{id}`                       | Update a sale               |
| `DELETE` | `/api/sales/{id}`                       | Delete a sale               |
| `PATCH`  | `/api/sales/{id}/cancel`                | Cancel an entire sale       |
| `PATCH`  | `/api/sales/{id}/items/{itemId}/cancel` | Cancel a single sale item   |

### Business Rules

| Quantity      | Discount    |
|---------------|-------------|
| 1 – 3 items   | 0%          |
| 4 – 9 items   | 10%         |
| 10 – 20 items | 20%         |
| > 20 items    | Not allowed |

