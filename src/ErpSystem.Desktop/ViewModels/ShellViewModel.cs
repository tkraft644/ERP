using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;
using ErpSystem.Desktop.ViewModels.Warehouse;
using ErpSystem.Shared.Security;

namespace ErpSystem.Desktop.ViewModels;

public sealed partial class ShellViewModel : ViewModelBase
{
    private static readonly SessionService DesignSessionService = new();
    private static readonly HashSet<string> EnabledRoutes = new(StringComparer.OrdinalIgnoreCase)
    {
        "/dashboard",
        "/warehouse/stock",
        "/warehouse/goods",
        "/warehouse/documents"
    };

    private readonly NavigationService navigationService;
    private readonly DialogService dialogService;
    private readonly ApiClient apiClient;
    private readonly ShellMenuBuilder menuBuilder;
    private readonly ThemeService themeService;
    private readonly SessionService sessionService;
    private readonly WarehouseWorkspaceStore warehouseWorkspaceStore;
    private readonly Action? signOutAction;
    private IReadOnlyList<ShellMenuSectionViewModel> allMenuSections = [];
    private long workspaceAccessCounter;
    private const int VisibleWorkspaceLimit = 6;

    public ShellViewModel()
        : this(new NavigationService(), new DialogService(), new ApiClient(DesignSessionService), new ShellMenuBuilder(), new ThemeService(), DesignSessionService, null, null)
    {
    }

    public ShellViewModel(
        NavigationService navigationService,
        DialogService dialogService,
        ApiClient apiClient,
        ShellMenuBuilder menuBuilder,
        ThemeService themeService,
        SessionService sessionService,
        WarehouseWorkspaceStore? warehouseWorkspaceStore = null,
        Action? signOutAction = null)
    {
        this.navigationService = navigationService;
        this.dialogService = dialogService;
        this.apiClient = apiClient;
        this.menuBuilder = menuBuilder;
        this.themeService = themeService;
        this.sessionService = sessionService;
        this.warehouseWorkspaceStore = warehouseWorkspaceStore ?? new WarehouseWorkspaceStore(apiClient);
        this.signOutAction = signOutAction;

        navigationService.Register(BuildWorkspaceDescriptors());

        allMenuSections = BuildMenuSectionsFromSession();
        MenuSections = new ObservableCollection<ShellMenuSectionViewModel>(allMenuSections);
        WorkspaceShortcuts = new ObservableCollection<ShellMenuItemViewModel>(BuildWorkspaceShortcuts());
        Workspaces.CollectionChanged += OnWorkspacesCollectionChanged;

        RefreshApiStatusCommand = new AsyncRelayCommand(RefreshApiStatusAsync);
        SignOutCommand = new RelayCommand(SignOut);
        OpenWorkspaceCommand = new RelayCommand<string>(OpenWorkspace);
        SelectWorkspaceCommand = new RelayCommand<IWorkspace?>(SelectWorkspace);
        CloseWorkspaceCommand = new AsyncRelayCommand<IWorkspace?>(CloseWorkspaceAsync);
        CloseAllWorkspacesCommand = new AsyncRelayCommand(CloseAllWorkspacesAsync, CanCloseAnyWorkspace);
        CloseOtherWorkspacesCommand = new AsyncRelayCommand(CloseOtherWorkspacesAsync, CanCloseOtherWorkspaces);
        TogglePinWorkspaceCommand = new RelayCommand<IWorkspace?>(TogglePinWorkspace, CanTogglePinWorkspace);
        ExecuteQuickActionCommand = new RelayCommand<QuickActionViewModel>(ExecuteQuickAction);
        SelectedThemeMode = themeService.CurrentMode;
        SignedInUser = string.IsNullOrWhiteSpace(sessionService.DisplayName) ? sessionService.UserName : sessionService.DisplayName;
        RoleSummary = sessionService.Roles.Count == 0
            ? "Brak przypisanych ról"
            : string.Join(", ", sessionService.Roles);
        StatusMessage = sessionService.MustChangePassword
            ? "Pierwsze logowanie wymaga zmiany hasła. Etap zmiany hasła będzie kolejnym krokiem."
            : "Sesja zalogowana. Wybierz obszar roboczy z lewego menu.";

        OpenWorkspace("/dashboard");
        _ = RefreshApiStatusAsync();
    }

