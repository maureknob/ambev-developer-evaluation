using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Set<Sale>().AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Sale>()
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetByNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Sale>()
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Set<Sale>().Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        _context.Set<Sale>().Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(IEnumerable<Sale> Items, int TotalCount)> GetPagedAsync(
        int page, int size, string order, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Sale>().Include(s => s.Items).AsQueryable();

        query = ApplyOrdering(query, order);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    private static IQueryable<Sale> ApplyOrdering(IQueryable<Sale> query, string order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return query.OrderByDescending(s => s.SaleDate);

        var parts = order.Trim().Split(' ', 2);
        var field = parts[0].ToLowerInvariant();
        var desc = parts.Length > 1 && parts[1].ToLowerInvariant() == "desc";

        return field switch
        {
            "salenumber" => desc ? query.OrderByDescending(s => s.SaleNumber) : query.OrderBy(s => s.SaleNumber),
            "totalamount" => desc ? query.OrderByDescending(s => s.TotalAmount) : query.OrderBy(s => s.TotalAmount),
            _ => desc ? query.OrderByDescending(s => s.SaleDate) : query.OrderBy(s => s.SaleDate),
        };
    }
}
