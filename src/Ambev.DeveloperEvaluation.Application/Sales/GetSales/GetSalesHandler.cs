using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesHandler : IRequestHandler<GetSalesCommand, GetSalesResult>
{
    private readonly IMongoSaleRepository _mongoRepo;
    private readonly IMapper _mapper;

    public GetSalesHandler(IMongoSaleRepository mongoRepo, IMapper mapper)
    {
        _mongoRepo = mongoRepo;
        _mapper = mapper;
    }

    public async Task<GetSalesResult> Handle(GetSalesCommand command, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _mongoRepo.GetPagedAsync(
            command.Page, command.Size, command.Order, cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)command.Size);

        return new GetSalesResult
        {
            Data = _mapper.Map<IEnumerable<GetSaleResult>>(items),
            TotalItems = (int)totalCount,
            CurrentPage = command.Page,
            TotalPages = totalPages
        };
    }
}
