namespace ErpSystem.Application.Modules.Transport;

public sealed record TransportRouteView(
    int Id,
    decimal? PlannedDistanceKm,
    decimal? PlannedRevenue,
    DateTime? PlannedLoadingAtUtc,
    DateTime? PlannedUnloadingAtUtc,
    string? RouteSummary);

public sealed record TransportStopView(
    int Id,
    int Sequence,
    string StopType,
    int? ContractorId,
    string? ContractorName,
    string Name,
    string CountryCode,
    string City,
    string AddressLine,
    DateTime? PlannedAtUtc,
    DateTime? ActualAtUtc,
    string? Notes);

public sealed record TransportDocumentView(
    int Id,
    string DocumentType,
    string? DocumentNumber,
    string? FileName,
    DateTime? IssuedAtUtc,
    DateTime? ReceivedAtUtc,
    bool IsRequired);

public sealed record TransportCostView(
    int Id,
    string CostType,
    string Description,
    string CurrencyCode,
    decimal Amount,
    decimal? ExchangeRate,
    bool IsLocalCost);

public sealed record TransportStatusHistoryView(
    int Id,
    string Status,
    int? ChangedByUserId,
    DateTime ChangedAtUtc,
    string? Note);

public sealed record TransportOrderListItemView(
    int Id,
    string Number,
    string OrderType,
    string Status,
    DateTime OrderDate,
    string? CarrierName,
    string? DriverName,
    string? VehicleRegistrationNumber,
    string? LoadingCountryCode,
    string? UnloadingCountryCode,
    string? DomesticRegion,
    string? CurrencyCode,
    decimal TotalCostAmount,
    byte[] RowVersion);

public sealed record TransportOrderDetailsView(
    int Id,
    string Number,
    string OrderType,
    string Status,
    DateTime OrderDate,
    int? CarrierId,
    string? CarrierName,
    int? VehicleId,
    string? VehicleRegistrationNumber,
    int? TrailerId,
    string? TrailerRegistrationNumber,
    int? DriverId,
    string? DriverName,
    string? LoadingCountryCode,
    string? UnloadingCountryCode,
    string? Incoterms,
    string? CurrencyCode,
    decimal? ExchangeRate,
    bool RequiresCustomsClearance,
    bool RequiresCmrDocuments,
    string? DomesticRegion,
    string? DomesticTransportKind,
    string? ExternalReference,
    string? Notes,
    TransportRouteView? Route,
    IReadOnlyList<TransportStopView> Stops,
    IReadOnlyList<TransportDocumentView> Documents,
    IReadOnlyList<TransportCostView> Costs,
    IReadOnlyList<TransportStatusHistoryView> StatusHistory,
    byte[] RowVersion);
