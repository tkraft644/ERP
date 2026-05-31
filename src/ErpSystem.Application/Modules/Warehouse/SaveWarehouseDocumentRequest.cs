namespace ErpSystem.Application.Modules.Warehouse;

public sealed record SaveWarehouseDocumentRequest(
    string Type,
    DateTime DocumentDate,
    int? ContractorId,
    int? SourceWarehouseId,
    int? SourceLocationId,
    int? TargetWarehouseId,
    int? TargetLocationId,
    string? ExternalReference,
    string? Notes,
    IReadOnlyList<SaveWarehouseDocumentPositionRequest> Positions,
    byte[]? RowVersion = null);

public sealed record SaveWarehouseDocumentPositionRequest(
    int ProductId,
    decimal Quantity,
    decimal? UnitPrice,
    int? SourceLocationId,
    int? TargetLocationId,
    string? Notes);

public sealed record PostWarehouseDocumentRequest(byte[] RowVersion);
