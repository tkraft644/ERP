using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.Finance;

public sealed partial class FinanceCurrencyRatesWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly FinanceWorkspaceStore store = FinanceWorkspaceStore.Instance;

    public FinanceCurrencyRatesWorkspaceViewModel()
        : base("/finance/currency-rates", "Finanse", "Kursy walut", "Tabela kursów dla dokumentów zagranicznych i rozliczeń walutowych.", false)
    {
        SummaryCards = new ObservableCollection<SummaryCardViewModel>();
        VisibleRates = new ObservableCollection<FinanceCurrencyRateRowViewModel>();
        RunActionCommand = new RelayCommand<string>(RunAction);
        SelectRateCommand = new RelayCommand<FinanceCurrencyRateRowViewModel>(SelectRate);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<SummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<FinanceCurrencyRateRowViewModel> VisibleRates { get; }
    public IRelayCommand<string> RunActionCommand { get; }
    public IRelayCommand<FinanceCurrencyRateRowViewModel> SelectRateCommand { get; }

    public IReadOnlyList<string> SourceFilterOptions => ["Wszystkie źródła", .. store.GetCurrencyRates().Select(item => item.Source).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(item => item)];

    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private string selectedSourceFilter = "Wszystkie źródła";
    [ObservableProperty] private bool showArchived;
    [ObservableProperty] private FinanceCurrencyRateRowViewModel? selectedRate;
    [ObservableProperty] private int? selectedRateId;
    [ObservableProperty] private string editorRateDate = DateTime.Today.ToString("yyyy-MM-dd");
    [ObservableProperty] private string editorBaseCurrency = "EUR";
    [ObservableProperty] private string editorQuoteCurrency = "PLN";
    [ObservableProperty] private string editorRate = "0";
    [ObservableProperty] private string editorSource = "NBP";
    [ObservableProperty] private string editorNotes = string.Empty;
    [ObservableProperty] private string lastActionMessage = "Utrzymuj kursy używane przez finanse i transport zagraniczny.";

    public bool HasSelectedRate => SelectedRateId is not null;
    public FinanceCurrencyRateRecord? SelectedRecord => SelectedRateId is null ? null : store.GetCurrencyRates().FirstOrDefault(item => item.Id == SelectedRateId);
    public string SelectedPair => SelectedRecord is null ? "-" : $"{SelectedRecord.BaseCurrency}/{SelectedRecord.QuoteCurrency}";
    public string SelectedRateLabel => SelectedRecord is null ? "-" : SelectedRecord.Rate.ToString("N4");
    public string SelectedArchiveLabel => SelectedRecord?.IsArchived == true ? "Archiwum" : "Aktywny";

    partial void OnSearchTextChanged(string value) => RefreshRows();
    partial void OnSelectedSourceFilterChanged(string value) => RefreshRows();
    partial void OnShowArchivedChanged(bool value) => RefreshRows();

    partial void OnSelectedRateChanged(FinanceCurrencyRateRowViewModel? value)
    {
        SelectedRateId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedRateIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasSelectedRate));
        OnPropertyChanged(nameof(SelectedRecord));
        OnPropertyChanged(nameof(SelectedPair));
        OnPropertyChanged(nameof(SelectedRateLabel));
        OnPropertyChanged(nameof(SelectedArchiveLabel));
    }

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshRows();
        LoadEditor();
        OnPropertyChanged(nameof(SourceFilterOptions));
    }

    private void RefreshSummaryCards()
    {
        var rows = store.GetCurrencyRates();
        SummaryCards.Clear();
        SummaryCards.Add(new SummaryCardViewModel("Aktywne kursy", rows.Count(item => !item.IsArchived).ToString(), "Kursy dostępne dla bieżących dokumentów.", "#2563EB"));
        SummaryCards.Add(new SummaryCardViewModel("Źródła", rows.Select(item => item.Source).Distinct().Count().ToString(), "NBP i wpisy manualne.", "#1F8A5B"));
        SummaryCards.Add(new SummaryCardViewModel("Pary walutowe", rows.Select(item => $"{item.BaseCurrency}/{item.QuoteCurrency}").Distinct().Count().ToString(), "Zakres utrzymywanych relacji kursowych.", "#D97706"));
        SummaryCards.Add(new SummaryCardViewModel("Archiwum", rows.Count(item => item.IsArchived).ToString(), "Kursy historyczne i wyłączone.", "#7B8794"));
    }

    private void RefreshRows()
    {
        var rows = store.GetCurrencyRates()
            .Where(item =>
                (ShowArchived || !item.IsArchived) &&
                (SelectedSourceFilter == "Wszystkie źródła" || string.Equals(item.Source, SelectedSourceFilter, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.BaseCurrency.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.QuoteCurrency.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Source.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            .Select(item => new FinanceCurrencyRateRowViewModel(
                item.Id,
                item.RateDate.ToString("dd.MM.yyyy"),
                item.BaseCurrency,
                item.QuoteCurrency,
                item.Rate.ToString("N4"),
                item.Source,
                item.IsArchived ? "Archiwum" : "Aktywny",
                item.IsArchived ? "#7B8794" : "#1F8A5B"))
            .ToArray();

        VisibleRates.Clear();
        foreach (var row in rows)
        {
            VisibleRates.Add(row);
        }

        SelectedRate = SelectedRateId is null
            ? VisibleRates.FirstOrDefault()
            : VisibleRates.FirstOrDefault(item => item.Id == SelectedRateId) ?? VisibleRates.FirstOrDefault();
    }

    private void LoadEditor()
    {
        var record = SelectedRecord;
        if (record is null)
        {
            EditorRateDate = DateTime.Today.ToString("yyyy-MM-dd");
            EditorBaseCurrency = "EUR";
            EditorQuoteCurrency = "PLN";
            EditorRate = "0";
            EditorSource = "NBP";
            EditorNotes = string.Empty;
            return;
        }

        EditorRateDate = record.RateDate.ToString("yyyy-MM-dd");
        EditorBaseCurrency = record.BaseCurrency;
        EditorQuoteCurrency = record.QuoteCurrency;
        EditorRate = record.Rate.ToString("N4");
        EditorSource = record.Source;
        EditorNotes = record.Notes ?? string.Empty;
    }

    private void SelectRate(FinanceCurrencyRateRowViewModel? row)
    {
        if (row is not null)
        {
            SelectedRate = row;
        }
    }

    private void RunAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedRate = null;
                    SelectedRateId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nowy kurs waluty.";
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
                    LastActionMessage = $"Wyeksportowano {VisibleRates.Count} kursów walut.";
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
        var rateDate = EditorValueParsers.ParseDate(EditorRateDate, "Data kursu");
        var rate = EditorValueParsers.ParseDecimal(EditorRate, "Kurs");

        if (SelectedRateId is null)
        {
            var created = store.CreateCurrencyRate(rateDate, EditorBaseCurrency, EditorQuoteCurrency, rate, EditorSource, EditorNotes);
            SelectedRateId = created.Id;
            LastActionMessage = $"Dodano kurs {created.BaseCurrency}/{created.QuoteCurrency}.";
            return;
        }

        var updated = store.UpdateCurrencyRate(SelectedRateId.Value, rateDate, EditorBaseCurrency, EditorQuoteCurrency, rate, EditorSource, EditorNotes);
        LastActionMessage = $"Zapisano kurs {updated.BaseCurrency}/{updated.QuoteCurrency}.";
    }

    private void Delete()
    {
        if (SelectedRateId is null)
        {
            throw new InvalidOperationException("Wybierz kurs do usunięcia.");
        }

        store.DeleteCurrencyRate(SelectedRateId.Value);
        SelectedRateId = null;
        LastActionMessage = "Usunięto kurs waluty.";
    }

    private void ToggleArchive()
    {
        if (SelectedRateId is null)
        {
            throw new InvalidOperationException("Wybierz kurs do archiwizacji.");
        }

        var record = SelectedRecord;
        store.ArchiveCurrencyRate(SelectedRateId.Value);
        LastActionMessage = record?.IsArchived == true
            ? $"Przywrócono kurs {record.BaseCurrency}/{record.QuoteCurrency}."
            : $"Zarchiwizowano kurs {record?.BaseCurrency}/{record?.QuoteCurrency}.";
    }
}

public sealed record FinanceCurrencyRateRowViewModel(
    int Id,
    string RateDateLabel,
    string BaseCurrency,
    string QuoteCurrency,
    string RateLabel,
    string Source,
    string StatusLabel,
    string StatusColor);
