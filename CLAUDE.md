# CLAUDE.md

## Project Overview

This project implements the **Sales domain** for the Ambev DeveloperStore API — a technical evaluation for a senior .NET developer role.

The API is built with **.NET 8 / C#** following **DDD + CQRS** principles, using MediatR, AutoMapper, EF Core, PostgreSQL, and xUnit. The Sales domain is implemented as an extension of an existing template that already contains a fully working Users domain as a reference.

---

## Documentation

| Document | Description | Path |
|----------|-------------|------|
| Sales Domain Spec | Domain model, API contract, business rules, validation, events, test scenarios | `.doc/sales-domain-spec.md` |
| Delivery Plan | Branching strategy, module breakdown, step-by-step execution, commit convention | `.doc/delivery-plan.md` |
| Step Briefs | Per-step focused briefs — files to create, rules, test IDs, definition of done | `.doc/steps/` |
| Tech Stack | Technologies used in the project | `.doc/tech-stack.md` |
| Frameworks | Libraries and frameworks | `.doc/frameworks.md` |

---

## Codebase Structure

The reference implementation (Users domain) lives at:

```
src/
├── Ambev.DeveloperEvaluation.Domain/
│   ├── Common/
│   ├── Entities/
│   ├── Enums/
│   ├── Events/
│   ├── Exceptions/
│   ├── Repositories/
│   ├── Services/
│   ├── Specifications/
│   └── Validation/
├── Ambev.DeveloperEvaluation.Application/
│   ├── Auth/
│   └── Users/
├── Ambev.DeveloperEvaluation.ORM/
│   ├── Mapping/
│   ├── Migrations/
│   └── Repositories/
├── Ambev.DeveloperEvaluation.Common/
│   ├── HealthChecks/
│   ├── Logging/
│   ├── Security/
│   └── Validation/
├── Ambev.DeveloperEvaluation.IoC/
│   └── ModuleInitializers/
└── Ambev.DeveloperEvaluation.WebApi/
    ├── Common/
    ├── Features/
    │   ├── Auth/
    │   └── Users/
    ├── Mappings/
    └── Middleware/
tests/
├── Ambev.DeveloperEvaluation.Unit/
│   ├── Application/
│   └── Domain/
├── Ambev.DeveloperEvaluation.Integration/
└── Ambev.DeveloperEvaluation.Functional/
```

All Sales domain code must follow the same structure, naming conventions, and coding patterns as the Users domain.

---

## Development Process

This project follows **spec-driven development** with a human-in-the-loop review cycle:

```
Spec → Tests (Red) → Implementation (Green) → Review → Commit → PR → Merge
```

The full workflow, branching strategy, and step-by-step execution order are defined in `.doc/delivery-plan.md`.

The spec at `.doc/sales-domain-spec.md` is the single source of truth — all code must trace back to it.

---

## Current Progress

> Update this section after every commit.

| Field | Value |
|-------|-------|
| **Current Module** | SALE-3 — Application Layer |
| **Last Completed Step** | **[SALE-3.3](.doc/steps/SALE-3.3.md)** — `UpdateSale` command and handler |
| **Next Step** | **[SALE-3.4](.doc/steps/SALE-3.4.md)** — `DeleteSale` command and handler |
| **Tests Status** | Red — A01–A09, A20, A21 target implemented; remaining handler tests still pending Application layer |

### Completed Steps
- [x] SALE-1.1 — `Sale.cs` and `SaleItem.cs` (`feat(domain): add Sale and SaleItem entities`)
- [x] SALE-1.2 — `ISaleRepository` (`feat(domain): add ISaleRepository interface`)
- [x] SALE-1.3 — `SaleValidator` (`feat(domain): add SaleValidator`)
- [x] SALE-1.4 — Domain Events (`feat(domain): add Sale domain events`)
- [x] SALE-2.1 — Sale entity unit tests (`test(domain): add Sale entity unit tests`)
- [x] SALE-2.2 — CreateSaleHandler tests (`test(application): add CreateSaleHandler unit tests`)
- [x] SALE-2.3 — UpdateSaleHandler tests (`test(application): add UpdateSaleHandler unit tests`)
- [x] SALE-2.4 — CancelSale/CancelSaleItem handler tests (`test(application): add CancelSale and CancelSaleItem handler unit tests`)
- [x] SALE-2.5 — DeleteSale/GetSale handler tests (`test(application): add DeleteSale and GetSale handler unit tests`)
- [x] SALE-3.1 — CreateSale command and handler (`feat(application): add CreateSale command and handler`)
- [x] SALE-3.2 — GetSale and GetSales queries (`feat(application): add GetSale and GetSales queries`)
- [x] SALE-3.3 — UpdateSale command and handler (`feat(application): add UpdateSale command and handler`)

### Completed Modules
- [x] SALE-1 — Domain Entities, Repository Interface, Validator, Events
- [x] SALE-2 — Unit Tests (Red — pending remaining Application Layer steps)
- [ ] SALE-3 — Application Layer (in progress)

