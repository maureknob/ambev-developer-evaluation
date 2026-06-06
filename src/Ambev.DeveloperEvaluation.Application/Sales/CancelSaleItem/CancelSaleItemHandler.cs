using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleItemHandler> _logger;

    public CancelSaleItemHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<CancelSaleItemHandler> logger)
    {
        _saleRepository = saleRepository;
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

        _logger.LogInformation("ItemCancelledEvent: {@Event}", new ItemCancelledEvent
        {
            SaleId = updated.Id,
            ItemId = command.ItemId,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            OccurredAt = DateTime.UtcNow
        });

        return _mapper.Map<CancelSaleItemResult>(updated);
    }
}
