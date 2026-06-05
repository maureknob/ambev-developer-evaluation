using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public static class UpdateSaleHandlerTestData
{
    private static readonly Faker Faker = new();

    public static UpdateSaleCommand GenerateValidCommand(Guid? saleId = null)
    {
        return new UpdateSaleCommand
        {
            Id = saleId ?? Guid.NewGuid(),
            SaleNumber = Faker.Random.AlphaNumeric(10),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = Faker.Name.FullName(),
            BranchId = Guid.NewGuid(),
            BranchName = Faker.Company.CompanyName(),
            Items = new List<UpdateSaleItemCommand>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = Faker.Commerce.ProductName(),
                    Quantity = Faker.Random.Int(1, 10),
                    UnitPrice = Faker.Random.Decimal(1, 100)
                }
            }
        };
    }

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
        sale.AddItem(SaleTestData.GenerateValidItem());
        return sale;
    }

    public static Sale GenerateCancelledSale()
    {
        var sale = GenerateActiveSale();
        sale.Cancel();
        return sale;
    }
}
