using Ambev.DeveloperEvaluation.Domain.ReadModel;

namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

public interface ISaleCacheService
{
    Task<SaleDocument?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task SetAsync(SaleDocument document, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);
}
