namespace ErpSystem.Application.Modules.Contractors;

public sealed record ContractorListItemView(
    int Id,
    string Code,
    string Name,
    string? ShortName,
    string? TaxId,
    bool IsActive,
    IReadOnlyList<string> Types,
    string? PrimaryCity,
    string? PrimaryEmail,
    string? PrimaryPhoneNumber,
    int AddressCount,
    int ContactCount,
    byte[] RowVersion);
