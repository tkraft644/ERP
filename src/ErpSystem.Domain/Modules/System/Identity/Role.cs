namespace ErpSystem.Domain.Modules.System.Identity;

public sealed class Role : Common.AuditableEntity
{
    public Role(
        string code,
        string name,
        string description,
        bool isSystem = false)
    {
        Code = code;
        Name = name;
        Description = description;
        IsSystem = isSystem;
    }

    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsSystem { get; set; }
}
