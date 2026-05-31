using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.Warehouse;

public sealed partial class WarehouseGoodsWorkspaceViewModel : WorkspaceViewModelBase
{
    private static readonly CultureInfo PolishCulture = CultureInfo.GetCultureInfo("pl-PL");

    private readonly WarehouseWorkspaceStore store;
    private List<WarehouseGoodsRowViewModel> allGoods = [];
    private List<WarehouseEditorOptionViewModel> categoryEditorOptions = [];
    private List<WarehouseEditorOptionViewModel> unitEditorOptions = [];

    public WarehouseGoodsWorkspaceViewModel(WarehouseWorkspaceStore store)
        : base(
            "/warehouse/goods",
            "Magazyn",
            "Kartoteki towarowe",
            "Indeksy, kategorie, jednostki miary i statusy towarow do utrzymania przez magazyn.",
            false)
    {
        this.store = store;
        store.Changed += OnStoreChanged;

        SummaryCards = [];
        VisibleGoods = [];
        CategoryFilterOptions = ["Wszystkie kategorie"];
        StatusFilterOptions = ["Wszystkie statusy", "Aktywny", "Poniżej minimum", "Brak", "Zarezerwowany", "Nieaktywne"];
        UnitFilterOptions = ["Wszystkie jednostki"];

        RunGoodsActionCommand = new AsyncRelayCommand<string>(RunGoodsActionAsync);
        SelectGoodsCommand = new RelayCommand<WarehouseGoodsRowViewModel>(SelectGoods);
        SaveGoodsChangesCommand = new AsyncRelayCommand(SaveGoodsChangesAsync, () => IsEditMode);
        CancelGoodsEditCommand = new RelayCommand(CancelGoodsEdit);

        _ = LoadAsync();
    }

    public ObservableCollection<WarehouseSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<WarehouseGoodsRowViewModel> VisibleGoods { get; }
    public IReadOnlyList<string> CategoryFilterOptions { get; private set; }
    public IReadOnlyList<string> StatusFilterOptions { get; }
    public IReadOnlyList<string> UnitFilterOptions { get; private set; }
    public IReadOnlyList<WarehouseEditorOptionViewModel> CategoryEditorOptions => categoryEditorOptions;
    public IReadOnlyList<WarehouseEditorOptionViewModel> UnitEditorOptions => unitEditorOptions;
    public IAsyncRelayCommand<string> RunGoodsActionCommand { get; }
    public IRelayCommand<WarehouseGoodsRowViewModel> SelectGoodsCommand { get; }
    public IAsyncRelayCommand SaveGoodsChangesCommand { get; }
    public IRelayCommand CancelGoodsEditCommand { get; }

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedCategoryFilter = "Wszystkie kategorie";

    [ObservableProperty]
    private string selectedStatusFilter = "Wszystkie statusy";

    [ObservableProperty]
    private string selectedUnitFilter = "Wszystkie jednostki";

    [ObservableProperty]
    private bool onlyBelowMinimum;

    [ObservableProperty]
    private bool showArchived;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz kartoteke, aby edytowac dane podstawowe albo uruchomic eksport.";

    [ObservableProperty]
    private WarehouseGoodsRowViewModel? selectedGoods;

    [ObservableProperty]
    private bool isEditMode;

    [ObservableProperty]
    private bool isCreatingGoods;

    [ObservableProperty]
    private string editorCode = string.Empty;

    [ObservableProperty]
    private string editorName = string.Empty;

    [ObservableProperty]
    private string editorSku = string.Empty;

    [ObservableProperty]
    private string editorMinimum = "0";

    [ObservableProperty]
    private bool editorIsActive = true;

    [ObservableProperty]
    private WarehouseEditorOptionViewModel? selectedCategoryOption;

    [ObservableProperty]
    private WarehouseEditorOptionViewModel? selectedUnitOption;

