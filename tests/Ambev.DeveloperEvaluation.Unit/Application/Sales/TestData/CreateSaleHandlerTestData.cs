using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public static class CreateSaleHandlerTestData
{
    private static readonly Faker Faker = new();

    public static CreateSaleCommand GenerateValidCommand(int itemCount = 1)
    {
        var items = Enumerable.Range(0, itemCount).Select(_ => new CreateSaleItemCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = Faker.Commerce.ProductName(),
            Quantity = Faker.Random.Int(1, 10),
            UnitPrice = Faker.Random.Decimal(1, 100)
        }).ToList();

        return new CreateSaleCommand
        {
            SaleNumber = Faker.Random.AlphaNumeric(10),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = Faker.Name.FullName(),
            BranchId = Guid.NewGuid(),
            BranchName = Faker.Company.CompanyName(),
            Items = items
        };
    }

    public static CreateSaleCommand GenerateInvalidCommand() => new();
}