    public string ApplicationTitle { get; } = "ERP System";
    public string ApplicationSubtitle { get; } = "Praca na realnych danych z API i bazy SQL Server";
    public string EnvironmentLabel { get; } = "Synchronizacja";
    public string LeftMenuCaption { get; } = "Moduły i widoki robocze";
    public string ApiEndpointLabel => apiClient.BaseAddress;
    public string LocalTimeLabel => DateTime.Now.ToString("dd.MM.yyyy HH:mm");
    public IReadOnlyList<string> ThemeModes => themeService.Options;
    public string EmptyMenuMessage => "Brak funkcji dla podanego filtra.";
    public bool HasMenuResults => MenuSections.Count > 0;
    public bool ShowEmptyMenuMessage => !HasMenuResults;
    public IReadOnlyList<WorkspaceViewModelBase> VisibleWorkspaces => BuildVisibleWorkspaces();
    public IReadOnlyList<WorkspaceViewModelBase> OverflowWorkspaces => BuildOverflowWorkspaces();
    public bool HasOverflowWorkspaces => OverflowWorkspaces.Count > 0;
    public string OverflowWorkspaceLabel => $"Więcej ({OverflowWorkspaces.Count})";
    public IReadOnlyList<WorkspaceViewModelBase> RecentWorkspaces => BuildRecentWorkspaces();
    public bool HasRecentWorkspaces => RecentWorkspaces.Count > 0;
    public IReadOnlyList<QuickActionViewModel> QuickActions => BuildQuickActions();
    public string QuickActionCaption => $"{CurrentModuleTitle} · szybkie akcje";
    public bool CanCloseSelectedWorkspace => SelectedWorkspace?.CanClose == true;
    public string PinSelectedWorkspaceLabel => SelectedWorkspaceBase?.IsPinned == true ? "Odepnij zakładkę" : "Przypnij zakładkę";

    public ObservableCollection<ShellMenuSectionViewModel> MenuSections { get; }
    public ObservableCollection<ShellMenuItemViewModel> WorkspaceShortcuts { get; }
    public ObservableCollection<IWorkspace> Workspaces { get; } = [];

    [ObservableProperty]
    private IWorkspace? selectedWorkspace;

    [ObservableProperty]
    private string apiStatusLabel = "Łączenie z API";

    [ObservableProperty]
    private string menuSearchText = string.Empty;

    [ObservableProperty]
    private string selectedThemeMode = "System";

    [ObservableProperty]
    private WorkspaceViewModelBase? selectedOverflowWorkspace;

    [ObservableProperty]
    private WorkspaceViewModelBase? selectedRecentWorkspace;

    [ObservableProperty]
    private string signedInUser = string.Empty;

    [ObservableProperty]
    private string roleSummary = string.Empty;

    [ObservableProperty]
    private string statusMessage = "Ekran gotowy. Wybierz zakładkę, filtr lub szybką akcję.";

    public ViewModelBase? ActiveWorkspace => SelectedWorkspace as ViewModelBase;
    public string CurrentWorkspaceTitle => SelectedWorkspace?.Title ?? "Brak aktywnej zakładki";
    public string CurrentModuleTitle => SelectedWorkspaceBase?.ModuleTitle ?? "Panel główny";
    public string WorkspaceHint => SelectedWorkspaceBase?.Subtitle ?? "Wybierz ekran z lewego menu albo skorzystaj z szybkich akcji.";
    public string WorkspaceCountLabel => $"{Workspaces.Count} otwartych zakładek";

