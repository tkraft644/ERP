namespace ErpSystem.Domain.Modules.System.Documents;

public sealed class DocumentStatus : Common.AuditableEntity
{
    public DocumentStatus(
        string moduleKey,
        string code,
        string name,
        int sortOrder,
        bool isTerminal = false)
    {
        ModuleKey = moduleKey;
        Code = code;
        Name = name;
        SortOrder = sortOrder;
        IsTerminal = isTerminal;
    }

    public string ModuleKey { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int SortOrder { get; set; }
    public bool IsTerminal { get; set; }
}
