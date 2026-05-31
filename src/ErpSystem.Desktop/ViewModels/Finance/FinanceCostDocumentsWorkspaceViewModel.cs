using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.Finance;

public sealed partial class FinanceCostDocumentsWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly FinanceWorkspaceStore store = FinanceWorkspaceStore.Instance;

    public FinanceCostDocumentsWorkspaceViewModel()
        : base(
            "/finance/cost-documents",
            "Finanse",
            "Lista dokumentów kosztowych",
            "Koszty przypięte do transportu, magazynu, pracowników, kontrahentów i działów.",
            false)
    {
        SummaryCards = new ObservableCollection<SummaryCardViewModel>();
        VisibleDocuments = new ObservableCollection<FinanceCostDocumentRowViewModel>();
        RunActionCommand = new RelayCommand<string>(RunAction);
        SelectDocumentCommand = new RelayCommand<FinanceCostDocumentRowViewModel>(SelectDocument);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<SummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<FinanceCostDocumentRowViewModel> VisibleDocuments { get; }
    public IRelayCommand<string> RunActionCommand { get; }
    public IRelayCommand<FinanceCostDocumentRowViewModel> SelectDocumentCommand { get; }

    public IReadOnlyList<string> StatusFilterOptions => ["Wszystkie statusy", .. store.GetCostDocuments().Select(item => item.Status).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(item => item)];
    public IReadOnlyList<string> CurrencyFilterOptions => ["Wszystkie waluty", .. store.GetCurrencies()];
    public IReadOnlyList<string> ContractorOptions => store.GetContractors();

    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private string selectedStatusFilter = "Wszystkie statusy";
    [ObservableProperty] private string selectedCurrencyFilter = "Wszystkie waluty";
    [ObservableProperty] private bool showArchived;
    [ObservableProperty] private FinanceCostDocumentRowViewModel? selectedDocument;
    [ObservableProperty] private int? selectedDocumentId;
    [ObservableProperty] private string editorNumber = string.Empty;
    [ObservableProperty] private string editorStatus = "Nowy";
    [ObservableProperty] private string editorDocumentDate = DateTime.Today.ToString("yyyy-MM-dd");
    [ObservableProperty] private string editorDueDate = DateTime.Today.AddDays(14).ToString("yyyy-MM-dd");
    [ObservableProperty] private string editorContractorName = string.Empty;
    [ObservableProperty] private string editorCurrencyCode = "PLN";
    [ObservableProperty] private string editorGrossAmount = "0";
    [ObservableProperty] private string editorCostTarget = string.Empty;
    [ObservableProperty] private string editorDescription = string.Empty;
    [ObservableProperty] private string lastActionMessage = "Wybierz dokument kosztowy albo przygotuj nowy koszt do zapisania.";

    public bool HasSelectedDocument => SelectedDocumentId is not null;
    public FinanceCostDocumentRecord? SelectedRecord => SelectedDocumentId is null ? null : store.GetCostDocuments().FirstOrDefault(item => item.Id == SelectedDocumentId);
    public string SelectedAmountLabel => SelectedRecord is null ? "-" : $"{SelectedRecord.GrossAmount:N2} {SelectedRecord.CurrencyCode}";
    public string SelectedDueDateLabel => SelectedRecord?.DueDate.ToString("dd.MM.yyyy") ?? "-";
    public string SelectedArchiveLabel => SelectedRecord?.IsArchived == true ? "Archiwum" : "Obieg aktywny";
    public string SelectedStatusColor => ResolveStatusColor(SelectedRecord?.Status);

    partial void OnSearchTextChanged(string value) => RefreshRows();
    partial void OnSelectedStatusFilterChanged(string value) => RefreshRows();
    partial void OnSelectedCurrencyFilterChanged(string value) => RefreshRows();
    partial void OnShowArchivedChanged(bool value) => RefreshRows();

    partial void OnSelectedDocumentChanged(FinanceCostDocumentRowViewModel? value)
    {
        SelectedDocumentId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedDocumentIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasSelectedDocument));
        OnPropertyChanged(nameof(SelectedRecord));
        OnPropertyChanged(nameof(SelectedAmountLabel));
        OnPropertyChanged(nameof(SelectedDueDateLabel));
        OnPropertyChanged(nameof(SelectedArchiveLabel));
        OnPropertyChanged(nameof(SelectedStatusColor));
    }

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshRows();
        LoadEditor();
        OnPropertyChanged(nameof(StatusFilterOptions));
        OnPropertyChanged(nameof(CurrencyFilterOptions));
        OnPropertyChanged(nameof(ContractorOptions));
    }

    private void RefreshSummaryCards()
    {
        var rows = store.GetCostDocuments();
        SummaryCards.Clear();
        SummaryCards.Add(new SummaryCardViewModel("Otwarte koszty", rows.Count(item => !item.IsArchived).ToString(), "Dokumenty nadal w aktywnym obiegu.", "#2563EB"));
        SummaryCards.Add(new SummaryCardViewModel("Do akceptacji", rows.Count(item => item.Status.Contains("akcept", StringComparison.OrdinalIgnoreCase)).ToString(), "Koszty czekające na decyzję kierownika.", "#D97706"));
        SummaryCards.Add(new SummaryCardViewModel("Wartość brutto", $"{rows.Where(item => !item.IsArchived).Sum(item => item.GrossAmount):N0} zł", "Suma aktywnych dokumentów kosztowych.", "#1F8A5B"));
        SummaryCards.Add(new SummaryCardViewModel("Waluty", rows.Select(item => item.CurrencyCode).Distinct().Count().ToString(), "Dokumenty krajowe i zagraniczne.", "#7C3AED"));
    }

    private void RefreshRows()
    {
        var rows = store.GetCostDocuments()
            .Where(item =>
                (ShowArchived || !item.IsArchived) &&
                (SelectedStatusFilter == "Wszystkie statusy" || string.Equals(item.Status, SelectedStatusFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedCurrencyFilter == "Wszystkie waluty" || string.Equals(item.CurrencyCode, SelectedCurrencyFilter, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.Number.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.ContractorName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.CostTarget.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            .Select(item => new FinanceCostDocumentRowViewModel(
                item.Id,
                item.Number,
                item.Status,
                ResolveStatusColor(item.Status),
                item.ContractorName,
                item.DocumentDate.ToString("dd.MM.yyyy"),
                item.DueDate.ToString("dd.MM.yyyy"),
                item.CurrencyCode,
                $"{item.GrossAmount:N2}",
                item.CostTarget,
                item.IsArchived ? "Tak" : "Nie"))
            .ToArray();

        VisibleDocuments.Clear();
        foreach (var row in rows)
        {
            VisibleDocuments.Add(row);
        }

        SelectedDocument = SelectedDocumentId is null
            ? VisibleDocuments.FirstOrDefault()
            : VisibleDocuments.FirstOrDefault(item => item.Id == SelectedDocumentId) ?? VisibleDocuments.FirstOrDefault();
    }

    private void LoadEditor()
    {
        var record = SelectedRecord;
        if (record is null)
        {
            EditorNumber = string.Empty;
            EditorStatus = "Nowy";
            EditorDocumentDate = DateTime.Today.ToString("yyyy-MM-dd");
            EditorDueDate = DateTime.Today.AddDays(14).ToString("yyyy-MM-dd");
            EditorContractorName = ContractorOptions.FirstOrDefault() ?? string.Empty;
            EditorCurrencyCode = "PLN";
            EditorGrossAmount = "0";
            EditorCostTarget = string.Empty;
            EditorDescription = string.Empty;
            return;
        }

        EditorNumber = record.Number;
        EditorStatus = record.Status;
        EditorDocumentDate = record.DocumentDate.ToString("yyyy-MM-dd");
        EditorDueDate = record.DueDate.ToString("yyyy-MM-dd");
        EditorContractorName = record.ContractorName;
        EditorCurrencyCode = record.CurrencyCode;
        EditorGrossAmount = record.GrossAmount.ToString("N2");
        EditorCostTarget = record.CostTarget;
        EditorDescription = record.Description ?? string.Empty;
    }

    private void SelectDocument(FinanceCostDocumentRowViewModel? row)
    {
        if (row is not null)
        {
            SelectedDocument = row;
        }
    }

    private void RunAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedDocument = null;
                    SelectedDocumentId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nowy dokument kosztowy.";
                    break;
                case "Save":
                    Save();
                    break;
                case "Delete":
                    Delete();
                    break;
                case "Archive":
                    ToggleArchive();
                    break;
                case "Export":
                    LastActionMessage = $"Wyeksportowano {VisibleDocuments.Count} dokumentów kosztowych.";
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
        var documentDate = EditorValueParsers.ParseDate(EditorDocumentDate, "Data dokumentu");
        var dueDate = EditorValueParsers.ParseDate(EditorDueDate, "Termin");
        var grossAmount = EditorValueParsers.ParseDecimal(EditorGrossAmount, "Kwota brutto");

        if (SelectedDocumentId is null)
        {
            var created = store.CreateCostDocument(EditorNumber, EditorStatus, documentDate, dueDate, EditorContractorName, EditorCurrencyCode, grossAmount, EditorCostTarget, EditorDescription);
            SelectedDocumentId = created.Id;
            LastActionMessage = $"Dodano dokument kosztowy {created.Number}.";
            return;
        }

        var updated = store.UpdateCostDocument(SelectedDocumentId.Value, EditorNumber, EditorStatus, documentDate, dueDate, EditorContractorName, EditorCurrencyCode, grossAmount, EditorCostTarget, EditorDescription);
        LastActionMessage = $"Zapisano zmiany dokumentu {updated.Number}.";
    }

    private void Delete()
    {
        if (SelectedDocumentId is null)
        {
            throw new InvalidOperationException("Wybierz dokument do usunięcia.");
        }

        var number = SelectedRecord?.Number ?? "dokument";
        store.DeleteCostDocument(SelectedDocumentId.Value);
        SelectedDocumentId = null;
        LastActionMessage = $"Usunięto dokument {number}.";
    }

    private void ToggleArchive()
    {
        if (SelectedDocumentId is null)
        {
            throw new InvalidOperationException("Wybierz dokument do archiwizacji.");
        }

        var record = SelectedRecord;
        store.ArchiveCostDocument(SelectedDocumentId.Value);
        LastActionMessage = record?.IsArchived == true
            ? $"Przywrócono dokument {record.Number}."
            : $"Zarchiwizowano dokument {record?.Number}.";
    }

    private static string ResolveStatusColor(string? status)
        => status?.ToLowerInvariant() switch
        {
            var value when value is not null && value.Contains("akcept") => "#D97706",
            var value when value is not null && value.Contains("bufor") => "#2563EB",
            var value when value is not null && value.Contains("arch") => "#7B8794",
            _ => "#1F8A5B"
        };
}

public sealed record FinanceCostDocumentRowViewModel(
    int Id,
    string Number,
    string Status,
    string StatusColor,
    string ContractorName,
    string DocumentDateLabel,
    string DueDateLabel,
    string CurrencyCode,
    string GrossAmountLabel,
    string CostTarget,
    string ArchivedLabel);
