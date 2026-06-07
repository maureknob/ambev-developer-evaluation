# Step 6.1 — MongoDB: Read Model Document and Repository Interface

**Branch:** `feature/SALE-6-mongo-readmodel`  
**Commit:** `feat(domain): add SaleDocument and IMongoSaleRepository for read-side`

---

## Context

The challenge requires demonstrating skills with **both PostgreSQL and MongoDB**. The pattern here is CQRS read-side projection: write operations go through EF Core → PostgreSQL; read operations (`GetSale`, `GetSales`) go through MongoDB using a denormalized document.

---

## Files to Create

- `src/Ambev.DeveloperEvaluation.Domain/Repositories/IMongoSaleRepository.cs`

## Files to Create (NoSQL project — new)

- `src/Ambev.DeveloperEvaluation.NoSQL/Documents/SaleDocument.cs`
- `src/Ambev.DeveloperEvaluation.NoSQL/Documents/SaleItemDocument.cs`

> If a dedicated `NoSQL` project does not exist yet, add the document classes to the `ORM` project under a `Documents/` subfolder, or create a new `Ambev.DeveloperEvaluation.NoSQL` class library. Follow the same project structure conventions as ORM.

---

## SaleDocument shape

```csharp
public class SaleDocument
{
    [BsonId]
    public Guid Id { get; set; }
    public string SaleNumber { get; set; }
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public Guid BranchId { get; set; }
    public string BranchName { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<SaleItemDocument> Items { get; set; }
}
```

## SaleItemDocument shape

```csharp
public class SaleItemDocument
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
}
```

## IMongoSaleRepository interface

```csharp
public interface IMongoSaleRepository
{
    Task UpsertAsync(SaleDocument document, CancellationToken cancellationToken = default);
    Task<SaleDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<SaleDocument> Items, long TotalCount)> GetPagedAsync(
        int page, int size, string? order, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
```

---

## Definition of Done

- [ ] `SaleDocument` and `SaleItemDocument` compile with no errors
- [ ] `IMongoSaleRepository` defined with the four methods above
- [ ] MongoDB driver NuGet package (`MongoDB.Driver`) referenced in the project
- [ ] Reviewed by developer
- [ ] Committed with `feat(domain): add SaleDocument and IMongoSaleRepository for read-side`
