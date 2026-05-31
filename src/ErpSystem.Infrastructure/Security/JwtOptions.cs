namespace ErpSystem.Infrastructure.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "ErpSystem";
    public string Audience { get; set; } = "ErpSystemDesktop";
    public string Key { get; set; } = string.Empty;
}
