namespace ErpSystem.Application.Modules.Warehouse;

public sealed record StockItemView(
    int Id,
    int WarehouseId,
    string WarehouseCode,
    string WarehouseName,
    int? WarehouseLocationId,
    string? WarehouseLocationCode,
    string? WarehouseLocationName,
    int ProductId,
    string ProductCode,
    string ProductName,
    string UnitOfMeasureSymbol,
    decimal QuantityOnHand,
    DateTime? LastMovementAtUtc);
