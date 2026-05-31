using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;
using ErpSystem.Shared.Security;

namespace ErpSystem.Desktop.ViewModels.System;

public sealed partial class SystemPermissionsWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly SystemWorkspaceStore store = SystemWorkspaceStore.Instance;

    public SystemPermissionsWorkspaceViewModel()
        : base(
            "/system/permissions",
            "System",
            "Permissions",
            "Katalog uprawnień akcyjnych z możliwością nadawania i odbierania ich rolom.",
            false)
    {
        SummaryCards = new ObservableCollection<SystemSummaryCardViewModel>();
        VisiblePermissions = new ObservableCollection<SystemPermissionRowViewModel>();
        AssignedRoles = new ObservableCollection<SystemRoleRecord>();

        RunPermissionActionCommand = new RelayCommand<string>(RunPermissionAction);
        SelectPermissionCommand = new RelayCommand<SystemPermissionRowViewModel>(SelectPermission);
        RemoveRolePermissionCommand = new RelayCommand<SystemRoleRecord>(RemoveRolePermission);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<SystemSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<SystemPermissionRowViewModel> VisiblePermissions { get; }
    public ObservableCollection<SystemRoleRecord> AssignedRoles { get; }
    public IRelayCommand<string> RunPermissionActionCommand { get; }
    public IRelayCommand<SystemPermissionRowViewModel> SelectPermissionCommand { get; }
    public IRelayCommand<SystemRoleRecord> RemoveRolePermissionCommand { get; }

    public IReadOnlyList<string> ModuleFilterOptions => ["Wszystkie moduły", .. store.GetPermissions().Select(item => item.Module).Distinct().OrderBy(item => item)];
    public IReadOnlyList<string> ResourceFilterOptions => ["Wszystkie zasoby", .. store.GetPermissions().Select(item => item.Resource).Distinct().OrderBy(item => item)];
    public IReadOnlyList<SystemRoleRecord> RolesToAssign => store.GetRoles()
        .Where(role => SelectedPermissionName is not null && !AssignedRoles.Any(item => item.Id == role.Id))
        .ToArray();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedModuleFilter = "Wszystkie moduły";

    [ObservableProperty]
    private string selectedResourceFilter = "Wszystkie zasoby";

    [ObservableProperty]
    private SystemPermissionRowViewModel? selectedPermission;

    [ObservableProperty]
    private string? selectedPermissionName;

    [ObservableProperty]
    private SystemRoleRecord? selectedRoleToAssign;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz permission, aby sprawdzić przypisane role i zarządzać dostępem.";

    public bool HasSelectedPermission => SelectedPermissionName is not null;
    public string SelectedPermissionTitle => SelectedPermission?.Name ?? "-";
    public string SelectedPermissionModule => SelectedPermission?.Module ?? "-";
    public string SelectedPermissionResource => SelectedPermission?.Resource ?? "-";
    public string SelectedPermissionAction => SelectedPermission?.Action ?? "-";
    public string SelectedPermissionDescription => SelectedPermission?.Description ?? "Brak opisu.";

    partial void OnSearchTextChanged(string value) => RefreshPermissions();
    partial void OnSelectedModuleFilterChanged(string value) => RefreshPermissions();
    partial void OnSelectedResourceFilterChanged(string value) => RefreshPermissions();

    partial void OnSelectedPermissionChanged(SystemPermissionRowViewModel? value)
    {
        SelectedPermissionName = value?.Name;
        LoadSelection();
    }

    partial void OnSelectedPermissionNameChanged(string? value)
    {
        OnPropertyChanged(nameof(HasSelectedPermission));
        OnPropertyChanged(nameof(SelectedPermissionTitle));
        OnPropertyChanged(nameof(SelectedPermissionModule));
        OnPropertyChanged(nameof(SelectedPermissionResource));
        OnPropertyChanged(nameof(SelectedPermissionAction));
        OnPropertyChanged(nameof(SelectedPermissionDescription));
        OnPropertyChanged(nameof(RolesToAssign));
    }

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshPermissions();
        LoadSelection();
        OnPropertyChanged(nameof(ModuleFilterOptions));
        OnPropertyChanged(nameof(ResourceFilterOptions));
        OnPropertyChanged(nameof(RolesToAssign));
    }

    private void RefreshSummaryCards()
    {
        var permissions = store.GetPermissions();
        SummaryCards.Clear();
        SummaryCards.Add(new SystemSummaryCardViewModel("Permissiony", permissions.Count.ToString(), "Łączna liczba akcji dostępnych w katalogu.", "#2563EB"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Moduły", permissions.Select(item => item.Module).Distinct().Count().ToString(), "Obszary systemu korzystające z permissionów.", "#1F8A5B"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Przypisania", permissions.Sum(item => store.GetAssignedRoleCount(item.Name)).ToString(), "Nadania permissionów do ról.", "#D97706"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Bez przypisania", permissions.Count(item => store.GetAssignedRoleCount(item.Name) == 0).ToString(), "Uprawnienia nieużywane przez żadną rolę.", "#D14343"));
    }

    private void RefreshPermissions()
    {
        var rows = store.GetPermissions()
            .Where(permission =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 permission.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 permission.Resource.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 permission.Action.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedModuleFilter == "Wszystkie moduły" ||
                 string.Equals(permission.Module, SelectedModuleFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedResourceFilter == "Wszystkie zasoby" ||
                 string.Equals(permission.Resource, SelectedResourceFilter, StringComparison.OrdinalIgnoreCase)))
            .Select(permission => new SystemPermissionRowViewModel(
                permission.Name,
                permission.Module,
                permission.Resource,
                permission.Action,
                permission.Description,
                store.GetAssignedRoleCount(permission.Name)))
            .ToArray();

        VisiblePermissions.Clear();
        foreach (var row in rows)
        {
            VisiblePermissions.Add(row);
        }

        SelectedPermission = SelectedPermissionName is not null
            ? VisiblePermissions.FirstOrDefault(item => item.Name == SelectedPermissionName) ?? VisiblePermissions.FirstOrDefault()
            : VisiblePermissions.FirstOrDefault();
    }

    private void LoadSelection()
    {
        AssignedRoles.Clear();

        if (SelectedPermissionName is null)
        {
            SelectedRoleToAssign = null;
            return;
        }

        foreach (var role in store.GetRolesForPermission(SelectedPermissionName))
        {
            AssignedRoles.Add(role);
        }

        SelectedRoleToAssign = RolesToAssign.FirstOrDefault();
        OnPropertyChanged(nameof(RolesToAssign));
    }

    private void RunPermissionAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "Assign":
                    AssignRole();
                    break;
                case "Export":
                    LastActionMessage = "Wyeksportowano listę permissionów do arkusza i PDF.";
                    break;
                case "Refresh":
                    Refresh();
                    LastActionMessage = "Odświeżono katalog permissionów.";
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }

    private void AssignRole()
    {
        if (SelectedPermissionName is null || SelectedRoleToAssign is null)
        {
            return;
        }

        store.AssignPermissionToRole(SelectedRoleToAssign.Id, SelectedPermissionName);
        LastActionMessage = $"Nadano {SelectedPermissionName} do roli {SelectedRoleToAssign.Name}.";
    }

    private void SelectPermission(SystemPermissionRowViewModel? permission)
    {
        SelectedPermission = permission;
    }

    private void RemoveRolePermission(SystemRoleRecord? role)
    {
        if (SelectedPermissionName is null || role is null)
        {
            return;
        }

        store.RemovePermissionFromRole(role.Id, SelectedPermissionName);
        LastActionMessage = $"Odebrano {SelectedPermissionName} roli {role.Name}.";
    }
}

public sealed record SystemPermissionRowViewModel(
    string Name,
    string Module,
    string Resource,
    string Action,
    string Description,
    int AssignedRolesCount);
