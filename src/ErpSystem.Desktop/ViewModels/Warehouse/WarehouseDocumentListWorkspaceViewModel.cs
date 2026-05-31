using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.Warehouse;

public sealed partial class WarehouseDocumentListWorkspaceViewModel : WorkspaceViewModelBase
{
    private static readonly CultureInfo PolishCulture = CultureInfo.GetCultureInfo("pl-PL");

    private readonly WarehouseWorkspaceStore store;
    private List<WarehouseDocumentRowViewModel> allDocuments = [];

    public WarehouseDocumentListWorkspaceViewModel(WarehouseWorkspaceStore store)
        : base(
            "/warehouse/documents",
            "Magazyn",
            "Lista dokumentow magazynowych",
            "PZ, WZ, MM, RW, PW i INW z filtrami, statusem oraz detalem zaznaczonego dokumentu.",
            false)
    {
        this.store = store;
        store.Changed += OnStoreChanged;

        SummaryCards = [];
        VisibleDocuments = [];
        TypeFilterOptions = ["Wszystkie typy"];
        StatusFilterOptions = ["Wszystkie statusy", "Roboczy", "Zaksiegowany", "Zarchiwizowany"];
        WarehouseFilterOptions = ["Wszystkie magazyny"];

        RunDocumentActionCommand = new AsyncRelayCommand<string>(RunDocumentActionAsync);
        SelectDocumentCommand = new RelayCommand<WarehouseDocumentRowViewModel>(SelectDocument);
        SaveDocumentChangesCommand = new AsyncRelayCommand(SaveDocumentChangesAsync, () => IsEditMode && SelectedDocument is not null);
        CancelDocumentEditCommand = new RelayCommand(CancelDocumentEdit);

        _ = LoadAsync();
    }

    public ObservableCollection<WarehouseSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<WarehouseDocumentRowViewModel> VisibleDocuments { get; }
    public IReadOnlyList<string> TypeFilterOptions { get; private set; }
    public IReadOnlyList<string> StatusFilterOptions { get; }
    public IReadOnlyList<string> WarehouseFilterOptions { get; private set; }
    public IAsyncRelayCommand<string> RunDocumentActionCommand { get; }
    public IRelayCommand<WarehouseDocumentRowViewModel> SelectDocumentCommand { get; }
    public IAsyncRelayCommand SaveDocumentChangesCommand { get; }
    public IRelayCommand CancelDocumentEditCommand { get; }

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedTypeFilter = "Wszystkie typy";

    [ObservableProperty]
    private string selectedStatusFilter = "Wszystkie statusy";

    [ObservableProperty]
    private string selectedWarehouseFilter = "Wszystkie magazyny";

    [ObservableProperty]
    private bool onlyPendingApproval;

    [ObservableProperty]
    private bool showArchived;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz dokument z listy, aby zobaczyc pozycje i wykonac akcje.";

    [ObservableProperty]
    private WarehouseDocumentRowViewModel? selectedDocument;

    [ObservableProperty]
    private bool isEditMode;

    [ObservableProperty]
    private string editorDocumentDate = string.Empty;

    [ObservableProperty]
    private string editorExternalReference = string.Empty;

    [ObservableProperty]
    private string editorNotes = string.Empty;

    public bool HasSelectedDocument => SelectedDocument is not null;
    public string SelectedDocumentNumber => SelectedDocument?.Number ?? "-";
    public string SelectedDocumentType => SelectedDocument?.Type ?? "-";
    public string SelectedDocumentWarehouse => SelectedDocument?.Warehouse ?? "-";
    public string SelectedDocumentCounterparty => SelectedDocument?.Counterparty ?? "-";
    public string SelectedDocumentStatus => SelectedDocument?.Status ?? "-";
    public string SelectedDocumentStatusColor => SelectedDocument?.StatusColor ?? "#7B8794";
    public string SelectedDocumentDate => SelectedDocument?.DocumentDate ?? "-";
    public string SelectedDocumentOperator => SelectedDocument?.Operator ?? "-";
    public string SelectedDocumentQuantity => SelectedDocument?.QuantityLabel ?? "-";
    public string SelectedDocumentValue => SelectedDocument?.ValueLabel ?? "-";
    public string SelectedDocumentBadge => SelectedDocument?.BadgeText ?? "-";
    public string SelectedDocumentNote => SelectedDocument?.Note ?? "Brak notatki operacyjnej.";
    public IReadOnlyList<WarehouseDocumentPositionViewModel> SelectedDocumentPositions => SelectedDocument?.Positions ?? [];
    public IReadOnlyList<WarehouseDocumentHistoryViewModel> SelectedDocumentHistory => SelectedDocument?.History ?? [];

