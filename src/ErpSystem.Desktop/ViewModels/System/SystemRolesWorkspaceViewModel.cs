using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;
using ErpSystem.Shared.Security;

namespace ErpSystem.Desktop.ViewModels.System;

public sealed partial class SystemRolesWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly SystemWorkspaceStore store = SystemWorkspaceStore.Instance;

    public SystemRolesWorkspaceViewModel()
        : base(
            "/system/roles",
            "System",
            "Roles",
            "Role, opisy i macierz uprawnień wykorzystywana przez wszystkie moduły systemu.",
            false)
    {
        SummaryCards = new ObservableCollection<SystemSummaryCardViewModel>();
        VisibleRoles = new ObservableCollection<SystemRoleRowViewModel>();
        AssignedPermissions = new ObservableCollection<PermissionDefinition>();
        RoleUsers = new ObservableCollection<SystemUserRecord>();

        RunRoleActionCommand = new RelayCommand<string>(RunRoleAction);
        SelectRoleCommand = new RelayCommand<SystemRoleRowViewModel>(SelectRole);
        RemovePermissionCommand = new RelayCommand<PermissionDefinition>(RemovePermission);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<SystemSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<SystemRoleRowViewModel> VisibleRoles { get; }
    public ObservableCollection<PermissionDefinition> AssignedPermissions { get; }
    public ObservableCollection<SystemUserRecord> RoleUsers { get; }
    public IRelayCommand<string> RunRoleActionCommand { get; }
    public IRelayCommand<SystemRoleRowViewModel> SelectRoleCommand { get; }
    public IRelayCommand<PermissionDefinition> RemovePermissionCommand { get; }

    public IReadOnlyList<string> ModuleFilterOptions => ["Wszystkie moduły", .. store.GetPermissions().Select(item => item.Module).Distinct().OrderBy(item => item)];
    public IReadOnlyList<PermissionDefinition> PermissionsToAssign => store.GetPermissions()
        .Where(item =>
            (SelectedPermissionModuleFilter == "Wszystkie moduły" ||
             string.Equals(item.Module, SelectedPermissionModuleFilter, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(PermissionSearchText) ||
             item.Name.Contains(PermissionSearchText, StringComparison.OrdinalIgnoreCase) ||
             item.Resource.Contains(PermissionSearchText, StringComparison.OrdinalIgnoreCase)) &&
            !AssignedPermissions.Any(permission => permission.Name == item.Name))
        .ToArray();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool showInactive = true;

    [ObservableProperty]
    private SystemRoleRowViewModel? selectedRole;

    [ObservableProperty]
    private int? selectedRoleId;

    [ObservableProperty]
    private string editorCode = string.Empty;

    [ObservableProperty]
    private string editorName = string.Empty;

    [ObservableProperty]
    private string editorDescription = string.Empty;

    [ObservableProperty]
    private bool editorIsActive = true;

    [ObservableProperty]
    private string selectedPermissionModuleFilter = "Wszystkie moduły";

    [ObservableProperty]
    private string permissionSearchText = string.Empty;

    [ObservableProperty]
    private PermissionDefinition? selectedPermissionToAssign;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz rolę albo utwórz nową, aby zarządzać dostępem.";

    public bool HasSelectedRole => SelectedRoleId is not null;
    public string SelectedRoleTitle => SelectedRole?.Name ?? "Nowa rola";
    public string SelectedRoleUsersLabel => RoleUsers.Count == 0 ? "Brak użytkowników" : string.Join(", ", RoleUsers.Select(item => item.DisplayName));

    partial void OnSearchTextChanged(string value) => RefreshRoles();
    partial void OnShowInactiveChanged(bool value) => RefreshRoles();
    partial void OnSelectedPermissionModuleFilterChanged(string value) => OnPropertyChanged(nameof(PermissionsToAssign));
    partial void OnPermissionSearchTextChanged(string value) => OnPropertyChanged(nameof(PermissionsToAssign));

    partial void OnSelectedRoleChanged(SystemRoleRowViewModel? value)
    {
        SelectedRoleId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedRoleIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasSelectedRole));
        OnPropertyChanged(nameof(SelectedRoleTitle));
        OnPropertyChanged(nameof(SelectedRoleUsersLabel));
        OnPropertyChanged(nameof(PermissionsToAssign));
    }

    private void OnStoreChanged(object? sender, EventArgs e)
    {
        Refresh();
    }

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshRoles();
        LoadEditor();
        OnPropertyChanged(nameof(ModuleFilterOptions));
        OnPropertyChanged(nameof(PermissionsToAssign));
    }

    private void RefreshSummaryCards()
    {
        var roles = store.GetRoles();
        SummaryCards.Clear();
        SummaryCards.Add(new SystemSummaryCardViewModel("Role aktywne", roles.Count(item => item.IsActive).ToString(), "Role dostępne do przypisania użytkownikom.", "#2563EB"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Role systemowe", roles.Count(item => item.IsSystem).ToString(), "Role chronione i gotowe od startu systemu.", "#1F8A5B"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Uprawnienia w użyciu", roles.SelectMany(item => item.PermissionNames).Distinct().Count().ToString(), "Permissiony realnie przypisane do ról.", "#D97706"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Role niestandardowe", roles.Count(item => !item.IsSystem).ToString(), "Role utworzone ręcznie pod organizację.", "#D14343"));
    }

    private void RefreshRoles()
    {
        var rows = store.GetRoles()
            .Where(role =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 role.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 role.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (ShowInactive || role.IsActive))
            .Select(role => new SystemRoleRowViewModel(
                role.Id,
                role.Code,
                role.Name,
                role.Description,
                role.IsActive ? "Aktywna" : "Nieaktywna",
                role.IsActive ? "#1F8A5B" : "#7B8794",
                store.GetUsersForRole(role.Id).Count,
                role.PermissionNames.Count,
                role.IsSystem ? "Systemowa" : "Niestandardowa"))
            .ToArray();

        VisibleRoles.Clear();
        foreach (var row in rows)
        {
            VisibleRoles.Add(row);
        }

        SelectedRole = SelectedRoleId is not null
            ? VisibleRoles.FirstOrDefault(item => item.Id == SelectedRoleId) ?? VisibleRoles.FirstOrDefault()
            : VisibleRoles.FirstOrDefault();
    }

    private void LoadEditor()
    {
        AssignedPermissions.Clear();
        RoleUsers.Clear();

        if (SelectedRoleId is null)
        {
            EditorCode = string.Empty;
            EditorName = string.Empty;
            EditorDescription = string.Empty;
            EditorIsActive = true;
            SelectedPermissionToAssign = null;
            OnPropertyChanged(nameof(SelectedRoleUsersLabel));
            return;
        }

        var role = store.GetRoles().FirstOrDefault(item => item.Id == SelectedRoleId);
        if (role is null)
        {
            return;
        }

        EditorCode = role.Code;
        EditorName = role.Name;
        EditorDescription = role.Description;
        EditorIsActive = role.IsActive;

        foreach (var permission in store.GetPermissions().Where(item => role.PermissionNames.Contains(item.Name)).OrderBy(item => item.Name))
        {
            AssignedPermissions.Add(permission);
        }

        foreach (var user in store.GetUsersForRole(role.Id))
        {
            RoleUsers.Add(user);
        }

        SelectedPermissionToAssign = PermissionsToAssign.FirstOrDefault();
        OnPropertyChanged(nameof(PermissionsToAssign));
        OnPropertyChanged(nameof(SelectedRoleUsersLabel));
    }

    private void RunRoleAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedRole = null;
                    SelectedRoleId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano formularz nowej roli.";
                    break;
                case "Save":
                    SaveRole();
                    break;
                case "Delete":
                    DeleteRole();
                    break;
                case "AddPermission":
                    AddPermission();
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }

    private void SaveRole()
    {
        if (SelectedRoleId is null)
        {
            var created = store.CreateRole(EditorCode, EditorName, EditorDescription, EditorIsActive);
            SelectedRoleId = created.Id;
            LastActionMessage = $"Dodano rolę {created.Name}.";
            return;
        }

        var updated = store.UpdateRole(SelectedRoleId.Value, EditorCode, EditorName, EditorDescription, EditorIsActive);
        LastActionMessage = $"Zapisano rolę {updated.Name}.";
    }

    private void SelectRole(SystemRoleRowViewModel? role)
    {
        SelectedRole = role;
    }

    private void DeleteRole()
    {
        if (SelectedRoleId is null)
        {
            return;
        }

        store.DeleteRole(SelectedRoleId.Value);
        SelectedRoleId = null;
        SelectedRole = null;
        LoadEditor();
        LastActionMessage = "Usunięto rolę.";
    }

    private void AddPermission()
    {
        if (SelectedRoleId is null || SelectedPermissionToAssign is null)
        {
            return;
        }

        store.AssignPermissionToRole(SelectedRoleId.Value, SelectedPermissionToAssign.Name);
        LastActionMessage = $"Nadano uprawnienie {SelectedPermissionToAssign.Name}.";
    }

    private void RemovePermission(PermissionDefinition? permission)
    {
        if (SelectedRoleId is null || permission is null)
        {
            return;
        }

        store.RemovePermissionFromRole(SelectedRoleId.Value, permission.Name);
        LastActionMessage = $"Odebrano uprawnienie {permission.Name}.";
    }
}

public sealed record SystemRoleRowViewModel(
    int Id,
    string Code,
    string Name,
    string Description,
    string Status,
    string StatusColor,
    int UserCount,
    int PermissionCount,
    string TypeLabel);
