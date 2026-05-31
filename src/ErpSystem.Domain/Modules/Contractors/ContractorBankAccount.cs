namespace ErpSystem.Domain.Modules.Contractors;

public sealed class ContractorBankAccount : Common.AuditableEntity
{
    private ContractorBankAccount()
    {
    }

    public ContractorBankAccount(
        string bankName,
        string accountNumber,
        string currencyCode,
        string? swift,
        bool isPrimary = false)
    {
        BankName = bankName;
        AccountNumber = accountNumber;
        CurrencyCode = currencyCode;
        Swift = swift;
        IsPrimary = isPrimary;
    }

    public int ContractorId { get; private set; }
    public Contractor Contractor { get; private set; } = null!;
    public string BankName { get; private set; } = string.Empty;
    public string AccountNumber { get; private set; } = string.Empty;
    public string CurrencyCode { get; private set; } = string.Empty;
    public string? Swift { get; private set; }
    public bool IsPrimary { get; private set; }

    internal void AssignTo(Contractor contractor)
    {
        Contractor = contractor;
        ContractorId = contractor.Id;
    }
}
