using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleItemHandler> _logger;
    private readonly CancelSaleItemHandler _handler;

    public CancelSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CancelSaleItemHandler>>();
        _handler = new CancelSaleItemHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "A15 — Valid item cancellation updates total and logs ItemCancelledEvent")]
    public async Task Handle_ValidCommand_CancelsItemAndLogsEvent()
    {
        // Given
        var sale = CancelSaleHandlerTestData.GenerateSaleWithActiveItem(out var itemId);
        var command = new CancelSaleItemCommand { SaleId = sale.Id, ItemId = itemId };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CancelSaleItemResult>(sale).Returns(new CancelSaleItemResult());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        _logger.Received(1).Log(LogLevel.Information, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<Exception?>(), Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact(DisplayName = "A16 — Sale not found throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = new CancelSaleItemCommand { SaleId = Guid.NewGuid(), ItemId = Guid.NewGuid() };
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "A17 — Item not found throws KeyNotFoundException")]
    public async Task Handle_ItemNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var sale = CancelSaleHandlerTestData.GenerateActiveSale();
        var command = new CancelSaleItemCommand { SaleId = sale.Id, ItemId = Guid.NewGuid() };
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "A18 — Sale already cancelled throws InvalidOperationException")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var sale = CancelSaleHandlerTestData.GenerateCancelledSale();
        var itemId = sale.Items[0].Id;
        var command = new CancelSaleItemCommand { SaleId = sale.Id, ItemId = itemId };
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact(DisplayName = "A19 — Item already cancelled throws InvalidOperationException")]
    public async Task Handle_CancelledItem_ThrowsInvalidOperationException()
    {
        // Given
        var sale = CancelSaleHandlerTestData.GenerateSaleWithActiveItem(out var itemId);
        sale.CancelItem(itemId);
        var command = new CancelSaleItemCommand { SaleId = sale.Id, ItemId = itemId };
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
