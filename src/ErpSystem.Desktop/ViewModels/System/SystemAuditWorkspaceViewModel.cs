using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.System;

public sealed partial class SystemAuditWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly SystemWorkspaceStore store = SystemWorkspaceStore.Instance;

    public SystemAuditWorkspaceViewModel()
        : base(
            "/system/audit",
            "System",
            "Audit log",
            "Historia zmian administracyjnych i operacyjnych wykonywanych w systemie.",
            false)
    {
        SummaryCards = new ObservableCollection<SystemSummaryCardViewModel>();
        VisibleEntries = new ObservableCollection<SystemAuditRecord>();
        RunAuditActionCommand = new RelayCommand<string>(RunAuditAction);
        SelectAuditCommand = new RelayCommand<SystemAuditRecord>(entry => SelectedEntry = entry);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<SystemSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<SystemAuditRecord> VisibleEntries { get; }
    public IRelayCommand<string> RunAuditActionCommand { get; }
    public IRelayCommand<SystemAuditRecord> SelectAuditCommand { get; }

    public IReadOnlyList<string> ModuleFilterOptions => ["Wszystkie moduły", .. store.GetAuditLogs().Select(item => item.Module).Distinct().OrderBy(item => item)];
    public IReadOnlyList<string> ActionFilterOptions => ["Wszystkie akcje", .. store.GetAuditLogs().Select(item => item.Action).Distinct().OrderBy(item => item)];

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedModuleFilter = "Wszystkie moduły";

    [ObservableProperty]
    private string selectedActionFilter = "Wszystkie akcje";

    [ObservableProperty]
    private SystemAuditRecord? selectedEntry;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz wpis audytowy, aby zobaczyć szczegóły operacji.";

    public string SelectedAuditTime => SelectedEntry?.At.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss") ?? "-";
    public string SelectedAuditUser => SelectedEntry?.User ?? "-";
    public string SelectedAuditModule => SelectedEntry?.Module ?? "-";
    public string SelectedAuditEntity => SelectedEntry?.Entity ?? "-";
    public string SelectedAuditAction => SelectedEntry?.Action ?? "-";
    public string SelectedAuditDetail => SelectedEntry?.Detail ?? "Brak szczegółów.";

    partial void OnSearchTextChanged(string value) => RefreshEntries();
    partial void OnSelectedModuleFilterChanged(string value) => RefreshEntries();
    partial void OnSelectedActionFilterChanged(string value) => RefreshEntries();

    partial void OnSelectedEntryChanged(SystemAuditRecord? value)
    {
        OnPropertyChanged(nameof(SelectedAuditTime));
        OnPropertyChanged(nameof(SelectedAuditUser));
        OnPropertyChanged(nameof(SelectedAuditModule));
        OnPropertyChanged(nameof(SelectedAuditEntity));
        OnPropertyChanged(nameof(SelectedAuditAction));
        OnPropertyChanged(nameof(SelectedAuditDetail));
    }

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshEntries();
        OnPropertyChanged(nameof(ModuleFilterOptions));
        OnPropertyChanged(nameof(ActionFilterOptions));
    }

    private void RefreshSummaryCards()
    {
        var audit = store.GetAuditLogs();
        SummaryCards.Clear();
        SummaryCards.Add(new SystemSummaryCardViewModel("Wpisy", audit.Count.ToString(), "Łączna liczba wpisów widocznych w logu.", "#2563EB"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Moduły", audit.Select(item => item.Module).Distinct().Count().ToString(), "Obszary systemu raportujące zmiany.", "#1F8A5B"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Ostatnia godzina", audit.Count(item => item.At >= DateTime.UtcNow.AddHours(-1)).ToString(), "Świeże zmiany i operacje administracyjne.", "#D97706"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Administrator", audit.Count(item => item.User.Contains("Admin", StringComparison.OrdinalIgnoreCase)).ToString(), "Wpisy wykonane przez administratorów.", "#D14343"));
    }

    private void RefreshEntries()
    {
        var entries = store.GetAuditLogs()
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.User.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Entity.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Detail.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedModuleFilter == "Wszystkie moduły" ||
                 string.Equals(item.Module, SelectedModuleFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedActionFilter == "Wszystkie akcje" ||
                 string.Equals(item.Action, SelectedActionFilter, StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        VisibleEntries.Clear();
        foreach (var entry in entries)
        {
            VisibleEntries.Add(entry);
        }

        SelectedEntry = VisibleEntries.FirstOrDefault(item => item.Id == SelectedEntry?.Id) ?? VisibleEntries.FirstOrDefault();
    }

    private void RunAuditAction(string? action)
    {
        switch (action)
        {
            case "Clear":
                SearchText = string.Empty;
                SelectedModuleFilter = "Wszystkie moduły";
                SelectedActionFilter = "Wszystkie akcje";
                LastActionMessage = "Wyczyszczono filtry audytu.";
                break;
            case "Export":
                LastActionMessage = "Wyeksportowano audyt do pliku CSV i PDF.";
                break;
            case "Refresh":
                Refresh();
                LastActionMessage = "Odświeżono log audytu.";
                break;
        }
    }
}