    public IAsyncRelayCommand RefreshApiStatusCommand { get; }
    public IRelayCommand SignOutCommand { get; }
    public IRelayCommand<string> OpenWorkspaceCommand { get; }
    public IRelayCommand<IWorkspace?> SelectWorkspaceCommand { get; }
    public IAsyncRelayCommand<IWorkspace?> CloseWorkspaceCommand { get; }
    public IAsyncRelayCommand CloseAllWorkspacesCommand { get; }
    public IAsyncRelayCommand CloseOtherWorkspacesCommand { get; }
    public IRelayCommand<IWorkspace?> TogglePinWorkspaceCommand { get; }
    public IRelayCommand<QuickActionViewModel> ExecuteQuickActionCommand { get; }

    private WorkspaceViewModelBase? SelectedWorkspaceBase => SelectedWorkspace as WorkspaceViewModelBase;

    partial void OnSelectedWorkspaceChanged(IWorkspace? value)
    {
        var selectedRoute = GetRoute(value);

        foreach (var workspace in Workspaces.OfType<WorkspaceViewModelBase>())
        {
            workspace.IsSelected = ReferenceEquals(workspace, value);
        }

        UpdateSelection(MenuSections.SelectMany(section => section.Items), selectedRoute);
        UpdateSelection(WorkspaceShortcuts, selectedRoute);

        OnPropertyChanged(nameof(ActiveWorkspace));
        OnPropertyChanged(nameof(CurrentWorkspaceTitle));
        OnPropertyChanged(nameof(CurrentModuleTitle));
        OnPropertyChanged(nameof(WorkspaceHint));
        OnPropertyChanged(nameof(QuickActions));
        OnPropertyChanged(nameof(QuickActionCaption));
        OnPropertyChanged(nameof(CanCloseSelectedWorkspace));
        OnPropertyChanged(nameof(PinSelectedWorkspaceLabel));
        CloseOtherWorkspacesCommand.NotifyCanExecuteChanged();
        TogglePinWorkspaceCommand.NotifyCanExecuteChanged();
    }

    partial void OnMenuSearchTextChanged(string value)
    {
        ApplyMenuFilter();
    }

    partial void OnSelectedThemeModeChanged(string value)
    {
        themeService.Apply(value);
    }

    partial void OnSelectedOverflowWorkspaceChanged(WorkspaceViewModelBase? value)
    {
        if (value is null)
        {
            return;
        }

        SelectWorkspace(value);
        SelectedOverflowWorkspace = null;
    }

    partial void OnSelectedRecentWorkspaceChanged(WorkspaceViewModelBase? value)
    {
        if (value is null)
        {
            return;
        }

        SelectWorkspace(value);
        SelectedRecentWorkspace = null;
    }

    private IReadOnlyList<ShellMenuSectionViewModel> BuildMenuSectionsFromSession()
    {
        var builtSections = menuBuilder.Build(sessionService.Permissions)
            .Select(section => new ShellMenuBuilder.MenuSectionDescriptor(
                section.Key,
                section.Title,
                section.Items
                    .Where(item => EnabledRoutes.Contains(item.Route))
                    .ToArray()))
            .Where(section => section.Items.Count > 0)
            .ToList();

        builtSections.Insert(
            0,
            new ShellMenuBuilder.MenuSectionDescriptor(
                "start",
                "Start",
                [new ShellMenuBuilder.MenuItemDescriptor("Dashboard", "/dashboard")]));

        return MapMenuSections(builtSections);
    }

    private IReadOnlyList<ShellMenuItemViewModel> BuildWorkspaceShortcuts()
    {
        string[] routes =
        [
            "/dashboard",
            "/warehouse/stock",
            "/warehouse/documents",
            "/warehouse/goods"
        ];

        return routes
            .Select(route =>
            {
                var descriptor = navigationService.GetDescriptor(route)
                    ?? throw new InvalidOperationException($"Workspace descriptor '{route}' was not registered.");
                return new ShellMenuItemViewModel(descriptor.Title, descriptor.Subtitle, descriptor.Route, OpenMenuItem);
            })
            .ToArray();
    }

