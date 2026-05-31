using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.HR;

public sealed partial class HrDepartmentsWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly HrWorkspaceStore store = HrWorkspaceStore.Instance;

    public HrDepartmentsWorkspaceViewModel()
        : base(
            "/hr/departments",
            "HR",
            "Działy",
            "Struktura organizacyjna firmy używana przez kadry, finanse i transport.",
            false)
    {
        SummaryCards = new ObservableCollection<HrSummaryCardViewModel>();
        VisibleDepartments = new ObservableCollection<HrDepartmentRowViewModel>();
        RunDepartmentActionCommand = new RelayCommand<string>(RunDepartmentAction);
        SelectDepartmentCommand = new RelayCommand<HrDepartmentRowViewModel>(SelectDepartment);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<HrSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<HrDepartmentRowViewModel> VisibleDepartments { get; }
    public IRelayCommand<string> RunDepartmentActionCommand { get; }
    public IRelayCommand<HrDepartmentRowViewModel> SelectDepartmentCommand { get; }

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool showInactive = true;

    [ObservableProperty]
    private HrDepartmentRowViewModel? selectedDepartment;

    [ObservableProperty]
    private int? selectedDepartmentId;

    [ObservableProperty]
    private string editorCode = string.Empty;

    [ObservableProperty]
    private string editorName = string.Empty;

    [ObservableProperty]
    private string editorDescription = string.Empty;

    [ObservableProperty]
    private bool editorIsActive = true;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz dział lub dodaj nową jednostkę organizacyjną.";

    public bool HasSelectedDepartment => SelectedDepartmentId is not null;

    partial void OnSearchTextChanged(string value) => RefreshDepartments();
    partial void OnShowInactiveChanged(bool value) => RefreshDepartments();

    partial void OnSelectedDepartmentChanged(HrDepartmentRowViewModel? value)
    {
        SelectedDepartmentId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedDepartmentIdChanged(int? value) => OnPropertyChanged(nameof(HasSelectedDepartment));

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshDepartments();
        LoadEditor();
    }

    private void RefreshSummaryCards()
    {
        var departments = store.GetDepartments();
        var employees = store.GetEmployees();
        var contracts = store.GetContracts();

        SummaryCards.Clear();
        SummaryCards.Add(new HrSummaryCardViewModel("Działy aktywne", departments.Count(item => item.IsActive).ToString(), "Jednostki gotowe do użycia w kadrach i finansach.", "#1D4ED8"));
        SummaryCards.Add(new HrSummaryCardViewModel("Pracownicy przypisani", employees.Count(item => item.DepartmentId is not null).ToString(), "Osoby osadzone w strukturze organizacyjnej.", "#1F8A5B"));
        SummaryCards.Add(new HrSummaryCardViewModel("Umowy aktywne", contracts.Count(item => item.IsActive && !item.IsArchived).ToString(), "Kontrakty korzystające z działów jako koszt center.", "#D97706"));
        SummaryCards.Add(new HrSummaryCardViewModel("Nieaktywne", departments.Count(item => !item.IsActive).ToString(), "Pozycje wycofane lub zamrożone.", "#D14343"));
    }

    private void RefreshDepartments()
    {
        var rows = store.GetDepartments()
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (ShowInactive || item.IsActive))
            .Select(item => new HrDepartmentRowViewModel(
                item.Id,
                item.Code,
                item.Name,
                item.Description,
                item.StatusLabel,
                item.StatusColor,
                store.GetEmployees().Count(employee => employee.DepartmentId == item.Id)))
            .ToArray();

        VisibleDepartments.Clear();
        foreach (var row in rows)
        {
            VisibleDepartments.Add(row);
        }

        SelectedDepartment = SelectedDepartmentId is not null
            ? VisibleDepartments.FirstOrDefault(item => item.Id == SelectedDepartmentId) ?? VisibleDepartments.FirstOrDefault()
            : VisibleDepartments.FirstOrDefault();
    }

    private void LoadEditor()
    {
        if (SelectedDepartmentId is null)
        {
            EditorCode = string.Empty;
            EditorName = string.Empty;
            EditorDescription = string.Empty;
            EditorIsActive = true;
            return;
        }

        var department = store.GetDepartments().FirstOrDefault(item => item.Id == SelectedDepartmentId);
        if (department is null)
        {
            return;
        }

        EditorCode = department.Code;
        EditorName = department.Name;
        EditorDescription = department.Description;
        EditorIsActive = department.IsActive;
    }

    private void RunDepartmentAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedDepartment = null;
                    SelectedDepartmentId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nowy dział.";
                    break;
                case "Save":
                    SaveDepartment();
                    break;
                case "Toggle":
                    if (SelectedDepartmentId is not null)
                    {
                        store.ToggleDepartment(SelectedDepartmentId.Value);
                        LastActionMessage = "Zmieniono aktywność działu.";
                    }
                    break;
                case "Delete":
                    if (SelectedDepartmentId is not null)
                    {
                        store.DeleteDepartment(SelectedDepartmentId.Value);
                        SelectedDepartmentId = null;
                        SelectedDepartment = null;
                        LoadEditor();
                        LastActionMessage = "Usunięto dział.";
                    }
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }

    private void SaveDepartment()
    {
        if (SelectedDepartmentId is null)
        {
            var created = store.CreateDepartment(EditorCode, EditorName, EditorDescription, EditorIsActive);
            SelectedDepartmentId = created.Id;
            LastActionMessage = $"Dodano dział {created.Name}.";
            return;
        }

        var updated = store.UpdateDepartment(SelectedDepartmentId.Value, EditorCode, EditorName, EditorDescription, EditorIsActive);
        LastActionMessage = $"Zapisano dział {updated.Name}.";
    }

    private void SelectDepartment(HrDepartmentRowViewModel? department)
    {
        SelectedDepartment = department;
    }
}

public sealed record HrDepartmentRowViewModel(
    int Id,
    string Code,
    string Name,
    string Description,
    string StatusLabel,
    string StatusColor,
    int EmployeeCount);
