using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMongoSaleRepository _mongoRepo;

    public DeleteSaleHandler(ISaleRepository saleRepository, IMongoSaleRepository mongoRepo)
    {
        _saleRepository = saleRepository;
        _mongoRepo = mongoRepo;
    }

    public async Task<DeleteSaleResult> Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var deleted = await _saleRepository.DeleteAsync(command.Id, cancellationToken);
        if (!deleted)
            throw new KeyNotFoundException($"Sale with ID '{command.Id}' not found.");

        await _mongoRepo.DeleteAsync(command.Id, cancellationToken);

        return new DeleteSaleResult { Success = true };
    }
}
