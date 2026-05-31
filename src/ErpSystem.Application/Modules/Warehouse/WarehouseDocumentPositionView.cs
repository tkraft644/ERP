namespace ErpSystem.Application.Modules.Warehouse;

public sealed record WarehouseDocumentPositionView(
    int Id,
    int ProductId,
    string ProductCode,
    string ProductName,
    decimal Quantity,
    decimal? UnitPrice,
    int? SourceLocationId,
    string? SourceLocationCode,
    string? SourceLocationName,
    int? TargetLocationId,
    string? TargetLocationCode,
    string? TargetLocationName,
    string? Notes);
