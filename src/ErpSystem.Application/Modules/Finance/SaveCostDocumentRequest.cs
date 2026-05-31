namespace ErpSystem.Application.Modules.Finance;

public sealed record SaveCostDocumentRequest(
    DateTime DocumentDate,
    DateTime PostingDate,
    DateTime? DueDate,
    int? ContractorId,
    string CurrencyCode,
    decimal? ExchangeRate,
    string? ExternalNumber,
    string? Description,
    IReadOnlyList<SaveCostPositionRequest> Positions,
    byte[]? RowVersion = null);

public sealed record SaveCostPositionRequest(
    int LineNumber,
    string CostCategory,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal TaxRate,
    int? TransportOrderId,
    int? VehicleId,
    int? EmployeeId,
    int? WarehouseId,
    int? ContractorId,
    int? DepartmentId,
    string? Notes);
