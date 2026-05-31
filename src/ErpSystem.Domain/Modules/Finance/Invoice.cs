using ErpSystem.Domain.Modules.Contractors;
using ErpSystem.Domain.Modules.Transport;

namespace ErpSystem.Domain.Modules.Finance;

public sealed class Invoice : Common.AuditableEntity
{
    private Invoice()
    {
    }

    public Invoice(
        string number,
        DateTime invoiceDate,
        DateTime saleDate,
        DateTime dueDate,
        int contractorId,
        int? transportOrderId,
        string currencyCode,
        decimal? exchangeRate,
        string? externalNumber,
        string? description)
    {
        Number = number;
        InvoiceDate = invoiceDate;
        SaleDate = saleDate;
        DueDate = dueDate;
        ContractorId = contractorId;
        TransportOrderId = transportOrderId;
        CurrencyCode = currencyCode;
        ExchangeRate = exchangeRate;
        ExternalNumber = externalNumber;
        Description = description;
        Status = InvoiceStatus.Issued;
    }

    public string Number { get; private set; } = string.Empty;
    public DateTime InvoiceDate { get; private set; }
    public DateTime SaleDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public int ContractorId { get; private set; }
    public Contractor Contractor { get; private set; } = null!;
    public int? TransportOrderId { get; private set; }
    public TransportOrder? TransportOrder { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public decimal? ExchangeRate { get; private set; }
    public string? ExternalNumber { get; private set; }
    public string? Description { get; private set; }
    public InvoiceStatus Status { get; private set; }

    public List<InvoicePosition> Positions { get; private set; } = [];
    public List<Settlement> Settlements { get; private set; } = [];

    public void UpdateCore(
        DateTime invoiceDate,
        DateTime saleDate,
        DateTime dueDate,
        int contractorId,
        int? transportOrderId,
        string currencyCode,
        decimal? exchangeRate,
        string? externalNumber,
        string? description)
    {
        InvoiceDate = invoiceDate;
        SaleDate = saleDate;
        DueDate = dueDate;
        ContractorId = contractorId;
        TransportOrderId = transportOrderId;
        CurrencyCode = currencyCode;
        ExchangeRate = exchangeRate;
        ExternalNumber = externalNumber;
        Description = description;
    }

    public void ReplacePositions(IEnumerable<InvoicePosition> items)
    {
        Positions.Clear();
        foreach (var item in items)
        {
            item.AssignTo(this);
            Positions.Add(item);
        }
    }

    public decimal GetGrossTotal()
        => Positions.Sum(item => item.GrossAmount);

    public void RecalculateStatus(decimal settledAmount)
    {
        var grossTotal = GetGrossTotal();
        if (settledAmount <= 0m)
        {
            Status = InvoiceStatus.Issued;
            return;
        }

        Status = settledAmount >= grossTotal ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
    }
}