    private IReadOnlyList<ShellMenuSectionViewModel> MapMenuSections(
        IReadOnlyList<ShellMenuBuilder.MenuSectionDescriptor> sections)
    {
        return sections
            .Select(section => new ShellMenuSectionViewModel(
                section.Key,
                LocalizeSectionTitle(section.Key, section.Title),
                section.Items
                    .Select(item =>
                    {
                        var descriptor = navigationService.GetDescriptor(item.Route);
                        return new ShellMenuItemViewModel(
                            descriptor?.Title ?? item.Title,
                            descriptor?.Subtitle ?? item.Route,
                            item.Route,
                            OpenMenuItem);
                    })
                    .ToArray()))
            .ToArray();
    }

    private IReadOnlyList<WorkspaceViewModelBase> BuildVisibleWorkspaces()
    {
        return GetWorkspaceDisplayOrder()
            .Take(VisibleWorkspaceLimit)
            .ToArray();
    }

    private IReadOnlyList<WorkspaceViewModelBase> BuildOverflowWorkspaces()
    {
        return GetWorkspaceDisplayOrder()
            .Skip(VisibleWorkspaceLimit)
            .ToArray();
    }

    private IReadOnlyList<WorkspaceViewModelBase> BuildRecentWorkspaces()
    {
        return Workspaces
            .OfType<WorkspaceViewModelBase>()
            .OrderByDescending(item => item.LastAccessSequence)
            .Take(8)
            .ToArray();
    }

    private IReadOnlyList<WorkspaceViewModelBase> GetWorkspaceDisplayOrder()
    {
        return Workspaces
            .OfType<WorkspaceViewModelBase>()
            .OrderByDescending(item => item.IsPinned)
            .ThenByDescending(item => item.LastAccessSequence)
            .ToArray();
    }

    private IReadOnlyList<QuickActionViewModel> BuildQuickActions()
    {
        var route = GetRoute(SelectedWorkspace) ?? string.Empty;

        if (route.StartsWith("/system", StringComparison.OrdinalIgnoreCase))
        {
            return [];
        }

        if (route.StartsWith("/warehouse", StringComparison.OrdinalIgnoreCase))
        {
            return
            [
                new QuickActionViewModel("Nowy dokument PZ", "Przyjecie zewnetrzne", "/warehouse/documents"),
                new QuickActionViewModel("Nowy dokument WZ", "Wydanie zewnetrzne", "/warehouse/documents"),
                new QuickActionViewModel("Przesuniecie MM", "Ruch miedzymagazynowy", "/warehouse/documents"),
                new QuickActionViewModel("Dodaj produkt", "Nowa kartoteka towarowa", "/warehouse/goods"),
                new QuickActionViewModel("Odśwież dane", "Pobierz bieżący stan z API", null, "Odśwież widok, aby pobrać najnowsze dane z serwera.")
            ];
        }

        return
        [
            new QuickActionViewModel("Stany magazynowe", "Zobacz dostępność i rezerwacje", "/warehouse/stock"),
            new QuickActionViewModel("Kartoteki towarowe", "Dodaj pierwszy produkt albo popraw istniejący", "/warehouse/goods"),
            new QuickActionViewModel("Dokumenty magazynowe", "PZ, WZ, MM, RW, PW, INW", "/warehouse/documents")
        ];
    }

