using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.Warehouse;

public sealed partial class WarehouseStockWorkspaceViewModel : WorkspaceViewModelBase
{
    private static readonly CultureInfo PolishCulture = CultureInfo.GetCultureInfo("pl-PL");

    private readonly WarehouseWorkspaceStore store;
    private readonly Action<string>? openWorkspace;
    private IReadOnlyList<WarehouseStockRowViewModel> allStockItems = [];

    public WarehouseStockWorkspaceViewModel(WarehouseWorkspaceStore store, Action<string>? openWorkspace = null)
        : base(
            "/warehouse/stock",
            "Magazyn",
            "Stany magazynowe",
            "Sprawdz stan, rezerwacje, produkty ponizej minimum i dokumenty wymagajace reakcji.",
            false)
    {
        this.store = store;
        this.openWorkspace = openWorkspace;
        store.Changed += OnStoreChanged;

        SummaryCards = [];
        AttentionItems = [];
        CategoryFilterOptions = ["Wszystkie kategorie"];
        StatusFilterOptions = ["Wszystkie statusy", "OK", "Niski stan", "Brak", "Zarezerwowane", "Nieaktywne"];
        WarehouseFilterOptions = ["Wszystkie magazyny"];
        VisibleStockItems = [];

        ApplyAttentionActionCommand = new RelayCommand<WarehouseAttentionItemViewModel>(ApplyAttentionAction);
        RunProductActionCommand = new AsyncRelayCommand<string>(RunProductActionAsync);

        _ = LoadAsync();
    }

    public ObservableCollection<WarehouseSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<WarehouseAttentionItemViewModel> AttentionItems { get; }
    public IReadOnlyList<string> CategoryFilterOptions { get; private set; }
    public IReadOnlyList<string> StatusFilterOptions { get; }
    public IReadOnlyList<string> WarehouseFilterOptions { get; private set; }
    public ObservableCollection<WarehouseStockRowViewModel> VisibleStockItems { get; }
    public IRelayCommand<WarehouseAttentionItemViewModel> ApplyAttentionActionCommand { get; }
    public IAsyncRelayCommand<string> RunProductActionCommand { get; }

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedWarehouseFilter = "Wszystkie magazyny";

    [ObservableProperty]
    private string selectedCategoryFilter = "Wszystkie kategorie";

    [ObservableProperty]
    private string selectedStatusFilter = "Wszystkie statusy";

    [ObservableProperty]
    private bool onlyBelowMinimum;

    [ObservableProperty]
    private bool onlyWithReservation;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz produkt albo kliknij alert, aby zawęzić tabelę.";

    [ObservableProperty]
    private WarehouseStockRowViewModel? selectedStockItem;

    public string SelectedProductName => SelectedStockItem?.Name ?? "Brak zaznaczonego produktu";
    public string SelectedProductCode => SelectedStockItem?.Code ?? "-";
    public string SelectedProductQuantity => SelectedStockItem is null ? "-" : $"{FormatQuantity(SelectedStockItem.OnHand)} {SelectedStockItem.Unit}";
    public string SelectedProductMinimum => SelectedStockItem is null ? "-" : $"{FormatQuantity(SelectedStockItem.MinimumLevel)} {SelectedStockItem.Unit}";
    public string SelectedProductReserved => SelectedStockItem is null ? "-" : $"{FormatQuantity(SelectedStockItem.Reserved)} {SelectedStockItem.Unit}";
    public string SelectedProductAvailable => SelectedStockItem is null ? "-" : $"{FormatQuantity(SelectedStockItem.Available)} {SelectedStockItem.Unit}";
    public string SelectedProductStatus => SelectedStockItem?.Status ?? "-";
    public string SelectedProductStatusColor => SelectedStockItem?.StatusColor ?? "#7B8794";
    public string SelectedProductLocation => SelectedStockItem is null ? "-" : $"{SelectedStockItem.Warehouse} / {SelectedStockItem.Location}";
    public string SelectedLastMovement => SelectedStockItem?.LastMovement ?? "-";
    public string SelectedRestockHint => SelectedStockItem?.RestockHint ?? "Brak danych dla wybranej pozycji.";
    public string SelectedSupplier => SelectedStockItem?.Supplier ?? "Brak dostawcy";
    public IReadOnlyList<WarehouseStockIssueViewModel> SelectedIssues => SelectedStockItem?.Issues ?? [];
    public IReadOnlyList<WarehouseMovementLineViewModel> SelectedMovements => SelectedStockItem?.Movements ?? [];
    public IReadOnlyList<WarehousePendingDocumentViewModel> SelectedPendingDocuments => SelectedStockItem?.RelatedDocuments ?? [];

