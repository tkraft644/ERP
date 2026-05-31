using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.System;

public sealed partial class SystemUsersWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly SystemWorkspaceStore store = SystemWorkspaceStore.Instance;

    public SystemUsersWorkspaceViewModel()
        : base(
            "/system/users",
            "System",
            "Users",
            "Zarządzanie użytkownikami, aktywnością, resetami haseł i przypisaniem ról.",
            false)
    {
        SummaryCards = new ObservableCollection<SystemSummaryCardViewModel>();
        VisibleUsers = new ObservableCollection<SystemUserRowViewModel>();
        AssignedRoles = new ObservableCollection<SystemRoleRecord>();

        RunUserActionCommand = new RelayCommand<string>(RunUserAction);
        SelectUserCommand = new RelayCommand<SystemUserRowViewModel>(SelectUser);
        RemoveAssignedRoleCommand = new RelayCommand<SystemRoleRecord>(RemoveAssignedRole);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<SystemSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<SystemUserRowViewModel> VisibleUsers { get; }
    public ObservableCollection<SystemRoleRecord> AssignedRoles { get; }
    public IRelayCommand<string> RunUserActionCommand { get; }
    public IRelayCommand<SystemUserRowViewModel> SelectUserCommand { get; }
    public IRelayCommand<SystemRoleRecord> RemoveAssignedRoleCommand { get; }

    public IReadOnlyList<string> RoleFilterOptions => ["Wszystkie role", .. store.GetRoles().Select(item => item.Name)];
    public IReadOnlyList<SystemRoleRecord> AvailableRoles => store.GetRoles();
    public IReadOnlyList<SystemRoleRecord> RolesToAssign => AvailableRoles
        .Where(role => SelectedUserId is null || !AssignedRoles.Any(item => item.Id == role.Id))
        .ToArray();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedRoleFilter = "Wszystkie role";

    [ObservableProperty]
    private bool showInactive = true;

    [ObservableProperty]
    private SystemUserRowViewModel? selectedUser;

    [ObservableProperty]
    private int? selectedUserId;

    [ObservableProperty]
    private string editorUserName = string.Empty;

    [ObservableProperty]
    private string editorDisplayName = string.Empty;

    [ObservableProperty]
    private string editorEmail = string.Empty;

    [ObservableProperty]
    private bool editorIsActive = true;

    [ObservableProperty]
    private SystemRoleRecord? selectedRoleToAssign;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz konto albo utwórz nowe, aby zarządzać rolami i aktywnością.";

    public bool HasSelectedUser => SelectedUserId is not null;
    public string SelectedUserTitle => SelectedUser?.DisplayName ?? "Nowy użytkownik";
    public string SelectedUserRolesLabel => AssignedRoles.Count == 0 ? "Brak ról" : string.Join(", ", AssignedRoles.Select(item => item.Name));
    public string SelectedLastSignIn => SelectedUser?.LastSignInLabel ?? "-";
    public string SelectedPasswordReset => SelectedUser?.LastPasswordResetLabel ?? "-";

    partial void OnSearchTextChanged(string value) => RefreshUsers();
    partial void OnSelectedRoleFilterChanged(string value) => RefreshUsers();
    partial void OnShowInactiveChanged(bool value) => RefreshUsers();

    partial void OnSelectedUserChanged(SystemUserRowViewModel? value)
    {
        SelectedUserId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedUserIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasSelectedUser));
        OnPropertyChanged(nameof(SelectedUserTitle));
        OnPropertyChanged(nameof(SelectedUserRolesLabel));
        OnPropertyChanged(nameof(SelectedLastSignIn));
        OnPropertyChanged(nameof(SelectedPasswordReset));
        OnPropertyChanged(nameof(RolesToAssign));
    }

    private void OnStoreChanged(object? sender, EventArgs e)
    {
        Refresh();
    }

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshUsers();
        LoadEditor();
        OnPropertyChanged(nameof(RoleFilterOptions));
        OnPropertyChanged(nameof(AvailableRoles));
        OnPropertyChanged(nameof(RolesToAssign));
    }

    private void RefreshSummaryCards()
    {
        var users = store.GetUsers();
        SummaryCards.Clear();
        SummaryCards.Add(new SystemSummaryCardViewModel("Aktywni użytkownicy", users.Count(item => item.IsActive).ToString(), "Konta gotowe do pracy w systemie.", "#2563EB"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Administratorzy", users.Count(item => item.RoleIds.Contains(1)).ToString(), "Użytkownicy z pełnym dostępem do konfiguracji.", "#1F8A5B"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Nieaktywni", users.Count(item => !item.IsActive).ToString(), "Konta wyłączone lub wstrzymane.", "#D14343"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Role w użyciu", users.SelectMany(item => item.RoleIds).Distinct().Count().ToString(), "Role przypisane do co najmniej jednego użytkownika.", "#D97706"));
    }

    private void RefreshUsers()
    {
        var rows = store.GetUsers()
            .Where(user =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 user.UserName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 user.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 user.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (ShowInactive || user.IsActive) &&
                (SelectedRoleFilter == "Wszystkie role" ||
                 store.GetRoleNames(user.RoleIds).Contains(SelectedRoleFilter, StringComparer.OrdinalIgnoreCase)))
            .Select(user => new SystemUserRowViewModel(
                user.Id,
                user.UserName,
                user.DisplayName,
                user.Email,
                string.Join(", ", store.GetRoleNames(user.RoleIds)),
                user.IsActive ? "Aktywny" : "Nieaktywny",
                user.IsActive ? "#1F8A5B" : "#7B8794",
                user.LastSignInAt.ToLocalTime().ToString("dd.MM HH:mm"),
                user.LastPasswordResetAt.ToLocalTime().ToString("dd.MM HH:mm")))
            .ToArray();

        VisibleUsers.Clear();
        foreach (var row in rows)
        {
            VisibleUsers.Add(row);
        }

        if (SelectedUserId is not null)
        {
            SelectedUser = VisibleUsers.FirstOrDefault(item => item.Id == SelectedUserId) ?? VisibleUsers.FirstOrDefault();
        }
        else
        {
            SelectedUser = VisibleUsers.FirstOrDefault();
        }
    }

    private void LoadEditor()
    {
        AssignedRoles.Clear();

        if (SelectedUserId is null)
        {
            EditorUserName = string.Empty;
            EditorDisplayName = string.Empty;
            EditorEmail = string.Empty;
            EditorIsActive = true;
            SelectedRoleToAssign = RolesToAssign.FirstOrDefault();
            OnPropertyChanged(nameof(SelectedUserTitle));
            OnPropertyChanged(nameof(SelectedUserRolesLabel));
            OnPropertyChanged(nameof(SelectedLastSignIn));
            OnPropertyChanged(nameof(SelectedPasswordReset));
            return;
        }

        var user = store.GetUsers().FirstOrDefault(item => item.Id == SelectedUserId);
        if (user is null)
        {
            return;
        }

        EditorUserName = user.UserName;
        EditorDisplayName = user.DisplayName;
        EditorEmail = user.Email;
        EditorIsActive = user.IsActive;

        foreach (var role in store.GetRoles().Where(role => user.RoleIds.Contains(role.Id)).OrderBy(role => role.Name))
        {
            AssignedRoles.Add(role);
        }

        SelectedRoleToAssign = RolesToAssign.FirstOrDefault();
        OnPropertyChanged(nameof(SelectedUserTitle));
        OnPropertyChanged(nameof(SelectedUserRolesLabel));
        OnPropertyChanged(nameof(SelectedLastSignIn));
        OnPropertyChanged(nameof(SelectedPasswordReset));
        OnPropertyChanged(nameof(RolesToAssign));
    }

    private void RunUserAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedUser = null;
                    SelectedUserId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano formularz nowego użytkownika.";
                    break;
                case "Save":
                    SaveUser();
                    break;
                case "Delete":
                    DeleteUser();
                    break;
                case "Toggle":
                    ToggleUser();
                    break;
                case "Reset":
                    ResetPassword();
                    break;
                case "AddRole":
                    AddRole();
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }

    private void SaveUser()
    {
        if (SelectedUserId is null)
        {
            var initialRoleIds = AssignedRoles.Select(item => item.Id)
                .Concat(SelectedRoleToAssign is null ? [] : [SelectedRoleToAssign.Id])
                .Distinct()
                .ToArray();
            var created = store.CreateUser(EditorUserName, EditorDisplayName, EditorEmail, EditorIsActive, initialRoleIds);
            SelectedUserId = created.Id;
            LastActionMessage = $"Dodano użytkownika {created.DisplayName}.";
            return;
        }

        var updated = store.UpdateUser(SelectedUserId.Value, EditorUserName, EditorDisplayName, EditorEmail, EditorIsActive);
        LastActionMessage = $"Zapisano zmiany użytkownika {updated.DisplayName}.";
    }

    private void SelectUser(SystemUserRowViewModel? user)
    {
        SelectedUser = user;
    }

    private void DeleteUser()
    {
        if (SelectedUserId is null)
        {
            return;
        }

        store.DeleteUser(SelectedUserId.Value);
        SelectedUserId = null;
        SelectedUser = null;
        LoadEditor();
        LastActionMessage = "Usunięto użytkownika.";
    }

    private void ToggleUser()
    {
        if (SelectedUserId is null)
        {
            return;
        }

        store.ToggleUserActive(SelectedUserId.Value);
        LastActionMessage = "Zmieniono aktywność konta.";
    }

    private void ResetPassword()
    {
        if (SelectedUserId is null)
        {
            return;
        }

        store.ResetPassword(SelectedUserId.Value);
        LastActionMessage = "Zresetowano hasło użytkownika.";
    }

    private void AddRole()
    {
        if (SelectedUserId is null || SelectedRoleToAssign is null)
        {
            return;
        }

        store.AssignRoleToUser(SelectedUserId.Value, SelectedRoleToAssign.Id);
        LastActionMessage = $"Przypisano rolę {SelectedRoleToAssign.Name}.";
    }

    private void RemoveAssignedRole(SystemRoleRecord? role)
    {
        if (SelectedUserId is null || role is null)
        {
            return;
        }

        try
        {
            store.RemoveRoleFromUser(SelectedUserId.Value, role.Id);
            LastActionMessage = $"Odebrano rolę {role.Name}.";
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }
}

public sealed record SystemUserRowViewModel(
    int Id,
    string UserName,
    string DisplayName,
    string Email,
    string RolesLabel,
    string Status,
    string StatusColor,
    string LastSignInLabel,
    string LastPasswordResetLabel);
