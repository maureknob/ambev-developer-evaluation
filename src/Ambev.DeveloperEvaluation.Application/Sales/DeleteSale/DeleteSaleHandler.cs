using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMongoSaleRepository _mongoRepo;
    private readonly ISaleCacheService _cache;
    private readonly ILogger<DeleteSaleHandler> _logger;

    public DeleteSaleHandler(
        ISaleRepository saleRepository,
        IMongoSaleRepository mongoRepo,
        ISaleCacheService cache,
        ILogger<DeleteSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mongoRepo = mongoRepo;
        _cache = cache;
        _logger = logger;
    }

    public async Task<DeleteSaleResult> Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var deleted = await _saleRepository.DeleteAsync(command.Id, cancellationToken);
        if (!deleted)
            throw new KeyNotFoundException($"Sale with ID '{command.Id}' not found.");

        await _mongoRepo.DeleteAsync(command.Id, cancellationToken);

        try
        {
            await _cache.RemoveAsync(command.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis unavailable; could not invalidate cache for sale {SaleId}.", command.Id);
        }

        return new DeleteSaleResult { Success = true };
    }
}
