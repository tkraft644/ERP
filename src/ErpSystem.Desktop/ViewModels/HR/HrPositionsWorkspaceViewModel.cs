using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.HR;

public sealed partial class HrPositionsWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly HrWorkspaceStore store = HrWorkspaceStore.Instance;

    public HrPositionsWorkspaceViewModel()
        : base(
            "/hr/positions",
            "HR",
            "Stanowiska",
            "Stanowiska i role operacyjne używane w umowach oraz kartach pracownika.",
            false)
    {
        SummaryCards = new ObservableCollection<HrSummaryCardViewModel>();
        VisiblePositions = new ObservableCollection<HrPositionRowViewModel>();
        RunPositionActionCommand = new RelayCommand<string>(RunPositionAction);
        SelectPositionCommand = new RelayCommand<HrPositionRowViewModel>(SelectPosition);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<HrSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<HrPositionRowViewModel> VisiblePositions { get; }
    public IRelayCommand<string> RunPositionActionCommand { get; }
    public IRelayCommand<HrPositionRowViewModel> SelectPositionCommand { get; }

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool showInactive = true;

    [ObservableProperty]
    private HrPositionRowViewModel? selectedPosition;

    [ObservableProperty]
    private int? selectedPositionId;

    [ObservableProperty]
    private string editorCode = string.Empty;

    [ObservableProperty]
    private string editorName = string.Empty;

    [ObservableProperty]
    private string editorDescription = string.Empty;

    [ObservableProperty]
    private bool editorIsActive = true;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz stanowisko lub dodaj nowe.";

    public bool HasSelectedPosition => SelectedPositionId is not null;

    partial void OnSearchTextChanged(string value) => RefreshPositions();
    partial void OnShowInactiveChanged(bool value) => RefreshPositions();

    partial void OnSelectedPositionChanged(HrPositionRowViewModel? value)
    {
        SelectedPositionId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedPositionIdChanged(int? value) => OnPropertyChanged(nameof(HasSelectedPosition));

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshPositions();
        LoadEditor();
    }

    private void RefreshSummaryCards()
    {
        var positions = store.GetPositions();
        var employees = store.GetEmployees();
        var drivers = employees.Count(item => item.HasDriverProfile);

        SummaryCards.Clear();
        SummaryCards.Add(new HrSummaryCardViewModel("Stanowiska aktywne", positions.Count(item => item.IsActive).ToString(), "Role dostępne dla nowych umów i pracowników.", "#1D4ED8"));
        SummaryCards.Add(new HrSummaryCardViewModel("Obsadzone", employees.Count(item => item.PositionId is not null).ToString(), "Pracownicy przypisani do stanowisk.", "#1F8A5B"));
        SummaryCards.Add(new HrSummaryCardViewModel("Kierowcy", drivers.ToString(), "Stanowiska powiązane z profilem kierowcy.", "#D97706"));
        SummaryCards.Add(new HrSummaryCardViewModel("Nieaktywne", positions.Count(item => !item.IsActive).ToString(), "Stanowiska wycofane z bieżącego obiegu.", "#D14343"));
    }

    private void RefreshPositions()
    {
        var rows = store.GetPositions()
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (ShowInactive || item.IsActive))
            .Select(item => new HrPositionRowViewModel(
                item.Id,
                item.Code,
                item.Name,
                item.Description,
                item.StatusLabel,
                item.StatusColor,
                store.GetEmployees().Count(employee => employee.PositionId == item.Id)))
            .ToArray();

        VisiblePositions.Clear();
        foreach (var row in rows)
        {
            VisiblePositions.Add(row);
        }

        SelectedPosition = SelectedPositionId is not null
            ? VisiblePositions.FirstOrDefault(item => item.Id == SelectedPositionId) ?? VisiblePositions.FirstOrDefault()
            : VisiblePositions.FirstOrDefault();
    }

    private void LoadEditor()
    {
        if (SelectedPositionId is null)
        {
            EditorCode = string.Empty;
            EditorName = string.Empty;
            EditorDescription = string.Empty;
            EditorIsActive = true;
            return;
        }

        var position = store.GetPositions().FirstOrDefault(item => item.Id == SelectedPositionId);
        if (position is null)
        {
            return;
        }

        EditorCode = position.Code;
        EditorName = position.Name;
        EditorDescription = position.Description;
        EditorIsActive = position.IsActive;
    }

    private void RunPositionAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedPosition = null;
                    SelectedPositionId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nowe stanowisko.";
                    break;
                case "Save":
                    SavePosition();
                    break;
                case "Toggle":
                    if (SelectedPositionId is not null)
                    {
                        store.TogglePosition(SelectedPositionId.Value);
                        LastActionMessage = "Zmieniono aktywność stanowiska.";
                    }
                    break;
                case "Delete":
                    if (SelectedPositionId is not null)
                    {
                        store.DeletePosition(SelectedPositionId.Value);
                        SelectedPositionId = null;
                        SelectedPosition = null;
                        LoadEditor();
                        LastActionMessage = "Usunięto stanowisko.";
                    }
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }

    private void SavePosition()
    {
        if (SelectedPositionId is null)
        {
            var created = store.CreatePosition(EditorCode, EditorName, EditorDescription, EditorIsActive);
            SelectedPositionId = created.Id;
            LastActionMessage = $"Dodano stanowisko {created.Name}.";
            return;
        }

        var updated = store.UpdatePosition(SelectedPositionId.Value, EditorCode, EditorName, EditorDescription, EditorIsActive);
        LastActionMessage = $"Zapisano stanowisko {updated.Name}.";
    }

    private void SelectPosition(HrPositionRowViewModel? position)
    {
        SelectedPosition = position;
    }
}

public sealed record HrPositionRowViewModel(
    int Id,
    string Code,
    string Name,
    string Description,
    string StatusLabel,
    string StatusColor,
    int EmployeeCount);
