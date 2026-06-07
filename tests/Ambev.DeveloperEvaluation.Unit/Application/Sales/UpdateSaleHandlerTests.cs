using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMongoSaleRepository _mongoRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mongoRepo = Substitute.For<IMongoSaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        _handler = new UpdateSaleHandler(_saleRepository, _mongoRepo, _mapper, _logger);
    }

    [Fact(DisplayName = "A06 — Valid update returns UpdateSaleResult")]
    public async Task Handle_ValidCommand_ReturnsUpdatedResult()
    {
        // Given
        var sale = UpdateSaleHandlerTestData.GenerateActiveSale();
        var command = UpdateSaleHandlerTestData.GenerateValidCommand(sale.Id);
        var result = new UpdateSaleResult { Id = sale.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.GetByNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>()).Returns((Sale?)null);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<UpdateSaleResult>(sale).Returns(result);

        // When
        var response = await _handler.Handle(command, CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Id.Should().Be(sale.Id);
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "A07 — Sale not found throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "A08 — Update cancelled sale throws InvalidOperationException")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var sale = UpdateSaleHandlerTestData.GenerateCancelledSale();
        var command = UpdateSaleHandlerTestData.GenerateValidCommand(sale.Id);
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact(DisplayName = "A09 — SaleModifiedEvent is logged on success")]
    public async Task Handle_ValidCommand_LogsSaleModifiedEvent()
    {
        // Given
        var sale = UpdateSaleHandlerTestData.GenerateActiveSale();
        var command = UpdateSaleHandlerTestData.GenerateValidCommand(sale.Id);

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.GetByNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>()).Returns((Sale?)null);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<UpdateSaleResult>(sale).Returns(new UpdateSaleResult());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
