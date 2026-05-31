namespace ErpSystem.Domain.Modules.HR;

public sealed class Department : Common.AuditableEntity
{
    private Department()
    {
    }

    public Department(string code, string name, string? description, bool isActive = true)
    {
        Code = code;
        Name = name;
        Description = description;
        IsActive = isActive;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
}
