namespace ErpSystem.Shared.Security;

public sealed record PermissionDefinition(
    string Name,
    string ModuleKey,
    string Resource,
    string Action,
    string Description)
{
    public string Module => ModuleKey;
}
