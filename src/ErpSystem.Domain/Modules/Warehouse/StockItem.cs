namespace ErpSystem.Domain.Modules.Warehouse;

public sealed class StockItem : Common.AuditableEntity
{
    private StockItem()
    {
    }

    public StockItem(int warehouseId, int? warehouseLocationId, int productId, decimal quantityOnHand = 0m)
    {
        WarehouseId = warehouseId;
        WarehouseLocationId = warehouseLocationId;
        ProductId = productId;
        QuantityOnHand = quantityOnHand;
    }

    public int WarehouseId { get; private set; }
    public Warehouse Warehouse { get; private set; } = null!;
    public int? WarehouseLocationId { get; private set; }
    public WarehouseLocation? WarehouseLocation { get; private set; }
    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public decimal QuantityOnHand { get; private set; }
    public DateTime? LastMovementAtUtc { get; private set; }

    public void ApplyMovement(decimal quantityDelta, DateTime movementDateUtc)
    {
        QuantityOnHand += quantityDelta;
        LastMovementAtUtc = movementDateUtc;
    }
}
