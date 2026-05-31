using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.HR;

public sealed partial class HrEmployeesWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly HrWorkspaceStore store = HrWorkspaceStore.Instance;
    private static readonly IReadOnlyList<string> ContractTypeOptionsSource = ["Employment", "B2B", "Mandate", "Internship"];

    public HrEmployeesWorkspaceViewModel()
        : base(
            "/hr/employees",
            "HR",
            "Lista pracowników",
            "Kartoteka pracowników z aktywnością, profilem kierowcy i podstawą zatrudnienia.",
            false)
    {
        SummaryCards = new ObservableCollection<HrSummaryCardViewModel>();
        VisibleEmployees = new ObservableCollection<HrEmployeeRowViewModel>();
        RunEmployeeActionCommand = new RelayCommand<string>(RunEmployeeAction);
        SelectEmployeeCommand = new RelayCommand<HrEmployeeRowViewModel>(SelectEmployee);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<HrSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<HrEmployeeRowViewModel> VisibleEmployees { get; }
    public IRelayCommand<string> RunEmployeeActionCommand { get; }
    public IRelayCommand<HrEmployeeRowViewModel> SelectEmployeeCommand { get; }

    public IReadOnlyList<string> DepartmentFilterOptions => ["Wszystkie działy", .. store.GetDepartments().Select(item => item.Name)];
    public IReadOnlyList<HrDepartmentRecord> DepartmentOptions => store.GetDepartments().Where(item => item.IsActive).ToArray();
    public IReadOnlyList<HrPositionRecord> PositionOptions => store.GetPositions().Where(item => item.IsActive).ToArray();
    public IReadOnlyList<string> ContractTypeOptions => ContractTypeOptionsSource;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedDepartmentFilter = "Wszystkie działy";

    [ObservableProperty]
    private bool showInactive = true;

    [ObservableProperty]
    private HrEmployeeRowViewModel? selectedEmployee;

    [ObservableProperty]
    private int? selectedEmployeeId;

    [ObservableProperty]
    private string editorEmployeeNumber = string.Empty;

    [ObservableProperty]
    private string editorFirstName = string.Empty;

    [ObservableProperty]
    private string editorLastName = string.Empty;

    [ObservableProperty]
    private string editorEmail = string.Empty;

    [ObservableProperty]
    private string editorPhoneNumber = string.Empty;

    [ObservableProperty]
    private string editorPersonalId = string.Empty;

    [ObservableProperty]
    private HrDepartmentRecord? selectedDepartment;

    [ObservableProperty]
    private HrPositionRecord? selectedPosition;

    [ObservableProperty]
    private string selectedContractType = "Employment";

    [ObservableProperty]
    private bool editorIsActive = true;

    [ObservableProperty]
    private bool editorHasDriverProfile;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz pracownika lub dodaj nowe konto pracownicze.";

    public bool HasSelectedEmployee => SelectedEmployeeId is not null;
    public string SelectedEmployeeTitle => SelectedEmployee?.FullName ?? "Nowy pracownik";
    public string SelectedEmployeeDepartment => SelectedEmployee?.DepartmentName ?? "-";
    public string SelectedEmployeePosition => SelectedEmployee?.PositionName ?? "-";

    partial void OnSearchTextChanged(string value) => RefreshEmployees();
    partial void OnSelectedDepartmentFilterChanged(string value) => RefreshEmployees();
    partial void OnShowInactiveChanged(bool value) => RefreshEmployees();

    partial void OnSelectedEmployeeChanged(HrEmployeeRowViewModel? value)
    {
        SelectedEmployeeId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedEmployeeIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasSelectedEmployee));
        OnPropertyChanged(nameof(SelectedEmployeeTitle));
        OnPropertyChanged(nameof(SelectedEmployeeDepartment));
        OnPropertyChanged(nameof(SelectedEmployeePosition));
    }

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshEmployees();
        LoadEditor();
        OnPropertyChanged(nameof(DepartmentFilterOptions));
        OnPropertyChanged(nameof(DepartmentOptions));
        OnPropertyChanged(nameof(PositionOptions));
    }

    private void RefreshSummaryCards()
    {
        var employees = store.GetEmployees();
        var leaveRequests = store.GetLeaveRequests();
        var documents = store.GetDocuments();

        SummaryCards.Clear();
        SummaryCards.Add(new HrSummaryCardViewModel("Pracownicy aktywni", employees.Count(item => item.IsActive).ToString(), "Osoby gotowe do pracy operacyjnej i administracyjnej.", "#1D4ED8"));
        SummaryCards.Add(new HrSummaryCardViewModel("Profile kierowcy", employees.Count(item => item.HasDriverProfile).ToString(), "Pracownicy powiązani z transportem i dokumentami kierowców.", "#1F8A5B"));
        SummaryCards.Add(new HrSummaryCardViewModel("Otwarte urlopy", leaveRequests.Count(item => item.Status == "Submitted").ToString(), "Wnioski urlopowe oczekujące na decyzję.", "#D97706"));
        SummaryCards.Add(new HrSummaryCardViewModel("Dokumenty do odnowienia", documents.Count(item => item.IsExpiringSoon).ToString(), "Badania i uprawnienia wymagające reakcji.", "#D14343"));
    }

    private void RefreshEmployees()
    {
        var rows = store.GetEmployees()
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.EmployeeNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (ShowInactive || item.IsActive) &&
                (SelectedDepartmentFilter == "Wszystkie działy" ||
                 string.Equals(store.GetDepartmentName(item.DepartmentId), SelectedDepartmentFilter, StringComparison.OrdinalIgnoreCase)))
            .Select(item => new HrEmployeeRowViewModel(
                item.Id,
                item.EmployeeNumber,
                item.FullName,
                item.Email,
                store.GetDepartmentName(item.DepartmentId),
                store.GetPositionName(item.PositionId),
                item.PrimaryContractType ?? "-",
                item.StatusLabel,
                item.StatusColor,
                item.DriverLabel))
            .ToArray();

        VisibleEmployees.Clear();
        foreach (var row in rows)
        {
            VisibleEmployees.Add(row);
        }

        SelectedEmployee = SelectedEmployeeId is not null
            ? VisibleEmployees.FirstOrDefault(item => item.Id == SelectedEmployeeId) ?? VisibleEmployees.FirstOrDefault()
            : VisibleEmployees.FirstOrDefault();
    }

    private void LoadEditor()
    {
        if (SelectedEmployeeId is null)
        {
            EditorEmployeeNumber = string.Empty;
            EditorFirstName = string.Empty;
            EditorLastName = string.Empty;
            EditorEmail = string.Empty;
            EditorPhoneNumber = string.Empty;
            EditorPersonalId = string.Empty;
            SelectedDepartment = DepartmentOptions.FirstOrDefault();
            SelectedPosition = PositionOptions.FirstOrDefault();
            SelectedContractType = "Employment";
            EditorIsActive = true;
            EditorHasDriverProfile = false;
            return;
        }

        var employee = store.GetEmployees().FirstOrDefault(item => item.Id == SelectedEmployeeId);
        if (employee is null)
        {
            return;
        }

        EditorEmployeeNumber = employee.EmployeeNumber;
        EditorFirstName = employee.FirstName;
        EditorLastName = employee.LastName;
        EditorEmail = employee.Email;
        EditorPhoneNumber = employee.PhoneNumber ?? string.Empty;
        EditorPersonalId = employee.PersonalId ?? string.Empty;
        SelectedDepartment = DepartmentOptions.FirstOrDefault(item => item.Id == employee.DepartmentId) ?? DepartmentOptions.FirstOrDefault();
        SelectedPosition = PositionOptions.FirstOrDefault(item => item.Id == employee.PositionId) ?? PositionOptions.FirstOrDefault();
        SelectedContractType = employee.PrimaryContractType ?? "Employment";
        EditorIsActive = employee.IsActive;
        EditorHasDriverProfile = employee.HasDriverProfile;
    }

    private void RunEmployeeAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedEmployee = null;
                    SelectedEmployeeId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nową kartę pracownika.";
                    break;
                case "Save":
                    SaveEmployee();
                    break;
                case "Toggle":
                    if (SelectedEmployeeId is not null)
                    {
                        store.ToggleEmployee(SelectedEmployeeId.Value);
                        LastActionMessage = "Zmieniono aktywność pracownika.";
                    }
                    break;
                case "Delete":
                    if (SelectedEmployeeId is not null)
                    {
                        store.DeleteEmployee(SelectedEmployeeId.Value);
                        SelectedEmployeeId = null;
                        SelectedEmployee = null;
                        LoadEditor();
                        LastActionMessage = "Usunięto pracownika i jego powiązania HR.";
                    }
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }

    private void SaveEmployee()
    {
        if (SelectedEmployeeId is null)
        {
            var created = store.CreateEmployee(
                EditorEmployeeNumber,
                EditorFirstName,
                EditorLastName,
                EditorEmail,
                EditorPhoneNumber,
                EditorPersonalId,
                SelectedDepartment?.Id,
                SelectedPosition?.Id,
                SelectedContractType,
                EditorIsActive,
                EditorHasDriverProfile);
            SelectedEmployeeId = created.Id;
            LastActionMessage = $"Dodano pracownika {created.FullName}.";
            return;
        }

        var updated = store.UpdateEmployee(
            SelectedEmployeeId.Value,
            EditorEmployeeNumber,
            EditorFirstName,
            EditorLastName,
            EditorEmail,
            EditorPhoneNumber,
            EditorPersonalId,
            SelectedDepartment?.Id,
            SelectedPosition?.Id,
            SelectedContractType,
            EditorIsActive,
            EditorHasDriverProfile);
        LastActionMessage = $"Zapisano kartę pracownika {updated.FullName}.";
    }

    private void SelectEmployee(HrEmployeeRowViewModel? employee)
    {
        SelectedEmployee = employee;
    }
}

public sealed record HrEmployeeRowViewModel(
    int Id,
    string EmployeeNumber,
    string FullName,
    string Email,
    string DepartmentName,
    string PositionName,
    string ContractType,
    string StatusLabel,
    string StatusColor,
    string DriverLabel);