    private IReadOnlyList<NavigationService.WorkspaceDescriptor> BuildWorkspaceDescriptors()
    {
        return
        [
            new("/dashboard", "Dashboard", "Start pracy na realnej bazie danych i podsumowaniu procesów.", () => new DashboardWorkspaceViewModel(apiClient, OpenWorkspace)),
            new("/warehouse/goods", "Kartoteki towarowe", "Indeksy, kategorie i statusy towarow dostepnych dla magazynu.", () => new WarehouseGoodsWorkspaceViewModel(warehouseWorkspaceStore)),
            new("/warehouse/documents", "Lista dokumentow magazynowych", "PZ, WZ, MM, RW, PW i INW z filtrami, statusem i archiwum.", () => new WarehouseDocumentListWorkspaceViewModel(warehouseWorkspaceStore)),
            new("/warehouse/stock", "Stany magazynowe", "Dashboard pracy magazynu z tabela, brakami i dokumentami do reakcji.", () => new WarehouseStockWorkspaceViewModel(warehouseWorkspaceStore, OpenWorkspace))
        ];
    }

    private static NavigationService.WorkspaceDescriptor CreateSystemWorkspace(
        string route,
        string title,
        string subtitle,
        bool canClose,
        IReadOnlyList<WorkspaceMetricViewModel> metrics,
        IReadOnlyList<WorkspaceActionViewModel> actions,
        IReadOnlyList<WorkspaceActivityViewModel> activity)
    {
        return CreateModuleWorkspace(
            route,
            "System",
            title,
            subtitle,
            canClose,
            "Obszar systemowy skupia dostep, slowniki, audit i wspolne kontrolki platformy ERP.",
            "Core",
            "#4C6FFF",
            "#EEF2FF",
            metrics,
            actions,
            activity);
    }

    private static NavigationService.WorkspaceDescriptor CreateModuleWorkspace(
        string route,
        string moduleTitle,
        string title,
        string subtitle,
        bool canClose,
        string description,
        string badgeText,
        string accentColor,
        string accentSurfaceColor,
        IReadOnlyList<WorkspaceMetricViewModel> metrics,
        IReadOnlyList<WorkspaceActionViewModel> actions,
        IReadOnlyList<WorkspaceActivityViewModel> activity)
    {
        return new NavigationService.WorkspaceDescriptor(
            route,
            title,
            subtitle,
            () => new ModuleWorkspaceViewModel(
                route,
                moduleTitle,
                title,
                subtitle,
                canClose,
                description,
                badgeText,
                accentColor,
                accentSurfaceColor,
                metrics,
                actions,
                activity));
    }

    private void OpenMenuItem(ShellMenuItemViewModel item) => OpenWorkspace(item.Route);

    private void OpenWorkspace(string? route)
    {
        if (string.IsNullOrWhiteSpace(route))
        {
            return;
        }

        var existing = Workspaces
            .OfType<WorkspaceViewModelBase>()
            .FirstOrDefault(item => string.Equals(item.Route, route, StringComparison.OrdinalIgnoreCase));

        if (existing is not null)
        {
            MarkWorkspaceAsUsed(existing);
            SelectedWorkspace = existing;
            StatusMessage = $"Przełączono na zakładkę '{existing.Title}'.";
            return;
        }

        var workspace = navigationService.Create(route);
        if (workspace is null)
        {
            return;
        }

        Workspaces.Add(workspace);
        if (workspace is WorkspaceViewModelBase createdWorkspace)
        {
            MarkWorkspaceAsUsed(createdWorkspace);
        }
        SelectedWorkspace = workspace;
        StatusMessage = $"Otwarto zakładkę '{workspace.Title}'.";
    }

    private void SelectWorkspace(IWorkspace? workspace)
    {
        if (workspace is null)
        {
            return;
        }

        if (workspace is WorkspaceViewModelBase workspaceViewModel)
        {
            MarkWorkspaceAsUsed(workspaceViewModel);
        }

        SelectedWorkspace = workspace;
        StatusMessage = $"Wybrano zakładkę '{workspace.Title}'.";
    }