    partial void OnSearchTextChanged(string value) => ApplyFilters();
    partial void OnSelectedWarehouseFilterChanged(string value) => ApplyFilters();
    partial void OnSelectedCategoryFilterChanged(string value) => ApplyFilters();
    partial void OnSelectedStatusFilterChanged(string value) => ApplyFilters();
    partial void OnOnlyBelowMinimumChanged(bool value) => ApplyFilters();
    partial void OnOnlyWithReservationChanged(bool value) => ApplyFilters();

    partial void OnSelectedStockItemChanged(WarehouseStockRowViewModel? value)
    {
        OnPropertyChanged(nameof(SelectedProductName));
        OnPropertyChanged(nameof(SelectedProductCode));
        OnPropertyChanged(nameof(SelectedProductQuantity));
        OnPropertyChanged(nameof(SelectedProductMinimum));
        OnPropertyChanged(nameof(SelectedProductReserved));
        OnPropertyChanged(nameof(SelectedProductAvailable));
        OnPropertyChanged(nameof(SelectedProductStatus));
        OnPropertyChanged(nameof(SelectedProductStatusColor));
        OnPropertyChanged(nameof(SelectedProductLocation));
        OnPropertyChanged(nameof(SelectedLastMovement));
        OnPropertyChanged(nameof(SelectedRestockHint));
        OnPropertyChanged(nameof(SelectedSupplier));
        OnPropertyChanged(nameof(SelectedIssues));
        OnPropertyChanged(nameof(SelectedMovements));
        OnPropertyChanged(nameof(SelectedPendingDocuments));
    }

