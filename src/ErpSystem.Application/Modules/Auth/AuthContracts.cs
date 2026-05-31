namespace ErpSystem.Application.Modules.Auth;

public sealed record LoginRequest(string Login, string Password);

public sealed record LoginResponse(
    string Token,
    int UserId,
    string UserName,
    string DisplayName,
    bool MustChangePassword,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);
