namespace ErpSystem.Domain.Modules.HR;

public sealed class LeaveType : Common.AuditableEntity
{
    private LeaveType()
    {
    }

    public LeaveType(string code, string name, bool isPaid, bool requiresApproval, int sortOrder, bool isActive = true)
    {
        Code = code;
        Name = name;
        IsPaid = isPaid;
        RequiresApproval = requiresApproval;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsPaid { get; private set; }
    public bool RequiresApproval { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
}
