using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ReadModel;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class SaleDocumentProfile : Profile
{
    public SaleDocumentProfile()
    {
        CreateMap<Sale, SaleDocument>();
        CreateMap<SaleItem, SaleItemDocument>();
    }
}
