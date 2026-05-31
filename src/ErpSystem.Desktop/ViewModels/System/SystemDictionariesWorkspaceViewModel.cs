using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.System;

public sealed partial class SystemDictionariesWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly SystemWorkspaceStore store = SystemWorkspaceStore.Instance;

    public SystemDictionariesWorkspaceViewModel()
        : base(
            "/system/dictionaries",
            "System",
            "Dictionaries",
            "Słowniki wspólne dla transportu, magazynu, HR i pozostałych modułów systemu.",
            false)
    {
        SummaryCards = new ObservableCollection<SystemSummaryCardViewModel>();
        VisibleItems = new ObservableCollection<SystemDictionaryRecord>();
        RunDictionaryActionCommand = new RelayCommand<string>(RunDictionaryAction);
        SelectDictionaryCommand = new RelayCommand<SystemDictionaryRecord>(item => SelectedItem = item);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<SystemSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<SystemDictionaryRecord> VisibleItems { get; }
    public IRelayCommand<string> RunDictionaryActionCommand { get; }
    public IRelayCommand<SystemDictionaryRecord> SelectDictionaryCommand { get; }

    public IReadOnlyList<string> AreaFilterOptions => ["Wszystkie obszary", .. store.GetDictionaries().Select(item => item.Area).Distinct().OrderBy(item => item)];
    public IReadOnlyList<string> CategoryFilterOptions => ["Wszystkie kategorie", .. store.GetDictionaries().Select(item => item.Category).Distinct().OrderBy(item => item)];

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedAreaFilter = "Wszystkie obszary";

    [ObservableProperty]
    private string selectedCategoryFilter = "Wszystkie kategorie";

    [ObservableProperty]
    private bool showArchived;

    [ObservableProperty]
    private SystemDictionaryRecord? selectedItem;

    [ObservableProperty]
    private int? selectedItemId;

    [ObservableProperty]
    private string editorArea = string.Empty;

    [ObservableProperty]
    private string editorCategory = string.Empty;

    [ObservableProperty]
    private string editorCode = string.Empty;

    [ObservableProperty]
    private string editorValue = string.Empty;

    [ObservableProperty]
    private string editorDescription = string.Empty;

    [ObservableProperty]
    private bool editorIsActive = true;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz wpis słownikowy albo utwórz nowy, aby utrzymać dane referencyjne.";

    public bool HasSelectedItem => SelectedItemId is not null;

    partial void OnSearchTextChanged(string value) => RefreshItems();
    partial void OnSelectedAreaFilterChanged(string value) => RefreshItems();
    partial void OnSelectedCategoryFilterChanged(string value) => RefreshItems();
    partial void OnShowArchivedChanged(bool value) => RefreshItems();

    partial void OnSelectedItemChanged(SystemDictionaryRecord? value)
    {
        SelectedItemId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedItemIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasSelectedItem));
    }

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshItems();
        LoadEditor();
        OnPropertyChanged(nameof(AreaFilterOptions));
        OnPropertyChanged(nameof(CategoryFilterOptions));
    }

    private void RefreshSummaryCards()
    {
        var items = store.GetDictionaries();
        SummaryCards.Clear();
        SummaryCards.Add(new SystemSummaryCardViewModel("Aktywne wpisy", items.Count(item => item.IsActive && !item.IsArchived).ToString(), "Pozycje gotowe do użycia przez moduły.", "#2563EB"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Kategorie", items.Select(item => item.Category).Distinct().Count().ToString(), "Zakres słowników dostępnych w systemie.", "#1F8A5B"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Archiwum", items.Count(item => item.IsArchived).ToString(), "Wpisy wycofane, ale zachowane historycznie.", "#D97706"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Obszary", items.Select(item => item.Area).Distinct().Count().ToString(), "Moduły korzystające ze słowników.", "#D14343"));
    }

    private void RefreshItems()
    {
        var items = store.GetDictionaries()
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Value.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedAreaFilter == "Wszystkie obszary" ||
                 string.Equals(item.Area, SelectedAreaFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedCategoryFilter == "Wszystkie kategorie" ||
                 string.Equals(item.Category, SelectedCategoryFilter, StringComparison.OrdinalIgnoreCase)) &&
                (ShowArchived || !item.IsArchived))
            .ToArray();

        VisibleItems.Clear();
        foreach (var item in items)
        {
            VisibleItems.Add(item);
        }

        SelectedItem = SelectedItemId is not null
            ? VisibleItems.FirstOrDefault(item => item.Id == SelectedItemId) ?? VisibleItems.FirstOrDefault()
            : VisibleItems.FirstOrDefault();
    }

    private void LoadEditor()
    {
        if (SelectedItemId is null)
        {
            EditorArea = string.Empty;
            EditorCategory = string.Empty;
            EditorCode = string.Empty;
            EditorValue = string.Empty;
            EditorDescription = string.Empty;
            EditorIsActive = true;
            return;
        }

        var item = store.GetDictionaries().FirstOrDefault(entry => entry.Id == SelectedItemId);
        if (item is null)
        {
            return;
        }

        EditorArea = item.Area;
        EditorCategory = item.Category;
        EditorCode = item.Code;
        EditorValue = item.Value;
        EditorDescription = item.Description;
        EditorIsActive = item.IsActive;
    }

    private void RunDictionaryAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedItem = null;
                    SelectedItemId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nowy wpis słownikowy.";
                    break;
                case "Save":
                    SaveDictionary();
                    break;
                case "Archive":
                    if (SelectedItemId is not null)
                    {
                        store.ArchiveDictionary(SelectedItemId.Value);
                        LastActionMessage = "Zarchiwizowano wpis słownikowy.";
                    }
                    break;
                case "Restore":
                    if (SelectedItemId is not null)
                    {
                        store.RestoreDictionary(SelectedItemId.Value);
                        LastActionMessage = "Przywrócono wpis słownikowy.";
                    }
                    break;
                case "Delete":
                    if (SelectedItemId is not null)
                    {
                        store.DeleteDictionary(SelectedItemId.Value);
                        SelectedItemId = null;
                        SelectedItem = null;
                        LoadEditor();
                        LastActionMessage = "Usunięto wpis słownikowy.";
                    }
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }

    private void SaveDictionary()
    {
        if (SelectedItemId is null)
        {
            var created = store.CreateDictionary(EditorArea, EditorCategory, EditorCode, EditorValue, EditorDescription, EditorIsActive);
            SelectedItemId = created.Id;
            LastActionMessage = $"Dodano wpis {created.Code}.";
            return;
        }

        var updated = store.UpdateDictionary(SelectedItemId.Value, EditorArea, EditorCategory, EditorCode, EditorValue, EditorDescription, EditorIsActive);
        LastActionMessage = $"Zapisano wpis {updated.Code}.";
    }
}
