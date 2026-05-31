namespace ErpSystem.Application.Modules.System.Foundation;

public sealed record MenuSectionView(string Key, string Title, IReadOnlyList<MenuItemView> Items);
