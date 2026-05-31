using ErpSystem.Shared.Navigation;

namespace ErpSystem.Application.Modules.System.Foundation;

public sealed class PermissionMenuBuilder : IMenuBuilder
{
    public IReadOnlyList<MenuSectionView> Build(IReadOnlyCollection<string> permissions)
    {
        var permissionSet = permissions.ToHashSet(StringComparer.OrdinalIgnoreCase);

        return MenuCatalog.All
            .Where(item => item.RequiredPermissions.All(permissionSet.Contains))
            .GroupBy(item => new { item.SectionKey, item.SectionTitle })
            .Select(group => new MenuSectionView(
                group.Key.SectionKey,
                group.Key.SectionTitle,
                group.Select(item => new MenuItemView(item.Title, item.Route)).ToArray()))
            .OrderBy(section => section.Title)
            .ToArray();
    }
}
