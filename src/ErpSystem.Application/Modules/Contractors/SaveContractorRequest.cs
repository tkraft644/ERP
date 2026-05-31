namespace ErpSystem.Application.Modules.Contractors;

public sealed record SaveContractorRequest(
    string Code,
    string Name,
    string? ShortName,
    string? TaxId,
    bool IsActive,
    IReadOnlyList<string> Types,
    IReadOnlyList<SaveContractorAddressRequest> Addresses,
    IReadOnlyList<SaveContractorContactRequest> Contacts,
    IReadOnlyList<SaveContractorBankAccountRequest> BankAccounts,
    IReadOnlyList<SaveContractorNoteRequest> Notes,
    byte[]? RowVersion = null);

public sealed record SaveContractorAddressRequest(
    string Kind,
    string Label,
    string CountryCode,
    string PostalCode,
    string City,
    string Street,
    string BuildingNumber,
    string? ApartmentNumber,
    bool IsPrimary);

public sealed record SaveContractorContactRequest(
    string FullName,
    string? Position,
    string? Email,
    string? PhoneNumber,
    bool IsPrimary);

public sealed record SaveContractorBankAccountRequest(
    string BankName,
    string AccountNumber,
    string CurrencyCode,
    string? Swift,
    bool IsPrimary);

public sealed record SaveContractorNoteRequest(
    string Title,
    string Content);
