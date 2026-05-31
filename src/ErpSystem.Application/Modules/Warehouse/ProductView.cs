namespace ErpSystem.Application.Modules.Warehouse;

public sealed record ProductView(
    int Id,
    string Code,
    string Name,
    string? Sku,
    bool IsActive,
    decimal MinimumStockLevel,
    int ProductCategoryId,
    string ProductCategoryCode,
    string ProductCategoryName,
    int UnitOfMeasureId,
    string UnitOfMeasureCode,
    string UnitOfMeasureSymbol,
    DateTime LastUpdatedAtUtc,
    byte[] RowVersion);
