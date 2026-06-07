# Delivery Plan

> **Purpose:** Defines how the Sales domain is delivered — branching strategy, module breakdown, step-by-step execution order, and commit conventions.  
> For *what* to build, see [Sales Domain Spec](./sales-domain-spec.md).

---

## Table of Contents

1. [Workflow Overview](#1-workflow-overview)
2. [Branching Strategy](#2-branching-strategy)
3. [Modules & Steps](#3-modules--steps)
4. [Commit Convention](#4-commit-convention)
5. [Definition of Done](#5-definition-of-done)

---

## 1. Workflow Overview

This project follows a **spec-driven, human-in-the-loop** development process:

```
Spec → Tests (Red) → Implementation (Green) → Review → Commit → PR → Merge
```

### Roles

| Role | Responsibility |
|------|---------------|
| **Agent** | Generates code one step at a time, strictly following the spec |
| **Developer** | Reviews each output, runs builds/tests locally, commits when satisfied |

### Rules

- The agent produces **one step at a time** — never skips ahead
- The developer **must approve** before the agent moves to the next step
- **No blind commits** — every commit is a conscious decision by the developer
- The spec is the **single source of truth** — if code contradicts the spec, the spec wins

---

## 2. Branching Strategy

```
main
└── dev
    ├── feature/SALE-1-domain-entities        → PR → dev
    ├── feature/SALE-2-unit-tests             → PR → dev
    ├── feature/SALE-3-application-layer      → PR → dev
    ├── feature/SALE-4-orm-persistence        → PR → dev
    └── feature/SALE-5-api-endpoints          → PR → dev
                                              → final PR: dev → main (v1.0.0)
```

### Branch Rules

| Branch | Purpose | Merges into |
|--------|---------|-------------|
| `main` | Production-ready code. Receives only the final release. | — |
| `dev` | Integration branch. Receives one PR per completed module. | `main` |
| `feature/SALE-*` | Active development. One branch per module. | `dev` |

### Setup Commands

```bash
git checkout -b dev
git checkout -b feature/SALE-1-domain-entities
```

---

## 3. Modules & Steps

### Module 1 — Domain Entities
**Branch:** `feature/SALE-1-domain-entities`

| Step | Files |
|------|-------|
| [1.1](.doc/steps/SALE-1.1.md) | `Domain/Entities/Sale.cs`<br>`Domain/Entities/SaleItem.cs` |
| [1.2](.doc/steps/SALE-1.2.md) | `Domain/Repositories/ISaleRepository.cs` |
| [1.3](.doc/steps/SALE-1.3.md) | `Domain/Validation/SaleValidator.cs` |
| [1.4](.doc/steps/SALE-1.4.md) | `Domain/Events/SaleCreatedEvent.cs`<br>`Domain/Events/SaleModifiedEvent.cs`<br>`Domain/Events/SaleCancelledEvent.cs`<br>`Domain/Events/ItemCancelledEvent.cs` |

**→ PR: `[SALE-1] Domain Entities — Sale and SaleItem` — `feature/SALE-1-domain-entities` → `dev`**

---

### Module 2 — Unit Tests
**Branch:** `feature/SALE-2-unit-tests`

> ⚠️ Tests will be **Red** (failing) until Module 3 is complete. This is intentional — TDD Red phase.

| Step | Files |
|------|-------|
| [2.1](.doc/steps/SALE-2.1.md) | `Unit/Domain/Entities/SaleTests.cs`<br>`Unit/Domain/Entities/TestData/SaleTestData.cs` |
| [2.2](.doc/steps/SALE-2.2.md) | `Unit/Application/Sales/CreateSaleHandlerTests.cs`<br>`Unit/Application/Sales/TestData/CreateSaleHandlerTestData.cs` |
| [2.3](.doc/steps/SALE-2.3.md) | `Unit/Application/Sales/UpdateSaleHandlerTests.cs`<br>`Unit/Application/Sales/TestData/UpdateSaleHandlerTestData.cs` |
| [2.4](.doc/steps/SALE-2.4.md) | `Unit/Application/Sales/CancelSaleHandlerTests.cs`<br>`Unit/Application/Sales/CancelSaleItemHandlerTests.cs`<br>`Unit/Application/Sales/TestData/CancelSaleHandlerTestData.cs` |
| [2.5](.doc/steps/SALE-2.5.md) | `Unit/Application/Sales/DeleteSaleHandlerTests.cs`<br>`Unit/Application/Sales/GetSaleHandlerTests.cs` |

**→ PR: `[SALE-2] Unit Tests — Sales domain` — `feature/SALE-2-unit-tests` → `dev`**

---

### Module 3 — Application Layer
**Branch:** `feature/SALE-3-application-layer`

> ✅ After this module, all tests from Module 2 should turn **Green**.

| Step | Files |
|------|-------|
| [3.1](.doc/steps/SALE-3.1.md) | `Application/Sales/CreateSale/CreateSaleCommand.cs`<br>`Application/Sales/CreateSale/CreateSaleCommandValidator.cs`<br>`Application/Sales/CreateSale/CreateSaleHandler.cs`<br>`Application/Sales/CreateSale/CreateSaleProfile.cs`<br>`Application/Sales/CreateSale/CreateSaleResult.cs` |
| [3.2](.doc/steps/SALE-3.2.md) | `Application/Sales/GetSale/...` (4 files)<br>`Application/Sales/GetSales/...` (4 files) |
| [3.3](.doc/steps/SALE-3.3.md) | `Application/Sales/UpdateSale/...` (5 files) |
| [3.4](.doc/steps/SALE-3.4.md) | `Application/Sales/DeleteSale/...` (3 files) |
| [3.5](.doc/steps/SALE-3.5.md) | `Application/Sales/CancelSale/...` (3 files)<br>`Application/Sales/CancelSaleItem/...` (3 files) |

**→ PR: `[SALE-3] Application Layer — Commands and Handlers` — `feature/SALE-3-application-layer` → `dev`**

---

### Module 4 — ORM & Persistence
**Branch:** `feature/SALE-4-orm-persistence`

| Step | Files |
|------|-------|
| [4.1](.doc/steps/SALE-4.1.md) | `ORM/Mapping/SaleConfiguration.cs` |
| [4.2](.doc/steps/SALE-4.2.md) | `ORM/Repositories/SaleRepository.cs` |
| [4.3](.doc/steps/SALE-4.3.md) | `ORM/DefaultContext.cs` (add `DbSet<Sale>`) |
| [4.4](.doc/steps/SALE-4.4.md) | EF Core migration files |

**→ PR: `[SALE-4] ORM & Persistence — Sale Repository and Migrations` — `feature/SALE-4-orm-persistence` → `dev`**

---

### Module 5 — API Endpoints
**Branch:** `feature/SALE-5-api-endpoints`

| Step | Files |
|------|-------|
| [5.1](.doc/steps/SALE-5.1.md) | `WebApi/Features/Sales/CreateSale/...` (5 files)<br>`WebApi/Features/Sales/GetSale/...` (3 files)<br>`WebApi/Features/Sales/GetSales/...` (3 files) |
| [5.2](.doc/steps/SALE-5.2.md) | `WebApi/Features/Sales/UpdateSale/...` (5 files)<br>`WebApi/Features/Sales/DeleteSale/...` (1 file)<br>`WebApi/Features/Sales/CancelSale/...` (1 file)<br>`WebApi/Features/Sales/CancelSaleItem/...` (1 file) |
| [5.3](.doc/steps/SALE-5.3.md) | `WebApi/Features/Sales/SalesController.cs` |
| [5.4](.doc/steps/SALE-5.4.md) | `IoC/ModuleInitializers/...` (DI registration) |

**→ PR: `[SALE-5] API Endpoints — SalesController and DI Registration` — `feature/SALE-5-api-endpoints` → `dev`**

---

### Module 6 — MongoDB Read Model
**Branch:** `feature/SALE-6-mongo-readmodel`

| Step | Files |
|------|-------|
| [6.1](.doc/steps/SALE-6.1.md) | `Domain/Repositories/IMongoSaleRepository.cs`<br>`NoSQL/Documents/SaleDocument.cs`<br>`NoSQL/Documents/SaleItemDocument.cs` |
| [6.2](.doc/steps/SALE-6.2.md) | `NoSQL/Repositories/MongoSaleRepository.cs` |
| [6.3](.doc/steps/SALE-6.3.md) | Update `GetSaleHandler`, `GetSalesHandler`, and all write handlers |
| [6.4](.doc/steps/SALE-6.4.md) | DI registration + `appsettings.json` MongoDB config |

**→ PR: `[SALE-6] MongoDB Read Model` — `feature/SALE-6-mongo-readmodel` → `dev`**

---

### Module 7 — Redis Cache
**Branch:** `feature/SALE-7-redis-cache`

| Step | Files |
|------|-------|
| [7.1](.doc/steps/SALE-7.1.md) | `Common/Cache/ISaleCacheService.cs`<br>`Common/Cache/RedisSaleCacheService.cs` |
| [7.2](.doc/steps/SALE-7.2.md) | Wire cache-aside into `GetSaleHandler`; invalidate on writes |
| [7.3](.doc/steps/SALE-7.3.md) | DI registration + `appsettings.json` Redis config |

**→ PR: `[SALE-7] Redis Cache` — `feature/SALE-7-redis-cache` → `dev`**

---

### Module 8 — Rebus Domain Event Publishing
**Branch:** `feature/SALE-8-rebus-events`

| Step | Files |
|------|-------|
| [8.1](.doc/steps/SALE-8.1.md) | Update all four write handlers to publish via `IBus` |
| [8.2](.doc/steps/SALE-8.2.md) | DI registration (in-memory transport) |

**→ PR: `[SALE-8] Rebus Event Publishing` — `feature/SALE-8-rebus-events` → `dev`**

---

### Module 9 — Startup Migrations & Infrastructure Seeding
**Branch:** `feature/SALE-9-startup-migrations`

| Step | Files |
|------|-------|
| [9.1](.doc/steps/SALE-9.1.md) | `WebApi/Infrastructure/DatabaseMigrationService.cs` — auto-applies EF Core migrations, creates MongoDB indexes, verifies Redis on startup |

**→ PR: `[SALE-9] Startup Migrations` — `feature/SALE-9-startup-migrations` → `dev`**

---

### Final Release

| Step | Action |
|------|--------|
| 10.1 | **PR: `dev` → `main`** — full review of all changes |
| 10.2 | Merge and tag `v1.0.0` |

---

## 4. Commit Convention

This project uses **Semantic Commits**:

```
<type>(<scope>): <short description>
```

### Types

| Type | Usage |
|------|-------|
| `feat` | New feature or file |
| `test` | Adding or updating tests |
| `fix` | Bug fix |
| `refactor` | Code restructure without behavior change |
| `docs` | Documentation only |
| `chore` | Build config, migrations, DI wiring |

### Scopes

| Scope | Layer |
|-------|-------|
| `domain` | Domain entities, events, validators, interfaces |
| `application` | MediatR commands, handlers, profiles |
| `orm` | EF Core mappings, repositories, migrations |
| `nosql` | MongoDB documents and repositories |
| `cache` | Redis cache service |
| `api` | Controllers, request/response models |
| `ioc` | Dependency injection |
| `startup` | Hosted services, migration runners |
| `test` | Unit test projects |

### Examples

```
feat(domain): add Sale and SaleItem entities
feat(domain): add ISaleRepository interface
test(domain): add Sale entity unit tests
feat(application): add CreateSale command and handler
feat(orm): add SaleRepository implementing ISaleRepository
feat(api): add SalesController with all 7 endpoints
feat(ioc): register Sales services in DI container
```

---

## 5. Definition of Done

A step is **done** when:
- [ ] Code compiles with no errors
- [ ] Code follows the patterns established in the template (naming, structure, style)
- [ ] All spec rules from `sales-domain-spec.md` are respected
- [ ] Developer has reviewed the output
- [ ] Commit has been made with a semantic commit message

A module is **done** when:
- [ ] All steps in the module are committed
- [ ] PR has been created from `feature/*` into `dev`
- [ ] PR has been reviewed and merged
- [ ] `dev` branch compiles cleanly after merge

The project is **done** when:
- [ ] All 9 modules are merged into `dev`
- [ ] All 35 test scenarios from the spec pass
- [ ] MongoDB read-side is wired and serving `GetSale` / `GetSales`
- [ ] Redis cache-aside is active for `GetSale`
- [ ] Rebus publishes all four domain events (in-memory transport)
- [ ] `docker compose up` brings the API to a working state with no manual steps
- [ ] Final PR `dev` → `main` is merged and tagged `v1.0.0`
