namespace ErpSystem.Desktop.ViewModels;

public abstract class ModuleWorkspaceViewModelBase : WorkspaceViewModelBase
{
    protected ModuleWorkspaceViewModelBase(
        string route,
        string moduleTitle,
        string title,
        string subtitle,
        bool canClose,
        string description,
        string badgeText,
        string accentColor,
        string accentSurfaceColor,
        IReadOnlyList<WorkspaceMetricViewModel> metrics,
        IReadOnlyList<WorkspaceActionViewModel> actions,
        IReadOnlyList<WorkspaceActivityViewModel> activity)
        : base(route, moduleTitle, title, subtitle, canClose)
    {
        Description = description;
        BadgeText = badgeText;
        AccentColor = accentColor;
        AccentSurfaceColor = accentSurfaceColor;
        Metrics = metrics;
        Actions = actions;
        Activity = activity;
    }

    public string WorkspaceTitle => Title;
    public string Description { get; }
    public string BadgeText { get; }
    public string AccentColor { get; }
    public string AccentSurfaceColor { get; }
    public IReadOnlyList<WorkspaceMetricViewModel> Metrics { get; }
    public IReadOnlyList<WorkspaceActionViewModel> Actions { get; }
    public IReadOnlyList<WorkspaceActivityViewModel> Activity { get; }

    public static IReadOnlyList<WorkspaceActionViewModel> BuildActions(
        string activeTitle,
        string accentColor,
        string accentSurfaceColor,
        params (string Title, string Caption)[] definitions)
    {
        return definitions
            .Select(item =>
            {
                var isActive = string.Equals(item.Title, activeTitle, StringComparison.OrdinalIgnoreCase);
                return new WorkspaceActionViewModel(
                    item.Title,
                    item.Caption,
                    isActive ? accentSurfaceColor : "#FFFFFF",
                    isActive ? accentColor : "#D7E2F2",
                    isActive ? accentColor : "#24364B");
            })
            .ToArray();
    }
}

public sealed record WorkspaceMetricViewModel(string Label, string Value, string Hint, string AccentColor);

public sealed record WorkspaceActionViewModel(
    string Title,
    string Caption,
    string Background,
    string BorderBrush,
    string Foreground);

public sealed record WorkspaceActivityViewModel(
    string Title,
    string Detail,
    string State,
    string StateColor);
