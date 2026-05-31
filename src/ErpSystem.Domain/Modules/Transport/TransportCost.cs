namespace ErpSystem.Domain.Modules.Transport;

public sealed class TransportCost : Common.AuditableEntity
{
    private TransportCost()
    {
    }

    public TransportCost(
        string costType,
        string description,
        string currencyCode,
        decimal amount,
        decimal? exchangeRate,
        bool isLocalCost)
    {
        CostType = costType;
        Description = description;
        CurrencyCode = currencyCode;
        Amount = amount;
        ExchangeRate = exchangeRate;
        IsLocalCost = isLocalCost;
    }

    public int TransportOrderId { get; private set; }
    public TransportOrder TransportOrder { get; private set; } = null!;
    public string CostType { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string CurrencyCode { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public decimal? ExchangeRate { get; private set; }
    public bool IsLocalCost { get; private set; }

    internal void AssignTo(TransportOrder order)
    {
        TransportOrder = order;
        TransportOrderId = order.Id;
    }
}