    private async Task CloseWorkspaceAsync(IWorkspace? workspace)
    {
        if (workspace is null || !workspace.CanClose)
        {
            return;
        }

        if (!await dialogService.ConfirmTabCloseAsync(workspace.Title))
        {
            return;
        }

        var index = Workspaces.IndexOf(workspace);
        if (index < 0)
        {
            return;
        }

        var wasSelected = ReferenceEquals(SelectedWorkspace, workspace);
        Workspaces.RemoveAt(index);

        if (!wasSelected)
        {
            StatusMessage = $"Zamknięto zakładkę '{workspace.Title}'.";
            return;
        }

        if (Workspaces.Count == 0)
        {
            SelectedWorkspace = null;
            StatusMessage = $"Zamknięto zakładkę '{workspace.Title}'.";
            return;
        }

        SelectedWorkspace = Workspaces[Math.Min(index, Workspaces.Count - 1)];
        StatusMessage = $"Zamknięto zakładkę '{workspace.Title}'.";
    }

    private async Task CloseAllWorkspacesAsync()
    {
        var closable = Workspaces
            .OfType<WorkspaceViewModelBase>()
            .Where(item => !item.IsPinned)
            .ToArray();

        if (closable.Length == 0)
        {
            return;
        }

        if (!await dialogService.ConfirmTabCloseAsync($"wszystkie nieprzypiete zakladki ({closable.Length})"))
        {
            return;
        }

        foreach (var workspace in closable)
        {
            Workspaces.Remove(workspace);
        }

        if (SelectedWorkspace is not null && !Workspaces.Contains(SelectedWorkspace))
        {
            SelectedWorkspace = Workspaces.FirstOrDefault();
        }

        StatusMessage = $"Zamknięto {closable.Length} zakładek.";
        RefreshWorkspaceCollections();
    }

    private async Task CloseOtherWorkspacesAsync()
    {
        if (SelectedWorkspace is not WorkspaceViewModelBase selected)
        {
            return;
        }

        var closable = Workspaces
            .OfType<WorkspaceViewModelBase>()
            .Where(item => !ReferenceEquals(item, selected) && !item.IsPinned)
            .ToArray();

        if (closable.Length == 0)
        {
            return;
        }

        if (!await dialogService.ConfirmTabCloseAsync($"pozostale zakladki ({closable.Length})"))
        {
            return;
        }

        foreach (var workspace in closable)
        {
            Workspaces.Remove(workspace);
        }

        StatusMessage = $"Pozostawiono tylko '{selected.Title}' i przypięte zakładki.";
        RefreshWorkspaceCollections();
    }

    private void TogglePinWorkspace(IWorkspace? workspace)
    {
        if (workspace is not WorkspaceViewModelBase workspaceViewModel)
        {
            return;
        }

        workspaceViewModel.IsPinned = !workspaceViewModel.IsPinned;
        MarkWorkspaceAsUsed(workspaceViewModel);
        StatusMessage = workspaceViewModel.IsPinned
            ? $"Przypięto zakładkę '{workspaceViewModel.Title}'."
            : $"Odpięto zakładkę '{workspaceViewModel.Title}'.";
        RefreshWorkspaceCollections();
    }

    private void ExecuteQuickAction(QuickActionViewModel? action)
    {
        if (action is null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(action.Route))
        {
            OpenWorkspace(action.Route);
        }

        if (!string.IsNullOrWhiteSpace(action.StatusMessage))
        {
            StatusMessage = action.StatusMessage;
            return;
        }

        StatusMessage = $"Uruchomiono szybka akcje '{action.Title}'.";
    }

    private async Task RefreshApiStatusAsync()
    {
        var snapshot = await apiClient.GetHealthAsync();
        ApiStatusLabel = snapshot.Label;
        StatusMessage = snapshot.Description;
    }

    private void SignOut()
    {
        signOutAction?.Invoke();
    }

