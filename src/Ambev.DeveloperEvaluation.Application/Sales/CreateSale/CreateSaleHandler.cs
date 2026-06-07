using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.ReadModel;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMongoSaleRepository _mongoRepo;
    private readonly ISaleCacheService _cache;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleHandler(
        ISaleRepository saleRepository,
        IMongoSaleRepository mongoRepo,
        ISaleCacheService cache,
        IMapper mapper,
        ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mongoRepo = mongoRepo;
        _cache = cache;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existing = await _saleRepository.GetByNumberAsync(command.SaleNumber, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException($"Sale with number '{command.SaleNumber}' already exists.");

        var sale = new Sale
        {
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName
        };

        foreach (var item in command.Items)
            sale.AddItem(new SaleItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice));

        var created = await _saleRepository.CreateAsync(sale, cancellationToken);

        var saleDocument = _mapper.Map<SaleDocument>(created);
        await _mongoRepo.UpsertAsync(saleDocument, cancellationToken);

        try
        {
            await _cache.SetAsync(saleDocument, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis unavailable; could not populate cache for sale {SaleId}.", created.Id);
        }

        _logger.LogInformation("SaleCreatedEvent: {@Event}", new SaleCreatedEvent
        {
            SaleId = created.Id,
            SaleNumber = created.SaleNumber,
            CustomerId = created.CustomerId,
            TotalAmount = created.TotalAmount,
            OccurredAt = DateTime.UtcNow
        });

        return _mapper.Map<CreateSaleResult>(created);
    }
}
