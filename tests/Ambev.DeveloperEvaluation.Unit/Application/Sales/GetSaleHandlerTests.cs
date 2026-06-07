using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.ReadModel;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class GetSaleHandlerTests
{
    private readonly IMongoSaleRepository _mongoRepo;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _mongoRepo = Substitute.For<IMongoSaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_mongoRepo, _mapper);
    }

    [Fact(DisplayName = "A20 — Valid Id returns full GetSaleResult")]
    public async Task Handle_ValidId_ReturnsSaleResult()
    {
        // Given
        var document = new SaleDocument { Id = Guid.NewGuid(), SaleNumber = "SALE-001" };
        var command = new GetSaleCommand { Id = document.Id };
        var result = new GetSaleResult { Id = document.Id, SaleNumber = document.SaleNumber };

        _mongoRepo.GetByIdAsync(document.Id, Arg.Any<CancellationToken>()).Returns(document);
        _mapper.Map<GetSaleResult>(document).Returns(result);

        // When
        var response = await _handler.Handle(command, CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Id.Should().Be(document.Id);
        response.SaleNumber.Should().Be("SALE-001");
    }

    [Fact(DisplayName = "A21 — Sale not found throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = new GetSaleCommand { Id = Guid.NewGuid() };
        _mongoRepo.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((SaleDocument?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
