using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private List<SaleItem> _items = new();
    public IReadOnlyList<SaleItem> Items => _items.AsReadOnly();

    public Sale()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(SaleItem item)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot modify a cancelled sale.");

        if (_items.Any(i => i.ProductId == item.ProductId))
            throw new InvalidOperationException($"Product '{item.ProductId}' is already in this sale.");

        item.SaleId = Id;
        _items.Add(item);
        RecalculateTotal();
    }

    public void Cancel()
    {
        if (IsCancelled)
            throw new InvalidOperationException("Sale is already cancelled.");

        IsCancelled = true;
        foreach (var item in _items.Where(i => !i.IsCancelled))
            item.Cancel();

        UpdatedAt = DateTime.UtcNow;
        TotalAmount = 0;
    }

    public void ReplaceItems(IEnumerable<SaleItem> newItems)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot modify a cancelled sale.");

        _items.Clear();
        foreach (var item in newItems)
            AddItem(item);
    }

    public void CancelItem(Guid itemId)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot modify a cancelled sale.");

        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new KeyNotFoundException($"Item '{itemId}' not found in this sale.");

        item.Cancel();
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount);
    }
}