    private void OnWorkspacesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshWorkspaceCollections();
    }

    private void ReplaceMenuSections(IReadOnlyList<ShellMenuSectionViewModel> sections)
    {
        MenuSections.Clear();

        foreach (var section in sections)
        {
            MenuSections.Add(section);
        }

        UpdateSelection(MenuSections.SelectMany(section => section.Items), GetRoute(SelectedWorkspace));
        OnPropertyChanged(nameof(HasMenuResults));
        OnPropertyChanged(nameof(ShowEmptyMenuMessage));
        OnPropertyChanged(nameof(EmptyMenuMessage));
    }

    private void RefreshWorkspaceCollections()
    {
        OnPropertyChanged(nameof(WorkspaceCountLabel));
        OnPropertyChanged(nameof(VisibleWorkspaces));
        OnPropertyChanged(nameof(OverflowWorkspaces));
        OnPropertyChanged(nameof(HasOverflowWorkspaces));
        OnPropertyChanged(nameof(OverflowWorkspaceLabel));
        OnPropertyChanged(nameof(RecentWorkspaces));
        OnPropertyChanged(nameof(HasRecentWorkspaces));
        OnPropertyChanged(nameof(CanCloseSelectedWorkspace));
        OnPropertyChanged(nameof(PinSelectedWorkspaceLabel));
        CloseAllWorkspacesCommand.NotifyCanExecuteChanged();
        CloseOtherWorkspacesCommand.NotifyCanExecuteChanged();
        TogglePinWorkspaceCommand.NotifyCanExecuteChanged();
    }

    private void MarkWorkspaceAsUsed(WorkspaceViewModelBase workspace)
    {
        workspace.LastAccessSequence = ++workspaceAccessCounter;
        RefreshWorkspaceCollections();
    }

    private void ApplyMenuFilter()
    {
        if (string.IsNullOrWhiteSpace(MenuSearchText))
        {
            ReplaceMenuSections(allMenuSections);
            return;
        }

        var query = MenuSearchText.Trim();
        var filtered = allMenuSections
            .Select(section =>
            {
                var sectionMatches = section.Title.Contains(query, StringComparison.OrdinalIgnoreCase);
                var items = section.Items
                    .Where(item =>
                        sectionMatches ||
                        item.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        item.Subtitle.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                return items.Length == 0
                    ? null
                    : new ShellMenuSectionViewModel(section.Key, section.Title, items, true);
            })
            .Where(section => section is not null)
            .Cast<ShellMenuSectionViewModel>()
            .ToArray();

        ReplaceMenuSections(filtered);
    }

    private static void UpdateSelection(IEnumerable<ShellMenuItemViewModel> items, string? selectedRoute)
    {
        foreach (var item in items)
        {
            item.IsSelected = selectedRoute is not null &&
                              string.Equals(item.Route, selectedRoute, StringComparison.OrdinalIgnoreCase);
        }
    }

    private bool CanCloseAnyWorkspace()
    {
        return Workspaces
            .OfType<WorkspaceViewModelBase>()
            .Any(item => !item.IsPinned);
    }

    private bool CanCloseOtherWorkspaces()
    {
        if (SelectedWorkspace is not WorkspaceViewModelBase selected)
        {
            return false;
        }

        return Workspaces
            .OfType<WorkspaceViewModelBase>()
            .Any(item => !ReferenceEquals(item, selected) && !item.IsPinned);
    }

    private bool CanTogglePinWorkspace(IWorkspace? workspace) => workspace is WorkspaceViewModelBase;

    private static string LocalizeSectionTitle(string key, string fallback) => key switch
    {
        "start" => "Start",
        "system" => "System",
        "contractors" => "Kontrahenci",
        "warehouse" => "Magazyn",
        "transport" => "Transport",
        "hr" => "HR",
        "finance" => "Finanse",
        "documents" => "Dokumenty",
        _ => fallback
    };

    private static string? GetRoute(IWorkspace? workspace) => (workspace as WorkspaceViewModelBase)?.Route;
}

public sealed record QuickActionViewModel(
    string Title,
    string Caption,
    string? Route,
    string? StatusMessage = null);
