namespace ErpSystem.Application.Modules.Contractors;

public sealed record ContractorDetailsView(
    int Id,
    string Code,
    string Name,
    string? ShortName,
    string? TaxId,
    bool IsActive,
    IReadOnlyList<string> Types,
    IReadOnlyList<ContractorAddressView> Addresses,
    IReadOnlyList<ContractorContactView> Contacts,
    IReadOnlyList<ContractorBankAccountView> BankAccounts,
    IReadOnlyList<ContractorNoteView> Notes,
    byte[] RowVersion);
