using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMongoSaleRepository _mongoRepo;
    private readonly ISaleCacheService _cache;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleHandler> _logger;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mongoRepo = Substitute.For<IMongoSaleRepository>();
        _cache = Substitute.For<ISaleCacheService>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CancelSaleHandler>>();
        _handler = new CancelSaleHandler(_saleRepository, _mongoRepo, _cache, _mapper, _logger);
    }

    [Fact(DisplayName = "A12 — Valid cancellation sets IsCancelled and logs SaleCancelledEvent")]
    public async Task Handle_ValidCommand_CancelsSaleAndLogsEvent()
    {
        // Given
        var sale = CancelSaleHandlerTestData.GenerateActiveSale();
        var command = new CancelSaleCommand { Id = sale.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CancelSaleResult>(sale).Returns(new CancelSaleResult { IsCancelled = true });

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.IsCancelled.Should().BeTrue();
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        _logger.Received(1).Log(LogLevel.Information, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<Exception?>(), Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact(DisplayName = "A13 — Sale not found throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = new CancelSaleCommand { Id = Guid.NewGuid() };
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "A14 — Already cancelled sale throws InvalidOperationException")]
    public async Task Handle_AlreadyCancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var sale = CancelSaleHandlerTestData.GenerateCancelledSale();
        var command = new CancelSaleCommand { Id = sale.Id };
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
