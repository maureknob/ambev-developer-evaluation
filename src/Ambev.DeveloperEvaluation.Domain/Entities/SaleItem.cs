using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }

    protected SaleItem() { }

    public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        Calculate(quantity, unitPrice);
    }

    internal void Cancel()
    {
        if (IsCancelled)
            throw new InvalidOperationException("Item is already cancelled.");

        IsCancelled = true;
    }

    private void Calculate(int quantity, decimal unitPrice)
    {
        Discount = DetermineDiscount(quantity);
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalAmount = quantity * unitPrice * (1 - Discount);
    }

    private static decimal DetermineDiscount(int quantity)
    {
        if (quantity > 20)
            throw new InvalidOperationException("Cannot sell more than 20 identical items.");

        if (quantity >= 10) return 0.20m;
        if (quantity >= 4) return 0.10m;
        return 0m;
    }
}
