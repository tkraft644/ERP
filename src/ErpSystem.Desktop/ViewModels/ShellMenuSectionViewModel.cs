using CommunityToolkit.Mvvm.ComponentModel;

namespace ErpSystem.Desktop.ViewModels;

public sealed partial class ShellMenuSectionViewModel : ViewModelBase
{
    public ShellMenuSectionViewModel(string key, string title, IReadOnlyList<ShellMenuItemViewModel> items, bool isExpanded = true)
    {
        Key = key;
        Title = title;
        Items = items;
        IsExpanded = isExpanded;
    }

    public string Key { get; }
    public string Title { get; }
    public IReadOnlyList<ShellMenuItemViewModel> Items { get; }
    public string ItemCountLabel => $"{Items.Count}";

    [ObservableProperty]
    private bool isExpanded;
}
