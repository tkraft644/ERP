namespace ErpSystem.Desktop.Services;

public sealed class SessionService
{
    public event EventHandler? Changed;

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);
    public int UserId { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string Token { get; private set; } = string.Empty;
    public bool MustChangePassword { get; private set; }
    public IReadOnlyList<string> Roles { get; private set; } = [];
    public IReadOnlyList<string> Permissions { get; private set; } = [];

    public void SetSession(AuthenticatedSession session)
    {
        UserId = session.UserId;
        UserName = session.UserName;
        DisplayName = session.DisplayName;
        Token = session.Token;
        MustChangePassword = session.MustChangePassword;
        Roles = session.Roles;
        Permissions = session.Permissions;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void Clear()
    {
        UserId = 0;
        UserName = string.Empty;
        DisplayName = string.Empty;
        Token = string.Empty;
        MustChangePassword = false;
        Roles = [];
        Permissions = [];
        Changed?.Invoke(this, EventArgs.Empty);
    }
}

public sealed record AuthenticatedSession(
    string Token,
    int UserId,
    string UserName,
    string DisplayName,
    bool MustChangePassword,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);
