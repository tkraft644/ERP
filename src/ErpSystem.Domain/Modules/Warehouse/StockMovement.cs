namespace ErpSystem.Domain.Modules.Warehouse;

public sealed class StockMovement : Common.AuditableEntity
{
    private StockMovement()
    {
    }

    public StockMovement(
        int warehouseDocumentId,
        int warehouseDocumentPositionId,
        int productId,
        int warehouseId,
        int? warehouseLocationId,
        decimal quantityDelta,
        decimal? unitPrice,
        DateTime movementDateUtc)
    {
        WarehouseDocumentId = warehouseDocumentId;
        WarehouseDocumentPositionId = warehouseDocumentPositionId;
        ProductId = productId;
        WarehouseId = warehouseId;
        WarehouseLocationId = warehouseLocationId;
        QuantityDelta = quantityDelta;
        UnitPrice = unitPrice;
        MovementDateUtc = movementDateUtc;
    }

    public int WarehouseDocumentId { get; private set; }
    public WarehouseDocument WarehouseDocument { get; private set; } = null!;
    public int WarehouseDocumentPositionId { get; private set; }
    public WarehouseDocumentPosition WarehouseDocumentPosition { get; private set; } = null!;
    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public int WarehouseId { get; private set; }
    public Warehouse Warehouse { get; private set; } = null!;
    public int? WarehouseLocationId { get; private set; }
    public WarehouseLocation? WarehouseLocation { get; private set; }
    public decimal QuantityDelta { get; private set; }
    public decimal? UnitPrice { get; private set; }
    public DateTime MovementDateUtc { get; private set; }
}