    public bool HasSelectedGoods => SelectedGoods is not null;
    public string SelectedGoodsName => SelectedGoods?.Name ?? "-";
    public string SelectedGoodsCode => SelectedGoods?.Code ?? "-";
    public string SelectedGoodsCategory => SelectedGoods?.Category ?? "-";
    public string SelectedGoodsUnit => SelectedGoods?.Unit ?? "-";
    public string SelectedGoodsSupplier => SelectedGoods?.Supplier ?? "-";
    public string SelectedGoodsStatus => SelectedGoods?.Status ?? "-";
    public string SelectedGoodsStatusColor => SelectedGoods?.StatusColor ?? "#7B8794";
    public string SelectedGoodsOnHand => SelectedGoods is null ? "-" : $"{FormatQuantity(SelectedGoods.OnHand)} {SelectedGoods.Unit}";
    public string SelectedGoodsMinimum => SelectedGoods is null ? "-" : $"{FormatQuantity(SelectedGoods.MinimumLevel)} {SelectedGoods.Unit}";
    public string SelectedGoodsBarcode => SelectedGoods?.Barcode ?? "-";
    public string SelectedGoodsPreferredWarehouse => SelectedGoods?.PreferredWarehouse ?? "-";
    public string SelectedGoodsLeadTime => SelectedGoods?.LeadTime ?? "-";
    public string SelectedGoodsReorderQuantity => SelectedGoods?.ReorderQuantity ?? "-";
    public string SelectedGoodsLastUpdated => SelectedGoods?.LastUpdated ?? "-";
    public string SelectedGoodsNote => SelectedGoods?.Note ?? "Brak notatki dla wybranej kartoteki.";
    public IReadOnlyList<WarehouseGoodsRelatedDocumentViewModel> SelectedGoodsDocuments => SelectedGoods?.RelatedDocuments ?? [];

    partial void OnSearchTextChanged(string value) => ApplyFilters();
    partial void OnSelectedCategoryFilterChanged(string value) => ApplyFilters();
    partial void OnSelectedStatusFilterChanged(string value) => ApplyFilters();
    partial void OnSelectedUnitFilterChanged(string value) => ApplyFilters();
    partial void OnOnlyBelowMinimumChanged(bool value) => ApplyFilters();
    partial void OnShowArchivedChanged(bool value) => ApplyFilters();
    partial void OnIsEditModeChanged(bool value) => SaveGoodsChangesCommand.NotifyCanExecuteChanged();

    partial void OnSelectedGoodsChanged(WarehouseGoodsRowViewModel? value)
    {
        LoadEditor();
        OnPropertyChanged(nameof(HasSelectedGoods));
        OnPropertyChanged(nameof(SelectedGoodsName));
        OnPropertyChanged(nameof(SelectedGoodsCode));
        OnPropertyChanged(nameof(SelectedGoodsCategory));
        OnPropertyChanged(nameof(SelectedGoodsUnit));
        OnPropertyChanged(nameof(SelectedGoodsSupplier));
        OnPropertyChanged(nameof(SelectedGoodsStatus));
        OnPropertyChanged(nameof(SelectedGoodsStatusColor));
        OnPropertyChanged(nameof(SelectedGoodsOnHand));
        OnPropertyChanged(nameof(SelectedGoodsMinimum));
        OnPropertyChanged(nameof(SelectedGoodsBarcode));
        OnPropertyChanged(nameof(SelectedGoodsPreferredWarehouse));
        OnPropertyChanged(nameof(SelectedGoodsLeadTime));
        OnPropertyChanged(nameof(SelectedGoodsReorderQuantity));
        OnPropertyChanged(nameof(SelectedGoodsLastUpdated));
        OnPropertyChanged(nameof(SelectedGoodsNote));
        OnPropertyChanged(nameof(SelectedGoodsDocuments));
        SaveGoodsChangesCommand.NotifyCanExecuteChanged();
    }

