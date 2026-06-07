using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    private static readonly Faker Faker = new();

    public static Sale GenerateValidSale(int itemCount = 1)
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

        for (var i = 0; i < itemCount; i++)
            sale.AddItem(GenerateValidItem());

        return sale;
    }

    public static SaleItem GenerateValidItem(int quantity = 1, decimal unitPrice = 10m)
    {
        return new SaleItem(Guid.NewGuid(), Faker.Commerce.ProductName(), quantity, unitPrice);
    }

    public static SaleItem GenerateItemWithProductId(Guid productId, int quantity = 1, decimal unitPrice = 10m)
    {
        return new SaleItem(productId, Faker.Commerce.ProductName(), quantity, unitPrice);
    }
}
