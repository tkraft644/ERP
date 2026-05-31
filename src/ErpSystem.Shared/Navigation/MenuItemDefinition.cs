namespace ErpSystem.Shared.Navigation;

public sealed record MenuItemDefinition(
    string SectionKey,
    string SectionTitle,
    string Title,
    string Route,
    IReadOnlyList<string> RequiredPermissions);
