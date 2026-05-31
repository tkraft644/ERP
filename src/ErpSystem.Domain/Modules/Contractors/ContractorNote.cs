namespace ErpSystem.Domain.Modules.Contractors;

public sealed class ContractorNote : Common.AuditableEntity
{
    private ContractorNote()
    {
    }

    public ContractorNote(string title, string content)
    {
        Title = title;
        Content = content;
    }

    public int ContractorId { get; private set; }
    public Contractor Contractor { get; private set; } = null!;
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;

    internal void AssignTo(Contractor contractor)
    {
        Contractor = contractor;
        ContractorId = contractor.Id;
    }
}
