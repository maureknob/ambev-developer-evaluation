using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Rebus.Bus;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMongoSaleRepository _mongoRepo;
    private readonly ISaleCacheService _cache;
    private readonly IBus _bus;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mongoRepo = Substitute.For<IMongoSaleRepository>();
        _cache = Substitute.For<ISaleCacheService>();
        _bus = Substitute.For<IBus>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _handler = new CreateSaleHandler(_saleRepository, _mongoRepo, _cache, _bus, _mapper, _logger);
    }

    [Fact(DisplayName = "A01 — Valid command returns CreateSaleResult with correct Id")]
    public async Task Handle_ValidCommand_ReturnsResultWithId()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale { SaleNumber = command.SaleNumber };
        var result = new CreateSaleResult { Id = sale.Id };

        _saleRepository.GetByNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>()).Returns((Sale?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(result);

        // When
        var response = await _handler.Handle(command, CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Id.Should().Be(result.Id);
    }

    [Fact(DisplayName = "A02 — Invalid command throws ValidationException")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateInvalidCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "A03 — Duplicate saleNumber throws InvalidOperationException")]
    public async Task Handle_DuplicateSaleNumber_ThrowsInvalidOperationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        _saleRepository.GetByNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(new Sale { SaleNumber = command.SaleNumber });

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*{command.SaleNumber}*");
    }

    [Fact(DisplayName = "A04 — Repository CreateAsync called exactly once")]
    public async Task Handle_ValidCommand_CallsRepositoryOnce()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale { SaleNumber = command.SaleNumber };

        _saleRepository.GetByNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>()).Returns((Sale?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(new CreateSaleResult());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "A05 — SaleCreatedEvent is logged on success")]
    public async Task Handle_ValidCommand_LogsSaleCreatedEvent()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale { SaleNumber = command.SaleNumber };

        _saleRepository.GetByNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>()).Returns((Sale?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(new CreateSaleResult());

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
