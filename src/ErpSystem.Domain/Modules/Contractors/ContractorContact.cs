namespace ErpSystem.Domain.Modules.Contractors;

public sealed class ContractorContact : Common.AuditableEntity
{
    private ContractorContact()
    {
    }

    public ContractorContact(
        string fullName,
        string? position,
        string? email,
        string? phoneNumber,
        bool isPrimary = false)
    {
        FullName = fullName;
        Position = position;
        Email = email;
        PhoneNumber = phoneNumber;
        IsPrimary = isPrimary;
    }

    public int ContractorId { get; private set; }
    public Contractor Contractor { get; private set; } = null!;
    public string FullName { get; private set; } = string.Empty;
    public string? Position { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public bool IsPrimary { get; private set; }

    internal void AssignTo(Contractor contractor)
    {
        Contractor = contractor;
        ContractorId = contractor.Id;
    }
}
