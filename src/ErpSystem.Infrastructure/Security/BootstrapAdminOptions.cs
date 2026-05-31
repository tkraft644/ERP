namespace ErpSystem.Infrastructure.Security;

public sealed class BootstrapAdminOptions
{
    public const string SectionName = "BootstrapAdmin";

    public string Login { get; set; } = "admin";
    public string Email { get; set; } = "admin@erpsystem.local";
    public string DisplayName { get; set; } = "Administrator";
    public string Password { get; set; } = string.Empty;
    public bool MustChangePassword { get; set; } = true;
    public bool ResetPasswordOnStartup { get; set; } = true;
}
