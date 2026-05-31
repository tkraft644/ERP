namespace ErpSystem.Application.Modules.Warehouse;

public sealed record SaveWarehouseProductRequest(
    string Code,
    string Name,
    string? Sku,
    bool IsActive,
    decimal MinimumStockLevel,
    int ProductCategoryId,
    int UnitOfMeasureId,
    byte[] RowVersion);

public sealed record RowVersionRequest(byte[] RowVersion);
