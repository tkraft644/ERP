namespace ErpSystem.Domain.Modules.Contractors;

public sealed class Contractor : Common.AuditableEntity
{
    private Contractor()
    {
    }

    public Contractor(
        string code,
        string name,
        string? shortName,
        string? taxId,
        ContractorType types,
        bool isActive = true)
    {
        Code = code;
        Name = name;
        ShortName = shortName;
        TaxId = taxId;
        Types = types;
        IsActive = isActive;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? ShortName { get; private set; }
    public string? TaxId { get; private set; }
    public ContractorType Types { get; private set; }
    public bool IsActive { get; private set; }

    public List<ContractorAddress> Addresses { get; private set; } = [];
    public List<ContractorContact> Contacts { get; private set; } = [];
    public List<ContractorBankAccount> BankAccounts { get; private set; } = [];
    public List<ContractorNote> Notes { get; private set; } = [];

    public void Update(
        string code,
        string name,
        string? shortName,
        string? taxId,
        ContractorType types,
        bool isActive)
    {
        Code = code;
        Name = name;
        ShortName = shortName;
        TaxId = taxId;
        Types = types;
        IsActive = isActive;
    }

    public void ReplaceAddresses(IEnumerable<ContractorAddress> items)
    {
        Addresses.Clear();
        foreach (var item in items)
        {
            item.AssignTo(this);
            Addresses.Add(item);
        }
    }

    public void ReplaceContacts(IEnumerable<ContractorContact> items)
    {
        Contacts.Clear();
        foreach (var item in items)
        {
            item.AssignTo(this);
            Contacts.Add(item);
        }
    }

    public void ReplaceBankAccounts(IEnumerable<ContractorBankAccount> items)
    {
        BankAccounts.Clear();
        foreach (var item in items)
        {
            item.AssignTo(this);
            BankAccounts.Add(item);
        }
    }

    public void ReplaceNotes(IEnumerable<ContractorNote> items)
    {
        Notes.Clear();
        foreach (var item in items)
        {
            item.AssignTo(this);
            Notes.Add(item);
        }
    }
}
