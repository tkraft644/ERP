namespace ErpSystem.Application.Modules.Contractors;

public sealed record ContractorBankAccountView(
    int Id,
    string BankName,
    string AccountNumber,
    string CurrencyCode,
    string? Swift,
    bool IsPrimary);
