using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
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
    private readonly IBus _bus;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleHandler(
        ISaleRepository saleRepository,
        IMongoSaleRepository mongoRepo,
        ISaleCacheService cache,
        IBus bus,
        IMapper mapper,
        ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mongoRepo = mongoRepo;
        _cache = cache;
        _bus = bus;
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

        var domainEvent = new SaleCreatedEvent
        {
            SaleId = created.Id,
            SaleNumber = created.SaleNumber,
            CustomerId = created.CustomerId,
            TotalAmount = created.TotalAmount,
            OccurredAt = DateTime.UtcNow
        };

        _logger.LogInformation("SaleCreatedEvent: {@Event}", domainEvent);

        try
        {
            await _bus.Publish(domainEvent);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish SaleCreatedEvent for sale {SaleId}.", created.Id);
        }

        return _mapper.Map<CreateSaleResult>(created);
    }
}
