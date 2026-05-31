namespace ErpSystem.Application.Modules.System.Foundation;

public sealed record PermissionView(
    string Name,
    string ModuleKey,
    string Resource,
    string Action,
    string Description);
