using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.ReadModel;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMongoSaleRepository _mongoRepo;
    private readonly ISaleCacheService _cache;
    private readonly IBus _bus;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleItemHandler> _logger;

    public CancelSaleItemHandler(
        ISaleRepository saleRepository,
        IMongoSaleRepository mongoRepo,
        ISaleCacheService cache,
        IBus bus,
        IMapper mapper,
        ILogger<CancelSaleItemHandler> logger)
    {
        _saleRepository = saleRepository;
        _mongoRepo = mongoRepo;
        _cache = cache;
        _bus = bus;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CancelSaleItemResult> Handle(CancelSaleItemCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID '{command.SaleId}' not found.");

        var item = sale.Items.FirstOrDefault(i => i.Id == command.ItemId)
            ?? throw new KeyNotFoundException($"Item '{command.ItemId}' not found in sale '{command.SaleId}'.");

        sale.CancelItem(command.ItemId);

        var updated = await _saleRepository.UpdateAsync(sale, cancellationToken);

        var saleDocument = _mapper.Map<SaleDocument>(updated);
        await _mongoRepo.UpsertAsync(saleDocument, cancellationToken);

        try
        {
            await _cache.SetAsync(saleDocument, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis unavailable; could not populate cache for sale {SaleId}.", updated.Id);
        }

        var domainEvent = new ItemCancelledEvent
        {
            SaleId = updated.Id,
            ItemId = command.ItemId,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            OccurredAt = DateTime.UtcNow
        };

        _logger.LogInformation("ItemCancelledEvent: {@Event}", domainEvent);

        try
        {
            await _bus.Publish(domainEvent);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish ItemCancelledEvent for sale {SaleId}, item {ItemId}.", updated.Id, command.ItemId);
        }

        return _mapper.Map<CancelSaleItemResult>(updated);
    }
}
