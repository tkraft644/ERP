namespace ErpSystem.Domain.Modules.System.Identity;

public sealed class User : Common.AuditableEntity
{
    private User()
    {
    }

    public User(
        string userName,
        string email,
        string displayName,
        bool isActive = true)
    {
        UserName = userName;
        Email = email;
        DisplayName = displayName;
        IsActive = isActive;
    }

    public string UserName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string PasswordSalt { get; private set; } = string.Empty;
    public bool MustChangePassword { get; private set; }
    public bool IsActive { get; private set; }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void UpdateProfile(string userName, string email, string displayName, bool isActive)
    {
        UserName = userName;
        Email = email;
        DisplayName = displayName;
        IsActive = isActive;
    }

    public void SetPassword(string passwordHash, string passwordSalt, bool mustChangePassword)
    {
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        MustChangePassword = mustChangePassword;
    }

    public void RequirePasswordChange() => MustChangePassword = true;
    public void MarkPasswordChanged() => MustChangePassword = false;
}
