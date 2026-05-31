namespace ErpSystem.Application.Modules.Contractors;

public sealed record ContractorAddressView(
    int Id,
    string Kind,
    string Label,
    string CountryCode,
    string PostalCode,
    string City,
    string Street,
    string BuildingNumber,
    string? ApartmentNumber,
    bool IsPrimary);
