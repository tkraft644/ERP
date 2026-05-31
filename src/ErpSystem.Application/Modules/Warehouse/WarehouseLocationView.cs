namespace ErpSystem.Application.Modules.Warehouse;

public sealed record WarehouseLocationView(
    int Id,
    int WarehouseId,
    string Code,
    string Name,
    bool IsActive);
