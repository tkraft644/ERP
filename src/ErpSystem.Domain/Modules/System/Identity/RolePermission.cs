namespace ErpSystem.Domain.Modules.System.Identity;

public sealed class RolePermission : Common.AuditableEntity
{
    public RolePermission(int roleId, int permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}
