using CommunityToolkit.Mvvm.ComponentModel;

namespace ErpSystem.Desktop.ViewModels;

public abstract partial class WorkspaceViewModelBase : ViewModelBase, IWorkspace
{
    protected WorkspaceViewModelBase(
        string route,
        string moduleTitle,
        string title,
        string subtitle,
        bool canClose)
    {
        Route = route;
        ModuleTitle = moduleTitle;
        Title = title;
        Subtitle = subtitle;
        CanClose = canClose;
    }

    public string Route { get; }
    public string ModuleTitle { get; }
    public string Title { get; }
    public string Subtitle { get; }
    public bool CanClose { get; }

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private bool isPinned;

    public long LastAccessSequence { get; set; }

    public string TabBackground => IsSelected ? "#FFFFFF" : "#E8F0FB";
    public string TabBorderBrush => IsSelected ? "#0F6CBD" : "#D5E0F0";
    public string TabForeground => IsSelected ? "#123B6A" : "#35506F";
    public string PinGlyph => IsPinned ? "★" : "☆";

    partial void OnIsSelectedChanged(bool value)
    {
        OnPropertyChanged(nameof(TabBackground));
        OnPropertyChanged(nameof(TabBorderBrush));
        OnPropertyChanged(nameof(TabForeground));
    }

    partial void OnIsPinnedChanged(bool value)
    {
        OnPropertyChanged(nameof(PinGlyph));
    }
}
