using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.HR;

public sealed partial class HrLeaveApprovalsWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly HrWorkspaceStore store = HrWorkspaceStore.Instance;

    public HrLeaveApprovalsWorkspaceViewModel()
        : base(
            "/hr/leave-approvals",
            "HR",
            "Akceptacje urlopów",
            "Kolejka decyzji dla przełożonych i kadr z podglądem wpływu na dział.",
            false)
    {
        SummaryCards = new ObservableCollection<HrSummaryCardViewModel>();
        VisibleApprovals = new ObservableCollection<HrLeaveApprovalRowViewModel>();
        RunApprovalActionCommand = new RelayCommand<string>(RunApprovalAction);
        SelectApprovalCommand = new RelayCommand<HrLeaveApprovalRowViewModel>(SelectApproval);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<HrSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<HrLeaveApprovalRowViewModel> VisibleApprovals { get; }
    public IRelayCommand<string> RunApprovalActionCommand { get; }
    public IRelayCommand<HrLeaveApprovalRowViewModel> SelectApprovalCommand { get; }

    public IReadOnlyList<string> QueueFilterOptions => ["Do decyzji", "Wszystkie", "Approved", "Rejected", "Cancelled"];

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedQueueFilter = "Do decyzji";

    [ObservableProperty]
    private HrLeaveApprovalRowViewModel? selectedApproval;

    [ObservableProperty]
    private int? selectedApprovalId;

    [ObservableProperty]
    private string decisionNote = string.Empty;

    [ObservableProperty]
    private string decisionBy = "Kierownik HR";

    [ObservableProperty]
    private string lastActionMessage = "Wybierz wniosek z kolejki i podejmij decyzję.";

    public bool HasSelectedApproval => SelectedApprovalId is not null;

    partial void OnSearchTextChanged(string value) => RefreshApprovals();
    partial void OnSelectedQueueFilterChanged(string value) => RefreshApprovals();

    partial void OnSelectedApprovalChanged(HrLeaveApprovalRowViewModel? value)
    {
        SelectedApprovalId = value?.Id;
        LoadSelection();
    }

    partial void OnSelectedApprovalIdChanged(int? value) => OnPropertyChanged(nameof(HasSelectedApproval));

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshApprovals();
        LoadSelection();
    }

    private void RefreshSummaryCards()
    {
        var requests = store.GetLeaveRequests();
        SummaryCards.Clear();
        SummaryCards.Add(new HrSummaryCardViewModel("Do decyzji", requests.Count(item => item.Status == "Submitted").ToString(), "Wnioski oczekujące na akceptację lub odrzucenie.", "#1D4ED8"));
        SummaryCards.Add(new HrSummaryCardViewModel("Dziś zatwierdzone", requests.Count(item => item.Status == "Approved" && item.DecidedAt?.Date == DateTime.UtcNow.Date).ToString(), "Decyzje zakończone w bieżącym dniu.", "#1F8A5B"));
        SummaryCards.Add(new HrSummaryCardViewModel("Konflikty terminu", CountPotentialConflicts().ToString(), "Nakładające się urlopy w tym samym dziale.", "#D97706"));
        SummaryCards.Add(new HrSummaryCardViewModel("Odrzucone", requests.Count(item => item.Status == "Rejected").ToString(), "Wnioski wymagające dalszej komunikacji z pracownikiem.", "#D14343"));
    }

    private void RefreshApprovals()
    {
        var rows = store.GetLeaveRequests()
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 store.GetEmployeeName(item.EmployeeId).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 store.GetLeaveTypeName(item.LeaveTypeId).Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedQueueFilter == "Wszystkie" ||
                 (SelectedQueueFilter == "Do decyzji" && item.Status == "Submitted") ||
                 string.Equals(item.Status, SelectedQueueFilter, StringComparison.OrdinalIgnoreCase)))
            .Select(item => new HrLeaveApprovalRowViewModel(
                item.Id,
                store.GetEmployeeName(item.EmployeeId),
                store.GetDepartmentName(store.GetEmployees().FirstOrDefault(employee => employee.Id == item.EmployeeId)?.DepartmentId),
                store.GetLeaveTypeName(item.LeaveTypeId),
                $"{item.DateFrom:dd.MM.yyyy} - {item.DateTo:dd.MM.yyyy}",
                item.DayCount.ToString("0.##"),
                item.StatusLabel,
                item.StatusColor,
                item.Reason ?? "-",
                item.DecisionNote ?? "Brak decyzji"))
            .ToArray();

        VisibleApprovals.Clear();
        foreach (var row in rows)
        {
            VisibleApprovals.Add(row);
        }

        SelectedApproval = SelectedApprovalId is not null
            ? VisibleApprovals.FirstOrDefault(item => item.Id == SelectedApprovalId) ?? VisibleApprovals.FirstOrDefault()
            : VisibleApprovals.FirstOrDefault();
    }

    private void LoadSelection()
    {
        var request = SelectedApprovalId is null
            ? null
            : store.GetLeaveRequests().FirstOrDefault(item => item.Id == SelectedApprovalId);
        DecisionNote = request?.DecisionNote ?? string.Empty;
    }

    private void RunApprovalAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "Approve":
                    ApplyDecision("Approved");
                    break;
                case "Reject":
                    ApplyDecision("Rejected");
                    break;
                case "Cancel":
                    ApplyDecision("Cancelled");
                    break;
                case "Clear":
                    DecisionNote = string.Empty;
                    LastActionMessage = "Wyczyszczono notatkę decyzji.";
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }

    private void ApplyDecision(string status)
    {
        if (SelectedApprovalId is null)
        {
            return;
        }

        store.DecideLeaveRequest(SelectedApprovalId.Value, status, DecisionNote, DecisionBy);
        LastActionMessage = status switch
        {
            "Approved" => "Wniosek został zatwierdzony.",
            "Rejected" => "Wniosek został odrzucony.",
            _ => "Wniosek został anulowany."
        };
    }

    private int CountPotentialConflicts()
    {
        var requests = store.GetLeaveRequests().Where(item => item.Status is "Submitted" or "Approved").ToArray();
        var conflictCount = 0;

        for (var i = 0; i < requests.Length; i++)
        {
            var leftEmployee = store.GetEmployees().FirstOrDefault(item => item.Id == requests[i].EmployeeId);
            if (leftEmployee?.DepartmentId is null)
            {
                continue;
            }

            for (var j = i + 1; j < requests.Length; j++)
            {
                var rightEmployee = store.GetEmployees().FirstOrDefault(item => item.Id == requests[j].EmployeeId);
                if (rightEmployee?.DepartmentId != leftEmployee.DepartmentId)
                {
                    continue;
                }

                if (requests[i].DateFrom <= requests[j].DateTo && requests[j].DateFrom <= requests[i].DateTo)
                {
                    conflictCount++;
                }
            }
        }

        return conflictCount;
    }

    private void SelectApproval(HrLeaveApprovalRowViewModel? approval)
    {
        SelectedApproval = approval;
    }
}

public sealed record HrLeaveApprovalRowViewModel(
    int Id,
    string EmployeeName,
    string DepartmentName,
    string LeaveTypeName,
    string PeriodLabel,
    string DayCountLabel,
    string StatusLabel,
    string StatusColor,
    string Reason,
    string DecisionSummary);
