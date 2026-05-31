using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.Contractors;

public sealed partial class ContractorListWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly ContractorWorkspaceStore store = ContractorWorkspaceStore.Instance;

    public ContractorListWorkspaceViewModel()
        : base(
            "/contractors/list",
            "Kontrahenci",
            "Lista kontrahentów",
            "Kartoteka partnerów wspólna dla magazynu, transportu, dokumentów i finansów.",
            false)
    {
        SummaryCards = new ObservableCollection<SummaryCardViewModel>();
        VisibleContractors = new ObservableCollection<ContractorRowViewModel>();

        RunActionCommand = new RelayCommand<string>(RunAction);
        SelectContractorCommand = new RelayCommand<ContractorRowViewModel>(SelectContractor);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<SummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<ContractorRowViewModel> VisibleContractors { get; }
    public IRelayCommand<string> RunActionCommand { get; }
    public IRelayCommand<ContractorRowViewModel> SelectContractorCommand { get; }

    public IReadOnlyList<string> TypeFilterOptions => ["Wszystkie typy", .. store.GetTypes()];
    public IReadOnlyList<ContractorAddressRecord> SelectedAddresses => SelectedRecord?.Addresses ?? [];
    public IReadOnlyList<ContractorContactRecord> SelectedContacts => SelectedRecord?.Contacts ?? [];
    public IReadOnlyList<ContractorHistoryRecord> SelectedHistory => SelectedRecord?.History ?? [];

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedTypeFilter = "Wszystkie typy";

    [ObservableProperty]
    private bool showInactive = true;

    [ObservableProperty]
    private bool showArchived;

    [ObservableProperty]
    private ContractorRowViewModel? selectedContractor;

    [ObservableProperty]
    private int? selectedContractorId;

    [ObservableProperty]
    private string editorCode = string.Empty;

    [ObservableProperty]
    private string editorName = string.Empty;

    [ObservableProperty]
    private string editorShortName = string.Empty;

    [ObservableProperty]
    private string editorTaxId = string.Empty;

    [ObservableProperty]
    private string editorCity = string.Empty;

    [ObservableProperty]
    private string editorEmail = string.Empty;

    [ObservableProperty]
    private string editorPhone = string.Empty;

    [ObservableProperty]
    private string editorPaymentTerms = string.Empty;

    [ObservableProperty]
    private string editorCreditLimit = string.Empty;

    [ObservableProperty]
    private string editorBankAccount = string.Empty;

    [ObservableProperty]
    private string editorTypes = "Klient";

    [ObservableProperty]
    private string editorPrimaryAddress = string.Empty;

    [ObservableProperty]
    private string editorNotes = string.Empty;

    [ObservableProperty]
    private bool editorIsActive = true;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz kontrahenta z listy albo utwórz nową kartotekę.";

    public bool HasSelectedContractor => SelectedContractorId is not null;
    public ContractorRecord? SelectedRecord => SelectedContractorId is null
        ? null
        : store.GetContractors().FirstOrDefault(item => item.Id == SelectedContractorId);
    public string SelectedTypesLabel => SelectedRecord is null ? "-" : string.Join(", ", SelectedRecord.Types);
    public string SelectedStatusLabel => SelectedRecord is null ? "-" : SelectedRecord.IsArchived ? "Archiwum" : SelectedRecord.IsActive ? "Aktywny" : "Nieaktywny";
    public string SelectedUpdatedAt => SelectedRecord?.UpdatedAtUtc.ToLocalTime().ToString("dd.MM.yyyy HH:mm") ?? "-";
    public string SelectedPaymentTerms => SelectedRecord?.PaymentTerms ?? "-";
    public string SelectedCreditLimit => SelectedRecord?.CreditLimit ?? "-";
    public string SelectedBankAccount => SelectedRecord?.BankAccount ?? "-";
    public string SelectedAddress => SelectedRecord?.PrimaryAddress ?? "-";
    public string SelectedNote => SelectedRecord?.Notes ?? "Brak notatki operacyjnej.";

    partial void OnSearchTextChanged(string value) => RefreshRows();
    partial void OnSelectedTypeFilterChanged(string value) => RefreshRows();
    partial void OnShowInactiveChanged(bool value) => RefreshRows();
    partial void OnShowArchivedChanged(bool value) => RefreshRows();

    partial void OnSelectedContractorChanged(ContractorRowViewModel? value)
    {
        SelectedContractorId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedContractorIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasSelectedContractor));
        OnPropertyChanged(nameof(SelectedRecord));
        OnPropertyChanged(nameof(SelectedTypesLabel));
        OnPropertyChanged(nameof(SelectedStatusLabel));
        OnPropertyChanged(nameof(SelectedUpdatedAt));
        OnPropertyChanged(nameof(SelectedPaymentTerms));
        OnPropertyChanged(nameof(SelectedCreditLimit));
        OnPropertyChanged(nameof(SelectedBankAccount));
        OnPropertyChanged(nameof(SelectedAddress));
        OnPropertyChanged(nameof(SelectedNote));
        OnPropertyChanged(nameof(SelectedAddresses));
        OnPropertyChanged(nameof(SelectedContacts));
        OnPropertyChanged(nameof(SelectedHistory));
    }

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshRows();
        LoadEditor();
        OnPropertyChanged(nameof(TypeFilterOptions));
    }

    private void RefreshSummaryCards()
    {
        var contractors = store.GetContractors();
        SummaryCards.Clear();
        SummaryCards.Add(new SummaryCardViewModel("Aktywni", contractors.Count(item => item.IsActive && !item.IsArchived).ToString(), "Partnerzy dostępni dla bieżących procesów.", "#2563EB"));
        SummaryCards.Add(new SummaryCardViewModel("Przewoźnicy i agencje", contractors.Count(item => item.Types.Any(type => type.Contains("Przewoźnik", StringComparison.OrdinalIgnoreCase) || type.Contains("Agencja", StringComparison.OrdinalIgnoreCase))).ToString(), "Podmioty używane w transporcie i odprawach.", "#1F8A5B"));
        SummaryCards.Add(new SummaryCardViewModel("Do uzupełnienia", contractors.Count(item => string.IsNullOrWhiteSpace(item.TaxId) || string.IsNullOrWhiteSpace(item.BankAccount)).ToString(), "Kartoteki z brakami rozliczeniowymi albo formalnymi.", "#D97706"));
        SummaryCards.Add(new SummaryCardViewModel("Archiwum", contractors.Count(item => item.IsArchived).ToString(), "Pozycje wycofane z aktywnego obiegu.", "#7B8794"));
    }

    private void RefreshRows()
    {
        var rows = store.GetContractors()
            .Where(item =>
                (ShowArchived || !item.IsArchived) &&
                (ShowInactive || item.IsActive || item.IsArchived) &&
                (SelectedTypeFilter == "Wszystkie typy" || item.Types.Contains(SelectedTypeFilter, StringComparer.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 (item.City?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                 (item.Email?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)))
            .Select(item => new ContractorRowViewModel(
                item.Id,
                item.Code,
                item.Name,
                string.Join(", ", item.Types),
                item.City ?? "-",
                item.Email ?? "-",
                item.IsArchived ? "Archiwum" : item.IsActive ? "Aktywny" : "Nieaktywny",
                item.IsArchived ? "#7B8794" : item.IsActive ? "#1F8A5B" : "#D97706",
                item.UpdatedAtUtc.ToLocalTime().ToString("dd.MM HH:mm")))
            .ToArray();

        VisibleContractors.Clear();
        foreach (var row in rows)
        {
            VisibleContractors.Add(row);
        }

        SelectedContractor = SelectedContractorId is null
            ? VisibleContractors.FirstOrDefault()
            : VisibleContractors.FirstOrDefault(item => item.Id == SelectedContractorId) ?? VisibleContractors.FirstOrDefault();
    }

    private void LoadEditor()
    {
        var record = SelectedRecord;
        if (record is null)
        {
            EditorCode = string.Empty;
            EditorName = string.Empty;
            EditorShortName = string.Empty;
            EditorTaxId = string.Empty;
            EditorCity = string.Empty;
            EditorEmail = string.Empty;
            EditorPhone = string.Empty;
            EditorPaymentTerms = string.Empty;
            EditorCreditLimit = string.Empty;
            EditorBankAccount = string.Empty;
            EditorTypes = "Klient";
            EditorPrimaryAddress = string.Empty;
            EditorNotes = string.Empty;
            EditorIsActive = true;
            return;
        }

        EditorCode = record.Code;
        EditorName = record.Name;
        EditorShortName = record.ShortName ?? string.Empty;
        EditorTaxId = record.TaxId ?? string.Empty;
        EditorCity = record.City ?? string.Empty;
        EditorEmail = record.Email ?? string.Empty;
        EditorPhone = record.Phone ?? string.Empty;
        EditorPaymentTerms = record.PaymentTerms ?? string.Empty;
        EditorCreditLimit = record.CreditLimit ?? string.Empty;
        EditorBankAccount = record.BankAccount ?? string.Empty;
        EditorTypes = string.Join(", ", record.Types);
        EditorPrimaryAddress = record.PrimaryAddress;
        EditorNotes = record.Notes ?? string.Empty;
        EditorIsActive = record.IsActive;
    }

    private void SelectContractor(ContractorRowViewModel? row)
    {
        if (row is not null)
        {
            SelectedContractor = row;
        }
    }

    private void RunAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedContractor = null;
                    SelectedContractorId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nową kartotekę kontrahenta.";
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
                    Export();
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
        if (SelectedContractorId is null)
        {
            var created = store.CreateContractor(
                EditorCode,
                EditorName,
                EditorShortName,
                EditorTaxId,
                EditorCity,
                EditorEmail,
                EditorPhone,
                EditorPaymentTerms,
                EditorCreditLimit,
                EditorBankAccount,
                EditorTypes,
                EditorIsActive,
                EditorPrimaryAddress,
                EditorNotes);
            SelectedContractorId = created.Id;
            LastActionMessage = $"Dodano kartotekę {created.Name}.";
            return;
        }

        var updated = store.UpdateContractor(
            SelectedContractorId.Value,
            EditorCode,
            EditorName,
            EditorShortName,
            EditorTaxId,
            EditorCity,
            EditorEmail,
            EditorPhone,
            EditorPaymentTerms,
            EditorCreditLimit,
            EditorBankAccount,
            EditorTypes,
            EditorIsActive,
            EditorPrimaryAddress,
            EditorNotes);
        LastActionMessage = $"Zapisano zmiany kartoteki {updated.Name}.";
    }

    private void Delete()
    {
        if (SelectedContractorId is null)
        {
            throw new InvalidOperationException("Wybierz kontrahenta do usunięcia.");
        }

        var deletedName = SelectedRecord?.Name ?? "kontrahenta";
        store.DeleteContractor(SelectedContractorId.Value);
        SelectedContractorId = null;
        LastActionMessage = $"Usunięto kartotekę {deletedName}.";
    }

    private void ToggleArchive()
    {
        if (SelectedContractorId is null)
        {
            throw new InvalidOperationException("Wybierz kontrahenta do archiwizacji.");
        }

        if (SelectedRecord?.IsArchived == true)
        {
            store.RestoreContractor(SelectedContractorId.Value);
            LastActionMessage = $"Przywrócono kontrahenta {SelectedRecord?.Name}.";
        }
        else
        {
            store.ArchiveContractor(SelectedContractorId.Value);
            LastActionMessage = $"Zarchiwizowano kontrahenta {SelectedRecord?.Name}.";
        }
    }

    private void Export()
    {
        LastActionMessage = $"Wyeksportowano {VisibleContractors.Count} kontrahentów do zestawienia XLSX / CSV.";
    }
}

public sealed record ContractorRowViewModel(
    int Id,
    string Code,
    string Name,
    string TypesLabel,
    string City,
    string Email,
    string StatusLabel,
    string StatusColor,
    string UpdatedAtLabel);