    partial void OnSearchTextChanged(string value) => ApplyFilters();
    partial void OnSelectedTypeFilterChanged(string value) => ApplyFilters();
    partial void OnSelectedStatusFilterChanged(string value) => ApplyFilters();
    partial void OnSelectedWarehouseFilterChanged(string value) => ApplyFilters();
    partial void OnOnlyPendingApprovalChanged(bool value) => ApplyFilters();
    partial void OnShowArchivedChanged(bool value) => ApplyFilters();

    partial void OnSelectedDocumentChanged(WarehouseDocumentRowViewModel? value)
    {
        LoadEditor();
        OnPropertyChanged(nameof(HasSelectedDocument));
        OnPropertyChanged(nameof(SelectedDocumentNumber));
        OnPropertyChanged(nameof(SelectedDocumentType));
        OnPropertyChanged(nameof(SelectedDocumentWarehouse));
        OnPropertyChanged(nameof(SelectedDocumentCounterparty));
        OnPropertyChanged(nameof(SelectedDocumentStatus));
        OnPropertyChanged(nameof(SelectedDocumentStatusColor));
        OnPropertyChanged(nameof(SelectedDocumentDate));
        OnPropertyChanged(nameof(SelectedDocumentOperator));
        OnPropertyChanged(nameof(SelectedDocumentQuantity));
        OnPropertyChanged(nameof(SelectedDocumentValue));
        OnPropertyChanged(nameof(SelectedDocumentBadge));
        OnPropertyChanged(nameof(SelectedDocumentNote));
        OnPropertyChanged(nameof(SelectedDocumentPositions));
        OnPropertyChanged(nameof(SelectedDocumentHistory));
        SaveDocumentChangesCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsEditModeChanged(bool value) => SaveDocumentChangesCommand.NotifyCanExecuteChanged();

    private void OnStoreChanged(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() => _ = LoadAsync());
    }

    private async Task LoadAsync()
    {
        var preferredNumber = SelectedDocument?.Number;

        try
        {
            var snapshot = await store.GetSnapshotAsync();
            allDocuments = BuildRows(snapshot).ToList();
            TypeFilterOptions = ["Wszystkie typy", .. allDocuments.Select(item => item.Type).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(item => item)];
            WarehouseFilterOptions = ["Wszystkie magazyny", .. allDocuments.Select(item => item.Warehouse).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(item => item)];
            OnPropertyChanged(nameof(TypeFilterOptions));
            OnPropertyChanged(nameof(WarehouseFilterOptions));

            RefreshSummaryCards(allDocuments);
            ApplyFilters(preferredNumber);

            LastActionMessage = allDocuments.Count == 0
                ? "Brak dokumentow magazynowych w bazie SQL."
                : "Lista dokumentow jest pobrana z API i bazy SQL.";
        }
        catch (Exception exception)
        {
            allDocuments = [];
            VisibleDocuments.Clear();
            SummaryCards.Clear();
            SelectedDocument = null;
            LastActionMessage = $"Nie udało się pobrać dokumentów z API/SQL: {TrimExceptionMessage(exception.Message)}";
        }
    }

