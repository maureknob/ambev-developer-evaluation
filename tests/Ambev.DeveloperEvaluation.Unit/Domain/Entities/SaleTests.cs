using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    // ── Discount Tiers (B01–B07) ────────────────────────────────────────────

    [Theory(DisplayName = "Discount should be 0% for quantities 1–3")]
    [InlineData(1)]
    [InlineData(3)]
    public void Given_QuantityBelow4_When_ItemCreated_Then_DiscountIsZero(int qty)
    {
        var item = new SaleItem(Guid.NewGuid(), "Product", qty, 10m);
        Assert.Equal(0m, item.Discount);
    }

    [Theory(DisplayName = "Discount should be 10% for quantities 4–9")]
    [InlineData(4)]
    [InlineData(9)]
    public void Given_QuantityBetween4And9_When_ItemCreated_Then_DiscountIsTenPercent(int qty)
    {
        var item = new SaleItem(Guid.NewGuid(), "Product", qty, 10m);
        Assert.Equal(0.10m, item.Discount);
    }

    [Theory(DisplayName = "Discount should be 20% for quantities 10–20")]
    [InlineData(10)]
    [InlineData(20)]
    public void Given_QuantityBetween10And20_When_ItemCreated_Then_DiscountIsTwentyPercent(int qty)
    {
        var item = new SaleItem(Guid.NewGuid(), "Product", qty, 10m);
        Assert.Equal(0.20m, item.Discount);
    }

    [Fact(DisplayName = "Quantity above 20 should throw InvalidOperationException")]
    public void Given_QuantityAbove20_When_ItemCreated_Then_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new SaleItem(Guid.NewGuid(), "Product", 21, 10m));
    }

    // ── Sale TotalAmount Calculations (D01–D04) ─────────────────────────────

    [Fact(DisplayName = "D01 — qty=3, price=10 → TotalAmount=30, Discount=0")]
    public void Given_Qty3Price10_When_ItemCreated_Then_TotalAmount30()
    {
        var item = new SaleItem(Guid.NewGuid(), "Product", 3, 10m);
        Assert.Equal(0m, item.Discount);
        Assert.Equal(30m, item.TotalAmount);
    }

    [Fact(DisplayName = "D02 — qty=4, price=10 → TotalAmount=36, Discount=0.10")]
    public void Given_Qty4Price10_When_ItemCreated_Then_TotalAmount36()
    {
        var item = new SaleItem(Guid.NewGuid(), "Product", 4, 10m);
        Assert.Equal(0.10m, item.Discount);
        Assert.Equal(36m, item.TotalAmount);
    }

    [Fact(DisplayName = "D03 — qty=10, price=10 → TotalAmount=80, Discount=0.20")]
    public void Given_Qty10Price10_When_ItemCreated_Then_TotalAmount80()
    {
        var item = new SaleItem(Guid.NewGuid(), "Product", 10, 10m);
        Assert.Equal(0.20m, item.Discount);
        Assert.Equal(80m, item.TotalAmount);
    }

    [Fact(DisplayName = "D04 — qty=20, price=10 → TotalAmount=160, Discount=0.20")]
    public void Given_Qty20Price10_When_ItemCreated_Then_TotalAmount160()
    {
        var item = new SaleItem(Guid.NewGuid(), "Product", 20, 10m);
        Assert.Equal(0.20m, item.Discount);
        Assert.Equal(160m, item.TotalAmount);
    }

    [Fact(DisplayName = "D05 — qty=21 should throw InvalidOperationException")]
    public void Given_Qty21_When_ItemCreated_Then_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new SaleItem(Guid.NewGuid(), "Product", 21, 10m));
    }

    // ── Sale Cancellation (D08–D09) ──────────────────────────────────────────

    [Fact(DisplayName = "D08 — Cancel sale sets IsCancelled=true on sale and all items")]
    public void Given_ActiveSale_When_Cancelled_Then_SaleAndAllItemsAreCancelled()
    {
        var sale = SaleTestData.GenerateValidSale(itemCount: 2);

        sale.Cancel();

        Assert.True(sale.IsCancelled);
        Assert.All(sale.Items, i => Assert.True(i.IsCancelled));
    }

    [Fact(DisplayName = "D09 — Cancel already-cancelled sale throws InvalidOperationException")]
    public void Given_CancelledSale_When_CancelledAgain_Then_Throws()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        Assert.Throws<InvalidOperationException>(() => sale.Cancel());
    }

    // ── Item Cancellation (D10–D11) ──────────────────────────────────────────

    [Fact(DisplayName = "D10 — Cancel item sets IsCancelled=true and recalculates TotalAmount")]
    public void Given_SaleWithTwoItems_When_OneItemCancelled_Then_TotalAmountRecalculated()
    {
        var sale = new Sale
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch"
        };

        var item1 = new SaleItem(Guid.NewGuid(), "Product A", 3, 10m); // 30
        var item2 = new SaleItem(Guid.NewGuid(), "Product B", 3, 10m); // 30
        sale.AddItem(item1);
        sale.AddItem(item2);

        sale.CancelItem(item1.Id);

        Assert.True(item1.IsCancelled);
        Assert.Equal(30m, sale.TotalAmount);
    }

    [Fact(DisplayName = "D11 — Cancel already-cancelled item throws InvalidOperationException")]
    public void Given_CancelledItem_When_CancelledAgain_Then_Throws()
    {
        var sale = SaleTestData.GenerateValidSale();
        var itemId = sale.Items[0].Id;
        sale.CancelItem(itemId);

        Assert.Throws<InvalidOperationException>(() => sale.CancelItem(itemId));
    }

    // ── Cancelled Sale Modifications (D12) ───────────────────────────────────

    [Fact(DisplayName = "D12 — Adding item to cancelled sale throws InvalidOperationException")]
    public void Given_CancelledSale_When_AddItem_Then_Throws()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        Assert.Throws<InvalidOperationException>(() =>
            sale.AddItem(SaleTestData.GenerateValidItem()));
    }

    // ── TotalAmount Excludes Cancelled Items (D13) ───────────────────────────

    [Fact(DisplayName = "D13 — TotalAmount only counts non-cancelled items")]
    public void Given_SaleWithCancelledItem_When_CheckTotal_Then_ExcludesCancelledItems()
    {
        var sale = new Sale
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch"
        };

        var item1 = new SaleItem(Guid.NewGuid(), "Product A", 3, 10m); // 30
        var item2 = new SaleItem(Guid.NewGuid(), "Product B", 3, 20m); // 60
        sale.AddItem(item1);
        sale.AddItem(item2);

        sale.CancelItem(item2.Id);

        Assert.Equal(30m, sale.TotalAmount);
    }

    // ── Duplicate ProductId (D14) ─────────────────────────────────────────────

    [Fact(DisplayName = "D14 — Adding item with duplicate ProductId throws InvalidOperationException")]
    public void Given_SaleWithItem_When_AddDuplicateProduct_Then_Throws()
    {
        var productId = Guid.NewGuid();
        var sale = new Sale
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch"
        };

        sale.AddItem(SaleTestData.GenerateItemWithProductId(productId));

        Assert.Throws<InvalidOperationException>(() =>
            sale.AddItem(SaleTestData.GenerateItemWithProductId(productId)));
    }
}
