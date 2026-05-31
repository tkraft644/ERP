using ErpSystem.Shared.Navigation;

namespace ErpSystem.Desktop.Services;

public sealed class ShellMenuBuilder
{
    public IReadOnlyList<MenuSectionDescriptor> Build(IReadOnlyCollection<string> permissions)
    {
        var permissionSet = permissions.ToHashSet(StringComparer.OrdinalIgnoreCase);

        return MenuCatalog.All
            .Where(item => item.RequiredPermissions.All(permissionSet.Contains))
            .GroupBy(item => new { item.SectionKey, item.SectionTitle })
            .Select(group => new MenuSectionDescriptor(
                group.Key.SectionKey,
                group.Key.SectionTitle,
                group.Select(item => new MenuItemDescriptor(item.Title, item.Route)).ToArray()))
            .OrderBy(section => section.Title)
            .ToArray();
    }

    public sealed record MenuSectionDescriptor(string Key, string Title, IReadOnlyList<MenuItemDescriptor> Items);

    public sealed record MenuItemDescriptor(string Title, string Route);
}