    private static IReadOnlyList<WarehouseDocumentRowViewModel> BuildRows(WarehouseWorkspaceSnapshot snapshot)
    {
        var productsById = snapshot.ReferenceData.Products.ToDictionary(item => item.Id);

        return snapshot.Documents
            .OrderByDescending(item => item.DocumentDate)
            .ThenByDescending(item => item.Id)
            .Select(document =>
            {
                var positionCount = document.Positions.Count;
                var totalQuantity = document.Positions.Sum(item => Math.Abs(item.Quantity));
                var totalValue = document.Positions.Sum(item => Math.Abs(item.Quantity) * (item.UnitPrice ?? 0m));
                var unitSymbols = document.Positions
                    .Select(item => productsById.TryGetValue(item.ProductId, out var product) ? product.UnitOfMeasureSymbol : string.Empty)
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                var positions = document.Positions
                    .Select(position =>
                    {
                        var unitSymbol = productsById.TryGetValue(position.ProductId, out var product)
                            ? product.UnitOfMeasureSymbol
                            : string.Empty;
                        var location = position.SourceLocationCode ?? position.TargetLocationCode ?? "-";
                        var unitPrice = position.UnitPrice.HasValue ? $"{position.UnitPrice.Value:N2} zl" : "-";
                        return new WarehouseDocumentPositionViewModel(
                            position.Id,
                            position.ProductCode,
                            position.ProductName,
                            $"{position.Quantity:N2} {unitSymbol}".Replace(",00", string.Empty),
                            unitPrice,
                            location);
                    })
                    .ToArray();

                var history = BuildHistory(document).ToArray();

                return new WarehouseDocumentRowViewModel(
                    document.Id,
                    document.Number,
                    document.Type,
                    ResolveWarehouseLabel(document),
                    document.ContractorName ?? "Wewnętrzny obieg",
                    LocalizeDocumentStatus(document.Status),
                    ResolveDocumentStateColor(document.Status),
                    document.DocumentDate.ToString("dd.MM.yyyy HH:mm"),
                    document.PostedByUserId.HasValue ? $"Uzytkownik #{document.PostedByUserId.Value}" : "System",
                    positionCount,
                    unitSymbols.Length == 1
                        ? $"{totalQuantity:N2} {unitSymbols[0]}".Replace(",00", string.Empty)
                        : $"{totalQuantity:N2}".Replace(",00", string.Empty),
                    $"{totalValue:N2} zl",
                    document.ExternalReference ?? document.Type,
                    string.Equals(document.Status, "Cancelled", StringComparison.OrdinalIgnoreCase),
                    string.Equals(document.Status, "Draft", StringComparison.OrdinalIgnoreCase),
                    document.Notes ?? "Brak notatki operacyjnej.",
                    document.ExternalReference,
                    document.ContractorId,
                    document.SourceWarehouseId,
                    document.SourceLocationId,
                    document.TargetWarehouseId,
                    document.TargetLocationId,
                    document.DocumentDate,
                    totalValue,
                    document.RowVersion,
                    positions,
                    history,
                    document.Positions);
            })
            .ToArray();
    }

    private static IEnumerable<WarehouseDocumentHistoryViewModel> BuildHistory(ApiClient.WarehouseDocumentDetailsSnapshot document)
    {
        yield return new WarehouseDocumentHistoryViewModel(
            document.DocumentDate.ToString("HH:mm"),
            "Utworzono dokument",
            "API / SQL");

        foreach (var movement in document.Movements.OrderBy(item => item.MovementDateUtc))
        {
            var direction = movement.QuantityDelta >= 0m ? "Przyjecie" : "Wydanie";
            yield return new WarehouseDocumentHistoryViewModel(
                movement.MovementDateUtc.ToLocalTime().ToString("HH:mm"),
                $"{direction} {movement.ProductCode} {Math.Abs(movement.QuantityDelta):N2}".Replace(",00", string.Empty),
                "API / SQL");
        }

        if (document.PostedAtUtc.HasValue)
        {
            yield return new WarehouseDocumentHistoryViewModel(
                document.PostedAtUtc.Value.ToLocalTime().ToString("HH:mm"),
                "Zaksiegowano dokument",
                document.PostedByUserId.HasValue ? $"Uzytkownik #{document.PostedByUserId.Value}" : "System");
        }

        if (string.Equals(document.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            yield return new WarehouseDocumentHistoryViewModel(
                document.DocumentDate.ToString("dd.MM"),
                "Dokument przeniesiony do archiwum",
                "API / SQL");
        }
    }

    private void ApplyFilters(string? preferredNumber = null)
    {
        var filtered = allDocuments
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.Number.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Counterparty.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Operator.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedTypeFilter == "Wszystkie typy" ||
                 string.Equals(item.Type, SelectedTypeFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedStatusFilter == "Wszystkie statusy" ||
                 string.Equals(item.Status, SelectedStatusFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedWarehouseFilter == "Wszystkie magazyny" ||
                 string.Equals(item.Warehouse, SelectedWarehouseFilter, StringComparison.OrdinalIgnoreCase)) &&
                (!OnlyPendingApproval || item.RequiresApproval) &&
                (ShowArchived || !item.IsArchived))
            .ToArray();

        VisibleDocuments.Clear();
        foreach (var item in filtered)
        {
            VisibleDocuments.Add(item);
        }

        SelectedDocument = VisibleDocuments.FirstOrDefault(item => item.Number == preferredNumber)
                           ?? VisibleDocuments.FirstOrDefault(item => item.Number == SelectedDocument?.Number)
                           ?? VisibleDocuments.FirstOrDefault();

        RefreshSummaryCards(filtered);
    }

    private void RefreshSummaryCards(IEnumerable<WarehouseDocumentRowViewModel> documents)
    {
        var rows = documents.ToArray();
        SummaryCards.Clear();
        SummaryCards.Add(new WarehouseSummaryCardViewModel("Do zatwierdzenia", rows.Count(item => item.RequiresApproval).ToString(), "Dokumenty czekajace na decyzje kierownika zmiany.", "#D97706"));
        SummaryCards.Add(new WarehouseSummaryCardViewModel("W obiegu", rows.Count(item => !item.IsArchived).ToString(), "Dokumenty aktywne w biezacym obiegu operacyjnym.", "#2563EB"));
        SummaryCards.Add(new WarehouseSummaryCardViewModel("Zarchiwizowane", rows.Count(item => item.IsArchived).ToString(), "Dokumenty przeniesione poza aktywny obieg.", "#7B8794"));
        SummaryCards.Add(new WarehouseSummaryCardViewModel("Wartość widoku", $"{rows.Sum(item => item.TotalValue):N0} zl", "Suma wartosci dokumentow z aktualnego filtra.", "#1F8A5B"));
    }

    private void SelectDocument(WarehouseDocumentRowViewModel? document)
    {
        if (document is null)
        {
            return;
        }

        SelectedDocument = document;
    }

    private async Task RunDocumentActionAsync(string? action)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            return;
        }

