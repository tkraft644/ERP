using ErpSystem.Desktop.ViewModels;

namespace ErpSystem.Desktop.Services;

public sealed class NavigationService
{
    private Dictionary<string, WorkspaceDescriptor> descriptors = new(StringComparer.OrdinalIgnoreCase);

    public void Register(IEnumerable<WorkspaceDescriptor> items)
    {
        descriptors = items.ToDictionary(item => item.Route, StringComparer.OrdinalIgnoreCase);
    }

    public WorkspaceDescriptor? GetDescriptor(string route)
    {
        descriptors.TryGetValue(route, out var descriptor);
        return descriptor;
    }

    public IWorkspace? Create(string route) => GetDescriptor(route)?.Factory();

    public sealed record WorkspaceDescriptor(
        string Route,
        string Title,
        string Subtitle,
        Func<IWorkspace> Factory);
}
