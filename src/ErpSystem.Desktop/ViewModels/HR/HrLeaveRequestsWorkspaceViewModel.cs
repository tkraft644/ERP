using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.HR;

public sealed partial class HrLeaveRequestsWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly HrWorkspaceStore store = HrWorkspaceStore.Instance;

    public HrLeaveRequestsWorkspaceViewModel()
        : base(
            "/hr/leave-requests",
            "HR",
            "Wnioski urlopowe",
            "Wnioski pracowników o urlopy i nieobecności wraz z terminami oraz uzasadnieniem.",
            false)
    {
        SummaryCards = new ObservableCollection<HrSummaryCardViewModel>();
        VisibleRequests = new ObservableCollection<HrLeaveRequestRowViewModel>();
        RunLeaveRequestActionCommand = new RelayCommand<string>(RunLeaveRequestAction);
        SelectLeaveRequestCommand = new RelayCommand<HrLeaveRequestRowViewModel>(SelectLeaveRequest);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<HrSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<HrLeaveRequestRowViewModel> VisibleRequests { get; }
    public IRelayCommand<string> RunLeaveRequestActionCommand { get; }
    public IRelayCommand<HrLeaveRequestRowViewModel> SelectLeaveRequestCommand { get; }

    public IReadOnlyList<string> StatusFilterOptions => ["Wszystkie statusy", "Submitted", "Approved", "Rejected", "Cancelled"];
    public IReadOnlyList<HrEmployeeRecord> EmployeeOptions => store.GetEmployees().Where(item => item.IsActive).ToArray();
    public IReadOnlyList<HrLeaveTypeRecord> LeaveTypeOptions => store.GetLeaveTypes();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedStatusFilter = "Wszystkie statusy";

    [ObservableProperty]
    private HrLeaveRequestRowViewModel? selectedRequest;

    [ObservableProperty]
    private int? selectedRequestId;

    [ObservableProperty]
    private HrEmployeeRecord? selectedEmployee;

    [ObservableProperty]
    private HrLeaveTypeRecord? selectedLeaveType;

    [ObservableProperty]
    private string editorDateFrom = string.Empty;

    [ObservableProperty]
    private string editorDateTo = string.Empty;

    [ObservableProperty]
    private string editorDayCount = "1";

    [ObservableProperty]
    private string editorReason = string.Empty;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz wniosek lub dodaj nowe zgłoszenie urlopowe.";

    public bool HasSelectedRequest => SelectedRequestId is not null;
    public string SelectedDecisionSummary => SelectedRequest?.DecisionSummary ?? "Brak decyzji.";

    partial void OnSearchTextChanged(string value) => RefreshRequests();
    partial void OnSelectedStatusFilterChanged(string value) => RefreshRequests();

    partial void OnSelectedRequestChanged(HrLeaveRequestRowViewModel? value)
    {
        SelectedRequestId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedRequestIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasSelectedRequest));
        OnPropertyChanged(nameof(SelectedDecisionSummary));
    }

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshRequests();
        LoadEditor();
        OnPropertyChanged(nameof(EmployeeOptions));
        OnPropertyChanged(nameof(LeaveTypeOptions));
    }

    private void RefreshSummaryCards()
    {
        var requests = store.GetLeaveRequests();
        SummaryCards.Clear();
        SummaryCards.Add(new HrSummaryCardViewModel("Nowe wnioski", requests.Count(item => item.Status == "Submitted").ToString(), "Pozycje czekające na sprawdzenie i akceptację.", "#1D4ED8"));
        SummaryCards.Add(new HrSummaryCardViewModel("Zatwierdzone", requests.Count(item => item.Status == "Approved").ToString(), "Nieobecności uwzględnione w planowaniu pracy.", "#1F8A5B"));
        SummaryCards.Add(new HrSummaryCardViewModel("Odrzucone", requests.Count(item => item.Status == "Rejected").ToString(), "Wnioski zakończone negatywną decyzją.", "#D14343"));
        SummaryCards.Add(new HrSummaryCardViewModel("Najbliższe urlopy", requests.Count(item => item.DateFrom >= DateTime.Today && item.DateFrom <= DateTime.Today.AddDays(14)).ToString(), "Nieobecności startujące w ciągu 14 dni.", "#D97706"));
    }

    private void RefreshRequests()
    {
        var rows = store.GetLeaveRequests()
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 store.GetEmployeeName(item.EmployeeId).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 store.GetLeaveTypeName(item.LeaveTypeId).Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedStatusFilter == "Wszystkie statusy" ||
                 string.Equals(item.Status, SelectedStatusFilter, StringComparison.OrdinalIgnoreCase)))
            .Select(item => new HrLeaveRequestRowViewModel(
                item.Id,
                store.GetEmployeeName(item.EmployeeId),
                store.GetLeaveTypeName(item.LeaveTypeId),
                $"{item.DateFrom:dd.MM.yyyy} - {item.DateTo:dd.MM.yyyy}",
                item.DayCount.ToString("0.##"),
                item.StatusLabel,
                item.StatusColor,
                item.Reason ?? "-",
                item.DecisionNote ?? "Brak decyzji."))
            .ToArray();

        VisibleRequests.Clear();
        foreach (var row in rows)
        {
            VisibleRequests.Add(row);
        }

        SelectedRequest = SelectedRequestId is not null
            ? VisibleRequests.FirstOrDefault(item => item.Id == SelectedRequestId) ?? VisibleRequests.FirstOrDefault()
            : VisibleRequests.FirstOrDefault();
    }

    private void LoadEditor()
    {
        if (SelectedRequestId is null)
        {
            SelectedEmployee = EmployeeOptions.FirstOrDefault();
            SelectedLeaveType = LeaveTypeOptions.FirstOrDefault();
            EditorDateFrom = DateTime.Today.ToString("dd.MM.yyyy");
            EditorDateTo = DateTime.Today.ToString("dd.MM.yyyy");
            EditorDayCount = "1";
            EditorReason = string.Empty;
            return;
        }

        var request = store.GetLeaveRequests().FirstOrDefault(item => item.Id == SelectedRequestId);
        if (request is null)
        {
            return;
        }

        SelectedEmployee = EmployeeOptions.FirstOrDefault(item => item.Id == request.EmployeeId) ?? EmployeeOptions.FirstOrDefault();
        SelectedLeaveType = LeaveTypeOptions.FirstOrDefault(item => item.Id == request.LeaveTypeId) ?? LeaveTypeOptions.FirstOrDefault();
        EditorDateFrom = request.DateFrom.ToString("dd.MM.yyyy");
        EditorDateTo = request.DateTo.ToString("dd.MM.yyyy");
        EditorDayCount = request.DayCount.ToString("0.##");
        EditorReason = request.Reason ?? string.Empty;
    }

    private void RunLeaveRequestAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedRequest = null;
                    SelectedRequestId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nowy wniosek urlopowy.";
                    break;
                case "Save":
                    SaveLeaveRequest();
                    break;
                case "Cancel":
                    if (SelectedRequestId is not null)
                    {
                        store.CancelLeaveRequest(SelectedRequestId.Value);
                        LastActionMessage = "Anulowano wniosek urlopowy.";
                    }
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }

    private void SaveLeaveRequest()
    {
        var employee = SelectedEmployee ?? throw new InvalidOperationException("Wybierz pracownika.");
        var leaveType = SelectedLeaveType ?? throw new InvalidOperationException("Wybierz typ urlopu.");
        var dateFrom = HrEditorParsers.ParseRequiredDate(EditorDateFrom, "Data od");
        var dateTo = HrEditorParsers.ParseRequiredDate(EditorDateTo, "Data do");
        var days = HrEditorParsers.ParseRequiredDecimal(EditorDayCount, "Liczba dni");

        if (SelectedRequestId is null)
        {
            var created = store.CreateLeaveRequest(employee.Id, leaveType.Id, dateFrom, dateTo, days, EditorReason);
            SelectedRequestId = created.Id;
            LastActionMessage = $"Dodano wniosek urlopowy dla {store.GetEmployeeName(created.EmployeeId)}.";
            return;
        }

        var updated = store.UpdateLeaveRequest(SelectedRequestId.Value, employee.Id, leaveType.Id, dateFrom, dateTo, days, EditorReason);
        LastActionMessage = $"Zapisano wniosek urlopowy dla {store.GetEmployeeName(updated.EmployeeId)}.";
    }

    private void SelectLeaveRequest(HrLeaveRequestRowViewModel? request)
    {
        SelectedRequest = request;
    }
}

public sealed record HrLeaveRequestRowViewModel(
    int Id,
    string EmployeeName,
    string LeaveTypeName,
    string PeriodLabel,
    string DayCountLabel,
    string StatusLabel,
    string StatusColor,
    string Reason,
    string DecisionSummary);
