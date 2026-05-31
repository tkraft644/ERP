namespace ErpSystem.Desktop.ViewModels;

public sealed class ModuleWorkspaceViewModel : ModuleWorkspaceViewModelBase
{
    public ModuleWorkspaceViewModel(
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
        : base(route, moduleTitle, title, subtitle, canClose, description, badgeText, accentColor, accentSurfaceColor, metrics, actions, activity)
    {
    }
}
