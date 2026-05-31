namespace ErpSystem.Domain.Modules.Warehouse;

public sealed class WarehouseDocumentPosition : Common.AuditableEntity
{
    private WarehouseDocumentPosition()
    {
    }

    public WarehouseDocumentPosition(
        int productId,
        decimal quantity,
        decimal? unitPrice,
        int? sourceLocationId,
        int? targetLocationId,
        string? notes)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        SourceLocationId = sourceLocationId;
        TargetLocationId = targetLocationId;
        Notes = notes;
    }

    public int WarehouseDocumentId { get; private set; }
    public WarehouseDocument WarehouseDocument { get; private set; } = null!;
    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public decimal? UnitPrice { get; private set; }
    public int? SourceLocationId { get; private set; }
    public WarehouseLocation? SourceLocation { get; private set; }
    public int? TargetLocationId { get; private set; }
    public WarehouseLocation? TargetLocation { get; private set; }
    public string? Notes { get; private set; }

    internal void AssignTo(WarehouseDocument document)
    {
        WarehouseDocument = document;
        WarehouseDocumentId = document.Id;
    }
}