    private void OnStoreChanged(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() => _ = LoadAsync());
    }

    private async Task LoadAsync()
    {
        var preferredCode = SelectedStockItem?.Code;

        try
        {
            var snapshot = await store.GetSnapshotAsync();
            allStockItems = BuildRows(snapshot);

            CategoryFilterOptions = ["Wszystkie kategorie", .. allStockItems
                .Select(item => item.Category)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item)];
            WarehouseFilterOptions = ["Wszystkie magazyny", .. allStockItems
                .Select(item => item.Warehouse)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item)];

            OnPropertyChanged(nameof(CategoryFilterOptions));
            OnPropertyChanged(nameof(WarehouseFilterOptions));

            RefreshSummaryCards(allStockItems, snapshot);
            RefreshAttentionItems(allStockItems, snapshot);
            ApplyFilters(preferredCode);

            LastActionMessage = allStockItems.Count == 0
                ? "Brak stanów magazynowych w bazie SQL. Dodaj albo zaksięguj dokument, aby zobaczyć pozycje."
                : "Dane magazynowe zostały pobrane z API i bazy SQL.";
        }
        catch (Exception exception)
        {
            allStockItems = [];
            VisibleStockItems.Clear();
            SummaryCards.Clear();
            AttentionItems.Clear();
            SelectedStockItem = null;
            LastActionMessage = $"Nie udało się pobrać stanów z API/SQL: {TrimExceptionMessage(exception.Message)}";
        }
    }

    private IReadOnlyList<WarehouseStockRowViewModel> BuildRows(WarehouseWorkspaceSnapshot snapshot)
    {
        var productsById = snapshot.ReferenceData.Products.ToDictionary(item => item.Id);
        var documentById = snapshot.Documents.ToDictionary(item => item.Id);

        return snapshot.StockItems
            .OrderBy(item => item.WarehouseName)
            .ThenBy(item => item.ProductName)
            .Select(stockItem =>
            {
                var product = productsById[stockItem.ProductId];
                var relatedDocuments = snapshot.Documents
                    .Where(document => document.Positions.Any(position => position.ProductId == stockItem.ProductId))
                    .Where(document => DocumentTouchesStock(document, stockItem))
                    .OrderByDescending(document => document.DocumentDate)
                    .ToArray();

                var reserved = snapshot.Documents
                    .Where(document => string.Equals(document.Status, "Draft", StringComparison.OrdinalIgnoreCase))
                    .Where(document => document.SourceWarehouseId == stockItem.WarehouseId)
                    .SelectMany(document => document.Positions
                        .Where(position => position.ProductId == stockItem.ProductId)
                        .Where(position => position.SourceLocationId == stockItem.WarehouseLocationId || position.SourceLocationId is null)
                        .Select(position => position.Quantity))
                    .DefaultIfEmpty(0m)
                    .Sum();

                var available = Math.Max(stockItem.QuantityOnHand - reserved, 0m);
                var minimum = product.MinimumStockLevel;
                var status = ResolveStatus(product.IsActive, stockItem.QuantityOnHand, available, reserved, minimum);
                var statusColor = ResolveStatusColor(status);
                var latestMovement = relatedDocuments
                    .SelectMany(document => document.Movements
                        .Where(movement =>
                            movement.ProductId == stockItem.ProductId &&
                            movement.WarehouseId == stockItem.WarehouseId &&
                            movement.WarehouseLocationId == stockItem.WarehouseLocationId)
                        .Select(movement => new { document.Number, movement.MovementDateUtc, movement.UnitPrice }))
                    .OrderByDescending(item => item.MovementDateUtc)
                    .FirstOrDefault();

                var supplierDocument = relatedDocuments
                    .Where(document => document.ContractorId.HasValue)
                    .Where(document => string.Equals(document.Type, "PZ", StringComparison.OrdinalIgnoreCase) || string.Equals(document.Type, "PW", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(document => document.DocumentDate)
                    .FirstOrDefault();

                var movements = relatedDocuments
                    .SelectMany(document => document.Movements
                        .Where(movement =>
                            movement.ProductId == stockItem.ProductId &&
                            movement.WarehouseId == stockItem.WarehouseId &&
                            movement.WarehouseLocationId == stockItem.WarehouseLocationId)
                        .Select(movement => new WarehouseMovementLineViewModel(
                            movement.MovementDateUtc.ToLocalTime().ToString("HH:mm"),
                            documentById[movement.Id == 0 ? document.Id : document.Id].Number,
                            movement.QuantityDelta >= 0m ? "Przyjecie" : "Wydanie",
                            $"{FormatQuantity(Math.Abs(movement.QuantityDelta))} {stockItem.UnitOfMeasureSymbol}",
                            "API / SQL")))
                    .OrderByDescending(item => item.Time)
                    .Take(6)
                    .ToArray();

                var pendingDocuments = relatedDocuments
                    .Take(6)
                    .Select(document => new WarehousePendingDocumentViewModel(
                        document.Number,
                        document.Type,
                        document.ContractorName ?? ResolveWarehouseLabel(document),
                        GetAgeLabel(document.DocumentDate),
                        LocalizeDocumentStatus(document.Status),
                        ResolveDocumentStateColor(document.Status)))
                    .ToArray();

                var issues = BuildIssues(stockItem, minimum, available, reserved, product.IsActive, relatedDocuments);
                var restockHint = BuildRestockHint(status, stockItem.ProductName, minimum, available, reserved);

                return new WarehouseStockRowViewModel(
                    stockItem.ProductId,
                    stockItem.WarehouseId,
                    stockItem.WarehouseLocationId,
                    stockItem.CodeOrProductCode(),
                    stockItem.ProductName,
                    product.ProductCategoryName,
                    stockItem.WarehouseName,
                    stockItem.WarehouseLocationCode ?? "Brak lokalizacji",
                    stockItem.QuantityOnHand,
                    stockItem.UnitOfMeasureSymbol,
                    minimum,
                    available,
                    reserved,
                    status,
                    statusColor,
                    latestMovement is null
                        ? "Brak ruchu"
                        : $"{latestMovement.Number} · {latestMovement.MovementDateUtc.ToLocalTime():HH:mm}",
                    supplierDocument?.ContractorName ?? "Brak dostawcy",
                    restockHint,
                    issues,
                    movements,
                    pendingDocuments,
                    supplierDocument?.ContractorId,
                    latestMovement?.UnitPrice);
            })
            .ToArray();
    }

    private static bool DocumentTouchesStock(ApiClient.WarehouseDocumentDetailsSnapshot document, ApiClient.WarehouseStockItemSnapshot stockItem)
    {
        return (document.SourceWarehouseId == stockItem.WarehouseId || document.TargetWarehouseId == stockItem.WarehouseId)
               && document.Positions.Any(position => position.ProductId == stockItem.ProductId);
    }

    private static IReadOnlyList<WarehouseStockIssueViewModel> BuildIssues(
        ApiClient.WarehouseStockItemSnapshot stockItem,
        decimal minimum,
        decimal available,
        decimal reserved,
        bool isActive,
        IReadOnlyList<ApiClient.WarehouseDocumentDetailsSnapshot> relatedDocuments)
    {
        var items = new List<WarehouseStockIssueViewModel>();

        if (available < minimum)
        {
            items.Add(new WarehouseStockIssueViewModel(
                "Poniżej minimum",
                $"Dostępne {FormatQuantity(available)} {stockItem.UnitOfMeasureSymbol} przy minimum {FormatQuantity(minimum)} {stockItem.UnitOfMeasureSymbol}.",
                "Sprawdz",
                "#D97706"));
        }

        if (reserved > 0m)
        {
            items.Add(new WarehouseStockIssueViewModel(
                "Rezerwacja aktywna",
                $"{FormatQuantity(reserved)} {stockItem.UnitOfMeasureSymbol} jest zarezerwowane w otwartych dokumentach.",
                "Sprawdz",
                "#2563EB"));
        }

        if (stockItem.WarehouseLocationId is null)
        {
            items.Add(new WarehouseStockIssueViewModel(
                "Brak lokalizacji",
                "Pozycja nie ma przypisanego miejsca składowania.",
                "Uzupelnij",
                "#2563EB"));
        }

        if (!isActive)
        {
            items.Add(new WarehouseStockIssueViewModel(
                "Pozycja nieaktywna",
                "Kartoteka jest wyłączona z bieżącego obiegu, ale nadal pojawia się w stanie.",
                "Sprawdz",
                "#7B8794"));
        }

        if (relatedDocuments.Any(document => string.Equals(document.Status, "Draft", StringComparison.OrdinalIgnoreCase)))
        {
            items.Add(new WarehouseStockIssueViewModel(
                "Otwarte dokumenty",
                "Dla tej pozycji istnieją dokumenty robocze oczekujące na dalszą akcję.",
                "Zatwierdz",
                "#D14343"));
        }

        return items;
    }

    private void RefreshSummaryCards(IReadOnlyList<WarehouseStockRowViewModel> rows, WarehouseWorkspaceSnapshot snapshot)
    {
        var latestMovementAt = snapshot.Documents
            .SelectMany(document => document.Movements)
            .OrderByDescending(movement => movement.MovementDateUtc)
            .Select(movement => movement.MovementDateUtc.ToLocalTime().ToString("HH:mm"))
            .FirstOrDefault() ?? "-";

        var stockValue = snapshot.Documents
            .SelectMany(document => document.Movements)
            .Where(movement => movement.QuantityDelta > 0m && movement.UnitPrice.HasValue)
            .GroupBy(movement => movement.ProductId)
            .ToDictionary(
                group => group.Key,
                group => group.OrderByDescending(item => item.MovementDateUtc).First().UnitPrice ?? 0m);

        var totalValue = rows.Sum(item =>
            item.OnHand * (stockValue.TryGetValue(item.ProductId, out var unitPrice) ? unitPrice : 0m));

        var occupiedLocations = rows.Count(item => item.LocationId.HasValue);
        var totalLocations = Math.Max(snapshot.ReferenceData.Locations.Count(item => item.IsActive), 1);
        var occupiedPercent = Math.Round((decimal)occupiedLocations / totalLocations * 100m, 0);

        SummaryCards.Clear();
        SummaryCards.Add(new WarehouseSummaryCardViewModel("Pozycji aktywnych", rows.Count(item => item.Status != "Nieaktywne").ToString(), "Pozycje pobrane z bazy SQL dla aktualnych stanów.", "#2563EB"));
        SummaryCards.Add(new WarehouseSummaryCardViewModel("Poniżej minimum", rows.Count(item => item.Available < item.MinimumLevel).ToString(), "Pozycje wymagajace uzupelnienia albo przesuniecia.", "#D14343"));
        SummaryCards.Add(new WarehouseSummaryCardViewModel("Do zatwierdzenia", snapshot.Documents.Count(item => string.Equals(item.Status, "Draft", StringComparison.OrdinalIgnoreCase)).ToString(), "Dokumenty robocze oczekujace na dalsza decyzje.", "#D97706"));
        SummaryCards.Add(new WarehouseSummaryCardViewModel("Wartość magazynu", $"{totalValue:N0} zl", "Wartosc wyliczona z ostatnich cen ruchow w SQL.", "#2563EB"));
        SummaryCards.Add(new WarehouseSummaryCardViewModel("Ostatni ruch", latestMovementAt, "Ostatnie zaksiegowane przesuniecie lub przyjecie.", "#1F8A5B"));
        SummaryCards.Add(new WarehouseSummaryCardViewModel("Lokalizacje zajęte", $"{occupiedPercent:0}%", "Wykorzystanie aktywnych lokalizacji z danych referencyjnych.", "#2563EB"));
    }

    private void RefreshAttentionItems(IReadOnlyList<WarehouseStockRowViewModel> rows, WarehouseWorkspaceSnapshot snapshot)
    {
        AttentionItems.Clear();

        var lowCount = rows.Count(item => item.Available < item.MinimumLevel);
        if (lowCount > 0)
        {
            AttentionItems.Add(new WarehouseAttentionItemViewModel(
                $"{lowCount} pozycji poniżej minimum",
                "Pokaz indeksy, ktore wymagaja uzupelnienia jeszcze na tej zmianie.",
                "Sprawdz",
                "#D14343",
                "Wszystkie magazyny",
                "Wszystkie kategorie",
                "Niski stan",
                true,
                false,
                null));
        }

        var draftCount = snapshot.Documents.Count(item => string.Equals(item.Status, "Draft", StringComparison.OrdinalIgnoreCase));
        if (draftCount > 0)
        {
            AttentionItems.Add(new WarehouseAttentionItemViewModel(
                $"{draftCount} dokumenty robocze",
                "Otwarte PZ, WZ, MM albo RW nadal czekaja na dalsza akcje.",
                "Zatwierdz",
                "#D97706",
                "Wszystkie magazyny",
                "Wszystkie kategorie",
                "Wszystkie statusy",
                false,
                false,
                null));
        }

        var noLocationCount = rows.Count(item => item.LocationId is null);
        if (noLocationCount > 0)
        {
            AttentionItems.Add(new WarehouseAttentionItemViewModel(
                $"{noLocationCount} pozycje bez lokalizacji",
                "Wyfiltruj pozycje bez prawidlowego miejsca skladowania.",
                "Uzupelnij",
                "#2563EB",
                "Wszystkie magazyny",
                "Wszystkie kategorie",
                "Wszystkie statusy",
                false,
                false,
                "Brak lokalizacji"));
        }

        var blockedCount = rows.Count(item => item.Available <= 0m && item.Reserved > 0m);
        if (blockedCount > 0)
        {
            AttentionItems.Add(new WarehouseAttentionItemViewModel(
                $"{blockedCount} pozycje z blokada dostepnosci",
                "Pozycje maja rezerwacje, ale brak zapasu do dalszego wydania.",
                "Sprawdz",
                "#D14343",
                "Wszystkie magazyny",
                "Wszystkie kategorie",
                "Brak",
                false,
                true,
                null));
        }
    }

    private void ApplyFilters(string? preferredCode = null)
    {
        var filtered = allStockItems
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Location.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedWarehouseFilter == "Wszystkie magazyny" ||
                 string.Equals(item.Warehouse, SelectedWarehouseFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedCategoryFilter == "Wszystkie kategorie" ||
                 string.Equals(item.Category, SelectedCategoryFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedStatusFilter == "Wszystkie statusy" ||
                 string.Equals(item.Status, SelectedStatusFilter, StringComparison.OrdinalIgnoreCase)) &&
                (!OnlyBelowMinimum || item.Available < item.MinimumLevel) &&
                (!OnlyWithReservation || item.Reserved > 0m))
            .ToArray();

        VisibleStockItems.Clear();
        foreach (var item in filtered)
        {
            VisibleStockItems.Add(item);
        }

        SelectedStockItem = VisibleStockItems.FirstOrDefault(item => item.Code == preferredCode)
                            ?? VisibleStockItems.FirstOrDefault(item => item.Code == SelectedStockItem?.Code)
                            ?? VisibleStockItems.FirstOrDefault();
    }

    private void ApplyAttentionAction(WarehouseAttentionItemViewModel? item)
    {
        if (item is null)
        {
            return;
        }

        SelectedWarehouseFilter = item.WarehouseFilter;
        SelectedCategoryFilter = item.CategoryFilter;
        SelectedStatusFilter = item.StatusFilter;
        OnlyBelowMinimum = item.OnlyBelowMinimum;
        OnlyWithReservation = item.OnlyWithReservation;
        SearchText = item.SearchText ?? string.Empty;
        ApplyFilters();
        LastActionMessage = $"Zastosowano filtr: {item.Title}.";
    }

    private async Task RunProductActionAsync(string? action)
    {
        if (SelectedStockItem is null || string.IsNullOrWhiteSpace(action))
        {
            return;
        }

        try
        {
            switch (action)
            {
                case "PZ":
                {
                    var quantity = Math.Max(SelectedStockItem.MinimumLevel - SelectedStockItem.Available, 1m);
                    var document = await store.CreateQuickDocumentAsync("PZ", new WarehouseQuickDocumentRequest(
                        SelectedStockItem.ProductId,
                        quantity,
                        SelectedStockItem.LastKnownUnitPrice,
                        SelectedStockItem.SupplierId,
                        null,
                        null,
                        SelectedStockItem.WarehouseId,
                        SelectedStockItem.LocationId,
                        $"AUTO-PZ-{SelectedStockItem.Code}",
                        "Roboczy dokument PZ utworzony z poziomu stanu magazynowego.",
                        "Szybkie uzupelnienie z widoku stanów."), CancellationToken.None);
                    LastActionMessage = $"Utworzono dokument {document.Number} dla produktu {SelectedStockItem.Code}.";
                    openWorkspace?.Invoke("/warehouse/documents");
                    break;
                }
                case "WZ":
                {
                    var document = await store.CreateQuickDocumentAsync("WZ", new WarehouseQuickDocumentRequest(
                        SelectedStockItem.ProductId,
                        Math.Max(1m, SelectedStockItem.Available > 0m ? 1m : SelectedStockItem.Reserved > 0m ? SelectedStockItem.Reserved : 1m),
                        SelectedStockItem.LastKnownUnitPrice,
                        null,
                        SelectedStockItem.WarehouseId,
                        SelectedStockItem.LocationId,
                        null,
                        null,
                        $"AUTO-WZ-{SelectedStockItem.Code}",
                        "Roboczy dokument WZ utworzony z poziomu stanu magazynowego.",
                        "Szybkie wydanie z widoku stanów."), CancellationToken.None);
                    LastActionMessage = $"Utworzono dokument {document.Number} dla produktu {SelectedStockItem.Code}.";
                    openWorkspace?.Invoke("/warehouse/documents");
                    break;
                }
                case "Historia":
                    openWorkspace?.Invoke("/warehouse/documents");
                    LastActionMessage = $"Otworzono listę dokumentów dla dalszej analizy historii produktu {SelectedStockItem.Code}.";
                    break;
                case "Edytuj":
                    openWorkspace?.Invoke("/warehouse/goods");
                    LastActionMessage = $"Przełączono do kartotek towarowych, aby edytować produkt {SelectedStockItem.Code}.";
                    break;
                default:
                    LastActionMessage = $"Uruchomiono akcje '{action}' dla produktu {SelectedStockItem.Code}.";
                    break;
            }
        }
        catch (Exception exception)
        {
            LastActionMessage = $"Akcja '{action}' nie powiodła się: {TrimExceptionMessage(exception.Message)}";
        }
    }

    private static string ResolveStatus(bool isActive, decimal onHand, decimal available, decimal reserved, decimal minimum)
    {
        if (!isActive)
        {
            return "Nieaktywne";
        }

        if (onHand <= 0m || available <= 0m)
        {
            return "Brak";
        }

        if (reserved > 0m)
        {
            return "Zarezerwowane";
        }

        if (available < minimum)
        {
            return "Niski stan";
        }

        return "OK";
    }

    private static string ResolveStatusColor(string status) => status switch
    {
        "OK" => "#1F8A5B",
        "Niski stan" => "#D97706",
        "Brak" => "#D14343",
        "Zarezerwowane" => "#2563EB",
        "Nieaktywne" => "#7B8794",
        _ => "#7B8794"
    };

    private static string ResolveDocumentStateColor(string status) => status switch
    {
        "Draft" => "#D97706",
        "Posted" => "#1F8A5B",
        "Cancelled" => "#7B8794",
        _ => "#2563EB"
    };

    private static string LocalizeDocumentStatus(string status) => status switch
    {
        "Draft" => "Roboczy",
        "Posted" => "Zaksiegowany",
        "Cancelled" => "Zarchiwizowany",
        _ => status
    };

    private static string BuildRestockHint(string status, string productName, decimal minimum, decimal available, decimal reserved)
    {
        return status switch
        {
            "Brak" => $"Brak dostepnego zapasu dla '{productName}'. Najpierw uzupelnij stan albo zdejmij rezerwacje.",
            "Niski stan" => $"Dostepne {FormatQuantity(available)} przy minimum {FormatQuantity(minimum)}. Zaplanuj szybkie uzupelnienie.",
            "Zarezerwowane" => $"Czesc stanu jest zablokowana w dokumentach roboczych ({FormatQuantity(reserved)}).",
            "Nieaktywne" => "Kartoteka jest nieaktywna i wymaga weryfikacji przed dalszym obiegiem.",
            _ => "Stan bezpieczny. Pozycja jest gotowa do normalnej pracy operacyjnej."
        };
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

    private static string GetAgeLabel(DateTime documentDate)
    {
        var span = DateTime.Now - documentDate;
        if (span.TotalMinutes < 60)
        {
            return $"od {Math.Max(1, (int)Math.Round(span.TotalMinutes))} min";
        }

        if (span.TotalHours < 24)
        {
            return $"od {(int)Math.Round(span.TotalHours)} h";
        }

        return documentDate.ToString("dd.MM.yyyy HH:mm");
    }

    private static string TrimExceptionMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return "Brak szczegółów błędu.";
        }

        return message.Length > 220 ? message[..220] : message;
    }

    private static string FormatQuantity(decimal value) => value.ToString("0.##", PolishCulture);
}

