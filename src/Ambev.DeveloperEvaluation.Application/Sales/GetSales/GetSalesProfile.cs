using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.ReadModel;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesProfile : Profile
{
    public GetSalesProfile()
    {
        CreateMap<SaleDocument, GetSaleResult>();
        CreateMap<SaleItemDocument, GetSaleItemResult>();
    }
}
