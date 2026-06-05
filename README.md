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

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)

### Clone the Repository

```bash
git clone https://github.com/maureknob/ambev-developer-evaluation.git
cd ambev-developer-evaluation
```

---

## Architecture

This project follows **Domain-Driven Design (DDD)** with **CQRS** implemented via MediatR.

```
Ambev.DeveloperEvaluation.Domain       → Entities, business rules, domain events
Ambev.DeveloperEvaluation.Application  → MediatR commands, queries, handlers
Ambev.DeveloperEvaluation.ORM          → EF Core mappings, repositories, migrations
Ambev.DeveloperEvaluation.Common       → JWT, security, validation pipeline
Ambev.DeveloperEvaluation.IoC          → Dependency injection wiring
Ambev.DeveloperEvaluation.WebApi       → Controllers, request/response models
```

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

> ⚠️ **Configuration, How to Run, and How to Test sections will be added once the implementation is complete.**