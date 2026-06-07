using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.ReadModel;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMongoSaleRepository _mongoRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;

    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        IMongoSaleRepository mongoRepo,
        IMapper mapper,
        ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mongoRepo = mongoRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID '{command.Id}' not found.");

        if (sale.IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale.");

        var previousTotal = sale.TotalAmount;

        var numberConflict = await _saleRepository.GetByNumberAsync(command.SaleNumber, cancellationToken);
        if (numberConflict != null && numberConflict.Id != sale.Id)
            throw new InvalidOperationException($"Sale number '{command.SaleNumber}' is already in use.");

        sale.SaleNumber = command.SaleNumber;
        sale.SaleDate = command.SaleDate;
        sale.CustomerId = command.CustomerId;
        sale.CustomerName = command.CustomerName;
        sale.BranchId = command.BranchId;
        sale.BranchName = command.BranchName;

        sale.ReplaceItems(command.Items.Select(i =>
            new SaleItem(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)));

        sale.UpdatedAt = DateTime.UtcNow;

        var updated = await _saleRepository.UpdateAsync(sale, cancellationToken);

        await _mongoRepo.UpsertAsync(_mapper.Map<SaleDocument>(updated), cancellationToken);

        _logger.LogInformation("SaleModifiedEvent: {@Event}", new SaleModifiedEvent
        {
            SaleId = updated.Id,
            SaleNumber = updated.SaleNumber,
            PreviousTotalAmount = previousTotal,
            NewTotalAmount = updated.TotalAmount,
            OccurredAt = DateTime.UtcNow
        });

        return _mapper.Map<UpdateSaleResult>(updated);
    }
}
