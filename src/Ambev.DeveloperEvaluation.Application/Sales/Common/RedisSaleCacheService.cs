using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.ReadModel;
using Microsoft.Extensions.Caching.Distributed;

namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

public class RedisSaleCacheService : ISaleCacheService
{
    private readonly IDistributedCache _cache;
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);
    private static string Key(Guid id) => $"sale:{id}";

    public RedisSaleCacheService(IDistributedCache cache) => _cache = cache;

    public async Task<SaleDocument?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var bytes = await _cache.GetAsync(Key(id), cancellationToken);
        return bytes is null ? null : JsonSerializer.Deserialize<SaleDocument>(bytes);
    }

    public async Task SetAsync(SaleDocument document, CancellationToken cancellationToken = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(document);
        await _cache.SetAsync(Key(document.Id), bytes,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = Ttl },
            cancellationToken);
    }

    public Task RemoveAsync(Guid id, CancellationToken cancellationToken = default) =>
        _cache.RemoveAsync(Key(id), cancellationToken);
}
