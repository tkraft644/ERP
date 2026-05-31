using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.Finance;

public sealed partial class FinanceInvoicesWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly FinanceWorkspaceStore store = FinanceWorkspaceStore.Instance;

    public FinanceInvoicesWorkspaceViewModel()
        : base("/finance/invoices", "Finanse", "Lista faktur", "Faktury sprzedażowe z terminami, walutami i rozliczeniem.", false)
    {
        SummaryCards = new ObservableCollection<SummaryCardViewModel>();
        VisibleInvoices = new ObservableCollection<FinanceInvoiceRowViewModel>();
        RunActionCommand = new RelayCommand<string>(RunAction);
        SelectInvoiceCommand = new RelayCommand<FinanceInvoiceRowViewModel>(SelectInvoice);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<SummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<FinanceInvoiceRowViewModel> VisibleInvoices { get; }
    public IRelayCommand<string> RunActionCommand { get; }
    public IRelayCommand<FinanceInvoiceRowViewModel> SelectInvoiceCommand { get; }

    public IReadOnlyList<string> StatusFilterOptions => ["Wszystkie statusy", .. store.GetInvoices().Select(item => item.Status).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(item => item)];
    public IReadOnlyList<string> CurrencyFilterOptions => ["Wszystkie waluty", .. store.GetCurrencies()];
    public IReadOnlyList<string> ContractorOptions => store.GetContractors();

    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private string selectedStatusFilter = "Wszystkie statusy";
    [ObservableProperty] private string selectedCurrencyFilter = "Wszystkie waluty";
    [ObservableProperty] private bool showArchived;
    [ObservableProperty] private FinanceInvoiceRowViewModel? selectedInvoice;
    [ObservableProperty] private int? selectedInvoiceId;
    [ObservableProperty] private string editorNumber = string.Empty;
    [ObservableProperty] private string editorStatus = "Nowa";
    [ObservableProperty] private string editorInvoiceDate = DateTime.Today.ToString("yyyy-MM-dd");
    [ObservableProperty] private string editorDueDate = DateTime.Today.AddDays(14).ToString("yyyy-MM-dd");
    [ObservableProperty] private string editorContractorName = string.Empty;
    [ObservableProperty] private string editorCurrencyCode = "PLN";
    [ObservableProperty] private string editorGrossAmount = "0";
    [ObservableProperty] private string editorOutstandingAmount = "0";
    [ObservableProperty] private string editorTransportOrderNumber = string.Empty;
    [ObservableProperty] private string editorDescription = string.Empty;
    [ObservableProperty] private string lastActionMessage = "Zarządzaj fakturami, terminami płatności i statusem rozliczenia.";

    public bool HasSelectedInvoice => SelectedInvoiceId is not null;
    public FinanceInvoiceRecord? SelectedRecord => SelectedInvoiceId is null ? null : store.GetInvoices().FirstOrDefault(item => item.Id == SelectedInvoiceId);
    public string SelectedGrossLabel => SelectedRecord is null ? "-" : $"{SelectedRecord.GrossAmount:N2} {SelectedRecord.CurrencyCode}";
    public string SelectedOutstandingLabel => SelectedRecord is null ? "-" : $"{SelectedRecord.OutstandingAmount:N2} {SelectedRecord.CurrencyCode}";
    public string SelectedDueDateLabel => SelectedRecord?.DueDate.ToString("dd.MM.yyyy") ?? "-";
    public string SelectedTransportOrder => SelectedRecord?.TransportOrderNumber ?? "-";
    public string SelectedStatusColor => ResolveStatusColor(SelectedRecord?.Status);

    partial void OnSearchTextChanged(string value) => RefreshRows();
    partial void OnSelectedStatusFilterChanged(string value) => RefreshRows();
    partial void OnSelectedCurrencyFilterChanged(string value) => RefreshRows();
    partial void OnShowArchivedChanged(bool value) => RefreshRows();

    partial void OnSelectedInvoiceChanged(FinanceInvoiceRowViewModel? value)
    {
        SelectedInvoiceId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedInvoiceIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasSelectedInvoice));
        OnPropertyChanged(nameof(SelectedRecord));
        OnPropertyChanged(nameof(SelectedGrossLabel));
        OnPropertyChanged(nameof(SelectedOutstandingLabel));
        OnPropertyChanged(nameof(SelectedDueDateLabel));
        OnPropertyChanged(nameof(SelectedTransportOrder));
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
        var rows = store.GetInvoices();
        SummaryCards.Clear();
        SummaryCards.Add(new SummaryCardViewModel("Aktywne faktury", rows.Count(item => !item.IsArchived).ToString(), "Dokumenty sprzedażowe w bieżącym obiegu.", "#2563EB"));
        SummaryCards.Add(new SummaryCardViewModel("Po terminie", rows.Count(item => item.DueDate < DateTime.Today && item.OutstandingAmount > 0 && !item.IsArchived).ToString(), "Faktury wymagające follow-upu płatniczego.", "#D14343"));
        SummaryCards.Add(new SummaryCardViewModel("Outstanding", $"{rows.Where(item => !item.IsArchived).Sum(item => item.OutstandingAmount):N0} zł", "Kwota pozostająca do rozliczenia.", "#D97706"));
        SummaryCards.Add(new SummaryCardViewModel("Eksport", rows.Count(item => !string.IsNullOrWhiteSpace(item.TransportOrderNumber)).ToString(), "Faktury połączone ze zleceniami transportowymi.", "#1F8A5B"));
    }

    private void RefreshRows()
    {
        var rows = store.GetInvoices()
            .Where(item =>
                (ShowArchived || !item.IsArchived) &&
                (SelectedStatusFilter == "Wszystkie statusy" || string.Equals(item.Status, SelectedStatusFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedCurrencyFilter == "Wszystkie waluty" || string.Equals(item.CurrencyCode, SelectedCurrencyFilter, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.Number.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.ContractorName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 (item.TransportOrderNumber?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)))
            .Select(item => new FinanceInvoiceRowViewModel(
                item.Id,
                item.Number,
                item.Status,
                ResolveStatusColor(item.Status),
                item.ContractorName,
                item.InvoiceDate.ToString("dd.MM.yyyy"),
                item.DueDate.ToString("dd.MM.yyyy"),
                item.CurrencyCode,
                $"{item.GrossAmount:N2}",
                $"{item.OutstandingAmount:N2}",
                item.TransportOrderNumber ?? "-"))
            .ToArray();

        VisibleInvoices.Clear();
        foreach (var row in rows)
        {
            VisibleInvoices.Add(row);
        }

        SelectedInvoice = SelectedInvoiceId is null
            ? VisibleInvoices.FirstOrDefault()
            : VisibleInvoices.FirstOrDefault(item => item.Id == SelectedInvoiceId) ?? VisibleInvoices.FirstOrDefault();
    }

    private void LoadEditor()
    {
        var record = SelectedRecord;
        if (record is null)
        {
            EditorNumber = string.Empty;
            EditorStatus = "Nowa";
            EditorInvoiceDate = DateTime.Today.ToString("yyyy-MM-dd");
            EditorDueDate = DateTime.Today.AddDays(14).ToString("yyyy-MM-dd");
            EditorContractorName = ContractorOptions.FirstOrDefault() ?? string.Empty;
            EditorCurrencyCode = "PLN";
            EditorGrossAmount = "0";
            EditorOutstandingAmount = "0";
            EditorTransportOrderNumber = string.Empty;
            EditorDescription = string.Empty;
            return;
        }

        EditorNumber = record.Number;
        EditorStatus = record.Status;
        EditorInvoiceDate = record.InvoiceDate.ToString("yyyy-MM-dd");
        EditorDueDate = record.DueDate.ToString("yyyy-MM-dd");
        EditorContractorName = record.ContractorName;
        EditorCurrencyCode = record.CurrencyCode;
        EditorGrossAmount = record.GrossAmount.ToString("N2");
        EditorOutstandingAmount = record.OutstandingAmount.ToString("N2");
        EditorTransportOrderNumber = record.TransportOrderNumber ?? string.Empty;
        EditorDescription = record.Description ?? string.Empty;
    }

    private void SelectInvoice(FinanceInvoiceRowViewModel? row)
    {
        if (row is not null)
        {
            SelectedInvoice = row;
        }
    }

    private void RunAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedInvoice = null;
                    SelectedInvoiceId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nową fakturę.";
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
                    LastActionMessage = $"Wyeksportowano {VisibleInvoices.Count} faktur do rejestru sprzedaży.";
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
        var invoiceDate = EditorValueParsers.ParseDate(EditorInvoiceDate, "Data faktury");
        var dueDate = EditorValueParsers.ParseDate(EditorDueDate, "Termin płatności");
        var grossAmount = EditorValueParsers.ParseDecimal(EditorGrossAmount, "Kwota brutto");
        var outstandingAmount = EditorValueParsers.ParseDecimal(EditorOutstandingAmount, "Kwota otwarta");

        if (SelectedInvoiceId is null)
        {
            var created = store.CreateInvoice(EditorNumber, EditorStatus, invoiceDate, dueDate, EditorContractorName, EditorCurrencyCode, grossAmount, outstandingAmount, EditorTransportOrderNumber, EditorDescription);
            SelectedInvoiceId = created.Id;
            LastActionMessage = $"Dodano fakturę {created.Number}.";
            return;
        }

        var updated = store.UpdateInvoice(SelectedInvoiceId.Value, EditorNumber, EditorStatus, invoiceDate, dueDate, EditorContractorName, EditorCurrencyCode, grossAmount, outstandingAmount, EditorTransportOrderNumber, EditorDescription);
        LastActionMessage = $"Zapisano zmiany faktury {updated.Number}.";
    }

    private void Delete()
    {
        if (SelectedInvoiceId is null)
        {
            throw new InvalidOperationException("Wybierz fakturę do usunięcia.");
        }

        var number = SelectedRecord?.Number ?? "fakturę";
        store.DeleteInvoice(SelectedInvoiceId.Value);
        SelectedInvoiceId = null;
        LastActionMessage = $"Usunięto fakturę {number}.";
    }

    private void ToggleArchive()
    {
        if (SelectedInvoiceId is null)
        {
            throw new InvalidOperationException("Wybierz fakturę do archiwizacji.");
        }

        var record = SelectedRecord;
        store.ArchiveInvoice(SelectedInvoiceId.Value);
        LastActionMessage = record?.IsArchived == true
            ? $"Przywrócono fakturę {record.Number}."
            : $"Zarchiwizowano fakturę {record?.Number}.";
    }

    private static string ResolveStatusColor(string? status)
        => status?.ToLowerInvariant() switch
        {
            var value when value is not null && value.Contains("termin") => "#D14343",
            var value when value is not null && value.Contains("części") => "#D97706",
            var value when value is not null && value.Contains("arch") => "#7B8794",
            _ => "#1F8A5B"
        };
}

public sealed record FinanceInvoiceRowViewModel(
    int Id,
    string Number,
    string Status,
    string StatusColor,
    string ContractorName,
    string InvoiceDateLabel,
    string DueDateLabel,
    string CurrencyCode,
    string GrossAmountLabel,
    string OutstandingLabel,
    string TransportOrderNumber);