        try
        {
            switch (action)
            {
                case "Nowy":
                {
                    var snapshot = await store.GetSnapshotAsync();
                    var product = snapshot.ReferenceData.Products.FirstOrDefault();
                    var warehouse = snapshot.ReferenceData.Warehouses.FirstOrDefault();
                    var targetLocation = warehouse is null
                        ? null
                        : snapshot.ReferenceData.Locations.FirstOrDefault(item => item.WarehouseId == warehouse.Id);

                    if (product is null || warehouse is null || targetLocation is null)
                    {
                        throw new InvalidOperationException("Brak produktu, magazynu albo lokalizacji do utworzenia nowego PZ.");
                    }

                    var created = await store.CreateQuickDocumentAsync(
                        "PZ",
                        new WarehouseQuickDocumentRequest(
                            product.Id,
                            1m,
                            1m,
                            null,
                            null,
                            null,
                            warehouse.Id,
                            targetLocation.Id,
                            $"MANUAL-{DateTime.Now:yyyyMMddHHmmss}",
                            "Szybko utworzony szkic PZ z poziomu listy dokumentów.",
                            "Pozycja startowa do dalszej edycji."));

                    IsEditMode = false;
                    LastActionMessage = $"Dodano nowy dokument {created.Number} do bazy SQL.";
                    break;
                }
                case "Edytuj":
                    if (SelectedDocument is null)
                    {
                        return;
                    }

                    IsEditMode = true;
                    LoadEditor();
                    LastActionMessage = $"Wlaczono tryb edycji dla dokumentu {SelectedDocument.Number}.";
                    break;
                case "Eksportuj":
                {
                    if (SelectedDocument is null)
                    {
                        return;
                    }

                    var path = store.ExportDocumentsCsv(
                    [
                        new WarehouseDocumentExportRow(
                            SelectedDocument.Number,
                            SelectedDocument.Type,
                            SelectedDocument.Status,
                            SelectedDocument.Warehouse,
                            SelectedDocument.Counterparty,
                            SelectedDocument.DocumentDate,
                            SelectedDocument.PositionCount.ToString(),
                            SelectedDocument.QuantityLabel,
                            SelectedDocument.ValueLabel)
                    ]);
                    LastActionMessage = $"Wyeksportowano dokument do pliku: {path}";
                    break;
                }
                case "Archiwizuj":
                {
                    if (SelectedDocument is null)
                    {
                        return;
                    }

                    var archived = await store.ArchiveDocumentAsync(SelectedDocument.Id, SelectedDocument.RowVersion);
                    LastActionMessage = $"Dokument {archived.Number} przeniesiono do archiwum.";
                    IsEditMode = false;
                    break;
                }
                case "Usun":
                {
                    if (SelectedDocument is null)
                    {
                        return;
                    }

                    var removedNumber = SelectedDocument.Number;
                    await store.DeleteDocumentAsync(SelectedDocument.Id, SelectedDocument.RowVersion);
                    LastActionMessage = $"Usunieto dokument {removedNumber} z bazy operacyjnej.";
                    IsEditMode = false;
                    break;
                }
            }
        }
        catch (Exception exception)
        {
            LastActionMessage = $"Akcja '{action}' nie powiodła się: {TrimExceptionMessage(exception.Message)}";
        }
    }

    private void LoadEditor()
    {
        if (SelectedDocument is null)
        {
            EditorDocumentDate = string.Empty;
            EditorExternalReference = string.Empty;
            EditorNotes = string.Empty;
            return;
        }

        EditorDocumentDate = SelectedDocument.SourceDocumentDate.ToString("yyyy-MM-dd");
        EditorExternalReference = SelectedDocument.ExternalReference ?? string.Empty;
        EditorNotes = SelectedDocument.Note;
    }

    private void CancelDocumentEdit()
    {
        IsEditMode = false;
        LoadEditor();
        LastActionMessage = "Anulowano edycje dokumentu.";
    }

    private async Task SaveDocumentChangesAsync()
    {
        if (SelectedDocument is null)
        {
            return;
        }

        try
        {
            var documentDate = EditorValueParsers.ParseDate(EditorDocumentDate, "Data dokumentu");
            var request = new WarehouseDocumentUpdateRequest(
                SelectedDocument.Id,
                SelectedDocument.Type,
                documentDate,
                SelectedDocument.ContractorId,
                SelectedDocument.SourceWarehouseId,
                SelectedDocument.SourceLocationId,
                SelectedDocument.TargetWarehouseId,
                SelectedDocument.TargetLocationId,
                string.IsNullOrWhiteSpace(EditorExternalReference) ? null : EditorExternalReference.Trim(),
                string.IsNullOrWhiteSpace(EditorNotes) ? null : EditorNotes.Trim(),
                SelectedDocument.RawPositions,
                SelectedDocument.RowVersion);

            var updated = await store.UpdateDocumentAsync(request);
            IsEditMode = false;
            LastActionMessage = $"Zapisano zmiany dokumentu {updated.Number}.";
        }
        catch (Exception exception)
        {
            LastActionMessage = $"Nie udało się zapisać dokumentu: {TrimExceptionMessage(exception.Message)}";
        }
    }

    private static string ResolveWarehouseLabel(ApiClient.WarehouseDocumentDetailsSnapshot document)
    {
        var source = document.SourceWarehouseCode ?? "-";
        var target = document.TargetWarehouseCode ?? "-";
        return document.Type switch
        {
            "MM" => $"{source} -> {target}",
            "PZ" or "PW" or "INW" => target,
            _ => source
        };
    }

    private static string LocalizeDocumentStatus(string status) => status switch
    {
        "Draft" => "Roboczy",
        "Posted" => "Zaksiegowany",
        "Cancelled" => "Zarchiwizowany",
        _ => status
    };

    private static string ResolveDocumentStateColor(string status) => status switch
    {
        "Draft" => "#D97706",
        "Posted" => "#1F8A5B",
        "Cancelled" => "#7B8794",
        _ => "#2563EB"
    };

    private static string TrimExceptionMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return "Brak szczegółów błędu.";
        }

        return message.Length > 220 ? message[..220] : message;
    }
}

public sealed record WarehouseDocumentRowViewModel(
    int Id,
    string Number,
    string Type,
    string Warehouse,
    string Counterparty,
    string Status,
    string StatusColor,
    string DocumentDate,
    string Operator,
    int PositionCount,
    string QuantityLabel,
    string ValueLabel,
    string BadgeText,
    bool IsArchived,
    bool RequiresApproval,
    string Note,
    string? ExternalReference,
    int? ContractorId,
    int? SourceWarehouseId,
    int? SourceLocationId,
    int? TargetWarehouseId,
    int? TargetLocationId,
    DateTime SourceDocumentDate,
    decimal TotalValue,
    byte[] RowVersion,
    IReadOnlyList<WarehouseDocumentPositionViewModel> Positions,
    IReadOnlyList<WarehouseDocumentHistoryViewModel> History,
    IReadOnlyList<ApiClient.WarehouseDocumentPositionSnapshot> RawPositions);

public sealed record WarehouseDocumentPositionViewModel(
    int Id,
    string Code,
    string Name,
    string Quantity,
    string UnitPrice,
    string Location);

public sealed record WarehouseDocumentHistoryViewModel(
    string Time,
    string Action,
    string Operator);
