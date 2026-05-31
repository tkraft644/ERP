namespace ErpSystem.Domain.Modules.System.Identity;

public sealed class Permission : Common.AuditableEntity
{
    public Permission(
        string name,
        string moduleKey,
        string resource,
        string action,
        string description)
    {
        Name = name;
        ModuleKey = moduleKey;
        Resource = resource;
        Action = action;
        Description = description;
    }

    public string Name { get; set; }
    public string ModuleKey { get; set; }
    public string Resource { get; set; }
    public string Action { get; set; }
    public string Description { get; set; }
}
