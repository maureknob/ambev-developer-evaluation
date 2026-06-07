using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.ReadModel;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleProfile : Profile
{
    public GetSaleProfile()
    {
        CreateMap<SaleDocument, GetSaleResult>();
        CreateMap<SaleItemDocument, GetSaleItemResult>();
    }
}
