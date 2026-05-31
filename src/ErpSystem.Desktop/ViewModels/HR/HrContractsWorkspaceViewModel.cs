using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.HR;

public sealed partial class HrContractsWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly HrWorkspaceStore store = HrWorkspaceStore.Instance;
    private static readonly IReadOnlyList<string> ContractTypeOptionsSource = ["Employment", "B2B", "Mandate", "Internship"];

    public HrContractsWorkspaceViewModel()
        : base(
            "/hr/contracts",
            "HR",
            "Umowy",
            "Kontrakty pracowników, wymiar etatu, koszty i historia zatrudnienia.",
            false)
    {
        SummaryCards = new ObservableCollection<HrSummaryCardViewModel>();
        VisibleContracts = new ObservableCollection<HrContractRowViewModel>();
        RunContractActionCommand = new RelayCommand<string>(RunContractAction);
        SelectContractCommand = new RelayCommand<HrContractRowViewModel>(SelectContract);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<HrSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<HrContractRowViewModel> VisibleContracts { get; }
    public IRelayCommand<string> RunContractActionCommand { get; }
    public IRelayCommand<HrContractRowViewModel> SelectContractCommand { get; }

    public IReadOnlyList<string> DepartmentFilterOptions => ["Wszystkie działy", .. store.GetDepartments().Select(item => item.Name)];
    public IReadOnlyList<string> ContractTypeFilterOptions => ["Wszystkie typy", .. ContractTypeOptionsSource];
    public IReadOnlyList<HrEmployeeRecord> EmployeeOptions => store.GetEmployees().Where(item => item.IsActive).ToArray();
    public IReadOnlyList<HrDepartmentRecord> DepartmentOptions => store.GetDepartments().Where(item => item.IsActive).ToArray();
    public IReadOnlyList<HrPositionRecord> PositionOptions => store.GetPositions().Where(item => item.IsActive).ToArray();
    public IReadOnlyList<string> ContractTypeOptions => ContractTypeOptionsSource;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedDepartmentFilter = "Wszystkie działy";

    [ObservableProperty]
    private string selectedContractTypeFilter = "Wszystkie typy";

    [ObservableProperty]
    private bool showArchived = true;

    [ObservableProperty]
    private HrContractRowViewModel? selectedContract;

    [ObservableProperty]
    private int? selectedContractId;

    [ObservableProperty]
    private HrEmployeeRecord? selectedEmployee;

    [ObservableProperty]
    private HrDepartmentRecord? selectedDepartment;

    [ObservableProperty]
    private HrPositionRecord? selectedPosition;

    [ObservableProperty]
    private string selectedContractType = "Employment";

    [ObservableProperty]
    private string editorContractNumber = string.Empty;

    [ObservableProperty]
    private string editorStartDate = string.Empty;

    [ObservableProperty]
    private string editorEndDate = string.Empty;

    [ObservableProperty]
    private string editorEmploymentRate = "1";

    [ObservableProperty]
    private string editorMonthlySalary = string.Empty;

    [ObservableProperty]
    private string editorNotes = string.Empty;

    [ObservableProperty]
    private bool editorIsActive = true;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz umowę lub dodaj nową pozycję zatrudnienia.";

    public bool HasSelectedContract => SelectedContractId is not null;

    partial void OnSearchTextChanged(string value) => RefreshContracts();
    partial void OnSelectedDepartmentFilterChanged(string value) => RefreshContracts();
    partial void OnSelectedContractTypeFilterChanged(string value) => RefreshContracts();
    partial void OnShowArchivedChanged(bool value) => RefreshContracts();

    partial void OnSelectedContractChanged(HrContractRowViewModel? value)
    {
        SelectedContractId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedContractIdChanged(int? value) => OnPropertyChanged(nameof(HasSelectedContract));

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshContracts();
        LoadEditor();
        OnPropertyChanged(nameof(DepartmentFilterOptions));
        OnPropertyChanged(nameof(EmployeeOptions));
        OnPropertyChanged(nameof(DepartmentOptions));
        OnPropertyChanged(nameof(PositionOptions));
    }

    private void RefreshSummaryCards()
    {
        var contracts = store.GetContracts();
        SummaryCards.Clear();
        SummaryCards.Add(new HrSummaryCardViewModel("Umowy aktywne", contracts.Count(item => item.IsActive && !item.IsArchived).ToString(), "Kontrakty aktualnie obowiązujące.", "#1D4ED8"));
        SummaryCards.Add(new HrSummaryCardViewModel("B2B", contracts.Count(item => item.ContractType == "B2B" && !item.IsArchived).ToString(), "Kontrakty usługowe aktywne w organizacji.", "#1F8A5B"));
        SummaryCards.Add(new HrSummaryCardViewModel("Kończące się", contracts.Count(item => item.EndDate is not null && item.EndDate.Value <= DateTime.Today.AddDays(45) && !item.IsArchived).ToString(), "Umowy wymagające aneksu lub odnowienia.", "#D97706"));
        SummaryCards.Add(new HrSummaryCardViewModel("Archiwum", contracts.Count(item => item.IsArchived).ToString(), "Historia zatrudnienia i wygasłe kontrakty.", "#D14343"));
    }

    private void RefreshContracts()
    {
        var rows = store.GetContracts()
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.ContractNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 store.GetEmployeeName(item.EmployeeId).Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedDepartmentFilter == "Wszystkie działy" ||
                 string.Equals(store.GetDepartmentName(item.DepartmentId), SelectedDepartmentFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedContractTypeFilter == "Wszystkie typy" ||
                 string.Equals(item.ContractType, SelectedContractTypeFilter, StringComparison.OrdinalIgnoreCase)) &&
                (ShowArchived || !item.IsArchived))
            .Select(item => new HrContractRowViewModel(
                item.Id,
                item.ContractNumber,
                store.GetEmployeeName(item.EmployeeId),
                store.GetDepartmentName(item.DepartmentId),
                store.GetPositionName(item.PositionId),
                item.ContractType,
                $"{item.StartDate:dd.MM.yyyy} - {(item.EndDate is null ? "bezterminowa" : item.EndDate.Value.ToString("dd.MM.yyyy"))}",
                item.EmploymentRateLabel,
                item.SalaryLabel,
                item.StatusLabel,
                item.StatusColor))
            .ToArray();

        VisibleContracts.Clear();
        foreach (var row in rows)
        {
            VisibleContracts.Add(row);
        }

        SelectedContract = SelectedContractId is not null
            ? VisibleContracts.FirstOrDefault(item => item.Id == SelectedContractId) ?? VisibleContracts.FirstOrDefault()
            : VisibleContracts.FirstOrDefault();
    }

    private void LoadEditor()
    {
        if (SelectedContractId is null)
        {
            SelectedEmployee = EmployeeOptions.FirstOrDefault();
            SelectedDepartment = DepartmentOptions.FirstOrDefault();
            SelectedPosition = PositionOptions.FirstOrDefault();
            SelectedContractType = "Employment";
            EditorContractNumber = string.Empty;
            EditorStartDate = DateTime.Today.ToString("dd.MM.yyyy");
            EditorEndDate = string.Empty;
            EditorEmploymentRate = "1";
            EditorMonthlySalary = string.Empty;
            EditorNotes = string.Empty;
            EditorIsActive = true;
            return;
        }

        var contract = store.GetContracts().FirstOrDefault(item => item.Id == SelectedContractId);
        if (contract is null)
        {
            return;
        }

        SelectedEmployee = EmployeeOptions.FirstOrDefault(item => item.Id == contract.EmployeeId) ?? EmployeeOptions.FirstOrDefault();
        SelectedDepartment = DepartmentOptions.FirstOrDefault(item => item.Id == contract.DepartmentId) ?? DepartmentOptions.FirstOrDefault();
        SelectedPosition = PositionOptions.FirstOrDefault(item => item.Id == contract.PositionId) ?? PositionOptions.FirstOrDefault();
        SelectedContractType = contract.ContractType;
        EditorContractNumber = contract.ContractNumber;
        EditorStartDate = contract.StartDate.ToString("dd.MM.yyyy");
        EditorEndDate = contract.EndDate?.ToString("dd.MM.yyyy") ?? string.Empty;
        EditorEmploymentRate = contract.EmploymentRate.ToString("0.##");
        EditorMonthlySalary = contract.MonthlySalary?.ToString("0.##") ?? string.Empty;
        EditorNotes = contract.Notes ?? string.Empty;
        EditorIsActive = contract.IsActive;
    }

    private void RunContractAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedContract = null;
                    SelectedContractId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nową umowę.";
                    break;
                case "Save":
                    SaveContract();
                    break;
                case "Archive":
                    if (SelectedContractId is not null)
                    {
                        store.ArchiveContract(SelectedContractId.Value);
                        LastActionMessage = "Zarchiwizowano umowę.";
                    }
                    break;
                case "Delete":
                    if (SelectedContractId is not null)
                    {
                        store.DeleteContract(SelectedContractId.Value);
                        SelectedContractId = null;
                        SelectedContract = null;
                        LoadEditor();
                        LastActionMessage = "Usunięto umowę.";
                    }
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }

    private void SaveContract()
    {
        var employee = SelectedEmployee ?? throw new InvalidOperationException("Wybierz pracownika.");
        var department = SelectedDepartment ?? throw new InvalidOperationException("Wybierz dział.");
        var position = SelectedPosition ?? throw new InvalidOperationException("Wybierz stanowisko.");
        var startDate = HrEditorParsers.ParseRequiredDate(EditorStartDate, "Data rozpoczęcia");
        var endDate = HrEditorParsers.ParseOptionalDate(EditorEndDate);
        var rate = HrEditorParsers.ParseRequiredDecimal(EditorEmploymentRate, "Wymiar etatu");
        var salary = HrEditorParsers.ParseOptionalDecimal(EditorMonthlySalary);

        if (SelectedContractId is null)
        {
            var created = store.CreateContract(employee.Id, EditorContractNumber, SelectedContractType, department.Id, position.Id, startDate, endDate, rate, salary, EditorNotes);
            SelectedContractId = created.Id;
            LastActionMessage = $"Dodano umowę {created.ContractNumber}.";
            return;
        }

        var updated = store.UpdateContract(SelectedContractId.Value, employee.Id, EditorContractNumber, SelectedContractType, department.Id, position.Id, startDate, endDate, rate, salary, EditorNotes, EditorIsActive);
        LastActionMessage = $"Zapisano umowę {updated.ContractNumber}.";
    }

    private void SelectContract(HrContractRowViewModel? contract)
    {
        SelectedContract = contract;
    }
}

public sealed record HrContractRowViewModel(
    int Id,
    string ContractNumber,
    string EmployeeName,
    string DepartmentName,
    string PositionName,
    string ContractType,
    string PeriodLabel,
    string EmploymentRateLabel,
    string SalaryLabel,
    string StatusLabel,
    string StatusColor);
