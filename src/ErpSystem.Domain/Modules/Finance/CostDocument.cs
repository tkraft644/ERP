using ErpSystem.Domain.Modules.Contractors;

namespace ErpSystem.Domain.Modules.Finance;

public sealed class CostDocument : Common.AuditableEntity
{
    private CostDocument()
    {
    }

    public CostDocument(
        string number,
        DateTime documentDate,
        DateTime postingDate,
        DateTime? dueDate,
        int? contractorId,
        string currencyCode,
        decimal? exchangeRate,
        string? externalNumber,
        string? description)
    {
        Number = number;
        DocumentDate = documentDate;
        PostingDate = postingDate;
        DueDate = dueDate;
        ContractorId = contractorId;
        CurrencyCode = currencyCode;
        ExchangeRate = exchangeRate;
        ExternalNumber = externalNumber;
        Description = description;
        Status = CostDocumentStatus.Draft;
    }

    public string Number { get; private set; } = string.Empty;
    public DateTime DocumentDate { get; private set; }
    public DateTime PostingDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public int? ContractorId { get; private set; }
    public Contractor? Contractor { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public decimal? ExchangeRate { get; private set; }
    public string? ExternalNumber { get; private set; }
    public string? Description { get; private set; }
    public CostDocumentStatus Status { get; private set; }

    public List<CostPosition> Positions { get; private set; } = [];
    public List<Settlement> Settlements { get; private set; } = [];

    public void UpdateCore(
        DateTime documentDate,
        DateTime postingDate,
        DateTime? dueDate,
        int? contractorId,
        string currencyCode,
        decimal? exchangeRate,
        string? externalNumber,
        string? description)
    {
        DocumentDate = documentDate;
        PostingDate = postingDate;
        DueDate = dueDate;
        ContractorId = contractorId;
        CurrencyCode = currencyCode;
        ExchangeRate = exchangeRate;
        ExternalNumber = externalNumber;
        Description = description;
    }

    public void ReplacePositions(IEnumerable<CostPosition> items)
    {
        Positions.Clear();
        foreach (var item in items)
        {
            item.AssignTo(this);
            Positions.Add(item);
        }
    }

    public void Approve()
    {
        Status = CostDocumentStatus.Approved;
    }
}
