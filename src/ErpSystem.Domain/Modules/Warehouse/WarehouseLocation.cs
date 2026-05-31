namespace ErpSystem.Domain.Modules.Warehouse;

public sealed class WarehouseLocation : Common.AuditableEntity
{
    private WarehouseLocation()
    {
    }

    public WarehouseLocation(int warehouseId, string code, string name, bool isActive = true)
    {
        WarehouseId = warehouseId;
        Code = code;
        Name = name;
        IsActive = isActive;
    }

    public int WarehouseId { get; private set; }
    public Warehouse Warehouse { get; private set; } = null!;
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
}
