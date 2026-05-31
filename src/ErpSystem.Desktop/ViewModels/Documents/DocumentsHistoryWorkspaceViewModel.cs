using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.Documents;

public sealed partial class DocumentsHistoryWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly DocumentsWorkspaceStore store = DocumentsWorkspaceStore.Instance;

    public DocumentsHistoryWorkspaceViewModel()
        : base("/documents/history", "Dokumenty", "Historia dokumentu", "Wspólny timeline zmian statusu i akcji użytkowników.", false)
    {
        SummaryCards = new ObservableCollection<SummaryCardViewModel>();
        VisibleEntries = new ObservableCollection<DocumentHistoryRowViewModel>();
        RunActionCommand = new RelayCommand<string>(RunAction);
        SelectEntryCommand = new RelayCommand<DocumentHistoryRowViewModel>(SelectEntry);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<SummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<DocumentHistoryRowViewModel> VisibleEntries { get; }
    public IRelayCommand<string> RunActionCommand { get; }
    public IRelayCommand<DocumentHistoryRowViewModel> SelectEntryCommand { get; }

    public IReadOnlyList<string> ModuleFilterOptions => ["Wszystkie moduły", .. store.GetModules()];
    public IReadOnlyList<string> StatusFilterOptions => ["Wszystkie statusy", .. store.GetHistoryEntries().Select(item => item.Status).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(item => item)];

    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private string selectedModuleFilter = "Wszystkie moduły";
    [ObservableProperty] private string selectedStatusFilter = "Wszystkie statusy";
    [ObservableProperty] private DocumentHistoryRowViewModel? selectedEntry;
    [ObservableProperty] private int? selectedEntryId;
    [ObservableProperty] private string editorModule = "Magazyn";
    [ObservableProperty] private string editorDocumentType = "WarehouseDocument";
    [ObservableProperty] private string editorDocumentNumber = string.Empty;
    [ObservableProperty] private string editorEntryType = "Status";
    [ObservableProperty] private string editorStatus = "Nowy";
    [ObservableProperty] private string editorActor = "Operator";
    [ObservableProperty] private string editorDescription = string.Empty;
    [ObservableProperty] private string lastActionMessage = "Śledź historię zmian i dopisuj ręczne wpisy do obiegu dokumentów.";

    public bool HasSelectedEntry => SelectedEntryId is not null;
    public DocumentHistoryRecord? SelectedRecord => SelectedEntryId is null ? null : store.GetHistoryEntries().FirstOrDefault(item => item.Id == SelectedEntryId);
    public string SelectedOccurredAt => SelectedRecord?.OccurredAt.ToString("dd.MM.yyyy HH:mm") ?? "-";
    public string SelectedStatusColor => ResolveStatusColor(SelectedRecord?.Status);

    partial void OnSearchTextChanged(string value) => RefreshRows();
    partial void OnSelectedModuleFilterChanged(string value) => RefreshRows();
    partial void OnSelectedStatusFilterChanged(string value) => RefreshRows();

    partial void OnSelectedEntryChanged(DocumentHistoryRowViewModel? value)
    {
        SelectedEntryId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedEntryIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasSelectedEntry));
        OnPropertyChanged(nameof(SelectedRecord));
        OnPropertyChanged(nameof(SelectedOccurredAt));
        OnPropertyChanged(nameof(SelectedStatusColor));
    }

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshRows();
        LoadEditor();
        OnPropertyChanged(nameof(ModuleFilterOptions));
        OnPropertyChanged(nameof(StatusFilterOptions));
    }

    private void RefreshSummaryCards()
    {
        var rows = store.GetHistoryEntries();
        SummaryCards.Clear();
        SummaryCards.Add(new SummaryCardViewModel("Wpisy historii", rows.Count.ToString(), "Zdarzenia z wielu modułów ERP.", "#2563EB"));
        SummaryCards.Add(new SummaryCardViewModel("Statusy", rows.Select(item => item.Status).Distinct().Count().ToString(), "Różne stany biznesowe w timeline.", "#1F8A5B"));
        SummaryCards.Add(new SummaryCardViewModel("Ostatnia doba", rows.Count(item => item.OccurredAt >= DateTime.Now.AddDays(-1)).ToString(), "Wpisy dodane w ostatnich 24h.", "#D97706"));
        SummaryCards.Add(new SummaryCardViewModel("Moduły", rows.Select(item => item.Module).Distinct().Count().ToString(), "Obszary korzystające z historii dokumentu.", "#7C3AED"));
    }

    private void RefreshRows()
    {
        var rows = store.GetHistoryEntries()
            .Where(item =>
                (SelectedModuleFilter == "Wszystkie moduły" || string.Equals(item.Module, SelectedModuleFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedStatusFilter == "Wszystkie statusy" || string.Equals(item.Status, SelectedStatusFilter, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.DocumentNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Actor.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            .Select(item => new DocumentHistoryRowViewModel(
                item.Id,
                item.Module,
                item.DocumentType,
                item.DocumentNumber,
                item.EntryType,
                item.Status,
                ResolveStatusColor(item.Status),
                item.Actor,
                item.OccurredAt.ToString("dd.MM HH:mm")))
            .ToArray();

        VisibleEntries.Clear();
        foreach (var row in rows)
        {
            VisibleEntries.Add(row);
        }

        SelectedEntry = SelectedEntryId is null
            ? VisibleEntries.FirstOrDefault()
            : VisibleEntries.FirstOrDefault(item => item.Id == SelectedEntryId) ?? VisibleEntries.FirstOrDefault();
    }

    private void LoadEditor()
    {
        var record = SelectedRecord;
        if (record is null)
        {
            EditorModule = "Magazyn";
            EditorDocumentType = "WarehouseDocument";
            EditorDocumentNumber = string.Empty;
            EditorEntryType = "Status";
            EditorStatus = "Nowy";
            EditorActor = "Operator";
            EditorDescription = string.Empty;
            return;
        }

        EditorModule = record.Module;
        EditorDocumentType = record.DocumentType;
        EditorDocumentNumber = record.DocumentNumber;
        EditorEntryType = record.EntryType;
        EditorStatus = record.Status;
        EditorActor = record.Actor;
        EditorDescription = record.Description;
    }

    private void SelectEntry(DocumentHistoryRowViewModel? row)
    {
        if (row is not null)
        {
            SelectedEntry = row;
        }
    }

    private void RunAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedEntry = null;
                    SelectedEntryId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nowy wpis historii.";
                    break;
                case "Save":
                    Save();
                    break;
                case "Delete":
                    Delete();
                    break;
                case "Export":
                    LastActionMessage = $"Wyeksportowano {VisibleEntries.Count} wpisów historii.";
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }

    private void Save()
    {
        if (SelectedEntryId is null)
        {
            var created = store.CreateHistoryEntry(EditorModule, EditorDocumentType, EditorDocumentNumber, EditorEntryType, EditorStatus, EditorActor, EditorDescription);
            SelectedEntryId = created.Id;
            LastActionMessage = $"Dodano wpis dla dokumentu {created.DocumentNumber}.";
            return;
        }

        var updated = store.UpdateHistoryEntry(SelectedEntryId.Value, EditorModule, EditorDocumentType, EditorDocumentNumber, EditorEntryType, EditorStatus, EditorActor, EditorDescription);
        LastActionMessage = $"Zapisano wpis historii {updated.DocumentNumber}.";
    }

    private void Delete()
    {
        if (SelectedEntryId is null)
        {
            throw new InvalidOperationException("Wybierz wpis historii do usunięcia.");
        }

        store.DeleteHistoryEntry(SelectedEntryId.Value);
        SelectedEntryId = null;
        LastActionMessage = "Usunięto wpis historii.";
    }

    private static string ResolveStatusColor(string? status)
        => status?.ToLowerInvariant() switch
        {
            var value when value is not null && value.Contains("błąd") => "#D14343",
            var value when value is not null && value.Contains("w realiz") => "#2563EB",
            var value when value is not null && value.Contains("zaak") => "#1F8A5B",
            _ => "#D97706"
        };
}

public sealed record DocumentHistoryRowViewModel(
    int Id,
    string Module,
    string DocumentType,
    string DocumentNumber,
    string EntryType,
    string Status,
    string StatusColor,
    string Actor,
    string OccurredAtLabel);
