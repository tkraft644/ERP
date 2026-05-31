using ErpSystem.Domain.Modules.Contractors;

namespace ErpSystem.Domain.Modules.Warehouse;

public sealed class WarehouseDocument : Common.AuditableEntity
{
    private WarehouseDocument()
    {
    }

    public WarehouseDocument(
        string number,
        WarehouseDocumentType type,
        DateTime documentDate,
        int? contractorId,
        int? sourceWarehouseId,
        int? sourceLocationId,
        int? targetWarehouseId,
        int? targetLocationId,
        string? externalReference,
        string? notes)
    {
        Number = number;
        Type = type;
        DocumentDate = documentDate;
        ContractorId = contractorId;
        SourceWarehouseId = sourceWarehouseId;
        SourceLocationId = sourceLocationId;
        TargetWarehouseId = targetWarehouseId;
        TargetLocationId = targetLocationId;
        ExternalReference = externalReference;
        Notes = notes;
        Status = WarehouseDocumentStatus.Draft;
    }

    public string Number { get; private set; } = string.Empty;
    public WarehouseDocumentType Type { get; private set; }
    public WarehouseDocumentStatus Status { get; private set; }
    public DateTime DocumentDate { get; private set; }
    public int? ContractorId { get; private set; }
    public Contractor? Contractor { get; private set; }
    public int? SourceWarehouseId { get; private set; }
    public Warehouse? SourceWarehouse { get; private set; }
    public int? SourceLocationId { get; private set; }
    public WarehouseLocation? SourceLocation { get; private set; }
    public int? TargetWarehouseId { get; private set; }
    public Warehouse? TargetWarehouse { get; private set; }
    public int? TargetLocationId { get; private set; }
    public WarehouseLocation? TargetLocation { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? PostedAtUtc { get; private set; }
    public int? PostedByUserId { get; private set; }

    public List<WarehouseDocumentPosition> Positions { get; private set; } = [];
    public List<StockMovement> StockMovements { get; private set; } = [];

    public void Update(
        WarehouseDocumentType type,
        DateTime documentDate,
        int? contractorId,
        int? sourceWarehouseId,
        int? sourceLocationId,
        int? targetWarehouseId,
        int? targetLocationId,
        string? externalReference,
        string? notes)
    {
        Type = type;
        DocumentDate = documentDate;
        ContractorId = contractorId;
        SourceWarehouseId = sourceWarehouseId;
        SourceLocationId = sourceLocationId;
        TargetWarehouseId = targetWarehouseId;
        TargetLocationId = targetLocationId;
        ExternalReference = externalReference;
        Notes = notes;
    }

    public void ReplacePositions(IEnumerable<WarehouseDocumentPosition> items)
    {
        Positions.Clear();
        foreach (var item in items)
        {
            item.AssignTo(this);
            Positions.Add(item);
        }
    }

    public void MarkAsPosted(int? userId, DateTime utcNow)
    {
        Status = WarehouseDocumentStatus.Posted;
        PostedByUserId = userId;
        PostedAtUtc = utcNow;
    }

    public void Archive()
    {
        Status = WarehouseDocumentStatus.Cancelled;
    }
}
