using Ambev.DeveloperEvaluation.Domain.ReadModel;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class MongoSaleRepository : IMongoSaleRepository
{
    private static bool _mapsRegistered;
    private static readonly object _lock = new();

    private readonly IMongoCollection<SaleDocument> _collection;

    public MongoSaleRepository(IMongoDatabase database)
    {
        RegisterClassMaps();
        _collection = database.GetCollection<SaleDocument>("sales");
    }

    private static void RegisterClassMaps()
    {
        if (_mapsRegistered) return;
        lock (_lock)
        {
            if (_mapsRegistered) return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(SaleDocument)))
            {
                BsonClassMap.RegisterClassMap<SaleDocument>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(d => d.Id)
                      .SetIdGenerator(GuidGenerator.Instance)
                      .SetSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
                });
            }

            _mapsRegistered = true;
        }
    }

    public async Task UpsertAsync(SaleDocument document, CancellationToken cancellationToken = default)
    {
        var filter = Builders<SaleDocument>.Filter.Eq(d => d.Id, document.Id);
        await _collection.ReplaceOneAsync(filter, document, new ReplaceOptions { IsUpsert = true }, cancellationToken);
    }

    public async Task<SaleDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<SaleDocument>.Filter.Eq(d => d.Id, id);
        var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(IEnumerable<SaleDocument> Items, long TotalCount)> GetPagedAsync(
        int page, int size, string? order, CancellationToken cancellationToken = default)
    {
        var filter = Builders<SaleDocument>.Filter.Empty;
        var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var sort = BuildSort(order);
        var items = await _collection
            .Find(filter)
            .Sort(sort)
            .Skip((page - 1) * size)
            .Limit(size)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<SaleDocument>.Filter.Eq(d => d.Id, id);
        await _collection.DeleteOneAsync(filter, cancellationToken);
    }

    private static SortDefinition<SaleDocument> BuildSort(string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return Builders<SaleDocument>.Sort.Descending(d => d.SaleDate);

        var parts = order.Trim().Split(' ', 2);
        var field = parts[0].ToLowerInvariant();
        var desc = parts.Length > 1 && parts[1].ToLowerInvariant() == "desc";

        return field switch
        {
            "salenumber" => desc
                ? Builders<SaleDocument>.Sort.Descending(d => d.SaleNumber)
                : Builders<SaleDocument>.Sort.Ascending(d => d.SaleNumber),
            "totalamount" => desc
                ? Builders<SaleDocument>.Sort.Descending(d => d.TotalAmount)
                : Builders<SaleDocument>.Sort.Ascending(d => d.TotalAmount),
            "createdat" => desc
                ? Builders<SaleDocument>.Sort.Descending(d => d.CreatedAt)
                : Builders<SaleDocument>.Sort.Ascending(d => d.CreatedAt),
            _ => desc
                ? Builders<SaleDocument>.Sort.Descending(d => d.SaleDate)
                : Builders<SaleDocument>.Sort.Ascending(d => d.SaleDate),
        };
    }
}
