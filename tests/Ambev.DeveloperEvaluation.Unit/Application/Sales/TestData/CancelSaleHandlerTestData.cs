using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public static class CancelSaleHandlerTestData
{
    private static readonly Faker Faker = new();

    public static Sale GenerateActiveSale()
    {
        var sale = new Sale
        {
            SaleNumber = Faker.Random.AlphaNumeric(10),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = Faker.Name.FullName(),
            BranchId = Guid.NewGuid(),
            BranchName = Faker.Company.CompanyName()
        };
        sale.AddItem(new SaleItem(Guid.NewGuid(), Faker.Commerce.ProductName(), 3, 10m));
        return sale;
    }

    public static Sale GenerateCancelledSale()
    {
        var sale = GenerateActiveSale();
        sale.Cancel();
        return sale;
    }

    public static Sale GenerateSaleWithActiveItem(out Guid itemId)
    {
        var sale = new Sale
        {
            SaleNumber = Faker.Random.AlphaNumeric(10),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = Faker.Name.FullName(),
            BranchId = Guid.NewGuid(),
            BranchName = Faker.Company.CompanyName()
        };
        var item = new SaleItem(Guid.NewGuid(), Faker.Commerce.ProductName(), 3, 10m);
        sale.AddItem(item);
        itemId = item.Id;
        return sale;
    }
}
