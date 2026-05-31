using ErpSystem.Domain.Modules.Contractors;

namespace ErpSystem.Domain.Modules.Finance;

public sealed class Payment : Common.AuditableEntity
{
    private Payment()
    {
    }

    public Payment(
        DateTime paymentDate,
        PaymentDirection direction,
        int? contractorId,
        string currencyCode,
        decimal? exchangeRate,
        decimal amount,
        string method,
        string? referenceNumber,
        string? notes)
    {
        PaymentDate = paymentDate;
        Direction = direction;
        ContractorId = contractorId;
        CurrencyCode = currencyCode;
        ExchangeRate = exchangeRate;
        Amount = amount;
        Method = method;
        ReferenceNumber = referenceNumber;
        Notes = notes;
    }

    public DateTime PaymentDate { get; private set; }
    public PaymentDirection Direction { get; private set; }
    public int? ContractorId { get; private set; }
    public Contractor? Contractor { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public decimal? ExchangeRate { get; private set; }
    public decimal Amount { get; private set; }
    public string Method { get; private set; } = string.Empty;
    public string? ReferenceNumber { get; private set; }
    public string? Notes { get; private set; }

    public List<Settlement> Settlements { get; private set; } = [];
}
