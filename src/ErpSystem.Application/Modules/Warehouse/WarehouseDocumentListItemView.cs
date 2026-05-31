namespace ErpSystem.Application.Modules.Warehouse;

public sealed record WarehouseDocumentListItemView(
    int Id,
    string Number,
    string Type,
    string Status,
    DateTime DocumentDate,
    int? ContractorId,
    string? ContractorName,
    int? SourceWarehouseId,
    string? SourceWarehouseCode,
    int? TargetWarehouseId,
    string? TargetWarehouseCode,
    int PositionCount,
    byte[] RowVersion);
