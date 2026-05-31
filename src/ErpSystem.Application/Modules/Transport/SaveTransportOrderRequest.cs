namespace ErpSystem.Application.Modules.Transport;

public sealed record SaveTransportOrderRequest(
    string OrderType,
    DateTime OrderDate,
    int? CarrierId,
    int? VehicleId,
    int? TrailerId,
    int? DriverId,
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
    SaveTransportRouteRequest Route,
    IReadOnlyList<SaveTransportStopRequest> Stops,
    IReadOnlyList<SaveTransportDocumentRequest> Documents,
    IReadOnlyList<SaveTransportCostRequest> Costs,
    byte[]? RowVersion = null);

public sealed record SaveTransportRouteRequest(
    decimal? PlannedDistanceKm,
    decimal? PlannedRevenue,
    DateTime? PlannedLoadingAtUtc,
    DateTime? PlannedUnloadingAtUtc,
    string? RouteSummary);

public sealed record SaveTransportStopRequest(
    int Sequence,
    string StopType,
    int? ContractorId,
    string Name,
    string CountryCode,
    string City,
    string AddressLine,
    DateTime? PlannedAtUtc,
    DateTime? ActualAtUtc,
    string? Notes);

public sealed record SaveTransportDocumentRequest(
    string DocumentType,
    string? DocumentNumber,
    string? FileName,
    DateTime? IssuedAtUtc,
    DateTime? ReceivedAtUtc,
    bool IsRequired);

public sealed record SaveTransportCostRequest(
    string CostType,
    string Description,
    string CurrencyCode,
    decimal Amount,
    decimal? ExchangeRate,
    bool IsLocalCost);

public sealed record ChangeTransportOrderStatusRequest(
    string Status,
    string? Note,
    byte[] RowVersion);
