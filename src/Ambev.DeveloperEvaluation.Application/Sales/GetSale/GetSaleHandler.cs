using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleHandler : IRequestHandler<GetSaleCommand, GetSaleResult>
{
    private readonly IMongoSaleRepository _mongoRepo;
    private readonly IMapper _mapper;

    public GetSaleHandler(IMongoSaleRepository mongoRepo, IMapper mapper)
    {
        _mongoRepo = mongoRepo;
        _mapper = mapper;
    }

    public async Task<GetSaleResult> Handle(GetSaleCommand command, CancellationToken cancellationToken)
    {
        var document = await _mongoRepo.GetByIdAsync(command.Id, cancellationToken);
        if (document == null)
            throw new KeyNotFoundException($"Sale with ID '{command.Id}' not found.");

        return _mapper.Map<GetSaleResult>(document);
    }
}