    private void OnStoreChanged(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() => _ = LoadAsync());
    }

    private async Task LoadAsync()
    {
        var preferredCode = SelectedGoods?.Code;

        try
        {
            var snapshot = await store.GetSnapshotAsync();
            allGoods = BuildRows(snapshot).ToList();
            categoryEditorOptions = snapshot.ReferenceData.Categories
                .OrderBy(item => item.Name)
                .Select(item => new WarehouseEditorOptionViewModel(item.Id, $"{item.Code} · {item.Name}"))
                .ToList();
            unitEditorOptions = snapshot.ReferenceData.UnitsOfMeasure
                .OrderBy(item => item.Name)
                .Select(item => new WarehouseEditorOptionViewModel(item.Id, $"{item.Symbol} · {item.Name}"))
                .ToList();
            CategoryFilterOptions = ["Wszystkie kategorie", .. allGoods.Select(item => item.Category).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(item => item)];
            UnitFilterOptions = ["Wszystkie jednostki", .. allGoods.Select(item => item.Unit).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(item => item)];
            OnPropertyChanged(nameof(CategoryFilterOptions));
            OnPropertyChanged(nameof(UnitFilterOptions));
            OnPropertyChanged(nameof(CategoryEditorOptions));
            OnPropertyChanged(nameof(UnitEditorOptions));

            RefreshSummaryCards(allGoods);
            ApplyFilters(preferredCode);
            LastActionMessage = allGoods.Count == 0
                ? "Brak kartotek produktowych w bazie SQL."
                : "Kartoteki towarowe sa pobrane z API i bazy SQL.";
        }
        catch (Exception exception)
        {
            allGoods = [];
            VisibleGoods.Clear();
            SummaryCards.Clear();
            SelectedGoods = null;
            LastActionMessage = $"Nie udało się pobrać kartotek z API/SQL: {TrimExceptionMessage(exception.Message)}";
        }
    }

    private static IReadOnlyList<WarehouseGoodsRowViewModel> BuildRows(WarehouseWorkspaceSnapshot snapshot)
    {
        var stockByProduct = snapshot.StockItems
            .GroupBy(item => item.ProductId)
            .ToDictionary(group => group.Key, group => group.ToArray());

        return snapshot.ReferenceData.Products
            .OrderBy(item => item.Name)
            .Select(product =>
            {
                var stockItems = stockByProduct.TryGetValue(product.Id, out var items) ? items : [];
                var onHand = stockItems.Sum(item => item.QuantityOnHand);
                var preferredStock = stockItems.OrderByDescending(item => item.QuantityOnHand).FirstOrDefault();
                var relatedDocuments = snapshot.Documents
                    .Where(document => document.Positions.Any(position => position.ProductId == product.Id))
                    .OrderByDescending(document => document.DocumentDate)
                    .ToArray();
                var reserved = snapshot.Documents
                    .Where(document => string.Equals(document.Status, "Draft", StringComparison.OrdinalIgnoreCase))
                    .SelectMany(document => document.Positions.Where(position => position.ProductId == product.Id).Select(position => position.Quantity))
                    .DefaultIfEmpty(0m)
                    .Sum();
                var available = Math.Max(onHand - reserved, 0m);
                var status = ResolveStatus(product.IsActive, onHand, available, reserved, product.MinimumStockLevel);
                var supplier = relatedDocuments
                    .Where(document => !string.IsNullOrWhiteSpace(document.ContractorName))
                    .Where(document => string.Equals(document.Type, "PZ", StringComparison.OrdinalIgnoreCase) || string.Equals(document.Type, "PW", StringComparison.OrdinalIgnoreCase))
                    .Select(document => document.ContractorName!)
                    .FirstOrDefault() ?? "Brak dostawcy";

                var related = relatedDocuments
                    .Take(6)
                    .Select(document => new WarehouseGoodsRelatedDocumentViewModel(
                        document.Number,
                        document.Type,
                        document.DocumentDate.ToLocalTime().ToString("dd.MM HH:mm")))
                    .ToArray();

                return new WarehouseGoodsRowViewModel(
                    product.Id,
                    product.ProductCategoryId,
                    product.UnitOfMeasureId,
                    product.Code,
                    product.Name,
                    product.ProductCategoryName,
                    product.UnitOfMeasureSymbol,
                    supplier,
                    onHand,
                    product.MinimumStockLevel,
                    status,
                    ResolveStatusColor(status),
                    false,
                    product.IsActive,
                    product.LastUpdatedAtUtc.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                    product.Sku ?? "-",
                    preferredStock?.WarehouseName ?? "Brak przypisania",
                    supplier == "Brak dostawcy" ? "Brak danych" : "wg ostatniej dostawy",
                    $"{FormatQuantity(Math.Max(product.MinimumStockLevel * 2m, 1m))} {product.UnitOfMeasureSymbol}",
                    BuildGoodsHint(status, product.Name, product.MinimumStockLevel, available),
                    related,
                    product.RowVersion);
            })
            .ToArray();
    }

    private void ApplyFilters(string? preferredCode = null)
    {
        var filtered = allGoods
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Supplier.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedCategoryFilter == "Wszystkie kategorie" ||
                 string.Equals(item.Category, SelectedCategoryFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedStatusFilter == "Wszystkie statusy" ||
                 string.Equals(item.Status, SelectedStatusFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedUnitFilter == "Wszystkie jednostki" ||
                 string.Equals(item.Unit, SelectedUnitFilter, StringComparison.OrdinalIgnoreCase)) &&
                (!OnlyBelowMinimum || item.OnHand < item.MinimumLevel) &&
                (ShowArchived || !item.IsArchived))
            .ToArray();

        VisibleGoods.Clear();
        foreach (var item in filtered)
        {
            VisibleGoods.Add(item);
        }

        SelectedGoods = VisibleGoods.FirstOrDefault(item => item.Code == preferredCode)
                        ?? VisibleGoods.FirstOrDefault(item => item.Code == SelectedGoods?.Code)
                        ?? VisibleGoods.FirstOrDefault();

        RefreshSummaryCards(filtered);
    }

    private void RefreshSummaryCards(IEnumerable<WarehouseGoodsRowViewModel> goods)
    {
        var rows = goods.ToArray();
        SummaryCards.Clear();
        SummaryCards.Add(new WarehouseSummaryCardViewModel("Kartoteki aktywne", rows.Count(item => item.IsActive).ToString(), "Indeksy dostepne do pracy magazynu i dokumentow.", "#2563EB"));
        SummaryCards.Add(new WarehouseSummaryCardViewModel("Poniżej minimum", rows.Count(item => item.OnHand < item.MinimumLevel).ToString(), "Towary wymagajace uzupelnienia lub przesuniecia.", "#D14343"));
        SummaryCards.Add(new WarehouseSummaryCardViewModel("Kategorie", rows.Select(item => item.Category).Distinct(StringComparer.OrdinalIgnoreCase).Count().ToString(), "Grupy produktowe i logistyczne w kartotekach.", "#1F8A5B"));
        SummaryCards.Add(new WarehouseSummaryCardViewModel("Archiwum", rows.Count(item => item.IsArchived || !item.IsActive).ToString(), "Pozycje wycofane z aktywnego obiegu.", "#7B8794"));
    }

    private void SelectGoods(WarehouseGoodsRowViewModel? goods)
    {
        if (goods is null)
        {
            return;
        }

        SelectedGoods = goods;
    }

    private async Task RunGoodsActionAsync(string? action)
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
                    IsCreatingGoods = true;
                    IsEditMode = true;
                    SelectedGoods = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nową kartotekę produktową do zapisu w SQL.";
                    break;
                case "Edytuj":
                    if (SelectedGoods is null)
                    {
                        return;
                    }

                    IsCreatingGoods = false;
                    IsEditMode = true;
                    LoadEditor();
                    LastActionMessage = $"Wlaczono tryb edycji dla kartoteki {SelectedGoods.Code}.";
                    break;
                case "Eksportuj":
                {
                    if (SelectedGoods is null)
                    {
                        return;
                    }

                    var path = store.ExportGoodsCsv(
                    [
                        new WarehouseGoodsExportRow(
                            SelectedGoods.Code,
                            SelectedGoods.Name,
                            SelectedGoods.Category,
                            SelectedGoods.Unit,
                            FormatQuantity(SelectedGoods.OnHand),
                            FormatQuantity(SelectedGoods.MinimumLevel),
                            SelectedGoods.Status,
                            SelectedGoods.Supplier)
                    ]);
                    LastActionMessage = $"Wyeksportowano kartoteke do pliku: {path}";
                    break;
                }
                case "Archiwizuj":
                {
                    if (SelectedGoods is null)
                    {
                        return;
                    }

                    var request = new WarehouseProductUpdateRequest(
                        SelectedGoods.Id,
                        SelectedGoods.Code,
                        SelectedGoods.Name,
                        SelectedGoods.Barcode == "-" ? null : SelectedGoods.Barcode,
                        false,
                        SelectedGoods.MinimumLevel,
                        SelectedGoods.CategoryId,
                        SelectedGoods.UnitId,
                        SelectedGoods.RowVersion);
                    var updated = await store.UpdateProductAsync(request);
                    LastActionMessage = $"Kartoteke {updated.Code} oznaczono jako nieaktywna.";
                    IsEditMode = false;
                    break;
                }
                case "Usun":
                {
                    if (SelectedGoods is null)
                    {
                        return;
                    }

                    var removedCode = SelectedGoods.Code;
                    await store.DeleteProductAsync(SelectedGoods.Id, SelectedGoods.RowVersion);
                    LastActionMessage = $"Usunieto kartoteke {removedCode} z bazy operacyjnej.";
                    IsCreatingGoods = false;
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
        if (SelectedGoods is null)
        {
            EditorCode = string.Empty;
            EditorName = string.Empty;
            EditorSku = string.Empty;
            EditorMinimum = "0";
            EditorIsActive = true;
            SelectedCategoryOption = categoryEditorOptions.FirstOrDefault();
            SelectedUnitOption = unitEditorOptions.FirstOrDefault();
            return;
        }

        EditorCode = SelectedGoods.Code;
        EditorName = SelectedGoods.Name;
        EditorSku = SelectedGoods.Barcode == "-" ? string.Empty : SelectedGoods.Barcode;
        EditorMinimum = FormatQuantity(SelectedGoods.MinimumLevel);
        EditorIsActive = SelectedGoods.IsActive;
        SelectedCategoryOption = categoryEditorOptions.FirstOrDefault(item => item.Id == SelectedGoods.CategoryId) ?? categoryEditorOptions.FirstOrDefault();
        SelectedUnitOption = unitEditorOptions.FirstOrDefault(item => item.Id == SelectedGoods.UnitId) ?? unitEditorOptions.FirstOrDefault();
    }

    private void CancelGoodsEdit()
    {
        IsCreatingGoods = false;
        IsEditMode = false;
        LoadEditor();
        LastActionMessage = "Anulowano edycje kartoteki.";
    }

    private async Task SaveGoodsChangesAsync()
    {
        try
        {
            var minimum = EditorValueParsers.ParseDecimal(EditorMinimum, "Minimum");
            var categoryId = SelectedCategoryOption?.Id ?? 0;
            var unitId = SelectedUnitOption?.Id ?? 0;

            if (IsCreatingGoods || SelectedGoods is null)
            {
                var created = await store.CreateProductAsync(new WarehouseProductCreateRequest(
                    EditorCode.Trim(),
                    EditorName.Trim(),
                    string.IsNullOrWhiteSpace(EditorSku) ? null : EditorSku.Trim(),
                    EditorIsActive,
                    minimum,
                    categoryId,
                    unitId));

                IsCreatingGoods = false;
                IsEditMode = false;
                LastActionMessage = $"Dodano kartoteke {created.Code} do bazy SQL.";
                return;
            }

            var updated = await store.UpdateProductAsync(new WarehouseProductUpdateRequest(
                SelectedGoods.Id,
                EditorCode.Trim(),
                EditorName.Trim(),
                string.IsNullOrWhiteSpace(EditorSku) ? null : EditorSku.Trim(),
                EditorIsActive,
                minimum,
                categoryId,
                unitId,
                SelectedGoods.RowVersion));

            IsCreatingGoods = false;
            IsEditMode = false;
            LastActionMessage = $"Zapisano kartoteke {updated.Code}.";
        }
        catch (Exception exception)
        {
            LastActionMessage = $"Nie udało się zapisać kartoteki: {TrimExceptionMessage(exception.Message)}";
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
            return "Zarezerwowany";
        }

        if (onHand < minimum)
        {
            return "Poniżej minimum";
        }

        return "Aktywny";
    }

    private static string ResolveStatusColor(string status) => status switch
    {
        "Aktywny" => "#1F8A5B",
        "Poniżej minimum" => "#D97706",
        "Brak" => "#D14343",
        "Zarezerwowany" => "#2563EB",
        "Nieaktywne" => "#7B8794",
        _ => "#7B8794"
    };

    private static string BuildGoodsHint(string status, string productName, decimal minimum, decimal available)
    {
        return status switch
        {
            "Brak" => $"Towar '{productName}' nie ma dostepnego zapasu. Najpierw przyjmij nowa dostawe albo zmniejsz rezerwacje.",
            "Poniżej minimum" => $"Pozycja jest ponizej minimum {FormatQuantity(minimum)}. Dostepne pozostaje {FormatQuantity(available)}.",
            "Zarezerwowany" => "Towar jest dostepny, ale znaczna czesc zapasu jest zablokowana w dokumentach roboczych.",
            "Nieaktywne" => "Kartoteka jest nieaktywna i zostala wycofana z biezacego obiegu.",
            _ => "Kartoteka jest gotowa do normalnej pracy operacyjnej."
        };
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

public sealed record WarehouseGoodsRowViewModel(
    int Id,
    int CategoryId,
    int UnitId,
    string Code,
    string Name,
    string Category,
    string Unit,
    string Supplier,
    decimal OnHand,
    decimal MinimumLevel,
    string Status,
    string StatusColor,
    bool IsArchived,
    bool IsActive,
    string LastUpdated,
    string Barcode,
    string PreferredWarehouse,
    string LeadTime,
    string ReorderQuantity,
    string Note,
    IReadOnlyList<WarehouseGoodsRelatedDocumentViewModel> RelatedDocuments,
    byte[] RowVersion);

public sealed record WarehouseGoodsRelatedDocumentViewModel(
    string Number,
    string Type,
    string Time);

public sealed record WarehouseEditorOptionViewModel(int Id, string Label);
