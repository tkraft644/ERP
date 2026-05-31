using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.Finance;

public sealed partial class FinancePaymentsWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly FinanceWorkspaceStore store = FinanceWorkspaceStore.Instance;

    public FinancePaymentsWorkspaceViewModel()
        : base("/finance/payments", "Finanse", "Płatności", "Wpływy i wypływy z rozliczeniem do dokumentów źródłowych.", false)
    {
        SummaryCards = new ObservableCollection<SummaryCardViewModel>();
        VisiblePayments = new ObservableCollection<FinancePaymentRowViewModel>();
        RunActionCommand = new RelayCommand<string>(RunAction);
        SelectPaymentCommand = new RelayCommand<FinancePaymentRowViewModel>(SelectPayment);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<SummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<FinancePaymentRowViewModel> VisiblePayments { get; }
    public IRelayCommand<string> RunActionCommand { get; }
    public IRelayCommand<FinancePaymentRowViewModel> SelectPaymentCommand { get; }

    public IReadOnlyList<string> DirectionFilterOptions => ["Wszystkie kierunki", "Wpływ", "Wypływ"];
    public IReadOnlyList<string> CurrencyFilterOptions => ["Wszystkie waluty", .. store.GetCurrencies()];
    public IReadOnlyList<string> ContractorOptions => store.GetContractors();

    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private string selectedDirectionFilter = "Wszystkie kierunki";
    [ObservableProperty] private string selectedCurrencyFilter = "Wszystkie waluty";
    [ObservableProperty] private FinancePaymentRowViewModel? selectedPayment;
    [ObservableProperty] private int? selectedPaymentId;
    [ObservableProperty] private string editorPaymentDate = DateTime.Today.ToString("yyyy-MM-dd");
    [ObservableProperty] private string editorDirection = "Wpływ";
    [ObservableProperty] private string editorContractorName = string.Empty;
    [ObservableProperty] private string editorCurrencyCode = "PLN";
    [ObservableProperty] private string editorAmount = "0";
    [ObservableProperty] private string editorSettledAmount = "0";
    [ObservableProperty] private string editorMethod = "Przelew";
    [ObservableProperty] private string editorReferenceNumber = string.Empty;
    [ObservableProperty] private string editorNotes = string.Empty;
    [ObservableProperty] private string lastActionMessage = "Zarządzaj wpływami, wypływami i ich rozliczeniem.";

    public bool HasSelectedPayment => SelectedPaymentId is not null;
    public FinancePaymentRecord? SelectedRecord => SelectedPaymentId is null ? null : store.GetPayments().FirstOrDefault(item => item.Id == SelectedPaymentId);
    public string SelectedAmountLabel => SelectedRecord is null ? "-" : $"{SelectedRecord.Amount:N2} {SelectedRecord.CurrencyCode}";
    public string SelectedSettledLabel => SelectedRecord is null ? "-" : $"{SelectedRecord.SettledAmount:N2} {SelectedRecord.CurrencyCode}";
    public string SelectedRemainingLabel => SelectedRecord is null ? "-" : $"{Math.Max(0m, SelectedRecord.Amount - SelectedRecord.SettledAmount):N2} {SelectedRecord.CurrencyCode}";
    public string SelectedDirectionColor => SelectedRecord?.Direction.Equals("Wpływ", StringComparison.OrdinalIgnoreCase) == true ? "#1F8A5B" : "#D97706";

    partial void OnSearchTextChanged(string value) => RefreshRows();
    partial void OnSelectedDirectionFilterChanged(string value) => RefreshRows();
    partial void OnSelectedCurrencyFilterChanged(string value) => RefreshRows();

    partial void OnSelectedPaymentChanged(FinancePaymentRowViewModel? value)
    {
        SelectedPaymentId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedPaymentIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasSelectedPayment));
        OnPropertyChanged(nameof(SelectedRecord));
        OnPropertyChanged(nameof(SelectedAmountLabel));
        OnPropertyChanged(nameof(SelectedSettledLabel));
        OnPropertyChanged(nameof(SelectedRemainingLabel));
        OnPropertyChanged(nameof(SelectedDirectionColor));
    }

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshRows();
        LoadEditor();
        OnPropertyChanged(nameof(CurrencyFilterOptions));
        OnPropertyChanged(nameof(ContractorOptions));
    }

    private void RefreshSummaryCards()
    {
        var rows = store.GetPayments();
        SummaryCards.Clear();
        SummaryCards.Add(new SummaryCardViewModel("Wpływy", rows.Count(item => item.Direction.Equals("Wpływ", StringComparison.OrdinalIgnoreCase)).ToString(), "Zaksięgowane wpływy z dokumentów sprzedażowych.", "#1F8A5B"));
        SummaryCards.Add(new SummaryCardViewModel("Wypływy", rows.Count(item => item.Direction.Equals("Wypływ", StringComparison.OrdinalIgnoreCase)).ToString(), "Koszty i przelewy wychodzące.", "#D97706"));
        SummaryCards.Add(new SummaryCardViewModel("Do przypisania", rows.Count(item => item.Amount > item.SettledAmount).ToString(), "Płatności nadal z otwartą kwotą.", "#2563EB"));
        SummaryCards.Add(new SummaryCardViewModel("Cashflow", $"{rows.Sum(item => item.Direction.Equals("Wpływ", StringComparison.OrdinalIgnoreCase) ? item.Amount : -item.Amount):N0} zł", "Saldo demonstracyjne bieżącego zestawu.", "#7C3AED"));
    }

    private void RefreshRows()
    {
        var rows = store.GetPayments()
            .Where(item =>
                (SelectedDirectionFilter == "Wszystkie kierunki" || string.Equals(item.Direction, SelectedDirectionFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedCurrencyFilter == "Wszystkie waluty" || string.Equals(item.CurrencyCode, SelectedCurrencyFilter, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SearchText) ||
                 (item.ReferenceNumber?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                 (item.ContractorName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                 item.Method.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            .Select(item => new FinancePaymentRowViewModel(
                item.Id,
                item.PaymentDate.ToString("dd.MM.yyyy"),
                item.Direction,
                item.Direction.Equals("Wpływ", StringComparison.OrdinalIgnoreCase) ? "#1F8A5B" : "#D97706",
                item.ContractorName ?? "-",
                item.CurrencyCode,
                $"{item.Amount:N2}",
                $"{item.SettledAmount:N2}",
                $"{Math.Max(0m, item.Amount - item.SettledAmount):N2}",
                item.Method,
                item.ReferenceNumber ?? "-"))
            .ToArray();

        VisiblePayments.Clear();
        foreach (var row in rows)
        {
            VisiblePayments.Add(row);
        }

        SelectedPayment = SelectedPaymentId is null
            ? VisiblePayments.FirstOrDefault()
            : VisiblePayments.FirstOrDefault(item => item.Id == SelectedPaymentId) ?? VisiblePayments.FirstOrDefault();
    }

    private void LoadEditor()
    {
        var record = SelectedRecord;
        if (record is null)
        {
            EditorPaymentDate = DateTime.Today.ToString("yyyy-MM-dd");
            EditorDirection = "Wpływ";
            EditorContractorName = ContractorOptions.FirstOrDefault() ?? string.Empty;
            EditorCurrencyCode = "PLN";
            EditorAmount = "0";
            EditorSettledAmount = "0";
            EditorMethod = "Przelew";
            EditorReferenceNumber = string.Empty;
            EditorNotes = string.Empty;
            return;
        }

        EditorPaymentDate = record.PaymentDate.ToString("yyyy-MM-dd");
        EditorDirection = record.Direction;
        EditorContractorName = record.ContractorName ?? string.Empty;
        EditorCurrencyCode = record.CurrencyCode;
        EditorAmount = record.Amount.ToString("N2");
        EditorSettledAmount = record.SettledAmount.ToString("N2");
        EditorMethod = record.Method;
        EditorReferenceNumber = record.ReferenceNumber ?? string.Empty;
        EditorNotes = record.Notes ?? string.Empty;
    }

    private void SelectPayment(FinancePaymentRowViewModel? row)
    {
        if (row is not null)
        {
            SelectedPayment = row;
        }
    }

    private void RunAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedPayment = null;
                    SelectedPaymentId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nową płatność.";
                    break;
                case "Save":
                    Save();
                    break;
                case "Delete":
                    Delete();
                    break;
                case "Export":
                    LastActionMessage = $"Wyeksportowano {VisiblePayments.Count} płatności do arkusza cashflow.";
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
        var paymentDate = EditorValueParsers.ParseDate(EditorPaymentDate, "Data płatności");
        var amount = EditorValueParsers.ParseDecimal(EditorAmount, "Kwota");
        var settledAmount = EditorValueParsers.ParseDecimal(EditorSettledAmount, "Kwota rozliczona");

        if (SelectedPaymentId is null)
        {
            var created = store.CreatePayment(paymentDate, EditorDirection, EditorContractorName, EditorCurrencyCode, amount, settledAmount, EditorMethod, EditorReferenceNumber, EditorNotes);
            SelectedPaymentId = created.Id;
            LastActionMessage = $"Dodano płatność {created.ReferenceNumber ?? created.Id.ToString()}.";
            return;
        }

        var updated = store.UpdatePayment(SelectedPaymentId.Value, paymentDate, EditorDirection, EditorContractorName, EditorCurrencyCode, amount, settledAmount, EditorMethod, EditorReferenceNumber, EditorNotes);
        LastActionMessage = $"Zapisano płatność {updated.ReferenceNumber ?? updated.Id.ToString()}.";
    }

    private void Delete()
    {
        if (SelectedPaymentId is null)
        {
            throw new InvalidOperationException("Wybierz płatność do usunięcia.");
        }

        store.DeletePayment(SelectedPaymentId.Value);
        SelectedPaymentId = null;
        LastActionMessage = "Usunięto płatność i powiązane rozliczenia demonstracyjne.";
    }
}

public sealed record FinancePaymentRowViewModel(
    int Id,
    string PaymentDateLabel,
    string Direction,
    string DirectionColor,
    string ContractorName,
    string CurrencyCode,
    string AmountLabel,
    string SettledAmountLabel,
    string RemainingAmountLabel,
    string Method,
    string ReferenceNumber);
