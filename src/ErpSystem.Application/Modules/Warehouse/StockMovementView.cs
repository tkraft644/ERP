namespace ErpSystem.Application.Modules.Warehouse;

public sealed record StockMovementView(
    int Id,
    int ProductId,
    string ProductCode,
    string ProductName,
    int WarehouseId,
    string WarehouseCode,
    string WarehouseName,
    int? WarehouseLocationId,
    string? WarehouseLocationCode,
    string? WarehouseLocationName,
    decimal QuantityDelta,
    decimal? UnitPrice,
    DateTime MovementDateUtc);
