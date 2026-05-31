namespace ErpSystem.Application.Modules.System.Foundation;

public sealed record UserAccessProfile(
    int UserId,
    string UserName,
    string DisplayName,
    string Email,
    bool IsActive,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);
