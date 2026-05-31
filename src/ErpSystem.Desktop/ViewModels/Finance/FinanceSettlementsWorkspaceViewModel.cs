using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.Finance;

public sealed partial class FinanceSettlementsWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly FinanceWorkspaceStore store = FinanceWorkspaceStore.Instance;

    public FinanceSettlementsWorkspaceViewModel()
        : base("/finance/settlements", "Finanse", "Rozliczenia", "Powiązania płatności z fakturami i dokumentami kosztowymi.", false)
    {
        SummaryCards = new ObservableCollection<SummaryCardViewModel>();
        VisibleSettlements = new ObservableCollection<FinanceSettlementRowViewModel>();
        RunActionCommand = new RelayCommand<string>(RunAction);
        SelectSettlementCommand = new RelayCommand<FinanceSettlementRowViewModel>(SelectSettlement);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<SummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<FinanceSettlementRowViewModel> VisibleSettlements { get; }
    public IRelayCommand<string> RunActionCommand { get; }
    public IRelayCommand<FinanceSettlementRowViewModel> SelectSettlementCommand { get; }

    public IReadOnlyList<string> StatusFilterOptions => ["Wszystkie statusy", .. store.GetSettlements().Select(item => item.Status).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(item => item)];
    public IReadOnlyList<FinancePaymentRecord> PaymentOptions => store.GetPayments();

    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private string selectedStatusFilter = "Wszystkie statusy";
    [ObservableProperty] private FinanceSettlementRowViewModel? selectedSettlement;
    [ObservableProperty] private int? selectedSettlementId;
    [ObservableProperty] private FinancePaymentRecord? selectedPaymentOption;
    [ObservableProperty] private string editorPaymentReference = string.Empty;
    [ObservableProperty] private string editorTargetDocumentNumber = string.Empty;
    [ObservableProperty] private string editorTargetDocumentType = "Faktura";
    [ObservableProperty] private string editorAmount = "0";
    [ObservableProperty] private string editorStatus = "Nowe";
    [ObservableProperty] private string editorNotes = string.Empty;
    [ObservableProperty] private string lastActionMessage = "Łącz płatności z dokumentami i kontroluj kwoty outstanding.";

    public bool HasSelectedSettlement => SelectedSettlementId is not null;
    public FinanceSettlementRecord? SelectedRecord => SelectedSettlementId is null ? null : store.GetSettlements().FirstOrDefault(item => item.Id == SelectedSettlementId);
    public string SelectedSettledAtLabel => SelectedRecord?.SettledAt.ToString("dd.MM.yyyy HH:mm") ?? "-";
    public string SelectedStatusColor => ResolveStatusColor(SelectedRecord?.Status);

    partial void OnSearchTextChanged(string value) => RefreshRows();
    partial void OnSelectedStatusFilterChanged(string value) => RefreshRows();

    partial void OnSelectedSettlementChanged(FinanceSettlementRowViewModel? value)
    {
        SelectedSettlementId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedSettlementIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasSelectedSettlement));
        OnPropertyChanged(nameof(SelectedRecord));
        OnPropertyChanged(nameof(SelectedSettledAtLabel));
        OnPropertyChanged(nameof(SelectedStatusColor));
    }

    partial void OnSelectedPaymentOptionChanged(FinancePaymentRecord? value)
    {
        if (value is not null)
        {
            EditorPaymentReference = value.ReferenceNumber ?? $"PAY-{value.Id}";
        }
    }

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshRows();
        LoadEditor();
        OnPropertyChanged(nameof(StatusFilterOptions));
        OnPropertyChanged(nameof(PaymentOptions));
    }

    private void RefreshSummaryCards()
    {
        var rows = store.GetSettlements();
        SummaryCards.Clear();
        SummaryCards.Add(new SummaryCardViewModel("Rozliczenia aktywne", rows.Count.ToString(), "Powiązania płatności z dokumentami.", "#2563EB"));
        SummaryCards.Add(new SummaryCardViewModel("Łączna kwota", $"{rows.Sum(item => item.Amount):N0} zł", "Wartość rozliczeń w bieżącym zestawie.", "#1F8A5B"));
        SummaryCards.Add(new SummaryCardViewModel("Faktury", rows.Count(item => item.TargetDocumentType.Contains("Fakt", StringComparison.OrdinalIgnoreCase)).ToString(), "Pozycje spięte z fakturami.", "#D97706"));
        SummaryCards.Add(new SummaryCardViewModel("Koszty", rows.Count(item => item.TargetDocumentType.Contains("Koszt", StringComparison.OrdinalIgnoreCase)).ToString(), "Rozliczenia po stronie zakupowej.", "#7C3AED"));
    }

    private void RefreshRows()
    {
        var rows = store.GetSettlements()
            .Where(item =>
                (SelectedStatusFilter == "Wszystkie statusy" || string.Equals(item.Status, SelectedStatusFilter, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.PaymentReference.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.TargetDocumentNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.TargetDocumentType.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            .Select(item => new FinanceSettlementRowViewModel(
                item.Id,
                item.PaymentReference,
                item.TargetDocumentNumber,
                item.TargetDocumentType,
                $"{item.Amount:N2}",
                item.Status,
                ResolveStatusColor(item.Status),
                item.SettledAt.ToString("dd.MM.yyyy HH:mm")))
            .ToArray();

        VisibleSettlements.Clear();
        foreach (var row in rows)
        {
            VisibleSettlements.Add(row);
        }

        SelectedSettlement = SelectedSettlementId is null
            ? VisibleSettlements.FirstOrDefault()
            : VisibleSettlements.FirstOrDefault(item => item.Id == SelectedSettlementId) ?? VisibleSettlements.FirstOrDefault();
    }

    private void LoadEditor()
    {
        var record = SelectedRecord;
        if (record is null)
        {
            SelectedPaymentOption = PaymentOptions.FirstOrDefault();
            EditorPaymentReference = SelectedPaymentOption?.ReferenceNumber ?? string.Empty;
            EditorTargetDocumentNumber = string.Empty;
            EditorTargetDocumentType = "Faktura";
            EditorAmount = "0";
            EditorStatus = "Nowe";
            EditorNotes = string.Empty;
            return;
        }

        SelectedPaymentOption = PaymentOptions.FirstOrDefault(item => item.Id == record.PaymentReferenceId) ?? PaymentOptions.FirstOrDefault();
        EditorPaymentReference = record.PaymentReference;
        EditorTargetDocumentNumber = record.TargetDocumentNumber;
        EditorTargetDocumentType = record.TargetDocumentType;
        EditorAmount = record.Amount.ToString("N2");
        EditorStatus = record.Status;
        EditorNotes = record.Notes ?? string.Empty;
    }

    private void SelectSettlement(FinanceSettlementRowViewModel? row)
    {
        if (row is not null)
        {
            SelectedSettlement = row;
        }
    }

    private void RunAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedSettlement = null;
                    SelectedSettlementId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nowe rozliczenie.";
                    break;
                case "Save":
                    Save();
                    break;
                case "Delete":
                    Delete();
                    break;
                case "Export":
                    LastActionMessage = $"Wyeksportowano {VisibleSettlements.Count} rozliczeń.";
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
        var payment = SelectedPaymentOption ?? throw new InvalidOperationException("Wybierz płatność źródłową.");
        var amount = EditorValueParsers.ParseDecimal(EditorAmount, "Kwota rozliczenia");

        if (SelectedSettlementId is null)
        {
            var created = store.CreateSettlement(payment.Id, EditorPaymentReference, EditorTargetDocumentNumber, EditorTargetDocumentType, amount, EditorStatus, EditorNotes);
            SelectedSettlementId = created.Id;
            LastActionMessage = $"Dodano rozliczenie do dokumentu {created.TargetDocumentNumber}.";
            return;
        }

        var updated = store.UpdateSettlement(SelectedSettlementId.Value, payment.Id, EditorPaymentReference, EditorTargetDocumentNumber, EditorTargetDocumentType, amount, EditorStatus, EditorNotes);
        LastActionMessage = $"Zapisano rozliczenie {updated.TargetDocumentNumber}.";
    }

    private void Delete()
    {
        if (SelectedSettlementId is null)
        {
            throw new InvalidOperationException("Wybierz rozliczenie do usunięcia.");
        }

        store.DeleteSettlement(SelectedSettlementId.Value);
        SelectedSettlementId = null;
        LastActionMessage = "Usunięto rozliczenie.";
    }

    private static string ResolveStatusColor(string? status)
        => status?.ToLowerInvariant() switch
        {
            var value when value is not null && value.Contains("now") => "#2563EB",
            var value when value is not null && value.Contains("błąd") => "#D14343",
            _ => "#1F8A5B"
        };
}

public sealed record FinanceSettlementRowViewModel(
    int Id,
    string PaymentReference,
    string TargetDocumentNumber,
    string TargetDocumentType,
    string AmountLabel,
    string Status,
    string StatusColor,
    string SettledAtLabel);
