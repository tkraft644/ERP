namespace ErpSystem.Application.Modules.Transport;

public sealed record TransportPartnerOptionView(int Id, string Code, string Name);

public sealed record CarrierOptionView(int Id, string Code, int ContractorId, string ContractorName, bool IsPreferred);

public sealed record VehicleOptionView(int Id, string RegistrationNumber, string DisplayName, int? CarrierId);

public sealed record TrailerOptionView(int Id, string RegistrationNumber, string TrailerType, int? CarrierId);

public sealed record DriverOptionView(int Id, string FullName, string? PhoneNumber, int? CarrierId);

public sealed record TransportOrderTypeOptionView(string Key, string Label);

public sealed record TransportStatusOptionView(string Key, string Label);

public sealed record TransportStopTypeOptionView(string Key, string Label);

public sealed record TransportDocumentTypeOptionView(string Key, string Label);

public sealed record TransportReferenceDataView(
    IReadOnlyList<TransportPartnerOptionView> Partners,
    IReadOnlyList<CarrierOptionView> Carriers,
    IReadOnlyList<VehicleOptionView> Vehicles,
    IReadOnlyList<TrailerOptionView> Trailers,
    IReadOnlyList<DriverOptionView> Drivers,
    IReadOnlyList<TransportOrderTypeOptionView> OrderTypes,
    IReadOnlyList<TransportStatusOptionView> Statuses,
    IReadOnlyList<TransportStopTypeOptionView> StopTypes,
    IReadOnlyList<TransportDocumentTypeOptionView> DocumentTypes);
