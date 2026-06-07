using Ambev.DeveloperEvaluation.Domain.ReadModel;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IMongoSaleRepository
{
    Task UpsertAsync(SaleDocument document, CancellationToken cancellationToken = default);
    Task<SaleDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<SaleDocument> Items, long TotalCount)> GetPagedAsync(
        int page, int size, string? order, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