public sealed record WarehouseSummaryCardViewModel(string Label, string Value, string Caption, string AccentColor);

public sealed record WarehouseAttentionItemViewModel(
    string Title,
    string Detail,
    string BadgeText,
    string BadgeColor,
    string WarehouseFilter,
    string CategoryFilter,
    string StatusFilter,
    bool OnlyBelowMinimum,
    bool OnlyWithReservation,
    string? SearchText);

public sealed record WarehousePendingDocumentViewModel(
    string Number,
    string Type,
    string Counterparty,
    string Age,
    string State,
    string StateColor);

public sealed record WarehouseMovementLineViewModel(
    string Time,
    string Document,
    string Direction,
    string Quantity,
    string Operator);

public sealed record WarehouseStockIssueViewModel(
    string Title,
    string Detail,
    string State,
    string StateColor);

public sealed record WarehouseStockRowViewModel(
    int ProductId,
    int WarehouseId,
    int? LocationId,
    string Code,
    string Name,
    string Category,
    string Warehouse,
    string Location,
    decimal OnHand,
    string Unit,
    decimal MinimumLevel,
    decimal Available,
    decimal Reserved,
    string Status,
    string StatusColor,
    string LastMovement,
    string Supplier,
    string RestockHint,
    IReadOnlyList<WarehouseStockIssueViewModel> Issues,
    IReadOnlyList<WarehouseMovementLineViewModel> Movements,
    IReadOnlyList<WarehousePendingDocumentViewModel> RelatedDocuments,
    int? SupplierId,
    decimal? LastKnownUnitPrice);

file static class WarehouseStockSnapshotExtensions
{
    public static string CodeOrProductCode(this ApiClient.WarehouseStockItemSnapshot stockItem) => stockItem.ProductCode;
}
