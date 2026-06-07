using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleHandler : IRequestHandler<GetSaleCommand, GetSaleResult>
{
    private readonly IMongoSaleRepository _mongoRepo;
    private readonly ISaleCacheService _cache;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSaleHandler> _logger;

    public GetSaleHandler(
        IMongoSaleRepository mongoRepo,
        ISaleCacheService cache,
        IMapper mapper,
        ILogger<GetSaleHandler> logger)
    {
        _mongoRepo = mongoRepo;
        _cache = cache;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetSaleResult> Handle(GetSaleCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var cached = await _cache.GetAsync(command.Id, cancellationToken);
            if (cached is not null)
                return _mapper.Map<GetSaleResult>(cached);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis unavailable on GetSale {SaleId}; falling back to MongoDB.", command.Id);
        }

        var document = await _mongoRepo.GetByIdAsync(command.Id, cancellationToken);
        if (document is null)
            throw new KeyNotFoundException($"Sale with ID '{command.Id}' not found.");

        try
        {
            await _cache.SetAsync(document, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis unavailable; could not populate cache for sale {SaleId}.", command.Id);
        }

        return _mapper.Map<GetSaleResult>(document);
    }
}
