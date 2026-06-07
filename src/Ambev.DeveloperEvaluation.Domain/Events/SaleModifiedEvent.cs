namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleModifiedEvent
{
    public Guid SaleId { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public decimal PreviousTotalAmount { get; init; }
    public decimal NewTotalAmount { get; init; }
    public DateTime OccurredAt { get; init; }
}
