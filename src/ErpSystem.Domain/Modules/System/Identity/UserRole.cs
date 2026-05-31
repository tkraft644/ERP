namespace ErpSystem.Domain.Modules.System.Identity;

public sealed class UserRole : Common.AuditableEntity
{
    public UserRole(int userId, int roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public int UserId { get; set; }
    public int RoleId { get; set; }
}
